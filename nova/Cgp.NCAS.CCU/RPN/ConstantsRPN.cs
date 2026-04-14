using System;

namespace Contal.Cgp.NCAS.CCU.RPN
{
    class ConstantsRPN
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

        static public bool ReturnValueRPN(PartRPN actPRPN, out bool boolValue)
        {
            boolValue = false;
            if (actPRPN.identity == POINTER_MV_BOOL)
            {
                boolValue = CcuCore.Singleton.TableVariables.GetVariableStatus(actPRPN.value);
                return true;
            }
            else if (actPRPN.identity == VALUE_BOOL || actPRPN.identity == CONSTATNT_BOOL)
            {
                byte[] tmpAB = actPRPN.value;
                if (tmpAB.Length < 1) return false;
                boolValue = BitConverter.ToBoolean(tmpAB, 0);
                return true;
            }
            return false;
        }

        //*********************************************************
        //NOT SUPPORT RETURN INT VALUE FROM DICTIONARY OF VARIABLES
        //*********************************************************
        //
        static public bool ReturnValueRPN(PartRPN actPRPN, out int intValue)
        {
            intValue = 0;
            //if (ActPRPN.identity == POINTER_INT)
            //if (actPRPN.identity == POINTER_MV_INT)
            //{
            //    //int Position = BitConverter.ToInt32(actPRPN.value, 0);
            //    //ClassLibCF.TabHod.ReturnValue(Position, out intValue);
            //    //return true;
            //}
            //else
            if (actPRPN.identity == VALUE_INT || actPRPN.identity == CONSTATNT_INT)
            {
                byte[] tmpAB = actPRPN.value;
                //Array.Resize(ref tmpAB, 4);
                if (tmpAB.Length < 4) return false;
                intValue = BitConverter.ToInt32(tmpAB, 0);
                return true;
            }
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
