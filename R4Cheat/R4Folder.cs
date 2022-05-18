using System.IO;

using R4Cheat.Extensions;

namespace R4Cheat;

public class R4Folder : R4Item
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool Enabled { get; set; }
    public List<R4Code> Codes { get; private set; }

    public R4Folder(int codesCount, ushort flags, Stream stream)
    {
        Enabled = ((flags & (ushort)R4ItemFlag.Enabled) == (ushort)R4ItemFlag.Enabled);

        BinaryReader reader = new BinaryReader(stream);
        Name = reader.ReadString(BinaryStringType.ZeroTerminated);
        Description = reader.ReadString(BinaryStringType.ZeroTerminated);
        reader.Align(4);

        Codes = new List<R4Code>();
        for (int i = 0; i < codesCount; i++)
        {
            var itemSize = reader.ReadUInt16(BinaryEndianess.Little);
            var itemFlags = reader.ReadUInt16(BinaryEndianess.Little);

            if ((itemFlags & (ushort)R4ItemFlag.Folder) == (ushort)R4ItemFlag.Folder)
            {
                throw new InvalidDataException("R4Folder: Nested folders are not supported.");
            }
            else
            {
                var code = new R4Code(itemFlags, stream);
                Codes.Add(code);
            }
        }
    }

    public R4Folder(string name, string description)
    {
        Name = name;
        Description = description;
        Codes = new List<R4Code>();
    }

    public byte[] ToBytes(System.Text.Encoding encoding)
    {
        using (MemoryStream output = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write((ushort)Codes.Count, BinaryEndianess.Little);
            var flags = (ushort)R4ItemFlag.Folder;
            if (Enabled)
            {
                flags |= (ushort)R4ItemFlag.Enabled;
            }
            writer.Write(flags, BinaryEndianess.Little);

            writer.Write(Name, BinaryStringType.ZeroTerminated, BinaryEndianess.Little, encoding);
            writer.Write(Description, BinaryStringType.ZeroTerminated, BinaryEndianess.Little, encoding);

            foreach (var code in Codes)
            {
                writer.Write(code.ToBytes(encoding));
            }

            return output.ToArray();
        }
    }
}