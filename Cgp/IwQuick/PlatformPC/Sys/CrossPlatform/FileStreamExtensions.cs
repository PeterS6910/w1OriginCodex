using System.IO;
#if !COMPACT_FRAMEWORK
// ReSharper disable once CheckNamespace
namespace Contal.IwQuick.Sys
#else
// ReSharper disable once CheckNamespace
namespace Contal.IwQuickCF.Sys
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileStreamExtensions
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
                fileStream.Flush();

                fileStream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
