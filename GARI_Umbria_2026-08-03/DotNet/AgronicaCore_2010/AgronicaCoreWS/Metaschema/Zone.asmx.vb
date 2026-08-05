Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Zone
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))


        Try
            Dim objParametri_server As AgronicaCoreParametri
            objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim objR As New AgronicaCoreAnagrafeDAL.Zone_R

            Dim dt As DataTable = objR.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)


            Dim listZone = (From dr In dt.Rows Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(dr("Zona_Cod"), dr("Descrizione"))).ToList


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


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiLingue(ByVal InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))
        Dim res As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr))

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If

        Dim objR As New AgronicaCoreAnagrafeDAL.Zone_R

        Try
            Dim objParametri_server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim DT = objR.LeggiLingue(objParametri_server)

            Dim listLingue As List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr) =
                (From dr In DT.Rows Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(dr("Lingua_Cod"), dr("Descrizione"))).ToList()

            res.RispostaStringa = listLingue
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

End Class