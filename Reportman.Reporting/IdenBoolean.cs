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
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Reportman.Drawing;


namespace Reportman.Reporting
{
	// Classes and constats for parser and evaluator

    /// <summary>
    /// True constant for expression evaluator
    /// </summary>
	class IdenTrue : IdenConstant
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eval"></param>
		public IdenTrue(Evaluator eval)
			: base(eval)
		{
			FValue = true;
			Name = "TRUE";
		}
	}
    /// <summary>
    /// Null constant for expression evaluator
    /// </summary>
    class IdenNull : IdenConstant
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eval"></param>
		public IdenNull(Evaluator eval)
			: base(eval)
		{
			Name = "NULL";
		}
	}
    /// <summary>
    /// False constant for expression evaluator
    /// </summary>
    class IdenFalse : IdenConstant
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eval"></param>
		public IdenFalse(Evaluator eval)
			: base(eval)
		{
			FValue = false;
			Name = "FALSE";
		}
	}
}
