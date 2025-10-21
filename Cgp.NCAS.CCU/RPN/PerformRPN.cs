using System;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.RPN
{
    public class PerformRPN
    {
        #region Constants
        private const byte ERROR_NOERROR = 0; //there is no error
        private const byte ERROR_FILE = 1; //File not exits
        private const byte ERROR_INPUTARRAY = 2; //empty input array
        private const byte ERROR_VARIABLES = 3;  //Not enought variables in stack
        private const byte ERROR_WRONGVALUETYPE = 10; //wrong value type
        private const byte ERROR_CALCULATIONFAIL = 13; //stack contains more values instend one result
        private const byte ERROR_OPERATIONNOTIMP = 14; //operation not implemented

        private const byte ERROR_READINGSTREAM = 21;
        private const byte ERROR_READINGFILE = 22;
        private const byte ERROR_READING_STRING = 23;
        private const byte ERROR_READING_TABLE = 24;

        private const byte ERROR_NORESULT = 30; //
        private const byte ERROR_INPUTSTREAMEMPTY = 101;
        private const byte ERROR_INVALIDINPUTSTREAM = 102;
        private const byte ERROR_NOINPUTDATA = 103;
        #endregion

        #region Variables
        ConstantsRPN.PartRPN[] _arrrayRPN;
        ConstantsRPN.PartRPN _resultRPN;
        byte _errorCode;
        StackArray _aryStack;
        
        #endregion

        #region Events
        public event DVoid2Void ErrorOccur;
        #endregion

        #region Properties
        public byte ErrorCode
        {
            get { return _errorCode; }
        }

        public double GetResultRPN
        {
            get
            {
                //if (_resultRPN == null) return 0;
                if (_resultRPN.identity == ConstantsRPN.POINTER_BOOL || _resultRPN.identity == ConstantsRPN.POINTER_MV_BOOL)
                {
                    bool VarBool;
                    if (!ConstantsRPN.ReturnValueRPN(_resultRPN, out VarBool))
                    {
                        _errorCode = ERROR_NORESULT;
                        if (ErrorOccur != null) ErrorOccur();
                        return 0;
                    }
                    if (VarBool)
                        return 1;
                    else
                        return 0;
                }

                if (_resultRPN.identity == ConstantsRPN.VALUE_BOOL)
                {
                    bool VarBool;
                    if (!ConstantsRPN.ReturnValueRPN(_resultRPN, out VarBool))
                    {
                        _errorCode = ERROR_NORESULT;
                        if (ErrorOccur != null) ErrorOccur();
                        return 0;
                    }
                    if (VarBool)
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    int VarInt;
                    if (!ConstantsRPN.ReturnValueRPN(_resultRPN, out VarInt))
                        return 0;
                    return (double)VarInt;
                }
            }
        }

        public bool GetBoolResultRPN
        {
            get
            {
                //if (_resultRPN == null) return 0;
                if (_resultRPN.identity == ConstantsRPN.POINTER_MV_BOOL || _resultRPN.identity == ConstantsRPN.POINTER_BOOL)
                {
                    bool VarBool;
                    if (!ConstantsRPN.ReturnValueRPN(_resultRPN, out VarBool))
                    {
                        _errorCode = ERROR_NORESULT;
                        if (ErrorOccur != null) ErrorOccur();
                        return false;
                    }
                    if (VarBool)
                        return true;
                    else
                        return false;
                }
                else if (_resultRPN.identity == ConstantsRPN.VALUE_BOOL)
                {
                    bool VarBool;
                    if (!ConstantsRPN.ReturnValueRPN(_resultRPN, out VarBool))
                    {
                        _errorCode = ERROR_NORESULT;
                        if (ErrorOccur != null) ErrorOccur();
                        return false;
                    }
                    if (VarBool)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Loading RPN
        public bool LoadRPNFromString(string inputString) //, string inputTable)
        {
            if (inputString == null || inputString == string.Empty)
            {
                _errorCode = 1;
                if (ErrorOccur != null) ErrorOccur();
                return false;
            }

            try
            {
                CcuCore.Singleton.TableVariables.RecreateTableOfVariables(inputString);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                _errorCode = ERROR_READING_TABLE;
                if (ErrorOccur != null) ErrorOccur();
                return false;
            }

            try
            {
                ConstantsRPN.ExprPart tmpExprPart;

                string[] partsRPN = inputString.Split('~');
                _arrrayRPN = new ConstantsRPN.PartRPN[partsRPN.Length];
                int actPos = 0;
                foreach (string part in partsRPN)
                {
                    tmpExprPart = GetExprPartFromWord(part);
                    _arrrayRPN[actPos].identity = tmpExprPart.exportType;
                    if (tmpExprPart.exportType == ConstantsRPN.POINTER_MV_BOOL)
                    {
                        _arrrayRPN[actPos].value = CcuCore.Singleton.TableVariables.GetNameFromTableOfVariables(part);
                    }
                    else
                    {
                        _arrrayRPN[actPos].value = tmpExprPart.exportValue;
                    }
                    actPos++;
                }
                CreateStack();
                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                _errorCode = ERROR_READINGFILE;
                if (ErrorOccur != null) ErrorOccur();
                return false;
            }
        }

        private ConstantsRPN.ExprPart GetExprPartFromWord(string word)
        {
            ConstantsRPN.ExprPart vResult;
            vResult = new ConstantsRPN.ExprPart();
            int iConstant;

            if (word == "(")
            {
                vResult.name = "(";
                vResult.priority = ConstantsRPN.PRIORITY_PARENTHESES;
                vResult.type = ConstantsRPN.TYPE_PARENTHESLEFT;
                vResult.exportType = ConstantsRPN.OPERATOR_NOTVALID;
                vResult.isOperator = true;
                return vResult;
            }

            if (word == ")")
            {
                vResult.name = ")";
                vResult.priority = ConstantsRPN.PRIORITY_PARENTHESES;
                vResult.type = ConstantsRPN.TYPE_PARENTHESRIGHT;
                vResult.exportType = ConstantsRPN.OPERATOR_NOTVALID;
                vResult.isOperator = true;
                return vResult;
            }

            try
            {
                iConstant = Convert.ToInt32(word);
                vResult.name = word;
                vResult.value = iConstant;
                vResult.priority = ConstantsRPN.PRIORITY_NONE;
                vResult.exportType = ConstantsRPN.CONSTATNT_INT;
                vResult.type = ConstantsRPN.CONSTATNT_INT;
                vResult.exportValue = BitConverter.GetBytes(iConstant);
                vResult.isOperator = false;
                return vResult;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            if (word.Length <= 2)
            {
                if (word == "&&")
                {
                    vResult.name = "&&";
                    vResult.priority = ConstantsRPN.PRIORITY_LOGICLALAND;
                    vResult.exportType = ConstantsRPN.OPERATOR_LOGICALAND;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "||")
                {
                    vResult.name = "||";
                    vResult.priority = ConstantsRPN.PRIORITY_LOGICALOR;
                    vResult.exportType = ConstantsRPN.OPERATOR_LOGICALOR;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "^")
                {
                    vResult.name = "^";
                    vResult.priority = ConstantsRPN.PRIORITY_XOR;
                    vResult.exportType = ConstantsRPN.OPERAROR_XOR; //33;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "!")
                {
                    vResult.name = "!";
                    vResult.priority = ConstantsRPN.PRIORITY_LOGICALNEGATION;
                    vResult.exportType = ConstantsRPN.OPERATOR_LOGICALNEGATION; //33;
                    vResult.type = ConstantsRPN.TYPE_UNARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "<")
                {
                    vResult.name = "<";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_LESS;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;

                    return vResult;
                }
                else if (word == ">")
                {
                    vResult.name = ">";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_GREATER;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "<=")
                {
                    vResult.name = "<=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.isOperator = true;
                    vResult.exportType = ConstantsRPN.OPERATOR_LESSEQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    return vResult;
                }
                else if (word == ">=")
                {
                    vResult.name = ">=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_GREATEREQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "=")
                {
                    vResult.name = "=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALEQUAL;
                    vResult.exportType = ConstantsRPN.OPERATOR_EQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "!=")
                {
                    vResult.name = "!=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALEQUAL;
                    vResult.exportType = ConstantsRPN.OPERATOR_NOTEQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }

                else if (word == "+")
                {
                    vResult.name = "+";
                    vResult.priority = ConstantsRPN.PRIORITY_ADDITIONSUBTRACTION;
                    vResult.exportType = ConstantsRPN.OPERATOR_ADDITION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;

                    return vResult;
                }
                else if (word == "-")
                {
                    vResult.name = "-";
                    vResult.priority = ConstantsRPN.PRIORITY_ADDITIONSUBTRACTION;
                    vResult.exportType = ConstantsRPN.OPERATOR_SUBSTRACTION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "*")
                {
                    vResult.name = "*";
                    vResult.priority = ConstantsRPN.PRIORITY_MULTIPLICATIONDIVUSION;
                    vResult.exportType = ConstantsRPN.OPERATOR_MULTIPLICATION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                else if (word == "/")
                {
                    vResult.name = "/";
                    vResult.priority = ConstantsRPN.PRIORITY_MULTIPLICATIONDIVUSION;
                    vResult.exportType = ConstantsRPN.OPERATOR_DIVISION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
            }

            if (word == "TRUE")
            {
                vResult.name = word;
                vResult.priority = ConstantsRPN.PRIORITY_NONE;
                vResult.exportType = ConstantsRPN.CONSTATNT_BOOL;
                vResult.type = ConstantsRPN.TYPE_CONSTANT;
                vResult.value = 1;
                vResult.exportValue = BitConverter.GetBytes(true);
                vResult.isOperator = false;
                return vResult;
            }
            if (word == "FALSE")
            {
                vResult.name = word;
                vResult.priority = ConstantsRPN.PRIORITY_NONE;
                vResult.exportType = ConstantsRPN.CONSTATNT_BOOL;
                vResult.type = ConstantsRPN.TYPE_CONSTANT;
                vResult.value = 0;
                vResult.exportValue = BitConverter.GetBytes(false);
                vResult.isOperator = false;
                return vResult;
            }

            vResult.name = word;
            vResult.priority = ConstantsRPN.PRIORITY_NONE;
            vResult.type = ConstantsRPN.VALUE;
            vResult.exportType = ConstantsRPN.POINTER_MV_BOOL;
            vResult.isOperator = false;
            return vResult;
        }

        private void CreateStack()
        {
            int valuesCount = 0;
            for (int i = 0; i < _arrrayRPN.Length; i++)
            {
                if (_arrrayRPN[i].identity <= ConstantsRPN.MAX_POINTER)
                    valuesCount++;
            }
            _aryStack = new StackArray(valuesCount);
        }
        #endregion

        /// <summary>
        /// Execute RPN from array RPN
        /// </summary>
        /// <returns>returns true on success</returns>
        public bool RunRPN()
        {
            _errorCode = ERROR_NOERROR;
            if (_arrrayRPN == null)
            {
                _errorCode = ERROR_NOINPUTDATA;
                if (ErrorOccur != null) ErrorOccur();
                return false;
            }
            ExecuteRPN();
            if (_errorCode != ERROR_NOERROR) return false;
            return true;
        }

        /// <summary>
        /// Execution of reverse polish notation
        /// </summary>
        /// <returns>returns true on success</returns>
        private bool ExecuteRPN()
        {
            try
            {
                _aryStack.Reset();
                for (int i = 0; i < _arrrayRPN.Length; i++)
                {
                    if (_arrrayRPN[i].identity <= ConstantsRPN.MAX_POINTER)
                    {// values, constants,pointer < MAX_POINTER simply inserted into stack
                        _aryStack.Insert((ConstantsRPN.PartRPN)_arrrayRPN[i]);
                    }
                    else if (_arrrayRPN[i].identity <= ConstantsRPN.OPERATOR_MAXUNARY)
                    {// operator execute determined operation
                        if (_aryStack.Count < 1)
                        {
                            _errorCode = ERROR_VARIABLES;
                            if (ErrorOccur != null) ErrorOccur();
                            return false;
                        }
                        RunUnaryOperation(_arrrayRPN[i].identity);
                    }
                    else
                    {
                        if (_aryStack.Count < 2)
                        {
                            _errorCode = ERROR_VARIABLES;
                            if (ErrorOccur != null) ErrorOccur();
                            return false;
                        }
                        RunBinaryOperation(_arrrayRPN[i].identity);
                    }
                }

                //all is done, in stact have to be only one value the result of RPN
                //if (_stackValues.Count == 1)
                if (_aryStack.Count == 1)
                {
                    _resultRPN = (_aryStack.Return());
                    return true;
                }
                else
                {//FAIL if there is more than one value
                    _errorCode = ERROR_CALCULATIONFAIL;
                    if (ErrorOccur != null) ErrorOccur();
                    return false;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                _errorCode = ERROR_CALCULATIONFAIL;
                if (ErrorOccur != null) ErrorOccur();
                return false;
            }
        }

        #region Defined operations
        /// <summary>
        /// Call unary operation by param and push result into stack
        /// </summary>
        /// <param name="operationType">operation type</param>
        private void RunUnaryOperation(byte operationType)
        {
            ConstantsRPN.PartRPN value = _aryStack.Return();
            _aryStack.Insert(OperationNot(value));
        }

        /// <summary>
        /// Call bynary operation by param and result push into the stack
        /// </summary>
        /// <param name="OperationType">byte that says which type of operation perform</param>
        private void RunBinaryOperation(byte operationType)
        {
            ConstantsRPN.PartRPN val2 = _aryStack.Return();
            ConstantsRPN.PartRPN val1 = _aryStack.Return();

            switch (operationType)
            {
                case ConstantsRPN.OPERATOR_LOGICALAND:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationAnd(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_LOGICALOR:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationOr(val1, val2));
                    break;
                case ConstantsRPN.OPERAROR_XOR:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationExclusiveOr(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_LESS:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationLess(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_GREATER:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationGreater(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_EQUAL:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationEqual(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_NOTEQUAL:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationNotEqual(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_LESSEQUAL:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationLessEqual(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_GREATEREQUAL:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationGreaterEqual(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_ADDITION:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationAddition(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_SUBSTRACTION:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationSubstraction(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_MULTIPLICATION:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationMultiplication(val1, val2));
                    break;
                case ConstantsRPN.OPERATOR_DIVISION:
                    _aryStack.Insert((ConstantsRPN.PartRPN)OperationDivision(val1, val2));
                    break;
                default:
                    _errorCode = ERROR_OPERATIONNOTIMP;
                    if (ErrorOccur != null) ErrorOccur();
                    throw new ApplicationException("not implemented operation");
            }
        }

        private ConstantsRPN.PartRPN OperationNot(ConstantsRPN.PartRPN value)
        {
            ConstantsRPN.PartRPN oprResult;
            bool tempBoolValue;
            oprResult = new ConstantsRPN.PartRPN();
            if (!ConstantsRPN.ReturnValueRPN(value, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (tempBoolValue)
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            oprResult.identity = ConstantsRPN.VALUE_BOOL;
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationAnd(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            //all have to by true
            ConstantsRPN.PartRPN oprResult;
            bool tempBoolValue;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
                //return oprResult;
            }
            if (!tempBoolValue)
            {
                ConstantsRPN.SetValueRPN(ref oprResult, false);
                return oprResult;
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
                //return oprResult;
            }
            if (!tempBoolValue)
            {
                ConstantsRPN.SetValueRPN(ref oprResult, false);
                return oprResult;
            }
            ConstantsRPN.SetValueRPN(ref oprResult, true);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationOr(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            //at least one have to by true
            ConstantsRPN.PartRPN oprResult;
            bool tempBoolValue;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (tempBoolValue)
            {
                ConstantsRPN.SetValueRPN(ref oprResult, true);
                return oprResult;
            }

            if (!ConstantsRPN.ReturnValueRPN(val2, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (tempBoolValue)
            {
                ConstantsRPN.SetValueRPN(ref oprResult, true);
                return oprResult;
            }
            ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationExclusiveOr(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            //only one value have to by true
            ConstantsRPN.PartRPN oprResult;
            bool tempBoolValue;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out tempBoolValue))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (tempBoolValue)
            {
                if (!ConstantsRPN.ReturnValueRPN(val2, out tempBoolValue))
                {
                    _errorCode = ERROR_WRONGVALUETYPE;
                    if (ErrorOccur != null) ErrorOccur();
                    throw new ApplicationException("wrong variable");
                }

                if (!tempBoolValue)
                {
                    ConstantsRPN.SetValueRPN(ref oprResult, true);
                    return oprResult;
                }
            }
            else
            {
                if (!ConstantsRPN.ReturnValueRPN(val2, out tempBoolValue))
                {
                    _errorCode = ERROR_WRONGVALUETYPE;
                    if (ErrorOccur != null) ErrorOccur();
                    throw new ApplicationException("wrong variable");
                }
                if (tempBoolValue)
                {
                    ConstantsRPN.SetValueRPN(ref oprResult, true);
                    return oprResult;
                }
            }
            ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationLess(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 < valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationGreater(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 > valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationEqual(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 == valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationNotEqual(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 != valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationLessEqual(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 <= valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationGreaterEqual(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (valInt1 >= valInt2)
                ConstantsRPN.SetValueRPN(ref oprResult, true);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationAddition(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            int resultInt;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_INT;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            resultInt = valInt1 + valInt2;
            ConstantsRPN.SetValueRPN(ref oprResult, resultInt);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationSubstraction(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            int resultInt;

            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_INT;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            resultInt = valInt1 - valInt2;
            ConstantsRPN.SetValueRPN(ref oprResult, resultInt);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationMultiplication(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            int valInt1;
            int valInt2;
            int resultInt;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_INT;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            resultInt = valInt1 * valInt2;
            ConstantsRPN.SetValueRPN(ref oprResult, resultInt);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationDivision(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            ConstantsRPN.PartRPN oprResult;
            oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_INT;
            int valInt1;
            int valInt2;
            int resultInt;

            if (!ConstantsRPN.ReturnValueRPN(val1, out valInt1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }
            if (!ConstantsRPN.ReturnValueRPN(val2, out valInt2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            resultInt = valInt1 / valInt2;
            ConstantsRPN.SetValueRPN(ref oprResult, resultInt);
            return oprResult;
        }
        #endregion


        private ConstantsRPN.PartRPN OperationExclusiveOrVERC(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            bool bval1;
            bool bval2;
            ConstantsRPN.PartRPN oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out bval1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (!ConstantsRPN.ReturnValueRPN(val2, out bval2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (bval1 ^ bval2)
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationAndVERC(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            bool bval1;
            bool bval2;
            ConstantsRPN.PartRPN oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out bval1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (!ConstantsRPN.ReturnValueRPN(val2, out bval2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (bval1 && bval2)
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }

        private ConstantsRPN.PartRPN OperationOrVERC(ConstantsRPN.PartRPN val1, ConstantsRPN.PartRPN val2)
        {
            bool bval1;
            bool bval2;
            ConstantsRPN.PartRPN oprResult = new ConstantsRPN.PartRPN();
            oprResult.identity = ConstantsRPN.VALUE_BOOL;

            if (!ConstantsRPN.ReturnValueRPN(val1, out bval1))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (!ConstantsRPN.ReturnValueRPN(val2, out bval2))
            {
                _errorCode = ERROR_WRONGVALUETYPE;
                if (ErrorOccur != null) ErrorOccur();
                throw new ApplicationException("wrong variable");
            }

            if (bval1 || bval2)
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            else
                ConstantsRPN.SetValueRPN(ref oprResult, false);
            return oprResult;
        }
    }
}
