namespace MRRC.SearchParser
{
    public readonly struct SuccessfulParseResult<T> : IParseResult<T>
    {
        public SuccessfulParseResult(T result)
        {
            Result = result;
        }

        public bool Successful => true;
        public T Result { get; }
    }
}