
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class FormulatixClassifica_R

    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal FR_COD As Int32,
                          ByVal CLASS_COD As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " &
                                  " FROM  FormulatixClassificazioni , Formulati , ClassificazioniFormulati " &
                                  " WHERE FormulatixClassificazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   FormulatixClassificazioni.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   ClassificazioniFormulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   ClassificazioniFormulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   FormulatixClassificazioni.FOR_Cod = Formulati.FR_COD " &
                                  " AND   FormulatixClassificazioni.CLASS_COD = ClassificazioniFormulati.CLASS_COD " &
                                  " AND   Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")


                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixClassificazioni.FOR_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If CLASS_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixClassificazioni.CLASS_COD =  " & Agro_SQL_SaveNum(CLASS_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC, ClassificazioniFormulati.CLASS_DES ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_Revocati_Sospesi(ByVal FR_COD As Int32,
                                             ByVal CLASS_COD As Int32,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.Leggi_Revocati_Sospesi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append("  SELECT * " &
                                  " FROM  FormulatixClassificazioni , Formulati , ClassificazioniFormulati " &
                                  " WHERE FormulatixClassificazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   FormulatixClassificazioni.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   ClassificazioniFormulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   ClassificazioniFormulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                  " AND   FormulatixClassificazioni.FOR_Cod = Formulati.FR_COD " &
                                  " AND   FormulatixClassificazioni.CLASS_COD = ClassificazioniFormulati.CLASS_COD " &
                                  " AND  (Abs(Formulati.Revocato) <> 0  OR Abs(Formulati.Sospeso) <> 0 ) " &
                                  " AND   Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")


                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixClassificazioni.FOR_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If CLASS_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixClassificazioni.CLASS_COD =  " & Agro_SQL_SaveNum(CLASS_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC, ClassificazioniFormulati.CLASS_DES ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Function LeggiXClassificazione(ByVal TestoRicerca As String,
                                             ByVal TipoRichiesto As Int32,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.LeggiXClassificazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Dt_App As New DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.NEWCLTOSS_COD, " &
                                  " Formulati.Revocato, Formulati.Data_Revo, Formulati.Sospeso, " &
                                  " Formulati.Data_Sosp, Formulati.Termine, Formulati.Data_Term, " &
                                  " Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " &
                                  " FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des ")

                    StrSQL.Append(" FROM FormulatixClassificazioni" &
                                    " INNER JOIN Formulati ON FormulatixClassificazioni.FOR_COD = Formulati.FR_COD" &
                                    " INNER JOIN ClassificazioniFormulati ON FormulatixClassificazioni.CLASS_COD = ClassificazioniFormulati.CLASS_COD " &
                                    " WHERE   Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%' " &
                                    " And     Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")


                    Select Case TipoRichiesto

                        Case 0  'Tutti i formulati

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607, 606, 500,501,502,503,504,505,506,602,613,617,1003) ")

                            '=================================================================

                        Case 1  'Trattamento Antiparassitario

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,606) ")

                            '=================================================================

                        Case 2  'Diserbo

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) ")

                            '=================================================================

                        Case 3  'Trattamenti Fitoregolatori

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")


                        Case 4  'Coadiuvanti, Bagnanti, Antischiuma

                            StrSQL.Append("  And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  ")

                        Case 5  'Concia

                            StrSQL.Append("  And  ClassificazioniFormulati.CLASS_COD IN (608)  ")

                        Case 6  'Disseccamento

                            StrSQL.Append("  And  ClassificazioniFormulati.CLASS_COD IN (603)  ")

                        Case 7  'Geodisinfestazione

                            StrSQL.Append("  And  ClassificazioniFormulati.CLASS_COD IN (607)  ")

                        Case 8  'Trattamenti Antiparassitari + Concianti

                            StrSQL.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,608,606) ")

                        Case 9  'Diserbo + Disseccanti

                            StrSQL.Append("  And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603) ")

                        Case 10  'Trattamenti Antiparassitari + Geodisinfestanti

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,607,606) ")

                    End Select

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FR_DES")
                    End If

            End Select

            '--------------------------------------------------------------------------
            Dt_App = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
            DT = objFiltro.Filtra_Formulati(Dt_App, Validita_Fine, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function LeggiXClassificazione_PA_OLD(ByVal TestoRicerca As String,
                                                 ByVal TipoRichiesto As Int32,
                                                 ByVal VEG_COD As Int32,
                                                 ByVal strPA As String,
                                                 ByVal Validita_Inizio As Date,
                                                 ByVal Validita_Fine As Date,
                                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.LeggiXClassificazione_PA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim ClassificazioneMin As Int32
        Dim ClassificazioneMax As Int32


        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.NEWCLTOSS_COD, FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, " &
                                  "                 Formulati.Revocato, Formulati.Data_Revo, Formulati.Sospeso, Formulati.Data_Sosp, Formulati.Termine, Formulati.Data_Term, " &
                                  "                 Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, " &
                                  "                (SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Cod " &
                                  "                  FROM     dbo.FormulatixPrincipiAttivi " &
                                  "                  INNER JOIN dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " &
                                  "                  Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " &
                                  "                  ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Cod, " &
                                  "                (SELECT  TOP 1 dbo.PrincipiAttivi.Pa_Des " &
                                  "                  FROM     dbo.FormulatixPrincipiAttivi " &
                                  "                  INNER JOIN dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " &
                                  "                  Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " &
                                  "                  ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Pa_Des, " &
                                  "                (SELECT  TOP 1 dbo.FormulatixPrincipiAttivi.Titolo " &
                                  "                  FROM     dbo.FormulatixPrincipiAttivi " &
                                  "                  INNER JOIN dbo.PrincipiAttivi ON dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod " &
                                  "                  Where   (dbo.FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD) " &
                                  "                  ORDER BY dbo.FormulatixPrincipiAttivi.Titolo DESC) as Titolo " &
                                  " FROM     PrincipiAttivi, FormulatixPrincipiAttivi, FormulatixClassificazioni, ClassificazioniFormulati, FormulatixSpecieVegetali, Formulati " &
                                  " Where    Formulati.FR_COD = FormulatixPrincipiAttivi.FR_COD  " &
                                  " And      PrincipiAttivi.PA_COD = FormulatixPrincipiAttivi.PA_COD " &
                                  " And      FormulatixClassificazioni.FOR_COD = Formulati.FR_COD  " &
                                  " And      FormulatixClassificazioni.CLASS_COD = ClassificazioniFormulati.CLASS_COD " &
                                  " And      FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD  " &
                                  " And     (Formulati.FR_DES LIKE '%" & TestoRicerca & "%') " &
                                  " And      Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")


                    Select Case TipoRichiesto

                        Case enum_TipoFormulato.Tutti  'Tutti i formulati

                            ClassificazioneMin = 0
                            ClassificazioneMax = 99999

                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607,  500,501,502,503,504,505,506,606,614,615,602,613,617,1003) ")

                            'sSql = sSql & " And FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "

                            '=================================================================

                        Case enum_TipoFormulato.Antiparassitari  'Trattamento Antiparassitario

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '
                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) ")


                            '=================================================================


                        Case enum_TipoFormulato.Diserbanti  'Diserbo

                            ClassificazioneMin = 200
                            ClassificazioneMax = 299

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203) ")

                            '=================================================================

                        Case enum_TipoFormulato.Fitoregolatori  'Trattamenti Fitoregolatori

                            ClassificazioneMin = 400
                            ClassificazioneMax = 499

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")


                        Case enum_TipoFormulato.Coadiuvanti  'Coadiuvanti, Bagnanti, Antischiuma

                            ClassificazioneMin = 500
                            ClassificazioneMax = 506

                            StrSQL.Append(" And  (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  ")

                        Case enum_TipoFormulato.Concianti  'Concia

                            '               Classificazione Concianti = 608

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN (608)  ")

                        Case enum_TipoFormulato.Disseccanti  'Disseccamento

                            '               Classificazione Disseccanti = 603

                            'Sql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN (603)  ")

                        Case enum_TipoFormulato.Geodisinfestanti  'Geodisinfestazione

                            '               Classificazione Geodisinfestanti = 607

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN (607)  ")

                        Case enum_TipoFormulato.Antiparassitari_Concianti  'Trattamenti Antiparassitari + Concianti

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '                + Classificazione Concianti = 608

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                        Case enum_TipoFormulato.Diserbanti_Disseccanti 'Diserbo + Disseccanti

                            '                ClassificazioneMin = 200
                            '                ClassificazioneMax = 299
                            '                + Classificazione Disseccanti = 603

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (200,201,202,203,603) ")

                        Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti  'Trattamenti Antiparassitari + Geodisinfestanti

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '                + Classificazione Geodisinfestanti = 607

                            'sSql = sSql & " And  FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.Append(" And  ClassificazioniFormulati.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

                    End Select

                    'If VEG_COD <> 0 Then
                    '   sSql = sSql & " AND  FormulatixSpecieVegetali.Veg_Cod = " & VEG_COD & "  "
                    'End If

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND ( FormulatixSpecieVegetali.Veg_Cod = " & VEG_COD & "  " &
                                      " OR  ( FormulatixSpecieVegetali.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " &
                                       "      Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & VEG_COD & " ) ) ) ")
                    End If


                    If Trim(strPA) <> "" Then
                        StrSQL.Append(" AND  PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA) & "  " &
                                      " AND  Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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

    Public Function LeggiXClassificazione_PA(ByVal TestoRicerca As String,
                                             ByVal TipoRichiesto As Integer,
                                             ByVal VEG_COD As Integer,
                                             ByVal strPA As String,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional Fr_Cod As Integer = 0,
                                                Optional ByVal parametrizza As Boolean = True
                                             ) As DataTable

        'NOTA
        'Il TipoRichiesto consente di selezionare solo i formulati specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i formulati
        '   1 = Trattamenti Antiparassitari
        '   2 = Diserbo
        '   3 = Trattamenti Fitoregolatori
        '   4 = Coadiuvanti, bagnanti, etc
        '   5 = Concianti
        '   6 = Disseccanti
        '   7 = Geodisinfestanti
        '   8 = Trattamenti Antiparassitari + Concianti
        '   9 = Diserbo + Disseccanti
        '  10 = Trattamenti Antiparassitari + Geodisinfestanti

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.LeggiXClassificazione_PA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim ClassificazioneMin As Int32
        Dim ClassificazioneMax As Int32


        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    'StrSQL.AppendLine(" ;WITH CTE_FormulatixPrincipiAttivi AS (")
                    StrSQL.AppendLine(" SELECT DISTINCT dbo.FormulatixPrincipiAttivi.FR_COD, PrincipiAttivi.Pa_Cod, PrincipiAttivi.Pa_Des, FormulatixPrincipiAttivi.Titolo ")
                    StrSQL.AppendLine(" INTO #CTE_FormulatixPrincipiAttivi ")
                    StrSQL.AppendLine(" FROM dbo.FormulatixPrincipiAttivi")
                    StrSQL.AppendLine(" INNER JOIN dbo.PrincipiAttivi ON ")
                    StrSQL.AppendLine("    dbo.FormulatixPrincipiAttivi.Pa_Cod = dbo.PrincipiAttivi.Pa_Cod  ")
                    StrSQL.AppendLine(" WHERE 1 = 1 ")
                    If Trim(strPA) <> "" Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA, False, parametrizza) & "  ")
                    End If
                    If Fr_Cod <> 0 Then
                        StrSQL.AppendLine(" AND FormulatixPrincipiAttivi.Fr_Cod = " & Agro_SQL_SaveNum(Fr_Cod) & " ")
                    End If

                    'StrSQL.AppendLine(") ")
                    StrSQL.AppendLine(" SELECT DISTINCT Formulati.FR_COD, Formulati.FR_DES, Formulati.NEWCLTOSS_COD, FormulatixClassificazioni.Class_Cod, ClassificazioniFormulati.Class_Des, ")
                    StrSQL.AppendLine("                 Formulati.Revocato, Formulati.Data_Revo, Formulati.Sospeso, Formulati.Data_Sosp, Formulati.Termine, Formulati.Data_Term, ")
                    StrSQL.AppendLine("                 Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, ")
                    StrSQL.AppendLine("                 CTE_FormulatixPrincipiAttivi.Pa_Cod, ")
                    StrSQL.AppendLine("                 CTE_FormulatixPrincipiAttivi.Pa_Des, ")
                    StrSQL.AppendLine("                 CTE_FormulatixPrincipiAttivi.Titolo ")
                    StrSQL.AppendLine(" FROM Formulati ")
                    StrSQL.AppendLine(" JOIN FormulatixSpecieVegetali ON FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD ")
                    StrSQL.AppendLine(" JOIN FormulatixClassificazioni ON FormulatixClassificazioni.FOR_COD = Formulati.FR_COD  ")
                    StrSQL.AppendLine(" JOIN ClassificazioniFormulati ON FormulatixClassificazioni.CLASS_COD = ClassificazioniFormulati.CLASS_COD ")
                    StrSQL.AppendLine(" JOIN #CTE_FormulatixPrincipiAttivi CTE_FormulatixPrincipiAttivi on CTE_FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD ")

                    StrSQL.AppendLine(" WHERE 1 = 1")

                    If TestoRicerca <> "" Then
                        StrSQL.AppendLine(" AND (Formulati.FR_DES LIKE '%" & Agro_SQL_SaveText(TestoRicerca) & "%') ")
                    End If

                    StrSQL.AppendLine(" AND Formulati.Data_Reg <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")


                    Select Case TipoRichiesto

                        Case enum_TipoFormulato.Tutti  'Tutti i formulati

                            ClassificazioneMin = 0
                            ClassificazioneMax = 99999

                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (101,102,103,104,105,106,107,108,109,110,300,610,611,200,201,202,203,400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,608,603,607,500,501,502,503,504,505,506,606,614,615,602,613,617,1003) ")

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "

                            '=================================================================

                        Case enum_TipoFormulato.Antiparassitari  'Trattamento Antiparassitario

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '
                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (101,102,103,104,105,106,107,108,109,110,610,611,606) ")


                            '=================================================================


                        Case enum_TipoFormulato.Diserbanti  'Diserbo

                            ClassificazioneMin = 200
                            ClassificazioneMax = 299

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (200,201,202,203) ")

                            '=================================================================

                        Case enum_TipoFormulato.Fitoregolatori  'Trattamenti Fitoregolatori

                            ClassificazioneMin = 400
                            ClassificazioneMax = 499

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")


                        Case enum_TipoFormulato.Coadiuvanti  'Coadiuvanti, Bagnanti, Antischiuma

                            ClassificazioneMin = 500
                            ClassificazioneMax = 506

                            StrSQL.AppendLine(" AND (ClassificazioniFormulati.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  ")

                        Case enum_TipoFormulato.Concianti  'Concia

                            '               Classificazione Concianti = 608

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (608)  ")

                        Case enum_TipoFormulato.Disseccanti  'Disseccamento

                            '               Classificazione Disseccanti = 603

                            'Sql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (603)  ")

                        Case enum_TipoFormulato.Geodisinfestanti  'Geodisinfestazione

                            '               Classificazione Geodisinfestanti = 607

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (607)  ")

                        Case enum_TipoFormulato.Antiparassitari_Concianti  'Trattamenti Antiparassitari + Concianti

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '                + Classificazione Concianti = 608

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                        Case enum_TipoFormulato.Diserbanti_Disseccanti 'Diserbo + Disseccanti

                            '                ClassificazioneMin = 200
                            '                ClassificazioneMax = 299
                            '                + Classificazione Disseccanti = 603

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (200,201,202,203,603) ")

                        Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti  'Trattamenti Antiparassitari + Geodisinfestanti

                            '                ClassificazioneMin = 0
                            '                ClassificazioneMax = 200
                            '                + Classificazione Geodisinfestanti = 607

                            'sSql = sSql & " AND FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD "
                            StrSQL.AppendLine(" AND ClassificazioniFormulati.CLASS_COD IN (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

                    End Select

                    'If VEG_COD <> 0 Then
                    '   sSql = sSql & " AND FormulatixSpecieVegetali.Veg_Cod = " & VEG_COD & "  "
                    'End If

                    If VEG_COD <> 0 Then
                        StrSQL.AppendLine(" AND ( FormulatixSpecieVegetali.Veg_Cod = " & VEG_COD & "  ")
                        StrSQL.AppendLine(" OR  ( FormulatixSpecieVegetali.Grsp_Cod IN (Select Grsp_Cod From GruppoColturaleXSpecieVegetali ")
                        StrSQL.AppendLine("      WHERE GruppoColturaleXSpecieVegetali.Veg_Cod = " & VEG_COD & " ) ) ) ")
                    End If


                    If Trim(strPA) <> "" Then
                        'StrSQL.AppendLine(" AND PrincipiAttivi.PA_Cod IN " & Agro_SQL_Save_Clausola_IN(strPA) & "  ")
                        StrSQL.AppendLine(" AND Formulati.FR_COD NOT IN (Select Fr_Cod From FormulatixPrincipiAttivi, PrincipiAttivi Where PrincipiAttivi.Pa_Cod Not IN " & Agro_SQL_Save_Clausola_IN(strPA, False, parametrizza) & " and PrincipiAttivi.PA_Cod = FormulatixPrincipiAttivi.Pa_Cod and PrincipiAttivi.Pa_Co_Trattamento = 0)")
                    End If

                    If Fr_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Formulati.FR_COD = " & Agro_SQL_SaveNum(Fr_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, parametrizza, objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Formulati.FR_DES ASC")
                    End If

                    StrSQL.AppendLine(" DROP TABLE #CTE_FormulatixPrincipiAttivi ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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

    Public Function LeggiLavCodCompatibili(ByVal FR_COD As Int32,
                                          ByVal CLASS_COD As Int32,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R.LeggiLavCodCompatibili()"

        Dim MessaggioErrore As String = ""
        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            If IsNothing(DictionaryLavCodTipoFormulato) OrElse DictionaryLavCodTipoFormulato.Count = 0 Then
                Throw New Exception("Dizionario LavCodTipoFormulato non valorizzato.")
            End If

            StrSQL.AppendLine(" WITH CTE_TutteClassificazioni AS ( ")
            StrSQL.AppendLine(" SELECT FormulatixClassificazioni.FOR_Cod, FormulatixClassificazioni.CLASS_COD ")
            StrSQL.AppendLine(" FROM  FormulatixClassificazioni ")
            StrSQL.AppendLine(" INNER JOIN Formulati ON FormulatixClassificazioni.FOR_Cod = Formulati.Fr_Cod ")
            StrSQL.AppendLine(" WHERE FormulatixClassificazioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   FormulatixClassificazioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If FR_COD <> 0 Then
                StrSQL.AppendLine(" AND FormulatixClassificazioni.FOR_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If CLASS_COD <> 0 Then
                StrSQL.AppendLine(" AND FormulatixClassificazioni.CLASS_COD =  " & Agro_SQL_SaveNum(CLASS_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine("), ")

            For x = 0 To DictionaryLavCodTipoFormulato.Keys.Count - 1

                Dim Lav_Cod As Integer = DictionaryLavCodTipoFormulato.Keys(x)

                StrSQL.AppendLine(GetCte_ClassificazioneName(Lav_Cod) & " AS ( ")
                StrSQL.AppendLine("  SELECT  FOR_Cod, " & Lav_Cod & " AS Lav_Cod")
                StrSQL.AppendLine(" FROM  CTE_TutteClassificazioni ")
                StrSQL.AppendLine(" WHERE 1 = 1 ")
                objFiltro.Filtro_Formulato_Classificazione(String.Join(",", DictionaryLavCodTipoFormulato(Lav_Cod)), StrSQL, "CTE_TutteClassificazioni")

                If x = DictionaryLavCodTipoFormulato.Keys.Count - 1 Then
                    StrSQL.AppendLine(" ) ")
                Else
                    StrSQL.AppendLine(" ), ")
                End If
            Next

            StrSQL.AppendLine("  SELECT FOR_Cod, Lav_Cod  ")
            StrSQL.AppendLine("  FROM " & GetCte_ClassificazioneName(DictionaryLavCodTipoFormulato.Keys(0)))

            If DictionaryLavCodTipoFormulato.Keys.Count > 1 Then
                For x = 1 To DictionaryLavCodTipoFormulato.Keys.Count - 1

                    Dim Lav_Cod As Integer = DictionaryLavCodTipoFormulato.Keys(x)

                    StrSQL.AppendLine(" UNION ")
                    StrSQL.AppendLine("  SELECT FOR_Cod, Lav_Cod  ")
                    StrSQL.AppendLine("  FROM  " & GetCte_ClassificazioneName(Lav_Cod))
                Next
            End If

            StrSQL.AppendLine(" ORDER BY FOR_Cod, Lav_Cod ASC")

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

    Private Function GetCte_ClassificazioneName(ByVal Lav_Cod As Integer) As String
        Return "CTE_Classificazioni_" & Lav_Cod
    End Function

    '#################
    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FrCod"></param>
    ''' <param name="VegCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    'Public Function PaDes_from_FrCod(ByVal FrCod As Integer, _
    '                                 ByVal VegCod As Integer, _
    '                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                 ) As String

    '    Dim Dt As DataTable

    '    Dim Filtro As String = "Formulati.Fr_Cod=" & FrCod

    '    Dt = LeggiXClassificazione_PA(CStr(""), _
    '                                         CInt(0), _
    '                                         CInt(VegCod), _
    '                                         CStr(""), _
    '                                         AGRODATAINIZIO, _
    '                                         AGRODATAFINE, _
    '                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                          Filtro, _
    '                                         "", objParametri)

    '    'Verifico se il recordset e' aperto
    '    If Not IsNothing(Dt) AndAlso Dt.Rows.Count <> 0 Then

    '        If Not IsDBNull(Dt.Rows(0).Item("pa_des")) And CStr(Dt.Rows(0).Item("pa_des")) <> "" Then

    '            Return Dt.Rows(0).Item("pa_des")

    '        Else
    '            Return ""

    '        End If

    '        Return ""

    '    Else

    '        Return ""

    '    End If


    'End Function


End Class
