Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Audit_Risposte_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiTipologieRiferimento(ByVal Piva As String,
                                        ByVal Riferimento_Cod As String,
                                        ByVal TipoDocumento_Cod As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.LeggiTipologieRiferimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT alert_tipologia.ID_Area,alert_tipologia.ID_Tipologia,alert_tipologia.Nome, alert_indicextipologia.ChkObbligatorio_Tipologia, ")
            StrSQL.AppendLine(" alert_indice.ID_Indice, alert_indice.Piva, Alert_Indice.TitoloIndice, Alert_Indice.ChkObbligatorio, Alert_Indice.ChkIndice_Speciale ")
            StrSQL.AppendLine(" FROM alert_tipologia ")
            StrSQL.AppendLine(" LEFT JOIN alert_indicextipologia ON alert_tipologia.id_tipologia=alert_indicextipologia.ID_Tipologia ")
            StrSQL.AppendLine(" LEFT JOIN alert_indice ON alert_indicextipologia.pivasuperuser=alert_indice.pivasuperuser ")
            StrSQL.AppendLine("   AND (alert_indicextipologia.piva=alert_indice.Piva OR alert_indice.Piva='') ")
            StrSQL.AppendLine("   AND alert_indicextipologia.id_indice=alert_indice.id_indice ")
            StrSQL.AppendLine(" WHERE alert_indicextipologia.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("   AND alert_indicextipologia.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine("   AND alert_indicextipologia.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine(" AND alert_tipologia.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            ' filtra per riferimento
            If Not String.IsNullOrEmpty(Riferimento_Cod) Then
                Dim chiave = Riferimento_Cod.Split("_")
                Dim elenco_tipo As Integer = 0
                Select Case chiave(0)
                    Case enum_TipoEntita.Centro
                        elenco_tipo = 6
                    Case enum_TipoEntita.Campo
                        elenco_tipo = 7
                    Case enum_TipoEntita.Contatto
                        elenco_tipo = 3
                    Case enum_TipoEntita.Impianto
                        elenco_tipo = 4
                    Case Else
                End Select
                StrSQL.AppendLine(" AND Alert_Indice.TipoCampo = 2 ")
                StrSQL.AppendLine(" AND Alert_Indice.Elenco_Tipo = " & Agro_SQL_SaveNum(elenco_tipo) & " ")
            End If

            If Not String.IsNullOrEmpty(TipoDocumento_Cod) Then
                StrSQL.AppendLine(" AND Alert_Tipologia.ID_Tipologia IN ( " & Agro_SQL_Save_Clausola_IN(TipoDocumento_Cod) & " ) ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Alert_Tipologia.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Alert_Tipologia.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiAreaDocumenti(ByVal TipoDocumento_Cod As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.LeggiAreaDocumenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Alert_Tipologia ")

            If Not String.IsNullOrEmpty(TipoDocumento_Cod) Then
                StrSQL.AppendLine(" WHERE ID_Tipologia IN (" & Agro_SQL_Save_Clausola_IN(TipoDocumento_Cod) & ") ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            Return DT.Rows(0).Item("ID_Area")
        End If

        Return 0

    End Function

    '##############################################################################################
    Public Function LeggiAuditDocumenti(ByVal Piva As String,
                                        ByVal Riferimento_Cod As String,
                                        ByVal TipoEntita_Cod As Integer,
                                        ByVal TipoDocumento_Cod As String,
                                        ByVal Allegati_Documenti_Cod As Integer,
                                        ByVal Data As Date,
                                        ByVal Indice As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal DataUpload As Boolean = False,
                                        Optional ByVal FiltroStoricizzati As Boolean = False,
                                        Optional ByVal WorkFlow_Documentale As Boolean = False,
                                        Optional ByVal Audit_Tipo As Integer = 0) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.LeggiAuditDocumenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Alert_Entita.ID_Alert_Entita, Alert_Entita.PivaSuperUser, Alert_Entita.Piva, Alert_Entita.Cod_Contatto, Alert_Entita.TipoEntita_Cod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Cod, Allegati_Documenti.Allegati_Documenti_Numero, Allegati_Documenti.Allegati_Documenti_Des, ")
            StrSQL.AppendLine("   Allegati_Documenti.Sottocartella, Allegati_Documenti.Allegati_Documenti_NomeFile, Allegati_Documenti.Allegati_Documenti_CatCod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Ente_Cod, Allegati_Documenti.Allegati_Documenti_Ente_Des, Allegati_Documenti.Validazione_Data, Allegati_Documenti.Validazione_Flag, ")
            StrSQL.AppendLine("   Alert_Elenco.ID_Elenco, Alert_Elenco.Data_Scadenza, Alert_Elenco.Descrizione_Scadenza, Alert_Elenco.ID_Tipologia, Alert_Tipologia.Nome AS Tipologia, ")
            StrSQL.AppendLine("   Alert_Tipologia.ID_Area, Alert_Area.Nome AS Area, Alert_Entita.ChkStorico ")
            If WorkFlow_Documentale Then
                StrSQL.AppendLine(" ,psa.stato_cod As Stato_Attuale, ")
                StrSQL.AppendLine(" wa.WAnagraficaStati_Des As Descrizione_Stato ")
            End If

            'StrSQL.AppendLine("   ,Alert_Indice.Piva, Alert_Indice.ID_Indice, Alert_Indice.TitoloIndice, Alert_Indice.ChkObbligatorio, Alert_Indice.Chkindice_Speciale, ")
            'StrSQL.AppendLine("   ,Alert_EntitaxIndici.ID_Indice_Det, Alert_EntitaxIndici.Elenco_Val, Alert_EntitaxIndici.Valore_Des ")

            If Indice <> 0 Then
                StrSQL.AppendLine("   ,Alert_EntitaxIndici.Valore_Des AS Valore_Indice ")
            End If
            StrSQL.AppendLine(" FROM Alert_Entita ")
            StrSQL.AppendLine(" INNER JOIN Allegati_Documenti ON Alert_Entita.PivaSuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco ON Alert_Entita.PivaSuperUser = Alert_Elenco.PivaSuperUser AND Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")

            If WorkFlow_Documentale Then
                StrSQL.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON Allegati_Documenti.Allegati_Documenti_SuperUser = psa.Piva_SuperUser and Allegati_Documenti.pratica_Cod = psa.pratica_Cod ")
                StrSQL.AppendLine(" LEFT JOIN WAnagraficaStati wa ON wa.WAnagraficaStati_Cod = psa.Stato_Cod ")
            End If


            If Not String.IsNullOrEmpty(Riferimento_Cod) OrElse Indice <> 0 And Audit_Tipo <> 17 Then
                StrSQL.AppendLine(" LEFT JOIN Alert_EntitaxIndici ON Alert_Entita.PivaSuperUser = Alert_EntitaxIndici.PivaSuperUser AND Alert_Entita.PivaSuperUser = Alert_EntitaxIndici.Piva AND Alert_Entita.ID_Alert_Entita = Alert_EntitaxIndici.ID_Alert_Entita ")
                StrSQL.AppendLine(" LEFT JOIN Alert_Indice ON Alert_EntitaxIndici.PivaSuperUser = Alert_Indice.PivaSuperUser AND (Alert_EntitaxIndici.Piva = Alert_Indice.Piva OR Alert_Indice.Piva = '') AND Alert_EntitaxIndici.ID_Indice = Alert_Indice.ID_Indice ")
            End If

            StrSQL.AppendLine(" WHERE Alert_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            ' escludo i documenti storicizzati
            If FiltroStoricizzati Then
                StrSQL.AppendLine("   AND Alert_Entita.ChkStorico <> 1 ")
            End If

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                'StrSQL.AppendLine(" AND (Alert_Indice.Piva='' OR Alert_Indice.Piva ='" & Agro_SQL_SaveText(Piva) & "') ")
            End If

            ' filtra per riferimento
            If Not String.IsNullOrEmpty(Riferimento_Cod) Then

                Select Case Audit_Tipo

                    Case 17 'Trasporti

                        Dim Arrayp = Split(Riferimento_Cod & "|", "|")
                        If UBound(Arrayp) > 0 Then
                            For i = 0 To UBound(Arrayp) - 1
                                Dim Arrayk = Split(Arrayp(i), "-")
                                If Trim(Arrayk(1)) <> "" Then
                                    StrSQL.AppendLine(" AND Alert_Entita.Id_Alert_Entita In (Select Id_Alert_Entita From Alert_EntitaxIndici Where Id_Indice = " & Arrayk(0) & " And (Elenco_Val = '" & Agro_SQL_SaveText(Arrayk(1)) & "' Or Upper(Valore_Des) = '" & Agro_SQL_SaveText(UCase(Arrayk(1))) & "'))")
                                End If

                            Next
                        End If

                    Case Else

                        Dim chiave = Riferimento_Cod.Split("_")
                        Dim elenco_tipo As Integer = 0
                        Dim elenco_val As String = ""
                        Select Case chiave(0)
                            Case enum_TipoEntita.Centro
                                elenco_tipo = 6
                                elenco_val = chiave(1) & "*" & chiave(2)
                            Case enum_TipoEntita.Campo
                                elenco_tipo = 7
                                elenco_val = chiave(1) & "*" & chiave(2) & "*" & chiave(3)
                            Case enum_TipoEntita.Contatto
                                elenco_tipo = 3
                                elenco_val = chiave(2)
                            Case enum_TipoEntita.Impianto
                                elenco_tipo = 4
                                elenco_val = chiave(2)
                            Case Else
                        End Select
                        StrSQL.AppendLine(" AND Alert_Indice.TipoCampo = 2 ")
                        StrSQL.AppendLine(" AND Alert_Indice.Elenco_Tipo = " & Agro_SQL_SaveNum(elenco_tipo) & " ")
                        StrSQL.AppendLine(" AND Alert_EntitaxIndici.Elenco_Val = '" & Agro_SQL_SaveText(elenco_val) & "' ")

                End Select

            End If




            If Indice <> 0 Then
                StrSQL.AppendLine(" AND Alert_EntitaxIndici.ID_Indice = " & Agro_SQL_SaveNum(Indice) & " ")
            End If

            If TipoEntita_Cod <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.TipoEntita_Cod = " & Agro_SQL_SaveNum(TipoEntita_Cod) & " ")
            End If

            If Not String.IsNullOrEmpty(TipoDocumento_Cod) Then
                StrSQL.AppendLine(" AND Alert_Elenco.ID_Tipologia IN (" & Agro_SQL_Save_Clausola_IN(TipoDocumento_Cod) & ") ")
                'StrSQL.AppendLine(" AND Allegati_Documenti.Allegati_Documenti_CatCod = " & Agro_SQL_SaveNum(TipoDocumento_Cod) & " ")
            End If

            If Allegati_Documenti_Cod <> 0 Then
                StrSQL.AppendLine(" AND Alert_Entita.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            End If

            If WorkFlow_Documentale Then
                StrSQL.AppendLine(" AND psa.pratica_Cod IS NOT NULL ")
            End If

            If Data <> AGRODATAINIZIO Then
                If DataUpload Then
                    StrSQL.AppendLine(" AND (Allegati_Documenti.Data_Upload >= " & Agro_SQL_SaveDate(Data) & " OR Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data) & ") ")
                Else
                    StrSQL.AppendLine(" AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data) & " ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            If LivelloCompatibilita(objParametri) >= 150 Then
                StrSQL.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
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
    Public Function Leggi(
                            ByVal Audit_Cod As Int32,
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Disp_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            'TODO
            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Audit_risposte ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Audit_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If


            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Disp_cod = " & Agro_SQL_SaveNum(Disp_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Audit_SuperUser, Audit_Tipo, Audit_Cod Asc ")
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


    '################################################################################
    Public Function AuditRisposte_Leggi_Punteggio(ByVal Audit_Tipo As Integer,
                                                  ByVal Regolamento_Cod As Integer,
                                                  ByVal Audit_Cod As Integer,
                                                  ByVal Audit_SuperUser As String,
                                                  ByVal Disp_Cod As Long,
                                                  ByVal Punto_Numero As String,
                                                  ByRef ErrMSG As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                  Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                  Optional ByVal OrdinaxDate As Boolean = False,
                                                  Optional ByVal Sezione_Cod As Integer = 0,
                                                  Optional ByVal Valore As String = ""
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Append(" SELECT Audit_Risposte.*, Audit_Codici.Tipo, Audit_Codici.Descrizione, Audit_Codici.Punteggio, Audit_Codici.PropostaCorrettiva ")
            StrSQL.Append("  FROM   Audit_Risposte ")
            StrSQL.Append(" INNER JOIN  Audit_Codici ON Audit_Risposte.Disp_Cod = Audit_Codici.Disp_Cod And Audit_Risposte.Punto_Numero = Audit_Codici.Punto_Numero ")
            StrSQL.Append(" And         Audit_Risposte.Audit_Tipo = Audit_Codici.Audit_Tipo ")
            StrSQL.Append(" And         Audit_Risposte.Regolamento_Cod = Audit_Codici.Regolamento_Cod")
            StrSQL.Append(" WHERE  Audit_Risposte.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" And    Audit_Risposte.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" And    Audit_Risposte.Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser) & "'")

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Audit_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            End If

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Punto_Numero <> "" Then
                StrSQL.Append(" AND Audit_Risposte.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "'")
            End If

            If Sezione_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod))
            End If

            If Valore <> "" Then
                StrSQL.Append(" AND Audit_Risposte.Valore = '" & Agro_SQL_SaveText(Valore) & "'")
            End If


            If OrdinaxDate Then
                StrSQL.Append(" ORDER BY Audit_Risposte.Validita_Inizio,Audit_Risposte.Audit_SuperUser, Audit_Risposte.Disp_cod, Audit_Risposte.Punto_Numero ASC ")
            Else
                StrSQL.Append(" ORDER BY Audit_Risposte.Audit_SuperUser, Audit_Risposte.Disp_cod, Audit_Risposte.Punto_Numero, Audit_Risposte.Validita_Inizio ASC ")
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

    Public Function AuditRisposte_Leggi_Punteggio_Senza_Codici(ByVal Audit_Tipo As Integer,
                                                  ByVal Regolamento_Cod As Integer,
                                                  ByVal Audit_Cod As Integer,
                                                  ByVal Audit_SuperUser As String,
                                                  ByVal Disp_Cod As Long,
                                                  ByVal Punto_Numero As String,
                                                  ByRef ErrMSG As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                  Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                  Optional ByVal OrdinaxDate As Boolean = False,
                                                  Optional ByVal Valore As String = ""
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT Audit_Risposte.*, '' AS Tipo, '' AS Descrizione, 0 AS Punteggio, 0 AS PropostaCorrettiva ")
            StrSQL.Append("  FROM   Audit_Risposte ")
            StrSQL.Append(" WHERE  Audit_Risposte.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" And    Audit_Risposte.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" And    Audit_Risposte.Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser) & "'")

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Audit_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            End If

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Risposte.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Punto_Numero <> "" Then
                StrSQL.Append(" AND Audit_Risposte.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "'")
            End If

            If Valore <> "" Then
                StrSQL.Append(" AND Audit_Risposte.Valore = '" & Agro_SQL_SaveText(Valore) & "'")
            End If


            If OrdinaxDate Then
                StrSQL.Append(" ORDER BY Audit_Risposte.Validita_Inizio,Audit_Risposte.Audit_SuperUser, Audit_Risposte.Disp_cod, Audit_Risposte.Punto_Numero ASC ")
            Else
                StrSQL.Append(" ORDER BY Audit_Risposte.Audit_SuperUser, Audit_Risposte.Disp_cod, Audit_Risposte.Punto_Numero, Audit_Risposte.Validita_Inizio ASC ")
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

    Public Function LeggiDaCodiciGG(Audit_Cod As Integer,
                                    Audit_Tipo As Integer,
                                    Regolamento_Cod As Integer,
                                    Disp_Cod As Integer,
                                    Punto_Numero As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.LeggiDaCodiciGG()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Append(" SELECT Valore, Valore_2, ISNULL(PropostaCorrettiva, '') as ValPropostaCorrettiva ")
            StrSQL.Append(" From Audit_Risposte ")
            StrSQL.Append(" WHERE Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & " ")
            StrSQL.Append(" AND Audit_Risposte.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            StrSQL.Append(" AND Audit_Risposte.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append(" AND Audit_Risposte.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod) & " ")
            StrSQL.Append(" AND Audit_Risposte.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "' ")

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

    Public Function Controlla(
                            ByVal Audit_Cod As Int32,
                            ByVal Audit_Tipo As Int32,
                            ByVal Regolamento_Cod As Int32,
                            ByVal Disp_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_R.Controlla()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Audit_Tipo, Audit_Cod, Disp_Cod, COUNT(*) AS Nr ")
            StrSQL.Append(" FROM  Audit_Risposte ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Audit_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Disp_cod = " & Agro_SQL_SaveNum(Disp_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.Append(" GROUP BY Audit_Tipo, Audit_Cod, Disp_Cod")

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Audit_Tipo, Audit_Cod ")
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


Public Class Audit_Risposte_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function scrivi(
                           ByVal Audit_Cod As Integer _
                        , ByVal Audit_Tipo As Integer _
                        , ByVal Punto_Numero As String _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Disp_Cod As Integer _
                        , ByVal PropostaCorrettiva As String _
                        , ByVal Valore As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByVal Data_creazione As Date _
                        , ByVal Data_modifica As Date _
                        , ByVal username_creazione As String _
                        , ByVal username_modifica As String _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Audit_risposte ( " & vbCrLf)
            StrSQL.Append("  [Audit_Cod] " & vbCrLf)
            StrSQL.Append(" ,[Audit_Tipo] " & vbCrLf)
            StrSQL.Append(" ,[Punto_Numero] " & vbCrLf)
            StrSQL.Append(" ,[Regolamento_Cod] " & vbCrLf)
            StrSQL.Append(" ,[Disp_Cod] " & vbCrLf)
            StrSQL.Append(" ,[PropostaCorrettiva] " & vbCrLf)
            StrSQL.Append(" ,[Valore] " & vbCrLf)
            StrSQL.Append(" , [Audit_SuperUser] " & vbCrLf)
            StrSQL.Append(" , Inviato " & vbCrLf)
            StrSQL.Append(" , DataInvio " & vbCrLf)
            StrSQL.Append(" , Data_Creazione " & vbCrLf)
            StrSQL.Append(" , Data_Modifica " & vbCrLf)
            StrSQL.Append(" , UserName_Creazione " & vbCrLf)
            StrSQL.Append(" , UserName_Modifica " & vbCrLf)
            StrSQL.Append(" , Validita_Inizio " & vbCrLf)
            StrSQL.Append(" , Validita_Fine " & vbCrLf)
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append("  " & Agro_SQL_SaveNum(Audit_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Audit_Tipo) & " " & vbCrLf)
            StrSQL.Append(", '" & Agro_SQL_SaveText(Punto_Numero) & "' " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Disp_Cod) & " " & vbCrLf)
            If IsNothing(PropostaCorrettiva) Then
                StrSQL.Append(", NULL" & vbCrLf)
            Else
                StrSQL.Append(", '" & Agro_SQL_SaveText(PropostaCorrettiva) & "' " & vbCrLf)
            End If
            If IsNothing(Valore) Then
                StrSQL.Append(", NULL" & vbCrLf)
            Else
                StrSQL.Append(", '" & Agro_SQL_SaveText(Valore) & "' " & vbCrLf)
            End If
            StrSQL.Append(",'" & Agro_SQL_Load(objParametri.PivaSuperUser) & "'" & vbCrLf)
            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append(") " & vbCrLf)
            '--------------------------------------------


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

    Public Function ScriviRisposte(
                           ByVal Audit_Cod As Integer _
                        , ByVal Audit_Tipo As Integer _
                        , ByVal Punto_Numero As String _
                        , ByVal Regolamento_Cod As Integer _
                        , ByVal Disp_Cod As Integer _
                        , ByVal PropostaCorrettiva As String _
                        , ByVal Valore As String _
                        , ByVal Valore_2 As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByVal Data_creazione As Date _
                        , ByVal Data_modifica As Date _
                        , ByVal username_creazione As String _
                        , ByVal username_modifica As String _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Audit_risposte ( " & vbCrLf)
            StrSQL.Append("  [Audit_Cod] " & vbCrLf)
            StrSQL.Append(" ,[Audit_Tipo] " & vbCrLf)
            StrSQL.Append(" ,[Punto_Numero] " & vbCrLf)
            StrSQL.Append(" ,[Regolamento_Cod] " & vbCrLf)
            StrSQL.Append(" ,[Disp_Cod] " & vbCrLf)
            StrSQL.Append(" ,[PropostaCorrettiva] " & vbCrLf)
            StrSQL.Append(" ,[Valore] " & vbCrLf)
            StrSQL.Append(" ,[Valore_2] " & vbCrLf)
            StrSQL.Append(" , [Audit_SuperUser] " & vbCrLf)
            StrSQL.Append(" , Inviato " & vbCrLf)
            StrSQL.Append(" , DataInvio " & vbCrLf)
            StrSQL.Append(" , Data_Creazione " & vbCrLf)
            StrSQL.Append(" , Data_Modifica " & vbCrLf)
            StrSQL.Append(" , UserName_Creazione " & vbCrLf)
            StrSQL.Append(" , UserName_Modifica " & vbCrLf)
            StrSQL.Append(" , Validita_Inizio " & vbCrLf)
            StrSQL.Append(" , Validita_Fine " & vbCrLf)
            StrSQL.Append("       ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append("  " & Agro_SQL_SaveNum(Audit_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Audit_Tipo) & " " & vbCrLf)
            StrSQL.Append(", '" & Agro_SQL_SaveText(Punto_Numero) & "' " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Disp_Cod) & " " & vbCrLf)
            If IsNothing(PropostaCorrettiva) Then
                StrSQL.Append(", NULL" & vbCrLf)
            Else
                StrSQL.Append(", '" & Agro_SQL_SaveText(PropostaCorrettiva) & "' " & vbCrLf)
            End If
            If IsNothing(Valore) Then
                StrSQL.Append(", NULL" & vbCrLf)
            Else
                StrSQL.Append(", '" & Agro_SQL_SaveText(Valore) & "' " & vbCrLf)
            End If
            If IsNothing(Valore_2) Then
                StrSQL.Append(", NULL" & vbCrLf)
            Else
                StrSQL.Append(", '" & Agro_SQL_SaveText(Valore_2) & "' " & vbCrLf)
            End If
            StrSQL.Append(",'" & Agro_SQL_Load(objParametri.PivaSuperUser) & "'" & vbCrLf)
            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append(") " & vbCrLf)
            '--------------------------------------------


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

    Public Function AggiornaRisposte(
                            ByVal Audit_Tipo As Integer,
                            ByVal Regolamento_Cod As Integer,
                            ByVal Audit_Cod As Integer,
                            ByVal Audit_SuperUser As String,
                            ByVal Disp_Cod As Long,
                            ByVal Punto_Numero As String,
                            ByVal Campo As String,
                            ByVal Valore As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_W.AggiornaRisposte()"

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
            StrSQL.AppendLine(" UPDATE Audit_Risposte ")
            StrSQL.AppendLine(" SET " & Campo & " = '" & Agro_SQL_SaveText(Valore) & "'")
            StrSQL.Append("    ,Data_Modifica =" & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("    ,UserName_Modifica ='" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.AppendLine(" WHERE 1=1 ")

            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            'If Audit_Cod <> 0 Then
            StrSQL.AppendLine(" AND Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            'End If

            If Audit_SuperUser <> "" Then
                StrSQL.AppendLine(" AND Audit_Risposte.Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser) & "'")
            End If

            If Disp_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Punto_Numero <> "" Then
                StrSQL.AppendLine(" AND Audit_Risposte.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "'")
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

    Public Function Cancellazione( _
                            ByVal Audit_Tipo As Integer, _
                            ByVal Regolamento_Cod As Integer, _
                            ByVal Audit_Cod As Integer, _
                            ByVal Audit_SuperUser As String, _
                            ByVal Disp_Cod As Long, _
                            ByVal Punto_Numero As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                     ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Risposte_W.Cancellazione()"

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
            StrSQL.AppendLine(" DELETE " & _
                 " FROM   Audit_Risposte " & _
                 " WHERE 1=1 ")

            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            'If Audit_Cod <> 0 Then
            StrSQL.AppendLine(" AND Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            'End If

            If Audit_SuperUser <> "" Then
                StrSQL.AppendLine(" AND Audit_Risposte.Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser) & "'")
            End If

            If Disp_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit_Risposte.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Punto_Numero <> "" Then
                StrSQL.AppendLine(" AND Audit_Risposte.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "'")
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

End Class