using System;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal interface ISetUnsetAlarmAreaContext
    {
        IAuthorizedSceneGroup AuthorizedSceneGroup
        {
            get;
        }

        CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get;
        }
    }

    internal abstract class ASetAlarmAreaRouteProviderBase : 
        IInstanceProvider<ACrSceneRoute, CrAlarmAreasManager.CrAlarmAreaInfo, IAuthorizedSceneGroup>
    {
        protected readonly bool NoPrewarning;
        protected readonly IInstanceProvider<ACrSceneRoute> DefaultRouteProvider;

        protected ASetAlarmAreaRouteProviderBase(
            bool noPrewarning,
            IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
        {
            NoPrewarning = noPrewarning;
            DefaultRouteProvider = defaultRouteProvider;
        }

        public ACrSceneRoute GetInstance(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo, 
            IAuthorizedSceneGroup authorizedSceneGroup)
        {
            var accessData = authorizedSceneGroup.AccessData;

            var cardReaderMechanism =
                authorizedSceneGroup
                    .CardReaderSettings;

            if (!CheckRightsForThisAction(
                crAlarmAreaInfo,
                accessData,
                cardReaderMechanism))
            {
                Events.ProcessEvent(
                    new EventCrAccessDeniedSetAlarmAreaNoRights(
                        cardReaderMechanism.Id,
                        accessData,
                        crAlarmAreaInfo.IdAlarmArea));

                return new NoAccessToThisMenuSceneGroup().EnterRoute;
            }

            return
                TrySetAlarmArea(
                    crAlarmAreaInfo, 
                    authorizedSceneGroup)
                ?? TryRecoverFromFailure(
                    crAlarmAreaInfo,
                    authorizedSceneGroup);
        }

        protected abstract ACrSceneRoute TryRecoverFromFailure(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup authorizedSceneGroup);

        protected abstract ACrSceneRoute TrySetAlarmArea(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo, 
            IAuthorizedSceneGroup authorizedSceneGroup);

        protected abstract bool CheckRightsForThisAction(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            AccessDataBase accessData,
            ACardReaderSettings cardReaderSettings);
    }

    internal class SetUnsetRouteProviderAdapter<TMenuItemsProvider> :
        IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
        where TMenuItemsProvider : ISetUnsetAlarmAreaContext
    {
        private readonly IInstanceProvider<ACrSceneRoute, CrAlarmAreasManager.CrAlarmAreaInfo, IAuthorizedSceneGroup> _targetRouteProvider;

        public SetUnsetRouteProviderAdapter(IInstanceProvider<ACrSceneRoute, CrAlarmAreasManager.CrAlarmAreaInfo, IAuthorizedSceneGroup> targetRouteProvider)
        {
            _targetRouteProvider = targetRouteProvider;
        }

        public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
        {
            return _targetRouteProvider.GetInstance(
                menuItemsProvider.CrAlarmAreaInfo, 
                menuItemsProvider.AuthorizedSceneGroup);
        }
    }

    internal class SetAlarmAreaRouteProvider :
        ASetAlarmAreaRouteProviderBase
    {
        public SetAlarmAreaRouteProvider(
            bool noPrewarning,
            IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
            : base(
                noPrewarning, 
                defaultRouteProvider)
        {
        }

        protected override bool CheckRightsForThisAction(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            AccessDataBase accessData,
            ACardReaderSettings cardReaderSettings)
        {
            return
                crAlarmAreaInfo.IsSettable
                && (accessData.IdPerson == Guid.Empty
                    || AlarmAreaAccessRightsManager.Singleton.CheckRigthsToSet(
                        accessData,
                        crAlarmAreaInfo.IdAlarmArea));
        }

        protected override ACrSceneRoute TrySetAlarmArea(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo, 
            IAuthorizedSceneGroup authorizedSceneGroup)
        {
            var accessData = authorizedSceneGroup.AccessData;

            var cardReaderMechanism =
                authorizedSceneGroup.CardReaderSettings;

            var idCardReader = cardReaderMechanism.Id;

            if (!AlarmArea.AlarmAreas.Singleton.SetAlarmArea(
                crAlarmAreaInfo.IdAlarmArea,
                true,
                new AlarmArea.AlarmAreas.SetUnsetParams(
                    idCardReader,
                    accessData,
                    Guid.Empty,
                    false,
                    NoPrewarning)))
            {
                return null;
            }

            Events.ProcessEvent(
                new EventSetAlarmAreaFromCardReader(
                    idCardReader,
                    accessData,
                    crAlarmAreaInfo.IdAlarmArea));

            return new SetAlarmAreaSucceededSceneGroup(
                crAlarmAreaInfo,
                cardReaderMechanism.CrDisplayProcessor,
                DefaultRouteProvider).EnterRoute;
        }

        protected override ACrSceneRoute TryRecoverFromFailure(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup authorizedSceneGroup)
        {
            var accessData = authorizedSceneGroup.AccessData;

            var cardReaderMechanism =
                authorizedSceneGroup.CardReaderSettings;

            var idCardReader = cardReaderMechanism.Id;

            if (crAlarmAreaInfo.IsUnconditionalSettable)
            {
                if ((!accessData.EntryViaCard
                     && !accessData.EntryViaPersonalCode)
                    || AlarmAreaAccessRightsManager.Singleton
                        .CheckRigthsToUnconditionalSet(
                            accessData,
                            crAlarmAreaInfo.IdAlarmArea))
                {
                    return
                        new UnconditionalSetAlarmAreaSceneGroup(
                            crAlarmAreaInfo,
                            authorizedSceneGroup,
                            NoPrewarning,
                            DefaultRouteProvider).EnterRoute;
                }

                Events.ProcessEvent(
                    new EventCrAccessDeniedSetAlarmAreaNoRights(
                        idCardReader,
                        accessData,
                        crAlarmAreaInfo.IdAlarmArea));
            }
            else
                Events.ProcessEvent(
                    new EventAlarmAreaSetFromCrFailed(
                        idCardReader,
                        accessData,
                        crAlarmAreaInfo.IdAlarmArea));

            return
                new SetAlarmAreaFailedSceneGroup(
                    cardReaderMechanism.CrDisplayProcessor,
                    DefaultRouteProvider).EnterRoute;
        }
    }

    internal class UnsetAlarmAreaRouteProvider :
        IInstanceProvider<ACrSceneRoute, CrAlarmAreasManager.CrAlarmAreaInfo, IAuthorizedSceneGroup>
    {
        private class UnsetAlarmAreaResult : AlarmAreaStateAndSettings.IAlarmAreaUnsetResult
        {
            private readonly Guid _idAlarmArea;
            private readonly Guid _idCardReader;
            private readonly AccessDataBase _accessData;

            public UnsetAlarmAreaResult(
                Guid idAlarmArea,
                Guid idCardReader,
                AccessDataBase accessData)
            {
                _idAlarmArea = idAlarmArea;
                _idCardReader = idCardReader;
                _accessData = accessData;
            }

            public bool Success { get; private set; }

            public void OnFailed(AlarmAreaActionResult alarmAreaActionResult, int timeToBuy, int remainingTime)
            {
            }

            public void OnSucceded(
                int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
                Events.ProcessEvent(
                    new EventUnsetAlarmAreaFromCardReader(
                        _idCardReader,
                        _accessData,
                        _idAlarmArea));

                Success = true;

                NonAcknowledgedAlarmDuringSetPeriod = nonAcknowledgedAlarmDuringSetPeriod;
            }

            public bool NonAcknowledgedAlarmDuringSetPeriod
            {
                get;
                private set;
            }
        }

        private readonly IInstanceProvider<ACrSceneRoute> _defaultRouteProvider;

        public UnsetAlarmAreaRouteProvider(IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
        {
            _defaultRouteProvider = defaultRouteProvider;
        }

        public virtual ACrSceneRoute GetInstance(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo, 
            IAuthorizedSceneGroup parentSceneGroup)
        {
            Guid idAlarmArea = crAlarmAreaInfo.IdAlarmArea;

            ACardReaderSettings cardReaderSettings =
                parentSceneGroup.CardReaderSettings;

            if (cardReaderSettings.IsPremium
                && crAlarmAreaInfo.IsTimeBuyingPossible
                && AlarmArea.AlarmAreas.Singleton.IsTimeBuyingEnabled(crAlarmAreaInfo.IdAlarmArea))
            {
                return
                    new AlarmAreaTimeBuyingSceneGroup(
                        crAlarmAreaInfo,
                        parentSceneGroup,
                        _defaultRouteProvider).EnterRoute;
            }

            var accessData = parentSceneGroup.AccessData;

            Guid idCardReader = cardReaderSettings.Id;

            if (!crAlarmAreaInfo.IsUnsettable
                || ((accessData.EntryViaCard
                     || accessData.EntryViaPersonalCode)
                    && !AlarmArea.AlarmAreas.Singleton.CheckUnsetRights(
                        idAlarmArea,
                        accessData.IdPerson)))
            {
                Events.ProcessEvent(
                    new EventCrAccessDeniedUnsetAlarmAreaNoRights(
                        idCardReader,
                        accessData,
                        idAlarmArea));

                return new NoAccessToThisMenuSceneGroup().EnterRoute;
            }

            var alarmAreaUnsetResult = 
                new UnsetAlarmAreaResult(
                    idAlarmArea,
                    idCardReader,
                    accessData);

            AlarmArea.AlarmAreas.Singleton.UnsetAlarmArea(
                alarmAreaUnsetResult,
                true,
                idAlarmArea,
                Guid.Empty,
                accessData.IdPerson,
                0,
                new AlarmArea.AlarmAreas.SetUnsetParams(
                    idCardReader,
                    accessData,
                    Guid.Empty,
                    false,
                    false));

            return !alarmAreaUnsetResult.Success
                ? new UnsetAlarmAreaFailedSceneGroup(
                    crAlarmAreaInfo,
                    cardReaderSettings.CrDisplayProcessor,
                    _defaultRouteProvider).EnterRoute
                : new UnsetAlarmAreaSucceededSceneGroup(
                    crAlarmAreaInfo,
                    cardReaderSettings,
                    alarmAreaUnsetResult.NonAcknowledgedAlarmDuringSetPeriod,
                    _defaultRouteProvider).EnterRoute;
        }
    }
}
