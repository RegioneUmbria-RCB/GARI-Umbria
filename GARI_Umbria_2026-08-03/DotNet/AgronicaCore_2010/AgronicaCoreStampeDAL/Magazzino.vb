Imports System.Text
Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL


Public Class Magazzino
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le giacenze di magazzino alla data passata
    ''' fa un sum della qta dei movimenti fino alla data specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzino_SIGPA_Blocco( _
                                                        ByVal SIGPA_ComandiEsportazione_cod As Integer, _
                                                  ByVal PIVA_PadreInGerarchia As String, _
                                                  ByVal Data_Giacenza As Date, _
                                                ByVal Piva As String, _
                                                ByVal Sa_Cod As Integer, _
                                                ByVal Id_Destinazione As Integer, _
                                                ByVal Elem_Cod As Integer, _
                                                ByVal Pro_Cod As Integer, _
                                                ByVal Mat_Cod As Integer, _
                                                ByVal Cal_Cod As Integer, _
                                                ByVal Cod_Progetto As Integer, _
                                                ByVal Fase_Cod As Integer, _
                                                ByVal Udm_Cod As Integer, _
                                                ByVal Lotto As String, _
                                                ByVal Flag_QtaNoZero As Boolean, _
                                                ByVal Esportazione_elem_cod As Integer, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xFiltroAggiuntivo_1 As String, _
                                                ByVal xFiltroAggiuntivo_2 As String, _
                                                ByVal xFiltroAggiuntivo_3 As String, _
                                                ByVal xFiltroAggiuntivo_4 As String, _
                                                ByVal xFiltroAggiuntivo_5 As String, _
                                                ByVal xFiltroAggiuntivo_6 As String, _
                                                ByVal xFiltroAggiuntivo_7 As String, _
                                                ByVal xFiltroAggiuntivo_8 As String, _
                                                ByVal xFiltroAggiuntivo_9 As String, _
                                                ByVal xFiltroAggiuntivo_10 As String, _
                                                ByVal xFiltroAggiuntivo_12 As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Magazzino.SchedaGiacenzeMagazzino()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim DT As DataTable
        Dim i, i_tot As Integer

        Dim gZero As enum_CodiciAnagrafe = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFertilizzanti

        Dim servizio_cod As Integer = 4
        If Esportazione_elem_cod <> 3 Then
            servizio_cod = 3
            gZero = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFormulati
        End If


        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 11
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra bottiglie e trasf raccolti
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fert banca dati e fert azi
                Case Else
                    i_tot = 1
            End Select

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.Append(" UNION ALL ")
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" ( ")

                SQL_Generale.Append(" SELECT Agenda.sa_cod, Agenda.ID_Agenda, Imprese_Codici.val_Cod as CUAA_Impresa, Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.Append(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.Append(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, " & vbCrLf)
                SQL_Generale.Append(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, " & vbCrLf)
                If Esportazione_elem_cod = 3 Then
                    SQL_Generale.Append(" cac.cod_prodotto_Cliente as Codice_Prodotto_SIGPA" & vbCrLf)
                Else
                    SQL_Generale.Append(" Movimenti_dettagli.pro_cod as Codice_Prodotto_SIGPA" & vbCrLf)
                End If

                SQL_Generale.Append(", P.Pratica_cod " & vbCrLf)

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i)



                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" FROM    Agenda " & vbCrLf)

                If SIGPA_ComandiEsportazione_cod > 0 Then
                    SQL_Generale.Append("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp " & vbCrLf)
                    SQL_Generale.Append("    on LImp.piva = Agenda.Piva " & vbCrLf)
                    SQL_Generale.Append("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod) & vbCrLf)
                End If

                SQL_Generale.Append(" left join ( " & vbCrLf)
                SQL_Generale.Append(" Select Piva " & vbCrLf)
                SQL_Generale.Append(" from Imprese_Codici  " & vbCrLf)
                SQL_Generale.Append(" where id_cod =  " & gzero & vbCrLf)
                SQL_Generale.Append(" ) GZero " & vbCrLf)
                SQL_Generale.Append(" on Agenda.PIVA = GZero.PIVA " & vbCrLf)

                SQL_Generale.Append("    inner join Pratiche p " & vbCrLf)
                SQL_Generale.Append("        on p.Piva = Agenda.PIVA   " & vbCrLf)
                SQL_Generale.Append("        and p.Servizio_Cod = " & servizio_cod & vbCrLf)
                SQL_Generale.Append("        and p.piva_superUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
                SQL_Generale.Append("  " & vbCrLf)
                SQL_Generale.Append("    inner join Pratiche_Stati_Attuali SP " & vbCrLf)
                SQL_Generale.Append("        on SP.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
                SQL_Generale.Append("        and SP.Stato_Cod in (2) " & vbCrLf)


                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                If PIVA_PadreInGerarchia <> "-1" Then
                    SQL_Generale.Append(" INNER JOIN GerarchieImpreseEsplosa es " & vbCrLf)
                    SQL_Generale.Append(" ON Agenda.PIVA = es.figlio AND es.padre in (" & Agro_SQL_Save_Clausola_IN(PIVA_PadreInGerarchia, True) & ")  " & vbCrLf)
                End If


                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" INNER JOIN Movimenti " & vbCrLf)
                SQL_Generale.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda" & vbCrLf)

                'JOIN IMPRESE - AGENDA
                SQL_Generale.Append(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva " & vbCrLf)

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

                'JOIN DECODIFICA PRODOTTI
                If Esportazione_elem_cod = 3 Then
                    SQL_Generale.Append(" INNER JOIN CAC_Codifica_ProdottiAziendali cac " & vbCrLf)
                    SQL_Generale.Append(" ON cac.codice_gias = Movimenti_dettagli.pro_cod " & vbCrLf)
                    SQL_Generale.Append(" AND cac.Elem_Cod = 3 " & vbCrLf)
                    SQL_Generale.Append(" AND cac.Tipo_codifica =  " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria & vbCrLf)
                End If

                'JOIN CUAA
                SQL_Generale.Append(" INNER JOIN Imprese_Codici  " & vbCrLf)
                SQL_Generale.Append(" on Imprese_Codici.PIVA = Agenda.PIVA  " & vbCrLf)
                SQL_Generale.Append(" and Imprese_Codici.id_cod = 1010 " & vbCrLf)

                'JOIN FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND " & vbCrLf)
                SQL_Generale.Append(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod " & vbCrLf)

                'JOIN TIPO FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.Append(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)

                'JOIN UNITA DI MISURA
                SQL_Generale.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)

                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i)


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
                SQL_Generale.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
                SQL_Generale.Append(" AND       Agenda.Blocco_Flag = 0  " & vbCrLf)
                SQL_Generale.Append(" AND       GZero.piva is null " & vbCrLf)

                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CALI_LAVORAZIONE) & " " & vbCrLf) 'escludere i cali di lavorazione
                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CORPI_ESTRANEI) & " " & vbCrLf) 'escludere i corpiestranei

                SQL_Generale.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
                '--------------

                SQL_Generale.Append(" AND       Mov_Destinazioni.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "" & vbCrLf)

                If Piva <> "" Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   " & vbCrLf)
                End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   " & vbCrLf)
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   " & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i, _
                                                xFiltroAggiuntivo_1, _
                                                 xFiltroAggiuntivo_2, _
                                                 xFiltroAggiuntivo_3, _
                                                 xFiltroAggiuntivo_4, _
                                                 xFiltroAggiuntivo_5, _
                                                 xFiltroAggiuntivo_6, _
                                                 xFiltroAggiuntivo_7, _
                                                 xFiltroAggiuntivo_8, _
                                                 xFiltroAggiuntivo_9, _
                                                 xFiltroAggiuntivo_10, _
                                                 xFiltroAggiuntivo_12)

                '------------------------------------------------------ 
                '----------------- GROUP BY ---------------------------
                '-----------------------------------------------------


                'SchedeMagazzinoGestioneGroupBy(SQL_Generale, Elem_Cod, i)

                '------------------------------------------------------ 
                '-------------------- HAVING --------------------------
                '------------------------------------------------------

                'If Flag_QtaNoZero = True Then
                '    'condizione per filtrare le giacenze=0
                '    SQL_Generale.Append("  HAVING SUM(CASE WHEN Movimenti.Cau_Mov= '" + Agro_SQL_SaveText(CAU_SCARICO) + "' THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END) <> 0 " & vbCrlF)
                'End If

                SQL_Generale.Append(" ) ")

            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                SQL_Generale.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                SQL_Generale.Append(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le giacenze di magazzino alla data passata
    ''' fa un sum della qta dei movimenti fino alla data specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzino_SIGPA(
                                                 ByVal SIGPA_ComandiEsportazione_cod As Integer,
                                                  ByVal PIVA_PadreInGerarchia As String,
                                                  ByVal Data_Giacenza As Date,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Destinazione As Integer,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Cal_Cod As Integer,
                                                ByVal Cod_Progetto As Integer,
                                                ByVal Fase_Cod As Integer,
                                                ByVal Udm_Cod As Integer,
                                                ByVal Lotto As String,
                                                ByVal Flag_QtaNoZero As Boolean,
                                                ByVal esportazione_elem_cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xFiltroAggiuntivo_1 As String,
                                                ByVal xFiltroAggiuntivo_2 As String,
                                                ByVal xFiltroAggiuntivo_3 As String,
                                                ByVal xFiltroAggiuntivo_4 As String,
                                                ByVal xFiltroAggiuntivo_5 As String,
                                                ByVal xFiltroAggiuntivo_6 As String,
                                                ByVal xFiltroAggiuntivo_7 As String,
                                                ByVal xFiltroAggiuntivo_8 As String,
                                                ByVal xFiltroAggiuntivo_9 As String,
                                                ByVal xFiltroAggiuntivo_10 As String,
                                                ByVal xFiltroAggiuntivo_12 As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Magazzino.SchedaGiacenzeMagazzino()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim DT As DataTable
        Dim i, i_tot As Integer

        Dim gZero As enum_CodiciAnagrafe = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFertilizzanti


        Dim servizio_cod As Integer = 4
        If esportazione_elem_cod <> 3 Then
            servizio_cod = 3
            gZero = enum_CodiciAnagrafe.TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFormulati
        End If

        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 11
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra bottiglie e trasf raccolti
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fert banca dati e fert azi
                Case Else
                    i_tot = 1
            End Select

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.Append(" UNION ALL ")
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" ( ")

                SQL_Generale.Append(" SELECT Imprese_Codici.val_Cod as CUAA_Impresa, Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.Append(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod, Movimenti_dettagli.Udm_Cod,   ")



                SQL_Generale.Append(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, " & vbCrLf)
                SQL_Generale.Append(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des " & vbCrLf)


                If esportazione_elem_cod = 3 Then


                    SQL_Generale.Append(" , case when s.UNITA_Misura is not null then s.UNITA_Misura else  " & vbCrLf)
                    SQL_Generale.Append("           case when udmExtra.UDM_COD = 0 then  " & vbCrLf)
                    SQL_Generale.Append("                   udmSalvata.UDM_DES collate latin1_general_CI_AS " & vbCrLf)
                    SQL_Generale.Append("               Else " & vbCrLf)
                    SQL_Generale.Append("                     udmExtra.UDM_DES " & vbCrLf)
                    SQL_Generale.Append("               End " & vbCrLf)
                    SQL_Generale.Append("           End " & vbCrLf)
                    SQL_Generale.Append("   as udm_des " & vbCrLf)

                    SQL_Generale.Append(" , case when s.UNITA_Misura is not null then s.UNITA_Misura else  " & vbCrLf)
                    SQL_Generale.Append("           case when udmExtra.UDM_COD = 0 then  " & vbCrLf)
                    SQL_Generale.Append("                   udmSalvata.UDM_SIM collate latin1_general_CI_AS " & vbCrLf)
                    SQL_Generale.Append("               Else " & vbCrLf)
                    SQL_Generale.Append("                     udmExtra.UDM_SIM " & vbCrLf)
                    SQL_Generale.Append("               End " & vbCrLf)
                    SQL_Generale.Append("           End " & vbCrLf)
                    SQL_Generale.Append("   as udm_sim " & vbCrLf)


                    SQL_Generale.Append(", cac.cod_prodotto_Cliente as Codice_Prodotto_SIGPA " & vbCrLf)
                Else

                    SQL_Generale.Append(", UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)
                    SQL_Generale.Append(", Movimenti_dettagli.pro_cod as Codice_Prodotto_SIGPA " & vbCrLf)

                End If



                SQL_Generale.Append(" , max(ppContaMagazzini.numeroMagazzini) as numeroMagazzini, p.pratica_cod  " & vbCrLf)

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i)

                Dim parteMovQTA As String = "Mov_Destinazioni.qta"
                If esportazione_elem_cod = 3 Then
                    parteMovQTA = "Mov_Destinazioni.Qta * coalesce(convSigpa.FattoreConversione, 1) * coalesce(convSalvaToExtra.FattoreConversione, 1) "
                End If

                SQL_Generale.Append(" , SUM( CASE WHEN Movimenti.CAU_MOV='" & CStr(CAU_SCARICO) & "' ")
                SQL_Generale.Append("           THEN -(" & parteMovQTA & ")")
                SQL_Generale.Append("           ELSE (" & parteMovQTA & ")")
                SQL_Generale.Append("           END) AS Giacenza ")

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" FROM    Agenda " & vbCrLf)

                If SIGPA_ComandiEsportazione_cod > 0 Then
                    SQL_Generale.Append("    inner join SIGPA_ComandiEsportazione_ListaImprese LImp " & vbCrLf)
                    SQL_Generale.Append("    on LImp.piva = Agenda.Piva " & vbCrLf)
                    SQL_Generale.Append("    and LImp.SIGPA_ComandiEsportazione_cod = " & Agro_SQL_SaveNum(SIGPA_ComandiEsportazione_cod) & vbCrLf)
                End If

                SQL_Generale.Append(" left join ( " & vbCrLf)
                SQL_Generale.Append(" Select Piva " & vbCrLf)
                SQL_Generale.Append(" from Imprese_Codici  " & vbCrLf)
                SQL_Generale.Append(" where id_cod =   " & gZero & vbCrLf)
                SQL_Generale.Append(" ) GZero " & vbCrLf)
                SQL_Generale.Append(" on Agenda.PIVA = GZero.PIVA " & vbCrLf)

                SQL_Generale.Append("    inner join Pratiche p " & vbCrLf)
                SQL_Generale.Append("        on p.Piva = Agenda.PIVA   " & vbCrLf)
                SQL_Generale.Append("        and p.Servizio_Cod = " & servizio_cod & vbCrLf)
                SQL_Generale.Append("        and p.piva_superUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
                SQL_Generale.Append("  " & vbCrLf)
                SQL_Generale.Append("    inner join Pratiche_Stati_Attuali SP " & vbCrLf)
                SQL_Generale.Append("        on SP.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
                SQL_Generale.Append("        and SP.Stato_Cod in (2) " & vbCrLf)


                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                If PIVA_PadreInGerarchia <> "-1" Then
                    SQL_Generale.Append(" INNER JOIN GerarchieImpreseEsplosa es " & vbCrLf)
                    SQL_Generale.Append(" ON Agenda.PIVA = es.figlio AND es.padre in (" & Agro_SQL_Save_Clausola_IN(PIVA_PadreInGerarchia, True) & " ) " & vbCrLf)
                End If

                'JOIN per conteggio dei magazzini
                SQL_Generale.Append("    INNER JOIN (  " & vbCrLf)
                SQL_Generale.Append("         Select Piva, COUNT (*) as numeroMagazzini  " & vbCrLf)
                SQL_Generale.Append("         from Fabbricati   " & vbCrLf)
                SQL_Generale.Append("         group by piva  " & vbCrLf)
                SQL_Generale.Append("     ) ppContaMagazzini   " & vbCrLf)
                SQL_Generale.Append("         on ppContaMagazzini.PIVA = Agenda.PIVA  " & vbCrLf)

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" INNER JOIN Movimenti " & vbCrLf)
                SQL_Generale.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda" & vbCrLf)

                'JOIN IMPRESE - AGENDA
                SQL_Generale.Append(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva " & vbCrLf)

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

                'JOIN DECODIFICA PRODOTTI
                If esportazione_elem_cod = 3 Then
                    SQL_Generale.Append(" INNER JOIN CAC_Codifica_ProdottiAziendali cac " & vbCrLf)
                    SQL_Generale.Append(" ON cac.codice_gias = Movimenti_dettagli.pro_cod " & vbCrLf)
                    SQL_Generale.Append(" AND cac.Elem_Cod = 3 " & vbCrLf)
                    SQL_Generale.Append(" AND cac.Tipo_codifica =  " & enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria & " " & vbCrLf)

                    SQL_Generale.Append("   inner join UnitaMisura udmSalvata " & vbCrLf)
                    SQL_Generale.Append("        on udmSalvata.UDM_COD = Movimenti_dettagli.Udm_Cod " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append(" left JOIN SIGPA_CodificaFertilizzanti s " & vbCrLf)
                    SQL_Generale.Append("    on s.CODICE = cac.Cod_Prodotto_Cliente   " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append(" left JOIN UnitaMisura udmExtra " & vbCrLf)
                    SQL_Generale.Append("    on udmExtra.UDM_COD = Movimenti_dettagli.Extra_Int   " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append(" left JOIN UnitaMisura udmSigpa  " & vbCrLf)
                    SQL_Generale.Append("    on udmSigpa.UDM_SIM = s.UNITA_Misura collate Latin1_General_CI_AS " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append(" left join UnitaMisura_Conversione convSalvaToExtra " & vbCrLf)
                    SQL_Generale.Append("    on convSalvaToExtra.UDM_COD_Da = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
                    SQL_Generale.Append("    and convSalvaToExtra.UDM_COD_A = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end " & vbCrLf)
                    SQL_Generale.Append("  " & vbCrLf)
                    SQL_Generale.Append(" left JOIN UnitaMisura_Conversione convSigpa " & vbCrLf)
                    SQL_Generale.Append("    on convSigpa.UDM_COD_Da = case when  Movimenti_dettagli.Extra_Int = 0 then Movimenti_dettagli.Udm_Cod else Movimenti_dettagli.Extra_Int end " & vbCrLf)
                    SQL_Generale.Append("    and convSigpa.UDM_COD_A = udmSigpa.Udm_Cod " & vbCrLf)

                End If

                'JOIN CUAA
                SQL_Generale.Append(" INNER JOIN Imprese_Codici  " & vbCrLf)
                SQL_Generale.Append(" on Imprese_Codici.PIVA = Agenda.PIVA  " & vbCrLf)
                SQL_Generale.Append(" and Imprese_Codici.id_cod = 1010 " & vbCrLf)

                'JOIN FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND " & vbCrLf)
                SQL_Generale.Append(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod " & vbCrLf)

                'JOIN TIPO FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.Append(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)

                'JOIN UNITA DI MISURA
                SQL_Generale.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)

                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i)


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
                SQL_Generale.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
                SQL_Generale.Append(" AND       Agenda.Blocco_Flag = 0  " & vbCrLf)
                SQL_Generale.Append(" AND       GZero.piva is null " & vbCrLf)

                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CALI_LAVORAZIONE) & " " & vbCrLf) 'escludere i cali di lavorazione
                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CORPI_ESTRANEI) & " " & vbCrLf) 'escludere i corpiestranei

                SQL_Generale.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
                '--------------

                SQL_Generale.Append(" AND       Mov_Destinazioni.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "" & vbCrLf)

                If Piva <> "" Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   " & vbCrLf)
                End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   " & vbCrLf)
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   " & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(xFiltroAggiuntivo)
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i,
                                                xFiltroAggiuntivo_1,
                                                 xFiltroAggiuntivo_2,
                                                 xFiltroAggiuntivo_3,
                                                 xFiltroAggiuntivo_4,
                                                 xFiltroAggiuntivo_5,
                                                 xFiltroAggiuntivo_6,
                                                 xFiltroAggiuntivo_7,
                                                 xFiltroAggiuntivo_8,
                                                 xFiltroAggiuntivo_9,
                                                 xFiltroAggiuntivo_10,
                                                 xFiltroAggiuntivo_12)

                '------------------------------------------------------ 
                '----------------- GROUP BY ---------------------------
                '-----------------------------------------------------

                SQL_Generale.Append(" GROUP BY Imprese_Codici.val_Cod, Imprese.Piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.Append(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.Append(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, " & vbCrLf)
                SQL_Generale.Append(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des,  " & vbCrLf)


                If esportazione_elem_cod = 3 Then
                    SQL_Generale.Append(" s.UNITA_Misura, udmSalvata.UDM_COD, udmExtra.UDM_COD, udmSalvata.UDM_DES, udmExtra.UDM_DES, udmExtra.UDM_SIM, udmSalvata.UDM_SIM, cac.cod_prodotto_Cliente " & vbCrLf)
                Else
                    SQL_Generale.Append(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, Movimenti_dettagli.pro_cod " & vbCrLf)
                End If

                SQL_Generale.Append(", p.pratica_cod   " & vbCrLf)

                SchedeMagazzinoGestioneGroupBy(SQL_Generale, Elem_Cod, i)

                '------------------------------------------------------ 
                '-------------------- HAVING --------------------------
                '------------------------------------------------------

                If Flag_QtaNoZero Then
                    'condizione per filtrare le giacenze=0
                    SQL_Generale.Append("  HAVING SUM(CASE WHEN Movimenti.Cau_Mov= '" & CAU_SCARICO & "' THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END) <> 0 " & vbCrLf)
                End If

                SQL_Generale.Append(" ) ")

            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                SQL_Generale.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                SQL_Generale.Append(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' chiama la query delle giacenze di magazzino
    ''' su quella fa il group by del prodotto e somma la qta
    ''' (per ovviare al fatto che sui trasformati viene generato un nuovo cal_cod negativo per ogni raccolta registrata)
    ''' realizzato x accontentare Immobiliare Dante
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzino_Raggruppamento(ByVal Data_Giacenza As Date,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Destinazione As Integer,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Cal_Cod As Integer,
                                                ByVal Cod_Progetto As Integer,
                                                ByVal Fase_Cod As Integer,
                                                ByVal Udm_Cod As Integer,
                                                ByVal Lotto As String,
                                                ByVal Flag_QtaNoZero As Boolean,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xFiltroAggiuntivo_1 As String,
                                                ByVal xFiltroAggiuntivo_2 As String,
                                                ByVal xFiltroAggiuntivo_3 As String,
                                                ByVal xFiltroAggiuntivo_4 As String,
                                                ByVal xFiltroAggiuntivo_5 As String,
                                                ByVal xFiltroAggiuntivo_6 As String,
                                                ByVal xFiltroAggiuntivo_7 As String,
                                                ByVal xFiltroAggiuntivo_8 As String,
                                                ByVal xFiltroAggiuntivo_9 As String,
                                                ByVal xFiltroAggiuntivo_10 As String,
                                                ByVal xFiltroAggiuntivo_12 As String,
                                                ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal xFiltroAggiuntivo_13 As String = "",
                                                Optional ByVal xFiltroAggiuntivo_14 As String = ""
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Magazzino.SchedaGiacenzeMagazzino_Raggruppamento()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim DT As DataTable
        Dim DTnonserve As DataTable
        Dim StrQuery_Output As String = ""

        Try

            MantieniParametri = True
            'passa "" per l'orderby perché viene fatto qui
            DTnonserve = SchedaGiacenzeMagazzino(Data_Giacenza,
                                                 Piva,
                                                 Sa_Cod,
                                                 Id_Destinazione,
                                                 Elem_Cod,
                                                 Pro_Cod,
                                                 Mat_Cod,
                                                 Cal_Cod,
                                                 Cod_Progetto,
                                                 Fase_Cod,
                                                 Udm_Cod,
                                                 Lotto,
                                                 Flag_QtaNoZero,
                                                 xFiltroAggiuntivo,
                                                 xFiltroAggiuntivo_1,
                                                 xFiltroAggiuntivo_2,
                                                 xFiltroAggiuntivo_3,
                                                 xFiltroAggiuntivo_4,
                                                 xFiltroAggiuntivo_5,
                                                 xFiltroAggiuntivo_6,
                                                 xFiltroAggiuntivo_7,
                                                 xFiltroAggiuntivo_8,
                                                 xFiltroAggiuntivo_9,
                                                 xFiltroAggiuntivo_10,
                                                 xFiltroAggiuntivo_12,
                                                 "NO",
                                                 objParametriServer, objParametriUtenti,
                                                 xFiltroAggiuntivo_13,
                                                 StrQuery_Output,
                                                 xFiltroAggiuntivo_14:=xFiltroAggiuntivo_14,
                                                 eseguiQuery:=False)

            Dim copiaParametri = MyBase.CopiaParametri()

            SQL_Generale.Append(" SELECT Piva, impresa, Tabella, NomeComune, Tabella_Cod, Tabella_Des, Tabella_Tipo, " & vbCrLf)
            SQL_Generale.Append("       Elem_Cod,  Pro_Cod,  Mat_Cod, Cod_Progetto,  Fase_Cod,  Lotto,  " & vbCrLf)
            SQL_Generale.Append("       0 AS Cal_Cod, --serve per non mandare in errore il report, ma non utilizzato  " & vbCrLf)
            SQL_Generale.Append("       Udm_Cod,  Sa_Cod, Id_Destinazione, Tipo_Destinazione,  Fabbricato_Des, Tipo_Fabbricato_Cod, Tipo_Fabbricato_Des, " & vbCrLf)
            SQL_Generale.Append("       Udm_Des, Udm_Sim,  Descrizione_Prodotto, Cod_Articolo, SUM( Giacenza ) AS Giacenza " & vbCrLf)
            SQL_Generale.Append(" FROM " & vbCrLf)
            SQL_Generale.Append(" ( " & vbCrLf)
            SQL_Generale.Append("        " & vbCrLf)

            SQL_Generale.Append(StrQuery_Output & vbCrLf)

            SQL_Generale.Append("       " & vbCrLf)
            SQL_Generale.Append(" ) as QueryGiacenze" & vbCrLf)
            SQL_Generale.Append(" GROUP BY Piva, impresa, Tabella, NomeComune, Tabella_Cod, Tabella_Des, Tabella_Tipo, " & vbCrLf)
            SQL_Generale.Append("       Elem_Cod,  Pro_Cod,  Mat_Cod, Cod_Progetto,  Fase_Cod,  Lotto,  " & vbCrLf)
            SQL_Generale.Append("       Udm_Cod,  Sa_Cod, Id_Destinazione, Tipo_Destinazione,  Fabbricato_Des, Tipo_Fabbricato_Cod, Tipo_Fabbricato_Des, " & vbCrLf)
            SQL_Generale.Append("       Udm_Des, Udm_Sim,  Descrizione_Prodotto, Cod_Articolo " & vbCrLf)
            SQL_Generale.Append("  " & vbCrLf)
            SQL_Generale.Append(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto " & vbCrLf)

            If LivelloCompatibilita(objParametriServer) >= 150 Then
                SQL_Generale.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            SettaParametriPrecedenti(copiaParametri)
            MantieniParametri = False

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DT

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge li prodotti movimentati in magazzino nel range temporale inviato
    ''' fa un sum della qta dei movimenti 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'xFiltroAggiuntivo_13 non è utilizzato!
    Public Function RiepilogoProdottiUtilizzati(ByVal Data_Inizio As Date,
                                                ByVal Data_Fine As Date,
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Destinazione As Integer,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Pro_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Cal_Cod As Integer,
                                                ByVal Cod_Progetto As Integer,
                                                ByVal Fase_Cod As Integer,
                                                ByVal Udm_Cod As Integer,
                                                ByVal Lotto As String,
                                                ByVal Cau_Mov As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xFiltroAggiuntivo_195 As String,
                                                ByVal xFiltroAggiuntivo_2 As String,
                                                ByVal xFiltroAggiuntivo_3 As String,
                                                ByVal xFiltroAggiuntivo_191 As String,
                                                ByVal xFiltroAggiuntivo_198 As String,
                                                ByVal xFiltroAggiuntivo_196 As String,
                                                ByVal xFiltroAggiuntivo_MP As String,
                                                ByVal xFiltroAggiuntivo_197 As String,
                                                ByVal xFiltroAggiuntivo_201a As String,
                                                ByVal xFiltroAggiuntivo_210 As String,
                                                ByVal xFiltroAggiuntivo_400 As String,
                                                ByVal xFiltroAggiuntivo_201b As String,
                                                ByVal xFiltroAggiuntivo_201c As String,
                                                ByVal xFiltroAggiuntivo_310 As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametriServer As AgronicaCoreParametri,
                                                ByRef objParametriUtenti As AgronicaCoreParametri
                                                ) As DataTable


        Const nomeRoutine = "AgronicaCoreStampeDAL.Magazzino.RiepilogoProdottiUtilizzati()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim dt As DataTable
        Dim i, i_tot As Integer
        ' Dim dtParamQual As New DataTable

        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 15
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra CAL_COD = 0 (bottiglie e trasformati acquistati) e CAL_COD <> 0 trasf raccolti
                Case SEMILAVORATI_VEGETALI
                    i_tot = 3
                    '3 cicli per gestire la union tra
                    'CAL_COD <> 0 e COD_PROGETTO <> 0 (semilavorati raccolti)
                    'CAL_COD = 0 e COD_PROGETTO = 0 (semilavorati acquistati)
                    'CAL_COD = 0 e COD_PROGETTO <> 0 (semilavorati importati, esempio Aboca)
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fertilizzanti banca dati e fertilizzanti azi
                Case CARBURANTI
                    i_tot = 2 '2 cicli per gestire la union tra vecchia gestione (banca dati e nuova gestione (materie prime)
                Case Else
                    i_tot = 1
            End Select

            Dim filtroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim dtCentriVisibili As DataTable
                dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametriServer)
                If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To dtCentriVisibili.Rows.Count - 1
                        filtroCentri &= dtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                End If
            End If

            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.AppendLine()
                    SQL_Generale.AppendLine(vbCrLf & " UNION ALL ")
                    SQL_Generale.AppendLine()
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.AppendLine(" ( ")

                'SQL_Generale.AppendLine(" SELECT 1 ")
                SQL_Generale.AppendLine(" SELECT Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ")
                SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                'SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ")
                'SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                'SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, ")
                SQL_Generale.AppendLine("  UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i, False, False)

                SQL_Generale.AppendLine(" , convert(decimal(38,5), SUM(Mov_Destinazioni.qta) ) AS QtaTot")
                SQL_Generale.AppendLine("            ")

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" FROM    Agenda ")

                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti ")
                SQL_Generale.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

                'JOIN IMPRESE - AGENDA
                SQL_Generale.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti_dettagli ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.AppendLine(" INNER JOIN Mov_Destinazioni ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                ''JOIN FABBRICATO
                'SQL_Generale.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                'SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                ''JOIN TIPO FABBRICATO
                'SQL_Generale.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")


                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.AppendLine(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod ")

                'JOIN UNITA DI MISURA
                SQL_Generale.AppendLine(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")

                'JOIN IMPRESE_PROGETTI
                SQL_Generale.AppendLine(" LEFT OUTER JOIN Imprese_Progetti  ON Imprese_Progetti.Piva = Movimenti_Dettagli.Piva And Imprese_Progetti.Progetto_Cod = Movimenti_dettagli.Cod_Progetto ")


                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i, False, False, False, objParametriServer.PivaSuperUser)

                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------

                'Tolto il <= e messo il giorno successivo così da gestire le operazioni con orario che altrimenti verrebbero escluse
                SQL_Generale.AppendLine(" WHERE     Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                SQL_Generale.AppendLine(" And       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

                SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " & CStr(CALI_LAVORAZIONE) & " ") 'escludere i cali di lavorazione
                SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " & CStr(CORPI_ESTRANEI) & " ") 'escludere i corpi estranei

                'SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   ")


                'CAU_ACCETTAZIONE_BENI
                'CAU_ACCETTAZIONE_BENI_DA_DIVERSI
                'CAU_CONFERIMENTO
                'CAU_CONFERIMENTO_DIVERSI
                'luke risponde:   Senza da_diversi è uno scarico, con da_diversi un carico
                Select Case Cau_Mov
                    Case CAU_CARICO
                        SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   ")
                    Case CAU_SCARICO
                        SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "')   ")
                End Select

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Contabilizzato >= 0  ")
                '--------------

                SQL_Generale.AppendLine(" AND       Mov_Destinazioni.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & " ")


                If Piva <> "" Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                Else
                    If filtroCentri <> "" Then
                        SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1)) & ") ) ")
                    End If
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")

                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i,
                                             xFiltroAggiuntivo_195,
                                             xFiltroAggiuntivo_2,
                                             xFiltroAggiuntivo_3,
                                             xFiltroAggiuntivo_191,
                                             xFiltroAggiuntivo_198,
                                             xFiltroAggiuntivo_196,
                                             xFiltroAggiuntivo_MP,
                                             xFiltroAggiuntivo_197,
                                             xFiltroAggiuntivo_201a,
                                             xFiltroAggiuntivo_210,
                                             xFiltroAggiuntivo_400,
                                             xFiltroAggiuntivo_201b,
                                             xFiltroAggiuntivo_201c,
                                             False,
                                             "",
                                             False,
                                             xFiltroAggiuntivo_310)

                '------------------------------------------------------ 
                '----------------- GROUP BY ---------------------------
                '-----------------------------------------------------

                SQL_Generale.AppendLine(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ")
                SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.AppendLine(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")
                'SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ")
                'SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                'SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des ")

                SchedeMagazzinoGestioneGroupBy(SQL_Generale, Elem_Cod, i, False, False)

                '------------------------------------------------------ 
                '-------------------- HAVING --------------------------
                '------------------------------------------------------
                'If Flag_QtaMaggioreZero = True Then
                '    SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & Agro_SQL_SaveText(CAU_SCARICO) & "','" & Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "','" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) > 0 ")
                'Else
                '    If Flag_QtaNoZero = True Then
                '        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & Agro_SQL_SaveText(CAU_SCARICO) & "','" & Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "','" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0 ")
                '    End If
                'End If



                'If Flag_QtaNoZero = True Then
                '    'condizione per filtrare le giacenze=0
                '    If Flag_QtaMaggioreZero = True Then
                '        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & Agro_SQL_SaveText(CAU_SCARICO) & "','" & Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "','" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) > 0 ")
                '    Else
                '        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & Agro_SQL_SaveText(CAU_SCARICO) & "','" & Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "','" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0 ")
                '    End If
                'End If

                SQL_Generale.AppendLine(" ) ")

            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                If xOrderBy.ToUpper <> "NO" Then
                    SQL_Generale.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
                End If
            Else
                SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")
            End If

            ' StrQuery_Output = SQL_Generale.ToString

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le giacenze di magazzino alla data passata
    ''' fa un sum della qta dei movimenti fino alla data specificata
    ''' 
    ''' xFiltroAggiuntivo      ---> filtro generale
    ''' xFiltroAggiuntivo_1    ---> elem_cod = 195
    ''' xFiltroAggiuntivo_2    ---> elem_cod = 2
    ''' xFiltroAggiuntivo_3    ---> elem_cod = 3
    ''' xFiltroAggiuntivo_4    ---> elem_cod = 191
    ''' xFiltroAggiuntivo_5    ---> elem_cod = 198
    ''' xFiltroAggiuntivo_6    ---> elem_cod = 196
    ''' xFiltroAggiuntivo_7    ---> elem_cod = MP (materie prima AKA 10??)
    ''' xFiltroAggiuntivo_8    ---> elem_cod = 197
    ''' xFiltroAggiuntivo_9    ---> elem_cod = 201a     
    ''' xFiltroAggiuntivo_10   ---> elem_cod = 210
    ''' xFiltroAggiuntivo_11   ---> elem_cod = 400
    ''' xFiltroAggiuntivo_12   ---> elem_cod = 201b
    ''' xFiltroAggiuntivo_13   ---> elem_cod = 201c
    ''' xFiltroAggiuntivo_14   ---> elem_cod = 310
    ''' xFiltroAggiuntivo_15   ---> elem_cod = 307

    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    'xFiltroAggiuntivo_13 non è utilizzato!
    'StrQuery_Output viene utilizzata da SchedaGiacenzeMagazzino_Raggruppamento
    Public Function SchedaGiacenzeMagazzino(ByVal Data_Giacenza As Date,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Pro_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Fase_Cod As Integer,
                                            ByVal Udm_Cod As Integer,
                                            ByVal Lotto As String,
                                            ByVal Flag_QtaNoZero As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xFiltroAggiuntivo_1 As String,
                                            ByVal xFiltroAggiuntivo_2 As String,
                                            ByVal xFiltroAggiuntivo_3 As String,
                                            ByVal xFiltroAggiuntivo_4 As String,
                                            ByVal xFiltroAggiuntivo_5 As String,
                                            ByVal xFiltroAggiuntivo_6 As String,
                                            ByVal xFiltroAggiuntivo_7 As String,
                                            ByVal xFiltroAggiuntivo_8 As String,
                                            ByVal xFiltroAggiuntivo_9 As String,
                                            ByVal xFiltroAggiuntivo_10 As String,
                                            ByVal xFiltroAggiuntivo_12 As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreParametri,
                                            Optional ByVal xFiltroAggiuntivo_13 As String = "",
                                            Optional ByRef StrQuery_Output As String = "",
                                            Optional ByVal isFreshAndFood As Boolean = False,
                                            Optional ByVal cercaLottoPerLike As Boolean = False,
                                            Optional ByVal xFiltroAggiuntivo_14 As String = "",
                                            Optional ByVal Flag_QtaMaggioreZero As Boolean = False,
                                            Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                            Optional ByVal codArticolo As String = "",
                                            Optional ByVal cercaCodArticoloPerLike As Boolean = False,
                                            Optional ByVal xFiltroAggiuntivo_15 As String = "",
                                            Optional ByVal eseguiQuery As Boolean = True,
                                            Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing,
                                            Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False,
                                            Optional ByVal creaParametriSql As Boolean = True,
                                            Optional ByVal xFiltroAggiuntivo_16 As String = "",
                                            Optional ByVal leggiLinea As Boolean = False,
                                            Optional ByVal filtroMagazziniEsterni As List(Of String) = Nothing,
                                            Optional ByVal calCodEsclusi As String = Nothing
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.Magazzino.SchedaGiacenzeMagazzino()"

        If filtroMagazziniEsterni IsNot Nothing AndAlso filtroMagazziniEsterni.Count > 0 Then
            Piva = ""
        End If

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim dt As DataTable
        Dim i, i_tot As Integer


        Dim dtParamQual As New DataTable
        If isFreshAndFood Then
            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            dtParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametriServer)
        End If

        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 16
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra CAL_COD = 0 (bottiglie e trasformati acquistati) e CAL_COD <> 0 trasf raccolti
                Case SEMILAVORATI_VEGETALI
                    i_tot = 3
                    '3 cicli per gestire la union tra
                    'CAL_COD <> 0 e COD_PROGETTO <> 0 (semilavorati raccolti)
                    'CAL_COD = 0 e COD_PROGETTO = 0 (semilavorati acquistati)
                    'CAL_COD = 0 e COD_PROGETTO <> 0 (semilavorati importati, esempio Aboca)
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fertilizzanti banca dati e fertilizzanti azi
                Case CARBURANTI
                    i_tot = 2 '2 cicli per gestire la union tra vecchia gestione (banca dati e nuova gestione (materie prime)
                Case FARMACI
                    i_tot = 2 '2 cicli per gestire la union tra materie_prime e tabella farmaci
                Case Else
                    i_tot = 1
            End Select

            Dim filtroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim dtCentriVisibili As DataTable
                dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametriServer)
                If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To dtCentriVisibili.Rows.Count - 1
                        filtroCentri &= dtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                End If
            End If

            '15/01/2019: spostato l'add fuori dal ciclo, altrimenti ad ogni giro veniva sommato un giorno alla data fine
            Data_Giacenza = DateAdd(DateInterval.Day, 1, CDate(Data_Giacenza))

            ' Giulia: 29/3/2019: leggo l'opzione per sapere se devo fare join su CAC per Piva o Piva_SuperUser
            Dim flagJoinSuperUserCac As Boolean = GetFlagJoinCac(objParametriUtenti)

            Dim objGruppiMerce As New Gruppi_Merce_R
            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {Piva}, objParametriServer, gruppiMerceDefaultPerCategoria)

                SQL_Generale.Append(strSql.ToString())
                SQL_Generale.AppendLine("")
            End If

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.AppendLine()
                    SQL_Generale.AppendLine(vbCrLf & " UNION ALL ")
                    SQL_Generale.AppendLine()
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.AppendLine(" ( ")

                SQL_Generale.AppendLine(" SELECT Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ")
                SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ")
                SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati.fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                SQL_Generale.AppendLine(" MAX(CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.Piva ELSE Imprese.partitaIvaReale END) AS partitaIvaReale, ")
                SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, ")
                SQL_Generale.AppendLine(" Centri_Aziendali.Sa_Nome ")

                SQL_Generale.AppendLine(" , Isnull(Imprese_Progetti.Progetto_Nome, '') as Lotto_Interno ")

                If isFreshAndFood Then
                    SQL_Generale.AppendLine(" , Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod,  ")
                    SQL_Generale.AppendLine(" Cantina_Insiemi.insieme_des ")

                    SQL_Generale.AppendLine(" , MIN(Movimenti.Data_Movimento) As Data_Movimento  ")

                    For Each paramQual In dtParamQual.Rows
                        If paramQual("Tipo") = 3 OrElse paramQual("Tipo") = 4 OrElse paramQual("Tipo") = 5 Then
                            SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), '') AS FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        End If
                        If paramQual("Tipo") = 1 Then
                            SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod), 0) AS FF_" & paramQual("Tabella_Key") & "_Tipo_Cod  ")

                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                SQL_Generale.AppendLine(" , COALESCE(MAX(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tara_Campionatura), 0) AS FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura  ")
                                SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla), '') AS FF_" & paramQual("Tabella_Key") & "_Sigla  ")
                                SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione), '') AS FF_" & paramQual("Tabella_Key") & "_Descrizione ")
                                SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Codice_Generazione_Link), '') AS FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link ")
                                SQL_Generale.AppendLine(" , COALESCE(MAX(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Mat_Cod_Generazione_Link), '') AS FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link ")
                            End If
                        End If
                    Next

                    SQL_Generale.AppendLine("  , SUM(  CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                    SQL_Generale.AppendLine("           THEN -(Movimenti_dettagli.Tara)")
                    SQL_Generale.AppendLine("           ELSE Movimenti_dettagli.Tara")
                    SQL_Generale.AppendLine("           END  ) AS TaraTotale ")

                    SQL_Generale.AppendLine(" , MIN( Movimenti_dettagli.Qta_Extra) AS Qta_Extra ")

                End If


                SQL_Generale.AppendLine(" , UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")


                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                    SQL_Generale.AppendLine(" ,COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ")
                    SQL_Generale.AppendLine(" ,CASE ")
                    SQL_Generale.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
                    SQL_Generale.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
                    SQL_Generale.AppendLine(" ELSE '' END AS Des_Gruppo_Merce ")
                End If

                SchedeMagazzinoGestioneSelect(SQL_Generale,
                                              Elem_Cod,
                                              i,
                                              isFreshAndFood,
                                              flagRecuperaCodArticolo,
                                              leggiLinea)

                'Il calcolo della giacenza è diverso se è isFreshAndFood ma solo in alcune categorie
                SchedeMagazzinoGestioneSelectGiacenza(SQL_Generale, Elem_Cod, i, isFreshAndFood)

                'Spostata la sua gestione nella funzione sopra
                'If Not isFreshAndFood Then

                '    SQL_Generale.AppendLine(" , convert(decimal(38,5), SUM( Case When Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.qta)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.qta")
                '    SQL_Generale.AppendLine("           END)) AS Giacenza ")

                'Else

                '    SQL_Generale.AppendLine(" , convert(decimal(38,5), SUM( CASE WHEN Movimenti_Dettagli.Udm_Cod = 38 ")
                '    SQL_Generale.AppendLine("           THEN  ")
                '    SQL_Generale.AppendLine("     CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra")
                '    SQL_Generale.AppendLine("           END ")
                '    SQL_Generale.AppendLine("           ELSE ")
                '    SQL_Generale.AppendLine("     CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.qta)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.qta")
                '    SQL_Generale.AppendLine("           END  END)) AS Giacenza ")

                '    SQL_Generale.AppendLine(" , SUM( CASE WHEN Movimenti_Dettagli.Udm_Cod = 38 ")
                '    SQL_Generale.AppendLine("           THEN  ")
                '    SQL_Generale.AppendLine("     CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.qta)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.qta")
                '    SQL_Generale.AppendLine("           END ")
                '    SQL_Generale.AppendLine("           ELSE 0")
                '    SQL_Generale.AppendLine("           END) AS NrConfezioni ")

                '    SQL_Generale.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest1)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest1")
                '    SQL_Generale.AppendLine("           END) AS NrContenitori ")

                '    SQL_Generale.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
                '    SQL_Generale.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest2)")
                '    SQL_Generale.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest2")
                '    SQL_Generale.AppendLine("           END) AS NrImballaggi ")

                'End If
                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" FROM    Agenda ")

                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti ")
                SQL_Generale.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

                'JOIN IMPRESE - AGENDA
                SQL_Generale.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti_dettagli ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.AppendLine(" INNER JOIN Mov_Destinazioni ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                'JOIN MOVIMENTI DESTINAZIONI - CENTRI AZIENDALI 
                SQL_Generale.AppendLine(" INNER JOIN Centri_Aziendali ")
                SQL_Generale.AppendLine(" ON Mov_Destinazioni.PIVA = Centri_Aziendali.Piva AND Mov_Destinazioni.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                If Not isFreshAndFood Then
                    'JOIN FABBRICATO
                    SQL_Generale.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                    'JOIN TIPO FABBRICATO
                    SQL_Generale.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
                Else
                    ' Id_Destinazione potrebbe essere un magazzino o una cella

                    'JOIN FABBRICATO
                    SQL_Generale.AppendLine(" LEFT JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                    'JOIN TIPO FABBRICATO
                    SQL_Generale.AppendLine(" LEFT JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

                    'JOIN Cantina Insiemi per trovare decodifica cella
                    SQL_Generale.AppendLine(" LEFT JOIN Cantina_Vasche ON Mov_Destinazioni.Piva = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.tipo_destinazione = Cantina_Vasche.tipo_destinazione AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.vas_cod ")

                    SQL_Generale.AppendLine(" LEFT JOIN Cantina_Insiemi ON Cantina_Vasche.Insieme_Cod = Cantina_Insiemi.Insieme_Cod ")

                End If

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.AppendLine(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod ")

                'JOIN UNITA DI MISURA
                SQL_Generale.AppendLine(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")

                'JOIN IMPRESE_PROGETTI
                SQL_Generale.AppendLine(" LEFT OUTER JOIN Imprese_Progetti  ON Imprese_Progetti.Piva = Movimenti_Dettagli.Piva And Imprese_Progetti.Progetto_Cod = Movimenti_dettagli.Cod_Progetto ")


                If isFreshAndFood Then
                    'JOIN MATERIE_PRIME_CAMPIONATURE
                    For Each paramQual In dtParamQual.Rows
                        SQL_Generale.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key") &
                                                " ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo " &
                                                " AND Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                        If paramQual("Tipo") = 1 Then
                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                SQL_Generale.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key") &
                                                        " ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod " &
                                                        " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & Agro_SQL_SaveText(paramQual("Tabella_ID")) & "'")
                            End If
                        End If
                    Next

                End If

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                    SQL_Generale.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock) ")
                    SQL_Generale.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametriServer.PivaSuperUser) & "'")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Mat_Cod = Movimenti_dettagli.Mat_Cod")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Pro_Cod = Movimenti_dettagli.Pro_Cod")
                    SQL_Generale.AppendLine("             AND (prodExtraPriv.Piva = Movimenti_dettagli.Piva OR Movimenti_dettagli.Pro_Cod = 0)")
                    SQL_Generale.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
                    SQL_Generale.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
                    SQL_Generale.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
                    SQL_Generale.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = Movimenti_dettagli.Elem_Cod")

                End If

                SchedeMagazzinoGestioneJoin(SQL_Generale,
                                            Elem_Cod,
                                            i,
                                            isFreshAndFood,
                                            flagRecuperaCodArticolo,
                                            flagJoinSuperUserCac,
                                            objParametriServer.PivaSuperUser)

                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------

                'Marco G. 19/01/2018 Tolto il <= e messo il giorno successivo così da gestire le operazioni con orario che altrimenti verrebbero escluse
                'Spostato l'add day fuori dal ciclo sennò ogni volta aggiungeva 1
                SQL_Generale.AppendLine(" WHERE     Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Giacenza) & " ")

                SQL_Generale.AppendLine(" And       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")

                SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " & CALI_LAVORAZIONE & " ") 'escludere i cali di lavorazione
                SQL_Generale.AppendLine(" And       Movimenti_Dettagli.Elem_Cod <> " & CORPI_ESTRANEI & " ") 'escludere i corpi estranei

                SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   ")

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Jolly_Int = " & MagazzinoMovimentato & "   ")
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Contabilizzato >= 0  ")
                '--------------

                If isFreshAndFood Then
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione IN (" & TIPO_DESTINAZIONE_MAGAZZINO & "," & CELLA_FRIGORIFERA & ")")
                    If Elem_Cod = TRASFORMATI_VEGETALI AndAlso Not String.IsNullOrEmpty(calCodEsclusi) Then
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(calCodEsclusi) & ") ")
                    End If
                Else
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_MAGAZZINO & " ")
                End If

                If Piva <> "" Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                Else
                    If filtroMagazziniEsterni IsNot Nothing AndAlso filtroMagazziniEsterni.Count > 0 Then
                        Dim listPivaMagazziniEsterni As String = "'" & String.Join("','", filtroMagazziniEsterni.ToArray()) & "'"
                        SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.Piva IN (" & Agro_SQL_Save_Clausola_IN(listPivaMagazziniEsterni, True) & ") ) ")
                    End If
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                Else
                    If filtroCentri <> "" Then
                        SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1)) & ") ) ")
                    End If
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    If isFreshAndFood AndAlso cercaLottoPerLike Then
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(Lotto) & "%'   ")
                    Else
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If
                End If

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 AndAlso
                   objParametriServer.UtenteUsername <> objParametriServer.SuperUserUsername AndAlso
                   Not inibisciVisibilitaGruppiMerce Then

                    'Calcolo gestione visibilità gruppi merce
                    Dim objGruppiUtenteMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametriServer, objParametriUtenti)
                    Dim dtGruppiUtenteMerce As DataTable = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & Piva & "')", "")

                    If dtGruppiUtenteMerce.Rows.Count > 0 Then
                        SQL_Generale.AppendLine(" AND COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) IN (")
                        SQL_Generale.AppendLine(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", Piva))
                        SQL_Generale.AppendLine(" )")
                    End If
                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, creaParametriSql))
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i,
                                             xFiltroAggiuntivo_1,
                                             xFiltroAggiuntivo_2,
                                             xFiltroAggiuntivo_3,
                                             xFiltroAggiuntivo_4,
                                             xFiltroAggiuntivo_5,
                                             xFiltroAggiuntivo_6,
                                             xFiltroAggiuntivo_7,
                                             xFiltroAggiuntivo_8,
                                             xFiltroAggiuntivo_9,
                                             xFiltroAggiuntivo_10,
                                             xFiltroAggiuntivo_12,
                                             xFiltroAggiuntivo_13,
                                             xFiltroAggiuntivo_14,
                                             flagRecuperaCodArticolo,
                                             codArticolo,
                                             cercaCodArticoloPerLike,
                                             xFiltroAggiuntivo_15,
                                             xFiltroAggiuntivo_16)

                '------------------------------------------------------ 
                '----------------- GROUP BY ---------------------------
                '-----------------------------------------------------

                SQL_Generale.AppendLine(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, ")
                SQL_Generale.AppendLine(" CategorieMagazzino.Tabella_Cod, CategorieMagazzino.Tabella_Des, CategorieMagazzino.Tabella_Tipo, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, Imprese_Progetti.Progetto_Nome, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ")
                SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati.Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, Centri_Aziendali.Sa_Nome,")
                If isFreshAndFood Then
                    SQL_Generale.AppendLine(" Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod,  ")
                    SQL_Generale.AppendLine(" Cantina_Insiemi.insieme_des, ")
                End If
                SQL_Generale.AppendLine(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                    SQL_Generale.AppendLine(", grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, grpMerce.Codice, grpMerce.Descrizione, #DefaultGruppiMerce.Elem_Cod, #DefaultGruppiMerce.Codice, #DefaultGruppiMerce.Descrizione ")
                End If

                SchedeMagazzinoGestioneGroupBy(SQL_Generale,
                                               Elem_Cod,
                                               i,
                                               isFreshAndFood,
                                               flagRecuperaCodArticolo,
                                               leggiLinea)

                '------------------------------------------------------ 
                '-------------------- HAVING --------------------------
                '------------------------------------------------------

                'L'if gestisce
                'a. Flag_QtaMaggioreZero = true, Flag_QtaNoZero = true/false filtro > 0
                'b. Flag_QtaMaggioreZero = false, Flag_QtaNoZero = true filtro <> 0
                'c. Flag_QtaMaggioreZero = false, Flag_QtaNoZero = false filtro non impostato
                'Queste condizioni, si possono ricondurre ai valori dell'impostazione 180 (enum_Gestione_Giacenze) in questo modo:
                'a. [1 SoloPresenti] (giacenza positiva)
                'b. [non corrisponde ad un valore specifico, è una opzione aggiuntiva] (giacenza positiva o negativa)
                'c. [0 SoloMovimentati] (giacenza positiva, negativa o neutra)
                '[2 TuttiProdotti] (uso la query di anagrafica (cau_carico), oppure se voglio vedere la giacenza passo i filtri come nel caso c)
                If Flag_QtaMaggioreZero Then
                    SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) > 0 ")
                Else
                    If Flag_QtaNoZero Then
                        SQL_Generale.AppendLine("  HAVING convert(decimal(38,5), SUM(CASE WHEN Movimenti.Cau_Mov In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0 ")
                    End If
                End If


                SQL_Generale.AppendLine(" ) ")

            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                If xOrderBy.ToUpper <> "NO" Then
                    SQL_Generale.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
                End If
            Else
                'SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")


                If InStr(1, LCase(SQL_Generale.ToString), LCase("Descrizione_Prodotto")) > 0 Then
                    SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")
                Else
                    SQL_Generale.AppendLine(" ORDER BY Impresa, NomeComune ")
                End If

            End If

            'Giulia 03/12/2025: questa va messa come ultima istruzione della query principale dei movimenti perchè altrimenti da errore di sintassi
            If LivelloCompatibilita(objParametriServer) >= 150 AndAlso eseguiQuery Then
                SQL_Generale.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()

                SQL_Generale.AppendLine("")
                SQL_Generale.Append(strSql.ToString())

            End If

            StrQuery_Output = SQL_Generale.ToString

            '--------------------------------------------------------------------------
            If (eseguiQuery) Then
                dt = EseguiQuery_Lettura(objParametriServer, SQL_Generale.ToString, nomeRoutine)
            Else
                dt = New DataTable
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Utilizzata per la select delle schede di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub SchedeMagazzinoGestioneSelect(ByRef SQL_Generale As StringBuilder,
                                             ByVal Elem_Cod As Integer,
                                             ByVal i As Integer,
                                             Optional ByVal isFreshAndFood As Boolean = False,
                                             Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                             Optional ByVal leggiLinea As Boolean = False)

        If Elem_Cod = 0 Then
            Select Case i
                Case 1 'coadiuvanti
                    SQL_Generale.Append(SchedeMagazzino_Select_1(flagRecuperaCodArticolo) & vbCrLf)
                Case 2 'carburanti (vecchia gestione)
                    SQL_Generale.Append(SchedeMagazzino_Select_2(flagRecuperaCodArticolo) & vbCrLf)
                Case 3 'fertilizzanti
                    SQL_Generale.Append(SchedeMagazzino_Select_3(flagRecuperaCodArticolo) & vbCrLf)
                Case 4 'formulati
                    SQL_Generale.Append(SchedeMagazzino_Select_4(flagRecuperaCodArticolo) & vbCrLf)
                Case 5 'inneschi trappole
                    SQL_Generale.Append(SchedeMagazzino_Select_5(flagRecuperaCodArticolo) & vbCrLf)
                Case 6 'insetti utili
                    SQL_Generale.Append(SchedeMagazzino_Select_6(flagRecuperaCodArticolo) & vbCrLf)
                Case 7 'materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
                    SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                Case 8 'trappole
                    SQL_Generale.Append(SchedeMagazzino_Select_8(flagRecuperaCodArticolo) & vbCrLf)
                Case 9 'semilavorati vegetali raccolti
                    SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) & vbCrLf)
                Case 10 'trasformati vegetali CAL_COD <>0
                    SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood, leggiLinea) & vbCrLf)
                Case 11 'trasformati vegetali CAL-COD =0
                    SQL_Generale.Append(SchedeMagazzino_Select_11(leggiLinea) & vbCrLf)
                Case 12 'confezioni prodotti
                    SQL_Generale.Append(SchedeMagazzino_Select_12() & vbCrLf)
                Case 13 'semilavorati vegetali acquistati
                    SQL_Generale.Append(SchedeMagazzino_Select_13() & vbCrLf)
                Case 14 'semilavorati vegetali importati
                    SQL_Generale.Append(SchedeMagazzino_Select_14() & vbCrLf)
                Case 15 'trasformati animali
                    SQL_Generale.Append(SchedeMagazzino_Select_15() & vbCrLf)
                Case 16 'farmaci
                    SQL_Generale.Append(SchedeMagazzino_Select_16(flagRecuperaCodArticolo) & vbCrLf)

            End Select
        Else
            Select Case Elem_Cod
                Case COADIUVANTI
                    SQL_Generale.Append(SchedeMagazzino_Select_1(flagRecuperaCodArticolo) & vbCrLf)
                Case CARBURANTI
                    'devo leggere la tabella carburanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_2(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                    End If
                Case FERTILIZZANTI
                    'devo leggere la tabella fertilizzanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_3(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                    End If
                Case FARMACI
                    'devo leggere la tabella farmaci e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_16(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                    End If
                Case FORMULATI
                    SQL_Generale.Append(SchedeMagazzino_Select_4(flagRecuperaCodArticolo) & vbCrLf)
                Case INNESCHI
                    SQL_Generale.Append(SchedeMagazzino_Select_5(flagRecuperaCodArticolo) & vbCrLf)
                Case INSETTI
                    SQL_Generale.Append(SchedeMagazzino_Select_6(flagRecuperaCodArticolo) & vbCrLf)
                Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE,
                    SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE,
                    MANGIMI, CARBURANTI, RICAMBI, CAT_MAG_SERVIZI_PROFESSIONALI
                    SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                Case TRAPPOLE
                    SQL_Generale.Append(SchedeMagazzino_Select_8(flagRecuperaCodArticolo) & vbCrLf)
                Case SEMILAVORATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) & vbCrLf)
                    ElseIf i = 2 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_13() & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Select_14() & vbCrLf)
                    End If
                Case TRASFORMATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood, leggiLinea) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Select_11(leggiLinea) & vbCrLf)
                    End If
                Case CONFEZIONI_PRODOTTI
                    SQL_Generale.Append(SchedeMagazzino_Select_12() & vbCrLf)
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_Select_15() & vbCrLf)
            End Select
        End If

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' utilizzata per le voci di giacenza della select delle schede di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub SchedeMagazzinoGestioneSelectGiacenza(ByRef SQL_Generale As StringBuilder,
                                                     ByVal Elem_Cod As Integer,
                                                     ByVal i As Integer,
                                                     Optional ByVal isFreshAndFood As Boolean = False)

        If Elem_Cod = 0 Then
            Select Case i
                Case 1 'coadiuvanti
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 2 'carburanti (vecchia gestione)
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 3 'fertilizzanti
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 4 'formulati
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 5 'inneschi trappole
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 6 'insetti utili
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 7 'materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                Case 8 'trappole
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 9 'semilavorati vegetali raccolti
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) & vbCrLf)
                Case 10 'trasformati vegetali CAL_COD <>0
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood) & vbCrLf)
                Case 11 'trasformati vegetali CAL-COD =0
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_11() & vbCrLf)
                Case 12 'confezioni prodotti
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
                Case 13 'semilavorati vegetali acquistati
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_13() & vbCrLf)
                Case 14 'semilavorati vegetali importati
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_14() & vbCrLf)
                Case 15 'trasformati animali
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                Case 16 'farmaci
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
            End Select
        Else
            Select Case Elem_Cod
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'SQL_Generale.Append(SchedeMagazzino_Select_7() & vbCrLf)
                Case SEMILAVORATI_VEGETALI
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'If i = 1 Then
                    '    SQL_Generale.Append(SchedeMagazzino_Select_9(isFreshAndFood) & vbCrLf)
                    'ElseIf i = 2 Then
                    '    SQL_Generale.Append(SchedeMagazzino_Select_13() & vbCrLf)
                    'Else
                    '    SQL_Generale.Append(SchedeMagazzino_Select_14() & vbCrLf)
                    'End If
                Case TRASFORMATI_VEGETALI
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                    'If i = 1 Then
                    '    SQL_Generale.Append(SchedeMagazzino_Select_10(isFreshAndFood) & vbCrLf)
                    'Else
                    '    SQL_Generale.Append(SchedeMagazzino_Select_11() & vbCrLf)
                    'End If
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieSiFF(isFreshAndFood) & vbCrLf)
                Case Else
                    SQL_Generale.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
            End Select
        End If

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' utilizzata per il join delle schede di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub SchedeMagazzinoGestioneJoin(ByRef SQL_Generale As StringBuilder,
                                           ByVal Elem_Cod As Integer,
                                           ByVal i As Integer,
                                           Optional ByVal isFreshAndFood As Boolean = False,
                                           Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                           Optional ByVal flagJoinSuperUserCac As Boolean = False,
                                           Optional ByVal pivaSuperUser As String = "")

        If Elem_Cod = 0 Then
            Select Case i
                Case 1 'coadiuvanti
                    SQL_Generale.Append(SchedeMagazzino_Join_1(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 2 'carburanti (vecchia gestione)
                    SQL_Generale.Append(SchedeMagazzino_Join_2(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 3 'fertilizzanti                    
                    SQL_Generale.Append(SchedeMagazzino_Join_3(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 4 'formulati
                    SQL_Generale.Append(SchedeMagazzino_Join_4(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 5 'inneschi trappole
                    SQL_Generale.Append(SchedeMagazzino_Join_5(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 6 'insetti utili
                    SQL_Generale.Append(SchedeMagazzino_Join_6(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 7 'materie prime
                    SQL_Generale.Append(SchedeMagazzino_Join_7() & vbCrLf)
                Case 8 'trappole
                    SQL_Generale.Append(SchedeMagazzino_Join_8(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case 9 'semilavorati vegetali raccolti
                    SQL_Generale.Append(SchedeMagazzino_Join_9(isFreshAndFood) & vbCrLf)
                Case 10 'trasformati vegetali raccolto (CAL_COD <> 0)
                    SQL_Generale.Append(SchedeMagazzino_Join_10(isFreshAndFood) & vbCrLf)
                Case 11 'trasformati vegetali bottiglie (CAL_COD = 0)
                    SQL_Generale.Append(SchedeMagazzino_Join_11() & vbCrLf)
                Case 12 'confezioni prodotti
                    SQL_Generale.Append(SchedeMagazzino_Join_12() & vbCrLf)
                Case 13 'semilavorati vegetali acquistati
                    SQL_Generale.Append(SchedeMagazzino_Join_13() & vbCrLf)
                    'Case 13 'ore conto terzi
                    '    SQL_Generale.Append(SchedeMagazzino_Join_13() & vbCrLf)
                Case 14 'semilavorati vegetali importati
                    SQL_Generale.Append(SchedeMagazzino_Join_14() & vbCrLf)
                Case 15 'trasformati animali
                    SQL_Generale.Append(SchedeMagazzino_Join_15() & vbCrLf)
                Case 16 'farmaci
                    SQL_Generale.Append(SchedeMagazzino_Join_16(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
            End Select
        Else
            Select Case Elem_Cod
                Case COADIUVANTI
                    SQL_Generale.Append(SchedeMagazzino_Join_1(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case CARBURANTI
                    'devo leggere la tabella carburanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_2(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Join_7() & vbCrLf)
                    End If
                Case FERTILIZZANTI
                    'devo leggere la tabella fertilizzanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_3(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Join_7() & vbCrLf)
                    End If
                Case FARMACI
                    'devo leggere la tabella farmaci e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_16(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Join_7() & vbCrLf)
                    End If
                Case FORMULATI
                    SQL_Generale.Append(SchedeMagazzino_Join_4(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case INNESCHI
                    SQL_Generale.Append(SchedeMagazzino_Join_5(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case INSETTI
                    SQL_Generale.Append(SchedeMagazzino_Join_6(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE,
                    SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE,
                    MANGIMI, CARBURANTI, RICAMBI, CAT_MAG_SERVIZI_PROFESSIONALI
                    SQL_Generale.Append(SchedeMagazzino_Join_7() & vbCrLf)
                Case TRAPPOLE
                    SQL_Generale.Append(SchedeMagazzino_Join_8(flagRecuperaCodArticolo, flagJoinSuperUserCac, pivaSuperUser) & vbCrLf)
                Case SEMILAVORATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_9(isFreshAndFood) & vbCrLf)
                    ElseIf i = 2 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_13() & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Join_14() & vbCrLf)
                    End If
                Case TRASFORMATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Join_10(isFreshAndFood) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Join_11() & vbCrLf)
                    End If
                Case CONFEZIONI_PRODOTTI
                    SQL_Generale.Append(SchedeMagazzino_Join_12() & vbCrLf)
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_Join_15() & vbCrLf)
            End Select
        End If

    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' utilizzata per il where delle schede di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub SchedeMagazzinoGestioneWhere(ByRef SQL_Generale As StringBuilder,
                                            ByVal Elem_Cod As Integer,
                                            ByVal i As Integer,
                                            ByVal xFiltroAggiuntivo_195 As String,
                                            ByVal xFiltroAggiuntivo_2 As String,
                                            ByVal xFiltroAggiuntivo_3 As String,
                                            ByVal xFiltroAggiuntivo_191 As String,
                                            ByVal xFiltroAggiuntivo_198 As String,
                                            ByVal xFiltroAggiuntivo_196 As String,
                                            ByVal xFiltroAggiuntivo_MP As String,
                                            ByVal xFiltroAggiuntivo_197 As String,
                                            ByVal xFiltroAggiuntivo_201a As String,
                                            ByVal xFiltroAggiuntivo_210 As String,
                                            ByVal xFiltroAggiuntivo_400 As String,
                                            Optional ByVal xFiltroAggiuntivo_201b As String = "",
                                            Optional ByVal xFiltroAggiuntivo_201c As String = "",
                                            Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                            Optional ByVal codArticolo As String = "",
                                            Optional ByVal cercaCodArticoloPerLike As Boolean = False,
                                            Optional ByVal xFiltroAggiuntivo_310 As String = "",
                                            Optional ByVal xFiltroAggiuntivo_307 As String = "")

        If Elem_Cod = 0 Then
            Select Case i
                Case 1 'coadiuvanti
                    SQL_Generale.Append(SchedeMagazzino_Where_1(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_195) & vbCrLf)
                Case 2 'carburanti (vecchia gestione)
                    SQL_Generale.Append(SchedeMagazzino_Where_2_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_2) & vbCrLf)
                Case 3 'fertilizzanti
                    SQL_Generale.Append(SchedeMagazzino_Where_3_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_3) & vbCrLf)
                Case 4 'formulati
                    SQL_Generale.Append(SchedeMagazzino_Where_4(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_191) & vbCrLf)
                Case 5 'inneschi trappole
                    SQL_Generale.Append(SchedeMagazzino_Where_5(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_198) & vbCrLf)
                Case 6 'insetti utili
                    SQL_Generale.Append(SchedeMagazzino_Where_6(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_196) & vbCrLf)
                Case 7 'materie prime
                    SQL_Generale.Append(SchedeMagazzino_Where_7(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) & vbCrLf)
                Case 8 'trappole
                    SQL_Generale.Append(SchedeMagazzino_Where_8(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_197) & vbCrLf)
                Case 9 'semilavorati veg raccolti
                    SQL_Generale.Append(SchedeMagazzino_Where_9(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201a) & vbCrLf)
                Case 10 'trasformati veg raccolto
                    SQL_Generale.Append(SchedeMagazzino_Where_10(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) & vbCrLf)
                Case 11 'trasformati veg bottiglie
                    SQL_Generale.Append(SchedeMagazzino_Where_11(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) & vbCrLf)
                Case 12 'confezioni prodotti
                    SQL_Generale.Append(SchedeMagazzino_Where_12(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_400) & vbCrLf)
                Case 13 'semilavorati veg acquistati
                    SQL_Generale.Append(SchedeMagazzino_Where_13(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201b) & vbCrLf)
                Case 14 'semilavorati veg importati
                    SQL_Generale.Append(SchedeMagazzino_Where_14(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201c) & vbCrLf)
                Case 15 'trasformati animali
                    SQL_Generale.Append(SchedeMagazzino_Where_15(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_310) & vbCrLf)
                Case 16 'farmaci
                    SQL_Generale.Append(SchedeMagazzino_Where_16_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_307) & vbCrLf)
            End Select
        Else
            Select Case Elem_Cod
                Case COADIUVANTI
                    SQL_Generale.Append(SchedeMagazzino_Where_1(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_195) & vbCrLf)
                Case CARBURANTI
                    'devo leggere la tabella carburanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_2_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_2) & vbCrLf)
                    Else
                        'il where deve essere sempre il 2 perché deve cercare solo l'elem_cod dei carburanti
                        SQL_Generale.Append(SchedeMagazzino_Where_2_A(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) & vbCrLf)
                    End If
                Case FERTILIZZANTI
                    'devo leggere la tabella fertilizzanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_3_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_3) & vbCrLf)
                    Else
                        'il where utilizza materie prime
                        SQL_Generale.Append(SchedeMagazzino_Where_3_A(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) & vbCrLf)
                    End If
                Case FARMACI
                    'devo leggere la tabella farmaci e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_16_B(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_307) & vbCrLf)
                    Else
                        'il where utilizza materie prime
                        SQL_Generale.Append(SchedeMagazzino_Where_16_A(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) & vbCrLf)
                    End If
                Case FORMULATI
                    SQL_Generale.Append(SchedeMagazzino_Where_4(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_191) & vbCrLf)
                Case INNESCHI
                    SQL_Generale.Append(SchedeMagazzino_Where_5(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_198) & vbCrLf)
                Case INSETTI
                    SQL_Generale.Append(SchedeMagazzino_Where_6(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_196) & vbCrLf)
                Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE,
                    SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE,
                    MANGIMI, CARBURANTI, RICAMBI, CAT_MAG_SERVIZI_PROFESSIONALI
                    SQL_Generale.Append(SchedeMagazzino_Where_7(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_MP) & vbCrLf)
                Case TRAPPOLE
                    SQL_Generale.Append(SchedeMagazzino_Where_8(flagRecuperaCodArticolo, codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_197) & vbCrLf)
                Case SEMILAVORATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_9(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201a) & vbCrLf)
                    ElseIf i = 2 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_13(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201b) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Where_14(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_201c) & vbCrLf)
                    End If
                Case TRASFORMATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_Where_10(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_Where_11(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                        SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_210) & vbCrLf)
                    End If
                Case CONFEZIONI_PRODOTTI
                    SQL_Generale.Append(SchedeMagazzino_Where_12(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_400) & vbCrLf)
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_Where_15(codArticolo, cercaCodArticoloPerLike) & vbCrLf)
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_310) & vbCrLf)
            End Select
        End If

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' utilizzata per il group by delle schede di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub SchedeMagazzinoGestioneGroupBy(ByRef SQL_Generale As StringBuilder,
                                              ByVal Elem_Cod As Integer,
                                              ByVal i As Integer,
                                              Optional ByVal isFreshAndFood As Boolean = False,
                                              Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                              Optional ByVal leggiLinea As Boolean = False)

        If Elem_Cod = 0 Then
            Select Case i
                Case 1 'coadiuvanti
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_1(flagRecuperaCodArticolo) & vbCrLf)
                Case 2 'carburanti (vecchia gestione)
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_2(flagRecuperaCodArticolo) & vbCrLf)
                Case 3 'fertilizzanti
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_3(flagRecuperaCodArticolo) & vbCrLf)
                Case 4 'formulati
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_4(flagRecuperaCodArticolo) & vbCrLf)
                Case 5 'inneschi trappole
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_5(flagRecuperaCodArticolo) & vbCrLf)
                Case 6 'insetti utili
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_6(flagRecuperaCodArticolo) & vbCrLf)
                Case 7 'materie prime (incluso fert azi) tranne semilav e trasf vegetali e trasf animali
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_7() & vbCrLf)
                Case 8 'trappole
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_8(flagRecuperaCodArticolo) & vbCrLf)
                Case 9 'semilavorati vegetali
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_9(isFreshAndFood) & vbCrLf)
                Case 10 'trasformati vegetali raccolto
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_10(isFreshAndFood, leggiLinea) & vbCrLf)
                Case 11 'trasformati vegetali bottiglie
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_11(leggiLinea) & vbCrLf)
                Case 12 'confezioni prodotti
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_12() & vbCrLf)
                Case 13 'semilavorati vegetali acquistati
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_13() & vbCrLf)
                    'Case 13 'confezioni prodotti
                    '    SQL_Generale.Append(SchedeMagazzino_GroupBy_13() & vbCrLf)
                Case 14 'semilavorati vegetali importati
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_14() & vbCrLf)
                Case 15 'trasformati animali
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_15() & vbCrLf)
                Case 16 'farmaci
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_16(flagRecuperaCodArticolo) & vbCrLf)
            End Select
        Else
            Select Case Elem_Cod
                Case COADIUVANTI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_1(flagRecuperaCodArticolo) & vbCrLf)
                Case CARBURANTI
                    'devo leggere la tabella carburanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_2(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_7() & vbCrLf)
                    End If
                Case FERTILIZZANTI
                    'devo leggere la tabella fertilizzanti e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_3(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_7() & vbCrLf)
                    End If
                Case FARMACI
                    'devo leggere la tabella farmaci e materie prime
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_16(flagRecuperaCodArticolo) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_7() & vbCrLf)
                    End If
                Case FORMULATI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_4(flagRecuperaCodArticolo) & vbCrLf)
                Case INNESCHI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_5(flagRecuperaCodArticolo) & vbCrLf)
                Case INSETTI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_6(flagRecuperaCodArticolo) & vbCrLf)
                Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE,
                    SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE,
                    MANGIMI, CARBURANTI, RICAMBI, CAT_MAG_SERVIZI_PROFESSIONALI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_7() & vbCrLf)
                Case TRAPPOLE
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_8(flagRecuperaCodArticolo) & vbCrLf)
                Case SEMILAVORATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_9(isFreshAndFood) & vbCrLf)
                    ElseIf i = 2 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_13() & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_14() & vbCrLf)
                    End If
                Case TRASFORMATI_VEGETALI
                    If i = 1 Then
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_10(isFreshAndFood, leggiLinea) & vbCrLf)
                    Else
                        SQL_Generale.Append(SchedeMagazzino_GroupBy_11(leggiLinea) & vbCrLf)
                    End If
                Case CONFEZIONI_PRODOTTI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_12() & vbCrLf)
                Case TRASFORMATI_ANIMALI
                    SQL_Generale.Append(SchedeMagazzino_GroupBy_15() & vbCrLf)
            End Select
        End If

    End Sub



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le giacenze di magazzino alla data odierna 
    ''' legge il record statico della giacenza (sperando che la qta sia salvata bene, se non lo è bisogna lanciare il verificatore)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    'se Flag_QtaNoZero = True  si filtrano le qta =0
    Public Function SchedaGiacenzeMagazzinoRecordStatico(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Tipo_Scorta As Integer,
                                                        ByVal Scorta_Min As Decimal,
                                                        ByVal Elem_Cod As Integer,
                                                        ByVal Pro_Cod As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Cal_Cod As Integer,
                                                        ByVal Cod_Progetto As Integer,
                                                        ByVal Fase_Cod As Integer,
                                                        ByVal Udm_Cod As Integer,
                                                        ByVal Lotto As String,
                                                        ByVal Flag_QtaNoZero As Boolean,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xFiltroAggiuntivo_1 As String,
                                                        ByVal xFiltroAggiuntivo_2 As String,
                                                        ByVal xFiltroAggiuntivo_3 As String,
                                                        ByVal xFiltroAggiuntivo_4 As String,
                                                        ByVal xFiltroAggiuntivo_5 As String,
                                                        ByVal xFiltroAggiuntivo_6 As String,
                                                        ByVal xFiltroAggiuntivo_7 As String,
                                                        ByVal xFiltroAggiuntivo_8 As String,
                                                        ByVal xFiltroAggiuntivo_9 As String,
                                                        ByVal xFiltroAggiuntivo_10 As String,
                                                        ByVal xFiltroAggiuntivo_12 As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Magazzino.SchedaGiacenzeMagazzino()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim DT As DataTable
        Dim i, i_tot As Integer

        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 11
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire una union 
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fert banca dati e fert azi
                Case Else
                    i_tot = 1
            End Select

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.Append(" UNION ALL ")
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" ( ")

                SQL_Generale.Append(" SELECT Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.*, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Giacenza, Mov_Destinazioni.Qta2, " & vbCrLf)
                SQL_Generale.Append(" Mov_Destinazioni.Tipo_Scorta, Mov_Destinazioni.Scorta_Min, Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i)


                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" FROM    Agenda " & vbCrLf)

                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" INNER JOIN Movimenti " & vbCrLf)
                SQL_Generale.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda" & vbCrLf)

                'JOIN IMPRESE - AGENDA
                SQL_Generale.Append(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva " & vbCrLf)

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

                'JOIN FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND " & vbCrLf)
                SQL_Generale.Append(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod " & vbCrLf)

                'JOIN TIPO FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.Append(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)

                'JOIN UNITA DI MISURA
                SQL_Generale.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)

                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i)


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" WHERE     Agenda.Lav_Cod = -1 " & vbCrLf)
                SQL_Generale.Append(" AND       Movimenti.Cau_Mov = 'GIACENZE' " & vbCrLf)
                'SQL_Generale.Append(" AND       Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
                'SQL_Generale.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
                'SQL_Generale.Append(" AND       Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Giacenza))
                'SQL_Generale.Append(" AND       Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO))
                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CALI_LAVORAZIONE) & " " & vbCrLf) 'escludere i cali di lavorazione
                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CORPI_ESTRANEI) & " " & vbCrLf) 'escludere i corpiestranei
                If Flag_QtaNoZero Then
                    SQL_Generale.Append(" AND       Mov_Destinazioni.Qta <> 0 " & vbCrLf) 'solo giacenze significative
                End If
                SQL_Generale.Append(" AND       Mov_Destinazioni.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "" & vbCrLf)

                If Piva <> "" Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   " & vbCrLf)
                End If

                'If Id_Agenda <> 0 Then
                '    SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   " & vbCrLf)
                'End If

                'If Id_Mov <> 0 Then
                '    SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   " + vbCrLf)
                'End If

                'If Id_Mov_Det <> 0 Then
                '    SQL_Generale.Append(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   " + vbCrLf)
                'End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   " & vbCrLf)
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   " & vbCrLf)
                End If

                If Tipo_Scorta <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Tipo_Scorta = " & Agro_SQL_SaveNum(Tipo_Scorta) & "   " & vbCrLf)
                End If

                If Scorta_Min <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Scorta_Min = " & Agro_SQL_SaveNum(Scorta_Min) & "   " & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i,
                                                xFiltroAggiuntivo_1,
                                                 xFiltroAggiuntivo_2,
                                                 xFiltroAggiuntivo_3,
                                                 xFiltroAggiuntivo_4,
                                                 xFiltroAggiuntivo_5,
                                                 xFiltroAggiuntivo_6,
                                                 xFiltroAggiuntivo_7,
                                                 xFiltroAggiuntivo_8,
                                                 xFiltroAggiuntivo_9,
                                                 xFiltroAggiuntivo_10,
                                                 xFiltroAggiuntivo_12)


                SQL_Generale.Append(" ) ")

            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                SQL_Generale.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                SQL_Generale.Append(" ORDER BY Impresa, NomeComune, Descrizione_Prodotto ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return DT

    End Function

    '###############################################################################

    Private Function GetFlagJoinCac(ByRef objParametriUtenti As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "GetFlagJoinCac"
        Dim flagJoinSuperUserCac As Boolean = False

        Try

            Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim joinSuperUser As String = objImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_JoinCacPivaSuperUser,
                                                                                          objParametriUtenti, 2)

            If joinSuperUser = "1" Then
                flagJoinSuperUserCac = True
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return flagJoinSuperUserCac
    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Coadiuvanti (195)
    ''' </summary>
    Private Function SchedeMagazzino_Select_1(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------
        sqlProdottiSelect.Append(", Coadiuvante.Coad_Des AS Descrizione_Prodotto")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Carburanti (2)
    ''' </summary>
    Private Function SchedeMagazzino_Select_2(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------
        sqlProdottiSelect.Append(" , Carburanti.Car_Des AS Descrizione_Prodotto ")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function


    '###############################################################################
    ''' <summary>
    ''' Select Fertilizzanti (3)
    ''' </summary>
    Private Function SchedeMagazzino_Select_3(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------
        sqlProdottiSelect.AppendLine(" , Fertilizzanti.Fer_Des AS Descrizione_Prodotto")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    ''###############################################################################
    ''Fertilizzanti aziendali
    'Private Function SchedaGiacenzeMagazzino_Select_3B() As String

    '    Dim SQL_Prodotti_Select_3 As New StringBuilder

    '    SQL_Prodotti_Select_3.Length = 0

    '    '------------------------------------------------------ 
    '    '------------------- SELECT ---------------------------
    '    '------------------------------------------------------
    '    SQL_Prodotti_Select_3.Append(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto, Movimenti_Dettagli.Mat_Cod AS Codice_Prodotto ")


    '    Return SQL_Prodotti_Select_3.ToString

    'End Function

    '###############################################################################
    ''' <summary>
    ''' Select Formulati (191)
    ''' </summary>
    Private Function SchedeMagazzino_Select_4(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------
        sqlProdottiSelect.Append(" , Formulati.Fr_Des AS Descrizione_Prodotto ")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Inneschi Trappole (198)
    ''' </summary>
    Private Function SchedeMagazzino_Select_5(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.Append(" , Avversita.Av_des_Vol AS Descrizione_Prodotto ")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Insetti utili (196)
    ''' </summary>
    Private Function SchedeMagazzino_Select_6(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.Append(" , InsettiUtili.Ins_Des AS Descrizione_Prodotto ")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Materie Prime
    ''' </summary>
    Private Function SchedeMagazzino_Select_7() As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_TecnologiaSementi, Materie_Prime.Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Trappole (197)
    ''' </summary>
    Private Function SchedeMagazzino_Select_8(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.Append(" , Trappole.Trap_Des AS Descrizione_Prodotto ")

        If flagRecuperaCodArticolo Then
            sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        Else
            sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Semilavorati vegetali raccolti (201)
    ''' </summary>
    Private Function SchedeMagazzino_Select_9(ByVal isFreshAndFood As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        If isFreshAndFood Then
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ")
        Else
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ")
        End If
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Semilavorati Vegetali acquistati (201)
    ''' </summary>
    Private Function SchedeMagazzino_Select_13() As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Semilavorati Vegetali importati (201)
    ''' </summary>
    Private Function SchedeMagazzino_Select_14() As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Trasformati Vegetali raccolti (210)
    ''' </summary>
    Private Function SchedeMagazzino_Select_10(
        ByVal isFreshAndFood As Boolean,
        ByVal leggiLinea As Boolean
        ) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        If isFreshAndFood Then
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto")
        Else
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des AS Descrizione_Prodotto ")
        End If
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        If leggiLinea Then
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Linea_Cod AS Linea_Cod ")
        End If
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Trasformati Vegetali: bottiglie (210)
    ''' </summary>
    Private Function SchedeMagazzino_Select_11(ByVal leggiLinea As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        If leggiLinea Then
            sqlProdottiSelect.AppendLine(" , Materie_Prime.Linea_Cod AS Linea_Cod ")
        End If
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")

        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Confezioni prodotti (400)
    ''' </summary>
    Private Function SchedeMagazzino_Select_12() As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Trasformati animali (310)
    ''' </summary>
    Private Function SchedeMagazzino_Select_15() As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------

        sqlProdottiSelect.AppendLine(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiSelect.AppendLine(" , (CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END) AS LegatoALinea ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Veg_Cod AS Veg_Cod, Materie_Prime.Cul_Cod AS Cul_Cod ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Regolamento AS Regolamento, Materie_Prime.sem_cod, Materie_Prime.GRVA_COD_VEG ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiSelect.AppendLine(" , '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica ")
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(SchedeMagazzino_Select_Mat_Cod_OMNI())
        sqlProdottiSelect.AppendLine(" , Materie_Prime.Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Select Farmaci (16)
    ''' </summary>
    Private Function SchedeMagazzino_Select_16(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiSelect As New StringBuilder

        '------------------------------------------------------ 
        '------------------- SELECT ---------------------------
        '------------------------------------------------------
        sqlProdottiSelect.AppendLine(" , (Farmaci.Denominazione + ' - ' + Farmaci.Confezione) AS Descrizione_Prodotto")
        sqlProdottiSelect.AppendLine(" , Farmaci.AIC AS Cod_Articolo")

        'If flagRecuperaCodArticolo = True Then
        '    sqlProdottiSelect.AppendLine(" , COALESCE(CAC.Cod_Articolo, '') AS Cod_Articolo ")
        'Else
        '    sqlProdottiSelect.AppendLine(" , '' AS Cod_Articolo ")
        'End If

        sqlProdottiSelect.AppendLine(" , 0 AS LegatoALinea, 0 AS Veg_Cod , 0 AS Cul_Cod, 0 AS Regolamento, 0 AS sem_cod, 0 AS GRVA_COD_VEG, 0 as cat_cod ")
        sqlProdottiSelect.AppendLine(" , 0 As Qta_Extra, 0 As Udm_Cod_Extra, '' AS Veg_Des , '' AS Cul_Des, '' AS Rag_Soc_Proprietaria ")
        sqlProdottiSelect.AppendLine(" , '' AS Utente_Creazione, '' AS Utente_Modifica, '' AS Data_Creazione, '' AS Data_Modifica, 0 AS Otabella_Cod_Base, '' AS Codice_Esterno ")
        sqlProdottiSelect.AppendLine(" , 0 AS Cod_TecnologiaSementi, 0 AS Germinabilita ")
        sqlProdottiSelect.AppendLine(" , 0 As Mat_Cod_OMNI")
        sqlProdottiSelect.AppendLine(" , '' As Extra_Str")

        Return sqlProdottiSelect.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Aggiunge select per passare prodotto OMNI
    ''' </summary>
    Private Function SchedeMagazzino_Select_Mat_Cod_OMNI() As String

        Dim sqlMatCod_Referenza As New StringBuilder
        sqlMatCod_Referenza.AppendLine(" ,CASE WHEN Movimenti_dettagli.Elem_Cod != " & TRASFORMATI_VEGETALI & " THEN 0 ")
        sqlMatCod_Referenza.AppendLine("  ELSE CASE WHEN Materie_Prime.Mat_Cod_Referenza != 0 THEN Materie_Prime.Mat_Cod_Referenza ")
        sqlMatCod_Referenza.AppendLine("       ELSE Movimenti_dettagli.Mat_Cod ")
        sqlMatCod_Referenza.AppendLine("       END ")
        sqlMatCod_Referenza.AppendLine("  END AS Mat_Cod_OMNI")

        Return sqlMatCod_Referenza.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Aggiunge select per passare giacenza quando non ho FF oppure ho categoria precisa (No FF)
    ''' </summary>
    Private Function SchedeMagazzino_Select_Giacenza_CategorieNoFF(ByVal isFreshAndFood As Boolean) As String

        Dim sqlGiacenza As New StringBuilder
        sqlGiacenza.AppendLine(" , CONVERT(decimal(38,5), SUM( ")
        sqlGiacenza.AppendLine("           CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
        sqlGiacenza.AppendLine("                THEN -(Mov_Destinazioni.qta)")
        sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.qta")
        sqlGiacenza.AppendLine("           END) ")
        sqlGiacenza.AppendLine("   ) AS Giacenza ")

        If isFreshAndFood Then
            'Mi è arrivato questo parametro, quindi anche se sono non in categoria FF,
            'per evitare casini inizializzo cmq le colonne altrimenti potrebbe andare in errore
            sqlGiacenza.AppendLine(" , 0 AS NrConfezioni ")
            sqlGiacenza.AppendLine(" , 0 AS NrContenitori ")
            sqlGiacenza.AppendLine(" , 0 AS NrImballaggi ")
        End If

        Return sqlGiacenza.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Aggiunge select per passare giacenza quando ho FF ed è una categoria gestita dal FF
    ''' </summary>
    Private Function SchedeMagazzino_Select_Giacenza_CategorieSiFF(ByVal isFreshAndFood As Boolean) As String

        Dim sqlGiacenza As New StringBuilder

        If isFreshAndFood Then

            sqlGiacenza.AppendLine(" , CONVERT(decimal(38,5), SUM( ")
            sqlGiacenza.AppendLine("           CASE WHEN (Movimenti_Dettagli.Udm_Cod = 38 AND Movimenti_Dettagli.Qta_Extra <> 0) THEN ")
            sqlGiacenza.AppendLine("                CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
            sqlGiacenza.AppendLine("                     THEN -(Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra)")
            sqlGiacenza.AppendLine("                ELSE Mov_Destinazioni.qta * Movimenti_Dettagli.Qta_Extra")
            sqlGiacenza.AppendLine("                END ")
            sqlGiacenza.AppendLine("           ELSE ")
            sqlGiacenza.AppendLine("                CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
            sqlGiacenza.AppendLine("                     THEN -(Mov_Destinazioni.qta)")
            sqlGiacenza.AppendLine("                ELSE Mov_Destinazioni.qta")
            sqlGiacenza.AppendLine("                END ")
            sqlGiacenza.AppendLine("           END) ")
            sqlGiacenza.AppendLine("   ) AS Giacenza ")

            sqlGiacenza.AppendLine(" , SUM( ")
            sqlGiacenza.AppendLine("        CASE WHEN Movimenti_Dettagli.Udm_Cod = 38 THEN  ")
            sqlGiacenza.AppendLine("            CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
            sqlGiacenza.AppendLine("                 THEN -(Mov_Destinazioni.qta)")
            sqlGiacenza.AppendLine("            ELSE Mov_Destinazioni.qta")
            sqlGiacenza.AppendLine("            END ")
            sqlGiacenza.AppendLine("        ELSE 0")
            sqlGiacenza.AppendLine("        END ")
            sqlGiacenza.AppendLine("   ) AS NrConfezioni ")

            sqlGiacenza.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
            sqlGiacenza.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest1)")
            sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest1")
            sqlGiacenza.AppendLine("           END) AS NrContenitori ")

            sqlGiacenza.AppendLine(" , SUM( CASE WHEN Movimenti.CAU_MOV In ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') ")
            sqlGiacenza.AppendLine("           THEN -(Mov_Destinazioni.Qta_Dest2)")
            sqlGiacenza.AppendLine("           ELSE Mov_Destinazioni.Qta_Dest2")
            sqlGiacenza.AppendLine("           END) AS NrImballaggi ")

        Else
            'È categoria possibile di FF, ma non c'è il flag, quindi giro normale
            sqlGiacenza.Append(SchedeMagazzino_Select_Giacenza_CategorieNoFF(isFreshAndFood) & vbCrLf)
        End If

        Return sqlGiacenza.ToString

    End Function

    ''###############################################################################
    ''ore conto terzi
    'Private Function SchedeMagazzino_Select_() As String

    '    Dim SQL_Prodotti_Select_13 As New System.Text.StringBuilder

    '    SQL_Prodotti_Select_13.Length = 0

    '    '------------------------------------------------------ 
    '    '------------------- SELECT ---------------------------
    '    '------------------------------------------------------

    '    SQL_Prodotti_Select_13.Append(" , Materie_Prime.Mat_Des AS Descrizione_Prodotto, Materie_Prime.Cod_Articolo ")

    '    Return SQL_Prodotti_Select_13.ToString

    'End Function

    '###############################################################################
    ''' <summary>
    ''' Join Coadiuvanti (195)
    ''' </summary>
    Private Function SchedeMagazzino_Join_1(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - COADIUVANTI
        sqlProdottiJoin.Append(" INNER JOIN Coadiuvante ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Coadiuvante.Coad_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(COADIUVANTI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Carburanti (2)
    ''' </summary>
    Private Function SchedeMagazzino_Join_2(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - CARBURANTI
        sqlProdottiJoin.Append(" INNER JOIN Carburanti ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Carburanti.Car_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(CARBURANTI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join fertilizzanti (3)
    ''' </summary>
    Private Function SchedeMagazzino_Join_3(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - FERTILIZZANTI
        sqlProdottiJoin.Append(" INNER JOIN Fertilizzanti ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(FERTILIZZANTI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Formulati (191)
    ''' </summary>
    Private Function SchedeMagazzino_Join_4(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - FORMULATI
        sqlProdottiJoin.Append(" INNER JOIN Formulati ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(FORMULATI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Inneschi trappole (198)
    ''' </summary>
    Private Function SchedeMagazzino_Join_5(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - INNESCHI TRAPPOLE
        sqlProdottiJoin.Append(" INNER JOIN Avversita ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Avversita.Av_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(INNESCHI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Insetti utili (196)
    ''' </summary>
    Private Function SchedeMagazzino_Join_6(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - INSETTI UTILI
        sqlProdottiJoin.Append(" INNER JOIN InsettiUtili ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = InsettiUtili.Ins_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(INSETTI, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Materie Prime
    ''' </summary>
    Private Function SchedeMagazzino_Join_7() As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Trappole (197)
    ''' </summary>
    Private Function SchedeMagazzino_Join_8(ByVal flagRecuperaCodArticolo As Boolean,
                                            ByVal flagJoinSuperUserCac As Boolean,
                                            ByVal pivaSuperUser As String
                                            ) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - TRAPPOLE
        sqlProdottiJoin.Append(" INNER JOIN Trappole ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Trappole.Trap_Cod " & vbCrLf)

        If flagRecuperaCodArticolo Then
            sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(TRAPPOLE, flagJoinSuperUserCac, pivaSuperUser))
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Semilavorati raccolti (201)
    ''' </summary>
    Private Function SchedeMagazzino_Join_9(ByVal isFreshAndFood As Boolean) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        'sqlProdottiJoin.AppendLine(" INNER JOIN Imprese_Progetti ON Movimenti_dettagli.PIVA = Imprese_Progetti.Piva AND Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod ")
        If Not isFreshAndFood Then
            sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")
            sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Materie_Prime_Campionature.Tipo_Cod ")
        End If

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Semilavorati Acquistati (201)
    ''' </summary>
    Private Function SchedeMagazzino_Join_13() As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Semilavorati importati (201)
    ''' </summary>
    Private Function SchedeMagazzino_Join_14() As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        'sqlProdottiJoin.AppendLine(" INNER JOIN Imprese_Progetti ON Movimenti_dettagli.PIVA = Imprese_Progetti.Piva AND Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod ")

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Materie Prime
    ''' </summary>
    Private Function SchedeMagazzino_Join_10(ByVal isFreshAndFood As Boolean) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        If Not isFreshAndFood Then
            sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")
            'sqlProdottiJoin.AppendLine(" LEFT OUTER JOIN Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")
            sqlProdottiJoin.AppendLine(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Materie_Prime_Campionature.Tipo_Cod AND Materie_Prime_Campionature.Tipo = 'calibro' ")
            'sqlProdottiJoin.AppendLine(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_Calibri.Cal_Cod = Materie_Prime_Campionature.Tipo_Cod ")
        End If



        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Trasformati Vegetali: bottiglie (210)
    ''' </summary>
    Private Function SchedeMagazzino_Join_11() As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)


        Return sqlProdottiJoin.ToString

    End Function


    '###############################################################################
    ''' <summary>
    ''' Join Confezioni Prodotti (400)
    ''' </summary>
    Private Function SchedeMagazzino_Join_12() As String

        Dim sqlProdottiJoin As New StringBuilder

        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join Trasformati Animali (310)
    ''' </summary>
    Private Function SchedeMagazzino_Join_15() As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
        sqlProdottiJoin.Append(" INNER JOIN Materie_Prime ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

        Return sqlProdottiJoin.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Join farmaci (16)
    ''' </summary>
    Private Function SchedeMagazzino_Join_16(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal flagJoinSuperUserCac As Boolean,
                                             ByVal pivaSuperUser As String) As String

        Dim sqlProdottiJoin As New StringBuilder

        'JOIN MOVIMENTI DETTAGLI - FARMACI
        sqlProdottiJoin.Append(" INNER JOIN Farmaci ")
        sqlProdottiJoin.Append(" ON Movimenti_dettagli.Pro_Cod = Farmaci.Farm_Cod " & vbCrLf)

        'If flagRecuperaCodArticolo = True Then
        '    sqlProdottiJoin.Append(SchedeMagazzino_Join_CacProdotti(FARMACI, flagJoinSuperUserCac, pivaSuperUser))
        'End If

        Return sqlProdottiJoin.ToString

    End Function


    '###############################################################################
    ''' <summary>
    ''' Join su CAC_Codifica_ProdottiAziendali da aggiungere sulle altre Join per ottenere il codice articolo
    ''' </summary>
    Private Function SchedeMagazzino_Join_CacProdotti(ByVal elemCod As String,
                                                      ByVal flagJoinSuperUserCac As Boolean,
                                                      ByVal pivaSuperUser As String
                                                      ) As String

        Dim sqlCacJoin As New StringBuilder

        'filtro ulteriormente per elem cod, perché così se non c'è alcuna codifica per quella categoria faccio meno lavoro
        sqlCacJoin.AppendLine(" OUTER APPLY ( ")
        sqlCacJoin.AppendLine("     SELECT ")
        sqlCacJoin.AppendLine("         CASE  ")
        sqlCacJoin.AppendLine("             WHEN NOT EXISTS(SELECT 1 FROM CAC_Codifica_ProdottiAziendali WHERE Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(elemCod) & "))  ")
        sqlCacJoin.AppendLine("             THEN '' ")
        sqlCacJoin.AppendLine("         ELSE   ")
        sqlCacJoin.AppendLine("             STUFF((SELECT ', ' + Cod_Prodotto_Cliente ")
        sqlCacJoin.AppendLine("                  FROM CAC_Codifica_ProdottiAziendali ")
        sqlCacJoin.AppendLine("             WHERE Elem_Cod = Movimenti_Dettagli.Elem_Cod ")
        sqlCacJoin.AppendLine("             AND Codice_GIAS = Movimenti_Dettagli.Pro_Cod ")

        If flagJoinSuperUserCac AndAlso pivaSuperUser <> "" Then
            sqlCacJoin.AppendLine("             AND Piva_SuperUser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
        Else
            sqlCacJoin.AppendLine("             AND Piva = Movimenti_Dettagli.Piva ")
        End If

        sqlCacJoin.AppendLine("             FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, '') ")
        sqlCacJoin.AppendLine("         END ")
        sqlCacJoin.AppendLine("     AS Cod_Articolo ")
        sqlCacJoin.AppendLine(" ) AS CAC ")

        Return sqlCacJoin.ToString

    End Function

    ''###############################################################################
    ''ore conto terzi
    'Private Function SchedeMagazzino_Join_() As String

    '    Dim SQL_Prodotti_Join_13 As New StringBuilder

    '    SQL_Prodotti_Join_13.Append(" INNER JOIN Materie_Prime ")
    '    SQL_Prodotti_Join_13.Append(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")

    '    Return SQL_Prodotti_Join_13.ToString

    'End Function


    '###############################################################################
    ''' <summary>
    ''' Where Coadiuvanti (195)
    ''' </summary>
    Private Function SchedeMagazzino_Where_1(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal codArticolo As String,
                                             ByVal cercaCodArticoloPerLike As Boolean
                                             ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & COADIUVANTI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Carburanti (2) Aziendali [nuova gestione]
    ''' </summary>
    Private Function SchedeMagazzino_Where_2_A(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & CARBURANTI & " ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Carburanti (2) Banche Dati [non sono più usati, ma serve per retro-compatibilità]
    ''' </summary>
    Private Function SchedeMagazzino_Where_2_B(ByVal flagRecuperaCodArticolo As Boolean,
                                               ByVal codArticolo As String,
                                               ByVal cercaCodArticoloPerLike As Boolean
                                               ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & CARBURANTI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Fertilizzanti (3) Aziendali [non sono più usati, ma serve per retro-compatibilità]
    ''' </summary>
    Private Function SchedeMagazzino_Where_3_A(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & FERTILIZZANTI & " ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Fertilizzanti (3) Banche Dati [nuova gestione]
    ''' </summary>
    Private Function SchedeMagazzino_Where_3_B(ByVal flagRecuperaCodArticolo As Boolean,
                                               ByVal codArticolo As String,
                                               ByVal cercaCodArticoloPerLike As Boolean
                                               ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & FERTILIZZANTI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Formulati (191)
    ''' </summary>
    Private Function SchedeMagazzino_Where_4(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal codArticolo As String,
                                             ByVal cercaCodArticoloPerLike As Boolean
                                             ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & FORMULATI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Inneschi trappole (198)
    ''' </summary>
    Private Function SchedeMagazzino_Where_5(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal codArticolo As String,
                                             ByVal cercaCodArticoloPerLike As Boolean
                                             ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & INNESCHI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Insetti utili (196)
    ''' </summary>
    Private Function SchedeMagazzino_Where_6(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal codArticolo As String,
                                             ByVal cercaCodArticoloPerLike As Boolean
                                             ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & INSETTI & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Materie Prime
    ''' </summary>
    Private Function SchedeMagazzino_Where_7(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod IN  (" & FERTILIZZANTI & ", " & SEMENTI & ", " & ALTRE_MATERIE &
                                    ", " & MATERIE_VEGETALI & ", " & BENI_CONFEZ_VEGETALE &
                                    ", " & SEMILAVORATI_ANIMALI & ", " & MATERIE_ANIMALI & ", " & BENI_CONFEZ_ANIMALE &
                                    ", " & MANGIMI & ", " & FARMACI &
                                    ", " & CARBURANTI & ", " & RICAMBI & ", " & CAT_MAG_SERVIZI_PROFESSIONALI &
                                    " )  ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Semilavorati raccolti (201) [CAL_COD <> 0 e COD_PROGETTO <> 0]
    ''' </summary>
    Private Function SchedeMagazzino_Where_9(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & SEMILAVORATI_VEGETALI & " ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod <> 0 ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto <> 0 ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Semilavorati acquistati (201) [CAL_COD = 0 e COD_PROGETTO = 0]
    ''' </summary>
    Private Function SchedeMagazzino_Where_13(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & SEMILAVORATI_VEGETALI & " ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto = 0 ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Semilavorati importati (201) [CAL_COD = 0 e COD_PROGETTO <> 0]
    ''' </summary>
    Private Function SchedeMagazzino_Where_14(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & SEMILAVORATI_VEGETALI & " ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cod_Progetto <> 0 ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Trasformati vegetali: Cal_Cod <> 0 (210)
    ''' </summary>
    Private Function SchedeMagazzino_Where_10(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & TRASFORMATI_VEGETALI & " ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod <> 0 ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Trasformati vegetali: Cal_Cod = 0 (210)
    ''' </summary>
    Private Function SchedeMagazzino_Where_11(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & TRASFORMATI_VEGETALI & " ")

        'sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod NOT IN ( SELECT Progressivo FROM Materie_Prime_Campionature ) ")
        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Cal_Cod = 0 ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Confezioni Prodotti (400)
    ''' </summary>
    Private Function SchedeMagazzino_Where_12(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & CONFEZIONI_PRODOTTI & " ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Trasformati Animali (310)
    ''' </summary>
    Private Function SchedeMagazzino_Where_15(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & TRASFORMATI_ANIMALI & " ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Farmaci (16) Aziendali [non sono più usati, ma serve per retro-compatibilità]
    ''' </summary>
    Private Function SchedeMagazzino_Where_16_A(ByVal codArticolo As String, ByVal cercaCodArticoloPerLike As Boolean) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & FARMACI & " ")

        If codArticolo <> "" Then
            If cercaCodArticoloPerLike Then
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
            Else
                sqlProdottiWhere.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' Where Farmaci (16) Banche Dati [nuova gestione]
    ''' </summary>
    Private Function SchedeMagazzino_Where_16_B(ByVal flagRecuperaCodArticolo As Boolean,
                                                ByVal codArticolo As String,
                                                ByVal cercaCodArticoloPerLike As Boolean) As String
        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & FARMACI & " ")

        If codArticolo <> "" Then
            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then
                If cercaCodArticoloPerLike Then
                    'sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                    sqlProdottiWhere.AppendLine(" AND Farmaci.AIC LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    'sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                    sqlProdottiWhere.AppendLine(" AND Farmaci.AIC = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If
        End If

        Return sqlProdottiWhere.ToString

    End Function

    ''###############################################################################
    ''ore conto terzi
    'Private Function SchedeMagazzino_Where_() As String

    '    Dim SQL_Prodotti_Where_13 As New StringBuilder

    '    SQL_Prodotti_Where_13.Append(" AND  (Movimenti_dettagli.Elem_Cod = " & CAT_MAG_SERVIZI_PROFESSIONALI & ") ")

    '    Return SQL_Prodotti_Where_13.ToString

    'End Function

    '###############################################################################
    ''' <summary>
    ''' Where Trappole (197)
    ''' </summary>
    Private Function SchedeMagazzino_Where_8(ByVal flagRecuperaCodArticolo As Boolean,
                                             ByVal codArticolo As String,
                                             ByVal cercaCodArticoloPerLike As Boolean
                                             ) As String

        Dim sqlProdottiWhere As New StringBuilder

        sqlProdottiWhere.AppendLine(" AND Movimenti_dettagli.Elem_Cod = " & TRAPPOLE & " ")

        If codArticolo <> "" Then

            'Facendo così se passo un cod articolo, ma non vado a recuperarlo, vedo anche tutti i prodotti di questa categoria... è corretto?!?
            If flagRecuperaCodArticolo Then

                If cercaCodArticoloPerLike Then
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(codArticolo) & "%' ")
                Else
                    sqlProdottiWhere.AppendLine(" AND CAC.Cod_Articolo = '" & Agro_SQL_SaveText(codArticolo) & "' ")
                End If
            End If

        End If

        Return sqlProdottiWhere.ToString

    End Function




    '###############################################################################
    ''' <summary>
    ''' GroupBy Coadiuvanti (195)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_1(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------
        sqlProdottiGroupBy.Append(", Coadiuvante.Coad_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Carburanti (2)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_2(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------
        sqlProdottiGroupBy.Append(" , Carburanti.Car_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Fertilizzanti (3)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_3(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------
        sqlProdottiGroupBy.AppendLine(" , Fertilizzanti.Fer_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Formulati (191)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_4(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------
        sqlProdottiGroupBy.Append(" , Formulati.Fr_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Inneschi (198)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_5(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.Append(" , Avversita.Av_des_Vol ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Insetti utili (196)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_6(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.Append(" , InsettiUtili.Ins_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Materie Prime
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_7() As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_TecnologiaSementi, Materie_Prime.Germinabilita ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Trappole (197)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_8(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.Append(" , Trappole.Trap_Des ")

        If flagRecuperaCodArticolo Then
            sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Semilavorati vegetali raccolti (201)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_9(ByVal isFreshAndFood As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        If isFreshAndFood Then
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ")
        Else
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ")
        End If
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Semilavorati vegetali acquistati (201)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_13() As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Semilavorati vegetali importati (201)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_14() As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Lotto Impianto: ' + Imprese_Progetti.Progetto_Nome ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Trasformati vegetali raccolti (210)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_10(
        ByVal isFreshAndFood As Boolean,
        ByVal leggiLinea As Boolean
        ) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        If isFreshAndFood Then
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des ")
        Else
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des + ' - Campionatura: ' + Materie_Prime_Calibri.Cal_Des ")
        End If
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        If leggiLinea Then
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Linea_Cod ")
        End If
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Trasformati vegetali: bottiglie (210)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_11(ByVal leggiLinea As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        If leggiLinea Then
            sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Linea_Cod ")
        End If
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function


    '###############################################################################
    ''' <summary>
    ''' GroupBy Confezioni prodotti (400)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_12() As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Trasformati Animali (310)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_15() As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------

        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.ChkReferenza, Materie_Prime.Mat_Cod_Referenza ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Regolamento, Materie_Prime.sem_cod ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.GRVA_COD_VEG, Materie_Prime.cat_cod, Materie_Prime.Qta_Extra, Materie_Prime.Udm_Cod_Extra ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Data_Creazione, Materie_Prime.Data_Modifica, Materie_Prime.Otabella_Cod_Base, Materie_Prime.Codice_Esterno  ")
        sqlProdottiGroupBy.AppendLine(" , Materie_Prime.Extra_Str ")

        Return sqlProdottiGroupBy.ToString

    End Function

    '###############################################################################
    ''' <summary>
    ''' GroupBy Farmaci (16)
    ''' </summary>
    Private Function SchedeMagazzino_GroupBy_16(ByVal flagRecuperaCodArticolo As Boolean) As String

        Dim sqlProdottiGroupBy As New StringBuilder

        '------------------------------------------------------ 
        '------------------- GROUP BY ---------------------------
        '------------------------------------------------------
        sqlProdottiGroupBy.AppendLine(" , Farmaci.Denominazione ")
        sqlProdottiGroupBy.AppendLine(" , Farmaci.Confezione ")
        sqlProdottiGroupBy.AppendLine(" , Farmaci.AIC ")

        If flagRecuperaCodArticolo Then
            'sqlProdottiGroupBy.AppendLine(" , CAC.Cod_Articolo ")
            'sqlProdottiGroupBy.AppendLine(" , Cod_Articolo ")
        End If

        Return sqlProdottiGroupBy.ToString

    End Function

    ''###############################################################################
    ''Ore conto terzi
    'Private Function SchedeMagazzino_GroupBy_() As String

    '    Dim SQL_Prodotti_Select_13 As New StringBuilder

    '    SQL_Prodotti_Select_13.Length = 0

    '    '------------------------------------------------------ 
    '    '------------------- GROUP BY ---------------------------
    '    '------------------------------------------------------

    '    SQL_Prodotti_Select_13.Append(" , Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo ")

    '    Return SQL_Prodotti_Select_13.ToString

    'End Function



    '##############################################################################################
    '##############################################################################################
    '##############################################################################################
    '##############################################################################################
    '##############################################################################################

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge i movimenti di magazzino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'default lotto=LOTTO_NONDEFINITO
    Public Function SchedaMovimentiMagazzino(ByVal Data_Inizio As Date,
                                             ByVal Data_Fine As Date,
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Id_Destinazione As Integer,
                                             ByVal Elem_Cod As Integer,
                                             ByVal Pro_Cod As Integer,
                                             ByVal Mat_Cod As Integer,
                                             ByVal Cal_Cod As Integer,
                                             ByVal Cod_Progetto As Integer,
                                             ByVal Fase_Cod As Integer,
                                             ByVal Udm_Cod As Integer,
                                             ByVal Lotto As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xFiltroAggiuntivo_1 As String,
                                             ByVal xFiltroAggiuntivo_2 As String,
                                             ByVal xFiltroAggiuntivo_3 As String,
                                             ByVal xFiltroAggiuntivo_4 As String,
                                             ByVal xFiltroAggiuntivo_5 As String,
                                             ByVal xFiltroAggiuntivo_6 As String,
                                             ByVal xFiltroAggiuntivo_7 As String,
                                             ByVal xFiltroAggiuntivo_8 As String,
                                             ByVal xFiltroAggiuntivo_9 As String,
                                             ByVal xFiltroAggiuntivo_10 As String,
                                             ByVal xFiltroAggiuntivo_12 As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametriServer As AgronicaCoreParametri,
                                             ByRef objParametriUtenti As AgronicaCoreParametri,
                                             Optional ByVal xFiltroAggiuntivo_13 As String = "",
                                             Optional ByVal xFiltroAggiuntivo_14 As String = "",
                                             Optional ByVal flagRecuperaCodArticolo As Boolean = False,
                                             Optional ByVal codArticolo As String = "",
                                             Optional ByVal cercaCodArticoloPerLike As Boolean = False,
                                             Optional ByVal isFreshAndFood As Boolean = False,
                                             Optional ByVal cercaLottoPerLike As Boolean = False,
                                             Optional ByVal xFiltroAggiuntivo_15 As String = "",
                                             Optional ByVal takeFROM_SaCod As Boolean = False,
                                             Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.Magazzino.SchedaMovimentiMagazzino()"

        Dim messaggioErrore As String = ""
        Dim SQL_Generale As New StringBuilder
        Dim dt As DataTable
        Dim i, i_tot As Integer

        Dim dtParamQual As New DataTable
        If isFreshAndFood Then
            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            dtParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametriServer)
        End If

        Try

            Select Case Elem_Cod
                Case 0
                    i_tot = 16
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra CAL_COD = 0 (bottiglie e trasformati acquistati) e CAL_COD <> 0 trasf raccolti
                Case SEMILAVORATI_VEGETALI
                    i_tot = 3
                    '3 cicli per gestire la union tra
                    'CAL_COD <> 0 e COD_PROGETTO <> 0 (semilavorati raccolti)
                    'CAL_COD = 0 e COD_PROGETTO = 0 (semilavorati acquistati)
                    'CAL_COD = 0 e COD_PROGETTO <> 0 (semilavorati importati, esempio Aboca)
                Case FERTILIZZANTI
                    i_tot = 2
                Case CARBURANTI
                    i_tot = 2 '2 cicli per gestire la union tra vecchia gestione (banca dati e nuova gestione (materie prime)
                    'Case FARMACI
                    '    i_tot = 2 '2 cicli per gestire la union tra farmaci e materie prime
                Case Else
                    i_tot = 1
            End Select

            Dim filtroCentri As String = ""
            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim dtCentriVisibili As DataTable
                dtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametriServer)
                If dtCentriVisibili IsNot Nothing AndAlso dtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To dtCentriVisibili.Rows.Count - 1
                        filtroCentri &= dtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                End If
            End If

            '02/11/2018: spostato l'add fuori dal ciclo, altrimenti ad ogni giro veniva sommato un giorno alla data fine
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))

            ' Giulia: 29/3/2019: leggo l'opzione per sapere se devo fare join su CAC per Piva o Piva_SuperUser
            Dim flagJoinSuperUserCac As Boolean = GetFlagJoinCac(objParametriUtenti)


            Dim objGruppiMerce As New Gruppi_Merce_R
            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {Piva}, objParametriServer, gruppiMerceDefaultPerCategoria)

                SQL_Generale.Append(strSql.ToString())
                SQL_Generale.AppendLine("")
            End If

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.AppendLine()
                    SQL_Generale.AppendLine(" UNION ALL ")
                    SQL_Generale.AppendLine()
                Else
                    SQL_Generale.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.AppendLine(" ( ")

                SQL_Generale.AppendLine(" SELECT Imprese.piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune AS Cat_Des, ")
                SQL_Generale.AppendLine(" Agenda.sa_cod AS Sa_Cod_Agenda, Agenda.Id_Agenda, Agenda.Des_Lib, Agenda.Lav_Cod, Agenda.Blocco_Flag, ")
                SQL_Generale.AppendLine(" Movimenti.Cau_Mov, Movimenti.Data_Movimento, Movimenti.Mov_Desc,  ")

                SQL_Generale.AppendLine(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, Imprese_Progetti.Progetto_Nome, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Fase_Cod, Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, ")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Id_Mov, Movimenti_dettagli.Id_Mov_det, Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Prezzo_Unitario_Netto, Movimenti_dettagli.pendente,")
                SQL_Generale.AppendLine(" Movimenti_dettagli.Extra_Str as Mov_Det_Extra_Str,")

                SQL_Generale.AppendLine(" Mov_Destinazioni.Sa_Cod,")

                'Select from G2G_Recode_Imprese: FROM_SaCod & TO_Sa_Cod, per Genagricola
                If takeFROM_SaCod Then
                    SQL_Generale.AppendLine("G2G_Recode_Imprese.FROM_SaCod, G2G_Recode_Imprese.TO_SaCod,")
                End If

                SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, ")
                SQL_Generale.AppendLine(" Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2, ")
                SQL_Generale.AppendLine(" Mov_Destinazioni.Tipo_Scorta, Mov_Destinazioni.Scorta_Min, ")
                SQL_Generale.AppendLine(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  ")
                SQL_Generale.AppendLine(" Fabbricati_Tipi.Tipo_Fabbricato_Des, ")
                SQL_Generale.AppendLine(" Centri_Aziendali.Sa_Nome ")

                SQL_Generale.AppendLine(" , Isnull(Imprese_Progetti.Progetto_Nome, '') as Lotto_Interno ")

                If isFreshAndFood Then
                    SQL_Generale.AppendLine(" , Cantina_Vasche.Identificativo, Cantina_Vasche.Insieme_Cod ")
                    SQL_Generale.AppendLine(" , Cantina_Insiemi.insieme_des ")

                    For Each paramQual In dtParamQual.Rows
                        If paramQual("Tipo") = 3 OrElse paramQual("Tipo") = 4 OrElse paramQual("Tipo") = 5 Then
                            SQL_Generale.AppendLine(" , COALESCE((Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), '') AS FF_" & paramQual("Tabella_Key") & "_Val_Cod  ")
                        End If
                        If paramQual("Tipo") = 1 Then
                            SQL_Generale.AppendLine(" , COALESCE((Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod), 0) AS FF_" & paramQual("Tabella_Key") & "_Tipo_Cod  ")

                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                SQL_Generale.AppendLine(" , COALESCE((Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tara_Campionatura), 0) AS FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura  ")
                                SQL_Generale.AppendLine(" , COALESCE((OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla), '') AS FF_" & paramQual("Tabella_Key") & "_Sigla  ")
                                SQL_Generale.AppendLine(" , COALESCE((OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione), '') AS FF_" & paramQual("Tabella_Key") & "_Descrizione ")
                                SQL_Generale.AppendLine(" , COALESCE((OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Codice_Generazione_Link), '') AS FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link ")
                                SQL_Generale.AppendLine(" , COALESCE((OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Mat_Cod_Generazione_Link), '') AS FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link ")
                            End If
                        End If
                    Next

                End If

                SQL_Generale.AppendLine(" , UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")

                SQL_Generale.AppendLine(" , Operazioni.Lav_Des ")

                'NO, rallenta troppo la query
                SQL_Generale.AppendLine(" , Agenda.Sa_Cod AS Sa_Cod_Operazione ")
                'SQL_Generale.AppendLine("  , ISNULL(( ")
                'SQL_Generale.AppendLine("           SELECT sa_nome ")
                'SQL_Generale.AppendLine("            FROM Centri_Aziendali ")
                'SQL_Generale.AppendLine("            WHERE Centri_Aziendali.Piva = Agenda.PIVA ")
                'SQL_Generale.AppendLine("            AND Centri_Aziendali.Sa_Cod = Agenda.SA_COD  ")
                'SQL_Generale.AppendLine("           ),'' ) AS Sa_Nome_OpColturale ")

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                    SQL_Generale.AppendLine(" ,COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ")
                    SQL_Generale.AppendLine(" ,CASE ")
                    SQL_Generale.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
                    SQL_Generale.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
                    SQL_Generale.AppendLine(" ELSE '' END AS Des_Gruppo_Merce ")
                End If

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i,
                                              isFreshAndFood, flagRecuperaCodArticolo)

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" FROM    Agenda ")

                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti ")
                SQL_Generale.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda")

                'JOIN OPERAZIONI - AGENDA
                SQL_Generale.AppendLine(" INNER JOIN Operazioni ON Operazioni.lav_cod = Agenda.lav_cod ")

                'JOIN IMPRESE - AGENDA
                SQL_Generale.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva ")

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.AppendLine(" INNER JOIN Movimenti_dettagli ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ")

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.AppendLine(" INNER JOIN Mov_Destinazioni ")
                SQL_Generale.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
                SQL_Generale.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

                'JOIN MOVIMENTI DESTINAZIONI - CENTRI AZIENDALI 
                SQL_Generale.AppendLine(" INNER JOIN Centri_Aziendali ")
                SQL_Generale.AppendLine(" ON Mov_Destinazioni.PIVA = Centri_Aziendali.Piva AND Mov_Destinazioni.Sa_Cod = Centri_Aziendali.Sa_Cod ")

                If Not isFreshAndFood Then
                    'JOIN FABBRICATO
                    SQL_Generale.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                    'JOIN TIPO FABBRICATO
                    SQL_Generale.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
                Else
                    ' Id_Destinazione potrebbe essere un magazzino o una cella

                    'JOIN FABBRICATO
                    SQL_Generale.AppendLine(" LEFT JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

                    'JOIN TIPO FABBRICATO
                    SQL_Generale.AppendLine(" LEFT JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

                    'JOIN Cantina Insiemi per trovare decodifica cella
                    SQL_Generale.AppendLine(" LEFT JOIN Cantina_Vasche ON Mov_Destinazioni.Piva = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.SA_COD AND ")
                    SQL_Generale.AppendLine(" Mov_Destinazioni.tipo_destinazione = Cantina_Vasche.tipo_destinazione AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.vas_cod ")

                    SQL_Generale.AppendLine(" LEFT JOIN Cantina_Insiemi ON Cantina_Vasche.Insieme_Cod = Cantina_Insiemi.Insieme_Cod ")

                End If

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.AppendLine(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod ")

                'JOIN UNITA DI MISURA
                SQL_Generale.AppendLine(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")

                'JOIN IMPRESE_PROGETTI
                SQL_Generale.AppendLine(" LEFT OUTER JOIN Imprese_Progetti  ON Imprese_Progetti.Piva = Movimenti_Dettagli.Piva And Imprese_Progetti.Progetto_Cod = Movimenti_dettagli.Cod_Progetto ")


                If isFreshAndFood Then
                    'JOIN MATERIE_PRIME_CAMPIONATURE
                    For Each paramQual In dtParamQual.Rows
                        SQL_Generale.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key") &
                                                " ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo " &
                                                " AND Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                        If paramQual("Tipo") = 1 Then
                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                SQL_Generale.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key") &
                                                        " ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod " &
                                                        " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & Agro_SQL_SaveText(paramQual("Tabella_ID")) & "'")
                            End If
                        End If
                    Next

                End If

                'JOIN G2G_Recode_Imprese, per Genagricola
                If takeFROM_SaCod Then
                    SQL_Generale.AppendLine("LEFT JOIN G2G_Recode_Imprese ON Mov_Destinazioni.Sa_Cod = G2G_Recode_Imprese.TO_SaCod")
                    SQL_Generale.AppendLine("AND Mov_Destinazioni.Piva = G2G_Recode_Imprese.To_Piva")
                End If

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                    SQL_Generale.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock) ")
                    SQL_Generale.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametriServer.PivaSuperUser) & "'")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Mat_Cod = Movimenti_dettagli.Mat_Cod")
                    SQL_Generale.AppendLine("       AND prodExtraPriv.Pro_Cod = Movimenti_dettagli.Pro_Cod")
                    SQL_Generale.AppendLine("             AND (prodExtraPriv.Piva = Movimenti_dettagli.Piva OR Movimenti_dettagli.Pro_Cod = 0)")
                    SQL_Generale.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
                    SQL_Generale.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
                    SQL_Generale.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
                    SQL_Generale.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = Movimenti_dettagli.Elem_Cod")

                End If

                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i,
                                            isFreshAndFood,
                                            flagRecuperaCodArticolo, flagJoinSuperUserCac,
                                            pivaSuperUser:=objParametriServer.PivaSuperUser)


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------
                '22/08/2018 MAGA:
                'da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento
                'l'ultimo giorno non veniva conteggiato
                'SQL_Generale.AppendLine(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
                SQL_Generale.AppendLine(" WHERE     Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " ")

                SQL_Generale.AppendLine(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
                SQL_Generale.AppendLine(" AND       Movimenti_Dettagli.Elem_Cod <> " & CALI_LAVORAZIONE & " ") 'escludere i cali di lavorazione
                SQL_Generale.AppendLine(" AND       Movimenti_Dettagli.Elem_Cod <> " & CORPI_ESTRANEI & " ") 'escludere i corpi estranei

                SQL_Generale.AppendLine(" And Movimenti.Cau_Mov In ('" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   ")

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Jolly_Int = " & MagazzinoMovimentato & "   ")
                SQL_Generale.AppendLine(" AND   Movimenti_Dettagli.Contabilizzato >= 0  ")
                '--------------

                If isFreshAndFood Then
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione IN (" & TIPO_DESTINAZIONE_MAGAZZINO & "," & CELLA_FRIGORIFERA & ")")
                Else
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Tipo_Destinazione = " & TIPO_DESTINAZIONE_MAGAZZINO & " ")
                End If

                If Piva <> "" Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                Else
                    If filtroCentri <> "" Then
                        SQL_Generale.AppendLine(" AND (Movimenti_Dettagli.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1)) & ") ) ")
                    End If
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
                End If

                'If Id_Agenda <> 0 Then
                '    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                'End If

                'If Id_Mov <> 0 Then
                '    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                'End If

                'If Id_Mov_Det <> 0 Then
                '    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
                'End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    If isFreshAndFood AndAlso cercaLottoPerLike Then
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(Lotto) & "%'   ")
                    Else
                        SQL_Generale.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If
                End If

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 AndAlso
                        objParametriServer.UtenteUsername <> objParametriServer.SuperUserUsername Then

                    'Calcolo gestione visibilità gruppi merce
                    Dim objGruppiUtenteMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametriServer, objParametriUtenti)
                    Dim dtGruppiUtenteMerce As DataTable = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & Piva & "')", "")

                    If dtGruppiUtenteMerce.Rows.Count > 0 Then
                        SQL_Generale.AppendLine(" AND COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) IN (")
                        SQL_Generale.AppendLine(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", Piva))
                        SQL_Generale.AppendLine(" )")
                    End If
                End If

                If xFiltroAggiuntivo <> "" Then
                    SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametriServer))
                End If

                SchedeMagazzinoGestioneWhere(SQL_Generale, Elem_Cod, i,
                                             xFiltroAggiuntivo_1,
                                             xFiltroAggiuntivo_2,
                                             xFiltroAggiuntivo_3,
                                             xFiltroAggiuntivo_4,
                                             xFiltroAggiuntivo_5,
                                             xFiltroAggiuntivo_6,
                                             xFiltroAggiuntivo_7,
                                             xFiltroAggiuntivo_8,
                                             xFiltroAggiuntivo_9,
                                             xFiltroAggiuntivo_10,
                                             xFiltroAggiuntivo_12,
                                             xFiltroAggiuntivo_13,
                                             xFiltroAggiuntivo_14,
                                             flagRecuperaCodArticolo,
                                             codArticolo,
                                             cercaCodArticoloPerLike,
                                             xFiltroAggiuntivo_15)

                SQL_Generale.AppendLine(" ) ")


            Next

            '------------------------------------------------------
            '------------------- ORDINAMENTO ----------------------
            '------------------------------------------------------

            If xOrderBy <> "" Then
                SQL_Generale.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
            Else
                SQL_Generale.AppendLine(" ORDER BY Movimenti.Data_Movimento ")
            End If

            'Giulia 03/12/2025: questa va messa come ultima istruzione della query principale dei movimenti perchè altrimenti da errore di sintassi
            If LivelloCompatibilita(objParametriServer) >= 150 Then
                SQL_Generale.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()

                SQL_Generale.AppendLine("")
                SQL_Generale.Append(strSql.ToString())

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, SQL_Generale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###############################################################################
    Public Function SchedaGiacenzeMagazzinoSementiConBolle(ByVal Data_Giacenza As Date, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Id_Destinazione As Integer, _
                                            ByVal Elem_Cod As Integer, _
                                            ByVal Pro_Cod As Integer, _
                                            ByVal Mat_Cod As Integer, _
                                            ByVal Cal_Cod As Integer, _
                                            ByVal Cod_Progetto As Integer, _
                                            ByVal Fase_Cod As Integer, _
                                            ByVal Udm_Cod As Integer, _
                                            ByVal Lotto As String, _
                                            ByVal Flag_QtaNoZero As Boolean, _
                                            ByVal ids_agenda As String(), _
                                            ByVal ids_movimenti As String(), _
                                            ByVal ids_movimenti_dett As String(), _
                                            ByVal xFiltroAggiuntivo_7 As String, _
                                            ByRef objParametri As AgronicaCoreParametri _
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Magazzino.SchedaGiacenzeMagazzino()"

        If ids_agenda.Length = 0 OrElse ids_agenda.Length <> ids_movimenti.Length OrElse ids_agenda.Length <> ids_movimenti_dett.Length Then
            Throw New Exception("[" & NomeRoutine & "] : non sono lo stesso numero")
        End If


        Dim MessaggioErrore As String = ""
        Dim SQL_Generale As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i, i_tot As Integer

        Try

            Select Case Elem_Cod
                Case 0 'tutte le categorie
                    i_tot = 11
                Case TRASFORMATI_VEGETALI
                    i_tot = 2 '2 cicli per gestire la union tra bottiglie e trasf raccolti
                Case FERTILIZZANTI
                    i_tot = 2 '2 cicli per gestire la union tra fert banca dati e fert azi
                Case Else
                    i_tot = 1
            End Select

            'per ogni sezione della union
            For i = 1 To i_tot

                If i <> 1 Then
                    SQL_Generale.Append(" UNION ALL ")
                End If

                '------------------------------------------------------ 
                '------------------- SELECT ---------------------------
                '------------------------------------------------------

                SQL_Generale.Append(" ( ")

                SQL_Generale.Append(" SELECT Imprese.Piva, Imprese.Rag_Soc AS Impresa, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.Append(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.Append(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, " & vbCrLf)
                SQL_Generale.Append(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)

                SchedeMagazzinoGestioneSelect(SQL_Generale, Elem_Cod, i)

                SQL_Generale.Append(" , SUM( CASE WHEN Movimenti.CAU_MOV='" & CStr(CAU_SCARICO) & "' ")
                SQL_Generale.Append("           THEN -(Mov_Destinazioni.qta)")
                SQL_Generale.Append("           ELSE Mov_Destinazioni.qta")
                SQL_Generale.Append("           END) AS Giacenza ")

                '------------------------------------------------------ 
                '-------------------- FROM ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" FROM    Agenda " & vbCrLf)

                '------------------------------------------------------ 
                '-------------------- JOIN ----------------------------
                '------------------------------------------------------

                'JOIN AGENDA - MOVIMENTI
                SQL_Generale.Append(" INNER JOIN Movimenti " & vbCrLf)
                SQL_Generale.Append(" ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda" & vbCrLf)

                'JOIN IMPRESE - AGENDA
                SQL_Generale.Append(" INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva " & vbCrLf)

                'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
                SQL_Generale.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)

                'JOIN MOVIMENTI DETTAGLI - MOVIMENTI DESTINAZIONI
                SQL_Generale.Append(" INNER JOIN Mov_Destinazioni " & vbCrLf)
                SQL_Generale.Append(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
                SQL_Generale.Append(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

                'JOIN FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND " & vbCrLf)
                SQL_Generale.Append(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod " & vbCrLf)

                'JOIN TIPO FABBRICATO
                SQL_Generale.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)

                'JOIN CATEGORIE MAGAZZINO
                SQL_Generale.Append(" INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod " & vbCrLf)

                'JOIN UNITA DI MISURA
                SQL_Generale.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)

                SchedeMagazzinoGestioneJoin(SQL_Generale, Elem_Cod, i)


                '------------------------------------------------------ 
                '-------------------- WHERE ---------------------------
                '------------------------------------------------------
                '22/08/2018 MAGA:
                'da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento
                'l'ultimo giorno non veniva conteggiato
                Data_Giacenza = DateAdd(DateInterval.Day, 1, CDate(Data_Giacenza))
                'SQL_Generale.Append(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
                SQL_Generale.Append(" WHERE     Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)

                SQL_Generale.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CALI_LAVORAZIONE) & " " & vbCrLf) 'escludere i cali di lavorazione
                SQL_Generale.Append(" AND       Movimenti_Dettagli.Elem_Cod <> " & CStr(CORPI_ESTRANEI) & " " & vbCrLf) 'escludere i corpiestranei

                SQL_Generale.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)

                '--------------
                'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
                SQL_Generale.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
                '--------------

                SQL_Generale.Append(" AND       Mov_Destinazioni.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "" & vbCrLf)

                If Piva <> "" Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
                End If

                If Sa_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
                End If

                If Id_Destinazione <> 0 Then
                    SQL_Generale.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   " & vbCrLf)
                End If

                'non togliere questa condizione!
                If Elem_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   " & vbCrLf)
                End If

                If Pro_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   " & vbCrLf)
                End If

                If Mat_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
                End If

                If Cal_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   " & vbCrLf)
                End If

                If Cod_Progetto <> 0 AndAlso Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   " & vbCrLf)
                End If

                If Fase_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   " & vbCrLf)
                End If

                If Udm_Cod <> 0 Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " & vbCrLf)
                End If

                If Lotto <> LOTTO_NONDEFINITO Then
                    SQL_Generale.Append(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   " & vbCrLf)
                End If


                SQL_Generale.Append(SchedeMagazzino_Where_7("", False) & vbCrLf)
                SQL_Generale.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo_7) & vbCrLf)

                '------------------------------------------------------ 
                '----------------- GROUP BY ---------------------------
                '-----------------------------------------------------

                SQL_Generale.Append(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, CategorieMagazzino.Tabella, CategorieMagazzino.NomeComune, " & vbCrLf)
                SQL_Generale.Append(" Movimenti_dettagli.Elem_Cod,  Movimenti_dettagli.Pro_Cod,  Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, ")
                SQL_Generale.Append(" Movimenti_dettagli.Fase_Cod,  Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod,  Movimenti_dettagli.Udm_Cod, ")
                SQL_Generale.Append(" Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, " & vbCrLf)
                SQL_Generale.Append(" Fabbricati.Fabbricato_Des, Fabbricati_Tipi.Tipo_Fabbricato_Cod,  " & vbCrLf)
                SQL_Generale.Append(" Fabbricati_Tipi.Tipo_Fabbricato_Des, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim " & vbCrLf)

                SchedeMagazzinoGestioneGroupBy(SQL_Generale, Elem_Cod, i)

                '------------------------------------------------------ 
                '-------------------- HAVING --------------------------
                '------------------------------------------------------

                If Flag_QtaNoZero Then
                    'condizione per filtrare le giacenze=0
                    SQL_Generale.Append("  HAVING SUM(CASE WHEN Movimenti.Cau_Mov= '" & CAU_SCARICO & "' THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END) <> 0 " & vbCrLf)
                End If

                SQL_Generale.Append(" ) ")

            Next


            'aggiungo join per bolle
            Dim SQL_2 As New System.Text.StringBuilder
            Dim k As Integer = 0
            SQL_2.Append(" select *, md.id_agenda as id_agenda_ddt_det, md.id_mov as id_mov_ddt_det, md.id_Mov_Det as id_Mov_Det_ddt_det, md.qta as qta_ddt_det , md.Mov_Det_Des as Mov_Det_Des_ddt_det , mov4000.Doc_Numero as Doc_Numero_ddt ")
            SQL_2.Append(" from  (" & SQL_Generale.ToString & ") t1 ")
            SQL_2.Append(" inner join  ")
            SQL_2.Append("       [Movimenti_dettagli] md  ")
            SQL_2.Append(" on t1.Elem_Cod = md.Elem_Cod  ")
            SQL_2.Append("   and t1.Pro_Cod =  md.Pro_Cod ")
            SQL_2.Append("  and t1.Mat_Cod =   md.Mat_Cod")
            SQL_2.Append("  and t1.UDM_Cod =   md.UDM_Cod")
            SQL_2.Append("  and t1.Lotto =   md.Lotto")
            'metto in join con il moovimento 4000 del ddt che contiene le info sul ddt
            'con i movimenti 7300 del ddt che contengono i dettagli sui prodotti
            SQL_2.Append(" inner join  ")
            SQL_2.Append("       [Movimenti] mov4000  ")
            SQL_2.Append(" on mov4000.id_agenda = md.id_agenda  ")
            SQL_2.Append("  and mov4000.Cau_Mov =   '4000'")
            SQL_2.Append(" where ")
            For k = 0 To ids_agenda.Length - 1

                If k = 0 Then
                    SQL_2.Append(" (   ")
                    SQL_2.Append("   md.id_agenda = " & ids_agenda(k) & " and ")
                    SQL_2.Append("   md.id_mov  =  " & ids_movimenti(k) & "  and ")
                    SQL_2.Append("   md.id_Mov_Det =  " & ids_movimenti_dett(k) & " ")
                    SQL_2.Append(" )   ")
                Else
                    SQL_2.Append(" or(   ")
                    SQL_2.Append("   md.id_agenda = " & ids_agenda(k) & " and ")
                    SQL_2.Append("   md.id_mov =  " & ids_movimenti(k) & "  and ")
                    SQL_2.Append("   md.id_Mov_Det =  " & ids_movimenti_dett(k) & " ")
                    SQL_2.Append(" )   ")
                End If
            Next


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, SQL_2.ToString, NomeRoutine)
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

