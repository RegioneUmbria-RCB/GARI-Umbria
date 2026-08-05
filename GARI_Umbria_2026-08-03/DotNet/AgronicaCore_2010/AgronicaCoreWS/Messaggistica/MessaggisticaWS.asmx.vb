Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class MessaggisticaWS
    Inherits System.Web.Services.WebService

    'Classe per la "deserializzazione" dell'oggetto in entrata dalle nuove chiamate API ai WebMethod
    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    'Viene utilizzata per la "deserializzazione" dell'oggetto in entrata dalle nuove chiamate API ai WebMethod
    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    Private Shared Sub Gias_InizializzaCultura_DaParams(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read

        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Utenti)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")

        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AccodaMessaggioEsecuzione(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of MessaggioEsecuzione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        Try
            resp.RispostaOK = xWrite.AccodaMessaggioEsecuzione(objParametri.InData.Destinatario_UserName,
                                                               objParametri.InData.Testo_Messaggio,
                                                               objParametri.Server)

            resp.RispostaStringa = "Messaggio accodato con successo."
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMessaggiEsecuzioneNonLetti(ByVal InData As Object) As rispostaStandard(Of List(Of MessaggioEsecuzione))

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of MessaggioEsecuzione))

        Dim xRead As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_R

        Try
            resp.RispostaOK = True

            resp.RispostaStringa = xRead.LeggiMessaggiEsecuzioneNonLetti(objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMessaggioEsecuzione(ByVal InData As Object) As rispostaStandard(Of MessaggioEsecuzione)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of MessaggioEsecuzione)

        Dim xRead As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_R

        Try
            resp.RispostaOK = True

            resp.RispostaStringa = xRead.LeggiMessaggioEsecuzione(objParametri.InData, objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaMessaggioEsecuzione(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of MessaggioEsecuzione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        Try
            resp.RispostaOK = xWrite.ModificaMessaggioEsecuzione(objParametri.InData.ID_Messaggio,
                                                                 objParametri.InData.letto,
                                                                 objParametri.InData.annullato,
                                                                 objParametri.Server)

            resp.RispostaStringa = "Modifica avvenuta con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

End Class