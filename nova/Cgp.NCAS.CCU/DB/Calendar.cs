using System;
using System.Collections.Generic;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(200)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class Calendar : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdCalendar { get; set; }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }

        public Guid GetGuid()
        {
            return IdCalendar;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Calendar;
        }
    }
}

