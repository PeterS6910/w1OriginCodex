using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class MultiDoorElements : 
        ASingleton<MultiDoorElements>,
        IDbObjectChangeListener<DB.MultiDoorElement>
    {
        private readonly SyncDictionary<Guid, MultiDoorElementSettings> _multiDoorElementSettings =
            new SyncDictionary<Guid, MultiDoorElementSettings>();

        private MultiDoorElements()
            : base(null)
        {
        }

        public void AddMultiDoorElement(Guid guidMultiDoorElement)
        {
            var multiDoorElement =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.MultiDoorElement,
                    guidMultiDoorElement) as MultiDoorElement;

            if (multiDoorElement == null)
                return;

            _multiDoorElementSettings.GetOrAddValue(
                guidMultiDoorElement,
                key => new MultiDoorElementSettings(multiDoorElement),
                (key, value, newlyAdded) =>
                {
                    if (!newlyAdded)
                        value.ApplyChanges(multiDoorElement);

                    value.Configure();
                });
        }

        public bool OnAccessGranted(
            Guid guidElement,
            Guid guidCardReader,
            AccessDataBase accessData,
            int crAddress)
        {
            bool result = true;

            _multiDoorElementSettings.TryGetValue(
                guidElement,
                (key, found, value) =>
                {
                    if (found)
                        result =
                            value.OnAccessGranted(
                                guidCardReader,
                                accessData,
                                crAddress);
                });

            return result;
        }

        public void SendAllStates(Guid guidElement)
        {
            _multiDoorElementSettings.TryGetValue(
                guidElement,
                (key, found, value) =>
                {
                    if (found)
                        value.SendDsmState();
                });
        }

        public void SetForceUnlocked(Guid guidElement, bool isForceUnlocked)
        {
            _multiDoorElementSettings.TryGetValue(
                guidElement,
                (key, found, value) =>
                {
                    if (found)
                        value.SetForceUnlocked(isForceUnlocked);
                });
        }

        public void OnTimingsChanged(
            Guid guidElement,
            MultiDoor multiDoor)
        {
            _multiDoorElementSettings.TryGetValue(
                guidElement,
                (key, found, value) =>
                {
                    if (found)
                        value.OnTimingsChanged(multiDoor);
                });
        }

        public bool IsUnlocked(Guid guidElement)
        {
            bool result = false;

            _multiDoorElementSettings.TryGetValue(
                guidElement,
                (key, found, value) =>
                {
                    result = value.IsUnlocked;
                });

            return result;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            MultiDoorElement newObject)
        {
            _multiDoorElementSettings.TryGetValue(
                idObject,
                (key, found, value) =>
                {
                    if (found)
                        value.Unconfigure();
                });
        }

        public void OnObjectSaved(
            Guid idObject,
            MultiDoorElement newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            _multiDoorElementSettings.Remove(
                idObject,
                (key, removed, value) =>
                {
                    if (removed)
                        value.OnRemoved();
                });
        }
    }
}
