using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public class PersonDepartmentChange
    {
        public UserFoldersStructure OldPersonDepartment { get; set; }
        public UserFoldersStructure NewPersonDepartment { get; set; }

        public PersonDepartmentChange(UserFoldersStructure oldPersonDepartment, UserFoldersStructure newPersonDepartment)
        {
            OldPersonDepartment = oldPersonDepartment;
            NewPersonDepartment = newPersonDepartment;
        }
    }
}
