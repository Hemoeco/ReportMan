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
using System.ComponentModel;
#if REPMAN_DESIGN
using System.ComponentModel.Design.Serialization;
using System.Drawing;
#endif
using System.Reflection;
using System.Data;
using Reportman.Drawing;

internal class resfinder { }

namespace Reportman.Reporting
{
    /// <summary>
    /// DatasetAlias is a component that can contain references to datasets for substitution
    /// in report dataset list or expression evaluator
    /// </summary>
#if REPMAN_DESIGN
	[ToolboxBitmapAttribute(typeof(DatasetAlias), "datasetalias.ico")]
#endif
    public class DatasetAlias : System.ComponentModel.Component
	{
		/// <summary>
		/// Required variable for designer
		/// </summary>
		private System.ComponentModel.Container components = null;


		private IdenField FIden;
		private AliasCollection FList;

#if REPMAN_DESIGN
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
		public AliasCollection List
		{
			get
			{
				return (FList);
			}
		}
		private void DoInit()
		{
			FList = new AliasCollection();
			FIden = new IdenField();
		}
		public DatasetAlias()
		{
			//
			// Required for the designer in Windows.Forms
			//
			InitializeComponent();
			//
			DoInit();
		}
		public DatasetAlias(System.ComponentModel.IContainer container)
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
		public EvalIdentifier FindField(string fieldname, string datasetname, ref bool duplicated)
		{
			duplicated = false;
			EvalIdentifier iden = null;
			DataTable adatatable;
			DataTable adata = null;
			string acolumn = null;
			int acurrentrow = 0;

			int i, index;

			if (datasetname.Length == 0)
			{
				for (i = 0; i < FList.Count; i++)
				{
					adatatable = FList[i].Data;
					if (adatatable != null)
					{
						index = adatatable.Columns.IndexOf(fieldname);
						if (index >= 0)
						{
							if (acolumn != null)
							{
								duplicated = true;
								break;
							}
							acolumn = fieldname;
							adata = adatatable;
							acurrentrow = FList[i].CurrentRow;
						}

					}
					if (duplicated)
						break;
				}
			}
			else
			{
				for (i = 0; i < FList.Count; i++)
				{
                    if (FList[i].Alias == datasetname)
                    {
                        adatatable = FList[i].Data;
                        if (adatatable != null)
                        {
                            index = adatatable.Columns.IndexOf(fieldname);
                            if (index >= 0)
                            {
                                adata = adatatable;
                                acolumn = fieldname;
                                acurrentrow = FList[i].CurrentRow;
                                break;
                            }

                        }
                    }
					if (acolumn != null)
						break;
				}
			}
			if (acolumn != null)
			{
				FIden.Field = acolumn;
				FIden.Data = adata;
				FIden.CurrentRow = acurrentrow;
				iden = FIden;
			}
			return (iden);
		}

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
		/// Needed by Designer
		/// </summary>
		private void InitializeComponent()
		{

		}
#endregion
	}
#if REPMAN_DESIGN
	internal class AliasCollectionItemConverter:TypeConverter
	{
		//If asked if we can convert to an
		//InstanceDescriptor then return "true".
		//Otherwise ask our base class.
		public override Boolean CanConvertTo(
			ITypeDescriptorContext context,
			Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return
				base.CanConvertTo(context,
				destinationType);
		}

		//Our converter is capable of performing
		//the conversion.
		public override object ConvertTo(
			ITypeDescriptorContext context,
			System.Globalization.CultureInfo culture,
			object value,
			Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor)) 
			{
				Type valueType = value.GetType();
				ConstructorInfo ci = valueType.GetConstructor(System.Type.EmptyTypes);
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context,culture, value, destinationType);
		}
	}
	
	[TypeConverter(typeof(AliasCollectionItemConverter))]
#endif
	public class AliasCollectionItem
	{
		string FAlias;
		DataTable FData;
		public int CurrentRow;

		public DataTable Data
		{
			get
			{
				return (FData);
			}
			set
			{
				FData = value;
			}
		}
		public string Alias
		{
			get
			{
				return FAlias;
			}
			set
			{
				FAlias = value.ToUpper();
			}
		}
		public AliasCollectionItem()
		{
		}

	}
	public class AliasCollection : CollectionBase
	{
		public AliasCollectionItem this[int index]
		{
			get
			{
				return ((AliasCollectionItem)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(AliasCollectionItem value)
		{
			return (List.Add(value));
		}

		public int IndexOf(AliasCollectionItem value)
		{
			return (List.IndexOf(value));
		}

		public void Insert(int index, AliasCollectionItem value)
		{
			List.Insert(index, value);
		}

		public void Remove(AliasCollectionItem value)
		{
			List.Remove(value);
		}

		public bool Contains(AliasCollectionItem value)
		{
			// If value is not of type AliasCollectionItem, this will return false.
			return (List.Contains(value));
		}

		protected override void OnInsert(int index, Object value)
		{
			if (!(value is AliasCollectionItem))
				throw new ArgumentException("value must be of type AlliasCollectionItem.", "value");
		}

		protected override void OnRemove(int index, Object value)
		{
			if (!(value is AliasCollectionItem))
				throw new ArgumentException("value must be of type AlliasCollectionItem.", "value");
		}

		protected override void OnSet(int index, Object oldValue, Object newValue)
		{
			if (!(newValue is AliasCollectionItem))
				throw new ArgumentException("value must be of type AlliasCollectionItem.", "value");
		}

		protected override void OnValidate(Object value)
		{
			if (!(value is AliasCollectionItem))
				throw new ArgumentException("value must be of type AlliasCollectionItem.", "value");
		}
		public void CopyTo(AliasCollectionItem[] array,
			int index)
		{
			List.CopyTo(array, index);
		}

	}


}
