using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

using OSGeo.GDAL;

namespace SharpMap.Layers
{
    /// <summary>
    /// Gdal raster image layer
    /// </summary>
    /// <remarks>
    /// Before you can use the Gdal raster layer, you have to install the FwTools
    /// from http://fwtools.maptools.org/ . This package provides all the nessesary
    /// libraries for correct use.
    /// <example>
    /// <code lang="C#">
    /// myMap = new SharpMap.Map(new System.Drawing.Size(500,250);
    /// SharpMap.Layers.GdalRasterLayer layGdal = new SharpMap.Layers.GdalRasterLayer("Blue Marble", @"C:\data\bluemarble.ecw");
    /// myMap.Layers.Add(layGdal);
    /// myMap.ZoomToExtents();
    /// </code>
    /// </example>
    /// </remarks>
    public class AgroGdalRasterLayer : SharpMap.Layers.Layer, IDisposable
    {
        private SharpMap.Geometries.BoundingBox _Envelope;
        private OSGeo.GDAL.Dataset _GdalDataset;
        private System.Drawing.Size imagesize;
        // MC 12-04-07
        private Color _ColoreTrasparente;
        // /MC 12-04-07

        private string _Filename;
        /// <summary>
        /// Gets or sets the filename of the raster file
        /// </summary>
        public string Filename
        {
            get { return _Filename; }
            set { _Filename = value; }
        }

        // MC 12-04-07
        /// <summary>
        /// Gets or sets il colore che deve essere renderizzato come trasparente
        /// </summary>
        public Color ColoreTrasparente
        {
            get { return _ColoreTrasparente; }
            set { _ColoreTrasparente = value; }
        }
        // /MC 12-04-07

        /// <summary>
        /// initialize a Gdal based raster layer
        /// </summary>
        /// <param name="strLayerName">Name of layer</param>
        /// <param name="imageFilename">location of image</param>
        public AgroGdalRasterLayer(string strLayerName, string imageFilename)
        {

            AgronicaGdalRasterLayer.GdalConfiguration.ConfigureGdal();

            this.LayerName = strLayerName;
            this.Filename = imageFilename;
            disposed = false;

            try
            {
                
                _GdalDataset = Gdal.Open(_Filename, Access.GA_ReadOnly);

                imagesize = new System.Drawing.Size (_GdalDataset.RasterXSize,_GdalDataset.RasterYSize)  ;
                _Envelope = this.GetExtent();
            }
            catch (Exception ex) { 
                _GdalDataset = null;
                throw new Exception("Couldn't load dataset. " + ex.Message + ex.InnerException);
            }
 
        }

        public string ProjctionFromDataset()
        {
            return _GdalDataset.GetProjection();
        }


        #region ILayer Members

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="g">Graphics object reference</param>
        /// <param name="map">Map which is rendered</param>
        public override void Render(System.Drawing.Graphics g, Map map)
        {
            if (disposed)
                throw (new ApplicationException("Error: An attempt was made to render a disposed layer"));
            
            //if (this.Envelope.Intersects(map.Envelope))
            //{
                this.GetPreview(_GdalDataset, map.Size, g, map.Envelope);
            //}
            base.Render(g, map);
        }

        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>

        public override SharpMap.Geometries.BoundingBox Envelope
        {
            get { return _Envelope; }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

		#region Disposers and finalizers

		private bool disposed = false;

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
                    if (_GdalDataset != null)
					{
						try {                            
                            _GdalDataset.Dispose();
                        }
                        finally { _GdalDataset = null; }
					}
				disposed = true;
			}
		}
		/// <summary>
		/// Disposes the GdalRasterLayer and release the raster file
		/// </summary>
		public void Dispose ()
		{
			this.Dispose (true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Finalizer
		/// </summary>
		~AgroGdalRasterLayer()
		{
			this.Dispose (true);
		}


		#endregion

        private SharpMap.Geometries.BoundingBox GetExtent()
        {
            if(_GdalDataset!=null)
            {
                //vanni, 12/10/2018: nessuna riproiezione
                //GeoTransform GT = _GdalDataset.GetGeoTransform();                

                //return new SharpMap.Geometries.BoundingBox( GT.Left,
                //                                            GT.Top + (GT.VerticalPixelResolution * _GdalDataset.YSize),
                //                                            GT.Left + (GT.HorizontalPixelResolution * _GdalDataset.XSize),
                //                                            GT.Top);
            }

            // return null; // Michelangelo Casali: modifico x chè in questa versione di SharpMap BoundingBox diventa un tipo value ==> non vale il null
            return new SharpMap.Geometries.BoundingBox(0,
                                                            0,
                                                            0,
                                                            0);
        }

        // MC 01/08/07: apportate alcune modifiche a questa funzione x gestire aventi fino a 16 bit x singolo colore
        /*
         * DA RIVEDERE LA GESTIONE  A 16 BIT...
         * 
         * 1. utilizzare comunque una bitmap a 8 bit riscalando tutti i valori letti dall'immagine originale in modo che siano compresi nell'intervallo [0..255]
         *    Questo però necessita la conoscenza del numero esatto di bit per pixel. Ad esempio l'immagine di Primi è 11 bit x pixel e non 16. Come faccio ad avere questa informazione con GDAL?
         * 
         * 2. utilizzare una bitmap a 16 bit. In tal caso però gestire manualmente la scrittura di 2 byte x pixel e devo sapere l'ordine di interpretazione dei byte
         *      inoltre se uso sempre una bitmap a 16 bit anche nel caso della monocromatica il valore del bianco non sarà più 255 255 255
         */
        private void GetPreview(Dataset dataset, System.Drawing.Size size, System.Drawing.Graphics g, SharpMap.Geometries.BoundingBox bbox)
        {
            //Transformer GT = dataset.GetGeoTransform();

            //int DsWidth = dataset.RasterXSize;
            //int DsHeight = dataset.RasterYSize;
            //Point topLeft = new Point(0, 0);

            //Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            //int iPixelSize = 3; //Format24bppRgb = byte[b,g,r]

            //if (dataset != null)
            //{              
            //    double left = Math.Max(bbox.Left, _Envelope.Left);
            //    double top = Math.Min(bbox.Top, _Envelope.Top);
            //    double right = Math.Min(bbox.Right, _Envelope.Right);
            //    double bottom = Math.Max(bbox.Bottom, _Envelope.Bottom);             

            //    int x1 = (int)GT.PixelX(left);
            //    int y1 = (int)GT.PixelY(top);
            //    int x1width = (int)GT.PixelXwidth(right - left);
            //    int y1height = (int)GT.PixelYwidth(bottom - top);

            //    // MC 28/11/06: x gestire correttamente il caso di pan ai bordi dell'immagine
            //    /*
            //     * Con il calcolo che segue determino la larghezza e l'altezza in pixel con le quali
            //     * verrà renderizzato il riquadro dell'immagine da estrarre
            //     */
            //    int x2width = (int)GT.PixelXwidth(bbox.Right - bbox.Left);
            //    int y2height = (int)GT.PixelYwidth(bbox.Top - bbox.Bottom);
            //    System.Drawing.Size sizeLayer = new Size();
            //    // dimensioni in pixel alle quali deve essere ridimensionato il riquadro dell'immagine visualizzato
            //    sizeLayer.Width = (int)(((double)(size.Width * x1width)) / x2width);
            //    sizeLayer.Height = (int)(((double)(size.Height * y1height)) / y2height);
            //    // devo determinare il punto top-left del riquadro da visualizzare
            //    topLeft.X = (bbox.Left >= _Envelope.Left) ? 0:(int)((GT.PixelXwidth(_Envelope.Left - bbox.Left) * (double)size.Width) / x2width);
            //    topLeft.Y = (bbox.Top <= _Envelope.Top) ? 0 : (int)((GT.PixelYwidth(bbox.Top - _Envelope.Top) * (double)size.Height) / y2height);
            //    // /MC 28/11/06

            //    // bitmap a 48 bit x pixel x gestire anche il caso di 16 bit x banda (RGB)
            //    //bitmap = new Bitmap(sizeLayer.Width, sizeLayer.Height, PixelFormat.Format48bppRgb);
            //    bitmap = new Bitmap(sizeLayer.Width, sizeLayer.Height, PixelFormat.Format24bppRgb);
            //    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, sizeLayer.Width, sizeLayer.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            //    try
            //    {
            //        unsafe
            //        {
            //            for (int i = 1; i <= (dataset.RasterCount > 3 ? 3 : dataset.RasterCount); ++i)
            //            {
            //                Band band = dataset.GetRasterBand(i);
            //                DataType dataType = band.DataType;

            //                // ==> fino a 8 bit per pixel
            //                if (dataType == DataType.GDT_Byte)
            //                {
            //                    byte[] buffer = new byte[sizeLayer.Width * sizeLayer.Height];

            //                    band.ReadRaster(RWFlag.GF_Read, x1, y1, x1width, y1height, buffer, sizeLayer.Width, sizeLayer.Height);

            //                    int p_indx = 0;
            //                    int ch = 0;

            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_BlueBand) ch = 0;
            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_GreenBand) ch = 1;
            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_RedBand) ch = 2;
            //                    if (band.GetColorInterpretation() != ColorInterp.GCI_PaletteIndex)
            //                    {
            //                        for (int y = 0; y < sizeLayer.Height; y++)
            //                        {
            //                            byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            //                            for (int x = 0; x < sizeLayer.Width; x++, p_indx++)
            //                            {
            //                                row[x * iPixelSize + ch] = buffer[p_indx];
            //                            }
            //                        }
            //                    }
            //                    else //8bit Grayscale
            //                    {
            //                        ColorTable CT = band.GetColorTable();
            //                        int numeroColori = (CT != null) ? CT.GetCount() : 256;
            //                        if (numeroColori == 2)  // monocromatico
            //                        {
            //                            byte bwhite = 255;
            //                            byte bblack = 0;
            //                            byte bcol;

            //                            for (int y = 0; y < sizeLayer.Height; y++)
            //                            {
            //                                byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            //                                for (int x = 0; x < sizeLayer.Width; x++, p_indx++)
            //                                {
            //                                    bcol = (buffer[p_indx] == 0 ? bwhite : bblack);
            //                                    row[x * iPixelSize] = bcol;
            //                                    row[x * iPixelSize + 1] = bcol;
            //                                    row[x * iPixelSize + 2] = bcol;
            //                                }
            //                            }
            //                        }
            //                        else //8bit Grayscale
            //                        {
            //                            for (int y = 0; y < sizeLayer.Height; y++)
            //                            {
            //                                byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            //                                for (int x = 0; x < sizeLayer.Width; x++, p_indx++)
            //                                {
            //                                    row[x * iPixelSize] = buffer[p_indx];
            //                                    row[x * iPixelSize + 1] = buffer[p_indx];
            //                                    row[x * iPixelSize + 2] = buffer[p_indx];
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //                else if (dataType == DataType.GDT_UInt16)    // ==> fino a 16 bit per pixel
            //                {
            //                    UInt16[] buffer = new UInt16[sizeLayer.Width * sizeLayer.Height];

            //                    band.RasterIO(RWFlag.Read, x1, y1, x1width, y1height, buffer, sizeLayer.Width, sizeLayer.Height);

            //                    int p_indx = 0;
            //                    int ch = 0;

            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_BlueBand) ch = 0;
            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_GreenBand) ch = 1;
            //                    if (band.GetColorInterpretation() == ColorInterp.GCI_RedBand) ch = 2;
            //                    if (band.GetColorInterpretation() != ColorInterp.GCI_PaletteIndex)
            //                    {
            //                        for (int y = 0; y < sizeLayer.Height; y++)
            //                        {
            //                            byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
            //                            for (int x = 0; x < sizeLayer.Width; x++, p_indx++)
            //                            {
            //                                // DEBUG: 2048 ==> 11 bit per pixel
            //                                row[x * iPixelSize + ch] = (byte)(( (float)buffer[p_indx]/ (float)2048 ) * 256);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        throw new Exception("Formato PaletteIndex a 16 bit per pixel non supportato.");
            //                    }
            //                }
            //                else
            //                {
            //                    throw new Exception("Tipo di dato per la banda corrente non supportato.");
            //                }
                            
            //            }
            //        }
            //    }
            //    finally
            //    {
            //        bitmap.UnlockBits(bitmapData);
            //    }
            //}
            //// MC 12-04-07
            //if (_ColoreTrasparente != null)
            //    bitmap.MakeTransparent(_ColoreTrasparente);
            //// /MC 12-04-07
            //// NOTA: il punto è il top-left
            //g.DrawImage(bitmap, topLeft);
        }
        // /MC 01/08/07

    }
}
