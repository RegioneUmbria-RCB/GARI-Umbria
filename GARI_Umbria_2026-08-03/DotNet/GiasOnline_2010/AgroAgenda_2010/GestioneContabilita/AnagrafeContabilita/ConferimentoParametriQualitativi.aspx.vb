
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json

Public Class ConferimentoParametriQualitativi
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public objparametri_server_string, objparametri_utenti_string As String

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        inizializzoObjParametri()

        ' TODO Autorizzazioni
        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Anagrafiche_Conferimento,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Anagrafiche_Conferimento,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                            AgroKey_EncoderDecoder,
                                            Server)

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable
        DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            PathCoreWS.Value = DTConfigSiti.Rows(0).Item("Valore")
        End If
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

#Region "Metodi Comuni"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiModuloGenerazione(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            DT = leggiAnagrafeLog.LeggiModuliParametriQualitativi(piva, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

#End Region

#Region "Gruppi Referenze"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_GruppiReferenze(ByVal piva As String) As RispostaStandard
        Return GruppiReferenzeUC.Leggi_GruppiReferenze(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaGruppiReferenze(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return GruppiReferenzeUC.AggiornaDaGrigliaGruppiReferenze(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Parametri Qualitativi"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ParametriQualitativi(ByVal piva As String) As RispostaStandard
        Return ParametriQualitativiUC.Leggi_ParametriQualitativi(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaParametriQualitativi(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return ParametriQualitativiUC.AggiornaDaGrigliaParametriQualitativi(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Gruppi Referenze Dettagli"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Elenco_GruppiReferenze(ByVal piva As String) As RispostaStandard
        Return ParametriQualitativiXReferenzaUC.Leggi_Elenco_GruppiReferenze(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Elenco_ParametriQualitativi(ByVal piva As String) As RispostaStandard
        Return ParametriQualitativiXReferenzaUC.Leggi_Elenco_ParametriQualitativi(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ParametriQualitativiXReferenza(ByVal piva As String) As RispostaStandard
        Return ParametriQualitativiXReferenzaUC.Leggi_ParametriQualitativiXReferenza(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaParametriQualitativiXReferenza(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return ParametriQualitativiXReferenzaUC.AggiornaDaGrigliaParametriQualitativiXReferenza(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Elenco Valori Parametri Qualitativi"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiElencoValoriParametriQualitativi(ByVal piva As String) As RispostaStandard
        Return ElencoValoriParametriQualitativiUC.LeggiElencoValoriParametriQualitativi(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ValoriParametriQualitativi(ByVal piva As String) As RispostaStandard
        Return ElencoValoriParametriQualitativiUC.Leggi_ValoriParametriQualitativi(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaElencoValoriParametriQualitativi(ByVal piva As String, ByVal righeInserite As String,
                                                                             ByVal righeModificate As String, ByVal righeCancellate As String,
                                                                             ByVal tutteleRighe As String) As RispostaStandard
        Return ElencoValoriParametriQualitativiUC.AggiornaDaGrigliaElencoValoriParametriQualitativi(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

End Class