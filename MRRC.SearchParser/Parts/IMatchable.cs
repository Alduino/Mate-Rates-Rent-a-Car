namespace MRRC.SearchParser.Parts
{
    public interface IMatchable
    {
        string[] Matches(string[] options);
    }
}