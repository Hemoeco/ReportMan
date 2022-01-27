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
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.IO;
#if REPMAN_DESIGN
using System.Drawing;
#else
#endif
using Reportman.Drawing;

namespace Reportman.Reporting
{
    /// <summary>
    /// Evaluator is an expression evaluator, it includes variable assignment and support
    /// for dataset fields, function calls are also supported
    /// </summary>
#if REPMAN_DESIGN
	[ToolboxBitmapAttribute(typeof(Evaluator), "evaluator.ico")]
#else
#endif
    public class Evaluator : System.ComponentModel.Component
	{
		/// <summary>
		/// Required variable by Designer
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string FExpression;
		private bool FChecking;
		private Variant FResult;
		private bool FEvaluating;
		private EvalIdentifiers FIdentifierList;
        public EvalIdentifiers Identifiers
        {
            get
            {
                return FIdentifierList;
            }
        }

		private EvalParser FParser;
		private DatasetAlias FAliasList;

        /// <summary>
        /// Language property is used in funtions returning localized strings
        /// </summary>
        public int Language;
        /// <summary>
        /// AliasList is used to provide access to dataset fields to the Evaluator
        /// </summary>
        public DatasetAlias AliasList
		{
			get
			{
				return FAliasList;
			}
			set
			{
				FAliasList = value;
			}
		}
        /// <summary>
        /// Provide syntax checking utility
        /// </summary>
        public void CheckSyntax(string atext)
		{
			FChecking = true;
			try
			{
				EvaluateText(atext);
			}
			finally
			{
				FChecking = false;
			}
		}
        /// <summary>
        /// Add a variable to the evaluator
        /// </summary>
        /// <param name="name1">Variable name</param>
        /// <param name="iden">Identifier object, usually a IdenVariable typed object</param>
        public void AddVariable(string name1, EvalIdentifier iden)
		{
			iden.Name = name1.ToUpper();
			FIdentifierList.Add("M." + iden.Name, iden);
		}
        /// <summary>
        /// Addd any identifier object to the evaluator
        /// </summary>
        /// <param name="name1">Identifier name</param>
        /// <param name="iden">Identifier object</param>
		public void AddIden(string name1, EvalIdentifier iden)
		{
			iden.Name = name1.ToUpper();
			FIdentifierList.Add(iden.Name, iden);
		}
        /// <summary>
        /// Create a new variable with an initial value
        /// </summary>
        /// <param name="name1">Variable name</param>
        /// <param name="valueIni">Initial value</param>
        /// <returns></returns>
		public IdenVariable NewVariable(string name1, Variant valueIni)
		{
			IdenVariable iden;
			if (SearchIdentifier(name1) != null)
				throw new NamedException("Variable exists:" + name1,name1);
			iden = new IdenVariable(this);
			AddVariable(name1, iden);
			iden.Value = valueIni;
			return iden;
		}
        /// <summary>
        /// Looks for an identifier, returns null if not found
        /// </summary>
        /// <param name="idenname">Identifier name</param>
        /// <returns>Identifier found or null</returns>
		public EvalIdentifier SearchIdentifier(string idenname)
		{
			EvalIdentifier iden = null;
			idenname = idenname.ToUpper();
			// Looks for field in a dataset
			string sdataset, sfield;
			sdataset = "";
			sfield = idenname;
			int index = idenname.IndexOf(".");
			if (index < 0)
			{
				// If is not a funcion, can be a variable
                index=FIdentifierList.IndexOfKey(idenname);
                if (index>=0)
				     return FIdentifierList[idenname];
				sfield = idenname;
			}
			else
			{
				sdataset = idenname.Substring(0, index);
				sfield = idenname.Substring(index + 1, idenname.Length - index - 1);
				if (sdataset == "M")
				{
                    string fieldm2 = "M." + sfield;
                    if (FIdentifierList.IndexOfKey(fieldm2) >= 0)
                        return FIdentifierList["M." + sfield];
				}
			}
			if (sdataset == "")
			{
                string fieldm="M." + sfield;
                if (FIdentifierList.IndexOfKey(fieldm)>=0)
				    iden = FIdentifierList[fieldm];
			}
			if (FAliasList != null)
			{
				bool duplicated = false;
				EvalIdentifier idenfield = FAliasList.FindField(sfield, sdataset, ref duplicated);
				if (duplicated)
					throw new NamedException("Especify variable or file:" + sfield,sfield);
				if (idenfield != null)
				{
					if (iden != null)
						throw new NamedException("Especify variable or file:" + sfield,sfield);
					else
						iden = idenfield;
				}
			}
			return iden;
		}
		private struct ParadoxGraphicHeader
		{
			public short Count;
			public short HType;
			public int Size;
		}
		/// <summary>
		/// Obtains a memory stream from an expression resulting in a file name path or
		/// a database field
		/// </summary>
		/// <param name="expre">Expression to evaluate</param>
		/// <returns>A Memory stream or null if the result is not a stream</returns>
		public MemoryStream GetStreamFromExpression(string expre)
		{
			EvalIdentifier iden = SearchIdentifier(expre);
			MemoryStream memstream = null;
			if (iden != null)
			{
				if (iden is IdenField)
				{
					Variant avalue = iden.Value;
					if (avalue.VarType == VariantType.Binary)
					{
						memstream = avalue.GetStream();
						memstream.Seek(0, System.IO.SeekOrigin.Begin);
						// Search for Paradox graphic header

						if (memstream.Length > 8)
						{
							byte[] buf = new byte[8];
							memstream.Read(buf, 0, 8);
							ParadoxGraphicHeader hd = new ParadoxGraphicHeader();
							hd.Count = StreamUtil.ByteArrayToShort(buf, 0, 2);
							hd.HType = StreamUtil.ByteArrayToShort(buf, 2, 2);
							hd.Size = StreamUtil.ByteArrayToInt(buf, 4, 4);
							if ((hd.Count == 1) && (hd.HType == 0x0100))
							{
								MemoryStream astream = new MemoryStream();
								buf = new byte[hd.Size];
								int readed = memstream.Read(buf, 0, hd.Size);
								astream.Write(buf, 0, readed);
								memstream = astream;
							}
							memstream.Seek(0, System.IO.SeekOrigin.Begin);
						}
					}
					else
						if (avalue.VarType != VariantType.Null)
						iden = null;
				}
				else
					iden = null;
			}
			if (iden == null)
			{
				Variant avalue = EvaluateText(expre);
				if (avalue.VarType == VariantType.Binary)
				{
					memstream = avalue.GetStream();
				}
				else
				{
					if (avalue.VarType != VariantType.String)
						throw new NamedException("Field or file not found:" + expre, expre);
					System.IO.FileInfo finfo = new System.IO.FileInfo(avalue.AsString);
					if (!finfo.Exists)
						throw new NamedException("File not found:" + avalue.AsString, avalue.AsString);
					memstream = new MemoryStream();
					FileStream afile = new FileStream(avalue.AsString,
					 System.IO.FileMode.Open, System.IO.FileAccess.Read);
					try
					{
						byte[] buf = new byte[120000];
						int readed = afile.Read(buf, 0, 120000);
						while (readed > 0)
						{
							memstream.Write(buf, 0, readed);
							readed = afile.Read(buf, 0, 120000);
						}
					}
					finally
					{
						afile.Close();
					}
				}
			}
			return memstream;
		}
		/// <summary>
		/// This property contains the last result of the evaluation of an expression
		/// </summary>
		public Variant Result
		{
			get
			{
				return FResult;
			}
		}
		private void operand(ref Variant FValue)
		{
			EvalIdentifier iden = null;
			switch (FParser.Token)
			{
				case TokenType.Symbol:
					iden = SearchIdentifier(FParser.TokenString());
					if (iden == null)
						throw new EvalException(Translator.TranslateStr(440) + FParser.TokenString(),
							FParser.SourceLine, FParser.SourcePos, "");
					FValue = iden.Value;
					FParser.NextToken();
					break;
				case TokenType.String:
					FValue = FParser.TokenString();
					FParser.NextToken();
					break;
				case TokenType.Integer:
					FValue = FParser.AsInteger;
					FParser.NextToken();
					break;
				case TokenType.Double:
					FValue = FParser.AsDouble;
					FParser.NextToken();
					break;
				case TokenType.Decimal:
					FValue = FParser.AsDecimal;
					FParser.NextToken();
					break;
				default:
					throw new EvalException(Translator.TranslateStr(449),
					 FParser.SourceLine, FParser.SourcePos, "");

			}
		}
		private void parentesis(ref Variant FValue)
		{
			if (FParser.Token == TokenType.Operator)
			{
				string aoperator = "";
				aoperator = FParser.TokenString();
				if (aoperator == "(")
				{
					FParser.NextToken();
					separator(ref FValue);
					aoperator = "";
					if (FParser.Token == TokenType.Operator)
						aoperator = FParser.TokenString();
					if (aoperator != ")")
						throw new EvalException(Translator.TranslateStr(441),
							FParser.SourceLine, FParser.SourcePos, "");
					else
						FParser.NextToken();
				}
				else
					operand(ref FValue);
			}
			else
				operand(ref FValue);
		}
		private void ExecuteIIF(ref Variant FValue)
		{
			Variant auxiliar = new Variant();
			string aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString();
			// Must be followed by a parentesis
			if (aoperator != "(")
				throw new EvalException(Translator.TranslateStr(441),
					FParser.SourceLine, FParser.SourcePos, "");
			// Decision term
			FParser.NextToken();
			separator(ref FValue);
			// Null=false
			if (FValue.IsNull)
				FValue = false;
			// Not boolean=error
			if (FValue.VarType != VariantType.Boolean && (!FChecking))
				throw new EvalException(Translator.TranslateStr(438),
					FParser.SourceLine, FParser.SourcePos, "");
			// Next tokens
			aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString();
			// Must be a ,
			if (aoperator != ",")
				throw new EvalException(Translator.TranslateStr(449),
					 FParser.SourceLine, FParser.SourcePos, "");
			FParser.NextToken();
			// Syntax check
			if (FChecking)
			{
				separator(ref FValue);
				aoperator = "";
				if (FParser.Token == TokenType.Operator)
					aoperator = FParser.TokenString();
				// Must be a ,
				if (aoperator != ",")
					throw new EvalException(Translator.TranslateStr(449),
						 FParser.SourceLine, FParser.SourcePos, "");
				FParser.NextToken();
				separator(ref auxiliar);

			}
			else
			{
				bool oldFChecking;
				if (FValue)
				{
					separator(ref FValue);

					aoperator = "";
					if (FParser.Token == TokenType.Operator)
						aoperator = FParser.TokenString();
					// Must be a ,
					if (aoperator != ",")
						throw new EvalException(Translator.TranslateStr(449),
							 FParser.SourceLine, FParser.SourcePos, "");
					FParser.NextToken();
					oldFChecking = FChecking;
					FChecking = true;
					separator(ref auxiliar);
					FChecking = oldFChecking;
				}
				else
				{
					oldFChecking = FChecking;
					FChecking = true;
					separator(ref auxiliar);

					aoperator = "";
					if (FParser.Token == TokenType.Operator)
						aoperator = FParser.TokenString();
					// Must be a ,
					if (aoperator != ",")
						throw new EvalException(Translator.TranslateStr(449),
							 FParser.SourceLine, FParser.SourcePos, "");
					FParser.NextToken();

					FChecking = oldFChecking;
					separator(ref FValue);
				}
			}

			aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString();
			// Must be followed by a parentesis
			if (aoperator != ")")
				throw new EvalException(Translator.TranslateStr(441),
					FParser.SourceLine, FParser.SourcePos, "");
			FParser.NextToken();
		}
		// Unary operators, NOT, unary - and functions
		private void dosign(ref Variant FValue)
		{
			string aoperator = "";
			EvalIdentifier iden = null;
			IdenFunction idenf = null;

			if (FParser.Token == TokenType.Operator)
			{
				aoperator = FParser.TokenString();
				if ((aoperator == "-") || (aoperator == "+") ||
														(aoperator == "NOT") || (aoperator == "IIF"))
					FParser.NextToken();
			}
			else
				if (FParser.Token == TokenType.Symbol)
				{
					iden = SearchIdentifier(FParser.TokenString());
					if (iden == null)
						throw new EvalException(Translator.TranslateStr(440) + FParser.TokenString(),
							FParser.SourceLine, FParser.SourcePos, "");
					// Process parameters
					if (iden is IdenFunction)
					{
						idenf = (IdenFunction)iden;
						int i;

						if (idenf.ParamCount > 0)
						{
							FParser.NextToken();
							if (FParser.Token != TokenType.Operator)
								throw new EvalException(Translator.TranslateStr(441),
									FParser.SourceLine, FParser.SourcePos, "");
							if (FParser.TokenString() != "(")
								throw new EvalException(Translator.TranslateStr(441),
									FParser.SourceLine, FParser.SourcePos, "");
						}
						for (i = 0; i < idenf.ParamCount; i++)
						{
							FParser.NextToken();
							// Looks for the param
							separator(ref idenf.Params[i]);
							// Parameter separator
							if (idenf.ParamCount > i + 1)
							{
								aoperator = "";
								if (FParser.Token == TokenType.Operator)
									aoperator = FParser.TokenString();
								if (aoperator != ",")
									throw new EvalException(String.Format(Translator.TranslateStr(448), ","),
										FParser.SourceLine, FParser.SourcePos, "");
							}
						}

						if (idenf.ParamCount > 0)
						{
							if (FParser.Token != TokenType.Operator)
								throw new EvalException(Translator.TranslateStr(441),
									FParser.SourceLine, FParser.SourcePos, "");
							if (FParser.TokenString() != ")")
								throw new EvalException(Translator.TranslateStr(441),
									FParser.SourceLine, FParser.SourcePos, "");
							FParser.NextToken();
						}
					}
					else
						iden = null;
					aoperator = "";
				}
			if (iden == null)
			{
				if (aoperator == "IIF")
					ExecuteIIF(ref FValue);
				else
					parentesis(ref FValue);
			}
			else
			{
				if (idenf.ParamCount < 1)
					FParser.NextToken();
			}
			if (iden != null)
			{
				// Execute the function
				if (!FChecking)
					FValue = idenf.Value;
                else
                    FValue = new Variant();
			}
			else
			{
				if (aoperator == "-")
				{
					if (!FChecking)
						FValue = -FValue;
				}
				else
					if (aoperator == "NOT")
					{
						if (!FChecking)
							FValue = !FValue;
					}
			}
		}
		private void mul_div(ref Variant FValue)
		{
			string aoperator;
			Variant auxiliar = new Variant();
			Variant auxiliar2 = new Variant();

			dosign(ref FValue);
			while (FParser.Token == TokenType.Operator)
			{
				aoperator = FParser.TokenString();
				if (aoperator == "*")
				{
					FParser.NextToken();
					auxiliar2 = FValue;
					dosign(ref auxiliar);
                    if (!FChecking)
                    {
                        try
                        {
                            FValue = auxiliar2 * auxiliar;
                        }
                        catch
                        {
                            throw;
                        }
                    }
				}
				else
					if (aoperator == "/")
					{
						FParser.NextToken();
						auxiliar2 = FValue;
						dosign(ref auxiliar);
                        if (!FChecking)
                        {
                            try
                            {
                                FValue = auxiliar2 / auxiliar;
                            }
                            catch
                            {
                                throw;
                            }
                        }
					}
					else
						break;
			}
		}
		private void sum_diff(ref Variant FValue)
		{
			string aoperator;
			Variant auxiliar = new Variant();
			Variant auxiliar2 = new Variant();

			mul_div(ref FValue);
			while (FParser.Token == TokenType.Operator)
			{
				aoperator = FParser.TokenString();
				if (aoperator == "+")
				{
					FParser.NextToken();
					auxiliar2 = FValue;
					mul_div(ref auxiliar);
					FValue = auxiliar2 + auxiliar;
				}
				else
					if (aoperator == "-")
					{
						FParser.NextToken();
						auxiliar2 = FValue;
						mul_div(ref auxiliar);
						FValue = auxiliar2 - auxiliar;
					}
					else
						break;
			}
		}
		private void comparations(ref Variant FValue)
		{
			string aoperator;
			Variant auxiliar = new Variant();
			Variant auxiliar2 = new Variant();

			sum_diff(ref FValue);
			while (FParser.Token == TokenType.Operator)
			{
				aoperator = FParser.TokenString();
				if ((aoperator == "<>") || (aoperator == "><"))
				{
					FParser.NextToken();
					auxiliar2 = FValue;
					sum_diff(ref auxiliar);
					if (!FChecking)
						FValue = (auxiliar != auxiliar2);
				}
				else
					if ((aoperator == "<>") || (aoperator == "><"))
					{
						FParser.NextToken();
						auxiliar2 = FValue;
						sum_diff(ref auxiliar);
						if (!FChecking)
							FValue = (auxiliar != auxiliar2);
					}
					else
						if ((aoperator == "==") || (aoperator == "="))
						{
							FParser.NextToken();
							auxiliar2 = FValue;
							sum_diff(ref auxiliar);
							if (!FChecking)
								FValue = (auxiliar == auxiliar2);
						}
						else
							if (aoperator == "<")
							{
								FParser.NextToken();
								auxiliar2 = FValue;
								sum_diff(ref auxiliar);
								if (!FChecking)
									FValue = (auxiliar2 < auxiliar);
							}
							else
								if (aoperator == "<=")
								{
									FParser.NextToken();
									auxiliar2 = FValue;
									sum_diff(ref auxiliar);
									if (!FChecking)
										FValue = (auxiliar2 <= auxiliar);
								}
								else
									if (aoperator == ">")
									{
										FParser.NextToken();
										auxiliar2 = FValue;
										sum_diff(ref auxiliar);
										if (!FChecking)
											FValue = (auxiliar2 > auxiliar);
									}
									else
										if (aoperator == ">=")
										{
											FParser.NextToken();
											auxiliar2 = FValue;
											sum_diff(ref auxiliar);
											if (!FChecking)
												FValue = (auxiliar2 >= auxiliar);
										}
										else
											break;
			}
		}
		private void logicalAND(ref Variant FValue)
		{
			// Precedes the and operator
			comparations(ref FValue);

			string aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString().ToUpper();
			while (aoperator == "AND")
			{
				Variant auxiliar = new Variant();
				Variant auxiliar2 = new Variant();
				auxiliar2 = FValue;
				FParser.NextToken();
				comparations(ref auxiliar);
				// Compatible types?
				if (!FChecking)
					FValue = (bool)auxiliar2 && (bool)auxiliar;
				aoperator = "";
				if (FParser.Token == TokenType.Operator)
					aoperator = FParser.TokenString().ToUpper();
			}

		}
		private void logicalOR(ref Variant FValue)
		{
			// Precedes the and operator
			logicalAND(ref FValue);

			string aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString().ToUpper();
			while (aoperator == "OR")
			{
				Variant auxiliar = new Variant();
				Variant auxiliar2 = new Variant();
				auxiliar2 = FValue;
				FParser.NextToken();
				logicalAND(ref auxiliar);
				// Compatible types?
				if (!FChecking)
					FValue = (bool)auxiliar2 || (bool)auxiliar;
				aoperator = "";
				if (FParser.Token == TokenType.Operator)
					aoperator = FParser.TokenString().ToUpper();
			}
		}
		private void variables(ref Variant FValue)
		{
			EvalIdentifier iden;

			if (FParser.Token == TokenType.Symbol)
			{
				iden = SearchIdentifier(FParser.TokenString());
				if (iden == null)
					throw new EvalException(Translator.TranslateStr(440) + FParser.TokenString() + FParser.TokenString(),
						FParser.SourceLine, FParser.SourcePos, "");
				else
				{
					// Assignment operator creates a new variable
					if (iden is IdenVariable)
					{
						// Looks for a :=
						if (FParser.IsAssignment())
						{
							// Assign the value to it
							FParser.NextToken();
							FParser.NextToken();
							separator(ref FValue);
							if (!FChecking)
								(iden as IdenVariable).Value = FValue;
							string aoperator = "";
							if (FParser.Token == TokenType.Operator)
							aoperator = FParser.TokenString();
							if (aoperator==";")
								separator(ref FValue);
						}
						else
							logicalOR(ref FValue);
					}
					else
						logicalOR(ref FValue);
				}
			}
			else
				logicalOR(ref FValue);
		}
		private void separator(ref Variant FValue)
		{
			string aoperator = "";
			if (FParser.Token == TokenType.Operator)
				aoperator = FParser.TokenString();
			if (aoperator==";")
			{
				while (aoperator == ";")
				{
					FParser.NextToken();
					if (FParser.Token!=TokenType.Eof)
					{
						variables(ref FValue);
					}
					aoperator = "";
					if (FParser.Token == TokenType.Operator)
						aoperator = FParser.TokenString();
				}
			}
			else
			{
				variables(ref FValue);
				aoperator = "";
				if (FParser.Token == TokenType.Operator)
					aoperator = FParser.TokenString();
				while (aoperator == ";")
				{
					FParser.NextToken();
					if (FParser.Token!=TokenType.Eof)
					{
						variables(ref FValue);
					}
					aoperator = "";
					if (FParser.Token == TokenType.Operator)
						aoperator = FParser.TokenString();
				}
			}
		}
		private Variant EvaluateExpression()
		{
			Variant FPartial = new Variant();
			try
			{
				FEvaluating = true;
				try
				{
					separator(ref FPartial);
				}
				finally
				{
					FEvaluating = false;
				}
				if (FParser.Token != TokenType.Eof)
					throw new EvalException(Translator.TranslateStr(449),
						FParser.SourceLine, FParser.SourcePos, "");
			}
			catch (EvalException)
			{
				throw;
			}
			catch (Exception E)
			{
				throw new EvalException(E.Message,
						FParser.SourceLine, FParser.SourcePos, "");
			}
			return FPartial;
		}
		public Variant EvaluateText(string text)
		{
			if (FEvaluating)
			{
				Evaluator eval = new Evaluator();
				eval.AliasList = AliasList;
				eval.FIdentifierList = FIdentifierList;
				eval.Expression = text;
				eval.Evaluate();
				return eval.Result;
			}
			else
			{
				Expression = text;
				Evaluate();
				return Result;
			}
		}
		public Variant Evaluate()
		{
			FParser.Expression = FExpression;
			FChecking = false;
			if (FParser.Token == TokenType.Eof)
			{
				FResult = true;
			}
			else
				FResult = EvaluateExpression();

			return FResult;
		}
		public string Expression
		{
			set
			{
				if (FEvaluating)
					throw new UnNamedException(Translator.TranslateStr(450));
				FExpression = value;

			}
			get
			{
				return FExpression;
			}
		}


		public Evaluator(System.ComponentModel.IContainer container)
		{
			//
			// Required for the designer in Windows.Forms
			//
#if REPMAN_DESIGN
			container.Add(this);
#endif
			InitializeComponent();

			//
			//
			DoInit();
		}
		private void DoInit()
		{
			FParser = new EvalParser();
			FIdentifierList = new EvalIdentifiers();
			AddIdentifiers();
		}
		public Evaluator()
		{
			//
			// Required for the designer in Windows.Forms
			//
			InitializeComponent();

			//
			//
			DoInit();
		}
		private void AddIdentifiers()
		{
			FIdentifierList.Clear();

			FIdentifierList.Add("TRUE", new IdenTrue(this));
			FIdentifierList.Add("FALSE", new IdenFalse(this));
            FIdentifierList.Add("FALSO", new IdenFalse(this));
            FIdentifierList.Add("CIERTO", new IdenTrue(this));
            FIdentifierList.Add("UPPERCASE", new IdenUpperCase(this));
			FIdentifierList.Add("STRINGTOBIN", new IdenStringToBin(this));
			FIdentifierList.Add("DAY", new IdenDay(this));
            FIdentifierList.Add("MONTHNAME", new IdenMonthName(this));
            FIdentifierList.Add("YEAR", new IdenYear(this));
            FIdentifierList.Add("TRIM", new IdenTrim(this));
			FIdentifierList.Add("FORMATSTR", new IdenFormatStr(this));
			FIdentifierList.Add("LEFT", new IdenLeft(this));
			FIdentifierList.Add("RIGHT", new IdenRight(this));
			FIdentifierList.Add("LENGTH", new IdenLength(this));
			FIdentifierList.Add("POS", new IdenPos(this));
			FIdentifierList.Add("SUBSTR", new IdenSubStr(this));
			FIdentifierList.Add("LOWERCASE", new IdenLowerCase(this));
			FIdentifierList.Add("STR", new IdenStr(this));
			FIdentifierList.Add("FILEEXISTS", new IdenFileExists(this));
			FIdentifierList.Add("NULL", new IdenNull(this));
			FIdentifierList.Add("TODAY", new IdenToday(this));
			FIdentifierList.Add("NOW", new IdenNow(this));
			FIdentifierList.Add("SIN", new IdenSinus(this));
			FIdentifierList.Add("MAX", new IdenMax(this));
			FIdentifierList.Add("ISINTEGER", new IdenIsInteger(this));
			FIdentifierList.Add("ISNUMERIC", new IdenIsNumeric(this));
			FIdentifierList.Add("ISVALIDDATETIME", new IdenIsValidDateTime(this));
			FIdentifierList.Add("CHECKEXPRESSION", new IdenCheckExpression(this));
            FIdentifierList.Add("FLOATTODATETIME", new IdenFloatToDateTime(this));
            FIdentifierList.Add("ROUND", new IdenRound(this));
            FIdentifierList.Add("ROUNDTOINTEGER", new IdenRoundToInteger(this));
            FIdentifierList.Add("ABS", new IdenAbs(this));
            FIdentifierList.Add("COMPAREVALUE", new IdenCompareValue(this));
            FIdentifierList.Add("INT", new IdenInt(this));
            FIdentifierList.Add("VAL", new IdenVal(this));
            FIdentifierList.Add("MOD", new IdenModul(this));
            FIdentifierList.Add("EVALTEXT", new IdenEvalText(this));
            FIdentifierList.Add("NUMTOTEXT", new IdenNumToText(this));
        }

		/// <summary> 
		/// Cleanup resources
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (components != null)
					{
						components.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
	 		}
		}


		#region Generated code by Desginer
		/// <summary>
		/// Required for the designer in Windows.Forms
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
