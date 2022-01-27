using System;
using System.Collections.Generic;
using System.Linq;

namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies about handling double values
    /// </summary>
    public class DoubleUtil
    {
        /// <summary>
        /// Check if a string can be converted to a number
        /// </summary>
        /// <param name="val"></param>
        /// <param name="NumberStyle"></param>
        /// <returns></returns>
        public static bool IsNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            bool boolresult = (Double.TryParse(val, NumberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result));
            if (boolresult)
            {
                if (NumberStyle == System.Globalization.NumberStyles.Integer)
                {
                    if ((result > System.Int32.MaxValue) || (result < System.Int32.MinValue))
                        boolresult = false;
                }
            }
            return boolresult;
        }
        public static bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Truncate a decimal number
        /// </summary>
        /// <param name="nvalue"></param>
        /// <returns></returns>
        public static decimal Truncate(decimal nvalue)
        {
            if (nvalue >= 0)
                return System.Convert.ToDecimal(Math.Floor(System.Convert.ToDouble(nvalue)));
            else
                return System.Convert.ToDecimal(Math.Ceiling(System.Convert.ToDouble(nvalue)));
        }
        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        ///    
        public static double StandardDeviation(IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        /// <summary>
        /// Format a number with a mask
        /// </summary>
        public static string FormatCurrAdv(string mask, double number)
        {
            return (number.ToString(mask));
        }
        /// <summary>
        /// Default format string for a number of decimals
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string FormatStringDecimals(int decimals)
        {
            string aformat = "N0";
            if (decimals > 0)
            {
                //                StringBuilder sbuild = new StringBuilder("0.");
                //                for (int i = 0; i < decimals; i++)
                //                    sbuild.Append('0');
                aformat = "N" + decimals.ToString();
            }
            return aformat;
        }
        /// <summary>
        /// Calculates the fractionary value of a number
        /// </summary>
        /// <param name="number">Decimal number</param>
        /// <returns>Fractionary part of the decimal number</returns>
        public static decimal Frac(decimal number)
        {
            return (number - Math.Truncate(number));
        }
        /// <summary>
        /// Calculates the integer part of a number
        /// </summary>
        /// <param name="number">Decimal number</param>
        /// <returns>Truncated numberr</returns>
        public static long Trunc(decimal number)
        {
            return (System.Convert.ToInt64(Math.Truncate(number)));
        }
        /// <summary>
        /// Round a decimal to the nearest multiple value, the midpoint will be rounded up
        /// </summary>
        public static decimal RoundDecimalUp(decimal number, decimal multiple)
        {
            if (multiple == 0)
                return 0;
            bool negative = false;
            if (number < 0)
            {
                number = -number;
                negative = true;
            }
            int scale = 1;
            while (DoubleUtil.Frac(number * scale) != 0)
            {
                scale = scale * 10;
                if (scale > 10000000)
                    break;
            }
            while (DoubleUtil.Frac(multiple * scale) != 0)
            {
                scale = scale * 10;
                if (scale > 10000000)
                    break;
            }
            decimal numberscaled = Math.Round(number * scale);

            decimal multiplescaled = multiple * scale;
            decimal division = Math.Round(numberscaled / multiplescaled);
            decimal moddiv = (numberscaled - (multiplescaled * division));
            if (moddiv < (multiplescaled / 2))
                numberscaled = numberscaled - moddiv;
            else
                numberscaled = numberscaled + moddiv;
            decimal aresult = numberscaled / scale;
            if (negative)
                aresult = -aresult;

            return aresult;
        }
        /// <summary>
        /// Compare values, but an difference lower than epsilon will return as equal
        /// </summary>
        public static int CompareValue(decimal p1, decimal p2, decimal epsilon)
        {
            int aresult = 0;
            decimal dif = Math.Abs(p1 - p2);
            epsilon = Math.Abs(epsilon);
            if (dif >= epsilon)
            {
                if (p1 < p2)
                    aresult = -1;
                else
                    aresult = 1;
            }
            return aresult;
        }
        // Function cortesy of Hamza Al-Aradi (AradBox@hotmail.com)
        // Altered to support negative and not show fractionary
        // when there is not fractionary part
        // Converted to dot net by Toni Martir
        private static string NumToStr(long Num)
        {
            string Num2Str = "";
            const int hundred = 100;
            const int thousand = 1000;
            const int million = 1000000;
            const int billion = 1000000000;
            if (Num >= billion)
                if ((Num % billion) == 0)
                    Num2Str = NumToStr(Num / billion) + " Billion";
                else
                    Num2Str = NumToStr(Num / billion) + " Billion " + NumToStr(Num % billion);
            else
                if (Num >= million)
                if ((Num % million) == 0)
                    Num2Str = NumToStr(Num / million) + " Million";
                else
                    Num2Str = NumToStr(Num / million) + " Million " + NumToStr(Num % million);
            else
                    if (Num >= thousand)
                if ((Num % thousand) == 0)
                    Num2Str = NumToStr(Num / thousand) + " Thousand";
                else
                    Num2Str = NumToStr(Num / thousand) + " Thousand " + NumToStr(Num % thousand);
            else
                        if (Num >= hundred)
                if ((Num % hundred) == 0)
                    Num2Str = NumToStr(Num / hundred) + " Hundred";
                else
                    Num2Str = NumToStr(Num / hundred) + " Hundred " + NumToStr(Num % hundred);
            else
                switch (Num / 10)
                {
                    case 6:
                    case 7:
                    case 9:
                        if ((Num % 10) == 0)
                            Num2Str = NumToStr(Num / 10) + "ty";
                        else
                            Num2Str = NumToStr(Num / 10) + "ty-" + NumToStr(Num % 10);
                        break;
                    case 8:
                        if (Num == 80)
                            Num2Str = "Eighty";
                        else
                            Num2Str = "Eighty-" + NumToStr(Num % 10);
                        break;
                    case 5:
                        if (Num == 50)
                            Num2Str = "Fifty";
                        else
                            Num2Str = "Fifty-" + NumToStr(Num % 10);
                        break;
                    case 4:
                        if (Num == 40)
                            Num2Str = "Forty";
                        else
                            Num2Str = "Forty-" + NumToStr(Num % 10);
                        break;
                    case 3:
                        if (Num == 30)
                            Num2Str = "Thirty";
                        else
                            Num2Str = "Thirty-" + NumToStr(Num % 10);
                        break;
                    case 2:
                        if (Num == 20)
                            Num2Str = "Twenty";
                        else
                            Num2Str = "Twenty-" + NumToStr(Num % 10);
                        break;
                    case 1:
                    case 0:
                        switch (Num)
                        {
                            case 0:
                                Num2Str = "Zero";
                                break;
                            case 1:
                                Num2Str = "One";
                                break;
                            case 2:
                                Num2Str = "Two";
                                break;
                            case 3:
                                Num2Str = "Three";
                                break;
                            case 4:
                                Num2Str = "Four";
                                break;
                            case 5:
                                Num2Str = "Five";
                                break;
                            case 6:
                                Num2Str = "Six";
                                break;
                            case 7:
                                Num2Str = "Seven";
                                break;
                            case 8:
                                Num2Str = "Eight";
                                break;
                            case 9:
                                Num2Str = "Nine";
                                break;
                            case 10:
                                Num2Str = "Ten";
                                break;
                            case 11:
                                Num2Str = "Eleven";
                                break;
                            case 12:
                                Num2Str = "Twelve";
                                break;
                            case 13:
                                Num2Str = "Thirteen";
                                break;
                            case 14:
                                Num2Str = "Fourteen";
                                break;
                            case 15:
                                Num2Str = "Fifteen";
                                break;
                            case 16:
                                Num2Str = "Sixteen";
                                break;
                            case 17:
                                Num2Str = "Seventeen";
                                break;
                            case 18:
                                Num2Str = "Eightteen";
                                break;
                            case 19:
                                Num2Str = "Nineteen";
                                break;
                        }
                        break;
                }
            return Num2Str;
        }
        private static string NumberToTextEnglish(decimal amount)
        {
            amount = Math.Abs(amount);
            long Num = System.Convert.ToInt64(DoubleUtil.Trunc(amount));
            long Fracture = Convert.ToInt64(Math.Round(1000 * (amount - Num)));
            string aresult = NumToStr(Num);
            if (Fracture > 0)
                aresult = aresult + " and " + Fracture.ToString() + "/1000";
            return aresult;
        }
        // Esta funciÓn nos da la longitud del número que vamos a
        // deletrear
        private static long Longitud(long numero)
        {
            if ((numero / 10) == 0)
                return 1;
            else
                return 1 + Longitud(numero / 10);
        }

        private static string UnidadesToStrS(long numero, bool female)
        {
            string Unidades = "";
            switch (numero)
            {
                case 1:
                    if (!female)
                        Unidades = "un";
                    else
                        Unidades = "una";
                    break;
                case 2:
                    Unidades = "dos";
                    break;
                case 3:
                    Unidades = "tres";
                    break;
                case 4:
                    Unidades = "cuatro";
                    break;
                case 5:
                    Unidades = "cinco";
                    break;
                case 6:
                    Unidades = "seis";
                    break;
                case 7:
                    Unidades = "siete";
                    break;
                case 8:
                    Unidades = "ocho";
                    break;
                case 9:
                    Unidades = "nueve";
                    break;
            }
            return Unidades;
        }

        private static string DecenasToStrS(long numero, bool female)
        {
            string Decenas = "";
            switch (numero)
            {
                case 0:
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    Decenas = UnidadesToStrS(numero, female);
                    break;
                case 10:
                    Decenas = "diez";
                    break;
                case 11:
                    Decenas = "once";
                    break;
                case 12:
                    Decenas = "doce";
                    break;
                case 13:
                    Decenas = "trece";
                    break;
                case 14:
                    Decenas = "catorce";
                    break;
                case 15:
                    Decenas = "quince";
                    break;
                case 16:
                    Decenas = "dieciséis";
                    break;
                case 17:
                    Decenas = "diecisiete";
                    break;
                case 18:
                    Decenas = "dieciocho";
                    break;
                case 19:
                    Decenas = "diecinueve";
                    break;
                case 20:
                    Decenas = "veinte";
                    break;
                case 21:
                    if (!female)
                        Decenas = "veintiuno";
                    else
                        Decenas = "veintiuna";
                    break;
                case 22:
                    Decenas = "veintidós";
                    break;
                case 23:
                    Decenas = "veintitrés";
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                    Decenas = "veinti" + UnidadesToStrS(numero % 10, female);
                    break;
                case 30:
                    Decenas = "treinta";
                    break;
                case 40:
                    Decenas = "cuarenta";
                    break;
                case 50:
                    Decenas = "cincuenta";
                    break;
                case 60:
                    Decenas = "sesenta";
                    break;
                case 70:
                    Decenas = "setenta";
                    break;
                case 80:
                    Decenas = "ochenta";
                    break;
                case 90:
                    Decenas = "noventa";
                    break;
                default:
                    Decenas = DecenasToStrS(numero - (numero % 10), female) + " y " + UnidadesToStrS(numero % 10, female);
                    break;
            }
            return Decenas;
        }
        private static string CentenasToStrS(long numero, bool female)
        {
            string centenas = "";
            if ((numero >= 1) && (numero <= 99))
                centenas = DecenasToStrS(numero, female);
            else
                if (numero == 100)
                centenas = "cien";
            else
                    if ((numero >= 101) && (numero <= 199))
                centenas = "ciento " + DecenasToStrS(numero % 100, female);
            else
                        if (numero == 100)
                if (female)
                    centenas = "doscientas";
                else
                    centenas = "doscientos";
            else
                            if (numero == 500)
                if (female)
                    centenas = "quinientas";
                else
                    centenas = "quinientos";
            else
                                if ((numero >= 501) && (numero <= 599))
                if (female)
                    centenas = "quinientas " + DecenasToStrS(numero % 100, female);
                else
                    centenas = "quinientos " + DecenasToStrS(numero % 100, female);
            else
                                    if (numero == 700)
                if (female)
                    centenas = "setecientas";
                else
                    centenas = "setecientos";
            else
                                        if ((numero >= 701) && (numero <= 799))
                if (female)
                    centenas = "setecientas " + DecenasToStrS(numero % 100, female);
                else
                    centenas = "setecientos " + DecenasToStrS(numero % 100, female);
            else
                                            if (numero == 900)
                if (female)
                    centenas = "novecientas";
                else
                    centenas = "novecientos";
            else
                                                if ((numero >= 901) && (numero <= 999))
                if (female)
                    centenas = "novecientas " + DecenasToStrS(numero % 100, female);
                else
                    centenas = "novecientos " + DecenasToStrS(numero % 100, female);
            else
            {
                string cientosstr = UnidadesToStrS(numero / 100, female);
                if (cientosstr == "")
                {
                    centenas = "";
                }
                else
                {
                    if (female)
                        centenas = cientosstr + "cientas ";
                    else
                        centenas = cientosstr + "cientos ";
                }
                centenas = centenas + DecenasToStrS(numero % 100, female);
            }
            return centenas;
        }

        private static string UnidadesDeMillarToStrS(long numero, bool female)
        {
            string UnidadesDeMillar = "";
            if (numero > 999)
                if (numero > 1999)
                    UnidadesDeMillar = UnidadesToStrS(numero / 1000, female) + " mil " + CentenasToStrS(numero % 1000, female);
                else
                    UnidadesDeMillar = "mil " + CentenasToStrS(numero % 1000, female);
            else
                UnidadesDeMillar = CentenasToStrS(numero, female);
            return UnidadesDeMillar;
        }
        private static string DecenasDeMillarToStrS(long numero, bool female)
        {
            string DecenasDeMillar = "";
            if (numero > 9999)
                DecenasDeMillar = DecenasToStrS(numero / 1000, female) + " mil " + CentenasToStrS(numero % 1000, female);
            else
                DecenasDeMillar = UnidadesDeMillarToStrS(numero, female);
            return DecenasDeMillar;
        }
        private static string CentenasDeMillarToStrS(long numero, bool female)
        {
            string CentenasDeMillar = "";
            if (numero > 99999)
                CentenasDeMillar = CentenasToStrS(numero / 1000, female) + " mil " + CentenasToStrS(numero % 1000, female);
            else
                CentenasDeMillar = DecenasDeMillarToStrS(numero, female);
            return CentenasDeMillar;
        }
        private static string UnidadesDeMillonToStrS(long numero, bool female)
        {
            string UnidadesDeMillon = "";
            if (numero > 1999999)
                UnidadesDeMillon = UnidadesToStrS(numero / 1000000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
            else
                UnidadesDeMillon = "un millón " + CentenasDeMillarToStrS(numero % 1000000, female);
            return UnidadesDeMillon;
        }
        private static string DecenasDeMillonToStrS(long numero, bool female)
        {
            return DecenasToStrS(numero / 1000000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
        }
        private static string CentenasDeMillonToStrS(long numero, bool female)
        {
            return CentenasToStrS(numero / 1000000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
        }
        private static string MilesDeMillonToStrS(long numero, bool female)
        {
            return UnidadesDeMillarToStrS(numero / 100000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
        }
        private static string DecenasDeMilesDeMillonToStrS(long numero, bool female)
        {
            return DecenasDeMillarToStrS(numero / 1000000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
        }
        private static string CentenasDeMilesDeMillonToStrS(long numero, bool female)
        {
            return CentenasDeMillarToStrS(numero / 1000000, false) + " millones " + CentenasDeMillarToStrS(numero % 1000000, female);
        }
        private static string NumberToTextSpanish(decimal number, bool female)
        {
            string s = "";
            long centavos;
            long numero;
            string aresult = "";
            number = Math.Abs(number);
            numero = System.Convert.ToInt64(DoubleUtil.Trunc(number));
            centavos = System.Convert.ToInt64(Math.Round((number - numero) * 100));
            switch (Longitud(numero))
            {
                case 1:
                    s = UnidadesToStrS(numero, female);
                    break;
                case 2:
                    s = DecenasToStrS(numero, female);
                    break;
                case 3:
                    s = CentenasToStrS(numero, female);
                    break;
                case 4:
                    s = UnidadesDeMillarToStrS(numero, female);
                    break;
                case 5:
                    s = DecenasDeMillarToStrS(numero, female);
                    break;
                case 6:
                    s = CentenasDeMillarToStrS(numero, female);
                    break;
                case 7:
                    s = UnidadesDeMillonToStrS(numero, female);
                    break;
                case 8:
                    s = DecenasDeMillonToStrS(numero, female);
                    break;
                case 9:
                    s = CentenasDeMillonToStrS(numero, female);
                    break;
                case 10:
                    s = MilesDeMillonToStrS(numero, female);
                    break;
                case 11:
                    s = DecenasDeMilesDeMillonToStrS(numero, female);
                    break;
                case 12:
                    s = CentenasDeMilesDeMillonToStrS(numero, female);
                    break;
                default:
                    s = "Demasiado grande";
                    break;
            }
            if (centavos > 0)
            {
                switch (Longitud(centavos))
                {
                    case 1:
                        aresult = UnidadesToStrS(centavos, female);
                        break;
                    case 2:
                        aresult = DecenasToStrS(centavos, female);
                        break;
                }
                aresult = s + " con " + aresult;
            }
            else
            {
                aresult = s;

            }
            if (aresult.Length > 0)
                aresult = char.ToUpper(aresult[0]).ToString() + aresult.Substring(1, aresult.Length - 1);
            return aresult;
        }
        private static string UnidadesToStrC(long numero, bool female)
        {
            string Unidades = "";
            switch (numero)
            {
                case 1:
                    if (!female)
                        Unidades = "un";
                    else
                        Unidades = "una";
                    break;
                case 2:
                    Unidades = "dos";
                    break;
                case 3:
                    Unidades = "tres";
                    break;
                case 4:
                    Unidades = "quatre";
                    break;
                case 5:
                    Unidades = "cinc";
                    break;
                case 6:
                    Unidades = "sis";
                    break;
                case 7:
                    Unidades = "set";
                    break;
                case 8:
                    Unidades = "vuit";
                    break;
                case 9:
                    Unidades = "nou";
                    break;
            }
            return Unidades;
        }

        private static string DecenasToStrC(long numero, bool female)
        {
            string Decenas = "";
            switch (numero)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    Decenas = UnidadesToStrC(numero, female);
                    break;
                case 10:
                    Decenas = "deu";
                    break;
                case 11:
                    Decenas = "onze";
                    break;
                case 12:
                    Decenas = "dotze";
                    break;
                case 13:
                    Decenas = "tretze";
                    break;
                case 14:
                    Decenas = "catorze";
                    break;
                case 15:
                    Decenas = "quinze";
                    break;
                case 16:
                    Decenas = "setze";
                    break;
                case 17:
                    Decenas = "disset";
                    break;
                case 18:
                    Decenas = "divuit";
                    break;
                case 19:
                    Decenas = "dinou";
                    break;
                case 20:
                    Decenas = "vint";
                    break;
                case 21:
                    if (!female)
                        Decenas = "vint-i-un";
                    else
                        Decenas = "vint-i-una";
                    break;
                case 22:
                    Decenas = "vint-i-dos";
                    break;
                case 23:
                    Decenas = "vint-i-tres";
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                    Decenas = "vint-i-" + UnidadesToStrC(numero % 10, female);
                    break;
                case 30:
                    Decenas = "trenta";
                    break;
                case 31:
                    if (!female)
                        Decenas = "trenta-un";
                    else
                        Decenas = "trenta-una";
                    break;
                case 40:
                    Decenas = "quaranta";
                    break;
                case 41:
                    if (!female)
                        Decenas = "quaranta-un";
                    else
                        Decenas = "quaranta-una";
                    break;
                case 50:
                    Decenas = "cinquanta";
                    break;
                case 51:
                    if (!female)
                        Decenas = "cincuanta-un";
                    else
                        Decenas = "cincuanta-una";
                    break;
                case 60:
                    Decenas = "seixanta";
                    break;
                case 61:
                    if (!female)
                        Decenas = "seixanta-un";
                    else
                        Decenas = "seixanta-una";
                    break;
                case 70:
                    Decenas = "setanta";
                    break;
                case 71:
                    if (!female)
                        Decenas = "setanta-un";
                    else
                        Decenas = "setanta-una";
                    break;
                case 80:
                    Decenas = "vuitanta";
                    break;
                case 81:
                    if (!female)
                        Decenas = "vuitanta-un";
                    else
                        Decenas = "vuitanta-una";
                    break;
                case 90:
                    Decenas = "noranta";
                    break;
                case 91:
                    if (!female)
                        Decenas = "noranta-un";
                    else
                        Decenas = "noranta-una";
                    break;
                case 0:
                    break;
                default:
                    Decenas = DecenasToStrC(numero - (numero % 10), female) + "-" + UnidadesToStrC(numero % 10, female);
                    break;
            }
            return Decenas;
        }
        private static string CentenasToStrC(long numero, bool female)
        {
            string centenas = "";
            if ((numero >= 1) && (numero <= 99))
                centenas = DecenasToStrC(numero, female);
            else
                if (numero == 100)
                centenas = "cent";
            else
                    if ((numero >= 101) && (numero <= 199))
                centenas = "cent " + DecenasToStrC(numero % 100, female);
            else
                        if (numero == 100)
                if (female)
                    centenas = "dos-centes";
                else
                    centenas = "dos-cents";
            else
                            if (numero == 500)
                if (female)
                    centenas = "cinc-cents";
                else
                    centenas = "cinc-centes";
            else
                                if ((numero >= 501) && (numero <= 599))
                if (female)
                    centenas = "cinc-centes " + DecenasToStrC(numero % 100, female);
                else
                    centenas = "cinc-cents " + DecenasToStrC(numero % 100, female);
            else
                                    if (numero == 700)
                if (female)
                    centenas = "set-centes";
                else
                    centenas = "set-cents";
            else
                                        if ((numero >= 701) && (numero <= 799))
                if (female)
                    centenas = "set-centes " + DecenasToStrC(numero % 100, female);
                else
                    centenas = "set-cents " + DecenasToStrC(numero % 100, female);
            else
                                            if (numero == 900)
                if (female)
                    centenas = "nou-centes";
                else
                    centenas = "nou-cents";
            else
                                                if ((numero >= 901) && (numero <= 999))
                if (female)
                    centenas = "nou-centes " + DecenasToStrC(numero % 100, female);
                else
                    centenas = "nou-cents " + DecenasToStrC(numero % 100, female);
            else
                                                    if (female)
                centenas = UnidadesToStrC(numero / 100, female) + "-centes " +
                    DecenasToStrC(numero % 100, female);
            else
            {
                if (UnidadesToStrC(numero / 100, female).ToString().Trim().Length > 0)
                {
                    centenas = UnidadesToStrC(numero / 100, female) + "-cents " +
                        DecenasToStrC(numero % 100, female);
                }
            }
            return centenas;
        }

        private static string UnidadesDeMillarToStrC(long numero, bool female)
        {
            string UnidadesDeMillar = "";
            if (numero > 999)
                if (numero > 1999)
                    UnidadesDeMillar = UnidadesToStrC(numero / 1000, female) + " mil " + CentenasToStrC(numero % 1000, female);
                else
                    UnidadesDeMillar = "mil " + CentenasToStrC(numero % 1000, female);
            else
                UnidadesDeMillar = CentenasToStrC(numero, female);
            return UnidadesDeMillar;
        }
        private static string DecenasDeMillarToStrC(long numero, bool female)
        {
            string DecenasDeMillar = "";
            if (numero > 9999)
                DecenasDeMillar = DecenasToStrC(numero / 1000, female) + " mil " + CentenasToStrC(numero % 1000, female);
            else
                DecenasDeMillar = UnidadesDeMillarToStrC(numero, female);
            return DecenasDeMillar;
        }
        private static string CentenasDeMillarToStrC(long numero, bool female)
        {
            string CentenasDeMillar = "";
            if (numero > 99999)
                CentenasDeMillar = CentenasToStrC(numero / 1000, female) + " mil " + CentenasToStrC(numero % 1000, female);
            else
                CentenasDeMillar = DecenasDeMillarToStrC(numero, female);
            return CentenasDeMillar;
        }
        private static string UnidadesDeMillonToStrC(long numero, bool female)
        {
            string UnidadesDeMillon = "";
            if (numero > 1999999)
                UnidadesDeMillon = UnidadesToStrC(numero / 1000000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
            else
                UnidadesDeMillon = "un milió " + CentenasDeMillarToStrC(numero % 1000000, female);
            return UnidadesDeMillon;
        }
        private static string DecenasDeMillonToStrC(long numero, bool female)
        {
            return DecenasToStrC(numero / 1000000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
        }
        private static string CentenasDeMillonToStrC(long numero, bool female)
        {
            return CentenasToStrC(numero / 1000000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
        }
        private static string MilesDeMillonToStrC(long numero, bool female)
        {
            return UnidadesDeMillarToStrC(numero / 100000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
        }
        private static string DecenasDeMilesDeMillonToStrC(long numero, bool female)
        {
            return DecenasDeMillarToStrC(numero / 1000000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
        }
        private static string CentenasDeMilesDeMillonToStrC(long numero, bool female)
        {
            return CentenasDeMillarToStrC(numero / 1000000, false) + " milions " + CentenasDeMillarToStrC(numero % 1000000, female);
        }
        private static string NumberToTextCatalan(decimal number, bool female)
        {
            string s = "";
            long centavos;
            long numero;
            string aresult = "";
            number = Math.Abs(number);
            numero = System.Convert.ToInt64(DoubleUtil.Trunc(number));
            centavos = System.Convert.ToInt64(Math.Round((number - numero) * 100));
            switch (Longitud(numero))
            {
                case 1:
                    s = UnidadesToStrC(numero, female);
                    break;
                case 2:
                    s = DecenasToStrC(numero, female);
                    break;
                case 3:
                    s = CentenasToStrC(numero, female);
                    break;
                case 4:
                    s = UnidadesDeMillarToStrC(numero, female);
                    break;
                case 5:
                    s = DecenasDeMillarToStrC(numero, female);
                    break;
                case 6:
                    s = CentenasDeMillarToStrC(numero, female);
                    break;
                case 7:
                    s = UnidadesDeMillonToStrC(numero, female);
                    break;
                case 8:
                    s = DecenasDeMillonToStrC(numero, female);
                    break;
                case 9:
                    s = CentenasDeMillonToStrC(numero, female);
                    break;
                case 10:
                    s = MilesDeMillonToStrC(numero, female);
                    break;
                case 11:
                    s = DecenasDeMilesDeMillonToStrC(numero, female);
                    break;
                case 12:
                    s = CentenasDeMilesDeMillonToStrC(numero, female);
                    break;
                default:
                    s = "Massa gran";
                    break;
            }
            if (centavos > 0)
            {
                switch (Longitud(centavos))
                {
                    case 1:
                        aresult = UnidadesToStrC(centavos, female);
                        break;
                    case 2:
                        aresult = DecenasToStrC(centavos, female);
                        break;
                }
                aresult = s + " amb " + aresult;
            }
            else
            {
                aresult = s;

            }
            if (aresult.Length > 0)
                aresult = char.ToUpper(aresult[0]).ToString() + aresult.Substring(1, aresult.Length - 1);
            return aresult;
        }
        private static string NumberToTextSpanishMex(decimal number, bool female)
        {
            return "";
        }
        private static string NumberToTextPortuguese(decimal number)
        {
            return "";
        }
        private static string NumberToTextTurkish(decimal number)
        {
            return "";
        }
        private static string NumberToTextLithuanian(decimal number, bool female)
        {
            return "";
        }
        /// <summary>
        /// Converts a number to his representation in natural words
        /// </summary>
        public static string NumberToText(decimal number, bool female, int lang)
        {
            string aresult = "";
            switch (lang)
            {
                case -1:
                case 0:
                    aresult = NumberToTextEnglish(number);
                    break;
                case 1:
                    aresult = NumberToTextSpanish(number, female);
                    break;
                case 2:
                    aresult = NumberToTextCatalan(number, female);
                    break;
                case 4:
                    aresult = NumberToTextPortuguese(number);
                    break;
                case 7:
                    aresult = NumberToTextTurkish(number);
                    break;
                case 8:
                    aresult = NumberToTextLithuanian(number, female);
                    break;
                case 13:
                    aresult = NumberToTextSpanishMex(number, female);
                    break;
            }
            return aresult;
        }
        /// <summary>
        /// Converts a numeric decimal value to sql literal
        /// </summary>
        /// <param name="nvalue">Value to convert</param>
        /// <returns>Formatted string to insert directly into SQL Statement</returns>
        public static string DecimalToSqlLiteral(decimal nvalue)
        {
            System.Globalization.NumberFormatInfo ninfo = new System.Globalization.NumberFormatInfo();
            ninfo.NegativeSign = "-";
            ninfo.PositiveSign = "+";
            ninfo.CurrencySymbol = "";
            ninfo.NumberGroupSeparator = "";
            string nresult = nvalue.ToString("", ninfo);
            return nresult;
        }
    }
}
