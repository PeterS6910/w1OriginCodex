using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(215)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class OldEventsSettings
    {
        private bool _saveOldInputStateEvents = true;
        private bool _saveOldOutputStateEvents = true;
        private bool _saveOldAlarmAreaActualStateEvents = true;
        private bool _saveOldAlarmAreaActivationStateEvents = true;
        private bool _saveOldCardReaderOnlineStateEvents = true;

        [LwSerialize]
        public bool SaveOldInputStateEvents { get { return _saveOldInputStateEvents; } set { _saveOldInputStateEvents = value; } }
        [LwSerialize]
        public bool SaveOldOutputStateEvents { get { return _saveOldOutputStateEvents; } set { _saveOldOutputStateEvents = value; } }
        [LwSerialize]
        public bool SaveOldAlarmAreaActualStateEvents { get { return _saveOldAlarmAreaActualStateEvents; } set { _saveOldAlarmAreaActualStateEvents = value; } }
        [LwSerialize]
        public bool SaveOldAlarmAreaActivationStateEvents { get { return _saveOldAlarmAreaActivationStateEvents; } set { _saveOldAlarmAreaActivationStateEvents = value; } }
        [LwSerialize]
        public bool SaveOldCardReaderOnlineStateEvents { get { return _saveOldCardReaderOnlineStateEvents; } set { _saveOldCardReaderOnlineStateEvents = value; } }

        public OldEventsSettings()
        {
        }

        public void Set(
            bool saveOldInputStateEvents,
            bool saveOldOutputStateEvents,
            bool saveOldAlarmAreaActualStateEvents,
            bool saveOldAlarmAreaActivationStateEvents,
            bool saveOldCardReaderOnlineStateEvents
            )
        {
            _saveOldInputStateEvents = saveOldInputStateEvents;
            _saveOldOutputStateEvents = saveOldOutputStateEvents;
            _saveOldAlarmAreaActualStateEvents = saveOldAlarmAreaActualStateEvents;
            _saveOldAlarmAreaActivationStateEvents = saveOldAlarmAreaActivationStateEvents;
            _saveOldCardReaderOnlineStateEvents = saveOldCardReaderOnlineStateEvents;
        }
    }
}
