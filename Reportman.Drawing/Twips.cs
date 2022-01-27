using System;


namespace Reportman.Drawing
{
    /// <summary>
    /// Provide utitilies and constants related to Twips measurement unit.
    /// One inch contains 1440 twips
    /// </summary>
    public class Twips
    {
        /// <summary>
        /// One inch measures 1440 twips
        /// </summary>
        public const int TWIPS_PER_INCH = 1440;
        /// <summary>
        /// One inch measures 2.54 centimeters
        /// </summary>
        public const decimal CMS_PER_INCH = 2.54M;
        /// <summary>
        /// One centimeter measures 1440/2.54 twips
        /// </summary>
        public const decimal TWIPS_PER_CM = TWIPS_PER_INCH / CMS_PER_INCH;
        public static int AlignToGridTwips(int nx, int gridx, int gridy)
        {
            nx = ((nx + gridx / 2) / gridx) * gridx;
            return nx;
        }
        /// <summary>
        /// Helper to convert Twips to centimeters
        /// </summary>
        public static decimal TwipsToCms(int atwips)
        {
            decimal at = atwips;
            return (at / TWIPS_PER_CM);
        }
        /// <summary>
        /// Helper to convert Twips to inch
        /// </summary>
        public static decimal TwipsToInch(int atwips)
        {
            decimal at = atwips;
            return (at / TWIPS_PER_INCH);
        }
        /// <summary>
        /// Helper to convert centimeters to Twips
        /// </summary>
        public static int CmsToTwips(decimal acms)
        {
            return ((int)Math.Round(TWIPS_PER_CM * acms));
        }
        /// <summary>
        /// Helper to convert inch to Twips 
        /// </summary>
        public static int InchToTwips(decimal ainch)
        {
            return ((int)Math.Round(TWIPS_PER_INCH * ainch));
        }
        /// <summary>
        /// Returns the abreviated description for a measurement unit
        /// </summary>
        /// <param name="unit">The unit type</param>
        /// <returns>A string</returns>
        public static string TranslateUnit(Units unit)
        {
            if (unit == Units.Cms)
                return Translator.TranslateStr(1437);
            else
                return Translator.TranslateStr(1436);
        }
        /// <summary>
        /// Returns the default measurement unit form current region as string
        /// </summary>
        public static string DefaultUnitString()
        {
            if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
                return TranslateUnit(Units.Cms);
            else
                return TranslateUnit(Units.Inch);
        }
        /// <summary>
        /// Returns the default measurement unit form current region
        /// </summary>
        public static Units DefaultUnit()
        {
            if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
                return Units.Cms;
            else
                return Units.Inch;
        }
        /// <summary>
        /// Returns a string in default measurement, converting twips to that unit
        /// </summary>
        public static string TextFromTwips(int twips)
        {
            return UnitsFromTwips(twips).ToString("##,##0.000");
        }
        /// <summary>
        /// Returns a twips value from a text, it uses the default measurement unit
        /// </summary>
        public static int TwipsFromText(string text)
        {
            decimal floatvalue = System.Convert.ToDecimal(text);
            return TwipsFromUnits(floatvalue);
        }
        /// <summary>
        /// Returns a double value, converting twips to default unit
        /// </summary>
        public static decimal UnitsFromTwips(int twips)
        {
            if (DefaultUnit() == Units.Cms)
                return Twips.TwipsToCms(twips);
            else
                return Twips.TwipsToInch(twips);
        }
        /// <summary>
        /// Returns a double value, converting the default unit to twips
        /// </summary>
        public static int TwipsFromUnits(decimal measure)
        {
            if (DefaultUnit() == Units.Cms)
                return Twips.CmsToTwips(measure);
            else
                return Twips.InchToTwips(measure);
        }
    }

}
