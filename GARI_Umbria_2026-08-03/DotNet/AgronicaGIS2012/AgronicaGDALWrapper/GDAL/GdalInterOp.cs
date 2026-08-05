using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GDALWrapper.Gdal
{
	
	
	
 
    public enum GDALDataType
    {
    GDT_Unknown = 0,
    /*! Eight bit unsigned integer */           GDT_Byte = 1,
    /*! Sixteen bit unsigned integer */         GDT_UInt16 = 2,
    /*! Sixteen bit signed integer */           GDT_Int16 = 3,
    /*! Thirty two bit unsigned integer */      GDT_UInt32 = 4,
    /*! Thirty two bit signed integer */        GDT_Int32 = 5,
    /*! Thirty two bit floating point */        GDT_Float32 = 6,
    /*! Sixty four bit floating point */        GDT_Float64 = 7,
    /*! Complex Int16 */                        GDT_CInt16 = 8,
    /*! Complex Int32 */                        GDT_CInt32 = 9,
    /*! Complex Float32 */                      GDT_CFloat32 = 10,
    /*! Complex Float64 */                      GDT_CFloat64 = 11,
    GDT_TypeCount = 12          /* maximum type # + 1 */
    };

    public enum GDALRWFlag{
    /*! Read data */   GF_Read = 0,
    /*! Write data */  GF_Write = 1
    };

    internal enum CPLErr
    {
        None = 0,
        Debug = 1,
        Warning = 2,
        Failure = 3,
        Fatal = 4
    }

    /// <summary>
    /// Common location for all GDAL InterOp method declarations
    /// </summary>
    internal class GdalInterOp
    {

        static GdalInterOp()
        {
            GDALWrapper.GdalOgr.PrepareGdal();
            GDALAllRegister();
        }

		public const string GdalDllName = "gdal_fw.dll";

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALOpen([MarshalAs(UnmanagedType.LPStr)] string pszFilename, Access eAccess);

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetDatasetDriver(HandleRef hDataset);

        /*
        [DllImport(GdalDllName)]
        internal static extern CPLErr GDALDatasetRasterIO(HandleRef hDataset,
                                                            GDALRWFlag eRWFlag,
                                                            int nXOff,
                                                            int nYOff,
                                                            int nXSize,
                                                            int nYSize,
                                                            byte[] pData,
                                                            int nBufXSize,
                                                            int nBufYSize,
                                                            GDALDataType eBufType,
                                                            int nBandCount,
                                                            int panBandMap,
                                                            int nPixelSpace,
                                                            int nLineSpace,
                                                            int nBandSpace
                                                        ); 	
        */

        [DllImport(GdalDllName)]
        internal static extern int GDALGetRasterXSize(HandleRef hDataset);

        [DllImport(GdalDllName)]
        internal static extern int GDALGetRasterYSize(HandleRef hDataset);

        [DllImport(GdalDllName)]
        internal static extern int GDALGetRasterCount(HandleRef hDataset);

        [DllImport(GdalDllName, CharSet = CharSet.Ansi, EntryPoint = "GDALGetProjectionRef")]
        private static extern IntPtr GDALGetProjectionRefExt(HandleRef hDataset);

        internal static string GDALGetProjectionRef(HandleRef hDataset)
        {
            return Marshal.PtrToStringAnsi(GDALGetProjectionRefExt(hDataset));
        }

        [DllImport(GdalDllName)]
        internal static extern CPLErr GDALGetGeoTransform(HandleRef hDataset, [In, Out] double[] Transform);

        [DllImport(GdalDllName)]
        internal static extern CPLErr GDALSetGeoTransform(HandleRef hDataset, [In, Out] double[] Transform);


        [DllImport(GdalDllName)]
        internal static extern void GDALClose(HandleRef hDriver);

        [DllImport(GdalDllName, EntryPoint = "CPLGetLastErrorMsg")]
        private static extern IntPtr CPLGetLastErrorMsgExt();

        internal static string CPLGetLastErrorMsg()
        {
            return Marshal.PtrToStringAnsi(CPLGetLastErrorMsgExt());
        }

        [DllImport(GdalDllName)]
        internal static extern void GDALAllRegister();

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALCreateCopy(HandleRef hDriver, [MarshalAs(UnmanagedType.LPStr)] string filename,
            HandleRef hDataset, int bStrict, HandleRef options, HandleRef progressFn, HandleRef progressData);

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALCreate(HandleRef hDriver, [MarshalAs(UnmanagedType.LPStr)] string filename,
            int width, int height, int nBands, DataType type, HandleRef options);

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetDriverByName([MarshalAs(UnmanagedType.LPStr)]string pszName);


        [DllImport(GdalDllName)]
        internal static extern int GDALGetDriverCount();

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetDriver(int iDriver);

        [DllImport(GdalDllName, CharSet = CharSet.Ansi, EntryPoint = "GDALGetDriverShortName")]
        private static extern IntPtr GDALGetDatasetDriverExt(HandleRef hDataset);

        internal static string GDALGetDriverShortName(HandleRef hDriver)
        {
            return Marshal.PtrToStringAnsi(GDALGetDatasetDriverExt(hDriver));
        }

        [DllImport(GdalDllName, CharSet = CharSet.Ansi, EntryPoint = "GDALGetDriverLongName")]
        private static extern IntPtr GDALGetDriverLongNameExt(HandleRef hDriver);

        internal static string GDALGetDriverLongName(HandleRef hDriver)
        {
            return Marshal.PtrToStringAnsi(GDALGetDriverLongNameExt(hDriver));
        }

        [DllImport(GdalDllName, CharSet = CharSet.Ansi, EntryPoint = "GDALGetDescription")]
        private static extern IntPtr GDALGetDescriptionExt(HandleRef hDriver);

        internal static string GDALGetDescription(HandleRef hDriver)
        {
            return Marshal.PtrToStringAnsi(GDALGetDescriptionExt(hDriver));
        }

        [DllImport(GdalDllName)]
        internal static extern void GDALDestroyDriverManager();

        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetRasterBand(HandleRef hDataset, int index);
        
        [DllImport(GdalDllName)]
        internal static extern int GDALGetOverviewCount(HandleRef hBand);
        
        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetOverview(HandleRef hBand, int index);
        
        [DllImport(GdalDllName)]
        internal static extern CPLErr GDALRasterIO(HandleRef hBand, RWFlag flag, int xOff, int yOff,
            int xSize, int ySize, IntPtr data, int bufXSize, int bufYSize, DataType type, int pixelStride,
            int lineStride);

        [DllImport(GdalDllName)]
        internal static extern CPLErr GDALDatasetRasterIO( HandleRef hDS, RWFlag RWFlag,
		int	nXOff, int nYOff, int nXSize, int nYSize, IntPtr Data, int bufXSize, int bufYSize, DataType type, int nBandCount,
		ref int panBandMap, int nPixelSpace, int nLineSpace, int nBandSpace);  
        
        
        [DllImport(GdalDllName)]
        internal static extern int GDALGetRasterColorInterpretation(HandleRef hBand);

        [DllImport(GdalDllName)]
        internal static extern int GDALGetAccess(HandleRef hDataset);

        [DllImport(GdalDllName, EntryPoint="GDALGetMetadata")]
        private static extern IntPtr GDALGetMetadataExt(HandleRef hDataset,
            [MarshalAs(UnmanagedType.LPStr)] string domain);

        internal unsafe static Dictionary<string, string> GDALGetMetadata(HandleRef hDataset, string domain)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            void** stringList = (void**) GDALGetMetadataExt(hDataset, domain);
            if (new IntPtr(stringList) != IntPtr.Zero) 
			{
                while (new IntPtr(*stringList) != IntPtr.Zero) 
				{
                    string keyValue = Marshal.PtrToStringAnsi(new IntPtr(*stringList));
                    string[] elements = keyValue.Split("=".ToCharArray());
					if (elements.Length < 2)
					{
						throw new ApplicationException("Parse error while parsing string list in GDALGetMetadata()");
					}
                    data.Add(elements[0], elements[1]);
                    ++stringList;
                }
            }
            return data;
        }

        [DllImport(GdalDllName, EntryPoint="GDALGetMetadataItem")]
        private static extern IntPtr GDALGetMetadataItemExt(HandleRef hDataset,
            [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string domain);

        internal static string GDALGetMetadataItem(HandleRef hDataset, string key, string domain)
        {
            return Marshal.PtrToStringAnsi(GDALGetMetadataItemExt(hDataset, key, domain));
        }

        [DllImport(GdalDllName)]
        internal static extern void GDALSetMetadataItem(HandleRef hDataset,
            [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value,
            [MarshalAs(UnmanagedType.LPStr)] string domain);

		[DllImport(GdalDllName)]
		internal static extern CPLErr GDALSetProjection(HandleRef hDS, string pszProjection);

		[DllImport(GdalDllName)]
		internal static extern IntPtr GDALCreateColorTable(GDALPaletteInterp palletteInterp);

		[DllImport(GdalDllName)]
		internal static extern void GDALDestroyColorTable(HandleRef hCT);

		[DllImport(GdalDllName)]
		internal static extern void GDALSetColorEntry(HandleRef hCT, int offset, ref GDALColorEntry colorEntry);

        // MC 28/11/06: per gestire correttamente le tif monocromatiche
        [DllImport(GdalDllName)]
        internal static extern IntPtr GDALGetRasterColorTable(HandleRef hBand);

        [DllImport(GdalDllName)]
        internal static extern int GDALGetColorEntryCount(HandleRef hColorTable); 
        // /MC 28/11/06

		[DllImport(GdalDllName)]
		internal static extern CPLErr GDALSetRasterColorTable(HandleRef hBand, HandleRef hCT);

		[DllImport(GdalDllName)]
		internal static extern double GDALGetRasterOffset(HandleRef hBand, IntPtr pbSuccess);

		[DllImport(GdalDllName)]
		internal static extern CPLErr GDALSetRasterOffset(HandleRef hBand, double dfNewOffset);

		[DllImport(GdalDllName)]
		internal static extern double GDALGetRasterScale(HandleRef hBand, IntPtr pbSuccess);

		[DllImport(GdalDllName)]
		internal static extern CPLErr GDALSetRasterScale(HandleRef hBand, double dfNewScale);

		[DllImport(GdalDllName)]
		internal static extern CPLErr GDALFillRaster(HandleRef hBand, double dfRealValue, double dfImaginaryValue);

        // MC 01/08/07: per gestire correttamente le tif con + di 8 bit per pixel
        [DllImport(GdalDllName)]
        internal static extern GDALDataType GDALGetRasterDataType(HandleRef hBand);
        // /MC 01/08/07
    
	}
    
}
