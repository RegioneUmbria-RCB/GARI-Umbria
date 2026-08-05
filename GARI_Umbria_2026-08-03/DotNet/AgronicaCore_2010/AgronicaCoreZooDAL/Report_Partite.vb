Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Report_Partite_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function TrovaStallePerAziendaCentro(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.TrovaStallePerAziendaCentro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" Select * from Stalla ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '------------------------------------------------------------------

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

    Public Function StallaDescDaCodice(ByVal stallaCod As Integer,
                                       ByVal centro As Integer,
                                       ByVal piva As String,
                                       ByRef objParametriSever As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.StallaDescDaCodice()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" Select * from Stalla s")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            If piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(stallaCod) & " AND s.sa_cod = " & Agro_SQL_SaveNum(centro) & " ")

            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriSever, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriSever, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return If(DT.Rows.Count = 0, "TUTTE", DT.Rows(0).Item("STA_DES").ToString)

    End Function

    Public Function LeggiSintesiDet(ByVal DataInizio As Date,
                                 ByVal DataFine As Date,
                                 ByVal Lotto As String,
                                 ByVal Piva As String,
                                 ByVal Centro As String,
                                 ByVal Stalla As String,
                                 ByVal Sesso As String,
                                 ByVal Razza As String,
                                 ByVal FornitoreFatturazione As String,
                                 ByVal FornitoreProvenienza As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal soloLotti As Boolean = False,
                                 Optional ByVal dettaglio As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        DataFine = DataFine.AddDays(1).AddSeconds(-1)

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" --Acquisti pesature ")
            StrSQL.AppendLine(" Select Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, Max(a.des_lib) as tipo_movimento, Min(m.Data_Movimento) as data_operazione_Iniziale,  ")
            StrSQL.AppendLine(" Max(m.Data_Movimento) as data_operazione_Finale, AVG(md.qta) as Peso_Pagato,  ")
            StrSQL.AppendLine(" Case when AVG(md.qta) > 0 Then 100 * AVG(md.Qta - md.Qta_Dettaglio1) / AVG(md.qta) Else 0 End As Calo_Perc_Calcolato,  ")
            StrSQL.AppendLine(" count(md.Id_Mov_Det) as capi  ")
            'StrSQL.AppendLine(" Case When AVG(md.qta) > 0 Then MAX(mz.Costo_Totale)/AVG(md.qta) Else 0 End as costoAlKg, ")
            'StrSQL.AppendLine(" MAX(mz.Costo_Totale) as Costo_Totale ")
            StrSQL.AppendLine(" into #ACQUISTI_PESATURE ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")

            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" And cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" GROUP BY Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD; ")
            StrSQL.AppendLine("  ")

            StrSQL.AppendLine(" --acquisti carichi ")
            StrSQL.AppendLine(" Select Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, Max(a.des_lib) as tipo_movimento, Min(m.Data_Movimento) as data_operazione_Iniziale,   ")
            StrSQL.AppendLine(" Max(m.Data_Movimento) as data_operazione_Finale, ")
            StrSQL.AppendLine(" SUM(md.Prezzo_Unitario) as Costo_Totale, ")
            StrSQL.AppendLine(" AVG(md.Prezzo_Unitario) as Costo_Totale_Medio ")
            StrSQL.AppendLine(" into #ACQUISTI_CARICHI ")
            StrSQL.AppendLine(" from Agenda a  ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" GROUP BY Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD;  ")

            StrSQL.AppendLine(" --Macelli pesature ")
            StrSQL.AppendLine(" Select Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, Max(a.des_lib) as tipo_movimento, Min(m.Data_Movimento) as data_operazione_Iniziale,  ")
            StrSQL.AppendLine(" Max(m.Data_Movimento) as data_operazione_Finale, AVG(md.qta) as Peso_Vendita,  ")
            StrSQL.AppendLine(" Case when AVG(md.qta) > 0 Then 100 * AVG(md.Qta - md.Qta_Dettaglio1) / AVG(md.qta) Else 0 End As Calo_Perc_Calcolato,  ")
            StrSQL.AppendLine(" count(md.Id_Mov_Det) as capi ")
            'StrSQL.AppendLine(" , MAX(mz.Costo_Totale) as Ricavo_Totale, Case When MAX(mz.Kg_Pagati) > 0 Then MAX(mz.Costo_Totale)/MAX(mz.Kg_Pagati) Else 0 End as ricavoAlKg ")
            StrSQL.AppendLine(" into #MACELLI_PESATURE ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_mov = md.Id_Mov ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where Codice_Distinta <> '' and a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            StrSQL.AppendLine(" GROUP BY Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD; ")

            StrSQL.AppendLine(" --Macelli Scarichi")
            StrSQL.AppendLine(" Select Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, Max(a.des_lib) as tipo_movimento, Min(m.Data_Movimento) as data_operazione_Iniziale,  ")
            StrSQL.AppendLine(" Max(m.Data_Movimento) as data_operazione_Finale,   ")
            StrSQL.AppendLine(" SUM(md.Prezzo_Unitario) as Ricavo_Totale, ")
            StrSQL.AppendLine(" AVG(md.Prezzo_Unitario) as Ricavo_Totale_Medio ")
            StrSQL.AppendLine(" into #MACELLI_SCARICHI ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where Codice_Distinta <> '' and a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.SCARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            StrSQL.AppendLine(" GROUP BY Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD; ")

            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" Select ")

            If soloLotti Then
                StrSQL.AppendLine(" cteAC.Codice_Distinta As Lotto ")
            Else

                StrSQL.AppendLine(" cteAC.Codice_Distinta, cteAC.Sesso, cteAC.RAZ_COD, lra.RAZ_DES As Razza, cteAC.capi as Capi_In_Ingresso, cteMS.capi as Capi_In_Uscita, ")
                StrSQL.AppendLine(" cteAC.data_operazione_Iniziale as Primo_Arrivo, cteAC.data_operazione_Finale as Ultimo_Arrivo, ")
                StrSQL.AppendLine(" cteMS.data_operazione_Iniziale as Prima_Macellazione, cteMS.data_operazione_finale as Ultima_Macellazione, ")
                StrSQL.AppendLine(" DATEDIFF(day, cteAC.data_operazione_Iniziale, cteMS.data_operazione_Iniziale) as Presenza, ")
                StrSQL.AppendLine(" cteAP.Peso_Pagato, cteAP.Calo_Perc_Calcolato as Calo_Acquisto, cteMP.Peso_Vendita, cteMP.Calo_Perc_Calcolato as Calo_Vendita, ")
                StrSQL.AppendLine(" cteMP.Peso_Vendita - cteAP.Peso_Pagato As Incremento_Kg,  ")
                StrSQL.AppendLine(" (cteMP.Peso_Vendita - cteAP.Peso_Pagato) / DATEDIFF(day, cteAP.data_operazione_Iniziale, cteMP.data_operazione_Iniziale) as Incremento_Giornaliero, ")
                StrSQL.AppendLine(" (cteAC.Costo_Totale_Medio / cteAP.Peso_Pagato) As costoAlKg, (cteMS.Ricavo_Totale / cteMP.Peso_Vendita) as ricavoAlKg, ")
                StrSQL.AppendLine(" (cteMS.Ricavo_Totale - CteAC.Costo_Totale) / (DATEDIFF(day, cteAC.data_operazione_Iniziale, cteMS.data_operazione_Iniziale) * cteMC.capi) as Resa_Presenza, ")
                StrSQL.AppendLine(" (cteMS.Ricavo_Totale - CteAC.Costo_Totale) / (cteMP.Peso_Vendita - cteAP.Peso_Pagato) as ResaAlKg ")

            End If

            StrSQL.AppendLine(" from #ACQUISTI_CARICHI cteAC ")
            StrSQL.AppendLine(" left Join #MACELLI_PESATURE cteMP on cteMP.Codice_Distinta = cteAC.Codice_Distinta and cteAC.Sesso = cteMP.Sesso And cteAC.GEN_COD = cteMP.GEN_COD And cteAC.SPE_COD = cteMP.SPE_COD And cteAC.RAZ_COD = cteMP.RAZ_COD ")
            StrSQL.AppendLine(" left Join #ACQUISTI_PESATURE cteAP on cteAP.Codice_Distinta = cteAC.Codice_Distinta and cteAP.Sesso = cteAC.Sesso And cteAP.GEN_COD = cteAC.GEN_COD And cteAP.SPE_COD = cteAC.SPE_COD And cteAP.RAZ_COD = cteAC.RAZ_COD ")
            StrSQL.AppendLine(" left join #MACELLI_SCARICHI cteMS on cteMS.Codice_Distinta = cteMP.Codice_Distinta and cteMS.Sesso = cteMP.Sesso And cteMS.GEN_COD = cteMP.GEN_COD And cteMS.SPE_COD = cteMP.SPE_COD And cteMS.RAZ_COD = cteMP.RAZ_COD ")
            StrSQL.AppendLine(" left join Lista_Razze_Animali lra on lra.GEN_COD = cteAC.GEN_COD and lra.SPE_COD = cteAC.SPE_COD and lra.RAZ_COD = cteAC.RAZ_COD ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Lotto <> "" Then
                StrSQL.AppendLine(" And cteAC.Codice_Distinta = '" & Agro_SQL_SaveText(Lotto) & "' ")
            End If

            Dim tabellaDate As String = If(dettaglio, "cteAC", "cteMP")

            If DataInizio <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND " & tabellaDate & ".data_operazione_Iniziale >= " & Agro_SQL_SaveDate(DataInizio) & " ")
            End If

            If DataFine <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine(" AND " & tabellaDate & ".data_operazione_finale <= " & Agro_SQL_SaveDateTime(DataFine) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY " & If(soloLotti, "Lotto", "Codice_Distinta"))
            End If
            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            StrSQL.Length = 0
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#ACQUISTI') IS NOT NULL DROP TABLE #ACQUISTI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#ACQUISTI_Carichi') IS NOT NULL DROP TABLE #ACQUISTI_Carichi; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#MACELLO') IS NOT NULL DROP TABLE #MACELLO; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#MACELLO_Scarico') IS NOT NULL DROP TABLE #MACELLO_Scarico; ")
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        End Try

        Return DT

    End Function

    Public Function LeggiSintesi(ByVal DataInizio As Date,
                                 ByVal DataFine As Date,
                                 ByVal Lotto As String,
                                 ByVal Piva As String,
                                 ByVal Centro As String,
                                 ByVal Stalla As String,
                                 ByVal Sesso As String,
                                 ByVal Razza As String,
                                 ByVal FornitoreFatturazione As String,
                                 ByVal FornitoreProvenienza As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal soloLotti As Boolean = False,
                                 Optional ByVal dettaglio As Boolean = False,
                                 Optional ByVal statoPartite As Integer = 1
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        DataFine = DataFine.AddDays(1)

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" --Macello ")
            StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione, ")
            StrSQL.AppendLine(" md.qta as Peso_Vendita, md.Qta_Dettaglio1 As Peso_Uscita, za.Matricola, mz.Calo_Perc As calo_peso ")
            'StrSQL.AppendLine(" , MAX(mz.Costo_Totale) as Ricavo_Totale, Case When MAX(mz.Kg_Pagati) > 0 Then MAX(mz.Costo_Totale)/MAX(mz.Kg_Pagati) Else 0 End as ricavoAlKg ")
            StrSQL.AppendLine(" into #MACELLO ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_mov = md.Id_Mov ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine(" left join Movimenti_Zoo mz on m.PIVA = mz.Piva And m.Id_Agenda = mz.Id_Agenda And m.Id_Mov = mz.Id_Mov ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" AND m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & " ")
            StrSQL.AppendLine(" AND m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & " ")

            StrSQL.AppendLine(" --Macello Scarico")
            StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione, ")
            StrSQL.AppendLine(" md.Prezzo_Unitario as Ricavo_Totale, za.Matricola ")
            StrSQL.AppendLine(" into #MACELLO_Scarico ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.SCARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" AND m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & " ")
            StrSQL.AppendLine(" AND m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & " ")


            StrSQL.AppendLine(" --Acquisto ")
            StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione,  ")
            StrSQL.AppendLine(" md.qta as Peso_Pagato, md.Qta_Dettaglio1 As Peso_Arrivo, za.Matricola, mz.Calo_Perc As Calo_Peso")
            'StrSQL.AppendLine(" Case When AVG(md.qta) > 0 Then MAX(mz.Costo_Totale)/AVG(md.qta) Else 0 End as costoAlKg, ")
            'StrSQL.AppendLine(" MAX(mz.Costo_Totale) as Costo_Totale ")
            StrSQL.AppendLine(" into #ACQUISTI ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine(" left join Movimenti_Zoo mz on m.PIVA = mz.Piva And m.Id_Agenda = mz.Id_Agenda And m.Id_Mov = mz.Id_Mov ")
            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod ")
                StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod ")

            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" And cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" and zad.Codice_Distinta in (SELECT DISTINCT Codice_Distinta FROM #MACELLO_Scarico) ")

            StrSQL.AppendLine(" --acquisto carico ")
            StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione, ")
            StrSQL.AppendLine(" md.Prezzo_Unitario as Costo_Totale, za.Matricola ")
            StrSQL.AppendLine(" into #ACQUISTI_Carichi  ")
            StrSQL.AppendLine(" from Agenda a  ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod ")
                StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" and zad.Codice_Distinta in (SELECT DISTINCT Codice_Distinta FROM #MACELLO_Scarico) ")

            StrSQL.AppendLine(" --conteggi ")
            StrSQL.AppendLine(" Select cteMS.Codice_Distinta, cteMS.Sesso, cteMS.GEN_COD, cteMS.SPE_COD, cteMS.RAZ_COD, lra.RAZ_DES As Razza, ")
            StrSQL.AppendLine(" (SELECT COUNT(*) FROM #ACQUISTI_Carichi) as Capi_In_Ingresso, (SELECT COUNT(*) FROM #MACELLO_Scarico) as Capi_In_Uscita, ")
            StrSQL.AppendLine(" cteAC.data_operazione as Primo_Arrivo, cteAC.data_operazione as Ultimo_Arrivo, ")
            StrSQL.AppendLine(" cteMS.data_operazione as Prima_Macellazione, cteMS.data_operazione as Ultima_Macellazione, ")
            StrSQL.AppendLine(" DATEDIFF(day, cteAC.data_operazione, cteMS.data_operazione) as Presenza, ")
            StrSQL.AppendLine(" cteA.Peso_Pagato, cteA.Peso_Arrivo, cteM.Peso_Vendita, cteM.Peso_Uscita, ")
            StrSQL.AppendLine(" cteAC.Costo_Totale, cteMS.Ricavo_Totale, ")
            StrSQL.AppendLine(" cteM.Peso_Vendita - cteA.Peso_Pagato As Incremento_Kg, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(DATEDIFF(day, cteAC.data_operazione, cteMS.data_operazione), 0) <> 0 THEN (cteM.Peso_Vendita - cteA.Peso_Pagato) / DATEDIFF(day, cteAC.data_operazione, cteMS.data_operazione) ELSE 0 END as Incremento_Giornaliero, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(cteA.Peso_Pagato, 0) <> 0 THEN (cteAC.Costo_Totale / cteA.Peso_Pagato) ELSE 0 END As costoAlKg, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(cteM.Peso_Vendita, 0) <> 0 THEN (cteMS.Ricavo_Totale / cteM.Peso_Vendita) ELSE 0 END as ricavoAlKg, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(DATEDIFF(day, cteAC.data_operazione, cteMS.data_operazione) * (SELECT COUNT(*) FROM #MACELLO_Scarico), 0) <> 0 THEN (cteMS.Ricavo_Totale - CteAC.Costo_Totale) / (DATEDIFF(day, cteAC.data_operazione, cteMS.data_operazione) * (SELECT COUNT(*) FROM #MACELLO_Scarico)) ELSE 0 END As Resa_Presenza, ")
            StrSQL.AppendLine(" CASE WHEN ISNULL(cteM.Peso_Vendita - cteA.Peso_Pagato, 0) <> 0 THEN (cteMS.Ricavo_Totale - CteAC.Costo_Totale) / (cteM.Peso_Vendita - cteA.Peso_Pagato) ELSE 0 END as ResaAlKg, ")
            StrSQL.AppendLine(" cteMS.matricola, cteA.calo_peso As Calo_Acquisto, cteM.calo_peso As Calo_Vendita ")
            StrSQL.AppendLine(" into #CONTEGGI ")
            StrSQL.AppendLine(" FROM #MACELLO_Scarico cteMS ")
            StrSQL.AppendLine(" LEFT JOIN #MACELLO cteM on cteMS.Matricola = cteM.Matricola ")
            StrSQL.AppendLine(" left Join #ACQUISTI cteA on cteMS.Matricola = cteA.Matricola ")
            StrSQL.AppendLine(" left Join #ACQUISTI_Carichi cteAC on cteAC.Matricola = cteMS.Matricola ")
            StrSQL.AppendLine(" left join Lista_Razze_Animali lra on lra.GEN_COD = cteMS.GEN_COD and lra.SPE_COD = cteMS.SPE_COD and lra.RAZ_COD = cteMS.RAZ_COD ")
            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" --costi farmaci ")
            StrSQL.AppendLine(" SELECT zad.Codice_Distinta, dw.Cod_Animale_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, ")
            StrSQL.AppendLine(" za.Matricola, za.Cod_Progetto As cod_animale, dw.Valore ")
            StrSQL.AppendLine(" INTO #COSTI_FARMACI ")
            StrSQL.AppendLine(" From DW_CDG_Costi_Ricavi dw INNER Join CDG_Testata cdg_t ON dw.Id_CDG = cdg_t.Id_CDG ")
            StrSQL.AppendLine(" INNER Join Zoo_Animali za on dw.Cod_Animale = za.Cod_Progetto ")
            StrSQL.AppendLine(" INNER Join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" INNER JOIN #CONTEGGI cont ON za.Matricola = cont.matricola ")
            StrSQL.AppendLine(" WHERE cdg_t.Lav_Cod IN (" & LAVCOD_CUREMEDICAMENTI_ANIMALI & ")")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND dw.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND dw.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND dw.Sta_Num = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" --costi alimentazione ")
            StrSQL.AppendLine(" Select zad.Codice_Distinta, dw.Cod_Animale_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, ")
            StrSQL.AppendLine(" za.Matricola, za.Cod_Progetto As cod_animale, dw.Valore ")
            StrSQL.AppendLine(" INTO #COSTI_ALIMENTAZIONE ")
            StrSQL.AppendLine(" From DW_CDG_Costi_Ricavi dw INNER Join CDG_Testata cdg_t ON dw.Id_CDG = cdg_t.Id_CDG ")
            StrSQL.AppendLine(" INNER Join Zoo_Animali za on dw.Cod_Animale = za.Cod_Progetto ")
            StrSQL.AppendLine(" INNER Join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" INNER JOIN #CONTEGGI cont ON za.Matricola = cont.matricola ")
            StrSQL.AppendLine(" WHERE cdg_t.Lav_Cod IN (" & LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI & ") ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND dw.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND dw.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND dw.Sta_Num = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" ")

            If statoPartite < 1 Then

                StrSQL.AppendLine(" --SOLO Acquisto ")
                StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione,  ")
                StrSQL.AppendLine(" md.qta as Peso_Pagato, md.Qta_Dettaglio1 As Peso_Arrivo, za.Matricola, mz.Calo_Perc As Calo_Peso")
                'StrSQL.AppendLine(" Case When AVG(md.qta) > 0 Then MAX(mz.Costo_Totale)/AVG(md.qta) Else 0 End as costoAlKg, ")
                'StrSQL.AppendLine(" MAX(mz.Costo_Totale) as Costo_Totale ")
                StrSQL.AppendLine(" into #SOLO_ACQUISTI ")
                StrSQL.AppendLine(" from Agenda a ")
                StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
                StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
                StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
                StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
                StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
                StrSQL.AppendLine(" left join Movimenti_Zoo mz on m.PIVA = mz.Piva And m.Id_Agenda = mz.Id_Agenda And m.Id_Mov = mz.Id_Mov ")
                If Centro <> "" Then
                    StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
                End If

                If Stalla <> "" Then
                    StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod ")
                    StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod ")

                End If

                If FornitoreFatturazione <> "" Then
                    StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
                End If

                If FornitoreProvenienza <> "" Then
                    StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
                End If

                StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

                If FornitoreFatturazione <> "" Then
                    StrSQL.AppendLine(" And cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
                End If

                If FornitoreProvenienza <> "" Then
                    StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
                End If

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

                If Centro <> "" Then
                    StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
                End If

                If Stalla <> "" Then
                    StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
                End If

                If Sesso <> "" Then
                    StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
                End If

                If Razza <> "" Then
                    StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                    StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                    StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
                End If

                StrSQL.AppendLine(" and zad.Codice_Distinta NOT in (SELECT DISTINCT Codice_Distinta FROM #MACELLO_Scarico) ")

                StrSQL.AppendLine(" -- SOLO acquisto carico ")
                StrSQL.AppendLine(" Select zad.Codice_Distinta, za.Sesso, za.GEN_COD, za.SPE_COD, za.RAZ_COD, a.des_lib as tipo_movimento, m.Data_Movimento as data_operazione, ")
                StrSQL.AppendLine(" md.Prezzo_Unitario as Costo_Totale, za.Matricola ")
                StrSQL.AppendLine(" into #SOLO_ACQUISTI_Carichi  ")
                StrSQL.AppendLine(" from Agenda a  ")
                StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
                StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.id_Mov = md.Id_Mov  ")
                StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
                StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
                StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

                If Centro <> "" Then
                    StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
                End If

                If Stalla <> "" Then
                    StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod ")
                    StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod ")
                End If

                If FornitoreFatturazione <> "" Then
                    StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
                End If

                If FornitoreProvenienza <> "" Then
                    StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
                End If

                StrSQL.AppendLine(" where zad.Codice_Distinta <> '' and a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

                If FornitoreFatturazione <> "" Then
                    StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
                End If

                If FornitoreProvenienza <> "" Then
                    StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
                End If

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND a.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

                If Centro <> "" Then
                    StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
                End If

                If Stalla <> "" Then
                    StrSQL.AppendLine(" AND s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & " ")
                End If

                If Sesso <> "" Then
                    StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
                End If

                If Razza <> "" Then
                    StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                    StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                    StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
                End If

                StrSQL.AppendLine(" and zad.Codice_Distinta NOT in (SELECT DISTINCT Codice_Distinta FROM #MACELLO_Scarico) ")

                StrSQL.AppendLine(" --SOLO conteggi ")
                StrSQL.AppendLine(" Select cteAC.Codice_Distinta, cteAC.Sesso, cteAC.GEN_COD, cteAC.SPE_COD, cteAC.RAZ_COD, lra.RAZ_DES As Razza,  ")
                StrSQL.AppendLine(" (SELECT COUNT(*) FROM #SOLO_ACQUISTI_Carichi) As Capi_In_Ingresso, (Select COUNT(*) FROM #MACELLO_Scarico) As Capi_In_Uscita,  ")
                StrSQL.AppendLine(" cteAC.data_operazione as Primo_Arrivo, cteAC.data_operazione as Ultimo_Arrivo,  ")
                StrSQL.AppendLine(" '' as Prima_Macellazione, '' as Ultima_Macellazione,  ")
                StrSQL.AppendLine(" 0 as Presenza,  ")
                StrSQL.AppendLine(" cteA.Peso_Pagato, cteA.Peso_Arrivo, 0 AS Peso_Vendita, 0 AS Peso_Uscita,  ")
                StrSQL.AppendLine(" cteAC.Costo_Totale, 0 AS Ricavo_Totale,  ")
                StrSQL.AppendLine(" 0 As Incremento_Kg,  ")
                StrSQL.AppendLine(" 0 as Incremento_Giornaliero,  ")
                StrSQL.AppendLine(" CASE WHEN ISNULL(cteA.Peso_Pagato, 0) <> 0 THEN (cteAC.Costo_Totale / cteA.Peso_Pagato) ELSE 0 END As costoAlKg,  ")
                StrSQL.AppendLine(" 0 as ricavoAlKg,  ")
                StrSQL.AppendLine(" 0 As Resa_Presenza,  ")
                StrSQL.AppendLine(" 0 as ResaAlKg,  ")
                StrSQL.AppendLine(" cteAC.matricola, cteA.calo_peso As Calo_Acquisto, 0 As Calo_Vendita  ")
                StrSQL.AppendLine(" into #SOLO_APERTI_CONTEGGI  ")
                StrSQL.AppendLine(" FROM #SOLO_ACQUISTI_Carichi cteAC ")
                StrSQL.AppendLine(" left Join #SOLO_ACQUISTI cteA on cteAC.Matricola = cteA.Matricola  ")
                StrSQL.AppendLine(" left join Lista_Razze_Animali lra on lra.GEN_COD = cteAC.GEN_COD and lra.SPE_COD = cteAC.SPE_COD and lra.RAZ_COD = cteAC.RAZ_COD ")

            End If

            If statoPartite = 1 OrElse statoPartite = -1 Then

                StrSQL.AppendLine(" Select ")

                If soloLotti Then
                    StrSQL.AppendLine(" Codice_Distinta As Lotto ")
                Else
                    StrSQL.AppendLine(" con.Codice_Distinta, COUNT(DISTINCT con.matricola) as Capi_In_Ingresso, con.Sesso, con.GEN_COD, con.SPE_COD, con.RAZ_COD, con.Razza, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Presenza) / COUNT(DISTINCT con.matricola)) ELSE 0 END As Presenza, ")
                    StrSQL.AppendLine(" COUNT(DISTINCT con.matricola) as capi_in_Uscita, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Peso_Pagato)/COUNT(DISTINCT con.matricola)) ELSE 0 END as Peso_Pagato, ")
                    StrSQL.AppendLine(" AVG(con.Calo_Acquisto) As Calo_Acquisto, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Peso_Vendita)/COUNT(DISTINCT con.matricola)) ELSE 0 END as Peso_Vendita, ")
                    StrSQL.AppendLine(" AVG(con.Calo_Vendita) As Calo_Vendita, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Peso_Vendita) - SUM(con.Peso_Pagato))/COUNT(DISTINCT con.matricola) ELSE 0 END As Incremento_Kg, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(SUM(DATEDIFF(day, con.Primo_Arrivo, con.Prima_Macellazione)), 0) <> 0 THEN (SUM((con.Peso_Vendita - con.Peso_Pagato)) / SUM(DATEDIFF(day, con.Primo_Arrivo, con.Prima_Macellazione))) ELSE 0 END As Incremento_Giornaliero, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(SUM(con.Peso_Pagato), 0) <> 0 THEN (SUM(con.Costo_Totale) / SUM(con.Peso_Pagato)) ELSE 0 END As costoAlKg, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(SUM(con.Peso_Vendita), 0) <> 0 THEN (SUM(con.Ricavo_Totale) / SUM(con.Peso_Vendita)) ELSE 0 END As ricavoAlKg, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(SUM(con.Incremento_Kg), 0) <> 0 THEN ((SUM(con.Ricavo_Totale) - SUM(con.Costo_Totale)) / SUM(con.Incremento_Kg)) ELSE 0 END As ResaAlKg, ")
                    StrSQL.AppendLine(" CASE WHEN ISNULL(SUM(con.Presenza), 0) <> 0 THEN ((SUM(con.Ricavo_Totale) - SUM(con.Costo_Totale)) / SUM(con.Presenza)) ELSE 0 END As Resa_Presenza, ")
                    StrSQL.AppendLine(" MIN(con.Primo_Arrivo) As Primo_Arrivo, MAX(con.Ultimo_Arrivo) As Ultimo_Arrivo, ")
                    StrSQL.AppendLine(" MIN(con.Prima_Macellazione) As Prima_Macellazione, MAX(con.Ultima_Macellazione) As Ultima_Macellazione, ")
                    StrSQL.AppendLine(" SUM(cfar.Valore) AS Costi_Farmaci, SUM(cali.Valore) AS Costi_Alimentazione ")
                End If

                StrSQL.AppendLine(" FROM #CONTEGGI con LEFT JOIN #COSTI_FARMACI cfar ON con.matricola = cfar.matricola LEFT JOIN #COSTI_ALIMENTAZIONE cali ON con.matricola = cali.matricola ")
                StrSQL.AppendLine(" WHERE con.Codice_Distinta IS NOT NULL ")

                If Lotto <> "" Then
                    StrSQL.AppendLine(" And con.Codice_Distinta = '" & Agro_SQL_SaveText(Lotto) & "' ")
                End If

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                StrSQL.AppendLine(" GROUP BY con.Codice_Distinta, con.Sesso, con.GEN_COD, con.SPE_COD, con.RAZ_COD, con.Razza ")

            End If

            If statoPartite = 1 Then
                If xOrderBy <> "" Then
                    StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.AppendLine(" ORDER BY " & If(soloLotti, "Lotto", "Codice_Distinta"))
                End If
            End If

            If statoPartite = -1 Then
                StrSQL.AppendLine("  UNION  ")
            End If

            If statoPartite = -1 OrElse statoPartite = 0 Then
                StrSQL.AppendLine(" Select  ")
                StrSQL.AppendLine(" con.Codice_Distinta, COUNT(DISTINCT con.matricola) as Capi_In_Ingresso, con.Sesso, con.GEN_COD, con.SPE_COD, con.RAZ_COD, con.Razza,  ")
                StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Presenza) / COUNT(DISTINCT con.matricola)) ELSE 0 END As Presenza,  ")
                StrSQL.AppendLine(" COUNT(DISTINCT con.matricola) as capi_in_Uscita,  ")
                StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Peso_Pagato)/COUNT(DISTINCT con.matricola)) ELSE 0 END as Peso_Pagato,  ")
                StrSQL.AppendLine(" AVG(con.Calo_Acquisto) As Calo_Acquisto,  ")
                StrSQL.AppendLine(" CASE WHEN ISNULL(COUNT(DISTINCT con.matricola), 0) <> 0 THEN (SUM(con.Peso_Vendita)/COUNT(DISTINCT con.matricola)) ELSE 0 END as Peso_Vendita,  ")
                StrSQL.AppendLine(" AVG(con.Calo_Vendita) As Calo_Vendita,  ")
                StrSQL.AppendLine(" 0 As Incremento_Kg,  ")
                StrSQL.AppendLine(" 0 As Incremento_Giornaliero,  ")
                StrSQL.AppendLine(" 0 As costoAlKg,  ")
                StrSQL.AppendLine(" 0 As ricavoAlKg,  ")
                StrSQL.AppendLine(" 0 As ResaAlKg,  ")
                StrSQL.AppendLine(" 0 As Resa_Presenza,  ")
                StrSQL.AppendLine(" MIN(con.Primo_Arrivo) As Primo_Arrivo, MAX(con.Ultimo_Arrivo) As Ultimo_Arrivo,  ")
                StrSQL.AppendLine(" MIN(con.Prima_Macellazione) As Prima_Macellazione, MAX(con.Ultima_Macellazione) As Ultima_Macellazione,  ")
                StrSQL.AppendLine(" SUM(cfar.Valore) AS Costi_Farmaci, SUM(cali.Valore) AS Costi_Alimentazione  ")
                StrSQL.AppendLine(" FROM #SOLO_APERTI_CONTEGGI con LEFT JOIN #COSTI_FARMACI cfar ON con.matricola = cfar.matricola LEFT JOIN #COSTI_ALIMENTAZIONE cali ON con.matricola = cali.matricola  ")
                StrSQL.AppendLine(" WHERE con.Codice_Distinta IS NOT NULL  ")
                StrSQL.AppendLine(" GROUP BY con.Codice_Distinta, con.Sesso, con.GEN_COD, con.SPE_COD, con.RAZ_COD, con.Razza  ")

                StrSQL.AppendLine(" ORDER BY Codice_Distinta ")
            End If
            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            StrSQL.Length = 0
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#ACQUISTI') IS NOT NULL DROP TABLE #ACQUISTI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#ACQUISTI_Carichi') IS NOT NULL DROP TABLE #ACQUISTI_Carichi; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#SOLO_ACQUISTI') IS NOT NULL DROP TABLE #SOLO_ACQUISTI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#SOLO_ACQUISTI_Carichi') IS NOT NULL DROP TABLE #SOLO_ACQUISTI_Carichi; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#MACELLO') IS NOT NULL DROP TABLE #MACELLO; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#MACELLO_Scarico') IS NOT NULL DROP TABLE #MACELLO_Scarico; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#CONTEGGI') IS NOT NULL DROP TABLE #CONTEGGI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#SOLO_APERTI_CONTEGGI') IS NOT NULL DROP TABLE #SOLO_APERTI_CONTEGGI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#COSTI_FARMACI') IS NOT NULL DROP TABLE #COSTI_FARMACI; ")
            StrSQL.AppendLine(" IF OBJECT_ID('tempdb..#COSTI_ALIMENTAZIONE') IS NOT NULL DROP TABLE #COSTI_ALIMENTAZIONE; ")
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliPartite(ByVal Piva As String,
                                          ByVal DataInizio As Date,
                                          ByVal DataFine As Date,
                                          ByVal LottoDa As String,
                                          ByVal LottoA As String,
                                          ByVal Centro As String,
                                          ByVal Stalla As String,
                                          ByVal FornitoreFatturazione As String,
                                          ByVal FornitoreProvenienza As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.LeggiDettagliPartite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        DataFine = DataFine.AddDays(1).AddSeconds(-1)

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("     SELECT  ")
            StrSQL.AppendLine("         Codice_Distinta, m.Data_Movimento as Data_Operazione, a.piva, ")
            StrSQL.AppendLine("         raz.RAZ_DES, ")
            StrSQL.AppendLine("         za.Sesso, ")
            StrSQL.AppendLine("         MIN(m.Data_Movimento) As Data_operazione_Inizio, ")
            StrSQL.AppendLine("         MAX(m.Data_Movimento) As Data_operazione_Fine, ")
            StrSQL.AppendLine("         COUNT(*) AS Frequenza, ")
            StrSQL.AppendLine("         ROW_NUMBER() OVER ( ")
            StrSQL.AppendLine("             PARTITION BY Codice_Distinta  ")
            StrSQL.AppendLine("             ORDER BY COUNT(*) DESC ")
            StrSQL.AppendLine("         ) AS rn ")
            StrSQL.AppendLine(" into #CombinazioniFrequenza ")
            StrSQL.AppendLine("     FROM Agenda a ")
            StrSQL.AppendLine("     JOIN Movimenti m ON a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine("     JOIN Movimenti_dettagli md ON m.Id_Agenda = md.Id_Agenda and m.Id_mov = md.Id_Mov ")
            StrSQL.AppendLine("     JOIN Zoo_Animali za ON md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine("     JOIN Zoo_Animali_Distinte zad ON za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine("     JOIN Lista_Razze_Animali raz ON raz.GEN_COD = za.GEN_COD  ")
            StrSQL.AppendLine("         AND raz.SPE_COD = za.SPE_COD  ")
            StrSQL.AppendLine("         AND za.RAZ_COD = raz.RAZ_COD ")
            StrSQL.AppendLine(" left join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")

            If Centro <> "" Then
                StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
                StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")
            End If

            StrSQL.AppendLine("     WHERE Codice_Distinta <> ''  ")
            StrSQL.AppendLine("         AND a.lav_cod = 3034  ")
            StrSQL.AppendLine("         AND m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            StrSQL.AppendLine("     GROUP BY Codice_Distinta, raz.RAZ_DES, za.Sesso, m.Data_Movimento, a.PIVA ")
            StrSQL.AppendLine(" ; ")
            StrSQL.AppendLine("     SELECT  ")
            StrSQL.AppendLine("         Codice_Distinta, ")
            StrSQL.AppendLine("         COUNT(*) AS Frequenza ")
            StrSQL.AppendLine(" into #TotaliScarichi ")
            StrSQL.AppendLine("     FROM Agenda a ")
            StrSQL.AppendLine("     JOIN Movimenti m ON a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine("     JOIN Movimenti_dettagli md ON m.Id_Agenda = md.Id_Agenda and m.Id_mov = md.Id_Mov ")
            StrSQL.AppendLine("     JOIN Zoo_Animali za ON md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine("     JOIN Zoo_Animali_Distinte zad ON za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine("     WHERE Codice_Distinta <> ''  ")
            StrSQL.AppendLine("         AND a.lav_cod = 3004 ")
            StrSQL.AppendLine("         AND m.CAU_MOV = " & enum_Agenda_Causali.SCARICO_CONSISTENZE & " ")
            StrSQL.AppendLine("     GROUP BY Codice_Distinta ")
            StrSQL.AppendLine(" ; ")
            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine("     t.Codice_Distinta, ")
            StrSQL.AppendLine("     t.RAZ_DES AS Moda_RAZ_DES, ")
            StrSQL.AppendLine("     t.Sesso AS Moda_Sesso, ")
            StrSQL.AppendLine(" 	(Select SUM(Frequenza) from #CombinazioniFrequenza x where x.Codice_Distinta = t.Codice_Distinta) As Capi_Totali, ")
            StrSQL.AppendLine("     ts.Frequenza as scarichi ")
            StrSQL.AppendLine(" FROM #CombinazioniFrequenza t ")
            StrSQL.AppendLine(" Left JOIN #TotaliScarichi ts ON ts.Codice_Distinta = t.Codice_Distinta ")
            StrSQL.AppendLine(" where t.rn = 1 ")

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" And t.Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If DataInizio <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND data_operazione_inizio >= " & Agro_SQL_SaveDate(DataInizio) & " ")
            End If

            If DataFine <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine(" AND data_operazione_fine <= " & Agro_SQL_SaveDateTime(DataFine) & " ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Codice_Distinta")
            End If
            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            StrSQL.Length = 0
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#CombinazioniFrequenza') IS NOT NULL DROP TABLE #CombinazioniFrequenza;")
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#TotaliScarichi') IS NOT NULL DROP TABLE #TotaliScarichi;")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliCarichi(ByVal LottoDa As String,
                                          ByVal LottoA As String,
                                          ByVal Centro As String,
                                          ByVal Stalla As String,
                                          ByVal FornitoreFatturazione As String,
                                          ByVal FornitoreProvenienza As String,
                                          ByVal Razza As String,
                                          ByVal Sesso As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.LeggiDettagliCarichi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" Select DISTINCT Codice_Distinta, o.Lav_Des as tipo_movimento, s.STA_DES,  ")
            StrSQL.AppendLine(" m.Data_Movimento as data_operazione, md.qta as Peso_Pagato, za.Matricola, za.Sesso, md.Qta_Dettaglio1 as Peso_Arrivo,  ")
            StrSQL.AppendLine(" ISNULL(CASE WHEN cff.Rag_Soc IS NULL THEN (cff.Cognome + ' ' + cff.Nome) ELSE cff.rag_soc END, '') as Fornitore_Fatturazione, cff.Cod_Contatto as Cod_Fat, ")
            StrSQL.AppendLine(" ISNULL(CASE WHEN cfp.Rag_Soc IS NULL THEN (cfp.Cognome + ' ' + cfp.Nome) ELSE cfp.rag_soc END, '') as Fornitore_Provenienza, cfp.Cod_Contatto as Cod_Prov, ")
            StrSQL.AppendLine(" --Case When AVG(md.qta) > 0 Then MAX(mz.Costo_Totale)/AVG(md.qta) Else 0 End as costoAlKg , ")
            StrSQL.AppendLine(" --MAX(mz.Costo_Totale) as Costo_Totale ")
            StrSQL.AppendLine(" mz.Calo_Perc ")
            StrSQL.AppendLine(" into #ACQUISTI_PESATURE ")
            StrSQL.AppendLine(" from Agenda a ")
            StrSQL.AppendLine(" join Operazioni o on o.Lav_Cod = a.Lav_Cod ")
            StrSQL.AppendLine(" join Movimenti m on a.Id_Agenda = m.Id_Agenda ")
            StrSQL.AppendLine(" join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine(" join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto ")
            StrSQL.AppendLine(" join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine(" left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod ")
            StrSQL.AppendLine(" left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod ")
            StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")
            StrSQL.AppendLine(" left join Movimenti_Zoo mz on m.PIVA = mz.Piva And m.Id_Agenda = mz.Id_Agenda And m.Id_Mov = mz.Id_Mov ")
            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore And (Contatti.Piva = za.Piva Or Contatti.Sa_Cod = -1)) As cff ")
            StrSQL.AppendLine(" OUTER APPLY (Select TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza And (Contatti.Piva = za.Piva Or Contatti.Sa_Cod = -1)) As cfp ")

            StrSQL.AppendLine(" where a.lav_cod = 3034 And m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" And cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" ; ")

            StrSQL.AppendLine("  Select DISTINCT Codice_Distinta, o.Lav_Des as tipo_movimento, m.Data_Movimento as data_operazione, s.STA_DES, ")
            StrSQL.AppendLine("  md.Prezzo_Unitario as Costo, za.Matricola, za.Sesso,  ")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cff.Rag_Soc IS NULL THEN (cff.Cognome + ' ' + cff.Nome) ELSE cff.rag_soc END, '') as Fornitore_Fatturazione, cff.Cod_Contatto as Cod_Fat,")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cfp.Rag_Soc IS NULL THEN (cfp.Cognome + ' ' + cfp.Nome) ELSE cfp.rag_soc END, '') as Fornitore_Provenienza, cfp.Cod_Contatto as Cod_Prov ")
            StrSQL.AppendLine(" into #ACQUISTI_Carichi ")
            StrSQL.AppendLine("  from Agenda a  ")
            StrSQL.AppendLine("  join Operazioni o on o.Lav_Cod = a.Lav_Cod ")
            StrSQL.AppendLine("  join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
            StrSQL.AppendLine("  join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine("  join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
            StrSQL.AppendLine("  join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
            StrSQL.AppendLine("  join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
            StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            StrSQL.AppendLine("  left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")

            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")

            StrSQL.AppendLine("  where a.lav_cod = 3034 and m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" ; ")

            StrSQL.AppendLine(" Select  ")
            StrSQL.AppendLine(" ac.Codice_Distinta, ac.data_operazione as data_operazione, ac.sta_des As Allevamento_Entrata, ")
            StrSQL.AppendLine(" count(*) as Capi, ")
            StrSQL.AppendLine(" ac.Sesso as Sesso, ")
            StrSQL.AppendLine(" SUM(ac.Costo) as Costo_Totale, SUM(ap.Peso_Pagato) as Peso_Pagato, ")
            StrSQL.AppendLine(" SUM(ap.Peso_Arrivo) as Peso_Arrivo, ")
            StrSQL.AppendLine(" SUM(ac.Costo)/count(*) as Costo_Totale_Medio, SUM(ap.Peso_Pagato)/count(*) as Peso_Pagato_Medio, ")
            StrSQL.AppendLine(" SUM(ap.Peso_Arrivo)/count(*) as Peso_Arrivo_Medio, ")
            StrSQL.AppendLine(" AVG(ap.Calo_Perc) as Calo_Perc_Calcolato, ")
            StrSQL.AppendLine(" Case When SUM(ap.Peso_Pagato) > 0 Then SUM(ac.Costo)/SUM(ap.Peso_Pagato) Else 0 End as costoAlKg, ")
            StrSQL.AppendLine(" ac.Fornitore_Fatturazione, ac.Fornitore_Provenienza ")
            StrSQL.AppendLine(" from #ACQUISTI_Carichi ac ")
            StrSQL.AppendLine(" LEFT JOIN #ACQUISTI_PESATURE ap on ac.Codice_Distinta = ap.Codice_Distinta and ac.Matricola = ap.Matricola and ac.Sesso = ap.Sesso  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" Group by ac.Codice_Distinta, ac.data_operazione, ac.Sesso, ac.sta_des, ac.Fornitore_Fatturazione, ac.Fornitore_Provenienza ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Capi DESC")
            End If
            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            StrSQL.Length = 0
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#ACQUISTI_PESATURE') IS NOT NULL DROP TABLE #ACQUISTI_PESATURE;")
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#ACQUISTI_Carichi') IS NOT NULL DROP TABLE #ACQUISTI_Carichi;")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        End Try

        Return DT

    End Function

    Public Function LeggiDettagliScarichi(ByVal LottoDa As String,
                                          ByVal LottoA As String,
                                          ByVal Centro As String,
                                          ByVal Stalla As String,
                                          ByVal FornitoreFatturazione As String,
                                          ByVal FornitoreProvenienza As String,
                                          ByVal Razza As String,
                                          ByVal Sesso As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.LeggiDettagliScarichi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("     Select DISTINCT ")
            StrSQL.AppendLine("     Codice_Distinta, m.Data_Movimento as data_operazione, za.Matricola ")
            StrSQL.AppendLine(" into #Presenze ")
            StrSQL.AppendLine("                 From Agenda a ")
            StrSQL.AppendLine("     join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine("     join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine("                 Join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine("     join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine("  join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
            StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")

            StrSQL.AppendLine("     where a.lav_cod = 3034  ")
            StrSQL.AppendLine("     and m.CAU_MOV = " & enum_Agenda_Causali.CARICO_CONSISTENZE & " ")

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine("     ; ")
            StrSQL.AppendLine("         Select  ")
            StrSQL.AppendLine("         Codice_Distinta, ")
            StrSQL.AppendLine("         count(*) as Capi, ")
            StrSQL.AppendLine("         MAX(mz.Costo_Totale) as Costo_Totale, MAX(mz.Kg_Pagati) as Peso_Pagato ")
            StrSQL.AppendLine("     into #Costo ")
            StrSQL.AppendLine("         from Agenda a ")
            StrSQL.AppendLine("         join Movimenti m on a.Id_Agenda = m.Id_Agenda  ")
            StrSQL.AppendLine("         join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov ")
            StrSQL.AppendLine("         join Movimenti_Zoo mz on mz.Id_Mov = md.Id_Mov and mz.Id_Agenda = md.Id_Agenda ")
            StrSQL.AppendLine("         join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto  ")
            StrSQL.AppendLine("         join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine("  join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det ")
            StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
            StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")

            StrSQL.AppendLine("         where a.lav_cod = 3034  ")
            StrSQL.AppendLine("         and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & " ")

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine("         Group by Codice_Distinta ")
            StrSQL.AppendLine(" ; ")

            StrSQL.AppendLine("  Select DISTINCT Codice_Distinta, fabb.Fabbricato_Des as Macello, o.Lav_Des as tipo_movimento, m.Data_Movimento as data_operazione,  md.qta as Peso_Vendita,   ")
            StrSQL.AppendLine("  md.Qta_Dettaglio1 As Peso_Arrivo, za.Matricola, za.Sesso, s.sta_Des, ")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cff.Rag_Soc IS NULL THEN (cff.Cognome + ' ' + cff.Nome) ELSE cff.rag_soc END, '') as Fornitore_Fatturazione, cff.Cod_Contatto as Cod_Fat, ")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cfp.Rag_Soc IS NULL THEN (cfp.Cognome + ' ' + cfp.Nome) ELSE cfp.rag_soc END, '') as Fornitore_Provenienza, cfp.Cod_Contatto as Cod_Prov ")
            StrSQL.AppendLine(" into #MACELLO_Pesatura ")
            StrSQL.AppendLine("  from Agenda a  ")
            StrSQL.AppendLine("  join Operazioni o on o.Lav_Cod = a.Lav_Cod ")
            StrSQL.AppendLine("  join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
            StrSQL.AppendLine("  join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine("  join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
            StrSQL.AppendLine("  join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
            StrSQL.AppendLine("  join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det  ")
            StrSQL.AppendLine("  left JOIN Fabbricati fabb ON d.Piva = fabb.PIVA AND d.Sa_Cod = fabb.SA_COD AND d.Id_Destinazione = fabb.Fabbricato_Cod ")
            StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
            StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")

            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")

            StrSQL.AppendLine("  where a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.PESATURA & "  ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND zad.Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" ; ")

            StrSQL.AppendLine("  Select DISTINCT Codice_Distinta, fabb.Fabbricato_Des as Macello, o.Lav_Des as tipo_movimento, m.Data_Movimento as data_operazione,  ")
            StrSQL.AppendLine("  md.Prezzo_Unitario as Ricavo, za.Matricola, za.Sesso, s.STA_DES, ")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cff.Rag_Soc IS NULL THEN (cff.Cognome + ' ' + cff.Nome) ELSE cff.rag_soc END, '') as Fornitore_Fatturazione, cff.Cod_Contatto as Cod_Fat, ")
            StrSQL.AppendLine("  ISNULL(CASE WHEN cfp.Rag_Soc IS NULL THEN (cfp.Cognome + ' ' + cfp.Nome) ELSE cfp.rag_soc END, '') as Fornitore_Provenienza, cfp.Cod_Contatto as Cod_Prov ")
            StrSQL.AppendLine(" into #MACELLO_Scarico ")
            StrSQL.AppendLine("  from Agenda a  ")
            StrSQL.AppendLine("  join Operazioni o on o.Lav_Cod = a.Lav_Cod ")
            StrSQL.AppendLine("  join Movimenti m on a.Id_Agenda = m.Id_Agenda   ")
            StrSQL.AppendLine("  join Movimenti_dettagli md on m.Id_Agenda = md.Id_Agenda and m.Id_Mov = md.Id_Mov  ")
            StrSQL.AppendLine("  join Zoo_Animali za on md.Cod_Progetto = za.Cod_Progetto   ")
            StrSQL.AppendLine("  join Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale  ")
            StrSQL.AppendLine("  join Mov_Destinazioni d on d.Id_Mov_Det = md.Id_Mov_Det  ")
            StrSQL.AppendLine("  left JOIN Fabbricati fabb ON d.Piva = fabb.PIVA AND d.Sa_Cod = fabb.SA_COD AND d.Id_Destinazione = fabb.Fabbricato_Cod ")
            StrSQL.AppendLine("  left join Stalla_Raggruppamenti sr on d.Id_Destinazione = sr.Raggruppamento_Cod  ")
            StrSQL.AppendLine("  left join Stalla s on sr.STA_NUM = s.STA_NUM AND sr.PIVA = s.PIVA and sr.sa_cod = s.sa_cod  ")
            StrSQL.AppendLine(" left join Centri_Aziendali caz on d.Sa_Cod = caz.Sa_Cod and d.Piva = caz.Piva ")

            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.CF_Fornitore AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cff ")
            StrSQL.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Contatti (NoLock) WHERE Contatti.Cod_Contatto = za.Fornitore_Provenienza AND (Contatti.Piva = za.Piva OR Contatti.Sa_Cod = -1)) as cfp ")

            StrSQL.AppendLine("  where a.lav_cod = 3004 and m.CAU_MOV = " & enum_Agenda_Causali.SCARICO_CONSISTENZE & " ")

            If FornitoreFatturazione <> "" Then
                StrSQL.AppendLine(" AND cff.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreFatturazione) & "' ")
            End If

            If FornitoreProvenienza <> "" Then
                StrSQL.AppendLine(" AND cfp.Cod_Contatto = '" & Agro_SQL_SaveText(FornitoreProvenienza) & "' ")
            End If

            If LottoDa <> "" AndAlso LottoA <> "" Then
                StrSQL.AppendLine(" AND zad.Codice_Distinta BETWEEN '" & Agro_SQL_SaveText(LottoDa) & "' AND '" & Agro_SQL_SaveText(LottoA) & "' ")
            End If

            If Centro <> "" Then
                StrSQL.AppendLine(" AND caz.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND (s.STA_NUM = " & Agro_SQL_SaveNum(Stalla) & ") ")
            End If

            If Sesso <> "" Then
                StrSQL.AppendLine(" AND za.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            End If

            If Razza <> "" Then
                StrSQL.AppendLine(" AND za.GEN_COD = " & Agro_SQL_SaveNum(Razza.Split("-").First) & " ")
                StrSQL.AppendLine(" AND za.SPE_COD = " & Agro_SQL_SaveNum(Razza.Split("-")(1)) & " ")
                StrSQL.AppendLine(" AND za.RAZ_COD = " & Agro_SQL_SaveNum(Razza.Split("-").Last) & " ")
            End If

            StrSQL.AppendLine(" ; ")

            StrSQL.AppendLine(" Select  ")
            StrSQL.AppendLine(" ms.Codice_Distinta, ms.data_operazione as data_operazione, ms.sta_des As Allevamento_Uscita, MAX(ms.tipo_movimento) As tipologia, ")
            StrSQL.AppendLine(" count(*) as Capi, ")
            StrSQL.AppendLine(" ms.Sesso as Sesso,  ")
            StrSQL.AppendLine(" SUM(ms.Ricavo) as Ricavo_Totale, SUM(mp.Peso_Vendita) as Peso_Vendita_Totale, ")
            StrSQL.AppendLine(" SUM(mp.Peso_Arrivo) as Peso_Netto_Totale, ")
            StrSQL.AppendLine(" SUM(ms.Ricavo)/count(*) as Ricavo_Medio, SUM(mp.Peso_Vendita)/count(*) as Peso_Vendita_Medio, ")
            StrSQL.AppendLine(" SUM(mp.Peso_Arrivo)/count(*) as Peso_Netto_Medio, ")
            StrSQL.AppendLine(" Case when SUM(mp.Peso_Arrivo) > 0 Then 100 * (SUM(mp.Peso_Arrivo) - SUM(mp.Peso_Vendita)) / SUM(mp.Peso_Arrivo) Else 0 End As Calo_Perc_Calcolato,  ")
            StrSQL.AppendLine(" Case When SUM(mp.Peso_Vendita) > 0 Then SUM(ms.Ricavo)/SUM(mp.Peso_Vendita) Else 0 End as ricavoAlKg, ")
            StrSQL.AppendLine(" ISNULL(ms.Macello, '') as Macello, MAX(cc.Costo_Totale) As Costo_Totale, MAX(cc.Peso_Pagato) As Peso_Pagato, ")
            StrSQL.AppendLine(" AVG(DATEDIFF(day, p.data_operazione, ms.data_operazione) + 1) as Presenze, ")
            StrSQL.AppendLine(" ms.Fornitore_Fatturazione, ms.Fornitore_Provenienza ")
            StrSQL.AppendLine(" from #MACELLO_Scarico ms ")
            StrSQL.AppendLine(" left Join #MACELLO_Pesatura mp on ms.Codice_Distinta = mp.Codice_Distinta And ms.Matricola = mp.Matricola And ms.Sesso = mp.Sesso ")
            StrSQL.AppendLine(" left join #Presenze p on p.Codice_Distinta = ms.Codice_Distinta and ms.Matricola = p.Matricola ")
            StrSQL.AppendLine(" left join #Costo cc on cc.Codice_Distinta = ms.Codice_Distinta ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" Group by ms.Codice_Distinta, ms.data_operazione, ms.Sesso, ms.sta_des, ms.Fornitore_Fatturazione, ms.Fornitore_Provenienza, ms.Macello ")
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" order by Capi DESC ")
            End If
            '------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            StrSQL.Length = 0
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#Presenze') IS NOT NULL DROP TABLE #Presenze;")
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#Costo') IS NOT NULL DROP TABLE #Costo;")
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#MACELLO_Pesatura') IS NOT NULL DROP TABLE #MACELLO_Pesatura;")
            StrSQL.AppendLine("IF OBJECT_ID('tempdb..#MACELLO_Scarico') IS NOT NULL DROP TABLE #MACELLO_Scarico;")
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        End Try

        Return DT

    End Function

    Public Function LeggiCostiAlimentazionePartite(ByVal Partite As List(Of String),
                                                    ByVal Piva As String,
                                                    ByVal Centro As String,
                                                    ByVal Stalla As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.LeggiCostiAlimentazionePartite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT zad.Codice_Distinta, SUM(dw.Valore) As Costo_Alimentare ")
            StrSQL.AppendLine(" FROM DW_CDG_Costi_Ricavi dw INNER JOIN CDG_Testata cdg_t ON dw.Id_CDG = cdg_t.Id_CDG ")
            StrSQL.AppendLine(" INNER JOIN Zoo_Animali za on dw.Cod_Animale = za.Cod_Progetto ")
            StrSQL.AppendLine(" INNER JOIN Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" WHERE zad.Codice_Distinta IN ('" & String.Join("','", Partite.Select(Function(p) Agro_SQL_SaveText(p))) & "') ")
            StrSQL.AppendLine(" AND cdg_t.Lav_Cod IN (" & LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI & ", " &
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI & ") ")
            StrSQL.AppendLine(" AND dw.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Centro <> "" Then
                StrSQL.AppendLine(" AND dw.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND dw.Sta_Num = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" GROUP BY zad.Codice_Distinta ")

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

    Public Function LeggiCostiFarmaciPartite(ByVal Partite As List(Of String),
                                                    ByVal Piva As String,
                                                    ByVal Centro As String,
                                                    ByVal Stalla As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Report_Partite_R.LeggiCostiFarmaciPartite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT zad.Codice_Distinta, SUM(dw.Valore) As Costo_Farmaci")
            StrSQL.AppendLine(" FROM DW_CDG_Costi_Ricavi dw INNER JOIN CDG_Testata cdg_t ON dw.Id_CDG = cdg_t.Id_CDG ")
            StrSQL.AppendLine(" INNER JOIN Zoo_Animali za on dw.Cod_Animale = za.Cod_Progetto ")
            StrSQL.AppendLine(" INNER JOIN Zoo_Animali_Distinte zad on za.Cod_Progetto = zad.Cod_Animale ")
            StrSQL.AppendLine(" WHERE zad.Codice_Distinta IN ('" & String.Join("','", Partite.Select(Function(p) Agro_SQL_SaveText(p))) & "') ")
            StrSQL.AppendLine(" And cdg_t.Lav_Cod IN (" & LAVCOD_CUREMEDICAMENTI_ANIMALI & ")")
            StrSQL.AppendLine(" AND dw.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Centro <> "" Then
                StrSQL.AppendLine(" AND dw.Sa_Cod = " & Agro_SQL_SaveNum(Centro) & " ")
            End If

            If Stalla <> "" Then
                StrSQL.AppendLine(" AND dw.Sta_Num = " & Agro_SQL_SaveNum(Stalla) & " ")
            End If

            StrSQL.AppendLine(" GROUP BY zad.Codice_Distinta ")

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
