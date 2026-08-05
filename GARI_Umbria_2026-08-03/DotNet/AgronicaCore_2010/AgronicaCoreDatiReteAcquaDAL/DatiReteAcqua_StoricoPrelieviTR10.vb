Public Class DatiReteAcqua_StoricoPrelieviTR10_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PivaSuperUser As String,
                          ByVal veg_cod As Integer,
                          ByVal settimana As Integer,
                          ByVal anno As Integer,
                          ByVal gruppoconsegna As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT a.Anno,a.GruppoConsegna,settimana,a.veg_cod,b.veg_des,FullReturn,DeficitReturn ")
            strSql.AppendLine(" FROM  [DatiReteAcqua_StoricoPrelieviTR10] a inner join SpecieVegetali b ")
            strSql.AppendLine(" on (a.veg_cod=b.Veg_Cod) ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If veg_cod <> 0 Then
                strSql.AppendLine(" AND a.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
            End If

            If settimana <> 0 Then
                strSql.AppendLine(" AND settimana = " & Agro_SQL_SaveNum(settimana) & " ")
            End If

            If anno <> 0 Then
                strSql.AppendLine(" AND anno = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If gruppoconsegna <> 0 Then
                strSql.AppendLine(" AND GruppoConsegna = " & Agro_SQL_SaveNum(gruppoconsegna) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

Public Class DatiReteAcqua_StoricoPrelieviTR10_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(ByVal PivaSuperUser As String,
                                     ByVal veg_cod As Integer,
                                     ByVal settimana As Integer,
                                     ByVal anno As Integer,
                                     ByVal gruppoconsegna As Integer,
                                     ByVal Full As Decimal,
                                     ByVal Deficit As Decimal,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                     Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                     Optional ByVal username_creazione As String = "",
                                     Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO DatiReteAcqua_StoricoPrelieviTR10( PivaSuperUser, Veg_Cod, ")
            StrSQL.AppendLine("                                               Settimana, Anno, ")
            StrSQL.AppendLine("                                               GruppoConsegna, ")
            StrSQL.AppendLine("                                               FullReturn, DeficitReturn, ")
            StrSQL.AppendLine("                                               Inviato, DataInvio, ")
            StrSQL.AppendLine("                                               Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                                               UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                                               Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(veg_cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(settimana) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(anno) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(gruppoconsegna) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Full) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Deficit) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal PivaSuperUser As String,
                             ByVal veg_cod As Integer,
                             ByVal settimana As Integer,
                             ByVal anno As Integer,
                             ByVal gruppoconsegna As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_StoricoPrelieviTR10_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from DatiReteAcqua_StoricoPrelieviTR10 ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If veg_cod <> 0 Then
                StrSQL.AppendLine(" AND veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
            End If

            If settimana <> 0 Then
                StrSQL.AppendLine(" AND settimana = " & Agro_SQL_SaveNum(settimana) & " ")
            End If

            If anno <> 0 Then
                StrSQL.AppendLine(" AND anno = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If gruppoconsegna <> 0 Then
                StrSQL.AppendLine(" AND gruppoconsegna = " & Agro_SQL_SaveNum(gruppoconsegna) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
End Class
