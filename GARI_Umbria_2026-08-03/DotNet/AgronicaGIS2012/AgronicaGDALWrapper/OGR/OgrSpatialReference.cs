using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// This class respresents a OpenGIS Spatial Reference System, and contains methods for converting between this object organization and well known text (WKT) format.
	/// </summary>
	public class OgrSpatialReference
	{
		internal HandleRef spatialReferenceHandle;

		/// <summary>
		/// Validate SRS tokens.
		/// </summary>
		public bool IsValid
		{
			get
			{
				OGRErr error = OgrInterop.OSRValidate(this.spatialReferenceHandle);
				return (error == OGRErr.None);
			}
		}

		#region constructors

		/// <summary>
		/// construcator of OgrSpatialReference
		/// </summary>
		public OgrSpatialReference()
		{
			IntPtr ptrSR = OgrInterop.OSRNewSpatialReference(String.Empty);
			if (ptrSR == IntPtr.Zero)
			{
				throw new Exception("Spatial reference could not be created.");
			}
			this.spatialReferenceHandle = new HandleRef(this, ptrSR);
		}

		internal OgrSpatialReference(IntPtr ptrSRS)
		{
			this.spatialReferenceHandle = new HandleRef(this, ptrSRS);
		}

		// couldn't get this working
		// tried marshalling to ANSI ptr, string

		//public OgrSpatialReference(string wellKnownTextDefinition)
		//{
		//    IntPtr ptrSR = OgrInterop.OSRNewSpatialReference(wellKnownTextDefinition);
		//    if (ptrSR == IntPtr.Zero)
		//    {
		//        throw new Exception("Spatial reference could not be created.");
		//    }
		//    this.spatialReferenceHandle = new HandleRef(this, ptrSR);
		//}

		#endregion

		#region public methods

		/// <summary>
		/// Set the user visible PROJCS name.
		/// </summary>
		/// <param name="coordSysName">the user visible name to assign. Not used as a key.</param>
		public void SetProjectionCoordinateSystem(string coordSysName)
		{
			OGRErr error = OgrInterop.OSRSetProjCS(this.spatialReferenceHandle, coordSysName);

			if (error != OGRErr.None)
			{
				throw new OgrException(error);
			}
		}
		
		/// <summary>
		/// This may be called on an empty OGRSpatialReference to make a geographic coordinate system, or on something with an existing PROJCS node to set the underlying geographic coordinate system of a projected coordinate system.
		/// </summary>
		/// <param name="coordSysName">name of well known geographic coordinate system.</param>
		public void SetWellKnownGeographicCoordinateSystem(string coordSysName)
		{
			OGRErr error = OgrInterop.OSRSetWellKnownGeogCS(this.spatialReferenceHandle, coordSysName);
			if (error != OGRErr.None)
			{
				throw new OgrException(error);
			}
		}

		/// <summary>
		/// Universal Transverse Mercator
		/// </summary>
		/// <param name="zone"></param>
		/// <param name="north"></param>
		public void SetUTM(int zone, bool north)
		{
			OGRErr error = OgrInterop.OSRSetUTM(this.spatialReferenceHandle, zone, north ? 1 : 0);
			if (error != OGRErr.None)
			{
				throw new OgrException(error);
			}
		}

		/// <summary>
		/// Get utm zone information.
		/// </summary>
		/// <param name="zone">UTM zone number or zero if this isn't a UTM definition.</param>
		/// <param name="north">true if north</param>
		public void GetUTMZone(out int zone, out bool north)
		{
			int northInt;
			zone = OgrInterop.OSRGetUTMZone(this.spatialReferenceHandle, out northInt);
			north = (northInt > 0);
		}

		/// <summary>
		/// Decrements the reference count by one.
		/// </summary>
		public void Dereference()
		{
			OgrInterop.OSRDereference(this.spatialReferenceHandle);
		}

		/// <summary>
		/// Make a duplicate of this OGRSpatialReference.
		/// </summary>
		/// <returns>a new SRS, which becomes the responsibility of the caller.</returns>
		public OgrSpatialReference Clone()
		{
			return new OgrSpatialReference(OgrInterop.OSRClone(this.spatialReferenceHandle));

		}

		/// <summary>
		/// Convert this SRS into WKT format.
		/// </summary>
		/// <returns>the resulting string is returned</returns>
		public unsafe string GetWktName()
		{
			//IntPtr wktNamePtr = IntPtr.Zero;
			char* wktNamePtr = null;
			OgrInterop.OSRExportToWkt(this.spatialReferenceHandle, &wktNamePtr);
			IntPtr wktIntPtr = new IntPtr(wktNamePtr);
			string wktName = Marshal.PtrToStringAnsi(wktIntPtr);
			OgrInterop.VSIFree(wktNamePtr);
			return wktName;
		}

		#endregion
	}
}
