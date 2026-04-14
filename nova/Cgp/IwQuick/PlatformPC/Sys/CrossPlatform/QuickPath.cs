using System.Reflection;
using System.IO;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Sys
#else
namespace Contal.IwQuick.Sys
#endif
{
    /// <summary>
    /// path helper
    /// </summary>
    public class QuickPath
    {
        /// <summary>
        /// returns startup path for current executing assembly
        /// </summary>
        public static string AssemblyStartupPath
        {
            get
            {
#if COMPACT_FRAMEWORK
                return Directory.GetCurrentDirectory();
#else
                Assembly aBinary = Assembly.GetExecutingAssembly();
                // according to R# aBinary is always not-null
                return Path.GetDirectoryName(aBinary.Location);
#endif
            }
        }

        public static string ApplicationStartupDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool EnsureDirectory(string directoryPath)
        {
            if (Validator.IsNullString(directoryPath))
                return false;

            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool EnsureDirectory(string rootPath, string subDirectory)
        {
            if (Validator.IsNullString(rootPath) ||
                Validator.IsNullString(subDirectory))
                return false;

            return EnsureDirectory(rootPath + StringConstants.BACKSLASH + subDirectory);
        }
    }
}
