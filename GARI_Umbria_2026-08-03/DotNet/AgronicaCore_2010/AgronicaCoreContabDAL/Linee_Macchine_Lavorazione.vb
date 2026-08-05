Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Linee_Macchine_Lavorazione_R : Inherits DataProvider

    Public Function LeggiEF(piva As String,
                          codice As String,
                          descrizione As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                        ) As List(Of Linee_Macchine_Lavorazione)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Macchine_Lavorazione_R.LeggiEF()"
        Dim bCloseContext As Boolean = False
        Dim listaMacchine As List(Of Linee_Macchine_Lavorazione) = Nothing
        Dim messaggioErrore As String = String.Empty

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Try
            listaMacchine = (From lml In GiasContext.Linee_Macchine_Lavorazione.AsNoTracking
                             Where lml.piva.Equals(piva) _
                             AndAlso (codice = String.Empty OrElse lml.codice.Equals(codice)) _
                             AndAlso (descrizione = String.Empty OrElse lml.descrizione.Contains(descrizione))).ToList

        Catch ex As Exception
            messaggioErrore = ex.Message
            MyBase.Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
        Finally
            If bCloseContext Then
                GiasContext.Dispose()
            End If
        End Try

        Return listaMacchine

    End Function

    Public Function Leggi(piva As String,
                          codice As String,
                          descrizione As String,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Linee_Macchine_Lavorazione_R.Leggi()"
        Dim dt As DataTable = Nothing
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String = String.Empty


        Try

            strSql.AppendLine(" Select * from Linee_Macchine_Lavorazione ")
            strSql.AppendLine(" where Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Not String.IsNullOrEmpty(codice) Then
                strSql.AppendLine(" and Codice = '" & Agro_SQL_SaveText(codice) & "' ")
            End If

            If Not String.IsNullOrEmpty(descrizione) Then
                strSql.AppendLine(" and descrizione = '" & Agro_SQL_SaveText(descrizione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY  Descrizione ")
            End If

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class Linee_Macchine_Lavorazione_W

End Class



