using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    public class Cpk
    {
        /// <summary>
        /// </summary>
        public List<FileEntry> FileTable;

        /// <summary>
        /// </summary>
        public Dictionary<string, object> CpkData;

        /// <summary>
        /// </summary>
        public Utf Utf;

        /// <summary>
        /// </summary>
        private Utf _files;

        /// <summary>
        /// </summary>
        /// <param name="cpkFilename"></param>
        public Cpk(string cpkFilename)
        {
            IsUtfEncrypted = false;

            if (!File.Exists(cpkFilename))
            {
                throw new FileNotFoundException(cpkFilename);
            }

            var reader = new EndianReader(File.OpenRead(cpkFilename), true);

            if (Tools.ReadCString(reader, 4) != "CPK ")
            {
                reader.Close();
                throw new Exception();
            }

            Utf = new Utf();
            ReadUtfData(reader);
            CpkPacket = UtfPacket;

            var cpakEntry = new FileEntry
            {
                FileType = "CPK",
                FileName = "CPK_HDR",
                Encrypted = IsUtfEncrypted,
                FileSize = CpkPacket.Length,
                FileOffsetPosition = reader.BaseStream.Position + 0x10
            };  

            FileTable = new List<FileEntry>
            {
                cpakEntry
            };

            using (var stream = new MemoryStream(UtfPacket))
            using (var utfReader = new EndianReader(stream, false))
            {
                if (!Utf.ReadUtf(utfReader))
                {
                    reader.Close();
                    throw new Exception();
                }
            }

            CpkData = new Dictionary<string, object>();

            for (var columnIndex = 0; columnIndex < Utf.Columns.Count; columnIndex++)
            {
                CpkData.Add(Utf.Columns[columnIndex].Name, Utf.Rows[0][columnIndex].GetValue());
            }

            TocOffset = (ulong)GetColumsData2(Utf, 0, "tocOffset", 3);
            EtocOffset = (ulong)GetColumsData2(Utf, 0, "EtocOffset", 3);
            GtocOffset = (ulong)GetColumsData2(Utf, 0, "GtocOffset", 3);
            ItocOffset = (ulong)GetColumsData2(Utf, 0, "ItocOffset", 3);
            ContentOffset = (ulong)GetColumsData2(Utf, 0, "contentOffset", 3);

            var tocOffset = GetColumnPostion(Utf, 0, "tocOffset");
            var eTocOffset = GetColumnPostion(Utf, 0, "EtocOffset");
            var itocOffset = GetColumnPostion(Utf, 0, "ItocOffset");
            var gtocOffset = GetColumnPostion(Utf, 0, "GtocOffset");
            var contentOffset = GetColumnPostion(Utf, 0, "contentOffset");

            FileTable.Add(CreateFileEntry("CONTENT_OFFSET", ContentOffset, typeof(ulong), contentOffset, "CPK", "CONTENT", false));

            if (TocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                FileTable.Add(CreateFileEntry("TOC_HDR", TocOffset, typeof(ulong), tocOffset, "CPK", "HDR", false));
                ReadToc(reader, TocOffset, ContentOffset);
            }

            if (EtocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                FileTable.Add(CreateFileEntry("ETOC_HDR", EtocOffset, typeof(ulong), eTocOffset, "CPK", "HDR", false));
                ReadEtoc(reader, (long)EtocOffset);
            }

            if (ItocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                FileTable.Add(CreateFileEntry("ITOC_HDR", ItocOffset, typeof(ulong), itocOffset, "CPK", "HDR", false));
                ReadItoc(reader, ItocOffset, ContentOffset, (ushort)GetColumsData2(Utf, 0, "Align", 1));
            }

            if (GtocOffset != 0xFFFFFFFFFFFFFFFF)
            {
                FileTable.Add(CreateFileEntry("GTOC_HDR", GtocOffset, typeof(ulong), gtocOffset, "CPK", "HDR", false));
                ReadGtoc(reader, (long)GtocOffset);
            }

            reader.Close();
            _files = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileOffset"></param>
        /// <param name="fileOffsetType"></param>
        /// <param name="fileOffsetPos"></param>
        /// <param name="tocName"></param>
        /// <param name="fileType"></param>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        private static FileEntry CreateFileEntry(string fileName, ulong fileOffset, Type fileOffsetType, long fileOffsetPos, string tocName, string fileType, bool encrypted) => new FileEntry
        {
            FileName = fileName,
            FileOffset = fileOffset,
            FileOffsetType = fileOffsetType,
            FileOffsetPosition = fileOffsetPos,
            TocName = tocName,
            FileType = fileType,
            Encrypted = encrypted
        };

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tocOffset"></param>
        /// <param name="contentOffset"></param>
        /// <returns></returns>
        public bool ReadToc(EndianReader reader, ulong tocOffset, ulong contentOffset)
        {
            var addOffset = tocOffset;

            if (contentOffset < tocOffset)
            {
                addOffset = contentOffset;
            }


            reader.BaseStream.Seek((long)tocOffset, SeekOrigin.Begin);

            if (Tools.ReadCString(reader, 4) != "TOC ")
            {
                reader.Close();
                return false;
            }

            ReadUtfData(reader);
            TocPacket = UtfPacket;

            var tocEntry = FileTable.Single(entry => entry.FileName.ToString() == "TOC_HDR");

            _files = new Utf();
            tocEntry.Encrypted = IsUtfEncrypted;
            tocEntry.FileSize = TocPacket.Length;

            using (var stream = new MemoryStream(UtfPacket))
            using (var endianReader = new EndianReader(stream, false))
            {
                if (_files.ReadUtf(endianReader) == false)
                {
                    reader.Close();
                    return false;
                }
            }

            for (var index = 0; index < _files.RowCount; index++)
            {
                FileTable.Add(new FileEntry
                {
                    TocName = "TOC",
                    FileType = "FILE",
                    Offset = addOffset,
                    DirectoryName = GetColumnData(_files, index, "DirName"),
                    FileName = GetColumnData(_files, index, "FileName"),
                    FileSize = GetColumnData(_files, index, "FileSize"),
                    FileSizePosition = GetColumnPostion(_files, index, "FileSize"),
                    FileSizeType = GetColumnType(_files, index, "FileSize"),
                    ExtractSize = GetColumnData(_files, index, "ExtractSize"),
                    ExtractSizePosition = GetColumnPostion(_files, index, "ExtractSize"),
                    ExtractSizeType = GetColumnType(_files, index, "ExtractSize"),
                    FileOffset = (ulong)GetColumnData(_files, index, "FileOffset") + addOffset,
                    FileOffsetPosition = GetColumnPostion(_files, index, "FileOffset"),
                    FileOffsetType = GetColumnType(_files, index, "FileOffset"),
                    Identifier = GetColumnData(_files, index, "Identifier"),
                    UserString = GetColumnData(_files, index, "UserString")
                });
            }

            _files = null;

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="startOffset"></param>
        /// <param name="contentOffset"></param>
        /// <param name="byteAlignment"></param>
        /// <returns></returns>
        public bool ReadItoc(EndianReader reader, ulong startOffset, ulong contentOffset, ushort byteAlignment)
        {
            reader.BaseStream.Seek((long)startOffset, SeekOrigin.Begin);

            if (Tools.ReadCString(reader, 4) != "ITOC")
            {
                reader.Close();
                return false;
            }

            _files = new Utf();
            ReadUtfData(reader);
            ItocPacket = UtfPacket;

            var itocEntry = FileTable.Single(x => x.FileName.ToString() == "ITOC_HDR");

            itocEntry.Encrypted = IsUtfEncrypted;
            itocEntry.FileSize = ItocPacket.Length;

            var stream = new MemoryStream(UtfPacket);
            var endianReader = new EndianReader(stream, false);

            if (!_files.ReadUtf(endianReader))
            {
                reader.Close();
                return false;
            }

            endianReader.Close();
            stream.Close();

            var dataL = (byte[])GetColumnData(_files, 0, "DataL");
            var dataLPosition = GetColumnPostion(_files, 0, "DataL");
            var dataH = (byte[])GetColumnData(_files, 0, "DataH");
            var dataHPos = GetColumnPostion(_files, 0, "DataH");
            var identifiers = new List<int>();
            var sizeTable = new Dictionary<int, uint>();
            var sizePositionTable = new Dictionary<int, long>();
            var sizeTypeTable = new Dictionary<int, Type>();
            var cSizeTable = new Dictionary<int, uint>();
            var cSizePosTable = new Dictionary<int, long>();
            var cSizeTypeTable = new Dictionary<int, Type>();

            if (dataL != null)
            {
                var utfDataL = new Utf();

                using (var streamL = new MemoryStream(dataL))
                using (var endianReaderL = new EndianReader(streamL, false))
                {
                    utfDataL.ReadUtf(endianReaderL);
                }

                for (var rowIndex = 0; rowIndex < utfDataL.RowCount; rowIndex++)
                {
                    var identifier = (ushort)GetColumnData(utfDataL, rowIndex, "Id");

                    sizeTable.Add(identifier, (ushort)GetColumnData(utfDataL, rowIndex, "FileSize"));
                    sizePositionTable.Add(identifier, GetColumnPostion(utfDataL, rowIndex, "FileSize") + dataLPosition);
                    sizeTypeTable.Add(identifier, GetColumnType(utfDataL, rowIndex, "FileSize"));

                    if (GetColumnData(utfDataL, rowIndex, "ExtractSize") != null)
                    {
                        cSizeTable.Add(identifier, (ushort)GetColumnData(utfDataL, rowIndex, "ExtractSize"));
                        cSizePosTable.Add(identifier, GetColumnPostion(utfDataL, rowIndex, "ExtractSize") + dataLPosition);
                        cSizeTypeTable.Add(identifier, GetColumnType(utfDataL, rowIndex, "ExtractSize"));
                    }

                    identifiers.Add(identifier);
                }
            }

            if (dataH != null)
            {
                var utfDataH = new Utf();

                using (var streamH = new MemoryStream(dataH))
                using (var endianReaderH = new EndianReader(streamH, false))
                {
                    utfDataH.ReadUtf(endianReaderH);
                }

                for (var rowIndex = 0; rowIndex < utfDataH.RowCount; rowIndex++)
                {
                    var identifier = (ushort)GetColumnData(utfDataH, rowIndex, "Identifier");

                    sizeTable.Add(identifier, (uint)GetColumnData(utfDataH, rowIndex, "FileSize"));
                    sizePositionTable.Add(identifier, GetColumnPostion(utfDataH, rowIndex, "FileSize") + dataHPos);
                    sizeTypeTable.Add(identifier, GetColumnType(utfDataH, rowIndex, "FileSize"));

                    if (GetColumnData(utfDataH, rowIndex, "ExtractSize") != null)
                    {
                        cSizeTable.Add(identifier, (uint)GetColumnData(utfDataH, rowIndex, "ExtractSize"));
                        cSizePosTable.Add(identifier, GetColumnPostion(utfDataH, rowIndex, "ExtractSize") + dataHPos);
                        cSizeTypeTable.Add(identifier, GetColumnType(utfDataH, rowIndex, "ExtractSize"));
                    }

                    identifiers.Add(identifier);
                }
            }

            var baseOffset = contentOffset;

            foreach (var id in identifiers.OrderBy(identifier => identifier))
            {
                var fileEntry = new FileEntry();

                sizeTable.TryGetValue(id, out var fileSize);
                cSizeTable.TryGetValue(id, out var extractSize);
                fileEntry.TocName = "ITOC";
                fileEntry.DirectoryName = null;
                fileEntry.FileName = id.ToString("D4");
                fileEntry.FileSize = fileSize;
                fileEntry.FileSizePosition = sizePositionTable[id];
                fileEntry.FileSizeType = sizeTypeTable[id];

                if (cSizeTable.Count > 0 && cSizeTable.ContainsKey(id))
                {
                    fileEntry.ExtractSize = extractSize;
                    fileEntry.ExtractSizePosition = cSizePosTable[id];
                    fileEntry.ExtractSizeType = cSizeTypeTable[id];
                }

                fileEntry.FileType = "FILE";
                fileEntry.FileOffset = baseOffset;
                fileEntry.Identifier = id;
                fileEntry.UserString = null;
                FileTable.Add(fileEntry);

                if (fileSize % byteAlignment > 0)
                {
                    baseOffset += fileSize + (byteAlignment - fileSize % byteAlignment);
                }
                else
                {
                    baseOffset += fileSize;
                }
            }

            _files = null;
            stream.Close();
            endianReader.Close();

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        private void ReadUtfData(EndianReader reader)
        {
            IsUtfEncrypted = false;
            reader.LittleEndian = true;
            Unknown1 = reader.ReadInt32();
            UtfSize = reader.ReadInt64();
            UtfPacket = reader.ReadBytes((int)UtfSize);

            if (UtfPacket[0] != 0x40 && UtfPacket[1] != 0x55 && UtfPacket[2] != 0x54 && UtfPacket[3] != 0x46)
            {
                UtfPacket = DecryptUtf(UtfPacket);
                IsUtfEncrypted = true;
            }

            reader.LittleEndian = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool ReadGtoc(EndianReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            var operationResult = Tools.ReadCString(reader, 4) == "GTOC";

            if (operationResult)
            {
                reader.BaseStream.Seek(0xC, SeekOrigin.Current);
            }
            else
            {
                reader.Close();
            }

            return operationResult;
        }

        /// <summary> 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool ReadEtoc(EndianReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            if (Tools.ReadCString(reader, 4) != "ETOC")
            {
                reader.Close();
                return false;
            }

            ReadUtfData(reader);
            EtocPacket = UtfPacket;

            var etocEntry = FileTable.Single(x => x.FileName.ToString() == "ETOC_HDR");

            etocEntry.Encrypted = IsUtfEncrypted;
            etocEntry.FileSize = EtocPacket.Length;

            var stream = new MemoryStream(UtfPacket);
            var endianReader = new EndianReader(stream, false);

            _files = new Utf();

            if (!_files.ReadUtf(endianReader))
            {
                reader.Close();
                return false;
            }

            endianReader.Close();
            stream.Close();

            var fileEntries = FileTable.Where(fileEntry => fileEntry.FileType == "FILE").ToList();

            for (var entryIndex = 0; entryIndex < fileEntries.Count; entryIndex++)
            {
                FileTable[entryIndex].LocalDirectory = GetColumnData(_files, entryIndex, "LocalDir");
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] DecryptUtf(byte[] input)
        {
            var m = 0x0000655f;
            var result = new byte[input.Length];

            for (var index = 0; index < input.Length; index++)
            {
                var d = input[index];
                result[index] = (byte)(d ^ (byte)(m & 0xFF));
                m *= 0x00004115;
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] DecompressCrilayla(byte[] buffer, int length)
        {
            var stream = new MemoryStream(buffer);
            var endianReader = new EndianReader(stream, true);

            endianReader.BaseStream.Seek(8, SeekOrigin.Begin);

            var uncompressedLength = endianReader.ReadInt32();
            var uncompressedHeaderOffset = endianReader.ReadInt32();
            var result = new byte[uncompressedLength + 0x100];

            Array.Copy(buffer, uncompressedHeaderOffset + 0x10, result, 0, 0x100);

            var inputEnd = buffer.Length - 0x100 - 1;
            var inputOffset = inputEnd;
            var outputEnd = 0x100 + uncompressedLength - 1;
            byte bitPool = 0;
            int bitsLeft = 0, bytesOutput = 0;
            var vleLens = new int[4] { 2, 3, 5, 8 };

            while (bytesOutput < uncompressedLength)
            {
                if (RetrieveNextBits(buffer, ref inputOffset, ref bitPool, ref bitsLeft, 1) > 0)
                {
                    var backreferenceOffset = outputEnd - bytesOutput + RetrieveNextBits(buffer, ref inputOffset, ref bitPool, ref bitsLeft, 13) + 3;
                    var backreferenceLength = 3;

                    int vleLevel;

                    for (vleLevel = 0; vleLevel < vleLens.Length; vleLevel++)
                    {
                        int thisLevel = RetrieveNextBits(buffer, ref inputOffset, ref bitPool, ref bitsLeft, vleLens[vleLevel]);

                        backreferenceLength += thisLevel;

                        if (thisLevel != (1 << vleLens[vleLevel]) - 1)
                        {
                            break;
                        }
                    }

                    if (vleLevel == vleLens.Length)
                    {
                        int thisLevel;

                        do
                        {
                            thisLevel = RetrieveNextBits(buffer, ref inputOffset, ref bitPool, ref bitsLeft, 8);
                            backreferenceLength += thisLevel;
                        } while (thisLevel == 255);
                    }

                    for (var index = 0; index < backreferenceLength; index++)
                    {
                        result[outputEnd - bytesOutput] = result[backreferenceOffset--];
                        bytesOutput++;
                    }
                }
                else
                {
                    result[outputEnd - bytesOutput] = (byte)RetrieveNextBits(buffer, ref inputOffset, ref bitPool, ref bitsLeft, 8);
                    bytesOutput++;
                }
            }

            endianReader.Close();
            stream.Close();

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <param name="bitPool"></param>
        /// <param name="bitsLeft"></param>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        private static ushort RetrieveNextBits(IList<byte> input, ref int offset, ref byte bitPool, ref int bitsLeft, int bitCount)
        {
            ushort outputBits = 0;
            var bitsProduced = 0;

            while (bitsProduced < bitCount)
            {
                if (bitsLeft == 0)
                {
                    bitPool = input[offset];
                    bitsLeft = 8;
                    offset--;
                }

                int bitsThisRound;

                if (bitsLeft > bitCount - bitsProduced)
                {
                    bitsThisRound = bitCount - bitsProduced;
                }
                else
                {
                    bitsThisRound = bitsLeft;
                }

                outputBits <<= bitsThisRound;
                outputBits |= (ushort)((ushort)(bitPool >> (bitsLeft - bitsThisRound)) & ((1 << bitsThisRound) - 1));
                bitsLeft -= bitsThisRound;
                bitsProduced += bitsThisRound;
            }

            return outputBits;
        }

        /// <summary>
        /// </summary>
        /// <param name="utf"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetColumsData2(Utf utf, int row, string name, int type)
        {
            switch (GetColumnData(utf, row, name))
            {
            case null:

                switch (type)
                {
                case 0:
                    return (byte)0xFF;
                case 1:
                    return (ushort)0xFFFF;
                case 2:
                    return 0xFFFFFFFF;
                case 3:
                    return 0xFFFFFFFFFFFFFFFF;
                default:
                    return 0;
                }

            case ulong valueUlong:

                return valueUlong;

            case uint valueUint:

                return valueUint;

            case ushort valueUshort:

                return valueUshort;

            default:

                return 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="utf"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetColumnData(Utf utf, int row, string name)
        {
            for (var column = 0; column < utf.ColumnCount; column++)
            {
                if (utf.Columns[column].Name == name)
                {
                    return utf.Rows[row][column].GetValue();
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="utf"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetColumnPostion(Utf utf, int row, string name)
        {
            for (var column = 0; column < utf.ColumnCount; column++)
            {
                if (utf.Columns[column].Name == name)
                {
                    return utf.Rows[row][column].Position;
                }
            }

            return -1;
        }

        /// <summary>
        /// </summary>
        /// <param name="utf"></param>
        /// <param name="row"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type GetColumnType(Utf utf, int row, string name)
        {
            for (var column = 0; column < utf.ColumnCount; column++)
            {
                if (utf.Columns[column].Name == name)
                {
                    return utf.Rows[row][column].GetType();
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        public bool IsUtfEncrypted { get; set; }

        /// <summary>
        /// </summary>
        public int Unknown1 { get; set; }

        /// <summary>
        /// </summary>
        public long UtfSize { get; set; }

        /// <summary>
        /// </summary>
        public byte[] UtfPacket { get; set; }

        /// <summary>
        /// </summary>
        public byte[] CpkPacket { get; set; }

        /// <summary>
        /// </summary>
        public byte[] TocPacket { get; set; }

        /// <summary>
        /// </summary>
        public byte[] ItocPacket { get; set; }

        /// <summary>
        /// </summary>
        public byte[] EtocPacket { get; set; }

        /// <summary>
        /// </summary>
        public ulong TocOffset;

        /// <summary>
        /// </summary>
        public ulong EtocOffset;

        /// <summary>
        /// </summary>
        public ulong ItocOffset;

        /// <summary>
        /// </summary>
        public ulong GtocOffset;

        /// <summary>
        /// </summary>
        public ulong ContentOffset;
    }
}