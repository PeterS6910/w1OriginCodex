using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using NHibernate.Util;
using System;
using System.Linq;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ExcelReportOutputs : ANcasBaseOrmTable<ExcelReportOutputs, ExcelReportOutput>, IExcelReportOutput
    {
        private readonly string TABLENAME = "ExcelReportOutput";
        private ExcelReportOutputs() : base(null)
        {
        }

        public override ObjectType ObjectType => ObjectType.ExcelReportOutput;

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

        public ExcelReportOutput GetSettings()
        {
            var items = Singleton.List();
            if (items != null && items.Count > 0)
            {
                return items.First();
            }
            return null;
        }

        public void CreateOrUpdate(ref ExcelReportOutput output)
        {
            try
            {
                var _output = GetSettings();
                if (_output != null)
                {
                    _output = Singleton.GetObjectForEdit(_output.Id);
                    _output.Output = output.Output;
                    _output.IsEnabled = output.IsEnabled;
                    _output.Interval = output.Interval;
                    _output.TimeZone = output.TimeZone;
                    _output.Filename = output.Filename;
                    Singleton.Update(_output);
                    Singleton.EditEnd(_output);
                }
                else
                {
                    Singleton.Insert(ref output);
                }


                if (OnChanged != null) OnChanged();
            }

            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }
        public event DVoid2Void OnChanged;
    }
}
