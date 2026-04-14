using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Client;
using Contal.IwQuick;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for TimeBuyingControl.xaml
    /// </summary>
    public partial class TimeBuyingControl : UserControl
    {
        private Guid _idAlarmArea;
        private AlarmArea _alarmArea;
        private Action<Guid, byte, int, int> _eventTimeBuyingFailed;
        private Action<Guid, string, int, int> _eventBoughtTimeChanged;

        public delegate void TimeBuyingPanelClosed(TimeBuyingControl sender);
        public event TimeBuyingPanelClosed Closed;

        public TimeBuyingControl(Guid idAlarmArea)
        {
            InitializeComponent();
            _idAlarmArea = idAlarmArea;
            _alarmArea = GraphicsScene.MainServerProvider.AlarmAreas.GetObjectById(_idAlarmArea);
            _lAlarmAreaName.Content = string.IsNullOrEmpty(_alarmArea.ShortName) 
                ? _alarmArea.ToString() 
                : _alarmArea.ShortName;

            _eventTimeBuyingFailed = EventTimeBuyingFailed;
            AlarmAreaTimeBuyingHandler.Singleton.RegisterTimeBuyingFailed(_eventTimeBuyingFailed);

            _eventBoughtTimeChanged = EventBoughtTimeChanged;
            AlarmAreaTimeBuyingHandler.Singleton.RegisterBoughtTimeChanged(_eventBoughtTimeChanged);
        }

        private void _bBuyTime_OnClick(object sender, RoutedEventArgs e)
        {
            if (_alarmArea == null)
            {
                Close();
                return;
            }

            if (_alarmArea.TimeBuyingEnabled)
            {
                int timeToBuy = _dtTimeToBuy.Value.Value.Hour * 3600 + _dtTimeToBuy.Value.Value.Minute * 60 + _dtTimeToBuy.Value.Value.Second;

                if (timeToBuy <= 0)
                {
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditFormEnterAmountOfTimeToBuy"));
                }
                else
                {
                    var result =
                        GraphicsScene.MainServerProvider.AlarmAreas.UnsetAlarmArea(_idAlarmArea, timeToBuy);

                    if (result != AlarmAreaActionResult.Success)
                    {
                        SetUnsetAlarmAreaResult(result, timeToBuy > 0);
                    }
                }
            }
        }

        private void SetUnsetAlarmAreaResult(AlarmAreaActionResult result, bool timeBuyingRequested)
        {
            switch (result)
            {
                case AlarmAreaActionResult.Success:
                    if (_alarmArea.TimeBuyingEnabled && timeBuyingRequested)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetRequested"));
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetSucceeded"));
                    }
                    break;

                case AlarmAreaActionResult.FailedNoImplicitManager:
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetFailedNoImplicitManager"));
                    break;

                case AlarmAreaActionResult.FailedCCUOffline:
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetFailedCCUOffline"));
                    break;

                case AlarmAreaActionResult.SetUnsetNotConfirm:
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetFailedNotConfirm"));
                    break;

                case AlarmAreaActionResult.FailedInsufficientRights:
                    Contal.IwQuick.UI.Dialog.Error(string.Format(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetFailedInsufficientRights"),
                            "\"" + CgpClient.Singleton.LoggedAs + "\""));
                    break;

                default:
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_UnsetFailed"));
                    break;
            }
        }

        private void _bClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            Visibility = Visibility.Collapsed;
            AlarmAreaTimeBuyingHandler.Singleton.UnregisterTimeBuyingFailed(_eventTimeBuyingFailed);
            AlarmAreaTimeBuyingHandler.Singleton.UnregisterBoughtTimeChanged(_eventBoughtTimeChanged);

            if (Closed != null)
                Closed(this);
        }

        private void EventBoughtTimeChanged(Guid idAlarmArea, string userName, int boughtTime, int remainingTime)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                Close();

                Contal.IwQuick.UI.Dialog.Info(string.Format(
                GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_EventBoughtTimeChanged"),
                GetTimeIntervalForTimeBuyingResult(boughtTime, false),
                GetTimeIntervalForTimeBuyingResult(remainingTime, false)));
            }));
        }

        private void EventTimeBuyingFailed(Guid idAlarmArea, byte reason, int timeToBuy, int remainingTime)
        {
            string actionResult = GraphicsScene.LocalizationHelper.GetString(
                    "AlarmAreaActionResult_"
                    + ((AlarmAreaActionResult)reason).ToString());

            Contal.IwQuick.UI.Dialog.Error(string.Format(
                    GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_EventTimeBuyingFailed"),
                    actionResult,
                    GetTimeIntervalForTimeBuyingResult(timeToBuy, false),
                    (remainingTime >= 0
                        ? GraphicsScene.LocalizationHelper.GetString("remaining")
                        : GraphicsScene.LocalizationHelper.GetString("missing")),
                    GetTimeIntervalForTimeBuyingResult(remainingTime, true)));
        }

        private string GetTimeIntervalForTimeBuyingResult(int time, bool negative)
        {
            string result;

            Func<int, TimeSpan> timeCalc = (timeInSeconds) =>
            {
                int hours = timeInSeconds / 3600;
                int seconds = timeInSeconds % 3600;
                int minutes = timeInSeconds / 600;
                seconds -= minutes * 60;

                return new TimeSpan(hours, minutes, seconds);
            };

            if (time == int.MinValue)
            {
                result = GraphicsScene.LocalizationHelper.GetString("Unknown");
            }
            else if (time == int.MaxValue)
            {
                result = GraphicsScene.LocalizationHelper.GetString("Unlimited");
            }
            else if (time < 0)
            {
                if (negative)
                    result = string.Format("{0:hh\\:mm\\:ss}", timeCalc(time * -1));
                else
                    result = GraphicsScene.LocalizationHelper.GetString("Unknown");
            }
            else
            {
                result = string.Format("{0:hh\\:mm\\:ss}", timeCalc(time));
            }

            return result;
        }
    }
}
