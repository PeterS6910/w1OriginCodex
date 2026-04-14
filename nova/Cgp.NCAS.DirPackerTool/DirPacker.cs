using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.IwQuick.Sys;
using System.IO;

namespace Cgp.NCAS.DirPackerTool
{
    public class DirPacker
    {
        private static DirPacker _singleton = null;
        public static DirPacker Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new DirPacker();
                return _singleton;
            }
        }

        public bool Pack(FilePacker.DeviceType deviceType, string version, string dirSrc, string dstFile, List<string> excludes)
        {
            FilePacker packer = new FilePacker();
            if (excludes != null && excludes.Count > 0)
                packer.SetExcludes(excludes.ToArray());
            switch (deviceType)
            {
                case FilePacker.DeviceType.CAT12CE:
                    return packer.Pack(FilePacker.DeviceType.CAT12CE, dirSrc, dstFile, version);
                case FilePacker.DeviceType.CCU:
                    {                       
                        dstFile = Path.Combine(Path.GetDirectoryName(dstFile),Path.GetFileNameWithoutExtension(dstFile) + "-" + version + Path.GetExtension(dstFile));
                        return packer.Pack(FilePacker.DeviceType.CCU, dirSrc, dstFile, version);
                    }
                case FilePacker.DeviceType.Other:
                    return packer.Pack(FilePacker.DeviceType.Other, dirSrc, dstFile, version);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
