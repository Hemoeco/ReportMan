using System.IO;


namespace Reportman.Drawing
{
    /// <summary>
    /// Export object stored in a a MetaFile, it's a fixed length record, still not implemented, it's used
    /// when exporting MetaFile to custom formats
    /// </summary>
    public class MetaObjectExport : MetaObject
    {
        /// <summary>Text export position in MetaPage strings pool</summary>
        public int TextExpP;
        /// <summary>Text export size in MetaPage strings pool</summary>
        public int TextExpS;
        /// <summary>Line to export the text</summary>
        public int Line;
        /// <summary>Postion to export the text</summary>
        public int Position;
        /// <summary>Size of the exported string</summary>
        public int Size;
        /// <summary>True if a new line must be generated</summary>
        public bool DoNewLine;
        /// <summary>
        /// Fill the values of a MetaObjectExport, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        override public void FillFromBuf(byte[] buf, int index)
        {
            base.FillFromBuf(buf, index);
            TextExpP = StreamUtil.ByteArrayToInt(buf, index + 17, 4);
            TextExpS = StreamUtil.ByteArrayToInt(buf, index + 21, 4);
            Line = StreamUtil.ByteArrayToInt(buf, index + 25, 4);
            Position = StreamUtil.ByteArrayToInt(buf, index + 29, 4);
            Size = StreamUtil.ByteArrayToInt(buf, index + 33, 4);
            DoNewLine = buf[index + 37] != 0;
        }
        /// <summary>
        /// Save the information of the object into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
        override public void SaveToStream(Stream astream)
        {
            base.SaveToStream(astream);
            astream.Write(StreamUtil.IntToByteArray(TextExpP), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(TextExpP), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Line), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Position), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Size), 0, 4);
            astream.Write(StreamUtil.BoolToByteArray(DoNewLine), 0, 1);
        }
    }

}
