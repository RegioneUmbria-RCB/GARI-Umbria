Imports System.Transactions
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabDAL.ws_Canopy_Anagrafica_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class ws_Canopy_Anagrafica
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Creazione oggetto Block
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    Public Function Canopy_CreazioneOggettoBlock(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal dt As DataTable) As List(Of ws_Canopy_Anagrafica_Dto)
        Dim AgronicaDAL As New ws_Canopy_Anagrafica_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.ws_Canopy_Anagrafica.LeggiCanopy()"
        Try

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dim obj As New List(Of ws_Canopy_Anagrafica_Dto)

            For Each riga As DataRow In dt.Rows
                If riga("BlockID") = -1 Then

                    Dim d = New ws_Canopy_Anagrafica_Dto

                    d.BlockID = 0
                    d.Kpin = riga("Kpin")
                    d.BlockName = riga("BlockName")
                    d.DescrizioneSalvataggio = "Date: " & riga("Data_Modifica")
                    d.MessaggiSincroCanopy = riga("Message")

                    d.Validita_Inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
                    d.Validita_Fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE

                    d.StatoSincronizzazione = riga("statoSincronizzazione")

                    d.Inviato = 0
                    d.DataInvio = Nothing
                    d.Data_Creazione = Now
                    d.Data_Modifica = Now
                    d.Username_Creazione = objParametri.UtenteUsername
                    d.Username_Modifica = objParametri.UtenteUsername

                    obj.Add(d)
                End If

            Next

            Return obj

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] :    " & MessaggioErrore)

        End Try

    End Function

    ''' <summary>
    ''' Salva i dati nel DB ws_Canopy_Anagrafica
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <param name="righeInserite"></param>
    ''' <param name="righeModificate"></param>
    ''' <returns></returns>
    Public Function Canopy_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                        righeInserite As List(Of ws_Canopy_Anagrafica_Dto),
                                        righeModificate As List(Of ws_Canopy_Anagrafica_Dto)) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreContabBIZ.ws_Canopy_Anagrafica.Canopy_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String
        Dim efConnString As String
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Using scope As New TransactionScope()
            Try
                ' ------------------------------------- Salva i dati nel database -------------------------------------
                Dim context As New Gias_DeveloperServer_Entities(efConnString)
                Dim AgronicaDAL As New AgronicaCoreContabDAL.ws_Canopy_Anagrafica_W

                If String.IsNullOrEmpty(Errori) Then
                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
                        AgronicaDAL.Inserisci(righeInserite, context, objParametri)
                    End If

                    If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
                        AgronicaDAL.Modifica(righeModificate, context, objParametri)
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
            End Try
        End Using
    End Function

End Class
