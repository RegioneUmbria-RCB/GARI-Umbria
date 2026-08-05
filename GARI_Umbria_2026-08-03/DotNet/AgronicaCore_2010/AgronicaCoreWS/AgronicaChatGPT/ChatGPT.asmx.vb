Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.AgronicaChatGPT
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.Widgets


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class ChatGPT
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckTrattamento(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim Risp As String = ""

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strxLeggiDisponibilita As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataCheck As CheckTrattamento = JsonConvert.DeserializeObject(Of CheckTrattamento)(strxLeggiDisponibilita,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objAgroChatGPT As New Agronica.ChatGPT.AgroChatGPT

            Risp = objAgroChatGPT.CheckTrattamentoByProfitosanDPIRegionale(inDataCheck.utilizzoTerreno, inDataCheck.dettaglioTrattamento, inDataCheck.disciplinare, inDataCheck.avversitaGruppo, objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = Risp

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PrevisioniCostiRicavi(InData As CoreWS_Generic(Of PrevisioniChatGPT_IN)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim Risp As String = ""

        Try

            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim dettCrops As PrevisioniChatGPT_IN = InData.InData

            Dim objAgroChatGPT As New Agronica.ChatGPT.AgroChatGPT

            Risp = objAgroChatGPT.getCostsAndRevenuesEstimates(dettCrops, objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = Risp

        Catch ex As Exception

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function


End Class