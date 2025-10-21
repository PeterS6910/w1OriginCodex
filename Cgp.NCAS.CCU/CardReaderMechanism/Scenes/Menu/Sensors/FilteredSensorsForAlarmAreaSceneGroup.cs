using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class UnblockAllMenuItem<TMenuItemsProvider> :
        ALocalizedMenuItem<TMenuItemsProvider>
        where TMenuItemsProvider : SensorsForAlarmAreaMenuScene<TMenuItemsProvider>.AMenuItemsProvider
    {
        private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
        {
            public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
            {
                var sensorsForAlarmAreaMenuSceneGroup = menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                var idCardReader =
                    sensorsForAlarmAreaMenuSceneGroup.CardReaderSettings.Id;

                return new UnblockAllInputsSceneGroup(
                    menuItemsProvider.CardReaderSettings.CrDisplayProcessor,
                    menuItemsProvider.VisibleSensors,
                    idCardReader,
                    sensorsForAlarmAreaMenuSceneGroup.AccessData).EnterRoute;
            }
        }

        public UnblockAllMenuItem()
            : base(new RouteProvider())
        {
        }

        protected override string GetLocalizationKey(TMenuItemsProvider menuItemsProvider)
        {
            return CardReaderConstants.MENUSENSORUNBLOCKALL;
        }

        protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
        {
            yield return CrIconSymbol.SensorsPermanentlyBlocked;
            yield return CrIconSymbol.UnblockSensor;
        }
    }

    internal class SensorsInAlarmForAlarmAreaSceneGroup
            : ASensorsForAlarmAreaSceneGroupBase<SensorsInAlarmForAlarmAreaSceneGroup.MenuItemsProvider>
    {
        private class BlockAllMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                private static void BlockAllActiveSensors(MenuItemsProvider menuItemsProvider)
                {
                    var sensorsMenuSceneGroup =
                        menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsMenuSceneGroup.CardReaderSettings.Id;

                    foreach (var sensor in menuItemsProvider.VisibleSensors)
                        sensor.EnqueueSetSensorBlockingTypeRequest(
                            sensor.DefaultBlockTemporarilyUntilType == DB.BlockTemporarilyUntilType.AreaUnset
                                ? SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                                : SensorBlockingType.BlockTemporarilyUntilSensorStateNormal,
                            idCardReader,
                            sensorsMenuSceneGroup.AccessData);
                }

                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    BlockAllActiveSensors(menuItemsProvider);
                    return CrSceneGroupReturnRoute.Default;
                }
            }

            public BlockAllMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUSENSORBLOCKALL;
            }
        }

        internal class MenuItemsProvider
            : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new BlockAllMenuItem();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return sensorStateAndSettings.IsInAlarm;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsInAlarmForAlarmAreaSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return
                new MenuItemsProvider(
                    this,
                    menuSceneProvider);
        }
    }

    internal class SensorsInAlarmOrSabotageForAlarmAreaSceneGroup
        : ASensorsForAlarmAreaSceneGroupBase<SensorsInAlarmOrSabotageForAlarmAreaSceneGroup.MenuItemsProvider>
    {
        private class BlockAllMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                private static void BlockAllSensorsInAlarmOrSabotage(MenuItemsProvider menuItemsProvider)
                {
                    var sensorsMenuSceneGroup =
                        menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsMenuSceneGroup.CardReaderSettings.Id;

                    foreach (var sensor in menuItemsProvider.VisibleSensors)
                        sensor.EnqueueSetSensorBlockingTypeRequest(
                            sensor.DefaultBlockTemporarilyUntilType == DB.BlockTemporarilyUntilType.AreaUnset
                                ? SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                                : SensorBlockingType.BlockTemporarilyUntilSensorStateNormal,
                            idCardReader,
                            sensorsMenuSceneGroup.AccessData);
                }

                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    BlockAllSensorsInAlarmOrSabotage(menuItemsProvider);
                    return CrSceneGroupReturnRoute.Default;
                }
            }

            public BlockAllMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUSENSORBLOCKALL;
            }
        }

        internal class MenuItemsProvider
            : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new BlockAllMenuItem();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return sensorStateAndSettings.IsInAlarm || sensorStateAndSettings.IsInTamper;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsInAlarmOrSabotageForAlarmAreaSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return
                new MenuItemsProvider(
                    this,
                    menuSceneProvider);
        }
    }

    internal class SensorsNotAcknowledgedForAlarmAreasSceneGroup :
            ASensorsForAlarmAreaSceneGroupBase<SensorsNotAcknowledgedForAlarmAreasSceneGroup.MenuItemsProvider>
    {
        private class AcknowledgeAllMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider : IInstanceProvider<
                ACrSceneRoute,
                MenuItemsProvider>
            {
                public ACrSceneRoute GetInstance(
                    MenuItemsProvider menuItemsProvider)
                {
                    AcknowledgeAllSensors(menuItemsProvider);

                    return CrSceneGroupReturnRoute.Default;
                }

                private static void AcknowledgeAllSensors(MenuItemsProvider menuItemsProvider)
                {
                    var sensorsMenuSceneGroup = menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsMenuSceneGroup.CardReaderSettings.Id;

                    foreach (var sensor in menuItemsProvider.VisibleSensors)
                        sensor.AcknowledgeAlarms(
                            idCardReader,
                            sensorsMenuSceneGroup.AccessData);
                }
            }

            public AcknowledgeAllMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUSENSORACKNOWLEDGEALL;
            }

            protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
            {
                //TODO check
                yield return CrIconSymbol.AcknowledgeAll;
            }
        }

        private class AcknowledgeAndBlockAllMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider
                : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    AcknowledgeAndBlockAllSensors(menuItemsProvider);
                    return CrSceneGroupReturnRoute.Default;
                }

                private static void AcknowledgeAndBlockAllSensors(MenuItemsProvider menuItemsProvider)
                {
                    var sensorsMenuSceneGroup = menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsMenuSceneGroup.CardReaderSettings.Id;

                    foreach (var sensor in menuItemsProvider.VisibleSensors)
                        sensor.BlockTemporarilyAndAcknowledge(
                            idCardReader,
                            sensorsMenuSceneGroup.AccessData);
                }
            }

            public AcknowledgeAndBlockAllMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUSENSORACKNOWLEDGEANDBLOCKALL;
            }

            protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
            {
                yield return CrIconSymbol.AcknowledgeAll;
                yield return CrIconSymbol.SensorsPermanentlyBlocked;
            }
        }

        internal class MenuItemsProvider : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new AcknowledgeAllMenuItem();
                yield return new AcknowledgeAndBlockAllMenuItem();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return sensorStateAndSettings.IsNotAcknowledged;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsNotAcknowledgedForAlarmAreasSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return new MenuItemsProvider(
                this,
                menuSceneProvider);
        }
    }

    internal class SensorsTemporarilyBlockedForAlarmAreaSceneGroup
            : ASensorsForAlarmAreaSceneGroupBase<SensorsTemporarilyBlockedForAlarmAreaSceneGroup.MenuItemsProvider>
    {
        internal class MenuItemsProvider : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new UnblockAllMenuItem<MenuItemsProvider>();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                var sensorBlockingType = sensorStateAndSettings.SensorBlockingType;

                return 
                    sensorBlockingType 
                        == SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                    || sensorBlockingType
                        == SensorBlockingType.BlockTemporarilyUntilSensorStateNormal;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsTemporarilyBlockedForAlarmAreaSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return new MenuItemsProvider(
                this,
                menuSceneProvider);
        }
    }

    internal class SensorsPermanentlyBlockedForAlarmAreaSceneGroup
            : ASensorsForAlarmAreaSceneGroupBase<SensorsPermanentlyBlockedForAlarmAreaSceneGroup.MenuItemsProvider>
    {
        public class MenuItemsProvider : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new UnblockAllMenuItem<MenuItemsProvider>();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return sensorStateAndSettings.SensorBlockingType == SensorBlockingType.BlockPermanently;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsPermanentlyBlockedForAlarmAreaSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return
                new MenuItemsProvider(
                    this,
                    menuSceneProvider);
        }
    }

    internal class SensorsInSabotageForAlarmAreaSceneGroup :
        ASensorsForAlarmAreaSceneGroupBase<
            SensorsInSabotageForAlarmAreaSceneGroup.MenuItemsProvider>
    {
        private class BlockAllMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                private static void BlockAllSensorsInSabotage(MenuItemsProvider menuItemsProvider)
                {
                    var sensorsMenuSceneGroup =
                        menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsMenuSceneGroup.CardReaderSettings.Id;

                    foreach (var sensor in menuItemsProvider.VisibleSensors)
                        sensor.EnqueueSetSensorBlockingTypeRequest(
                            sensor.DefaultBlockTemporarilyUntilType == DB.BlockTemporarilyUntilType.AreaUnset
                                ? SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                                : SensorBlockingType.BlockTemporarilyUntilSensorStateNormal,
                            idCardReader,
                            sensorsMenuSceneGroup.AccessData);
                }

                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    BlockAllSensorsInSabotage(menuItemsProvider);
                    return CrSceneGroupReturnRoute.Default;
                }
            }

            public BlockAllMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUSENSORBLOCKALL;
            }
        }

        public class MenuItemsProvider : SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            public MenuItemsProvider(
                ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new BlockAllMenuItem();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return sensorStateAndSettings.IsInTamper;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public SensorsInSabotageForAlarmAreaSceneGroup(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                alarmAreaSensorListener,
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return
                new MenuItemsProvider(
                    this,
                    menuSceneProvider);
        }
    }

    internal class UnblockAllInputsSceneGroup : CrSimpleSceneGroup
    {
        private readonly CrDisplayProcessor _crDisplayProcessor;

        private readonly IEnumerable<ISensorStateAndSettings> _sensors;
        
        private readonly Guid _idCardReader;
        private readonly AccessDataBase _accessData;

        private class QuestionScene : CrBaseScene
        {
            private readonly UnblockAllInputsSceneGroup _sceneGroup;

            public QuestionScene(UnblockAllInputsSceneGroup sceneGroup)
            {
                _sceneGroup = sceneGroup;
            }

            private void UnblockAll()
            {
                if (_sceneGroup._sensors == null)
                    return;

                foreach (var sensorStateAndSettings in _sceneGroup._sensors)
                    sensorStateAndSettings.EnqueueSetSensorBlockingTypeRequest(
                        SensorBlockingType.Unblocked,
                        _sceneGroup._idCardReader,
                        _sceneGroup._accessData);
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                if (_sceneGroup._sensors == null)
                    return false;

                if (_sceneGroup._sensors.Any(
                    input =>
                        Inputs.Singleton.IsInputBlockedAndCurrentStateIsNotNormal(input.IdInput)))
                {
                    var cr = crSceneContext.CardReader;
                    cr.DisplayCommands.ClearAllDisplay(cr);

                    cr.MenuCommands.SetBottomMenuButtons(
                        cr,
                        new CRBottomMenu
                        {
                            Button1 = CRMenuButtonLook.No,
                            Button1ReturnCode = CRSpecialKey.No,
                            Button2 = CRMenuButtonLook.Yes,
                            Button2ReturnCode = CRSpecialKey.Yes,
                            Button3 = CRMenuButtonLook.Clear,
                            Button4 = CRMenuButtonLook.Clear
                        });

                    _sceneGroup._crDisplayProcessor.DisplayWriteText(
                        _sceneGroup._crDisplayProcessor.GetLocalizationString("QuestionDoYouWantToProceedWithUnblockingInputs"),
                        0,
                        0);

                    return true;
                }

                UnblockAll();
                return false;
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                if (specialKey == CRSpecialKey.Yes)
                    UnblockAll();

                _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);
            }
        }

        private UnblockAllInputsSceneGroup(
            DelayedInitReference<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                CrSceneGroupReturnRoute.Default)
        {
            sceneProvider.Instance =
                new QuestionScene(this);
        }

        public UnblockAllInputsSceneGroup(
            CrDisplayProcessor crDisplayProcessor,
            IEnumerable<ISensorStateAndSettings> sensors,
            Guid idCardReader,
            AccessDataBase accessData)
            : this(
                new DelayedInitReference<ICrScene>())
        {
            _crDisplayProcessor = crDisplayProcessor;
            _sensors = sensors;

            _idCardReader = idCardReader;
            _accessData = accessData;
        }
    }
}
