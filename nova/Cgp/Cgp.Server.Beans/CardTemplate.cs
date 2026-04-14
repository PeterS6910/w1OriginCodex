using System;
using System.Text;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using System.Collections;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    [LwSerialize(204)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class CardTemplate : AOrmObject
    {
        public const string COLUMNID = "Id";
        public const string COLUMNNAME = "Name";
        public const string COLUMNTEMPLATEDATA = "TenplateData";

        [LwSerialize]
        public virtual Guid Id { get; set; }

        [LwSerialize]
        public virtual string Name { get; set; }

        [LwSerialize]
        public virtual byte[] TemplateData { get; set; }

        public CardTemplate()
        {
            Name = string.Empty;
            TemplateData = null;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CardTemplate)
            {
                return (obj as CardTemplate).Id.Equals(Id);
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return Id.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.CardTemplate;
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public virtual string GetDataAsString()
        {
            if (TemplateData == null || TemplateData.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(TemplateData);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class CardTemplateShort : IComparer, IShortObject
    {
        public const string COLUMNID = "Id";
        public const string COLUMNNAME = "Name";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Image Symbol { get; set; }

        public CardTemplateShort()
        {
        }

        public CardTemplateShort(CardTemplate cardTemplate)
        {
            Id = cardTemplate.Id;
            Name = cardTemplate.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CardTemplateShort))
                return false;

            return (Id.Equals((obj as CardTemplateShort).Id));
        }

        #region IComparer Members

        public virtual int Compare(object x, object y)
        {
            CardTemplateShort tX = x as CardTemplateShort;
            CardTemplateShort tY = y as CardTemplateShort;

            if (tX == null && tY == null)
                return 0;

            if (tX != null && tY == null)
                return 1;

            if (tX == null && tY != null)
                return -1;

            if (tX.Name == null && tY.Name == null)
                return 0;

            if (tX.Name != null && tY.Name == null)
                return 1;

            if (tX.Name == null && tY.Name != null)
                return -1;

            return (tX.Name.CompareTo(tY.Name));
        }

        #endregion

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.CardTemplate; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        object IShortObject.Id { get { return Id; } }

        #endregion
    }
}
