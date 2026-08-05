using System;
using System.Collections.Generic;
using System.Text;
using SharpMap;
using System.Drawing;
using System.Configuration;

using OSGeo.GDAL;
using NetTopologySuite.Geometries;

namespace CalcoloPianoConcimazioneVariabile
{
    public class CalcoloPianoConcimazioneVariabile
    {
        private double _ConsiglioMedioConcimazione;
        private SharpMap.Geometries.Polygon _PoligonoAppezzamento;
        private SharpMap.Geometries.Point _PuntoSingolo;
        private SharpMap.Layers.AgronicaGdalRasterLayer.MapServer _MapServer;
        //private NetAgree.RintraccioManager.GestioneGIS.TipoRaster.TipiRaster _TipoRaster;
        private string _PathOutputFileGif, _PathOutputFileXml, _DescrizioneSpecie;
        private string _fullFileNameWithPath;

        private double _BoundingBox_xllCorner, _BoundingBox_yllCorner;

        //DEBUG
        //private NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azioni _Azioni;
        // /DEBUG

        
        public string EsriiAcii { get; set; }

        // Costruttore di debug
        public CalcoloPianoConcimazioneVariabile(double pConsiglioMedioConcimazione
                                                , SharpMap.Geometries.Polygon pPoligonoAppezzamento
                                                , string pPathOutputFileGif
                                                , string pPathOutputFileXml
                                                , string pDescrizioneSpecie
                                                )
        {
            _ConsiglioMedioConcimazione = pConsiglioMedioConcimazione;
            _PoligonoAppezzamento = pPoligonoAppezzamento;
            _PathOutputFileGif = pPathOutputFileGif;
            _PathOutputFileXml = pPathOutputFileXml;
            _DescrizioneSpecie = pDescrizioneSpecie;
            //DEBUG

            // DEBUG
        }

        public CalcoloPianoConcimazioneVariabile(double pConsiglioMedioConcimazione
                                                , string fullFileNameWithPath
                                                , SharpMap.Geometries.Polygon pPoligonoAppezzamento
                                                , string pPathOutputFileGif
                                                , string pPathOutputFileXml
                                                , string pDescrizioneSpecie
                                                )
        {
            _ConsiglioMedioConcimazione = pConsiglioMedioConcimazione;
            _PoligonoAppezzamento = pPoligonoAppezzamento;
            _PathOutputFileGif = pPathOutputFileGif;
            _PathOutputFileXml = pPathOutputFileXml;
            _DescrizioneSpecie = pDescrizioneSpecie;
            _fullFileNameWithPath = fullFileNameWithPath;
            //DEBUG
            Piano_Concimazione_Variabile.GdalConfiguration.ConfigureGdal();
            // DEBUG
        }

        public CalcoloPianoConcimazioneVariabile(double pConsiglioMedioConcimazione
                                                , string fullFileNameWithPath
                                                , SharpMap.Geometries.Point pPuntoSingolo
                                                , string pPathOutputFileGif
                                                , string pPathOutputFileXml
                                                , string pDescrizioneSpecie
                                                )
        {
            _ConsiglioMedioConcimazione = pConsiglioMedioConcimazione;
            _PuntoSingolo = pPuntoSingolo;
            _PathOutputFileGif = pPathOutputFileGif;
            _PathOutputFileXml = pPathOutputFileXml;
            _DescrizioneSpecie = pDescrizioneSpecie;
            _fullFileNameWithPath = fullFileNameWithPath;
            //DEBUG
            Piano_Concimazione_Variabile.GdalConfiguration.ConfigureGdal();
            // DEBUG
        }
        public CalcoloPianoConcimazioneVariabile(double pConsiglioMedioConcimazione
                                                , SharpMap.Geometries.Polygon pPoligonoAppezzamento
                                                , string pPathOutputFileGif
                                                , string pPathOutputFileXml
                                                , string pDescrizioneSpecie
                                                , string idle
                                                )
        {
            _ConsiglioMedioConcimazione = pConsiglioMedioConcimazione;
            _PoligonoAppezzamento = pPoligonoAppezzamento;
            _PathOutputFileGif = pPathOutputFileGif;
            _PathOutputFileXml = pPathOutputFileXml;
            _DescrizioneSpecie = pDescrizioneSpecie;
            Piano_Concimazione_Variabile.GdalConfiguration.ConfigureGdal();

        }

        public void Elabora(int cellsize)
        {
            RasterData DRNI;
            OutputData OD;

            DRNI = ReadData();
            //OD = CalcolaPianoConcimazioneVariabile(DRNI);
            //2010
            OD = CalcolaPianoConcimazioneVariabile2010(DRNI);

            EsriiAcii = GeneraEsriASCII(OD, DRNI, cellsize);
            //GeneraFileOutputGIF(OD, DRNI);
            //GeneraFileOutputXml(OD, DRNI);
        }

        private RasterData ReadDataFromPoint()
        {
            RasterData RD;
            Dataset Dataset;
            Dataset = Gdal.Open(_fullFileNameWithPath, Access.GA_ReadOnly);

            double[] affineTransform1 = new double[6];
            Dataset.GetGeoTransform(affineTransform1);

            SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform GT = new SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform(affineTransform1);


            int xPx = (int)GT.PixelX(_PuntoSingolo.X);
            int yPx = (int)GT.PixelY(_PuntoSingolo.Y);

            int WidthPx = 1;
            int HeightPx = 1;

            Band Band = Dataset.GetRasterBand(1);

            double[] GrayIndexData = new double[WidthPx * HeightPx];

            Band.ReadRaster(xPx, yPx, WidthPx, HeightPx, GrayIndexData, WidthPx, HeightPx, 0, 0);

            //vanni, capire la trasformazione. ..: 
            int HorizontalPixelResolution = 10;
            int VerticalPixelResolution = 10;



            RD = new RasterData(_PuntoSingolo.Y, _PuntoSingolo.X, HorizontalPixelResolution, VerticalPixelResolution, 1, 1, WidthPx, HeightPx,_PuntoSingolo, null, null, GrayIndexData, Dataset.GetProjection());

            return RD;
        }

        /*
         * Legge i dati dal set di immagini di input e restituisce un struttura contenente i dati nella banda del rosso e quelli nella banda dell'infrarosso vicino
         * 
         * Restrizioni:
         * . si suppone che l'appezzamento sia tutto in un unico file raster. In caso contrario eccezione
         * . si suppone che il tipo di dati di ciascuna banda sia Byte. In caso contrario eccezione
         * . poiché non è possibile dedurre dall'immagine quale sia la banda dell'infrarosso vicino si suppone che la banda
         *   dell'infrarosso vicino sia sempre la prima banda dove ColorInterpretation = Undefined
         * . si suppone che i raster layer restituiti dal MapServer siano di tipo NetAgreeGDALRasterLayer in quanto
         *   è l'unico tipo di layer che espone pubblicamente il nome del file relativo al raster caricato
         */
        private RasterData ReadData()
        {
            SharpMap.Geometries.BoundingBox Bb;
            //List<SharpMap.Layers.Layer> ElencoLayer;
            SharpMap.Layers.AgroGdalRasterLayer RasterLayer;
            Dataset Dataset;
            Band Band;
            //GDALWrapper.Gdal.GeoTransform GT;
            RasterData RD;
            byte[] RedBandData, NearInfraredData;
            double[] GrayIndexData;
            int GrayIndex, RedBandIndex, NearInfraredBandIndex, i, xPx, yPx, WidthPx, HeightPx;
            //string Filename;
            int tipoElaborazione = 1;

            RasterLayer = null;
            Dataset = null;
            try
            {
                RedBandData = null;
                NearInfraredData = null;
                GrayIndexData = null;
                int TipoImmagine = 0;

                /*
                 * tre tipi di immagini:
                 * 1. 4 canali - RGB + infrarosso su undefined
                 * 2. 3 canali - RGB, dove R=I, G=R, B=G (infrarosso sulla banda R, Rosso sulla banda G)
                 * 3. toni di grigi con la sola elaborazione degli indici
                 * 
                 * */

                Bb = _PoligonoAppezzamento.GetBoundingBox();
                _BoundingBox_xllCorner = Bb.Left;
                _BoundingBox_yllCorner = Bb.Bottom;
                // NOTA: al map server passo come risoluzione del pixel 0 (sia in altezza che in larghezza) poichè in questo modo il map server
                //      restituisce il set di raster che coprono la regione in input con la risoluzione maggiormente definita disponibile

                ////todo get elenco layer
                ////ElencoLayer = _MapServer.GetMap( , Bb.Top, Bb.Left, Bb.Bottom, Bb.Right, 0, 0);
                //SharpMap.Layers.AgroGdalRasterLayer Layer = new SharpMap.Layers.AgroGdalRasterLayer("tmp", _fullFileNameWithPath);

                //ElencoLayer = new List<SharpMap.Layers.Layer>();
                //ElencoLayer.Add(Layer);

                //if (ElencoLayer == null || ElencoLayer.Count == 0)
                //    throw new Exception("Il server cartografico non ha trovato alcuna immagine multispettrale che contenga l'appezzamento da elaborare.");
                //else if (ElencoLayer.Count > 1)
                //    throw new Exception("Il server cartografico ha restituito più di un raster per l'appezzamento da elaborare. L'elaborazione funziona con un solo raster.");

                //if (ElencoLayer[0].GetType() != typeof(SharpMap.Layers.AgroGdalRasterLayer))
                //    throw new Exception("Il layer restituito dal server cartografico deve essere di tipo GDAL.");

                //RasterLayer = (SharpMap.Layers.AgroGdalRasterLayer)ElencoLayer[0];
                //Filename = RasterLayer.Filename;

                //RasterLayer.Dispose();
                //RasterLayer = null;

                Dataset = Gdal.Open(_fullFileNameWithPath, Access.GA_ReadOnly);                
                switch (Dataset.RasterCount)
                {
                    case 4:
                        TipoImmagine = 1;
                        break;
                    case 3:
                        TipoImmagine = 2;
                        break;
                    case 1:
                        TipoImmagine = 3;
                        break;
                    default:
                        throw new Exception("L'immagine restituita dal server cartografico non è nel formato richiesto. Per eseguire l'elaborazione è necessaria un'immagine in uno dei formati prestabiliti.");
                }


                //if (Dataset.RasterCount < 2)
                //    throw new Exception("L'immagine restituita dal server cartografico non è multispettrale. Per eseguire l'elaborazione è necessaria un'immagine multispettrale.");

                double[] affineTransform1 = new double[6];
                Dataset.GetGeoTransform(affineTransform1);

                SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform GT = new SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform(affineTransform1);
                
                xPx = (int)GT.PixelX(Bb.Left);
                yPx = (int)GT.PixelY(Bb.Top);
                
                WidthPx = (int)GT.PixelXwidth(Bb.Right - Bb.Left);
                HeightPx = (int)GT.PixelXwidth(Bb.Bottom - Bb.Top);
                
                RedBandIndex = -1;
                NearInfraredBandIndex = -1;
                GrayIndex = -1;
                i = 1;
                while (i <= Dataset.RasterCount) //&& (RedBandIndex == -1 || NearInfraredBandIndex == -1))
                {
                    Band = Dataset.GetRasterBand(i);
                    string msg;
                    msg = "Band.ColorInterpretation:" + Band.GetColorInterpretation().ToString() + " - Band.GetDataType(): " + Band.DataType.ToString();
                    //_Azioni.Add(new NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione(NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione.Tipi.Warning,msg ));

                    //if (Band.DataType == DataType.GDT_Byte)
                    if (Band.GetColorInterpretation() != ColorInterp.GCI_GrayIndex )
                    {
                        //2010: accetto in ingresso anche una mappa con il risultato dell'elaborazione da una mappa multispettrale. questo è memorizato in un'unica banda a scala di grigi.
                        //throw new Exception("Una o più bande dell'immagine multispettrale non è di tipo Byte. Il sistema supporta solo bande di tipo Byte.");

                        // vanni 2010 DEBUG
                        msg = "TipoImmagine: " + TipoImmagine.ToString() + " - ID della banda: " + Band.GetColorInterpretation().ToString() + " <BR>Top: " + Bb.Top.ToString() + "<BR>Left: " + Bb.Left.ToString() + "<BR>Bottom: " + Bb.Bottom.ToString() + "<BR>Right: " + Bb.Right.ToString();
                        //_Azioni.Add(new NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione(NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione.Tipi.Warning, msg));
                        tipoElaborazione = 1;

                        if (Band.GetColorInterpretation() == ColorInterp.GCI_RedBand)
                        {
                            //if (TipoImmagine == 1)
                            //{
                            //    LeggiDaBanda(Band, i, xPx, yPx, WidthPx, HeightPx, out RedBandData, out RedBandIndex);
                            //}
                            //else
                            //{
                            //    LeggiDaBanda(Band, i, xPx, yPx, WidthPx, HeightPx, out NearInfraredData, out NearInfraredBandIndex);
                            //}
                            LeggiDaBanda(Band, i, xPx, yPx, WidthPx, HeightPx, out RedBandData, out RedBandIndex);
                        }

                        if (Band.GetColorInterpretation() == ColorInterp.GCI_GreenBand && TipoImmagine == 2)
                        {
                            LeggiDaBanda(Band, i, xPx, yPx, WidthPx, HeightPx, out NearInfraredData, out NearInfraredBandIndex);
                        }

                        if (Band.GetColorInterpretation() == ColorInterp.GCI_Undefined
                                || Band.GetColorInterpretation() == ColorInterp.GCI_AlphaBand)  // la condizione su AlphaBand è xchè in fase di conversione da 11 a 8 bit GDAL ha assegnato alla banda Undefined l'interpretazione di banda Alpha (trasparenza)
                        {
                            NearInfraredBandIndex = i;
                            NearInfraredData = new byte[WidthPx * HeightPx];

                            // Vanni Costa, 26/02/2025: OBSOLETO: ora si ottiene il tutto attraverso le nuove api “WS_Mappe_2024” (.net6 + c#, con ultime versioni di GDAL, che non richiedono UNSAFE).
                            //unsafe
                            //{
                            //    //vanni, 12/10/2018, capire gli ultimi due parametri
                            //    Band.ReadRaster(xPx, yPx, WidthPx, HeightPx, NearInfraredData, WidthPx, HeightPx, 0, 0);
                            //}
                        }
                    }
                    else
                    {
                        tipoElaborazione = 2;
                        GrayIndex = i;
                        GrayIndexData = new double[WidthPx * HeightPx];

                        // Vanni Costa, 26/02/2025: OBSOLETO: ora si ottiene il tutto attraverso le nuove api “WS_Mappe_2024” (.net6 + c#, con ultime versioni di GDAL, che non richiedono UNSAFE).
                        //unsafe
                        //{
                        //    Band.ReadRaster(xPx, yPx, WidthPx, HeightPx, GrayIndexData, WidthPx, HeightPx, 0, 0);
                        //}
                    }
                    i++;
                }
                if (tipoElaborazione == 1 && RedBandIndex == -1)
                    throw new Exception("L'immagie multispettrale non contiene la banda del rosso. Per eseguire l'elaborazione sono necessarie le bande del rosso e dell'infrarosso vicino.");
                if (tipoElaborazione == 1 && NearInfraredBandIndex == -1)
                    throw new Exception("L'immagie multispettrale non contiene la banda dell'infrarosso vicino. Per eseguire l'elaborazione sono necessarie le bande del rosso e dell'infrarosso vicino.");

                //vanni, capire la trasformazione. ..: 
                int HorizontalPixelResolution = 10;
                int VerticalPixelResolution = 10;

                RD = new RasterData(Bb.Top, Bb.Left, HorizontalPixelResolution, VerticalPixelResolution, Bb.Width, Bb.Height, WidthPx, HeightPx, Bb.GetCentroid(), RedBandData, NearInfraredData, GrayIndexData, Dataset.GetProjection());
                // DEBUG
                //                string msg = "Top: " + Bb.Top.ToString() + "<BR>Left: " + Bb.Left.ToString() + "<BR>Bottom: " + Bb.Bottom.ToString() + "<BR>Right: " + Bb.Right.ToString();
                //                _Azioni.Add(new NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione(NetAgree.RintraccioManager.GestioneDocumenti.Istanze.Azione.Tipi.Warning, msg));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // rilascio delle risorse allocate
                if (RasterLayer != null)
                {
                    RasterLayer.Dispose();
                }
                if (Dataset != null)
                {
                    //Dataset.Close();
                    Dataset.Dispose();
                }
            }

            return RD;
        }

        //public RasterData readDataEmpty()
        //{

        //    SharpMap.Geometries.BoundingBox Bb;
        //    List<SharpMap.Layers.Layer> ElencoLayer;
        //    SharpMap.Layers.AgroGdalRasterLayer RasterLayer;
        //    Dataset _Dataset;
        //    Band Band;
        //    SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform GT;
        //    RasterData RD;
        //    byte[] RedBandData, NearInfraredData;
        //    double[] GrayIndexData;
        //    int GrayIndex, RedBandIndex, NearInfraredBandIndex, i, xPx, yPx, WidthPx, HeightPx;
        //    string Filename;
        //    int tipoElaborazione = 1;

        //    RasterLayer = null;
        //    _Dataset = null;
        //    try
        //    {
        //        RedBandData = null;
        //        NearInfraredData = null;
        //        GrayIndexData = null;
        //        int TipoImmagine = 0;

        //        Bb = _PoligonoAppezzamento.GetBoundingBox();


        //        double[] affineTransform1 = new double[6];
        //        _Dataset.GetGeoTransform(affineTransform1);

        //        GT = new SharpMap.Layers.AgronicaGdalRasterLayer.GeoTransform(affineTransform1);


        //        xPx = (int)GT.PixelX(Bb.Left);
        //        yPx = (int)GT.PixelY(Bb.Top);
        //        WidthPx = (int)GT.PixelXwidth(Bb.Right - Bb.Left);
        //        HeightPx = (int)GT.PixelXwidth(Bb.Bottom - Bb.Top);

        //        /*
        //         * tre tipi di immagini:
        //         * 1. 4 canali - RGB + infrarosso su undefined
        //         * 2. 3 canali - RGB, dove R=I, G=R, B=G (infrarosso sulla banda R, Rosso sulla banda G)
        //         * 3. toni di grigi con la sola elaborazione degli indici
        //         * 
        //         * */



        //        RD = new RasterData(Bb.Top, Bb.Left, GT.HorizontalPixelResolution, GT.VerticalPixelResolution, Bb.Width, Bb.Height, WidthPx, HeightPx, Bb.GetCentroid(), RedBandData, NearInfraredData, GrayIndexData, Dataset.GetProjection());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        // rilascio delle risorse allocate
        //        if (RasterLayer != null)
        //        {
        //            RasterLayer.Dispose();
        //        }
        //        if (_Dataset != null)
        //        {                    
        //            _Dataset.Dispose();
        //        }
        //    }

        //    return RD;
        //}

        private static void LeggiDaBanda(Band BandToBeRead, int i, int xPx, int yPx, int WidthPx, int HeightPx, out byte[] BandData, out int BandIndex)
        {
            BandIndex = i;
            BandData = new byte[WidthPx * HeightPx];

            // Vanni Costa, 26/02/2025: OBSOLETO: ora si ottiene il tutto attraverso le nuove api “WS_Mappe_2024” (.net6 + c#, con ultime versioni di GDAL, che non richiedono UNSAFE).
            //unsafe
            //{
            //    BandToBeRead.ReadRaster(xPx, yPx, WidthPx, HeightPx, BandData, WidthPx, HeightPx, 0, 0);
            //}
        }

        // algoritmo di calcolo versione 2007.
        // x ogni pixel dell'appezzamento calcola la quantità di azoto necessaria
        // restituisce l'array di output valorizzato e il massimo valore nell'array di output
        /*
         * Il calcolo della quantità di azoto x pixel viene effettuato partendo dalla seguente tabella
         * 
         * NDVI         N (Azoto in Kg/Ha)
         * > 0.6        0*(QMA/QMAB)
         * [0.6, 0.45]  20*(QMA/QMAB)
         * [0.45, 0.3]  40*(QMA/QMAB)
         * < 0.3        60*(QMA/QMAB)
         * 
         * dove QMAB = 180 è la Quantità Massima Annua Base, ovvero la quantità annua di azoto a partire dalla quale è stata tarata a tabella sopra.
         * Poichè il calcolo è riferito alla sola concimazione di copertura si suppone che 2/3 del quantitativo massimo per anno sia stato distribuito uniformemente in pre-semina
         * e che 1/3 sia la quantità da distribuire in copertura.
         * QMA è la Quantitò Massma Annua, ovvero il valore restituito dall'algoritmo di calcolo del consiglio medio di concimazione (Agronica)
         * 
         * La tabella riportata sopra è stata trasformata su indicazione di Vincini in una funzione lineare come segue
         * 
         * ---
         * | N = 0                                          NDVI > 0.6
         * | N = (-200*(QMA/QMAB))*NDVI + (120*(QMA/QMAB))  0.3 <= NDVI <= 0.6
         * | N = 60 * (QMA/QMAB)                            NDVI < 0.3
         * ---
         */
        private OutputData CalcolaPianoConcimazioneVariabile(RasterData pData)
        {
            double OutputValue, MaxOutputValue;
            double[] OD;
            int i, j, index;
            SharpMap.Geometries.Point WCSPoint;
            //GisSharpBlog.NetTopologySuite.Geometries.Geometry NTSPolygon, NTSPoint;
            NetTopologySuite.Geometries.Geometry NTSPolygon, NTSPoint;
            SharpMap.Data.Providers.GeometryProvider GProvider;
            SharpMap.Layers.VectorLayer VLayer;
            SharpMap.Map Mappa;
            double QMAB = 180;
            double QMA = _ConsiglioMedioConcimazione;
            double NDVI;
            OutputData OutD = null;

            Mappa = null;
            try
            {
                OD = new double[pData.WidthPx * pData.HeightPx];
                GProvider = new SharpMap.Data.Providers.GeometryProvider(_PoligonoAppezzamento);
                VLayer = new SharpMap.Layers.VectorLayer("VLayer", GProvider);
                Mappa = new SharpMap.Map();
                Mappa.Layers.Add(VLayer);
                Mappa.Size = new System.Drawing.Size(pData.WidthPx, pData.HeightPx);
                Mappa.Zoom = pData.Width;
                Mappa.Center = pData.Center;

                //NTSPolygon = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(_PoligonoAppezzamento, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                NTSPolygon = ConvertToNTSGeometry(_PoligonoAppezzamento.ExteriorRing, _PoligonoAppezzamento.InteriorRings);


                System.IO.StreamWriter sw = new System.IO.StreamWriter("D:\\www_manager\\CitimapMaster\\Allegati\\AAA_Debug2010.txt");

                MaxOutputValue = 0.0f;
                for (i = 0; i < pData.HeightPx; i++)
                {
                    for (j = 0; j < pData.WidthPx; j++)
                    {
                        WCSPoint = Mappa.ImageToWorld(new System.Drawing.PointF(j, i));
                        //NTSPoint = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(WCSPoint, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                        NTSPoint = ConvertToNTSGeometry(WCSPoint);
                        index = i * pData.WidthPx + j;
                        if (NTSPoint.Within(NTSPolygon))
                        {
                            // Punto interno al poligono ==> calcolo la quantità di azoto
                            NDVI = ((double)pData.NearInfraredBandData[index] - (double)pData.RedBandData[index])
                                    / ((double)pData.NearInfraredBandData[index] + (double)pData.RedBandData[index]);
                            if (NDVI < 0.3f)
                            {
                                OutputValue = 60f * (QMA / QMAB);
                            }
                            else if (NDVI > 0.6f)
                            {
                                OutputValue = 0.0f;
                            }
                            else
                            {
                                OutputValue = (-200f * (QMA / QMAB)) * NDVI + (120f * (QMA / QMAB));
                            }

                            //Debug
                            sw.WriteLine(pData.NearInfraredBandData[index].ToString() + "   " + pData.RedBandData[index].ToString() + "     " + NDVI.ToString() + "     " + OutputValue.ToString());
                        }
                        else
                        {
                            // Punto esterno al poligono ==> qtà di azoto = 0, ma lo contrassegno con un -1 x distinguerlo dagli altri punti
                            OutputValue = -1.0f;
                        }
                        OD[index] = OutputValue;
                        if (OutputValue > MaxOutputValue)
                            MaxOutputValue = OutputValue;
                    }
                }
                OutD = new OutputData(OD, MaxOutputValue);

                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // deallocazione delle risorse
                if (Mappa != null)
                    Mappa.Dispose();
            }

            return OutD;
        }

        //algoritmo di calcolo versione marzo 2010
        private OutputData CalcolaPianoConcimazioneVariabile2010(RasterData pData)
        {
            double[] OD;
            //SharpMap.Geometries.Point WCSPoint;
            //GisSharpBlog.NetTopologySuite.Geometries.Geometry NTSPolygon, NTSPoint;
            NetTopologySuite.Geometries.Geometry NTSPolygon, NTSPoint;
            SharpMap.Data.Providers.GeometryProvider GProvider;
            SharpMap.Layers.VectorLayer VLayer;
            SharpMap.Map Mappa;
            double QMAB = 180;
            double QMA = _ConsiglioMedioConcimazione;
            OutputData OutD = null;

            Mappa = null;
            try
            {
                OD = new double[pData.WidthPx * pData.HeightPx];
                GProvider = new SharpMap.Data.Providers.GeometryProvider(_PoligonoAppezzamento);
                VLayer = new SharpMap.Layers.VectorLayer("VLayer", GProvider);
                Mappa = new SharpMap.Map();
                Mappa.Layers.Add(VLayer);
                Mappa.Size = new System.Drawing.Size(pData.WidthPx, pData.HeightPx);
                Mappa.Zoom = pData.Width;
                Mappa.Center = pData.Center;

                //NTSPolygon = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(_PoligonoAppezzamento, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                NTSPolygon = ConvertToNTSGeometry(_PoligonoAppezzamento.ExteriorRing, _PoligonoAppezzamento.InteriorRings);

                double VIstd_Media, stdDev;
                CalcoloMediaStdDEV(pData, NTSPolygon, Mappa, out VIstd_Media, out stdDev);
                OutD = CalcoloPianoConcimazione(VIstd_Media, stdDev, QMA, QMAB, 70.0d, 100.0d, pData, NTSPolygon, Mappa);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // deallocazione delle risorse
                if (Mappa != null)
                    Mappa.Dispose();
            }


            return OutD;
        }


        private void CalcoloMediaStdDEV(RasterData pData, NetTopologySuite.Geometries.Geometry NTSPolygon, SharpMap.Map Mappa, out double VIstd_Media, out double stdDEV)
        {
            SharpMap.Geometries.Point WCSPoint;
            //GisSharpBlog.NetTopologySuite.Geometries.Geometry NTSPoint;
            NetTopologySuite.Geometries.Geometry NTSPoint;
            int i, j, index;
            double somma = 0;
            double conteggio = 0;
            double ti = 0;
            double tiQuadro = 0;
            double NDVI;

            for (i = 0; i < pData.HeightPx; i++)
            {
                for (j = 0; j < pData.WidthPx; j++)
                {
                    WCSPoint = Mappa.ImageToWorld(new System.Drawing.PointF(j, i));
                    //NTSPoint = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(WCSPoint, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                    NTSPoint = ConvertToNTSGeometry(WCSPoint);
                    index = i * pData.WidthPx + j;
                    if (NTSPoint.Within(NTSPolygon))
                    {

                        //calcolo su quale dato??
                        // Punto interno al poligono ==> calcolo la quantità di azoto
                        NDVI = IndicePrescelto(pData, index);
                        ti += Math.Pow(NDVI, 2.0d);
                        tiQuadro += (NDVI);
                        somma += NDVI;
                        conteggio++;

                    }
                    else
                    {
                        // Punto esterno al poligono ==> qtà di azoto = 0, ma lo contrassegno con un -1 x distinguerlo dagli altri punti

                    }
                }
            }
            VIstd_Media = somma / conteggio;
            ti = conteggio * ti;
            tiQuadro = Math.Pow(tiQuadro, 2.0d);
            stdDEV = (Math.Sqrt(ti - tiQuadro)) / conteggio;
        }



        private OutputData CalcoloPianoConcimazione(double Media, double stdDev, double QMA, double QMAB, double percMin, double percMax, RasterData pData, NetTopologySuite.Geometries.Geometry NTSPolygon, SharpMap.Map Mappa)
        {
            SharpMap.Geometries.Point WCSPoint;
            //GisSharpBlog.NetTopologySuite.Geometries.Geometry NTSPoint;
            NetTopologySuite.Geometries.Geometry NTSPoint;
            int i, j, index;
            double MaxOutputValue = 0.0f;
            double VIstd;
            double OutputValue;
            double percentApplicata, PercentualeConcimazioneCorrente, PercentualeAzoto;
            double VIstd_Min, VIstd_Max;



            double[] OD = new double[pData.WidthPx * pData.HeightPx];

            //determino min e max
            CalcolaMinMax(Media, stdDev, pData, NTSPolygon, Mappa, out VIstd_Min, out VIstd_Max);

            //affinità lineare ---- x in [VIstd_Min, VIstd_Max] diventa x1 in [percMin,percMax] quindi la scala è:
            double scala = (percMax - percMin) / (VIstd_Max - VIstd_Min);

            Boolean debug = false;

            System.IO.StreamWriter sw;
            if (debug)
                sw = new System.IO.StreamWriter("D:\\www_manager\\CitimapMaster\\Allegati\\AAA_Debug2010-nuovo.txt");
            else
                sw = null;

            if (debug)
                sw.WriteLine("Min:" + VIstd_Min.ToString() + " - Max: " + VIstd_Max.ToString() + " - Media: " + Media.ToString() + " - stdDEV: " + stdDev.ToString());

            //leggo da web.config al momento ...
            PercentualeConcimazioneCorrente = double.Parse(ConfigurationManager.AppSettings["PercentualeConcimazioneCorrente"]);
            PercentualeAzoto = double.Parse(ConfigurationManager.AppSettings["PercentualeAzoto"]);

            MaxOutputValue = 0.0f;
            for (i = 0; i < pData.HeightPx; i++)
            {
                for (j = 0; j < pData.WidthPx; j++)
                {
                    WCSPoint = Mappa.ImageToWorld(new System.Drawing.PointF(j, i));
                    //NTSPoint = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(WCSPoint, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                    NTSPoint = ConvertToNTSGeometry(WCSPoint);
                    index = i * pData.WidthPx + j;
                    if (NTSPoint.Within(NTSPolygon))
                    {

                        // Punto interno al poligono ==> calcolo la quantità di azoto
                        VIstd = IndicePrescelto(pData, index);
                        VIstd = ((VIstd - Media) / stdDev);

                        //b1 + ( x + a ) * scala
                        percentApplicata = percMax + ((VIstd + VIstd_Min) * scala);
                        OutputValue = ((QMA * percentApplicata) / 100.0d);

                        //pondero la quantità di azoto ...
                        OutputValue = OutputValue / PercentualeAzoto;

                        //tengo conto delle concimazioni già effettuate
                        OutputValue = OutputValue * PercentualeConcimazioneCorrente;

                        //Debug
                        if (debug)
                            sw.WriteLine("outputvalue [" + OutputValue.ToString() + "] = percentApplicata [" + percentApplicata.ToString() + "]= percMax [" + percMax.ToString() + "] + ((NDVI [" + VIstd.ToString() + "] - VIstd_Min [" + VIstd_Min.ToString() + "]) * scala [" + scala.ToString() + "])");
                    }
                    else
                    {
                        // Punto esterno al poligono ==> qtà di azoto = 0, ma lo contrassegno con un -1 x distinguerlo dagli altri punti
                        OutputValue = -1.0f;
                    }
                    OD[index] = OutputValue;
                    if (OutputValue > MaxOutputValue)
                        MaxOutputValue = OutputValue;
                }
            }
            if (debug)
            {
                sw.Close();
                sw.Dispose();
            }
            return new OutputData(OD, MaxOutputValue);

        }

        private void CalcolaMinMax(double Media, double stdDev, RasterData pData, NetTopologySuite.Geometries.Geometry NTSPolygon, SharpMap.Map Mappa, out double VIStd_min, out double VIStd_max)
        {
            VIStd_min = 0;
            VIStd_max = 0;

            SharpMap.Geometries.Point WCSPoint;
            //GisSharpBlog.NetTopologySuite.Geometries.Geometry NTSPoint;
            NetTopologySuite.Geometries.Geometry NTSPoint;
            int i, j, index;
            double VIstd;

            for (i = 0; i < pData.HeightPx; i++)
            {
                for (j = 0; j < pData.WidthPx; j++)
                {
                    WCSPoint = Mappa.ImageToWorld(new System.Drawing.PointF(j, i));
                    //NTSPoint = SharpMap.Converters.NTS.GeometryConverter.ToNTSGeometry(WCSPoint, new GisSharpBlog.NetTopologySuite.Geometries.GeometryFactory());
                    NTSPoint = ConvertToNTSGeometry(WCSPoint);
                    index = i * pData.WidthPx + j;
                    if (NTSPoint.Within(NTSPolygon))
                    {

                        // Punto interno al poligono ==> calcolo la quantità di azoto
                        VIstd = IndicePrescelto(pData, index);
                        VIstd = ((VIstd - Media) / stdDev);

                        VIStd_max = (VIStd_max > VIstd ? VIStd_max : VIstd);
                        VIStd_min = (VIStd_min < VIstd ? VIStd_min : VIstd);
                    }
                    else
                    {
                        // Punto esterno al poligono ==> qtà di azoto = 0, ma lo contrassegno con un -1 x distinguerlo dagli altri punti
                    }
                }
            }

        }

        public double LetturaDatiElaborati()
        {
            RasterData pData;

            pData = ReadDataFromPoint();

            double rval;
            rval = IndicePrescelto(pData, 0);
            return rval;
        }

        private static double IndicePrescelto(RasterData pData, int index)
        {
            double rval;
            if (pData.NearInfraredBandData != null)
                rval = ((double)pData.NearInfraredBandData[index] - (double)pData.RedBandData[index])
                                / ((double)pData.NearInfraredBandData[index] + (double)pData.RedBandData[index]);
            else
                rval = (double)pData.GrayIndexData[index]; //2010: dato che si presuppone essere già elaborato
            return rval;
        }

        //fine piano 2010


        //Lavez - 05/09/2025 - conversione a NetTopologySuite 2.5
        #region Mapper per compatibilità con NetTopologySuite 2.x

        private Polygon ConvertToNTSGeometry(SharpMap.Geometries.LinearRing exRing, List<SharpMap.Geometries.LinearRing> internalRings)
        {
            var cc = new List<Coordinate>();
            foreach(var v in exRing.Vertices)
                cc.Add(new Coordinate(v.X, v.Y));

            var extR =  new LinearRing(cc.ToArray());
            var intR = new List<LinearRing>();

            if (internalRings == null || internalRings.Count> 0)
            {
                foreach(var r in internalRings)
                {
                    cc.Clear();
                    foreach (var v in r.Vertices)
                        cc.Add(new Coordinate(v.X, v.Y));
                    intR.Add(new LinearRing(cc.ToArray()));
                }
            }

            return new Polygon(extR, intR.ToArray(), new GeometryFactory());
        }

        private NetTopologySuite.Geometries.Point ConvertToNTSGeometry(SharpMap.Geometries.Point point)
        {
            return new NetTopologySuite.Geometries.Point(new Coordinate(point.X, point.Y));
        }
        #endregion


        private void GeneraFileOutputGIF(OutputData pOutputData, RasterData pRasterData)
        {
            // genera il file di output dell'algoritmo in formato GIF
            //int i, j;
            //byte Value;
            double MaxValue;
            //double OutputValue;
            System.Drawing.Bitmap bitmap = new Bitmap(pRasterData.WidthPx, pRasterData.HeightPx, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, pRasterData.WidthPx, pRasterData.HeightPx), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            MaxValue = 255f;
            if (pOutputData.MaxValue > MaxValue)
                MaxValue = pOutputData.MaxValue;
            try
            {
                // Vanni Costa, 26/02/2025: OBSOLETO: ora si ottiene il tutto attraverso le nuove api “WS_Mappe_2024” (.net6 + c#, con ultime versioni di GDAL, che non richiedono UNSAFE).
                //unsafe
                //{
                //    for (i = 0; i < pRasterData.HeightPx; i++)
                //    {
                //        byte* row = (byte*)bitmapData.Scan0 + (i * bitmapData.Stride);
                //        for (j = 0; j < pRasterData.WidthPx; j++)
                //        {
                //            OutputValue = pOutputData.Data[i * pRasterData.WidthPx + j];
                //            if (OutputValue < 0.0f)
                //            {
                //                // Area al d fuori dall'appezzamento
                //                row[j * 3] = 0;
                //                row[j * 3 + 1] = 128;
                //                row[j * 3 + 2] = 128;
                //            }
                //            else
                //            {
                //                Value = (byte)(255 - (OutputValue / MaxValue) * 255);
                //                row[j * 3] = Value;
                //                row[j * 3 + 1] = Value;
                //                row[j * 3 + 2] = Value;
                //            }
                //        }
                //    }
                //}
                bitmap.UnlockBits(bitmapData);
                bitmap.Save(_PathOutputFileGif, System.Drawing.Imaging.ImageFormat.Gif);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore in fase di creazione dell'immagine di output", ex);
            }
            finally
            {
                try
                {
                    bitmap.UnlockBits(bitmapData);
                    bitmap.Dispose();
                }
                catch (Exception ex1)
                { }
            }
        }

        private string GeneraEsriASCII (OutputData pOutputData, RasterData pRasterData, int cellsize)
        {
            
            string Separatore, Line;
            StringBuilder Sw = new StringBuilder();
            StringBuilder swTestata = new StringBuilder();
            Separatore = " ";
            System.Globalization.NumberFormatInfo numberFormat_EnUS = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
            

            int i, j;
            double Value;

            int nTotRows, nTotCols;
            nTotRows = 0;
            nTotCols = 0;

            cellsize = cellsize / 10;

            for (i = 0; i < pRasterData.HeightPx; i+=cellsize)
            {
                Line = "";
                nTotCols++;
                for (j = 0; j < pRasterData.WidthPx; j+=cellsize)
                {
                    Value = pOutputData.Data[i * pRasterData.WidthPx + j];
                    Value = (Value < 0.0f) ? 0.0f : Value;
                    Line += Separatore + Value.ToString(numberFormat_EnUS);
                    if (j==0) {
                        nTotRows++;
                    }
                    
                }
                Sw.AppendLine(Line);
            }

            // intestazione del file
            swTestata.AppendLine("ncols" + (nTotCols + 1).ToString().PadLeft(10, ' '));
            swTestata.AppendLine("nrows" + (nTotRows - 1).ToString().PadLeft(10, ' '));
            //swTestata.AppendLine("xllcorner    " + pRasterData.Left.ToString(numberFormat_EnUS));
            //swTestata.AppendLine("yllcorner    " + pRasterData.Top.ToString(numberFormat_EnUS));
            swTestata.AppendLine("xllcorner    " + _BoundingBox_xllCorner.ToString(numberFormat_EnUS));
            swTestata.AppendLine("yllcorner    " + _BoundingBox_yllCorner.ToString(numberFormat_EnUS));
            //swTestata.AppendLine("yllcorner    " + (pRasterData.Top - nTotRows * cellsize * 10).ToString(numberFormat_EnUS));
            //swTestata.AppendLine("yllcorner    " + pRasterData.Top.ToString(numberFormat_EnUS));
            swTestata.AppendLine("cellsize     " + cellsize * 10);            

            

            string rval = swTestata.ToString() + Sw.ToString();

            return rval;
            
        }

        // Genera una mappa di prescrizione xml nel formato definito da H&S
        private void GeneraFileOutputXml(OutputData pOutputData, RasterData pRasterData)
        {
            string PathOutputESRIASCII, Separatore, Line;
            System.IO.StreamWriter Sw = null;
            int i, j;
            double Value;
            Separatore = " ";
            System.Globalization.NumberFormatInfo numberFormat_EnUS = new System.Globalization.CultureInfo("en-US", false).NumberFormat;

            try
            {
                // Creo il file ESRI ASCII
                PathOutputESRIASCII = System.IO.Path.ChangeExtension(_PathOutputFileXml, ".eag");
                Sw = new System.IO.StreamWriter(PathOutputESRIASCII);
                // intestazione del file
                Sw.WriteLine("ncols        " + pRasterData.WidthPx.ToString());
                Sw.WriteLine("nrows        " + pRasterData.HeightPx.ToString());
                Sw.WriteLine("xllcorner    " + pRasterData.Left.ToString(numberFormat_EnUS));
                Sw.WriteLine("yllcorner    " + pRasterData.Top.ToString(numberFormat_EnUS));
                Sw.WriteLine("cellsize     " + pRasterData.HorizontalPixelResolution.ToString(numberFormat_EnUS));

                for (i = 0; i < pRasterData.HeightPx; i++)
                {
                    Line = "";
                    for (j = 0; j < pRasterData.WidthPx; j++)
                    {
                        Value = pOutputData.Data[i * pRasterData.WidthPx + j];
                        Value = (Value < 0.0f) ? 0.0f : Value;
                        Line += Separatore + Value.ToString(numberFormat_EnUS);
                    }
                    Sw.WriteLine(Line);
                }

                Sw.Close();
                Sw.Dispose();
                Sw = null;

                // Converto il file ESRI ASCII in xml con le procedure fornite da H&S

                ESRIAsciiToXml.ConvertFile(PathOutputESRIASCII, "Concimazione", "35", "1", DateTime.Now.ToString(), _PathOutputFileXml, _DescrizioneSpecie, _PoligonoAppezzamento.ToString(), "");
                // DEBUG
                // System.IO.File.Copy(PathOutputESRIASCII,_PathOutputFileXml);
                // DEBUG

                // Elimino il file ESRI ASCII
                System.IO.File.Delete(PathOutputESRIASCII);
            }
            catch (Exception ex)
            {
                throw new Exception("Si è verificato un errore nella creazione della mappa di prescrizione in formato xml.", ex);
            }
            finally
            {
                if (Sw != null)
                {
                    Sw.Close();
                    Sw.Dispose();
                }
            }
        }

    }

    public class RasterData
    {
        private byte[] _RedBandData, _NearInfraredBandData;
        private double[] _GrayIndexData;
        private int _WidthPx, _HeightPx;
        private double _Top, _Left, _HorizontalPixelResolution, _VerticalPixelResolution, _Width, _Height;
        private string _Projection;
        private SharpMap.Geometries.Point _Center;

        public RasterData(double pTop
                        , double pLeft
                        , double pHorizontalPixelResoltion
                        , double pVerticalPixelResolution
                        , double pWidth
                        , double pHeight
                        , int pWidthPx
                        , int pHeightPx
                        , SharpMap.Geometries.Point pCenter
                        , byte[] pRedBandData
                        , byte[] pNearInfraredBandData
                        , double[] pGrayIndexData
                        , string pProjection
                        )
        {
            _Top = pTop;
            _Left = pLeft;
            _HorizontalPixelResolution = pHorizontalPixelResoltion;
            _VerticalPixelResolution = pVerticalPixelResolution;
            _Width = pWidth;
            _Height = pHeight;
            _WidthPx = pWidthPx;
            _HeightPx = pHeightPx;
            _Center = pCenter;
            _RedBandData = pRedBandData;
            _NearInfraredBandData = pNearInfraredBandData;
            _GrayIndexData = pGrayIndexData;
            _Projection = pProjection;
        }

        // Larghezza in pixel dell'area dati
        public int WidthPx
        {
            get { return _WidthPx; }
        }

        // Altezza in pixel dell'area dati
        public int HeightPx
        {
            get { return _HeightPx; }
        }

        public double Width
        {
            get { return _Width; }
        }

        public double Height
        {
            get { return _Height; }
        }

        public double Left
        {
            get { return _Left; }
        }

        public double Top
        {
            get { return _Top; }
        }

        public double HorizontalPixelResolution
        {
            get { return _HorizontalPixelResolution; }
        }

        public SharpMap.Geometries.Point Center
        {
            get { return _Center; }
        }

        public byte[] RedBandData
        {
            get { return _RedBandData; }
        }

        public byte[] NearInfraredBandData
        {
            get { return _NearInfraredBandData; }
        }

        public double[] GrayIndexData
        {
            get { return _GrayIndexData; }
        }
    }

    public class OutputData
    {
        private double _MaxValue;
        private double[] _Data;

        public OutputData(double[] pData, double pMaxValue)
        {
            _Data = pData;
            _MaxValue = pMaxValue;
        }

        public double[] Data
        {
            get { return _Data; }
        }

        public double MaxValue
        {
            get { return _MaxValue; }
        }
    }
}
