Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Fabbricati_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================  
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Fabbricato_Cod As Integer,
                          ByVal Id_Cod As Integer,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByVal Optional Val_Cod_Esatto As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Fabbricato_Cod = 0
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
                    'TODO: modificare la query select
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT  Fabbricati_Codici.*,  ")
                    strSql.AppendLine("         Fabbricati_Codici.Validita_Inizio AS xValidita_Inizio,  ")
                    strSql.AppendLine("         Fabbricati_Codici.Validita_Fine AS xValidita_Fine, ")
                    strSql.AppendLine("         Codici_Anagrafe.codice,  ")
                    strSql.AppendLine("         Codici_Anagrafe.descrizione ")
                    strSql.AppendLine(" FROM    Fabbricati_Codici INNER JOIN ")
                    strSql.AppendLine("         Codici_Anagrafe ON Fabbricati_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.AppendLine(" WHERE   (Fabbricati_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Fabbricati_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        If Val_Cod_Esatto Then
                            strSql.AppendLine(" AND     (Fabbricati_Codici.val_cod = '" & Agro_SQL_SaveText(Val_Cod) & "')  ")
                        Else
                            strSql.AppendLine(" AND     (Fabbricati_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                        End If
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Fabbricati_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Fabbricati_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Fabbricati_Codici.Validita_Inizio ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    'TODO: modificare la query select
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT  Fabbricati_Codici.*,  ")
                    strSql.AppendLine("         Fabbricati_Codici.Validita_Inizio AS xValidita_Inizio,  ")
                    strSql.AppendLine("         Fabbricati_Codici.Validita_Fine AS xValidita_Fine, ")
                    strSql.AppendLine("         Codici_Anagrafe.codice,  ")
                    strSql.AppendLine("         Codici_Anagrafe.descrizione ")
                    strSql.AppendLine(" FROM    Fabbricati_Codici INNER JOIN ")
                    strSql.AppendLine("         Codici_Anagrafe ON Fabbricati_Codici.id_cod = Codici_Anagrafe.codice ")

                    strSql.AppendLine(" WHERE   (Fabbricati_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    strSql.AppendLine(" AND     (Fabbricati_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                    End If

                    If Fabbricato_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")  ")
                    End If

                    If Id_Cod <> 0 Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND     (Fabbricati_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Fabbricati_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Fabbricati_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Fabbricati_Codici.Validita_Inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
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

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Fabbricati_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Fabbricato_Cod As Integer,
                           ByVal Id_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Val_Cod As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Fabbricato_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Fabbricato_Cod obbligatorio)")
            End If
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Fabbricati_Codici ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Piva,        ")
            strSql.AppendLine("          Sa_Cod,      ")
            strSql.AppendLine("          Fabbricato_Cod,   ")
            strSql.AppendLine("          Id_Cod,      ")
            strSql.AppendLine("          Val_Cod,     ")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")


            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine(") ")

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
                             ByVal Sa_Cod As Integer,
                             ByVal Fabbricato_Cod As Integer,
                             ByVal Id_Cod As Integer,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Fabbricato_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Fabbricato_Cod obbligatorio)")
            End If

            If Id_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Cod obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Fabbricati_Codici SET ")
            strSql.AppendLine("          Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")

            strSql.AppendLine("         ,Inviato           =  0 ")
            strSql.AppendLine("         ,DataInvio         =  Null ")
            strSql.AppendLine("         ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("         ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("         ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("         ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE   Piva =            '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND     Sa_Cod    =        " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(" AND     Fabbricato_cod   = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            strSql.AppendLine(" AND     Id_Cod    =        " & Agro_SQL_SaveNum(Id_Cod) & " ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
                             ByVal Sa_Cod As Integer,
                             ByVal Fabbricato_Cod As Integer,
                             ByVal Id_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_Codici_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Fabbricato_Cod = 0
        '   Id_Cod = 0
        '====================================================================================

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
                strSql.AppendLine(" UPDATE Fabbricati_Codici ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Fabbricati_Codici ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Fabbricato_Cod <> 0 Then
                strSql.AppendLine(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   ")
            End If

            If Id_Cod <> 0 Then
                strSql.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "   ")
            End If
            
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
