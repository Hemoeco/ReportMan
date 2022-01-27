using System;
using System.IO;
using Reportman.Drawing;
using System.Drawing;

namespace Reportman.Reporting
{
    public class ImageItem : PrintPosItem, IDisposable
    {
        private const int DEF_DRAWWIDTH = 500;
        private const int DEFAULT_DPI = 100;
        private MemoryStream FStream;
#if REPMAN_ZLIB
        private MemoryStream FDecompStream;
#endif
        public ImageDrawStyleType DrawStyle;
        private long OldStreamPos;
        private MemoryStream FOldStream;
        public int dpires;
        public int CopyMode;
        public short Rotation;
        public string Expression;
        public SharedImageType SharedImage;
        override public void Dispose()
        {
            base.Dispose();
#if REPMAN_DOTNET1
#else
            if (FStream != null)
            {
                FStream.Dispose();
                FStream = null;
            }
#if REPMAN_ZLIB
            if (FDecompStream != null)
            {
                FDecompStream.Dispose();
                FDecompStream = null;
            }
#endif
#endif
        }
        public ImageItem(BaseReport rp)
            : base(rp)
        {
#if REPMAN_ZLIB
            FDecompStream = new MemoryStream();
#endif
            dpires = DEFAULT_DPI;
            Height = DEF_DRAWWIDTH;
            Width = Height;
            Expression = "";
            SharedImage = SharedImageType.None;

            FStream = new MemoryStream();
        }
        protected override string GetClassName()
        {
            return "TRPIMAGE";
        }
        public MemoryStream Stream
        {
            get { return FStream; }
        }
        public override void SubReportChanged(SubReportEvent newstate, string newgroup)
        {
            base.SubReportChanged(newstate, newgroup);
            if (newstate == SubReportEvent.Start)
            {
                OldStreamPos = -1;
                FOldStream = null;
            }
        }
        public MemoryStream GetMemoryStream()
        {
            MemoryStream aresult = null;
            if (FStream.Length > 0)
            {
                if (FDecompStream != null)
                    aresult = FDecompStream;
                else
                {
                    FStream.Seek(0, SeekOrigin.Begin);
                    if (StreamUtil.IsCompressed(FStream))
                    {
#if REPMAN_ZLIB
                        FStream.Seek(0, SeekOrigin.Begin);
                        StreamUtil.DeCompressStream(FStream, FDecompStream);
                        aresult = FDecompStream;
#else
	    					throw new UnNamedException("REPMAN_ZLIB not defined compressed streams not supported");
#endif
                    }
                    else
                        aresult = FStream;
                }
            }
            return aresult;
        }
        public MemoryStream GetStream()
        {
            MemoryStream aresult = null;
            if (Expression.Length > 0)
            {
                aresult = Report.Evaluator.GetStreamFromExpression(Expression);
                if (aresult != null)
                {
                    if (FOldStream != null)
                    {
                        if (FOldStream.Length == aresult.Length)
                        {
                            if (SharedImage == SharedImageType.Variable)
                            {
                                byte[] sx = FOldStream.ToArray();
                                byte[] sy = aresult.ToArray();
                                for (int i = 0; i < FOldStream.Length; i++)
                                {
                                    if (sx[i] != sy[i])
                                    {
                                        OldStreamPos = -1;
                                        break;
                                    }
                                }
                            }
                            else
                                OldStreamPos = -1;
                        }
                        else
                            OldStreamPos = -1;
                    }
                }
            }
            else
            {
                if (FStream.Length > 0)
                {
                    if (FOldStream != null)
                        aresult = FOldStream;
                    else
                    {
                        FStream.Seek(0, SeekOrigin.Begin);
                        if (StreamUtil.IsCompressed(FStream))
                        {
#if REPMAN_ZLIB
                            FStream.Seek(0, SeekOrigin.Begin);
                            StreamUtil.DeCompressStream(FStream, FDecompStream);
                            aresult = FDecompStream;
#else
	    					throw new UnNamedException("REPMAN_ZLIB not defined compressed streams not supported");
#endif
                        }
                        else
                            aresult = FStream;
                    }
                }
            }
            FOldStream = aresult;
            return aresult;
        }
        override public Point GetExtension(PrintOut adriver, Point MaxExtent, bool ForcePartial)
        {
            MemoryStream FMStream;
            Point aresult = base.GetExtension(adriver, MaxExtent, ForcePartial);
            if ((DrawStyle == ImageDrawStyleType.Crop) ||
             (DrawStyle == ImageDrawStyleType.Tile) ||
             (DrawStyle == ImageDrawStyleType.Tiledpi) ||
             (DrawStyle == ImageDrawStyleType.Stretch))
                return aresult; ;
            FMStream = GetStream();
            if (FMStream == null)
                return aresult;
            aresult = adriver.GraphicExtent(FMStream, aresult, dpires);
            LastExtent = aresult;
            return aresult;
        }
        override protected void DoPrint(PrintOut adriver, int aposx, int aposy,
            int newwidth, int newheight, MetaFile metafile, Point MaxExtent,
            ref bool PartialPrint)
        {
            base.DoPrint(adriver, aposx, aposy, newwidth, newheight,
                metafile, MaxExtent, ref PartialPrint);
            MemoryStream FMStream = GetStream();
            if (FMStream == null)
                return;
            if (FMStream.Length == 0)
                return;
            MetaObjectImage metaobj = new MetaObjectImage();
            metaobj.MetaType = MetaObjectType.Image;
            metaobj.Top = aposy; metaobj.Left = aposx;
            metaobj.Width = PrintWidth;
            metaobj.Height = PrintHeight;
            metaobj.CopyMode = 20;
            metaobj.DrawImageStyle = DrawStyle;
            metaobj.DPIRes = dpires;
            metaobj.PreviewOnly = false;
            if (OldStreamPos >= 0)
            {
                metaobj.StreamPos = OldStreamPos;
                metaobj.SharedImage = true;
            }
            else
            {
                metaobj.StreamPos = metafile.Pages[metafile.CurrentPage].AddStream(FMStream, SharedImage != SharedImageType.None);
                if (SharedImage != SharedImageType.None)
                    OldStreamPos = metaobj.StreamPos;
                metaobj.SharedImage = SharedImage != SharedImageType.None;
            }
            metaobj.StreamSize = FMStream.Length;
            metafile.Pages[metafile.CurrentPage].Objects.Add(metaobj);

        }
    }
}
