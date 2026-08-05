Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Macrousi
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetMacroUsi(objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try
            Dim objParametri_server As AgronicaCoreParametri
            If objP_server = "" Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If

            Dim objR As New AgronicaCoreMetaSchemaDAL.Macrousi_R

            Dim dt As DataTable = objR.Leggi("", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)


            Dim lista As New List(Of String)

            For Each dr As DataRow In dt.Rows
                lista.Add("{""macrouso"":""" & jSon.Escape(dr.Item("macrouso_des")) & """, ""macrouso_cod"":""" & dr.Item("macrouso_cod") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista) & "]"


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.metaschema.Macrouso)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Macrouso))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Macrouso))


        Try
            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim objR As New AgronicaCoreMetaSchemaDAL.Macrousi_R

            Dim dt As DataTable = objR.Leggi("", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)


            Dim listMacrousi = (From dr In dt.Rows Select New AgronicaCoreModelsSTD.metaschema.Macrouso(dr("Macrouso_Cod")) With {
                .descrizione = dr("Macrouso_Des")
            }).ToList


            r.RispostaOK = True
            r.RispostaStringa = listMacrousi

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

End Class