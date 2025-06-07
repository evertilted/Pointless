using Pointless.Constants;
using Pointless.Exceptions;
using Pointless.Game.Text.Enums;

namespace Pointless.Game.Text
{
    /// <summary>
    /// The main class in the text rendering mechanism
    /// </summary>
    internal class TextRenderer
    {
        public TextRenderer()
        {
            try
            {
                ReadTTF();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Reads the provided font as a .ttf file <br/>
        /// https://developer.apple.com/fonts/TrueType-Reference-Manual/
        /// </summary>
        private void ReadTTF()
        {
            /* everything is big edian in this class:
             * read byte -> bitshift it to the end of byte sequence with.
             *
             * all shifts ([n + x] or << x) are made according to the TTF specifications.
             */
            byte[] data = File.ReadAllBytes(TextRendererConstants.DebugFontPath);

            uint tableCount = (uint)(data[4] << 8 | data[5]);
            const int subtableOffset = 12; // the subtable consists of 12 bytes - so the table directory starts at data[12]
            const int tableDirectoryOffset = 16; // each table directory entry consists of 16 bytes

            const uint cmapCode = 0x636D6170;
            for (int i = 0; i < tableCount; i++)
            {
                int currentByte = subtableOffset + tableDirectoryOffset * i;
                uint tag = (uint)(data[currentByte + 0] << 24 | data[currentByte + 1] << 16 | data[currentByte + 2] << 8 | data[currentByte + 3]);

;               if (tag == cmapCode)
                {
                    ReadCMAP(data, currentByte);
                    break;
                }
            }
        }

        /// <summary>
        /// Reads the cmap table
        /// </summary>
        /// <param name="data">File bytes</param>
        /// <param name="start">Pointer to cmap entry in the table directory</param>
        /// <returns>A dictionary with chars as keys and glyph ids as values</returns>
        private Dictionary<int, int> ReadCMAP(byte[] data, int start)
        {
            /* PLEASE NOTE that the 'start' variable is where the cmap entry in the table directory located.
             * declared below 'cmapBegin' is where the cmap table actually starts */
            int cmapBegin = (int)(data[start + 8] << 24 | data[start + 9] << 16 | data[start + 10] << 8 | data[start + 11]);
            int cmapLength = (int)(data[start + 12] << 24 | data[start + 13] << 16 | data[start + 14] << 8 | data[start + 15]);

            int subtableCount = (int)(data[cmapBegin + 2] << 8 | data[cmapBegin + 3]);
            int supportedTableOffset = -1;

            for (int i = 0; i < subtableCount; i++)
            {
                int indexShift = i * 8;
                int platformId = (int)(data[cmapBegin + 4 + indexShift] << 8 | data[cmapBegin + 5 + indexShift]);
                int platformSpecificId = (int)(data[cmapBegin + 6 + indexShift] << 8 | data[cmapBegin + 7 + indexShift]);

                if (platformId == (int)Enum_SupportedPlatformIds.Microsoft &&
                    platformSpecificId == (int)Enum_SupportedPlatformSpecificIds.UnicodeBMPOnly_UCS2)
                {
                    supportedTableOffset = (int)(data[cmapBegin + 8 + indexShift] << 24 | data[cmapBegin + 9 + indexShift] << 16 | data[cmapBegin + 10 + indexShift] << 8 | data[cmapBegin + 11 + indexShift]);
                }
            }

            if (supportedTableOffset == -1)
            {
                throw new FontUnsupportedException("Font platform is unsupported");
            }

            uint format = (uint)(data[cmapBegin + supportedTableOffset] << 8 | data[cmapBegin + supportedTableOffset + 1]);
            if (format != (int)Enum_SupportedFormats.TwoByte)
            {
                throw new FontUnsupportedException("Font format is unsupported");
            }

            return new();
        }
    }
}
