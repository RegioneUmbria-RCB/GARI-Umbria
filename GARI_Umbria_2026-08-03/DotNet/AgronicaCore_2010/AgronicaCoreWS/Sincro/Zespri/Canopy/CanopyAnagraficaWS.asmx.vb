Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
<System.Web.Script.Services.ScriptService()>
Public Class CanopyAnagraficaWS
    Inherits System.Web.Services.WebService



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SincronizzazioneCanopyAnagrafica(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, InData As SincronizzazioneCanopyAnagraficaInData) As rispostaStandard(Of SincronizzazioneCanopyAnagraficaOutData)
        Dim r As New rispostaStandard(Of SincronizzazioneCanopyAnagraficaOutData)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            r.RispostaOK = True
            r.RispostaStringa = New SincronizzazioneCanopyAnagraficaOutData

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function



End Class