using Pointless.Constants;

namespace Pointless.Game.TextRenderer
{
    /// <summary>
    /// The main class in the text rendering mechanism
    /// </summary>
    internal class ScreenText
    {
        public ScreenText()
        {
            try
            {
                ReadFont();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Reads the provided font as a .ttf file <br/>
        /// https://docs.fileformat.com/font/ttf/ <br/>
        /// https://developer.apple.com/fonts/TrueType-Reference-Manual/
        /// </summary>
        private void ReadFont()
        {
            byte[] data = File.ReadAllBytes(TextRendererConstants.DebugFontPath);

            uint tableCount = (uint)(data[4] << 8 | data[5]);
            const int subtableOffset = 12; // the subtable consists of 12 bytes - so the table directory starts at data[12]
            const int tableDirectoryOffset = 16; // each table directory entry consists of 16 bytes

            /* todo: search for the 'cmap' and 'glyf' tables (probably)
             * and try to get each glyph's pixel data.
             * 
             * if succeed, then these pixels can be put into a texture2D
             * and rendered in the game context. the game will not have to rely on
             * .net build-in solutions like fontFamily and bitmap.
             * and it will probably also work on all major platforms (windows, linux, macos).
             * 
             * hope it actualy works. */
            for (int i = 0; i < tableCount; i++)
            {
                int location = subtableOffset + tableDirectoryOffset * i;
                uint tag = (uint)(data[location + 0] << 24 | data[location + 1] << 16 | data[location + 2] << 8 | data[location + 3]);
                Console.WriteLine($"{tag:X8}");
            }
        }
    }
}
