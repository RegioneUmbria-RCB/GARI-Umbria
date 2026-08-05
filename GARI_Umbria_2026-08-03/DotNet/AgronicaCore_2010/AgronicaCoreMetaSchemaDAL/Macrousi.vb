Imports System.Web
Imports System.Web.Caching
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

<CachedDataProviderAttribute("Macrousi_R")>
Public Class Macrousi_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    <Cacheable(True)>
    Public Function Leggi(ByVal Macrouso_Cod As String,
                          ByVal Macrouso_Des As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Macrousi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Macrousi ")
                    StrSQL.Append(" WHERE 1 = 1 ")

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Macrouso_Des <> "" Then
                        StrSQL.Append(" AND Macrouso_Des = '" & Agro_SQL_SaveText(Macrouso_Des) & "' ")
                    End If


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
                        StrSQL.Append(" ORDER BY Macrouso_Des ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Macrousi ")
                    StrSQL.Append(" WHERE 1 = 1 ")

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Macrouso_Des <> "" Then
                        StrSQL.Append(" AND Macrouso_Des = '" & Agro_SQL_SaveText(Macrouso_Des) & "' ")
                    End If


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
                        StrSQL.Append(" ORDER BY Macrouso_Des ASC ")
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

    <Cacheable(True)>
    Public Function Leggi_MacrousoDes_from_MacrousoCod(ByVal Macrouso_Cod As String,
                                                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Macrousi_R.Leggi_MacrousoDes_from_MacrousoCod()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim Macrouso_Des As String = ""

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Macrousi ")
                    StrSQL.Append(" WHERE 1 = 1 ")

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If



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
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY Macrouso_Des ASC ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Macrousi ")
                    StrSQL.Append(" WHERE 1 = 1 ")

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If



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
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY Macrouso_Des ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Macrouso_Des = dt.Rows(0).Item("Macrouso_Des")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Macrouso_Des

    End Function

End Class
