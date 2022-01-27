#region Copyright
/*
 *  Report Manager:  Database Reporting tool for .Net and Mono
 *
 *     The contents of this file are subject to the MPL License
 *     with optional use of GPL or LGPL licenses.
 *     You may not use this file except in compliance with the
 *     Licenses. You may obtain copies of the Licenses at:
 *     http://reportman.sourceforge.net/license
 *
 *     Software is distributed on an "AS IS" basis,
 *     WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations.
 *
 *  Copyright (c) 1994 - 2008 Toni Martir (toni@reportman.es)
 *  All Rights Reserved.
*/
#endregion

using System;
using System.IO;
using Reportman.Drawing;

namespace Reportman.Reporting
{
	/// <summary>
	/// Functions provided in expression evaluator
	/// </summary>
	class IdenSinus : IdenFunction
	{
		public IdenSinus(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "SIN";
			FModel = "function Sin(value:double):double";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				if (!Params[0].IsNumber())
				{
					throw new NamedException(Translator.TranslateStr(438), "SIN");
				}
				aresult = Math.Sin(Params[0].AsDouble);
			}
			return aresult;
		}
	}
	class IdenMax : IdenFunction
	{
		public IdenMax(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "MAX";
			FModel = "function Max(value1,value2:Variant):Variant";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType == VariantType.Null)
				throw new NamedException(Translator.TranslateStr(438), "MAX");
			if (Params[1].VarType == VariantType.Null)
				throw new NamedException(Translator.TranslateStr(438), "MAX");
			if (Params[0] > Params[1])
				aresult = Params[0];
			else
				aresult = Params[1];
			return aresult;
		}
	}
	class IdenUpperCase : IdenFunction
	{
		public IdenUpperCase(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "UPPERCASE";
			FModel = "function UpperCase(value:string):string";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				if (Params[0].VarType != VariantType.String)
				{
					throw new NamedException(Translator.TranslateStr(438), "UPPERCASE");
				}
				aresult = Params[0].AsString.ToUpper();
			}
			return aresult;
		}
	}
    class IdenStringToBin : IdenFunction
    {
        public IdenStringToBin(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "STRINGTOBIN";
            FModel = "function StringToBin(value:string):binary";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (Params[0].VarType != VariantType.Null)
            {
                if (Params[0].VarType != VariantType.String)
                {
                    throw new NamedException(Translator.TranslateStr(438), "STRINGTOBIN");
                }
                byte[] bytes = Convert.FromBase64String(Params[0].AsString);
                aresult.SetStream(new MemoryStream(bytes));
            }
            return aresult;
        }
    }
    class IdenDay : IdenFunction
    {
        public IdenDay(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "DAY";
            FModel = "function DAY(value:DateTime):Integer";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (Params[0].VarType != VariantType.Null)
            {
                if (Params[0].VarType != VariantType.DateTime)
                {
                    throw new NamedException(Translator.TranslateStr(438), "DAY");
                }
                aresult = ((DateTime)Params[0]).Day;
            }
            return aresult;
        }
    }
    class IdenMonthName : IdenFunction
    {
        public IdenMonthName(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "MONTHNAME";
            FModel = "function MonthName(value:Integer,DateTime):string";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (Params[0].VarType != VariantType.Null)
            {
                if (Params[0].VarType == VariantType.Integer)
                {
                    aresult = GetMonthName(Params[0].AsInteger, EvalObject.Language);
                }
                else
                    if (Params[0].VarType == VariantType.DateTime)
                    {
                        aresult = GetMonthName(((DateTime)Params[0]).Month, EvalObject.Language);
                    }
                    else
                    throw new NamedException(Translator.TranslateStr(438), "MONTHNAME");
            }
            return aresult;
        }
        static string GetMonthName(int month, int language)
        {
            string nresult = "";
            switch (language)
            {
                case 1:
                    switch (month)
                    {
                        case 1:
                            nresult = "Enero";
                            break;
                        case 2:
                            nresult = "Febrero";
                            break;
                        case 3:
                            nresult = "Marzo";
                            break;
                        case 4:
                            nresult = "Abril";
                            break;
                        case 5:
                            nresult = "Mayo";
                            break;
                        case 6:
                            nresult = "Junio";
                            break;
                        case 7:
                            nresult = "Julio";
                            break;
                        case 8:
                            nresult = "Agosto";
                            break;
                        case 9:
                            nresult = "Setiembre";
                            break;
                        case 10:
                            nresult = "Octubre";
                            break;
                        case 11:
                            nresult = "Noviembre";
                            break;
                        case 12:
                            nresult = "Diciembre";
                            break;
                    }
                    break;
                case 0:
                    switch (month)
                    {
                        case 1:
                            nresult = "January";
                            break;
                        case 2:
                            nresult = "February";
                            break;
                        case 3:
                            nresult = "March";
                            break;
                        case 4:
                            nresult = "April";
                            break;
                        case 5:
                            nresult = "May";
                            break;
                        case 6:
                            nresult = "June";
                            break;
                        case 7:
                            nresult = "July";
                            break;
                        case 8:
                            nresult = "August";
                            break;
                        case 9:
                            nresult = "September";
                            break;
                        case 10:
                            nresult = "October";
                            break;
                        case 11:
                            nresult = "November";
                            break;
                        case 12:
                            nresult = "December";
                            break;
                    }
                    break;
                case 2:
                    switch (month)
                    {
                        case 1:
                            nresult = "Gener";
                            break;
                        case 2:
                            nresult = "Febrer";
                            break;
                        case 3:
                            nresult = "Març";
                            break;
                        case 4:
                            nresult = "Abril";
                            break;
                        case 5:
                            nresult = "Maig";
                            break;
                        case 6:
                            nresult = "Juny";
                            break;
                        case 7:
                            nresult = "Juliol";
                            break;
                        case 8:
                            nresult = "Agost";
                            break;
                        case 9:
                            nresult = "Setembre";
                            break;
                        case 10:
                            nresult = "Octubre";
                            break;
                        case 11:
                            nresult = "Novembre";
                            break;
                        case 12:
                            nresult = "Desembre";
                            break;
                    }
                    break;
            }
            return nresult;
        }
    }
    class IdenYear : IdenFunction
    {
        public IdenYear(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "Year";
            FModel = "function Year(value:DateTime):Integer";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (Params[0].VarType != VariantType.Null)
            {
                    if (Params[0].VarType == VariantType.DateTime)
                    {
                        aresult = ((DateTime)Params[0]).Year;
                    }
                    else
                        throw new NamedException(Translator.TranslateStr(438), "MONTHNAME");
            }
            return aresult;
        }
    }

	class IdenFormatStr : IdenFunction
	{
		public IdenFormatStr(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "FORMATSTR";
			FModel = "function FormatStr(format:string;value:Variant):string";
		}
		protected override Variant GetValue()
		{
			if (Params[0].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "FORMATSTR");
			}
			Variant aresult = new Variant();
			string format = (string)Params[0];
			aresult = "";
			if (Params[0].VarType == VariantType.Null)
				Params[0] = "";
			aresult = Params[1].ToString(format, ParamType.Unknown, true);
			return aresult;
		}
	}
	class IdenTrim : IdenFunction
	{
		public IdenTrim(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "TRIM";
			FModel = "function Trim(value:string):string";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				if (Params[0].VarType != VariantType.String)
				{
					throw new NamedException(Translator.TranslateStr(438), "TRIM");
				}
				aresult = Params[0].AsString.Trim();
			}
			return aresult;
		}
	}
	class IdenLeft : IdenFunction
	{
		public IdenLeft(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "Left";
			FModel = "function Left(value:string;count:integer):string";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				String astring = Params[0];
				int count = Params[1];
				if (astring.Length < count)
					count = astring.Length;
				aresult = astring.Substring(0, count);
			}
			return aresult;
		}
	}
	class IdenRight : IdenFunction
	{
		public IdenRight(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "Right";
			FModel = "function Right(value:string;count:integer):string";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				String astring = Params[0];
				int count = Params[1];
				if (astring.Length < count)
					count = astring.Length;
				aresult = astring.Substring(astring.Length - count, count);
			}
			return aresult;
		}
	}
	class IdenLength : IdenFunction
	{
		public IdenLength(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "Length";
			FModel = "function Length(value:string):integer";
		}
		protected override Variant GetValue()
		{
			Variant aresult = 0;
			if (Params[0].VarType != VariantType.Null)
			{
				if (Params[0].VarType != VariantType.String)
				{
					throw new NamedException(Translator.TranslateStr(438), "LENGTH");
				}
				String astring = Params[0];
				aresult = astring.Length;
			}
			return aresult;
		}
	}
	class IdenIsInteger : IdenFunction
	{
		public IdenIsInteger(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "IsInteger";
			FModel = "function IsInteger(value:string):boolean";
		}
		protected override Variant GetValue()
		{
			if (Params[0].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "ISINTEGER");
			}
			Variant aresult = true;
			try
			{
				System.Convert.ToInt32((String)Params[0]);
			}
			catch
			{
				aresult=false;
			}
			return aresult;
		}
	}
	class IdenIsNumeric : IdenFunction
	{
		public IdenIsNumeric(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "IsNumeric";
			FModel = "function IsNumeric(value:string):boolean";
		}
		protected override Variant GetValue()
		{
			if (Params[0].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "ISNUMERIC");
			}
			Variant aresult = true;
			try
			{
				System.Convert.ToDouble((String)Params[0]);
			}
			catch
			{
				aresult=false;
			}
			return aresult;
		}
	}
	class IdenIsValidDateTime : IdenFunction
	{
		public IdenIsValidDateTime(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "IsValidDateTime";
			FModel = "function IsValidDateTime(value:string):boolean";
		}
		protected override Variant GetValue()
		{
			if (Params[0].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "ISVALIDDATETIME");
			}
			Variant aresult = true;
			try
			{
				System.Convert.ToDateTime((String)Params[0]);
			}
			catch
			{
				aresult=false;
			}
			return aresult;
		}
	}
	class IdenCheckExpression : IdenFunction
	{
		public IdenCheckExpression(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "CheckExpression";
			FModel = "function CheckExpression(value:string):boolean";
		}
		protected override Variant GetValue()
		{
			if (Params[0].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "CHECKEXPRESSION");
			}
			if (Params[1].VarType != VariantType.String)
			{
				throw new NamedException(Translator.TranslateStr(438), "CHECKEXPRESSION");
			}
			Variant aresult = EvalObject.EvaluateText(Params[0]);
			if (aresult==false)
				throw new UnNamedException(Params[1]);
			return aresult;
		}
	}
	class IdenPos : IdenFunction
	{
		public IdenPos(Evaluator eval)
			: base(eval)
		{
			SetParamCount(2);
			Name = "POS";
			FModel = "function Pos(substring,fullstring:string):integer";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				String substring = Params[0];
				String fullstring = Params[1];
				aresult = fullstring.IndexOf(substring)+1;
			}
			return aresult;
		}
	}
	class IdenSubStr : IdenFunction
	{
		public IdenSubStr(Evaluator eval)
			: base(eval)
		{
			SetParamCount(3);
			Name = "SubStr";
			FModel = "function SubStr(value:string;index,count:integer):string";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				String astring = Params[0];
                if (astring.Length > 0)
                {
                    int index = Params[1];
                    int count = Params[2];
                    if (astring.Length < (count - index))
                        count = astring.Length;
                    if (count <= 0)
                        aresult = "";
                    else
                    {
                        aresult = astring.SafeSubstring(index - 1, count);
                        // aresult = astring.Substring(index - 1, count);
                    }
                }
                else
                    aresult = "";
			}
			return aresult;
		}
	}
	class IdenLowerCase : IdenFunction
	{
		public IdenLowerCase(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "LOWERCASE";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				if (Params[0].VarType != VariantType.String)
				{
					throw new NamedException(Translator.TranslateStr(438), "LOWERCASE");
				}
				aresult = Params[0].AsString.ToLower();
			}
			return aresult;
		}
	}
	class IdenFileExists : IdenFunction
	{
		public IdenFileExists(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "FILEEXISTS";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			aresult = false;
			if (Params[0].VarType != VariantType.Null)
			{
				if (Params[0].VarType != VariantType.String)
				{
					throw new NamedException(Translator.TranslateStr(438), Name);
				}
				FileInfo afileinfo = new FileInfo(Params[0].AsString);
				aresult = afileinfo.Exists;
			}
			return aresult;
		}
	}
	class IdenToday : IdenFunction
	{
		public IdenToday(Evaluator eval)
			: base(eval)
		{
			SetParamCount(0);
			Name = "TODAY";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			aresult = System.DateTime.Today;
			return aresult;
		}
	}
	class IdenNow : IdenFunction
	{
		public IdenNow(Evaluator eval)
			: base(eval)
		{
			SetParamCount(0);
			Name = "NOW";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			aresult = System.DateTime.Now;
			return aresult;
		}
	}

	class IdenStr : IdenFunction
	{
		public IdenStr(Evaluator eval)
			: base(eval)
		{
			SetParamCount(1);
			Name = "STR";
		}
		protected override Variant GetValue()
		{
			Variant aresult = new Variant();
			if (Params[0].VarType != VariantType.Null)
			{
				aresult = Params[0].ToString();
			}
			else
				aresult = "";
			return aresult;
		}
	}
    class IdenFloatToDateTime : IdenFunction
    {
        public IdenFloatToDateTime(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "FloatToDateTime";
            FModel = "function FloatToDateTime(value:double):DateTime";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
                if (!Params[0].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "FloatToDateTime");
                }
                aresult = System.Convert.ToDateTime(Params[0].AsDouble);
            return aresult;
        }
    }
    class IdenRound : IdenFunction
    {
        public IdenRound(Evaluator eval)
            : base(eval)
        {
            SetParamCount(2);
            Name = "Round";
            FModel = "function Round(num:double,r:double):double";
        }
        protected override Variant GetValue()
        {
            if (Params[0].IsNull)
                return Variant.VariantFromObject(DBNull.Value);
            Variant aresult = new Variant();
            decimal num = 0;
            decimal r = (decimal)0.01;
                if (!Params[0].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "Round");
                }
                num = Params[0].AsDecimal;
                if (!Params[1].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "Round");
                }
                r = Params[0].AsDecimal;
            aresult = DoubleUtil.RoundDecimalUp(num, r);
            return aresult;
        }
    }
    class IdenRoundToInteger : IdenFunction
    {
        public IdenRoundToInteger(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "RoundToInteger";
            FModel = "function RoundToInteger(num:double):integer";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
                if (!Params[0].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "RoundToInteger");
                }
            decimal num = Params[0];
            aresult = System.Convert.ToInt64(Math.Round(num));
            return aresult;
        }
    }
    class IdenAbs : IdenFunction
    {
        public IdenAbs(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "Abs";
            FModel = "function Abs(num:double):double";
        }
        protected override Variant GetValue()
        {
                if (!Params[0].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "Abs");
                }
            return Params[0].Abs();
        }
    }
    class IdenCompareValue : IdenFunction
    {
        public IdenCompareValue(Evaluator eval)
            : base(eval)
        {
            SetParamCount(3);
            Name = "CompareValue";
            FModel = "function CompareValue(p1,p2,epsilon:double):integer";
        }
        protected override Variant GetValue()
        {
                if (!Params[0].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "CompareValue");
                }
                if (!Params[1].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "CompareValue");
                }
                if (!Params[2].IsNumber())
                {
                    throw new NamedException(Translator.TranslateStr(438), "CompareValue");
                }
                Variant aresult = new Variant();
                aresult = DoubleUtil.CompareValue(Params[0], Params[1], Params[2]);
            return aresult;
        }
    }
    class IdenInt : IdenFunction
    {
        public IdenInt(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "Int";
            FModel = "function Int(num:double):integer";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (!Params[0].IsNumber())
            {
                throw new NamedException(Translator.TranslateStr(438), "Int");
            }
            decimal num = Params[0];
#if REPMAN_DOTNET1
			long num2 = System.Convert.ToInt64(num);
#else
#if REPMAN_COMPACT
            long num2 = System.Convert.ToInt64(num);
#else
			long num2 = System.Convert.ToInt64(Math.Truncate(num));
#endif
#endif
            aresult = num2;
            return aresult;
        }
    }
    class IdenVal : IdenFunction
    {
        public IdenVal(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "Val";
            FModel = "function Val(s:string):double";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (!Params[0].IsNumber())
            {
                aresult=Params[0];
                return aresult;
            }
            if (!Params[0].IsString())
            {
                throw new NamedException(Translator.TranslateStr(438), "Val");
            }
            string astring = Params[0];
            if (astring.Length < 1)
            {
                aresult = 0;
                return aresult;
            }
            aresult = System.Convert.ToDecimal(astring);
            return aresult;
        }
    }
    class IdenModul : IdenFunction
    {
        public IdenModul(Evaluator eval)
            : base(eval)
        {
            SetParamCount(2);
            Name = "Mod";
            FModel = "function Mod(d1,d2:integer):integer";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (!Params[0].IsInteger())
            {
                throw new NamedException(Translator.TranslateStr(438), "Mod");
            }
            if (!Params[1].IsInteger())
            {
                throw new NamedException(Translator.TranslateStr(438), "Mod");
            }
            long d1 = Params[0];
            long d2 = Params[1];
            long mod = d1 % d2;
            aresult = mod;
            return aresult;
        }
    }
    class IdenEvalText : IdenFunction
    {
        public IdenEvalText(Evaluator eval)
            : base(eval)
        {
            SetParamCount(1);
            Name = "Evaltext";
            FModel = "function EvalText(expression:string):Variant";
        }
        protected override Variant GetValue()
        {
            if (!Params[0].IsString())
            {
                throw new NamedException(Translator.TranslateStr(438), "EvalText");
            }
            return EvalObject.EvaluateText(Params[0]);
        }
    }
    class IdenNumToText : IdenFunction
    {
        public IdenNumToText(Evaluator eval)
            : base(eval)
        {
            SetParamCount(2);
            Name = "NumToText";
            FModel = "function NumToText(num:double;female:boolean):string";
        }
        protected override Variant GetValue()
        {
            Variant aresult = new Variant();
            if (!Params[0].IsNumber())
            {
                throw new NamedException(Translator.TranslateStr(438), "NumToText");
            }
            if (!Params[1].IsBoolean())
            {
                throw new NamedException(Translator.TranslateStr(438), "NumToText");
            }
            decimal num = Params[0];
            aresult = DoubleUtil.NumberToText(Params[0], Params[1],EvalObject.Language);
            return aresult;
        }
    }
}
