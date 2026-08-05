Imports System.Web.Services
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports InData.Agenda
Imports InData.Anagrafica
Imports InData.Statistiche
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class StatisticheApp
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function LeggiRilieviProduzione(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiRilieviProduzione))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAppHelper As New Statistiche_R
            Dim dati = objAppHelper.Leggi_Rilievi_Stime_Produzione(InData.InData.esercizi, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dati)

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiStimeProduzione(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiStimeProduzione))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objAppHelper As New Statistiche_R
            Dim parametri As LeggiStimeProduzione = InData.InData
            Dim progetti As List(Of Integer) = (From i In parametri.esercizi.Split(",") Select CInt(i)).Distinct.ToList
            Dim dati = objAppHelper.Leggi_Report_Stime_Produzione(progetti, "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dati)

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

End Class