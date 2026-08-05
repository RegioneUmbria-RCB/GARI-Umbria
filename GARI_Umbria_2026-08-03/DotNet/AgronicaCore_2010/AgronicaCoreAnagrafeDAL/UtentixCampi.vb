Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class UtentixCampi_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Campo_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentixCampi_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  UtentixCampi , Campi ")
                    StrSQL.AppendLine(" WHERE Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   UtentixCampi.PIVA = Campi.PIVA ")
                    StrSQL.AppendLine(" AND   UtentixCampi.Sa_Cod = Campi.Sa_Cod ")
                    StrSQL.AppendLine(" AND   UtentixCampi.Campo_Cod = Campi.Campo_Cod ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND UtentixCampi.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND UtentixCampi.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND UtentixCampi.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.AppendLine(" AND UtentixCampi.Campo_Cod =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_recode_campo rr where rr.From_Piva = campi.piva and rr.From_Sa_cod = campi.Sa_cod and rr.From_campo_cod = campi.Campo_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_recode_campo rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < campi.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = campi.piva  ")
                            StrSQL.AppendLine("    and rr.From_Sa_cod = campi.Sa_cod  ")
                            StrSQL.AppendLine("    and rr.From_campo_cod = campi.Campo_Cod ")
                            StrSQL.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   UtentixCampi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   UtentixCampi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Campi.Campo_Des ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  UtentixCampi , Campi ")
                    StrSQL.AppendLine(" WHERE Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   UtentixCampi.PIVA = Campi.PIVA ")
                    StrSQL.AppendLine(" AND   UtentixCampi.Sa_Cod = Campi.Sa_Cod ")
                    StrSQL.AppendLine(" AND   UtentixCampi.Campo_Cod = Campi.Campo_Cod ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND UtentixCampi.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND UtentixCampi.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND UtentixCampi.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.AppendLine(" AND UtentixCampi.Campo_Cod =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_recode_campo rr where rr.From_Piva = campi.piva and rr.From_Sa_cod = campi.Sa_cod and rr.From_campo_cod = campi.Campo_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_recode_campo rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < campi.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.From_Piva = campi.piva  ")
                            StrSQL.AppendLine("    and rr.From_Sa_cod = campi.Sa_cod  ")
                            StrSQL.AppendLine("    and rr.From_campo_cod = campi.Campo_Cod ")
                            StrSQL.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   UtentixCampi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   UtentixCampi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Campi.Campo_Des ASC")
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
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class UtentixCampi_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Campo_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixCampi_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO UtentixCampi(       ")
            StrSQL.AppendLine("                    [User],         ")
            StrSQL.AppendLine("                    PIVA,  Sa_Cod, Campo_Cod,      ")
            StrSQL.AppendLine("                    Inviato,            DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixCampi_Write.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE UtentixCampi SET ")
            StrSQL.AppendLine("        Inviato           =  0 ")
            StrSQL.AppendLine("       ,DataInvio         =  Null ")
            StrSQL.AppendLine("       ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva='" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.AppendLine(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
            StrSQL.AppendLine(" AND   Sa_Cod= " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" AND   Campo_Cod= " & Agro_SQL_SaveNum(Campo_Cod) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Campo_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixCampi_Write.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE UtentixCampi ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If objParametri.PivaSuperUser <> "" Then
                    StrSQL.AppendLine(" AND   [User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.AppendLine(" AND   Campo_Cod= " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     UtentixCampi ")
                StrSQL.AppendLine(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If objParametri.PivaSuperUser <> "" Then
                    StrSQL.AppendLine(" AND   [User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.AppendLine(" AND   Campo_Cod= " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Campo_Cod As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixCampi_Write.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE UtentixCampi SET ")
            StrSQL.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" AND  Campo_Cod= " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            StrSQL.AppendLine(" AND  [User]= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Campo_Cod As Integer,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.UtentixCampi_Write.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE UtentixCampi SET ")
            StrSQL.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND  Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" AND  Campo_Cod= " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            StrSQL.AppendLine(" AND  [User]= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND  Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
