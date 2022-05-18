namespace R4Cheat.Extensions;

using System.Buffers.Binary;
using System.Text;

public static class BinaryReaderExtensions
{
    public static short ReadInt16(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(2);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadInt16BigEndian(bytes);
        }
    }

    public static ushort ReadUInt16(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(2);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadUInt16BigEndian(bytes);
        }
    }

    public static int ReadInt32(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(4);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadInt32BigEndian(bytes);
        }
    }

    public static uint ReadUInt32(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(4);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadUInt32BigEndian(bytes);
        }
    }

    public static long ReadInt64(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(8);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadInt64BigEndian(bytes);
        }
    }

    public static ulong ReadUInt64(this BinaryReader reader, BinaryEndianess endianess)
    {
        byte[] bytes = reader.ReadBytes(8);

        if (endianess == BinaryEndianess.Little)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        }
        else
        {
            return BinaryPrimitives.ReadUInt64BigEndian(bytes);
        }
    }

    public static string ReadString(this BinaryReader reader, BinaryStringType type, BinaryEndianess endianess = BinaryEndianess.Little, int length = 0)
    {
        switch (type)
        {
            case BinaryStringType.ZeroTerminated:
                return reader.ReadStringZeroTerminated();
            case BinaryStringType.FixedLength:
                return reader.ReadStringFixedLength(length);
            case BinaryStringType.BytePrefixed:
                return reader.ReadStringBytePrefixed();
            case BinaryStringType.ShortPrefixed:
                return reader.ReadStringShortPrefixed(endianess);
            case BinaryStringType.IntPrefixed:
                return reader.ReadStringIntPrefixed(endianess);
            default:
                throw new System.NotImplementedException();
        }
    }

    public static string ReadStringZeroTerminated(this BinaryReader reader)
    {
        StringBuilder result = new StringBuilder();
        char c = reader.ReadChar();

        while (c != '\0')
        {
            result.Append(c);
            c = reader.ReadChar();
        }

        return result.ToString();
    }

    public static string ReadStringFixedLength(this BinaryReader reader, int length)
    {
        return new string(reader.ReadChars(length));
    }

    public static string ReadStringBytePrefixed(this BinaryReader reader)
    {
        int length = reader.ReadByte();
        return new string(reader.ReadChars(length));
    }

    public static string ReadStringShortPrefixed(this BinaryReader reader, BinaryEndianess endianess)
    {
        int length = reader.ReadInt16(endianess);
        return reader.ReadStringFixedLength(length);
    }

    public static string ReadStringIntPrefixed(this BinaryReader reader, BinaryEndianess endianess)
    {
        int length = reader.ReadInt32(endianess);
        return reader.ReadStringFixedLength(length);
    }

    public static void Align(this BinaryReader reader, int alignment)
    {
        long offset = reader.BaseStream.Position % alignment;
        if (offset != 0)
        {
            reader.BaseStream.Position += alignment - offset;
        }
    }
}