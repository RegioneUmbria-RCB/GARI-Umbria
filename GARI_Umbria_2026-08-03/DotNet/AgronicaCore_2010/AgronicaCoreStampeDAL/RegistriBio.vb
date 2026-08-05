Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ModelloTerzistiBio
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiMovimentiTerzisti(ByVal Piva As String,
                                           ByVal Centro As String,
                                           ByVal Mat_Cod As String,
                                           ByVal DataInizio As Date,
                                           ByVal DataFine As Date,
                                           ByVal Fornitore As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.ModelloTerzistiBio.LeggiMovimentiTerzisti"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine("select DISTINCT Case ")
            StbSQL.AppendLine("when isnull(operazioni.lav_des , '') = '' then a.[desc] ")
            StbSQL.AppendLine("when operazioni.lav_des = 'Altre Lavorazioni' then (operazioni.lav_des + ' - ' + a.[desc]) ")
            StbSQL.AppendLine("Else operazioni.LAV_DES ")
            StbSQL.AppendLine("End ")

            StbSQL.AppendLine("AS attivita, i.Progetto_Nome,  ")

            StbSQL.AppendLine("CD.Data_Creazione,  ")

            StbSQL.AppendLine("reg.val_cod As Progetto, ")

            StbSQL.AppendLine("dw.Progetto_Des,  ")

            StbSQL.AppendLine("Case WHEN ISNULL (agenda.Id_Agenda, 0) = 0 Or agenda_4500.Split = 1 THEN ")
            StbSQL.AppendLine("Case WHEN isnull(i_p_4500.sup_prog, 0) = 0 Then ")
            StbSQL.AppendLine("isnull(r_i_4500.sup_imp, 0) Else i_p_4500.sup_prog End ")
            StbSQL.AppendLine("Else ")
            StbSQL.AppendLine("Case WHEN isnull(m_d.qta2, 0) = 0 THEN ")
            StbSQL.AppendLine("Case WHEN isnull(i_p_4500.sup_prog, 0) = 0 Then ")
            StbSQL.AppendLine("isnull(r_i_4500.sup_imp, 0) Else i_p_4500.sup_prog End ")
            StbSQL.AppendLine("Else m_d.qta2 End ")
            StbSQL.AppendLine("End  As Superficie_Lavorata, ")

            StbSQL.AppendLine(" case ")
            StbSQL.AppendLine(" when isnull(sv.veg_des,'') != '' then (sv.veg_des + ' - ' + cul.cul_des) ")
            StbSQL.AppendLine(" when isnull(sv.veg_des,'') = '' then isnull(( select top 1 codici_anagrafe.descrizione  ")
            StbSQL.AppendLine(" from reg_impianti_codici  ")
            StbSQL.AppendLine(" inner join codici_anagrafe on reg.id_cod = codici_anagrafe.codice ")
            StbSQL.AppendLine(" where(reg.piva = regi.piva) ")
            StbSQL.AppendLine(" and   (reg.sa_cod = regi.sa_cod)  ")
            StbSQL.AppendLine(" and   (reg.appezza  = regi.appezza) ")
            StbSQL.AppendLine(" and   (reg.id_reg = regi.id_reg) ")
            StbSQL.AppendLine(" and   (reg.progetto_cod = 0) ")
            StbSQL.AppendLine(" and   (reg.id_cod = cd.id_cod_reg_impianti_codici)  ")
            StbSQL.AppendLine(" and (codici_anagrafe.gruppo = 'terreno')  ")
            StbSQL.AppendLine(" ) , 'terreno nudo') ")
            StbSQL.AppendLine(" end as coltura, ")

            StbSQL.AppendLine(" c.Rag_Soc As Fornitore,  ")

            StbSQL.AppendLine("regi.Sup_Imp AS Superficie, ")

            StbSQL.AppendLine("app.val_cod AS Campo, ")

            StbSQL.AppendLine("ap.APP_NOME As 'Descrizione Campo'  ")
            'StbSQL.AppendLine("ct.id_agenda, ct.Id_Attivita, ct.lotto, ct.sa_cod, ct.id_destinazione, cd.appezza, cd.id_reg, cd.Progetto_Cod  ")
            StbSQL.AppendLine("from cdg_testata ct  ")

            StbSQL.AppendLine("join CDG_Dettagli cd on cd.Id_CDG = ct.Id_CDG  ")

            StbSQL.AppendLine("left join Imprese_Progetti i  ")

            StbSQL.AppendLine("on i.Progetto_Cod = cd.Progetto_Cod and cd.Piva = i.Piva ")

            StbSQL.AppendLine("join attivita a on ct.id_attivita = a.id_attivita  ")

            StbSQL.AppendLine("left join mov_dettagli_riferimenti on id_agenda_rif = ct.id_agenda  ")

            StbSQL.AppendLine("left join agenda on mov_dettagli_riferimenti.id_agenda  = agenda.id_agenda  ")

            StbSQL.AppendLine("left join operazioni on operazioni.LAV_COD = agenda.lav_cod  ")

            StbSQL.AppendLine("left join Appezzamento ap  ")
            StbSQL.AppendLine("on ap.APPEZZA = cd.Appezza and ap.PIVA = cd.Piva and ap.SA_COD = cd.Sa_Cod  ")

            StbSQL.AppendLine("left join Appezzamento_Codici app  ")
            StbSQL.AppendLine("on cd.Appezza = app.appezza and cd.piva = app.piva and cd.sa_cod = app.sa_cod and app.id_cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento & " ")

            StbSQL.AppendLine("left outer join Reg_Impianti_Codici reg  ")
            StbSQL.AppendLine("on reg.piva = cd.piva and reg.Id_Reg = cd.Id_Reg and reg.sa_cod = cd.Sa_Cod and reg.appezza = cd.Appezza and reg.id_cod = " & enum_CodiciAnagrafe.Codice_Impianto & " ")

            StbSQL.AppendLine("left outer join Reg_Impianti regi  ")
            StbSQL.AppendLine("on regi.APPEZZA = cd.Appezza and regi.piva = cd.piva and regi.sa_cod = cd.Sa_Cod and cd.Id_Reg = regi.Id_Reg  ")

            StbSQL.AppendLine("left join DW_CDG_Costi_Ricavi dw  ")
            StbSQL.AppendLine("on dw.Id_CDG_Dettagli = cd.Id_CDG_Dettagli  ")

            StbSQL.AppendLine("Left JOIN Cultivar cul ON regi.CUL_COD = cul.Cul_Cod ")

            StbSQL.AppendLine("Left JOIN SpecieVegetali sv ON sv.Veg_Cod = cul.Veg_Cod ")

            StbSQL.AppendLine(" Join movimenti_dettagli mvd on ct.lotto = mvd.lotto ")

            StbSQL.AppendLine(" Join movimenti mv on mv.id_mov = mvd.id_mov ")

            StbSQL.AppendLine(" Join Risorse_Umane ru on mv.Cod_RisUm = ru.Cod_RisUm ")

            StbSQL.AppendLine(" Join contatti c on c.Cod_Contatto = ru.Cod_Contatto ")

            StbSQL.AppendLine("Join agenda as agenda_4500 ")
            StbSQL.AppendLine("On ct.id_agenda = agenda_4500.id_agenda ")

            StbSQL.AppendLine("Left Join movimenti on movimenti.id_agenda  = agenda.id_agenda And movimenti.Cau_Mov = '" & CAU_LAVORAZIONE & "' ")

            StbSQL.AppendLine("Left Join mov_destinazioni m_d ")
            StbSQL.AppendLine("On  m_d.piva = cd.Piva And ")
            StbSQL.AppendLine("m_d.Sa_Cod = cd.Sa_Cod And ")
            StbSQL.AppendLine("m_d.Appezza = cd.Appezza And ")
            StbSQL.AppendLine("m_d.Id_Destinazione = cd.Id_Reg ")
            StbSQL.AppendLine("And m_d.id_agenda = agenda.id_agenda ")

            StbSQL.AppendLine("Join reg_impianti r_i_4500 on ")
            StbSQL.AppendLine("r_i_4500.piva = cd.Piva And ")
            StbSQL.AppendLine("r_i_4500.Sa_Cod = cd.Sa_Cod And ")
            StbSQL.AppendLine("r_i_4500.Appezza = cd.Appezza And ")
            StbSQL.AppendLine("r_i_4500.Id_Reg = cd.Id_Reg ")

            StbSQL.AppendLine("Join Imprese_Progetti i_p_4500 on ")
            StbSQL.AppendLine("i_p_4500.piva = cd.Piva And ")
            StbSQL.AppendLine("i_p_4500.Sa_Cod = cd.Sa_Cod And ")
            StbSQL.AppendLine("i_p_4500.Appezza = cd.Appezza And ")
            StbSQL.AppendLine("i_p_4500.Id_Reg = cd.Id_Reg And ")
            StbSQL.AppendLine("i_p_4500.Progetto_Cod = cd.Progetto_Cod And ")

            StbSQL.AppendLine("i_p_4500.validita_inizio <= isnull(movimenti.data_movimento, ct.data_inserimento) And ")
            StbSQL.AppendLine("i_p_4500.validita_fine >= isnull(movimenti.data_movimento, ct.data_inserimento) ")


            StbSQL.AppendLine("where dw.Budget_Cons = 0 And ct.Budget = 0 And ct.elem_cod= " & CAT_MAG_SERVIZI_PROFESSIONALI & " And cd.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If (Mat_Cod <> "") Then
                StbSQL.AppendLine(" And ct.mat_Cod In (" & Agro_SQL_Save_Clausola_IN(Mat_Cod) & ") ")
            End If

            If Centro <> 0 Then
                StbSQL.AppendLine("and cd.sa_cod = " & CInt(Centro))
            End If

            StbSQL.AppendLine("and ct.lotto in (  ")
            StbSQL.AppendLine("select lotto From movimenti_dettagli join movimenti mv  ")
            StbSQL.AppendLine("on mv.id_mov = movimenti_dettagli.id_mov   ")
            StbSQL.AppendLine("join risorse_umane ru on ru.cod_risum = mv.cod_risum   ")
            StbSQL.AppendLine("join contatti c on c.cod_contatto = ru.cod_contatto   ")
            StbSQL.AppendLine("where mv.Data_Movimento between " & Agro_SQL_SaveDate(DataInizio) & " and " & Agro_SQL_SaveDate(DataFine) & " ")
            StbSQL.AppendLine(" and cau_mov = '" & CAU_CARICO & "'")
            If (Mat_Cod <> "") Then
                StbSQL.AppendLine("and Mat_Cod in (" & Agro_SQL_Save_Clausola_IN(Mat_Cod) & ") ")
            End If
            If (Fornitore <> "") Then
                StbSQL.AppendLine("and ru.Cod_Risum in ( " & Agro_SQL_Save_Clausola_IN(Fornitore) & " ) ")
            End If
            StbSQL.AppendLine(") ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
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

End Class

Public Class SchedaMateriePrimeBio
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge i movimenti di carico:
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiMovimentiCarico(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Fabbricato_Cod As Integer,
                                         ByVal Data_Inizio As Date,
                                         ByVal Data_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.SchedaMateriePrimeBio.LeggiMovimentiCarico"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------

            StbSQL.Append(" ( " & vbCrLf)

            StbSQL.Append(" -- PARTE DI MAGAZZINO " & vbCrLf)
            StbSQL.Append(" ( " & vbCrLf)

            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append("         Movimenti.Data_Movimento,  Movimenti.Cau_Mov,   " & vbCrLf)
            StbSQL.Append("         ISNULL(Mov_Destinazioni.Sa_Cod,0) AS Sa_Cod, ISNULL(Movimenti_dettagli.Elem_Cod,0) AS Elem_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(Movimenti_dettagli.Mat_Cod,0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod,0) AS Pro_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Formulati.Fr_Des,'') AS Fr_Des, ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, " & vbCrLf)
            StbSQL.Append("         ISNULL(Materie_Prime.Cod_Articolo,'') AS Etichetta, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto, " & vbCrLf)
            StbSQL.Append("         ISNULL(Movimenti_dettagli.Qta,'') AS Qta, ISNULL(Movimenti_dettagli.Udm_Cod,0) AS Udm_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(Mov_Registrazione.Extra_Str, '') AS Extra_Str, " & vbCrLf)
            StbSQL.Append("         ISNULL(UnitaMisura.Udm_Sim,'') AS Udm_Sim, " & vbCrLf)

            'Colonne del documento contabile
            StbSQL.Append("         ISNULL(" & vbCrLf)

            StbSQL.Append("                  (SELECT TOP 1 Dati_Documento  FROM " & vbCrLf)

            StbSQL.Append("                 (" & vbCrLf) 'parentesi della UNION

            StbSQL.Append("                 (" & vbCrLf) 'parentesi della prima query
            StbSQL.Append("                 -- LEGGE L'INDIRIZZO DEL CONTATTO " & vbCrLf)
            StbSQL.Append("                 SELECT Mov_Cont.Doc_Numero_Sin + LTRIM(STR(Mov_Cont.Doc_Numero, 10, 0)) + Mov_Cont.Doc_Numero_Des " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.cognome " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Contatti.Cod_Contatto,'') " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Risorse_Umane.Attivita_Des,'') " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Indirizzi.ind_des,'') + ' ' + ISNULL(Indirizzi.frz_des,'') + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')'  " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Contatti.Codice_Fiscale,'') " & vbCrLf)
            StbSQL.Append("                         AS Dati_Documento " & vbCrLf)

            StbSQL.Append("                 FROM Movimenti Mov_Cont   " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Risorse_Umane ON Mov_Cont.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            'StbSQL.Append("                 INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN ContattiXIndirizzi ON Contatti.Cod_Contatto =ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Cod_Indirizzo = Mov_Cont.Cod_IndirizzoRisUm  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            StbSQL.Append("                 INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM " & vbCrLf)

            StbSQL.Append("                 WHERE Agenda.PIVA = Mov_Cont.PIVA  AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL.Append("                 AND    Mov_Cont.Cau_Mov = '" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            'StbSQL.Append("                 AND    Rapporti_Contabili.Piva = '" & CStr(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StbSQL.Append("                 )" & vbCrLf) 'parentesi della prima query

            StbSQL.Append("         UNION ALL " & vbCrLf)

            StbSQL.Append("                 (" & vbCrLf) 'parentesi della seconda query
            StbSQL.Append("                 -- LEGGE L'INDIRIZZO DELL'IMPRESA GIAS " & vbCrLf)
            StbSQL.Append("                 SELECT Mov_Cont.Doc_Numero_Sin + LTRIM(STR(Mov_Cont.Doc_Numero, 10, 0)) + Mov_Cont.Doc_Numero_Des " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         Contatti.Rag_Soc " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Contatti.Cod_Contatto,'') " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Risorse_Umane.Attivita_Des,'') " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Indirizzi.ind_des,'') + ' ' + ISNULL(Indirizzi.frz_des,'') + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')'  " & vbCrLf)
            StbSQL.Append("                        + '|' + " & vbCrLf)
            StbSQL.Append("                         ISNULL(Contatti.Codice_Fiscale,'') " & vbCrLf)
            StbSQL.Append("                         AS Dati_Documento " & vbCrLf)

            StbSQL.Append("                 FROM Movimenti Mov_Cont   " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Risorse_Umane ON Mov_Cont.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
            'StbSQL.Append("                 INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN ImpreseXIndirizzi ON Contatti.Cod_Contatto =ImpreseXIndirizzi.Piva AND ImpreseXIndirizzi.Cod_Indirizzo = Mov_Cont.Cod_IndirizzoRisUm  " & vbCrLf)
            StbSQL.Append("                 INNER JOIN Indirizzi ON ImpreseXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            StbSQL.Append("                 INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM " & vbCrLf)

            StbSQL.Append("                 WHERE Agenda.PIVA = Mov_Cont.PIVA  AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
            StbSQL.Append("                 AND    Mov_Cont.Cau_Mov = '" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            'StbSQL.Append("                 AND    Rapporti_Contabili.Piva = '" & CStr(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StbSQL.Append("                 )" & vbCrLf) 'parentesi della seconda query

            StbSQL.Append("                 )" & vbCrLf) 'parentesi della union

            StbSQL.Append("                 AS Dati_Documento ) " & vbCrLf) 'parentesi del TOP 1

            StbSQL.Append("          , '' ) AS Dati_Documento " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Agenda  " & vbCrLf)

            'no AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod
            StbSQL.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti_Dettagli ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)

            StbSQL.Append(" LEFT JOIN Movimenti AS Mov_Registrazione ON Mov_Registrazione.Piva = Agenda.PIVA AND Mov_Registrazione.Id_Agenda = Agenda.Id_Agenda AND Mov_Registrazione.Cau_Mov = '" & CAU_REGISTRAZIONI & "'  " & vbCrLf)

            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Formulati ON Formulati.Fr_Cod = Movimenti_Dettagli.Pro_Cod   " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Fertilizzanti ON Fertilizzanti.Fer_Cod = Movimenti_dettagli.Pro_Cod  " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------

            Dim lavCodDaCercare As New List(Of String) From {LAVCOD_BOLLA_RICEVUTA, LAVCOD_CARICO, LAVCOD_FATTURA_RICEVUTA, LAVCOD_DOCO_RICEVUTO,
                                               LAVCOD_MVV_RICEVUTO, LAVCOD_ACQUISTO_BENI, LAVCOD_ACQUISTO, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}

            If Fabbricato_Cod <> 0 Then
                'Leggo i trasferimenti solo quando seleziono uno specifico magazzino, altrimenti mostrerei contemporaneamente sia gli scarichi che i carichi
                'generati dai trasferimenti
                lavCodDaCercare.Add(LAVCOD_TRASFERIMENTO)
            End If

            StbSQL.Append(" WHERE ( Agenda.Lav_Cod IN ( " & Agro_SQL_Save_Clausola_IN(String.Join(", ", lavCodDaCercare)) & ")" & vbCrLf)
            'Leggo i seguenti lav_cod solo quando rappresentano un reso del prodotto
            StbSQL.Append("         OR ( Agenda.LAV_COD IN( " & String.Join(", ", {LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA}.ToArray) & ")" & vbCrLf)
            StbSQL.Append("              AND (Mov_Registrazione.Causale_Trasporto_Cod = 15 OR UPPER(Mov_Registrazione.Causale_Trasporto) = 'MERCE RESA')" & vbCrLf)
            StbSQL.Append("            )" & vbCrLf)
            'StbSQL.Append("         OR ( Agenda.LAV_COD = " & LAVCOD_TRASFERIMENTO & " AND Movimenti.Cau_Mov = '" & CAU_CARICO & "')" & vbCrLf)
            StbSQL.Append("       )" & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            '22/08/2018 MAGA:
            'da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento
            'l'ultimo giorno non veniva conteggiato
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))
            'StbSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)

            StbSQL.Append(" AND    Movimenti.Cau_Mov IN ( '" &
                                    Agro_SQL_SaveText(CAU_CARICO) & "', '" &
                                    Agro_SQL_SaveText(CAU_SCARICO) & "', '" &
                                    Agro_SQL_SaveText(CAU_ABBUONI) & "', '" &
                                    Agro_SQL_SaveText(CAU_CONFERIMENTO) & "', '" &
                                    Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "', '" &
                                    Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "', '" &
                                    Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) &
                                    "'  ) " & vbCrLf)

            'SOLO FATTURE IMMEDIATE, NON DIFFERITE (CHE HANNO IL JOLLY_INT=1)
            StbSQL.Append(" AND     Movimenti_dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.Append(" ) -- FINE MAGAZZINO " & vbCrLf)

            'StbSQL.Append(" UNION ALL " & vbCrLf)

            'StbSQL.Append(" -- PARTE DI VASCA " & vbCrLf)
            'StbSQL.Append(" ( " & vbCrLf)
            'StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  " & vbCrLf)
            'StbSQL.Append(" Movimenti.Data_Movimento,  Movimenti.Cau_Mov,  " & vbCrLf)
            'StbSQL.Append(" ISNULL(Mov_Destinazioni.Sa_Cod,0) AS Sa_Cod, ISNULL(Movimenti_dettagli.Elem_Cod,0) AS Elem_Cod,  " & vbCrLf)
            'StbSQL.Append(" ISNULL(Movimenti_dettagli.Mat_Cod,0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod,0) AS Pro_Cod,  " & vbCrLf)
            'StbSQL.Append(" Materie_Prime.Mat_Des, '' AS Fr_Des, '' AS Fer_Des, " & vbCrLf)
            'StbSQL.Append(" '' AS Etichetta, ISNULL(Movimenti_dettagli.Lotto,'') AS Lotto,  " & vbCrLf)
            'StbSQL.Append(" ISNULL(Movimenti_dettagli.Qta,'') AS Qta, ISNULL(Movimenti_dettagli.Udm_Cod,0) AS Udm_Cod, " & vbCrLf)
            'StbSQL.Append(" ISNULL(UnitaMisura.Udm_Sim,'') AS Udm_Sim " & vbCrLf)
            'StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" FROM agenda  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN movimenti on agenda.piva= movimenti.PIVA AND agenda.id_agenda= movimenti.id_agenda  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN movimenti_dettagli on movimenti_dettagli.piva= movimenti.PIVA AND movimenti_dettagli.id_agenda= movimenti.id_agenda  AND movimenti_dettagli.id_mov= movimenti.id_mov " & vbCrLf)
            'StbSQL.Append(" INNER JOIN mov_destinazioni on movimenti_dettagli.piva= mov_destinazioni.PIVA AND movimenti_dettagli.id_agenda= mov_destinazioni.id_agenda  AND movimenti_dettagli.id_mov= mov_destinazioni.id_mov AND movimenti_dettagli.id_mov_det= mov_destinazioni.id_mov_det " & vbCrLf)
            'StbSQL.Append(" INNER JOIN Linee_Preparazioni on agenda.piva= Linee_Preparazioni.PIVA AND Linee_Preparazioni.preparazione_cod= agenda.preparazione_cod  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)

            ''------------------------------------------------------ 
            ''-------------------- WHERE ---------------------------
            ''------------------------------------------------------
            'StbSQL.Append(" WHERE Agenda.Piva = '" + Agro_SQL_SaveText(Piva) + "' " & vbCrLf)
            'StbSQL.Append(" AND Modulo_Generazione = " & CStr(enum_Omni_Modulo_Generazione.Cantine))
            'StbSQL.Append(" AND Codice_Generazione = " & CStr(enum_Omni_Preparazione_Cod.RilevamentoProdottiSfusiVasca))
            'StbSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            'StbSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)

            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            ''StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" ) -- FINE VASCA " & vbCrLf)

            StbSQL.Append(" )  " & vbCrLf)

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                'doc_numero è 0 perché è quello del movimento di magazzino
                'StbSQL.Append(" ORDER BY Movimenti.Data_Movimento, Movimenti.Doc_Numero, Etichetta ")
                StbSQL.Append(" ORDER BY Movimenti.Data_Movimento, des_lib, Mat_Des")
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


End Class



Public Class SchedaVenditeBio
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le vendite    
    ''' xFiltroAggiuntivo richiede l'AND
    ''' modificata in data 10/04/2017: fa il sum della qta su groupby 
    ''' (così per i trasformati che hanno stesso mat_cod somma le qta che prima erano divise per cal_cod)
    ''' il calibro non veniva stampato neanche prima
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiVendite(ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Fabbricato_Cod As Integer,
                                 ByVal Mat_Cod As Integer,
                                 ByVal Cod_RisUm As Integer,
                                 ByVal Data_Inizio As Date,
                                 ByVal Data_Fine As Date,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.SchedaVenditeBio.LeggiVendite"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            ' se si aggiungono campi metterli anche nel group by
            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append("         Movimenti.Data_Movimento,  Movimenti.Cau_Mov, " & vbCrLf)
            StbSQL.Append("         ISNULL(Mov_Destinazioni.Sa_Cod,0) AS Sa_Cod, ISNULL(Movimenti_dettagli.Elem_Cod,0) AS Elem_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(Movimenti_dettagli.Mat_Cod,0) AS Mat_Cod, ISNULL(Movimenti_dettagli.Pro_Cod,0) AS Pro_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, Movimenti_dettagli.Lotto, Movimenti_dettagli.pendente, " & vbCrLf)
            StbSQL.Append("         ISNULL(Materie_Prime.Cod_Articolo,'') AS Etichetta, " & vbCrLf)
            StbSQL.Append("         ISNULL(Movimenti_dettagli.Udm_Cod,0) AS Udm_Cod, " & vbCrLf)
            StbSQL.Append("         ISNULL(UnitaMisura.Udm_Sim,'') AS Udm_Sim, " & vbCrLf)
            '10/04/2017
            'StbSQL.Append("         ISNULL(Movimenti_dettagli.Qta,'') AS Qta, " & vbCrLf)
            StbSQL.Append("         SUM(Movimenti_dettagli.Qta) AS Qta, ")

            '---------------------------------------------------------------------
            ' CASO SENZA FILTRO DEL COD_RISUM
            '---------------------------------------------------------------------
            If Cod_RisUm = 0 Then

                'Colonne del documento contabile
                StbSQL.Append("         ISNULL(" & vbCrLf)

                StbSQL.Append("                  (SELECT TOP 1 Dati_Documento  FROM " & vbCrLf)

                StbSQL.Append("                 (" & vbCrLf) 'parentesi della UNION

                StbSQL.Append("                 (" & vbCrLf) 'parentesi della prima query: legge ContattiXIndirizzi
                StbSQL.Append("                 -- LEGGE L'INDIRIZZO DEL CONTATTO " & vbCrLf)
                StbSQL.Append("                 SELECT Mov_Cont.Doc_Numero_Sin + LTRIM(STR(Mov_Cont.Doc_Numero, 10, 0)) + Mov_Cont.Doc_Numero_Des " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.cognome " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Contatti.Cod_Contatto,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Risorse_Umane.Attivita_des,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Indirizzi.ind_des,'') + ' ' + ISNULL(Indirizzi.frz_des,'') + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')'  " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Contatti.Codice_Fiscale,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Mov_Cont.causale_trasporto,'') " & vbCrLf)
                StbSQL.AppendLine(" + '|' + ")
                StbSQL.AppendLine("ISNULL(Contatti_Cess_Diverso.Rag_Soc, '') +ISNULL(Contatti_Cess_Diverso.Nome,'') + ' ' + ISNULL(Contatti_Cess_Diverso.cognome,'') ")
                StbSQL.AppendLine(" + '|' + ")
                StbSQL.AppendLine("ISNULL(Contatti_Cess_Diverso.Cod_Contatto,'') ")
                StbSQL.AppendLine(" + '|' + ")
                StbSQL.AppendLine(" ISNULL(Indirizzi_Cess_Diverso.ind_des,'') + ' ' + ISNULL(Indirizzi_Cess_Diverso.frz_des,'') + ' ' + ISNULL(ISTAT_Cess_Diverso.LOCALITA, '') + ' (' + ISNULL(ISTAT_Cess_Diverso.COMUNI_PROV, '') + ')'  ")
                StbSQL.AppendLine("+ '|' +  ")
                StbSQL.AppendLine("ISNULL(Contatti_Cess_Diverso.Codice_Fiscale,'') ")
                StbSQL.Append("                         AS Dati_Documento " & vbCrLf)
                StbSQL.Append("                 FROM Movimenti Mov_Cont   " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Risorse_Umane ON Mov_Cont.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
                'StbSQL.Append("                 INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN ContattiXIndirizzi ON Contatti.Cod_Contatto =ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Cod_Indirizzo = Mov_Cont.Cod_IndirizzoRisUm  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("                 INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM " & vbCrLf)
                'Join Cessionario_Diverso
                StbSQL.Append("Left Join Risorse_Umane risorse_umane_cess_diverso ON Mov_Cont.Cod_Destinazione = risorse_umane_cess_diverso.Cod_RisUm  ")
                StbSQL.Append("LEFT Join Contatti Contatti_Cess_Diverso ON risorse_umane_cess_diverso.Cod_Contatto = Contatti_Cess_Diverso.Cod_Contatto  ")
                StbSQL.Append("LEFT Join Indirizzi Indirizzi_Cess_Diverso ON Mov_Cont.Cod_IndirizzoDestinazione = Indirizzi_Cess_Diverso.cod_indirizzo ")
                StbSQL.Append("Left Join ISTAT ISTAT_Cess_Diverso ON Indirizzi_Cess_Diverso.pro_cod_istat = ISTAT_Cess_Diverso.PROV And Indirizzi_Cess_Diverso.com_cod_istat = ISTAT_Cess_Diverso.COM ")

                StbSQL.Append("                 WHERE Agenda.PIVA = Mov_Cont.PIVA  AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
                StbSQL.Append("                 AND    Mov_Cont.Cau_Mov = '" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                'StbSQL.Append("                 AND    Rapporti_Contabili.Piva = '" & CStr(objParametri.PivaSuperUser) & "' " & vbCrLf)
                StbSQL.Append("                 )" & vbCrLf) 'parentesi della prima query

                StbSQL.Append("         UNION ALL " & vbCrLf)

                StbSQL.Append("                 (" & vbCrLf) 'parentesi della seconda query: legge ImpreseXIndirizzi
                StbSQL.Append("                 -- LEGGE L'INDIRIZZO DELL'IMPRESA GIAS " & vbCrLf)
                StbSQL.Append("                 SELECT Mov_Cont.Doc_Numero_Sin + FORMAT(Mov_Cont.Doc_Numero, 'g17') + Mov_Cont.Doc_Numero_Des " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         Contatti.Rag_Soc " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Contatti.Cod_Contatto,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Risorse_Umane.Attivita_des,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Indirizzi.ind_des,'') + ' ' + ISNULL(Indirizzi.frz_des,'') + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')' " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Contatti.Codice_Fiscale,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Mov_Cont.causale_trasporto,'') " & vbCrLf)
                StbSQL.AppendLine("                    + '|' + ") 'Cessionario diverso
                StbSQL.AppendLine("                      ISNULL(Contatti_Cess_Diverso.Rag_Soc, '') +ISNULL(Contatti_Cess_Diverso.Nome,'') + ' ' + ISNULL(Contatti_Cess_Diverso.cognome,'') ")
                StbSQL.AppendLine("                    + '|' + ")
                StbSQL.AppendLine("                     ISNULL(Contatti_Cess_Diverso.Cod_Contatto,'') ")
                StbSQL.AppendLine("                    + '|' + ")
                StbSQL.AppendLine("                     ISNULL(Indirizzi_Cess_Diverso.ind_des,'') + ' ' + ISNULL(Indirizzi_Cess_Diverso.frz_des,'') + ' ' + ISNULL(ISTAT_Cess_Diverso.LOCALITA, '') + ' (' + ISNULL(ISTAT_Cess_Diverso.COMUNI_PROV, '') + ')'  ")
                StbSQL.AppendLine("                    + '|' +  ")
                StbSQL.AppendLine("                     ISNULL(Contatti_Cess_Diverso.Codice_Fiscale,'') ")
                StbSQL.Append("                         AS Dati_Documento " & vbCrLf)

                StbSQL.Append("                 FROM Movimenti Mov_Cont   " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Risorse_Umane ON Mov_Cont.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
                'StbSQL.Append("                 INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN ImpreseXIndirizzi ON Contatti.Cod_Contatto =ImpreseXIndirizzi.Piva AND ImpreseXIndirizzi.Cod_Indirizzo = Mov_Cont.Cod_IndirizzoRisUm  " & vbCrLf)
                StbSQL.Append("                 INNER JOIN Indirizzi ON ImpreseXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("                 INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM " & vbCrLf)
                'Join per Cessionario_Diverso
                StbSQL.Append("Left Join Risorse_Umane risorse_umane_cess_diverso ON Mov_Cont.Cod_Destinazione = risorse_umane_cess_diverso.Cod_RisUm  ")
                StbSQL.Append("LEFT Join Contatti Contatti_Cess_Diverso ON risorse_umane_cess_diverso.Cod_Contatto = Contatti_Cess_Diverso.Cod_Contatto  ")
                StbSQL.Append("LEFT Join Indirizzi Indirizzi_Cess_Diverso ON Mov_Cont.Cod_IndirizzoDestinazione = Indirizzi_Cess_Diverso.cod_indirizzo ")
                StbSQL.Append("Left Join ISTAT ISTAT_Cess_Diverso ON Indirizzi_Cess_Diverso.pro_cod_istat = ISTAT_Cess_Diverso.PROV And Indirizzi_Cess_Diverso.com_cod_istat = ISTAT_Cess_Diverso.COM ")

                StbSQL.Append("                 WHERE Agenda.PIVA = Mov_Cont.PIVA  AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)
                StbSQL.Append("                 AND    Mov_Cont.Cau_Mov = '" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                'StbSQL.Append("                 AND    Rapporti_Contabili.Piva = '" & CStr(objParametri.PivaSuperUser) & "' " & vbCrLf)
                StbSQL.Append("                 )" & vbCrLf) 'parentesi della seconda query

                StbSQL.Append("                 )" & vbCrLf) 'parentesi della union

                StbSQL.Append("                 AS Dati_Documento ) " & vbCrLf) 'parentesi del TOP 1

                StbSQL.Append("          , '' ) AS Dati_Documento " & vbCrLf)

            Else
                '---------------------------------------------------------------------
                ' CASO CON FILTRO DEL COD_RISUM
                '---------------------------------------------------------------------
                'Colonne del documento contabile (non gestisco ImpreseXIndirizzi)
                StbSQL.Append("         ISNULL(" & vbCrLf)
                StbSQL.Append("                ( Mov_Cont.Doc_Numero_Sin + LTRIM(STR(Mov_Cont.Doc_Numero, 10, 0)) + Mov_Cont.Doc_Numero_Des " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         Contatti.Rag_Soc " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Contatti.Cod_Contatto,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Risorse_Umane.Attivita_Des,'') " & vbCrLf)
                StbSQL.Append("                        + '|' + " & vbCrLf)
                StbSQL.Append("                         ISNULL(Indirizzi.ind_des,'') + ' ' + ISNULL(Indirizzi.frz_des,'') + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' (' + ISNULL(ISTAT.COMUNI_PROV, '') + ')' " & vbCrLf)
                StbSQL.Append("                 ), '' ) AS Dati_Documento " & vbCrLf)
            End If

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Agenda  " & vbCrLf)

            'no AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod
            StbSQL.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  AND Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti_Dettagli ON Movimenti.PIVA = Movimenti_Dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov  " & vbCrLf)

            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Movimenti_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_Dettagli.Mat_Cod   " & vbCrLf)

            If Cod_RisUm <> 0 Then
                '---------------------------------------------------------------------
                ' CASO CON FILTRO DEL COD_RISUM
                '---------------------------------------------------------------------
                'legge ContattiXIndirizzi
                StbSQL.Append(" INNER JOIN  Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA  AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda   " & vbCrLf)
                StbSQL.Append(" INNER JOIN Risorse_Umane ON Mov_Cont.Cod_RisUm = Risorse_Umane.Cod_RisUm  " & vbCrLf)
                'StbSQL.Append(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
                StbSQL.Append(" INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
                StbSQL.Append(" INNER JOIN ContattiXIndirizzi ON Contatti.Cod_Contatto =ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Cod_Indirizzo = Mov_Cont.Cod_IndirizzoRisUm  " & vbCrLf)
                StbSQL.Append(" INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = Istat.PROV AND Indirizzi.com_cod_istat = Istat.COM " & vbCrLf)
            End If

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------

            '12/11/2018: aggiunte causali
            StbSQL.Append("WHERE (   (  Agenda.Lav_Cod IN ( " &
                                   CStr(LAVCOD_BOLLA_EMESSA) & "," &
                                   CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & "," &
                                   CStr(LAVCOD_DOCO_EMESSO) & "," &
                                   CStr(LAVCOD_MVV_EMESSO) & "," &
                                   CStr(LAVCOD_DAA_EMESSO) & "," &
                                   CStr(LAVCOD_FATTURA_EMESSA) & "," &
                                   CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & "," &
                                   CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & "," &
                                   CStr(LAVCOD_RICEVUTA_EMESSA) & "," &
                                   CStr(LAVCOD_CONFERIMENTO_DIVERSI) & "," &
                                   CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & "," &
                                   CStr(LAVCOD_AUTOCONSUMO) & "," &
                                   CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & "," &
                                   CStr(LAVCOD_TRASFERIMENTO) & "," &
                                   CStr(LAVCOD_VENDITA) & ")" & vbCrLf)
            StbSQL.Append("         ) ")
            StbSQL.Append("     OR ")
            '   A T T E N Z I O N E ! ! !
            'se si aggiungono dei pendenti al filtro, occorre gestirli anche nella pagina SchedaVenditeBiologico.aspx
            StbSQL.Append("     (Agenda.Lav_Cod=" & CStr(LAVCOD_SCARICO) & " AND Movimenti_Dettagli.PENDENTE IN (" & CStr(enum_Pendenza.Smaltimento) & ", " &
                                                                                                                    CStr(enum_Pendenza.Furto) & ", " &
                                                                                                                    CStr(enum_Pendenza.ScaricoFuoriRegione) & ", " &
                                                                                                                    CStr(enum_Pendenza.ResoFornitore) & ", " &
                                                                                                                    CStr(enum_Pendenza.Rottura) & ", " &
                                                                                                                    CStr(enum_Pendenza.Altra_Pendenza) & ", " &
                                                                                                                    CStr(22) & ", " &
                                                                                                                    CStr(enum_Pendenza.AutoConsumo) & ") ) " & vbCrLf)
            StbSQL.Append("         ) ")
            '                                                                                                       22 è il furto sul giaslan 


            'SOLO FATTURE IMMEDIATE, NON DIFFERITE (CHE HANNO IL JOLLY_INT=1)
            StbSQL.Append(" AND     Movimenti_dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Materie_Prime.Regolamento = 4 " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                '---------------------------------------------------------------------
                ' CASO CON FILTRO DEL COD_RISUM
                '---------------------------------------------------------------------
                StbSQL.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "   " & vbCrLf)
                StbSQL.Append(" AND Mov_Cont.Cau_Mov = '" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                'StbSQL.Append(" AND Rapporti_Contabili.Piva = '" & CStr(objParametri.PivaSuperUser) & "' " & vbCrLf)
            End If

            StbSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            '22/08/2018 MAGA:
            'da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento
            'l'ultimo giorno non veniva conteggiato
            Data_Fine = DateAdd(DateInterval.Day, 1, CDate(Data_Fine))
            'StbSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)

            '12/11/2018: eliminato il carico, altrimenti per i trasferimenti si vedeva due volte il movimento
            StbSQL.Append(" AND    Movimenti.Cau_Mov IN ( '" &
                                    Agro_SQL_SaveText(CAU_SCARICO) & "', '" &
                                    Agro_SQL_SaveText(CAU_ABBUONI) & "', '" &
                                    Agro_SQL_SaveText(CAU_CONFERIMENTO) & "', '" &
                                    Agro_SQL_SaveText(CAU_CONFERIMENTO_DIVERSI) & "', '" &
                                    Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI) & "', '" &
                                    Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) &
                                    "'  ) " & vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            StbSQL.Append(" GROUP BY Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib,  ")
            StbSQL.Append(" Movimenti.Data_Movimento,  Movimenti.Cau_Mov, Movimenti_dettagli.pendente, ")
            StbSQL.Append(" Mov_Destinazioni.Sa_Cod, Movimenti_dettagli.Elem_Cod, ")
            StbSQL.Append(" Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Pro_Cod,  ")
            StbSQL.Append(" Materie_Prime.Mat_Des , Materie_Prime.Cod_Articolo,	 Movimenti_dettagli.Lotto,  ")
            StbSQL.Append(" Movimenti_dettagli.Udm_Cod,  UnitaMisura.Udm_Sim ")
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'doc_numero è 0 perché è quello del movimento di magazzino
                'StbSQL.Append(" ORDER BY Movimenti.Data_Movimento, Movimenti.Doc_Numero, Etichetta ")
                StbSQL.Append(" ORDER BY Movimenti.Data_Movimento, des_lib, Mat_Des")
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

End Class
