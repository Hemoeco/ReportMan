using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Reporting
{
    /// <summary>
    ///  This enumeration is used in string parsing functions, a string is divided in tokens,
    /// each token can represent operators, symbols...
    /// </summary>
	public enum TokenType
    {
        /// <summary>There is no more tokens to parse so current token is End of File</summary>
        Eof,
        /// <summary>The token is an identifier, usually a variable or function name</summary>
        Symbol,
        /// <summary>The token is a string</summary>
        String,
        /// <summary>The token is an integer</summary>
        Integer,
        /// <summary>The token is a dobule value</summary>
        Double,
        /// <summary>The token is a decimal value</summary>
        Decimal,
        /// <summary>The token is an operator (sum,multiply...)</summary>
        Operator,
        /// <summary>The token is a code commnet </summary>
        Comment

    };
}
