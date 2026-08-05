
Imports System.Web.Services
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreProfilazioneBIZ
Imports AgronicaCoreXML.XML_Stampe
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.Menu
Imports System.IO
Imports AgronicaControlli_2010

Public Class MenuBS_2017
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public RedirectPagina As String = ""
    Public FiltroAziende As Boolean = False
    Public LenFiltroAziende As String = "0"
    Public DSSDifesa_Autorizzato As Boolean

#Region "Web service"
    <WebMethod(EnableSession:=True)>
    Public Shared Function redirectGiasNG(ByVal paginaRichiesta As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim redirectUrl As String = ""
        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG, paginaRichiesta, redirectUrl, objParametri_Server)
        r.RispostaOK = True
        r.Tipo = 0
        r.RispostaStringa = redirectUrl
        Return r
    End Function
    'salvataggio dati dell'azienda selezionata con redirect alla pagina serivizio
    <WebMethod(EnableSession:=True)>
    Public Shared Function cambiaImpresaConGestioneRedirect(ByVal Piva As String, ByVal Azienda As String, ByVal IDSezione As Integer) As RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Piva = Piva
        objParametriAgenda.RagSoc = Azienda
        objParametriAgenda.salva()
        HttpContext.Current.Session("_Piva") = Piva
        Return salvaTitoloSezioneConGestioneRedirect(IDSezione)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiungiBreadcrumbs(ByVal paginaRichiesta As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim redirectUrl As String = ""
        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG, paginaRichiesta, redirectUrl, objParametri_Server)
        r.RispostaOK = True
        r.RispostaStringa = redirectUrl
        Return r
    End Function

    'salvataggio dell'oggetto con i dati della sezione selezionata da utilizzare nei redirect
    <WebMethod(EnableSession:=True)>
    Public Shared Function salvaTitoloSezioneConGestioneRedirect(ByVal IDSezione As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim apiController As CoreApiControllerFactory = New CoreApiControllerFactory
            apiController.Inizializza(objParametri_Super_Server, objParametri_Server)

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu

            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
            Dim dtLettura As DataTable

            'lo utilizzo quando faccio redirect
            'Dim oggettoSalvatoPerRedirect As JObject = JObject.Parse(oggetto)
            Dim IDSezionePadre As Integer
            Dim testo As String
            Dim colore As String
            Dim classeCSS As String
            Dim sitoRichiesto As Integer
            Dim RedirectURL As String = ""
            Dim paginaRichiesta As String
            Dim aziendaRichiesta As Integer
            Dim tipoAperturaPagina As Integer
            Dim configurazione As String = ""

            'Funzione che decide se andare sul menuAgenda nuovo o quello vecchio, attualemente lascio andare su entrambi
            'DecideSectionIDBasedOnConfigurazioneSitiForMenuAgenda(IDSezione)
            dtLettura = lettureDB.LeggiSezione(IDSezione, 2, objParametri_Server, objParametri_Utenti)


            If Not IsNothing(dtLettura) AndAlso dtLettura.Rows.Count > 0 Then

                Dim riga = dtLettura.Rows(0)
                testo = riga.Item("Testo")
                colore = riga.Item("Colore")
                classeCSS = riga.Item("ClasseCSS")
                sitoRichiesto = riga.Item("Enum_SiteRedirector")
                paginaRichiesta = riga.Item("PaginaRichiesta")
                aziendaRichiesta = riga.Item("RichiedeAziendaSelezionata")
                If Not IsDBNull(riga.Item("RedirectURL")) Then
                    RedirectURL = riga.Item("RedirectURL")
                End If
                If Not IsDBNull(riga.Item("Enum_TipoAperturaPagina")) Then
                    tipoAperturaPagina = riga.Item("Enum_TipoAperturaPagina")
                End If
                If Not IsDBNull(riga.Item("Configurazione")) Then
                    configurazione = riga.Item("Configurazione")
                End If

                IDSezione = riga.Item("IDSezione")
                IDSezionePadre = riga.Item("IDSezionePadre")

                ' fix per saltare alla pagina gestione analisi nei PDC se attiva la nuova gestione
                If IDSezione = enum_Sezioni_MenuBS_2017.Inserimento_Analisi_Laboratorio OrElse IDSezione = enum_Sezioni_MenuBS_2017.PianiCampionamentiAnalisi Then
                    Dim objDefault As New Utenti_Impostazioni_Read
                    Dim dtImpostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SuperUser_Gestione_PDC_Analisi, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
                    If dtImpostazioni.Rows.Count > 0 AndAlso dtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = "1" Then
                        If IDSezione = enum_Sezioni_MenuBS_2017.Inserimento_Analisi_Laboratorio Then
                            sitoRichiesto = Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                            paginaRichiesta = enum_PaginePianiCampionamento.GestioneAnalisi
                        Else
                            tipoAperturaPagina = 0
                        End If
                    End If
                End If

                If Not IsNothing(IDSezionePadre) Then
                    dtLettura = lettureDB.LeggiSezione(IDSezionePadre, 1, objParametri_Server, objParametri_Utenti)
                    If Not IsNothing(dtLettura) AndAlso dtLettura.Rows.Count > 0 Then
                        If testo <> dtLettura.Rows(0).Item("Testo") Then
                            testo = dtLettura.Rows(0).Item("Testo") & "_" & testo
                        End If
                    End If
                End If

                Dim sezione As New JObject()
                sezione.Add("testo", testo)
                sezione.Add("colore", colore)
                sezione.Add("classeCSS", classeCSS)
                sezione.Add("IDSezione", IDSezione)
                sezione.Add("IDSezionePadre", IDSezionePadre)
                sezione.Add("sitoRichiesto", sitoRichiesto)
                sezione.Add("RedirectURL", RedirectURL)
                sezione.Add("paginaRichiesta", paginaRichiesta)
                sezione.Add("aziendaRichiesta", aziendaRichiesta)
                sezione.Add("tipoAperturaPagina", tipoAperturaPagina)
                sezione.Add("configurazione", configurazione)
                Dim oggetto As String = JsonConvert.SerializeObject(sezione)

                HttpContext.Current.Session("ASG_MenuBS_2017") = oggetto

                ' redirect generico
                If sitoRichiesto <> 0 AndAlso (String.IsNullOrEmpty(RedirectURL) OrElse Not RedirectURL.Contains("..")) Then
                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, sitoRichiesto, paginaRichiesta, RedirectURL, objParametri_Server)
                End If

                ' Se non sto andando su zootecnica (contiene defaulttab=5 nel RedirectURL) allora controllo se sto andando su QdC vecchio (idSezione = 7) o su ricette (stesa pagina defaulttab diverso)
                ' Per quanto riguarda le anagrafiche il redirect di quello viene gestito nel GestioneRichieste, questo perche' non avendo un redirectURL verra fatto sempre un redirect al gestionerichieste che si occupera di aprire le anagrafiche
                If Not String.IsNullOrEmpty(RedirectURL) AndAlso Not RedirectURL.ToLowerInvariant().Contains("defaulttab=5") Then
                    If (IDSezione = 7 OrElse RedirectURL.ToLowerInvariant().Contains("/menu/menubs_agenda_nuovo.aspx")) AndAlso PermessoRedirectMenuAgendaNG(objParametri_Server, objParametri_Utenti) Then
                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                      Enum_SiteRedirector.GiasNG,
                                                                      enum_PagineGiasNG.Pagina_Menu_Agenda,
                                                                      RedirectURL,
                                                                      objParametri_Server)
                    End If
                End If

                ' gestione redirect specifico per la sezione
                If sitoRichiesto = Enum_SiteRedirector.Sito_AgronicaStampe_2010 Then
                    GestioneRedirectStampe(paginaRichiesta, objParametriAgenda.Piva, RedirectURL)
                Else
                    GestioneRedirectSezione(IDSezione, objParametriAgenda.Piva, RedirectURL)
                End If

                If tipoAperturaPagina = 1 Then
                    r.Tipo = "1"
                ElseIf tipoAperturaPagina = 2 Then
                    RedirectURL = RedirectGestione.PreparaScripPerPopup(RedirectURL, testo)
                    r.Tipo = "1"
                ElseIf tipoAperturaPagina = 3 Then
                    Dim titolo = Replace(testo, "_", " - ")
                    Dim actions = "actions: ['Maximize','Close']"
                    Dim options = "width: '1000px',  height: '700px', title: '" & titolo & "', content: '" & RedirectURL
                    RedirectURL = "<script>$('#popupGias').kendoWindow({" & options & "', " & actions & "});</script>"
                    RedirectURL &= "<div id='popupGias'></div>"
                    r.Tipo = "1"
                End If

            Else

                If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                  Enum_SiteRedirector.GiasNG,
                                                                  enum_PagineGiasNG.Pagina_Dashboard,
                                                                  RedirectURL,
                                                                  objParametri_Server)
                Else
                    RedirectURL = "../Menu/MenuBS_2017.aspx"
                End If

            End If

            If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
                RedirectURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(RedirectURL, "idBC", Stringa_Codifica(IDSezione.ToString, AgroKey_EncoderDecoder))
                If tipoAperturaPagina <> 0 Then
                    RedirectURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(RedirectURL, "sidebar", "off")
                End If
            End If

            r.RispostaOK = True
            r.RispostaStringa = RedirectURL

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Function PermessoRedirectMenuAgendaNG(ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim Permesso_Lettura = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Agenda_AccessoMenu_NG,
                enum_Security_Operazione.Lettura,
                Date.Now, "", objParametri_Utenti)


        If objConfSiti.Leggi_Valore(0, "MenuAgendaNG", "", "", objParametri_Server) = "true" AndAlso
            Permesso_Lettura Then
            Return True
        End If
        Return False

    End Function

    Private Shared Sub DecideSectionIDBasedOnConfigurazioneSitiForMenuAgenda(ByRef sectionID As Integer)
        Const IDMenuAgenda = 7
        Const IDMenuAgendaNG = 260

        If sectionID <> IDMenuAgenda Then
            Return
        End If

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim gotoMenuAgendaNG = objConfSiti.LeggiValoreAsBooleanType("MenuAgendaNG", objParametri_Server)

        If gotoMenuAgendaNG Then
            sectionID = IDMenuAgendaNG
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImprese(ByVal ricerca As String, ByVal idSezione As Integer) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))
        r.RispostaStringa = New List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiImprese(ricerca, idSezione, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiSezioni(ByVal IDTipoSezione As Integer,
                                        ByVal IDSezionePadre As Integer,
                                        ByVal Ricerca As String,
                                        ByVal Preferiti As String
                                        ) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))
        r.RispostaStringa = New List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim ConfigMenu = LeggiConfigMenu(objParametri_Server)
            Dim NascondiMenu = GetConfigMenu(ConfigMenu, "nascondiMenu", True)
            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiSezioni(IDTipoSezione, IDSezionePadre, Ricerca, Preferiti, NascondiMenu, objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiSezioniPreferiti(ByVal IDSezionePadre As Integer, ByVal Ricerca As String) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))
        r.RispostaStringa = New List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiSezioniPreferiti(IDSezionePadre, Ricerca, objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiWidgetAllarmi() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiWidgetAllarmi(objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ScegliPreferiti() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        'indice del pulsante dei preferiti
        Dim indicePreferito As String = ""
        'array per gli indici dei preferiti
        Dim indici() As String

        Try
            'Inserire il codice QUI..
            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            'per prendere le letture dei database
            Dim dtLettura As DataTable
            Dim dtPreferiti As DataTable

            'menu
            Dim IDSezione As Integer
            Dim colore As String
            Dim idhtml As String
            Dim testo As String
            Dim classecss As String

            Dim stringaHTML As New System.Text.StringBuilder

            stringaHTML.Length = 0

            stringaHTML.AppendLine("<div Class='row'>")
            'tutte le sezioni
            stringaHTML.AppendLine("<div class='col-sm-6' id='sottocategoria_tutte'>")
            'letto tutte le sezioni
            dtLettura = lettureDB.LeggiSezioni(0, 0, "", objParametri_Server, objParametri_Utenti)
            'composizione del codice html che ospita le sezioni da scegliere
            If Not IsNothing(dtLettura) Then
                For Each riga As DataRow In dtLettura.Rows
                    IDSezione = riga.Item("IDSezione")
                    colore = riga.Item("Colore")
                    testo = riga.Item("Testo")
                    classecss = riga.Item("ClasseCSS")
                    idhtml = riga.Item("id_html")

                    stringaHTML.AppendLine("<div id='" & idhtml & "_blocco'>")
                    stringaHTML.AppendLine("<i id='" & idhtml & "_blocco_arr_left' Class='fa fa-arrow-circle-left fa-2x' style='color: black; cursor: pointer; display:none;' onClick='delPref(&quot;" & idhtml & "_blocco&quot;)'></i>")
                    stringaHTML.AppendLine("<div class='btn btn-default " & classecss & " disattivato' id='" & idhtml & "' style='background-color:" & colore & "' aria-richiedeaziendaselezionata='true' aria-sitorichiesto='' aria-paginarichiesta='' aria-idsezione='" & IDSezione & "' aria-redirecturl='' onClick=''>")
                    stringaHTML.AppendLine("<span class>")
                    stringaHTML.AppendLine("<span class></span>")
                    stringaHTML.AppendLine("<span>" & testo & "</span>")
                    stringaHTML.AppendLine("</span>")
                    stringaHTML.AppendLine("</div>")
                    stringaHTML.AppendLine("<i id='" & idhtml & "_blocco_arr_right' Class='fa fa-arrow-circle-right fa-2x' style='color: black; cursor: pointer;' onClick='setPref(&quot;" & idhtml & "_blocco&quot;)'></i>")
                    stringaHTML.AppendLine("</div>")
                Next

            Else
                stringaHTML.Append("<h1> " & AgronicaAgenda_2010.ErroreNellaLetturaDelleSezioni & " </h1>")
            End If
            stringaHTML.AppendLine("</div>")

            'sezioni scelte
            stringaHTML.AppendLine("<div class='col-sm-6' id='sottocategoria_scelti'>")
            'qui ci vanno le sezioni scelte
            dtPreferiti = letturaPreferiti.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If (Not IsNothing(dtPreferiti) And (dtPreferiti.Rows.Count > 0)) Then
                For Each preferito As DataRow In dtPreferiti.Rows
                    indicePreferito = preferito.Item("Impostazione_Valore_1")
                    'ora che ho l'elenco degli indici dei preferiti vado a prendere solamente quelli che mi interessano tra la lista degli altri
                    indici = indicePreferito.Split("|")
                    'ora vado a prendere le sezioni per ogni indice che avevo in memoria
                    For Each index As Integer In indici
                        'leggo il db delle sezioni per ogni indice
                        dtLettura = lettureDB.LeggiSezione(index, 2, objParametri_Server, objParametri_Utenti)
                        If Not IsNothing(dtLettura) Then
                            For Each riga As DataRow In dtLettura.Rows
                                IDSezione = riga.Item("IDSezione")
                                colore = riga.Item("Colore")
                                testo = riga.Item("Testo")
                                classecss = riga.Item("ClasseCSS")
                                idhtml = riga.Item("id_html")

                                stringaHTML.AppendLine("<div id='" & idhtml & "_blocco'>")
                                stringaHTML.AppendLine("<i id='" & idhtml & "_blocco_arr_left' Class='fa fa-arrow-circle-left fa-2x' style='color: black; cursor: pointer;' onClick='delPref(&quot;" & idhtml & "_blocco&quot;)'></i>")
                                stringaHTML.AppendLine("<div class='btn btn-default " & classecss & " disattivato' id='" & idhtml & "' style='background-color:" & colore & "' aria-richiedeaziendaselezionata='true' aria-sitorichiesto='' aria-paginarichiesta='' aria-idsezione='" & IDSezione & "' aria-redirecturl='' onClick=''>")
                                stringaHTML.AppendLine("<span class>")
                                stringaHTML.AppendLine("<span class></span>")
                                stringaHTML.AppendLine("<span>" & testo & "</span>")
                                stringaHTML.AppendLine("</span>")
                                stringaHTML.AppendLine("</div>")
                                stringaHTML.AppendLine("<i id='" & idhtml & "_blocco_arr_right' Class='fa fa-arrow-circle-right fa-2x' style='color: black; cursor: pointer; display:none;' onClick='setPref(&quot;" & idhtml & "_blocco&quot;)'></i>")
                                stringaHTML.AppendLine("</div>")
                            Next

                        Else
                            stringaHTML.Append("<h1> " & AgronicaAgenda_2010.ErroreNellaLetturaDelleSezioni & " </h1>")
                        End If
                    Next
                Next
            Else
                stringaHTML.Append("<h3 id='err_no_pref' > " & AgronicaAgenda_2010.NessunaSezionePreferitaInMemoria & " </h3>")
            End If
            'chiusura div per i preferiti in memoria
            stringaHTML.AppendLine("</div>")
            'chiudura del div che contiene le due sezioni, il row
            stringaHTML.AppendLine("</div>")

            'ritorno il div html appena completato
            r.RispostaOK = True
            r.RispostaStringa = stringaHTML.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'salvataggio delle sezioni preferite: se passo dei dati validi salvo il record, se passo "nothing" cancello tutto
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaPreferiti(ByVal listaPreferiti As String, ByVal modalitaScrittura As String) As RispostaStandard

        Dim r As New RispostaStandard

        'stringa contenente la lista degli indici delle sezioni preferite, per elaborazioni
        Dim listaPreferitiSenzaUltimoPipe = listaPreferiti.Substring(0, listaPreferiti.Length - 1)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Try
            'vado a procedere con la scrittura solamente se ho dati
            If listaPreferiti <> "nothing" Then
                'se sono nella SCRITTURA di un NUOVO RECORD
                If modalitaScrittura = "scrivi" Or modalitaScrittura = "aggiorna" Then

                    'se aggiorna cancello prima le impostazioni
                    If modalitaScrittura = "aggiorna" Then
                        r.RispostaOK = letturaPreferiti.Cancella(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                       "",
                                                       objParametri_Utenti)
                    End If

                    'controllo se la scrittura va a buon fine
                    r.RispostaOK = letturaPreferiti.Scrivi(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                       listaPreferitiSenzaUltimoPipe,
                                                       "",
                                                       "",
                                                       "",
                                                       CostantiPersonalizzate.AGRODATAINIZIO,
                                                       CostantiPersonalizzate.AGRODATAFINE,
                                                       objParametri_Utenti
                                                       )

                    'imposto la risposta sotto forma di stringa
                    If r.RispostaOK = True Then
                        r.RispostaStringa = "true"
                    Else
                        r.RispostaStringa = AgronicaAgenda_2010.ProblemaNelSalvataggioDeiPreferiti
                    End If
                    'se sono nella MODIFICA di un RECORD ESISTENTE e non nella scrittura di un nuovo record
                ElseIf modalitaScrittura = "modifica" Then

                    'controllo della scrittura andata a buon fine
                    r.RispostaOK = letturaPreferiti.Modifica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                             listaPreferitiSenzaUltimoPipe,
                                                             "",
                                                             "",
                                                             "",
                                                             CostantiPersonalizzate.AGRODATAINIZIO,
                                                             CostantiPersonalizzate.AGRODATAFINE,
                                                             objParametri_Utenti
                                                             )
                    'imposto la risposta sotto forma di stringa
                    If r.RispostaOK = True Then
                        r.RispostaStringa = "true"
                    Else
                        r.RispostaStringa = AgronicaAgenda_2010.ProblemaNellUpgradeDeiPreferiti
                    End If
                Else
                    'nel caso in cui non sia modifica o scrittura allora ho dei problemi e mando un errore
                End If
                'se sono nella CANCELLAZIONE DEL RECORD
            Else
                'se avevo il nothing da salvare allora vado a cancellare il record
                r.RispostaOK = letturaPreferiti.Cancella(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                       "",
                                                       objParametri_Utenti)

                'imposto la risposta sotto forma di stringa
                If r.RispostaOK = True Then
                    r.RispostaStringa = "true"
                Else
                    r.RispostaStringa = AgronicaAgenda_2010.ProblemaNellaCancellazioneDeiPreferiti
                End If
            End If

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Function GetConfigMenuBS2017(ByVal Chiave As String, ByVal DefVal As Object) As Object
        Dim Config = Master.config_MenuBS_2017
        If Not IsNothing(Config) Then
            If Not IsNothing(Config.Property(Chiave)) Then
                Return Config.GetValue(Chiave)
            End If
        End If
        Return DefVal
    End Function

    Private Sub MenuBS_2017_Load(sender As Object, e As EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim apiController As New CoreApiControllerFactory()
        apiController.Inizializza(objParametri_Super_Server, objParametri_Server)

        meteo.Meteo_headerPlaceHeader = Meteo_headerPlaceHeader
        Master.flag_pag_MenuBS_2017 = True
        FiltroAziende = Master.flag_MostraBtnCambiaImpresa
        LenFiltroAziende = GetConfigMenuBS2017("filtroAziende", "3")
        RedirectPagina = ""

        ' se utente monoazienda imposto l'azienda selezionata
        Dim objParametriAgenda As New ParametriAgenda
        If Not FiltroAziende Then
            objParametriAgenda.Piva = Master.piva_MenuBS_2017
        End If

        ' redirect alla pagina del menu al ritorno dal filtro azienda
        If Not IsNothing(Request.QueryString("IDSezione")) AndAlso Request.QueryString("IDSezione") <> "0" Then
            Session("IDSezione") = CInt(Request.QueryString("IDSezione"))
            Session("FiltroAziende") = Not IsNothing(Request.QueryString("FiltroAziende"))
            If Not IsNothing(Request.QueryString("sidebar")) Then
                Session("siderbar") = False
            End If
            Response.Redirect("MenuBS_2017.aspx")
        End If

        Dim Parametri_Aggiuntivi As New JObject

        'Lorenzo 26/09/2023
        'Se ho abilitato il nuovo header (quindi apro la dashboard di NG) allora imposto come Sito_provenienza = Enum_SiteRedirector.GiasNG mentre
        'come Pagina_Provenienza = enum_PagineGiasNG.Pagina_Dashboard (per evitare da angular di tornare indietro nel vecchio MenuBS_2017.aspx)
        If apiController.VersioneHeader = "2022" Then
            Parametri_Aggiuntivi.Item("Pagina_Provenienza") = enum_PagineGiasNG.Pagina_Dashboard
        End If

        ' se impostata IDSezione in session salta alla pagina relativa
        If Not IsNothing(Session("IDSezione")) AndAlso Session("IDSezione") <> 0 Then

            Dim RedirectURL = ""

            If Not IsNothing(Session("FiltroAziende")) AndAlso Session("FiltroAziende") Then
                RedirectURL = GetFiltroAziende(Session("IDSezione"))
            Else
                Dim risposta
                If (Session("IDSezione")) = enum_PagineGiasNG.Pagina_Dashboard Or (Session("IDSezione")) = enum_PagineGiasNG.Pagina_Gestione_Preferiti Then
                    risposta = redirectGiasNG(Session("IDSezione"))
                Else
                    risposta = salvaTitoloSezioneConGestioneRedirect(Session("IDSezione"))
                End If

                If risposta.RispostaOK Then
                    RedirectURL = risposta.RispostaStringa
                End If

                If risposta.Tipo = "1" AndAlso Not RedirectURL.Contains("<script") Then
                    If apiController.VersioneHeader = "2022" AndAlso Not IsNothing(Session("siderbar")) Then
                        RedirectURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(RedirectURL, "sidebar", "off")
                    End If
                    RedirectURL = "<script>window.open('" & RedirectURL & "');</script>"
                End If
            End If

            ' resetto le varibili di sessione
            Session("IDSezione") = Nothing
            Session("FiltroAziende") = Nothing
            Session("siderbar") = Nothing

            If Not String.IsNullOrEmpty(RedirectURL) Then
                If Not RedirectURL.Contains("<script") Then
                    Response.Redirect(RedirectURL)
                Else
                    RedirectPagina = RedirectURL
                    If apiController.VersioneHeader = "2022" Then
                        Dim linkDashboard = ""
                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Dashboard, linkDashboard, objParametri_Server,
                                                                        Parametri_Aggiuntivi:=Parametri_Aggiuntivi, SitoOrigine:=Enum_SiteRedirector.GiasNG)
                        RedirectPagina &= "<script>window.location = '" & linkDashboard & "';</script>"
                    End If
                    'Response.Write(RedirectURL)
                End If
            End If

        End If

        If RedirectPagina = "" And Not (objParametri_Super_Server Is Nothing) And Not (objParametriAgenda Is Nothing) And Not (objParametri_Server Is Nothing) Then

            If apiController.VersioneHeader = "2022" Then
                Dim RedirectURL = ""
                MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva, Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Dashboard, RedirectURL, objParametri_Server,
                                                              Parametri_Aggiuntivi:=Parametri_Aggiuntivi, SitoOrigine:=Enum_SiteRedirector.GiasNG)
                Response.Redirect(RedirectURL)
            End If
        End If

    End Sub

    Public Shared Sub GestioneRedirectSezione(ByVal IDSezione As Integer, ByVal Piva As String, ByRef RedirectURL As String)

        Dim Origine As String = "../Menu/MenuBS_2017.aspx"

        Select Case IDSezione

            Case enum_Sezioni_MenuBS_2017.QDC_Verifica_Conformita_Quaderno,
                 enum_Sezioni_MenuBS_2017.QUALITA_Verifica_Conformita_Quaderno
                ' Verifica conformità quaderno

                Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare
                Elemento_Verifica_Disciplinare.Piva = Piva
                HttpContext.Current.Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                'RedirectURL = "../GestioneDisciplinari/Verifica_DisciplinareBS.aspx"

            Case enum_Sezioni_MenuBS_2017.AUDIT_Corpi_Estranei
                ' Corpi Estranei

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case enum_Sezioni_MenuBS_2017.CDG_Inserimento_Costi,
                 enum_Sezioni_MenuBS_2017.BUDGET_Inserimento_Costi,
                 enum_Sezioni_MenuBS_2017.CDG_AttivitaInterne
                ' Inserimento Costi

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&entrata_diretta=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                    "&costi_ricavi=" & Stringa_Codifica("costi", AgroKey_EncoderDecoder, Nothing)

                If IDSezione = enum_Sezioni_MenuBS_2017.BUDGET_Inserimento_Costi Then
                    RedirectURL &= "&Budget=1"
                End If

                If IDSezione = enum_Sezioni_MenuBS_2017.CDG_AttivitaInterne Then
                    RedirectURL &= "&AttivitaInterna=1"
                End If

            Case enum_Sezioni_MenuBS_2017.CDG_Analisi_CostiRicavi,
                 enum_Sezioni_MenuBS_2017.BUDGET_Analisi_CostiRicavi
                ' Analisi Costi e Ricavi

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                               "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

                If IDSezione = enum_Sezioni_MenuBS_2017.BUDGET_Analisi_CostiRicavi Then
                    RedirectURL &= "&Budget=1"
                End If

            Case enum_Sezioni_MenuBS_2017.CDG_Menu_CdG,
                 enum_Sezioni_MenuBS_2017.BUDGET_Menu_CdG
                ' Menu CdG

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

                If IDSezione = enum_Sezioni_MenuBS_2017.BUDGET_Menu_CdG Then
                    RedirectURL &= "&Budget=1"
                End If

            Case enum_Sezioni_MenuBS_2017.CDG_Time_Sheet,
                 enum_Sezioni_MenuBS_2017.CDG_Time_Sheet_Personale
                ' Scarico tempi

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

                If IDSezione = enum_Sezioni_MenuBS_2017.CDG_Time_Sheet_Personale Then
                    RedirectURL &= "&Personale=1"
                End If

            Case enum_Sezioni_MenuBS_2017.CDG_Split
                ' Split

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case enum_Sezioni_MenuBS_2017.CDG_Inserimento_Ricavi,
                 enum_Sezioni_MenuBS_2017.BUDGET_Inserimento_Ricavi
                ' Inserimento Ricavi

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&entrata_diretta=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                    "&costi_ricavi=" & Stringa_Codifica("ricavi", AgroKey_EncoderDecoder, Nothing)

                If IDSezione = enum_Sezioni_MenuBS_2017.BUDGET_Inserimento_Ricavi Then
                    RedirectURL &= "&Budget=1"
                End If

            Case enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Acquisto,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Ricevuto,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Distinta_Carico,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_AutoDDT_Emesso,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuova_fattura_Ricevuta,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Ricevuta,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Vendita,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Emesso,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Fattura_Emessa,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Emessa,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento_Pomodoro,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Carico_Magazzino,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Scarico_Magazzino,
                 enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Contratto_Affitto

                ' Documenti Acquisto / Vendita / Movimenti Magazzino / Contratti Affitto

                Dim configDocContabile As New Dictionary(Of Integer, DocContabile_Menu) From {
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Acquisto, New DocContabile_Menu(LAVCOD_ORDINE_ACQUISTO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Ordine)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Ricevuto, New DocContabile_Menu(LAVCOD_BOLLA_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Distinta_Carico, New DocContabile_Menu(LAVCOD_DISTINTA_CARICO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_AutoDDT_Emesso, New DocContabile_Menu(LAVCOD_AUTO_DDT_EMESSO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_fattura_Ricevuta, New DocContabile_Menu(LAVCOD_FATTURA_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Fattura)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Ricevuta, New DocContabile_Menu(LAVCOD_NOTA_ACCREDITO_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Fattura)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Vendita, New DocContabile_Menu(LAVCOD_ORDINE_VENDITA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Ordine)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Emesso, New DocContabile_Menu(LAVCOD_BOLLA_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Consegna)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Fattura_Emessa, New DocContabile_Menu(LAVCOD_FATTURA_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Fattura)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Emessa, New DocContabile_Menu(LAVCOD_NOTA_ACCREDITO_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Fattura)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento, New DocContabile_Menu(0, DocContab_TipoRicerca_Conferimenti, DocContab_TipoDoc_Consegna)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento_Pomodoro, New DocContabile_Menu(0, DocContab_TipoRicerca_Conferimenti, DocContab_TipoDoc_Pomodoro)},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Carico_Magazzino, New DocContabile_Menu(LAVCOD_CARICO, "", "")},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Scarico_Magazzino, New DocContabile_Menu(LAVCOD_SCARICO, "", "")},
                    {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Contratto_Affitto, New DocContabile_Menu(LAVCOD_CONTRATTO_AFFITTO, DocContab_TipoRicerca_Contratti, DocContab_TipoDoc_ContrattoAffitto)}
                }

                Dim tipo As String = If(IDSezione = enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento OrElse
                                        IDSezione = enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento_Pomodoro, "2", "0")

                Dim stringaCodificaOrig As String = ""

                If IDSezione = enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Carico_Magazzino OrElse
                   IDSezione = enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Scarico_Magazzino Then
                    stringaCodificaOrig = Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder)
                Else
                    stringaCodificaOrig = Stringa_Codifica(enum_PagineAgenda_2010.Pagina_RicercaDocContabili, AgroKey_EncoderDecoder)
                End If

                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder) &
                               "&o=" & Stringa_Codifica("1", AgroKey_EncoderDecoder) &
                               "&s=" & Stringa_Codifica("0", AgroKey_EncoderDecoder) &
                               "&i=" & Stringa_Codifica("0", AgroKey_EncoderDecoder) &
                               "&l=" & Stringa_Codifica(configDocContabile.Item(IDSezione).LavCod, AgroKey_EncoderDecoder) &
                               "&md=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) &
                               "&orig=" & stringaCodificaOrig &
                               "&ricercatype=" & configDocContabile.Item(IDSezione).RicercaType &
                               "&ricercadoc=" & configDocContabile.Item(IDSezione).RicercaDoc

                    '"&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder)

            Case enum_Sezioni_MenuBS_2017.CDG_Analisi_costi_diretti_1

                Dim objParametriAgenda As New ParametriAgenda
                objParametriAgenda.Impianti.Clear()

                RedirectURL = "../Filtrone/Filtrone_nuovo.aspx?" &
                    "p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder, Nothing) &
                    "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                    "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_AnalisiCosti, AgroKey_EncoderDecoder, Nothing) &
                    "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                    "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.AnalisiCosti, AgroKey_EncoderDecoder, Nothing) &
                    "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                    "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Nothing)

            Case enum_Sezioni_MenuBS_2017.QDC_PROFITOSAN

                Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
                Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                RedirectURL = profitosan.getLinkSimple(True, HttpContext.Current.Request, New AgroWebConfig, objParametri_Server, objParametri_Utenti)

            Case enum_Sezioni_MenuBS_2017.MAGAZZINI_Nuovo_Trasferimento

                'la piva è già inclusa nel link sul db
                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder) &
                               "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder)

                '"&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder)

            Case enum_Sezioni_MenuBS_2017.MENU_NUOVA_VISITE
                RedirectURL = Visite_Lista.ottieniLinkNuovaOperazione()

            Case Else

                RedirectURL = RedirectURL.Replace("[piva]", Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session))

        End Select

        If RedirectURL = "../Gis/Gis.aspx" Then

            ' VAnni: 18/3/2019: se mi trovo in un contesto di mappatura sementi devo tenere conto degli sportelli

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim leggiSementi As New AgronicaCoreSementieriDAL.Mappatura_Specie_R
            Dim dtSementi As DataTable =
                leggiSementi.Leggi("", "", objParametri_Server)

            If dtSementi.Rows.Count > 0 Then
                HttpContext.Current.Session.Remove("Sementi")
                HttpContext.Current.Session.Remove("DatiPassaggio")
                HttpContext.Current.Session.Remove("AggiornaLayer_FiltroAggiuntivo")
                HttpContext.Current.Session("SementiMappaturaLibera") = "1"
            End If

        End If

        If String.IsNullOrEmpty(RedirectURL) Then
            RedirectURL = "../Menu/MenuBS_2017.aspx"
        End If

    End Sub

    Private Class DocContabile_Menu
        Public Property LavCod As Integer
        Public Property RicercaType As String
        Public Property RicercaDoc As String

        Public Sub New(lavCod As Integer, ricercaType As String, ricercaDoc As String)
            Me.LavCod = lavCod
            Me.RicercaType = ricercaType
            Me.RicercaDoc = ricercaDoc
        End Sub
    End Class

    Public Shared Sub GestioneRedirectStampe(ByVal Report As enum_CodificaStampe, ByVal Piva As String, ByRef RedirectURL As String)

        Select Case Report
            'Se in futuro qualcuno dovesse aggiungere una stampa che non va direttamente sulle stampe ma sul agenda, bisogna cambiare il select case
            'della funzione gestioneRedirectFiltrone dentro GiasNG_Redirect.vb nel CoreGestioneRichieste
            Case enum_CodificaStampe.PianoColturaleCatasto
                RedirectURL = "../Statistiche/Anagrafica/InvestimentoCatasto.aspx"

            Case enum_CodificaStampe.ReportRisultatoFilrone
                Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
                objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, Nothing)
                objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                RedirectURL = "../Filtrone/Filtrone_Nuovo.aspx?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine &
                           "&nopiva=1"

            Case enum_CodificaStampe.ReportRisultatoFiltroneG2G,
                 enum_CodificaStampe.PianoColturale,
                 enum_CodificaStampe.ReportRisultatoFiltroneIncludiVisita

                Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
                objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, Nothing)
                objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                RedirectURL = "../Filtrone/Filtrone_Nuovo.aspx?p_o=" & objp.Pagina_Origine &
                                   "&s_o=" & objp.Sito_Origine &
                                   "&p_d=" & objp.Pagina_Destinazione &
                                   "&s_d=" & objp.Sito_Destinazione &
                                   "&t_f=" & objp.TipoFiltrone &
                                   "&c_s=" & objp.CodificaStampe &
                                   "&v_c=" & objp.Veg_Cod &
                                   "&c_c=" & objp.Cul_Cod &
                                   "&d_i=" & objp.Data_Inizio &
                                   "&d_f=" & objp.Data_Fine

                ''questo sarà da aggiungere (anche in if che contiene l'istruzione quando lo si rilascia 
                ''come comportamento per il pulsante  "statistiche piano colturale catasto"
                'If Report = enum_CodificaStampe.PianoColturaleCatasto Then
                '    RedirectURL &= "&PCC=1"
                'End If

                If Report = enum_CodificaStampe.ReportRisultatoFiltroneG2G Then
                    RedirectURL &= "&g2g=1"
                End If

                If Report = enum_CodificaStampe.ReportRisultatoFiltroneIncludiVisita Then
                    RedirectURL &= "&IncludiVisite=true"
                End If

            Case enum_CodificaStampe.Conf_FiltroStampe,
                 enum_CodificaStampe.Filtro_StampeBiologico,
                 enum_CodificaStampe.Lista_InsolutiClienti,
                 enum_CodificaStampe.Lista_InsolutiFornitori,
                 enum_CodificaStampe.EstrazioneCatastoAffitti,
                 enum_CodificaStampe.Statistometro



                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
                Dim StrNodiVariabili As String = StrNodo

                Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                objAgronicaStampe.report = Report
                objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                objAgronicaStampe.Xml_Generico.Length = 0
                objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case Else

                Dim risp = MenuBS_Agenda_Nuovo.gestisciStampa(Report, -1, "", New List(Of Reg_Impianti)())

                If risp.RispostaOK Then
                    If risp.Tipo = "1" Then
                        RedirectURL = risp.ParametroDue_stringa
                    Else
                        RedirectURL = risp.RispostaStringa
                    End If
                End If

        End Select

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetFiltroAziende(ByVal IDSezione As Integer) As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        HttpContext.Current.Session("VariabiliFiltro") = Nothing
        Lingua.Gias_InizializzaCultura_DaSession()

        ' imposto titolo pagina x filtro aziende
        'If IDSezione = 0 Then
        Dim sezione As New JObject()
        sezione.Add("testo", AgronicaAgenda_2010.FiltroDiRicercaImprese)
        sezione.Add("colore", "#002F5F")
        sezione.Add("classeCSS", "")
        sezione.Add("IDSezione", "")
        sezione.Add("IDSezionePadre", "")
        sezione.Add("aziendaRichiesta", "0")
        sezione.Add("configurazione", "")
        Dim oggetto As String = JsonConvert.SerializeObject(sezione)
        HttpContext.Current.Session("ASG_MenuBS_2017") = oggetto
        'End If

        'Costruisco il link
        Dim Origine As String = "../Menu/MenuBS_2017.aspx"
        Dim Destinazione As String = IIf(IDSezione <> 0, Origine & "?IDSezione=" & IDSezione, Origine)

        Origine = Stringa_Codifica(Origine, AgroKey_EncoderDecoder)
        Destinazione = Stringa_Codifica(Destinazione, AgroKey_EncoderDecoder)

        Dim linkBuilder As New UriBuilder
        linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
        linkBuilder.Host = HttpContext.Current.Request.Url.Host
        linkBuilder.Port = HttpContext.Current.Request.Url.Port

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim handleImpostazioniSuperuser As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim impostazionePagRicerca = handleImpostazioniSuperuser.ImpostazioneValore1_from_ImpostazioneCod(
            enum_Impostazioni_Utenti.SUPERUSER_Mod_Ricerca_Impresa, objParametri_Utenti, 2)

        Select Case impostazionePagRicerca
            Case "1"
                linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/Filtrino/FiltrinoImprese.aspx")
                linkBuilder.Query = "o=" & Origine & "&d=" & Destinazione

            Case Else

                Dim rowsModalitaFiltroRicerca = handleImpostazioniSuperuser.LeggiImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_Mod_Filtro_Ricerca, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                Dim modalitaFiltroRicerca = rowsModalitaFiltroRicerca.First().Item("Impostazione_Valore_1")

                'Il filtro di ricerca NG è il default (case else)
                Select Case modalitaFiltroRicerca
                    Case "1"
                        Dim sitoOrigineDest As String = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder)
                        Dim tipoOpFiltrone As String = Stringa_Codifica(enum_TipoFiltrone.OperazioniMultiAziendali, AgroKey_EncoderDecoder)
                        Dim codificaStampe As String = Stringa_Codifica(0, AgroKey_EncoderDecoder)
                        Dim categoriaOutput As String = Stringa_Codifica("azienda", AgroKey_EncoderDecoder)
                        Dim tipoFiltrino As String = Stringa_Codifica("1", AgroKey_EncoderDecoder)

                        linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/Filtrone/Filtrone_Nuovo.aspx")
                        linkBuilder.Query = "s_o=" & sitoOrigineDest & "&p_o=" & Origine & "&p_d=" & Destinazione & "&s_d=" & sitoOrigineDest &
                            "&t_f=" & tipoOpFiltrone & "&c_s=" & codificaStampe & "&cat=" & categoriaOutput & "&f=" & tipoFiltrino

                    Case Else
                        HttpContext.Current.Session("ASG_MenuBS_2017") = Nothing
                        Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                            .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {Enum_TipoMostra_FiltroRicerca.Aziende}
                        }

                        If IDSezione = 0 Then
                            parametriFiltroRicercaNG.SitoDestinazioneDopoIlRedirect = CInt(Enum_SiteRedirector.Sito_AgronicaAgenda_2010)
                            parametriFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect = CInt(enum_PagineAgenda_2010.Menu)
                        Else
                            parametriFiltroRicercaNG.IDSezione = IDSezione
                        End If

                        parametriFiltroRicercaNG.CodificaStampe = enum_CodificaStampe.Nessuna
                        parametriFiltroRicercaNG.TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda
                        parametriFiltroRicercaNG.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                        Dim parametriFiltroRicercaNGAsJson = JsonConvert.SerializeObject(parametriFiltroRicercaNG)

                        Dim Parametri_Aggiuntivi As New JObject
                        Parametri_Aggiuntivi.Item("GenericObj_string") = parametriFiltroRicercaNGAsJson

                        Dim objParametriAgenda As New ParametriAgenda
                        Dim url As String = ""

                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                  Enum_SiteRedirector.GiasNG,
                                                                  enum_PagineGiasNG.Pagina_Filtro_Ricerca,
                                                                  url,
                                                                  objParametri_Server,
                                                                  Parametri_Aggiuntivi)

                        Return url

                End Select

        End Select

        Dim TargetUrl = linkBuilder.ToString()

        Return TargetUrl

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetProfitosan() As String

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Return AgronicaCoreGestioneRichieste.profitosan.getLinkSimple(True, HttpContext.Current.Request, New AgronicaCoreGestioneRichieste.AgroWebConfig, objParametri_Server, objParametri_Utenti)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetMenuPrecedente() As String

        Dim objParametriAgenda As New ParametriAgenda
        Dim ParametriGiasOnline As New ParametriGiasOnline

        ParametriGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Menu_principale
        ParametriGiasOnline.Piva = objParametriAgenda.Piva

        Return AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriGiasOnline)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiWorkflow(ByVal WWorkflow_Cod As Integer, ByVal Servizio_Cod As Integer) As RispostaStandard


        Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim objParametriAgenda As New ParametriAgenda
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim lettura As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
            Dim dt As DataTable = lettura.LeggiListaPerImpresa(objParametriAgenda.Piva, WWorkflow_Cod, Servizio_Cod, "", "", objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

    Public Shared Function LeggiConfigMenu(ByRef objParametri_Server As AgronicaCoreParametri) As JObject
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017_Config", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            Return JsonConvert.DeserializeObject(DTConfigSiti.Rows(0)("valore").ToString)
        End If
        Return Nothing
    End Function

    Public Shared Function GetConfigMenu(ByRef Config As JObject, ByVal Chiave As String, ByVal DefVal As Object) As Object
        If Not IsNothing(Config) Then
            If Not IsNothing(Config.Property(Chiave)) Then
                Return Config.GetValue(Chiave)
            End If
        End If
        Return DefVal
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DSSDifesa_Autorizzato = False
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            DSSDifesa_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Analisi_Modelli_Previsionali,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)

        End If
        HttpContext.Current.Session.Remove("ckViewModal")
        'Forzo versione vecchia, tanto questa pagina verrà sostituita da dashboard Angular
        Master.Master_versione = "Agronica"
    End Sub

End Class