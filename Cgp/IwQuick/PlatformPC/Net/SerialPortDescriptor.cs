using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Net
{
    public class SerialPortDescriptor
    {
        private string _caption;
        public string Caption
        {
            get { return _caption; }
        }

        private string _portName;
        public string PortName
        {
            get { return _portName; }
        }

        private string ExtractPortName(string caption)
        {
            string portName = caption.Trim();
            int start = portName.IndexOf("(COM");
            if (start < 0)
                return portName;
            else
            {
                portName = portName.Substring(start + 1);
                int end = portName.IndexOf(')');
                if (end >= 0)
                {
                    portName = portName.Substring(0, end);
                }

                return portName;
            }
        }

        protected internal SerialPortDescriptor(string wmiCaption)
        {
            Validator.CheckNullString(wmiCaption);

            _caption = wmiCaption;
            _portName = ExtractPortName(wmiCaption);
        }

        public override string ToString()
        {
            return _caption;
        }
    }
}
