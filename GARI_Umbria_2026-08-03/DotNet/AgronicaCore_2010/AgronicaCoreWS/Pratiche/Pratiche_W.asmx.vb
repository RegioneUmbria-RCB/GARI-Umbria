Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Utility
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Pratiche_W
    Inherits System.Web.Services.WebService

    <Script.Services.ScriptMethod()>
    <WebMethod()>
    Public Function GeneraPratica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = False

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Pratiche.GeneraPratica) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Pratiche.GeneraPratica))(JsonConvert.SerializeObject(InData), a)

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim xPraticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            Dim Pratica_Cod As Integer = 0
            Dim res = xPraticheW.GeneraPratica(iData.InData.piva,
                                                  iData.InData.servizio_cod,
                                                  iData.InData.Data_Inizio.ToString("yyyy-MM-dd"),
                                                  iData.InData.Data_Fine.ToString("yyyy-MM-dd"),
                                                  iData.InData.Data_Inizio_Pratica.ToString("yyyy-MM-dd"),
                                                  iData.InData.Numero,
                                                  objP_Server,
                                                  objP_Utenti,
                                                  True,
                                                  Pratica_Cod)

            If res.RispostaOK = True Then
                If Pratica_Cod <= 0 Then
                    Throw New Exception("Pratica non generata.")
                Else
                    r = res
                    r.RispostaStringa = Pratica_Cod
                End If
            End If

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.RispostaOK = False
            r.Errore = MessaggioErrore
        End Try
        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod()>
    Public Function Pratica_PassaggioDiStato_SuperUser_SenzaVerifiche(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = False

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Pratiche.Pratica_AvanzaStato) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Pratiche.Pratica_AvanzaStato))(JsonConvert.SerializeObject(InData), a)
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)
            Dim xPraticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objP_Server.StringaConnessione)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim res = xPraticheW.Esegui_PassaggioDiStato_SuperUser_SenzaVerifiche(GiasContext,
                                                                            iData.InData.piva,
                                                                            iData.InData.pratica_cod,
                                                                            iData.InData.servizio_cod,
                                                                            iData.InData.statoFinaleRichiesto,
                                                                            iData.InData.note,
                                                                            objP_Server)
                If res Then
                    GiasContext.SaveChanges()
                Else
                    Throw New Exception("Errore in avanzamento pratica. operazione annullata.")
                End If
            End Using
            r.RispostaOK = True
            r.Errore = ""
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String =
                "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.RispostaOK = False
            r.Errore = MessaggioErrore
        End Try
        Return r
    End Function

    ''' <summary>
    ''' [Usato per Coldiretti] Verifica se la tipologia applicata all'utente è IV o CuraAzienda per verificare l'abilitazione all'inserimento del QDC
    ''' </summary>
    ''' <param name="InData"></param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckUtenteTipologiaAccessoQDC(InData As CoreWS_Generic(Of UtenteTipologiaAccessoQDC)) As rispostaStandard(Of Boolean)
        Dim res As New rispostaStandard(Of Boolean)
        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objCheckServizio = New AgronicaCoreVarieBIZ.SottoscrizioneServizioQDC

        Try
            res.RispostaStringa = objCheckServizio.VerificaSottoscrizioneServizioQDC(InData.InData.piva, InData.InData.data, obj_Server)
            res.RispostaOK = True

        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

End Class