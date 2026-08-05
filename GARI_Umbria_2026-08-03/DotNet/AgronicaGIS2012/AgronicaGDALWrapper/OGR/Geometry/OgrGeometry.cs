using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// type of geometry
	/// </summary>
	public enum OGRwkbGeometryType :uint
	{
		/// <summary>
		/// non-standard
		/// </summary>
		wkbUnknown = 0,	
		/// <summary>
		/// point geometry
		/// </summary>
		wkbPoint = 1,	
		/// <summary>
		/// linestring geometry
		/// </summary>
		wkbLineString = 2,
		/// <summary>
		/// polygon geometry
		/// </summary>
		wkbPolygon = 3,
		/// <summary>
		/// multipoint geometry
		/// </summary>
		wkbMultiPoint = 4,
		/// <summary>
		/// multilinestring geometry
		/// </summary>
		wkbMultiLineString = 5,
		/// <summary>
		/// multipolygon geometry
		/// </summary>
		wkbMultiPolygon = 6,
		/// <summary>
		/// geometrycollection geometry
		/// </summary>
		wkbGeometryCollection = 7,
		/// <summary>
		/// none geometry
		/// </summary>
		wkbNone = 100,		
		/// <summary>
		/// non-standard, just for createGeometry()
		/// </summary>
		wkbLinearRing = 101,
		
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbPoint25D = (uint)0x80000001,		
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbLineString25D = (uint)0x80000002,
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbPolygon25D = (uint)0x80000003,
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbMultiPoint25D = (uint)0x80000004,
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbMultiLineString25D = (uint)0x80000005,
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbMultiPolygon25D = (uint)0x80000006,
		/// <summary>
		/// 2.5D extensions as per 99-402
		/// </summary>
		wkbGeometryCollection25D = (uint)0x80000007
	};

	/// <summary>
	/// OgrDimension
	/// </summary>
	public enum OgrDimension :int
	{
		/// <summary>
		/// point type
		/// </summary>
		point = 0,
		/// <summary>
		/// line type
		/// </summary>
		line = 1,
		/// <summary>
		/// surface type
		/// </summary>
		surface = 2
	};
	
	/// <summary>
	/// The base geometry class. Since the OGR wrapper classes only store handle references to 
	/// the unmanaged objects, this abstract class is not abstract in the wrapper, to enable
	/// instantiation of any base geometry types. This is necessary in a case such as determining
	/// the geometry type from a geometry list.
	/// </summary>
	public class OgrGeometry
	{
		internal HandleRef geometryHandle;

		/// <summary>
		/// In a sense this converts all Z coordinates to 0.0.
		/// </summary>
		public const uint wkbFlattenMask = 0x80000000;

		#region public properties

		/// <summary>
		/// The number of points in the geometry.
		/// </summary>
		/// 
		public int NumberPoints
		{
			get { return OgrInterop.OGR_G_GetPointCount(this.geometryHandle); }
		}

		#endregion

		#region constructors

		internal OgrGeometry(IntPtr ptrGeometry)
		{
			this.geometryHandle = new HandleRef(this, ptrGeometry);
		}

		/// <summary>
		/// constructor of OgrGeometry
		/// </summary>
		/// <param name="geometryType">type of geometry</param>
		public OgrGeometry(OGRwkbGeometryType geometryType)
		{
			this.geometryHandle = new HandleRef(this, OgrInterop.OGR_G_CreateGeometry(geometryType));
		}

		internal OgrGeometry(IntPtr ptrGeometry, OGRwkbGeometryType geometryType)
		{
			this.geometryHandle = new HandleRef(this, ptrGeometry);
		}

		#endregion

		#region public methods

		/// <summary>
		/// Compute boundary.
		/// </summary>
		/// <returns>a newly allocated geometry, or NULL on failure.</returns>
		public OgrGeometry GetBoundary()
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_GetBoundary(this.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}
		
		/// <summary>
		/// Compute convex hull.
		/// </summary>
		/// <returns>a newly allocated geometry now owned by the caller, or NULL on failure.</returns>
		public OgrGeometry ConvexHull()
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_ConvexHull(this.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}		
		
		/// <summary>
		/// Compute buffer of geometry.
		/// </summary>
		/// <returns>a newly allocated geometry now owned by the caller, or NULL on failure.</returns>
		public OgrGeometry Buffer(double Distance, int QuadSegs)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_Buffer(this.geometryHandle, Distance, QuadSegs);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}		
		
		/// <summary>
		/// Compute intersection.
		/// </summary>
		/// <param name="OtherGeom">the other geometry intersected with "this" geometry.</param>
		/// <returns>a new geometry representing the intersection or NULL if there is no intersection or an error occurs.</returns>
		public OgrGeometry Intersection(OgrGeometry OtherGeom)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_Intersection(this.geometryHandle, OtherGeom.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}		
		
		/// <summary>
		/// Compute union.
		/// <remarks>Generates a new geometry which is the region of union of the two geometries operated on.</remarks>
		/// </summary>
		/// <param name="OtherGeom">the other geometry unioned with "this" geometry.</param>
		/// <returns>a new geometry representing the union or NULL if an error occurs.</returns>
		public OgrGeometry Union(OgrGeometry OtherGeom)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_Union(this.geometryHandle, OtherGeom.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}		
		
		/// <summary>
		/// Compute difference.
		/// </summary>
		/// <remarks>Generates a new geometry which is the region of this geometry with the region of the second geometry removed.</remarks>
		/// <param name="OtherGeom">the other geometry removed from "this" geometry.</param>
		/// <returns>a new geometry representing the difference or NULL if the difference is empty or an error occurs.</returns>
		public OgrGeometry Difference (OgrGeometry OtherGeom)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_Difference (this.geometryHandle, OtherGeom.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}		
		
		/// <summary>
		/// Compute symmetric difference.
		/// </summary>
		/// <remarks>Generates a new geometry which is the symmetric difference of this geometry and the second geometry passed into the method.</remarks>
		/// <param name="OtherGeom">the other geometry.</param>
		/// <returns>a new geometry representing the symmetric difference or NULL if the difference is empty or an error occurs.</returns>
		public OgrGeometry SymmetricDifference  (OgrGeometry OtherGeom)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_SymmetricDifference  (this.geometryHandle, OtherGeom.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;
		}
	
		/// <summary>
		/// Compute distance between two geometries.
		/// </summary>
		/// <remarks>Returns the shortest distance between the two geometries.</remarks>
		/// <param name="OtherGeom">the other geometry.</param>
		/// <returns>the distance between the geometries or -1 if an error occurs.</returns>
		public double Distance (OgrGeometry OtherGeom)
		{
			return OgrInterop.OGR_G_Distance (this.geometryHandle, OtherGeom.geometryHandle);
		}
		
		/// <summary>
		/// Computes and returns the bounding envelope for this geometry in the passed psEnvelope structure.
		/// </summary>
		/// <returns>a Minimum Bounding rect of the geometry</returns>
		public OgrEnvelope GetEnvelope()
		{
			StructOgrEnvelope structOgrEnvelope = new StructOgrEnvelope();
						
			OgrInterop.OGR_G_GetEnvelope(this.geometryHandle, ref structOgrEnvelope);

			return new OgrEnvelope(structOgrEnvelope.MinX,
			                       structOgrEnvelope.MinY,
			                       structOgrEnvelope.MaxX,
			                       structOgrEnvelope.MaxY);
		}	
		
		
		/// <summary>
		/// Fetch geometry type.
		/// </summary>
		/// <param name="flatten">Flattens any 2.5D geometry types to 2D.</param>
		/// <returns>Geometry type</returns>
		public OGRwkbGeometryType GetGeometryType(bool flatten)
		{
			OGRwkbGeometryType geoType = (OGRwkbGeometryType)(OgrInterop.OGR_G_GetGeometryType(this.geometryHandle));

			if (flatten && (((uint)geoType & OgrGeometry.wkbFlattenMask) > 0))
			{
				// flatten any 2.5D types
				geoType = (OGRwkbGeometryType)((uint)geoType - OgrGeometry.wkbFlattenMask);
			}
			return geoType;
		}

		/// <summary>
		/// Fetch a point in line string or a point geometry.
		/// </summary>
		/// <param name="pointIndex">The vertex to fetch, from 0 to NumPoints-1, zero for a point.</param>
		/// <param name="x">Value of x coordinate</param>
		/// <param name="y">Value of y coordinate</param>
		/// <param name="z">Value of z coordinate</param>
		public void GetPoint(int pointIndex, out double x, out double y, out double z)
		{
			OgrInterop.OGR_G_GetPoint(this.geometryHandle, pointIndex, out x, out y, out z);
		}

		/// <summary>
		/// Fetch the number of elements in a geometry.
		/// </summary>
		/// <returns>the number of elements.</returns>
		public int GetGeometryCount()
		{
			return (int)OgrInterop.OGR_G_GetGeometryCount(this.geometryHandle);
		}
		
		/// <summary>
		/// Fetch WKT name for geometry type.
		/// </summary>
		/// <returns>name used for this geometry type in well known text format.</returns>
		public string GetGeometryName()
		{
			return OgrInterop.OGR_G_GetGeometryName(this.geometryHandle);
		}

		/// <summary>
		/// Returns size of related binary representation.
		/// </summary>
		/// <returns>size of binary representation in bytes.</returns>
		public unsafe int WkbSize()
		{
			return (int)OgrInterop.OGR_G_WkbSize(this.geometryHandle);
		}

		/// <summary>
		/// Get the dimension of this geometry.
		/// </summary>
		/// <returns>0 for points, 1 for lines and 2 for surfaces.</returns>
		public OgrDimension GetDimension()
		{
			return (OgrDimension)OgrInterop.OGR_G_GetDimension(this.geometryHandle);
		}

		/// <summary>
		/// Get the dimension of the coordinates in this geometry.
		/// </summary>
		/// <returns>in practice this always returns 2 indicating that coordinates are specified within a two dimensional space.</returns>
		public int GetCoordinateDimension()
		{
			return (int)OgrInterop.OGR_G_GetCoordinateDimension(this.geometryHandle);
		}
		
		
		/// <summary>
		/// Do these features intersect?
		/// </summary>
		/// <param name="otherGeom"></param>
		/// <returns>TRUE if the geometries intersect, otherwise FALSE.</returns>
		public bool Intersects(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Intersects(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;
		}
		
		/// <summary>
		/// Returns two if two geometries are equivalent.
		/// </summary>
		/// <param name="otherGeom"></param>
		/// <returns>TRUE if equivalent or FALSE otherwise.</returns>
		public bool Equals(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Equals(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;
		}
		
		/// <summary>
		/// Clear geometry information. This restores the geometry to it's initial state after construction, and before assignment of actual geometry.
		/// </summary>
		public void Empty()
		{
			OgrInterop.OGR_G_Empty(this.geometryHandle);
		}
		
		/// <summary>
		/// Make a copy of this object.
		/// </summary>
		/// <param name="Geom"></param>
		/// <returns>copy of the geometry with the spatial reference system as the original.</returns>
		public OgrGeometry Clone(OgrGeometry Geom)
		{
			IntPtr ptrGeom = OgrInterop.OGR_G_Clone(Geom.geometryHandle);
			
			if (ptrGeom != IntPtr.Zero)
			{
				return new OgrGeometry(ptrGeom);
			}
			return null;		
		}
		
		/// <summary>
		/// Fetch the X coordinate of a point from a geometry.
		/// </summary>
		/// <param name="i">point to get the X coordinate.</param>
		/// <returns>the X coordinate of this point.</returns>
		public double GetX(int i)
		{
			return OgrInterop.OGR_G_GetX(this.geometryHandle, i);
		}
		
		/// <summary>
		/// Fetch the Y coordinate of a point from a geometry.
		/// </summary>
		/// <param name="i">point to get the Y coordinate.</param>
		/// <returns>the Y coordinate of this point.</returns>
		public double GetY(int i)
		{
			return OgrInterop.OGR_G_GetY(this.geometryHandle, i);
		}
		
		/// <summary>
		/// Fetch the Z coordinate of a point from a geometry.
		/// </summary>
		/// <param name="i">point to get the Z coordinate.</param>
		/// <returns>the Z coordinate of this point.</returns>
		public double GetZ(int i)
		{
			return OgrInterop.OGR_G_GetZ(this.geometryHandle, i);
		}
			
		/// <summary>
		/// Create a geometry object of the appropriate type from it's well known text representation.
		/// </summary>
		/// <param name="WktData">string containing well known text representation of the geometry to be created.</param>
		/// <param name="OgrSRS">the spatial reference to be assigned to the created geometry object. This may be NULL.</param>
		/// <returns>OGRERR_NONE if all goes well, otherwise any of OGRERR_NOT_ENOUGH_DATA, OGRERR_UNSUPPORTED_GEOMETRY_TYPE, or OGRERR_CORRUPT_DATA may be returned.</returns>
		public void CreateFromWkt(string WktData, OgrSpatialReference OgrSRS)
		{
			OGRErr error;
			if (OgrSRS != null)
				error = OgrInterop.OGR_G_CreateFromWkt(ref WktData, OgrSRS.spatialReferenceHandle , this.geometryHandle);
			else //		error = OGRErr.Failure;
			{
				HandleRef NullRef = new HandleRef(null, IntPtr.Zero);
				error = OgrInterop.OGR_G_CreateFromWkt(ref WktData, NullRef, this.geometryHandle);
				
			}
			
			if(error != OGRErr.None)
			{
				string message = "Could not create geometry from WKT. " + error.ToString();
				throw new OgrException(message);
			}

		}
				
		/// <summary>
		/// Remove a geometry from an exiting geometry container.
		/// </summary>
		/// <remarks>
		/// Removing a geometry will cause the geometry count to drop by one, and all "higher" geometries will shuffle down one in index.
		/// </remarks>
		/// <param name="iGeom">the index of the geometry to delete. A value of -1 is a special flag meaning that all geometries should be removed.</param>
		/// <param name="bDelete">if TRUE the geometry will be destroyed, otherwise it will not. The default is TRUE as the existing geometry is considered to own the geometries in it.</param>
		public void RemoveGeometry(int iGeom, bool bDelete)
		{
			OGRErr _OgrErr;
			
			if(bDelete)
				_OgrErr = OgrInterop.OGR_G_RemoveGeometry(this.geometryHandle, iGeom, 1);
			else
				_OgrErr = OgrInterop.OGR_G_RemoveGeometry(this.geometryHandle, iGeom, 0);			
		}
		
		/// <summary>
		/// Convert a geometry into well known text format.
		/// </summary>
		/// <returns>WKT representation of the geometry</returns>
		public string ExportToWkt()
		{
			StringBuilder sbWkt = new StringBuilder(1024);
            String sWkt = null;
            OGRErr error = OgrInterop.OGR_G_ExportToWkt(this.geometryHandle, ref sWkt);
			
			if(error != OGRErr.None)
			{
				string message = "Could not create Wkt from geometry. " + error.ToString();
				throw new OgrException(message);
			}
			
			return sWkt;
		}
		
		/// <summary>
		/// Convert a geometry into well known binary format.
		/// </summary>
		/// <returns>WKB representation of the geometry</returns>
		public byte[] ExportToWkb()
		{
            int iWkbSize = (int)OgrInterop.OGR_G_WkbSize(this.geometryHandle);
			//char wkbNamePtr;			
			
            StringBuilder sbWkb = new StringBuilder(1024);
            byte[] bWkb = new byte[iWkbSize];

			OGRErr error = OgrInterop.OGR_G_ExportToWkb(this.geometryHandle, OgrWkbByteOrder.wkbNDR, bWkb);
			
			if(error != OGRErr.None)
			{
				string message = "Could not create Wkb from geometry. " + error.ToString();
				throw new OgrException(message);
			}

            return bWkb;
		}
		
		/// <summary>
		/// Returns true if two geometries disjoint.
		/// </summary>
		/// <param name="otherGeom">the geometry to compare to this geometry.</param>
		/// <returns>TRUE if disjoint or FALSE otherwise.</returns>
		public bool Disjoint(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Disjoint(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}		
		
		/// <summary>
		/// Returns true if two geometries touches.
		/// </summary>
		/// <param name="otherGeom">the geometry to compare to this geometry.</param>
		/// <returns>TRUE if touches or FALSE otherwise.</returns>
		public bool Touches(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Touches(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}		
		
		/// <summary>
		/// Returns true if two geometries crosses.
		/// </summary>
		/// <param name="otherGeom">the geometry to compare to this geometry.</param>
		/// <returns>TRUE if they are crossing, otherwise FALSE.</returns>
		public bool Crosses(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Crosses(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}
		
		/// <summary>
		/// Tests if the passed in geometry is within the target geometry.
		/// </summary>
		/// <param name="otherGeom"></param>
		/// <returns>TRUE if they are crossing, otherwise FALSE.</returns>
		public bool Within(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Within(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}
			
		/// <summary>
		/// Tests if the passed in geometry contains the target geometry.
		/// </summary>
		/// <param name="otherGeom">the geometry to compare to this geometry.</param>
		/// <returns>TRUE if otherGeom contains this geometry, otherwise FALSE.</returns>
		public bool Contains(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Contains(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}
		
		/// <summary>
		/// Tests if this geometry and the other passed into the method overlap, that is their intersection has a non-zero area.
		/// </summary>
		/// <param name="otherGeom">the geometry to compare to this geometry.</param>
		/// <returns>TRUE if they are overlapping, otherwise FALSE.</returns>
		public bool Overlaps(OgrGeometry otherGeom)
		{
			if (OgrInterop.OGR_G_Overlaps(this.geometryHandle, otherGeom.geometryHandle)==0)
				return false;
			else 
				return true;	
		}
		
		/// <summary>
		/// Assign geometry from well known text data.
		/// </summary>
		/// <param name="WktData">the source text</param>
		public void ImportFromWkt(string WktData)
		{
			OGRErr error = OgrInterop.OGR_G_ImportFromWkt(this.geometryHandle, ref WktData);
			
			if(error != OGRErr.None)
			{
				string message = "Could not create geometry from WKT. " + error.ToString();
				throw new OgrException(message);
			}
		}		
		
		/// <summary>
		/// Assign geometry from well known binary data.
		/// </summary>
		/// <param name="bWkbData">the source text</param>
		public void ImportFromWkb(byte[] bWkbData)
		{			
			OGRErr error = OgrInterop.OGR_G_ImportFromWkb(this.geometryHandle, bWkbData, bWkbData.Length);
			
			if(error != OGRErr.None)
			{
				string message = "Could not create geometry from WKB. " + error.ToString();
				throw new OgrException(message);
			}
		}
		#endregion
	}
}
