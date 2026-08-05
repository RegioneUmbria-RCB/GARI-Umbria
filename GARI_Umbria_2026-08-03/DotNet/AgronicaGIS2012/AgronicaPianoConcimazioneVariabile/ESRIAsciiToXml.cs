using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace CalcoloPianoConcimazioneVariabile
{
    class ESRIAsciiToXml
    {

        private static double _AgronicaPuntoConcimazioneLatOffest=0;

        public static void ConvertFile(string sNomeInputFile,
            string sTipoIrrigazione,
            string sUnitaBase,
            string sGradoConfidenza,
            string sDataFineValidita,
            string sTargetFile,
            string sColtivation,
            string sPoligonoAppezzamentoWKT,
            string sCoordinateSystem
            )
        {
            // - Dichiarazione delle variabili per gli argomenti 

            // - Dichiarazione delle variabili per le informazioni contenute nel file di input
            int nCols = 0;
            int nRows = 0;

            Double XLowerLeftCorner = 0;
            Double YLowerLeftCorner = 0;
            Double CellSize = 0;

            Double YUpperLeftCorner = 0;
            /*
            // - Lettura degli argomenti
            */
            try
            {
                /* - Argomenti base
                sNomeInputFile = sNomeInputFile;
                sTargetFile = sTargetFile;
                 - Informazioni da aggiungere nel file
                sTipoIrrigazione = sTipoIrrigazione;
                sUnitaBase = sUnitaBase;
                sGradoConfidenza = sGradoConfidenza;
                sDataFineValidita = sDataFineValidita;
                sColtivation = sColtivation;
                */

            }
            catch (Exception exc)
            {
                string sError = "Errore: " + exc.Message + " - Source: " + exc.Source;
                throw new Exception(sError);
            }

            try
            {
                /*
                // - Validazione degli argomenti
                */

                // - Controllo dell'esistenza del file di input
                if (!System.IO.File.Exists(sNomeInputFile))
                {
                    throw new Exception("Il file di input non esiste.");
                }

                if (!System.IO.Directory.Exists(sTargetFile.Substring(0, sTargetFile.LastIndexOf("\\"))))
                {
                    throw new Exception("La directory di input non esiste.");
                }

                /*
                // - Inizio trasformazione
                */

                /*System.IO.StreamReader oR = new System.IO.StreamReader(sNomeInputFile, System.Text.Encoding.ASCII);	
                string sFile = oR.ReadToEnd();
                string[] InputFileContent = sFile.Split('\n');
				
                 for(int iIndex =0; iIndex < InputFileContent.Length; iIndex++)
                {
                    if (InputFileContent[iIndex].Length > 0)
                    {
                        if(InputFileContent[iIndex][InputFileContent[iIndex].Length-1].CompareTo('\r') == 0)
                            InputFileContent[iIndex] = InputFileContent[iIndex].Substring(0, InputFileContent[iIndex].Length-1);
                    }
                } */
                string[] InputFileContent = System.IO.File.ReadAllLines(sNomeInputFile, Encoding.ASCII);

                // - Righe e colonne
                nCols = Convert.ToInt32(InputFileContent[0].Substring(13));
                nRows = Convert.ToInt32(InputFileContent[1].Substring(13));

                // - Coordinate angolo in basso a sx
                XLowerLeftCorner = Convert.ToDouble(InputFileContent[2].Substring(13).Replace(".", ","));
                YLowerLeftCorner = Convert.ToDouble(InputFileContent[3].Substring(13).Replace(".", ","));

                // - Unità di misura
                CellSize = Convert.ToDouble(InputFileContent[4].Substring(13).Replace(".", ","));

                // - Calcolo della coordinata Y dell'angolo in alto a sx
                YUpperLeftCorner = YLowerLeftCorner + (nRows * CellSize);

                object[,] oMatrix = new object[nRows, nCols];
                // - Calcolo le coordinate per ogni valore
                for (int iIndex = 5; iIndex <= InputFileContent.GetUpperBound(0); iIndex++)
                {
                    string sTemp = InputFileContent[iIndex].TrimStart().TrimEnd();
                    char[] charSeparators = new char[] { ' ' };
                    string[] FileRow = sTemp.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    System.Diagnostics.Debug.Write(iIndex.ToString());
                    for (int jIndex = 0; jIndex <= FileRow.GetUpperBound(0); jIndex++)
                    {
                        System.Diagnostics.Debug.Write(jIndex.ToString());
                        Double fValue = Convert.ToDouble(FileRow[jIndex].Replace(".", ","));
                        Double XPointCenter = XLowerLeftCorner + (jIndex * CellSize) + (CellSize / 2);
                        Double YPointCenter = YUpperLeftCorner - ((iIndex - 5) * CellSize) - (CellSize / 2);

                        PointObject oPoint = new PointObject();
                        oPoint.Value = fValue;
                        oPoint.XCoord = XPointCenter;
                        oPoint.YCoord = YPointCenter;

                        oMatrix[iIndex - 5, jIndex] = oPoint;
                    }
                }
                // System.Diagnostics.Debug.
                // - Creazione del file XML

                System.Xml.XmlDocument oTargetDom = new System.Xml.XmlDocument();
                String FileName = ConfigurationManager.AppSettings["FileModelloFODM"];
                String EXAppezzamento = ConfigurationManager.AppSettings["EXAppezzamento"];
                String EXImpianto = ConfigurationManager.AppSettings["EXImpianto"];

                //creazione da modello
                if (System.IO.File.Exists(FileName))
                    oTargetDom.Load(FileName);
                else
                    FileName = "-1";

                System.Xml.XmlNode oTipoIrr;
                if (FileName == "-1")
                {
                    System.Xml.XmlNode oGeoNode = oTargetDom.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "GeoAnswer", ""));

                    System.Xml.XmlNode oNTmp = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "EndPeriod", ""));
                    oNTmp.InnerText = sDataFineValidita;

                    oNTmp = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Cultivation", ""));
                    oNTmp.InnerText = sColtivation;

                    oTipoIrr = oGeoNode.AppendChild(oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, sTipoIrrigazione, ""));
                    System.Xml.XmlAttribute oAttr1 = oTargetDom.CreateAttribute("BaseUnit");
                    oAttr1.Value = sUnitaBase;
                    System.Xml.XmlAttribute oAttr2 = oTargetDom.CreateAttribute("PixelUnit");
                    oAttr2.Value = CellSize.ToString();
                    System.Xml.XmlAttribute oAttr3 = oTargetDom.CreateAttribute("ConfidentialLevel");
                    oAttr3.Value = sGradoConfidenza;

                    oTipoIrr.Attributes.Append(oAttr1);
                    oTipoIrr.Attributes.Append(oAttr2);
                    oTipoIrr.Attributes.Append(oAttr3);
                }
                else
                {
                    oTipoIrr = oTargetDom.SelectSingleNode("//FODM/Resources/RxList/RxPoly/Records");

                    //clear veloce!
                    oTipoIrr.InnerXml = "";
                }

                //la distanza fra punti dipende ovviamente dal sistema di riferimento
                double distanceBetweenPoints;
                PointObject p1, p2;

                if (ConfigurationManager.AppSettings["ApplicaTrasformazione"] == "true")
                {
                    p1 = getMinLatitude(oMatrix);
                    p2 = getMinLatitudeWKTPolygon(sPoligonoAppezzamentoWKT);
                    _AgronicaPuntoConcimazioneLatOffest = TransformED50ToWGSDecimal (p2).YCoord  - TransformED50ToWGSDecimal (p1).YCoord ;
                }

                distanceBetweenPoints = CellSize;

                //if (ConfigurationManager.AppSettings["ApplicaTrasformazione"] == "true")
                //    distanceBetweenPoints = CellSize;
                //    //distanceBetweenPoints = getDistance(TransformED50ToWGSDecimal ((PointObject)oMatrix[0, 0]), TransformED50ToWGSDecimal ((PointObject)oMatrix[0, 1]));
                //else
                //    distanceBetweenPoints = CellSize;

                for (int i = 0; i <= oMatrix.GetUpperBound(0); i++)
                    for (int j = 0; j <= oMatrix.GetUpperBound(1); j++)
                    {
                        PointObject oPoint = (PointObject)oMatrix[i, j];
                        
                        //Formato ISOBUS
                        /*
                        System.Xml.XmlNode oPointNode = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "point", "");
                        System.Xml.XmlAttribute oPAttr1 = oTargetDom.CreateAttribute("E");
                        
                        //oPAttr1.Value = oPoint.XCoord.ToString();
                        oPAttr1.Value = oTranslatedWGS84.XCoord.ToString();
                        System.Xml.XmlAttribute oPAttr2 = oTargetDom.CreateAttribute("N");
                        
                        //oPAttr2.Value = oPoint.YCoord.ToString();
                        oPAttr2.Value = oTranslatedWGS84.YCoord.ToString();

                        oPointNode.Attributes.Append(oPAttr1);
                        oPointNode.Attributes.Append(oPAttr2);

                        oPointNode.InnerText = oPoint.Value.ToString("#0.000");
                        oTipoIrr.AppendChild(oPointNode);
                         */
                        //Fine Formato ISOBUS


                        //Formato FDOM
                       
                        //Piano di prescrizione variabile
                        if (Math.Abs(oPoint.Value) >= 0.0001d)
                        {
                            System.Xml.XmlNode oPointRecord = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Record", "");
                            System.Xml.XmlNode oPointGeometryWKT = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "GeometryWKT", "");
                            System.Xml.XmlNode oPointValue = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Value", "");
                            System.Xml.XmlAttribute oValueKey = oTargetDom.CreateAttribute("ValueKey");
                            oValueKey.Value = "a:1";
                            oPointValue.Attributes.Append(oValueKey);
                            oPointValue.InnerText = oPoint.Value.ToString("#0.000");
                            oPointGeometryWKT.InnerText = DatoPuntoDammiQuadrato_WKTFormat(oPoint, distanceBetweenPoints);

                            oPointRecord.AppendChild(oPointGeometryWKT);
                            oPointRecord.AppendChild(oPointValue);
                            oTipoIrr.AppendChild(oPointRecord);
                        }

                    }

                //Poligono appezzamento ed impianto
                if (FileName == "-1")
                {
                    System.Xml.XmlNode oPoligonoAppezzamento = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "Appezzamento", "");
                    oPoligonoAppezzamento.InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT);
                    oTipoIrr.AppendChild(oPoligonoAppezzamento);
                }
                else 
                {
                    oTargetDom.SelectSingleNode("//FODM/Resources/DomainList/Domain[@uri='" + EXAppezzamento + "']/BoundaryWKT").InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT);
                    oTargetDom.SelectSingleNode("//FODM/Resources/DomainList/Domain[@uri='" + EXImpianto + "']/BoundaryWKT").InnerText = TransformWKTPolygonED50ToWGS84(sPoligonoAppezzamentoWKT);

                }

                //System.Xml.XmlNode oCoordinateSystem = oTargetDom.CreateNode(System.Xml.XmlNodeType.Element, "SistemaRiferimento", "");
                //oCoordinateSystem.InnerText = sCoordinateSystem;
                //oTipoIrr.AppendChild(oCoordinateSystem);


                oTargetDom.Save(sTargetFile);

                // - Creazione del file ASCII
                System.IO.FileStream oStream = System.IO.File.Open(sTargetFile.Replace(".xml", ".txt"), System.IO.FileMode.CreateNew);
                System.IO.StreamWriter oStreamW = new System.IO.StreamWriter(oStream);
                for (int i = 0; i <= oMatrix.GetUpperBound(0); i++)
                    for (int j = 0; j <= oMatrix.GetUpperBound(1); j++)
                    {
                        PointObject oPoint = (PointObject)oMatrix[i, j];
                        oStreamW.WriteLine(oPoint.XCoord.ToString() + ";" + oPoint.YCoord.ToString() + ";" + oPoint.Value.ToString("#0.000"));
                    }

                oStreamW.Flush();
                oStreamW.Close();
            }
            catch (Exception exc)
            {
                string sError = "Errore: " + exc.Message + " - Source: " + exc.Source;
                throw new Exception(sError);
            }

        }

        private static PointObject  getMinLatitude(object[,]  oMatrix)
        {
            PointObject rval = new PointObject();                        
            PointObject app=new PointObject();

            double minx = 0, minY = 500000000.0;

            for (int i = 0; i <= oMatrix.GetUpperBound(0); i++)
                    for (int j = 0; j <= oMatrix.GetUpperBound(1); j++)
                    {

                        app=(PointObject)oMatrix[i, j];
                        if (app.YCoord < minY && Math.Abs(app.Value) >= 0.0001d)
                        {
                            minx = app.XCoord;
                            minY = app.YCoord;

                        }
                        
                    }

            rval.XCoord = app.XCoord;
            rval.YCoord = app.YCoord;
            return rval; 
        }

        private static double getDistance(PointObject p1, PointObject p2)
        {
            return Math.Sqrt( Math.Pow((p1.XCoord-p2.XCoord),2) +  Math.Pow((p1.YCoord-p2.YCoord),2) );
        }

        private static String DatoPuntoDammiQuadrato_WKTFormat(PointObject Center, double DistanceBeetweenPoints)
        {

            double Center_XCoord = double.Parse(ConfigurationManager.AppSettings["AgronicaPuntoConcimazioneLonOffest"]);
            double Center_YCoordFromWEBConfig = double.Parse(ConfigurationManager.AppSettings["AgronicaPuntoConcimazioneLatOffest"]);

            double Center_YCoord = _AgronicaPuntoConcimazioneLatOffest + Center_YCoordFromWEBConfig;


            bool ApplicaTrasformazione = (ConfigurationManager.AppSettings["ApplicaTrasformazione"] == "true");

            double p1x, p1y, p2x, p2y, p3x, p3y, p4x, p4y;
            PointObject appoggio = new PointObject();

            p1x = (Center.XCoord - (DistanceBeetweenPoints / 2));
            p1y = (Center.YCoord - (DistanceBeetweenPoints / 2));
            if (ApplicaTrasformazione)
            {
                appoggio.XCoord = p1x;
                appoggio.YCoord = p1y;
                appoggio = TransformED50ToWGSDecimal(appoggio);
                p1x=appoggio.XCoord + Center_XCoord;
                p1y=appoggio.YCoord + Center_YCoord;
            }

            p2x = (Center.XCoord - (DistanceBeetweenPoints / 2));
            p2y = (Center.YCoord + (DistanceBeetweenPoints / 2));
            if (ApplicaTrasformazione)
            {
                appoggio.XCoord = p2x;
                appoggio.YCoord = p2y;
                appoggio = TransformED50ToWGSDecimal(appoggio);
                p2x=appoggio.XCoord + Center_XCoord;
                p2y=appoggio.YCoord + Center_YCoord;
            }


            p3x = (Center.XCoord + (DistanceBeetweenPoints / 2));
            p3y = (Center.YCoord + (DistanceBeetweenPoints / 2));
            if (ApplicaTrasformazione)
            {
                appoggio.XCoord = p3x;
                appoggio.YCoord = p3y;
                appoggio = TransformED50ToWGSDecimal(appoggio);
                p3x=appoggio.XCoord + Center_XCoord;
                p3y=appoggio.YCoord + Center_YCoord;
            }

            p4x = (Center.XCoord + (DistanceBeetweenPoints / 2));
            p4y = (Center.YCoord - (DistanceBeetweenPoints / 2));
            if (ApplicaTrasformazione)
            {
                appoggio.XCoord = p4x;
                appoggio.YCoord = p4y;
                appoggio = TransformED50ToWGSDecimal(appoggio);
                p4x=appoggio.XCoord + Center_XCoord;
                p4y=appoggio.YCoord + Center_YCoord;
            }

            
            //POLYGON ((p1x p1y,p2x p2y, p3x p3y, p4x p4y))
            return ("POLYGON ((" + p1x.ToString().Replace(",",".") + " " + p1y.ToString().Replace(",",".") + ", " 
                                 + p2x.ToString().Replace(",",".") + " " + p2y.ToString().Replace(",",".") + ", " 
                                 + p3x.ToString().Replace(",",".") + " " + p3y.ToString().Replace(",",".") + ", " 
                                 + p4x.ToString().Replace(",",".") + " " + p4y.ToString().Replace(",",".") + "))");
 
        }

        public static PointObject getMinLatitudeWKTPolygon(String sWKT)
        {
            Char[] sep = new Char[2];
            sep[0] = ',';
            sep[1] = ' ';
            String[] AppData = sWKT.Substring(10,sWKT.Length-11).Split(sep);

            double minx = 0, miny = 500000000.0;
            
            string sourceDecimalSeparator;
            
            try
            {
                sourceDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            }
            catch (Exception e)
            {
                sourceDecimalSeparator = ",";                
            }

            PointObject  rval= new PointObject();
            PointObject  cur= new PointObject();
             
            for (int i = 0; i < AppData.Length - 3; i+=3)
            {

                cur.XCoord= double.Parse(AppData[i].Replace(".",sourceDecimalSeparator ));
                cur.YCoord = double.Parse(AppData[i+1].Replace(".",sourceDecimalSeparator ));
                if (cur.YCoord < miny)
                {
                    minx = cur.XCoord;
                    miny = cur.YCoord;
                }

            }

            rval.XCoord = minx;
            rval.YCoord = miny;
            return rval;
        }

        public static String TransformWKTPolygonED50ToWGS84(String sWKT)
        {

            String rVal = "POLYGON ((";
            Char[] sep = new Char[2];
            sep[0] = ',';
            sep[1] = ' ';
            String[] AppData = sWKT.Substring(10,sWKT.Length-11).Split(sep);

            PointObject vertex;
            PointObject vertexTarget;

            string sourceDecimalSeparator;
            string finalDecimalSeparator=".";

            try
            {
                sourceDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            }
            catch (Exception e)
            {
                sourceDecimalSeparator = ",";                
            }

            for (int i = 0; i < AppData.Length - 3; i+=3)
            {
                vertex = new PointObject();
                vertex.XCoord = double.Parse(AppData[i].Replace(".",sourceDecimalSeparator ));
                vertex.YCoord = double.Parse(AppData[i+1].Replace(".",sourceDecimalSeparator ));
                if (ConfigurationManager.AppSettings["ApplicaTrasformazionePoligono"] == "true")
                    vertexTarget = TransformED50ToWGSDecimal(vertex);
                else 
                    vertexTarget = vertex;
                rVal = rVal + vertexTarget.XCoord.ToString().Replace(",", finalDecimalSeparator) + " " + vertexTarget.YCoord.ToString().Replace(",", finalDecimalSeparator) + ", ";
            }

            return rVal.Substring(0,rVal.Length-3) + "))";
            
        }


        private static PointObject TransformED50ToWGSDecimal(PointObject In)
        {

            string tbCSFrom_Text = ConfigurationManager.AppSettings["CSFrom"];
            string tbCSto_Text = ConfigurationManager.AppSettings["CSTo"];
            string tbGeo_Text = ConfigurationManager.AppSettings["CStoGeo"];


            //ICoordinateSystem fromCS;
            //ICoordinateSystem toCS;
            //ICoordinateSystem toCSGEO;

            ////non Projected
            //if (tbCSFrom_Text.StartsWith("PROJCS"))
            //    fromCS = (IProjectedCoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom_Text);
            //else
            //    fromCS = (ICoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom_Text);

            ////From
            ////ICoordinateSystem fromCS = SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom.Text);
            //if (tbCSto_Text.StartsWith("PROJCS"))
            //    toCS = (IProjectedCoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSto_Text);
            //else
            //    toCS = (ICoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSto_Text);

            ////TO
            //if (tbCSFrom_Text.StartsWith("PROJCS"))
            //    fromCS = (IProjectedCoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom_Text);
            //else
            //    fromCS = (ICoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom_Text);

            ////TO_GEO
            ////ICoordinateSystem fromCS = SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbCSFrom.Text);
            //if (tbGeo_Text.StartsWith("PROJCS"))
            //    toCSGEO = (IProjectedCoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbGeo_Text);
            //else
            //    toCSGEO = (ICoordinateSystem)ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse(tbGeo_Text);


            var fact = new ProjNet.CoordinateSystems.CoordinateSystemFactory();

            //First create a CoordinateTransformationFactory:
            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

            //Then create the transformation instance:
            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(fact.CreateFromWkt(tbCSFrom_Text), fact.CreateFromWkt(tbCSto_Text));
            ICoordinateTransformation planGeo = ctfac.CreateFromCoordinateSystems(fact.CreateFromWkt(tbCSFrom_Text), fact.CreateFromWkt(tbGeo_Text));

            double[] fromPoint = new double[] { In.XCoord , In.YCoord  };
            double[] toPoint = trans.MathTransform.Transform(fromPoint);

            
            double[] fromPlanPoint = new double[] { toPoint[0], toPoint[1] };
            double[] toGeoPoint = planGeo.MathTransform.Transform(fromPlanPoint);

            //Agronica Offset
            double AgroLatOffset, AgroLonOffset;

            AgroLatOffset = double.Parse(ConfigurationManager.AppSettings["AgronicaLatOffest"]);
            AgroLonOffset = double.Parse(ConfigurationManager.AppSettings["AgronicaLonOffest"]);

            PointObject rVal = new PointObject();
            rVal.XCoord = toGeoPoint[0] + AgroLonOffset;
            rVal.YCoord = toGeoPoint[1] + AgroLatOffset;
            rVal.Value = In.Value;

            return rVal;

        }
    }

    class PointObject
    {
        public double Value;
        public double XCoord;
        public double YCoord;
    }
}
