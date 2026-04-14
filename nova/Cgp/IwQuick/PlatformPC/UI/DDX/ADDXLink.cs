using System;
using System.Windows.Forms;
using System.Threading;

namespace Contal.IwQuick.UI.DDX
{
    [Obsolete]
    public abstract class ADDXLink<T> : IDDXLink<T>
    {
        private delegate void DTranscript(ref T externalValue);

        protected ADDXLink(ref T externalValue, Control control)
        {
            Validator.CheckForNull(control,"control");

            Validator.CheckNull(control.Parent);

            _control = control;

            DTranscript transcriptThread2UI = TranscriptValue2UI;
            transcriptThread2UI.BeginInvoke(ref externalValue, null, null);

            DTranscript transcriptThread2Data = TranscriptValue2Data;
            transcriptThread2Data.BeginInvoke(ref externalValue, null, null);

            
           
        }

        protected T _temporaryValue;

        #region IDDXLink<T> Members

        private readonly Control _control;
        public Control Control
        {
            get { return _control; }
        }

        private readonly ManualResetEvent _transcriptMutex2Data = new ManualResetEvent(false);
        private readonly ManualResetEvent _transcriptMutex2UI = new ManualResetEvent(false);
        private void TranscriptValue2Data(ref T externalValue)
        {
            while (true)
            {
                try
                {
                    _transcriptMutex2Data.WaitOne();
                    if (!externalValue.Equals(_temporaryValue))
                        externalValue = _temporaryValue;
                    _transcriptMutex2Data.Reset();
                }
                catch
                {
                    break;
                }
            }
        }

        private void TranscriptValue2UI(ref T externalValue)
        {
            _temporaryValue = externalValue;
            while (true)
            {
                try
                {
                    _transcriptMutex2UI.WaitOne();
                    _temporaryValue = externalValue;
                    _transcriptMutex2UI.Reset();
                }
                catch
                {
                    break;
                }
            }
        }

        public void Data2UI()
        {
            _transcriptMutex2UI.Set();

            DSetData2UI d = SetData2UI;
            d.Invoke(_temporaryValue,_control);

            
        }

        public void UI2Data()
        {
            DSetUI2Data d = SetUI2Data;

            if (d.Invoke(_control, ref _temporaryValue))
            {
                _transcriptMutex2Data.Set();
            }
        }

        private delegate void DSetData2UI(T data, Control control);
        protected abstract void SetData2UI(T data,Control control);

        private delegate bool DSetUI2Data(Control control, ref T data);
        protected abstract bool SetUI2Data(Control control, ref T data);


        #endregion
    }
}
