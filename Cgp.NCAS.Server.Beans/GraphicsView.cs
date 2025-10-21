using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class GraphicsView : AOrmObject
    {
        public const string COLUMN_ID = "IdGraphicsView";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";

        public virtual Guid IdGraphicsView { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual byte[] RowData { get; set; }
        //public virtual byte[] GraphicsViewScreenRowData { get; set; }
        public virtual byte ObjectType { get; set; }

        public GraphicsView()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.GraphicsView;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GraphicsView)
            {
                return (obj as GraphicsView).IdGraphicsView == IdGraphicsView;
            }
            
            return false;
        }

        public override string GetIdString()
        {
            return IdGraphicsView.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.GraphicsView;
        }

        public override object GetId()
        {
            return IdGraphicsView;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }

    [Serializable()]
    public class GraphicsViewShort : IShortObject
    {
        public const string COLUMN_ID = "IdGraphicsView";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        //public const string COLUMN_SCENESCREEN = "GraphicsViewScreen";

        public Guid IdGraphicsView { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public byte[] GraphicsViewScreenRowData { get; set; }
        //public Image GraphicsViewScreen { get; set; }

        public GraphicsViewShort(GraphicsView view)
        {
            IdGraphicsView = view.IdGraphicsView;
            Name = view.Name;
            Description = view.Description;
            //GraphicsViewScreenRowData = view.GraphicsViewScreenRowData;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.GraphicsView; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdGraphicsView; } }

        #endregion
    }

    [Serializable]
    public class ViewModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.GraphicsView; } }

        public ViewModifyObj(GraphicsView view)
        {
            Id = view.IdGraphicsView;
            FullName = view.Name;
            Description = view.Description;
        }
    }
}
