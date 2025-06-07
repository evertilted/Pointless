using Pointless.Game.Constants;
using Pointless.Game.Exceptions;
using Pointless.Game.Text.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Dictionary<int, int> charGlyphMap;
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
                    charGlyphMap = ReadCMAP(data, currentByte);
                    break;
                }
            }
        }

        /// <summary>
        /// Reads the cmap table
        /// </summary>
        /// <param name="data">File bytes</param>
        /// <param name="cmapEntryLocation">Pointer to cmap entry in the table directory</param>
        /// <returns>A dictionary with chars as keys and glyph ids as values</returns>
        private Dictionary<int, int> ReadCMAP(byte[] data, int cmapEntryLocation)
        {
            int subtableStart = cmapEntryLocation + LocateSupportedCMAPSubtable(data, cmapEntryLocation);
            Dictionary<int, int> charGlyphMap = new();

            int segCountX2 = (data[subtableStart + 6] << 8) | data[subtableStart + 7];
            int segCount = segCountX2 / 2;

            int endCodeOffset = subtableStart + 14;
            int startCodeOffset = endCodeOffset + 2 * segCount + 2; // +2 for reservedPad
            int idDeltaOffset = startCodeOffset + 2 * segCount;
            int idRangeOffsetOffset = idDeltaOffset + 2 * segCount;
            int glyphIdArrayOffset = idRangeOffsetOffset + 2 * segCount;

            for (int i = 0; i < segCount; i++)
            {
                UInt16 endCode = ByteReader.ReadUInt16(data, endCodeOffset + i * 2);
                UInt16 startCode = ByteReader.ReadUInt16(data, startCodeOffset + i * 2);
                UInt16 idDelta = ByteReader.ReadUInt16(data, idDeltaOffset + i * 2);
                UInt16 idRangeOffset = ByteReader.ReadUInt16(data, idRangeOffsetOffset + i * 2);

                for (int code = startCode; code <= endCode; code++)
                {
                    if (code == 0xFFFF) continue; // end of table

                    int glyphIndex;

                    if (idRangeOffset == 0)
                    {
                        glyphIndex = (code + idDelta) & 0xFFFF;
                    }
                    else
                    {
                        int offsetInWords = (idRangeOffset / 2) + (code - startCode);
                        int glyphOffset = idRangeOffsetOffset + i * 2 + offsetInWords * 2;

                        if (glyphOffset >= data.Length - 1)
                        {
                            continue;
                        }

                        int glyphId = (int)ByteReader.ReadUInt16(data, glyphOffset);

                        if (glyphId == 0)
                        {
                            glyphIndex = 0;
                        }
                        else
                        {
                            glyphIndex = (glyphId + idDelta) & 0xFFFF;
                        }
                    }

                    charGlyphMap.TryAdd(code, glyphIndex);
                }
            }
            return charGlyphMap;
        }

        /// <summary>
        /// Locates the first byte of a supported cmap subtable relative to <paramref name="cmapEntryLocation"/>, if one exists
        /// </summary>
        /// <param name="data">TTF file bytes</param>
        /// <param name="cmapEntryLocation">Pointer to cmap entry in the table directory</param>
        /// <returns>Index of the first byte of supported cmap subtable</returns>
        /// <exception cref="FontUnsupportedException">Thrown is no supported subtables are found</exception>
        private int LocateSupportedCMAPSubtable(byte[] data, int cmapEntryLocation)
        {
            UInt32 cmapStart = ByteReader.ReadUInt32(data, cmapEntryLocation + 8);
            UInt32 cmapLength = ByteReader.ReadUInt32(data, cmapEntryLocation + 12);

            UInt16 subtableCount = ByteReader.ReadUInt16(data, (int)cmapStart + 2);
            int supportedTableStart = -1;

            for (int i = 0; i < subtableCount; i++)
            {
                int indexShift = i * 8;
                UInt16 platformId = ByteReader.ReadUInt16(data, (int)cmapStart + 4 + indexShift);
                UInt16 platformSpecificId = ByteReader.ReadUInt16(data, (int)cmapStart + 6 + indexShift);

                if (platformId == (int)Enum_SupportedPlatformIds.Microsoft &&
                    platformSpecificId == (int)Enum_SupportedPlatformSpecificIds.UnicodeBMPOnly_UCS2)
                {
                    supportedTableStart = (int)ByteReader.ReadUInt32(data, (int)cmapStart + 8 + indexShift);
                }
            }

            if (supportedTableStart == -1)
            {
                throw new FontUnsupportedException("Font platform is unsupported");
            }

            UInt16 format = ByteReader.ReadUInt16(data, (int)cmapStart + supportedTableStart);
            if (format != (int)Enum_SupportedFormats.TwoByte)
            {
                throw new FontUnsupportedException("Font format is unsupported");
            }

            return supportedTableStart;
        }
    }
}
