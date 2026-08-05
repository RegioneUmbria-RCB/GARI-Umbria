Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class AppezzaxParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function AppezzamentixParticelle_Leggi(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal PROV As String,
                                                  ByVal COM As String,
                                                  ByVal SEZIONE As String,
                                                  ByVal FOGLIO As Integer,
                                                  ByVal NUMERO As Integer,
                                                  ByVal SUBALTERNO As String,
                                                  ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal SUBALTERNOObbligatorio As Boolean,
                                                  Optional ByVal joinImpreseParticelle As Boolean = False,
                                                  Optional ByVal idImpreseParticelleDaEscludere As Integer = 0,
                                                  Optional ByVal filtroImpreseParticelleAssenti As Boolean = False
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.AppezzamentixParticelle_Leggi()"

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
            'Select Case xSelezioneVariabile

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  AppezzamentixParticelle.*, ")
            StrSQL.Append(" AppezzamentixParticelle.Validita_Inizio as xValidita_Inizio, ")
            StrSQL.Append(" AppezzamentixParticelle.Validita_Fine as xValidita_Fine, ")
            StrSQL.Append(" Appezzamento.Campo_Cod, ISNULL(Campi.Campo_Des,'') AS Campo_Des, ")
            StrSQL.Append(" Appezzamento.App_Nome, Appezzamento.SUP_APP, Appezzamento.Validita_Inizio as Validita_Inizio_App, Appezzamento.Validita_Fine as Validita_Fine_App,")
            StrSQL.Append(" ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
            StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
            StrSQL.Append(" AppezzamentixParticelle.Piva, Imprese.rag_soc, AppezzamentixParticelle.Sa_Cod, Centri_Aziendali.sa_nome ")

            StrSQL.Append(" FROM  AppezzamentiXParticelle INNER JOIN  ")
            StrSQL.Append(" Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND  ")
            StrSQL.Append(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
            StrSQL.Append(" ParticelleCatastali ON AppezzamentiXParticelle.PROV = ParticelleCatastali.PROV AND AppezzamentiXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.Append(" AppezzamentiXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND AppezzamentiXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.Append(" AppezzamentiXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ")
            StrSQL.Append(" AppezzamentiXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.Append(" ISTAT ON AppezzamentiXParticelle.PROV = ISTAT.PROV AND AppezzamentiXParticelle.COM = ISTAT.COM INNER JOIN ")
            StrSQL.Append(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
            StrSQL.Append(" Centri_Aziendali ON AppezzamentiXParticelle.PIVA = Centri_Aziendali.PIVA AND  ")
            StrSQL.Append(" AppezzamentiXParticelle.SA_COD = Centri_Aziendali.sa_cod INNER JOIN ")
            StrSQL.Append(" Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA LEFT OUTER JOIN ")
            StrSQL.Append(" Campi ON AppezzamentiXParticelle.PIVA = Campi.Piva AND Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND ")
            StrSQL.Append(" Appezzamento.Campo_Cod = Campi.Campo_Cod ")

            If joinImpreseParticelle Then

                StrSQL.Append(" LEFT OUTER JOIN ImpreseXParticelle IxP ON")
                StrSQL.Append(" IxP.PIVA = AppezzamentiXParticelle.PIVA AND")
                StrSQL.Append(" IxP.SA_COD = AppezzamentiXParticelle.SA_COD AND")
                StrSQL.Append(" IxP.PROV = AppezzamentiXParticelle.PROV AND")
                StrSQL.Append(" IxP.COM = AppezzamentiXParticelle.COM AND")
                StrSQL.Append(" IxP.SEZIONE = AppezzamentiXParticelle.SEZIONE AND")
                StrSQL.Append(" IxP.FOGLIO = AppezzamentiXParticelle.FOGLIO AND")
                StrSQL.Append(" IxP.NUMERO = AppezzamentiXParticelle.NUMERO AND")
                StrSQL.Append(" IxP.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO AND")
                StrSQL.Append(" IxP.Validita_Inizio <= AppezzamentiXParticelle.Validita_Fine AND ")
                StrSQL.Append(" IxP.Validita_Fine >= AppezzamentiXParticelle.Validita_Inizio")

                If idImpreseParticelleDaEscludere <> 0 Then
                    StrSQL.Append(" AND IxP.ID <> " & Agro_SQL_SaveNum(idImpreseParticelleDaEscludere) & "  ")
                End If

            End If

            StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.Append(" AND     (AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            '----- Condizioni

            If (Piva <> "") Then
                StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND AppezzamentixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND AppezzamentixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "0" Then
                StrSQL.Append(" AND AppezzamentixParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNOObbligatorio Or SUBALTERNO <> "0" Then
                StrSQL.Append(" AND AppezzamentixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If joinImpreseParticelle AndAlso filtroImpreseParticelleAssenti Then
                StrSQL.Append(" AND IxP.ID IS NULL ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
            '        '
            '        '

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
            '        '
            '        '
            '        '
            '        '

            'End Select

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



    '##############################################################################################
    Public Function Recupera_Somma_Superfici_Particella_intersecata_con_Appezzamenti(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Appezza As Integer,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Int32,
                                ByVal NUMERO As Int32,
                                ByVal SUBALTERNO As String,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Decimal


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Recupera_Somma_Superfici_Particella_intersecata_con_Appezzamenti()"


        Dim DTParticella As DataTable
        Dim Somma As Decimal = 0
        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

        'modifico la data
        'Dim app_data As Date
        'app_data = objParametri.FinestraTemporaleFine
        'objParametri.FinestraTemporaleFine = Now.Date

        DTParticella = objAppezzaxParticelle.AppezzamentixParticelle_Leggi(
                                                      CStr(PIVA),
                                                      CInt(Sa_Cod),
                                                      CInt(Appezza),
                                                      PROV,
                                                      COM,
                                                      SEZIONE,
                                                      FOGLIO,
                                                      NUMERO,
                                                      SUBALTERNO,
                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "AppezzamentixParticelle.validita_fine >= " & Agro_SQL_SaveDate(Now.Date),
                                                            "",
                                                            objParametri, False)

        'reimposto la data
        'objParametri.FinestraTemporaleFine = app_data

        'Filtro per ottenere solo le particelle attive
        'RsParticella.Filter = "validita_fine >= #" & Now.Date & "# "
        Dim i As Integer
        If Not IsNothing(DTParticella) And DTParticella.Rows.Count <> 0 Then
            For i = 0 To DTParticella.Rows.Count - 1

                Somma = Somma + DTParticella.Rows(i).Item("Area")

                'Passo al prossimo record
            Next
        End If

        Return Somma

    End Function





    ''Trova Tutte le particelle associate ad un dato appezzamento (Gias Lan: vedi FrmAppezzamento)
    ''============================================================================
    'Public Function LeggiParticelle_Da_Appezzamento(ByVal Piva As String, _
    '                                                ByVal Sa_Cod As Long, _
    '                                                ByVal Appezza As Long, _
    '                                                ByVal FinestraTemp_Inizio As Date, _
    '                                                ByVal FinestraTemp_Fine As Date, _
    '                                                ByRef objConnessione As DbConnection, _
    '                                                ByVal StringaConnessione As String, _
    '                                                ByVal FlagVisibilita As Int32, _
    '                                                ByVal DirectoryLOG As String, _
    '                                                ByVal FileLOG As String, _
    '                                                ByVal IdentificatoreUtente As String _
    '                                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  si leggono tutte le imprese
    '    '   Sa_Cod = 0
    '    '   Appezza = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------


    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod ")
    '        StrSQL.Append(" FROM   AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")

    '        StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
    '        StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        If Appezza <> 0 Then
    '            StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
    '        End If

    '        '---------------------------------------------
    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function


    'Versione che utilizza AgronicaCoreParametri
    'Trova Tutte le particelle associate ad un dato appezzamento (Gias Lan: vedi FrmAppezzamento)
    '============================================================================
    Public Function LeggiParticelle_Da_Appezzamento(ByVal Piva As String,
                                                    ByVal Sa_Cod As Long,
                                                    ByVal Appezza As Long,
                                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* ")
                    StrSQL.Append(" FROM   AppezzamentixParticelle ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod , ISTAT.LOCALITA AS Comune , ISTAT.COMUNI_PROV As Provincia ")
                    StrSQL.AppendLine(" FROM   AppezzamentixParticelle , ParticelleCatastali , Appezzamento, ISTAT  ")

                    StrSQL.AppendLine(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")
                    StrSQL.AppendLine(" AND   AppezzamentixParticelle.Com = ISTAT.COM ")
                    StrSQL.AppendLine(" AND   AppezzamentiXParticelle.prov = ISTAT.PROV ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
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

    'Versione che utilizza AgronicaCoreParametri
    'Trova Tutte le particelle associate ad un dato appezzamento (Gias Lan: vedi FrmAppezzamento)
    '============================================================================
    Public Function LeggiParticelle_Da_Appezzamento_MinMaxPossesso(ByVal Piva As String,
                                                                    ByVal Sa_Cod As Long,
                                                                    ByVal Appezza As Long,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento_MinMaxPossesso()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try




            StrSQL.Length = 0
            StrSQL.Append(" SELECT AppezzamentixParticelle.* , ")

            StrSQL.Append(" (SELECT MAX(ImpreseXParticelle.Validita_Inizio)  ")
            StrSQL.Append(" FROM ImpreseXParticelle INNER JOIN ")
            StrSQL.Append(" AppezzamentixParticelle ON ImpreseXParticelle.PROV = AppezzamentixParticelle.Prov AND ImpreseXParticelle.COM = AppezzamentixParticelle.Com AND  ")
            StrSQL.Append(" ImpreseXParticelle.SEZIONE = AppezzamentixParticelle.Sezione AND ImpreseXParticelle.FOGLIO = AppezzamentixParticelle.Foglio AND  ")
            StrSQL.Append(" ImpreseXParticelle.NUMERO = AppezzamentixParticelle.Numero AND ImpreseXParticelle.SUBALTERNO = AppezzamentixParticelle.Subalterno ")
            StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND AppezzamentixParticelle.Validita_Inizio <= ImpreseXParticelle.Validita_Fine ")

            If Piva <> "" Then
                StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            StrSQL.Append(" ) AS Inizio_Possesso, ")

            StrSQL.Append(" (SELECT MIN(ImpreseXParticelle.Validita_Fine)  ")
            StrSQL.Append(" FROM ImpreseXParticelle INNER JOIN ")
            StrSQL.Append(" AppezzamentixParticelle ON ImpreseXParticelle.PROV = AppezzamentixParticelle.Prov AND ImpreseXParticelle.COM = AppezzamentixParticelle.Com AND  ")
            StrSQL.Append(" ImpreseXParticelle.SEZIONE = AppezzamentixParticelle.Sezione AND ImpreseXParticelle.FOGLIO = AppezzamentixParticelle.Foglio AND  ")
            StrSQL.Append(" ImpreseXParticelle.NUMERO = AppezzamentixParticelle.Numero AND ImpreseXParticelle.SUBALTERNO = AppezzamentixParticelle.Subalterno ")
            StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_Fine >= ImpreseXParticelle.Validita_Inizio AND AppezzamentixParticelle.Validita_Inizio <= ImpreseXParticelle.Validita_Fine ")

            If Piva <> "" Then
                StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If
            StrSQL.Append(" ) AS Fine_Possesso ")

            StrSQL.Append(" FROM   AppezzamentixParticelle  ")

            StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
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

    'Trova Tutte le particelle associate ad un dato appezzamento (Gias Lan: vedi FrmAppezzamento)
    '============================================================================
    Public Function LeggiParticelle_con_Macrousi_Da_Appezzamento(ByVal Piva As String,
                                                    ByVal Sa_Cod As Long,
                                                    ByVal Appezza As Long,
                                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* ")
                    StrSQL.Append(" FROM   AppezzamentixParticelle ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod ")
                    StrSQL.Append(" FROM   AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")

                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND AppezzamentixParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
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


    Public Function LeggiAppezzamenti_Da_ParticellaGis(
        ByVal mostraRipartoAppezzamenti As Boolean,
        ByVal mostraAppezzamentoSenzaRiparto As Boolean,
        ByVal mostraParticelleSenzaRiparto As Boolean,
        ByVal gisEntita_Cod As Integer,
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal idTestataTemp As Integer,
        ByVal LeggiDettagliKendoGrid As Boolean,
        ByVal stbAgea2015 As StringBuilder,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

        '====================================================================================
        'Parametri opzionali :    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try

            Dim UnionRichiesto As Boolean = False

            Dim conteggioXQueryAggrega As Integer = 0
            If mostraRipartoAppezzamenti Then
                conteggioXQueryAggrega += 1
            End If
            If mostraAppezzamentoSenzaRiparto Then
                conteggioXQueryAggrega += 1
            End If
            If mostraParticelleSenzaRiparto Then
                conteggioXQueryAggrega += 1
            End If

            If conteggioXQueryAggrega > 1 Then
                stb.AppendLine("select * ")
                stb.AppendLine("from (")
            End If


            If mostraRipartoAppezzamenti Then

                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery(
                enum_ParticelleAppezza_TipoQuery.RipartoAppezzamentiSuParticelle,
                gisEntita_Cod,
                piva,
                sa_Cod,
                DataDa,
                DataA,
                idTestataTemp,
                LeggiDettagliKendoGrid,
                stbAgea2015,
                stb)

                UnionRichiesto = True

            End If

            If mostraParticelleSenzaRiparto Then

                If UnionRichiesto Then
                    stb.AppendLine(" UNION ALL ")
                End If

                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery(
                                enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto,
                                gisEntita_Cod,
                                piva,
                                sa_Cod,
                                DataDa,
                                DataA,
                                idTestataTemp,
                                LeggiDettagliKendoGrid,
                                New StringBuilder,
                                stb)

                UnionRichiesto = True

            End If

            If mostraAppezzamentoSenzaRiparto Then

                If UnionRichiesto Then
                    stb.AppendLine(" UNION ALL ")
                End If

                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery(
                                enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto,
                                gisEntita_Cod,
                                piva,
                                sa_Cod,
                                DataDa,
                                DataA,
                                idTestataTemp,
                                LeggiDettagliKendoGrid,
                                stbAgea2015,
                                stb)

            End If

            If conteggioXQueryAggrega > 1 Then
                stb.AppendLine(") agg1")
            End If

            'Se l'utente non imposta nessun check, non viene creata la query sopra, non posso aggiungere l'order by
            If LeggiDettagliKendoGrid AndAlso stb.Length > 0 Then
                stb.AppendLine("ORDER BY chiave")
            End If

            'Se l'utente non imposta nessun check, non viene creata la query sopra, non posso eseguirla
            If stb.Length > 0 Then
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



    Friend Enum enum_ParticelleAppezza_TipoQuery
        NonSpecificato = 0
        RipartoAppezzamentiSuParticelle = 1
        AppezzamentoSenzaRiparto = 2
        ParticelleSenzaRiparto = 3
    End Enum

    Private Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery(
            TipoQuery As enum_ParticelleAppezza_TipoQuery,
            gisEntita_Cod As Integer,
            piva As String,
            sa_Cod As Integer,
            DataDa As Date,
            DataA As Date,
            idTestataTemp As Integer,
            LeggiDettagliKendoGrid As Boolean,
            stbAgea2015 As StringBuilder,
            stb As StringBuilder)

        Dim aliasTabellaParticelle As String = "iPart"
        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            aliasTabellaParticelle = "IPP"
        End If

        stb.AppendLine("SELECT")

        Select Case TipoQuery
            Case enum_ParticelleAppezza_TipoQuery.RipartoAppezzamentiSuParticelle
                stb.AppendLine("	'Appezzamenti con riparto' AS TipoDatoEstratto,")
            Case enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto
                stb.AppendLine("    'Appezzamenti senza riparto' AS TipoDatoEstratto,")
            Case enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto
                stb.AppendLine("    'Particelle in conduzione senza riparto' AS TipoDatoEstratto,")
        End Select


        stb.AppendLine("	ISNULL(ii.rag_soc, '') AS PadreInGerarchia,")

        If LeggiDettagliKendoGrid Then
            stb.AppendLine("	ISNULL(iOrganismoReferente.rag_soc, '') AS OrganismoReferente,")
        Else
            stb.AppendLine("	'' AS OrganismoReferente,")
        End If

        stb.AppendLine("	i.piva,")
        stb.AppendLine("    i.Rag_Soc,")
        stb.AppendLine("	ISNULL(cuaa.val_Cod, '') AS cuaa,")
        stb.AppendLine("	sa.sa_nome,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".PROV, '') AS PROV,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".com, '') AS com,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".SEZIONE, '') AS SEZIONE,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".FOGLIO, 0) AS FOGLIO,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".NUMERO, 0) AS NUMERO,")
        stb.AppendLine("    ISNULL(" & aliasTabellaParticelle & ".SUBALTERNO, '') AS SUBALTERNO,")

        If LeggiDettagliKendoGrid Then
            stb.AppendLine("    ISNULL( istat.CAP, '') AS CAP,")
        End If

        stb.AppendLine("")
        If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            stb.AppendLine("	(")
            stb.AppendLine("        CAST(ParticelleCatastali.ETTARI AS FLOAT) +")
            stb.AppendLine("		(CAST(ParticelleCatastali.ARE AS DECIMAL) * 0.01) +")
            stb.AppendLine("		(CAST(ParticelleCatastali.CENTIARE AS DECIMAL) * 0.0001 )")
            stb.AppendLine("    ) AS ParticellaSuperficieHa,")
            stb.AppendLine("")
            stb.AppendLine("	ParticelleCatastali.ETTARI,")
            stb.AppendLine("	ParticelleCatastali.ARE,")
            stb.AppendLine("	ParticelleCatastali.CENTIARE,")
        Else
            stb.AppendLine("	(")
            stb.AppendLine("        CAST(cc.ETTARI AS FLOAT) +")
            stb.AppendLine("		(CAST(cc.ARE AS DECIMAL) * 0.01) +")
            stb.AppendLine("		(CAST(cc.CENTIARE AS DECIMAL) * 0.0001 )")
            stb.AppendLine("    ) AS ParticellaSuperficieHa,")
            stb.AppendLine("")
            stb.AppendLine("	cc.ETTARI,")
            stb.AppendLine("	cc.ARE,")
            stb.AppendLine("	cc.CENTIARE,")
        End If



        If LeggiDettagliKendoGrid Then
            stb.AppendLine("	ISNULL(a.APP_NOME, '') AS APP_NOME,")
            stb.AppendLine("	ISNULL(a.Sup_App, 0) AS Sup_App,")
            stb.AppendLine("	ISNULL(cp.campo_Des, '') AS campo_des,")

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                stb.AppendLine("")
                stb.AppendLine("	ISNULL(")
                stb.AppendLine("		indApp.ind_des + ', ' +")
                stb.AppendLine("		indApp.frz_des + ' - ' +")
                stb.AppendLine("		indApp.CAP + ' - ' +")
                stb.AppendLine("		indApp.com_des + ' ' +")
                stb.AppendLine("		ISNULL(istatApp.LOCALITA, '') + ' ' +")
                stb.AppendLine("		ISNULL(istatApp.COMUNI_PROV, '') + ' ' +")
                stb.AppendLine("        ISNULL(istatApp.CAP, ''), ''")
                stb.AppendLine("    ) AS IndirizzoAppezzamento,")
            Else
                stb.AppendLine("    '' AS IndirizzoAppezzamento,")
            End If

        Else
            stb.AppendLine("    a.APP_NOME + ISNULL(' - ' + cp.campo_Des, '') as APP_nome,  ")
        End If

        stb.AppendLine("")
        stb.AppendLine("    ISNULL(a.sa_cod, 0) AS sa_cod,")
        stb.AppendLine("    ISNULL(a.appezza, 0 ) AS appezza,")

        If LeggiDettagliKendoGrid Then
            stb.AppendLine("    ISNULL(dist.id_reg, 0) AS id_reg,")
            stb.AppendLine("	ISNULL(dist.progetto_cod, 0) AS progetto_cod,")
            stb.AppendLine("	ISNULL(dist.Progetto_Nome, '') AS LottoEsercizio,")
            stb.AppendLine("    ISNULL(dist.Progetto_Des, '') AS RiferimentoEsercizio,")
            stb.AppendLine("    ISNULL(DATEDIFF(m, dist.validita_inizio, DATEADD(d, 1, dist.validita_fine)), 0) AS MesiValiditaEsercizio,")
        End If

        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            stb.AppendLine("    0.0 AS AREA,")
            stb.AppendLine("    " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AS Riparto_Validita_Inizio,")
            stb.AppendLine("    " & Agro_SQL_SaveDate(AGRODATAFINE) & " AS Riparto_Validita_Fine,")
            stb.AppendLine("    NULL AS Appezzamento_Validita_Inizio,")
            stb.AppendLine("    NULL AS Appezzamento_Validita_Fine,")
        Else
            stb.AppendLine("")
            stb.AppendLine("	ipart.AREA,")
            stb.AppendLine("	ipart.validita_inizio AS Riparto_Validita_Inizio,")
            stb.AppendLine("    ipart.validita_fine AS Riparto_Validita_Fine,")
            stb.AppendLine("    a.validita_inizio AS Appezzamento_Validita_Inizio,")
            stb.AppendLine("    a.validita_fine AS Appezzamento_Validita_Fine,")
        End If


        stb.AppendLine("    ISNULL(istat.comuni_prov, '') AS comuni_prov,")
        stb.AppendLine("    ISNULL(istat.localita, '') AS localita,")

        stb.AppendLine("    ISNULL(ac.val_Cod, '') AS RiferimentoAppezzamento,")

        stb.AppendLine("")
        stb.AppendLine("	CASE WHEN ePart.prov IS NULL")
        stb.AppendLine("		THEN 'No'")
        stb.AppendLine("		ELSE 'Sì'")
        stb.AppendLine("	END AS ePart,")

        If LeggiDettagliKendoGrid Then

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_PianteHaSelect(False, "PianteHa", stb)
                stb.Append(",")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_PianteHaSelect(True, "PianteHa_Impianto", stb)
                stb.Append(",")

                stb.AppendLine("")
                stb.AppendLine("	A2015.Macrouso_DES,")
                stb.AppendLine("	reg.Sup_imp,")
                stb.AppendLine("	ISNULL(NAppBio.val_Cod, '') AS N_AppezzamentoBiologico,")

                stb.AppendLine("")
                stb.AppendLine("	ISNULL(")
                stb.AppendLine("		CASE WHEN MetodoProd.val_Cod = " & CInt(enum_MetodoProduzione.Biologico) & " THEN 'Biologico' ELSE")
                stb.AppendLine("			CASE WHEN MetodoProd.val_Cod = " & CInt(enum_MetodoProduzione.InConversione) & " THEN 'In Conversione' ELSE")
                stb.AppendLine("				CASE WHEN MetodoProd.val_Cod = " & CInt(enum_MetodoProduzione.Integrato) & " THEN 'Integrato' ELSE")
                stb.AppendLine("					NULL")
                stb.AppendLine("				END")
                stb.AppendLine("			END")
                stb.AppendLine("		END, ''")
                stb.AppendLine("	) AS N_MetodoProduzione,")
            Else
                stb.AppendLine("")
                stb.AppendLine("    0.0 AS PianteHA,")
                stb.AppendLine("    0.0 AS PianteHa_Impianto,")
                stb.AppendLine("    '' AS Macrouso_DES,")
                stb.AppendLine("    0.0 AS SUP_Imp,")
                stb.AppendLine("    '' AS N_AppezzamentoBiologico,")
                stb.AppendLine("    '' AS N_MetodoProduzione,")
            End If

            stb.AppendLine("")
            stb.AppendLine("	CASE WHEN eApp.piva  IS NULL THEN 'No' ELSE 'Sì' END AS eApp,")
            stb.AppendLine("	CASE WHEN eImp.piva  IS NULL THEN 'No' ELSE 'Sì' END AS eImp,")

            stb.AppendLine("")
            LatitudineLongitudineDaKWT_QRY("eApp.Baricentro", "Appezzamento_Latitudine", "Appezzamento_Longitudine", True, stb)
            stb.Append(",")

            stb.AppendLine("")
            stb.AppendLine("	CASE WHEN eApp.piva  IS NULL THEN 0 ELSE eApp.Area / 10000 END AS AreaAppezzamentoGIS,")

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Then
                stb.AppendLine("	ISNULL(ipc.val_Cod, '') AS RiferimentoParticella,")
            Else
                stb.AppendLine("    '' AS RiferimentoParticella,")
            End If

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

                stb.AppendLine("	ISNULL(rc.val_Cod, '') AS RiferimentoImpianto,")

                'Salvatore Zammataro: 06/07/2022
                stb.AppendLine("	ISNULL(cacCapPrivato.InfoAgg_Des, '') AS CapitolatoPrivato,")
                stb.AppendLine("	ISNULL(cacDettalioSpeciePersonalizzato.InfoAgg_Des, '') AS DettaglioSpeciePersonalizzato,")
                stb.AppendLine("	ISNULL(Regolamenti.Reg_Des, '') AS Regolamento,")
                stb.AppendLine("	ISNULL(FormeAllevamento.foral_des, '') AS FormaAllevamento,")
                stb.AppendLine("	ISNULL(GruppoVegetale.Gru_Des, '') AS GruppoVegetaleDes,")
                stb.AppendLine("	ISNULL(Copertura.Cop_Des, '') AS Copertura,")
                stb.AppendLine("	ISNULL(Portinnesti.Port_Des, '') AS Portinnesto,")
                stb.AppendLine("	ISNULL(ImpiantiIrrigazioni.Imp_Des, '') AS ImpiantoIrrigazione,")
                '-------------------------------

                stb.AppendLine("	ISNULL(prec1_Veg.veg_Des + ' (' + prec1_Gru.Gru_Des + ')', '') AS Precessione1,")
                stb.AppendLine("	ISNULL(prec2_Veg.veg_Des + ' (' + prec2_Gru.Gru_Des + ')', '') AS Precessione2,")
                stb.AppendLine("	ISNULL(prec3_Veg.veg_Des + ' (' + prec3_Gru.Gru_Des + ')', '') AS Precessione3,")
                stb.AppendLine("	ISNULL(prec4_Veg.veg_Des + ' (' + prec4_Gru.Gru_Des + ')', '') AS Precessione4,")
                stb.AppendLine("	ISNULL(veg.veg_des, codAnagrafeDes.descrizione) AS Specie,")
                stb.AppendLine("	ISNULL(cul.cul_des, '') AS Varieta,")
                stb.AppendLine("	ISNULL(grva.grva_des, '') AS TipologiaVarietale,")
                stb.AppendLine("	ISNULL(grfi.grfi_des, '') AS Finalita,")
            Else

                stb.AppendLine("    '' AS RiferimentoImpianto,")

                'Salvatore Zammataro: 06/07/2022
                stb.AppendLine("	'' AS CapitolatoPrivato,")
                stb.AppendLine("	'' AS DettaglioSpeciePersonalizzato,")
                stb.AppendLine("	'' AS Regolamento,")
                stb.AppendLine("	'' AS FormaAllevamento,")
                stb.AppendLine("	'' AS GruppoVegetaleDes,")
                stb.AppendLine("	'' AS Portinnesto,")
                stb.AppendLine("	'' AS ImpiantoIrrigazione,")
                stb.AppendLine("	'' AS Copertura,")
                '-------------------------------

                stb.AppendLine("    '' AS Precessione1,")
                stb.AppendLine("    '' AS Precessione2,")
                stb.AppendLine("    '' AS Precessione3,")
                stb.AppendLine("    '' AS Precessione4,")
                stb.AppendLine("    '' AS Specie,")
                stb.AppendLine("    '' AS Varieta,")
                stb.AppendLine("    '' AS TipologiaVarietale,")
                stb.AppendLine("    '' AS Finalita,")
            End If



            stb.AppendLine("	ISNULL(dist.produzione_Prevista, 0) AS ResaKgHa,")
            stb.AppendLine("	ISNULL(dist.produzione_Prevista, 0) * reg.sup_imp AS ResaKgHa_Impianto,")

            stb.AppendLine("	reg.validita_inizio AS Impianto_Validita_Inizio,")
            stb.AppendLine("	reg.validita_fine AS Impianto_Validita_Fine,")

            stb.AppendLine("	dist.validita_inizio AS Esercizio_Validita_Inizio,")
            stb.AppendLine("	dist.validita_fine AS Esercizio_Validita_Fine,")

        End If

        stb.AppendLine("	'' AS infoGeneraliRiparto,")

        'dati di dettaglio impianto
        stb.AppendLine("	'' AS Conduzione,")

        'dati di esercizio
        stb.AppendLine("	0.0 AS Piante_HA,")
        stb.AppendLine("	0.0 AS Piante_Impianto,")

        'dati di semina/trapianto
        stb.AppendLine("	'' AS SeminaTrapianto_data,")
        stb.AppendLine("	'' AS SeminaTrapianto_fornitoreSeme,")
        stb.AppendLine("	'' AS SeminaTrapianto_lottoSeme,")

        stb.AppendLine("	0.0  AS SemTrap_Superficie,")
        stb.AppendLine("	0  AS SemTrap_Ha,")
        stb.AppendLine("	0  AS SemTrap_Are,")
        stb.AppendLine("	0  AS SemTrap_Centiare,")

        stb.AppendLine("	'' AS SeminaTrapianto_udm,")
        stb.AppendLine("	0.0  AS SeminaTrapianto_qta,")
        stb.AppendLine("	0.0  AS SeminaTrapianto_qta_ha,")


        stb.AppendLine("	ipart.username_creazione,")
        stb.AppendLine("	ipart.username_modifica,")
        stb.AppendLine("	ipart.data_creazione,")


        If LeggiDettagliKendoGrid Then

            stb.AppendLine("	ipart.data_modifica,")

            If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                stb.AppendLine("    ''                                + '_'+")
                stb.AppendLine("        CAST(0 AS VARCHAR(100))           + '_' +")
                stb.AppendLine("        CAST(0 AS VARCHAR(100))           + '_' +")
                stb.AppendLine("        ipp.PROV                          + '_' +")
                stb.AppendLine("        ipp.com                           + '_' +")
                stb.AppendLine("        ipp.SEZIONE                       + '_' +")
                stb.AppendLine("        CAST(ipp.FOGLIO AS VARCHAR(100))  + '_' +")
                stb.AppendLine("        CAST(ipp.NUMERO AS VARCHAR(100))  + '_' +")
                stb.AppendLine("        ipp.SUBALTERNO ")
                stb.AppendLine("    AS chiave")
            Else
                stb.AppendLine("    iPart.piva                          + '_'+")
                stb.AppendLine("        CAST(iPart.sa_cod AS VARCHAR(100))  + '_'+")
                stb.AppendLine("        CAST(iPart.appezza AS VARCHAR(100)) + '_'+")
                stb.AppendLine("        iPart.PROV                          + '_'+")
                stb.AppendLine("        iPart.com                           + '_'+")
                stb.AppendLine("        iPart.SEZIONE                       + '_'+")
                stb.AppendLine("        CAST(iPart.FOGLIO AS VARCHAR(100))  + '_'+")
                stb.AppendLine("        CAST(iPart.NUMERO AS VARCHAR(100))  + '_'+")
                stb.AppendLine("        iPart.SUBALTERNO")
                stb.AppendLine("    AS chiave")
            End If

        Else
            stb.AppendLine("	ipart.data_modifica ")
        End If


        If TipoQuery = enum_ParticelleAppezza_TipoQuery.RipartoAppezzamentiSuParticelle Then

            stb.AppendLine("")
            stb.AppendLine("FROM AppezzamentiXParticelle ipart")

            stb.AppendLine("")
            stb.AppendLine("	INNER JOIN Appezzamento a")
            stb.AppendLine("		ON  a.PIVA = ipart.PIVA")
            stb.AppendLine("		AND a.SA_COD = ipart.SA_COD")
            stb.AppendLine("		AND a.APPEZZA = ipart.appezza")

        End If

        If TipoQuery = enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Then
            stb.AppendLine("")
            stb.AppendLine("FROM Appezzamento a")

            stb.AppendLine("")
            stb.AppendLine("    LEFT JOIN AppezzamentiXParticelle ipart ON a.PIVA = ipart.PIVA")
            stb.AppendLine("        AND a.SA_COD = ipart.SA_COD")
            stb.AppendLine("        AND a.APPEZZA = ipart.appezza")
        End If


        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

            stb.AppendLine("")
            stb.AppendLine("FROM ParticelleCatastali cc")

            stb.AppendLine("")
            stb.AppendLine("    LEFT JOIN AppezzamentiXParticelle ipart ON cc.PROV = ipart.PROV")
            stb.AppendLine("        AND cc.com = ipart.COM")
            stb.AppendLine("        AND cc.SEZIONE = ipart.SEZIONE")
            stb.AppendLine("        AND cc.FOGLIO = ipart.FOGLIO")
            stb.AppendLine("        AND cc.NUMERO = ipart.NUMERO")
            stb.AppendLine("        AND cc.SUBALTERNO = ipart.SUBALTERNO")

            stb.AppendLine("    LEFT JOIN Appezzamento a ON a.PIVA = ipart.PIVA")
            stb.AppendLine("        AND a.SA_COD = ipart.SA_COD")
            stb.AppendLine("        AND a.APPEZZA = ipart.appezza")

        End If


        'visibilità delle tabelle in base a cosa mostrare.
        Dim aliasJoinPivaSaCod As String = "ipart"
        Dim aliasTabellaFiltriWhere As String = "a"
        Dim tipoJoinProgettiImpianti As String
        Dim tipoJoinIstat As String = "INNER"

        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            tipoJoinProgettiImpianti = "LEFT"
            aliasTabellaFiltriWhere = "ipp"
        End If


        If TipoQuery = enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Then
            tipoJoinProgettiImpianti = "INNER"
            aliasJoinPivaSaCod = "a"
            tipoJoinIstat = "LEFT"
        End If

        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            tipoJoinProgettiImpianti = "LEFT"
            aliasJoinPivaSaCod = "a"
        End If

        If LeggiDettagliKendoGrid Then

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN (SELECT piva, sa_Cod, appezza, MIN(cod_indirizzo) AS cod_indirizzo FROM AppezzamentixIndirizzi GROUP BY piva, sa_Cod, appezza) aii")
                stb.AppendLine("		ON  a.PIVA = aii.PIVA")
                stb.AppendLine("		AND a.SA_COD = aii.sa_cod")
                stb.AppendLine("		AND a.APPEZZA = aii.appezza")

                stb.AppendLine("")
                stb.AppendLine("    LEFT JOIN Indirizzi indApp ON indApp.cod_indirizzo = aii.cod_indirizzo")

                stb.AppendLine("")
                stb.AppendLine("    LEFT JOIN ISTAT istatAPP ON istatAPP.PROV = indApp.pro_cod_istat")
                stb.AppendLine("        AND istatAPP.COM = indApp.com_cod_istat")
            End If

            stb.AppendLine("")
            stb.AppendLine("    " & tipoJoinProgettiImpianti & " JOIN Imprese_Progetti dist ON a.PIVA = dist.PIVA")
            stb.AppendLine("		AND a.SA_COD = dist.SA_COD")
            stb.AppendLine("		AND a.APPEZZA = dist.appezza")
            stb.AppendLine("		AND dist.Validita_Inizio <= " & Agro_SQL_SaveDate(DataA))
            stb.AppendLine("		AND dist.Validita_Fine >= " & Agro_SQL_SaveDate(DataDa))

            stb.AppendLine("")
            stb.AppendLine("    " & tipoJoinProgettiImpianti & " JOIN reg_impianti reg ON reg.PIVA = dist.PIVA")
            stb.AppendLine("		AND reg.SA_COD = dist.SA_COD")
            stb.AppendLine("		AND reg.APPEZZA = dist.appezza")
            stb.AppendLine("		AND reg.id_reg = dist.id_reg")

            'Codifiche AGEA dedotte per l'impianto.
            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                stb.AppendLine("")
                stb.AppendLine("    --lettura codifiche agea per impianto ")
                stb.AppendLine("    LEFT JOIN(")

                stb.Append(stbAgea2015)

                stb.AppendLine("	) A2015 ON reg.PIVA = A2015.PIVA")
                stb.AppendLine("		AND reg.Sa_Cod = A2015.Sa_Cod")
                stb.AppendLine("		AND reg.Appezza = A2015.APPEZZA")
                stb.AppendLine("		AND reg.id_reg = A2015.ID_REG")

                stb.AppendLine("")
                stb.AppendLine("	--fine lettura codifiche agea per impianto")
            End If

            If TipoQuery = enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Then

                'stb.AppendLine("")
                'stb.AppendLine("    INNER JOIN (SELECT DISTINCT piva, sa_cod FROM ImpreseXParticelle) ipp ON ipp.piva = " & aliasJoinPivaSaCod & ".piva")
                'stb.AppendLine("        AND ipp.sa_Cod = " & aliasJoinPivaSaCod & ".sa_cod")

            Else

                stb.AppendLine("")
                stb.AppendLine("	INNER JOIN ImpreseXParticelle ipp")

                If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

                    stb.AppendLine("        ON ipp.PROV = cc.PROV")
                    stb.AppendLine("        AND ipp.com = cc.COM")
                    stb.AppendLine("        AND ipp.SEZIONE = cc.SEZIONE")
                    stb.AppendLine("        AND ipp.FOGLIO = cc.FOGLIO")
                    stb.AppendLine("        AND ipp.NUMERO = cc.NUMERO")
                    stb.AppendLine("        AND ipp.SUBALTERNO = cc.SUBALTERNO")
                Else

                    stb.AppendLine("        ON ipp.PROV = iPart.PROV")
                    stb.AppendLine("		AND ipp.com = iPart.COM")
                    stb.AppendLine("		AND ipp.SEZIONE =iPart.SEZIONE")
                    stb.AppendLine("		AND ipp.FOGLIO = iPart.FOGLIO")
                    stb.AppendLine("		AND ipp.NUMERO = iPart.NUMERO")
                    stb.AppendLine("		AND ipp.SUBALTERNO = iPart.SUBALTERNO")

                End If
                stb.AppendLine("		AND ipp.Validita_Inizio <= " & Agro_SQL_SaveDate(DataA))
                stb.AppendLine("		AND ipp.Validita_Fine >= " & Agro_SQL_SaveDate(DataDa))

                If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                    stb.AppendLine("		AND ipp.piva = " & aliasJoinPivaSaCod & ".piva")
                    stb.AppendLine("        AND ipp.sa_Cod = " & aliasJoinPivaSaCod & ".sa_cod")
                End If

            End If



        End If

        If idTestataTemp <> 0 Then

            If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                stb.AppendLine("")
                stb.AppendLine("    INNER JOIN (SELECT DISTINCT IDTestataTemp, piva, sa_Cod FROM __tmp_FiltroImpianti) tt ON tt.piva = ipp.piva")
                stb.AppendLine("        AND tt.sa_cod = ipp.SA_COD")
                stb.AppendLine("        AND tt.idTestataTemp = " & idTestataTemp)
            Else
                stb.AppendLine("")
                stb.AppendLine("    INNER JOIN (SELECT DISTINCT IDTestataTemp, piva, sa_Cod, appezza, id_reg FROM __tmp_FiltroImpianti) tt ON tt.piva = reg.piva")
                stb.AppendLine("        AND tt.sa_cod = reg.SA_COD")
                stb.AppendLine("        AND tt.appezza = reg.APPEZZA")
                stb.AppendLine("        AND tt.id_reg = reg.ID_REG")
                stb.AppendLine("        AND tt.idTestataTemp = " & idTestataTemp)
            End If


        End If

        If LeggiDettagliKendoGrid Then
            stb.AppendLine("")
            stb.AppendLine("    " & tipoJoinProgettiImpianti & " JOIN Centri_Aziendali sa")
            If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
                stb.AppendLine("        ON sa.PIVA = " & aliasTabellaParticelle & ".piva")
                stb.AppendLine("        AND sa.sa_cod = " & aliasTabellaParticelle & ".sa_cod")
            Else
                stb.AppendLine("        ON sa.PIVA = " & aliasJoinPivaSaCod & ".piva")
                stb.AppendLine("        AND sa.sa_cod = " & aliasJoinPivaSaCod & ".sa_cod")
            End If

            stb.AppendLine("")
            stb.AppendLine("    " & tipoJoinProgettiImpianti & " JOIN imprese i ON  i.piva = sa.piva")
        Else
            stb.AppendLine("")
            stb.AppendLine("    INNER JOIN Centri_Aziendali sa")
            stb.AppendLine("        ON sa.PIVA = ipart.piva")
            stb.AppendLine("        AND sa.sa_cod = ipart.sa_cod")
            stb.AppendLine("")
            stb.AppendLine("    INNER JOIN imprese i")
            stb.AppendLine("        ON i.piva = sa.piva")
        End If

        stb.AppendLine("")
        stb.AppendLine("    --GIS su catasto presente (tipo query: " & TipoQuery & ")")

        Dim aliasXJoinGisSuCatasto As String = "iPart"
        If TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            aliasXJoinGisSuCatasto = "ipp"
        End If

        stb.AppendLine("")
        stb.AppendLine("	LEFT JOIN GIS_Entita ePart ON ePart.PROV = " & aliasXJoinGisSuCatasto & ".PROV")
        stb.AppendLine("		AND ePart.com = " & aliasXJoinGisSuCatasto & ".COM")
        stb.AppendLine("		AND ePart.SEZIONE = " & aliasXJoinGisSuCatasto & ".SEZIONE")
        stb.AppendLine("		AND ePart.FOGLIO = " & aliasXJoinGisSuCatasto & ".FOGLIO")
        stb.AppendLine("		AND ePart.NUMERO = " & aliasXJoinGisSuCatasto & ".NUMERO")
        stb.AppendLine("		AND ePart.SUBALTERNO = " & aliasXJoinGisSuCatasto & ".SUBALTERNO")
        stb.AppendLine("		AND ePart.tipoEntita_cod = 3")

        stb.AppendLine("")
        stb.AppendLine("	LEFT JOIN imprese_codici cuaa ON cuaa.piva = i.piva")
        stb.AppendLine("		AND cuaa.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

        stb.AppendLine("")
        stb.AppendLine("	--Gerarchia imprese")
        stb.AppendLine("	LEFT JOIN (SELECT MIN(padre) AS Padre, figlio FROM GerarchiaImprese GROUP BY figlio) gi ON gi.figlio = i.piva")

        stb.AppendLine("")
        stb.AppendLine("    LEFT JOIN imprese ii ON ii.piva = gi.padre")

        If LeggiDettagliKendoGrid Then
            stb.AppendLine("")
            stb.AppendLine("    " & tipoJoinIstat & " JOIN istat")
            stb.AppendLine("        ON istat.prov = " & aliasTabellaParticelle & ".prov")
            stb.AppendLine("        AND istat.com = " & aliasTabellaParticelle & ".com")
        Else
            stb.AppendLine("")
            stb.AppendLine("    INNER JOIN istat")
            stb.AppendLine("        ON istat.prov = ipart.prov")
            stb.AppendLine("        AND istat.com = ipart.com")
        End If

        LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "ac", enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)

        If LeggiDettagliKendoGrid Then

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "NAppBio", enum_CodiciAnagrafe.Codice_Appezza_Biologico)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "MetodoProd", enum_CodiciAnagrafe.MetodoDiProduzione)

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "prec1", enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieGruppoVegDataCultivar(stb, "prec1", "prec1_cul", "prec1_veg", "prec1_gru")

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "prec2", enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieGruppoVegDataCultivar(stb, "prec2", "prec2_cul", "prec2_veg", "prec2_gru")

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "prec3", enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieGruppoVegDataCultivar(stb, "prec3", "prec3_cul", "prec3_veg", "prec3_gru")

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb, "prec4", enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieGruppoVegDataCultivar(stb, "prec4", "prec4_cul", "prec4_veg", "prec4_gru")

                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(enum_CodiciAnagrafe.Codice_Impianto, "rc", stb)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(enum_CodiciAnagrafe.Impianto_Interbina, "rcInterbina", stb)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(enum_CodiciAnagrafe.Impianto_Germinabilita, "rcGerminabilita", stb)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(enum_CodiciAnagrafe.Impianto_SuFila_Maschio, "rcSuFila", stb)
                stb.AppendLine("")
                LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(enum_CodiciAnagrafe.Impianto_TraFila_Maschio, "rcTraFila", stb)

                'Salvatore Zammataro: 06/07/2022
                stb.AppendLine("")
                stb.AppendLine(" 	LEFT JOIN Reg_Impianti_codici codAnagrafe ON codAnagrafe.piva = reg.piva")
                stb.AppendLine("		AND codAnagrafe.sa_cod = reg.sa_Cod")
                stb.AppendLine("		AND codAnagrafe.appezza= reg.appezza")
                stb.AppendLine("		AND codAnagrafe.id_reg= reg.id_reg")
                stb.AppendLine("		AND codAnagrafe.id_cod >= 3000 AND codAnagrafe.id_Cod < 4000")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Codici_Anagrafe codAnagrafeDes ON codAnagrafe.id_cod = codAnagrafeDes.codice")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Reg_Impianti_codici capPrivato ON capPrivato.piva = dist.piva")
                stb.AppendLine("		AND capPrivato.sa_cod = dist.sa_Cod")
                stb.AppendLine("		AND capPrivato.appezza= dist.appezza")
                stb.AppendLine("		AND capPrivato.id_reg= dist.id_reg")
                stb.AppendLine("		AND capPrivato.progetto_cod = dist.progetto_cod")
                stb.AppendLine("		AND capPrivato.id_cod = 1093")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN CAC_Codifica_InfoAggiuntive cacCapPrivato ON capPrivato.Val_Cod = cacCapPrivato.InfoAgg_Cod")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Reg_Impianti_codici dettaglioSpeciePersonalizzato ON dettaglioSpeciePersonalizzato.piva = reg.piva")
                stb.AppendLine("		AND dettaglioSpeciePersonalizzato.sa_cod = reg.sa_Cod")
                stb.AppendLine("		AND dettaglioSpeciePersonalizzato.appezza= reg.appezza")
                stb.AppendLine("		AND dettaglioSpeciePersonalizzato.id_reg= reg.id_reg")
                stb.AppendLine("		AND dettaglioSpeciePersonalizzato.id_cod = 1108")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN CAC_Codifica_InfoAggiuntive cacDettalioSpeciePersonalizzato")
                stb.AppendLine("		ON dettaglioSpeciePersonalizzato.val_cod = cacDettalioSpeciePersonalizzato.InfoAgg_Cod")
                stb.AppendLine($"		AND cacDettalioSpeciePersonalizzato.Argomento_Cod = {CInt(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato)}")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Regolamenti ON dist.Regolamento_Cod = Regolamenti.Reg_Cod")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN FormeAllevamento ON reg.FORAL_COD = FormeAllevamento.Foral_Cod")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Portinnesti on reg.port_cod = Portinnesti.port_cod")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN ImpiantiIrrigazioni on reg.Imp_Cod = ImpiantiIrrigazioni.Imp_Cod")

                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN Copertura on reg.Cop_Cod = Copertura.Cop_Cod")
                '-------------------------------
            End If

            If TipoQuery <> enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Then
                stb.AppendLine("")
                stb.AppendLine("	LEFT JOIN ImpreseXParticelle_Codici ipc ON ipc.id = ipp.id")
                stb.AppendLine("		AND ipc.id_cod = " & enum_CodiciAnagrafe.CodiceParticella)
            End If

            ' Salvatore Zammataro 14/04/2023: aggiunta estrazione Organismo Referente -------------------------------------------
            stb.AppendLine("")
            stb.AppendLine("	LEFT JOIN Reg_Impianti_codici rcOrganismoReferente")
            stb.AppendLine("		ON rcOrganismoReferente.piva = reg.piva")
            stb.AppendLine("		AND rcOrganismoReferente.sa_cod = reg.sa_Cod")
            stb.AppendLine("		AND rcOrganismoReferente.appezza= reg.appezza")
            stb.AppendLine("		AND rcOrganismoReferente.id_reg= reg.id_reg")
            stb.AppendLine("        AND rcOrganismoReferente.id_cod = 1074")

            stb.AppendLine("")
            stb.AppendLine("	LEFT JOIN Imprese iOrganismoReferente")
            stb.AppendLine("		ON rcOrganismoReferente.val_cod = iOrganismoReferente.PIVA")
            '---------------------------------------------------------------------------------------------------------------------

            LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieCultivar(stb, "reg", "cul", "veg")


            stb.AppendLine("")
            stb.AppendLine("	LEFT JOIN GruppoVarietale grva ON reg.GRVA_Cod_VEG = grva.grva_cod")

            stb.AppendLine("")
            stb.AppendLine("	LEFT JOIN GruppoFinalita grfi ON reg.grfi_cod = grfi.grfi_cod")

            stb.AppendLine("")
            stb.AppendLine("    --GIS su appezzamento presente  ")
            stb.AppendLine("    LEFT JOIN(")
            stb.AppendLine("        SELECT")
            stb.AppendLine("			piva,")
            stb.AppendLine("			sa_Cod,")
            stb.AppendLine("			appezza,")
            stb.AppendLine("			round(MIN(g.Poligono_GeoEntity.STArea()), 6) AS Area,")
            stb.AppendLine("			MIN(g.Poligono_GeoEntity.EnvelopeCenter().STAsText()) AS Baricentro")
            stb.AppendLine("")
            stb.AppendLine("        FROM GIS_Entita e")
            stb.AppendLine("			INNER JOIN GIS_ElementiGrafici g ON e.Entita_Cod = g.Entita_Cod")
            stb.AppendLine("")
            stb.AppendLine("        WHERE e.tipoEntita_Cod = 1")
            stb.AppendLine("")
            stb.AppendLine("		GROUP BY piva, sa_Cod, appezza")
            stb.AppendLine("")
            stb.AppendLine("    ) eApp ON eApp.piva = a.piva")
            stb.AppendLine("		AND eApp.sa_cod = a.sa_Cod")
            stb.AppendLine("		AND eApp.appezza= a.appezza")

            stb.AppendLine("")
            stb.AppendLine("    --GIS su impianto presente")
            stb.AppendLine("	LEFT JOIN(")
            stb.AppendLine("        SELECT DISTINCT piva, sa_Cod, appezza, id_imp")
            stb.AppendLine("        FROM GIS_Entita")
            stb.AppendLine("        WHERE tipoEntita_Cod In (")
            stb.AppendLine("            SELECT TipoEntita_cod")
            stb.AppendLine("			FROM gis_tipoEntita")
            stb.AppendLine("			WHERE LayerElementiGrafici_Cod = 19")
            stb.AppendLine("		)")
            stb.AppendLine("    ) eImp ON eImp.piva = reg.piva")
            stb.AppendLine("		AND eImp.sa_cod = reg.sa_Cod")
            stb.AppendLine("		AND eImp.appezza= reg.appezza")
            stb.AppendLine("		AND eImp.id_imp= reg.id_reg")

        End If

        If TipoQuery <> enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            stb.AppendLine("")
            stb.AppendLine("    LEFT JOIN ParticelleCatastali ON iPart.PROV = ParticelleCatastali.PROV")
            stb.AppendLine("        AND ipart.COM = ParticelleCatastali.COM")
            stb.AppendLine("        AND ipart.SEZIONE = ParticelleCatastali.SEZIONE")
            stb.AppendLine("        AND ipart.FOGLIO = ParticelleCatastali.FOGLIO")
            stb.AppendLine("        AND ipart.NUMERO = ParticelleCatastali.NUMERO")
            stb.AppendLine("        AND ipart.SUBALTERNO = ParticelleCatastali.SUBALTERNO")
        End If

        stb.AppendLine("")
        stb.AppendLine("	LEFT JOIN campi cp ON cp.piva = a.piva")
        stb.AppendLine("		AND cp.Sa_Cod = a.SA_COD")
        stb.AppendLine("		AND cp.Campo_Cod = a.Campo_Cod")

        stb.AppendLine("WHERE (" & aliasTabellaFiltriWhere & ".Validita_Inizio <= " & Agro_SQL_SaveDate(DataA) & ")")
        stb.AppendLine("    AND (" & aliasTabellaFiltriWhere & ".Validita_Fine >= " & Agro_SQL_SaveDate(DataDa) & ")")

        If TipoQuery = enum_ParticelleAppezza_TipoQuery.AppezzamentoSenzaRiparto Or
            TipoQuery = enum_ParticelleAppezza_TipoQuery.ParticelleSenzaRiparto Then
            stb.AppendLine("    AND ipart.piva IS NULL")
        End If


        If gisEntita_Cod <> 0 Then
            stb.AppendLine($"    AND ePart.entita_cod = {gisEntita_Cod}")
        End If

        If piva <> "" Then
            stb.AppendLine($"    AND sa.Piva = '{Agro_SQL_SaveText(Trim(piva))}'")
        End If

        If sa_Cod <> 0 Then
            stb.AppendLine($"    AND sa.Sa_Cod = {Agro_SQL_SaveNum(sa_Cod)}")
        End If

    End Sub



    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_PianteHaSelect(
        ByVal MoltiplicaSup As Boolean, AliasColonna As String, stb As StringBuilder)

        stb.AppendLine(" 	CASE WHEN ISNULL(dist.p_Ha, 0.0) <> 0.0")
        stb.Append("        THEN")
        If Not MoltiplicaSup Then
            stb.Append(" dist.p_ha / reg.Sup_imp")
        Else
            stb.Append(" dist.p_ha")
        End If

        stb.AppendLine("")
        stb.AppendLine("        ELSE CASE WHEN ISNULL(rcInterbina.val_cod, '0') <> '0'")
        stb.AppendLine("            AND ISNULL(rcGerminabilita.val_cod, '0') <> '0'")


        stb.AppendLine("			AND CASE rcInterbina.val_cod When '' THEN '0' ELSE rcInterbina.val_cod END <> '0'")
        stb.AppendLine("			AND CASE rcGerminabilita.val_cod When '' THEN '0' ELSE rcGerminabilita.val_cod END <> '0'")
        stb.AppendLine("			AND CAST(REPLACE(	CASE RcSuFila.val_cod When '' THEN '0' ELSE RcSuFila.val_cod END	, ',', '.') AS FLOAT) <> 0")
        stb.AppendLine("            AND CAST(REPLACE(	CASE RcTraFila.val_cod When '' THEN '0' ELSE RcTraFila.val_cod END	, ',', '.') AS FLOAT) <> 0")
        stb.AppendLine("			AND (ABS(RcTraFila.val_cod - CAST(REPLACE(rcInterbina.val_cod, ',', '.') AS FLOAT))) <> 0")

        stb.AppendLine("")
        stb.AppendLine("			THEN")

        If MoltiplicaSup Then
            stb.AppendLine("                reg.sup_imp *")
            stb.AppendLine("				(")
        End If

        LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_PianteHaSelectCalc(stb)

        If MoltiplicaSup Then
            stb.AppendLine("				)")
        End If

        stb.AppendLine("")
        stb.AppendLine("            ELSE 0")
        stb.AppendLine("        END")
        stb.AppendLine("    END AS " & AliasColonna)

    End Sub

    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_PianteHaSelectCalc(stb As StringBuilder)
        stb.AppendLine("				10000 / (ABS(RcTraFila.val_cod - CAST(REPLACE(rcInterbina.val_cod, ',', '.') AS FLOAT)) * RcSuFila.val_cod) *")
        stb.AppendLine("				(CAST(REPLACE(rcGerminabilita.val_cod, ',', '.') AS FLOAT) / 100)")
        'stb.AppendLine("                 10000 / (ABS(RcTraFila.val_cod - cast(replace(rcInterbina.val_cod, ',', '.') as float)) * RcSuFila.val_cod) * (cast(replace(rcGerminabilita.val_cod, ',', '.') as float) / 100) ")
    End Sub

    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza_JoinCodiciImpianti(id_Cod As enum_CodiciAnagrafe, aliasTabellaCodici As String, stb As StringBuilder)
        stb.AppendLine("    LEFT JOIN Reg_Impianti_codici " & aliasTabellaCodici)
        stb.AppendLine("        ON " & aliasTabellaCodici & ".piva = reg.piva")
        stb.AppendLine("        AND " & aliasTabellaCodici & ".sa_cod = reg.sa_Cod")
        stb.AppendLine("        AND " & aliasTabellaCodici & ".appezza= reg.appezza")
        stb.AppendLine("        AND " & aliasTabellaCodici & ".id_reg= reg.id_reg")
        stb.AppendLine("        AND " & aliasTabellaCodici & ".id_cod = " & id_Cod)
    End Sub

    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieCultivar(stb As StringBuilder, tabPrinc As String, cul As String, veg As String)
        stb.AppendLine("    LEFT JOIN Cultivar " & cul)
        stb.AppendLine("        ON " & tabPrinc & ".cul_cod = " & cul & ".cul_cod")

        stb.AppendLine("")
        stb.AppendLine("    LEFT JOIN SpecieVegetali  " & veg)
        stb.AppendLine("        ON " & veg & ".veg_cod = " & cul & ".veg_cod")

        stb.AppendLine("")
        stb.AppendLine("	LEFT JOIN GruppoVegetale ON " & veg & ".Gru_Cod = GruppoVegetale.Gru_Cod")
    End Sub
    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_SpecieGruppoVegDataCultivar(stb As StringBuilder, tabPrinc As String, cul As String, veg As String, gru As String)
        stb.AppendLine("    LEFT JOIN SpecieVegetali " & veg)
        stb.AppendLine("        ON SUBSTRING(" & tabPrinc & ".val_Cod, 0, charindex('|', " & tabPrinc & ".val_cod, 1)) = " & veg & ".veg_cod")

        stb.AppendLine("    LEFT JOIN GruppoVegetale " & gru)
        stb.AppendLine("        ON SUBSTRING(" & tabPrinc & ".val_Cod, charindex('|', " & tabPrinc & ".val_cod, 1) + 1, len(" & tabPrinc & ".val_cod) - charindex('|'," & tabPrinc & ".val_cod, 1)) = " & gru & ".gru_cod")
    End Sub

    Private Shared Sub LeggiAppezzamenti_Da_ParticelleGIS_GetQuery_JoinCodiciAppezza(stb As StringBuilder, aliasTabella As String, id_cod As enum_CodiciAnagrafe)
        stb.AppendLine("	LEFT JOIN appezzamento_codici " & aliasTabella)
        stb.AppendLine("        ON " & aliasTabella & ".piva = ipart.piva")
        stb.AppendLine("        AND " & aliasTabella & ".sa_cod = ipart.sa_Cod")
        stb.AppendLine("        AND " & aliasTabella & ".appezza= ipart.appezza")
        stb.AppendLine("        AND " & aliasTabella & ".id_cod = " & id_cod)
    End Sub



    '##############################################################################################
    '##############################################################################################
    'Trova tutti gli appezzamenti associati ad una data particella
    '============================================================================
    Public Function LeggiAppezzamenti_Da_Particella(ByVal PROV As String,
                                                    ByVal COM As String,
                                                    ByVal SEZIONE As String,
                                                    ByVal FOGLIO As Int32,
                                                    ByVal NUMERO As Int32,
                                                    ByVal SUBALTERNO As String,
                                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

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
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare query
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod, AppezzamentixParticelle.Validita_Inizio as xValidita_Inizio, AppezzamentixParticelle.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod, AppezzamentixParticelle.Validita_Inizio as xValidita_Inizio, AppezzamentixParticelle.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
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

    Public Function LeggiAppezzamenti_Da_Particella(ByVal Piva As String,
                                                    ByVal Sa_Cod As Long,
                                                    ByVal Appezza As Long,
                                                    ByVal ID_Reg As Long,
                                                    ByVal PROV As String,
                                                    ByVal COM As String,
                                                    ByVal SEZIONE As String,
                                                    ByVal FOGLIO As Int32,
                                                    ByVal NUMERO As Int32,
                                                    ByVal SUBALTERNO As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

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
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT   ")
            StrSQL.AppendLine(" Imprese.rag_soc, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine(" Appezzamento.Appezza, ")
            StrSQL.AppendLine(" Reg_Impianti.Id_Reg, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.Piva, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.SA_COD, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.PROV, ")
            StrSQL.AppendLine(" ISTAT.COMUNI_PROV, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.COM, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.SEZIONE, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.FOGLIO, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.NUMERO, ")
            StrSQL.AppendLine(" AppezzamentixParticelle.SUBALTERNO, ")
            StrSQL.AppendLine(" ISNULL(Campi.Campo_Des, '') As Campo_Des, ")
            StrSQL.AppendLine(" Appezzamento.APP_NOME,  ")
            StrSQL.AppendLine(" Appezzamento.SUP_APP,  ")
            StrSQL.AppendLine(" Reg_Impianti.Validita_Inizio,  ")
            StrSQL.AppendLine(" Reg_Impianti.Validita_Fine,  ")
            StrSQL.AppendLine(" AppezzamentixParticelle.AREA,  ")
            StrSQL.AppendLine(" Cultivar.Cul_Des,  ")
            StrSQL.AppendLine(" SpecieVegetali.Veg_Des, ")
            StrSQL.AppendLine(" dest.descrizione, ")
            StrSQL.AppendLine(" CASE WHEN Reg_Impianti.Cul_Cod = 0  ")
            StrSQL.AppendLine(" THEN CASE WHEN destinazione.id_cod > 0 THEN dest.descrizione ELSE 'Terreno Nudo' END ")
            StrSQL.AppendLine(" ELSE SpecieVegetali.Veg_Des + ' - ' + Cultivar.Cul_Des ")
            StrSQL.AppendLine(" END as Utilizzo, ")
            StrSQL.AppendLine(" CASE WHEN Appezzamento.Validita_Inizio < GETDATE() AND Appezzamento.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo, ")
            StrSQL.AppendLine(" CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
            StrSQL.AppendLine(" FROM AppezzamentixParticelle ")
            StrSQL.AppendLine(" JOIN Appezzamento ON AppezzamentixParticelle.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine(" 	AND AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" 	AND AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")
            StrSQL.AppendLine(" LEFT JOIN Campi ON Appezzamento.Piva = Campi.Piva ")
            StrSQL.AppendLine("                   AND Appezzamento.Sa_Cod = Campi.Sa_Cod ")
            StrSQL.AppendLine("                   AND Appezzamento.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" JOIN Reg_Impianti ON Reg_Impianti.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine(" 				AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" 				AND Reg_Impianti.Appezza = Appezzamento.Appezza ")
            StrSQL.AppendLine(" JOIN ISTAT ON AppezzamentixParticelle.Prov = ISTAT.Prov  ")
            StrSQL.AppendLine(" 		AND AppezzamentixParticelle.Com = ISTAT.Com ")
            StrSQL.AppendLine(" JOIN Imprese ON AppezzamentixParticelle.Piva = imprese.PIVA ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ON AppezzamentixParticelle.Piva = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" 					AND	AppezzamentixParticelle.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici destinazione ON Reg_Impianti.Piva = destinazione.Piva ")
            StrSQL.AppendLine(" 				AND Reg_Impianti.Sa_Cod = destinazione.Sa_Cod ")
            StrSQL.AppendLine(" 				AND Reg_Impianti.Appezza = destinazione.Appezza ")
            StrSQL.AppendLine(" 				AND Reg_Impianti.ID_REG = destinazione.Id_Reg ")
            StrSQL.AppendLine(" 				AND destinazione.id_cod >= 3000 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe dest ON destinazione.id_cod = dest.codice ")
            StrSQL.AppendLine(" LEFT JOIN ZonexParticelle ON AppezzamentixParticelle.PROV = ZonexParticelle.PROV ")
            StrSQL.AppendLine("             AND AppezzamentixParticelle.COM = ZonexParticelle.COM ")
            StrSQL.AppendLine("             AND AppezzamentixParticelle.SEZIONE = ZonexParticelle.SEZIONE ")
            StrSQL.AppendLine("             AND AppezzamentixParticelle.FOGLIO = ZonexParticelle.FOGLIO ")
            StrSQL.AppendLine("             AND AppezzamentixParticelle.NUMERO = ZonexParticelle.NUMERO ")
            StrSQL.AppendLine("             AND AppezzamentixParticelle.SUBALTERNO = ZonexParticelle.SUBALTERNO ")
            StrSQL.AppendLine("             AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If ID_Reg <> 0 Then
                StrSQL.AppendLine(" AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(ID_Reg) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Prov = '" & Agro_SQL_SaveText(PROV) & "'  ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Com = '" & Agro_SQL_SaveText(COM) & "'  ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Foglio = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Numero = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND AppezzamentixParticelle.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
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


    '##############################################################################################
    '##############################################################################################

    'Trova tutti gli appezzamenti associati ad una data particella
    '============================================================================
    Public Function LeggiAppezzamentiImpresa_Da_Particella(ByVal Piva As String,
                                                           ByVal Sa_Cod As Int32,
                                                           ByVal PROV As String,
                                                           ByVal COM As String,
                                                           ByVal SEZIONE As String,
                                                           ByVal FOGLIO As Int32,
                                                           ByVal NUMERO As Int32,
                                                           ByVal SUBALTERNO As String,
                                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella()"

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
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare query
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod, AppezzamentixParticelle.Validita_Inizio as xValidita_Inizio, AppezzamentixParticelle.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   AppezzamentixParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT AppezzamentixParticelle.* , ParticelleCatastali.*  , Appezzamento.Campo_Cod, AppezzamentixParticelle.Validita_Inizio as xValidita_Inizio, AppezzamentixParticelle.Validita_Fine as xValidita_Fine ")
                    StrSQL.Append(" FROM  AppezzamentixParticelle , ParticelleCatastali , Appezzamento ")
                    StrSQL.Append(" WHERE AppezzamentixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Piva = Appezzamento.Piva ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   AppezzamentixParticelle.Appezza = Appezzamento.Appezza ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   AppezzamentixParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   AppezzamentixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AppezzamentixParticelle.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY AppezzamentixParticelle.Appezza ASC")
                    End If




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
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

    Public Function Leggi_AppezzamentiXParticelleXZona(ByVal PivaSuperUser As String,
                                                       ByVal ZonaCod As Integer,
                                                       ByVal InizioValidita As DateTime,
                                                       ByVal FineValidita As DateTime,
                                                       ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.Leggi_AppezzamentiXParticelleXZona()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0


            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("     zp.Zona_Cod, ")
            StrSQL.AppendLine("     ap.piva, ")
            StrSQL.AppendLine("     ap.SA_COD, ")
            StrSQL.AppendLine("     ap.APPEZZA, ")
            StrSQL.AppendLine("     b.ID_REG, ")
            StrSQL.AppendLine("     b.CUL_COD, ")
            StrSQL.AppendLine("     c.Cul_Des, ")
            StrSQL.AppendLine("     b.Sup_Imp, ")
            StrSQL.AppendLine("     b.GRFI_COD, ")
            StrSQL.AppendLine("     d.Veg_Cod, ")
            StrSQL.AppendLine("     d.Veg_Des, ")
            StrSQL.AppendLine("     a.APP_NOME, ")
            StrSQL.AppendLine("     zp.Piva_SuperUser, ")
            StrSQL.AppendLine("     zp.PROV, ")
            StrSQL.AppendLine("     zp.COM, ")
            StrSQL.AppendLine("     zp.SEZIONE, ")
            StrSQL.AppendLine("     zp.FOGLIO, ")
            StrSQL.AppendLine("     zp.NUMERO, ")
            StrSQL.AppendLine("     zp.SUBALTERNO, ")
            StrSQL.AppendLine("     b.Validita_Inizio, ")
            StrSQL.AppendLine("     b.Validita_Fine ")
            StrSQL.AppendLine(" From AppezzamentiXParticelle ap inner Join ZonexParticelle zp ")
            StrSQL.AppendLine(" On (ap.PROV = zp.PROV And ap.COM=zp.COM And ap.SEZIONE=zp.SEZIONE And ap.FOGLIO=zp.FOGLIO And ap.NUMERO=zp.NUMERO And ap.SUBALTERNO=zp.SUBALTERNO) ")
            StrSQL.AppendLine(" inner Join Appezzamento a on (ap.piva = a.PIVA And ap.SA_COD=a.SA_COD And ap.APPEZZA=a.APPEZZA) ")
            StrSQL.AppendLine(" Left Join Reg_Impianti b on (ap.piva = b.PIVA And ap.SA_COD=b.SA_COD And ap.APPEZZA=b.APPEZZA) ")
            StrSQL.AppendLine(" Left Join(Cultivar c left join SpecieVegetali d On (c.Veg_Cod=d.Veg_Cod)) on ")
            StrSQL.AppendLine(" (b.CUL_COD=c.Cul_Cod) ")
            StrSQL.AppendLine(" where 1=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" and zp.Piva_SuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If
            If ZonaCod <> 0 Then
                StrSQL.AppendLine(" and zp.Zona_Cod =" & Agro_SQL_SaveNum(ZonaCod) & " ")
            End If

            'compreso nell'intervallo 
            If InizioValidita <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" and b.Validita_Inizio <=" & Agro_SQL_SaveDate(InizioValidita) & " ")
            End If
            If FineValidita <> AGRODATAFINE Then
                StrSQL.AppendLine(" and b.Validita_Fine >=" & Agro_SQL_SaveDate(FineValidita) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY b.Cul_Cod,ap.piva, ap.SA_COD, ap.APPEZZA, b.ID_REG,b.Validita_Inizio")
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

    Public Function Leggi_TotaleSuperficiXSpecie_AppezzamentiXParticelleXZona(ByVal PivaSuperUser As String,
                                                      ByVal ZonaCod As Integer,
                                                      ByVal InizioValidita As DateTime,
                                                      ByVal FineValidita As DateTime,
                                                      ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.Leggi_TotaleSuperficiXColtura_AppezzamentiXParticelleXZona()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0


            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("     zp.Zona_Cod, ")
            'StrSQL.AppendLine("     b.CUL_COD, ")
            'StrSQL.AppendLine("     c.Cul_Des, ")
            StrSQL.AppendLine("     d.Veg_Cod, ")
            StrSQL.AppendLine("     d.Veg_Des, ")
            StrSQL.AppendLine("     sum(ap.area) as Sup_Irrigua, ")
            StrSQL.AppendLine("     sum(b.Sup_Imp) as Sup_Imp ")
            StrSQL.AppendLine(" From DomandaIrrigua_Testata dih inner join DomandaIrrigua_Dettaglio did ")
            StrSQL.AppendLine(" on (dih.Id=did.Id_testata) inner join AppezzamentiXParticelle ap ")
            StrSQL.AppendLine(" On (did.piva=ap.PIVA And did.sa_cod=ap.SA_COD And did.appezza=ap.APPEZZA) ")
            StrSQL.AppendLine(" inner Join ZonexParticelle zp ")
            StrSQL.AppendLine(" On (ap.PROV = zp.PROV And ap.COM=zp.COM And ap.SEZIONE=zp.SEZIONE And ap.FOGLIO=zp.FOGLIO And ap.NUMERO=zp.NUMERO And ap.SUBALTERNO=zp.SUBALTERNO) ")
            StrSQL.AppendLine(" inner Join Appezzamento a on (ap.piva = a.PIVA And ap.SA_COD=a.SA_COD And ap.APPEZZA=a.APPEZZA) ")
            StrSQL.AppendLine(" Left Join Reg_Impianti b on (ap.piva = b.PIVA And ap.SA_COD=b.SA_COD And ap.APPEZZA=b.APPEZZA) ")
            StrSQL.AppendLine(" Left Join(Cultivar c left join SpecieVegetali d On (c.Veg_Cod=d.Veg_Cod)) on ")
            StrSQL.AppendLine(" (b.CUL_COD=c.Cul_Cod) ")
            StrSQL.AppendLine(" where 1=1 ")
            StrSQL.AppendLine("     and did.Selezionato=1 ")
            StrSQL.AppendLine("     and zp.Zona_Cod>0 ")
            StrSQL.AppendLine("     and b.cul_cod >0 ")
            StrSQL.AppendLine("     and ((dih.Validita_Inizio <= " & Agro_SQL_SaveDate(FineValidita) & " and dih.Validita_Fine >=  " & Agro_SQL_SaveDate(InizioValidita) & ")  or ")
            StrSQL.AppendLine("          (dih.Validita_Inizio >= " & Agro_SQL_SaveDate(InizioValidita) & " and dih.Validita_Fine >=  " & Agro_SQL_SaveDate(InizioValidita) & " )) ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" and zp.Piva_SuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If
            If ZonaCod <> 0 Then
                StrSQL.AppendLine(" and zp.Zona_Cod =" & Agro_SQL_SaveNum(ZonaCod) & " ")
            End If



            ''compreso nell'intervallo 
            'If InizioValidita <> AGRODATAINIZIO Then
            '    StrSQL.AppendLine(" and b.Validita_Inizio <=" & Agro_SQL_SaveDate(InizioValidita) & " ")
            'End If
            'If FineValidita <> AGRODATAFINE Then
            '    StrSQL.AppendLine(" and b.Validita_Fine >=" & Agro_SQL_SaveDate(FineValidita) & " ")
            'End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'StrSQL.Append(" GROUP BY zp.Zona_Cod,b.CUL_COD,c.Cul_Des,d.Veg_Cod,d.Veg_Des ")
            StrSQL.Append(" GROUP BY zp.Zona_Cod,d.Veg_Cod,d.Veg_Des ")

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



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class AppezzaxParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'Versione che utilizza AgronicaCoreParametri
    Public Function Scrivi(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal Area As Decimal,
                            ByVal SAU_Convenz_Ettari As Decimal,
                            ByVal SAU_Convenz_Are As Int32,
                            ByVal SAU_Convenz_Centiare As Int32,
                            ByVal SAU_Convers_Ettari As Decimal,
                            ByVal SAU_Convers_Are As Int32,
                            ByVal SAU_Convers_Centiare As Int32,
                            ByVal SAU_Bio_Ettari As Decimal,
                            ByVal SAU_Bio_Are As Int32,
                            ByVal SAU_Bio_Centiare As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO AppezzamentixParticelle(       ")
            StrSQL.Append("                    PIVA, Sa_Cod, Appezza, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO,  AREA,     ")
            StrSQL.Append("                    SAU_Convenz_Ettari, SAU_Convenz_Are, SAU_Convenz_Centiare, ")
            StrSQL.Append("                    SAU_Convers_Ettari, SAU_Convers_Are, SAU_Convers_Centiare, ")
            StrSQL.Append("                    SAU_Bio_Ettari,     SAU_Bio_Are,     SAU_Bio_Centiare, ")
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
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Area) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
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

    Public Function Modifica(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal Area As Decimal,
                            ByVal SAU_Convenz_Ettari As Decimal,
                            ByVal SAU_Convenz_Are As Int32,
                            ByVal SAU_Convenz_Centiare As Int32,
                            ByVal SAU_Convers_Ettari As Decimal,
                            ByVal SAU_Convers_Are As Int32,
                            ByVal SAU_Convers_Centiare As Int32,
                            ByVal SAU_Bio_Ettari As Decimal,
                            ByVal SAU_Bio_Are As Int32,
                            ByVal SAU_Bio_Centiare As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Modifica()"

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
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
            StrSQL.Append("        AREA  = " & Agro_SQL_SaveNum(Area) & " ")
            StrSQL.Append("       ,SAU_Convenz_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.Append("       ,SAU_Convenz_Are      =  " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.Append("       ,SAU_Convenz_Centiare =  " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.Append("       ,SAU_Convers_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.Append("       ,SAU_Convers_Are      =  " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.Append("       ,SAU_Convers_Centiare =  " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.Append("       ,SAU_Bio_Ettari       =  " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.Append("       ,SAU_Bio_Are          =  " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.Append("       ,SAU_Bio_Centiare     =  " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
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

    '######################################################################################################
    Public Function ModificaChiave(
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal PROV_Origine As String,
                          ByVal COM_Origine As String,
                          ByVal SEZIONE_Origine As String,
                          ByVal FOGLIO_Origine As Int32,
                          ByVal NUMERO_Origine As Int32,
                          ByVal SUBALTERNO_Origine As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.ModificaChiave()"

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

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE AppezzamentiXParticelle ")
            StrSQL.Append(" SET  ")
            StrSQL.Append(" PROV = '" & Agro_SQL_SaveText(PROV) & "', ")
            StrSQL.Append(" COM = '" & Agro_SQL_SaveText(COM) & "', ")
            StrSQL.Append(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "', ")
            StrSQL.Append(" FOGLIO =" & FOGLIO & ", ")
            StrSQL.Append(" NUMERO =" & NUMERO & ", ")
            StrSQL.Append(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "',")
            StrSQL.Append(" Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ",")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")
            StrSQL.Append(" WHERE PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "' ")
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "' ")
            StrSQL.Append(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "' ")
            StrSQL.Append(" AND FOGLIO =" & FOGLIO_Origine & " ")
            StrSQL.Append(" AND NUMERO =" & NUMERO_Origine & " ")
            StrSQL.Append(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "'")


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

    Public Function ModificaChiave2(
                          ByVal Piva As String,
                          ByVal sa_cod As Integer,
                          ByVal appezza As Integer,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Piva_Origine As String,
                          ByVal sa_cod_Origine As Integer,
                          ByVal appezza_Origine As Integer,
                          ByVal PROV_Origine As String,
                          ByVal COM_Origine As String,
                          ByVal SEZIONE_Origine As String,
                          ByVal FOGLIO_Origine As Int32,
                          ByVal NUMERO_Origine As Int32,
                          ByVal SUBALTERNO_Origine As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.ModificaChiave()"

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

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE AppezzamentiXParticelle ")
            StrSQL.Append(" SET  ")
            StrSQL.Append(" PIVA = '" & Agro_SQL_SaveText(Piva) & "', ")
            StrSQL.Append(" SA_COD = " & sa_cod & ", ")
            StrSQL.Append(" APPEZZA = " & appezza & ", ")
            StrSQL.Append(" PROV = '" & Agro_SQL_SaveText(PROV) & "', ")
            StrSQL.Append(" COM = '" & Agro_SQL_SaveText(COM) & "', ")
            StrSQL.Append(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "', ")
            StrSQL.Append(" FOGLIO =" & FOGLIO & ", ")
            StrSQL.Append(" NUMERO =" & NUMERO & ", ")
            StrSQL.Append(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "',")
            StrSQL.Append(" Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ",")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")
            StrSQL.Append(" WHERE PIVA = '" & Agro_SQL_SaveText(Piva_Origine) & "' ")
            StrSQL.Append(" AND SA_COD = " & sa_cod_Origine & " ")
            StrSQL.Append(" AND APPEZZA = " & appezza_Origine & " ")
            StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "' ")
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "' ")
            StrSQL.Append(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "' ")
            StrSQL.Append(" AND FOGLIO =" & FOGLIO_Origine & " ")
            StrSQL.Append(" AND NUMERO =" & NUMERO_Origine & " ")
            StrSQL.Append(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "'")


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
    Public Function Cancella(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Cancella()"

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

            Dim CancellaAppezzamento As Boolean = False
            Dim CancellaParticella As Boolean = False

            If Sa_Cod <> 0 And Appezza <> 0 Then
                CancellaAppezzamento = True
            End If

            If PROV <> "" And COM <> "" And FOGLIO <> 0 And NUMERO <> 0 Then
                CancellaParticella = True
            End If

            If CancellaAppezzamento = False And CancellaParticella = False Then
                Throw New Exception("Chiamata funzione senza specificare ne appezzamento ne Dato Catastale")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE AppezzamentixParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("       Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("      ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     AppezzamentixParticelle ")
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


    'Public Function AggiornaValiditaInizio( _
    '                                    ByVal Piva As String, _
    '                                    ByVal Sa_Cod As integer, _
    '                                    ByVal Id_Campo As Integer, _
    '                                    ByVal Appezza As integer, _
    '                                    ByVal PROV As String, _
    '                                    ByVal COM As String, _
    '                                    ByVal SEZIONE As String, _
    '                                    ByVal FOGLIO As Int32, _
    '                                    ByVal NUMERO As Int32, _
    '                                    ByVal SUBALTERNO As String, _
    '                                    ByVal UserName_Modifica As String, _
    '                                    ByVal Validita_Inizio As Date, _
    '                                    ByRef objConnessione As DbConnection, _
    '                                    ByRef objTransazione As DbTransaction, _
    '                                    ByVal StringaConnessione As String, _
    '                                    ByVal DirectoryLOG As String, _
    '                                    ByVal FileLOG As String, _
    '                                    ByVal IdentificatoreUtente As String _
    '                                    ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaInizio()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0           =>  aggiorna tutte le particelle dell'impresa
    '    '   Appezza = 0          =>  aggiorna tutte le particelle del centro aziendale
    '    '   PROV = ""
    '    '   COM = ""
    '    '   SEZIONE = ""
    '    '   FOGLIO = 0
    '    '   NUMERO = 0
    '    '   SUBALTERNO = ""
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
    '        StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
    '        StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If
    '        If Id_Campo <> 0 Then
    '            StrSQL.Append(" AND  Appezza IN ( Select Appezzamento.Appezza From Appezzamento ")
    '            StrSQL.Append(" Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo))
    '            StrSQL.Append(" And   Piva      = '" & Trim(Piva) & "' ")
    '            StrSQL.Append(" And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
    '        End If

    '        If Appezza <> 0 Then
    '            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
    '        End If

    '        If PROV <> "" Then
    '            StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
    '        End If

    '        If COM <> "" Then
    '            StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
    '        End If

    '        If SEZIONE <> "" Then
    '            StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
    '        End If

    '        If FOGLIO <> 0 Then
    '            StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
    '        End If

    '        If NUMERO <> 0 Then
    '            StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
    '        End If

    '        If SUBALTERNO <> "" Then
    '            StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
    '        End If

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        StrSQL = Nothing

    '    End Try

    '    Return xRisp
    'End Function

    'Versione che utilizza AgronicaCoreParametri

    Public Function AggiornaValiditaInizio(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Validita_Inizio As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaInizio()"

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
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp
    End Function

    Public Function AggiornaValiditaInizioForzata(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Campo As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Int32,
                                    ByVal NUMERO As Int32,
                                    ByVal SUBALTERNO As String,
                                        ByVal Validita_Inizio As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaInizio()"

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
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp
    End Function

    '##############################################################################################
    'Public Function AggiornaValiditaFine( _
    '                                    ByVal Piva As String, _
    '                                    ByVal Sa_Cod As Int32, _
    '                                    ByVal Id_Campo As Integer, _
    '                                    ByVal Appezza As Int32, _
    '                                    ByVal PROV As String, _
    '                                    ByVal COM As String, _
    '                                    ByVal SEZIONE As String, _
    '                                    ByVal FOGLIO As Int32, _
    '                                    ByVal NUMERO As Int32, _
    '                                    ByVal SUBALTERNO As String, _
    '                                    ByVal UserName_Modifica As String, _
    '                                    ByVal Validita_Fine As Date, _
    '                                    ByRef objConnessione As DbConnection, _
    '                                    ByRef objTransazione As DbTransaction, _
    '                                    ByVal StringaConnessione As String, _
    '                                    ByVal DirectoryLOG As String, _
    '                                    ByVal FileLOG As String, _
    '                                    ByVal IdentificatoreUtente As String _
    '                                    ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaValiditaFine()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0           =>  aggiorna tutte le particelle dell'impresa
    '    '   Appezza = 0          =>  aggiorna tutte le particelle del centro aziendale
    '    '   PROV = ""
    '    '   COM = ""
    '    '   SEZIONE = ""
    '    '   FOGLIO = 0
    '    '   NUMERO = 0
    '    '   SUBALTERNO = ""
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
    '        StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
    '        StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If
    '        If Id_Campo <> 0 Then
    '            StrSQL.Append(" AND  Appezza IN ( Select Appezzamento.Appezza From Appezzamento ")
    '            StrSQL.Append(" Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo))
    '            StrSQL.Append(" And   Piva      = '" & Trim(Piva) & "' ")
    '            StrSQL.Append(" And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
    '        End If

    '        If Appezza <> 0 Then
    '            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
    '        End If

    '        If PROV <> "" Then
    '            StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
    '        End If

    '        If COM <> "" Then
    '            StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
    '        End If

    '        If SEZIONE <> "" Then
    '            StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
    '        End If

    '        If FOGLIO <> 0 Then
    '            StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
    '        End If

    '        If NUMERO <> 0 Then
    '            StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
    '        End If

    '        If SUBALTERNO <> "" Then
    '            StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
    '        End If

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        StrSQL = Nothing

    '    End Try

    '    Return xRisp

    'End Function


    'Versione che utilizza AgronicaCoreParametri
    Public Function AggiornaValiditaFine(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Int32,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
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
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
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

    Public Function AggiornaValiditaFineForzata(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Int32,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
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
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

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

    Public Sub Cancella_Associazione(ByVal Piva As String,
                                    ByVal Sa_Cod As Long,
                                    ByVal Appezza As Long,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Long,
                                    ByVal NUMERO As Long,
                                    ByVal SUBALTERNO As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Cancella_Associazione()"

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
            '######################################
            '### Cancellazione Fisica Rinviata  ###
            '######################################

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE   AppezzamentixParticelle " &
                   " SET " &
                   "          Validita_Fine = " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " &
                   "         ,Username_Modifica = '" & objParametri.UsernameOperazione & "' " &
                   "         ,Inviato = -1 " &
                   " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                   " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                   " AND      Appezza     =  " & Agro_SQL_SaveNum(Appezza) & "  " &
                   " AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' " &
                   " AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' " &
                   " AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  " &
                   " AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  " &
                   " AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  " &
                   " AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '######################################
            '### Cancellazione Fisica Immediata ###
            '######################################
            '


            StrSQL.Length = 0
            StrSQL.Append(" DELETE " &
                   " FROM     AppezzamentixParticelle " &
                   " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                   " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                   " AND      Appezza     =  " & Agro_SQL_SaveNum(Appezza) & "  " &
                   " AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' " &
                   " AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' " &
                   " AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  " &
                   " AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  " &
                   " AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  " &
                   " AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")


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

    End Sub



    Public Function Aggiungi_Aggiorna(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Appezza As Int32,
                        ByVal PROV As String,
                        ByVal COM As String,
                        ByVal SEZIONE As String,
                        ByVal FOGLIO As Int32,
                        ByVal NUMERO As Int32,
                        ByVal SUBALTERNO As String,
                        ByVal Area As Decimal,
                        ByVal SAU_Convenz_Ettari As Decimal,
                        ByVal SAU_Convenz_Are As Int32,
                        ByVal SAU_Convenz_Centiare As Int32,
                        ByVal SAU_Convers_Ettari As Decimal,
                        ByVal SAU_Convers_Are As Int32,
                        ByVal SAU_Convers_Centiare As Int32,
                        ByVal SAU_Bio_Ettari As Decimal,
                        ByVal SAU_Bio_Are As Int32,
                        ByVal SAU_Bio_Centiare As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            , Optional ByVal Data_creazione As Date = #2/1/1900# _
            , Optional ByVal Data_modifica As Date = #2/1/1900# _
            , Optional ByVal username_creazione As String = "" _
            , Optional ByVal username_modifica As String = ""
) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.Aggiungi_Aggiorna()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            Dim AppezzaxParticelle_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim dtPC As DataTable = AppezzaxParticelle_R.AppezzamentixParticelle_Leggi(Piva, Sa_Cod, Appezza, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, True)

            If dtPC.Rows.Count = 1 Then

                Modifica(Piva, Sa_Cod, Appezza,
                                 PROV, COM, SEZIONE, FOGLIO,
                                 NUMERO, SUBALTERNO, Area,
                                 SAU_Convenz_Ettari,
                                 SAU_Convenz_Are,
                                 SAU_Convenz_Centiare,
                                 SAU_Convers_Ettari,
                                 SAU_Convers_Are,
                                 SAU_Convers_Centiare,
                                 SAU_Bio_Ettari,
                                  SAU_Bio_Are,
                                 SAU_Bio_Centiare,
                                 Validita_Inizio,
                                 Validita_Fine,
                                 "",
                               objParametri)


            ElseIf dtPC.Rows.Count = 0 Then

                Scrivi(Piva, Sa_Cod, Appezza,
                               PROV, COM, SEZIONE, FOGLIO,
                               NUMERO, SUBALTERNO, Area,
                               SAU_Convenz_Ettari,
                               SAU_Convenz_Are,
                               SAU_Convenz_Centiare,
                               SAU_Convers_Ettari,
                               SAU_Convers_Are,
                               SAU_Convers_Centiare,
                               SAU_Bio_Ettari,
                                SAU_Bio_Are,
                               SAU_Bio_Centiare,
                               Validita_Inizio,
                               Validita_Fine,
                             objParametri, Data_creazione, Data_modifica, username_creazione, username_modifica)

            Else
                Throw New Exception("La query deve selezionare al massimo un solo record")
            End If

            xRisp = True

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AggiornaSup(ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Id_Campo As Integer,
                                        ByVal Appezza As Int32,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Int32,
                                        ByVal NUMERO As Int32,
                                        ByVal SUBALTERNO As String,
                                            ByVal Sup As Decimal,
                                            ByVal Metodo_Produzione As enum_MetodoProduzione,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W.AggiornaSup()"

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

            Dim SAU_Convenz_Ettari As Decimal = 0
            Dim SAU_Convenz_Are As Integer = 0
            Dim SAU_Convenz_Centiare As Integer = 0
            Dim SAU_Convers_Ettari As Decimal = 0
            Dim SAU_Convers_Are As Integer = 0
            Dim SAU_Convers_Centiare As Integer = 0
            Dim SAU_Bio_Ettari As Decimal = 0
            Dim SAU_Bio_Are As Integer = 0
            Dim SAU_Bio_Centiare As Integer = 0

            Select Case Metodo_Produzione
                Case enum_MetodoProduzione.Integrato
                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(Sup, SAU_Convenz_Ettari, SAU_Convenz_Are, SAU_Convenz_Centiare)
                Case enum_MetodoProduzione.InConversione
                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(Sup, SAU_Convers_Ettari, SAU_Convers_Are, SAU_Convers_Centiare)
                Case enum_MetodoProduzione.Biologico
                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(Sup, SAU_Bio_Ettari, SAU_Bio_Are, SAU_Bio_Centiare)
            End Select


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE AppezzamentixParticelle SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Area   =  " & Agro_SQL_SaveNum(Sup))
            StrSQL.Append("             ,SAU_Convenz_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convenz_Ettari))
            StrSQL.Append("             ,SAU_Convenz_Are   =  " & Agro_SQL_SaveNum(SAU_Convenz_Are))
            StrSQL.Append("             ,SAU_Convenz_Centiare   =  " & Agro_SQL_SaveNum(SAU_Convenz_Centiare))
            StrSQL.Append("             ,SAU_Convers_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convers_Ettari))
            StrSQL.Append("             ,SAU_Convers_Are   =  " & Agro_SQL_SaveNum(SAU_Convers_Are))
            StrSQL.Append("             ,SAU_Convers_Centiare   =  " & Agro_SQL_SaveNum(SAU_Convers_Centiare))
            StrSQL.Append("             ,SAU_Bio_Ettari   =  " & Agro_SQL_SaveNum(SAU_Bio_Ettari))
            StrSQL.Append("             ,SAU_Bio_Are   =  " & Agro_SQL_SaveNum(SAU_Bio_Are))
            StrSQL.Append("             ,SAU_Bio_Centiare   =  " & Agro_SQL_SaveNum(SAU_Bio_Centiare))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

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

