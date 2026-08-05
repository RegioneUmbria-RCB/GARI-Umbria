Imports AgronicaCoreDataProvider

Public Class MaschereLayerRaster_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiElencoMaschere(ByVal layer_cod As Int32,
                                        ByVal tipologia_layer_cod As Int32,
                                        ByVal gruppiUtente As List(Of Int32),
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_R.LeggiElencoMaschere()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQLQueryInterna As New System.Text.StringBuilder
            StrSQLQueryInterna.Length = 0

            StrSQLQueryInterna.Append("SELECT 1 FROM GIS_ApplicazioneMaschereUtenti")
            StrSQLQueryInterna.Append(String.Format(" WHERE PivaSuperUser = '{0}'", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQLQueryInterna.Append(String.Format(" AND Username = '{0}'", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQLQueryInterna.Append(" AND Maschera_Cod = M.Maschera_Cod")
            StrSQLQueryInterna.Append(" AND Applica = 1")

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	M.Maschera_Cod ")
            StrSQL.AppendLine(" 	, M.Maschera_Des ")
            StrSQL.AppendLine(" 	, M.LayerElementiGrafici_Raster_Cod ")
            StrSQL.AppendLine(" 	, M.TipologiaLayer_Raster_Cod ")
            StrSQL.AppendLine(" 	, M.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	, M.TipologiaLayer_Cod ")
            StrSQL.AppendLine(" 	, M.Validita_Inizio ")
            StrSQL.AppendLine(" 	, M.Validita_Fine ")
            StrSQL.AppendLine(String.Format(" 	, ISNULL(({0}), 0) as AttivaPerUtente ", StrSQLQueryInterna.ToString))
            StrSQL.AppendLine(" FROM GIS_Maschera M ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" M.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            If layer_cod <> 0 Then
                StrSQL.AppendLine(String.Format(" AND M.LayerElementiGrafici_Raster_Cod = {0} ", Agro_SQL_SaveNum(layer_cod)))
                StrSQL.AppendLine(String.Format(" AND M.TipologiaLayer_Raster_Cod = {0} ", Agro_SQL_SaveNum(tipologia_layer_cod)))
            End If

            StrSQL.AppendLine(" AND ( EXISTS(Select 1 from GIS_MascheraXUtente ")
            StrSQL.AppendLine(String.Format(" Where UserName = '{0}' and Maschera_Cod = M.Maschera_Cod ) ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            If gruppiUtente.Count > 0 Then
                StrSQL.AppendLine("         OR EXISTS(Select 1 from GIS_MascheraXGruppiUtente ")
                StrSQL.AppendLine(String.Format("           Where Gruppi_Utente_cod IN ({0}) and Maschera_Cod = M.Maschera_Cod ) ", String.Join(",", gruppiUtente)))
            End If
            StrSQL.AppendLine(" ) ")


            StrSQL.AppendLine(" ORDER BY ")
            StrSQL.AppendLine(" M.Maschera_Cod, M.LayerElementiGrafici_Raster_Cod, M.LayerElementiGrafici_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiAttivazioneMascheraFiltro(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_R.LeggiAttivazioneMascheraFiltro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     ISNULL(Applica, 0) as Attiva ")
            StrSQL.AppendLine(" FROM GIS_ApplicazioneMaschereUtenti ")
            StrSQL.AppendLine(String.Format(" WHERE PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND Username = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(CostantiPersonalizzate.CodMascheraFiltroMappeSatellitari)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return True
        End If

        Return CBool(DT.Rows(0)("Attiva"))

    End Function

    Public Function VerificaEsistenzaMaschera(ByVal LayerElementiGrafici_Raster_Cod As Int32,
                                              ByVal TipologiaLayer_Raster_Cod As Int32,
                                              ByVal LayerElementiGrafici_Cod As Int32,
                                              ByVal TipologiaLayer_Cod As Int32,
                                              ByRef objParametri As AgronicaCoreParametri) As Int32

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_R.VerificaEsistenzaMaschera()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Maschera_Cod ")
            StrSQL.AppendLine(" FROM GIS_Maschera ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Raster_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_Raster_Cod = {0} ", Agro_SQL_SaveNum(TipologiaLayer_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND TipologiaLayer_Cod = {0} ", Agro_SQL_SaveNum(TipologiaLayer_Cod)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return 0
        End If

        Return CInt(DT.Rows(0)("Maschera_Cod"))

    End Function

End Class
Public Class MaschereLayerRaster_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function InserisciMascheraLayerRaster(ByVal NuovoIDMaschera As Int32,
                                                 ByVal Maschera_Des As String,
                                                 ByVal LayerElementiGrafici_Raster_Cod As Int32,
                                                 ByVal TipologiaLayer_Raster_Cod As Int32,
                                                 ByVal LayerElementiGrafici_Cod As Int32,
                                                 ByVal TipologiaLayer_Cod As Int32,
                                                 ByVal Validita_Inizio As Date,
                                                 ByVal Validita_Fine As Date,
                                                 ByRef ObjParametri As AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_W.InserisciMascheraLayerRaster()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_Maschera ( ")
            StrSQL.AppendLine(" 	PivaSuperUser ")
            StrSQL.AppendLine(" 	, Maschera_Cod ")
            StrSQL.AppendLine(" 	, Maschera_Des ")
            StrSQL.AppendLine(" 	, LayerElementiGrafici_Raster_Cod ")
            StrSQL.AppendLine(" 	, TipologiaLayer_Raster_Cod ")
            StrSQL.AppendLine(" 	, LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	, TipologiaLayer_Cod ")
            StrSQL.AppendLine(" 	, Validita_Inizio ")
            StrSQL.AppendLine(" 	, Validita_Fine ")
            StrSQL.AppendLine(" 	, Username_Creazione ")
            StrSQL.AppendLine(" 	, Username_Modifica ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(ObjParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(NuovoIDMaschera)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Maschera_Des)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(TipologiaLayer_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(TipologiaLayer_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Fine)))

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri.UsernameOperazione)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AggiornaMascheraLayerRaster(ByVal Maschera_Cod As Int32,
                                                ByVal Maschera_Des As String,
                                                ByVal LayerElementiGrafici_Raster_Cod As Int32,
                                                ByVal TipologiaLayer_Raster_Cod As Int32,
                                                ByVal LayerElementiGrafici_Cod As Int32,
                                                ByVal TipologiaLayer_Cod As Int32,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_W.AggiornaMascheraLayerRaster()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_Maschera SET ")
            StrSQL.AppendLine(String.Format(" 	Maschera_Des = '{0}' ", Agro_SQL_SaveText(Maschera_Des)))
            StrSQL.AppendLine(String.Format(" 	, LayerElementiGrafici_Raster_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" 	, TipologiaLayer_Raster_Cod = {0} ", Agro_SQL_SaveNum(TipologiaLayer_Raster_Cod)))
            StrSQL.AppendLine(String.Format(" 	, LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" 	, TipologiaLayer_Cod = {0} ", Agro_SQL_SaveNum(TipologiaLayer_Cod)))
            StrSQL.AppendLine(String.Format(" 	, Validita_Inizio = {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            StrSQL.AppendLine(String.Format(" 	, Validita_Fine  = {0} ", Agro_SQL_SaveDate(Validita_Fine)))
            StrSQL.AppendLine(" 	, Data_Modifica = GETDATE() ")
            StrSQL.AppendLine(String.Format(" 	, Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function EliminaMascheraLayerRaster(ByVal Maschera_Cod As Int32,
                                               ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_W.EliminaMascheraLayerRaster()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE FROM GIS_Maschera ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function InserisciAttivazione(ByVal Maschera_Cod As Int32,
                                         ByVal isAttivo As Boolean,
                                         ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_W.EliminaAttivazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_ApplicazioneMaschereUtenti ( ")
            StrSQL.AppendLine(" PivaSuperUser ")
            StrSQL.AppendLine(" , Maschera_Cod ")
            StrSQL.AppendLine(" , Username ")
            StrSQL.AppendLine(" , Applica ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Maschera_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Convert.ToInt32(isAttivo))))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function EliminaAttivazione(ByVal Maschera_Cod As Int32,
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.MaschereLayerRaster_W.EliminaAttivazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE FROM GIS_ApplicazioneMaschereUtenti ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))
            StrSQL.AppendLine(String.Format(" AND Username = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function

End Class
