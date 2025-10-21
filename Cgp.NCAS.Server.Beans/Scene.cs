using System;
using System.Drawing;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class Scene : AOrmObject
    {
        public const string COLUMN_ID = "IdScene";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";

        public virtual Guid IdScene { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual byte[] RowDataBackground { get; set; }
        public virtual SymbolDataType BackgroundDataType { get; set; }
        public virtual byte[] RowData { get; set; }
        public virtual byte[] SceneScreenRowData { get; set; }
        public virtual byte ObjectType { get; set; }

        public Scene()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.Scene;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Scene)
            {
                return (obj as Scene).IdScene == IdScene;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdScene.ToString();
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Scene;
        }

        public override object GetId()
        {
            return IdScene;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }

    [Serializable()]
    public class SceneShort : IShortObject
    {
        public const string COLUMN_ID = "IdScene";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SCENESCREEN = "SceneScreen";

        public Guid IdScene { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] SceneScreenRowData { get; set; }
        public Image SceneScreen { get; set; }

        public SceneShort(Scene scene)
        {
            IdScene = scene.IdScene;
            Name = scene.Name;
            Description = scene.Description;
            SceneScreenRowData = scene.SceneScreenRowData;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.Scene; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdScene; } }

        #endregion
    }

    [Serializable]
    public class SceneModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Scene; } }

        public SceneModifyObj(Scene scene)
        {
            Id = scene.IdScene;
            FullName = scene.Name;
            Description = scene.Description;
        }
    }
}
