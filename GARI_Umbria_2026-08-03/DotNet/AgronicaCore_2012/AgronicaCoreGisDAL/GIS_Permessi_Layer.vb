Imports AgronicaCoreDataProvider

Public Class GIS_Permessi_Layer_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiPermessiDaUtenteOGruppo(ByVal gruppi_appartenenza As List(Of Int32),
                                                 ByVal LayerElementiGrafici_Cod As Int32,
                                                 ByVal Utente As String,
                                                 ByRef objParametri_Server As AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Permessi_Layer_R.LeggiPermessiDaUtenteOGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" , Flag_Inserimento ")
            StrSQL.AppendLine(" , Flag_Modifica ")
            StrSQL.AppendLine(" , Flag_Cancellazione ")
            StrSQL.AppendLine(" , Flag_Informazioni ")
            StrSQL.AppendLine(" , Flag_Amministrazione ")
            StrSQL.AppendLine(" FROM GIS_LayerElementiGraficiXUtente ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Utente = '{0}' ", Agro_SQL_SaveText(Utente)))

            If LayerElementiGrafici_Cod > 0 Then
                StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            End If

            If gruppi_appartenenza IsNot Nothing AndAlso gruppi_appartenenza.Count > 0 Then

                StrSQL.AppendLine(" UNION ")

                StrSQL.AppendLine(" SELECT LayerElementiGrafici_Cod ")
                StrSQL.AppendLine(" , Flag_Inserimento ")
                StrSQL.AppendLine(" , Flag_Modifica ")
                StrSQL.AppendLine(" , Flag_Cancellazione ")
                StrSQL.AppendLine(" , Flag_Informazioni ")
                StrSQL.AppendLine(" , Flag_Amministrazione ")
                StrSQL.AppendLine(" FROM GIS_LayerElementiGraficiXGruppiUtente ")

                StrSQL.AppendLine(" WHERE ")

                StrSQL.AppendLine(String.Format(" Gruppi_Utente_cod IN ({0}) ", String.Join(",", gruppi_appartenenza)))

                If LayerElementiGrafici_Cod > 0 Then
                    StrSQL.AppendLine(String.Format(" AND LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
                End If

            End If

            StrSQL.AppendLine(" ORDER BY LayerElementiGrafici_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
Public Class GIS_Permessi_Layer_W

End Class
