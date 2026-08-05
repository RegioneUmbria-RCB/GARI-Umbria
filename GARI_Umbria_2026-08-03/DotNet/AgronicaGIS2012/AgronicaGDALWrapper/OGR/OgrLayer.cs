using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using GDALWrapper.OGR;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// This class represents a layer of simple features, with access methods.
	/// </summary>
	public class OgrLayer
	{
		internal HandleRef layerHandle;

		#region constructor

		internal OgrLayer(IntPtr ptrLayer)
		{
			this.layerHandle = new HandleRef(this, ptrLayer);
		}
		
		#endregion

		#region public methods

		/// <summary>
		/// Create a new field on a layer. You must use this to create new fields on a real layer.
		/// Internally the OGRFeatureDefn for the layer will be updated to reflect the new field.
		/// Applications should never modify the OGRFeatureDefn used by a layer directly.
		/// </summary>
		/// <param name="fieldDefn">The field definition for the field to be created.</param>
		/// <param name="approxOK">
		/// If TRUE, the field may be created in a slightly different form depending on the
		/// limitations of the format driver.
		/// </param>
		/// <returns>Error code</returns>
		public OGRErr CreateField(OgrFieldDefinition fieldDefn, bool approxOK)
		{
			return OgrInterop.OGR_L_CreateField(this.layerHandle,
				fieldDefn.fieldDefnHandle, (approxOK ? 1 : 0));
		}

		/// <summary>
		/// Delete feature from layer.
		/// </summary>
		/// <remarks>
		/// The feature with the indicated feature id is deleted from the layer if supported by the driver. Most drivers do not support feature deletion, and will return
		/// OGRERR_UNSUPPORTED_OPERATION. The OGR_L_TestCapability() function may be called with OLCDeleteFeature to check if the driver supports feature deletion.
		/// </remarks>
		/// <param name="nFID">the feature id of the feature to delete.</param>
		public void DeleteFeature(long nFID)
		{
			OGRErr error = OgrInterop.OGR_L_DeleteFeature(this.layerHandle, nFID);
			if (error != OGRErr.None)
			{
				string message = "Could not delete feature from layer. " + error.ToString();
				throw new OgrException(message);
			}
		}
		
		/// <summary>
		/// Called CreateFeature in the C API, but is really adding a feature to 
		/// the layer.
		/// </summary>
		/// <param name="feature">The feature to be added to this layer.</param>
		public void AddFeature(OgrFeature feature)
		{
			OGRErr error = OgrInterop.OGR_L_CreateFeature(this.layerHandle,
				feature.featureHandle);

			if (error != OGRErr.None)
			{
				string message = "Could not add feature to layer. " + error.ToString();
				throw new OgrException(message);
			}
		}

		/// <summary>
		/// Reset feature reading to start on the first feature. This affects GetNextFeature().
		/// </summary>
		public void ResetReading()
		{
			OgrInterop.OGR_L_ResetReading(this.layerHandle);
		}

		/// <summary>
		/// Fetch the next available feature from this layer.
		/// The returned feature becomes the responsiblity of the caller to delete.
		/// 
		/// It is critical that all features associated with an OGRLayer
		/// (more specifically an OGRFeatureDefn) be deleted before that layer/datasource is deleted.
		/// Only features matching the current spatial filter (set with SetSpatialFilter()) will be returned.
		/// This function implements sequential access to the features of a layer.
		/// The OGR_L_ResetReading() function can be used to start at the beginning again.
		/// </summary>
		/// <returns>OgrFeature object. Null if the end of the layer has been reached.</returns>
		public OgrFeature GetNextFeature()
		{
			IntPtr ptrFeature = OgrInterop.OGR_L_GetNextFeature(this.layerHandle);
			if (ptrFeature != IntPtr.Zero)
			{
				return new OgrFeature(ptrFeature);
			}
			return null;
		}
		
		/// <summary>
		/// Fetch a feature by it's identifier.
		/// </summary>
		/// <param name="nFeatureId">the feature id of the feature to read.</param>
		/// <returns>a feature now owned by the caller, or NULL on failure.</returns>
		public OgrFeature GetFeature(Int64 nFeatureId)
		{
			IntPtr ptrFeature = OgrInterop.OGR_L_GetFeature(this.layerHandle, nFeatureId);
			if (ptrFeature != IntPtr.Zero)
			{
				return new OgrFeature(ptrFeature);
			}
			return null;
		}	
		
		/// <summary>
		/// Fetch the feature count in this layer.
		/// </summary>
		/// <param name="bForce">Flag indicating whether the count should be computed even if it is expensive.</param>
		/// <returns>feature count, -1 if count not known.</returns>
		public int GetFeatureCount(bool bForce)
		{
			if (bForce)
				return OgrInterop.OGR_L_GetFeatureCount(this.layerHandle, 1);
			else
				return OgrInterop.OGR_L_GetFeatureCount(this.layerHandle, 0);
		}			
		
		/// <summary>
		/// Fetch the schema information for this layer.
		/// </summary>
		/// <returns>OgrFeatureDefinition object if successful. Null otherwise.</returns>
		public OgrFeatureDefinition GetLayerFeatureDefinition()
		{
			IntPtr ptrFD = OgrInterop.OGR_L_GetLayerDefn(this.layerHandle);
			if (ptrFD != IntPtr.Zero)
			{
				return new OgrFeatureDefinition(ptrFD);
			}
			return null;
		}

		/// <summary>
		/// Fetch the extent of this layer.
		/// </summary>
		/// <returns>the extent of this layer</returns>
		public OgrEnvelope GetExtent()
		{
			StructOgrEnvelope structOgrEnvelope = new StructOgrEnvelope();
						
			IntPtr ptrFD = OgrInterop.OGR_L_GetExtent(this.layerHandle, ref structOgrEnvelope, 1);

			return new OgrEnvelope(structOgrEnvelope.MinX,
			                       structOgrEnvelope.MinY,
			                       structOgrEnvelope.MaxX,
			                       structOgrEnvelope.MaxY);
		}
		
		/// <summary>
		/// Set a new spatial filter.
		/// </summary>
		/// <remarks>
		/// This function set the geometry to be used as a spatial filter when fetching features via the GetNextFeature() function. Only features that geometrically intersect
		/// the filter geometry will be returned.
		/// </remarks>
		/// <param name="oGeom"></param>
		public void SetSpatialFilter(OgrGeometry oGeom)
		{
			OgrInterop.OGR_L_SetSpatialFilter(this.layerHandle, oGeom.geometryHandle);
		}
		
		/// <summary>
		/// Set a new spatial filter.
		/// </summary>		
		public void SetSpatialFilterRect(double dMinX, double dMinY, double dMaxX, double dMaxY)
		{
			GDALWrapper.OGR.OgrLinearRing  oRing = new GDALWrapper.OGR.OgrLinearRing(GDALWrapper.OGR.OGRwkbGeometryType.wkbLinearRing);
    		GDALWrapper.OGR.OgrPolygon oPoly = new GDALWrapper.OGR.OgrPolygon();

    		oRing.AddPoint( dMinX, dMinY );
    		oRing.AddPoint( dMinX, dMaxY );
    		oRing.AddPoint( dMaxX, dMaxY );
    		oRing.AddPoint( dMaxX, dMinY );
    		oRing.AddPoint( dMinX, dMinY );

    		oPoly.AddRingDirectly( oRing );

    		this.SetSpatialFilter( oPoly );
		}		
		
		/// <summary>
		/// This function returns the current spatial filter for this layer.
		/// </summary>
		/// <returns>the spatial filter geometry.</returns>
		public OgrGeometry GetSpatialFilter()
		{
			IntPtr ptrSF = OgrInterop.OGR_L_GetSpatialFilter(this.layerHandle);
			if (ptrSF != IntPtr.Zero)
			{
				return new OgrGeometry(ptrSF);
			}
			return null;			
		}
		
		/// <summary>
		/// Set a new attribute query.
		/// </summary>
		/// <param name="Query">query in restricted SQL WHERE format, or NULL to clear the current query.</param>
		/// <returns>OGRERR_NONE if successfully installed, or an error code if the query expression is in error, or some other failure occurs.</returns>
		public OGRErr SetAttributeFilter(string Query)
		{
			return (OGRErr)OgrInterop.OGR_L_SetAttributeFilter(this.layerHandle, Query);
		}
		
		/// <summary>
		/// Fetch the spatial reference system for this layer.
		/// </summary>
		/// <returns>spatial reference, or NULL if there isn't one.</returns>
		public OgrSpatialReference GetSpatialRef()
		{
			IntPtr ptrSRS = OgrInterop.OGR_L_GetSpatialRef(this.layerHandle);
			if(ptrSRS != IntPtr.Zero)
			{
				return new OgrSpatialReference(ptrSRS);
			}
			
			return null;		
		}
		
		/// <summary>
		/// Test if this layer supported the named capability.
		/// </summary>
		/// <remarks>
		/// RandomRead - TRUE if the OGR_L_GetFeature() function works for this layer.
		/// SequentialWrite - TRUE if the OGR_L_CreateFeature() function works for this layer. Note this means that this particular layer is writable. The same OGRLayer class may returned FALSE for other layer instances that are effectively read-only.
		/// RandomWrite - TRUE if the OGR_L_SetFeature() function is operational on this layer. Note this means that this particular layer is writable. The same OGRLayer class may returned FALSE for other layer instances that are effectively read-only.
		/// FastSpatialFilter - TRUE if this layer implements spatial filtering efficiently. Layers that effectively read all features, and test them with the OGRFeature intersection methods should return FALSE. This can be used as a clue by the application whether it should build and maintain it's own spatial index for features in this layer.
		/// FastFeatureCount - TRUE if this layer can return a feature count (via OGR_L_GetFeatureCount()) efficiently ... ie. without counting the features. In some cases this will return TRUE until a spatial filter is installed after which it will return FALSE.
		/// FastGetExtent - TRUE if this layer can return its data extent (via OGR_L_GetExtent()) efficiently ... ie. without scanning all the features. In some cases this will return TRUE until a spatial filter is installed after which it will return FALSE.
		/// </remarks>
		/// <param name="caps">the name of the capability to test.</param>
		/// <returns>TRUE if the layer has the requested capability, or FALSE otherwise.</returns>
		public bool TestCapabilities(OgrLayerCapabilities caps)
		{
			int iRes = OgrInterop.OGR_L_TestCapability(this.layerHandle, caps.ToString());
			
			if(iRes==0)
				return false;
			else
				return true;				
		}
		
		#endregion
	}
	
	/// <summary>
	/// Avialable layer capabilities
	/// </summary>
	public enum OgrLayerCapabilities
	{
		/// <summary>
		/// TRUE if the OGR_L_GetFeature() function works for this layer.
		/// </summary>
		RandomRead,
		/// <summary>
		/// TRUE if the OGR_L_CreateFeature() function works for this layer. Note this means that this particular layer is writable. The same OGRLayer class may returned FALSE for other layer instances that are effectively read-only.
		/// </summary>
		SequentialWrite,
		/// <summary>
		/// TRUE if the OGR_L_SetFeature() function is operational on this layer. Note this means that this particular layer is writable. The same OGRLayer class may returned FALSE for other layer instances that are effectively read-only.
		/// </summary>
		RandomWrite,
		/// <summary>
		/// TRUE if this layer implements spatial filtering efficiently. Layers that effectively read all features, and test them with the OGRFeature intersection methods should return FALSE. This can be used as a clue by the application whether it should build and maintain it's own spatial index for features in this layer.
		/// </summary>
		FastSpatialFilter,
		/// <summary>
		/// TRUE if this layer can return a feature count (via OGR_L_GetFeatureCount()) efficiently ... ie. without counting the features. In some cases this will return TRUE until a spatial filter is installed after which it will return FALSE.
		/// </summary>
		FastFeatureCount,
		/// <summary>
		/// TRUE if this layer can return its data extent (via OGR_L_GetExtent()) efficiently ... ie. without scanning all the features. In some cases this will return TRUE until a spatial filter is installed after which it will return FALSE.
		/// </summary>
		FastGetExtent
	};
}
