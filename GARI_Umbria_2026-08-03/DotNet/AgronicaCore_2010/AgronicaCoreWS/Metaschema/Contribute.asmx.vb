Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Contribute
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Read(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.metaschema.Contribute)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Contribute))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Contribute))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objContribute As New AgronicaCoreMetaSchemaBIZ.Contribute_R

            Dim contributes = objContribute.Read(
                objServer:=objServer,
                objUtenti:=objUtenti,
                code:=InData.InData.code,
                type:=InData.InData.type,
                description:=InData.InData.description,
                startValidity:=InData.InData.validity.inizio,
                endValidity:=InData.InData.validity.fine
                )

            r.RispostaStringa = contributes
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class