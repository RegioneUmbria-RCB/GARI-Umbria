using System;
using System.Runtime.InteropServices;

namespace GDALWrapper.OGR
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct StructOgrEnvelope
	{
		public double MinX;
		public double MaxX;
		public double MinY;
		public double MaxY;
 	}

	
	
	/// <summary>
	/// Simple container for a bounding region.
	/// </summary>
	public class OgrEnvelope
	{
		private IntPtr ptrEnvelope = new IntPtr(2);
		
		/// <summary>
		/// left value
		/// </summary>
		public double MinX;
		/// <summary>
		/// right value
		/// </summary>
		public double MaxX;
		/// <summary>
		/// bottom value
		/// </summary>
		public double MinY;
		/// <summary>
		/// bottom value
		/// </summary>
		public double MaxY;
		
		/// <summary>
		/// constructor of OgrEnvelope
		/// </summary>
		public OgrEnvelope()
		{
			MinX = MaxX = MinY = MaxY = 0;			
		}

		/// <summary>
		/// constructor of OgrEnvelope
		/// </summary>
		/// <param name="_MinX">left</param>
		/// <param name="_MinY">bottom</param>
		/// <param name="_MaxX">right</param>
		/// <param name="_MaxY">top</param>
		public OgrEnvelope(double _MinX, double _MinY, double _MaxX, double _MaxY)
		{
			MinX = _MinX;
			MaxX = _MaxX;
			MinY = _MinY;
			MaxY = _MaxY;
		}
		
		private bool  IsInit() 
		{ 
			return 	(MinX != 0) || (MinY != 0) || (MaxX != 0) || (MaxY != 0);
		}
		
		/// <summary>
		/// merging with other envelope
		/// </summary>
		/// <param name="sOther"></param>
		public void Merge( GDALWrapper.OGR.OgrEnvelope sOther ) {
         	if( IsInit() )
         	{
         		
             	MinX = Math.Min(MinX, sOther.MinX);
             	MaxX = Math.Max(MaxX, sOther.MaxX);
             	MinY = Math.Min(MinY, sOther.MinY);
             	MaxY = Math.Max(MaxY, sOther.MaxY);
         	}
         	else
         	{
             	MinX = sOther.MinX;
             	MaxX = sOther.MaxX;
             	MinY = sOther.MinY;
             	MaxY = sOther.MaxY;
         	}
     	}
	}
}		
