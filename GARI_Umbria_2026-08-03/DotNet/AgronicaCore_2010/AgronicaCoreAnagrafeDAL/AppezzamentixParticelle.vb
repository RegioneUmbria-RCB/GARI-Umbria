Imports AgronicaCoreDataProvider

Public Class AppezzamentixParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function UpdateColonna_Massivo_DaChiaviAppezzamento(listChiaviAppezzamento As List(Of (String, Integer, Integer)),
                                                               nomeColonna As String,
                                                               valore As String,
                                                               tipoDato As String,
                                                               timeStamp As Date,
                                                               ByVal objParametri_Server As AgronicaCoreParametri
                                                               ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzamentixParticelle_W.UpdateColonna_Massivo_DaChiaviAppezzamento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try

            If listChiaviAppezzamento.Count = 0 Then
                Throw New Exception("Parametro non corretto nella query (listChiaviAppezzamento obbligatorio)")
            End If

            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            TempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(listChiaviAppezzamento, nomeRoutine, objParametri_Server)

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE A ")
            StrSQL.AppendLine("SET ")

            StrSQL.Append($" {nomeColonna} = ")
            Select Case tipoDato
                Case "string"
                    StrSQL.Append($"'{Agro_SQL_SaveText(valore)}'")
                Case "date"
                    StrSQL.Append(Agro_SQL_SaveDate(valore))
                Case "number"
                    StrSQL.Append(Agro_SQL_SaveNum(valore))
            End Select

            StrSQL.AppendLine($" , Data_Modifica = {Agro_SQL_SaveDateTime(timeStamp)} ")

            StrSQL.AppendLine(" FROM AppezzamentiXParticelle A ")
            StrSQL.AppendLine(" JOIN #TempAppezzamento temp (NOLOCK) ON  ")
            StrSQL.AppendLine("     A.Piva = temp.Piva ")
            StrSQL.AppendLine(" AND A.Sa_Cod = temp.Sa_Cod ")
            StrSQL.AppendLine(" AND A.Appezza = temp.Appezza ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(nomeRoutine, objParametri_Server)
            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As Exception
            ' Rollback
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return xRisp

    End Function

End Class
Public Class AppezzamentixParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider



End Class
