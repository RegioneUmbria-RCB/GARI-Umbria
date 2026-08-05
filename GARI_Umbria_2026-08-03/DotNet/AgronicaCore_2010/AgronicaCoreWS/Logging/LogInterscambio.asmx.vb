Imports System.ComponentModel
Imports System.Globalization
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModello
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports InData.Agenda
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDTOStd.InData.GiasApp

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class LogInterscambio
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_isModalitaDemetra(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard
        Dim NomeRoutine As String = "Get_isModalitaDemetra"


        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R
            Dim isModalitaDemetra As Boolean = objChiamate.Get_LoggingIsModalitaDemetra(objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(isModalitaDemetra)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ConsultaLogInterscambio(InData As Object) As rispostaStandard(Of OutData.Logging.MonitorLogInterscambio_OUT)

        Dim r As New rispostaStandard(Of OutData.Logging.MonitorLogInterscambio_OUT)
        Dim NomeRoutine As String = "ConsultaLogInterscambio"


        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ConsultaSincroDatiApp))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_R
            Dim dati = objChiamate.Consulta_Log_Interscambio(objRequest.InData.tipiDato,
                                                             objRequest.InData.dataFiltro_inizio,
                                                             objRequest.InData.dataFiltro_fine,
                                                             objRequest.InData.filtroImportati,
                                                             objRequest.InData.pivaCUAA,
                                                             objParametri_Server)

            Dim MsgCodaDataPublish As String = ""
            If objRequest.InData.tipiDato.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish) OrElse objRequest.InData.tipiDato.Count = 0 Then
                Dim objRicezioneNotifiche As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_R
                'DCA20251216: devo filtrare anche per CUAA -> task 200862
                Dim objImpreseCodidiDAL As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim cuaa As String = objImpreseCodidiDAL.Leggi_CUAA(objRequest.InData.pivaCUAA, objParametri_Server)

                If cuaa = "" Then   'significa che quella passata o è già il CUAA, oppure è un dato sbagliato
                    cuaa = objRequest.InData.pivaCUAA
                End If

                MsgCodaDataPublish = objRicezioneNotifiche.LeggiCodaNotificheDaElaborare(objParametri_Server, 0, cuaa)
            End If

            Dim res As New OutData.Logging.MonitorLogInterscambio_OUT With {
                .ListaLog = dati,
                .MsgCodaDataPublish = MsgCodaDataPublish
            }

            r.RispostaOK = True
            r.RispostaStringa = res

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

End Class
