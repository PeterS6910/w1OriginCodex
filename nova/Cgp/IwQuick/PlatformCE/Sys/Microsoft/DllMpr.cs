using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllMpr
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NetworkResource
        {
            public int _scope;
            public int _type;
            public int _displayType;
            public int _tsage;
            public string m_szLocalName;
            public string m_szRemoteName;
            public string _comment;
            public string _provider;
        }

        public const int RESOURCETYPE_DISK = 0x1;

        public const int RESOURCEDISPLAYTYPE_SHARE = 0x3;

        public const int RESOURCEUSAGE_CONNECTABLE = 0x1;

        public const int RESOURCE_GLOBALNET = 0x2;

        //Standard	
        public const int CONNECT_INTERACTIVE = 0x00000008;
        public const int CONNECT_PROMPT = 0x00000010;
        public const int CONNECT_UPDATE_PROFILE = 0x00000001;
        //IE4+
        public const int CONNECT_REDIRECT = 0x00000080;
        //NT5 only
        public const int CONNECT_COMMANDLINE = 0x00000800;
        public const int CONNECT_CMD_SAVECRED = 0x00001000;

        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2A(ref NetworkResource pstNetRes, string psPassword, string psUsername, int piFlags);
        
        [DllImport("mpr.dll")]
        public static extern int WNetCancelConnection2A(string psName, int piFlags, int pfForce);
        
        [DllImport("mpr.dll")]
        public static extern int WNetRestoreConnectionW(int phWnd, string psLocalDrive);

        [DllImport("mpr.dll")]
        public static extern int WNetConnectionDialog(int phWnd, int piType);
        [DllImport("mpr.dll")]
        public static extern int WNetDisconnectDialog(int phWnd, int piType);
    }
}
