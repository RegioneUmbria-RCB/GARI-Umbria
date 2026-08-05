Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class QualitaCatasto
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))


        Try
            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim objR As New AgronicaCoreMetaSchemaDAL.QualitaCatasto_R

            Dim dt As DataTable = objR.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)


            Dim listZone = (From dr In dt.Rows Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(dr("Qualita_Cod"), dr("Qualita_Des"))).ToList


            r.RispostaOK = True
            r.RispostaStringa = listZone

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

End Class