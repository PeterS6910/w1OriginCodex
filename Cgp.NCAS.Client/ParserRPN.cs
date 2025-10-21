using System;
using System.Collections.Generic;
using System.Collections;

namespace Contal.Cgp.NCAS.Client
{
    class ParserRPN
    {
        private Stack _stack = new Stack();
        private Queue _queue = new Queue();
        string _expression; // logical expression
        ConstantsRPN.ExprPart[] _partExprs; //rovVyrazov;
        string _errorText;

        public string ErrorText
        {
            get { return _errorText; }
        }

        public string Expresion
        {
            get { return _expression; }
        }

        public ConstantsRPN.ExprPart[] PartExprs
        {
            get { return _partExprs; }
        }

        /// <summary>
        /// Insert blank space around parenthes in member _expression
        /// </summary>
        private void SeparateParentheses()
        {
            if (_expression == null) return;
            if (_expression.Length == 0) return;

            string newExpression = string.Empty;
            newExpression += _expression[0];
            if (_expression[0] == '(')
            {
                if (_expression[1] != ' ')
                    newExpression += " ";
            }

            for (int i = 1; i < _expression.Length - 1; i++)
            {
                if (_expression[i] == '(' || _expression[i] == ')')
                {
                    if (_expression[i - 1] != ' ') newExpression += " ";
                    newExpression += _expression[i];
                    if (_expression[i + 1] != ' ') newExpression += " ";
                }
                else
                {
                    newExpression += _expression[i];
                }
            }
            if (_expression[_expression.Length - 1] == ')')
            {
                if (_expression[_expression.Length - 2] != ' ')
                    newExpression += " ";
            }
            newExpression += _expression[_expression.Length - 1];
            _expression = newExpression;
        }

        public bool TransformExpression(string expression)
        {
            _expression = expression;
            _errorText = string.Empty;
            SeparateParentheses();
            if (!ParenthesesOK())
            {
                _errorText = "Parentheses wrong.";
                return false;
            }

            _queue.Clear();
            string ActWord = string.Empty;

            string[] parts = _expression.Split(' ', '\n', '\r');
            if (parts.Length == 0) return false;
            foreach (string word in parts)
            {
                InsertString(word);
            }

            int count;
            count = _queue.Count;
            _partExprs = new ConstantsRPN.ExprPart[count];
            for (int i = 0; i < count; i++)
            {
                _partExprs[i] = (ConstantsRPN.ExprPart)_queue.Dequeue();
            }
            _queue.Clear();


            if (!OperandsOK())
            {
                _errorText = "Wrong operands.";
                return false;
            }
            if (!OperatorsOK())
            {
                _errorText = "Wrong operators.";
                return false;
            }
            return true;
        }

        private bool WillContinue(char z1, char z2)
        {
            int type2 = GetCharType(z2);
            if (type2 == 10) return true;
            int type1 = GetCharType(z1);
            if (type1 == type2)
                return true;
            return false;
        }

        //1 slovo
        //2 operacia
        //3 konstanta
        //10 predosly
        //50 nepodporovane
        private int GetCharType(char wChar)
        {
            if (wChar >= '0' && wChar <= '9')
                return 3;
            if (wChar >= 'A' && wChar <= 'Z')
                return 1;
            if (wChar == '.')
                return 10;
            if (wChar == '&' || wChar == '|' || wChar == '^' || wChar == '*' || wChar == '<'
                || wChar == '>' || wChar == '=' || wChar == '+' || wChar == '-' || wChar == '/')
                return 2;
            if (wChar == '!')
                return 27;
            if (wChar == '(')
                return 28;
            if (wChar == ')')
                return 29;
            return 50;
        }

        private bool InsertString(string input)
        {
            input = input.Trim();
            if (input == null) return false;
            if (input == string.Empty) return false;
            if (input.Length == 0) return false;
            ConstantsRPN.ExprPart inputPart;
            inputPart = GetExprPartFromWord(input);
            if (inputPart.name == string.Empty) return false;
            _queue.Enqueue(inputPart);
            return true;
        }

        private ConstantsRPN.ExprPart GetExprPartFromWord(string word)
        {
            ConstantsRPN.ExprPart vResult;
            vResult = new ConstantsRPN.ExprPart();

            int iConstant;
            bool bResult;

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

            bResult = Int32.TryParse(word, out iConstant);
            if (bResult)
            {
                vResult.name = word;
                vResult.value = iConstant;
                vResult.priority = ConstantsRPN.PRIORITY_NONE;
                vResult.exportType = ConstantsRPN.CONSTATNT_INT;
                vResult.type = ConstantsRPN.CONSTATNT_INT;
                vResult.exportValue = BitConverter.GetBytes(iConstant);
                vResult.isOperator = false;
                return vResult;
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
                if (word == "||")
                {
                    vResult.name = "||";
                    vResult.priority = ConstantsRPN.PRIORITY_LOGICALOR;
                    vResult.exportType = ConstantsRPN.OPERATOR_LOGICALOR;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "^")
                {
                    vResult.name = "^";
                    vResult.priority = ConstantsRPN.PRIORITY_XOR;
                    vResult.exportType = ConstantsRPN.OPERAROR_XOR; //33;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "!")
                {
                    vResult.name = "!";
                    vResult.priority = ConstantsRPN.PRIORITY_LOGICALNEGATION;
                    vResult.exportType = ConstantsRPN.OPERATOR_LOGICALNEGATION; //33;
                    vResult.type = ConstantsRPN.TYPE_UNARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "<")
                {
                    vResult.name = "<";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_LESS;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;

                    return vResult;
                }
                if (word == ">")
                {
                    vResult.name = ">";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_GREATER;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "<=")
                {
                    vResult.name = "<=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.isOperator = true;
                    vResult.exportType = ConstantsRPN.OPERATOR_LESSEQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    return vResult;
                }
                if (word == ">=")
                {
                    vResult.name = ">=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALLESSGREATER;
                    vResult.exportType = ConstantsRPN.OPERATOR_GREATEREQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "=")
                {
                    vResult.name = "=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALEQUAL;
                    vResult.exportType = ConstantsRPN.OPERATOR_EQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "!=")
                {
                    vResult.name = "!=";
                    vResult.priority = ConstantsRPN.PRIORITY_RELATIONALEQUAL;
                    vResult.exportType = ConstantsRPN.OPERATOR_NOTEQUAL;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }

                if (word == "+")
                {
                    vResult.name = "+";
                    vResult.priority = ConstantsRPN.PRIORITY_ADDITIONSUBTRACTION;
                    vResult.exportType = ConstantsRPN.OPERATOR_ADDITION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;

                    return vResult;
                }
                if (word == "-")
                {
                    vResult.name = "-";
                    vResult.priority = ConstantsRPN.PRIORITY_ADDITIONSUBTRACTION;
                    vResult.exportType = ConstantsRPN.OPERATOR_SUBSTRACTION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "*")
                {
                    vResult.name = "*";
                    vResult.priority = ConstantsRPN.PRIORITY_MULTIPLICATIONDIVUSION;
                    vResult.exportType = ConstantsRPN.OPERATOR_MULTIPLICATION;
                    vResult.type = ConstantsRPN.TYPE_BINARYOPERAND;
                    vResult.isOperator = true;
                    return vResult;
                }
                if (word == "/")
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
            vResult.isOperator = false;
            return vResult;
        }

        public string[] ReturnParams()
        {
            List<string> result = new List<string>();
            foreach (ConstantsRPN.ExprPart part in _partExprs)
            {
                if (part.type == ConstantsRPN.VALUE)
                {
                    result.Add(part.name);
                }
            }
            string[] stringResult = new string[result.Count];
            result.CopyTo(stringResult);
            return stringResult;
        }

        #region Checking

        private bool ParenthesesOK()
        {
            int parenthesesCount = 0;
            for (int i = 0; i < _expression.Length; i++)
            {
                if (_expression[i] == '(') parenthesesCount++;
                if (_expression[i] == ')')
                {
                    parenthesesCount--;
                    if (parenthesesCount < 0) return false;
                }
            }
            if (parenthesesCount == 0)
                return true;
            return false;
        }


        private bool OperatorsOK()
        {
            for (int i = 0; i < _partExprs.Length; i++)
            {
                if (_partExprs[i].isOperator)
                {
                    if (i == 0)
                    {
                        if (_partExprs[i].type != ConstantsRPN.TYPE_UNARYOPERAND)
                            if (_partExprs[i].type != ConstantsRPN.TYPE_PARENTHESLEFT)
                                return false; //first could be only unary operator and left parenthes
                    }
                    else if ((i + 1) == _partExprs.Length)
                    {
                        if (_partExprs[i].type != ConstantsRPN.TYPE_PARENTHESRIGHT)
                            return false; // operator cant be the last
                    }
                    else if (_partExprs[i].type == ConstantsRPN.TYPE_PARENTHESLEFT)
                    {
                        if (_partExprs[i + 1].isOperator)
                            return false;
                    }
                    else if (_partExprs[i].type == ConstantsRPN.TYPE_PARENTHESRIGHT)
                    {
                        if (_partExprs[i - 1].isOperator)
                            return false;
                    }
                    else
                    {
                        if (_partExprs[i + 1].isOperator)
                        {
                            if (_partExprs[i + 1].type != ConstantsRPN.TYPE_UNARYOPERAND)
                                if (_partExprs[i + 1].type != ConstantsRPN.TYPE_PARENTHESLEFT)
                                    return false;
                        }
                        if (_partExprs[i].type == ConstantsRPN.TYPE_UNARYOPERAND)
                        {
                            if (!_partExprs[i - 1].isOperator) return false;
                        }
                        else
                        {
                            if (_partExprs[i - 1].isOperator)
                                if (_partExprs[i - 1].type != ConstantsRPN.TYPE_UNARYOPERAND)
                                    if (_partExprs[i - 1].type != ConstantsRPN.TYPE_PARENTHESRIGHT)
                                        return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool OperandsOK()
        {
            //operand cant by next another operand
            for (int i = 0; i < _partExprs.Length; i++)
            {
                if (!_partExprs[i].isOperator)
                {
                    if (i != 0)
                        if (!_partExprs[i - 1].isOperator) return false;
                    if (i + 1 != _partExprs.Length)
                        if (!_partExprs[i + 1].isOperator) return false;
                }
            }
            return true;
        }
        #endregion
    }

    public class ConstantsRPN
    {
        //constants values
        public const byte VALUE = 1;
        public const byte VALUE_BOOL = 2;
        public const byte VALUE_BYTE = 3;
        public const byte VALUE_INT = 4;
        public const byte VALUE_LONG = 5;
        public const byte VALUE_FLOAT = 6;
        public const byte VALUE_DOUBLE = 7;
        public const byte VALUE_STRING = 8;

        public const byte CONSTATNT = 11;
        public const byte CONSTATNT_BOOL = 12;
        public const byte CONSTATNT_BYTE = 13;
        public const byte CONSTATNT_INT = 14;
        public const byte CONSTATNT_LONG = 15;
        public const byte CONSTATNT_FLOAT = 16;
        public const byte CONSTATNT_DOUBLE = 17;
        public const byte CONSTATNT_STRING = 18;

        public const byte POINTER_MV = 21;
        public const byte POINTER_MV_BOOL = 22;
        public const byte POINTER_MV_BYTE = 23;
        public const byte POINTER_MV_INT = 24;
        public const byte POINTER_MV_LONG = 25;
        public const byte POINTER_MV_FLOAT = 26;
        public const byte POINTER_MV_DOUBLE = 27;
        public const byte POINTER_MV_STRING = 28;

        public const byte POINTER = 31;
        public const byte POINTER_BOOL = 32;
        public const byte POINTER_BYTE = 33;
        public const byte POINTER_INT = 34;
        public const byte POINTER_LONG = 35;
        public const byte POINTER_FLOAT = 36;
        public const byte POINTER_DOUBLE = 37;
        public const byte POINTER_STRING = 38;

        public const byte MAX_VALUE = 9;
        public const byte MAX_CONSTANT = 19;
        public const byte MAX_POINTER = 50;


        //constants operators 
        public const byte OPERATOR_LOGICALNEGATION = 51;    // ! 
        public const byte OPERATOR_MAXUNARY = 60;           // decide maximal numbers for unary operators

        public const byte OPERATOR_LOGICALOR = 71;          // || 
        public const byte OPERATOR_LOGICALAND = 72;         // && 
        public const byte OPERAROR_XOR = 73;                // ^ 

        public const byte OPERATOR_LESS = 81;               // < 
        public const byte OPERATOR_GREATER = 82;            // > 
        public const byte OPERATOR_LESSEQUAL = 83;          // <= 
        public const byte OPERATOR_GREATEREQUAL = 84;       // >= 
        public const byte OPERATOR_EQUAL = 85;              // == 
        public const byte OPERATOR_NOTEQUAL = 86;           // !=

        public const byte OPERATOR_ADDITION = 91;           // + 
        public const byte OPERATOR_SUBSTRACTION = 92;       // - 
        public const byte OPERATOR_MULTIPLICATION = 93;     // * 
        public const byte OPERATOR_DIVISION = 94;           // / 

        public const byte MAXLOGICALINPUT = 80;
        public const byte MAXLOGICALOUTPUT = 90;

        /*
         * pri operatoroch sledovat
         * 1 ci su binarne alebo unarne
         * s akymy premennymi pracuju boolean alebo cislo
         * aky vratia vsledok bool, alebo cislo
         * */


        public const byte OPERATOR_NOTVALID = 250;          // () - will not be valid, not used in 

        //constants priority
        public const byte PRIORITY_PARENTHESES = 90;            // ()
        public const byte PRIORITY_LOGICALNEGATION = 80;        // !
        public const byte PRIORITY_MULTIPLICATIONDIVUSION = 70; // *,/
        public const byte PRIORITY_ADDITIONSUBTRACTION = 60;    // +,-
        public const byte PRIORITY_RELATIONALLESSGREATER = 50;  // <, <=, >, >=
        public const byte PRIORITY_RELATIONALEQUAL = 40;        // ==, !=
        public const byte PRIORITY_XOR = 30;                    // ^
        public const byte PRIORITY_LOGICLALAND = 20;            // &&
        public const byte PRIORITY_LOGICALOR = 10;              // ||
        public const byte PRIORITY_NONE = 250;

        //constants operator type
        public const byte TYPE_VALUE = 0;
        public const byte TYPE_CONSTANT = 5;
        public const byte TYPE_PARENTHESLEFT = 10;
        public const byte TYPE_PARENTHESRIGHT = 11;
        public const byte TYPE_UNARYOPERAND = 20;
        public const byte TYPE_BINARYOPERAND = 21;

        public struct ExprPart
        {
            public string name;
            public int priority;
            public int value;
            public bool isOperator;
            public byte type;
            public byte exportType;
            public byte[] exportValue;
        }

        [Serializable]
        public struct PartRPN
        {
            public byte identity;
            public byte[] value;
        }

        //static public double ReturnValueFromPartRPN(PartRPN workingPart)
        //{
        //    byte[] tempByte = workingPart.value;

        //    //Array.Resize(ref tempByte, 8);
        //    return BitConverter.ToDouble(tempByte, 0);
        //}

        //static public void SetValuePartRPNDouble(ref PartRPN workingPart, double insertValue)
        //{
        //    workingPart.value = BitConverter.GetBytes(insertValue);
        //}

        static public bool ValueRPNIsBool(PartRPN actPRPN)
        {
            if (actPRPN.identity == CONSTATNT_BOOL)
                return true;
            if (actPRPN.identity == VALUE_BOOL)
                return true;
            if (actPRPN.identity == POINTER_BOOL)
                return true;
            if (actPRPN.identity == POINTER_MV_BOOL)
                return true;
            return false;
        }


        static public bool ValueRPNIsInt(PartRPN actPRPN)
        {
            if (actPRPN.identity == CONSTATNT_INT)
                return true;
            if (actPRPN.identity == VALUE_INT)
                return true;
            if (actPRPN.identity == POINTER_INT)
                return true;
            if (actPRPN.identity == POINTER_MV_INT)
                return true;
            return false;
        }

        static public bool ValueRPNIsLong(PartRPN actPRPN)
        {
            if (actPRPN.identity == CONSTATNT_LONG)
                return true;
            if (actPRPN.identity == VALUE_LONG)
                return true;
            if (actPRPN.identity == POINTER_LONG)
                return true;
            if (actPRPN.identity == POINTER_MV_LONG)
                return true;
            return false;
        }

        static public bool ValueRPNIsFloat(PartRPN actPRPN)
        {
            if (actPRPN.identity == CONSTATNT_FLOAT)
                return true;
            if (actPRPN.identity == VALUE_FLOAT)
                return true;
            if (actPRPN.identity == POINTER_FLOAT)
                return true;
            if (actPRPN.identity == POINTER_MV_FLOAT)
                return true;
            return false;
        }

        static public bool ValueRPNIsDouble(PartRPN actPRPN)
        {
            if (actPRPN.identity == VALUE_DOUBLE)
                return true;
            if (actPRPN.identity == POINTER_DOUBLE)
                return true;
            if (actPRPN.identity == POINTER_MV_DOUBLE)
                return true;
            return false;
        }

        static public bool ReturnValueRPN(PartRPN actPRPN, out float floatValue)
        {
            floatValue = 0;
            //if (!ValueRPNIsFloat(ActPRPN))
            //{
            //    FloatValue = 0;
            //    return false;
            //}
            byte[] tmpAB = actPRPN.value;
            //Array.Resize(ref tmpAB, 4);
            if (tmpAB.Length < 4) return false;
            floatValue = BitConverter.ToSingle(tmpAB, 0);
            return true;
        }

        static public bool ReturnValueRPN(PartRPN actPRPN, out double doubleValue)
        {
            doubleValue = 0;
            //if (!ValueRPNIsDouble(ActPRPN))
            //{
            //    DoubleValue = 0;
            //    return false;
            //}
            byte[] tmpAB = actPRPN.value;
            //Array.Resize(ref tmpAB, 8);
            if (tmpAB.Length < 8) return false;
            doubleValue = BitConverter.ToDouble(tmpAB, 0);
            return true;
        }

        static public void SetValueRPN(ref PartRPN actPRPN, bool boolValue)
        {
            actPRPN.value = BitConverter.GetBytes(boolValue);
        }

        static public void SetValueRPN(ref PartRPN actPRPN, int intValue)
        {
            actPRPN.value = BitConverter.GetBytes(intValue);
        }

        static public void SetValueRPN(ref PartRPN actPRPN, float floatValue)
        {
            actPRPN.value = BitConverter.GetBytes(floatValue);
        }

        static public void SetValueRPN(ref PartRPN actPRPN, double doubleValue)
        {
            actPRPN.value = BitConverter.GetBytes(doubleValue);
        }
    }

}
