using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class FileCorruptedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public FileCorruptedException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public FileCorruptedException()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NotUpgradeFileException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public NotUpgradeFileException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public NotUpgradeFileException()
        {
        }
    }

   

    /// <summary>
    /// 
    /// </summary>
    public class FilePacker
    {
        /*public enum DeviceType
        {
            CAT12CE = 0,
            CCU = 1,
            DCU = 2,
            CR = 3,
            Client = 4,
            Other = 5
        }*/

        public class PackingContext
        {
            public string[] HeaderParts = null;
            
            /// <summary>
            /// if false, the final file will be simple container
            /// </summary>
            public bool UseGzCompression = true;

            /// <summary>
            /// if true, packing and unpacking process will skip over the files, that are unable to be accessed
            /// thus read or written
            /// </summary>
            public bool IgnoreFileInaccessibility = false;

            internal IEnumerable<string> _excludePatternList;

            public IEnumerable<string> Excludes
            {
                get { return _excludePatternList; }
                set { _excludePatternList = value; }
            }
        }

        public class UnpackingContext
        {
            /// <summary>
            /// if true, packing and unpacking process will skip over the files, that are unable to be accessed
            /// thus read or written
            /// </summary>
            public bool SkipOverInaccessibleFile = false;

            /// <summary>
            /// if false, the source file will be considered simple container
            /// </summary>
            internal bool _gzCompression = true;

            public bool GzCompression
            {
                get { return _gzCompression; }
            }

            internal long _packagePosition;

            internal long _packageSize;

            internal int _lastProgressUpdate;

            internal int ApproxProgress
            {
                get { return (int) (((double) _packagePosition/ _packageSize)*50) + 50; }
            }

        }

        private static readonly PackingContext DefaultPackingContext = new PackingContext();

        public FilePacker()
        {
            UnpackProgressThreshold = DefaultUnpackProgressThreshold;
        }

        #region constans
        private const string HeaderDelimiter = StringConstants.COMMA;

        private const int HeaderPrefixLength = 2; // 0xFF 0x?? - length of header
        private const byte CompressedPackageType = 0xFF; // in uncompressed form
        private const byte TarPackageType = 0xFE;

        private const string TmpFileExtension = ".tmp";
        //private const string PACKED_FILE_EXTENSION = ".gz";
        private const int HeaderFixedLength = 64;
        private const int MaxHeaderStringLength = HeaderFixedLength - HeaderPrefixLength;

        private const int DataStartPosition = HeaderFixedLength + sizeof (uint);

        private const int IdFile = 0x01;
        private const int IdDirectory = 0x02;
        #endregion


        private readonly LinkedList<Exception> _packingErrors = new LinkedList<Exception>();

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Exception> PackingErrors
        {
            get { return _packingErrors; }
        } 

        private void RecordPackError([NotNull] Exception error)
        {
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(error,null))
// ReSharper restore ConditionIsAlwaysTrueOrFalse
// ReSharper disable once HeuristicUnreachableCode
                return;

            _packingErrors.AddLast(error);
        }

        private void ClearPackErrors()
        {
            _packingErrors.Clear();
        }

        

        #region Packing 

        /// <summary>
        /// TryPack directory structure into single file and then runs gzipstream on it
        /// </summary>
        /// <param name="archiveName">first header parameter</param>
        /// <param name="sourcePath">source file or directory</param>
        /// <param name="outputFilePath">Output file</param>
        /// <returns></returns>
        public bool TryPack(
            [NotNull] string archiveName, 
            [NotNull] string sourcePath, 
            [NotNull] string outputFilePath)
        {
            return TryPack(archiveName, sourcePath, outputFilePath, null);
        }

        /// <summary>
        /// TryPack directory structure into single file and then runs gzipstream on it
        /// </summary>
        /// <param name="archiveName">first header parameter</param>
        /// <param name="sourcePath">source file or directory</param>
        /// <param name="outputFilePath">Output file</param>
        /// <param name="packingContext"></param>
        /// <returns></returns>
        public bool TryPack(
            [NotNull] string archiveName, 
            [NotNull] string sourcePath, 
            [NotNull] string outputFilePath, 
            [CanBeNull] PackingContext packingContext)
        {
            FileStream file = null;
            bool result = false;

            packingContext = packingContext ?? DefaultPackingContext;

            try
            {
                Validator.CheckNullString(archiveName, "archiveName");
                Validator.CheckNullString(sourcePath, "sourcePath");
                Validator.CheckNullString(outputFilePath, "outputFilePath");
            }
            catch (Exception e)
            {
                RecordPackError(e);
                return false;
            }

            ClearPackErrors();
            byte[] reusableBuffer = null;
            BinaryWriter outputWriter = null;

            try
            {
                

                file = new FileStream(outputFilePath + TmpFileExtension, FileMode.Create);
                outputWriter = new BinaryWriter(file);

                if (!packingContext.UseGzCompression)
                    FillHeader(packingContext, archiveName, outputWriter, 0);

                reusableBuffer =_bufferPool.GetBuffer();

                //Check if source is path to directory or file
                if (Directory.Exists(sourcePath))
                {
                    var mainDir = new DirectoryInfo(sourcePath);
                    result = PackDirectory(packingContext, mainDir, null,null, outputWriter, reusableBuffer);
                }
                else if (File.Exists(sourcePath))
                {
                    var fileInfo = new FileInfo(sourcePath);
                    result = PackFile(packingContext, fileInfo, outputWriter, reusableBuffer);
                }

                if (!result)
                    return false;
            }
            catch (Exception err)
            {
                RecordPackError(err);
                return false;
            }
            finally
            {
                try
                {
                    if (null != reusableBuffer)
                        _bufferPool.ReturnBuffer(reusableBuffer);
                }
                catch
                {
                    
                }

                if (outputWriter != null)
                    try
                    {
                        outputWriter.Close();
                    }
                    catch
                    {
                    }

                file.TryClose();
            }

            if (packingContext.UseGzCompression)
                return CompressAndFillHeader(packingContext, archiveName, outputFilePath);

            return FinalizeTar(packingContext, outputFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packingContext"></param>
        /// <param name="archiveName"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        private bool CompressAndFillHeader([NotNull] PackingContext packingContext, string archiveName, string outputFile)
        {
            FileStream tarStream = null;
            Stream finalStream = null;


            try
            {
                

                
                
                // calculate checksum
                tarStream = new FileStream(outputFile + TmpFileExtension, FileMode.Open);

                // do some compression
                

                    
                    uint checkumOfTar = Crc32.ComputeChecksum(tarStream);
                    tarStream.Seek(0, SeekOrigin.Begin);


                    finalStream = new GZipStream(
                        File.Create(outputFile),
                        CompressionMode.Compress);

                    // write header
                    var bWriter = new BinaryWriter(finalStream);


                    FillHeader(packingContext, archiveName, bWriter, checkumOfTar);


                    var buffer = _bufferPool.GetBuffer();

                    Stopwatch sw = null;
                    try
                    {
                        _log.Info("Compressing " + archiveName + " ... ");
                        sw = StopwatchPool.Singleton.MeasureStart();

                        while (tarStream.Position < tarStream.Length)
                        {
                            int read = tarStream.Read(buffer, 0, buffer.Length);
                            if (read > 0)
                                finalStream.Write(buffer, 0, read);
                        }



                    }
                    finally
                    {
                        _log.Info("Compressing " + archiveName + " took " + sw.MeasureStopAndReturn() + "ms");

                        _bufferPool.ReturnBuffer(buffer);
                    }
               


                return true;
            }
            catch(Exception err)
            {
                RecordPackError(err);
                //Console.WriteLine("TryPack: " + error.Message);
                return false;
            }
            finally
            {
                tarStream.TryClose();

                finalStream.TryClose();

                try {File.Delete(outputFile + TmpFileExtension);}catch{}
            }
        }

// ReSharper disable UnusedParameter.Local
        private bool FinalizeTar(PackingContext packingContext, string outputFile)
// ReSharper restore UnusedParameter.Local
        {
            try
            {
                var tarStream = new FileStream(outputFile + TmpFileExtension, FileMode.Open);

                uint checkumOfTar = Crc32.ComputeChecksum(tarStream, HeaderFixedLength+sizeof(uint), -1);




                byte[] checksumBytes = BitConverter.GetBytes(checkumOfTar);
                tarStream.Seek(HeaderFixedLength, SeekOrigin.Begin);

                tarStream.Write(checksumBytes, 0, checksumBytes.Length);


                tarStream.Close();

                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                File.Move(outputFile + TmpFileExtension, outputFile);

                return true;
            }
            catch (Exception e)
            {
                RecordPackError(e);
                return false;
            }
        }

        private static string HeaderPartsAsString(string[] headerParts, string archiveName)
        {
            string headerAllString = archiveName ?? String.Empty;

            if (!headerParts.IsEmpty())
            {
                foreach (var part in headerParts)
                {
                    if (part == null)
                        continue;

                    headerAllString +=
                        HeaderDelimiter + part;
                }
            }
            return headerAllString;
        }

        private void FillHeader(
            PackingContext packingContext,
            string archiveName,
            [NotNull] BinaryWriter bWriter, 
            uint checkumOfTar)
        {
            var headerBytes = new byte[HeaderFixedLength];
            headerBytes[0] = packingContext.UseGzCompression
                ? CompressedPackageType
                : TarPackageType;

            var headerAllString = HeaderPartsAsString(packingContext.HeaderParts, archiveName);

            
            var lengthOfStringToCopy =
                    headerAllString.Length > MaxHeaderStringLength
                        ? MaxHeaderStringLength
                        : headerAllString.Length;
            
            //append header string length
            headerBytes[1] = (byte)(lengthOfStringToCopy);

            ByteDataCarrier.StringToBytes(headerAllString, Encoding.ASCII, 
                (stringBytes, stringBytesLength) => 
                    Buffer.BlockCopy(stringBytes,0,headerBytes,HeaderPrefixLength,lengthOfStringToCopy));
            
            bWriter.Write(headerBytes);

            bWriter.Write(checkumOfTar);
        }

        /// <summary>
        /// Add content of single directory to file (also the dir name, etc..)
        /// </summary>
        /// <param name="packingContext"></param>
        /// <param name="directory">Object holding directory to be added</param>
        /// <param name="subFilesCached"></param>
        /// <param name="outputWriter"></param>
        /// <param name="reusableBuffer"></param>
        /// <param name="subDirectioriesCached"></param>
        /// <returns>true if succesfull, false otherwise</returns>
        private bool PackDirectory(
            [NotNull] PackingContext packingContext,
            [NotNull] DirectoryInfo directory,
            [CanBeNull] DirectoryInfo[] subDirectioriesCached,
            [CanBeNull] FileInfo[] subFilesCached,
            [NotNull] BinaryWriter outputWriter,
            [NotNull] byte[] reusableBuffer)
        {
            try
            {
                DirectoryInfo[] subDirectories =
                    subDirectioriesCached ?? directory.GetDirectories();

                var excludedSubDirectories = GetExcludedSubDirectories(directory,packingContext._excludePatternList);

                FileInfo[] subFiles =
                    subFilesCached ?? directory.GetFiles();

                var excludedSubFiles = GetExcludedSubFiles(directory,packingContext._excludePatternList);

                foreach (var subDir in subDirectories)
                {
                    if (excludedSubDirectories != null &&
                        excludedSubDirectories.Contains(subDir.FullName))
                        continue;

                    outputWriter.Write((byte)IdDirectory);
                    char[] dirName = subDir.Name.ToCharArray();
                    outputWriter.Write(dirName.Length);
                    outputWriter.Write(dirName);

                    var subDirectoriesOfSubdir = subDir.GetDirectories();
                    var subFilesOfSubdir = subDir.GetFiles();
                    outputWriter.Write(subDirectoriesOfSubdir.Length + subFilesOfSubdir.Length);

                    if (PackDirectory(
                        packingContext, 
                        subDir, 
                        subDirectoriesOfSubdir,
                        subFilesOfSubdir,
                        outputWriter,
                        reusableBuffer) == false)
                        return false;
                }

                foreach (FileInfo subFileInfo in subFiles)
                {
                    if (excludedSubFiles != null &&
                        excludedSubFiles.Contains(subFileInfo.FullName))
                        continue;
                    
                    if (!PackFile(packingContext, subFileInfo, outputWriter, reusableBuffer))
                        return false;
                }

                return true;
            }
            catch(Exception err)
            {
                RecordPackError(err);
                return false;
            }
        }

        private static readonly ByteBufferPool _bufferPool = new ByteBufferPool(4, 65536);

        /// <summary>
        /// Add content of file to file
        /// </summary>
        /// <param name="packingContext"></param>
        /// <param name="fileInfo">Object holding file to be added</param>
        /// <param name="outputWriter"></param>
        /// <param name="reusableBuffer"></param>
        /// <returns>true if succesfull, false otherwise</returns>
        private bool PackFile(
            [NotNull] PackingContext packingContext, 
            [NotNull] FileInfo fileInfo,
            [NotNull] BinaryWriter outputWriter,
            [NotNull] byte[] reusableBuffer)
        {
            FileStream fileToCopy = null;
            try
            {
                // write the mark only after openRead did not failed
                outputWriter.Write((byte)IdFile);
                char[] fileName = fileInfo.Name.ToCharArray();
                outputWriter.Write(fileName.Length);
                outputWriter.Write(fileName);

                try
                {
                    fileToCopy = fileInfo.OpenRead();
                    outputWriter.Write(fileInfo.Length);
                }
                catch
                {
                    // simulate empty file on file opening failure
                    outputWriter.Write((long)0);
                    _log.Info("Skipping " + fileInfo.FullName);
                    throw;
                }


                _log.Info("Copying " + fileInfo.FullName + " size=" + fileInfo.Length + "B ... ");
                Stopwatch sw = StopwatchPool.Singleton.MeasureStart();

                try
                {
                    while (fileToCopy.Position < fileToCopy.Length)
                    {
                        int read = fileToCopy.Read(reusableBuffer, 0, reusableBuffer.Length);
                        if (read > 0)
                            outputWriter.Write(reusableBuffer, 0, read);
                    }

                }
                finally
                {
                    _log.Info("Copying " + fileInfo.FullName + " size=" + fileInfo.Length + "B took " + sw.MeasureStopAndReturn() + "ms");
                }
                

                return true;
            }
            catch (IOException ioError)
            {
                RecordPackError(ioError);
                return packingContext.IgnoreFileInaccessibility;
            }
            catch (Exception err)
            {
                RecordPackError(err);
                return false;
            }
            finally
            {
                fileToCopy.TryClose();
            }
        }


        public static HashSet<string> GetExcludedSubFiles(DirectoryInfo directory,IEnumerable<string> excludePatternList)
        {

            if (excludePatternList == null)
                return null;

            HashSet<string> excludeSet = null;
            foreach (var pattern in excludePatternList)
            {
                try
                {
                    foreach (var fileInfo in directory.GetFiles(pattern))
                    {
                        if (excludeSet == null)
                            excludeSet = new HashSet<string>();

                        excludeSet.Add(fileInfo.FullName);
                    }
                }
                catch { }
            }
            return excludeSet;
        }

        public static HashSet<string> GetExcludedSubDirectories(DirectoryInfo directory,IEnumerable<string> excludePatternList)
        {

            if (excludePatternList == null)
                return null;

            HashSet<string> excludeSet = null;

            foreach (string pattern in excludePatternList)
            {
                try
                {
                    foreach (DirectoryInfo dirInfo in directory.GetDirectories(pattern))
                    {
                        if (excludeSet == null)
                            excludeSet = new HashSet<string>();

                        excludeSet.Add(dirInfo.FullName);
                    }
                }
                catch { }
            }
            return excludeSet;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("Use UnpackingProgressChanged event instead")]
        public event DInt2Void UpgradeProgressChanged;

        /// <summary>
        /// 
        /// </summary>
        public event Action<int> UnpackingProgressChanged;

        private const int DefaultUnpackProgressThreshold = 5;
        private int _unpackProgressThreshold = DefaultUnpackProgressThreshold;

        /// <summary>
        /// Threadshold for unpack progress event firing
        /// </summary>
        public int UnpackProgressThreshold
        {
            get { return _unpackProgressThreshold; }
            set
            {
                Validator.CheckIntegerRange(value, 1, 99);
                _unpackProgressThreshold = value;
            }
        }

        

        private void FireUnpackProgress(UnpackingContext unpackingContext, int progressUpdate)
        {
            try
            {
                /* Fire event only when difference between last update is greater than threshold */
                if (Math.Abs(progressUpdate - unpackingContext._lastProgressUpdate) < UnpackProgressThreshold)
                    return;
                unpackingContext._lastProgressUpdate = progressUpdate;

                if (UnpackingProgressChanged != null)
                    UnpackingProgressChanged(progressUpdate);
            }
            catch { }

            // TODO : remove in future
            try
            {
#pragma warning disable 612
#pragma warning disable 618
                if (UpgradeProgressChanged != null)
                    UpgradeProgressChanged(progressUpdate);
#pragma warning restore 612
#pragma warning restore 618
            }
            catch
            {
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectory"></param>
        public void Unpack(
            [NotNull] string inputFilePath, 
            [NotNull] string outputDirectory)
        {
            Unpack(inputFilePath, outputDirectory, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="unpackingContext"></param>
        public void Unpack(
            [NotNull] string inputFilePath, 
            [NotNull] string outputDirectory, 
            [CanBeNull] UnpackingContext unpackingContext)
        {
            Validator.CheckNullString(inputFilePath, "inputFilePath");
            Validator.CheckNullString(outputDirectory,"outputDirectory");

            FileStream fs=null;

            try
            {
                fs = File.OpenRead(inputFilePath);

                Unpack(fs, outputDirectory,unpackingContext);
            }
            finally
            {
                fs.TryClose();
            }
        }

        

        private const string FpSource = "FilePacker";

        private static readonly Log _log = 
#if COMPACT_FRAMEWORK
            new Log(FpSource, true, true, null, 0);
#else
            new Log(FpSource, true, true, false);
#endif


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory"></param>
        public void Unpack(
            [NotNull] Stream inputStream,
            [NotNull] string outputDirectory)
        {
            Unpack(inputStream, outputDirectory, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="unpackingContext"></param>
        public void Unpack(
            [NotNull] Stream inputStream,
            [NotNull] string outputDirectory,
            [CanBeNull] UnpackingContext unpackingContext)
        {
            Validator.CheckForNull(inputStream, "inputStream");
            Validator.CheckNullString(outputDirectory, "outputDirectory");

            unpackingContext = unpackingContext ?? new UnpackingContext();

            if (inputStream.CanSeek &&
                inputStream.Position != 0)
                inputStream.Seek(0, SeekOrigin.Begin);

            unpackingContext._gzCompression = QuickPreambleValidation(inputStream);

            if (unpackingContext.GzCompression)
                ValidateChecksumForCompressed(unpackingContext,inputStream);
            else
                ValidateChecksumForTar(inputStream);


            var mainDir = new DirectoryInfo(outputDirectory);
            mainDir.Create();

            Stream streamForExtraction = null;
            try
            {

                if (unpackingContext.GzCompression)
                {
                    inputStream.Seek(0, SeekOrigin.Begin);
                    /* Reopen the stream, so offset is set on data start again */
                    streamForExtraction = new FilePackerStream(
                        inputStream,
                        CompressionMode.Decompress,
                        true,
                        HeaderFixedLength);
                }
                else
                {
                    inputStream.Seek(DataStartPosition, SeekOrigin.Begin);
                    streamForExtraction = inputStream;
                }



                ExtractionFromStream(unpackingContext, streamForExtraction, ref mainDir);
            }
            finally
            {
                if (unpackingContext.GzCompression)
                    streamForExtraction.TryClose();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns>true, if inputStream is compressed</returns>
        /// <returns>false, if inputStream is uncompressed</returns>
        private static bool QuickPreambleValidation([NotNull] Stream inputStream)
        {


            var packageType = inputStream.ReadByte();
            int byte2;

            switch (packageType)
            {
                case FilePackerStream.MagicNumber0: // CompressedPackageType after compression
                    byte2 = inputStream.ReadByte();
                    if (byte2 != FilePackerStream.MagicNumber1)
                        goto default;

                    var byte3 = inputStream.ReadByte();
                    if (byte3 != FilePackerStream.MagicNumber2)
                        goto default;


                    return true;
                case TarPackageType:
                    byte2 = inputStream.ReadByte();
                    if (byte2 > MaxHeaderStringLength)
                        goto default;

                    return false;
                default:
                    throw new NotUpgradeFileException("Not a valid FilePacker file or file corrupted");
            }
            
        }

        private void ExtractionFromStream(
            [NotNull] UnpackingContext unpackingContext,
            [NotNull] Stream inputStream,
            ref DirectoryInfo parentDirectoryInfo)
        {
            var inputReader = new BinaryReader(inputStream);

            unpackingContext._packagePosition = 0;


            while (true)
            {
                int type = inputReader.BaseStream.ReadByte();
                if (type < 0)
                {
                    FireUnpackProgress(unpackingContext,100);
                    break;
                }

                unpackingContext._packagePosition++;

                if (type == IdFile)
                {
                    UnpackFile(unpackingContext,parentDirectoryInfo,inputReader);
                }
                else if (type == IdDirectory)
                {
                    
                    UnpackDirectory(unpackingContext,ref parentDirectoryInfo,inputReader);
                }
                else
                    throw new Exception("Unsuported mark: " + type);

                FireUnpackProgress(unpackingContext,unpackingContext.ApproxProgress);
            }
        }

        private void ValidateChecksumForCompressed(
            [NotNull] UnpackingContext unpackingContext,
            [NotNull] Stream inputStream)
        {
            FilePackerStream filePackerStream = null;

            try
            {
                inputStream.Position = 0;
                filePackerStream = new FilePackerStream(
                    inputStream, 
                    CompressionMode.Decompress, 
                    true,
                    HeaderFixedLength);
            

                /* Set to -threshold so the first 0 will be reported as well */
                unpackingContext._lastProgressUpdate = -UnpackProgressThreshold;
                FireUnpackProgress(unpackingContext,0);

                
                // calculate checksum
                var readChecksum = filePackerStream.Checksum;

                _log.Info("Calculating upgrade file checksum ... ");
                Stopwatch sw = StopwatchPool.Singleton.MeasureStart();
                uint checksum;

                try
                {
                     checksum = Crc32.ComputeChecksum(
                         filePackerStream, 
                         out unpackingContext._packageSize, 
                         progress=>FireUnpackProgress(unpackingContext, progress / 2)
                         );
                }
                finally
                {
                    _log.Info("Calculating upgrade file checksum took " + sw.MeasureStopAndReturn() + "ms");
                }
               

                if (checksum != readChecksum)
                    throw new FileCorruptedException("Read and calculated checksum does not match. File corrupted.");
            }
            finally
            {
                filePackerStream.TryClose();
            }
        }

        private void ValidateChecksumForTar(
            [NotNull] Stream inputStream)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            var headerAndChecksum = new byte[DataStartPosition];

            inputStream.Read(headerAndChecksum, 0, headerAndChecksum.Length);
            

            var checksumFromTar = BitConverter.ToUInt32(headerAndChecksum, HeaderFixedLength);

            var computedChecksum = Crc32.ComputeChecksum(inputStream, DataStartPosition, -1);

            if (checksumFromTar != computedChecksum)
                throw new FileCorruptedException("Read and calculated checksum does not match. File corrupted.");



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TryUnpack(
            [NotNull] string inputFilePath, 
            [NotNull] string outputDirectory,
            out Exception error)
        {
            return TryUnpack(inputFilePath, outputDirectory, null, out error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="unpackingContext"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TryUnpack(
            [NotNull] string inputFilePath, 
            [NotNull] string outputDirectory,
            UnpackingContext unpackingContext,
            out Exception error)
        {
            FileStream fs = null;

            try
            {
                fs = File.OpenRead(inputFilePath);

                Unpack(fs, outputDirectory, unpackingContext);

                error = null;

                return true;
            }
            catch (Exception e)
            {
                error = e;

                return false;
            }
            finally
            {
                fs.TryClose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TryUnpack(
            [NotNull] Stream inputStream,
            [NotNull] string outputDirectory,
            out Exception error)
        {
            return TryUnpack(inputStream, outputDirectory, null, out error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="unpackingContext"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TryUnpack(
            [NotNull] Stream inputStream,
            [NotNull] string outputDirectory,
            [CanBeNull] UnpackingContext unpackingContext,
            out Exception error)
        {
            try
            {
                Unpack(inputStream, outputDirectory, unpackingContext);

                error = null;

                return true;
            }
            catch (Exception e)
            {
                error = e;

                return false;
            }

        }

        
        


        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        public bool TryUnpack(string inputFilePath, string outputDirectory)
        {
            Exception e;
            return TryUnpack(inputFilePath, outputDirectory,out e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        public bool TryUnpack(Stream inputStream, string outputDirectory)
        {
            Exception e;
            return TryUnpack(inputStream, outputDirectory, out e);
        }



        /// <summary>
        /// 
        /// </summary>
        public event Action<bool, string> UnpackingProgress;
        private void InvokeUnpackingProgress(bool isFile, string path)
        {
            if (path == null)
                return;

            if (null != UnpackingProgress)
                try { UnpackingProgress(isFile, path); }
                catch
                {
                }
        }

        /// <summary>
        /// Depack content of single directory
        /// </summary>
        /// <returns>true if successfull, false otherwise</returns>
        private void UnpackDirectory(
            [NotNull] UnpackingContext unpackingContext,
            [NotNull] ref DirectoryInfo parentDirectoryInfo,
            [NotNull] BinaryReader inputReader)
        {
            long dirSize = 0;

            Int32 nameLength = inputReader.ReadInt32();
            dirSize += 4 + nameLength;
            char[] dirName = inputReader.ReadChars(nameLength);

            string dirPath = parentDirectoryInfo.FullName + "\\" + new string(dirName);
            InvokeUnpackingProgress(false, dirPath);

            Debug.WriteLine("Unpacking directory : " + dirPath);

            parentDirectoryInfo = new DirectoryInfo(dirPath);
            parentDirectoryInfo.Create();

            Int32 itemCount = inputReader.ReadInt32();
            dirSize += 4;

            for (Int32 i = 0; i < itemCount; i++)
            {
                byte type = inputReader.ReadByte();
                dirSize++;

                if (type == IdFile)
                {
                    UnpackFile(unpackingContext,parentDirectoryInfo,inputReader);
                }
                else if (type == IdDirectory)
                {
                    UnpackDirectory(unpackingContext,ref parentDirectoryInfo,inputReader);
                }
                else
                    throw new Exception("Unsuported mark: " + type);

                FireUnpackProgress(unpackingContext,unpackingContext.ApproxProgress);
            }
            parentDirectoryInfo = parentDirectoryInfo.Parent;

            unpackingContext._packagePosition += dirSize;
        }

        /// <summary>
        /// Depack single file
        /// </summary>
        /// <returns>true if successfull, false otherwise</returns>
        private void UnpackFile(
            [NotNull] UnpackingContext unpackingContext,
            [NotNull] DirectoryInfo parentDirectoryInfo,
            [NotNull] BinaryReader inputReader)
        {
            BinaryWriter outputFile = null;
            try
            {
                Int32 nameLength = inputReader.ReadInt32();

                char[] fileName = inputReader.ReadChars(nameLength);

                Int64 fileLength = inputReader.ReadInt64();
                string filePath = parentDirectoryInfo.FullName + "\\" + new string(fileName);

                InvokeUnpackingProgress(true, filePath);

                if (fileLength > 0)
                {
                    _log.Info("Unpacking file : " + filePath + " size=" + fileLength + "B ... ");
                    Stopwatch sw = StopwatchPool.Singleton.MeasureStart();

                    long expectedFuturePosition = -1;

                    if (!unpackingContext.GzCompression)
                        // TODO : seeking currently not supported on GZipStream, find different solution
                        expectedFuturePosition = inputReader.BaseStream.Position + fileLength;


                    int subFileReadCount = 0;
                            
                    try
                    {
                        outputFile = new BinaryWriter(new FileStream(filePath, FileMode.Create));

                        int chunkSize = _bufferPool.BufferSize;
                        var buffer = _bufferPool.GetBuffer();

                        try
                        {

                            int read = -1;

                            while (subFileReadCount < fileLength || read == 0)
                            {
                                /* 
                             * NOTE!! 
                             * Read ONLY fileLength of data, not more 
                             * otherwise it will corrupt following files in the TAR */
                                int toRead;
                                if (subFileReadCount + chunkSize > fileLength)
                                    toRead = (int) fileLength - subFileReadCount;
                                else
                                    toRead = chunkSize;

                                read = inputReader.Read(buffer, 0, toRead);
                                outputFile.Write(buffer, 0, toRead);

                                subFileReadCount += read;

                                unpackingContext._packagePosition += read;

                                FireUnpackProgress(unpackingContext, unpackingContext.ApproxProgress);
                            }
                        }
                       
                        finally
                        {
                            _bufferPool.ReturnBuffer(buffer);
                        }
                    }
                    catch
                    {
                        // need to ensure continuity in the tar stream
                        // thus seek, as if whole subfile was successfuly read from container
                        if (!unpackingContext.GzCompression)
                            // TODO : seeking currently not supported on GZipStream, find different solution
                            inputReader.BaseStream.Seek(expectedFuturePosition, SeekOrigin.Begin);
                        throw;
                    }
                    finally
                    {
                        _log.Info("Unpacking file : " + filePath + " size=" + fileLength + "B took " + sw.MeasureStopAndReturn() + "ms");
                    }

                }
                else
                {
                    _log.Info("Skipping file : " + filePath);

                }
            }
            catch (IOException ioError)
            {

                if (!unpackingContext.SkipOverInaccessibleFile)
                {
                    HandledExceptionAdapter.Examine(ioError);
                    inputReader.TryClose();
                    throw;
                }

            }
            catch(Exception e)
            {
                HandledExceptionAdapter.Examine(e);

                inputReader.TryClose();
                throw;
            }
            finally
            {
                outputFile.TryClose();
            }
        }

        

        /// <summary>
        /// Gets header text from specific file
        /// </summary>
        /// <param name="fileName">Full file name</param>
        /// <returns>Header text</returns>       
        public static string GetHeaderText(string fileName)
        {
            Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return GetHeaderText(fileStream);
            }
            finally
            {
                fileStream.TryClose();
            }

        }

        /// <summary> 
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [NotNull]
        public static byte[] GetHeaderTextBytes(Stream stream)
        {
            if (ReferenceEquals(stream, null))
                throw new ArgumentNullException("stream");

            stream.Seek(0, SeekOrigin.Begin);

            var gzCompression = QuickPreambleValidation(stream);

            stream.Seek(0, SeekOrigin.Begin);

            Stream readingStream = null;
            var headerBytes = new byte[HeaderFixedLength];
            int bytesRead;

            try
            {
                readingStream =
                    gzCompression
                        ? new GZipStream(stream, CompressionMode.Decompress, true)
                        : stream;
            
                bytesRead = readingStream.Read(headerBytes, 0, HeaderFixedLength);
            }
            finally
            {
                if (gzCompression)
                    readingStream.TryClose();
            }


            if (bytesRead != HeaderFixedLength)
            {
                throw new InvalidDataException("File header has wrong length");
            }

            byte textLength = headerBytes[1];
            var headertextBytes = new byte[textLength];
            Buffer.BlockCopy(headerBytes, 2, headertextBytes, 0, textLength);

            return headertextBytes;
        }

        /// <summary>
        /// Gets header text from specific stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Header text</returns>       
        public static string GetHeaderText(Stream stream)
        {
            var headerTextBytes = GetHeaderTextBytes(stream);

            return Encoding.UTF8.GetString(headerTextBytes, 0, headerTextBytes.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] GetHeaderParameters(string fileName)
        {
            string headerText = GetHeaderText(fileName);
            if (string.IsNullOrEmpty(headerText))
                return StringConstants.EMPTY_ARRAY;
            
            return headerText.Split(HeaderDelimiter[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string[] GetHeaderParameters(Stream stream)
        {
            string headerText = GetHeaderText(stream);
            if (string.IsNullOrEmpty(headerText))
                return StringConstants.EMPTY_ARRAY;

            return headerText.Split(HeaderDelimiter[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string TryGetHeaderText(string fileName, out Exception ex)
        {
            ex = null;
            try
            {
                return GetHeaderText(fileName);
            }
            catch (Exception e)
            {
                ex = e;
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string[] TryGetHeaderParameters(string fileName, out Exception error)
        {
            error = null;
            Stream fileStream = null;
            string[] parameters = StringConstants.EMPTY_ARRAY;
            try
            {
                fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                parameters = TryGetHeaderParameters(fileStream, out error);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                if (null != fileStream)
                    try
                    {
                        fileStream.Close();
                    }
                    catch
                    {

                    }
            }


            return parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string[] TryGetHeaderParameters(Stream stream, out Exception error)
        {
            string headerText;
            error = null;
            try
            {
                headerText = GetHeaderText(stream);
            }
            catch (Exception e)
            {
                error = e;
                return StringConstants.EMPTY_ARRAY;
            }

            if (null == headerText)
                return StringConstants.EMPTY_ARRAY;
            
            try
            {
                return headerText.Split(HeaderDelimiter[0]);
            }
            catch
            {
                return StringConstants.EMPTY_ARRAY;
            }
        }
    }
}
