Imports System.Web
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreModelsSTD.utente

Public Class warmUpGiasNG_Response
    Public objP_super_server As AgronicaCoreParametri_NG
    Public objP_server As AgronicaCoreParametri_NG
    Public objP_utenti As AgronicaCoreParametri_NG
    Public objParametri_Agenda As Parametri_ObjParametriAgenda_NG
    Public utente As Utente
    Public link As AgronicaLink_NG
    Public VariabiliInSessione As VariabiliInSessione_NG
    Public impresa_impostazioni As List(Of Imprese_Impostazioni)
    Public Lingua_Cod As Integer
End Class

Public Class ParametriAggiuntivi_QueryString
    Public key As String
    Public value As String
    Public codifica As Boolean
End Class


Public Class GiasNG_Redirect


    Public Function warmUpGiasNG(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByVal unid As String) As warmUpGiasNG_Response

        Dim warmResponse As New warmUpGiasNG_Response

        warmResponse.objP_super_server = from_AgronicaCoreParametri_To_NG(objParametri_Super_Server)
        warmResponse.objP_server = from_AgronicaCoreParametri_To_NG(objParametri_Server)
        warmResponse.objP_utenti = from_AgronicaCoreParametri_To_NG(objParametri_Utenti)

        ' Chiamo la verifica sulla validità dei permessi utente e della licenza gias (la stessa che viene chiamata nella 
        ' AgroMasterPage dell'Agenda 
        Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente
        Dim validitaUtentePermessiLicenza = auK.Verifica_Validita_Permessi_E_Chiave_Licenza(objParametri_Utenti)

        warmResponse.utente = Get_Utente_Permessi(objParametri_Utenti)
        If Not IsNothing(warmResponse.utente) Then
            warmResponse.utente.ValiditaUtentePermessiLicenza = validitaUtentePermessiLicenza
        End If

        Dim objVarie As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
        Dim strParametri = objVarie.LeggiParametriGias(unid, True, objParametri_Server)

        Dim XmlDoc = New System.Xml.XmlDocument
        Dim link As New AgronicaLink_NG

        Dim linkGiasBase As String = ""

        GiasBaseHelper.Setta_Link_GiasBase(objParametri_Server, linkGiasBase)
        link.linkGiasBase = linkGiasBase
        link.linkAgronicaCoreAPI = "http://localhost/AgronicaCoreAPI/"

        If strParametri <> "" Then

            XmlDoc.LoadXml(strParametri)

            If XmlDoc.HasChildNodes Then

                Dim XML_Parametri As System.Xml.XmlElement
                'identifico il nodo di destinazione
                XML_Parametri = XmlDoc.SelectSingleNode("Parametri")
                If XML_Parametri IsNot Nothing Then

                    Dim Sito_Origine As Integer
                    Dim Sito_Destinazione As String
                    Sito_Origine = CInt(XML_Parametri.GetAttribute("sito_origine"))
                    Sito_Destinazione = XML_Parametri.GetAttribute("sito_destinazione")
                    Dim objP_Agenda As New Parametri_ObjParametriAgenda_NG

                    Dim XML_AgroWebConfig As System.Xml.XmlElement
                    'identifico il nodo di destinazione
                    XML_AgroWebConfig = XML_Parametri.SelectSingleNode("Parametri_ObjParametriAgenda_NG")
                    objP_Agenda = CaricaParametriAgenda_NG(XML_AgroWebConfig, Sito_Origine, objParametri_Server)

                    XML_AgroWebConfig = XML_Parametri.SelectSingleNode("VariabiliSessione")
                    Dim _variabili_in_sessione = CaricaVariabiliInSessione(XML_AgroWebConfig)

                    warmResponse.objParametri_Agenda = objP_Agenda

                    If objP_Agenda.Piva <> "" Then
                        warmResponse.impresa_impostazioni = Get_Imprese_Impostazioni(objP_Agenda.Piva, If(objP_Agenda.Sa_Cod = 0, SACOD_NOFILTRO, objP_Agenda.Sa_Cod),
                                                                                     objParametri_Server)
                    End If

                    warmResponse.Lingua_Cod = objParametri_Utenti.Lingua_Cod

                    warmResponse.VariabiliInSessione = _variabili_in_sessione
                End If

            End If

        End If

        warmResponse.link = link

        Return warmResponse

    End Function


    Public Function from_AgronicaCoreParametri_To_NG(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreParametri_NG
        Dim r As New AgronicaCoreParametri_NG

        r.PivaSuperUser = objParametri.PivaSuperUser
        r.SuperUserUsername = objParametri.SuperUserUsername
        r.UsernameOperazione = objParametri.UsernameOperazione
        r.UtenteUsername = objParametri.UtenteUsername
        r.UtenteCodFiscale = objParametri.UtenteCodFiscale
        r.FinestraTemporaleInizio = objParametri.FinestraTemporaleInizio
        r.FinestraTemporaleFine = objParametri.FinestraTemporaleFine
        r.Lingua_Cod = objParametri.Lingua_Cod

        Return r
    End Function

    Public Function Get_Utente_Permessi(objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Utente
        Dim up As New Utente

        Dim objUtenti_Dettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim objUtenti_Permessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim objUtenti_Impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        up.Username = objParametri_Utenti.UtenteUsername
        If up.Username <> "" Then
            Dim dt_dettagli = objUtenti_Dettagli.Leggi(up.Username, 5,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                   "", "", objParametri_Utenti)

            up.Nome = dt_dettagli.Rows(0)("Nome")
            up.Cognome = dt_dettagli.Rows(0)("Cognome")
            up.Rag_Soc = dt_dettagli.Rows(0)("Rag_Soc")
            up.Cod_Fisc = dt_dettagli.Rows(0)("CodFisc")
            up.UsernameCommerciale = dt_dettagli.Rows(0)("UsernameCommerciale")

            Dim dt_permessi = objUtenti_Permessi.LeggiCached(up.Username, 5, 0, 9999, 0, "", "", objParametri_Utenti)
            up.Permessi = Popola_UtentePermessi(dt_permessi)

            Dim impostazioni = objUtenti_Impostazioni.LeggiTuttoScalare(objParametri_Utenti)
            up.Impostazioni = impostazioni.ToList()

        End If


        Return up
    End Function

    Public Function Get_Imprese_Impostazioni(Piva As String, Sa_Cod As Integer, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Imprese_Impostazioni)
        Dim l As New List(Of Imprese_Impostazioni)

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim dtImpreseImpostazioni As DataTable = objImpreseImpostazioni.Leggi(Piva,
                                                                                 Sa_Cod,
                                                                                 0,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri_Server)

        If Not IsNothing(dtImpreseImpostazioni) AndAlso dtImpreseImpostazioni.Rows.Count > 0 Then

            For Each row In dtImpreseImpostazioni.Rows
                Dim imp As New Imprese_Impostazioni

                imp.Piva = row("Piva")
                imp.Sa_Cod = row("Sa_Cod")
                imp.Impostazione_Cod = row("Impostazione_Cod")
                imp.Valore = row("Impostazione_Valore")

                l.Add(imp)
            Next
        End If

        Return l
    End Function

    Function Popola_UtentePermessi(dt_permessi As DataTable) As List(Of Utente_Permesso)
        Dim l As New List(Of Utente_Permesso)

        Dim distinctDT As DataTable = dt_permessi.DefaultView.ToTable(True, "Id_Attivita")

        For Each row In distinctDT.Rows
            Dim up As New Utente_Permesso
            up.Permesso_ID = row("Id_Attivita")
            If dt_permessi.Select(" ID_Attivita = " & row("Id_Attivita") & " AND ID_Operazione = 2 ").Length > 0 Then
                up.Permesso_Tipo = 2
            Else
                up.Permesso_Tipo = 0
            End If
            l.Add(up)

        Next

        Return l
    End Function

    Function Popola_UtenteImpostazioni(dt_Impostazioni As DataTable) As List(Of Utente_Impostazioni)
        Dim l As New List(Of Utente_Impostazioni)

        For Each row In dt_Impostazioni.Rows
            Dim ui As New Utente_Impostazioni
            ui.Impostazione_Cod = row("Impostazione_Cod")
            ui.Valore = row("Impostazione_Valore_1")
            l.Add(ui)
        Next

        Return l
    End Function

    Public Function PassaggioSitoAgenda(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)
        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda, , IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoPianoConcimazione(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        ByVal IDSezione As Integer) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjPianoConcimazione(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                Enum_SiteRedirector.GiasNG,
                objAgenda, IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoSincronizzatore(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IdSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)
        If objAgendaNG.RedirectUrl <> "" Then
            objAgendaNG.QueryStringFiltrino &= objAgendaNG.RedirectUrl
        End If
        Dim objAgenda = from_objAgendaNG_to_ObjParametriSincro2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda, IdSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoAnalisi(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1
                                        ) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriAnalisi(objAgendaNG)

        If objAgendaNG.Pagina_Richiesta <> enum_PagineAnalisi_2010.Pagina_Gestione_Laboratori Then
            objAgenda.Tipo_Analisi = If(objAgendaNG.Pagina_Richiesta = 0, enum_AnalisiTipo.Analisi_Fitofarmaci, objAgendaNG.Pagina_Richiesta)
        End If
        objAgenda.Salva()
        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda, IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoPua(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriPua(objAgendaNG)

        objAgenda.Tipo_Pua = objAgendaNG.Pagina_Richiesta

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaPUA_PassandoDirettamente_Parametri(Enum_SiteRedirector.GiasNG, objAgenda.Tipo_Pua, objParametri_Server.UtenteCodFiscale, objAgenda.Piva, 0)

        Return strRet

    End Function

    Public Function PassaggioSitoPlanning(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriPlanning2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda, IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoGiasOnline(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriGiasOnline(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda)

        Return strRet

    End Function

    Public Function PassaggioSitoStampe_2010(ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        addSessionParameters(objAgendaNG, ParametriAggiuntivi)

        Dim targetUrl As String = String.Empty

        Dim Report As Integer? = objAgendaNG.Pagina_Richiesta

        gestioneRedirectFiltrone(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, VariabiliInSessione, objAgendaNG, ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi, IDSezione, targetUrl, Report)
        If targetUrl = String.Empty Then
            RedirectGestione_Stampe.GestioneRedirectStampe(Report, objAgendaNG.Piva, targetUrl,
                                                               ParametriAggiuntivi, objParametri_Server, objParametri_Utenti)
        End If

        Return targetUrl

    End Function

    Private Sub gestioneRedirectFiltrone(ByRef objParametri_Super_Server As AgronicaCoreParametri, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri, VariabiliInSessione As VariabiliInSessione_NG, ByRef objAgendaNG As Parametri_ObjParametriAgenda_NG, ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString), AggiungiSoloParametriAggiuntivi As Boolean, IDSezione As Integer, ByRef targetUrl As String, Report As Integer?)
        'Uhalid 13/03/23, Alcune pagine sono logicamente sulle stampe ma fisicamente sulla agenda 2010, (quelle nel select case qui sotto, hanno come enum_site_redirector quello dello stampe)
        'per ovviare ai problemi con redirect da angular, viene fatto un redirect al gestioneRichieste del agenda
        'Che si occupera di fare il redirect corretto sfruttando l'IDSezione in querystring, meccanismo gia usato prima per i redirect tra progetti diversi.
        If IDSezione <> -1 Then
            Select Case Report

                Case enum_CodificaStampe.ReportRisultatoFilrone,
                     enum_CodificaStampe.PianoColturale,
                     enum_CodificaStampe.ReportRisultatoFiltroneG2G,
                     enum_CodificaStampe.ReportRisultatoFiltroneIncludiVisita,
                     enum_CodificaStampe.PianoColturaleCatasto


                    objAgendaNG.Pagina_Richiesta = enum_PagineAgenda_2010.Menu
                    targetUrl = PassaggioSitoAgenda(objParametri_Super_Server, objParametri_Server _
                                                    , objParametri_Utenti, VariabiliInSessione, objAgendaNG _
                                                    , ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi _
                                                    , IDSezione)
                    targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "IDSezione", IDSezione)

            End Select
        End If
    End Sub

    Private Sub addSessionParameters(ByVal objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                     ByRef ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString))
        If ParametriAggiuntivi Is Nothing Then
            ParametriAggiuntivi = New List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString)
        End If

        Dim newParam = New ParametriAggiuntivi_QueryString()
        newParam.key = "Piva"
        newParam.value = objAgendaNG.Piva
        ParametriAggiuntivi.Add(newParam)

        newParam = New ParametriAggiuntivi_QueryString()
        newParam.key = "Sa_Cod"
        newParam.value = objAgendaNG.Sa_Cod
        ParametriAggiuntivi.Add(newParam)
    End Sub

    Public Function PassaggioSitoProfilazione(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriProfilazione_2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoProfilazione_PassandoDirettamente_ParametriProfilazione_2010(Enum_SiteRedirector.GiasNG,
                                                                                                                              objAgenda)

        Return strRet

    End Function


    Public Function PassaggioSitoAudit(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        Dim Audit_Tipo = IIf(objAgendaNG.Pagina_Richiesta = 0, enum_AuditPuaTipo.Audit_Condizionalita, objAgendaNG.Pagina_Richiesta)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAudit_PassandoDirettamente_Parametri(Enum_SiteRedirector.GiasNG,
                                                                                                                                    Audit_Tipo,
                                                                                                                                    objParametri_Server.UtenteCodFiscale,
                                                                                                                                    objAgenda.Piva,
                                                                                                                                    0, IDSezione)

        Return strRet

    End Function
    Public Function PassaggioSitoAgronicaAuditSicurezzaGlobalCoop(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        Dim Audit_Tipo = IIf(objAgendaNG.Pagina_Richiesta = 0, enum_AuditPuaTipo.Audit_Condizionalita, objAgendaNG.Pagina_Richiesta)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_Parametri_NG(Enum_SiteRedirector.GiasNG,
                                                                                                                                    Audit_Tipo,
                                                                                                                                    objParametri_Server.UtenteCodFiscale,
                                                                                                                                    objAgenda.Piva,
                                                                                                                                    0, IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoLabQualita(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IdSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjParametriLabCQ(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaLabCQ_PassandoDirettamente_ParametriLabCQ(Enum_SiteRedirector.GiasNG, objAgenda)

        Return strRet

    End Function

    Public Function PassaggioSitoPianiCampionamento(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IdSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianiCampionamento_PassandoDirettamenteIParametri(Enum_SiteRedirector.GiasNG,
                                                                                                                                    objAgendaNG.Pagina_Richiesta,
                                                                                                                                    0,
                                                                                                                                    Nothing,
                                                                                                                                    0,
                                                                                                                                    Piva:=objAgenda.Piva,
                                                                                                                                    IdSezione:=IdSezione)

        Return strRet

    End Function


    Public Function PassaggioSitoPianiSemina(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianiSemina_PassandoDirettamenteIParametri(Enum_SiteRedirector.GiasNG, objAgenda.PaginaRichiesta, objAgenda.PaginaProvenienza)

        Return strRet

    End Function

    Public Function PassaggioSitoAgronicaUma(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgronicaUMA_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda, "", IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoAgronicaDomandaIrrigua(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        ' Costruisco un ParametriAgenda_2010 a partire dal payload Angular: la conversione enum
        ' (TipoOperazioneAgenda, TipoRicetta) e le trasformazioni dei campi sono già implementate in
        ' from_objAgendaNG_to_ObjAgenda2010. Da lì copio nello schema "single object" di DomandaIrrigua.
        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        Dim ObjParametriDomanda = New ParametriDomandaIrrigua() With {
            .PaginaProvenienza = objAgenda.PaginaProvenienza,
            .PaginaRichiesta = objAgenda.PaginaRichiesta,
            .Piva = If(objAgenda.Piva, ""),
            .DataSelezionata = objAgenda.DataSelezionata,
            .Lav_Cod = objAgenda.Lav_Cod,
            .Id_Agenda = objAgenda.Id_Agenda,
            .Sa_Cod = objAgenda.Sa_Cod,
            .Veg_Cod = CStr(objAgenda.Veg_Cod),
            .Appezza = objAgenda.Appezza,
            .Id_Reg = objAgenda.Id_Reg,
            .TipoOperazioneAgenda = objAgenda.TipoOperazioneAgenda,
            .TargetOperazione = objAgenda.TargetOperazione,
            .Programmazione_Cod = objAgenda.Programmazione_Cod,
            .TipoRicetta = objAgenda.TipoRicetta,
            .Tipo_Operazione = objAgenda.Tipo_Operazione,
            .RedirectUrl = If(objAgenda.RedirectUrl, ""),
            .QueryStringFiltrino = If(objAgenda.QueryStringFiltrino, "")}

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(Enum_SiteRedirector.GiasNG,
                                                                                                                                                           ObjParametriDomanda, IDSezione)

        Return strRet

    End Function

    Public Function PassaggioSitoGiasOnlineVecchio(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal ParametriAggiuntivi As List(Of AgronicaCoreGestioneRichieste.ParametriAggiuntivi_QueryString),
                                        ByVal AggiungiSoloParametriAggiuntivi As Boolean,
                                        Optional ByVal IDSezione As Integer = -1) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        objAgendaNG.QueryStringFiltrino = Componi_QueryStringFiltrino(ParametriAggiuntivi, AggiungiSoloParametriAggiuntivi)

        Dim ObjParametriGiasOnline = New ParametriGiasOnline() With {
            .PaginaRichiesta = objAgendaNG.Pagina_Richiesta,
            .Piva = objAgendaNG.Piva}

        strRet = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.GiasNG, ObjParametriGiasOnline)

        Return strRet

    End Function

    Public Function GestioneStampe(ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   ByVal Report As Integer,
                                   ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                   ByRef objAgendaNG As Parametri_ObjParametriAgenda_NG,
                                   ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString)) As String

        Dim piva As String = objAgendaNG.Piva
        Dim sa_cod As String = objAgendaNG.Sa_Cod
        Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)

        Select Case Report

            Case enum_CodificaStampe.Quadro_P, enum_CodificaStampe.RiepilogoImpiegoSuperfici

                Dim vVarStampe(1) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = piva

                vVarStampe(1).Nome = "sa_cod"
                vVarStampe(1).Valore = sa_cod

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
                Dim StrNodiVariabili As String = StrNodo

                HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

                ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

                Dim ParametriAgronicaStampe As New ParametriAgronicaStampe
                ParametriAgronicaStampe.report = Report
                ParametriAgronicaStampe.username = objParametri_Utenti.UtenteUsername
                ParametriAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                ParametriAgronicaStampe.Xml_Generico.Length = 0
                ParametriAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
                ParametriAgronicaStampe.Id_Sezione = objAgendaNG.IdSezione
                'RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
                'RedirectURL &= "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

                'Dim ParametriAgronicaStampe As ParametriAgronicaStampe = RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(report, piva, HttpContext.Current.Session, objParametri_Server, Sa_Cod:=sa_cod)
                Dim objWebConfig As New AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)
                Dim strJS = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.GiasNG, ParametriAgronicaStampe)
                Return strJS

        End Select

    End Function

    Function from_objAgendaNG_to_ObjAgenda2010(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriAgenda_2010

        Dim objAgenda As New ParametriAgenda_2010

        objAgenda.Operazione = objAgendaNG.TipoOperazioneDB
        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Sa_Cod = objAgendaNG.Sa_Cod
        objAgenda.Fabbricato = objAgendaNG.Fabbricato
        objAgenda.Campo_Cod = objAgendaNG.Campo_Cod
        objAgenda.Appezza = objAgendaNG.Appezza
        objAgenda.Id_Reg = objAgendaNG.Id_Reg
        objAgenda.Validita_Inizio = objAgendaNG.Validita_Inizio
        objAgenda.Validita_Fine = objAgendaNG.Validita_Fine
        objAgenda.Veg_Cod = objAgendaNG.Veg_Cod
        objAgenda.Id_Cod = objAgendaNG.Id_Cod
        objAgenda.PaginaProvenienza = objAgendaNG.Pagina_Provenienza
        objAgenda.PaginaRichiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.Cod_Contatto = objAgendaNG.Cod_Contatto
        objAgenda.QueryStringFiltrino = objAgendaNG.QueryStringFiltrino
        objAgenda.DataSelezionata = objAgendaNG.Data
        objAgenda.Lav_Cod = objAgendaNG.Lav_Cod
        objAgenda.Lavorazione = objAgendaNG.Lav_Cod
        objAgenda.Operazione = objAgendaNG.TipoOperazioneDB
        objAgenda.Id_Agenda = objAgendaNG.Id_Agenda
        objAgenda.GenericObj_string = objAgendaNG.GenericObj_string
        objAgenda.RedirectUrl = objAgendaNG.RedirectUrl
        objAgenda.Chiave = objAgendaNG.Chiave
        objAgenda.TipoRicetta = enum_TipoRicetta.Non_Filtrare

        If objAgendaNG.TipoOperazioneAgenda = attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna Then

            objAgenda.TipoOperazioneAgenda = objAgendaNG.TipoOperazioneAgenda

        ElseIf objAgendaNG.TipoOperazioneAgenda = attivita.Attivita.Tipo_Attivita.Ricetta Then

            If objAgendaNG.Stato = attivita.Attivita.Stati.Da_Eseguire Then

                objAgenda.TipoOperazioneAgenda = objAgendaNG.TipoOperazioneAgenda

            ElseIf objAgendaNG.Stato = attivita.Attivita.Stati.Eseguita Then

                objAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio

            End If

            objAgenda.TipoRicetta = objAgendaNG.TipoRicetta

        End If

        objAgenda.TargetOperazione = objAgendaNG.TargetOperazione
        objAgenda.Programmazione_Cod = objAgendaNG.Programmazione_Cod
        objAgenda.Tipo_Operazione = objAgendaNG.TipoOperazioneDB



        If objAgendaNG.Impianti IsNot Nothing Then
            Dim listImpianti2010 As New List(Of Impianti_2010)
            For Each imp In objAgendaNG.Impianti
                Dim imp2010 = New Impianti_2010
                imp2010.Piva = imp.Piva
                imp2010.Sa_Cod = imp.Sa_Cod
                imp2010.Appezza = imp.Appezza
                imp2010.Id_Reg = imp.Id_Reg
                imp2010.Progetto_Cod = imp.Progetto_Cod
                imp2010.veg_cod = imp.Veg_Cod
                imp2010.id_cod = imp.Id_Cod


                listImpianti2010.Add(imp2010)
            Next
            objAgenda.Impianti = listImpianti2010
        End If

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjPianoConcimazione(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriConcimazione_2017

        Dim objParametriConcimazione As New ParametriConcimazione_2017

        objParametriConcimazione.Tipo_Operazione = objAgendaNG.TipoOperazioneDB
        objParametriConcimazione.Piva = objAgendaNG.Piva
        objParametriConcimazione.Sa_Cod = objAgendaNG.Sa_Cod
        objParametriConcimazione.Veg_Cod = objAgendaNG.Veg_Cod
        objParametriConcimazione.Pagina_Richiesta = objAgendaNG.Pagina_Richiesta
        objParametriConcimazione.SitoOrigine = Enum_SiteRedirector.GiasNG
        objParametriConcimazione.Pagina_SitoOrigine = objAgendaNG.Pagina_Provenienza
        objParametriConcimazione.PianoConcimazione_Testata_Cod = objAgendaNG.Ricetta_Cod

        If objAgendaNG.Impianti IsNot Nothing Then
            Dim listImpianti2010 As New List(Of Impianto)
            For Each imp In objAgendaNG.Impianti
                Dim imp2010 = New Impianto()
                imp2010.Piva = imp.Piva
                imp2010.Sa_Cod = imp.Sa_Cod
                imp2010.Appezza = imp.Appezza
                imp2010.Id_Reg = imp.Id_Reg
                imp2010.Progetto_Cod = imp.Progetto_Cod
                imp2010.Veg_Cod = imp.Veg_Cod

                listImpianti2010.Add(imp2010)
            Next
            objParametriConcimazione.ListaImpianti = listImpianti2010.ToArray
        End If

        Return objParametriConcimazione
    End Function

    Function from_objAgendaNG_to_ObjParametriSincro2010(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriSincronizzatore_2010

        Dim objAgenda As New ParametriSincronizzatore_2010

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Pagina_Richiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.ParametriQueryString = objAgendaNG.QueryStringFiltrino

        objAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG
        objAgenda.Pagina_SitoOrigine = objAgendaNG.Pagina_Provenienza

        Return objAgenda
    End Function

    Private Class GenericObjNG
        Public Analisi_Testata_Cod As Integer
    End Class

    Function from_objAgendaNG_to_ObjParametriAnalisi(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriAnalisi_2010

        Dim objAgenda As New ParametriAnalisi_2010

        If objAgendaNG.GenericObj_string <> "" Then
            Try
                Dim Analisi_Testata_Cod = JsonConvert.DeserializeObject(Of GenericObjNG)(objAgendaNG.GenericObj_string)
                objAgenda.Analisi_Testata_Cod = Analisi_Testata_Cod.Analisi_Testata_Cod
            Catch ex As Exception
            End Try
        End If

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Pagina_Richiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG
        objAgenda.Pagina_SitoOrigine = objAgendaNG.Pagina_Provenienza

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjParametriPua(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriPUA

        Dim objAgenda As New ParametriPUA

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Pagina_Richiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG
        objAgenda.Pagina_SitoOrigine = objAgendaNG.Pagina_Provenienza

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjParametriPlanning2010(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriPlanning

        Dim objAgenda As New ParametriPlanning

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.PaginaProvenienza = objAgendaNG.Pagina_Provenienza
        objAgenda.PaginaRichiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.ParametriAggiuntivi = objAgendaNG.QueryStringFiltrino

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjParametriGiasOnline(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriGiasOnline

        Dim objAgenda As New ParametriGiasOnline
        objAgenda.Operazione = objAgendaNG.TipoOperazioneDB
        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Sa_Cod = objAgendaNG.Sa_Cod
        objAgenda.Id_Reg = objAgendaNG.Id_Reg
        objAgenda.Veg_Cod = objAgendaNG.Veg_Cod
        objAgenda.PaginaRichiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.Cod_Contatto = objAgendaNG.Cod_Contatto
        objAgenda.DataSelezionata = objAgendaNG.Data
        objAgenda.Lavorazione = objAgendaNG.Lav_Cod
        objAgenda.Operazione = objAgendaNG.TipoOperazioneDB

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjParametriProfilazione_2010(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriProfilazione_2010

        Dim objAgenda As New ParametriProfilazione_2010

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.Pagina_Richiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.SitoRichiesto = Enum_SiteRedirector.Sito_AgronicaProfilazione

        Return objAgenda
    End Function

    Function from_objAgendaNG_to_ObjParametriLabCQ(ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As ParametriLabCQ

        Dim objAgenda As New ParametriLabCQ

        objAgenda.Piva = objAgendaNG.Piva
        objAgenda.PaginaRichiesta = objAgendaNG.Pagina_Richiesta
        objAgenda.PaginaProvenienza = objAgendaNG.Pagina_Provenienza

        Return objAgenda
    End Function

    Public Sub ImpostaVariabiliInSessione(VariabiliInSessione As VariabiliInSessione_NG,
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        HttpContext.Current.Session("Collegamento_DPI") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.collegamento_dpi, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("Collegamento_Fito") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.collegamento_fito, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_IdServizio") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.id_servizio, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_PathFileINI") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.pathfileini, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Connessione_Server") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.cn_server, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Connessione_Utenti") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.cn_utenti, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Connessione_LOG") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.cn_logaccessi, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_StringaConnessione_Server") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.stringa_cn_server, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_StringaConnessione_Utenti") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.stringa_cn_utenti, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_ProgressivoGIAS") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.progressivo_gias, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Utente_Username") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.utente_usr, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Utente_Password") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.utente_pwd, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Utente_CodFiscale") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.utente_codfiscale, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Utente_Username_Crypt") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.utente_usr_crypt, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Utente_Password_Crypt") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.utente_pwd_crypt, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)

        HttpContext.Current.Session("ASG_SuperUser_Username") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_usr, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)

        HttpContext.Current.Session("ASG_SuperUser_Password") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_pwd, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_piva, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_SuperUser_Username_Crypt") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_usr_crypt, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_SuperUser_Password_Crypt") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_pwd_crypt, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_FinestraTemporale_Inizio") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.finestratemporale_inizio, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_FinestraTemporale_Fine") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.finestratemporale_fine, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_AgronicaCore_Flag_CancellazioneLogica") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.agronicacore_flag_cancellazionelogica, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_AgronicaCore_Flag_Visibilita") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.agronicacore_flag_visibilita, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_AgronicaCore_DirectoryLOG") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.agronicacore_directorylog, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_Super_Server_PivaSuperUser") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile(VariabiliInSessione.superuser_piva, AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        HttpContext.Current.Session("ASG_StringaConnessione_Super_Server") = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica_LANCompatibile("", AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)

        HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
        HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
    End Sub


    Public Function GetVariabiliInSessioneNG(VarInSessione As AgronicaCoreDTOStd.InData.VariabiliInSessione_NG) As AgronicaCoreGestioneRichieste.VariabiliInSessione_NG
        Dim VariabiliInSessione As New AgronicaCoreGestioneRichieste.VariabiliInSessione_NG

        VariabiliInSessione.collegamento_dpi = VarInSessione.collegamento_dpi
        VariabiliInSessione.collegamento_fito = VarInSessione.collegamento_fito
        VariabiliInSessione.id_servizio = VarInSessione.id_servizio
        VariabiliInSessione.pathfileini = VarInSessione.pathfileini
        VariabiliInSessione.cn_server = VarInSessione.cn_server
        VariabiliInSessione.cn_utenti = VarInSessione.cn_utenti
        VariabiliInSessione.cn_logaccessi = VarInSessione.cn_logaccessi
        VariabiliInSessione.stringa_cn_server = VarInSessione.stringa_cn_server
        VariabiliInSessione.stringa_cn_utenti = VarInSessione.stringa_cn_utenti
        VariabiliInSessione.progressivo_gias = VarInSessione.progressivo_gias
        VariabiliInSessione.utente_usr = VarInSessione.utente_usr
        VariabiliInSessione.utente_pwd = VarInSessione.utente_pwd
        VariabiliInSessione.utente_codfiscale = VarInSessione.utente_codfiscale
        VariabiliInSessione.utente_usr_crypt = VarInSessione.utente_usr_crypt
        VariabiliInSessione.utente_pwd_crypt = VarInSessione.utente_pwd_crypt
        VariabiliInSessione.superuser_usr = VarInSessione.superuser_usr
        VariabiliInSessione.superuser_pwd = VarInSessione.superuser_pwd
        VariabiliInSessione.superuser_piva = VarInSessione.superuser_piva
        VariabiliInSessione.superuser_usr_crypt = VarInSessione.superuser_usr_crypt
        VariabiliInSessione.superuser_pwd_crypt = VarInSessione.superuser_pwd_crypt
        VariabiliInSessione.finestratemporale_inizio = VarInSessione.finestratemporale_inizio
        VariabiliInSessione.finestratemporale_fine = VarInSessione.finestratemporale_fine
        VariabiliInSessione.agronicacore_flag_cancellazionelogica = VarInSessione.agronicacore_flag_cancellazionelogica
        VariabiliInSessione.agronicacore_flag_visibilita = VarInSessione.agronicacore_flag_visibilita
        VariabiliInSessione.agronicacore_directorylog = VarInSessione.agronicacore_directorylog
        VariabiliInSessione.agronicacore_nomefilelog = VarInSessione.agronicacore_nomefilelog

        Return VariabiliInSessione
    End Function

    Public Function CaricaVariabiliInSessione(XML_AgroWebConfig As System.Xml.XmlElement) As VariabiliInSessione_NG
        Dim variabili_in_sessione As New VariabiliInSessione_NG

        If XML_AgroWebConfig.HasAttribute("collegamento_dpi") Then
            variabili_in_sessione.collegamento_dpi = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("collegamento_dpi"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("collegamento_fito") Then
            variabili_in_sessione.collegamento_fito = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("collegamento_fito"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("id_servizio") Then
            variabili_in_sessione.id_servizio = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("id_servizio"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("pathfileini") Then
            variabili_in_sessione.pathfileini = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("pathfileini"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("cn_server") Then
            variabili_in_sessione.cn_server = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("cn_server"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("cn_utenti") Then
            variabili_in_sessione.cn_utenti = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("cn_utenti"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("cn_logaccessi") Then
            variabili_in_sessione.cn_logaccessi = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("cn_logaccessi"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("stringa_cn_server") Then
            variabili_in_sessione.stringa_cn_server = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("stringa_cn_server"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("stringa_cn_utenti") Then
            variabili_in_sessione.stringa_cn_utenti = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("stringa_cn_utenti"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("progressivo_gias") Then
            variabili_in_sessione.progressivo_gias = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("progressivo_gias"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("utente_usr") Then
            variabili_in_sessione.utente_usr = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("utente_usr"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("finestratemporale_inizio") Then
            variabili_in_sessione.finestratemporale_inizio = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("finestratemporale_inizio"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("finestratemporale_fine") Then
            variabili_in_sessione.finestratemporale_fine = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("finestratemporale_fine"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("agronicacore_flag_cancellazionelogica") Then
            variabili_in_sessione.agronicacore_flag_cancellazionelogica = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("agronicacore_flag_cancellazionelogica"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("agronicacore_flag_visibilita") Then
            variabili_in_sessione.agronicacore_flag_visibilita = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("agronicacore_flag_visibilita"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("agronicacore_directorylog") Then
            variabili_in_sessione.agronicacore_directorylog = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("agronicacore_directorylog"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("utente_pwd") Then
            variabili_in_sessione.utente_pwd = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("utente_pwd"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("utente_codfiscale") Then
            variabili_in_sessione.utente_codfiscale = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("utente_codfiscale"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("utente_usr_crypt") Then
            variabili_in_sessione.utente_usr_crypt = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("utente_usr_crypt"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("utente_pwd_crypt") Then
            variabili_in_sessione.utente_pwd_crypt = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("utente_pwd_crypt"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("superuser_pwd") Then
            variabili_in_sessione.superuser_pwd = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("superuser_pwd"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("superuser_usr_crypt") Then
            variabili_in_sessione.superuser_usr_crypt = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("superuser_usr_crypt"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        If XML_AgroWebConfig.HasAttribute("superuser_pwd_crypt") Then
            variabili_in_sessione.superuser_pwd_crypt = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(XML_AgroWebConfig.GetAttribute("superuser_pwd_crypt"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder, Esci_Se_Vuoto:=True)
        End If

        variabili_in_sessione.agronicacore_nomefilelog = ""
        variabili_in_sessione.superuser_piva = ""
        variabili_in_sessione.superuser_usr = ""

        Return variabili_in_sessione
    End Function

    Public Function CaricaParametriAgenda_NG(XML_AgroWebConfig As System.Xml.XmlElement,
                                             Sito_Origine As Integer,
                                             objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Parametri_ObjParametriAgenda_NG

        Dim objP_Agenda As New Parametri_ObjParametriAgenda_NG
        If XML_AgroWebConfig.HasAttribute("pagina_richiesta") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("pagina_richiesta")) Then
            objP_Agenda.Pagina_Richiesta = XML_AgroWebConfig.GetAttribute("pagina_richiesta")
        End If

        If XML_AgroWebConfig.HasAttribute("pagina_provenienza") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("pagina_provenienza")) Then
            objP_Agenda.Pagina_Provenienza = XML_AgroWebConfig.GetAttribute("pagina_provenienza")
        End If

        If XML_AgroWebConfig.HasAttribute("pagina_provenienza_altrosito") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("pagina_provenienza_altrosito")) Then
            objP_Agenda.Pagina_Provenienza_AltroSito = XML_AgroWebConfig.GetAttribute("pagina_provenienza_altrosito")
        End If

        If XML_AgroWebConfig.HasAttribute("tipooperazionedb") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("tipooperazionedb")) Then
            objP_Agenda.TipoOperazioneDB = XML_AgroWebConfig.GetAttribute("tipooperazionedb")
        End If

        If XML_AgroWebConfig.HasAttribute("piva") Then
            objP_Agenda.Piva = XML_AgroWebConfig.GetAttribute("piva")
        End If

        If XML_AgroWebConfig.HasAttribute("sa_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("sa_cod")) Then
            objP_Agenda.Sa_Cod = XML_AgroWebConfig.GetAttribute("sa_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("fabbricato") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("fabbricato")) Then
            objP_Agenda.Fabbricato = XML_AgroWebConfig.GetAttribute("fabbricato")
        End If

        If XML_AgroWebConfig.HasAttribute("campo_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("campo_cod")) Then
            objP_Agenda.Campo_Cod = XML_AgroWebConfig.GetAttribute("campo_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("appezza") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("appezza")) Then
            objP_Agenda.Appezza = XML_AgroWebConfig.GetAttribute("appezza")
        End If

        If XML_AgroWebConfig.HasAttribute("id_reg") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("id_reg")) Then
            objP_Agenda.Id_Reg = XML_AgroWebConfig.GetAttribute("id_reg")
        End If

        If XML_AgroWebConfig.HasAttribute("progetto_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("progetto_cod")) Then
            objP_Agenda.Progetto_Cod = XML_AgroWebConfig.GetAttribute("progetto_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("validita_inizio") AndAlso IsDate(XML_AgroWebConfig.GetAttribute("validita_inizio")) Then
            objP_Agenda.Validita_Inizio = CDate(XML_AgroWebConfig.GetAttribute("validita_inizio"))
        Else
            objP_Agenda.Validita_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
        End If

        If XML_AgroWebConfig.HasAttribute("validita_fine") AndAlso IsDate(XML_AgroWebConfig.GetAttribute("validita_fine")) Then
            objP_Agenda.Validita_Fine = CDate(XML_AgroWebConfig.GetAttribute("validita_fine"))
        Else
            objP_Agenda.Validita_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
        End If

        If XML_AgroWebConfig.HasAttribute("data") AndAlso IsDate(XML_AgroWebConfig.GetAttribute("data")) Then
            objP_Agenda.Data = CDate(XML_AgroWebConfig.GetAttribute("data"))
        End If

        If XML_AgroWebConfig.HasAttribute("veg_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("veg_cod")) Then
            objP_Agenda.Veg_Cod = XML_AgroWebConfig.GetAttribute("veg_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("veg_des") Then
            objP_Agenda.Veg_Des = XML_AgroWebConfig.GetAttribute("veg_des")
        End If

        If XML_AgroWebConfig.HasAttribute("id_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("id_cod")) Then
            objP_Agenda.Id_Cod = XML_AgroWebConfig.GetAttribute("id_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("id_des") Then
            objP_Agenda.Id_Des = XML_AgroWebConfig.GetAttribute("id_des")
        End If

        If XML_AgroWebConfig.HasAttribute("cau_mov") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("cau_mov")) Then
            objP_Agenda.Cau_Mov = XML_AgroWebConfig.GetAttribute("cau_mov")
        End If

        If XML_AgroWebConfig.HasAttribute("mac_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("mac_cod")) Then
            objP_Agenda.Mac_Cod = XML_AgroWebConfig.GetAttribute("mac_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("lav_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("lav_cod")) Then
            objP_Agenda.Lav_Cod = XML_AgroWebConfig.GetAttribute("lav_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("lav_des") Then
            objP_Agenda.Lav_Des = XML_AgroWebConfig.GetAttribute("lav_des")
        End If

        If XML_AgroWebConfig.HasAttribute("sanome") Then
            objP_Agenda.SaNome = XML_AgroWebConfig.GetAttribute("sanome")
        End If

        If XML_AgroWebConfig.HasAttribute("ragsoc") Then
            objP_Agenda.RagSoc = XML_AgroWebConfig.GetAttribute("ragsoc")
        End If

        If XML_AgroWebConfig.HasAttribute("querystringfiltrino") Then
            objP_Agenda.QueryStringFiltrino = XML_AgroWebConfig.GetAttribute("querystringfiltrino")
        End If

        If XML_AgroWebConfig.HasAttribute("idsezione") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("idsezione")) Then
            objP_Agenda.IdSezione = XML_AgroWebConfig.GetAttribute("idsezione")
        End If

        If XML_AgroWebConfig.HasAttribute("id_agenda") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("id_agenda")) Then
            objP_Agenda.Id_Agenda = XML_AgroWebConfig.GetAttribute("id_agenda")
        End If

        If XML_AgroWebConfig.HasAttribute("cod_contatto") Then
            objP_Agenda.Cod_Contatto = XML_AgroWebConfig.GetAttribute("cod_contatto")
        End If

        If XML_AgroWebConfig.HasAttribute("tipooperazioneagenda") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("tipooperazioneagenda")) Then
            objP_Agenda.TipoOperazioneAgenda = XML_AgroWebConfig.GetAttribute("tipooperazioneagenda")
        End If

        If XML_AgroWebConfig.HasAttribute("tiporicetta") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("tiporicetta")) Then
            objP_Agenda.TipoRicetta = XML_AgroWebConfig.GetAttribute("tiporicetta")
        End If

        If XML_AgroWebConfig.HasAttribute("stato") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("stato")) Then
            objP_Agenda.Stato = XML_AgroWebConfig.GetAttribute("stato")
        End If

        If XML_AgroWebConfig.HasAttribute("ricetta_operazione_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("ricetta_operazione_cod")) Then
            objP_Agenda.Ricetta_Operazione_Cod = XML_AgroWebConfig.GetAttribute("ricetta_operazione_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("ricetta_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("ricetta_cod")) Then
            objP_Agenda.Ricetta_Cod = XML_AgroWebConfig.GetAttribute("ricetta_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("genericobj_string") Then
            objP_Agenda.GenericObj_string = XML_AgroWebConfig.GetAttribute("genericobj_string")
        End If

        If XML_AgroWebConfig.HasAttribute("targetoperazione") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("targetoperazione")) Then
            objP_Agenda.TargetOperazione = XML_AgroWebConfig.GetAttribute("targetoperazione")
        End If

        If XML_AgroWebConfig.HasAttribute("programmazione_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("programmazione_cod")) Then
            objP_Agenda.Programmazione_Cod = XML_AgroWebConfig.GetAttribute("programmazione_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("redirecturl") Then
            objP_Agenda.RedirectUrl = XML_AgroWebConfig.GetAttribute("redirecturl")
        End If

        If XML_AgroWebConfig.HasAttribute("chiave") Then
            objP_Agenda.Chiave = XML_AgroWebConfig.GetAttribute("chiave")
        End If

        If XML_AgroWebConfig.HasAttribute("impianti") AndAlso Not IsNothing(XML_AgroWebConfig.GetAttribute("impianti")) AndAlso Not String.IsNullOrEmpty(XML_AgroWebConfig.GetAttribute("impianti")) Then
            objP_Agenda.Impianti = JsonConvert.DeserializeObject(Of List(Of ImpiantiAgendaNG))(XML_AgroWebConfig.GetAttribute("impianti").ToString())
        End If

        If XML_AgroWebConfig.HasAttribute("regolamento_cod") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("regolamento_cod")) Then
            objP_Agenda.Regolamento_Cod = XML_AgroWebConfig.GetAttribute("regolamento_cod")
        End If

        If XML_AgroWebConfig.HasAttribute("tipo_regolamento") AndAlso IsNumeric(XML_AgroWebConfig.GetAttribute("tipo_regolamento")) Then
            objP_Agenda.Tipo_Regolamento = XML_AgroWebConfig.GetAttribute("tipo_regolamento")
        End If

        If objP_Agenda.Piva <> "" AndAlso objP_Agenda.RagSoc = "" Then
            Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Read
            objP_Agenda.RagSoc = objAnagrafe.RagSoc_from_Piva(objP_Agenda.Piva, objParametri_Server)
        End If

        objP_Agenda.Sito_Provenienza = Sito_Origine

        Return objP_Agenda
    End Function

    Public Function Componi_QueryStringFiltrino(ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString), ByVal AggiungiSoloParametriAggiuntivi As Boolean) As String

        Dim QueryStringFiltrino As String = String.Empty

        If Not IsNothing(ParametriAggiuntivi) AndAlso ParametriAggiuntivi.Count > 0 Then

            If AggiungiSoloParametriAggiuntivi Then
                QueryStringFiltrino = "?"
            Else
                QueryStringFiltrino = "&"
            End If

            For index As Integer = 0 To ParametriAggiuntivi.Count - 1

                Dim paramValue As String = ""
                If ParametriAggiuntivi(index).codifica Then
                    paramValue = Stringa_Codifica(ParametriAggiuntivi(index).value, AgroKey_EncoderDecoder)
                Else
                    paramValue = ParametriAggiuntivi(index).value
                End If

                If index = 0 Then

                    QueryStringFiltrino &= ParametriAggiuntivi(index).key & "=" & paramValue

                Else

                    QueryStringFiltrino &= "&" & ParametriAggiuntivi(index).key & "=" & paramValue

                End If

            Next
        End If

        Return QueryStringFiltrino

    End Function

    Public Function GestioneRedirectSitoAgenda(ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal VariabiliInSessione As VariabiliInSessione_NG,
                                        ByRef objAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As String

        Dim strRet = ""

        HttpContext.Current.Session("ASG_Connessione_Server") = VariabiliInSessione.cn_server

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

        ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        Dim objAgenda = from_objAgendaNG_to_ObjAgenda2010(objAgendaNG)

        objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu
        strRet = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)

        Return strRet

    End Function



End Class
