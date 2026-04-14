using System;
using JetBrains.Annotations;


namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class ResourceExhaustedException: Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResourceExhaustedException(string message,Exception innerException)
            :base(message,innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ResourceExhaustedException(string message)
            : base(message, null)
        {
        }

        private const string IMPLICIT_MESSAGE = "Relevant resource has been exhausted";
        /// <summary>
        /// 
        /// </summary>
        public ResourceExhaustedException()
            :base(IMPLICIT_MESSAGE,null)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class OperationDeniedException : Exception
    {
// ReSharper disable once EmptyConstructor
        /// <summary>
        /// 
        /// </summary>
        public OperationDeniedException()
        {
        }


    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PossibleDeadlockException : Exception
    {
// ReSharper disable once EmptyConstructor
        /// <summary>
        /// 
        /// </summary>
        public PossibleDeadlockException()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class OutOfRangeException : Exception
    {
        private readonly double _value;
        /// <summary>
        /// value that is out of interval
        /// </summary>
        public double Value
        {
            get { return _value; }
        }

        private readonly double _intervalStart;
        /// <summary>
        /// expected interval lower bound
        /// </summary>
        public double IntervalStart
        {
            get { return _intervalStart; }
        }

        private readonly double _intervalStop;
        /// <summary>
        /// expected interval upper bound
        /// </summary>
        public double IntervalStop
        {
            get { return _intervalStop; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        public OutOfRangeException(double value, double intervalStart, double intervalStop)
            : base(string.Format("Value {0:0.000} is out of interval <{1:0.000};{2:0.000}>", value, intervalStart, intervalStop))
        {
            _value = value;
            _intervalStart = intervalStart;
            _intervalStop = intervalStop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        public OutOfRangeException(decimal value, decimal intervalStart, decimal intervalStop)
            : base(string.Format("Value {0} is out of interval <{1};{2}>", value, intervalStart, intervalStop))
        {
            _value = (double)value;
            _intervalStart = (double)intervalStart;
            _intervalStop = (double)intervalStop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        public OutOfRangeException(int value, int intervalStart, int intervalStop)
            : base(string.Format("Value {0} is out of interval <{1},{2}>", value, intervalStart, intervalStop))
        {
            _value = value;
            _intervalStart = intervalStart;
            _intervalStop = intervalStop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        public OutOfRangeException(uint value, uint intervalStart, uint intervalStop)
            : base(string.Format("Value {0} is out of interval <{1},{2}>", value, intervalStart, intervalStop))
        {
            _value = value;
            _intervalStart = intervalStart;
            _intervalStop = intervalStop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intervalStartOrStop"></param>
        /// <param name="infinityDownwards"></param>
        public OutOfRangeException(int value, int intervalStartOrStop, bool infinityDownwards)
            : base(
                (infinityDownwards ?
                    string.Format("Value {0} is out of interval <-inf,{1}>", value, intervalStartOrStop)
                :
                    string.Format("Value {0} is out of interval <{1},+inf>", value, intervalStartOrStop)
                ))
        {
            _value = value;
            if (infinityDownwards)
            {
                _intervalStart = double.MinValue;
                _intervalStop = intervalStartOrStop;

            }
            else
            {
                _intervalStart = intervalStartOrStop;
                _intervalStop = double.MaxValue;
            }
        }


    }

    /// <summary>
    /// 
    /// </summary>
// ReSharper disable once PartialTypeWithSinglePart
    public abstract partial class ANameCarryingException : Exception
    {
        protected string _name = null;
        /// <summary>
        /// name of the entity
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected ANameCarryingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        protected ANameCarryingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }



    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class AlreadyExistsException : ANameCarryingException
    {
        /// <summary>
        /// 
        /// </summary>
        public AlreadyExistsException()
            : base("Entity already exists !")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public AlreadyExistsException(string name)
            : base("Entity \"" + (name ?? String.Empty) + "\" already exists !")
        {
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        public AlreadyExistsException(object @object)
            : base("Entity \"" + (null == @object ? String.Empty : @object.ToString()) + "\" already exists !")
        {
            if (null != @object)
                _name = @object.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exceptionMessage"></param>
        public AlreadyExistsException(string name, String exceptionMessage)
            : base(exceptionMessage)
        {
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duplicateObject"></param>
        /// <param name="exceptionMessage"></param>
        public AlreadyExistsException(object duplicateObject, String exceptionMessage)
            : base(exceptionMessage)
        {
            if (null != duplicateObject)
                _name = duplicateObject.ToString();
        }



    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class DoesNotExistException : ANameCarryingException
    {
        /// <summary>
        /// 
        /// </summary>
        public DoesNotExistException()
            : base("Entity does not exist !")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DoesNotExistException(string name)
            : base("Entity \"" + (name ?? String.Empty) + "\" does not exist !")
        {
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nonexistingObject"></param>
        public DoesNotExistException(object nonexistingObject)
            : base("Entity \"" + (null == nonexistingObject ? String.Empty : nonexistingObject.ToString()) + "\" does not exist !")
        {
            if (null != nonexistingObject)
                _name = nonexistingObject.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exceptionMessage"></param>
        public DoesNotExistException(string name, String exceptionMessage)
            : base(exceptionMessage)
        {
            _name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nonexistingObject"></param>
        /// <param name="exceptionMessage"></param>
        public DoesNotExistException(object nonexistingObject, String exceptionMessage)
            : base(exceptionMessage)
        {
            if (null != nonexistingObject)
                _name = nonexistingObject.ToString();
        }



    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class InvalidSecureEntityException : ANameCarryingException
    {
        private const string _implicitMessage = "Specified secure entity is invalid !";


        /// <summary>
        /// 
        /// </summary>
        public InvalidSecureEntityException()
            : base(_implicitMessage)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidSecureEntityException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerException"></param>
        public InvalidSecureEntityException(Exception innerException)
            : base(_implicitMessage, innerException)
        {
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidSecureEntityException(String message, Exception innerException)
            : base(message, innerException)
        {
        }



    }

    /// <summary>
    /// 
    /// </summary>
    public class NativeActionException : Exception
    {
        private readonly int _errorCode;
        /// <summary>
        /// 
        /// </summary>
        public int ErrorCode
        {
            get { return _errorCode; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        public NativeActionException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            _errorCode = errorCode;
        }

        private const string IMPLICIT_ERROR_MESSAGE = "Native action failed with error {0} (0x{0:X})";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        public NativeActionException(int errorCode, Exception innerException)
            : base(string.Format(IMPLICIT_ERROR_MESSAGE, errorCode), innerException)
        {
            _errorCode = errorCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        public NativeActionException(int errorCode)
            : base(string.Format(IMPLICIT_ERROR_MESSAGE, errorCode))
        {
            _errorCode = errorCode;
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class InvalidChecksumException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidChecksumException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidChecksumException(string message)
            : base(message, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidChecksumException()
            : base("Checksum invalid", null)
        {
        }
    }

    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class AccessDeniedException : Exception
    {
        [PublicAPI]
        public AccessDeniedException()
        {
        }
    }



    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class InvalidDeleteOperationException : Exception
    {
        [PublicAPI]
        public InvalidDeleteOperationException()
        {
        }

    }


    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class IncoherentDataException : Exception
    {
        [PublicAPI]
        public IncoherentDataException()
        {
        }


    }

    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class SqlDeleteReferenceConstraintException : Exception
    {
        [PublicAPI]
        public SqlDeleteReferenceConstraintException()
        {
        }



    }

    [Serializable]
// ReSharper disable once PartialTypeWithSinglePart
    public partial class SqlUniqueException : Exception
    {
        public SqlUniqueException()
        {
        }

        public SqlUniqueException(string message)
            : base(message)
        {
        }

    }
}
