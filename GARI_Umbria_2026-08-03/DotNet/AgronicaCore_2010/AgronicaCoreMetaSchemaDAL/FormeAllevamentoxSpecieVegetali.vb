Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class FormeAllevamentoxSpecieVegetali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Veg_Cod As Integer, _
                            ByVal Foral_Cod As Integer, _
                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.FormeAllevamentoxSpecieVegetali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Append(" SELECT Veg_Cod, Foral_Cod " + vbCrLf)
                    StrSQL.Append(" FROM  FormeAllevamentoxSpecieVegetali " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Foral_Cod <> 0 Then
                        StrSQL.Append(" AND Foral_Cod = " & Agro_SQL_SaveNum(Foral_Cod) + vbCrLf)
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
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
                    StrSQL.Append(" FROM  FormeAllevamentoxSpecieVegetali " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Foral_Cod <> 0 Then
                        StrSQL.Append(" AND Foral_Cod = " & Agro_SQL_SaveNum(Foral_Cod) + vbCrLf)
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
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

                    StrSQL.Append(" SELECT  SpecieVegetali.Veg_des, FormeAllevamentoxSpecieVegetali.Veg_Cod,  FormeAllevamento.* ")
                    StrSQL.Append(" FROM        FormeAllevamento ")
                    StrSQL.Append(" INNER JOIN    FormeAllevamentoxSpecieVegetali ON FormeAllevamentoxSpecieVegetali.Foral_Cod = FormeAllevamento.Foral_Cod ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = FormeAllevamentoxSpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE FormeAllevamento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   FormeAllevamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Foral_Cod <> 0 Then
                        StrSQL.Append(" AND FormeAllevamentoxSpecieVegetali.Foral_Cod = " & Agro_SQL_SaveNum(Foral_Cod) + vbCrLf)
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND FormeAllevamentoxSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

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
    Public Function Esiste_VegCodXForalCod(ByVal Veg_Cod As Integer, _
                                            ByVal Foral_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.FormeAllevamentoxSpecieVegetali_R.Esiste_VegCodXForalCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risp As Boolean = False

        Try

            DT = Leggi(Veg_Cod, _
                        Foral_Cod, _
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                risp = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function



    '##############################################################################################
    Public Function LeggiFormeAllevamento(ByVal Veg_Cod As Integer, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.FormeAllevamentoxSpecieVegetali_R.LeggiForme()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT F.Foral_Cod, F.Foral_Des " + vbCrLf)
            StrSQL.Append(" FROM  FormeAllevamento F " + vbCrLf)
            StrSQL.Append(" inner join FormeAllevamentoxSpecieVegetali FA on F.foral_cod = FA.foral_cod  " + vbCrLf)
            StrSQL.Append(" WHERE 1=1")

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND FA.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
            End If
            StrSQL.Append(" order by F.Foral_Des  " + vbCrLf)
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


Public Class FormeAllevamento
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Foral_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.FormeAllevamento.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0



            StrSQL.Append(" SELECT * " + vbCrLf)
            StrSQL.Append(" FROM  FormeAllevamento " + vbCrLf)
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

            If Foral_Cod <> 0 Then
                StrSQL.Append(" AND Foral_Cod = " & Agro_SQL_SaveNum(Foral_Cod) + vbCrLf)
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
    Public Function Descrizione(ByVal Foral_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.FormeAllevamentoDescrizione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risp As String = ""

        Try

            DT = Leggi(Foral_Cod, _
                                                "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count = 1 Then
                risp = DT.Rows(0).Item("Foral_Des")
            Else
                Throw New Exception("Foral_Cod non esiste")
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function




End Class