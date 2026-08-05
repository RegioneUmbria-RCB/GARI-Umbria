Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class RicettexCultivar_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Veg_Cod As Int32,
                          ByVal Cul_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexCultivar_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Veg_Cod = 0
        '   Cul_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RicettexCultivar ")
                    StrSQL.Append(" WHERE RicettexCultivar.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   RicettexCultivar.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND RicettexCultivar.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RicettexCultivar.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RicettexCultivar.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Veg_Cod, Cul_Cod Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RicettexCultivar ")
                    StrSQL.Append(" WHERE RicettexCultivar.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   RicettexCultivar.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND RicettexCultivar.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexCultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RicettexCultivar.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RicettexCultivar.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Veg_Cod, Cul_Cod Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
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


Public Class RicettexCultivar_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Veg_Cod As Int32,
                           ByVal Cul_Cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO RicettexCultivar ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,      Ricetta_Cod,   Veg_Cod,   Cul_Cod, ")
            strSql.Append("          DataLock,  ")

            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Veg_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Cul_Cod))
            strSql.Append("         , 0 ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")

            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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

    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Veg_Cod As Int32,
                             ByVal Cul_Cod As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE RicettexCultivar SET ")

            strSql.Append("   Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
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

    '##############################################################################################
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal Veg_Cod As Int32,
                             ByVal Cul_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Write.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Veg_Cod = 0
        '   Cul_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE RicettexCultivar ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM RicettexCultivar ")
                strSql.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND RicettexCultivar.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                strSql.Append(" AND RicettexCultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
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

End Class
