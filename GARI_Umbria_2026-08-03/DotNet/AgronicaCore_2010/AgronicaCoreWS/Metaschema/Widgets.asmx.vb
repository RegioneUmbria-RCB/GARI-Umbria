Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreModelsSTD.Widgets


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Widgets
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ElencoWidgets(ByVal InData As Object) As rispostaStandard(Of List(Of Widget))

        Dim r As New rispostaStandard(Of List(Of Widget))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widgets_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widgets_In))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim objMeta As New AgronicaCoreMetaSchemaBIZ.Widgets
            Dim widgets = objMeta.LeggiWidgets(params.InData.Codice,
                                          params.InData.Titolo,
                                          params.InData.Descrizione,
                                          objParametri_Server,
                                          objParametri_Utenti,
                                          Nothing,
                                          params.InData.Abilitato,
                                          params.InData.PresetIniziale
                                            )

            r.RispostaStringa = widgets
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function



End Class

