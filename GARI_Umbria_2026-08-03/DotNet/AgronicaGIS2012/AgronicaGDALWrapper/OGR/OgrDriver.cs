using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDALWrapper.OGR
{
	/// <summary>
	/// Represents an operational format driver.
	/// </summary>
	public class OgrDriver
	{
		internal HandleRef driverHandle;

		#region public properties

		/// <summary>
		/// Name of actual used driver
		/// </summary>
		public string Name
		{
			get
			{
				return OgrInterop.OGR_Dr_GetName(this.driverHandle);
			}
		}

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new OgrDriver object. Does not initiate the driver handle,
		/// so the OpenDataSource() method must be called prior to using.
		/// </summary>
		public OgrDriver()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="driverIndex">The driver index, from 0 to (GetDriverCount - 1).</param>
		public OgrDriver(int driverIndex)
        {
			
			driverHandle = new HandleRef(this, OgrInterop.OGRGetDriver(driverIndex));
		}

		#endregion

		#region public methods

		/// <summary>
		/// This function attempts to create a new data source based on the passed driver.
		/// </summary>
		/// <param name="folderName">
		/// The folder in which the datasource output will be created.
		/// </param>
		/// <param name="options">
		/// The options argument can be used to control driver specific creation options.
		/// These options are normally documented in the format specific documentation.
		/// </param>
		/// <returns>OgrDataSource object if successful. Null otherwise.</returns>
		public OgrDataSource CreateDataSource(string folderName, List<string> options)
		{
			string[] optionsArray = options.ToArray();

			IntPtr ptrDS = OgrInterop.OGR_Dr_CreateDataSource(this.driverHandle,
				folderName, ref optionsArray);

			if (ptrDS == IntPtr.Zero)
			{
				return null;
			}
			return new OgrDataSource(ptrDS);
		}

		/// <summary>
		/// Open a file / data source with one of the registered drivers.
		/// </summary>
		/// <param name="folderName">The name of the file, or data source to open. </param>
		/// <param name="accessType">Read-only or read-write access.</param>
		/// <returns>OgrDataSource object if successful.</returns>
		/// <exception cref="OgrException">Thrown if the datasource could not be opened.</exception>
		public OgrDataSource OpenDataSource(string folderName, OpenAccessTypes accessType)
		{
			OgrDataSource dataSource = new OgrDataSource(
				OgrInterop.OGROpen(folderName, (int)accessType, this.driverHandle));

			if (dataSource.dataSourceHandle.Handle == IntPtr.Zero)
			{
				string message = GDALWrapper.Gdal.GdalInterOp.CPLGetLastErrorMsg();
				throw new OgrException(message);
			}

			return dataSource;
		}

		/// <summary>
		/// Deletes a datasource.
		/// </summary>
		/// <param name="dataSource">The OgrDataSource to delete.</param>
		/// <returns>Error code.</returns>
		public OGRErr DeleteDataSource(OgrDataSource dataSource)
		{
			return dataSource.Release();
		}
		
		/// <summary>
		/// Test if capability is available.
		/// </summary>
		/// <remarks>
		/// One of the following data source capability names can be passed into this function, and a TRUE or FALSE value will be returned indicating whether or not the capability is available for this object.
		/// CreateDataSource - True if this driver can support creating data sources.
		/// DeleteDataSource - True if this driver supports deleting data sources.
		/// </remarks>
		/// <param name="caps">the capability to test.</param>
		/// <returns>TRUE if capability available otherwise FALSE.</returns>
		public bool TestCapabilities(OgrDriverCapabilities caps)
		{
			int iRes = OgrInterop.OGR_Dr_TestCapability(this.driverHandle, caps.ToString());
			
			if(iRes==0)
				return false;
			else
				return true;	
		}

		#endregion

		#region public static methods

		/// <summary>
		/// Gets a driver by name. e.g. "MapInfo File"
		/// </summary>
		/// <param name="driverName">The driver name.</param>
		/// <returns>OgrDriver object if successful. Null otherwise.</returns>
		public static OgrDriver GetDriverByName(string driverName)
		{
			OgrDriver rv = null;

			// find driver by name
			int numDrivers = OgrInterop.OGRGetDriverCount();
			for (int i = 0; i < numDrivers; i++)
			{
				OgrDriver driver = new OgrDriver(i);
				if (String.Compare(driver.Name, driverName, true) == 0)
				{
					rv = driver;
					break;
				}
			}
			return rv;
		}

		/// <summary>
		/// Fetch the number of registered drivers.
		/// </summary>
		/// <returns>the drivers count.</returns>
		public static int GetDriverCount()
		{
			return OgrInterop.OGRGetDriverCount();	
		}
		
		/// <summary>
		/// Registers all Ogr drivers.
		/// </summary>
		internal static void OgrRegisterAll()
		{
			PrepareGdal();
			OgrInterop.OGRRegisterAll();
		}

		/// <summary>
		/// Sets the path dnyamically to the right directory, via
		/// searching the registry for the FwTools subkey
		/// This was contributed by Morten Nielsen http://www.iter.dk
		/// </summary>
		public static void PrepareGdal()
		{
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\FWTools\\");
			if (key.GetValue("Install_Dir") == null)
					throw new ApplicationException("Error: FW Tools must be installed to use this feature. Download at www.gdal.org");

			string fwtoolsdir = key.GetValue("Install_Dir").ToString();

			if (fwtoolsdir == null)
				throw new ApplicationException("Error: FW Tools must be installed to use this feature. Download at www.gdal.org");

			//Set FW Tools path
			string path = System.Environment.GetEnvironmentVariable("PATH");
			if (!path.Contains(fwtoolsdir + "\\bin"))
				System.Environment.SetEnvironmentVariable("PATH", fwtoolsdir + "\\bin;" + path);
		} 	
		
		
		#endregion
	}
	
	/// <summary>
	/// Available capabilities for the driver
	/// </summary>
	public enum OgrDriverCapabilities
	{
		/// <summary>
		/// True if this driver can support creating data sources.
		/// </summary>
		CreateDataSource,
		/// <summary>
		/// True if this driver supports deleting data sources.
		/// </summary>
		DeleteDataSource
	};
}
