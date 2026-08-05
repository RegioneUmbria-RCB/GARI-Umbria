Imports AgronicaCoreContabDAL

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreAnagrafeBIZ

Public Class Lotto_AssegnaxRisumSpeVarQualCert
    Inherits System.Web.UI.Page

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

#Region "script services Carica Dati"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Lotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal tipo_lotto As String) As RispostaStandard
        Return Lotto_AssegnazioneUC.Carica_Lotto_AssegnaxRisumSpeVarQualCert(piva, tipo_lotto)
    End Function

#End Region

#Region "script services aggiorna dati"
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaLotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return Lotto_AssegnazioneUC.AggiornaLotto_AssegnaxRisumSpeVarQualCert(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_pag_CampionamentoConferito = True

        Master().Lbl_Titolo.Text = "Criteri assegnazione lotto"

        inizializzoObjParametri()
        inizializzoParametriPagina()

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


    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property
End Class