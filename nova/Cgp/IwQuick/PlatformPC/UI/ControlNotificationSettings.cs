using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Contal.IwQuick.UI
{
    public class ControlNotificationSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public ControlNotificationSettings() {
            SetDefaultAppearances();
        }


        private static ControlNotificationSettings _defaultErrorSettings;
        /// <summary>
        /// 
        /// </summary>
        public static ControlNotificationSettings Default
        {
            get { return _defaultErrorSettings ?? (_defaultErrorSettings = new ControlNotificationSettings()); }
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationParts"></param>
        /// <param name="hintPosition"></param>
        /// <param name="duration"></param>
        public ControlNotificationSettings(
            NotificationParts notificationParts, 
            HintPosition hintPosition, 
            int duration)
        {
            SetDefaultAppearances();

            _hintPosition = hintPosition;

            if (duration > 0)
                _duration = duration;
            _parts = notificationParts;
        }

        public ControlNotificationSettings(NotificationParts notificationParts, HintPosition hintPosition)
            :this(notificationParts,hintPosition,0)
        {
            
        }


        // error appearance
        private readonly ColorPair _errorControlColors = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair ErrorControlColors
        {
            get
            {
                return _errorControlColors;
            }
        }

        private readonly ColorPair _errorHintColor = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair ErrorHintColors
        {
            get { return _errorHintColor; }
        }

        private ToolTipIcon _errorToolTipIcon;
        /// <summary>
        /// 
        /// </summary>
        public ToolTipIcon ErrorToolTipIcon
        {
            get
            {
                return _errorToolTipIcon;
            }
            set
            {
                _errorToolTipIcon = value;
            }
        }

        private string _errorHintTitle;
        /// <summary>
        /// 
        /// </summary>
        public string ErrorHintTitle
        {
            get { return _errorHintTitle; }
            set
            {
                if (Validator.IsNullString(value))
                    return;

                _errorHintTitle = value;
            }
        }

        // warning appearance
        private readonly ColorPair _warningControlColors = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair WarningControlColors
        {
            get
            {
                return _warningControlColors;
            }
        }

        private readonly ColorPair _warningHintColor = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair WarningHintColors
        {
            get { return _warningHintColor; }
        }

        private ToolTipIcon _warningToolTipIcon;
        /// <summary>
        /// 
        /// </summary>
        public ToolTipIcon WarningToolTipIcon
        {
            get
            {
                return _warningToolTipIcon;
            }
            set
            {
                _warningToolTipIcon = value;
            }
        }

        private string _warningHintTitle;
        /// <summary>
        /// 
        /// </summary>
        public string WarningHintTitle
        {
            get { return _warningHintTitle; }
            set
            {
                if (Validator.IsNullString(value))
                    return;

                _warningHintTitle = value;
            }
        }

        // info appearance
        private readonly ColorPair _infoControlColors = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair InfoControlColors
        {
            get
            {
                return _infoControlColors;
            }
        }

        private readonly ColorPair _infoHintColor = new ColorPair();
        /// <summary>
        /// 
        /// </summary>
        public ColorPair InfoHintColors
        {
            get { return _infoHintColor; }
        }

        private ToolTipIcon _infoToolTipIcon;
        /// <summary>
        /// 
        /// </summary>
        public ToolTipIcon InfoToolTipIcon
        {
            get
            {
                return _infoToolTipIcon;
            }
            set
            {
                _infoToolTipIcon = value;
            }
        }

        private string _infoHintTitle;
        /// <summary>
        /// 
        /// </summary>
        public string InfoHintTitle
        {
            get { return _infoHintTitle; }
            set
            {
                if (Validator.IsNullString(value))
                    return;

                _infoHintTitle = value;
            }
        }

        private void SetDefaultAppearances()
        {
            var aHint = new ToolTip();

            InfoHintColors.BackColor =
                ErrorHintColors.BackColor =
                    WarningHintColors.BackColor =
                        aHint.BackColor;

            InfoHintColors.ForeColor =
                ErrorHintColors.ForeColor =
                    WarningHintColors.ForeColor =
                        aHint.ForeColor;

            _errorToolTipIcon = ToolTipIcon.Error;
            _infoToolTipIcon = ToolTipIcon.Info;
            _warningToolTipIcon = ToolTipIcon.Warning;

            _errorHintTitle = "Error";
            _warningHintTitle = "Warning";
            _infoHintTitle = "Information";

            ErrorControlColors.BackColor = Color.Red;
            ErrorControlColors.ForeColor = Color.White;

            WarningControlColors.BackColor = Color.LightYellow;
            WarningControlColors.ForeColor = Color.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hint"></param>
        /// <param name="severity"></param>
        private void SetHintAppearance(
            [NotNull] ToolTip hint,
            NotificationSeverity severity,
            string caption)
        {
            switch (severity)
            {
                case NotificationSeverity.Error:
                    hint.BackColor = ErrorHintColors.BackColor;
                    hint.ForeColor = ErrorHintColors.ForeColor;
                    hint.ToolTipIcon = _errorToolTipIcon;
                    hint.ToolTipTitle = caption ?? _errorHintTitle;
                    break;
                case NotificationSeverity.Warning:
                    hint.BackColor = WarningHintColors.BackColor;
                    hint.ForeColor = WarningHintColors.ForeColor;
                    hint.ToolTipIcon = _warningToolTipIcon;
                    hint.ToolTipTitle = caption ?? _warningHintTitle;
                    break;
                case NotificationSeverity.Info:
                    hint.BackColor = InfoHintColors.BackColor;
                    hint.ForeColor = InfoHintColors.ForeColor;
                    hint.ToolTipIcon = _infoToolTipIcon;
                    hint.ToolTipTitle = caption ?? _infoHintTitle;
                    break;
            }
        }

        private NotificationParts _parts = NotificationParts.All;
        /// <summary>
        /// 
        /// </summary>
        public NotificationParts Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }

        private int _duration = 4000;
        /// <summary>
        /// 
        /// </summary>
        public int Duration {
            get { return _duration;}
            set
            {
                if (value < 1)
                    throw new OutOfRangeException(value, 1, false);

                _duration = value;
            }
        }

        private HintPosition _hintPosition = HintPosition.Bottom;
        /// <summary>
        /// 
        /// </summary>
        public HintPosition HintPosition
        {
            get { return _hintPosition; }
            set { _hintPosition = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="severity"></param>
        public void SetControlApperance(
            Control control, 
            NotificationSeverity severity)
        {
            if (null == control)
                return;

            if (control is Button)
                return;

            Control parentControl = control.Parent
                // in case of forms
                ?? control;
            

            if (parentControl.InvokeRequired)
            {
                parentControl.BeginInvoke(new Action<Control,NotificationSeverity>(SetControlApperance), control, severity);
            }
            else
            {

                switch (severity)
                {
                    case NotificationSeverity.Error:
                        ErrorControlColors.Apply(control);
                        break;
                    case NotificationSeverity.Warning:
                        WarningControlColors.Apply(control);
                        break;
                    case NotificationSeverity.Info:
                        InfoControlColors.Apply(control);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void CountHintPosition(
            [NotNull] Control control, 
            ref int newX, 
            ref int newY)
        {
            Validator.CheckForNull(control,"control");

            Control aParentControl = control.Parent
                // in case of forms
                ?? control;
            

            int iXShift = 0;
            int iYShift = 0;

            // if the parent control is the topmost component ( usually Form )
            if (null == aParentControl.Parent)
            {
                Rectangle aRect = aParentControl.RectangleToScreen(aParentControl.ClientRectangle);
                iXShift = aRect.Left - aParentControl.Left;
                iYShift = aRect.Top - aParentControl.Top;
            }

            
            // MDI childs form hack
            if (aParentControl is Form && ((Form)aParentControl).IsMdiChild)
            {
                
                iXShift = aParentControl.Size.Width - aParentControl.ClientSize.Width - aParentControl.Margin.Left;
                iYShift = aParentControl.Size.Height - aParentControl.ClientSize.Height - aParentControl.Margin.Top;
                
            }

            switch (_hintPosition)
            {
                case HintPosition.Bottom:
                    newX = control.Left + iXShift;
                    newY = control.Top + control.Height + iYShift;
                    break;
                case HintPosition.RightTop:
                    newX = control.Left + control.Width + iXShift;
                    newY = control.Top + iYShift;
                    break;
                case HintPosition.Center:
                    newX = iXShift + control.Left + control.Width / 2;
                    newY = iYShift + control.Top + control.Height / 2;
                    break;
                case HintPosition.RightCenter:
                    newX = control.Left + control.Width + iXShift;
                    newY = iYShift + control.Top + control.Height / 2;
                    break;
                case HintPosition.RightBottom:
                    newX = control.Left + control.Width + iXShift;
                    newY = iYShift + control.Top + control.Height;
                    break;
                case HintPosition.LeftTop:
                    newX = control.Left + iXShift;
                    newY = control.Top + iYShift;
                    break;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="severity"></param>
        public void ApplyControlAppearance(
            [NotNull] Control control,
            NotificationSeverity severity)
        {
            if (NotificationParts.All == _parts ||
                NotificationParts.Colors == _parts)
            {
                Validator.CheckForNull(control,"control");

                SetControlApperance(control,severity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="control"></param>
        /// <param name="hint"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        public void ApplyHintAppearance(
            [NotNull] Control parentControl,
            Control control,
            ToolTip hint, 
            NotificationSeverity severity, 
            string message,
            string caption)
        {
            if (NotificationParts.All == _parts ||
                NotificationParts.Hint == _parts)
            {
                Validator.CheckForNull(parentControl, "parentControl");
                Validator.CheckForNull(control, "control");
                Validator.CheckForNull(hint, "hint");

                // reset the visual properties before each show, because other 
                // appearance type of this tooltip can be visible previously

                SetHintAppearance(hint,severity,caption);

                int iX = 0; int iY = 0;

                CountHintPosition(control, ref iX, ref iY);

                hint.ShowAlways = true;
                hint.Show(message??String.Empty, parentControl, iX, iY);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="originalColors"></param>
        public void RevokeControlAppearance(
            [NotNull] Control control, 
            [NotNull] ColorPair originalColors)
        {
            // revoking colors
            if (NotificationParts.All == _parts ||
                NotificationParts.Colors == _parts)
            {
                Validator.CheckForNull(control,"control");
                Validator.CheckForNull(originalColors,"originalColors");

                originalColors.Apply(control);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="hint"></param>
        public void RevokeHintApperance(
            [NotNull] Control parentControl, 
            [NotNull] ToolTip hint)
        {
            // revoking hints
            if (NotificationParts.All == _parts ||
                NotificationParts.Hint == _parts)
            {
                Validator.CheckForNull(parentControl,"parentControl");
                Validator.CheckForNull(hint,"hint");
                try
                {
                    hint.Hide(parentControl);
                }
                catch(Exception e)
                {
                    DebugHelper.Keep(e,parentControl,hint);
                }
            }
            
        }
    }
}
