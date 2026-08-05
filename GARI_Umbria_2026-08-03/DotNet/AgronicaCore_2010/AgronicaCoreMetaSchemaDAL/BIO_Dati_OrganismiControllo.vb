Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class BIO_Dati_OrganismiControllo_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Organismo_Cod As Integer, _
                            ByVal Organismo_Sigla As String, _
                            ByVal Codice As String, _
                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_Dati_OrganismiControllo_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Append(" SELECT Organismo_Cod, Organismo_Sigla, Organismo_Des ")
                    StrSQL.Append(" FROM  BIO_Dati_OrganismiControllo " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Organismo_Cod <> 0 Then
                        StrSQL.Append(" AND Organismo_Cod = " & Agro_SQL_SaveNum(Organismo_Cod) + vbCrLf)
                    End If

                    If Organismo_Sigla <> "" Then
                        StrSQL.Append(" AND Organismo_Sigla = '" & Agro_SQL_SaveText(Organismo_Sigla) + "'" + vbCrLf)
                    End If

                    If Codice <> "" Then
                        StrSQL.Append(" AND Codice = '" & Agro_SQL_SaveText(Codice) + "'" + vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  BIO_Dati_OrganismiControllo " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Organismo_Cod <> 0 Then
                        StrSQL.Append(" AND Organismo_Cod = " & Agro_SQL_SaveNum(Organismo_Cod) + vbCrLf)
                    End If

                    If Organismo_Sigla <> "" Then
                        StrSQL.Append(" AND Organismo_Sigla = '" & Agro_SQL_SaveText(Organismo_Sigla) + "'" + vbCrLf)
                    End If

                    If Codice <> "" Then
                        StrSQL.Append(" AND Codice = '" & Agro_SQL_SaveText(Codice) + "'" + vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_JoinCompleta


                    '=====================================================================

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


    '###################################################################################
    Public Function OrganismoCodiceSiglaDes_from_OrganismoCod(ByVal Organismo_Cod As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_Dati_OrganismiControllo_R.OrganismoDes_from_OrganismoCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim OrganismoDes As String = ""

        Try

            DT = Leggi(Organismo_Cod, _
                            "", _
                            "", _
                         enumSelezioneVariabile.Selezione_TabellaCompleta, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                OrganismoDes = DT.Rows(0).Item("Codice") + " - " + DT.Rows(0).Item("Organismo_Des") + " (" + DT.Rows(0).Item("Organismo_Sigla") + ")"
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return OrganismoDes

    End Function


    '###################################################################################
    Public Function OrganismoDes_from_OrganismoCod(ByVal Organismo_Cod As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_Dati_OrganismiControllo_R.OrganismoDes_from_OrganismoCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim OrganismoDes As String = ""

        Try

            DT = Leggi(Organismo_Cod, _
                            "", _
                            "", _
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                OrganismoDes = DT.Rows(0).Item("Organismo_Des")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return OrganismoDes

    End Function

    '###################################################################################
    Public Function OrganismoDes_from_OrganismoSigla(ByVal Organismo_Sigla As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_Dati_OrganismiControllo_R.OrganismoDes_from_OrganismoSigla()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim OrganismoDes As String = ""

        Try

            DT = Leggi(0, _
                           Organismo_Sigla, _
                           "", _
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                OrganismoDes = DT.Rows(0).Item("Organismo_Des")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return OrganismoDes

    End Function

    '###################################################################################
    Public Function OrganismoDes_from_Codice(ByVal Codice As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.BIO_Dati_OrganismiControllo_R.OrganismoDes_from_Codice()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim OrganismoDes As String = ""

        Try

            DT = Leggi(0, _
                           "", _
                           Codice, _
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                OrganismoDes = DT.Rows(0).Item("Organismo_Des")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return OrganismoDes

    End Function

End Class
