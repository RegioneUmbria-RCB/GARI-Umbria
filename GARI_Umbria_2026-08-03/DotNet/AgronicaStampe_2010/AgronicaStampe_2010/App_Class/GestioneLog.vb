Public Class GestioneLogStampe

    '################################################################################
    Public Sub Gestione_LogErrori_Stampe(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                         ByVal Sottocartella As String, _
                                         ByVal NomeFile_ConEstensione As String, _
                                         ByVal IdentUtente As String, _
                                         ByVal NomeRoutine As String, _
                                         ByVal Log_Errori As String)

        Dim Path_Errore As String

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Sottocartella = Replace(Sottocartella, "\", "-")
        Sottocartella = Replace(Sottocartella, "/", "-")

        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "\", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "/", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, """", "")

        If objParametri_Server.LogDirectory <> "" Then
            If Not objParametri_Server.LogDirectory.EndsWith("\") Then
                objParametri_Server.LogDirectory &= "\"
            End If
            Path_Errore = objParametri_Server.LogDirectory & Sottocartella
        Else
            If System.IO.Directory.Exists("C:\GIASLAN\Log") = True Then
                Path_Errore = "C:\GIASLAN\Log\" & Sottocartella
            ElseIf System.IO.Directory.Exists("C:\Agronica_LOG") = True Then
                Path_Errore = "C:\Agronica_LOG\" & Sottocartella
            Else
                Path_Errore = "C:\" & Sottocartella
            End If
        End If

        If Not Path_Errore.EndsWith("\") Then
            Path_Errore &= "\"
        End If

        If Path_Errore.Length > 259 Then
            Throw New Exception("Superata la lunghezza tra path e nome del file: " & CStr(Path_Errore.Length) & "caratteri (max 259 caratteri).")
        End If

        Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
            .LogDescrizioneUtente = IdentUtente,
            .LogDirectory = Path_Errore,
            .LogFileName = NomeFile_ConEstensione
        }

        'objLog.Scrivi_LOG(Path_Errore, NomeFile_ConEstensione, IdentUtente, NomeRoutine, Log_Errori)
        objLog.Scrivi_LOG(objParametri_Server, NomeRoutine, Log_Errori, CustomLOGParams:=CustomLOGParams)


    End Sub



End Class
