Public Class DatiReteAcqua_Parametri_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_ParametriGenerali(ByVal PivaSuperUser As String,
                                             ByVal settimana As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R.Leggi_Evapotraspirazione()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     Settimana as week, ")
            strSql.AppendLine("     RadiazioneExtraterrestre ")
            strSql.AppendLine(" FROM  [DatiReteAcqua_ParametriGenerali] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If settimana <> 0 Then
                strSql.AppendLine(" AND settimana = " & Agro_SQL_SaveNum(settimana) & " ")
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

    Public Function Leggi_CoeffXSpecie(ByVal PivaSuperUser As String,
                                      ByVal veg_cod As Integer,
                                      ByVal settimana As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_R.Leggi_CoeffXSpecie()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     Settimana as week, ")
            strSql.AppendLine("     a.Veg_Cod, ")
            strSql.AppendLine("     b.Veg_Des, ")
            strSql.AppendLine("     a.Kc, ")
            strSql.AppendLine("     a.DFc ")
            strSql.AppendLine(" FROM  [DatiReteAcqua_ParametriXSpecie] a inner join SpecieVegetali b ")
            strSql.AppendLine(" on (a.Veg_Cod=b.Veg_Cod) ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If veg_cod <> 0 Then
                strSql.AppendLine(" AND a.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
            End If

            If settimana <> 0 Then
                strSql.AppendLine(" AND Settimana = " & Agro_SQL_SaveNum(settimana) & " ")
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
Public Class DatiReteAcqua_Parametri_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_ParametriGenerali(ByVal PivaSuperUser As String,
                                             ByVal settimana As Integer,
                                             ByVal RadiazioneExtraterrestre As Decimal,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                             Optional ByVal username_creazione As String = "",
                                             Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W.Scrivi_ParametriGenerali()"


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
            StrSQL.AppendLine("INSERT INTO DatiReteAcqua_ParametriGenerali ( PivaSuperUser, Settimana, RadiazioneExtraterrestre, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(settimana) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(RadiazioneExtraterrestre) & "  ")
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

    Public Function Cancella_ParametriGenerali(ByVal PivaSuperUser As String,
                                                ByVal settimana As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W.Cancella_ParametriGenerali()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from DatiReteAcqua_ParametriGenerali ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If settimana <> 0 Then
                StrSQL.AppendLine(" AND settimana = " & Agro_SQL_SaveNum(settimana) & " ")
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


    Public Function Scrivi_CoeffXSpecie(ByVal PivaSuperUser As String,
                                        ByVal veg_cod As Integer,
                                        ByVal settimana As Integer,
                                        ByVal Kc As Decimal,                'coeff moltiplicativo evapotraspirato per specie
                                        ByVal DFc As Decimal,               'coeff moltiplicativo deficit irrigation per specie
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                             Optional ByVal username_creazione As String = "",
                                             Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W.Scrivi_ParametriGenerali()"


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
            StrSQL.AppendLine("INSERT INTO DatiReteAcqua_ParametriXSpecie ( PivaSuperUser, Veg_Cod, Settimana, Kc, DFc, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(veg_cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(settimana) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Kc) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(DFc) & "  ")
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

    Public Function Cancella_CoeffXSpecie(ByVal PivaSuperUser As String,
                                          ByVal settimana As Integer,
                                          ByVal veg_cod As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDatiReteAcquaDAL.DatiReteAcqua_Parametri_W.Cancella_ParametriGenerali()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from DatiReteAcqua_ParametriXSpecie ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If veg_cod <> 0 Then
                StrSQL.AppendLine(" AND Veg_Cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
            End If

            If settimana <> 0 Then
                StrSQL.AppendLine(" AND settimana = " & Agro_SQL_SaveNum(settimana) & " ")
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
