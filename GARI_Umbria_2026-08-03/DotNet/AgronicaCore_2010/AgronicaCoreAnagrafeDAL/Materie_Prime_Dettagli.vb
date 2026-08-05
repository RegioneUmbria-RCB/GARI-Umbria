Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Materie_Prime_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PIVA As String,
                          ByVal Mat_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Dettagli_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.Append(" SELECT Materie_Prime_Dettagli.* ")
                    strSql.Append(" FROM   Materie_Prime_Dettagli ")
                    strSql.Append(" WHERE  Materie_Prime_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND    Materie_Prime_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.Append(" AND    (Materie_Prime_Dettagli.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Materie_Prime_Dettagli.Piva_SuperUser = 'AAAAAAAAAAA')")

                    If PIVA <> "" Then
                        strSql.Append(" AND Materie_Prime_Dettagli.Piva =  '" & Agro_SQL_SaveText(PIVA) & "'  ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.Append(" AND Materie_Prime_Dettagli.Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Materie_Prime_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Materie_Prime_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Materie_Prime_Dettagli.Mat_Cod ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


            End Select

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


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Materie_Prime_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Mat_Cod As Long,
                           ByVal Extra_Smallint1 As Int16,
                           ByVal Extra_Smallint2 As Int16,
                           ByVal Extra_Smallint3 As Int16,
                           ByVal Extra_Smallint4 As Int16,
                           ByVal Extra_Smallint5 As Int16,
                           ByVal Extra_Smallint6 As Int16,
                           ByVal Extra_Int1 As Int32,
                           ByVal Extra_Int2 As Int32,
                           ByVal Extra_Int3 As Int32,
                           ByVal Extra_Int4 As Int32,
                           ByVal Extra_Int5 As Int32,
                           ByVal Extra_Int6 As Int32,
                           ByVal Extra_Dbl1 As Decimal,
                           ByVal Extra_Dbl2 As Decimal,
                           ByVal Extra_Dbl3 As Decimal,
                           ByVal Extra_Dbl4 As Decimal,
                           ByVal Extra_Dbl5 As Decimal,
                           ByVal Extra_Dbl6 As Decimal,
                           ByVal Extra_Str1 As String,
                           ByVal Extra_Str2 As String,
                           ByVal Extra_Str3 As String,
                           ByVal Extra_Str4 As String,
                           ByVal Extra_Str5 As String,
                           ByVal Extra_Str6 As String,
                           ByVal Extra_Str7 As String,
                           ByVal Extra_Str8 As String,
                           ByVal Extra_Str9 As String,
                           ByVal Extra_Date1 As Date,
                           ByVal Extra_Date2 As Date,
                           ByVal Extra_Date3 As Date,
                           ByVal Extra_Date4 As Date,
                           ByVal Extra_Date5 As Date,
                           ByVal Extra_Date6 As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Dettagli_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            strSql.Length = 0

            strSql.Append(" INSERT INTO Materie_Prime_Dettagli ")
            strSql.Append("         ( Piva_SuperUser,   Piva,             Mat_Cod,         ")
            strSql.Append("             Extra_Smallint1,  Extra_Smallint2,  Extra_Smallint3, ")
            strSql.Append("             Extra_Smallint4,  Extra_Smallint5,  Extra_Smallint6, ")
            strSql.Append("             Extra_Int1,       Extra_Int2,       Extra_Int3,      ")
            strSql.Append("             Extra_Int4,       Extra_Int5,       Extra_Int6,      ")
            strSql.Append("             Extra_Dbl1,       Extra_Dbl2,       Extra_Dbl3,      ")
            strSql.Append("             Extra_Dbl4,       Extra_Dbl5,       Extra_Dbl6,      ")
            strSql.Append("             Extra_Str1,       Extra_Str2,       Extra_Str3,      ")
            strSql.Append("             Extra_Str4,       Extra_Str5,       Extra_Str6,      ")
            strSql.Append("             Extra_Str7,       Extra_Str8,       Extra_Str9,      ")
            strSql.Append("             Extra_Date1,      Extra_Date2,      Extra_Date3,      ")
            strSql.Append("             Extra_Date4,      Extra_Date5,      Extra_Date6,      ")

            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mat_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint1))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint2))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint3))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint4))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint5))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Smallint6))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int1))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int2))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int3))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int4))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int5))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int6))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl1))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl2))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl3))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl4))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl5))
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Dbl6))

            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str1) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str2) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str3) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str4) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str5) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str6) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str7) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str8) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str9) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date1) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date2) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date3) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date4) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date5) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Extra_Date6) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append(") ")

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

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Mat_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Dettagli_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Materie_Prime_Dettagli ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Materie_Prime_Dettagli ")
                strSql.Append(" WHERE  1=1 ")

            End If


            strSql.Append(" AND  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Piva <> "" Then
                strSql.Append(" AND Piva =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    '============================================================================
    Public Function Modifica(ByVal Piva As String,
                             ByVal Mat_Cod As Int32,
                             ByVal Extra_Smallint1 As Int16,
                             ByVal Extra_Smallint2 As Int16,
                             ByVal Extra_Smallint3 As Int16,
                             ByVal Extra_Smallint4 As Int16,
                             ByVal Extra_Smallint5 As Int16,
                             ByVal Extra_Smallint6 As Int16,
                             ByVal Extra_Int1 As Int32,
                             ByVal Extra_Int2 As Int32,
                             ByVal Extra_Int3 As Int32,
                             ByVal Extra_Int4 As Int32,
                             ByVal Extra_Int5 As Int32,
                             ByVal Extra_Int6 As Int32,
                             ByVal Extra_Dbl1 As Decimal,
                             ByVal Extra_Dbl2 As Decimal,
                             ByVal Extra_Dbl3 As Decimal,
                             ByVal Extra_Dbl4 As Decimal,
                             ByVal Extra_Dbl5 As Decimal,
                             ByVal Extra_Dbl6 As Decimal,
                             ByVal Extra_Str1 As String,
                             ByVal Extra_Str2 As String,
                             ByVal Extra_Str3 As String,
                             ByVal Extra_Str4 As String,
                             ByVal Extra_Str5 As String,
                             ByVal Extra_Str6 As String,
                             ByVal Extra_Str7 As String,
                             ByVal Extra_Str8 As String,
                             ByVal Extra_Str9 As String,
                             ByVal Extra_Date1 As Date,
                             ByVal Extra_Date2 As Date,
                             ByVal Extra_Date3 As Date,
                             ByVal Extra_Date4 As Date,
                             ByVal Extra_Date5 As Date,
                             ByVal Extra_Date6 As Date,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        '------------------------------

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Materie_Prime_Dettagli SET ")
            strSql.Append("    Extra_Smallint1   = " & Agro_SQL_SaveNum(Extra_Smallint1) & "  ")
            strSql.Append("   ,Extra_Smallint2   = " & Agro_SQL_SaveNum(Extra_Smallint2) & "  ")
            strSql.Append("   ,Extra_Smallint3   = " & Agro_SQL_SaveNum(Extra_Smallint3) & "  ")
            strSql.Append("   ,Extra_Smallint4   = " & Agro_SQL_SaveNum(Extra_Smallint4) & "  ")
            strSql.Append("   ,Extra_Smallint5   = " & Agro_SQL_SaveNum(Extra_Smallint5) & "  ")
            strSql.Append("   ,Extra_Smallint6   = " & Agro_SQL_SaveNum(Extra_Smallint6) & "  ")
            strSql.Append("   ,Extra_Int1        = " & Agro_SQL_SaveNum(Extra_Int1) & "  ")
            strSql.Append("   ,Extra_Int2        = " & Agro_SQL_SaveNum(Extra_Int2) & "  ")
            strSql.Append("   ,Extra_Int3        = " & Agro_SQL_SaveNum(Extra_Int3) & "  ")
            strSql.Append("   ,Extra_Int4        = " & Agro_SQL_SaveNum(Extra_Int4) & "  ")
            strSql.Append("   ,Extra_Int5        = " & Agro_SQL_SaveNum(Extra_Int5) & "  ")
            strSql.Append("   ,Extra_Int6        = " & Agro_SQL_SaveNum(Extra_Int6) & "  ")
            strSql.Append("   ,Extra_Dbl1        = " & Agro_SQL_SaveNum(Extra_Dbl1) & "  ")
            strSql.Append("   ,Extra_Dbl2        = " & Agro_SQL_SaveNum(Extra_Dbl2) & "  ")
            strSql.Append("   ,Extra_Dbl3        = " & Agro_SQL_SaveNum(Extra_Dbl3) & "  ")
            strSql.Append("   ,Extra_Dbl4        = " & Agro_SQL_SaveNum(Extra_Dbl4) & "  ")
            strSql.Append("   ,Extra_Dbl5        = " & Agro_SQL_SaveNum(Extra_Dbl5) & "  ")
            strSql.Append("   ,Extra_Dbl6        = " & Agro_SQL_SaveNum(Extra_Dbl6) & "  ")

            strSql.Append("   ,Extra_Str1        = '" & Agro_SQL_SaveText(Extra_Str1) & "'  ")
            strSql.Append("   ,Extra_Str2        = '" & Agro_SQL_SaveText(Extra_Str2) & "'  ")
            strSql.Append("   ,Extra_Str3        = '" & Agro_SQL_SaveText(Extra_Str3) & "'  ")
            strSql.Append("   ,Extra_Str4        = '" & Agro_SQL_SaveText(Extra_Str4) & "'  ")
            strSql.Append("   ,Extra_Str5        = '" & Agro_SQL_SaveText(Extra_Str5) & "'  ")
            strSql.Append("   ,Extra_Str6        = '" & Agro_SQL_SaveText(Extra_Str6) & "'  ")
            strSql.Append("   ,Extra_Str7        = '" & Agro_SQL_SaveText(Extra_Str7) & "'  ")
            strSql.Append("   ,Extra_Str8        = '" & Agro_SQL_SaveText(Extra_Str8) & "'  ")
            strSql.Append("   ,Extra_Str9        = '" & Agro_SQL_SaveText(Extra_Str9) & "'  ")
            strSql.Append("   ,Extra_Date1       =  " & Agro_SQL_SaveDate(Extra_Date1))
            strSql.Append("   ,Extra_Date2       =  " & Agro_SQL_SaveDate(Extra_Date2))
            strSql.Append("   ,Extra_Date3       =  " & Agro_SQL_SaveDate(Extra_Date3))
            strSql.Append("   ,Extra_Date4       =  " & Agro_SQL_SaveDate(Extra_Date4))
            strSql.Append("   ,Extra_Date5       =  " & Agro_SQL_SaveDate(Extra_Date5))
            strSql.Append("   ,Extra_Date6       =  " & Agro_SQL_SaveDate(Extra_Date6))

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Data_modifica))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))


            strSql.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If Piva <> "" Then
                strSql.Append(" AND Materie_Prime_Dettagli.Piva =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Materie_Prime_Dettagli.Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
