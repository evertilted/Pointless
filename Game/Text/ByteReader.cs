namespace Pointless.Game.Text
{
    /// <summary>
    /// A little helper that does all the cumbersome byte reading for you.<br/>
    /// BigEndian.
    /// </summary>
    internal static class ByteReader
    {
        /// <summary>
        /// Reads 2 bytes as uint16
        /// </summary>
        /// <param name="source">The array to read from</param>
        /// <param name="start">The point in the array to read from</param>
        /// <returns>An uint16</returns>
        public static UInt16 ReadUInt16(byte[] source, int start)
        {
            return (UInt16)(source[start] << 8 | source[start + 1]);
        }

        /// <summary>
        /// Reads 4 bytes as uint32
        /// </summary>
        /// <param name="source">The array to read from</param>
        /// <param name="start">The point in the array to read from</param>
        /// <returns>An uint32</returns>
        public static UInt32 ReadUInt32(byte[] source, int start)
        {
            return (UInt32)(source[start] << 24 | source[start + 1] << 16 | source[start + 2] << 8 | source[start + 3]);
        }
    }
}
