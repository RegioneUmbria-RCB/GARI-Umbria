using System;
using System.Collections.Generic;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// OgrPoint object
	/// </summary>
	public class OgrPoint : OgrGeometry
	{
		/// <summary>
		/// constructor for OgrPoint
		/// </summary>
		/// <param name="geometryType">type of geometry</param>
		public OgrPoint(OGRwkbGeometryType geometryType)
			: base(geometryType)
		{
		}

		/// <summary>
		/// constructor for OgrPoint
		/// </summary>
		public OgrPoint()
			: base(OGRwkbGeometryType.wkbPoint)
		{
		}

		/// <summary>
		/// constructor for OgrPoint
		/// </summary>
		/// <param name="x">x value of the coordinate</param>
		/// <param name="y">y value of the coordinate</param>
		public OgrPoint(double x, double y)
			: base(OGRwkbGeometryType.wkbPoint)
		{
			this.AddPoint(x, y);
		}

		/// <summary>
		/// Add a point to a geometry
		/// </summary>
		/// <param name="x">x coordinate of point to add.</param>
		/// <param name="y">y coordinate of point to add.</param>
		public void AddPoint(double x, double y)
		{
			OgrInterop.OGR_G_AddPoint_2D(this.geometryHandle, x, y);
		}
	}
}
