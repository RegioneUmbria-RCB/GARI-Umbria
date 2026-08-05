using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace GDALWrapper.Gdal
{
	#region enums

	public enum RWFlag
    {
        Read = 0,
        Write = 1
    };

    public enum DataType
    {
        Unknown = 0,
        Byte = 1,
        UInt16 = 2,
        Int16 = 3,
        UInt32 = 4,
        Int32 = 5,
        Float32 = 6,
        Float64 = 7,
        CInt16 = 8,
        CInt32 = 9,
        CFloat32 = 10,
        CFloat64 = 11,
        TypeCount = 12
    };

    /// <summary>
    /// Types of color interpretation for raster bands.
    /// </summary>
    public enum ColorInterp
    {
        Undefined = 0,
        /// <summary>
        /// Greyscale
        /// </summary>
        GrayIndex = 1,
        /// <summary>
        /// Paletted (see associated color table)
        /// </summary>
        PaletteIndex = 2,
        /// <summary>
        /// Red band of RGBA image
        /// </summary>               
        RedBand = 3,
        /// <summary>
        /// Green band of RGBA image
        /// </summary>
        GreenBand = 4,
        /// <summary>
        /// Blue band of RGBA image
        /// </summary>                       
        BlueBand = 5,
        /// <summary>
        /// Alpha (0=transparent, 255=opaque)
        /// </summary>   
        AlphaBand = 6,
        /// <summary>
        /// Hue band of HLS image 
        /// </summary>                 
        HueBand = 7,
        /// <summary>
        /// Saturation band of HLS image 
        /// </summary>       
        SaturationBand = 8,
        /// <summary>
        /// Lightness band of HLS image
        /// </summary>            
        LightnessBand = 9,
        /// <summary>
        /// Cyan band of CMYK image
        /// </summary>                
        CyanBand = 10,
        /// <summary>
        /// Magenta band of CMYK image
        /// </summary>             
        MagentaBand = 11,
        /// <summary>
        /// Yellow band of CMYK image
        /// </summary>             
        YellowBand = 12,
        /// <summary>
        /// Black band of CMYK image
        /// </summary>                      
        BlackBand = 13,
        /// <summary>
        /// Y Luminance
        /// </summary>                             
        YCbCr_YBand = 14,
        /// <summary>
        /// Cb Chroma
        /// </summary>                              
        YCbCr_CbBand = 15,
        /// <summary>
        /// Cr Chroma
        /// </summary>                                          
        YCbCr_CrBand = 16,
        /*! Max current value */
        Max = 16
    };

	#endregion

	public class GdalRasterBand
	{
		internal HandleRef hBand;

		#region public properties

		/// <summary>
		/// Get the color interpretation of this band.
		/// </summary>
		public ColorInterp ColorInterpretation
		{
			get
			{
				return (ColorInterp)GdalInterOp.GDALGetRasterColorInterpretation(hBand);
			}
		}

		public double Offset
		{
			get
			{
				IntPtr success = IntPtr.Zero;
				return GdalInterOp.GDALGetRasterOffset(this.hBand, success);
			}
			set
			{
				GdalInterOp.GDALSetRasterOffset(this.hBand, value);
			}
		}

		public double Scale
		{
			get
			{
				IntPtr success = IntPtr.Zero;
				return GdalInterOp.GDALGetRasterScale(this.hBand, success);
			}
			set
			{
				GdalInterOp.GDALSetRasterScale(this.hBand, value);
			}
		}

		public int OverviewCount
		{
			get { return GdalInterOp.GDALGetOverviewCount(this.hBand); }
		}
		
		#endregion

		#region constructors

		internal GdalRasterBand(IntPtr ptrBand)
        {
            hBand = new HandleRef(this, ptrBand);
		}

		#endregion

		#region public methods

		/// <summary>
        /// Read or write a block of data from/to a RasterBand. The data type of the buffer is automatically
        /// converted to that of the image file if necessary, and resampling is performed as necessary to
        /// match the buffer dimensions to the requested rectangle. The buffer is passed in as an IntPtr.
        /// This method allows for non-standard pixel and line strides to be specified.
        /// </summary>
        /// <param name="flag">Set to RWFlag.Read to read from the file, or RWFlag.Write to write
        /// to the file.</param>
        /// <param name="xOff">The x coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="yOff">The y coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="xSize">The width of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="ySize">The height of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="buffer">A pointer to an in memory buffer.</param>
        /// <param name="bufXSize">The width of the buffer in pixels.</param>
        /// <param name="bufYSize">The height of the buffer in pixels.</param>
        /// <param name="dataType">The type of the data within the buffer.</param>
        /// <param name="pixelStride">The byte offset from the start of one pixel in the buffer
        /// to the start of the next pixel. If zero, defaults to the size of the specified data type.</param>
        /// <param name="lineStride">The byte offset from the start of one scanline in the buffer
        /// to the start of the next scanline. If zero, defaults to the size of the specified data type, times
        /// the buffer width.</param>
        public void RasterIO(RWFlag flag, int xOff, int yOff, int xSize, int ySize,
            IntPtr buffer, int bufXSize, int bufYSize, DataType dataType, int pixelStride, int lineStride)
        {
            if (GdalInterOp.GDALRasterIO(hBand, flag, xOff, yOff, xSize, ySize,
                    buffer, bufXSize, bufYSize, dataType, pixelStride, lineStride) != CPLErr.None)
                throw new ApplicationException("Error while reading raster file\n" + GdalInterOp.CPLGetLastErrorMsg());
        }

        /// <summary>
        /// Read or write a block of data from/to a RasterBand. The data type of the buffer is automatically
        /// converted to that of the image file if necessary, and resampling is performed as necessary to
        /// match the buffer dimensions to the requested rectangle. The buffer is passed in as an IntPtr.
        /// </summary>
        /// <param name="flag">Set to RWFlag.Read to read from the file, or RWFlag.Write to write
        /// to the file.</param>
        /// <param name="xOff">The x coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="yOff">The y coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="xSize">The width of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="ySize">The height of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="buffer">A pointer to an in memory buffer.</param>
        /// <param name="bufXSize">The width of the buffer in pixels.</param>
        /// <param name="bufYSize">The height of the buffer in pixels.</param>
        /// <param name="dataType">The type of the data within the buffer.</param>
        public void RasterIO(RWFlag flag, int xOff, int yOff, int xSize, int ySize,
            IntPtr buffer, int bufXSize, int bufYSize, DataType dataType)
        {
			if (GdalInterOp.GDALRasterIO(hBand, flag, xOff, yOff, xSize, ySize,
					buffer, bufXSize, bufYSize, dataType, 0, 0) != CPLErr.None)
			{
				throw new ApplicationException("Error while reading raster file\n" + GdalInterOp.CPLGetLastErrorMsg());
			}
        }

        /// <summary>
        /// Read or write a block of data from/to a RasterBand. The data type of the buffer is automatically
        /// converted to that of the image file if necessary, and resampling is performed as necessary to
        /// match the buffer dimensions to the requested rectangle. The buffer is passed in as an array of bytes.
        /// </summary>
        /// <param name="flag">Set to RWFlag.Read to read from the file, or RWFlag.Write to write
        /// to the file.</param>
        /// <param name="xOff">The x coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="yOff">The y coordinate of the top left corner of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="xSize">The width of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="ySize">The height of the rectangular region of 
        /// the image to be read/written.</param>
        /// <param name="buffer">An array of bytes actings as a buffer.</param>
        /// <param name="bufXSize">The width of the buffer in pixels.</param>
        /// <param name="bufYSize">The height of the buffer in pixels.</param>
        public unsafe void RasterIO(RWFlag flag, int xOff, int yOff, int xSize, int ySize,
            byte[] buffer, int bufXSize, int bufYSize)
        {
            fixed (byte* bufPtr = buffer)
			{
				if (GdalInterOp.GDALRasterIO(hBand, flag, xOff, yOff, xSize, ySize,
						new IntPtr(bufPtr), bufXSize, bufYSize, DataType.Byte, 0, 0) != CPLErr.None)
				{
                     throw new ApplicationException("Error while reading raster file\n" + GdalInterOp.CPLGetLastErrorMsg());
				}
            }
        }

        public unsafe void RasterIO(RWFlag flag, int xOff, int yOff, int xSize, int ySize,
            double[] buffer, int bufXSize, int bufYSize)
        {
            fixed (double* bufPtr = buffer)
            {
                if (GdalInterOp.GDALRasterIO(hBand, flag, xOff, yOff, xSize, ySize,
                        new IntPtr(bufPtr), bufXSize, bufYSize, DataType.Byte, 0, 0) != CPLErr.None)
                {
                    throw new ApplicationException("Error while reading raster file\n" + GdalInterOp.CPLGetLastErrorMsg());
                }
            }
        }

		public unsafe void RasterIO(RWFlag flag, int xOff, int yOff, int xSize, int ySize,
			UInt16[] buffer, int bufXSize, int bufYSize)
		{
			fixed (UInt16* bufPtr = buffer)
			{
				if (GdalInterOp.GDALRasterIO(hBand, flag, xOff, yOff, xSize, ySize,
						new IntPtr(bufPtr), bufXSize, bufYSize, DataType.UInt16, 0, 0) != CPLErr.None)
				{
					throw new ApplicationException("Error while reading raster file\n" + GdalInterOp.CPLGetLastErrorMsg());
				}
			}
		}

        // MC 28/11/06: per gestire correttamente le tif monocromatiche
        public GdalColorTable GetColorTable()
        {
            GdalColorTable ct;
            IntPtr pCT = GdalInterOp.GDALGetRasterColorTable(this.hBand);
            ct = (pCT == null) ? null : new GdalColorTable(pCT);
            return ct;
        }
        // /MC 28/11/06

        // MC 01/08/07: per gestire correttamente le tif con + di 8 bit per pixel
        public GDALDataType GetDataType()
        {
            return GdalInterOp.GDALGetRasterDataType(this.hBand);
        }
        // /MC 01/08/07

		public void SetColorTable(GdalColorTable colorTable)
		{
			CPLErr error;
			if ((error = GdalInterOp.GDALSetRasterColorTable(this.hBand, colorTable.colorTableHandle)) != CPLErr.None)
			{
				string message = "Error setting raster band color table.\n" + GdalInterOp.CPLGetLastErrorMsg();
				throw new GdalException(message);
			}
		}

		public void FillRaster(double value)
		{
			CPLErr error = GdalInterOp.GDALFillRaster(this.hBand, value, 0.0);
			if (error != CPLErr.None)
			{
				string message = "Error filling raster.\n" + GdalInterOp.CPLGetLastErrorMsg();
				throw new GdalException(message);
			}
		}

		public GdalRasterBand GetOverview(int index)
		{
			return (new GdalRasterBand(GdalInterOp.GDALGetOverview(this.hBand, index)));
		}
		
		#endregion
	}
}
