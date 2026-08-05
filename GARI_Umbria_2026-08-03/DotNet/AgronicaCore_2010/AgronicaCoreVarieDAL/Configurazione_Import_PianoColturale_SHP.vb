Imports System.Text
Imports AgronicaCoreDataProvider

Public Class Configurazione_Import_PianoColturale_SHP_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal idConfig As Integer,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Import_PianoColturale_SHP_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Configurazione_Import_PianoColturale_SHP ")
            strSql.AppendLine(" WHERE 1=1  ")

            If idConfig <> 0 Then
                strSql.AppendLine(" AND id =" & Agro_SQL_SaveNum(idConfig) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function





End Class

Public Class Configurazione_Import_PianoColturale_SHP_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal idConfig As Integer,
                           ByVal descr As String,
                           ByVal params As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Import_PianoColturale_SHP_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Configurazione_Import_PianoColturale_SHP ")
            strSql.AppendLine(" (id, descr, impresa_pars, centro_pars, params) ")
            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(idConfig) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(descr) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(params) & "' ")
            strSql.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna(ByVal idConfig As Integer,
                           ByVal descr As String,
                           ByVal params As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Import_PianoColturale_SHP_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Configurazione_Import_PianoColturale_SHP ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("    params='" & Agro_SQL_SaveText(params) & "' ")

            strSql.AppendLine(" where 1=1")
            If idConfig <> 0 Then
                strSql.AppendLine(" AND id=" & Agro_SQL_SaveNum(idConfig) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
