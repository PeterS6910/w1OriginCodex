using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal interface IAlarmAreaActivationEventHandler : IEquatable<IAlarmAreaActivationEventHandler>
    {
        void OnActivationStateChanged(State activationState);
    }

    internal interface IAlarmAreaEventHandler : IEquatable<IAlarmAreaEventHandler>
    {
        void OnActivationStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            State activationState);

        void OnAlarmStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            State alarmState);

        void OnSpecificCrReportingChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool? isCrReportingEnabled);

        void OnNotAcknolwedgedStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool notAcknowledged);

        void OnSetUnsetTimeout(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool failedOperationIsSet);

        void OnSetUnsetStarted(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool operationStartedIsSet);

        void OnAnySensorInAlarmChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value);

        void OnAnySensorInTamperChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value);

        void OnAnySensorNotAcknowledgedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value);

        void OnAnySensorTemporarilyBlockedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value);

        void OnAnySensorPermanentlyBlockedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value);
    }
}