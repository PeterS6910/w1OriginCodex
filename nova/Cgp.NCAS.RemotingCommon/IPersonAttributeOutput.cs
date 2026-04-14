using Contal.Cgp.NCAS.Server.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IPersonAttributeOutput
    {
        void CreateOrUpdate(ref PersonAttributeOutput output);
        PersonAttributeOutput GetPersonAttributeOutput();
        void UpdatePersonAttributeLastReportDate(DateTime lastReportDate);
    }
}
