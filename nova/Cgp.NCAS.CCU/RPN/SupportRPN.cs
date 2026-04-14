using System;

using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.RPN
{
    class SupportRPN
    {
    }

    class StackArray
    {
        ConstantsRPN.PartRPN[] pole;
        int actPossition;

        public StackArray(int count)
        {
            pole = new ConstantsRPN.PartRPN[count];
            actPossition = -1;
        }

        public void Reset()
        {
            actPossition = -1;
        }

        public int Count
        {
            get { return (actPossition + 1); }
        }

        public void Insert(ConstantsRPN.PartRPN newItem)
        {
            actPossition++;
            pole[actPossition] = newItem;
        }

        public ConstantsRPN.PartRPN Return()
        {
            ConstantsRPN.PartRPN exit;
            exit = pole[actPossition];
            actPossition--;
            return exit;
        }
    }

    public class TableOfVariables
    {
        Dictionary<byte, StoreTableObject> _table = new Dictionary<byte,StoreTableObject>();

        public void Insert(byte name, StoreTableObject newVariable)
        {
            _table.Add(name, newVariable);
        }

        public bool GetVariableStatus(byte[] name)
        {
            if (name != null && name.Length < 1)
            {
                throw new Exception("No valid name");
            }

            byte lookFor = name[0];
            if (_table.ContainsKey(lookFor))
            {
                return _table[lookFor].Status;
            }
            else
            {
                throw new Exception("Variable not in dictionary");
            }
        }

        public void RecreateTableOfVariables(string inputVarString)
        {
            //_table.Clear();
            string[] parts = inputVarString.Split('~');
            foreach (string part in parts)
            {
                if (part != string.Empty)
                {
                    if (!IsInDictionary(part))
                    {
                        _table.Add((byte)_table.Count, new StoreTableObject(part));
                    }
                }
            }
        }

        private bool IsInDictionary(string fullName)
        {
            ; foreach (KeyValuePair<byte, StoreTableObject> kvp in _table)
            {
                if (kvp.Value.DatabaseName == fullName)
                {
                    return true;
                }
            }
            return false;
        }

        public byte[] GetNameFromTableOfVariables(string name)
        {
            foreach (KeyValuePair<byte, StoreTableObject> kvp in _table)
            {
                if (kvp.Value.DatabaseName == name)
                {
                    byte[] result = new byte[1];
                    result[0] = kvp.Key;
                    return result;
                }
            }
            throw new Exception("part not in dictionary");
        }
    }

    public class StoreTableObject
    {
        //if object is in wanted state
        bool _status = false;
        //state declared in RPN
        State _trueStatus = 0;
        //database name
        string _fullName;

        public bool Status
        {
            get { return _status; }
        }
        public string DatabaseName
        {
            get { return _fullName; }
        }

        public StoreTableObject(string name)
        {
            //ObjectType$Guid$State byte.ToString()
            string[] lowRoot = name.Split('$');
            if (lowRoot.Length == 3)
            {
                _fullName = name;
                var objectState = GetObjectState(lowRoot[0], lowRoot[1]);

                if (objectState == State.Unknown)
                    return;

                _trueStatus = GetStatusFromByteString(lowRoot[2]);

                    //_dbsObj.StateChanged += new Contal.IwQuick.Action<State>(DdbsObjStatusChanged);
                if (objectState == _trueStatus)
                    _status = true;
            }
        }

        private static State GetObjectState(string dbObjectType, string guid)
        {
            byte objectType;
            Guid objGuid = new Guid(guid);
            try
            {
                objectType = Byte.Parse(dbObjectType);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return State.Unknown;
            }

            switch (objectType)
            {
                case (byte)ObjectType.DailyPlan:
                    return StateObjects.GetObjectState(ObjectType.DailyPlan, objGuid);
                case (byte)ObjectType.TimeZone:
                    return StateObjects.GetObjectState(ObjectType.TimeZone, objGuid);
                case (byte)ObjectType.Input:
                    return StateObjects.GetObjectState(ObjectType.Input, objGuid);
                case (byte)ObjectType.Output:
                    return StateObjects.GetObjectState(ObjectType.Output, objGuid);
            }
            return State.Unknown;
        }

        private State GetStatusFromByteString(string name)
        {
            return State.On;
        }

        public State GetEnumFromString(string usedExrension)
        {
            switch (usedExrension)
            {
                case "On":
                    return State.On;
                case "Off":
                    return State.Off;
                case "":
                    return State.On;
                case "Tamper":
                    return State.Tamper;
                case "Unset":
                    return State.Unset;
                case "Set":
                    return State.Set;
                case "Alarm":
                    return State.Alarm;
                case "Prewarning":
                    return State.Prewarning;
                case "BuyTime":
                    return State.BuyTime;
                case "TemporaryUnsetEntry":
                    return State.TemporaryUnsetEntry;
                case "TemporaryUnsetExit":
                    return State.TemporaryUnsetExit;
            }
            return State.On;
        }
    }
}
