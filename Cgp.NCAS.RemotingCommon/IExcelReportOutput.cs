using Contal.Cgp.NCAS.Server.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IExcelReportOutput
    {
        void CreateOrUpdate(ref ExcelReportOutput output);
        ExcelReportOutput GetSettings();
    }
}
