using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// Polygon drawing object stored in a a MetaFile, it's a fixed length record, still not implemented, for future use
    /// </summary>
    public class MetaObjectPolygon : MetaObject
    {
        /// <summary>Polygon brush style</summary>
        public int PolyBrushStyle;
        /// <summary>Polygon brush color</summary>
        public int PolyBrushColor;
        /// <summary>Polygon pen style</summary>
        public int PolyPenStyle;
        /// <summary>Polygon pen width</summary>
        public int PolyPenWidth;
        /// <summary>Polygon pen color</summary>
        public int PolyPenColor;
        /// <summary>Polygon point count</summary>
        public int PolyPointCount;
        /// <summary>Polygon stream position</summary>
        public long PolyStreamPos;
        /// <summary>Polygon stream size</summary>
        public long PolyStreamSize;
        /// <summary>
        /// Fill the values of a MetaObjectPolygon, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        override public void FillFromBuf(byte[] buf, int index)
        {
            base.FillFromBuf(buf, index);
            PolyBrushStyle = StreamUtil.ByteArrayToInt(buf, index + 17, 4);
            PolyBrushColor = StreamUtil.ByteArrayToInt(buf, index + 21, 4);
            PolyPenStyle = StreamUtil.ByteArrayToInt(buf, index + 25, 4);
            PolyPenWidth = StreamUtil.ByteArrayToInt(buf, index + 29, 4);
            PolyPenColor = StreamUtil.ByteArrayToInt(buf, index + 33, 4);
            PolyPointCount = StreamUtil.ByteArrayToInt(buf, index + 37, 4);
            PolyStreamPos = StreamUtil.ByteArrayToLong(buf, index + 41, 8);
            PolyStreamSize = StreamUtil.ByteArrayToLong(buf, index + 49, 8);
        }
        /// <summary>
        /// Save the information of the object into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
        override public void SaveToStream(Stream astream)
        {
            base.SaveToStream(astream);
            astream.Write(StreamUtil.IntToByteArray(PolyBrushStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PolyBrushColor), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PolyPenStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PolyPenColor), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PolyBrushStyle), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PolyPointCount), 0, 4);
            astream.Write(StreamUtil.LongToByteArray(PolyStreamPos), 0, 8);
            astream.Write(StreamUtil.LongToByteArray(PolyStreamSize), 0, 8);
        }
    }

}
