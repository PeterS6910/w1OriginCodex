using System;
using System.IO;
using System.IO.Compression;
using JetBrains.Annotations;

namespace Contal.IwQuick.Sys

{
    /// <summary>
    /// Stream used specificaly by FilePacker to simplify unpacking (no need to create tmp ungziped file
    /// </summary>
    public class FilePackerStream : GZipStream
    {
        //private readonly GZipStream _gzipStream;
        private readonly int _headerSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="headerSize"></param>
        public FilePackerStream(Stream stream, CompressionMode mode, int headerSize) : base(stream, mode)
        {
            _headerSize = headerSize;
            ReadUncompressedSize(stream);
            ReadHeader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="leaveOpen"></param>
        /// <param name="headerSize"></param>
        public FilePackerStream(Stream stream, CompressionMode mode, bool leaveOpen, int headerSize)
            : base(stream, mode, leaveOpen)
        {
            _headerSize = headerSize;
            ReadUncompressedSize(stream);
            ReadHeader();
        }

        public const byte MagicNumber0 = 31;
        public const byte MagicNumber1 = 139;
        public const byte MagicNumber2 = 8;

        private void ReadUncompressedSize([NotNull] Stream stream)
        {
            try
            {
                if (!stream.CanSeek || !stream.CanRead)
                {
                    OriginalFileSize = -1;
                    return;
                }

                try
                {
                    var fh = new byte[3];
                    stream.Read(fh, 0, 3);
                    if (fh[0] == MagicNumber0 && fh[1] == MagicNumber1 && fh[2] == MagicNumber2) //If magic numbers are 31 and 139 and the deflation id is 8 then...
                    {
                        const int sizeofInt = sizeof (Int32);
                        var ba = new byte[sizeofInt];
                        stream.Seek(-1*sizeofInt, SeekOrigin.End);
                        stream.Read(ba, 0, sizeofInt);
                        OriginalFileSize = BitConverter.ToInt32(ba, 0);
                    }
                    else
                        OriginalFileSize = -1;
                }
                finally
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch
            {
                OriginalFileSize = -1;
            }
        }

        private void ReadHeader()
        {
            HeaderBytes = new byte[_headerSize];
            base.Read(HeaderBytes, 0, _headerSize);

            var checkSumBytes = new byte[4];
            base.Read(checkSumBytes, 0, checkSumBytes.Length);
            Checksum = (uint)(checkSumBytes[0] | (checkSumBytes[1] << 8) | (checkSumBytes[2] << 16) | (checkSumBytes[3] << 24));
        }

        /// <summary>
        /// Checksum read from the GZipStream (read directly after FilePackerStream created)
        /// </summary>
        public uint Checksum { get; internal set; }
        
        /// <summary>
        /// Header bytes read from the GZipStream (read directly after FilePackerStream created)
        /// </summary>
        public byte[] HeaderBytes { get; internal set; }

        /// <summary>
        /// Size of the original uncompressed file
        /// </summary>
        public int OriginalFileSize { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
