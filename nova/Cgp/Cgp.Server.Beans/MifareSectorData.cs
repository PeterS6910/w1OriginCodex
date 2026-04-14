using System;
using System.Collections.Generic;

using Contal.LwSerialization;
using Contal.Cgp.Globals;
using System.ComponentModel;

using Contal.IwQuick.Data;
using Contal.IwQuick.Crypto;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    [LwSerialize(208)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class MifareSectorData : AOrmObject, INotifyPropertyChanged
    {
        public const string COLUMN_ID_MIFARE_SECTOR_DATA = "Id";
        public const string COLUMN_AID = "Aid";
        public const string COLUMN_GENERAL_A_KEY = "GeneralAKey";
        public const string COLUMN_GENERAL_B_KEY = "GeneralBKey";
        public const string COLUMN_ENCODING = "Encoding";
        public const string COLUMN_CYPHER_DATA = "CypherData";
        public const string COLUMN_SIZE_KB = "SizeKB";
        public const string COLUMN_SECTORS_INFO = "SectorsInfo";

        public virtual Guid Id { get; set; }
        private string _aid;
        public virtual string Aid
        {
            get
            {
                return _aid;
            }
            set
            {
                if (_aid == value)
                    return;

                _aid = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Aid"));
            }
        }
        [field: NonSerialized]
        private XTEA _xtea = null;

        public virtual string GeneralAKeyString
        {
            get
            {
                if (GeneralAKey == null)
                    return string.Empty;
                else
                    return "Value encoded!";
            }
            set
            {
                if (value == "Value encoded")
                    return;

                if (value == null || value == string.Empty)
                    GeneralAKey = null;
                else
                {
                    byte[] bytes = null;
                    int discarded = 0;
                    bytes = HexEncoding.GetBytes(value, out discarded);
                    if (discarded == 0)
                    {
                        EnsureInitialisedXTEA();
                        GeneralAKey = _xtea.XTeaFrameEnc(bytes);
                    }
                    else
                        GeneralAKey = null;
                }
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GeneralAKeyString"));
            }
        }
        public virtual byte[] GeneralAKey { get; set; }

        public virtual string GeneralBKeyString
        {
            get
            {
                if (GeneralBKey == null)
                    return string.Empty;
                else
                    return "Value encoded!";
            }
            set
            {
                if (value == "Value encoded")
                    return;

                if (value == null || value == string.Empty)
                    GeneralBKey = null;
                else
                {
                    byte[] bytes = null;
                    int discarded = 0;
                    bytes = HexEncoding.GetBytes(value, out discarded);
                    if (discarded == 0)
                    {
                        EnsureInitialisedXTEA();
                        GeneralBKey = _xtea.XTeaFrameEnc(bytes);
                    }
                    else
                        GeneralBKey = null;
                }

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GeneralBKeyString"));
            }
        }
        public virtual byte[] GeneralBKey { get; set; }

        private void EnsureInitialisedXTEA()
        {
            if (_xtea == null)
            {
                _xtea = new XTEA();
                _xtea.XTeaInit();
            }
        }

        private byte? _encoding = 0;
        public virtual byte? Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                if (_encoding == value)
                    return;

                _encoding = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Encoding"));
            }
        }
        public virtual byte[] CypherData { get; set; }
        private byte _sizeKB = 0;
        public virtual byte SizeKB
        {
            get
            {
                return _sizeKB;
            }
            set
            {
                if (_sizeKB == value)
                    return;

                _sizeKB = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SizeKB"));
            }
        }
        public virtual ICollection<MifareSectorSectorInfo> SectorsInfo { get; set; }

        public override bool Compare(object obj)
        {
            if (!(obj is MifareSectorData))
                return false;

            return ((obj as MifareSectorData).Id == this.Id);
        }

        public override string GetIdString()
        {
            return Id.ToString();
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return ObjectType.MifareSectorData;
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        #region INotifyPropertyChanged Members

        [field: NonSerializedAttribute()]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Creates the bytes to be sent to CCU
        /// </summary>
        /// <returns>result bytes</returns>
        public virtual byte[] GetSmartCardDataForCCU()
        {
            List<byte> result = new List<byte>();
            result.Add(_encoding.Value);
            result.AddRange(CypherData);

            return result.ToArray();
        }
    }
}
