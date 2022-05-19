namespace R4Cheat;

using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

public class R4CheatHeader
{
    private byte[] headerBytes;

    public string Title
    {
        get => Encoding.GetString(headerBytes[0x10..0x4C]).TrimEnd('\0');
        set
        {
            var titleBytes = Encoding.GetBytes(value);
            if (titleBytes.Length >= 0x3C)
            {
                throw new ArgumentException("Title is too long.");
            }
            Array.Copy(titleBytes, 0, headerBytes, 0x10, titleBytes.Length);
        }
    }

    public System.Text.Encoding Encoding
    {
        get
        {
            var encodingMagic = BinaryPrimitives.ReadUInt16LittleEndian(headerBytes[0x4C..0x4E]);
            switch (encodingMagic)
            {
                case 0x5375:
                    return Misc.TryGetEncoding("GBK");
                case 0x53F5:
                    return Misc.TryGetEncoding("Shift-JIS");
                default:
                    throw new ArgumentException("Unknown encoding.");
            }
        }
        set
        {
            var title = Title;
            switch (value.EncodingName)
            {
                case "GBK":
                    headerBytes[0x4C] = 0x75;
                    headerBytes[0x4D] = 0x53;
                    break;
                case "Shift-JIS":
                    headerBytes[0x4C] = 0xF5;
                    headerBytes[0x4D] = 0x53;
                    break;
                default:
                    throw new ArgumentException("Unsupported encoding.");
            }
            Title = title;
        }
    }

    public bool Enabled
    {
        get => headerBytes[0x50] == 0x01;
        set => headerBytes[0x50] = value ? (byte)0x01 : (byte)0x00;
    }

    public int GameTableOffset
    {
        get => BinaryPrimitives.ReadInt32LittleEndian(headerBytes[0x0C..0x10]);
        set
        {
            byte[] buffer = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            Array.Copy(buffer, 0, headerBytes, 0x0C, 4);
        }
    }

    public R4CheatHeader(string title, System.Text.Encoding encoding)
    {
        headerBytes = new byte[0x100];
        Encoding = encoding;
        Title = title;
    }

    public R4CheatHeader(Stream input)
    {
        headerBytes = new byte[0x100];
        input.Read(headerBytes, 0, 0x100);

        var magic = Encoding.ASCII.GetString(headerBytes[0..0xC]);
        if (magic != "R4 CheatCode")
        {
            throw new ArgumentException($"Invalid header: {magic}");
        }
    }
}

public class EndOfCheatTableException : Exception
{
    public EndOfCheatTableException(string message) : base(message) { }
}

public class R4CheatTableEntry
{
    public string GameId { get; set; }
    public uint Hash { get; set; }
    public int Offset { get; set; }

    public R4CheatTableEntry(string gameId, uint hash, int offset)
    {
        GameId = gameId;
        Hash = hash;
        Offset = offset;
    }

    public R4CheatTableEntry(Stream input)
    {
        var buffer = new byte[0x10];
        input.Read(buffer, 0, 0x10);
        GameId = Encoding.ASCII.GetString(buffer[0..0x4]);
        Hash = BinaryPrimitives.ReadUInt32LittleEndian(buffer[0x4..0x8]);
        Offset = BinaryPrimitives.ReadInt32LittleEndian(buffer[0x8..0xC]);

        if (GameId == "\0\0\0\0" && Hash == 0 && Offset == 0)
        {
            throw new EndOfCheatTableException("End of cheat table.");
        }
    }
}

public class R4Cheat
{
    public R4CheatHeader Header { get; private set; }
    public List<R4CheatTableEntry> Table { get; private set; }
    public List<R4Game> Games { get; private set; }
    public string Path { get; private set; }

    public R4Cheat(string path)
    {
        Path = path;
        Games = new List<R4Game>();
        using (var input = File.OpenRead(path))
        {
            Header = new R4CheatHeader(input);
            input.Position = Header.GameTableOffset;
            Table = new List<R4CheatTableEntry>();
            while (true)
            {
                try
                {
                    Table.Add(new R4CheatTableEntry(input));
                }
                catch (EndOfCheatTableException)
                {
                    break;
                }
                catch
                {
                    throw;
                }
            }
        }
    }

    public async Task LoadAllGames(IProgress<ProgressArgs> progress)
    {
        var progressArgs = new ProgressArgs();
        progressArgs.Max = Table.Count;
        progressArgs.Current = 0;
        progress.Report(progressArgs);

        using (var fs = File.OpenRead(Path))
        {
            Games = new List<R4Game>();
            foreach (var entry in Table)
            {
                await Task.Run(() =>
                {
                    var game = new R4Game(fs, entry.Offset, entry.GameId, entry.Hash, Header.Encoding);
                    Games.Add(game);
                });
                progressArgs.Current++;
                progress.Report(progressArgs);
            }
        }
    }

    public R4Game GetGame(string gameId)
    {
        if (Games != null && Games.Count > 0 && Games.Any(x => x.GameId == gameId))
        {
            return Games.First(x => x.GameId == gameId);
        }
        else
        {
            if (Games == null)
            {
                Games = new List<R4Game>();
            }

            using (var fs = File.OpenRead(Path))
            {
                var entry = Table.First(x => x.GameId == gameId);
                var game = new R4Game(fs, entry.Offset, entry.GameId, entry.Hash, Header.Encoding);
                Games.Add(game);
                return game;
            }
        }
    }
}