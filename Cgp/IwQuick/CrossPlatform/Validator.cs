using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    /// <summary>
    /// provides simple validation routines encapsulation
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// returns true, if the object input equals null
        /// </summary>
        /// <param name="inputObject">object variable to test</param>
        public static bool IsNull(Object inputObject)
        {
            if (ReferenceEquals(inputObject,null))
                return true;

            try
            {
                inputObject.ToString();
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE = (IntPtr) (-1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>
        public static bool IsNull(IntPtr intPtr)
        {
            return IntPtr.Zero == intPtr ||
                   INVALID_HANDLE == intPtr;
        }

        /*
        /// <summary>
        /// throws ArgumentNullException, if the object input equals null
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="object">object variable to test</param>
        /// <param name="i_strParamName">name of the object variable</param>
        public static void CheckNull(Object object, string i_strParamName)
        {
            if (null == object)
            {
                ArgumentNullException aError;

                if (null == i_strParamName)
                    aError = new ArgumentNullException();
                else
                    aError = new ArgumentNullException(i_strParamName);
#if DEBUG
                Debug.Assert(false);
#endif
                throw aError;
            }
        }*/

        /// <summary>
        /// throws ArgumentNullException, if the at least one of the object inputs is equal to null
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        [Obsolete("Use Validator.CheckForNull instead")]
        public static void CheckNull(params Object[] objects)
        {
            if (ReferenceEquals(objects,null))
            {
                throw new ArgumentNullException();
            }

            if (objects.Any(o => ReferenceEquals(o,null)))
            {
                ArgumentNullException aError = new ArgumentNullException();

                throw aError;
            }
        }

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="objectToValidate">string variable to test</param>
        /// <param name="parameterName">name of the string variable</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void CheckForNull<T>( // naming intentionally changed, to avoid ambiguity with CheckNull and multiple object params
            T objectToValidate,
            [InvokerParameterName]
            string parameterName)
            where T: class
        {
            if (ReferenceEquals(null,objectToValidate))
            {
                ArgumentException aError = null == parameterName ?
                    new ArgumentNullException("objectToValidate", "Object is null") :
                    new ArgumentNullException(parameterName, "Object "+parameterName+" is null");

                throw aError;
            }
        }

#if csharp6
        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="objectToValidate">string variable to test</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void CheckForNull<T>( // naming intentionally changed, to avoid ambiguity with CheckNull and multiple object params
            T objectToValidate)
            where T : class
        {
            if (ReferenceEquals(null, objectToValidate))
            {
                string parameterName = nameof(objectToValidate);

                ArgumentException e = 
                    new ArgumentNullException(
                        parameterName, 
                        @"Object " + parameterName + @" is null");

                throw e;
            }
        }
#endif

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="objectToValidate">string variable to test</param>
        /// <param name="parameterName">name of the string variable</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        internal static void CheckObjectNull( // naming intentionally changed, to avoid ambiguity with CheckNull and multiple object params
            object objectToValidate,
            [InvokerParameterName]
            string parameterName)
        {
            if (ReferenceEquals(null, objectToValidate))
            {
                ArgumentException aError = null == parameterName ?
                    new ArgumentNullException("objectToValidate", "Object is null") :
                    new ArgumentNullException(parameterName, "Object " + parameterName + " is null");

                throw aError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrays"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        [Obsolete("Use Validator.CheckForNull or Validator.CheckNullAndEmpty instead")]
        public static void CheckNull(params Array[] arrays)
        {
            if (ReferenceEquals(arrays,null))
                return;

            foreach (Array array in arrays)
            {
                if (ReferenceEquals(array, null) ||
                    array.Length == 0)
                {
                    var aError = new ArgumentNullException();

                    throw aError;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="parameterName"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNullAndEmpty<T>(
            ICollection<T> collection, 
            [InvokerParameterName]
            string parameterName)
        {
            if (ReferenceEquals(collection, null) ||
                collection.Count == 0)
            {
                var error = new ArgumentNullException(parameterName);

                throw error;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNull(params IntPtr[] handles)
        {
            if (ReferenceEquals(handles,null))
                return;

            foreach (IntPtr p in handles)
            {
                if (IsNull(p))
                {
                    ArgumentNullException aError = new ArgumentNullException();

                    throw aError;
                }
            }
        }

        /// <summary>
        /// returns true, if the string input is not null and not empty
        /// </summary>
        /// <param name="stringToValidate">string variable to test</param>
        public static bool IsNotNullString(string stringToValidate)
        {
            return !string.IsNullOrEmpty(stringToValidate);
        }

        /// <summary>
        /// returns true, if the string input is null or empty
        /// </summary>
        /// <param name="stringToValidate">string variable to test</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static bool IsNullString(string stringToValidate)
        {
            return
                string.IsNullOrEmpty(stringToValidate);
        }

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="stringToValidate">string variable to test</param>
        /// <param name="parameterName">name of the string variable</param>
        [DebuggerStepThrough]
        [DebuggerHidden]
        public static void CheckNullString(
            String stringToValidate, 
            [InvokerParameterName]
            string parameterName)
        {
            if (string.IsNullOrEmpty(stringToValidate))
            {
                ArgumentException aError = null == parameterName ?
                    new ArgumentNullException("stringToValidate", "String is null or empty") : 
                    new ArgumentNullException(parameterName, "String is null or empty");

                throw aError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integerToValidate"></param>
        public static void CheckZero(long integerToValidate)
        {
            if (0 == integerToValidate)
                throw new ArgumentNullException("integerToValidate");

        }

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="stringToValidate">string variable to test</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNullString(String stringToValidate)
        {
            CheckNullString(stringToValidate, null);
        }


        /// <summary>
        /// throws ArgumentOutOfRangeException, if the integer input is less than zero
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="integerToValidate">integer variable to test</param>
        /// <param name="parameterName">name of the integer variable</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNegativeInt(
            int integerToValidate, 
            [InvokerParameterName]
            string parameterName)
        {
            if (integerToValidate < 0)
            {
                ArgumentOutOfRangeException aError = null == parameterName ?
                    new ArgumentOutOfRangeException("integerToValidate") : 
                    new ArgumentOutOfRangeException(parameterName, string.Format("Integer value \"{0}\" is negative", parameterName) );

                throw aError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integerToValidate"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNegativeOrZeroInt(
            long integerToValidate,
            [InvokerParameterName]
            string parameterName)
        {
            if (integerToValidate <= 0)
            {
                ArgumentOutOfRangeException aError = null == parameterName ?
                    new ArgumentOutOfRangeException("integerToValidate") : 
                    new ArgumentOutOfRangeException(parameterName, string.Format("Integer value \"{0}\" is negative", parameterName));

                throw aError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integerToValidate"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNegativeOrZeroInt(long integerToValidate)
        {
            CheckNegativeOrZeroInt(integerToValidate, null);
        }



        /// <summary>
        /// throws ArgumentOutOfRangeException, if the integer input is less than zero
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="integerToValidate">integer variable to test</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckNegativeInt(int integerToValidate)
        {
            CheckNegativeInt(integerToValidate, null);
        }

        /// <summary>
        /// throws InvalidOperationException, if the condition input is true
        /// </summary>
        /// <param name="isConditionTrue">input condition</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckInvalidOperation(bool isConditionTrue)
        {
            if (isConditionTrue)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">path of the file to check</param>
        /// <exception cref="ArgumentNullException">if the file path is null or empty</exception>
        /// <exception cref="DoesNotExistException">if the file on specified path does not exist</exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckFileExists(string filePath)
        {
            CheckNullString(filePath);

            if (!File.Exists(filePath))
                throw new DoesNotExistException(filePath, "File specified by path \"" + filePath + "\" does not exist");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <exception cref="DoesNotExistException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckDirectoryExists(string directoryPath)
        {
            CheckNullString(directoryPath);

            if (!Directory.Exists(directoryPath))
                throw new DoesNotExistException(directoryPath, "Directory specified by path \"" + directoryPath + "\" does not exist");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realToValidate"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="isInclusive"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckFloatRange(double realToValidate, double minimum, double maximum, bool isInclusive)
        {
            if (Math.Abs(minimum - maximum) < DOUBLE_TOLERANCE &&
                !isInclusive)
                throw new InvalidOperationException("Range is empty set");

            if (Math.Abs(minimum - maximum) < DOUBLE_TOLERANCE &&
                Math.Abs(realToValidate - minimum) > DOUBLE_TOLERANCE)
                throw new IndexOutOfRangeException("Value " + realToValidate + " is not equal " + minimum);

            
            if (minimum < maximum)
            {
                if (isInclusive)
                {
                    if (realToValidate < minimum ||
                        realToValidate > maximum)
                        throw new IndexOutOfRangeException("Value " + realToValidate + " is out of interval " + minimum + " - " + maximum );
                }
                else
                {
                    if (realToValidate <= minimum ||
                        realToValidate >= maximum)
                        throw new IndexOutOfRangeException("Value " + realToValidate + " is out of interval (" + minimum + "," + maximum + ")");
                }
            }
            else
            {
                if (isInclusive)
                {
                    if (realToValidate > minimum ||
                        realToValidate < maximum)
                        throw new IndexOutOfRangeException("Value " + realToValidate + " is out of interval (-inf) - " + maximum + " or " + minimum + " - (+inf)");
                }
                else
                {
                    if (realToValidate >= minimum ||
                        realToValidate <= maximum)
                        throw new IndexOutOfRangeException("Value " + realToValidate + " is out of interval (-inf) - " + maximum + " or " + minimum + " - (+inf)");
                }

            }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integerToValidate"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <exception cref="IndexOutOfRangeException">if integerToValidate is less then minimum or more than maximum</exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckIntegerRange(long integerToValidate, long minimum, long maximum)
        {
            if (minimum == maximum &&
                integerToValidate != minimum)
                throw new IndexOutOfRangeException("Value " + integerToValidate + " is not equal " + minimum);


            if (minimum < maximum)
            {
                if (integerToValidate < minimum ||
                    integerToValidate > maximum)
                    throw new IndexOutOfRangeException("Value " + integerToValidate + " is out of interval " + minimum + " - " + maximum );

            }
            else
            {
                if (integerToValidate > minimum ||
                    integerToValidate < maximum)
                    throw new IndexOutOfRangeException("Value " + integerToValidate + " is out of interval (-inf) - " + maximum + " or " + minimum + " - (+inf)");

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToValidate"></param>
        /// <param name="errorMessage"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckDefault<T>(T valueToValidate, string errorMessage)
        {
            if ((ReferenceEquals(null,valueToValidate) && ReferenceEquals(null,default(T))) ||
                (!ReferenceEquals(null,valueToValidate) && !ReferenceEquals(null,default(T)) && valueToValidate.Equals(default(T))))
                throw new ArgumentException(errorMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToValidate"></param>
        /// <param name="invalidValue"></param>
        /// <param name="errorMessage"></param>
        /// <typeparam name="T"></typeparam>
        public static void CheckInvalidArgument<T>(T valueToValidate, T invalidValue, string errorMessage)
        {
            if ((ReferenceEquals(invalidValue,null) && ReferenceEquals(valueToValidate,null)) ||
                (!ReferenceEquals(valueToValidate,null) && !ReferenceEquals(invalidValue,null) && valueToValidate.Equals(invalidValue)))
#if DEBUG && !COMPACT_FRAMEWORK
                Debug.Assert(false,errorMessage);
#else
                throw new ArgumentException(errorMessage);
#endif
        }

        private const string _patternStrict = 
                @"^(([^<>()[\]\\.,;:\s@\""]+"
              + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
              + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
              + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
              + @"[a-zA-Z]{2,}))$";

        private static volatile Regex _reStrict = null;
        private static volatile object _reStrictLock = new object();
        private const double DOUBLE_TOLERANCE = 0.000000000001;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckEmail(string address)
        {
            if (_reStrict == null)
            {
                lock (_reStrictLock)
                {
                    if (_reStrict == null)
                    {
                        _reStrict = new Regex(_patternStrict);
                    }
                }
            }

            if (!_reStrict.IsMatch(address))
                throw new ArgumentException("Not a valid e-mail address");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeToValidate"></param>
        /// <param name="requiredBaseType"></param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckBaseType(Type typeToValidate, Type requiredBaseType)
        {
            if (!requiredBaseType.IsAssignableFrom(typeToValidate))
            {
                throw new ArgumentOutOfRangeException(
                    "typeToValidate", 
                    string.Format("Required base type is not assignable from \"{0}\"",
                        typeToValidate.Name));
            }
        }
    }
}
