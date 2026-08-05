Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AppezzaxParticellexMacrousixUtilizzo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi(ByVal Piva As String, _
                          ByVal Sa_Cod As Int32, _
                          ByVal Appezza As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal Macrouso_Cod As String, _
                                    ByVal Veg_Cod_Agea As String, _
                                    ByVal Cul_Cod_Agea As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            'TODO
            'modificare query
            StrSQL.Length = 0
            StrSQL.Append(" SELECT AppezzamentiXParticellexMacrousixUtilizzo.*, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des, ")
            StrSQL.Append(" ParticelleCatastali.ETTARI, ParticelleCatastali.ARE, ParticelleCatastali.CENTIARE, ")
            StrSQL.Append(" ImpreseXParticelle.TitoloPossesso, ImpreseXParticelle.Validita_Inizio AS Validita_Inizio_Possesso, ImpreseXParticelle.Validita_Fine AS Validita_Fine_Possesso, ")
            StrSQL.Append(" ImpreseXParticelle.Sup_Condotta, AppezzamentiXParticelle.AREA,  ")
            StrSQL.Append(" Appezzamento.Campo_Cod, Appezzamento.SUP_APP, Appezzamento.APP_NOME, Appezzamento.Validita_Inizio AS Validita_Inizio_Appezzamento, Appezzamento.Validita_Fine AS Validita_Fine_Appezzamento ")


            StrSQL.Append(" FROM   AppezzamentiXParticelle INNER JOIN  ")
            StrSQL.Append("        AppezzamentiXParticellexMacrousixUtilizzo ON AppezzamentiXParticelle.PIVA = AppezzamentiXParticellexMacrousixUtilizzo.Piva AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.SA_COD = AppezzamentiXParticellexMacrousixUtilizzo.Sa_cod AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.PROV = AppezzamentiXParticellexMacrousixUtilizzo.PROV AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.APPEZZA = AppezzamentiXParticellexMacrousixUtilizzo.Appezza AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.COM = AppezzamentiXParticellexMacrousixUtilizzo.COM AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.SEZIONE = AppezzamentiXParticellexMacrousixUtilizzo.SEZIONE AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.FOGLIO = AppezzamentiXParticellexMacrousixUtilizzo.FOGLIO AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.NUMERO = AppezzamentiXParticellexMacrousixUtilizzo.NUMERO AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.SUBALTERNO = AppezzamentiXParticellexMacrousixUtilizzo.SUBALTERNO INNER JOIN  ")
            StrSQL.Append("        ParticelleCatastali INNER JOIN  ")
            StrSQL.Append("        ImpreseXParticelle ON ParticelleCatastali.PROV = ImpreseXParticelle.PROV AND ParticelleCatastali.COM = ImpreseXParticelle.COM AND   ")
            StrSQL.Append("        ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE AND ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO AND   ")
            StrSQL.Append("        ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO AND ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ON   ")
            StrSQL.Append("        AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND AppezzamentiXParticelle.COM = ImpreseXParticelle.COM AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND AppezzamentiXParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO INNER JOIN  ")
            StrSQL.Append("        Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND   ")
            StrSQL.Append("        AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA LEFT OUTER JOIN ")
            StrSQL.Append("        Macrousi ON AppezzamentiXParticellexMacrousixUtilizzo.Macrouso_Cod = Macrousi.Macrouso_Cod ")

            StrSQL.Append(" WHERE AppezzamentiXParticellexMacrousixUtilizzo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Macrouso_Cod = '" & Agro_SQL_SaveText(Trim(LCase(Macrouso_Cod))) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            End If
            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY AppezzamentiXParticellexMacrousixUtilizzo.Appezza ASC")
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


    '============================================================================
    Public Function Leggi_SuperficieMacrousoxUtilizzo(ByVal Piva As String, _
                          ByVal Sa_Cod As Int32, _
                          ByVal Appezza As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal Macrouso_Cod As String, _
                                    ByVal Veg_Cod_Agea As String, _
                                    ByVal Cul_Cod_Agea As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R.Leggi_SuperficieMacrousoxUtilizzo()"

        '====================================================================================
        'Parametri opzionali :    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            'TODO
            'modificare query
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Sum(Superficie) AS Superficie ")


            StrSQL.Append(" FROM   AppezzamentiXParticellexMacrousixUtilizzo ")

            StrSQL.Append(" WHERE AppezzamentiXParticellexMacrousixUtilizzo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Piva <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Macrouso_Cod = '" & Agro_SQL_SaveText(Trim(LCase(Macrouso_Cod))) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            End If
            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND AppezzamentiXParticellexMacrousixUtilizzo.Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   AppezzamentiXParticellexMacrousixUtilizzo.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
End Class

Public Class AppezzaxParticellexMacrousixUtilizzo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi( _
                    ByVal Piva As String, _
                    ByVal Sa_Cod As Int32, _
                    ByVal Appezza As Int32, _
                    ByVal PROV As String, _
                    ByVal COM As String, _
                    ByVal SEZIONE As String, _
                    ByVal FOGLIO As Int32, _
                    ByVal NUMERO As Int32, _
                    ByVal SUBALTERNO As String, _
                    ByVal Macrouso_Cod As String, _
                    ByVal Veg_Cod_Agea As String, _
                    ByVal Cul_Cod_Agea As String, _
                    ByVal Superficie As Decimal, _
                    ByVal Validita_Inizio As Date, _
                    ByVal Validita_Fine As Date, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = "" _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W.Scrivi()"

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



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO AppezzamentiXParticellexMacrousixUtilizzo(       ")
            StrSQL.Append("                    PIVA, Sa_Cod, Appezza, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO,  Macrouso_Cod, ")
            StrSQL.Append("                    Veg_Cod_Agea, Cul_Cod_Agea, Superficie,  ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Macrouso_Cod)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Superficie) & " ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
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

    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal Macrouso_Cod As String, _
                                    ByVal Veg_Cod_Agea As String, _
                                    ByVal Cul_Cod_Agea As String, _
                                    ByVal Superficie As Decimal, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W.Modifica()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE AppezzamentiXParticellexMacrousixUtilizzo SET ")
            StrSQL.Append("        Superficie  = " & Agro_SQL_SaveNum(Superficie) & " ")
            StrSQL.Append("       ,Inviato              =  0 ")
            StrSQL.Append("       ,DataInvio            =  Null ")
            StrSQL.Append("       ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("       ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND      Appezza     =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append(" AND      Macrouso_Cod  = '" & Agro_SQL_SaveText(Trim(Macrouso_Cod)) & "' ")
            StrSQL.Append(" AND      Veg_Cod_Agea  = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "'  ")
            StrSQL.Append(" AND      Cul_Cod_Agea  = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "'  ")


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


    Public Function Modifica_Parametrizzata(
                                          ByVal ID As Integer,
                                          ByVal Campo As String,
                                          ByVal Valore As Object,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentiXParticellexMacrousixUtilizzo_W.Modifica_Parametrizzata()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        'Dim Intero32 As Type = GetType(System.Int32)
        'Dim Doubl As Type = GetType(System.Decimal)


        Try

            If ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE AppezzamentiXParticellexMacrousixUtilizzo SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE ID = " & Agro_SQL_SaveNum(ID) & " ")

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

    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal Macrouso_Cod As String, _
                                    ByVal Veg_Cod_Agea As String, _
                                    ByVal Cul_Cod_Agea As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W.Cancella()"

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


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE AppezzamentiXParticellexMacrousixUtilizzo ")
                StrSQL.Append(" SET ")
                StrSQL.Append("       Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("      ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     AppezzamentiXParticellexMacrousixUtilizzo ")
                StrSQL.Append(" WHERE    PIVA <> '0' ")

            End If

            If Piva <> "" Then
                StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND   Macrouso_Cod = '" & Agro_SQL_SaveText(LCase(Macrouso_Cod)) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND Veg_Cod_Agea =  '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            End If
            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND Cul_Cod_Agea =  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If

            '---------------------------------------------


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

    'Versione che utilizza AgronicaCoreParametri
    Public Function AggiornaValiditaFine( _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Int32, _
                                        ByVal Id_Campo As Integer, _
                                        ByVal Appezza As Int32, _
                                        ByVal PROV As String, _
                                        ByVal COM As String, _
                                        ByVal SEZIONE As String, _
                                        ByVal FOGLIO As Int32, _
                                        ByVal NUMERO As Int32, _
                                        ByVal SUBALTERNO As String, _
                                        ByVal Macrouso_Cod As String, _
                                            ByVal Veg_Cod_Agea As String, _
                                            ByVal Cul_Cod_Agea As String, _
                                            ByVal Validita_Fine As Date, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  aggiorna tutte le particelle dell'impresa
        '   Appezza = 0          =>  aggiorna tutte le particelle del centro aziendale
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE AppezzamentiXParticellexMacrousixUtilizzo SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Id_Campo <> 0 Then
                StrSQL.Append(" AND  Appezza IN ( Select Appezzamento.Appezza From Appezzamento ")
                StrSQL.Append(" Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo))
                StrSQL.Append(" And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                StrSQL.Append(" And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND   Macrouso_Cod = '" & Agro_SQL_SaveText(LCase(Macrouso_Cod)) & "' ")
            End If

            If Veg_Cod_Agea <> "" Then
                StrSQL.Append(" AND   Veg_Cod_Agea = '" & Agro_SQL_SaveText(LCase(Veg_Cod_Agea)) & "' ")
            End If

            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND   Cul_Cod_Agea = '" & Agro_SQL_SaveText(LCase(Cul_Cod_Agea)) & "' ")
            End If

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function



End Class
