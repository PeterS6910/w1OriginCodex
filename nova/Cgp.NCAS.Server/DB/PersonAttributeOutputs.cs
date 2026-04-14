using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using NHibernate.Util;
using System;
using System.Linq;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class PersonAttributeOutputs : ANcasBaseOrmTable<PersonAttributeOutputs, PersonAttributeOutput>, IPersonAttributeOutput
    {
        private readonly string TABLENAME = "PersonAttributeOutput";
        private PersonAttributeOutputs() : base(null)
        {
        }

        public override ObjectType ObjectType => ObjectType.PersonAttributeOutput;

        public override bool HasAccessDelete(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessView(Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public bool IsConsecutiveEvent(Eventlog eventlog )
        {
            var _output = GetPersonAttributeOutput();
            if (_output != null && _output.IsEnabled && _output.FailsCount > 1)
            {
                return ConsecutiveEvents.Singleton.TriggerEventlog(eventlog, _output.FailsCount);
            }

            return false;
        }
        public PersonAttributeOutput GetPersonAttributeOutput()
        {
            var items = Singleton.List();
            if (items != null && items.Count > 0)
            {
                return items.First();
            }
            return null;
        }

        void IPersonAttributeOutput.CreateOrUpdate(ref PersonAttributeOutput output)
        {
            try
            {
                var perattroutput = GetPersonAttributeOutput();
                if (perattroutput != null)
                {
                    perattroutput = Singleton.GetObjectForEdit(perattroutput.Id);
                    perattroutput.Output = output.Output;
                    perattroutput.IsEnabled = output.IsEnabled;
                    perattroutput.Interval = output.Interval;
                    perattroutput.FailsCount = output.FailsCount;
                    perattroutput.IdTimeZone = output.IdTimeZone;
                    Singleton.Update(perattroutput);
                    Singleton.EditEnd(perattroutput);
                }
                else
                {
                    output.LastReportDate = null;
                    Singleton.Insert(ref output);
                }


                if (OnChanged != null) OnChanged();
            }

            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void UpdatePersonAttributeLastReportDate(DateTime lastReportDate)
        {
            var perattroutput = GetPersonAttributeOutput();
            if (perattroutput != null)
            {
                perattroutput.LastReportDate = lastReportDate.Date;
                Singleton.Update(perattroutput);
                Singleton.EditEnd(perattroutput);
            }

            if(OnChanged!= null) OnChanged();
        }

        public event DVoid2Void OnChanged;
    }
}
