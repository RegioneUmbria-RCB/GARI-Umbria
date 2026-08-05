Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreScadenziario
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMASetup_W
Imports AgronicaCoreUtentiDAL.ws_ppt
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class UMA_Macrousi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Macrousi_LeggiTabella(inizioValidita As String, fineValidita As String, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMA_Macrousi_R
        Dim alertTipologie As New Alert_Tipologia_R
        Dim MessaggioErrore As String
        Dim filtroAggiutivo As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMA_Macrousi.Macrousi_LeggiTabella()"
        Try

            AgronicaDAL.ComponiFiltroAggiuntivoValidita(filtroAggiutivo, inizioValidita, fineValidita)

            Dim uf As DataTable = AgronicaDAL.Leggi("", filtroAggiutivo, "", objParametri_Server)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("Regione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

            For Each elem As DataRow In uf.Rows
                Dim d = Dt.NewRow
                d("Regione_Cod") = elem("Regione_Cod")
                d("Macrouso_UMA_Cod") = elem("Macrouso_UMA_Cod")
                d("Macrouso_UMA_Des") = elem("Macrouso_UMA_Des")
                d("Inviato") = elem("Inviato")
                d("DataInvio") = elem("DataInvio")
                d("Data_Creazione") = elem("Data_Creazione")
                d("Data_Modifica") = elem("Data_Modifica")
                d("Username_Creazione") = elem("Username_Creazione")
                d("Username_Modifica") = elem("Username_Modifica")
                d("Validita_Inizio") = elem("Validita_Inizio")
                d("Validita_Fine") = elem("Validita_Fine")

                Dt.Rows.Add(d)
            Next

            Return Dt

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    Public Function AssociazioniMacrousi_LeggiTabella(inizioValidita As String, fineValidita As String, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMA_Macrousi_R
        Dim alertTipologie As New Alert_Tipologia_R
        Dim MessaggioErrore As String
        Dim filtroAggiutivo As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMA_Macrousi.AssociazioniMacrousi_LeggiTabella()"
        Try

            AgronicaDAL.ComponiFiltroAggiuntivoValidita_AssociazioniMacrousi(filtroAggiutivo, inizioValidita, fineValidita)

            Dim uf As DataTable = AgronicaDAL.Leggi_AssociazioniMacrousi("", filtroAggiutivo, "", objParametri_Server)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("Chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Uso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Uso_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Occupazione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Occupazione_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Destinazione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Destinazione_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qualita_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qualita_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

            For Each elem As DataRow In uf.Rows
                Dim d = Dt.NewRow

                d("Chiave") = String.Join("-", elem("Cul_Cod_Agea"), elem("Occupazione_Cod"), elem("Destinazione_Cod"), elem("Uso_Cod"), elem("Qualita_Cod"))
                d("Cul_Cod_Agea") = elem("Cul_Cod_Agea")
                d("Cul_Des_Agea") = elem("Cul_Des_Agea")
                d("Uso_Cod") = elem("Uso_Cod")
                d("Uso_Des") = elem("Uso_Des")
                d("Macrouso_Cod") = elem("Macrouso_Cod")
                d("Macrouso_Des") = elem("Macrouso_Des")
                d("Occupazione_Cod") = elem("Occupazione_Cod")
                d("Occupazione_Des") = elem("Occupazione_Des")
                d("Destinazione_Cod") = elem("Destinazione_Cod")
                d("Destinazione_Des") = elem("Destinazione_Des")
                d("Qualita_Cod") = elem("Qualita_Cod")
                d("Qualita_Des") = elem("Qualita_Des")
                d("Macrouso_UMA_Cod") = elem("Macrouso_UMA_Cod")
                d("Macrouso_UMA_Des") = elem("Macrouso_UMA_Des")
                d("Inviato") = elem("Inviato")
                d("DataInvio") = elem("DataInvio")
                d("Data_Creazione") = elem("Data_Creazione")
                d("Data_Modifica") = elem("Data_Modifica")
                d("Username_Creazione") = elem("Username_Creazione")
                d("Username_Modifica") = elem("Username_Modifica")
                d("Validita_Inizio") = elem("Validita_Inizio")
                d("Validita_Fine") = elem("Validita_Fine")

                Dt.Rows.Add(d)
            Next

            Return Dt

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    Public Function Macrousi_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMA_Macrousi.Macrousi_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String
        Dim efConnString As String
        Dim righeCancellate As List(Of UMA_Macrousi_Dto)
        Dim righeModificate As List(Of UMA_Macrousi_Dto)
        Dim righeInserite As List(Of UMA_Macrousi_Dto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMA_Macrousi_Dto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMA_Macrousi_Dto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMA_Macrousi_Dto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Using scope As New TransactionScope()
            Try
                ' ------------------------------------- Salva i dati nel database -------------------------------------
                Dim context As New Gias_DeveloperServer_Entities(efConnString)
                Dim AgronicaDAL As New AgronicaCoreUmaDal.UMA_Macrousi_W

                '------------------------- Rimuovi le righe cancellate -------------------------
                If Not righeCancellate Is Nothing AndAlso righeCancellate.Count > 0 Then
                    AgronicaDAL.Rimuovi(righeCancellate, objParametri)
                End If

                '------------------------------ Verifica validità righe input ------------------------------    
                Errori = String.Empty
                'If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                '    ControlliValidita_UMASetup(righeInserite, righeModificate, objParametri, efConnString, Errori)
                'End If

                If (Not String.IsNullOrEmpty(Errori)) Then
                    risposta.RispostaOK = False
                    risposta.Errore = Errori
                    Return risposta
                End If

                If String.IsNullOrEmpty(Errori) Then

                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
                        AgronicaDAL.Aggiorna(righeModificate, objParametri, Errori)
                    End If

                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
                        Errori = AgronicaDAL.AggiungiNuovi(righeInserite, objParametri, Errori)
                    End If

                End If
                scope.Complete()
                scope.Dispose()

                If Errori.Length > 0 Then

                    risposta.RispostaOK = False
                    risposta.RispostaStringa = Errori
                    risposta.Errore = Errori

                Else

                    risposta.RispostaOK = True
                    risposta.RispostaStringa = "L'operazione è stata completata con successo."

                End If

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
