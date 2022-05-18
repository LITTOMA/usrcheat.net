namespace R4Cheat.Extensions;

using System.Buffers.Binary;
using System.Text;

public static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter writer, string value, BinaryStringType type, BinaryEndianess endianess, Encoding encoding)
    {
        var bytes = encoding.GetBytes(value);
        switch (type)
        {
            case BinaryStringType.ZeroTerminated:
                writer.Write(bytes);
                writer.Write((byte)0);
                break;
            case BinaryStringType.BytePrefixed:
                writer.Write((byte)bytes.Length);
                writer.Write(bytes);
                break;
            case BinaryStringType.ShortPrefixed:
                writer.Write((short)bytes.Length);
                writer.Write(bytes);
                break;
            case BinaryStringType.IntPrefixed:
                writer.Write((int)bytes.Length);
                writer.Write(bytes);
                break;
            case BinaryStringType.FixedLength:
                writer.Write(bytes);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static void Write(this BinaryWriter writer, string value, BinaryStringType type, BinaryEndianess endianess)
    {
        Write(writer, value, type, endianess, Encoding.UTF8);
    }

    public static void Write(this BinaryWriter writer, int value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[4]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Write(this BinaryWriter writer, uint value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[4]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Write(this BinaryWriter writer, long value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[8]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt64BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Write(this BinaryWriter writer, ulong value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[8]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Write(this BinaryWriter writer, short value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[2]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Write(this BinaryWriter writer, ushort value, BinaryEndianess endianess)
    {
        var buffer = new Span<byte>(new byte[2]);
        if (endianess == BinaryEndianess.Little)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }
        writer.Write(buffer);
    }

    public static void Align(this BinaryWriter writer, int alignment, byte paddingContent = 0)
    {
        var remainder = writer.BaseStream.Position % alignment;
        if (remainder != 0)
        {
            for (var i = 0; i < (alignment - remainder); i++)
            {
                writer.Write(paddingContent);
            }
        }
    }
}