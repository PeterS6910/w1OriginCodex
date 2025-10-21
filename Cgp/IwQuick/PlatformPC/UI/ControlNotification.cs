using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.UI
{
    public class ControlNotification
    {
        /// <summary>
        /// defines notification state (apperance of the hint and/or color change )
        /// </summary>
        private class NotificationSnapshot
        {
            public ToolTip _hint;
            public string _message = String.Empty;
            public string _caption;
            public NotificationSeverity _severity = NotificationSeverity.Info;
            public ControlNotificationSettings _settings;
            public Threads.ITimer _timer;
        }

        /// <summary>
        /// defines information about states per event
        /// </summary>
        private class NotificationInfo
        {
            public Control Control;
            public ColorPair OriginalColors;

            public NotificationSnapshot CurrentSnapshot
            {
                get { return _currentSnapshot; }
            }

            private NotificationSnapshot _currentSnapshot;
            private readonly LinkedList<NotificationSnapshot> _stateQueue = new LinkedList<NotificationSnapshot>();

            internal void QueueOnTop(NotificationSnapshot snapshot)
            {
                lock (_stateQueue)
                {
                    _stateQueue.AddFirst(snapshot);
                }
            }

            internal void Queue(NotificationSnapshot snapshot)
            {
                lock (_stateQueue)
                {
                    _stateQueue.AddLast(snapshot);
                }
            }

            internal NotificationSnapshot PopSnapshot()
            {
                NotificationSnapshot ret = null;

                lock (_stateQueue)
                {

                    var lln = _stateQueue.First;
                    if (lln == null) return ret;

                    _stateQueue.RemoveFirst();
                    ret = _currentSnapshot = lln.Value;
                }

                return ret;
            }

            internal bool Any(Func<NotificationSnapshot,bool> predicate)
            {
                lock (_stateQueue)
                    return _stateQueue.Any(predicate);
            }

            public void ClearSnapshot()
            {
                lock (_stateQueue)
                {
                    _currentSnapshot = null;
                }
            }
        }

        [NotNull]
        private readonly Dictionary<Control, NotificationInfo> _notifications = new Dictionary<Control, NotificationInfo>(32);

        private static volatile ControlNotification _singleton;
        private static readonly object _syncRoot = new object();

        public static ControlNotification Singleton
        {
            get
            {
                if (null == _singleton) // optimization
                    lock (_syncRoot)    // atomicity
                    {
                        if (_singleton == null)
                            _singleton = new ControlNotification();
                    }

                return _singleton;
            }
        }

        // ---

        [NotNull]
        private readonly Threads.TimerManager _timers = new Threads.TimerManager();
       

        private bool RegisterControl(Control control, out NotificationInfo notifyInfo)
        {
            if (null == control)
            {
                notifyInfo = new NotificationInfo();
                return false;
            }

            if (!_notifications.TryGetValue(control, out notifyInfo))
            {
                notifyInfo = new NotificationInfo
                {
                    Control = control,
                    OriginalColors = new ColorPair(control),
                };

                //DateTime.Now.ToString(CultureInfo.InvariantCulture);

                _notifications.Add(control, notifyInfo);

            }

            return true;
        }

        public bool RegisterControl(Control control)
        {
            NotificationInfo aNotifyInfo;
            return RegisterControl(control, out aNotifyInfo);
        }

        private void RegisterNotification(
            NotificationInfo notifyInfo, 
            string message, 
            string caption,
            NotificationSeverity severity, 
            ControlNotificationSettings notificationSettings, 
            bool isInsert)
        {

            var noficicationState = new NotificationSnapshot
            {
                _hint = new ToolTip(),
                _settings = notificationSettings,
                _message = message,
                _caption = caption,
                _severity = severity
            };

            if (isInsert)
                notifyInfo.QueueOnTop(noficicationState);
            else
                notifyInfo.Queue(noficicationState);
        }

        private void PopNotification(NotificationInfo notifyInfo)
        {
            if (null != notifyInfo.CurrentSnapshot)
                return;

            NotificationSnapshot notifyState;

            try
            {
                notifyState = notifyInfo.PopSnapshot();
                if (notifyState == null)
                    return;
            }
            catch (InvalidOperationException)
            {
                return;
            }

            ApplyAppearance(notifyInfo, notifyState);

            
            if (notifyState._settings.Duration > 0)
            {
                var timeout = _timers.StartTimeout(notifyState._settings.Duration, notifyInfo, OnNotifyTimeOut);
                Debug.Assert(null != timeout, "Timer did not start");
                notifyState._timer = timeout;
            }

        }

        private void ApplyAppearance(NotificationInfo notifyInfo,NotificationSnapshot notifyState)
        {
            var aParentControl = 
                notifyInfo.Control.Parent
                // in case of forms
                ?? notifyInfo.Control;
            

            if (aParentControl.InvokeRequired)
            {
                aParentControl.BeginInvoke(new Action<NotificationInfo,NotificationSnapshot>(ApplyAppearance), notifyInfo, notifyState);
            }
            else
            {
                notifyState._settings.ApplyControlAppearance(notifyInfo.Control, notifyState._severity);
                notifyState._settings.ApplyHintAppearance(
                    aParentControl, 
                    notifyInfo.Control, 
                    notifyState._hint, 
                    notifyState._severity, 
                    notifyState._message,
                    notifyState._caption);
            }

            
        }

        private bool CheckNotificationRedundancy(NotificationInfo notifyInfo, NotificationSeverity severity, string message)
        {
            if (null == notifyInfo)
                return false;

            if (null != notifyInfo.CurrentSnapshot)
            {
                if (notifyInfo.CurrentSnapshot._severity == severity &&
                    notifyInfo.CurrentSnapshot._message.ToLower() ==
                        message.ToLower())
                    return true;
            }

            return notifyInfo.Any(
                aState => 
                    aState._severity == severity 
                    && string.Equals(aState._message, message, StringComparison.CurrentCultureIgnoreCase));
        }

        private void RemoveAllInQueue()
        {
            _notifications.Clear();
        }

        public void Invoke(
            [NotNull] Control control, 
            String message,
            NotificationPriority priority, 
            NotificationSeverity severity, 
            [NotNull] ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, null, priority, severity, notificationSettings);
        }

        public void Invoke(
            [NotNull] Control control, 
            string message, 
            [CanBeNull] string caption,
            NotificationPriority priority, 
            NotificationSeverity severity, 
            [NotNull] ControlNotificationSettings notificationSettings)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (null == control ||
                null == notificationSettings)
                // ReSharper disable once HeuristicUnreachableCode
                return;

            if (null == message)
                message = String.Empty;

            NotificationInfo aNotifyInfo;
            RegisterControl(control, out aNotifyInfo);

            // if same message already exists, do not add notification
            if (CheckNotificationRedundancy(aNotifyInfo, severity, message))
                return;

            // temporary solution
            switch (priority)
            {
                case NotificationPriority.Last:
                    break;
                case NotificationPriority.TopMost:
                    HideNotification(aNotifyInfo, true, true);
                    break;
                case NotificationPriority.JustOne:
                    HideNotification(aNotifyInfo, true, false);
                    RemoveAllInQueue();
                    
                    break;

            }

            RegisterNotification(aNotifyInfo, message, caption, severity, notificationSettings, true);

            PopNotification(aNotifyInfo);
        }


        private bool OnNotifyTimeOut(Threads.TimerCarrier timer)
        {
            if (null == timer)
                return true;
            var aNotifyInfo = (NotificationInfo)timer.Data;
            try
            {                
                HideNotification(aNotifyInfo, false, false);
                PopNotification(aNotifyInfo);
                return true;
            }
            catch (ObjectDisposedException)
            {
                _notifications.Remove(aNotifyInfo.Control);
                // the referrenced control has been closed or disposed
                // let the timeout end properly
                return true;
            }
            catch (Exception aError)
            {
                // retry 
                Debug.Assert(false, aError.Message);
                // ReSharper disable once HeuristicUnreachableCode
                return false;
            }
        }

        private delegate void DHideState(NotificationInfo notifyInfo, NotificationSnapshot state, bool stopTimer);
        private void HideState(NotificationInfo notifyInfo, NotificationSnapshot state, bool stopTimer)
        {
            if (null == state)
                return;

            
            var aParentControl = notifyInfo.Control.Parent ?? notifyInfo.Control;

            Thread2UI.Invoke(aParentControl, () =>
            {
                // revoking hints
                state._settings.RevokeHintApperance(aParentControl, state._hint);

                // revoking colors
                state._settings.RevokeControlAppearance(notifyInfo.Control, notifyInfo.OriginalColors);

                if (stopTimer &&
                    null != state._timer)
                    state._timer.StopTimer();
            },true);
            
        }

        private void HideNotification(NotificationInfo notifyInfo, bool stopTimer, bool enqueueAgain)
        {
            var aState = notifyInfo.CurrentSnapshot;
            if (null == aState)
                return;

            HideState(notifyInfo, aState, stopTimer);

            if (enqueueAgain &&
                null != notifyInfo.CurrentSnapshot)
                notifyInfo.QueueOnTop(notifyInfo.CurrentSnapshot);

            notifyInfo.ClearSnapshot();
        }

        /*
        private void HideSpecificNotification(NotificationInfo notifyInfo, NotificationStyle style, string message)
        {
            Debug.Assert(null != notifyInfo);

            if (null != notifyInfo._currentSnapshot)
            {
                if (notifyInfo._currentSnapshot._style == style &&
                    notifyInfo._currentSnapshot._message.ToLower() ==
                        message.ToLower())
                    HideState(notifyInfo, notifyInfo._currentSnapshot, true);
            }

            foreach (NotificationSnapshot aState in notifyInfo._stateQueue)
            {
                if (aState._style == style &&
                    aState._message.ToLower() ==
                        message.ToLower())
                    HideState(notifyInfo, aState, true);
            }
        }*/


        public void Error(NotificationPriority priority, Control control, String message, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, null, priority, NotificationSeverity.Error, notificationSettings);
        }

        public void Error(NotificationPriority priority, Control control, String message, string caption, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, caption, priority, NotificationSeverity.Error, notificationSettings);
        }

        public void Warning(NotificationPriority priority, Control control, String message, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, null, priority, NotificationSeverity.Warning, notificationSettings);
        }

        public void Warning(NotificationPriority priority, Control control, String message, string caption, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, caption, priority, NotificationSeverity.Warning, notificationSettings);
            //notificationSettings.NotificationStyle = ns;
        }

        public void Info(NotificationPriority priority, Control control, String message, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, null, priority, NotificationSeverity.Info, notificationSettings);
        }

        public void Info(NotificationPriority priority, Control control, String message, string caption, ControlNotificationSettings notificationSettings)
        {
            Invoke(control, message, caption, priority, NotificationSeverity.Info, notificationSettings);
            //notificationSettings.NotificationStyle = ns;
        }

        public void Revoke(Control control)
        {
            if (null == control)
                return;

            NotificationInfo aNotifyInfo;
            if (!_notifications.TryGetValue(control, out aNotifyInfo))
                return;

            //if (aNotifyInfo._forcedNotifying)
            //    return;

            HideNotification(aNotifyInfo, true, false);
            RemoveAllInQueue();
            //PopNotification(aNotifyInfo);
        }

        /*
        public void RevokeSpecific(Control control, NotificationStyle style, string i_strMessage)
        {
            if (null == control)
                return;

            NotificationInfo aNotifyInfo;
            if (!_notifications.TryGetValue(control, out aNotifyInfo))
                return;

            //if (aNotifyInfo._forcedNotifying)
            //    return;

            HideSpecificNotification(aNotifyInfo, style, i_strMessage);
            PopNotification(aNotifyInfo);
        }*/
    }
}
