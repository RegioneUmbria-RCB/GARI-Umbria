
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class InsettiUtili_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Usare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Ins_Cod"></param>
    ''' <param name="FinestraTemp_Inizio"></param>
    ''' <param name="FinestraTemp_Fine"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Ins_Cod As Integer,
                          ByVal FinestraTemp_Inizio As Date,
                          ByVal FinestraTemp_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.Leggi()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  InsettiUtili ")
                    StrSQL.AppendLine(" WHERE InsettiUtili.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    StrSQL.AppendLine(" AND   InsettiUtili.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


                    If Ins_Cod <> 0 Then
                        StrSQL.AppendLine(" AND InsettiUtili.INS_COD =  " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY InsettiUtili.INS_DES ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.AppendLine(" SELECT distinct InsettiUtili.* ")
                    StrSQL.AppendLine(" FROM  InsettiUtili inner join InsettiUtilixAvversita ")
                    StrSQL.AppendLine(" on InsettiUtili.INS_COD = InsettiUtilixAvversita.INS_COD ")
                    StrSQL.AppendLine(" inner join AvversitaxGruppoAvversita ")
                    StrSQL.AppendLine(" on AvversitaxGruppoAvversita.av_cod = InsettiUtilixAvversita.av_cod ")

                    StrSQL.AppendLine(" WHERE InsettiUtili.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
                    StrSQL.AppendLine(" AND   InsettiUtili.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


                    If Ins_Cod <> 0 Then
                        StrSQL.AppendLine(" AND InsettiUtili.INS_COD =  " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY InsettiUtili.INS_DES ASC ")
                    End If


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

    Public Function LeggiXAvversitaXSpecie(ByVal Ins_Cod As Integer,
                                           ByVal COD As Int32,
                                           ByVal VEG_COD As Int32,
                                           ByVal AV_COD As Int32,
                                           ByVal FinestraTemp_Inizio As Date,
                                           ByVal FinestraTemp_Fine As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.LeggiXAvversitaXSpecie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT DISTINCT Avversita.*, SpecieVegetali.* ")
            StrSQL.AppendLine(" FROM SpecieVegetalixAvversita ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetalixAvversita.VEG_COD = SpecieVegetali.VEG_COD  ")
            StrSQL.AppendLine(" INNER JOIN Avversita ON specieVegetalixAvversita.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" INNER JOIN InsettiUtilixAvversita ON Avversita.av_cod = InsettiUtilixAvversita.av_cod")

            StrSQL.AppendLine(" INNER JOIN InsettiUtili ON InsettiUtili.INS_COD = InsettiUtilixAvversita.INS_COD ")

            StrSQL.AppendLine(" INNER JOIN AvversitaxGruppoAvversita ON AvversitaxGruppoAvversita.av_cod = InsettiUtilixAvversita.av_cod")

            StrSQL.AppendLine(" WHERE InsettiUtili.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.AppendLine(" AND InsettiUtili.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND Avversita.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND Avversita.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = SpecieVegetali.VEG_COD")

            If Ins_Cod <> 0 Then
                StrSQL.AppendLine(" AND InsettiUtili.INS_COD = " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
            End If

            If COD <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetalixAvversita.COD = " & Agro_SQL_SaveNum(COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If AV_COD <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetalixAvversita.AV_COD = " & Agro_SQL_SaveNum(AV_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND dbo.Avversita.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND dbo.Avversita.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Avversita.AV_DES_VOL ASC , SpecieVegetali.VEG_DES ASC ")
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

    Public Function LeggiXGruAvversitaXSpecie(
                         ByVal Ins_Cod As Integer,
                         ByVal COD As Int32,
                         ByVal VEG_COD As Int32,
                         ByVal GRU_COD As Int32,
                         ByVal FinestraTemp_Inizio As Date,
                         ByVal FinestraTemp_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.LeggiXAvversitaXSpecie()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT DISTINCT GruppoAvversita.*, SpecieVegetali.* ")

            StrSQL.AppendLine(" FROM SpecieVegetalixAvversita ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetalixAvversita.VEG_COD = SpecieVegetali.VEG_COD ")

            StrSQL.AppendLine(" INNER JOIN Avversita ON specieVegetalixAvversita.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" INNER JOIN AvversitaxGruppoAvversita ON AvversitaxGruppoAvversita.av_cod = Avversita.av_cod ")

            StrSQL.AppendLine(" INNER JOIN GruppoAvversita ON GruppoAvversita.AV_GRU = AvversitaxGruppoAvversita.AV_GRU ")

            StrSQL.AppendLine(" INNER JOIN InsettiUtilixAvversita ON Avversita.av_cod = InsettiUtilixAvversita.av_cod")

            StrSQL.AppendLine(" INNER JOIN InsettiUtili ON InsettiUtili.INS_COD = InsettiUtilixAvversita.INS_COD ")

            StrSQL.AppendLine(" WHERE InsettiUtili.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.AppendLine(" AND InsettiUtili.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND Avversita.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine))
            StrSQL.AppendLine(" AND Avversita.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = SpecieVegetali.VEG_COD")

            If Ins_Cod <> 0 Then
                StrSQL.AppendLine(" AND InsettiUtili.INS_COD = " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
            End If

            If COD <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetalixAvversita.COD = " & Agro_SQL_SaveNum(COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If GRU_COD <> 0 Then
                StrSQL.AppendLine(" AND GruppoAvversita.AV_GRU = " & Agro_SQL_SaveNum(GRU_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND dbo.Avversita.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND dbo.Avversita.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY GruppoAvversita.AV_GRU_DES ASC, SpecieVegetali.VEG_DES ASC ")
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

    Public Function LeggiSenzaAversitaCollegate(ByVal Ins_Cod As Integer,
                                                ByVal FinestraTemp_Inizio As Date,
                                                ByVal FinestraTemp_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.Leggi()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try





            StrSQL.AppendLine(" SELECT distinct InsettiUtili.* ")
            StrSQL.AppendLine(" FROM  InsettiUtili where InsettiUtili.INS_COD not in ( ")
            StrSQL.AppendLine("SELECT InsettiUtilixAvversita.INS_COD from InsettiUtilixAvversita ")
            StrSQL.AppendLine(" ) ")


            StrSQL.AppendLine(" and InsettiUtili.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.AppendLine(" AND   InsettiUtili.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


            If Ins_Cod <> 0 Then
                StrSQL.AppendLine(" AND InsettiUtili.INS_COD =  " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY InsettiUtili.INS_DES ASC ")
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

    Public Function InsDes_from_InsCod(ByVal InsCod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        'Recupero le informazioni		
        Dt = Leggi(CInt(InsCod), objParametri.FinestraTemporaleInizio, objParametri.FinestraTemporaleFine, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Ins_Des")

        End If

        Return ""

    End Function

    Public Function LeggInsettiUtilixPrincipiAttivi(Ins_Cod As Integer,
                                                    Pa_Cod As Integer,
                                                    xFiltroAggiuntivo As String,
                                                    xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.LeggInsettiUtilixPrincipiAttivi()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT *   ")
            StrSQL.AppendLine(" FROM  InsettiUtilixPrincipiAttivi ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Ins_Cod <> 0 Then
                StrSQL.AppendLine(" AND Ins_Cod = " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Cod = " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiInsettiNG(Ins_Cod As Integer,
                                   joinAvversita As Boolean,
                                   isImpollinatore As Boolean,
                                   xFiltroAggiuntivo As String,
                                   xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InsettiUtili_R.LeggiInsettiNG()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT DISTINCT InsettiUtili.* ")
            StrSQL.AppendLine(" FROM  InsettiUtili ")

            If joinAvversita Then
                StrSQL.AppendLine(" INNER JOIN InsettiUtilixAvversita ")
                StrSQL.AppendLine(" ON InsettiUtili.INS_COD = InsettiUtilixAvversita.INS_COD ")
                StrSQL.AppendLine(" INNER JOIN AvversitaxGruppoAvversita ")
                StrSQL.AppendLine(" ON AvversitaxGruppoAvversita.av_cod = InsettiUtilixAvversita.av_cod ")
            End If

            StrSQL.AppendLine(" WHERE 1 = 1")

            If Ins_Cod <> 0 Then
                StrSQL.AppendLine(" AND InsettiUtili.INS_COD =  " & Agro_SQL_SaveNum(Ins_Cod) & "  ")
            End If

            If isImpollinatore Then
                StrSQL.AppendLine(" AND InsettiUtili.INS_COD NOT IN (SELECT Ins_Cod FROM InsettiUtilixPrincipiAttivi) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     dbo.InsettiUtili.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY InsettiUtili.INS_DES ASC ")
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
End Class
