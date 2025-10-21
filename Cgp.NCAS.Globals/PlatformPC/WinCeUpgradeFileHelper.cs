using System;
using System.IO;
using System.Text;

namespace Contal.Cgp.NCAS.Globals.PlatformPC
{
    public class WinCeUpgradeFileHelper
    {
        private const int CRCv1DataLenght = 64;
        private const int CRCv2DataLenght = 28;

        public static string GetVersion(string filePath)
        {
            int crcVersion;
            return GetVersion(filePath, out crcVersion);
        }

        public static string GetVersion(string filePath, out int crcVersion)
        {
            crcVersion = 0;

            if (string.IsNullOrEmpty(filePath))
                return null;

            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fileStream.Seek(fileStream.Length - CRCv2DataLenght, SeekOrigin.Begin);
                var data = ReadBytes(fileStream, 8);

                if (data == null)
                    return null;

                if (IsCRCv2(data))
                {
                    crcVersion = 2;
                    var versionBytes = ReadBytes(fileStream, 4);

                    if (versionBytes == null)
                        return null;

                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(versionBytes);

                    return BitConverter.ToInt32(versionBytes, 0).ToString();
                }

                fileStream.Seek(fileStream.Length - CRCv1DataLenght, SeekOrigin.Begin);

                data = ReadBytes(fileStream, CRCv1DataLenght);

                if (data == null)
                    return null;

                var dataString = Encoding.ASCII.GetString(data).Split(',');

                if (dataString.Length < 3
                    || dataString[0] != "WinCE")
                {
                    return null;
                }

                crcVersion = 1;

                return Int32.Parse(dataString[1]).ToString();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (fileStream != null)
                    try
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    catch
                    {
                    }
            }
        }

        private static byte[] ReadBytes(FileStream fs, int count)
        {
            var data = new byte[count];

            var readedBytes = 0;

            while (readedBytes < count)
            {
                var acutalReadedBytes = fs.Read(data, readedBytes, count - readedBytes);

                if (acutalReadedBytes == 0)
                    return null;

                readedBytes += acutalReadedBytes;
            }

            return data;
        }

        private static bool IsCRCv2(byte[] dataBytes)
        {
            var crcv2NeededBytes = new byte[] { 0x13, 0x07, 0x0F, 0xA6, 0x31, 0xCF, 0x7A, 0x4E };

            if (dataBytes.Length != crcv2NeededBytes.Length)
                return false;

            for (int i = 0; i < crcv2NeededBytes.Length; i++)
            {
                if (dataBytes[i] != crcv2NeededBytes[i])
                    return false;
            }

            return true;
        }
    }
}
