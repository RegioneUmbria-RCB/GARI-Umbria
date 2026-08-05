Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Tessiture
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_ClasseTessitura(InData As CoreWS_Generic(Of LeggiClasseTessitura)) As rispostaStandard(Of ClasseTessitura)

        Dim r As New rispostaStandard(Of ClasseTessitura)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiClasseTessitura))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_ClasseTessitura As LeggiClasseTessitura = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim objMetaschema As New AgronicaControlli_2010.STD_PianoConcimazione
            Dim classeTessitura = objMetaschema.LeggiClasseTessitura(objParametri_ClasseTessitura.Sabbia,
                                                                     objParametri_ClasseTessitura.Argilla,
                                                                     objParametri_Super_Server,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti)

            r.RispostaStringa = classeTessitura
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

End Class