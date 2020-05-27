namespace MRRC.SearchParser
{
    public interface IMatcher
    {
        /// <summary>
        /// The type of the token this returns
        /// </summary>
        Token.Type Type { get; }
        
        /// <summary>
        /// Matches the token against the remaining text
        /// </summary>
        /// <param name="remaining">The remaining text that is being tokenised</param>
        /// <returns>The length of the match, or zero if none was found</returns>
        int Match(string remaining);
    }
}