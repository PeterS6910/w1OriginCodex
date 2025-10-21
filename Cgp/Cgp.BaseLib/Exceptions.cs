using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Contal.Cgp.BaseLib
{
    [Serializable]
    public class SqlUniqueException : Exception 
    {
        public SqlUniqueException()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SqlUniqueException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }
    }

    [Serializable]
    public class SqlDeleteReferenceConstraintException : Exception
    {
        public SqlDeleteReferenceConstraintException()
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SqlDeleteReferenceConstraintException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }
    }    
}
