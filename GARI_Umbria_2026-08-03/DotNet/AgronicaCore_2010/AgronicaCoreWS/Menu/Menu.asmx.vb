Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Menu
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Menu
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Menu
    Inherits System.Web.Services.WebService


#Region "Ultime Aziende Selezionate, Scrittura e Lettura"

    'prende una piva e la inserisce nel db utenti, tabella: Utenti_Navigazione_Aziende
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function aggiornaAttivitaNavigazioneAziende(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of attivitaNavigazioneAziende_in)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Dim NUM_DI_OPERAZIONE_DA_TENERE As Integer
        Try

            'Se NUM_DI_OPERAZIONE_DA_TENERE e' nel db, altrimenti(primo utilizzo) lo inserisco nel db, valore default in CostantiPersonalizzate 
            Dim impostazioniUtentiRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtUtenteImpostazione = impostazioniUtentiRead.Leggi(enum_Impostazioni_Utenti.UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)


            If Not IsNothing(dtUtenteImpostazione) AndAlso dtUtenteImpostazione.Rows.Count > 0 Then
                Dim temp = CStr(dtUtenteImpostazione.Rows(0).Item("Impostazione_Valore_1"))
                NUM_DI_OPERAZIONE_DA_TENERE = CInt(CStr(dtUtenteImpostazione.Rows(0).Item("Impostazione_Valore_1")))
            Else
                Dim impostazioniUtentiWrite As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
                impostazioniUtentiWrite.Scrivi(enum_Impostazioni_Utenti.UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE,
                                            DEFAULT_UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE,
                                            "",
                                            "",
                                            "",
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            objParametri_Utenti)
                NUM_DI_OPERAZIONE_DA_TENERE = DEFAULT_UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE
            End If



            Dim piva As String = objParametri.InData.piva
            Dim scritturaDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazione_DB_W


            scritturaDB.inserisciNavigazioneAziendeUtente(piva, objParametri_Server, objParametri_Utenti, NUM_DI_OPERAZIONE_DA_TENERE)

            r.RispostaOK = True
            r.RispostaStringa = "Attivita navigazione azienda inserita"
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ultimeAziendeSelezionate(ByVal InData As Object) As rispostaStandard(Of List(Of Impresa))
        Dim r As New rispostaStandard(Of List(Of Impresa))

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)


        Try
            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
            Dim dt As DataTable = lettureDB.LeggiUltimeAziendeSelezionate(objParametri_Server, objParametri_Utenti)
            Dim dati = dt.ToExpandoObject

            Dim rispostaDati As New List(Of Impresa)


            rispostaDati = dati.Select(Function(azienda) New Impresa With {.ragioneSociale = azienda.Item("rag_soc"), .partitaIva = azienda.Item("piva"), .CUAA = azienda.Item("CodiceCUAA")}).ToList()


            r.RispostaOK = True
            r.RispostaStringa = rispostaDati
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ultimeAziendeSelezionate2(ByVal InData As Object) As String 'rispostaStandard(Of List(Of Impresa))
        Return "test ultimeAziendeSelezionate2"
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function aggiornaNumeroDiOperazioniAzienda(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Try
            Dim numeroDiUltimeAziendeSelezionateDaVisulizzare = objParametri.InData.numeroDiUltimeAziendeSelezionate

            letturaPreferiti.Cancella(enum_Impostazioni_Utenti.UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE,
                                                       "",
                                                       objParametri_Utenti)

            letturaPreferiti.Scrivi(enum_Impostazioni_Utenti.UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE,
                                                       numeroDiUltimeAziendeSelezionateDaVisulizzare,
                                                       "",
                                                       "",
                                                       "",
                                                       AGRODATAINIZIO,
                                                       AGRODATAFINE,
                                                       objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = "OK"
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

#End Region

#Region "Preferiti"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function aggiornaPreferiti(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of preferiti_in)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

        Try
            Dim preferiti As List(Of Integer) = objParametri.InData.preferiti
            Dim stringaPreferiti = String.Join("|", preferiti)

            letturaPreferiti.Cancella(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                       "",
                                                       objParametri_Utenti)

            letturaPreferiti.Scrivi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017,
                                                       stringaPreferiti,
                                                       "",
                                                       "",
                                                       "",
                                                       AGRODATAINIZIO,
                                                       AGRODATAFINE,
                                                       objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = "OK"
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
#End Region

#Region "alberoMenu"


    Private Function CreaNodoAlberoDatoExpando(element As IDictionary(Of String, Object)) As LinkMenu
        Dim testoFiglio As String = element.Item("Testo").ToString
        Dim idSezioneFiglio As Integer = element.Item("IDSezione")
        Dim richiedeAziendaSelezionataFiglio As Integer = element.Item("RichiedeAziendaSelezionata")
        Dim paginaRichiestaFiglio As Integer = element.Item("PaginaRichiesta")
        Dim Enum_SiteRedirectorFiglio As Integer = element.Item("Enum_SiteRedirector")
        Dim redirectUrlFiglio As String = element.Item("RedirectURL").ToString
        Dim coloreFiglio As String = element.Item("Colore").ToString
        Dim ColoreAlternativoFiglio As String = element.Item("ColoreAlternativo").ToString
        If String.IsNullOrEmpty(ColoreAlternativoFiglio) Then
            ColoreAlternativoFiglio = coloreFiglio
        End If

        Dim idHtmlFiglio As String = element.Item("id_html").ToString
        Dim classeCssIcona As String = element.Item("ClasseCssIcona").ToString

        Dim menuFiglio As New LinkMenu With {
                                    .testo = testoFiglio,
                                    .idSezione = idSezioneFiglio,
                                    .richiedeAziendaSelezionata = richiedeAziendaSelezionataFiglio,
                                    .paginaRichiesta = paginaRichiestaFiglio,
                                    .sitoRichiesto = Enum_SiteRedirectorFiglio,
                                    .redirectUrl = redirectUrlFiglio,
                                    .colore = coloreFiglio,
                                    .coloreAlternativo = ColoreAlternativoFiglio,
                                    .idHtml = idHtmlFiglio,
                                    .classeCssIcona = classeCssIcona
        }

        Return menuFiglio
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAlberoMenu(ByVal InData As Object) As rispostaStandard(Of AlberoMenu)
        Dim r As New rispostaStandard(Of AlberoMenu)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
            Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti


            Dim IDTipoPadri = 1
            Dim IDTipoFigli = 2

            Dim ConfigMenu = LeggiConfigMenu(objParametri_Server)
            Dim NascondiMenu = GetConfigMenu(ConfigMenu, "nascondiMenu", True)

            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R

            Dim albero As New AlberoMenu

            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read


            Dim dtPreferiti = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim menuPreferiti As New List(Of String)
            If Not IsNothing(dtPreferiti) AndAlso (dtPreferiti.Rows.Count > 0) Then
                Dim temp = CStr(dtPreferiti.Rows(0).Item("Impostazione_Valore_1"))
                menuPreferiti = temp.Split("|").ToList()
            End If

            'Il preset iniziale sono le aziende preferiti del superUser
            Dim dtPresetIniziale = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim menuPresetIniziale As New List(Of String)
            If Not IsNothing(dtPresetIniziale) AndAlso (dtPresetIniziale.Rows.Count > 0) Then
                Dim temp = CStr(dtPresetIniziale.Rows(0).Item("Impostazione_Valore_1"))
                menuPresetIniziale = temp.Split("|").ToList()
            End If

            Dim objDefault As New Utenti_Impostazioni_Read
            Dim dtImpostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SuperUser_Gestione_PDC_Analisi, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim fixPDCAnalisi As Boolean = dtImpostazioni.Rows.Count > 0 AndAlso dtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = "1"


            Dim dtTutto = lettureDB.LeggiPulsantiPadriFigli(NascondiMenu, objParametri_Server, objParametri_Utenti)
            Dim eTutto = dtTutto.ToExpandoObject



            'eTutto.Where(Function(el) el.Item("IDTipoSezione") = IDTipoPadri).Select(Of LinkMenu)(Function(padre) CreaNodoAlberoDatoExpando(padre)) _
            '    .Select(Of LinkMenu)(Function(padre)
            '                             eTutto.Where(Function(x) x.Item("IDTipoSezione") = IDTipoFigli AndAlso x.Item("IDSezionePadre") = padre.idSezione).Select(Of LinkMenu)(
            '                                          Function(figlio)
            '                                              Return CreaNodoAlberoDatoExpando(figlio)
            '                                          End Function).ToList().ForEach(Function(el) padre.Figli.Append(el))
            '                             Return padre
            '                         End Function)
            eTutto.Where(Function(el) el.Item("IDTipoSezione") = IDTipoPadri).ToList().ForEach(
                Sub(padre)
                    Dim testoMacrocategoria As String = padre.Item("Testo").ToString
                    Dim idSezioneMacrocategoria As Integer = padre.Item("IDSezione")
                    Dim richiedeAziendaSelezionata As Integer = padre.Item("RichiedeAziendaSelezionata")
                    Dim paginaRichiesta As Integer = padre.Item("PaginaRichiesta")
                    Dim Enum_SiteRedirector As Integer = padre.Item("Enum_SiteRedirector")
                    Dim redirectUrl As String = padre.Item("RedirectURL").ToString
                    Dim colore As String = padre.Item("Colore").ToString
                    Dim ColoreAlternativo As String = padre.Item("ColoreAlternativo").ToString
                    If String.IsNullOrEmpty(ColoreAlternativo) Then
                        ColoreAlternativo = colore
                    End If
                    Dim idHtml As String = padre.Item("id_html").ToString
                    Dim classeCssIcona As String = padre.Item("ClasseCssIcona").ToString


                    Dim menuPadre As New LinkMenu With {
                    .testo = testoMacrocategoria,
                    .idSezione = idSezioneMacrocategoria,
                    .richiedeAziendaSelezionata = richiedeAziendaSelezionata,
                    .paginaRichiesta = paginaRichiesta,
                    .sitoRichiesto = Enum_SiteRedirector,
                    .redirectUrl = redirectUrl,
                    .colore = colore,
                    .coloreAlternativo = ColoreAlternativo,
                    .idHtml = idHtml,
                    .classeCssIcona = classeCssIcona
                    }

                    eTutto.Where(Function(x) x.Item("IDTipoSezione") = IDTipoFigli AndAlso x.Item("IDSezionePadre") = idSezioneMacrocategoria).ToList().ForEach(
                    Sub(figlio)
                        Dim testoFiglio As String = figlio.Item("Testo").ToString
                        Dim idSezioneFiglio As Integer = figlio.Item("IDSezione")
                        Dim richiedeAziendaSelezionataFiglio As Integer = figlio.Item("RichiedeAziendaSelezionata")
                        Dim paginaRichiestaFiglio As Integer = figlio.Item("PaginaRichiesta")
                        Dim Enum_SiteRedirectorFiglio As Integer = figlio.Item("Enum_SiteRedirector")
                        Dim redirectUrlFiglio As String = figlio.Item("RedirectURL").ToString
                        Dim coloreFiglio As String = figlio.Item("Colore").ToString
                        Dim ColoreAlternativoFiglio As String = figlio.Item("ColoreAlternativo").ToString
                        Dim enum_TipoAperturaPagina As String = figlio.Item("Enum_TipoAperturaPagina").ToString
                        If String.IsNullOrEmpty(enum_TipoAperturaPagina) Then
                            enum_TipoAperturaPagina = 0
                        End If
                        If String.IsNullOrEmpty(ColoreAlternativoFiglio) Then
                            ColoreAlternativoFiglio = coloreFiglio
                        End If

                        Dim idHtmlFiglio As String = figlio.Item("id_html").ToString
                        Dim classeCssIconaFiglio As String = classeCssIcona

                        Dim presetIniziale As Boolean = menuPresetIniziale.Contains(idSezioneFiglio)
                        Dim preferito As Boolean = menuPreferiti.Contains(idSezioneFiglio)

                        ' fix per saltare alla pagina gestione analisi nei PDC se attiva la nuova gestione
                        If fixPDCAnalisi AndAlso idSezioneFiglio = enum_Sezioni_MenuBS_2017.Inserimento_Analisi_Laboratorio Then
                            Enum_SiteRedirectorFiglio = TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                            paginaRichiestaFiglio = enum_PaginePianiCampionamento.GestioneAnalisi
                        ElseIf fixPDCAnalisi AndAlso idSezioneFiglio = enum_Sezioni_MenuBS_2017.PianiCampionamentiAnalisi Then
                            enum_TipoAperturaPagina = 0
                        End If

                        Dim menuFiglio As New LinkMenu With {
                                    .testo = testoFiglio,
                                    .idSezione = idSezioneFiglio,
                                    .richiedeAziendaSelezionata = richiedeAziendaSelezionataFiglio,
                                    .paginaRichiesta = paginaRichiestaFiglio,
                                    .sitoRichiesto = Enum_SiteRedirectorFiglio,
                                    .redirectUrl = redirectUrlFiglio,
                                    .colore = coloreFiglio,
                                    .coloreAlternativo = ColoreAlternativoFiglio,
                                    .idHtml = idHtmlFiglio,
                                    .classeCssIcona = classeCssIconaFiglio,
                                    .presetIniziale = presetIniziale,
                                    .preferito = preferito,
                                    .enum_TipoAperturaPagina = enum_TipoAperturaPagina
                        }

                        menuPadre.Figli.Add(menuFiglio)
                        If preferito Then
                            Dim padreTrovato = albero.MenusPreferiti.Find(Function(x) x.idSezione = menuPadre.idSezione)
                            If padreTrovato IsNot Nothing Then
                                padreTrovato.Figli.Add(menuFiglio)
                            Else
                                Dim menuPadrePreferiti = DeepClone(Of LinkMenu)(menuPadre)
                                menuPadrePreferiti.Figli = New List(Of LinkMenu)
                                menuPadrePreferiti.Figli.Add(menuFiglio)
                                albero.MenusPreferiti.Add(menuPadrePreferiti)

                            End If
                        End If
                    End Sub)
                    albero.Menus.Add(menuPadre)
                End Sub)

            r.RispostaStringa = albero
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAlberoMenu_APP(ByVal InData As Object) As rispostaStandard(Of AlberoMenu)
        Dim r As New rispostaStandard(Of AlberoMenu)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
            Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

            Dim ConfigMenu = LeggiConfigMenu(objParametri_Server)
            Dim NascondiMenu = GetConfigMenu(ConfigMenu, "nascondiMenu", True)

            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R

            Dim albero As New AlberoMenu

            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read


            Dim dtPreferiti = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim menuPreferiti As New List(Of String)
            If Not IsNothing(dtPreferiti) AndAlso dtPreferiti.Rows.Count > 0 Then
                Dim temp = CStr(dtPreferiti.Rows(0).Item("Impostazione_Valore_1"))
                menuPreferiti = temp.Split("|").ToList()
            End If

            'Il preset iniziale sono le aziende preferiti del superUser
            Dim dtPresetIniziale = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim menuPresetIniziale As New List(Of String)
            If Not IsNothing(dtPresetIniziale) AndAlso dtPresetIniziale.Rows.Count > 0 Then
                Dim temp = CStr(dtPresetIniziale.Rows(0).Item("Impostazione_Valore_1"))
                menuPresetIniziale = temp.Split("|").ToList()
            End If

            Dim objDefault As New Utenti_Impostazioni_Read
            Dim dtImpostazioni = objDefault.Leggi(enum_Impostazioni_Utenti.SuperUser_Gestione_PDC_Analisi, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim fixPDCAnalisi As Boolean = dtImpostazioni.Rows.Count > 0 AndAlso dtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = "1"


            Dim dtTutto = lettureDB.LeggiPulsantiChecklist(NascondiMenu,
                                                            objParametri_Server,
                                                            objParametri_Utenti)
            Dim eTutto = dtTutto.ToExpandoObject




            eTutto.Where(Function(el) el.Item("isChecklist") = 1).ToList().ForEach(
                Sub(checklist)
                    Dim testoFiglio As String = checklist.Item("Testo").ToString
                    Dim idSezioneFiglio As Integer = checklist.Item("IDSezione")
                    Dim richiedeAziendaSelezionataFiglio As Integer = checklist.Item("RichiedeAziendaSelezionata")
                    Dim paginaRichiestaFiglio As Integer = checklist.Item("PaginaRichiesta")
                    Dim Enum_SiteRedirectorFiglio As Integer = checklist.Item("Enum_SiteRedirector")
                    Dim redirectUrlFiglio As String = checklist.Item("RedirectURL").ToString
                    Dim coloreFiglio As String = checklist.Item("Colore").ToString
                    Dim ColoreAlternativoFiglio As String = checklist.Item("ColoreAlternativo").ToString
                    Dim enum_TipoAperturaPagina As String = checklist.Item("Enum_TipoAperturaPagina").ToString
                    If String.IsNullOrEmpty(enum_TipoAperturaPagina) Then
                        enum_TipoAperturaPagina = 0
                    End If
                    If String.IsNullOrEmpty(ColoreAlternativoFiglio) Then
                        ColoreAlternativoFiglio = coloreFiglio
                    End If

                    Dim idHtmlFiglio As String = checklist.Item("id_html").ToString
                    Dim classeCssIconaFiglio As String = ""

                    Dim presetIniziale As Boolean = menuPresetIniziale.Contains(idSezioneFiglio)
                    Dim preferito As Boolean = menuPreferiti.Contains(idSezioneFiglio)

                    ' fix per saltare alla pagina gestione analisi nei PDC se attiva la nuova gestione
                    If fixPDCAnalisi AndAlso idSezioneFiglio = enum_Sezioni_MenuBS_2017.Inserimento_Analisi_Laboratorio Then
                        Enum_SiteRedirectorFiglio = Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                        paginaRichiestaFiglio = enum_PaginePianiCampionamento.GestioneAnalisi
                    ElseIf fixPDCAnalisi AndAlso idSezioneFiglio = enum_Sezioni_MenuBS_2017.PianiCampionamentiAnalisi Then
                        enum_TipoAperturaPagina = 0
                    End If

                    Dim menuFiglio As New LinkMenu With {
                                .testo = testoFiglio,
                                .idSezione = idSezioneFiglio,
                                .richiedeAziendaSelezionata = richiedeAziendaSelezionataFiglio,
                                .paginaRichiesta = paginaRichiestaFiglio,
                                .sitoRichiesto = Enum_SiteRedirectorFiglio,
                                .redirectUrl = redirectUrlFiglio,
                                .colore = coloreFiglio,
                                .coloreAlternativo = ColoreAlternativoFiglio,
                                .idHtml = idHtmlFiglio,
                                .classeCssIcona = classeCssIconaFiglio,
                                .presetIniziale = presetIniziale,
                                .preferito = preferito,
                                .enum_TipoAperturaPagina = enum_TipoAperturaPagina
                    }
                    albero.Menus.Add(menuFiglio)
                End Sub)

            r.RispostaStringa = albero
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Function LeggiConfigMenu(ByRef objParametri_Server As AgronicaCoreParametri) As JObject
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017_Config", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            Return JsonConvert.DeserializeObject(DTConfigSiti.Rows(0)("valore").ToString)
        End If
        Return Nothing
    End Function

    Private Function GetConfigMenu(ByRef Config As JObject, ByVal Chiave As String, ByVal DefVal As Object) As Object
        If Not IsNothing(Config) Then
            If Not IsNothing(Config.Property(Chiave)) Then
                Return Config.GetValue(Chiave)
            End If
        End If
        Return DefVal
    End Function
#End Region

#Region "ottieni Informazioni Assistenza/Utente"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ottieniInformazioniAssistenza(ByVal InData As Object) As rispostaStandard(Of InformazioniAssistenza)
        Dim r As New rispostaStandard(Of InformazioniAssistenza)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)


        Try

            Dim xAssistenza As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim sAssistenza As String = xAssistenza.Assistenza(objParametri_SuperServer, objParametri_Server)

            Dim email = ""
            Dim tel = ""
            Dim whatsapp = ""

            Dim emailPattern As Regex = New Regex("(?:href=""mailto:)(.*)(?:"">)", RegexOptions.None, TimeSpan.FromSeconds(3))
            Dim telPattern As Regex = New Regex("/href=""callto:(.*)"">/", RegexOptions.None, TimeSpan.FromSeconds(3))
            Dim whatsappPattern As Regex = New Regex("/href=""https://wa.me/+39:(.*)"">/", RegexOptions.None, TimeSpan.FromSeconds(3))

            Dim emailMatches = emailPattern.Match(sAssistenza, RegexOptions.IgnoreCase)
            If emailMatches.Success AndAlso emailMatches.Groups.Count >= 2 Then
                email = emailMatches.Groups(1).Value
            End If

            Dim telMatches = telPattern.Match(sAssistenza, RegexOptions.IgnoreCase)
            If telMatches.Success AndAlso telMatches.Groups.Count >= 2 Then
                tel = telMatches.Groups(1).Value
            End If
            Dim whatsappMatches = whatsappPattern.Match(sAssistenza, RegexOptions.IgnoreCase)
            If whatsappMatches.Success AndAlso whatsappMatches.Groups.Count >= 2 Then
                whatsapp = whatsappMatches.Groups(1).Value
            End If

            Dim informazioniAssistenza As New InformazioniAssistenza With {
                .email = email,
                .tel = tel,
                .whatsapp = whatsapp
                }


            r.RispostaOK = True
            r.RispostaStringa = informazioniAssistenza
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ottieniInformazioneUtente(ByVal InData As Object) As rispostaStandard(Of Utente)
        Dim r As New rispostaStandard(Of Utente)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
        Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)


        Try
            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim DTUtente As DataTable = objUtente.Leggi(objParametri_Utenti.UtenteUsername, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim utenteRow = DTUtente.ToExpandoObject.FirstOrDefault()

            Dim xLeggiUltimoAccesso As New AgronicaCoreUtentiBIZ.AWS_Log_R
            Dim ultimoAccesso = xLeggiUltimoAccesso.UltimoAccesso(objParametri_Utenti)

            Dim visibilita As String
            If objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO AndAlso objParametri_Server.FinestraTemporaleFine = AGRODATAFINE Then
                'visibilita = My.Resources.Menu_asmx.VisualizzazioneIllimitata
                visibilita = "Visualizzazione Illimitata"
            Else
                Dim stb As New StringBuilder("Visualizzazione Limitata Tra Il")
                'Dim stb As New StringBuilder(My.Resources.Menu_asmx.VisualizzazioneLimitataTraIl)
                If (objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO) Then
                    stb.Append("...")
                Else
                    stb.Append(objParametri_Server.FinestraTemporaleInizio)
                End If

                'stb.Append(My.Resources.Menu_asmx.EIl)
                stb.Append("e Il")

                If (objParametri_Server.FinestraTemporaleFine = AGRODATAFINE) Then
                    stb.Append("...")
                Else
                    stb.Append(objParametri_Server.FinestraTemporaleFine)
                End If

                visibilita = stb.ToString()
            End If

            Dim rispUtente As New Utente With {
                .Nome = utenteRow.Item("Nome"),
                .Cognome = utenteRow.Item("Cognome"),
                .Rag_Soc = utenteRow.Item("Rag_Soc"),
                .UserNameCommerciale = utenteRow.Item("UserNameCommerciale"),
                .Email = utenteRow.Item("Email"),
                .Tel = utenteRow.Item("Tel"),
                .UltimoAccesso = ultimoAccesso.ToString(),
                .Visibilita = visibilita,
                .UserName = utenteRow.Item("UserName")
            }

            r.RispostaOK = True
            r.RispostaStringa = rispUtente
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

#End Region

#Region "breadcrum"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ottieniIconaDaTesto(ByVal InData As Object) As rispostaStandard(Of BreadcrumbsInfo)
        Dim r As New rispostaStandard(Of BreadcrumbsInfo)
        Dim breadcrumbs As New BreadcrumbsInfo
        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
            Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
            Dim dtTutto = lettureDB.LeggiIconaPerBreadCrum(objParametri.InData, objParametri_Server, objParametri_Utenti)

            If dtTutto.Rows.Count > 0 Then
                breadcrumbs.ClasseCssIcona = dtTutto.Rows(0).Item("ClasseCssIcona").ToString
                breadcrumbs.Colore = dtTutto.Rows(0).Item("ColoreAlternativo").ToString
                breadcrumbs.IDSezionePadre = dtTutto.Rows(0).Item("IDSezione").ToString
            End If
            r.RispostaOK = True
            r.RispostaStringa = breadcrumbs
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OttieniBreadcrumbs(ByVal InData As Object) As rispostaStandard(Of BreadcrumbsInfo)

        Dim r As New rispostaStandard(Of BreadcrumbsInfo)
        Dim breadcrumbs As New BreadcrumbsInfo

        Dim objParametri = DeserializzaInData(Of Integer)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
            Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti

            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
            Dim dtTutto = lettureDB.LeggiTestoDatoIDSezione(objParametri.InData, objParametri_Server, objParametri_Utenti)

            If dtTutto.Rows.Count >= 2 Then
                breadcrumbs.TestoPadre = dtTutto.Rows(0).Item("Testo").ToString
                breadcrumbs.TestoFiglio = dtTutto.Rows(1).Item("Testo").ToString
                breadcrumbs.idSezionePadre = dtTutto.Rows(1).Item("IDSezionePadre").ToString
            End If

            If Not String.IsNullOrEmpty(breadcrumbs.TestoPadre) Then
                Dim dtIcona = lettureDB.LeggiIconaPerBreadCrum(breadcrumbs.TestoPadre, objParametri_Server, objParametri_Utenti)
                If Not IsNothing(dtIcona) AndAlso dtIcona.Rows.Count > 0 Then
                    breadcrumbs.ClasseCssIcona = If(IsDBNull(dtIcona.Rows(0).Item("ClasseCssIcona")), "", dtIcona.Rows(0).Item("ClasseCssIcona").ToString)
                    breadcrumbs.Colore = If(IsDBNull(dtIcona.Rows(0).Item("ColoreAlternativo")), "", dtIcona.Rows(0).Item("ColoreAlternativo").ToString)
                End If
            End If

            r.RispostaOK = True
            r.RispostaStringa = breadcrumbs

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ottieniTestoDaIDSezione(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of Object)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server
            Dim objParametri_Utenti As AgronicaCoreParametri = objParametri.Utenti



            Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R




            Dim dtTutto = lettureDB.LeggiTestoDatoIDSezione(objParametri.InData, objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = ""
            If dtTutto.Rows.Count >= 2 Then
                r.RispostaStringa = dtTutto.Rows(0).Item("Testo").ToString & "_" & dtTutto.Rows(1).Item("Testo").ToString
            End If
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Versione Header"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiVersioneHeader(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of Object)(InData)
        Dim versioneHeader As String = ""


        Try
            Dim objParametri_SuperServer As AgronicaCoreParametri = objParametri.Super_Server
            Dim objParametri_Server As AgronicaCoreParametri = objParametri.Server

            Dim objConfSiti As New Configurazione_Siti_R

            'cerco versione personalizzata su db Server
            versioneHeader = objConfSiti.Leggi_Valore(0, "VersioneHeader", "", "", objParametri_Server)

            If String.IsNullOrEmpty(versioneHeader) Then
                versioneHeader = objConfSiti.Leggi_Valore(0, "VersioneHeader", "", "", objParametri_SuperServer)
            End If

            If String.IsNullOrEmpty(versioneHeader) OrElse versioneHeader.ToLower = "ultimaversione" Then
                versioneHeader = VERSIONE_HEADER_DEFAULT
            End If

            r.RispostaOK = True
            r.RispostaStringa = versioneHeader

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Utility"
    Function DeepClone(Of T)(ByVal objectToClone As T) As T
        If (objectToClone Is Nothing) Then Return Nothing

        Return JsonConvert.DeserializeObject(Of T)(JsonConvert.SerializeObject(objectToClone))
    End Function

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    Private Shared Sub Gias_InizializzaCultura_DaParams(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read

        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Utenti)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")

        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    End Sub
#End Region

End Class