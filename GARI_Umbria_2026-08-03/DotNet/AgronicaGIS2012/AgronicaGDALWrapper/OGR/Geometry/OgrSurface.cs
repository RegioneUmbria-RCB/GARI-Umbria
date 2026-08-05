using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Abstract base class for 2 dimensional objects like polygons.
	/// </summary>
	public class OgrSurface : OgrGeometry
	{
		internal OgrSurface(IntPtr ptrSurface, OGRwkbGeometryType geometryType)
			: base(ptrSurface, geometryType)
		{
		}
		
		/// <summary>
		/// consructor of OgrSurface
		/// </summary>
		/// <param name="geometryType"></param>
		public OgrSurface(OGRwkbGeometryType geometryType)
			: base(geometryType)
		{
		}
	}
}
