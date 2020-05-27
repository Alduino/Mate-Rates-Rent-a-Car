namespace MRRC.SearchParser
{
    public interface IParseResult<out T>
    {
        bool Successful { get; }
        T Result { get; }
    }
}