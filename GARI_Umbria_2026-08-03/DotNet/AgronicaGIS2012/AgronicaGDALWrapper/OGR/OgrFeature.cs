using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// A simple feature, including geometry and attributes.
	/// </summary>
	public class OgrFeature : IDisposable
	{
		internal HandleRef featureHandle;

		#region constructors

		internal OgrFeature(IntPtr ptrFeature)
		{
			this.featureHandle = new HandleRef(this, ptrFeature);
		}

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="featureDefinition">feature class (layer) definition to which the feature will adhere.</param>
		public OgrFeature(OgrFeatureDefinition featureDefinition)
		{
			IntPtr ptrfeature = OgrInterop.OGR_F_Create(featureDefinition.featureDefinitionHandle);
			if (ptrfeature != IntPtr.Zero)
			{
				this.featureHandle = new HandleRef(this, ptrfeature);
			}
			else
			{
				throw new OgrException("Could not create feature");
			}
		}

		/// <summary>
		/// destructor
		/// </summary>
		~OgrFeature()
		{
			Dispose();
		}

		#endregion

		#region properties
		
		/// <summary>
		/// Fetch number of fields on this feature. This will always be the same as the field count for the OGRFeatureDefn.
		/// </summary>
		public Int32 FieldCount
		{
			get { return OgrInterop.OGR_F_GetFieldCount(this.featureHandle); }
		}
		#endregion
		
		#region public methods

		/// <summary>
		/// Set the feature geometry.
		/// </summary>
		/// <param name="geometry">The geometry to apply to this feature.</param>
		public void SetGeometryDirectly(OgrGeometry geometry)
		{
			OGRErr error = OgrInterop.OGR_F_SetGeometryDirectly(this.featureHandle,
				geometry.geometryHandle);

			if (error != OGRErr.None)
			{
				throw new OgrException(error);
			}
		}

		/// <summary>
		/// Set the feature style string.
		/// </summary>
		/// <param name="styleString">The style string to apply to this feature.</param>
		public void SetStyleString(string styleString)
		{
			OgrInterop.OGR_F_SetStyleString(this.featureHandle, styleString);
		}

		#region GetFeatureGeometry methods

		/// <summary>
		/// Get the feature's geometry object. Because this is only a wrapper
		/// object, there is no way to cast the base OgrGeometry class to one
		/// of its child classes, so use this method to get the base class, determine
		/// its type then call the specific geometry method to get the proper object.
		/// </summary>
		/// <returns>OgrGeometry object if successful. Null otherwise.</returns>
		public OgrGeometry GetFeatureGeometry()
		{
			IntPtr ptrGeom = OgrInterop.OGR_F_GetGeometryRef(this.featureHandle);
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}

		/// <summary>
		/// Gets the feature's polygon object.
		/// </summary>
		/// <returns>OgrPolygon object if one exists. Null if the feature's geometry is not a polygon.</returns>
		public OgrPolygon GetFeaturePolygon()
		{
			IntPtr ptrGeom = OgrInterop.OGR_F_GetGeometryRef(this.featureHandle);
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrPolygon(ptrGeom);
			}
			return null;
		}

		/// <summary>
		/// Destroy feature
		/// </summary>
		public void DestroyFeature()
		{
			if (this.featureHandle.Handle != IntPtr.Zero)
			{
				OgrInterop.OGR_F_Destroy(this.featureHandle);
				this.featureHandle = new HandleRef(this, IntPtr.Zero);
			}
		}
		
		/// <summary>
		/// Get feature identifier.
		/// </summary>
		/// <returns>feature id or OGRNullFID if none has been assigned.</returns>
		public Int32 GetFID()
		{
			return (Int32)OgrInterop.OGR_F_GetFID(this.featureHandle);
		}
		
		/// <summary>
		/// Set the feature identifier.
		/// </summary>
		/// <param name="nFID"></param>
		public void SetFID( Int32 nFID)
		{
			OgrInterop.OGR_F_SetFID(this.featureHandle, nFID);
		}

		#endregion

		#region field get/set methods

		/// <summary>
		/// Sets the value of a field.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <param name="value">The value to which the field is to be set.</param>
		public void SetField(int fieldIndex, object value)
		{
			Type valueType = value.GetType();

			if (valueType == typeof(int))
			{
				SetField(fieldIndex, (int)value);
			}
			else if (valueType == typeof(long))
			{
				SetField(fieldIndex, (long)value);
			}
			else if (valueType == typeof(double))
			{
				SetField(fieldIndex, (double)value);
			}
			else if (valueType == typeof(string))
			{
				SetField(fieldIndex, (string)value);
			}
			else
			{
				throw new NotSupportedException("Field type " + valueType.ToString() + " not supported.");
			}
		}

		/// <summary>
		/// Set field to integer value.
		/// OFTInteger and OFTReal fields will be set directly.
		/// OFTString fields will be assigned a string representation of the value,
		/// but not necessarily taking into account formatting constraints on this field.
		/// Other field types may be unaffected.
		/// </summary>
		/// <param name="fieldIndex"></param>
		/// <param name="value">The value to assign.</param>
		public void SetField(int fieldIndex, int value)
		{
			OgrInterop.OGR_F_SetFieldInteger(this.featureHandle, fieldIndex, value);
		}

		/// <summary>
		/// Set field to integer value.
		/// OFTInteger and OFTReal fields will be set directly.
		/// OFTString fields will be assigned a string representation of the value,
		/// but not necessarily taking into account formatting constraints on this field.
		/// Other field types may be unaffected.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <param name="value">The value to assign.</param>
		public void SetField(int fieldIndex, long value)
		{
			OgrInterop.OGR_F_SetFieldInteger(this.featureHandle, fieldIndex, (int)value);
		}

		/// <summary>
		/// Set field to double value.
		/// OFTInteger and OFTReal fields will be set directly.
		/// OFTString fields will be assigned a string representation of the value,
		/// but not necessarily taking into account formatting constraints on this field.
		/// Other field types may be unaffected.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <param name="value">The value to assign.</param>
		public void SetField(int fieldIndex, double value)
		{
			OgrInterop.OGR_F_SetFieldDouble(this.featureHandle, fieldIndex, value);
		}

		/// <summary>
		/// Set field to string value.
		/// OFTInteger fields will be set based on an atoi() conversion of the string.
		/// OFTReal fields will be set based on an atof() conversion of the string.
		/// Other field types may be unaffected.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <param name="value">The value to assign.</param>
		public void SetField(int fieldIndex, string value)
		{
			OgrInterop.OGR_F_SetFieldString(this.featureHandle, fieldIndex, value);
		}

		/// <summary>
		/// Fetch field value as integer.
		/// OFTString features will be translated using atoi().
		/// OFTReal fields will be cast to integer.
		/// Other field types, or errors will result in a return value of zero.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <returns>Field value as an integer.</returns>
		public int GetFieldAsInteger(int fieldIndex)
		{
			return OgrInterop.OGR_F_GetFieldAsInteger(this.featureHandle, fieldIndex);
		}

		/// <summary>
		/// Fetch field value as a double.
		/// String features will be translated using atof().
		/// Integer fields will be cast to double.
		/// Other field types, or errors will result in a return value of zero.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <returns>Field value as a double.</returns>
		public double GetFieldAsDouble(int fieldIndex)
		{
			return OgrInterop.OGR_F_GetFieldAsDouble(this.featureHandle, fieldIndex);
		}

		/// <summary>
		/// Fetch field value as a string.
		/// OFTReal and OFTInteger fields will be translated to string using sprintf(),
		/// but not necessarily using the established formatting rules.
		/// Other field types, or errors will result in a return value of zero.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <returns>The field value as a string.</returns>
		public string GetFieldAsString(int fieldIndex)
		{
			return OgrInterop.OGR_F_GetFieldAsString(this.featureHandle, fieldIndex);
		}

		#endregion

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
			}
		}

		#endregion
	}
}
