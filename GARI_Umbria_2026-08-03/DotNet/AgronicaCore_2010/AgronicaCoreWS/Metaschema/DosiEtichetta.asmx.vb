Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class DosiEtichetta
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_DosiEtichetta_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DoseEtichetta))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DoseEtichetta))

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiDosiEtichetta))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_DosiEtichetta As LeggiDosiEtichetta = objRequest.InData

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

            Dim DosiEtichettaBiz As New AgronicaControlli_2010.STD_DosiEtichetta
            Dim dosiEtichettaList = DosiEtichettaBiz.LeggiDosiEtichetta(objParametri_DosiEtichetta.specie,
                                                                        objParametri_DosiEtichetta.dettaglioTrattamento,
                                                                        objParametri_DosiEtichetta.avversitaGruppo,
                                                                        objParametri_DosiEtichetta.impianti,
                                                                        objParametri_DosiEtichetta.prodottiDaTrattare,
                                                                        objParametri_DosiEtichetta.data,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

            r.RispostaStringa = dosiEtichettaList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

End Class