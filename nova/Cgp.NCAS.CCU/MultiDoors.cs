using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Drivers.CardReader;

namespace Contal.Cgp.NCAS.CCU
{
    internal sealed class MultiDoors 
        : AStateAndSettingsObjectCollection<
            MultiDoors, 
            MultiDoorSettings,
            DB.MultiDoor>
    {
        private MultiDoors() : base(null)
        {
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.MultiDoor; }
        }

        protected override void PrepareConfigure(DB.MultiDoor multiDoor)
        {
            var idCardReader = multiDoor.CardReaderId;

            if (idCardReader == Guid.Empty)
                return;

            CardReaders.Singleton.PrepareMultiDoorAdapter(
                idCardReader,
                multiDoor.IdMultiDoor);
        }

        protected override MultiDoorSettings CreateNewStateAndSettingsObject(DB.MultiDoor multiDoor)
        {
            return new MultiDoorSettings(multiDoor.IdMultiDoor);
        }

        public void OnAccessGranted(
            Guid guidMultiDoor,
            ICollection<Guid> elementGuids,
            Guid guidCardReader,
            AccessDataBase accessData,
            CardReader cardReader)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return;
            }

            value.OnAccessGranted(
                elementGuids,
                guidCardReader,
                accessData,
                cardReader);
        }

        public void OnMultiDoorElementAdded(
            Guid guidMultiDoor,
            Guid guidMultiDoorElement)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return;
            }

            value.OnMultiDoorElementAdded(guidMultiDoorElement);
        }

        public void OnMultiDoorElementRemoved(
            Guid guidMultiDoor,
            Guid guidMultiDoorElement)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return;
            }

            value.OnMultiDoorElementRemoved(guidMultiDoorElement);
        }

        public void SendAllStates()
        {
            foreach (var multiDoorSettings in _objects.Values)
            {
                multiDoorSettings.SendAllStates();
            }
        }

        public bool IsForceUnlocked(Guid guidMultiDoor)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return false;
            }

            return value.IsForceUnlocked;
        }

        public void SetForceUnlocked(Guid guidMultiDoor, bool isForceUnlocked)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return;
            }

            value.SetForceUnlocked(isForceUnlocked);
        }

        public bool IsUnlocked(Guid guidMultiDoor)
        {
            MultiDoorSettings value;

            if (!_objects.TryGetValue(
                guidMultiDoor,
                out value))
            {
                return true;
            }

            return value.IsUnlocked;
        }

        public bool IsCrStartScheduled(Guid idMultiDoor)
        {
            MultiDoorSettings multiDoorSettings;

            return
                !_objects.TryGetValue(
                    idMultiDoor,
                    out multiDoorSettings)
                || multiDoorSettings.IsCrStartScheduled;
        }

        public bool IsCardReaderSuppressed(Guid idMultiDoor)
        {
            MultiDoorSettings multiDoorSettings;

            return
                _objects.TryGetValue(
                    idMultiDoor,
                    out multiDoorSettings)
                && multiDoorSettings.IsCardReaderSuppressed;
        }

        public void LooseCardReaderIfSuppressed(
            Guid idMultiDoor,
            CardReader cardReader)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return;
            }

            multiDoorSettings.LooseCardReaderIfSuppressed(cardReader);
        }

        public void LooseCardReader(
            Guid idMultiDoor,
            CardReader cardReader)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return;
            }

            multiDoorSettings.LooseCardReader(cardReader);
        }

        public void SuppressCardReader(Guid idMultiDoor)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return;
            }

            multiDoorSettings.SuppressCardReader();
        }

        public void SetImplicitCrCode(
            Guid idMultiDoor,
            CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed,
            CardReader cardReader)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return;
            }

            multiDoorSettings.SetImplicitCrCode(
                implicitCrMessage,
                followingMessages,
                intrusionOnlyViaLed,
                cardReader);
        }

        public void OnCrOnlineStateChanged(
            Guid idMultiDoor,
            bool isOnline)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return;
            }

            multiDoorSettings.OnCrOnlineStateChanged(isOnline);
        }

        public ICollection<Guid> HasAccessMultiDoor(
            AccessDataBase accessData,
            Guid idMultiDoor)
        {
            MultiDoorSettings multiDoorSettings;

            if (!_objects.TryGetValue(
                idMultiDoor,
                out multiDoorSettings))
            {
                return null;
            }

            return multiDoorSettings.HasAccessMultiDoor(accessData);
        }
    }
}
