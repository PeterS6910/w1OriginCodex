using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DBCreate.plugin
{
    class Writer
    {
        private int _id;
        public virtual int WriterId
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

        private IList _employees;
        public virtual IList Books
        {
            get
            {
                return _employees == null ? new List<Book>() : _employees;
            }
            set
            {
                _employees = value;
            }
        }

        public Writer()
        {
            _employees = new List<Book>();
        }
    }
}
