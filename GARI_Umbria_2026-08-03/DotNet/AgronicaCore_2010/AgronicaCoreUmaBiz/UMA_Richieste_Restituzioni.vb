Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Transactions

Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUmaDal.UMA_Richieste_Restituzioni_R
Public Class UMA_Richieste_Restituzioni_BIZ

    Inherits AgronicaCoreDataProvider.LogProvider
    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable



        Dim AgronicaDAL As New UMA_Richieste_Restituzioni_R
        Dim MessaggioErrore As String
        Dim result As DataTable
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "UMA_Richieste_Restituzioni_BIZ.Leggi()"
        Try
            result = AgronicaDAL.Leggi(piva, Richiesta_Cod, objParametri)
        Catch ex As Exception
            result = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result

    End Function

    Public Function Salva_Restituzioni(ByRef objParametri As AgronicaCoreParametri,
                                                righeinseriteJson As String,
                                                righeModificateJson As String,
                                                righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMA_Richieste_Restituzioni_BIZ.Salva_Rendicontazioni()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String = ""

        Dim AgronicaDAL As New UMA_Richieste_Restituzioni_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        '------------------------------- Carica Entity Framework ----------------------------------------
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        '------------------------------ Deserializza le righe di input ------------------------------
        Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim righeInseriteArr As List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni) = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni))(righeinseriteJson, deserializerSettings)
        Dim righeModificateArr As List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni) = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni))(righeModificateJson, deserializerSettings)
        Dim righeCancellateArr As List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni) = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Restituzioni_W.RichiesteRestituzioni))(righeCancellateJson, deserializerSettings)
        Dim Errori = String.Empty
        'Try
        '    '------------------------------ Verifica validità righe input ------------------------------    

        '    ControlliValidita_LavorazioniUMA(righeInseriteArr, righeModificateArr, righeCancellateArr, objParametri, efConnString, Errori)
        '    ' If (String.IsNullOrEmpty(Errori)) Then Errori = "pippo"
        '    If (Not String.IsNullOrEmpty(Errori)) Then
        '        risposta.RispostaOK = False
        '        risposta.Errore = Errori
        '        Return risposta
        '    End If
        'Catch ex As Exception


        '    MessaggioErrore = ex.Message
        '    Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
        '    Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        'End Try
        Using scope As New TransactionScope()

            Try
                ' ------------------------------------- Salva i dati nel database -------------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                Dim context As New Gias_DeveloperServer_Entities(efConnString)


                If String.IsNullOrEmpty(Errori) Then
                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInseriteArr Is Nothing AndAlso righeInseriteArr.Count > 0 Then
                        AgronicaDAL.AggiungiNuovi(righeInseriteArr, objParametri)
                    End If

                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeModificateArr Is Nothing AndAlso righeModificateArr.Count > 0 Then
                        AgronicaDAL.Aggiorna(righeModificateArr, objParametri)
                    End If

                    '------------------------- Rimuovi le righe cancellate -------------------------
                    If Not righeCancellateArr Is Nothing AndAlso righeCancellateArr.Count > 0 Then

                        AgronicaDAL.Rimuovi(righeCancellateArr, objParametri)

                    End If
                End If
                scope.Complete()
                scope.Dispose()

                risposta.RispostaOK = True
                risposta.RispostaStringa = "L'operazione è stata completata con successo."

                Return risposta
            Catch ex As Exception
                scope.Dispose()

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
                Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
            Finally
                scope.Dispose()
            End Try
        End Using
    End Function

End Class
