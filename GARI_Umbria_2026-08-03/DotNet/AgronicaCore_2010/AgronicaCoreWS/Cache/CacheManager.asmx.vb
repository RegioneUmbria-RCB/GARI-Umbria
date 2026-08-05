Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaControlli_2010


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class CacheManager
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    Public Function PulisciCache() As RispostaStandard

        Dim r As New RispostaStandard

        Try

            ' inizializza stringhe connessione in application
            GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

            ' Cache core ws
            MemoryCacheFactory.Instance.Pulisci_Tuttto()

            If Not IsNothing(MemoryCacheFactory.Instance.COREWS_CACHE) Then
                Dim tutteLeChiavi = MemoryCacheFactory.Instance.COREWS_CACHE.Select(Function(k) k.Key).ToList
                For Each c As String In tutteLeChiavi
                    If MemoryCacheFactory.Instance.COREWS_CACHE.Contains(c) Then
                        MemoryCacheFactory.Instance.COREWS_CACHE.Remove(c)
                    End If
                Next
            End If

            r.RispostaStringa = "Cache AgronicaCoreWS pulita correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function PulisciCacheEFactory(objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try


            If Not String.IsNullOrEmpty(objP_server) Then
                Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
                If Not IsNothing(objParametri_Server) Then
                    ConfigurazioneAjaxFactory.Reset(objParametri_Server)
                End If
            End If

            ' Cache core ws
            MemoryCacheFactory.Instance.Pulisci_Tuttto()

            r.RispostaStringa = "Cache + Factories AgronicaCoreWS pulite correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function PulisciCacheMetodo(NomeMetodo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            ' inizializza stringhe connessione in application
            GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

            ' Cache core ws
            Dim elencoCache = MemoryCacheFactory.Instance.Elenca_Tuttto()

            For Each cacheElem In elencoCache
                If cacheElem.NomeMetodo = NomeMetodo Then
                    MemoryCacheFactory.Instance.COREWS_CACHE.Remove(cacheElem.Chiave)
                End If
            Next

            r.RispostaStringa = "Cache AgronicaCoreWS Impostazioni pulita correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

End Class