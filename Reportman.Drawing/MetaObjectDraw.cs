using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// Basic drawing object stored in a a MetaFile, it's a fixed length record
    /// </summary>
    public class MetaObjectDraw : MetaObject
    {
        // Draw object
        /// <summary>Type of drawing</summary>
        public ShapeType DrawStyle;
        /// <summary>Brush style</summary>
        public int BrushStyle;
        /// <summary>Integer representation of brush color</summary>
        public int BrushColor;
        /// <summary>Pen Style, Dash=1, Dot=2, DashDot=3,DashDotDot=4,Clear=5</summary>
        public int PenStyle;
        /// <summary>Pen width in twips</summary>
        public int PenWidth;
        /// <summary>Integer representation of pen color</summary>
        public int PenColor;
        /// <summary>
        /// Fill the values of a MetaObjectDraw, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        override public void FillFromBuf(byte[] buf, int index)
        {
            base.FillFromBuf(buf, index);
            DrawStyle = (ShapeType)StreamUtil.ByteArrayToInt(buf, index + 17, 4);
            BrushStyle = StreamUtil.ByteArrayToInt(buf, index + 21, 4);
            BrushColor = StreamUtil.ByteArrayToInt(buf, index + 25, 4);
            PenStyle = StreamUtil.ByteArrayToInt(buf, index + 29, 4);
            PenWidth = StreamUtil.ByteArrayToInt(buf, index + 33, 4);
            PenColor = StreamUtil.ByteArrayToInt(buf, index + 37, 4);
        }
        /// <summary>
        /// Save the information of the object into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
        override public void SaveToStream(Stream astream)
        {
            base.SaveToStream(astream);
            astream.Write(StreamUtil.IntToByteArray((int)DrawStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(BrushStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(BrushColor), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PenStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PenWidth), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PenColor), 0, 4);
            astream.Write(emptybuf, 0, RECORD_SIZE - 24 - 17);
        }
    }
}
