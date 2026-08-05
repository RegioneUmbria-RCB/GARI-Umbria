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
Public Class Zoo_Animali
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Giacenze_Zoo(ByVal piva As String,
                                 ByVal sa_cod As Integer,
                                 ByVal STA_NUM As Integer,
                                 ByVal Raggruppamento_Cod As Integer,
                                 ByVal Cod_Animale As Integer,
                                 ByVal Data As DateTime,
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

            Dim ZooBIZ As New AgronicaCoreAnagrafeDAL.Zoo_Animali

            Dim dt = ZooBIZ.Leggi_Giacenze(piva, sa_cod, STA_NUM, Raggruppamento_Cod, Cod_Animale, Data, objParametri_Server)

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

End Class