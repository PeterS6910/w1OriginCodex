using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.NCAS.Client
{
    public class MultiDoorTypeItem
    {
        private string _name = string.Empty;

        public MultiDoorType Type { get; private set; }

        public override string ToString()
        {
            return _name;
        }

        public void Translate(LocalizationHelper localizationHelper)
        {
            _name = localizationHelper.GetString(string.Format("MultiDoorType_{0}", Type));
        }

        public static ICollection<MultiDoorTypeItem> GetMultiDoorTypeItems()
        {
            return new LinkedList<MultiDoorTypeItem>(
                Enum.GetValues(typeof(MultiDoorType))
                    .Cast<MultiDoorType>()
                    .Select(type => new MultiDoorTypeItem { Type = type }));
        }
    }
}
