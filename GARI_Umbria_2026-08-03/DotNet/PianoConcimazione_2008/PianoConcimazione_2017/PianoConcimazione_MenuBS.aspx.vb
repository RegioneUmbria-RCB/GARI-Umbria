Imports System.Web.Services
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello
Imports AgronicaCoreModello.ParametriInvestimentoCatasto_Temp

Public Class PianoConcimazione_MenuBS
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Dim objParametri_Concimazione As ParametriConcimazione_2017

    Dim objParametri_Pua As ParametriPUA

    '14/10/21 Anna: Piano Nutrizionale 
    Dim objParametri_PianoNutrizionale As PianoNutrizionale

    Public Master_Concimaz As MasterConcimazione

    Public permessi As PermessiUtente

    Public tipologiaPagina As enum_PUARegolamenti_Tipo
    Public mostraPrivati_xPianoNutrizionale As String = "false"
    Public filtroRegolamentiPrivati As String = ""

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPianiConcimazione(ByVal data_inizio As String, ByVal data_fine As String, TipologiaPagina As Integer, filtroRegolamentiPrivati As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            'Inserire il codice QUI..

            r.RispostaOK = True

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE
            End If

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""
            Dim UtenteProfiloCentriSql As String = ""
            Dim DtImpreseVisibili As DataTable
            Dim i As Integer

            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            If Not DtImpreseVisibili Is Nothing Then
                For i = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                Next
                If UtenteProfiloImpreseSql <> "" Then
                    UtenteProfiloImpreseSql = " IMP.piva IN (" & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                End If
            End If



            Dim dt As DataTable
            Dim VisualizzaSpecie As Boolean = False
            Dim VisualizzaFlagDichiarazioneNonUtilizzo As Boolean = True

            If TipologiaPagina = enum_PUARegolamenti_Tipo.PianoComcimazione Then
                Dim objPC As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
                dt = objPC.Leggi_xGriglia(objParamentriConcimazione.Piva, 0, 0, 0,
                                            CDate(data_inizio), CDate(data_fine),
                                                UtenteProfiloImpreseSql, "PT.validita_inizio desc", objParametri_Server, Regolamento_Tipo:=1)
                VisualizzaSpecie = True

                ' 14/10/21 Anna: Piano Nutrizionale
            ElseIf TipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale Or TipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF Then
                'AF: 06/23 Aggiunta gestione di un piano nutrizionale generale. Carico sempre quelli con Regolamento_Tipo = 5 (PianoNutrizionale_IBF), e se l'ambiente è configurato per l'uso dei privati li carico usando il Regolamento_Cod specifico
                Dim objPianoNutrizionale As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
                dt = objPianoNutrizionale.Leggi_xGriglia(objParamentriConcimazione.Piva, 0, 0, 0,
                                                         CDate(data_inizio), CDate(data_fine),
                                                         UtenteProfiloImpreseSql, "PT.validita_inizio desc", objParametri_Server, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF, filtroRegolamentiPrivati)

                VisualizzaFlagDichiarazioneNonUtilizzo = False

            Else
                Dim objPuaTestata As New AgronicaCorePUA_DAL.PUA_Testata_R
                dt = objPuaTestata.Leggi_xGriglia(objParamentriConcimazione.Piva, 0, 0,
                                                        CDate(data_inizio), CDate(data_fine),
                                                            " PT.Regolamento_Cod >= 78 ", " PT.validita_inizio desc ", objParametri_Server)
                'VisualizzaFlagDichiarazioneNonUtilizzo = True

            End If

            'aggiungo il centro alla descrizione se valorizzato
            For Each dr As DataRow In dt.Rows
                If Not IsDBNull(dr.Item("sa_nome")) AndAlso dr.Item("sa_nome") <> "" Then
                    dr.Item("rag_soc") &= " - " & dr.Item("sa_nome")
                End If
            Next

            r.RispostaStringa = CaricaGriglia_PianoConcimazione_xJSON(dt, VisualizzaSpecie, VisualizzaFlagDichiarazioneNonUtilizzo)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggixPratichePianiConcimazione(ByVal data_inizio As String, ByVal data_fine As String, TipologiaPagina As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            'Inserire il codice QUI..

            r.RispostaOK = True

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE
            End If
            Dim impostaPratica As Boolean = False

            Dim servizio As enum_Servizi
            If TipologiaPagina = enum_PUARegolamenti_Tipo.PianoComcimazione Then
                servizio = enum_Servizi.PianoConcimazione
            Else
                servizio = enum_Servizi.PUA
            End If

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpo = objUtenti.Leggi(enum_Impostazioni_Utenti.UTENTE_PRATICHE_DA_ATTIVARE_SCARICO_FASCICOLO,
                                         1,
                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "",
                                         "",
                                         objParametri_Utenti)


            Dim objUtentiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim write_pratica = objUtentiPermessi.Controlla_Permessi_Utente(objParametri_Server.UtenteUsername, 5,
                                                                            enum_Security_Attivita.Gestione_Servizi,
                                                                            enum_Security_Operazione.Modifica,
                                                                            DateTime.Now, "",
                                                                            objParametri_Utenti) OrElse objUtentiPermessi.Controlla_Permessi_Utente(objParametri_Server.UtenteUsername, 5,
                                                                            enum_Security_Attivita.Gestione_Servizi_NEW,
                                                                            enum_Security_Operazione.Modifica,
                                                                            DateTime.Now, "",
                                                                            objParametri_Utenti)


            If dtImpo.Rows.Count > 0 AndAlso write_pratica = True Then
                Dim strImpo = CStr(dtImpo.Rows(0)("Impostazione_Valore_1"))
                Dim arrImpo = strImpo.Split("|")
                If arrImpo.Contains(CStr(servizio)) Then
                    impostaPratica = True
                End If
            End If


            If impostaPratica Then
                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""
                Dim UtenteProfiloCentriSql As String = ""
                Dim DtImpreseVisibili As DataTable
                Dim i As Integer

                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
                If Not DtImpreseVisibili Is Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " IMP.piva IN (" & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
                    End If
                End If



                Dim dt As DataTable

                If TipologiaPagina = enum_PUARegolamenti_Tipo.PianoComcimazione Then
                    Dim objPC As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
                    dt = objPC.Leggi_xGriglia(objParamentriConcimazione.Piva, 0, 0, 0, CDate(data_inizio), CDate(data_fine), UtenteProfiloImpreseSql, "PT.validita_inizio desc", objParametri_Server)
                    'dt = objPC.DistinctTestate_conFiltroPiva(objParamentriConcimazione.Piva, "", "", objParametri_Server)

                Else
                    Dim objPuaTestata As New AgronicaCorePUA_DAL.PUA_Testata_R

                    dt = objPuaTestata.Leggi_xGriglia(objParamentriConcimazione.Piva, 0, 0,
                                                    CDate(data_inizio), CDate(data_fine),
                                         "", " PT.validita_inizio desc ",
                                         objParametri_Server)

                End If

                Dim distinctDT As DataTable = dt.DefaultView.ToTable(True, "Validita_Inizio")
                Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R

                Dim listAnno As New List(Of Integer)
                Dim servizio_des = ""

                Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R
                Dim dtServizio = objServizi.Leggi(servizio, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
                servizio_des = dtServizio.Rows(0)("Servizio_Des")

                For Each rowAnno In distinctDT.Rows

                    Dim data_inizioxPratica As Date = rowAnno(0)



                    Dim dtPratiche = ObjPratiche.Leggi(0, "", objParamentriConcimazione.Piva, "", 0, 0, 0, servizio, data_inizioxPratica, data_inizioxPratica, "", "", objParametri_Server, 0, 0)

                    If dtPratiche.Rows.Count = 0 Then
                        If Not listAnno.Contains(data_inizioxPratica.Year) Then
                            listAnno.Add(data_inizioxPratica.Year)
                        End If
                    End If

                Next

                Dim objAnni As New JArray
                For Each anno In listAnno
                    objAnni.Add(anno)
                Next

                Dim objResp As New JObject()

                objResp("anni") = objAnni
                objResp("servizio_cod") = CStr(servizio)

                Dim msgAnno = ""
                For i = 0 To listAnno.Count - 1
                    msgAnno &= listAnno(i)
                    If i <> listAnno.Count - 1 Then
                        msgAnno &= ", "
                    End If
                Next
                If listAnno.Count > 0 Then
                    objResp("msg") = String.Format(Resources.PianoConcimazione_2017.PerAnniXNonImpostataRelativaPraticaImpostareAutomaticamentePratica, msgAnno, servizio_des)
                Else
                    objResp("msg") = ""
                End If

                r.RispostaStringa = objResp.ToString
                r.RispostaOK = True
            Else
                Dim objResp As New JObject()
                objResp("msg") = ""
                r.RispostaStringa = objResp.ToString
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaPratiche(ByVal strAnni As String, servizio_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim jAnni = JArray.Parse(strAnni)

            Dim objPraticaBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_W
            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
            Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R

            Dim dt_Stati = ObjPratiche.Leggi_StatiServizio(servizio_cod, objParametri_Server)
            Dim stato_iniziale = ObjPratiche.Leggi_StatoInizialeServizio(servizio_cod, "", objParametri_Server)

            Dim WWorkflow_Cod = CInt(ObjPratiche.Leggi_WAnagraficaStati(stato_iniziale, 0, objParametri_Server).Rows(0)("WWorkflow_Cod"))
            For Each jAnno In jAnni
                Dim anno = CInt(jAnno)


                Dim piva = objParamentriConcimazione.Piva
                Dim cuaa = ""
                Dim imprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                cuaa = imprese_codici.Leggi_CUAA(piva, objParametri_Server)
                Dim listaPratiche As New JArray

                Dim Stato = stato_iniziale
                Dim Data_Inizio = New Date(anno, 1, 1)
                Dim Data_Fine = New Date(anno, 12, 31)
                Dim Pratica_Cod As Integer
                objPraticaBIZ.impostaPratica(WWorkflow_Cod,
                                             piva,
                                             cuaa,
                                             objParametri_Server.UtenteUsername,
                                             servizio_cod,
                                             Stato,
                                             objParametri_Server,
                                             objParametri_Utenti, 0, "", 0, True, "",
                                             Data_Inizio, Data_Fine, 0, 0)

            Next

            r.RispostaOK = True
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Public Shared Function CaricaGriglia_PianoDistribuzione_xJSON(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Ricetta_Cod", "kendoKey", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Validita_Inizio", Resources.PianoConcimazione_2017.Creazione, "date")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Piva", Resources.PianoConcimazione_2017.PIVA, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("PivaReale", Resources.PianoConcimazione_2017.PIVA, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        c._Display = False
        l.Add(c)

        c = New ColonneNome("rag_soc", Resources.PianoConcimazione_2017.Azienda, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Ricetta_Des_Long", Resources.PianoConcimazione_2017.Descrizione, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Ricetta_Numero", Resources.PianoConcimazione_2017.Codice, "String")
        l.Add(c)

        c = New ColonneNome("numero_operazioni", Resources.PianoConcimazione_2017.NumeroOperazioni, "number")
        l.Add(c)

        c = New ColonneNome("Validita_Inizio", Resources.PianoConcimazione_2017.Inizio, "Date")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Validita_Fine", Resources.PianoConcimazione_2017.Fine, "date")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Veg_Des", Resources.PianoConcimazione_2017.Specie, "String")
        l.Add(c)

        c = New ColonneNome("Note", Resources.PianoConcimazione_2017.Note, "string")
        l.Add(c)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp


    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function apriAllegatoPC(ByVal PC_Testata_Cod As Integer, ByVal PC_Dettagli_PIVA As String, ByVal Allegati_Documenti_Cod As Integer) As RispostaStandard

        Dim ret As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim objConf As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            ret.RispostaOK = True

            Dim PathAllegati As String = objConf.GestioneAllegati_Repository

            If PathAllegati = "" Then
                ret.RispostaOK = False
                ret.Errore = "config_siti: Path allegati non configurato"
            Else

                If Not PathAllegati.EndsWith("\") Then
                    PathAllegati &= "\"
                End If

                Dim DtAllegati As DataTable = objAllegati.Leggi(Allegati_Documenti_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, PC_Dettagli_PIVA)

                If Not IsNothing(DtAllegati) AndAlso DtAllegati.Rows.Count Then
                    '25/07/2018: cambiata gestione apertura file, altrimenti su amstergias non funzionava
                    'ret.RispostaStringa = objConf.LinkAgronicaStampe.Substring(0, objConf.LinkAgronicaStampe.LastIndexOf("/")) & "/File_Allegati/" & DtAllegati.Rows(0).Item("Sottocartella") & "/" & DtAllegati.Rows(0).Item("Allegati_documenti_nomefile")

                    Dim Sottocartella As String = DtAllegati.Rows(0).Item("Sottocartella")
                    Dim NomeFile As String = DtAllegati.Rows(0).Item("Allegati_documenti_nomefile")

                    If Sottocartella.Trim = "" Then
                        ret.RispostaOK = False
                        ret.Errore = "allegati: sottocartella non valorizzata"
                    Else
                        If NomeFile.Trim = "" Then
                            ret.RispostaOK = False
                            ret.Errore = "allegati: nomefile non valorizzato"
                        Else
                            Dim Pathcompleto As String = PathAllegati & Sottocartella & "\" & NomeFile
                            ret.RispostaStringa = "PC_Stampe/VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica(0,
                                                                                                      AgroKey_EncoderDecoder,
                                                                                                      Nothing) &
                                                  "&fall=" & Stringa_Codifica(True,
                                                                            AgroKey_EncoderDecoder,
                                                                            Nothing) &
                                                  "&pdf=" & Stringa_Codifica(Pathcompleto,
                                                                            AgroKey_EncoderDecoder,
                                                                            Nothing)
                        End If
                    End If

                End If

            End If

        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = "Errore durante l'apertura dell'allegato"
        End Try

        Return ret
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function NuovoPC(ByVal Tipo As enum_PianoConcimazione_Tipo,
                                   ByVal SingolaAzienda As Boolean, ByVal SingolaColtura As Boolean,
                                   ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Inserire il codice QUI..

            r.RispostaOK = True
            Dim link As String

            Select Case Tipo
                Case enum_PianoConcimazione_Tipo.Bilancio
                    link = Link_Bilancio(SingolaAzienda, SingolaColtura, objParametri_Server, Regolamento_Tipo)
                Case enum_PianoConcimazione_Tipo.Schede
                    link = Link_Schede(SingolaAzienda, SingolaColtura, objParametri_Server, Regolamento_Tipo)
                Case Else
                    Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPCNonGestito)
            End Select

            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoPC(ByVal PC_Testata_Cod As Integer, ByVal PC_Dettagli_PIVA As String, ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                  ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Controllo se posso modificare i piani di concimazione

            'Prelevo il link
            r.RispostaOK = True
            Dim link As String = ""

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server)
                        Case enum_PianoConcimazione_Tipo.Schede 'MAS
                            link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server)
                    End Select

                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server)
                        Case enum_PianoConcimazione_Tipo.Schede
                            link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server)
                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPCNonGestito)
                    End Select

                    '15/10/21 Anna: Piano Nutrizionale
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF

                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        link = "PianoNutrizionale\PianoNutrizionale.aspx"
                    Else
                        link = "PianoNutrizionale\PianoNutrizionale_IBF.aspx"
                    End If

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio 'MAS
                            link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server)
                        Case enum_PianoConcimazione_Tipo.Schede 'MAS
                            link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server)
                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPianoNutrizionaleNonGestito)
                    End Select

                    link += "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server)

            End Select

            r.RispostaStringa = link


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaPC(ByVal PC_Testata_Cod As Integer, ByVal PC_Dettagli_PIVA As String, ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                    ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo, ByVal Regolamento_Cod As Integer) As RispostaStandard

        Dim Pagina As String = ""
        Dim ret As New RispostaStandard
        Dim UrlTarget As String
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            ret.Sessione = False
            Return ret
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            ret.RispostaOK = True

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    Pagina = "PUA_STAMPA/PUA_Stampa.aspx"

                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            Pagina = "PC_Bilancio/Bilancio_NPK.aspx"
                        Case enum_PianoConcimazione_Tipo.Schede
                            Pagina = "PC_Schede/Scheda_NPK.aspx"

                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPCNonGestito)
                    End Select

                    '15/10/21 Anna: Piano Nutrizionale
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale
                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            Pagina = "PianoNutrizionale_Bilancio/PianoNutrizionale_StampaBilancio.aspx"
                        Case enum_PianoConcimazione_Tipo.Schede
                            Pagina = "PianoNutrizionale_Schede/PianoNutrizionale_StampaSchede.aspx"

                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPianoNutrizionaleNonGestito)
                    End Select
                    '15/10/21 Anna: Piano Nutrizionale
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF
                    Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPianoNutrizionaleNonGestito)
            End Select


            '' da riattivare quando inseriamo i regolamenti
            'Select Case Regolamento_Cod
            '    Case AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti.PianoComcimazione
            '        Pagina = ""
            '    Case Else
            '        '  Galassi, 14/03/2017 17.01.08: Il /PC_Stampe viene allegato poi
            '        If PC_Tipo = AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Tipo.Schede Then
            '            Pagina = "PC_Schede/Scheda_NPK.aspx"
            '        ElseIf PC_Tipo = AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Tipo.Bilancio Then
            '            Pagina = "PC_Bilancio/Bilancio_NPK.aspx"
            '        End If
            'End Select

            If Pagina <> "" Then

                ' Decommentare per attivare lo scadenziario
                UrlTarget = Pagina &
                            "?o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
                            "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                            "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                            "&r=" + Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                            "&tipo=" + Stringa_Codifica(CStr(Regolamento_Tipo), AgroKey_EncoderDecoder, objParametri_Server)

                UrlTarget = ApriFiltroStampaScadenza(UrlTarget, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Tipo, objParametri_Server)

                'UrlTarget = "PC_Stampe/" & Pagina & _
                '                "?n=" + Stringa_Codifica(CStr(xChiave), AgroKey_EncoderDecoder, Server) + _
                '                "&o=" + Stringa_Codifica(CStr(Qs_Operazione), AgroKey_EncoderDecoder, Server) + _
                '                "&p=" + Stringa_Codifica(CStr(xPiva), AgroKey_EncoderDecoder, Server) + _
                '                "&q=" + Stringa_Codifica(CStr(xPianoConcimazione_Testata), AgroKey_EncoderDecoder, Server) + _
                '                "&t="

                'Dim strOpen As String = "<script language='javascript'>" & vbNewLine & _
                '                        "window.open('" & UrlTarget & "'," & _
                '                        "'Stampe','height=700,width=1000,menubar=yes,scrollbars=yes,top=0,left=0');" & vbNewLine & _
                '                        "</script>"

                'Me.Page.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

                ret.RispostaStringa = UrlTarget

            Else
                ret.RispostaOK = False
                ret.Errore = Resources.PianoConcimazione_2017.NonPossibileStamparePianoSelezionato
            End If
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = Resources.PianoConcimazione_2017.ErroreAperturaPaginaStampa
        End Try

        Return ret
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaPC(ByVal PC_Testata_Cod As Integer, ByVal PC_Dettagli_PIVA As String, ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                      ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo, ByVal Regolamento_Cod As Integer, ByVal blocco_flag As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim permessi As New PermessiUtente()

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Select Case Regolamento_Tipo
                Case enum_PUARegolamenti_Tipo.PUA
                    'If Not (permessi.getPermesso(enum_Security_Attivita.Gest_PUA).Scrittura = True Or permessi.getPermesso(enum_Security_Attivita.Gest_PUA_2).Scrittura = True) Then
                    '    Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiModificaPC)
                    'End If
                Case enum_PUARegolamenti_Tipo.PianoComcimazione
                    If Not (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiModificaPC)
                    End If

                '15/10/21 Anna: Piano Nutrizionale
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF
                    If Not (permessi.getPermesso(enum_Security_Attivita.Piano_Nutrizionale).Scrittura = True) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiModificaPianoNutrizionale)
                    End If
            End Select

            'Prelevo il link
            r.RispostaOK = True
            Dim link As String = ""

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Modifica), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&r=" + Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&blocco_flag=" + Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server)
                        Case enum_PianoConcimazione_Tipo.Schede 'MAS
                            link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Modifica), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&r=" + Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&blocco_flag=" + Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server)
                    End Select

                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Modifica), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&blocco_flag=" + Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server)

                        Case enum_PianoConcimazione_Tipo.Schede
                            link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Modifica), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                                    "&blocco_flag=" + Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server)

                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPCNonGestito)
                    End Select

                    '15/10/21 Anna: Piano Nutrizionale 
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF

                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        link = "PianoNutrizionale\PianoNutrizionale.aspx"
                    Else
                        link = "PianoNutrizionale\PianoNutrizionale_IBF.aspx"
                    End If

                    Select Case PC_Tipo
                        Case enum_PianoConcimazione_Tipo.Bilancio
                            link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server)
                        Case enum_PianoConcimazione_Tipo.Schede
                            link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server)
                        Case Else
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPianoNutrizionaleNonGestito)
                    End Select

                    link += "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Modifica), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&blocco_flag=" + Stringa_Codifica(CStr(blocco_flag), AgroKey_EncoderDecoder, objParametri_Server)
            End Select

            r.RispostaStringa = link


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaPC(ByVal PC_Testata_Cod As Integer,
                                     ByVal PC_Dettagli_PIVA As String,
                                     ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo,
                                     ByVal Regolamento_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim permessi As New PermessiUtente()

        Try
            Lingua.Gias_InizializzaCultura_DaSession()


            r.RispostaOK = True

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    If Not (permessi.getPermesso(enum_Security_Attivita.Gest_PUA).Scrittura = True Or permessi.getPermesso(enum_Security_Attivita.Gest_PUA_2).Scrittura = True) Then
                        Throw New Exception("Non si hanno i permessi per eliminare il PUA")
                    End If

                    Dim FlagTransazioneLocale As Boolean = False
                    Dim FlagConnessioneLocale As Boolean = False

                    Dim bRet As Boolean = False

                    Try
                        'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)


                        '-----------------------------------------------------------------------------------------------------------------------------------

                        'Leggo il Codice della Ricetta
                        Dim objRicette_R As New AgronicaCoreContabDAL.Ricette_R
                        Dim DtRic As DataTable = objRicette_R.Leggi(0, PC_Dettagli_PIVA, 0, enum_TipoRicetta.PianoDistribuzionePua, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Programmazione_Cod = " & PC_Testata_Cod, "", objParametri_Server)
                        Dim ricetta_cod As Integer = If(IsNothing(DtRic) OrElse DtRic.Rows.Count = 0 OrElse Not IsNumeric(DtRic.Rows(0).Item("Ricetta_cod")), 0, DtRic.Rows(0).Item("Ricetta_cod"))

                        If ricetta_cod <> 0 Then
                            Dim objRicettaxAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
                            Dim dtTemp As DataTable = objRicettaxAgenda.Leggi(ricetta_cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If dtTemp.Rows.Count > 0 Then
                                Throw New Exception(Resources.PianoConcimazione_2017.PUAConFertilizzazioniPianificateRibaltateSulRealeCancellareOperazioni)
                            End If
                        End If


                        '-----------------------------------------------------------------------------------------------------------------------------------

                        'letture per LOG

                        Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W

                        Dim objPUA_Test_R As New AgronicaCorePUA_DAL.PUA_Testata_R
                        Dim DtTestata As DataTable
                        DtTestata = objPUA_Test_R.Leggi(Regolamento_Cod, PC_Testata_Cod, PC_Dettagli_PIVA, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                        Dim objPUA_Eff_R As New AgronicaCorePUA_DAL.Pua_Effluente_R
                        Dim DtEff As DataTable
                        DtEff = objPUA_Eff_R.Leggi(Regolamento_Cod, PC_Testata_Cod, 0, "", "", objParametri_Server)

                        Dim objAnaV_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
                        Dim DtAnaV As DataTable
                        DtAnaV = objAnaV_R.Leggi(0, PC_Dettagli_PIVA, 0, 0, 0, 0, PC_Testata_Cod, Regolamento_Cod, 0, "", "", objParametri_Server)

                        Dim objPUA_Let_R As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
                        Dim DtLet As DataTable
                        DtLet = objPUA_Let_R.Leggi(Regolamento_Cod, PC_Testata_Cod, PC_Dettagli_PIVA, 0, 0, 0, 0, "", "", objParametri_Server)

                        '-----------------------------------------------------------------------------------------------------------------------------------

                        'Cancello la testata del PUA
                        Dim objPUA_Test_W As New AgronicaCorePUA_DAL.PUA_Testata_W
                        bRet = objPUA_Test_W.Cancella(PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod, "", objParametri_Server)
                        If Not bRet Then
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUAImpossibileCancellareTestata)
                        Else
                            'log
                            Dim Sa_Cod As Integer = 0
                            Dim Anno As String = ""
                            Dim Pua_Tipo As Integer = 0
                            If Not DtTestata Is Nothing AndAlso DtTestata.Rows.Count > 0 Then
                                Sa_Cod = DtTestata.Rows(0).Item("sa_cod")
                                Anno = DtTestata.Rows(0).Item("pua_anno")
                                Pua_Tipo = DtTestata.Rows(0).Item("pua_tipo")
                            End If
                            objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_Testata", PC_Testata_Cod, Regolamento_Cod, PC_Dettagli_PIVA, Sa_Cod, Anno, Pua_Tipo, Nothing, Nothing, Nothing, Nothing, "", objParametri_Server)
                        End If

                        'Cancello gli Effluenti
                        Dim objPUA_Eff_W As New AgronicaCorePUA_DAL.Pua_Effluente_W
                        bRet = objPUA_Eff_W.Cancella(PC_Testata_Cod, 0, Regolamento_Cod, "", objParametri_Server)
                        If Not bRet Then
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUAImpossibileCancellareEffluenti)
                        Else
                            'log
                            If Not DtEff Is Nothing AndAlso DtEff.Rows.Count > 0 Then
                                For Each dr As DataRow In DtEff.Rows
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_Effluente", PC_Testata_Cod, Regolamento_Cod, dr("id"), dr("eff_cod"), dr("azoto_titoli"), dr("flag_provenienzaesterna"), Nothing, Nothing, Nothing, Nothing, "", objParametri_Server)
                                Next
                            End If
                        End If

                        'Cancello il legame con gli appezzamenti
                        Dim objAnagrafe_VincoliAgronomici_W As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
                        bRet = objAnagrafe_VincoliAgronomici_W.CancellaByPuaCod(PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod, "", objParametri_Server)
                        If Not bRet Then
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUAImpossibileCancellareAppezzamenti)
                        Else
                            'log
                            If Not DtAnaV Is Nothing AndAlso DtAnaV.Rows.Count > 0 Then
                                For Each dr As DataRow In DtAnaV.Rows
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "Anagrafe_VincoliAgronomici", PC_Testata_Cod, Regolamento_Cod, dr("id"), dr("piva"), dr("sa_cod"), dr("appezza"), dr("id_reg"), dr("progetto_cod"), Nothing, Nothing, "", objParametri_Server)
                                Next
                            End If
                        End If

                        'cancello le letamazioni precedenti
                        Dim objPUA_LetamazioniPrecedenti_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W
                        bRet = objPUA_LetamazioniPrecedenti_W.Cancella(PC_Testata_Cod, Regolamento_Cod, PC_Dettagli_PIVA, 0, 0, 0, 0, "", objParametri_Server)
                        If Not bRet Then
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUAImpossibileCancellareLetamazioniPrecedenti)
                        Else
                            'log
                            If Not DtLet Is Nothing AndAlso DtLet.Rows.Count > 0 Then
                                For Each dr As DataRow In DtLet.Rows
                                    objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", PC_Testata_Cod, Regolamento_Cod, dr("id"), dr("piva"), dr("sa_cod"), dr("appezza"), dr("id_reg"), dr("progetto_cod"), dr("eff_cod"), dr("id_fre"), "", objParametri_Server)
                                Next
                            End If
                        End If

                        'cancello le distribuzioni
                        If ricetta_cod <> 0 Then
                            'Recupero la stringa XML di cancellazione della Ricetta
                            Dim r_Read As New AgronicaCoreContabBIZ.Ricette_R
                            Dim StringaXmlCancellazione As String = r_Read.Ricetta_Leggi(ricetta_cod, PC_Dettagli_PIVA, 0, 0, 0, True, objParametri_Server)

                            'Cancello la Ricetta
                            Dim r_Write As New AgronicaCoreContabBIZ.Ricette_W
                            bRet = r_Write.Ricetta_Scrivi(StringaXmlCancellazione, ricetta_cod, objParametri_Server)
                            If Not bRet Then
                                Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUAImpossibileCancellareFertilizzazioniPianificate)
                            End If
                        End If

                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                    Catch ex As Exception
                        AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametri_Server)
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                        Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePUA + ": " & ex.Message)
                    End Try

                    r.RispostaStringa = Resources.PianoConcimazione_2017.CancellazionePUAAvvenutaSuccesso

                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    If Not (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiEliminazionePC)
                    End If

                    Dim objElab_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_R
                    Dim objElab_W As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_W
                    Dim objTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R
                    Dim DT_Testata = objTestata_R.Leggi(PC_Testata_Cod, 0, 0,
                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        " PC_Elaborazione_Cod <> 0 ", "", objParametri_Server)

                    If DT_Testata.Rows.Count > 0 Then
                        For Each testata In DT_Testata.Rows
                            Dim Elaborazione_Cod As Integer = testata("PC_Elaborazione_Cod")
                            Dim dt_Test_Elab = objTestata_R.Leggi(0, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  " PC_Elaborazione_Cod = " & Elaborazione_Cod & " AND PC_Testata_Cod <> " & testata("PC_Testata_Cod") & " ", "", objParametri_Server)

                            If dt_Test_Elab.Rows.Count = 0 Then
                                objElab_W.Cancella(Elaborazione_Cod, "", objParametri_Server)
                            End If

                        Next
                    End If

                    Dim objTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W
                    objTestata.Cancella(PC_Testata_Cod, 0, "", objParametri_Server)

                    Dim objDettagli As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W
                    objDettagli.Cancella(PC_Testata_Cod, 0, PC_Dettagli_PIVA, "", objParametri_Server)

                    Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W
                    objEntitaxTestata.Cancella(PC_Testata_Cod, 0, PC_Dettagli_PIVA, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", "", objParametri_Server)

                    Dim objFattoriCorrettivi As New AgronicaCoreAnagrafeDAL.PianoConcimazione_FattoriCorrettivi_W
                    Dim bRet As Boolean = objFattoriCorrettivi.Cancella(0,
                                                  PC_Testata_Cod,
                                                  0,
                                                  "",
                                                  objParametri_Server)
                    objFattoriCorrettivi = Nothing

                    If Not bRet Then
                        Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazioneFattoriCorrettivi)
                    Else
                        r.RispostaStringa = Resources.PianoConcimazione_2017.CancellazionePCAvvenutaSuccesso
                    End If

                    '15/10/21 Anna: Piano Nutrizionale 
                Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF

                    If Not (permessi.getPermesso(enum_Security_Attivita.Piano_Nutrizionale).Scrittura = True) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.NonPermessiEliminazionePianoNutrizionale)
                    End If

                    Dim objElab_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_R
                    Dim objElab_W As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_W
                    Dim objTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R

                    Dim DT_Testata = objTestata_R.Leggi(PC_Testata_Cod, 0, 0,
                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        " PC_Elaborazione_Cod <> 0 ", "", objParametri_Server)

                    If DT_Testata.Rows.Count > 0 Then
                        For Each testata In DT_Testata.Rows
                            Dim Elaborazione_Cod As Integer = testata("PC_Elaborazione_Cod")
                            Dim dt_Test_Elab = objTestata_R.Leggi(0, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  " PC_Elaborazione_Cod = " & Elaborazione_Cod & " AND PC_Testata_Cod <> " & testata("PC_Testata_Cod") & " ", "", objParametri_Server)

                            If dt_Test_Elab.Rows.Count = 0 Then
                                objElab_W.Cancella(Elaborazione_Cod, "", objParametri_Server)
                            End If

                        Next
                    End If

                    Dim objTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W
                    Dim bRet As Boolean = objTestata.Cancella(PC_Testata_Cod, 0, "", objParametri_Server)

                    If Not bRet Then
                        Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePianoNutrizionaleImpossibileCancellareTestata)
                    Else

                        Dim objDettagli As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_W
                        bRet = objDettagli.Cancella(PC_Testata_Cod, 0, PC_Dettagli_PIVA, "", objParametri_Server)

                        If Not bRet Then
                            Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePianoNutrizionaleImpossibileCancellareDettagli)
                        Else

                            Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W
                            bRet = objEntitaxTestata.Cancella(PC_Testata_Cod, 0, PC_Dettagli_PIVA, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", "", objParametri_Server)

                            If Not bRet Then
                                Throw New Exception(Resources.PianoConcimazione_2017.ErroreCancellazionePianoNutrizionaleImpossibileCancellareRiferimentoImpresa)
                            Else
                                r.RispostaStringa = Resources.PianoConcimazione_2017.CancellazionePianoNutrizionalevvenutaSuccesso
                            End If

                        End If

                    End If


            End Select



        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaPianoDistribuzione(ByVal PC_Testata_Cod As Integer,
                                                  ByVal PC_Dettagli_PIVA As String,
                                                  ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                  ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo,
                                                  ByVal Regolamento_Cod As Integer,
                                                  ByVal Data_Da As Date, ByVal Data_A As Date,
                                                  ByVal PC_Testata_Des As String,
                                                  ByVal blocco_flag As Integer
                                                  ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim UrlTarget As String

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    'Controllo che mi sia tornato un id del piano concimazione
                    If IsNothing(PC_Testata_Cod) Or Not IsNumeric(PC_Testata_Cod) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.ErrorePUACorrotto)
                    End If

                    Dim progressivoGias As Integer = HttpContext.Current.Session("ASG_ProgressivoGIAS")

                    Dim OUTPUT_Ricetta_Cod As Integer = 0
                    Dim xRisp As Boolean = ScriviNuovaRicettaPUA(PC_Dettagli_PIVA, PC_Testata_Cod, PC_Testata_Des, Data_Da,
                                                                 progressivoGias,
                                                                 objParametri_Server, OUTPUT_Ricetta_Cod)

                    If xRisp = False OrElse OUTPUT_Ricetta_Cod = 0 Then
                        Throw New Exception(Resources.PianoConcimazione_2017.ErroreImpossibileCrearePianoDistribuzionePUA)
                    End If

                    UrlTarget = GetUrlPuaPianoDistribuzione(enum_TipoOperazioneDB.Scrittura,
                                                            enum_PUA_Modalita.Modalita_PianoDistribuzione,
                                                            PC_Tipo, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod, Data_Da, Data_A,
                                                            OUTPUT_Ricetta_Cod, blocco_flag, objParametri_Server)

                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    Dim XmlDoc As New System.Xml.XmlDocument
                    Dim Xml_FiltroRisultati As System.Xml.XmlElement
                    Dim Xml_Risultato As System.Xml.XmlElement

                    Dim StrNodiVariabili As String = ""
                    Dim StrNodo As String = ""
                    Dim Piva As String = PC_Dettagli_PIVA
                    Dim rag_soc As String
                    Dim sa_nome As String
                    Dim f As Boolean = False
                    Dim Veg_Cod As Integer 'si suppongono specie omogenee
                    Dim Num_Checked As Integer = 0 'utilizzata solo per controllare di selezionare almeno una riga


                    'Controllo che mi sia tornato un id del piano concimazione
                    If IsNothing(PC_Testata_Cod) Or Not IsNumeric(PC_Testata_Cod) Then
                        Throw New Exception(Resources.PianoConcimazione_2017.ErrorePCCorrotto)
                    End If

                    'Leggo la testata del piano di concimazione
                    Dim objPC_TestataR As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R
                    Dim Dt_Testata As DataTable = objPC_TestataR.Leggi(PC_Testata_Cod, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    'Leggo gli impianti collegati
                    Dim objPC_EntitaxTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
                    Dim Dt_EntitaxTestata As DataTable = objPC_EntitaxTestata_R.Leggi(PC_Testata_Cod, 0, PC_Dettagli_PIVA, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    'Oggetto per leggere gli appezzamenti
                    Dim obj_AppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

                    'Lista degli impianti collegati
                    Dim listaOfImpianti As New List(Of AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo)

                    'Popolo la lista degli impianti collegati con tanto di NPK
                    For Each dr As DataRow In Dt_EntitaxTestata.Rows

                        'If dr.Item("Id_Imp") <> 0 OrElse sCurImp(0) = 51 Then
                        If dr.Item("Id_Imp") <> 0 Then

                            'Leggo il nome dell'appezzamento
                            Dim nomeApp As String = obj_AppezzamentoR.AppezzamentoNome_from_Appezza(
                                dr.Item("Piva"), dr.Item("Sa_Cod"), dr.Item("Appezza"), objParametri_Server)

                            'Creo l'oggetto con i dati dell'appezzamento
                            listaOfImpianti.Add(New AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo With {
                                .Piva = dr.Item("Piva"),
                                .Sa_Cod = dr.Item("Sa_Cod"),
                                .campo_cod = dr.Item("Campo_Cod"),
                                .Appezza = dr.Item("Appezza"),
                                .Id_reg = dr.Item("Id_Imp"),
                                .app_nome = nomeApp,
                                .Progetto_Cod = dr.Item("Progetto_Cod"),
                                .N = dr.Item("QtaMaxN"),
                                .P = dr.Item("QtaMaxP2O5"),
                                .K = dr.Item("QtaMaxK2O")
                            })
                        Else
                            Throw New Exception(Resources.PianoConcimazione_2017.SelezionarePC)
                        End If
                    Next

                    ''''''''''''''''VERIFICA
                    'se avessi un elenco multiplo di piani, dovrei ciclare...
                    Veg_Cod = Gestione_Ricetta_GeneraXMl(objParametri_Server, XmlDoc, Xml_Risultato, StrNodiVariabili, StrNodo, Piva, Num_Checked, listaOfImpianti, rag_soc, sa_nome, f)


                    'Creo il nodo "FiltroRisultati"
                    Xml_FiltroRisultati = XmlDoc.CreateElement("FiltroRisultati")

                    'Rendo l'albero figlio del documento
                    XmlDoc.AppendChild(Xml_FiltroRisultati)

                    'Inserisco gli elementi "VariabiliStampe" come figli del nodo "FiltroStampa"
                    Xml_FiltroRisultati.InnerXml = StrNodiVariabili

                    'Estraggo la stringa XML complessiva
                    Dim StrVariabiliFiltro As String = XmlDoc.InnerXml

                    'Creo l'oggetto con le info sulla ricetta
                    Dim objRicette2010 As New AgronicaCoreGestioneRichieste.ParametriRicette_2010
                    If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
                        objRicette2010 = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriRicette_2010)
                    End If

                    'inizializzo l'oggetto con le altre informazioni
                    objRicette2010.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    objRicette2010.Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi
                    objRicette2010.Ricetta_Cod = 0
                    objRicette2010.Piva = Piva
                    objRicette2010.Sa_Cod = 0
                    objRicette2010.Veg_Cod = Veg_Cod
                    objRicette2010.Cul_Cod = 0
                    objRicette2010.data_inizio = Dt_Testata.Rows(0).Item("Validita_Inizio") 'TEST Simone
                    objRicette2010.data_fine = Dt_Testata.Rows(0).Item("Validita_Fine") 'TEST Simone
                    objRicette2010.StrVariabiliAgenda = StrVariabiliFiltro
                    objRicette2010.Ricetta_Des = "Piano Distribuzione (" & Dt_Testata.Rows(0).Item("PC_Testata_Des") & ")"
                    objRicette2010.Ricetta_Des_Long = objRicette2010.Ricetta_Des
                    objRicette2010.Programmazione_Cod = Dt_Testata.Rows(0).Item("PC_Testata_Cod")
                    objRicette2010.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                    objRicette2010.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017

                    'Carico la pagina
                    UrlTarget = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objRicette2010)

            End Select

            r.RispostaOK = True
            r.RispostaStringa = UrlTarget
            'Response.Redirect(link)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Private Shared Function GetUrlPuaPianoDistribuzione(ByVal tipoOp As enum_TipoOperazioneDB,
                                                        ByVal modalita As enum_PUA_Modalita,
                                                        ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                        ByVal PC_Dettagli_PIVA As String,
                                                        ByVal PC_Testata_Cod As Integer,
                                                        ByVal Regolamento_Cod As Integer,
                                                        ByVal Data_Inizio As Date, ByVal Data_Fine As Date,
                                                        ByVal Ricetta_Cod As Integer,
                                                        ByVal Blocco_Flag As Integer,
                                                        ByRef objParametri_Server As AgronicaCoreParametri
                                                        ) As String
        Dim urlTarget As String

        urlTarget = "PUA\PUA_Piano_Distribuzione.aspx?tipo=" + Stringa_Codifica(CStr(PC_Tipo), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&o=" & Stringa_Codifica(CStr(tipoOp), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&m=" & Stringa_Codifica(CStr(modalita), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&p=" & Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&q=" & Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&r=" & Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_da=" & Stringa_Codifica(CStr(Data_Inizio), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&data_a=" & Stringa_Codifica(CStr(Data_Fine), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&blocco_flag=" & Stringa_Codifica(CStr(Blocco_Flag), AgroKey_EncoderDecoder, objParametri_Server) &
                    "&ric=" & Stringa_Codifica(CStr(Ricetta_Cod), AgroKey_EncoderDecoder, objParametri_Server)

        Return urlTarget

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPianiDistribuzione(ByVal PC_Testata_Cod As Integer,
                                                   ByVal PC_Dettagli_PIVA As String,
                                                   ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                   ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo,
                                                   ByVal Regolamento_Cod As Integer,
                                                   ByVal blocco_flag As Integer,
                                                   ByVal data_inizio As String,
                                                   ByVal data_fine As String
                                                   ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        'objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Inserire il codice QUI..

            r.RispostaOK = True

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE
            End If

            Select Case Regolamento_Tipo

                Case enum_PUARegolamenti_Tipo.PUA

                    Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
                    Dim dt As DataTable = objRicette.Leggi_xGriglia(0, PC_Dettagli_PIVA, 0, enum_TipoRicetta.PianoDistribuzionePua,
                                                                    0, PC_Testata_Cod, data_inizio, data_fine,
                                                                    "", "", objParametri_Server)

                    If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                        Dim urlTarget As String = GetUrlPuaPianoDistribuzione(enum_TipoOperazioneDB.Modifica,
                                                                              enum_PUA_Modalita.Modalita_PianoDistribuzione,
                                                                              PC_Tipo, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod,
                                                                              CDate(dt.Rows(0).Item("Validita_Inizio")), CDate(dt.Rows(0).Item("Validita_Fine")),
                                                                              dt.Rows(0).Item("Ricetta_Cod"), blocco_flag,
                                                                              objParametri_Server)

                        r.RispostaStringa = urlTarget
                        r.ParametroDue_stringa = CStr(dt.Rows.Count)
                    Else
                        r.RispostaStringa = ""
                        r.ParametroDue_stringa = CStr(0)
                    End If


                Case enum_PUARegolamenti_Tipo.PianoComcimazione

                    Dim dt As DataTable
                    Dim objRicette As New AgronicaCoreContabDAL.Ricette_R

                    dt = objRicette.Leggi_xGriglia(0, PC_Dettagli_PIVA, 0, enum_TipoRicetta.PianoDistribuzioneConcimi, 0, PC_Testata_Cod, data_inizio, data_fine,
                                          "", "", objParametri_Server)


                    r.RispostaStringa = CaricaGriglia_PianoDistribuzione_xJSON(dt)
                    r.ParametroDue_stringa = CStr(dt.Rows.Count)

                Case Else

            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriVerificaIndiciBilancio(ByVal PC_Testata_Cod As Integer,
                                                      ByVal PC_Dettagli_PIVA As String,
                                                      ByVal PC_Tipo As enum_PianoConcimazione_Tipo,
                                                      ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo,
                                                      ByVal Regolamento_Cod As Integer,
                                                    ByVal blocco_flag As Integer,
                                                     ByVal data_inizio As String,
                                                      ByVal data_fine As String
                                                      ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        'objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE
            End If


            If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PUA Then

                Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
                Dim dt As DataTable = objRicette.Leggi_xGriglia(0, PC_Dettagli_PIVA, 0, enum_TipoRicetta.PianoDistribuzionePua,
                                                                0, PC_Testata_Cod, data_inizio, data_fine,
                                                                "", "", objParametri_Server)

                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    Dim urlTarget As String = GetUrlPuaPianoDistribuzione(enum_TipoOperazioneDB.Modifica,
                                                                          enum_PUA_Modalita.Modalita_Verifica,
                                                                          PC_Tipo, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Cod,
                                                                          CDate(dt.Rows(0).Item("Validita_Inizio")), CDate(dt.Rows(0).Item("Validita_Fine")),
                                                                          dt.Rows(0).Item("Ricetta_Cod"), blocco_flag,
                                                                          objParametri_Server)

                    r.RispostaOK = True
                    r.RispostaStringa = urlTarget
                    r.ParametroDue_stringa = CStr(dt.Rows.Count)

                Else
                    r.RispostaOK = False
                    r.Errore = Resources.PianoConcimazione_2017.NonStatoAncoraCreatoPD
                    'r.RispostaStringa = ""
                    'r.ParametroDue_stringa = CStr(0)
                End If

            Else
                r.RispostaOK = False
                r.Errore = "Tipologia regolamento è " & Regolamento_Tipo
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Public Shared Function CaricaGriglia_PianoConcimazione_xJSON(ByVal dt As DataTable, ByVal VisualizzaSpecie As Boolean, ByVal VisualizzaFlagDichiarazioneNonUtilizzo As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("PC_Testata_Cod", "PC_Testata_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Tipo", "Regolamento_Tipo", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Validita_Inizio", Resources.PianoConcimazione_2017.ValiditaInizio, "date")
        l.Add(c)
        c = New ColonneNome("Validita_Fine", Resources.PianoConcimazione_2017.ValiditaFine, "date")
        l.Add(c)

        c = New ColonneNome("PC_Dettagli_PIVA", Resources.PianoConcimazione_2017.PIVA, "string")
        c._hidden = True
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)


        c = New ColonneNome("PivaReale", Resources.PianoConcimazione_2017.PIVA, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("rag_soc", Resources.PianoConcimazione_2017.Azienda, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("PC_Testata_Des", Resources.PianoConcimazione_2017.Descrizione, "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        'c = New ColonneNome("Tipo_Des", "Tipo", "string")
        'l.Add(c)

        c = New ColonneNome("Regolamento_Des", Resources.PianoConcimazione_2017.Regolamento, "string")
        c._hidden = False
        l.Add(c)

        c = New ColonneNome("Note", Resources.PianoConcimazione_2017.Note, "string")
        l.Add(c)

        If VisualizzaSpecie Then
            c = New ColonneNome("Veg_Des", Resources.PianoConcimazione_2017.Specie, "string")
            l.Add(c)
        End If


        c = New ColonneNome("PC_Tipo", "PC_Tipo", "number")
        c._hidden = True
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Regolamento_Cod", "Regolamento_Cod", "number")
        c._hidden = True
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Allegati_Documenti_Cod", "Allegati_Documenti_Cod", "number")
        c._hidden = True
        c._Display = False
        l.Add(c)

        c = New ColonneNome("nPianiDistr", "nPianiDistr", "number")
        c._hidden = True
        c._Display = False
        l.Add(c)

        If VisualizzaFlagDichiarazioneNonUtilizzo = True Then

            c = New ColonneNome("Flag_NonUtilizzo_Fertilizzanti", Resources.PianoConcimazione_2017.DichiarazioneNonUtilizzoFertilizzanti, "string")
            c._Tipo = "integer"
            c._FormatoParticolare = "#=(Flag_NonUtilizzo_Fertilizzanti === '1') ? '" & Resources.PianoConcimazione_2017.Si & "' : '" & Resources.PianoConcimazione_2017.No & "'#"
            c._width = "100px"
            l.Add(c)

        End If



        'c = New ColonneNome("PC_Dettagli_ColturaPrincipale_Veg_Cod", "Specie", "number")
        'l.Add(c)

        'c = New ColonneNome("Centro", "Centri di Costo", "string")
        'c._Filtrabile = True
        'c._Display = False
        'l.Add(c)

        c = New ColonneNome("blocco_flag", "blocco_flag", "number")
        c._hidden = True
        c._Display = False
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoPD(ByVal Ricetta_Cod As Integer, ByVal Piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Controllo se posso modificare i piani di concimazione

            'Prelevo il link
            r.RispostaOK = True
            Dim link As String

            'Creo l'oggetto con le info sulla ricetta
            Dim objRicette2010 As New AgronicaCoreGestioneRichieste.ParametriRicette_2010
            If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
                objRicette2010 = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriRicette_2010)
            End If

            'inizializzo l'oggetto con le altre informazioni
            objRicette2010.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
            objRicette2010.Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi
            objRicette2010.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objRicette2010.Ricetta_Cod = Ricetta_Cod
            objRicette2010.Piva = Piva

            'Carico la pagina
            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objRicette2010)

            r.RispostaStringa = link
            'Response.Redirect(link)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaPD(ByVal Ricetta_Cod As Integer, ByVal Piva As String) As RispostaStandard

        Dim Pagina As String
        Dim ret As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            ret.Sessione = False
            Return ret
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            ret.RispostaOK = True

            'Creo l'oggetto con le info sulla ricetta
            Dim objRicette2010 As New AgronicaCoreGestioneRichieste.ParametriRicette_2010
            If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
                objRicette2010 = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriRicette_2010)
            End If

            'inizializzo l'oggetto con le altre informazioni
            objRicette2010.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
            objRicette2010.Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi
            objRicette2010.Ricetta_Cod = Ricetta_Cod
            objRicette2010.Piva = Piva
            objRicette2010.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objRicette2010.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
            objRicette2010.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_RicetteStampa

            Dim link As String

            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objRicette2010)

            'apro la finestra...


            ret.RispostaStringa = link

        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = Resources.PianoConcimazione_2017.ErroreAperturaPaginaStampa
        End Try

        Return ret
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaPD(ByVal Ricetta_Cod As Integer, ByVal Piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim permessi As New PermessiUtente()

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            If Not (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then
                Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiModificaPD)
            End If


            'Controllo se posso modificare i piani di concimazione

            'Prelevo il link
            r.RispostaOK = True
            Dim link As String

            'Creo l'oggetto con le info sulla ricetta
            Dim objRicette2010 As New AgronicaCoreGestioneRichieste.ParametriRicette_2010
            If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
                objRicette2010 = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriRicette_2010)
            End If

            'inizializzo l'oggetto con le altre informazioni
            objRicette2010.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
            objRicette2010.Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi
            objRicette2010.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objRicette2010.Ricetta_Cod = Ricetta_Cod
            objRicette2010.Piva = Piva

            'Carico la pagina
            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objRicette2010)

            r.RispostaStringa = link
            'Response.Redirect(link)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try


        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaPD(ByVal Ricetta_Cod As Integer, ByVal Piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim permessi As New PermessiUtente()

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            If Not (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then
                Throw New Exception(Resources.PianoConcimazione_2017.NoPermessiEliminazionePD)
            End If


            r.RispostaOK = True

            'Creo l'oggetto con le info sulla ricetta
            Dim objRicette2010 As New AgronicaCoreGestioneRichieste.ParametriRicette_2010
            If Not HttpContext.Current.Session("objRicette_2010PerRitorno") Is Nothing Then
                objRicette2010 = CType(HttpContext.Current.Session("objRicette_2010PerRitorno"), AgronicaCoreGestioneRichieste.ParametriRicette_2010)
            End If

            'inizializzo l'oggetto con le altre informazioni
            objRicette2010.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione
            objRicette2010.Tipo_Ricetta = enum_TipoRicetta.PianoDistribuzioneConcimi
            objRicette2010.Ricetta_Cod = Ricetta_Cod
            objRicette2010.Piva = Piva
            objRicette2010.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
            objRicette2010.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017

            'Carico la pagina
            Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriRicette_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objRicette2010)

            r.RispostaStringa = link
            'Response.Redirect(link)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ImportaComunicazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Inserire il codice QUI..

            r.RispostaOK = True
            Dim link As String


            'Select Case Tipo
            '    Case enum_PianoConcimazione_Tipo.Bilancio
            '        link = Link_Bilancio(SingolaAzienda, SingolaColtura, objParametri_Server, Regolamento_Tipo)
            '    Case enum_PianoConcimazione_Tipo.Schede
            '        link = Link_Schede(SingolaAzienda, SingolaColtura, objParametri_Server, Regolamento_Tipo)
            '    Case Else
            '        Throw New Exception(Resources.PianoConcimazione_2017.ErroreTipoPCNonGestito)
            'End Select

            Dim objPua As New AgronicaCoreGestioneRichieste.ParametriPUA
            objPua.Leggi()

            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
                                 TipiEnumerativi.Enum_SiteRedirector.Sito_PianoConcimazione_2017, enum_AuditPuaTipo.PUA,
                                 HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, objPua.Piva, 0)

            'Dim script As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
            '                     TipiEnumerativi.Enum_SiteRedirector.Sito_PianoConcimazione_2017, enum_AuditPuaTipo.PUA,
            '                     HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, objPua.Piva, 0)



            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

#Region "Funzione commentata"
    '<WebMethod(EnableSession:=True)>
    'Public Shared Function StampaRegistroFertilizzazioni(ByVal PC_Testata_Cod As Integer, ByVal PC_Dettagli_PIVA As String, ByVal Data_inizio As Date) As RispostaStandard

    '    Dim Pagina As String
    '    Dim ret As New RispostaStandard
    '    Dim UrlTarget As String
    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        ret.Sessione = False
    '        Return ret
    '    End If

    '    Try
    '        ret.RispostaOK = True


    '        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe

    '        objAgronicaStampe.report = enum_CodificaStampe.Registro_Fertilizzazioni


    '        ''Verifico le stampe che devono avere la specie selezionata
    '        ''spostata la scelta della specie nel filtro pre-stampa
    '        'Dim strVegCod As String = ""
    '        'Dim filtroImp As String = ""

    '        'If Not IsNothing(HttpContext.Current.Session("Filtro")) Then
    '        '    'se ho il filtro dell'impianto allora non occorre selezionare la scpecie perchè l'impianto è 1
    '        '    Dim sessionfiltro As String = HttpContext.Current.Session("Filtro")
    '        '    If sessionfiltro.Split("|").Count > 1 AndAlso sessionfiltro.Split("|")(1).Trim <> "" Then
    '        '        filtroImp = " (" & sessionfiltro.Split("|")(1) & " ) "
    '        '    End If
    '        'Else
    '        '    If specie <> "-1" Then
    '        '        'tare etc
    '        '        If InStr(specie, "/") <> 0 Then
    '        '            strVegCod = Split(specie, "/")(0)
    '        '        Else
    '        '            'specie
    '        '            strVegCod = specie
    '        '        End If
    '        '    End If
    '        'End If



    '        'Dim XmlDoc As New System.Xml.XmlDocument
    '        'Dim StrVariabiliStampe As String = ""
    '        'Dim StrNodiVariabili As String = ""
    '        'Dim StrNodo As String = ""
    '        'Dim objVS As New AgronicaCoreXML.XML_Stampe

    '        'Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
    '        'Dim HashImp As New Hashtable
    '        'Dim leggiAncheBloccati As Boolean = True
    '        'Dim Dt As DataTable = objImpianti.Leggi_Impianti_xAgenda2(False,
    '        '                                    objParametriAgenda.Piva,
    '        '                                    objParametriAgenda.Sa_Cod,
    '        '                                    strVegCod,
    '        '                                    objParametriAgenda.Cul_Cod,
    '        '                                    "",
    '        '                                    0,
    '        '                                    -1,
    '        '                                    filtroImp,
    '        '                                    " Cul_Des, App_Nome, Progetto ",
    '        '                                    objParametri_Server, leggiAncheBloccati)

    '        'Dim vegcodstrtemp As String = ""
    '        'If Dt.Rows.Count = 0 Then
    '        '    'Messaggi.AgroMsgBox("Nessun Impianto Selezionato Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '        '    'Exit Function
    '        '    r.RispostaOK = False
    '        '    r.Errore = "Nessun Impianto Selezionato Per La Stampa"
    '        'End If
    '        'For i = 0 To Dt.Rows.Count - 1
    '        '    'controllo che ci sia una sola specie
    '        '    If i = 0 Then
    '        '        vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
    '        '    End If
    '        '    If vegcodstrtemp <> Dt.Rows(i).Item("veg_cod") AndAlso (tipo_operazione = "7a" Or tipo_operazione = "7b") Then
    '        '        'Messaggi.AgroMsgBox("Non c'è un'unica specie negli impianti selezionati Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '        '        'Exit Function
    '        '        r.RispostaOK = False
    '        '        r.Errore = "Non c'è un'unica specie negli impianti selezionati Per La Stampa"

    '        '    End If
    '        '    vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
    '        '    If Not HashImp.ContainsKey(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg")) Then
    '        '        HashImp.Add(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg"), "")
    '        '        Dim vVarStampe(4) As ElementoStampe
    '        '        vVarStampe(0).Nome = "piva"
    '        '        vVarStampe(0).Valore = objParametriAgenda.Piva
    '        '        vVarStampe(1).Nome = "sa_cod"
    '        '        vVarStampe(1).Valore = Dt.Rows(i).Item("sa_cod")
    '        '        vVarStampe(2).Nome = "appezza"
    '        '        vVarStampe(2).Valore = Dt.Rows(i).Item("appezza")
    '        '        vVarStampe(3).Nome = "id_reg"
    '        '        vVarStampe(3).Valore = Dt.Rows(i).Item("id_reg")
    '        '        vVarStampe(4).Nome = "veg_cod"
    '        '        vVarStampe(4).Valore = Dt.Rows(i).Item("veg_cod")
    '        '        StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
    '        '        StrNodiVariabili = StrNodiVariabili & StrNodo
    '        '    End If
    '        'Next


    '        'objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
    '        'objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
    '        'objAgronicaStampe.Xml_Generico.Length = 0
    '        'objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

    '        'Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '        '                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)

    '        'r.Tipo = "1"
    '        'r.ParametroDue_stringa = strJS


    '        'Pagina = "PUA_STAMPA/PUA_Stampa.aspx"

    '        'If Pagina <> "" Then

    '        '    ' Decommentare per attivare lo scadenziario
    '        '    UrlTarget = Pagina &
    '        '                "?o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Lettura), AgroKey_EncoderDecoder, objParametri_Server) +
    '        '                "&p=" + Stringa_Codifica(CStr(PC_Dettagli_PIVA), AgroKey_EncoderDecoder, objParametri_Server) +
    '        '                "&q=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
    '        '                "&r=" + Stringa_Codifica(CStr(Regolamento_Cod), AgroKey_EncoderDecoder, objParametri_Server) +
    '        '                "&tipo=" + Stringa_Codifica(CStr(Regolamento_Tipo), AgroKey_EncoderDecoder, objParametri_Server)

    '        '    UrlTarget = ApriFiltroStampaScadenza(UrlTarget, PC_Dettagli_PIVA, PC_Testata_Cod, Regolamento_Tipo, objParametri_Server)


    '        '    ret.RispostaStringa = UrlTarget

    '        'Else
    '        '    ret.RispostaOK = False
    '        '    ret.Errore = "Resources.PianoConcimazione_2017.NonPossibileStamparePianoSelezionato "
    '        'End If

    '    Catch ex As Exception
    '        ret.RispostaOK = False
    '        ret.Errore = "Errore durante l'apertura della pagina di Stampa Registro"
    '    End Try

    '    Return ret

    'End Function


#End Region

    '  Galassi, 27/02/2017 17.33.37: '  Marco Grilli, 11/04/2016 15:57:50: Copiato dal GIS di Vanni, il codice non mi piace come è scritto, ma devo fare copia-incolla... :-P
    Private Shared Function Gestione_Ricetta_GeneraXMl(ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                       ByVal XmlDoc As System.Xml.XmlDocument,
                                                       ByRef Xml_Risultato As System.Xml.XmlElement,
                                                       ByRef StrNodiVariabili As String,
                                                       ByRef StrNodo As String,
                                                       ByRef Piva As String,
                                                       ByRef Num_Checked As Integer,
                                                       ByVal ListOFobjImpianto As List(Of AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo),
                                                       ByRef rag_soc As String,
                                                       ByRef sa_nome As String,
                                                       ByRef f As Boolean
                                                       ) As Integer

        Dim leggiDati As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ragsocFrompiva As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim saNomeFromPivaSaCod As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        'si suppongono specie omogenee
        Dim Veg_Cod As Integer
        For Each objImpianto As AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo In ListOFobjImpianto

            Dim dtLeggiDescrizioni As DataTable =
                leggiDati.Leggi_DescrizioniImpianti2(
                objImpianto.Piva,
                objImpianto.Sa_Cod,
                objImpianto.Appezza,
                objImpianto.Id_reg,
                AGRODATAINIZIO,
                AGRODATAFINE,
                "",
                "",
                ObjParametri_Server
            )

            Dim dtLeggiDati As DataTable =
                leggiDati.Leggi(
                objImpianto.Piva,
                objImpianto.Sa_Cod,
                objImpianto.Appezza,
                objImpianto.Id_reg,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                ObjParametri_Server
            )

            'leggiVegCodDatoImpianto
            objImpianto.Veg_Cod = leggiDati.VegCod_from_PivaSaCodAppezzaIdimp(
                objImpianto.Piva,
                objImpianto.Sa_Cod,
                objImpianto.Appezza,
                objImpianto.Id_reg,
                "",
                "",
                ObjParametri_Server
            )

            If Not f Then
                Piva = objImpianto.Piva
                rag_soc = ragsocFrompiva.RagSoc_from_Piva(objImpianto.Piva, ObjParametri_Server)
                sa_nome = saNomeFromPivaSaCod.SaNome_from_SaCod(objImpianto.Piva, objImpianto.Sa_Cod, ObjParametri_Server)
                f = True
            End If

            If (objImpianto.Veg_Cod <> 0) Then

                Xml_Risultato = XmlDoc.CreateElement("Risultato")

                Xml_Risultato.SetAttribute("piva", objImpianto.Piva)
                Xml_Risultato.SetAttribute("rag_soc", rag_soc)
                Xml_Risultato.SetAttribute("sa_cod", objImpianto.Sa_Cod)
                Xml_Risultato.SetAttribute("sa_nome", sa_nome)
                Xml_Risultato.SetAttribute("campo_cod", objImpianto.campo_cod)
                Xml_Risultato.SetAttribute("appezza", objImpianto.Appezza)
                Xml_Risultato.SetAttribute("app_nome", objImpianto.app_nome)
                Xml_Risultato.SetAttribute("id_reg", objImpianto.Id_reg)
                Xml_Risultato.SetAttribute("veg_cod", objImpianto.Veg_Cod)
                Xml_Risultato.SetAttribute("cul_cod", objImpianto.Cul_Cod)

                'Aggiungo i valori massimi di NPK per lo specifico impianto
                Xml_Risultato.SetAttribute("max_n", objImpianto.N)
                Xml_Risultato.SetAttribute("max_p", objImpianto.P)
                Xml_Risultato.SetAttribute("max_k", objImpianto.K)

                Veg_Cod = objImpianto.Veg_Cod

                Xml_Risultato.SetAttribute("lotto", objImpianto.Progetto_Nome)
                Xml_Risultato.SetAttribute("descrizione", dtLeggiDescrizioni.Rows(0)("Campo_des") & " - " & dtLeggiDescrizioni.Rows(0)("App_nome"))
                Xml_Risultato.SetAttribute("sup_imp", dtLeggiDescrizioni.Rows(0)("sup_imp"))
                Xml_Risultato.SetAttribute("grfi_cod", dtLeggiDati.Rows(0)("grfi_cod"))
                Xml_Risultato.SetAttribute("finanziamento", dtLeggiDati.Rows(0)("finanziamento"))
                Xml_Risultato.SetAttribute("regolamento", dtLeggiDati.Rows(0)("regolamento"))
                Xml_Risultato.SetAttribute("cop_cod", IIf(IsDBNull(dtLeggiDati.Rows(0)("cop_cod")), 0, dtLeggiDati.Rows(0)("cop_cod")))

                Xml_Risultato.SetAttribute("validita_inizio", AGRODATAINIZIO)
                Xml_Risultato.SetAttribute("validita_fine", AGRODATAFINE)

                StrNodo = Xml_Risultato.OuterXml

                Num_Checked += 1

                'Inserisco l'XML nella stringa complessiva
                StrNodiVariabili = StrNodiVariabili & StrNodo

            End If
        Next

        Return Veg_Cod

    End Function

    Private Sub PianoConcimazione_MenuBS_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Master_Concimaz = CType(Page.Master, MasterConcimazione)
        Master_Concimaz.flag_MostraBtnCambiaImpresa = True
        Master_Concimaz.flag_MostraBtnIndietro = True
        Master_Concimaz.Lbl_Titolo.Text = Resources.PianoConcimazione_2017.PianoConcimazione
        AddHandler Master_Concimaz.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler Master_Concimaz.ImgBtnFiltro.Click, AddressOf Me.FiltraAziende
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("../Autenticazione/Autenticazione.aspx")
        End If

        permessi = New PermessiUtente()

        If Not IsNothing(Request.QueryString("m")) Then
            tipologiaPagina = Stringa_Decodifica(Request.QueryString("m"), AgroKey_EncoderDecoder, Server)
        Else
            tipologiaPagina = enum_PUARegolamenti_Tipo.PianoComcimazione
        End If

        objParametri_Concimazione = New ParametriConcimazione_2017
        objParametri_Concimazione.Leggi()

        If tipologiaPagina = enum_PUARegolamenti_Tipo.PUA Then
            objParametri_Pua = New ParametriPUA
            objParametri_Pua.Leggi()
        End If

        '14/10/21 Anna: Piano Nutrizionale TO DO
        'If tipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
        '    objParametri_PianoNutrizionale = New PianoNutrizionale
        '    objParametri_PianoNutrizionale.Leggi()
        'End If

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'end_date.Value = "31/12/" & Date.Now.Year.ToString
        'start_date.Value = "01/01/" & Date.Now.Year.ToString

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DataInizio, DataFine As Date
        objImpost.AnnataAgraria(Date.Today, DataInizio, DataFine, objParametri_Utenti)

        'visualizzo gli ultimi 2 anni
        Txt_ValiditaInizio.Text = DateAdd(DateInterval.Year, -1, DataInizio)
        Txt_ValiditaFine.Text = DataFine

        If tipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale OrElse tipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF Then
            mostraPrivati_xPianoNutrizionale = get_mostraPrivati_xPianoNutrizionale(objParametri_Server.PivaSuperUser, filtroRegolamentiPrivati)
        End If
        hd_filtroRegolamentiPrivati.Value = filtroRegolamentiPrivati
        'start_date.Value = DateAdd(DateInterval.Year, -1, DataInizio)
        'end_date.Value = DataFine


        'InizializzaScriptClient()
        Dim usaFiltroRicercaNG As Boolean

        Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objConcimazione.Leggi()

        If Not Page.IsPostBack Then
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            usaFiltroRicercaNG = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
            hd_usaFiltroRicercaNG.Value = usaFiltroRicercaNG
            hd_CurrentPiva.Value = objConcimazione.Piva
        End If

    End Sub


    Public Shared Function get_mostraPrivati_xPianoNutrizionale(Piva_Superuser As String, ByRef filtroRegolamentiPrivati As String) As String

        Dim Dt As New DataTable
        Dim i As Integer = 0
        Dim listaRegolamenti As New List(Of Integer)

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        objParametriIngresso.strFiltro = ""
        objParametriIngresso.Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale
        objParametriIngresso.TipoMetodo = enum_PUARegolamenti_Tipo.PianoNutrizionale
        objParametriIngresso.VisualizzaPrivati = True
        objParametriIngresso.Piva_Superuser = Piva_Superuser
        objParametriIngresso.strOrdinamento = ""
        objParametriIngresso.DataInizio = AGRODATAINIZIO
        objParametriIngresso.DataFine = AGRODATAFINE

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        If objParametriUscita.ListaRegolamenti.Count > 0 Then
            For Each regolamento In objParametriUscita.ListaRegolamenti
                listaRegolamenti.Add(regolamento.Codice)
            Next

            filtroRegolamentiPrivati = String.Join(",", listaRegolamenti)

            Return "true"
        End If

        Return "false"

    End Function

    Private Shared Function Link_Schede(ByVal SingolaAzienda As Boolean, ByVal SingolaColtura As Boolean,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo) As String

        Dim link As String = ""

        Select Case Regolamento_Tipo

            Case enum_PUARegolamenti_Tipo.PUA

                Dim objPua As New AgronicaCoreGestioneRichieste.ParametriPUA
                objPua.Leggi()
                If Not IsNothing(objPua.Piva) AndAlso objPua.Piva <> "" Then
                    link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&p=" + Stringa_Codifica(CStr(objPua.Piva), AgroKey_EncoderDecoder, objParametri_Server)

                Else

                    objPua.Tipo_Pua = enum_PUA_Tipo.Semplificato
                    objPua.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    objPua.Salva()

                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objPua.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti),
                                                 link)

                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                    '                               objFiltrino)
                End If

            Case enum_PUARegolamenti_Tipo.PianoComcimazione

                Select Case SingolaAzienda

                    Case True

                        Select Case SingolaColtura

                             'accedo al piano singolo

                            Case True

                                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                                objConcimazione.Leggi()
                                If Not IsNothing(objConcimazione.Piva) AndAlso objConcimazione.Piva <> "" Then
                                    link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&p=" + Stringa_Codifica(CStr(objConcimazione.Piva), AgroKey_EncoderDecoder, objParametri_Server)
                                Else

                                    objConcimazione.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
                                    objConcimazione.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                                    objConcimazione.Salva()

                                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objConcimazione.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.PCB_Inserimento),
                                                 link)

                                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PCB_Inserimento
                                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    '                               objFiltrino)
                                End If

                            Case Else

                                'accedo al massivo non aggregando

                                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                                objConcimazione.Leggi()
                                objConcimazione.ListaImpianti = Nothing

                                If Not IsNothing(objConcimazione.Piva) AndAlso objConcimazione.Piva <> "" Then
                                    link = "PC_Bilancio\PCB_InserimentoMultiplo.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&p=" + Stringa_Codifica(CStr(objConcimazione.Piva), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&aggrega=" + Stringa_Codifica(False, AgroKey_EncoderDecoder, objParametri_Server)
                                Else

                                    objConcimazione.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
                                    objConcimazione.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                                    objConcimazione.Salva()

                                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objConcimazione.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo),
                                                 link)

                                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo
                                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    '                               objFiltrino)
                                End If

#Region "commentato"
                                'Dim dtimpianti As DataTable = HttpContext.Current.Session("dt_x_export_impianto")


                                'For Each chiave As String In richiesta.chiavi

                                '    'Dim objImpianto As New AgronicaCoreGestioneRichieste.Impianto
                                '    'Dim objImpianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                                '    Dim objImpianto As New AgronicaCoreGestioneRichieste.Impianto
                                '    Dim piva As String = chiave.Split("_")(0)
                                '    Dim sa_cod As String = chiave.Split("_")(1)
                                '    Dim appezza As String = chiave.Split("_")(2)
                                '    Dim id_reg As String = chiave.Split("_")(3)
                                '    Dim veg_cod As String = chiave.Split("_")(4)
                                '    Dim progetto_cod As String = chiave.Split("_")(5)


                                '    Dim objAImp_r As New AgronicaCoreAnagrafeDAL.Imprese_Read
                                '    objImpianto.Piva = piva
                                '    objImpianto.Sa_Cod = sa_cod
                                '    objImpianto.Appezza = appezza
                                '    objImpianto.Id_Reg = id_reg
                                '    objImpianto.Veg_Cod = veg_cod
                                '    objImpianto.Progetto_Cod = progetto_cod

                                '    Dim dr() As DataRow = dtimpianti.Select("chiave = '" & chiave & "'")

                                '    If Not dr Is Nothing AndAlso dr.Length > 0 Then
                                '        If IsDBNull(dr(0).Item("Rag_Soc")) Then
                                '            objImpianto.Rag_Soc = ""
                                '        Else
                                '            objImpianto.Rag_Soc = CStr(dr(0).Item("Rag_Soc"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("Sa_Nome")) Then
                                '            objImpianto.Sa_Nome = ""
                                '        Else
                                '            objImpianto.Sa_Nome = CStr(dr(0).Item("Sa_Nome"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("App_Nome")) Then
                                '            objImpianto.App_Nome = ""
                                '        Else
                                '            objImpianto.App_Nome = CStr(dr(0).Item("App_Nome"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("veg_des")) Then
                                '            objImpianto.Veg_Des = ""
                                '        Else
                                '            objImpianto.Veg_Des = CStr(dr(0).Item("veg_des"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("cul_cod")) Then
                                '            objImpianto.Cul_Cod = ""
                                '        Else
                                '            objImpianto.Cul_Cod = CStr(dr(0).Item("cul_cod"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("cul_des")) Then
                                '            objImpianto.Cul_Des = ""
                                '        Else
                                '            objImpianto.Cul_Des = CStr(dr(0).Item("cul_des"))
                                '        End If
                                '        objImpianto.Sup_Imp = CStr(dr(0).Item("sup_imp"))
                                '        If IsDBNull(dr(0).Item("Validita_Inizio")) Then
                                '            objImpianto.Validita_Inizio = ""
                                '        Else
                                '            objImpianto.Validita_Inizio = CStr(dr(0).Item("Validita_Inizio"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("Validita_fine")) Then
                                '            objImpianto.Validita_Fine = ""
                                '        Else
                                '            objImpianto.Validita_Fine = CStr(dr(0).Item("Validita_fine"))
                                '        End If
                                '        If IsDBNull(dr(0).Item("Grfi_Cod")) Then
                                '            objImpianto.Grfi_Cod = ""
                                '        Else
                                '            objImpianto.Grfi_Cod = CStr(dr(0).Item("Grfi_Cod"))
                                '        End If
                                '    End If
                                '    'objParametriAgenda.Impianti.Add(objImpianto)
                                '    objParametriConcimazione_2017.AddList(objImpianto)

                                'Next

                                'objParametriConcimazione_2017.Pagina_Richiesta = enum_CodificaPagPianoConcimazione.BilancioMultiAziendale
                                'objParametriConcimazione_2017.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo

                                'Dim Link As String = "PianoConcimazione/PC_Bilancio/PCB_InserimentoMultiplo.aspx"

                                'Dim Link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(FiltroAzioni.Sito_Origine,
                                '                                 objParametriConcimazione_2017)

#End Region
                        End Select



                    Case Else


                        'accedo al massivo passando dal filtrone per poi aggregare per azienda, centro, coltura e finalita

                        Dim objPianoConcOLD As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                        objPianoConcOLD.Leggi()

                        Dim objPianoConc As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                        objPianoConc.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
                        objPianoConc.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                        objPianoConc.Piva = objPianoConcOLD.Piva
                        objPianoConc.Salva()

                        Dim objpFiltrone As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

                        objpFiltrone.Sito_Origine = Stringa_Codifica(
                                    Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    AgroKey_EncoderDecoder, objParametri_Server)
                        objpFiltrone.Pagina_Origine = Stringa_Codifica(
                                         enum_PaginePianoConcimazione_2017.MenuBS,
                                        AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.Sito_Destinazione = Stringa_Codifica(
                                Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                AgroKey_EncoderDecoder, objParametri_Server)


                        objpFiltrone.TipoFiltrone = Stringa_Codifica(
                               CStr(enum_TipoFiltrone.PianoConcimazione),
                               AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.Piva = Stringa_Codifica(
                               CStr(objPianoConcOLD.Piva),
                               AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.CodificaStampe = Stringa_Codifica(
                              "",
                               AgroKey_EncoderDecoder, objParametri_Server)

                        link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
                                               Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                               objpFiltrone)

                        link = AgronicaCoreUtility.Varie.aggiungiAQueryString(link, "FiltroNew", "1")

                End Select

            Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF

                '14/10/21 ANNA 
                Dim objPianoNutrizionale As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                objPianoNutrizionale.Leggi()
                If Not IsNothing(objPianoNutrizionale.Piva) AndAlso objPianoNutrizionale.Piva <> "" Then

                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        link = "PianoNutrizionale\PianoNutrizionale.aspx"
                    Else
                        link = "PianoNutrizionale\PianoNutrizionale_IBF.aspx"
                    End If

                    link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                        "&p=" + Stringa_Codifica(CStr(objPianoNutrizionale.Piva), AgroKey_EncoderDecoder, objParametri_Server)
                Else

                    objPianoNutrizionale.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
                    objPianoNutrizionale.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    objPianoNutrizionale.Salva()

                    Dim paginaPianoConcimazione As enum_PaginePianoConcimazione_2017
                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        paginaPianoConcimazione = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale
                    Else
                        paginaPianoConcimazione = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale_IBF
                    End If

                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objPianoNutrizionale.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(paginaPianoConcimazione),
                                                 link)

                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017

                    'If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                    '    objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale
                    'Else
                    '    objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale_IBF
                    'End If

                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objFiltrino)
                End If

        End Select

        Return link

    End Function

#Region "funzione COMMENTATA"
    'Private Shared Function Link_DoseStd(ByVal Singolo As Boolean, objParametri_Server As AgronicaCoreParametri) As String


    '    Dim link As String = ""

    '    Select Case Singolo

    '        Case True

    '            Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017

    '            objConcimazione.Leggi()
    '            If Not IsNothing(objConcimazione.Piva) AndAlso objConcimazione.Piva <> "" Then

    '                link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) + _
    '                                            "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) + _
    '                                            "&p=" + Stringa_Codifica(CStr(objConcimazione.Piva), AgroKey_EncoderDecoder, objParametri_Server)
    '            Else
    '                objConcimazione.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
    '                objConcimazione.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
    '                objConcimazione.Salva()

    '                Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
    '                objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
    '                objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PCS_Inserimento
    '                objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
    '                objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

    '                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, _
    '                                                       objFiltrino)
    '            End If

    '            'link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Schede), AgroKey_EncoderDecoder, objParametri_Server) +
    '            '                            "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server)

    '        Case Else

    '            Dim objpFiltrone As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

    '            objpFiltrone.Sito_Origine = Stringa_Codifica(
    '                                Enum_SiteRedirector.Sito_PianoConcimazione_2017,
    '                                AgroKey_EncoderDecoder, objParametri_Server)
    '            objpFiltrone.Pagina_Origine = Stringa_Codifica(
    '                             "",
    '                            AgroKey_EncoderDecoder, objParametri_Server)

    '            objpFiltrone.Sito_Destinazione = Stringa_Codifica(
    '                Enum_SiteRedirector.Sito_PianoConcimazione_2017,
    '                AgroKey_EncoderDecoder, objParametri_Server)

    '            objpFiltrone.Pagina_Destinazione = Stringa_Codifica(
    '                "PC_Semplificato/PCS_InserimentoMultiplo.aspx?Tipo=" + enum_PianoConcimazione_Tipo.Bilancio.ToString,
    '                AgroKey_EncoderDecoder, objParametri_Server)

    '            objpFiltrone.TipoFiltrone = Stringa_Codifica(
    '                           CStr(enum_TipoFiltrone.PianoConcimazione),
    '                           AgroKey_EncoderDecoder, objParametri_Server)

    '            objpFiltrone.Piva = Stringa_Codifica(
    '                           CStr(""),
    '                           AgroKey_EncoderDecoder, objParametri_Server)

    '            objpFiltrone.CodificaStampe = Stringa_Codifica(
    '                          "",
    '                           AgroKey_EncoderDecoder, objParametri_Server)

    '            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
    '                                           Enum_SiteRedirector.Sito_PianoConcimazione_2017,
    '                                           objpFiltrone)

    '    End Select

    '    Return link

    'End Function
#End Region

    Private Shared Function Link_Bilancio(ByVal SingolaAzienda As Boolean, ByVal SingolaColtura As Boolean,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo) As String

        Dim link As String = ""

        Select Case Regolamento_Tipo

            Case enum_PUARegolamenti_Tipo.PUA

                Dim objPua As New AgronicaCoreGestioneRichieste.ParametriPUA
                objPua.Leggi()
                If Not IsNothing(objPua.Piva) AndAlso objPua.Piva <> "" Then
                    link = "PUA\PUA_Dichiarazione_Effluenti.aspx?tipo=" + Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&p=" + Stringa_Codifica(CStr(objPua.Piva), AgroKey_EncoderDecoder, objParametri_Server)

                Else

                    objPua.Tipo_Pua = enum_PUA_Tipo.Completo
                    objPua.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    objPua.Salva()

                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objPua.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti),
                                                 link)

                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                    '                               objFiltrino)
                End If

            Case enum_PUARegolamenti_Tipo.PianoComcimazione

                Select Case SingolaAzienda

                    Case True

                        Select Case SingolaColtura

                             'accedo al piano singolo

                            Case True

                                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                                objConcimazione.Leggi()
                                If Not IsNothing(objConcimazione.Piva) AndAlso objConcimazione.Piva <> "" Then
                                    link = "PC_Bilancio\PCB_Inserimento.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                                                "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                                                "&p=" + Stringa_Codifica(CStr(objConcimazione.Piva), AgroKey_EncoderDecoder, objParametri_Server)
                                Else
                                    objConcimazione.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Bilancio
                                    objConcimazione.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                                    objConcimazione.Salva()

                                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objConcimazione.Piva,
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                 CInt(enum_PaginePianoConcimazione_2017.PCB_Inserimento),
                                                 link)

                                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PCB_Inserimento
                                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    '                                       objFiltrino)
                                End If



                            Case Else

                                'accedo al massivo non aggregando

                                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                                objConcimazione.Leggi()
                                objConcimazione.ListaImpianti = Nothing

                                If Not IsNothing(objConcimazione.Piva) AndAlso objConcimazione.Piva <> "" Then
                                    link = "PC_Bilancio\PCB_InserimentoMultiplo.aspx?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&p=" + Stringa_Codifica(CStr(objConcimazione.Piva), AgroKey_EncoderDecoder, objParametri_Server) +
                                                        "&aggrega=" + Stringa_Codifica(False, AgroKey_EncoderDecoder, objParametri_Server)
                                Else

                                    objConcimazione.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Bilancio
                                    objConcimazione.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                                    objConcimazione.Salva()

                                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objConcimazione.Piva,
                                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                                 CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                                 Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                                 CInt(enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo),
                                                                 link)

                                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
                                    'objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo
                                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    '                               objFiltrino)
                                End If

                        End Select



                    Case Else

                        Dim objPianoConcOLD As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                        objPianoConcOLD.Leggi()

                        Dim objPianoConc As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                        objPianoConc.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Schede
                        objPianoConc.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                        objPianoConc.Piva = objPianoConcOLD.Piva
                        objPianoConc.Salva()

                        Dim objpFiltrone As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

                        objpFiltrone.Sito_Origine = Stringa_Codifica(
                                                    Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                    AgroKey_EncoderDecoder, objParametri_Server)
                        objpFiltrone.Pagina_Origine = Stringa_Codifica(
                                                 enum_PaginePianoConcimazione_2017.MenuBS,
                                                AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.Sito_Destinazione = Stringa_Codifica(
                                    Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                    AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.TipoFiltrone = Stringa_Codifica(
                                               CStr(enum_TipoFiltrone.PianoConcimazione),
                                               AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.Piva = Stringa_Codifica(
                                               CStr(objPianoConcOLD.Piva),
                                               AgroKey_EncoderDecoder, objParametri_Server)

                        objpFiltrone.CodificaStampe = Stringa_Codifica(
                                              "",
                                               AgroKey_EncoderDecoder, objParametri_Server)

                        link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
                                                               Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                               objpFiltrone)

                        link = AgronicaCoreUtility.Varie.aggiungiAQueryString(link, "FiltroNew", "1")

                End Select

            Case enum_PUARegolamenti_Tipo.PianoNutrizionale, enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF
                '14/10/21 ANNA 
                Dim objPianoNutrizionale As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                objPianoNutrizionale.Leggi()
                If Not IsNothing(objPianoNutrizionale.Piva) AndAlso objPianoNutrizionale.Piva <> "" Then
                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        link = "PianoNutrizionale\PianoNutrizionale.aspx"
                    Else
                        link = "PianoNutrizionale\PianoNutrizionale_IBF.aspx"
                    End If

                    link += "?tipo=" + Stringa_Codifica(CStr(enum_PianoConcimazione_Tipo.Bilancio), AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&o=" + Stringa_Codifica(CStr(enum_TipoOperazioneDB.Scrittura), AgroKey_EncoderDecoder, objParametri_Server) +
                                        "&p=" + Stringa_Codifica(CStr(objPianoNutrizionale.Piva), AgroKey_EncoderDecoder, objParametri_Server)
                Else

                    objPianoNutrizionale.Tipo_Concimazione = enum_PianoConcimazione_Tipo.Bilancio
                    objPianoNutrizionale.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    objPianoNutrizionale.Salva()

                    Dim paginePianoConcimazione As enum_PaginePianoConcimazione_2017

                    If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                        paginePianoConcimazione = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale
                    Else
                        paginePianoConcimazione = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale_IBF
                    End If

                    RedirectGestione.GetLinkPerRedirectVersoFiltrinoImpreseOFiltroneNG(objPianoNutrizionale.Piva,
                                                Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                CInt(enum_PaginePianoConcimazione_2017.MenuBS),
                                                Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                                                CInt(paginePianoConcimazione),
                                                link)

                    'Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                    'objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017

                    'If Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale Then
                    '    objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale
                    'Else
                    '    objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale_IBF
                    'End If

                    'objFiltrino.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                    'objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese

                    'link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objFiltrino)
                End If

        End Select

        Return link

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca

            Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                            .PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS,
                            .Piva = piva,
                            .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Impianti},
                            .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                            .SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                            .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Impianto)
            }

            r.RispostaStringa = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaEntitaDaChiavi(chiavi As List(Of String)) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametriConcimazione_2017 As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
            objParametriConcimazione_2017.Leggi()
            objParametriConcimazione_2017.ListaImpianti = Nothing

            Dim filtroProgetti As New List(Of Integer)
            For Each chiave In chiavi
                filtroProgetti.Add(chiave.Split("_")(5))
            Next

            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim dtimpianti = objRegImpianti.Leggi_x_ParametriAgenda(filtroProgetti, objParametri_Server, False)

            For Each chiave As String In chiavi
                Dim objImpianto As New AgronicaCoreGestioneRichieste.Impianto
                Dim piva As String = chiave.Split("_")(0)
                Dim sa_cod As String = chiave.Split("_")(1)
                Dim appezza As String = chiave.Split("_")(2)
                Dim id_reg As String = chiave.Split("_")(3)
                Dim veg_cod As String = chiave.Split("_")(4)
                Dim progetto_cod As String = chiave.Split("_")(5)

                objImpianto.Piva = piva
                objImpianto.Sa_Cod = sa_cod
                objImpianto.Appezza = appezza
                objImpianto.Id_Reg = id_reg
                objImpianto.Veg_Cod = veg_cod
                objImpianto.Progetto_Cod = progetto_cod

                Dim dr() As DataRow = dtimpianti.Select("progetto_cod = " & progetto_cod)

                If Not dr Is Nothing AndAlso dr.Length > 0 Then
                    If IsDBNull(dr(0).Item("Rag_Soc")) Then
                        objImpianto.Rag_Soc = ""
                    Else
                        objImpianto.Rag_Soc = CStr(dr(0).Item("Rag_Soc"))
                    End If
                    If IsDBNull(dr(0).Item("Sa_Nome")) Then
                        objImpianto.Sa_Nome = ""
                    Else
                        objImpianto.Sa_Nome = CStr(dr(0).Item("Sa_Nome"))
                    End If
                    If IsDBNull(dr(0).Item("App_Nome")) Then
                        objImpianto.App_Nome = ""
                    Else
                        objImpianto.App_Nome = CStr(dr(0).Item("App_Nome"))
                    End If
                    If IsDBNull(dr(0).Item("veg_des")) Then
                        objImpianto.Veg_Des = ""
                    Else
                        objImpianto.Veg_Des = CStr(dr(0).Item("veg_des"))
                    End If
                    If IsDBNull(dr(0).Item("cul_cod")) Then
                        objImpianto.Cul_Cod = 0
                    Else
                        objImpianto.Cul_Cod = CStr(dr(0).Item("cul_cod"))
                    End If
                    If IsDBNull(dr(0).Item("cul_des")) Then
                        objImpianto.Cul_Des = ""
                    Else
                        objImpianto.Cul_Des = CStr(dr(0).Item("cul_des"))
                    End If
                    objImpianto.Sup_Imp = CStr(dr(0).Item("sup_imp"))
                    If IsDBNull(dr(0).Item("Validita_Inizio")) Then
                        objImpianto.Validita_Inizio = ""
                    Else
                        objImpianto.Validita_Inizio = CStr(dr(0).Item("Validita_Inizio"))
                    End If
                    If IsDBNull(dr(0).Item("Validita_fine")) Then
                        objImpianto.Validita_Fine = ""
                    Else
                        objImpianto.Validita_Fine = CStr(dr(0).Item("Validita_fine"))
                    End If
                    If IsDBNull(dr(0).Item("Grfi_Cod")) Then
                        objImpianto.Grfi_Cod = 0
                    Else
                        objImpianto.Grfi_Cod = CInt(dr(0).Item("Grfi_Cod"))
                    End If
                End If

                objParametriConcimazione_2017.AddList(objImpianto)

                objParametriConcimazione_2017.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.PCB_InserimentoMultiplo

                Dim Link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(Enum_SiteRedirector.Sito_PianoConcimazione_2017,
                    objParametriConcimazione_2017)

                r.RispostaStringa = Link
            Next

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Sub AnnullaTutto(sender As Object, e As ImageClickEventArgs)
        Dim TargetURL As String = ""

        Select Case HttpContext.Current.Session("Sito_Origine")
            Case Enum_SiteRedirector.Sito_GiasOnline
                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.MenuSupportoDecisioni

                TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objGiasOnline)


            Case Enum_SiteRedirector.Sito_PianoConcimazione

                TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_PianoConcimazione, enum_PagineGiasOnline_2010.Menu, 0, objParametri_Concimazione.Piva, "", "", 0, "")

            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objParametri_Concimazione)

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                Dim objParametri_Agenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                objParametri_Agenda.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS
                objParametri_Agenda.PaginaRichiesta = objParametri_Concimazione.Pagina_SitoOrigine
                objParametri_Agenda.Piva = objParametri_Concimazione.Piva
                TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objParametri_Agenda)

            Case Enum_SiteRedirector.GiasNG
                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametri_Concimazione.Piva,
                                                                                      Enum_SiteRedirector.GiasNG,
                                                                                      objParametri_Concimazione.Pagina_SitoOrigine,
                                                                                      TargetURL,
                                                                                      objParametri_Server)

                'SITO ONLINE DA DOVE ARRIVA IL PC
            Case Else
                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.MenuSupportoDecisioni

                TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_PianoConcimazione_2017, objGiasOnline)

        End Select
        Response.Redirect(TargetURL)
    End Sub

    Private Sub FiltraAziende(sender As Object, e As ImageClickEventArgs)

        Dim TargetURL As String = ""

        Dim objFiltrino As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objFiltrino.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_PianoConcimazione_2017
        objFiltrino.PaginaDestinazioneFiltrino = enum_PaginePianoConcimazione_2017.MenuBS
        objFiltrino.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese
        objFiltrino.QueryStringFiltrino = "&multi="

        Dim sitoOrigine As Enum_SiteRedirector
        If IsNumeric(Session("sito_origine")) AndAlso Session("sito_origine") = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
            'Non controllo la chiave del menu nuovo o vecchio perché sul vecchio non esiste il link
            sitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objFiltrino.PaginaProvenienza = enum_PagineAgenda_2010.Menu_BS
        Else
            sitoOrigine = Enum_SiteRedirector.Sito_GiasOnline
            objFiltrino.PaginaProvenienza = enum_PagineGiasOnline.MenuSupportoDecisioni
        End If

        TargetURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(sitoOrigine, objFiltrino)

        Response.Redirect(TargetURL)

    End Sub

    Private Shared Function ApriFiltroStampaScadenza(ByVal UrlPaginaStampa As String,
                                                     ByVal Piva As String, ByVal PC_Testata_Cod As Integer,
                                                     ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo,
                                                     ByRef Server As AgronicaCoreParametri) As String

        Dim UrlTarget As String

        Select Case Regolamento_Tipo
            Case enum_PUARegolamenti_Tipo.PUA
                UrlTarget = "PC_Stampe/Filtro_StampaScadenza.aspx" &
            "?p=" + Stringa_Codifica(CStr(UrlPaginaStampa), AgroKey_EncoderDecoder, Server) +
            "&a=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Attivita.Gest_PUA_2), AgroKey_EncoderDecoder, Server) +
            "&o=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Operazione.Lettura), AgroKey_EncoderDecoder, Server) &
            "&piva=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) &
            "&pc=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, Server)
            Case Else
                UrlTarget = "PC_Stampe/Filtro_StampaScadenza.aspx" &
            "?p=" + Stringa_Codifica(CStr(UrlPaginaStampa), AgroKey_EncoderDecoder, Server) +
            "&a=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione), AgroKey_EncoderDecoder, Server) +
            "&o=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Operazione.Lettura), AgroKey_EncoderDecoder, Server) &
            "&piva=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) &
            "&pc=" + Stringa_Codifica(CStr(PC_Testata_Cod), AgroKey_EncoderDecoder, Server)
        End Select

        Return UrlTarget

    End Function

    '#################################################
    '################### RICETTE #####################
    '#################################################
    Private Shared Function ScriviNuovaRicettaPUA(ByVal piva As String,
                                                  ByVal puaCod As Integer,
                                                  ByVal puaDes As String,
                                                  ByVal dataReg As Date,
                                                  ByVal progressivoGias As Integer,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  ByRef OUTPUT_Ricetta_Cod As Integer)

        Dim xRisp As Boolean = False

        Dim xmlStr As String = CreaXmlRicettaPUA(piva, puaCod, puaDes, dataReg, progressivoGias)

        Dim objW As New AgronicaCoreContabBIZ.Ricette_W
        xRisp = objW.Ricetta_Scrivi(xmlStr, OUTPUT_Ricetta_Cod, objParametriServer)

        Return xRisp

    End Function

    Private Shared Function CreaXmlRicettaPUA(ByVal piva As String,
                                              ByVal puaCod As Integer,
                                              ByVal puaDes As String,
                                              ByVal dataReg As Date,
                                              ByVal progressivoGias As Integer
                                              ) As String

        Dim strRet As String

        Dim TipoOperazioneDb As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, progressivoGias)

        Dim XmlDoc As New XmlDocument

        Dim XmlDatiRicetta As XmlElement = XmlDoc.CreateElement("DatiRicetta")

        '----- < RICETTA > -----
        Dim XmlRicetta As XmlElement = XmlDoc.CreateElement("Ricetta")
        XmlDatiRicetta.AppendChild(XmlRicetta)

        Dim des As String = Resources.PianoConcimazione_2017.PianoDistribuzione & " (" & puaDes & ")"

        With XmlRicetta
            .SetAttribute("TipoOperazioneDB", TipoOperazioneDb)
            .SetAttribute("ricetta_cod", 0)
            .SetAttribute("ricetta_numero", CStr(puaCod) & "_1")
            .SetAttribute("piva", piva)
            .SetAttribute("sa_cod", 0)
            .SetAttribute("tipo_ricetta", enum_TipoRicetta.PianoDistribuzionePua)
            .SetAttribute("ricetta_des", des)
            .SetAttribute("ricetta_des_long", des)
            .SetAttribute("veg_cod", 0)
            .SetAttribute("note", "")
            .SetAttribute("basecode", BaseCode.ToString)
            .SetAttribute("topcode", TopCode.ToString)
            .SetAttribute("programmazione_cod", puaCod)
            .SetAttribute("validita_inizio", dataReg)
            .SetAttribute("validita_fine", AGRODATAFINE)
        End With

        XmlDoc.AppendChild(XmlDatiRicetta)

        strRet = XmlDoc.InnerXml

        Return strRet

    End Function


    '######################################################
    '################# BLOCCA SBLOCCA #####################
    '######################################################
    <WebMethod(EnableSession:=True)>
    Public Shared Function BloccaSbloccaPC(ByVal arrPC As String(), ByVal blocca As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()


            Dim objPianoConcimazione As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_W
            For Each PCT In arrPC
                If blocca = 0 Then
                    objPianoConcimazione.BloccaSblocca(PCT, blocca, AGRODATAINIZIO, "", objParametri_Server)
                Else
                    objPianoConcimazione.BloccaSblocca(PCT, blocca, Date.Now, objParametri_Server.UsernameOperazione, objParametri_Server)
                End If
            Next


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function BloccaSbloccaPUA(ByVal arrPUA As String(), ByVal blocca As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim pbjPua As New AgronicaCorePUA_DAL.PUA_Testata_W
            For Each PUAT In arrPUA
                Dim Pua_Cod As Integer
                Dim Regolamento_Cod As Integer
                Pua_Cod = PUAT.Split("_")(0)
                Regolamento_Cod = PUAT.Split("_")(1)
                If blocca = 0 Then
                    pbjPua.BloccaSblocca(Pua_Cod, Regolamento_Cod, blocca, AGRODATAINIZIO, "", objParametri_Server)
                Else
                    pbjPua.BloccaSblocca(Pua_Cod, Regolamento_Cod, blocca, Date.Now, objParametri_Server.UsernameOperazione, objParametri_Server)
                End If
            Next


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

End Class