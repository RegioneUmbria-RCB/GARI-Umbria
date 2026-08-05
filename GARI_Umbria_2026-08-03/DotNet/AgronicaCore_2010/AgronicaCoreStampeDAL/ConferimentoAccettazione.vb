Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

'/******************************************************************************
'/******************************************************************************
'/******************************************************************************
' CLASSE USATA DAI REPORT IBRIDI CONFERIMENTO LIKE AGRISFERA E F&F SU GIASLAN
' PER I REPORT F&F WEB, UTILIZZARE LA CLASSE FreshAndFood.vb
'/******************************************************************************
'/******************************************************************************
'/******************************************************************************


Public Class ConferimentoAccettazione
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function SaldoImballi(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal Mat_Cod As Integer,
                            ByVal Cod_RisUm As Integer,
                             ByVal FiltroAggConferenti As String,
                            ByVal Data_Inizio As Date,
                            ByVal Data_Fine As Date,
                            ByVal Data_Giacenza As Date,
                            ByVal Cod_Rapporto As Integer,
                            ByVal Dettaglio_Stabilimento As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        '   ByVal Codice_ConfCli As String,
        'ByVal FinestraTemp_Inizio As Date, _
        'ByVal FinestraTemp_Fine As Date, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.SaldoImballi"
        Dim DT As DataTable
        Dim str_SaldiImballi As String
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New StringBuilder


        Try

            str_SaldiImballi = SaldoImballiQueryStr(Piva,
                                       Sa_Cod,
                                       Fabbricato_Cod,
                                       Mat_Cod,
                                       Cod_RisUm,
                                       FiltroAggConferenti,
                                       Data_Inizio,
                                       Data_Fine,
                                       Data_Giacenza,
                                       "",
                                       Cod_Rapporto,
                                       Dettaglio_Stabilimento,
                                       xFiltroAggiuntivo, "",
                                       int_Configurazione_Moduli,
                                       objParametri)


            StbSQL.Length = 0
            StbSQL.Append(" SELECT Piva_ConfCli, Codice_ConfCli, RagSoc_ConfCli, " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL.Append("   Codice_Stab, RagSoc_Stab, " & vbCrLf)
            End If
            StbSQL.Append(" Cod_Articolo, Mat_Des, SUM(Qta_Totale_Carichi) as Qta_Totale_Carichi, SUM(Qta_Totale_Scarichi) AS Qta_Totale_Scarichi , SUM(Giacenza) AS Giacenza " & vbCrLf)

            StbSQL.Append(" FROM " & vbCrLf)

            StbSQL.Append(" ( " & vbCrLf)

            StbSQL.Append(str_SaldiImballi)

            StbSQL.Append(" " & vbCrLf)
            StbSQL.Append(" " & vbCrLf)
            StbSQL.Append(" ) SALDI_IMBALLI " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL.Append(" GROUP BY Piva_ConfCli, Codice_ConfCli, RagSoc_ConfCli, Codice_Stab, RagSoc_Stab, Cod_Articolo, Mat_Des  " & vbCrLf)
            Else
                StbSQL.Append(" GROUP BY Piva_ConfCli, Codice_ConfCli, RagSoc_ConfCli, Cod_Articolo, Mat_Des  " & vbCrLf)
            End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                If Dettaglio_Stabilimento Then
                    StbSQL.Append(" ORDER BY  RagSoc_ConfCli, RagSoc_Stab, Mat_Des ")
                Else
                    StbSQL.Append(" ORDER BY  RagSoc_ConfCli, Mat_Des ")
                End If
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    Private Function SaldoImballiQueryStr(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Fabbricato_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cod_RisUm As Integer,
                                            ByVal FiltroAggConferenti As String,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Data_Giacenza As Date,
                                            ByVal Codice_ConfCli As String,
                                            ByVal Cod_Rapporto As Integer,
                                            ByVal Dettaglio_Stabilimento As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As System.String

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.SaldoImballiQueryStr"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New StringBuilder
        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_from_movcont As New StringBuilder
        Dim StbSQL_from_raccolta As New StringBuilder
        Dim StbSQL_where_movcont As New StringBuilder
        Dim StbSQL_where_raccolta As New StringBuilder
        Dim StbSQL_where As New StringBuilder
        Dim StbSQL_groupby_heaving As New StringBuilder
        Dim StbSQL_orderby As New StringBuilder


        Dim str_SaldiImballi As String

        Try
            StbSQL.Length = 0
            StbSQL_select.Length = 0
            StbSQL_from_movcont.Length = 0
            StbSQL_from_raccolta.Length = 0
            StbSQL_where_movcont.Length = 0
            StbSQL_where_raccolta.Length = 0
            StbSQL_where.Length = 0
            StbSQL_groupby_heaving.Length = 0
            StbSQL_orderby.Length = 0


            StbSQL_select.Append(" SELECT  Contatti_ConfCli.Cod_Contatto AS Piva_ConfCli, " & vbCrLf)
            StbSQL_select.Append("         Risorse_Umane_Conferenti.Settore_Des AS Codice_ConfCli, " & vbCrLf)
            StbSQL_select.Append("         Contatti_ConfCli.Rag_Soc AS RagSoc_ConfCli, " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_select.Append("   ISNULL( Contatti_Dest.Cod_Contatto, Contatti_ConfCli.Cod_Contatto) AS Codice_Stab, ISNULL( Contatti_Dest.Rag_Soc, Contatti_ConfCli.Rag_Soc) + ' ' + ISNULL(ind_des, '') AS RagSoc_Stab, " & vbCrLf)
            End If
            StbSQL_select.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des " & vbCrLf)

            StbSQL_select.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            StbSQL_select.Append("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL_select.Append("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL_select.Append("                 THEN        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL_select.Append("                 ELSE 0 " & vbCrLf)
            StbSQL_select.Append("                 END) AS Qta_Totale_Carichi " & vbCrLf)

            StbSQL_select.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL_select.Append("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL_select.Append("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL_select.Append("                 THEN        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL_select.Append("                 ELSE 0 " & vbCrLf)
            StbSQL_select.Append("                 END) AS Qta_Totale_Scarichi " & vbCrLf)

            StbSQL_select.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL_select.Append("                 THEN        -(Mov_Dest_Magazzino.qta) " & vbCrLf)
            StbSQL_select.Append("                 ELSE        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL_select.Append("                 END) AS Giacenza " & vbCrLf)


            StbSQL_from_movcont.Append(" FROM Agenda " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_from_movcont.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_from_movcont.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_from_movcont.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_from_raccolta.Append(" FROM Agenda                                                                                                    " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Mov_Dettagli_Riferimenti Agenda_Rif ON (Agenda.PIVA = Agenda_Rif.Piva AND Agenda.Id_Agenda = Agenda_Rif.Id_Agenda )      " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda_Rif.Piva_Rif = Mov_Cont.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda_Rif.Piva_Rif = Mov_Magazzino.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Magazzino.Id_Agenda   " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov                                                                                                                                                                    " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det                          " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod                                                                                                                                                                                                                                        " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Movimenti Movimenti_Umani  ON Agenda.Piva = Movimenti_Umani.PIVA AND Agenda.Id_Agenda = Movimenti_Umani.Id_Agenda  " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Risorse_Umane  ON Movimenti_Umani.Cod_RisUm = Risorse_Umane.Cod_RisUm                                                                                                                                                                                                                                                                                                    " & vbCrLf)
            StbSQL_from_raccolta.Append(" INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane.Piva = Contatti_ConfCli.Piva AND Risorse_Umane.Cod_Contatto = Contatti_ConfCli.Cod_Contatto                                                                                                                                                                                                                                   " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_from_raccolta.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_from_raccolta.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_from_raccolta.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_where_movcont.Append(" WHERE     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)

            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                If Cod_Rapporto = 0 Then
                    '15/06/2020: gestito cod_rapporto -11 personalizzato di Fruttagel
                    StbSQL_where_movcont.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," &
                                                Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTE) & "," &
                                              Agro_SQL_SaveNum(COD_FORNITORE) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
                End If
            End If

            StbSQL_where_raccolta.Append(" WHERE     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_RILIEVO_RACCOLTA) & "'" & vbCrLf)

            StbSQL_where.Append(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL_where.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQL_where.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_RisUm = " & Agro_SQL_SaveText(Cod_RisUm) & " " & vbCrLf)
            End If
            If Codice_ConfCli <> "" Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_ConfCli) & "' " & vbCrLf)
            End If
            '04/02/2021
            If FiltroAggConferenti <> "" Then
                'tale filtro utilizza l'alias tabella Mov_Accett, faccio replace per Risorse_Umane_Conferenti
                FiltroAggConferenti = Replace(FiltroAggConferenti, "Mov_Accett", "Risorse_Umane_Conferenti")
                StbSQL_where.AppendLine(FiltroAggConferenti)
            End If

            '13/06/2017:aggiunto x richiesta cofruta
            If Cod_Rapporto <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
            End If

            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            If Mat_Cod <> 0 Then
                StbSQL_where.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_where.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            'si leggono solo gli imballi
            StbSQL_where.Append(" AND MP_Imballi.chkimballaggio=1 ")
            '--------------------------------------------------------------------------

            StbSQL_groupby_heaving.Append(" GROUP BY Contatti_ConfCli.Cod_Contatto, Risorse_Umane_Conferenti.Settore_Des, " & vbCrLf)
            StbSQL_groupby_heaving.Append("         Contatti_ConfCli.Rag_Soc, " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_groupby_heaving.Append(" ind_des, Contatti_Dest.Cod_Contatto, Contatti_Dest.Rag_Soc,  " & vbCrLf)
            End If
            StbSQL_groupby_heaving.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des " & vbCrLf)
            'condizione per filtrare le giacenze=0
            StbSQL_groupby_heaving.Append(" HAVING SUM(CASE WHEN Mov_Magazzino.Cau_Mov= '" & Agro_SQL_SaveText(CAU_SCARICO) & "' THEN -(Mov_Dest_Magazzino.qta) ELSE Mov_Dest_Magazzino.qta END) <> 0 " & vbCrLf)

            'If xOrderBy <> "" Then
            '    StbSQL_orderby.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    StbSQL_orderby.Append(" ORDER BY  RagSoc_ConfCli, MP_Imballi.Mat_Des ")
            'End If


            '**********************************************************
            '******************** QUERY FINALE  ***********************
            '**********************************************************
            StbSQL.Append(StbSQL_select.ToString & StbSQL_from_movcont.ToString & StbSQL_where_movcont.ToString & StbSQL_where.ToString & StbSQL_groupby_heaving.ToString)

            'Nei casi No F&F i dati degli imballi si trovano nei movimenti di raccolta (cau_mov = 2200),
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" -- query NO F&F ")
                StbSQL.Append(" UNION ALL ")
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from_raccolta.ToString & StbSQL_where_raccolta.ToString & StbSQL_where.ToString & StbSQL_groupby_heaving.ToString)
            End If

            '--------------------------------------------------------------------------
            'DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            str_SaldiImballi = StbSQL.ToString
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'DT = Nothing
            str_SaldiImballi = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        'Return DT
        Return str_SaldiImballi

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report estratto conto imballi: nuova query ottimizzata
    ''' Default:
    ''' Optional ByVal FinestraTemp_Inizio As Date = agrodatainizio
    '''Optional ByVal FinestraTemp_Fine As Date = agrodatafine
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EstrattoConto_Imballi(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Fabbricato_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cod_RisUm As Integer,
                                            ByVal FiltroAggConferenti As String,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Data_Saldo_Precedente As Date,
                                            ByVal Cod_Rapporto As Integer,
                                            ByVal Dettaglio_Stabilimento As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.EstrattoConto_Imballi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New StringBuilder

        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_from_movcont As New StringBuilder
        Dim StbSQL_from_raccolta As New StringBuilder
        Dim StbSQL_where_movcont As New StringBuilder
        Dim StbSQL_where_raccolta As New StringBuilder
        Dim StbSQL_where As New StringBuilder
        'SB per Giacenze attuali
        Dim StbSQL_ga_select As New StringBuilder
        Dim StbSQL_ga_from_movcont As New StringBuilder
        Dim StbSQL_ga_from_raccolta As New StringBuilder
        Dim StbSQL_ga_where_movcont As New StringBuilder
        Dim StbSQL_ga_where_raccolta As New StringBuilder
        Dim StbSQL_ga_where As New StringBuilder
        Dim StbSQL_ga_groupby As New StringBuilder


        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL_select.Length = 0
            StbSQL_from_movcont.Length = 0
            StbSQL_from_raccolta.Length = 0
            StbSQL_where_movcont.Length = 0
            StbSQL_where_raccolta.Length = 0
            StbSQL_where.Length = 0

            'SB per Giacenze attuali
            StbSQL_ga_select.Length = 0
            StbSQL_ga_from_movcont.Length = 0
            StbSQL_ga_from_raccolta.Length = 0
            StbSQL_ga_where_movcont.Length = 0
            StbSQL_ga_where_raccolta.Length = 0
            StbSQL_ga_where.Length = 0
            StbSQL_ga_groupby.Length = 0


            StbSQL_select.Append(" SELECT  Contatti_ConfCli.Cod_Contatto AS Piva_ConfCli, Contatti_ConfCli.Rag_Soc AS RagSoc_ConfCli, " & vbCrLf)
            StbSQL_select.Append("         Risorse_Umane_Conferenti.Settore_Des AS Codice_ConfCli, " & vbCrLf)

            If Dettaglio_Stabilimento Then
                StbSQL_select.Append("   ISNULL( Contatti_Dest.Cod_Contatto, Contatti_ConfCli.Cod_Contatto) AS Codice_Stab, ISNULL( Contatti_Dest.Rag_Soc, Contatti_ConfCli.Rag_Soc) + ' ' + ISNULL(ind_des, '') AS RagSoc_Stab, " & vbCrLf)
            End If
            StbSQL_select.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, Agenda.lav_cod, " & vbCrLf)
            StbSQL_select.Append("         Mov_Cont.Doc_Numero_Sin ,Mov_Cont.Doc_Numero, Mov_Cont.Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            StbSQL_select.Append("         Mov_Cont.Data_Movimento AS Data_Doc, Mov_Magazzino.CAU_MOV AS Causale, Mov_Dest_Magazzino.Qta   " & vbCrLf)

            StbSQL_from_movcont.Append(" FROM Agenda " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL_from_movcont.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_from_movcont.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_from_movcont.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_from_movcont.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_from_raccolta.Append("  FROM Agenda  " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Mov_Dettagli_Riferimenti Agenda_Rif ON (Agenda.PIVA = Agenda_Rif.Piva AND Agenda.Id_Agenda = Agenda_Rif.Id_Agenda )   " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Movimenti Mov_Cont ON Agenda_Rif.Piva_Rif = Mov_Cont.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Cont.Id_Agenda   " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Movimenti Mov_Magazzino ON Agenda_Rif.Piva_Rif = Mov_Magazzino.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov    " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det    " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod  " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Movimenti Movimenti_Umani  ON Agenda.Piva = Movimenti_Umani.PIVA AND Agenda.Id_Agenda = Movimenti_Umani.Id_Agenda " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Movimenti_Umani.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm " & vbCrLf)
            StbSQL_from_raccolta.Append("  INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto ")
            If Dettaglio_Stabilimento Then
                StbSQL_from_raccolta.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_from_raccolta.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_from_raccolta.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_where_movcont.Append(" WHERE    Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)

            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                If Cod_Rapporto = 0 Then
                    '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                    StbSQL_where_movcont.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," &
                                                Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTE) & "," &
                                              Agro_SQL_SaveNum(COD_FORNITORE) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
                End If
            End If

            If Sa_Cod <> 0 Then
                StbSQL_where_movcont.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQL_where_movcont.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If


            StbSQL_where_raccolta.Append(" WHERE     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_RILIEVO_RACCOLTA) & "'" & vbCrLf)

            StbSQL_where.Append(" AND   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL_where.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL_where.Append(" AND       Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)
            StbSQL_where.Append(" AND       Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            If Mat_Cod <> 0 Then
                StbSQL_where.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_where.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            '-----------------------
            '04/02/2021
            If FiltroAggConferenti <> "" Then
                'tale filtro utilizza l'alias tabella Mov_Accett, faccio replace per Risorse_Umane_Conferenti
                FiltroAggConferenti = Replace(FiltroAggConferenti, "Mov_Accett", "Risorse_Umane_Conferenti")
                StbSQL_where.AppendLine(FiltroAggConferenti)
            End If
            If Cod_RisUm <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_RisUm = " & Agro_SQL_SaveText(Cod_RisUm) & " " & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            '13/06/2017:aggiunto x richiesta cofruta
            If Cod_Rapporto <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_where.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------




            '----------------------------------------------------------
            '--------------------- GIACENZA ATTUALE -------------------
            '----------------------------------------------------------
            StbSQL_ga_select.Append(" SELECT DISTINCT    Contatti_ConfCli.Cod_Contatto AS Piva_ConfCli, " & vbCrLf)
            StbSQL_ga_select.Append(" Risorse_Umane_Conferenti.Settore_Des AS Codice_ConfCli, " & vbCrLf)
            StbSQL_ga_select.Append(" Contatti_ConfCli.Rag_Soc AS RagSoc_ConfCli, " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_ga_select.Append("   ISNULL( Contatti_Dest.Cod_Contatto, Contatti_ConfCli.Cod_Contatto) AS Codice_Stab, ISNULL( Contatti_Dest.Rag_Soc, Contatti_ConfCli.Rag_Soc) + ' ' + ISNULL(ind_des, '') AS RagSoc_Stab, " & vbCrLf)
            End If
            StbSQL_ga_select.Append(" MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, -1 AS lav_cod, " & vbCrLf)
            StbSQL_ga_select.Append(" '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            '19/01/2015: non serve
            'StbSQL_ga_select.Append(" -1 AS Lunghezza_Sin, -1 AS Lunghezza_Centro, -1 AS Lunghezza_Des, '' AS CarattereFormattazione, " & vbCrLf)
            StbSQL_ga_select.Append(" '01/01/1900' AS Data_Doc, '-1' AS Causale " & vbCrLf)

            StbSQL_ga_select.Append(" , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL_ga_select.Append("         THEN        -(Mov_Dest_Magazzino.qta) " & vbCrLf)
            StbSQL_ga_select.Append("         ELSE        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL_ga_select.Append("         END) AS Giacenza " & vbCrLf)

            StbSQL_ga_from_movcont.Append(" FROM Agenda " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto  " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL_ga_from_movcont.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_ga_from_movcont.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_ga_from_movcont.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_ga_from_movcont.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_ga_from_raccolta.Append("  FROM Agenda " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Mov_Dettagli_Riferimenti Agenda_Rif ON (Agenda.PIVA = Agenda_Rif.Piva AND Agenda.Id_Agenda = Agenda_Rif.Id_Agenda )   " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Movimenti Mov_Cont ON Agenda_Rif.Piva_Rif = Mov_Cont.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Cont.Id_Agenda   " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Movimenti Mov_Magazzino ON Agenda_Rif.Piva_Rif = Mov_Magazzino.PIVA AND Agenda_Rif.Id_Agenda_Rif = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov    " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det    " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod  " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Movimenti Movimenti_Umani  ON Agenda.Piva = Movimenti_Umani.PIVA AND Agenda.Id_Agenda = Movimenti_Umani.Id_Agenda " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Movimenti_Umani.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm   " & vbCrLf)
            StbSQL_ga_from_raccolta.Append("  INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto  ")
            If Dettaglio_Stabilimento Then
                StbSQL_ga_from_raccolta.Append(" LEFT JOIN Risorse_Umane Risorse_Umane_Dest ON Mov_Cont.Cod_Destinazione = Risorse_Umane_Dest.Cod_RisUm  " & vbCrLf)
                StbSQL_ga_from_raccolta.Append(" LEFT JOIN Contatti Contatti_Dest ON Risorse_Umane_Dest.Piva = Contatti_Dest.Piva AND Risorse_Umane_Dest.Cod_Contatto = Contatti_Dest.Cod_Contatto " & vbCrLf)
                StbSQL_ga_from_raccolta.Append(" LEFT JOIN Indirizzi on Indirizzi.cod_indirizzo = Mov_Cont.Cod_IndirizzoDestinazione " & vbCrLf)
            End If

            StbSQL_ga_where_movcont.Append(" WHERE    Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)

            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                If Cod_Rapporto = 0 Then
                    '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                    StbSQL_where_movcont.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," &
                                                Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTE) & "," &
                                              Agro_SQL_SaveNum(COD_FORNITORE) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
                End If
            End If

            If Sa_Cod <> 0 Then
                StbSQL_ga_where_movcont.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQL_ga_where_movcont.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            StbSQL_ga_where_raccolta.Append(" WHERE     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_RILIEVO_RACCOLTA) & "'" & vbCrLf)

            StbSQL_ga_where.Append(" AND   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Saldo_Precedente) & " " & vbCrLf)
            StbSQL_ga_where.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL_ga_where.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQL_ga_where.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)
            StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            If Mat_Cod <> 0 Then
                StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If
            StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            StbSQL_ga_where.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)
            '-----------------------
            '04/02/2021
            If FiltroAggConferenti <> "" Then
                'tale filtro utilizza l'alias tabella Mov_Accett, faccio replace per Risorse_Umane_Conferenti
                FiltroAggConferenti = Replace(FiltroAggConferenti, "Mov_Accett", "Risorse_Umane_Conferenti")
                StbSQL_where.AppendLine(FiltroAggConferenti)
            End If
            If Cod_RisUm <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_RisUm = " & Agro_SQL_SaveText(Cod_RisUm) & " " & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            '13/06/2017:aggiunto x richiesta cofruta
            If Cod_Rapporto <> 0 Then
                StbSQL_where.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
            End If
            '---------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_ga_where.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '01/06/2018: commentato perché non serve  solo per gli imballi ma anche per contenitori e confezioni 
            '(sulla parte dei movimenti era già stato tolto o mai stato messo dalla sua creazione, 
            'originata da un copia-incolla del report di fruttagel che invece aveva il filtro)
            'StbSQL_ga_where.Append(" AND MP_Imballi.chkimballaggio=1 ")
            '--------------------------------------------------------------------------

            StbSQL_ga_groupby.Append(" GROUP BY Contatti_ConfCli.Cod_Contatto, Risorse_Umane_Conferenti.Settore_Des,  Contatti_ConfCli.Rag_Soc, " & vbCrLf)
            If Dettaglio_Stabilimento Then
                StbSQL_ga_groupby.Append(" ind_des, Contatti_Dest.Cod_Contatto, Contatti_Dest.Rag_Soc,  " & vbCrLf)
            End If
            StbSQL_ga_groupby.Append("           MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des  " & vbCrLf)

            '----------------------------------------------------------
            '----------------- FINE GIACENZA ATTUALE ------------------
            '----------------------------------------------------------



            '**********************************************************
            '******************** QUERY FINALE  ***********************
            '**********************************************************
            '  Roberta, 28/10/2015 16.01.19: 
            StbSQL.Append(" -- query che legge i movimenti " & vbCrLf)
            'MOVCONT
            StbSQL.Append(" ( ")
            StbSQL.Append(StbSQL_select.ToString & StbSQL_from_movcont.ToString & StbSQL_where_movcont.ToString & StbSQL_where.ToString)
            StbSQL.Append(" ) " & vbCrLf)
            'RACCOLTA
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" -- query per non F&F " & vbCrLf)
                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" ( ")
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from_raccolta.ToString & StbSQL_where_raccolta.ToString & StbSQL_where.ToString)
                StbSQL.Append(" ) " & vbCrLf)
            End If

            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" UNION ALL " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" -- query che legge la giacenza al giorno prima " & vbCrLf)

            'MOVCONT
            StbSQL.Append(" ( " & vbCrLf)
            StbSQL.Append(StbSQL_ga_select.ToString & StbSQL_ga_from_movcont.ToString & StbSQL_ga_where_movcont.ToString & StbSQL_ga_where.ToString & StbSQL_ga_groupby.ToString)
            StbSQL.Append(" ) " & vbCrLf)
            'RACCOLTA
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" -- query per non F&F " & vbCrLf)
                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" ( " & vbCrLf)
                StbSQL.Append(StbSQL_ga_select.ToString & StbSQL_ga_from_raccolta.ToString & StbSQL_ga_where_raccolta.ToString & StbSQL_ga_where.ToString & StbSQL_ga_groupby.ToString)
                StbSQL.Append(" ) " & vbCrLf)
            End If

            If Dettaglio_Stabilimento Then
                StbSQL.Append(" ORDER BY  RagSoc_ConfCli, RagSoc_Stab, MP_Imballi.Mat_Des, Data_Doc, Causale, Mov_Cont.Doc_Numero ")
            Else
                StbSQL.Append(" ORDER BY  RagSoc_ConfCli, MP_Imballi.Mat_Des, Data_Doc, Causale, Mov_Cont.Doc_Numero ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '###################################################################
    Public Function RiepilogoConferimenti(ByVal Piva As String, _
                                            ByVal Mat_Cod As Integer, _
                                            ByVal Mat_Des As String, _
                                            ByVal Cod_Articolo As String, _
                                            ByVal Data_Inizio As Date, _
                                            ByVal Data_Fine As Date, _
                                            ByVal Codice_ConfCli As String, _
                                            ByVal RagSoc_ConfCli As String, _
                                            ByVal Cod_Rapporto As Integer, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        'filtro sa_cod e fabbricato_cod non utilizzati, arriveranno sempre a 0
        'nella pagina di filtro questi menù sono disattivati (nei conferimenti la destinazione è la cella)
        ' ByVal Sa_Cod As Integer, _
        ' ByVal Fabbricato_Cod As Integer, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.RiepilogoConferimenti_FF"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StbSQL As New StringBuilder
        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_from As New StringBuilder
        Dim StbSQL_where As New StringBuilder
        Dim StbSQL_orderby As New StringBuilder

        Dim StbSQL_select_base As New StringBuilder
        Dim StbSQL_select_1_2 As New StringBuilder
        Dim StbSQL_select_ConfDiversi As New StringBuilder

        Dim StbSQL_from_base As New StringBuilder
        Dim StbSQL_from_FF_OLD As New StringBuilder
        Dim StbSQL_from_FF_NEW As New StringBuilder

        Dim StbSQL_where_Accet_Base As New StringBuilder
        Dim StbSQL_where_Accet_FF_NEW As New StringBuilder
        Dim StbSQL_where_Accet_FF_OLD As New StringBuilder
        Dim StbSQL_where_NO_Accet_FF_NEW As New StringBuilder
        Dim StbSQL_where_NO_Accet_FF_OLD As New StringBuilder
        Dim StbSQL_where_ConfDiversi As New StringBuilder
        Dim StbSQL_where_filtri As New StringBuilder


        StbSQL_select.Length = 0
        StbSQL_from.Length = 0
        StbSQL_where.Length = 0

        StbSQL_select_base.Length = 0
        StbSQL_select_1_2.Length = 0
        StbSQL_select_ConfDiversi.Length = 0
        StbSQL_from_base.Length = 0
        StbSQL_from_FF_OLD.Length = 0
        StbSQL_from_FF_NEW.Length = 0

        StbSQL_where_Accet_Base.Length = 0
        StbSQL_where_Accet_FF_NEW.Length = 0
        StbSQL_where_Accet_FF_OLD.Length = 0
        StbSQL_where_ConfDiversi.Length = 0
        StbSQL_where_filtri.Length = 0
        StbSQL.Length = 0
        StbSQL_orderby.Length = 0

        Try

            ' Giulia: 25/8/2017: Aggiunto distinct per evitare doppioni... bisognerebbe evitare che ci siano e non utilizzare il distinct
            StbSQL_select_base.Append(" SELECT DISTINCT  " & vbCrLf)
            StbSQL_select_base.Append(" Agenda.piva ,Agenda.des_lib ,Agenda.id_agenda, Agenda.lav_cod,            " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett,    " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Mov_Desc AS Note,        " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Cod_RisUm,     " & vbCrLf)
            StbSQL_select_base.Append(" Risorse_Umane_Conferenti.Cod_Rapporto AS Cod_Rapporto_ConfCli,  " & vbCrLf)
            StbSQL_select_base.Append(" Contatti_ConfCli.Cod_Contatto AS Cod_Contatto_ConfCli,  " & vbCrLf)
            StbSQL_select_base.Append(" Contatti_ConfCli.Rag_Soc AS Rag_Soc_ConfCli, ISNULL(Contatti_ConfCli.Codice_Fiscale, '') AS Codice_Fiscale_ConfCli,  " & vbCrLf)
            StbSQL_select_base.Append(" Contatti_ConfCli.Nome AS Nome_ConfCli, Contatti_ConfCli.Cognome AS Cognome_ConfCli,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Cod_IndirizzoRisUm,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Cod_RisUm_Altro,             Mov_Accett.Extra_Int,             Mov_Accett.Cod_Destinazione,   " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, Mov_Accett.Cod_IndirizzoVettore,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Accett.Cod_IndirizzoVettore,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Conf.Doc_Numero AS Doc_Numero_Conf,             Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Conf.Colli,              Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Raccolta.Data_Movimento AS Data_Raccolta,             Mov_Raccolta.ora AS DataOra_Ingresso,    " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.sa_cod AS Sa_cod_Raccolta,   " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Dett_Raccolta.Mov_Det_Des AS Mov_Det_Des_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta, " & vbCrLf)
            StbSQL_select_base.Append(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,             Mov_Dett_Raccolta.Qta, Mov_Dett_Raccolta.Tara AS Tara,  Mov_Dett_Raccolta.Qta_Extra, Mov_Dett_Raccolta.Udm_Cod_Extra, Mov_Dett_Raccolta.Qta_Extra_totale, Mov_Dett_Raccolta.Tara,            Mov_Dett_Raccolta.Qta_Dettaglio1 AS Qta_sottoconfezioni, Mov_Dett_Raccolta.Qta_Dettaglio2 AS Qta_Imballaggi,                      Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Prezzo_Unitario,             Mov_Dett_Raccolta.Prezzo_Unitario_Netto, Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,             Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,    " & vbCrLf)
            StbSQL_select_base.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)
            StbSQL_select_base.Append(" MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG, " & vbCrLf)
            StbSQL_select_base.Append(" MP_Raccolta.Regolamento, MP_Raccolta.ChkListino      ,  " & vbCrLf)

            '///////////////////////////////////////////////////////////////////////
            '14/06/2017: 
            'parametri letti dinamicamente dal cal_cod (query apposita richiamata dai sorgenti)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='ocalibro'               AND OTabelle_Parametri.Tabella_Cod = 1               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as ocalibro        ,  " & vbCrLf)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='oqualita'               AND OTabelle_Parametri.Tabella_Cod = 3               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as oqualita        ,  " & vbCrLf)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='odeclassamento'               AND OTabelle_Parametri.Tabella_Cod = 6               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as odeclassamento        ,  " & vbCrLf)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='omarca'               AND OTabelle_Parametri.Tabella_Cod = 2               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as omarca        ,  " & vbCrLf)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT Materie_Prime_Campionature.Descrizione               FROM Materie_Prime_Campionature                WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod                AND Materie_Prime_Campionature.tipo ='onote'                ), '') as onote,   " & vbCrLf)

            'aggiornata, c'era la sottoconfezione al posto del contenitore
            StbSQL_select_1_2.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='oconfezione'               AND OTabelle_Parametri.Tabella_Cod = 5               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as confezione        , " & vbCrLf)
            StbSQL_select_1_2.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='ocontenitore'               AND OTabelle_Parametri.Tabella_Cod = 8               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as contenitore        ,  " & vbCrLf)
            StbSQL_select_1_2.Append(" ISNULL (               (               SELECT OTabelle_Parametri.Descrizione               FROM Materie_Prime_Campionature                INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod               AND Materie_Prime_Campionature.tipo ='oimballaggio'               AND OTabelle_Parametri.Tabella_Cod = 4               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as imballaggio        ,  " & vbCrLf)

            StbSQL_select_ConfDiversi.Append(" '' AS confezione, '' AS contenitore," & vbCrLf)
            StbSQL_select_ConfDiversi.Append(" ISNULL (               (			   SELECT Materie_Prime.Mat_Des FROM Mov_Dettaglio_Tecnico_Extra INNER JOIN Materie_Prime ON (Mov_Dettaglio_Tecnico_Extra.Piva = Materie_Prime.Piva and Mov_Dettaglio_Tecnico_Extra.Sa_Cod = Materie_Prime.Sa_Cod and Materie_Prime.Mat_Cod=Mov_Dettaglio_Tecnico_Extra.Imballaggio_Cod) 				WHERE (Mov_Dettaglio_Tecnico_Extra.Id_Mov = Mov_Dett_Raccolta.Id_Mov and Mov_Dettaglio_Tecnico_Extra.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det)), '') as imballaggio        ,     " & vbCrLf)

            'nel F&F il numero degli imballaggi e dei contenitori sono salvati qui:
            StbSQL_select_1_2.Append("   Mov_Dett_Raccolta.Qta_Dettaglio1 AS num_contenitori, Mov_Dett_Raccolta.Qta_Dettaglio2 AS Num_imballi " & vbCrLf)

            StbSQL_select_ConfDiversi.Append(" 0 AS num_contenitori," & vbCrLf)
            StbSQL_select_ConfDiversi.Append(" ISNULL (               (			   SELECT Mov_Dettaglio_Tecnico_Extra.Num_Contenitori  FROM Mov_Dettaglio_Tecnico_Extra 				WHERE (Mov_Dettaglio_Tecnico_Extra.Id_Mov = Mov_Dett_Raccolta.Id_Mov and Mov_Dettaglio_Tecnico_Extra.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det)), '') as Num_imballi          " & vbCrLf)

            'informazioni non necessarie
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT contatti.rag_soc + contatti.nome + ' ' + contatti.cognome               FROM Materie_Prime_Campionature                INNER JOIN Risorse_Umane ON  Materie_Prime_Campionature.tipo_cod = Risorse_Umane.Cod_RisUm               INNER JOIN Contatti ON  Contatti.piva = Risorse_Umane.piva AND Contatti.cod_contatto = Risorse_Umane.cod_contatto               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod                AND Materie_Prime_Campionature.tipo ='ocliente'               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as ocliente        ,  " & vbCrLf)
            'StbSQL_select_base.Append(" ISNULL (               (               SELECT contatti.rag_soc + contatti.nome + ' ' + contatti.cognome               FROM Materie_Prime_Campionature                INNER JOIN Risorse_Umane ON  Materie_Prime_Campionature.tipo_cod = Risorse_Umane.Cod_RisUm               INNER JOIN Contatti ON  Contatti.piva = Risorse_Umane.piva AND Contatti.cod_contatto = Risorse_Umane.cod_contatto               WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod                AND Materie_Prime_Campionature.tipo ='ofornitore'               AND Materie_Prime_Campionature.tipo_cod <> 0                ), '') as ofornitore        ,  " & vbCrLf)
            '///////////////////////////////////////////////////////////////////////

            StbSQL_select_base.Append(" (CASE WHEN Mov_Dett_Raccolta.udm_cod_extra = '0' THEN 'kg' ELSE UDM_Peso.UDM_SIM END)  AS Udm_Des " & vbCrLf)

            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL_select_base.Append(" , " & vbCrLf)
                StbSQL_select_base.Append(" (CASE WHEN Cantina_Vasche.Identificativo IS NULL THEN 'Non allocato' ELSE Cantina_Vasche.Identificativo END ) AS Cella_Stiva,  " & vbCrLf)
            End If

            'FROM
            StbSQL_from_base.Append(" FROM Agenda " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Movimenti Mov_Accett ON (Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda) " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Movimenti Mov_Conf ON (Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda)      " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Movimenti Mov_Raccolta ON (Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda)  " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON (Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov)    " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm    " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Contatti Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto  " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN Materie_Prime MP_Raccolta ON (Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod )  " & vbCrLf)
            StbSQL_from_base.Append(" INNER JOIN UnitaMisura UDM_Peso ON Mov_Dett_Raccolta.udm_cod_extra = UDM_Peso.Udm_Cod   " & vbCrLf)
            StbSQL_from_base.Append("")

            '12/12/19 commentato perché non veniva usata la tabella
            'StbSQL_from_1_2.Append(" INNER JOIN Movimenti Mov_Carico ON (Agenda.PIVA = Mov_Carico.PIVA AND Agenda.Sa_Cod = Mov_Carico.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico.Id_Agenda AND Mov_Carico.Modalita='0')" & vbCrLf)

            StbSQL_from_FF_OLD.Append(" --- Recupero valore colonna Cella_Stiva " & vbCrLf)
            StbSQL_from_FF_OLD.Append("   INNER JOIN Movimenti MovimentiB  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         ON MovimentiB.PIVA=Agenda.PIVA  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND MovimentiB.Sa_Cod=Agenda.Sa_Cod  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND MovimentiB.Id_Agenda=Agenda.Id_Agenda  -- è l'id_agenda della bolla " & vbCrLf)
            StbSQL_from_FF_OLD.Append("   INNER JOIN Movimenti_dettagli Movimenti_dettagliB  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         ON MovimentiB.PIVA=Movimenti_dettagliB.PIVA  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND MovimentiB.id_agenda=Movimenti_dettagliB.id_agenda  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND MovimentiB.Id_Mov=Movimenti_dettagliB.Id_Mov " & vbCrLf)
            StbSQL_from_FF_OLD.Append("   INNER JOIN Mov_destinazioni Mov_destinazioniB  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         ON Movimenti_dettagliB.PIVA=Mov_destinazioniB.PIVA  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND Movimenti_dettagliB.id_agenda=Mov_destinazioniB.id_agenda  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND Movimenti_dettagliB.Id_Mov=Mov_destinazioniB.Id_Mov " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND Movimenti_dettagliB.Id_Mov_Det=Mov_destinazioniB.Id_Mov_Det " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         AND Mov_destinazioniB.Tipo_destinazione=16 " & vbCrLf)
            StbSQL_from_FF_OLD.Append("   LEFT JOIN Cantina_Vasche  " & vbCrLf)
            StbSQL_from_FF_OLD.Append("         ON Mov_destinazioniB.Id_destinazione=Cantina_Vasche.Vas_Cod AND Mov_destinazioniB.sa_cod=Cantina_Vasche.sa_Cod    " & vbCrLf)
            StbSQL_from_FF_OLD.Append("   --- " & vbCrLf)

            StbSQL_from_FF_NEW.Append("    " & vbCrLf)
            StbSQL_from_FF_NEW.Append(" INNER JOIN Mov_destinazioni Mov_Dest_Raccolta   " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         ON Mov_Dett_Raccolta.PIVA=Mov_Dest_Raccolta.PIVA   " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         AND Mov_Dett_Raccolta.id_agenda=Mov_Dest_Raccolta.id_agenda    " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         AND Mov_Dett_Raccolta.Id_Mov=Mov_Dest_Raccolta.Id_Mov    " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         AND Mov_Dett_Raccolta.Id_Mov_Det=Mov_Dest_Raccolta.Id_Mov_Det    " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         AND Mov_Dest_Raccolta.Tipo_destinazione=16    " & vbCrLf)
            StbSQL_from_FF_NEW.Append(" LEFT JOIN Cantina_Vasche  " & vbCrLf)
            StbSQL_from_FF_NEW.Append("         ON Mov_Dest_Raccolta.Id_destinazione=Cantina_Vasche.Vas_Cod  AND Mov_Dest_Raccolta.sa_cod=Cantina_Vasche.sa_Cod     " & vbCrLf)
            StbSQL_from_FF_NEW.Append("   --- " & vbCrLf)

            StbSQL_where_Accet_Base.Append(" WHERE (Agenda.Lav_Cod = 1054 OR Agenda.Lav_Cod = 1078 OR Agenda.Lav_Cod = 1076)  " & vbCrLf)
            StbSQL_where_Accet_Base.Append(" AND Agenda.Tipo_Accettazione = 2 " & vbCrLf)
            StbSQL_where_Accet_Base.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
            StbSQL_where_Accet_Base.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_where_Accet_Base.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_Accet_Base.Append(" AND Mov_Conf.Cau_Mov        = '4050' " & vbCrLf)
            If Cod_Rapporto = 0 Then
                '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                StbSQL_where_Accet_Base.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & _
                                            Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
            End If

            '12/12/19 commentato perché non veniva usata la tabella
            'StbSQL_where_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300'  " & vbCrLf)

            StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Raccolta.Cau_Mov    = '7300' " & vbCrLf)
            StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Dett_Raccolta.elem_cod    = " & CStr(TRASFORMATI_VEGETALI) & " " & vbCrLf)

            StbSQL_where_Accet_FF_OLD.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL_where_Accet_FF_OLD.Append(" AND MovimentiB.Cau_Mov='7920'  " & vbCrLf)
                StbSQL_where_Accet_FF_OLD.Append("        AND Mov_Dett_Raccolta.elem_cod= Movimenti_dettagliB.elem_cod " & vbCrLf)
                StbSQL_where_Accet_FF_OLD.Append("         AND Mov_Dett_Raccolta.Mat_cod= Movimenti_dettagliB.Mat_cod " & vbCrLf)
                StbSQL_where_Accet_FF_OLD.Append("        AND Mov_Dett_Raccolta.pro_cod= Movimenti_dettagliB.pro_cod  " & vbCrLf)
                StbSQL_where_Accet_FF_OLD.Append("         AND Mov_Dett_Raccolta.udm_cod= Movimenti_dettagliB.udm_cod  " & vbCrLf)
                '   StbSQL_where_Accet.Append("        AND Mov_Dett_Raccolta.cal_cod= Movimenti_dettagliB.cal_cod " & vbCrLf)
                StbSQL_where_Accet_FF_OLD.Append("          AND Mov_Dett_Raccolta.lotto= Movimenti_dettagliB.lotto " & vbCrLf)
            End If

            StbSQL_where_NO_Accet_FF_OLD.Append(" WHERE (Agenda.Lav_Cod = 1025 or Agenda.Lav_Cod = 1077 or Agenda.Lav_Cod = 1075)  " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'recupero i dati del ddt dal 4000 (nel caso di doc con accettazione i dati del ddt sono nel 4050)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
            If Cod_Rapporto = 0 Then
                '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                StbSQL_where_NO_Accet_FF_OLD.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & _
                                            Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
            End If

            '12/12/19 commentato perché non veniva usata la tabella
            'StbSQL_where_NO_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300' " & vbCrLf) '??? chiedere : il 7300 ci sono 2 righe invece di una --> per impostare il criterio di scelta utilizzo il campo Modalita

            StbSQL_where_NO_Accet_FF_OLD.Append(" AND (MovimentiB.Cau_Mov='7300' AND Id_destinazione >0 )  " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append(" AND Mov_Dett_Raccolta.elem_cod= Movimenti_dettagliB.elem_cod " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append("   AND Mov_Dett_Raccolta.Mat_cod= Movimenti_dettagliB.Mat_cod " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append("   AND Mov_Dett_Raccolta.pro_cod= Movimenti_dettagliB.pro_cod  " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append("   AND Mov_Dett_Raccolta.udm_cod= Movimenti_dettagliB.udm_cod  " & vbCrLf)
            '   StbSQL_where_NO_Accet.Append("      AND Mov_Dett_Raccolta.cal_cod= Movimenti_dettagliB.cal_cod " & vbCrLf)
            StbSQL_where_NO_Accet_FF_OLD.Append("   AND Mov_Dett_Raccolta.lotto= Movimenti_dettagliB.lotto " & vbCrLf)


            StbSQL_where_NO_Accet_FF_NEW.Append(" WHERE (Agenda.Lav_Cod = 1077 or Agenda.Lav_Cod = 1075)  " & vbCrLf)
            StbSQL_where_NO_Accet_FF_NEW.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
            StbSQL_where_NO_Accet_FF_NEW.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL_where_NO_Accet_FF_NEW.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_NO_Accet_FF_NEW.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'recupero i dati del ddt dal 4000 (nel caso di doc con accettazione i dati del ddt sono nel 4050)
            StbSQL_where_NO_Accet_FF_NEW.Append(" AND Mov_Raccolta.Cau_Mov    = '7300' " & vbCrLf)
            If Cod_Rapporto = 0 Then
                '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                StbSQL_where_NO_Accet_FF_NEW.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & _
                                            Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
            End If

            StbSQL_where_ConfDiversi.Append(" WHERE (Agenda.Lav_Cod = 1052)  " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'dati dal ddt
            StbSQL_where_ConfDiversi.Append(" AND Mov_Raccolta.Cau_Mov    = '4200' " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            '23/07/2019: non serve filtrare contemporaneamente cod_contatto e like su rag_soc (il cod_contatto è più importante)
            'modificato cmq per evitare il problema di report vuoto nel caso di contatti con lettere accentate nella rag_soc:
            'la pagina di pre stampa elimina le lettere accentate nel passaggio via querystring
            If Codice_ConfCli <> "" Then
                StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Cod_Contatto = '" & Agro_SQL_SaveText(Codice_ConfCli) & "'   " & vbCrLf)
            Else
                If RagSoc_ConfCli <> "" Then
                    StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Rag_Soc LIKE '%" & Agro_SQL_SaveText(RagSoc_ConfCli) & "%'   " & vbCrLf)
                End If
            End If

            '13/06/2017:aggiunto x richiesta cofruta
            If Cod_Rapporto <> 0 Then
                StbSQL_where_filtri.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
            End If
            'Il Mat_Cod è quello del prodotto oggetto del conferimento 
            If Mat_Cod <> 0 Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            If Mat_Des <> "" Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Des LIKE '%" & Agro_SQL_SaveText(Mat_Des) & "%'   " & vbCrLf)
            End If

            If Cod_Articolo <> "" Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(Cod_Articolo) & "%'   " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_where_filtri.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL_orderby.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL_orderby.Append(" ")
            End If


            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then

                ''''********************************************************************
                ''''*** CASO 1 :  conferimento con Accettazione (F&F OLD VERSION)
                '''' Es. MINI - TROMBIN
                ''''********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_1_2.ToString)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_FF_OLD.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_Accet_Base.ToString & StbSQL_where_Accet_FF_OLD.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" -- CASO 1 :  conferimento con Accettazione (F&F OLD VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


                ''''*********************************************************************
                ''''*** CASO 2 : conferimento senza Accettazione (F&F OLD VERSION)
                '''' Es. MINI - TROMBIN
                ''''*********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_1_2.ToString)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_FF_OLD.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_NO_Accet_FF_OLD.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 2 : conferimento senza Accettazione (F&F OLD VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


                ''''********************************************************************
                ''''*** CASO 4 :  conferimento con Accettazione (F&F NEW VERSION)
                '''' Es. COOP SOLE - 
                ''''********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_1_2.ToString)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_FF_NEW.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_Accet_Base.ToString & StbSQL_where_Accet_FF_NEW.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 4 :  conferimento con Accettazione (F&F NEW VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)

                ''''*********************************************************************
                ''''*** CASO 5 : conferimento senza Accettazione (F&F NEW VERSION)
                '''' Es. COOP SOLE -
                ''''*********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_1_2.ToString)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_FF_OLD.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_NO_Accet_FF_NEW.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 5 : conferimento senza Accettazione (F&F OLD VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


            Else
                ''''***************************************
                ''''*** CASO 3 : conferimento VERSO diversi
                '''' Es. AGRISFERA
                ''''***************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_ConfDiversi.ToString)
                StbSQL_from.Append(StbSQL_from_base.ToString)
                StbSQL_where.Append(StbSQL_where_ConfDiversi.ToString & StbSQL_where_filtri.ToString)

                StbSQL.Append(" -- CASO 3 : conferimento VERSO diversi " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString)

                ''''*******************************************************************************
            End If
            StbSQL.Append(StbSQL_orderby.ToString)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    'Public Function TracciabilitaDaBolle_XLS(ByVal Piva As String, _
    '                                         ByVal Codice_ConfCli As String, _
    '                                         ByVal RagSoc_ConfCli As String, _
    '                                         ByVal Cod_Rapporto As Integer, _
    '                                         ByVal Data_Inizio As Date, _
    '                                         ByVal Data_Fine As Date, _
    '                                         ByVal Mat_Cod As Integer, _
    '                                         ByVal Mat_Des As String, _
    '                                         ByVal Cod_Articolo As String, _
    '                                         ByVal xFiltroAggiuntivo As String, _
    '                                         ByRef str_ElencoBolle As String, _
    '                                         ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione, _
    '                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                         ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.TracciabilitaDaBolle_XLS"
    '    Dim MessaggioErrore As String = ""

    '    'filtro sa_cod e fabbricato_cod non utilizzati, arriveranno sempre a 0
    '    'nella pagina di filtro questi menù sono disattivati (nei conferimenti la destinazione è la cella)
    '    ' ByVal Sa_Cod As Integer, _
    '    ' ByVal Fabbricato_Cod As Integer, _

    '    Dim DT As DataTable

    '    Dim StbSQL As New System.Text.StringBuilder
    '    Dim StbSQL_select As New System.Text.StringBuilder
    '    Dim StbSQL_select_base As New System.Text.StringBuilder
    '    Dim StbSQL_select_Accet_FF_OLD As New System.Text.StringBuilder
    '    Dim StbSQL_select_Accet_FF_NEW As New System.Text.StringBuilder
    '    Dim StbSQL_select_NoAccett As New System.Text.StringBuilder
    '    Dim StbSQL_select_ConfDiversi As New System.Text.StringBuilder
    '    Dim StbSQL_from As New System.Text.StringBuilder
    '    Dim StbSQL_from_base As New System.Text.StringBuilder
    '    Dim StbSQL_from_Accet_FF_OLD As New System.Text.StringBuilder
    '    Dim StbSQL_from_ConfDiversi As New System.Text.StringBuilder
    '    Dim StbSQL_where As New System.Text.StringBuilder
    '    Dim StbSQL_where_Accet_base As New System.Text.StringBuilder
    '    Dim StbSQL_where_Accet_FF_OLD As New System.Text.StringBuilder
    '    Dim StbSQL_where_Accet_FF_NEW As New System.Text.StringBuilder
    '    Dim StbSQL_where_NO_Accet As New System.Text.StringBuilder
    '    Dim StbSQL_where_ConfDiversi As New System.Text.StringBuilder
    '    Dim StbSQL_where_filtri As New System.Text.StringBuilder
    '    Dim StbSQL_orderby As New System.Text.StringBuilder


    '    StbSQL_select.Length = 0
    '    StbSQL_select_base.Length = 0
    '    StbSQL_select_Accet_FF_NEW.Length = 0
    '    StbSQL_select_Accet_FF_OLD.Length = 0
    '    StbSQL_select_NoAccett.Length = 0
    '    StbSQL_select_ConfDiversi.Length = 0
    '    StbSQL_from.Length = 0
    '    StbSQL_from_base.Length = 0
    '    StbSQL_from_Accet_FF_OLD.Length = 0
    '    StbSQL_from_ConfDiversi.Length = 0
    '    StbSQL_where.Length = 0
    '    StbSQL_where_Accet_base.Length = 0
    '    StbSQL_where_Accet_FF_OLD.Length = 0
    '    StbSQL_where_Accet_FF_NEW.Length = 0
    '    StbSQL_where_NO_Accet.Length = 0
    '    StbSQL_where_ConfDiversi.Length = 0
    '    StbSQL_where_filtri.Length = 0
    '    StbSQL_orderby.Length = 0
    '    StbSQL.Length = 0


    '    Try

    '        StbSQL_select_base.Append(" SELECT 	" & vbCrLf)
    '        StbSQL_select_base.Append("	Agenda.Id_Agenda, Risorse_Umane_Conferenti.Settore_Des AS Codice_ConfCli, " & vbCrLf)
    '        StbSQL_select_base.Append("  Risorse_Umane_Conferenti.Cod_Contatto AS Piva_ConfCli, ISNULL(Contatti_ConfCli.Rag_Soc, ' ') AS RagSoc_ConfCli, " & vbCrLf)

    '        StbSQL_select_base.Append("  0.0 AS Peso_Totale, Mov_Accett.Tara_Veicolo,  " & vbCrLf)
    '        StbSQL_select_base.Append("  0.0 AS Peso_Lordo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, 0.0 AS Peso_Netto, 0.0 AS Degrado_Perc, 0.0 AS Degrado, 0.0 AS Netto_Pagamento,                                               " & vbCrLf)
    '        StbSQL_select_base.Append("  Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla,                                                       " & vbCrLf)
    '        StbSQL_select_base.Append("  mov_Accett.Data_Movimento AS Data_Bolla, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf,                                                                                                       " & vbCrLf)
    '        StbSQL_select_base.Append("  Mov_Conf.Doc_Numero AS Doc_Numero_Conf, Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,                                                                                      " & vbCrLf)
    '        StbSQL_select_base.Append("  Mov_Conf.Data_Movimento AS Data_Conf, " & vbCrLf)

    '        StbSQL_select_base.Append("  Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta,                                                                 " & vbCrLf)
    '        StbSQL_select_base.Append("  Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,                                                      " & vbCrLf)
    '        StbSQL_select_base.Append("  Mov_Dett_Raccolta.Tara,  " & vbCrLf)
    '        StbSQL_select_base.Append(" ISNULL( " & vbCrLf)
    '        StbSQL_select_base.Append(" ( " & vbCrLf)
    '        StbSQL_select_base.Append("     SELECT  Listino_Des " & vbCrLf)
    '        StbSQL_select_base.Append("     FROM    Listini_Prezzi " & vbCrLf)
    '        StbSQL_select_base.Append("     WHERE   Listini_Prezzi.Listino_Cod = Mov_Dett_Raccolta.Listino_Cod " & vbCrLf)
    '        StbSQL_select_base.Append("     AND     Listini_Prezzi.Piva = Mov_Dett_Raccolta.Piva " & vbCrLf)
    '        StbSQL_select_base.Append("     AND     Listini_Prezzi.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
    '        StbSQL_select_base.Append(" ) , '' ) AS Listino, Mov_Dett_Raccolta.Prezzo_Unitario_Netto, " & vbCrLf)
    '        StbSQL_select_base.Append("  MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Des AS Descr_Specie, ' ' AS Descr_Varieta, MP_Raccolta.Mat_Cod AS Codice_Prodotto,                                   " & vbCrLf)
    '        StbSQL_select_base.Append("  MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)
    '        StbSQL_select_base.Append("  MP_Raccolta.Regolamento as Regolamento_Prodotto, ' ' as Tipologia_Agricoltura, " & vbCrLf)

    '        StbSQL_select_base.Append(" ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro " & vbCrLf)
    '        StbSQL_select_base.Append("         FROM materie_prime_calibri  " & vbCrLf)
    '        StbSQL_select_base.Append("         INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod " & vbCrLf)
    '        StbSQL_select_base.Append("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
    '        StbSQL_select_base.Append("         AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro_Des, " & vbCrLf)

    '        StbSQL_select_base.Append(" ISNULL( (SELECT TOP 1 Descrizione  " & vbCrLf)
    '        StbSQL_select_base.Append("             FROM Materie_Prime_Campionature MP_Camp_Indice  " & vbCrLf)
    '        StbSQL_select_base.Append("             WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
    '        StbSQL_select_base.Append("             AND MP_Camp_Indice.tipo = 'indice' " & vbCrLf)
    '        StbSQL_select_base.Append("         ), 0 ) AS Indice, " & vbCrLf)

    '        StbSQL_select_base.Append("  '' AS Str_Progetto_Nome, '' AS Str_Sup_Imp,                                    " & vbCrLf)
    '        StbSQL_select_base.Append("  '' AS Str_Veg_Des, '' AS Str_Cul_Des, '' AS Str_Grva_Des, '' AS Str_Grfi_Des, '' AS Str_Setup_Cod, '' AS Str_Validita_Inizio_Impianto, '' AS Str_Validita_Fine_Impianto,                        " & vbCrLf)
    '        StbSQL_select_base.Append("  '' AS Str_Validita_Inizio_Distinta, '' AS Str_Validita_Fine_Distinta,                                                                                                                            " & vbCrLf)
    '        StbSQL_select_base.Append("  MP_Raccolta.Veg_Cod AS CodSpecieVeg, " & vbCrLf)

    '        'StbSQL_select_Accett.Append(" Fabbr_Raccolta.Fabbricato_Des AS Magazzino, ")

    '        StbSQL_select_Accet_ff_new.Append(" Mov_Accett.Peso " & vbCrLf)
    '        StbSQL_select_Accet_ff_old.Append(" Mov_Accett_Peso.Peso " & vbCrLf)

    '        'StbSQL_select_NoAccett.Append(" Fabbr_Raccolta.Fabbricato_Des AS Magazzino, ")

    '        StbSQL_select_NoAccett.Append(" Mov_Carico_Peso.Peso AS Peso " & vbCrLf)

    '        'StbSQL_select_ConfDiversi.Append("  '' AS Magazzino,  ")
    '        StbSQL_select_ConfDiversi.Append("   Mov_Accett.Peso as Peso ")

    '        StbSQL_from_base.Append("  FROM            Agenda 																																																																					" & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda                                                                                                                                                     " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Risorse_Umane AS Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm                                                                                                                                                                                         " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Contatti AS Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto                                                                                                                       " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda                                                                                                                                                             " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda                                                                                                                                            " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Movimenti_dettagli AS Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov                                                                                              " & vbCrLf)
    '        StbSQL_from_base.Append("  INNER JOIN Materie_Prime AS MP_Raccolta ON Mov_DettRaccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod                                                                                                                                                        " & vbCrLf)

    '        StbSQL_from_Accet_FF_OLD.Append("  INNER JOIN Movimenti AS Mov_Accett_Peso ON Agenda.PIVA = Mov_Accett_Peso.PIVA AND Agenda.Sa_Cod = Mov_Accett_Peso.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett_Peso.Id_Agenda   " & vbCrLf)

    '        'StbSQL_from_1_2.Append("  INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Carico.PIVA = Mov_Dest_Raccolta.Piva AND  Mov_Carico.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Carico.Id_Mov = Mov_Dest_Raccolta.Id_Mov                                                                                                        " & vbCrLf)
    '        'StbSQL_from_1_2.Append("  INNER JOIN Fabbricati AS Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod                                                                                        " & vbCrLf)

    '        'StbSQL_from_1_2.Append("  INNER JOIN Movimenti Mov_Carico ON (Agenda.PIVA = Mov_Carico.PIVA AND Agenda.Sa_Cod = Mov_Carico.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico.Id_Agenda AND Mov_Carico.Modalita='0')                                                                                                                          " & vbCrLf)
    '        'StbSQL_from_1_2.Append("  INNER JOIN Movimenti Mov_Carico_Peso ON (Agenda.PIVA = Mov_Carico_Peso.PIVA AND Agenda.Sa_Cod = Mov_Carico_Peso.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico_Peso.Id_Agenda AND Mov_Carico_Peso.Modalita='1')                                                                                                 " & vbCrLf)

    '        ''Questa join è fittizia : serve solo per avere nel caso 3 la tabella Mov_Carico_Peso 
    '        'StbSQL_from_ConfDiversi.Append(" INNER JOIN (select  id_agenda,PIVA,Sa_Cod ,0.0 as peso from Agenda where PIVA = Agenda.PIVA and Sa_Cod=Agenda.sa_cod and Id_agenda=Agenda.Id_Agenda) as Mov_Carico_Peso ON  Mov_Carico_Peso.PIVA=Agenda.PIVA AND Mov_Carico_Peso.Sa_Cod=Agenda.Sa_Cod AND Mov_Carico_Peso.id_agenda=Agenda.id_agenda   " & vbCrLf)

    '        StbSQL_where_Accet_base.Append(" WHERE (Agenda.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_DISTINTA_CARICO_ACCETTAZIONE) & ")  " & vbCrLf)
    '        StbSQL_where_Accet_base.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
    '        StbSQL_where_Accet_base.Append(" AND Agenda.Piva = '" + Piva + "' " & vbCrLf)
    '        If Cod_Rapporto = 0 Then
    '            StbSQL_where_Accet_base.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & ")  " & vbCrLf)
    '        End If
    '        StbSQL_where_Accet_base.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
    '        StbSQL_where_Accet_base.Append(" AND Mov_Conf.Cau_Mov        = '4050' " & vbCrLf)

    '        StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Raccolta.Cau_Mov    = '7300' " & vbCrLf)
    '        StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Dett_Raccolta.elem_cod    = " & CStr(TRASFORMATI_VEGETALI) & " " & vbCrLf)

    '        StbSQL_where_Accet_FF_OLD.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
    '        StbSQL_where_Accet_FF_OLD.Append(" AND Mov_Accett_Peso.Cau_Mov = '7920' " & vbCrLf)

    '        'StbSQL_where_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300'  " & vbCrLf)


    '        StbSQL_where_NO_Accet.Append(" WHERE (Agenda.Lav_Cod = " & CStr(LAVCOD_BOLLA_RICEVUTA) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_AUTO_DDT_EMESSO) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_DISTINTA_CARICO) & ")  " & vbCrLf)
    '        StbSQL_where_NO_Accet.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
    '        StbSQL_where_NO_Accet.Append(" AND Agenda.Piva = '" + Piva + "' " & vbCrLf)
    '        If Cod_Rapporto = 0 Then
    '            StbSQL_where_NO_Accet.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & ")  " & vbCrLf)
    '        End If
    '        StbSQL_where_NO_Accet.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
    '        StbSQL_where_NO_Accet.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'recupero i dati del ddt dal 4000 (nel caso di doc con accettazione i dati del ddt sono nel 4050)
    '        StbSQL_where_NO_Accet.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
    '        'StbSQL_where_NO_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300' " & vbCrLf) '??? chiedere : il 7300 ci sono 2 righe invece di una --> per impostare il criterio di scelta utilizzo il campo Modalita  
    '        'StbSQL_where_NO_Accet.Append(" AND Mov_Carico_Peso.Cau_Mov  = '7300' " & vbCrLf)


    '        StbSQL_where_ConfDiversi.Append(" WHERE (Agenda.Lav_Cod = 1052)  " & vbCrLf)
    '        StbSQL_where_ConfDiversi.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
    '        StbSQL_where_ConfDiversi.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'dati dal ddt
    '        StbSQL_where_ConfDiversi.Append(" AND Mov_Raccolta.Cau_Mov    = '4200' " & vbCrLf)
    '        StbSQL_where_ConfDiversi.Append(" AND Agenda.Piva = '" + Piva + "' " & vbCrLf)

    '        StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
    '        StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

    '        '23/07/2019: non serve filtrare contemporaneamente cod_contatto e like su rag_soc (il cod_contatto è più importante)
    '        'modificato cmq per evitare il problema di report vuoto nel caso di contatti con lettere accentate nella rag_soc:
    '        'la pagina di pre stampa elimina le lettere accentate nel passaggio via querystring
    '        If Codice_ConfCli <> "" Then
    '            StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Cod_Contatto = '" & Agro_SQL_SaveText(Codice_ConfCli) & "'   " & vbCrLf)
    '        Else
    '            If RagSoc_ConfCli <> "" Then
    '                StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Rag_Soc LIKE '%" & Agro_SQL_SaveText(RagSoc_ConfCli) & "%'   " & vbCrLf)
    '            End If
    '        End If

    '        '13/06/2017:aggiunto x richiesta cofruta
    '        If Cod_Rapporto <> 0 Then
    '            StbSQL_where_filtri.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
    '        End If

    '        'il Mat_cod è quello del prodotto oggetto del conferimento
    '        If Mat_Cod <> 0 Then
    '            StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
    '        End If

    '        If Mat_Des <> "" Then
    '            StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Des LIKE '%" & Agro_SQL_SaveText(Mat_Des) & "%'   " & vbCrLf)
    '        End If

    '        If Cod_Articolo <> "" Then
    '            StbSQL_where_filtri.Append(" AND MP_Raccolta.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(Cod_Articolo) & "%'   " & vbCrLf)
    '        End If

    '        '--------------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StbSQL_where_filtri.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------


    '        'If Ordinamento = "" Then
    '        '    StbSQL_orderby.Append(" ORDER BY Cod_Articolo_Raccolta, Mat_Des_Raccolta, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento " & vbCrLf)
    '        'Else
    '        '    StbSQL_orderby.Append(Ordinamento)
    '        'End If
    '        'Ordinamento fisso
    '        StbSQL_orderby.Append(" ORDER BY Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento ")


    '        If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then

    '            '********************************************************************
    '            '*** CASO 1 :  conferimento DA diversi con Accettazione (F&F OLD VERSION)
    '            ' Es. MINI - TROMBIN
    '            '********************************************************************
    '            StbSQL_select.Length = 0
    '            StbSQL_from.Length = 0
    '            StbSQL_where.Length = 0

    '            StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_Accet_FF_OLD.ToString & vbCrLf)
    '            StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_Accet_FF_OLD.ToString & vbCrLf)
    '            StbSQL_where.Append(StbSQL_where_Accet_base.ToString & StbSQL_where_Accet_FF_OLD.ToString & StbSQL_where_filtri.ToString & vbCrLf)

    '            StbSQL.Append(" -- CASO 1 :  conferimento DA diversi con Accettazione (F&F OLD VERSION) " & vbCrLf)
    '            StbSQL.Append(StbSQL_select.ToString + StbSQL_from.ToString + StbSQL_where.ToString & vbCrLf)


    '            '*********************************************************************
    '            '*** CASO 2 : conferimento DA diversi senza Accettazione (F&F OLD VERSION)
    '            ' Es. MINI - TROMBIN
    '            '*********************************************************************
    '            StbSQL_select.Length = 0
    '            StbSQL_from.Length = 0
    '            StbSQL_where.Length = 0

    '            StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_NoAccett.ToString & vbCrLf)
    '            StbSQL_from.Append(StbSQL_from_base.ToString & vbCrLf)
    '            StbSQL_where.Append(StbSQL_where_NO_Accet.ToString + StbSQL_where_filtri.ToString & vbCrLf)

    '            StbSQL.Append(" UNION ALL " & vbCrLf)
    '            StbSQL.Append(" -- CASO 2 : conferimento DA diversi senza Accettazione (F&F OLD VERSION) " & vbCrLf)
    '            StbSQL.Append(StbSQL_select.ToString + StbSQL_from.ToString + StbSQL_where.ToString & vbCrLf)


    '            '********************************************************************
    '            '*** CASO 4 :  conferimento DA diversi con Accettazione (F&F NEW VERSION)
    '            ' Es. SOLE - 
    '            '********************************************************************
    '            StbSQL_select.Length = 0
    '            StbSQL_from.Length = 0
    '            StbSQL_where.Length = 0

    '            StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_Accet_FF_NEW.ToString & vbCrLf)
    '            StbSQL_from.Append(StbSQL_from_base.ToString & vbCrLf)
    '            StbSQL_where.Append(StbSQL_where_Accet_base.ToString & StbSQL_where_Accet_FF_NEW.ToString & StbSQL_where_filtri.ToString & vbCrLf)

    '            StbSQL.Append(" UNION ALL " & vbCrLf)
    '            StbSQL.Append(" -- CASO 4 :  conferimento DA diversi con Accettazione (F&F NEW VERSION) " & vbCrLf)
    '            StbSQL.Append(StbSQL_select.ToString + StbSQL_from.ToString + StbSQL_where.ToString & vbCrLf)


    '            '*********************************************************************
    '            '*** CASO 5 : conferimento DA diversi senza Accettazione (F&F NEW VERSION)
    '            ' Es. SOLE - 
    '            '*********************************************************************
    '            StbSQL_select.Length = 0
    '            StbSQL_from.Length = 0
    '            StbSQL_where.Length = 0

    '            StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_NoAccett.ToString & vbCrLf)
    '            StbSQL_from.Append(StbSQL_from_base.ToString & vbCrLf)
    '            StbSQL_where.Append(StbSQL_where_NO_Accet.ToString + StbSQL_where_filtri.ToString & vbCrLf)

    '            StbSQL.Append(" UNION ALL " & vbCrLf)
    '            StbSQL.Append(" -- CASO 5 : conferimento DA diversi senza Accettazione (F&F NEW VERSION) " & vbCrLf)
    '            StbSQL.Append(StbSQL_select.ToString + StbSQL_from.ToString + StbSQL_where.ToString & vbCrLf)

    '        Else

    '            '***************************************
    '            '*** CASO 3 : conferimento VERSO diversi
    '            ' Es. AGRISFERA
    '            '***************************************
    '            StbSQL_select.Length = 0
    '            StbSQL_from.Length = 0
    '            StbSQL_where.Length = 0

    '            StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_ConfDiversi.ToString & vbCrLf)
    '            StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_ConfDiversi.ToString & vbCrLf)
    '            StbSQL_where.Append(StbSQL_where_ConfDiversi.ToString + StbSQL_where_filtri.ToString & vbCrLf)

    '            'StbSQL.Append(" UNION ALL " & vbCrLf)
    '            StbSQL.Append(" -- CASO 3 : conferimento VERSO diversi " & vbCrLf)
    '            StbSQL.Append(StbSQL_select.ToString + StbSQL_from.ToString + StbSQL_where.ToString & vbCrLf)

    '        End If

    '        StbSQL.Append(StbSQL_orderby.ToString & vbCrLf)

    '        str_ElencoBolle = StbSQL.ToString


    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------


    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        str_ElencoBolle = ""
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT

    'End Function


    'Passando l'Id_agenda della Bolla di conferimento,
    'si ricava l'Id_Agenda dell'operazione di raccolta collegata
    'e da questa i riferimenti (Id_destinazione, Appezza) agli impianti 
    Public Function RifImpiantiDaRaccolta_Leggi(ByVal Piva As String, _
                                        ByVal Id_Agenda As Integer, _
                                        ByVal CodSpecieVeg As Integer, _
                                         ByVal xFiltroAggiuntivo As String, _
                                         ByVal xOrderBy As String, _
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.RifImpiantiDaRaccolta_Leggi"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable


        Dim StbSQL As New StringBuilder
        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_from As New StringBuilder
        Dim StbSQL_where As New StringBuilder

        StbSQL_select.Length = 0
        StbSQL_from.Length = 0

        StbSQL_where.Length = 0
        StbSQL.Length = 0



        Try

            StbSQL.Append(" SELECT Mov_Destinazioni_RaccoltaImpianti.Piva,Mov_Destinazioni_RaccoltaImpianti.Sa_Cod,Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione,Mov_Destinazioni_RaccoltaImpianti.Appezza,MP_Raccolta.Veg_Cod " & vbCrLf)

            StbSQL.Append(" FROM Mov_Dettagli_Riferimenti " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva = Agenda_Accettazione.PIVA  " & vbCrLf)
            StbSQL.Append("            AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Accettazione.Sa_Cod  " & vbCrLf)
            StbSQL.Append("            AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Accettazione.Id_Agenda  " & vbCrLf)
            StbSQL.Append("INNER JOIN  Agenda Agenda_RaccoltaImpianti ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA  " & vbCrLf)
            StbSQL.Append("           -- Non è valorizzato il Sa_Cod_Rif: AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod  " & vbCrLf)
            StbSQL.Append("            AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda  " & vbCrLf)
            StbSQL.Append("INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA  " & vbCrLf)
            StbSQL.Append("           AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda " & vbCrLf)
            StbSQL.Append("INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti ON Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA  " & vbCrLf)
            StbSQL.Append("            AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda  " & vbCrLf)
            StbSQL.Append("           AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov  " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  " & vbCrLf)
            StbSQL.Append("            AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod " & vbCrLf)
            StbSQL.Append("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda " & vbCrLf)
            StbSQL.Append("            AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov " & vbCrLf)
            StbSQL.Append("            AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime AS MP_Raccolta ON (MP_Raccolta.Elem_Cod=Movimenti_dettagli_RaccoltaImpianti.Elem_Cod AND MP_Raccolta.Mat_Cod=Movimenti_dettagli_RaccoltaImpianti.Mat_Cod) " & vbCrLf)

            StbSQL.Append(" WHERE         (Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & ") " & vbCrLf)
            StbSQL.Append(" AND Movimenti_RaccoltaImpianti.Cau_Mov='2200' " & vbCrLf)


            If Piva <> "" Then
                StbSQL.Append(" AND     Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND     Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  " & vbCrLf)
            End If
            If CodSpecieVeg <> 0 Then
                StbSQL.Append(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(CodSpecieVeg) & "  " & vbCrLf)
            End If

            StbSQL.Append(" GROUP bY Mov_Destinazioni_RaccoltaImpianti.Piva,Mov_Destinazioni_RaccoltaImpianti.Sa_Cod,Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione,Mov_Destinazioni_RaccoltaImpianti.Appezza,MP_Raccolta.Veg_Cod " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    'Lettura dei dati degli impianti
    Public Function DatiImpiantiRaccoltaDaRif_Leggi(ByVal Piva As String, _
                                                    ByVal Sa_Cod As Integer, _
                                                    ByVal Appezza As Integer, _
                                                    ByVal IdDestinazione As Integer, _
                                                    ByVal Veg_Cod As Integer, _
                                                    ByVal xFiltroAggiuntivo As String, _
                                                    ByVal xOrderBy As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.DatiImpiantiRaccoltaDaRif_Leggi"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable


        Dim StbSQL As New StringBuilder
        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_from As New StringBuilder
        Dim StbSQL_where As New StringBuilder
        Dim StbSQL_orderby As New StringBuilder

        StbSQL_select.Length = 0
        StbSQL_from.Length = 0
        StbSQL_where.Length = 0
        StbSQL.Length = 0
        StbSQL_orderby.Length = 0


        Try

            StbSQL_select.Append(" SELECT  Imprese_Progetti.Progetto_Nome, Reg_Impianti.Sup_Imp, Reg_Impianti.Cul_Cod, Cul_des, Reg_Impianti.Grfi_Cod, GruppoFinalita.Grfi_Des, Reg_Impianti.Grva_Cod_Veg, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des, Cultivar.Veg_Cod, veg_des, Reg_Impianti.Setup_Cod,  " & vbCrLf)
            StbSQL_select.Append("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, " & vbCrLf)
            StbSQL_select.Append("         Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta " & vbCrLf)

            StbSQL_from.Append(" FROM Reg_Impianti " & vbCrLf)
            StbSQL_from.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva " & vbCrLf)
            StbSQL_from.Append("    AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod " & vbCrLf)
            StbSQL_from.Append("    AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza " & vbCrLf)
            StbSQL_from.Append("    AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg " & vbCrLf)
            StbSQL_from.Append(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " & vbCrLf)
            StbSQL_from.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)
            StbSQL_from.Append(" INNER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod " & vbCrLf)
            StbSQL_from.Append(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG  = GruppoVarietale.Grva_Cod " & vbCrLf)


            StbSQL_where.Append(" WHERE Reg_Impianti.PIVA  = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            If Sa_Cod <> 0 Then
                StbSQL_where.Append("   AND Reg_Impianti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Appezza <> 0 Then
                StbSQL_where.Append("   AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  " & vbCrLf)
            End If
            If IdDestinazione <> 0 Then
                StbSQL_where.Append("   AND Reg_Impianti.Id_Reg = " & Agro_SQL_SaveNum(IdDestinazione) & "  " & vbCrLf)
            End If
            If Veg_Cod <> 0 Then
                StbSQL_where.Append("   AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_where.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL_orderby.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL_orderby.Append(" ")
            End If

            StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & StbSQL_orderby.ToString)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



    Public Function ElencoBolle(ByVal str_selectDatiBolle As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.ElencoBolle"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim i As Integer
        Dim str_ElencoIdAgendaBolle As String = ""

        Dim StbSQL As New StringBuilder
        StbSQL.Length = 0

        Try

            StbSQL.Append(" Select DISTINCT ELENCO_BOLLE.Id_Agenda  FROM " & vbCrLf)
            StbSQL.Append(" ( " & str_selectDatiBolle & ") AS ELENCO_BOLLE " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            str_ElencoIdAgendaBolle = ""
            For i = 0 To DT.Rows.Count - 1
                str_ElencoIdAgendaBolle = str_ElencoIdAgendaBolle & CStr(DT.Rows(i).Item("Id_Agenda")) & ","
            Next
            If str_ElencoIdAgendaBolle <> "" Then
                str_ElencoIdAgendaBolle = "(" & Left(str_ElencoIdAgendaBolle, Len(str_ElencoIdAgendaBolle) - 1) & ")"
            Else
                str_ElencoIdAgendaBolle = "(0)"
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            str_ElencoIdAgendaBolle = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return str_ElencoIdAgendaBolle

    End Function

    '###################################################################
    Public Function DatiBollePerTracciabilita_XLS(ByVal Piva As String, _
                                           ByVal Codice_ConfCli As String, _
                                           ByVal RagSoc_ConfCli As String, _
                                            ByVal Cod_Rapporto As Integer, _
                                           ByVal Data_Inizio As Date, _
                                           ByVal Data_Fine As Date, _
                                           ByVal Mat_Cod As Integer, _
                                           ByVal Mat_Des As String, _
                                           ByVal Cod_Articolo As String, _
                                           ByVal xFiltroAggiuntivo As String, _
                                           ByRef str_ElencoBolle As String, _
                                           ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione, _
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                           ) As DataTable


        'filtro sa_cod e fabbricato_cod non utilizzati, arriveranno sempre a 0
        'nella pagina di filtro questi menù sono disattivati (nei conferimenti la destinazione è la cella)
        ' ByVal Sa_Cod As Integer, _
        ' ByVal Fabbricato_Cod As Integer, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.DatiBollePerTracciabilita_XLS"

        Dim MessaggioErrore As String = ""

        Dim DT As DataTable

        Dim StbSQL As New StringBuilder
        Dim StbSQL_select As New StringBuilder
        Dim StbSQL_select_base As New StringBuilder
        Dim StbSQL_select_Accet_FF_OLD As New StringBuilder
        Dim StbSQL_select_Accet_FF_NEW As New StringBuilder
        Dim StbSQL_select_NoAccett As New StringBuilder
        Dim StbSQL_select_ConfDiversi As New StringBuilder
        Dim StbSQL_from As New StringBuilder
        Dim StbSQL_from_base As New StringBuilder
        Dim StbSQL_from_NoAccet As New StringBuilder
        Dim StbSQL_from_Accet_FF_OLD As New StringBuilder
        Dim StbSQL_from_ConfDiversi As New StringBuilder
        Dim StbSQL_where As New StringBuilder
        Dim StbSQL_where_Accet_base As New StringBuilder
        Dim StbSQL_where_Accet_FF_OLD As New StringBuilder
        Dim StbSQL_where_Accet_FF_NEW As New StringBuilder
        Dim StbSQL_where_NO_Accet As New StringBuilder
        Dim StbSQL_where_ConfDiversi As New StringBuilder
        Dim StbSQL_where_filtri As New StringBuilder
        Dim StbSQL_orderby As New StringBuilder

        StbSQL_select.Length = 0
        StbSQL_select_base.Length = 0
        StbSQL_select_Accet_FF_NEW.Length = 0
        StbSQL_select_Accet_FF_OLD.Length = 0
        StbSQL_select_NoAccett.Length = 0
        StbSQL_select_ConfDiversi.Length = 0
        StbSQL_from.Length = 0
        StbSQL_from_base.Length = 0
        StbSQL_from_NoAccet.Length = 0
        StbSQL_from_Accet_FF_OLD.Length = 0
        StbSQL_from_ConfDiversi.Length = 0
        StbSQL_where.Length = 0
        StbSQL_where_Accet_base.Length = 0
        StbSQL_where_Accet_FF_OLD.Length = 0
        StbSQL_where_Accet_FF_NEW.Length = 0
        StbSQL_where_NO_Accet.Length = 0
        StbSQL_where_ConfDiversi.Length = 0
        StbSQL_where_filtri.Length = 0
        StbSQL_orderby.Length = 0
        StbSQL.Length = 0

        Try


            StbSQL_select_base.Append(" SELECT 	" & vbCrLf)
            StbSQL_select_base.Append("	Agenda.Id_Agenda, Risorse_Umane_Conferenti.Settore_Des AS Codice_ConfCli, 												" & vbCrLf)
            StbSQL_select_base.Append("  Risorse_Umane_Conferenti.Cod_Contatto AS Piva_ConfCli, ISNULL(Contatti_ConfCli.Rag_Soc, ' ') AS RagSoc_ConfCli,    " & vbCrLf)

            StbSQL_select_base.Append("  0.0 AS Peso_Totale, Mov_Accett.Tara_Veicolo,  " & vbCrLf)
            StbSQL_select_base.Append("  0.0 AS Peso_Lordo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, 0.0 AS Peso_Netto, 0.0 AS Degrado_Perc, 0.0 AS Degrado, 0.0 AS Netto_Pagamento,                                               " & vbCrLf)
            StbSQL_select_base.Append("  Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla,                                                       " & vbCrLf)
            StbSQL_select_base.Append("  Mov_Accett.Data_Movimento AS Data_Bolla, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf,                                                                                                       " & vbCrLf)
            StbSQL_select_base.Append("  Mov_Conf.Doc_Numero AS Doc_Numero_Conf, Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,                                                                                      " & vbCrLf)
            StbSQL_select_base.Append("  Mov_Conf.Data_Movimento AS Data_Conf, " & vbCrLf)

            StbSQL_select_base.Append(" ISNULL( " & vbCrLf)
            StbSQL_select_base.Append(" ( " & vbCrLf)
            StbSQL_select_base.Append("     SELECT  Listino_Des " & vbCrLf)
            StbSQL_select_base.Append("     FROM    Listini_Prezzi " & vbCrLf)
            StbSQL_select_base.Append("     WHERE   Listini_Prezzi.Listino_Cod = Mov_Dett_Raccolta.Listino_Cod " & vbCrLf)
            StbSQL_select_base.Append("     AND     Listini_Prezzi.Piva = Mov_Dett_Raccolta.Piva " & vbCrLf)
            StbSQL_select_base.Append("     AND     Listini_Prezzi.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StbSQL_select_base.Append(" ) , '' ) AS Listino, Mov_Dett_Raccolta.Prezzo_Unitario_Netto, " & vbCrLf)

            StbSQL_select_Accet_FF_NEW.Append(" Mov_Accett.Peso " & vbCrLf)
            StbSQL_select_Accet_FF_OLD.Append(" Mov_Accett_Peso.Peso " & vbCrLf)

            StbSQL_select_NoAccett.Append(" Mov_Carico_Peso.Peso AS Peso " & vbCrLf)

            StbSQL_select_ConfDiversi.Append("  Mov_Accett.Peso as Peso ")


            StbSQL_from_base.Append("  FROM  Agenda 																																																																					" & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda                                                                                                                                                     " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Risorse_Umane AS Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm                                                                                                                                                                                         " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Contatti AS Contatti_ConfCli ON Risorse_Umane_Conferenti.Piva = Contatti_ConfCli.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_ConfCli.Cod_Contatto                                                                                                                       " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda                                                                                                                                                             " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Movimenti AS Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda                                                                                                                                            " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Movimenti_dettagli AS Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov                                                                                              " & vbCrLf)
            StbSQL_from_base.Append("  INNER JOIN Materie_Prime AS MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod                                                                                                                                                        " & vbCrLf)

            StbSQL_from_Accet_FF_OLD.Append("  INNER JOIN Movimenti AS Mov_Accett_Peso ON Agenda.PIVA = Mov_Accett_Peso.PIVA AND Agenda.Sa_Cod = Mov_Accett_Peso.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett_Peso.Id_Agenda   " & vbCrLf)


            'StbSQL_from_1_2.Append("  INNER JOIN Movimenti Mov_Carico ON (Agenda.PIVA = Mov_Carico.PIVA AND Agenda.Sa_Cod = Mov_Carico.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico.Id_Agenda AND Mov_Carico.Modalita='0')                                                                                                                          " & vbCrLf)
            StbSQL_from_NoAccet.Append("  INNER JOIN Movimenti Mov_Carico_Peso ON (Agenda.PIVA = Mov_Carico_Peso.PIVA AND Agenda.Sa_Cod = Mov_Carico_Peso.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico_Peso.Id_Agenda AND Mov_Carico_Peso.Modalita='1')                                                                                                 " & vbCrLf)

            ''Questa join è fittizia : serve solo per avere nel caso 3 la tabella Mov_Carico_Peso 
            'StbSQL_from_3.Append(" INNER JOIN (select  id_agenda,PIVA,Sa_Cod ,0.0 as peso from Agenda where PIVA = Agenda.PIVA and Sa_Cod=Agenda.sa_cod and Id_agenda=Agenda.Id_Agenda) as Mov_Carico_Peso ON  Mov_Carico_Peso.PIVA=Agenda.PIVA AND Mov_Carico_Peso.Sa_Cod=Agenda.Sa_Cod AND Mov_Carico_Peso.id_agenda=Agenda.id_agenda   " & vbCrLf)

            StbSQL_where_Accet_base.Append(" WHERE (Agenda.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_DISTINTA_CARICO_ACCETTAZIONE) & ")  " & vbCrLf)
            StbSQL_where_Accet_base.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
            StbSQL_where_Accet_base.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Cod_Rapporto = 0 Then
                '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                StbSQL_where_Accet_base.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & _
                                            Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
            End If
            StbSQL_where_Accet_base.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_Accet_base.Append(" AND Mov_Conf.Cau_Mov        = '4050' " & vbCrLf)

            StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Raccolta.Cau_Mov    = '7300' " & vbCrLf)
            StbSQL_where_Accet_FF_NEW.Append(" AND Mov_Dett_Raccolta.elem_cod    = " & CStr(TRASFORMATI_VEGETALI) & " " & vbCrLf)

            StbSQL_where_Accet_FF_OLD.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
            StbSQL_where_Accet_FF_OLD.Append(" AND Mov_Accett_Peso.Cau_Mov = '7920' " & vbCrLf)

            'StbSQL_where_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300'  " & vbCrLf)



            StbSQL_where_NO_Accet.Append(" WHERE (Agenda.Lav_Cod = " & CStr(LAVCOD_BOLLA_RICEVUTA) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_AUTO_DDT_EMESSO) & " or Agenda.Lav_Cod = " & CStr(LAVCOD_DISTINTA_CARICO) & ")  " & vbCrLf)
            StbSQL_where_NO_Accet.Append(" AND Agenda.Modulo = 2 " & vbCrLf)
            StbSQL_where_NO_Accet.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Cod_Rapporto = 0 Then
                '15/06/2020: gestito cod_rapporto -11 personalizzato di fruttagel
                StbSQL_where_NO_Accet.Append(" AND       Risorse_Umane_Conferenti.Cod_Rapporto IN (" & Agro_SQL_SaveNum(COD_CONFERENTE) & "," & _
                                            Agro_SQL_SaveNum(COD_FORNITORE_ORTOFRUTTA) & "," & Agro_SQL_SaveNum(COD_CLIENTEFORNITORE) & ")  " & vbCrLf)
            End If
            StbSQL_where_NO_Accet.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_NO_Accet.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'recupero i dati del ddt dal 4000 (nel caso di doc con accettazione i dati del ddt sono nel 4050)
            StbSQL_where_NO_Accet.Append(" AND Mov_Raccolta.Cau_Mov    = '4070' " & vbCrLf)
            'StbSQL_where_NO_Accet.Append(" AND Mov_Carico.Cau_Mov	    = '7300' " & vbCrLf) '??? chiedere : il 7300 ci sono 2 righe invece di una --> per impostare il criterio di scelta utilizzo il campo Modalita  
            StbSQL_where_NO_Accet.Append(" AND Mov_Carico_Peso.Cau_Mov  = '7300' " & vbCrLf)

            StbSQL_where_ConfDiversi.Append(" WHERE (Agenda.Lav_Cod = 1052)  " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Mov_Accett.Cau_Mov      = '4000' " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Mov_Conf.Cau_Mov        = '4000' " & vbCrLf)  'dati dal ddt
            StbSQL_where_ConfDiversi.Append(" AND Mov_Raccolta.Cau_Mov    = '4200' " & vbCrLf)
            StbSQL_where_ConfDiversi.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)


            StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL_where_filtri.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            '23/07/2019: non serve filtrare contemporaneamente cod_contatto e like su rag_soc (il cod_contatto è più importante)
            'modificato cmq per evitare il problema di report vuoto nel caso di contatti con lettere accentate nella rag_soc:
            'la pagina di pre stampa elimina le lettere accentate nel passaggio via querystring
            If Codice_ConfCli <> "" Then
                StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Cod_Contatto = '" & Agro_SQL_SaveText(Codice_ConfCli) & "'   " & vbCrLf)
            Else
                If RagSoc_ConfCli <> "" Then
                    StbSQL_where_filtri.Append(" AND Contatti_ConfCli.Rag_Soc LIKE '%" & Agro_SQL_SaveText(RagSoc_ConfCli) & "%'   " & vbCrLf)
                End If
            End If

            '13/06/2017:aggiunto x richiesta cofruta
            If Cod_Rapporto <> 0 Then
                StbSQL_where_filtri.Append(" AND Risorse_Umane_Conferenti.Cod_rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & vbCrLf)
            End If
            'il Mat_cod è quello del prodotto oggetto del conferimento
            If Mat_Cod <> 0 Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            If Mat_Des <> "" Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Mat_Des LIKE '%" & Agro_SQL_SaveText(Mat_Des) & "%'   " & vbCrLf)
            End If

            If Cod_Articolo <> "" Then
                StbSQL_where_filtri.Append(" AND MP_Raccolta.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(Cod_Articolo) & "%'   " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL_where_filtri.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------


            'If Ordinamento = "" Then
            '    StbSQL_orderby.Append(" ORDER BY Cod_Articolo_Raccolta, Mat_Des_Raccolta, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento " & vbCrLf)
            'Else
            '    StbSQL_orderby.Append(Ordinamento)
            'End If
            'Ordinamento fisso
            StbSQL_orderby.Append(" ORDER BY Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento ")


            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then

                '********************************************************************
                '*** CASO 1 :  conferimento DA diversi con Accettazione (F&F OLD VERSION)
                ' Es. MINI - TROMBIN
                '********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_Accet_FF_OLD.ToString & vbCrLf)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_Accet_FF_OLD.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_Accet_base.ToString & StbSQL_where_Accet_FF_OLD.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" -- CASO 1 :  conferimento DA diversi con Accettazione (F&F OLD VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


                '*********************************************************************
                '*** CASO 2 : conferimento DA diversi senza Accettazione (F&F OLD VERSION)
                ' Es. MINI - TROMBIN
                '*********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_NoAccett.ToString & vbCrLf)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_NoAccet.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_NO_Accet.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 2 : conferimento DA diversi senza Accettazione (F&F OLD VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


                '********************************************************************
                '*** CASO 4 :  conferimento DA diversi con Accettazione (F&F NEW VERSION)
                ' Es. SOLE - 
                '********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_Accet_FF_NEW.ToString & vbCrLf)
                StbSQL_from.Append(StbSQL_from_base.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_Accet_base.ToString & StbSQL_where_Accet_FF_NEW.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 4 :  conferimento DA diversi con Accettazione (F&F NEW VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)


                '*********************************************************************
                '*** CASO 5 : conferimento DA diversi senza Accettazione (F&F NEW VERSION)
                ' Es. SOLE - 
                '*********************************************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_NoAccett.ToString & vbCrLf)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_NoAccet.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_NO_Accet.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 5 : conferimento DA diversi senza Accettazione (F&F NEW VERSION) " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)

            Else
                '***************************************
                '*** CASO 3 : conferimento VERSO diversi
                ' Es. AGRISFERA
                '***************************************
                StbSQL_select.Length = 0
                StbSQL_from.Length = 0
                StbSQL_where.Length = 0

                StbSQL_select.Append(StbSQL_select_base.ToString & StbSQL_select_ConfDiversi.ToString & vbCrLf)
                StbSQL_from.Append(StbSQL_from_base.ToString & StbSQL_from_ConfDiversi.ToString & vbCrLf)
                StbSQL_where.Append(StbSQL_where_ConfDiversi.ToString & StbSQL_where_filtri.ToString & vbCrLf)

                'StbSQL.Append(" UNION ALL " & vbCrLf)
                StbSQL.Append(" -- CASO 3 : conferimento VERSO diversi " & vbCrLf)
                StbSQL.Append(StbSQL_select.ToString & StbSQL_from.ToString & StbSQL_where.ToString & vbCrLf)

            End If

            str_ElencoBolle = StbSQL.ToString

            StbSQL.Append(StbSQL_orderby.ToString)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            str_ElencoBolle = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function DatiRaccoltaCollegatiABolle(ByVal Piva As String, _
                             ByVal ElencoBolle As String, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByVal xOrderBy As String, _
                             ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ConferimentoAccettazione.DatiRaccoltaCollegatiABolle"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StbSQL As New StringBuilder
        StbSQL.Length = 0

        Try

            StbSQL.Append(" SELECT  " & vbCrLf)
            StbSQL.Append(" Mov_Dettagli_Riferimenti.Id_Agenda AS Id_Agenda_Bolla , Mov_Dettagli_Riferimenti.Id_Agenda_Rif AS IdAgenda_OpRaccolta," & vbCrLf)

            StbSQL.Append(" '' AS Codice_ConfCli,                                                " & vbCrLf)
            StbSQL.Append(" '' AS Piva_ConfCli,  " & vbCrLf)
            StbSQL.Append(" '' AS RagSoc_ConfCli,                                                                         " & vbCrLf)

            StbSQL.Append(" '' AS Peso_Lordo, '' AS Tara_Imballi, '' AS Peso_Netto, " & vbCrLf)

            StbSQL.Append(" '' AS Doc_Numero_Sin, '' AS Doc_Numero, '' AS Doc_Numero_Des, '' AS Numero_Bolla,                                                        " & vbCrLf)
            StbSQL.Append(" '' AS Data_Bolla, '' AS Doc_Numero_Sin_Conf,                                                                                                        " & vbCrLf)
            StbSQL.Append(" '' AS Doc_Numero_Conf, '' AS Doc_Numero_Des_Conf, '' AS Numero_Conf,                                                                                       " & vbCrLf)
            StbSQL.Append(" '' AS Data_Conf,  " & vbCrLf)

            'Dati dell'intera raccolta
            StbSQL.Append(" Movimenti_dettagli_RaccoltaImpianti.Lotto AS Lotto_Raccolta, " & vbCrLf)
            'StbSQL.Append(" SpecieVegetali.Veg_des AS Specie, " & vbCrLf)
            StbSQL.Append(" '' AS Cal_Cod_Raccolta, " & vbCrLf)
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" MPC_indice.Descrizione AS Indice,  " & vbCrLf)
            Else
                StbSQL.Append(" '' AS Indice,  " & vbCrLf)
            End If
            'Dati dettagli raccolta
            StbSQL.Append(" Movimenti_dettagli_RaccoltaImpianti.Udm_Cod AS Udm_Cod_Raccolta, " & vbCrLf)
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" (case when Mov_Destinazioni_RaccoltaImpianti.Id_destinazione >0 then Movimenti_dettagli_RaccoltaImpianti.Qta else 0 end) AS Qta_Raccolta, " & vbCrLf)
            Else
                StbSQL.Append(" Movimenti_dettagliB.qta AS Qta_Raccolta, " & vbCrLf)
            End If
            StbSQL.Append(" UnitaMisura.Udm_Sim AS Unita_misura, " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli_RaccoltaImpianti.Variazione AS Variazione_Raccolta,  " & vbCrLf)
            StbSQL.Append(" '' AS Listino, '' AS Prezzo_Unitario_Netto,  " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Des AS Descr_Specie, ' ' AS Descr_Varieta, MP_Raccolta.Mat_Cod AS Codice_Prodotto,  " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Mat_Des AS Mat_Des_Raccolta,  " & vbCrLf)

            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" Materie_Prime_Calibri.Cal_Des AS Calibro_Des,  " & vbCrLf)
            Else
                StbSQL.Append(" '' AS Calibro_Des,  " & vbCrLf)
            End If
            StbSQL.Append(" 0.0 AS Percentuale_rip , ")
            StbSQL.Append(" (case when Mov_Destinazioni_RaccoltaImpianti.Id_destinazione >0 then Mov_Destinazioni_RaccoltaImpianti.Qta  else 0 end) AS Ripartizioni_qta_imp,")
            StbSQL.Append(" MP_Raccolta.Regolamento AS Regolamento_Prodotto, '' AS Tipologia_Agricoltura,  " & vbCrLf)

            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" (CASE WHEN Cantina_Vasche.Identificativo IS NULL THEN 'Non allocato' ELSE Cantina_Vasche.Identificativo END ) AS Cella_Stiva,  " & vbCrLf)
            End If

            StbSQL.Append(" Imprese_Progetti.Progetto_Nome AS Progetto_Nome,  " & vbCrLf)
            StbSQL.Append(" veg_des AS Veg_Des, " & vbCrLf)
            StbSQL.Append(" Cul_des AS Cul_Des, " & vbCrLf)
            StbSQL.Append(" Reg_Impianti.Sup_Imp AS Sup_Imp, " & vbCrLf)
            StbSQL.Append(" GruppoFinalita.Grfi_Des  AS Grfi_Des, " & vbCrLf)
            StbSQL.Append(" ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des,  " & vbCrLf)
            StbSQL.Append(" Reg_Impianti.Setup_Cod AS Setup_Cod, " & vbCrLf)
            StbSQL.Append(" Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto,  " & vbCrLf)
            StbSQL.Append(" Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto,  " & vbCrLf)
            StbSQL.Append(" Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta,  " & vbCrLf)
            StbSQL.Append(" Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta ")



            StbSQL.Append(" FROM  Agenda Agenda_RaccoltaImpianti  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append("INNER JOIN Mov_Dettagli_Riferimenti  " & vbCrLf)
            StbSQL.Append("             ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA   " & vbCrLf)
            StbSQL.Append("             -- AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod   " & vbCrLf)
            StbSQL.Append("             AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda ")
            StbSQL.Append(" INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA   " & vbCrLf)
            StbSQL.Append("             AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti  " & vbCrLf)
            StbSQL.Append("             ON Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA   " & vbCrLf)
            StbSQL.Append("             AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda   " & vbCrLf)
            StbSQL.Append("             AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti  " & vbCrLf)
            StbSQL.Append("             ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva   " & vbCrLf)
            StbSQL.Append("             AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  " & vbCrLf)
            StbSQL.Append("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  " & vbCrLf)
            StbSQL.Append("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  " & vbCrLf)
            StbSQL.Append("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli_RaccoltaImpianti.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)


            StbSQL.Append("--- Recupero cal_cod dal 7300 dell'operazione di raccolta per Indice campionatura             " & vbCrLf)
            StbSQL.Append("  INNER JOIN  Movimenti Movimenti_RaccoltaMagazzino  " & vbCrLf)
            StbSQL.Append("              ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaMagazzino.PIVA    " & vbCrLf)
            StbSQL.Append("              AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaMagazzino.Id_Agenda   " & vbCrLf)
            StbSQL.Append("  INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaMagazzino   " & vbCrLf)
            StbSQL.Append("              ON Movimenti_RaccoltaMagazzino.PIVA = Movimenti_dettagli_RaccoltaMagazzino.PIVA    " & vbCrLf)
            StbSQL.Append("              AND Movimenti_RaccoltaMagazzino.Id_Agenda = Movimenti_dettagli_RaccoltaMagazzino.Id_Agenda    " & vbCrLf)
            StbSQL.Append("              AND Movimenti_RaccoltaMagazzino.Id_Mov = Movimenti_dettagli_RaccoltaMagazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)


            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append("  INNER JOIN Materie_Prime_Campionature MPC_indice " & vbCrLf)
                StbSQL.Append("             ON MPC_indice.Progressivo = Movimenti_dettagli_RaccoltaMagazzino.cal_cod " & vbCrLf)
                StbSQL.Append("             AND MPC_indice.Tipo='indice' " & vbCrLf)
                StbSQL.Append("  " & vbCrLf)
                StbSQL.Append("  INNER JOIN Materie_Prime_Campionature MPC_calibro  " & vbCrLf)
                StbSQL.Append("             ON MPC_calibro.Tipo_Cod = Movimenti_dettagli_RaccoltaImpianti.cal_cod " & vbCrLf)
                StbSQL.Append("             AND MPC_calibro.Tipo='calibro'   " & vbCrLf)
                StbSQL.Append("             AND MPC_calibro.Progressivo=Movimenti_dettagli_RaccoltaMagazzino.cal_cod                                                 " & vbCrLf)
                StbSQL.Append("  ---            " & vbCrLf)
            End If

            StbSQL.Append(" INNER JOIN Materie_Prime AS MP_Raccolta ON (MP_Raccolta.Elem_Cod=Movimenti_dettagli_RaccoltaImpianti.Elem_Cod AND MP_Raccolta.Mat_Cod=Movimenti_dettagli_RaccoltaImpianti.Mat_Cod) " & vbCrLf)
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" INNER JOIN Materie_Prime_Calibri ON Movimenti_dettagli_RaccoltaImpianti.Cal_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)
            End If

            StbSQL.Append(" LEFT JOIN Reg_Impianti  " & vbCrLf)
            StbSQL.Append("             ON Reg_Impianti.PIVA= Mov_Destinazioni_RaccoltaImpianti.Piva " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione " & vbCrLf)
            StbSQL.Append("  LEFT JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva  " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  " & vbCrLf)
            StbSQL.Append("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg  " & vbCrLf)
            StbSQL.Append("  LEFT JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod  " & vbCrLf)
            StbSQL.Append("             AND Cultivar.Veg_Cod = MP_Raccolta.Veg_Cod  " & vbCrLf)
            StbSQL.Append("  LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  " & vbCrLf)
            StbSQL.Append("  LEFT JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod  " & vbCrLf)
            StbSQL.Append("  LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG  = GruppoVarietale.Grva_Cod " & vbCrLf)

            'F&F : serve per popolare la colonna cella/stiva 
            ' --> serve l'id_destinazione presente nella tabella Mov_destinazioni , Tipo_destinazione=16 , della Bolla   
            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append(" --- Recupero valore colonna Cella_Stiva " & vbCrLf)
                StbSQL.Append("   INNER JOIN Movimenti MovimentiB  " & vbCrLf)
                StbSQL.Append("         ON MovimentiB.PIVA=Mov_Dettagli_Riferimenti.PIVA  " & vbCrLf)
                StbSQL.Append("         AND MovimentiB.Sa_Cod=Mov_Dettagli_Riferimenti.Sa_Cod  " & vbCrLf)
                StbSQL.Append("         AND MovimentiB.Id_Agenda=Mov_Dettagli_Riferimenti.Id_Agenda -- è l'id_agenda della bolla " & vbCrLf)
                StbSQL.Append("   INNER JOIN Movimenti_dettagli Movimenti_dettagliB  " & vbCrLf)
                StbSQL.Append("         ON MovimentiB.PIVA=Movimenti_dettagliB.PIVA  " & vbCrLf)
                StbSQL.Append("         AND MovimentiB.id_agenda=Movimenti_dettagliB.id_agenda  " & vbCrLf)
                StbSQL.Append("         AND MovimentiB.Id_Mov=Movimenti_dettagliB.Id_Mov " & vbCrLf)
                StbSQL.Append("         AND Movimenti_dettagli_RaccoltaImpianti.Mat_cod= Movimenti_dettagliB.Mat_cod " & vbCrLf)
                StbSQL.Append("   INNER JOIN Mov_destinazioni Mov_destinazioniB  " & vbCrLf)
                StbSQL.Append("         ON Movimenti_dettagliB.PIVA=Mov_destinazioniB.PIVA  " & vbCrLf)
                StbSQL.Append("         AND Movimenti_dettagliB.id_agenda=Mov_destinazioniB.id_agenda  " & vbCrLf)
                StbSQL.Append("         AND Movimenti_dettagliB.Id_Mov=Mov_destinazioniB.Id_Mov " & vbCrLf)
                StbSQL.Append("         AND Movimenti_dettagliB.Id_Mov_Det=Mov_destinazioniB.Id_Mov_Det " & vbCrLf)
                StbSQL.Append("         AND Mov_destinazioniB.Tipo_destinazione=16 " & vbCrLf)
                StbSQL.Append("   LEFT JOIN Cantina_Vasche  " & vbCrLf)
                StbSQL.Append("         ON Mov_destinazioniB.Id_destinazione=Cantina_Vasche.Vas_Cod  AND Mov_destinazioniB.sa_cod=Cantina_Vasche.sa_Cod    " & vbCrLf)
                StbSQL.Append("   ---")

            End If


            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append("  WHERE Mov_Dettagli_Riferimenti.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(ElencoBolle) & "  " & vbCrLf)
            StbSQL.Append("  AND Agenda_RaccoltaImpianti.Lav_Cod = 125  " & vbCrLf)
            StbSQL.Append("  AND Movimenti_RaccoltaImpianti.Cau_Mov=  '2200'  " & vbCrLf)
            StbSQL.Append("  AND Movimenti_RaccoltaMagazzino.Cau_Mov= '7300' " & vbCrLf)
            'In F&F le PIVA sono quelle dei conferenti
            'No F&F la raccolta proviene dai propri impianti
            If int_Configurazione_Moduli <> enum_Omni_Modulo_Generazione.FreshFood Then
                StbSQL.Append("  AND Agenda_RaccoltaImpianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'Order by fisso
            'If xOrderBy <> "" Then
            '    StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    StbSQL.Append(" ")
            'End If


            StbSQL.Append(" ORDER BY Id_Agenda_Bolla ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
