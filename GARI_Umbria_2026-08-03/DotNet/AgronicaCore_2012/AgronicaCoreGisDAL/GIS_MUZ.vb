Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class GIS_MUZ_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiDatiMUZVisibili(ByVal piva As String,
                                         ByVal lista_MUZ_Cod As List(Of Int32),
                                         ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiDatiMUZVisibili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" AO.Area_Cod ")
            StrSQL.AppendLine(" , AO.Piva ")
            StrSQL.AppendLine(" , AO.Sa_Cod ")
            StrSQL.AppendLine(" , AO.Appezza ")
            StrSQL.AppendLine(" , ISNULL(GAO.Gruppo_Area_Cod, 0) Gruppo_Area_Cod ")
            StrSQL.AppendLine(" , ISNULL(GAO.Gruppo_Area_Des, '') Gruppo_Area_Des ")
            StrSQL.AppendLine(" FROM Appezzamento_XArea_Omogenea AO ")
            StrSQL.AppendLine(" LEFT JOIN Area_Omogenea_XGruppo_Area_Omogenea AOXG ")
            StrSQL.AppendLine("        ON AOXG.Area_Cod = AO.Area_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Gruppo_Area_Omogenea GAO ")
            StrSQL.AppendLine("        ON GAO.Gruppo_Area_Cod = AOXG.Gruppo_Area_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" AO.Area_Cod in({0}) ", Agro_SQL_Save_Clausola_IN(String.Join(",", lista_MUZ_Cod))))

            If Not piva.Equals("") Then
                StrSQL.AppendLine(String.Format(" AND AO.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
            End If

            StrSQL.AppendLine(" ORDER BY AO.Area_Cod, AO.Sa_Cod ")

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

    Public Function LeggiListaGruppiMUZ(ByVal piva As String,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiListaGruppiMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" Piva ")
            StrSQL.AppendLine(" , Gruppo_Area_Cod ")
            StrSQL.AppendLine(" , Gruppo_Area_Des ")
            StrSQL.AppendLine(" FROM Gruppo_Area_Omogenea ")

            If Not piva.Equals("") Then
                StrSQL.AppendLine(" WHERE ")

                StrSQL.AppendLine(String.Format(" Piva = '{0}' ", Agro_SQL_SaveText(piva)))
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

    Public Function LeggiMUZ(ByVal area_Cod As Int32,
                             ByVal piva As String,
                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" MUZ.piva ")
            StrSQL.AppendLine(" , MUZ.Area_Cod ")
            StrSQL.AppendLine(" , MUZ.Area_Des ")
            StrSQL.AppendLine(" , ISNULL(GAO.Gruppo_Area_Cod, 0) Gruppo_Area_Cod ")
            StrSQL.AppendLine(" , ISNULL(GAO.Gruppo_Area_Des, '') Gruppo_Area_Des ")
            StrSQL.AppendLine(" , MUZ.Tessitura_cod ")
            StrSQL.AppendLine(" , MUZ.Altimetria ")
            StrSQL.AppendLine(" , MUZ.SO ")
            StrSQL.AppendLine(" , MUZ.TipoZona ")
            StrSQL.AppendLine(" , MUZ.Validita_Inizio ")
            StrSQL.AppendLine(" , MUZ.Validita_Fine ")
            StrSQL.AppendLine(" , ISNULL(AMUZ.Sa_Cod, 0) Sa_Cod ")
            StrSQL.AppendLine(" , ISNULL(AMUZ.Appezza, 0) Appezza ")
            StrSQL.AppendLine(" FROM Area_Omogenea_MUZ MUZ ")
            StrSQL.AppendLine(" LEFT JOIN Area_Omogenea_XGruppo_Area_Omogenea MUZGAO ")
            StrSQL.AppendLine("     ON MUZGAO.Area_Cod = MUZ.Area_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Gruppo_Area_Omogenea GAO ")
            StrSQL.AppendLine("     ON GAO.Gruppo_Area_Cod = MUZGAO.Gruppo_Area_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento_XArea_Omogenea AMUZ ")
            StrSQL.AppendLine("     ON AMUZ.Area_Cod = MUZ.Area_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" MUZ.Piva_SuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            If area_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" AND MUZ.Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))
            End If

            If Not piva.Equals("") Then
                StrSQL.AppendLine(String.Format(" AND MUZ.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
            End If

            StrSQL.AppendLine(" ORDER BY MUZ.Area_Cod, AMUZ.Sa_Cod, AMUZ.Appezza ")

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

    Public Function LeggiAnalisiTestateDaMUZ(ByVal area_Cod As Int32,
                                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiAnalisiTestateDaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" AT.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" , AT.Analisi_Testata_Des ")

            StrSQL.AppendLine(" FROM Area_Omogenea_MUZ MUZ ")
            StrSQL.AppendLine(" INNER JOIN Area_Omogenea_XAnalisi_Testata MUZAT ")
            StrSQL.AppendLine("     ON MUZAT.Area_Cod = MUZ.Area_Cod ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Testata AT ")
            StrSQL.AppendLine("     ON AT.Analisi_Testata_Cod = MUZAT.Analisi_Testata_Cod ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" MUZ.Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

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

    Public Function LeggiParticelleCatastaliDaMUZ(ByVal area_Cod As Int32,
                                                  ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiParticelleCatastaliDaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" MUZPC.PROV ")
            StrSQL.AppendLine(" , MUZPC.COM ")
            StrSQL.AppendLine(" , MUZPC.SEZIONE ")
            StrSQL.AppendLine(" , MUZPC.FOGLIO ")
            StrSQL.AppendLine(" , MUZPC.NUMERO ")
            StrSQL.AppendLine(" , MUZPC.SUBALTERNO ")
            StrSQL.AppendLine(" , IXP.Sup_Condotta ")
            StrSQL.AppendLine(" , ISNULL(IXP.TitoloPossesso, 0) As TitoloPossesso ")
            StrSQL.AppendLine(" , IXP.Validita_Inizio ")
            StrSQL.AppendLine(" , IXP.Validita_Fine ")
            StrSQL.AppendLine(" , ISNULL(ISTAT.LOCALITA, '') As LOCALITA ")
            StrSQL.AppendLine(" , ISNULL(LP.PROVINCIA, '') As PROVINCIA ")

            StrSQL.AppendLine(" FROM Area_Omogenea_MUZ MUZ ")
            StrSQL.AppendLine(" INNER JOIN Area_Omogenea_XParticelle MUZPC ")
            StrSQL.AppendLine("     ON MUZPC.Area_Cod = MUZ.Area_Cod ")
            StrSQL.AppendLine(" INNER JOIN ImpreseXParticelle IXP ")
            StrSQL.AppendLine("     ON IXP.PROV = MUZPC.PROV AND ")
            StrSQL.AppendLine("        IXP.COM = MUZPC.COM AND ")
            StrSQL.AppendLine("        IXP.SEZIONE = MUZPC.SEZIONE AND ")
            StrSQL.AppendLine("        IXP.FOGLIO = MUZPC.FOGLIO AND ")
            StrSQL.AppendLine("        IXP.NUMERO = MUZPC.NUMERO AND ")
            StrSQL.AppendLine("        IXP.SUBALTERNO = MUZPC.SUBALTERNO AND ")
            StrSQL.AppendLine("        IXP.PIVA = MUZ.Piva ")
            StrSQL.AppendLine(" INNER JOIN ISTAT ")
            StrSQL.AppendLine("     ON ISTAT.PROV = MUZPC.PROV AND ")
            StrSQL.AppendLine("        ISTAT.COM = MUZPC.COM ")
            StrSQL.AppendLine(" INNER JOIN Lista_Province LP ")
            StrSQL.AppendLine("     ON LP.PROV = MUZPC.PROV ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" IXP.Validita_Fine > CAST(GETDATE() AS DATE) and IXP.Validita_Inizio <= CAST(GETDATE() AS DATE) ")

            StrSQL.AppendLine(String.Format(" AND MUZ.Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

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

    Public Function LeggiMUZDaGruppo(ByVal Gruppo_Area_Cod As Int32,
                                     ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiMUZDaGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" DISTINCT Area_Cod ")
            StrSQL.AppendLine(" FROM  Area_Omogenea_XGruppo_Area_Omogenea ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Gruppo_Area_Cod = {0} ", Agro_SQL_SaveNum(Gruppo_Area_Cod)))

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

    Public Function leggiElementoGraficoDaMUZ(ByVal Area_Cod As Int32,
                                              ByRef objParametri As AgronicaCoreParametri) As Int32

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_MUZ_R.leggiElementoGraficoDaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" DISTINCT EG.ElementoGrafico_Cod ")
            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN GIS_ElementiGrafici EG ")
            StrSQL.AppendLine("     ON EG.Entita_Cod = E.Entita_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" E.Area_Cod = {0} ", Agro_SQL_SaveNum(Area_Cod)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Esistono più Entità con lo stesso Area_Cod.")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return CInt(DT.Rows(0)("ElementoGrafico_Cod"))

    End Function

    ''' <summary>
    ''' Restituisce tutte le MUZ associate ad un appezzamento identificato dalle sue chiavi.
    ''' </summary>
    Public Function LeggiMUZDaAppezzamento(ByVal piva As String,
                                           ByVal sa_cod As Int32,
                                           ByVal appezza As Int32,
                                           ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const routine = "AgronicaCoreUtentiDAL.GIS_MUZ_R.LeggiMUZDaAppezzamento()"
        Dim dataTable As DataTable

        Try

            Dim strSql As New Text.StringBuilder
            strSql.Length = 0

            strSql.AppendLine(" SELECT ")
            strSql.AppendLine(" a.Area_Cod ")
            strSql.AppendLine(" , b.Piva ")
            strSql.AppendLine(" , b.Sa_Cod ")
            strSql.AppendLine(" , b.Appezza ")
            strSql.AppendLine(" , d.Poligono_GeoEntity_WKT ")
            strSql.AppendLine(" FROM Area_Omogenea_MUZ a ")
            strSql.AppendLine(" INNER JOIN Appezzamento_XArea_Omogenea b ")
            strSql.AppendLine("     ON a.Area_Cod = b.Area_Cod ")
            strSql.AppendLine(" INNER JOIN Area_OmogeneaxEntita c ")
            strSql.AppendLine("     ON c.Area_Cod = b.Area_Cod ")
            strSql.AppendLine(" INNER JOIN Gis_ElementiGrafici d ")
            strSql.AppendLine("     ON d.Entita_Cod = c.Entita_Cod ")

            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(String.Format(" b.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
            strSql.AppendLine(String.Format(" AND b.Sa_Cod = {0} ", Agro_SQL_SaveNum(sa_cod)))
            strSql.AppendLine(String.Format(" AND b.Appezza = {0} ", Agro_SQL_SaveNum(appezza)))

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

    ''' <summary>
    ''' Legge le analisi MUZ (Mappe di Unità di Zonazione) filtrate per aree omogenee e parametri analitici.
    ''' </summary>
    Public Function LeggiAnalisiMuz(ByRef areaCods As List(Of Integer),
                                     ByRef paramsCods As List(Of Integer),
                                     ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const routing = "AgronicaCoreGisDAL.GIS_MUZ_R.LeggiAnalisiMuz()"
        Dim dataTable As DataTable

        Try
            Dim strSql As New Text.StringBuilder
            strSql.Length = 0

            strSql.AppendLine(" SELECT a.Area_Cod ")
            strSql.AppendLine("      , c.Analisi_Dettaglio_Cod ")
            strSql.AppendLine("      , c.Analisi_Parametro_Cod ")
            strSql.AppendLine("      , d.Analisi_Parametro_Des ")
            strSql.AppendLine("      , c.Analisi_Dettaglio_Valore_1 ")
            strSql.AppendLine(" FROM Area_Omogenea_XAnalisi_Testata a ")
            strSql.AppendLine(" INNER JOIN Analisi_Testata b ON b.Analisi_Testata_Cod = a.Analisi_Testata_Cod ")
            strSql.AppendLine(" INNER JOIN Analisi_Dettagli c ON c.Analisi_Testata_Cod = b.Analisi_Testata_Cod ")
            strSql.AppendLine(" INNER JOIN Analisi_Parametri d ON d.Analisi_Parametro_Cod = c.Analisi_Parametro_Cod ")
            strSql.AppendLine(String.Format(" WHERE a.Area_Cod IN ({0}) ", Agro_SQL_Save_Clausola_IN(String.Join(",", areaCods))))
            strSql.AppendLine(String.Format(" AND c.Analisi_Parametro_Cod IN ({0}) ", Agro_SQL_Save_Clausola_IN(String.Join(",", paramsCods))))

            '--------------------------------------------------------------------------
            dataTable = EseguiQuery_Lettura(objParametri, strSql.ToString, routing)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim errorMessage = ex.Message
            Scrivi_LOG(objParametri, routing, errorMessage)
            Throw New Exception("[" & routing & "] : " & errorMessage)
        End Try

        Return dataTable

    End Function
End Class
Public Class GIS_MUZ_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function AggiungiMUZAGruppo(ByVal Area_Cod As Integer,
                                       ByVal Gruppo_Area_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AggiungiMUZAGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Area_Omogenea_XGruppo_Area_Omogenea ( ")
            StrSQL.AppendLine(" Area_Cod ")
            StrSQL.AppendLine(" , Gruppo_Area_Cod ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(Area_Cod)))
            StrSQL.AppendLine(String.Format(",  {0} ", Agro_SQL_SaveNum(Gruppo_Area_Cod)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function
    
    Public Function AssociaMUZAdEntita(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Entita_Cod As Integer,
                                       ByVal Area_cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AggiungiMUZAEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Area_OmogeneaxEntita ( ")
            StrSQL.AppendLine(" Piva_SuperUser ")
            StrSQL.AppendLine(" , Piva ")
            StrSQL.AppendLine(" , Sa_Cod ")
            StrSQL.AppendLine(" , Area_Cod ")
            StrSQL.AppendLine(" , Entita_Cod ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(Piva)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Sa_Cod)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Area_Cod)))
            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(Entita_Cod)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function DeleteMUZ(ByVal area_Cod As Int32,
                              ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DeleteMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Area_Omogenea_MUZ ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp


    End Function

    Public Function UpdateMUZ(ByVal muz As MUZ,
                              ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.UpdateMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Area_Omogenea_MUZ SET ")

            StrSQL.AppendLine(String.Format(" Area_Des = '{0}' ", Agro_SQL_SaveText(muz.Area_Des)))
            StrSQL.AppendLine(String.Format(" , Tessitura_cod  = {0} ", Agro_SQL_SaveNum(muz.Tessitura_cod)))
            StrSQL.AppendLine(String.Format(" , Altimetria = '{0}' ", Agro_SQL_SaveText(muz.Altimetria)))
            StrSQL.AppendLine(String.Format(" , SO = {0} ", Agro_SQL_SaveNum(muz.SO)))
            StrSQL.AppendLine(String.Format(" , TipoZona = '{0}' ", Agro_SQL_SaveText(muz.TipoZona)))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , Validita_Inizio = {0} ", Agro_SQL_SaveDate(muz.Validita_Inizio)))
            StrSQL.AppendLine(String.Format(" , Validita_Fine = {0} ", Agro_SQL_SaveDate(muz.Validita_Fine)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(muz.Area_Cod)))
            StrSQL.AppendLine(String.Format(" AND Piva = '{0}' ", Agro_SQL_SaveText(muz.Piva)))


            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function AggiornaElementoGrafico(ByVal area_Cod As Int32,
                                            ByVal geometry As String,
                                            ByVal elementoGrafico_Des As String,
                                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AggiornaDescrizioneElementoGrafico()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_ElementiGrafici SET ")
            StrSQL.AppendLine(String.Format(" ElementoGrafico_Des = '{0}' ", Agro_SQL_SaveText(elementoGrafico_Des)))

            If Not geometry.Equals("") Then
                StrSQL.AppendLine(String.Format(" , Poligono_GeoEntity = {0} ", Agro_SQL_SaveGeograpyFromWKTString(geometry)))
            End If

            StrSQL.AppendLine(" WHERE ElementoGrafico_Cod = ( select EG.ElementoGrafico_Cod ")
            StrSQL.AppendLine("                               from GIS_ElementiGrafici EG ")
            StrSQL.AppendLine("                               inner join GIS_Entita E ")
            StrSQL.AppendLine("                                     On E.Entita_Cod = EG.Entita_Cod ")

            StrSQL.AppendLine(String.Format("                 where E.Area_Cod = {0} ) ", Agro_SQL_SaveNum(area_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function DisassociaAnalisiDaMUZ(ByVal area_Cod As Int32,
                                           ByVal analisi_Testata_Cod As Int32,
                                           ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DisassociaAnalisiDaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Area_Omogenea_XAnalisi_Testata ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            If analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" AND Analisi_Testata_Cod = {0} ", Agro_SQL_SaveNum(analisi_Testata_Cod)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function DisassociaMUZDaParticellaCatastale(ByVal area_Cod As Int32,
                                                       ByVal particella As ParticelleCatastali_MUZ,
                                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DisassociaMUZDaParticellaCatastale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Area_Omogenea_XParticelle ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            If particella IsNot Nothing Then
                StrSQL.AppendLine(String.Format(" AND  PROV = '{0}' ", Agro_SQL_SaveText(particella.Provincia)))
                StrSQL.AppendLine(String.Format(" AND  COM = '{0}' ", Agro_SQL_SaveText(particella.Comune)))
                StrSQL.AppendLine(String.Format(" AND  SEZIONE = '{0}' ", Agro_SQL_SaveText(IIf(particella.Sezione.Equals(""), "0", particella.Sezione))))
                StrSQL.AppendLine(String.Format(" AND  FOGLIO = {0} ", Agro_SQL_SaveNum(particella.Foglio)))
                StrSQL.AppendLine(String.Format(" AND  NUMERO = {0} ", Agro_SQL_SaveNum(particella.Numero)))
                StrSQL.AppendLine(String.Format(" AND  SUBALTERNO = '{0}' ", Agro_SQL_SaveText(IIf(particella.Subalterno.Equals(""), "0", particella.Subalterno))))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp
    End Function

    Public Function ScriviMUZ(ByVal muz As MUZ,
                              ByRef objParametri As AgronicaCoreParametri) As Int32

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.ScriviMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Dim Area_Cod As Int32

        Try

            Dim sequenza As New Agro_Sequenze
            Area_Cod = sequenza.NuovoId_Tabella("Area_Omogenea_MUZ", 0, Int32.MaxValue, objParametri, True)

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Area_Omogenea_MUZ ( ")

            StrSQL.AppendLine(" Piva_SuperUser ")
            StrSQL.AppendLine(" , Piva ")
            StrSQL.AppendLine(" , Area_Cod ")
            StrSQL.AppendLine(" , Area_Des ")
            StrSQL.AppendLine(" , Tessitura_cod ")
            StrSQL.AppendLine(" , Altimetria ")
            StrSQL.AppendLine(" , SO ")
            StrSQL.AppendLine(" , TipoZona ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" , Validita_Inizio ")
            StrSQL.AppendLine(" , Validita_Fine ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(muz.Piva)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Area_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(muz.Area_Des)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(muz.Tessitura_cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(muz.Altimetria)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(muz.SO)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(muz.TipoZona)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(muz.Validita_Inizio)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(muz.Validita_Fine)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Area_Cod

    End Function

    Public Function AssociaMUZAdAnalisi(ByVal area_cod As Int32,
                                        ByVal analisi_Testata_Cod As Int32,
                                        ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AssociaMUZAdAnalisi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Area_Omogenea_XAnalisi_Testata ( ")
            StrSQL.AppendLine(" Area_Cod ")
            StrSQL.AppendLine(" , Analisi_Testata_Cod ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(area_cod)))
            StrSQL.AppendLine(String.Format(",  {0} ", Agro_SQL_SaveNum(analisi_Testata_Cod)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function AssociaMUZAParticellaCatastale(ByVal area_cod As Int32,
                                                   ByVal particella As ParticelleCatastali_MUZ,
                                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AssociaMUZAParticellaCatastale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Area_Omogenea_XParticelle ( ")
            StrSQL.AppendLine(" Area_Cod ")
            StrSQL.AppendLine(" , PROV ")
            StrSQL.AppendLine(" , COM ")
            StrSQL.AppendLine(" , SEZIONE ")
            StrSQL.AppendLine(" , FOGLIO ")
            StrSQL.AppendLine(" , NUMERO ")
            StrSQL.AppendLine(" , SUBALTERNO ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(area_cod)))
            StrSQL.AppendLine(String.Format(",  '{0}' ", Agro_SQL_SaveText(particella.Provincia)))
            StrSQL.AppendLine(String.Format(",  '{0}' ", Agro_SQL_SaveText(particella.Comune)))
            StrSQL.AppendLine(String.Format(",  '{0}' ", Agro_SQL_SaveText(IIf(particella.Sezione.Equals(""), "0", particella.Sezione))))
            StrSQL.AppendLine(String.Format(",  {0} ", Agro_SQL_SaveNum(particella.Foglio)))
            StrSQL.AppendLine(String.Format(",  {0} ", Agro_SQL_SaveNum(particella.Numero)))
            StrSQL.AppendLine(String.Format(",  '{0}' ", Agro_SQL_SaveText(IIf(particella.Subalterno.Equals(""), "0", particella.Subalterno))))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function ScriviGruppo(ByVal Gruppo_Area_Des As String,
                                 ByVal Piva As String,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal Gruppo_Area_Cod As Integer = 0) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AggiungiMUZAGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO Gruppo_Area_Omogenea ( ")
            StrSQL.AppendLine(" Piva ")
            StrSQL.AppendLine(" , Gruppo_Area_Cod ")
            StrSQL.AppendLine(" , Gruppo_Area_Des ")

            StrSQL.AppendLine(" ) VALUES ( ")

            If Gruppo_Area_Cod = 0 Then
                Dim sequenza As New Agro_Sequenze

                Gruppo_Area_Cod = sequenza.NuovoId_Tabella("Gruppo_Area_Omogenea", 0, Int32.MaxValue, objParametri, True)
            End If

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(Piva)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Gruppo_Area_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Gruppo_Area_Des)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function ClonaMUZ(ByVal Old_Area_Cod As Int32,
                              ByRef objParametri As AgronicaCoreParametri) As Int32

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.ClonaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Dim Area_Cod As Int32

        Try
            Dim sequenza As New Agro_Sequenze
            Area_Cod = sequenza.NuovoId_Tabella("Area_Omogenea_MUZ", 0, Int32.MaxValue, objParametri, True)

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Area_Omogenea_MUZ ( ")

            StrSQL.AppendLine("  Piva_SuperUser ")
            StrSQL.AppendLine(" , Piva ")
            StrSQL.AppendLine(" , Area_Cod ")
            StrSQL.AppendLine(" , Area_Des ")
            StrSQL.AppendLine(" , Tessitura_cod ")
            StrSQL.AppendLine(" , Altimetria ")
            StrSQL.AppendLine(" , SO ")
            StrSQL.AppendLine(" , TipoZona ")
            StrSQL.AppendLine(" , Data_Creazione ")
            StrSQL.AppendLine(" , Data_Modifica ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ) ")

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("  Piva_SuperUser ")
            StrSQL.AppendLine(" , Piva ")

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Area_Cod)))

            StrSQL.AppendLine(" , Area_Des ")
            StrSQL.AppendLine(" , Tessitura_cod ")
            StrSQL.AppendLine(" , Altimetria ")
            StrSQL.AppendLine(" , SO ")
            StrSQL.AppendLine(" , TipoZona ")
            StrSQL.AppendLine(" , GETDATE() ")
            StrSQL.AppendLine(" , GETDATE() ")

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" FROM Area_Omogenea_MUZ ")

            StrSQL.AppendLine(String.Format(" WHERE Area_Cod = {0} ", Agro_SQL_SaveNum(Old_Area_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not resp Then
                Throw New Exception("Errore nella clonazione della MUZ.")
            End If
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Area_Cod

    End Function

    Public Function ClonaGisEntita(ByVal nuovo_Area_Cod As Int32,
                                   ByVal vecchio_Area_Cod As Int32,
                                   ByRef objParametri As AgronicaCoreParametri) As Int32

        Dim sequenza As New Agro_Sequenze
        Dim Entita_Cod = sequenza.NuovoId_Tabella("GIS_Entita", 0, Int32.MaxValue, objParametri, True)

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.ClonaGisEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_Entita ( ")

            StrSQL.AppendLine(" PivaSuperUser ")
            StrSQL.AppendLine(" , Entita_Cod ")
            StrSQL.AppendLine(" , TipoEntita_Cod ")
            StrSQL.AppendLine(" , Piva ")
            StrSQL.AppendLine(" , Sa_Cod ")
            StrSQL.AppendLine(" , Appezza ")
            StrSQL.AppendLine(" , Campo_Cod ")
            StrSQL.AppendLine(" , Id_Imp ")
            StrSQL.AppendLine(" , PROV ")
            StrSQL.AppendLine(" , COM ")
            StrSQL.AppendLine(" , SEZIONE ")
            StrSQL.AppendLine(" , FOGLIO ")
            StrSQL.AppendLine(" , NUMERO ")
            StrSQL.AppendLine(" , SUBALTERNO ")
            StrSQL.AppendLine(" , Programmazione_Entita_Cod ")
            StrSQL.AppendLine(" , ID_Agenda ")
            StrSQL.AppendLine(" , Ricetta_Operazione_cod ")
            StrSQL.AppendLine(" , analisi_campione_cod ")
            StrSQL.AppendLine(" , OLDGrafica_ID ")
            StrSQL.AppendLine(" , Data_Creazione ")
            StrSQL.AppendLine(" , Data_Modifica ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" , Validita_Inizio ")
            StrSQL.AppendLine(" , Validita_Fine ")
            StrSQL.AppendLine(" , programmazione_cod ")
            StrSQL.AppendLine(" , id_mov_det ")
            StrSQL.AppendLine(" , GIS_Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" , Entita_GUID ")
            StrSQL.AppendLine(" , ParametriVisualizzazioneLayer ")
            StrSQL.AppendLine(" , Area_Cod ) ")

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" PivaSuperUser ")

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Entita_Cod)))

            StrSQL.AppendLine(" , TipoEntita_Cod ")
            StrSQL.AppendLine(" , Piva ")
            StrSQL.AppendLine(" , Sa_Cod ")
            StrSQL.AppendLine(" , Appezza ")
            StrSQL.AppendLine(" , Campo_Cod ")
            StrSQL.AppendLine(" , Id_Imp ")
            StrSQL.AppendLine(" , PROV ")
            StrSQL.AppendLine(" , COM ")
            StrSQL.AppendLine(" , SEZIONE ")
            StrSQL.AppendLine(" , FOGLIO ")
            StrSQL.AppendLine(" , NUMERO ")
            StrSQL.AppendLine(" , SUBALTERNO ")
            StrSQL.AppendLine(" , Programmazione_Entita_Cod ")
            StrSQL.AppendLine(" , ID_Agenda ")
            StrSQL.AppendLine(" , Ricetta_Operazione_cod ")
            StrSQL.AppendLine(" , analisi_campione_cod ")
            StrSQL.AppendLine(" , OLDGrafica_ID ")
            StrSQL.AppendLine(" , GETDATE() ")
            StrSQL.AppendLine(" , GETDATE() ")

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" , Validita_Inizio ")
            StrSQL.AppendLine(" , Validita_Fine ")
            StrSQL.AppendLine(" , programmazione_cod ")
            StrSQL.AppendLine(" , id_mov_det ")
            StrSQL.AppendLine(" , 0 ")
            StrSQL.AppendLine(" , null ")
            StrSQL.AppendLine(" , '' ")

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(nuovo_Area_Cod)))

            StrSQL.AppendLine(" FROM GIS_Entita ")

            StrSQL.AppendLine(String.Format(" WHERE Area_Cod = {0} ", Agro_SQL_SaveNum(vecchio_Area_Cod)))


            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not resp Then
                Throw New Exception("Errore nella clonazione dell'Entità.")
            End If
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Entita_Cod

    End Function

    Public Function AssociaMUZAdAppezzamento(ByVal Piva As String,
                                                ByVal Sa_Cod As Int32,
                                                ByVal Appezza As Int32,
                                                ByVal Area_Cod As Int32,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.AssociaMUZAdAppezzamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Appezzamento_XArea_Omogenea ( ")
            StrSQL.AppendLine(" Piva ")
            StrSQL.AppendLine(" , Sa_Cod ")
            StrSQL.AppendLine(" , Appezza ")
            StrSQL.AppendLine(" , Area_Cod ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")


            StrSQL.AppendLine(" ) VALUES ( ")
            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(Piva)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Sa_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Appezza)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Area_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ) ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function DeleteDataMUZ(ByVal Area_Cod As Int32,
                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        resp = DisassociaAnalisiDaMUZ(Area_Cod, 0, objParametri)

        If Not resp Then
            Return resp
        End If

        resp = DisassociaMUZDaParticellaCatastale(Area_Cod, Nothing, objParametri)

        If Not resp Then
            Return resp
        End If

        resp = DisassociaMUZDaGruppo(Area_Cod, 0, objParametri)

        If Not resp Then
            Return resp
        End If

        resp = DisassociaMUZDaAppezzamento(Area_Cod, Nothing, objParametri)

        Return resp
    End Function

    Private Function DisassociaMUZDaAppezzamento(ByVal area_Cod As Int32,
                                                 ByVal appezzamento As Appezzamento_MUZ,
                                                 ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DisassociaMUZDaAppezzamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Appezzamento_XArea_Omogenea ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            If appezzamento IsNot Nothing Then
                StrSQL.AppendLine(String.Format(" AND  Piva = '{0}' ", Agro_SQL_SaveText(appezzamento.Piva)))
                StrSQL.AppendLine(String.Format(" AND  Sa_Cod = {0} ", Agro_SQL_SaveNum(appezzamento.Sa_Cod)))
                StrSQL.AppendLine(String.Format(" AND  Appezza = {0} ", Agro_SQL_SaveNum(appezzamento.Appezza)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Private Function DisassociaMUZDaGruppo(ByVal area_Cod As Int32,
                                           ByVal gruppo_Area_Cod As Int32,
                                           ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DisassociaMUZDaGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Area_Omogenea_XGruppo_Area_Omogenea ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            If gruppo_Area_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" AND Gruppo_Area_Cod = {0} ", Agro_SQL_SaveNum(gruppo_Area_Cod)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function

    Public Function DeleteEntitaMUZ(ByVal area_Cod As Int32,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_MUZ_W.DeleteEntitaMUZ()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM GIS_ElementiGrafici ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Entita_Cod = (SELECT Entita_Cod FROM GIS_ENTITA WHERE Area_Cod = {0}) ", Agro_SQL_SaveNum(area_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not resp Then
                Return resp
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM GIS_ENTITA ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Area_Cod = {0} ", Agro_SQL_SaveNum(area_Cod)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return resp

    End Function
End Class
