using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Definition of a feature class or feature layer.
	/// </summary>
	public class OgrFeatureDefinition
	{
		internal HandleRef featureDefinitionHandle;

		#region constructors

		internal OgrFeatureDefinition(IntPtr ptrFeatureDefinition)
		{
			this.featureDefinitionHandle = new HandleRef(this, ptrFeatureDefinition);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="featureDefinitionName">	the name to be assigned to this layer/class. It does not need to be unique.</param>
		public OgrFeatureDefinition(string featureDefinitionName)
		{
			IntPtr ptrFD = OgrInterop.OGR_FD_Create(featureDefinitionName);
			if (ptrFD != IntPtr.Zero)
			{
				this.featureDefinitionHandle = new HandleRef(this, ptrFD);
			}
			else
			{
				throw new OgrException("Could not create feature definition");
			}
		}

		#endregion

		#region public properties

		/// <summary>
		/// Gets the number of fields in the field definition.
		/// </summary>
		public int NumberFields
		{
			get
			{
				return OgrInterop.OGR_FD_GetFieldCount(this.featureDefinitionHandle);
			}
		}

        /// <summary>
        /// Gets or sets the geometry base type of the feature definition.
        /// </summary>
        public OGRwkbGeometryType GeomType
        {
            get { return OgrInterop.OGR_FD_GetGeomType(this.featureDefinitionHandle); }
            set { OgrInterop.OGR_FD_SetGeomType(this.featureDefinitionHandle, value); }
        }

		#endregion

		#region public methods

		/// <summary>
		/// Gets the field definition.
		/// </summary>
		/// <param name="fieldIndex">The field index.</param>
		/// <returns>Field definition object if successful. Null otherwise.</returns>
		public OgrFieldDefinition GetFieldDefinition(int fieldIndex)
		{
			IntPtr ptrFD = OgrInterop.OGR_FD_GetFieldDefn(this.featureDefinitionHandle, fieldIndex);
			if (ptrFD != IntPtr.Zero)
			{
				return new OgrFieldDefinition(ptrFD);
			}
			return null;
		}

		#endregion
	}
	


}
