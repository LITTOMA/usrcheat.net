namespace R4Cheat;

public class ProgressArgs
{
    public double Max { get; set; }
    public double Current { get; set; }
    public double Progress => Current / Max;
    public double Percentage => Progress * 100;
    public string Message { get; set; }
}
