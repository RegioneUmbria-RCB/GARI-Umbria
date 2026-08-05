Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CorpiEstranei_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Piva As String,
                           ByVal Cod_CorpoEstraneo As Integer,
                           ByVal Desc_CorpoEstraneo As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO CorpiEstranei ")
            StrSQL.AppendLine("             (Piva_SuperUser,    Piva,   ")
            StrSQL.AppendLine("               Cod_CorpoEstraneo, Desc_CorpoEstraneo, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")

            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Desc_CorpoEstraneo) & "'  ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
                             ByVal Cod_CorpoEstraneo As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Cod_CorpoEstraneo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_CorpoEstraneo obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE CorpiEstranei ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND     Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
                StrSQL.AppendLine(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM    CorpiEstranei ")
                StrSQL.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND     Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CorpiEstranei_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(ByVal Cod_CorpoEstraneo As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Cod_CorpoEstraneo , Desc_CorpoEstraneo")
                    StrSQL.AppendLine(" FROM  CorpiEstranei ")
                    StrSQL.AppendLine(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cod_CorpoEstraneo <> 0 Then
                        StrSQL.AppendLine(" AND Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Desc_CorpoEstraneo")
                    End If
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  CorpiEstranei ")
                    StrSQL.AppendLine(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cod_CorpoEstraneo <> 0 Then
                        StrSQL.AppendLine(" AND Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Desc_CorpoEstraneo")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT CorpiEstranei.Cod_CorpoEstraneo, CorpiEstranei.Desc_CorpoEstraneo, CorpiEstranei_LimitiPericolosita.LimiteMax_Aeroseparatori,  ")
                    StrSQL.AppendLine(" CorpiEstranei_LimitiPericolosita.LimiteMax_CernitriciOttiche, CorpiEstranei_LimitiPericolosita.LimiteMax_CernitaManuale  ")
                    StrSQL.AppendLine(" FROM         CorpiEstranei INNER JOIN ")
                    StrSQL.AppendLine(" CorpiEstranei_LimitiPericolosita ON CorpiEstranei.Cod_CorpoEstraneo = CorpiEstranei_LimitiPericolosita.Cod_CorpoEstraneo ")
                    StrSQL.AppendLine(" WHERE CorpiEstranei.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   CorpiEstranei.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cod_CorpoEstraneo <> 0 Then
                        StrSQL.AppendLine(" AND CorpiEstranei.Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   CorpiEstranei.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   CorpiEstranei.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY CorpiEstranei.Desc_CorpoEstraneo")
                    End If

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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Controlla se esiste già un corpo estraneo passatagli la descrizione 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EsisteCE(ByVal Descrizione As String,
                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.EsisteCE()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Cod_CorpoEstraneo , Desc_CorpoEstraneo")
                    StrSQL.AppendLine(" FROM  CorpiEstranei ")
                    StrSQL.AppendLine(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Descrizione <> "" Then
                        StrSQL.AppendLine(" AND LOWER(Desc_CorpoEstraneo) = LOWER('" & Agro_SQL_SaveText(Descrizione) & "') ")
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    StrSQL.AppendLine(" ORDER BY Desc_CorpoEstraneo")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  CorpiEstranei ")
                    StrSQL.AppendLine(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Descrizione <> "" Then
                        StrSQL.AppendLine(" AND LOWER(Desc_CorpoEstraneo) = LOWER('" & Agro_SQL_SaveText(Descrizione) & "') ")
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    StrSQL.AppendLine(" ORDER BY Desc_CorpoEstraneo")

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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' CSontrolla se esiste già un corpo estraneo passatagli la descrizione 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiDescrizioneCE(ByVal Cod_CorpoEstraneo As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.LeggiDescrizioneCE()"

        Dim dt As DataTable
        dt = Leggi(Cod_CorpoEstraneo,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Desc_CorpoEstraneo")
        End If

        Return ""

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Controlla se esiste già un corpo estraneo passatagli la descrizione 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiRegistrazioneBolla(ByVal Piva As String,
                                            ByVal Numero_Bolla_Sin As String,
                                            ByVal Numero_Bolla As Integer,
                                            ByVal Numero_Bolla_Des As String,
                                            ByVal Anno_Bolla As Integer,
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.EsisteRegistrazioneBolla()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.AppendLine("SELECT     Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, Movimenti.Mov_Desc, Movimenti.Doc_Numero, ")
                    StrSQL.AppendLine(" Movimenti.Doc_Numero_Des, Movimenti.Doc_Numero_Sin, Movimenti.Extra_Int, Movimenti.Colli, Movimenti.Extra_Date, Movimenti.Ora,  Movimenti.Extra_Str, ")
                    StrSQL.AppendLine(" Movimenti.Natura_Beni, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Mat_Cod")
                    StrSQL.AppendLine(" FROM Agenda INNER JOIN  Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN  Movimenti_dettagli ON Agenda.PIVA = Movimenti_dettagli.PIVA ")
                    StrSQL.AppendLine(" AND Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  ")

                    StrSQL.AppendLine(" WHERE     (Agenda.Lav_Cod = 5003) ")

                    If Anno_Bolla <> 0 Then
                        StrSQL.AppendLine(" AND Movimenti.Extra_Int = " & Agro_SQL_SaveNum(Anno_Bolla) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND  Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Numero_Bolla_Sin <> "" Then
                        StrSQL.AppendLine(" AND  Doc_Numero_Sin = '" & Agro_SQL_SaveText(Numero_Bolla_Sin) & "' ")
                    End If

                    If Numero_Bolla <> 0 Then
                        StrSQL.AppendLine(" AND  Doc_Numero = " & Agro_SQL_SaveNum(Numero_Bolla) & " ")
                    End If

                    If Numero_Bolla_Des <> "" Then
                        StrSQL.AppendLine(" AND  Doc_Numero_Des = '" & Agro_SQL_SaveText(Numero_Bolla_Des) & "' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Controlla se esiste già un corpo estraneo passatagli la descrizione 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EsisteRegistrazioneBolla(ByVal Piva As String,
                                             ByVal Numero_Bolla_Sin As String,
                                             ByVal Numero_Bolla As Integer,
                                             ByVal Numero_Bolla_Des As String,
                                             ByVal Anno_Bolla As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.EsisteRegistrazioneBolla()"

        Dim dt As DataTable
        dt = LeggiRegistrazioneBolla(Piva, Numero_Bolla_Sin, Numero_Bolla, Numero_Bolla_Des, Anno_Bolla,
                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "", objParametri)
        If dt.Rows.Count > 0 Then
            Return True
        End If

        Return False

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge la tabella dei limiti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiLimitiPericolosita(ByVal ID As Integer,
                                            ByVal Piva_SuperUser As String,
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.LeggiLimitiPericolosita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM CorpiEstranei_LimitiPericolosita ")
                    StrSQL.AppendLine(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID <> 0 Then
                        StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")
                    End If

                    If Piva_SuperUser <> "" Then
                        StrSQL.AppendLine(" AND Piva_SuperUser = " & Agro_SQL_SaveText(Piva_SuperUser) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Controlla il superamento del limite
    ''' ritorna TRUE se il limite è superato
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function SuperatoLimitePericolosita(ByVal Cod_corpoEstraneo As Integer,
                                               ByVal Aereoseparatore As Integer,
                                               ByVal CernitriceOttica As Integer,
                                               ByVal CernitriceManuale As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.SuperatoLimitePericolosita()"

        Dim dt As DataTable
        dt = LeggiLimitiPericolosita(0, "",
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     " Cod_CorpoEstraneo = " & Cod_corpoEstraneo,
                                     "", objParametri)

        If dt.Rows.Count > 0 Then
            ''controllo i vari limiti 
            If dt.Rows(0).Item("LimiteMax_Aeroseparatori") < Aereoseparatore Then
                Return True
            End If

            If dt.Rows(0).Item("LimiteMax_CernitriciOttiche") < CernitriceOttica Then
                Return True
            End If

            If dt.Rows(0).Item("LimiteMax_CernitaManuale") < CernitriceManuale Then
                Return True
            End If
        End If

        Return False

    End Function

    Private Shared Sub ValorizzazioneModalitaScrittura(ByVal modalitaScritturaConferimenti As String,
                                                       ByRef cau_movCollegamentoRaccolta As String,
                                                       ByRef nomeTabellaJoinCentroFabbricato As String,
                                                       ByRef siglaTabellaJoinCentroFabbricato As String,
                                                       ByRef desStabilimento As String,
                                                       ByRef fabbrCodStabilimento As String,
                                                       ByRef dataCambioModalitaConferimento As Date)

        dataCambioModalitaConferimento = #1/1/2021#

        Select Case modalitaScritturaConferimenti
            Case "GIASLAN"
                cau_movCollegamentoRaccolta = CAU_ACCETTAZIONE_BENI_DA_DIVERSI
                nomeTabellaJoinCentroFabbricato = "Fabbricati"
                siglaTabellaJoinCentroFabbricato = "Fabbr_Raccolta"
                desStabilimento = "Fabbricato_Des"
                fabbrCodStabilimento = $"{siglaTabellaJoinCentroFabbricato}.Fabbricato_Cod"
            Case Else
                cau_movCollegamentoRaccolta = CAU_CARICO
                nomeTabellaJoinCentroFabbricato = "Centri_Aziendali"
                siglaTabellaJoinCentroFabbricato = "CA_Raccolta"
                desStabilimento = "Sa_Nome"
                fabbrCodStabilimento = "0"
        End Select
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge la registrazione dei corpi estranei e le bolle relative
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiCorpiEstraneiBolle(ByVal Flag_Distinct As Boolean,
                                            ByVal Piva_Produttore As String,
                                            ByVal dal_Data_Arrivo As Date,
                                            ByVal al_Data_Arrivo As Date,
                                            ByVal Codice_Specie As Integer,
                                            ByVal Codice_Prodotto As Integer,
                                            ByVal Flag_Join_Appezzamento As Boolean,
                                            ByVal Sa_Cod_Appezzamento As Integer,
                                            ByVal Appezza_Appezzamento As Integer,
                                            ByVal Id_Reg_Appezzamento As Integer,
                                            ByVal Tipologia_Ritrovamento As Integer,
                                            ByVal Indice_Pericolosita As Integer,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Fabbricato_Cod As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.LeggiCorpiEstraneiBolle()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim Stb_Select As New Text.StringBuilder
        Dim Stb_SelectRif As New Text.StringBuilder
        Dim Stb_Join As New Text.StringBuilder
        Dim Stb_JoinRif As New Text.StringBuilder
        Dim Stb_Where As New Text.StringBuilder
        Dim Stb_WhereRif As New Text.StringBuilder

        Dim dt As DataTable

        Dim modalitaScritturaConferimenti As String = ""
        Dim cau_movCollegamentoRaccolta As String = ""
        Dim nomeTabellaJoinCentroFabbricato As String = ""
        Dim siglaTabellaJoinCentroFabbricato As String = ""
        Dim desStabilimento As String = ""
        Dim fabbrCodStabilimento As String = ""
        Dim dataCambioModalitaConferimento As Date

        Try
            StrSQL.Length = 0

            ' ciclo per le due modalità di scrittura dei conferimenti

            StrSQL.AppendLine(" ( ")
            For i As Integer = 1 To 2

                'i = 1 ==> modalità pre 2021 (conferimenti scritti dal LAN)
                'i = 2 ==> modalità dal 2021 (conferimenti scritti dal WEB)
                If i = 1 Then
                    modalitaScritturaConferimenti = "GIASLAN"
                Else
                    modalitaScritturaConferimenti = "WEB"
                End If

                ValorizzazioneModalitaScrittura(modalitaScritturaConferimenti,
                                                cau_movCollegamentoRaccolta,
                                                nomeTabellaJoinCentroFabbricato,
                                                siglaTabellaJoinCentroFabbricato,
                                                desStabilimento,
                                                fabbrCodStabilimento,
                                                dataCambioModalitaConferimento)

                If i = 2 Then
                    'Svuoto gli StringBuilder perché già aggiunti alla query principale
                    Stb_Select.Length = 0
                    Stb_SelectRif.Length = 0
                    Stb_Join.Length = 0
                    Stb_JoinRif.Length = 0
                    Stb_Where.Length = 0
                    Stb_WhereRif.Length = 0

                    StrSQL.AppendLine(" ")
                    StrSQL.AppendLine(" UNION ALL ")
                    StrSQL.AppendLine(" ")
                End If
                StrSQL.AppendLine(" ( ")
                StrSQL.AppendLine($"     ---- [SCRITTURA FATTA DA {modalitaScritturaConferimenti}] ")

                ' QUI QUERY VERA E PROPRIA



                'la union deve avere le stesse colonne della prima select e nello stesso ordine
                'quindi devo mettere la condizione sul not distinct
                If Not Flag_Distinct AndAlso Flag_Join_Appezzamento Then
                    StrSQL.AppendLine(" ( ")
                End If

                ' CONVERT(varchar(50), CONVERT(int, Movimenti_CE.Doc_Numero)) 

                If Flag_Distinct Then
                    Stb_Select.AppendLine("SELECT  DISTINCT Agenda_CE.PIVA as Piva, Agenda_CE.Sa_Cod , Agenda_CE.Id_Agenda, Movimenti_CE.Id_Mov, Movimenti_CE.Extra_Int, CONVERT(VARCHAR(11),Movimenti_CE.Data_Movimento,106) as Data_Movimento ,  ")
                    Stb_Select.AppendLine(" CONVERT(VARCHAR(11), Movimenti_Bolla_Accettazione.Data_Movimento, 106) AS Data_Bolla, ")
                    Stb_Select.AppendLine(" (Movimenti_CE.Doc_Numero_Sin +     RIGHT(   ('00000'+  CONVERT(varchar(50), CONVERT(int, Movimenti_CE.Doc_Numero))     ), 5   ) +  Movimenti_CE.Doc_Numero_Des    ) As Numero_Bolla, Movimenti_Bolla_Accettazione.Cod_RisUm,")
                    Stb_Select.AppendLine(" Movimenti_Bolla_Accettazione.Cod_RisUm,")
                    Stb_Select.AppendLine(" Movimenti_CE.Natura_Beni, Movimenti_CE.Extra_Str, CONVERT(VARCHAR(11),Movimenti_CE.Extra_Date,106) as Extra_Date, CONVERT(VARCHAR(5),Movimenti_CE.Ora,108) as Ora , Movimenti_CE.Colli, Movimenti_CE.Mov_Desc,  ")
                    Stb_Select.AppendLine(" Materie_Prime.Veg_Cod, Materie_Prime.Mat_Des, Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, Movimenti_Dettagli_Raccolta.QTA , CONVERT(VARCHAR(11),Movimenti_CE.Data_Movimento,106) as 'Data di Arrivo', ")
                    Stb_Select.AppendLine(" datepart(year, Movimenti_CE.Data_Movimento) AS Anno_Carico, datepart(month,Movimenti_CE.Data_Movimento) AS Mese_Carico, datepart(DAY,Movimenti_CE.Data_Movimento)  AS Giorno_Carico, ")
                    Stb_Select.AppendLine($" {siglaTabellaJoinCentroFabbricato}.sa_cod AS Stabilimento_Sa_Cod, {fabbrCodStabilimento} AS Stabilimento_Fabbr_Cod, {siglaTabellaJoinCentroFabbricato}.{desStabilimento} AS Stabilimento ")
                Else
                    Stb_Select.AppendLine(" SELECT  Movimenti_CE.Extra_Int as Anno, ")
                    Stb_Select.AppendLine(" (Movimenti_CE.Doc_Numero_Sin +     RIGHT(   ('00000'+  CONVERT(varchar(50), CONVERT(int, Movimenti_CE.Doc_Numero))     ), 5   ) +  Movimenti_CE.Doc_Numero_Des    ) As 'Numero Bolla',      ")
                    Stb_Select.AppendLine(" CONVERT(VARCHAR(11),Movimenti_CE.Data_Movimento,106) as 'Data di Arrivo', Movimenti_CE.Colli as 'Carico Numero',     ")
                    Stb_Select.AppendLine(" Movimenti_Dettagli_Raccolta.QTA as 'Peso Netto', Movimenti_Dettagli_Raccolta.Variazione as 'Degrado', Movimenti_Dettagli_Raccolta.udm_cod, 0 AS 'Netto Pagamento',   ")
                    Stb_Select.AppendLine(" Materie_Prime.Mat_Des as 'Specie - Varieta',    ")
                    Stb_Select.AppendLine(" ISNULL( (SELECT TOP 1 VAL_COD      ")
                    Stb_Select.AppendLine(" FROM Materie_Prime_Campionature MP_Camp_Indice    ")
                    Stb_Select.AppendLine(" WHERE MP_Camp_Indice.progressivo = Movimenti_Dettagli_Raccolta.cal_cod   ")
                    Stb_Select.AppendLine(" AND MP_Camp_Indice.tipo = 'indice' AND MP_Camp_Indice.tipo_cod IN ('999', '12','1')  ), 0 ) AS Punteggio,    ")
                    Stb_Select.AppendLine(" ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro    ")
                    Stb_Select.AppendLine(" FROM materie_prime_calibri   ")
                    Stb_Select.AppendLine(" INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod    ")
                    Stb_Select.AppendLine(" WHERE MP_Camp_Calibro.progressivo = Movimenti_Dettagli_Raccolta.cal_cod    ")
                    Stb_Select.AppendLine(" AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro ,   ")
                    Stb_Select.AppendLine(" Contatti_Conferenti.Rag_Soc AS 'Ragione Sociale Conferente', ")
                    Stb_Select.AppendLine(" ISNULL(Contatti_Produttori.Rag_Soc, '') AS 'Ragione Sociale Produttore', ")
                    Stb_Select.AppendLine(" -- Appezzamento.App_Nome as Appezzamento , Reg_Impianti.Sup_Imp as 'Superficie Impianto',   ")
                    Stb_Select.AppendLine(" -- CONVERT(VARCHAR(11),Reg_Impianti.Validita_Inizio ,106)as 'Data Inizio Impianto',  ")
                    Stb_Select.AppendLine(" CONVERT(VARCHAR(11),Movimenti_CE.Extra_Date,106) + ' - '+ CONVERT(VARCHAR(5),Movimenti_CE.Ora,108) as 'Data-Ora Inizio Cottura',  ")
                    Stb_Select.AppendLine(" Movimenti_CE.Natura_Beni as 'Confezione / Marchio 1', ")
                    Stb_Select.AppendLine(" Movimenti_CE.Aspetto as 'Confezione / Marchio 2',  Movimenti_CE.Extra_Str  as 'Livello Qualitativo', CONVERT(VARCHAR(50), Movimenti_CE.Tipo_Sconto) as 'Pesticidi',  ")
                    Stb_Select.AppendLine(" Movimenti_Dettagli_CE.Fase_Cod as Fase_Cod, ")
                    Stb_Select.AppendLine(" --'case per il separatore   ")
                    Stb_Select.AppendLine(" CASE Movimenti_Dettagli_CE.Mat_Cod  ")
                    Stb_Select.AppendLine(" WHEN '1' THEN 'Aereoseparatore'  ")
                    Stb_Select.AppendLine(" WHEN '2' THEN 'Cernitrice Ottica'  ")
                    Stb_Select.AppendLine(" WHEN '3' THEN 'Cernita Manuale'  ")
                    Stb_Select.AppendLine(" ELSE 'Errore Conversione'  ")
                    Stb_Select.AppendLine(" END as 'Tipologia Ritrovamento',  ")
                    Stb_Select.AppendLine(" (Select CorpiEstranei.Desc_CorpoEstraneo from CorpiEstranei where  CorpiEstranei.Cod_CorpoEstraneo=Movimenti_Dettagli_CE.Pro_Cod) as 'Corpo Estraneo',  ")
                    Stb_Select.AppendLine(" Movimenti_Dettagli_CE.QTA as 'Quantita Rilevata' ,  ")
                    Stb_Select.AppendLine(" --'case per il Pericolosita   ")
                    Stb_Select.AppendLine(" Case Movimenti_Dettagli_CE.Cod_Progetto  ")
                    Stb_Select.AppendLine(" WHEN '0' THEN 'Medio/Bassa'  ")
                    Stb_Select.AppendLine(" WHEN '1' THEN 'Alta'  ")
                    Stb_Select.AppendLine(" ELSE 'Errore Conversione'  ")
                    Stb_Select.AppendLine(" END as  'Pericolosita',  ")
                    Stb_Select.AppendLine(" '' as  'Linea 1 - Aereoseparatore' ,'' as  'Linea 1 - Cernitrice Ottica' ,'' as  'Linea 1 - Cernita Manuale' , ")
                    Stb_Select.AppendLine(" '' as  'Linea 2 - Aereoseparatore' ,'' as  'Linea 2 - Cernitrice Ottica' ,'' as  'Linea 2 - Cernita Manuale' , ")
                    Stb_Select.AppendLine(" Movimenti_CE.Mov_Desc as 'Note',  ")
                    Stb_Select.AppendLine($" {siglaTabellaJoinCentroFabbricato}.sa_cod AS Stabilimento_Sa_Cod, {fabbrCodStabilimento} AS Stabilimento_Fabbr_Cod, {siglaTabellaJoinCentroFabbricato}.{desStabilimento} AS Stabilimento ")

                End If

                If Flag_Join_Appezzamento Then
                    Stb_SelectRif.AppendLine(" , APPEZZAMENTO.APP_NOME AS 'Nome Appezzamento', Reg_Impianti.Validita_Inizio AS 'Data Inizio Impianto', Reg_Impianti.SUP_IMP AS 'Superficie Impianto' ")
                    'StrSQL.AppendLine("  -- Imprese_Progetti.Progetto_Nome, ")
                End If

                StrSQL.Append(Stb_Select)
                StrSQL.Append(Stb_SelectRif)

                Stb_Join.AppendLine("FROM     Agenda AS Agenda_CE  ")

                Stb_Join.AppendLine("INNER JOIN Movimenti AS Movimenti_CE ON Movimenti_CE.Id_Agenda = Agenda_CE.Id_Agenda AND Movimenti_CE.PIVA = Agenda_CE.PIVA AND ")
                Stb_Join.AppendLine("Movimenti_CE.Sa_Cod = Agenda_CE.Sa_Cod  ")

                Stb_Join.AppendLine("INNER JOIN Movimenti_dettagli AS Movimenti_Dettagli_CE ON Movimenti_CE.Id_Mov = Movimenti_Dettagli_CE.Id_Mov AND  ")
                Stb_Join.AppendLine("Movimenti_CE.PIVA = Movimenti_Dettagli_CE.PIVA AND Movimenti_CE.Sa_Cod = Movimenti_Dettagli_CE.Sa_Cod AND  ")
                Stb_Join.AppendLine("Movimenti_CE.Id_Agenda = Movimenti_Dettagli_CE.Id_Agenda  ")

                Stb_Join.AppendLine("INNER JOIN Movimenti AS Movimenti_Bolla_Accettazione ON Movimenti_CE.Doc_Numero = Movimenti_Bolla_Accettazione.Doc_Numero AND  ")
                Stb_Join.AppendLine("Movimenti_CE.Doc_Numero_Des = Movimenti_Bolla_Accettazione.Doc_Numero_Des AND  ")
                Stb_Join.AppendLine("Movimenti_CE.Doc_Numero_Sin = Movimenti_Bolla_Accettazione.Doc_Numero_Sin AND  ")
                Stb_Join.AppendLine("Movimenti_CE.Extra_Int = YEAR(Movimenti_Bolla_Accettazione.Data_Movimento)  ")

                Stb_Join.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Movimenti_Bolla_Accettazione.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
                Stb_Join.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
                Stb_Join.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Movimenti_Bolla_Accettazione.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
                Stb_Join.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")

                Stb_Join.AppendLine(" INNER JOIN Agenda AS Agenda_Bolla ON Movimenti_Bolla_Accettazione.PIVA = Agenda_Bolla.PIVA AND  ")
                Stb_Join.AppendLine(" Movimenti_Bolla_Accettazione.Sa_Cod = Agenda_Bolla.Sa_Cod AND Movimenti_Bolla_Accettazione.Id_Agenda = Agenda_Bolla.Id_Agenda  ")

                Stb_Join.AppendLine(" INNER JOIN Movimenti AS Movimenti_Bolla_Raccolta ON Agenda_Bolla.PIVA = Movimenti_Bolla_Raccolta.PIVA AND ")
                Stb_Join.AppendLine(" Agenda_Bolla.Sa_Cod = Movimenti_Bolla_Raccolta.Sa_Cod AND Agenda_Bolla.Id_Agenda = Movimenti_Bolla_Raccolta.Id_Agenda ")

                Stb_Join.AppendLine(" INNER JOIN Movimenti_dettagli AS Movimenti_Dettagli_Raccolta ON Movimenti_Bolla_Raccolta.PIVA = Movimenti_Dettagli_Raccolta.PIVA AND ")
                Stb_Join.AppendLine(" Movimenti_Bolla_Raccolta.Id_Mov = Movimenti_Dettagli_Raccolta.Id_Mov AND ")
                Stb_Join.AppendLine(" Movimenti_Bolla_Raccolta.Id_Agenda = Movimenti_Dettagli_Raccolta.Id_Agenda ")

                Stb_Join.AppendLine(" INNER JOIN Materie_Prime ON Movimenti_Dettagli_Raccolta.Elem_Cod = Materie_Prime.Elem_Cod AND ")
                Stb_Join.AppendLine(" Movimenti_Dettagli_Raccolta.Mat_Cod = Materie_Prime.Mat_Cod")

                Stb_Join.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Movimenti_Dettagli_Raccolta.PIVA = Mov_Dest_Raccolta.Piva ")
                Stb_Join.AppendLine(" AND Movimenti_Dettagli_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Movimenti_Dettagli_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda ")
                Stb_Join.AppendLine(" AND Movimenti_Dettagli_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Movimenti_Dettagli_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   ")

                Stb_Join.AppendLine($" INNER JOIN {nomeTabellaJoinCentroFabbricato} {siglaTabellaJoinCentroFabbricato} ON Mov_Dest_Raccolta.Piva = {siglaTabellaJoinCentroFabbricato}.PIVA AND Mov_Dest_Raccolta.Sa_Cod = {siglaTabellaJoinCentroFabbricato}.SA_COD ")
                If modalitaScritturaConferimenti = "GIASLAN" Then
                    Stb_Join.AppendLine($"             And Mov_Dest_Raccolta.Id_Destinazione = {siglaTabellaJoinCentroFabbricato}.Fabbricato_Cod ")
                End If

                Stb_Join.AppendLine(" INNER JOIN specieVegetali On specieVegetali.Veg_Cod = Materie_Prime.Veg_Cod ")

                If Flag_Join_Appezzamento Then
                    Stb_JoinRif.AppendLine(" INNER JOIN  Mov_Dettagli_Riferimenti ")
                    Stb_JoinRif.AppendLine(" On Mov_Dettagli_Riferimenti.Piva = Agenda_Bolla.PIVA   ")
                    Stb_JoinRif.AppendLine(" And Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Bolla.Sa_Cod  ")
                    Stb_JoinRif.AppendLine(" And Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Bolla.Id_Agenda   ")
                    'la raccolta sta in piva_rif sa_cod_rif lav_cod_rif
                    Stb_JoinRif.AppendLine(" INNER JOIN  Agenda Agenda_RaccoltaImpianti On Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA  ")
                    Stb_JoinRif.AppendLine("             And Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod  ")
                    Stb_JoinRif.AppendLine("             And Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda  ")
                    Stb_JoinRif.AppendLine(" INNER JOIN  Movimenti Movimenti_RaccoltaImpianti On Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA  ")
                    Stb_JoinRif.AppendLine("             And Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda ")
                    Stb_JoinRif.AppendLine(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti On Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov  ")
                    Stb_JoinRif.AppendLine(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti On Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  ")
                    Stb_JoinRif.AppendLine("             And Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  ")
                    Stb_JoinRif.AppendLine(" INNER JOIN Reg_Impianti On Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
                    Stb_JoinRif.AppendLine("             And Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
                    Stb_JoinRif.AppendLine("             And Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
                    Stb_JoinRif.AppendLine("             And Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione ")
                    Stb_JoinRif.AppendLine(" INNER JOIN APPEZZAMENTO On Reg_Impianti.PIVA = APPEZZAMENTO.Piva  ")
                    Stb_JoinRif.AppendLine("             And Reg_Impianti.Sa_Cod = APPEZZAMENTO.Sa_Cod  ")
                    Stb_JoinRif.AppendLine("             And Reg_Impianti.Appezza = APPEZZAMENTO.Appezza  ")
                End If

                StrSQL.Append(Stb_Join)
                StrSQL.Append(Stb_JoinRif)

                'sbagliato!
                'If Flag_Join_Appezzamento = True Then
                '    StrSQL.AppendLine(" INNER JOIN Appezzamento On Contatti_Produttori.Cod_Contatto = Appezzamento.PIVA ")
                '    StrSQL.AppendLine(" INNER JOIN Reg_Impianti On Appezzamento.PIVA = Reg_Impianti.PIVA ")
                '    StrSQL.AppendLine("And Appezzamento.SA_COD = Reg_Impianti.SA_COD And ")
                '    StrSQL.AppendLine("Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
                '    StrSQL.AppendLine("And Materie_Prime.Cul_Cod = Reg_Impianti.CUL_COD ")
                'End If

                Stb_Where.AppendLine("WHERE    (Agenda_CE.Lav_Cod = " & CStr(LAVCOD_MONITORAGGIO_CE) & ")  ")
                Stb_Where.AppendLine("And      (Movimenti_Bolla_Accettazione.Cau_Mov = '" & CAU_REGISTRAZIONI & "') ")
                Stb_Where.AppendLine("AND      (Agenda_Bolla.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) & ") ")
                Stb_Where.AppendLine("AND      (Movimenti_Bolla_Raccolta.Cau_Mov = '" & Cstr(cau_movCollegamentoRaccolta) & "') ")
                Stb_Where.AppendLine("AND      (Movimenti_Dettagli_Raccolta.Elem_Cod = " & CStr(TRASFORMATI_VEGETALI) & ") ")

                If modalitaScritturaConferimenti = "GIASLAN" Then
                    'modalità pre 2021 (conferimenti scritti dal LAN)
                    'devo escludere le bolle di accettazione scritte dal web
                    Stb_Where.AppendLine($" AND Movimenti_Bolla_Accettazione.Data_Movimento < {Agro_SQL_SaveDate(dataCambioModalitaConferimento)} ")
                    Stb_Where.AppendLine($" AND Movimenti_Bolla_Raccolta.Data_Movimento < {Agro_SQL_SaveDate(dataCambioModalitaConferimento)} ")
                Else
                    'modalità dal 2021 (conferimenti scritti dal WEB)
                    'devo escludere le bolle di accettazione scritte dal lan
                    Stb_Where.AppendLine($" AND Movimenti_Bolla_Accettazione.Data_Movimento >= {Agro_SQL_SaveDate(dataCambioModalitaConferimento)} ")
                    Stb_Where.AppendLine($" AND Movimenti_Bolla_Raccolta.Data_Movimento >= {Agro_SQL_SaveDate(dataCambioModalitaConferimento)} ")
                End If


                ''controllo sulla data
                Stb_Where.AppendLine(" AND     Movimenti_CE.Data_Movimento <= " & Agro_SQL_SaveDate(al_Data_Arrivo) & " ")
                Stb_Where.AppendLine(" AND     Movimenti_CE.Data_Movimento >= " & Agro_SQL_SaveDate(dal_Data_Arrivo) & " ")

                If Piva_Produttore <> "" Then
                    Stb_Where.AppendLine(" AND Contatti_Produttori.Cod_Contatto='" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
                End If

                If Codice_Specie <> 0 Then
                    Stb_Where.AppendLine(" AND Materie_Prime.Veg_Cod=" & Agro_SQL_SaveNum(Codice_Specie) & " ")
                End If
                If Codice_Prodotto <> 0 Then
                    Stb_Where.AppendLine(" AND Movimenti_Dettagli_Raccolta.Mat_Cod=" & Agro_SQL_SaveNum(Codice_Prodotto) & " ")
                End If

                If Flag_Join_Appezzamento Then
                    Stb_WhereRif.AppendLine(" AND (Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & ")  ")
                    Stb_WhereRif.AppendLine(" AND (Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & ") ")
                    'Stb_WhereRif.AppendLine(" -- AND	        Movimenti_RaccoltaImpianti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
                    'Stb_WhereRif.AppendLine(" -- AND	        Movimenti_RaccoltaImpianti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")

                    If Piva_Produttore <> "" Then
                        Stb_WhereRif.AppendLine("AND Reg_Impianti.Piva= '" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
                    End If
                    If Sa_Cod_Appezzamento <> 0 Then
                        Stb_WhereRif.AppendLine("AND Reg_Impianti.Sa_Cod= " & Agro_SQL_SaveNum(Sa_Cod_Appezzamento) & " ")
                    End If
                    If Appezza_Appezzamento <> 0 Then
                        Stb_WhereRif.AppendLine("AND Reg_Impianti.Appezza= " & Agro_SQL_SaveNum(Appezza_Appezzamento) & " ")
                    End If
                    If Id_Reg_Appezzamento <> 0 Then
                        Stb_WhereRif.AppendLine("AND Reg_Impianti.Id_Reg= " & Agro_SQL_SaveNum(Id_Reg_Appezzamento) & " ")
                    End If
                End If

                If Tipologia_Ritrovamento <> 0 Then
                    Stb_Where.AppendLine(" AND Movimenti_Dettagli_CE.Mat_Cod =" & Agro_SQL_SaveNum(Tipologia_Ritrovamento) & " ")
                End If

                If Indice_Pericolosita = 1 Then
                    Stb_Where.AppendLine(" AND Movimenti_Dettagli_CE.Cod_Progetto = 1")
                ElseIf Indice_Pericolosita = 2 Then
                    Stb_Where.AppendLine(" AND Movimenti_Dettagli_CE.Cod_Progetto = 0")
                End If

                If Sa_Cod <> 0 Then
                    Stb_Where.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                End If
                If modalitaScritturaConferimenti = "GIASLAN" AndAlso Fabbricato_Cod <> 0 Then
                    Stb_Where.AppendLine(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    Stb_Where.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------

                StrSQL.Append(Stb_Where)
                StrSQL.Append(Stb_WhereRif)

                'la union deve avere le stesse colonne della prima select e nello stesso ordine
                'quindi devo mettere la condizione sul not distinct
                If Not Flag_Distinct AndAlso Flag_Join_Appezzamento Then

                    StrSQL.AppendLine(" ) ")
                    StrSQL.AppendLine(" UNION ALL ")
                    StrSQL.AppendLine(" ( ")

                    Stb_Select.AppendLine(" , '' AS 'Nome Appezzamento', '01/01/1900' AS 'Data Inizio Impianto', 0 AS 'Superficie Impianto' ")
                    StrSQL.Append(Stb_Select)

                    'Stb_Join.AppendLine(" ")
                    StrSQL.Append(Stb_Join)

                    Stb_Where.AppendLine(" AND              ")
                    'MODIFICA DEL 04/04/2012: aggiunto il not exists
                    Stb_Where.AppendLine("       NOT EXISTS              ")
                    Stb_Where.AppendLine("      (              ")

                    'Stb_Where.AppendLine("                      (Agenda_Bolla.PIVA  ")
                    'Stb_Where.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda_Bolla.Sa_Cod)  ")
                    'Stb_Where.AppendLine("                     + '_' + CONVERT(varchar(10), Agenda_Bolla.Id_Agenda)  ")
                    'Stb_Where.AppendLine("                   NOT  IN (  ")
                    'Stb_Where.AppendLine("                              SELECT Mov_Dettagli_Riferimenti.Piva  ")
                    'Stb_Where.AppendLine("                              + '_' + CONVERT(varchar(10), Mov_Dettagli_Riferimenti.Sa_Cod)  ")
                    'Stb_Where.AppendLine("                              + '_' + CONVERT(varchar(10), Mov_Dettagli_Riferimenti.Id_Agenda)  ")
                    Stb_Where.AppendLine("                              SELECT  1            ")
                    Stb_Where.AppendLine("                              FROM Mov_Dettagli_Riferimenti ")
                    'l'accettazione sta in piva sa_cod lav_cod
                    Stb_Where.AppendLine("                              INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva = Agenda_Accettazione.PIVA  ")
                    Stb_Where.AppendLine("                              AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Accettazione.Sa_Cod  ")
                    Stb_Where.AppendLine("                              AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Accettazione.Id_Agenda  ")
                    'la raccolta sta in piva_rif sa_cod_rif lav_cod_rif
                    Stb_Where.AppendLine("                              INNER JOIN  Agenda Agenda_RaccoltaImpianti ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA  ")
                    Stb_Where.AppendLine("                                          AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod  ")
                    Stb_Where.AppendLine("                                          AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda  ")
                    Stb_Where.AppendLine("                              INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA  ")
                    Stb_Where.AppendLine("                                          AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda ")
                    Stb_Where.AppendLine("                              INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti ON Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov  ")
                    Stb_Where.AppendLine("                              INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  ")
                    Stb_Where.AppendLine("                                          AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  ")
                    Stb_Where.AppendLine("                              INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
                    Stb_Where.AppendLine("                                          AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
                    Stb_Where.AppendLine("                                          AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
                    Stb_Where.AppendLine("                                          AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione ")
                    Stb_Where.AppendLine("                              WHERE       (Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & ")  ")
                    Stb_Where.AppendLine("                              AND (Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & ") ")
                    'MODIFICA DEL 04/04/2012: aggiunta di queste tre clausole di join
                    Stb_Where.AppendLine("                              AND Agenda_Bolla.PIVA = Mov_Dettagli_Riferimenti.Piva ")
                    Stb_Where.AppendLine("                              AND Agenda_Bolla.Sa_Cod = Mov_Dettagli_Riferimenti.Sa_Cod ")
                    Stb_Where.AppendLine("                              AND Agenda_Bolla.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda ")
                    'Stb_Where.AppendLine("                          )  ")
                    'Stb_Where.AppendLine("                      )  ")
                    Stb_Where.AppendLine("      )  ")
                    StrSQL.Append(Stb_Where)
                    StrSQL.AppendLine(" ) ")

                End If

                StrSQL.AppendLine(" ) ")
            Next
            StrSQL.AppendLine(" ) ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY  " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                If Flag_Distinct Then
                    StrSQL.AppendLine(" ORDER BY Anno_Carico, Mese_carico, Giorno_Carico, Numero_Bolla")
                Else
                    StrSQL.AppendLine(" ORDER BY 'Numero Bolla', 'Corpo Estraneo'")
                End If
            End If

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

    '' -----------------------------------------------------------------------------
    ''' <summary>
    ''' recupera il codice più grande
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Ricava_Nuovo_Cod_CorpoEstraneo(ByVal Piva As String,
                                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.Ricava_Nuovo_Cod_CorpoEstraneo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim codCorpoEstraneo As Integer

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT ISNULL(MAX(Cod_CorpoEstraneo), 0) AS Cod_CorpoEstraneo ")
                    StrSQL.AppendLine(" FROM  CorpiEstranei ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")
                    'StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                codCorpoEstraneo = CInt(dt.Rows(0).Item("Cod_CorpoEstraneo")) + 1
            Else
                Throw New Exception("Non sono stati trovati dati.")
            End If

        Catch ex As Exception
            codCorpoEstraneo = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codCorpoEstraneo

    End Function

End Class
