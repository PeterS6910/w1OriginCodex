using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class PresentationFormatter : AOrmObject
    {
        public const string COLUMNIDFORMATER = "IdFormatter";
        public const string COLUMNFORMATTERNAME = "FormatterName";
        public const string COLUMNMESSAGEFORMAT = "MessageFormat";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOBJECTTYPE = "ObjectType";

        public virtual Guid IdFormatter { get; set; }
        public virtual string FormatterName { get; set; }
        public virtual string MessageFormat { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public PresentationFormatter()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.PresentationFormatter;
        }

        public override string ToString()
        {
            return FormatterName;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is PresentationFormatter)
            {
                return (obj as PresentationFormatter).IdFormatter == IdFormatter;
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.FormatterName.ToString().ToLower().Contains(expression)) return true;
            if (this.Description != null)
            {
                if (this.Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdFormatter.ToString();
        }

        public override object GetId()
        {
            return IdFormatter;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new PresentationFormatterModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.PresentationFormatter;
        }
    }

    [Serializable()]
    public class PresentationFormatterModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.PresentationFormatter; } }

        public PresentationFormatterModifyObj(PresentationFormatter presentationFormatter)
        {
            Id = presentationFormatter.IdFormatter;
            FullName = presentationFormatter.FormatterName;
            Description = presentationFormatter.Description;
        }
    }
}