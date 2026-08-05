Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PianoConcimazione_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '  Galassi, 24/02/2017 11.56.17: Copiato dai Core dell'ANAGRAFE
    '##############################################################################################
    Public Function Leggi_default(ByVal PC_Testata_Cod As Integer, _
                          ByVal Regolamento_Cod As Integer, _
                          ByVal PC_Tipo As Integer, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM  PianoConcimazione_Testata " & vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If Regolamento_Cod <> 0 Then
                        StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
                    End If

                    If PC_Tipo <> 0 Then
                        StrSQL.Append(" AND PC_Tipo = " & Agro_SQL_SaveNum(PC_Tipo) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append("SELECT PT.PC_Testata_Cod,PT.PC_Testata_Des,PT.PC_Tipo,PT.Data_Creazione,PT.Regolamento_Cod,PT.PC_Elaborazione_Cod,PD.PC_Dettagli_PIVA, IMP.rag_soc, " & vbCrLf)
                    StrSQL.Append(" PD.PC_Dettagli_Anno,PD.PC_Dettagli_ColturaPrincipale_Veg_Cod, SP.Veg_Des, REG.Reg_Des " & vbCrLf)
                    StrSQL.Append("  FROM  PianoConcimazione_Testata PT  " & vbCrLf)
                    StrSQL.Append("  INNER JOIN  PianoConcimazione_Dettagli PD  " & vbCrLf)
                    StrSQL.Append("  ON  PD.PC_Testata_Cod = PT.PC_Testata_Cod  " & vbCrLf)
                    StrSQL.Append("  left join SpecieVegetali SP on PD.PC_Dettagli_ColturaPrincipale_Veg_Cod = SP.Veg_Cod " & vbCrLf)
                    StrSQL.Append("  left join Regolamenti REG on PT.Regolamento_Cod = REG.Reg_Cod  " & vbCrLf)
                    StrSQL.Append("  left join Imprese IMP on PD.PC_Dettagli_PIVA = IMP.PIVA " & vbCrLf)
                    StrSQL.Append(" WHERE PT.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND   PT.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND PT.PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PT.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If Regolamento_Cod <> 0 Then
                        StrSQL.Append(" AND PT.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
                    End If

                    If PC_Tipo <> 0 Then
                        StrSQL.Append(" AND PT.PC_Tipo = " & Agro_SQL_SaveNum(PC_Tipo) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PT.Inviato >=0 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PT.Inviato =-1 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT PT.*,PD.* " & vbCrLf)
                    StrSQL.Append(" FROM  PianoConcimazione_Testata PT " & vbCrLf)
                    StrSQL.Append(" INNER JOIN  PianoConcimazione_Dettagli PD " & vbCrLf)
                    StrSQL.Append(" ON  PD.PC_Testata_Cod = PT.PC_Testata_Cod " & vbCrLf)
                    StrSQL.Append(" WHERE PT.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND   PT.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND PT.PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PT.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    If Regolamento_Cod <> 0 Then
                        StrSQL.Append(" AND PT.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
                    End If

                    If PC_Tipo <> 0 Then
                        StrSQL.Append(" AND PT.PC_Tipo = " & Agro_SQL_SaveNum(PC_Tipo) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PT.Inviato >=0 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PT.Inviato =-1 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '  Galassi, 27/02/2017 15.54.36: 
    '##############################################################################################
    Public Function Leggi_xGriglia(ByVal Piva As String,
                                   ByVal PC_Testata_Cod As Integer,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal PC_Tipo As Integer,
                                   ByVal Data_inizio As Date,
                                   ByVal Data_fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Regolamento_Tipo As enum_PUARegolamenti_Tipo = 0,
                                       Optional ByVal filtroRegolamentiPrivati As String = ""
                                   ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine(" PT.PC_Testata_Cod, PT.PC_Testata_Des, PT.PC_Tipo,  ")
            StrSQL.AppendLine(" PT.validita_inizio, PT.Validita_Fine, ")
            StrSQL.AppendLine(" PT.Regolamento_Cod, PT.PC_Elaborazione_Cod, PD.PC_Dettagli_PIVA,  ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(IMP.partitaIvaReale, '') = '' THEN PD.PC_Dettagli_PIVA ELSE IMP.partitaIvaReale END PivaReale, ")
            StrSQL.AppendLine(" IMP.rag_soc, PD.PC_Dettagli_Anno,  ")
            StrSQL.AppendLine(" PD.PC_Dettagli_ColturaPrincipale_Veg_Cod, SP.Veg_Des, REG.Regolamento_Des, ")
            StrSQL.AppendLine(" ISNULL(PD.Allegati_Documenti_Cod, 0) as Allegati_Documenti_Cod,")

            StrSQL.AppendLine("ISNULL( ")
            StrSQL.AppendLine("  (Select top 1 SPE_VE.Veg_Des ")
            StrSQL.AppendLine("  from PianoConcimazione_EntitaxTestata PET inner join Reg_Impianti IMP ")
            StrSQL.AppendLine("  on PET.Piva = IMP.PIVA and PET.Sa_Cod = IMP.SA_COD and PET.Campo_Cod = IMP.ID_CAMPO and PET.Appezza = IMP.APPEZZA and PET.Id_Imp = IMP.ID_REG  ")
            StrSQL.AppendLine("  left join Cultivar CUL on CUL.Cul_Cod = IMP.CUL_COD ")
            StrSQL.AppendLine("  left join SpecieVegetali SPE_VE on SPE_VE.Veg_Cod = CUL.Veg_Cod ")
            StrSQL.AppendLine("  where PET.PC_Testata_Cod = PT.PC_Testata_Cod), SP.Veg_Des) as Veg_Des,")

            '  Galassi, 05/04/2017 09.45.01: uso tipo ricetta statico a Piano Distribuzione concimi
            StrSQL.AppendLine("ISNULL((select COUNT (*) FROM  Ricette R WHERE R.Programmazione_Cod = PT.PC_Testata_Cod ")
            StrSQL.AppendLine(" AND R.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" and R.Tipo_Ricetta = " & Agro_SQL_SaveNum(enum_TipoRicetta.PianoDistribuzioneConcimi) & "),0) as nPianiDistr ")

            'StrSQL.AppendLine(", " & enum_PUARegolamenti_Tipo.PianoComcimazione & " AS Regolamento_Tipo ")
            StrSQL.AppendLine(", REG.Tipo AS Regolamento_Tipo ")

            StrSQL.AppendLine(" , ISNULL(pt.blocco_flag,0) AS blocco_flag ")
            StrSQL.AppendLine(" , ISNULL(CA.sa_nome,'') AS sa_nome ")

            StrSQL.AppendLine(" , ISNULL(pt.note,0) AS note ")
            StrSQL.AppendLine(" , ISNULL(pt.Flag_NonUtilizzo_Fertilizzanti,0) AS Flag_NonUtilizzo_Fertilizzanti ")


            StrSQL.AppendLine("  FROM  PianoConcimazione_Testata PT  ")
            StrSQL.AppendLine("  INNER JOIN  PianoConcimazione_Dettagli PD  ")
            StrSQL.AppendLine("  ON  PD.PC_Testata_Cod = PT.PC_Testata_Cod  ")
            StrSQL.AppendLine("  left join SpecieVegetali SP on PD.PC_Dettagli_ColturaPrincipale_Veg_Cod = SP.Veg_Cod ")
            StrSQL.AppendLine("  left join PUA_Regolamenti REG on PT.Regolamento_Cod = REG.Regolamento_Cod  ")
            StrSQL.AppendLine("  left join Imprese IMP on PD.PC_Dettagli_PIVA = IMP.PIVA ")
            StrSQL.AppendLine("  left join Centri_Aziendali CA on PD.PC_Dettagli_PIVA = CA.PIVA AND PD.PC_Dettagli_SaCod = CA.Sa_Cod  ")

            StrSQL.AppendLine(" WHERE PT.Validita_inizio <= " & Agro_SQL_SaveDate(Data_fine) & " ")
            StrSQL.AppendLine(" AND   PT.Validita_Fine >= " & Agro_SQL_SaveDate(Data_inizio) & " ")

            StrSQL.AppendLine(" AND PT.PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PC_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PT.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PD.PC_Dettagli_PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND PT.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If PC_Tipo <> 0 Then
                StrSQL.AppendLine(" AND PT.PC_Tipo = " & Agro_SQL_SaveNum(PC_Tipo) & " ")
            End If

            If Regolamento_Tipo <> 0 Then
                'AF: 06/23 Aggiunta gestione di un piano nutrizionale generale. Carico sempre quelli con Regolamento_Tipo = 5 (PianoNutrizionale_IBF), e se l'ambiente è configurato per l'uso dei privati li carico usando il Regolamento_Cod specifico
                If (Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF OrElse Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoNutrizionale) AndAlso filtroRegolamentiPrivati <> "" Then
                    StrSQL.AppendLine(" AND (REG.Tipo = " & Agro_SQL_SaveNum(enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF) & " OR REG.Regolamento_Cod IN ( " & Agro_SQL_Save_Clausola_IN(filtroRegolamentiPrivati) & "))")
                Else
                    StrSQL.AppendLine(" AND REG.Tipo = " & Agro_SQL_SaveNum(Regolamento_Tipo) & " ")
                End If
            End If

            '(25/02/2020 fede) aggiunto filtro visibilita
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= " (PD.PC_Dettagli_PIVA = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND PD.PC_Dettagli_SaCod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If FiltroCentri <> "" Then
                    StrSQL.AppendLine(" AND (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (PD.PC_Dettagli_PIVA = '" & Agro_SQL_SaveText(Piva) & "' AND PD.PC_Dettagli_SaCod = 0) ) ")
                End If
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   PT.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   PT.Inviato =-1 ")
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


    '##############################################################################################
    Public Function DistinctTestataCod(ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R.DistinctTestataCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PC_Testata_Cod ")
            StrSQL.Append(" FROM   PianoConcimazione_Testata ")
            StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PC_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PC_Testata_Cod ")
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

    '##############################################################################################
    Public Function DistinctTestataCod_conFiltroPiva(ByVal Piva As String, _
                                                     ByVal xFiltroAggiuntivo As String, _
                                                    ByVal xOrderBy As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R.DistinctTestataCod_conFiltroPiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PianoConcimazione_Testata.PC_Testata_Cod ")
            StrSQL.Append(" FROM   PianoConcimazione_Testata ")
            StrSQL.Append(" INNER JOIN PianoConcimazione_EntitaxTestata ")
            StrSQL.Append(" ON PianoConcimazione_EntitaxTestata.PC_Testata_Cod = PianoConcimazione_Testata.PC_Testata_Cod AND PianoConcimazione_EntitaxTestata.PC_SuperUser = PianoConcimazione_Testata.PC_SuperUser ")

            StrSQL.Append(" WHERE  PianoConcimazione_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PianoConcimazione_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PianoConcimazione_Testata.PC_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PianoConcimazione_Testata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PianoConcimazione_Testata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PianoConcimazione_Testata.PC_Testata_Cod ")
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

    '##############################################################################################
    Public Function DistinctTestate_conFiltroPiva(ByVal Piva As String, _
                                                  ByVal xFiltroAggiuntivo As String, _
                                                  ByVal xOrderBy As String, _
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_R.DistinctTestate_conFiltroPiva()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT PianoConcimazione_Testata.* ")
            StrSQL.Append(" FROM   PianoConcimazione_Testata ")
            StrSQL.Append(" INNER JOIN PianoConcimazione_EntitaxTestata ")
            StrSQL.Append(" ON PianoConcimazione_EntitaxTestata.PC_Testata_Cod = PianoConcimazione_Testata.PC_Testata_Cod AND PianoConcimazione_EntitaxTestata.PC_SuperUser = PianoConcimazione_Testata.PC_SuperUser ")

            StrSQL.Append(" WHERE  PianoConcimazione_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    PianoConcimazione_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PianoConcimazione_Testata.PC_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Piva <> "" Then
                StrSQL.Append(" AND    PianoConcimazione_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PianoConcimazione_Testata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PianoConcimazione_Testata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PianoConcimazione_Testata.PC_Testata_Cod ")
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

    '  Monti, 24/02/2017 11.56.06: 
    '###############################################################
    Public Function Leggi( _
                ByVal PC_Testata_Cod As Int32, _
                ByVal Regolamento_Cod As enum_PUARegolamenti, _
                ByVal PC_Tipo As enum_PianoConcimazione_Tipo, _
                ByVal xFiltroAggiuntivo As String, _
                ByVal xOrderBy As String, _
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  PianoConcimazione_Testata ")
            StrSQL.Append(" WHERE   (PC_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")


            If PC_Testata_Cod <> 0 Then
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If PC_Tipo <> 0 Then
                StrSQL.Append(" AND PC_Tipo = " & Agro_SQL_SaveNum(PC_Tipo) & " ")
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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

'  Galassi, 24/02/2017 11.57.10: Copiato dai CORE dell'ANAGRAFE
Public Class PianoConcimazione_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PC_Testata_Cod As Integer,
                            ByVal PC_Testata_Des As String,
                            ByVal Regolamento_Cod As Integer,
                            ByVal PC_Tipo As Integer,
                            ByVal PC_Elaborazione_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal Note As String, ByVal Flag_NonUtilizzo_Fertilizzanti As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_Testata ")

            StrSQL.Append("             (PC_SuperUser,      PC_Testata_Cod,     PC_Testata_Des, ")
            StrSQL.Append("              Regolamento_Cod,   PC_Tipo,            PC_Elaborazione_Cod, ")
            StrSQL.Append("              Flag_NonUtilizzo_Fertilizzanti,        Note,  ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine,     DataLock ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PC_Testata_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(PC_Testata_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PC_Tipo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PC_Elaborazione_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_NonUtilizzo_Fertilizzanti) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append(") ")

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
    Public Function Modifica(ByVal PC_Testata_Cod As Integer,
                             ByVal PC_Testata_Des As String,
                             ByVal Regolamento_Cod As Integer,
                             ByVal PC_Tipo As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                      ByVal Note As String, ByVal Flag_NonUtilizzo_Fertilizzanti As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Regolamento_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            'Query per la modifica dei dati             
            StrSQL.Append("UPDATE PianoConcimazione_Testata SET ")
            StrSQL.Append("    PC_Testata_Des    = '" & Agro_SQL_SaveText(PC_Testata_Des) & "'")
            StrSQL.Append("   ,Note             = '" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.Append("   ,Flag_NonUtilizzo_Fertilizzanti        = " & Agro_SQL_SaveNum(Flag_NonUtilizzo_Fertilizzanti) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            StrSQL.Append(" WHERE PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            StrSQL.Append(" AND PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            '------------------------------
            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
    Public Function Cancella(ByVal PC_Testata_Cod As Integer, _
                             ByVal Regolamento_Cod As Integer, _
                               ByVal xFiltroAggiuntivo As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE PianoConcimazione_Testata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    PianoConcimazione_Testata ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")


            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND     Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function BloccaSblocca(ByVal PC_Testata_Cod As Integer, Blocco_Flag As Integer, Blocco_Data As Date, Blocco_Username As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W.BloccaSblocca()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            'Query per la modifica dei dati             
            StrSQL.Append("UPDATE PianoConcimazione_Testata SET ")
            StrSQL.Append("   Blocco_Flag         =  " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            StrSQL.Append("   ,Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data) & " ")
            StrSQL.Append("   ,Blocco_Username     =  " & Agro_SQL_SaveText_NULL(Blocco_Username) & " ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
            StrSQL.Append(" AND PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

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


