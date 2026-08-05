using System;
using System.Collections.Generic;
using System.Text;

namespace GDALWrapper.OGR
{
	public class OgrException : Exception
	{
		public OgrException()
			: base()
		{
		}

		public OgrException(string message)
			: base(message)
		{
		}

		public OgrException(OGRErr error)
			: base(error.ToString())
		{
		}
	}
}
