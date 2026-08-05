
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreContabDAL

Public Class Conferimento_Config
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

#Region "Common CriteriAggregazione"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_CriteriAggregazione(ByVal piva As String, ByVal tipoLav As Integer, ByVal tipoLavLinea As Integer, ByVal aggregaFornitore As Integer,
                                                     ByVal aggregaSpecie As Integer, ByVal aggregaVarieta As Integer, ByVal aggregaRegolamento As Integer,
                                                     ByVal aggregaLotto As Integer, ByVal aggregaLottoUscita As Integer, ByVal aggregaProdotto As Integer,
                                                     ByVal aggregaProdottoUscita As Integer, ByVal aggregaCella As Integer, ByVal aggregaCellaUscita As Integer,
                                                        ByVal aggregaUnitaMisura As Integer, ByVal paramQual As String, ByVal azione As Integer) As RispostaStandard
        Return criteri_AggregazioneUC.Aggiorna_CriteriAggregazione(piva, tipoLav, tipoLavLinea, aggregaFornitore, aggregaSpecie, aggregaVarieta, aggregaRegolamento,
                                                aggregaLotto, aggregaLottoUscita, aggregaProdotto, aggregaProdottoUscita,
                                                aggregaCella, aggregaCellaUscita, aggregaUnitaMisura, paramQual, azione)
    End Function

#End Region

#Region "AttivazioneModuli"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStatoAttivazioni(ByVal piva As String) As RispostaStandard
        Return AttivazioneModuli.LeggiStatoAttivazioni(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaAttivazioneModuli(ByVal piva As String, trasformazioniVegetali As Boolean, trasformazioniAnimali As Boolean) As RispostaStandard
        Return AttivazioneModuli.SalvaAttivazioneModuli(piva, trasformazioniVegetali, trasformazioniAnimali)
    End Function

#End Region

#Region "Configurazione"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ParamEntrataXSpecieVarieta(ByVal piva As String) As RispostaStandard
        Return ParamEntrataXSpecieVarietaUC.Leggi_ParamEntrataXSpecieVarieta(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaParamEntrataXSpecieVarieta(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return ParamEntrataXSpecieVarietaUC.AggiornaDaGrigliaParamEntrataXSpecieVarieta(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "AssegnaLotti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiConfigurazioneModulo(ByVal piva As String, generazioneModulo As Integer) As RispostaStandard
        Return AssegnazioneLottoConferimentoUC.LeggiConfigurazioneModulo(piva, generazioneModulo)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiTipologieConferimento(ByVal piva As String) As RispostaStandard
        Return AssegnazioneLottoConferimentoUC.LeggiTipologieConferimento(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaConfigurazioneLotto(ByVal piva As String, generazioneModulo As Integer, separatore As String,
                                                    parametriScelti As Integer()) As RispostaStandard
        Return AssegnazioneLottoConferimentoUC.SalvaConfigurazioneLotto(piva, generazioneModulo, separatore, parametriScelti)
    End Function

#End Region

#Region "Conferimento CriteriAggregazione"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Lotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String) As RispostaStandard
        Return Lotto_AssegnaxRisumSpeVarQualCertUC.Carica_Lotto_AssegnaxRisumSpeVarQualCert(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaLotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return Lotto_AssegnaxRisumSpeVarQualCertUC.AggiornaLotto_AssegnaxRisumSpeVarQualCert(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Conferimento Reparti Piani"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiCentriAziendali(ByVal piva As String) As RispostaStandard
        Return GestioneRepartiPianiUC.LeggiCentriAziendali(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_RepartiPiani(ByVal piva As String) As RispostaStandard
        Return GestioneRepartiPianiUC.Leggi_RepartiPiani(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaReparti(ByVal piva As String, ByVal righeInserite As String,
                                                    ByVal righeModificate As String, ByVal righeCancellate As String,
                                                    ByVal tutteleRighe As String) As RispostaStandard
        Return GestioneRepartiPianiUC.AggiornaDaGrigliaReparti(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Conferimento Celle"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiReparti(ByVal piva As String) As RispostaStandard
        Return GestioneCelleUC.LeggiReparti(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Celle(ByVal piva As String) As RispostaStandard
        Return GestioneCelleUC.Leggi_Celle(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaCelle(ByVal piva As String,
                                                    ByVal righeInserite As String,
                                                    ByVal righeModificate As String,
                                                    ByVal righeCancellate As String,
                                                    ByVal tutteleRighe As String) As RispostaStandard
        Return GestioneCelleUC.AggiornaDaGrigliaCelle(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Imprese_Idonee(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard
        Return criteri_AggregazioneUC.Carica_Imprese_Idonee(objP_server, objP_utenti)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_CriteriAggregazione(ByVal piva As String) As RispostaStandard
        Return criteri_AggregazioneUC.Leggi_CriteriAggregazione(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPreparazioniGeneriche(ByVal piva As String) As RispostaStandard
        Return criteri_AggregazioneUC.LeggiPreparazioniGeneriche(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiLineePreparazione(ByVal piva As String, ByVal codGenerazione As Integer) As RispostaStandard
        Return criteri_AggregazioneUC.LeggiLineePreparazione(piva, codGenerazione)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaCriterioAggregazione(ByVal cancellate As String) As RispostaStandard
        Return criteri_AggregazioneUC.CancellaCriterioAggregazione(cancellate)
    End Function



End Class