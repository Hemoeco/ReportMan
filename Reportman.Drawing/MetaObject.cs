using System.IO;

namespace Reportman.Drawing
{
    /// <summary>
    /// Base class for printable objects inside a MetaPage
    /// </summary>
	public abstract class MetaObject
    {
        /// <summary>
        /// Unique identifier used by MetaFile search functions
        /// </summary>
        public long Id;
        /// <summary>
        /// Record size, all objects share the same size
        /// </summary>
		public const int RECORD_SIZE = 66;
        /// <summary>
        /// Internally used buffer
        /// </summary>
		protected static byte[] emptybuf = new byte[100];
        /// <summary>Top position of the object in twips</summary>
		public int Top;
        /// <summary>Left position of the object in twips</summary>
        public int Left;
        /// <summary>Width of the object in twips</summary>
        public int Width;
        /// <summary>height of the object in twips</summary>
        public int Height;
        /// <summary>Type of object</summary>
        public MetaObjectType MetaType;
        /// <summary>
        /// Creates a MetaObject, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        /// <returns>Create a MetaObject using the information in the buffer</returns>
		static public MetaObject CreateFromBuf(byte[] buf, int index)
        {
            MetaObject aresult = null;
            MetaObjectType metatype = (MetaObjectType)buf[index + 16];
            switch (metatype)
            {
                case MetaObjectType.Text:
                    aresult = new MetaObjectText();
                    break;
                case MetaObjectType.Draw:
                    aresult = new MetaObjectDraw();
                    break;
                case MetaObjectType.Image:
                    aresult = new MetaObjectImage();
                    break;
                case MetaObjectType.Polygon:
                    aresult = new MetaObjectPolygon();
                    break;
                case MetaObjectType.Export:
                    aresult = new MetaObjectExport();
                    break;
            }
            return aresult;
        }
        public static int GetIntHorizAlignment(TextAlignType Alignment)
        {
            // Inverse the alignment for BidiMode Full
            int aresult = 0;
            if (Alignment == TextAlignType.Right)
                aresult = MetaFile.AlignmentFlags_AlignRight;
            else
                if (Alignment == TextAlignType.Center)
                aresult = MetaFile.AlignmentFlags_AlignHCenter;
            else
                    if (Alignment == TextAlignType.Justify)
                aresult = MetaFile.AlignmentFlags_AlignHJustify;
            return aresult;
        }
        public static int GetIntVertAlignment(TextAlignVerticalType VAlignment)
        {
            // Inverse the alignment for BidiMode Full
            int aresult = 0;
            if (VAlignment == TextAlignVerticalType.Center)
                aresult = MetaFile.AlignmentFlags_AlignVCenter;
            else
                if (VAlignment == TextAlignVerticalType.Bottom)
                aresult = MetaFile.AlignmentFlags_AlignBottom;
            return aresult;
        }
        /// <summary>
        /// Fill the values of a MetaObject, loading it from a binary buffer
        /// </summary>
        /// <param name="buf">Buffer containing information in binary format</param>
        /// <param name="index">Index to begin read in the buffer</param>
        virtual public void FillFromBuf(byte[] buf, int index)
        {
            Top = StreamUtil.ByteArrayToInt(buf, index + 0, 4);
            Left = StreamUtil.ByteArrayToInt(buf, index + 4, 4);
            Width = StreamUtil.ByteArrayToInt(buf, index + 8, 4);
            Height = StreamUtil.ByteArrayToInt(buf, index + 12, 4);
            MetaType = (MetaObjectType)buf[index + 16];
        }
        /// <summary>
        /// Save the information of the object into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
		virtual public void SaveToStream(Stream astream)
        {
            astream.Write(StreamUtil.IntToByteArray(Top), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Left), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Width), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(Height), 0, 4);
            astream.Write(StreamUtil.ByteToByteArray((byte)MetaType), 0, 1);
        }
    }
}
