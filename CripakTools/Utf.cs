using System.Collections.Generic;
using System.IO;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    public class Utf
    {
        /// <summary>
        /// </summary>
        public List<Column> Columns;

        /// <summary>
        /// </summary>
        public List<List<Row>> Rows;

        /// <summary>
        /// </summary>
        public int TableSize { get; set; }

        /// <summary>
        /// </summary>
        public long RowsOffset { get; set; }

        /// <summary>
        /// </summary>
        public long StringsOffset { get; set; }

        /// <summary>
        /// </summary>
        public long DataOffset { get; set; }

        /// <summary>
        /// </summary>
        public int TableName { get; set; }

        /// <summary>
        /// </summary>
        public short ColumnCount { get; set; }

        /// <summary>
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// </summary>
        public short RowLength { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public bool ReadUtf(EndianReader reader)
        {
            var offset = reader.BaseStream.Position + 8;

            if (Tools.ReadCString(reader, 4) != "@UTF")
            {
                return false;
            }

            Columns = new List<Column>();
            TableSize = reader.ReadInt32();
            RowsOffset = reader.ReadInt32();
            StringsOffset = reader.ReadInt32();
            DataOffset = reader.ReadInt32();
            DataOffset += offset;
            RowsOffset += offset;
            StringsOffset += offset;
            TableName = reader.ReadInt32();
            ColumnCount = reader.ReadInt16();
            RowLength = reader.ReadInt16();
            RowCount = reader.ReadInt32();

            for (var position = 0; position < ColumnCount; position++)
            {
                var column = new Column
                {
                    Flags = reader.ReadByte()
                };

                if (column.Flags == 0)
                {
                    reader.BaseStream.Seek(3, SeekOrigin.Current);
                    column.Flags = reader.ReadByte();
                }

                column.Name = Tools.ReadCString(reader, -1, reader.ReadInt32() + StringsOffset);
                Columns.Add(column);
            }

            Rows = new List<List<Row>>();

            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                reader.BaseStream.Seek(RowsOffset + rowIndex * RowLength, SeekOrigin.Begin);

                var entries = new List<Row>();

                for (var columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
                {
                    var row = new Row();

                    switch ((ColumnFlags)Columns[columnIndex].Flags & ColumnFlags.StorageMask)
                    {
                    case ColumnFlags.StorageNone:
                        entries.Add(row);
                        continue;
                    case ColumnFlags.StorageZero:
                        entries.Add(row);
                        continue;
                    case ColumnFlags.StorageConstant:
                        entries.Add(row);
                        continue;
                    }

                    row.Position = reader.BaseStream.Position;
                    row.Type = Columns[columnIndex].Flags & (int)ColumnFlags.TypeMask;

                    switch (row.Type)
                    {
                    case 0x00: case 0x01:
                        row.Uint8 = reader.ReadByte();
                        break;
                    case 0x02: case 0x03:
                        row.Uint16 = reader.ReadUInt16();
                        break;
                    case 0x04: case 0x05:
                        row.Uint32 = reader.ReadUInt32();
                        break;
                    case 0x06: case 0x07:
                        row.Uint64 = reader.ReadUInt64();
                        break;
                    case 0x08:
                        row.Ufloat = reader.ReadSingle();
                        break;
                    case 0x0A:
                        row.Str = Tools.ReadCString(reader, -1, reader.ReadInt32() + StringsOffset);
                        break;
                    case 0x0B:
                        row.Position = reader.ReadInt32() + DataOffset;
                        row.Data = Tools.GetData(reader, row.Position, reader.ReadInt32());
                        break;
                    default:
                        continue;
                    }

                    entries.Add(row);
                }

                Rows.Add(entries);
            }

            return true;
        }
    }
}