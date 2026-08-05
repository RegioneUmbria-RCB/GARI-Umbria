Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Globalization
Imports AgronicaCoreContabObject
Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility

Imports AgronicaCoreModello.Agenda
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class CentriAziendali
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getCentriAziendali_APP(ByVal piva As String,
                                           ByVal Data As String,
                                           ByVal objP_super_server As String,
                                           ByVal objP_server As String,
                                           ByVal objP_utenti As String
                                           ) As RispostaStandard
        
        Dim r As New RispostaStandard

        Try

            'TODO: fare fix definitivo per data
            Dim data_Date As Date

            If Date.TryParseExact(Data, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                data_Date = Date.ParseExact(Data, "dd/MM/yyyy",
                                            CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            ElseIf Not IsDate(Data) Then
                data_Date = Today
            End If

            Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim dtCentriAziendali As DataTable
            Dim objCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim xFiltroAggiuntivo As New StringBuilder
            xFiltroAggiuntivo.AppendLine("         ( Validita_inizio <= " & Agro_SQL_SaveDate(data_Date) & " ")
            xFiltroAggiuntivo.AppendLine("         AND Validita_Fine >= " & Agro_SQL_SaveDate(data_Date) & " ) ")

            dtCentriAziendali = objCentriAziendali.Leggi(piva,
                                                         0,
                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                         xFiltroAggiuntivo.ToString,
                                                         " Sa_Nome ",
                                                         objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtCentriAziendali, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class