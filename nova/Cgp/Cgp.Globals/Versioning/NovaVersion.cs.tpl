using System;

// https://stackoverflow.com/questions/17713309/how-to-link-software-versions-to-tortoise-svn-revisions?rq=1

namespace Contal.Cgp.Globals.Versioning
{
    public static class NovaVersion
    {
        // The version number is loaded from SVN revision automatically. Build PC 4.5 (Client/Server) first to regenerate this class.
		public const string Edition = "2.2";  
        public const string Version = "2.2.$WCREV$";   
    }
}
