using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Concrete class representing polygons.
	/// </summary>
	public class OgrPolygon : OgrSurface
	{
		#region constructors

		internal OgrPolygon(IntPtr ptrPolygon)
			: base(ptrPolygon, OGRwkbGeometryType.wkbPolygon)
		{
		}
		
		/// <summary>
		/// OgrPolygon
		/// </summary>
		public OgrPolygon()
			: base(OGRwkbGeometryType.wkbPolygon)
		{
		}

		#endregion

		#region public methods

		/// <summary>
		/// Adds linear ring object to the polygon object.
		/// </summary>
		/// <param name="linearRing"></param>
		public void AddRingDirectly(OgrLinearRing linearRing)
		{
			OGRErr error = OgrInterop.OGR_G_AddGeometryDirectly(this.geometryHandle,
				linearRing.geometryHandle);

			if (error != OGRErr.None)
			{
				string message = "Could not add Linear Ring to Polygon. " +
					error.ToString();

				throw new OgrException(message);
			}
		}

		/// <summary>
		/// Gets the polygon's exterior linear ring.
		/// </summary>
		/// <returns>OgrLinearRing object if one exists. Null otherwise.</returns>
		public OgrLinearRing GetExteriorRing()
		{
			IntPtr ptrLinearRing = OgrInterop.OGR_G_GetGeometryRef(this.geometryHandle, 0);
			if (ptrLinearRing != IntPtr.Zero)
			{
				return new OgrLinearRing(ptrLinearRing);
			}
			return null;
		}
		
		/// <summary>
		/// Compute geometry area.
		/// </summary>
		/// <returns>the area or 0.0 for unsupported geometry types.</returns>
		public double GetArea()
		{
			return (double)OgrInterop.OGR_G_GetArea(this.geometryHandle);
		}

		#endregion
	}
}
