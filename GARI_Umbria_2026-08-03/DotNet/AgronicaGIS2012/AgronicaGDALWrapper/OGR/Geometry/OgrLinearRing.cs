using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Concrete representation of a closed ring.
	/// </summary>
	public class OgrLinearRing : OgrLineString
	{
		/// <summary>
		/// constructor of OgrLinearRing
		/// </summary>
		/// <param name="ptrLinearRing">pointer to a geometry</param>
		internal OgrLinearRing(IntPtr ptrLinearRing)
			: base(ptrLinearRing, OGRwkbGeometryType.wkbLinearRing)
		{
		}

		/// <summary>
		/// constructor of OgrLinearRing
		/// </summary>
		/// <param name="ptrLinearRing">pointer to a geometry</param>
		/// <param name="geometryType">type of geometry</param>
		internal OgrLinearRing(IntPtr ptrLinearRing, OGRwkbGeometryType geometryType)
			: base(ptrLinearRing, geometryType)
		{
		}

		/// <summary>
		/// constructor of OgrLinearRing
		/// </summary>
		/// <param name="geometryType">type of geometry</param>
		public OgrLinearRing(OGRwkbGeometryType geometryType)
			: base(geometryType)
		{
		}

		/// <summary>
		/// constructor of OgrLinearRing
		/// </summary>
		public OgrLinearRing()
			: base(OGRwkbGeometryType.wkbLinearRing)
		{
		}

		/// <summary>
		/// Force linear ring to be closed.
		/// Simply adds the first point as the end point to close the ring.
		/// </summary>
		public void CloseRings()
		{
			int numberPoints = OgrInterop.OGR_G_GetPointCount(this.geometryHandle);
			if (numberPoints < 2)
			{
				return;
			}

			double x0, y0, z0;
			double xend, yend, zend;

			OgrInterop.OGR_G_GetPoint(this.geometryHandle, 0, out x0, out y0, out z0);
			OgrInterop.OGR_G_GetPoint(this.geometryHandle, numberPoints - 1, out xend, out yend, out zend);

			if (x0 != xend || y0 != yend || z0 != zend)
			{
				OgrInterop.OGR_G_AddPoint_2D(this.geometryHandle, x0, y0);
			}
		}
		
		/// <summary>
		/// Compute geometry area.
		/// </summary>
		/// <returns>the area or 0.0 for unsupported geometry types.</returns>
		public double GetArea()
		{
			return (double)OgrInterop.OGR_G_GetArea(this.geometryHandle);
		}

	}
}
