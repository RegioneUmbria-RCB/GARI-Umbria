Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreXML.XML_Stampe
Imports System.Drawing
Imports System.Web
Imports System.Net
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.ServiceModel.Channels
Imports AgronicaCoreModello.DatiStampe
Imports AgronicaCoreGestioneRichieste
Imports CrystalDecisions.Web
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreFiltroneBIZ.FiltroRicerca
Imports AgronicaCoreModello
Imports AgronicaCoreFiltroneBIZ

Public Class MenuStampeBootstrap
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public Shared objParametriAgenda As ParametriAgenda

    Public Sub New()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda


        If Not IsPostBack Then

            DistruggiSessionVecchie()

            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente(
                                        Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gest_Stampe,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)

            ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

            Dim UtenteAbilitato_Modifica As Boolean = False
            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                       Session("ASG_IdServizio"),
                                       enum_Security_Attivita.Gest_Stampe,
                                       enum_Security_Operazione.Modifica,
                                       Date.Now,
                                       "",
                                       objParametri_Utenti)

            ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################
            ImpostaPermessi()

            If Not Page.IsPostBack Then
                Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
                hd_usaFiltroRicercaNG.Value = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
                hd_CurrentPiva.Value = objParametriAgenda.Piva
            End If

        End If
    End Sub
    Public Sub ImpostaPermessi()
        'Controllo se ha il permesso di lettura
        If ViewState("UtenteAbilitato_Lettura") = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

    End Sub

    Private Sub DistruggiSessionVecchie()

        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaStampeAutorizzate() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If
            Dim StampeReport As New AgronicaCoreMetaSchemaDAL.StampeReport


            Dim expObj As List(Of IDictionary(Of String, Object))


            expObj = StampeReport.LeggiConGruppi(0, " StampeReportGruppi.Id_StampeReportGruppi <> 13 ", "Id_StampeReportGruppi", objParametri_Server).ToExpandoObject

            Dim stampeAutorizzate = expObj.
                Select(Of StampeItem)(Function(item) New StampeItem With {
                    .text = WebUtility.HtmlDecode(item("Descrizione")),
                    .value = item("Id_StampeReport"),
                    .group = WebUtility.HtmlDecode(item("GruppiDescrizione"))
                }).ToList


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(stampeAutorizzate)
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    Private Shared Function EnumToDictionary(enumType As Type) As Dictionary(Of String, Integer)
        If enumType Is Nothing OrElse Not enumType.IsEnum Then
            Throw New ArgumentException("Parameter must be an enum type")
        End If

        Dim enumDict As New Dictionary(Of String, Integer)()
        For Each value In [Enum].GetValues(enumType)
            enumDict.Add([Enum].GetName(enumType, value), CInt(value))
        Next

        Return enumDict
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ottieniEnumStampe() As RispostaStandard

        Dim r As New RispostaStandard
        Try
            Dim enumDictionary As Dictionary(Of String, Integer) = EnumToDictionary(GetType(enum_CodificaStampe))

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(enumDictionary)
        Catch ex As Exception
            r.RispostaOK = False
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function gestisciStampa(ByVal reportSel As Integer, ByVal datiStampe As AgronicaCoreModello.DatiStampe) As RispostaStandard

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = reportSel
        Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
        Dim session = HttpContext.Current.Session


        Dim r As New RispostaStandard

        Dim piva As String = objParametriAgenda.Piva
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim Origine As String
        Dim Destinazione As String
        Dim Funzione As String

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Origine = Stringa_Codifica("../Stampe/MenuStampeBootstrap.aspx", AgroKey_EncoderDecoder)

        Destinazione = Stringa_Codifica(
                        "../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder)

        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder)

        Dim link As String = ""
        Dim errNonGestito As String = ""
        Dim categoriaEsito As String = ""

        Dim RispostaOK As Boolean = True
        Dim RispostaStringa As String = ""

        gestisciStampa(reportSel, datiStampe, link, errNonGestito, objParametri_Server, objParametri_Utenti)

        If link = "" Then
            RispostaOK = False
            RispostaStringa = errNonGestito
        Else
            RispostaOK = True
            RispostaStringa = link
        End If

        r.RispostaOK = RispostaOK
        r.RispostaStringa = RispostaStringa

        Return r
    End Function

    Private Shared Function parametriFiltrone(reportselezionato As enum_CodificaStampe, ByRef objp As AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010) As AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
        objp.Sito_Origine = Stringa_Codifica(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    AgroKey_EncoderDecoder)
        objp.Pagina_Origine = Stringa_Codifica(
                                "../Stampe/Menu_Stampe.aspx",
                                AgroKey_EncoderDecoder)


        objp.Sito_Destinazione = Stringa_Codifica(
                    Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                    AgroKey_EncoderDecoder)
        objp.Pagina_Destinazione = Stringa_Codifica(
                    "../GestioneStampe/ChiamaStampe.aspx",
                    AgroKey_EncoderDecoder)


        objp.TipoFiltrone = Stringa_Codifica(
                               CStr(enum_TipoFiltrone.Stampa),
                               AgroKey_EncoderDecoder)

        objp.CodificaStampe = Stringa_Codifica(
                              reportselezionato,
                               AgroKey_EncoderDecoder)
        Return objp
    End Function

    Class StampeItem
        Public text As String
        Public value As String
        Public group As String
    End Class

    Class StampeContainer
        Public text As String
        Public value As String
        Public items As New List(Of StampeItem)
    End Class

    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaStampePreferite() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Try

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim expObj = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUserNotCacheable(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti).ToExpandoObject

            Dim metaStampe = New AgronicaCoreMetaSchemaDAL.StampeReport()
            Dim stampePreferite = expObj _
                   .Where(Function(row) Not IsDBNull(row("Impostazione_Valore_1")) AndAlso row("Impostazione_Valore_1") <> "") _
                   .SelectMany(Of String)(Function(row) row("Impostazione_Valore_1").ToString.Split("|").ToList) _
                   .Select(Of StampeItem)(Function(idStampe) New StampeItem With {
                                .text = WebUtility.HtmlDecode(metaStampe.LeggiDescrizione(idStampe, "", objParametri_Server)),
                                .value = CInt(idStampe)
                            }).ToList

            r.RispostaOK = True

            r.RispostaStringa = JsonConvert.SerializeObject(value:=stampePreferite, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})
        Catch ex As Exception
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function aggiungiPreferiti(idStampe As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = idStampe

        Dim nuovivalori As String = CStr(CInt(reportselezionato))

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dt = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUserNotCacheable(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim expObj = dt.ToExpandoObject
        Dim listaStampePreferite As New List(Of Integer)({idStampe})
        listaStampePreferite.AddRange(expObj _
                           .Where(Function(row) Not IsDBNull(row("Impostazione_Valore_1")) AndAlso row("Impostazione_Valore_1") <> "") _
                           .SelectMany(Of String)(Function(row) row("Impostazione_Valore_1").ToString.Split("|").ToList) _
                           .Select(Of Integer)(Function(s) CInt(s)).ToList)

        Dim nuovoValore = listaStampePreferite.Distinct _
                .Aggregate(New StringBuilder(), Function(stb, s) stb.Append(s).Append("|"c), Function(stb) stb.Remove(stb.Length - 1, 1).ToString)

        Dim Utenti_Impostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Utenti_Impostazioni_W.Cancella(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, "", objParametri_Utenti)
        Utenti_Impostazioni_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, nuovoValore, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)


        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function eliminaPreferiti(idStampe As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim reportselezionato As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe
        reportselezionato = idStampe

        Dim nuovivalori As String = ""

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUserNotCacheable(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim expObj = DT.ToExpandoObject
        Dim listaStampePreferite = expObj _
                           .Where(Function(row) Not IsDBNull(row("Impostazione_Valore_1")) AndAlso row("Impostazione_Valore_1") <> "") _
                           .SelectMany(Of String)(Function(row) row("Impostazione_Valore_1").ToString.Split("|").ToList) _
                           .Select(Of Integer)(Function(s) CInt(s)) _
                           .Where(Function(x) x <> idStampe).ToList

        Dim nuovoValore = ""
        If listaStampePreferite.Any Then
            nuovoValore = listaStampePreferite _
                .Aggregate(New StringBuilder(), Function(stb, s) stb.Append(s).Append("|"c), Function(stb) stb.Remove(stb.Length - 1, 1).ToString)
        End If
        Dim Utenti_Impostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Utenti_Impostazioni_W.Cancella(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, "", objParametri_Utenti)
        Utenti_Impostazioni_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, nuovoValore, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

        r.RispostaOK = True
        Return r
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codificaStampe"></param>
    ''' <param name="datiStampe"></param>
    ''' <param name="link"></param>
    ''' <param name="errNonGestito"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <param name="TipoMostra"></param>
    ''' <param name="RedirectFiltroRicercaNG">
    '''     Quando TRUE, e Filtro Ricerca (new!) attivato, si apre iFrame per la selezione impianti, altrimenti redirect a Sito Stampe
    ''' </param>
    Public Shared Sub gestisciStampa(codificaStampe As enum_CodificaStampe,
                                     datiStampe As AgronicaCoreModello.DatiStampe,
                                     ByRef link As String,
                                     ByRef errNonGestito As String,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                         Optional ByRef TipoMostra As Enum_TipoMostra_FiltroRicerca = -1,
                                         Optional ByRef RedirectFiltroRicercaNG As Boolean = False)

        Dim objp As New ParametriFILTRONE_2010
        Dim session = HttpContext.Current.Session

        Dim piva As String = objParametriAgenda.Piva

        Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampeBootstrap.aspx", AgroKey_EncoderDecoder)
        Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder)
        Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder)

        Dim Rag_Soc As String = ""
        RedirectFiltroRicercaNG = False

#Region "Controlli Azienda Corrente Selezionata"
        Select Case codificaStampe
            Case enum_CodificaStampe.Esportazione_AnagraficaProdotti,
                 enum_CodificaStampe.PacchettoIgiene_RegistroFornitori,
                 enum_CodificaStampe.PacchettoIgiene_RegistroClienti,
                 enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla,
                 enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento,
                 enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi,
                 enum_CodificaStampe.Registri_Preparazioni,
                 enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea

                Dim utenteAbilitato As Boolean = True

                Select Case codificaStampe
                    Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi,
                         enum_CodificaStampe.Registri_Preparazioni,
                         enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea

                        Dim id_Attivita As enum_Security_Attivita = -1
                        Select Case codificaStampe
                            Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi
                                id_Attivita = enum_Security_Attivita.Report_Accettazione_DaDiversi
                            Case enum_CodificaStampe.Registri_Preparazioni
                                id_Attivita = enum_Security_Attivita.Registri_Cantina
                            Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea
                                id_Attivita = enum_Security_Attivita.Report_Incongruenze_CatastoVSAgrea
                        End Select

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        utenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                              session("ASG_Utente_Username"),
                                              session("ASG_IdServizio"),
                                              id_Attivita,
                                              enum_Security_Operazione.Lettura,
                                              Date.Now, "", objParametri_Utenti)
                End Select

                If utenteAbilitato Then

                    If piva = "" Then
                        link = ""
                        errNonGestito = "Per questa stampa è obbligatorio avere un'azienda selezionata"
                        Exit Sub
                    Else
                        Dim LinguaCorrente As Lingua = CType(session("LinguaCorrente"), Lingua)

                        Dim redirectManager As AgroRedirectManager
                        redirectManager = New AgroRedirectManager(LinguaCorrente, objParametri_Server, HttpContext.Current.Session)

                        Dim strOpenScript As String
                        Dim dt_selected As DataTable

                        redirectManager.aprisitoversione2013(
                            piva,
                            Enum_SiteRedirector.Sito_AgronicaStampe,
                            codificaStampe,
                            "../GestioneStampe/ChiamaStampe.aspx",
                            link,
                            strOpenScript,
                            dt_selected)

                        Dim strSplit As String() = strOpenScript.Split(New Char() {"'"c}, StringSplitOptions.RemoveEmptyEntries)

                        link = strSplit(3)

                        Exit Sub
                    End If

                Else

                    link = ""
                    errNonGestito = "Non si dispone dei permessi di stampa di questo report."

                    Exit Sub

                End If
        End Select
#End Region

        Select Case codificaStampe
#Region "Scheda Campagna"
            Case enum_CodificaStampe.SchedaCampagna_Biologico,
                 enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata,
                 enum_CodificaStampe.Esportatore_Universale_Appezza

                link = ""
                errNonGestito = "Report in fase di costruzione!"
                Exit Sub

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
                 enum_CodificaStampe.SchedaVendite_Biologico,
                 enum_CodificaStampe.SchedaPreparati_Biologico

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_GiasOnline, ParametriAgronicaStampe)

            Case enum_CodificaStampe.PAP_Vegetale

                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                '----- Utente
                UserName = session("ASG_Utente_Username").ToString
                Password = session("ASG_Utente_Password").ToString
                User_Profilo = session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = session("ASG_Utente_CodFiscale")

                If Not piva Is Nothing Then
                    piva = piva.ToString
                End If


                link = RedirectGestione.IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaPagBio.PAP_Vegetale,
                                        session("ASG_Utente_CodFiscale").ToString,
                                        piva)

            Case enum_CodificaStampe.Notifica_Biologico

                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                UserName = session("ASG_Utente_Username").ToString
                Password = session("ASG_Utente_Password").ToString
                User_Profilo = session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = session("ASG_Utente_CodFiscale")

                link = RedirectGestione.IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaPagBio.Notifica,
                                        session("ASG_Utente_CodFiscale").ToString,
                                        piva)

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                 enum_CodificaStampe.SchedaMagazzinoMovimenti,
                 enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                 enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                 enum_CodificaStampe.RiepilogoProdottiUtilizzati

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server,
                                     "",
                                     0,
                                     0,
                                     0,
                                     0,
                                     0)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.Esporta_GiasToSap

                Dim XmlDoc As New System.Xml.XmlDocument

                Dim LinkPaginaStampa As String = "../GestioneStampe/ChiamaStampe.aspx"
                Dim LinkSitoStampe As String = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaStampe.Esporta_GiasToSap,
                                        CStr(session("ASG_Utente_Username")),
                                        CStr(session("ASG_ProgressivoGIAS")),
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "")

            Case enum_CodificaStampe.Costo_Manodopera_XLS

                Dim objGiasOnline As New ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_Manodopera
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & codificaStampe

                link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                        objGiasOnline)

            Case enum_CodificaStampe.Costo_ParcoMacchine_XLS

                Dim objGiasOnline As New ParametriGiasOnline
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_ParcoMacchine
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & codificaStampe

                link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                        objGiasOnline)

                'Case enum_CodificaStampe.Esportazione_OP_Inv


                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Inv), _
                '                                AgroKey_EncoderDecoder, Server)


                'Case enum_CodificaStampe.Esportazione_OP_Gest

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest), _
                '                                AgroKey_EncoderDecoder, Server)


                'Case enum_CodificaStampe.Esportazione_OP_Gest_Coop

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest_Coop), _
                '                                AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.Report_RiconversioneVarietale

                Dim XmlDoc As New System.Xml.XmlDocument

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaStampe.Report_RiconversioneVarietale,
                                        CStr(session("ASG_Utente_Username")),
                                        CStr(session("ASG_ProgressivoGIAS")),
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "")


            Case enum_CodificaStampe.Report_ImpegnoProduzioneSoci

                Dim LinkSitoStampe As String
                Dim XmlDoc As New System.Xml.XmlDocument

                LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaStampe.Report_ImpegnoProduzioneSoci,
                                        CStr(session("ASG_Utente_Username")),
                                        CStr(session("ASG_ProgressivoGIAS")),
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "")


                'Case enum_CodificaStampe.Esportazione_CellulariContatti


                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_CellulariTecnici), _
                '                                AgroKey_EncoderDecoder, Server)


                ' Case enum_CodificaStampe.Esportatore_Universale_Imprese

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Imprese), _
                '                                AgroKey_EncoderDecoder, Server)


                'Case enum_CodificaStampe.Esportatore_Universale_Centri

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Centri), _
                '                                AgroKey_EncoderDecoder, Server)


                'Case enum_CodificaStampe.Esportatore_Universale_Impianti

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Impianti), _
                '                                AgroKey_EncoderDecoder, Server)


                ' Case enum_CodificaStampe.Esportatore_Universale_Agenda

            '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Agenda),
            '                                AgroKey_EncoderDecoder)


                'Case enum_CodificaStampe.Esportatore_Universale_Rintraccio

                '    Dim UtenteAbilitato As Boolean
                '    Dim strDummy As String

                '    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                '    UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
                '                         Session("ASG_Utente_Username"), _
                '                            Session("ASG_IdServizio"), _
                '                                                         enum_Security_Attivita.Stampe_Esportazione_Rintraccio, _
                '                                                         enum_Security_Operazione.Modifica, _
                '                                                          Date.Now, "", objParametri_Utenti)

                '    If UtenteAbilitato = True Then

                '        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Rintraccio), _
                '                                    AgroKey_EncoderDecoder, Server)

                '    Else
                '        r.rispostaStringa = ("Permesso negato!", Page)
                '        Exit Function

                '    End If

#End Region

#Region "Schede Varie x le OP"

            Case enum_CodificaStampe.Impegnative_capitolati

                Dim LinkSitoStampe As String

                LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                        enum_CodificaStampe.Impegnative_capitolati,
                                        CStr(session("ASG_Utente_Username")),
                                        CStr(session("ASG_ProgressivoGIAS")),
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "")

            Case enum_CodificaStampe.Programmazione_Vegetale

                Dim Qs_Piva As String = ""
                Dim UserName As String
                Dim Password As String
                Dim User_Profilo As String
                Dim UserName_CodFisc As String
                Dim UserProfilo_CodFisc As String

                '----- Utente

                UserName = session("ASG_Utente_Username").ToString
                Password = session("ASG_Utente_Password").ToString
                User_Profilo = session("ASG_SuperUser_Username").ToString
                UserProfilo_CodFisc = session("ASG_SuperUser_CodFiscale").ToString

                UserName_CodFisc = session("ASG_Utente_CodFiscale")

                '---  Creazione della stringa XML dei parametri  

                Dim ParametriPlanning As New ParametriPlanning
                ParametriPlanning.PaginaProvenienza = enum_PagineGiasOnline.MenuStampe
                ParametriPlanning.PaginaRichiesta = enum_CodificaPagPlanning.PianificazioneVegetale
                ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, piva)
                ParametriPlanning.Piva = piva

                link = RedirectGestione.IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriPlanning)

            Case enum_CodificaStampe.Esportazione_AnagraficaProdotti
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                    Enum_SiteRedirector.Sito_GiasOnline,
                                    ParametriAgronicaStampe)

                'Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                '    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Contatti), _
                '                                AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.SchedaTracciabilita_Animale

                'imposto la versione ZOO dell'alberoimprese
                session("VersioneAlbero") = enum_VersioneAlberoImprese.Albero_Stalle

                'come pagina di ritorno non metto il menùstampe, ma l'alberoimprese
                'perchè così se l'utente vuole cambiare centro, può farlo facendo exit
                'dalla apgian delle consistenze

                If piva <> "" Then
                    'c'è una sola azienda o l'utente vede una sola azienda
                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.StalleConsistenze_Info
                    objGiasOnline.Piva = objParametriAgenda.Piva

                    link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                            objGiasOnline)
                Else

                    link = RedirectGestione.GetLinkFiltrinoAgenda(
                                            "../Stampe/MenuStampeBootStrap.aspx",
                                            ".../GestioneStalle/StalleConsistenze_Info.aspx",
                                            Enum_SiteRedirector.Sito_AgronicaStampe,
                                            codificaStampe)

                End If

            Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori
                Dim StrNodiVariabili As String = ""
                Dim StrNodo As String = ""
                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = piva
                StrNodo = XML_VariabiliStampe(vVarStampe)
                StrNodiVariabili = StrNodiVariabili & StrNodo

                Dim username As String = session("ASG_Utente_Username")
                Dim user_profilo As String = session("ASG_ProgressivoGIAS")

                Dim user_profilo_codfiscale As String = session("ASG_Utente_CodFiscale")
                Dim Sql_Filtro As String = ""
                Dim XML_Filtro As String = ""
                Dim username_codfisc As String = ""

                Dim utente_codfiscale As String = session("ASG_Utente_CodFiscale")
                Dim superuser_username As String = session("ASG_SuperUser_Username")
                Dim superuser_codfiscale As String = session("ASG_SuperUser_CodFiscale")

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                        codificaStampe,
                                        username,
                                        user_profilo,
                                        StrNodiVariabili,
                                        user_profilo_codfiscale,
                                        Sql_Filtro,
                                        XML_Filtro,
                                        username_codfisc,
                                        utente_codfiscale,
                                        superuser_username,
                                        superuser_codfiscale)

            Case enum_CodificaStampe.PacchettoIgiene_RegistroClienti
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.PacchettoIgiene_SchedaUsoAlimentiOGM,
                 enum_CodificaStampe.PacchettoIgiene_RegistroAnalisiNonConformi
                link = ""
                errNonGestito = "Stampa in fase di manutenzione."
                Exit Sub

            Case enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                    Enum_SiteRedirector.Sito_GiasOnline,
                                    ParametriAgronicaStampe)

            Case enum_CodificaStampe.Registri_Preparazioni
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

            Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea
                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server, Rag_Soc)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)

                'al momento non essendoci un filtro delle operazioni contabili vado al menù agenda
                'la stampa è chiamabile infatti nel menù agenda
            Case enum_CodificaStampe.Bolle, enum_CodificaStampe.Fatture, enum_CodificaStampe.Nota_Accredito

                link = "../menu/menu.aspx"
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                'Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(session("ASG_objParametri_Server"))
                Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                    link = "../Menu/MenuBS_Agenda_Nuovo.aspx"
                End If

            Case enum_CodificaStampe.Registro_Fertilizzazioni_Massivo, enum_CodificaStampe.Registro_Trattamenti_Massivo

                Dim UtenteAbilitato As Boolean

                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                                         session("ASG_Utente_Username"),
                                                         session("ASG_IdServizio"),
                                                        enum_Security_Attivita.Stampa_SchedaCampagna_Massiva,
                                                        enum_Security_Operazione.Lettura,
                                                        Date.Now, "", objParametri_Utenti)

                If UtenteAbilitato = True Then

                    'verifico in configurazione_siti
                    'se devo leggere le aziende da tabella
                    'se devo passare dal filtrone

                    Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dt As DataTable = cf.Leggi(0, "Stampe_Massive_Filtro", " valore = 'false' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))

                    If dt.Rows.Count = 1 Then

                        'devo leggere le aziende da tabella
                        Dim objAgronicaStampe As New ParametriAgronicaStampe
                        objAgronicaStampe.report = codificaStampe

                        'Verifico le stampe che devono avere la specie selezionata
                        Dim strVegCod As String = ""
                        Dim filtroImp As String = ""

                        Dim XmlDoc As New System.Xml.XmlDocument
                        Dim StrVariabiliStampe As String = ""
                        Dim StrNodiVariabili As String = ""
                        Dim StrNodo As String = ""
                        Dim objVS As New AgronicaCoreXML.XML_Stampe

                        objAgronicaStampe.username = CStr(session("ASG_Utente_Username"))
                        objAgronicaStampe.user_profilo = CStr(session("ASG_ProgressivoGIAS"))
                        objAgronicaStampe.Xml_Generico.Length = 0
                        objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

                        Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                               objAgronicaStampe)
                    Else

                        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder)
                        objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder)


                        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder)
                        objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder)


                        objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder)

                        objp.CodificaStampe = Stringa_Codifica(codificaStampe, AgroKey_EncoderDecoder)

                    End If

                Else

                    link = ""
                    errNonGestito = "Non si dispone dei permessi di stampa di questo report."
                    Exit Sub

                End If

            Case enum_CodificaStampe.EsportazioneAgeaTxtCSV

                objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder)
                objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder)


                objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaSincronizzatore, AgroKey_EncoderDecoder)
                objp.Pagina_Destinazione = Stringa_Codifica(enum_PagineAgronicaSincro.EsportazioneAGEATxtCsv, AgroKey_EncoderDecoder)


                objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_AgeaTXTCSV), AgroKey_EncoderDecoder)

                objp.CodificaStampe = Stringa_Codifica(codificaStampe, AgroKey_EncoderDecoder)

#End Region
            Case enum_CodificaStampe.Stampa_Zoo_Dettaglio_Partita,
                     enum_CodificaStampe.Stampa_Zoo_Sintesi_Partite

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                     codificaStampe,
                                     piva,
                                     session,
                                     objParametri_Server,
                                     "",
                                     0,
                                     0,
                                     0,
                                     0,
                                     0)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_GiasOnline,
                                        ParametriAgronicaStampe)


            Case Else
                parametriFiltrone(codificaStampe, objp)
        End Select

        Dim categoriaEsito = ""
        Select Case codificaStampe
            Case enum_CodificaStampe.Esportatore_Universale_Imprese,
                 enum_CodificaStampe.Esportazione_AnagraficaContatti,
                 enum_CodificaStampe.Quadro_P,
                 enum_CodificaStampe.Adesione_Etico_Ambientale,
                 enum_CodificaStampe.Tenuta_Scheda_Campagna,
                 enum_CodificaStampe.Codice_Condotta,
                 enum_CodificaStampe.Adesione_DPI,
                 enum_CodificaStampe.Impegnativa_Eurep,
                 enum_CodificaStampe.Impegnativa_QC,
                 enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                 enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati,
                 enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento,
                 enum_CodificaStampe.Fitoregolatori_Kiwi,
                 enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento,
                 enum_CodificaStampe.Accordo_Responsabilita_di_Filiera,
                 enum_CodificaStampe.Dichiarazione_di_Responsabilita,
                 enum_CodificaStampe.ObiettivoDiProduzioneAsipo
                categoriaEsito = "azienda"
                TipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende
            Case enum_CodificaStampe.Esportatore_Universale_Centri,
                 enum_CodificaStampe.EstrattoreDatiGrafici
                categoriaEsito = "centro"
                TipoMostra = Enum_TipoMostra_FiltroRicerca.CentriAziendali
            Case enum_CodificaStampe.Esportatore_Universale_Agenda
                categoriaEsito = "movimento"
                TipoMostra = Enum_TipoMostra_FiltroRicerca.Movimenti
            Case enum_CodificaStampe.Adesione_Conad,
                   enum_CodificaStampe.Adesione_Despar
                categoriaEsito = "esercizio"
                TipoMostra = Enum_TipoMostra_FiltroRicerca.Esercizi
            Case Else
                TipoMostra = Enum_TipoMostra_FiltroRicerca.Impianti
        End Select

        If codificaStampe = enum_CodificaStampe.ImpegnativaColtivazioneConferimento AndAlso datiStampe.switchGenerale1 Then
            categoriaEsito = "azienda"
            TipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende
        End If

        If link = "" Then

            RedirectFiltroRicercaNG = True

            If Not String.IsNullOrEmpty(datiStampe.anno) Then
                session("annoStampeOP" + objParametri_Utenti.UtenteUsername) = datiStampe.anno
            End If
            If codificaStampe = enum_CodificaStampe.ImpegnativaColtivazioneConferimento AndAlso Not String.IsNullOrEmpty(datiStampe.switchGenerale1) Then
                session("stampaVuota" + objParametri_Utenti.UtenteUsername) = datiStampe.switchGenerale1
            End If

            If codificaStampe = enum_CodificaStampe.ImpegnativaColtivazioneConferimento AndAlso Not String.IsNullOrEmpty(datiStampe.switchGenerale2) Then
                session("fronteRetro" + objParametri_Utenti.UtenteUsername) = datiStampe.switchGenerale2
            End If

            If (codificaStampe = enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento Or
                codificaStampe = enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri Or
                codificaStampe = enum_CodificaStampe.SchedaAziendale Or
                codificaStampe = enum_CodificaStampe.Adesione_ModuloGrasp Or
                codificaStampe = enum_CodificaStampe.Adesione_ProtocolloGlobalGAP Or
                codificaStampe = enum_CodificaStampe.Adesione_NurtureModule Or
                codificaStampe = enum_CodificaStampe.Adesione_Despar Or
                codificaStampe = enum_CodificaStampe.Adesione_Conad Or
                codificaStampe = enum_CodificaStampe.Adesione_StandardLeaf Or
                codificaStampe = enum_CodificaStampe.Accordo_Responsabilita_di_Filiera Or
                codificaStampe = enum_CodificaStampe.Dichiarazione_di_Responsabilita Or
                codificaStampe = enum_CodificaStampe.Fitoregolatori_Kiwi) AndAlso Not String.IsNullOrEmpty(datiStampe.switchGenerale1) Then
                session("fronteRetro" + objParametri_Utenti.UtenteUsername) = datiStampe.switchGenerale1
            End If

            If codificaStampe = enum_CodificaStampe.ObiettivoDiProduzioneAsipo Then
                session("datiStampe") = datiStampe
            End If

            link = "../Filtrone/Filtrone_Nuovo.aspx" &
                             "?p_o=" & objp.Pagina_Origine &
                             "&s_o=" & objp.Sito_Origine &
                             "&p_d=" & objp.Pagina_Destinazione &
                             "&s_d=" & objp.Sito_Destinazione &
                             "&t_f=" & objp.TipoFiltrone &
                             "&c_s=" & objp.CodificaStampe &
                             "&v_c=" & objp.Veg_Cod &
                             "&c_c=" & objp.Cul_Cod &
                             "&d_i=" & objp.Data_Inizio &
                             "&nopiva=" & Str(1)

            If categoriaEsito <> "" Then
                link &= "&cat=" & Stringa_Codifica(categoriaEsito, AgroKey_EncoderDecoder)
            End If
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(piva As String, id_stampa As Integer, datiStampe As AgronicaCoreModello.DatiStampe) As RispostaStandard

        Dim r As New RispostaStandard

        Dim CodificaStampe As enum_CodificaStampe = id_stampa

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim RispostaOK As Boolean
        Dim RispostaStringa As String

        Try

            Dim categoriaEsito = ""
            Dim TipoMostra As Enum_TipoMostra_FiltroRicerca
            Dim link As String = ""
            Dim errNonGestito As String = ""
            Dim RedirectFiltroRicercaNG As Boolean = False

            Dim filtroRicerca = New AgronicaCoreFiltroneBIZ.FiltroRicerca
            If CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Agenda AndAlso filtroRicerca.usaFiltroRicercaNG(objParametri_Utenti) Then

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe = RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(CodificaStampe, piva, HttpContext.Current.Session, objParametri_Server, "", 0, 0, 0, 0, 0)

                link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

            Else
                gestisciStampa(CodificaStampe, datiStampe, link, errNonGestito, objParametri_Server, objParametri_Utenti, TipoMostra, RedirectFiltroRicercaNG)

                If RedirectFiltroRicercaNG Then

                    Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca

                    Dim TipoMostraFiltriRicerca = objFiltroRicerca.GetTipoMostraFiltroRicercaPerStampa(CodificaStampe, datiStampe)
                    Dim TipoComportamentoFiltroRicerca = objFiltroRicerca.GetTipoComportamentoFiltroRicercaPerStampa(CodificaStampe)
                    Dim BlocchiSelezionexStampa = objFiltroRicerca.GetBlocchiSelezionePerStampa(CodificaStampe)

                    Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                        .CodificaStampe = CodificaStampe,
                        .TipoComportamentoFiltroRicercaNG = TipoComportamentoFiltroRicerca,
                        .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                        .PaginaProvenienza = enum_PagineAgenda_2010.Pagina_MenuStampe,
                        .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Impianto),
                        .TipoMostraGestitiChiamante = TipoMostraFiltriRicerca,
                        .BlocchiSelezionexStampa = BlocchiSelezionexStampa
                    }

                    If CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Agenda OrElse
                        CodificaStampe = enum_CodificaStampe.Bilancio_Fertilizzazioni OrElse
                        CodificaStampe = enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato Then
                        parametriFiltroRicercaNG.Piva = piva
                    End If

                    link = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)
                End If
            End If

            If link = "" Then
                RispostaOK = False
                RispostaStringa = errNonGestito
            Else
                RispostaOK = True
                RispostaStringa = link
            End If

            r.RispostaOK = RispostaOK
            r.RispostaStringa = JsonConvert.SerializeObject(New RespFiltroRicercaNG With {.RispostaStringa = RispostaStringa, .RedirectFiltroRicercaNG = RedirectFiltroRicercaNG, .TipoMostra = TipoMostra})

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function WS_Gestisci_Redirect_Stampe(tipoMostra As Integer,
                                                       id_stampa As Integer,
                                                       chiavi As List(Of String),
                                                       datiStampe As DatiStampe) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            r.RispostaStringa = objFiltroRicerca.Gestisci_Redirect_Stampe(tipoMostra, id_stampa, chiavi, datiStampe, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
End Class