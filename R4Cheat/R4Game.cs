using System.Text;

using R4Cheat.Extensions;

namespace R4Cheat;

public class R4Game : R4Item
{
    public uint[] MasterCodes { get; set; } = new uint[8];
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public List<R4Item> Items { get; private set; }
    public string GameId { get; private set; }
    public uint Hash { get; private set; }
    public string Description => Name;

    public R4Game(string path, int gameOffset, string gameId, uint hash, Encoding encoding)
    {
        GameId = gameId;
        Hash = hash;

        using (var input = File.OpenRead(path))
        {
            Load(input, gameOffset, encoding);
        }
    }

    public R4Game(Stream stream, int gameOffset, string gameId, uint hash, Encoding encoding)
    {
        GameId = gameId;
        Hash = hash;
        Load(stream, gameOffset, encoding);
    }

    private void Load(Stream stream, int gameOffset, Encoding encoding)
    {
        stream.Position = gameOffset;
        var reader = new BinaryReader(stream, encoding);
        Name = reader.ReadString(BinaryStringType.ZeroTerminated);
        reader.Align(4);

        var itemCount = reader.ReadUInt16(BinaryEndianess.Little);
        var flags = reader.ReadUInt16(BinaryEndianess.Little);
        Enabled = ((flags & (ushort)R4ItemFlag.Enabled) == (ushort)R4ItemFlag.Enabled);

        for (int i = 0; i < MasterCodes.Length; i++)
        {
            MasterCodes[i] = reader.ReadUInt32(BinaryEndianess.Little);
        }

        Items = new List<R4Item>();
        int n = 0;
        while (n < itemCount)
        {
            var itemSize = reader.ReadUInt16(BinaryEndianess.Little);
            var itemFlags = reader.ReadUInt16(BinaryEndianess.Little);

            if ((itemFlags & (ushort)R4ItemFlag.Folder) == (ushort)R4ItemFlag.Folder)
            {
                var item = new R4Folder(itemSize, itemFlags, stream, encoding);
                Items.Add(item);
                n += itemSize + 1;
            }
            else
            {
                var item = new R4Code(itemFlags, stream, encoding);
                Items.Add(item);
                n++;
            }
        }
    }

    public byte[] ToBytes(Encoding encoding)
    {
        using (var output = new MemoryStream())
        {
            var writer = new BinaryWriter(output);
            writer.Write(Name, BinaryStringType.ZeroTerminated, BinaryEndianess.Little, encoding);
            writer.Align(4);

            var itemCount = 0;
            foreach (var item in Items)
            {
                if (item is R4Folder)
                {
                    var folder = item as R4Folder;
                    if (folder != null)
                    {
                        itemCount += folder.Codes.Count + 1;
                    }
                }
                else
                {
                    itemCount++;
                }
            }
            writer.Write((ushort)itemCount, BinaryEndianess.Little);

            var flags = (ushort)(Enabled ? 0xF000 : 0x00);
            writer.Write(flags, BinaryEndianess.Little);

            foreach (var code in MasterCodes)
            {
                writer.Write(code, BinaryEndianess.Little);
            }

            foreach (var item in Items)
            {
                var itemBytes = item.ToBytes(encoding);
                writer.Write(itemBytes);
            }

            return output.ToArray();
        }
    }
}