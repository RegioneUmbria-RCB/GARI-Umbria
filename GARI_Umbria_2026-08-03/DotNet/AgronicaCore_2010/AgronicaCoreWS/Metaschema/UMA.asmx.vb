Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class UMA
    Inherits System.Web.Services.WebService

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUMA_Lavorazioni_NG(ByVal InData As CoreWS_Generic(Of LeggiUMA_Lavorazioni)) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim lavorazioni_r As New AgronicaCoreMetaSchemaDAL.UMA_Lavorazioni_R

            Dim lista = lavorazioni_r.Leggi(InData.InData.Lav_UMA_Cod, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUMA_Lavorazioni(Lav_UMA_Cod As String,
                                                         Ordinamento As String,
                                                         objP_server As String) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim lavorazioni_r As New AgronicaCoreMetaSchemaDAL.UMA_Lavorazioni_R

            Dim lista = lavorazioni_r.Leggi(Lav_UMA_Cod, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUMA_Macrousi_NG(ByVal InData As CoreWS_Generic(Of LeggiUMA_Macrousi)) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim macrousi_r As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R

            Dim lista = macrousi_r.Leggi(InData.InData.Macrouso_UMA_Cod, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUMA_Macrousi(Macrouso_UMA_Cod As String,
                                                         Ordinamento As String,
                                                         objP_server As String) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.CaricaComboTipologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim macrousi_r As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R

            Dim lista = macrousi_r.Leggi(Macrouso_UMA_Cod, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
End Class