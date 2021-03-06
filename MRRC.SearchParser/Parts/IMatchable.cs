using System;

namespace MRRC.SearchParser.Parts
{
    public interface IMatchable
    {
        string[] Matches(string[] options);
        Tuple<Tuple<string, T>[], T[]> Matches<T>(Tuple<string, T>[] options);
    }
}