using System;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public class NCASSupport
    {
        static public ObjectType GetObjectType(object obj)
        {
            if (obj.GetType() == typeof(DailyPlan))
            {
                return ObjectType.DailyPlan;
            }
            if (obj.GetType() == typeof(TimeZone))
            {
                return ObjectType.TimeZone;
            }
            if (obj.GetType() == typeof(Calendar))
            {
                return ObjectType.Calendar;
            }
            if (obj.GetType() == typeof(CalendarDateSetting))
            {
                return ObjectType.CalendarDateSetting;
            }
            if (obj.GetType() == typeof(DayInterval))
            {
                return ObjectType.DayInterval;
            }
            if (obj.GetType() == typeof(TimeZoneDateSetting))
            {
                return ObjectType.TimeZoneDateSetting;
            }
            if (obj.GetType() == typeof(DayType))
            {
                return ObjectType.DayType;
            }
            if (obj.GetType() == typeof(ACLPerson))
            {
                return ObjectType.ACLPerson;
            }
            if (obj.GetType() == typeof(ACLSetting))
            {
                return ObjectType.ACLSetting;
            }
            if (obj.GetType() == typeof(AccessControlList))
            {
                return ObjectType.AccessControlList;
            }
            if (obj.GetType() == typeof(Card))
            {
                return ObjectType.Card;
            }
            if (obj.GetType() == typeof(CardSystem))
            {
                return ObjectType.CardSystem;
            }
            if (obj.GetType() == typeof(AlarmArea))
            {
                return ObjectType.AlarmArea;
            }
            if (obj.GetType() == typeof(AACardReader))
            {
                return ObjectType.AACardReader;
            }
            if (obj.GetType() == typeof(AccessZone))
            {
                return ObjectType.AccessZone;
            }
            if (obj.GetType() == typeof(Input))
            {
                return ObjectType.Input;
            }
            if (obj.GetType() == typeof(Output))
            {
                return ObjectType.Output;
            }
            if (obj.GetType() == typeof(CCU))
            {
                return ObjectType.CCU;
            }

            return ObjectType.NotSupport;
        }
    }

    public class DelayTextEdit
    {
        int _minutes;
        int _second;
        int _miliSec;
        TextBox ownTextBox;

        public void AssimilateTextBox(TextBox textBox)
        {
            ownTextBox = textBox;
            ownTextBox.KeyDown += ownTextBox_KeyDown;
            ownTextBox.KeyPress += ownTextBox_KeyPress;
            ownTextBox.MouseClick += ownTextBox_MouseClick;
            ownTextBox.Enter += ownTextBox_Enter;
            WriteTextBoxTimeString();
        }

        void ownTextBox_Enter(object sender, EventArgs e)
        {
            DoSelection();
        }

        void ownTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            DoSelection();
        }

        void ownTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        void ownTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab) return;

            e.Handled = true;
            int xPos = ownTextBox.SelectionStart;

            if (e.KeyCode == Keys.Right)
            {
                if (xPos < 5)
                    ownTextBox.SelectionStart = 7;
                else
                    ownTextBox.SelectionStart = 11;
                DoSelection();
                return;
            }

            if (e.KeyCode == Keys.Left)
            {
                if (xPos < 8)
                    ownTextBox.SelectionStart = 4;
                else
                    ownTextBox.SelectionStart = 7;
                DoSelection();
                return;
            }

            int position;
            if (xPos < 5)
                position = 0;
            else if (xPos < 8)
                position = 1;
            else
                position = 2;

            if (e.KeyCode == Keys.Up)
            {
                ChangeUp(position);
                DoSelection();
                return;
            }

            if (e.KeyCode == Keys.Down)
            {
                ChangeDown(position);
                DoSelection();
                return;
            }


            if (e.KeyValue < 48 ||
                e.KeyValue > 105 ||
                (e.KeyValue > 57 && e.KeyValue < 96))
            {
                return;
            }

            int value;
            if (e.KeyValue > 90)
                value = e.KeyValue - 96;
            else
                value = e.KeyValue - 48;

            ChangeText(position, value);
            DoSelection();
        }

        private void ChangeText(int position, int value)
        {
            if (position == 0)
                CalculateMinutes(value);
            else if (position == 1)
                CalculateSeconds(value);
            else
                CalculateMiliSeconds(value);
        }

        private void ChangeUp(int position)
        {
            if (position == 0)
            {
                if (_minutes == 1439)
                    _minutes = 0;
                else
                    _minutes++;
            }
            else if (position == 1)
            {
                if (_second == 59)
                    _second = 0;
                else
                    _second++;
            }
            else
            {
                if (_miliSec == 999)
                    _miliSec = 0;
                else
                    _miliSec++;
            }
        }

        private void ChangeDown(int position)
        {
            if (position == 0)
            {
                if (_minutes == 0)
                    _minutes = 1439;
                else
                    _minutes--;
            }
            else if (position == 1)
            {
                if (_second == 0)
                    _second = 59;
                else
                    _second--;
            }
            else
            {
                if (_miliSec == 0)
                    _miliSec = 999;
                else
                    _miliSec--;
            }
        }

        private void WriteTextBoxTimeString()
        {
            if (ownTextBox != null)
            {
                string ret = string.Empty;
                ret += String.Format("{0:0000}", _minutes);
                ret += ":";
                ret += String.Format("{0:00}", _second);
                ret += ":";
                ret += String.Format("{0:000}", _miliSec);
                ownTextBox.Text = ret;
            }
        }

        public int GetMiliSeconds()
        {
            int totalMiliSecond;
            totalMiliSecond = _miliSec;
            totalMiliSecond += _second * 1000;
            totalMiliSecond += _minutes * 60000;
            return totalMiliSecond;
        }

        public void SetMiliSeconds(int inputMiliSec)
        {
            int tmpMS;
            _minutes = inputMiliSec / 60000;
            tmpMS = inputMiliSec % 60000;
            _second = tmpMS / 1000;
            _miliSec = tmpMS % 1000;

            WriteTextBoxTimeString();
        }

        private void CalculateMinutes(int value)
        {
            if (_minutes == 0)
            {
                _minutes = value;
                return;
            }
            if (_minutes == 1439)
            {
                _minutes = value;
                return;
            }
            _minutes = _minutes * 10 + value;
            if (_minutes > 1439)
            {
                _minutes = 1439;
            }
        }

        private void CalculateSeconds(int value)
        {
            if (_second == 0)
            {
                _second = value;
                return;
            }
            if (_second < 6)
            {
                _second = _second * 10 + value;
            }
            else
            {
                _second = value;
            }
        }

        private void CalculateMiliSeconds(int value)
        {
            if (_miliSec > 100)
            {
                _miliSec = value;
            }
            else
            {
                _miliSec = _miliSec * 10 + value;
            }
        }

        private void DoSelection()
        {
            if (ownTextBox != null)
            {
                int xPos = ownTextBox.SelectionStart;
                WriteTextBoxTimeString();
                if (xPos < 5)
                {
                    ownTextBox.SelectionStart = 0;
                    ownTextBox.SelectionLength = 4;
                }
                else if (xPos < 8)
                {
                    ownTextBox.SelectionStart = 5;
                    ownTextBox.SelectionLength = 2;
                }
                else
                {
                    ownTextBox.SelectionStart = 8;
                    ownTextBox.SelectionLength = 3;
                }
            }
        }
    }

    public class DictionaryExpressionType
    {
        Guid _id;
        string _type;
        byte _valueType;
        ObjectType _objectType;

        public Guid Id
        {
            get { return _id; }
        }
        public string Type
        {
            get { return _type; }
        }
        public byte ValueType
        {
            get { return _valueType; }
        }
        public ObjectType ObjectType
        {
            get { return _objectType; }
        }

        public DictionaryExpressionType(Guid id, string type, byte valueType, ObjectType objectType)
        {
            _id = id;
            _type = type;
            _valueType = valueType;
            _objectType = objectType;
        }
    }

    [LwSerialize(250)]
    public enum RpnExtensions : byte
    {
        On = 0,
        Off = 1,
        Tamper = 2,
        Unset = 3,
        Set = 4,
        Alarm = 5,
        Prewarning = 6,
        BuyTime = 7,
        TemporaryUnsetEntry = 8,
        TemporaryUnsetExit = 9,
        RealOn = 10,
        RealOff = 11,
        NotSupported = 200
    }
 

    public class ExpressionExtensions
    {
        public string GetAllExtensions(byte dctType)
        {
            if (dctType == 0)
                return "On Off";
            if (dctType == 1)
                return "On Tamper Off";
            if (dctType == 2)
                return "Unset Set Alarm Prewarning BuyTime TemporaryUnsetEntry TemporaryUnsetExit";
            if (dctType == 5)
                return "On Off RealOn RealOff";

            return string.Empty;
        }

        public bool IsExtensionsOk(byte dctType, string usedExrension)
        {
            if (dctType == 0)
            {
                if (usedExrension == "On")
                    return true;
                if (usedExrension == "Off")
                    return true;
                if (usedExrension == "")
                    return true;
            }
            else if (dctType == 1)
            {
                if (usedExrension == "On")
                    return true;
                if (usedExrension == "Off")
                    return true;
                if (usedExrension == "Tamper")
                    return true;
                if (usedExrension == "")
                    return true;
            }
            else if (dctType == 2)
            {
                if (usedExrension == "Unset")
                    return true;
                if (usedExrension == "Set")
                    return true;
                if (usedExrension == "Alarm")
                    return true;
                if (usedExrension == "Prewarning")
                    return true;
                if (usedExrension == "BuyTime")
                    return true;
                if (usedExrension == "TemporaryUnsetEntry")
                    return true;
                if (usedExrension == "TemporaryUnsetExit")
                    return true;
            }
            else if (dctType == 5)
            {
                if (usedExrension == "On")
                    return true;
                if (usedExrension == "Off")
                    return true;
                if (usedExrension == "RealOff")
                    return true;
                if (usedExrension == "RealOn")
                    return true;
                if (usedExrension == "")
                    return true;
            }
            return false;
        }

        public bool IsStringExtensions(string usedExrension)
        {
            //foreach (string str in Enum.GetNames(typeof(Extension)))
            //{
            //    if (str.ToLower() == pokusString)
            //    {
            //    }
            //}

            if (usedExrension == "On")
                return true;
            if (usedExrension == "Off")
                return true;
            if (usedExrension == "Tamper")
                return true;
            if (usedExrension == "")
                return true;
            if (usedExrension == "Unset")
                return true;
            if (usedExrension == "Set")
                return true;
            if (usedExrension == "Alarm")
                return true;
            if (usedExrension == "Prewarning")
                return true;
            if (usedExrension == "BuyTime")
                return true;
            if (usedExrension == "TemporaryUnsetEntry")
                return true;
            if (usedExrension == "TemporaryUnsetExit")
                return true;
            if (usedExrension == "RealOn")
                return true;
            if (usedExrension == "RealOff")
                return true;
            return false;
        }

        public byte GetRpnExtensionByte(string extension)
        {
            if (extension == null || extension == string.Empty)
            {
                return (byte)RpnExtensions.On;
            }

            foreach (byte val in Enum.GetValues(typeof(RpnExtensions)))
            {
                if (Enum.GetName(typeof(RpnExtensions), val) == extension)
                {
                    return val;
                }
            }
            return (byte)RpnExtensions.NotSupported;
        }


        //public class Docasu<T> where T : AOrmObject
        //{
        //    T _result;
        //    TextBox _editName;
        //    Button _bSet;
        //    Button _bCreate;
        //    Button _bDelete;
        //    Contal.Cgp.Client.ACgpTableForm<T> _mainTableForm;

        //    public Docasu(T result, TextBox editName, Button bSet, Button bCreate, Button bDelete, Contal.Cgp.Client.ACgpTableForm<T> tableForm)
        //    {
        //        _result = result;
        //        _editName = editName;
        //        _bSet = bSet; ;
        //        _bCreate = bCreate;
        //        _bDelete = bDelete;
        //        Contal.Cgp.Client.ACgpTableForm<T> _mainTableForm = tableForm;

        //        _editName.DoubleClick += new EventHandler(_editName_DoubleClick);
        //        _editName.DragDrop += new DragEventHandler(_editName_DragDrop);
        //        _editName.DragOver += new DragEventHandler(_editName_DragOver);

        //        _bSet.Click += new EventHandler(_bSet_Click);
        //        _bCreate.Click += new EventHandler(_bCreate_Click);
        //        _bDelete.Click += new EventHandler(_bDelete_Click);

        //    }

        //    void _bSet_Click(object sender, EventArgs e)
        //    {
        //        if (Contal.Cgp.Client.CgpClient.Singleton.IsConnectionLost(true)) return;

        //        try
        //        {
        //            List<object> listPG = new List<object>();

        //            Exception error;
        //            ICollection<T> listPGFromDatabase =
        //                Contal.Cgp.Client.CgpClient.Singleton.MainServerProvider.PresentationGroups.List(out error);
        //            if (error != null)
        //                throw error;

        //            foreach (T presentationGroup in listPGFromDatabase)
        //            {
        //                listPG.Add(presentationGroup);
        //            }

        //            Contal.Cgp.Client.ListboxFormAdd formAdd = new Contal.Cgp.Client.ListboxFormAdd(listPG, Contal.Cgp.Client.CgpClient.Singleton.LocalizationHelper.GetString("PresentationGroupsFormPresentationGroupsForm"));
        //            object outPresentationGroup;
        //            formAdd.ShowDialog(out outPresentationGroup);
        //            if (outPresentationGroup != null)
        //            {
        //                T presetationGroup = outPresentationGroup as T;
        //                SetResult(presetationGroup);
        //                Contal.Cgp.Client.CgpClientMainForm.Singleton.AddToRecentList(_result);
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    void _bCreate_Click(object sender, EventArgs e)
        //    {
        //        T presetationGroup = new T();
        //        if (_mainTableForm.Singleton.OpenInsertDialg(ref presetationGroup))
        //        {
        //            SetResult(presetationGroup);
        //        }
        //    }

        //    void _bDelete_Click(object sender, EventArgs e)
        //    {
        //        SetResult(null);
        //    }

        //    void _editName_DoubleClick(object sender, EventArgs e)
        //    {
        //        if (_result != null)
        //        {
        //            _mainTableForm.Singleton.OpenEditForm(_result);
        //        }
        //    }

        //    void _editName_DragDrop(object sender, DragEventArgs e)
        //    {
        //        try
        //        {
        //            string[] output = e.Data.GetFormats();
        //            if (output == null) return;
        //            AddResult((object)e.Data.GetData(output[0]));
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    void _editName_DragOver(object sender, DragEventArgs e)
        //    {
        //        e.Effect = DragDropEffects.All;
        //    }

        //    private void SetResult(T workResult)
        //    {
        //        _result = workResult;
        //        if (_result == null)
        //        {
        //            _editName.Text = string.Empty;
        //        }
        //        else
        //        {
        //            _editName.Text = _result.ToString();
        //        }
        //    }

        //    private void AddResult(object newResult)
        //    {
        //        try
        //        {
        //            if (newResult.GetType() == typeof(T))
        //            {
        //                SetResult((T)newResult);
        //            }
        //            else
        //            {
        //                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _editName,
        //                   GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
        //            }
        //        }
        //        catch
        //        { }
        //    }
        //}
    }
}
