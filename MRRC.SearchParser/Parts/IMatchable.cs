using System;

namespace MRRC.SearchParser.Parts
{
    public interface IMatchable
    {
        string[] Matches(string[] options);
        Tuple<string, T>[] Matches<T>(Tuple<string, T>[] options);
    }
}