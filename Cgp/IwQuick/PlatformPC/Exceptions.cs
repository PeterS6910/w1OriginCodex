/**
 * this is just a PlatformPC-dependant complement to CrossPlatform/Exceptions.cs
 * 
 * solution to avoid preprocessor directives
 */

using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Contal.IwQuick
{
    public partial class OutOfRangeException
    {
        private const string TagValue = "value";
        private const string TagIntervalStart = "intervalStart";
        private const string TagIntervalStop = "intervalStop";


        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public OutOfRangeException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
            _value = serialInfo.GetDouble(TagValue);
            _intervalStart = serialInfo.GetDouble(TagIntervalStart);
            _intervalStop = serialInfo.GetDouble(TagIntervalStop);

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo serialInfo, StreamingContext context)
        {
            base.GetObjectData(serialInfo, context);

            serialInfo.AddValue(TagValue, _value);
            serialInfo.AddValue(TagIntervalStart, _intervalStart);
            serialInfo.AddValue(TagIntervalStop, _intervalStop);

        }

    }

    public partial class ResourceExhaustedException 
    {

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public ResourceExhaustedException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }

    public partial class OperationDeniedException 
    {

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public OperationDeniedException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }

    public abstract partial class ANameCarryingException
    {

        private const string TagName = "name";

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected ANameCarryingException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
            _name = serialInfo.GetString(TagName);

        }

        public override void GetObjectData(SerializationInfo serialInfo, StreamingContext context)
        {
            base.GetObjectData(serialInfo, context);

            serialInfo.AddValue(TagName, _name);
        }

    }

    public partial class AlreadyExistsException
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public AlreadyExistsException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {


        }
    }

    public partial class DoesNotExistException
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public DoesNotExistException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {

        }

    }

    public partial class InvalidSecureEntityException
    {

        private readonly int _reason = -1;
        public int Reason
        {
            get { return _reason; }
        }


        private const string TagReason = "reason";

        public InvalidSecureEntityException(int reason, string name, String message)
            : base(message)
        {
            _reason = reason;
            _name = name;

        }

        public InvalidSecureEntityException(int reason, string name)
            : base(_implicitMessage)
        {
            _reason = reason;
            _name = name;

        }


        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public InvalidSecureEntityException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
            _reason = serialInfo.GetInt32(TagReason);
        }

        public override void GetObjectData(SerializationInfo serialInfo, StreamingContext context)
        {
            base.GetObjectData(serialInfo, context);

            serialInfo.AddValue(TagReason, _reason);

        }

    }

    public partial class AccessDeniedException
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public AccessDeniedException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }
    }



    public partial class InvalidDeleteOperationException
    {
       
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]

        public InvalidDeleteOperationException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }


    
    public partial class IncoherentDataException
    {
        
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public IncoherentDataException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }

    public partial class SqlDeleteReferenceConstraintException
    {
        
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SqlDeleteReferenceConstraintException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }

    public partial class SqlUniqueException
    {

        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public SqlUniqueException(SerializationInfo serialInfo, StreamingContext context) :
            base(serialInfo, context)
        {
        }

    }
}