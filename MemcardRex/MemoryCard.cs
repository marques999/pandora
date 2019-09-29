using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MemcardRex
{
    /// <summary>
    /// </summary>
    internal class MemoryCard
    {
        /// <summary>
        /// </summary>
        private const int MemoryCardSlots = 15;

        /// <summary>
        /// </summary>
        private const int MemoryCardRawLength = 131072;

        /// <summary>
        /// </summary>
        private const int GmeHeaderLength = 3904;

        /// <summary>
        /// </summary>
        private const int SlotHeaderLength = 128;

        /// <summary>
        /// </summary>
        private const int SlotRawLength = 8192;

        /// <summary>
        /// </summary>
        private const int IconRawLength = 416;

        /// <summary>
        /// </summary>
        private const int IconDimensions = 16;

        /// <summary>
        /// </summary>
        private const int IconPaletteLength = 16;

        /// <summary>
        /// </summary>
        private const int GmeCommentsLength = 256;

        /// <summary>
        /// </summary>
        private readonly ShiftJisConverter _sjisc = new ShiftJisConverter();

        /// <summary>
        /// </summary>
        private byte[] _rawMemoryCard = new byte[MemoryCardRawLength];

        /// <summary>
        /// </summary>
        public string CardLocation;

        /// <summary>
        /// </summary>
        public string CardName;

        /// <summary>
        /// </summary>
        public byte CardType;

        /// <summary>
        /// </summary>
        public byte[] GmeHeader = new byte[GmeHeaderLength];

        /// <summary>
        /// </summary>
        public byte[,] HeaderData = new byte[MemoryCardSlots, SlotHeaderLength];

        /// <summary>
        /// </summary>
        public Bitmap[,] IconData = new Bitmap[MemoryCardSlots, 3];

        /// <summary>
        /// </summary>
        public int[] IconFrames = new int[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public Color[,] IconPalette = new Color[MemoryCardSlots, IconPaletteLength];

        /// <summary>
        /// </summary>
        public string[] SaveComments = new string[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public byte[,] SaveData = new byte[MemoryCardSlots, SlotRawLength];

        /// <summary>
        /// </summary>
        public string[] SaveIdentifier = new string[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public string[,] SaveName = new string[MemoryCardSlots, 2];

        /// <summary>
        /// </summary>
        public string[] SaveProdCode = new string[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public ushort[] SaveRegion = new ushort[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public int[] SaveSize = new int[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public byte[] SaveType = new byte[MemoryCardSlots];

        /// <summary>
        /// </summary>
        public bool WasChanged;

        /// <summary>
        /// </summary>
        private void LoadDataFromRawCard()
        {
            for (var slotNumber = 0; slotNumber < MemoryCardSlots; slotNumber++)
            {
                for (var currentByte = 0; currentByte < SlotHeaderLength; currentByte++)
                    HeaderData[slotNumber, currentByte] =
                        _rawMemoryCard[SlotHeaderLength + slotNumber * SlotHeaderLength + currentByte];

                for (var currentByte = 0; currentByte < SlotRawLength; currentByte++)
                    SaveData[slotNumber, currentByte] =
                        _rawMemoryCard[SlotRawLength + slotNumber * SlotRawLength + currentByte];
            }
        }

        /// <summary>
        /// </summary>
        private void LoadDataToRawCard()
        {
            _rawMemoryCard = new byte[MemoryCardRawLength];
            _rawMemoryCard[0x00] = 0x4D;
            _rawMemoryCard[0x01] = 0x43;
            _rawMemoryCard[0x7F] = 0x0E;
            _rawMemoryCard[0x1F80] = 0x4D;
            _rawMemoryCard[0x1F81] = 0x43;
            _rawMemoryCard[0x1FFF] = 0x0E;

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                for (var byteIndex = 0; byteIndex < SlotHeaderLength; byteIndex++)
                {
                    _rawMemoryCard[SlotHeaderLength + slotIndex * SlotHeaderLength + byteIndex] = HeaderData[slotIndex, byteIndex];
                }

                for (var byteIndex = 0; byteIndex < SlotRawLength; byteIndex++)
                {
                    _rawMemoryCard[SlotRawLength + slotIndex * SlotRawLength + byteIndex] = SaveData[slotIndex, byteIndex];
                }
            }

            for (var slotIndex = 0; slotIndex < 20; slotIndex++)
            {
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength] = 0xFF;
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength + 1] = 0xFF;
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength + 2] = 0xFF;
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength + 3] = 0xFF;
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength + 8] = 0xFF;
                _rawMemoryCard[2048 + slotIndex * SlotHeaderLength + 9] = 0xFF;
            }
        }

        /// <summary>
        /// </summary>
        private void FillGmeHeader()
        {
            GmeHeader = new byte[GmeHeaderLength];
            GmeHeader[0x00] = 0x31;
            GmeHeader[0x01] = 0x32;
            GmeHeader[0x02] = 0x33;
            GmeHeader[0x03] = 0x2D;
            GmeHeader[0x04] = 0x34;
            GmeHeader[0x05] = 0x35;
            GmeHeader[0x06] = 0x36;
            GmeHeader[0x07] = 0x2D;
            GmeHeader[0x08] = 0x53;
            GmeHeader[0x09] = 0x54;
            GmeHeader[0x0A] = 0x44;
            GmeHeader[0x12] = 0x1;
            GmeHeader[0x14] = 0x1;
            GmeHeader[0x15] = 0x4D;

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                GmeHeader[22 + slotIndex] = HeaderData[slotIndex, 0];
                GmeHeader[38 + slotIndex] = HeaderData[slotIndex, 8];

                var byteArray = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(SaveComments[slotIndex]));

                for (var byteIndex = 0; byteIndex < byteArray.Length; byteIndex++)
                {
                    GmeHeader[byteIndex + 64 + GmeCommentsLength * slotIndex] = byteArray[byteIndex];
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static byte[] GetVgsHeader()
        {
            var vgsHeader = new byte[64];

            vgsHeader[0] = 0x56;
            vgsHeader[1] = 0x67;
            vgsHeader[2] = 0x73;
            vgsHeader[3] = 0x4D;
            vgsHeader[4] = 0x01;
            vgsHeader[8] = 0x01;
            vgsHeader[12] = 0x01;
            vgsHeader[17] = 0x02;

            return vgsHeader;
        }

        private void LoadSlotTypes()
        {
            SaveType = new byte[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                switch (HeaderData[slotIndex, 0])
                {
                case 0xA0:
                    SaveType[slotIndex] = 0;
                    break;
                case 0x51:
                    SaveType[slotIndex] = 1;
                    break;
                case 0x52:
                    SaveType[slotIndex] = 2;
                    break;
                case 0x53:
                    SaveType[slotIndex] = 3;
                    break;
                case 0xA1:
                    SaveType[slotIndex] = 4;
                    break;
                case 0xA2:
                    SaveType[slotIndex] = 5;
                    break;
                case 0xA3:
                    SaveType[slotIndex] = 6;
                    break;
                default:
                    SaveType[slotIndex] = 7;
                    break;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void LoadStringData()
        {
            SaveName = new string[MemoryCardSlots, 2];
            SaveProdCode = new string[MemoryCardSlots];
            SaveIdentifier = new string[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                var byteArray = new byte[10];

                for (var byteIndex = 0; byteIndex < 10; byteIndex++)
                {
                    byteArray[byteIndex] = HeaderData[slotIndex, byteIndex + 12];
                }

                SaveProdCode[slotIndex] = Encoding.Default.GetString(byteArray);
                byteArray = new byte[8];

                for (var byteIndex = 0; byteIndex < 8; byteIndex++)
                {
                    byteArray[byteIndex] = HeaderData[slotIndex, byteIndex + 22];
                }

                SaveIdentifier[slotIndex] = Encoding.Default.GetString(byteArray);
                byteArray = new byte[64];

                for (var byteIndex = 0; byteIndex < 64; byteIndex++)
                {
                    byteArray[byteIndex] = SaveData[slotIndex, byteIndex + 4];
                }

                SaveName[slotIndex, 0] = _sjisc.ConvertShiftJisToAscii(byteArray);
                SaveName[slotIndex, 1] = Encoding.GetEncoding(932).GetString(byteArray);

                if (SaveName[slotIndex, 0] == null)
                {
                    SaveName[slotIndex, 0] = Encoding.Default.GetString(byteArray, 0, 32);
                }
            }
        }

        private void LoadSaveSize()
        {
            SaveSize = new int[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
                SaveSize[slotIndex] = (HeaderData[slotIndex, 4] | (HeaderData[slotIndex, 5] << 8) |
                                       (HeaderData[slotIndex, 6] << 16)) / 1024;
        }

        public void ToggleDeleteSave(int slotIndex)
        {
            foreach (var slot in FindSaveLinks(slotIndex))
                switch (SaveType[slot])
                {
                case 1:
                    HeaderData[slot, 0x00] = 0xA1;
                    break;
                case 2:
                    HeaderData[slot, 0x00] = 0xA2;
                    break;
                case 3:
                    HeaderData[slot, 0x00] = 0xA3;
                    break;
                case 4:
                    HeaderData[slot, 0x00] = 0x51;
                    break;
                case 5:
                    HeaderData[slot, 0x00] = 0x52;
                    break;
                case 6:
                    HeaderData[slot, 0x00] = 0x53;
                    break;
                }

            CalculateXor();
            LoadSlotTypes();
            WasChanged = true;
        }

        public void FormatSave(int slotIndex)
        {
            foreach (var slot in FindSaveLinks(slotIndex)) FormatSlot(slot);

            CalculateXor();
            LoadStringData();
            LoadSlotTypes();
            LoadRegion();
            LoadSaveSize();
            LoadPalette();
            LoadIcons();
            LoadIconFrames();
            WasChanged = true;
        }

        public int[] FindSaveLinks(int initialSlotNumber)
        {
            var slotList = new List<int>();
            var slotIndex = initialSlotNumber;

            for (var index = 0; index < MemoryCardSlots; index++)
            {
                slotList.Add(slotIndex);

                if (slotIndex > MemoryCardSlots || SaveType[slotIndex] == 7) break;

                if (HeaderData[slotIndex, 8] == 0xFF) break;

                slotIndex = HeaderData[slotIndex, 8];
            }

            return slotList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="slotCount"></param>
        /// <returns></returns>
        private int[] FindFreeSlots(int slotNumber, int slotCount)
        {
            var slotList = new List<int>();

            for (var slotIndex = slotNumber; slotIndex < slotNumber + slotCount; slotIndex++)
            {
                if (SaveType[slotIndex] == 0)
                {
                    slotList.Add(slotIndex);
                }
                else
                {
                    break;
                }

                if (slotNumber + slotCount > MemoryCardSlots)
                {
                    break;
                }
            }

            return slotList.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="slotINdex"></param>
        /// <returns></returns>
        public byte[] GetSaveBytes(int slotINdex)
        {
            var slots = FindSaveLinks(slotINdex);
            var saveBytes = new byte[8320 + (slots.Length - 1) * SlotRawLength];

            for (var byteIndex = 0; byteIndex < SlotHeaderLength; byteIndex++)
            {
                saveBytes[byteIndex] = HeaderData[slots[0], byteIndex];
            }

            for (var slotIndex = 0; slotIndex < slots.Length; slotIndex++)
                for (var byteIndex = 0; byteIndex < SlotRawLength; byteIndex++)
                    saveBytes[SlotHeaderLength + slotIndex * SlotRawLength + byteIndex] =
                        SaveData[slots[slotIndex], byteIndex];

            return saveBytes;
        }

        /// <summary>
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="saveBytes"></param>
        /// <param name="slotsRequired"></param>
        /// <returns></returns>
        public bool SetSaveBytes(int slotNumber, byte[] saveBytes, out int slotsRequired)
        {
            var numberOfSlots = (saveBytes.Length - SlotHeaderLength) / SlotRawLength;
            var numberOfBytes = numberOfSlots * SlotRawLength;
            var freeSlots = FindFreeSlots(slotNumber, numberOfSlots);

            slotsRequired = numberOfSlots;

            if (freeSlots.Length < numberOfSlots)
            {
                return false;
            }

            for (var byteIndex = 0; byteIndex < SlotHeaderLength; byteIndex++)
            {
                HeaderData[freeSlots[0], byteIndex] = saveBytes[byteIndex];
            }

            HeaderData[freeSlots[0], 4] = (byte)(numberOfBytes & 0xFF);
            HeaderData[freeSlots[0], 5] = (byte)((numberOfBytes & 0xFF00) >> 8);
            HeaderData[freeSlots[0], 6] = (byte)((numberOfBytes & 0xFF0000) >> 16);

            for (var slotIndex = 0; slotIndex < numberOfSlots; slotIndex++)
                for (var byteCount = 0; byteCount < SlotRawLength; byteCount++)
                    SaveData[freeSlots[slotIndex], byteCount] =
                        saveBytes[SlotHeaderLength + slotIndex * SlotRawLength + byteCount];

            for (var slotIndex = 0; slotIndex < freeSlots.Length - 1; slotIndex++)
            {
                HeaderData[freeSlots[slotIndex], 0x00] = 0x52;
                HeaderData[freeSlots[slotIndex], 0x08] = (byte)freeSlots[slotIndex + 1];
                HeaderData[freeSlots[slotIndex], 0x09] = 0x00;
            }

            HeaderData[freeSlots[freeSlots.Length - 1], 0] = 0x53;
            HeaderData[freeSlots[freeSlots.Length - 1], 8] = 0xFF;
            HeaderData[freeSlots[freeSlots.Length - 1], 9] = 0xFF;
            HeaderData[freeSlots[0], 0] = 0x51;
            CalculateXor();
            LoadStringData();
            LoadSlotTypes();
            LoadRegion();
            LoadSaveSize();
            LoadPalette();
            LoadIcons();
            LoadIconFrames();
            WasChanged = true;

            return true;
        }

        public void SetHeaderData(int slotIndex, string sProdCode, string sIdentifier, ushort sRegion)
        {
            var header = sProdCode + sIdentifier;
            var byteArray = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(header));

            for (var byteIndex = 0; byteIndex < 20; byteIndex++)
            {
                HeaderData[slotIndex, byteIndex + 10] = 0x00;
            }

            for (var byteIndex = 0; byteIndex < header.Length; byteIndex++)
            {
                HeaderData[slotIndex, byteIndex + 12] = byteArray[byteIndex];
            }

            HeaderData[slotIndex, 10] = (byte)(sRegion & 0xFF);
            HeaderData[slotIndex, 11] = (byte)(sRegion >> 8);
            LoadStringData();
            LoadRegion();
            CalculateXor();
            WasChanged = true;
        }

        private void LoadRegion()
        {
            SaveRegion = new ushort[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                SaveRegion[slotIndex] = (ushort)((HeaderData[slotIndex, 11] << 8) | HeaderData[slotIndex, 10]);
            }
        }

        private void LoadPalette()
        {
            IconPalette = new Color[MemoryCardSlots, IconPaletteLength];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                var colorIndex = 0;

                for (var byteIndex = 0; byteIndex < 32; byteIndex += 2)
                {
                    var redChannel = (SaveData[slotIndex, byteIndex + 96] & 0x1F) << 3;
                    var greenChannel = ((SaveData[slotIndex, byteIndex + 97] & 0x03) << 6) | ((SaveData[slotIndex, byteIndex + 96] & 0xE0) >> 2);
                    var blueChannel = (SaveData[slotIndex, byteIndex + 97] & 0x7C) << 1;

                    if ((redChannel | greenChannel | blueChannel | (SaveData[slotIndex, byteIndex + 97] & 0x80)) == 0)
                    {
                        IconPalette[slotIndex, colorIndex] = Color.Transparent;
                    }
                    else
                    {
                        IconPalette[slotIndex, colorIndex] = Color.FromArgb(redChannel, greenChannel, blueChannel);
                    }

                    colorIndex++;
                }
            }
        }

        private void LoadIcons()
        {
            IconData = new Bitmap[MemoryCardSlots, 3];

            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                for (var iconIndex = 0; iconIndex < 3; iconIndex++)
                {
                    IconData[slotIndex, iconIndex] = new Bitmap(IconDimensions, IconDimensions);

                    var byteIndex = SlotHeaderLength + SlotHeaderLength * iconIndex;

                    for (var yPixel = 0; yPixel < IconDimensions; yPixel++)
                    {
                        for (var xPixel = 0; xPixel < IconDimensions; xPixel += 2)
                        {
                            IconData[slotIndex, iconIndex].SetPixel(xPixel, yPixel,
                                IconPalette[slotIndex, SaveData[slotIndex, byteIndex] & 0xF]);
                            IconData[slotIndex, iconIndex].SetPixel(xPixel + 1, yPixel,
                                IconPalette[slotIndex, SaveData[slotIndex, byteIndex] >> 4]);
                            byteIndex++;
                        }
                    }
                }
            }
        }

        public byte[] GetIconBytes(int slotNumber)
        {
            var iconBytes = new byte[IconRawLength];

            for (var byteIndex = 0; byteIndex < iconBytes.Length; byteIndex++)
            {
                iconBytes[byteIndex] = SaveData[slotNumber, byteIndex + 96];
            }

            return iconBytes;
        }

        public void SetIconBytes(int slotIndex, byte[] iconBytes)
        {
            for (var byteIndex = 0; byteIndex < IconRawLength; byteIndex++)
            {
                SaveData[slotIndex, byteIndex + 96] = iconBytes[byteIndex];
            }

            LoadPalette();
            LoadIcons();
            WasChanged = true;
        }

        private void LoadIconFrames()
        {
            IconFrames = new int[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < IconFrames.Length; slotIndex++)
            {
                switch (SaveData[slotIndex, 2])
                {
                case 0x11:
                    IconFrames[slotIndex] = 1;
                    break;
                case 0x12:
                    IconFrames[slotIndex] = 2;
                    break;
                case 0x13:
                    IconFrames[slotIndex] = 3;
                    break;
                }
            }
        }

        private void LoadGmeComments()
        {
            SaveComments = new string[MemoryCardSlots];

            for (var slotIndex = 0; slotIndex < SaveComments.Length; slotIndex++)
            {
                var byteArray = new byte[GmeCommentsLength];

                for (var byteIndex = 0; byteIndex < byteArray.Length; byteIndex++)
                {
                    byteArray[byteIndex] = GmeHeader[byteIndex + 64 + byteArray.Length * slotIndex];
                }

                SaveComments[slotIndex] = Encoding.Default.GetString(byteArray);
            }
        }

        private void CalculateXor()
        {
            for (var slotIndex = 0; slotIndex < MemoryCardSlots; slotIndex++)
            {
                byte xorChecksum = 0;

                for (var byteCount = 0; byteCount < 126; byteCount++)
                {
                    xorChecksum ^= HeaderData[slotIndex, byteCount];
                }

                HeaderData[slotIndex, 127] = xorChecksum;
            }
        }

        private void FormatSlot(int slotIndex)
        {
            for (var byteIndex = 0; byteIndex < SlotHeaderLength; byteIndex++)
            {
                HeaderData[slotIndex, byteIndex] = 0;
            }

            for (var byteIndex = 0; byteIndex < SlotRawLength; byteIndex++)
            {
                SaveData[slotIndex, byteIndex] = 0;
            }

            SaveComments[slotIndex] = new string('\0', GmeCommentsLength);
            HeaderData[slotIndex, 0x00] = 0xA0;
            HeaderData[slotIndex, 0x08] = 0xFF;
            HeaderData[slotIndex, 0x09] = 0xFF;
        }

        private void FormatMemoryCard()
        {
            for (var slotNumber = 0; slotNumber < MemoryCardSlots; slotNumber++) FormatSlot(slotNumber);

            CalculateXor();
            LoadStringData();
            LoadSlotTypes();
            LoadRegion();
            LoadSaveSize();
            LoadPalette();
            LoadIcons();
            LoadIconFrames();
            WasChanged = true;
        }

        public bool SaveSingleSave(string fileName, int slotNumber, int singleSaveType)
        {
            BinaryWriter binaryWriter;

            try
            {
                binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            }
            catch
            {
                return false;
            }

            var saveBytes = GetSaveBytes(slotNumber);

            if (singleSaveType == 2)
            {
                binaryWriter.Write(saveBytes);
            }
            else if (singleSaveType == 3)
            {
                binaryWriter.Write(saveBytes, SlotHeaderLength, saveBytes.Length - SlotHeaderLength);
            }
            else
            {
                var headerBytes = new byte[54];

                for (var byteIndex = 0; byteIndex < 22; byteIndex++)
                    headerBytes[byteIndex] = HeaderData[slotNumber, byteIndex + 10];

                var nameBytes = Encoding.Default.GetBytes(SaveName[slotNumber, 0]);

                for (var byteIndex = 0; byteIndex < nameBytes.Length; byteIndex++)
                    headerBytes[byteIndex + 21] = nameBytes[byteIndex];

                binaryWriter.Write(headerBytes);
                binaryWriter.Write(saveBytes, SlotHeaderLength, saveBytes.Length - SlotHeaderLength);
            }

            binaryWriter.Close();

            return true;
        }

        public bool OpenSingleSave(string fileName, int slotNumber, out int requiredSlots)
        {
            requiredSlots = 0;

            byte[] finalData;
            BinaryReader binaryReader;

            try
            {
                binaryReader =
                    new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            catch (Exception)
            {
                return false;
            }

            var inputData = binaryReader.ReadBytes(123008);

            binaryReader.Close();

            string tempString = null;

            if (inputData.Length > 3)
            {
                tempString = Encoding.ASCII.GetString(inputData, 0, 2).Trim((char)0x0);
            }

            if (tempString == "Q")
            {
                finalData = inputData;
            }
            else if (tempString == "SC")
            {
                finalData = new byte[inputData.Length + SlotHeaderLength];

                var singleSaveHeader = Encoding.Default.GetBytes(Path.GetFileName(fileName));

                finalData[0] = 0x51;

                for (var i = 0; i < 20 && i < singleSaveHeader.Length; i++)
                {
                    finalData[i + 10] = singleSaveHeader[i];
                }

                for (var i = 0; i < inputData.Length; i++)
                {
                    finalData[i + SlotHeaderLength] = inputData[i];
                }
            }
            else if (tempString == "V")
            {
                if (inputData[60] != 1)
                {
                    return false;
                }

                finalData = new byte[inputData.Length - 4];
                finalData[0] = 0x51;

                for (var i = 0; i < 20; i++) finalData[i + 10] = inputData[i + 100];

                for (var i = 0; i < inputData.Length - 132; i++) finalData[i + SlotHeaderLength] = inputData[i + 132];
            }
            else
            {
                if (!(inputData[0x36] == 0x53 && inputData[0x37] == 0x43)) return false;

                finalData = new byte[inputData.Length + 74];
                finalData[0] = 0x51; //Q

                for (var byteIndex = 0; byteIndex < 20; byteIndex++)
                {
                    finalData[byteIndex + 10] = inputData[byteIndex];
                }

                for (var byteIndex = 0; byteIndex < inputData.Length - 54; byteIndex++)
                {
                    finalData[byteIndex + SlotHeaderLength] = inputData[byteIndex + 54];
                }
            }

            return SetSaveBytes(slotNumber, finalData, out requiredSlots);
        }

        public bool SaveMemoryCard(string fileName, int memoryCardType)
        {
            BinaryWriter binaryWriter;

            try
            {
                binaryWriter =
                    new BinaryWriter(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            }
            catch
            {
                return false;
            }

            LoadDataToRawCard();

            switch (memoryCardType)
            {
            case 2:
                FillGmeHeader();
                binaryWriter.Write(GmeHeader);
                binaryWriter.Write(_rawMemoryCard);
                break;
            case 3:
                binaryWriter.Write(GetVgsHeader());
                binaryWriter.Write(_rawMemoryCard);
                break;
            default:
                binaryWriter.Write(_rawMemoryCard);
                break;
            }

            WasChanged = false;
            CardLocation = fileName;
            CardName = Path.GetFileNameWithoutExtension(fileName);
            binaryWriter.Close();

            return true;
        }

        public byte[] SaveMemoryCardStream()
        {
            LoadDataToRawCard();
            return _rawMemoryCard;
        }

        public void OpenMemoryCardStream(byte[] memCardData)
        {
            _rawMemoryCard = memCardData;
            LoadDataFromRawCard();
            CardName = "Untitled";
            CalculateXor();
            LoadStringData();
            LoadGmeComments();
            LoadSlotTypes();
            LoadRegion();
            LoadSaveSize();
            LoadPalette();
            LoadIcons();
            LoadIconFrames();
            WasChanged = true;
        }

        public string OpenMemoryCard(string fileName)
        {
            if (fileName == null)
            {
                CardName = "Untitled";
                FormatMemoryCard();
                WasChanged = false;
            }
            else
            {
                BinaryReader binaryReader;

                try
                {
                    binaryReader =
                        new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
                catch (Exception exception)
                {
                    return exception.Message;
                }

                int startOffset;
                var sourceArray = new byte[134976];
                binaryReader.BaseStream.Read(sourceArray, 0, 134976);
                binaryReader.Close();
                CardLocation = fileName;
                CardName = Path.GetFileNameWithoutExtension(fileName);

                var tempString = Encoding.ASCII.GetString(sourceArray, 0, 11)
                    .Trim((char)0x00, (char)0x01, (char)0x3F);

                switch (tempString)
                {
                default:

                    return "'" + CardName + "' is not a supported Memory Card format.";

                case "MC":

                    startOffset = 0;
                    CardType = 1;

                    break;

                case "123-456-STD": //DexDrive GME Memory Card
                    startOffset = GmeHeaderLength;
                    CardType = 2;

                    for (var i = 0; i < GmeHeaderLength; i++) GmeHeader[i] = sourceArray[i];
                    break;

                case "VgsM": //VGS Memory Card
                    startOffset = 64;
                    CardType = 3;
                    break;

                case "PMV": //PSP virtual Memory Card
                    startOffset = SlotHeaderLength;
                    CardType = 4;
                    break;
                }

                Array.Copy(sourceArray, startOffset, _rawMemoryCard, 0, MemoryCardRawLength);
                LoadDataFromRawCard();
            }

            CalculateXor();
            LoadStringData();
            LoadGmeComments();
            LoadSlotTypes();
            LoadRegion();
            LoadSaveSize();
            LoadPalette();
            LoadIcons();
            LoadIconFrames();

            return null;
        }
    }
}