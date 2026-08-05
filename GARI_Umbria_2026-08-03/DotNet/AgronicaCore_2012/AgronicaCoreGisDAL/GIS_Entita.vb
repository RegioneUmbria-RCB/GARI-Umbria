Imports System.Data.Entity.Spatial
Imports System.Text
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_Entita_R
    Inherits AgronicaCoreDataProvider.DataProvider2010

    'Proprietà utilizzate da LeggiDocumentiGIS e funzioni in cascata

    Private _filtroTemporaleAvanzato As FiltroTemporaleAvanzato
    Private _dtLayerSingolaData As DataTable
    Private _cfgAlbero As ConfigurazioneAlbero


    'Proprietà sola lettura

    Private ReadOnly _limiteInferiore As String = "limite_inferiore"
    Private ReadOnly _limiteSuperiore As String = "limite_superiore"
    Private ReadOnly _defaultLimiteInferiore As String = "<="
    Private ReadOnly _defaultLimiteSuperiore As String = ">="

    Sub New()
        _filtroTemporaleAvanzato = Nothing
        _dtLayerSingolaData = Nothing
        _cfgAlbero = Nothing
    End Sub

    Public Function leggiABDaDatiImpianto(ByVal piva As String,
                                          ByVal sa_Cod As Int32,
                                          ByVal appezza As Int32,
                                          ByVal id_Imp As Int32,
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.leggiABDaDatiImpianto()"
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New System.Text.StringBuilder

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     E.Entita_Cod ")
            StrSQL.AppendLine("     , ISNULL(E.Entita_GUID, '') as Entita_GUID ")
            StrSQL.AppendLine("     , EG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine("     , EG.ElementoGrafico_Des ")
            StrSQL.AppendLine("     , EG.Poligono_GeoEntity.STAsText() as geoData ")
            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN GIS_ElementiGrafici EG ON EG.Entita_Cod = E.Entita_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" E.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
            StrSQL.AppendLine(String.Format(" AND E.Sa_Cod = {0} ", Agro_SQL_SaveNum(sa_Cod)))
            StrSQL.AppendLine(String.Format(" AND E.Appezza = {0} ", Agro_SQL_SaveNum(appezza)))
            StrSQL.AppendLine(String.Format(" AND E.Id_Imp = {0}", Agro_SQL_SaveNum(id_Imp)))
            StrSQL.AppendLine(String.Format(" AND EG.LayerElementiGrafici_Cod in ({0}, {1})",
                                            CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI),
                                            CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Precision)))

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


    Public Function CatastoSenzaDatoGis(
        ByVal ElementiDaLeggere As Integer,
        ByVal FiltroAggiuntivo As String,
        ByVal objParametri As AgronicaCoreParametri
    ) As String

        Dim DT As String
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.CatastoSenzaDatoGis("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder



            stb.AppendLine("select distinct  ")
            If ElementiDaLeggere <> 0 Then
                stb.AppendLine(" top  " & ElementiDaLeggere)
            End If
            stb.AppendLine("    '@prov' = iPart.prov ")
            stb.AppendLine("  , '@com' = iPart.com ")
            stb.AppendLine("  , '@sezione' = iPart.sezione ")
            stb.AppendLine("  , '@foglio' = iPart.foglio ")
            stb.AppendLine("  , '@numero' = iPart.numero ")
            stb.AppendLine("  , '@subalterno' = iPart.subalterno ")
            stb.AppendLine("  , '@data_modifica' = isnull(g.Data_Modifica, '01/01/1900') ")
            stb.AppendLine("  , '@entita_cod' = isnull(e.entita_cod, 0) ")
            stb.AppendLine("  , '@elementografico_cod' = isnull(g.elementografico_cod, 0) ")

            stb.AppendLine(" from impresexparticelle iPart ")
            stb.AppendLine("  left join GIS_Entita e ")
            stb.AppendLine("      on  ")
            stb.AppendLine("      iPart.PROV = e.PROV ")
            stb.AppendLine("  AND iPart.COM = e.COM ")
            stb.AppendLine("  AND iPart.SEZIONE =  e.SEZIONE ")
            stb.AppendLine("  AND iPart.FOGLIO = e.FOGLIO ")
            stb.AppendLine("  AND iPart.NUMERO = e.NUMERO ")
            stb.AppendLine("  AND iPart.SUBALTERNO = e.SUBALTERNO ")
            stb.AppendLine("  left join GIS_ElementiGrafici g ")
            stb.AppendLine("      on  ")
            stb.AppendLine("      g.entita_cod = e.entita_cod ")

            If FiltroAggiuntivo <> "" Then
                stb.Append(FiltroAggiuntivo)
            End If

            stb.AppendLine(" for xml path('particella'), root('catasto')")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XML(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiValiditaImpiantoDaEntitaGUID(ByVal guidEntita1 As String,
                                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiValiditaImpiantoDaEntitaGUID"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT RI.Validita_Inizio ")
            StrSQL.AppendLine("        , RI.Validita_Fine ")

            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti RI ")
            StrSQL.AppendLine("       ON RI.PIVA = E.Piva ")
            StrSQL.AppendLine("       AND RI.SA_COD = E.Sa_Cod ")
            StrSQL.AppendLine("       AND RI.APPEZZA = E.Appezza ")
            StrSQL.AppendLine("       AND RI.ID_REG = E.Id_Imp ")

            StrSQL.AppendLine(String.Format(" WHERE E.Entita_GUID = '{0}' ", Agro_SQL_SaveText(guidEntita1)))

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

    Public Function LeggiValiditaImpiantoDaEntitaCod(ByVal entitaCod As String,
                                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiValiditaImpiantoDaEntitaCod"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT RI.Validita_Inizio ")
            StrSQL.AppendLine("        , RI.Validita_Fine ")

            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti RI ")
            StrSQL.AppendLine("       ON RI.PIVA = E.Piva ")
            StrSQL.AppendLine("       AND RI.SA_COD = E.Sa_Cod ")
            StrSQL.AppendLine("       AND RI.APPEZZA = E.Appezza ")
            StrSQL.AppendLine("       AND RI.ID_REG = E.Id_Imp ")

            StrSQL.AppendLine(String.Format(" WHERE E.Entita_Cod = {0} ", Agro_SQL_SaveNum(entitaCod)))

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

    Public Function LeggiValiditaEntitaDaEntitaGUID(ByVal guidEntita1 As String,
                                                    ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiValiditaEntitaDaEntitaGUID"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT E.Validita_Inizio ")
            StrSQL.AppendLine("        , E.Validita_Fine ")

            StrSQL.AppendLine(" FROM GIS_Entita E ")

            StrSQL.AppendLine(String.Format(" WHERE E.Entita_GUID = '{0}' ", Agro_SQL_SaveText(guidEntita1)))

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

    Public Function LeggiValiditaEntitaDaEntitaCod(ByVal entitaCod As Int32,
                                                    ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiValiditaEntitaDaEntitaGUID"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT E.Validita_Inizio ")
            StrSQL.AppendLine("        , E.Validita_Fine ")

            StrSQL.AppendLine(" FROM GIS_Entita E ")

            StrSQL.AppendLine(String.Format(" WHERE E.Entita_Cod = {0} ", Agro_SQL_SaveNum(entitaCod)))

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

    Public Function InizializzazioneLetturaParametriCartografici(
        ByVal FiltroAggiuntivo As String,
        ByVal objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder


            stb.AppendLine(" Select top 1  ")
            stb.AppendLine("        geometry::STGeomFromText(Poligono_GeoEntity.STAsText(), 0).STEnvelope().STAsText() As PoligonoWKT      ")
            stb.AppendLine("      , geometry::STGeomFromText(Poligono_GeoEntity.STAsText(), 0).STEnvelope().STCentroid().STAsText() As Center_PoligonoWKT    ")
            stb.AppendLine(" From GIS_ElementiGrafici geodata  ")
            stb.AppendLine(" WHERE ")

            stb.Append(FiltroAggiuntivo)

            stb.AppendLine(" order by Poligono_GeoEntity.STArea()")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiParametriVisualizzazione(ByVal entita_cod As Int32,
                                                  ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiParametriVisualizzazione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ParametriVisualizzazioneLayer ")
            StrSQL.AppendLine(" FROM GIS_Entita ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_cod)))

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

    Public Function AgendaLetturaWKT_DaListaImpianti(
        ByVal ImpiantiFiltroSQL As String,
        ByVal LayerElementiGrafici_cod As enum_Gis_LayerElementiGrafici_std,
        ByVal GIS_TipoOggetto_Cod As enum_Gis_TipoOggetto,
        ByVal objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            stb.AppendLine("  Select ")
            stb.AppendLine("    e.piva ")
            stb.AppendLine("  , e.sa_cod ")
            stb.AppendLine("  , e.Appezza ")
            stb.AppendLine("  , e.Id_Imp ")
            stb.AppendLine("  , e.Entita_Cod ")
            stb.AppendLine("  , g.flag_gps ")
            stb.AppendLine("  , e.id_agenda ")
            stb.AppendLine("  , e.id_mov_det ")
            stb.AppendLine("  , e.tipoentita_cod as GisTipoEntita_cod ")
            stb.AppendLine("  , g.LayerElementiGrafici_Cod as GisLayerCod ")
            stb.AppendLine("  , -1 as GisWktSistemaRiferimento ")


            If GIS_TipoOggetto_Cod = enum_Gis_TipoOggetto.Punto Then
                stb.AppendLine("  , g.Poligono_GeoEntity.EnvelopeCenter().STAsText() as WKT ")
            Else
                stb.AppendLine("  , g.Poligono_GeoEntity.STAsText() as WKT ")
            End If


            stb.AppendLine(" From GIS_Entita e WITH(NOLOCK)")
            stb.AppendLine("  inner Join GIS_ElementiGrafici g WITH(NOLOCK)")
            stb.AppendLine("         On e.Entita_Cod = g.Entita_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine(" where g.LayerElementiGrafici_Cod = " & CInt(LayerElementiGrafici_cod))
            stb.AppendLine(" and " & ImpiantiFiltroSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function BufferZone_CalcolaArea_Da_Entita_Cod(
          ByVal entita_cod As Integer,
          ByVal xFiltroAggiuntivo As String,
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
               ) As Decimal


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            BufferZone_CalcolaArea_Da_Entita_Cod_Qry(entita_cod, stb)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim rval As Decimal = -1
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("Area_Buffer_Zone")
        End If

        Return rval


    End Function


    Public Function VerificaIntersezioneFraDuePoligoniGML(TestaPoligoni As Boolean, Poligono_GeoEntity_1 As String, Poligono_GeoEntity_2 As String, ByVal objParametri_Server As AgronicaCoreParametri) As Boolean


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim xTest As Boolean
        Dim gisW As New GIS_ElementiGrafici_W
        Try

            If TestaPoligoni Then

                xTest =
                    OrientamentoPoligonoTestaERiorienta(Poligono_GeoEntity_1, objParametri_Server)

                xTest =
                OrientamentoPoligonoTestaERiorienta(Poligono_GeoEntity_2, objParametri_Server)
            End If

            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" Declare @P1 varchar(max) ")
            stb.AppendLine(" Declare @P2 varchar(max) ")
            stb.AppendLine(" Set @P1 = '" & Agro_SQL_SaveText(Poligono_GeoEntity_1, False) & "' ")
            stb.AppendLine(" Set @P2 = '" & Agro_SQL_SaveText(Poligono_GeoEntity_2, False) & "' ")
            stb.AppendLine(" Declare @G1 geography ")
            stb.AppendLine(" Declare @G2 geography ")
            stb.AppendLine(" Set @G1 = geography::GeomFromGml(@P1, 4326) ")
            stb.AppendLine(" Set @G2 = geography::GeomFromGml(@P2, 4326) ")
            stb.AppendLine(" Select @G1.STIntersects(@G2) As Intesezione")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim rval As String = ""
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("Intesezione")
        End If

        Return rval



    End Function

    Public Function AreaDaGML(Poligono_GeoEntity As String, ByVal objParametri_Server As AgronicaCoreParametri) As Double


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim xTest As Boolean
        Dim gisW As New GIS_ElementiGrafici_W
        Try

            xTest =
                OrientamentoPoligonoTestaERiorienta(Poligono_GeoEntity, objParametri_Server)

            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" Declare @P varchar(max) ")
            stb.AppendLine(" Set @P = '" & Agro_SQL_SaveText(Poligono_GeoEntity, False) & "' ")
            stb.AppendLine(" Declare @G geography ")
            stb.AppendLine(" Set @G = geography::GeomFromGml(@P, 4326) ")
            stb.AppendLine(" Select round( @G.STArea(), 5) As Area")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim rval As String = ""
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("Area")
        End If

        Return rval



    End Function

    ''' <summary>
    ''' Testa l'orientamento e ri-orienta il poligono
    ''' </summary>
    ''' <param name="Poligono_GeoEntity">Viene Ri-Orientato se il test fallisce</param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Private Shared Function OrientamentoPoligonoTestaERiorienta(ByRef Poligono_GeoEntity As String, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean


        Dim xTest As Boolean
        Dim gisW As New GIS_ElementiGrafici_W

        If Poligono_GeoEntity.StartsWith("<gml:Polygon") Or Poligono_GeoEntity.StartsWith("<gml:LineString") Then
            xTest = gisW.TestaPoligono(Poligono_GeoEntity)
        Else
            xTest = True
        End If


        '1 tentativo con i dati passati
        If Not xTest Then


            '2 tentativo con il poligono invertito
            Dim oldPolygon As String = Poligono_GeoEntity
            Poligono_GeoEntity = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono(Poligono_GeoEntity, True)

            xTest = gisW.TestaPoligono(Poligono_GeoEntity, objParametri_Server)
            If Not xTest Then

                Throw New Exception("Elemento gis non valido")

            End If

        End If
    End Function

    Public Function AreaDaWKT(wkt As String, ByVal objParametri_Server As AgronicaCoreParametri) As Double


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" Declare @P varchar(max) ")
            stb.AppendLine(" Set @P = '" & Agro_SQL_SaveText(wkt, False) & "' ")
            stb.AppendLine(" Declare @G geography ")
            stb.AppendLine(" Set @G = geography:STGeomFromText(@P, 4326) ")
            stb.AppendLine(" Select round( @G.STArea(), 5) As Area")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim rval As String = ""
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("Area")
        End If

        Return rval



    End Function

    Public Function BufferZone_CalcolaArea_Da_Entita_Cod_WKT(
          ByVal entita_cod As Integer,
          ByVal xFiltroAggiuntivo As String,
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
               ) As String


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            BufferZone_CalcolaArea_Da_Entita_Cod_Qry(entita_cod, stb)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Dim rval As String = ""
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("WKT_Buffer_Zone")
        End If

        Return rval


    End Function

    Private Shared Sub BufferZone_CalcolaArea_Da_Entita_Cod_Qry(entita_cod As Integer, stb As StringBuilder)
        stb.Append(" declare @AppezzamentoDaConfrontare geometry " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" --Appezzamento da confontrare " & vbCrLf)
        stb.Append(" select @AppezzamentoDaConfrontare = GEOMETRY::STGeomFromText( poligono_GeoEntity.STAsText(), 4326) " & vbCrLf)
        stb.Append(" from gis_elementigrafici " & vbCrLf)
        stb.Append(" where Entita_Cod = " & entita_cod & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" --Intersezione fra l'appezzamento da confrontare e le buffer zone " & vbCrLf)
        stb.Append(" DECLARE @BufferZoneIntersezione GEOMETRY " & vbCrLf)
        stb.Append(" SET @BufferZoneIntersezione = GEOMETRY::STGeomFromText('GEOMETRYCOLLECTION EMPTY', 4326) " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" select @BufferZoneIntersezione = @BufferZoneIntersezione.STUnion( GEOMETRY::STGeomFromText(poligono_GeoEntity.STAsText() , 4326 ))  " & vbCrLf)
        stb.Append(" from gis_elementigrafici " & vbCrLf)
        stb.Append(" where layerElementigrafici_cod = 86 " & vbCrLf)
        stb.Append(" and @AppezzamentoDaConfrontare.STIntersects( GEOMETRY::STGeomFromText(poligono_GeoEntity.STAsText(), 4326 ) ) > 0 " & vbCrLf)
        stb.Append(" ----- " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" --select @BufferZoneIntersezione, '@BufferZoneIntersezione' as BufferZoneIntersezione " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" --Differenza fra l'appezzamento e la buffer zone (area appezzamento ridotta dalla buffer zone) " & vbCrLf)
        stb.Append(" declare @BufferZoneIntersezione_ConAppezzamento Geometry " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" select @BufferZoneIntersezione_ConAppezzamento =  @AppezzamentoDaConfrontare.STDifference(@BufferZoneIntersezione)  " & vbCrLf)
        stb.Append(" declare @RisultatoFinaleGEO geography " & vbCrLf)
        stb.Append(" set @RisultatoFinaleGEO =  geography::STGeomFromText(  @BufferZoneIntersezione_ConAppezzamento.STAsText(), 4326)  " & vbCrLf)
        stb.Append(" select @RisultatoFinaleGEO.STArea() as Area_Buffer_Zone, @RisultatoFinaleGEO.STAsText() AS WKT_Buffer_Zone")
    End Sub

    Public Function LeggiCompleto_Entita_cod(ByVal Entita_cod As String,
       ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            stb.Append(" select e.* " & vbCrLf)
            stb.Append(" from GIS_Entita e " & vbCrLf)
            stb.Append(" where e.Entita_cod =  " & Agro_SQL_SaveNum(Entita_cod) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function
    

    Public Function LeggiEntitaCod_x_ChiaveAlbero(ByVal Entita_cod As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable


        Dim nomeRoutine = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiEntitaCod_x_ChiaveAlbero("

        Try
            Dim stb As New System.Text.StringBuilder

            stb.Append(" SELECT ")
            stb.Append("   entita.PivaSuperUser AS PivaSuperUser ")
            stb.Append("   ,entita.Entita_Cod AS Entita_Cod ")
            stb.Append("   ,entita.TipoEntita_Cod AS TipoEntita_Cod ")
            stb.Append("   ,entita.Piva AS Piva ")
            stb.Append("   ,entita.Sa_Cod AS Sa_Cod ")
            stb.Append("   ,entita.Appezza AS Appezza ")
            stb.Append("   ,entita.Campo_Cod AS Campo_Cod ")
            stb.Append("   ,entita.Id_Imp AS Id_Imp ")
            stb.Append("   ,entita.PROV AS PROV ")
            stb.Append("   ,entita.COM AS COM ")
            stb.Append("   ,entita.SEZIONE AS SEZIONE ")
            stb.Append("   ,entita.FOGLIO AS FOGLIO ")
            stb.Append("   ,entita.NUMERO AS NUMERO ")
            stb.Append("   ,entita.SUBALTERNO AS SUBALTERNO ")
            stb.Append("   ,entita.Programmazione_Entita_Cod AS Programmazione_Entita_Cod ")
            stb.Append("   ,entita.ID_Agenda AS ID_Agenda ")
            stb.Append("   ,entita.Ricetta_Operazione_cod AS Ricetta_Operazione_cod ")
            stb.Append("   ,entita.analisi_campione_cod AS analisi_campione_cod ")
            stb.Append("   ,entita.programmazione_cod AS programmazione_cod ")
            stb.Append("   ,entita.id_mov_det AS id_mov_det ")
            stb.Append("   ,entita.GIS_Allegati_Documenti_Cod AS GIS_Allegati_Documenti_Cod ")
            stb.Append("   ,entita.Entita_GUID AS Entita_GUID ")
            stb.Append("   ,entita.ParametriVisualizzazioneLayer AS ParametriVisualizzazioneLayer ")
            stb.Append("   ,entita.Area_Cod AS Area_Cod ")
            stb.Append("   ,entita.Fabbricato_Cod AS Fabbricato_Cod ")
            stb.Append("   ,tipoEntita.TipoNodoAlberoAnagrafe AS TipoNodoAlberoAnagrafe ")
            stb.Append(" FROM GIS_Entita entita ")
            stb.Append("   INNER JOIN GIS_TipoEntita tipoEntita ")
            stb.Append("	 ON entita.TipoEntita_Cod = tipoEntita.TipoEntita_Cod ")
            stb.Append(" WHERE entita.Entita_cod = " & Agro_SQL_SaveNum(Entita_cod))

            Dim dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            Return dt
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)

        End Try

    End Function


    Public Function LeggiSmallAppezza(
       ByVal piva As String,
       ByVal sa_cod As String,
       ByVal appezza As String,
       ByVal id_reg As Integer,
       ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder

            stb.Append(" select  " & vbCrLf)
            stb.Append("      g.Poligono_GeoEntity.STAsText() as Oggetto " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.EnvelopeCenter().STAsText() as Envelope " & vbCrLf)
            stb.Append("    , g.Poligono_GeoEntity.STArea() as Superficie " & vbCrLf)
            stb.Append(" from GIS_Entita e " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("        on e.Entita_Cod = g.Entita_Cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where e.piva =  '" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            stb.Append(" and e.sa_Cod = " & sa_cod & vbCrLf)
            stb.Append(" and e.Appezza =  " & appezza & vbCrLf)
            If id_reg <> 0 Then
                stb.Append(" and e.Id_Imp = " & id_reg & vbCrLf)
            End If


            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiIntersezioni(
                ByVal Entita_Cod As Int32,
                ByVal RicercaIntersezioniSuStessoLayer As Boolean,
                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try
            Dim stb As New System.Text.StringBuilder
            LeggiIntersezioniGetQuery(Entita_Cod, stb)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub LeggiIntersezioniGetQuery(ByVal Entita_Cod As Int32, ByRef stb As System.Text.StringBuilder)

        stb.Append(" " & vbCrLf)
        stb.Append(" declare @a geography  " & vbCrLf)
        stb.Append(" declare @Conversione int  " & vbCrLf)
        stb.Append(" set @Conversione = 10000   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" declare @entita_Cod int  " & vbCrLf)
        stb.Append(" set @entita_Cod =  " & Entita_Cod & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" declare @LayerInt int  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" select @a = Poligono_GeoEntity, @layerInt=LayerElementiGrafici_Cod " & vbCrLf)
        stb.Append(" from GIS_ElementiGrafici   " & vbCrLf)
        stb.Append(" where Entita_Cod = @Entita_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" select Entita.* " & vbCrLf)
        stb.Append("   from gis_entita Entita   " & vbCrLf)
        stb.Append("     inner join GIS_ElementiGrafici g    " & vbCrLf)
        stb.Append("         on Entita.Entita_Cod = g.Entita_cod  " & vbCrLf)
        stb.Append("  where @a.STIntersects(Poligono_GeoEntity) = 1        " & vbCrLf)
        stb.Append("  and g.Entita_cod <> @entita_Cod  " & vbCrLf)
        stb.Append("  and g.LayerElementiGrafici_Cod = @layerInt   " & vbCrLf)
        stb.Append("  and not ( " & vbCrLf)
        stb.Append("    @a.STIntersection(Poligono_GeoEntity).STAsText() like 'LineString%' " & vbCrLf)
        stb.Append("    or @a.STIntersection(Poligono_GeoEntity).STAsText() like 'point%' " & vbCrLf)
        stb.Append(" ) " & vbCrLf)


    End Sub

    Public Sub LeggiIntersezioniGetQuery(ByVal LayerIntersecato As enum_Gis_LayerElementiGrafici_std, ByVal Entita_Cod As Int32, ByRef stb As System.Text.StringBuilder)

        stb.Append(" " & vbCrLf)
        stb.Append(" declare @a geography  " & vbCrLf)
        stb.Append(" declare @Conversione int  " & vbCrLf)
        stb.Append(" set @Conversione = 10000   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" declare @entita_Cod int  " & vbCrLf)
        stb.Append(" set @entita_Cod =  " & Entita_Cod & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" declare @LayerInt int  " & vbCrLf)
        stb.Append(" set @LayerInt = " & LayerIntersecato & vbCrLf)
        stb.Append(" select @a = Poligono_GeoEntity " & vbCrLf)
        stb.Append(" from GIS_ElementiGrafici   " & vbCrLf)
        stb.Append(" where Entita_Cod = @Entita_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" select * " & vbCrLf)
        stb.Append("   from gis_entita Entita   " & vbCrLf)
        stb.Append("     inner join GIS_ElementiGrafici g    " & vbCrLf)
        stb.Append("         on Entita.Entita_Cod = g.Entita_cod  " & vbCrLf)
        stb.Append("  where @a.STIntersects(Poligono_GeoEntity) = 1        " & vbCrLf)
        stb.Append("  and g.Entita_cod <> @entita_Cod  " & vbCrLf)
        stb.Append("  and g.LayerElementiGrafici_Cod = @layerInt   " & vbCrLf)
        stb.Append("  and not ( " & vbCrLf)
        stb.Append("    @a.STIntersection(Poligono_GeoEntity).STAsText() like 'LineString%' " & vbCrLf)
        stb.Append("    or @a.STIntersection(Poligono_GeoEntity).STAsText() like 'point%' " & vbCrLf)
        stb.Append(" ) " & vbCrLf)


    End Sub
    ''' <summary>
    ''' Lettura xml per esportazione shp
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <returns></returns>
    Public Function LeggiXDocumentExportSHP(
        ByVal PivaSuperUser As String,
        ByVal CODICE_FISCALE_TECNICO As String,
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal objParametri As AgronicaCoreParametri
    ) As XDocument


        Dim DT As XDocument
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder

        Try


            LeggiGetQueryExportSHP(PivaSuperUser, CODICE_FISCALE_TECNICO, Piva, Sa_Cod, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XDoc(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT





    End Function

    Private Function LeggiGetFiltroAppezzaQueryExportJD(ByVal vChiavi As String()) As String

        Dim l As New List(Of String)
        For Each s In vChiavi
            Dim v1 As String() = s.Split("-")
            l.Add("app.piva = '" & v1(0) & "' AND app.sa_Cod = " & v1(1) & " AND app.appezza = " & v1(2) & vbCrLf)
        Next

        Return String.Join(" OR ", l)

    End Function

    ''' <summary>
    ''' Lettura xml per esportazione shp
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <returns></returns>
    Public Function LeggiXDocumentExportJD(
        ByVal layerLetturaDati As enum_Gis_LayerElementiGrafici_std,
        ByVal FiltroAppezzamenti As String(),
        ByVal objParametri As AgronicaCoreParametri
    ) As XDocument


        Dim DT As XDocument
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder

        Try

            Dim FiltroAggiuntivoAppezzamenti As String =
                LeggiGetFiltroAppezzaQueryExportJD(FiltroAppezzamenti)

            LeggiGetQueryExportJD(layerLetturaDati, FiltroAggiuntivoAppezzamenti, StrSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XDoc(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT





    End Function

    ''' <summary>
    ''' Lettura diretta xml
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="TipoEntita_Cod"></param>
    ''' <param name="PivaPadre">se specificato l'impresa deve appartenere al padre</param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="ID_Imp"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="ID_Agenda"></param>
    ''' <param name="Programmazione_Entita_cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiXDocument(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal PivaPadre As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal ID_Agenda As Int32,
                            ByVal Ricetta_Operazione_Cod As Int32,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_cod As Integer,
                            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                            ByVal Codice_Fiscale_Tecnico As String,
                            ByVal wktBoundaySTIntersects As String,
                            ByVal cfgAlbero As ConfigurazioneAlbero,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal calcola_AreaPerimetro As Boolean = False,
                            Optional ByVal leggiLayerNonVisibili As Boolean = False
            ) As XDocument

        Dim DT As XDocument
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder

        Try

            LeggiGetQuery(PivaSuperUser,
                          Entita_Cod,
                          TipoEntita_Cod,
                          PivaPadre,
                          Piva,
                          Sa_Cod,
                          Appezza,
                          Campo_Cod,
                          ID_Imp,
                          Prov,
                          Com,
                          Sezione,
                          Foglio,
                          Numero,
                          Subalterno,
                          ID_Agenda,
                          Programmazione_Cod,
                          Programmazione_Entita_cod,
                          Ricetta_Operazione_Cod,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri,
                          objParametri_Utenti,
                          StrSQL,
                          True,
                          Sementieri_Sportello_Configurazione_cod,
                          calcola_AreaPerimetro,
                          Codice_Fiscale_Tecnico,
                          wktBoundaySTIntersects,
                          cfgAlbero,
                          leggiLayerNonVisibili)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XDoc(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    ''' <summary>
    ''' Lettura diretta xml
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="TipoEntita_Cod"></param>
    ''' <param name="PivaPadre">se specificato l'impresa deve appartenere al padre</param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="ID_Imp"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="ID_Agenda"></param>
    ''' <param name="Programmazione_Entita_cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiXML(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal PivaPadre As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal ID_Agenda As Int32,
                            ByVal Ricetta_Operazione_Cod As Int32,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_cod As Integer,
                            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                            ByVal Codice_Fiscale_Tecnico As String,
                            ByVal wktBoundaySTIntersects As String,
                            ByVal cfgAlbero As ConfigurazioneAlbero,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal calcola_AreaPerimetro As Boolean = False
            ) As String

        Dim DT As String
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Try

            Dim StrSQL As New System.Text.StringBuilder

            LeggiGetQuery(PivaSuperUser,
                          Entita_Cod,
                          TipoEntita_Cod,
                          PivaPadre,
                          Piva,
                          Sa_Cod,
                          Appezza,
                          Campo_Cod,
                          ID_Imp,
                          Prov,
                          Com,
                          Sezione,
                          Foglio,
                          Numero,
                          Subalterno,
                          ID_Agenda,
                          Programmazione_Cod,
                          Programmazione_Entita_cod,
                          Ricetta_Operazione_Cod,
                          xFiltroAggiuntivo,
                          xOrderBy,
                          objParametri,
                          objParametri_Utenti,
                          StrSQL,
                          True,
                          Sementieri_Sportello_Configurazione_cod,
                          calcola_AreaPerimetro,
                          Codice_Fiscale_Tecnico,
                          wktBoundaySTIntersects,
                          cfgAlbero,
                          False)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XML(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggixEliminaImpiantoPuntiScomposti(
                                    ByVal piva As String,
                                    ByVal sa_cod As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"

        Dim DT As DataTable
        Dim MessaggioErrore As String

        Dim Stb As New System.Text.StringBuilder

        Try

            Stb.Length = 0
            Stb.AppendLine(" SELECT  e.Entita_cod, g.ElementoGrafico_cod ")
            Stb.AppendLine(" FROM    GIS_Entita e")
            Stb.AppendLine("    INNER JOIN    GIS_ElementiGrafici g on e.entita_cod = g.entita_cod")
            Stb.AppendLine(" WHERE G.ElementoGrafico_Des = 'P'  ")
            Stb.AppendLine(" AND G.LayerElementiGrafici_Cod = 4 ")

            Stb.AppendLine(" AND G.Username_Creazione = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")

            If Not String.IsNullOrEmpty(piva) Then
                Stb.AppendLine(" AND E.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If

            If sa_cod <> 0 Then
                Stb.AppendLine(" AND E.Sa_Cod =  " & sa_cod & " ")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function LeggixVecchiaChiaveGis(
                                ByVal entita_cod As Integer,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"

        Dim DT As DataTable
        Dim MessaggioErrore As String

        Dim Stb As New System.Text.StringBuilder

        Try

            Stb.Length = 0
            Stb.Append(" SELECT  e.OLDGrafica_ID")
            Stb.Append(" FROM    GIS_Entita e")
            Stb.Append(" WHERE E.Entita_cod = " & entita_cod)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal id_agenda As Integer,
                            ByVal Ricetta_Operazione_Cod As Integer,
                            ByVal Programmazione_cod As Integer,
                            ByVal Programmazione_entita_cod As Integer,
                            ByVal Codice_Fiscale_Tecnico As String,
                            ByVal wktBoundaySTIntersects As String,
                            ByVal cfgAlbero As ConfigurazioneAlbero,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Entita_Cod = 0
        '
        '   Piva = ""
        '   Sa_Cod = 0
        '   Appezza = 0
        '   Campo_Cod = 0
        '   ID_Imp = 0
        '
        '   Prov = ""
        '   Com = ""
        '   Sezione = "-1"
        '   Foglio = -1
        '   Numero = -1
        '   Subalterno = "-1"
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    LeggiGetQuery(PivaSuperUser,
                                  Entita_Cod,
                                  TipoEntita_Cod,
                                  "",
                                  Piva,
                                  Sa_Cod,
                                  Appezza,
                                  Campo_Cod,
                                  ID_Imp,
                                  Prov,
                                  Com,
                                  Sezione,
                                  Foglio,
                                  Numero,
                                  Subalterno,
                                  id_agenda,
                                  Programmazione_cod,
                                  Programmazione_entita_cod,
                                  Ricetta_Operazione_Cod,
                                  xFiltroAggiuntivo,
                                  xOrderBy,
                                  objParametri,
                                  objParametri_Utenti,
                                  StrSQL,
                                  False,
                                  -1,
                                  False,
                                  Codice_Fiscale_Tecnico,
                                  wktBoundaySTIntersects,
                                  cfgAlbero,
                                  False)

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

            End Select

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

    Public Function LeggiEntitaGuidDaEntitaCod(entitaCod As Int32, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiEntitaGuidDaEntitaCod()"

        Try

            Dim builder As New System.Text.StringBuilder

            builder.AppendLine("SELECT Entita_GUID ")
            builder.AppendLine("FROM GIS_Entita ")
            builder.AppendLine(String.Format("WHERE Entita_Cod = {0} ", Agro_SQL_SaveNum(entitaCod)))

            '--------------------------------------------------------------------------
            Dim dt = EseguiQuery_Lettura(objParametri, builder.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
    
            Return dt
            
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

    End Function

    Public Function LeggiEntitaDaGUID(ByVal entita_GUID As String,
                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiEntitaDaGUID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("Select * ")
            StrSQL.AppendLine("From GIS_Entita ")
            StrSQL.AppendLine(String.Format("WHERE Entita_GUID = '{0}' ", Agro_SQL_SaveText(entita_GUID)))

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

    Public Function LeggiDB(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal id_agenda As Integer,
                            ByVal Ricetta_Operazione_Cod As Integer,
                            ByVal Programmazione_cod As Integer,
                            ByVal Programmazione_entita_cod As Integer,
                            ByVal Codice_Fiscale_Tecnico As String,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal id_mov_det As Integer = 0
                            ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiDB()"

        '====================================================================================
        'Parametri opzionali :
        '   Entita_Cod = 0
        '
        '   Piva = ""
        '   Sa_Cod = 0
        '   Appezza = 0
        '   Campo_Cod = 0
        '   ID_Imp = 0
        '
        '   Prov = ""
        '   Com = ""
        '   Sezione = "-1"
        '   Foglio = -1
        '   Numero = -1
        '   Subalterno = "-1"
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    Stb.AppendLine("Select * ")
                    Stb.AppendLine("From GIS_Entita")
                    Stb.AppendLine("WHERE 1=1 ")

                    If PivaSuperUser <> "" Then
                        Stb.AppendLine(" AND PivaSuperUser = " + Agro_SQL_SaveText_NULL(PivaSuperUser) + " ")
                    End If

                    If Entita_Cod <> 0 Then
                        Stb.AppendLine(" AND Entita_Cod = " + Agro_SQL_SaveNum(Entita_Cod) + " ")
                    End If

                    If TipoEntita_Cod <> 0 Then
                        Stb.AppendLine(" AND TipoEntita_Cod = " + Agro_SQL_SaveNum(TipoEntita_Cod) + " ")
                    End If

                    If Piva <> "" Then
                        Stb.AppendLine(" AND Piva = " + Agro_SQL_SaveText_NULL(Piva) + " ")
                    End If

                    If Sa_Cod <> 0 Then
                        Stb.AppendLine(" AND Sa_Cod = " + Agro_SQL_SaveNum(Sa_Cod) + " ")
                    End If

                    If Appezza <> 0 Then
                        Stb.AppendLine(" AND Appezza = " + Agro_SQL_SaveNum(Appezza) + " ")
                    End If

                    If Campo_Cod <> 0 Then
                        Stb.AppendLine(" AND Campo_Cod = " + Agro_SQL_SaveNum(Campo_Cod) + " ")
                    End If

                    If ID_Imp <> 0 Then
                        Stb.AppendLine(" AND ID_Imp = " + Agro_SQL_SaveNum(ID_Imp) + " ")
                    End If

                    If Prov <> "" Then
                        Stb.AppendLine(" AND Prov = " + Agro_SQL_SaveText_NULL(Prov) + " ")
                    End If

                    If Com <> "" Then
                        Stb.AppendLine(" AND Com = " + Agro_SQL_SaveText_NULL(Com) + " ")
                    End If

                    If Sezione <> "" Then
                        Stb.AppendLine(" AND Sezione = " + Agro_SQL_SaveText_NULL(Sezione) + " ")
                    End If

                    If Foglio <> 0 Then
                        Stb.AppendLine(" AND Foglio = " + Agro_SQL_SaveNum(Foglio) + " ")
                    End If

                    If Numero <> 0 Then
                        Stb.AppendLine(" AND Numero = " + Agro_SQL_SaveNum(Numero) + " ")
                    End If

                    If Subalterno <> "" Then
                        Stb.AppendLine(" AND Subalterno = " + Agro_SQL_SaveText_NULL(Subalterno) + " ")
                    End If

                    If Programmazione_entita_cod <> 0 Then
                        Stb.AppendLine(" AND Programmazione_entita_cod = " + Agro_SQL_SaveNum(Programmazione_entita_cod) + " ")
                    End If

                    If id_agenda <> 0 Then
                        Stb.AppendLine(" AND id_agenda = " + Agro_SQL_SaveNum(id_agenda) + " ")
                    End If

                    If id_mov_det <> 0 Then
                        Stb.AppendLine(" AND id_mov_det = " + Agro_SQL_SaveNum(id_mov_det) + " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        Stb.AppendLine(xFiltroAggiuntivo)
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Private Shared Sub LeggiGetQuery_AnalisiPeriodicitaAgenda_Int(ByRef stb As StringBuilder)
        stb.AppendLine("        Select c.* ")
        stb.AppendLine(" from( ")
        stb.AppendLine(" select  ")
        stb.AppendLine("           a.LAV_COD ")
        stb.AppendLine("      , a.LAV_DES ")
        stb.AppendLine("      , a.ggUltimaOperazione ")
        stb.AppendLine("      , a.Piva ")
        stb.AppendLine("      , a.Sa_Cod ")
        stb.AppendLine("      , a.Appezza ")
        stb.AppendLine("      , a.id_reg       ")
        stb.AppendLine("      , MIN(a.Id_Mov_Det) as id_mov_det ")
        stb.AppendLine("      , a.DataOperazione ")

        stb.AppendLine(" from( ")
        stb.AppendLine("     select  ")
        stb.AppendLine("        o.LAV_COD ")
        stb.AppendLine("      , o.LAV_DES ")
        stb.AppendLine("      , b.ggUltimaOperazione ")
        stb.AppendLine("      , b.Piva ")
        stb.AppendLine("      , b.Sa_Cod ")
        stb.AppendLine("      , b.Appezza ")
        stb.AppendLine("      , b.Id_Destinazione as id_reg ")
        stb.AppendLine("      , a.Id_Mov_Det ")
        stb.AppendLine("      , b.DataOperazione ")

        stb.AppendLine("  from ")
        stb.AppendLine("  ( ")
        stb.AppendLine("  Select ")
        stb.AppendLine("           a1.Lav_Cod ")
        stb.AppendLine("      , a1.Validita_Inizio as DataOperazione ")
        stb.AppendLine("      , d.Piva ")
        stb.AppendLine("      , d.Sa_Cod ")
        stb.AppendLine("      , d.Appezza ")
        stb.AppendLine("      , d.Id_Destinazione ")
        stb.AppendLine("      , d.Id_Mov_Det ")
        stb.AppendLine("  From Agenda a1 ")
        stb.AppendLine("  inner Join Mov_Destinazioni d ")
        stb.AppendLine("             On a1.Id_Agenda = d.Id_Agenda ")
        stb.AppendLine("          And d.Tipo_Destinazione = 0  ")
        stb.AppendLine("   ) a ")
        stb.AppendLine("  inner Join Operazioni o ")
        stb.AppendLine("         On a.Lav_Cod = o.LAV_COD ")
        stb.AppendLine("  inner Join( ")
        stb.AppendLine("         select ")
        stb.AppendLine("          o.LAV_COD ")
        stb.AppendLine("      , o.LAV_DES      ")
        stb.AppendLine("      , d.Piva ")
        stb.AppendLine("      , d.Sa_Cod ")
        stb.AppendLine("      , d.Appezza ")
        stb.AppendLine("      , d.Id_Destinazione      ")
        stb.AppendLine("      , ABS( DATEDIFF(DAY, GETDATE(),  max(a1.Validita_Inizio))) as ggUltimaOperazione ")
        stb.AppendLine("      , MAX( a1.Validita_Inizio) as DataOperazione ")
        stb.AppendLine("      From Agenda a1 ")
        stb.AppendLine("      inner Join Mov_Destinazioni d ")
        stb.AppendLine("             On a1.Id_Agenda = d.Id_Agenda ")
        stb.AppendLine("          And d.Tipo_Destinazione = 0 ")
        stb.AppendLine("      inner Join Operazioni o ")
        stb.AppendLine("             On a1.Lav_Cod = o.LAV_COD ")
        stb.AppendLine("      group by  ")
        stb.AppendLine("           o.LAV_COD ")
        stb.AppendLine("      , o.LAV_DES      ")
        stb.AppendLine("      , d.Piva ")
        stb.AppendLine("      , d.Sa_Cod ")
        stb.AppendLine("      , d.Appezza ")
        stb.AppendLine("      , d.Id_Destinazione ")
        stb.AppendLine("       ")
        stb.AppendLine("      ) b ")
        stb.AppendLine("      On   a.LAV_COD =   b.LAV_COD ")
        stb.AppendLine("      And a.Piva = b.Piva   ")
        stb.AppendLine("      And a.Sa_Cod =  b.Sa_Cod ")
        stb.AppendLine("      And a.Appezza = b.Appezza ")
        stb.AppendLine("      And a.Id_Destinazione = b.Id_Destinazione ")
        stb.AppendLine("      And a.dataOperazione =  b.dataOperazione ")
        stb.AppendLine("  ")
        stb.AppendLine(" ) a ")
        stb.AppendLine(" group by  ")
        stb.AppendLine("  ")
        stb.AppendLine("           a.LAV_COD ")
        stb.AppendLine("      , a.LAV_DES ")
        stb.AppendLine("      , a.ggUltimaOperazione ")
        stb.AppendLine("      , a.Piva ")
        stb.AppendLine("      , a.Sa_Cod ")
        stb.AppendLine("      , a.Appezza ")
        stb.AppendLine("      , a.id_reg       ")
        stb.AppendLine("      , a.DataOperazione       ")
        stb.AppendLine("  ")
        stb.AppendLine(" ) c ")
        stb.AppendLine(" inner Join GIS_Entita e1 ")
        stb.AppendLine("     On e1.Piva = c.Piva  ")
        stb.AppendLine("  And e1.Sa_Cod = c.sa_cod ")
        stb.AppendLine("  And e1.Appezza = c.Appezza ")
        stb.AppendLine("  And e1.Id_Imp = c.id_reg ")
        stb.AppendLine("  And e1.id_mov_det = c.Id_Mov_Det ")
        stb.AppendLine("  ")
        stb.AppendLine(" union ")
        stb.AppendLine("        Select  c.* ")
        stb.AppendLine("  ")
        stb.AppendLine(" from( ")
        stb.AppendLine("      Select   ")
        stb.AppendLine("      113 as LAV_COD ")
        stb.AppendLine("  , 'Rilievo Avversità in campo' as lav_Des   ")
        stb.AppendLine("  , ABS(DATEDIFF(DAY, GETDATE(), Data_Riferimento ) ) as  ggUltimaOperazione  ")
        stb.AppendLine("  , Piva  ")
        stb.AppendLine("  , Sa_Cod  ")
        stb.AppendLine("  , Appezza  ")
        stb.AppendLine("  , id_reg  ")
        stb.AppendLine("  , -1 as Id_Mov_Det  ")
        stb.AppendLine("  , Data_Riferimento as DataOperazione   ")
        stb.AppendLine("  from( ")
        stb.AppendLine("   select piva, Sa_Cod, Appezza, id_reg, Max(srv.Data_Riferimento)  as Data_Riferimento  ")
        stb.AppendLine("      From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo imp  ")
        stb.AppendLine("       inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv  ")
        stb.AppendLine("                  On imp.tipo_sorgente = srv.tipo_sorgente   ")
        stb.AppendLine("                   And imp.Stazione_Cod = srv.stazione_Cod   ")
        stb.AppendLine("   group by imp.piva, imp.sa_cod, imp.appezza, imp.id_reg  ")
        stb.AppendLine("  ) a ")
        stb.AppendLine("  ) c ")
        stb.AppendLine("    inner Join GIS_Entita e1  ")
        stb.AppendLine("      On e1.Piva = c.Piva   ")
        stb.AppendLine("   And e1.Sa_Cod = c.sa_cod  ")
        stb.AppendLine("   And e1.Appezza = c.Appezza  ")
        stb.AppendLine("   And e1.Id_Imp = c.id_reg  ")
        stb.AppendLine("   And ( (c.id_mov_det > 0 And e1.id_mov_det = c.Id_Mov_Det ) Or (c.Id_Mov_Det = -1 And e1.tipoEntita_Cod in (select tipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ))  ")
        stb.AppendLine("  ")
        stb.AppendLine("  union")
        stb.AppendLine("  ")
        stb.AppendLine("                 Select ")
        stb.AppendLine(" 0 as LAV_COD  ")
        stb.AppendLine(" , 'Nessuna Operazione' as LAV_DES     ")
        stb.AppendLine(" , 99999 as ggUltimaOperazione     ")
        stb.AppendLine(" , Piva    ")
        stb.AppendLine(" , Sa_Cod  ")
        stb.AppendLine(" , Appezza     ")
        stb.AppendLine(" , Id_Imp as id_reg    ")
        stb.AppendLine(" , 0 as id_mov_det ")
        stb.AppendLine(" , '01/01/1900' as DataOperazione ")
        stb.AppendLine(" From GIS_Entita e1 ")
        stb.AppendLine(" Where TipoEntita_Cod In (Select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ")
        stb.AppendLine(" And Not exists ( ")
        stb.AppendLine("  Select  1 ")
        stb.AppendLine("     From Mov_Destinazioni c ")
        stb.AppendLine("  Where e1.Piva = c.Piva ")
        stb.AppendLine("  And e1.Sa_Cod = c.sa_cod ")
        stb.AppendLine("  And e1.Appezza = c.Appezza ")
        stb.AppendLine("  And e1.Id_Imp = c.Id_Destinazione ")
        stb.AppendLine("  And c.Tipo_Destinazione  = 0  ")
        stb.AppendLine("  ")
        stb.AppendLine(" ) ")
        stb.AppendLine(" ")
    End Sub

    Private Shared Sub LeggiGetQuery_AnalisiPeriodicitaAgenda(ByRef stb As StringBuilder)

        stb.AppendLine(" Select ")
        stb.AppendLine("        op.LAV_COD ")
        stb.AppendLine("      , op.LAV_DES ")
        stb.AppendLine("      , op.ggUltimaOperazione ")
        stb.AppendLine("      , op.DataOperazione ")
        stb.AppendLine(" from ( ")

        LeggiGetQuery_AnalisiPeriodicitaAgenda_Int(stb)


        stb.AppendLine(" ) op ")

        'stb.AppendLine(" inner Join GIS_Entita e1 ")
        'stb.AppendLine("     On e1.Piva = op.Piva  ")
        'stb.AppendLine("  And e1.Sa_Cod = op.sa_cod ")
        'stb.AppendLine("  And e1.Appezza = op.Appezza ")
        'stb.AppendLine("  And e1.Id_Imp = op.id_reg ")
        'stb.AppendLine("  And e1.id_mov_det = op.Id_Mov_Det")

        stb.AppendLine("   where op.piva = entita.piva ")
        stb.AppendLine("   and op.sa_Cod = entita.sa_Cod ")
        stb.AppendLine("   and op.appezza = entita.appezza ")
        stb.AppendLine("   and op.id_reg = entita.id_imp")
        stb.AppendLine("   and op.id_mov_det = entita.Id_Mov_Det")

        'stb.AppendLine("   and Entita.TipoEntita_Cod in (select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 )")

    End Sub


    Private Shared Sub LeggiGetQuery_Rilievi_AG(ByRef stb As StringBuilder)

        stb.AppendLine("        SELECT    ")
        stb.AppendLine("            ")


        'stb.AppendLine("         case when mav.cod = mav.cod then    ")
        'stb.AppendLine("             ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '')  ")
        'stb.AppendLine("         else   ")
        'stb.AppendLine("             ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '')  ")
        'stb.AppendLine("         end AS UDM_DES   ")

        stb.AppendLine("         ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') AS UDM_DES   ")
        stb.AppendLine("     , case when mav.cod = mav.cod then  op.Qta else '' end as Qta   ")
        stb.AppendLine("            ")
        stb.AppendLine("     , case when mav.cod = mav.cod then    ")
        stb.AppendLine("         coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) )   ")
        stb.AppendLine("     else   ")
        stb.AppendLine("                 ''   ")
        stb.AppendLine("     end AS qta_Descrittiva   ")
        stb.AppendLine("            ")
        stb.AppendLine("     , op.id_agenda   ")
        stb.AppendLine("     , op.Validita_Inizio as DataOperazione     ")
        stb.AppendLine("     , mav.ordine   ")
        stb.AppendLine("            ")
        stb.AppendLine("     , 0 as Ricetta_Operazione_Cod  ")
        stb.AppendLine("            ")
        stb.AppendLine("                 FROM  ")
        stb.AppendLine("            ")
        stb.AppendLine("     (   ")

        stb.AppendLine("  Select  ")
        stb.AppendLine("            " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " as lav_cod ")
        stb.AppendLine("          , mav.cod ")
        stb.AppendLine("          , Avversita.Av_Cod ")
        stb.AppendLine("          , Avversita.Av_Des_Vol ")
        stb.AppendLine("          , UnitaMisura.UDM_COD ")
        stb.AppendLine("          , UnitaMisura.UDM_DES ")
        stb.AppendLine("          , mav.ordine  ")
        stb.AppendLine(" ")

        stb.AppendLine("     from    ")
        stb.AppendLine("     MisuraxAvversita mav    ")
        stb.AppendLine("            ")
        stb.AppendLine("     INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod    ")
        stb.AppendLine("            ")
        stb.AppendLine("     INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod    ")

        stb.AppendLine("        union ")
        stb.AppendLine("  ")
        stb.AppendLine("         Select ")
        stb.AppendLine("          " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & " as lav_cod  ")
        stb.AppendLine("        , 0 as cod  ")
        stb.AppendLine("        ,  id1.ind_mat_Cod as av_cod  ")
        stb.AppendLine("        , id1.[IND_MAT_DES] as av_des_vol ")
        stb.AppendLine("        , UnitaMisura.UDM_COD  ")
        stb.AppendLine("        , UnitaMisura.UDM_DES        ")
        stb.AppendLine("        , 1 as ordine     ")
        stb.AppendLine("       from ")
        stb.AppendLine("             [dbo].[MisuraxIndiciMaturita] mXid  ")
        stb.AppendLine("  ")
        stb.AppendLine("          INNER Join  indiciMaturita id1 ON mXid.[IND_MAT_COD] = id1.[IND_MAT_COD]   ")
        stb.AppendLine("  ")
        stb.AppendLine("           INNER Join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod   ")

        stb.AppendLine("        union ")
        stb.AppendLine("  ")
        stb.AppendLine("         Select ")
        stb.AppendLine("          " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " as lav_cod  ")
        stb.AppendLine("        , 0 as cod  ")
        stb.AppendLine("        ,  id1.ind_mat_Cod as av_cod  ")
        stb.AppendLine("        , id1.[IND_MAT_DES] as av_des_vol ")
        stb.AppendLine("        , UnitaMisura.UDM_COD  ")
        stb.AppendLine("        , UnitaMisura.UDM_DES        ")
        stb.AppendLine("        , 1 as ordine     ")
        stb.AppendLine("       from ")
        stb.AppendLine("             [dbo].[MisuraxIndiciMaturita] mXid  ")
        stb.AppendLine("  ")
        stb.AppendLine("          INNER Join  indiciMaturita id1 ON mXid.[IND_MAT_COD] = id1.[IND_MAT_COD]   ")
        stb.AppendLine("  ")
        stb.AppendLine("           INNER Join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod   ")

        stb.AppendLine("     ) mav   ")

        stb.AppendLine("            ")
        stb.AppendLine("            ")
        stb.AppendLine("     inner join    ")
        stb.AppendLine("     (   ")
        stb.AppendLine("            ")
        stb.AppendLine("            ")
        stb.AppendLine("            ")
        stb.AppendLine("     select    ")
        stb.AppendLine("           Op.Piva    ")
        stb.AppendLine("         , Op.sa_cod  ")
        stb.AppendLine("         , Op.id_Agenda  ")
        stb.AppendLine("         , 0 as Ricetta_Operazione_Cod   ")
        stb.AppendLine("         , Op.Lav_Cod   ")
        stb.AppendLine("         , Op.inviato   ")
        stb.AppendLine("         , dest.mov_destinazioni_graphickey  ")
        stb.AppendLine("         , tec.Av_Cod   ")
        stb.AppendLine("         , case when Lav_Cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " then D.Udm_Cod   else tec.dett_cod end as udm_cod   ")
        stb.AppendLine("         , RegImp.CUL_COD ")
        stb.AppendLine("         , tec.FF_Classe   ")
        stb.AppendLine("         , Op.Validita_Inizio as Data_Movimento  ")
        stb.AppendLine("         , 0 as Sup_Imp   ")
        stb.AppendLine("         , dest.qta as Qta   ")
        stb.AppendLine("         , '' as Via_Stringa   ")
        stb.AppendLine("         , '' as APP_NOME   ")
        stb.AppendLine("         , Op.Username_Modifica   ")
        stb.AppendLine("         , tec.Piezo1   ")
        stb.AppendLine("         , tec.Piezo2 as FF_Classe2   ")
        stb.AppendLine("         , Op.Des_Lib  ")
        stb.AppendLine("         , RegImp.Appezza   ")
        stb.AppendLine("         , RegImp.id_reg   ")
        stb.AppendLine("         , Op.Validita_Inizio   ")
        stb.AppendLine("         , Op.Validita_Fine   ")
        stb.AppendLine("         , Op.Validita_Inizio as Agenda_Validita_Inizio   ")
        stb.AppendLine("         , Op.Validita_Fine as Agenda_Validita_Fine   ")
        stb.AppendLine("         , '' as NoteGenerali   ")
        stb.AppendLine("      ")
        stb.AppendLine("  --select 1 ")
        stb.AppendLine("  from Agenda Op ")
        stb.AppendLine("      inner join Movimenti_dettagli D ")
        stb.AppendLine("          on Op.Id_Agenda = D.Id_Agenda ")
        stb.AppendLine("  ")
        stb.AppendLine("      inner join  Mov_Dettaglio_Tecnico tec ")
        stb.AppendLine("          on Op.Id_Agenda = tec.Id_Agenda ")
        stb.AppendLine("          and D.id_mov = tec.id_mov ")
        stb.AppendLine("          and D.Id_Mov_Det = tec.Id_Mov_Det ")
        stb.AppendLine("      ")

        stb.AppendLine("    inner join mov_Destinazioni dest ")
        stb.AppendLine("      on dest.id_agenda = D.id_agenda ")
        stb.AppendLine("      and dest.id_mov_det = D.id_Mov_det ")
        stb.AppendLine("  ")
        stb.AppendLine("    inner join Reg_Impianti RegImp ")
        stb.AppendLine("      on Regimp.Piva = dest.Piva ")
        stb.AppendLine("      and Regimp.sa_cod = dest.sa_cod ")
        stb.AppendLine("      and Regimp.appezza = dest.appezza ")
        stb.AppendLine("      and Regimp.id_reg = dest.id_destinazione ")
        stb.AppendLine("    where op.lav_cod in (109, 113, 169)  ")

        ' VAnni: 28/11/2019: lettura delle previsioni di indici DSS Difesa
        stb.AppendLine("        union ")
        stb.AppendLine("  ")
        stb.AppendLine("  Select  ")
        stb.AppendLine("    Piva ")
        stb.AppendLine("  , sa_cod     ")
        stb.AppendLine("  , -1 as id_Agenda    ")
        stb.AppendLine("  , 0 as Ricetta_Operazione_Cod    ")
        stb.AppendLine("  , 113 as Lav_Cod     ")
        stb.AppendLine("  , 0 as inviato   ")
        stb.AppendLine("  , '' as mov_destinazioni_graphickey  ")
        stb.AppendLine("  , Av_Cod     ")
        stb.AppendLine("  , udm_cod  --capire ")
        stb.AppendLine("  , CUL_COD    ")
        stb.AppendLine("  , 0 as FF_Classe -- Per adesso no")
        stb.AppendLine("  , CAST(srv.Data_Riferimento as date) as Data_Movimento    ")
        stb.AppendLine("  , 0 as Sup_Imp   ")
        stb.AppendLine("  , Qta_Ril as QTA  --capire  ")
        stb.AppendLine("  , '' as Via_Stringa  ")
        stb.AppendLine("  , '' as APP_NOME     ")
        stb.AppendLine("  , d.Username_Modifica    ")
        stb.AppendLine("  , 0 as Piezo1    ")
        stb.AppendLine("  , 0 as FF_Classe2    ")
        stb.AppendLine("  , '' as Des_Lib  ")
        stb.AppendLine("  , Appezza    ")
        stb.AppendLine("  , id_reg     ")
        stb.AppendLine("  , d.Validita_Inizio  ")
        stb.AppendLine("  , d.Validita_Fine    ")
        stb.AppendLine("  , d.Validita_Inizio as Agenda_Validita_Inizio    ")
        stb.AppendLine("  , d. Validita_Fine as Agenda_Validita_Fine   ")
        stb.AppendLine("  , '' as NoteGenerali ")
        stb.AppendLine("  ")
        stb.AppendLine("  from( ")
        stb.AppendLine("  select d.*, c.Veg_Cod, c.Cul_Cod ")
        stb.AppendLine("     From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d ")
        stb.AppendLine("      inner Join Reg_Impianti i ")
        stb.AppendLine("             On i.PIVA = d.piva  ")
        stb.AppendLine("          And i.SA_COD = d.sa_cod ")
        stb.AppendLine("          And i.APPEZZA = d.appezza ")
        stb.AppendLine("          And i.ID_REG = d.id_reg ")
        stb.AppendLine("      inner Join Cultivar c ")
        stb.AppendLine("             On c.Cul_Cod = i.CUL_COD ")
        stb.AppendLine("  ) d ")
        stb.AppendLine("      inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
        stb.AppendLine("             On d.tipo_sorgente = srv.tipo_sorgente ")
        stb.AppendLine("          And d.Stazione_Cod = srv.stazione_Cod ")
        'stb.AppendLine("      inner Join modelliPrevisionali p ")
        'stb.AppendLine("             On p.Mod_Cod = srv.Mod_Cod ")

        stb.AppendLine("     ) op   ")

        stb.AppendLine("     on ( ( op.av_cod = mav.av_cod and op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & ")")
        stb.AppendLine("        or ( op.FF_Classe = mav.av_cod and op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & " )    ")
        stb.AppendLine("        or ( op.FF_Classe = mav.av_cod and op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " ) )    ")
        stb.AppendLine("     and op.udm_cod = mav.udm_cod   ")
        stb.AppendLine("     and op.lav_cod = mav.lav_cod   ")
        stb.AppendLine("          ")
        stb.AppendLine("     LEFT JOIN Cultivar    ")
        stb.AppendLine("         ON Cultivar.Cul_Cod = op.CUL_COD    ")
        stb.AppendLine("  ")
        stb.AppendLine("     LEFT JOIN SpecieVegetali    ")
        stb.AppendLine("     ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod   ")
        stb.AppendLine("    ")
        stb.AppendLine("     LEFT JOIN Imprese AS i    ")
        stb.AppendLine("     ON i.PIVA = op.PIVA    ")
        stb.AppendLine("  ")
        stb.AppendLine("     LEFT JOIN Centri_Aziendali AS sa    ")
        stb.AppendLine("     ON sa.PIVA = op.PIVA    ")
        stb.AppendLine("     AND sa.sa_cod = op.sa_cod    ")
        stb.AppendLine("  ")
        stb.AppendLine("     LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe   ")
        stb.AppendLine("     LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2   ")
        stb.AppendLine("            ")

        ' VAnni: 24/4/2018: perchè questo join ..? approfondire ... 
        'stb.AppendLine("     LEFT OUTER JOIN   ")
        'stb.AppendLine("         Grafica AS G ON G.Piva = op.PIVA AND G.Sa_Cod = op.Sa_Cod AND G.Id = op.mov_destinazioni_graphickey   ")

        stb.AppendLine("            ")

        stb.AppendLine("     left join ( ")
        stb.AppendLine("        select MxAV_Cod, anag_valore, anag_Des ")
        stb.AppendLine("         From MisuraXAvversita_Anagrafiche ")
        stb.AppendLine("         union ")
        stb.AppendLine("         Select ma.cod As MxAV_Cod, maa.* ")
        stb.AppendLine("         from(select * from  MisuraxAvversita where UDM_COD = 5001049 )  ma ")
        stb.AppendLine("         inner Join( ")
        stb.AppendLine("             select 0 as anag_valore, 'Nessun Rischio'  as anag_Des ")
        stb.AppendLine("             union  ")
        stb.AppendLine("             Select 5 As anag_valore, 'Rischio Medio'  ")
        stb.AppendLine("             union ")
        stb.AppendLine("             Select 10 As anag_valore, 'Rischio Alto' ")
        stb.AppendLine("         ) maa ")
        stb.AppendLine("     On 1=1 ")
        stb.AppendLine("    ) anag ")

        stb.AppendLine("     on anag.MxAV_Cod = mav.COD    ")
        stb.AppendLine("     and anag.Anag_valore = op.Qta   ")
        stb.AppendLine("     WHERE 1=1   ")
        stb.AppendLine("     AND   op.Inviato >=0  ")

        'stb.AppendLine("   and op.id_agenda = entita.id_agenda ")
        stb.AppendLine("   and (op.id_agenda > 0 and op.Id_Agenda = entita.id_agenda or (op.Id_Agenda = -1 and entita.tipoEntita_cod in (select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19) ) ) ")
        stb.AppendLine("   and op.piva = entita.piva ")
        stb.AppendLine("   and op.sa_Cod = entita.sa_Cod ")
        stb.AppendLine("   and op.appezza = entita.appezza ")
        stb.AppendLine("   and op.id_reg = entita.id_imp")

    End Sub

    Private Shared Sub LeggiGetQuery_Rilievi(ByRef stb As StringBuilder)

        stb.Append("SELECT   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         case when mav.cod = mav.cod then   " & vbCrLf)
        stb.Append("                ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') " & vbCrLf)
        stb.Append("            else  " & vbCrLf)
        stb.Append("                ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') " & vbCrLf)
        stb.Append("            end AS UDM_DES  " & vbCrLf)
        stb.Append("        , case when mav.cod = mav.cod then  op.Qta else '' end as Qta  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        , case when mav.cod = mav.cod then   " & vbCrLf)
        stb.Append("            coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) )  " & vbCrLf)
        stb.Append("        else  " & vbCrLf)
        stb.Append("                 ''  " & vbCrLf)
        stb.Append("        end AS qta_Descrittiva  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        , op.id_agenda  " & vbCrLf)
        stb.Append("        , op.Validita_Inizio as DataOperazione  " & vbCrLf)
        stb.Append("        , mav.ordine  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        , op.Ricetta_Operazione_Cod " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("                 FROM " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     (  " & vbCrLf)
        stb.Append("        select mav.cod, Avversita.Av_Cod, Avversita.Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, mav.ordine    " & vbCrLf)
        stb.Append("        from   " & vbCrLf)
        stb.Append("        MisuraxAvversita mav   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod   " & vbCrLf)
        stb.Append("     ) mav  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     inner join   " & vbCrLf)
        stb.Append("     (  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("      select   " & vbCrLf)
        stb.Append("              R.Piva   " & vbCrLf)
        stb.Append("            , R.sa_cod " & vbCrLf)
        stb.Append("            , R.Ricetta_Cod as id_agenda " & vbCrLf)
        stb.Append("            , op.Ricetta_Operazione_Cod  " & vbCrLf)
        stb.Append("            , Op.Lav_Cod  " & vbCrLf)
        stb.Append("            , R.inviato  " & vbCrLf)
        stb.Append("            , tec.Ricette_Dettaglio_Tecnico_GraphicKey as mov_destinazioni_graphickey " & vbCrLf)
        stb.Append("            , tec.Av_Cod  " & vbCrLf)
        stb.Append("            , D.Udm_Cod  " & vbCrLf)
        stb.Append("            , rcul.CUL_COD  " & vbCrLf)
        stb.Append("            , tec.FF_Classe  " & vbCrLf)
        stb.Append("            , R.Validita_Inizio as Data_Movimento " & vbCrLf)
        stb.Append("            , 0 as Sup_Imp  " & vbCrLf)
        stb.Append("            , tec.qta_ril as Qta  " & vbCrLf)
        stb.Append("            , '' as Via_Stringa  " & vbCrLf)
        stb.Append("            , '' as APP_NOME  " & vbCrLf)
        stb.Append("            , R.Username_Modifica  " & vbCrLf)
        stb.Append("            , tec.Piezo1  " & vbCrLf)
        stb.Append("            , tec.Piezo2 as FF_Classe2  " & vbCrLf)
        stb.Append("            , R.Ricetta_Des as Des_Lib " & vbCrLf)
        stb.Append("            , 0 as Appezza  " & vbCrLf)
        stb.Append("            , 0 as id_reg  " & vbCrLf)
        stb.Append("            , R.Validita_Inizio  " & vbCrLf)
        stb.Append("            , R.Validita_Fine  " & vbCrLf)
        stb.Append("            , R.Validita_Inizio as Agenda_Validita_Inizio  " & vbCrLf)
        stb.Append("            , R.Validita_Fine as Agenda_Validita_Fine  " & vbCrLf)
        stb.Append("            , Op.note as NoteGenerali  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        from Ricette R " & vbCrLf)
        stb.Append("        inner join Ricette_Operazioni Op " & vbCrLf)
        stb.Append("            on R.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
        stb.Append("            and R.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
        stb.Append("        inner join Ricette_dettagli D " & vbCrLf)
        stb.Append("            on D.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
        stb.Append("            and D.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        inner join Ricette_Dettaglio_Tecnico tec " & vbCrLf)
        stb.Append("            on D.Ricetta_Cod = tec.Ricetta_Cod " & vbCrLf)
        stb.Append("            and D.Ricetta_SuperUser = tec.Ricetta_superUser " & vbCrLf)
        stb.Append("            and D.Ricetta_Dettaglio_Cod = tec.Ricetta_Dettaglio_Cod " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("        left join RicettexCultivar rcul " & vbCrLf)
        stb.Append("            on rcul.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
        stb.Append("            and rcul.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
        stb.Append("     ) op  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     on op.av_cod = mav.av_cod  " & vbCrLf)
        stb.Append("     and op.udm_cod = mav.udm_cod  " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        'stb.Append("     left join   " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        'stb.Append("     (  " & vbCrLf)
        'stb.Append("        select mav.cod, Avversita.Av_Cod, Avversita.Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, mav.ordine    " & vbCrLf)
        'stb.Append("        from   " & vbCrLf)
        'stb.Append("        MisuraxAvversita mav   " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        'stb.Append("        INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod   " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        'stb.Append("        INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod   " & vbCrLf)

        'stb.Append("        inner join Ricette_Dettaglio_Tecnico t " & vbCrLf)
        'stb.Append("            on t.Av_Cod = mav.AV_COD " & vbCrLf)
        'stb.Append("            and t.Ricetta_Operazione_Cod = entita.Ricetta_Operazione_cod " & vbCrLf)


        'stb.Append("     ) mav  " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        'stb.Append("     on 1=1  " & vbCrLf)
        'stb.Append("  " & vbCrLf)
        stb.Append("     LEFT JOIN Cultivar   " & vbCrLf)
        stb.Append("            ON Cultivar.Cul_Cod = op.CUL_COD   " & vbCrLf)
        stb.Append("     LEFT JOIN SpecieVegetali   " & vbCrLf)
        stb.Append("        ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod   " & vbCrLf)
        stb.Append("     LEFT JOIN Imprese AS i   " & vbCrLf)
        stb.Append("        ON i.PIVA = op.PIVA   " & vbCrLf)
        stb.Append("     LEFT JOIN Centri_Aziendali AS sa   " & vbCrLf)
        stb.Append("        ON sa.PIVA = op.PIVA   " & vbCrLf)
        stb.Append("        AND sa.sa_cod = op.sa_cod   " & vbCrLf)
        stb.Append("     LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe  " & vbCrLf)
        stb.Append("     LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     LEFT OUTER JOIN  " & vbCrLf)
        stb.Append("         Grafica AS G ON G.Piva = op.PIVA AND G.Sa_Cod = op.Sa_Cod AND G.Id = op.mov_destinazioni_graphickey  " & vbCrLf)
        stb.Append("  " & vbCrLf)

        stb.AppendLine("     left join ( ")
        stb.AppendLine("        select MxAV_Cod, anag_valore, anag_Des ")
        stb.AppendLine("         From MisuraXAvversita_Anagrafiche ")
        stb.AppendLine("         union ")
        stb.AppendLine("         Select ma.cod As MxAV_Cod, maa.* ")
        stb.AppendLine("         from(select * from  MisuraxAvversita where UDM_COD = 5001049 )  ma ")
        stb.AppendLine("         inner Join( ")
        stb.AppendLine("             select 0 as anag_valore, 'Nessun Rischio'  as anag_Des ")
        stb.AppendLine("             union  ")
        stb.AppendLine("             Select 5 As anag_valore, 'Rischio Medio'  ")
        stb.AppendLine("             union ")
        stb.AppendLine("             Select 10 As anag_valore, 'Rischio Alto' ")
        stb.AppendLine("         ) maa ")
        stb.AppendLine("     On 1=1 ")
        stb.AppendLine("    ) anag ")

        stb.Append("        on anag.MxAV_Cod = mav.COD   " & vbCrLf)
        stb.Append("        and anag.Anag_valore = op.Qta  " & vbCrLf)
        stb.Append("     WHERE 1=1  " & vbCrLf)
        stb.Append("     AND   op.Inviato >=0 " & vbCrLf)
        stb.Append("     and op.Ricetta_Operazione_Cod = entita.Ricetta_Operazione_Cod  " & vbCrLf)

    End Sub

    Private Shared Sub LeggiGetQuery_infoEstese_AnalisiPeriodicitaAgenda(ByVal Stb As System.Text.StringBuilder)
        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione
        Dim giorniTrascorsi = My.Resources.AgronicaCoreGisDAL.GiorniTrascorsi
        Stb.Append("        , isNull( ( " & vbCrLf)
        Stb.Append("            select a.classi " & vbCrLf)
        Stb.Append("            from  ( " & vbCrLf)
        Stb.Append("                SELECT distinct A1.lav_cod, classi =  " & vbCrLf)
        Stb.Append("                '" & dataOperazione & "§ ' + convert(varchar(50), A1.DataOperazione, 103 ) + '|' +  STUFF(  " & vbCrLf)
        Stb.Append("                    CAST(( " & vbCrLf)
        Stb.Append("                    SELECT '" & giorniTrascorsi & "§  ' +  replace( cast(ggUltimaOperazione as varchar(100)), ',', '.') + '|' as [text()] " & vbCrLf)
        Stb.Append("                    FROM  ( " & vbCrLf)

        LeggiGetQuery_AnalisiPeriodicitaAgenda(Stb)

        Stb.Append("                ) cl " & vbCrLf)

        Stb.Append("                order by cl.lav_Cod, [text()] " & vbCrLf)
        Stb.Append("                FOR XML PATH(''), TYPE) AS " & vbCrLf)
        Stb.Append("                VARCHAR(max) " & vbCrLf)
        Stb.Append("            ), 1, 0, '' " & vbCrLf)
        Stb.Append("        )    " & vbCrLf)
        Stb.Append("    from ( " & vbCrLf)

        LeggiGetQuery_AnalisiPeriodicitaAgenda(Stb)


        Stb.Append("    ) A1 " & vbCrLf)

        'Stb.Append("        where T1.Ricetta_Operazione_Cod = entita.Ricetta_Operazione_cod " & vbCrLf)
        Stb.Append("    ) a  " & vbCrLf)
        Stb.Append(" ) , '' ) as '@info_estese' " & vbCrLf)
        Stb.Append(" ")
    End Sub

    Private Shared Sub LeggiGetQuery_infoEstese_Ag(ByVal Stb As System.Text.StringBuilder)
        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione
        Stb.Append("        , isNull( ( " & vbCrLf)
        Stb.Append("            select a.classi " & vbCrLf)
        Stb.Append("            from  ( " & vbCrLf)
        Stb.Append("                SELECT distinct A1.id_Agenda, classi =  " & vbCrLf)
        Stb.Append("                '" & dataOperazione & "§ ' + convert(varchar(50), A1.DataOperazione, 103 ) + '|' +  STUFF(  " & vbCrLf) 
        Stb.Append("                    CAST(( " & vbCrLf)
        Stb.Append("                    SELECT [udm_des] + '§  ' +  replace( cast(qta as varchar(100)), ',', '.') + '|' as [text()] " & vbCrLf)
        Stb.Append("                    FROM  ( " & vbCrLf)

        LeggiGetQuery_Rilievi(Stb)
        Stb.Append(" union all " & vbCrLf)
        LeggiGetQuery_Rilievi_AG(Stb)

        Stb.Append("                ) cl " & vbCrLf)

        Stb.Append("                order by cl.ordine, cl.id_agenda, [text()] " & vbCrLf)
        Stb.Append("                FOR XML PATH(''), TYPE) AS " & vbCrLf)
        Stb.Append("                VARCHAR(max) " & vbCrLf)
        Stb.Append("            ), 1, 0, '' " & vbCrLf)
        Stb.Append("        )    " & vbCrLf)
        Stb.Append("    from ( " & vbCrLf)

        LeggiGetQuery_Rilievi(Stb)
        Stb.Append(" union all " & vbCrLf)
        LeggiGetQuery_Rilievi_AG(Stb)

        Stb.Append("    ) A1 " & vbCrLf)

        'Stb.Append("        where T1.Ricetta_Operazione_Cod = entita.Ricetta_Operazione_cod " & vbCrLf)
        Stb.Append("    ) a  " & vbCrLf)
        Stb.Append(" ) , '' ) as '@info_estese' " & vbCrLf)
        Stb.Append(" ")
    End Sub


    Private Shared Sub LeggiGetQuery_infoEstese(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("        , isNull( ( " & vbCrLf)
        Stb.Append("            select a.classi " & vbCrLf)
        Stb.Append("            from  ( " & vbCrLf)
        Stb.Append("                SELECT distinct t1.Ricetta_Operazione_Cod, classi =  " & vbCrLf)
        Stb.Append("                STUFF( " & vbCrLf)
        Stb.Append("                    CAST(( " & vbCrLf)
        Stb.Append("                    SELECT [udm_des] + '§  ' +  replace( cast(qta as varchar(100)), ',', '.') + '|' as [text()] " & vbCrLf)
        Stb.Append("                    FROM  ( " & vbCrLf)

        LeggiGetQuery_Rilievi(Stb)

        Stb.Append("                ) cl " & vbCrLf)

        Stb.Append("                order by cl.ordine, cl.id_agenda, cl.Ricetta_Operazione_Cod, [text()] " & vbCrLf)
        Stb.Append("                FOR XML PATH(''), TYPE) AS " & vbCrLf)
        Stb.Append("                VARCHAR(max) " & vbCrLf)
        Stb.Append("            ), 1, 0, '' " & vbCrLf)
        Stb.Append("        )    " & vbCrLf)
        Stb.Append("    from ( " & vbCrLf)

        LeggiGetQuery_Rilievi(Stb)

        Stb.Append("    ) T1 " & vbCrLf)

        'Stb.Append("        where T1.Ricetta_Operazione_Cod = entita.Ricetta_Operazione_cod " & vbCrLf)
        Stb.Append("    ) a  " & vbCrLf)
        Stb.Append(" ) , '' ) as '@info_estese' " & vbCrLf)
        Stb.Append(" ")
    End Sub

    Public Shared Function LeggiGetQuery_DecidiFiltro_Utenti_Visibilita_Appoggio(ByVal ObjParametri_server As AgronicaCoreParametri,
                                                                                 Optional username As String = "") As Boolean

        Dim UtentiVisiblitaAppoggioLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim dt As DataTable =
            UtentiVisiblitaAppoggioLeggi.Leggi(2, "", "", ObjParametri_server, username)

        Return (dt.Rows.Count > 0)
    End Function


    Private Sub LeggiGetQuery_GisEntitaConFiltroTemporale(
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal UtentiVisibilitaAppoggioApplica As Boolean,
        ByVal wktBoundaySTIntersects As String,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal includiJoinTabelleCatasto As Boolean,
        ByVal objParametri As AgronicaCoreParametri,
        ByRef stb As StringBuilder)

        stb.AppendLine(" ( ")

        stb.AppendLine("--IMPIANTI")
        LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
        stb.AppendLine(" union all ")

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--CENTRI AZIENDALI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Centri_Aziendali, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--CATASTO")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.CATASTO, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--TEMA LIBERO")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.TEMALIBERO, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--CAMPI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--ETTARI_EQUIVALENTI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--AREEOMOGENEE")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--UTILIZZODEISUOLI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--IMPIANTI PIANIFICATI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15)) Then
            stb.AppendLine("--APPEZZAMENTI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--OP.AGENDA")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Op_Agenda, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15)) Then
            stb.AppendLine("--AGENDA RILIEVI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        stb.AppendLine("--Mappe di prescrizione")
        LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
        stb.AppendLine(" union all ")

        stb.AppendLine("--Precision farming")
        LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Precision, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
        stb.AppendLine(" union all ")

        stb.AppendLine("--Dettagli Precision farming")
        LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
        stb.AppendLine(" union all ")

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--CAMPIONI ANALISI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 5 AndAlso cfgAlbero.TipologiaLayer_Cod <> 15 AndAlso cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--CAMPIONAMENTI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        '' VAnni: 16/3/2018: al momento il filtro sui percorsi è conditio sine qua non
        Dim ParametriFiltroXSessione As String = ""
        If Not HttpContext.Current Is Nothing Then
            If Not HttpContext.Current.Session Is Nothing Then
                ParametriFiltroXSessione = HttpContext.Current.Session("ParametriFiltroPercorsiXSessione")
            End If
        End If
        If Not String.IsNullOrEmpty(ParametriFiltroXSessione) Then
            stb.AppendLine("--TRATTORI")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Trattori, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod <> 11)) Then
            stb.AppendLine("--DETTAGLIO RICETTA")
            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)
            stb.AppendLine(" union all ")
        End If

        stb.AppendLine("--ALTRO")
        LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, objParametri, -1, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, stb)

        stb.AppendLine(" ) Entita ")

    End Sub


    Private Sub LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery(
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal objParametri As AgronicaCoreParametri,
        ByVal LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
        ByVal UtentiVisibilitaAppoggioApplica As Boolean,
        ByVal wktBoundaySTIntersects As String,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal includiJoinTabelleCatasto As Boolean,
        ByRef stb As StringBuilder)

        Dim TabellaAliasDateValidita As String = ""

        '' VAnni: 16/3/2018: se sul layer dei trattori non ho un filtro allora non carico nulla...
        Dim ParametriFiltroXSessione As String = ""
        If Not HttpContext.Current Is Nothing Then
            If Not HttpContext.Current.Session Is Nothing Then
                ParametriFiltroXSessione = HttpContext.Current.Session("ParametriFiltroPercorsiXSessione")
            End If
        End If

        If String.IsNullOrEmpty(ParametriFiltroXSessione) And LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Trattori Then
            Exit Sub
        End If

        Dim etichetta As String = "''"

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO

                '' VAnni:  6/4/2018:se non ci sono le tabelle in join occorre usare la gis_entita
                If includiJoinTabelleCatasto Then
                    TabellaAliasDateValidita = "pc"
                Else
                    TabellaAliasDateValidita = "e"
                End If

                If LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CATASTO Then
                    etichetta = "CONVERT(VARCHAR(10), e.FOGLIO) + ' - ' + CONVERT(VARCHAR(10), e.NUMERO)"
                End If

            Case enum_Gis_LayerElementiGrafici_std.Centri_Aziendali
                TabellaAliasDateValidita = "sa1"

            Case enum_Gis_LayerElementiGrafici_std.CAMPI
                TabellaAliasDateValidita = "cp"
                etichetta = "cp.Campo_Des"

            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                TabellaAliasDateValidita = "reg1"
                etichetta = "app1.APP_NOME"

            Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                TabellaAliasDateValidita = "ent"
                etichetta = "ent.Entita_Des"

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                TabellaAliasDateValidita = "app1"
                etichetta = "app1.APP_NOME"

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                TabellaAliasDateValidita = "a1"
                etichetta = "app1.APP_NOME + ' - ' + a1.des_lib + ' - ' + CONVERT(varchar(100), a1.Validita_Inizio, 103)"

            Case enum_Gis_LayerElementiGrafici_std.Op_Agenda
                TabellaAliasDateValidita = "a2"
                etichetta = "a2.des_lib + ' - ' + CONVERT(varchar(100), A2.Validita_Inizio, 103)"

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI
                TabellaAliasDateValidita = "Camp1"
                etichetta = "camp1.Analisi_Campione_Des"

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI
                TabellaAliasDateValidita = "Camp1"
                etichetta = "camp1.Analisi_Campione_Des"

            Case enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta
                TabellaAliasDateValidita = "rOperaz"
                etichetta = "rOperaz.ricetta_operazione_des"

            Case enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI
                TabellaAliasDateValidita = "e"
                etichetta = "gel.elementografico_des"

            Case Else
                TabellaAliasDateValidita = "e"

        End Select

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                stb.AppendLine(" select * ")
                stb.AppendLine(" from ( ")

                stb.AppendLine("-- con dato GIS")

                LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_SelectPart(True, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita, etichetta, enum_GIS2012_TipoEntita.Nessuno)

                stb.AppendLine("  from gis_entita e      ")
                stb.AppendLine("  ")
                stb.AppendLine("  inner Join Agenda a1 on   ")
                stb.AppendLine("      a1.piva = e.piva  ")
                stb.AppendLine("      And a1.id_agenda = e.id_agenda  ")
                stb.AppendLine("      inner Join Appezzamento app1 on    ")
                stb.AppendLine("              app1.PIVA = e.Piva  ")
                stb.AppendLine("          And app1.SA_COD = e.Sa_Cod     ")
                stb.AppendLine("          And app1.APPEZZA = e.Appezza     ")
                stb.AppendLine("  ")
                stb.AppendLine("   inner Join GIS_TipoEntita t  ")
                stb.AppendLine("          On e.tipoEntita_cod = t.tipoEntita_Cod ")

                If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then
                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, stb)
                End If
                If UtentiVisibilitaAppoggioApplica Then
                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, stb)
                End If

                If piva <> "" Then

                    stb.Append("    AND  " & vbCrLf)
                    stb.Append("    ( " & vbCrLf)
                    stb.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")

                    If sa_Cod <> 0 Then
                        stb.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                    End If
                    stb.Append("    ) ")

                End If

                LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita)

                stb.AppendLine(" union ")

                stb.AppendLine("-- senza dato GIS")

                LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_SelectPart(False, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita, etichetta, enum_GIS2012_TipoEntita.Destinazione_Agenda)

                stb.AppendLine("  From Mov_Destinazioni d1 ")
                stb.AppendLine("  inner Join Agenda a1 on ")
                stb.AppendLine("       a1.piva = d1.piva ")
                stb.AppendLine("      And a1.id_agenda = d1.id_agenda  ")
                stb.AppendLine(String.Format("      And a1.lav_cod In ({0}, {1}, {2}, {3}) ",
                               CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE,
                               CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA,
                               CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                               CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA))
                stb.AppendLine("  inner join gis_entita e ")
                stb.AppendLine("      on ")
                stb.AppendLine("           d1.PIVA = e.Piva ")
                stb.AppendLine("          And d1.SA_COD = e.Sa_Cod ")
                stb.AppendLine("          And d1.APPEZZA = e.Appezza ")
                stb.AppendLine("          And d1.id_destinazione = e.id_imp ")
                stb.AppendLine("       and e.tipoEntita_cod in (select TipoEntita_Cod From GIS_TipoEntita where LayerElementiGrafici_Cod = 19) ")
                stb.AppendLine("   inner Join Appezzamento app1 on ")
                stb.AppendLine("              app1.PIVA = e.Piva ")
                stb.AppendLine("          And app1.SA_COD = e.Sa_Cod ")
                stb.AppendLine("          And app1.APPEZZA = e.Appezza ")
                stb.AppendLine(" ")

                If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then
                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, stb)
                End If
                If UtentiVisibilitaAppoggioApplica Then
                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, stb)
                End If

                'prima condizione where 
                stb.Append("    where  1=1 " & vbCrLf)

                If piva <> "" Then
                    stb.Append("    and ( " & vbCrLf)
                    stb.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")

                    If sa_Cod <> 0 Then
                        stb.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                    End If
                    stb.Append("    ) ")
                End If

                'non selezionati in parte "con dato GIS"
                stb.AppendLine("  and not exists ( ")
                stb.AppendLine("  select 1 ")
                stb.AppendLine("  from gis_entita e63 ")
                stb.AppendLine("  where e63.id_mov_det = d1.id_mov_det ")
                stb.AppendLine("       And d1.PIVA = e63.Piva ")
                stb.AppendLine("          And d1.SA_COD = e63.Sa_Cod ")
                stb.AppendLine("          And d1.APPEZZA = e63.Appezza ")
                stb.AppendLine("          And d1.id_destinazione = e63.id_imp ")
                stb.AppendLine("       And d1.id_agenda = e63.id_agenda ")
                stb.AppendLine(" ) ")

                LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita)

                stb.AppendLine(" ) e63 ")

            Case Else

                LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_SelectPart(False, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita, etichetta, enum_GIS2012_TipoEntita.Nessuno)

                stb.AppendLine(" From gis_entita e     ")
                stb.AppendLine()

                If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then

                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, stb)

                End If

        End Select

        'altre letture ... 
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI

                stb.AppendLine("inner join gis_elementigrafici gel on gel.entita_cod = e.entita_cod ")

            Case enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE
                'nessun join richiesto

            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO

                If includiJoinTabelleCatasto Then

                    If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
                        stb.AppendLine("inner Join  ( ")
                        stb.AppendLine(" Select * ")
                        stb.AppendLine("     From ( ")
                        stb.AppendLine("        Select Prov, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, 0 as PART_COD  ")
                        stb.AppendLine("        From gis_entita ")
                        stb.AppendLine("        Where TipoEntita_Cod = " & enum_GIS2012_TipoEntita.CATASTO)

                        stb.AppendLine("        union ")
                        stb.AppendLine("        Select Prov, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, PART_COD ")
                        stb.AppendLine("        From particellecatastali")

                        stb.AppendLine(" ) p  ")
                        stb.AppendLine("     inner Join(select '" & Agro_SQL_SaveText(piva) & "' as piva, " & sa_Cod & " as sa_cod) fintaImpresa ")
                        stb.AppendLine(" On 1=1 ")
                        stb.AppendLine(" ) pp")

                    Else
                        If UtentiVisibilitaAppoggioApplica Then
                            stb.AppendLine(" inner Join ( ")
                            stb.AppendLine("     select pp.* ")
                            stb.AppendLine("     From ImpreseXParticelle pp ")
                            stb.AppendLine(" inner join Utenti_Visibilita_Appoggio visParticelle (NOLOCK) ")
                            stb.AppendLine(" on  visParticelle.piva = pp.piva  ")
                            stb.AppendLine(" and visParticelle.sa_Cod = pp.sa_cod")
                            stb.AppendLine(" and visParticelle.Entita_Cod = 2 ")
                            stb.AppendLine(" and visParticelle.pivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                            stb.AppendLine(" and visParticelle.UserName = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
                            stb.AppendLine(" and visParticelle.Appezza = 0 ")
                            stb.AppendLine(" and visParticelle.id_reg = 0 ")
                            stb.AppendLine(" ) pp")

                        Else
                            stb.Append("    inner join ImpreseXParticelle pp " & vbCrLf)
                        End If
                    End If

                    stb.Append("        on pp.prov = e.prov " & vbCrLf)
                    stb.Append("        and pp.com = e.COM  " & vbCrLf)
                    stb.Append("        and pp.SEZIONE = e.SEZIONE  " & vbCrLf)
                    stb.Append("        and pp.Foglio = e.Foglio  " & vbCrLf)
                    stb.Append("        and pp.NUMERO = e.NUMERO  " & vbCrLf)
                    stb.Append("        and pp.SUBALTERNO = e.SUBALTERNO " & vbCrLf)

                    stb.Append("    inner join ParticelleCatastali pc " & vbCrLf)

                    stb.Append("        on  pc.prov = e.prov  " & vbCrLf)
                    stb.Append("        and pc.com = e.COM   " & vbCrLf)
                    stb.Append("        and pc.SEZIONE = e.SEZIONE   " & vbCrLf)
                    stb.Append("        and pc.Foglio = e.Foglio   " & vbCrLf)
                    stb.Append("        and pc.NUMERO = e.NUMERO   " & vbCrLf)
                    stb.Append("        and pc.SUBALTERNO = e.SUBALTERNO " & vbCrLf)

                End If

            Case enum_Gis_LayerElementiGrafici_std.Centri_Aziendali
                stb.AppendLine("     inner Join Centri_Aziendali sa1 on   ")
                stb.AppendLine("                 sa1.PIVA = e.Piva ")
                stb.AppendLine("         And sa1.SA_COD = e.Sa_Cod    ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPI
                'stb.AppendLine("     inner Join Appezzamento app1 on   ")
                'stb.AppendLine("                 app1.PIVA = e.Piva ")
                'stb.AppendLine("         And app1.SA_COD = e.Sa_Cod    ")
                'stb.AppendLine("         And app1.APPEZZA = e.Appezza    ")

                stb.AppendLine("  inner join Campi cp on ")
                stb.AppendLine("                 e.PIVA = cp.Piva ")
                stb.AppendLine("         And e.SA_COD = cp.Sa_Cod    ")
                stb.AppendLine("         And e.Campo_cod = cp.campo_cod ")

            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                stb.AppendLine("    INNER JOIN reg_impianti reg1 ON reg1.PIVA = e.Piva AND reg1.SA_COD = e.Sa_Cod ")
                stb.AppendLine("        AND reg1.APPEZZA = e.Appezza AND reg1.ID_REG = e.id_imp ")
                stb.AppendLine()
                stb.AppendLine("    INNER JOIN Appezzamento app1 ON app1.PIVA = e.Piva AND app1.SA_COD = e.Sa_Cod  AND app1.APPEZZA = e.Appezza ")

            Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                stb.AppendLine("     inner Join programmazione_entita ent   ")
                stb.AppendLine("         on ent.programmazione_entita_cod = e.Programmazione_Entita_COD   ")

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                stb.AppendLine("     inner Join Appezzamento app1 on   ")
                stb.AppendLine("                 app1.PIVA = e.Piva ")
                stb.AppendLine("         And app1.SA_COD = e.Sa_Cod    ")
                stb.AppendLine("         And app1.APPEZZA = e.Appezza    ")
                stb.AppendLine("  ")

            Case enum_Gis_LayerElementiGrafici_std.Op_Agenda
                stb.AppendLine(" inner Join Agenda a2 on  ")
                stb.AppendLine("     a2.piva = e.piva ")
                stb.AppendLine("     And a2.id_agenda = e.id_agenda ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI
                stb.AppendLine(" inner Join Analisi_campioni Camp1 on  ")
                stb.AppendLine("     camp1.Analisi_SuperUser = e.PivaSuperUser ")
                stb.AppendLine("     And camp1.analisi_campione_Cod = e.analisi_campione_Cod  ")
                stb.AppendLine(" inner join Analisi_CampionixDettagli acd ")
                stb.AppendLine(" on acd.analisi_campione_Cod = camp1.analisi_campione_Cod  ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI
                stb.AppendLine(" inner Join Analisi_campioni Camp1 on  ")
                stb.AppendLine("     camp1.Analisi_SuperUser = e.PivaSuperUser ")
                stb.AppendLine("     And camp1.analisi_campione_Cod = e.analisi_campione_Cod  ")
                stb.AppendLine(" inner join Analisi_CampionixDettagli acd ")
                stb.AppendLine(" on acd.analisi_campione_Cod = camp1.analisi_campione_Cod  ")

            Case enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta
                stb.AppendLine("  inner Join Ricette_Operazioni rOperaz  ")
                stb.AppendLine("         On rOperaz.Ricetta_Operazione_cod = e.Ricetta_Operazione_cod  ")

            Case enum_Gis_LayerElementiGrafici_std.Trattori
                stb.AppendLine("  inner Join Gis_elementiGrafici geodata  ")
                stb.AppendLine("         On geodata.Entita_Cod = e.Entita_Cod  ")

            Case Else
                TabellaAliasDateValidita = "e"

        End Select

        If LayerElementiGrafici_Cod <> enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI Then
            stb.AppendLine("  inner Join GIS_TipoEntita t ")
            stb.AppendLine("         On e.tipoEntita_cod = t.tipoEntita_Cod ")
            stb.AppendLine("  ")
        End If

        '' VAnni: 6/4/2018: il filtro su Utenti_Visibilita_Appoggio ora è qui..
        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO,
                 enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                'nessun join richiesto poichè già gestito prima

            Case Else

                If UtentiVisibilitaAppoggioApplica Then
                    LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, stb)
                End If

        End Select

        'condizioni where...

        If LayerElementiGrafici_Cod <> -1 Then

            Select Case LayerElementiGrafici_Cod
                Case enum_Gis_LayerElementiGrafici_std.Trattori
                    stb.AppendLine(" where geodata.LayerElementiGrafici_Cod =  " & LayerElementiGrafici_Cod)
                    ' VAnni: 8/3/2018: filtro dei percorsi..:                        
                    If Not String.IsNullOrEmpty(ParametriFiltroXSessione) Then
                        stb.AppendLine(" AND (" & ParametriFiltroXSessione & ")")
                    End If

                Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI

                Case enum_Gis_LayerElementiGrafici_std.CATASTO
                    stb.AppendLine(" where 1=1 ")

                Case Else
                    stb.AppendLine(" where t.LayerElementiGrafici_Cod =  " & LayerElementiGrafici_Cod)
            End Select

            'If LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Trattori Then
            '    stb.AppendLine(" where geodata.LayerElementiGrafici_Cod =  " & LayerElementiGrafici_Cod)
            '    ' VAnni: 8/3/2018: filtro dei percorsi..:                        
            '    If Not String.IsNullOrEmpty(ParametriFiltroXSessione) Then
            '        stb.AppendLine(" AND (" & ParametriFiltroXSessione & ")")
            '    End If
            'Else
            '    stb.AppendLine(" where t.LayerElementiGrafici_Cod =  " & LayerElementiGrafici_Cod)
            'End If

            If piva <> "" Then

                'verifico piva e sa_cod laddove necessario.
                Select Case LayerElementiGrafici_Cod

                    Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                        enum_Gis_LayerElementiGrafici_std.TEMALIBERO

                        stb.Append("    AND  " & vbCrLf)
                        stb.Append("    ( " & vbCrLf)
                        stb.Append("            pp.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")

                        If sa_Cod <> 0 Then
                            stb.Append("        AND pp.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        stb.Append("    ) ")

                    Case enum_Gis_LayerElementiGrafici_std.Trattori,
                         enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                        'non deve fare nulla, nè passare al case else...

                    Case enum_Gis_LayerElementiGrafici_std.Op_Agenda,
                         enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI

                        stb.Append("    AND  " & vbCrLf)
                        stb.Append("    ( " & vbCrLf)
                        stb.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")

                        If sa_Cod <> 0 Then
                            stb.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " or (e.Piva = '" & Agro_SQL_SaveText(piva) & "' and e.sa_cod = 0) ")
                        End If

                        stb.Append("    ) ")

                    Case Else

                        stb.Append("    AND  " & vbCrLf)
                        stb.Append("    ( " & vbCrLf)
                        stb.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")

                        If sa_Cod <> 0 Then
                            stb.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        stb.Append("    ) ")

                End Select
            End If

        Else
            'quanto già gestito in precedenza lo escludo...
            stb.AppendLine(" where t.LayerElementiGrafici_Cod not in ( ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.CATASTO & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.TEMALIBERO & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Centri_Aziendali & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.IMPIANTI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Op_Agenda & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Trattori & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Precision & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione & ", ")
            stb.AppendLine(enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta & " ")

            stb.AppendLine(" ) and ( ")
            ''catasto
            'stb.AppendLine("  ( e.tipoentita_cod in (5) and  e.Numero > 0  )")

            ''fine catasto

            ''tema libero su catasto
            'stb.AppendLine("  OR (e.tipoentita_cod in (3) )")

            'fine tema libero su catasto

            ' VAnni: 25/1/2017: Nell'interfaccia "scomponi e ricomponi" tenere conto dell'utente che sta utilizzando lo strumento 
            ' vanni, 28/02/2017, verificare questo: and geodata.ElementoGrafico_Des = 'P'
            stb.AppendLine("   (e.tipoentita_cod = 3 and  t.layerElementiGrafici_cod = 4 and e.username_creazione = '" & objParametri.UtenteCodFiscale & "'  ) ") 'testo

            '  Vanni, 22/04/2014 15:25:40: altrimenti non legge ettari equivalenti e aree omogenee
            '  Vanni, 14/04/2015 11:10:22: aggiunto anche layer 60
            '  Vanni, 30/04/2015 18:09:23: rimosso layer 60, da capire se sufficiente la gestione nel filtro temporale
            '  Vanni, 15/02/2016 17:08:17: Stessa cosa per i centri Aziendali

            '  Vanni, 17/08/2016 10:19:23: a differenza di prima, inserisco quanto non governato da condizioni precedenti.
            ' VAnni: 6/3/2017: Aggiunta gestione filtro temporale su campioni analisi
            ' VAnni: 16/3/2018: attenzione, lo zero identifica layer come quello dei trattori non identificati sull'albero..
            ' VAnni: 26/3/2018: aggiunti campi, ettari_equivalenti, aree omogenee, centri_aziendali
            'Stb.Append("  OR (entita.tipoentita_cod in (8, 12, 67)  ) ")  'vecchia versione

            stb.AppendLine("  OR (e.tipoentita_cod not in (3, 5, 8, 12, 13, 17, 18, 19, 33, 23, 20, 19, 22, 21, 1, 50, 55, 51, 54, 67, 0 )  ) " & vbCrLf)  'nuova versione

            'fine
            stb.AppendLine(")")

        End If

        If LayerElementiGrafici_Cod <> enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI Then

            LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, stb, TabellaAliasDateValidita)

        End If

    End Sub

    Private Sub LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(
            objParametri As AgronicaCoreParametri,
            LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
            includiJoinTabelleCatasto As Boolean,
            stb As StringBuilder,
            TabellaAliasDateValidita As String)

        If IsFiltroTemporaleDaApplicare(LayerElementiGrafici_Cod, includiJoinTabelleCatasto) Then

            Dim datiFiltroTemporale = GetDatiFiltroTemporale(LayerElementiGrafici_Cod, objParametri)

            stb.AppendLine(" AND ")

            LeggiGetQueryCondizioniWhere_PorzioneFiltroTemporale(
                TabellaAliasDateValidita,
                "validita_fine",
                False,
                datiFiltroTemporale.finestraTemporaleInizio,
                datiFiltroTemporale.finestraTemporaleFine,
                stb,
                datiFiltroTemporale.operatoreLimiteInferiore,
                datiFiltroTemporale.operatoreLimiteSuperiore,
                LayerElementiGrafici_Cod)

        End If

    End Sub

    Private Sub LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri As AgronicaCoreParametri, stb As StringBuilder)
        stb.AppendLine(" inner join ( ")
        stb.AppendLine(" Select '" & objParametri.PivaSuperUser & "' as pivaSuperUser, '" & objParametri.UtenteUsername & "' as Username, 2 as entita_cod, '' as piva , 0 as sa_cod, 0 as appezza, 0 as id_reg")
        stb.AppendLine(" union all")
        stb.AppendLine(" select * from Utenti_Visibilita_Appoggio (NOLOCK) ")
        stb.AppendLine(") vis ")
        stb.AppendLine(" on  vis.piva = e.piva  ")
        stb.AppendLine(" and vis.sa_Cod = e.sa_cod")
        stb.AppendLine(" and vis.Entita_Cod = 2 ")
        stb.AppendLine(" and vis.pivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
        stb.AppendLine(" and vis.UserName = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
        stb.AppendLine(" and vis.Appezza = 0 ")
        stb.AppendLine(" and vis.id_reg = 0 ")
    End Sub

    Private Shared Sub LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std, cfgAlbero As ConfigurazioneAlbero, stb As StringBuilder)
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI,
                 enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI

                stb.AppendLine(" inner Join __tmp_FiltroImpianti f ")
                stb.AppendLine("   On e.Piva    = f.piva  ")
                stb.AppendLine("  And e.Sa_Cod = f.sa_cod ")
                stb.AppendLine("  And e.Appezza = f.Appezza  ")
                stb.AppendLine("  And e.Id_Imp = f.id_reg")
                stb.AppendLine("  And f.idTestataTemp =  " & cfgAlbero.FiltroImpiantiIdTestataTemp)

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                stb.AppendLine(" inner Join __tmp_FiltroImpianti f ")
                stb.AppendLine("   On e.Piva    = f.piva  ")
                stb.AppendLine("  And e.Sa_Cod = f.sa_cod ")
                stb.AppendLine("  And e.Appezza = f.Appezza  ")
                stb.AppendLine("  And e.Id_Imp = 0 ")
                stb.AppendLine("  And f.idTestataTemp =  " & cfgAlbero.FiltroImpiantiIdTestataTemp)


        End Select
    End Sub

    Private Shared Sub LeggiGetQuery_GisEntitaConFiltroTemporaleGetQuery_SelectPart(
        AgendaRilieviParteGis As Boolean,
        LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
        includiJoinTabelleCatasto As Boolean,
        stb As StringBuilder,
        TabellaAliasDateValidita As String,
        etichetta As String,
        ByVal TipoEntitaCodRiproiettata As enum_GIS2012_TipoEntita)

        stb.AppendLine("  Select ")
        stb.AppendLine("   e.PivaSuperUser ")
        stb.AppendLine(" , e.Entita_Cod ")

        'Gestione delle riproiezioni di layer e di tipo Entità
        If TipoEntitaCodRiproiettata = enum_GIS2012_TipoEntita.Nessuno Then

            'Non richiesta
            stb.AppendLine(" , e.TipoEntita_Cod ")
            stb.AppendLine(" , -1 as layerElementiGrafici_cod_Riproiettato ")

        Else

            'Richiesta riproiezione
            stb.AppendLine(" , " & TipoEntitaCodRiproiettata & " as TipoEntita_Cod ")

            'Gestione specifica per layer

            Select Case LayerElementiGrafici_Cod

                Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                    stb.AppendLine(" , " & enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI & " as layerElementiGrafici_cod_Riproiettato ")

                Case Else
                    stb.AppendLine(" , -1 as layerElementiGrafici_cod_Riproiettato ")

            End Select

        End If

        If ({enum_Gis_LayerElementiGrafici_std.CATASTO,
             enum_Gis_LayerElementiGrafici_std.TEMALIBERO}.Contains(LayerElementiGrafici_Cod)) And includiJoinTabelleCatasto Then

            stb.AppendLine(" , pp.Piva ")
            stb.AppendLine(" , pp.Sa_Cod ")

        Else

            stb.AppendLine(" , e.Piva ")
            stb.AppendLine(" , e.Sa_Cod ")

        End If

        stb.AppendLine(" , e.Appezza ")

        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                stb.AppendLine(" , app1.Campo_Cod ")

            Case Else
                stb.AppendLine(" , e.Campo_Cod ")

        End Select

        stb.AppendLine(" , e.Id_Imp ")
        stb.AppendLine(" , e.PROV ")
        stb.AppendLine(" , e.COM ")
        stb.AppendLine(" , e.SEZIONE ")
        stb.AppendLine(" , e.FOGLIO ")
        stb.AppendLine(" , e.NUMERO ")
        stb.AppendLine(" , e.SUBALTERNO ")
        stb.AppendLine(" , e.Programmazione_Entita_Cod ")

        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI

                If AgendaRilieviParteGis Then
                    stb.AppendLine(" , e.ID_Agenda ")
                Else
                    stb.AppendLine(" , d1.ID_Agenda ")
                End If

            Case Else
                stb.AppendLine(" , e.ID_Agenda ")

        End Select

        stb.AppendLine(" , e.Ricetta_Operazione_cod ")

        If LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI Or
           LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI Then
            stb.AppendLine(" , acd.analisi_testata_cod ")
        Else
            stb.AppendLine(" , 0 as analisi_testata_cod ")
        End If

        stb.AppendLine(" , e.analisi_campione_cod ")
        stb.AppendLine(" , e.OLDGrafica_ID ")
        stb.AppendLine(" , e.inviato ")
        stb.AppendLine(" , e.datainvio ")
        stb.AppendLine(" , e.Data_Creazione ")
        stb.AppendLine(" , e.Data_Modifica ")
        stb.AppendLine(" , e.Username_Creazione ")
        stb.AppendLine(" , e.Username_Modifica ")
        stb.AppendLine(" , " & TabellaAliasDateValidita & ".Validita_Inizio ")
        stb.AppendLine(" , " & TabellaAliasDateValidita & ".Validita_Fine ")
        stb.AppendLine(" , e.programmazione_cod ")
        stb.AppendLine(" , e.id_mov_det ")

        ' VAnni: 6/4/2018: includo per il catasto il codice particella

        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO

                If includiJoinTabelleCatasto Then
                    stb.AppendLine(" , pc.Part_Cod ")
                Else
                    stb.AppendLine(" , -1 as Part_Cod ")
                End If

            Case Else

                stb.AppendLine(" , -1 as Part_Cod ")

        End Select

        stb.AppendLine(" , " & etichetta & " AS ETICHETTA ")

        stb.AppendLine()

    End Sub

    Private Shared Sub LeggiGetQueryExportJD(
        ByVal layerLetturaDati As enum_Gis_LayerElementiGrafici_std,
        ByVal FiltroAggiuntivoImpianti As String,
        ByRef stb As StringBuilder)

        stb.AppendLine("        Select ")

        stb.AppendLine("   'Name§ ' + substring(( isnull(cast(ID as nvarchar(50)), 0) + '-' + isnull( sa.sa_Nome, '') + '-' + isnull( app.App_Nome, '')),1, 500) + '|'   ")
        stb.AppendLine("   as '@text'")

        stb.AppendLine("    , ( select ")
        stb.AppendLine("        geodata1.Poligono_GeoEntity.AsGml() as geodata    ")
        stb.AppendLine("        from GIS_ElementiGrafici geodata1 ")
        stb.AppendLine("             where(geodata1.entita_cod = geg.entita_cod) ")
        stb.AppendLine("    ) as geodata " & vbCrLf)

        stb.AppendLine("    , ( ")
        stb.AppendLine("     Select ")
        stb.AppendLine("        geodata1.Poligono_GeoEntity.STArea() / 10000 As geodata_Area     ")
        stb.AppendLine("         From GIS_ElementiGrafici geodata1  ")
        stb.AppendLine("              Where (geodata1.entita_cod = geg.entita_cod) ")
        stb.AppendLine("             For Xml path('DatiCalcolati'),type, elements  ")
        stb.AppendLine("     ) as geodata")

        stb.AppendLine("    , ( select geg.Entita_Cod for xml path ('DatoGias'), type, elements ) as EntitaGIAS")

        stb.AppendLine("   --query lettura CF ")
        stb.AppendLine("  From Appezzamento app ")
        stb.AppendLine("      inner join Centri_Aziendali sa ")
        stb.AppendLine("        on (app.PIVA=sa.PIVA and app.SA_COD=sa.sa_cod) ")
        stb.AppendLine("      inner join gis_entita geg ")
        stb.AppendLine("          on geg.piva = app.piva ")
        stb.AppendLine("          and geg.sa_cod = app.sa_cod ")
        stb.AppendLine("          and geg.appezza = app.appezza ")
        stb.AppendLine("          and geg.TipoEntita_Cod = 1 ")
        stb.AppendLine("      left join Appezzamento_Codici ic ")
        stb.AppendLine("          on geg.piva = app.piva ")
        stb.AppendLine("          and geg.sa_cod = app.sa_cod ")
        stb.AppendLine("          and geg.appezza = app.appezza ")
        stb.AppendLine("          and ic.id_Cod = 2213 ")
        stb.AppendLine("  WHERE ( ")
        stb.AppendLine(FiltroAggiuntivoImpianti)
        stb.AppendLine("  ) ")


        stb.Append(" for xml path('Entita'), root('DatiEntita'), elements ")

    End Sub


    Private Sub LeggiGetQueryExportSHP( ' the '§' logic will be maintained
        ByVal PivaSuperUser As String,
        ByVal CODICE_FISCALE_TECNICO As String,
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal objParametri As AgronicaCoreParametri,
        ByRef stb As StringBuilder)

        'distinta : + '-' + coalesce( ip.Progetto_Nome, '')

        stb.AppendLine("        Select ")

        stb.AppendLine("   isnull (           'CA_PIVA§ ' + ca.piva   ")
        stb.AppendLine("     + '|CA_RagSo§ ' + coalesce(imp.rag_soc, '')   ")
        stb.AppendLine("     + '|CA_Nome§ ' + coalesce(ca.sa_nome, '')   ")
        stb.AppendLine("     + '|CA_Indir§ ' + coalesce(i.ind_des, '')  ")
        stb.AppendLine("     + '|CA_Com§ ' +  coalesce(ist.localita , '')   ")
        stb.AppendLine("     + '|CA_CAP§ ' +  coalesce(i.CAP , '')  ")
        stb.AppendLine("     + '|CA_Prov§ ' +  coalesce(ist.comuni_prov, '')    ")
        stb.AppendLine("     + '|CA_Stato§ ' +  coalesce(i.stato , '')   ")
        stb.AppendLine("     + '|App_Nome§ ' + coalesce(cc.campo_des, '')  + ' - ' +  coalesce(a.APP_NOME , '')   ")
        stb.AppendLine("     + '|App_Sup§ ' +  cast(a.SUP_APP  as varchar(50))   ")
        stb.AppendLine("     + '|Imp_Sup§ ' +  cast(ri.Sup_Imp as varchar(50))   ")
        stb.AppendLine("     + '|Tip_Var§ ' +    ")
        stb.AppendLine("    ")
        stb.AppendLine("     Case when ri.GRVA_Cod_VEG < 0  ")
        stb.AppendLine("         then gv.grva_des + ' - ibrido'  ")
        stb.AppendLine("         Else gv.grva_des  ")
        stb.AppendLine("              End ")
        stb.AppendLine("    ")
        stb.AppendLine("     + '|Spec_Veg§ ' +  coalesce(sv.Veg_Des, '')   ")
        stb.AppendLine("     + '|Sup_Ggle§ ' +  cast(geg.Poligono_GeoEntity.STArea() as varchar(50))  ")
        stb.AppendLine("     + '|N_Lotto§ ' +  coalesce(ic.val_Cod, '')    ")
        stb.AppendLine("     + '|Note§ ' +  ' '   ")
        stb.AppendLine("     + '|Coord§ ' + cast( round( Centro.Lat, 6) as varchar(100)) + ' ' + cast( round( Centro.Long, 6) as varchar(100)) , '') ")
        stb.AppendLine("   as '@text'")

        stb.Append("    , (select geodata1.Poligono_GeoEntity.AsGml() as geodata    " & vbCrLf)
        stb.Append("        from GIS_ElementiGrafici geodata1 " & vbCrLf)
        stb.Append("             where(geodata1.entita_cod = geg.entita_cod) " & vbCrLf)
        stb.Append("    ) as geodata " & vbCrLf)

        stb.AppendLine("   --query lettura CF ")
        stb.AppendLine("  From ( ")
        stb.AppendLine("     select *  ")
        stb.AppendLine("     From Reg_Impianti ri  ")
        stb.AppendLine("     Where ri.CODICE_FISCALE_TECNICO = '" & Agro_SQL_SaveText(CODICE_FISCALE_TECNICO) & "'  ")
        stb.AppendLine("     and ri.piva = '" & Agro_SQL_SaveText(Piva) & "'  ")

        If Sa_Cod <> 0 Then
            stb.AppendLine("     and ri.sa_cod = " & Sa_Cod & "  ")
        End If

        Dim nessunFiltroTemporale As Boolean = True
        Dim l_inf As String = ""
        Dim l_sup As String = ""
        GetLimitiDaSessione(nessunFiltroTemporale, l_inf, l_sup)

        'TODO: valutare se introdurre la gestione filtro temporale avanzato (vedi GetDatiFiltroTemporale)

        stb.AppendLine(" AND ")

        LeggiGetQueryCondizioniWhere_PorzioneFiltroTemporale(
            "ri",
            "validita_fine",
            False,
            objParametri.FinestraTemporaleInizio,
            objParametri.FinestraTemporaleFine,
            stb,
            l_inf,
            l_sup,
            enum_Gis_LayerElementiGrafici_std.IMPIANTI)

        stb.AppendLine("  ) ri ")
        stb.AppendLine("     INNER Join Centri_Aziendali ca  ")
        stb.AppendLine("         On ri.piva = ca.piva  ")
        stb.AppendLine("         And ri.SA_COD = ca.sa_cod   ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join Imprese imp  ")
        stb.AppendLine("         On imp.PIVA = ca.PIVA   ")
        stb.AppendLine("    ")
        stb.AppendLine("     Left Join Imprese_Codici ic  ")
        stb.AppendLine("          On ic.PIVA = ca.PIVA    ")
        stb.AppendLine("         And ic.id_cod = 1033  ")
        stb.AppendLine("  ")
        stb.AppendLine("     Left Join CentrixIndirizzi CInd  ")
        stb.AppendLine("         On ca.PIVA = CInd.PIVA   ")
        stb.AppendLine("         And ca.sa_cod = CInd.SA_COD  ")
        stb.AppendLine("    ")
        stb.AppendLine("     Left Join Indirizzi i  ")
        stb.AppendLine("         On CInd.cod_indirizzo = i.cod_indirizzo  ")
        stb.AppendLine("    ")
        stb.AppendLine("     Left Join ISTAT ist  ")
        stb.AppendLine("         On i.pro_cod_istat = ist.PROV   ")
        stb.AppendLine("         And i.com_cod_istat = ist.COM  ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join Appezzamento a  ")
        stb.AppendLine("         On ri.piva = a.piva  ")
        stb.AppendLine("         And ri.sa_cod = a.sa_cod  ")
        stb.AppendLine("         And ri.appezza = a.appezza  ")
        stb.AppendLine("  ")
        stb.AppendLine("     inner Join Campi cc ")
        stb.AppendLine("         On cc.Piva = a.PIVA ")
        stb.AppendLine("      And cc.Sa_Cod = a.SA_COD  ")
        stb.AppendLine("      And cc.Campo_Cod = a.Campo_Cod  ")
        stb.AppendLine("  ")
        stb.AppendLine("     INNER Join( ")
        stb.AppendLine("         SELECT 0 as Cul_Cod, 0 as Veg_Cod  ")
        stb.AppendLine("         UNION ")
        stb.AppendLine("                 Select cul.Cul_Cod, cul.Veg_Cod  ")
        stb.AppendLine("         From Cultivar cul  ")
        stb.AppendLine("         ) cul  ")
        stb.AppendLine("         On cul.Cul_Cod = ri.CUL_COD  ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join( ")
        stb.AppendLine("         SELECT 0 as  Grva_Cod, 'non specificato' as  Grva_des  ")
        stb.AppendLine("         UNION  ")
        stb.AppendLine("         Select gv.Grva_Cod, gv.Grva_Des  ")
        stb.AppendLine("         From GruppoVarietale gv  ")
        stb.AppendLine("         ) gv  ")
        stb.AppendLine("         On gv.Grva_Cod = abs(ri.GRVA_Cod_VEG)  ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join SpecieVegetali sv  ")
        stb.AppendLine("         On sv.Veg_Cod = cul.Veg_Cod  ")
        stb.AppendLine("    ")
        stb.AppendLine("  --lettura Sub layer che interseca .. (campi) ")
        stb.AppendLine("     INNER Join gis_entita e  ")
        stb.AppendLine("         On cc.piva = e.Piva  ")
        stb.AppendLine("         And cc.sa_cod = e.Sa_Cod  ")
        stb.AppendLine("      And cc.Campo_Cod = e.Campo_Cod ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join GIS_ElementiGrafici geg  ")
        stb.AppendLine("         On e.entita_cod = geg.Entita_Cod   ")
        stb.AppendLine("    ")
        stb.AppendLine("     INNER Join( ")
        stb.AppendLine("         select entita_cod, Poligono_GeoEntity.EnvelopeCenter() as Centro  ")
        stb.AppendLine("         From gis_elementigrafici   ")
        stb.AppendLine("  ) aEvelope ")
        stb.AppendLine("         On aEvelope.entita_cod = geg.Entita_Cod   ")
        stb.AppendLine("  ")
        'stb.AppendLine("     Left Join imprese_Progetti ip  ")
        'stb.AppendLine("         On ip.Piva = ri.PIVA  ")
        'stb.AppendLine("         And ip.Sa_Cod = ri.SA_COD  ")
        'stb.AppendLine("         And ip.Appezza = ri.APPEZZA  ")
        'stb.AppendLine("         And ip.Id_Reg = ri.id_reg ")

        stb.Append(" for xml path('Entita'), root('DatiEntita'), elements ")

    End Sub

    Private Sub GetLimitiDaSessione(ByRef nessunFiltroTemporale As Boolean,
                                    ByRef l_inf As String,
                                    ByRef l_sup As String)

        nessunFiltroTemporale = True
        l_inf = _defaultLimiteInferiore
        l_sup = _defaultLimiteSuperiore

        If Not HttpContext.Current Is Nothing Then
            If HttpContext.Current.Session IsNot Nothing Then
                nessunFiltroTemporale = IIf(IsNothing(HttpContext.Current.Session(_limiteInferiore)), True, False)
                If Not nessunFiltroTemporale Then
                    l_inf = IIf(IsNothing(HttpContext.Current.Session(_limiteInferiore)), _defaultLimiteInferiore, HttpContext.Current.Session(_limiteInferiore)) & " "
                    l_sup = IIf(IsNothing(HttpContext.Current.Session(_limiteSuperiore)), _defaultLimiteSuperiore, HttpContext.Current.Session(_limiteSuperiore)) & " "
                End If
            End If
        End If

    End Sub

    Private Sub LeggiGetQuery(
                ByVal PivaSuperUser As String,
                ByVal Entita_Cod As Int32,
                ByVal TipoEntita_Cod As Int32,
                ByVal PivaPadre As String,
                ByVal Piva As String,
                ByVal Sa_Cod As Int32,
                ByVal Appezza As Int32,
                ByVal Campo_Cod As Int32,
                ByVal ID_Imp As Int32,
                ByVal Prov As String,
                ByVal Com As String,
                ByVal Sezione As String,
                ByVal Foglio As Int32,
                ByVal Numero As Int32,
                ByVal Subalterno As String,
                ByVal ID_Agenda As Integer,
                ByVal programmazione_cod As Integer,
                ByVal Programmazione_Entita_cod As Integer,
                ByVal Ricetta_Operazione_Cod As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal Stb As System.Text.StringBuilder,
                ByVal isXml As Boolean,
                ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                ByVal CalcolaAreaPerimetro As Boolean,
                ByVal Codice_Fiscale_Tecnico As String,
                ByVal wktBoundaySTIntersects As String,
                ByVal cfgAlbero As ConfigurazioneAlbero,
                ByVal leggiLayerNonVisibili As Boolean
            )

        ' VAnni: 27/11/2017: operazioni preliminari:

        ' Decido se applicare la visibilità (esistono record in tabella Utenti_Visibilita_Appoggio? E se non sono in contesto sementi, dove scattano filtro per codice_fiscale_tecnico)
        Dim UtentiVisibilitaAppoggioApplica As Boolean = False
        If Sementieri_Sportello_Configurazione_cod = -1 Then
            UtentiVisibilitaAppoggioApplica = LeggiGetQuery_DecidiFiltro_Utenti_Visibilita_Appoggio(objParametri)
        End If

        Dim chiamataXAggiornaElencoTipologie As Boolean = False
        If wktBoundaySTIntersects = "chiamataXAggiornaElencoTipologie" Then
            chiamataXAggiornaElencoTipologie = True
            wktBoundaySTIntersects = ""
        End If

        '' VAnni: 6/4/2018: se si richiede il filtro su una particella allora non eseguo il join
        Dim includiJoinTabelleCatasto As Boolean = True
        If Numero > 0 Then
            includiJoinTabelleCatasto = False
        End If

        Dim xLeggiVersioneSql As New DataProvider
        Dim major = xLeggiVersioneSql.VersioneSqlServer_Major(objParametri)

        'GABRIELE -> utilizzo queries ottimizzate...
        Dim test_Ottimizzazione_1 As Boolean = True
        Dim test_Ottimizzazione_2 As Boolean = False

        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            test_Ottimizzazione_2 = True
        End If

        Dim TableOp_LeftJoin As String = ""

        Stb.AppendLine("DECLARE @DataI DATE ")
        Stb.AppendLine("DECLARE @DataF DATE ")
        Stb.AppendLine()
        Stb.AppendLine("SET @DataI = " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio, False))
        Stb.AppendLine("SET @DataF = " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine, False))
        Stb.AppendLine()
        Stb.AppendLine()

        If isXml Then

            'Stb.Append("WITH XMLNAMESPACES (DEFAULT 'http://www.agronica.it/grafica/')   " & vbCrLf)           

            Stb.Append("select   " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    --<Entita/@Attributi> " & vbCrLf)
            Stb.Append("      isNull( geodata.ElementoGrafico_Des, '' ) as '@text'  " & vbCrLf)

            ' VAnni: 21/4/2017: info divise fra angenda e ricette ...
            'LeggiGetQuery_infoEstese(Stb)

            If Not chiamataXAggiornaElencoTipologie Then

                If Not cfgAlbero Is Nothing Then

                    If cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda And test_Ottimizzazione_1 Then

                        'GABRIELE 27-08-2018
                        Dim Stb0 As New System.Text.StringBuilder

                        '**************************************************************************************************
                        'tabella temporanea completa (non esclude le raccolte)
                        '**************************************************************************************************
'                        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione
'                        Dim giorniTrascorsi = My.Resources.AgronicaCoreGisDAL.GiorniTrascorsi
                        'Stb0.Append("Declare @TableOperazioni table(Piva varchar(50), Sa_Cod int, Appezza int, id_reg int, id_mov_det int, LAV_COD int, InfoEstese varchar(max));  " & vbCrLf)
                        'Stb0.Append(" " & vbCrLf)
                        'Stb0.Append(" insert into @TableOperazioni --(Piva, sa_cod, appezza, id_reg, id_mov_det, LAV_COD, InfoEstese)  " & vbCrLf)
                        'Stb0.Append(" " & vbCrLf)
                        'Stb0.Append("     Select op.Piva, op.Sa_Cod, op.Appezza, op.id_reg, op.id_mov_det, op.LAV_COD, '" & dataOperazione & "§ ' + convert(varchar(50), op.DataOperazione, 103 ) + '|' +  '" & giorniTrascorsi & "§  ' +  replace( cast(op.ggUltimaOperazione as varchar(100)), ',', '.') + '|' as InfoEstese " & vbCrLf)
                        'Stb0.Append(" 	from (	Select c.* " & vbCrLf)
                        'Stb0.Append(" 			from (	select a.LAV_COD, a.ggUltimaOperazione, a.Piva, a.Sa_Cod, a.Appezza, a.id_reg, MIN(a.Id_Mov_Det) as id_mov_det, a.DataOperazione " & vbCrLf)
                        'Stb0.Append(" 					from (	select a.LAV_COD, b.ggUltimaOperazione, b.Piva, b.Sa_Cod, b.Appezza, b.Id_Destinazione as id_reg, a.Id_Mov_Det, b.DataOperazione " & vbCrLf)
                        'Stb0.Append(" 							from (	Select a1.Lav_Cod, a1.Validita_Inizio as DataOperazione, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione, d.Id_Mov_Det " & vbCrLf)
                        'Stb0.Append(" 									from Agenda a1 " & vbCrLf)
                        'Stb0.Append(" 									inner join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0) a " & vbCrLf)
                        'Stb0.Append(" 							inner join(    select a1.LAV_COD, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione, ABS(DateDiff(Day, GETDATE(), MAX(a1.Validita_Inizio))) As ggUltimaOperazione, MAX(a1.Validita_Inizio) As DataOperazione " & vbCrLf)
                        'Stb0.Append(" 											From Agenda a1 " & vbCrLf)
                        'Stb0.Append(" 											inner Join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0 " & vbCrLf)
                        'Stb0.Append(" 											group by a1.LAV_COD, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione) b " & vbCrLf)
                        'Stb0.Append(" 							On a.LAV_COD = b.LAV_COD And a.Piva = b.Piva And a.Sa_Cod =  b.Sa_Cod And a.Appezza = b.Appezza And a.Id_Destinazione = b.Id_Destinazione And a.dataOperazione =  b.dataOperazione) a " & vbCrLf)
                        'Stb0.Append(" 							group by a.LAV_COD, a.ggUltimaOperazione, a.Piva, a.Sa_Cod, a.Appezza, a.id_reg, a.DataOperazione) c " & vbCrLf)
                        'Stb0.Append(" 			inner Join GIS_Entita e1 On e1.Piva = c.Piva And e1.Sa_Cod = c.sa_cod And e1.Appezza = c.Appezza And e1.Id_Imp = c.id_reg And e1.id_mov_det = c.Id_Mov_Det " & vbCrLf)
                        'Stb0.Append(" " & vbCrLf)
                        'Stb0.Append(" 			union " & vbCrLf)
                        'Stb0.Append(" " & vbCrLf)
                        'Stb0.Append(" 	        Select 0 As LAV_COD, 99999 As ggUltimaOperazione, Piva, Sa_Cod, Appezza, Id_Imp As id_reg, 0 As id_mov_det, '01/01/1900' as DataOperazione " & vbCrLf)
                        'Stb0.Append(" 			From GIS_Entita e1 " & vbCrLf)
                        'Stb0.Append(" 			Where TipoEntita_Cod In (Select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19) " & vbCrLf)
                        'Stb0.Append(" 			And Not exists (	Select 1 From Mov_Destinazioni c " & vbCrLf)
                        'Stb0.Append(" 								Where e1.Piva = c.Piva And e1.Sa_Cod = c.sa_cod And e1.Appezza = c.Appezza And e1.Id_Imp = c.Id_Destinazione And c.Tipo_Destinazione = 0) " & vbCrLf)
                        'Stb0.Append(" 	) op " & vbCrLf)
                        'Stb0.Append(" " & vbCrLf)

                        '**************************************************************************************************
                        'tabella temporanea con esclusione delle raccolte
                        '**************************************************************************************************
                        Stb0.Append("DECLARE @TableOperazioniTmp table(Piva varchar(50), Sa_Cod int, Appezza int, id_reg int, id_mov_det int, LAV_COD int, DataOperazione datetime, ggUltimaOperazione int) " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("insert into @TableOperazioniTmp " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("Select op.Piva, op.Sa_Cod, op.Appezza, op.id_reg, op.id_mov_det, op.LAV_COD, op.DataOperazione, op.ggUltimaOperazione " & vbCrLf)
                        Stb0.Append("   from (	" & vbCrLf)
                        Stb0.Append("           Select c.* " & vbCrLf)
                        Stb0.Append("           from (	select a.LAV_COD, a.ggUltimaOperazione, a.Piva, a.Sa_Cod, a.Appezza, a.id_reg, MIN(a.Id_Mov_Det) as id_mov_det, a.DataOperazione " & vbCrLf)
                        Stb0.Append("                   from (	select a.LAV_COD, b.ggUltimaOperazione, b.Piva, b.Sa_Cod, b.Appezza, b.Id_Destinazione as id_reg, a.Id_Mov_Det, b.DataOperazione " & vbCrLf)
                        Stb0.Append("                           from (	Select a1.Lav_Cod, a1.Validita_Inizio as DataOperazione, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione, d.Id_Mov_Det " & vbCrLf)
                        Stb0.Append("                                   from Agenda a1 " & vbCrLf)
                        Stb0.Append("                                   inner join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0) a " & vbCrLf)
                        Stb0.Append("                           inner join (	select a1.LAV_COD, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione, ABS(DATEDIFF(DAY, GETDATE(),  max(a1.Validita_Inizio))) as ggUltimaOperazione, MAX( a1.Validita_Inizio) as DataOperazione " & vbCrLf)
                        Stb0.Append("                                           From Agenda a1 " & vbCrLf)
                        Stb0.Append("                                           inner Join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0 " & vbCrLf)
                        Stb0.Append("                                           group by a1.LAV_COD, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione) b " & vbCrLf)
                        Stb0.Append("                           On a.LAV_COD = b.LAV_COD And a.Piva = b.Piva And a.Sa_Cod =  b.Sa_Cod And a.Appezza = b.Appezza And a.Id_Destinazione = b.Id_Destinazione And a.dataOperazione =  b.dataOperazione) a " & vbCrLf)
                        Stb0.Append("                           group by a.LAV_COD, a.ggUltimaOperazione, a.Piva, a.Sa_Cod, a.Appezza, a.id_reg, a.DataOperazione " & vbCrLf)
                        Stb0.AppendLine("     union  ")
                        Stb0.AppendLine("     Select  ")
                        Stb0.AppendLine("     113 as LAV_COD ")
                        Stb0.AppendLine(" , ABS(DATEDIFF(DAY, GETDATE(), Data_Riferimento ) ) as  ggUltimaOperazione ")
                        Stb0.AppendLine(" , Piva ")
                        Stb0.AppendLine(" , Sa_Cod ")
                        Stb0.AppendLine(" , Appezza ")
                        Stb0.AppendLine(" , id_reg ")
                        Stb0.AppendLine(" , -1 as Id_Mov_Det ")
                        Stb0.AppendLine(" , Data_Riferimento as DataOperazione  ")
                        Stb0.AppendLine(" from( ")
                        Stb0.AppendLine("  select piva, Sa_Cod, Appezza, id_reg, Max(srv.Data_Riferimento)  as Data_Riferimento ")
                        Stb0.AppendLine("     From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo imp ")
                        Stb0.AppendLine("      inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
                        Stb0.AppendLine("                 On imp.tipo_sorgente = srv.tipo_sorgente  ")
                        Stb0.AppendLine("                  And imp.Stazione_Cod = srv.stazione_Cod  ")
                        Stb0.AppendLine("  group by imp.piva, imp.sa_cod, imp.appezza, imp.id_reg ")
                        Stb0.AppendLine(" ) a")
                        Stb0.Append("           ) c " & vbCrLf)
                        Stb0.AppendLine("  inner Join GIS_Entita e1 On e1.Piva = c.Piva  ")
                        Stb0.AppendLine("         And e1.Sa_Cod = c.sa_cod  ")
                        Stb0.AppendLine("         And e1.Appezza = c.Appezza  ")
                        Stb0.AppendLine("         And e1.Id_Imp = c.id_reg ")
                        Stb0.AppendLine("         And ( (c.id_mov_det > 0 And e1.id_mov_det = c.Id_Mov_Det ) Or (c.Id_Mov_Det = -1 and e1.tipoEntita_Cod in (select tipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ))")
                        Stb0.Append(") op " & vbCrLf)

                        Dim bEscludiRaccolte As Boolean = True

                        If bEscludiRaccolte Then
                            'esclusione raccolte
                            Stb0.Append(" " & vbCrLf)
                            Stb0.Append("DELETE FROM @TableOperazioniTmp " & vbCrLf)
                            Stb0.Append("where id_mov_det In (  Select id_mov_det from @TableOperazioniTmp t_o " & vbCrLf)
                            Stb0.Append("                       inner join (Select * from ( " & vbCrLf)
                            Stb0.Append("                           Select piva, sa_cod, appezza, id_reg, count(*) As CONTO " & vbCrLf)
                            Stb0.Append("                           from @TableOperazioniTmp " & vbCrLf)
                            Stb0.Append("                           where id_mov_det > 0 " & vbCrLf)
                            Stb0.Append("                           GROUP BY piva, sa_cod, appezza, id_reg) to1 " & vbCrLf)
                            Stb0.Append("                           where to1.CONTO > 1 ) a " & vbCrLf)
                            Stb0.Append("                       On a.piva = t_o.Piva And a.Sa_Cod = t_o.Sa_Cod And a.Appezza = t_o.Appezza " & vbCrLf)
                            Stb0.Append("                       where lav_cod <> " & CostantiPersonalizzate.LAVCOD_RACCOLTA & ") " & vbCrLf)
                            Stb0.Append(" " & vbCrLf)
                        End If

                        
                        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione
                        Dim giorniTrascorsi = My.Resources.AgronicaCoreGisDAL.GiorniTrascorsi
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("Declare @TableOperazioni table(Piva varchar(50), Sa_Cod int, Appezza int, id_reg int, id_mov_det int, LAV_COD int, InfoEstese varchar(max)); " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("insert into @TableOperazioni " & vbCrLf)
                        Stb0.Append("Select Piva, Sa_Cod, Appezza, id_reg, id_mov_det, LAV_COD, '" & dataOperazione & "§ ' + convert(varchar(50), DataOperazione, 103 ) + '|' +  '" & giorniTrascorsi & "§  ' +  replace( cast(ggUltimaOperazione as varchar(100)), ',', '.') + '|' as InfoEstese " & vbCrLf)
                        Stb0.Append("from ( " & vbCrLf)
                        Stb0.Append("   Select * from @TableOperazioniTmp " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("   union " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("   Select e1.Piva, e1.Sa_Cod, e1.Appezza, e1.Id_Imp As id_reg, 0 As id_mov_det, 0 As LAV_COD, '01/01/1900' as DataOperazione, 99999 As ggUltimaOperazione " & vbCrLf)
                        Stb0.Append("   From GIS_Entita e1 " & vbCrLf)
                        Stb0.Append("   Where TipoEntita_Cod In (Select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19) " & vbCrLf)
                        Stb0.Append("   And Not exists (   Select 1 From ( " & vbCrLf)
                        Stb0.AppendLine("                        Select Piva, sa_cod, appezza, id_destinazione , Tipo_Destinazione ")
                        Stb0.AppendLine("                         From Mov_Destinazioni  ")
                        Stb0.AppendLine("                         union ")
                        Stb0.AppendLine("                             Select Piva, Sa_Cod, Appezza, id_Reg As id_destinazione, 0 As tipo_destinazione ")
                        Stb0.AppendLine("                         From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo ")
                        Stb0.Append(" ) c " & vbCrLf)
                        Stb0.Append("	    				Where e1.Piva = c.Piva And e1.Sa_Cod = c.sa_cod And e1.Appezza = c.Appezza And e1.Id_Imp = c.Id_Destinazione And c.Tipo_Destinazione = 0) " & vbCrLf)
                        Stb0.Append(") op " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)

                        Stb.Insert(0, Stb0.ToString)

                        Stb.Append(", isNull( (select InfoEstese from  @TableOperazioni op " & vbCrLf)
                        Stb.Append("            where op.piva = Entita.piva And op.sa_Cod = Entita.sa_Cod And op.appezza = Entita.appezza And op.id_reg = Entita.id_imp And ( (op.id_mov_det > 0 And entita.id_mov_det = op.Id_Mov_Det ) Or (op.Id_Mov_Det = -1 and entita.tipoEntita_Cod in (select tipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ))  ) " & vbCrLf)
                        Stb.Append("            , '') as '@info_estese' " & vbCrLf)

                    ElseIf cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar Then

                        Dim Stb0 As New System.Text.StringBuilder
                        Dim dataRaccolta = My.Resources.AgronicaCoreGisDAL.DataRaccolta

                        Stb0.Append("Declare @TableRaccolta table(Piva varchar(50), Sa_Cod int, Appezza int, id_imp int, DataRaccolta datetime, ggUltimaRaccolta int) " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("insert into @TableRaccolta " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("Select * from ( " & vbCrLf)
                        Stb0.Append("   Select *, DATEDIFF(DAY, DataRaccolta, GETDATE()) as ggUltimaRaccolta from ( " & vbCrLf)
                        Stb0.Append("       Select d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione, max(a1.Validita_Inizio) As DataRaccolta " & vbCrLf)
                        Stb0.Append("       From Agenda a1  " & vbCrLf)
                        Stb0.Append("       inner Join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0 " & vbCrLf)
                        Stb0.Append("       inner Join GIS_Entita e1 On e1.Piva = a1.Piva And e1.Sa_Cod = a1.sa_cod And e1.Appezza = d.Appezza And e1.Id_Imp = d.id_destinazione And e1.id_mov_det = d.Id_Mov_Det " & vbCrLf)
                        Stb0.Append("       where a1.LAV_COD = " & CostantiPersonalizzate.LAVCOD_RACCOLTA & vbCrLf)
                        Stb0.Append("       group by d.piva, d.sa_cod, d.appezza, d.id_destinazione " & vbCrLf)
                        Stb0.Append("   ) tmp1 " & vbCrLf)
                        Stb0.Append(") tmp2 " & vbCrLf)
                        Stb0.Append("where ggUltimaRaccolta <= 180 " & vbCrLf) 'GABRIELE Raccolta avvenuta negli ultimi 6 mesi...

                        Stb.Insert(0, Stb0.ToString)

                        Stb.Append(",isNull( " & vbCrLf)
                        Stb.Append("(select '" & dataRaccolta & "§ ' + convert(varchar(50), DataRaccolta, 103 ) from @TableRaccolta tr where tr.piva = entita.piva and tr.sa_cod = entita.sa_cod and tr.appezza = entita.appezza and tr.id_imp = entita.id_imp) " & vbCrLf)
                        Stb.Append(", '') as '@info_estese' " & vbCrLf)

                    ElseIf ({enum_TipologiaLayer.RilieviVegetoProduttivi, enum_TipologiaLayer.Avversita}).Contains(cfgAlbero.TipologiaLayer_Cod) And test_Ottimizzazione_2 Then

                        'Funzione solo con SQLSERVER2017 -> STRING_AGG(...)

                        'GABRIELE 27-08-2018
                        Dim Stb0 As New System.Text.StringBuilder
                        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione

                        Stb0.Append("Declare @TableOp Table(piva varchar(20), sa_cod int, appezza int, id_reg int, id_agenda int, ricetta_operazione_cod int, info_estese varchar(max), DataOperazione date) " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("insert into @TableOp " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("Select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, '" & dataOperazione & "§ ' + convert(varchar(50), DataOperazione, 103 ) + '|' + descr , DataOperazione " & vbCrLf)
                        Stb0.Append("from ( " & vbCrLf)
                        Stb0.Append("Select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, Dataoperazione, STRING_AGG(UDM_DES + '§  ' + replace( cast(qta as varchar(100)), ',', '.'), '|') as descr " & vbCrLf) 
                        Stb0.Append("	from ( " & vbCrLf)
                        Stb0.Append("        Select '' as piva, 0 as sa_cod, 0 as appezza, 0 as id_reg, op.id_agenda, op.Ricetta_Operazione_Cod, mav.ordine,  op.Validita_Inizio as DataOperazione " & vbCrLf)
                        Stb0.Append("			, ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') + ' [' + mav.veg_des + ']' AS UDM_DES " & vbCrLf)
                        Stb0.Append("			, op.Qta as Qta " & vbCrLf)
                        Stb0.Append("		FROM ( " & vbCrLf)
                        Stb0.Append("			Select mav1.cod, Avversita.Av_Cod, Avversita.Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, mav1.ordine, sv1.veg_des " & vbCrLf)
                        Stb0.Append("			from MisuraxAvversita mav1 " & vbCrLf)
                        Stb0.Append("			INNER JOIN Avversita On mav1.Av_Cod = Avversita.Av_Cod " & vbCrLf)
                        Stb0.Append("			INNER JOIN UnitaMisura On mav1.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
                        Stb0.Append("			INNER JOIN specievegetali sv1 on sv1.veg_cod = mav1.veg_cod " & vbCrLf)
                        Stb0.Append("		) mav " & vbCrLf)
                        Stb0.Append("		INNER JOIN (" & vbCrLf)
                        Stb0.Append("            select " & vbCrLf)
                        Stb0.Append("		          R.Piva " & vbCrLf)
                        Stb0.Append("			    , R.sa_cod " & vbCrLf)
                        Stb0.Append("				, 0 as Appezza " & vbCrLf)
                        Stb0.Append("	            , 0 as id_reg " & vbCrLf)
                        Stb0.Append("		        , R.Ricetta_Cod as id_agenda " & vbCrLf)
                        Stb0.Append("			    , ro.Ricetta_Operazione_Cod " & vbCrLf)
                        Stb0.Append("			    , tec.Av_Cod " & vbCrLf)
                        Stb0.Append("				, rd.Udm_Cod " & vbCrLf)
                        Stb0.Append("	            , tec.qta_ril as Qta " & vbCrLf)
                        Stb0.Append("	            , R.Validita_Inizio " & vbCrLf)
                        Stb0.Append("			from Ricette R " & vbCrLf)
                        Stb0.Append("			inner join Ricette_Operazioni ro On R.Ricetta_Cod = ro.Ricetta_Cod And R.Ricetta_SuperUser = ro.Ricetta_SuperUser " & vbCrLf)
                        Stb0.Append("			inner join Ricette_dettagli rd On rd.Ricetta_Cod = ro.Ricetta_Cod And rd.Ricetta_SuperUser = ro.Ricetta_SuperUser " & vbCrLf)
                        Stb0.Append("			inner join Ricette_Dettaglio_Tecnico tec On rd.Ricetta_Cod = tec.Ricetta_Cod And rd.Ricetta_SuperUser = tec.Ricetta_superUser And rd.Ricetta_Dettaglio_Cod = tec.Ricetta_Dettaglio_Cod " & vbCrLf)
                        Stb0.Append("			where R.Inviato >= 0 " & vbCrLf)
                        Stb0.Append("		) op " & vbCrLf)
                        Stb0.Append("			On op.av_cod = mav.av_cod And op.udm_cod = mav.udm_cod " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)
                        Stb0.Append("        UNION ALL " & vbCrLf)
                        Stb0.Append("  " & vbCrLf)
                        Stb0.Append("       SELECT op.piva, op.sa_Cod, op.appezza, op.id_reg, op.id_agenda, 0 As Ricetta_Operazione_Cod, mav.ordine, op.Validita_Inizio As DataOperazione " & vbCrLf)
                        Stb0.Append("			, ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '')  + mav.veg_des_str AS UDM_DES " & vbCrLf)
                        Stb0.Append("			, op.Qta as Qta " & vbCrLf)
                        Stb0.Append("       FROM ( " & vbCrLf)
                        Stb0.Append("			Select " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " As lav_cod, mav.cod, Avversita.Av_Cod, Avversita.Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, mav.ordine, ' [' + sv2.veg_des + ']' as veg_des_str " & vbCrLf)
                        Stb0.Append("			from MisuraxAvversita mav " & vbCrLf)
                        Stb0.Append("			INNER JOIN specievegetali sv2 on sv2.veg_cod = mav.veg_cod " & vbCrLf)
                        Stb0.Append("			INNER JOIN Avversita On mav.Av_Cod = Avversita.Av_Cod " & vbCrLf)
                        Stb0.Append("			INNER JOIN UnitaMisura On mav.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
                        Stb0.Append("			union " & vbCrLf)
                        Stb0.Append("			Select " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & " As lav_cod, 0 As cod, id1.ind_mat_Cod As av_cod, id1.IND_MAT_DES As av_des_vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, 1 As ordine, '' as veg_des_str " & vbCrLf)
                        Stb0.Append("           From MisuraxIndiciMaturita mXid " & vbCrLf)
                        Stb0.Append("			INNER Join  indiciMaturita id1 On mXid.IND_MAT_COD = id1.IND_MAT_COD " & vbCrLf)
                        Stb0.Append("			INNER Join UnitaMisura On mXid.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
                        Stb0.Append("			union " & vbCrLf)
                        Stb0.Append("			Select " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " As lav_cod, 0 As cod, id1.ind_mat_Cod As av_cod, id1.IND_MAT_DES As av_des_vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, 1 As ordine, '' as veg_des_str " & vbCrLf)
                        Stb0.Append("			from MisuraxIndiciMaturita mXid " & vbCrLf)
                        Stb0.Append("			INNER Join indiciMaturita id1 On mXid.IND_MAT_COD = id1.IND_MAT_COD " & vbCrLf)
                        Stb0.Append("			INNER Join UnitaMisura On mXid.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)
                        Stb0.Append("		) mav " & vbCrLf)
                        Stb0.Append("		INNER JOIN( " & vbCrLf)
                        Stb0.Append("            select " & vbCrLf)
                        Stb0.Append("				  Op.Piva " & vbCrLf)
                        Stb0.Append("				, Op.sa_cod " & vbCrLf)
                        Stb0.Append("			    , RegImp.Appezza " & vbCrLf)
                        Stb0.Append("				, RegImp.id_reg " & vbCrLf)
                        Stb0.Append("				, Op.id_Agenda " & vbCrLf)
                        Stb0.Append("				, 0 as Ricetta_Operazione_Cod " & vbCrLf)
                        Stb0.Append("				, Op.Lav_Cod " & vbCrLf)
                        Stb0.Append("				, tec.Av_Cod " & vbCrLf)
                        Stb0.Append("				, case when Lav_Cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " then D.Udm_Cod else tec.dett_cod end as udm_cod " & vbCrLf)
                        Stb0.Append("				, tec.FF_Classe " & vbCrLf)
                        Stb0.Append("				, dest.qta as Qta " & vbCrLf)
                        Stb0.Append("				, Op.Validita_Inizio " & vbCrLf)
                        Stb0.Append("			from Agenda Op " & vbCrLf)
                        Stb0.Append("			inner join Movimenti_dettagli D On Op.Id_Agenda = D.Id_Agenda " & vbCrLf)
                        Stb0.Append("			inner join Mov_Dettaglio_Tecnico tec On Op.Id_Agenda = tec.Id_Agenda And D.id_mov = tec.id_mov And D.Id_Mov_Det = tec.Id_Mov_Det " & vbCrLf)
                        Stb0.Append("			inner join mov_Destinazioni dest On dest.id_agenda = D.id_agenda And dest.id_mov_det = D.id_Mov_det " & vbCrLf)
                        Stb0.Append("			inner join Reg_Impianti RegImp On Regimp.Piva = dest.Piva And Regimp.sa_cod = dest.sa_cod And Regimp.appezza = dest.appezza And Regimp.id_reg = dest.id_destinazione " & vbCrLf)
                        Stb0.Append("			where Op.lav_cod In (" & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & ", " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & ", " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & ") " & vbCrLf)
                        Stb0.Append("			        And Op.inviato >= 0 " & vbCrLf)

                        ' VAnni: 28/11/2019: lettura delle previsioni di indici DSS Difesa
                        Stb0.AppendLine("        union ")
                        Stb0.AppendLine("  ")
                        Stb0.AppendLine("  Select  ")
                        Stb0.AppendLine("    Piva ")
                        Stb0.AppendLine("  , sa_cod     ")
                        Stb0.AppendLine("  , Appezza    ")
                        Stb0.AppendLine("  , id_reg     ")
                        Stb0.AppendLine("  , -1 as id_Agenda    ")
                        Stb0.AppendLine("  , 0 as Ricetta_Operazione_Cod    ")
                        Stb0.AppendLine("  , 113 as Lav_Cod     ")
                        Stb0.AppendLine("  , Av_Cod     ")
                        Stb0.AppendLine("  , udm_cod  --capire ")
                        Stb0.AppendLine("  , 0 as FF_Classe -- Per adesso no")
                        Stb0.AppendLine("  , Qta_Ril as QTA  --capire  ")
                        Stb0.AppendLine("  , CAST(srv.Data_Riferimento as date) as Validita_Inizio    ")
                        Stb0.AppendLine("  ")
                        Stb0.AppendLine("  from( ")
                        Stb0.AppendLine("  select d.*, c.Veg_Cod, c.Cul_Cod ")
                        Stb0.AppendLine("     From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d ")
                        Stb0.AppendLine("      inner Join Reg_Impianti i ")
                        Stb0.AppendLine("             On i.PIVA = d.piva  ")
                        Stb0.AppendLine("          And i.SA_COD = d.sa_cod ")
                        Stb0.AppendLine("          And i.APPEZZA = d.appezza ")
                        Stb0.AppendLine("          And i.ID_REG = d.id_reg ")
                        Stb0.AppendLine("      inner Join Cultivar c ")
                        Stb0.AppendLine("             On c.Cul_Cod = i.CUL_COD ")
                        Stb0.AppendLine("  ) d ")
                        Stb0.AppendLine("      inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
                        Stb0.AppendLine("             On d.tipo_sorgente = srv.tipo_sorgente ")
                        Stb0.AppendLine("          And d.Stazione_Cod = srv.stazione_Cod ")
                        'Stb0.AppendLine("      inner Join modelliPrevisionali p ")
                        'Stb0.AppendLine("             On p.Mod_Cod = srv.Mod_Cod ")

                        Stb0.Append("		) op " & vbCrLf)
                        Stb0.Append("			On ( ( op.av_cod = mav.av_cod And op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & ") " & vbCrLf)
                        Stb0.Append("               Or ( op.FF_Classe = mav.av_cod And op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & " ) " & vbCrLf)
                        Stb0.Append("               Or ( op.FF_Classe = mav.av_cod And op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " ) ) " & vbCrLf)
                        Stb0.Append("				And op.udm_cod = mav.udm_cod And op.lav_cod = mav.lav_cod " & vbCrLf)
                        Stb0.Append("   ) tmp1 " & vbCrLf)
                        Stb0.Append("   group by piva, sa_cod, appezza, id_reg, id_agenda, ordine, ricetta_operazione_cod, dataoperazione " & vbCrLf)
                        Stb0.Append(") tmp2 " & vbCrLf)
                        Stb0.Append(" " & vbCrLf)

                        Stb.Insert(0, Stb0.ToString)

                        Stb.Append(", isNull( t_o.info_estese, isNull(t_ag.info_estese, '') ) as '@info_estese' " & vbCrLf)

                        Dim stb1 As New System.Text.StringBuilder
                        stb1.Append(" " & vbCrLf)
                        stb1.Append("LEFT JOIN @TableOp t_o on t_o.ricetta_operazione_cod > 0 and t_o.ricetta_operazione_cod = entita.Ricetta_Operazione_cod " & vbCrLf)
                        stb1.AppendLine("LEFT JOIN ( ")
                        stb1.AppendLine("  Select t.* ")
                        stb1.AppendLine("      from ( ")
                        stb1.AppendLine("      select piva, Sa_Cod, Appezza, id_reg, MAX(DataOperazione) as DataOperazione ")
                        stb1.AppendLine("         from @TableOp op1 ")
                        stb1.AppendLine("         where op1.DataOperazione>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                        stb1.AppendLine("         and op1.DataOperazione <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                        stb1.AppendLine("      group by piva, Sa_Cod, Appezza, id_reg  ")
                        stb1.AppendLine("  ) a  ")
                        stb1.AppendLine("     inner Join @TableOp t ")
                        stb1.AppendLine("         On t.piva = a.piva  ")
                        stb1.AppendLine("      And t.sa_cod = a.sa_cod  ")
                        stb1.AppendLine("      And t.appezza = a.appezza  ")
                        stb1.AppendLine("      And t.id_reg = a.id_reg      ")
                        stb1.AppendLine("      And t.DataOperazione = a.DataOperazione ")
                        stb1.Append(") t_ag on t_ag.ricetta_operazione_cod = 0  " & vbCrLf)
                        stb1.Append("    and ( ( t_ag.id_agenda > 0 and t_ag.Id_Agenda = entita.id_agenda) or (t_ag.Id_Agenda = -1 and entita.tipoEntita_cod in (select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19) ) ) " & vbCrLf)
                        stb1.Append("    and t_ag.piva = entita.piva " & vbCrLf)
                        stb1.Append("    and t_ag.sa_Cod = entita.sa_Cod " & vbCrLf)
                        stb1.Append("    and t_ag.appezza = entita.appezza  " & vbCrLf)
                        stb1.Append("    and t_ag.id_reg = entita.id_imp " & vbCrLf)
                        stb1.Append(" " & vbCrLf)

                        TableOp_LeftJoin = stb1.ToString()

                    Else

                        Select Case cfgAlbero.TipologiaLayer_Cod
                            Case enum_TipologiaLayer.RilieviVegetoProduttivi,
                                 enum_TipologiaLayer.Avversita,
                                 enum_TipologiaLayer.Fenologia
                                LeggiGetQuery_infoEstese_Ag(Stb)

                            Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                                LeggiGetQuery_infoEstese_AnalisiPeriodicitaAgenda(Stb)

                            Case Else
                                Stb.Append("      ,'' as '@info_estese' " & vbCrLf)

                        End Select

                    End If

                Else
                    LeggiGetQuery_infoEstese_Ag(Stb)
                End If



                Stb.Append("      ,'' as '@TipoIcona' " & vbCrLf)
                Stb.Append("  " & vbCrLf)
            End If

            Stb.Append("    --<Entita> " & vbCrLf)
            Stb.Append("    , (      " & vbCrLf)
            Stb.Append("     select " & vbCrLf)
            Stb.Append("    Entita.PivaSuperUser " & vbCrLf)
            Stb.Append("  , Entita.Entita_Cod " & vbCrLf)

            Stb.Append("  , geodata.flag_gps " & vbCrLf)
            Stb.Append("  , geodata.LayerElementiGrafici_Cod as StandardEntita_layerDiAppartenenza  " & vbCrLf)

            'Gabriele
            'Stb.Append("  , COALESCE(app1.APP_NOME, ent.Entita_Des) AS APP_NOME " & vbCrLf)
            Stb.Append("  , Entita.ETICHETTA " & vbCrLf)

            Stb.Append("  , Entita.TipoEntita_Cod " & vbCrLf)
            Stb.Append("  , Entita.Piva " & vbCrLf)
            Stb.Append("  , Entita.Sa_Cod " & vbCrLf)
            Stb.Append("  , Entita.Appezza " & vbCrLf)
            Stb.Append("  , coalesce(app1.campo_cod, Entita.Campo_Cod) as Campo_Cod " & vbCrLf)
            Stb.Append("  , Entita.Id_Imp " & vbCrLf)
            Stb.Append("  , Entita.PROV " & vbCrLf)
            Stb.Append("  , Entita.COM " & vbCrLf)
            Stb.Append("  , Entita.SEZIONE " & vbCrLf)
            Stb.Append("  , Entita.FOGLIO " & vbCrLf)
            Stb.Append("  , Entita.NUMERO " & vbCrLf)
            Stb.Append("  , Entita.SUBALTERNO " & vbCrLf)
            Stb.Append("  , Entita.Programmazione_Entita_Cod " & vbCrLf)
            Stb.Append("  , Entita.ID_Agenda " & vbCrLf)
            Stb.Append("  , Entita.Ricetta_Operazione_Cod " & vbCrLf)
            Stb.Append("  , Entita.analisi_testata_cod " & vbCrLf)
            Stb.Append("  , Entita.analisi_campione_cod " & vbCrLf)
            Stb.Append("  , Entita.OLDGrafica_ID " & vbCrLf)
            Stb.Append("  , Entita.inviato " & vbCrLf)
            Stb.Append("  , Entita.datainvio " & vbCrLf)
            Stb.Append("  , Entita.Data_Creazione " & vbCrLf)
            Stb.Append("  , Entita.Data_Modifica " & vbCrLf)
            Stb.Append("  , Entita.Username_Creazione " & vbCrLf)
            Stb.Append("  , Entita.Username_Modifica " & vbCrLf)
            Stb.Append("  , Entita.Validita_Inizio " & vbCrLf)
            Stb.Append("  , Entita.Validita_Fine " & vbCrLf)
            Stb.Append("  , coalesce( ent.Programmazione_Cod, Entita.Programmazione_cod) as Programmazione_Cod " & vbCrLf)


            'Se mi trovo nel caso "Sementieri" ...
            If Sementieri_Sportello_Configurazione_cod <> -1 Then
                Stb.Append("  , 0 as Ricetta_Cod " & vbCrLf)
            Else
                Stb.Append("  , rOperaz.Ricetta_Cod " & vbCrLf)
            End If


            Stb.Append("  , tEntita.TipoNodoAlberoAnagrafe  " & vbCrLf)
            Stb.Append("  , coalesce(Entita.part_cod, -1) as part_cod " & vbCrLf)
            Stb.Append("  , coalesce(coalesce( reg1.codice_fiscale_tecnico, ent.Codice_Fiscale_tecnico),'') as codice_fiscale_tecnico " & vbCrLf)

            'Se mi trovo nel caso "Sementieri" ...
            If Sementieri_Sportello_Configurazione_cod <> -1 Then

                Stb.AppendLine()
                Stb.AppendLine("  , filtroSementi.InOsservazione ")
                Stb.AppendLine("  , filtroSementi.Colore_Primario ")
                Stb.AppendLine("  , filtroSementi.Colore_Retinatura ")
                Stb.AppendLine("  , filtroSementi.Trasparenza ")
                Stb.AppendLine()

            Else

                Stb.AppendLine()
                Stb.AppendLine("  , 0 AS InOsservazione ")
                Stb.AppendLine("  , '' AS Colore_Primario ")
                Stb.AppendLine("  , '' AS Colore_Retinatura ")
                Stb.AppendLine("  , 0 AS Trasparenza ")
                Stb.AppendLine()

            End If

            'GABRIELE Aggiunto baricentro poligono (***)
            'Stb.AppendLine("    , geodata.baricentro AS baricentro ")
            'Stb.AppendLine()

            Stb.Append("            for xml path ('DatoGias'), type, elements            " & vbCrLf)
            Stb.Append("    ) as EntitaGIAS " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            If Not chiamataXAggiornaElencoTipologie Then



                Stb.Append("    --<Dettaglio> " & vbCrLf)
                Stb.Append("    , ( " & vbCrLf)
                Stb.Append("         SELECT " & vbCrLf)
                Stb.Append("              dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'AppldRate','|','§') as AppIdRate " & vbCrLf) ' the '§' logic will be maintained
                Stb.Append("            , dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'Speed','|','§') as Speed " & vbCrLf)
                Stb.Append("            , dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'Height','|','§') as Height " & vbCrLf)
                Stb.Append("        for xml path ('PrecisionFarming'), type, elements " & vbCrLf)
                Stb.Append("    ) as Dettaglio " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    --<DatiAnagrafici> " & vbCrLf)
                Stb.Append("    , ( " & vbCrLf)
                Stb.Append("        select cul_reale.veg_cod " & vbCrLf)
                Stb.Append("        for xml path ('Impianto'), type, elements " & vbCrLf)
                Stb.Append("    ) as DatiAnagrafici " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    --<Planning> " & vbCrLf)
                Stb.Append("    , ( " & vbCrLf)
                Stb.Append("        select cul_planning.veg_cod " & vbCrLf)
                Stb.Append("        for xml path ('programmazione_entita'), type, elements " & vbCrLf)
                Stb.Append("    ) as Planning " & vbCrLf)
                Stb.Append(" " & vbCrLf)
                Stb.Append("                --<autorizzazioni> " & vbCrLf)
                Stb.Append("    , ( " & vbCrLf)
                Stb.Append("        select   " & vbCrLf)

                'TODO: lettura dei permessi nel caso di non sementieri
                If Sementieri_Sportello_Configurazione_cod = -1 Then

                    'Per gestione Sementieri -> Mappatura Libera
                    Dim str_modCanc As String = "CASE WHEN cul_reale.veg_cod IS NOT NULL AND EXISTS (SELECT 1 FROM mappatura_specie WHERE veg_cod = cul_reale.veg_cod AND grfi_cod = reg1.GRFI_COD) THEN 'False' ELSE 'True' END"
                    Stb.Append("              'True' as inserimento " & vbCrLf)
                    Stb.Append("              , " & str_modCanc & " as modifica " & vbCrLf)
                    Stb.Append("              , " & str_modCanc & " as cancellazione " & vbCrLf)
                    Stb.Append("              , 'True' as informazioni " & vbCrLf)
                Else
                    Stb.Append("              case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as inserimento " & vbCrLf)
                    Stb.Append("            , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as modifica " & vbCrLf)
                    Stb.Append("            , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as cancellazione " & vbCrLf)
                    Stb.Append("            , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as informazioni " & vbCrLf)
                End If

                Stb.Append("        for xml path ('gis'), type, elements " & vbCrLf)
                Stb.Append("    ) as autorizzazioni " & vbCrLf)
                Stb.Append(" " & vbCrLf)



                Stb.Append("    --<geodata> " & vbCrLf)

                ' VAnni: 27/7/2018: per ricomprendere anche i rilievi vegeto produttivi tematizzando poligoni...:
                'If Not cfgAlbero Is Nothing AndAlso (
                '    cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Or
                '    cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

                'End If


                If Not cfgAlbero Is Nothing AndAlso (
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Or
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

                    'If Not cfgAlbero Is Nothing AndAlso (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda) Then


                    Stb.Append("    , (select geodata1.Poligono_GeoEntity.AsGml() as geodata    " & vbCrLf)
                    Stb.Append("        from  ( " & vbCrLf)

                    Stb.AppendLine("  Select ")
                    Stb.AppendLine("     e.Punto_Entita_Cod as Entita_Cod ")
                    Stb.AppendLine("   , g1.Poligono_GeoEntity ")
                    Stb.AppendLine(" from( ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("     Select  ")
                    Stb.AppendLine("         e.Entita_Cod as Punto_Entita_Cod ")
                    Stb.AppendLine("      , e1.Entita_Cod  as poligono_Entita_Cod ")
                    Stb.AppendLine("      , g.LayerElementiGrafici_Cod ")
                    Stb.AppendLine("      , g.ElementoGrafico_Des ")
                    Stb.AppendLine("      , g.Flag_GPS ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("  From GIS_ElementiGrafici g ")
                    Stb.AppendLine("      inner Join GIS_Entita e  ")
                    Stb.AppendLine("             On g.Entita_Cod = e.Entita_Cod ")

                    Stb.AppendLine("      inner Join GIS_Entita e1 ")
                    Stb.AppendLine("             On  e1.piva  = e.Piva   ")
                    Stb.AppendLine("              And e1.sa_Cod = e.Sa_Cod  ")
                    Stb.AppendLine("              And e1.appezza = e.Appezza  ")
                    Stb.AppendLine("              And e1.id_imp = e.id_imp   ")
                    Stb.AppendLine("              And e1.TipoEntita_Cod in (19, 20, 21, 22,23)       ")
                    Stb.AppendLine("              And e.TipoEntita_Cod Not in (19, 20, 21, 22,23)      ")


                    'Stb.AppendLine("      inner Join( ")
                    'Stb.AppendLine("             select ee.* ")
                    'Stb.AppendLine("          From GIS_Entita ee ")
                    'Stb.AppendLine("              inner Join GIS_ElementiGrafici gg ")
                    'Stb.AppendLine("                     On ee.Entita_Cod = gg.Entita_Cod      ")
                    'Stb.AppendLine("          where ee.TipoEntita_Cod in (select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19) ")
                    'Stb.AppendLine("          ) e1 ")
                    'Stb.AppendLine("          On e1.piva  = e.Piva  ")
                    'Stb.AppendLine("          And e1.sa_Cod = e.Sa_Cod ")
                    'Stb.AppendLine("          And e1.appezza = e.Appezza ")
                    'Stb.AppendLine("          And e1.id_imp = e.id_imp         ")
                    'Stb.AppendLine("       ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("  where g.Poligono_GeoEntity.STGeometryType() = 'Point' ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("   union ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("   Select ")
                    Stb.AppendLine("        e.Entita_Cod as Punto_Entita_Cod ")
                    Stb.AppendLine("      , e.Entita_Cod as Poligono_Entita_Cod ")
                    Stb.AppendLine("      , g.LayerElementiGrafici_Cod ")
                    Stb.AppendLine("      , g.ElementoGrafico_Des ")
                    Stb.AppendLine("      , g.Flag_GPS ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine("  From GIS_ElementiGrafici g ")
                    Stb.AppendLine("      inner Join GIS_Entita e ")
                    Stb.AppendLine("             On g.Entita_Cod = e.Entita_Cod ")
                    Stb.AppendLine("  where Poligono_GeoEntity.STGeometryType() <> 'Point' ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine(" ) e ")
                    Stb.AppendLine("  ")
                    Stb.AppendLine(" inner Join GIS_ElementiGrafici g1 ")
                    Stb.AppendLine("     On g1.Entita_Cod = e.poligono_Entita_Cod ")

                    Stb.Append("        ) Geodata1  " & vbCrLf)
                    Stb.Append("             where(geodata1.entita_cod = geodata.entita_cod) " & vbCrLf)
                    Stb.Append("    ) as geodata " & vbCrLf)

                Else

                    Stb.Append("    , (select geodata1.Poligono_GeoEntity.AsGml() as geodata    " & vbCrLf)
                    Stb.Append("        from GIS_ElementiGrafici geodata1 " & vbCrLf)
                    Stb.Append("             where(geodata1.entita_cod = geodata.entita_cod) " & vbCrLf)
                    Stb.Append("    ) as geodata " & vbCrLf)
                End If




                If CalcolaAreaPerimetro Then
                    Stb.Append("                , (     select   round(geodata1.Poligono_GeoEntity.STArea(), 5) as geodata_Area " & vbCrLf)
                    Stb.Append("                               , round(geodata1.Poligono_GeoEntity.STLength(), 5) as geodata_Perimetro  " & vbCrLf)
                    Stb.Append("                               , geometry::STGeomFromText(geodata1.Poligono_GeoEntity.STAsText(), 0).MakeValid().STCentroid().STAsText() as geodata_Baricentro " & vbCrLf)
                    Stb.Append("  from GIS_ElementiGrafici geodata1 " & vbCrLf)
                    Stb.Append(" where(geodata1.entita_cod = geodata.entita_cod)" & vbCrLf)

                    Stb.Append("            for xml path('DatiCalcolati'),type, elements  " & vbCrLf)
                    Stb.Append("       ) geodata      " & vbCrLf)
                End If

                'parte di lettura dei layers

                Stb.Append("    -- <layers> " & vbCrLf)

                Stb.Append("    , (  " & vbCrLf)
                Stb.Append("        select * from (  " & vbCrLf)

                Dim xLayerRichiesto As enum_TipologiaLayer = enum_TipologiaLayer.NonSpecificato
                If Not cfgAlbero Is Nothing Then
                    xLayerRichiesto = cfgAlbero.TipologiaLayer_Cod
                End If

                '1 - standard (sempre richiesto)
                LeggigetQuery_Layer_Standard(Stb, objParametri, leggiLayerNonVisibili)

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.Organizzazione Then
                    '100, struttura di appertenenza
                    LeggigetQuery_Layer_StrutturaDiAppertenenza(Stb, Sementieri_Sportello_Configurazione_cod)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.SpecieVegetale Then
                    '5 - Specie Vegetale
                    LeggigetQuery_Layer_SpecieVegetale(Stb)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.Cultivar Then
                    '15 - Cultivar
                    LeggigetQuery_Layer_Cultivar(Stb)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.Fenologia Then
                    '8 - fenologia
                    LeggigetQuery_Layer_Fenologia(Stb)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.Avversita Then
                    '7 - avversità
                    LeggigetQuery_Layer_Avversità(Stb)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.RilieviVegetoProduttivi Then
                    '9 - rilievi Vegeto Produttivi
                    LeggigetQuery_Layer_RilieviVegetoProduttivi(Stb)
                End If

                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Then
                    '11 - Analisi Periodicità Agenda

                    'GABRIELE 27-08-2018
                    If xLayerRichiesto = enum_TipologiaLayer.AnalisiPeriodicitaAgenda And test_Ottimizzazione_1 Then

                        Stb.AppendLine("            union all  ")
                        Stb.AppendLine("                select  distinct ")
                        Stb.AppendLine("                11 as '@tipologia_layer'  ")
                        Stb.AppendLine("                , cast(lav_cod as varchar(1000)) as '*'   ")
                        Stb.AppendLine("                from ( ")
                        Stb.AppendLine("        Select reg.LAV_COD from @TableOperazioni reg ")
                        Stb.AppendLine("    where reg.PIVA = Entita.Piva   ")
                        Stb.AppendLine("    And reg.SA_COD = Entita.Sa_Cod   ")
                        Stb.AppendLine("    And reg.APPEZZA = Entita.Appezza   ")
                        Stb.AppendLine("    And reg.ID_REG = entita.id_imp    ")
                        Stb.AppendLine("    And ( (reg.id_mov_det > 0 And entita.id_mov_det = reg.Id_Mov_Det ) Or (reg.Id_Mov_Det = -1 And entita.tipoEntita_Cod in (select tipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ))   ")
                        Stb.AppendLine(" ) reg1  ")

                    Else

                        LeggigetQuery_Layer_AnalisiPeriodicitaAgenda(Stb)

                    End If
                End If


                If xLayerRichiesto = enum_TipologiaLayer.NonSpecificato Or xLayerRichiesto = enum_TipologiaLayer.Percorsi Then
                    '10 - Percorsi
                    LeggigetQuery_Layer_Percorsi(Stb)
                End If




                Stb.Append("    ) a For xml path('layer'),type, elements   " & vbCrLf)
                Stb.Append("  ) as layers   " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                'fine parte di lettura dei layers


                'StrSQL.Append("  --<layersdescrizioni> " & vbCrLf)
                'StrSQL.Append("  , ( " & vbCrLf)
                'StrSQL.Append("    select case when DENSE_RANK () over ( order by geodata.ElementoGrafico_cod) <> 1 then null else l.Colore_Base end " & vbCrLf)
                'StrSQL.Append("    for xml path('layer'),type, elements   " & vbCrLf)
                'StrSQL.Append("    )  layersdescrizioni " & vbCrLf)

            End If

            Stb.Append("  " & vbCrLf)
            'Stb.Append(" from  GIS_Entita Entita " & vbCrLf)
            Stb.Append(" FROM ")
            LeggiGetQuery_GisEntitaConFiltroTemporale(Piva, Sa_Cod, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, objParametri, Stb)

            Stb.AppendLine("INNER JOIN GIS_TipoEntita tEntita ON tEntita.TipoEntita_cod = Entita.TipoEntita_cod ")

            'GABRIELE Aggiunto baricentro poligono (***)
            Stb.AppendLine("INNER JOIN (")
            Stb.AppendLine("    SELECT ")
            Stb.AppendLine("        entita_cod ")
            Stb.AppendLine("        , LayerElementiGrafici_Cod ")
            Stb.AppendLine("        , ElementoGrafico_Des ")
            Stb.AppendLine("        , Flag_GPS ")
            'Stb.AppendLine("       , Poligono_GeoEntity.EnvelopeCenter().AsGml() AS baricentro ")
            Stb.AppendLine("    FROM GIS_ElementiGrafici ) geodata ON Entita.Entita_Cod = geodata.Entita_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("INNER JOIN gis_layerElementiGrafici layerPrincipale ")
            ' VAnni: 19/8/2020: il layer rilievi è gestito a parte
            Stb.AppendLine("    ON layerPrincipale.LayerElementiGrafici_Cod =   case when Entita.TipoEntita_Cod = 63 then 63 else  geodata.LayerElementiGrafici_Cod  end --geodata.LayerElementiGrafici_Cod  ")
            Stb.AppendLine("        AND layerPrincipale.TipologiaLayer_cod = 1 ")
            If Not leggiLayerNonVisibili Then
                Stb.AppendLine("        AND layerPrincipale.flag_visibile = 1 ")
            End If
            Stb.AppendLine("        AND layerPrincipale.Utente = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            Stb.AppendLine()

            If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
                Stb.AppendLine("INNER JOIN GIS_ElementiGrafici GeoDataIntersezione ON GeoDataIntersezione.entita_cod = Entita.entita_cod")
                Stb.AppendLine()
            End If


            'If Sementieri_Sportello_Configurazione_cod = -1 Then
            '    Stb.Append(" inner join (  " & vbCrLf)
            '    Stb.Append("        Select padre, figlio  " & vbCrLf)
            '    Stb.Append("        from GerarchieImpreseEsplosa   " & vbCrLf)
            '    Stb.Append("        where padre ='" & PivaPadre & "'" & vbCrLf)
            '    Stb.Append("        union " & vbCrLf)
            '    Stb.Append("        select '', '' " & vbCrLf)
            '    Stb.Append("    ) GI on figlio = Entita.Piva  " & vbCrLf)
            'End If


            'vanni, 24/11/2017: vecchia versione con GerarchieImpreseEsplosa, non è chiaro se e come viene ancora usata,
            '  inoltre adesso in visualizzazione totale si legge il filtro imprese.

            'If Sementieri_Sportello_Configurazione_cod = -1 Then
            '    Stb.Append(" inner join (  " & vbCrLf)
            '    Stb.Append("        Select padre, figlio  " & vbCrLf)
            '    Stb.Append("        from GerarchieImpreseEsplosa   " & vbCrLf)
            '    Stb.Append("        where padre ='" & PivaSuperUser & "'" & vbCrLf)
            '    Stb.Append("        union " & vbCrLf)
            '    Stb.Append("        select '', '' " & vbCrLf)
            '    Stb.Append("    ) GI on figlio = Entita.Piva  " & vbCrLf)
            'End If


            ' VAnni: 27/11/2017: se non chiedo una p.iva allora devo considerare, eccetto i casi dove la piva e vuota in GIS_Entita 
            '   (eccetto il caso dei sementieri)
            ' nel caso delle entità aggiungo anche quelle con piva stringa vuota (deduco de debbano essere visibili per tutti)
            ' mentre nel caso delle particelle vado in join sulla ImpreseXParticelle invece che su GIS_Entita


            'spostato su lettura della singola tipologia layer
            'Stb.Append("    inner join GIS_LayerElementiGrafici l on l.LayerElementiGrafici_Cod = geodata.LayerElementiGrafici_Cod  " & vbCrLf)
            'Stb.Append("    and l.Utente = '" & objParametri.UtenteUsername & "' " & vbCrLf)
            'Stb.Append("    and l.flag_visibile <> 0 " & vbCrLf)
            'Stb.Append("    and l.flag_attivo <> 0 " & vbCrLf)

            Stb.AppendLine("LEFT JOIN reg_impianti reg1 ON reg1.PIVA = Entita.Piva AND reg1.SA_COD = Entita.Sa_Cod AND reg1.APPEZZA = Entita.Appezza AND reg1.ID_REG = entita.id_imp ")
            Stb.AppendLine()
            Stb.AppendLine("LEFT JOIN Appezzamento app1 ON app1.PIVA = Entita.Piva AND app1.SA_COD = Entita.Sa_Cod AND app1.APPEZZA = Entita.Appezza ")
            Stb.AppendLine()
            Stb.AppendLine("LEFT JOIN cultivar cul_reale ON reg1.cul_cod = cul_reale.Cul_Cod ")
            Stb.AppendLine()
            Stb.AppendLine("LEFT JOIN Programmazione_Entita ent ON ent.programmazione_entita_cod = entita.Programmazione_Entita_COD ")
            Stb.AppendLine()
            Stb.AppendLine("LEFT JOIN Ricette_Operazioni rOperaz ON rOperaz.Ricetta_Operazione_cod = entita.Ricetta_Operazione_cod ")
            Stb.AppendLine()
            Stb.AppendLine("LEFT JOIN cultivar cul_planning ON ent.cul_cod = cul_planning.Cul_Cod ")
            Stb.AppendLine()

            If Sementieri_Sportello_Configurazione_cod <> -1 Then

                'Stb.Append("    left join ( " & vbCrLf)
                'Stb.Append("        select  " & vbCrLf)
                'Stb.Append("        MIN (ID_Specie) as id_specie, min(id_sottoSpecie) as  id_sottoSpecie, min(ID_Gruppo) as id_gruppo, MIN (id_genotipo) as id_genotipo, Veg_Cod, 0 as Grva_Cod, MIN(Raggruppamento) as raggruppamento, min(hybrid) as hybrid " & vbCrLf)
                'Stb.Append("        from Mappatura_Specie " & vbCrLf)
                'Stb.Append("        group by Veg_Cod " & vbCrLf)
                'Stb.Append("   ) farlocc on farlocc.Grva_Cod = reg1.GRVA_Cod_VEG  " & vbCrLf)
                'Stb.Append("   and farlocc.veg_cod = cul_reale.veg_cod " & vbCrLf)

                'Stb.Append("    left join Mappatura_Specie ms  " & vbCrLf)
                'Stb.Append("        on ms.veg_cod = coalesce(cul_reale.veg_cod, cul_planning.veg_cod) " & vbCrLf)
                'Stb.Append("        and ms.grva_cod = abs(coalesce(reg1.GRVA_Cod_VEG, ent.Grva_cod)) " & vbCrLf)
                'Stb.Append("        and ms.Hybrid =  (CASE WHEN coalesce(cul_reale.veg_cod, cul_planning.veg_cod) in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN coalesce(reg1.GRVA_Cod_VEG, ent.Grva_cod) <0 THEN 1 ELSE 0 END) END) " & vbCrLf)

                'Stb.Append("  " & vbCrLf)
                'Stb.Append("    inner join Sementieri_Sportello_ConfigurazioneXmappatura_specie sscms " & vbCrLf)
                'Stb.Append("        on sscms.ID_Specie = coalesce(ms.ID_Specie, farlocc.ID_Specie)  " & vbCrLf)
                'Stb.Append("        and sscms.ID_SottoSpecie = coalesce(ms.ID_SottoSpecie, farlocc.ID_SottoSpecie)  " & vbCrLf)
                'Stb.Append("        and sscms.id_genotipo = coalesce(ms.id_genotipo, farlocc.id_genotipo) " & vbCrLf)
                'Stb.Append("        and sscms.id_gruppo = coalesce(ms.id_gruppo, farlocc.id_gruppo) " & vbCrLf)
                'Stb.Append("        and sscms.Sementieri_Sportello_Configurazione_cod =  " & Sementieri_Sportello_Configurazione_cod & vbCrLf)

                'Stb.Append("    left join (  " & vbCrLf)
                'Stb.Append("        select top 1 ssp.Visibilita_Impianti, Sementieri_Sportello_Configurazione_cod, case when ssp.DestinazioneSalvataggio = -1 then 'False' else 'True' end as PermessoModifiche " & vbCrLf)
                'Stb.Append("        from Sementieri_Sportello_ConfigurazioneXPassaggi sscp " & vbCrLf)
                'Stb.Append("            inner join Sementieri_Sportello_Passaggi ssp  " & vbCrLf)
                'Stb.Append("            on sscp.Sementieri_Sportello_Passaggi_cod = ssp.Sementieri_Sportello_Passaggi_cod " & vbCrLf)
                'Stb.Append("            and sscp.Sementieri_Sportello_Configurazione_cod =  " & Sementieri_Sportello_Configurazione_cod & vbCrLf)
                'Stb.Append("            and cast(GETDATE() as date) between sscp.Data_Inizio and  sscp.Data_fine           " & vbCrLf)
                'Stb.Append("        ) vis on sscms.Sementieri_Sportello_Configurazione_cod = vis.Sementieri_Sportello_Configurazione_cod " & vbCrLf)


                Stb.AppendLine()
                Stb.AppendLine("    INNER JOIN ( ")
                Stb.AppendLine()
                Stb.AppendLine("        --da sportello corrente")
                Stb.AppendLine()
                Stb.AppendLine("        SELECT ")
                Stb.AppendLine("            ms.Grva_Cod AS grva_cod ")
                Stb.AppendLine("            , ms.Veg_Cod AS veg_cod ")
                Stb.AppendLine("            , ms.grfi_cod AS grfi_cod ")
                Stb.AppendLine("            , ms.Hybrid AS hybrid ")
                Stb.AppendLine("            , vis.Visibilita_Impianti ")
                Stb.AppendLine("            , vis.PermessoModifiche ")
                Stb.AppendLine("            , rOriginale.PIVA ")
                Stb.AppendLine("            , rOriginale.SA_COD ")
                Stb.AppendLine("            , rOriginale.APPEZZA ")
                Stb.AppendLine("            , rOriginale.ID_REG	")
                Stb.AppendLine()
                Stb.AppendLine("            , 0 AS InOsservazione ")
                Stb.AppendLine("            , '' AS Colore_Primario ")
                Stb.AppendLine("            , '' AS Colore_Retinatura ")
                Stb.AppendLine("            , 0 AS Trasparenza ")
                Stb.AppendLine()
                Stb.AppendLine("        FROM Reg_Impianti rOriginale ")
                Stb.AppendLine("        INNER JOIN Cultivar vegOriginale ")
                Stb.AppendLine("            ON vegOriginale.Cul_Cod = rOriginale.CUL_COD ")
                Stb.AppendLine("        INNER JOIN Mappatura_Specie ms ")
                Stb.AppendLine("            ON (ms.Veg_Cod = vegOriginale.Veg_Cod) ")
                Stb.AppendLine("                AND (ms.Grva_Cod = ABS(rOriginale.GRVA_Cod_VEG)) ")
                Stb.AppendLine("                AND (ms.grfi_Cod = rOriginale.GRFI_COD) ")
                Stb.AppendLine("                AND (ms.Hybrid = (CASE WHEN vegOriginale.veg_cod IN (6, 5000021, 70, 69) THEN -1 ELSE (CASE WHEN rOriginale.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END)) ")
                Stb.AppendLine()
                Stb.AppendLine("        INNER JOIN Sementieri_Sportello_ConfigurazioneXmappatura_specie sscms ")
                Stb.AppendLine("            ON sscms.ID_Specie = ms.ID_Specie ")
                Stb.AppendLine("                AND sscms.ID_SottoSpecie = ms.ID_SottoSpecie ")
                Stb.AppendLine("                AND sscms.id_genotipo = ms.id_genotipo ")
                Stb.AppendLine("                AND sscms.id_gruppo = ms.id_gruppo")
                Stb.AppendLine()
                Stb.AppendLine("        LEFT JOIN ( ")
                Stb.AppendLine("            SELECT TOP 1 ssp.Visibilita_Impianti, Sementieri_Sportello_Configurazione_cod, CASE WHEN ssp.DestinazioneSalvataggio = -1 THEN 'False' ELSE 'True' END AS PermessoModifiche ")
                Stb.AppendLine("            FROM Sementieri_Sportello_ConfigurazioneXPassaggi sscp ")
                Stb.AppendLine("                INNER JOIN Sementieri_Sportello_Passaggi ssp ")
                Stb.AppendLine("                    ON sscp.Sementieri_Sportello_Passaggi_cod = ssp.Sementieri_Sportello_Passaggi_cod ")
                Stb.AppendLine("                        AND sscp.Sementieri_Sportello_Configurazione_cod = " & Sementieri_Sportello_Configurazione_cod)
                Stb.AppendLine("                        AND CAST(GETDATE() AS DATE) BETWEEN sscp.Data_Inizio AND sscp.Data_fine ")
                Stb.AppendLine("        ) vis ")
                Stb.AppendLine("        ON sscms.Sementieri_Sportello_Configurazione_cod = vis.Sementieri_Sportello_Configurazione_cod ")
                Stb.AppendLine()
                Stb.AppendLine("        WHERE sscms.Sementieri_Sportello_Configurazione_Cod = " & Sementieri_Sportello_Configurazione_cod)
                Stb.AppendLine("        AND rOriginale.Validita_inizio >= @DataI ")
                Stb.AppendLine("        AND rOriginale.Validita_fine <= @DataF ")
                Stb.AppendLine()
                Stb.AppendLine("        UNION ")
                Stb.AppendLine()
                Stb.AppendLine("        --da osservati per specie vegetale ")
                Stb.AppendLine()
                Stb.AppendLine("        SELECT ")
                Stb.AppendLine("            Osservato_GRVA_Cod AS grva_cod ")
                Stb.AppendLine("            , Osservato_veg_cod AS veg_cod ")
                Stb.AppendLine("            , Osservato_grfi_cod AS grfi_cod ")
                Stb.AppendLine("            , Osservato_Hybrid AS hybrid ")
                Stb.AppendLine("            , 1 as Visibilita_Impianti ")
                Stb.AppendLine("            , 'False' AS PermessoModifiche ")
                Stb.AppendLine("            , rEsteso.PIVA ")
                Stb.AppendLine("            , rEsteso.SA_COD ")
                Stb.AppendLine("            , rEsteso.APPEZZA ")
                Stb.AppendLine("            , rEsteso.ID_REG ")
                Stb.AppendLine()
                Stb.AppendLine("            , 1 AS InOsservazione ")
                Stb.AppendLine("            , COALESCE(oss.Colore_Primario, '') AS Colore_Primario ")
                Stb.AppendLine("            , COALESCE(oss.Colore_Retinatura, '') AS Colore_Retinatura ")
                Stb.AppendLine("            , COALESCE(oss.Trasparenza, 0) AS Trasparenza ")
                Stb.AppendLine()
                Stb.AppendLine("        FROM Reg_Impianti rEsteso ")
                Stb.AppendLine("        INNER JOIN Cultivar vegEsteso ")
                Stb.AppendLine("            ON vegEsteso.Cul_Cod = rEsteso.CUL_COD ")
                Stb.AppendLine("        INNER JOIN SpecieVegetaliInOsservazione oss ")
                Stb.AppendLine("            ON (oss.Osservato_cul_cod = rEsteso.CUL_COD OR oss.osservato_CUL_COD IS NULL) ")
                Stb.AppendLine("                AND (oss.Osservato_Veg_Cod = vegEsteso.Veg_Cod OR oss.Osservato_Veg_Cod IS NULL) ")
                Stb.AppendLine("                AND (oss.Osservato_Grva_Cod = rEsteso.GRVA_Cod_VEG OR oss.Osservato_Grva_Cod IS NULL) ")
                Stb.AppendLine("                AND (oss.Osservato_grfi_Cod = rEsteso.GRFI_COD OR Osservato_grfi_Cod IS NULL) ")
                Stb.AppendLine("                AND (oss.Osservato_Hybrid = (CASE WHEN vegEsteso.veg_cod IN (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rEsteso.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) OR oss.Osservato_Hybrid IS NULL) ")
                Stb.AppendLine()
                Stb.AppendLine("        WHERE oss.Sementieri_Sportello_Configurazione_Cod = " & Sementieri_Sportello_Configurazione_cod)
                Stb.AppendLine("        AND oss.Osservato_Sementieri_Sportello_Configurazione_Cod IS NULL ")
                Stb.AppendLine("        AND NOT (@DataF < DATEADD(DAY, -ISNULL(oss.Osservazione_GG_Precedenti, 0), rEsteso.Validita_inizio) OR DATEADD(DAY, ISNULL(oss.Osservazione_GG_Successivi, 0), rEsteso.Validita_fine) < @DataI) ")
                Stb.AppendLine()
                Stb.AppendLine("        UNION ")
                Stb.AppendLine()
                Stb.AppendLine("        --da osservati per sportello ")
                Stb.AppendLine()
                Stb.AppendLine("        SELECT ")
                Stb.AppendLine("            ms.Grva_Cod AS grva_cod ")
                Stb.AppendLine("            , ms.Veg_Cod AS veg_cod ")
                Stb.AppendLine("            , ms.grfi_cod AS grfi_cod ")
                Stb.AppendLine("            , ms.Hybrid AS hybrid ")
                Stb.AppendLine("            , 1 AS Visibilita_Impianti ")
                Stb.AppendLine("            , 'False' AS PermessoModifiche ")
                Stb.AppendLine("            , rEsteso.PIVA ")
                Stb.AppendLine("            , rEsteso.SA_COD ")
                Stb.AppendLine("            , rEsteso.APPEZZA ")
                Stb.AppendLine("            , rEsteso.ID_REG ")
                Stb.AppendLine()
                Stb.AppendLine("            , 1 AS InOsservazione ")
                Stb.AppendLine("            , COALESCE(oss.Colore_Primario, '') AS Colore_Promario ")
                Stb.AppendLine("            , COALESCE(oss.Colore_Retinatura, '') AS Colore_Retinatura ")
                Stb.AppendLine("            , COALESCE(oss.Trasparenza, 0) AS Trasparenza ")
                Stb.AppendLine()
                Stb.AppendLine("        FROM Reg_Impianti rEsteso ")
                Stb.AppendLine("        INNER JOIN Cultivar vegEsteso ")
                Stb.AppendLine("            ON vegEsteso.Cul_Cod = rEsteso.CUL_COD ")
                Stb.AppendLine("        INNER JOIN Mappatura_Specie ms ")
                Stb.AppendLine("            ON (ms.Veg_Cod = vegEsteso.Veg_Cod) ")
                Stb.AppendLine("                AND (ms.Grva_Cod = abs(rEsteso.GRVA_Cod_VEG)) ")
                Stb.AppendLine("                AND (ms.grfi_Cod = rEsteso.GRFI_COD) ")
                Stb.AppendLine("                AND (ms.Hybrid = (CASE WHEN vegEsteso.veg_cod IN (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rEsteso.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END)) ")
                Stb.AppendLine("        INNER JOIN Sementieri_Sportello_ConfigurazioneXmappatura_specie sscms ")
                Stb.AppendLine("            ON sscms.ID_Specie = ms.ID_Specie ")
                Stb.AppendLine("                AND sscms.ID_SottoSpecie = ms.ID_SottoSpecie ")
                Stb.AppendLine("                AND sscms.id_genotipo = ms.id_genotipo ")
                Stb.AppendLine("                AND sscms.id_gruppo = ms.id_gruppo ")
                Stb.AppendLine("        INNER JOIN SpecieVegetaliInOsservazione oss ")
                Stb.AppendLine("            ON oss.Osservato_Sementieri_Sportello_Configurazione_Cod = sscms.Sementieri_Sportello_Configurazione_cod ")
                Stb.AppendLine()
                Stb.AppendLine("	INNER JOIN Sementieri_Sportello_Configurazione ssc ")
                Stb.AppendLine("		ON ssc.Sementieri_Sportello_Configurazione_cod = oss.Osservato_Sementieri_Sportello_Configurazione_Cod ")
                Stb.AppendLine("		AND rEsteso.Validita_inizio = ssc.Validita_Inizio ")
                Stb.AppendLine("		AND rEsteso.Validita_fine = ssc.Validita_Fine ")
                Stb.AppendLine()
                Stb.AppendLine("        WHERE oss.Sementieri_Sportello_Configurazione_Cod = " & Sementieri_Sportello_Configurazione_cod)
                Stb.AppendLine("        AND oss.Osservato_Sementieri_Sportello_Configurazione_Cod IS NOT NULL ")
                'Stb.AppendLine("        AND NOT (@DataF < DATEADD(DAY, -ISNULL(oss.Osservazione_GG_Precedenti, 0), rEsteso.Validita_inizio) OR DATEADD(DAY, ISNULL(oss.Osservazione_GG_Successivi, 0), rEsteso.Validita_fine) < @DataI) ")
                Stb.AppendLine()
                Stb.AppendLine("    ) filtroSementi ")
                Stb.AppendLine()
                Stb.AppendLine("        ON filtroSementi.PIVA = reg1.PIVA ")
                Stb.AppendLine("            AND filtroSementi.SA_COD = reg1.SA_COD ")
                Stb.AppendLine("            AND filtroSementi.APPEZZA = reg1.APPEZZA ")
                Stb.AppendLine("            AND filtroSementi.ID_REG = reg1.ID_REG ")
                Stb.AppendLine()
            End If

            If cfgAlbero Is Nothing Or (cfgAlbero IsNot Nothing AndAlso cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Organizzazione) Then

                Dim coalesce As String

                Dim connBuilder = New Common.DbConnectionStringBuilder
                connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione

                'lavez - 01/07/2024 - Eliminata Gerarchia imprese. Nei casi in cui un'azienda abbia più padri la join moltiplica i record
                'If Sementieri_Sportello_Configurazione_cod = "-1" Then
                '    coalesce = "coalesce(reg1.codice_Fiscale_tecnico, coalesce(ent.codice_fiscale_tecnico, E1.padre))"
                'Else
                coalesce = "coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico)"
                'End If

                Stb.AppendLine(String.Format("    LEFT JOIN {0}.dbo.Gruppi_Utente GU
	                                                         ON GU.Gruppi_Utente_Identificativo = cast( {1} as varchar(1000)) ", connBuilder("Initial Catalog"), coalesce))

                Stb.AppendLine()
            End If

        Else
            Stb.Length = 0
            Stb.Append(" SELECT  Entita.*, Ent.Programmazione_Cod, reg1.*, geodata.ElementoGrafico_cod, geodata.LayerElementiGrafici_Cod ")

            'Stb.Append(" FROM    GIS_Entita Entita")

            Stb.Append(" FROM ")
            LeggiGetQuery_GisEntitaConFiltroTemporale(Piva, Sa_Cod, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, objParametri, Stb)

            Stb.Append("    inner join GIS_ElementiGrafici geodata on Entita.Entita_Cod = geodata.Entita_Cod " & vbCrLf)

            Stb.Append("    left join Programmazione_Entita ent " & vbCrLf)
            Stb.Append("        on ent.programmazione_entita_cod = entita.Programmazione_Entita_COD  " & vbCrLf)

            Stb.Append("    left join reg_impianti reg1 on  " & vbCrLf)
            Stb.Append("             reg1.PIVA = Entita.Piva " & vbCrLf)
            Stb.Append("        and reg1.SA_COD = Entita.Sa_Cod   " & vbCrLf)
            Stb.Append("        and reg1.APPEZZA = Entita.Appezza   " & vbCrLf)
            Stb.Append("        and reg1.ID_REG = entita.id_imp    " & vbCrLf)


            Stb.Append("    left join Appezzamento app1 on  " & vbCrLf)
            Stb.Append("             app1.PIVA = Entita.Piva " & vbCrLf)
            Stb.Append("        and app1.SA_COD = Entita.Sa_Cod   " & vbCrLf)
            Stb.Append("        and app1.APPEZZA = Entita.Appezza   " & vbCrLf)

            Stb.Append("    left join Ricette_Operazioni rOperaz " & vbCrLf)
            Stb.Append("        on rOperaz.Ricetta_Operazione_cod = entita.Ricetta_Operazione_cod  " & vbCrLf)

            Stb.Append("  " & vbCrLf)

        End If

        Stb.AppendLine("LEFT JOIN Agenda a1 ON a1.piva = entita.piva AND a1.id_agenda = entita.id_agenda ")
        Stb.AppendLine()
        Stb.AppendLine("LEFT JOIN Analisi_campioni Camp1 ON camp1.Analisi_SuperUser = entita.PivaSuperUser ")

        If isXml Then
            Stb.AppendLine("    AND camp1.analisi_campione_Cod = entita.analisi_campione_Cod ")
        Else
            Stb.AppendLine("    AND camp1.Analisi_Campione_Key_Piva = Entita.Piva ")
            Stb.AppendLine("    AND Camp1.Analisi_Campione_Key_SaCod = Entita.Sa_Cod ")
            Stb.AppendLine("    AND camp1.Analisi_Campione_Key_IDGrafica = Entita.OLDGrafica_ID ")
        End If

        Stb.Append(TableOp_LeftJoin)

        LeggiGetQueryCondizioniWhere(PivaSuperUser, Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp, Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, programmazione_cod, Programmazione_Entita_cod, Ricetta_Operazione_Cod, xFiltroAggiuntivo, objParametri, Stb, isXml, Sementieri_Sportello_Configurazione_cod, Codice_Fiscale_Tecnico, wktBoundaySTIntersects, False)
        '--------------------------------------------------------------------------
        Select Case objParametri.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                Stb.Append(" AND   Entita.Inviato >=0 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                Stb.Append(" AND   Entita.Inviato =-1 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------

        If Not isXml Then
            Stb.Append("ORDER BY LayerElementiGrafici_Cod DESC")
        End If

        '' VAnni: 15/12/2017: non chiaro questo ordinamento
        'If isXml Then
        '    Stb.Append("ORDER BY l.LayerElementiGrafici_Cod DESC")
        'Else
        '    Stb.Append("ORDER BY LayerElementiGrafici_Cod DESC")
        'End If


        If xOrderBy <> "" Then
            Stb.Append("," & xOrderBy)
        End If

        If isXml Then

            Stb.Append(" for xml path('Entita'), root('DatiEntita'), elements ")

        End If
    End Sub

    Private Shared Sub LeggigetQuery_Layer_Fenologia(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                 Select distinct " & vbCrLf)
        Stb.Append("                 8 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append("                 , case when rDt.ff_Cod=0 then null else cast( rDt.ff_Cod as varchar(100)) end as '*'   " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)

        Stb.Append("                    select ricetta_operazione_cod, cast( FF_Classe as int) as ff_cod " & vbCrLf)
        Stb.Append("                    from Ricette_Dettaglio_Tecnico " & vbCrLf)
        Stb.Append("                    where ff_classe <> 0 " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select ricetta_operazione_cod, cast( piezo2 as int) as  ff_cod " & vbCrLf)
        Stb.Append("                    from ricette_dettaglio_tecnico")
        Stb.Append("                    where piezo2 <> 0 " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select 0 as ricetta_operazione_cod, 0 as ff_cod " & vbCrLf)

        Stb.Append("                ) rDt    " & vbCrLf)
        Stb.Append("                 where rDt.ricetta_operazione_cod = Entita.Ricetta_Operazione_cod " & vbCrLf)

        Stb.Append("                 union " & vbCrLf)
        Stb.Append("                Select distinct " & vbCrLf)
        Stb.Append("                 8 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append("                 , case when rDt.ff_Cod=0 then null else cast( rDt.ff_Cod as varchar(100)) end as '*'   " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)

        Stb.Append("                    select id_Agenda, cast( FF_Classe as int) as ff_cod " & vbCrLf)
        Stb.Append("                    from Mov_Dettaglio_Tecnico " & vbCrLf)
        Stb.Append("                    where ff_classe <> 0 " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select id_Agenda, cast( piezo2 as int) as  ff_cod " & vbCrLf)
        Stb.Append("                    from Mov_Dettaglio_Tecnico")
        Stb.Append("                    where piezo2 <> 0 " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select 0 as id_Agenda, 0 as ff_Cod " & vbCrLf)

        Stb.Append("                ) rDt    " & vbCrLf)
        Stb.Append("                 where rDt.id_Agenda = Entita.Id_Agenda " & vbCrLf)

    End Sub
    Private Shared Sub LeggigetQuery_Layer_Avversità(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                 Select " & vbCrLf)
        Stb.Append("                 7 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append("                 , case when rDt.Av_Cod=0 then null else cast( rDt.Av_Cod as varchar(100)) end as '*'   " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)
        Stb.Append("                    select Ricetta_Operazione_cod, av_Cod " & vbCrLf)
        Stb.Append("                    from Ricette_Dettaglio_Tecnico  " & vbCrLf)
        Stb.Append("                    where av_cod <> 0  " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select 0 as ricetta_operazione_cod, 0 as Av_Cod " & vbCrLf)
        Stb.Append("                ) rDt    " & vbCrLf)
        Stb.Append("                 where rDt.ricetta_operazione_cod = Entita.Ricetta_Operazione_cod " & vbCrLf)
        Stb.Append("            union " & vbCrLf)
        Stb.Append("                 Select  " & vbCrLf)
        Stb.Append("                 7 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append("                 , case when rDt.Av_Cod=0 then null else cast( rDt.Av_Cod as varchar(100)) end as '*'   " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)
        Stb.Append("                    select a.id_Agenda, t.av_cod " & vbCrLf)
        Stb.Append("                    from Mov_Dettaglio_Tecnico t " & vbCrLf)
        Stb.Append("                    inner join  agenda a  " & vbCrLf)
        Stb.Append("                    on a.id_Agenda = t.id_agenda  " & vbCrLf)
        Stb.Append("                    where a.lav_cod =  " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & vbCrLf)
        Stb.Append("                    and t.av_cod <> 0  " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select 0 as id_Agenda, 0 as Av_Cod" & vbCrLf)
        Stb.Append("                ) rDt    " & vbCrLf)
        Stb.Append("                 where rDt.id_Agenda = Entita.id_agenda " & vbCrLf)

        Stb.AppendLine("        union ")
        Stb.AppendLine("         Select  ")
        Stb.AppendLine("                  7 as '@tipologia_layer'    ")
        Stb.AppendLine("                  , case when rDt.Av_Cod=0 then null else cast( rDt.Av_Cod as varchar(100)) end as '*'    ")
        Stb.AppendLine("               from( ")
        Stb.AppendLine("                  select distinct -1 as id_agenda, piva, sa_cod, appezza, id_reg, av_cod ")
        Stb.AppendLine("                     From DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
        Stb.AppendLine("                      inner Join DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d                      ")
        Stb.AppendLine("                         On  srv.Stazione_Cod = d.Stazione_Cod ")
        Stb.AppendLine("                      And srv.tipo_sorgente = d.tipo_sorgente ")
        Stb.AppendLine("               ) rDt  ")
        Stb.AppendLine("               where rDt.piva = entita.piva  ")
        Stb.AppendLine("               And rDt.sa_cod = entita.sa_Cod ")
        Stb.AppendLine("               And rDt.appezza = entita.appezza ")
        Stb.AppendLine("               And rDt.id_reg = entita.id_imp ")
        Stb.AppendLine("               And entita.tipoEntita_cod in (select TipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19)")

    End Sub
    Private Shared Sub LeggigetQuery_Layer_RilieviVegetoProduttivi(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                 Select " & vbCrLf)
        Stb.Append("                 9 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append("                 , case when rDt.ff_classe=0 then null else cast( rDt.ff_classe as varchar(100)) end as '*'   " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)
        Stb.Append("                    select a.id_Agenda, t.ff_classe " & vbCrLf)
        Stb.Append("                    from Mov_Dettaglio_Tecnico t " & vbCrLf)
        Stb.Append("                    inner join  agenda a  " & vbCrLf)
        Stb.Append("                    on a.id_Agenda = t.id_agenda  " & vbCrLf)
        Stb.Append("                    where (a.lav_cod =  " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & vbCrLf)
        Stb.Append("                    or a.lav_cod =  " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & ") " & vbCrLf)
        Stb.Append("                    and t.ff_classe <> 0  " & vbCrLf)
        Stb.Append("                    union  " & vbCrLf)
        Stb.Append("                    select 0 as id_Agenda, 0 as ff_classe" & vbCrLf)
        Stb.Append("                ) rDt    " & vbCrLf)
        Stb.Append("                 where rDt.id_Agenda = Entita.id_agenda " & vbCrLf)
        Stb.Append("            union  " & vbCrLf)
        Stb.Append(" 				select  " & vbCrLf)
        Stb.Append(" 				9 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append(" 				, cast(-2 as varchar(1000)) as '*'   " & vbCrLf)
        Stb.Append(" 				where Entita.TipoEntita_Cod in (select TipoEntita_Cod from GIS_TipoEntita  where LayerElementiGrafici_Cod = 19)" & vbCrLf)

    End Sub

    Private Shared Sub LeggigetQuery_Layer_AnalisiPeriodicitaAgenda(ByVal Stb As System.Text.StringBuilder)
        Stb.AppendLine("            union all  ")
        Stb.AppendLine("                select  distinct ")
        Stb.AppendLine("                11 as '@tipologia_layer'  ")
        Stb.AppendLine("                , cast(lav_cod as varchar(1000)) as '*'   ")
        Stb.AppendLine("                from ( ")
        Stb.AppendLine("        Select ")
        Stb.AppendLine("         reg.LAV_COD ")
        Stb.AppendLine("       , reg.LAV_DES  ")
        Stb.AppendLine("       , reg.Piva ")

        Stb.AppendLine(" from ( ")

        LeggiGetQuery_AnalisiPeriodicitaAgenda_Int(Stb)

        Stb.AppendLine("    ) reg  ")

        Stb.AppendLine("    where reg.PIVA = Entita.Piva   ")
        Stb.AppendLine("    And reg.SA_COD = Entita.Sa_Cod   ")
        Stb.AppendLine("    And reg.APPEZZA = Entita.Appezza   ")
        Stb.AppendLine("    And reg.ID_REG = entita.id_imp    ")
        'Stb.AppendLine("    And reg.ID_Mov_Det = entita.ID_Mov_Det   ")
        Stb.AppendLine("    And ( (reg.id_mov_det > 0 And entita.id_mov_det = reg.Id_Mov_Det ) Or (reg.Id_Mov_Det = -1 And entita.tipoEntita_Cod in (select tipoEntita_Cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 ) ))  ")

        Stb.AppendLine(" ) reg1  ")

    End Sub
    Private Shared Sub LeggigetQuery_Layer_SpecieVegetale(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                Select  distinct " & vbCrLf)
        Stb.Append("                5 As '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                , cast(veg_cod as varchar(1000)) as '*'   " & vbCrLf)
        Stb.Append("                from reg_impianti reg  " & vbCrLf)
        Stb.Append("                    inner join ( select 0 as Cul_Cod, 0 as Veg_Cod union all select Cul_Cod, Veg_Cod from cultivar) cul on reg.cul_cod = cul.Cul_Cod   " & vbCrLf)
        Stb.Append("                where reg.PIVA = Entita.Piva   " & vbCrLf)
        Stb.Append("                and reg.SA_COD = Entita.Sa_Cod   " & vbCrLf)
        Stb.Append("                and reg.APPEZZA = Entita.Appezza   " & vbCrLf)
        Stb.Append("                and reg.ID_REG = entita.id_imp    " & vbCrLf)
        Stb.Append("            union " & vbCrLf)
        Stb.Append("                Select distinct " & vbCrLf)
        Stb.Append("                5 as '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                , cast(cul.veg_cod as varchar(1000)) as '*'  " & vbCrLf)
        Stb.Append("                from Programmazione_Entita entl " & vbCrLf)
        Stb.Append("                    inner join ( select 0 as Cul_Cod, 0 as Veg_Cod union all select Cul_Cod, Veg_Cod from cultivar) cul on entl.cul_cod = cul.cul_cod " & vbCrLf)
        Stb.Append("                where entl.Programmazione_Entita_Cod = Entita.Programmazione_Entita_COD " & vbCrLf)
        Stb.Append("            union  " & vbCrLf)
        Stb.Append(" 				select  " & vbCrLf)
        Stb.Append(" 				5 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append(" 				, cast(-1 as varchar(1000)) as '*'   " & vbCrLf)
        Stb.Append(" 				where Entita.TipoEntita_Cod in (3, 5)" & vbCrLf)
    End Sub


    Private Shared Sub LeggigetQuery_Layer_Cultivar(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                Select  distinct " & vbCrLf)
        Stb.Append("                15 As '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                , cast(cul.cul_cod as varchar(1000)) as '*'   " & vbCrLf)
        Stb.Append("                from reg_impianti reg  " & vbCrLf)
        Stb.Append("                    inner join ( select 0 as Cul_Cod, 0 as Veg_Cod union all select Cul_Cod, Veg_Cod from cultivar) cul on reg.cul_cod = cul.Cul_Cod   " & vbCrLf)
        Stb.Append("                where reg.PIVA = Entita.Piva   " & vbCrLf)
        Stb.Append("                and reg.SA_COD = Entita.Sa_Cod   " & vbCrLf)
        Stb.Append("                and reg.APPEZZA = Entita.Appezza   " & vbCrLf)
        Stb.Append("                and reg.ID_REG = entita.id_imp    " & vbCrLf)
        Stb.Append("            union " & vbCrLf)
        Stb.Append("                Select distinct " & vbCrLf)
        Stb.Append("                15 as '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                , cast(cul.cul_cod as varchar(1000)) as '*'  " & vbCrLf)
        Stb.Append("                from Programmazione_Entita entl " & vbCrLf)
        Stb.Append("                    inner join ( select 0 as Cul_Cod, 0 as Veg_Cod union all select Cul_Cod, Veg_Cod from cultivar) cul on entl.cul_cod = cul.cul_cod " & vbCrLf)
        Stb.Append("                where entl.Programmazione_Entita_Cod = Entita.Programmazione_Entita_COD " & vbCrLf)
        Stb.Append("            union  " & vbCrLf)
        Stb.Append(" 				select  " & vbCrLf)
        Stb.Append(" 				15 as '@tipologia_layer'   " & vbCrLf)
        Stb.Append(" 				, cast(-1 as varchar(1000)) as '*'   " & vbCrLf)
        Stb.Append(" 				where Entita.TipoEntita_Cod in (3, 5)" & vbCrLf)
    End Sub




    Private Shared Sub LeggigetQuery_Layer_Percorsi(ByVal Stb As System.Text.StringBuilder)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                select  distinct " & vbCrLf)
        Stb.Append("                10 as '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                , cast(Reg.Codice as varchar(1000)) as '*'   " & vbCrLf)

        'Stb.Append("                from (select case when Entita.TipoEntita_cod = 0 then case when charindex('isStop§ 1', geodata.ElementoGrafico_DES , 0)>1 then 81 else 83 end else 82 end as Codice ) reg  " & vbCrLf)

        Stb.Append("                from (  " & vbCrLf)
        Stb.Append("                    select   " & vbCrLf)
        Stb.Append("                        case when Entita.TipoEntita_cod = 0 then   " & vbCrLf)
        Stb.Append("                            case when Entita.piva <> '' then 85 else   " & vbCrLf)
        Stb.Append("                                case when charindex('isStop§ 1', geodata.ElementoGrafico_DES , 0)>1 then 81   " & vbCrLf)
        Stb.Append("                                    else 83   " & vbCrLf)
        Stb.Append("                            End " & vbCrLf)
        Stb.Append("                        End " & vbCrLf)
        Stb.Append("                        else   " & vbCrLf)
        Stb.Append("                            case when entita.TipoEntita_cod in (select tipoEntita_Cod from gis_tipoEntita where layerElementigrafici_cod = 19) then 82   " & vbCrLf)
        Stb.Append("                                else   " & vbCrLf)
        Stb.Append("                                    case when entita.TipoEntita_cod = 63 then 84   " & vbCrLf)
        Stb.Append("                                End " & vbCrLf)
        Stb.Append("                            End " & vbCrLf)
        Stb.Append("                        End " & vbCrLf)
        Stb.Append("                 AS Codice ) reg   " & vbCrLf)
        Stb.Append(" ")


    End Sub
    Private Shared Sub LeggigetQuery_Layer_Standard(ByVal Stb As System.Text.StringBuilder,
                                                    objparametri As AgronicaCoreParametri,
                                                    leggiLayerNonVisibili As Boolean)

        Stb.Append("                select   " & vbCrLf)
        Stb.Append("                1 as '@tipologia_layer'  " & vbCrLf)
        Stb.Append("                ,case when Entita.layerElementiGrafici_cod_Riproiettato <> -1 then cast( Entita.layerElementiGrafici_cod_Riproiettato as varchar(1000)) else cast(geodata.layerElementiGrafici_cod as varchar(1000)) end as '*'                 " & vbCrLf)
        Stb.AppendLine("              From GIS_LayerElementiGrafici l1 ")
        Stb.AppendLine("              Where l1.LayerElementiGrafici_Cod = geodata.LayerElementiGrafici_Cod ")
        Stb.AppendLine("              And l1.Utente = '" & objparametri.UtenteUsername & "' " & vbCrLf)
        If Not leggiLayerNonVisibili Then
            Stb.AppendLine("              And l1.flag_visibile <> 0  ")
        End If
        Stb.AppendLine("              And l1.flag_attivo <> 0  ")
        Stb.AppendLine("              And l1.TipologiaLayer_cod = 1")

    End Sub

    Private Shared Sub LeggigetQuery_Layer_StrutturaDiAppertenenza(ByVal Stb As System.Text.StringBuilder, ByVal Sementieri_Sportello_Configurazione_cod As Integer)
        Stb.Append("            union all  " & vbCrLf)
        Stb.Append("                select   " & vbCrLf)
        Stb.Append("                    100 as '@tipologia_layer'  " & vbCrLf)

        Stb.Append("                    , GU.Gruppi_Utente_cod as '*'  " & vbCrLf)

        'If Sementieri_Sportello_Configurazione_cod = "-1" Then
        'Stb.Append("                , cast(coalesce(reg1.codice_Fiscale_tecnico, coalesce(ent.codice_fiscale_tecnico, E1.padre))  as varchar(1000)) as '*'  " & vbCrLf)
        'Else
        'Stb.Append("                , cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico)  as varchar(1000)) as '*'  " & vbCrLf)
        'End If
    End Sub

    Private Shared Function LeggiGetQueryCondizioniWhere_GetPiveDatoFiltrone(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt_Imprese As DataTable

        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone

        ''----------------------------------------------------------------
        ''--- Filtro associato all'utente 
        ''----------------------------------------------------------------

        Dim xFiltroAggiuntivo As String = ""
        'If FiltroxUtente <> "" Then
        '    'se l'utente ha un filtro associato

        '    'concateno al filtro scelto, la stringa dei permessi dell'utente

        '    xFiltroAggiuntivo = FiltroxUtente

        'Per il momento imposto sempre a true perchè può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        'End If

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(xFiltroAggiuntivo, ClassJoin)
        'HttpContext.Current.Session("ASG_objParametri_Server")

        Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_server,
                                                    xFiltroAggiuntivo,
                                                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                    "",
                                                    ClassJoin)

        Dim listaPiva As String = ""

        Dim vLista As List(Of String) = (
            From dd In Dt_Imprese.AsEnumerable
            Select "'" & CStr(dd("piva")) & "'").ToList

        Return String.Join(",", vLista)

    End Function

    'Public Shared Sub LeggiGetQueryCondizioniWhere(ByVal PivaSuperUser As String, ByVal Entita_Cod As Int32, ByVal TipoEntita_Cod As Int32, ByVal PivaPadre As String, ByVal Piva As String, ByVal Sa_Cod As Int32, ByVal Appezza As Int32, ByVal Campo_Cod As Int32, ByVal ID_Imp As Int32, ByVal Prov As String, ByVal Com As String, ByVal Sezione As String, ByVal Foglio As Int32, ByVal Numero As Int32, ByVal Subalterno As String, ByVal ID_Agenda As Integer, ByVal Programmazione_cod As Integer, ByVal Programmazione_Entita_cod As Integer, ByVal Ricetta_Operazione_Cod As Integer, ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Stb As System.Text.StringBuilder, ByVal isXml As Boolean, ByVal Sementieri_Sportello_Configurazione_cod As Integer)
    Public Sub LeggiGetQueryCondizioniWhere(ByVal PivaSuperUser As String, ByVal Entita_Cod As Int32, ByVal TipoEntita_Cod As Int32, ByVal PivaPadre As String, ByVal Piva As String, ByVal Sa_Cod As Int32, ByVal Appezza As Int32, ByVal Campo_Cod As Int32, ByVal ID_Imp As Int32, ByVal Prov As String, ByVal Com As String, ByVal Sezione As String, ByVal Foglio As Int32, ByVal Numero As Int32, ByVal Subalterno As String, ByVal ID_Agenda As Integer, ByVal Programmazione_cod As Integer, ByVal Programmazione_Entita_cod As Integer, ByVal Ricetta_Operazione_Cod As Integer, ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Stb As System.Text.StringBuilder, ByVal isXml As Boolean, ByVal Sementieri_Sportello_Configurazione_cod As Integer, ByVal Codice_Fiscale_Tecnico As String, ByVal wktBoundaySTIntersects As String, ByVal FiltroAggiuntivo_EscludeAltriFiltri As Boolean)

        Stb.Append(" WHERE   1=1 " & vbCrLf)

        ' VAnni: 2/3/2017: se devo leggere un entita singola allora tutti gli altri filtri sono superflui.
        If Entita_Cod <> 0 Then
            Stb.Append(" AND Entita.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " " & vbCrLf)
            Exit Sub
        End If

        ' VAnni: 2/3/2017: decide in comportament
        If Not FiltroAggiuntivo_EscludeAltriFiltri Then

            Stb.Append(" AND ")

            Dim andImpostato As Boolean = True

            'il valore CF_TEC identifica l'esclusione del filtro per il codice fiscale tecnico, così come arriva da configurazione_siti in DB Server.
            If PivaSuperUser <> Codice_Fiscale_Tecnico AndAlso Codice_Fiscale_Tecnico <> "CF TEC" Then

                andImpostato = False

                'C_F
                Stb.Append(" ( " & vbCrLf)
                Stb.Append("  (reg1.codice_fiscale_tecnico is not null AND reg1.codice_fiscale_tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ) " & vbCrLf)
                Stb.Append("  OR (ent.codice_fiscale_tecnico is not null AND ent.codice_fiscale_tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ) " & vbCrLf)
                Stb.Append("  OR (reg1.codice_fiscale_tecnico is null   AND ent.codice_fiscale_tecnico is null) " & vbCrLf)

                If Sementieri_Sportello_Configurazione_cod <> -1 Then
                    Stb.Append(" OR coalesce(filtroSementi.Visibilita_Impianti, 1) = 1 " & vbCrLf)
                End If

                Stb.Append(" ) " & vbCrLf)
                '
            End If

            If isXml Then
                If Not andImpostato Then
                    Stb.Append(" and ")
                End If
                Stb.Append(" Entita.PivaSuperUser is not null " & vbCrLf)
                andImpostato = False
            End If

            If Not andImpostato Then
                Stb.Append(" and ")
            End If

            Stb.Append(" Entita.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)

            If TipoEntita_Cod <> 0 Then
                Stb.Append(" AND Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(TipoEntita_Cod) & " " & vbCrLf)
            End If

            'If Sementieri_Sportello_Configurazione_cod <> -1 Then
            '    Stb.Append(" AND coalesce(reg1.Validita_inizio, ent.Validita_inizio) >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  " & vbCrLf)
            '    Stb.Append(" AND coalesce(reg1.Validita_fine, ent.Validita_fine) <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "    " & vbCrLf)
            'End If

            If Piva <> "" Then

                If isXml Then

                    If Sementieri_Sportello_Configurazione_cod <> -1 Then
                        Stb.Append("    AND  " & vbCrLf)
                        Stb.Append("    ( " & vbCrLf)
                        Stb.Append("        ( " & vbCrLf)
                        Stb.Append("            coalesce(filtroSementi.Visibilita_Impianti, 1) = 0 and coalesce(reg1.Codice_Fiscale_Tecnico, ent.Codice_Fiscale_Tecnico) =  '" & Agro_SQL_SaveText(PivaPadre) & "' OR '" & Agro_SQL_SaveText(PivaPadre) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "'" & vbCrLf)
                        Stb.Append("        ) " & vbCrLf)
                        Stb.Append("        OR coalesce(filtroSementi.Visibilita_Impianti, 1) = 1 " & vbCrLf)
                        Stb.Append("    ) " & vbCrLf)

                        '' VAnni: 6/4/2018: gestito nella lettura dei singoli layers
                        'Else
                        '    Stb.Append("        OR  (  " & vbCrLf)
                        '    Stb.Append("             pp.prov = Entita.prov " & vbCrLf)
                        '    Stb.Append("         and pp.com = Entita.COM   " & vbCrLf)
                        '    Stb.Append("         and pp.SEZIONE = Entita.SEZIONE   " & vbCrLf)
                        '    Stb.Append("         and pp.Foglio = Entita.Foglio   " & vbCrLf)
                        '    Stb.Append("         and pp.NUMERO = Entita.NUMERO   " & vbCrLf)
                        '    Stb.Append("         and pp.SUBALTERNO = Entita.SUBALTERNO " & vbCrLf)
                        '    Stb.Append("         and pp.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
                        '    If Sa_Cod <> 0 Then
                        '        Stb.Append("        AND pp.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                        '    End If
                        '    Stb.Append("        ) " & vbCrLf)

                    End If


                Else
                    Stb.Append(" AND Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    If Sa_Cod <> 0 Then
                        Stb.Append("        AND Entita.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If
                End If

            End If

            If Appezza <> 0 Then
                Stb.Append(" AND Entita.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Campo_Cod <> 0 Then
                Stb.Append(" AND Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            End If

            If ID_Imp <> 0 Then
                Stb.Append(" AND Entita.ID_Imp = " & Agro_SQL_SaveNum(ID_Imp) & " ")
            End If

            If Prov <> "" Then
                Stb.Append(" AND Entita.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                Stb.Append(" AND Entita.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "-1" Then
                Stb.Append(" AND Entita.Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If

            If Foglio <> -1 Then
                Stb.Append(" AND Entita.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> -1 Then
                Stb.Append(" AND Entita.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "-1" Then
                Stb.Append(" AND Entita.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            If ID_Agenda <> 0 Then
                Stb.Append(" AND Entita.ID_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                Stb.Append(" AND Entita.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            End If

            If Programmazione_cod <> 0 Then
                Stb.Append(" AND Entita.Programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_cod) & " ")
            End If

            If Programmazione_Entita_cod <> 0 Then
                Stb.Append(" AND Entita.Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_Entita_cod) & " ")
            End If

        End If

        'lavez - 17/11/2023 - spostato
        '' Vanni: 27/11/2017: intersezione per caricare solo quello che sta sulla porzione di mappa
        'If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
        '    Stb.AppendLine(" AND GeoDataIntersezione.Poligono_GeoEntity.STIntersects( ")
        '    Stb.AppendLine("     geography::STGeomFromText( ")
        '    Stb.AppendLine("      '" & wktBoundaySTIntersects & "' ")
        '    Stb.AppendLine("      , 4326)          ")
        '    Stb.AppendLine("  ) = 1")

        'End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
        End If

    End Sub


    Private Sub LeggiGetQueryCondizioniWhere_PorzioneFiltroTemporale(
            ByVal TabellaAlias As String,
            ByVal ValiditaFineAlias As String,
            ByVal AggiungiOR_Iniziale As Boolean,
            ByVal FinestraTemporaleInizio As DateTime,
            ByVal FinestraTemporaleFine As DateTime,
            ByRef Stb As System.Text.StringBuilder,
            ByVal l_inf As String,
            ByVal l_sup As String,
            ByVal LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
            Optional ByVal xFiltroAggiuntivo As String = ""
        )

        If AggiungiOR_Iniziale Then
            Stb.Append(" OR ")
        End If

        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, enum_Gis_LayerElementiGrafici_std.Op_Agenda

                Stb.Append(" (" & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                Stb.Append(" AND " & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & "   " & vbCrLf)

            Case Else

                'vecchia gestione filtro temporale
                'Stb.Append(" (" & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " " & l_inf & " " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                'Stb.Append(" AND  " & TabellaAlias & "." & ValiditaFineAlias & " " & l_sup & Agro_SQL_SaveDate(FinestraTemporaleInizio) & " " & vbCrLf)

                Stb.Append(" (" & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                Stb.Append(" AND  " & TabellaAlias & "." & ValiditaFineAlias & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & " " & vbCrLf)

        End Select

        If xFiltroAggiuntivo <> "" Then
            Stb.Append(xFiltroAggiuntivo)
        End If

        Stb.Append(" ) " & vbCrLf)

    End Sub


    Public Function Max_Entita_cod(
           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                   ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Append(" select max(Entita_cod)as Entita_cod from GIS_Entita ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT.Rows(0).Item("Entita_cod")

    End Function


    Public Function GiasPalmDettagli(ByVal piva As String, ByVal sa_cod As String, ByVal id As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.GiasPalmDettagli()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine(" select piva, sa_cod, id, isNull(VX2, 0) as Perimetro, lat, lon, PDOP, Quota, isNull(VY2, 0) as Area,  case when [text] is null then replace(Proiezione, 'AA', 'Algoritmo Area poligono') else [text] end as dettagli ")
            Stb.AppendLine(" from grafica cart ")
            Stb.AppendLine(" where Cart.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" and Cart.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            Stb.AppendLine(" and Cart.id = '" & Agro_SQL_SaveText(id) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////

    Public Function getProprietaImpianto(
                            ByVal Piva As String,
                            ByVal sa_cod As Integer,
                            ByVal appezza As Integer,
                            ByVal id_reg As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine(" SELECT         r.Piva AS Piva ")
            Stb.AppendLine("              , r.Sa_Cod AS Sa_Cod ")
            Stb.AppendLine("              , r.Appezza AS Appezza ")
            Stb.AppendLine("              , r.id_Reg AS ID_imp ")

            Stb.AppendLine("              , isnull(r.sup_imp, 0) AS Superficie ")

            Stb.AppendLine("              , isnull(r.Codice_fiscale_tecnico,'') AS Codice_fiscale_tecnico  ")
            Stb.AppendLine("              , isnull(r.CUL_COD, 0) AS Cul_Cod ")
            Stb.AppendLine("              , isnull(r.GRFI_COD, 0) AS Grfi_Cod ")
            Stb.AppendLine("              , isnull(r.GRVA_Cod_VEG, 0 ) as GRVA_Cod ")

            Stb.AppendLine("              , isnull(r.validita_inizio, '01-01-1900' ) as Validita_Inizio ")
            Stb.AppendLine("              , isnull(r.validita_fine, '31-12-2100' ) as Validita_Fine ")
            Stb.AppendLine("              , 0 as Flag_GPS ")
            Stb.AppendLine("              ,isnull(a.via_stringa, '' ) as via_stringa ")
            Stb.AppendLine("              , isnull((select top 1 Progetto_nome from Imprese_Progetti i where r.Piva = i.PIVA AND r.Sa_Cod = i.SA_COD AND r.Appezza = i.APPEZZA AND  r.id_reg = i.ID_REG order by progetto_cod desc ),'') as Progetto_nome ")

            Stb.AppendLine("              ,  '' as GiasPalm")

            Stb.AppendLine("              ,  '' as GiasPalm_piva")
            Stb.AppendLine("              ,  '' GiasPalm_sa_cod")
            Stb.AppendLine("              ,  '' GiasPalm_id")


            Stb.AppendLine(" FROM Reg_Impianti r  ")
            Stb.AppendLine(" inner join appezzamento a  ")


            Stb.AppendLine("          on a.piva = r.piva ")
            Stb.AppendLine("          and a.sa_cod = r.Sa_cod ")
            Stb.AppendLine("          and a.appezza = r.appezza")

            Stb.AppendLine("   where r.piva = '" & Agro_SQL_SaveText(Piva) & "'")
            Stb.AppendLine("   and r.sa_cod = " & sa_cod & " ")
            Stb.AppendLine("   and r.appezza = " & appezza & " ")
            Stb.AppendLine("   and r.id_reg = " & id_reg & " ")



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function getProprieta(
        ByVal Entita_Cod As Int32,
        ByVal TipoEntitaCod As TipiEnumerativi.enum_GIS2012_TipoEntita,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine(" SELECT         GIS_Entita.Piva AS Piva ")
            Stb.AppendLine("              , GIS_Entita.Sa_Cod AS Sa_Cod ")
            Stb.AppendLine("              , GIS_Entita.Appezza AS Appezza ")
            Stb.AppendLine("              , GIS_Entita.ID_imp AS ID_imp ")
            Stb.AppendLine("              , GIS_Entita.Programmazione_Entita_Cod AS Programmazione_Entita_Cod ")
            Stb.AppendLine("              , GIS_Entita.PivaSuperUser AS PivaSuperUser ")

            'Stb.AppendLine("              isnull(isnull(Programmazione_Entita.Superficie, Reg_Impianti.sup_imp ), 0) AS Superficie, ")
            If TipoEntitaCod = enum_GIS2012_TipoEntita.CATASTO Then
                Stb.AppendLine("           , (p.ETTARI * 10000 + p.ARE * 100 + p.CENTIARE) / 10000  AS Superficie ")
            Else
                Stb.AppendLine("           , coalesce(Programmazione_Entita.Superficie, Reg_Impianti.sup_imp, Appezzamento.SUP_APP, 0) AS Superficie ")
            End If


            Stb.AppendLine("             , isnull(isnull(Programmazione_Entita.Codice_fiscale_tecnico, Reg_Impianti.Codice_fiscale_tecnico ),'') AS Codice_fiscale_tecnico  ")
            Stb.AppendLine("             , isnull(isnull(Programmazione_Entita.Cul_Cod, Reg_Impianti.CUL_COD ), 0) AS Cul_Cod ")
            Stb.AppendLine("             , isnull(isnull(Programmazione_Entita.Grfi_Cod,  Reg_Impianti.GRFI_COD ), 0) AS Grfi_Cod ")
            Stb.AppendLine("             , isnull(isnull(Programmazione_Entita.Grva_Cod,  Reg_Impianti.GRVA_Cod_VEG ), 0 ) as GRVA_Cod ")

            If TipoEntitaCod = enum_GIS2012_TipoEntita.CATASTO Then

                'Esempio:
                'AR : I155 : SANSEPOLCRO : __ : ____17 : ____97 : __} ..... 4,4450 [ha] ..... Affitto con contratto

                Stb.AppendLine("         , '{' +  ")
                Stb.AppendLine("             ISTAT_Comuni.Provincia  + ' : ' + ")
                Stb.AppendLine("             ISTAT_Comuni.Cod_Belfiore + ' : ' + ")
                Stb.AppendLine("             ISTAT_Comuni.Descrizione  + ' : ' +  ")
                Stb.AppendLine("             right('___' + cast(p.Sezione as Varchar(3)), 3) + ' : '   + ")
                Stb.AppendLine("             right('______' + cast(p.Foglio as Varchar(6)), 6) + ' : ' + ")
                Stb.AppendLine("             right('______' + cast(p.Numero as Varchar(6)), 6) + ' : ' + ")
                Stb.AppendLine("             right('__' + cast(p.Subalterno as Varchar(2)), 2) + ")
                Stb.AppendLine("         '}'  ")

                Stb.AppendLine("                ")
                Stb.AppendLine("         as  ElementoGrafico_Des ")
            Else
                Stb.AppendLine("         , gg.ElementoGrafico_Des ")
            End If

            Stb.AppendLine("              , gg.LayerElementiGrafici_Cod ")
            Stb.AppendLine("              , gg.Flag_GPS ")
            Stb.AppendLine("              , GIS_Entita.TipoEntita_Cod   ")


            Stb.AppendLine("              , isnull(isnull(Programmazione_Entita.validita_inizio,  Reg_Impianti.validita_inizio ), '01-01-1900' ) as Validita_Inizio ")
            Stb.AppendLine("              , isnull(isnull(Programmazione_Entita.validita_fine,  Reg_Impianti.validita_fine ), '31-12-2100' ) as Validita_Fine ")

            Stb.AppendLine("              , isnull(Appezzamento.Validita_Inizio, '01/01/1900' ) as Appezzamento_Validita_Inizio ")
            Stb.AppendLine("              , isnull(Appezzamento.Validita_Fine, '31/12/2100' ) as Appezzamento_Validita_Fine ")


            'Stb.AppendLine("              , isnull(isnull(Programmazione_Entita.via_stringa,  appezzamento.via_stringa ), '' ) as via_stringa ")
            Stb.AppendLine("              , isnull(appezzamento.via_stringa, '' ) as via_stringa ")
            Stb.AppendLine("              , isnull((select top 1 Progetto_nome from Imprese_Progetti i where GIS_Entita.Piva = i.PIVA AND GIS_Entita.Sa_Cod = i.SA_COD AND GIS_Entita.Appezza = i.APPEZZA AND  GIS_Entita.Id_Imp = i.ID_REG order by progetto_cod desc ),'') as Progetto_nome ")

            Stb.AppendLine("              , case when Cart.id is null then '' else 'Misurato con GIAS PALM' end as GiasPalm")

            Stb.AppendLine("              , case when cart.piva is not null then cart.piva else '' end as GiasPalm_piva")
            Stb.AppendLine("              , case when cart.sa_cod is not null then cart.sa_cod else '' end as GiasPalm_sa_cod")
            Stb.AppendLine("              , case when cart.id is not null then cart.id else '' end as GiasPalm_id")
            Stb.AppendLine("              , appezzamento.app_nome")
            Stb.AppendLine("			  , destinazione_uso = coalesce(ric.descrizione, '') ")

            Stb.AppendLine(" From GIS_Entita  ")
            Stb.AppendLine(" inner Join gis_elementiGrafici gg on GIS_Entita.Entita_cod = gg.Entita_cod ")


            If TipoEntitaCod = enum_GIS2012_TipoEntita.CATASTO Then
                Stb.AppendLine(" inner Join particelleCatastali p ")
                Stb.AppendLine("     On p.prov = GIS_Entita.PROV ")
                Stb.AppendLine("  And p.com = GIS_Entita.COM ")
                Stb.AppendLine("  And p.SEZIONE = GIS_Entita.SEZIONE ")
                Stb.AppendLine("  And p.FOGLIO = GIS_Entita.FOGLIO ")
                Stb.AppendLine("  And p.NUMERO = GIS_Entita.NUMERO ")
                Stb.AppendLine("  And p.SUBALTERNO = GIS_Entita.SUBALTERNO")

                Stb.AppendLine("  INNER Join ISTAT_Comuni  ")
                Stb.AppendLine("     On ISTAT_Comuni.Pro_Cod_Istat = p.PROV  ")
                Stb.AppendLine("  And ISTAT_Comuni.Com_Cod_Istat = p.COM ")
                Stb.AppendLine(" ")

            End If

            Stb.AppendLine(" Left Join Appezzamento  ")
            Stb.AppendLine("     On GIS_Entita.Piva = Appezzamento.PIVA  ")
            Stb.AppendLine("  And GIS_Entita.Sa_Cod = Appezzamento.SA_COD  ")
            Stb.AppendLine("  And GIS_Entita.Appezza = Appezzamento.APPEZZA  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Left OUTER JOIN  Reg_Impianti  ")
            Stb.AppendLine("     On GIS_Entita.Piva = Reg_Impianti.PIVA  ")
            Stb.AppendLine("  And GIS_Entita.Sa_Cod = Reg_Impianti.SA_COD  ")
            Stb.AppendLine("  And GIS_Entita.Appezza = Reg_Impianti.APPEZZA  ")
            Stb.AppendLine("  And  GIS_Entita.Id_Imp = Reg_Impianti.ID_REG  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Left OUTER JOIN Programmazione_Entita  ")
            Stb.AppendLine("     On GIS_Entita.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod  ")
            Stb.AppendLine("  And GIS_Entita.PivaSuperUser = Programmazione_Entita.Piva_SuperUser ")

            Stb.AppendLine(" Left OUTER JOIN  ( ")
            Stb.AppendLine("    select ric.PIVA, ric.sa_cod, ric.appezza, ric.Id_Reg, ca.descrizione ")
            Stb.AppendLine("    from Reg_Impianti_Codici ric ")
            Stb.AppendLine("    inner join codici_anagrafe ca on ca.codice = ric.id_cod ")
            Stb.AppendLine("    where ric.id_cod >= 3000 and ric.id_cod < 4000 ")
            Stb.AppendLine(" ) ric On GIS_Entita.Piva = ric.PIVA  ")
            Stb.AppendLine("  And GIS_Entita.Sa_Cod = ric.SA_COD  ")
            Stb.AppendLine("  And GIS_Entita.Appezza = ric.APPEZZA  ")
            Stb.AppendLine("  And  GIS_Entita.Id_Imp = ric.ID_REG  ")
            Stb.AppendLine("  ")


            Stb.AppendLine("            left join ( ")
            Stb.AppendLine("              select piva, sa_cod, id ")
            Stb.AppendLine("              from grafica ")
            Stb.AppendLine("              group by piva, sa_cod, id        ")
            Stb.AppendLine("         ) Cart  ")
            Stb.AppendLine("          on Cart.piva = GIS_Entita.piva ")
            Stb.AppendLine("          and Cart.sa_cod = GIS_Entita.Sa_cod ")
            Stb.AppendLine("          and Cart.id = GIS_Entita.OLDGrafica_ID")


            Stb.AppendLine("           where GIS_Entita.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            If Entita_Cod <> 0 Then
                Stb.AppendLine("       AND GIS_Entita.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function
    Public Function CaricaCodiciAnagrafeViaChiaveAlbero(
                            ByVal piva As String,
                            ByVal sa_cod As Integer,
                            ByVal appezza As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal id_reg As Integer,
                            ByVal gruppo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select c.id_cod, c.val_cod ")


            Select Case gruppo.ToUpper

                Case "CENTRO"

                    Stb.AppendLine(" From Centri_Aziendali_Codici c ")
                    Stb.AppendLine(" where c.piva = '" & Agro_SQL_SaveText(piva) & "'")
                    Stb.AppendLine("      And c.sa_cod = " & sa_cod)

                Case "CAMPO"
                    Stb.AppendLine(" From Campi_Codici c ")
                    Stb.AppendLine(" where c.piva = '" & Agro_SQL_SaveText(piva) & "'")
                    Stb.AppendLine("      And c.sa_cod = " & sa_cod)
                    Stb.AppendLine("      And c.Campo_Cod = " & Campo_Cod)

                Case "APPEZZA"

                    Stb.AppendLine(" From Appezzamento_Codici c ")
                    Stb.AppendLine(" where c.piva = '" & Agro_SQL_SaveText(piva) & "'")
                    Stb.AppendLine("      And c.sa_cod = " & sa_cod)
                    Stb.AppendLine("      And c.appezza = " & appezza)

                Case "IMPIANTI", "IMPIANTO"
                    Stb.AppendLine(" From Reg_Impianti_Codici c ")
                    Stb.AppendLine(" where c.piva = '" & Agro_SQL_SaveText(piva) & "'")
                    Stb.AppendLine("      And c.sa_cod = " & sa_cod)
                    Stb.AppendLine("      And c.appezza = " & appezza)
                    Stb.AppendLine("      And c.Id_Reg = " & id_reg)

                Case Else
                    Stb.AppendLine(" From Imprese_Codici c ")
                    Stb.AppendLine(" where c.piva = '" & Agro_SQL_SaveText(piva) & "'")

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function CaricaCodiciAnagrafe(
                            ByVal Entita_Cod As Int32,
                            ByVal Gruppo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            Stb.Length = 0

            Stb.AppendLine(" Select distinct c.id_cod, c.val_cod ")
            Stb.AppendLine(" From gis_entita e ")

            Select Case Gruppo.ToUpper

                Case "CENTRO"
                    Stb.AppendLine("  inner Join Centri_Aziendali_Codici c ")
                    Stb.AppendLine("         On e.piva = c.piva ")
                    Stb.AppendLine("      And e.Sa_Cod = c.sa_cod ")

                Case "CAMPO"
                    Stb.AppendLine("  inner Join Campi_Codici c ")
                    Stb.AppendLine("         On e.piva = c.piva ")
                    Stb.AppendLine("      And e.Sa_Cod = c.sa_cod ")
                    Stb.AppendLine("      And e.Campo_Cod = c.Campo_cod ")

                Case "APPEZZA"
                    Stb.AppendLine("  inner Join Appezzamento_Codici c ")
                    Stb.AppendLine("         On e.piva = c.piva ")
                    Stb.AppendLine("      And e.Sa_Cod = c.sa_cod ")
                    Stb.AppendLine("      And e.Appezza = c.appezza ")

                'doppia voce per retro-compatibilità con client
                Case "IMPIANTI", "IMPIANTO"
                    Stb.AppendLine("  inner Join Reg_Impianti_Codici c ")
                    Stb.AppendLine("         On e.piva = c.piva ")
                    Stb.AppendLine("      And e.Sa_Cod = c.sa_cod ")
                    Stb.AppendLine("      And e.Appezza = c.appezza ")
                    Stb.AppendLine("      And e.Id_Imp = c.Id_Reg ")

                Case Else
                    Stb.AppendLine("  inner Join Imprese_Codici c ")
                    Stb.AppendLine("         On e.piva = c.piva ")
            End Select


            'Stb.AppendLine("      And c.progetto_cod = 0 ")

            Stb.AppendLine("       AND e.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi_Entita_Cod(
                            ByVal piva As String,
                            ByVal sa_cod As Integer,
                            ByVal programmazione_cod As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim xlayer As enum_Gis_LayerElementiGrafici_std
            If programmazione_cod = 0 Then
                xlayer = enum_Gis_LayerElementiGrafici_std.IMPIANTI
            Else
                xlayer = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
            End If

            Stb.Length = 0

            Stb.AppendLine(" select e.Entita_Cod ")
            Stb.AppendLine(" from gis_entita e ")
            Stb.AppendLine("    inner join  gis_elementiGrafici g ")
            Stb.AppendLine("    on  e.entita_cod = g.entita_cod ")
            Stb.AppendLine(" where e.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" and e.Sa_Cod =  " & sa_cod)
            Stb.AppendLine(" and g.layerElementiGrafici_cod = " & CInt(xlayer))

            If programmazione_cod <> 0 Then
                Stb.AppendLine(" and programmazione_Cod = " & programmazione_cod)
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi_Entita_Cod_Da_Chiave_PC(
                            ByVal piva As String,
                            ByVal sa_cod As Integer,
                            ByVal appezza As Integer,
                            ByVal id_reg As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi_Entita_Cod_Da_Chiave_PC()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select e.Entita_Cod, e.TipoEntita_Cod, g.ElementoGrafico_Cod, Poligono_GeoEntity.STAsText() as wkt ")
            Stb.AppendLine(" from gis_entita e ")
            Stb.AppendLine("    inner join gis_elementigrafici g ")
            Stb.AppendLine("    on  e.entita_cod = g.entita_cod ")
            Stb.AppendLine(" where e.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" and e.Sa_Cod =  " & Agro_SQL_SaveNum(sa_cod))
            Stb.AppendLine(" and e.appezza =  " & Agro_SQL_SaveNum(appezza))
            Stb.AppendLine(" and e.id_imp =  " & Agro_SQL_SaveNum(id_reg))
            Stb.AppendLine(" and g.layerElementiGrafici_cod = " & CInt(enum_Gis_LayerElementiGrafici_std.IMPIANTI))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function PlanningRibaltaFastLeggiEntita(
                            ByVal Programmazione_Entita_cod As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try



            Stb.Length = 0

            Stb.AppendLine(" select e.Entita_Cod ")
            Stb.AppendLine(" from gis_entita e ")
            Stb.AppendLine(" where e.Programmazione_Entita_Cod = " & Programmazione_Entita_cod & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function TipoEntita_Cod_Leggi_Dato_Cul_Cod(
        ByVal cul_cod As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer


        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim rval As Integer = 23

        Try



            Stb.Length = 0


            Stb.AppendLine(" select Case when veg.Gru_Cod Is null then 21 else ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 1 then 22 else   ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 2 then 20 else   ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 3 then 19 else 23    ")
            Stb.AppendLine("  End End End End As Tipoentita_Cod ")

            Stb.AppendLine("  from Cultivar cul  ")
            Stb.AppendLine("        ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  inner Join SpecieVegetali veg ")
            Stb.AppendLine("         On veg.Veg_Cod = cul.Veg_Cod")

            Stb.AppendLine("  where cul.cul_cod =   " & cul_cod)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                rval = DT(0)("TipoEntita_cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval

    End Function


    Public Function TipoEntita_Cod_Leggi_Da_Impianto(
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        ByVal id_reg As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer


        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim rval As Integer = 23

        Try



            Stb.Length = 0


            Stb.AppendLine(" select Case when veg.Gru_Cod Is null then 21 else ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 1 then 22 else   ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 2 then 20 else   ")
            Stb.AppendLine("  Case when veg.Gru_Cod = 3 then 19 else 23    ")
            Stb.AppendLine("  End End End End As Tipoentita_Cod ")

            Stb.AppendLine("  from Reg_Impianti ent ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  Left Join Cultivar cul  ")
            Stb.AppendLine("         On cul.Cul_Cod = ent.Cul_Cod ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  Left Join SpecieVegetali veg ")
            Stb.AppendLine("         On veg.Veg_Cod = cul.Veg_Cod")


            Stb.AppendLine(" Where ent.piva = '" & Agro_SQL_SaveText(piva) & "'")
            Stb.AppendLine(" and ent.sa_cod = " & sa_cod & "")
            Stb.AppendLine(" and ent.appezza = " & appezza & "")
            Stb.AppendLine(" and ent.id_reg = " & id_reg & "")



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                rval = DT(0)("TipoEntita_cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval

    End Function


    Public Function AnalisiLeggiDatoEntitaCod(
        ByVal entita_cod As String,
        ByRef ID_PDC_Testata As Integer,
        ByRef ID_PDC_Dettagli As Integer,
        ByRef ID_PDC_Campione As Integer,
        ByRef Analisi_Testata_Cod As Integer,
        ByRef piva As String,
        ByRef sa_Cod As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim rval As Boolean

        Try



            Stb.Length = 0
            Stb.AppendLine(" Select ")
            Stb.AppendLine("      t.Analisi_Testata_Cod ")
            Stb.AppendLine("    , isnull(PDC_Analisi.ID_PDC_Testata, 0) as ID_PDC_Testata ")
            Stb.AppendLine("    , isnull(PDC_Analisi.ID_PDC_Campione, 0) as ID_PDC_Campione")
            Stb.AppendLine("    , isnull(PDC_Analisi.ID_PDC_Dettagli, 0) as ID_PDC_Dettagli ")
            Stb.AppendLine("    , e.piva ")
            Stb.AppendLine("    , e.sa_cod ")
            Stb.AppendLine(" From GIS_Entita e ")
            Stb.AppendLine("  inner Join GIS_ElementiGrafici gg ")
            Stb.AppendLine("         On gg.Entita_Cod = e.Entita_Cod ")
            Stb.AppendLine("  inner Join Analisi_CampionixDettagli dd ")
            Stb.AppendLine("         On e.analisi_campione_cod = dd.Analisi_Campione_Cod ")
            Stb.AppendLine("  inner Join Analisi_Testata t ")
            Stb.AppendLine("         On t.Analisi_Testata_Cod = dd.Analisi_Testata_Cod ")
            Stb.AppendLine("              ")
            Stb.AppendLine("     Left Join PDC_Analisi  ")
            Stb.AppendLine("                On PDC_Analisi.Analisi_Testata_Cod = t.Analisi_Testata_Cod  ")
            Stb.AppendLine("              And PDC_Analisi.PivaSuperUser = t.Analisi_SuperUser                  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          ")
            Stb.AppendLine("    ")
            Stb.AppendLine("   where e.Entita_Cod = " & entita_cod)



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                rval = True

                ID_PDC_Testata = DT.Rows(0)("ID_PDC_Testata")
                ID_PDC_Dettagli = DT.Rows(0)("ID_PDC_Dettagli")
                ID_PDC_Campione = DT.Rows(0)("ID_PDC_Campione")
                Analisi_Testata_Cod = DT.Rows(0)("Analisi_Testata_Cod")
                piva = DT.Rows(0)("piva")
                sa_Cod = DT.Rows(0)("sa_cod")

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval

    End Function

    Public Function LeggiDateSpecieDaEntitaCod(ByVal entita_cod As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiDateSpecieDaEntitaCod()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    ri.Validita_Inizio ")
            Stb.AppendLine("    , ri.Validita_Fine ")
            Stb.AppendLine("    , c.Veg_Cod ")
            Stb.AppendLine("    , ri.GRVA_Cod_VEG ")
            Stb.AppendLine("FROM GIS_Entita e ")
            Stb.AppendLine("INNER JOIN Reg_Impianti ri ON ri.PIVA = e.Piva AND ri.SA_COD = e.Sa_Cod AND ri.APPEZZA = e.Appezza AND ri.ID_REG = e.Id_Imp ")
            Stb.AppendLine("INNER JOIN Cultivar c ON c.Cul_Cod = ri.CUL_COD ")
            Stb.AppendLine("WHERE e.Entita_Cod = " & entita_cod)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiElementiGraficiCatasto(ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As XDocument

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiElementiGrafici()"

        Dim MessaggioErrore As String = ""

        Dim stb As New StringBuilder

        Dim XDoc As XDocument = Nothing

        Try

            stb.Clear()
            'strXmlDatiRemoti = strXmlDatiRemoti.Replace("<DatiEntita>", "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">")
            stb.AppendLine("WITH  XMLNAMESPACES ('http://www.opengis.net/gml' as gml,DEFAULT 'http://www.agronica.it/grafica/') ")

            stb.AppendLine("SELECT ")
            stb.AppendLine("	EntitaGIAS = ( ")
            stb.AppendLine("		SELECT ")
            stb.AppendLine("			PivaSuperUser = 'sostituire sul client' ")
            stb.AppendLine("			, Entita_Cod = 0 ")
            stb.AppendLine("			, Piva = '' ")
            stb.AppendLine("			, Sa_Cod = 0 ")
            stb.AppendLine("			, Appezza = 0 ")
            stb.AppendLine("			, Campo_Cod = 0 ")
            stb.AppendLine("			, Id_Imp = 0 ")
            stb.AppendLine("			, e.TipoEntita_Cod ")
            stb.AppendLine("			, PROV ")
            stb.AppendLine("			, COM ")
            stb.AppendLine("			, SEZIONE ")
            stb.AppendLine("			, FOGLIO ")
            stb.AppendLine("			, NUMERO ")
            stb.AppendLine("			, SUBALTERNO ")
            stb.AppendLine("			, Programmazione_Cod = 0 ")
            stb.AppendLine("			, Programmazione_Entita_Cod = 0 ")
            stb.AppendLine("			, Id_Agenda = 0 ")
            stb.AppendLine("			, id_mov_det = 0 ")
            stb.AppendLine("			, Ricetta_Operazione_Cod = 0 ")
            stb.AppendLine("			, analisi_testata_cod = 0 ")
            stb.AppendLine("			, analisi_campione_cod = 0 ")
            stb.AppendLine("			, OLDGrafica_ID = '' ")
            stb.AppendLine("			, e.Data_Creazione ")
            stb.AppendLine("			, e.Data_Modifica ")
            stb.AppendLine("			, e.Username_Creazione ")
            stb.AppendLine("			, e.Username_Modifica ")
            stb.AppendLine("			, e.Validita_Inizio ")
            stb.AppendLine("			, e.Validita_Fine ")
            stb.AppendLine("			, e.inviato ")
            stb.AppendLine("			, e.datainvio ")
            stb.AppendLine("		FOR XML PATH('DatoGias'), TYPE, ELEMENTS) ")
            stb.AppendLine("	, ( select '@ElementoGrafico_Cod' = '0',  Poligono_GeoEntity.AsGml()  for xml path('geodata'), type, elements)  ")
            stb.AppendLine("	, layers = ( select '@tipologia_layer' = '1', '3' FOR XML PATH('layer') , TYPE, ELEMENTS ) ")
            stb.AppendLine("FROM GIS_Entita e ")
            stb.AppendLine("INNER JOIN GIS_ElementiGrafici g ON g.Entita_Cod = e.Entita_Cod ")
            stb.AppendLine("WHERE (TipoEntita_Cod = 3) ")

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then

                stb.AppendLine("AND ( ")
                stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                stb.AppendLine(") ")
            End If

            stb.AppendLine("FOR XML PATH('Entita'), ROOT('DatiEntita'), ELEMENTS ")

            'sql.AppendLine("SELECT * FROM ( ")
            'sql.AppendLine("    SELECT ")
            'sql.AppendLine("    	PROV ")
            'sql.AppendLine("    	, COM ")
            'sql.AppendLine("    	, SEZIONE ")
            'sql.AppendLine("    	, FOGLIO ")
            'sql.AppendLine("    	, NUMERO ")
            'sql.AppendLine("    	, SUBALTERNO ")
            'sql.AppendLine("    	, GeoEntity = Poligono_GeoEntity.AsGml() ")
            'sql.AppendLine("		, Data_Modifica = elg.Data_Modifica ")
            'sql.AppendLine("    FROM GIS_Entita ent ")
            'sql.AppendLine("    INNER JOIN GIS_ElementiGrafici elg ON elg.Entita_Cod = ent.Entita_Cod ")
            'sql.AppendLine("    WHERE (TipoEntita_Cod = " & enum_Gis_LayerElementiGrafici_std.CATASTO & ") ")
            'sql.AppendLine(") T")

            'If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then

            '    sql.AppendLine("WHERE ")
            '    sql.AppendLine(xFiltroAggiuntivo)
            'End If

            'sql.AppendLine("FOR XML PATH('Entita'), ROOT('DatiEntita'), ELEMENTS ")

            XDoc = EseguiQuery_Lettura_XDoc(objParametri, stb.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            XDoc = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return XDoc
    End Function

    Public Function Leggi(PivaSuperUser As String,
                          Entita_Cod As Integer,
                          TipoEntita_Cod As enum_GIS2012_TipoEntita,
                          Piva As String,
                          Sa_Cod As Integer,
                          Appezza As Integer,
                          Campo_Cod As Integer,
                          Id_Imp As Integer,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaGisDAL.GIS_Entita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM GIS_Entita ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser  = '" & Agro_SQL_SaveText(PivaSuperUser) & "'   ")
            End If

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Entita_Cod  = " & Agro_SQL_SaveNum(Entita_Cod) & "   ")
            End If

            If TipoEntita_Cod <> 0 Then
                StrSQL.AppendLine(" AND TipoEntita_Cod  = " & Agro_SQL_SaveNum(TipoEntita_Cod) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva  = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod  = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza  = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Campo_Cod  = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
            End If

            If Id_Imp <> 0 Then
                StrSQL.AppendLine(" AND Id_Imp  = " & Agro_SQL_SaveNum(Id_Imp) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Entita_Cod ASC")
            End If

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

#Region "Nuove chiamate GIS"
    Public Function LeggiDocumentiGIS(
            ByVal PivaSuperUser As String,
            ByVal Entita_Cod As Int32,
            ByVal TipoEntita_Cod As Int32,
            ByVal PivaPadre As String,
            ByVal Piva As String,
            ByVal Sa_Cod As Int32,
            ByVal Appezza As Int32,
            ByVal Campo_Cod As Int32,
            ByVal Veg_Cod As Int32,
            ByVal DestinazioneUso_Cod As Int32,
            ByVal ID_Imp As Int32,
            ByVal Prov As String,
            ByVal Com As String,
            ByVal Sezione As String,
            ByVal Foglio As Int32,
            ByVal Numero As Int32,
            ByVal Subalterno As String,
            ByVal ID_Agenda As Int32,
            ByVal Ricetta_Operazione_Cod As Int32,
            ByVal Programmazione_Cod As Integer,
            ByVal Programmazione_Entita_cod As Integer,
            ByVal Sementieri_Sportello_Configurazione_cod As Integer,
            ByVal Codice_Fiscale_Tecnico As String,
            ByVal wktBoundaySTIntersects As String,
            ByVal cfgAlbero As ConfigurazioneAlbero,
            ByVal layerElementiGrafici_cod As List(Of Integer),
            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal calcola_AreaPerimetro As Boolean = False,
            Optional ByVal filtroTemporaleAvanzato As FiltroTemporaleAvanzato = Nothing,
            Optional ByVal ElencoLayerVisibili As LayerVisibiliUtenteTipologia = Nothing
        ) As DataTable

        Dim DT As New DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi("
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder

        Try

            'Carica configurazione albero

            _cfgAlbero = cfgAlbero

            'Carica filtro temporale

            _filtroTemporaleAvanzato = filtroTemporaleAvanzato

            'Carica layer singola data

            Dim objLeggiLayerAnagr = New GIS_LayerElementiGrafici_Anagrafica_R

            _dtLayerSingolaData = objLeggiLayerAnagr.Leggi(
                objParametri_Server.PivaSuperUser,
                0,
                0,
                "",
                "",
                "",
                "",
                "1",
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server)

            Dim setup_visualizzazione = New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim impostazioniUtente As New Impostazioni_Utente

            Dim impostazioniDB As String = ""
            Dim impostazioni_dt = setup_visualizzazione.Leggi(enum_Impostazioni_Utenti.UTENTE_GIS,
                                                     1,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     objParametri_Utenti)

            If impostazioni_dt IsNot Nothing AndAlso impostazioni_dt.Rows.Count > 0 Then
                impostazioniDB = impostazioni_dt.Rows(0)("Impostazione_Valore_1").ToString
            End If

            Dim impostazioniPart = impostazioniDB.Split("§")

            If impostazioniDB.IndexOf("ckAvversitaUsaPuntoInterno") <> -1 Then

                Dim usaPuntoInterno = Array.Find(Of String)(impostazioniPart, Function(s) s.StartsWith("ckAvversitaUsaPuntoInterno"))
                Dim upi As Boolean

                If Not Boolean.TryParse(usaPuntoInterno.Split("^")(1), upi) Then
                    Throw New Exception("Il parametro utente ckAvversitaUsaPuntoInterno deve essere di tipo Boolean")
                End If

                impostazioniUtente.UsaPuntoInterno = upi
            End If

            If impostazioniDB.IndexOf("ckAvversitaPuntoPoligono") <> -1 Then
                Dim usaPuntoPoligono = Array.Find(Of String)(impostazioniPart, Function(s) s.StartsWith("ckAvversitaPuntoPoligono"))
                Dim upp As Boolean

                If Not Boolean.TryParse(usaPuntoPoligono.Split("^")(1), upp) Then
                    Throw New Exception("Il parametro utente ckAvversitaPuntoPoligono deve essere di tipo Boolean")
                End If

                impostazioniUtente.PuntoPoligono = upp
            End If

            'Composizione ed esecuzione query

            StrSQL = LeggiElencoEntita_GetQuery(
                PivaSuperUser,
                Entita_Cod,
                TipoEntita_Cod,
                PivaPadre,
                Piva,
                Sa_Cod,
                Appezza,
                Campo_Cod,
                Veg_Cod,
                DestinazioneUso_Cod,
                ID_Imp,
                Prov,
                Com,
                Sezione,
                Foglio,
                Numero,
                Subalterno,
                ID_Agenda,
                Programmazione_Cod,
                Programmazione_Entita_cod,
                Ricetta_Operazione_Cod,
                xFiltroAggiuntivo,
                xOrderBy,
                objParametri_Server,
                objParametri_Utenti,
                StrSQL,
                True,
                Sementieri_Sportello_Configurazione_cod,
                calcola_AreaPerimetro,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                cfgAlbero,
                layerElementiGrafici_cod,
                impostazioniUtente,
                ElencoLayerVisibili)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            _filtroTemporaleAvanzato = Nothing
            _dtLayerSingolaData = Nothing
            _cfgAlbero = Nothing

        End Try

        Return DT

    End Function

    Private Function LeggiElencoEntita_GetQuery(
        ByVal PivaSuperUser As String,
        ByVal Entita_Cod As Int32,
        ByVal TipoEntita_Cod As Int32,
        ByVal PivaPadre As String,
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal Campo_Cod As Int32,
        ByVal Veg_Cod As Int32,
        ByVal DestinazioneUso_Cod As Int32,
        ByVal ID_Imp As Int32,
        ByVal Prov As String,
        ByVal Com As String,
        ByVal Sezione As String,
        ByVal Foglio As Int32,
        ByVal Numero As Int32,
        ByVal Subalterno As String,
        ByVal ID_Agenda As Integer,
        ByVal programmazione_cod As Integer,
        ByVal Programmazione_Entita_cod As Integer,
        ByVal Ricetta_Operazione_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal Stb As System.Text.StringBuilder,
        ByVal isXml As Boolean,
        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
        ByVal CalcolaAreaPerimetro As Boolean,
        ByVal Codice_Fiscale_Tecnico As String,
        ByVal wktBoundaySTIntersects As String,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal layerElementiGrafici_cod As List(Of Integer),
        ByVal impostazioniUtente As Impostazioni_Utente,
        Optional ByVal ElencoLayerVisibili As LayerVisibiliUtenteTipologia = Nothing
        ) As StringBuilder

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiElencoEntita_GetQuery("
        Dim MessaggioErrore As String = ""

        'Decido se applicare la visibilità
        'Esistono record in tabella Utenti_Visibilita_Appoggio?
        'E se non sono in contesto sementi, dove scattano filtro per codice_fiscale_tecnico?
        Dim UtentiVisibilitaAppoggioApplica As Boolean = False
        If Sementieri_Sportello_Configurazione_cod = -1 Then
            UtentiVisibilitaAppoggioApplica = Internal_SQL_DecidiFiltro_Utenti_Visibilita_Appoggio(objParametri_Server)
        End If

        'Chiamata per elenco tipologie (forse obsoleta)
        Dim chiamataXAggiornaElencoTipologie As Boolean = False
        If wktBoundaySTIntersects = "chiamataXAggiornaElencoTipologie" Then
            chiamataXAggiornaElencoTipologie = True
            wktBoundaySTIntersects = ""
        End If

        'VAnni: 6/4/2018: se si richiede il filtro su una particella allora non eseguo il join
        Dim includiJoinTabelleCatasto As Boolean = True
        If Numero > 0 Then
            includiJoinTabelleCatasto = False
        End If

        'Verifica se versione SQL compatibile
        Dim xLeggiVersioneSql As New DataProvider
        Dim majorSql = xLeggiVersioneSql.VersioneSqlServer_Major(objParametri_Server)
        Dim versioneSqlCompatibile As Boolean = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            versioneSqlCompatibile = True
        End If

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Clear()

            InizializzaStringBuilder(StrSQL, objParametri_Server)

            SQL_CTE_ElencoEntita(
                Piva,
                Sa_Cod,
                DestinazioneUso_Cod,
                UtentiVisibilitaAppoggioApplica,
                wktBoundaySTIntersects,
                cfgAlbero,
                layerElementiGrafici_cod,
                includiJoinTabelleCatasto,
                objParametri_Server,
                StrSQL,
                impostazioneUtente:=impostazioniUtente,
                ElencoLayerVisibili:=ElencoLayerVisibili)

            StrSQL.AppendLine()

            If Not chiamataXAggiornaElencoTipologie Then

                LeggiElencoEntita_GetQuery_PredisponiCTE(
                    StrSQL,
                    cfgAlbero,
                    versioneSqlCompatibile,
                    Sementieri_Sportello_Configurazione_cod,
                    objParametri_Server,
                    ElencoLayerVisibili)

                StrSQL.AppendLine()

            End If

            'Query di selezione

            LeggiElencoEntita_GetQuery_Select(
                StrSQL,
                chiamataXAggiornaElencoTipologie,
                cfgAlbero,
                versioneSqlCompatibile,
                Sementieri_Sportello_Configurazione_cod,
                CalcolaAreaPerimetro,
                Codice_Fiscale_Tecnico,
                PivaSuperUser,
                wktBoundaySTIntersects,
                impostazioniUtente,
                objParametri_Server,
                objParametri_Utenti)

            'Condizioni where

            Internal_SQL_CondizioniWhere(
                PivaSuperUser,
                Entita_Cod,
                TipoEntita_Cod,
                PivaPadre,
                Piva,
                Sa_Cod,
                Appezza,
                Campo_Cod,
                Veg_Cod,
                DestinazioneUso_Cod,
                ID_Imp,
                Prov,
                Com,
                Sezione,
                Foglio,
                Numero,
                Subalterno,
                ID_Agenda,
                programmazione_cod,
                Programmazione_Entita_cod,
                Ricetta_Operazione_Cod,
                xFiltroAggiuntivo,
                objParametri_Server,
                StrSQL,
                Sementieri_Sportello_Configurazione_cod,
                Codice_Fiscale_Tecnico,
                wktBoundaySTIntersects,
                False)

            AggiungiFiltriFinaliLayer(cfgAlbero, StrSQL)

            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" And Entita.Inviato >=0 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" And Entita.Inviato =-1 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            'NB: l'order by non è significativa per il caricamento del geojson, dato che viene fatta una
            '    elaborazione parallela per il caricamento del relativo oggetto

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            StrSQL = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return StrSQL

    End Function

    Private Sub LeggiElencoEntita_GetQuery_Select(
        ByRef StrSQL As StringBuilder,
        ByVal chiamataXAggiornaElencoTipologie As Boolean,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal versioneSqlCompatibile As Boolean,
        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
        ByVal CalcolaAreaPerimetro As Boolean,
        ByVal Codice_Fiscale_Tecnico As String,
        ByVal PivaSuperUser As String,
        ByVal wktBoundaySTIntersects As String,
        ByVal impostazioniUtente As Impostazioni_Utente,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri)

        StrSQL.AppendLine("select ")
        StrSQL.AppendLine("    --<Entita/@Attributi> ")
        StrSQL.AppendLine("      COALESCE(Entita.Testo, isNull( geodata.ElementoGrafico_Des, '' )) as '@text'  ")

        If Not chiamataXAggiornaElencoTipologie Then

            If cfgAlbero IsNot Nothing Then

                Select Case cfgAlbero.TipologiaLayer_Cod

                    Case enum_TipologiaLayer.RilieviVegetoProduttivi, enum_TipologiaLayer.Avversita
                        If versioneSqlCompatibile Then
                            StrSQL.AppendLine("     , isNull( t_o.info_estese, isNull(t_ag.info_estese, '') ) as '@info_estese' ")
                        Else
                            StrSQL.AppendLine("     , isNull( txt.testo, '' ) as '@info_estese'  ")
                        End If

                    Case enum_TipologiaLayer.Cultivar
                        Dim dataRaccolta = My.Resources.AgronicaCoreGisDAL.DataRaccolta
                        StrSQL.AppendLine("      , isNull( '" & dataRaccolta & "§ ' + convert(varchar(50), tr.DataRaccolta, 103 ) , '' ) as '@info_estese'  ")

                    Case enum_TipologiaLayer.Fenologia
                        StrSQL.AppendLine("     , isNull( txt.testo, '' ) as '@info_estese'  ")

                    Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                        StrSQL.AppendLine("      , isNull( op.InfoEstese, '' ) as '@info_estese'  ")

                    Case Else
                        StrSQL.AppendLine("  , '' as '@info_estese' ")

                End Select

            Else

                StrSQL.AppendLine("     , isNull( txt.testo, '' ) as '@info_estese'  ")

            End If

            StrSQL.AppendLine("  , '' as '@TipoIcona' ")

        End If

        StrSQL.AppendLine("  , Entita.PivaSuperUser ")
        StrSQL.AppendLine("  , Entita.Entita_Cod ")
        StrSQL.AppendLine("  , ISNULL(Entita.Entita_GUID, '') As Entita_GUID ")
        StrSQL.AppendLine("  , ISNULL(Entita.ParametriVisualizzazioneLayer, '') As ParametriVisualizzazioneLayer ")
        StrSQL.AppendLine("  , geodata.flag_gps ")
        StrSQL.AppendLine("  , geodata.LayerElementiGrafici_Cod as StandardEntita_layerDiAppartenenza  ")
        StrSQL.AppendLine("  , Entita.ETICHETTA " & vbCrLf)
        StrSQL.AppendLine("  , Entita.TipoEntita_Cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Piva " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Sa_Cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Appezza " & vbCrLf)
        StrSQL.AppendLine("  , coalesce(app1.campo_cod, Entita.Campo_Cod) as Campo_Cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Id_Imp " & vbCrLf)
        StrSQL.AppendLine("  , Entita.PROV " & vbCrLf)
        StrSQL.AppendLine("  , Entita.COM " & vbCrLf)
        StrSQL.AppendLine("  , Entita.SEZIONE " & vbCrLf)
        StrSQL.AppendLine("  , Entita.FOGLIO " & vbCrLf)
        StrSQL.AppendLine("  , Entita.NUMERO " & vbCrLf)
        StrSQL.AppendLine("  , Entita.SUBALTERNO " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Programmazione_Entita_Cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.ID_Agenda " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Ricetta_Operazione_Cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.analisi_testata_cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.analisi_campione_cod " & vbCrLf)
        StrSQL.AppendLine("  , Entita.OLDGrafica_ID " & vbCrLf)
        StrSQL.AppendLine("  , Entita.inviato " & vbCrLf)
        StrSQL.AppendLine("  , Entita.datainvio " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Data_Creazione " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Data_Modifica " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Username_Creazione " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Username_Modifica " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Validita_Inizio " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Validita_Fine " & vbCrLf)
        StrSQL.AppendLine("  , Entita.Area_Cod ")
        StrSQL.AppendLine("  , Entita.Fabbricato_Cod ")
        StrSQL.AppendLine("  , coalesce( ent.Programmazione_Cod, Entita.Programmazione_cod) as Programmazione_Cod " & vbCrLf)

        'Se mi trovo nel caso "Sementieri"
        If Sementieri_Sportello_Configurazione_cod <> -1 Then
            StrSQL.AppendLine("  , 0 as Ricetta_Cod ")
        Else
            StrSQL.AppendLine("  , rOperaz.Ricetta_Cod ")
        End If

        StrSQL.AppendLine("  , tEntita.TipoNodoAlberoAnagrafe  ")
        StrSQL.AppendLine("  , coalesce(Entita.part_cod, -1) as part_cod ")
        StrSQL.AppendLine("  , coalesce(coalesce( reg1.codice_fiscale_tecnico, ent.Codice_Fiscale_tecnico),'') as codice_fiscale_tecnico ")

        'Se mi trovo nel caso "Sementieri"
        If Sementieri_Sportello_Configurazione_cod <> -1 Then
            StrSQL.AppendLine("  , filtroSementi.InOsservazione ")
            StrSQL.AppendLine("  , filtroSementi.Colore_Primario ")
            StrSQL.AppendLine("  , filtroSementi.Colore_Retinatura ")
            StrSQL.AppendLine("  , filtroSementi.Trasparenza ")
        Else
            StrSQL.AppendLine("  , 0 AS InOsservazione ")
            StrSQL.AppendLine("  , '' AS Colore_Primario ")
            StrSQL.AppendLine("  , '' AS Colore_Retinatura ")
            StrSQL.AppendLine("  , 0 AS Trasparenza ")
        End If

        If Not chiamataXAggiornaElencoTipologie Then

            'Dettaglio
            StrSQL.AppendLine("  , dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'AppldRate','|','§') as AppIdRate ") ' the '§' logic will be maintained
            StrSQL.AppendLine("  , dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'Speed','|','§') as Speed ")
            StrSQL.AppendLine("  , dbo.fGetKeyValue(geodata.ElementoGrafico_DES,'Height','|','§') as Height ")

            'Dati anagrafici

            StrSQL.AppendLine("  , case when cul_reale.veg_cod is null then -1 else cul_reale.veg_cod end as veg_cod_impianto")

            'Planning
            StrSQL.AppendLine("  , case when cul_planning.veg_cod is null then -1 else cul_planning.veg_cod end as veg_cod_programmazione_entita ")

            'Autorizzazioni

            'TODO: lettura dei permessi nel caso di non sementieri

            If Sementieri_Sportello_Configurazione_cod = -1 Then
                'Per gestione Sementieri -> Mappatura Libera
                StrSQL.AppendLine("  ,'True' as inserimento " & vbCrLf)
                StrSQL.AppendLine("  , CASE WHEN cul_reale.veg_cod IS NOT NULL AND EXISTS (SELECT 1 FROM mappatura_specie WHERE veg_cod = cul_reale.veg_cod AND grfi_cod = reg1.GRFI_COD) THEN 'False' ELSE 'True' END as modifica ")
                StrSQL.AppendLine("  , CASE WHEN cul_reale.veg_cod IS NOT NULL AND EXISTS (SELECT 1 FROM mappatura_specie WHERE veg_cod = cul_reale.veg_cod AND grfi_cod = reg1.GRFI_COD) THEN 'False' ELSE 'True' END as cancellazione ")
                StrSQL.AppendLine("  , 'True' as informazioni " & vbCrLf)
            Else
                StrSQL.AppendLine("  , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as inserimento ")
                StrSQL.AppendLine("  , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as modifica ")
                StrSQL.AppendLine("  , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as cancellazione ")
                StrSQL.AppendLine("  , case when coalesce(filtroSementi.PermessoModifiche, 'False') = 'False' then 'False' else case when '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "' then 'False' else case when cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico) as varchar(1000)) = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' then 'True' else 'False' end end end as informazioni ")
            End If

            'Geodata
            If Not cfgAlbero Is Nothing AndAlso (
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Or
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

                'StrSQL.AppendLine("    , geodata1.Poligono_GeoEntity.STAsText() as geodata ")
                '01/12/2023, vanni, lettura Poligono_GeoEntity_WKT
                StrSQL.AppendLine("    , COALESCE(geodata1.Poligono_GeoEntity_WKT, geodata1.Poligono_GeoEntity.STAsText()) as geodata ")

            Else

                Dim geometria_avversita As String = String.Empty

                If Not cfgAlbero Is Nothing Then
                    'where
                    'If impostazioniUtente.UsaPuntoInterno Then
                    'puntoPoligono = "geometry::STGeomFromText(geodata.Poligono_GeoEntity.STAsText(), 0).STPointOnSurface().STAsText()"

                    If impostazioniUtente.PuntoPoligono Then
                        'geometria_avversita = "geometry::STGeomFromText(geodata.Poligono_GeoEntity.STAsText(), 0).STPointOnSurface().STAsText()"
                        '01/12/2023, vanni, lettura Poligono_GeoEntity_WKT
                        geometria_avversita = "geometry::STGeomFromText(COALESCE(geodata.Poligono_GeoEntity_WKT, geodata.Poligono_GeoEntity.STAsText()), 0).STPointOnSurface().STAsText()"
                    Else
                        '01/12/2023, vanni, lettura Poligono_GeoEntity_WKT
                        geometria_avversita = "COALESCE(geodata.Poligono_GeoEntity_WKT, geodata.Poligono_GeoEntity.STAsText())"

                    End If
                End If

                StrSQL.AppendLine(
                    String.Format("    , case when Entita.layerElementiGrafici_cod_Riproiettato = {0} and Entita.lav_cod in ({1},{2}) then {3} else {4} end as geodata ",
                                  CInt(enum_GIS2012_TipoEntita.Destinazione_Agenda),
                                  CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                                  CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE,
                                  geometria_avversita,
                                  "COALESCE(geodata.Poligono_GeoEntity_WKT, geodata.Poligono_GeoEntity.STAsText())"))
            End If

            'Dati calcolati
            If CalcolaAreaPerimetro Then
                StrSQL.AppendLine("  , round(geodata1.Poligono_GeoEntity.STArea(), 5) as geodata_Area ")
                StrSQL.AppendLine("  , round(geodata1.Poligono_GeoEntity.STLength(), 5) as geodata_Perimetro  ")
                '01/12/2023, vanni, lettura Poligono_GeoEntity_WKT
                StrSQL.AppendLine("  , geometry::STGeomFromText(COALESCE(geodata1.Poligono_GeoEntity_WKT, geodata1.Poligono_GeoEntity.STAsText()), 0).MakeValid().STCentroid().STAsText() as geodata_Baricentro ")
            Else
                StrSQL.AppendLine("  , 0 as geodata_Area ")
                StrSQL.AppendLine("  , 0 as geodata_Perimetro ")
                StrSQL.AppendLine("  , null as geodata_Baricentro ")       'ATTENZIONE DA VERIFICARE SE VA BENE
            End If

            'Sezione colonne layer
            LeggiElencoEntita_GetQuery_SelectLayer(
                StrSQL,
                cfgAlbero,
                Sementieri_Sportello_Configurazione_cod)

            'Sezione from cte_entita + join

            LeggiElencoEntita_GetQuery_From_CTE_Entita(
                StrSQL,
                cfgAlbero,
                versioneSqlCompatibile,
                Sementieri_Sportello_Configurazione_cod,
                wktBoundaySTIntersects,
                objParametri_Server,
                objParametri_Utenti)

        End If

        'lavez - 23/11/2023 - disattivato non sono sicuro serva
        'StrSQL.AppendLine("    LEFT JOIN Agenda a1 ")
        'StrSQL.AppendLine("        On a1.piva = entita.piva ")
        'StrSQL.AppendLine("        And a1.id_agenda = entita.id_agenda ")
        'StrSQL.AppendLine("    LEFT JOIN Analisi_campioni Camp1 ")
        'StrSQL.AppendLine("        On camp1.Analisi_SuperUser = entita.PivaSuperUser ")
        'StrSQL.AppendLine("        And camp1.analisi_campione_Cod = entita.analisi_campione_Cod ")

        If Not chiamataXAggiornaElencoTipologie AndAlso cfgAlbero IsNot Nothing Then

            Select Case cfgAlbero.TipologiaLayer_Cod

                Case enum_TipologiaLayer.Organizzazione
                    Dim coalesce As String

                    Dim connBuilder = New Common.DbConnectionStringBuilder
                    connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione

                    'lavez - 01/07/2024 - Eliminata Gerarchia imprese. Nei casi in cui un'azienda abbia più padri la join moltiplica i record
                    'If Sementieri_Sportello_Configurazione_cod = "-1" Then
                    '    coalesce = "coalesce(reg1.codice_Fiscale_tecnico, coalesce(ent.codice_fiscale_tecnico, E1.padre))"
                    'Else
                    coalesce = "coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico)"
                    'End If

                    StrSQL.AppendLine(String.Format("    LEFT JOIN {0}.dbo.Gruppi_Utente GU
	                                                         ON GU.Gruppi_Utente_Identificativo = cast( {1} as varchar(1000)) ", connBuilder("Initial Catalog"), coalesce))

                Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                    StrSQL.AppendLine("		left join  @TableOperazioni op ")
                    StrSQL.AppendLine("			on op.piva = Entita.piva ")
                    StrSQL.AppendLine("			And op.sa_Cod = Entita.sa_Cod ")
                    StrSQL.AppendLine("			And op.appezza = Entita.appezza ")
                    StrSQL.AppendLine("			And op.id_reg = Entita.id_imp ")
                    StrSQL.AppendLine("			And ( (op.id_mov_det > 0 And entita.id_mov_det = op.Id_Mov_Det ) Or (op.Id_Mov_Det = -1 and entita.tipoEntita_Cod in (select * from cte_GIS_TipoEntita ) )) ")

                Case enum_TipologiaLayer.Cultivar
                    StrSQL.AppendLine("		left join  @TableRaccolta tr ")
                    StrSQL.AppendLine("		    on tr.piva = Entita.Piva ")
                    StrSQL.AppendLine("		    And tr.sa_cod = Entita.Sa_Cod ")
                    StrSQL.AppendLine("		    And tr.appezza = Entita.Appezza ")
                    StrSQL.AppendLine("		    And tr.id_imp = Entita.Id_Imp ")

                Case enum_TipologiaLayer.Avversita
                    If versioneSqlCompatibile Then
                        StrSQL.AppendLine("    LEFT JOIN @TableOp t_o on t_o.ricetta_operazione_cod > 0 and t_o.ricetta_operazione_cod = entita.Ricetta_Operazione_cod ")
                        StrSQL.AppendLine("    LEFT JOIN @TableOp t_ag on t_ag.ricetta_operazione_cod = 0 ")
                        StrSQL.AppendLine("                           and ( ( t_ag.id_agenda > 0 And t_ag.Id_Agenda = entita.id_agenda) Or (t_ag.Id_Agenda <= 0 And entita.tipoEntita_cod In (Select TipoEntita_Cod from cte_GIS_TipoEntita) ) ) ")
                        StrSQL.AppendLine("                           and t_ag.piva = entita.piva ")
                        StrSQL.AppendLine("                           and t_ag.sa_Cod = entita.sa_Cod ")
                        StrSQL.AppendLine("                           and t_ag.appezza = entita.appezza ")
                        StrSQL.AppendLine("                           and t_ag.id_reg = entita.id_imp ")
                        StrSQL.AppendLine("                           and t_ag.av_cod = l7.av_cod ")
                    End If

                Case enum_TipologiaLayer.RilieviVegetoProduttivi
                    If versioneSqlCompatibile Then
                        StrSQL.AppendLine("    LEFT JOIN @TableOp t_o on t_o.ricetta_operazione_cod > 0 and t_o.ricetta_operazione_cod = entita.Ricetta_Operazione_cod ")
                        StrSQL.AppendLine("    LEFT JOIN @TableOp t_ag on t_Ag.ricetta_operazione_cod = 0 ")
                        StrSQL.AppendLine("                           and ( ( t_ag.id_agenda > 0 And t_ag.Id_Agenda = entita.id_agenda) Or (t_ag.Id_Agenda <= 0 And entita.tipoEntita_cod In (Select TipoEntita_Cod from cte_GIS_TipoEntita) ) ) ")
                        StrSQL.AppendLine("                           and t_ag.piva = entita.piva ")
                        StrSQL.AppendLine("                           and t_ag.sa_Cod = entita.sa_Cod ")
                        StrSQL.AppendLine("                           and t_ag.appezza = entita.appezza ")
                        StrSQL.AppendLine("                           and t_ag.id_reg = entita.id_imp ")
                        StrSQL.AppendLine("                           and t_ag.av_cod = l9.ff_classe ")
                    End If

                    'Case enum_TipologiaLayer.Avversita, enum_TipologiaLayer.RilieviVegetoProduttivi
                    'If test_Ottimizzazione_2 Then
                    '    StrSQL.AppendLine("		LEFT JOIN @TableOp t_o on t_o.ricetta_operazione_cod > 0 and t_o.ricetta_operazione_cod = entita.Ricetta_Operazione_cod ")
                    '    StrSQL.AppendLine("		LEFT JOIN ( ")
                    '    StrSQL.AppendLine("		    Select ")
                    '    StrSQL.AppendLine("		        t.* ")
                    '    StrSQL.AppendLine("		    from ( ")
                    '    StrSQL.AppendLine("		      select piva, Sa_Cod, Appezza, id_reg, MAX(DataOperazione) as DataOperazione ")
                    '    StrSQL.AppendLine("		         from @TableOp op1 ")
                    '    Dim datiFiltroTemporale = GetDatiFiltroTemporale(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, objParametri_Server)
                    '    StrSQL.AppendLine("		         where op1.DataOperazione>= " & Agro_SQL_SaveDate(datiFiltroTemporale.finestraTemporaleInizio))
                    '    StrSQL.AppendLine("		         And op1.DataOperazione <= " & Agro_SQL_SaveDate(datiFiltroTemporale.finestraTemporaleFine))
                    '    StrSQL.AppendLine("		      group by piva, Sa_Cod, Appezza, id_reg ")
                    '    StrSQL.AppendLine("		  ) a  ")
                    '    StrSQL.AppendLine("		     inner Join @TableOp t ")
                    '    StrSQL.AppendLine("		         On t.piva = a.piva ")
                    '    StrSQL.AppendLine("		      And t.sa_cod = a.sa_cod ")
                    '    StrSQL.AppendLine("		      And t.appezza = a.appezza ")
                    '    StrSQL.AppendLine("		      And t.id_reg = a.id_reg ")
                    '    StrSQL.AppendLine("		      And t.DataOperazione = a.DataOperazione ")
                    '    StrSQL.AppendLine("		) t_Ag On t_Ag.ricetta_operazione_cod = 0  ")
                    '    StrSQL.AppendLine("		    And ( ( t_ag.id_agenda > 0 And t_ag.Id_Agenda = entita.id_agenda) Or (t_ag.Id_Agenda = -1 And entita.tipoEntita_cod In (Select TipoEntita_Cod from cte_GIS_TipoEntita) ) ) ")
                    '    StrSQL.AppendLine("		    And t_ag.piva = entita.piva ")
                    '    StrSQL.AppendLine("		    And t_ag.sa_Cod = entita.sa_Cod ")
                    '    StrSQL.AppendLine("		    And t_ag.appezza = entita.appezza  ")
                    '    StrSQL.AppendLine("		    And t_ag.id_reg = entita.id_imp ")
                    'End If

            End Select

        End If

    End Sub

    Private Sub LeggiElencoEntita_GetQuery_From_CTE_Entita(
        ByRef StrSQL As StringBuilder,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal versioneSqlCompatibile As Boolean,
        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
        ByVal wktBoundaySTIntersects As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri)

        StrSQL.AppendLine()
        StrSQL.AppendLine(" FROM ")
        'StrSQL.AppendLine("     cte_entita Entita ")
        'StrSQL.AppendLine("     @TEMPENTITA Entita ")
        StrSQL.AppendLine("     #TEMPENTITA Entita ")
        StrSQL.AppendLine("     INNER JOIN GIS_TipoEntita tEntita ON tEntita.TipoEntita_cod = Entita.TipoEntita_cod ")
        StrSQL.AppendLine("     INNER JOIN ( ")
        StrSQL.AppendLine("         SELECT ")
        StrSQL.AppendLine("             entita_cod ")
        StrSQL.AppendLine("             , LayerElementiGrafici_Cod ")
        StrSQL.AppendLine("             , ElementoGrafico_Des ")
        StrSQL.AppendLine("             , Flag_GPS ")
        StrSQL.AppendLine("             , Poligono_GeoEntity ")
        StrSQL.AppendLine("             , Poligono_GeoEntity_WKT ")
        StrSQL.AppendLine("     FROM GIS_ElementiGrafici ")
        ''lavez - 17/11/2023
        'If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
        '    StrSQL.AppendLine("    where Poligono_GeoEntity.STIntersects( ")
        '    StrSQL.AppendLine("     geography::STGeomFromText( ")
        '    StrSQL.AppendLine("      '" & wktBoundaySTIntersects & "' ")
        '    StrSQL.AppendLine("      , 4326)          ")
        '    StrSQL.AppendLine("  ) = 1")

        'End If

        StrSQL.AppendLine(" ) geodata ON Entita.Entita_Cod = geodata.Entita_Cod ")

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

            StrSQL.AppendLine("     INNER JOIN cte_geodata geodata1 ")
            StrSQL.AppendLine("     on geodata1.Entita_Cod= geodata.Entita_Cod ")

        End If

        StrSQL.AppendLine("     INNER JOIN gis_layerElementiGrafici layerPrincipale ")
        StrSQL.AppendLine("         ON layerPrincipale.LayerElementiGrafici_Cod =   case when Entita.TipoEntita_Cod = 63 then 63 else  geodata.LayerElementiGrafici_Cod  end --geodata.LayerElementiGrafici_Cod  ")
        StrSQL.AppendLine("         And layerPrincipale.TipologiaLayer_cod = 1 ")
        StrSQL.AppendLine("         And layerPrincipale.flag_visibile = 1 ")
        StrSQL.AppendLine("         And layerPrincipale.Utente = '" & Agro_SQL_SaveText(objParametri_Server.UtenteUsername) & "' ")
        StrSQL.AppendLine("     left join cte_GIS_TipoEntita tp ")
        StrSQL.AppendLine("         on tp.TipoEntita_Cod = Entita.TipoEntita_Cod ")
        StrSQL.AppendLine("     Left Join 	GIS_LayerElementiGrafici l1 ")
        StrSQL.AppendLine("         On l1.Utente = '" & Agro_SQL_SaveText(objParametri_Server.UtenteUsername) & "' ")
        StrSQL.AppendLine("         And l1.flag_visibile <> 0 ")
        StrSQL.AppendLine("         And l1.flag_attivo <> 0 ")
        StrSQL.AppendLine("         And l1.TipologiaLayer_cod = 1 ")
        StrSQL.AppendLine("         And l1.LayerElementiGrafici_Cod = geodata.LayerElementiGrafici_Cod ")

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.SpecieVegetale) Then

            StrSQL.AppendLine("     Left Join cte_layer_specie_vegetale l5 ")
            StrSQL.AppendLine("         On l5.PIVA = Entita.Piva ")
            StrSQL.AppendLine("         And l5.SA_COD = Entita.Sa_Cod ")
            StrSQL.AppendLine("         And l5.APPEZZA = Entita.Appezza ")
            StrSQL.AppendLine("         And l5.ID_REG = entita.id_imp ")
            StrSQL.AppendLine("         And l5.Programmazione_Entita_Cod = Entita.Programmazione_Entita_COD ")

        End If

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar) Then

            StrSQL.AppendLine("     Left Join cte_layer_varieta l15 ")
            StrSQL.AppendLine("         On l15.PIVA = Entita.Piva ")
            StrSQL.AppendLine("         And l15.SA_COD = Entita.Sa_Cod ")
            StrSQL.AppendLine("         And l15.APPEZZA = Entita.Appezza ")
            StrSQL.AppendLine("         And l15.ID_REG = entita.id_imp ")
            StrSQL.AppendLine("         And l15.Programmazione_Entita_Cod = Entita.Programmazione_Entita_COD ")

        End If

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Fenologia) Then

            StrSQL.AppendLine("     Left Join cte_layer_fenologia l8 ")
            StrSQL.AppendLine("         On l8.ricetta_operazione_cod = Entita.Ricetta_Operazione_cod ")
            StrSQL.AppendLine("         And l8.id_Agenda = Entita.ID_Agenda ")

        End If

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Avversita) Then

            StrSQL.AppendLine("     Left Join cte_layer_avversita l7 On l7.Ricetta_Operazione_Cod = Entita.Ricetta_Operazione_cod ")
            StrSQL.AppendLine("                                     And l7.id_Agenda = Entita.ID_Agenda ")
            StrSQL.AppendLine("                                     And l7.piva = Entita.Piva ")
            StrSQL.AppendLine("                                     And l7.sa_Cod = Entita.Sa_Cod ")
            StrSQL.AppendLine("                                     And l7.appezza = Entita.appezza ")
            StrSQL.AppendLine("                                     And l7.id_reg = Entita.Id_Imp ")
            StrSQL.AppendLine("                                     And l7.av_cod = Entita.Rilievo_Cod ")
            StrSQL.AppendLine("                                     And (l7.id_mov_det = Entita.id_mov_det or Entita.id_mov_det = 0) ")

        End If

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

            StrSQL.AppendLine("     Left Join cte_layer_rilievivegetoproduttivi l9 ")
            StrSQL.AppendLine("         On l9.Id_Agenda = Entita.ID_Agenda ")
            StrSQL.AppendLine("        And l9.ff_classe = Entita.Rilievo_Cod ")

        End If

        'vanni, 31/10/2023 ... obsoleto, è stato spostato su insert dei dati
        'If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
        '    StrSQL.AppendLine("     INNER JOIN GIS_ElementiGrafici GeoDataIntersezione ")
        '    StrSQL.AppendLine("         ON GeoDataIntersezione.entita_cod = Entita.entita_cod ")
        'End If

        If Not cfgAlbero Is Nothing Then

            Select Case cfgAlbero.TipologiaLayer_Cod

                Case enum_TipologiaLayer.RilieviVegetoProduttivi, enum_TipologiaLayer.Avversita

                    If Not versioneSqlCompatibile Then
                        StrSQL.AppendLine("     Left Join cte_text txt ")
                        StrSQL.AppendLine("         On txt.id_agenda = entita.id_agenda ")
                        StrSQL.AppendLine("         And txt.piva = entita.piva ")
                        StrSQL.AppendLine("         And txt.sa_Cod = entita.sa_Cod ")
                        StrSQL.AppendLine("         And txt.appezza = entita.appezza ")
                        StrSQL.AppendLine("         And txt.id_reg = entita.id_imp ")
                        StrSQL.AppendLine("         And txt.ricetta_operazione_cod= entita.ricetta_operazione_cod ")
                    End If

                Case enum_TipologiaLayer.Fenologia

                    StrSQL.AppendLine("     Left Join cte_text txt ")
                    StrSQL.AppendLine("         On txt.id_agenda = entita.id_agenda ")
                    StrSQL.AppendLine("         And txt.piva = entita.piva ")
                    StrSQL.AppendLine("         And txt.sa_Cod = entita.sa_Cod ")
                    StrSQL.AppendLine("         And txt.appezza = entita.appezza ")
                    StrSQL.AppendLine("         And txt.id_reg = entita.id_imp ")
                    StrSQL.AppendLine("         And txt.ricetta_operazione_cod= entita.ricetta_operazione_cod ")

            End Select

        Else

            StrSQL.AppendLine("     Left Join cte_text txt ")
            StrSQL.AppendLine("         On txt.id_agenda = entita.id_agenda ")
            StrSQL.AppendLine("         And txt.piva = entita.piva ")
            StrSQL.AppendLine("         And txt.sa_Cod = entita.sa_Cod ")
            StrSQL.AppendLine("         And txt.appezza = entita.appezza ")
            StrSQL.AppendLine("         And txt.id_reg = entita.id_imp ")
            StrSQL.AppendLine("         And txt.ricetta_operazione_cod= entita.ricetta_operazione_cod ")

        End If

        StrSQL.AppendLine("     LEFT JOIN reg_impianti reg1 ")
        StrSQL.AppendLine("         On reg1.PIVA = Entita.Piva ")
        StrSQL.AppendLine("         And reg1.SA_COD = Entita.Sa_Cod ")
        StrSQL.AppendLine("         And reg1.APPEZZA = Entita.Appezza ")
        StrSQL.AppendLine("         And reg1.ID_REG = entita.id_imp ")
        StrSQL.AppendLine("    left Join reg_impianti_codici reg_cod on reg_cod.PIVA = Entita.Piva And reg_cod.SA_COD = Entita.Sa_Cod ")
        StrSQL.AppendLine("        AND reg_cod.APPEZZA = Entita.Appezza AND reg_cod.ID_REG = Entita.id_imp and reg_cod.id_cod between 3000 and 4000 ")
        StrSQL.AppendLine("     LEFT JOIN Appezzamento app1 ")
        StrSQL.AppendLine("         On app1.PIVA = Entita.Piva ")
        StrSQL.AppendLine("         And app1.SA_COD = Entita.Sa_Cod ")
        StrSQL.AppendLine("         And app1.APPEZZA = Entita.Appezza ")
        StrSQL.AppendLine("     LEFT JOIN cultivar cul_reale ")
        StrSQL.AppendLine("         On reg1.cul_cod = cul_reale.Cul_Cod ")
        StrSQL.AppendLine("     LEFT JOIN Programmazione_Entita ent ")
        StrSQL.AppendLine("         On ent.programmazione_entita_cod = entita.Programmazione_Entita_COD ")
        StrSQL.AppendLine("     LEFT JOIN Ricette_Operazioni rOperaz ")
        StrSQL.AppendLine("         On rOperaz.Ricetta_Operazione_cod = entita.Ricetta_Operazione_cod ")
        StrSQL.AppendLine("     LEFT JOIN cultivar cul_planning ")
        StrSQL.AppendLine("         On ent.cul_cod = cul_planning.Cul_Cod ")

        If Sementieri_Sportello_Configurazione_cod <> -1 Then

            StrSQL.AppendLine("    INNER JOIN cte_sportello_sementieri filtroSementi")
            StrSQL.AppendLine("        ON filtroSementi.PIVA = reg1.PIVA ")
            StrSQL.AppendLine("        AND filtroSementi.SA_COD = reg1.SA_COD ")
            StrSQL.AppendLine("        AND filtroSementi.APPEZZA = reg1.APPEZZA ")
            StrSQL.AppendLine("        AND filtroSementi.ID_REG = reg1.ID_REG ")

        Else
            'lavez - 01/07/2024 - Eliminata Gerarchia imprese. Nei casi in cui un'azienda abbia più padri la join moltiplica i record
            '                     Allineata a nuova logica sementieri che sfrutta la Gruppi_Utente ma la jaon viene fatta solo ne caso in cui il layer sia quello della organizzazione aziendale
            If cfgAlbero Is Nothing OrElse (cfgAlbero IsNot Nothing AndAlso cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Organizzazione) Then
                Dim coalesce As String

                Dim connBuilder = New Common.DbConnectionStringBuilder
                connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione


                'If Sementieri_Sportello_Configurazione_cod = "-1" Then
                '    coalesce = "coalesce(reg1.codice_Fiscale_tecnico, coalesce(ent.codice_fiscale_tecnico, E1.padre))"
                'Else
                coalesce = "coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico)"

                ' Mengarda 2026-03-02 #207311
                ' This join is already being added in LeggiElencoEntita_GetQuery_Select
'                StrSQL.AppendLine(String.Format("    LEFT JOIN {0}.dbo.Gruppi_Utente GU
'	                                                         ON GU.Gruppi_Utente_Identificativo = cast( {1} as varchar(1000)) ", connBuilder("Initial Catalog"), coalesce))

            End If
        End If

    End Sub

    Private Sub LeggiElencoEntita_GetQuery_SelectLayer(
        ByRef StrSQL As StringBuilder,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal Sementieri_Sportello_Configurazione_cod As String)

        'Layer standard
        StrSQL.AppendLine("   , case when l1.LayerElementiGrafici_Cod Is Not null then ")
        StrSQL.AppendLine("         Case when Entita.layerElementiGrafici_cod_Riproiettato <> -1 then ")
        StrSQL.AppendLine("             cast(Entita.layerElementiGrafici_cod_Riproiettato As varchar(1000)) ")
        StrSQL.AppendLine("         Else ")
        StrSQL.AppendLine("             cast(geodata.layerElementiGrafici_cod As varchar(1000)) ")
        StrSQL.AppendLine("         End ")
        StrSQL.AppendLine("     Else ")
        StrSQL.AppendLine("         null ")
        StrSQL.AppendLine("     End As layer_1 ")

        'Layer organizzazione
        If cfgAlbero IsNot Nothing AndAlso cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Organizzazione Then

            'If Sementieri_Sportello_Configurazione_cod = "-1" Then
            'StrSQL.AppendLine("   , cast(coalesce(reg1.codice_Fiscale_tecnico, coalesce(ent.codice_fiscale_tecnico, E1.padre))  as varchar(1000)) as layer_100 ")
            'Else
            'StrSQL.AppendLine("   , cast(coalesce(reg1.codice_Fiscale_tecnico, ent.codice_fiscale_tecnico)  as varchar(1000)) as layer_100 ")
            'End If
            StrSQL.AppendLine("   , GU.Gruppi_Utente_cod as Layer_100 ")
        End If

        'Layer specie vegetale
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.SpecieVegetale) Then

            StrSQL.Append(" , ")
            EstrazioneSqlLayerSpecieVegetale(StrSQL)
            StrSQL.AppendLine(" as layer_5 ")

        End If

        'Layer cultivar
        If Not cfgAlbero Is Nothing AndAlso (
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
                cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar) Then

            StrSQL.Append(" , ")
            EstrazioneSqlLayerCultivar(StrSQL)
            StrSQL.AppendLine(" as layer_15 ")

        End If

        'Layer fenologia
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Fenologia) Then

            StrSQL.Append(" , ")
            EstrazioneSqlLayerFenologia(StrSQL)
            StrSQL.AppendLine(" as layer_8 ")

        End If

        'Layer avversita
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Avversita) Then

            StrSQL.Append(" , ")
            EstrazioneSqlLayerAvversita(StrSQL)
            StrSQL.AppendLine(" as layer_7 ")

        End If

        'Layer rilievi vegeto produttivi
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

            StrSQL.Append(" , ")
            EstrazioneSqlLayerRilieviVegetoProduttivi(StrSQL)
            StrSQL.AppendLine(" as layer_9 ")

        End If

        'Layer analisi periodicità agenda
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda) Then

            StrSQL.AppendLine("   , case when op.lav_cod Is Not null then cast(op.lav_cod as varchar(1000)) else null end as layer_11 ")

        End If

        'Layer percorsi
        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.NonSpecificato Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Percorsi) Then

            StrSQL.AppendLine("   , case when Entita.TipoEntita_cod = 0 Then ")
            StrSQL.AppendLine("         Case When Entita.piva <> '' then ")
            StrSQL.AppendLine("             cast(85 As varchar(1000)) ")
            StrSQL.AppendLine("         Else ")
            StrSQL.AppendLine("             Case when charindex('isStop§ 1', geodata.ElementoGrafico_DES , 0)>1 then ") ' the '§' logic will be maintained
            StrSQL.AppendLine("                 cast(81 As varchar(1000)) ")
            StrSQL.AppendLine("             Else ")
            StrSQL.AppendLine("                 cast(83 As varchar(1000)) ")
            StrSQL.AppendLine("             End ")
            StrSQL.AppendLine("         End ")
            StrSQL.AppendLine("     Else ")
            StrSQL.AppendLine("         Case when tp.TipoEntita_Cod Is Not null then ")
            StrSQL.AppendLine("             cast(82 As varchar(1000)) ")
            StrSQL.AppendLine("         Else ")
            StrSQL.AppendLine("             Case when entita.TipoEntita_cod = 63 then ")
            StrSQL.AppendLine("                 cast(84 As varchar(1000)) ")
            StrSQL.AppendLine("             Else ")
            StrSQL.AppendLine("                 null ")
            StrSQL.AppendLine("             End ")
            StrSQL.AppendLine("         End ")
            StrSQL.AppendLine("     End As layer_10 ")

        End If

    End Sub

    Private Sub LeggiElencoEntita_GetQuery_PredisponiCTE(
        ByRef StrSQL As StringBuilder,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal versioneSqlCompatibile As Boolean,
        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByVal ElencoLayerVisibili As LayerVisibiliUtenteTipologia)

        If cfgAlbero IsNot Nothing Then

            Select Case cfgAlbero.TipologiaLayer_Cod
                Case enum_TipologiaLayer.RilieviVegetoProduttivi, enum_TipologiaLayer.Avversita
                    If versioneSqlCompatibile Then
                        SQL_InMemory_Table_Op(StrSQL, cfgAlbero.TipologiaLayer_Cod, objParametri_Server)
                    End If

                Case enum_TipologiaLayer.Cultivar
                    SQL_InMemory_Table_Raccolta(StrSQL)

                Case enum_TipologiaLayer.AnalisiPeriodicitaAgenda
                    SQL_InMemory_TableTemp_Operazioni(StrSQL)

            End Select

        End If

        SQL_CTE_GIS_TipoEntita(StrSQL, True, True)

        If cfgAlbero IsNot Nothing Then

            Select Case cfgAlbero.TipologiaLayer_Cod

                Case enum_TipologiaLayer.NonSpecificato
                    'SQL_CTE_Cultivar(StrSQL, True)
                    SQL_CTE_layer_specie_vegetale(StrSQL, True)
                    SQL_CTE_RicetteAvversita(StrSQL, True)
                    SQL_CTE_MovAgendaXAvversita(StrSQL, True)
                    SQL_CTE_layer_avversita(StrSQL, True)
                    'SQL_CTE_ClassiFenologicheRicette(StrSQL, True)
                    If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                        SQL_CTE_ClassiFenologicheAgenda(StrSQL, True)
                    End If
                    SQL_CTE_layer_fenologia(StrSQL, True)
                    SQL_CTE_AgendaRilieviVegetoProduttivi(StrSQL, True)
                    SQL_CTE_layer_rilievivegetoproduttivi(StrSQL, True)


                Case enum_TipologiaLayer.Std
                    If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                        SQL_CTE_Avversita(StrSQL, True)
                        SQL_CTE_Ricette(StrSQL, True)
                        SQL_CTE_Risk(StrSQL, True)
                        SQL_CTE_MisureXAvversita(StrSQL, True)
                        SQL_CTE_Movimenti(StrSQL, True)
                        SQL_CTE_RicetteConAvversita(StrSQL, True)
                        SQL_CTE_Operazioni(StrSQL, True)

                        SQL_CTE_ClassiFenologicheAgenda(StrSQL, True)
                    End If

                Case enum_TipologiaLayer.Avversita
                    If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                        SQL_CTE_Avversita(StrSQL, True)
                        SQL_CTE_Ricette(StrSQL, True)
                        SQL_CTE_Risk(StrSQL, True)
                        SQL_CTE_MisureXAvversita(StrSQL, True)
                        SQL_CTE_Movimenti(StrSQL, True)
                        SQL_CTE_RicetteConAvversita(StrSQL, True)
                        SQL_CTE_Operazioni(StrSQL, True)
                    End If
                    SQL_CTE_RicetteAvversita(StrSQL, True)
                    SQL_CTE_MovAgendaXAvversita(StrSQL, True)
                    SQL_CTE_layer_avversita(StrSQL, True)
                    'If Not versioneSqlCompatibile Then
                    '    SQL_CTE_Text(StrSQL, True)
                    'End If

                Case enum_TipologiaLayer.SpecieVegetale
                    If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                        SQL_CTE_Avversita(StrSQL, True)
                        SQL_CTE_Ricette(StrSQL, True)
                        SQL_CTE_Risk(StrSQL, True)
                        SQL_CTE_MisureXAvversita(StrSQL, True)
                        SQL_CTE_Movimenti(StrSQL, True)
                        SQL_CTE_RicetteConAvversita(StrSQL, True)
                        SQL_CTE_Operazioni(StrSQL, True)
                    End If
                    SQL_CTE_Cultivar(StrSQL, True)
                    SQL_CTE_layer_specie_vegetale(StrSQL, True)

                Case enum_TipologiaLayer.Cultivar
                    If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                        SQL_CTE_Avversita(StrSQL, True)
                        SQL_CTE_Ricette(StrSQL, True)
                        SQL_CTE_Risk(StrSQL, True)
                        SQL_CTE_MisureXAvversita(StrSQL, True)
                        SQL_CTE_Movimenti(StrSQL, True)
                        SQL_CTE_RicetteConAvversita(StrSQL, True)
                        SQL_CTE_Operazioni(StrSQL, True)
                    End If
                    SQL_CTE_Cultivar(StrSQL, True)
                    SQL_CTE_layer_varieta(StrSQL, True)

                Case enum_TipologiaLayer.Fenologia
                    SQL_CTE_ClassiFenologicheAgenda(StrSQL, True)
                    SQL_CTE_layer_fenologia(StrSQL, True)
                    SQL_CTE_Avversita(StrSQL, True)
                    SQL_CTE_Ricette(StrSQL, True)
                    SQL_CTE_Risk(StrSQL, True)
                    SQL_CTE_MisureXAvversita(StrSQL, True)
                    SQL_CTE_Movimenti(StrSQL, True)
                    SQL_CTE_RicetteConAvversita(StrSQL, True)
                    SQL_CTE_Operazioni(StrSQL, True)
                    SQL_CTE_Text(StrSQL, True)

                Case enum_TipologiaLayer.RilieviVegetoProduttivi
                    SQL_CTE_AgendaRilieviVegetoProduttivi(StrSQL, True)
                    SQL_CTE_layer_rilievivegetoproduttivi(StrSQL, True)
                    'If Not versioneSqlCompatibile Then
                    '    SQL_CTE_Text(StrSQL, True)
                    'End If

            End Select
            'Else

            '    SQL_CTE_Text(StrSQL, True)

        End If

        If cfgAlbero IsNot Nothing AndAlso {enum_TipologiaLayer.Avversita, enum_TipologiaLayer.RilieviVegetoProduttivi}.Contains(cfgAlbero.TipologiaLayer_Cod) Then
            If Not versioneSqlCompatibile Then
                SQL_CTE_Text(StrSQL, True)
            End If
        Else
            If ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = CInt(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI)).Count > 0 Then
                SQL_CTE_Text(StrSQL, True)
            End If
        End If

        If Sementieri_Sportello_Configurazione_cod <> -1 Then
            SQL_CTE_SportelloSementieri(StrSQL, Sementieri_Sportello_Configurazione_cod, False)
        End If

        If cfgAlbero IsNot Nothing AndAlso
            (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.AnalisiPeriodicitaAgenda Or
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then
            SQL_CTE_Geodata(StrSQL)
        End If

        If StrSQL.ToString().EndsWith(", " & vbCrLf) Then
            StrSQL.Length -= 4
            StrSQL.Append("  " & vbCrLf)
        End If

    End Sub

    Private Sub AggiungiFiltriFinaliLayer(ByVal cfgAlbero As ConfigurazioneAlbero,
                                          ByRef StrSQL As StringBuilder)

        If Not cfgAlbero Is Nothing AndAlso (
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.SpecieVegetale OrElse
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar OrElse
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Fenologia OrElse
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Avversita OrElse
            cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi) Then

            StrSQL.AppendLine(" and ")

            Select Case cfgAlbero.TipologiaLayer_Cod

                Case enum_TipologiaLayer.SpecieVegetale
                    EstrazioneSqlLayerSpecieVegetale(StrSQL)

                Case enum_TipologiaLayer.Cultivar
                    EstrazioneSqlLayerCultivar(StrSQL)

                Case enum_TipologiaLayer.Fenologia
                    EstrazioneSqlLayerFenologia(StrSQL)

                Case enum_TipologiaLayer.Avversita
                    EstrazioneSqlLayerAvversita(StrSQL)

                Case enum_TipologiaLayer.RilieviVegetoProduttivi
                    EstrazioneSqlLayerRilieviVegetoProduttivi(StrSQL)

            End Select

            StrSQL.AppendLine(" is not null ")

        End If

    End Sub

    Private Sub EstrazioneSqlLayerSpecieVegetale(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine(" case when l5.specie_vegetale Is null then ")
        StrSQL.AppendLine("      case when Entita.TipoEntita_Cod in (3,5) then '-1' ")
        StrSQL.AppendLine("           else null ")
        StrSQL.AppendLine("      end ")
        StrSQL.AppendLine(" else l5.specie_vegetale ")
        StrSQL.Append(" end ")
    End Sub

    Private Sub EstrazioneSqlLayerCultivar(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine(" case when l15.Varieta Is null then ")
        StrSQL.AppendLine("      case when Entita.TipoEntita_Cod in (3,5) then '-1' ")
        StrSQL.AppendLine("           else null ")
        StrSQL.AppendLine("      end ")
        StrSQL.AppendLine("      else l15.Varieta ")
        StrSQL.Append(" end ")
    End Sub

    Private Sub EstrazioneSqlLayerFenologia(ByRef StrSQL As StringBuilder)
        StrSQL.Append(" l8.ff_Cod ")
    End Sub

    Private Sub EstrazioneSqlLayerAvversita(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine(" case when (Entita.ID_Agenda > 0 or (Entita.ID_Agenda < 1 and ")
        StrSQL.AppendLine("                                     tp.TipoEntita_Cod is not null and ")
        StrSQL.AppendLine("                                     Entita.layerElementiGrafici_cod_Riproiettato = 63)) ")
        StrSQL.AppendLine("      then l7.av_cod ")
        StrSQL.AppendLine("      else case when tp.TipoEntita_Cod Is Not null then cast(-2 as varchar(1000)) ")
        StrSQL.AppendLine("                else null ")
        StrSQL.AppendLine("           end ")
        StrSQL.AppendLine(" end ")
    End Sub

    Private Sub EstrazioneSqlLayerRilieviVegetoProduttivi(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine(" case when l9.ff_classe is not null then l9.ff_classe ")
        StrSQL.AppendLine("      else case when tp.TipoEntita_Cod Is Not null then cast(-2 as varchar(1000)) ")
        StrSQL.AppendLine("                else null ")
        StrSQL.AppendLine("           end ")
        StrSQL.AppendLine(" end ")
    End Sub

    Private Sub SQL_CTE_LayerWMS(ByVal Username As String,
                                 ByRef strsql As StringBuilder,
                                 Optional addCommaToEnd As Boolean = False,
                                 Optional addWithToStart As Boolean = False)

        If addWithToStart Then
            strsql.Append("with ")
        End If
        strsql.AppendLine("cte_layer_wms as ( ")
        strsql.AppendLine(" Select * ")
        strsql.AppendLine(" from ")
        strsql.AppendLine("     gis_layerelementigrafici wms ")
        strsql.AppendLine(" where ")
        strsql.AppendLine("     wms.Utente='" & Agro_SQL_SaveText(Username) & "' ")
        strsql.AppendLine("     AND wms.TipologiaLayer_Cod = 1 ")
        strsql.AppendLine("     AND wms.LayerElementiGrafici_Cod<0 ")
        strsql.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))

    End Sub

    Private Sub InizializzaStringBuilder(ByRef StrSQL As StringBuilder,
                                         ByRef objParametri As AgronicaCoreParametri)
        StrSQL.AppendLine("DECLARE @DataI DATE ;")
        StrSQL.AppendLine("DECLARE @DataF DATE ;")
        StrSQL.AppendLine()
        StrSQL.AppendLine("SET @DataI = " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio, False) + ";")
        StrSQL.AppendLine("SET @DataF = " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine, False) + ";")
        StrSQL.AppendLine()
        StrSQL.AppendLine()
    End Sub


    Private Sub SQL_CTE_Avversita(ByRef strsql As StringBuilder,
                                  Optional addCommaToEnd As Boolean = False,
                                  Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            strsql.Append("with ")
        End If
        strsql.AppendLine("cte_avversita as ( ")
        strsql.AppendLine(" Select mav.cod, Avversita.Av_Cod, Avversita.Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_DES, mav.ordine ")
        strsql.AppendLine(" from ")
        strsql.AppendLine(" MisuraxAvversita mav ")
        strsql.AppendLine(" INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod ")
        strsql.AppendLine(" INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod ")
        strsql.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Ricette(ByRef strsql As StringBuilder,
                                Optional addCommaToEnd As Boolean = False,
                                Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            strsql.Append("with ")
        End If
        strsql.AppendLine("cte_ricette as ( ")
        strsql.AppendLine("     Select ")
        strsql.AppendLine("         R.Piva ")
        strsql.AppendLine("	        , R.sa_cod ")
        strsql.AppendLine("	        , R.Ricetta_Cod as id_agenda ")
        strsql.AppendLine("	        , op.Ricetta_Operazione_Cod ")
        strsql.AppendLine("	        , Op.Lav_Cod ")
        strsql.AppendLine("	        , R.inviato   ")
        strsql.AppendLine("	        , tec.Ricette_Dettaglio_Tecnico_GraphicKey as mov_destinazioni_graphickey  ")
        strsql.AppendLine(" 	        , tec.Av_Cod   ")
        strsql.AppendLine("	        , D.Udm_Cod   ")
        strsql.AppendLine("	        , rcul.CUL_COD   ")
        strsql.AppendLine("	        , tec.FF_Classe   ")
        strsql.AppendLine("	        , R.Validita_Inizio as Data_Movimento  ")
        strsql.AppendLine("	        , 0 as Sup_Imp   ")
        strsql.AppendLine("	        , tec.qta_ril as Qta   ")
        strsql.AppendLine("           , '' as Via_Stringa   ")
        strsql.AppendLine("	        , '' as APP_NOME   ")
        strsql.AppendLine("	        , R.Username_Modifica   ")
        strsql.AppendLine("	        , tec.Piezo1   ")
        strsql.AppendLine("	        , tec.Piezo2 as FF_Classe2   ")
        strsql.AppendLine("	        , R.Ricetta_Des as Des_Lib  ")
        strsql.AppendLine("	        , 0 as Appezza   ")
        strsql.AppendLine("	        , 0 as id_reg   ")
        strsql.AppendLine("	        , R.Validita_Inizio   ")
        strsql.AppendLine("	        , R.Validita_Fine   ")
        strsql.AppendLine("	        , R.Validita_Inizio as Agenda_Validita_Inizio   ")
        strsql.AppendLine("	        , R.Validita_Fine as Agenda_Validita_Fine   ")
        strsql.AppendLine("	        , Op.note as NoteGenerali   ")
        strsql.AppendLine("     From Ricette R  ")
        strsql.AppendLine("     inner Join Ricette_Operazioni Op  ")
        strsql.AppendLine("         On R.Ricetta_Cod = Op.Ricetta_Cod  ")
        strsql.AppendLine("     	And R.Ricetta_SuperUser = Op.Ricetta_SuperUser  ")
        strsql.AppendLine("     inner Join Ricette_dettagli D  ")
        strsql.AppendLine("         On D.Ricetta_Cod = Op.Ricetta_Cod  ")
        strsql.AppendLine("     	And D.Ricetta_SuperUser = Op.Ricetta_SuperUser  ")
        strsql.AppendLine("     inner Join Ricette_Dettaglio_Tecnico tec  ")
        strsql.AppendLine("         On D.Ricetta_Cod = tec.Ricetta_Cod  ")
        strsql.AppendLine("	        And D.Ricetta_SuperUser = tec.Ricetta_superUser  ")
        strsql.AppendLine("	        And D.Ricetta_Dettaglio_Cod = tec.Ricetta_Dettaglio_Cod  ")
        strsql.AppendLine("     Left Join RicettexCultivar rcul  ")
        strsql.AppendLine("         On rcul.Ricetta_Cod = Op.Ricetta_Cod  ")
        strsql.AppendLine("	        And rcul.Ricetta_SuperUser = Op.Ricetta_SuperUser ")
        strsql.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_RicetteXOp(
        ByRef strsql As StringBuilder,
        ByVal TipologiaLayer_Cod As Integer,
        Optional addCommaToEnd As Boolean = False,
        Optional addWithToStart As Boolean = False
        )

        If addWithToStart Then
            strsql.Append("with ")
        End If
        strsql.AppendLine("cte_ricettexop as ( ")
        strsql.AppendLine("		select ")
        strsql.AppendLine("			R.Piva ")
        strsql.AppendLine("			, R.sa_cod ")
        strsql.AppendLine("			, 0 as Appezza ")
        strsql.AppendLine("			, 0 as id_reg ")
        strsql.AppendLine("			, R.Ricetta_Cod as id_agenda ")
        strsql.AppendLine("			, ro.Ricetta_Operazione_Cod ")
        strsql.AppendLine("			, tec.Av_Cod ")
        strsql.AppendLine("			, rd.Udm_Cod ")
        strsql.AppendLine("			, tec.qta_ril as Qta ")
        strsql.AppendLine("			, R.Validita_Inizio ")
        strsql.AppendLine("		from Ricette R ")
        strsql.AppendLine("		inner join Ricette_Operazioni ro ")
        strsql.AppendLine("			On R.Ricetta_Cod = ro.Ricetta_Cod ")
        strsql.AppendLine("			And R.Ricetta_SuperUser = ro.Ricetta_SuperUser ")
        strsql.AppendLine("		inner join Ricette_dettagli rd ")
        strsql.AppendLine("			On rd.Ricetta_Cod = ro.Ricetta_Cod ")
        strsql.AppendLine("			And rd.Ricetta_SuperUser = ro.Ricetta_SuperUser ")
        strsql.AppendLine("		inner join Ricette_Dettaglio_Tecnico tec ")
        strsql.AppendLine("			On rd.Ricetta_Cod = tec.Ricetta_Cod ")
        strsql.AppendLine("			And rd.Ricetta_SuperUser = tec.Ricetta_superUser ")
        strsql.AppendLine("			And rd.Ricetta_Dettaglio_Cod = tec.Ricetta_Dettaglio_Cod ")
        strsql.AppendLine("		where R.Inviato >= 0 ")
        AggiungiFiltroLavCodRilievi(strsql, TipologiaLayer_Cod, "ro.lav_cod")
        strsql.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub AggiungiFiltroLavCodRilievi(
        ByRef strsql As StringBuilder,
        ByVal TipologiaLayer_Cod As Integer,
        ByVal colonnaLavCod As String)

        strsql.Append(" and " & colonnaLavCod)

        If TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Avversita Then
            strsql.AppendLine(" = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " ")
        Else
            strsql.AppendLine(" in (" &
                              CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & ", " &
                              CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & ") ")
        End If
    End Sub



    Private Sub SQL_CTE_Risk(ByRef strsql As StringBuilder,
                             Optional addCommaToEnd As Boolean = False,
                             Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            strsql.Append("with ")
        End If
        strsql.AppendLine("cte_risk as ( ")
        strsql.AppendLine("     Select MxAV_Cod, anag_valore, anag_Des ")
        strsql.AppendLine("     From MisuraXAvversita_Anagrafiche ")
        strsql.AppendLine("     union ")
        strsql.AppendLine("     Select ma.cod As MxAV_Cod, maa.*  ")
        strsql.AppendLine("     from( ")
        strsql.AppendLine("         Select ")
        strsql.AppendLine("         * ")
        strsql.AppendLine("         from ")
        strsql.AppendLine("         MisuraxAvversita ")
        strsql.AppendLine("         where ")
        strsql.AppendLine("         UDM_COD = 5001049 ")
        strsql.AppendLine("     )  ma  ")
        strsql.AppendLine("     inner Join(  ")
        strsql.AppendLine("     Select 0 As anag_valore, 'Nessun Rischio'  as anag_Des  ")
        strsql.AppendLine("     union  ")
        strsql.AppendLine("     Select 5 As anag_valore, 'Rischio Medio' ")
        strsql.AppendLine("     union ")
        strsql.AppendLine("     Select 10 As anag_valore, 'Rischio Alto' ")
        strsql.AppendLine("     ) maa ")
        strsql.AppendLine("     On 1=1 ")
        strsql.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_MisureXAvversita(ByRef StrSQL As StringBuilder,
                                         Optional addCommaToEnd As Boolean = False,
                                         Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_misurexavversita as ( ")
        StrSQL.AppendLine("	Select ")
        StrSQL.AppendLine("		113 as lav_cod ")
        StrSQL.AppendLine("		, mav.cod ")
        StrSQL.AppendLine("		, Avversita.Av_Cod ")
        StrSQL.AppendLine("		, Avversita.Av_Des_Vol ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_COD ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_DES ")
        StrSQL.AppendLine("		, mav.ordine ")
        StrSQL.AppendLine("	from ")
        StrSQL.AppendLine("		MisuraxAvversita mav ")
        StrSQL.AppendLine("		INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod ")
        StrSQL.AppendLine("		INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod ")
        StrSQL.AppendLine("	union ")
        StrSQL.AppendLine("	Select ")
        StrSQL.AppendLine("		109 as lav_cod ")
        StrSQL.AppendLine("		, 0 as cod ")
        StrSQL.AppendLine("		,  id1.ind_mat_Cod as av_cod ")
        StrSQL.AppendLine("		, id1.[IND_MAT_DES] as av_des_vol ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_COD ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_DES ")
        StrSQL.AppendLine("		, 1 as ordine ")
        StrSQL.AppendLine("	from ")
        StrSQL.AppendLine("		[dbo].[MisuraxIndiciMaturita] mXid ")
        StrSQL.AppendLine("		INNER Join  indiciMaturita id1 ON mXid.[IND_MAT_COD] = id1.[IND_MAT_COD] ")
        StrSQL.AppendLine("		INNER Join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod ")
        StrSQL.AppendLine("	union ")
        StrSQL.AppendLine("	Select ")
        StrSQL.AppendLine("		169 as lav_cod ")
        StrSQL.AppendLine("		, 0 as cod ")
        StrSQL.AppendLine("		,  id1.ind_mat_Cod as av_cod ")
        StrSQL.AppendLine("		, id1.[IND_MAT_DES] as av_des_vol ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_COD ")
        StrSQL.AppendLine("		, UnitaMisura.UDM_DES ")
        StrSQL.AppendLine("		, 1 as ordine ")
        StrSQL.AppendLine("	from ")
        StrSQL.AppendLine("		[dbo].[MisuraxIndiciMaturita] mXid ")
        StrSQL.AppendLine("		INNER Join  indiciMaturita id1 ON mXid.[IND_MAT_COD] = id1.[IND_MAT_COD] ")
        StrSQL.AppendLine("		INNER Join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Movimenti(ByRef StrSQL As StringBuilder,
                                  Optional addCommaToEnd As Boolean = False,
                                  Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_movimenti as ( ")
        StrSQL.AppendLine("     select ")
        StrSQL.AppendLine("		    Op.Piva ")
        StrSQL.AppendLine("		    , Op.sa_cod ")
        StrSQL.AppendLine("		    , Op.id_Agenda ")
        StrSQL.AppendLine("		    , 0 as Ricetta_Operazione_Cod ")
        StrSQL.AppendLine("		    , Op.Lav_Cod ")
        StrSQL.AppendLine("		    , Op.inviato ")
        StrSQL.AppendLine("		    , dest.mov_destinazioni_graphickey ")
        StrSQL.AppendLine("		    , tec.Av_Cod ")
        StrSQL.AppendLine("		    , case when Lav_Cod = 113 then D.Udm_Cod   else tec.dett_cod end as udm_cod ")
        StrSQL.AppendLine("		    , RegImp.CUL_COD ")
        StrSQL.AppendLine("		    , tec.FF_Classe ")
        StrSQL.AppendLine("		    , Op.Validita_Inizio as Data_Movimento ")
        StrSQL.AppendLine("		    , 0 as Sup_Imp ")
        StrSQL.AppendLine("		    , dest.qta as Qta ")
        StrSQL.AppendLine("		    , '' as Via_Stringa ")
        StrSQL.AppendLine("           , '' as APP_NOME ")
        StrSQL.AppendLine("		    , Op.Username_Modifica    ")
        StrSQL.AppendLine("		    , tec.Piezo1    ")
        StrSQL.AppendLine("		    , tec.Piezo2 as FF_Classe2    ")
        StrSQL.AppendLine("		    , Op.Des_Lib   ")
        StrSQL.AppendLine("		    , RegImp.Appezza  ")
        StrSQL.AppendLine("		    , RegImp.id_reg    ")
        StrSQL.AppendLine("		    , Op.Validita_Inizio  ")
        StrSQL.AppendLine("		    , Op.Validita_Fine    ")
        StrSQL.AppendLine("		    , Op.Validita_Inizio as Agenda_Validita_Inizio    ")
        StrSQL.AppendLine("		    , Op.Validita_Fine as Agenda_Validita_Fine    ")
        StrSQL.AppendLine("		    , '' as NoteGenerali    ")
        StrSQL.AppendLine("	    from Agenda Op  ")
        StrSQL.AppendLine("		    inner join Movimenti_dettagli D  ")
        StrSQL.AppendLine("		    	on Op.Id_Agenda = D.Id_Agenda  ")
        StrSQL.AppendLine("		    inner join  Mov_Dettaglio_Tecnico tec ")
        StrSQL.AppendLine("		    	on Op.Id_Agenda = tec.Id_Agenda  ")
        StrSQL.AppendLine("		    	and D.id_mov = tec.id_mov  ")
        StrSQL.AppendLine("		    	and D.Id_Mov_Det = tec.Id_Mov_Det  ")
        StrSQL.AppendLine("		    inner join mov_Destinazioni dest  ")
        StrSQL.AppendLine("		    	on dest.id_agenda = D.id_agenda  ")
        StrSQL.AppendLine("		    	and dest.id_mov_det = D.id_Mov_det  ")
        StrSQL.AppendLine("		    inner join Reg_Impianti RegImp  ")
        StrSQL.AppendLine("		    	on Regimp.Piva = dest.Piva  ")
        StrSQL.AppendLine("		    	and Regimp.sa_cod = dest.sa_cod  ")
        StrSQL.AppendLine("		    	and Regimp.appezza = dest.appezza  ")
        StrSQL.AppendLine("		    	and Regimp.id_reg = dest.id_destinazione  ")
        StrSQL.AppendLine("	    where op.lav_cod in (" & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & ", " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & ", " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & ")   ")
        StrSQL.AppendLine("	    union  ")
        StrSQL.AppendLine("	    Select   ")
        StrSQL.AppendLine("		    Piva  ")
        StrSQL.AppendLine("		    , sa_cod ")
        StrSQL.AppendLine("		    , -1 as id_Agenda     ")
        StrSQL.AppendLine("		    , 0 as Ricetta_Operazione_Cod     ")
        StrSQL.AppendLine("		    , " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " as Lav_Cod      ")
        StrSQL.AppendLine("		    , 0 as inviato    ")
        StrSQL.AppendLine("           , '' as mov_destinazioni_graphickey   ")
        StrSQL.AppendLine("		    , Av_Cod      ")
        StrSQL.AppendLine("		    , udm_cod  --capire  ")
        StrSQL.AppendLine("		    , CUL_COD     ")
        StrSQL.AppendLine("		    , 0 as FF_Classe -- Per adesso no ")
        StrSQL.AppendLine("		    , CAST(srv.Data_Riferimento as date) as Data_Movimento     ")
        StrSQL.AppendLine("		    , 0 as Sup_Imp    ")
        StrSQL.AppendLine("		    , Qta_Ril as QTA  --capire   ")
        StrSQL.AppendLine("           , '' as Via_Stringa   ")
        StrSQL.AppendLine("		    , '' as APP_NOME    ")
        StrSQL.AppendLine("		    , d.Username_Modifica  ")
        StrSQL.AppendLine("		    , 0 as Piezo1     ")
        StrSQL.AppendLine("		    , 0 as FF_Classe2    ")
        StrSQL.AppendLine("		    , '' as Des_Lib   ")
        StrSQL.AppendLine("		    , Appezza     ")
        StrSQL.AppendLine("		    , id_reg      ")
        StrSQL.AppendLine("		    , d.Validita_Inizio   ")
        StrSQL.AppendLine("		    , d.Validita_Fine     ")
        StrSQL.AppendLine("		    , d.Validita_Inizio as Agenda_Validita_Inizio     ")
        StrSQL.AppendLine("		    , d. Validita_Fine as Agenda_Validita_Fine   ")
        StrSQL.AppendLine("         , '' as NoteGenerali  ")
        StrSQL.AppendLine("	    from (  ")
        StrSQL.AppendLine("		    select ")
        StrSQL.AppendLine("			    d.* ")
        StrSQL.AppendLine("			    , c.Veg_Cod ")
        StrSQL.AppendLine("			    , c.Cul_Cod  ")
        StrSQL.AppendLine("		    From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d  ")
        StrSQL.AppendLine("			inner Join Reg_Impianti i  ")
        StrSQL.AppendLine("				On i.PIVA = d.piva   ")
        StrSQL.AppendLine("				And i.SA_COD = d.sa_cod  ")
        StrSQL.AppendLine("				And i.APPEZZA = d.appezza  ")
        StrSQL.AppendLine("				And i.ID_REG = d.id_reg  ")
        StrSQL.AppendLine("			inner Join Cultivar c  ")
        StrSQL.AppendLine("				On c.Cul_Cod = i.CUL_COD  ")
        StrSQL.AppendLine("		) d  ")
        StrSQL.AppendLine("	    inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv  ")
        StrSQL.AppendLine("	    	On d.tipo_sorgente = srv.tipo_sorgente  ")
        StrSQL.AppendLine("	    	And d.Stazione_Cod = srv.stazione_Cod  ")
        'StrSQL.AppendLine("	    inner Join modelliPrevisionali p  ")
        'StrSQL.AppendLine("	    	On p.Mod_Cod = srv.Mod_Cod  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_GIS_TipoEntita(ByRef StrSQL As StringBuilder,
                                       Optional addCommaToEnd As Boolean = False,
                                       Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_GIS_TipoEntita as ( ")
        StrSQL.AppendLine("     Select ")
        StrSQL.AppendLine("         TipoEntita_Cod ")
        StrSQL.AppendLine("     From ")
        StrSQL.AppendLine("         GIS_TipoEntita ")
        StrSQL.AppendLine("     Where ")
        StrSQL.AppendLine("         LayerElementiGrafici_Cod = 19 ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Imprese_Filtrone(ByRef StrSQL As StringBuilder,
                                         ByRef Filtro As String,
                                         ByRef TipoSelect As enum_TipoSelect_FiltroneSuperNova,
                                         ByRef Ordinamento As String,
                                         ByRef Join As JoinFiltrone,
                                         objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional addCommaToEnd As Boolean = False,
                                         Optional addWithToStart As Boolean = False)

        Dim objFiltrone As New AgronicaCoreUtility.Filtrone
        Dim sqlFiltroneImprese = objFiltrone.CreaStringaQueryPerDTFiltrone(objParametri_Server, Filtro, TipoSelect, Ordinamento, Join, True)

        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_imprese as ( ")
        StrSQL.AppendLine(sqlFiltroneImprese.ToOrigin())
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_RicetteConAvversita(ByRef StrSQL As StringBuilder,
                                            Optional addCommaToEnd As Boolean = False,
                                            Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_ricetteconavversita as ( ")
        StrSQL.AppendLine("     SELECT ")
        StrSQL.AppendLine("		    case when mav.cod = mav.cod then  ")
        StrSQL.AppendLine("		    	ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '')  ")
        StrSQL.AppendLine("		    else  ")
        StrSQL.AppendLine("		    	ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '')  ")
        StrSQL.AppendLine("		    end AS UDM_DES  ")
        'StrSQL.AppendLine("		    , case when mav.cod = mav.cod then  op.Qta else '' end as Qta  ")
        'StrSQL.AppendLine("		    , case when mav.cod = mav.cod then  ")
        'StrSQL.AppendLine("		    	coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) )  ")
        'StrSQL.AppendLine("		    else  ")
        'StrSQL.AppendLine("		    	''  ")
        'StrSQL.AppendLine("		    end AS qta_Descrittiva  ")
        StrSQL.AppendLine("		    , op.Qta ")
        StrSQL.AppendLine("		    , coalesce((select anag.anag_des from cte_risk anag where anag.MxAV_Cod = mav.COD And anag.Anag_valore = op.Qta ), cast( op.qta as varchar(1000))) AS qta_Descrittiva ")
        StrSQL.AppendLine("		    , op.piva ")
        StrSQL.AppendLine("		    , op.id_agenda ")
        StrSQL.AppendLine("		    , op.sa_Cod ")
        StrSQL.AppendLine("		    , op.appezza ")
        StrSQL.AppendLine("		    , op.id_reg ")
        StrSQL.AppendLine("		    , op.Validita_Inizio as DataOperazione  ")
        StrSQL.AppendLine("		    , mav.ordine  ")
        StrSQL.AppendLine("		    , op.Ricetta_Operazione_Cod  ")
        StrSQL.AppendLine("			, op.inviato  ")
        StrSQL.AppendLine("	    FROM  cte_avversita mav")
        StrSQL.AppendLine("		    inner join cte_ricette op ")
        StrSQL.AppendLine("		    on op.av_cod = mav.av_cod  ")
        StrSQL.AppendLine("		    and op.udm_cod = mav.udm_cod  ")
        'StrSQL.AppendLine("		    LEFT JOIN Cultivar  ")
        'StrSQL.AppendLine("		    		ON Cultivar.Cul_Cod = op.CUL_COD  ")
        'StrSQL.AppendLine("		    LEFT JOIN SpecieVegetali  ")
        'StrSQL.AppendLine("		    	ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
        'StrSQL.AppendLine("		    LEFT JOIN Imprese AS i  ")
        'StrSQL.AppendLine("		    	ON i.PIVA = op.PIVA  ")
        'StrSQL.AppendLine("		    LEFT JOIN Centri_Aziendali AS sa  ")
        'StrSQL.AppendLine("		    	ON sa.PIVA = op.PIVA  ")
        'StrSQL.AppendLine("		    	AND sa.sa_cod = op.sa_cod  ")
        'StrSQL.AppendLine("		    LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe  ")
        'StrSQL.AppendLine("		    LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2  ")
        'StrSQL.AppendLine("		    LEFT OUTER JOIN  ")
        'StrSQL.AppendLine("		    	Grafica AS G ON G.Piva = op.PIVA AND G.Sa_Cod = op.Sa_Cod AND G.Id = op.mov_destinazioni_graphickey  ")
        'StrSQL.AppendLine("		    left join (  ")
        'StrSQL.AppendLine("		    	select * from cte_risk ")
        'StrSQL.AppendLine("		    	) anag  ")
        'StrSQL.AppendLine("		    on anag.MxAV_Cod = mav.COD  ")
        'StrSQL.AppendLine("		    and anag.Anag_valore = op.Qta  ")
        'StrSQL.AppendLine("		WHERE 1=1  ")
        'StrSQL.AppendLine("		AND   op.Inviato >=0  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Operazioni(ByRef StrSQL As StringBuilder,
                                   Optional addCommaToEnd As Boolean = False,
                                   Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_operazioni as ( ")
        StrSQL.AppendLine("		SELECT  ")
        StrSQL.AppendLine("			ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') AS UDM_DES  ")
        'StrSQL.AppendLine("			, case when mav.cod = mav.cod then  op.Qta else '' end as Qta  ")
        'StrSQL.AppendLine("			, case when mav.cod = mav.cod then  ")
        'StrSQL.AppendLine("				coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) )  ")
        'StrSQL.AppendLine("			else  ")
        'StrSQL.AppendLine("				''  ")
        'StrSQL.AppendLine("			end AS qta_Descrittiva  ")
        StrSQL.AppendLine("		    , op.Qta ")
        StrSQL.AppendLine("		    , coalesce((select anag.anag_des from cte_risk anag where anag.MxAV_Cod = mav.COD And anag.Anag_valore = op.Qta ), cast( op.qta as varchar(1000))) AS qta_Descrittiva ")
        StrSQL.AppendLine("			, op.piva ")
        StrSQL.AppendLine("			, op.id_agenda ")
        StrSQL.AppendLine("			, op.sa_Cod ")
        StrSQL.AppendLine("			, op.appezza ")
        StrSQL.AppendLine("			, op.id_reg ")
        StrSQL.AppendLine("			, op.Validita_Inizio as DataOperazione  ")
        StrSQL.AppendLine("			, mav.ordine  ")
        StrSQL.AppendLine("			, 0 as Ricetta_Operazione_Cod  ")
        StrSQL.AppendLine("			, op.inviato  ")
        StrSQL.AppendLine("		FROM cte_misurexavversita mav ")
        StrSQL.AppendLine("			inner join cte_movimenti op ")
        StrSQL.AppendLine("			on ( ( op.av_cod = mav.av_cod and op.lav_cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & ") ")
        StrSQL.AppendLine(String.Format("				or ( op.FF_Classe = mav.av_cod and op.lav_cod in ({0}, {1}, {2})) ) ",
                          CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE,
                          CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA,
                          CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA))
        StrSQL.AppendLine("				and op.udm_cod = mav.udm_cod  ")
        StrSQL.AppendLine("				and op.lav_cod = mav.lav_cod  ")
        'StrSQL.AppendLine("			LEFT JOIN Cultivar  ")
        'StrSQL.AppendLine("				ON Cultivar.Cul_Cod = op.CUL_COD  ")
        'StrSQL.AppendLine("			LEFT JOIN SpecieVegetali  ")
        'StrSQL.AppendLine("				ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
        'StrSQL.AppendLine("			LEFT JOIN Imprese AS i  ")
        'StrSQL.AppendLine("				ON i.PIVA = op.PIVA  ")
        'StrSQL.AppendLine("			LEFT JOIN Centri_Aziendali AS sa  ")
        'StrSQL.AppendLine("				ON sa.PIVA = op.PIVA  ")
        'StrSQL.AppendLine("				AND sa.sa_cod = op.sa_cod  ")
        'StrSQL.AppendLine("			LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe  ")
        'StrSQL.AppendLine("			LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2  ")
        'StrSQL.AppendLine("			left join (  ")
        'StrSQL.AppendLine("				select * from cte_risk ")
        'StrSQL.AppendLine("			) anag  ")
        'StrSQL.AppendLine("			on anag.MxAV_Cod = mav.COD  ")
        'StrSQL.AppendLine("				and anag.Anag_valore = op.Qta  ")
        'StrSQL.AppendLine("			WHERE 1=1  ")
        'StrSQL.AppendLine("				AND   op.Inviato >=0  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_AvversitaCampo_IndiciMaturita_ResaRaccolta(
        ByRef StrSQL As StringBuilder,
        ByVal TipologiaLayer_Cod As Integer,
        Optional addCommaToEnd As Boolean = False,
        Optional addWithToStart As Boolean = False
        )

        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_rilievi_113_109_169 as ( ")
        If TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Avversita Then
            StrSQL.AppendLine("     select ")
            StrSQL.AppendLine("            " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " as lav_cod ")
            StrSQL.AppendLine("          , mav.cod ")
            StrSQL.AppendLine("          , Avversita.Av_Cod ")
            StrSQL.AppendLine("          , Avversita.Av_Des_Vol ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_COD ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_DES ")
            StrSQL.AppendLine("          , mav.ordine ")
            StrSQL.AppendLine("          , mav.veg_cod ")
            StrSQL.AppendLine("     from   MisuraxAvversita mav ")
            StrSQL.AppendLine("     inner  join Avversita ON mav.Av_Cod = Avversita.Av_Cod ")
            StrSQL.AppendLine("     inner  join UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod ")
        Else
            StrSQL.AppendLine("     select ")
            StrSQL.AppendLine("            " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & " as lav_cod ")
            StrSQL.AppendLine("          , 0 as cod ")
            StrSQL.AppendLine("          , id1.ind_mat_Cod as av_cod ")
            StrSQL.AppendLine("          , id1.[IND_MAT_DES] as av_des_vol ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_COD ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_DES ")
            StrSQL.AppendLine("          , 1 as ordine ")
            StrSQL.AppendLine("          , 0 as veg_cod ")
            StrSQL.AppendLine("     from	 MisuraxIndiciMaturita mXid ")
            StrSQL.AppendLine("     inner  join indiciMaturita id1 ON mXid.IND_MAT_COD = id1.IND_MAT_COD ")
            StrSQL.AppendLine("     inner  join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod ")
            StrSQL.AppendLine("     UNION ")
            StrSQL.AppendLine("     select ")
            StrSQL.AppendLine("            " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " as lav_cod ")
            StrSQL.AppendLine("          , 0 as cod ")
            StrSQL.AppendLine("          , id1.ind_mat_Cod as av_cod ")
            StrSQL.AppendLine("          , id1.[IND_MAT_DES] as av_des_vol ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_COD  ")
            StrSQL.AppendLine("          , UnitaMisura.UDM_DES ")
            StrSQL.AppendLine("          , 1 as ordine  ")
            StrSQL.AppendLine("          , 0 as veg_cod ")
            StrSQL.AppendLine("     from   MisuraxIndiciMaturita mXid ")
            StrSQL.AppendLine("     inner  join indiciMaturita id1 ON mXid.IND_MAT_COD = id1.IND_MAT_COD ")
            StrSQL.AppendLine("     inner  join UnitaMisura ON mXid.Udm_Cod = UnitaMisura.Udm_Cod ")
        End If
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_ClassiFenologicheRicette(
        ByRef StrSQL As StringBuilder,
        Optional addCommaToEnd As Boolean = False,
        Optional addWithToStart As Boolean = False)

        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        'TODO: intercettare solo ricette relative a rilievi fenologici
        StrSQL.AppendLine("cte_classifenologichericette as ( ")
        StrSQL.AppendLine("   select ricetta_operazione_cod, ")
        StrSQL.AppendLine("          cast(FF_Classe as int) as ff_cod ")
        StrSQL.AppendLine("   from   Ricette_Dettaglio_Tecnico ")
        StrSQL.AppendLine("   where  ff_classe <> 0 ")
        StrSQL.AppendLine("   union ")
        StrSQL.AppendLine("   select ricetta_operazione_cod, ")
        StrSQL.AppendLine("          cast(piezo2 as int) as ff_cod ")
        StrSQL.AppendLine("   from   Ricette_Dettaglio_Tecnico ")
        StrSQL.AppendLine("   where  piezo2 <> 0 ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub
    Private Sub SQL_CTE_ClassiFenologicheAgenda(ByRef StrSQL As StringBuilder,
                               Optional addCommaToEnd As Boolean = False,
                               Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_classifenologicheagenda as ( ")
        StrSQL.AppendLine("   select mdt.id_Agenda, ")
        StrSQL.AppendLine("          cast(mdt.FF_Classe as int) as ff_cod ")
        StrSQL.AppendLine("   from   Mov_Dettaglio_Tecnico mdt")
        StrSQL.AppendLine("   inner  join agenda a on a.id_agenda = mdt.id_agenda ")
        StrSQL.AppendLine("   where  mdt.ff_classe <> 0 ")
        StrSQL.AppendLine("   and    a.lav_cod = " & CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE & " ")
        StrSQL.AppendLine("   union ")
        StrSQL.AppendLine("   select mdt.id_Agenda, ")
        StrSQL.AppendLine("          cast(mdt.piezo2 as int) as ff_cod ")
        StrSQL.AppendLine("   from   Mov_Dettaglio_Tecnico mdt ")
        StrSQL.AppendLine("   inner  join agenda a on a.id_agenda = mdt.id_agenda ")
        StrSQL.AppendLine("   and    a.lav_cod = " & CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE & " ")
        StrSQL.AppendLine("   where  mdt.piezo2 <> 0  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_RicetteAvversita(ByRef StrSQL As StringBuilder,
                               Optional addCommaToEnd As Boolean = False,
                               Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_ricetteavversita as ( ")
        StrSQL.AppendLine("	select ")
        StrSQL.AppendLine("		Ricetta_Operazione_cod, ")
        StrSQL.AppendLine("		av_Cod ")
        StrSQL.AppendLine("	from ")
        StrSQL.AppendLine("		Ricette_Dettaglio_Tecnico  ")
        StrSQL.AppendLine("	where ")
        StrSQL.AppendLine("		av_cod <> 0  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_MovAgendaXAvversita(ByRef StrSQL As StringBuilder,
                                            Optional addCommaToEnd As Boolean = False,
                                            Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_movagendaxavversita as ( ")
        StrSQL.AppendLine("     select a.id_Agenda, ")
        StrSQL.AppendLine("            t.av_cod, ")
        StrSQL.AppendLine("            a.PIVA, ")
        StrSQL.AppendLine("            a.Sa_Cod, ")
        StrSQL.AppendLine("            h.Appezza, ")
        StrSQL.AppendLine("            h.Id_Destinazione as id_reg, ")
        StrSQL.AppendLine("            t.id_mov_det ")
        StrSQL.AppendLine("     from   Mov_Dettaglio_Tecnico t ")
        StrSQL.AppendLine("     inner  join Mov_Destinazioni h On t.Piva = h.Piva ")
        StrSQL.AppendLine("                                   And t.Id_Agenda = h.Id_Agenda ")
        StrSQL.AppendLine("                                   And t.Id_Mov_Det = h.Id_Mov_Det ")
        StrSQL.AppendLine("     inner  join agenda a On a.id_Agenda = t.id_agenda ")
        StrSQL.AppendLine("     where  a.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " ")
        StrSQL.AppendLine("     And    t.av_cod <> 0 ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Cultivar(ByRef StrSQL As StringBuilder,
                               Optional addCommaToEnd As Boolean = False,
                               Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_cultivar as ( ")
        StrSQL.AppendLine("     select ")
        StrSQL.AppendLine("         0 as Cul_Cod ")
        StrSQL.AppendLine("         ,0 as Veg_Cod ")
        StrSQL.AppendLine("     union all ")
        StrSQL.AppendLine("     select ")
        StrSQL.AppendLine("         Cul_Cod ")
        StrSQL.AppendLine("         ,Veg_Cod ")
        StrSQL.AppendLine("     from cultivar ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Geodata(ByRef StrSQL As StringBuilder,
                                Optional addCommaToEnd As Boolean = False,
                                Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_geodata as ( ")
        StrSQL.AppendLine("     Select ")
        StrSQL.AppendLine("         e.Punto_Entita_Cod as Entita_Cod ")
        StrSQL.AppendLine("         , g1.Poligono_GeoEntity ")
        StrSQL.AppendLine("         , g1.Poligono_GeoEntity_WKT ")
        StrSQL.AppendLine("     from ( ")
        StrSQL.AppendLine("         Select ")
        StrSQL.AppendLine("             e.Entita_Cod as Punto_Entita_Cod ")
        StrSQL.AppendLine("             , e1.Entita_Cod  as poligono_Entita_Cod ")
        StrSQL.AppendLine("             , g.LayerElementiGrafici_Cod ")
        StrSQL.AppendLine("             , g.ElementoGrafico_Des ")
        StrSQL.AppendLine("             , g.Flag_GPS ")
        StrSQL.AppendLine("         From GIS_ElementiGrafici g ")
        StrSQL.AppendLine("	        inner Join GIS_Entita e ")
        StrSQL.AppendLine("             On g.Entita_Cod = e.Entita_Cod ")
        StrSQL.AppendLine("	        inner Join GIS_Entita e1 ")
        StrSQL.AppendLine("             On e1.piva  = e.Piva ")
        StrSQL.AppendLine("		        And e1.sa_Cod = e.Sa_Cod ")
        StrSQL.AppendLine("		        And e1.appezza = e.Appezza ")
        StrSQL.AppendLine("		        And e1.id_imp = e.id_imp ")
        StrSQL.AppendLine("		        And e1.TipoEntita_Cod In (19, 20, 21, 22,23) ")
        StrSQL.AppendLine("		        And e.TipoEntita_Cod Not In (19, 20, 21, 22,23) ")
        StrSQL.AppendLine("	        where g.Poligono_GeoEntity.STGeometryType() = 'Point' ")
        StrSQL.AppendLine("         union ")
        StrSQL.AppendLine("         Select ")
        StrSQL.AppendLine("             e.Entita_Cod as Punto_Entita_Cod ")
        StrSQL.AppendLine("		        , e.Entita_Cod as Poligono_Entita_Cod ")
        StrSQL.AppendLine("		        , g.LayerElementiGrafici_Cod ")
        StrSQL.AppendLine("		        , g.ElementoGrafico_Des ")
        StrSQL.AppendLine("		        , g.Flag_GPS ")
        StrSQL.AppendLine("	        From GIS_ElementiGrafici g ")
        StrSQL.AppendLine("	        inner Join GIS_Entita e ")
        StrSQL.AppendLine("         On g.PivaSuperUser = e.PivaSuperUser and g.Entita_Cod = e.Entita_Cod ")
        StrSQL.AppendLine("	        where Poligono_GeoEntity.STGeometryType() <> 'Point' ")
        StrSQL.AppendLine("     ) e ")
        StrSQL.AppendLine("     inner Join GIS_ElementiGrafici g1 ")
        StrSQL.AppendLine("     On g1.Entita_Cod = e.poligono_Entita_Cod ")
        'Else
        '    StrSQL.AppendLine("     Select ")
        '    StrSQL.AppendLine("         Poligono_GeoEntity.AsGml() As geodata ")
        '    StrSQL.AppendLine("         , entita_cod ")
        '    StrSQL.AppendLine("     from ")
        '    StrSQL.AppendLine("        GIS_ElementiGrafici ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_SportelloSementieri(ByRef StrSQL As StringBuilder,
                                            ByVal Sportello_Configurazione_Cod As Integer,
                                            Optional addCommaToEnd As Boolean = False,
                                            Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_sportello_sementieri as ( ")
        '--da sportello corrente
        StrSQL.AppendLine("     Select ")
        StrSQL.AppendLine("         MS.Grva_Cod AS grva_cod ")
        StrSQL.AppendLine("         , ms.Veg_Cod AS veg_cod ")
        StrSQL.AppendLine("         , ms.grfi_cod AS grfi_cod ")
        StrSQL.AppendLine("         , ms.Hybrid AS hybrid ")
        StrSQL.AppendLine("         , vis.Visibilita_Impianti ")
        StrSQL.AppendLine("         , vis.PermessoModifiche ")
        StrSQL.AppendLine("         , rOriginale.PIVA ")
        StrSQL.AppendLine("         , rOriginale.SA_COD ")
        StrSQL.AppendLine("         , rOriginale.APPEZZA ")
        StrSQL.AppendLine("         , rOriginale.ID_REG	")
        StrSQL.AppendLine("         , 0 AS InOsservazione ")
        StrSQL.AppendLine("         , '' AS Colore_Primario ")
        StrSQL.AppendLine("         , '' AS Colore_Retinatura ")
        StrSQL.AppendLine("         , 0 AS Trasparenza ")
        StrSQL.AppendLine("     From Reg_Impianti rOriginale ")
        StrSQL.AppendLine("     INNER Join Cultivar vegOriginale ")
        StrSQL.AppendLine("         On vegOriginale.Cul_Cod = rOriginale.CUL_COD ")
        StrSQL.AppendLine("     INNER Join Mappatura_Specie ms ")
        StrSQL.AppendLine("         On (ms.Veg_Cod = vegOriginale.Veg_Cod)  ")
        StrSQL.AppendLine("	        And (ms.Grva_Cod = ABS(rOriginale.GRVA_Cod_VEG))  ")
        StrSQL.AppendLine("         And (ms.grfi_Cod = rOriginale.GRFI_COD)  ")
        StrSQL.AppendLine("         And (ms.Hybrid = (Case When vegOriginale.veg_cod In (6, 5000021, 70, 69) Then -1 Else (Case When rOriginale.GRVA_Cod_VEG < 0 Then 1 Else 0 End) End))  ")
        StrSQL.AppendLine("     INNER Join Sementieri_Sportello_ConfigurazioneXmappatura_specie sscms  ")
        StrSQL.AppendLine("         On sscms.ID_Specie = ms.ID_Specie  ")
        StrSQL.AppendLine("         And sscms.ID_SottoSpecie = ms.ID_SottoSpecie  ")
        StrSQL.AppendLine("         And sscms.id_genotipo = ms.id_genotipo  ")
        StrSQL.AppendLine("         And sscms.id_gruppo = ms.id_gruppo  ")
        StrSQL.AppendLine("     Left Join( ")
        StrSQL.AppendLine("         Select TOP 1 ssp.Visibilita_Impianti, Sementieri_Sportello_Configurazione_cod, Case When ssp.DestinazioneSalvataggio = -1 Then 'False' ELSE 'True' END AS PermessoModifiche  ")
        StrSQL.AppendLine("         From Sementieri_Sportello_ConfigurazioneXPassaggi sscp  ")
        StrSQL.AppendLine("         INNER Join Sementieri_Sportello_Passaggi ssp  ")
        StrSQL.AppendLine("             On sscp.Sementieri_Sportello_Passaggi_cod = ssp.Sementieri_Sportello_Passaggi_cod  ")
        StrSQL.AppendLine("             And sscp.Sementieri_Sportello_Configurazione_cod = " & Agro_SQL_SaveNum(Sportello_Configurazione_Cod))
        StrSQL.AppendLine("             And CAST(GETDATE() AS DATE) BETWEEN sscp.Data_Inizio And sscp.Data_fine  ")
        StrSQL.AppendLine("         ) vis  ")
        StrSQL.AppendLine("         On sscms.Sementieri_Sportello_Configurazione_cod = vis.Sementieri_Sportello_Configurazione_cod  ")
        StrSQL.AppendLine("     WHERE sscms.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sportello_Configurazione_Cod))
        StrSQL.AppendLine("     And rOriginale.Validita_inizio >= @DataI  ")
        StrSQL.AppendLine("     And rOriginale.Validita_fine <= @DataF  ")
        StrSQL.AppendLine("     UNION ")
        '--da osservati per specie vegetale
        StrSQL.AppendLine("     Select ")
        StrSQL.AppendLine("         Osservato_GRVA_Cod AS grva_cod  ")
        StrSQL.AppendLine("         , Osservato_veg_cod AS veg_cod  ")
        StrSQL.AppendLine("         , Osservato_grfi_cod AS grfi_cod  ")
        StrSQL.AppendLine("         , Osservato_Hybrid AS hybrid  ")
        StrSQL.AppendLine("         , 1 as Visibilita_Impianti  ")
        StrSQL.AppendLine("         , 'False' AS PermessoModifiche  ")
        StrSQL.AppendLine("         , rEsteso.PIVA  ")
        StrSQL.AppendLine("         , rEsteso.SA_COD  ")
        StrSQL.AppendLine("         , rEsteso.APPEZZA  ")
        StrSQL.AppendLine("         , rEsteso.ID_REG  ")
        StrSQL.AppendLine("         , 1 AS InOsservazione  ")
        StrSQL.AppendLine("         , COALESCE(oss.Colore_Primario, '') AS Colore_Primario  ")
        StrSQL.AppendLine("         , COALESCE(oss.Colore_Retinatura, '') AS Colore_Retinatura  ")
        StrSQL.AppendLine("         , COALESCE(oss.Trasparenza, 0) AS Trasparenza  ")
        StrSQL.AppendLine("     From Reg_Impianti rEsteso  ")
        StrSQL.AppendLine("     INNER Join Cultivar vegEsteso  ")
        StrSQL.AppendLine("         On vegEsteso.Cul_Cod = rEsteso.CUL_COD  ")
        StrSQL.AppendLine("     INNER Join SpecieVegetaliInOsservazione oss  ")
        StrSQL.AppendLine("         On (oss.Osservato_cul_cod = rEsteso.CUL_COD Or oss.osservato_CUL_COD Is NULL)  ")
        StrSQL.AppendLine("         And (oss.Osservato_Veg_Cod = vegEsteso.Veg_Cod Or oss.Osservato_Veg_Cod Is NULL)  ")
        StrSQL.AppendLine("         And (oss.Osservato_Grva_Cod = rEsteso.GRVA_Cod_VEG Or oss.Osservato_Grva_Cod Is NULL)  ")
        StrSQL.AppendLine("         And (oss.Osservato_grfi_Cod = rEsteso.GRFI_COD Or Osservato_grfi_Cod Is NULL)  ")
        StrSQL.AppendLine("         And (oss.Osservato_Hybrid = (CASE WHEN vegEsteso.veg_cod IN (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rEsteso.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) Or oss.Osservato_Hybrid Is NULL)  ")
        StrSQL.AppendLine("     WHERE oss.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sportello_Configurazione_Cod))
        StrSQL.AppendLine("     And oss.Osservato_Sementieri_Sportello_Configurazione_Cod Is NULL  ")
        StrSQL.AppendLine("     And Not (@DataF < DATEADD(DAY, -ISNULL(oss.Osservazione_GG_Precedenti, 0), rEsteso.Validita_inizio) Or DATEADD(DAY, ISNULL(oss.Osservazione_GG_Successivi, 0), rEsteso.Validita_fine) < @DataI)  ")
        StrSQL.AppendLine("     UNION ")
        '--da osservati per sportello
        StrSQL.AppendLine("     Select  ")
        StrSQL.AppendLine("         MS.Grva_Cod AS grva_cod  ")
        StrSQL.AppendLine("         , ms.Veg_Cod AS veg_cod  ")
        StrSQL.AppendLine("         , ms.grfi_cod AS grfi_cod  ")
        StrSQL.AppendLine("         , ms.Hybrid AS hybrid  ")
        StrSQL.AppendLine("         , 1 AS Visibilita_Impianti  ")
        StrSQL.AppendLine("         , 'False' AS PermessoModifiche  ")
        StrSQL.AppendLine("         , rEsteso.PIVA  ")
        StrSQL.AppendLine("         , rEsteso.SA_COD  ")
        StrSQL.AppendLine("         , rEsteso.APPEZZA  ")
        StrSQL.AppendLine("         , rEsteso.ID_REG  ")
        StrSQL.AppendLine("         , 1 AS InOsservazione  ")
        StrSQL.AppendLine("         , COALESCE(oss.Colore_Primario, '') AS Colore_Promario  ")
        StrSQL.AppendLine("         , COALESCE(oss.Colore_Retinatura, '') AS Colore_Retinatura  ")
        StrSQL.AppendLine("         , COALESCE(oss.Trasparenza, 0) AS Trasparenza  ")
        StrSQL.AppendLine("     From Reg_Impianti rEsteso  ")
        StrSQL.AppendLine("     INNER Join Cultivar vegEsteso  ")
        StrSQL.AppendLine("         On vegEsteso.Cul_Cod = rEsteso.CUL_COD  ")
        StrSQL.AppendLine("     INNER Join Mappatura_Specie ms  ")
        StrSQL.AppendLine("         On (ms.Veg_Cod = vegEsteso.Veg_Cod) ")
        StrSQL.AppendLine("         And (ms.Grva_Cod = abs(rEsteso.GRVA_Cod_VEG))  ")
        StrSQL.AppendLine("         And (ms.grfi_Cod = rEsteso.GRFI_COD)  ")
        StrSQL.AppendLine("         And (ms.Hybrid = (CASE WHEN vegEsteso.veg_cod IN (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rEsteso.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END))  ")
        StrSQL.AppendLine("     INNER Join Sementieri_Sportello_ConfigurazioneXmappatura_specie sscms  ")
        StrSQL.AppendLine("         On sscms.ID_Specie = ms.ID_Specie  ")
        StrSQL.AppendLine("         And sscms.ID_SottoSpecie = ms.ID_SottoSpecie  ")
        StrSQL.AppendLine("         And sscms.id_genotipo = ms.id_genotipo  ")
        StrSQL.AppendLine("         And sscms.id_gruppo = ms.id_gruppo  ")
        StrSQL.AppendLine("     INNER Join SpecieVegetaliInOsservazione oss  ")
        StrSQL.AppendLine("         On oss.Osservato_Sementieri_Sportello_Configurazione_Cod = sscms.Sementieri_Sportello_Configurazione_cod  ")
        StrSQL.AppendLine("     INNER Join Sementieri_Sportello_Configurazione ssc  ")
        StrSQL.AppendLine("         On ssc.Sementieri_Sportello_Configurazione_cod = oss.Osservato_Sementieri_Sportello_Configurazione_Cod  ")
        StrSQL.AppendLine("	        And rEsteso.Validita_inizio = ssc.Validita_Inizio  ")
        StrSQL.AppendLine("	        And rEsteso.Validita_fine = ssc.Validita_Fine  ")
        StrSQL.AppendLine("     WHERE oss.Sementieri_Sportello_Configurazione_Cod = " & Agro_SQL_SaveNum(Sportello_Configurazione_Cod))
        StrSQL.AppendLine("	    And oss.Osservato_Sementieri_Sportello_Configurazione_Cod Is Not NULL  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_ElencoEntita(ByVal piva As String,
                                     ByVal sa_cod As Integer,
                                     ByVal id_cod As Integer,
                                     ByVal UtentiVisibilitaAppoggioApplica As Boolean,
                                     ByVal wktBoundaySTIntersects As String,
                                     ByVal cfgAlbero As ConfigurazioneAlbero,
                                     ByVal layerElementiGrafici_cod As List(Of Integer),
                                     ByVal includiJoinTabelleCatasto As Boolean,
                                     ByVal objParametri As AgronicaCoreParametri,
                                     ByRef StrSQL As StringBuilder,
                                     Optional ByVal ParametriFiltroXSessione As String = "",
                                     Optional addCommaToEnd As Boolean = False,
                                     Optional addWithToStart As Boolean = False,
                                     Optional impostazioneUtente As Impostazioni_Utente = Nothing,
                                     Optional ByVal ElencoLayerVisibili As LayerVisibiliUtenteTipologia = Nothing)

        Dim xRead As New GIS_LayerElementiGrafici_R
        'Dim elencoLayerVisibili As New List(Of Integer)

        'If layerElementiGrafici_cod IsNot Nothing AndAlso layerElementiGrafici_cod.Count > 0 Then
        '    elencoLayerVisibili.AddRange(layerElementiGrafici_cod)
        'End If

        'If cfgAlbero IsNot Nothing AndAlso elencoLayerVisibili.Count = 0 Then
        '    DT = xRead.Leggi(objParametri.PivaSuperUser,
        '                 objParametri.UtenteUsername,
        '                 0,
        '                 cfgAlbero.TipologiaLayer_Cod,
        '                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
        '                 "",
        '                 "",
        '                 objParametri)

        '    If DT Is Nothing Then
        '        Throw New Exception("Errore nella lettura della visibilità dei layer")
        '    End If

        '    For Each row In DT.Rows
        '        If Integer.Parse(row("Flag_Visibile")) = 1 Then
        '            elencoLayerVisibili.Add(Integer.Parse(row("LayerElementiGrafici_Cod")))
        '        End If
        '    Next
        'End If

        'If addWithToStart Then
        'StrSQL.Append("With ")
        'End If
        'StrSQL.AppendLine(" cte_entita As ( ")

        'StrSQL.AppendLine(" Declare @TEMPENTITA TABLE ( ")
        'StrSQL.AppendLine("    PivaSuperUser nvarchar(20) ")
        'StrSQL.AppendLine("  , Entita_Cod  int ")
        'StrSQL.AppendLine("  , Entita_GUID  varchar(50) ")
        'StrSQL.AppendLine("  , ParametriVisualizzazioneLayer nvarchar(max) ")
        'StrSQL.AppendLine("  , TipoEntita_Cod int ")
        'StrSQL.AppendLine("  , layerElementiGrafici_cod_Riproiettato int ")
        'StrSQL.AppendLine("  , Piva  nvarchar(20) ")
        'StrSQL.AppendLine("  , Sa_Cod int ")
        'StrSQL.AppendLine("  , Appezza int ")
        'StrSQL.AppendLine("  , Campo_Cod int ")
        'StrSQL.AppendLine("  , Id_Imp int ")
        'StrSQL.AppendLine("  , PROV nvarchar(20) ")
        'StrSQL.AppendLine("  , COM  nvarchar(200) ")
        'StrSQL.AppendLine("  , SEZIONE nvarchar(10) ")
        'StrSQL.AppendLine("  , FOGLIO int ")
        'StrSQL.AppendLine("  , NUMERO int ")
        'StrSQL.AppendLine("  , SUBALTERNO nvarchar(20) ")
        'StrSQL.AppendLine("  , Programmazione_Entita_Cod int ")
        'StrSQL.AppendLine("  , ID_Agenda int ")
        'StrSQL.AppendLine("  , lav_cod int ")
        'StrSQL.AppendLine("  , Ricetta_Operazione_cod int ")
        'StrSQL.AppendLine("  , analisi_testata_cod int ")
        'StrSQL.AppendLine("  , analisi_campione_cod int ")
        'StrSQL.AppendLine("  , OLDGrafica_ID nvarchar(200) ")
        'StrSQL.AppendLine("  , inviato smallint ")
        'StrSQL.AppendLine("  , datainvio datetime ")
        'StrSQL.AppendLine("  , Data_Creazione datetime ")
        'StrSQL.AppendLine("  , Data_Modifica datetime ")
        'StrSQL.AppendLine("  , Username_Creazione nvarchar(200) ")
        'StrSQL.AppendLine("  , Username_Modifica nvarchar(200) ")
        'StrSQL.AppendLine("  , Validita_Inizio datetime ")
        'StrSQL.AppendLine("  , Validita_Fine datetime ")
        'StrSQL.AppendLine("  , programmazione_cod int ")
        'StrSQL.AppendLine("  , id_mov_det nvarchar(200) ")
        'StrSQL.AppendLine("  , Part_Cod int ")
        'StrSQL.AppendLine("  , ETICHETTA nvarchar(300) ")
        'StrSQL.AppendLine("  , id_cod int ")
        'StrSQL.AppendLine("  , Rilievo_Cod int ")
        'StrSQL.AppendLine("  , Area_Cod int ")
        'StrSQL.AppendLine("  , Testo nvarchar(500) ")
        'StrSQL.AppendLine(" ); ")

        StrSQL.AppendLine(" Create TABLE #TEMPENTITA ( ")
        StrSQL.AppendLine("     PivaSuperUser nvarchar(20) COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , Entita_Cod  int ")
        StrSQL.AppendLine("   , Entita_GUID  nvarchar(50)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , ParametriVisualizzazioneLayer nvarchar(max)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , TipoEntita_Cod int ")
        StrSQL.AppendLine("   , layerElementiGrafici_cod_Riproiettato int ")
        StrSQL.AppendLine("   , Piva  nvarchar(20)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , Sa_Cod int ")
        StrSQL.AppendLine("   , Appezza int ")
        StrSQL.AppendLine("   , Campo_Cod int ")
        StrSQL.AppendLine("   , Id_Imp int ")
        StrSQL.AppendLine("   , PROV nvarchar(10)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , COM  nvarchar(10)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , SEZIONE nvarchar(2)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , FOGLIO int ")
        StrSQL.AppendLine("   , NUMERO int ")
        StrSQL.AppendLine("   , SUBALTERNO nvarchar(5)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , Programmazione_Entita_Cod int ")
        StrSQL.AppendLine("   , ID_Agenda int ")
        StrSQL.AppendLine("   , lav_cod int ")
        StrSQL.AppendLine("   , Ricetta_Operazione_cod int ")
        StrSQL.AppendLine("   , analisi_testata_cod int ")
        StrSQL.AppendLine("   , analisi_campione_cod int ")
        StrSQL.AppendLine("   , OLDGrafica_ID nvarchar(200)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , inviato smallint ")
        StrSQL.AppendLine("   , datainvio datetime ")
        StrSQL.AppendLine("   , Data_Creazione datetime ")
        StrSQL.AppendLine("   , Data_Modifica datetime ")
        StrSQL.AppendLine("   , Username_Creazione nvarchar(200)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , Username_Modifica nvarchar(200)  COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , Validita_Inizio datetime ")
        StrSQL.AppendLine("   , Validita_Fine datetime ")
        StrSQL.AppendLine("   , programmazione_cod int ")
        StrSQL.AppendLine("   , id_mov_det int ")
        StrSQL.AppendLine("   , Part_Cod int ")
        StrSQL.AppendLine("   , ETICHETTA nvarchar(1000)   COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("   , id_cod int ")
        StrSQL.AppendLine("   , Rilievo_Cod int ")
        StrSQL.AppendLine("   , Area_Cod int ")
        StrSQL.AppendLine("   , Fabbricato_Cod int ")
        StrSQL.AppendLine("   , Testo nvarchar(500)   COLLATE SQL_Latin1_General_CP850_CI_AS ")
        StrSQL.AppendLine("  ); ")
        StrSQL.AppendLine("  CREATE CLUSTERED INDEX ix_TEMPENTITA ON #TEMPENTITA ([PivasuperUser],[Entita_Cod]); ")
        StrSQL.AppendLine("")

        'LAVEZ - 21/11/2023 - SE ho una bounding box creo una cte già filtrata da usare nelle singole query per tipologia entita in modo da minimizzare le operazioni di ser\deser
        If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
            StrSQL.AppendLine("with cte_entita as( ")
            StrSQL.AppendLine("	select ")
            StrSQL.AppendLine("		e.PivaSuperUser ")
            StrSQL.AppendLine("		, e.Entita_Cod ")
            StrSQL.AppendLine("		, e.Entita_GUID ")
            StrSQL.AppendLine("		, e.ParametriVisualizzazioneLayer ")
            StrSQL.AppendLine("		, e.TipoEntita_Cod ")
            StrSQL.AppendLine("		, e.Piva  ")
            StrSQL.AppendLine("		, e.Sa_Cod  ")
            StrSQL.AppendLine("		, e.Appezza  ")
            StrSQL.AppendLine("		, e.Campo_Cod  ")
            StrSQL.AppendLine("		, e.Id_Imp  ")
            StrSQL.AppendLine("		, e.PROV  ")
            StrSQL.AppendLine("		, e.COM  ")
            StrSQL.AppendLine("		, e.SEZIONE ")
            StrSQL.AppendLine("		, e.FOGLIO  ")
            StrSQL.AppendLine("		, e.NUMERO  ")
            StrSQL.AppendLine("		, e.SUBALTERNO ")
            StrSQL.AppendLine("		, e.Programmazione_Entita_Cod  ")
            StrSQL.AppendLine("		, e.ID_Agenda  ")
            StrSQL.AppendLine("		, e.Ricetta_Operazione_cod ")
            StrSQL.AppendLine("		, e.analisi_campione_cod  ")
            StrSQL.AppendLine("		, e.OLDGrafica_ID  ")
            StrSQL.AppendLine("		, e.inviato  ")
            StrSQL.AppendLine("		, e.datainvio  ")
            StrSQL.AppendLine("		, e.Data_Creazione ")
            StrSQL.AppendLine("		, e.Data_Modifica  ")
            StrSQL.AppendLine("		, e.Username_Creazione  ")
            StrSQL.AppendLine("		, e.Username_Modifica  ")
            StrSQL.AppendLine("		, e.programmazione_cod  ")
            StrSQL.AppendLine("		, e.id_mov_det ")
            StrSQL.AppendLine("		, e.Area_Cod ")
            StrSQL.AppendLine("		, e.Validita_Inizio  ")
            StrSQL.AppendLine("		, e.Validita_Fine  ")
            StrSQL.AppendLine("		, e.Fabbricato_Cod  ")
            StrSQL.AppendLine("	    ,f.ElementoGrafico_Cod ")
            StrSQL.AppendLine("		from ")
            StrSQL.AppendLine("		GIS_Entita e inner join GIS_ElementiGrafici f on  ")
            StrSQL.AppendLine("		(e.PivaSuperUser=f.PivaSuperUser and e.Entita_Cod=f.Entita_Cod)  ")
            StrSQL.AppendLine("	where  ")
            StrSQL.AppendLine("		f.Poligono_GeoEntity.STIntersects(  ")
            StrSQL.AppendLine("		 geography::STGeomFromText(  ")
            StrSQL.AppendLine("		  '" & wktBoundaySTIntersects & "'  ")
            StrSQL.AppendLine("		  , 4326) ")
            StrSQL.AppendLine("	  ) = 1  ")
            StrSQL.AppendLine(")  ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" , cte_GIS_TipoEntita as ( ")
        Else
            StrSQL.AppendLine(" with cte_GIS_TipoEntita as ( ")
        End If



        StrSQL.AppendLine("      Select ")
        StrSQL.AppendLine("          TipoEntita_Cod ")
        StrSQL.AppendLine("      From ")
        StrSQL.AppendLine("          GIS_TipoEntita ")
        StrSQL.AppendLine("      Where ")
        StrSQL.AppendLine("          LayerElementiGrafici_Cod = 19 ")
        StrSQL.AppendLine(" ) ")

        StrSQL.AppendLine("")

        'StrSQL.AppendLine("INSERT INTO @TEMPENTITA")

        StrSQL.AppendLine("INSERT INTO #TEMPENTITA")

        StrSQL.AppendLine("SELECT xyz.* FROM (")

        If cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                        ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.IMPIANTI).Count > 0
                                        ) _
                                OrElse cfgAlbero.TipologiaLayer_Cod <> enum_TipologiaLayer.Std Then

            StrSQL.AppendLine("--IMPIANTI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, id_cod, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Centri_Aziendali).Count > 0
                                        )) Then
            StrSQL.AppendLine("--CENTRI_AZIENDALI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Centri_Aziendali, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CATASTO).Count > 0
                                        ) _
                                 OrElse cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.SpecieVegetale _
                                 OrElse cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Cultivar) Then

            StrSQL.AppendLine("--CATASTO")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.CATASTO, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.TEMALIBERO).Count > 0
                                        )) Then
            StrSQL.AppendLine("--TEMA_LIBERO")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.TEMALIBERO, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--CAMPI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--ETTARI_EQUIVALENTI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE).Count > 0
                                        )) Then
            StrSQL.AppendLine("--AREE_OMOGENEE")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--UTILIZZO_SUOLI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita).Count > 0
                                        )) Then
            StrSQL.AppendLine("--IMPIANTI_PIANIFICATI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, id_cod, objParametri, enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--APPEZZAMENTI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Op_Agenda).Count > 0
                                        )) Then
            StrSQL.AppendLine("--OP_AGENDA")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Op_Agenda, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        Dim tipologieLayerAgenda_DettaglioRicetta = {enum_TipologiaLayer.Avversita,
                                                     enum_TipologiaLayer.Fenologia,
                                                     enum_TipologiaLayer.RilieviVegetoProduttivi,
                                                     enum_TipologiaLayer.AnalisiPeriodicitaAgenda}

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI).Count > 0
                                        ) _
                                        OrElse tipologieLayerAgenda_DettaglioRicetta.Contains(cfgAlbero.TipologiaLayer_Cod)
            ) Then
            StrSQL.AppendLine("--AGENDA_RILIEVI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL, impostazioniUtente:=impostazioneUtente)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione).Count > 0
                                        )) Then

            StrSQL.AppendLine("--MAPPE_PRESCRIZIONE")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Precision).Count > 0
                                        )) Then

            StrSQL.AppendLine("--PRECISION_FARMING")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Precision, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming).Count > 0
                                        )) Then

            StrSQL.AppendLine("--DETTAGLI_PRECISION_FARMING")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--CAMPIONI_ANALISI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI).Count > 0
                                        )) Then
            StrSQL.AppendLine("--CAMPIONAMENTI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If Not String.IsNullOrEmpty(ParametriFiltroXSessione) And
                (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                              ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Trattori).Count > 0
                                             ) _
                                      OrElse cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Percorsi
                ) Then

            StrSQL.AppendLine("--TRATTORI")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Trattori, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta).Count > 0
                                        ) _
                                 OrElse tipologieLayerAgenda_DettaglioRicetta.Contains(cfgAlbero.TipologiaLayer_Cod)
            ) Then
            StrSQL.AppendLine("--DETTAGLIO_RICETTA")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.MUZ).Count > 0
                                        )) Then
            StrSQL.AppendLine("--MUZ")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.MUZ, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Indici_Rischio_Produttivita).Count > 0
                                        )) Then
            StrSQL.AppendLine("--PRODUTTIVITA")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL, riproiettaImpianti:=enum_Gis_LayerElementiGrafici_std.Indici_Rischio_Produttivita)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Indice_Erosione).Count > 0
                                        )) Then
            StrSQL.AppendLine("--INDICE EROSIONE")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL, riproiettaImpianti:=enum_Gis_LayerElementiGrafici_std.Indice_Erosione)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Indice_CO2).Count > 0
                                        )) Then
            StrSQL.AppendLine("--INDICE CO2")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL, riproiettaImpianti:=enum_Gis_LayerElementiGrafici_std.Indice_CO2)
            StrSQL.AppendLine(" union all ")
        End If

        If (cfgAlbero Is Nothing OrElse (cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std And
                                         ElencoLayerVisibili.elencoLayers.Where(Function(x) x.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Indice_Rischio_Meteo).Count > 0
                                        )) Then
            StrSQL.AppendLine("--INDICE RISCHIO METEO")
            internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, enum_Gis_LayerElementiGrafici_std.IMPIANTI, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL, riproiettaImpianti:=enum_Gis_LayerElementiGrafici_std.Indice_Rischio_Meteo)
            StrSQL.AppendLine(" union all ")
        End If

        'Lavez - 24/07/2024 - fix per chiamate da app che non passano il vettore layerElementiGrafici_cod
        If layerElementiGrafici_cod IsNot Nothing Then
            'Lavez - 18/07/2024 - nel caso provenga da agenda ho scelto solo il layer degli impianti da dover estrarre
            If Not (layerElementiGrafici_cod.Count = 1 AndAlso layerElementiGrafici_cod(0) = enum_Gis_LayerElementiGrafici_std.IMPIANTI) Then
                StrSQL.AppendLine("--ALTRO")
                If (cfgAlbero Is Nothing OrElse cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std) Then
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, -1, False, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
                Else
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, -1, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
                End If
            Else
                StrSQL = StrSQL.Remove(StrSQL.Length - 12, 11)
            End If
        Else
            StrSQL.AppendLine("--ALTRO")
            If (cfgAlbero Is Nothing OrElse cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Std) Then
                internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, -1, False, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            Else
                internal_SQL_GisEntitaConFiltroTemporaleGetQuery(piva, sa_cod, 0, objParametri, -1, UtentiVisibilitaAppoggioApplica, wktBoundaySTIntersects, cfgAlbero, includiJoinTabelleCatasto, StrSQL)
            End If
        End If


        'StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
        StrSQL.AppendLine(") XYZ")

        'lavez 17/11/2023 -
        'If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
        '    StrSQL.AppendLine("        inner Join GIS_ElementiGrafici GeoDataIntersezione")
        '    StrSQL.AppendLine("            On GeoDataIntersezione.Entita_Cod = XYZ.Entita_Cod")
        '    StrSQL.AppendLine("")
        '    StrSQL.AppendLine("   WHERE")
        '    StrSQL.AppendLine("")
        '    StrSQL.AppendLine("  GeoDataIntersezione.Poligono_GeoEntity.STIntersects ( ")
        '    StrSQL.AppendLine("     geography::STGeomFromText(")
        '    StrSQL.AppendLine("         ")
        '    StrSQL.Append("'")
        '    StrSQL.Append(wktBoundaySTIntersects)
        '    StrSQL.Append("'")
        '    StrSQL.AppendLine("      , 4326)")
        '    StrSQL.AppendLine(") = 1")
        'End If


        StrSQL.AppendLine(";")



    End Sub

    Private Sub internal_SQL_GisEntitaConFiltroTemporaleGetQuery(
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal id_Cod As Integer,
        ByVal objParametri As AgronicaCoreParametri,
        ByVal LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
        ByVal UtentiVisibilitaAppoggioApplica As Boolean,
        ByVal wktBoundaySTIntersects As String,
        ByVal cfgAlbero As ConfigurazioneAlbero,
        ByVal includiJoinTabelleCatasto As Boolean,
        ByRef StrSQL As StringBuilder,
        Optional ByVal ParametriFiltroXSessione As String = "",
        Optional ByVal impostazioniUtente As Impostazioni_Utente = Nothing,
        Optional ByVal riproiettaImpianti As Int32 = 0)

        Dim TabellaAliasDateValidita As String = "e"

        If String.IsNullOrEmpty(ParametriFiltroXSessione) And LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.Trattori Then
            Exit Sub
        End If

        Dim etichetta As String = "''"

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.CATASTO
                If includiJoinTabelleCatasto Then
                    TabellaAliasDateValidita = "pp"
                End If
                etichetta = "CONVERT(VARCHAR(10), e.FOGLIO) + ' - ' + CONVERT(VARCHAR(10), e.NUMERO)"

            Case enum_Gis_LayerElementiGrafici_std.TEMALIBERO
                If includiJoinTabelleCatasto Then
                    TabellaAliasDateValidita = "pc"
                End If

            Case enum_Gis_LayerElementiGrafici_std.Centri_Aziendali
                TabellaAliasDateValidita = "sa1"

            Case enum_Gis_LayerElementiGrafici_std.CAMPI
                TabellaAliasDateValidita = "cp"
                etichetta = "cp.Campo_Des"

            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                If riproiettaImpianti <> 0 Then
                    TabellaAliasDateValidita = "iai"
                    etichetta = "''"
                Else

                    TabellaAliasDateValidita = "reg1"
                    If IsFiltroTemporaleSuEsercizio() Then
                        TabellaAliasDateValidita = "ese"
                    End If
                    etichetta = String.Format("app1.APP_NOME + case when {0} then {1} when {2} then {3} else '' end",
                                              "cod_ana.descrizione IS NOT NULL",
                                              "' - ' + cod_ana.descrizione",
                                              "(spe1.veg_des IS NOT NULL) OR (var1.cul_des IS NOT NULL) ",
                                              "' - ' + coalesce(spe1.veg_des,'') + ' - ' + coalesce(var1.cul_des,'')")
                End If

            Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                TabellaAliasDateValidita = "ent"
                etichetta = "ent.Entita_Des"

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                TabellaAliasDateValidita = "app1"
                etichetta = "app1.APP_NOME"

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                TabellaAliasDateValidita = "d1"
                etichetta = GetEtichettaAgendaRilievi()

            Case enum_Gis_LayerElementiGrafici_std.Op_Agenda
                TabellaAliasDateValidita = "a2"
                etichetta = "a2.des_lib + ' - ' + CONVERT(varchar(100), A2.Validita_Inizio, 103)"

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI
                TabellaAliasDateValidita = "Camp1"
                etichetta = "camp1.Analisi_Campione_Des"

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI
                TabellaAliasDateValidita = "Camp1"
                etichetta = "camp1.Analisi_Campione_Des"

            Case enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta
                TabellaAliasDateValidita = "rOperaz"
                etichetta = "rOperaz.ricetta_operazione_des"

            Case enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI
                TabellaAliasDateValidita = "e"
                etichetta = "gel.elementografico_des"

            Case enum_Gis_LayerElementiGrafici_std.MUZ
                TabellaAliasDateValidita = "ao"
                etichetta = "ao.Area_Des"

        End Select

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                StrSQL.AppendLine(" select * ")
                StrSQL.AppendLine(" from ( ")

                '--------------------------------------------------------------------------------
                ' AGENDA RILIEVI CON DATI GIS
                '--------------------------------------------------------------------------------

                StrSQL.AppendLine("-- con dato GIS")

                'colonne da estrarre
                Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_SelectPart(
                    True,
                    False,
                    LayerElementiGrafici_Cod,
                    includiJoinTabelleCatasto,
                    StrSQL,
                    TabellaAliasDateValidita,
                    etichetta,
                    enum_GIS2012_TipoEntita.Nessuno,
                    0)

                'from
                StrSQL.AppendLine("  from " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " e ")
                StrSQL.AppendLine("  inner Join Agenda a1 on a1.piva = e.piva And ")
                StrSQL.AppendLine("                          a1.id_agenda = e.id_agenda ")
                StrSQL.AppendLine("  inner Join Appezzamento app1 on app1.piva = e.piva And ")
                StrSQL.AppendLine("                                  app1.sa_cod = e.sa_cod And ")
                StrSQL.AppendLine("                                  app1.appezza = e.appezza ")
                StrSQL.AppendLine("  inner Join GIS_TipoEntita t on e.tipoEntita_cod = t.tipoEntita_Cod ")

                StrSQL.AppendLine("  left Join Mov_Destinazioni d1 on d1.piva = e.piva And ")
                StrSQL.AppendLine("                                   d1.sa_cod = e.sa_cod And ")
                StrSQL.AppendLine("                                   d1.id_agenda = e.id_agenda And ")
                StrSQL.AppendLine("                                   d1.id_mov_det = e.id_mov_det And ")
                StrSQL.AppendLine("                                   d1.appezza = e.appezza And ")
                StrSQL.AppendLine("                                   d1.id_destinazione = e.id_imp ")

                LeftJoin_MovDettaglioTecnico_TabelleRilievi(StrSQL)

                'filtri con join
                If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, StrSQL)
                End If
                If UtentiVisibilitaAppoggioApplica Then
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, StrSQL)
                End If

                'prima condizione where 
                StrSQL.AppendLine("  where 1=1 ")

                If piva <> "" Then
                    StrSQL.AppendLine("    AND ")
                    StrSQL.AppendLine("    ( ")
                    StrSQL.AppendLine("      e.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                    If sa_Cod <> 0 Then
                        StrSQL.AppendLine("      AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                    End If
                    StrSQL.AppendLine("    ) ")
                End If

                Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, StrSQL, TabellaAliasDateValidita, False)


                If impostazioniUtente Is Nothing OrElse (impostazioniUtente IsNot Nothing AndAlso impostazioniUtente.UsaPuntoInterno) Then
                    '--------------------------------------------------------------------------------
                    ' AGENDA RILIEVI SENZA DATI GIS
                    '--------------------------------------------------------------------------------
                    ' NB: Si tratta di rilievi senza il dato GIS, per cui viene preso il poligono
                    '     dell'impianto
                    '--------------------------------------------------------------------------------

                    StrSQL.AppendLine(" union ")
                    StrSQL.AppendLine("-- senza dato GIS")

                    'colonne da estrarre
                    Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_SelectPart(
                        False,
                        False,
                        LayerElementiGrafici_Cod,
                        includiJoinTabelleCatasto,
                        StrSQL,
                        TabellaAliasDateValidita,
                        etichetta,
                        enum_GIS2012_TipoEntita.Destinazione_Agenda,
                        0)

                    'from
                    StrSQL.AppendLine("  From Mov_Destinazioni d1 ")
                    StrSQL.AppendLine("  inner Join Agenda a1 on a1.piva = d1.piva And ")
                    StrSQL.AppendLine("                          a1.id_agenda = d1.id_agenda And ")
                    AggiungiFiltroLavCodSenzaGis(StrSQL, "")
                    StrSQL.AppendLine("  inner join " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " e on d1.PIVA = e.Piva And ")
                    StrSQL.AppendLine("                             d1.SA_COD = e.Sa_Cod And ")
                    StrSQL.AppendLine("                             d1.APPEZZA = e.Appezza And ")
                    StrSQL.AppendLine("                             d1.id_destinazione = e.id_imp ")
                    StrSQL.AppendLine("  inner Join Appezzamento app1 on app1.PIVA = e.Piva And ")
                    StrSQL.AppendLine("                                  app1.SA_COD = e.Sa_Cod And ")
                    StrSQL.AppendLine("                                  app1.APPEZZA = e.Appezza ")
                    StrSQL.AppendLine("  inner join cte_GIS_TipoEntita tp on tp.TipoEntita_Cod = e.TipoEntita_Cod ")

                    LeftJoin_MovDettaglioTecnico_TabelleRilievi(StrSQL)

                    'filtri con join
                    If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then
                        internal_SQL_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, StrSQL)
                    End If
                    If UtentiVisibilitaAppoggioApplica Then
                        internal_SQL_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, StrSQL)
                    End If

                    'prima condizione where 
                    StrSQL.AppendLine("  where 1=1 ")

                    'filtro piva + centro aziendale
                    If piva <> "" Then
                        StrSQL.AppendLine("    and ( ")
                        StrSQL.AppendLine("          e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.AppendLine("          AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        StrSQL.AppendLine("        ) ")
                    End If

                    'non selezionati nella porzione di query "con dato GIS"
                    StrSQL.AppendLine("  and not exists ( ")
                    StrSQL.AppendLine("  select 1 ")
                    StrSQL.AppendLine("  from " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " e63 ")
                    StrSQL.AppendLine("  where e63.id_mov_det = d1.id_mov_det ")
                    StrSQL.AppendLine("                     And d1.PIVA = e63.Piva ")
                    StrSQL.AppendLine("                     And d1.SA_COD = e63.Sa_Cod ")
                    StrSQL.AppendLine("                     And d1.APPEZZA = e63.Appezza ")
                    StrSQL.AppendLine("                     And d1.id_destinazione = e63.id_imp ")
                    StrSQL.AppendLine("                     And d1.id_agenda = e63.id_agenda ")
                    StrSQL.AppendLine(" )")

                    Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, StrSQL, TabellaAliasDateValidita, False)

                    SeAggiungiFiltroUltimaData(StrSQL, objParametri)

                End If

                StrSQL.AppendLine(" ) e63 ")

                '--------------------------------------------------------------------------------
                ' AVVERSITA
                '--------------------------------------------------------------------------------
                ' TODO: usare le funzioni per comporre
                '--------------------------------------------------------------------------------

                'vanni, 07/09/2023: sulla tipologia dei rilievi vegeto-produttivi non estraggo la union con le avversità.
                '                   questo risolve l'errore: Impossibile associare l'identificatore in più parti "mdt.FF_Classe".
                If cfgAlbero.TipologiaLayer_Cod <> enum_TipologiaLayer.RilieviVegetoProduttivi Then

                    StrSQL.AppendLine("     union ")
                    StrSQL.AppendLine("  -- avversita ")

                    'colonne da estrarre
                    Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_SelectPart(
                        False,
                        True,
                        LayerElementiGrafici_Cod,
                        includiJoinTabelleCatasto,
                        StrSQL,
                        "p",
                        "app1.APP_NOME + ' - ' + isnull(avv.Av_Des_Vol,'')",
                        enum_GIS2012_TipoEntita.Destinazione_Agenda,
                        0)

                    'from
                    StrSQL.AppendLine("     from " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " e ")
                    StrSQL.AppendLine("    inner Join Appezzamento app1 on app1.PIVA = e.Piva And ")
                    StrSQL.AppendLine("                                    app1.SA_COD = e.Sa_Cod And ")
                    StrSQL.AppendLine("                                    app1.APPEZZA = e.Appezza ")
                    StrSQL.AppendLine("    inner join cte_GIS_TipoEntita tp on tp.TipoEntita_Cod = e.TipoEntita_Cod ")
                    StrSQL.AppendLine("    inner join [dbo].[DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo] m ")
                    StrSQL.AppendLine("  				on e.piva = m.piva ")
                    StrSQL.AppendLine("  				and e.SA_COD = m.sa_cod ")
                    StrSQL.AppendLine("  				and e.APPEZZA = m.appezza ")
                    StrSQL.AppendLine("  				and e.Id_Imp = m.id_reg ")
                    StrSQL.AppendLine("  inner join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV p ")
                    StrSQL.AppendLine("  on m.tipo_sorgente = p.tipo_sorgente ")
                    StrSQL.AppendLine("  and m.Stazione_Cod = p.Stazione_Cod ")
                    StrSQL.AppendLine("  Left Join Avversita avv on avv.Av_Cod = p.av_cod ")
                    StrSQL.AppendLine("    where 1=1 ")

                    Dim datiFiltroTemporale = GetDatiFiltroTemporale(LayerElementiGrafici_Cod, objParametri)

                    Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, StrSQL, "p", True)

                End If

            Case Else

                Dim LayerRiproiezione = IIf(riproiettaImpianti <> 0, riproiettaImpianti, enum_GIS2012_TipoEntita.Nessuno)

                Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_SelectPart(False, False, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, StrSQL, TabellaAliasDateValidita, etichetta, LayerRiproiezione, IIf(LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.IMPIANTI, id_Cod, 0))

                StrSQL.AppendLine(" From " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " e ")
                StrSQL.AppendLine()

                If Not cfgAlbero Is Nothing AndAlso cfgAlbero.FiltroImpiantiIdTestataTemp > 0 Then
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod, cfgAlbero, StrSQL)
                End If

        End Select

        'altre letture... 
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI
                StrSQL.AppendLine("inner join gis_elementigrafici gel on gel.entita_cod = e.entita_cod ")

            Case enum_Gis_LayerElementiGrafici_std.MUZ
                StrSQL.AppendLine("     inner Join Area_Omogenea_MUZ ao on   ")
                StrSQL.AppendLine("         ao.Area_Cod = e.Area_Cod ")

            Case enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE
                'Nessuna join richiesta

            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO
                If includiJoinTabelleCatasto Then
                    If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
                        StrSQL.AppendLine("inner Join  ( ")
                        'StrSQL.AppendLine(" Select * ")
                        'StrSQL.AppendLine("     From ( ")
                        StrSQL.AppendLine("        Select ")
                        StrSQL.AppendLine("             a.Prov, ")
                        StrSQL.AppendLine("             a.COM, ")
                        StrSQL.AppendLine("             a.SEZIONE, ")
                        StrSQL.AppendLine("             a.FOGLIO, ")
                        StrSQL.AppendLine("             a.NUMERO, ")
                        StrSQL.AppendLine("             a.SUBALTERNO, ")
                        StrSQL.AppendLine("             Case when b.PART_COD Is null then 0 else b.PART_COD end as PART_COD, ")
                        StrSQL.AppendLine("             '' as Piva, ")
                        StrSQL.AppendLine("             0 as Sa_Cod, ")
                        StrSQL.AppendLine("             a.TipoEntita_Cod, ")
                        StrSQL.AppendLine("             a.Validita_Inizio, ")
                        StrSQL.AppendLine("             a.Validita_Fine ")
                        StrSQL.AppendLine("        From " & IIf(String.IsNullOrEmpty(wktBoundaySTIntersects), "gis_entita", "cte_entita") & " a ")
                        StrSQL.AppendLine("        inner Join particellecatastali b ")
                        StrSQL.AppendLine("        On a.Prov=b.PROV ")
                        StrSQL.AppendLine("           And a.COM=b.COM ")
                        StrSQL.AppendLine("           And a.SEZIONE=b.SEZIONE ")
                        StrSQL.AppendLine("           And a.FOGLIO=b.FOGLIO ")
                        StrSQL.AppendLine("           And a.NUMERO=b.NUMERO ")
                        StrSQL.AppendLine("           And a.SUBALTERNO=b.SUBALTERNO ")
                        StrSQL.AppendLine("        Where a.TipoEntita_Cod = " & enum_GIS2012_TipoEntita.CATASTO)
                        'StrSQL.AppendLine(" ) p  ")
                        'StrSQL.AppendLine("     inner Join(select '" & piva & "' as piva, " & sa_Cod & " as sa_cod) fintaImpresa ")
                        'StrSQL.AppendLine(" On 1=1 ")
                        StrSQL.AppendLine(" ) pp")
                    Else
                        If UtentiVisibilitaAppoggioApplica Then
                            StrSQL.AppendLine(" inner Join ( ")
                            StrSQL.AppendLine("     select pp.* ")
                            StrSQL.AppendLine("     From ImpreseXParticelle pp ")
                            StrSQL.AppendLine(" inner join Utenti_Visibilita_Appoggio visParticelle (NOLOCK) ")
                            StrSQL.AppendLine(" on  visParticelle.piva = pp.piva  ")
                            StrSQL.AppendLine(" and visParticelle.sa_Cod = pp.sa_cod")
                            StrSQL.AppendLine(" and visParticelle.Entita_Cod = 2 ")
                            StrSQL.AppendLine(" and visParticelle.pivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                            StrSQL.AppendLine(" and visParticelle.UserName = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
                            StrSQL.AppendLine(" and visParticelle.Appezza = 0 ")
                            StrSQL.AppendLine(" and visParticelle.id_reg = 0 ")
                            StrSQL.AppendLine(" ) pp")
                        Else
                            StrSQL.Append("    inner join ImpreseXParticelle pp " & vbCrLf)
                        End If
                    End If

                    StrSQL.Append("        on pp.prov = e.prov " & vbCrLf)
                    StrSQL.Append("        and pp.com = e.COM  " & vbCrLf)
                    StrSQL.Append("        and pp.SEZIONE = e.SEZIONE  " & vbCrLf)
                    StrSQL.Append("        and pp.Foglio = e.Foglio  " & vbCrLf)
                    StrSQL.Append("        and pp.NUMERO = e.NUMERO  " & vbCrLf)
                    StrSQL.Append("        and pp.SUBALTERNO = e.SUBALTERNO " & vbCrLf)
                    StrSQL.Append("    inner join ParticelleCatastali pc " & vbCrLf)
                    StrSQL.Append("        on  pc.prov = e.prov  " & vbCrLf)
                    StrSQL.Append("        and pc.com = e.COM   " & vbCrLf)
                    StrSQL.Append("        and pc.SEZIONE = e.SEZIONE   " & vbCrLf)
                    StrSQL.Append("        and pc.Foglio = e.Foglio   " & vbCrLf)
                    StrSQL.Append("        and pc.NUMERO = e.NUMERO   " & vbCrLf)
                    StrSQL.Append("        and pc.SUBALTERNO = e.SUBALTERNO " & vbCrLf)
                End If

            Case enum_Gis_LayerElementiGrafici_std.Centri_Aziendali
                StrSQL.AppendLine("     inner Join Centri_Aziendali sa1 on   ")
                StrSQL.AppendLine("                 sa1.PIVA = e.Piva ")
                StrSQL.AppendLine("         And sa1.SA_COD = e.Sa_Cod    ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPI
                StrSQL.AppendLine("  inner join Campi cp on ")
                StrSQL.AppendLine("                 e.PIVA = cp.Piva ")
                StrSQL.AppendLine("         And e.SA_COD = cp.Sa_Cod    ")
                StrSQL.AppendLine("         And e.Campo_cod = cp.campo_cod ")

            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                If riproiettaImpianti <> 0 Then
                    StrSQL.AppendLine("    INNER JOIN Reg_Impianti_XIndiciProduttivitaAi indpr ON indpr.PIVA = e.Piva AND ")
                    StrSQL.AppendLine("                                                           indpr.SA_COD = e.Sa_Cod AND ")
                    StrSQL.AppendLine("                                                           indpr.APPEZZA = e.Appezza AND ")
                    StrSQL.AppendLine("                                                           indpr.ID_REG = e.id_imp ")

                    StrSQL.AppendLine("    INNER JOIN IndiciProduttivitaAi iai ON iai.IndiciProduttivitaAi_COD = indpr.IndiciProduttivitaAi_COD ")

                    StrSQL.AppendLine("    INNER JOIN Imprese impxi ON impxi.PIVA = e.Piva ")

                    'Aggiungere JOIN alla tabella Imprese con Piva, leggere rag_soc SOSTITUIRE PARAGRAFO E PIPE CON _
                    'Impostare in "Text" chiamandolo "Impresa"
                Else
                    StrSQL.AppendLine("    INNER JOIN reg_impianti reg1 ON reg1.PIVA = e.Piva AND ")
                    StrSQL.AppendLine("                                    reg1.SA_COD = e.Sa_Cod AND ")
                    StrSQL.AppendLine("                                    reg1.APPEZZA = e.Appezza AND ")
                    StrSQL.AppendLine("                                    reg1.ID_REG = e.id_imp ")
                    'Join esercizi
                    StrSQL.AppendLine("    INNER JOIN imprese_progetti ese on ese.PIVA = e.Piva AND ")
                    StrSQL.AppendLine("                                       ese.SA_COD = e.Sa_Cod AND ")
                    StrSQL.AppendLine("                                       ese.APPEZZA = e.Appezza AND ")
                    StrSQL.AppendLine("                                       ese.ID_REG = e.id_imp ")
                    'Join specie/varietà
                    StrSQL.AppendLine("    LEFT JOIN Cultivar var1 ON var1.cul_cod = reg1.cul_cod ")
                    StrSQL.AppendLine("    LEFT JOIN SpecieVegetali spe1 ON spe1.veg_cod = var1.veg_cod ")
                    'Join destinazione d'uso
                    StrSQL.AppendLine("    LEFT JOIN reg_impianti_codici reg_cod on reg_cod.PIVA = e.Piva AND ")
                    StrSQL.AppendLine("                                             reg_cod.SA_COD = e.Sa_Cod AND ")
                    StrSQL.AppendLine("                                             reg_cod.APPEZZA = e.Appezza AND ")
                    StrSQL.AppendLine("                                             reg_cod.ID_REG = e.id_imp AND ")
                    StrSQL.AppendLine("                                             reg_cod.id_cod > 3000 AND reg_cod.id_cod <= 4000 ")
                    StrSQL.AppendLine("    LEFT JOIN Codici_Anagrafe cod_ana ON cod_ana.codice = reg_cod.id_cod ")
                    'Join appezzamento
                    StrSQL.AppendLine()
                    StrSQL.AppendLine("    INNER JOIN Appezzamento app1 ON app1.PIVA = e.Piva AND ")
                    StrSQL.AppendLine("                                    app1.SA_COD = e.Sa_Cod AND ")
                    StrSQL.AppendLine("                                    app1.APPEZZA = e.Appezza ")
                End If

            Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                StrSQL.AppendLine("     inner Join programmazione_entita ent   ")
                StrSQL.AppendLine("         on ent.programmazione_entita_cod = e.Programmazione_Entita_COD   ")

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                StrSQL.AppendLine("     inner Join Appezzamento app1 on   ")
                StrSQL.AppendLine("                 app1.PIVA = e.Piva ")
                StrSQL.AppendLine("         And app1.SA_COD = e.Sa_Cod    ")
                StrSQL.AppendLine("         And app1.APPEZZA = e.Appezza    ")
                StrSQL.AppendLine("  ")

            Case enum_Gis_LayerElementiGrafici_std.Op_Agenda
                StrSQL.AppendLine(" inner Join Agenda a2 on  ")
                StrSQL.AppendLine("     a2.piva = e.piva ")
                StrSQL.AppendLine("     And a2.id_agenda = e.id_agenda ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI
                StrSQL.AppendLine(" inner Join Analisi_campioni Camp1 on  ")
                StrSQL.AppendLine("     camp1.Analisi_SuperUser = e.PivaSuperUser ")
                StrSQL.AppendLine("     And camp1.analisi_campione_Cod = e.analisi_campione_Cod  ")
                StrSQL.AppendLine(" inner join Analisi_CampionixDettagli acd ")
                StrSQL.AppendLine(" on acd.analisi_campione_Cod = camp1.analisi_campione_Cod  ")

            Case enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI
                StrSQL.AppendLine(" inner Join Analisi_campioni Camp1 on  ")
                StrSQL.AppendLine("     camp1.Analisi_SuperUser = e.PivaSuperUser ")
                StrSQL.AppendLine("     And camp1.analisi_campione_Cod = e.analisi_campione_Cod  ")
                StrSQL.AppendLine(" inner join Analisi_CampionixDettagli acd ")
                StrSQL.AppendLine(" on acd.analisi_campione_Cod = camp1.analisi_campione_Cod  ")

            Case enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta
                StrSQL.AppendLine("  inner Join Ricette_Operazioni rOperaz  ")
                StrSQL.AppendLine("         On rOperaz.Ricetta_Operazione_cod = e.Ricetta_Operazione_cod  ")

            Case enum_Gis_LayerElementiGrafici_std.Trattori
                StrSQL.AppendLine("  inner Join Gis_elementiGrafici geodata  ")
                StrSQL.AppendLine("         On geodata.PivaSuperUser = e.PivaSuperUser and geodata.Entita_Cod = e.Entita_Cod  ")

            Case Else
                TabellaAliasDateValidita = "e"

        End Select

        If LayerElementiGrafici_Cod <> enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI Then
            StrSQL.AppendLine("  inner Join GIS_TipoEntita t ")
            StrSQL.AppendLine("         On e.tipoEntita_cod = t.tipoEntita_Cod ")
            StrSQL.AppendLine("  ")
        End If


        '' VAnni: 6/4/2018: il filtro su Utenti_Visibilita_Appoggio ora è qui..
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO,
                 enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                'nessun join richiesto poichè già gestito prima

            Case Else
                If UtentiVisibilitaAppoggioApplica Then
                    internal_SQL_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri, StrSQL)
                End If

        End Select

        'condizioni where...
        If LayerElementiGrafici_Cod <> -1 Then
            Select Case LayerElementiGrafici_Cod
                Case enum_Gis_LayerElementiGrafici_std.Trattori
                    StrSQL.AppendLine(" where geodata.LayerElementiGrafici_Cod =  " & LayerElementiGrafici_Cod)
                    ' VAnni: 8/3/2018: filtro dei percorsi..:                        
                    If Not String.IsNullOrEmpty(ParametriFiltroXSessione) Then
                        StrSQL.AppendLine(" AND (" & ParametriFiltroXSessione & ")")
                    End If

                Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI

                Case Else
                    StrSQL.AppendLine(" where t.LayerElementiGrafici_Cod = " & LayerElementiGrafici_Cod)
            End Select

            If piva <> "" Then
                'verifico piva e sa_cod laddove necessario.
                Select Case LayerElementiGrafici_Cod

                    Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                        enum_Gis_LayerElementiGrafici_std.TEMALIBERO
                        StrSQL.Append(" And " & vbCrLf)
                        StrSQL.Append("( " & vbCrLf)
                        StrSQL.Append("            pp.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.Append("        AND pp.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        StrSQL.Append("    ) ")

                    Case enum_Gis_LayerElementiGrafici_std.Trattori,
                         enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                        'non deve fare nulla, nè passare al case else...

                    Case enum_Gis_LayerElementiGrafici_std.Op_Agenda,
                         enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI
                        StrSQL.Append("    AND  " & vbCrLf)
                        StrSQL.Append("    ( " & vbCrLf)
                        StrSQL.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.Append("        AND e.Sa_Cod in (0," & Agro_SQL_SaveNum(sa_Cod) & ") ")
                        End If
                        StrSQL.Append("    ) ")

                    Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                        StrSQL.Append("    AND  " & vbCrLf)
                        StrSQL.Append("    ( " & vbCrLf)
                        StrSQL.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        If id_Cod <> 0 Then
                            StrSQL.Append("        AND reg_cod.id_cod = " & Agro_SQL_SaveNum(id_Cod) & " ")
                        End If
                        StrSQL.Append("    ) ")

                    Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                        StrSQL.Append("    AND  " & vbCrLf)
                        StrSQL.Append("    ( " & vbCrLf)
                        StrSQL.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        If id_Cod <> 0 Then
                            StrSQL.Append("        AND ent.id_cod = " & Agro_SQL_SaveNum(id_Cod) & " ")
                        End If
                        StrSQL.Append("    ) ")

                    Case Else
                        StrSQL.Append("    AND  " & vbCrLf)
                        StrSQL.Append("    ( " & vbCrLf)
                        StrSQL.Append("            e.Piva = '" & Agro_SQL_SaveText(piva) & "'  ")
                        If sa_Cod <> 0 Then
                            StrSQL.Append("        AND e.Sa_Cod = " & Agro_SQL_SaveNum(sa_Cod) & " ")
                        End If
                        StrSQL.Append("    ) ")

                End Select
            End If
        Else
            'quanto già gestito in precedenza lo escludo...
            StrSQL.AppendLine(" where t.LayerElementiGrafici_Cod not in ( ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.UTILIZZODEISUOLI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.CATASTO & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.TEMALIBERO & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.ETTARI_EQUIVALENTI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.AREEOMOGENEE & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Centri_Aziendali & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.IMPIANTI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Op_Agenda & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Trattori & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Precision & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Dettagli_Precision_Farming & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Dettaglio_Ricetta & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.MUZ & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Indici_Rischio_Produttivita & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Indice_CO2 & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Indice_Erosione & ", ")
            StrSQL.AppendLine(enum_Gis_LayerElementiGrafici_std.Indice_Rischio_Meteo & " ")
            StrSQL.AppendLine(" ) and ( ")
            StrSQL.AppendLine("   (e.tipoentita_cod = 3 and  t.layerElementiGrafici_cod = 4 and e.username_creazione = '" & objParametri.UtenteCodFiscale & "'  ) ") 'testo
            StrSQL.AppendLine("  OR (e.tipoentita_cod not in (3, 5, 8, 12, 13, 17, 18, 19, 33, 23, 20, 19, 22, 21, 1, 50, 55, 51, 54, 67, 90, 91, 92, 93, 94 )  ) " & vbCrLf)  'nuova versione
            StrSQL.AppendLine(")")
        End If

        If LayerElementiGrafici_Cod <> enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI Then
            Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(objParametri, LayerElementiGrafici_Cod, includiJoinTabelleCatasto, StrSQL, TabellaAliasDateValidita, False)
        End If
    End Sub

    Private Sub LeftJoin_MovDettaglioTecnico_TabelleRilievi(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("  left Join Mov_Dettaglio_Tecnico mdt on mdt.piva = d1.piva And ")
        StrSQL.AppendLine("                                         mdt.sa_cod = d1.sa_cod And ")
        StrSQL.AppendLine("                                         mdt.id_agenda = d1.id_agenda And ")
        StrSQL.AppendLine("                                         mdt.id_mov = d1.id_mov And ")
        StrSQL.AppendLine("                                         mdt.id_mov_det = d1.id_mov_det ")
        StrSQL.AppendLine("  left Join Avversita avv on avv.Av_Cod = mdt.Av_Cod ")
        StrSQL.AppendLine("  left Join IndiciMaturita indmat on indmat.ind_mat_cod = mdt.FF_Classe ")
        StrSQL.AppendLine("  left Join UnitaMisura udmmat on udmmat.udm_cod = mdt.dett_cod ")
        StrSQL.AppendLine("  left Join SpecieVegetaliXStadiCrescita stadi on stadi.cod_SS = mdt.FF_Classe ")
        StrSQL.AppendLine("  left Join Stadi_Crescita_BBCH bbch on bbch.id_bbch = stadi.id_bbch ")
    End Sub

    Private Sub AggiungiFiltroLavCodSenzaGis(ByRef StrSQL As StringBuilder, prefissoAlias As String)
        Select Case True

            Case Not IsNothing(_cfgAlbero) AndAlso _cfgAlbero.TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Avversita
                StrSQL.AppendLine(String.Format("                          {0}a1.lav_cod = {1} ",
                                                        prefissoAlias,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO))

            Case Not IsNothing(_cfgAlbero) AndAlso _cfgAlbero.TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Fenologia
                StrSQL.AppendLine(String.Format("                          {0}a1.lav_cod = {1} ",
                                                        prefissoAlias,
                                                        CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE))

            Case Not IsNothing(_cfgAlbero) AndAlso _cfgAlbero.TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.RilieviVegetoProduttivi
                StrSQL.AppendLine(String.Format("                          {0}a1.lav_cod in ({1}, {2}) ",
                                                        prefissoAlias,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA))

            Case Else
                StrSQL.AppendLine(String.Format("                         {0}a1.lav_cod in ({1}, {2}, {3}, {4}) ",
                                                        prefissoAlias,
                                                        CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                                                        CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA))

        End Select
    End Sub

    Private Sub SeAggiungiFiltroUltimaData(ByRef StrSQL As StringBuilder, ByRef objParametri As AgronicaCoreParametri)

        If Not IsNothing(_cfgAlbero) AndAlso (
            _cfgAlbero.TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Avversita OrElse
            _cfgAlbero.TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.RilieviVegetoProduttivi) Then

            StrSQL.AppendLine(" and d1.validita_inizio = ( ")

            StrSQL.AppendLine(" select max(sd1.validita_inizio) ")
            StrSQL.AppendLine(" from mov_destinazioni sd1 ")
            StrSQL.AppendLine(" inner Join agenda sa1 on sa1.piva = sd1.piva and ")
            StrSQL.AppendLine("                          sa1.id_agenda = sd1.id_agenda and ")
            AggiungiFiltroLavCodSenzaGis(StrSQL, "s")
            StrSQL.AppendLine(" inner join gis_entita se on sd1.PIVA = se.Piva and ")
            StrSQL.AppendLine("                             sd1.SA_COD = se.Sa_Cod and ")
            StrSQL.AppendLine("                             sd1.APPEZZA = se.Appezza and ")
            StrSQL.AppendLine("                             sd1.id_destinazione = se.id_imp ")
            StrSQL.AppendLine(" inner join cte_GIS_TipoEntita stp on stp.TipoEntita_Cod = se.TipoEntita_Cod ")
            StrSQL.AppendLine(" inner Join Mov_Dettaglio_Tecnico smdt on smdt.piva = sd1.piva And ")
            StrSQL.AppendLine("                                          smdt.sa_cod = sd1.sa_cod And ")
            StrSQL.AppendLine("                                          smdt.id_agenda = sd1.id_agenda And ")
            StrSQL.AppendLine("                                          smdt.id_mov = sd1.id_mov And ")
            StrSQL.AppendLine("                                          smdt.id_mov_det = sd1.id_mov_det ")

            Select Case _cfgAlbero.TipologiaLayer_Cod

                Case TipiEnumerativi.enum_TipologiaLayer.Avversita
                    StrSQL.AppendLine(" inner join Avversita savv on savv.Av_Cod = smdt.Av_Cod ")

                Case TipiEnumerativi.enum_TipologiaLayer.RilieviVegetoProduttivi
                    StrSQL.AppendLine(" inner join IndiciMaturita sindmat on sindmat.ind_mat_cod = smdt.FF_Classe ")

            End Select

            StrSQL.AppendLine(" where se.Entita_Cod = e.entita_Cod ")

            Select Case _cfgAlbero.TipologiaLayer_Cod

                Case TipiEnumerativi.enum_TipologiaLayer.Avversita
                    StrSQL.AppendLine(" and savv.Av_Cod = avv.Av_Cod ")

                Case TipiEnumerativi.enum_TipologiaLayer.RilieviVegetoProduttivi
                    StrSQL.AppendLine(" and sindmat.ind_mat_cod = indmat.ind_mat_cod ")

            End Select

            Dim datiFiltroTemporale = GetDatiFiltroTemporale(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, objParametri)

            AggiungiFiltroTemporale(StrSQL, False, "sd1.Validita_Inizio", datiFiltroTemporale)

            StrSQL.AppendLine(" ) ")

        End If

    End Sub

    Private Function GetEtichettaAgendaRilievi() As String

        Const nomeAppezzamento = "app1.APP_NOME"
        Dim dataValidita As String = "CONVERT(varchar(100), d1.Validita_Inizio, 103)"

        Dim stbEtich As New System.Text.StringBuilder

        stbEtich.AppendLine(
                String.Format("{0} + ' - ' + {1} + ' - ' + case ",
                nomeAppezzamento,
                dataValidita))

        Dim formatoLavCodWhenThen As String = "when a1.lav_cod = {0} and {1} then {2} "

        'Rilievo avversita in campo: <descrizioneAvversita>
        Dim etichettaAvversitaCampo = "coalesce(avv.av_des_vol,'')"
        stbEtich.AppendLine(String.Format(formatoLavCodWhenThen,
                                          CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                                          "avv.av_des_vol is not null",
                                          etichettaAvversitaCampo))

        'Rilievo indici maturità: <descrizioneIndiceMaturita>: <qtà> <udm>
        Dim etichettaIndiciMaturita = String.Format("{0} + ': ' + {1} + ' ' + {2}",
                                                    "coalesce(indmat.ind_mat_des,'')",
                                                    "cast(d1.qta as varchar)",
                                                    "coalesce(udmmat.udm_sim collate Latin1_General_CI_AS,'')")

        stbEtich.AppendLine(String.Format(formatoLavCodWhenThen,
                                          CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA,
                                          "indmat.ind_mat_des is not null",
                                          etichettaIndiciMaturita))

        'Rilievo fioritura / fasi fenologiche: <descrizioneStadio> - BBCH <stadioPrincipale><secondaCifra>
        Dim etichettaFasiFenologiche = String.Format("{0} + {1} + {2} + {3}",
                                                     "coalesce(stadi.descrizione,'')",
                                                     "case when bbch.stadio_principale is not null then ' - " + CostantiPersonalizzate.ScalaBBCH + " ' else '' end",
                                                     "coalesce(cast(bbch.stadio_principale as varchar),'')",
                                                     "coalesce(cast(bbch.seconda_cifra as varchar),'')")

        stbEtich.AppendLine(String.Format(formatoLavCodWhenThen,
                                          CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE,
                                          "stadi.descrizione is not null",
                                          etichettaFasiFenologiche))

        'Altre casistiche
        stbEtich.AppendLine("else a1.des_lib end")

        Return stbEtich.ToString

    End Function

    Private Function IsFiltroTemporaleSuEsercizio() As Boolean
        Dim filtroTemporaleSuEsercizio = False

        If Not IsNothing(_filtroTemporaleAvanzato) AndAlso Not IsNothing(_filtroTemporaleAvanzato.filtroTemporalePeriodo) Then
            If _filtroTemporaleAvanzato.filtroTemporalePeriodo.TipoFiltroTemporale = FiltroTemporale.enum_TipoFiltroTemporale.EserciziValidiAllaData Then
                filtroTemporaleSuEsercizio = True
            End If
        End If

        Return filtroTemporaleSuEsercizio

    End Function

    Private Sub Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_SelectPart(AgendaRilieviParteGis As Boolean, AgendaRilieviAvversita As Boolean, LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std, includiJoinTabelleCatasto As Boolean, StrSQL As StringBuilder, TabellaAliasDateValidita As String, etichetta As String, ByVal TipoEntitaCodRiproiettata As enum_GIS2012_TipoEntita, ByVal id_cod As Integer)
        StrSQL.AppendLine("  Select ")

        If AgendaRilieviAvversita Then
            StrSQL.AppendLine("  distinct ")
        End If

        StrSQL.AppendLine("   e.PivaSuperUser ")
        StrSQL.AppendLine(" , e.Entita_Cod ")
        StrSQL.AppendLine(" , e.Entita_GUID ")
        StrSQL.AppendLine(" , e.ParametriVisualizzazioneLayer ")

        'Gestione delle riproiezioni di layer e di tipo Entità

        If TipoEntitaCodRiproiettata = enum_GIS2012_TipoEntita.Nessuno Then

            'Non richiesta
            StrSQL.AppendLine(" , e.TipoEntita_Cod ")
            StrSQL.AppendLine(" , -1 as layerElementiGrafici_cod_Riproiettato ")
        Else

            If AgendaRilieviAvversita Then
                StrSQL.AppendLine(" , e.TipoEntita_Cod ")
                StrSQL.AppendLine(" , " & TipoEntitaCodRiproiettata & " as layerElementiGrafici_cod_Riproiettato ")
            Else

                'Richiesta riproiezione
                StrSQL.AppendLine(" , " & TipoEntitaCodRiproiettata & " as TipoEntita_Cod ")

                'Gestione specifica per layer

                Select Case LayerElementiGrafici_Cod
                    Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                        StrSQL.AppendLine(" , " & enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI & " as layerElementiGrafici_cod_Riproiettato ")
                    Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                        StrSQL.AppendLine(" , " & TipoEntitaCodRiproiettata & " as layerElementiGrafici_cod_Riproiettato ")
                    Case Else
                        StrSQL.AppendLine(" , -1 as layerElementiGrafici_cod_Riproiettato ")
                End Select

            End If

        End If

        If ({enum_Gis_LayerElementiGrafici_std.CATASTO,
             enum_Gis_LayerElementiGrafici_std.TEMALIBERO}.Contains(LayerElementiGrafici_Cod)) And includiJoinTabelleCatasto Then
            StrSQL.AppendLine(" , pp.Piva ")
            StrSQL.AppendLine(" , pp.Sa_Cod ")
        Else

            StrSQL.AppendLine(" , e.Piva ")
            StrSQL.AppendLine(" , e.Sa_Cod ")
        End If

        StrSQL.AppendLine(" , e.Appezza ")

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                StrSQL.AppendLine(" , app1.Campo_Cod ")

            Case Else
                StrSQL.AppendLine(" , e.Campo_Cod ")

        End Select

        StrSQL.AppendLine(" , e.Id_Imp ")
        StrSQL.AppendLine(" , e.PROV ")
        StrSQL.AppendLine(" , e.COM ")
        StrSQL.AppendLine(" , e.SEZIONE ")
        StrSQL.AppendLine(" , e.FOGLIO ")
        StrSQL.AppendLine(" , e.NUMERO ")
        StrSQL.AppendLine(" , e.SUBALTERNO ")
        StrSQL.AppendLine(" , e.Programmazione_Entita_Cod ")

        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI

                If AgendaRilieviParteGis Then
                    StrSQL.AppendLine(" , e.ID_Agenda ")
                    StrSQL.AppendLine(" , 0 as lav_cod ")
                ElseIf AgendaRilieviAvversita Then
                    StrSQL.AppendLine(" , -1 as ID_Agenda ")
                    StrSQL.AppendLine(String.Format(" , {0} as lav_cod ", CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO.ToString))
                Else
                    StrSQL.AppendLine(" , d1.ID_Agenda ")
                    StrSQL.AppendLine(" , a1.lav_cod ")
                End If

            Case Else

                StrSQL.AppendLine(" , e.ID_Agenda ")
                StrSQL.AppendLine(" , 0 as lav_cod ")

        End Select

        If AgendaRilieviAvversita Then
            StrSQL.AppendLine(" , -1 as Ricetta_Operazione_cod ")
        Else
            StrSQL.AppendLine(" , e.Ricetta_Operazione_cod ")
        End If

        If LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI Or
           LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CAMPIONIANALISI Then
            StrSQL.AppendLine(" , acd.analisi_testata_cod ")
        Else
            StrSQL.AppendLine(" , 0 as analisi_testata_cod ")
        End If

        StrSQL.AppendLine(" , e.analisi_campione_cod ")
        StrSQL.AppendLine(" , e.OLDGrafica_ID ")
        StrSQL.AppendLine(" , e.inviato ")
        StrSQL.AppendLine(" , e.datainvio ")
        StrSQL.AppendLine(" , e.Data_Creazione ")
        StrSQL.AppendLine(" , e.Data_Modifica ")
        StrSQL.AppendLine(" , e.Username_Creazione ")
        StrSQL.AppendLine(" , e.Username_Modifica ")
        StrSQL.AppendLine(" , " & TabellaAliasDateValidita & ".Validita_Inizio ")
        StrSQL.AppendLine(" , " & TabellaAliasDateValidita & ".Validita_Fine ")
        StrSQL.AppendLine(" , e.programmazione_cod ")
        StrSQL.AppendLine(" , e.id_mov_det ")

        ' VAnni: 6/4/2018: includo per il catasto il codice particella
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.CATASTO,
                 enum_Gis_LayerElementiGrafici_std.TEMALIBERO
                If includiJoinTabelleCatasto Then
                    StrSQL.AppendLine(" , pc.Part_Cod ")
                Else
                    StrSQL.AppendLine(" , - 1 as Part_Cod ")
                End If

            Case Else
                StrSQL.AppendLine(" , -1 as Part_Cod ")

        End Select

        StrSQL.AppendLine(" , " & etichetta & " AS ETICHETTA ")

        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                If id_cod <> 0 Then
                    StrSQL.AppendLine(" , reg_cod.id_cod ")
                Else
                    StrSQL.AppendLine(" , -1 AS id_cod ")
                End If
            Case enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
                StrSQL.AppendLine(" , ent.id_cod ")
            Case Else
                StrSQL.AppendLine(" , -1 AS id_cod ")
        End Select

        If LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI Then

            Select Case True

                Case Not IsNothing(_cfgAlbero) AndAlso _cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.Avversita
                    If AgendaRilieviAvversita Then
                        StrSQL.AppendLine(" , p.av_cod as Rilievo_Cod ")
                    Else
                        StrSQL.AppendLine(" , mdt.av_cod as Rilievo_Cod ")
                    End If

                Case Not IsNothing(_cfgAlbero) AndAlso _cfgAlbero.TipologiaLayer_Cod = enum_TipologiaLayer.RilieviVegetoProduttivi
                    StrSQL.AppendLine(" , mdt.FF_Classe as Rilievo_Cod ")

                Case Else
                    StrSQL.AppendLine(" , -1 as Rilievo_Cod ")

            End Select

        Else

            StrSQL.AppendLine(" , -1 as Rilievo_Cod ")

        End If

        StrSQL.AppendLine(" , e.Area_Cod ")
        StrSQL.AppendLine(" , e.Fabbricato_Cod ")
        Dim anno = My.Resources.AgronicaCoreGisDAL.Anno
        Dim impresa = My.Resources.AgronicaCoreGisDAL.Impresa

        Select Case TipoEntitaCodRiproiettata

            Case enum_GIS2012_TipoEntita.Indici_Rischio_Produttivita
                Dim produttivitaQHa = My.Resources.AgronicaCoreGisDAL.ProduttivitaQHa
                Dim plvProduzioneLordaVendibileEuroHa = My.Resources.AgronicaCoreGisDAL.PLVProduzioneLordaVendibileEuroHa

                StrSQL.AppendLine(", '" & anno & "§ ' + CAST(YEAR(iai.Validita_Fine) as nvarchar) + '|" & produttivitaQHa & "§ ' + CAST(iai.Produttivita as nvarchar) + '|" & plvProduzioneLordaVendibileEuroHa & "§ ' + CAST(iai.PLV as nvarchar) + '|" & impresa & "§ ' + REPLACE(REPLACE(impxi.rag_soc, '§', '_'), '|', '_') + '|' as Testo")
            Case enum_GIS2012_TipoEntita.Indice_Erosione
                Dim indiceDiErosioneTHaYr = My.Resources.AgronicaCoreGisDAL.IndiceDiErosioneTHaYr

                StrSQL.AppendLine(", '" & anno & "§ ' + CAST(YEAR(iai.Validita_Fine) as nvarchar) + '|" & indiceDiErosioneTHaYr & "§ ' + CAST(iai.IndiceErosione as nvarchar) + '|" & impresa & "§ ' + REPLACE(REPLACE(impxi.rag_soc, '§', '_'), '|', '_') + '|' as Testo")
            Case enum_GIS2012_TipoEntita.Indice_CO2
                Dim indiceDiCo2THa = My.Resources.AgronicaCoreGisDAL.IndiceDiCO2THa

                StrSQL.AppendLine(", '" & anno & "§ ' + CAST(YEAR(iai.Validita_Fine) as nvarchar) + '|" & indiceDiCO2THa & "§ ' + CAST(iai.IndiceCO2 as nvarchar) + '|" & impresa & "§ ' + REPLACE(REPLACE(impxi.rag_soc, '§', '_'), '|', '_') + '|' as Testo")
            Case enum_GIS2012_TipoEntita.Indice_Rischio_Meteo
                Dim indiceDiRischioMeteoAggregatoPerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoAggregatoPerc
                Dim indiceDiRischioMeteoAllagamentoPerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoAllagamentoPerc
                Dim indiceDiRischioMeteoGrandinePerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoGrandinePerc
                Dim indiceDiRischioMeteoSiccitaPerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoSiccitaPerc
                Dim indiceDiRischioMeteoVentoFortePerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoVentoFortePerc
                Dim indiceDiRischioMeteoGelataPerc = My.Resources.AgronicaCoreGisDAL.IndiceDiRischioMeteoGelataPerc
                StrSQL.AppendLine(", '" & anno & "§ ' + CAST(YEAR(iai.Validita_Fine) as nvarchar) + 
                                     '|" & indiceDiRischioMeteoAggregatoPerc & "§ ' + CAST(iai.IndiceRischioMeteoAggregato as nvarchar) + 
                                     '|" & indiceDiRischioMeteoAllagamentoPerc & "§ ' + CAST(iai.IndiceRischioAllagamento as nvarchar) + 
                                     '|" & indiceDiRischioMeteoGrandinePerc & "§ ' + CAST(iai.IndiceRischioGrandine as nvarchar) + 
                                     '|" & indiceDiRischioMeteoSiccitaPerc & "§ ' + CAST(iai.IndiceRischioSiccita as nvarchar) + 
                                     '|" & indiceDiRischioMeteoVentoFortePerc & "§ ' + CAST(iai.IndiceRischioVentoForte as nvarchar) + 
                                     '|" & indiceDiRischioMeteoGelataPerc & "§ ' + CAST(iai.IndiceRischioGelata as nvarchar) + 
                                     '|" & impresa & "§ '+ REPLACE(REPLACE(impxi.rag_soc, '§', '_'), '|', '_') +
                                     '|' as Testo")
            Case Else
                StrSQL.AppendLine(" , null as Testo")
        End Select

        StrSQL.AppendLine()

    End Sub

    Private Sub internal_SQL_GisEntitaConFiltroTemporaleGetQuery_TmpFiltroImpianti(LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std, cfgAlbero As ConfigurazioneAlbero, StrSQL As StringBuilder)
        Select Case LayerElementiGrafici_Cod
            Case enum_Gis_LayerElementiGrafici_std.IMPIANTI,
                 enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                StrSQL.AppendLine(" inner Join __tmp_FiltroImpianti f ")
                StrSQL.AppendLine("   On e.Piva    = f.piva  ")
                StrSQL.AppendLine("  And e.Sa_Cod = f.sa_cod ")
                StrSQL.AppendLine("  And e.Appezza = f.Appezza  ")
                StrSQL.AppendLine("  And e.Id_Imp = f.id_reg")
                StrSQL.AppendLine("  And f.idTestataTemp =  " & cfgAlbero.FiltroImpiantiIdTestataTemp)

            Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                StrSQL.AppendLine(" inner Join __tmp_FiltroImpianti f ")
                StrSQL.AppendLine("   On e.Piva    = f.piva  ")
                StrSQL.AppendLine("  And e.Sa_Cod = f.sa_cod ")
                StrSQL.AppendLine("  And e.Appezza = f.Appezza  ")
                StrSQL.AppendLine("  And e.Id_Imp = 0 ")
                StrSQL.AppendLine("  And f.idTestataTemp =  " & cfgAlbero.FiltroImpiantiIdTestataTemp)

        End Select
    End Sub

    Private Sub internal_SQL_GisEntitaConFiltroTemporaleGetQuery_UtentiVisibilitaAppoggio(objParametri As AgronicaCoreParametri, StrSQL As StringBuilder)
        StrSQL.AppendLine(" inner join ( ")
        StrSQL.AppendLine(" Select '" & objParametri.PivaSuperUser & "' as pivaSuperUser, '" & objParametri.UtenteUsername & "' as Username, 2 as entita_cod, '' as piva , 0 as sa_cod, 0 as appezza, 0 as id_reg")
        StrSQL.AppendLine(" union all")
        StrSQL.AppendLine(" select * from Utenti_Visibilita_Appoggio (NOLOCK) ")
        StrSQL.AppendLine(") vis ")
        StrSQL.AppendLine(" on  vis.piva = e.piva  ")
        StrSQL.AppendLine(" and vis.sa_Cod = e.sa_cod")
        StrSQL.AppendLine(" and vis.Entita_Cod = 2 ")
        StrSQL.AppendLine(" and vis.pivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
        StrSQL.AppendLine(" and vis.UserName = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
        StrSQL.AppendLine(" and vis.Appezza = 0 ")
        StrSQL.AppendLine(" and vis.id_reg = 0 ")
    End Sub

    Private Sub Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale(
            objParametri As AgronicaCoreParametri,
            LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
            includiJoinTabelleCatasto As Boolean,
            StrSQL As StringBuilder,
            TabellaAliasDateValidita As String,
            AgendaRilieviAvversita As Boolean
        )

        If IsFiltroTemporaleDaApplicare(LayerElementiGrafici_Cod, includiJoinTabelleCatasto) Then

            Dim datiFiltroTemporale = GetDatiFiltroTemporale(LayerElementiGrafici_Cod, objParametri)

            StrSQL.AppendLine(" AND ")

            Internal_SQL_CondizioniWhere_PorzioneFiltroTemporale(
                TabellaAliasDateValidita,
                AgendaRilieviAvversita,
                "validita_fine",
                False,
                datiFiltroTemporale.finestraTemporaleInizio,
                datiFiltroTemporale.finestraTemporaleFine,
                StrSQL,
                datiFiltroTemporale.operatoreLimiteInferiore,
                datiFiltroTemporale.operatoreLimiteSuperiore,
                LayerElementiGrafici_Cod)

        End If

    End Sub

    Private Function IsFiltroTemporaleDaApplicare(LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
                                                  includiJoinTabelleCatasto As Boolean) As Boolean

        '------------------------------------------------------------------------------------------
        ' VAnni: 6/4/2018
        '------------------------------------------------------------------------------------------
        ' La parte di filtro temporale va esclusa nel caso di layer catasto o tema libero
        ' su catasto quando non si richiede il join su tabelle catasto
        '------------------------------------------------------------------------------------------

        Return Not ((LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CATASTO Or
                     LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.TEMALIBERO) And Not includiJoinTabelleCatasto)

    End Function

    Private Function GetDatiFiltroTemporale(
                ByVal LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
                ByRef objParametri As AgronicaCoreParametri
            ) As DatiFiltroTemporale

        Dim datiFiltroTemporale = New DatiFiltroTemporale With {
            .operatoreLimiteInferiore = _defaultLimiteInferiore,
            .operatoreLimiteSuperiore = _defaultLimiteSuperiore,
            .finestraTemporaleInizio = objParametri.FinestraTemporaleInizio,
            .finestraTemporaleFine = objParametri.FinestraTemporaleFine
        }

        Dim filtroApplicato = enumFiltroTemporaleApplicato.Nessuno

        Dim layerSingolaData = False

        If Not IsNothing(_filtroTemporaleAvanzato) Then

            If Not IsNothing(_dtLayerSingolaData) AndAlso _dtLayerSingolaData.Rows.Count > 0 Then

                Dim filtroLayer = String.Format("LayerElementiGrafici_Cod = '{0}'", LayerElementiGrafici_Cod.ToString("D"))

                Dim dtFiltroLayer = _dtLayerSingolaData.Select(filtroLayer)

                If Not IsNothing(dtFiltroLayer) AndAlso dtFiltroLayer.Count > 0 Then
                    layerSingolaData = True
                End If

            End If

            Dim filtroTemporale = New FiltroTemporale()

            If layerSingolaData AndAlso Not IsNothing(_filtroTemporaleAvanzato.filtroTemporaleSingolaData) Then
                filtroApplicato = enumFiltroTemporaleApplicato.SingolaData
                filtroTemporale = _filtroTemporaleAvanzato.filtroTemporaleSingolaData
            Else
                filtroApplicato = enumFiltroTemporaleApplicato.Periodo
                filtroTemporale = _filtroTemporaleAvanzato.filtroTemporalePeriodo
            End If

            ImpostaDatiDaFiltroTemporale(filtroTemporale, datiFiltroTemporale)

        End If

        If filtroApplicato = enumFiltroTemporaleApplicato.Nessuno Then

            Dim nessunFiltroTemporale As Boolean = True

            GetLimitiDaSessione(nessunFiltroTemporale,
                                datiFiltroTemporale.operatoreLimiteInferiore,
                                datiFiltroTemporale.operatoreLimiteSuperiore)

            If Not nessunFiltroTemporale Then
                filtroApplicato = enumFiltroTemporaleApplicato.DaSessione
            End If

        End If

        Return datiFiltroTemporale

    End Function

    Private Sub ImpostaDatiDaFiltroTemporale(ByVal filtroTemporale As FiltroTemporale,
                                             ByRef datiFiltroTemporale As DatiFiltroTemporale)

        datiFiltroTemporale.operatoreLimiteInferiore = GIS_Utility.GetLimiteByTipoOperatoreData(_defaultLimiteInferiore, filtroTemporale.TipoOperatoreDataFine)
        datiFiltroTemporale.operatoreLimiteSuperiore = GIS_Utility.GetLimiteByTipoOperatoreData(_defaultLimiteSuperiore, filtroTemporale.TipoOperatoreDataInizio)
        datiFiltroTemporale.finestraTemporaleInizio = filtroTemporale.DataInizio
        datiFiltroTemporale.finestraTemporaleFine = filtroTemporale.DataFine

    End Sub


    Private Enum enumFiltroTemporaleApplicato
        Nessuno = 0
        DaSessione = 1
        Periodo = 2
        SingolaData = 3
    End Enum

    Private Sub Internal_SQL_CondizioniWhere_PorzioneFiltroTemporale(
                ByVal TabellaAlias As String,
                ByVal AgendaRilieviAvversita As Boolean,
                ByVal ValiditaFineAlias As String,
                ByVal AggiungiOR_Iniziale As Boolean,
                ByVal FinestraTemporaleInizio As DateTime,
                ByVal FinestraTemporaleFine As DateTime,
                ByRef StrSQL As System.Text.StringBuilder,
                ByVal l_inf As String,
                ByVal l_sup As String,
                ByVal LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
                Optional ByVal xFiltroAggiuntivo As String = ""
            )

        If AggiungiOR_Iniziale Then
            StrSQL.Append(" OR ")
        End If


        Select Case LayerElementiGrafici_Cod

            Case enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, enum_Gis_LayerElementiGrafici_std.Op_Agenda

                If AgendaRilieviAvversita Then
                    'lavez - 17/11/2023 - filtro migliore
                    'StrSQL.Append(" (" & TabellaAlias & ".Data_Riferimento is not null AND " & TabellaAlias & ".Data_Riferimento " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                    'StrSQL.Append(" AND " & TabellaAlias & ".Data_Riferimento is not null AND " & TabellaAlias & ".Data_Riferimento " & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & "   " & vbCrLf)
                    StrSQL.Append(" (" & TabellaAlias & ".Data_Riferimento " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                    StrSQL.Append(" AND " & TabellaAlias & ".Data_Riferimento " & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & "   " & vbCrLf)
                Else
                    'lavez - 17/11/2023 - filtro migliore
                    'StrSQL.Append(" (" & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                    'StrSQL.Append(" AND " & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & "   " & vbCrLf)
                    StrSQL.Append(" (" & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                    StrSQL.Append(" AND " & TabellaAlias & ".Validita_inizio " & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & "   " & vbCrLf)
                End If
            Case Else
                'lavez - 17/11/2023 - filtro migliore
                'StrSQL.Append(" (" & TabellaAlias & ".Validita_inizio is not null AND " & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                StrSQL.Append(" (" & TabellaAlias & ".Validita_inizio " & " <= " & Agro_SQL_SaveDate(FinestraTemporaleFine) & "   " & vbCrLf)
                StrSQL.Append(" AND  " & TabellaAlias & "." & ValiditaFineAlias & " >= " & Agro_SQL_SaveDate(FinestraTemporaleInizio) & " " & vbCrLf)

        End Select

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(xFiltroAggiuntivo)
        End If

        StrSQL.Append(" ) " & vbCrLf)
    End Sub

    Private Function Internal_SQL_DecidiFiltro_Utenti_Visibilita_Appoggio(ByVal ObjParametri_server As AgronicaCoreParametri) As Boolean

        Dim UtentiVisiblitaAppoggioLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim dt As DataTable =
            UtentiVisiblitaAppoggioLeggi.Leggi(2, "", "", ObjParametri_server)

        Return (dt.Rows.Count > 0)
    End Function

    Public Sub Internal_SQL_CondizioniWhere(
        ByVal PivaSuperUser As String,
        ByVal Entita_Cod As Int32,
        ByVal TipoEntita_Cod As Int32,
        ByVal PivaPadre As String,
        ByVal Piva As String,
        ByVal Sa_Cod As Int32,
        ByVal Appezza As Int32,
        ByVal Campo_Cod As Int32,
        ByVal Veg_Cod As Int32,
        ByVal DestinazioneUso_Cod As Int32,
        ByVal ID_Imp As Int32,
        ByVal Prov As String,
        ByVal Com As String,
        ByVal Sezione As String,
        ByVal Foglio As Int32,
        ByVal Numero As Int32,
        ByVal Subalterno As String,
        ByVal ID_Agenda As Integer,
        ByVal Programmazione_cod As Integer,
        ByVal Programmazione_Entita_cod As Integer,
        ByVal Ricetta_Operazione_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreParametri,
        ByRef StrSQL As StringBuilder,
        ByVal Sementieri_Sportello_Configurazione_cod As Integer,
        ByVal Codice_Fiscale_Tecnico As String,
        ByVal wktBoundaySTIntersects As String,
        ByVal FiltroAggiuntivo_EscludeAltriFiltri As Boolean)

        StrSQL.AppendLine(" WHERE   1=1 ")

        ' VAnni: 2/3/2017: se devo leggere un entita singola allora tutti gli altri filtri sono superflui.
        If Entita_Cod <> 0 Then
            StrSQL.AppendLine("     AND Entita.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            Exit Sub
        End If

        ' VAnni: 2/3/2017: decide in comportament
        If Not FiltroAggiuntivo_EscludeAltriFiltri Then

            'il valore CF_TEC identifica l'esclusione del filtro per il codice fiscale tecnico, così come arriva da configurazione_siti in DB Server.
            If PivaSuperUser <> Codice_Fiscale_Tecnico AndAlso Codice_Fiscale_Tecnico <> "CF TEC" Then

                'C_F
                StrSQL.AppendLine("    AND ( ")
                StrSQL.AppendLine("         (reg1.codice_fiscale_tecnico is not null AND reg1.codice_fiscale_tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ) ")
                StrSQL.AppendLine("         OR (ent.codice_fiscale_tecnico is not null AND ent.codice_fiscale_tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ) ")
                StrSQL.AppendLine("         OR (reg1.codice_fiscale_tecnico is null   AND ent.codice_fiscale_tecnico is null) ")

                If Sementieri_Sportello_Configurazione_cod <> -1 Then
                    StrSQL.AppendLine(" OR coalesce(filtroSementi.Visibilita_Impianti, 1) = 1 ")
                End If

                StrSQL.AppendLine("     ) ")
            End If

            StrSQL.AppendLine("     AND Entita.PivaSuperUser is not null ")
            StrSQL.AppendLine("     AND Entita.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

            If TipoEntita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(TipoEntita_Cod) & " ")
            End If

            If Piva <> "" Then
                If Sementieri_Sportello_Configurazione_cod <> -1 Then
                    StrSQL.AppendLine("    AND  ")
                    StrSQL.AppendLine("    ( ")
                    StrSQL.AppendLine("        ( ")
                    StrSQL.AppendLine("            coalesce(filtroSementi.Visibilita_Impianti, 1) = 0 and coalesce(reg1.Codice_Fiscale_Tecnico, ent.Codice_Fiscale_Tecnico) =  '" & Agro_SQL_SaveText(PivaPadre) & "' OR '" & Agro_SQL_SaveText(PivaPadre) & "' = '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
                    StrSQL.AppendLine("        ) ")
                    StrSQL.AppendLine("        OR coalesce(filtroSementi.Visibilita_Impianti, 1) = 1 ")
                    StrSQL.AppendLine("    ) ")
                End If
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Entita.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Entita.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " or app1.campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " )")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND (cul_reale.veg_cod = " & Agro_SQL_SaveNum(Veg_Cod) & " or cul_planning.veg_cod = " & Agro_SQL_SaveNum(Veg_Cod) & ") ")
            End If

            If DestinazioneUso_Cod <> 0 Then
                StrSQL.AppendLine(" AND Entita.id_cod = " & Agro_SQL_SaveNum(DestinazioneUso_Cod) & " ")
            End If

            If ID_Imp <> 0 Then
                StrSQL.AppendLine(" AND Entita.ID_Imp = " & Agro_SQL_SaveNum(ID_Imp) & " ")
            End If

            If Prov <> "" Then
                StrSQL.AppendLine(" AND Entita.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.AppendLine(" AND Entita.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "-1" Then
                StrSQL.AppendLine(" AND Entita.Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If

            If Foglio <> -1 Then
                StrSQL.AppendLine(" AND Entita.Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> -1 Then
                StrSQL.AppendLine(" AND Entita.Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "-1" Then
                StrSQL.AppendLine(" AND Entita.Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            If ID_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Entita.ID_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & " ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Entita.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            End If

            If Programmazione_cod <> 0 Then
                StrSQL.AppendLine(" AND Entita.Programmazione_cod = " & Agro_SQL_SaveNum(Programmazione_cod) & " ")
            End If

            If Programmazione_Entita_cod <> 0 Then
                StrSQL.AppendLine(" AND Entita.Programmazione_Entita_cod = " & Agro_SQL_SaveNum(Programmazione_Entita_cod) & " ")
            End If

        End If

        ' Vanni: 27/11/2017: intersezione per caricare solo quello che sta sulla porzione di mappa
        ' Vanni, 31/10/2023: spostato filtro su inserimento in tabella @TEMPENTITA
        'If Not String.IsNullOrEmpty(wktBoundaySTIntersects) Then
        '    StrSQL.AppendLine(" AND GeoDataIntersezione.Poligono_GeoEntity.STIntersects( ")
        '    StrSQL.AppendLine("     geography::STGeomFromText( ")
        '    StrSQL.AppendLine("      '" & wktBoundaySTIntersects & "' ")
        '    StrSQL.AppendLine("      , 4326)          ")
        '    StrSQL.AppendLine("  ) = 1")

        'End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

    End Sub

    Private Sub SQL_InMemory_Table_Op(
        ByRef StrSQL As StringBuilder,
        ByVal TipologiaLayer_Cod As Integer,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        )
        internal_SQL_Declare_InMemory_Table_Op(StrSQL)
        SQL_CTE_Avversita(StrSQL, True, True)
        SQL_CTE_RicetteXOp(StrSQL, TipologiaLayer_Cod, True)
        SQL_CTE_AvversitaCampo_IndiciMaturita_ResaRaccolta(StrSQL, TipologiaLayer_Cod, True)
        If TipologiaLayer_Cod = TipiEnumerativi.enum_TipologiaLayer.Avversita Then
            SQL_CTE_AgendaXOp_113_109_169(StrSQL, TipologiaLayer_Cod, True)
            SQL_CTE_PrevisionaliAvv(StrSQL)
            internal_SQL_InsertSelect_TableOpAvv(StrSQL, objParametri_Server)
        Else
            SQL_CTE_AgendaXOp_113_109_169(StrSQL, TipologiaLayer_Cod, False)
            internal_SQL_InsertSelect_TableOpVegProd(StrSQL, objParametri_Server)
        End If
    End Sub

    Private Sub SQL_InMemory_TableTemp_Operazioni(ByRef StrSQL As StringBuilder)
        internal_SQL_Declare_InMemory_TableTemp_Operazioni(StrSQL)
        SQL_CTE_GIS_TipoEntita(StrSQL, True, True)
        SQL_CTE_OperazioniAgenda(StrSQL, True)
        SQL_CTE_OperazioniTmp(StrSQL)
        internal_SQL_InsertSelect_OperazioniTmp(StrSQL)
        internal_SQL_Delete_OpoerazioniTmp_RaccolteMultiple(StrSQL)
        internal_SQL_Declare_InMemory_Table_Operazioni(StrSQL)
        SQL_CTE_GIS_TipoEntita(StrSQL, True, True)
        SQL_CTE_EntitaSenzaOperazioni(StrSQL)
        internal_SQL_InsertSelect_Operazioni(StrSQL)
    End Sub

    Private Sub SQL_InMemory_Table_Raccolta(ByRef StrSQL As StringBuilder)
        internal_SQL_Declare_TableRaccolta(StrSQL)
        internal_SQL_InsertSelect_TableRaccolta(StrSQL)
    End Sub

    Private Sub internal_SQL_Declare_InMemory_TableTemp_Operazioni(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("DECLARE @TableOperazioniTmp table (Piva varchar(50), ")
        StrSQL.AppendLine("                                   Sa_Cod int, ")
        StrSQL.AppendLine("                                   Appezza Int, ")
        StrSQL.AppendLine("                                   id_reg int, ")
        StrSQL.AppendLine("                                   id_mov_det int, ")
        StrSQL.AppendLine("                                   LAV_COD int, ")
        StrSQL.AppendLine("                                   DataOperazione datetime, ")
        StrSQL.AppendLine("                                   ggUltimaOperazione int); ")
    End Sub

    Private Sub internal_SQL_Declare_InMemory_Table_Operazioni(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("Declare @TableOperazioni table(Piva varchar(50), ")
        StrSQL.AppendLine("                               Sa_Cod Int, ")
        StrSQL.AppendLine("                               Appezza Int, ")
        StrSQL.AppendLine("                               id_reg Int, ")
        StrSQL.AppendLine("                               id_mov_det Int, ")
        StrSQL.AppendLine("                               LAV_COD Int, ")
        StrSQL.AppendLine("                               InfoEstese varchar(max)); ")
    End Sub

    Private Sub internal_SQL_Declare_InMemory_Table_Op(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("Declare @TableOp Table(piva varchar(20), ")
        StrSQL.AppendLine("                       Sa_Cod Int, ")
        StrSQL.AppendLine("                       Appezza Int, ")
        StrSQL.AppendLine("                       id_reg Int, ")
        StrSQL.AppendLine("                       id_agenda Int, ")
        StrSQL.AppendLine("                       ricetta_operazione_cod Int, ")
        StrSQL.AppendLine("                       info_estese varchar(max), ")
        StrSQL.AppendLine("                       DataOperazione Date, ")
        StrSQL.AppendLine("                       av_cod Int ); ")
    End Sub

    Private Sub SQL_CTE_OperazioniAgenda(ByRef StrSQL As StringBuilder,
                         Optional addCommaToEnd As Boolean = False,
                         Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_agenda as( ")
        StrSQL.AppendLine("		select ")
        StrSQL.AppendLine("		a.LAV_COD ")
        StrSQL.AppendLine("		, a.ggUltimaOperazione ")
        StrSQL.AppendLine("		, a.Piva ")
        StrSQL.AppendLine("		, a.Sa_Cod ")
        StrSQL.AppendLine("		, a.Appezza ")
        StrSQL.AppendLine("		, a.id_reg ")
        StrSQL.AppendLine("		, MIN(a.Id_Mov_Det) as id_mov_det, a.DataOperazione ")
        StrSQL.AppendLine("		from ( ")
        StrSQL.AppendLine("			select ")
        StrSQL.AppendLine("			a.LAV_COD ")
        StrSQL.AppendLine("			, b.ggUltimaOperazione ")
        StrSQL.AppendLine("			, b.Piva ")
        StrSQL.AppendLine("			, b.Sa_Cod ")
        StrSQL.AppendLine("			, b.Appezza ")
        StrSQL.AppendLine("			, b.Id_Destinazione as id_reg ")
        StrSQL.AppendLine("			, a.Id_Mov_Det, b.DataOperazione ")
        StrSQL.AppendLine("			from ( ")
        StrSQL.AppendLine("				Select ")
        StrSQL.AppendLine("				a1.Lav_Cod ")
        StrSQL.AppendLine("				, a1.Validita_Inizio as DataOperazione ")
        StrSQL.AppendLine("				, d.Piva ")
        StrSQL.AppendLine("				, d.Sa_Cod ")
        StrSQL.AppendLine("				, d.Appezza ")
        StrSQL.AppendLine("				, d.Id_Destinazione ")
        StrSQL.AppendLine("				, d.Id_Mov_Det ")
        StrSQL.AppendLine("				from Agenda a1 ")
        StrSQL.AppendLine("				inner join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0) a ")
        StrSQL.AppendLine("				inner join ( ")
        StrSQL.AppendLine("					select ")
        StrSQL.AppendLine("					a1.LAV_COD ")
        StrSQL.AppendLine("					, d.Piva, d.Sa_Cod ")
        StrSQL.AppendLine("					, d.Appezza ")
        StrSQL.AppendLine("					, d.Id_Destinazione ")
        StrSQL.AppendLine("					, ABS(DATEDIFF(DAY, GETDATE(),  max(a1.Validita_Inizio))) as ggUltimaOperazione ")
        StrSQL.AppendLine("					, MAX( a1.Validita_Inizio) as DataOperazione ")
        StrSQL.AppendLine("					From Agenda a1 ")
        StrSQL.AppendLine("					inner Join Mov_Destinazioni d On a1.Id_Agenda = d.Id_Agenda And d.Tipo_Destinazione = 0 ")
        StrSQL.AppendLine("					group by a1.LAV_COD, d.Piva, d.Sa_Cod, d.Appezza, d.Id_Destinazione ")
        StrSQL.AppendLine("				) b ")
        StrSQL.AppendLine("				On a.LAV_COD = b.LAV_COD ")
        StrSQL.AppendLine("			And a.Piva = b.Piva ")
        StrSQL.AppendLine("			And a.Sa_Cod = b.Sa_Cod ")
        StrSQL.AppendLine("			And a.Appezza = b.Appezza ")
        StrSQL.AppendLine("			And a.Id_Destinazione = b.Id_Destinazione ")
        StrSQL.AppendLine("			And a.dataOperazione =  b.dataOperazione ")
        StrSQL.AppendLine("			) a ")
        StrSQL.AppendLine("		group by a.LAV_COD, a.ggUltimaOperazione, a.Piva, a.Sa_Cod, a.Appezza, a.id_reg, a.DataOperazione ")
        StrSQL.AppendLine("		union ")
        StrSQL.AppendLine("		Select ")
        StrSQL.AppendLine("			113 as LAV_COD ")
        StrSQL.AppendLine("			, ABS(DATEDIFF(DAY, GETDATE(), Data_Riferimento ) ) as  ggUltimaOperazione ")
        StrSQL.AppendLine("			, Piva ")
        StrSQL.AppendLine("			, Sa_Cod ")
        StrSQL.AppendLine("			, Appezza ")
        StrSQL.AppendLine("			, id_reg ")
        StrSQL.AppendLine("			, -1 as Id_Mov_Det ")
        StrSQL.AppendLine("			, Data_Riferimento as DataOperazione ")
        StrSQL.AppendLine("		from ( ")
        StrSQL.AppendLine("			select ")
        StrSQL.AppendLine("			piva ")
        StrSQL.AppendLine("			, Sa_Cod ")
        StrSQL.AppendLine("			, Appezza ")
        StrSQL.AppendLine("			, id_reg ")
        StrSQL.AppendLine("			, Max(srv.Data_Riferimento) as Data_Riferimento ")
        StrSQL.AppendLine("			From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo imp ")
        StrSQL.AppendLine("			inner Join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
        StrSQL.AppendLine("				On imp.tipo_sorgente = srv.tipo_sorgente ")
        StrSQL.AppendLine("				And imp.Stazione_Cod = srv.stazione_Cod ")
        StrSQL.AppendLine("			group by imp.piva, imp.sa_cod, imp.appezza, imp.id_reg ")
        StrSQL.AppendLine("		    ) a ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_OperazioniTmp(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("_cte_operazionitmp as( ")
        StrSQL.AppendLine("		Select ")
        StrSQL.AppendLine("			op.Piva ")
        StrSQL.AppendLine("			, op.Sa_Cod ")
        StrSQL.AppendLine("			, op.Appezza ")
        StrSQL.AppendLine("			, op.id_reg ")
        StrSQL.AppendLine("			, op.id_mov_det ")
        StrSQL.AppendLine("			, op.LAV_COD ")
        StrSQL.AppendLine("			, op.DataOperazione ")
        StrSQL.AppendLine("			, op.ggUltimaOperazione ")
        StrSQL.AppendLine("		from ( ")
        StrSQL.AppendLine("			select ")
        StrSQL.AppendLine("					c.* ")
        StrSQL.AppendLine("		from cte_agenda c ")
        StrSQL.AppendLine("			inner Join GIS_Entita e1 ")
        StrSQL.AppendLine("				On e1.Piva = c.Piva ")
        StrSQL.AppendLine("				And e1.Sa_Cod = c.sa_cod ")
        StrSQL.AppendLine("				And e1.Appezza = c.Appezza ")
        StrSQL.AppendLine("				And e1.Id_Imp = c.id_reg ")
        StrSQL.AppendLine("				And ( (c.id_mov_det > 0 And e1.id_mov_det = c.Id_Mov_Det ) Or (c.Id_Mov_Det = -1 and e1.tipoEntita_Cod in (select * from cte_GIS_TipoEntita ) )) ")
        StrSQL.AppendLine("			) op  ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub internal_SQL_InsertSelect_OperazioniTmp(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("	insert into @TableOperazioniTmp ")
        StrSQL.AppendLine("		Select * from _cte_operazionitmp")
    End Sub

    Private Sub internal_SQL_Delete_OpoerazioniTmp_RaccolteMultiple(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("	DELETE ")
        StrSQL.AppendLine("	FROM @TableOperazioniTmp ")
        StrSQL.AppendLine("	where id_mov_det In ( ")
        StrSQL.AppendLine("					Select ")
        StrSQL.AppendLine("						id_mov_det ")
        StrSQL.AppendLine("					from @TableOperazioniTmp t_o ")
        StrSQL.AppendLine("					inner join ( ")
        StrSQL.AppendLine("								Select ")
        StrSQL.AppendLine("									piva, ")
        StrSQL.AppendLine("									sa_cod, ")
        StrSQL.AppendLine("									appezza, ")
        StrSQL.AppendLine("									id_reg, ")
        StrSQL.AppendLine("									count(*) As CONTO ")
        StrSQL.AppendLine("								from @TableOperazioniTmp ")
        StrSQL.AppendLine("								where id_mov_det > 0 ")
        StrSQL.AppendLine("								GROUP BY piva, sa_cod, appezza, id_reg ")
        StrSQL.AppendLine("								HAVING count(*)>1 ")
        StrSQL.AppendLine("							) a  ")
        StrSQL.AppendLine("						On a.piva = t_o.Piva And a.Sa_Cod = t_o.Sa_Cod And a.Appezza = t_o.Appezza ")
        StrSQL.AppendLine("						where lav_cod <> 125 ")
        StrSQL.AppendLine("					) ")
    End Sub

    Private Sub SQL_CTE_EntitaSenzaOperazioni(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("cte_entitasenzaoperazioni as ( ")
        StrSQL.AppendLine("		Select ")
        StrSQL.AppendLine("			e1.Piva ")
        StrSQL.AppendLine("			, e1.Sa_Cod ")
        StrSQL.AppendLine("			, e1.Appezza ")
        StrSQL.AppendLine("			, e1.Id_Imp As id_reg ")
        StrSQL.AppendLine("			, 0 As id_mov_det ")
        StrSQL.AppendLine("			, 0 As LAV_COD ")
        StrSQL.AppendLine("			, '01/01/1900' as DataOperazione ")
        StrSQL.AppendLine("			, 99999 As ggUltimaOperazione ")
        StrSQL.AppendLine("		From GIS_Entita e1 ")
        StrSQL.AppendLine("		Where TipoEntita_Cod In (Select TipoEntita_Cod from cte_GIS_TipoEntita) ")
        StrSQL.AppendLine("		And Not exists (  ")
        StrSQL.AppendLine("				Select  ")
        StrSQL.AppendLine("					1  ")
        StrSQL.AppendLine("				From (  ")
        StrSQL.AppendLine("					Select ")
        StrSQL.AppendLine("						Piva ")
        StrSQL.AppendLine("						, sa_cod ")
        StrSQL.AppendLine("						, appezza ")
        StrSQL.AppendLine("						, id_destinazione ")
        StrSQL.AppendLine("						, Tipo_Destinazione ")
        StrSQL.AppendLine("					From Mov_Destinazioni ")
        StrSQL.AppendLine("					union ")
        StrSQL.AppendLine("					Select ")
        StrSQL.AppendLine("						Piva ")
        StrSQL.AppendLine("						, Sa_Cod ")
        StrSQL.AppendLine("						, Appezza ")
        StrSQL.AppendLine("						, id_Reg As id_destinazione ")
        StrSQL.AppendLine("						, 0 As tipo_destinazione ")
        StrSQL.AppendLine("					From DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo ")
        StrSQL.AppendLine("				) c ")
        StrSQL.AppendLine("				Where e1.Piva = c.Piva ")
        StrSQL.AppendLine("				And e1.Sa_Cod = c.sa_cod ")
        StrSQL.AppendLine("				And e1.Appezza = c.Appezza ")
        StrSQL.AppendLine("				And e1.Id_Imp = c.Id_Destinazione ")
        StrSQL.AppendLine("				And c.Tipo_Destinazione = 0 ")
        StrSQL.AppendLine("			) ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub internal_SQL_InsertSelect_Operazioni(ByRef StrSQL As StringBuilder)
        Dim dataOperazione = My.Resources.AgronicaCoreGisDAL.DataOperazione
        Dim giorniTrascorsi = My.Resources.AgronicaCoreGisDAL.GiorniTrascorsi
        StrSQL.AppendLine("	insert into @TableOperazioni ")
        StrSQL.AppendLine("		Select ")
        StrSQL.AppendLine("			Piva ")
        StrSQL.AppendLine("			, Sa_Cod ")
        StrSQL.AppendLine("			, Appezza ")
        StrSQL.AppendLine("			, id_reg ")
        StrSQL.AppendLine("			, id_mov_det ")
        StrSQL.AppendLine("			, LAV_COD ")
        StrSQL.AppendLine("			, '" & dataOperazione & "§ ' + convert(varchar(50), DataOperazione, 103 ) + '|' +  '" & giorniTrascorsi & "§  ' +  replace( cast(ggUltimaOperazione as varchar(100)), ',', '.') + '|' as InfoEstese ")
        StrSQL.AppendLine("		from ( ")
        StrSQL.AppendLine("			Select * from @TableOperazioniTmp ")
        StrSQL.AppendLine("			union ")
        StrSQL.AppendLine("			select * from cte_entitasenzaoperazioni ")
        StrSQL.AppendLine("			) op ;")
    End Sub

    Private Sub internal_SQL_Declare_TableRaccolta(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("	Declare @TableRaccolta table(Piva varchar(50) ")
        StrSQL.AppendLine("	                            , Sa_Cod int ")
        StrSQL.AppendLine("	                            , Appezza int ")
        StrSQL.AppendLine("	                            , id_imp int ")
        StrSQL.AppendLine("	                            , DataRaccolta datetime ")
        StrSQL.AppendLine("	                            , ggUltimaRaccolta int); ")
    End Sub

    Private Sub internal_SQL_InsertSelect_TableRaccolta(ByRef StrSQL As StringBuilder)
        StrSQL.AppendLine("	insert into @TableRaccolta ")
        StrSQL.AppendLine("	Select  ")
        StrSQL.AppendLine("		* ")
        StrSQL.AppendLine("		, DATEDIFF(DAY, DataRaccolta, GETDATE()) as ggUltimaRaccolta  ")
        StrSQL.AppendLine("		from (  ")
        StrSQL.AppendLine("	       Select  ")
        StrSQL.AppendLine("				d.Piva ")
        StrSQL.AppendLine("				, d.Sa_Cod ")
        StrSQL.AppendLine("				, d.Appezza ")
        StrSQL.AppendLine("				, d.Id_Destinazione ")
        StrSQL.AppendLine("				, max(a1.Validita_Inizio) As DataRaccolta ")
        StrSQL.AppendLine("	       From Agenda a1 ")
        StrSQL.AppendLine("	       inner Join Mov_Destinazioni d  ")
        StrSQL.AppendLine("				On a1.Id_Agenda = d.Id_Agenda  ")
        StrSQL.AppendLine("				And d.Tipo_Destinazione = 0 ")
        StrSQL.AppendLine("	       inner Join GIS_Entita e1  ")
        StrSQL.AppendLine("				On e1.Piva = a1.Piva  ")
        StrSQL.AppendLine("				And e1.Sa_Cod = a1.sa_cod  ")
        StrSQL.AppendLine("				And e1.Appezza = d.Appezza  ")
        StrSQL.AppendLine("				And e1.Id_Imp = d.id_destinazione  ")
        StrSQL.AppendLine("				And e1.id_mov_det = d.Id_Mov_Det ")
        StrSQL.AppendLine("	       where a1.LAV_COD = " & CostantiPersonalizzate.LAVCOD_RACCOLTA & " ")
        StrSQL.AppendLine("	       group by d.piva, d.sa_cod, d.appezza, d.id_destinazione ")
        StrSQL.AppendLine("	   ) tmp1 ")
        StrSQL.AppendLine("	   where DATEDIFF(DAY, DataRaccolta, GETDATE())<=180 ;")
    End Sub

    Private Sub SQL_CTE_layer_specie_vegetale(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_layer_specie_vegetale as ( ")
        StrSQL.AppendLine("	    Select distinct ")
        StrSQL.AppendLine("	        reg.PIVA, ")
        StrSQL.AppendLine("	        reg.SA_COD, ")
        StrSQL.AppendLine("	        reg.APPEZZA, ")
        StrSQL.AppendLine("	        reg.ID_REG, ")
        StrSQL.AppendLine("	        0 As Programmazione_Entita_Cod, ")
        StrSQL.AppendLine("	        cast(veg_cod As varchar(1000)) As specie_vegetale ")
        StrSQL.AppendLine("	    From reg_impianti reg ")
        StrSQL.AppendLine("	    inner Join cte_cultivar cul ")
        StrSQL.AppendLine("	        On reg.cul_cod = cul.Cul_Cod ")
        StrSQL.AppendLine("	    union ")
        StrSQL.AppendLine("	    Select distinct ")
        StrSQL.AppendLine("	        '' as PIVA, ")
        StrSQL.AppendLine("	        -1 as SA_COD, ")
        StrSQL.AppendLine("	        -1 as APPEZZA, ")
        StrSQL.AppendLine("	        -1 as ID_REG, ")
        StrSQL.AppendLine("	        entl.Programmazione_Entita_Cod, ")
        StrSQL.AppendLine("	        cast(cul.veg_cod As varchar(1000)) as specie_vegetale ")
        StrSQL.AppendLine("	    From Programmazione_Entita entl ")
        StrSQL.AppendLine("	    inner Join cte_cultivar cul ")
        StrSQL.AppendLine("	        On entl.cul_cod = cul.cul_cod ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_layer_varieta(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_layer_varieta as ( ")
        StrSQL.AppendLine("	    Select distinct ")
        StrSQL.AppendLine("	        reg.PIVA, ")
        StrSQL.AppendLine("	        reg.SA_COD, ")
        StrSQL.AppendLine("	        reg.APPEZZA, ")
        StrSQL.AppendLine("	        reg.ID_REG, ")
        StrSQL.AppendLine("	        0 As Programmazione_Entita_Cod, ")
        StrSQL.AppendLine("	        cast(reg.cul_cod As varchar(1000)) As Varieta ")
        StrSQL.AppendLine("	    From reg_impianti reg ")
        'StrSQL.AppendLine("	    inner Join cte_cultivar cul ")
        'StrSQL.AppendLine("	        On reg.cul_cod = cul.Cul_Cod ")
        StrSQL.AppendLine("	    union ")
        StrSQL.AppendLine("	    Select distinct ")
        StrSQL.AppendLine("	        '' as PIVA, ")
        StrSQL.AppendLine("	        -1 as SA_COD, ")
        StrSQL.AppendLine("	        -1 as APPEZZA, ")
        StrSQL.AppendLine("	        -1 as ID_REG, ")
        StrSQL.AppendLine("	        entl.Programmazione_Entita_Cod, ")
        StrSQL.AppendLine("	        cast(entl.cul_cod As varchar(1000)) as Varieta ")
        StrSQL.AppendLine("	    From Programmazione_Entita entl ")
        'StrSQL.AppendLine("	    inner Join cte_cultivar cul ")
        'StrSQL.AppendLine("	        On entl.cul_cod = cul.cul_cod ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_layer_cultivar(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_layer_specie_vegetale as ( ")
        StrSQL.AppendLine("	    Select ")
        StrSQL.AppendLine("	        0 as cul_cod ")
        StrSQL.AppendLine("	        , 0 as veg_cod ")
        StrSQL.AppendLine("	    union ")
        StrSQL.AppendLine("	    Select ")
        StrSQL.AppendLine("	        cul_cod ")
        StrSQL.AppendLine("	        , veg_cod ")
        StrSQL.AppendLine("	    From cultivar ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_layer_fenologia(ByRef StrSQL As StringBuilder,
                                     Optional addCommaToEnd As Boolean = False,
                                     Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_layer_fenologia as ( ")
        'StrSQL.AppendLine("	    select distinct ")
        'StrSQL.AppendLine("	           ricetta_operazione_cod ")
        'StrSQL.AppendLine("	         , 0 as id_Agenda ")
        'StrSQL.AppendLine("	         , case when rDt.ff_Cod = 0 then null ")
        'StrSQL.AppendLine("	                else cast(rDt.ff_Cod As varchar(100)) ")
        'StrSQL.AppendLine("	           end As ff_Cod ")
        'StrSQL.AppendLine("	    from (select * from cte_classifenologichericette) rDt ")
        'StrSQL.AppendLine("	    union ")
        StrSQL.AppendLine("	    select distinct ")
        StrSQL.AppendLine("	           0 as ricetta_operazione_cod ")
        StrSQL.AppendLine("	         , id_Agenda ")
        StrSQL.AppendLine("	         , case when rDt.ff_Cod = 0 then null ")
        StrSQL.AppendLine("	                else cast(rDt.ff_Cod As varchar(100)) ")
        StrSQL.AppendLine("	           end As ff_Cod ")
        StrSQL.AppendLine("	    from (select * from cte_classifenologicheagenda) rDt ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_AgendaRilieviVegetoProduttivi(ByRef StrSQL As StringBuilder,
                                      Optional addCommaToEnd As Boolean = False,
                                      Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("With ")
        End If
        StrSQL.AppendLine("	cte_agendarilievivegetoproduttivi As (")
        StrSQL.AppendLine("	    Select ")
        StrSQL.AppendLine("	        a.id_Agenda, ")
        StrSQL.AppendLine("	        t.ff_classe ")
        StrSQL.AppendLine("	    From Mov_Dettaglio_Tecnico t ")
        StrSQL.AppendLine("	    inner Join agenda a ")
        StrSQL.AppendLine("	    On a.id_Agenda = t.id_agenda ")
        StrSQL.AppendLine("	    where a.lav_cod in (" & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA & ", " & CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & ") ")
        StrSQL.AppendLine("	    And t.ff_classe <> 0 ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_layer_rilievivegetoproduttivi(ByRef StrSQL As StringBuilder,
                                                      Optional addCommaToEnd As Boolean = False,
                                                      Optional addWithToStart As Boolean = False)

        If addWithToStart Then
            StrSQL.Append("With ")
        End If
        StrSQL.AppendLine("	cte_layer_rilievivegetoproduttivi As (")
        StrSQL.AppendLine("	    Select ")
        StrSQL.AppendLine("	        id_agenda ")
        StrSQL.AppendLine("         , Case When rDt.ff_classe=0 Then ")
        StrSQL.AppendLine("	    	    null ")
        StrSQL.AppendLine("	          Else ")
        StrSQL.AppendLine("	            cast(rDt.ff_classe As varchar(100)) ")
        StrSQL.AppendLine("	          End ff_classe ")
        StrSQL.AppendLine("	    from ( ")
        StrSQL.AppendLine("	        Select * from cte_agendarilievivegetoproduttivi ")
        StrSQL.AppendLine("	        union ")
        StrSQL.AppendLine("	        Select 0 As id_Agenda, 0 As ff_classe ")
        StrSQL.AppendLine("	    ) rDt ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_layer_avversita(ByRef StrSQL As StringBuilder,
                                        Optional addCommaToEnd As Boolean = False,
                                        Optional addWithToStart As Boolean = False)

        If addWithToStart Then
            StrSQL.Append("With ")
        End If
        StrSQL.AppendLine("	cte_layer_avversita As ( ")
        StrSQL.AppendLine("	    select ")
        StrSQL.AppendLine("	        ricetta_operazione_cod ")
        StrSQL.AppendLine("	        , -1 As id_Agenda ")
        StrSQL.AppendLine("           , '' as piva ")
        StrSQL.AppendLine("	        , -1 as sa_Cod ")
        StrSQL.AppendLine("	        , -1 as appezza ")
        StrSQL.AppendLine("	        , -1 as id_reg ")
        StrSQL.AppendLine("	        , -1 as id_mov_det ")
        StrSQL.AppendLine("	        , case when rDt.Av_Cod=0 then ")
        StrSQL.AppendLine("		        null ")
        StrSQL.AppendLine("	          Else ")
        StrSQL.AppendLine("	            cast(rDt.Av_Cod As varchar(100)) ")
        StrSQL.AppendLine("	          End As av_cod ")
        StrSQL.AppendLine("	    from cte_ricetteavversita rDt ")
        StrSQL.AppendLine("	    UNION ")
        StrSQL.AppendLine("	    select ")
        StrSQL.AppendLine("		    0 as ricetta_operazione_cod ")
        StrSQL.AppendLine("		    , id_agenda ")
        StrSQL.AppendLine("		    , piva ")
        StrSQL.AppendLine("		    , sa_Cod ")
        StrSQL.AppendLine("		    , appezza ")
        StrSQL.AppendLine("		    , id_reg ")
        StrSQL.AppendLine("		    , id_mov_det ")
        StrSQL.AppendLine("		    , case when rDt.Av_Cod=0 then ")
        StrSQL.AppendLine("			    null ")
        StrSQL.AppendLine("	          Else ")
        StrSQL.AppendLine("	            cast(rDt.Av_Cod As varchar(100)) ")
        StrSQL.AppendLine("		      End As av_cod ")
        StrSQL.AppendLine("	    from( select * from cte_movagendaxavversita ) rDt ")
        StrSQL.AppendLine("	    UNION ")
        StrSQL.AppendLine("	    Select ")
        StrSQL.AppendLine("		    -1 as ricetta_operazione_cod ")
        StrSQL.AppendLine("		    , -1 as id_Agenda ")
        StrSQL.AppendLine("		    , rDt.piva ")
        StrSQL.AppendLine("		    , rDt.sa_cod ")
        StrSQL.AppendLine("		    , rDt.appezza ")
        StrSQL.AppendLine("		    , rDt.id_reg ")
        StrSQL.AppendLine("	        , -1 as id_mov_det ")
        StrSQL.AppendLine("		    , case when rDt.Av_Cod=0 then ")
        StrSQL.AppendLine("			    null ")
        StrSQL.AppendLine("	          Else ")
        StrSQL.AppendLine("	            cast(rDt.Av_Cod As varchar(100)) ")
        StrSQL.AppendLine("		      End As '*' ")
        StrSQL.AppendLine("	    from ( ")
        StrSQL.AppendLine("		      select distinct -1 as id_agenda, piva, sa_cod, appezza, id_reg, av_cod ")
        StrSQL.AppendLine("		      From DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv ")
        StrSQL.AppendLine("		      inner Join DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d On srv.Stazione_Cod = d.Stazione_Cod ")
        StrSQL.AppendLine("		      		      		      		      		      		     And srv.tipo_sorgente = d.tipo_sorgente ")
        StrSQL.AppendLine("		     ) rDt ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_Text(ByRef StrSQL As StringBuilder,
                             Optional addCommaToEnd As Boolean = False,
                             Optional addWithToStart As Boolean = False)

        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_text as ( ")
        StrSQL.AppendLine("		select ")
        StrSQL.AppendLine("			a.ricetta_operazione_cod, ")
        StrSQL.AppendLine("			a.id_agenda, ")
        StrSQL.AppendLine("			a.piva, ")
        StrSQL.AppendLine("			a.Sa_Cod, ")
        StrSQL.AppendLine("			a.Appezza, ")
        StrSQL.AppendLine("			a.id_reg, ")
        StrSQL.AppendLine("			string_agg(a.testo,'') as testo ")
        StrSQL.AppendLine("		from ( ")
        StrSQL.AppendLine("			select * from ( ")
        StrSQL.AppendLine("			SELECT [udm_des] + '§  ' +  replace( cast(qta as varchar(100)), ',', '.') + '|' as testo ")
        StrSQL.AppendLine("			, Ricetta_Operazione_Cod ")
        StrSQL.AppendLine("			, id_agenda ")
        StrSQL.AppendLine("			, piva ")
        StrSQL.AppendLine("			, sa_cod ")
        StrSQL.AppendLine("			, appezza ")
        StrSQL.AppendLine("			, id_reg ")
        StrSQL.AppendLine("			FROM ")
        StrSQL.AppendLine("				(  ")
        StrSQL.AppendLine("					select * from cte_ricetteconavversita op where op.inviato>=0 ")
        StrSQL.AppendLine("					union all ")
        StrSQL.AppendLine("					select * from cte_operazioni op1 where op1.inviato>=0 ")
        StrSQL.AppendLine("				) cl  ")
        StrSQL.AppendLine("			) b ")
        StrSQL.AppendLine("		)a ")
        StrSQL.AppendLine("		group by a.ricetta_operazione_cod,a.id_agenda,a.piva,a.Sa_Cod,a.Appezza,a.id_reg ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_AgendaXOp_113_109_169(
        ByRef StrSQL As StringBuilder,
        ByVal TipologiaLayer_Cod As Integer,
        Optional addCommaToEnd As Boolean = False,
        Optional addWithToStart As Boolean = False
        )

        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_agendaxop_113_109_169 as ( ")
        StrSQL.AppendLine("                select Op.Piva ")
        StrSQL.AppendLine("                     , Op.sa_cod ")
        StrSQL.AppendLine("                     , RegImp.Appezza ")
        StrSQL.AppendLine("                     , RegImp.id_reg ")
        StrSQL.AppendLine("                     , Op.id_Agenda ")
        StrSQL.AppendLine("                     , 0 as Ricetta_Operazione_Cod ")
        StrSQL.AppendLine("                     , Op.Lav_Cod ")
        StrSQL.AppendLine("                     , tec.Av_Cod ")
        StrSQL.AppendLine("                     , case when Lav_Cod = " & CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO & " then D.Udm_Cod else tec.dett_cod end as udm_cod ")
        StrSQL.AppendLine("                     , tec.FF_Classe ")
        StrSQL.AppendLine("                     , dest.qta as Qta ")
        StrSQL.AppendLine("                     , Op.Validita_Inizio ")
        StrSQL.AppendLine("                     , spe.veg_cod ")
        StrSQL.AppendLine("                from   Agenda Op ")
        StrSQL.AppendLine("                inner  join Movimenti_dettagli D On Op.Id_Agenda = D.Id_Agenda ")
        StrSQL.AppendLine("                inner  join Mov_Dettaglio_Tecnico tec On Op.Id_Agenda = tec.Id_Agenda ")
        StrSQL.AppendLine("                                                     And D.id_mov = tec.id_mov ")
        StrSQL.AppendLine("                                                     And D.Id_Mov_Det = tec.Id_Mov_Det ")
        StrSQL.AppendLine("                inner  join mov_Destinazioni dest On dest.id_agenda = D.id_agenda ")
        StrSQL.AppendLine("                                                 And dest.id_mov_det = D.id_Mov_det ")
        StrSQL.AppendLine("                inner  join Reg_Impianti RegImp On Regimp.Piva = dest.Piva ")
        StrSQL.AppendLine("                                               And Regimp.sa_cod = dest.sa_cod ")
        StrSQL.AppendLine("                                               And Regimp.appezza = dest.appezza ")
        StrSQL.AppendLine("                                               And Regimp.id_reg = dest.id_destinazione ")
        StrSQL.AppendLine("                left   join cultivar cul on cul.cul_cod = RegImp.cul_cod ")
        StrSQL.AppendLine("                left   join specievegetali spe on spe.veg_cod = cul.veg_cod ")
        StrSQL.AppendLine("                where Op.inviato >= 0 ")
        AggiungiFiltroLavCodRilievi(StrSQL, TipologiaLayer_Cod, "Op.lav_cod")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub SQL_CTE_PrevisionaliAvv(ByRef StrSQL As StringBuilder,
                                        Optional addCommaToEnd As Boolean = False,
                                        Optional addWithToStart As Boolean = False)
        If addWithToStart Then
            StrSQL.Append("with ")
        End If
        StrSQL.AppendLine("	cte_previsionaliavv as ( ")
        StrSQL.AppendLine("                select	Piva ")
        StrSQL.AppendLine("                     , sa_cod ")
        StrSQL.AppendLine("                     , Appezza ")
        StrSQL.AppendLine("                     , id_reg ")
        StrSQL.AppendLine("                     , -1 as id_Agenda ")
        StrSQL.AppendLine("                     , 0 as Ricetta_Operazione_Cod ")
        StrSQL.AppendLine("                     , 113 as Lav_Cod ")
        StrSQL.AppendLine("                     , Av_Cod ")
        StrSQL.AppendLine("                     , udm_cod  --capire  ")
        StrSQL.AppendLine("                     , 0 as FF_Classe -- Per adesso no ")
        StrSQL.AppendLine("                     , Qta_Ril as QTA  --capire ")
        StrSQL.AppendLine("                     , CAST(srv.Data_Riferimento as date) as Validita_Inizio ")
        StrSQL.AppendLine("                     , d.veg_cod ")
        StrSQL.AppendLine("                from   ( ")
        StrSQL.AppendLine("                        select d.* ")
        StrSQL.AppendLine("                             , c.Veg_Cod ")
        StrSQL.AppendLine("                             , c.Cul_Cod ")
        StrSQL.AppendLine("                        from   DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo d ")
        StrSQL.AppendLine("                        inner  join Reg_Impianti i 	On i.PIVA = d.piva ")
        StrSQL.AppendLine("                                                  And i.SA_COD = d.sa_cod ")
        StrSQL.AppendLine("                                                  And i.APPEZZA = d.appezza ")
        StrSQL.AppendLine("                                                  And i.ID_REG = d.id_reg ")
        StrSQL.AppendLine("                        inner  join Cultivar c On c.Cul_Cod = i.CUL_COD ")
        StrSQL.AppendLine("                       ) d ")
        StrSQL.AppendLine("                inner  join DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV srv On d.tipo_sorgente = srv.tipo_sorgente ")
        StrSQL.AppendLine("                                                                                   And d.Stazione_Cod = srv.stazione_Cod ")
        'StrSQL.AppendLine("                inner  join modelliPrevisionali p On p.Mod_Cod = srv.Mod_Cod ")
        StrSQL.AppendLine(")" & IIf(addCommaToEnd, ", ", " "))
    End Sub

    Private Sub internal_SQL_InsertSelect_TableOpAvv(
        ByRef StrSQL As StringBuilder,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        )

        Dim datiFiltroTemporale = GetDatiFiltroTemporale(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, objParametri_Server)

        Dim avversita = My.Resources.AgronicaCoreGisDAL.Avversita
        StrSQL.AppendLine("   insert into @TableOp ")
        StrSQL.AppendLine("   select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, ")
        StrSQL.AppendLine("          '" & avversita & "§ ' + cast(av_cod as varchar) + '|' + descr, ")
        StrSQL.AppendLine("          null, av_cod ")
        StrSQL.AppendLine("   from ( ")
        StrSQL.AppendLine("         select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, ")
        StrSQL.AppendLine("                av_cod, STRING_AGG(UDM_DES + '§  ' + replace( cast(qta as varchar(100)), ',', '.'), '|') as descr ")
        StrSQL.AppendLine("         from ( ")
        StrSQL.AppendLine("               select '' as piva, 0 as sa_cod, 0 as appezza, 0 as id_reg, op.id_agenda, op.Ricetta_Operazione_Cod, ")
        StrSQL.AppendLine("                      mav.ordine,  mav.av_cod, ")
        StrSQL.AppendLine("                      ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') AS UDM_DES, ")
        StrSQL.AppendLine("                      op.Qta as Qta ")
        StrSQL.AppendLine("               from cte_avversita mav ")
        StrSQL.AppendLine("               inner join cte_ricettexop op on op.av_cod = mav.av_cod And op.udm_cod = mav.udm_cod ")
        AggiungiFiltroTemporale(StrSQL, True, "op.Validita_Inizio", datiFiltroTemporale)
        StrSQL.AppendLine("               UNION ALL ")
        StrSQL.AppendLine("               select op.piva, op.sa_Cod, op.appezza, op.id_reg, op.id_agenda, 0 As Ricetta_Operazione_Cod, ")
        StrSQL.AppendLine("                      mav.ordine, mav.av_cod, ")
        StrSQL.AppendLine("                      ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') + ' [' + ISNULL(sv.veg_des,'') + ']' AS UDM_DES, ")
        StrSQL.AppendLine("                      op.Qta as Qta ")
        StrSQL.AppendLine("               from cte_rilievi_113_109_169 mav ")
        StrSQL.AppendLine("               inner join ( ")
        StrSQL.AppendLine("			                select * from cte_agendaxop_113_109_169 ")
        StrSQL.AppendLine("			                union ")
        StrSQL.AppendLine("			                select * from cte_previsionaliavv ")
        StrSQL.AppendLine("			               ) op on op.av_cod = mav.av_cod ")
        StrSQL.AppendLine("			                   and op.veg_cod = mav.veg_cod ")
        StrSQL.AppendLine("			                   and op.udm_cod = mav.udm_cod ")
        StrSQL.AppendLine("			                   and op.lav_cod = mav.lav_cod ")
        StrSQL.AppendLine("               left join reg_impianti imp on imp.piva = op.piva ")
        StrSQL.AppendLine("                                         and imp.sa_cod = op.sa_cod ")
        StrSQL.AppendLine("                                         and imp.appezza = op.appezza ")
        StrSQL.AppendLine("                                         and imp.id_reg = op.id_reg ")
        StrSQL.AppendLine("               left join cultivar c on c.cul_cod = imp.cul_cod ")
        StrSQL.AppendLine("               left join specievegetali sv on sv.veg_cod = c.veg_cod ")
        AggiungiFiltroTemporale(StrSQL, True, "op.Validita_Inizio", datiFiltroTemporale)
        StrSQL.AppendLine("              ) tmp1 ")
        StrSQL.AppendLine("         group by piva, sa_cod, appezza, id_reg, id_agenda, ordine, ricetta_operazione_cod, av_cod ")
        StrSQL.AppendLine("   ) tmp2 ;")
    End Sub

    Private Sub internal_SQL_InsertSelect_TableOpVegProd(
    ByRef StrSQL As StringBuilder,
    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim datiFiltroTemporale = GetDatiFiltroTemporale(enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI, objParametri_Server)
        Dim rilieviVegetoProduttivi = "RilieviVegetoProduttivi" 
        StrSQL.AppendLine("   insert into @TableOp ")
        StrSQL.AppendLine("   select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, ")
        StrSQL.AppendLine("          '" & rilieviVegetoProduttivi & "§ ' + cast(av_cod as varchar) + '|' + descr, ")
        StrSQL.AppendLine("          null, av_cod ")
        StrSQL.AppendLine("   from ( ")
        StrSQL.AppendLine("         select piva, sa_cod, appezza, id_reg, id_agenda, ricetta_operazione_cod, ")
        StrSQL.AppendLine("                av_cod, STRING_AGG(UDM_DES + '§  ' + replace( cast(qta as varchar(100)), ',', '.'), '|') as descr ")
        StrSQL.AppendLine("         from ( ")
        StrSQL.AppendLine("               select '' as piva, 0 as sa_cod, 0 as appezza, 0 as id_reg, op.id_agenda, op.Ricetta_Operazione_Cod, ")
        StrSQL.AppendLine("                      mav.ordine,  mav.av_cod, ")
        StrSQL.AppendLine("                      ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') AS UDM_DES, ")
        StrSQL.AppendLine("                      op.Qta as Qta ")
        StrSQL.AppendLine("               from cte_avversita mav ")
        StrSQL.AppendLine("               inner join cte_ricettexop op on op.av_cod = mav.av_cod And op.udm_cod = mav.udm_cod ")
        AggiungiFiltroTemporale(StrSQL, True, "op.Validita_Inizio", datiFiltroTemporale)
        StrSQL.AppendLine("               UNION ALL ")
        StrSQL.AppendLine("               select op.piva, op.sa_Cod, op.appezza, op.id_reg, op.id_agenda, 0 As Ricetta_Operazione_Cod, ")
        StrSQL.AppendLine("                      mav.ordine, mav.av_cod, ")
        StrSQL.AppendLine("                      ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') AS UDM_DES, ")
        StrSQL.AppendLine("                      op.Qta as Qta ")
        StrSQL.AppendLine("               from cte_rilievi_113_109_169 mav ")
        StrSQL.AppendLine("               inner join ( ")
        StrSQL.AppendLine("			                select * from cte_agendaxop_113_109_169 ")
        StrSQL.AppendLine("			               ) op on op.FF_Classe = mav.av_cod ")
        StrSQL.AppendLine("			                   and op.udm_cod = mav.udm_cod ")
        StrSQL.AppendLine("			                   and op.lav_cod = mav.lav_cod ")
        StrSQL.AppendLine("               left join reg_impianti imp on imp.piva = op.piva ")
        StrSQL.AppendLine("                                         and imp.sa_cod = op.sa_cod ")
        StrSQL.AppendLine("                                         and imp.appezza = op.appezza ")
        StrSQL.AppendLine("                                         and imp.id_reg = op.id_reg ")
        AggiungiFiltroTemporale(StrSQL, True, "op.Validita_Inizio", datiFiltroTemporale)
        StrSQL.AppendLine("              ) tmp1 ")
        StrSQL.AppendLine("         group by piva, sa_cod, appezza, id_reg, id_agenda, ordine, ricetta_operazione_cod, av_cod ")
        StrSQL.AppendLine("   ) tmp2 ;")

    End Sub

    Private Sub AggiungiFiltroTemporale(ByRef StrSQL As StringBuilder,
                                        ByVal iniziaFiltroConWhere As Boolean,
                                        ByVal colonnaData As String,
                                        ByRef datiFiltroTemporale As DatiFiltroTemporale)

        Dim primoOperatore As String

        If iniziaFiltroConWhere Then
            primoOperatore = " where "
        Else
            primoOperatore = " and "
        End If

        StrSQL.AppendLine(primoOperatore & colonnaData & " >= " & Agro_SQL_SaveDate(datiFiltroTemporale.finestraTemporaleInizio))
        StrSQL.AppendLine(" and   " & colonnaData & " <= " & Agro_SQL_SaveDate(datiFiltroTemporale.finestraTemporaleFine))

    End Sub

    Public Function LeggiEntitaImpianti(
            ByVal Piva As String,
            ByVal SaCod As Integer,
            ByVal CampoCod As Integer,
            ByVal ValiditaInizio As DateTime,
            ByVal ValiditaFine As DateTime,
            ByVal VegCod As Integer,
            ByVal DestinazioneUso As Integer,
            ByRef objParametri As AgronicaCoreParametri
        ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiEntitaImpianti("
        Dim MessaggioErrore As String = ""

        Dim terrenoNudo = False

        If VegCod = 0 AndAlso DestinazioneUso = 0 Then
            terrenoNudo = True
        End If

        Try
            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" SELECT ")
            stb.AppendLine("   entita.Entita_Cod ")
            stb.AppendLine(" , entita.Piva ")
            stb.AppendLine(" , entita.Sa_Cod ")
            stb.AppendLine(" , entita.Appezza ")
            stb.AppendLine(" , entita.Id_Imp ")
            stb.AppendLine(" FROM GIS_Entita entita ")
            ' Join impianti
            stb.AppendLine(" INNER JOIN Reg_Impianti impianti ON ")
            stb.AppendLine("            impianti.PIVA = entita.Piva AND ")
            stb.AppendLine("            impianti.SA_COD = entita.Sa_Cod AND ")
            stb.AppendLine("            impianti.APPEZZA = entita.Appezza AND ")
            stb.AppendLine("            impianti.ID_REG = entita.Id_Imp ")
            ' Join appezzamenti
            stb.AppendLine(" INNER JOIN Appezzamento appezza ON ")
            stb.AppendLine("            appezza.PIVA = entita.Piva AND ")
            stb.AppendLine("            appezza.SA_COD = entita.Sa_Cod AND ")
            stb.AppendLine("            appezza.APPEZZA = entita.Appezza ")
            ' Join esercizi
            stb.AppendLine(" INNER JOIN Imprese_Progetti esercizi ON ")
            stb.AppendLine("            esercizi.Piva = impianti.PIVA AND ")
            stb.AppendLine("            esercizi.Sa_Cod = impianti.SA_COD AND ")
            stb.AppendLine("            esercizi.Appezza = impianti.APPEZZA AND ")
            stb.AppendLine("            esercizi.Id_Reg = impianti.ID_REG ")
            ' Join impianti codici
            If DestinazioneUso <> 0 OrElse terrenoNudo Then
                stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici impianticodici ON ")
                stb.AppendLine("           impianticodici.PIVA = impianti.PIVA AND ")
                stb.AppendLine("           impianticodici.SA_COD = impianti.SA_COD AND ")
                stb.AppendLine("           impianticodici.APPEZZA = impianti.APPEZZA AND ")
                stb.AppendLine("           impianticodici.ID_REG = impianti.ID_REG ")
            End If
            ' Where
            stb.AppendLine(" WHERE entita.TipoEntita_Cod IN (19,20,21,22,23) ") ' => Impianti
            stb.AppendLine(" AND   esercizi.Validita_Inizio <= " & Agro_SQL_SaveDate(ValiditaFine) & " ")
            stb.AppendLine(" AND   esercizi.Validita_Fine >= " & Agro_SQL_SaveDate(ValiditaInizio) & " ")

            If Piva <> "" Then
                stb.AppendLine(" AND   impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If SaCod <> 0 Then
                stb.AppendLine(" AND   impianti.SA_COD = " & Agro_SQL_SaveNum(SaCod) & " ")
            End If

            If CampoCod <> 0 Then
                stb.AppendLine(" AND   appezza.CAMPO_COD = " & Agro_SQL_SaveNum(CampoCod) & " ")
            End If

            Select Case True
                Case terrenoNudo
                    stb.AppendLine(" AND   impianti.CUL_COD = 0 ")
                    stb.AppendLine(" AND   impianticodici.ID_COD IS NULL ")
                Case VegCod <> 0
                    stb.AppendLine(" AND   impianti.CUL_COD IN (SELECT CUL_COD FROM Cultivar WHERE Veg_Cod = " & Agro_SQL_SaveNum(VegCod) & ") ")
                Case DestinazioneUso <> 0
                    stb.AppendLine(" AND   impianticodici.ID_COD = " & Agro_SQL_SaveNum(DestinazioneUso) & " ")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiEntitaImpianti_FiltroTemporale(
            ByVal Piva As String,
            ByVal SaCod As Integer,
            ByVal validita As DateTime,
            ByVal op_LimInferiore As String,
            ByVal op_LimSuperiore As String,
            ByRef objParametri As AgronicaCoreParametri
        ) As DataTable

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiEntitaImpianti_FiltroTemporale()"
        Dim MessaggioErrore As String = ""

        Dim terrenoNudo = False

        Try
            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" SELECT DISTINCT")
            'stb.AppendLine("   entita.Entita_Cod ")
            'stb.AppendLine(" , entita.Piva ")
            'stb.AppendLine(" , entita.Sa_Cod ")
            'stb.AppendLine(" , entita.Appezza ")
            'stb.AppendLine(" , entita.Id_Imp ")
            stb.AppendLine("   impianti.Validita_Inizio ")
            stb.AppendLine(" , impianti.Validita_Fine ")
            stb.AppendLine(" FROM GIS_Entita entita ")

            stb.AppendLine(" INNER JOIN Reg_Impianti impianti ON ")
            stb.AppendLine("            impianti.PIVA = entita.Piva AND ")
            stb.AppendLine("            impianti.SA_COD = entita.Sa_Cod AND ")
            stb.AppendLine("            impianti.APPEZZA = entita.Appezza AND ")
            stb.AppendLine("            impianti.ID_REG = entita.Id_Imp ")

            stb.AppendLine(" INNER JOIN GIS_TipoEntita t ON ")
            stb.AppendLine("            entita.tipoEntita_cod = t.tipoEntita_Cod ")

            stb.AppendLine("WHERE t.LayerElementiGrafici_Cod = 19 ")

            If Piva <> "" Then
                stb.AppendLine(" AND   impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If SaCod <> 0 Then
                stb.AppendLine(" AND   impianti.SA_COD = " & Agro_SQL_SaveNum(SaCod) & " ")
            End If

            If validita <> Nothing Then
                stb.AppendLine(" AND   impianti.Validita_Inizio " & op_LimInferiore & Agro_SQL_SaveDate(validita) & " ")
                stb.AppendLine(" AND   impianti.Validita_Fine " & op_LimSuperiore & Agro_SQL_SaveDate(validita) & " ")
            End If

            stb.AppendLine("ORDER BY Validita_Inizio DESC")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

#End Region

    Public Function LeggiWKT_xAnalisiDemetra(Piva As String,
                                             ByRef lat As Double?,
                                             ByRef lang As Double?,
                                             LayerElementiGrafici_Cod As enum_Gis_LayerElementiGrafici_std,
                                             ByRef objParametri As AgronicaCoreParametri,
                                                 Optional Sa_Cod As Integer = 0,
                                                 Optional Appezza As Integer = 0,
                                                 Optional Id_Imp As Integer = 0,
                                                 Optional PROV As String = "",
                                                 Optional COM As String = "",
                                                 Optional SEZIONE As String = "",
                                                 Optional FOGLIO As Integer = 0,
                                                 Optional NUMERO As Integer = 0,
                                                 Optional SUBALTERNO As String = ""
                                             ) As Boolean

        Dim DT As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiWKTCentroxAnalisiDemetra"
        Dim MessaggioErrore As String = ""

        Dim found As Boolean = False

        Try
            Dim stb As New System.Text.StringBuilder

            stb.AppendLine(" SELECT TOP 1 g.Poligono_GeoEntity.EnvelopeCenter().STAsText() AS WKT ")
            stb.AppendLine(" , lat = TRIM(SUBSTRING(TRIM(REPLACE(REPLACE(g.Poligono_GeoEntity.EnvelopeCenter().STAsText(), 'POINT (', ''), ')', '') ), ")
            stb.AppendLine("         CHARINDEX(' ', TRIM(REPLACE(REPLACE(g.Poligono_GeoEntity.EnvelopeCenter().STAsText(), 'POINT (', ''), ')', '') )  ")
            stb.AppendLine("         , 0), LEN(g.Poligono_GeoEntity.EnvelopeCenter().STAsText()) ) )   ")

            stb.AppendLine(" , lang = SUBSTRING( TRIM( REPLACE(REPLACE(g.Poligono_GeoEntity.EnvelopeCenter().STAsText(), 'POINT (', ''), ')', '') )  ")
            stb.AppendLine("          , 0, LEN(TRIM( REPLACE(REPLACE(g.Poligono_GeoEntity.EnvelopeCenter().STAsText(), 'POINT (', ''), ')', '') )) - CHARINDEX(' ',  ")
            stb.AppendLine("          TRIM(REPLACE(REPLACE(g.Poligono_GeoEntity.EnvelopeCenter().STAsText(), 'POINT (', ''), ')', '') ), 0 ) ) ")

            stb.AppendLine(" FROM GIS_Entita e ")
            stb.AppendLine(" INNER JOIN GIS_ElementiGrafici g ON e.Entita_Cod = g.Entita_Cod")

            stb.AppendLine(" WHERE e.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                stb.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Imp <> 0 Then
                stb.AppendLine(" AND Id_Imp = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                stb.AppendLine(" AND PROV = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                stb.AppendLine(" AND COM = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                stb.AppendLine(" AND SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "' ")
            End If
            If FOGLIO <> 0 Then
                stb.AppendLine(" AND FOGLIO = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                stb.AppendLine(" AND NUMERO = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                stb.AppendLine(" AND SUBALTERNO = '" & Agro_SQL_SaveText(SUBALTERNO) & "' ")
            End If

            stb.AppendLine(" AND g.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                lat = If(IsDBNull(DT.Rows(0).Item("lat")), Nothing, CDec(DT.Rows(0).Item("lat").replace(".", ",")))
                lang = If(IsDBNull(DT.Rows(0).Item("lang")), Nothing, CDec(DT.Rows(0).Item("lang").replace(".", ",")))

                found = True
            End If
        Catch ex As Exception

        End Try

        Return found

    End Function

    Public Function LeggiAziendeMappa_Visibilita(wkt As String,
                                               filtro_Visibilita_Utente As Boolean,
                                               TipoSelect As enum_TipoSelect_FiltroneSuperNova,
                                               ByRef classJoin As JoinFiltrone,
                                               objParametri As AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim stb As New System.Text.StringBuilder
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiAziendeMappa_Visibilita()"
        Dim MessaggioErrore As String = ""

        Try

            stb.Length = 0

            stb.AppendLine("Declare @UsernameCollegato varchar(max)")
            stb.AppendLine("Set @UsernameCollegato = '" & Agro_SQL_SaveText(objParametri.UtenteUsername, False) & "' ")
            stb.AppendLine("Declare @t varchar(max)")
            stb.AppendLine("Set @t = '" & Agro_SQL_SaveText(wkt, False) & "' ")
            stb.AppendLine("Declare @g geography")
            stb.AppendLine("Set @g = GEOGRAPHY::STGeomFromText(@t, 4326)")
            stb.AppendLine(";")

            SQL_CTE_Imprese_Filtrone(stb, "", TipoSelect, " ", classJoin, objParametri, False, True)

            stb.AppendLine("SELECT distinct i.PIVA, i.rag_soc, i.TipoImpresaGerarchia, i.CodiceCuaa, i.Validita_Inizio, i.Validita_Fine, i.Padre")
            stb.AppendLine(", i.Livello, i.Foglia, i.PROV, i.PROVINCIA, i.REG, i.REGIONE, i.stato")
            stb.AppendLine("FROM gis_entita e")

            stb.AppendLine("INNER JOIN gis_elementiGrafici g")
            stb.AppendLine("ON e.entita_Cod = g.entita_Cod")

            If (filtro_Visibilita_Utente) Then
                stb.AppendLine("INNER JOIN Utenti_Visibilita_Appoggio app")
                stb.AppendLine("ON app.piva = e.piva")
                stb.AppendLine("AND app.Username = @UsernameCollegato")
                stb.AppendLine("AND app.entita_Cod = 1")
            End If

            stb.AppendLine("INNER JOIN cte_imprese i")
            stb.AppendLine("ON i.piva = e.piva ")

            stb.AppendLine("WHERE ( g.LayerElementiGrafici_Cod = " & enum_Gis_LayerElementiGrafici_std.IMPIANTI & "")
            stb.AppendLine("OR g.LayerElementiGrafici_Cod = " & enum_Gis_LayerElementiGrafici_std.Centri_Aziendali & " )")
            stb.AppendLine("AND g.Poligono_GeoEntity.STIntersects(@g) = 1")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function


    Private Class DatiFiltroTemporale
        Public operatoreLimiteInferiore As String
        Public operatoreLimiteSuperiore As String
        Public finestraTemporaleInizio As Date
        Public finestraTemporaleFine As Date
    End Class

    Public Function LeggiXDocumentExportSHP_List( ' the '§' logic will be maintained
        ByVal layerElementiGrafici_cod As Integer,
        ByVal fullLayerExport As Boolean,
        ByVal applicaFiltroTabellaUtentiVisibilitaAppoggio As Boolean,
        ByVal idEsp As Integer,
        ByVal codiceFiscaleTecnico As String,
        ByVal isImpianto As Boolean,
        ByVal usaCFTecnico As Boolean,
        ByRef objParametri As AgronicaCoreParametri,
        Optional username As String = ""
    ) As XDocument
        Dim resp As XDocument
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiXDocumentExportSHP_List()"
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            If Not isImpianto Then
                StrSQL.AppendLine("     case when ISNULL(CHARINDEX('§', ElementoGrafico_Des), 0) > 0 ")
                StrSQL.AppendLine("          THEN ElementoGrafico_Des ")
                StrSQL.AppendLine("          ELSE 'Descrizione§ ' + ElementoGrafico_Des ")
                StrSQL.AppendLine("     END as '@text' ")
            Else
                StrSQL.AppendLine(" isnull(    'CA_ID§ ' + ri.piva + '-' + cast(ri.sa_Cod as varchar(25)) + '-' + cast(ri.appezza as varchar(25)) + '-' + cast(ri.id_reg as varchar(25))  ")
                StrSQL.AppendLine("         + '|CA_PIVA§ ' + coalesce(ca.piva, '') ")
                StrSQL.AppendLine("         + '|CA_RagSo§ ' + coalesce(imp.rag_soc, '') ")
                StrSQL.AppendLine("         + '|CA_Nome§ ' +  coalesce(ca.sa_nome, '') ")
                StrSQL.AppendLine("         + '|CA_Indir§ ' + coalesce(i.ind_des, '') ")
                StrSQL.AppendLine("         + '|CA_Com§ ' +   coalesce(ist.localita , '') ")
                StrSQL.AppendLine("         + '|CA_CAP§ ' +   coalesce(i.CAP , '') ")
                StrSQL.AppendLine("         + '|CA_Prov§ ' +  coalesce(ist.comuni_prov, '') ")
                StrSQL.AppendLine("         + '|CA_Stato§ ' + coalesce(i.stato , '') ")
                StrSQL.AppendLine("         + '|App_Nome§ ' + coalesce(a.APP_NOME , '') ")
                StrSQL.AppendLine("         + '|App_Sup§ ' +  cast(a.SUP_APP  as varchar(50)) ")
                StrSQL.AppendLine("         + '|Imp_Sup§ ' +  cast(ri.Sup_Imp as varchar(50)) ")
                StrSQL.AppendLine("         + '|Tip_Var§ ' +  ")
                StrSQL.AppendLine("         Case when ri.GRVA_Cod_VEG < 0 ")
                StrSQL.AppendLine("              then gv.grva_des + ' - ibrido' ")
                StrSQL.AppendLine("              Else gv.grva_des ")
                StrSQL.AppendLine("         End ")
                StrSQL.AppendLine("         + '|Spec_Veg§ ' +  coalesce(sv.Veg_Des, '') ")
                StrSQL.AppendLine("         + '|Cultivar§ ' +  coalesce(cul.Cul_Des, '') ")
                StrSQL.AppendLine("         + '|Sup_Poly§ ' +  cast(geg.Poligono_GeoEntity.STArea() as varchar(50)) ")
                StrSQL.AppendLine("         + '|N_Lotto§ ' +  coalesce(ic.val_Cod, '') ")
                StrSQL.AppendLine("         + '|Note§ ' +  ' ' ")
                StrSQL.AppendLine("         + '|Coord§ ' + cast( round( Centro.Lat, 6) as varchar(100)) + ' ' + cast( round( Centro.Long, 6) as varchar(100)) , '') ")
                StrSQL.AppendLine(" As '@text' ")
            End If

            StrSQL.AppendLine(" , (select geodata1.Poligono_GeoEntity.AsGml() as geodata ")
            StrSQL.AppendLine("    From GIS_ElementiGrafici geodata1 ")
            StrSQL.AppendLine("    Where (geodata1.entita_cod = geg.entita_cod) ")
            StrSQL.AppendLine("   ) As geodata ")

            StrSQL.AppendLine(" From ")

            If Not isImpianto Then
                StrSQL.AppendLine("    gis_entita e ")
                StrSQL.AppendLine("    Inner Join GIS_ElementiGrafici geg ")
                StrSQL.AppendLine("         On e.entita_cod = geg.Entita_Cod ")
                If Not fullLayerExport Then
                    StrSQL.AppendLine(String.Format(" Where e.Entita_Cod in (SELECT Entita_Cod FROM EntitaXEsportazioni WHERE IdEsp = {0}) ", idEsp))
                Else
                    StrSQL.AppendLine(" WHERE geg.layerElementiGrafici_cod = " & layerElementiGrafici_cod)
                End If
            Else
                StrSQL.AppendLine(" ( ")
                StrSQL.AppendLine("     select ri.* From Reg_Impianti ri ")

                If applicaFiltroTabellaUtentiVisibilitaAppoggio Then
                    StrSQL.AppendLine("    inner Join utenti_Visibilita_Appoggio appv")
                    StrSQL.AppendLine($"    On appv.username = '{If(Not String.IsNullOrEmpty(username), username, Agro_SQL_SaveText(objParametri.UtenteUsername))}'")
                    StrSQL.AppendLine("    And appv.entita_cod = 1")
                    StrSQL.AppendLine("    And appv.piva = ri.piva ")
                End If

                If usaCFTecnico Then
                    StrSQL.AppendLine(String.Format("   Where ri.CODICE_FISCALE_TECNICO = '{0}' ", Agro_SQL_SaveText(codiceFiscaleTecnico)))
                End If

                StrSQL.AppendLine(" ) ri ")
                StrSQL.AppendLine(" INNER Join Centri_Aziendali ca ")
                StrSQL.AppendLine("     On ri.piva = ca.piva ")
                StrSQL.AppendLine("     And ri.SA_COD = ca.sa_cod ")

                StrSQL.AppendLine(" INNER Join Imprese imp ")
                StrSQL.AppendLine("     On imp.PIVA = ca.PIVA ")

                StrSQL.AppendLine(" Left Join Imprese_Codici ic ")
                StrSQL.AppendLine("     On ic.PIVA = ca.PIVA ")
                StrSQL.AppendLine("     And ic.id_cod = 1033 ")

                StrSQL.AppendLine(" Left Join CentrixIndirizzi CInd ")
                StrSQL.AppendLine("     On ca.PIVA = CInd.PIVA ")
                StrSQL.AppendLine("     And ca.sa_cod = CInd.SA_COD ")

                StrSQL.AppendLine(" Left Join Indirizzi i ")
                StrSQL.AppendLine("     On CInd.cod_indirizzo = i.cod_indirizzo ")

                StrSQL.AppendLine(" Left Join ISTAT ist ")
                StrSQL.AppendLine("     On i.pro_cod_istat = ist.PROV ")
                StrSQL.AppendLine("     And i.com_cod_istat = ist.COM ")

                StrSQL.AppendLine(" INNER Join Appezzamento a ")
                StrSQL.AppendLine("     On ri.piva = a.piva ")
                StrSQL.AppendLine("     And ri.sa_cod = a.sa_cod ")
                StrSQL.AppendLine("     And ri.appezza = a.appezza ")

                StrSQL.AppendLine(" INNER Join ( ")
                StrSQL.AppendLine("     SELECT 0 as Cul_Cod, 0 as Veg_Cod, '' as Cul_des ")
                StrSQL.AppendLine("     UNION ")
                StrSQL.AppendLine("     Select cul.Cul_Cod, cul.Veg_Cod, cul.Cul_Des")
                StrSQL.AppendLine("     From Cultivar cul ")
                StrSQL.AppendLine(" ) cul ")
                StrSQL.AppendLine("     On cul.Cul_Cod = ri.CUL_COD ")

                StrSQL.AppendLine(" INNER Join ( ")
                StrSQL.AppendLine("     SELECT 0 as  Grva_Cod, 'non specificato' as  Grva_des ")
                StrSQL.AppendLine("     UNION ")
                StrSQL.AppendLine("     Select gv.Grva_Cod, gv.Grva_Des ")
                StrSQL.AppendLine("     From GruppoVarietale gv ")
                StrSQL.AppendLine(" ) gv ")
                StrSQL.AppendLine("     On gv.Grva_Cod = abs(ri.GRVA_Cod_VEG) ")

                StrSQL.AppendLine(" INNER Join SpecieVegetali sv ")
                StrSQL.AppendLine("     On sv.Veg_Cod = cul.Veg_Cod ")

                StrSQL.AppendLine(" INNER Join gis_entita e ")
                StrSQL.AppendLine("     On ri.piva = e.Piva ")
                StrSQL.AppendLine("     And ri.sa_cod = e.Sa_Cod ")
                StrSQL.AppendLine("     And ri.APPEZZA = e.Appezza ")
                StrSQL.AppendLine("     And ri.ID_REG = e.Id_Imp ")

                If Not fullLayerExport Then
                    StrSQL.AppendLine(String.Format(" And e.Entita_Cod in (SELECT Entita_Cod FROM EntitaXEsportazioni WHERE IdEsp = {0}) ", idEsp))
                End If

                StrSQL.AppendLine(" INNER Join GIS_ElementiGrafici geg ")
                StrSQL.AppendLine("     On e.entita_cod = geg.Entita_Cod ")


                StrSQL.AppendLine(" INNER Join ( ")
                StrSQL.AppendLine("     Select entita_cod, Poligono_GeoEntity.EnvelopeCenter() as Centro ")
                StrSQL.AppendLine("     From gis_elementigrafici ")
                StrSQL.AppendLine(" ) aEvelope ")
                StrSQL.AppendLine("     On aEvelope.entita_cod = geg.Entita_Cod ")

                StrSQL.AppendLine(" WHERE geg.layerElementiGrafici_cod = 19 ")

                'StrSQL.AppendLine(" AND e.piva not in ('ID101400125') ") 'Dati sporchi su Costa d'avorio ...

            End If

            StrSQL.AppendLine(" For Xml path('Entita'), root('DatiEntita'), elements ")
            '--------------------------------------------------------------------------
            resp = EseguiQuery_Lettura_XDoc(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp
    End Function

    ''' <summary>
    ''' Legge gli impianti associati all'appezzamento relativi agli ultimi N anni agronomici.
    ''' Esclude l'impianto identificato da id_imp e filtra per Validita_Fine compresa
    ''' negli ultimi years_to_consider anni (a partire dal 1° gennaio dell'anno corrente - years_to_consider).
    ''' DS22-BL: LetturaImpiantiAppezzamento
    ''' </summary>
    Public Function LeggiImpiantiAppezzamentoUltimiAnni(piva As String,
                                                         saCod As Integer,
                                                         appezza As Integer,
                                                         idImp As Integer,
                                                         yearsToConsider As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim dataTable As DataTable
        Const routine = "AgronicaCoreGisDAL.GIS_Entita_R.LeggiImpiantiAppezzamentoUltimiAnni"

        Try
            Dim strSql As New StringBuilder
            strSql.Length = 0

            strSql.AppendLine(" SELECT b.PIVA ")
            strSql.AppendLine("      , b.SA_COD ")
            strSql.AppendLine("      , b.APPEZZA ")
            strSql.AppendLine("      , b.ID_REG ")
            strSql.AppendLine("      , b.Validita_Inizio ")
            strSql.AppendLine("      , b.Validita_Fine ")
            strSql.AppendLine("      , b.CUL_COD ")
            strSql.AppendLine("      , c.Veg_Cod ")
            strSql.AppendLine(" FROM Appezzamento a ")
            strSql.AppendLine(" INNER JOIN Reg_Impianti b ON a.piva = b.piva AND a.SA_COD = b.SA_COD AND a.appezza = b.APPEZZA ")
            strSql.AppendLine(" INNER JOIN Cultivar c ON c.Cul_Cod = b.CUL_COD ")
            strSql.AppendLine(String.Format(" WHERE b.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
            strSql.AppendLine(String.Format(" AND b.Sa_Cod = {0} ", Agro_SQL_SaveNum(saCod)))
            strSql.AppendLine(String.Format(" AND b.Appezza = {0} ", Agro_SQL_SaveNum(appezza)))
            strSql.AppendLine(String.Format(" AND b.ID_REG != {0} ", Agro_SQL_SaveNum(idImp)))
            strSql.AppendLine(String.Format(" AND b.Validita_Fine >= DATEADD(YEAR, -{0}, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) ", Agro_SQL_SaveNum(yearsToConsider)))
            strSql.AppendLine(" ORDER BY b.Validita_Fine DESC ")

            '--------------------------------------------------------------------------
            dataTable = EseguiQuery_Lettura(objParametri, strSql.ToString, routine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim errorMessage = ex.Message
            Scrivi_LOG(objParametri, routine, errorMessage)
            Throw New Exception("[" & routine & "] : " & errorMessage)

        End Try

        Return dataTable
    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_Entita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function PlanningRibaltaFast(
        ByVal EntitaCodNuovo As Integer,
        ByVal programmazione_Entita_Cod As Integer,
        ByVal Piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        ByVal id_reg As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False




        Try
            '---------------------------------------------
            stb.Length = 0


            stb.AppendLine("  insert [dbo].[GIS_Entita]( ")
            stb.AppendLine("                 PivaSuperUser, ")
            stb.AppendLine("                 Entita_Cod, ")
            stb.AppendLine("                 TipoEntita_Cod, ")
            stb.AppendLine("                 Piva, ")
            stb.AppendLine("                 sa_cod, ")
            stb.AppendLine("                 appezza, ")
            stb.AppendLine("                 Campo_Cod, ")
            stb.AppendLine("                 Id_Imp, ")
            stb.AppendLine("                 PROV, ")
            stb.AppendLine("                 COM, ")
            stb.AppendLine("                 SEZIONE, ")
            stb.AppendLine("                 FOGLIO, ")
            stb.AppendLine("                 NUMERO, ")
            stb.AppendLine("                 SUBALTERNO, ")
            stb.AppendLine("                 programmazione_Entita_Cod, ")
            stb.AppendLine("                 ID_Agenda, ")
            stb.AppendLine("                 id_mov_det, ")
            stb.AppendLine("                 Ricetta_Operazione_cod, ")
            stb.AppendLine("                 analisi_campione_cod, ")
            stb.AppendLine("                 OLDGrafica_ID, ")
            stb.AppendLine("                 inviato, ")
            stb.AppendLine("                 datainvio, ")
            stb.AppendLine("                 Data_Creazione, ")
            stb.AppendLine("                 Data_Modifica, ")
            stb.AppendLine("                 Username_Creazione, ")
            stb.AppendLine("                 Username_Modifica, ")
            stb.AppendLine("                 Validita_Inizio, ")
            stb.AppendLine("                 Validita_Fine, ")
            stb.AppendLine("                 programmazione_cod ")
            stb.AppendLine("             ) ")

            stb.AppendLine(" Select top 1   ")
            stb.AppendLine("     PivaSuperUser, ")
            stb.AppendLine("     " & EntitaCodNuovo & " As Entita_Cod, ")

            stb.AppendLine("  Case When veg.Gru_Cod Is null Then 21 Else ")
            stb.AppendLine("  Case When veg.Gru_Cod = 1 Then 22 Else   ")
            stb.AppendLine("  Case When veg.Gru_Cod = 2 Then 20 Else   ")
            stb.AppendLine("  Case When veg.Gru_Cod = 3 Then 19 Else 23    ")
            stb.AppendLine("  End End End End As Tipoentita_Cod, ")

            stb.AppendLine("     '" & Agro_SQL_SaveText(Piva) & "' as Piva, ")
            stb.AppendLine("     " & sa_cod & " as sa_cod, ")
            stb.AppendLine("     " & appezza & " as appezza, ")
            stb.AppendLine("     0 as Campo_Cod, ")
            stb.AppendLine("     " & id_reg & " as Id_Imp, ")
            stb.AppendLine("     e.PROV, ")
            stb.AppendLine("     e.COM, ")
            stb.AppendLine("     e.SEZIONE, ")
            stb.AppendLine("     e.FOGLIO, ")
            stb.AppendLine("     e.NUMERO, ")
            stb.AppendLine("     e.SUBALTERNO, ")
            stb.AppendLine("     0 As Programmazione_Entita_Cod,  ")
            stb.AppendLine("     e.ID_Agenda, ")
            stb.AppendLine("     e.id_mov_det, ")
            stb.AppendLine("     e.Ricetta_Operazione_cod, ")
            stb.AppendLine("     e.analisi_campione_cod, ")
            stb.AppendLine("     e.OLDGrafica_ID, ")
            stb.AppendLine("     e.inviato, ")
            stb.AppendLine("     e.datainvio, ")
            stb.AppendLine("     " & Agro_SQL_SaveDate(Now) & " as Data_Creazione, ")
            stb.AppendLine("     " & Agro_SQL_SaveDate(Now) & " as Data_Modifica, ")
            stb.AppendLine("     e.Username_Creazione, ")
            stb.AppendLine("     e.Username_Modifica, ")
            stb.AppendLine("     e.Validita_Inizio, ")
            stb.AppendLine("     e.Validita_Fine, ")
            stb.AppendLine("      0 as programmazione_cod ")
            stb.AppendLine("  ")
            stb.AppendLine("   From GIS_Entita e  ")
            stb.AppendLine("  inner Join Programmazione_Entita ent ")
            stb.AppendLine("         On e.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine("  Left Join Cultivar cul  ")
            stb.AppendLine("         On cul.Cul_Cod = ent.Cul_Cod ")
            stb.AppendLine("  ")
            stb.AppendLine("  Left Join SpecieVegetali veg ")
            stb.AppendLine("         On veg.Veg_Cod = cul.Veg_Cod")


            stb.AppendLine(" Where ent.programmazione_Entita_Cod = " & programmazione_Entita_Cod)



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Scrivi(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal Programmazione_cod As Integer,
                            ByVal Programmazione_entita_cod As Integer,
                            ByVal ID_Agenda As Integer,
                            ByVal id_mov_det As Integer,
                            ByVal Ricetta_Operazione_Cod As Integer,
                            ByVal analisi_campione_cod As Integer,
                            ByVal OLDGrafica_ID As String,
                            ByVal DatoImportato As Int16,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = "",
                            Optional ByVal GIS_Allegato_Cod As Int32 = 0,
                            Optional ByVal Area_Cod As Int32 = 0,
                            Optional ByVal Entita_GUID As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO GIS_Entita ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    PivaSuperUser,       Entita_Cod,    ")
            StrSQL.Append("                    TipoEntita_Cod,      Piva,    ")
            StrSQL.Append("                    Sa_Cod,              Appezza,    ")
            StrSQL.Append("                    Campo_Cod,           Id_Imp,   ")
            StrSQL.Append("                    Prov,                Com,    ")
            StrSQL.Append("                    Sezione,             Foglio,    ")
            StrSQL.Append("                    Numero,              Subalterno,    ")
            StrSQL.Append("                   Programmazione_cod,  Programmazione_entita_cod,              ID_Agenda, id_mov_det,  Ricetta_Operazione_Cod, analisi_campione_cod, OLDGrafica_ID,  ")

            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")

            If GIS_Allegato_Cod <> 0 Then
                StrSQL.Append("                , GIS_Allegati_Documenti_Cod ")
            End If

            If Area_Cod <> 0 Then
                StrSQL.Append("                , Area_Cod ")
            End If

            If Not Entita_GUID.Equals("") Then
                StrSQL.Append("                , Entita_GUID ")
            End If

            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")

            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Entita_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TipoEntita_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Imp) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Prov) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Com) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sezione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Foglio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Subalterno) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Programmazione_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Programmazione_entita_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(id_mov_det) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(analisi_campione_cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(OLDGrafica_ID) & "'  ")

            StrSQL.Append("         , " & DatoImportato & "  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If GIS_Allegato_Cod <> 0 Then
                StrSQL.Append("     , " & Agro_SQL_SaveNum(GIS_Allegato_Cod) & " ")
            End If

            If Area_Cod <> 0 Then
                StrSQL.Append("     , " & Agro_SQL_SaveNum(Area_Cod) & " ")
            End If

            If Not Entita_GUID.Equals("") Then
                StrSQL.Append("     , '" & Agro_SQL_SaveText(Entita_GUID) & "' ")
            End If

            StrSQL.Append(" )")
            '---------------------------------------------

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



    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Modifica(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32?,
                            ByVal TipoEntita_Cod As Int32?,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32?,
                            ByVal Appezza As Int32?,
                            ByVal Campo_Cod As Int32?,
                            ByVal ID_Imp As Int32?,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32?,
                            ByVal Numero As Int32?,
                            ByVal Subalterno As String,
                            ByVal Programmazione_Cod As Integer?,
                            ByVal Programmazione_entita_Cod As Integer?,
                            ByVal ID_Agenda As Integer?,
                            ByVal Ricetta_Operazione_Cod As Integer?,
                                    ByVal Validita_Inizio As Date?,
                                    ByVal Validita_Fine As Date?,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser Is Nothing OrElse PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Entita_Cod Is Nothing OrElse Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Entita_Cod obbligatorio)")
            End If





            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If



            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_Entita SET ")

            StrSQL.Append("   Data_Modifica     =  " & Agro_SQL_SaveDate(Data_modifica))
            If Not IsNothing(TipoEntita_Cod) AndAlso TipoEntita_Cod <> 0 Then StrSQL.Append("   ,[TipoEntita_Cod] =  " & Agro_SQL_SaveNum(TipoEntita_Cod) & " ")
            If Not IsNothing(Piva) AndAlso Piva <> "" Then StrSQL.Append("   ,[Piva]           = '" & Agro_SQL_SaveText(Piva) & "'")
            If Not IsNothing(Sa_Cod) AndAlso Sa_Cod <> 0 Then StrSQL.Append("   ,[Sa_Cod]         =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            If Not IsNothing(Appezza) AndAlso Appezza <> 0 Then StrSQL.Append("   ,[Appezza]        =  " & Agro_SQL_SaveNum(Appezza) & " ")
            If Not IsNothing(Campo_Cod) AndAlso Campo_Cod <> 0 Then StrSQL.Append("   ,[Campo_Cod]      =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            If Not IsNothing(ID_Imp) AndAlso ID_Imp <> 0 Then StrSQL.Append("   ,[Id_Imp]         =  " & Agro_SQL_SaveNum(ID_Imp) & " ")
            If Not IsNothing(Prov) AndAlso Prov <> "0" Then StrSQL.Append("   ,[Prov]           = '" & Agro_SQL_SaveText(Prov) & "'")
            If Not IsNothing(Com) AndAlso Com <> "0" Then StrSQL.Append("   ,[Com]            = '" & Agro_SQL_SaveText(Com) & "'")
            If Not IsNothing(Sezione) AndAlso Sezione <> "-1" Then StrSQL.Append("   ,[Sezione]        = '" & Agro_SQL_SaveText(Sezione) & "'")
            If Not IsNothing(Foglio) AndAlso Foglio <> -1 Then StrSQL.Append("   ,[Foglio]         =  " & Agro_SQL_SaveNum(Foglio) & " ")
            If Not IsNothing(Numero) AndAlso Numero <> -1 Then StrSQL.Append("   ,[Numero]         =  " & Agro_SQL_SaveNum(Numero) & " ")
            If Not IsNothing(Subalterno) AndAlso Subalterno <> "-1" Then StrSQL.Append("   ,[Subalterno]     = '" & Agro_SQL_SaveText(Subalterno) & "'")
            If Not IsNothing(Programmazione_Cod) AndAlso Programmazione_Cod <> 0 Then StrSQL.Append("   ,[Programmazione_Cod]     = " & Agro_SQL_SaveNum(Programmazione_Cod) & "")
            If Not IsNothing(Programmazione_entita_Cod) AndAlso Programmazione_entita_Cod <> 0 Then StrSQL.Append("   ,[Programmazione_entita_Cod]     = " & Agro_SQL_SaveNum(Programmazione_entita_Cod) & "")
            If Not IsNothing(ID_Agenda) AndAlso ID_Agenda <> 0 Then StrSQL.Append("   ,[id_agenda]     = " & Agro_SQL_SaveNum(ID_Agenda) & "")
            If Not IsNothing(Ricetta_Operazione_Cod) AndAlso Ricetta_Operazione_Cod <> 0 Then StrSQL.Append("   ,[Ricetta_Operazione_Cod]     = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "")


            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")

            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            If Not IsNothing(Validita_Inizio) AndAlso Validita_Inizio <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            If Not IsNothing(Validita_Fine) AndAlso Validita_Fine <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE Then StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            StrSQL.Append(" WHERE PivaSuperUser  = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Entita_Cod     =  " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            '---------------------------------------------

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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

    Public Function AggregaPoligoni(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod_DaCorreggere As Int32,
                            ByVal Entita_Cod_Origine As String,
                            ByVal FattoreConversione As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.AggregaPoligoni()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            Dim lAggregateSQL As String

            lAggregateSQL = AggregaPoligoni_verifica_getAggregateFunction(objParametri)



            xRisp = AggregaPoligoni_verifica(PivaSuperUser, Entita_Cod_Origine, FattoreConversione, "", objParametri)

            'se l'entità è un multipoly allora esco
            If Not xRisp Then
                Return False
            End If


            stb.Length = 0
            stb.Append(" declare @FattoreConversione int " & vbCrLf)
            stb.Append(" set @FattoreConversione = " & FattoreConversione & vbCrLf)

            stb.Append(" declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod =  " & Entita_Cod_DaCorreggere & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" update g " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" set Poligono_GeoEntity =  " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append(" select top 1 " & vbCrLf)


            stb.Append("    " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione) " & vbCrLf)

            stb.Append(" from gis_entita e  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where E.entita_Cod in (" & Agro_SQL_Save_Clausola_IN(Entita_Cod_Origine) & ") " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" group by  " & vbCrLf)
            stb.Append("    e.PivaSuperUser  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  having " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione) is not null  " & vbCrLf)
            stb.Append("  and not " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione).STAsText() collate Latin1_General_CI_AS like '%MULTIPOLYGON%'  " & vbCrLf)
            stb.Append("  and not " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione).STAsText() collate Latin1_General_CI_AS like '%GEOMETRYCOLLECTION%' " & vbCrLf)
            stb.Append("  )  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  from gis_entita e " & vbCrLf)
            stb.Append("        inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("            on e.Entita_Cod = g.Entita_Cod  " & vbCrLf)
            stb.Append("            and e.PivaSuperUser = g.PivaSuperUser   " & vbCrLf)
            stb.Append(" where e.Entita_Cod = @entita_cod " & vbCrLf)
            stb.Append(" and e.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            stb.Length = 0
            stb.Append(" update Gis_entita set inviato = 0 where entita_Cod = " & Entita_Cod_DaCorreggere)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Private Function AggregaPoligoni_verifica_getAggregateFunction(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim rval As String = "dbo.GeographyUnionAggregate"

        Dim major = VersioneSqlServer_Major(objParametri)
        If major <> 10 Then 'major 10 corrisponde a sql server 2008 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-And-install-latest-updates
            rval = "Geography::UnionAggregate"
        End If

        Return rval
    End Function


    Public Function AggregaPoligoni_verifica(
                    ByVal PivaSuperUser As String,
                    ByVal Entita_Cod_Origine As String,
                    ByVal fattoreConversione As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.AggregaPoligoni()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try



            '---------------------------------------------

            Dim lAggregateSQL As String

            lAggregateSQL = AggregaPoligoni_verifica_getAggregateFunction(objParametri)

            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.Append(" declare @FattoreConversione int " & vbCrLf)
            stb.Append(" set @FattoreConversione = " & fattoreConversione & vbCrLf)

            stb.Append(" select top 1 " & vbCrLf)
            stb.Append("    --e.PivaSuperUser  " & vbCrLf)

            stb.Append("    " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione) " & vbCrLf)
            stb.Append("    --, " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione).AsGml() " & vbCrLf)
            stb.Append(" from gis_entita e  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici g " & vbCrLf)
            stb.Append("        on e.entita_cod = g.entita_cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where E.entita_Cod in (" & Agro_SQL_Save_Clausola_IN(Entita_Cod_Origine) & ") " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" group by  " & vbCrLf)
            stb.Append("    e.PivaSuperUser  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  having " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione) is not null  " & vbCrLf)
            stb.Append("  and not " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione).STAsText()  collate Latin1_General_CI_AS like '%MULTIPOLYGON%'  " & vbCrLf)
            stb.Append("  and not " & lAggregateSQL & "(g.Poligono_GeoEntity).STBuffer(@FattoreConversione).STAsText()  collate Latin1_General_CI_AS like '%GEOMETRYCOLLECTION%' " & vbCrLf)




            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                xRisp = True
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function EliminaDatiPrecisionFarmingFuoriImpianto(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ricetta_operazione_cod As Integer,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append("declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod = " & Entita_Cod & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" declare @PivaSuperUser varchar(100) " & vbCrLf)
            stb.Append(" declare @piva  varchar(100) " & vbCrLf)
            stb.Append(" declare @Sa_cod int " & vbCrLf)
            stb.Append(" declare @Appezza  int " & vbCrLf)
            stb.Append(" declare @Id_Imp int " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" declare @g geography " & vbCrLf)
            stb.Append(" select  @g =gg.Poligono_GeoEntity , @pivasuperuser = e.pivasuperuser, @piva = e.piva, @sa_cod = e.Sa_Cod , @Appezza = e.Appezza, @Id_Imp = e.Id_Imp  " & vbCrLf)
            stb.Append(" from GIS_Entita e " & vbCrLf)
            stb.Append("    inner join GIS_ElementiGrafici gg " & vbCrLf)
            stb.Append("    on e.Entita_Cod = gg.entita_cod " & vbCrLf)
            stb.Append(" where e.Entita_Cod = @Entita_cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" create table #Intersezione (Entita_cod int) " & vbCrLf)
            stb.Append(" create table #ToDelete (Entita_cod int) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert #Intersezione " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" select gg.entita_Cod " & vbCrLf)
            stb.Append(" from GIS_ElementiGrafici gg " & vbCrLf)
            stb.Append(" where gg.Poligono_GeoEntity.STIntersects(@g) = 1 " & vbCrLf)
            stb.Append(" and gg.layerElementiGrafici_cod = 51 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert #todelete " & vbCrLf)
            stb.Append(" select entita_cod " & vbCrLf)
            stb.Append(" from gis_entita e1 " & vbCrLf)
            stb.Append(" where 1=1 " & vbCrLf)
            stb.Append("    and e1.PivaSuperUser = @PivaSuperUser  " & vbCrLf)
            stb.Append("    and e1.piva = @piva " & vbCrLf)
            stb.Append("    and e1.sa_cod = @Sa_cod " & vbCrLf)
            stb.Append("    and e1.Ricetta_Operazione_cod =  " & Agro_SQL_SaveNum(ricetta_operazione_cod) & vbCrLf)
            stb.Append("    and e1.tipoEntita_cod = 51 " & vbCrLf)
            stb.Append("    and e1.Appezza = @Appezza " & vbCrLf)
            stb.Append("    and e1.Id_Imp = @Id_Imp" & vbCrLf)
            stb.Append("    and e1.entita_cod not in ( " & vbCrLf)
            stb.Append("    select entita_cod " & vbCrLf)
            stb.Append("    from #Intersezione  " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete from gis_elementiGrafici where Entita_Cod in (select Entita_Cod from #ToDelete ) " & vbCrLf)
            stb.Append(" delete from GIS_Entita  where Entita_Cod in (select Entita_Cod from #ToDelete ) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" drop table #ToDelete " & vbCrLf)
            stb.Append(" drop table #Intersezione " & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function AssegnaOperazioniPF_Impianto(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal sa_cod As Integer,
                            ByVal programmazione_cod As Integer,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim DataOperazione As Date = Now.Date

            '---------------------------------------------
            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.Append(" update eRpl " & vbCrLf)
            stb.Append(" set  " & vbCrLf)
            stb.Append("      Piva = eR.Piva " & vbCrLf)
            stb.Append("    , sa_cod = eR.Sa_cod " & vbCrLf)
            stb.Append("    , Appezza = eR.appezza " & vbCrLf)
            stb.Append("    , Id_Imp = eR.Id_Imp  " & vbCrLf)
            stb.Append("    , programmazione_cod = 0 " & vbCrLf)
            stb.Append(" from  " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append("    select Entita_cod_PF, MIN (  Entita_cod_impianto ) as Entita_cod_impianto " & vbCrLf)
            stb.Append("    from ( " & vbCrLf)
            stb.Append("    select g1.Entita_Cod as Entita_cod_impianto, g.Entita_Cod as Entita_cod_PF " & vbCrLf)
            stb.Append("    from gis_entita e " & vbCrLf)
            stb.Append("        inner join GIS_ElementiGrafici g     " & vbCrLf)
            stb.Append("            on g.Entita_Cod = e.Entita_Cod  " & vbCrLf)
            stb.Append("        inner join Gis_elementiGrafici g1 " & vbCrLf)
            stb.Append("            on g1.LayerElementiGrafici_Cod = 19 " & vbCrLf)
            stb.Append("            and g.Poligono_GeoEntity.STIntersects(g1.Poligono_GeoEntity) = 1 " & vbCrLf)
            stb.Append("            and g1.entita_cod <> g.Entita_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    where e.TipoEntita_Cod in (50,55) " & vbCrLf)

            If programmazione_cod <> 0 Then
                stb.Append("    and e.programmazione_cod =  " & Agro_SQL_SaveNum(programmazione_cod) & " " & vbCrLf)
            End If


            stb.Append("    ) GG " & vbCrLf)
            stb.Append("    group by Entita_cod_PF  " & vbCrLf)
            stb.Append(" ) Ragg " & vbCrLf)
            stb.Append(" inner join GIS_Entita eR on eR.Entita_Cod = Ragg.Entita_cod_impianto  " & vbCrLf)
            stb.Append(" inner join gis_entita eRpl on eRpl.Entita_Cod = Ragg.Entita_cod_PF  " & vbCrLf)
            stb.Append("  " & vbCrLf)




            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function RibaltaGraficaPlanningCampo(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_cod As Integer,
                            ByVal Piva As String,
                            ByVal sa_cod As Integer,
                            ByVal campo_cod As Integer,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim DataOperazione As Date = Now.Date

            '---------------------------------------------


            stb.Length = 0


            stb.Append(" create table #gisentita (entita_cod int, nuovoentita_cod int, tipoEntita_cod int) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert #gisentita " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      entita_cod " & vbCrLf)
            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                stb.Append("    , dense_rank() over (order by Entita_cod) + (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita')  " & vbCrLf)
            Else
                stb.Append("    , dense_rank() over (order by Entita_cod) + (select ultimo_valore from Sequenza_Tabelle where Nome_Tabella = 'Gis_entita')  " & vbCrLf)
            End If

            stb.Append("    , " & enum_GIS2012_TipoEntita.CAMPI & vbCrLf)

            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append("    where Entita_cod =  " & Entita_cod & vbCrLf)
            'Era ProgrammazioneEntita_Cod... da verificare??? query da rimaneggiare, tabella temporanea non più necessaria...

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.AppendLine()
            stb.AppendLine()
            stb.AppendLine(" insert gis_entita  ")
            stb.AppendLine(" select  ")
            stb.AppendLine("      PivaSuperUser ")
            stb.AppendLine("    , e1.nuovoentita_cod     ")
            stb.AppendLine("    , e1.tipoEntita_cod ")
            stb.AppendLine("    , '" & Agro_SQL_SaveText(Piva) & "' as Piva ")
            stb.AppendLine("    , " & sa_cod & " as Sa_Cod ")
            stb.AppendLine("    , 0 as Appezza ")
            stb.AppendLine("    , " & campo_cod & " as Campo_Cod ")
            stb.AppendLine("    , 0 as Id_Imp ")
            stb.AppendLine("    , PROV ")
            stb.AppendLine("    , COM ")
            stb.AppendLine("    , SEZIONE ")
            stb.AppendLine("    , FOGLIO ")
            stb.AppendLine("    , NUMERO ")
            stb.AppendLine("    , SUBALTERNO ")
            stb.AppendLine("    , 0 as Programmazione_Entita_Cod ")
            stb.AppendLine("    , ID_Agenda ")
            stb.AppendLine("    , Ricetta_Operazione_cod ")
            stb.AppendLine("    , analisi_campione_cod ")
            stb.AppendLine("    , OLDGrafica_ID ")
            stb.AppendLine("    , inviato ")
            stb.AppendLine("    , datainvio ")
            stb.AppendLine("    , " & Agro_SQL_SaveDate(DataOperazione) & " as Data_Creazione ")
            stb.AppendLine("    , " & Agro_SQL_SaveDate(DataOperazione) & " as Data_Modifica ")
            stb.AppendLine("    , '" & objParametri.UsernameOperazione & "' as Username_Creazione ")
            stb.AppendLine("    , '" & objParametri.UsernameOperazione & "' as Username_Modifica ")
            stb.AppendLine("    , Validita_Inizio ")
            stb.AppendLine("    , Validita_Fine ")
            stb.AppendLine("    , programmazione_cod ")
            stb.AppendLine("    , id_mov_det ")
            stb.AppendLine(" from gis_entita ee ")
            stb.AppendLine("    inner join #gisentita e1 on ee.Entita_Cod  = e1.entita_cod  ")
            stb.AppendLine()
            stb.AppendLine()
            stb.AppendLine(" insert gis_elementiGrafici ")
            stb.AppendLine(" select PivaSuperUser, e1.nuovoentita_cod , ElementoGrafico_Des, e1.nuovoentita_cod, " & enum_Gis_LayerElementiGrafici_std.CAMPI & " as LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, inviato, datainvio, " & Agro_SQL_SaveDate(DataOperazione) & " as  Data_Creazione, " & Agro_SQL_SaveDate(DataOperazione) & " as Data_Modifica, '" & objParametri.UsernameOperazione & "' as Username_Creazione, '" & objParametri.UsernameOperazione & "' as Username_Modifica, Validita_Inizio, Validita_Fine ")
            stb.AppendLine(" from gis_elementiGrafici ee ")
            stb.AppendLine("    inner join #gisentita e1 on ee.Entita_Cod  = e1.entita_cod  ")
            stb.AppendLine()
            stb.AppendLine()
            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                stb.AppendLine(" Declare @newGisEnt as int = coalesce( (select MAX(nuovoentita_cod) from #gisentita ), (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita')) ")
                stb.AppendLine(" Alter sequence Sequence_Gis_entita ")
                stb.AppendLine(" restart with @newGisEnt ")
                stb.AppendLine(" Alter sequence Sequence_Gis_elementiGrafici ")
                stb.AppendLine(" restart with @newGisEnt ")
            Else
                stb.AppendLine(" update sequenza_tabelle ")
                stb.AppendLine(" set Ultimo_Valore = coalesce( (select MAX(nuovoentita_cod) from #gisentita ), (select ultimo_valore from Sequenza_Tabelle where Nome_Tabella = 'Gis_entita')) ")
                stb.AppendLine(" where  Nome_Tabella in ( 'Gis_entita', 'Gis_elementiGrafici') ")
            End If

            stb.AppendLine()
            stb.AppendLine()
            stb.AppendLine(" drop table #gisentita " & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function RibaltaGraficaAppezzaImpianto(
                            ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal sa_cod As Integer,
                            ByVal campo_cod As Integer,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim DataOperazione As Date = Now.Date

            '---------------------------------------------


            stb.Length = 0

            stb.Append("create table #decode (gru_cod int, tipoEntita_cod int) " & vbCrLf)
            stb.Append(" insert into #decode values " & vbCrLf)
            stb.Append("    (1, 22),  " & vbCrLf)
            stb.Append("    (2, 20),  " & vbCrLf)
            stb.Append("    (3, 19) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" create table #gisentita (entita_cod int, nuovoentita_cod int, tipoEntita_cod int, id_reg int) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert #gisentita " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      entita_cod " & vbCrLf)
            'Lavez - 05/11/2024 - gestione sequence
            If AgronicaCoreDataProvider.Agro_Sequenze.CheckAllowAppSettingsFlagUseSequence() Then
                stb.Append("    , dense_rank() over (order by Entita_cod) + (SELECT current_value FROM sys.sequences WHERE name = 'Sequence_Gis_entita')  " & vbCrLf)
            Else
                stb.Append("    , dense_rank() over (order by Entita_cod) + (select ultimo_valore from Sequenza_Tabelle where Nome_Tabella = 'Gis_entita')  " & vbCrLf)
            End If
            stb.Append("    , coalesce(impNoGrafica.tipoEntita_cod, 23) " & vbCrLf)
            stb.Append("    , impNoGrafica.ID_REG " & vbCrLf)

            stb.Append(" from gis_entita ee inner join (  " & vbCrLf)
            stb.Append("    select distinct ii.piva, ii.Sa_Cod, ii.Appezza, ii.id_reg, dd.tipoEntita_cod   " & vbCrLf)
            stb.Append("    from campi c " & vbCrLf)
            stb.Append("        inner join Appezzamento a " & vbCrLf)
            stb.Append("            on c.Piva = a.PIVA " & vbCrLf)
            stb.Append("            and c.Sa_Cod = a.SA_COD  " & vbCrLf)
            stb.Append("            and c.Campo_Cod = a.Campo_Cod  " & vbCrLf)
            stb.Append("        inner join Reg_Impianti ii " & vbCrLf)
            stb.Append("            on ii.PIVA = a.PIVA  " & vbCrLf)
            stb.Append("            and ii.SA_COD = a.SA_COD  " & vbCrLf)
            stb.Append("            and ii.APPEZZA = a.APPEZZA       " & vbCrLf)
            stb.Append("        inner join Cultivar cc " & vbCrLf)
            stb.Append("            on cc.cul_cod = ii.cul_cod " & vbCrLf)
            stb.Append("        inner join specievegetali veg " & vbCrLf)
            stb.Append("            on Veg.veg_cod = cc.Veg_Cod  " & vbCrLf)
            stb.Append("        left join #decode dd " & vbCrLf)
            stb.Append("            on veg.Gru_Cod = dd.gru_cod      " & vbCrLf)
            stb.Append("        left join GIS_Entita e " & vbCrLf)
            stb.Append("            on e.Piva = ii.PIVA  " & vbCrLf)
            stb.Append("            and e.Sa_Cod = ii.SA_COD   " & vbCrLf)
            stb.Append("            and e.Appezza = ii.APPEZZA  " & vbCrLf)
            stb.Append("            and e.Id_Imp = ii.ID_REG         " & vbCrLf)
            stb.Append("            and e.tipoEntita_cod in (select tipoEntita_cod from GIS_TipoEntita where LayerElementiGrafici_Cod = 19 )  " & vbCrLf)
            stb.Append("    where 1=1 " & vbCrLf)
            stb.Append("    and c.Campo_Cod =  " & campo_cod & vbCrLf)
            stb.Append("    and a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stb.Append("    and a.sa_cod = " & sa_cod & vbCrLf)
            stb.Append("    and e.Piva is null " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            stb.Append(" ) impNoGrafica " & vbCrLf)
            stb.Append(" on ee.piva = impNoGrafica.Piva  " & vbCrLf)
            stb.Append(" and ee.Sa_Cod = impNoGrafica.Sa_Cod  " & vbCrLf)
            stb.Append(" and ee.Appezza = impNoGrafica.Appezza " & vbCrLf)
            stb.Append(" and ee.TipoEntita_Cod = 1  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert gis_entita  " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      PivaSuperUser " & vbCrLf)
            stb.Append("    , e1.nuovoentita_cod     " & vbCrLf)
            stb.Append("    , e1.tipoEntita_cod , Piva, Sa_Cod, Appezza, Campo_Cod, e1.id_reg as  Id_Imp, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, Programmazione_Entita_Cod, ID_Agenda, Ricetta_Operazione_cod, analisi_campione_cod, OLDGrafica_ID, inviato, datainvio, " & Agro_SQL_SaveDate(DataOperazione) & " as  Data_Creazione, " & Agro_SQL_SaveDate(DataOperazione) & " as Data_Modifica, '" & objParametri.UsernameOperazione & "' as Username_Creazione, '" & objParametri.UsernameOperazione & "' as Username_Modifica, Validita_Inizio, Validita_Fine, programmazione_cod " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append("    inner join #gisentita e1 on ee.Entita_Cod  = e1.entita_cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" insert gis_elementiGrafici " & vbCrLf)
            stb.Append(" select PivaSuperUser, e1.nuovoentita_cod , ElementoGrafico_Des, e1.nuovoentita_cod, 19 as LayerElementiGrafici_Cod, Poligono_GeoEntity, Flag_GPS, inviato, datainvio, " & Agro_SQL_SaveDate(DataOperazione) & " as  Data_Creazione, " & Agro_SQL_SaveDate(DataOperazione) & " as Data_Modifica, '" & objParametri.UsernameOperazione & "' as Username_Creazione, '" & objParametri.UsernameOperazione & "' as Username_Modifica, Validita_Inizio, Validita_Fine " & vbCrLf)
            stb.Append(" from gis_elementiGrafici ee " & vbCrLf)
            stb.Append("    inner join #gisentita e1 on ee.Entita_Cod  = e1.entita_cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" update sequenza_tabelle " & vbCrLf)
            stb.Append(" set Ultimo_Valore = coalesce( (select MAX(nuovoentita_cod) from #gisentita ), (select ultimo_valore from Sequenza_Tabelle where Nome_Tabella = 'Gis_entita')) " & vbCrLf)
            stb.Append(" where  Nome_Tabella in ( 'Gis_entita', 'Gis_elementiGrafici') " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" drop table #gisentita " & vbCrLf)
            stb.Append(" drop table #decode " & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function EliminaDatiPrecisionFarmingDatoPlanning(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append("declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod = " & Entita_Cod & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete ee  " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append(" where exists (  " & vbCrLf)
            stb.Append("   Select 1 " & vbCrLf)
            stb.Append("        from gis_entita ii " & vbCrLf)
            stb.Append("            inner join programmazione_entita ee1 " & vbCrLf)
            stb.Append("                on ii.programmazione_cod = ee1.programmazione_cod             " & vbCrLf)
            stb.Append("        where entita_cod = @Entita_cod " & vbCrLf)
            stb.Append("            and ee.PivaSuperUser = ii.PivaSuperUser  " & vbCrLf)
            stb.Append("            and ee.piva = ii.piva " & vbCrLf)
            stb.Append("            and ee.sa_cod = ii.sa_cod " & vbCrLf)
            stb.Append("            and ee.Appezza = ee1.Appezza             " & vbCrLf)
            stb.Append("            and ee.Id_Imp = ee1.Id_Reg       " & vbCrLf)



            stb.Append(" ) " & vbCrLf)
            stb.Append(" and ee.tipoentita_Cod in (" & Agro_SQL_Save_Clausola_IN(ElencoLayer) & " ) " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    Public Function EliminaDatiPrecisionFarmingDatoImpianto(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal ElencoLayer As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append("declare @Entita_cod int " & vbCrLf)
            stb.Append(" set @Entita_cod = " & Entita_Cod & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" delete ee  " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append(" where exists ( " & vbCrLf)
            stb.Append("  Select 1 " & vbCrLf)
            stb.Append("    from gis_entita ii " & vbCrLf)
            stb.Append("    where entita_cod = @Entita_cod " & vbCrLf)
            stb.Append("    and ee.PivaSuperUser = ii.PivaSuperUser  " & vbCrLf)
            stb.Append("    and ee.piva = ii.piva " & vbCrLf)
            stb.Append("    and ee.sa_cod = ii.sa_cod " & vbCrLf)
            stb.Append("    and ee.Appezza = ii.Appezza  " & vbCrLf)
            stb.Append("    and ee.Campo_Cod = ii.Campo_Cod  " & vbCrLf)
            stb.Append("    and ee.Id_Imp = ii.Id_Imp  " & vbCrLf)
            stb.Append("    and ee.programmazione_cod = ii.programmazione_cod " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" and ee.tipoentita_Cod in (" & Agro_SQL_Save_Clausola_IN(ElencoLayer) & ") " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Cancella(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Entita_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE GIS_Entita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND     Entita_Cod      =   " & Agro_SQL_SaveNum(Entita_Cod) & "  ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     GIS_Entita ")
                StrSQL.Append(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND     Entita_Cod      =   " & Agro_SQL_SaveNum(Entita_Cod) & "  ")

            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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



    Public Function AggiornaCodiceRicetta_operazione(
                            ByVal PivaSuperUser As String,
                            ByVal ricetta_operazione_cod_vecchio As Int32,
                            ByVal ricetta_cod_nuovo As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------


            stb.Length = 0

            stb.Append(" update ee " & vbCrLf)
            stb.Append(" set ricetta_operazione_cod = op.Ricetta_Operazione_Cod " & vbCrLf)
            stb.Append(" from gis_entita ee " & vbCrLf)
            stb.Append("    inner join ricette_operazioni op " & vbCrLf)
            stb.Append("    on op.ricetta_cod =  " & ricetta_cod_nuovo & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where op.ricetta_cod =  " & ricetta_cod_nuovo & vbCrLf)
            stb.Append(" and ee.ricetta_Operazione_cod = " & ricetta_operazione_cod_vecchio & vbCrLf)
            stb.Append(" and PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function SalvaGUIDEntita(ByVal entita_cod As Integer,
                                    ByVal GUID_entita As String,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.SalvaGUIDEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_Entita SET ")

            StrSQL.AppendLine(String.Format(" Entita_GUID = '{0}' ", Agro_SQL_SaveText(GUID_entita)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_cod)))

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

    Public Function SalvaParametriVisualizzazioneEntita(ByVal entita_cod As Int32,
                                                        ByVal parametri_visualizzazione As String,
                                                        ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.SalvaParametriVisualizzazioneEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_Entita SET ")

            StrSQL.AppendLine(String.Format(" ParametriVisualizzazioneLayer = '{0}' ", Agro_SQL_SaveText(parametri_visualizzazione)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_cod)))

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

    Public Function SaveExportEntities(ByVal newIdEsp As Int32,
                                       ByVal elencoEntita As List(Of Int32),
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.SaveExportEntities()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO EntitaXEsportazioni (Entita_Cod, IdEsp)")
            StrSQL.AppendLine("VALUES")

            Dim increment As Int32 = CostantiPersonalizzate.BulkExportIncrement
            Dim skip As Int32 = 0
            Dim tmpSkip As Int32 = 0

            Do While skip <= elencoEntita.Count

                Dim sliced = elencoEntita.Skip(skip).Take(Math.Min(elencoEntita.Count - skip, increment))

                StrSQL.AppendLine(String.Format(" ({0}, {1})", String.Join(String.Format(" , {0}),(", newIdEsp), sliced), newIdEsp))

                skip += increment
                tmpSkip += increment

                If skip >= elencoEntita.Count Then ' Eseguo la scrittura delle entità rimanenti ed esco dal loop
                    '--------------------------------------------------------------------------
                    resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------
                    Exit Do
                ElseIf (tmpSkip + increment) > 1000 Then ' se ho raggiunto il numero massimo di elementi di un INSERT, eseguo la scrittura e proseguo creandone un altra
                    '--------------------------------------------------------------------------
                    resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------
                    tmpSkip = 0
                    StrSQL.Length = 0
                    StrSQL.AppendLine("INSERT INTO EntitaXEsportazioni (Entita_Cod, IdEsp)")
                    StrSQL.AppendLine("VALUES")
                Else
                    StrSQL.Append(", ")
                End If
            Loop

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return resp
    End Function

    Public Function CleanCompletedExportEntitiesRecords(ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.CleanCompletedExportEntitiesRecords()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = True

        Try
            StrSQL.Length = 0

            ' seleziono i PrametriElaborazione delle esecuzioni completate
            StrSQL.AppendLine("SELECT ParametriElaborazione")
            StrSQL.AppendLine("FROM GIS_LayerAnalysisConfig_Exec_Log")
            StrSQL.AppendLine("WHERE StatoElaborazione = 1")
            StrSQL.AppendLine("    AND ParametriElaborazione IS NOT NULL")
            StrSQL.AppendLine("    AND ParametriElaborazione LIKE '%idEsp%'")
            Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            Dim idEspList As New List(Of Int32)
            For Each row In dt.Rows
                Dim idEsp = JObject.Parse(CStr(row("ParametriElaborazione")))("idEsp")
                If Not IsNothing(idEsp) AndAlso CInt(idEsp) > 0 Then
                    idEspList.Add(CInt(idEsp))
                End If
            Next

            If idEspList.Count > 0 Then
                StrSQL.Length = 0
                StrSQL.AppendLine("DELETE FROM EntitaXEsportazioni")
                StrSQL.AppendLine(String.Format("WHERE IdEsp IN ({0})", String.Join(", ", idEspList)))

                '--------------------------------------------------------------------------
                resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function
End Class

Public Class Impostazioni_Utente
    Public UsaPuntoInterno As Boolean = True
    Public PuntoPoligono As Boolean = True
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
