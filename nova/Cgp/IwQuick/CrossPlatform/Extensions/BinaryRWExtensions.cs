using System.IO;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public static class BinaryRWExtensions
    {
        public static bool TryClose([CanBeNull] this BinaryReader binaryReader)
        {
            if (ReferenceEquals(binaryReader, null))
                return false;

            try
            {
                binaryReader.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryClose([CanBeNull] this BinaryWriter binaryWriter)
        {
            if (ReferenceEquals(binaryWriter, null))
                return false;

            try
            {
                binaryWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
