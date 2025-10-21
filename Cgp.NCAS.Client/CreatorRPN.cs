using System;
using System.Collections.Generic;
using System.Collections;

namespace Contal.Cgp.NCAS.Client
{
    class CreatorRPN
    {
        private PrepareRPN _rpnTree;

        public bool CreateRPN(ParserRPN inputParserRPN)
        {
            _rpnTree = new PrepareRPN();
            _rpnTree.CreateTree(inputParserRPN.PartExprs);
            return true;
        }
    }

    class Element
    {
        ConstantsRPN.ExprPart _exprPart;
        Element _left;
        Element _right;

        public ConstantsRPN.ExprPart Value
        {
            get { return _exprPart; }
            set { _exprPart = value; }
        }

        public Element LeftElement
        {
            get { return _left; }
            set { _left = value; }
        }

        public Element RightElement
        {
            get { return _right; }
            set { _right = value; }
        }
    }

    public class PrepareRPN
    {
        Element _rootTreeRPN;
        Queue _resultQueue;
        ConstantsRPN.PartRPN[] _arrayPartsRPN;
        byte[] _RPNByteStream;
        List<byte> _sendList;

        public byte[] RPNByteStream
        {
            get
            {
                CreateQeueu();
                CreateArrayPartsRPN();
                if (_arrayPartsRPN == null) return null;
                _sendList = new List<byte>();
                byte hlavicka;
                hlavicka = 1; //ide o pole RPN
                _sendList.Add(hlavicka);
                byte[] dlzka = BitConverter.GetBytes((int)_arrayPartsRPN.Length);
                for (int i = 0; i < dlzka.Length; i++)
                {
                    _sendList.Add(dlzka[i]);
                }

                for (int i = 0; i < _arrayPartsRPN.Length; i++)
                {
                    _sendList.Add(_arrayPartsRPN[i].identity);
                    if (_arrayPartsRPN[i].value != null)
                    {
                        for (int z = 0; z < _arrayPartsRPN[i].value.Length; z++)
                        {
                            _sendList.Add(_arrayPartsRPN[i].value[z]);
                        }
                    }
                }

                _RPNByteStream = _sendList.ToArray();
                return _RPNByteStream;
            }
        }

        public void ReadFromStream(byte[] inStream)
        {
            int positionByte = 0;

            if (inStream == null || inStream.Length < 5) return;
            if (inStream[0] != 1) return;

            byte[] lengthRPN = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                lengthRPN[i] = inStream[i + 1];
            }

            int dlzka = BitConverter.ToInt32(lengthRPN, 0);
            _arrayPartsRPN = new ConstantsRPN.PartRPN[dlzka];
            positionByte = 5;
            for (int i = 0; i < _arrayPartsRPN.Length; i++)
            {
                _arrayPartsRPN[i].identity = inStream[positionByte];
                positionByte++;
                if (_arrayPartsRPN[i].identity == ConstantsRPN.CONSTATNT_BOOL ||
                    _arrayPartsRPN[i].identity == ConstantsRPN.VALUE_BOOL)
                {
                    //nacitat 1
                    _arrayPartsRPN[i].value = new byte[1];
                    _arrayPartsRPN[i].value[0] = inStream[positionByte];
                    positionByte++;
                }
                else if (_arrayPartsRPN[i].identity <= ConstantsRPN.MAX_POINTER)
                {
                    //nacitat 4;
                    _arrayPartsRPN[i].value = new byte[4];
                    for (int z = 0; z < 4; z++)
                    {
                        _arrayPartsRPN[i].value[z] = inStream[positionByte];
                        positionByte++;
                    }
                }
            }
            //all is done.

        }

        private void CreateArrayPartsRPN()
        {
            if (_resultQueue == null || _resultQueue.Count == 0) return;

            Queue tmpQueue = (Queue)_resultQueue.Clone();
            ConstantsRPN.ExprPart actExprPart;
            _arrayPartsRPN = new ConstantsRPN.PartRPN[_resultQueue.Count];

            for (int i = 0; i < _arrayPartsRPN.Length; i++)
            {
                actExprPart = (ConstantsRPN.ExprPart)tmpQueue.Dequeue();
                _arrayPartsRPN[i].identity = actExprPart.exportType;
                _arrayPartsRPN[i].value = actExprPart.exportValue;
            }
        }

        public ConstantsRPN.PartRPN[] GetArrPartsRPN()
        {
            CreateArrayPartsRPN();
            return _arrayPartsRPN;
        }

        public void CreateTree(ConstantsRPN.ExprPart[] expression)
        {
            if (expression == null)
            {
                return;
            }
            if (expression.Length == 0)
            {
                return;
            }
            _rootTreeRPN = new Element();
            InsertIntoTree(_rootTreeRPN, expression);
        }

        private void CreateQeueu()
        {
            _resultQueue = new Queue();
            FillQueue(_rootTreeRPN);
        }

        public string StringRPN()
        {
            CreateQeueu();
            if (_resultQueue == null || _resultQueue.Count == 0)
                return string.Empty;
            string result = string.Empty;
            while (_resultQueue.Count != 0)
            {
                ConstantsRPN.ExprPart obj = (ConstantsRPN.ExprPart)_resultQueue.Dequeue();
                result += obj.name;
                if (_resultQueue.Count != 0)
                    result += "~";
            }
            return result;
        }

        public List<string> ListRPN()
        {
            CreateQeueu();
            if (_resultQueue == null || _resultQueue.Count == 0)
                return null;
            List<string> result = new List<string>();

            while (_resultQueue.Count != 0)
            {
                ConstantsRPN.ExprPart obj = (ConstantsRPN.ExprPart)_resultQueue.Dequeue();
                result.Add(obj.name);
            }
            return result;
        }

        private void FillQueue(Element actualElement)
        {
            if (actualElement.LeftElement != null)
                FillQueue(actualElement.LeftElement);
            if (actualElement.RightElement != null)
                FillQueue(actualElement.RightElement);
            _resultQueue.Enqueue(actualElement.Value);
        }

        private void InsertIntoTree(Element actualElement, ConstantsRPN.ExprPart[] expression)
        {
            if (expression == null || expression.Length == 0) return;
            if (expression.Length == 1)
            {
                actualElement.Value = expression[0];
                return;
            }

            int separatePoint;
            separatePoint = ReturnSeparationPosition(ref expression);
            if (separatePoint == -1) return;
            actualElement.Value = expression[separatePoint];

            if (separatePoint != 0)
            {
                Element TmpElmLeft = new Element();
                actualElement.LeftElement = TmpElmLeft;
                InsertIntoTree(TmpElmLeft, ReturnLeftSide(expression, separatePoint));
            }
            if (separatePoint != expression.Length)
            {
                Element TmpElmRight = new Element();
                actualElement.RightElement = TmpElmRight;
                InsertIntoTree(TmpElmRight, ReturnRightSide(expression, separatePoint));
            }
        }

        /// <summary>
        /// this methode returns the opertor position where will divide term
        /// </summary>
        /// <param name="arVyraz"></param>
        /// <returns></returns>
        private int ReturnSeparationPosition(ref ConstantsRPN.ExprPart[] expression)
        {
            int result;
            result = MinOperatorPosition(expression);
            if (result == -1)
            {
                if (!RemoveParentheses(ref expression)) return -1;
                result = MinOperatorPosition(expression);
            }
            return result;
        }

        /// <summary>
        /// in array find first operator with minimal priority
        /// </summary>
        /// <param name="arVyraz"></param>
        /// <returns></returns>
        private int MinOperatorPosition(ConstantsRPN.ExprPart[] expression)
        {
            if (expression == null && expression.Length == 0)
            {
                return -1;
            }
            int parenthesCount = 0;
            int position = -1;
            int priority = 300;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i].isOperator)
                {
                    if (expression[i].type == ConstantsRPN.TYPE_PARENTHESLEFT)
                    {
                        parenthesCount++;
                        continue;
                    }
                    if (expression[i].type == ConstantsRPN.TYPE_PARENTHESRIGHT)
                    {
                        parenthesCount--;
                        continue;
                    }

                    if (parenthesCount != 0) continue;
                    if (priority > expression[i].priority)
                    {
                        position = i;
                        priority = expression[i].priority;
                    }
                }
            }
            return position;
        }

        private bool RemoveParentheses(ref ConstantsRPN.ExprPart[] arrExpression)
        {
            if (arrExpression.Length == 0) return false;
            if ((arrExpression[0].type == 10) && (arrExpression[arrExpression.Length - 1].type == 11))
            {
                ConstantsRPN.ExprPart[] vtmp = new ConstantsRPN.ExprPart[arrExpression.Length - 2];
                for (int i = 0; i < vtmp.Length; i++)
                {
                    vtmp[i] = arrExpression[i + 1];
                }
                arrExpression = vtmp;
                return true;
            }
            return false;
        }

        private ConstantsRPN.ExprPart[] ReturnLeftSide(ConstantsRPN.ExprPart[] arrExpression, int positionCut)
        {
            int lengthNewArray = positionCut;
            if (lengthNewArray == 0) return null;
            ConstantsRPN.ExprPart[] vtmp = new ConstantsRPN.ExprPart[lengthNewArray];
            for (int i = 0; i < vtmp.Length; i++)
            {
                vtmp[i] = arrExpression[i];
            }
            return vtmp;
        }

        private ConstantsRPN.ExprPart[] ReturnRightSide(ConstantsRPN.ExprPart[] arrExpression, int positionCut)
        {
            int lengthNewArray = arrExpression.Length - positionCut - 1;
            if (lengthNewArray == 0) return null;
            ConstantsRPN.ExprPart[] vtmp = new ConstantsRPN.ExprPart[lengthNewArray];
            for (int i = 0; i < vtmp.Length; i++)
            {
                vtmp[i] = arrExpression[i + positionCut + 1];
            }
            return vtmp;
        }
    }
}
