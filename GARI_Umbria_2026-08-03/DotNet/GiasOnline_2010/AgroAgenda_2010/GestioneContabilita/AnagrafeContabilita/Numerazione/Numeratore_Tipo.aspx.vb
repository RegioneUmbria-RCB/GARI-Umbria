
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework

Public Class Numeratore_Tipo
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Master().Lbl_Titolo.Text = "Assegnazione prefissi e suffissi di numerazione"

        inizializzoObjParametri()
        inizializzoParametriPagina()

        ' TODO Autorizzazioni
        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Numerazione_Documenti,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Numerazione_Documenti,
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

#Region "script services Carica Dati"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaNumeratoriTipo(ByVal piva As String) As RispostaStandard

        Return Numeratore_Tipo_UC.CaricaNumeratoriTipo(piva)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDropDownNumeratoriTipo(ByVal piva As String) As RispostaStandard

        Return Numeratore_Tipo_UC.CaricaDropDownNumeratoriTipo(piva)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaNumeratoriPS(ByVal piva As String) As RispostaStandard

        Return Numeratore_PrefissoSuffisso_UC.CaricaNumeratoriPSB(piva)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDocumentiDefault(ByVal piva As String) As RispostaStandard

        Return Numeratore_Default_UC.CaricaDocumentiDefault(piva)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDropDownNumeratoriPS_Default(ByVal piva As String) As RispostaStandard

        Return Numeratore_Default_UC.CaricaDropDownNumeratoriPS_Default(piva)

    End Function

#End Region

#Region "script services aggiorna dati"


    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaNumeratoriTipo(ByVal paramString As String) As RispostaStandard

        Return Numeratore_Tipo_UC.AggiornaNumeratoriTipo(paramString)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPreSalva_NumeratoriPS(ByVal paramString As String) As RispostaStandard

        Return Numeratore_PrefissoSuffisso_UC.CheckPreSalva_NumeratoriPS(paramString)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaNumeratoriPS(ByVal paramString As String) As RispostaStandard

        Return Numeratore_PrefissoSuffisso_UC.AggiornaNumeratoriPS(paramString)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDocumentiDefault(ByVal paramString As String) As RispostaStandard

        Return Numeratore_Default_UC.AggiornaDocumentiDefault(paramString)

    End Function

#End Region


End Class

