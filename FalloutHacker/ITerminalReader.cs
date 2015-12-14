namespace FalloutHacker
{
    public interface ITerminalReader
    {
        TerminalData AnalyseImage(string imagePath);
        TerminalData AnalyseImage(string imagePath, RegionOfInterest roi);
    }
}