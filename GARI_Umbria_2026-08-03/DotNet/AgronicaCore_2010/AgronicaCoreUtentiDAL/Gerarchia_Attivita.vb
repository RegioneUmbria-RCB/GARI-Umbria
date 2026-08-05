Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Gerarchia_Attivita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Attivita_Padre As Integer, _
                          ByVal Id_Attivita_Figlio As Integer, _
                          ByVal Id_Servizio As Integer, _
                          ByVal Livello As Integer, _
                          ByVal Foglia As Integer, _
                          ByVal Ordine As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Gerarchia_Attivita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Gerarchia_Attivita.*, TB_Padre.Attivita_Des AS Attivita_Des_Padre, TB_Figlio.Attivita_Des AS Attivita_Des_Figlio ")
            StrSQL.Append(" FROM    Gerarchia_Attivita LEFT OUTER JOIN ")
            StrSQL.Append(" TB_Attivita AS TB_Padre  ON Gerarchia_Attivita.Id_Attivita_Padre = TB_Padre.Id_Attivita INNER JOIN ")
            StrSQL.Append(" TB_Attivita AS TB_Figlio ON Gerarchia_Attivita.Id_Attivita_Figlio = TB_Figlio.Id_Attivita ")
            StrSQL.Append(" WHERE 1=1 ")

            If Id_Attivita_Padre <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.Id_Attivita_Padre = " & Agro_SQL_SaveNum(Id_Attivita_Padre) & " ")
            End If
            If Id_Attivita_Figlio <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.Id_Attivita_Figlio = " & Agro_SQL_SaveNum(Id_Attivita_Figlio) & " ")
            End If

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If
            If Livello <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.Livello = " & Agro_SQL_SaveNum(Livello) & " ")
            End If
            If foglia <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.foglia = " & Agro_SQL_SaveNum(Foglia) & " ")
            End If
            If Ordine <> 0 Then
                StrSQL.Append(" AND Gerarchia_Attivita.Ordine = " & Agro_SQL_SaveNum(Ordine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Gerarchia_Attivita.Id_Servizio, Gerarchia_Attivita.Livello, Gerarchia_Attivita.Ordine" & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
End Class
