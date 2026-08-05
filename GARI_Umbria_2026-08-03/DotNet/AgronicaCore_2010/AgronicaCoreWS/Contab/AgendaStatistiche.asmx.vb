Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports InData.Agenda
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class AgendaStatistiche
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaStatistiche(InData As CoreWS_Generic(Of Object)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
        Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

        objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
        objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
        objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

        Dim inDataParsed As LeggiAgendaStatistiche = JsonConvert.DeserializeObject(Of LeggiAgendaStatistiche)(JsonConvert.SerializeObject(InData.InData))

        Try

            Dim stat_agenda As AgronicaCoreContabBIZ.AgendaStatistiche = New AgronicaCoreContabBIZ.AgendaStatistiche
            r.RispostaStringa = JsonConvert.SerializeObject(stat_agenda.CaricaOperazioniStatistiche(inDataParsed, objParametri_Server, objParametri_Utenti))
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


End Class