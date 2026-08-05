Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class ParamEntrataXSpecieVarieta
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Percentuale_Degrado(
                                        ByVal Piva As String,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer?,
                                        ByVal Reg_Cod As Integer?,
                                        ByVal Data_Riferimento As DateTime,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal objP_server As String) As RispostaStandard

        Return Leggi_ParamEntrataXSpecieVarieta(Piva,
                                                Veg_Cod,
                                                Cul_Cod,
                                                Reg_Cod,
                                                Data_Riferimento,
                                                xFiltroAggiuntivo,
                                                objP_server, TipoParamRichiesto.Degrado)

    End Function

    Private Function Leggi_ParamEntrataXSpecieVarieta(ByVal Piva As String,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer?,
                                        ByVal Reg_Cod As Integer?,
                                        ByVal Data_Riferimento As DateTime,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal objP_server As String,
                                        ByVal valoreRitorno As TipoParamRichiesto) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If

            Dim dal As New AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_R

            ' Primo tentativo a chiave completa
            Dim dt As DataTable = dal.Leggi(Piva,
                                            Veg_Cod,
                                            Cul_Cod,
                                            Reg_Cod,
                                            Nothing,
                                            "",
                                            Data_Riferimento,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            xFiltroAggiuntivo, "", objParametri_server)


            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                ' Secondo tentativo senza regolamento con varietà
                dt = dal.Leggi(Piva,
                                Veg_Cod,
                                Cul_Cod,
                                0,
                                Nothing,
                                "",
                                Data_Riferimento,
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo, "", objParametri_server)
            End If

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                ' Terzo tentativo senza varietà con regolamento 
                dt = dal.Leggi(Piva,
                                Veg_Cod,
                                0,
                                Reg_Cod,
                                Nothing,
                                "",
                                Data_Riferimento,
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo, "", objParametri_server)
            End If

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                ' Quarto tentativo senza varietà e senza regolamento 
                dt = dal.Leggi(Piva,
                                Veg_Cod,
                                0,
                                0,
                                Nothing,
                                "",
                                Data_Riferimento,
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                xFiltroAggiuntivo, "", objParametri_server)
            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            serializerSettings.Culture = Globalization.CultureInfo.CurrentCulture
            If dt.Rows.Count > 0 Then
                If valoreRitorno = TipoParamRichiesto.Degrado Then
                    'r.RispostaStringa = JsonConvert.SerializeObject(dt.AsEnumerable().FirstOrDefault().Item("Percentuale_Degrado"), Formatting.None, serializerSettings)
                    r.RispostaStringa = Convert.ToDecimal(dt.AsEnumerable().FirstOrDefault().Item("Percentuale_Degrado"), Globalization.CultureInfo.CurrentCulture).ToString()
                Else
                    r.RispostaStringa = JsonConvert.SerializeObject(dt.AsEnumerable().FirstOrDefault().Item("Riferimento_Prezzi"), Formatting.None, serializerSettings)
                End If
            Else
                If valoreRitorno = TipoParamRichiesto.Degrado Then
                    r.RispostaStringa = JsonConvert.SerializeObject(0, Formatting.None, serializerSettings)
                Else
                    ' Default data entrata
                    r.RispostaStringa = JsonConvert.SerializeObject("E", Formatting.None, serializerSettings)
                End If
            End If

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = MessaggioErrore

        End Try

        Return r

    End Function

    Private Function GetJson(ByVal dt As DataTable) As String
        Dim dictionary = From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col))
        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        Return JsonConvert.SerializeObject(dictionary, Newtonsoft.Json.Formatting.None, serializerSettings)
    End Function

    Private Enum TipoParamRichiesto
        Degrado = 0
        RiferimentoPrezzo = 1
    End Enum

End Class
