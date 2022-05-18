namespace R4Cheat;

public enum R4ItemFlag : ushort
{
    None = 0,
    Enabled = 1 << 8,
    Folder = 1 << 12,
}

public interface R4Item
{
    public string Name { get; }
    public string Description { get; }
    public byte[] ToBytes(System.Text.Encoding encoding);
}