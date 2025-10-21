using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Diagnostics;

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
        /// <param name="object">object variable to test</param>
        public static bool IsNull(Object inputObject)
        {
            if (null == inputObject)
                return true;

            try
            {
                string strTmp = inputObject.ToString();
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// throws ArgumentNullException, if the object input equals null
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="object">object variable to test</param>
        /// <param name="i_strParamName">name of the object variable</param>
        /*public static void CheckNull(Object object, string i_strParamName)
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
        public static void CheckNull(params Object[] objects)
        {
            ArgumentNullException aError = null;
            if (null == objects)
            {
                aError = new ArgumentNullException();
#if DEBUG
                Debug.Assert(false,aError.GetType().Name,aError.Message);
#else
                throw aError;
#endif
            }

            foreach (Object aObject in objects)
            {
                if (null == aObject)
                {
                    aError = new ArgumentNullException();
#if DEBUG
                    Debug.Assert(false,aError.GetType().Name,aError.Message);
#else
                    throw aError;
#endif
                }
            }
        }

        /// <summary>
        /// returns true, if the string input is not null and not empty
        /// </summary>
        /// <param name="i_strObject">string variable to test</param>
        public static bool IsNotNullString(string stringToValidate)
        {
            return
                (null != stringToValidate &&
                0 != stringToValidate.Length);
        }

        /// <summary>
        /// returns true, if the string input is null or empty
        /// </summary>
        /// <param name="i_strObject">string variable to test</param>
        public static bool IsNullString(string stringToValidate)
        {
            return
                (null == stringToValidate ||
                0 == stringToValidate.Length);
        }

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="i_strObject">string variable to test</param>
        /// <param name="i_strParamName">name of the string variable</param>
        public static void CheckNullString(String stringToValidate, string parameterName)
        {
            if (null == stringToValidate || 0 == stringToValidate.Length)
            {
                ArgumentException aError = null;

                if (null == parameterName)
                    aError = new ArgumentNullException("String is invalid");
                else
                    aError = new ArgumentNullException("String \"" + parameterName + "\" is invalid", parameterName);

                throw aError;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_lValue"></param>
        public static void CheckZero(long integerToValidate)
        {
            if (0 == integerToValidate)
                    throw new ArgumentNullException("Integer value cannot be zero");

        }

        /// <summary>
        /// throws ArgumentNullException, if the string input is invalid (null or empty)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="i_strObject">string variable to test</param>
        public static void CheckNullString(String stringToValidate)
        {
            CheckNullString(stringToValidate, null);
        }


        /// <summary>
        /// throws ArgumentOutOfRangeException, if the integer input is less than zero
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="i_iValue">integer variable to test</param>
        /// <param name="i_strParamName">name of the integer variable</param>
        public static void CheckNegativeInt(int integerToValidate, string parameterName)
        {
            if (integerToValidate < 0)
            {
                ArgumentOutOfRangeException aError = null;

                if (null == parameterName)
                    aError = new ArgumentOutOfRangeException("Integer value is negative");
                else
                    aError = new ArgumentOutOfRangeException("Integer value \"" + parameterName + "\" is negative", parameterName);

                throw aError;
            }
        }


        /// <summary>
        /// throws ArgumentOutOfRangeException, if the integer input is less than zero
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <param name="i_iValue">integer variable to test</param>
        public static void CheckNegativeInt(int integerToValidate)
        {
            CheckNegativeInt(integerToValidate, null);
        }

        public static void CheckNegativeOrZeroInt(int integerToValidate, string parameterName)
        {
            if (integerToValidate <= 0)
            {
                ArgumentOutOfRangeException aError = null;

                if (null == parameterName)
                    aError = new ArgumentOutOfRangeException("Integer value is negative");
                else
                    aError = new ArgumentOutOfRangeException("Integer value \"" + parameterName + "\" is negative", parameterName);

                throw aError;
            }
        }

        public static void CheckNegativeOrZeroInt(int integerToValidate)
        {
            CheckNegativeOrZeroInt(integerToValidate, null);
        }

        /// <summary>
        /// throws InvalidOperationException, if the condition input is true
        /// </summary>
        /// <param name="isConditionTrue">input condition</param>
        public static void CheckInvalidOperation(bool isConditionTrue)
        {
            if (isConditionTrue)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">path of the file to check</param>
        /// <exception cref="ArgumentNullException">if the file path is null or empty</exception>
        /// <exception cref="EDoesNotExist">if the file on specified path does not exist</exception>
        public static void CheckFileExists(string filePath)
        {
            CheckNullString(filePath);

            if (!File.Exists(filePath))
                throw new DoesNotExistException(filePath, "File specified by path \"" + filePath + "\" does not exist");

        }

        public static void CheckDirectoryExists(string directoryPath)
        {
            CheckNullString(directoryPath);

            if (!Directory.Exists(directoryPath))
                throw new DoesNotExistException(directoryPath, "Directory specified by path \"" + directoryPath + "\" does not exist");

        }

        public static void CheckFloatRange(double realToValidate, double minimum, double maximum, bool isInclusive)
        {
            if (minimum == maximum &&
                !isInclusive)
                throw new InvalidOperationException("Range is empty set");

            if (minimum == maximum &&
                realToValidate != minimum)
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

        public static void CheckIntegerRange(long integerToValidate, long minimum, long maximum)
        {
            if (minimum == maximum &&
                integerToValidate != minimum)
                throw new IndexOutOfRangeException("Value " + integerToValidate + " is not equal " + minimum+
                    "Out of range <"+minimum+","+maximum+">");


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

        public static void CheckDefault<T>(T valueToValidate, string errorMessage)
        {
            if ((null == valueToValidate && null == default(T)) ||
                (null != valueToValidate && null != default(T) && valueToValidate.Equals(default(T))))
                throw new ArgumentException(errorMessage);
        }

        public static void CheckInvalidArgument<T>(T valueToValidate, T invalidValue, string errorMessage)
        {
            if ((null == invalidValue && null == valueToValidate) ||
                (null != valueToValidate && null != invalidValue && valueToValidate.Equals(invalidValue)))
                throw new ArgumentException(errorMessage);
        }
    }
}
