Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Imprese_ContrattixTras_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Contratto_Cod As Int32,
                          ByVal Progetto_Cod As Int32,
                          ByVal Fase_Cod As Int32,
                          ByVal Id_Trasformazione As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_ContrattixTras_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Contratto_Cod = 0 
        '   Progetto_Cod = 0    
        '   Fase_Cod = 0
        '   Id_Trasformazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    strSql.Append(" SELECT  Imprese_Contratti.*,  Imprese_Contratto_Fasi.*,  Imprese_Contratto_Fasi.Udm_Cod as Udm_Cod_Fase, Imprese_Contratto_Fasi.Qta as Qta_Fase, Imprese_ContrattixTrasformazioni.* ")
                    strSql.Append(" FROM    Imprese_Contratti,  Imprese_Contratto_Fasi, Imprese_ContrattixTrasformazioni  ")
                    strSql.Append(" WHERE   Imprese_ContrattixTrasformazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND     Imprese_ContrattixTrasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.Append(" AND     Imprese_Contratti.Contratto_Cod = Imprese_ContrattixTrasformazioni.Contratto_Cod ")
                    strSql.Append(" AND     Imprese_Contratto_Fasi.Contratto_Cod = Imprese_ContrattixTrasformazioni.Contratto_Cod ")
                    strSql.Append(" AND     Imprese_Contratto_Fasi.Fase_Cod = Imprese_ContrattixTrasformazioni.Fase_Cod ")


                    If Piva <> "" Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Contratto_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Imprese_Contratti.Inviato >=0 ")
                            strSql.Append(" AND   Imprese_Contratto_Fasi.Inviato >=0 ")
                            strSql.Append(" AND   Imprese_ContrattixTrasformazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Imprese_Contratti.Inviato =-1 ")
                            strSql.Append(" AND   Imprese_Contratto_Fasi.Inviato =-1 ")
                            strSql.Append(" AND   Imprese_ContrattixTrasformazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Veg_Des ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    strSql.Length = 0

                    strSql.Append(" SELECT  Imprese_Contratti.*,  Imprese_Contratto_Fasi.*,  Imprese_Contratto_Fasi.Udm_Cod as Udm_Cod_Fase, Imprese_Contratto_Fasi.Qta as Qta_Fase, Imprese_ContrattixTrasformazioni.* ")
                    strSql.Append(" FROM    Imprese_Contratti,  Imprese_Contratto_Fasi, Imprese_ContrattixTrasformazioni  ")
                    strSql.Append(" WHERE   Imprese_ContrattixTrasformazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND     Imprese_ContrattixTrasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.Append(" AND     Imprese_Contratti.Contratto_Cod = Imprese_ContrattixTrasformazioni.Contratto_Cod ")
                    strSql.Append(" AND     Imprese_Contratto_Fasi.Contratto_Cod = Imprese_ContrattixTrasformazioni.Contratto_Cod ")
                    strSql.Append(" AND     Imprese_Contratto_Fasi.Fase_Cod = Imprese_ContrattixTrasformazioni.Fase_Cod ")


                    If Piva <> "" Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Contratto_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        strSql.Append(" AND Imprese_ContrattixTrasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Imprese_Contratti.Inviato >=0 ")
                            strSql.Append(" AND   Imprese_Contratto_Fasi.Inviato >=0 ")
                            strSql.Append(" AND   Imprese_ContrattixTrasformazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Imprese_Contratti.Inviato =-1 ")
                            strSql.Append(" AND   Imprese_Contratto_Fasi.Inviato =-1 ")
                            strSql.Append(" AND   Imprese_ContrattixTrasformazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Veg_Des ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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


Public Class Imprese_ContrattixTras_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Contratto_Cod As Int32,
                           ByVal Progetto_Cod As Int32,
                           ByVal Fase_Cod As Int32,
                           ByVal Id_Trasformazione As Int32,
                           ByVal Udm_Cod As Int32,
                           ByVal Qta As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_ContrattixTras_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO Imprese_ContrattixTrasformazioni ")
            strSql.Append("         ( ")
            strSql.Append("          Piva,                 Contratto_Cod,  Progetto_Cod, ")
            strSql.Append("          Fase_Cod,             Id_Trasformazione,      ")
            strSql.Append("          Udm_Cod,              Qta,                    ")

            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Contratto_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Trasformazione) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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
    Public Function Modifica(ByVal Piva As String,
                             ByVal Contratto_Cod As Int32,
                             ByVal Progetto_Cod As Int32,
                             ByVal Fase_Cod As Int32,
                             ByVal Id_Trasformazione As Int32,
                             ByVal Udm_Cod As Int32,
                             ByVal Qta As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_ContrattixTras_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.Append(" UPDATE Imprese_ContrattixTrasformazioni SET ")
            strSql.Append("     Udm_Cod           =  " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            strSql.Append("    ,Qta               =  " & Agro_SQL_SaveNum(Qta) & " ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE Piva <> '' ") 'Dummy per il where

            If Piva <> "" Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Contratto_Cod <> 0 Then
                strSql.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.Append(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.Append(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Id_Trasformazione <> 0 Then
                strSql.Append(" AND Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
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

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Contratto_Cod As Int32,
                             ByVal Progetto_Cod As Int32,
                             ByVal Fase_Cod As Int32,
                             ByVal Id_Trasformazione As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Imprese_ContrattixTras_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Contratto_Cod = 0
        '   Progetto_Cod = 0
        '   Fase_Cod = 0
        '   Id_Trasformazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Imprese_ContrattixTrasformazioni ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Imprese_ContrattixTrasformazioni ")
                strSql.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Contratto_Cod <> 0 Then
                strSql.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.Append(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.Append(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Id_Trasformazione <> 0 Then
                strSql.Append(" AND Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
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
