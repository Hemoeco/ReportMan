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
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Reportman.Reporting;
#if REPMAN_DOTNET2
using System.Collections.Generic;
#endif

namespace Reportman.Reporting.Forms
{

      internal class ControlInfo
      {
          public Label label;
          public Control control;
          public Control control2;
          public Button button;
          public Param param;
          public int PosY;
      }
#if REPMAN_DOTNET1
	internal class ControlInfoList:IEnumerable
	{
		ControlInfo[] FItems;
		const int FIRST_ALLOCATION_OBJECTS = 100;
		int FCount;
		public ControlInfoList()
		{
			FCount = 0;
			FItems = new ControlInfo[FIRST_ALLOCATION_OBJECTS];
		}
		public void Clear()
		{
			FCount = 0;
		}
		private void CheckRange(int index)
		{
			if ((index < 0) || (index >= FCount))
				throw new Exception("Index out of range on ControlInfoList collection");
		}
		public ControlInfo this[int index]
		{
			get { CheckRange(index); return FItems[index]; }
			set { CheckRange(index); FItems[index] = value; }
		}
		public int Count { get { return FCount; } }
		public void Add(ControlInfo obj)
		{
			if (FCount > (FItems.Length - 2))
			{
				ControlInfo[] nobjects = new ControlInfo[FCount];
				System.Array.Copy(FItems, 0, nobjects, 0, FCount);
				FItems = new ControlInfo[FItems.Length * 2];
				System.Array.Copy(nobjects, 0, FItems, 0, FCount);
			}
			FItems[FCount] = obj;
			FCount++;
		}
	
#else
	internal  class ControlInfoList:System.Collections.Generic.List<ControlInfo>
	{
	
#endif
		// IEnumerable Interface Implementation:
		//   Declaration of the GetEnumerator() method 
		//   required by IEnumerable
		public new IEnumerator GetEnumerator()
		{
			return new ControlInfoEnumerator(this);
		}
		// Inner class implements IEnumerator interface:
		public class ControlInfoEnumerator : IEnumerator
		{
			private int position = -1;
			private ControlInfoList t;

			public ControlInfoEnumerator(ControlInfoList t)
			{
				this.t = t;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < t.Count - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					return t[position];
				}
			}
		}

	}
}
