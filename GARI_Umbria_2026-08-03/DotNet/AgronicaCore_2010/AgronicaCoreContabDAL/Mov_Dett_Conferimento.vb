Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Mov_Dett_Conferimento_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Id_Reg_Dettaglio As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Conferimento_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM  Mov_Dettaglio_Conferimento WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Reg_Dettaglio <> 0 Then
                        strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Piva Asc ")
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


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Mov_Dett_Conferimento_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Id_Reg_Dettaglio As Integer,
                           ByVal Tagliando_Pesa As String,
                           ByVal Premio_Complessivo As Decimal,
                           ByVal Prezzo_Unitario_Finale As Decimal,
                           ByVal Cod_Varieta As Integer,
                           ByVal Desc_Appezzamenti As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Conferimento_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

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
            
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Mov_Dettaglio_Conferimento ")
            strSql.AppendLine("   ( Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio ")
            strSql.AppendLine("  , Tagliando_Pesa, Premio_Complessivo, Prezzo_Unitario_Finale ")
            strSql.AppendLine("  , Cod_Varieta, Desc_Appezzamenti ")

            strSql.AppendLine("          , Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("   ) ")
            
            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine(" '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Mov) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")

            strSql.AppendLine(",'" & Agro_SQL_SaveText(Tagliando_Pesa) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Premio_Complessivo) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Prezzo_Unitario_Finale) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Cod_Varieta) & " ")
            strSql.AppendLine(",'" & Agro_SQL_SaveText(Desc_Appezzamenti) & "'")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
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

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Id_Reg_Dettaglio As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Tagliando_Pesa As String = Nothing,
                                     Optional ByVal Premio_Complessivo As Decimal? = Nothing,
                                     Optional ByVal Prezzo_Unitario_Finale As Decimal? = Nothing,
                                     Optional ByVal Cod_Varieta As Integer? = Nothing,
                                     Optional ByVal Desc_Appezzamenti As String = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional Byval Validita_Fine As Date? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Conferimento_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'per alcuni tipi di movimento dettaglio il sa_cod è valorizzato a 0 (non valorizzato)
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            'If Id_Mov = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            'End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Dettaglio_Conferimento ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Tagliando_Pesa) Then
                strSql.AppendLine("   , Tagliando_Pesa = '" & Agro_SQL_SaveText(Tagliando_Pesa) & "' ")
            End If

            If Not IsNothing(Premio_Complessivo) Then
                strSql.AppendLine("   , Premio_Complessivo = " & Agro_SQL_SaveNum(Premio_Complessivo) & " ")
            End If

            If Not IsNothing(Prezzo_Unitario_Finale) Then
                strSql.AppendLine("   , Prezzo_Unitario_Finale = " & Agro_SQL_SaveNum(Prezzo_Unitario_Finale) & " ")
            End If

            If Not IsNothing(Cod_Varieta) Then
                strSql.AppendLine("   , Cod_Varieta = " & Agro_SQL_SaveNum(Cod_Varieta) & " ")
            End If

            If Not IsNothing(Desc_Appezzamenti) Then
                strSql.AppendLine("   , Desc_Appezzamenti = '" & Agro_SQL_SaveText(Desc_Appezzamenti) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '----------------------------------------------------------------------------

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            
            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_det) & " ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
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
    
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Id_Reg_Dettaglio As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dett_Conferimento_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Mov_Dettaglio_Conferimento ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Mov_Dettaglio_Conferimento ")
                strSql.AppendLine(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
            End If


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
