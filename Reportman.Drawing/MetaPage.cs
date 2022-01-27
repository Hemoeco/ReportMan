using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
#if NETSTANDARD2_0
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace Reportman.Drawing
{
    /// <summary>
    /// Metafile page, part of metafile
    /// </summary>
    public class MetaPage : IDisposable
    {
        const int FIRST_ALLOCATED_WIDESTRING = 1000;
        /// <summary>If true, the metafile version is 2_2 or lower, that is an older metafile format</summary>
		public bool Version2_2;
        /// <summary>If true, the page size differs from the default page size in the MetaFile</summary>
        public bool UpdatedPageSize;
        /// <summary>Collection of objects in the page</summary>
        public List<MetaObject> Objects;
        /// <summary>Page orientation</summary>
        public OrientationType Orientation;
        /// <summary>Page size information, filled when UpdatedPageSize is true</summary>
        public PageSizeDetail PageDetail;
        private char[] FPool;
        private int FPoolPos = 0;
        /// <summary>The strings in the page are save to into this list, for memory and file size optimization</summary>
		public SortedList stringlist;
        private MemoryStream FMemStream;
        MetaFile FMetafile;
#if NETSTANDARD2_0
#else
        /// <summary>Reference to a Windows Metafile, for optimization, and depending on configuration of
        /// print driver, the variable will contain a System.Drawing.Image.Metafile with page contents</summary>
        public System.Drawing.Imaging.Metafile WindowsMetafile;
#endif
        /// <summary>Windows Metafile scale, valid only if WindowsMetafile is assigned</summary>
        public float WindowsMetafileScale;
        /// <summary>
        /// The memory stream contains information from objects in the page, la MetaObjectImages not shared
        /// </summary>
		public MemoryStream MemStream { get { return FMemStream; } }
        /// <summary>
        /// The Pool is a char array containing all the strings in the page, the MetaObjects reference strings
        /// by position and length inside this array
        /// </summary>
		public char[] Pool { get { return FPool; } }
        /// <summary>
        /// The MetaFile owner of the page
        /// </summary>
        public MetaFile MetaFile
        {
            get { return FMetafile; }
        }
        /// <summary>Free resources</summary>
		virtual public void Dispose()
        {
            if (FMemStream != null)
            {
                FMemStream.Dispose();
                FMemStream = null;
            }
        }
        /// <summary>Physic with of the page</summary>
		public int PhysicWidth
        {
            get
            {
                if (UpdatedPageSize)
                {
                    //					if (Orientation == OrientationType.Landscape)
                    //						return PageDetail.PhysicHeight;
                    //					else
                    return PageDetail.PhysicWidth;
                }
                else
                {
                    //					if (FMetafile.Orientation == OrientationType.Landscape)
                    //						return FMetafile.CustomY;
                    //					else
                    return FMetafile.CustomX;
                }
            }
        }
        /// <summary>Physic height of the page</summary>
        public int PhysicHeight
        {
            get
            {
                if (UpdatedPageSize)
                {
                    //					if (Orientation == OrientationType.Landscape)
                    //						return PageDetail.PhysicWidth;
                    //					else
                    return PageDetail.PhysicHeight;
                }
                else
                {
                    //					if (FMetafile.Orientation == OrientationType.Landscape)
                    //						return FMetafile.CustomX;
                    //					else
                    return FMetafile.CustomY;
                }
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
		public MetaPage(MetaFile meta)
        {
            FMetafile = meta;
            FMemStream = new MemoryStream();
            Objects = new List<MetaObject>();
            Version2_2 = false;
            FPool = new char[FIRST_ALLOCATED_WIDESTRING];
            stringlist = new SortedList();
        }
        /// <summary>
        /// Clear all objects in the page
        /// </summary>
		public void Clear()
        {
            Objects.Clear();
            stringlist.Clear();
            FPoolPos = 0;
            FMemStream.SetLength(0);
#if NETSTANDARD2_0
#else
            if (WindowsMetafile != null)
            {
                WindowsMetafile.Dispose();
                WindowsMetafile = null;
            }
#endif
        }
        /// <summary>
        /// Add a string
        /// </summary>
        /// <param name="value">String to add</param>
        /// <returns>Postion where the string was added into the pool, if the string already exists, it returns the
        /// position of the string</returns>
		public int AddString(string value)
        {
            object avalue = stringlist[value];
            if (avalue != null)
                return (int)avalue;
            // Error en texto grande
            if (FPool.Length < (FPoolPos + value.Length + 2))
            {
                char[] FPool2 = new char[FPool.Length];
                System.Array.Copy(FPool, 0, FPool2, 0, FPool.Length);
                FPool = new char[(FPool.Length + value.Length + 2) * 2];
                System.Array.Copy(FPool2, 0, FPool, 0, FPool2.Length);
            }
            int OldFPoolPos = FPoolPos;
            for (int i = 0; i < value.Length; i++)
            {
                FPool[FPoolPos] = value[i];
                FPoolPos++;
            }
            int newvalue = OldFPoolPos + 1;
            stringlist.Add(value, newvalue);
            return newvalue;
        }
        /// <summary>
        /// Get the text value from the pool
        /// </summary>
        /// <returns>The text referenced in the TexP position</returns>
		public string GetText(MetaObjectText obj)
        {
            string ares = new string(Pool, obj.TextP - 1, obj.TextS);
            return ares;
        }
        /// <summary>
        /// Get the Windows Font Name text value from the pool
        /// </summary>
        /// <returns>The text referenced in the WFontNameP position</returns>
        public string GetWFontNameText(MetaObjectText obj)
        {
            return new string(Pool, obj.WFontNameP - 1, obj.WFontNameS);
        }
        /// <summary>
        /// Get the Linux Font Name text value from the pool
        /// </summary>
        /// <returns>The text referenced in the LFontNameP position</returns>
        public string GetLFontNameText(MetaObjectText obj)
        {
            return new string(Pool, obj.LFontNameP - 1, obj.LFontNameS);
        }
        /// <summary>
        /// Get a memory stream from a MetaObjectImage
        /// </summary>
        /// <returns>The stream referenced in StreamPos</returns>
		public MemoryStream GetStream(MetaObjectImage obj)
        {
            int asize = (int)obj.StreamSize;
            byte[] buf;
            MemoryStream astream = new MemoryStream();
            if (obj.SharedImage)
            {
                buf = new byte[asize];
                Monitor.Enter(FMetafile.SharedStream);
                try
                {
                    FMetafile.SharedStream.Seek(obj.StreamPos, System.IO.SeekOrigin.Begin);
                    FMetafile.SharedStream.Read(buf, 0, asize);
                }
                finally
                {
                    Monitor.Exit(FMetafile.SharedStream);
                }
                astream.Write(buf, 0, asize);
                astream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            else
            {
                FMemStream.Seek(obj.StreamPos, System.IO.SeekOrigin.Begin);
                buf = new byte[asize];
                FMemStream.Read(buf, 0, asize);
                astream.Write(buf, 0, asize);
                astream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            return astream;
        }
        /// <summary>
        /// Add a stream to the main stream of the page
        /// </summary>
        /// <param name="astream">Stream to add</param>
        /// <param name="shared">If true, the stream will be added to the MetaFile instead to the MetaPage</param>
        /// <returns>Stream position</returns>
		public int AddStream(MemoryStream astream, bool shared)
        {
            int aresult;
            byte[] buf;
            if (shared)
            {
                buf = new byte[astream.Length];
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                astream.Read(buf, 0, (int)astream.Length);
                Monitor.Enter(FMetafile.SharedStream);
                try
                {
                    FMetafile.SharedStream.Seek(0, System.IO.SeekOrigin.End);
                    aresult = (int)FMetafile.SharedStream.Position;
                    FMetafile.SharedStream.Write(buf, 0, (int)astream.Length);
                }
                finally
                {
                    Monitor.Exit(FMetafile.SharedStream);
                }
            }
            else
            {
                MemStream.Seek(0, System.IO.SeekOrigin.End);
                aresult = (int)MemStream.Position;
                astream.Seek(0, System.IO.SeekOrigin.Begin);
                buf = new byte[astream.Length];
                astream.Read(buf, 0, (int)astream.Length);
                MemStream.Write(buf, 0, (int)astream.Length);
            }
            return aresult;
        }
        static private void ReadBuf(Stream astream, ref byte[] buf, int count)
        {
            if (astream.Read(buf, 0, count) < count)
                throw new Exception(Translator.TranslateStr(522));
        }
        /// <summary>
        /// Save the MetaPage into a stream
        /// </summary>
        /// <param name="astream">Destination stream</param>
		public void SaveToStream(Stream astream)
        {
            int separator = (int)MetaSeparator.ObjectHeader;
            astream.Write(StreamUtil.IntToByteArray(separator), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(0), 0, 4);
            int ainteger = (int)Orientation;
            astream.Write(StreamUtil.IntToByteArray(ainteger), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.Index), 0, 4);
            astream.Write(StreamUtil.BoolToByteArray(PageDetail.Custom), 0, 1);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.CustomWidth), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.CustomHeight), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.PhysicWidth), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.PhysicHeight), 0, 4);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.PaperSource), 0, 4);
            astream.Write(StreamUtil.StringToByteArray(PageDetail.ForcePaperName, 61), 0, 61);
            astream.Write(StreamUtil.IntToByteArray(PageDetail.Duplex), 0, 4);
            // Record alignment
            astream.Write(StreamUtil.IntToByteArray(0), 0, 3);

            astream.Write(StreamUtil.BoolToByteArray(UpdatedPageSize), 0, 1);
            astream.Write(StreamUtil.IntToByteArray(Objects.Count), 0, 4);

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].SaveToStream(astream);
            }
            int wsize = FPoolPos * 2;
            astream.Write(StreamUtil.IntToByteArray(wsize), 0, 4);
            if (wsize > 0)
            {
                StreamUtil.WriteCharArrayToStream(FPool, FPoolPos, astream);
            }
            long asize = FMemStream.Length;
            astream.Write(StreamUtil.LongToByteArray(asize), 0, 8);
            FMemStream.Seek(0, SeekOrigin.Begin);
            FMemStream.WriteTo(astream);
        }
        /// <summary>
        /// Load the MetaPage from a stream
        /// </summary>
        /// <param name="astream">Source stream</param>
        public void LoadFromStream(Stream astream)
        {
            byte[] buf = new byte[61];
            int i;
            MetaSeparator separator = MetaSeparator.ObjectHeader;

            ReadBuf(astream, ref buf, 4);
            if (StreamUtil.ByteArrayToInt(buf, 4) != (int)separator)
                throw new Exception(Translator.TranslateStr(523));
            // Mark begin
            ReadBuf(astream, ref buf, 4);
            if (!Version2_2)
            {
                ReadBuf(astream, ref buf, 4);
                Orientation = (OrientationType)StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 4);
                PageDetail.Index = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 1);
                PageDetail.Custom = StreamUtil.ByteArrayToInt(buf, 1) != 0;
                ReadBuf(astream, ref buf, 4);
                PageDetail.CustomWidth = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 4);
                PageDetail.CustomHeight = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 4);
                PageDetail.PhysicWidth = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 4);
                PageDetail.PhysicHeight = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 4);
                PageDetail.PaperSource = StreamUtil.ByteArrayToInt(buf, 4);
                ReadBuf(astream, ref buf, 61);
                PageDetail.ForcePaperName = StreamUtil.ByteArrayToString(buf, 61);

                ReadBuf(astream, ref buf, 4);
                PageDetail.Duplex = StreamUtil.ByteArrayToInt(buf, 4);
                // Record alignment
                ReadBuf(astream, ref buf, 3);

                ReadBuf(astream, ref buf, 1);
                UpdatedPageSize = StreamUtil.ByteArrayToInt(buf, 1) != 0;
            }

            ReadBuf(astream, ref buf, 4);
            int objcount = StreamUtil.ByteArrayToInt(buf, 4);
            if (objcount < 0)
                throw new Exception(Translator.TranslateStr(523));
            buf = new byte[objcount * MetaObject.RECORD_SIZE];
            ReadBuf(astream, ref buf, objcount * MetaObject.RECORD_SIZE);

            Objects.Clear();
            for (i = 0; i < objcount; i++)
            {
                MetaObject obj = MetaObject.CreateFromBuf(buf, i * MetaObject.RECORD_SIZE);
                obj.FillFromBuf(buf, i * MetaObject.RECORD_SIZE);
                Objects.Add(obj);
            }


            // String pool
            buf = new byte[5];
            ReadBuf(astream, ref buf, 4);
            int wsize = StreamUtil.ByteArrayToInt(buf, 4);
            if (wsize < 0)
                throw new Exception(Translator.TranslateStr(523));
            buf = new byte[wsize];
            ReadBuf(astream, ref buf, wsize);
            if (FPool.Length < wsize)
            {
                FPool = new char[wsize];
            }
            for (i = 0; i < (wsize / 2); i++)
            {
                FPool[i] = (char)((int)buf[i * 2] + (int)(buf[i * 2 + 1] << 8));
            }
            FPoolPos = wsize / 2 + 1;

            // Update stringlist
            for (i = 0; i < Objects.Count; i++)
            {
                MetaObject obj = Objects[i];
                switch (obj.MetaType)
                {
                    case MetaObjectType.Text:
                        MetaObjectText atext = (MetaObjectText)obj;
                        string WFontName = GetWFontNameText(atext);
                        if (stringlist.IndexOfKey(WFontName) < 0)
                            stringlist.Add(WFontName, atext.WFontNameP);
                        string LFontName = GetWFontNameText(atext);
                        if (stringlist.IndexOfKey(LFontName) < 0)
                            stringlist.Add(LFontName, atext.LFontNameP);
                        string Text = GetText(atext);
                        if (stringlist.IndexOfKey(Text) < 0)
                            stringlist.Add(Text, atext.TextP);
                        break;

                }
            }
            // Read Stream
            // Stream
            buf = new byte[9];
            ReadBuf(astream, ref buf, 8);
            long asize = StreamUtil.ByteArrayToLong(buf, 8);
            if (asize < 0)
                throw new Exception(Translator.TranslateStr(523));
            if (asize > 0)
            {
                buf = new byte[asize];
                ReadBuf(astream, ref buf, (int)asize);
                FMemStream = new MemoryStream((int)asize);
                FMemStream.Write(buf, 0, (int)asize);
            }
            buf = new byte[9];
        }
        public MetaObjectText DrawText(int PosX, int PosY, int PrintWidth, int PrintHeight, string Text, string WFontName, string LFontName, short FontSize, short FontRotation, int FontColor,
             int BackColor, bool Transparent, int FontStyle, PDFFontType Type1Font, TextAlignType horzalign, TextAlignVerticalType vertalign, bool SingleLine, bool WordWrap, bool CutText,
            PrintStepType PrintStep)
        {
            MetaObjectText metaobj = new MetaObjectText();
            metaobj.TextP = AddString(Text);
            metaobj.TextS = Text.Length;
            metaobj.LFontNameP = AddString(LFontName);
            metaobj.LFontNameS = LFontName.Length;
            metaobj.WFontNameP = AddString(WFontName);
            metaobj.WFontNameS = WFontName.Length;
            metaobj.FontSize = FontSize;
            metaobj.BackColor = BackColor;
            metaobj.FontRotation = FontRotation;
            metaobj.FontStyle = (short)FontStyle;
            metaobj.FontColor = FontColor;
            metaobj.Type1Font = Type1Font;
            metaobj.CutText = CutText;
            metaobj.Transparent = Transparent;
            metaobj.WordWrap = WordWrap;
            metaobj.Top = PosY;
            metaobj.Left = PosX;
            metaobj.Width = PrintWidth;
            metaobj.Height = PrintHeight;
            //			metaobj.RightToLeft=RightToLeft;
            metaobj.PrintStep = PrintStep;
            int aalign = MetaObject.GetIntHorizAlignment(horzalign) | MetaObject.GetIntVertAlignment(vertalign);
            if (SingleLine)
                aalign = aalign | MetaFile.AlignmentFlags_SingleLine;
            metaobj.Alignment = aalign;
            Objects.Add(metaobj);
            return metaobj;
        }
        public MetaObjectDraw DrawShape(int PosX, int PosY, int PrintWidth, int PrintHeight, ShapeType Shape, BrushType BrushStyle, PenType PenStyle,
            int PenWidth, int PenColor, int BrushColor)
        {
            MetaObjectDraw metaobj = new MetaObjectDraw();
            metaobj.MetaType = MetaObjectType.Draw;
            metaobj.Top = PosY; metaobj.Left = PosX;
            metaobj.Width = PrintWidth; metaobj.Height = PrintHeight;
            metaobj.DrawStyle = Shape;
            metaobj.BrushStyle = (int)BrushStyle;
            metaobj.PenStyle = (int)PenStyle;
            metaobj.PenWidth = PenWidth;
            metaobj.PenColor = PenColor;
            metaobj.BrushColor = BrushColor;
            Objects.Add(metaobj);
            return metaobj;
        }
        public MetaObjectImage DrawImage(int PosX, int PosY, int PrintWidth, int PrintHeight, ImageDrawStyleType DrawStyle, int dpires,
            object nvalue)
        {
            MetaObjectImage metaobj = new MetaObjectImage();
            metaobj.MetaType = MetaObjectType.Image;
            metaobj.Top = PosY; metaobj.Left = PosX;
            metaobj.Width = PrintWidth;
            metaobj.Height = PrintHeight;
            metaobj.CopyMode = 20;
            metaobj.DrawImageStyle = DrawStyle;
            metaobj.DPIRes = dpires;
            metaobj.PreviewOnly = false;
            if (nvalue is MemoryStream)
            {
                MemoryStream xstream = (MemoryStream)nvalue;
                {
                    metaobj.StreamPos = AddStream(xstream, false);
                    metaobj.StreamSize = xstream.Length;
                }
            }
            else
            {
#if NETSTANDARD2_0
#else
                if (nvalue is Image)
                {
                    Image nimage = (Image)nvalue;
                    using (MemoryStream mstream = new MemoryStream())
                    {
                        nimage.Save(mstream, ImageFormat.Jpeg);
                    }
                }
                else
#endif
                if (nvalue != DBNull.Value)
                    if (nvalue != null)
                        throw new Exception("Unsupported type MetaPage.DrawImage");
            }
            Objects.Add(metaobj);
            return metaobj;
        }
    }
    /// <summary>
    /// Collection of pages
    /// </summary>
	public class MetaPages
    {
        const int FIRST_ALLOCATION_OBJECTS = 50;
        MetaFile metafile;
        MetaPage[] FPages;
        int FCount;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Parent MetaFile</param>
		public MetaPages(MetaFile meta)
        {
            FCount = 0;
            FPages = new MetaPage[FIRST_ALLOCATION_OBJECTS];
            metafile = meta;
        }
        /// <summary>
        /// Clear the collection, freeing pages
        /// </summary>
		public void Clear()
        {
            for (int i = 0; i < FCount; i++)
            {
                FPages[i].Clear();
                FPages[i].Dispose();
                FPages[i] = null;
            }
            FCount = 0;
        }
        /// <summary>
        /// Clear the collection, not freeing pages
        /// </summary>
		public void ClearCollection()
        {
            FCount = 0;
        }
        /// <summary>
        /// Current page count
        /// </summary>
		public int CurrentCount
        {
            get { return FCount; }
        }
        /// <summary>
        /// Force report processing until the page requested, else throw an exception
        /// </summary>
        /// <param name="index">Index requested</param>
		private void CheckRange(int index)
        {
            metafile.RequestPage(index);
            if ((index < 0) || (index >= FCount))
                throw new Exception("Index out of range on MetaPage collection");
        }
        /// <summary>
        /// Access page by index
        /// </summary>
        /// <param name="index">Index requested</param>
        /// <returns>MetaPage in this index</returns>
		public MetaPage this[int index]
        {
            get
            {
                CheckRange(index);
                return FPages[index];
            }

            set { CheckRange(index); FPages[index] = value; }
        }
        /// <summary>
        /// Retrieve the page count, the report processing will be done until the
        /// total number of pages is known
        /// </summary>
		public int Count
        {
            get
            {
                metafile.RequestPage(int.MaxValue);
                return FCount;
            }
        }
        /// <summary>
        /// Add a metapage to the collection
        /// </summary>
        /// <param name="obj">MetaPage to add</param>
		public void Add(MetaPage obj)
        {
            if (FCount > (FPages.Length - 2))
            {
                MetaPage[] npages = new MetaPage[FCount];
                System.Array.Copy(FPages, 0, npages, 0, FCount);
                FPages = new MetaPage[FPages.Length * 2];
                System.Array.Copy(npages, 0, FPages, 0, FCount);
            }

            FPages[FCount] = obj;
            FCount++;
            if (metafile.ForwardOnly)
            {
                if (FCount > 2)
                {
                    //					FPages[FCount - 333].Clear();
                }
            }
        }
    }

}
