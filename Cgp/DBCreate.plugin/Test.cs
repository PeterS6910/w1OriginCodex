using System;
using System.Collections.Generic;
using System.Text;

namespace DBCreate.plugin
{
    class Test
    {
        private int _id;
        public virtual int id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public virtual string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
