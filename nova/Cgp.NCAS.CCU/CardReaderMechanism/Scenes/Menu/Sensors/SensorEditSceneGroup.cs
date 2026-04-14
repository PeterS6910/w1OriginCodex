using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class SensorEditSceneGroup :
            CrSimpleSceneGroup,
            IInstanceProvider<SensorEditSceneGroup>
    {
        private readonly ISensorStateAndSettings _sensorStateAndSettings;
        private readonly ISensorsForAlarmAreaSceneGroup _sensorsForAlarmAreaSceneGroup;

        private readonly CrSceneGroupExitRoute _timeoutGroupExitRoute;

        private class SensorEditScene : CrMenuScene
        {
            private abstract class ASensorEditMenuItemRouteProvider :
                IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    var sensorEditMenuSceneGroup = menuItemsProvider.SensorEditSceneGroup;

                    var sensorsForAlarmAreaMenuSceneGroup =
                        sensorEditMenuSceneGroup._sensorsForAlarmAreaSceneGroup;

                    var idCardReader =
                        sensorsForAlarmAreaMenuSceneGroup.CardReaderSettings.Id;

                    return CreateInstance(
                        menuItemsProvider,
                        sensorEditMenuSceneGroup._sensorStateAndSettings,
                        idCardReader,
                        sensorsForAlarmAreaMenuSceneGroup.AccessData);
                }

                protected abstract ACrSceneRoute CreateInstance(
                    MenuItemsProvider menuItemsProvider,
                    ISensorStateAndSettings sensorStateAndSettings,
                    Guid idCardReader,
                    AccessDataBase accessData);
            }

            private class AcknowledgeSensorAlarmMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(
                        MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        sensorStateAndSettings.AcknowledgeAlarms(
                            idCardReader,
                            accessData);

                        return CrSceneGroupReturnRoute.Default;
                    }
                }

                public AcknowledgeSensorAlarmMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AcknowledgeSensorAlarm;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUACKNOWLEDGESENSORALARM;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings.IsNotAcknowledged;
                }
            }

            private class AcknowledgeAndTemporarilyBlockSensorAlarmMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(
                        MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        sensorStateAndSettings.BlockTemporarilyAndAcknowledge(
                            idCardReader,
                            accessData);

                        return CrSceneGroupReturnRoute.Default;
                    }
                }

                public AcknowledgeAndTemporarilyBlockSensorAlarmMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUACKNOWLEDGEANDTEMPORARILYBLOCKSENSORALARM;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AcknowledgeSensorAlarm;
                    yield return CrIconSymbol.TemporarilyBlockSensorAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings = 
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    return
                        sensorStateAndSettings.IsNotAcknowledged 
                        && sensorStateAndSettings.SensorBlockingType != SensorBlockingType.Unblocked
                        && sensorStateAndSettings.SensorBlockingType != SensorBlockingType.BlockPermanently;
                }
            }

            private class AcknowledgeAndPermanentlyBlockSensorAlarmMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        sensorStateAndSettings.BlockPermanentlyAndAcknowledge(
                            idCardReader,
                            accessData);

                        return CrSceneGroupReturnRoute.Default;
                    }
                }

                public AcknowledgeAndPermanentlyBlockSensorAlarmMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUACKNOWLEDGEANDPERMANENTLYBLOCKSENSORALARM;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AcknowledgeSensorAlarm;
                    yield return CrIconSymbol.PermanentlyBlockSensorAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings =
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    return
                        sensorStateAndSettings.IsNotAcknowledged 
                        && sensorStateAndSettings.SensorBlockingType == SensorBlockingType.BlockPermanently;
                }
            }

            private class PermanentlyBlockSensorMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(
                        MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        sensorStateAndSettings.EnqueueSetSensorBlockingTypeRequest(
                            SensorBlockingType.BlockPermanently,
                            idCardReader,
                            accessData);

                        return CrSceneGroupReturnRoute.Default;
                    }
                }

                public PermanentlyBlockSensorMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUBLOCKSENSORPERMANENTLY;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.PermanentlyBlockSensorAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings =
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    return sensorStateAndSettings.SensorBlockingType != SensorBlockingType.BlockPermanently;
                }
            }

            private class TemporarilyBlockSensorMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        sensorStateAndSettings.EnqueueSetSensorBlockingTypeRequest(
                            SensorBlockingType.BlockTemporarilyUntilSensorStateNormal,
                            idCardReader,
                            accessData);

                        return CrSceneGroupReturnRoute.Default;
                    }
                }

                public TemporarilyBlockSensorMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings =
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    switch (sensorStateAndSettings.DefaultBlockTemporarilyUntilType)
                    {
                        case DB.BlockTemporarilyUntilType.AreaUnset:
                            return CardReaderConstants.MENUBLOCKSENSORTEMPORARILYAREAUNSET;

                        default:
                            return CardReaderConstants.MENUBLOCKSENSORTEMPORARILYSENSORSTATENORMAL;
                    }
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.TemporarilyBlockSensorAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings =
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    return
                        sensorStateAndSettings.SensorBlockingType
                            != SensorBlockingType.BlockTemporarilyUntilAlarmAreaUnset
                        && sensorStateAndSettings.SensorBlockingType
                            != SensorBlockingType.BlockTemporarilyUntilSensorStateNormal;
                }
            }

            private class UnblockSensorMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : ASensorEditMenuItemRouteProvider
                {
                    protected override ACrSceneRoute CreateInstance(MenuItemsProvider menuItemsProvider,
                        ISensorStateAndSettings sensorStateAndSettings,
                        Guid idCardReader,
                        AccessDataBase accessData)
                    {
                        return new UnblockAllInputSceneGroup(
                            menuItemsProvider.CardReaderSettings.CrDisplayProcessor,
                            sensorStateAndSettings,
                            idCardReader,
                            accessData).EnterRoute;
                    }
                }

                public UnblockSensorMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUUNBLOCKINPUT;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsPermanentlyBlocked;
                    yield return CrIconSymbol.UnblockSensor;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    var sensorStateAndSettings =
                        menuItemsProvider.SensorEditSceneGroup._sensorStateAndSettings;

                    return
                        sensorStateAndSettings.SensorBlockingType != SensorBlockingType.Unblocked;
                }
            }

            private class MenuItemsProvider :
                ACrMenuSceneItemsProvider<MenuItemsProvider>,
                AlarmAreaSensorListener.IEventHandler,
                ICcuMenuItemsProvider
            {
                public readonly SensorEditSceneGroup SensorEditSceneGroup;

                public MenuItemsProvider(
                    SensorEditSceneGroup sensorEditSceneGroup,
                    IInstanceProvider<CrMenuScene> menuSceneProvider)
                    : base(menuSceneProvider)
                {
                    SensorEditSceneGroup = sensorEditSceneGroup;
                }

                protected override MenuItemsProvider This
                {
                    get { return this; }
                }

                private ICollection<ACrMenuSceneItem<MenuItemsProvider>> _menuItems;

                private static IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateMenuItems()
                {
                    yield return new AcknowledgeSensorAlarmMenuItem();
                    yield return new AcknowledgeAndTemporarilyBlockSensorAlarmMenuItem();
                    yield return new AcknowledgeAndPermanentlyBlockSensorAlarmMenuItem();

                    yield return new TemporarilyBlockSensorMenuItem();
                    yield return new PermanentlyBlockSensorMenuItem();

                    yield return new UnblockSensorMenuItem();
                }

                protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
                {
                    SensorEditSceneGroup._sensorsForAlarmAreaSceneGroup.AlarmAreaSensorListener
                        .AttachSensorEventHandler(this);

                    _menuItems =
                        new LinkedList<ACrMenuSceneItem<MenuItemsProvider>>(CreateMenuItems());

                    return _menuItems;
                }

                protected override void OnExitedInternal()
                {
                    SensorEditSceneGroup._sensorsForAlarmAreaSceneGroup.AlarmAreaSensorListener
                        .DetachSensorEventHandler(this);
                }

                public ACardReaderSettings CardReaderSettings
                {
                    get { return SensorEditSceneGroup.CardReaderSettings; }
                }

                public void OnAttached(ICollection<ISensorStateAndSettings> sensors)
                {
                }

                public void OnDetached()
                {
                }

                public void OnSensorAdded(
                    ISensorStateAndSettings sensorStateAndSettings,
                    ISensorStateAndSettings predecessor)
                {
                }

                public void OnSensorRemoved(ISensorStateAndSettings sensorStateAndSettings)
                {
                    //TODO exit
                }

                public void OnSensorBlockingTypeChanged(
                    ISensorStateAndSettings sensorStateAndSettings,
                    SensorBlockingType sensorBlockingType)
                {
                    if (!sensorStateAndSettings.IdInput.Equals(
                            SensorEditSceneGroup._sensorStateAndSettings.IdInput))
                        return;

                    foreach (var menuItem in _menuItems)
                        UpdateItem(
                            CardReaderSettings.SceneContext,
                            menuItem);
                }

                public void OnSensorInAlarmStateChanged(
                    ISensorStateAndSettings sensorStateAndSettings,
                    bool inAlarm)
                {
                    if (!sensorStateAndSettings.IdInput.Equals(
                            SensorEditSceneGroup._sensorStateAndSettings.IdInput))
                        return;

                    foreach (var menuItem in _menuItems)
                        UpdateItem(
                            CardReaderSettings.SceneContext,
                            menuItem);
                }

                public void OnSensorInTamperStateChanged(
                    ISensorStateAndSettings sensorStateAndSettings,
                    bool inTamper)
                {
                    if (!sensorStateAndSettings.IdInput.Equals(
                            SensorEditSceneGroup._sensorStateAndSettings.IdInput))
                        return;

                    foreach (var menuItem in _menuItems)
                        UpdateItem(
                            CardReaderSettings.SceneContext,
                            menuItem);
                }

                public void OnSensorNotAcknowledgedChanged(
                    ISensorStateAndSettings sensorStateAndSettings,
                    bool newValue)
                {
                    if (!sensorStateAndSettings.IdInput.Equals(
                            SensorEditSceneGroup._sensorStateAndSettings.IdInput))
                        return;

                    foreach (var menuItem in _menuItems)
                        UpdateItem(
                            CardReaderSettings.SceneContext,
                            menuItem);
                }
            }

            public SensorEditScene(
                SensorEditSceneGroup sensorEditSceneGroup)
                : this(
                    sensorEditSceneGroup,
                    new DelayedInitReference<CrMenuScene>())
            {

            }

            private SensorEditScene(
                SensorEditSceneGroup sensorEditSceneGroup,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(
                        sensorEditSceneGroup,
                        delayedInitReference),
                    MenuConfigurations.GetSensorEditMenuConfiguration(sensorEditSceneGroup._sensorsForAlarmAreaSceneGroup.CardReaderSettings.CardReader),
                    sensorEditSceneGroup.DefaultGroupExitRoute,
                    sensorEditSceneGroup._timeoutGroupExitRoute)
            {
                delayedInitReference.Instance = this;
            }
        }

        private ACardReaderSettings CardReaderSettings
        {
            get
            {
                return _sensorsForAlarmAreaSceneGroup.CardReaderSettings;
            }
        }

        public SensorEditSceneGroup(
            ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
            ISensorStateAndSettings sensorStateAndSettings)
            : this(new DelayedInitReference<ICrScene>(),
                sensorStateAndSettings,
                sensorsForAlarmAreaSceneGroup)
        {

        }

        private SensorEditSceneGroup(
            DelayedInitReference<ICrScene> sceneProvider,
            ISensorStateAndSettings sensorStateAndSettings,
            ISensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup)
            : base(
                sceneProvider,
                CrSceneGroupReturnRoute.Default)
        {
            _sensorStateAndSettings = sensorStateAndSettings;
            _sensorsForAlarmAreaSceneGroup = sensorsForAlarmAreaSceneGroup;

            _timeoutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                sensorsForAlarmAreaSceneGroup.TimeOutGroupExitRoute);

            sceneProvider.Instance = new SensorEditScene(this);
        }

        public SensorEditSceneGroup Instance
        {
            get { return this; }
        }
    }
}

internal class UnblockAllInputSceneGroup : CrSimpleSceneGroup
{
    private readonly CrDisplayProcessor _crDisplayProcessor;

    private readonly ISensorStateAndSettings _sensorStateAndSettings;

    private readonly Guid _idCardReader;
    private readonly AccessDataBase _accessData;

    private class QuestionScene : CrBaseScene
    {
        private readonly UnblockAllInputSceneGroup _sceneGroup;

        public QuestionScene(UnblockAllInputSceneGroup sceneGroup)
        {
            _sceneGroup = sceneGroup;
        }

        private void Unblock()
        {
            _sceneGroup._sensorStateAndSettings.EnqueueSetSensorBlockingTypeRequest(
                SensorBlockingType.Unblocked,
                _sceneGroup._idCardReader,
                _sceneGroup._accessData);
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            if (Inputs.Singleton.IsInputBlockedAndCurrentStateIsNotNormal(_sceneGroup._sensorStateAndSettings.IdInput))
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

                var crDisplayProcessor = _sceneGroup._crDisplayProcessor;

                crDisplayProcessor.DisplayWriteText(
                    crDisplayProcessor.GetLocalizationString("QuestionDoYouWantToProceedWithUnblockingInput"),
                    0,
                    0);

                return true;
            }

            Unblock();
            return false;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.Yes)
                Unblock();

            _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);
        }
    }

    private UnblockAllInputSceneGroup(
        DelayedInitReference<ICrScene> sceneProvider)
        : base(
            sceneProvider,
            CrSceneGroupReturnRoute.Default)
    {
        sceneProvider.Instance =
            new QuestionScene(this);
    }

    public UnblockAllInputSceneGroup(
        CrDisplayProcessor crDisplayProcessor,
        ISensorStateAndSettings sensorStateAndSettings,
        Guid idCardReader,
        AccessDataBase accessData)
        : this(
            new DelayedInitReference<ICrScene>())
    {
        _crDisplayProcessor = crDisplayProcessor;
        _sensorStateAndSettings = sensorStateAndSettings;

        _idCardReader = idCardReader;
        _accessData = accessData;
    }
}
