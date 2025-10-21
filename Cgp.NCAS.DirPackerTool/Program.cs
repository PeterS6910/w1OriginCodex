using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Contal.IwQuick.Sys;

namespace Cgp.NCAS.DirPackerTool
{
    class Program
    {
        static void Main(string[] args)
        {
            FilePacker.DeviceType deviceType;
            string version = "";
            string srcDir = "";
            string dstFile = "";
            List<string> excludes = new List<string>();
            if (args.Length >= 4)
            {
                deviceType = GetDeviceType(args[0].TrimEnd(' '));                
                version = AssemblyName.GetAssemblyName(args[1].TrimEnd(' ')).Version.ToString();
                srcDir = args[2].TrimEnd(' ');
                dstFile = args[3].TrimEnd(' ');
                if (args.Length > 4)
                {
                    for (int i = 4; i < args.Length; i++)
                    {
                        excludes.Add(args[i].TrimEnd(' '));
                    }
                }
                if (!DirPacker.Singleton.Pack(deviceType, version, srcDir, dstFile, excludes))
                    Console.WriteLine("An error occured while packing directory");
            }
        }

        static FilePacker.DeviceType GetDeviceType(string deviceType)
        {
            Array values = Enum.GetValues(typeof(FilePacker.DeviceType));
            foreach (object value in values)
            {
                if (Enum.GetName(typeof(FilePacker.DeviceType), value) == deviceType)
                {
                    return (FilePacker.DeviceType)value;
                }
            }
            return FilePacker.DeviceType.Other;
        }
    }
}
