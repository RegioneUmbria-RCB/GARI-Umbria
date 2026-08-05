Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GSB_Log_Esecuzioni
    Public ID As Integer
    Public PIVA_SuperUser As String
    Public Tipo_Sincro As Integer
    Public DataOraUltimaEsecuz As Date
    Public PIVA As String
    Public Note As String

    Public Sub New(ByVal dr As DataRow)
        ID = dr.Item("ID")
        PIVA_SuperUser = dr.Item("PIVA_SuperUser")
        Tipo_Sincro = dr.Item("Tipo_Sincro")
        DataOraUltimaEsecuz = dr.Item("DataOraUltimaEsecuz")
        PIVA = dr.Item("PIVA")
        Note = dr.Item("Note")
    End Sub
End Class

Public Class GSB_Log_Esecuzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSingolo(ByVal PIVA_SuperUser As String,
                                 ByVal tipoSincro As String,
                                 ByRef objParametriServer As AgronicaCoreParametri
                                 ) As GSB_Log_Esecuzioni

        Const nomeRoutine = "AgronicaCoreVarieDAL.GSB_Log_Esecuzioni.LeggiSingolo()"

        Dim messaggioErrore As String = ""
        Dim objLogEsecuzioni As GSB_Log_Esecuzioni

        Try

            Dim dt As DataTable = Leggi(PIVA_SuperUser,
                                        tipoSincro,
                                        objParametriServer)
            If dt.Rows.Count > 0 Then
                objLogEsecuzioni = New GSB_Log_Esecuzioni(dt.Rows(0))
            Else
                objLogEsecuzioni = Nothing
            End If

            Return objLogEsecuzioni

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            Return Nothing

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return Nothing

    End Function

    Public Function Leggi(ByVal PIVA_SuperUser As String,
                          ByVal tipoSincro As String,
                          ByRef objParametriServer As AgronicaCoreParametri,
                          Optional orderByLast As Boolean = True
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.GSB_Log_Esecuzioni.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  GSB_Log_Esecuzioni ")
            strSql.AppendLine(" WHERE 1=1  ")

            If PIVA_SuperUser <> "" Then
                strSql.AppendLine(" AND Piva_Superuser ='" & Agro_SQL_SaveText(PIVA_SuperUser) & "' ")
            End If

            If tipoSincro <> 0 Then
                strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            End If

            If orderByLast Then
                strSql.AppendLine(" ORDER BY DataOraUltimaEsecuz desc ")
            Else
                strSql.AppendLine(" ORDER BY DataOraUltimaEsecuz ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            dt = Nothing

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function
End Class

Public Class GSB_Log_Esecuzioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviOAggiorna(ByVal PIVA_SuperUser As String,
                           ByVal tipoSincro As String,
                           ByRef objParametriServer As AgronicaCoreParametri,
                           Optional dataOraEsecuz As Date = Nothing,
                           Optional PIVA As String = Nothing,
                           Optional Note As String = Nothing
                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.GSB_Log_Esecuzioni.Scrivi"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If IsNothing(PIVA_SuperUser) Or PIVA_SuperUser = "" Then
            messaggioErrore = "valore di PIVA_SuperUser non fornito"
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            Return False
        End If

        If IsNothing(tipoSincro) Or tipoSincro = "" Then
            messaggioErrore = "valore di Tipo_Sincro non fornito"
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            Return False
        End If

        If IsNothing(dataOraEsecuz) Or dataOraEsecuz = Date.MinValue Then
            dataOraEsecuz = DateAndTime.Now
        End If

        Try
            Dim objLogR As New GSB_Log_Esecuzioni_R
            Dim objLog As GSB_Log_Esecuzioni = objLogR.LeggiSingolo(PIVA_SuperUser, tipoSincro, objParametriServer)

            If IsNothing(objLog) Then

                strSql.AppendLine(" INSERT INTO GSB_Log_Esecuzioni (Piva_SuperUser, Tipo_Sincro, DataOraUltimaEsecuz, PIVA, Note) ")
                strSql.AppendLine(" VALUES( ")
                strSql.AppendLine("     '" & Agro_SQL_SaveText(PIVA_SuperUser) & "', ")
                strSql.AppendLine("     " & Agro_SQL_SaveNum(tipoSincro) & ", ")
                strSql.AppendLine("     " & Agro_SQL_SaveDateTime(dataOraEsecuz) & ", ")
                strSql.AppendLine("     '" & Agro_SQL_SaveText(PIVA) & "', ")
                strSql.AppendLine("     '" & Agro_SQL_SaveText(Note) & "' ")
                strSql.AppendLine(" ) ")

            Else

                strSql.AppendLine(" UPDATE GSB_Log_Esecuzioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("     DataOraUltimaEsecuz = " & Agro_SQL_SaveDateTime(dataOraEsecuz) & " ")

                If Not IsNothing(PIVA) Then
                    strSql.AppendLine("     PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Not IsNothing(Note) Then
                    strSql.AppendLine("     Note = '" & Agro_SQL_SaveText(Note) & "' ")
                End If

                strSql.AppendLine(" WHERE PIVA_SuperUser = '" & Agro_SQL_SaveText(PIVA_SuperUser) & "' ")
                strSql.AppendLine(" AND Tipo_Sincro = " & Agro_SQL_SaveNum(tipoSincro) & " ")

            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

            Return False

        End Try

        Return xRisp

    End Function

End Class
