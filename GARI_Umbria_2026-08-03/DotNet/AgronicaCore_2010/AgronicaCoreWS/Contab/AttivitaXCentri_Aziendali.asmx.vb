Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class AttivitaXCentri_Aziendali
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(ByVal piva As String,
                              ByVal Utilizzo_GiasAPP As Boolean,
                              ByVal flag_inclusa As Integer,
                              ByVal objP_super_server As String,
                              ByVal objP_server As String,
                              ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objAttivitaXCentriAziendali As New AgronicaCoreAnagrafeDAL.AttivitaXCentri_Aziendali_R
            r.RispostaStringa = objAttivitaXCentriAziendali.Leggi(piva, 0, 0, flag_inclusa, Utilizzo_GiasAPP, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


End Class