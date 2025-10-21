using System.Reflection;
using System.IO;
using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// path helper
    /// </summary>
    public class QuickPath
    {
        private const int MaxRetries = 2;
        private const int RetryDelayMs = 100;

        private static string _cachedAssemblyStartupPath = null;

        /// <summary>
        /// returns startup path for current executing assembly
        /// </summary>
        public static string AssemblyStartupPath
        {
            get
            {
                // not updated often, probability of concurrent initialisations is low
                if (null == _cachedAssemblyStartupPath)
                {

                    Assembly assembly = Assembly.GetExecutingAssembly();
                    // according to R# aBinary is always not-null
                    _cachedAssemblyStartupPath = Path.GetDirectoryName(
#if COMPACT_FRAMEWORK
                        assembly.GetName().CodeBase
#else
                        assembly.Location
#endif
                        );
                }

                return _cachedAssemblyStartupPath;
            }
        }

        private static string _cachedApplicationFilePath = null;

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationFilePath
        {
            get {
                return _cachedApplicationFilePath ??
                       (_cachedApplicationFilePath =
                           AssemblyStartupPath + StringConstants.BACKSLASH + AppDomain.CurrentDomain.FriendlyName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static string ExecutableFilePath
        {
            get { return ApplicationFilePath; }
        }

        private static volatile string _cachedStartupDirectory;

        /// <summary>
        /// 
        /// </summary>
        public static string ApplicationStartupDirectory
        {
            get {
                return _cachedStartupDirectory ??
                       (_cachedStartupDirectory =
                           Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subdirectory"></param>
        /// <returns></returns>
        public static string StartupSubDirectory(string subdirectory)
        {
            if (string.IsNullOrEmpty(subdirectory))
                return ApplicationStartupDirectory;

            return ApplicationStartupDirectory + StringConstants.BACKSLASH + subdirectory;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="subDirectory"></param>
        /// <returns></returns>
        public static bool EnsureDirectory(string rootPath, string subDirectory)
        {
            if (Validator.IsNullString(rootPath) ||
                Validator.IsNullString(subDirectory))
                return false;

            return EnsureDirectory(rootPath + StringConstants.BACKSLASH + subDirectory);
        }

        public delegate void DFsElementProcessing(bool isDirectory, string elementPath);

        /// <summary>
        /// implementation mainly due lack of Directory.EnumerateFile in .NET Compact
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="directorySearchPattern"></param>
        /// <param name="fileSearchPattern"></param>
        /// <param name="processingLambda"></param>
        /// <param name="processRoot"></param>
        /// <param name="processFiles"></param>
        /// <param name="exceptionsInProcessingIgnored"></param>
        /// <param name="processDirectories"></param>
        public static void TraverseDirectory(
            [NotNull] string directoryPath,
            [CanBeNull] string directorySearchPattern,
            [CanBeNull] string fileSearchPattern,
            [NotNull] DFsElementProcessing processingLambda,
            bool processRoot,
            bool processDirectories,
            bool processFiles,
            bool exceptionsInProcessingIgnored
            )
        {
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException("Directory \"" + directoryPath + "\" not found", "directoryPath");

            Validator.CheckForNull(processingLambda, "processingLambda");

            var subDirs =
                string.IsNullOrEmpty(directorySearchPattern)
                    ? Directory.GetDirectories(directoryPath)
                    : Directory.GetDirectories(directoryPath, directorySearchPattern);

            if (processRoot && processDirectories)
                try
                {
                    processingLambda(true, directoryPath);
                }
                catch
                {
                    if (!exceptionsInProcessingIgnored)
                        throw;
                }

            foreach (var subDir in subDirs)
            {
                TraverseDirectory(
                    subDir,
                    directorySearchPattern,
                    fileSearchPattern,
                    processingLambda,
                    true,
                    processDirectories,
                    processFiles,
                    exceptionsInProcessingIgnored);
            }

            if (processFiles)
            {
                var subFiles =
                    string.IsNullOrEmpty(fileSearchPattern)
                        ? Directory.GetFiles(directoryPath)
                        : Directory.GetFiles(directoryPath, fileSearchPattern);

                foreach (var subFile in subFiles)
                {
                    try
                    {
                        processingLambda(false, subFile);
                    }
                    catch
                    {
                        if (!exceptionsInProcessingIgnored)
                            throw;
                    }
                }
            }

        }

#if ! COMPACT_FRAMEWORK
        [UsedImplicitly]
        public static void DeleteFile(string filePath)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    //File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                    break;
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (NotSupportedException)
                {
                    throw;
                }
                catch (PathTooLongException)
                {
                    throw;
                }
                catch
                {
                    if (retryCount > MaxRetries)
                        throw;

                    retryCount++;
                    Thread.Sleep(RetryDelayMs);
                }
            }
        }


        [UsedImplicitly]
        public static void DeleteDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath);
            var dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
                DeleteFile(file);

            foreach (string dir in dirs)
                DeleteDirectory(dir);

            int retryCount = 0;
            while (true)
            {
                try
                {
                    Directory.Delete(dirPath, false);
                    break;
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (PathTooLongException)
                {
                    throw;
                }
                catch
                {
                    if (retryCount > MaxRetries)
                        throw;

                    retryCount++;
                    Thread.Sleep(RetryDelayMs);
                }
            }
        }
#endif
    }
}
