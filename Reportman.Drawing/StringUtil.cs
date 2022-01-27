using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies about handling string values
    /// </summary>
    public static class StringUtil
    {
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }
        private static char GetControlDigit(string CadenaNumerica)
        {
            int[] pesos = { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            UInt32 suma = 0;
            UInt32 resto;

            for (int i = 0; i < pesos.Length; i++)
            {
                suma += (UInt32)pesos[i] * UInt32.Parse(CadenaNumerica.Substring(i, 1));
            }
            resto = 11 - (suma % 11);

            if (resto == 10) resto = 1;
            if (resto == 11) resto = 0;

            return resto.ToString("0")[0];
        }
        public static string CheckBankAccount20(string cadena)
        {
            string resultado = "";
            if (cadena.Length != 20)
                return "La cuenta debe tener 20 dígitos";
            foreach (char nchar in cadena)
            {
                if (!char.IsDigit(nchar))
                    return "La cuenta debe ser numérica";
            }
            string entidad = cadena.Substring(0, 4);
            string oficina = cadena.Substring(4, 4);
            char dc1 = cadena[8];
            char dc2 = cadena[9];

            string cuenta = cadena.Substring(10, 10);

            if (GetControlDigit("00" + entidad + oficina) != dc1)
                return "Cuenta incorrecta";
            if (GetControlDigit(cuenta) != dc2)
                return "Cuenta incorrecta";

            return resultado;
        }
        public static string ConvertLineBreaks(string nstring)
        {
            StringBuilder nresult = new StringBuilder();
            for (int i = 0; i < nstring.Length; i++)
            {
                if (nstring[i] == '\r')
                {
                    nresult.Append(nstring[i]);
                    if (i >= nstring.Length - 1)
                    {
                        nresult.Append((char)10);
                    }
                    else
                    {
                        nresult.Append((char)10);
                        if (nstring[i + 1] == (char)10)
                            i++;
                    }
                }
                else
                {
                    if (nstring[i] == '\n')
                    {
                        if (i == 0)
                        {
                            nresult.Append('\r');
                            nresult.Append(nstring[i]);
                        }
                        else
                        {
                            if (nstring[i - 1] != '\r')
                            {
                                nresult.Append('\r');
                                nresult.Append(nstring[i]);
                            }
                        }
                    }
                    else
                        nresult.Append(nstring[i]);

                }
            }
            return nresult.ToString();
        }
        public static string ConvertToHtml(string plaintext)
        {
            string nresult = plaintext.Replace("" + (char)13 + (char)10, "<br/>");
            nresult = plaintext.Replace("" + (char)10, "<br/>");
            return nresult;
        }
        private const string consignos = "ÁÀÄÂÉÈËÊÍÌÏÎÓÒÖÔÚÙÜÛ";
        private const string sinsignos = "AAAAEEEEIIIIOOOOUUUU                    ";
        private const string signosvalidos = "ÇÑ &',-./:;_0123456789";
        public static string UpperCaseSpecial(string source)
        {
            string nresult = source.ToUpper();
            char[] narray = nresult.ToCharArray();
            for (int i = 0; i < narray.Length; i++)
            {
                int idx = consignos.IndexOf(narray[i]);
                if (idx >= 0)
                    narray[i] = sinsignos[idx];
                if ((narray[i] < 'A') || (narray[i] > 'Z'))
                {
                    idx = signosvalidos.IndexOf(narray[i]);
                    if (idx < 0)
                    {
                        narray[i] = ' ';
                    }
                }
            }
            string xresult = new String(narray);
            xresult = xresult.Replace("  ", " ");
            xresult = xresult.Replace("  ", " ");
            return xresult;
        }
        public static string PadStringRightN(string source, int total)
        {
            string nresult = UpperCaseSpecial(source.Trim());
            if (nresult.Length >= total)
            {
                nresult = nresult.Substring(0, total);
            }
            else
            {
                nresult = nresult.PadRight(total);
            }
            return nresult;
        }
        public static string PadStringLeftN(string source, int total)
        {
            string nresult = UpperCaseSpecial(source.Trim());
            if (nresult.Length >= total)
            {
                nresult = nresult.Substring(0, total);
            }
            else
            {
                nresult = nresult.PadLeft(total);
            }
            return nresult;
        }
        public static bool VerifyHash(string plainText,
    string hashAlgorithm,
    string hashString)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashString);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            string expectedHashString =
                ComputeHash(plainText, hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashString == expectedHashString);
        }
        public static string ComputeHash(string plainText,
        string hashAlgorithm,
        byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;
#if PocketPC
#else
                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

#endif
                case "MD5":
                    hash = new MD5CryptoServiceProvider();
                    break;
                default:
                    throw new Exception("Hash algorithm not supported: " + hashAlgorithm.ToUpper());
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashString = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashString;
        }

        public static string PadDecimalLeftN(decimal source, int totallength, int decimals)
        {
            string nresult = "";
            if (source >= 0)
                nresult = nresult + " ";
            else
            {
                source = -source;
                nresult = nresult + "N";
            }
            int intlen = totallength - 1 - decimals;
            long npart = System.Convert.ToInt64(Math.Truncate(source));
            nresult = nresult + npart.ToString().PadLeft(intlen, '0');
            decimal frac = (source - npart) * System.Convert.ToDecimal(Math.Pow(10, decimals));
            npart = System.Convert.ToInt64(Math.Truncate(frac));
            nresult = nresult + npart.ToString().PadLeft(decimals, '0');


            return nresult;
        }
        public static string PadDecimalLeftS(decimal source, int totallength, int decimals)
        {
            bool negative = false;
            string nresult = "";

            if (source < 0)
            {
                negative = true;
                totallength = totallength - 1;
                source = Math.Abs(source);
            }
                //throw new Exception("Only positive numbers in PadDecimalLeftS");
                
            int intlen = totallength - decimals;
            long npart = System.Convert.ToInt64(Math.Truncate(source));
            nresult = nresult + npart.ToString().PadLeft(intlen, '0');
            decimal frac = (source - npart) * System.Convert.ToDecimal(Math.Pow(10, decimals));
            npart = System.Convert.ToInt64(Math.Truncate(frac));
            nresult = nresult + npart.ToString().PadLeft(decimals, '0');
            if (negative)
                nresult = "N" + nresult;
            return nresult;
        }
        public static string StripHTML(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch (Exception E)
            {
                return E.Message + (char)10 + source;
            }
        }
        /// <summary>
        /// Return a string representing the size of a stream in bytes, kbytes, megabytes
        /// </summary>
        /// <param name="sizeinbytes"></param>
        /// <returns></returns>
        public static string GetSizeAsString(long sizeinbytes)
        {
            if (sizeinbytes < 1024)
                return sizeinbytes.ToString("##0") + " bytes";
            if (sizeinbytes < 1024 * 1024)
                return (((double)sizeinbytes) / 1024).ToString("##0.00") + " Kilobytes";
            return (((double)sizeinbytes) / (1024 * 1024)).ToString("##0.00") + " Megabytes";
        }
        /// <summary>
        /// Return a string representing the size of a stream in bytes, kbytes, megabytes
        /// </summary>
        /// <param name="sizeinbytes"></param>
        /// <returns></returns>
        public static string GetSizeAsSmallString(long sizeinbytes)
        {
            if (sizeinbytes < 1024)
                return sizeinbytes.ToString("##0") + "b";
            if (sizeinbytes < 1024 * 1024)
                return (((double)sizeinbytes) / 1024).ToString("##0") + "K";
            return (((double)sizeinbytes) / (1024 * 1024)).ToString("##0") + "M";
        }
        /// <summary>
        /// Number of occurrences of a char inside string
        /// </summary>
        /// <param name="achar"></param>
        /// <param name="nstring"></param>
        /// <returns></returns>
        public static int CountOfChar(char achar, string nstring)
        {
            int ncount = 0;
            foreach (char xchar in nstring)
            {
                if (xchar == achar)
                    ncount++;
            }
            return ncount;
        }
        /// <summary>
        /// Returns a string quoted with single quotes, if
        /// a single quote is contained, doubles de quote
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static string QuoteStr(string ident)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.Append('\'');
            foreach (char c in ident)
            {
                if (c == '\'')
                {
                    sbuilder.Append(c);
                }
                sbuilder.Append(c);

            }
            sbuilder.Append('\'');
            return sbuilder.ToString();
        }
        /// <summary>
        /// Returns a string quoted with double quotes, if
        /// a double quote is contained, doubles de double quote
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static string DoubleQuoteStr(string ident)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.Append('"');
            foreach (char c in ident)
            {
                if (c == '"')
                {
                    sbuilder.Append(c);
                }
                sbuilder.Append(c);

            }
            sbuilder.Append('"');
            return sbuilder.ToString();
        }
        /// <summary>
        /// Returns a string quoted with custom 'quote' separator, if
        /// quote separator is contained, doubles de double quote
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static string CustomQuoteStr(string ident, char quote)
        {
            StringBuilder sbuilder = new StringBuilder();
            sbuilder.Append(quote);
            foreach (char c in ident)
            {
                if (c == quote)
                {
                    sbuilder.Append(c);
                }
                sbuilder.Append(c);

            }
            sbuilder.Append(quote);
            return sbuilder.ToString();
        }
        /// <summary>
        /// Returns a string repeating a character n times
        /// </summary>
        public static string RepeatChar(char c, int count)
        {
            if (count <= 0)
                count = 10;
            StringBuilder s = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                s.Append(c);
            }
            return s.ToString();
        }
        /// <summary>
        /// Transform a byte to his hexadecimal representation
        /// </summary>
        public static string ByteToHex(byte avalue)
        {
            char char1, char2;
            byte hvalue = (byte)(avalue >> 4);
            byte lvalue = (byte)(avalue & 0x0F);

            if (hvalue > 9)
                char1 = (char)(((byte)'A') + (hvalue - 10));
            else
                char1 = (char)(((byte)'0') + (hvalue));
            if (lvalue > 9)
                char2 = (char)(((byte)'A') + (lvalue - 10));
            else
                char2 = (char)(((byte)'0') + (lvalue));
            return char1.ToString() + char2.ToString();
        }
        /// <summary>
        /// Returns true if the char is inside the set: ['0'..'9','a'..'z','A'..'Z','(',')','.',' ',';',':','_','=']
        /// </summary>
        public static bool IsAlpha(char achar)
        {
            bool aresult = false;
            if ((achar >= '0') && (achar <= '9'))
                aresult = true;
            else
                if ((achar >= 'A') && (achar <= 'Z'))
                aresult = true;
            else
                    if ((achar >= 'a') && (achar <= 'z'))
                aresult = true;
            else
                        if ((achar == '_') || (achar == ' ') || (achar == '.') || (achar == '(') ||
                         (achar == ')') || (achar == '=') || (achar == ';') || (achar == ':'))
                aresult = true;
            return aresult;
        }
        /// <summary>
        /// Returns true if the char is inside the set: ['0'..'9']
        /// </summary>
        public static bool IsDigit(char achar)
        {
            bool aresult = false;
            if ((achar >= '0') && (achar <= '9'))
                aresult = true;
            return aresult;
        }
        /// <summary>
        /// Returns true if the string contains only digits: ['0'..'9']
        /// </summary>
        public static bool IsAllDigits(string nstring)
        {
            if (nstring == null)
                return false;
            if (nstring.Length == 0)
                return false;
            bool aresult = true;
            foreach (char achar in nstring)
            {
                if (!((achar >= '0') && (achar <= '9')))
                {
                    aresult = false;
                    break;
                }
            }
            return aresult;
        }
        /// <summary>
        /// Transform a hexadecimal representation to an array of byte, return number of bytes writed into the buffer
        /// </summary>
        public static int HexToBytes(string hex, byte[] buf)
        {
            int binindex = 0;
            int hexindex = 0;
            int highnibble = -1;
            char nchar;
            int aval;
            while (hexindex < hex.Length)
            {
                nchar = hex[hexindex];
                aval = -1;
                switch (nchar)
                {
                    case '0':
                        aval = 0;
                        break;
                    case '1':
                        aval = 1;
                        break;
                    case '2':
                        aval = 2;
                        break;
                    case '3':
                        aval = 3;
                        break;
                    case '4':
                        aval = 4;
                        break;
                    case '5':
                        aval = 5;
                        break;
                    case '6':
                        aval = 6;
                        break;
                    case '7':
                        aval = 7;
                        break;
                    case '8':
                        aval = 8;
                        break;
                    case '9':
                        aval = 9;
                        break;
                    case 'A':
                        aval = 10;
                        break;
                    case 'B':
                        aval = 11;
                        break;
                    case 'C':
                        aval = 12;
                        break;
                    case 'D':
                        aval = 13;
                        break;
                    case 'E':
                        aval = 14;
                        break;
                    case 'F':
                        aval = 15;
                        break;
                }
                if (aval >= 0)
                {
                    if (highnibble >= 0)
                    {
                        aval = aval + (highnibble << 4);
                        highnibble = -1;
                        buf[binindex] = (byte)aval;
                        binindex++;
                    }
                    else
                        highnibble = aval;
                }
                hexindex++;
            }
            return (binindex);
        }
        public static string Decode(string value, int codepage)
        {
            if (codepage == 0)
                return value;
            Encoding enc = Encoding.GetEncoding(codepage);
            byte[] bytes = new byte[value.Length];

            int i = 0;
            foreach (char c in value)
            {
                bytes[i] = (byte)c;
                i++;
            }

            string newresult = enc.GetString(bytes, 0, bytes.Length);

            return newresult;
        }
        public static string Encode(string value, int codepage)
        {
            if (codepage == 0)
                return value;
            Encoding enc = Encoding.GetEncoding(codepage);
            byte[] bytes = enc.GetBytes(value);
            StringBuilder nbuild = new StringBuilder();
            foreach (byte c in bytes)
            {
                nbuild.Append((char)c);
            }


            return nbuild.ToString();
        }
        public static bool ComprovarNif(string nif)
        {
            bool correcte = false;
            if (nif.Length == 0)
                return false;
            if (nif.Length == 1)
                return false;
            if (((nif[0] >= 'a' && nif[0] <= 'z') || (nif[0] >= 'A' && nif[0] <= 'Z')) && ((nif[1] >= 'a' && nif[1] <= 'z') || (nif[1] >= 'A' && nif[1] <= 'Z')))
            {
                //es un nif intracomunitario
                if (nif.Length > 5)
                    correcte = true;
                else
                    correcte = false;
            }
            //if (nif.Length == 0 || nif.Equals("XXXXXXXX")) correcte = true;//no s'ha escrit un nif
            else
            {
                //s'ha escrit un nif o cif
                char a = nif[0];
                if ((nif[0] >= 'a' && nif[0] <= 'z') || (nif[0] >= 'A' && nif[0] <= 'Z'))
                {
                    //es un CIF o NIE
                    if ((nif[nif.Length - 1] >= 'a' && nif[nif.Length - 1] <= 'z') || nif[nif.Length - 1] >= 'A' && nif[nif.Length - 1] <= 'Z')
                    {
                        switch (nif[0])
                        {
                            case 'X':
                            case 'Y':
                            case 'Z':
                            case 'x':
                            case 'y':
                            case 'z':
                                nif = nif.ToUpper();
                                char primer_digito = '0';
                                switch (nif[0])
                                {
                                    case 'Y':
                                        primer_digito = '1';
                                        break;
                                    case 'Z':
                                        primer_digito = '2';
                                        break;
                                }
                                nif = primer_digito + nif.Substring(1, nif.Length - 1);
                                correcte = NifCorrecto(nif);//es NIE
                                break;
                            default:
                                if ((nif[0] >= 'a' && nif[0] <= 'z') || nif[0] >= 'A' && nif[0] <= 'Z')
                                    correcte = CifCorrecto(nif);//es CIF
                                else
                                    correcte = NifCorrecto(nif.Substring(1, nif.Length - 1));//es NIE
                                break;
                        }

                    }
                    else correcte = CifCorrecto(nif);//es CIF
                }
                else
                {
                    //es un nif
                    correcte = NifCorrecto(nif);
                }
            }
            return correcte;
        }
        private enum TiposCodigosEnum { NIF, NIE, CIF };

        private static TiposCodigosEnum GetTipoDocumento(char letra)
        {
            Regex regexNumeros = new Regex("[0-9]");
            if (regexNumeros.IsMatch(letra.ToString()))
                return TiposCodigosEnum.NIF;

            Regex regexLetrasNIE = new Regex("[LKXYM]");
            if (regexLetrasNIE.IsMatch(letra.ToString()))
                return TiposCodigosEnum.NIE;

            Regex regexLetrasCIF = new Regex("[ABCDEFGHJPQRSUVNW]");
            if (regexLetrasCIF.IsMatch(letra.ToString()))
                return TiposCodigosEnum.CIF;

            throw new ApplicationException("El código no es reconocible");
        }

        private static bool CifCorrecto(string nif)
        {
            bool correcte = false;
            string[] letrasCodigo = { "J", "A", "B", "C", "D", "E", "F", "G", "H", "I" };

            string LetraInicial = nif[0].ToString();
            string DigitoControl = nif[nif.Length - 1].ToString();
            string n = nif.ToString().Substring(1, nif.Length - 2);
            int sumaPares = 0;
            int sumaImpares = 0;
            int sumaTotal = 0;
            int i = 0;
            bool retVal = false;
            // Recorrido por todos los dígitos del número
            for (i = 0; i < n.Length; i++)
            {
                int aux;
                Int32.TryParse(n[i].ToString(), out aux);
                if ((i + 1) % 2 == 0)
                {
                    // Si es una posición par, se suman los dígitos
                    sumaPares += aux;
                }
                else
                {
                    // Si es una posición impar, se multiplican los dígitos por 2
                    aux = aux * 2;

                    // se suman los dígitos de la suma
                    sumaImpares += SumaDigitos(aux);
                }
            }
            // Se suman los resultados de los números pares e impares
            sumaTotal += sumaPares + sumaImpares;
            // Se obtiene el dígito de las unidades
            Int32 unidades = sumaTotal % 10;
            // Si las unidades son distintas de 0, se restan de 10
            if (unidades != 0) unidades = 10 - unidades;
            switch (LetraInicial)
            {
                // Sólo números
                case "A":
                case "B":
                case "E":
                case "H":
                    retVal = DigitoControl == unidades.ToString();
                    break;

                // Sólo letras
                case "K":
                case "P":
                case "Q":
                case "S":
                    retVal = DigitoControl == letrasCodigo[unidades];
                    break;
                default:
                    retVal = (DigitoControl == unidades.ToString()) || (DigitoControl == letrasCodigo[unidades]);
                    break;
            }
            correcte = retVal;
            return correcte;
        }
        private static bool NifCorrecto(string nif)
        {
            bool correcte = false;
            String aux = null;
            nif = nif.ToUpper();

            // ponemos la letra en mayúscula
            aux = nif.Substring(0, nif.Length - 1);
            // quitamos la letra del NIF
            bool numero = DoubleUtil.IsNumeric(aux, NumberStyles.Integer);
            if (aux.Length >= 7 && numero)
                aux = CalculaNIF(aux); // calculamos la letra del NIF para comparar con la que tenemos
            else
                return false;
            // comparamos las letras
            if (nif == aux) correcte = true;
            else correcte = false;
            return correcte;
        }
        private static int SumaDigitos(Int32 digitos)
        {
            string sNumero = digitos.ToString();
            Int32 suma = 0;
            for (Int32 i = 0; i < sNumero.Length; i++)
            {
                Int32 aux;
                Int32.TryParse(sNumero[i].ToString(), out aux);
                suma += aux;
            }
            return suma;
        }

        private static String CalculaNIF(String strA)
        {
            const String cCADENA = "TRWAGMYFPDXBNJZSQVHLCKE";
            const String cNUMEROS = "0123456789";
            int a = 0;
            int b = 0;
            int c = 0;
            int NIF = 0;
            StringBuilder sb = new StringBuilder();
            strA = strA.Trim();
            if (strA.Length == 0) return "";
            // Dejar sólo los números
            for (int i = 0; i <= strA.Length - 1; i++)
                if (cNUMEROS.IndexOf(strA[i]) > -1) sb.Append(strA[i]);
            strA = sb.ToString();
            a = 0;
            NIF = Convert.ToInt32(strA);
            do
            {
                b = Convert.ToInt32((NIF / 24));
                c = NIF - (24 * b);
                a = a + c;
                NIF = b;
            } while (b != 0);

            b = Convert.ToInt32((a / 23));
            c = a - (23 * b);
            return strA.ToString() + cCADENA.Substring(c, 1);
        }
        public static bool ValidaCuentaBancaria(string cuentaCompleta)
        {
            // Comprobaciones de la cadena
            if (cuentaCompleta.Length != 20)
                throw new ArgumentException("El número de cuenta no el formato adecuado");

            string banco = cuentaCompleta.Substring(0, 4);
            string oficina = cuentaCompleta.Substring(4, 4);
            string dc = cuentaCompleta.Substring(8, 2);
            string cuenta = cuentaCompleta.Substring(10, 10);

            return CompruebaCuenta(banco, oficina, dc, cuenta);

        }

        private static bool CompruebaCuenta(string banco, string oficina, string dc, string cuenta)
        {
            return GetDigitoControl("00" + banco + oficina) + GetDigitoControl(cuenta) == dc;
        }

        private static string GetDigitoControl(string CadenaNumerica)
        {
            int[] pesos = { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            UInt32 suma = 0;
            UInt32 resto;

            for (int i = 0; i < pesos.Length; i++)
            {
                suma += (UInt32)pesos[i] * UInt32.Parse(CadenaNumerica.Substring(i, 1));
            }
            resto = 11 - (suma % 11);

            if (resto == 10) resto = 1;
            if (resto == 11) resto = 0;

            return resto.ToString("0");
        }
        /*02/04/2013 añadido sergi
         comprueba que el correo electronico tenga un formato valido
         */
        public static Boolean ComprobarMail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
        public static string NormalizeLineBreaks(string input)
        {
            // Allow 10% as a rough guess of how much the string may grow.
            // If we're wrong we'll either waste space or have extra copies -
            // it will still work
            StringBuilder builder = new StringBuilder((int)(input.Length * 1.1));

            bool lastWasCR = false;

            foreach (char c in input)
            {
                if (lastWasCR)
                {
                    lastWasCR = false;
                    if (c == '\n')
                    {
                        continue; // Already written \r\n
                    }
                }
                switch (c)
                {
                    case '\r':
                        builder.Append("\r\n");
                        lastWasCR = true;
                        break;
                    case '\n':
                        builder.Append("\r\n");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }
    }
}
