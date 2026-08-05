Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Widgets_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function EsisteTabella(ByVal nomeTabella As String, ByVal objParametri As AgronicaCoreParametri) As Boolean


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Widgets_R.EsisteTabella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(String.Format("select count(1) from sys.views tt where tt.name = '{0}'", nomeTabella))


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows.Count > 0


    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Codice As String,
                          ByVal Titolo As String,
                          ByVal Descrizione As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Visibile As Boolean? = Nothing,
                          Optional ByVal Abilitato As Boolean? = Nothing,
                          Optional ByVal presetIniziale As Boolean? = Nothing,
                          Optional ByVal IdWidget As Integer? = Nothing
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Widgets_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////
                    StrSQL.Length = 0
                    StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Widgets ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Not String.IsNullOrEmpty(Codice) Then
                        StrSQL.Append(" AND Codice LIKE '" & Agro_SQL_SaveText(Codice) & "%' ")
                    End If

                    If Not String.IsNullOrEmpty(Titolo) Then
                        StrSQL.Append(" AND Titolo LIKE '" & Agro_SQL_SaveText(Titolo) & "%' ")
                    End If

                    If Not String.IsNullOrEmpty(Descrizione) Then
                        StrSQL.Append(" AND Descrizione LIKE '" & Agro_SQL_SaveText(Descrizione) & "%' ")
                    End If

                    If Not IsNothing(Visibile) AndAlso Visibile.HasValue Then
                        StrSQL.Append(" AND Visibile = " & Convert.ToInt32(Visibile.Value))
                    End If

                    If Not IsNothing(Abilitato) AndAlso Abilitato.HasValue Then
                        StrSQL.Append(" AND Abilitato = " & Convert.ToInt32(Abilitato))
                    End If

                    If Not IsNothing(presetIniziale) AndAlso presetIniziale.HasValue Then
                        StrSQL.Append(" AND PresetIniziale = " & Convert.ToInt32(presetIniziale))
                    End If

                    If Not IsNothing(IdWidget) AndAlso IdWidget.HasValue Then
                        StrSQL.Append(" AND IdWidget = " & Agro_SQL_SaveNum(IdWidget))
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
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
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Descrizione ASC")
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

