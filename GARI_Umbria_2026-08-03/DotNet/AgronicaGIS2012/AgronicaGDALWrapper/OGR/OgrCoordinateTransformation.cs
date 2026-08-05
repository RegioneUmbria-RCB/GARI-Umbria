using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Object for transforming between coordinate systems.
	/// </summary>
	public class OgrCoordinateTransformation
	{
		internal HandleRef coordinateTransformHandle;

		#region constructors

		/// <summary>
		/// creates a new transformation
		/// </summary>
		/// <param name="sourceSRS">source SRS</param>
		/// <param name="targetSRS">target SRS</param>
		public OgrCoordinateTransformation(OgrSpatialReference sourceSRS, OgrSpatialReference targetSRS)
		{
			IntPtr ptrCT = OgrInterop.OCTNewCoordinateTransformation(sourceSRS.spatialReferenceHandle,
				targetSRS.spatialReferenceHandle);
			if (ptrCT != IntPtr.Zero)
			{
				this.coordinateTransformHandle = new HandleRef(this, ptrCT);
			}
			else
			{
				string message = Gdal.GdalInterOp.CPLGetLastErrorMsg();
				throw new OgrException("Could not create coordinate transformation. " + message);
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Transform points from source to destination space.
		/// </summary>
		/// <param name="numPoints">number of points to transform.</param>
		/// <param name="x">array of nCount X vertices, modified in place</param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Transform(int numPoints, ref double[] x, ref double[] y)
		{
			// need z coordinates to pass to method
			double[] z = new double[numPoints];
			for (int i = 0; i < numPoints; i++)
			{
				z[i] = 0.0;
			}

			if (OgrInterop.OCTTransform(this.coordinateTransformHandle, numPoints, x, y, z) == 0)
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}
