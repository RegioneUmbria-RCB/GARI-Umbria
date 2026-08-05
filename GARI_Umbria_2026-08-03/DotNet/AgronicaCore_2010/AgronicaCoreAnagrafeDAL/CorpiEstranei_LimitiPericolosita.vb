Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CorpiEstranei_LimitiPericolosita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID As Integer,
                           ByVal Piva As String,
                           ByVal Cod_CorpoEstraneo As Integer,
                           ByVal Cod_Pericolosita As Integer,
                           ByVal Desc_Pericolosita As String,
                           ByVal LimiteMax_Aeroseparatori As Integer,
                           ByVal LimiteMax_CernitriciOttiche As Integer,
                           ByVal LimiteMax_CernitaManuale As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CorpiEstranei_LimitiPericolosita ")
            StrSQL.Append("             (ID, Piva_SuperUser,    Piva,   ")
            StrSQL.Append("               Cod_CorpoEstraneo, Cod_Pericolosita, Desc_Pericolosita, ")
            StrSQL.Append("               LimiteMax_Aeroseparatori, LimiteMax_CernitriciOttiche, LimiteMax_CernitaManuale, ")

            StrSQL.Append("              Inviato,            DataInvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("         " & Agro_SQL_SaveNum(ID) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Pericolosita) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Desc_Pericolosita) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LimiteMax_Aeroseparatori) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LimiteMax_CernitriciOttiche) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LimiteMax_CernitaManuale) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

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


    Public Function Modifica(ByVal Piva As String,
                             ByVal Cod_CorpoEstraneo As Integer,
                             ByVal Cod_Pericolosita As Integer,
                             ByVal LimiteMax_Aeroseparatori As Integer,
                             ByVal LimiteMax_CernitriciOttiche As Integer,
                             ByVal LimiteMax_CernitaManuale As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_W.Modifica()"

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

            If Cod_Pericolosita = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Pericolosita obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Append("UPDATE CorpiEstranei_LimitiPericolosita SET ")
            StrSQL.Append("    LimiteMax_Aeroseparatori       = " & Agro_SQL_SaveNum(LimiteMax_Aeroseparatori))
            StrSQL.Append("   ,LimiteMax_CernitriciOttiche           = " & Agro_SQL_SaveNum(LimiteMax_CernitriciOttiche))
            StrSQL.Append("   ,LimiteMax_CernitaManuale               = " & Agro_SQL_SaveNum(LimiteMax_CernitaManuale))
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND     Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
            StrSQL.Append(" AND     Cod_Pericolosita = " & Agro_SQL_SaveNum(Cod_Pericolosita) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function Cancella_from_ChiaveCE(ByVal Piva As String,
                                           ByVal Cod_CorpoEstraneo As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_W.Cancella_from_ChiaveCE()"

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
                StrSQL.Append(" UPDATE CorpiEstranei_LimitiPericolosita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND     Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
                StrSQL.Append(" AND     Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    CorpiEstranei_LimitiPericolosita ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND     Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


Public Class CorpiEstranei_LimitiPericolosita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '' -----------------------------------------------------------------------------
    ''' <summary>
    ''' recupera il codice più grande
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Ricava_Nuovo_ID(ByVal Piva As String,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_R.Ricava_Nuovo_ID()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim id As Integer

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT ISNULL(MAX(ID), 0) AS ID ")
                    StrSQL.Append(" FROM  CorpiEstranei_LimitiPericolosita ")
                    StrSQL.Append(" WHERE 1 = 1 ")

                    'StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    'If Piva <> "" Then
                    '    StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
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
                id = CInt(dt.Rows(0).Item("ID")) + 1
            Else
                Throw New Exception("Non sono stati trovati dati.")
            End If

        Catch ex As Exception
            id = -1
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return id

    End Function


    ''##############################################################################################
    'Public Function Leggi(ByVal Cod_CorpoEstraneo As Integer, _
    '                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CorpiEstranei_R.Leggi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT Cod_CorpoEstraneo , Desc_CorpoEstraneo")
    '                StrSQL.Append(" FROM  CorpiEstranei ")
    '                StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                If Cod_CorpoEstraneo <> 0 Then
    '                    StrSQL.Append(" AND Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
    '                End If

    '                '--------------------------------------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   Inviato >=0 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   Inviato =-1 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                Else
    '                    StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
    '                End If
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT * ")
    '                StrSQL.Append(" FROM  CorpiEstranei ")
    '                StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                If Cod_CorpoEstraneo <> 0 Then
    '                    StrSQL.Append(" AND Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
    '                End If

    '                '--------------------------------------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   Inviato >=0 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   Inviato =-1 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                Else
    '                    StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
    '                End If

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT CorpiEstranei.Cod_CorpoEstraneo, CorpiEstranei.Desc_CorpoEstraneo, CorpiEstranei_LimitiPericolosita.LimiteMax_Aeroseparatori,  ")
    '                StrSQL.Append(" CorpiEstranei_LimitiPericolosita.LimiteMax_CernitriciOttiche, CorpiEstranei_LimitiPericolosita.LimiteMax_CernitaManuale  ")
    '                StrSQL.Append(" FROM         CorpiEstranei INNER JOIN ")
    '                StrSQL.Append(" CorpiEstranei_LimitiPericolosita ON CorpiEstranei.Cod_CorpoEstraneo = CorpiEstranei_LimitiPericolosita.Cod_CorpoEstraneo ")
    '                StrSQL.Append(" WHERE CorpiEstranei.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                StrSQL.Append(" AND   CorpiEstranei.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                If Cod_CorpoEstraneo <> 0 Then
    '                    StrSQL.Append(" AND CorpiEstranei.Cod_CorpoEstraneo = " & Agro_SQL_SaveNum(Cod_CorpoEstraneo) & " ")
    '                End If

    '                '--------------------------------------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   CorpiEstranei.Inviato >=0 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   CorpiEstranei.Inviato =-1 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                Else
    '                    StrSQL.Append(" ORDER BY CorpiEstranei.Desc_CorpoEstraneo")
    '                End If


    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
    '                '
    '                '
    '                '
    '                '

    '        End Select

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

End Class