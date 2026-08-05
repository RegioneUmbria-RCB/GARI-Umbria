
Imports AgronicaCoreDataProvider
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class DSS_Gauges
    Inherits System.Web.UI.Page

    Private Sub DSS_Gauges_Init(sender As Object, e As EventArgs) Handles Me.Init
        'AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function PreparaParametriIndicatore(params As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim parametriEncoded = Stringa_Codifica(params, AgroKey_EncoderDecoder, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = parametriEncoded

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

End Class

