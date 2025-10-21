using System.IO;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static bool TryClose(this FileStream fileStream)
        {
            try
            {
                if (ReferenceEquals(fileStream, null))
                    return false;

                fileStream.Flush();

                fileStream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool TryClose(this Stream stream)
        {
            try
            {
                if (ReferenceEquals(stream, null))
                    return false;

                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
