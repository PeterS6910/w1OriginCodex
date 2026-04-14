using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;
using Contal.Drivers.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class SensorsForAlarmAreaMenuScene<TMenuItemsProvider> :
        CrMenuScene
        where TMenuItemsProvider : SensorsForAlarmAreaMenuScene<TMenuItemsProvider>.AMenuItemsProvider
    {
        public abstract class AMenuItemsProvider :
            ACrMenuSceneItemsProvider<TMenuItemsProvider>,
            AlarmAreaSensorListener.IEventHandler,
            ICcuMenuItemsProvider
        {
            private class SensorMenuItem : ACcuMenuItem<TMenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
                {
                    private readonly ISensorStateAndSettings _sensorStateAndSettings;

                    public RouteProvider(
                        ISensorStateAndSettings sensorStateAndSettings)
                    {
                        _sensorStateAndSettings = sensorStateAndSettings;
                    }

                    public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
                    {
                        return new SensorEditSceneGroup(
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup,
                            _sensorStateAndSettings).EnterRoute;
                    }
                }

                public ISensorStateAndSettings SensorStateAndSettings
                {
                    get;
                    private set;
                }

                public SensorMenuItem(ISensorStateAndSettings sensorStateAndSettings)
                    : base(new RouteProvider(sensorStateAndSettings))
                {
                    SensorStateAndSettings = sensorStateAndSettings;
                }

                protected override string GetText(TMenuItemsProvider menuItemsProvider)
                {
                    var text = string.Format(
                        "{0}{1} {2}",
                        menuItemsProvider.SensorsForAlarmAreaSceneGroup.CrAlarmAreaInfo.AlarmArea.Id.ToString("D2"),
                        SensorStateAndSettings.SensorId.ToString("D2"),
                        SensorStateAndSettings.NickName);

                    if (menuItemsProvider.CardReaderSettings.IsPremium)
                        return text;

                    return SensorStateAndSettings.IsInAlarm
                        ? "A  " + text
                        : (SensorStateAndSettings.IsInTamper
                            ? "S! " + text
                            : text);
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
                {
                    if (!menuItemsProvider.CardReaderSettings.IsPremium)
                        yield break;

                    if (SensorStateAndSettings.IsInAlarm)
                    {

                        yield return SensorStateAndSettings.IsNotAcknowledged
                            ? CrIconSymbol.SensorsInAlarmNotAcknowledged
                            : CrIconSymbol.SensorsInAlarm;

                        yield break;
                    }

                    if (SensorStateAndSettings.IsInTamper)
                        yield return CrIconSymbol.SensorsInSabotage;
                    else
                        switch (SensorStateAndSettings.SensorBlockingType)
                        {
                            case SensorBlockingType.BlockPermanently:

                                yield return CrIconSymbol.SensorsPermanentlyBlocked;
                                break;

                            case SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset:
                            case SensorBlockingType.BlockTemporarilyUntilSensorStateNormal:

                                yield return CrIconSymbol.SensorsTemporarilyBlocked;
                                break;

                            default:

                                yield return SensorStateAndSettings.IsNotAcknowledged
                                    ? CrIconSymbol.SensorsNormalNotAcknowledged
                                    : CrIconSymbol.SensorsNormal;

                                yield break;
                        }

                    if (SensorStateAndSettings.IsNotAcknowledged)
                        yield return CrIconSymbol.SensorsNormalNotAcknowledged;
                }

                public override bool IsVisible(TMenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider.IsSensorVisible(SensorStateAndSettings);
                }
            }

            public readonly ISensorsForAlarmAreaSceneGroup SensorsForAlarmAreaSceneGroup;

            protected AMenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(menuSceneProvider)
            {
                SensorsForAlarmAreaSceneGroup = sensorsForAlarmAreaSceneGroup;
            }

            protected virtual IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> CreateGeneralItems()
            {
                yield break;
            }

            private readonly SyncDictionary<Guid, SensorMenuItem> _sensorMenuItems =
                new SyncDictionary<Guid, SensorMenuItem>();

            private readonly ICollection<ACrMenuSceneItem<TMenuItemsProvider>> _generalItems =
                new LinkedList<ACrMenuSceneItem<TMenuItemsProvider>>();

            public IEnumerable<ISensorStateAndSettings> VisibleSensors
            {
                get
                {
                    return _sensorMenuItems.ValuesSnapshot
                        .Where(menuItem => menuItem.IsVisible(This))
                        .Select(menuItem => menuItem.SensorStateAndSettings)
                        .ToArray();
                }
            }

            protected override IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> OnEnteredInternal()
            {
                SensorsForAlarmAreaSceneGroup.AlarmAreaSensorListener.AttachSensorEventHandler(this);

                foreach (var generalMenuItem in CreateGeneralItems())
                {
                    _generalItems.Add(generalMenuItem);
                    yield return generalMenuItem;
                }

                foreach (var sensorMenuItem in _sensorMenuItems.ValuesSnapshot)
                    yield return sensorMenuItem;
            }

            protected override void OnExitedInternal()
            {
                SensorsForAlarmAreaSceneGroup.AlarmAreaSensorListener.DetachSensorEventHandler(this);
            }

            protected abstract bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings);

            public ACardReaderSettings CardReaderSettings
            {
                get
                {
                    return SensorsForAlarmAreaSceneGroup.CardReaderSettings;
                }
            }
           
            public void OnAttached(ICollection<ISensorStateAndSettings> sensors)
            {
                foreach (var sensorStateAndSetting in sensors)
                    _sensorMenuItems.Add(
                        sensorStateAndSetting.IdInput,
                        new SensorMenuItem(sensorStateAndSetting));
            }

            public void OnDetached()
            {
                _sensorMenuItems.Clear();
            }

            public virtual void OnSensorAdded(
                ISensorStateAndSettings sensorStateAndSettings,
                ISensorStateAndSettings predecessor)
            {
                var sensorMenuItem = new SensorMenuItem(sensorStateAndSettings);

                _sensorMenuItems.Add(
                    sensorStateAndSettings.IdInput,
                    sensorMenuItem);

                SensorMenuItem predecessorMenuItem = null;

                if (predecessor != null)
                    _sensorMenuItems.TryGetValue(
                        predecessor.IdInput,
                        out predecessorMenuItem);

                InsertItem(
                    CardReaderSettings.SceneContext,
                    sensorMenuItem,
                    predecessorMenuItem);
            }

            public virtual void OnSensorRemoved(ISensorStateAndSettings sensorStateAndSettings)
            {
                _sensorMenuItems.Remove(
                    sensorStateAndSettings.IdInput,
                    (key, removed, value) =>
                    {
                        if (removed)
                            RemoveItem(
                                CardReaderSettings.SceneContext,
                                value);
                    });
            }

            private void UpdateSensor(ISensorStateAndSettings sensorStateAndSettings)
            {
                SensorMenuItem sensorMenuItem;

                if (!_sensorMenuItems.TryGetValue(
                    sensorStateAndSettings.IdInput,
                    out sensorMenuItem))
                {
                    return;
                }

                foreach (var generalItem in _generalItems)
                    UpdateItem(
                        CardReaderSettings.SceneContext,
                        generalItem);

                UpdateItem(
                    CardReaderSettings.SceneContext,
                    sensorMenuItem);
            }

            void AlarmAreaSensorListener.IEventHandler.OnSensorBlockingTypeChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                SensorBlockingType sensorBlockingType)
            {
                UpdateSensor(sensorStateAndSettings);
            }

            void AlarmAreaSensorListener.IEventHandler.OnSensorInAlarmStateChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool inAlarm)
            {
                UpdateSensor(sensorStateAndSettings);
            }

            void AlarmAreaSensorListener.IEventHandler.OnSensorInTamperStateChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool inTamper)
            {
                UpdateSensor(sensorStateAndSettings);
            }

            void AlarmAreaSensorListener.IEventHandler.OnSensorNotAcknowledgedChanged(
                ISensorStateAndSettings sensorStateAndSettings,
                bool newValue)
            {
                UpdateSensor(sensorStateAndSettings);
            }
        }

        private readonly CrDisplayProcessor _crDisplayProcessor;

        public SensorsForAlarmAreaMenuScene(
            ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
            TMenuItemsProvider menuItemsProvider)
            : base(
                menuItemsProvider,
                MenuConfigurations.GetSensorsByAlarmAreaMenuConfiguration(sensorsForAlarmAreaSceneGroup.CardReaderSettings.CardReader),
                sensorsForAlarmAreaSceneGroup.DefaultGroupExitRoute,
                sensorsForAlarmAreaSceneGroup.TimeOutGroupExitRoute)
        {
            _crDisplayProcessor = sensorsForAlarmAreaSceneGroup.CardReaderSettings.CrDisplayProcessor;
        }

        protected override void ShowNoMenuItems(CardReader cardReader)
        {
            _crDisplayProcessor.DisplayWriteText(
                _crDisplayProcessor.GetLocalizationString(
                    "NoItemsToDisplay"),
                0,
                0);
        }
    }
}