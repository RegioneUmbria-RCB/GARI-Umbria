using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GDALWrapper.Gdal
{
    public class GdalDriver
    {
        internal HandleRef hDriver;

		#region public properties

		/// <summary>
		/// Gets the short name of the driver
		/// </summary>
		public string ShortName
		{
			get { return GdalInterOp.GDALGetDriverShortName(hDriver); }
		}

		/// <summary>
		/// Gets the long name of the driver
		/// </summary>
		public string LongName
		{
			get { return GdalInterOp.GDALGetDriverLongName(hDriver); }
		}

		#endregion

		#region constructors

		public GdalDriver()
        {
            hDriver = new HandleRef(this, IntPtr.Zero);
        }

        public GdalDriver(string name)
        {
            hDriver = new HandleRef(this, GdalInterOp.GDALGetDriverByName(name));

            if (hDriver.Handle == IntPtr.Zero)
                throw (new ArgumentException("Driver not found"));
        }

        internal GdalDriver(IntPtr ptrDriver)
        {
            hDriver = new HandleRef(this, ptrDriver);
		}

		#endregion

		#region public methods

		/// <summary>
        /// Create a copy of a dataset.
        /// </summary>
        /// <remarks>
        /// <para>This method will attempt to create a copy of a raster dataset with the indicated filename, and in this drivers format. Band number, size, type, projection, geotransform and so forth are all to be copied from the provided template dataset.</para>
        /// <para>Note that many sequential write once formats (such as JPEG and PNG) don't implement the Create() method but do implement this CreateCopy() method. If the driver doesn't implement CreateCopy(), but does implement Create() then the default CreateCopy() mechanism built on calling Create() will be used.</para>
        /// <para>It is intended that CreateCopy() would often be used with a source dataset which is a virtual dataset allowing configuration of band types, and other information without actually duplicating raster data. This virtual dataset format hasn't yet been implemented at the time of this documentation being written.</para>
        /// </remarks>
        /// <param name="filename">Name of output file</param>
        /// <param name="srcDataset">The dataset to copy</param>
        /// <param name="strict">Insist that the copy is a "strict" copy</param>
        /// <param name="options">File construction options. See the GDAL documentation on individual
        /// drivers for details. Note that this argument is currently ignored...</param>
        public GdalDataset CreateCopy(string filename, GdalDataset srcDataset, bool strict, string[] options)
        {
            //Do a straight copy.
            GdalDataset dstDataset = new GdalDataset(GdalInterOp.GDALCreateCopy(hDriver, filename, srcDataset.hDataset, 
                strict ? 1 : 0, new HandleRef(this, IntPtr.Zero), new HandleRef(this, IntPtr.Zero), 
                new HandleRef(this, IntPtr.Zero)));
            if (!dstDataset.IsValid)
                throw new ApplicationException("Error while creating new file " + filename + "\n" +
                    GdalInterOp.CPLGetLastErrorMsg());
            return dstDataset;
        }

        /// <summary>
        /// Create a Dataset in the current driver's format, with the given filename, 
        /// width, height, number of bands and data type.
        /// </summary>
        /// <param name="filename">Filename to create</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="nBands">The number of bands in the image</param>
        /// <param name="type">The data type of the image</param>
        /// <param name="options">Construction options. See the GDAL documentation on individual
        /// drivers for details. Note that this argument is currently ignored...</param>
        /// <returns></returns>
        public GdalDataset Create(string filename, int width, int height, int nBands, DataType type, string[] options)
        {
            GdalDataset dstDataset = new GdalDataset(GdalInterOp.GDALCreate(hDriver, filename, width, height, nBands, type,
                new HandleRef(this, IntPtr.Zero)));
            if (!dstDataset.IsValid)
                throw new ApplicationException("Error while creating new file " + filename + "\n" +
                    GdalInterOp.CPLGetLastErrorMsg());
            return dstDataset;
		}

		public override string ToString()
		{
			return ShortName + " (" + LongName + ")";
		}

		/// <summary>
		/// Fetch the number of registered drivers.
		/// </summary>
		/// <returns>the number of registered drivers.</returns>
		public static int GetDriverCount()
		{
			return GdalInterOp.GDALGetDriverCount();
		}

		/// <summary>
		/// Fetch driver by index.
		/// </summary>
		/// <param name="i">the driver index from 0 to GetDriverCount()-1</param>
		/// <returns>GDAL Driver</returns>
		public static GdalDriver GetDriver(int i)
		{
			return new GdalDriver(GdalInterOp.GDALGetDriver(i));
		}

		public static void DestroyDriverManager()
		{
			GdalInterOp.GDALDestroyDriverManager();
		}

 		#endregion
    }
}
