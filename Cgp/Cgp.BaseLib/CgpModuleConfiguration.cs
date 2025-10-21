using System;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.BaseLib
{
    [Serializable]
    [LwSerialize(101)]
    [LwSerializeMode(LwSerializationMode.Selective)]
    //[LwSerializeAssign(AssignMode.LenientDynamic)]
    public class CgpModuleConfiguration
    {
        [LwSerialize]
        private string _name = null;

        [LwSerialize]
        private string _description = null;

        public string Name
        {
            get { return _name; }
            protected set
            {
                Validator.CheckNullString(value,"name");
                _name = value;
            }
        }

        public string Description
        {
            get { return _name; }
            protected set
            {
                if (null == value)
                    _description = String.Empty;
                else
                    _description = value;
            }
        }

        public CgpModuleConfiguration(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public CgpModuleConfiguration(string name)
        {
            Name = name;
            // will set description to ""
            Description = null;
        }

        private string _cgpType = null;
        protected internal string CgpType
        {
            get
            {
                return _cgpType;
            }
            set
            {
                if (null != _cgpType)
                    return;

                Validator.CheckNullString(value);
                _cgpType = value;
            }
        }

        [LwSerialize]
        protected internal Dictionary<short,string[]> _referrenceStorage = null;
       

    }
}
