using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class CentralNameRegisters : 
        ABaseOrmTable<CentralNameRegisters, CentralNameRegister>, 
        ICentralNameRegisters
    {
        public ICollection<Guid> CnrEventSourcesGuids { get; private set; }

        private CentralNameRegisters() : base(null)
        {
            CnrEventSourcesGuids = new LinkedList<Guid>();
        }

        public void EnsureFixedEventSources()
        {
            try
            {
                CentralNameRegister centralNameRegister;
                var result =
                    SelectLinq<CentralNameRegister>(
                        cnr =>
                            cnr.Name == CentralNameRegister.IMPLICIT_CN_CLIENT_NAME);

                if (result == null || result.Count == 0)
                {
                    centralNameRegister = new CentralNameRegister
                    {
                        Name = CentralNameRegister.IMPLICIT_CN_CLIENT_NAME,
                        ObjectType = (byte)ObjectType.CentralNameRegister
                    };

                    if (Insert(ref centralNameRegister))
                        CnrEventSourcesGuids.Add(centralNameRegister.Id);
                }
                else
                {
                    foreach (var cnr in result)
                        CnrEventSourcesGuids.Add(cnr.Id);
                }

                result =
                    SelectLinq<CentralNameRegister>(
                        cnr =>
                            cnr.Name == CentralNameRegister.IMPLICIT_CN_SERVER_NAME);

                if (result == null || result.Count == 0)
                {
                    centralNameRegister = new CentralNameRegister
                    {
                        Name = CentralNameRegister.IMPLICIT_CN_SERVER_NAME,
                        ObjectType = (byte)ObjectType.CentralNameRegister
                    };
                    
                    if (Insert(ref centralNameRegister))
                        CnrEventSourcesGuids.Add(centralNameRegister.Id);
                }
                else
                {
                    foreach (var cnr in result)
                        CnrEventSourcesGuids.Add(cnr.Id);
                }

                result =
                    SelectLinq<CentralNameRegister>(
                        cnr =>
                            cnr.Name == CentralNameRegister.IMPLICIT_CN_DATABASE_NAME);

                if (result == null || result.Count == 0)
                {
                    centralNameRegister = new CentralNameRegister
                    {
                        Name = CentralNameRegister.IMPLICIT_CN_DATABASE_NAME,
                        ObjectType = (byte)ObjectType.CentralNameRegister
                    };

                    if (Insert(ref centralNameRegister))
                        CnrEventSourcesGuids.Add(centralNameRegister.Id);
                }
                else
                {
                    foreach (var cnr in result)
                        CnrEventSourcesGuids.Add(cnr.Id);
                }
            }
            catch
            { }
        }

        public override bool HasAccessView(Login login)
        {
            return true;            
        }

        public override bool HasAccessInsert(Login login)
        {
            return true;
        }

        public override bool HasAccessUpdate(Login login)
        {
            return true;
        }

        public override bool HasAccessDelete(Login login)
        {
            return true;
        }

        #region ICentralNameRegisters Members

        public ObjectType GetObjectTypeFromGuid(Guid guid)
        {
            ICollection<CentralNameRegister> ret = SelectLinq<CentralNameRegister>(cnr => cnr.Id == guid);
            if (ret == null || ret.Count != 1)
                return ObjectType.NotSupport;
            return (ObjectType)ret.ElementAt(0).ObjectType;            
        }

        public string GetNameFromId(Guid id)
        {
            ICollection<CentralNameRegister> ret = SelectLinq<CentralNameRegister>(cnr => cnr.Id == id);

            return ret == null || ret.Count != 1
                ? string.Empty
                : ret.ElementAt(0).Name;
        }

        public Guid GetGuidFromName(string name)
        {
            try
            {
                ICollection<CentralNameRegister> ret = SelectLinq<CentralNameRegister>(cnr => cnr.Name == name);
                if (ret == null || ret.Count != 1)
                    return Guid.Empty;
                return ret.ElementAt(0).Id;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        #endregion

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CentralNameRegister; }
        }  
    }
}
