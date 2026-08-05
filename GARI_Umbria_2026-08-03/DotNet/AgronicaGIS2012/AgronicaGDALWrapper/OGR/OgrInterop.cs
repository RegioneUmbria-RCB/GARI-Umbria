using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// error enum of OGR
	/// </summary>
	public enum OGRErr
	{
		/// <summary>
		/// no error
		/// </summary>
		None = 0,
		NotEnoughDataToDeserialize,
		NotEnoughMemory,
		UnsupportedGeometryType,
		UnsupportedOperation,
		CorruptData,
		Failure,
		UnsupportedSRS
	};
	
	/// <summary>
	/// Byte order
	/// </summary>
	public enum OgrWkbByteOrder
	{
    	/// <summary>
    	/// MSB/Sun/Motoroloa: Most Significant Byte First
    	/// </summary>
		wkbXDR = 0,        
		/// <summary>
		/// LSB/Intel/Vax: Least Significant Byte First
		/// </summary>
    	wkbNDR = 1     
	};

	internal class OgrInterop
	{
		
		[ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Ansi )]
		public struct StringStruct 
		{  
		   public String buffer;
		   public int size;
		}
		
		static OgrInterop()
		{
			GDALWrapper.GdalOgr.PrepareGdal();;
            OGRRegisterAll();
		}
		
		#region methods for common tasks

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGRGetDriver(int iDriver);
	
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGRGetDriverCount();

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGROpen(string pszName, int bUpdate,
			HandleRef pahDriverList);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGRRegisterAll();

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGRReleaseDataSource(HandleRef hDS);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal unsafe static extern void VSIFree(char* pData);

		#endregion

		#region methods for OgrCoordinateTransformation (OCT...)
	
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OCTDestroyCoordinateTransformation(HandleRef hCT);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OCTNewCoordinateTransformation(HandleRef hSourceSRS,
			HandleRef hTargetSRS);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern Int32 OCTTransform(HandleRef hCT, int nCount, [In, Out] double[] x,
			[In, Out] double[] y, [In, Out] double[] z);
	
		#endregion

		#region methods for OgrDriver (OGR_Dr_...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGR_Dr_CreateDataSource(HandleRef hDriver,
			string pszName, ref string[] papszOptions);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_Dr_DeleteDataSource(HandleRef hDriver);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern string OGR_Dr_GetName(HandleRef hDriver);
			
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_Dr_TestCapability(HandleRef hDriver, string sCap);

		#endregion

		#region methods for OgrSpatialReference (OSR...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OSRClone(HandleRef hSRS);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OSRDereference(HandleRef hSRS);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal unsafe static extern void OSRExportToWkt(HandleRef hSRS, char** pszSRS_WKT);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OSRGetUTMZone(HandleRef hSRS, out int pbNorth);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OSRNewSpatialReference(string pszWKT);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern OGRErr OSRSetProjCS(HandleRef hSpatRef, string pszCS);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OSRSetUTM(HandleRef hSRS, int nZone, int bNorth);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern OGRErr OSRSetWellKnownGeogCS(HandleRef hSRS, string pszName);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OSRValidate(HandleRef hSRS);

		#endregion			
		
		#region methods for OgrDatasource (OGR_DS_...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGR_DS_CreateLayer(HandleRef hDS,
			string pszName, HandleRef hSpatialRef, OGRwkbGeometryType eType,
			ref string[] papszOptions);

		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_DS_GetLayer(HandleRef hDS, int iLayer);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_DS_GetLayerByName(HandleRef hDS, string sLayerName);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_DS_GetLayerCount(HandleRef hDS);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern string OGR_DS_GetName(HandleRef hDS);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_DS_TestCapability(HandleRef hDS, string sCapability);
		
		#endregion
		
		#region methods for OgrFeature (OGR_F_...)

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_F_Create(HandleRef hDefn);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_F_Destroy(HandleRef hFeat);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern long OGR_F_GetFID(HandleRef hFeat);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_F_GetFieldAsDouble(HandleRef hFeat, int iField);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_F_GetFieldAsInteger(HandleRef hFeat, int iField);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern string OGR_F_GetFieldAsString(HandleRef hFeat, int iField);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_F_GetFieldCount(HandleRef hFeat);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_F_GetGeometryRef(HandleRef hFeat);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_F_SetFID(HandleRef hFeat, long nFID);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_F_SetFieldDouble(HandleRef hFeat,
			int iField, double dfValue);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_F_SetFieldInteger(HandleRef hFeat,
			int iField, int nValue);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern void OGR_F_SetFieldString(HandleRef hFeat,
			int iField, string pszValue);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_F_SetGeometryDirectly(HandleRef hFeat,
			HandleRef hGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern void OGR_F_SetStyleString(HandleRef hFeat,
			string pszStyle);

		#endregion		
		
		#region methods for OgrFeatureDefinition (OGR_FD_...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGR_FD_Create(string pszName);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_FD_GetFieldCount(HandleRef hDefn);

        [DllImport(Gdal.GdalInterOp.GdalDllName)]
        internal static extern IntPtr OGR_FD_GetFieldDefn(HandleRef hDefn, int iField);
        
        [DllImport(Gdal.GdalInterOp.GdalDllName)]
        internal static extern OGRwkbGeometryType OGR_FD_GetGeomType(HandleRef hDefn);
        
        [DllImport(Gdal.GdalInterOp.GdalDllName)]
        internal static extern void OGR_FD_SetGeomType(HandleRef hDefn, OGRwkbGeometryType eType);

		#endregion
		
		#region methods for OgrFieldDefinition (OGR_Fld_...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGR_Fld_Create(string pszName, OGRFieldType eType);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Ansi)]
		internal static extern IntPtr OGR_Fld_Destroy(HandleRef hDefn);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRFieldType OGR_Fld_GetType(HandleRef hDefn);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_Fld_GetNameRef(HandleRef hDefn);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_Fld_GetPrecision(HandleRef hDefn);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OgrJustification OGR_Fld_GetJustify(HandleRef hDefn);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_Fld_GetWidth(HandleRef hDefn);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_Fld_SetJustify(HandleRef hDefn, OgrJustification eJustify);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_Fld_SetPrecision(HandleRef hDefn, int nPrecision);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_Fld_SetWidth(HandleRef hDefn, int nNewWidth);

		#endregion
		
		#region methods for OgrGeometries (OGR_G_...)
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_G_AddGeometryDirectly(HandleRef hGeom,
			HandleRef hNewSubGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_G_AddPoint_2D(HandleRef hGeom, double dFX, double dFY);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_Buffer(HandleRef hGeom, double dDistance, int nQuadSegs );
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_Clone(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Contains(HandleRef hGeom, HandleRef hOtherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_ConvexHull(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_G_CreateFromWkt(ref string WktData, HandleRef hSRS ,HandleRef hGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_CreateGeometry(OGRwkbGeometryType eType);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Crosses(HandleRef hGeom, HandleRef hOtherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_Difference(HandleRef hGeom, HandleRef hOtherGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Disjoint(HandleRef hGeom, HandleRef hOtherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_G_Distance(HandleRef hGeom, HandleRef hOtherGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_G_Empty(HandleRef hGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Equals(HandleRef hGeom, HandleRef hOtherGeom);

        [DllImport(Gdal.GdalInterOp.GdalDllName, SetLastError = true, CharSet = CharSet.Auto)]
        internal unsafe static extern OGRErr OGR_G_ExportToWkb(HandleRef hGeom, OgrWkbByteOrder byteOrder, byte[] bWkb);

        [DllImport(Gdal.GdalInterOp.GdalDllName, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern OGRErr OGR_G_ExportToWkt(HandleRef hGeom, ref String sbWkt);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_G_GetArea(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_GetBoundary(HandleRef hGeom);
        
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_GetCoordinateDimension(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_GetDimension(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_G_GetEnvelope(HandleRef hGeom, ref StructOgrEnvelope structOgrEnvelop);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_GetGeometryCount(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern string OGR_G_GetGeometryName(HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_GetGeometryRef(HandleRef hGeom, int iSubGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern uint OGR_G_GetGeometryType(HandleRef hGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_G_GetPoint(HandleRef hGeom, int i,
			out double pdfX, out double pdfY, out double pdfZ);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_GetPointCount(HandleRef hGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_G_GetX(HandleRef hGeom, int i);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_G_GetY(HandleRef hGeom, int i);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern double OGR_G_GetZ(HandleRef hGeom, int i);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_Intersection(HandleRef hGeom, HandleRef otherGeom);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Intersects(HandleRef hGeom, HandleRef hOtherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_G_ImportFromWkb(HandleRef hGeom, [MarshalAs(UnmanagedType.LPArray)]byte[] bWkb, int nSize);
  		
		[DllImport(Gdal.GdalInterOp.GdalDllName, CharSet = CharSet.Auto)]
		internal static extern OGRErr OGR_G_ImportFromWkt(HandleRef hGeom, ref string WktData);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Overlaps(HandleRef hGeom, HandleRef hOtherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_G_RemoveGeometry(HandleRef hGeom, int iGeom, int bDelete);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_SymmetricDifference(HandleRef hGeom, HandleRef otherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Touches(HandleRef hGeom, HandleRef otherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_G_Union(HandleRef hGeom, HandleRef otherGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_G_Within(HandleRef hGeom, HandleRef otherGeom);
				
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal unsafe static extern int OGR_G_WkbSize(HandleRef hGeom);
		
		#endregion	
		
		#region methods for OgrLayer (OGR_L_...)

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_L_CreateFeature(HandleRef hLayer, HandleRef hFeature);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_L_CreateField(HandleRef hLayer,
			HandleRef hField, int bApproxOK);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_L_DeleteFeature(HandleRef hLayer, long nFID);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetExtent(HandleRef hLayer, ref StructOgrEnvelope strOgrEnvelop, int bForce);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetFeature(HandleRef hLayer, long nFeatureId);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_L_GetFeatureCount(HandleRef hLayer, int bForce);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetLayerDefn(HandleRef hLayer);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetSpatialFilter(HandleRef hLayer);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetNextFeature(HandleRef hLayer);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern IntPtr OGR_L_GetSpatialRef(HandleRef hLayer);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_L_ResetReading(HandleRef hLayer);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern OGRErr OGR_L_SetAttributeFilter(HandleRef hLayer, string Query);

		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_L_SetSpatialFilter(HandleRef hLayer, HandleRef hGeom);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern void OGR_L_SetSpatialFilterRect(HandleRef hLayer, double dLeft, double dBottom, double dRight, double dTop);
		
		[DllImport(Gdal.GdalInterOp.GdalDllName)]
		internal static extern int OGR_L_TestCapability(HandleRef hLayer, string sCap);

		#endregion
	}
}
