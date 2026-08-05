Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaControlliGIS
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.GiasApp
Imports AgronicaCoreModelsSTD.GiasAPP
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class GiasApp
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Gestisce la sottoscrizione alle notifiche push ad un servizio per un utente
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PushNotification(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of Notifica_Utente_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim objBiz As New AgronicaCoreGiasAppBIZ.Notifica_Utente_W

        If String.IsNullOrEmpty(objParametri.InData.Piattaforma) Then
            objParametri.InData.Piattaforma = CostantiPersonalizzate.Notifiche_Piattaforma_Default
        End If

        Try
            r.RispostaOK = objBiz.PushNotifica(objParametri.InData.SubscriberID,
                                               objParametri.InData.IDServizio,
                                               objParametri.InData.Piattaforma,
                                               objParametri.Server,
                                               objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    ''' <summary>
    ''' Gestisce la sottoscrizione alle notifiche push ad un servizio per un utente
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PopNotification(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of Notifica_Utente_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim objBiz As New AgronicaCoreGiasAppBIZ.Notifica_Utente_W

        Try
            r.RispostaOK = objBiz.PopNotifica(objParametri.InData.SubscriberID,
                                              objParametri.InData.IDServizio,
                                              objParametri.Server,
                                              objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    ''' <summary>
    ''' Gestisce la sottoscrizione alle notifiche push ad un servizio per un utente
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ServiziNotificheSottoscrivibili(ByVal InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of ServiziSottoscrivibili_Out)
        Dim r As New rispostaStandard(Of ServiziSottoscrivibili_Out)

        Dim objParametri = DeserializzaInData(Of CoreWS_Generic(Of Object))(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim xRead As New AgronicaCoreGiasAppBIZ.Notifica_Utente_R
        Dim DT As DataTable

        Try
            Dim result As New ServiziSottoscrivibili_Out
            result.ListaServizi = New List(Of ServiziSottoscrivibili)

            DT = xRead.LeggiNotificheSottoscrivibili(objParametri.Server, objParametri.Utenti)

            If DT IsNot Nothing Then

                For Each row In DT.Rows
                    Dim servizio As New ServiziSottoscrivibili

                    servizio.IdServizio = Convert.ToInt32(row("IdServizio"))
                    servizio.Descrizione = row("Descrizione").ToString
                    servizio.Obbligatorio = Convert.ToBoolean(row("Obbligatorio"))
                    servizio.Sottoscrivi = Convert.ToBoolean(row("Sottoscrivi"))
                    servizio.DeepLink = row("DeepLink").ToString
                    servizio.UrlMedia = row("UrlMedia").ToString

                    result.ListaServizi.Add(servizio)
                Next

            End If

            r.RispostaOK = True
            r.RispostaStringa = result

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

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

End Class