Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Modalita_Applicazione
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Codice As Integer,
                            ByVal Codice_Personalizzato As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Modalita_Applicazione.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Modalita_Applicazione WITH(NOLOCK)")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Codice <> 0 Then
                StrSQL.AppendLine(" AND Codice = " & Agro_SQL_SaveNum(Codice))
            End If

            If Codice_Personalizzato <> 0 Then
                StrSQL.AppendLine(" AND Codice_Personalizzato = " & Agro_SQL_SaveNum(Codice_Personalizzato))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & xOrderBy)
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function Lettura_Scalare_Modalita_Applicazione(ByVal Lav_Cod As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Modalita_Applicazione.Lettura_Scalare_Modalita_Applicazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" with CTE_Modalita_Applicazione(Codice, Descrizione) AS (")
            StrSQL.AppendLine(" select Modalita_Applicazione.Codice,Modalita_Applicazione.Descrizione_Personalizzata AS Descrizione  from Modalita_Applicazione")
            StrSQL.AppendLine(" inner join Modalita_Applicazione_Operazioni")
            StrSQL.AppendLine(" on Modalita_Applicazione.Codice = Modalita_Applicazione_Operazioni.Codice")
            StrSQL.AppendLine("       where Modalita_Applicazione_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "")
            StrSQL.AppendLine(" )")

            StrSQL.AppendLine(" select * from CTE_Modalita_Applicazione")
            StrSQL.AppendLine(" UNION")
            StrSQL.AppendLine(" select Modalita_Applicazione_Globali_Operazioni.Codice,  AGEA_DES AS Descrizione  from Modalita_Applicazione_Globali")
            StrSQL.AppendLine(" inner join Modalita_Applicazione_Globali_Operazioni")
            StrSQL.AppendLine(" on Modalita_Applicazione_Globali.Codice = Modalita_Applicazione_Globali_Operazioni.Codice")
            StrSQL.AppendLine("       where Modalita_Applicazione_Globali_Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "")
            StrSQL.AppendLine(" and not exists (select 1 from CTE_Modalita_Applicazione)")
            StrSQL.AppendLine(" order by Descrizione ASC")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
