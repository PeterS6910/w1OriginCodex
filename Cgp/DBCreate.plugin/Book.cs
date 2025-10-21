using System;
using System.Collections.Generic;
using System.Text;

namespace DBCreate.plugin
{
    public class Book
    {
        private int _bookId;
        public virtual int BookId
        {
            get { return _bookId; }
            set { _bookId = value; }
        }

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _writer;
        public virtual int WriterId
        {
            get { return _writer; }
            set { _writer = value; }
        }



    }
}