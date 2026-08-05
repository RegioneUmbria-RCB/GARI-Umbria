Imports Newtonsoft.Json

Public Class ReimpostazionePasswordPRO
    Inherits System.Web.UI.Page

    Private idPasswordDaRinnovare As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        idPasswordDaRinnovare = Request.QueryString.Get("id")
    End Sub

    Private Sub btnConferma_ServerClick(sender As Object, e As EventArgs) Handles btnConferma.ServerClick
        Dim nuovaPassword = txtNuovaPassword.Value
        Dim ripetiNuovaPassword = txtRipetiNuovaPassword.Value
        If String.IsNullOrEmpty(nuovaPassword) Then
            alertMessageBox("Nuova password non impostata")
            Return
        End If
        Dim passwordOk = controllaNuovaPassword(nuovaPassword)
        If Not passwordOk Then
            Dim msgControlloNuovaPassword = "La password non soddisfa tutti i requisiti: " &
                "8 caratteri," &
                "1 lettera minuscola, " &
                "1 lettera maiuscola, " &
                "1 numero, " &
                "1 carattere speciale (ovvero non alfanumerico)"
            alertMessageBox(msgControlloNuovaPassword)
            Return
        End If
        If nuovaPassword <> ripetiNuovaPassword Then
            alertMessageBox("Le password impostate non coincidono")
            Return
        End If
        alertMessageBox(modificaPassword())
    End Sub

    Protected Sub alertMessageBox(messaggio As String)
        Dim sb As New StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        sb.Append("window.onload=function(){")
        sb.Append("alert('")
        sb.Append(messaggio)
        sb.Append("')};")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub

    Private Function controllaNuovaPassword(nuovaPassword As String) As Boolean

        Return AgronicaCoreUtentiBIZ.Utenti.ValidaComplessitaPassword(nuovaPassword)

    End Function

    Private Function modificaPassword() As String

        Dim inDataJson = getParametriJsonModificaPassword()

        Dim baseUrlApiProfitosan As String = ConfigurationManager.AppSettings("BaseUrlApiProfitosan")
        Dim tokenProfitosan As String = ConfigurationManager.AppSettings("tokenProfitosan")

        Dim urlApiProfitosanModificaPassword = baseUrlApiProfitosan & "/provisioning/ModificaPassword"

        Dim chiamataRest As New AgronicaCoreUtility.Http

        Dim DatiSrv As AgronicaCoreUtility.Http.JsonRestResponse = chiamataRest.PostWS_RestSharp_JSON(
            urlApiProfitosanModificaPassword,
            tokenProfitosan,
            inDataJson)

        Dim msgModificaPassword As String = ""

        Try
            Select Case True
                Case IsNothing(DatiSrv.StatusCode) OrElse IsNothing(DatiSrv.Content)
                Case DatiSrv.StatusCode = 0 OrElse String.IsNullOrEmpty(DatiSrv.Content)
                    msgModificaPassword = "Nessuna risposta dal server"
                Case DatiSrv.StatusCode = 200 AndAlso Not String.IsNullOrEmpty(DatiSrv.Content)
                    Dim response = JsonConvert.DeserializeObject(Of ProvisioningStandardResponse)(DatiSrv.Content)
                    msgModificaPassword = response.Payload.FirstOrDefault().esitoTxt
                Case Else
                    Dim response = JsonConvert.DeserializeObject(Of ProvisioningStandardResponse)(DatiSrv.Content)
                    msgModificaPassword = response.errore
            End Select
        Catch ex As Exception
            msgModificaPassword = String.Format("Errore inaspettato ({0})", ex.Message)
        End Try

        Return msgModificaPassword

    End Function

    Private Function getParametriJsonModificaPassword() As String

        Dim utente As New Utente()

        utente.username = idPasswordDaRinnovare
        utente.password = txtNuovaPassword.Value

        Dim inData As New InData()

        inData.utente = utente

        Return JsonConvert.SerializeObject(inData)

    End Function

    Private Class InData
        Property utente As Utente
    End Class

    Private Class Utente
        Property username As String
        Property password As String
    End Class

    Private Class ProvisioningStandardResponse
        Property rispostaOk As Boolean
        Property errore As String
        Property payload As List(Of ProvisioningDetailResponse)
    End Class

    Private Class ProvisioningDetailResponse
        Property esitoBool As Boolean
        Property esitoTxt As String
    End Class

End Class