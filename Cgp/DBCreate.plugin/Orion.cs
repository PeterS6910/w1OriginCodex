using System;
using System.Collections.Generic;
using System.Text;

namespace DBCreate.plugin
{
    public class Orion
    {
        private int _id;
        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _start;
        public virtual int start
        {
            get { return _start; }
            set { _start = value; }
        }
    }
}
