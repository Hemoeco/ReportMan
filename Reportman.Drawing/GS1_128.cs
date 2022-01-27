using System;
using System.Text;

namespace Reportman.Drawing
{
    public class GS1_128
    {
        public DateTime ExpirationDate = DateTime.MinValue;
        public DateTime SellByDate = DateTime.MinValue;
        public string SSCC = "";
        public string SSCC_Company = "";
        public string SSCC_Number = "";
        public string ProductCode = "";
        public string ProductionDateTime = "";
        public DateTime ProductionDate = DateTime.MinValue;
        public DateTime PackagingDate = DateTime.MinValue;
        public string ProductVariant = "";
        public string SerialNumber = "";
        public string HIBCC = "";
        public string LotNumber = "";
        public string SecondSerialNumber = "";
        public decimal Quantity = 0.0m;
        public decimal ProductVolume = 0.0m;
        public decimal ContainerGrossWeight = 0.0m;
        public string CustomerPurchaseNumber = "";
        public string BatchNumber = "";
        public string Mutual = "";
        public string InternalCode = "";
        // 31
        public decimal ProductNetWeightKg = 0m;
        public decimal ProductLengthMeters = 0m;
        public decimal ProductWidthMeters = 0m;
        public decimal ProductDepthMeters = 0m;
        public decimal ProductAreaMeters2 = 0m;
        public decimal ProductNetVolumeLiters = 0m;
        public decimal ProductNetVolumeMeters3 = 0m;
        // 32
        public decimal ProductNetWeightPounds = 0m;
        public decimal ProductLengthInch = 0m;
        public decimal ProductLengthFeet = 0m;
        public decimal ProductLengthYard = 0m;
        public decimal ProductWidthInch = 0m;
        public decimal ProductWidthFeet = 0m;
        public decimal ProductWidthYard = 0m;
        public decimal ProductDepthInch = 0m;
        public decimal ProductDepthFeet = 0m;
        public decimal ProductDepthYard = 0m;
        // 33
        public decimal ContainerGrossWeightKg = 0m;
        public decimal ContainerLengthMeters = 0m;
        public decimal ContainerWidthMeters = 0m;
        public decimal ContainerDepthMeters = 0m;
        public decimal ContainerAreaMeters2 = 0m;
        public decimal ContainerGrossVolumeLiters = 0m;
        public decimal ContainerGrossVolumeMeters3 = 0m;
        // 34
        public decimal ContainerGrossWeightPounds = 0m;
        public decimal ContainerLengthInch = 0m;
        public decimal ContainerLengthFeet = 0m;
        public decimal ContainerLengthYard = 0m;
        public decimal ContainerWidthInch = 0m;
        public decimal ContainerWidthFeet = 0m;
        public decimal ContainerWidthYard = 0m;
        public decimal ContainerDepthInch = 0m;
        public decimal ContainerDepthFeet = 0m;
        public decimal ContainerDepthYard = 0m;
        // 35
        public decimal ProductAreaInch2 = 0m;
        public decimal ProductAreaFeet2 = 0m;
        public decimal ProductAreaYard2 = 0m;
        public decimal ContainerAreaInch2 = 0m;
        public decimal ContainerAreaFeet2 = 0m;
        public decimal ContainerAreaYard2 = 0m;
        public decimal NetWeightTroyOunces = 0m;
        public decimal NetWeightOunces = 0m;
        // 36
        public decimal ProductVolumeQuarts = 0m;
        public decimal ProductVolumeGallons = 0m;
        public decimal ContainerVolumeQuarts = 0m;
        public decimal ContainerVolumeGallons = 0m;
        public decimal ProductVolumeInch3 = 0m;
        public decimal ProductVolumeFeet3 = 0m;
        public decimal ProductVolumeYard3 = 0m;
        public decimal ContainerVolumeInch3 = 0m;
        public decimal ContainerVolumeFeet3 = 0m;
        public decimal ContainerVolumeYard3 = 0m;




        public decimal DecodeNumber(string nbarcode, ref int idx)
        {
            if (!(char.IsDigit(nbarcode[idx])))
                throw new Exception("Invalid digit number DecodeNumber 128");
            int decimalplaces = System.Convert.ToInt32(nbarcode[idx] + "");
            if (decimalplaces > 6)
                throw new Exception("Invalid digit number decimal places DecodeNumber 128");
            idx++;
            decimal intpart = 0;
            if (decimalplaces == 0)
            {
                intpart = System.Convert.ToDecimal(nbarcode.Substring(idx, 6));
            }
            else
            {
                if (decimalplaces > 4)
                    decimalplaces = 4;
                intpart = System.Convert.ToDecimal(nbarcode.Substring(idx, 6 - decimalplaces));
                decimal decpart = System.Convert.ToDecimal(nbarcode.Substring(idx + 6 - decimalplaces, decimalplaces));
                while (decimalplaces > 0)
                {
                    decpart = decpart / 10.0m;
                    decimalplaces--;
                }
                intpart = intpart + decpart;
            }
            idx = idx + 6;
            return intpart;
        }
        public string Advance(string nbarcode, ref int idx)
        {
            StringBuilder nresult = new StringBuilder();
            for (; idx < nbarcode.Length; idx++)
            {
                if (nbarcode[idx] == (char)29)
                {
                    idx++;
                    break;
                }
                else
                    nresult.Append(nbarcode[idx]);
            }
            return nresult.ToString();
        }
        public void Decode(string nbarcode)
        {
            int idx = 0;
            string previousprefix = "";
            while (idx < nbarcode.Length - 1)
            {
                string prefix2 = nbarcode.Substring(idx, 2);
                if (prefix2[0]==(char)29)
                {
                    idx++;
                    if (!(idx < nbarcode.Length - 1))
                        break;
                    prefix2 = nbarcode.Substring(idx, 2);
                }
                string prefix3 = nbarcode.Substring(idx, 3);
                string prefix4 = prefix3;
                if (nbarcode.Length > idx + 2)
                    prefix4 = nbarcode.Substring(idx, 4);

                switch (prefix2)
                {
                    case "00":
                        if (nbarcode.Length < 20)
                            throw new Exception("Incorrect SSCC Length, expected 20");
                        SSCC = nbarcode.Substring(idx + 2, 18);
                        SSCC_Company = SSCC.Substring(1, 8);
                        SSCC_Number = SSCC.Substring(9, 9);
                        idx = idx + 20;
                        break;
                    case "01":
                    case "02":
                        ProductCode = nbarcode.Substring(idx + 2, 14);
                        idx = idx + 16;
                        break;
                    case "10":
                        idx = idx + 2;
                        BatchNumber = Advance(nbarcode, ref idx); ;

                        break;
                    case "11":
                        ProductionDate = DecodeDate(nbarcode, idx + 2);
                        idx = idx + 8;
                        break;
                    case "13":
                        PackagingDate = DecodeDate(nbarcode, idx + 2);
                        idx = idx + 8;
                        break;
                    case "15":
                        SellByDate = DecodeDate(nbarcode, idx + 2);
                        idx = idx + 8;
                        break;
                    case "17":
                        ExpirationDate = DecodeDate(nbarcode, idx + 2);
                        idx = idx + 8;
                        break;
                    case "20":
                        ProductVariant = nbarcode.Substring(idx + 2, 2);
                        idx = idx + 4;
                        break;
                    case "21":
                        idx = idx + 2;
                        SerialNumber = Advance(nbarcode, ref idx); ;


                        break;
                    case "25":
                        switch (prefix3)
                        {
                            case "250":
                                idx = idx + 3;
                                string SecondarySerialNumber = Advance(nbarcode, ref idx);
                                if (SerialNumber.Length == 0)
                                    SerialNumber = SecondarySerialNumber;
                                break;
                            case "251":
                                idx = idx + 3;
                                string ReferenceToSource = Advance(nbarcode, ref idx);
                                break;
                            case "253":
                                idx = idx + 3;
                                string GlobalDocumentTypeIdentifier = Advance(nbarcode, ref idx);
                                break;
                            case "254":
                                idx = idx + 3;
                                string GLNExtensionComponent = Advance(nbarcode, ref idx);
                                break;
                            case "255":
                                idx = idx + 3;
                                string GlobalCouponNumber = Advance(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "37":
                        idx = idx + 2;
                        string numberunits = Advance(nbarcode, ref idx);
                        if (DoubleUtil.IsNumeric(numberunits, System.Globalization.NumberStyles.Number))
                            Quantity = System.Convert.ToDecimal(numberunits);
                        break;
                    case "30":
                        idx = idx + 2;
                        string numberunits2 = Advance(nbarcode, ref idx);
                        if (DoubleUtil.IsNumeric(numberunits2, System.Globalization.NumberStyles.Number))
                            Quantity = System.Convert.ToDecimal(numberunits2);
                        break;
                    case "40":
                        if (prefix3 == "400")
                        {
                            idx = idx + 3;
                            CustomerPurchaseNumber = Advance(nbarcode, ref idx);
                        }
                        break;
                    case "24":
                        if (prefix3 == "240")
                        {
                            idx = idx + 3;
                            Advance(nbarcode, ref idx);
                        }
                        else
                            throw new Exception("Unimplemented prefix: " + prefix3);
                        break;
                    case "90":
                        idx = idx + 2;
                        Mutual = Advance(nbarcode, ref idx);
                        break;
                    case "91":
                    case "92":
                    case "93":
                    case "94":
                    case "95":
                    case "96":
                    case "97":
                    case "98":
                    case "99":
                        idx = idx + 2;
                        InternalCode = Advance(nbarcode, ref idx);
                        break;
                    case "31":
                        switch (prefix3)
                        {
                            case "310":
                                idx = idx + 3;
                                ProductNetWeightKg = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "311":
                                idx = idx + 3;
                                ProductLengthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "312":
                                idx = idx + 3;
                                ProductWidthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "313":
                                idx = idx + 3;
                                ProductDepthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "314":
                                idx = idx + 3;
                                ProductAreaMeters2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "315":
                                idx = idx + 3;
                                ProductNetVolumeLiters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "316":
                                idx = idx + 3;
                                ProductNetVolumeMeters3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "32":
                        switch (prefix3)
                        {
                            case "320":
                                idx = idx + 3;
                                ProductNetWeightPounds = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "321":
                                idx = idx + 3;
                                ProductLengthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "322":
                                idx = idx + 3;
                                ProductLengthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "323":
                                idx = idx + 3;
                                ProductLengthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "324":
                                idx = idx + 3;
                                ProductWidthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "325":
                                idx = idx + 3;
                                ProductWidthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "326":
                                idx = idx + 3;
                                ProductWidthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "327":
                                idx = idx + 3;
                                ProductDepthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "328":
                                idx = idx + 3;
                                ProductDepthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "329":
                                idx = idx + 3;
                                ProductDepthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "33":
                        switch (prefix3)
                        {
                            case "330":
                                idx = idx + 3;
                                ContainerGrossWeightKg = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "331":
                                idx = idx + 3;
                                ContainerLengthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "332":
                                idx = idx + 3;
                                ContainerWidthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "333":
                                idx = idx + 3;
                                ContainerDepthMeters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "334":
                                idx = idx + 3;
                                ContainerAreaMeters2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "335":
                                idx = idx + 3;
                                ContainerGrossVolumeLiters = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "336":
                                idx = idx + 3;
                                ContainerGrossVolumeMeters3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "34":
                        switch (prefix3)
                        {
                            case "340":
                                idx = idx + 3;
                                ContainerGrossWeightPounds = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "341":
                                idx = idx + 3;
                                ContainerLengthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "342":
                                idx = idx + 3;
                                ContainerLengthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "343":
                                idx = idx + 3;
                                ContainerLengthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "344":
                                idx = idx + 3;
                                ContainerWidthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "345":
                                idx = idx + 3;
                                ContainerWidthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "346":
                                idx = idx + 3;
                                ContainerWidthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "347":
                                idx = idx + 3;
                                ContainerDepthInch = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "348":
                                idx = idx + 3;
                                ContainerDepthFeet = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "349":
                                idx = idx + 3;
                                ContainerDepthYard = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "35":
                        switch (prefix3)
                        {
                            case "350":
                                idx = idx + 3;
                                ProductAreaInch2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "351":
                                idx = idx + 3;
                                ProductAreaFeet2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "352":
                                idx = idx + 3;
                                ProductAreaYard2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "353":
                                idx = idx + 3;
                                ContainerAreaInch2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "354":
                                idx = idx + 3;
                                ContainerAreaFeet2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "355":
                                idx = idx + 3;
                                ContainerAreaYard2 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "356":
                                idx = idx + 3;
                                NetWeightTroyOunces = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "357":
                                idx = idx + 3;
                                NetWeightOunces = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "36":
                        switch (prefix3)
                        {
                            case "360":
                                idx = idx + 3;
                                ProductVolumeQuarts = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "361":
                                idx = idx + 3;
                                ProductVolumeGallons = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "362":
                                idx = idx + 3;
                                ContainerVolumeQuarts = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "363":
                                idx = idx + 3;
                                ContainerVolumeGallons = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "364":
                                idx = idx + 3;
                                ProductVolumeInch3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "365":
                                idx = idx + 3;
                                ProductVolumeFeet3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "366":
                                idx = idx + 3;
                                ProductVolumeYard3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "367":
                                idx = idx + 3;
                                ContainerVolumeInch3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "368":
                                idx = idx + 3;
                                ContainerVolumeFeet3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            case "369":
                                idx = idx + 3;
                                ContainerVolumeYard3 = DecodeNumber(nbarcode, ref idx);
                                break;
                            default:
                                throw new Exception("Unimplemented prefix: " + prefix3);
                        }
                        break;
                    case "80":
                        switch (prefix3)
                        {
                            case "800":
                                switch (prefix4)
                                {
                                    // Fecha y hora de fabricación de 8 a 12
                                    case "8008":
                                        idx = idx + 4;

                                        ProductionDateTime = Advance(nbarcode, ref idx); ;

                                        break;
                                }
                                break;
                        }
                        break;
                    default:
                        {
                            if (previousprefix == "01")
                                idx--;
                            else
                                throw new Exception("Unimplemented prefix: " + prefix2);
                        }
                        break;
                }
                previousprefix = prefix2;
            }
        }
        private DateTime DecodeDate(string barcode, int position)
        {
            string nyear = barcode.Substring(position, 2);
            int prefixyear = System.Convert.ToInt32(nyear);
            if (prefixyear > 80)
            {
                prefixyear = 1900 + prefixyear;
            }
            else
            {
                prefixyear = 2000 + prefixyear;
            }
            int nday = System.Convert.ToInt32(barcode.Substring(position + 4, 2));
            int nmonth = System.Convert.ToInt32(barcode.Substring(position + 2, 2));
            if (nday == 0)
            {
                nday = DateTime.DaysInMonth(prefixyear, nmonth);
            }
            return new DateTime(prefixyear, nmonth,
                nday);
        }
    }
}
