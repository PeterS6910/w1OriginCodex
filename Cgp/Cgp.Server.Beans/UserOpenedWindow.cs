using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class UserOpenedWindow : AOrmObject
    {
        public const string COLUMNIDUSEROPENEDWINDOW = "IdUserOpenedWindow";
        public const string COLUMNWINDOWINDEX = "WindowIndex";
        public const string COLUMNMODUL = "Modul";
        public const string COLUMNFORM = "FormName";
        public const string COLUMNGUID = "ObjectId";
        public const string COLUMNTOP = "Top";
        public const string COLUMNLEF = "Left";
        public const string COLUMNWIDTH = "Width";
        public const string COLUMNHEIGHT = "Height";
        public const string COLUMNDOCKED = "Docked";
        public const string COLUMNSELECTED = "Selected";
        public const string COLUMNMONITOR = "Monitor";
        public const string COLUMNSHOWOPTION = "ShowOption";
        public const string COLUMNBORDERSTYLE = "BorderStyle";
        public const string COLUMNLOGIN = "Login";
        public const string COLUMNALLOWEDIT = "AllowEdit";
        public const string COLUMNHASPARENT = "HasParent";


        public virtual Guid IdUserOpenedWindow { get; set; }
        public virtual int WindowIndex { get; set; }
        public virtual string Modul { get; set; }
        public virtual string FormName { get; set; }
        public virtual string ObjectId { get; set; }
        public virtual int PosTop { get; set; }
        public virtual int PosLeft { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual int Docked { get; set; }
        public virtual bool Selected { get; set; }
        public virtual int Monitor { get; set; }
        public virtual int ShowOption { get; set; }
        public virtual int BorderStyle { get; set; }
        public virtual Login Login { get; set; }
        public virtual bool? AllowEdit { get; set; }
        public virtual bool HasParent { get; set; }
        

        public UserOpenedWindow()
        {
        }

        public UserOpenedWindow(string username, int windowIndex, string modul, string formName, string objectId, int top, int left, int width, int height, int docked, bool selected, 
            int monitor, int showOption, int borderStyle, Login login, bool? allowEdit, bool hasParent)
        {
            WindowIndex = windowIndex;
            Modul = modul;
            FormName = formName;
            ObjectId = objectId;
            PosTop = top;
            PosLeft = left;
            Width = width;
            Height = height;
            Docked = docked;
            Selected = selected;
            Monitor = monitor;
            ShowOption = showOption;
            BorderStyle = borderStyle;
            Login = login;
            AllowEdit = allowEdit;
            HasParent = hasParent;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is UserOpenedWindow)
            {
                return (obj as UserOpenedWindow).IdUserOpenedWindow == this.IdUserOpenedWindow;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdUserOpenedWindow.ToString();
        }

        public override object GetId()
        {
            return IdUserOpenedWindow;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.NotSupport;
        }
    }
}
