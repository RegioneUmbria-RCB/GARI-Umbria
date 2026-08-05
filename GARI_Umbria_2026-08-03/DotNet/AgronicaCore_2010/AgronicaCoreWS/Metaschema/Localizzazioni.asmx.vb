Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Localizzazioni
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiLocalizzazioni_Modello(InData As Object) As rispostaStandard(Of List(Of metaschema.Localizzazione))

        Dim r As New rispostaStandard(Of List(Of metaschema.Localizzazione))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiLocalizzazioni))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Localizzazioni As LeggiLocalizzazioni = objRequest.InData

            Dim strErr As String = ""

            Dim objLimitazioni As New AgronicaControlli_2010.STD_Limitazioni

            Dim localizzazioniList = objLimitazioni.LeggiLocalizzazioni_Modello(objParametri_Localizzazioni.lavorazione,
                                                                                objParametri_Localizzazioni.dettaglioTrattamento,
                                                                                objParametri_Localizzazioni.disciplinare,
                                                                                objParametri_Localizzazioni.avversitaGruppo,
                                                                                objParametri_Localizzazioni.specie,
                                                                                strErr,
                                                                                objParametri_Super_Server,
                                                                                objParametri_Server,
                                                                                objParametri_Utenti)
            If Not String.IsNullOrEmpty(strErr) Then

                r.Errore = strErr

                r.RispostaOK = False
            Else
                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = localizzazioniList

                r.RispostaOK = True
            End If


        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class