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

Public Class Contabilita_Config
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
                                            enum_Security_Attivita.ConfigurazioneModalitaPagamento,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.ConfigurazioneModalitaPagamento,
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

#Region "Modalita Pagamento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ModalitaPagamento(ByVal piva As String) As RispostaStandard
        Return ModalitaPagamentoUC.Leggi_ModalitaPagamento(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaCausalePagamento(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return ModalitaPagamentoUC.AggiornaDaGrigliaCausalePagamento(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Causale Trasporto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_CausaleTrasporto(ByVal piva As String) As RispostaStandard
        Return CausaleTrasportoUC.Leggi_CausaleTrasporto(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaCausaleTrasporto(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return CausaleTrasportoUC.AggiornaDaGrigliaCausaleTrasporto(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Istituti Credito"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_IstitutiCredito(ByVal piva As String) As RispostaStandard
        Return IstitutiCreditoUC.Leggi_IstitutiCredito(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaIstitutiCredito(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return IstitutiCreditoUC.AggiornaDaGrigliaIstitutiCredito(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Sezionali"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_RegimiFiscale() As RispostaStandard
        Return SezionaliUC.Leggi_RegimiFiscale()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Sezionali(ByVal piva As String) As RispostaStandard
        Return SezionaliUC.Leggi_Sezionali(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaSezionali(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return SezionaliUC.AggiornaDaGrigliaSezionali(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

#Region "Liquidita"

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoIstitutiCredito(ByVal piva As String) As RispostaStandard
        Return LiquiditaUC.ElencoIstitutiCredito(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Liquidita(ByVal piva As String) As RispostaStandard
        Return LiquiditaUC.Leggi_Liquidita(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDaGrigliaLiquidita(ByVal piva As String, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, ByVal tutteleRighe As String) As RispostaStandard
        Return LiquiditaUC.AggiornaDaGrigliaLiquidita(piva, righeInserite, righeModificate, righeCancellate, tutteleRighe)
    End Function

#End Region

End Class