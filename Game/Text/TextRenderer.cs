using Pointless.Game.Constants;
using Pointless.Game.Exceptions;
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

            UInt16 tableCount = ByteReader.ReadUInt16(data, 4);
            const int subtableOffset = 12; // the subtable consists of 12 bytes - so the table directory starts at data[12]
            const int tableDirectoryOffset = 16; // each table directory entry consists of 16 bytes

            const uint cmapCode = 0x636D6170;
            for (int i = 0; i < tableCount; i++)
            {
                int currentByte = subtableOffset + tableDirectoryOffset * i;
                UInt32 tag = ByteReader.ReadUInt32(data, currentByte);

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
            UInt32 cmapBegin = ByteReader.ReadUInt32(data, start + 8);
            UInt32 cmapLength = ByteReader.ReadUInt32(data, start + 12);

            UInt16 subtableCount = ByteReader.ReadUInt16(data, (int)cmapBegin + 2);
            int supportedTableOffset = -1;

            for (int i = 0; i < subtableCount; i++)
            {
                int indexShift = i * 8;
                UInt16 platformId = ByteReader.ReadUInt16(data, (int)cmapBegin + 4 + indexShift);
                UInt16 platformSpecificId = ByteReader.ReadUInt16(data, (int)cmapBegin + 6 + indexShift);

                if (platformId == (int)Enum_SupportedPlatformIds.Microsoft &&
                    platformSpecificId == (int)Enum_SupportedPlatformSpecificIds.UnicodeBMPOnly_UCS2)
                {
                    supportedTableOffset = (int)ByteReader.ReadUInt32(data, (int)cmapBegin + 8 + indexShift);
                }
            }

            if (supportedTableOffset == -1)
            {
                throw new FontUnsupportedException("Font platform is unsupported");
            }

            UInt16 format = ByteReader.ReadUInt16(data, (int)cmapBegin + supportedTableOffset);
            if (format != (int)Enum_SupportedFormats.TwoByte)
            {
                throw new FontUnsupportedException("Font format is unsupported");
            }

            return new();
        }
    }
}
