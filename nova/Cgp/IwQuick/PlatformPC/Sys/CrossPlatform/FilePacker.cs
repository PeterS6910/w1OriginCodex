using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;

#if !COMPACT_FRAMEWORK
using Contal.IwQuick.Crypto;
#else
using Contal.IwQuickCF.Crypto;
#endif

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Sys
#else
namespace Contal.IwQuickCF.Sys
#endif
{
    public class FileCorruptedException : Exception
    {
        public FileCorruptedException(string msg)
            : base(msg)
        {
        }

        public FileCorruptedException()
        {
        }
    }

    public class NotUpgradeFileException : Exception
    {
        public NotUpgradeFileException(string msg)
            : base(msg)
        {
        }

        public NotUpgradeFileException()
        {
        }
    }

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

        private BinaryWriter _outputStream;
        private BinaryReader _inputStream;

        private const string HEADER_DELIMITER = StringConstants.COMMA;

        public const string TMP_FILE_EXTENSION = ".tmp";
        private const string PACKED_FILE_EXTENSION = ".gz";
        private const int FILE_HEADER_FIXED_LENGTH = 64;

        private bool _deleteOriginalSource = true;

        /// <summary>
        /// Pack directory structure into single file and then runs gzipstream on it
        /// </summary>
        /// <param name="archiveName">first header parameter</param>
        /// <param name="source">source file or directory</param>
        /// <param name="outputFile">Output file</param>
        /// <param name="headerParts">additional parameters of the header</param>
        /// <returns></returns>
        public bool Pack(string archiveName, string source, string outputFile, params string[] headerParts)
        {
            try
            {
                bool result = false;

                FileStream file = new FileStream(outputFile + TMP_FILE_EXTENSION, FileMode.Create);
                _outputStream = new BinaryWriter(file);

                //Check if source is path to directory or file
                if (Directory.Exists(source))
                {
                    DirectoryInfo mainDir = new DirectoryInfo(source);
                    result = PackDirectory(mainDir);
                }
                else if (File.Exists(source))
                {
                    FileInfo fileInfo = new FileInfo(source);
                    result = PackFile(fileInfo);
                }

                _outputStream.Close();
                file.Close();

                // calculate checksum
                file = new FileStream(outputFile + TMP_FILE_EXTENSION, FileMode.Open);
                uint checksum = Crc32.ComputeChecksum(file);
                file.Close();

                // do some compression
                FileStream upgradeStream = File.OpenRead(outputFile + TMP_FILE_EXTENSION);
                GZipStream compressStream = new GZipStream(File.Create(outputFile),
                    CompressionMode.Compress);

                // write header
                BinaryWriter bWriter = new BinaryWriter(compressStream);

                byte[] headerBytes;

                string headerAllString = archiveName ?? String.Empty;

                if (headerParts != null && headerParts.Length != 0)
                {
                    foreach (string part in headerParts)
                    {
                        if (part == null)
                            continue;

                        //part.Replace(HEADER_DELIMITER, StringConstants.UNDERSCORE);

                        headerAllString +=
                            HEADER_DELIMITER + part;
                    }
                }

                headerBytes = CreateFileHeader(headerAllString);

                //bWriter.Write((byte)0xff);
                //byte[] textBytes = Encoding.UTF8.GetBytes("CAT12CE");
                //bWriter.Write(textBytes, 0, textBytes.Length);
                bWriter.Write(headerBytes);
                bWriter.Write(checksum);


                byte[] buffer = new byte[Int16.MaxValue];
                while (true)
                {
                    int read = upgradeStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        break;
                    compressStream.Write(buffer, 0, read);
                }

                upgradeStream.Close();
                compressStream.Close();
                File.Delete(outputFile + TMP_FILE_EXTENSION);

                return result;
            }
            catch
            {
                //Console.WriteLine("Pack: " + error.Message);
                return false;
            }
        }

        /// <summary>
        /// Add content of single directory to file (also the dir name, etc..)
        /// </summary>
        /// <param name="directory">Object holding directory to be added</param>
        /// <returns>true if succesfull, false otherwise</returns>
        private bool PackDirectory(DirectoryInfo directory)
        {
            try
            {
                DirectoryInfo[] subDirectories = directory.GetDirectories();
                List<DirectoryInfo> excludedSubDirectories = GetExcludedSubDirectories(directory);
                FileInfo[] subFiles = directory.GetFiles();
                List<FileInfo> excludedSubFiles = GetExcludedSubFiles(directory);
                foreach (DirectoryInfo subDir in subDirectories)
                {
                    if (IsDirectoryExcluded(subDir, excludedSubDirectories))
                        continue;
                    _outputStream.Write((byte)0x02);
                    char[] dirName = subDir.Name.ToCharArray();
                    _outputStream.Write(dirName.Length);
                    _outputStream.Write(dirName);
                    _outputStream.Write(subDir.GetDirectories().Length +
                                        subDir.GetFiles().Length);
                    if (PackDirectory(subDir) == false)
                        return false;
                }

                foreach (FileInfo subFile in subFiles)
                {
                    if (IsFileExcluded(subFile, excludedSubFiles))
                        continue;
                    _outputStream.Write((byte)0x01);
                    char[] fileName = subFile.Name.ToCharArray();
                    _outputStream.Write(fileName.Length);
                    _outputStream.Write(fileName);
                    _outputStream.Write(subFile.Length);

                    FileStream fileToCopy = subFile.OpenRead();
                    while (true)
                    {
                        _outputStream.Write((byte)fileToCopy.ReadByte());
                        if (fileToCopy.Position >= fileToCopy.Length)
                            break;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Add content of file to file
        /// </summary>
        /// <param name="file">Object holding file to be added</param>
        /// <returns>true if succesfull, false otherwise</returns>
        private bool PackFile(FileInfo file)
        {
            try
            {
                _outputStream.Write((byte)0x01);
                char[] fileName = file.Name.ToCharArray();
                _outputStream.Write(fileName.Length);
                _outputStream.Write(fileName);
                _outputStream.Write(file.Length);


                FileStream fileToCopy = file.OpenRead();
                while (true)
                {
                    _outputStream.Write((byte)fileToCopy.ReadByte());
                    if (fileToCopy.Position >= fileToCopy.Length)
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }



        private bool IsFileExcluded(FileInfo subFile, IEnumerable<FileInfo> excludedSubFiles)
        {
            foreach (FileInfo fileInfo in excludedSubFiles)
            {
                if (subFile.FullName == fileInfo.FullName)
                    return true;
            }
            return false;
        }

        private bool IsDirectoryExcluded(DirectoryInfo subDir, IEnumerable<DirectoryInfo> excludedSubDirectories)
        {
            foreach (DirectoryInfo dirInfo in excludedSubDirectories)
            {
                if (subDir.FullName == dirInfo.FullName)
                    return true;
            }
            return false;
        }

        private List<FileInfo> GetExcludedSubFiles(DirectoryInfo directory)
        {
            List<FileInfo> result = new List<FileInfo>();
            if (_excludes == null)
                return result;
            foreach (string pattern in _excludes)
            {
                try
                {
                    foreach (FileInfo fileInfo in directory.GetFiles(pattern))
                    {
                        if (!result.Contains(fileInfo))
                            result.Add(fileInfo);
                    }
                }
                catch { }
            }
            return result;
        }

        private List<DirectoryInfo> GetExcludedSubDirectories(DirectoryInfo directory)
        {
            List<DirectoryInfo> result = new List<DirectoryInfo>();
            if (_excludes == null)
                return result;
            foreach (string pattern in _excludes)
            {
                try
                {
                    foreach (DirectoryInfo dirInfo in directory.GetDirectories(pattern))
                    {
                        if (!result.Contains(dirInfo))
                            result.Add(dirInfo);
                    }
                }
                catch { }
            }
            return result;
        }


        public event DInt2Void UpgradeProgressChanged;

        private DirectoryInfo _parentDirectory;

        public void Unpack(string inputFile, string outputDirectory)
        {
            Validator.CheckNull(inputFile, outputDirectory);

            Unpack(inputFile, null, outputDirectory);
        }

        public void Unpack(Stream inputStream, string outputDirectory)
        {
            Validator.CheckNull(inputStream, outputDirectory);

            Unpack(null, inputStream, outputDirectory);
        }

        /// <summary>
        /// Depack single file into original file tree structure
        /// </summary>
        /// <param name="inputFile">Packed filename</param>
        /// <param name="inputStream"></param>
        /// <param name="outputDirectory">Name of the directory that will hold depacked files</param>
        /// <returns>true if all successfull, false otherwise</returns>

        private void Unpack(string inputFile, Stream inputStream, string outputDirectory, bool deleteOriginalSource)
        {
            _deleteOriginalSource = deleteOriginalSource;
            Unpack(inputFile,inputStream,outputDirectory);
        }

        private void Unpack(string inputFile, Stream inputStream, string outputDirectory)
        {
            GZipStream decompressStream;
            if (inputStream != null)
            {
                inputStream.Position = 0;
                decompressStream = new GZipStream(inputStream, CompressionMode.Decompress, true);
            }
            else
                decompressStream = new GZipStream(File.Open(inputFile,
                   FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress);

            if (UpgradeProgressChanged != null)
                UpgradeProgressChanged(0);

            string inputWithTempExtension = inputFile;
            inputWithTempExtension = Path.ChangeExtension(inputWithTempExtension, TMP_FILE_EXTENSION);

            _parentDirectory = new DirectoryInfo(outputDirectory);

            if (File.Exists(inputWithTempExtension))
                try { File.Delete(inputWithTempExtension); }
                catch { }

            FileStream upgradeStream = File.OpenWrite(inputWithTempExtension);

            byte[] headerBuffer = new byte[FILE_HEADER_FIXED_LENGTH];
            // read header
            decompressStream.Read(headerBuffer, 0, FILE_HEADER_FIXED_LENGTH);
            // check whether it is cat12ce upgrade file
            //byte[] textBytes = Encoding.UTF8.GetBytes("CAT12CE");
            bool allCorrect = (headerBuffer[0] == 0xff);
            byte headerStringLength = headerBuffer[1];

            byte[] checkSumBytes = new byte[4];
            decompressStream.Read(checkSumBytes, 0, checkSumBytes.Length);
            uint readChecksum = (uint)(checkSumBytes[0] | (checkSumBytes[1] << 8) | (checkSumBytes[2] << 16) | (checkSumBytes[3] << 24));
            string headerString = Encoding.ASCII.GetString(headerBuffer, 2, headerStringLength);
            DebugHelper.NOP(headerString);
            //for (int i = 1; i < 7; i++)
            //{
            //    if (textBytes[i] != headerBuffer[i + 1])
            //    {
            //        allCorrect = false;
            //        break;
            //    }
            //}
            byte[] readBuffer = new byte[Int16.MaxValue];
            if (!allCorrect)
            {
                //throw new NotUpgradeFileException("Not a CAT12CE upgrade file or file corrupted");
                throw new NotUpgradeFileException("Not a valid upgrade file or file corrupted");
            }

            while (true)
            {
                int read = decompressStream.Read(readBuffer, 0, readBuffer.Length);
                if (read <= 0)
                    break;
                upgradeStream.Write(readBuffer, 0, read);
                if (UpgradeProgressChanged != null)
                    UpgradeProgressChanged((int)(((double)decompressStream.BaseStream.Position / decompressStream.BaseStream.Length) * 50));
            }
            upgradeStream.Close();
            decompressStream.Close();

            // now delete original file
            if (_deleteOriginalSource)
            {
                try
                {
                    File.Delete(inputFile);
                }
                catch
                {
                }
            }
            _deleteOriginalSource = true;

            // calculate checksum
            FileStream file = new FileStream(inputWithTempExtension, FileMode.Open);

            uint checksum = Crc32.ComputeChecksum(file);
            file.Close();

            if (checksum != readChecksum)
            {
                throw new FileCorruptedException("Read and calculated checksum does not match. File corrupted.");
            }

            DirectoryInfo mainDir = new DirectoryInfo(outputDirectory);
            mainDir.Create();

            file = new FileStream(inputWithTempExtension, FileMode.Open);
            _inputStream = new BinaryReader(file);

            if (UpgradeProgressChanged != null)
                UpgradeProgressChanged(50);

            while (true)
            {
                byte type = _inputStream.ReadByte();
                if (type == 0x01)
                {
                    UnpackFile();
                }
                else if (type == 0x02)
                {
                    _parentDirectory = mainDir;
                    UnpackDirectory();
                }
                else
                    throw new Exception("Unsuported mark: " + type);

                if (UpgradeProgressChanged != null)
                    UpgradeProgressChanged((int)(((double)_inputStream.BaseStream.Position / _inputStream.BaseStream.Length) * 50) + 50);

                if (_inputStream.BaseStream.Position >= _inputStream.BaseStream.Length)
                    break;
            }

            _inputStream.Close();

            try { File.Delete(inputWithTempExtension); }
            catch
            {
            }

        }

        public bool TryUnpack(string inputFile, Stream inputStream, string outputDirectory, out Exception error)
        {
            try
            {
                Unpack(inputFile, inputStream, outputDirectory);

                error = null;

                return true;
            }
            catch (Exception e)
            {
                error = e;

                return false;
            }
        }

        public bool TryUnpack(string inputFile, Stream inputStream, string outputDirectory, out Exception error, bool deleteOriginalSource)
        {
            try
            {
                Unpack(inputFile, inputStream, outputDirectory, deleteOriginalSource);

                error = null;

                return true;
            }
            catch (Exception e)
            {
                error = e;

                return false;
            }
        }

        private string[] _excludes;

        public void SetExcludes(string[] excludes)
        {
            _excludes = excludes;
        }

        public bool TryUnpack(string inputFile, Stream inputStream, string outputDirectory)
        {
            Exception e;
            return TryUnpack(inputFile, inputStream, outputDirectory, out e);
        }



        public event D2Type2Void<bool, string> UnpackingProgress;
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
        private void UnpackDirectory()
        {

            Int32 nameLength = _inputStream.ReadInt32();
            char[] dirName = _inputStream.ReadChars(nameLength);

            string dirPath = _parentDirectory.FullName + "\\" + new string(dirName);
            InvokeUnpackingProgress(false, dirPath);

            System.Diagnostics.Debug.WriteLine("Unpacking directory : " + dirPath);

            _parentDirectory = new DirectoryInfo(dirPath);
            _parentDirectory.Create();

            //Console.WriteLine("Recreating directory: " + new string(dirName));

            Int32 itemCount = _inputStream.ReadInt32();
            for (Int32 i = 0; i < itemCount; i++)
            {
                byte type = _inputStream.ReadByte();
                if (type == 0x01)
                {
                    UnpackFile();
                }
                else if (type == 0x02)
                {
                    UnpackDirectory();
                }
                else
                    throw new Exception("Unsuported mark: " + type);
            }
            _parentDirectory = _parentDirectory.Parent;


        }

        /// <summary>
        /// Depack single file
        /// </summary>
        /// <returns>true if successfull, false otherwise</returns>
        private void UnpackFile()
        {
            BinaryWriter binaryFile = null;
            try
            {
                Int32 nameLength = _inputStream.ReadInt32();
                char[] fileName = _inputStream.ReadChars(nameLength);
                string name = new string(fileName);
                DebugHelper.NOP(name);
                Int64 fileLength = _inputStream.ReadInt64();
                string filePath = _parentDirectory.FullName + "\\" +
                    new string(fileName);
                System.Diagnostics.Debug.WriteLine("Unpacking file : " + filePath);
                InvokeUnpackingProgress(true, filePath);
                binaryFile = new BinaryWriter(new FileStream(filePath, FileMode.Create));

                //Console.WriteLine("Recreating file: " + new string(fileName));

                for (int i = 0; i < fileLength; i++)
                {
                    binaryFile.Write(_inputStream.ReadByte());
                }
            }
            catch
            {
                if (binaryFile != null)
                {
                    try { binaryFile.Close(); }
                    catch { }
                }

                if (_inputStream != null)
                {
                    try { _inputStream.Close(); }
                    catch { }
                }

                throw;
            }
            binaryFile.Close();
        }

        private const int HEADER_PREFIX_LENGTH = 2; // 0xFF 0x?? - length of header

        /// <summary>
        /// Creates head of specific lenght
        /// </summary>
        /// <param name="headerString">String, which will be put into header</param>
        /// <returns>Header byte array of specific length</returns>
        private byte[] CreateFileHeader(string headerString)
        {
            byte[] headerBytes = new byte[FILE_HEADER_FIXED_LENGTH];
            headerBytes[0] = 0xFF;

            if (headerString.Length > FILE_HEADER_FIXED_LENGTH - HEADER_PREFIX_LENGTH)
                headerString = headerString.Remove(FILE_HEADER_FIXED_LENGTH - HEADER_PREFIX_LENGTH, headerString.Length - (FILE_HEADER_FIXED_LENGTH - HEADER_PREFIX_LENGTH));

            //append header string length
            headerBytes[1] = (byte)(headerString.Length);

            //append header string
            byte[] headerStringBytes = Encoding.ASCII.GetBytes(headerString);
            Buffer.BlockCopy(headerStringBytes, 0, headerBytes, HEADER_PREFIX_LENGTH, headerStringBytes.Length);
            return headerBytes;
        }

        /// <summary>
        /// Gets header text from specific file
        /// </summary>
        /// <param name="fileName">Full file name</param>
        /// <returns>Header text</returns>       
        public static string GetHeaderText(string fileName)
        {
            string result = string.Empty;
            if (!File.Exists(fileName))
                return result;

            Stream fileStream;
            // let the exception be thrown 
            fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            string headerText = GetHeaderText(fileStream);
            fileStream.Close();

            return headerText;
        }

        /// <summary>
        /// Gets header text from specific stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Header text</returns>       
        public static string GetHeaderText(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            GZipStream decompressStream = null;
            try
            {
                decompressStream = new GZipStream(stream, CompressionMode.Decompress, true);
            }
            catch (Exception)
            {
                if (decompressStream != null)
                    try
                    {
                        decompressStream.Close();
                    }
                    catch { }
                throw;
            }

            byte[] headerBytes = new byte[FILE_HEADER_FIXED_LENGTH];
            int bytesRead = 0;
            Exception error = null;

            try
            {
                bytesRead = decompressStream.Read(headerBytes, 0, FILE_HEADER_FIXED_LENGTH);
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                // R# suggesting this always be true
                //if (decompressStream != null)
                    try
                    {
                        decompressStream.Close();
                    }
                    catch { }
            }

            if (error != null)
                throw error;

            if (bytesRead != FILE_HEADER_FIXED_LENGTH)
            {
                throw new InvalidDataException("File header has wrong length");
            }

            byte textLength = headerBytes[1];
            byte[] headertextBytes = new byte[textLength];
            Buffer.BlockCopy(headerBytes, 2, headertextBytes, 0, textLength);
            decompressStream.Close();
            return Encoding.ASCII.GetString(headertextBytes, 0, headertextBytes.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] GetHeaderParameters(string fileName)
        {
            string headerText = GetHeaderText(fileName);
            if (null == headerText)
                return StringConstants.EMPTY_ARRAY;
            
            return headerText.Split(HEADER_DELIMITER[0]);
        }

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

        public static string[] TryGetHeaderParameters(string fileName, out Exception error)
        {
            error = null;
            try
            {
                Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                return TryGetHeaderParameters(fileStream, out error);
            }
            catch (Exception)
            { }

            return StringConstants.EMPTY_ARRAY;
        }

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
                return new string[0];
            
            try
            {
                return headerText.Split(HEADER_DELIMITER[0]);
            }
            catch
            {
                return StringConstants.EMPTY_ARRAY;
            }
        }
    }
}
