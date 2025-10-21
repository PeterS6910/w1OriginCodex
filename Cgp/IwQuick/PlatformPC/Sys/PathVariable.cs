using System;
using System.IO;

using Contal.IwQuick.Parsing;

namespace Contal.IwQuick.Sys
{
    public class PathVariable : EnvironmentVariable
    {
        protected TokenString _paths;

        public PathVariable()
            : base("PATH")
        {
            if (null == _buffer)
                throw new NullReferenceException();

            string separator;
            if (Environment.OSVersion.Platform ==
                PlatformID.Unix)
                separator = ":";
            else
                separator = ";";

            _paths = new TokenString(separator, 16);

            _paths.SetCaseSensitive(true);
            _paths.Load(_buffer);
        }

        public PathVariable(SystemAccessLevel accessLevel)
            : base("PATH", accessLevel)
        {
        }

        public String GetPath(int i_iPos)
        {
            return _paths.Get(i_iPos);
        }


        public int GetCount()
        {
            return _paths.GetCount();
        }

        public void RegisterPath(String i_strPath)
        {
            if (!Directory.Exists(i_strPath))
                throw new DirectoryNotFoundException();

            _paths.AddUnique(i_strPath);
            Dump();
        }

        public void UnregisterPath(String i_strPath)
        {
            _paths.DelUnique(i_strPath);
            Dump();
        }
    }
}
