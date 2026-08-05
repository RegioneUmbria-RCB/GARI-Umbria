Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class Parco_Macchine_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Mac_Cod As Int32,
                          ByVal Id_Cod As Int32,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Parco_Macchine_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Mac_Cod = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    strSql.Append(" SELECT  Parco_Macchine_Codici.*,  ")
                    strSql.Append("         Parco_Macchine_Codici.Validita_Inizio AS xValidita_Inizio,  ")
                    strSql.Append("         Parco_Macchine_Codici.Validita_Fine AS xValidita_Fine, ")
                    strSql.Append("         Codici_Anagrafe.codice,  ")
                    strSql.Append("         Codici_Anagrafe.descrizione ")
                    strSql.Append(" FROM    Parco_Macchine_Codici INNER JOIN ")
                    strSql.Append("         Codici_Anagrafe ON Parco_Macchine_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.Append(" WHERE   (Parco_Macchine_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.Append(" AND     (Parco_Macchine_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Mac_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.Mac_cod = " & Agro_SQL_SaveNum(Mac_Cod) & ")  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Parco_Macchine_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Parco_Macchine_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.Append(" SELECT  Parco_Macchine_Codici.*,  ")
                    strSql.Append("         Parco_Macchine_Codici.Validita_Inizio AS xValidita_Inizio,  ")
                    strSql.Append("         Parco_Macchine_Codici.Validita_Fine AS xValidita_Fine, ")
                    strSql.Append("         Codici_Anagrafe.codice,  ")
                    strSql.Append("         Codici_Anagrafe.descrizione ")
                    strSql.Append(" FROM    Parco_Macchine_Codici INNER JOIN ")
                    strSql.Append("         Codici_Anagrafe ON Parco_Macchine_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.Append(" WHERE   (Parco_Macchine_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.Append(" AND     (Parco_Macchine_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If Piva <> "" Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Mac_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.Mac_cod = " & Agro_SQL_SaveNum(Mac_Cod) & ")  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.Append(" AND     (Parco_Macchine_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Parco_Macchine_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Parco_Macchine_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perché 0 è significativo
    Public Function LeggiJoinParcoMacchine(ByVal Piva As String,
                                           ByVal Mac_Cod As Int32,
                                           ByVal Sa_Cod As Int32,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_R.LeggiJoinParcoMacchine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT *  ")
            strSql.Append(" FROM   Parco_Macchine_Codici ")
            strSql.Append(" INNER JOIN Parco_Macchine ")
            strSql.Append(" ON Parco_Macchine_Codici.piva = Parco_Macchine.piva AND Parco_Macchine_Codici.Mac_Cod = Parco_Macchine.Mac_Cod ")

            strSql.Append(" WHERE  Parco_Macchine_Codici.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND    Parco_Macchine_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Mac_Cod <> 0 Then
                strSql.Append(" AND Parco_Macchine_Codici.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.Append(" AND Parco_Macchine_Codici.piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.Append(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Parco_Macchine_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Parco_Macchine_Codici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Parco_Macchine_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Mac_Cod As Int32,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Parco_Macchine_Codici_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.Append("INSERT INTO Parco_Macchine_Codici ")
            strSql.Append("                    (")
            strSql.Append("                    Piva,        ")
            strSql.Append("                    Sa_Cod,      ")
            strSql.Append("                    Mac_Cod,     ")
            strSql.Append("                    Id_Cod,      ")
            strSql.Append("                    Val_Cod,     ")
            strSql.Append("                    Inviato, DataInvio, ")
            strSql.Append("                    Data_Creazione,     Data_Modifica, ")
            strSql.Append("                    UserName_Creazione, UserName_Modifica, ")
            strSql.Append("                    Validita_Inizio,    Validita_Fine ")
            strSql.Append("                    ) ")
            strSql.Append("VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.Append(")")

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
                             ByVal Sa_Cod As Int32,
                             ByVal Mac_Cod As Int32,
                             ByVal Id_Cod As Int32,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Parco_Macchine_Codici_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Mac_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            End If

            If Id_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append("UPDATE Parco_Macchine_Codici SET ")
            strSql.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.Append(" AND   Mac_Cod   =  " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            strSql.Append(" AND   Id_Cod    =  " & Agro_SQL_SaveNum(Id_Cod) & " ")

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
                             ByVal Sa_Cod As Int32,
                             ByVal Mac_Cod As Int32,
                             ByVal Id_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Parco_Macchine_Codici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            'If Mac_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Mac_Cod obbligatorio)")
            'End If

            'If Id_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Cod obbligatorio)")
            'End If


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Parco_Macchine_Codici ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM     Parco_Macchine_Codici ")
                strSql.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND Inviato = 0")

            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod = " & Sa_Cod & " ")
            End If

            If Mac_Cod <> 0 Then
                strSql.Append(" AND Mac_Cod = " & Mac_Cod & " ")
            End If

            If Id_Cod <> 0 Then
                strSql.Append(" AND Id_Cod = " & Id_Cod & " ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------
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
