using System;
using System.Linq;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    public class CcuDataBatch
    {
        public enum ItemTypeEnum
        {
            NormalEnqueue,
            InitCCUMarker,
            InitCCUEnqueue
        }

        private readonly Dictionary<ObjectType, ICollection<Guid>> _objectsToDelete =
            new Dictionary<ObjectType, ICollection<Guid>>();

        private readonly List<ModifiedObjectsCollection> _modifiedObjectsForCCU =
            new List<ModifiedObjectsCollection>();

        public CcuDataBatch(bool invokedFromInitCCU)
        {
            ItemType = 
                invokedFromInitCCU 
                    ? ItemTypeEnum.InitCCUEnqueue 
                    : ItemTypeEnum.NormalEnqueue;
        }

        private CcuDataBatch()
        {
        }

        public Dictionary<ObjectType, ICollection<Guid>> ObjectsToDelete
        {
            get { return _objectsToDelete; }
        }

        public IEnumerable<ModifiedObjectsCollection> ModifiedObjectsForCCU
        {
            get { return _modifiedObjectsForCCU; }
        }

        public bool RunApplyChanges
        {
            get
            {
                return _modifiedObjectsForCCU.Any(modifyObjectsCollection => !modifyObjectsCollection.IsEmpty)
                       || _objectsToDelete.Values.Any(ids => ids.Count > 0);
            }
        }

        public ItemTypeEnum ItemType { get; private set; }

        public void SetObjectToDelete(
            ObjectType objectType,
            Guid idObjectToDelete)
        {
            ICollection<Guid> idObjectsToDelete;

            if (!_objectsToDelete.TryGetValue(
                objectType,
                out idObjectsToDelete))
            {
                idObjectsToDelete = new LinkedList<Guid>();

                _objectsToDelete.Add(
                    objectType, 
                    idObjectsToDelete);
            }

            idObjectsToDelete.Add(idObjectToDelete);
        }

        public void AddModifiedObjectsCollection(ModifiedObjectsCollection modifiedObjectCollection)
        {
            _modifiedObjectsForCCU.Add(modifiedObjectCollection);
        }

        public static CcuDataBatch CreateInitMarker()
        {
            return new CcuDataBatch
            {
                ItemType = ItemTypeEnum.InitCCUMarker
            };
        }
    }
}