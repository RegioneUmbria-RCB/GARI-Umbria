
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Agronica_Log_Invio_Analisi_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class
Public Class Agronica_Log_Invio_Analisi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                           ID_Log_Invio As Integer,
                           Stato As Integer,
                           Analisi_Testata_Cod As Integer,
                           Chiave_Esterna As String,
                           objParametri As AgronicaCoreParametri,
                            Optional Piva As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Analisi_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder

        Dim xRisp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Agronica_Log_Invio_Analisi ( ")
            StrSQL.AppendLine("           Tipo_Esportazione")
            StrSQL.AppendLine("         , ID_Log_Invio")
            StrSQL.AppendLine("         , Stato")

            StrSQL.AppendLine("         , Analisi_Testata_Cod")
            StrSQL.AppendLine("         , Chiave_Esterna")
            StrSQL.AppendLine("         , Piva")

            StrSQL.AppendLine("         , inviato")
            StrSQL.AppendLine("         , datainvio")

            StrSQL.AppendLine("         , Data_Creazione")
            StrSQL.AppendLine("         , Data_Modifica")
            StrSQL.AppendLine("         , Username_Creazione")
            StrSQL.AppendLine("         , Username_Modifica")
            StrSQL.AppendLine("         , Validita_Inizio")
            StrSQL.AppendLine("         , Validita_Fine")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("            " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(ID_Log_Invio) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Stato) & " ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Chiave_Esterna) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.AppendLine("          , 0 ")
            StrSQL.AppendLine("          , NULL ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function
End Class
