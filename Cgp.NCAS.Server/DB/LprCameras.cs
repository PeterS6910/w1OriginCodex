using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class LprCameras :
        ANcasBaseOrmTableWithAlarmInstruction<LprCameras, LprCamera>,
        ILprCameras
    {
        private LprCameras()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<LprCamera>())
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var camera = ormObject as LprCamera;

            return camera != null ? camera.CCU : null;
        }

        protected override IModifyObject CreateModifyObject(LprCamera ormbObject)
        {
            return new LprCameraModifyObj(ormbObject);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(LprCamera camera)
        {
            if (camera != null && camera.CCU != null)
                yield return camera.CCU;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.LprCameras),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.LprCameras),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.LprCameras),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.LprCameras),
                login);
        }

        public override void CUDSpecial(LprCamera camera, ObjectDatabaseAction objectDatabaseAction)
        {
            if (camera == null)
                return;

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        camera.GetId(),
                        camera.GetObjectType()));
            }
            else
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(camera);
            }
        }

        protected override IEnumerable<LprCamera> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<LprCamera>(
                camera =>
                    camera.LocalAlarmInstruction != null &&
                    camera.LocalAlarmInstruction != string.Empty);
        }

        protected override void LoadObjectsInRelationship(LprCamera obj)
        {
            if (obj != null && obj.CCU != null)
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
        }

        protected override void LoadObjectsInRelationshipGetById(LprCamera obj)
        {
            if (obj != null && obj.CCU != null)
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
        }

        public ICollection<LprCameraShort> ShortSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception error)
        {
            var cameras = SelectByCriteria(filterSettings, out error);
            ICollection<LprCameraShort> result = new List<LprCameraShort>();

            if (cameras != null)
            {
                foreach (var camera in cameras)
                {
                    PrepareCamera(camera);

                    var shortCamera = LprCameraShort.FromLprCamera(camera);
                    if (shortCamera != null)
                        result.Add(shortCamera);
                }
            }

            return result.OrderBy(camera => camera.FullName).ToList();
        }

        public ICollection<LprCameraShort> ShortSelectByCriteria(
            out Exception error,
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings)
        {
            var cameras = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            ICollection<LprCameraShort> result = new List<LprCameraShort>();

            if (cameras != null)
            {
                foreach (var camera in cameras)
                {
                    PrepareCamera(camera);

                    var shortCamera = LprCameraShort.FromLprCamera(camera);
                    if (shortCamera != null)
                        result.Add(shortCamera);
                }
            }

            return result.OrderBy(camera => camera.FullName).ToList();
        }

        private void PrepareCamera(LprCamera camera)
        {
            if (camera == null)
                return;

            LoadObjectsInRelationship(camera);
            LoadObjectsInRelationshipGetById(camera);
            camera.PrepareToSend();
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.LprCamera; }
        }


        public void LprCamerasLookup(Guid clientId)
        {
            LprCameraDiscoveryHandler.Singleton.Lookup(clientId);
        }

        public void CreateLookupedLprCameras(
            ICollection<LookupedLprCamera> lookupedCameras,
            int? idStructuredSubSite)
        {
            if (lookupedCameras == null || lookupedCameras.Count == 0)
                return;

            foreach (var lookupedCamera in lookupedCameras)
            {
                if (lookupedCamera == null
                    || string.IsNullOrWhiteSpace(lookupedCamera.IpAddress))
                {
                    continue;
                }

                var ipAddress = lookupedCamera.IpAddress.Trim();

                var existingCamera = SelectLinq<LprCamera>(
                    camera => string.Equals(camera.IpAddress, ipAddress, StringComparison.OrdinalIgnoreCase))
                    ?.FirstOrDefault();

                if (existingCamera != null)
                    continue;

                var newCamera = new LprCamera
                {
                    Name = !string.IsNullOrWhiteSpace(lookupedCamera.Name)
                        ? lookupedCamera.Name
                        : ipAddress,
                    IpAddress = ipAddress,
                    Port = lookupedCamera.Port,
                    PortSsl = lookupedCamera.PortSsl,
                    MacAddress = lookupedCamera.MacAddress,
                    Description = BuildDescription(lookupedCamera)
                };

                Insert(ref newCamera, idStructuredSubSite);
            }
        }

        private static string BuildDescription(LookupedLprCamera lookupedCamera)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Model))
                parts.Add(lookupedCamera.Model);

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Type))
                parts.Add(lookupedCamera.Type);

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Equipment))
                parts.Add(lookupedCamera.Equipment);

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Version))
                parts.Add(lookupedCamera.Version);

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Build))
                parts.Add("Build " + lookupedCamera.Build);

            if (!string.IsNullOrWhiteSpace(lookupedCamera.Serial))
                parts.Add("SN " + lookupedCamera.Serial);

            return parts.Count > 0
                ? string.Join(" | ", parts)
                : lookupedCamera.InterfaceSource;
        }
    }
}
