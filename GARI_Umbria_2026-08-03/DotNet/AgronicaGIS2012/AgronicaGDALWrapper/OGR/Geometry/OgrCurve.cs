using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Abstract curve base class.
	/// </summary>
	public class OgrCurve : OgrGeometry
	{
		internal OgrCurve(IntPtr ptrCurve, OGRwkbGeometryType geometryType)
			: base(ptrCurve, geometryType)
		{
		}

		/// <summary>
		/// constructor for OgrCurve
		/// </summary>
		/// <param name="geometryType"></param>
		public OgrCurve(OGRwkbGeometryType geometryType)
			: base(geometryType)
		{
		}
	}
}
