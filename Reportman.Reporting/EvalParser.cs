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
	/// Class used by Evaluator to divide a expression into tokens
	/// </summary>
	public class EvalParser
	{
		private string FExpression;
		char aoperator;
		private int FSourcePtr, FTokenPtr, FSourceLine, FBufEnd;
		private string FString;
		private int FInteger;
		private double FDouble;
		private decimal FDecimal;
		private TokenType FToken;

		public TokenType Token
		{
			get
			{
				return FToken;
			}
		}
		public int SourceLine
		{
			get
			{
				return FSourceLine;
			}
		}
		public int SourcePos
		{
			get
			{
				return FSourcePtr;
			}
		}
		public string TokenString()
		{
			int L;
			string aresult;
			if (FToken == TokenType.String)
				aresult = FString;
			else
				if (FToken == TokenType.Integer)
					aresult = FInteger.ToString();
				else
					if (FToken == TokenType.Decimal)
						aresult = FDecimal.ToString();
					else
						if (FToken == TokenType.Double)
							aresult = FDouble.ToString();
						else
                            if (FToken == TokenType.Operator)
                            {
                                L = FSourcePtr - FTokenPtr;
                                if (L>2)
                                    aresult = FExpression.Substring(FTokenPtr, L).ToUpper();
                                else
                                    aresult = FExpression.Substring(FTokenPtr, L);
                            }
                            else
						{
							L = FSourcePtr - FTokenPtr;
							aresult = FExpression.Substring(FTokenPtr, L);
							// Brackets out
							if (FToken == TokenType.Symbol)
							{
								if (aresult[0] == '[')
									if (aresult[aresult.Length - 1] == ']')
										aresult = aresult.Substring(1, aresult.Length - 2);
							}
						}
			return aresult;
		}
		public EvalParser()
		{
		}
		private void SkipBlanks()
		{
			while (FSourcePtr <= FBufEnd)
			{
				if (FExpression[FSourcePtr] == (char)10)
					FSourceLine++;
				if (!Char.IsWhiteSpace(FExpression[FSourcePtr]))
					break;
				FSourcePtr++;
			}
		}
		private static bool IsValidFirstSymbol(char alet)
		{
			bool aresult = Char.IsLetter(alet);
			if (!aresult)
			{
				aresult = ((alet == '.') || (alet == '_'));

			}
			return aresult;
		}
		private static bool IsValidSecondSymbol(char alet)
		{
			bool aresult = Char.IsLetterOrDigit(alet);
			if (!aresult)
			{
				aresult = ((alet == '.') || (alet == '_'));

			}
			return aresult;
		}

		public TokenType NextToken()
		{

			int P;
			TokenType resulttype;
			const string EvalOperators = "*+-/()=><:;,";
			const string CompositeOperators = ":!<>=";
			const string HexaDigits = "0123456789ABCDEF";
			const string NumberSet = ".Ee";
			const string FloatExplicit = "cCdDsSfF";
			SkipBlanks();
			P = FSourcePtr;
			FTokenPtr = P;
			resulttype = TokenType.Eof;
			if (P >= FExpression.Length)
			{
				resulttype = TokenType.Eof;
				//				throw new EvalException(Translator.TranslateStr(449,"Syntax error"),
				//					FSourceLine,P-1,"");
			}
			else
				if (IsValidFirstSymbol(FExpression[P]))
				{
					P++;
					while (P < FExpression.Length)
					{
						if (IsValidSecondSymbol(FExpression[P]) ||
							(FExpression[P] == '.'))
							P++;
						else
						{
							break;
						}
					}
					resulttype = TokenType.Symbol;
					// Looks if the symbol is an operator
					string thetoken = FExpression.Substring(FSourcePtr, P - FSourcePtr).ToUpper();
					if ((thetoken == "OR") || (thetoken == "AND") || (thetoken == "NOT") ||
						(thetoken == "IIF"))
					{
						resulttype = TokenType.Operator;
					}
				}
				else
					if (FExpression[P] == '[')
					{
						P++;
						while (P < FExpression.Length)
						{
							if (FExpression[P] != ']')
								P++;
							else
							{
								P++;
								break;
							}
						}

						if (FExpression[P - 1] != ']')
							throw new EvalException(String.Format(Translator.TranslateStr(448), "]"),
								FSourceLine, P - 1, "");
						resulttype = TokenType.Symbol;
					}
					else
						if (EvalOperators.IndexOf(FExpression[P]) >= 0)
						{
							aoperator = FExpression[P];
							P++;
							if (P < FExpression.Length)
							{
								switch (Expression[P])
								{
									case '=':
										if (CompositeOperators.IndexOf(aoperator) >= 0)
											P++;
										break;
									case '<':
										if (aoperator == '>')
											P++;
										break;
									case '>':
										if (aoperator == '<')
											P++;
										break;
								}
							}
							resulttype = TokenType.Operator;
						}
						else
							// Strings
							if ((FExpression[P] == '#') || (FExpression[P] == '\''))
							{
								int J = 0, I = 0;
								FString = "";
								while (P < FExpression.Length)
								{
									if (FExpression[P] == '#')
									{
										P++;
										while (P < FExpression.Length)
										{
											if (Char.IsDigit(FExpression[P]))
											{
												I = I * 10 + ((int)FExpression[P] - (int)'0');
												P++;
												J++;
											}
											else
												break;
										}
										FString = FString + (char)I;
									}
									else
										if (FExpression[P] == '\'')
										{
											P++;
											while (P < FExpression.Length)
											{
												if (FExpression[P] == '\'')
												{
													P++;
                                                    if (P < FExpression.Length)
                                                    {
                                                        if (FExpression[P] != '\'')
                                                        {
                                                            J++;
                                                            break;
                                                        }
                                                        else
                                                            FString = FString + FExpression[P];
                                                    }
                                                    else
                                                    {
                                                        J++;
                                                        break;
                                                    }
												}
												else
													FString = FString + FExpression[P];
												J++;
												P++;
											}
										}
										else
											break;
								}
								if (P >= FExpression.Length)
									if (J == 0)
										throw new EvalException(String.Format(Translator.TranslateStr(448), "'"),
											FSourceLine, P - 1, "");
								resulttype = TokenType.String;
							}
							else
								if (FExpression[P] == '$')
								{
									P++;
									int I = 0, J = 0, index;
									if (P < FExpression.Length)
									{
										index = HexaDigits.IndexOf(Char.ToUpper(FExpression[P]));
										while (index >= 0)
										{
											I = I * 16 + index;
											J++;
											P++;
											if (P < FExpression.Length)
												index = HexaDigits.IndexOf(Char.ToUpper(FExpression[P]));
											else
												index = -1;
										}
									}
									if (J == 0)
										throw new EvalException(Translator.TranslateStr(445),
											FSourceLine, P - 1, "");
									FInteger = I;
									resulttype = TokenType.Integer;
								}
								else
									if (Char.IsDigit(FExpression[P]))
									{
										string FNumber = "";
										int decimals = -1;

										FNumber = FNumber + FExpression[P];
										P++;
										resulttype = TokenType.Integer;
										int index;
										if (P < FExpression.Length)
										{
											index = NumberSet.IndexOf(FExpression[P]);
											while ((index >= 0) || Char.IsDigit(FExpression[P]))
											{
												if (index == 0)
												{
													if (decimals < 0)
														decimals++;
													else
														throw new EvalException(Translator.TranslateStr(445),
															FSourceLine, P - 1, "");
													FNumber = FNumber + System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
													resulttype = TokenType.Decimal;
												}
												else
													if (index < 0)
														FNumber = FNumber + FExpression[P];
													else
													{
														resulttype = TokenType.Double;
														FNumber = FNumber + FExpression[P];
													}
												P++;
												if (P >= FExpression.Length)
													break;
												index = NumberSet.IndexOf(FExpression[P]);
											}
										}
										if (FNumber.Length > 20)
											resulttype = TokenType.Double;
										if (resulttype == TokenType.Integer)
											if (FNumber.Length > 8)
												resulttype = TokenType.Double;
										// Floatexplicit
										if (P < FExpression.Length)
											if (FloatExplicit.IndexOf(FExpression[P]) >= 0)
											{
												P++;
												resulttype = TokenType.Double;
											}
										switch (resulttype)
										{
											case TokenType.Integer:
												FInteger = System.Convert.ToInt32(FNumber);
												break;
											case TokenType.Decimal:
												FDecimal = System.Convert.ToDecimal(FNumber);
												break;
											case TokenType.Double:
												FDouble = System.Convert.ToDouble(FNumber);
												break;
										}

									}

			FToken = resulttype;
			FSourcePtr = P;
			return resulttype;
		}
		public string Expression
		{
			get
			{
				return FExpression;
			}
			set
			{
				// End character to simply algoritms
				FExpression = value;
				FSourcePtr = 0;
				FTokenPtr = 1;
				FSourceLine = 1;
				FBufEnd = FExpression.Length - 2;
				NextToken();
			}
		}
		public int AsInteger
		{
			get
			{
				return FInteger;
			}
		}
		public double AsDouble
		{
			get
			{
				return FDouble;
			}
		}
		public decimal AsDecimal
		{
			get
			{
				return FDecimal;
			}
		}
		public bool IsAssignment()
		{
			int index = FSourcePtr;
			bool foundtwopoints = false;
			bool foundequal = false;
			while (index <= FBufEnd)
			{
				if (!Char.IsWhiteSpace(FExpression[index]))
				{
					if (FExpression[index] == ':')
					{
						if (foundtwopoints)
							break;
						else
							foundtwopoints = true;
					}
					else
						if (FExpression[index] == '=')
						{
							if (foundtwopoints)
							{
								foundequal = true;
							}
							break;
						}
						else
							break;
				}
				else
					if (foundtwopoints)
						break;
				index++;
			}
			return (foundtwopoints && foundequal);
		}
	}
}
