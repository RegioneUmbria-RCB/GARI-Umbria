using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GDALWrapper.Gdal
{
	public enum Access
	{
		ReadOnly = 0,
		Update = 1,
	}

	public class GdalDataset : IDisposable
	{
		internal HandleRef hDataset;

		#region constructors 

		/// <summary>
		/// Construct a Dataset that is associated with an existing file.
		/// </summary>
		/// <param name="filename">Filename to open</param>
		/// <param name="access">Access mode with which to open file</param>
		public GdalDataset(string filename, Access access)
		{
			hDataset = new HandleRef(this, GdalInterOp.GDALOpen(filename, access));
			if (!IsValid)
			{
				throw new IOException("Unable to open file " + filename + "\n" +
					GdalInterOp.CPLGetLastErrorMsg());
			}
		}

		/// <summary>
		/// Construct a Dataset that is associated with an existing file, with ReadOnly access.
		/// </summary>
		/// <param name="filename"></param>
		public GdalDataset(string filename) : this(filename, Access.ReadOnly) { }

		internal GdalDataset(IntPtr ptrDataset)
		{
			hDataset = new HandleRef(this, ptrDataset);
		}

		#endregion

		#region public properties

		/// <summary>
		/// True if the Dataset was successfully opened / created and has not been closed.
		/// </summary>
		public bool IsValid
		{
			get { return hDataset.Handle != IntPtr.Zero; }
		}

		/// <summary>
		/// The filename associated with this Dataset.
		/// </summary>
		public string Filename
		{
			get { return GdalInterOp.GDALGetDescription(hDataset); }
		}

		/// <summary>
		/// The access with which this Dataset was opened.
		/// </summary>
		public Access Access
		{
			get { return (Access)GdalInterOp.GDALGetAccess(hDataset); }
		}

		/// <summary>
		/// The width of the image.
		/// </summary>
		public int XSize
		{
			get { return GdalInterOp.GDALGetRasterXSize(hDataset); }
		}

		/// <summary>
		/// The height of the image.
		/// </summary>
		public int YSize
		{
			get { return GdalInterOp.GDALGetRasterYSize(hDataset); }
		}

		/// <summary>
		/// The width and height as a System.Drawing.Size object.
		/// </summary>
		public System.Drawing.Size ImageSize
		{
			get { return new System.Drawing.Size(XSize, YSize); }
		}

		/// <summary>
		/// The number of bands in the image.
		/// </summary>
		public int RasterCount
		{
			get { return GdalInterOp.GDALGetRasterCount(hDataset); }
		}

		#endregion

		#region public methods

		/// <summary>
		/// Close the Dataset. This method is also called by Dispose(). Do not invoke any methods
		/// on a Dataset after closing it!
		/// </summary>
		public void Close()
		{
			if (hDataset.Handle != IntPtr.Zero)
			{
				GdalInterOp.GDALClose(hDataset);
				hDataset = new HandleRef(this, IntPtr.Zero);
			}
		}

		/// <summary>
		/// Return a RasterBand object for the i'th band. The first band has index 1.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public GdalRasterBand GetRasterBand(int i)
		{
			if (i > RasterCount || i < 1)
			{
				throw new ApplicationException("Gdal.Dataset.GetRasterBand() invoked with invalid index");
			}
			return new GdalRasterBand(GdalInterOp.GDALGetRasterBand(hDataset, i));
		}

    	/// <summary>
		/// Fetch the affine transformation coefficients
		/// </summary>
		/// <remarks>
		/// Fetches the coefficients for transforming between pixel/line (X,Y) raster space, and projection coordinates (Xp,Yp) space.<br/>
		/// Xp = T[0] + T[1]*X + T[2]*Y<br/>
		/// Yp = T[3] + T[4]*X + T[5]*Y<br/>
		/// In a north up image, T[1] is the pixel width, and T[5] is the pixel height.
		/// The upper left corner of the upper left pixel is at position (T[0],T[3]).
		/// </remarks>
		/// <returns>Affine transformation parameters</returns>
		public GeoTransform GetGeoTransform()
		{
			double[] rawTransform = new double[6];
			if (GdalInterOp.GDALGetGeoTransform(hDataset, rawTransform) == CPLErr.None)
			{
				return new GeoTransform(rawTransform);
			}
			else
			{
				return new GeoTransform();
			}
		}

		/// <summary>
		/// Set the affine transformation coefficients for this dataset.
		/// </summary>
		/// <param name="transform"></param>
		public void SetGeoTransform(GeoTransform transform)
		{
			// Silently ignore errors for now
			GdalInterOp.GDALSetGeoTransform(hDataset, transform.transform);
		}

		/// <summary>
		/// Get the string describing the projection used by this dataset.
		/// </summary>
		/// <returns></returns>
		public string GetProjection()
		{
			return GdalInterOp.GDALGetProjectionRef(hDataset);
		}

		public void SetProjection(string projectionName)
		{
			OGR.OgrSpatialReference spatialReference = new GDALWrapper.OGR.OgrSpatialReference();
			spatialReference.SetProjectionCoordinateSystem(projectionName);
			string wktName = spatialReference.GetWktName();

			CPLErr error = Gdal.GdalInterOp.GDALSetProjection(this.hDataset, wktName);
			spatialReference.Dereference();
			if (error != CPLErr.None)
			{
				throw new GdalException(error);
			}
		}

		public void SetProjection(string projectionName, int utmZone, bool north)
		{
			OGR.OgrSpatialReference spatialReference = new GDALWrapper.OGR.OgrSpatialReference();
			string projectionString = "UTM " + utmZone + " (WGS84) in " +
					(north ? "northern" : "southern") + " hemisphere.";
			spatialReference.SetProjectionCoordinateSystem(projectionString);
			spatialReference.SetWellKnownGeographicCoordinateSystem(projectionName);
			spatialReference.SetUTM(utmZone, north);

			string wktName = spatialReference.GetWktName();

			CPLErr error = Gdal.GdalInterOp.GDALSetProjection(this.hDataset, wktName);
			spatialReference.Dereference();
			if (error != CPLErr.None)
			{
				throw new GdalException(error);
			}
		}

		/// <summary>
		/// Get the Driver object that handles files of the same format as this Dataset.
		/// </summary>
		/// <returns></returns>
		public GdalDriver GetDriver()
		{
			return new GdalDriver(GdalInterOp.GDALGetDatasetDriver(hDataset));
		}

		public Dictionary<string, string> GetMetadata()
		{
			return GdalInterOp.GDALGetMetadata(hDataset, null);
		}

		public string GetMetadataItem(string key)
		{
			return GdalInterOp.GDALGetMetadataItem(hDataset, key, null);
		}

		#endregion

		#region dispose

		/// <summary>
		/// Close the Dataset. Do not invoke any methods on this Dataset after calling Dispose()!
		/// </summary>
		/// <param name="disposing"></param>
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
			}
			Close();
		}

		/// <summary>
		/// Close the Dataset. Do not invoke any methods on this Dataset after calling Dispose()!
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		
		~GdalDataset()
		{
			Dispose(false);
		}

		#endregion
	}
}
