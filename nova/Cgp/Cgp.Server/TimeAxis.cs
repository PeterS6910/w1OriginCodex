using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using System.Threading;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server
{
    public class TimeAxis
    {
        public const long TOTAL_DAY_MILISECONDS = 86400000;

        public const byte ON = 1;
        public const byte OFF = 0;
        public const byte NOT_SET = 10;

        private static volatile TimeAxis _singleton = null;
        private static object _syncRoot = new object();

        private System.Threading.Timer _axisTimer = null;
        LinkedList<TimeNode> _axisNodes = new LinkedList<TimeNode>();

        public event Action<Guid, byte> TimeZoneStateChanged;
        public event Action<Guid, byte> DailyPlanStateChanged;

        /// <summary>
        /// Stores information about daily plan and time zones, which use it
        /// </summary>
        private SyncDictionary<Guid, LinkedList<Guid>> _dailyPlanReferencedTimeZones = new SyncDictionary<Guid, LinkedList<Guid>>();
        /// <summary>
        /// Stores information about time zone and its daily plans
        /// </summary>
        private SyncDictionary<Guid, LinkedList<Guid>> _timeZoneDailyPlans = new SyncDictionary<Guid, LinkedList<Guid>>();
        /// <summary>
        /// Stores state info for each daily plan
        /// </summary>
        private SyncDictionary<Guid, byte> _dailyPlanStates = new SyncDictionary<Guid, byte>();
        /// <summary>
        /// Stores state info for each time zone
        /// </summary>
        private SyncDictionary<Guid, byte> _timeZoneStates = new SyncDictionary<Guid, byte>();

        public TimeAxis()
        {
            Microsoft.Win32.SystemEvents.TimeChanged += SystemEvents_TimeChanged;
        }

        void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            StartOrReset();
        }

        public static TimeAxis Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new TimeAxis();
                    }

                return _singleton;
            }
        }

        /// <summary>
        /// Returns actual time in miliseconds
        /// </summary>
        /// <returns></returns>
        private long GetActualTimeMilisecond()
        {
            return (long)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public void StartOrReset()
        {
            if (_axisTimer != null)
                _axisTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ClearExistingRecords();
            FillTimeAxisAndSetCurrentStates();
            PrepareNextNodeExecution();
        }

        /// <summary>
        /// Removes all existing time axis records
        /// </summary>
        private void ClearExistingRecords()
        {
            if (_axisNodes != null)
                _axisNodes.Clear();
            //ClearStateRecords();
            ClearDependencyRecords();
        }

        /// <summary>
        /// Removes all daily plan and time zone state information
        /// </summary>
        private void ClearStateRecords()
        {
            _dailyPlanStates.Clear();
            _timeZoneStates.Clear();
        }

        /// <summary>
        /// Removes information about time zones and daily plans dependency
        /// </summary>
        private void ClearDependencyRecords()
        {
            _dailyPlanReferencedTimeZones.Clear(
                (key, value) => 
                { 
                    value.Clear(); 
                }, 
                null);

            _timeZoneDailyPlans.Clear(
                (key, value) => 
                { 
                    value.Clear(); 
                }, 
                null);
        }

        private void PrepareNextNodeExecution()
        {
            PrepareNextNodeExecution(null);
        }

        private void PrepareNextNodeExecution(LinkedListNode<TimeNode> nextNode)
        {
            LinkedListNode<TimeNode> nodeToProcess = null;
            long period = -1;
            //search for next node and start timeout
            if (nextNode == null)
            {
                period = GetTimeToNextNode(GetActualTimeMilisecond(), out nodeToProcess);
                //this should not happened
                if (nodeToProcess == null)
                    return;
            }
            //next node is available so only count time for timeout
            else
            {
                nodeToProcess = nextNode;
                period = nodeToProcess.Value.Time - GetActualTimeMilisecond();
            }

            StartTimerWithTimeout(nodeToProcess, period);
        }

        /// <summary>
        /// Gets time in miliseconds to next processing node
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="nextNode"></param>
        /// <returns></returns>
        private long GetTimeToNextNode(long fromTimeInMiliseconds, out LinkedListNode<TimeNode> nextNode)
        {
            nextNode = null;

            //handling invalid input parameters
            if (fromTimeInMiliseconds < 0 || fromTimeInMiliseconds > TOTAL_DAY_MILISECONDS)
                return -1;

            //stops also when node time is same as fromTime (immediate tick will be done then)
            LinkedListNode<TimeNode> node = _axisNodes.First;
            if (node == null)
                return -1;
            while (node.Value != null && node.Value.Time < fromTimeInMiliseconds && !node.Value.DayChangeNode)
            {
                if (node.Next != null)
                    node = node.Next;
                else
                    break;
            }

            nextNode = node;
            return node.Value.Time - fromTimeInMiliseconds;
        }

        private void StartTimerWithTimeout(LinkedListNode<TimeNode> nodeToProcess, long timeInMiliseconds)
        {
            //process node directly
            if (timeInMiliseconds == 0 || timeInMiliseconds < 0)
            {
                ProcessNode(nodeToProcess);
                return;
            }

            //ensure dispose of old timer
            if (_axisTimer != null)
            {
                _axisTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _axisTimer.Dispose();
            }

            _axisTimer = new Timer(ProcessNode, nodeToProcess, timeInMiliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Processes the node (processes also other nodes with same processing time)
        /// </summary>
        /// <param name="node"></param>
        private void ProcessNode(object node)
        {
            //unexpected object type
            if (!(node is LinkedListNode<TimeNode>))
                return;

            LinkedListNode<TimeNode> linkedNode = node as LinkedListNode<TimeNode>;

            LinkedListNode<TimeNode> nextNode = null;
            LinkedList<TimeNode> nodesWithSameTimeToProcess = GetNodesWithSameProcessTime(linkedNode, out nextNode);
            if (nodesWithSameTimeToProcess == null)
                return;

            bool dayChange = false;
            foreach (TimeNode timeNode in nodesWithSameTimeToProcess)
            {
                if (timeNode.DayChangeNode)
                    dayChange = true;
                else if (timeNode.Time != TOTAL_DAY_MILISECONDS)
                    timeNode.ProcessTimeNode();
            }

            //the day is changing, so it is important to redesign time axis
            if (dayChange)
                ProcessDayChange();
            else
                PrepareNextNodeExecution(nextNode);
        }

        private void ProcessDayChange()
        {
            ICollection<Guid> olderDailyPlans = _dailyPlanStates.Keys;
            ICollection<Guid> olderTimeZones = _timeZoneStates.Keys;
            ICollection<Guid> actualDailyPlans = new HashSet<Guid>();
            ICollection<Guid> actualTimeZones = new HashSet<Guid>();

            //when older daily plans info is stored in another structure, it is safe to remove dependency records
            ClearDependencyRecords();

            SetCurrentStates(out actualTimeZones, out actualDailyPlans);

            //collect removed time zones
            ICollection<Guid> removedTimeZones = new HashSet<Guid>();
            foreach (Guid timeZoneId in olderTimeZones)
            {
                if (!actualTimeZones.Contains(timeZoneId))
                    removedTimeZones.Add(timeZoneId);
            }

            RemoveStateRecords(removedTimeZones);

            PrepareNextNodeExecution();
        }

        private void RemoveStateRecords(ICollection<Guid> removedTimeZones)
        {
            _timeZoneStates.RemoveWhere((key, value) =>
            {
                return removedTimeZones.Contains(key);
            });
        }

        private void AddNodesForAddedDailyPlans(ICollection<Guid> addedDailyPlans)
        {
            foreach (Guid dailyPlanId in addedDailyPlans)
            {
                DailyPlan dailyPlan = DailyPlans.Singleton.GetById(dailyPlanId);
                if (dailyPlan != null)
                {
                    foreach (TimeNode node in GetNodesForDailyPlan(dailyPlan))
                    {
                        _axisNodes.AddFirst(node);
                    }
                }
            }
        }

        private void RemoveNodesFromRemovedDailyPlans(ICollection<Guid> removedDailyPlans)
        {
            LinkedList<TimeNode> nodesToRemove = new LinkedList<TimeNode>();

            //iterate through all nodes and mark for delete those, which belongs to old daily plan
            foreach (TimeNode node in _axisNodes)
            {
                if (removedDailyPlans.Contains(node.DailyPlanId))
                    nodesToRemove.AddFirst(node);
            }

            //delete them
            foreach (TimeNode node in nodesToRemove)
            {
                _axisNodes.Remove(node);
            }
        }

        private void SetCurrentStates(out ICollection<Guid> actualTimeZones, out ICollection<Guid> actualDailyPlans)
        {
            actualTimeZones = new HashSet<Guid>();
            actualDailyPlans = new HashSet<Guid>();

            List<Contal.Cgp.Server.Beans.TimeZone> timeZones = TimeZones.Singleton.List().ToList();
            if (timeZones == null)
                return;

            foreach (Contal.Cgp.Server.Beans.TimeZone timeZone in timeZones)
            {
                //add info, that current time zone is actual
                actualTimeZones.Add(timeZone.IdTimeZone);

                //update time zone state info
                UpdateTimeZoneState(timeZone.IdTimeZone, timeZone.State ? ON : OFF);

                List<DailyPlan> dailyPlans = GetActualDailyPlans(timeZone, DateTime.Now);

                //update daily plans state info
                foreach (DailyPlan dailyPlan in dailyPlans)
                {
                    //add info, that this daily plan is actual
                    if (!actualDailyPlans.Contains(dailyPlan.IdDailyPlan))
                        actualDailyPlans.Add(dailyPlan.IdDailyPlan);

                    //update dependency info
                    UpdateDependencyInfo(timeZone.IdTimeZone, dailyPlan.IdDailyPlan);

                    //update daily plan state info
                    UpdateDailyPlanState(dailyPlan.IdDailyPlan, dailyPlan.State ? ON : OFF);
                }
            }

            //add nodes from daily plans which are not in TimeZones
            List<Contal.Cgp.Server.Beans.DailyPlan> allDailyPlans = DailyPlans.Singleton.List().ToList();
            if (allDailyPlans != null)
            {
                foreach (DailyPlan dailyPlan in allDailyPlans)
                {
                    // check if DailyPlan was previously added
                    if (!actualDailyPlans.Contains(dailyPlan.IdDailyPlan))
                    {
                        //save processed DailyPlan
                        actualDailyPlans.Add(dailyPlan.IdDailyPlan);

                        //update daily plan state info
                        UpdateDailyPlanState(dailyPlan.IdDailyPlan, dailyPlan.State ? ON : OFF);

                        List<TimeNode> dailyPlanNodes = GetNodesForDailyPlan(dailyPlan);
                        if (dailyPlanNodes == null)
                            continue;

                        foreach (TimeNode timeNode in dailyPlanNodes)
                            _axisNodes.AddLast(timeNode);
                    }
                }
            }
        }

        private void UpdateTimeZoneState(Guid timeZoneId, byte newState)
        {
            bool stateChanged = false;
            if (!_timeZoneStates.ContainsKey(timeZoneId))
            {
                _timeZoneStates.Add(timeZoneId, newState);
                stateChanged = true;
            }
            else
            {
                stateChanged = _timeZoneStates[timeZoneId] != newState;
                _timeZoneStates[timeZoneId] = newState;
            }

            //consider calling state change
            if (stateChanged)
                TZStatusChanged(timeZoneId, newState);
        }

        private void UpdateDailyPlanState(Guid dailyPlanId, byte newState)
        {
            bool stateChanged = false;
            if (!_dailyPlanStates.ContainsKey(dailyPlanId))
            {
                _dailyPlanStates.Add(dailyPlanId, newState);
                stateChanged = true;
            }
            else
            {
                stateChanged = _dailyPlanStates[dailyPlanId] != newState;
                _dailyPlanStates[dailyPlanId] = newState;
            }
            //consider calling state chenge
            if (stateChanged)
                DPStatusChanged(dailyPlanId, _dailyPlanStates[dailyPlanId]);
        }

        /// <summary>
        /// Returns time nodes with same execution time as specific node (including specific node)
        /// </summary>
        /// <param name="linkedNode"></param>
        /// <param name="nextNode">Next node to be processed (null if no next node is present)</param>
        /// <returns></returns>
        private LinkedList<TimeNode> GetNodesWithSameProcessTime(LinkedListNode<TimeNode> linkedNode, out LinkedListNode<TimeNode> nextNode)
        {
            nextNode = null;
            if (linkedNode == null)
                return null;

            LinkedList<TimeNode> result = new LinkedList<TimeNode>();
            result.AddFirst(linkedNode.Value);

            //otherNode will be used for iteration
            LinkedListNode<TimeNode> otherNode = linkedNode;

            //iterate backwards to get nodes with same processing time
            while (otherNode.Previous != null && otherNode.Previous.Value != null && otherNode.Previous.Value.Time == otherNode.Value.Time)
            {
                otherNode = otherNode.Previous;
                result.AddFirst(otherNode.Value);
            }

            //set otherNode to linkedNode and start search forwards
            otherNode = linkedNode;

            //iterate forwards to get nodes with same processing time
            while (otherNode.Next != null && otherNode.Next.Value != null && otherNode.Next.Value.Time == linkedNode.Value.Time)
            {
                otherNode = otherNode.Next;
                result.AddFirst(otherNode.Value);
            }
            //assigning next node
            nextNode = otherNode.Next;

            return result;
        }

        /// <summary>
        /// Fills time axis with all nodes representing daily plan interval changes and also sets time zone and daily plan current states
        /// </summary>
        private void FillTimeAxisAndSetCurrentStates()
        {
            ICollection<DailyPlan> addedDailyPlans = new HashSet<DailyPlan>();

            _axisNodes = new LinkedList<TimeNode>();

            List<Contal.Cgp.Server.Beans.TimeZone> timeZones = TimeZones.Singleton.List().ToList();
            if (timeZones == null)
                return;

            foreach (Contal.Cgp.Server.Beans.TimeZone timeZone in timeZones)
            {
                //update time zone state info
                UpdateTimeZoneState(timeZone.IdTimeZone, timeZone.State ? ON : OFF);

                List<DailyPlan> dailyPlans = GetActualDailyPlans(timeZone, DateTime.Now);
                if (dailyPlans == null)
                    continue;

                //fill axis with nodes from daily plans
                foreach (DailyPlan dailyPlan in dailyPlans)
                {
                    //save processed DailyPlan
                    addedDailyPlans.Add(dailyPlan);

                    //update dependency info
                    UpdateDependencyInfo(timeZone.IdTimeZone, dailyPlan.IdDailyPlan);
                    //update daily plan state info
                    UpdateDailyPlanState(dailyPlan.IdDailyPlan, dailyPlan.State ? ON : OFF);

                    List<TimeNode> dailyPlanNodes = GetNodesForDailyPlan(dailyPlan);
                    if (dailyPlanNodes == null)
                        continue;

                    foreach (TimeNode timeNode in dailyPlanNodes)
                        _axisNodes.AddLast(timeNode);
                }
            }

            //Fill axis with nodes from daily plans which are not in TimeZones
            List<Contal.Cgp.Server.Beans.DailyPlan> allDailyPlans = DailyPlans.Singleton.List().ToList();
            if (allDailyPlans != null)
            {
                foreach (DailyPlan dailyPlan in allDailyPlans)
                {
                    // check if DailyPlan was previously added
                    if (!addedDailyPlans.Contains(dailyPlan))
                    {
                        //save processed DailyPlan
                        addedDailyPlans.Add(dailyPlan);

                        //update daily plan state info
                        UpdateDailyPlanState(dailyPlan.IdDailyPlan, dailyPlan.State ? ON : OFF);

                        List<TimeNode> dailyPlanNodes = GetNodesForDailyPlan(dailyPlan);
                        if (dailyPlanNodes == null)
                            continue;

                        foreach (TimeNode timeNode in dailyPlanNodes)
                            _axisNodes.AddLast(timeNode);
                    }
                }
            }

            //adding day change node, so it will process any change situation
            _axisNodes.AddLast(new TimeNode(TOTAL_DAY_MILISECONDS, NOT_SET, Guid.Empty, true));

            //sort axis nodes by time
            _axisNodes = new LinkedList<TimeNode>(_axisNodes.OrderBy(timeNode => timeNode.Time));
        }

        private void UpdateDependencyInfo(Guid timeZoneId, Guid dailyPlanId)
        {
            _timeZoneDailyPlans.GetOrAddValue(timeZoneId,
                    (key) =>
                    {
                        return new LinkedList<Guid>();
                    },
                    (key, value, added) =>
                    {
                        if (added || !value.Contains(dailyPlanId))
                            value.AddFirst(dailyPlanId);
                    });

            _dailyPlanReferencedTimeZones.GetOrAddValue(dailyPlanId,
                    (key) =>
                    {
                        return new LinkedList<Guid>();
                    },
                    (key, value, added) =>
                    {
                        if (added || !value.Contains(timeZoneId))
                            value.AddFirst(timeZoneId);
                    });
        }

        /// <summary>
        /// Creates nodes for time axis
        /// </summary>
        /// <param name="dailyPlan"></param>
        /// <returns></returns>
        private List<TimeNode> GetNodesForDailyPlan(DailyPlan dailyPlan)
        {
            List<TimeNode> result = new List<TimeNode>();
            if (dailyPlan.DayIntervals == null)
                return result;

            foreach (DayInterval dayInterval in dailyPlan.DayIntervals)
            {
                result.Add(new TimeNode(dayInterval.MinutesFrom * 60 * 1000, ON, dailyPlan.IdDailyPlan));
                result.Add(new TimeNode((dayInterval.MinutesTo + 1) * 60 * 1000, OFF, dailyPlan.IdDailyPlan));
            }

            return result;
        }

        public void ProcessDailyPlanStateChanged(Guid dailyPlanId, byte newState)
        {
            //update daily plan state
            UpdateDailyPlanState(dailyPlanId, newState);

            //process possible time zone state change
            LinkedList<Guid> referencedTimeZonesGuids;
            _dailyPlanReferencedTimeZones.TryGetValue(dailyPlanId, out referencedTimeZonesGuids);
            if (referencedTimeZonesGuids != null && referencedTimeZonesGuids.Count > 0)
            {
                foreach (Guid timeZoneGuid in referencedTimeZonesGuids)
                {
                    if (!_timeZoneStates.ContainsKey(timeZoneGuid))
                        _timeZoneStates.Add(timeZoneGuid, OFF);

                    byte oldTimeZoneState = _timeZoneStates[timeZoneGuid];
                    byte newTimeZoneState = SetNewTimeZoneState(timeZoneGuid);

                    //time zone state did not changed, so proceed to next time zone
                    if (oldTimeZoneState == newTimeZoneState)
                        continue;
                    //time zone state has changed, so call event
                    else
                        TZStatusChanged(timeZoneGuid, newTimeZoneState);
                }
            }
        }

        private byte SetNewTimeZoneState(Guid timeZoneGuid)
        {
            if (!_timeZoneDailyPlans.ContainsKey(timeZoneGuid))
                return NOT_SET;

            byte dailyPlanState = NOT_SET;
            foreach (Guid dailyPlanId in _timeZoneDailyPlans[timeZoneGuid])
            {
                //if at least one of daily plan has state set to on, time zone state will be set to on
                if (_dailyPlanStates.TryGetValue(dailyPlanId, out dailyPlanState) && dailyPlanState == ON)
                {
                    //update time zone state in dictionary
                    if (!_timeZoneStates.ContainsKey(timeZoneGuid))
                        _timeZoneStates.Add(timeZoneGuid, ON);
                    else
                        _timeZoneStates[timeZoneGuid] = ON;

                    return ON;
                }
            }

            //no daily plan has its state set to on
            //update time zone state in dictionary
            if (!_timeZoneStates.ContainsKey(timeZoneGuid))
                _timeZoneStates.Add(timeZoneGuid, OFF);
            else
                _timeZoneStates[timeZoneGuid] = OFF;

            return OFF;
        }

        private object _lockStartProcessRunTimeAxis = new object();

        private List<DailyPlan> GetActualDailyPlans(Contal.Cgp.Server.Beans.TimeZone timeZone, DateTime dateTime)
        {
            List<DailyPlan> dailyPlans = new List<DailyPlan>();
            List<DailyPlan> explicitDailyPlans = new List<DailyPlan>();

            foreach (TimeZoneDateSetting dateSetting in timeZone.DateSettings)
            {
                if (dateSetting.DailyPlan != null && dateSetting.IsActual(dateTime))
                {
                    if (dateSetting.ExplicitDailyPlan)
                    {
                        explicitDailyPlans.Add(dateSetting.DailyPlan);
                    }
                    else
                    {
                        dailyPlans.Add(dateSetting.DailyPlan);
                    }
                }
            }

            if (explicitDailyPlans.Count > 0)
            {
                return explicitDailyPlans;
            }
            else
            {
                return dailyPlans;
            }
        }

        public void DPStatusChanged(Guid idDailyPlan, byte status)
        {
            try
            {
                CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunDPStatusChanged, Contal.IwQuick.Threads.DelegateSequenceBlockingMode.Asynchronous, false, idDailyPlan, status);
            }
            catch { }
            if (DailyPlanStateChanged != null)
            {
                try
                {
                    DailyPlanStateChanged(idDailyPlan, status);
                }
                catch { }
            }
        }

        public void RunDPStatusChanged(Contal.IwQuick.Remoting.ARemotingCallbackHandler remoteHandler, params object[] objects)
        {
            if (objects.Length == 2 && objects[0] is Guid && objects[1] is byte)
            {
                try
                {
                    if (remoteHandler is StatusChangedDailyPlainHandler)
                        (remoteHandler as StatusChangedDailyPlainHandler).RunEvent((Guid)objects[0], (byte)objects[1]);
                }
                catch
                {
                    throw;
                }
            }
        }

        private void TZStatusChanged(Guid idTimeZone, byte status)
        {
            try
            {
                CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunTZStatusChanged, Contal.IwQuick.Threads.DelegateSequenceBlockingMode.Asynchronous, false, idTimeZone, status);
            }
            catch { }
            if (TimeZoneStateChanged != null)
            {
                try
                {
                    TimeZoneStateChanged(idTimeZone, status);
                }
                catch { }
            }
        }

        public void RunTZStatusChanged(Contal.IwQuick.Remoting.ARemotingCallbackHandler remoteHandler, params object[] objects)
        {
            if (objects.Length == 2 && objects[0] is Guid && objects[1] is byte)
            {
                try
                {
                    if (remoteHandler is StatusChangedTimeZoneHandler)
                        (remoteHandler as StatusChangedTimeZoneHandler).RunEvent((Guid)objects[0], (byte)objects[1]);
                }
                catch
                {
                    throw;
                }
            }
        }

        public byte GetActualStatusDP(Guid idDailyPlan)
        {
            byte acutalState;
            if (_dailyPlanStates != null && _dailyPlanStates.TryGetValue(idDailyPlan, out acutalState))
                return acutalState;

            DailyPlan dailyPlan = DailyPlans.Singleton.GetById(idDailyPlan);
            if (dailyPlan != null)
                return dailyPlan.State ? ON : OFF;

            return NOT_SET;
        }

        public byte GetActualStatusTZ(Guid idTimeZone)
        {
            if (_timeZoneStates == null || !_timeZoneStates.ContainsKey(idTimeZone))
                return NOT_SET;

            return _timeZoneStates[idTimeZone];
        }
    }

    /// <summary>
    /// Element on TimeAxis, that hold time and new state for DP
    /// </summary>
    public class TimeNode
    {
        /// <summary>
        /// Time in day in miliseconds
        /// </summary>
        long _time;
        byte _nextState;
        Guid _dailyPlanId = Guid.Empty;
        bool _dayChangeNode = false;

        /// <summary>
        /// Special node for handling date change situations
        /// </summary>
        public bool DayChangeNode
        {
            get { return _dayChangeNode; }
            set { _dayChangeNode = value; }
        }

        public Guid DailyPlanId
        {
            get { return _dailyPlanId; }
            set { _dailyPlanId = value; }
        }

        /// <summary>
        /// Time in day in miliseconds
        /// </summary>
        public long Time
        {
            get { return _time; }
        }

        public TimeNode(long milisecondsTime, byte status, Guid dailyPlanId, bool isDayChangeNode)
        {
            _time = milisecondsTime;
            _nextState = status;
            _dailyPlanId = dailyPlanId;
            _dayChangeNode = isDayChangeNode;
        }

        public TimeNode(long milisecondsTime, byte status, Guid dailyPlanId)
        {
            _time = milisecondsTime;
            _nextState = status;
            _dailyPlanId = dailyPlanId;
        }

        /// <summary>
        /// Sets new daily plan state (do nothing, when it is day change node)
        /// </summary>
        public void ProcessTimeNode()
        {
            if (_dayChangeNode)
                return;

            TimeAxis.Singleton.ProcessDailyPlanStateChanged(_dailyPlanId, _nextState);
        }
    }
}
