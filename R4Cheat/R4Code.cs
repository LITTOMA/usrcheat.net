using System.IO;

using R4Cheat.Extensions;

namespace R4Cheat;

public class R4Code : R4Item
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<int> Values { get; set; }
    public bool Enabled { get; set; }

    public R4Code(ushort flags, Stream input)
    {
        this.Enabled = ((flags & (ushort)R4ItemFlag.Enabled) == (ushort)R4ItemFlag.Enabled);

        BinaryReader reader = new BinaryReader(input);
        Name = reader.ReadString(BinaryStringType.ZeroTerminated);
        Description = reader.ReadString(BinaryStringType.ZeroTerminated);
        reader.Align(4);

        Values = new List<int>();
        var numOfValues = reader.ReadInt32(BinaryEndianess.Little);
        for (int i = 0; i < numOfValues; i++)
        {
            int value = reader.ReadInt32(BinaryEndianess.Little);
            Values.Add(value);
        }
    }

    public R4Code(string name, string description)
    {
        Name = name;
        Description = description;
        Values = new List<int>();
    }

    public byte[] ToBytes(System.Text.Encoding encoding)
    {
        using (MemoryStream output = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(output);

            // Write flags
            uint flags = 0;
            if (Enabled)
            {
                flags |= (ushort)R4ItemFlag.Enabled;
            }
            writer.Write(flags, BinaryEndianess.Little);

            // Write name and description
            writer.Write(Name, BinaryStringType.ZeroTerminated, BinaryEndianess.Little, encoding);
            writer.Write(Description, BinaryStringType.ZeroTerminated, BinaryEndianess.Little, encoding);
            writer.Align(4);

            // Write number of codes
            writer.Write(Values.Count, BinaryEndianess.Little);

            // Write codes
            foreach (int value in Values)
            {
                writer.Write(value, BinaryEndianess.Little);
            }

            // Write number of chunks
            ushort numOfChunks = (ushort)(output.Length / 4);
            output.Position = 0;
            writer.Write(numOfChunks, BinaryEndianess.Little);

            // return
            return output.ToArray();
        }
    }

    public override string ToString()
    {
        if (Values == null || Values.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(Environment.NewLine, Values.GroupBy(x => x / 2).Select(x => $"{x.ElementAt(0):X08} {x.ElementAt(1):X08}"));
    }
}