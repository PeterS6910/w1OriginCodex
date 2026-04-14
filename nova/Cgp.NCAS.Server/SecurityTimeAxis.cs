using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using System.Threading;

using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    class SecurityTimeAxis
    {
        public const long TOTAL_DAY_MILISECONDS = 86400000;

        private static volatile SecurityTimeAxis _singleton = null;
        private static object _syncRoot = new object();

        private System.Threading.Timer _axisTimer = null;
        LinkedList<TimeNode> _axisNodes = new LinkedList<TimeNode>();

        /// <summary>
        /// Stores information about security daily plan and security time zones, which use it
        /// </summary>
        private Dictionary<Guid, LinkedList<Guid>> _securityDailyPlanReferencedTimeZones = new Dictionary<Guid, LinkedList<Guid>>();
        /// <summary>
        /// Stores information about security time zone and its security daily plans
        /// </summary>
        private Dictionary<Guid, LinkedList<Guid>> _securityTimeZoneDailyPlans = new Dictionary<Guid, LinkedList<Guid>>();
        /// <summary>
        /// Stores state info for each security daily plan
        /// </summary>
        private Dictionary<Guid, byte> _securityDailyPlanStates = new Dictionary<Guid, byte>();
        /// <summary>
        /// Stores state info for each security time zone
        /// </summary>
        private Dictionary<Guid, byte> _securityTimeZoneStates = new Dictionary<Guid, byte>();

        public SecurityTimeAxis()
        {
            Microsoft.Win32.SystemEvents.TimeChanged += new EventHandler(SystemEvents_TimeChanged);
        }

        /// <summary>
        /// Restart security time axis after time changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            StartOrReset();
        }

        public static SecurityTimeAxis Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new SecurityTimeAxis();
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

        /// <summary>
        /// Start or restart security time axis
        /// </summary>
        public void StartOrReset()
        {
            if (_axisTimer != null)
                _axisTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ClearExistingRecords();
            FillTimeAxisAndSetCurrentStates();
            PrepareNextNodeExecution();
        }

        /// <summary>
        /// Removes all existing security time axis records
        /// </summary>
        private void ClearExistingRecords()
        {
            if (_axisNodes != null)
                _axisNodes.Clear();
            //ClearStateRecords();
            ClearDependencyRecords();
        }

        /// <summary>
        /// Removes all security daily plan and security time zone state information
        /// </summary>
        private void ClearStateRecords()
        {
            if (_securityDailyPlanStates != null)
                _securityDailyPlanStates.Clear();
            if (_securityTimeZoneStates != null)
                _securityTimeZoneStates.Clear();
        }

        /// <summary>
        /// Removes information about security time zones and security daily plans dependency
        /// </summary>
        private void ClearDependencyRecords()
        {
            if (_securityDailyPlanReferencedTimeZones != null)
                _securityDailyPlanReferencedTimeZones.Clear();
            if (_securityTimeZoneDailyPlans != null)
                _securityTimeZoneDailyPlans.Clear();
        }

        /// <summary>
        /// Start timer to execution next node
        /// </summary>
        private void PrepareNextNodeExecution()
        {
            PrepareNextNodeExecution(null);
        }

        /// <summary>
        /// Start timer to execution next node
        /// </summary>
        /// <param name="nextNode"></param>
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

        /// <summary>
        /// Start timer to run ProcessNode
        /// </summary>
        /// <param name="nodeToProcess"></param>
        /// <param name="timeInMiliseconds"></param>
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

        /// <summary>
        /// Read security time zone and security daily plans for the next day
        /// </summary>
        private void ProcessDayChange()
        {
            List<Guid> olderSecurityDailyPlans = _securityDailyPlanStates.Keys.ToArray().ToList();
            List<Guid> olderSecurityTimeZones = _securityTimeZoneStates.Keys.ToArray().ToList();
            List<Guid> actualSecurityDailyPlans = new List<Guid>();
            List<Guid> actualSecurityTimeZones = new List<Guid>();

            //when older security daily plans info is stored in another structure, it is safe to remove dependency records
            ClearDependencyRecords();

            SetCurrentStates(out actualSecurityTimeZones, out actualSecurityDailyPlans);

            //collect removed security daily plans
            List<Guid> removedSecurityDailyPlans = new List<Guid>();
            foreach (Guid securityDailyPlanId in olderSecurityDailyPlans)
            {
                if (!actualSecurityDailyPlans.Contains(securityDailyPlanId))
                    removedSecurityDailyPlans.Add(securityDailyPlanId);
            }

            //collect added security daily plans
            List<Guid> addedSecurityDailyPlans = new List<Guid>();
            foreach (Guid securityDailyPlanId in actualSecurityDailyPlans)
            {
                if (!olderSecurityDailyPlans.Contains(securityDailyPlanId))
                    addedSecurityDailyPlans.Add(securityDailyPlanId);
            }

            //collect removed security time zones
            List<Guid> removedSecurityTimeZones = new List<Guid>();
            foreach (Guid securityTimeZoneId in olderSecurityTimeZones)
            {
                if (!actualSecurityTimeZones.Contains(securityTimeZoneId))
                    removedSecurityTimeZones.Add(securityTimeZoneId);
            }

            RemoveStateRecords(removedSecurityTimeZones, removedSecurityDailyPlans);

            if (removedSecurityDailyPlans.Count != 0 || addedSecurityDailyPlans.Count != 0)
            {
                RemoveNodesFromRemovedSecurityDailyPlans(removedSecurityDailyPlans);
                AddNodesForAddedSecurityDailyPlans(addedSecurityDailyPlans);
                _axisNodes = new LinkedList<TimeNode>(_axisNodes.OrderBy(node => node.Time));
            }
            PrepareNextNodeExecution();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="removedSecurityTimeZones"></param>
        /// <param name="removedSecurityDailyPlans"></param>
        private void RemoveStateRecords(List<Guid> removedSecurityTimeZones, List<Guid> removedSecurityDailyPlans)
        {
            if (removedSecurityTimeZones != null)
                foreach (Guid securityTimeZoneId in removedSecurityTimeZones)
                    _securityTimeZoneStates.Remove(securityTimeZoneId);

            if (removedSecurityDailyPlans != null)
                foreach (Guid securityDailyPlanId in removedSecurityDailyPlans)
                    _securityDailyPlanStates.Remove(securityDailyPlanId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addedSecurityDailyPlans"></param>
        private void AddNodesForAddedSecurityDailyPlans(List<Guid> addedSecurityDailyPlans)
        {
            foreach (Guid securitydailyPlanId in addedSecurityDailyPlans)
            {
                SecurityDailyPlan securitydailyPlan = SecurityDailyPlans.Singleton.GetById(securitydailyPlanId);
                if (securitydailyPlan != null)
                {
                    foreach (TimeNode node in GetNodesForSecurityDailyPlan(securitydailyPlan))
                    {
                        _axisNodes.AddFirst(node);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="removedSecurityDailyPlans"></param>
        private void RemoveNodesFromRemovedSecurityDailyPlans(List<Guid> removedSecurityDailyPlans)
        {
            LinkedList<TimeNode> nodesToRemove = new LinkedList<TimeNode>();

            //iterate through all nodes and mark for delete those, which belongs to old security daily plan
            foreach (TimeNode node in _axisNodes)
            {
                if (removedSecurityDailyPlans.Contains(node.SecrurityDailyPlanId))
                    nodesToRemove.AddFirst(node);
            }

            //delete them
            foreach (TimeNode node in nodesToRemove)
            {
                _axisNodes.Remove(node);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualSecurityTimeZones"></param>
        /// <param name="actualSecurityDailyPlans"></param>
        private void SetCurrentStates(out List<Guid> actualSecurityTimeZones, out List<Guid> actualSecurityDailyPlans)
        {
            actualSecurityTimeZones = new List<Guid>();
            actualSecurityDailyPlans = new List<Guid>();

            List<SecurityTimeZone> securityTimeZones = SecurityTimeZones.Singleton.List().ToList();
            if (securityTimeZones == null)
                return;

            foreach (SecurityTimeZone securityTimeZone in securityTimeZones)
            {
                //add info, that current security time zone is actual
                actualSecurityTimeZones.Add(securityTimeZone.IdSecurityTimeZone);

                List<SecurityDailyPlan> securityDailyPlans = SecurityTimeZones.Singleton.GetActualSecurityDailyPlans(securityTimeZone, DateTime.Now);

                //update security daily plans state info
                foreach (SecurityDailyPlan securityDailyPlan in securityDailyPlans)
                {
                    //add info, that this security daily plan is actual
                    if (!actualSecurityDailyPlans.Contains(securityDailyPlan.IdSecurityDailyPlan))
                        actualSecurityDailyPlans.Add(securityDailyPlan.IdSecurityDailyPlan);

                    //update dependency info
                    UpdateDependencyInfo(securityTimeZone.IdSecurityTimeZone, securityDailyPlan.IdSecurityDailyPlan);

                    //update security daily plan state info
                    UpdateSecurityDailyPlanState(securityDailyPlan.IdSecurityDailyPlan, GetActualStateForSecurityDailyPlan(securityDailyPlan));
                }

                //update security time zone state info
                UpdateSecurityTimeZoneState(securityTimeZone.IdSecurityTimeZone, GetActualStateForSecurityTimeZone(securityTimeZone.IdSecurityTimeZone));
            }
        }

        /// <summary>
        /// Set new starte for security time zone
        /// </summary>
        /// <param name="securityTimeZoneId"></param>
        /// <param name="newState"></param>
        private void UpdateSecurityTimeZoneState(Guid securityTimeZoneId, byte newState)
        {
            bool stateChanged = false;
            if (!_securityTimeZoneStates.ContainsKey(securityTimeZoneId))
            {
                _securityTimeZoneStates.Add(securityTimeZoneId, newState);
                stateChanged = true;
            }
            else
            {
                stateChanged = _securityTimeZoneStates[securityTimeZoneId] != newState;
                _securityTimeZoneStates[securityTimeZoneId] = newState;
            }

            //consider calling state change
            if (stateChanged)
                SecurityTZStatusChanged(securityTimeZoneId, newState);
        }

        /// <summary>
        /// Set new state for security daily plan
        /// </summary>
        /// <param name="securityDailyPlanId"></param>
        /// <param name="newState"></param>
        private void UpdateSecurityDailyPlanState(Guid securityDailyPlanId, byte newState)
        {
            bool stateChanged = false;
            if (!_securityDailyPlanStates.ContainsKey(securityDailyPlanId))
            {
                _securityDailyPlanStates.Add(securityDailyPlanId, newState);
                stateChanged = true;
            }
            else
            {
                stateChanged = _securityDailyPlanStates[securityDailyPlanId] != newState;
                _securityDailyPlanStates[securityDailyPlanId] = newState;
            }

            //consider calling state change
            if (stateChanged)
                SecurityDPStatusChanged(securityDailyPlanId, newState);
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
        /// Fills security time axis with all nodes representing security daily plan interval changes and also sets security time zone and security daily plan current states
        /// </summary>
        private void FillTimeAxisAndSetCurrentStates()
        {
            _axisNodes = new LinkedList<TimeNode>();

            List<SecurityTimeZone> securityTimeZones = SecurityTimeZones.Singleton.List().ToList();
            if (securityTimeZones == null)
                return;

            foreach (SecurityTimeZone securityTimeZone in securityTimeZones)
            {
                List<SecurityDailyPlan> securityDailyPlans = SecurityTimeZones.Singleton.GetActualSecurityDailyPlans(securityTimeZone, DateTime.Now);
                if (securityDailyPlans == null)
                    continue;

                //fill axis with nodes from security daily plans
                foreach (SecurityDailyPlan securityDailyPlan in securityDailyPlans)
                {
                    //update dependency info
                    UpdateDependencyInfo(securityTimeZone.IdSecurityTimeZone, securityDailyPlan.IdSecurityDailyPlan);
                    //update security daily plan state info

                    UpdateSecurityDailyPlanState(securityDailyPlan.IdSecurityDailyPlan, GetActualStateForSecurityDailyPlan(securityDailyPlan));

                    List<TimeNode> dailyPlanNodes = GetNodesForSecurityDailyPlan(securityDailyPlan);
                    if (dailyPlanNodes == null)
                        continue;

                    foreach (TimeNode timeNode in dailyPlanNodes)
                        _axisNodes.AddLast(timeNode);
                }

                //update security time zone state info
                UpdateSecurityTimeZoneState(securityTimeZone.IdSecurityTimeZone, GetActualStateForSecurityTimeZone(securityTimeZone.IdSecurityTimeZone));
            }
            //adding day change node, so it will process ady change situation
            _axisNodes.AddLast(new TimeNode(TOTAL_DAY_MILISECONDS, (byte)SecurityLevel4SLDP.locked, Guid.Empty, true));

            //sort axis nodes by time
            _axisNodes = new LinkedList<TimeNode>(_axisNodes.OrderBy(timeNode => timeNode.Time));
        }

        /// <summary>
        /// Return actual state for security daily plan
        /// </summary>
        /// <param name="securityDailyPlan"></param>
        /// <returns></returns>
        private byte GetActualStateForSecurityDailyPlan(SecurityDailyPlan securityDailyPlan)
        {
            if (securityDailyPlan != null && securityDailyPlan.SecurityDayIntervals != null && securityDailyPlan.SecurityDayIntervals.Count > 0)
            {
                long actualTimeInMiliseconds = GetActualTimeMilisecond();
                foreach (SecurityDayInterval securityDayInterval in securityDailyPlan.SecurityDayIntervals)
                {
                    if (securityDayInterval != null && securityDayInterval.MinutesFrom * 60 * 1000 <= actualTimeInMiliseconds && (securityDayInterval.MinutesTo + 1) * 60 * 1000 > actualTimeInMiliseconds)
                    {
                        return securityDayInterval.IntervalType;
                    }
                }
            }

            return (byte)SecurityLevel4SLDP.locked;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <param name="dailyPlanId"></param>
        private void UpdateDependencyInfo(Guid timeZoneId, Guid dailyPlanId)
        {
            if (!_securityTimeZoneDailyPlans.ContainsKey(timeZoneId))
                _securityTimeZoneDailyPlans.Add(timeZoneId, new LinkedList<Guid>());
            if (_securityTimeZoneDailyPlans[timeZoneId] == null)
                _securityTimeZoneDailyPlans[timeZoneId] = new LinkedList<Guid>();
            if (!_securityTimeZoneDailyPlans[timeZoneId].Contains(dailyPlanId))
                _securityTimeZoneDailyPlans[timeZoneId].AddFirst(dailyPlanId);

            if (!_securityDailyPlanReferencedTimeZones.ContainsKey(dailyPlanId))
                _securityDailyPlanReferencedTimeZones.Add(dailyPlanId, new LinkedList<Guid>());
            if (_securityDailyPlanReferencedTimeZones[dailyPlanId] == null)
                _securityDailyPlanReferencedTimeZones[dailyPlanId] = new LinkedList<Guid>();
            if (!_securityDailyPlanReferencedTimeZones[dailyPlanId].Contains(timeZoneId))
                _securityDailyPlanReferencedTimeZones[dailyPlanId].AddFirst(timeZoneId);
        }

        /// <summary>
        /// Creates nodes for security time axis
        /// </summary>
        /// <param name="dailyPlan"></param>
        /// <returns></returns>
        private List<TimeNode> GetNodesForSecurityDailyPlan(SecurityDailyPlan securityDailyPlan)
        {
            List<TimeNode> result = new List<TimeNode>();
            if (securityDailyPlan.SecurityDayIntervals == null)
                return result;

            foreach (SecurityDayInterval securityDayInterval in securityDailyPlan.SecurityDayIntervals)
            {
                result.Add(new TimeNode(securityDayInterval.MinutesFrom * 60 * 1000, securityDayInterval.IntervalType, securityDailyPlan.IdSecurityDailyPlan));
            }

            return result;
        }

        /// <summary>
        /// Set state for the security daily plan and change state for referenced security time zones
        /// </summary>
        /// <param name="dailyPlanId"></param>
        /// <param name="newState"></param>
        public void ProcessSecurityDailyPlanStateChanged(Guid dailyPlanId, byte newState)
        {
            //update security daily plan state
            UpdateSecurityDailyPlanState(dailyPlanId, newState);

            //no security time zone is using this security daily plan
            if (_securityDailyPlanReferencedTimeZones == null || !_securityDailyPlanReferencedTimeZones.ContainsKey(dailyPlanId))
                return;

            //no security time zone is using this security daily plan
            LinkedList<Guid> referencedSecurityTimeZonesGuids = _securityDailyPlanReferencedTimeZones[dailyPlanId];
            if (referencedSecurityTimeZonesGuids == null || referencedSecurityTimeZonesGuids.Count == 0)
            {
                _securityDailyPlanReferencedTimeZones.Remove(dailyPlanId);
                if (referencedSecurityTimeZonesGuids == null)
                    return;
            }

            //process possible security time zone state change
            foreach (Guid securityTimeZoneGuid in referencedSecurityTimeZonesGuids)
            {
                if (!_securityTimeZoneStates.ContainsKey(securityTimeZoneGuid))
                    _securityTimeZoneStates.Add(securityTimeZoneGuid, (byte)SecurityLevel4SLDP.locked);

                byte oldSecurityTimeZoneState = _securityTimeZoneStates[securityTimeZoneGuid];
                byte newSecurityTimeZoneState = GetActualStateForSecurityTimeZone(securityTimeZoneGuid);

                //if security time zone state has changed, so set new state and call event
                if (oldSecurityTimeZoneState != newSecurityTimeZoneState)
                    UpdateSecurityTimeZoneState(securityTimeZoneGuid, newSecurityTimeZoneState);
            }
        }

        /// <summary>
        /// Read state from the actual security dialy plan, and return it
        /// </summary>
        /// <param name="securityTimeZoneGuid"></param>
        /// <returns></returns>
        private byte GetActualStateForSecurityTimeZone(Guid securityTimeZoneGuid)
        {
            if (!_securityTimeZoneDailyPlans.ContainsKey(securityTimeZoneGuid))
                return (byte)SecurityLevel4SLDP.locked;

            byte securityTimeZoneState = (byte)SecurityLevel4SLDP.locked;
            byte securityDailyPlanState;
            byte statePrioroty = 0;
            SecurityLevelPriority slp = new SecurityLevelPriority();

            foreach (Guid securityDailyPlanId in _securityTimeZoneDailyPlans[securityTimeZoneGuid])
            {
                if (_securityDailyPlanStates.TryGetValue(securityDailyPlanId, out securityDailyPlanState))
                {
                    if (statePrioroty < slp.GetPriorityForSecurityLevel(securityDailyPlanState))
                    {
                        statePrioroty = slp.GetPriorityForSecurityLevel(securityDailyPlanState);
                        securityTimeZoneState = securityDailyPlanState;
                    }
                }
            }

            return securityTimeZoneState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSecurityTimeZone"></param>
        /// <param name="status"></param>
        private void SecurityTZStatusChanged(Guid idSecurityTimeZone, byte status)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunSecurityTZStatusChanged, Contal.IwQuick.Threads.DelegateSequenceBlockingMode.Asynchronous, false, idSecurityTimeZone, status);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <param name="objects"></param>
        public void RunSecurityTZStatusChanged(Contal.IwQuick.Remoting.ARemotingCallbackHandler remoteHandler, params object[] objects)
        {
            if (objects.Length == 2 && objects[0] is Guid && objects[1] is byte)
            {
                try
                {
                    if (remoteHandler is StatusChangedSecurityTimeZoneHandler)
                        (remoteHandler as StatusChangedSecurityTimeZoneHandler).RunEvent((Guid)objects[0], (byte)objects[1]);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idSecurityTimeZone"></param>
        /// <param name="status"></param>
        private void SecurityDPStatusChanged(Guid idSecurityTimeZone, byte status)
        {
            try
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunSecurityDPStatusChanged, Contal.IwQuick.Threads.DelegateSequenceBlockingMode.Asynchronous, false, idSecurityTimeZone, status);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <param name="objects"></param>
        public void RunSecurityDPStatusChanged(Contal.IwQuick.Remoting.ARemotingCallbackHandler remoteHandler, params object[] objects)
        {
            if (objects.Length == 2 && objects[0] is Guid && objects[1] is byte)
            {
                try
                {
                    if (remoteHandler is StatusChangedSecurityDailyPlanHandler)
                        (remoteHandler as StatusChangedSecurityDailyPlanHandler).RunEvent((Guid)objects[0], (byte)objects[1]);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Return actual state for security daily plan
        /// </summary>
        /// <param name="idDailyPlan"></param>
        /// <returns></returns>
        public byte GetActualStatusSecurityDP(Guid idDailyPlan)
        {
            byte actualState;
            if (_securityDailyPlanStates != null && _securityDailyPlanStates.TryGetValue(idDailyPlan, out actualState))
                return actualState;
                

            SecurityDailyPlan securityDailyPlan = SecurityDailyPlans.Singleton.GetById(idDailyPlan);
            if (securityDailyPlan != null)
                return GetActualStateForSecurityDailyPlan(securityDailyPlan);

            return (byte)SecurityLevel4SLDP.locked;
        }

        /// <summary>
        /// Return actual state for security time zone
        /// </summary>
        /// <param name="idTimeZone"></param>
        /// <returns></returns>
        public byte GetActualStatusSecurityTZ(Guid idTimeZone)
        {
            if (_securityTimeZoneStates == null || !_securityTimeZoneStates.ContainsKey(idTimeZone))
                return (byte)SecurityLevel4SLDP.locked;

            return _securityTimeZoneStates[idTimeZone];
        }
    }

    /// <summary>
    /// Element on TimeAxis, that hold time and new state for security DP
    /// </summary>
    public class TimeNode
    {
        /// <summary>
        /// Time in day in miliseconds
        /// </summary>
        long _time;
        byte _nextState;
        Guid _secirtyDailyPlanId = Guid.Empty;
        bool _dayChangeNode = false;

        /// <summary>
        /// Special node for handling date change situations
        /// </summary>
        public bool DayChangeNode
        {
            get { return _dayChangeNode; }
            set { _dayChangeNode = value; }
        }

        public Guid SecrurityDailyPlanId
        {
            get { return _secirtyDailyPlanId; }
            set { _secirtyDailyPlanId = value; }
        }

        /// <summary>
        /// Time in day in miliseconds
        /// </summary>
        public long Time
        {
            get { return _time; }
        }

        public TimeNode(long milisecondsTime, byte status, Guid secirtyDailyPlanId, bool isDayChangeNode)
        {
            _time = milisecondsTime;
            _nextState = status;
            _secirtyDailyPlanId = secirtyDailyPlanId;
            _dayChangeNode = isDayChangeNode;
        }

        public TimeNode(long milisecondsTime, byte status, Guid secirtyDailyPlanId)
        {
            _time = milisecondsTime;
            _nextState = status;
            _secirtyDailyPlanId = secirtyDailyPlanId;
        }

        /// <summary>
        /// Sets new security daily plan state (do nothing, when it is day change node)
        /// </summary>
        public void ProcessTimeNode()
        {
            if (_dayChangeNode)
                return;

            SecurityTimeAxis.Singleton.ProcessSecurityDailyPlanStateChanged(_secirtyDailyPlanId, _nextState);
        }
    }
}
