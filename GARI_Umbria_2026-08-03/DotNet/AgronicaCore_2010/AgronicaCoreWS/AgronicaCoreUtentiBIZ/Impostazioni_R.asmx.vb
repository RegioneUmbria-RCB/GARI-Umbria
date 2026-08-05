Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Impostazioni_R
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Impostazione_Scalare(ByVal piva As String,
                                             ByVal sa_cod_list As List(Of Integer),
                                             ByVal impostazione_cod As Integer,
                                             ByVal objP_server As String,
                                             ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim objLeggi As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim strImpostazioni = objLeggi.LeggiScalareMulticentroAziendaSuperUser(piva, sa_cod_list, impostazione_cod, "", objParametri_Utenti, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = strImpostazioni
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class