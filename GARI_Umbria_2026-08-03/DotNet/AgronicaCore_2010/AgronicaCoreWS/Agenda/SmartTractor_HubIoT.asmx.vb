Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.SmartTractors_HubIoT
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaPrecisionFarming
Imports Newtonsoft.Json


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class SmartTractor_HubIoT
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiParametriConnessioni(ByVal InData As Object) As rispostaStandard(Of ParametriConnessioni_Out)
        Dim r As New rispostaStandard(Of ParametriConnessioni_Out)

        Dim objParametri = DeserializzaInData(Of List(Of ParametriConnessioni_In))(InData)
        Dim objPivaList As List(Of ParametriConnessioni_In) = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            r = HubIoT_Helper.LeggiParametriConnessioni(objPivaList, objParametri.Server, objParametri.Utenti)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaParametriConnessioni(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of SalvaParametriConnessioni_In)(InData)
        Dim objParametriConnessioniList As SalvaParametriConnessioni_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            r = HubIoT_Helper.SalvaParametriConnessioni(objParametriConnessioniList.parList, objParametri.Server, objParametri.Utenti)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function InviaRicetta(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of List(Of RicettaOperazione2WorkOrderKey))(InData)
        Dim objRicetteList As List(Of RicettaOperazione2WorkOrderKey) = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            r = HubIoT_Helper.ScriviWorkOrderKeyDaRicettaOperazione(objRicetteList, objParametri.Server, objParametri.Utenti)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


#Region "Private"
    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    Private Shared Sub Gias_InizializzaCultura_DaParams(ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read

        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Utenti)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")

        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    End Sub
#End Region


End Class