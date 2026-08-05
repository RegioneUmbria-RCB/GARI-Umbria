Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class Imprese_Contratto_Fasi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PIVA As String,
                       ByVal Contratto_Cod As Integer,
                       ByVal Fase_Cod As Integer,
                       ByVal Cod_RisUm As Integer,
                       ByVal Cod_Contatto As String,
                       ByVal Elem_Cod As Integer,
                       ByVal Pro_Cod As Integer,
                       ByVal Mat_Cod As Integer,
                       ByVal Cod_Progetto As Integer,
                       ByVal Cal_Cod As Integer,
                       ByVal Lotto As String,
                       ByVal Udm_Cod As Integer,
                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratto_Fasi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.AppendLine("SELECT Imprese_Contratto_Fasi.*, Imprese_Contratti.Contratto_Nome, Imprese_Contratti.Contratto_Numero, Imprese_Contratti.Cod_RisUm, Imprese_Contratti.Cau_Contratto, Imprese_ContrattixTrasformazioni.Udm_Cod as Udm_Cod_Tras, Imprese_ContrattixTrasformazioni.Qta as Qta_Tras, Contatti.Rag_Soc, (Isnull(Cod_Articolo, '') + ' ' + Isnull(Mat_Des, '')) as Mat_Des_Esteso, Isnull(Veg_Des, '') as veg_des ")
                    StrSQL.AppendLine("FROM  Imprese_Contratto_Fasi  ")
                    StrSQL.AppendLine("    LEFT OUTER JOIN Imprese_Contratti ON (Imprese_Contratto_Fasi.Piva = Imprese_Contratti.Piva and Imprese_Contratto_Fasi.Contratto_Cod = Imprese_Contratti.Contratto_Cod)")
                    StrSQL.AppendLine("    LEFT OUTER JOIN Risorse_Umane ON (Imprese_Contratti.Cod_RisUm = Risorse_Umane.Cod_RisUm)")
                    StrSQL.AppendLine("    LEFT OUTER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
                    StrSQL.AppendLine("         AND Contatti.Piva = Risorse_Umane.Piva")
                    StrSQL.AppendLine("    LEFT OUTER JOIN Imprese_ContrattixTrasformazioni ON (Imprese_Contratto_Fasi.Piva = Imprese_ContrattixTrasformazioni.Piva and Imprese_Contratto_Fasi.Fase_Cod = Imprese_ContrattixTrasformazioni.Fase_Cod)")
                    StrSQL.AppendLine("    LEFT OUTER JOIN Materie_Prime ON (Imprese_Contratto_Fasi.Elem_Cod = Materie_Prime.Elem_Cod and Imprese_Contratto_Fasi.Mat_Cod = Materie_Prime.Mat_Cod)")
                    StrSQL.AppendLine("    LEFT OUTER JOIN SpecieVegetali ON (SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod )")
                    StrSQL.AppendLine("WHERE Imprese_Contratto_Fasi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("    AND   Imprese_Contratto_Fasi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   "
                    End If

                    If Contratto_Cod <> -1 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   "
                    End If

                    If Cod_RisUm <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   ")
                        'sSql = sSql & " AND Imprese_Contratti.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   "
                    End If

                    If Cod_Contatto <> "" Then
                        StrSQL.AppendLine("    AND Contatti.Cod_Contatto  = '" & Agro_SQL_SaveNum(Cod_Contatto) & "'   ")
                        'sSql = sSql & " AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   "
                    End If


                    If Elem_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Elem_Cod  = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Pro_Cod  = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   "
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine("    AND  Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Udm_Cod  = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   "
                    End If


                    If Cod_Progetto <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Cod_Progetto  = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   "
                    End If

                    If Cal_Cod <> 0 Then
                        StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Cal_Cod  = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                        'sSql = sSql & " AND Imprese_Contratto_Fasi.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   "
                    End If


                    If Lotto <> "" Then
                        StrSQL.AppendLine("    AND Upper(Imprese_Contratto_Fasi.Lotto)  = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'  ")
                        'sSql = sSql & " AND Upper(Imprese_Contratto_Fasi.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   "
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("    AND Imprese_Contratti.Inviato >=0 ")
                            StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Inviato >=0 ")

                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("    AND Imprese_Contratti.Inviato =-1 ")
                            StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Inviato =-1 ")

                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine("ORDER BY Imprese_Contratto_Fasi.Piva ASC, Imprese_Contratti.Contratto_Numero ASC, Imprese_Contratti.Contratto_Cod ASC")
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

    Public Function LeggiNew(ByVal PIVA As String,
                             ByVal Contratto_Cod As Integer,
                             ByVal Clausola_Cod As Integer,
                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.LeggiNew()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile
                'AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi()
                Case Else
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Imprese_Contratti_Clausole.*, Imprese_ContrattixClausole.Data, (Clausola_Numero + ' - ' + Clausola_Nome) as Clausola_Nome_Esteso ")
                    StrSQL.Append(" FROM  Imprese_Contratti_Clausole, Imprese_ContrattixClausole  ")
                    StrSQL.Append(" WHERE Imprese_ContrattixClausole.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Imprese_ContrattixClausole.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    'StrSQL.Append(" AND   Imprese_ContrattixClausole.Piva = Imprese_Contratti_Clausole.Piva ")
                    StrSQL.Append(" AND   Imprese_ContrattixClausole.Clausola_Cod = Imprese_Contratti_Clausole.Clausola_Cod ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND Imprese_ContrattixClausole.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Contratto_Cod <> -1 Then
                        StrSQL.Append(" AND Imprese_ContrattixClausole.Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
                    End If

                    If Clausola_Cod <> 0 Then
                        StrSQL.Append(" AND Imprese_ContrattixClausole.Clausola_Cod = " & Agro_SQL_SaveNum(Clausola_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese_Contratti_Clausole.Inviato >=0 ")
                            StrSQL.Append(" AND   Imprese_ContrattixClausole.Inviato >=0 ")

                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese_Contratti_Clausole.Inviato =-1 ")
                            StrSQL.Append(" AND   Imprese_ContrattixClausole.Inviato =-1 ")

                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("ORDER BY Data Asc")
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

    Public Function readPianoColturale(
                                             ByVal risUm As Integer,
                                             ByVal dataInizio As Date,
                                             ByVal dataFine As Date,
                                             ByVal matCod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal idBudget As Integer = Nothing
                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.readRequisitiStabilimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("WITH contratti AS (")
            StrSQL.AppendLine("    SELECT")
            StrSQL.AppendLine("        Imprese_Contratti.Id_Budget,")
            StrSQL.AppendLine("        Imprese_Contratti.Piva,")
            StrSQL.AppendLine("        Imprese.rag_soc + ' ' + COALESCE(Centri_Aziendali.sa_nome, '') + ' ' + COALESCE(Fabbricati.Fabbricato_Des, '') as Azienda,")
            StrSQL.AppendLine("        Imprese_Contratti.Contratto_Cod,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.Fase_Cod,")
            StrSQL.AppendLine("        Imprese_Contratti.Cod_Risum,")
            StrSQL.AppendLine("        Imprese.Rag_Soc,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.Mat_Cod,")
            StrSQL.AppendLine("        Contatti.Cod_Contatto,")
            StrSQL.AppendLine("        Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des as Prodotto,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.Superficie,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.ResaPrevista,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.QtaPrevista,")
            StrSQL.AppendLine("        Imprese_Contratto_Fasi.Tipo_Trasporto,")
            StrSQL.AppendLine("        Imprese_Contratti.Validita_Inizio,")
            StrSQL.AppendLine("        Imprese_Contratti.Validita_Fine,")
            StrSQL.AppendLine("        icfPadre.Contratto_Cod As Contratto_Cod_Padre,")
            StrSQL.AppendLine("        icfPadre.Fase_Cod as Fase_Cod_Padre")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("    FROM Imprese_Contratti")
            StrSQL.AppendLine("        JOIN Imprese_Contratto_Fasi On Imprese_Contratti.Contratto_Cod = Imprese_Contratto_Fasi.Contratto_Cod")
            StrSQL.AppendLine("        JOIN Imprese On Imprese.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("        JOIN Imprese_Contratti icPadre ON Imprese_Contratti.Contratto_Cod_Padre = icPadre.Contratto_Cod")
            StrSQL.AppendLine("        JOIN Imprese_Contratto_Fasi icfPadre ON Imprese_Contratto_Fasi.Fase_Cod_Padre = icfPadre.Fase_Cod And icPadre.Contratto_Cod = icfPadre.Contratto_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("        LEFT JOIN Centri_Aziendali ON Centri_Aziendali.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("            AND Centri_Aziendali.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("        LEFT JOIN Fabbricati ON Fabbricati.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("            AND Fabbricati.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("            AND Fabbricati.Fabbricato_Cod = Imprese_Contratti.Fabbricato_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("        LEFT JOIN Risorse_Umane ON Imprese_Contratti.Cod_Risum = Risorse_Umane.Cod_RisUm")
            StrSQL.AppendLine("        LEFT JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto And Risorse_Umane.Piva = Contatti.Piva")
            StrSQL.AppendLine("        JOIN Materie_Prime ON Imprese_Contratto_Fasi.Mat_Cod = Materie_Prime.Mat_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    WHERE 1 = 1")
            StrSQL.AppendLine("        AND icPadre.Cod_Risum = " & Agro_SQL_SaveNum(risUm))
            'StrSQL.AppendLine("        AND Imprese_Contratti.ID_Budget = " & Agro_SQL_SaveNum(idBudget))
            StrSQL.AppendLine("        AND Imprese_Contratti.Cau_Contratto = '9301' -- CAU che identifica il contratto figlio")
            StrSQL.AppendLine("        AND Imprese_Contratti.Validita_Inizio <= " & Agro_SQL_SaveDate(dataFine))
            StrSQL.AppendLine("        AND Imprese_Contratti.Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("        AND Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(matCod))
            StrSQL.AppendLine("        AND Imprese_Contratto_Fasi.Elem_Cod = 210 --Elem_Cod Trasformati Vegetali")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("")


            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    Imprese.PIVA,")
            StrSQL.AppendLine("    Imprese.rag_soc,")
            StrSQL.AppendLine("    COALESCE(cuaa.val_cod, '') AS cuaa,")
            StrSQL.AppendLine("    COALESCE(socio.val_cod, '') AS socio,")
            StrSQL.AppendLine("    p.PIVA AS Piva_Padre,")
            StrSQL.AppendLine("    p.rag_soc AS Rag_Soc_Padre,")
            StrSQL.AppendLine("    ISNULL(Gruppi_Raccolta.GruppoRaccolta_Cod, 0) AS GruppoRaccolta_Cod,")
            StrSQL.AppendLine("    ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS GruppoRaccolta_Des,")
            StrSQL.AppendLine("    SpecieVegetali.Veg_Cod,")
            StrSQL.AppendLine("    SpecieVegetali.Veg_Des,")
            'StrSQL.AppendLine("    Cultivar.Cul_Cod,")
            'StrSQL.AppendLine("    Cultivar.Cul_Des,")
            StrSQL.AppendLine("    Materie_Prime.Mat_Cod,")
            StrSQL.AppendLine("    Materie_Prime.Mat_Des,")
            StrSQL.AppendLine("    ISNULL(Contratti.Contratto_Cod, 0) AS Contratto_Cod,")
            StrSQL.AppendLine("    ISNULL(Contratti.Fase_Cod, 0) AS Fase_Cod,")
            StrSQL.AppendLine("    ISNULL(Contratti.Contratto_Cod_Padre, 0) AS Contratto_Cod_Padre,")
            StrSQL.AppendLine("    ISNULL(Contratti.Fase_Cod_Padre, 0) AS Fase_Cod_Padre,")
            StrSQL.AppendLine("    ISNULL(Contratti.Superficie, 0) AS Superficie,")
            StrSQL.AppendLine("    ISNULL(Contratti.ResaPrevista, 0) AS ResaPrevista,")
            StrSQL.AppendLine("    ISNULL(Contratti.QtaPrevista, 0) AS QtaPrevista,")
            StrSQL.AppendLine("    ISNULL(Contratti.Tipo_Trasporto, 0) AS TipoTrasporto_Cod,")
            StrSQL.AppendLine("    SUM(ISNULL(impianti.Sup_Imp, 0)) AS ImpiantiSuperficie,")
            StrSQL.AppendLine("    SUM(impianti.Sup_Imp * esercizi.Produzione_Prevista) AS ImpiantiResaPrevista,")
            StrSQL.AppendLine("    AVG(esercizi.Produzione_Prevista) AS ImpiantiResaMedia ")

            StrSQL.AppendLine("From Imprese")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici cuaa ON Imprese.PIVA = cuaa.PIVA And cuaa.id_cod = 1010")
            StrSQL.AppendLine("    LEFT JOIN Imprese_Codici socio ON Imprese.PIVA = socio.PIVA And socio.id_cod = 1033")
            StrSQL.AppendLine("    LEFT JOIN GerarchiaImprese ON Imprese.PIVA = GerarchiaImprese.Figlio")
            StrSQL.AppendLine("    LEFT JOIN Imprese p ON GerarchiaImprese.Padre = p.PIVA")
            StrSQL.AppendLine("    LEFT JOIN Gruppi_Raccolta ON Imprese.GruppoRaccolta_Cod =")
            StrSQL.AppendLine("        Gruppi_Raccolta.GruppoRaccolta_Cod")

            If idBudget <> Nothing AndAlso idBudget <> 0 Then
                StrSQL.AppendLine("    JOIN Budget_Imprese_Progetti esercizi ON Imprese.PIVA = esercizi.Piva")
                StrSQL.AppendLine("         AND esercizi.Id_Budget = " & Agro_SQL_SaveNum(idBudget))

                StrSQL.AppendLine("    JOIN Budget_Reg_Impianti impianti ON esercizi.Piva = impianti.PIVA")
                StrSQL.AppendLine("        AND esercizi.Sa_Cod = impianti.SA_COD")
                StrSQL.AppendLine("        AND esercizi.Appezza = impianti.APPEZZA")
                StrSQL.AppendLine("        AND esercizi.Id_Reg = impianti.ID_REG")
                StrSQL.AppendLine("        AND impianti.Id_Budget = " & Agro_SQL_SaveNum(idBudget))

                StrSQL.AppendLine("    JOIN Budget_Appezzamento app ON app.Piva = impianti.PIVA")
                StrSQL.AppendLine("        AND app.Sa_Cod = impianti.SA_COD")
                StrSQL.AppendLine("        AND app.Appezza = impianti.APPEZZA")
                StrSQL.AppendLine("        AND app.Id_Budget = " & Agro_SQL_SaveNum(idBudget))
            Else
                StrSQL.AppendLine("    JOIN Imprese_Progetti esercizi ON Imprese.PIVA = esercizi.Piva")

                StrSQL.AppendLine("    JOIN Reg_Impianti impianti ON esercizi.Piva = impianti.PIVA")
                StrSQL.AppendLine("        AND esercizi.Sa_Cod = impianti.SA_COD")
                StrSQL.AppendLine("        AND esercizi.Appezza = impianti.APPEZZA")
                StrSQL.AppendLine("        AND esercizi.Id_Reg = impianti.ID_REG")

                StrSQL.AppendLine("    JOIN Appezzamento app ON app.Piva = impianti.PIVA")
                StrSQL.AppendLine("        AND app.Sa_Cod = impianti.SA_COD")
                StrSQL.AppendLine("        AND app.Appezza = impianti.APPEZZA")
            End If

            StrSQL.AppendLine("    JOIN Cultivar ON impianti.CUL_COD = Cultivar.Cul_Cod")
            StrSQL.AppendLine("    JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod")
            StrSQL.AppendLine("    LEFT JOIN Materie_Prime ON esercizi.Mat_Cod = Materie_Prime.Mat_Cod")

            StrSQL.AppendLine("    LEFT JOIN Contratti ON Contratti.Piva = Imprese.Piva")
            StrSQL.AppendLine("        AND Contratti.Mat_Cod = esercizi.Mat_Cod")

            StrSQL.AppendLine("WHERE 1 = 1")
            StrSQL.AppendLine("    AND esercizi.Validita_Inizio <= " & Agro_SQL_SaveDate(dataFine))
            StrSQL.AppendLine("    AND esercizi.Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("    AND esercizi.Mat_Cod = " & Agro_SQL_SaveNum(matCod))

            StrSQL.AppendLine("GROUP BY Imprese.PIVA,")
            StrSQL.AppendLine("    Imprese.rag_soc,")
            StrSQL.AppendLine("    cuaa.val_cod,")
            StrSQL.AppendLine("    socio.val_cod,")
            StrSQL.AppendLine("    p.PIVA,")
            StrSQL.AppendLine("    p.rag_soc,")
            StrSQL.AppendLine("    Gruppi_Raccolta.GruppoRaccolta_Cod,")
            StrSQL.AppendLine("    Gruppi_Raccolta.GruppoRaccolta_Des,")
            StrSQL.AppendLine("    SpecieVegetali.Veg_Cod,")
            StrSQL.AppendLine("    SpecieVegetali.Veg_Des,")
            'StrSQL.AppendLine("    Cultivar.Cul_Cod,")
            'StrSQL.AppendLine("    Cultivar.Cul_Des,")
            StrSQL.AppendLine("    Materie_Prime.Mat_Cod,")
            StrSQL.AppendLine("    Materie_Prime.Mat_Des,")

            StrSQL.AppendLine("    Contratti.Contratto_Cod,")
            StrSQL.AppendLine("    Contratti.Fase_Cod,")
            StrSQL.AppendLine("    Contratti.Contratto_Cod_Padre,")
            StrSQL.AppendLine("    Contratti.Fase_Cod_Padre,")
            StrSQL.AppendLine("    Contratti.Superficie,")
            StrSQL.AppendLine("    Contratti.ResaPrevista,")
            StrSQL.AppendLine("    Contratti.QtaPrevista,")
            StrSQL.AppendLine("    Contratti.Tipo_Trasporto")


            StrSQL.AppendLine("")

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

    Public Function readContracts(
                                 ByVal risUm As Integer,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByVal matCod As Integer,
                                 ByVal mostraAssegnazioni As Boolean,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional codContattoConferene As String = ""
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.readContracts()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    CAST(Imprese_Contratti.Contratto_Cod AS varchar(50)) +  '_' + CAST(Imprese_Contratto_Fasi.Fase_Cod  AS varchar(50)) as chiave,")
            StrSQL.AppendLine("    Imprese_Contratti.Piva,")
            StrSQL.AppendLine("    Imprese_Contratti.Contratto_Nome,")
            StrSQL.AppendLine("    Imprese.rag_soc + ' ' + COALESCE(Centri_Aziendali.sa_nome, '') + ' ' + COALESCE(Fabbricati.Fabbricato_Des, '') AS Azienda,")
            StrSQL.AppendLine("    Imprese_Contratti.Contratto_Cod,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Fase_Cod,")
            StrSQL.AppendLine("    Imprese_Contratti.Cod_Risum,")
            StrSQL.AppendLine("    Contatti.Rag_Soc,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Mat_Cod,")
            StrSQL.AppendLine("    Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des AS Mat_Des,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Superficie,")
            'StrSQL.AppendLine("    Imprese_Contratto_Fasi.ResaPrevista,")
            StrSQL.AppendLine("    IIF(Imprese_Contratto_Fasi.Superficie > 0, (Imprese_Contratto_Fasi.QtaPrevista / Imprese_Contratto_Fasi.Superficie), 0) as ResaPrevista,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.QtaPrevista,")
            StrSQL.AppendLine("    CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese_Contratti.Piva ELSE Imprese.partitaIvaReale END AS partitaIvaReale,")
            StrSQL.AppendLine("    Imprese_Contratti.Validita_Inizio,")
            StrSQL.AppendLine("    Imprese_Contratti.Validita_Fine")
            If mostraAssegnazioni Then
                StrSQL.AppendLine("    , SUM(ff.Superficie) as Sup_Assegnata")
                StrSQL.AppendLine("    , SUM(ff.QtaPrevista) as Qta_Assegnata")
                StrSQL.AppendLine("    , COALESCE( IIF((SUM(ff.Superficie) / Imprese_Contratto_Fasi.Superficie) < 0.01, 0, (SUM(ff.Superficie) / Imprese_Contratto_Fasi.Superficie)) * 100, 0) AS Percentuale_Sup_Assegnata ")
                StrSQL.AppendLine("    , COALESCE( IIF((SUM(ff.QtaPrevista) / Imprese_Contratto_Fasi.QtaPrevista)  < 0.01, 0, (SUM(ff.QtaPrevista) / Imprese_Contratto_Fasi.QtaPrevista)) * 100, 0)  AS Percentuale_Qta_Assegnata ")
            End If
            StrSQL.AppendLine("")
            StrSQL.AppendLine("FROM Imprese_Contratti")
            StrSQL.AppendLine("    JOIN Imprese_Contratto_Fasi On Imprese_Contratti.Contratto_Cod = Imprese_Contratto_Fasi.Contratto_Cod")
            StrSQL.AppendLine("    JOIN Imprese On Imprese.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("    LEFT JOIN Centri_Aziendali ON Centri_Aziendali.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("        AND Centri_Aziendali.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("    LEFT JOIN Fabbricati ON Fabbricati.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("        AND Fabbricati.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("        AND Fabbricati.Fabbricato_Cod = Imprese_Contratti.Fabbricato_Cod")
            StrSQL.AppendLine("    JOIN Risorse_Umane ON Imprese_Contratti.Cod_Risum = Risorse_Umane.Cod_RisUm")
            StrSQL.AppendLine("    JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto And Risorse_Umane.Piva = Contatti.Piva")
            StrSQL.AppendLine("    JOIN Materie_Prime ON Imprese_Contratto_Fasi.Mat_Cod = Materie_Prime.Mat_Cod")
            If mostraAssegnazioni Then
                StrSQL.AppendLine("    LEFT JOIN Imprese_Contratti f ON Imprese_Contratti.Contratto_Cod = f.Contratto_Cod_Padre ")
                StrSQL.AppendLine("    LEFT JOIN Imprese_Contratto_Fasi ff ON Imprese_Contratto_Fasi.Fase_Cod = ff.Fase_Cod_Padre AND f.Contratto_Cod = ff.Contratto_Cod ")
            End If
            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE 1 = 1")

            StrSQL.AppendLine("    AND Imprese_Contratti.Cau_Contratto = '9300'")

            If risUm <> 0 Then
                StrSQL.AppendLine("    AND Imprese_Contratti.Cod_Risum = " & Agro_SQL_SaveNum(risUm))
            End If

            If dataFine <> AGRODATAFINE Then
                StrSQL.AppendLine("    AND Imprese_Contratti.Validita_Inizio <= " & Agro_SQL_SaveDate(dataFine))
            End If

            If dataInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine("    AND Imprese_Contratti.Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio))
            End If

            If matCod <> 0 Then
                StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(matCod))
                StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Elem_Cod = 210 --Elem_Cod Trasformati Vegetali")
            End If

            If codContattoConferene <> "" Then
                StrSQL.AppendFormat("    AND Contatti.Cod_Contatto = '{0}'", Agro_SQL_SaveText(codContattoConferene)).AppendLine()
            End If

            If mostraAssegnazioni Then
                StrSQL.AppendLine("GROUP BY ")
                StrSQL.AppendLine("    Imprese_Contratti.Piva,")
                StrSQL.AppendLine("    Imprese_Contratti.Contratto_Nome,")
                StrSQL.AppendLine("    Imprese.rag_soc,")
                StrSQL.AppendLine("    Centri_Aziendali.sa_nome,")
                StrSQL.AppendLine("    Fabbricati.Fabbricato_Des,")
                StrSQL.AppendLine("    Imprese_Contratti.Contratto_Cod, ")
                StrSQL.AppendLine("    Imprese_Contratto_Fasi.Fase_Cod, ")
                StrSQL.AppendLine("    Imprese_Contratti.Cod_Risum, ")
                StrSQL.AppendLine("    Contatti.Rag_Soc, ")
                StrSQL.AppendLine("    Imprese_Contratto_Fasi.Mat_Cod, ")
                StrSQL.AppendLine("    Materie_Prime.Cod_Articolo, ")
                StrSQL.AppendLine("	   Materie_Prime.Mat_Des, ")
                StrSQL.AppendLine("    Imprese_Contratto_Fasi.Superficie, ")
                StrSQL.AppendLine("    Imprese_Contratto_Fasi.ResaPrevista, ")
                StrSQL.AppendLine("    Imprese_Contratto_Fasi.QtaPrevista, ")
                StrSQL.AppendLine("    Imprese_Contratti.Validita_Inizio, ")
                StrSQL.AppendLine("    Imprese_Contratti.Validita_Fine ")
            End If

            StrSQL.AppendLine("")

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

    Public Function readSurfaceForProducts(
                                          ByVal risUm As Integer,
                                          ByVal dataInizio As Date,
                                          ByVal dataFine As Date,
                                          ByVal matCod As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal idBudget As Integer = Nothing
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.readSurfaceForProducts()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    Imprese_Contratti.Piva,")
            StrSQL.AppendLine("    Imprese.rag_soc + ' ' + COALESCE(Centri_Aziendali.sa_nome, '') + ' ' + COALESCE(Fabbricati.Fabbricato_Des, '') as Azienda,")
            StrSQL.AppendLine("    Imprese_Contratti.Contratto_Cod,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Fase_Cod,")
            StrSQL.AppendLine("    Imprese_Contratti.Cod_Risum,")
            StrSQL.AppendLine("    Contatti.Rag_Soc,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Mat_Cod,")
            StrSQL.AppendLine("    Contatti.Cod_Contatto,")
            StrSQL.AppendLine("    Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des AS Prodotto,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.Superficie,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.ResaPrevista,")
            StrSQL.AppendLine("    Imprese_Contratto_Fasi.QtaPrevista,")
            StrSQL.AppendLine("    Imprese_Contratti.Validita_Inizio,")
            StrSQL.AppendLine("    Imprese_Contratti.Validita_Fine,")
            StrSQL.AppendLine("    icfPadre.Contratto_Cod As Contratto_Cod_Padre,")
            StrSQL.AppendLine("    icfPadre.Fase_Cod as Fase_Cod_Padre")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("FROM Imprese_Contratti")
            StrSQL.AppendLine("    JOIN Imprese_Contratto_Fasi On Imprese_Contratti.Contratto_Cod = Imprese_Contratto_Fasi.Contratto_Cod")
            StrSQL.AppendLine("    JOIN Imprese On Imprese.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("    JOIN Imprese_Contratti icPadre ON Imprese_Contratti.Contratto_Cod_Padre = icPadre.Contratto_Cod")
            StrSQL.AppendLine("    JOIN Imprese_Contratto_Fasi icfPadre ON Imprese_Contratto_Fasi.Fase_Cod_Padre = icfPadre.Fase_Cod And icPadre.Contratto_Cod = icfPadre.Contratto_Cod")
            StrSQL.AppendLine("    LEFT JOIN Centri_Aziendali ON Centri_Aziendali.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("        AND Centri_Aziendali.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("    LEFT JOIN Fabbricati ON Fabbricati.PIVA = Imprese_Contratti.Piva")
            StrSQL.AppendLine("        AND Fabbricati.sa_cod = Imprese_Contratti.sa_cod")
            StrSQL.AppendLine("        AND Fabbricati.Fabbricato_Cod = Imprese_Contratti.Fabbricato_Cod")
            StrSQL.AppendLine("    JOIN Risorse_Umane ON Imprese_Contratti.Cod_Risum = Risorse_Umane.Cod_RisUm")
            StrSQL.AppendLine("    JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto And Risorse_Umane.Piva = Contatti.Piva")
            StrSQL.AppendLine("    JOIN Materie_Prime ON Imprese_Contratto_Fasi.Mat_Cod = Materie_Prime.Mat_Cod")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE 1 = 1")
            StrSQL.AppendLine("    --And icPadre.Cod_Risum = " & Agro_SQL_SaveNum(risUm))
            StrSQL.AppendLine("    AND Imprese_Contratti.Cau_Contratto = '9301' --CAU Che identifica il contratto 'figlio'")
            StrSQL.AppendLine("    AND Imprese_Contratti.Validita_Inizio >= " & Agro_SQL_SaveDate(dataInizio))
            StrSQL.AppendLine("    AND Imprese_Contratti.Validita_Fine <= " & Agro_SQL_SaveDate(dataFine))
            StrSQL.AppendLine("    --AND Imprese_Contratto_Fasi.Mat_Cod = " & Agro_SQL_SaveNum(matCod))
            StrSQL.AppendLine("    AND Imprese_Contratto_Fasi.Elem_Cod = 210 --Elem_Cod Trasformati Vegetali")
            StrSQL.AppendLine("")

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

    Public Function readDettaglioAziendale(
                                 ByVal piva As String,
                                 ByVal matCod As Integer,
                                 ByVal idBudget As Integer,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal xFiltroAggiuntivo As String = ""
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.readContracts()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" WITH ICCertificazioni as ( ")
            StrSQL.AppendLine(" SELECT t.strName AS codice, ")
            StrSQL.AppendLine("        Piva ")
            StrSQL.AppendLine(" FROM   Imprese_Codici ")
            StrSQL.AppendLine(" CROSS apply dbo.fSplit(val_cod, ',') t ")
            StrSQL.AppendLine(" WHERE id_cod = 1341 ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Imprese_Certificazioni as ( ")
            StrSQL.AppendLine(" 	SELECT ICCertificazioni.PIVA as Piva,  STRING_AGG(CertificazioniAziendali.CA_des, ',') as Certificazioni ")
            StrSQL.AppendLine(" 	FROM ICCertificazioni ")
            StrSQL.AppendLine(" 	JOIN CertificazioniAziendali ON ICCertificazioni.codice = CertificazioniAziendali.CA_Cod ")
            StrSQL.AppendLine(" 	GROUP BY ICCertificazioni.Piva ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" Imprese_ContattiCte as ( ")
            StrSQL.AppendLine(" 	SELECT Imprese_Codici.PIVA, Contatti.Nome, Contatti.Cognome  ")
            StrSQL.AppendLine(" 	FROM Imprese_Codici ")
            StrSQL.AppendLine(" 	JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto  ")
            StrSQL.AppendLine(" 	WHERE Imprese_Codici.id_cod = 1088 ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" ImpresexProdottoxSuperficie as ( ")
            StrSQL.AppendLine(" SELECT impP.Piva, ")
            StrSQL.AppendLine("		impP.Mat_Cod, ")
            StrSQL.AppendLine("		SUM(reg.Sup_Imp) as Sup, ")
            StrSQL.AppendLine("		SUM(reg.Sup_Imp * impP.Produzione_Prevista) as Resa,")
            StrSQL.AppendLine("		AVG(impP.Produzione_Prevista) as Resa_Media ")
            StrSQL.AppendLine(" FROM Imprese_Progetti impP ")
            StrSQL.AppendLine(" JOIN Reg_Impianti reg ON impP.Piva = reg.PIVA  ")
            StrSQL.AppendLine(" 				AND impP.Sa_Cod = reg.SA_COD ")
            StrSQL.AppendLine(" 				AND impP.Appezza = reg.APPEZZA ")
            StrSQL.AppendLine(" 				AND impP.Id_Reg = reg.Id_Reg ")
            StrSQL.AppendLine(" GROUP BY impP.Piva, impP.Mat_Cod ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	CAST(ic_figlio.Contratto_Cod AS varchar(50)) +  '_' + CAST(icf_figlio.Fase_Cod  AS varchar(50)) as chiave, ")
            StrSQL.AppendLine(" 	ic_figlio.Contratto_Cod, ")
            StrSQL.AppendLine(" 	icf_figlio.Fase_Cod, ")
            StrSQL.AppendLine(" 	iFiglio.PIVA, ")
            StrSQL.AppendLine(" 	ic_figlio.Id_Budget, ")
            StrSQL.AppendLine(" 	cuaa.val_cod as Cuaa, ")
            StrSQL.AppendLine(" 	iFiglio.rag_soc, ")
            StrSQL.AppendLine(" 	icf_figlio.mat_cod, ")
            StrSQL.AppendLine(" 	Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des as Prodotto, ")
            StrSQL.AppendLine(" 	icf_figlio.Superficie, ")
            StrSQL.AppendLine(" 	icf_figlio.QtaPrevista, ")
            StrSQL.AppendLine(" 	icf_figlio.ResaPrevista, ")
            StrSQL.AppendLine(" 	ic_padre.Contratto_Cod, ")
            StrSQL.AppendLine(" 	ic_padre.Contratto_Numero, ")
            StrSQL.AppendLine(" 	ic_padre.PIVA as Piva_Azienda, ")
            StrSQL.AppendLine(" 	ic_padre.sa_cod as Sa_Cod_Azienda, ")
            StrSQL.AppendLine(" 	ic_padre.Fabbricato_Cod as Fabbricato_Cod_Azienda, ")
            StrSQL.AppendLine(" 	azienda.rag_soc as Rag_Soc_Azienda, ")
            StrSQL.AppendLine(" 	ca_azienda.sa_nome as Sa_Nome_Azienda, ")
            StrSQL.AppendLine(" 	fa_azienda.Fabbricato_Des as Fabbricato_Des_Azienda, ")

            StrSQL.AppendLine(" 	COALESCE(Gruppi_Raccolta.GruppoRaccolta_Cod, 0) as GruppoRaccolta_Cod, ")
            StrSQL.AppendLine(" 	COALESCE(Gruppi_Raccolta.GruppoRaccolta_Des, '') as GruppoRaccolta_Des, ")
            StrSQL.AppendLine(" 	COALESCE(Imprese_Certificazioni.Certificazioni, '') AS Certificazioni, ")
            StrSQL.AppendLine(" 	COALESCE(Imprese_ContattiCte.Cognome +' '+Imprese_ContattiCte.Nome, '') as Tecnico, ")
            StrSQL.AppendLine(" 	COALESCE(ImpresexProdottoxSuperficie.Sup, 0) as Sup, ")
            StrSQL.AppendLine(" 	COALESCE(ImpresexProdottoxSuperficie.Resa, 0) as Resa, ")
            StrSQL.AppendLine(" 	COALESCE(ImpresexProdottoxSuperficie.Resa_Media, 0) as Resa_Media, ")
            StrSQL.AppendLine(" 	(icf_figlio.Superficie/ImpresexProdottoxSuperficie.Sup*100) AS Percentuale_Superficie, ")
            StrSQL.AppendLine(" 	(icf_figlio.QtaPrevista/ImpresexProdottoxSuperficie.Resa*100) AS Percentuale_Qta, ")

            StrSQL.AppendLine(" 	iPadre.Piva as Piva_Padre, ")
            StrSQL.AppendLine(" 	iPadre.rag_soc as Rag_Soc_Padre ")

            StrSQL.AppendLine(" FROM Imprese_Contratti ic_figlio ")
            StrSQL.AppendLine(" 	JOIN Imprese_Contratto_Fasi icf_figlio ON ic_figlio.Contratto_Cod = icf_figlio.Contratto_Cod ")
            StrSQL.AppendLine(" 	JOIN Imprese_Contratti ic_padre ON ic_figlio.Contratto_Cod_Padre = ic_padre.Contratto_Cod ")
            StrSQL.AppendLine(" 	--JOIN Imprese_Contratto_Fasi icf_padre ON ic_padre.Contratto_Cod = icf_padre.Contratto_Cod AND icf_figlio.Fase_Cod_Padre = icf_padre.Fase_Cod_Padre ")
            StrSQL.AppendLine(" 	JOIN Imprese iFiglio ON ic_figlio.Piva = iFiglio.PIVA ")
            StrSQL.AppendLine(" 	JOIN Imprese_Codici cuaa ON iFiglio.PIVA = cuaa.PIVA AND cuaa.id_cod = 1010 ")
            StrSQL.AppendLine(" 	JOIN GerarchiaImprese ON iFiglio.PIVA = GerarchiaImprese.Figlio ")
            StrSQL.AppendLine(" 	JOIN Imprese iPadre ON iPadre.PIVA = GerarchiaImprese.Padre ")
            StrSQL.AppendLine(" 	JOIN Materie_Prime ON icf_figlio.Mat_Cod = Materie_Prime.Mat_Cod ")
            StrSQL.AppendLine(" 	JOIN Imprese azienda ON ic_padre.Piva = azienda.PIVA ")

            StrSQL.AppendLine("     LEFT JOIN Gruppi_Raccolta ON iFiglio.GruppoRaccolta_Cod = Gruppi_Raccolta.GruppoRaccolta_Cod ")
            StrSQL.AppendLine(" 	LEFT JOIN Imprese_Certificazioni ON iFiglio.PIVA = Imprese_Certificazioni.PIVA ")
            StrSQL.AppendLine(" 	LEFT JOIN Imprese_ContattiCte ON iFiglio.PIVA = Imprese_ContattiCte.PIVA ")
            StrSQL.AppendLine(" 	LEFT JOIN ImpresexProdottoxSuperficie ON iFiglio.PIVA = ImpresexProdottoxSuperficie.Piva AND icf_figlio.mat_cod = ImpresexProdottoxSuperficie.Mat_Cod ")

            StrSQL.AppendLine(" 	LEFT JOIN Centri_Aziendali ca_azienda ON ic_padre.Piva = ca_azienda.PIVA AND ic_padre.sa_cod = ca_azienda.sa_cod ")
            StrSQL.AppendLine(" 	LEFT JOIN Fabbricati fa_azienda ON ic_padre.Piva = fa_azienda.PIVA AND ic_padre.sa_cod = fa_azienda.sa_cod AND ic_padre.Fabbricato_Cod = fa_azienda.Fabbricato_Cod ")
            StrSQL.AppendLine(" 	WHERE ic_figlio.Cau_Contratto = '9301' ")

            If piva <> "" Then
                StrSQL.AppendLine(" 	AND ic_figlio.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idBudget <> -1 Then
                StrSQL.AppendLine(" 	AND ic_figlio.Id_Budget = " & Agro_SQL_SaveNum(idBudget) & " ")
            End If

            If matCod <> 0 Then
                StrSQL.AppendLine(" 	AND icf_figlio.Mat_Cod = " & Agro_SQL_SaveNum(matCod) & " ")
            End If

            If dataInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" 	AND ic_figlio.Validita_Fine >= " & Agro_SQL_SaveDateTime_NULL(dataInizio) & " ")
            End If

            If dataFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" 	AND ic_figlio.Validita_Inizio <= " & Agro_SQL_SaveDateTime_NULL(dataFine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function readXStampe(
                                 ByVal Cod_Contatto As String,
                                 ByVal dataInizio As Date,
                                 ByVal dataFine As Date,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_ContrattoxClausole_R.readXStampe()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("select IC.Contratto_Nome, ICF.QtaPrevista, ICF.Superficie, ICF.Tipo_Trasporto, IC.sa_cod, Indirizzi.* from Imprese_Contratti IC")
            StrSQL.AppendLine("INNER JOIN")
            StrSQL.AppendLine("Risorse_Umane on Risorse_Umane.Cod_RisUm = IC.Cod_Risum")
            StrSQL.AppendLine("INNER JOIN")
            StrSQL.AppendLine("Imprese_Contratto_Fasi ICF On ICF.fase_cod = IC.Fase_Cod_Padre")
            StrSQL.AppendLine("LEFT JOIN")
            StrSQL.AppendLine("CentrixIndirizzi ON CentrixIndirizzi.PIVA = IC.Piva AND CentrixIndirizzi.sa_cod = IC.sa_cod")
            StrSQL.AppendLine("LEFT JOIN")
            StrSQL.AppendLine("Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo")
            StrSQL.AppendFormat("Where risorse_umane.Cod_Contatto = '{0}'", Cod_Contatto)
            StrSQL.AppendLine("AND  Cau_Contratto = 9301")


            If dataInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" 	AND IC.Validita_Fine >= " & Agro_SQL_SaveDateTime_NULL(dataInizio) & " ")
            End If

            If dataFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" 	AND IC.Validita_Inizio <= " & Agro_SQL_SaveDateTime_NULL(dataFine) & " ")
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

Public Class Imprese_Contratto_Fasi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Contratto_Cod As Integer,
                           ByVal Fase_Cod As Long,
                           ByVal Fase_Des As String,
                           ByVal Data_Inizio_Prevista As Date,
                           ByVal Data_Fine_Prevista As Date,
                           ByVal Giudizio As String,
                           ByVal Importo As Double,
                           ByVal Elem_Cod As Integer,
                           ByVal Pro_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Cal_Cod As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal Udm_Cod As Integer,
                           ByVal Lotto As String, ByVal Qta As Double, ByVal Listino_Cod As Integer,
                           ByVal Valore1 As Double, ByVal Valore2 As Double, ByVal Valore3 As Double,
                           ByVal Valore4 As Double, ByVal Valore5 As Double, ByVal Valore6 As Double,
                           ByVal Valore7 As Double, ByVal Valore8 As Double, ByVal Valore9 As Double,
                           ByVal Valore1_2 As Double, ByVal Valore2_2 As Double, ByVal Valore3_2 As Double,
                           ByVal Data_Inizio_Prevista2 As Date,
                           ByVal Data_Fine_Prevista2 As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Superficie As Decimal,
                           ByVal QtaPrevista As Decimal,
                           ByVal ResaPrevista As Decimal,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratti_W.Scrivi()"

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

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Imprese_Contratto_Fasi ")
            StrSQL.Append("         ( Piva,          Contratto_Cod,         Fase_Cod, ")
            StrSQL.Append("           Fase_Des,      Data_Inizio_Prevista,  Data_Fine_Prevista, ")
            StrSQL.Append("           Giudizio,      Elem_Cod,              Pro_Cod,  ")
            StrSQL.Append("           Mat_Cod,       Progetto_Cod,          Cal_Cod, ")
            StrSQL.Append("           Udm_Cod,       Lotto,                 Qta, ")
            StrSQL.Append("           Importo,       Listino_Cod,          ")
            StrSQL.Append("           Valore1,       Valore2,               Valore3, ")
            StrSQL.Append("           Valore4,       Valore5,               Valore6, ")
            StrSQL.Append("           Valore7,       Valore8,               Valore9, ")
            StrSQL.Append("           Valore1_2,     Valore2_2,             Valore3_2,")
            StrSQL.Append("           ResaPrevista,     QtaPrevista,             Superficie,")
            StrSQL.Append("           Data_Inizio_Prevista2,  Data_Fine_Prevista2, ")


            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Contratto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Fase_Des) & "'  ")
            StrSQL.Append("        , " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            StrSQL.Append("        , " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Giudizio) & "'   ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Lotto) & "'  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Importo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Listino_Cod) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore1) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore3) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore4) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore5) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore6) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore7) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore8) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore9) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore1_2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore2_2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore3_2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ResaPrevista) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(QtaPrevista) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Superficie) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Inizio_Prevista2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Fine_Prevista2) & "  ")

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


    Public Function Modifica(ByVal Piva As String,
                             ByVal Contratto_Cod As Integer,
                             ByVal Fase_Cod As Long,
                             ByVal Fase_Des As String,
                             ByVal Data_Inizio_Prevista As Date,
                             ByVal Data_Fine_Prevista As Date,
                             ByVal Giudizio As String,
                             ByVal Importo As Double,
                             ByVal Elem_Cod As Integer,
                             ByVal Pro_Cod As Integer,
                             ByVal Mat_Cod As Integer,
                             ByVal Cal_Cod As Integer,
                             ByVal Progetto_Cod As Integer,
                             ByVal Udm_Cod As Integer,
                             ByVal Lotto As String, ByVal Qta As Double, ByVal Listino_Cod As Integer,
                             ByVal Valore1 As Double, ByVal Valore2 As Double, ByVal Valore3 As Double,
                             ByVal Valore4 As Double, ByVal Valore5 As Double, ByVal Valore6 As Double,
                             ByVal Valore7 As Double, ByVal Valore8 As Double, ByVal Valore9 As Double,
                             ByVal Valore1_2 As Double, ByVal Valore2_2 As Double, ByVal Valore3_2 As Double,
                             ByVal Data_Inizio_Prevista2 As Date,
                             ByVal Data_Fine_Prevista2 As Date,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal Superficie As Decimal,
                             ByVal QtaPrevista As Decimal,
                             ByVal ResaPrevista As Decimal,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratto_Fasi_W.Scrivi()"

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

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" Update Imprese_Contratto_Fasi ")
            StrSQL.Append(" Set ")

            StrSQL.Append("  Fase_Des = '" & Agro_SQL_SaveText(Fase_Des) & "'  ")
            StrSQL.Append(" ,Data_Inizio_Prevista = " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            StrSQL.Append(" ,Data_Fine_Prevista =  " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")
            StrSQL.Append(" ,Giudizio = '" & Agro_SQL_SaveText(Giudizio) & "'   ")

            StrSQL.Append(" ,Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append(" ,Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            StrSQL.Append(" ,Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append(" ,Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            StrSQL.Append(" ,Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            StrSQL.Append(" ,Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append(" ,Lotto = '" & Agro_SQL_SaveText(Lotto) & "'  ")

            StrSQL.Append(" ,Qta = " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append(" ,Importo = " & Agro_SQL_SaveNum(Importo) & "  ")
            StrSQL.Append(" ,Listino_Cod = " & Agro_SQL_SaveNum(Listino_Cod) & "  ")

            StrSQL.Append(" ,Valore1 = " & Agro_SQL_SaveNum(Valore1) & "  ")
            StrSQL.Append(" ,Valore2 = " & Agro_SQL_SaveNum(Valore2) & "  ")
            StrSQL.Append(" ,Valore3 = " & Agro_SQL_SaveNum(Valore3) & "  ")
            StrSQL.Append(" ,Valore4 = " & Agro_SQL_SaveNum(Valore4) & "  ")
            StrSQL.Append(" ,Valore5 = " & Agro_SQL_SaveNum(Valore5) & "  ")
            StrSQL.Append(" ,Valore6 = " & Agro_SQL_SaveNum(Valore6) & "  ")
            StrSQL.Append(" ,Valore7 = " & Agro_SQL_SaveNum(Valore7) & "  ")
            StrSQL.Append(" ,Valore8 = " & Agro_SQL_SaveNum(Valore8) & "  ")
            StrSQL.Append(" ,Valore9 = " & Agro_SQL_SaveNum(Valore9) & "  ")
            StrSQL.Append(" ,Superficie = " & Agro_SQL_SaveNum(Superficie) & "  ")
            StrSQL.Append(" ,QtaPrevista = " & Agro_SQL_SaveNum(QtaPrevista) & "  ")
            StrSQL.Append(" ,ResaPrevista = " & Agro_SQL_SaveNum(ResaPrevista) & "  ")

            StrSQL.Append(" ,Valore1_2 = " & Agro_SQL_SaveNum(Valore1_2) & "  ")
            StrSQL.Append(" ,Valore2_2 = " & Agro_SQL_SaveNum(Valore2_2) & "  ")
            StrSQL.Append(" ,Valore3_2 = " & Agro_SQL_SaveNum(Valore3_2) & "  ")

            StrSQL.Append(" ,Data_Inizio_Prevista2 = " & Agro_SQL_SaveDate(Data_Inizio_Prevista2) & "  ")
            StrSQL.Append(" ,Data_Fine_Prevista2 =  " & Agro_SQL_SaveDate(Data_Fine_Prevista2) & "  ")


            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            'If Contratto_Cod <> 0 Then
            StrSQL.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            ' End If

            'If Fase_Cod <> 0 Then
            StrSQL.Append(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            'End If

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Contratto_Cod As Int32,
                             ByVal Fase_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Fase_Cod = ""


        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Imprese_Contratto_Fasi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Imprese_Contratto_Fasi ")
                StrSQL.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            'If Contratto_Cod <> 0 Then
            StrSQL.Append(" AND Contratto_Cod = " & Agro_SQL_SaveNum(Contratto_Cod) & "   ")
            'End If

            If Fase_Cod <> 0 Then
                StrSQL.Append(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If


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

    Public Function writeRequisitiStabilimento(
                                              ByVal piva As String,
                                              ByVal sup As Decimal,
                                              ByVal qta As Decimal,
                                              ByVal trDes As String,
                                              ByVal trCod As Integer,
                                              ByVal contrattoCodPadre As Integer,
                                              ByVal faseCodPadre As Integer,
                                              ByVal contrattoCodFiglio As Integer,
                                              ByVal faseCodFiglio As Integer,
                                              ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              )

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratti_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT TOP(1) * FROM Imprese_Contratto_Fasi WHERE Contratto_Cod = " & contrattoCodPadre & " AND Fase_Cod = " & faseCodPadre)
            '-------------------------------------------------------------------------------------
            Dim dtFatherFase = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Imprese_Contratto_Fasi (")
            StrSQL.AppendLine("    Piva,                   Contratto_Cod,         Fase_Cod,")
            StrSQL.AppendLine("    Fase_Des,               Data_Inizio_Prevista,  Data_Fine_Prevista,")
            StrSQL.AppendLine("    Giudizio,               Elem_Cod,              Pro_Cod,")
            StrSQL.AppendLine("    Mat_Cod,                Progetto_Cod,          Cal_Cod,")
            StrSQL.AppendLine("    Udm_Cod,                Lotto,                 Qta,")
            StrSQL.AppendLine("    Importo,                Listino_Cod,")
            StrSQL.AppendLine("    Valore1,                Valore2,               Valore3,")
            StrSQL.AppendLine("    Valore4,                Valore5,               Valore6,")
            StrSQL.AppendLine("    Valore7,                Valore8,               Valore9,")
            StrSQL.AppendLine("    Valore1_2,              Valore2_2,             Valore3_2,")
            StrSQL.AppendLine("    ResaPrevista,           QtaPrevista,           Superficie,")
            StrSQL.AppendLine("    Fase_Cod_Padre ,")
            StrSQL.AppendLine("    Tipo_Trasporto ,")
            StrSQL.AppendLine("    Data_Inizio_Prevista2,  Data_Fine_Prevista2,")
            StrSQL.AppendLine("    Inviato,                DataInvio,")
            StrSQL.AppendLine("    Data_Creazione,         Data_Modifica,")
            StrSQL.AppendLine("    UserName_Creazione,     UserName_Modifica,")
            StrSQL.AppendLine("    Validita_Inizio,        Validita_Fine ")
            StrSQL.AppendLine(")")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("     '" & Agro_SQL_SaveText(piva) & "',")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(contrattoCodFiglio) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(faseCodFiglio) & ",")

            StrSQL.AppendLine("     '" & Agro_SQL_SaveText(dtFatherFase.Rows(0)("Fase_Des")) & "',")
            StrSQL.AppendLine("     " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Data_Inizio_Prevista")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Data_Fine_Prevista")) & ",")
            StrSQL.AppendLine("     '" & Agro_SQL_SaveText(dtFatherFase.Rows(0)("Giudizio")) & "',")

            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Elem_Cod")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Pro_Cod")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Mat_Cod")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Progetto_Cod")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Cal_Cod")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Udm_Cod")) & ",")
            StrSQL.AppendLine("     '" & Agro_SQL_SaveText(dtFatherFase.Rows(0)("Lotto")) & "',")

            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("qta")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Importo")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Listino_Cod")) & ",")

            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore1")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore2")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore3")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore4")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore5")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore6")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore7")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore8")) & ",")
            StrSQL.AppendLine("     " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore9")) & ",")

            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore1_2")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore2_2")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("Valore3_2")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(dtFatherFase.Rows(0)("ResaPrevista")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(qta) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(sup) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(faseCodPadre) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(trCod) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Data_Inizio_Prevista2")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Data_Fine_Prevista2")) & ",")

            StrSQL.AppendLine("    0,")
            StrSQL.AppendLine("    Null,")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(Date.Now) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(Date.Now) & ",")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Validita_Inizio")) & ",")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDate(dtFatherFase.Rows(0)("Validita_Fine")) & "")

            StrSQL.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function editRequisitiStabilimento(
                                             ByVal sup As Decimal,
                                             ByVal qta As Decimal,
                                             ByVal trDes As String,
                                             ByVal trCod As Integer,
                                             ByVal contrattoCodFiglio As Integer,
                                             ByVal faseCodFiglio As Integer,
                                             ByVal contrattoCodPadre As Integer,
                                             ByVal faseCodPadre As Integer,
                                             ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             )

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratto_Fase_W.editRequisitiStabilimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("Update Imprese_Contratto_Fasi ")
            StrSQL.AppendLine("Set ")
            StrSQL.AppendLine("    Superficie = " & Agro_SQL_SaveNum(sup) & ",")
            StrSQL.AppendLine("    QtaPrevista = " & Agro_SQL_SaveNum(qta) & ",")
            StrSQL.AppendLine("    Tipo_Trasporto = " & Agro_SQL_SaveNum(trCod) & "")
            'StrSQL.Append(" ,ResaPrevista = " & Agro_SQL_SaveNum(ResaPrevista) & "  ")

            StrSQL.AppendLine("WHERE 1 = 1")
            StrSQL.AppendLine("    AND Contratto_Cod = " & Agro_SQL_SaveNum(contrattoCodFiglio))
            StrSQL.AppendLine("    AND Fase_Cod = " & Agro_SQL_SaveNum(faseCodFiglio))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function deleteRequisitiStabilimento(
                                               ByVal contrattoCodFiglio As Integer,
                                               ByVal faseCodFiglio As Integer,
                                               ByVal contrattoCodPadre As Integer,
                                               ByVal faseCodPadre As Integer,
                                               ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               )

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Imprese_Contratti_W.deleteRequisitiStabilimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If objParametriServer.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.AppendLine("UPDATE Imprese_Contratto_Fasi")
                StrSQL.AppendLine("SET")
                StrSQL.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "',")
                StrSQL.AppendLine("    Inviato = -1")
                StrSQL.AppendLine("WHERE Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.AppendLine("DELETE")
                StrSQL.AppendLine("FROM Imprese_Contratto_Fasi")
                StrSQL.AppendLine("WHERE 1=1")
            End If

            StrSQL.AppendLine("    AND Contratto_Cod = " & Agro_SQL_SaveNum(contrattoCodFiglio))
            StrSQL.AppendLine("    AND Fase_Cod = " & Agro_SQL_SaveNum(faseCodFiglio))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
