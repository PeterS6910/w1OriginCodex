using System.Windows.Forms;
using JetBrains.Annotations;

namespace Contal.IwQuick.UI
{
    public class ControlNotificationProfile
    {
        private readonly ControlNotificationSettings _cns;
        private static readonly ControlNotification _notificator = new ControlNotification();

        private readonly Control _control;
        private readonly NotificationPriority _priority = NotificationPriority.Last;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationControl"></param>
        /// <param name="priority"></param>
        /// <param name="parts"></param>
        /// <param name="hintPosition"></param>
        /// <param name="duration"></param>
        public ControlNotificationProfile(
            [NotNull] Control notificationControl, 
            NotificationPriority priority, 
            NotificationParts parts,
            HintPosition hintPosition, 
            int duration)
        {
            Validator.CheckForNull(notificationControl,"notificationControl");

            _cns = new ControlNotificationSettings(parts, hintPosition, duration);
            _priority = priority;
            _control = notificationControl;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationControl"></param>
        /// <param name="parts"></param>
        /// <param name="hintPosition"></param>
        /// <param name="duration"></param>
        public ControlNotificationProfile(
            [NotNull] Control notificationControl, 
            NotificationParts parts, 
            HintPosition hintPosition, 
            int duration)
        {
            Validator.CheckForNull(notificationControl,"notificationControl");

            _cns = new ControlNotificationSettings(parts, hintPosition, duration);
            _priority = NotificationPriority.Last;
            _control = notificationControl;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationControl"></param>
        /// <param name="priority"></param>
        /// <param name="controlNotificationSettings"></param>
        public ControlNotificationProfile(
            [NotNull] Control notificationControl, 
            NotificationPriority priority, 
            [NotNull] ControlNotificationSettings controlNotificationSettings)
        {
            Validator.CheckForNull(notificationControl,"notificationControl");
            Validator.CheckForNull(controlNotificationSettings,"controlNotificationSettings");

            _cns = controlNotificationSettings;
            _priority = priority;
            _control = notificationControl;


        }

        public void Error(string message)
        {
            _notificator.Error(_priority, _control, message, _cns);
        }

        public void Info(string message)
        {
            _notificator.Info(_priority, _control, message, _cns);
        }

        public void Warning(string message)
        {
            _notificator.Warning(_priority, _control, message, _cns);
        }

    }
}
