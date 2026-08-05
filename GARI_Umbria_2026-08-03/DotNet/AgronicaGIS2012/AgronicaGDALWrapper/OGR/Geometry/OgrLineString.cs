using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// OgrLineString object
	/// </summary>
	public class OgrLineString : OgrCurve
	{
		#region constructors

		/// <summary>
		/// constructor for OgrLineString
		/// </summary>
		/// <param name="ptrLineString">pointer of linestring geometry</param>
		internal OgrLineString(IntPtr ptrLineString)
			: base(ptrLineString, OGRwkbGeometryType.wkbLineString)
		{
		}

		/// <summary>
		/// constructor for OgrLineString 
		/// </summary>
		/// <param name="ptrLineString">pointer of linestring geometry</param>
		/// <param name="geometryType">type of geometry</param>
		internal OgrLineString(IntPtr ptrLineString, OGRwkbGeometryType geometryType)
			: base(ptrLineString, geometryType)
		{
		}

		/// <summary>
		/// constructor for OgrLineString
		/// </summary>
		/// <param name="geometryType">type of geometry</param>
		public OgrLineString(OGRwkbGeometryType geometryType)
			: base(geometryType)
		{
		}

		#endregion

		#region public methods

		/// <summary>
		/// Add point to a line string geometry.
		/// </summary>
		/// <param name="x">x coordinate of the point to add.</param>
		/// <param name="y">y coordinate of the point to add.</param>
		public void AddPoint(double x, double y)
		{
			OgrInterop.OGR_G_AddPoint_2D(this.geometryHandle, x, y);
		}

		#endregion
	}
}
