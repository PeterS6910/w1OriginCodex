using System;
using System.Collections.Generic;
using System.Linq;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;

namespace Contal.Cgp.NCAS.CCU.LogicalConfigurator
{
    partial class LogicalConfigurator
    {
        private abstract class SpecialOutputsConfigurator
        {
            private class SpecialOutputListener
                : BoolExpressionStateChangedListener
                , IDisposable
            {
                private readonly IBoolExpression _boolExpression;
                private Guid _idOutput;

                private const string OutputActivator = "BoolExpression";

                private bool _isConfigured;

                public SpecialOutputListener(IBoolExpression boolExpression)
                {
                    _boolExpression = boolExpression;
                    _boolExpression.AddListenerAndGetState(this);
                }

                public override void OnStateChanged(bool? oldState, bool? newState)
                {
                    if (!_isConfigured)
                        return;

                    if (!newState.HasValue)
                        return;

                    if (newState.Value)
                        On();
                    else
                        Off();
                }

                private void On()
                {
                    Outputs.Singleton.On(
                        string.Format(
                            "{0}{1}",
                            _boolExpression.UniqueId,
                            OutputActivator),
                        _idOutput);
                }

                private void Off()
                {
                    Outputs.Singleton.Off(
                        string.Format(
                            "{0}{1}",
                            _boolExpression.UniqueId,
                            OutputActivator),
                        _idOutput);
                }

                public void Configure(Guid idOutput)
                {
                    if (_isConfigured)
                        return;

                    _idOutput = idOutput;

                    if (!_boolExpression.State.HasValue)
                        return;

                    if (_boolExpression.State.Value)
                        On();

                    _isConfigured = true;
                }

                public void Unconfigure(Guid idOutput)
                {
                    if (!_isConfigured
                        || (_idOutput == idOutput))
                        return;

                    _isConfigured = false;

                    var state = _boolExpression.State;

                    if (state.HasValue && !state.Value)
                        return;

                    Off();
                }

                public void Dispose()
                {
                    _boolExpression.RemoveListenerAndGetState(this);
                }
            }

            protected class SpecialOutputSettings
            {
                public IBoolExpression BoolExpression { get; private set; }
                public Guid IdOutput { get; private set; }

                public SpecialOutputSettings(
                    IBoolExpression boolExpression,
                    Guid idOutput)
                {
                    BoolExpression = boolExpression;
                    IdOutput = idOutput;
                }
            }

            private readonly Dictionary<int, SpecialOutputListener> _listeners =
                new Dictionary<int, SpecialOutputListener>();

            public void Configure(IdAndObjectType idAndObjectType)
            {
                var idObject = (Guid)idAndObjectType.Id;

                var specialOutputsSettings = GetSpecialOutputsSettings(idObject);

                foreach (var specialOutputSettings in specialOutputsSettings)
                {
                    SpecialOutputListener listener;

                    if (specialOutputSettings.IdOutput == Guid.Empty)
                        continue;

                    var boolExpression = specialOutputSettings.BoolExpression;

                    if (!_listeners.TryGetValue(
                        boolExpression.UniqueId,
                        out listener))
                    {
                        listener =
                            new SpecialOutputListener(boolExpression);

                        _listeners.Add(
                            boolExpression.UniqueId,
                            listener);
                    }

                    listener.Configure(specialOutputSettings.IdOutput);
                }
            }

            protected abstract IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(Guid idObject);

            protected abstract IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(
                Guid idObject,
                DB.IDbObject dbObject);

            public void Unconfigure(
                Guid idObject,
                DB.IDbObject newDbObject)
            {
                var specialOutputsSettings = GetSpecialOutputsSettings(
                    idObject,
                    newDbObject);

                foreach (var specialOutputSettings in specialOutputsSettings)
                {
                    SpecialOutputListener listener;

                    if (!_listeners.TryGetValue(
                        specialOutputSettings.BoolExpression.UniqueId,
                        out listener))
                        continue;

                    listener.Unconfigure(specialOutputSettings.IdOutput);

                    if (specialOutputSettings.IdOutput == Guid.Empty)
                    {
                        listener.Dispose();
                        _listeners.Remove(specialOutputSettings.BoolExpression.UniqueId);
                    }
                }
            }
        }

        private class SpecialOutputsConfiguratorForDcu
            : SpecialOutputsConfigurator
        {
            protected override IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(Guid idObject)
            {
                var dcuStateAndSettings = DCUs.Singleton.GetDcuStateAndSettings(idObject);

                if (dcuStateAndSettings == null)
                    return Enumerable.Empty<SpecialOutputSettings>();

                return GetBoolExpressions(
                    dcuStateAndSettings.SabotageDcuInputs,
                    dcuStateAndSettings.SabotageDcuInputsOutputId);
            }

            private IEnumerable<SpecialOutputSettings> GetBoolExpressions(
                IBoolExpression sabotageDcuInputs,
                Guid dcuInputsSabotageOutputId)
            {
                return Enumerable.Repeat(
                    new SpecialOutputSettings(
                        sabotageDcuInputs,
                        dcuInputsSabotageOutputId),
                    1);
            }

            protected override IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(
                Guid idObject,
                DB.IDbObject dbObject)
            {
                var dcuStateAndSettings = DCUs.Singleton.GetDcuStateAndSettings(idObject);

                if (dcuStateAndSettings == null)
                    return Enumerable.Empty<SpecialOutputSettings>();

                var dcu = dbObject as DB.DCU;

                return GetBoolExpressions(
                    dcuStateAndSettings.SabotageDcuInputs,
                    dcu != null
                        ? dcu.GuidDcuInputsSabotageOutput
                        : Guid.Empty);
            }
        }

        private class SpecialOutputsConfiguratorForAlarmArea
            : SpecialOutputsConfigurator
        {
            protected override IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(Guid idObject)
            {
                var alarmAreaStateAndSettings = AlarmAreas.Singleton.GetAlarmAreaSettings(idObject);

                if (alarmAreaStateAndSettings == null)
                    return Enumerable.Empty<SpecialOutputSettings>();

                return GetBoolExpressions(
                    alarmAreaStateAndSettings,
                    alarmAreaStateAndSettings.DbObject);
            }

            private IEnumerable<SpecialOutputSettings> GetBoolExpressions(
                IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
                DB.AlarmArea alarmArea)
            {
                var specialOutputSettingsList = new LinkedList<SpecialOutputSettings>();

                var inAlarm = alarmAreaStateAndSettings.InAlarm;

                if (inAlarm != null)
                    specialOutputSettingsList.AddLast(
                        new SpecialOutputSettings(
                            inAlarm,
                            alarmArea != null
                                ? alarmArea.GuidOutputAlarmState
                                : Guid.Empty));

                var inSabotage = alarmAreaStateAndSettings.InSabotage;

                if (inSabotage != null)
                    specialOutputSettingsList.AddLast(
                        new SpecialOutputSettings(
                            inSabotage,
                            alarmArea != null
                                ? alarmArea.GuidOutputSabotage
                                : Guid.Empty));

                var alarmNotAcknowledged = alarmAreaStateAndSettings.AlarmNotAcknowledged;

                if (alarmNotAcknowledged != null)
                    specialOutputSettingsList.AddLast(
                        new SpecialOutputSettings(
                            alarmNotAcknowledged,
                            alarmArea != null
                                ? alarmArea.GuidOutputNotAcknowledged
                                : Guid.Empty));

                var anySensorInAlarm = alarmAreaStateAndSettings.AnySensorInAlarm;

                if (anySensorInAlarm != null)
                    specialOutputSettingsList.AddLast(
                        new SpecialOutputSettings(
                            anySensorInAlarm,
                            alarmArea != null
                                ? alarmArea.GuidOutputMotion
                                : Guid.Empty));

                var siren = alarmAreaStateAndSettings.SirenOutput;

                if (siren != null)
                    specialOutputSettingsList.AddLast(
                        new SpecialOutputSettings(
                            siren,
                            alarmArea != null
                                ? alarmArea.GuidOutputSiren
                                : Guid.Empty));

                return specialOutputSettingsList;
            }

            protected override IEnumerable<SpecialOutputSettings> GetSpecialOutputsSettings(
                Guid idObject,
                DB.IDbObject dbObject)
            {
                var alarmAreaStateAndSettings = AlarmAreas.Singleton.GetAlarmAreaSettings(idObject);

                if (alarmAreaStateAndSettings == null)
                    return Enumerable.Empty<SpecialOutputSettings>();

                return GetBoolExpressions(
                    alarmAreaStateAndSettings,
                    dbObject as DB.AlarmArea);
            }
        }
    }
}