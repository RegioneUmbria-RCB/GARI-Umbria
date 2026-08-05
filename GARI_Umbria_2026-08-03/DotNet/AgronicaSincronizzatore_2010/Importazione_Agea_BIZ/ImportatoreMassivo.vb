Imports AgronicaCoreDataProvider

Public Class ImportatoreMassivo

    Public Function Importa_DatiMassivo(ByVal Flag_ImportaAnagrafica As Boolean,
                                           ByVal Flag_ImportaPianoColturale As Boolean,
                                           ByVal StringaConnessione As String,
                                           ByVal Utente_Username As String,
                                           ByVal Utente_Password As String,
                                           ByVal ProgressivoGIAS As Integer,
                                           ByVal CodiceChiaveCliente As Integer,
                                           ByVal LinkWSImportaGIAS As String,
                                           ByVal LogDirectory As String,
                                           ByVal LogFileName As String,
                                           ByRef Messaggio As String,
                                           ByRef LogCodificheMancantiSpecie As String,
                                           ByRef LogCodificheMancantiVarieta As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                           ByRef Piva_Padre As String) As Boolean

        Dim Dt_Imprese As New DataTable
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        objLog.Scrivi_LOG(objParametri_Server,
                   NomeRoutine,
                   "Inizio importazione",
                   CustomLOGParams:=customLOGParams)

        'Leggo i File
        Crea_Dt_Imprese(StringaConnessione, Messaggio, Dt_Imprese)

        objLog.Scrivi_LOG(objParametri_Server,
                         NomeRoutine,
                         Messaggio,
                         CustomLOGParams:=customLOGParams)

        If Dt_Imprese.Rows.Count = 0 Then
            objLog.Scrivi_LOG(objParametri_Server,
                              NomeRoutine,
                              "Importazione arrestata a causa di errore.",
                              CustomLOGParams:=customLOGParams)
            Messaggio = "Impossibile importare i dati: " & Messaggio
            Return False
        Else
            objLog.Scrivi_LOG(objParametri_Server,
                              NomeRoutine,
                              "Inizio lettura dei dati.",
                              CustomLOGParams:=customLOGParams)
        End If

        Dim fascicolo_agea As New Import_Agea

        If Dt_Imprese.Rows.Count > 0 Then

            For Each rowCuaa In Dt_Imprese.Rows

                Dim Cuaa As String = rowCuaa(0).ToString.Trim
                Cuaa = Cuaa.Replace("'", "")
                Dim messImportazione As String = ""

                Try



                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             "Importo Impresa:" & Cuaa,
                             CustomLOGParams:=customLOGParams)

                    fascicolo_agea.importa_CUAA(Cuaa, messImportazione, True, True, True, True, True, ProgressivoGIAS, Utente_Password, objParametri_Server, objParametri_Utenti, Piva_Padre)

                    Messaggio &= "<br/> " & messImportazione

                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             messImportazione,
                             CustomLOGParams:=customLOGParams)

                    Messaggio &= "<br/> " & messImportazione

                Catch ex As Exception

                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             "Errore su :" & Cuaa & " - " & ex.Message,
                             CustomLOGParams:=customLOGParams)

                End Try

            Next

        End If

        objLog.Scrivi_LOG(objParametri_Server,
                          NomeRoutine,
                          "Importazione massiva terminata",
                          CustomLOGParams:=customLOGParams)

    End Function

    Private Sub Crea_Dt_Imprese(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)
        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try

    End Sub

End Class
