/*
 * Created by SharpDevelop.
 * User: Christian
 * Date: 23.06.2006
 * Time: 00:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace GDALWrapper
{
	/// <summary>
	/// Description of Gdal.
	/// </summary>
	public class GdalOgr
	{
/// <summary>
        /// Sets the path dnyamically to the right directory, via
        /// searching the registry for the FwTools subkey
        /// This was contributed by Morten Nielsen http://www.iter.dk
        /// </summary>
        public static void PrepareGdal()
        {
        	PrepareGdal("");
        }

        /// <summary>
        /// Sets the path dnyamically to the right directory, via
        /// searching the registry for the FwTools subkey
        /// This was contributed by Morten Nielsen http://www.iter.dk
        /// </summary>
        /// <param name="CustomPath">Path to custom installation of Gdal</param>
        public static void PrepareGdal(String CustomPath)
        {
        	if(String.IsNullOrEmpty(CustomPath))
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
        	else
        	{
        		//Set custom path
	            string path = System.Environment.GetEnvironmentVariable("PATH");
	            if (!path.Contains(CustomPath))
	                System.Environment.SetEnvironmentVariable("PATH", CustomPath + ";" + path);	
        		
        		
        	}
        } 
	}
}
