using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Available field types for a OgrFieldDefinition
	/// </summary>
	public enum OGRFieldType
	{
		/// <summary>
		/// Simple 32bit integer
		/// </summary>
		Integer = 0,
		/// <summary>
		/// List of 32bit integers
		/// </summary>
		IntegerList = 1,		
		/// <summary>
		/// Double Precision floating point
		/// </summary>
		Real = 2,			
		/// <summary>
		/// List of doubles
		/// </summary>
		RealList = 3,		
		/// <summary>
		/// String of ASCII chars
		/// </summary>
		String = 4,			
		/// <summary>
		/// Array of strings
		/// </summary>
		StringList = 5,		 
		/// <summary>
		/// Double byte string (unsupported)
		/// </summary>
		WideString = 6,		
		/// <summary>
		/// List of wide strings (unsupported)
		/// </summary>
		WideStringList = 7,	
		/// <summary>
		/// Raw Binary data (unsupported)
		/// </summary>
		Binary = 8			
	};

	/// <summary>
	/// Definition of an attribute of an OGRFeatureDefn.
	/// </summary>
	public class OgrFieldDefinition
	{
		internal HandleRef fieldDefnHandle;

		#region constructors

		internal OgrFieldDefinition(IntPtr ptrFieldDefinition)
		{
			this.fieldDefnHandle = new HandleRef(this, ptrFieldDefinition);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fieldDefinitionName">the name of the new field.</param>
		/// <param name="fieldType">the type of the new field.</param>
		public OgrFieldDefinition(string fieldDefinitionName, OGRFieldType fieldType)
		{
			IntPtr ptrFieldDefn = OgrInterop.OGR_Fld_Create(fieldDefinitionName, fieldType);
			if (ptrFieldDefn != IntPtr.Zero)
			{
				this.fieldDefnHandle = new HandleRef(this, ptrFieldDefn);
			}
			else
			{
				throw new OgrException("Could not create field definition");
			}
		}

		#endregion

		#region public properties

		/// <summary>
		/// Gets or sets the formatting width for this field.
		/// </summary>
		public int Width
		{
			get
			{
				return OgrInterop.OGR_Fld_GetWidth(this.fieldDefnHandle);
			}
			set
			{
				OgrInterop.OGR_Fld_SetWidth(this.fieldDefnHandle, value);
			}
		}

		/// <summary>
		/// Gets or sets the formatting precision for this field.
		/// This should normally be zero for fields of types other than OFTReal.
		/// </summary>
		public int Precision
		{
			get
			{
				return OgrInterop.OGR_Fld_GetPrecision(this.fieldDefnHandle);
			}
			set
			{
				OgrInterop.OGR_Fld_SetPrecision(this.fieldDefnHandle, value);
			}
		}

		/// <summary>
		/// Gets the OGRFieldType of this field.
		/// </summary>
		public OGRFieldType FieldType
		{
			get	{ return OgrInterop.OGR_Fld_GetType(this.fieldDefnHandle); }
		}
		
		/// <summary>
		/// Gets or sets the justification for this field.
		/// </summary>
		public OgrJustification Justify
		{
			get { return OgrInterop.OGR_Fld_GetJustify(this.fieldDefnHandle); }
			set { OgrInterop.OGR_Fld_SetJustify(this.fieldDefnHandle, value ); }
		}

		/// <summary>
		/// Gets the name of this field.
		/// </summary>
		public string FieldName
		{
			get
			{
				return Marshal.PtrToStringAnsi(OgrInterop.OGR_Fld_GetNameRef(this.fieldDefnHandle));
			}
		}

		#endregion

		#region public static methods

		/// <summary>
		/// Gets the OGRFieldType from a .NET object type.
		/// </summary>
		/// <param name="objectType">The type of the field.</param>
		/// <returns>OGRFieldType</returns>
		/// <exception cref="NotSupportedException">If the objectType is not supported.</exception>
		public static OGRFieldType GetFieldTypeFromType(Type objectType)
		{
			OGRFieldType fieldType;

			if (objectType == typeof(int) || objectType == typeof(long))
			{
				fieldType = OGRFieldType.Integer;
			}
			else if (objectType == typeof(double))
			{
				fieldType = OGRFieldType.Real;
			}
			else
			{
				throw new NotSupportedException(objectType.ToString() + " not supported.");
			}

			return fieldType;
		}
		
		/// <summary>
		/// Destroy a field definition.
		/// </summary>
		public void Destroy()
		{
			OgrInterop.OGR_Fld_Destroy(this.fieldDefnHandle);			
		}

		#endregion
	}
	
	/// <summary>
	/// Display justification for field values.
	/// </summary>
	public enum OgrJustification
	{
	    /// <summary>
	    /// justify is undefined
	    /// </summary>
		Undefined = 0,
		/// <summary>
		/// justify left
		/// </summary>
	    Left = 1,
	    /// <summary>
	    /// justify right
	    /// </summary>
	    Right = 2
	};
}
