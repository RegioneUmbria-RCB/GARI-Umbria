Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Contatti_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi(ByVal Piva As String,
                          ByVal Cod_Contatto As String,
                          ByVal Id_Cod As Integer,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Cod_Contatto = ""
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    'TODO: Modificare la query select
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  Contatti_Codici.* ")
                    strSql.AppendLine(" FROM    Contatti_Codici ")
                    strSql.AppendLine(" WHERE   (Contatti_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Contatti_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    
                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Contatti_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Contatti_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Contatti_Codici.val_cod ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  Contatti_Codici.* ")
                    strSql.AppendLine(" FROM    Contatti_Codici ")
                    strSql.AppendLine(" WHERE   (Contatti_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Contatti_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    
                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND     (Contatti_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Contatti_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Contatti_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Contatti_Codici.val_cod ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '

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

    Public Function LeggiOperatBioContatto_daCod_Contatto(ByVal Piva As String,
                                                          ByVal Cod_Contatto As String,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Cod_Contatto = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  BIO_Dati_OrganismiControllo.Organismo_Cod,BIO_Dati_OrganismiControllo.Organismo_Sigla,BIO_Dati_OrganismiControllo.Codice ")
            strSql.AppendLine(" FROM    Contatti_Codici ")
            strSql.AppendLine(" INNER JOIN  BIO_Dati_OrganismiControllo on ")
            strSql.AppendLine(" BIO_Dati_OrganismiControllo.Organismo_Cod = Contatti_Codici.Val_Cod ")
            strSql.AppendLine(" WHERE   (Contatti_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            strSql.AppendLine(" AND     (Contatti_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


            strSql.AppendLine(" AND     (Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Contatto_OrganismoDiControllo_BIO) & ")  ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     (Contatti_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
            End If

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND     (Contatti_Codici.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')  ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Contatti_Codici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Contatti_Codici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ORDER BY Contatti_Codici.val_cod ASC")

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


Public Class Contatti_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Cod_Contatto As String,
                           ByVal Id_Cod As Integer,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Contatti_Codici_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
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

            strSql.Length = 0

            strSql.AppendLine("INSERT INTO Contatti_Codici( ")
            strSql.AppendLine("                    Piva,        ")
            strSql.AppendLine("                    Sa_Cod,      ")
            strSql.AppendLine("                    Cod_Contatto,")
            strSql.AppendLine("                    Id_Cod,      ")
            strSql.AppendLine("                    Val_Cod,     ")
            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")

            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

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



    '############################################################################
    '############################################################################

    '============================================================================
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Cod_Contatto As String,
                             ByVal Id_Cod As Integer,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Contatti_Codici_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Id_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Cod obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Contatti_Codici SET ")
            strSql.AppendLine("    Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("   ,Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.AppendLine(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'")
            strSql.AppendLine(" AND   Id_Cod = " & Id_Cod & " ")

            '---------------------------------------------

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

    '############################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Cod_Contatto As String,
                             ByVal Id_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Contatti_Codici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Contatti_Codici ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Contatti_Codici ")
                strSql.AppendLine(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato = 0")

            End If

            If Trim(Cod_Contatto) <> "" Then
                strSql.Append(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'")
            End If

            If Id_Cod <> 0 Then
                strSql.Append(" AND   Id_Cod = " & Id_Cod & " ")
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
