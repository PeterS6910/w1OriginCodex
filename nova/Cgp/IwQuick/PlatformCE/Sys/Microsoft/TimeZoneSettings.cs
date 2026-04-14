using System;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class TimeZoneSettings
    {
        private const string ROOT_REGISTRY = @"HKLM\Time Zones";

        private static string[] _allZones = null;

        public static string[] ListZoneNames()
        {
            
                if (null == _allZones)
                {
                    try
                    {
                        _allZones = RegistryHelper.GetSubkeys(ROOT_REGISTRY);
                        if (_allZones == null)
                            _allZones = new string[0];
                        else
                            Array.Sort(_allZones);
                    }
                    catch
                    {
                        _allZones = new string[0];
                    }
                }

                return _allZones;
            
        }

        public static string[] ListZoneDescriptions()
        {
            string[] zones = ListZoneNames();

            if (null != zones)
            {
                string display = null;
                foreach(string zone in zones) {
                    if (RegistryHelper.TryGetValue(ROOT_REGISTRY+"\\"+zone, "Display", ref display))
                        Console.WriteLine(zone+ " / "+display);
                }
            }

            return null;
        }

        public static bool IsTimeZoneValid(string timeZoneName)
        {
            if (Validator.IsNullString(timeZoneName))
                return false;

            string[] zones = ListZoneNames();

            if (null != zones && zones.Length > 0)
                foreach (string zone in zones)
                {
                    if (zone == timeZoneName)
                        return true;
                }

            return false;
        }


        public static bool SetTimeZone(string timeZoneName)
        {
            if (!IsTimeZoneValid(timeZoneName))
                return false;           

            return RegistryHelper.SetValue(ROOT_REGISTRY,null,timeZoneName);
        }

        public static string CurrentTimeZoneName
        {
            get
            {
                TimeZone tz = TimeZone.CurrentTimeZone;
                if (null != tz)
                    return tz.StandardName;
                return string.Empty;
            }
        }
    }
}
