using System;


// https://stackoverflow.com/questions/17713309/how-to-link-software-versions-to-tortoise-svn-revisions?rq=1

namespace Contal.Cgp.Globals.Versioning
{
    public static class NovaVersion
    {
        // The version number is loaded from SVN revision automatically. Build PC 4.5 (Client/Server) first to regenerate this class.

        public const string Edition = "2.2";
        public const string Version = "2.2.9463"; //build 1 oktobra 2025

    }

    public static class NovaVersionDay
    {
        // The version number is loaded from SVN revision automatically. Build PC 4.5 (Client/Server) first to regenerate this class.

        public const string Edition = "2.2";
        public const string Version = "2.2.9463"; //build 1 oktobra 2025

    }

    internal  class VersionDay
    {
        private const int StartNumber = 8430;  //for date 30.01.2023
       // private const int StartNumber = 8440;  //for date 09.02.2023
        public  readonly static long DayNum = StartNumber+ (DateTime.Today - new DateTime(2023, 01, 30)).Days;
        public  string Version = "2.2."+ StartNumber.ToString();
    }

}
