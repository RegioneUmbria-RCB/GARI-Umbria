using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GDALWrapper.Gdal
{
	public enum GDALPaletteInterp
	{
		Gray = 0,		// Grayscale 
		RGB,			// Red, Green, Blue and Alpha in (in c1, c2, c3 and c4)
		CMYK,			// Cyan, Magenta, Yellow and Black (in c1, c2, c3 and c4)
		HLS				// Hue, Lightness and Saturation (in c1, c2, and c3)
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct GDALColorEntry
	{
		public short c1;
		public short c2;
		public short c3;
		public short c4;
	}

	public class GdalColorTable
	{
		internal HandleRef colorTableHandle;

		#region constructors

		public GdalColorTable(GDALPaletteInterp palletteInterp)
		{
			this.colorTableHandle = new HandleRef(this, GdalInterOp.GDALCreateColorTable(palletteInterp));
		}

        // MC 28/11/06: per gestire correttamente le tif monocromatiche
        public GdalColorTable(IntPtr colorTableH)
        {
            this.colorTableHandle = new HandleRef(this, colorTableH);
        }
        // /MC 28/11/06

		#endregion

		#region public methods

        // MC 28/11/06: per gestire correttamente le tif monocromatiche
        public int GetColorEntryCount()
        {
            return GdalInterOp.GDALGetColorEntryCount(this.colorTableHandle);
        }
        // /MC 28/11/06

		public void DestroyColorTable()
		{
			GdalInterOp.GDALDestroyColorTable(this.colorTableHandle);
		}

		public void SetColorEntry(int offset, GDALColorEntry colorEntry)
		{
			GdalInterOp.GDALSetColorEntry(this.colorTableHandle, offset, ref colorEntry);
		}

		public void SetColorEntry(int offset, short r, short g, short b)
		{
			GDALColorEntry colorEntry = new GDALColorEntry();
			colorEntry.c1 = r;
			colorEntry.c2 = g;
			colorEntry.c3 = b;
			colorEntry.c4 = 0;
			SetColorEntry(offset, colorEntry);
		}

		#endregion
	}
}
