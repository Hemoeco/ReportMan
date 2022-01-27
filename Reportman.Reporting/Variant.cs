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
using System.Runtime.InteropServices;
#if CUSTOM_DATA
using MyData;
#else
using System.Data;
#endif
using System.IO;
using System.Text;
using System.Drawing;
using Reportman.Drawing;

namespace Reportman.Reporting
{
	/// <summary>
	/// Variant internal type, used to store current type value for a <see cref="Variant">Variant</see>
	/// </summary>
	public enum VariantType {
        /// <summary>Null value type</summary>
        Null,
        /// <summary>Internally stores a byte value</summary>
        Byte,
        /// <summary>Internally stores a boolean value</summary>
        Boolean,
        /// <summary>Internally stores a byte Char</summary>
        Char,
        /// <summary>Internally stores an integer value</summary>
        Integer,
        /// <summary>Internally stores a string value</summary>
        String,
        /// <summary>Internally stores a double value</summary>
        Double,
        /// <summary>Internally stores a decimal value</summary>
        Decimal,
        /// <summary>Internally stores a DateTime value</summary>
        DateTime,
        /// <summary>Internally stores a binary byte[] value</summary>
        Binary,
        /// <summary>Internally stores a long value</summary>
        Long
    };
	/// <summary>
	/// Type used in report parameters, for each parameter type, a diferent action will be performed
	/// to assign the value before executing the report, a parameter can be assigned to
	/// any number of database querys
	/// </summary>
	public enum ParamType
	{
		String, Integer, Double, Date, Time, DateTime,
		Currency, Bool, ExpreB, ExpreA, Subst, List, Multiple, SubstExpre,SubsExpreList,InitialValue,Unknown
	};

	/// <summary>
	/// Implementation of a Variant type with common arithmetic and 
	/// logic operations.
	/// Variant is a container, not a class reference, that is when you assign
	/// a variant to another variant, you are copying content, just like you
	/// work with base types like int, char, string.
	/// The Variant type is a useful feature to implement an expression
	/// <see cref="Evaluator">evaluator</see>.
	/// </summary>
	/// <remarks>
	/// C# is a strongly typed language, this feature minimize runtime 
	/// errors common when using implicit variable declaration (Visual Basic),
    /// or implicit type conversion (C or C++),
	/// however, at runtime, the requirements of an application can require 
	/// the feature of operating with unknown/variable data types, usually this data
    /// comes from a database, and this data types can be unknown at compile time.
	/// Expression evaluation involving variable database fields is an example.
	/// Use only Variant type if your aplication requires this abstraction
	/// else it's better to use singular types.
	/// </remarks>
	/// <example>
	/// <code>
	/// Variant a=12;
	/// Variant b=13;
	/// Variant c=a+b;
	/// Variant d="The result is:";
	/// d=d+c.ToString();
	/// </code>
	/// </example>
	public struct Variant:IConvertible,IComparable
	{
		VariantType FVarType;
		byte FByte;
        int FInteger;
        long FLong;
        bool FBoolean;
		char FChar;
		decimal FDecimal;
        double FDouble;
		DateTime FDateTime;
		MemoryStream FMemStream;
		string FString;
		/// <summary>
		/// Function to check if the current value is an integer value.
		/// </summary>
		/// <returns>
		/// Returns true if the current value is an ordinal, integer value.
		/// That is currently a Byte, Integer or Long type values.
		/// </returns>
		public bool IsInteger()
		{
			bool aresult = false;
			switch (FVarType)
			{
				case VariantType.Byte:
				case VariantType.Integer:
				case VariantType.Long:
					aresult = true;
					break;
			}
			return aresult;
		}
		/// <summary>
		/// Function to check if the current value is a boolean value.
		/// </summary>
		/// <returns>
		/// Returns true if the current value is a boolean value.
		/// </returns>
		public bool IsBoolean()
		{
			bool aresult = false;
			switch (FVarType)
			{
				case VariantType.Boolean:
					aresult = true;
					break;
			}
			return aresult;
		}
        /// <summary>
        /// Function to check if the current value is a Datetime value.
        /// </summary>
        /// <returns>
        /// Returns true if the current value is a Datetime value.
        /// </returns>
        public bool IsDateTime()
        {
            bool aresult = false;
            switch (FVarType)
            {
                case VariantType.DateTime:
                    aresult = true;
                    break;
            }
            return aresult;
        }
        /// <summary>
        /// Function to check if the current value is a string value.
        /// </summary>
        /// <returns>
        /// Returns true if the current value is a string value.
        /// </returns>
        public bool IsString()
		{
			bool aresult = false;
			switch (FVarType)
			{
				case VariantType.String:
					aresult = true;
					break;
			}
			return aresult;
		}
		/// <summary>
		/// Function to check if the current value is a number.
		/// </summary>
		/// <returns>
		/// Returns true if the current value is number.
		/// That is any integer, Double or Decimal value.
		/// </returns>
		public bool IsNumber()
		{
			bool aresult = false;
			switch (FVarType)
			{
				case VariantType.Byte:
				case VariantType.Integer:
				case VariantType.Long:
				case VariantType.Decimal:
				case VariantType.Double:
					aresult = true;
					break;
			}
			return aresult;
		}
		/// <summary>
		/// Function to get the type of the value as a string.
		/// </summary>
		/// <returns>
		/// Returns the string representation from the type of the value.
		/// Possible values are:
		/// <ul>
		/// <li>VariantType.Null</li>
		/// <li>VariantType.Boolean</li>
		/// <li>VariantType.Byte</li>
		/// <li>VariantType.Char</li>
		/// <li>VariantType.Integer</li>
		/// <li>VariantType.Long</li>
		/// <li>VariantType.Decimal</li>
		/// <li>VariantType.Double</li>
		/// <li>VariantType.DateTime</li>
		/// <li>VariantType.Binary</li>
		/// <li>VariantType.String</li>
		/// </ul>
		/// </returns>
		public string GetTypeString()
		{
			string ares = "";
			switch (FVarType)
			{

				case VariantType.Null:
					ares = "VariantType.Null";
					break;
				case VariantType.Boolean:
					ares = "VariantType.Boolean";
					break;
				case VariantType.Byte:
					ares = "VariantType.Byte";
					break;
				case VariantType.Char:
					ares = "VariantType.Char";
					break;
				case VariantType.Integer:
					ares = "VariantType.Integer";
					break;
				case VariantType.Long:
					ares = "VariantType.Long";
					break;
				case VariantType.Decimal:
					ares = "VariantType.Decimal";
					break;
				case VariantType.Double:
					ares = "VariantType.Double";
					break;
				case VariantType.DateTime:
					ares = "VariantType.DateTime";
					break;
				case VariantType.Binary:
					ares = "VariantType.Binary";
					break;
				case VariantType.String:
					ares = "VariantType.String";
					break;
			}
			return ares;

		}
		/// <summary>
		/// Function to check if the current value is null.
		/// </summary>
		/// <returns>
		/// Returns true if the value is null (VariantType.Null).
		/// </returns>
		public bool IsNull
		{
			get
			{
				return (FVarType == VariantType.Null);
			}
		}
		/// <summary>
		/// Function to check the variant type.
		/// </summary>
		/// <returns>
		/// Returns the VariantType for the current value.
		/// <see cref="VariantType">VariantType</see>
		/// </returns>
		public VariantType VarType
		{
			get
			{
				return FVarType;
			}
		}
		/// <summary>
		/// Assigns a bool value to a Variant
		/// </summary>
		public static implicit operator Variant(bool avalue)
		{
			Variant aresult = new Variant();
			aresult.FBoolean = avalue;
			aresult.FVarType = VariantType.Boolean;
			return aresult;
		}
        /// <summary>
		/// Assigns an int value to a Variant
		/// </summary>
		public static implicit operator Variant(int avalue)
		{
			Variant aresult = new Variant();
			aresult.FInteger = avalue;
			aresult.FVarType = VariantType.Integer;
			return aresult;
		}
        /// <summary>
		/// Assigns a long value to a Variant
		/// </summary>
		public static implicit operator Variant(long avalue)
		{
			Variant aresult = new Variant();
			aresult.FLong = avalue;
			aresult.FVarType = VariantType.Long;
			return aresult;
		}
        /// <summary>
		/// Assigns a decimal value to a Variant
		/// </summary>
		public static implicit operator Variant(decimal avalue)
		{
			Variant aresult = new Variant();
			aresult.FDecimal = avalue;
			aresult.FVarType = VariantType.Decimal;
			return aresult;
		}
		/// <summary>
		/// Assigns a double value to a Variant
		/// </summary>
		public static implicit operator Variant(double avalue)
		{
			Variant aresult = new Variant();
			aresult.FDouble = avalue;
			aresult.FVarType = VariantType.Double;
			return aresult;
		}
        /// <summary>
		/// Assigns a string value to a Variant
		/// </summary>
		public static implicit operator Variant(string avalue)
		{
			Variant aresult = new Variant();
      if (avalue == null)
        avalue = "";
      aresult.FString = avalue;
			aresult.FVarType = VariantType.String;
			return aresult;
		}
		/// <summary>
		/// Assigns a DateTime value to a Variant
		/// </summary>
		public static implicit operator Variant(DateTime avalue)
		{
			Variant aresult = new Variant();
			aresult.FDateTime = avalue;
			aresult.FVarType = VariantType.DateTime;
			return aresult;
		}
		/// <summary>
		/// Assigns a memory stream to a Variant
		/// </summary>
		public void SetStream(MemoryStream memstream)
		{
			FVarType = VariantType.Binary;
			FMemStream = new MemoryStream();
			memstream.Seek(0, System.IO.SeekOrigin.Begin);
			memstream.WriteTo(FMemStream);
		}
		/// <summary>
		/// Gets the stream on a binary type Variant
		/// </summary>
		public MemoryStream GetStream()
		{
			if (FVarType != VariantType.Binary)
				throw new UnNamedException("No Binary Variant type");
			MemoryStream memstream = new MemoryStream();
			FMemStream.Seek(0, System.IO.SeekOrigin.Begin);
			FMemStream.WriteTo(memstream);
			memstream.Seek(0, System.IO.SeekOrigin.Begin);
			return memstream;
		}
        /// <summary>
        /// Assigns a char value to a Variant
        /// </summary>
        public static implicit operator Variant(char avalue)
		{
			Variant aresult = new Variant();
			aresult.FChar = avalue;
			aresult.FVarType = VariantType.Char;
			return aresult;
		}
		/// <summary>
		/// Assigns a byte value to a Variant
		/// </summary>
		public static implicit operator Variant(byte avalue)
		{
			Variant aresult = new Variant();
			aresult.FByte = avalue;
			aresult.FVarType = VariantType.Byte;
			return aresult;
		}
        /// <summary>
		/// Implicit conversion from Variant to bool
		/// </summary>
		public static implicit operator bool(Variant avalue)
		{
			bool aresult = false;
			switch (avalue.VarType)
			{
				case VariantType.Null:
//					throw new UnNamedException(Translator.TranslateStr(438));
                   return false;
				case VariantType.Boolean:
					aresult = avalue.FBoolean;
					break;
				case VariantType.Byte:
					aresult = avalue.FByte != 0;
					break;
				case VariantType.Char:
					aresult = avalue.FChar != (Char)0;
					break;
				case VariantType.Integer:
					aresult = avalue.FInteger != 0;
					break;
				case VariantType.Long:
					aresult = avalue.FLong != 0;
					break;
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.DateTime:
				case VariantType.Binary:
				case VariantType.String:
					throw new UnNamedException(Translator.TranslateStr(438)+
						"Implicit bool from "+avalue.GetTypeString());
			}
			return aresult;
		}
		/// <summary>
		/// Implicit conversion from Variant to int
		/// </summary>
		public static implicit operator int(Variant avalue)
		{
			int aresult = 0;
			switch (avalue.VarType)
			{
				case VariantType.Byte:
					aresult = (int)avalue.FByte;
					break;
				case VariantType.Integer:
					aresult = avalue.FInteger;
					break;
				case VariantType.Long:
					aresult = (int)avalue.FLong;
					break;
				case VariantType.Null:
				case VariantType.Boolean:
				case VariantType.Char:
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.DateTime:
				case VariantType.String:
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
        /// <summary>
        /// Implicit conversion from Variant to long
        /// </summary>
        public static implicit operator long(Variant avalue)
        {
            long aresult = 0;
            switch (avalue.VarType)
            {
                case VariantType.Byte:
                    aresult = (long)avalue.FByte;
                    break;
                case VariantType.Integer:
                    aresult = avalue.FInteger;
                    break;
                case VariantType.Long:
                    aresult = avalue.FLong;
                    break;
                case VariantType.Null:
                case VariantType.Boolean:
                case VariantType.Char:
                case VariantType.Decimal:
                case VariantType.Double:
                case VariantType.DateTime:
                case VariantType.String:
                case VariantType.Binary:
                    throw new UnNamedException(Translator.TranslateStr(438));
            }
            return aresult;
        }
        /// <summary>
		/// Implicit conversion from Variant to byte
		/// </summary>
		public static implicit operator byte(Variant avalue)
		{
			byte aresult = 0;
			switch (avalue.VarType)
			{
				case VariantType.Byte:
					aresult = avalue.FByte;
					break;
				case VariantType.Char:
					aresult = (byte)avalue.FChar;
					break;
        case VariantType.Integer:
          aresult = (byte)avalue.FInteger;
          break;
        case VariantType.Long:
          aresult = (byte)avalue.FLong;
          break;
        case VariantType.Boolean:
				case VariantType.Null:
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.DateTime:
				case VariantType.String:
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Implicit conversion from Variant to char
		/// </summary>
		public static implicit operator char(Variant avalue)
		{
			char aresult = (char)0;
			switch (avalue.VarType)
			{
				case VariantType.Byte:
					aresult = (char)avalue.FByte;
					break;
				case VariantType.Char:
					aresult = avalue.FChar;
					break;
        case VariantType.Boolean:
				case VariantType.Null:
				case VariantType.DateTime:
				case VariantType.String:
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
    /// <summary>
    /// Implicit conversion from Variant to short
    /// </summary>
    public static implicit operator short(Variant avalue)
    {
      short aresult = (short)0;
      switch (avalue.VarType)
      {
        case VariantType.Byte:
          aresult = (short)avalue.FByte;
          break;
        case VariantType.Char:
          aresult = (short)avalue.FChar;
          break;
        case VariantType.Integer:
          aresult = (short)avalue.FInteger;
          break;
        case VariantType.Long:
          aresult = (short)avalue.FLong;
          break;
        case VariantType.Boolean:
        case VariantType.Null:
        case VariantType.DateTime:
        case VariantType.String:
        case VariantType.Decimal:
        case VariantType.Double:
        case VariantType.Binary:
          throw new UnNamedException(Translator.TranslateStr(438));
      }
      return aresult;
    }
    /// <summary>
		/// Implicit conversion from Variant to decimal
		/// </summary>
		public static implicit operator decimal(Variant avalue)
		{
			decimal aresult = 0.0M;
			switch (avalue.VarType)
			{
				case VariantType.Byte:
					aresult = (decimal)avalue.FByte;
					break;
				case VariantType.Char:
					aresult = (decimal)avalue.FChar;
					break;
				case VariantType.Decimal:
					aresult = avalue.FDecimal;
					break;
				case VariantType.Double:
					aresult = (decimal)avalue.FDouble;
					break;
				case VariantType.Integer:
					aresult = (decimal)avalue.FInteger;
					break;
				case VariantType.Long:
					aresult = (decimal)avalue.FLong;
					break;
				case VariantType.Null:
				case VariantType.Boolean:
				case VariantType.DateTime:
				case VariantType.Binary:
				case VariantType.String:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Implicit conversion from Variant to double
		/// </summary>
		public static implicit operator double(Variant avalue)
		{
			double aresult = 0.0;
			switch (avalue.VarType)
			{
				case VariantType.Byte:
					aresult = (double)avalue.FByte;
					break;
				case VariantType.Integer:
					aresult = (double)avalue.FInteger;
					break;
				case VariantType.Long:
					aresult = (double)avalue.FLong;
					break;
				case VariantType.Decimal:
					aresult = (double)avalue.FDecimal;
					break;
				case VariantType.Double:
					aresult = avalue.FDouble;
					break;
				case VariantType.Char:
				case VariantType.DateTime:
				case VariantType.Binary:
				case VariantType.Null:
				case VariantType.Boolean:
				case VariantType.String:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Implicit conversion from Variant to DateTime
		/// </summary>
		public static implicit operator DateTime(Variant avalue)
		{
			DateTime aresult = DateTime.Now;
			switch (avalue.VarType)
			{
				case VariantType.DateTime:
					aresult = avalue.FDateTime;
					break;
				case VariantType.String:
				case VariantType.Null:
				case VariantType.Boolean:
				case VariantType.Byte:
				case VariantType.Char:
				case VariantType.Integer:
				case VariantType.Long:
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Implicit conversion from Variant to string
		/// </summary>
		public static implicit operator string(Variant avalue)
		{
			string aresult = "";
			switch (avalue.VarType)
			{
                case VariantType.Char:
                    aresult = avalue.FChar.ToString();
                    break;
                case VariantType.Null:
				case VariantType.Boolean:
				case VariantType.Byte:
				case VariantType.Integer:
				case VariantType.Long:
				case VariantType.Decimal:
				case VariantType.Double:
				case VariantType.DateTime:
                case VariantType.String:
                    aresult = avalue.FString;
                    break;
                case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Returns the value of a Variant as a string
		/// </summary>
		override public string ToString()
		{
			string aresult = "";
			switch (VarType)
			{
				case VariantType.Null:
					aresult = "";
					break;
				case VariantType.Boolean:
					aresult = FBoolean.ToString();
					break;
				case VariantType.Byte:
					aresult = FByte.ToString();
					break;
				case VariantType.Char:
					aresult = FChar.ToString();
					break;
				case VariantType.Integer:
					aresult = FInteger.ToString();
					break;
				case VariantType.Long:
					aresult = FLong.ToString();
					break;
				case VariantType.Decimal:
					aresult = FDecimal.ToString();
					break;
				case VariantType.Double:
					aresult = FDouble.ToString();
					break;
				case VariantType.DateTime:
					aresult = FDateTime.ToString();
					break;
				case VariantType.String:
					aresult = FString;
					break;
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return aresult;
		}
		/// <summary>
		/// Returns a TypeCode based on the Variant type
		/// </summary>
		public TypeCode GetTypeCode()
		{
			TypeCode aresult = TypeCode.DBNull;
			switch (VarType)
			{
				case VariantType.Null:
					aresult = TypeCode.DBNull;
					break;
				case VariantType.Boolean:
					aresult = TypeCode.Boolean;
					break;
				case VariantType.Byte:
					aresult = TypeCode.Byte;
					break;
				case VariantType.Char:
					aresult = TypeCode.Char;
					break;
				case VariantType.Integer:
					aresult = TypeCode.Int32;
					break;
				case VariantType.Long:
					aresult = TypeCode.Int64;
					break;
				case VariantType.Decimal:
					aresult = TypeCode.Decimal;
					break;
				case VariantType.Double:
					aresult = TypeCode.Double;
					break;
				case VariantType.DateTime:
					aresult = TypeCode.DateTime;
					break;
				case VariantType.Binary:
					aresult = TypeCode.Object;
					break;
				case VariantType.String:
					aresult = TypeCode.String;
					break;
			}
			return aresult;
		}
		/// <summary>
		/// Returns a DbType based on the Variant type
		/// </summary>
		public DbType GetDbType()
		{
			DbType aresult = DbType.Object;
			switch (VarType)
			{
				case VariantType.Null:
					aresult = DbType.Object;
					break;
				case VariantType.Boolean:
					aresult = DbType.Boolean;
					break;
				case VariantType.Byte:
					aresult = DbType.Byte;
					break;
				case VariantType.Char:
					aresult = DbType.SByte;
					break;
				case VariantType.Integer:
					aresult = DbType.Int32;
					break;
				case VariantType.Long:
					aresult = DbType.Int64;
					break;
				case VariantType.Decimal:
					aresult = DbType.Decimal;
					break;
				case VariantType.Double:
					aresult = DbType.Double;
					break;
				case VariantType.DateTime:
					aresult = DbType.DateTime;
					break;
				case VariantType.Binary:
					aresult = DbType.Binary;
					break;
				case VariantType.String:
					aresult = DbType.String;
					break;
			}
			return aresult;
		}
		/// <summary>
		/// Compare to other Variant
		/// </summary>
		public int CompareTo(Variant obj)
		{
			int aresult = -2;
            if (obj.IsNull)
            {
                if (IsNull)
                {
                    aresult = 0;
                    return aresult;
                }
                else
                    return -1;
//                    throw new UnNamedException(Translator.TranslateStr(438) +
//                        " CompareTo " + GetTypeString() + "-" + obj.GetTypeString());
            }
			switch (FVarType)
			{
				case VariantType.Null:
                    aresult = 1;
                    break;
//					throw new UnNamedException(Translator.TranslateStr(438) +
//						" CompareTo " + GetTypeString() + "-" + obj.GetTypeString());
				case VariantType.Integer:
                    if (obj.IsBoolean())
                    {
                        bool partial = true;
                        if (obj == 0)
                        {
                            partial = false;
                        }
                        aresult = partial.CompareTo((bool)obj);
                    }
                    else
                    {
                        decimal aint = (decimal)obj;
                        decimal adec = FInteger;
                        aresult = adec.CompareTo(aint);
                    }
					break;
				case VariantType.Long:
					decimal aint2 = (decimal)obj;
					decimal adec2 = (decimal)FLong;
					aresult = adec2.CompareTo(aint2);
					break;
				case VariantType.Boolean:
                    // Boolean values can compare to integer
                    if (obj.IsInteger())
                    {
                            if (obj == 0)
                                obj = false;
                            else
                                obj = true;
                    }
					aresult = FBoolean.CompareTo((bool)obj);
					break;
				case VariantType.Char:
					aresult = FChar.CompareTo((char)obj);
					break;
				case VariantType.String:
					string astring = (string)obj;
					aresult = FString.CompareTo(astring);
					break;
				case VariantType.Byte:
					aresult = FByte.CompareTo((byte)obj);
					break;
				case VariantType.Double:
					aresult = FDouble.CompareTo((double)obj);
					break;
				case VariantType.Decimal:
					aresult = FDecimal.CompareTo((decimal)obj);
					break;
				case VariantType.DateTime:
					aresult = FDateTime.CompareTo((DateTime)obj);
					break;
				case VariantType.Binary:
					throw new UnNamedException(Translator.TranslateStr(438) +
					 " CompareTo " + GetTypeString() + "-" + obj.GetTypeString());
			}
			return aresult;
		}
		/// <summary>
		/// Binary or logic operator
		/// </summary>
		public static bool operator |(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			bool a1 = avalue1, a2 = avalue2;
			aresult.FVarType = VariantType.Boolean;
			aresult.FBoolean = (a1 || a2);
			return aresult;
		}
		/// <summary>
		/// Binary and logic operator
		/// </summary>
		public static Variant operator &(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			bool a1 = avalue1, a2 = avalue2;
			aresult.FVarType = VariantType.Boolean;
			aresult.FBoolean = (a1 && a2);
			return aresult;
		}
		/// <summary>
		/// Binary add arithmetic operator
		/// </summary>
		public static Variant operator +(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			switch (avalue1.FVarType)
			{
				case VariantType.Null:
                    switch (avalue2.FVarType)
                    {
                        case VariantType.Byte:
                        case VariantType.Integer:
                        case VariantType.Long:
                            avalue1.FVarType = VariantType.Long;
                            avalue1 = 0;
                            break;
                        case VariantType.String:
                            avalue1.FVarType = VariantType.String;
                            avalue1.FString = "";
                            break;
                        case VariantType.Decimal:
                            avalue1.FVarType = VariantType.Decimal;
                            avalue1.FDecimal = 0;
                            break;
                        case VariantType.Double:
                            avalue1.FVarType = VariantType.Double;
                            avalue1.FDouble = 0;
                            break;
                        case VariantType.Null:
                            return Variant.VariantFromObject(null);
                    }
					break;
				case VariantType.Byte:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = avalue1.FByte;
					break;
				case VariantType.Char:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = (int)avalue1.FChar;
					break;
			}
			switch (avalue2.FVarType)
			{
/*				case VariantType.Null:
                    if ((avalue1.FVarType == VariantType.String) ||
                     (avalue1.FVarType == VariantType.Null))
                    {
                        avalue2 = "";
                    }
                    else
                    {
                        avalue2.FVarType = VariantType.Decimal;
                        avalue2.FDecimal = 0;
//                        throw new UnNamedException(Translator.TranslateStr(438) +
//                            " Variant+ " + avalue1.GetTypeString() + "-" +
//                            avalue2.GetTypeString());
                    }
					break;*/
				case VariantType.Byte:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = avalue2.FByte;
					break;
				case VariantType.Char:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = (int)avalue2.FChar;
					break;
			}
			switch (avalue1.FVarType)
			{
				case VariantType.Integer:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = avalue1.FVarType;
                            aresult.FInteger = avalue1.FInteger;
                            break;
                        case VariantType.Integer:
							aresult.FVarType = VariantType.Integer;
							aresult.FInteger = avalue1.FInteger + avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = (long)avalue1.FInteger + avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FInteger + avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FInteger + avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(avalue1.FInteger);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;
				case VariantType.Long:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = avalue1.FVarType;
                            aresult.FLong = avalue1.FLong;
                            break;
                        case VariantType.Integer:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong + (long)avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong + avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FLong + avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = (decimal)avalue1.FLong + avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(avalue1.FLong);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;
				case VariantType.Double:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = avalue1.FVarType;
                            aresult.FDouble = avalue1.FDouble;
                            break;
                        case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble + avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble + (double)avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble + avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble + (double)avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(avalue1.FDouble);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;

				case VariantType.Decimal:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = avalue1.FVarType;
                            aresult.FDecimal = avalue1.FDecimal;
                            break;
						case VariantType.Integer:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal + avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal + (decimal)avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FDecimal + avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal + avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = 
								avalue2.FDateTime.AddDays((double)avalue1.FDecimal);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;

				case VariantType.DateTime:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = VariantType.DateTime;
                            aresult.FDateTime = avalue1.FDateTime;
                            break;
                        case VariantType.Integer:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								(double)avalue2.FInteger);
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								(double)avalue2.FLong);
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(avalue2.FDouble);
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								(double)avalue2.FDecimal);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;

				case VariantType.String:
					switch (avalue2.FVarType)
					{
                        case VariantType.Null:
                            // Sum + NULL = Sum
                            aresult.FVarType = avalue1.FVarType;
                            aresult.FString = avalue1.FString;
                            break;
                        case VariantType.String:
							aresult.FVarType = VariantType.String;
							aresult.FString = avalue1.FString + avalue2.FString;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438) + 
								" Variant+ " + avalue1.GetTypeString() + "-" + 
								avalue2.GetTypeString());
					}
					break;

				default:
					throw new UnNamedException(Translator.TranslateStr(438) + 
						" Variant+ " + avalue1.GetTypeString() + "-" + 
						avalue2.GetTypeString());
			}

			return aresult;
		}
		/// <summary>
		/// Binary substraction arithmetic operator
		/// </summary>
		public static Variant operator -(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			switch (avalue1.FVarType)
			{
				case VariantType.Null:
					//throw new UnNamedException(Translator.TranslateStr(438));
                    avalue1.FVarType = VariantType.Decimal;
                    avalue1.FDecimal = 0;
                    break;
				case VariantType.Byte:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = avalue1.FByte;
					break;
				case VariantType.Char:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = (int)avalue1.FChar;
					break;
			}
			switch (avalue2.FVarType)
			{
				case VariantType.Null:
					//throw new UnNamedException(Translator.TranslateStr(438));
                    avalue2.FVarType = VariantType.Decimal;
                    avalue2.FDecimal = 0;
                    break;
				case VariantType.Byte:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = avalue2.FByte;
					break;
				case VariantType.Char:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = (int)avalue2.FChar;
					break;
			}
			switch (avalue1.FVarType)
			{
				case VariantType.Integer:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Integer;
							aresult.FInteger = avalue1.FInteger - avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = (long)avalue1.FInteger - avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FInteger - avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FInteger - avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(-avalue1.FInteger);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Long:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong - avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong - avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FLong - avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = (decimal)avalue1.FLong - avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(-avalue1.FLong);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Double:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble - avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble - (double)avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble - avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble - (double)avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(-avalue1.FDouble);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				case VariantType.Decimal:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal - avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal - (decimal)avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FDecimal - avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal - avalue2.FDecimal;
							break;
						case VariantType.DateTime:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue2.FDateTime.AddDays(-(double)avalue1.FDecimal);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				case VariantType.DateTime:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								-(double)avalue2.FInteger);
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								-(double)avalue2.FLong);
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(-avalue2.FDouble);
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.DateTime;
							aresult.FDateTime = avalue1.FDateTime.AddDays(
								-(double)avalue2.FDecimal);
							break;
                        case VariantType.DateTime:
                            aresult.FVarType = VariantType.Double;
                            TimeSpan ndist = (avalue1.FDateTime - avalue2.FDateTime);
                            aresult.FDouble = ndist.TotalDays;
                            break;
                        default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				default:
					throw new UnNamedException(Translator.TranslateStr(438));
			}

			return aresult;
		}
		/// <summary>
		/// Unary sign arithmetic operator
		/// </summary>
		public static Variant operator -(Variant avalue1)
		{
			switch (avalue1.FVarType)
			{
				case VariantType.Null:
					//throw new UnNamedException(Translator.TranslateStr(438));
                    avalue1.FVarType = VariantType.Decimal;
                    avalue1.FDecimal = 0.0m;
                    break;
				case VariantType.Byte:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = avalue1.FByte;
					break;
				case VariantType.Char:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = (int)avalue1.FChar;
					break;
            }
			switch (avalue1.FVarType)
			{
				case VariantType.Integer:
					avalue1.FInteger = -avalue1.FInteger;
					break;
				case VariantType.Long:
					avalue1.FLong = -avalue1.FLong;
					break;
				case VariantType.Double:
					avalue1.FDouble = -avalue1.FDouble;
					break;

				case VariantType.Decimal:
					avalue1.FDecimal = -avalue1.FDecimal;
					break;
				default:
					throw new UnNamedException(Translator.TranslateStr(438));
			}
			return avalue1;
		}
		/// <summary>
		/// Binary multiplier arithmetic operator
		/// </summary>
		public static Variant operator *(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			switch (avalue1.FVarType)
			{
				case VariantType.Null:
				//	throw new UnNamedException(Translator.TranslateStr(438));
                    return aresult;
                case VariantType.Byte:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = avalue1.FByte;
					break;
				case VariantType.Char:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = (int)avalue1.FChar;
					break;
			}
			switch (avalue2.FVarType)
			{
				case VariantType.Null:
				//	throw new UnNamedException(Translator.TranslateStr(438));
                    return aresult;
                case VariantType.Byte:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = avalue2.FByte;
					break;
				case VariantType.Char:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = (int)avalue2.FChar;
					break;
			}
			switch (avalue1.FVarType)
			{
				case VariantType.Integer:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Integer;
							aresult.FInteger = avalue1.FInteger * avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = System.Convert.ToInt64(avalue1.FInteger) * avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = System.Convert.ToDouble(avalue1.FInteger) * avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = System.Convert.ToDecimal(avalue1.FInteger) * avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Long:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong * System.Convert.ToInt64(avalue2.FInteger);
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Long;
							aresult.FLong = avalue1.FLong * avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = System.Convert.ToDouble(avalue1.FLong) * avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = System.Convert.ToDecimal(avalue1.FLong) * avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Double:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble * System.Convert.ToDouble(avalue2.FInteger);
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble * System.Convert.ToDouble(avalue2.FLong);
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble * avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble * System.Convert.ToDouble(avalue2.FDecimal);
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				case VariantType.Decimal:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal * System.Convert.ToDecimal(avalue2.FInteger);
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal * System.Convert.ToDecimal(avalue2.FLong);
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = System.Convert.ToDouble(avalue1.FDecimal) * avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal * avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				default:
					throw new UnNamedException(Translator.TranslateStr(438));
			}

			return aresult;
		}
		/// <summary>
		/// Binary division arithmetic operator
		/// </summary>
		public static Variant operator /(Variant avalue1, Variant avalue2)
		{
			Variant aresult = new Variant();
			switch (avalue1.FVarType)
			{
				case VariantType.Null:
					// throw new UnNamedException(Translator.TranslateStr(438));
                    return aresult;
				case VariantType.Byte:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = avalue1.FByte;
					break;
				case VariantType.Char:
					avalue1.FVarType = VariantType.Integer;
					avalue1.FInteger = (int)avalue1.FChar;
					break;
			}
			switch (avalue2.FVarType)
			{
				case VariantType.Null:
				//	throw new UnNamedException(Translator.TranslateStr(438));
                    return aresult;
                case VariantType.Byte:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = avalue2.FByte;
					break;
				case VariantType.Char:
					avalue2.FVarType = VariantType.Integer;
					avalue2.FInteger = (int)avalue2.FChar;
					break;
			}
			switch (avalue1.FVarType)
			{
				case VariantType.Integer:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FInteger / avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FInteger / avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FInteger / avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = (decimal)avalue1.FInteger / avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Long:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FLong / avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FLong / avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FLong / avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = (decimal)avalue1.FLong / avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;
				case VariantType.Double:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble / avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble / avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble / avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = avalue1.FDouble / (double)avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				case VariantType.Decimal:
					switch (avalue2.FVarType)
					{
						case VariantType.Integer:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal / avalue2.FInteger;
							break;
						case VariantType.Long:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal / avalue2.FLong;
							break;
						case VariantType.Double:
							aresult.FVarType = VariantType.Double;
							aresult.FDouble = (double)avalue1.FDecimal / avalue2.FDouble;
							break;
						case VariantType.Decimal:
							aresult.FVarType = VariantType.Decimal;
							aresult.FDecimal = avalue1.FDecimal / avalue2.FDecimal;
							break;
						default:
							throw new UnNamedException(Translator.TranslateStr(438));
					}
					break;

				default:
					throw new UnNamedException(Translator.TranslateStr(438));
			}

			return aresult;
		}
		/// <summary>
		/// Compare two variants
		/// </summary>
		/// <remarks>
		/// Two Variants containing null value are not equal
		/// </remarks>
		public static bool operator ==(Variant avalue1, Variant avalue2)
		{
            if ((avalue1.FVarType == VariantType.Null) ||
                (avalue2.FVarType == VariantType.Null))
            {
                if ((avalue1.FVarType == VariantType.Null) &&
                    (avalue2.FVarType == VariantType.Null))
                    return true;
                else
                    return false;
            }
			return (avalue1.CompareTo(avalue2) == 0);
		}
		/// <summary>
		/// Compare a Variant with a object
		/// </summary>
		public override bool Equals(object obj)
		{
			// TODO: Check the function, implicit conversion maybe needed
			if (obj is Variant)
				return (this == (Variant)obj);
			else
				return base.Equals(obj);
		}
		/// <summary>
		/// Necessary to override == operator
		/// </summary>
		public override int GetHashCode()
		{
/*			DbType aresult = DbType.Object;
			switch (VarType)
			{
				case VariantType.Null:
					aresult = DbType.Object;
					break;
				case VariantType.Boolean:
					aresult = DbType.Boolean;
					break;
				case VariantType.Byte:
					aresult = DbType.Byte;
					break;
				case VariantType.Char:
					aresult = DbType.SByte;
					break;
				case VariantType.Integer:
					aresult = DbType.Int32;
					break;
				case VariantType.Long:
					aresult = DbType.Int64;
					break;
				case VariantType.Decimal:
					aresult = DbType.Decimal;
					break;
				case VariantType.Double:
					aresult = DbType.Double;
					break;
				case VariantType.DateTime:
					aresult = DbType.DateTime;
					break;
				case VariantType.Binary:
					aresult = DbType.Binary;
					break;
				case VariantType.String:
					aresult = DbType.String;
					break;
			}
			return aresult;
*/			return base.GetHashCode();
		}

		/// <summary>
		/// Compare two Variants
		/// </summary>
		public static bool operator !=(Variant avalue1, Variant avalue2)
		{
			return (avalue1.CompareTo(avalue2) != 0);
		}
		/// <summary>
		/// Compare two Variants
		/// </summary>
		public static bool operator <(Variant avalue1, Variant avalue2)
		{
			return (avalue1.CompareTo(avalue2) < 0);
		}
		/// <summary>
		/// Compare two Variants
		/// </summary>
		public static bool operator >(Variant avalue1, Variant avalue2)
		{
			return (avalue1.CompareTo(avalue2) > 0);
		}
		/// <summary>
		/// Compare two Variants
		/// </summary>
		public static bool operator <=(Variant avalue1, Variant avalue2)
		{
			return (avalue1.CompareTo(avalue2) <= 0);
		}
		/// <summary>
		/// Compare two Variants
		/// </summary>
		public static bool operator >=(Variant avalue1, Variant avalue2)
		{
			return (avalue1.CompareTo(avalue2) >= 0);
		}
		/// <summary>
		/// Procedure to assign a Variant from any object type
		/// </summary>
		/// <remarks>
		/// If the object is a null reference, the result will be a Variant
		/// with type Null
		/// </remarks>
		public void AssignFromObject(object obj)
		{
			if (obj == null)
			{
				FVarType = VariantType.Null;
				return;
			}
			string types = obj.GetType().ToString();
			switch (types)
			{
                case "System.Drawing.Bitmap":
#if NODRAWING
                    throw new Exception("Drawing.ToBitmap() not supported in assign object");
#else
                    FMemStream = new MemoryStream();
                    Bitmap nbitmap = ((System.Drawing.Bitmap)obj);
                    nbitmap.Save(FMemStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    FVarType = VariantType.Binary;
                    break;

#endif
                case "System.Drawing.Icon":
#if NODRAWING
                    throw new Exception("Icon.ToBitmap() not supported in COmpact framework");
#else
                    FMemStream = new MemoryStream();
                    using (System.Drawing.Bitmap rbitmap = ((System.Drawing.Icon)obj).ToBitmap())
                    {
                        rbitmap.Save(FMemStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        FVarType = VariantType.Binary;
                    }
                    break;
#endif

                case "System.String":
					FString = (String)obj;
					FVarType = VariantType.String;
					break;
				case "System.Byte[]":
					byte[] FBytes = (byte[])obj;
					FMemStream = new MemoryStream(FBytes.Length);
					FMemStream.Write(FBytes, 0, FBytes.Length);
					FVarType = VariantType.Binary;
					break;
                case "System.IO.MemoryStream":
					byte[] FBytes2 = ((MemoryStream)obj).ToArray();
					FMemStream = new MemoryStream(FBytes2.Length);
					FMemStream.Write(FBytes2, 0, FBytes2.Length);
					FVarType = VariantType.Binary;
					break;
                case "System.Int32":
					FInteger = (int)obj;
					FVarType = VariantType.Integer;
					break;
				case "System.Int64":
					FLong = (long)obj;
					FVarType = VariantType.Long;
					break;
				case "System.Int16":
					FInteger = (int)(short)obj;
					FVarType = VariantType.Integer;
					break;
				case "System.Decimal":
					FDecimal = (Decimal)obj;
					FVarType = VariantType.Decimal;
					break;

				case "System.Bool":
					FBoolean = (bool)obj;
					FVarType = VariantType.Boolean;
					break;
                case "System.Boolean":
                    FBoolean = (bool)obj;
                    FVarType = VariantType.Boolean;
                    break;
                case "System.DateTime":
					FDateTime = (DateTime)obj;
					FVarType = VariantType.DateTime;
					break;
				case "System.Byte":
					FByte = (Byte)obj;
					FVarType = VariantType.Byte;
					break;
                case "System.Single":
                    FDouble = ((Single)obj);
                    FVarType = VariantType.Double;
                    break;
                case "System.Double":
					FDouble = (double)obj;
					FVarType = VariantType.Double;
					break;
				case "System.DBNull":
					FVarType = VariantType.Null;
					break;
                case "Reportman.Reporting.Variant":
                    this=(Variant)obj;
                    break;
                default:
					// TODO: Probably better to lauch an exception once the 
					// testing is done
					FVarType = VariantType.Null;
					//System.Console.WriteLine(Translator.TranslateStr(1417) + ": " + 
					//	obj.GetType().ToString());
					break;
			}
		}
		/// <summary>
		/// Get the Variant as an object
		/// </summary>
		/// <remarks>
		/// For Null value the result is DBNull.Value.
		/// For binary data type, the result is a byte[] type
		/// </remarks>
		public object AsObject()
		{
			object aresult = null;
			switch (FVarType)
			{
				case VariantType.Null:
					aresult = DBNull.Value;
					//				throw new UnNamedException(Translator.TranslateStr(438));
					break;
				case VariantType.Boolean:
					aresult = FBoolean;
					break;
				case VariantType.Byte:
					aresult = FByte;
					break;
				case VariantType.Char:
					aresult = FChar;
					break;
				case VariantType.Integer:
					aresult = FInteger;
					break;
				case VariantType.Long:
					aresult = FLong;
					break;
				case VariantType.Decimal:
					aresult = FDecimal;
					break;
				case VariantType.Double:
					aresult = FDouble;
					break;
				case VariantType.DateTime:
					aresult = FDateTime;
					break;
				case VariantType.String:
					aresult = FString;
					break;
				// TODO: Return binary type as byte[]
			}
			return aresult;
		}
		/// <summary>
		/// This function creates a Variant from a object
		/// </summary>
		public static Variant VariantFromObject(object obj)
		{
			Variant avar = new Variant();
			avar.AssignFromObject(obj);
			return avar;
		}
		/// <summary>
		/// Property to get the variant value as a string
		/// </summary>
		public string AsString
		{
			get
			{
				string nvalue = this.ToString();
				return nvalue;
			}
		}
		/// <summary>
		/// Property to get the variant value as a double
		/// </summary>
		public double AsDouble
		{
			get
			{
				double nvalue = this;
				return nvalue;
			}
		}
        /// <summary>
        /// Property to get the variant value as a decimal
        /// </summary>
        public decimal AsDecimal
        {
            get
            {
                decimal nvalue = this;
                return nvalue;
            }
        }
        /// <summary>
		/// Property to get the variant value as an int
		/// </summary>
		public int AsInteger
		{
			get
			{
				int nvalue = this;
				return nvalue;
			}
		}
        /// <summary>
        /// Property to get the variant value as a Datetime
        /// </summary>
        public DateTime AsDateTime
        {
            get
            {
                DateTime nvalue = this;
                return nvalue;
            }
        }
        /// <summary>
		/// Property to get the variant value as a long
		/// </summary>
		public long AsLong
		{
			get
			{
				long nvalue = this;
				return nvalue;
			}
		}
		private string DefaultDateTimeFormat(ParamType paramtype)
		{
			if (paramtype == ParamType.Date)
				return System.Globalization.CultureInfo.CurrentCulture.
					DateTimeFormat.ShortDatePattern;
			else
				if (paramtype == ParamType.Time)
					return System.Globalization.CultureInfo.CurrentCulture.
						DateTimeFormat.ShortTimePattern;
				else
					return System.Globalization.CultureInfo.CurrentCulture.
						DateTimeFormat.FullDateTimePattern;
		}
		/// <summary>
		/// Formats a Variant, with a display format
		/// </summary>
		/// <param name="displayformat">
		/// The formatting string, depending on current value
		/// </param>
		/// <param cref="paramtype">
		/// The variant will be formatted with a logic related to parameter type
		/// specified, unknown can be specified
		/// </param>
		/// <param cref="printnulls">
		/// For numeric values the 0 will return an empty string
		/// </param>
		public string ToString(string displayformat, ParamType paramtype, 
			bool printnulls)
		{
			VariantType atype;
			Variant Value = this;

			if (paramtype != ParamType.Unknown)
			{
				if (Value.IsNull)
				{
					switch (paramtype)
					{
						case ParamType.String:
						case ParamType.ExpreA:
						case ParamType.ExpreB:
						case ParamType.Multiple:
						case ParamType.Subst:
							Value = "";
							break;
						case ParamType.Integer:
						case ParamType.Double:
						case ParamType.Currency:
							Value = 0;
							break;
						case ParamType.Bool:
							Value = false;
							break;
					}
				}
			}
			else
			{
				if (Value.IsNull)
				{
					return "";
				}
			}
			atype = Value.VarType;
			if (!Value.IsNull)
			{
				if (displayformat.Length == 0)
				{
					if (printnulls)
					{
						if ((paramtype == ParamType.Date) ||
							(paramtype == ParamType.Time) ||
							(paramtype == ParamType.DateTime))
						{
							displayformat = DefaultDateTimeFormat(paramtype);
							if ((atype == VariantType.Byte) ||
								(atype == VariantType.Integer) ||
								(atype == VariantType.Long) ||
								(atype == VariantType.DateTime) ||
								(atype == VariantType.Decimal) ||
								(atype == VariantType.Double))
							{
								DateTime adate = Value;
								return adate.ToString(displayformat);
							}
							else
								return Value.ToString();
						}
						else
							return Value.ToString();
					}
				}
			}
			string aresult = "";
			switch (atype)
			{
				case VariantType.Null:
					break;
				case VariantType.Integer:
				case VariantType.Long:
				case VariantType.Byte:
				case VariantType.Decimal:
				case VariantType.Double:
					if ((Value == 0) && (!printnulls))
						aresult = "";
					else
					{
						if (displayformat.Length == 0)
						{
							aresult = Value.ToString();
						}
						else
						{
							if ((paramtype == ParamType.Date) ||
								(paramtype == ParamType.Time) ||
								(paramtype == ParamType.DateTime))
							{
								if (displayformat.Length == 0)
									displayformat = DefaultDateTimeFormat(paramtype);
								DateTime adate = Value;
								aresult = adate.ToString(displayformat);
							}
							else
							{
								if (displayformat[0] == '*')
									aresult = DoubleUtil.FormatCurrAdv(displayformat.
										Substring(1, displayformat.Length - 1), Value);
								else
								{
									double avalue = Value;
									aresult = avalue.ToString(displayformat);
								}
							}
						}
					}
					break;
				case VariantType.DateTime:
					if (displayformat.Length == 0)
						displayformat = DefaultDateTimeFormat(paramtype);
					DateTime dvalue = Value;
					aresult = dvalue.ToString(displayformat);
					break;
				case VariantType.String:
					string astring = Value;
					if (displayformat.Length > 0)
						aresult = string.Format(displayformat, astring);
					else
						aresult = astring;
					break;
				case VariantType.Boolean:
					bool bvalue = Value;
					aresult = string.Format(displayformat, bvalue.ToString());
					break;
				default:
					aresult = "";
					break;
			}
			return aresult;
		}
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public bool ToBoolean(System.IFormatProvider provid)
        {
            bool avalue=this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public double ToDouble(System.IFormatProvider provid)
        {
            double avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public byte ToByte(System.IFormatProvider provid)
        {
            Byte avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public sbyte ToSByte(System.IFormatProvider provid)
        {
            SByte avalue = (SByte)this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public char ToChar(System.IFormatProvider provid)
        {
            char avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public Int16 ToInt16(System.IFormatProvider provid)
        {
            Int16 avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public UInt16 ToUInt16(System.IFormatProvider provid)
        {
            int a = this;
            UInt16 avalue = System.Convert.ToUInt16(a);
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public Int32 ToInt32(System.IFormatProvider provid)
        {
            Int32 avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public UInt32 ToUInt32(System.IFormatProvider provid)
        {
            int a = this;
            UInt32 avalue = System.Convert.ToUInt32(a);
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public Int64 ToInt64(System.IFormatProvider provid)
        {
            Int64 avalue = this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public UInt64 ToUInt64(System.IFormatProvider provid)
        {
            Int64 a = this;
            UInt64 avalue = System.Convert.ToUInt64(a);
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public Single ToSingle(System.IFormatProvider provid)
        {
            Single avalue = (Single)this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public string ToString(System.IFormatProvider provid)
        {
            string avalue = (string)this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public Decimal ToDecimal(System.IFormatProvider provid)
        {
            Decimal avalue = (Decimal)this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public DateTime ToDateTime(System.IFormatProvider provid)
        {
            DateTime avalue = (DateTime)this;
            return avalue;
        }
        /// <summary>
        /// Type transformation using a IFormatProvider
        /// </summary>
        public object ToType(System.Type ntype, System.IFormatProvider provid)
        {
            DateTime avalue = this;
            return avalue;
        }
        /// <summary>
        /// Compare a Variant to an object
        /// </summary>
        /// <remarks>
        /// Returns 0 if both are equal, 1 if current instance is greater than object, else returns -1
        /// </remarks>
        public int CompareTo(object obj)
        {
            Variant v = Variant.VariantFromObject(obj);
            if (v == this)
                return 0;
            if (this > v)
                return 1;
            else
                return -1;
        }
        /// <summary>
        /// Returns the variant as a positive number, absolute value
        /// </summary>
        public Variant Abs()
        {
            Variant aresult = new Variant();
            if (!IsNumber())
            {
                throw new UnNamedException(Translator.TranslateStr(438));
            }
            switch (FVarType)
            {
                case VariantType.Decimal:
                    aresult = Math.Abs(FDecimal);
                    break;
                case VariantType.Double:
                    aresult = Math.Abs(FDouble);
                    break;
                case VariantType.Integer:
                    aresult = Math.Abs(FInteger);
                    break;
                case VariantType.Long:
                    aresult = Math.Abs(FLong);
                    break;
                default:
                    aresult = Math.Abs((double)this);
                    break;
            }
            return aresult;
        }
    }
    // Array of variants
    /// <summary>
    /// Collection of Variant values
    /// </summary>
    public class Variants : System.Collections.Generic.List<Variant>
    {
    }

}

