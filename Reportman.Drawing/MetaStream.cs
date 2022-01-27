using System;
using System.IO;

namespace Reportman.Drawing
{
    class MetaStream : Stream
    {
        const int BLOCK_SIZE = 8192;
        Stream FStream;
        bool FCompressed;
        long FPosition;
        long intreaded;
        bool feof;
        public bool Compressed
        {
            get { return FCompressed; }
        }
        MemoryStream FBuf;
        byte[] intbuf;
        public MetaStream(Stream stream)
            : base()
        {
            intbuf = new byte[BLOCK_SIZE];
            FStream = stream;
            intreaded = 0;
            FCompressed = false;
            FPosition = 0;
            FBuf = new MemoryStream();
            DoInit();
        }
        override public bool CanRead
        {
            get { return FStream.CanRead; }
        }
        override public bool CanWrite
        {
            get { return false; }
        }
        override public bool CanSeek
        {
            get { return false; }
        }
        override public long Length
        {
            get { throw new Exception("Property not supported Length - MetaStream"); }
        }
        override public long Position
        {
            get { throw new Exception("Property not supported Position - MetaStream"); }
            set { throw new Exception("Property not supported Position - MetaStream"); }
        }
        override public void Flush()
        {
            throw new Exception("Method not supported Flush - MetaStream");
        }
        override public void SetLength(long asize)
        {
            throw new Exception("Method not supported SetLength - MetaStream");
        }
        override public long Seek(long index, System.IO.SeekOrigin origin)
        {
            throw new Exception("Method not supported Seek - MetaStream");
        }
        override public void Write(byte[] buf, int index, int count)
        {
            throw new Exception("Method not supported Write - MetaStream");
        }
        private int ReadBlock()
        {
            if (feof)
                return 0;
            int readed = FStream.Read(intbuf, 0, BLOCK_SIZE);
            intreaded = intreaded + readed;
            if (readed == 0)
                feof = true;
            else
            {
                FBuf.Seek(0, System.IO.SeekOrigin.End);
                FBuf.Write(intbuf, 0, readed);
            }
            return readed;
        }
        private void DoInit()
        {
            ReadBlock();
            if (!feof)
                FCompressed = (intbuf[0] == 'x');
        }

        override public int Read(byte[] buf, int index, int count)
        {
            if (feof)
                return 0;
            int partial = 0;
            FBuf.Seek(FPosition, System.IO.SeekOrigin.Begin);
            int readed = FBuf.Read(buf, index, count);
            while ((readed > 0) || (!feof))
            {
                FPosition = FPosition + readed;
                partial = partial + readed;
                if (partial == count)
                    break;
                ReadBlock();
                if (feof)
                    break;
                FBuf.Seek(FPosition, System.IO.SeekOrigin.Begin);
                readed = FBuf.Read(buf, index + partial, count - partial);
            }
            return partial;
        }
    }

}
