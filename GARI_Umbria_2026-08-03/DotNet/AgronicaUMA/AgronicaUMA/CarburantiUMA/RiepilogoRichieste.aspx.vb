
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaUMA.Resources
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreXML.XML_Stampe
Imports System.Web

Public Class RiepilogoRichieste
    Inherits System.Web.UI.Page

    Public QS_Piva As String = ""
    Public QS_PagArrivo As String = ""

#Region "Riepilogo"

    ''' <summary>
    ''' Funzione dedicata a restituire la sintesi UMA divisa per Azienda e Tipo Azienda (Privata, Terzista, Cooperativa)
    ''' </summary>
    ''' <param name="piva"> se diverso da "" filtra solo su questa piva</param>
    ''' <param name="statoCod"> se diverso da 0 filtra solo sulle richieste/rendicontazioni che sono nello stato specificato</param>
    ''' <param name="citta"> se diverso da "-1", filtra per il codice della citta selezionata </param>
    ''' <param name="prov"> se diverso da "-1" filtra per il codice della provincia selezionata </param>
    ''' <param name="anno"> L'anno di riferimento per la ricerca</param>
    ''' <param name="filtroConto"> Se minore di 1, filtra per il conto selezionato (0 conto proprio, -1 conto terzi)</param>
    ''' <param name="nuovaVisibilita"> Se True, usa le impostazioni della nuova visibilità utente per mostrare i risultati (un utente potrebbe non essere in grado di vedere tutti i dati)</param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Riepiloghi(ByVal piva As String,
                                           ByVal statoCod As Integer,
                                           ByVal citta As String,
                                           ByVal prov As String,
                                           ByVal anno As Integer,
                                           ByVal filtroConto As Integer,
                                           ByVal nuovaVisibilita As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim piveVisibili As String = ""
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

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

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = Not nuovaVisibilita AndAlso Not objProfilo.HasFullVisibility(
            HttpContext.Current.Session("ASG_Utente_Username"), objParametri_Utenti)

        If (piva = "-1") Then
            piva = ""
        End If

        Dim FiltroUtente As Boolean = False
        Dim FiltroGruppo As Boolean = False
        Dim Gruppo As Integer = 0
        Dim VisibilitaTotale As Boolean = False

        If (nuovaVisibilita) Then
            'Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            'Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            'Dim userName As String = objParametri_Server.UtenteUsername

            'Dim dtt As DataTable = objUtenti_Visibilita.OttieniPIVEVisibilita(userName, objParametri_Server, objParametri_Utenti)

            'If (dtt.Rows.Count > 0) Then
            '    piveVisibili = dtt.AsEnumerable().
            '    [Select](Function(x) x("Piva_Azienda").ToString()).Aggregate(Function(a, b) String.Concat(a & "'" & "," & "'" & b))
            'Else
            '    piveVisibili = "-1"
            'End If

            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objParametri_Utenti, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

        End If

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dt = objTestate.LeggiRiepilogo(piva,
                                       anno,
                                       Filtro_Visibilita_Utente,
                                       piveVisibili, "", "",
                                       objParametri_Server, objParametri_Utenti,
                                       rendicontazioni:=False,
                                       statoCod:=statoCod,
                                       citta:=citta,
                                       prov:=prov,
                                       conto:=filtroConto,
                                       FiltroNuovaVisibilita:=nuovaVisibilita,
                                       FiltroUtente:=FiltroUtente,
                                       FiltroGruppoUtente:=FiltroGruppo,
                                       GruppoUtente:=Gruppo,
                                       VisibilitaTotale:=VisibilitaTotale)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPerc(ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R
        Dim Dt As DataTable
        Dim perc As Integer

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = setup.LeggiSetup(anno, objParametri_Server)

            If Dt.Rows.Count > 0 Then
                perc = Dt.Rows.Item(0).Item("Per_Riduzione")
            Else
                perc = 23
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(perc, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaPivaReale(ByVal piva As String) As RispostaStandard


        Dim r As New RispostaStandard
        Dim PartitaIvaReale As String = ""
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                PartitaIvaReale = leggiPivaReale.Leggi_PivaReale(piva, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(PartitaIvaReale, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Master.flag_pag_GestioneCosti = True

        inizializzoObjParametri()
        inizializzoParametriPagina()

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        If Not UtenteAbilitatoLettura Then
            UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Riepilogo_UMA,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If Not UtenteAbilitatoScrittura Then
            UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        If Not IsNothing(Request.QueryString("p")) Then
            QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("pa")) Then
            Dim s_Arrivo = Request.QueryString("pa").ToString
            If (s_Arrivo = "1") Then
                QS_PagArrivo = "1"
                Master.flag_MostraHeader = False
                Master.flag_MostraFooter = False
            Else
                Master.flag_MostraHeader = True
                Master.flag_MostraFooter = True
            End If
        End If

        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_2017.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        hdId_Agenda.Value = 0
        hdId_Mov.Value = 0
        hdId_Mov_Det.Value = 0
        hdId_Agenda_CDG.Value = 0
        hdModalita.Value = Entrata_Diretta
        hdId_CDG.Value = 0
        hdLav_Cod.Value = 0
        hdVeg_Cod.Value = 0
        hdDes_Lib.Value = ""
        hdMov_Desc.Value = ""
        hdSplit.Value = Split
        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))

        'Automatico (Impostazione da QDC)
        hdAutomatico.Value = 1 'Default

        If Not Page.IsPostBack Then


            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()
                hdPiva.Value = objParametriAgenda_2010.Piva
                hdId_Agenda.Value = Val(objParametriAgenda_2010.Id_Agenda)

            End If

            'If Entrata_Diretta = 0 Then

            'Introdotta per non rieseguirlo se appena creata con provenienza da APP
            Dim bombardinoDaEseguire = True

            If Val(hdId_Agenda.Value) = 0 Then

            End If

        End If


    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub


    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

End Class