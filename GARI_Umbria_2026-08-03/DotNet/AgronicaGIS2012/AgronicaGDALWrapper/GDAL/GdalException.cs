using System;
using System.Collections.Generic;
using System.Text;

namespace GDALWrapper.Gdal
{
	internal class GdalException : Exception
	{
		public GdalException()
			: base()
		{
		}

		public GdalException(string message)
			: base(message)
		{
		}

		public GdalException(Gdal.CPLErr error)
			: base(error.ToString())
		{
		}
	}
}
