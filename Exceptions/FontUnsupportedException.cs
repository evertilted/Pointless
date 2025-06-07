namespace Pointless.Exceptions
{
    /// <summary>
    /// Indicates an error that occured on reading a TTF file
    /// </summary>
    [Serializable]
    internal class FontUnsupportedException : Exception
    {
        public FontUnsupportedException() : base() { }
        public FontUnsupportedException(string message) : base(message) { }
        public FontUnsupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
