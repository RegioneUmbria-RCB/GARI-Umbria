Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider

Public Class Messaggi_da_Agronica_R
    Inherits DataProvider

    'Public Function Leggi(ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreParametri) As DataTable
    '    Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Messaggi_da_Agronica_R.Leggi()"

    '    Dim dt As DataTable
    '    Dim strSql As New StringBuilder
    '    Try
    '        strSql.AppendLine("SELECT *")
    '        strSql.AppendLine("FROM Messaggi_da_Agronica")
    '        strSql.AppendLine("WHERE 1=1")

    '        If xFiltroAggiuntivo.Length > 0 
    '            strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
    '        Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
    '    End Try

    '    Return dt
    'End Function


    Public Function LeggiMessaggiAttivi(ByRef objParametri As AgronicaCoreParametri) As List(Of String)
        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Messaggi_da_Agronica_R.LeggiMessaggiAttivi()"
        Dim messaggiAttivi As New List(Of String)

        Try
            Dim dt As DataTable
            Dim strSql As New StringBuilder

            If objParametri.Lingua_Cod = 1 Then
                'Se la lingua è italiano leggo solo dalla tabella originaria...
                strSql.AppendLine("SELECT *")
                strSql.AppendLine("FROM Messaggi_da_Agronica")
                strSql.AppendLine("WHERE Messaggi_da_Agronica.Validita_Fine > GETDATE()")
            Else
                '...altrimenti vado in left join con la tabella XLingue
                strSql.AppendLine("SELECT ISNULL(Messaggi_da_Agronica_XLingue.Messaggio, Messaggi_da_Agronica.Messaggio) AS Messaggio")
                strSql.AppendLine(", Messaggi_da_Agronica.Validita_Inizio, Messaggi_da_Agronica.Validita_Fine")
                strSql.AppendLine(", Messaggi_da_Agronica_XLingue.Lingua_Cod")
                strSql.AppendLine("FROM Messaggi_da_Agronica")
                strSql.AppendLine("LEFT JOIN Messaggi_da_Agronica_XLingue")
                strSql.AppendLine("ON Messaggi_da_Agronica.ID = Messaggi_da_Agronica_XLingue.ID_Messaggio")
                strSql.AppendLine("AND Messaggi_da_Agronica_XLingue.Lingua_Cod = " & objParametri.Lingua_Cod)
                strSql.AppendLine("WHERE Messaggi_da_Agronica.Validita_Fine > GETDATE()")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                For Each riga As DataRow In dt.Rows
                    Dim dataInizio As Date = riga("Validita_Inizio")
                    Dim dataFine As Date = riga("Validita_Fine")

                    Dim messaggioAgronica As String = ""

                    Dim regexPlaceholder As New Regex("{\d{1,2}?}", RegexOptions.None, TimeSpan.FromSeconds(3))

                    Dim messaggioDB As String = riga("Messaggio")
                    Select Case regexPlaceholder.Matches(messaggioDB).Count
                        Case 0
                            messaggioAgronica = messaggioDB

                        Case 1
                            messaggioAgronica = String.Format(
                                messaggioDB,
                                dataInizio.ToString("D", Threading.Thread.CurrentThread.CurrentUICulture)
                            )

                        Case 2
                            messaggioAgronica = String.Format(
                                messaggioDB,
                                dataInizio.ToString("M", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataFine.ToString("M", Threading.Thread.CurrentThread.CurrentUICulture)
                            )

                        Case 3
                            messaggioAgronica = String.Format(
                                messaggioDB,
                                dataInizio.ToString("t", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataFine.ToString("t", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataFine.ToString("M", Threading.Thread.CurrentThread.CurrentUICulture)
                            )

                        Case 4
                            messaggioAgronica = String.Format(
                                messaggioDB,
                                dataInizio.ToString("t", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataInizio.ToString("M", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataFine.ToString("t", Threading.Thread.CurrentThread.CurrentUICulture),
                                dataFine.ToString("M", Threading.Thread.CurrentThread.CurrentUICulture)
                            )
                    End Select

                    
                    messaggiAttivi.Add(messaggioAgronica)
                Next
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return messaggiAttivi
    End Function

End Class
