using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// Text object stored in a a MetaFile, it's a fixed length record, so
    /// references to strings or streams are done throught position and sizes in page stream
    /// or MetaFile stream
    /// </summary>
	public class MetaObjectText : MetaObject
    {
        /// <summary>Text position in MetaPage strings pool</summary>
        public int TextP;
        /// <summary>Text length</summary>
        public int TextS;
        /// <summary>Linux Font Name position in MetaPage strings pool</summary>
        public int LFontNameP;
        /// <summary>Linux Font Name length</summary>
        public int LFontNameS;
        /// <summary>Windows Font Name position in MetaPage strings pool</summary>
        public int WFontNameP;
        /// <summary>Linux Font Name length</summary>
        public int WFontNameS;
        /// <summary>Font size in points</summary>
        public short FontSize;
        /// <summary>Font rotation in degrees</summary>
        public short FontRotation;
        /// <summary>Integer representing the font style</summary>
        public short FontStyle;
        /// <summary>Type1Font, the type of font when exporting to Adobe PDF</summary>
        public PDFFontType Type1Font;
        /// <summary>Font color as integer</summary>
        public int FontColor;
        /// <summary>Text background color as integer</summary>
        public int BackColor;
        /// <summary>True if background of the text must be transparent </summary>
		public bool Transparent;
        /// <summary>True if text must be clipped to the bounding box</summary>
        public bool CutText;
        /// <summary>Integer representation of text alignment, contains vertical and horizontal alignemnt</summary>
        public int Alignment;
        /// <summary>True if words must wrap multiple lines if the text does not fit in the bounding box width</summary>
        public bool WordWrap;
        /// <summary>True if words must be drawn from right to left (Arabic languages)</summary>
        public bool RightToLeft;
        /// <summary>Print step</summary>
        public PrintStepType PrintStep;
        /// <summary>
        /// Fill the values of a MetaObjectText, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        override public void FillFromBuf(byte[] buf, int index)
        {
            base.FillFromBuf(buf, index);
            TextP = StreamUtil.ByteArrayToInt(buf, index + 17, 4);
            TextS = StreamUtil.ByteArrayToInt(buf, index + 21, 4);
            LFontNameP = StreamUtil.ByteArrayToInt(buf, index + 25, 4);
            LFontNameS = StreamUtil.ByteArrayToInt(buf, index + 29, 4);
            WFontNameP = StreamUtil.ByteArrayToInt(buf, index + 33, 4);
            WFontNameS = StreamUtil.ByteArrayToInt(buf, index + 37, 4);
            FontSize = StreamUtil.ByteArrayToShort(buf, index + 41, 2);
            FontRotation = StreamUtil.ByteArrayToShort(buf, index + 43, 2);
            FontStyle = StreamUtil.ByteArrayToShort(buf, index + 45, 2);
            Type1Font = (PDFFontType)StreamUtil.ByteArrayToShort(buf, index + 47, 2);
            FontColor = StreamUtil.ByteArrayToInt(buf, index + 49, 4);
            BackColor = StreamUtil.ByteArrayToInt(buf, index + 53, 4);
            Transparent = buf[index + 57] != 0;
            CutText = buf[index + 58] != 0;
            Alignment = StreamUtil.ByteArrayToShort(buf, index + 59, 2);
            WordWrap = buf[index + 61] != 0;
            RightToLeft = buf[index + 62] != 0;
            PrintStep = (PrintStepType)buf[index + 63];
        }
        /// <summary>
        /// Save the information of the object into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
        override public void SaveToStream(Stream astream)
        {
            base.SaveToStream(astream);
            astream.Write(StreamUtil.IntToByteArray(TextP), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(TextS), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(LFontNameP), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(LFontNameS), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(WFontNameP), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(WFontNameS), 0, 4);
            astream.Write(StreamUtil.ShortToByteArray(FontSize), 0, 2);
            astream.Write(StreamUtil.ShortToByteArray(FontRotation), 0, 2);
            astream.Write(StreamUtil.ShortToByteArray(FontStyle), 0, 2);
            astream.Write(StreamUtil.ShortToByteArray((short)Type1Font), 0, 2);
            astream.Write(StreamUtil.IntToByteArray(FontColor), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(BackColor), 0, 4);
            astream.Write(StreamUtil.BoolToByteArray(Transparent), 0, 1);
            astream.Write(StreamUtil.BoolToByteArray(CutText), 0, 1);
            astream.Write(StreamUtil.ShortToByteArray(Alignment), 0, 2);
            astream.Write(StreamUtil.BoolToByteArray(WordWrap), 0, 1);
            astream.Write(StreamUtil.BoolToByteArray(RightToLeft), 0, 1);
            astream.Write(StreamUtil.ByteToByteArray((byte)PrintStep), 0, 1);
            astream.Write(emptybuf, 0, RECORD_SIZE - 47 - 17);
        }
    }
}
