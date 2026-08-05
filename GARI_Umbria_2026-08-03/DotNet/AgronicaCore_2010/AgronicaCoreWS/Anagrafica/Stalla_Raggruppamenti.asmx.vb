Imports System.Web.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework
Imports System.Transactions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Stalla_Raggrupamenti
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_Raggruppamenti_Stalle(ByVal piva As String,
                                                        ByVal sa_cod As Integer,
                                                        ByVal STA_NUM As Integer,
                                                        ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim lista_raggruppamenti = From s In GiasContext.Stalla_Raggruppamenti Where s.PIVA = piva And s.sa_cod = sa_cod And s.STA_NUM = STA_NUM

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(lista_raggruppamenti.ToList)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

            GiasContext.Dispose()

        End Try

        Return r

    End Function


End Class