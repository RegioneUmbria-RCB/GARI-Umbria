using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	public enum OpenAccessTypes
	{
		ReadOnly = 0,
		Write
	};

	/// <summary>
	/// This class represents a data source. A data source potentially consists 
	/// of many layers (OGRLayer). A data source normally consists of one, or a 
	/// related set of files, though the name doesn't have to be a real item in 
	/// the file system.
	/// </summary>
	public class OgrDataSource : IDisposable
	{
		internal HandleRef dataSourceHandle;

		#region constructor/destructor

		/// <summary>
		/// Creates an instance of OgrDataSource
		/// </summary>
		/// <param name="ptrDataSource">Pointer to the datasource.</param>
		internal OgrDataSource(IntPtr ptrDataSource)
		{
			this.dataSourceHandle = new HandleRef(this, ptrDataSource);
		}

		/// <summary>
		/// destructor
		/// </summary>
		~OgrDataSource()
		{
			Dispose();
		}

		#endregion

		#region IDisposable Members

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				
			}
			this.Release();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region public properties

		/// <summary>
		/// Returns the number of layers in the Data Source
		/// </summary>
		public int LayerCount
		{
			get
			{
				return OgrInterop.OGR_DS_GetLayerCount(this.dataSourceHandle);
			}
		}

		/// <summary>
		/// Returns the datasource name.
		/// </summary>
		public string Name
		{
			get
			{
				return OgrInterop.OGR_Dr_GetName(this.dataSourceHandle);
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// This function attempts to create a new layer on the data source with the
		/// indicated name, coordinate system, geometry type. 
		/// </summary>
		/// <param name="layerName">The name of the new layer.</param>
		/// <param name="spatialReference">The coordinate system reference.</param>
		/// <param name="geometryType">The type of geometry.</param>
		/// <returns>OgrLayer object if successful. Null otherwise.</returns>
		public OgrLayer CreateLayer(string layerName,
			OgrSpatialReference spatialReference, OGRwkbGeometryType geometryType)
		{
			//The papszOptions argument can be used to control driver specific
			// creation options. These options are normally documented in the format
			//specific documentation.
			List<string> options = new List<string>();
			options.Add(string.Empty);

			string[] optionsArray = options.ToArray();

			IntPtr ptrLayer = OgrInterop.OGR_DS_CreateLayer(this.dataSourceHandle,
				layerName, spatialReference.spatialReferenceHandle, geometryType,
				ref optionsArray);

			if (ptrLayer != IntPtr.Zero)
			{
				return new OgrLayer(ptrLayer);
			}
			return null;
		}

		/// <summary>
		/// Fetch a layer by index.
		/// The returned layer remains owned by the OGRDataSource and
		/// should not be deleted by the application.
		/// </summary>
		/// <param name="layerIndex">Layer index</param>
		/// <returns>OgrLayer object if successful. Null otherwise.</returns>
		public OgrLayer GetLayer(int layerIndex)
		{
			IntPtr ptrLayer = OgrInterop.OGR_DS_GetLayer(this.dataSourceHandle, layerIndex);
			if (ptrLayer != IntPtr.Zero)
			{
				return new OgrLayer(ptrLayer);
			}
			return null;
		}
		
		/// <summary>
		/// Fetch a layer by name. The returned layer remains owned by the OGRDataSource and should not be deleted by the application.
		/// </summary>
		/// <param name="layerName">the layer name of the layer to fetch.</param>
		/// <returns>the layer, or NULL if the layer is not found or an error occurs.</returns>
		public OgrLayer GetLayerByName(string layerName)
		{
			IntPtr ptrLayer = OgrInterop.OGR_DS_GetLayerByName(this.dataSourceHandle, layerName);
			if (ptrLayer != IntPtr.Zero)
			{
				return new OgrLayer(ptrLayer);
			}
			return null;
		}
		
		/// <summary>
		/// Returns the name of the data source. This string should be sufficient to open the data source if passed to the same OGRSFDriver that this data source was opened with, but it need not be exactly the same string that was used to open the data source. Normally this a filename.
		/// </summary>
		/// <returns>pointer to an internal name string which should not be modified or freed by the caller.</returns>
		public string GetName()
		{
			return (string)OgrInterop.OGR_DS_GetName(this.dataSourceHandle);
		}		

		/// <summary>
		/// Drop a reference to this datasource,
		/// and if the reference count drops to zero close (destroy) the datasource.
		/// </summary>
		/// <returns>OGR Error code.</returns>
		public OGRErr Release()
		{
			OGRErr error = OGRErr.None;
			if (this.dataSourceHandle.Handle != IntPtr.Zero)
			{
				error = OgrInterop.OGRReleaseDataSource(this.dataSourceHandle);
				this.dataSourceHandle = new HandleRef(this, IntPtr.Zero);
			}
			return error;
		}

		/// <summary>
		/// Test if capability is available.
		/// </summary>
		/// <remarks>
		/// One of the following data source capability names can be passed into this function, and a TRUE or FALSE value will be returned indicating whether or not the capability is available for this object.
		/// 
		/// </remarks>
		/// <param name="caps">the capability to test.</param>
		/// <returns>TRUE if capability available otherwise FALSE.</returns>
		public bool TestCapability(OgrDatasourceCapability caps)
		{
			int iRes = OgrInterop.OGR_DS_TestCapability(this.dataSourceHandle, caps.ToString());
			
			if(iRes==0)
				return false;
			else
				return true;					
		}
		
		#endregion
	}
	
	/// <summary>
	/// enum for datasource capabilities
	/// </summary>
	public enum OgrDatasourceCapability
	{
		/// <summary>
		/// True if this datasource can create new layers.
		/// </summary>
		CreateLayer
	};
}
