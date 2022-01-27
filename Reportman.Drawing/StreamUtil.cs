using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Reportman.Drawing
{
    public class StreamUtil
    {
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            Strings files = new Strings();
            foreach (string sp in searchPatterns)
            {
                string[] nresult = System.IO.Directory.GetFiles(path, sp, searchOption);
                foreach (string nfile in nresult)
                {
                    files.Add(nfile);
                }
            }
            return files.ToArray();
        }
        public static byte[] LFarray = { 13, 10 };
        /// <summary>
        /// Check for a file if in use
        /// return true
        /// </summary>
        public static bool FileInUse(string path)
        {
            if (!File.Exists(path))
                return false;
            try
            {
                //Just opening the file as open/create
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    //If required we can check for read/write by using fs.CanRead or fs.CanWrite
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Write a string to a stream, the string should contain only
        /// single byte characters
        /// </summary>
        public static void SWriteLine(Stream astream, string astring)
        {
            //astring = astring + (char)13 + (char)10;
            byte[] buf = new byte[astring.Length];
            for (int i = 0; i < astring.Length; i++)
                buf[i] = (byte)astring[i];
            astream.Write(buf, 0, astring.Length);

            astream.Write(LFarray, 0, 2);
            /*
            byte[] buf = ASCIIEncoding.ASCII.GetBytes(astring+(char)13+(char)10);
            astream.Write(buf, 0, buf.Length);*/
        }
        /// <summary>
        /// Default buffer size for buffered applications
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 65535;


        /// <summary>
        /// Generates a MemoryStream from any Stream input
        /// </summary>
        /// <param name="astream">Source stream</param>
        /// <param name="bufsize">
        /// Buffer size can be specified size for performance enhacement
        /// when handling long streams.
        /// You can specify 0 value, so the DEFAULT_BUFFER_SIZE (65535) will
        /// be used.
        /// Note that standard input does not allow reading long segments
        /// so keep the default value if you don't know the source stream
        /// procedence
        /// </param>
        /// <returns>The resulting memory stream</returns>
        static public MemoryStream StreamToMemoryStream(Stream astream, int bufsize)
        {
            // Don't increase BUF_SIZE, standerd input
            if (bufsize == 0)
                bufsize = 65535;
            byte[] buf = new byte[bufsize];

            MemoryStream aresult = new MemoryStream();
            int readed;
            readed = astream.Read(buf, 0, bufsize);
            while (readed > 0)
            {
                aresult.Write(buf, 0, readed);
                readed = astream.Read(buf, 0, bufsize);
            }
            return aresult;
        }
        /// <summary>
        /// Generates a MemoryStream from any Stream input
        /// </summary>
        static public MemoryStream StreamToMemoryStream(Stream astream)
        {
            // Don't increase BUF_SIZE, standerd input
            return StreamToMemoryStream(astream, 0);
        }
        /// <summary>
        /// Generates a MemoryStream from a file
        /// </summary>
        static public MemoryStream FileToMemoryStream(string filename)
        {
            MemoryStream rstream = null;
            // Don't increase BUF_SIZE, standerd input
            using (FileStream fstream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                rstream = StreamToMemoryStream(fstream, 0);
            }
            return rstream;
        }
        /// <summary>
        /// Generate a file from a memory stream
        /// </summary>
        static public void MemoryStreamToFile(MemoryStream memstream, string filename)
        {
            memstream.Seek(0, SeekOrigin.Begin);
            // Don't increase BUF_SIZE, standerd input
            using (FileStream fstream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                memstream.WriteTo(fstream);
            }
        }

        /// <summary>
        /// Writes a string to a stream formatted as multibyte UFT8 encondig
        /// </summary>
        static public int WriteStringToUTF8Stream(string astring, Stream astream)
        {
            int len = astring.Length;
            if (len == 0)
                return 0;
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] abuf = utf8.GetBytes(astring);
            astream.Write(abuf, 0, abuf.Length);
            return abuf.Length;
        }
        /// <summary>
        /// Writes a string to a stream formatted as multibyte UFT8 encondig
        /// </summary>
        static public int WriteStringToStream(string astring, Stream astream, System.Text.Encoding nencoding)
        {
            int len = astring.Length;
            if (len == 0)
                return 0;
            byte[] abuf = nencoding.GetBytes(astring);
            astream.Write(abuf, 0, abuf.Length);
            return abuf.Length;
        }

        /// <summary>
        /// Writes a string to a stream formatted as multibyte UFT8 encondig
        /// </summary>
        static public int WriteCharArrayToStream(char[] chars, int len, Stream astream)
        {
            byte[] abuf = new byte[2];
            if (len == 0)
                return 0;
            for (int i = 0; i < len; i++)
            {
                abuf[0] = (byte)chars[i];
                abuf[1] = (byte)(((int)chars[i]) >> 8);
                astream.Write(abuf, 0, 2);
            }
            return len;
        }
        /// <summary>
        /// Transforms stream content to string representation as hexadecimal bytes
        /// </summary>
        public static string StreamToHex(Stream astream)
        {
            StringBuilder astring = new StringBuilder();

            const int BUFSIZE = 120000;

            byte[] buf = new byte[BUFSIZE];
            byte[] dest = new byte[BUFSIZE * 2];
            long readed;
            readed = astream.Read(buf, 0, BUFSIZE);
            while (readed > 0)
            {
                for (int i = 0; i < readed; i++)
                    astring.Append(StringUtil.ByteToHex(buf[i]));
                readed = astream.Read(buf, 0, BUFSIZE);
            }
            return astring.ToString();
        }
        /// <summary>
        /// Converts an int to a byte array
        /// </summary>
        public static byte[] IntToByteArray(int avalue)
        {
            byte[] aresult = new byte[4];
            aresult[0] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[1] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[2] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[3] = (byte)avalue;
            return aresult;
        }
        /// <summary>
        /// Converts an int to a byte array
        /// </summary>
        public static byte[] Int64ToByteArray(long avalue)
        {
            byte[] aresult = new byte[8];
            aresult[0] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[1] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[2] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[3] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[4] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[5] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[6] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[7] = (byte)avalue;
            return aresult;
        }
        /// <summary>
        /// Converts a short to a byte array
        /// </summary>
        public static byte[] ShortToByteArray(int avalue)
        {
            byte[] aresult = new byte[2];
            aresult[0] = (byte)avalue;
            avalue = avalue >> 8;
            aresult[1] = (byte)avalue;
            avalue = avalue >> 8;
            return aresult;
        }
        /// <summary>
        /// Converts a bool to a byte array
        /// </summary>
        public static byte[] BoolToByteArray(bool avalue)
        {
            byte[] aresult = new byte[1];
            if (avalue)
                aresult[0] = 1;
            else
                aresult[0] = 0;
            return aresult;
        }
        /// <summary>
        /// Converts a string (not unicode) to a byte array
        /// the array is created with the first byte of the char value
        /// only ANSI alowed
        /// </summary>
        public static byte[] StringToByteArray(string avalue, int length)
        {
            byte[] aresult = new byte[length];
            for (int i = 0; i < length; i++)
                aresult[i] = 0;
            if (avalue == null)
                return aresult;
            int alen = avalue.Length;
            if (alen > length)
                alen = length;
            for (int i = 0; i < alen; i++)
                aresult[i] = (byte)avalue[i];
            return aresult;
        }
        /// <summary>
        /// Converts a string (not unicode) to a byte array
        /// </summary>
        public static byte[] StringToByteArray(string avalue, int length, int codepage)
        {
            /*            byte[] aresult = new byte[length];
                        for (int i = 0; i < length; i++)
                            aresult[i] = 0;
                        if (avalue == null)
                            return aresult;
                        int alen = avalue.Length;
                        if (alen > length)
                            alen = length;
                        for (int i = 0; i < alen; i++)
                            aresult[i] = (byte)avalue[i];
                        return aresult;*/
            Encoding encoder = Encoding.GetEncoding(codepage);
            return encoder.GetBytes(avalue);
        }
        /// <summary>
        /// Converts a string to a byte array, choosing unicode or not
        /// </summary>
        public static byte[] StringToByteArray(string avalue, int length, bool unicode)
        {
            if (!unicode)
                return StringToByteArray(avalue, length);
            UTF8Encoding encoder = new UTF8Encoding();
            return encoder.GetBytes(avalue.ToCharArray(), 0, length);
        }
        /// <summary>
        /// Converts a byte to a byte array
        /// </summary>
        public static byte[] ByteToByteArray(byte avalue)
        {
            byte[] aresult = new byte[1];
            aresult[0] = avalue;
            return aresult;
        }
        /// <summary>
        /// Converts a byte array to an int value
        /// </summary>
        public static int ByteArrayToInt(byte[] b1, int index, int alen)
        {
            int aresult = 0;
            switch (alen)
            {
                case 0:
                    aresult = 0;
                    break;
                case 1:
                    aresult = (int)b1[index + 0];
                    break;
                case 2:
                    aresult = (int)b1[index + 0] + (((int)b1[index + 1]) << 8);
                    break;
                case 3:
                    aresult = (int)b1[index + 0] + ((int)b1[index + 1]) * 0xFF + ((int)b1[index + 2]) * 0xFFFF;
                    break;
                default:
                    aresult = (int)b1[index + 0] + (((int)b1[index + 1]) << 8) +
                        (((int)b1[index + 2]) << 16) + (((int)b1[index + 3]) << 24);
                    break;
            }
            return (aresult);

        }
        /// <summary>
        /// Converts a byte array to an int64  value
        /// </summary>
        public static Int64 ByteArrayToInt64(byte[] b1, int index, int alen)
        {
            Int64 aresult = 0;
            switch (alen)
            {
                case 0:
                    aresult = 0;
                    break;
                case 1:
                    aresult = (int)b1[index + 0];
                    break;
                case 2:
                    aresult = (int)b1[index + 0] + (((int)b1[index + 1]) << 8);
                    break;
                case 3:
                    aresult = (int)b1[index + 0] + ((int)b1[index + 1]) * 0xFF + ((int)b1[index + 2]) * 0xFFFF;
                    break;
                case 4:
                    aresult = (int)b1[index + 0] + (((int)b1[index + 1]) << 8) +
                        (((int)b1[index + 2]) << 16) + (((int)b1[index + 3]) << 24);
                    break;
                case 5:
                    aresult = (Int64)b1[index + 0] + (((Int64)b1[index + 1]) << 8) +
                        (((Int64)b1[index + 2]) << 16) + (((Int64)b1[index + 3]) << 24) +
                        (((Int64)b1[index + 4]) << 32);
                    break;
                case 6:
                    aresult = (Int64)b1[index + 0] + (((Int64)b1[index + 1]) << 8) +
                        (((Int64)b1[index + 2]) << 16) + (((Int64)b1[index + 3]) << 24) +
                        (((Int64)b1[index + 4]) << 32) + (((Int64)b1[index + 5]) << 40);
                    break;
                case 7:
                    aresult = (Int64)b1[index + 0] + (((Int64)b1[index + 1]) << 8) +
                        (((Int64)b1[index + 2]) << 16) + (((Int64)b1[index + 3]) << 24) +
                        (((Int64)b1[index + 4]) << 32) + (((Int64)b1[index + 5]) << 40) +
                        (((Int64)b1[index + 6]) << 48);
                    break;
                case 8:
                    aresult = (Int64)b1[index + 0] + (((Int64)b1[index + 1]) << 8) +
                        (((Int64)b1[index + 2]) << 16) + (((Int64)b1[index + 3]) << 24) +
                        (((Int64)b1[index + 4]) << 32) + (((Int64)b1[index + 5]) << 40) +
                        (((Int64)b1[index + 6]) << 48) + (((Int64)b1[index + 7]) << 56);
                    break;
                default:
                    throw new Exception("Not supported Int64, alen=" + alen.ToString());
            }
            return (aresult);

        }
        /// <summary>
        /// Converts a byte array to an uint value
        /// </summary>
        public static uint ByteArrayToUInt(byte[] b1, int index, int alen)
        {
            uint aresult = 0;
            switch (alen)
            {

                case 0:
                    aresult = 0;
                    break;
                case 1:
                    aresult = (uint)b1[index + 0];
                    break;
                case 2:
                    aresult = (uint)b1[index + 0] + (((uint)b1[index + 1]) << 8);
                    break;
                case 3:
                    aresult = (uint)b1[index + 0] + ((uint)b1[index + 1]) * 0xFF + ((uint)b1[index + 2]) * 0xFFFF;
                    break;
                default:
                    aresult = (uint)b1[index + 0] + (((uint)b1[index + 1]) << 8) +
                        (((uint)b1[index + 2]) << 16) + (((uint)b1[index + 3]) << 24);
                    break;
            }
            return (aresult);

        }
        /// <summary>
        /// Converts a byte array to an ushort value
        /// </summary>
        public static ushort ByteArrayToUShort(byte[] b1, int index, int alen)
        {
            ushort aresult = 0;
            switch (alen)
            {

                case 0:
                    aresult = 0;
                    break;
                case 1:
                    aresult = (ushort)b1[index + 0];
                    break;
                default:
                    aresult = (ushort)((ushort)b1[index + 0] + (ushort)(((ushort)b1[index + 1]) << 8));
                    break;
            }
            return (aresult);

        }
        /// <summary>
        /// Converts a byte array to an int value
        /// </summary>
        public static int ByteArrayToInt(byte[] b1, int alen)
        {
            return ByteArrayToInt(b1, 0, alen);
        }
        /// <summary>
        /// Converts a byte array to a short value
        /// </summary>
        public static short ByteArrayToShort(byte[] b1, int alen)
        {
            return (short)ByteArrayToInt(b1, 0, alen);
        }
        /// <summary>
        /// Converts a byte array to a short value
        /// </summary>
        public static short ByteArrayToShort(byte[] b1, int index, int alen)
        {
            return (short)ByteArrayToInt(b1, index, alen);
        }
        /// <summary>
        /// Converts a byte array to a long value
        /// </summary>
        public static long ByteArrayToLong(byte[] b1, int alen)
        {
            return (ByteArrayToLong(b1, 0, alen));

        }
        /// <summary>
        /// Compare the content of two byte arrays, returns true if the content is the same
        /// </summary>
        public static bool CompareArrayContent(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            bool aresult = true;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    aresult = false;
                    break;
                }
            }
            return aresult;
        }
        /// <summary>
        /// Converts a long value to a byte array to a long value
        /// </summary>
        public static byte[] LongToByteArray(long avalue)
        {
            byte[] aresult = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                aresult[i] = (byte)avalue;
                avalue = avalue >> 8;
            }
            return aresult;
        }
        /// <summary>
        /// Converts a byte array to a long value
        /// </summary>
        public static long ByteArrayToLong(byte[] b1, int index, int alen)
        {
            long aresult = 0;
            long int1 = (long)ByteArrayToInt(b1, index, 4);
            long int2 = (long)ByteArrayToInt(b1, index + 4, 4);
            aresult = int1 + (int2 << 32);
            return (aresult);

        }
        /// <summary>
        /// Converts a byte array to a string (not unicode)
        /// </summary>
        public static string ByteArrayToString(byte[] b1, int alen)
        {
            StringBuilder aresult = new StringBuilder(alen);
            for (int i = 0; i < b1.Length; i++)
                aresult.Append((char)b1[i]);
            return aresult.ToString();
        }
        /// <summary>
        /// Converts a byte array to a string (not unicode)
        /// </summary>
        public static string ByteArrayToString(byte[] b1, int nindex, int alen)
        {
            StringBuilder aresult = new StringBuilder();
            for (int i = 0; i < alen; i++)
                aresult.Append((char)b1[i + nindex]);
            return aresult.ToString();
        }
        /// <summary>
        /// Converts a byte array to a string, choosing unicode
        /// </summary>
        public static string ByteArrayToString(byte[] b1, int alen, bool unicode)
        {
            if (!unicode)
                return ByteArrayToString(b1, alen);
            UTF8Encoding nencode = new UTF8Encoding();

            return nencode.GetString(b1, 0, alen);

        }
        /// <summary>
        /// Converts a byte array to a MemoryStream
        /// </summary>
        public static MemoryStream ByteArrayToStream(byte[] b1)
        {
            MemoryStream nstream = new MemoryStream();
            int len = b1.Length;
            nstream.Write(b1, 0, len);
            nstream.Seek(0, SeekOrigin.Begin);
            return nstream;
        }
        /// <summary>
        /// Check if the stream is a compressed stream
        /// </summary>
        static public bool IsCompressed(MemoryStream mems)
        {
            bool aresult = false;
            mems.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] buf = new byte[1];
            if (mems.Read(buf, 0, 1) > 0)
            {
                if (buf[0] == 'x')
                    aresult = true;
            }
            return aresult;
        }
        /// <summary>
        /// Check if the stream is a compressed stream
        /// </summary>
        static public bool IsCompressed(byte[] buf)
        {
            if (buf.Length == 0)
                return false;
            return (buf[0] == 'x');
        }
#if REPMAN_ZLIB
        /// <summary>
        /// Dercompress the first memory stream into the second memory stream
        /// </summary>
        static public void DeCompressStream(MemoryStream memstream,
            MemoryStream dest)
        {
            //#if PocketPC
            //            int bufsize = 10000;
            //#else
            int bufsize = 100000;
            //#endif

            //dest.Capacity = 0;
            byte[] bufuncomp = new byte[bufsize];
            ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(memstream, inf);
            int readed = zstream.Read(bufuncomp, 0, bufsize);
            while (readed > 0)
            {
                dest.Write(bufuncomp, 0, readed);
                readed = zstream.Read(bufuncomp, 0, bufsize);
            }
            dest.Seek(0, System.IO.SeekOrigin.Begin);
        }
        /// <summary>
        /// Dercompress the first memory stream into the second memory stream
        /// </summary>
        static public void DeCompressBuffer(byte[] buffer, int count,
            Stream dest,ProgressEvent progevent)
        {
            //#if PocketPC
            //            int bufsize = 10000;
            //#else
            int bufsize = 100000;
            //#endif

            //dest.Capacity = 0;
            byte[] bufuncomp = new byte[bufsize];
            ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
            inf.SetInput(buffer, 0, count);
            int readed = inf.Inflate(bufuncomp, 0, bufsize);
            while (readed > 0)
            {
                dest.Write(bufuncomp, 0, readed);
                readed = inf.Inflate(bufuncomp, 0, bufsize);
                if (progevent!=null)
                {
                    progevent(dest,new ProgressArgs(dest.Length,dest.Length));
                }
            }
            dest.Seek(0, System.IO.SeekOrigin.Begin);
        }
        /// <summary>
        /// Dercompress the first memory stream into the second memory stream
        /// </summary>
        static public void DeCompressBuffer(byte[] buffer,int count,
            Stream dest)
        {
            // Check if it's a GZip stream

            
            //#if PocketPC
            //            int bufsize = 10000;
            //#else
            int bufsize = 100000;
            int readed = 0;
            //#endif

            //dest.Capacity = 0;
            byte[] bufuncomp = new byte[bufsize];
            if ((buffer[0] == 31) && (buffer[1] == 139))
            {
                using (MemoryStream mstream = new MemoryStream(buffer,0,count))
                {
                    ICSharpCode.SharpZipLib.GZip.GZipInputStream nstream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(mstream);
                    readed = nstream.Read(bufuncomp, 0, bufsize);
                    while (readed > 0)
                    {
                        dest.Write(bufuncomp, 0, readed);
                        readed = nstream.Read(bufuncomp, 0, bufsize);
                    }
                    dest.Seek(0, System.IO.SeekOrigin.Begin);
                    
                }
            }
            else
            {


                ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
                inf.SetInput(buffer, 0, count);
                readed = inf.Inflate(bufuncomp, 0, bufsize);
                while (readed > 0)
                {
                    dest.Write(bufuncomp, 0, readed);
                    readed = inf.Inflate(bufuncomp, 0, bufsize);
                }
                dest.Seek(0, System.IO.SeekOrigin.Begin);
            }
        }
#endif
#if REPMAN_ZLIB
        /// <summary>
        /// Compress the first memory stream into the second memory stream
        /// </summary>
        static public void CompressStream(MemoryStream memstream,
            MemoryStream dest)
        {
            byte[] bufuncomp = new byte[100000];
            ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_SPEED);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(dest, inf, 131072);
            int readed = memstream.Read(bufuncomp, 0, 100000);
            while (readed > 0)
            {
                zstream.Write(bufuncomp, 0, readed);
                readed = memstream.Read(bufuncomp, 0, 100000);
            }
            zstream.Finish();
        }
        /// <summary>
        /// Compress the first memory stream into the second memory stream using Gzip
        /// </summary>
        static public void CompressStreamGZip(MemoryStream memstream,
            MemoryStream dest)
        {
            byte[] bufuncomp = new byte[100000];
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream zstream = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(dest);
            int readed = memstream.Read(bufuncomp, 0, 100000);
            while (readed > 0)
            {
                zstream.Write(bufuncomp, 0, readed);
                readed = memstream.Read(bufuncomp, 0, 100000);
            }
            zstream.Finish();
        }
        /// <summary>
        /// Compress the first memory stream into the second memory stream, option for optimize size
        /// </summary>
        static public void CompressStream(MemoryStream memstream,
            MemoryStream dest,bool optimizesize)
        {
            int level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_SPEED;
            if (optimizesize)
            level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION;

            byte[] bufuncomp = new byte[100000];
            ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(level);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(dest, inf, 131072);
            int readed = memstream.Read(bufuncomp, 0, 100000);
            while (readed > 0)
            {
                zstream.Write(bufuncomp, 0, readed);
                readed = memstream.Read(bufuncomp, 0, 100000);
            }
            zstream.Finish();
        }
        static public void CompressFile(string source, string destination)
        {
            using (FileStream fstream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (FileStream dest = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] bufuncomp = new byte[100000];
                    ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater();
                    ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(dest, inf, 131072);
                    int readed = fstream.Read(bufuncomp, 0, 100000);
                    while (readed > 0)
                    {
                        zstream.Write(bufuncomp, 0, readed);
                        readed = fstream.Read(bufuncomp, 0, 100000);
                    }
                    zstream.Finish();
                }
            }
        }
        static public void DeCompressFile(string source, string destination)
        {
            using (FileStream fstream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (FileStream dest = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] bufuncomp = new byte[100000];
                    ICSharpCode.SharpZipLib.Zip.Compression.Inflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
                    ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(fstream, inf);
                    int readed = zstream.Read(bufuncomp, 0, 100000);
                    while (readed > 0)
                    {
                        dest.Write(bufuncomp, 0, readed);
                        readed = zstream.Read(bufuncomp, 0, 100000);
                    }
                }
            }
        }
        static public async System.Threading.Tasks.Task<TaskCompressResult> CompressStreamAsync(MemoryStream memstream,
            MemoryStream dest, bool optimizesize, bool disposeSource, System.Threading.CancellationTokenSource cancelSource)
        {
            int level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_SPEED;
            if (optimizesize)
                level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION;

            byte[] bufuncomp = new byte[100000];
            long previousLength = dest.Length;
            ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(level);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(dest, inf, 131072);
            int readed = memstream.Read(bufuncomp, 0, 100000);
            bool cancelled = false;
            while (readed > 0)
            {
                await zstream.WriteAsync(bufuncomp, 0, readed);
                readed = await memstream.ReadAsync(bufuncomp, 0, 100000);
                if ((cancelSource != null) && (cancelSource.IsCancellationRequested))
                {
                    cancelled = true;
                    break;
                }
            }
            zstream.Finish();
            if (disposeSource)
            {
                memstream.Dispose();
            }
            TaskCompressResult nresult = new TaskCompressResult();
            nresult.Source = memstream;
            nresult.Destination = dest;
            nresult.DisposedSource = disposeSource;
            nresult.CompressedBytes = dest.Length - previousLength;
            nresult.Cancelled = cancelled;
            return nresult;
        }

        static public System.Threading.Tasks.Task<TaskCompressResult> CompressStreamTask(MemoryStream memstream,
    MemoryStream dest, bool optimizesize, bool disposeSource, System.Threading.CancellationTokenSource cancelSource)
        {
            var tarea = new System.Threading.Tasks.Task<TaskCompressResult>(() => {
                int level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_SPEED;
                if (optimizesize)
                    level = ICSharpCode.SharpZipLib.Zip.Compression.Deflater.BEST_COMPRESSION;

                byte[] bufuncomp = new byte[100000];
                long previousLength = dest.Length;
                ICSharpCode.SharpZipLib.Zip.Compression.Deflater inf = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(level);
                ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream zstream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(dest, inf, 131072);
                int readed = memstream.Read(bufuncomp, 0, 100000);
                bool cancelled = false;
                while (readed > 0)
                {
                    zstream.Write(bufuncomp, 0, readed);
                    readed = memstream.Read(bufuncomp, 0, 100000);
                    if ((cancelSource != null) && (cancelSource.IsCancellationRequested))
                    {
                        cancelled = true;
                        break;
                    }
                }
                zstream.Finish();
                if (disposeSource)
                {
                    memstream.Dispose();
                }
                TaskCompressResult nresult = new TaskCompressResult();
                nresult.Source = memstream;
                nresult.Destination = dest;
                nresult.DisposedSource = disposeSource;
                nresult.CompressedBytes = dest.Length - previousLength;
                nresult.Cancelled = cancelled;
                return nresult;
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
            tarea.Start(System.Threading.Tasks.TaskScheduler.Default);
            return tarea;
        }

#endif
    }
    public class TaskCompressResult
    {
        public Stream Source;
        public Stream Destination;
        public bool DisposedSource;
        public bool Cancelled;
        public long CompressedBytes;
    }
}
