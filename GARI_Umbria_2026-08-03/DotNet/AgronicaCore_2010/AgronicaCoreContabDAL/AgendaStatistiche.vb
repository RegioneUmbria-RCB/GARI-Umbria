Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.attivita
Imports InData.Agenda

Public Class AgendaStatistiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Estrazione_OpColturali(ByVal inData As LeggiAgendaStatistiche,
                                           ByRef objp_Server As AgronicaCoreParametri,
                                           ByRef objp_Utenti As AgronicaCoreParametri,
                                           Optional ByVal pivaList As String = "",
                                           Optional ByVal specieCodList As String = "",
                                           Optional ByVal filtroOperazioni As String = "",
                                           Optional ByVal filtroLavorazioni As String = "",
                                           Optional ByVal referentiList As String = "",
                                           Optional ByVal filtroNazioni As String = "",
                                           Optional ByVal filtroRegioni As String = "",
                                           Optional ByVal filtroProvince As String = "",
                                           Optional ByVal filtroComuni As String = "",
                                           Optional ByVal filtro_visiblita_utente As Boolean = True,
                                           Optional ByVal filtro_vegetali_utente As String = "") As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgendaStatistiche_R.Estrazione()"
        Dim stb As New StringBuilder
        Dim messaggioErrore As String = ""
        Dim dt As New DataTable

        Try

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            stb.AppendLine(" with ")
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" PIVE_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(pivaList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" PIVE_REFERENTI_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(referentiList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Specie_vegetale.Length > 0 Then
                stb.AppendLine(" SPECIE_CTE as (")
                stb.AppendLine(" select Veg_Cod, Veg_Des")
                stb.AppendLine(" from SpecieVegetali")
                stb.AppendLine(" WHERE Veg_Cod IN " & Agro_SQL_Save_Clausola_IN(specieCodList) & " ")
                If filtro_vegetali_utente.Length > 0 Then
                    stb.AppendLine(" AND " & filtro_vegetali_utente)
                End If
                stb.AppendLine(" ), ")
            End If

            stb.AppendLine(" LOCALITA_CTE as (")
            stb.AppendLine(" Select ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice, ")
            stb.AppendLine(" ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, ")
            stb.AppendLine(" Lista_Regioni.REG, ")
            stb.AppendLine(" Lista_Regioni.Regione_Des, ")
            stb.AppendLine(" Lista_Province.PROVINCIA, ")
            stb.AppendLine(" ISTAT.PROV, ISTAT.COM, ")
            stb.AppendLine(" ISTAT.LOCALITA ")
            stb.AppendLine(" FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            stb.AppendLine(" JOIN Lista_Regioni on Lista_Regioni.Stato_Country = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")
            stb.AppendLine(" JOIN Lista_Province on Lista_Province.REG = Lista_Regioni.REG ")
            stb.AppendLine(" JOIN ISTAT on ISTAT.PROV = Lista_Province.PROV ")
            'stb.AppendLine(" WHERE Lista_Regioni.Regione_Des <> 'Not Defined' and Lista_Regioni.Regione_Des <> 'Non Definita' ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" indirizzi_imprese_CTE as ( ")
            stb.AppendLine("select Piva, MAX(cod_indirizzo) As Cod_Indirizzo from ImpresexIndirizzi ")
            stb.AppendLine("group by Piva ")
            stb.AppendLine("), ")

            stb.AppendLine(" utenti_CTE as (")
            stb.AppendLine(" select cognome, nome, rag_soc, codfisc")
            stb.AppendLine(" from " + objp_Utenti.Recupera_NomeDB + ".dbo.Utenti_Dettagli")
            stb.AppendLine(" ), ")

            If inData.Estrazione = 0 OrElse inData.Estrazione = 1 Then
                stb.AppendLine(Estrazione_OpColturaliCompCte(inData,
                                           objp_Server,
                                           filtroOperazioni,
                                           filtroLavorazioni,
                                           filtroNazioni,
                                           filtroRegioni,
                                           filtroProvince,
                                           filtroComuni,
                                           filtro_visiblita_utente,
                                           True))
            End If

            If inData.Estrazione = 0 Then
                stb.Append(",")
            End If

            If inData.Estrazione = 0 OrElse inData.Estrazione = 2 Then
                stb.AppendLine(Estrazione_OpColturaliCompCte(inData,
                                           objp_Server,
                                           filtroOperazioni,
                                           filtroLavorazioni,
                                           filtroNazioni,
                                           filtroRegioni,
                                           filtroProvince,
                                           filtroComuni,
                                           filtro_visiblita_utente,
                                           False))
            End If

            If inData.Estrazione = 0 OrElse inData.Estrazione = 1 Then
                stb.AppendLine(Estrazione_OpColturaliCompSelect(inData, True))
            End If

            If inData.Estrazione = 0 Then
                stb.AppendLine(" UNION ")
            End If

            If inData.Estrazione = 0 OrElse inData.Estrazione = 2 Then
                stb.AppendLine(Estrazione_OpColturaliCompSelect(inData, False))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objp_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objp_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function Estrazione_OpColturaliCompCte(ByVal inData As LeggiAgendaStatistiche,
                                           ByRef objp_Server As AgronicaCoreParametri,
                                           ByVal filtroOperazioni As String,
                                           ByVal filtroLavorazioni As String,
                                           ByVal filtroNazioni As String,
                                           ByVal filtroRegioni As String,
                                           ByVal filtroProvince As String,
                                           ByVal filtroComuni As String,
                                           ByVal filtro_visiblita_utente As Boolean,
                                           ByVal soloSenza As Boolean) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgendaStatistiche_R.Estrazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim nomeCte As String = If(soloSenza, "colture_CTE_senza", "colture_CTE_con")

        stb.Length = 0

        stb.AppendLine(" " + nomeCte + " as ( ")
        stb.AppendLine(" SELECT    i.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda ")
        stb.AppendLine("         , Agenda.Lav_Cod, Agenda.Des_Lib ")
        stb.AppendLine("         , Movimenti.Data_Movimento, Movimenti.Ora, YEAR(Movimenti.Data_Movimento) AS Anno_Movimento ")
        stb.AppendLine("         , MONTH(Movimenti.Data_Movimento) AS Mese_Movimento, Movimenti.Mov_Desc ")
        stb.AppendLine("         , ISNULL((utentiAzienda.Cognome + ' ' + utentiAzienda.Nome + ' ' + utentiAzienda.Rag_Soc), 'N.D.') As Utente_Creazione_Azienda  ")
        stb.AppendLine("         , i.Data_Creazione As Data_Creazione_Azienda ")
        stb.AppendLine("         , YEAR(i.Data_Creazione) As Anno_Creazione_Azienda ")
        stb.AppendLine("         , MONTH(i.Data_Creazione) As Mese_Creazione_Azienda")
        stb.AppendLine("         , ISNULL((utentiOperazioni.Cognome + ' ' + utentiOperazioni.Nome + ' ' + utentiOperazioni.Rag_Soc), 'N.D.') As Utente_Creazione_Op ")
        stb.AppendLine("         , Movimenti.Data_Creazione As Data_Creazione_Op ")
        stb.AppendLine("         , YEAR(Movimenti.Data_Creazione) As Anno_Creazione_Op ")
        stb.AppendLine("         , MONTH(Movimenti.Data_Creazione) As Mese_Creazione_Op ")
        stb.AppendLine("         , Agenda.Username_Creazione, Agenda.Data_Creazione, Movimenti.Cau_Mov ")
        If inData.Impianti Then
            stb.AppendLine("         , ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod ")
            stb.AppendLine("         , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
            stb.AppendLine("         , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            stb.AppendLine("         , ISNULL(Cultivar.Cul_des, '') AS cul_des ")
            stb.AppendLine("         , ISNULL(Campi.Campo_des, '') AS campo_des ")
            stb.AppendLine("         , ISNULL((utentiImp.Cognome + ' ' + utentiImp.Nome + ' ' + utentiImp.Rag_Soc), 'N.D.') As Utente_Creazione_Imp ")
            stb.AppendLine("         , Reg_Impianti.Data_Creazione As Data_Creazione_Imp ")
            stb.AppendLine("         , YEAR(Reg_Impianti.Data_Creazione) As Anno_Creazione_Imp ")
            stb.AppendLine("         , MONTH(Reg_Impianti.Data_Creazione) As Mese_Creazione_Imp")
            stb.AppendLine("         , ISNULL(Reg_Impianti.ID_REG, 0) AS IdImpianto")
            stb.AppendLine("         , ISNULL(Reg_Impianti.sup_imp, 0) AS SupApp")
            stb.AppendLine("         , ISNULL(Appezzamento.APPEZZA, 0) AS Appezza ")
            stb.AppendLine("         , ISNULL(Appezzamento.APP_NOME, '') AS App_Nome ")
            stb.AppendLine("         , ISNULL(Imprese_Progetti.Progetto_Nome, '') AS LottoImpianto ")
            stb.AppendLine("         , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
            stb.AppendLine("         , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")
            stb.AppendLine("         , LOCALITA_CTE.Descrizione as STATO, LOCALITA_CTE.Regione_Des AS REGIONE, LOCALITA_CTE.PROVINCIA, LOCALITA_CTE.LOCALITA")
            stb.AppendLine("         , g.ElementoGrafico_Cod ")
            stb.AppendLine("         , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
            stb.AppendLine(" 		   , ISNULL(Mov_Destinazioni.Qta, 0) AS QtaImp")
            stb.AppendLine("         , ISNULL(Mov_Destinazioni.Qta2, 0) AS SupTrattata ")
            stb.AppendLine("         , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

        Else
            stb.AppendLine("         , 0 AS Cul_Cod ")
            stb.AppendLine("         , 0 AS Veg_Cod ")
            stb.AppendLine("         , '' AS Veg_Des ")
            stb.AppendLine("         , '' AS cul_des ")
            stb.AppendLine("         , '' AS campo_des ")
            stb.AppendLine("         , 0 AS IdImpianto")
            stb.AppendLine("         , '' As Utente_Creazione_Imp ")
            stb.AppendLine("         , CAST(null as DATETIME) As Data_Creazione_Imp ")
            stb.AppendLine("         , 0 As Anno_Creazione_Imp ")
            stb.AppendLine("         , 0 As Mese_Creazione_Imp")
            stb.AppendLine("         , CAST(0 AS float) AS SupApp")
            stb.AppendLine("         , 0 AS Appezza ")
            stb.AppendLine("         , '' AS App_Nome ")
            stb.AppendLine("         , '' AS LottoImpianto ")
            stb.AppendLine("         , 0 AS DestinazioneTerreniNudi_Cod ")
            stb.AppendLine("         , '' AS DestinazioneTerreniNudi_Des ")
            stb.AppendLine("         , '' as STATO, '' AS REGIONE, '' As PROVINCIA, '' As LOCALITA")
            stb.AppendLine("         , -1 AS Tipo_Destinazione ")
            stb.AppendLine(" 		   , 0 AS QtaImp")
            stb.AppendLine("         , 0 AS SupTrattata ")
            stb.AppendLine("         , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

        End If

        If inData.Prodotti Then
            stb.AppendLine("         , Movimenti_dettagli.id_Mov, Movimenti_dettagli.Id_Mov_Det ")
            stb.AppendLine("         , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            stb.AppendLine("         , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            stb.AppendLine("         , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            stb.AppendLine("         , ISNULL(CategorieMagazzino.NomeComune, '') AS TipoProdotto  ")
            stb.AppendLine("         , Case When CategorieMagazzino.Elem_Cod = 3 Then Fertilizzanti.Fer_Des Else ")
            stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 191 Then Formulati.Fr_Des Else  ")
            stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 196 Then InsettiUtili.Ins_Des Else ")
            stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 197 Then Trappole.Trap_Des Else ISNULL(Materie_Prime.Mat_Des, '') ")
            stb.AppendLine("           End End End End As NomeProdotto  ")
            stb.AppendLine("         , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            stb.AppendLine("         , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
            stb.AppendLine("         , coalesce(Movimenti_dettagli.Qta, 0) AS QtaProd")
            stb.AppendLine("         , Movimenti_dettagli.QTA_EXTRA")
            stb.AppendLine("         , Movimenti_dettagli.Udm_Cod As UdmProd")
            stb.AppendLine("         , coalesce(um2.UDM_SIM, '') AS UdmProdSim")
            stb.AppendLine("         , Movimenti_dettagli.Extra_Int UdmExtra")
            stb.AppendLine("         , coalesce(um1.UDM_SIM, '') AS UdmExtraSim")
        Else
            stb.AppendLine("         , Movimenti.id_Mov, 0 AS Id_Mov_Det ")
            stb.AppendLine("         , 0 AS Elem_Cod ")
            stb.AppendLine("         , 0 AS Mat_Cod ")
            stb.AppendLine("         , 0 AS Pro_Cod ")
            stb.AppendLine("         , '' AS TipoProdotto ")
            stb.AppendLine("         , '' AS NomeProdotto ")
            stb.AppendLine("         , '' AS Cod_Articolo ")
            stb.AppendLine("         , 0 AS Qta_Extra_Totale")
            stb.AppendLine("         , 0 AS QtaProd")
            stb.AppendLine("         , 0 As QTA_EXTRA")
            stb.AppendLine("         , 0 As UdmProd")
            stb.AppendLine("         , '' AS UdmProdSim")
            stb.AppendLine("         , 0 UdmExtra")
            stb.AppendLine("         , '' AS UdmExtraSim")
        End If
        stb.AppendLine("         , Agenda.Raccoglitore_Cod ")
        stb.AppendLine("         , ISNULL((utenti.Cognome + ' ' + utenti.Nome + ' ' + utenti.Rag_Soc), 'N.D.') AS Tecnico ")
        stb.AppendLine("         , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and Centri_Aziendali.piva = Agenda.piva ) , '') AS sa_nome ")
        stb.AppendLine("         , i.rag_soc + ' (' + i.PIVA + ')' as rag_soc ")
        stb.AppendLine("         , Operazioni.lav_des ")
        stb.AppendLine("         , GruppoOperazioni.gru_Des ")
        stb.AppendLine("         , ISNULL(GruppoOperazioni.tipo, 'C') as tipo ")
        stb.AppendLine("         , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

        stb.AppendLine("         , ap.Piva AS PIVA_Padre, ap.rag_soc + ' (' + ap.PIVA + ')' AS Azienda_Padre")
        stb.AppendLine(" FROM Imprese i")

        If Not soloSenza Then
            'finestra temporale
            stb.AppendLine(" LEFT JOIN Utenti u on u.[USER] = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ")
        End If

        'Agenda e movimenti
        If Not soloSenza Then
            stb.AppendLine(" INNER JOIN Agenda ON Agenda.Piva = i.Piva ")
            stb.AppendLine(" INNER JOIN Movimenti ON Movimenti.Piva = Agenda.piva AND ")
            stb.AppendLine("                         Movimenti.Id_Agenda = Agenda.Id_Agenda    ")
        Else
            stb.AppendLine(" LEFT JOIN Agenda ON Agenda.Piva = i.Piva ")
            stb.AppendLine(" LEFT JOIN Movimenti ON Movimenti.Piva = Agenda.piva AND ")
            stb.AppendLine("                        Movimenti.Id_Agenda = Agenda.Id_Agenda    ")
            If inData.Data_inizio_operazione > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inData.Data_inizio_operazione) & " ")
            End If
            If inData.Data_fine_operazione < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(inData.Data_fine_operazione) & " ")
            End If
            If inData.Periodo_da_data_operazione > -1 Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Date.Now.AddDays(-inData.Periodo_da_data_operazione)) & " ")
                stb.AppendLine("  AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Date.Now) & " ")
            End If
            ' Finestra temporale
            'stb.AppendLine(" AND Movimenti.Validita_Inizio <= u.FinestraTemp_Fine ")
            'stb.AppendLine(" AND Movimenti.Validita_Fine >= u.FinestraTemp_Inizio ")
        End If
        If inData.Prodotti Then
            stb.AppendLine(" LEFT  JOIN Movimenti_dettagli ON Movimenti_dettagli.Piva = Movimenti.piva AND ")
            stb.AppendLine("                                  Movimenti_dettagli.id_agenda = Movimenti.id_Agenda and")
            stb.AppendLine("                                  Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")
            ' Prodotti
            stb.AppendLine(" LEFT JOIN Formulati     ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod  ")
            stb.AppendLine(" LEFT JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
            stb.AppendLine(" LEFT JOIN Trappole      ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD  ")
            stb.AppendLine(" LEFT JOIN InsettiUtili  ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod  ")
            stb.AppendLine(" LEFT JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  ")
            stb.AppendLine("                            Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
            stb.AppendLine(" LEFT JOIN CategorieMagazzino ON CategorieMagazzino.ELem_Cod = Movimenti_Dettagli.Elem_Cod ")
            ' UdM 
            stb.AppendLine(" LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
            stb.AppendLine(" LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
        End If
        ' Utenti
        stb.AppendLine(" LEFT JOIN utenti_CTE utenti ON utenti.CodFisc = Agenda.Username_Creazione  ")
        If inData.Impianti Then

            If inData.Prodotti Then
                ' Destinazioni
                stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                stb.AppendLine("                            Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                stb.AppendLine(" 						      Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
                stb.AppendLine(" 							  Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det  ")
            Else
                stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                stb.AppendLine("                            Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                stb.AppendLine(" 						      Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
            End If

            ' Impianti
            stb.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND ")
            stb.AppendLine("                           Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND ")
            stb.AppendLine(" 					      Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND")
            stb.AppendLine(" 						  Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione  ")
            If inData.Data_inizio_impianto > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= " & Agro_SQL_SaveDate(inData.Data_inizio_impianto))
            End If
            If inData.Data_fine_impianto < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione <= " & Agro_SQL_SaveDate(inData.Data_fine_impianto))
            End If
            If inData.Periodo_da_data_Impianto > -1 Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= CONVERT(DateTime, GETDATE() - " + inData.Periodo_da_data_Impianto.ToString + ", 120) ")
            End If

            ' Appezzamenti
            stb.AppendLine(" LEFT JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND ")
            stb.AppendLine("                           Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
            stb.AppendLine(" 						  Appezzamento.APPEZZA = Reg_Impianti.APPEZZA   ")

            ' Esercizi
            stb.AppendLine(" LEFT JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND ")
            stb.AppendLine("                               Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD AND ")
            stb.AppendLine(" 							  Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA AND ")
            stb.AppendLine(" 							  Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
            ' Varieta
            stb.AppendLine(" LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")
            ' Specie
            stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            ' Campi 
            stb.AppendLine(" LEFT JOIN Campi On Campi.Piva = Appezzamento.Piva And ")
            stb.AppendLine("                    Campi.Sa_Cod = Appezzamento.Sa_Cod And ")
            stb.AppendLine(" 		           Campi.Campo_Cod = Appezzamento.Campo_Cod ")
            ' ImpiantiCodici
            stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici On Reg_Impianti.piva = Reg_Impianti_Codici.piva And ")
            stb.AppendLine("                                  Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod And")
            stb.AppendLine(" 								 Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And ")
            stb.AppendLine(" 								 Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg And ")
            stb.AppendLine(" 								 Reg_Impianti.Cul_Cod=0 And ")
            stb.AppendLine(" 								 Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
            ' CodiciAnagrafe
            stb.AppendLine(" LEFT JOIN Codici_Anagrafe On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
            stb.AppendLine(" LEFT JOIN Utenti_CTE utentiImp on utentiImp.CodFisc = Reg_Impianti.Username_Creazione ")

            ' Poligoni
            stb.AppendLine(" LEFT JOIN GIS_Entita e on e.Piva = Reg_Impianti.PIVA ")
            stb.AppendLine("                       and e.Sa_Cod = Reg_Impianti.SA_COD ")
            stb.AppendLine("                       and e.Appezza = Reg_Impianti.APPEZZA ")
            stb.AppendLine("                       and e.Id_Imp = Reg_Impianti.ID_REG ")
            stb.AppendLine("                       and e.TipoEntita_Cod In (19,20,21,22,23) ")
            stb.AppendLine(" LEFT JOIN GIS_ElementiGrafici g on g.Entita_Cod = e.Entita_Cod ")
        End If

        ' ImpresexIndirizzi
        stb.AppendLine(" LEFT JOIN indirizzi_imprese_CTE ON indirizzi_imprese_CTE.PIVA = i.PIVA ")
        ' Indirizzi
        stb.AppendLine(" LEFT JOIN Indirizzi ON Indirizzi.cod_indirizzo = indirizzi_imprese_CTE.cod_Indirizzo ")
        stb.AppendLine(" LEFT JOIN LOCALITA_CTE on LOCALITA_CTE.Codice = Indirizzi.stato AND LOCALITA_CTE.PROV = Indirizzi.pro_cod_istat AND LOCALITA_CTE.COM = Indirizzi.com_cod_istat ")

        ' Operazioni
        stb.AppendLine(" LEFT JOIN dbo.Operazioni On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        ' GruppoOperazioni
        stb.AppendLine(" LEFT JOIN dbo.GruppoOperazioni On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
        ' Attivita
        stb.AppendLine(" LEFT JOIN Attivita On Agenda.Id_Attivita = Attivita.Id_Attivita ")
        'Gerarchia imprese
        stb.AppendLine(" LEFT JOIN (select i.PIVA, i.rag_soc, gi.figlio from GerarchiaImprese gi join imprese i on i.piva = gi.padre) ap on ap.figlio = Agenda.PIVA ")

        stb.AppendLine(" LEFT JOIN Utenti_CTE utentiAzienda on utentiAzienda.CodFisc = i.Username_Creazione ")
        stb.AppendLine(" LEFT JOIN Utenti_CTE utentiOperazioni on utentiOperazioni.CodFisc = Movimenti.Username_Creazione ")

        ' FILTRI CTE
        If inData.Piva_aziende.Length > 0 Then
            stb.AppendLine(" JOIN PIVE_CTE pcte on pcte.Piva = i.piva ")
        End If

        If inData.Aziende_referenti.Length > 0 Then
            stb.AppendLine(" INNER JOIN PIVE_REFERENTI_CTE prcte on prcte.Piva = ap.piva")
        End If

        If Not soloSenza Then
            If filtro_visiblita_utente Then
                'visibilità imprese
                stb.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND u.[USER] = Utenti_Visibilita_Appoggio.Username ")
            End If
        End If

        stb.AppendLine(" ")
        ' FILTRI
        stb.AppendLine(" WHERE 1=1")
        'stb.AppendLine("  AND Agenda.Piva in ('01406660389','01311570384','01745310381','01156200386')")
        'stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ") 'todo check
        If Not soloSenza Then
            stb.AppendLine(" AND CONVERT(DateTime, GETDATE(), 120) BETWEEN u.FinestraTemp_Inizio AND u.FinestraTemp_Fine ") 'finestra temporale
            If filtroOperazioni <> "" Then
                stb.AppendLine(" AND " & filtroOperazioni & " ") 'visibilità operazioni
            End If
            If filtroLavorazioni <> "" Then
                stb.AppendLine(" AND " & filtroLavorazioni & " ") 'visibilità lavorazioni
            End If

            'stb.AppendLine("  --AND (  GruppoOperazioni.TIPO = 'C'  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 )  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 )  ) ")
            stb.AppendLine("  AND (Agenda.Lav_Cod < 1000)  ")
            stb.AppendLine("  AND Movimenti.Cau_Mov IN ('2050','2100','2200','2300') ")
            If inData.Data_inizio_operazione > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inData.Data_inizio_operazione) & " ")
            End If
            If inData.Data_fine_operazione < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(inData.Data_fine_operazione) & " ")
            End If
            If inData.Periodo_da_data_operazione > -1 Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Date.Now.AddDays(-inData.Periodo_da_data_operazione)) & " ")
                stb.AppendLine("  AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Date.Now) & " ")
            End If
        End If
        If Not soloSenza Then
            If inData.Data_inizio_operazione > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inData.Data_inizio_operazione) & " ")
            End If
            If inData.Data_fine_operazione < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(inData.Data_fine_operazione) & " ")
            End If
            If inData.Periodo_da_data_operazione > -1 Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Date.Now.AddDays(-inData.Periodo_da_data_operazione)) & " ")
                stb.AppendLine("  AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Date.Now) & " ")
            End If
            ' Finestra temporale
            stb.AppendLine(" AND Movimenti.Validita_Inizio <= u.FinestraTemp_Fine ")
            stb.AppendLine(" AND Movimenti.Validita_Fine >= u.FinestraTemp_Inizio ")
        End If
        If inData.Impianti Then
            stb.AppendLine("  AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
            stb.AppendLine("  AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")
        End If

        If inData.Nazioni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.Codice IN " & Agro_SQL_Save_Clausola_IN(filtroNazioni, True) & " ")
        End If

        If inData.Regioni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.REG IN " & Agro_SQL_Save_Clausola_IN(filtroRegioni, True) & " ")
        End If

        If inData.Province.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.PROV IN " & Agro_SQL_Save_Clausola_IN(filtroProvince, True) & " ")
        End If

        If inData.Comuni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.COM " & Agro_SQL_Save_Clausola_IN(filtroComuni, True) & " ")
        End If

        stb.AppendLine(" ) ")
        stb.AppendLine(" ")

        Return stb.ToString

    End Function

    Private Function Estrazione_OpColturaliCompSelect(ByVal inData As LeggiAgendaStatistiche,
                                                      ByVal soloSenza As Boolean) As String

        Dim stb As New StringBuilder
        Dim nomeCte As String = If(soloSenza, "colture_CTE_senza", "colture_CTE_con")

        stb.AppendLine(" select *, ")
        stb.AppendLine(" sum(case when Id_Mov is null then 0 else 1 end) num_operazioni, ")

        If inData.Impianti Then

            stb.AppendLine("  COUNT( DISTINCT App_Nome ) num_imp, ")
            stb.AppendLine("  COUNT( DISTINCT ElementoGrafico_Cod ) num_pol, ")
            stb.AppendLine("  COUNT( DISTINCT App_Nome ) - COUNT( DISTINCT ElementoGrafico_Cod ) num_pol_manc  ")

        Else

            stb.AppendLine(" 0 as num_imp, 0 as num_pol, 0 as num_pol_manc ")

        End If

        stb.AppendLine(" from " + nomeCte + " ")

        If inData.Specie_vegetale.Length > 0 Then
            stb.AppendLine(" JOIN SPECIE_CTE scte on scte.Veg_Cod = " + nomeCte + ".Veg_Cod And scte.Veg_Des = " + nomeCte + ".Veg_Des")
        End If

        stb.AppendLine(" group by Piva, Sa_Cod, Id_Agenda ,id_Mov, Id_Mov_Det , Lav_Cod, Des_Lib ")
        stb.AppendLine("         , Data_Movimento, Ora, Anno_Movimento, Mese_Movimento, Mov_Desc ")
        stb.AppendLine("         , Username_Creazione, Data_Creazione, Cau_Mov ")
        stb.AppendLine("         , Cul_Cod , " + nomeCte + ".Veg_Cod , " + nomeCte + ".Veg_Des , Raccoglitore_Cod , Tecnico ")
        stb.AppendLine("         , Tipo_Destinazione , Appezza , App_Nome , Elem_Cod ")
        stb.AppendLine("         , Mat_Cod , Pro_Cod, campo_des ")

        If inData.Impianti Then
            stb.AppendLine(" , ElementoGrafico_Cod ")
        End If

        stb.AppendLine("         , Cod_Articolo, TipoProdotto, NomeProdotto ")
        stb.AppendLine("         , IdImpianto, sa_nome, rag_soc, lav_des, gru_Des, tipo, cul_des")
        stb.AppendLine("         , LottoImpianto, SupApp, QtaImp, SupTrattata")
        stb.AppendLine("         , DestinazioneTerreniNudi_Cod , DestinazioneTerreniNudi_Des")
        stb.AppendLine("         , Data_Ultima_Modifica_Intervento , validita_inizio_destinazione")
        stb.AppendLine("         , Qta_Extra_Totale, QtaProd, QTA_EXTRA, UdmProd")
        stb.AppendLine("         ,  UdmProdSim, UdmExtra,  UdmExtraSim")
        stb.AppendLine("         , PIVA_Padre, Azienda_Padre")
        stb.AppendLine("         , STATO, REGIONE, PROVINCIA, LOCALITA ")
        stb.AppendLine("         , Utente_Creazione_Op, Anno_Creazione_Op, Data_Creazione_Op, Mese_Creazione_Op ")
        stb.AppendLine("         , Utente_Creazione_Imp, Data_Creazione_Imp, Anno_Creazione_Imp, Mese_Creazione_Imp ")
        stb.AppendLine("         , Utente_Creazione_Azienda, Data_Creazione_Azienda, Anno_Creazione_Azienda, Mese_Creazione_Azienda ")

        If soloSenza Then
            stb.AppendLine(" HAVING sum(Case When Id_Agenda Is null Then 0 Else 1 End) = 0")
        Else
            stb.AppendLine(" HAVING sum(Case When Id_Agenda Is null Then 0 Else 1 End) > 0")
        End If

        Return stb.ToString

    End Function

    Public Function Estrazione_PostRaccolta(ByVal inData As LeggiAgendaStatistiche,
                                            ByRef objp_Server As AgronicaCoreParametri,
                                            ByRef objp_Utenti As AgronicaCoreParametri,
                                            Optional ByVal pivaList As String = "",
                                            Optional ByVal specieCodList As String = "",
                                            Optional ByVal filtroOperazioni As String = "",
                                            Optional ByVal filtroLavorazioni As String = "",
                                            Optional ByVal referentiList As String = "",
                                            Optional ByVal filtroNazioni As String = "",
                                            Optional ByVal filtroRegioni As String = "",
                                            Optional ByVal filtroProvince As String = "",
                                            Optional ByVal filtroComuni As String = "",
                                            Optional ByVal filtro_visiblita_utente As Boolean = True,
                                            Optional ByVal filtro_vegetali_utente As String = "") As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgendaStatistiche_R.Estrazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            stb.AppendLine(" with ")
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" PIVE_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(pivaList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" PIVE_REFERENTI_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(referentiList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Specie_vegetale.Length > 0 Then
                stb.AppendLine(" SPECIE_CTE as (")
                stb.AppendLine(" select Veg_Cod, Veg_Des")
                stb.AppendLine(" from SpecieVegetali")
                stb.AppendLine(" WHERE Veg_Cod IN " & Agro_SQL_Save_Clausola_IN(specieCodList) & " ")
                If filtro_vegetali_utente.Length > 0 Then
                    stb.AppendLine(" AND " & filtro_vegetali_utente)
                End If
                stb.AppendLine(" ), ")
            End If

            stb.AppendLine(" LOCALITA_CTE as (")
            stb.AppendLine(" Select ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice, ")
            stb.AppendLine(" ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, ")
            stb.AppendLine(" Lista_Regioni.REG, ")
            stb.AppendLine(" Lista_Regioni.Regione_Des, ")
            stb.AppendLine(" Lista_Province.PROVINCIA, ")
            stb.AppendLine(" ISTAT.PROV, ISTAT.COM, ")
            stb.AppendLine(" ISTAT.LOCALITA ")
            stb.AppendLine(" FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            stb.AppendLine(" JOIN Lista_Regioni on Lista_Regioni.Stato_Country = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")
            stb.AppendLine(" JOIN Lista_Province on Lista_Province.REG = Lista_Regioni.REG ")
            stb.AppendLine(" JOIN ISTAT on ISTAT.PROV = Lista_Province.PROV ")
            'stb.AppendLine(" WHERE Lista_Regioni.Regione_Des <> 'Not Defined' and Lista_Regioni.Regione_Des <> 'Non Definita' ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" indirizzi_imprese_CTE as ( ")
            stb.AppendLine("select Piva, MAX(cod_indirizzo) As Cod_Indirizzo from ImpresexIndirizzi ")
            stb.AppendLine("group by Piva ")
            stb.AppendLine("), ")

            stb.AppendLine(" utenti_CTE as (")
            stb.AppendLine(" select cognome, nome, rag_soc, codfisc")
            stb.AppendLine(" from " + objp_Utenti.Recupera_NomeDB + ".dbo.Utenti_Dettagli")
            stb.AppendLine(" ), ")
            stb.AppendLine(" colture_CTE as ( ")
            stb.AppendLine(" SELECT    Agenda.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda ")
            stb.AppendLine("         , Agenda.Lav_Cod, Agenda.Des_Lib ")
            stb.AppendLine("         , Movimenti.Data_Movimento, Movimenti.Ora, YEAR(Movimenti.Data_Movimento) AS Anno_Movimento ")
            stb.AppendLine("         , MONTH(Movimenti.Data_Movimento) AS Mese_Movimento, Movimenti.Mov_Desc ")
            stb.AppendLine("         , ISNULL((utentiAzienda.Cognome + ' ' + utentiAzienda.Nome + ' ' + utentiAzienda.Rag_Soc), 'N.D.') As Utente_Creazione_Azienda  ")
            stb.AppendLine("         , i.Data_Creazione As Data_Creazione_Azienda ")
            stb.AppendLine("         , YEAR(i.Data_Creazione) As Anno_Creazione_Azienda ")
            stb.AppendLine("         , MONTH(i.Data_Creazione) As Mese_Creazione_Azienda")
            stb.AppendLine("         , ISNULL((utentiOperazioni.Cognome + ' ' + utentiOperazioni.Nome + ' ' + utentiOperazioni.Rag_Soc), 'N.D.') As Utente_Creazione_Op ")
            stb.AppendLine("         , Movimenti.Data_Creazione As Data_Creazione_Op ")
            stb.AppendLine("         , YEAR(Movimenti.Data_Creazione) As Anno_Creazione_Op ")
            stb.AppendLine("         , MONTH(Movimenti.Data_Creazione) As Mese_Creazione_Op ")
            stb.AppendLine("         , Agenda.Username_Creazione, Agenda.Data_Creazione, Movimenti.Cau_Mov ")
            If inData.Impianti Then
                stb.AppendLine("         , ISNULL(Reg_Impianti.Cul_Cod, 0) AS Cul_Cod ")
                stb.AppendLine("         , ISNULL(Cultivar.Veg_Cod, 0) AS Veg_Cod ")
                stb.AppendLine("         , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
                stb.AppendLine("         , ISNULL(Cultivar.Cul_des, '') AS cul_des ")
                stb.AppendLine("         , ISNULL(Campi.Campo_des, '') AS campo_des ")
                stb.AppendLine("         , ISNULL((utentiImp.Cognome + ' ' + utentiImp.Nome + ' ' + utentiImp.Rag_Soc), 'N.D.') As Utente_Creazione_Imp ")
                stb.AppendLine("         , Reg_Impianti.Data_Creazione As Data_Creazione_Imp ")
                stb.AppendLine("         , YEAR(Reg_Impianti.Data_Creazione) As Anno_Creazione_Imp ")
                stb.AppendLine("         , MONTH(Reg_Impianti.Data_Creazione) As Mese_Creazione_Imp")
                stb.AppendLine("         , ISNULL(Reg_Impianti.ID_REG, 0) AS IdImpianto")
                stb.AppendLine("         , ISNULL(Reg_Impianti.sup_imp, 0) AS SupApp")
                stb.AppendLine("         , ISNULL(Appezzamento.APPEZZA, 0) AS Appezza ")
                stb.AppendLine("         , ISNULL(Appezzamento.APP_NOME, '') AS App_Nome ")
                stb.AppendLine("         , ISNULL(Imprese_Progetti.Progetto_Nome, '') AS LottoImpianto ")
                stb.AppendLine("         , ISNULL(Reg_Impianti_Codici.id_cod, 0) AS DestinazioneTerreniNudi_Cod ")
                stb.AppendLine("         , ISNULL(Codici_Anagrafe.descrizione, '') AS DestinazioneTerreniNudi_Des ")
                stb.AppendLine("         , LOCALITA_CTE.Descrizione as STATO, LOCALITA_CTE.Regione_Des AS REGIONE, LOCALITA_CTE.PROVINCIA, LOCALITA_CTE.LOCALITA")
                stb.AppendLine("         , g.ElementoGrafico_Cod ")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
                stb.AppendLine(" 		   , ISNULL(Mov_Destinazioni.Qta, 0) AS QtaImp")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.Qta2, 0) AS SupTrattata ")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            Else
                stb.AppendLine("         , 0 AS Cul_Cod ")
                stb.AppendLine("         , 0 AS Veg_Cod ")
                stb.AppendLine("         , '' AS Veg_Des ")
                stb.AppendLine("         , '' AS cul_des ")
                stb.AppendLine("         , '' AS campo_des ")
                stb.AppendLine("         , 0 AS IdImpianto")
                stb.AppendLine("         , '' As Utente_Creazione_Imp ")
                stb.AppendLine("         , CAST(null as DATETIME) As Data_Creazione_Imp ")
                stb.AppendLine("         , 0 As Anno_Creazione_Imp ")
                stb.AppendLine("         , 0 As Mese_Creazione_Imp")
                stb.AppendLine("         , CAST(0 AS float) AS SupApp")
                stb.AppendLine("         , 0 AS Appezza ")
                stb.AppendLine("         , '' AS App_Nome ")
                stb.AppendLine("         , '' AS LottoImpianto ")
                stb.AppendLine("         , 0 AS DestinazioneTerreniNudi_Cod ")
                stb.AppendLine("         , '' AS DestinazioneTerreniNudi_Des ")
                stb.AppendLine("         , '' as STATO, '' AS REGIONE, '' As PROVINCIA, '' As LOCALITA")
                stb.AppendLine("         , -1 AS Tipo_Destinazione ")
                stb.AppendLine(" 		   , 0 AS QtaImp")
                stb.AppendLine("         , 0 AS SupTrattata ")
                stb.AppendLine("         , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            End If

            If inData.Prodotti Then
                stb.AppendLine("         , Movimenti_dettagli.id_Mov, Movimenti_dettagli.Id_Mov_Det ")
                stb.AppendLine("         , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
                stb.AppendLine("         , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
                stb.AppendLine("         , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
                stb.AppendLine("         , ISNULL(CategorieMagazzino.NomeComune, '') AS TipoProdotto  ")
                stb.AppendLine("         , Case When CategorieMagazzino.Elem_Cod = 3 Then Fertilizzanti.Fer_Des Else ")
                stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 191 Then Formulati.Fr_Des Else  ")
                stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 196 Then InsettiUtili.Ins_Des Else ")
                stb.AppendLine("           Case When CategorieMagazzino.Elem_Cod = 197 Then Trappole.Trap_Des Else ISNULL(Materie_Prime.Mat_Des, '') ")
                stb.AppendLine("           End End End End As NomeProdotto  ")
                stb.AppendLine("         , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
                stb.AppendLine("         , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
                stb.AppendLine("         , coalesce(Movimenti_dettagli.Qta, 0) AS QtaProd")
                stb.AppendLine("         , Movimenti_dettagli.QTA_EXTRA")
                stb.AppendLine("         , Movimenti_dettagli.Udm_Cod As UdmProd")
                stb.AppendLine("         , coalesce(um2.UDM_SIM, '') AS UdmProdSim")
                stb.AppendLine("         , Movimenti_dettagli.Extra_Int UdmExtra")
                stb.AppendLine("         , coalesce(um1.UDM_SIM, '') AS UdmExtraSim")
            Else
                stb.AppendLine("         , Movimenti.id_Mov, 0 AS Id_Mov_Det ")
                stb.AppendLine("         , 0 AS Elem_Cod ")
                stb.AppendLine("         , 0 AS Mat_Cod ")
                stb.AppendLine("         , 0 AS Pro_Cod ")
                stb.AppendLine("         , '' AS TipoProdotto ")
                stb.AppendLine("         , '' AS NomeProdotto ")
                stb.AppendLine("         , '' AS Cod_Articolo ")
                stb.AppendLine("         , 0 AS Qta_Extra_Totale")
                stb.AppendLine("         , 0 AS QtaProd")
                stb.AppendLine("         , 0 As QTA_EXTRA")
                stb.AppendLine("         , 0 As UdmProd")
                stb.AppendLine("         , '' AS UdmProdSim")
                stb.AppendLine("         , 0 UdmExtra")
                stb.AppendLine("         , '' AS UdmExtraSim")
            End If
            stb.AppendLine("         , Agenda.Raccoglitore_Cod ")
            stb.AppendLine("         , ISNULL((utenti.Cognome + ' ' + utenti.Nome + ' ' + utenti.Rag_Soc), 'N.D.') AS Tecnico ")

            stb.AppendLine("         , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and Centri_Aziendali.piva = Agenda.piva ) , '') AS sa_nome ")
            stb.AppendLine("         , i.rag_soc + ' (' + i.PIVA + ')' as rag_soc ")
            stb.AppendLine("         , Operazioni.lav_des ")
            stb.AppendLine("         , GruppoOperazioni.gru_Des ")
            stb.AppendLine("         , ISNULL(GruppoOperazioni.tipo, 'C') as tipo ")

            stb.AppendLine("         , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")

            stb.AppendLine("         , ap.Piva AS PIVA_Padre, ap.rag_soc + ' (' + ap.PIVA + ')' AS Azienda_Padre")
            stb.AppendLine(" FROM Imprese i")

            'finestra temporale
            stb.AppendLine(" LEFT JOIN Utenti u on u.[USER] = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ")

            'Agenda e movimenti
            stb.AppendLine(" INNER JOIN Agenda ON Agenda.Piva = i.Piva ")
            stb.AppendLine(" INNER JOIN Movimenti ON Movimenti.Piva = Agenda.piva AND ")
            stb.AppendLine("                         Movimenti.Id_Agenda = Agenda.Id_Agenda    ")

            If inData.Prodotti Then
                stb.AppendLine(" LEFT  JOIN Movimenti_dettagli ON Movimenti_dettagli.Piva = Movimenti.piva AND ")
                stb.AppendLine("                                  Movimenti_dettagli.id_agenda = Movimenti.id_Agenda and")
                stb.AppendLine("                                  Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")
                ' Prodotti
                stb.AppendLine(" LEFT JOIN Formulati     ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod  ")
                stb.AppendLine(" LEFT JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
                stb.AppendLine(" LEFT JOIN Trappole      ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD  ")
                stb.AppendLine(" LEFT JOIN InsettiUtili  ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod  ")
                stb.AppendLine(" LEFT JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  ")
                stb.AppendLine("                            Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
                stb.AppendLine(" LEFT JOIN CategorieMagazzino ON CategorieMagazzino.Elem_cod = Movimenti_Dettagli.Elem_Cod ")
                ' UdM 
                stb.AppendLine(" LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
                stb.AppendLine(" LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            End If
            ' Utenti
            stb.AppendLine(" LEFT JOIN utenti_CTE utenti ON utenti.CodFisc = Agenda.Username_Creazione  ")
            If inData.Impianti Then

                If inData.Prodotti Then
                    ' Destinazioni
                    stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                    stb.AppendLine("                            Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    stb.AppendLine(" 						      Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
                    stb.AppendLine(" 							  Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det  ")
                Else
                    stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                    stb.AppendLine("                            Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    stb.AppendLine(" 						      Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
                End If

                ' Impianti
                stb.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND ")
                stb.AppendLine("                           Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND ")
                stb.AppendLine(" 					      Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND")
                stb.AppendLine(" 						  Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione  ")

                ' Appezzamenti
                stb.AppendLine(" LEFT JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND ")
                stb.AppendLine("                           Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
                stb.AppendLine(" 						  Appezzamento.APPEZZA = Reg_Impianti.APPEZZA   ")

                ' Esercizi
                stb.AppendLine(" LEFT JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND ")
                stb.AppendLine("                               Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD AND ")
                stb.AppendLine(" 							  Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA AND ")
                stb.AppendLine(" 							  Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
                ' Varieta
                stb.AppendLine(" LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")
                ' Specie
                stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
                ' Campi 
                stb.AppendLine(" LEFT JOIN Campi On Campi.Piva = Appezzamento.Piva And ")
                stb.AppendLine("                    Campi.Sa_Cod = Appezzamento.Sa_Cod And ")
                stb.AppendLine(" 		           Campi.Campo_Cod = Appezzamento.Campo_Cod ")
                ' ImpiantiCodici
                stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici On Reg_Impianti.piva = Reg_Impianti_Codici.piva And ")
                stb.AppendLine("                                  Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod And")
                stb.AppendLine(" 								 Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And ")
                stb.AppendLine(" 								 Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg And ")
                stb.AppendLine(" 								 Reg_Impianti.Cul_Cod=0 And ")
                stb.AppendLine(" 								 Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
                ' CodiciAnagrafe
                stb.AppendLine(" LEFT JOIN Codici_Anagrafe On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
                stb.AppendLine(" LEFT JOIN Utenti_CTE utentiImp on utentiImp.CodFisc = Reg_Impianti.Username_Creazione ")
                ' Poligoni
                stb.AppendLine(" LEFT JOIN GIS_Entita e on e.Piva = Reg_Impianti.PIVA ")
                stb.AppendLine("                       and e.Sa_Cod = Reg_Impianti.SA_COD ")
                stb.AppendLine("                       and e.Appezza = Reg_Impianti.APPEZZA ")
                stb.AppendLine("                       and e.Id_Imp = Reg_Impianti.ID_REG ")
                stb.AppendLine("                       and e.TipoEntita_Cod In (19,20,21,22,23) ")
                stb.AppendLine(" LEFT JOIN GIS_ElementiGrafici g on g.Entita_Cod = e.Entita_Cod ")
            End If

            ' ImpresexIndirizzi
            stb.AppendLine(" LEFT JOIN indirizzi_imprese_CTE ON indirizzi_imprese_CTE.PIVA = i.PIVA ")
            ' Indirizzi
            stb.AppendLine(" LEFT JOIN Indirizzi ON Indirizzi.cod_indirizzo = indirizzi_imprese_CTE.cod_Indirizzo ")
            stb.AppendLine(" LEFT JOIN LOCALITA_CTE on LOCALITA_CTE.Codice = Indirizzi.stato AND LOCALITA_CTE.PROV = Indirizzi.pro_cod_istat AND LOCALITA_CTE.COM = Indirizzi.com_cod_istat ")

            ' Operazioni
            stb.AppendLine(" LEFT JOIN dbo.Operazioni On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            ' GruppoOperazioni
            stb.AppendLine(" LEFT JOIN dbo.GruppoOperazioni On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            ' Attivita
            stb.AppendLine(" LEFT JOIN Attivita On Agenda.Id_Attivita = Attivita.Id_Attivita ")

            stb.AppendLine(" LEFT JOIN (select i.PIVA, i.rag_soc, gi.figlio from GerarchiaImprese gi join imprese i on i.piva = gi.padre) ap on ap.figlio = Agenda.PIVA ")

            stb.AppendLine(" LEFT JOIN Utenti_CTE utentiAzienda on utentiAzienda.CodFisc = i.Username_Creazione ")
            stb.AppendLine(" LEFT JOIN Utenti_CTE utentiOperazioni on utentiOperazioni.CodFisc = Movimenti.Username_Creazione ")

            ' FILTRI CTE
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" JOIN PIVE_CTE pcte on pcte.Piva = i.piva ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" INNER JOIN PIVE_REFERENTI_CTE prcte on prcte.Piva = ap.piva")
            End If

            If filtro_visiblita_utente Then
                'visibilità imprese
                stb.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND u.[USER] = Utenti_Visibilita_Appoggio.Username ")
            End If

            stb.AppendLine(" ")
            ' FILTRI
            stb.AppendLine(" WHERE 1=1")
            'stb.AppendLine("  AND Agenda.Piva in ('01406660389','01311570384','01745310381','01156200386')")
            'stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ") 'todo check
            stb.AppendLine(" AND CONVERT(DateTime, GETDATE(), 120) BETWEEN u.FinestraTemp_Inizio AND u.FinestraTemp_Fine ") 'finestra temporale
            If filtroOperazioni <> "" Then
                stb.AppendLine(" AND " & filtroOperazioni & " ") 'visibilità operazioni
            End If
            If filtroLavorazioni <> "" Then
                stb.AppendLine(" AND " & filtroLavorazioni & " ") 'visibilità lavorazioni
            End If
            'stb.AppendLine("  --AND (  GruppoOperazioni.TIPO = 'C'  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 )  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 )  ) ")
            stb.AppendLine("  AND (Agenda.Lav_Cod IN (163,5004,5000))  ")
            stb.AppendLine("  AND Movimenti.Cau_Mov IN ('2050','2100','2200','2300','10001','7350') ")
            If inData.Data_inizio_operazione > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inData.Data_inizio_operazione) & " ")
            End If
            If inData.Data_fine_operazione < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(inData.Data_fine_operazione) & " ")
            End If
            ' Finestra temporale
            stb.AppendLine(" AND Movimenti.Validita_Inizio <= u.FinestraTemp_Fine ")
            stb.AppendLine(" AND Movimenti.Validita_Fine >= u.FinestraTemp_Inizio ")
            If inData.Data_inizio_impianto > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= " & Agro_SQL_SaveDate(inData.Data_inizio_impianto))
            End If
            If inData.Data_fine_impianto < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione <= " & Agro_SQL_SaveDate(inData.Data_fine_impianto))
            End If
            If inData.Periodo_da_data_Impianto > -1 Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= CONVERT(DateTime, GETDATE() - " + inData.Periodo_da_data_Impianto.ToString + ", 120) ")
            End If
            If inData.Periodo_da_data_operazione > -1 Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Date.Now.AddDays(-inData.Periodo_da_data_operazione)) & " ")
                stb.AppendLine("  AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Date.Now) & " ")
            End If
            If inData.Impianti Then
                stb.AppendLine("  AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
                stb.AppendLine("  AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")
            End If

            If inData.Nazioni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.Codice IN " & Agro_SQL_Save_Clausola_IN(filtroNazioni, True) & " ")
            End If

            If inData.Regioni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.REG IN " & Agro_SQL_Save_Clausola_IN(filtroRegioni, True) & " ")
            End If

            If inData.Province.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.PROV IN " & Agro_SQL_Save_Clausola_IN(filtroProvince, True) & " ")
            End If

            If inData.Comuni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.COM IN " & Agro_SQL_Save_Clausola_IN(filtroComuni, True) & " ")
            End If

            stb.AppendLine(" ) ")
            stb.AppendLine(" ")
            stb.AppendLine(" select *, ")
            'stb.AppendLine("   Piva, Sa_Cod, Id_Agenda ,id_Mov, Id_Mov_Det , Lav_Cod, Des_Lib ")
            'stb.AppendLine(" , Data_Movimento, Ora, Anno_Movimento, Mese_Movimento, Mov_Desc ")
            'stb.AppendLine(" , Username_Creazione, Data_Creazione, Cau_Mov ")
            'stb.AppendLine(" , Cul_Cod , colture_CTE.Veg_Cod , colture_CTE.Veg_Des , Raccoglitore_Cod , Tecnico ")
            'stb.AppendLine(" , Tipo_Destinazione , Appezza , App_Nome , Elem_Cod ")
            'stb.AppendLine(" , Mat_Cod , Pro_Cod , MAX(Fr_Des) As Fr_Des , MAX(Fer_Des) As Fer_Des ")
            'stb.AppendLine(" , MAX(Trap_Des) As Trap_Des , MAX(Ins_Des) As Ins_Des , MAX(Mat_Des) As Mat_Des , Cod_Articolo ")
            'stb.AppendLine(" , MAX(sa_nome) As sa_nome , MAX(rag_soc) As rag_soc , MAX(lav_des) As lav_des , MAX(gru_Des) As gru_Des ")
            'stb.AppendLine(" , MAX(tipo) As tipo , MAX(cul_des) As cul_des , MAX(campo_des) As campo_des , IdImpianto")
            'stb.AppendLine(" , SUM(SupApp) As SupApp, SUM(QtaImp) As QtaImp, SUM(SupTrattata) as SupTrattata , LottoImpianto")
            'stb.AppendLine(" , DestinazioneTerreniNudi_Cod , DestinazioneTerreniNudi_Des")
            'stb.AppendLine(" , Data_Ultima_Modifica_Intervento , validita_inizio_destinazione")
            'stb.AppendLine(" , Qta_Extra_Totale, QtaProd, QTA_EXTRA, UdmProd")
            'stb.AppendLine(" , UdmProdSim, UdmExtra,  UdmExtraSim")
            'stb.AppendLine(" , PIVA_Padre, Azienda_Padre")
            'stb.AppendLine(" , STATO, REGIONE, PROVINCIA, LOCALITA")
            'stb.AppendLine(" , Utente_Creazione_Op, MAX(Data_Creazione_Op) as Data_Creazione_Op, Anno_Creazione_Op, Mese_Creazione_Op ")
            'stb.AppendLine(" , Utente_Creazione_Imp, Data_Creazione_Imp, Anno_Creazione_Imp, Mese_Creazione_Imp ")
            'stb.AppendLine(" , Utente_Creazione_Azienda, Data_Creazione_Azienda, Anno_Creazione_Azienda, Mese_Creazione_Azienda,  ")
            stb.AppendLine(" sum(case when Id_Mov is null then 0 else 1 end) num_operazioni, ")

            If inData.Impianti Then

                stb.AppendLine("  COUNT( DISTINCT App_Nome ) num_imp, ")
                stb.AppendLine("  COUNT( DISTINCT ElementoGrafico_Cod ) num_pol, ")
                stb.AppendLine("  COUNT( DISTINCT App_Nome ) - COUNT( DISTINCT ElementoGrafico_Cod ) num_pol_manc  ")

            Else

                stb.AppendLine(" 0 as num_imp, 0 as num_pol, 0 as num_pol_manc ")

            End If

            stb.AppendLine(" from colture_CTE")

            If inData.Specie_vegetale.Length > 0 Then
                stb.AppendLine(" JOIN SPECIE_CTE scte on scte.Veg_Cod = colture_CTE.Veg_Cod AND scte.Veg_Des = colture_CTE.Veg_Des")
            End If

            stb.AppendLine(" group by Piva, Sa_Cod, Id_Agenda ,id_Mov, Id_Mov_Det , Lav_Cod, Des_Lib ")
            stb.AppendLine("         , Data_Movimento, Ora, Anno_Movimento, Mese_Movimento, Mov_Desc ")
            stb.AppendLine("         , Username_Creazione, Data_Creazione, Cau_Mov ")
            stb.AppendLine("         , Cul_Cod , colture_CTE.Veg_Cod , colture_CTE.Veg_Des , Raccoglitore_Cod , Tecnico ")
            stb.AppendLine("         , Tipo_Destinazione , Appezza , App_Nome , Elem_Cod ")
            stb.AppendLine("         , Mat_Cod , Pro_Cod, campo_des ")

            If inData.Impianti Then
                stb.AppendLine(" , ElementoGrafico_Cod ")
            End If

            stb.AppendLine("         , Cod_Articolo, TipoProdotto, NomeProdotto ")
            stb.AppendLine("         , IdImpianto, sa_nome, rag_soc, lav_des, gru_Des, tipo, cul_des")
            stb.AppendLine("         , LottoImpianto, SupApp, QtaImp, SupTrattata")
            stb.AppendLine("         , DestinazioneTerreniNudi_Cod , DestinazioneTerreniNudi_Des")
            stb.AppendLine("         , Data_Ultima_Modifica_Intervento , validita_inizio_destinazione")
            stb.AppendLine("         , Qta_Extra_Totale, QtaProd, QTA_EXTRA, UdmProd")
            stb.AppendLine("         ,  UdmProdSim, UdmExtra,  UdmExtraSim")
            stb.AppendLine("         , PIVA_Padre, Azienda_Padre")
            stb.AppendLine("         , STATO, REGIONE, PROVINCIA, LOCALITA ")
            stb.AppendLine("         , Utente_Creazione_Op, Anno_Creazione_Op, Data_Creazione_Op, Mese_Creazione_Op ")
            stb.AppendLine("         , Utente_Creazione_Imp, Data_Creazione_Imp, Anno_Creazione_Imp, Mese_Creazione_Imp ")
            stb.AppendLine("         , Utente_Creazione_Azienda, Data_Creazione_Azienda, Anno_Creazione_Azienda, Mese_Creazione_Azienda ")

            stb.AppendLine(" HAVING sum(case when Id_Agenda is null then 0 else 1 end) > 0")

            'stb.AppendLine(" where ")
            'stb.AppendLine(" rag_soc = 'BARBONI GIULIANO'")
            'stb.AppendLine(" And colture_CTE.veg_des = 'Barbabietola da zucchero'")
            'stb.AppendLine(" colture_CTE.fer_Des = 'Life N 310 L'")
            ' stb.AppendLine(" */")
            'stb.AppendLine(" ORDER BY ")
            'stb.AppendLine(" colture_CTE.Data_Movimento DESC,")
            'stb.AppendLine(" colture_CTE.Id_Agenda DESC,")
            'stb.AppendLine(" App_Nome ASC ")
            'stb.AppendLine(" ;")

            stb.AppendLine(" ")
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED;")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objp_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objp_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Estrazione_Visite(ByVal inData As LeggiAgendaStatistiche,
                                      ByRef objp_Server As AgronicaCoreParametri,
                                      ByRef objp_Utenti As AgronicaCoreParametri,
                                      Optional ByVal pivaList As String = "",
                                      Optional ByVal specieCodList As String = "",
                                      Optional ByVal filtroOperazioni As String = "",
                                      Optional ByVal filtroLavorazioni As String = "",
                                      Optional ByVal referentiList As String = "",
                                      Optional ByVal filtroNazioni As String = "",
                                      Optional ByVal filtroRegioni As String = "",
                                      Optional ByVal filtroProvince As String = "",
                                      Optional ByVal filtroComuni As String = "",
                                      Optional ByVal filtro_visiblita_utente As Boolean = True) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgendaStatistiche_R.Estrazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            stb.AppendLine(" with ")
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" PIVE_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(pivaList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" PIVE_REFERENTI_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(referentiList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            'If inData.Specie_vegetale.Length > 0 Then
            '    stb.AppendLine(" SPECIE_CTE as (")
            '    stb.AppendLine(" select Veg_Cod, Veg_Des")
            '    stb.AppendLine(" from SpecieVegetali")
            '    stb.AppendLine(" WHERE Veg_Cod " & specieCodList & " ")
            '    stb.AppendLine(" ), ")
            'End If

            stb.AppendLine(" LOCALITA_CTE as (")
            stb.AppendLine(" Select ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice, ")
            stb.AppendLine(" ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, ")
            stb.AppendLine(" Lista_Regioni.REG, ")
            stb.AppendLine(" Lista_Regioni.Regione_Des, ")
            stb.AppendLine(" Lista_Province.PROVINCIA, ")
            stb.AppendLine(" ISTAT.PROV, ISTAT.COM, ")
            stb.AppendLine(" ISTAT.LOCALITA ")
            stb.AppendLine(" FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            stb.AppendLine(" JOIN Lista_Regioni on Lista_Regioni.Stato_Country = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")
            stb.AppendLine(" JOIN Lista_Province on Lista_Province.REG = Lista_Regioni.REG ")
            stb.AppendLine(" JOIN ISTAT on ISTAT.PROV = Lista_Province.PROV ")
            'stb.AppendLine(" WHERE Lista_Regioni.Regione_Des <> 'Not Defined' and Lista_Regioni.Regione_Des <> 'Non Definita' ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" indirizzi_imprese_CTE as ( ")
            stb.AppendLine(" select Piva, MAX(cod_indirizzo) As Cod_Indirizzo from ImpresexIndirizzi ")
            stb.AppendLine(" group by Piva ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" utenti_CTE as (")
            stb.AppendLine(" select cognome, nome, rag_soc, codfisc")
            stb.AppendLine(" from " + objp_Utenti.Recupera_NomeDB + ".dbo.Utenti_Dettagli")
            stb.AppendLine(" ), ")
            stb.AppendLine(" colture_CTE as ( ")
            stb.AppendLine(" SELECT    Agenda.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda ")
            stb.AppendLine("         , Agenda.Lav_Cod, Agenda.Des_Lib ")
            stb.AppendLine("         , ISNULL(Movimenti.Data_Movimento, CONVERT(DateTime,'1900/01/01',120) ) As Data_Movimento, CAST(null as DATETIME) AS Ora, YEAR(Movimenti.Data_Movimento) AS Anno_Movimento ")
            stb.AppendLine("         , MONTH(Movimenti.Data_Movimento) AS Mese_Movimento, '' AS Mov_Desc ")
            stb.AppendLine("         , ISNULL((utentiAzienda.Cognome + ' ' + utentiAzienda.Nome + ' ' + utentiAzienda.Rag_Soc), 'N.D.') As Utente_Creazione_Azienda  ")
            stb.AppendLine("         , i.Data_Creazione As Data_Creazione_Azienda ")
            stb.AppendLine("         , YEAR(i.Data_Creazione) As Anno_Creazione_Azienda ")
            stb.AppendLine("         , MONTH(i.Data_Creazione) As Mese_Creazione_Azienda")
            stb.AppendLine("         , ISNULL((utentiOperazioni.Cognome + ' ' + utentiOperazioni.Nome + ' ' + utentiOperazioni.Rag_Soc), 'N.D.') As Utente_Creazione_Op ")
            stb.AppendLine("         , Movimenti.Data_Creazione As Data_Creazione_Op ")
            stb.AppendLine("         , YEAR(Movimenti.Data_Creazione) As Anno_Creazione_Op ")
            stb.AppendLine("         , MONTH(Movimenti.Data_Creazione) As Mese_Creazione_Op ")
            stb.AppendLine("         , Agenda.Username_Creazione, Agenda.Data_Creazione, '' AS Cau_Mov ")
            stb.AppendLine("         , 0 AS Cul_Cod ")
            stb.AppendLine("         , 0 AS Veg_Cod ")
            stb.AppendLine("         , '' AS Veg_Des ")
            stb.AppendLine("         , '' AS cul_des ")
            stb.AppendLine("         , '' AS campo_des ")
            If inData.Impianti Then
                stb.AppendLine("         , Reg_Impianti.Username_Creazione As Utente_Creazione_Imp ")
                stb.AppendLine("         , Reg_Impianti.Data_Creazione As Data_Creazione_Imp ")
                stb.AppendLine("         , YEAR(Reg_Impianti.Data_Creazione) As Anno_Creazione_Imp ")
                stb.AppendLine("         , MONTH(Reg_Impianti.Data_Creazione) As Mese_Creazione_Imp")
            Else
                stb.AppendLine("         , '' As Utente_Creazione_Imp ")
                stb.AppendLine("         , CAST(null as DATETIME) As Data_Creazione_Imp ")
                stb.AppendLine("         , 0 As Anno_Creazione_Imp ")
                stb.AppendLine("         , 0 As Mese_Creazione_Imp")
            End If
            stb.AppendLine("         , 0 AS IdImpianto")
            stb.AppendLine("         , CAST(0 AS float) AS SupApp")
            stb.AppendLine("         , 0 AS Appezza ")
            stb.AppendLine("         , '' AS App_Nome ")
            stb.AppendLine("         , '' AS LottoImpianto ")
            stb.AppendLine("         , 0 AS DestinazioneTerreniNudi_Cod ")
            stb.AppendLine("         , '' AS DestinazioneTerreniNudi_Des ")

            If inData.Impianti Then
                stb.AppendLine("         , LOCALITA_CTE.Descrizione as STATO, LOCALITA_CTE.Regione_Des AS REGIONE, LOCALITA_CTE.PROVINCIA, LOCALITA_CTE.LOCALITA")
                stb.AppendLine("         , g.ElementoGrafico_Cod ")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.Tipo_Destinazione, -1) AS Tipo_Destinazione ")
                stb.AppendLine(" 		   , ISNULL(Mov_Destinazioni.Qta, 0) AS QtaImp")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.Qta2, 0) AS SupTrattata ")
                stb.AppendLine("         , ISNULL(Mov_Destinazioni.validita_inizio, CONVERT(DateTime,'1900/01/01',120)) AS validita_inizio_destinazione ")

            Else

                stb.AppendLine("         , '' as STATO, '' AS REGIONE, '' As PROVINCIA, '' As LOCALITA")
                stb.AppendLine("         , -1 AS Tipo_Destinazione ")
                stb.AppendLine(" 		   , 0 AS QtaImp")
                stb.AppendLine("         , 0 AS SupTrattata ")
                stb.AppendLine("         , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")

            End If

            stb.AppendLine("         , Movimenti.id_Mov, 0 AS Id_Mov_Det ")
            stb.AppendLine("         , 0 AS Elem_Cod ")
            stb.AppendLine("         , 0 AS Mat_Cod ")
            stb.AppendLine("         , 0 AS Pro_Cod ")
            stb.AppendLine("         , '' AS TipoProdotto ")
            stb.AppendLine("         , '' AS NomeProdotto ")
            stb.AppendLine("         , '' AS Cod_Articolo ")

            If inData.Prodotti Then
                stb.AppendLine("         , ROUND(coalesce(Movimenti_dettagli.Qta_Extra_Totale, 0), +4) AS Qta_Extra_Totale")
                stb.AppendLine("         , coalesce(Movimenti_dettagli.Qta, 0) AS QtaProd")
                stb.AppendLine("         , Movimenti_dettagli.QTA_EXTRA")
                stb.AppendLine("         , Movimenti_dettagli.Udm_Cod As UdmProd")
                stb.AppendLine("         , coalesce(um2.UDM_SIM, '') AS UdmProdSim")
                stb.AppendLine("         , Movimenti_dettagli.Extra_Int UdmExtra")
                stb.AppendLine("         , coalesce(um1.UDM_SIM, '') AS UdmExtraSim")
            Else
                stb.AppendLine("         , 0 AS Qta_Extra_Totale")
                stb.AppendLine("         , 0 AS QtaProd")
                stb.AppendLine("         , 0 As QTA_EXTRA")
                stb.AppendLine("         , 0 As UdmProd")
                stb.AppendLine("         , '' AS UdmProdSim")
                stb.AppendLine("         , 0 UdmExtra")
                stb.AppendLine("         , '' AS UdmExtraSim")
            End If

            stb.AppendLine("         , Agenda.Raccoglitore_Cod ")
            stb.AppendLine("         , ISNULL((utenti.Cognome + ' ' + utenti.Nome + ' ' + utenti.Rag_Soc), 'N.D.') AS Tecnico ")
            stb.AppendLine("         , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and Centri_Aziendali.piva = Agenda.piva ) , '') AS sa_nome ")
            stb.AppendLine("         , i.rag_soc + ' (' + i.PIVA + ')' as rag_soc ")
            stb.AppendLine("         , Operazioni.lav_des ")
            stb.AppendLine("         , GruppoOperazioni.gru_Des ")
            stb.AppendLine("         , ISNULL(GruppoOperazioni.tipo, 'C') as tipo ")
            stb.AppendLine("         , Agenda.Data_Modifica AS Data_Ultima_Modifica_Intervento ")
            stb.AppendLine("         , ap.Piva AS PIVA_Padre, ap.rag_soc + ' (' + ap.PIVA + ')' AS Azienda_Padre")

            stb.AppendLine(" FROM Imprese i")

            'finestra temporale
            stb.AppendLine(" LEFT JOIN Utenti u on u.[USER] = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ")

            'Agenda e movimenti
            stb.AppendLine(" INNER JOIN Agenda ON Agenda.Piva = i.Piva ")
            stb.AppendLine(" INNER JOIN Movimenti ON Movimenti.Piva = Agenda.piva AND ")
            stb.AppendLine("                         Movimenti.Id_Agenda = Agenda.Id_Agenda    ")

            If inData.Prodotti Then
                stb.AppendLine(" LEFT  JOIN Movimenti_dettagli ON Movimenti_dettagli.Piva = Movimenti.piva AND ")
                stb.AppendLine("                                  Movimenti_dettagli.id_agenda = Movimenti.id_Agenda and")
                stb.AppendLine("                                  Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")
                ' Prodotti
                stb.AppendLine(" LEFT JOIN Formulati     ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod  ")
                stb.AppendLine(" LEFT JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
                stb.AppendLine(" LEFT JOIN Trappole      ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD  ")
                stb.AppendLine(" LEFT JOIN InsettiUtili  ON Movimenti_dettagli.Pro_Cod = InsettiUtili.ins_cod  ")
                stb.AppendLine(" LEFT JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  ")
                stb.AppendLine("                            Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
                stb.AppendLine(" LEFT JOIN CategorieMagazzino ON CategorieMagazzino.Elem_Cod = Movimenti_Dettagli.Elem_Cod ")
                ' UdM 
                stb.AppendLine(" LEFT JOIN UnitaMisura um1 on Movimenti_dettagli.Extra_Int = um1.UDM_COD")
                stb.AppendLine(" LEFT JOIN UnitaMisura um2 on Movimenti_dettagli.Udm_Cod = um2.UDM_COD")
            End If
            ' Utenti
            stb.AppendLine(" LEFT JOIN utenti_CTE utenti ON utenti.CodFisc = Agenda.Username_Creazione  ")
            If inData.Impianti Then

                If inData.Prodotti Then
                    ' Destinazioni
                    stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                    stb.AppendLine("                            Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    stb.AppendLine(" 						      Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND ")
                    stb.AppendLine(" 							  Movimenti_dettagli.Id_Mov_det = Mov_Destinazioni.Id_Mov_det  ")
                Else
                    stb.AppendLine(" LEFT JOIN Mov_Destinazioni ON Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND ")
                    stb.AppendLine("                            Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
                    stb.AppendLine(" 						      Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
                End If

                ' Impianti
                stb.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND ")
                stb.AppendLine("                           Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND ")
                stb.AppendLine(" 					      Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND")
                stb.AppendLine(" 						  Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione  ")

                ' Appezzamenti
                stb.AppendLine(" LEFT JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND ")
                stb.AppendLine("                           Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
                stb.AppendLine(" 						  Appezzamento.APPEZZA = Reg_Impianti.APPEZZA   ")
                ' Esercizi
                stb.AppendLine(" LEFT JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND ")
                stb.AppendLine("                               Imprese_Progetti.Sa_Cod = Mov_Destinazioni.SA_COD AND ")
                stb.AppendLine(" 							  Imprese_Progetti.Appezza = Mov_Destinazioni.APPEZZA AND ")
                stb.AppendLine(" 							  Imprese_Progetti.Id_Reg = Mov_Destinazioni.id_destinazione ")
                ' Varieta
                stb.AppendLine(" LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")
                ' Specie
                stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
                ' Campi 
                stb.AppendLine(" LEFT JOIN Campi On Campi.Piva = Appezzamento.Piva And ")
                stb.AppendLine("                    Campi.Sa_Cod = Appezzamento.Sa_Cod And ")
                stb.AppendLine(" 		           Campi.Campo_Cod = Appezzamento.Campo_Cod ")
                ' ImpiantiCodici
                stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici On Reg_Impianti.piva = Reg_Impianti_Codici.piva And ")
                stb.AppendLine("                                  Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod And")
                stb.AppendLine(" 								 Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And ")
                stb.AppendLine(" 								 Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg And ")
                stb.AppendLine(" 								 Reg_Impianti.Cul_Cod=0 And ")
                stb.AppendLine(" 								 Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
                ' CodiciAnagrafe
                stb.AppendLine(" LEFT JOIN Codici_Anagrafe On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
                stb.AppendLine(" LEFT JOIN Utenti_CTE utentiImp on utentiImp.CodFisc = Reg_Impianti.Username_Creazione ")
                ' Poligoni
                stb.AppendLine(" LEFT JOIN GIS_Entita e on e.Piva = Reg_Impianti.PIVA ")
                stb.AppendLine("                       and e.Sa_Cod = Reg_Impianti.SA_COD ")
                stb.AppendLine("                       and e.Appezza = Reg_Impianti.APPEZZA ")
                stb.AppendLine("                       and e.Id_Imp = Reg_Impianti.ID_REG ")
                stb.AppendLine("                       and e.TipoEntita_Cod In (19,20,21,22,23) ")
                stb.AppendLine(" LEFT JOIN GIS_ElementiGrafici g on g.Entita_Cod = e.Entita_Cod ")
            End If
            ' ImpresexIndirizzi
            stb.AppendLine(" LEFT JOIN indirizzi_imprese_CTE ON indirizzi_imprese_CTE.PIVA = i.PIVA ")
            ' Indirizzi
            stb.AppendLine(" LEFT JOIN Indirizzi ON Indirizzi.cod_indirizzo = indirizzi_imprese_CTE.cod_Indirizzo ")

            stb.AppendLine(" LEFT JOIN LOCALITA_CTE on LOCALITA_CTE.Codice = Indirizzi.stato AND LOCALITA_CTE.PROV = Indirizzi.pro_cod_istat AND LOCALITA_CTE.COM = Indirizzi.com_cod_istat ")

            ' Operazioni
            stb.AppendLine(" LEFT JOIN dbo.Operazioni On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            ' GruppoOperazioni
            stb.AppendLine(" LEFT JOIN dbo.GruppoOperazioni On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            stb.AppendLine(" LEFT JOIN (select i.PIVA, i.rag_soc, gi.figlio from GerarchiaImprese gi join imprese i on i.piva = gi.padre) ap on ap.figlio = Agenda.PIVA ")

            stb.AppendLine(" LEFT JOIN Utenti_CTE utentiAzienda on utentiAzienda.CodFisc = i.Username_Creazione ")
            stb.AppendLine(" LEFT JOIN Utenti_CTE utentiOperazioni on utentiOperazioni.CodFisc = Movimenti.Username_Creazione ")

            ' FILTRI CTE
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" JOIN PIVE_CTE pcte on pcte.Piva = i.piva ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" INNER JOIN PIVE_REFERENTI_CTE prcte on prcte.Piva = ap.piva")
            End If

            If filtro_visiblita_utente Then
                'visibilità imprese
                stb.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND u.[USER] = Utenti_Visibilita_Appoggio.Username ")
            End If

            stb.AppendLine(" ")
            ' FILTRI
            stb.AppendLine(" WHERE 1=1")
            'stb.AppendLine("  AND Agenda.Piva in ('01406660389','01311570384','01745310381','01156200386')")
            'stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ") 'todo check
            stb.AppendLine(" AND CONVERT(DateTime, GETDATE(), 120) BETWEEN u.FinestraTemp_Inizio AND u.FinestraTemp_Fine ") 'finestra temporale
            If filtroOperazioni <> "" Then
                stb.AppendLine(" AND " & filtroOperazioni & " ") 'visibilità operazioni
            End If
            If filtroLavorazioni <> "" Then
                stb.AppendLine(" AND " & filtroLavorazioni & " ") 'visibilità lavorazioni
            End If
            'stb.AppendLine("  --AND (  GruppoOperazioni.TIPO = 'C'  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 )  OR  ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 )  ) ")
            stb.AppendLine("  AND (Agenda.Lav_Cod = 5007 )  ")
            'stb.AppendLine("  AND Movimenti.Cau_Mov IN ('2050','2100','2200','2300') ")
            If inData.Data_inizio_operazione > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(inData.Data_inizio_operazione) & " ")
            End If
            If inData.Data_fine_operazione < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(inData.Data_fine_operazione) & " ")
            End If
            ' Finestra temporale
            stb.AppendLine(" AND Movimenti.Validita_Inizio <= u.FinestraTemp_Fine ")
            stb.AppendLine(" AND Movimenti.Validita_Fine >= u.FinestraTemp_Inizio ")
            If inData.Data_inizio_impianto > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= " & Agro_SQL_SaveDate(inData.Data_inizio_impianto))
            End If
            If inData.Data_fine_impianto < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione <= " & Agro_SQL_SaveDate(inData.Data_fine_impianto))
            End If
            If inData.Periodo_da_data_Impianto > -1 Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= CONVERT(DateTime, GETDATE() - " + inData.Periodo_da_data_Impianto.ToString + ", 120) ")
            End If
            If inData.Periodo_da_data_operazione > -1 Then
                stb.AppendLine(" And Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Date.Now.AddDays(-inData.Periodo_da_data_operazione)) & " ")
                stb.AppendLine("  AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(Date.Now) & " ")
            End If
            If inData.Impianti Then
                stb.AppendLine("  AND (Movimenti.Data_Movimento >= Imprese_progetti.validita_inizio OR Imprese_progetti.validita_inizio IS NULL)")
                stb.AppendLine("  AND (Movimenti.Data_Movimento <= Imprese_progetti.validita_fine OR Imprese_progetti.validita_fine IS NULL) ")
            End If

            If inData.Nazioni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.Codice IN " & Agro_SQL_Save_Clausola_IN(filtroNazioni, True) & " ")
            End If

            If inData.Regioni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.REG IN " & Agro_SQL_Save_Clausola_IN(filtroRegioni, True) & " ")
            End If

            If inData.Province.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.PROV IN " & Agro_SQL_Save_Clausola_IN(filtroProvince, True) & " ")
            End If

            If inData.Comuni.Length > 0 Then
                stb.AppendLine(" And LOCALITA_CTE.COM IN " & Agro_SQL_Save_Clausola_IN(filtroComuni, True) & " ")
            End If

            stb.AppendLine(" ) ")
            stb.AppendLine(" ")
            stb.AppendLine(" select *, ")
            stb.AppendLine(" sum(case when Id_Mov is null then 0 else 1 end) num_operazioni, ")

            If inData.Impianti Then

                stb.AppendLine("  COUNT( DISTINCT App_Nome ) num_imp, ")
                stb.AppendLine("  COUNT( DISTINCT ElementoGrafico_Cod ) num_pol, ")
                stb.AppendLine("  COUNT( DISTINCT App_Nome ) - COUNT( DISTINCT ElementoGrafico_Cod ) num_pol_manc  ")

            Else

                stb.AppendLine(" 0 as num_imp, 0 as num_pol, 0 as num_pol_manc ")

            End If

            stb.AppendLine(" from colture_CTE")

            stb.AppendLine(" group by Piva, Sa_Cod, Id_Agenda ,id_Mov, Id_Mov_Det , Lav_Cod, Des_Lib ")
            stb.AppendLine("         , Data_Movimento, Ora, Anno_Movimento, Mese_Movimento, Mov_Desc ")
            stb.AppendLine("         , Username_Creazione, Data_Creazione, Cau_Mov ")
            stb.AppendLine("         , Cul_Cod , colture_CTE.Veg_Cod , colture_CTE.Veg_Des , Raccoglitore_Cod , Tecnico ")
            stb.AppendLine("         , Tipo_Destinazione , Appezza , App_Nome , Elem_Cod ")
            stb.AppendLine("         , Mat_Cod , Pro_Cod, campo_des ")

            If inData.Impianti Then
                stb.AppendLine(" , ElementoGrafico_Cod ")
            End If

            stb.AppendLine("         , Cod_Articolo, TipoProdotto, NomeProdotto ")
            stb.AppendLine("         , IdImpianto, sa_nome, rag_soc, lav_des, gru_Des, tipo, cul_des")
            stb.AppendLine("         , LottoImpianto, SupApp, QtaImp, SupTrattata")
            stb.AppendLine("         , DestinazioneTerreniNudi_Cod , DestinazioneTerreniNudi_Des")
            stb.AppendLine("         , Data_Ultima_Modifica_Intervento , validita_inizio_destinazione")
            stb.AppendLine("         , Qta_Extra_Totale, QtaProd, QTA_EXTRA, UdmProd")
            stb.AppendLine("         ,  UdmProdSim, UdmExtra,  UdmExtraSim")
            stb.AppendLine("         , PIVA_Padre, Azienda_Padre")
            stb.AppendLine("         , STATO, REGIONE, PROVINCIA, LOCALITA ")
            stb.AppendLine("         , Utente_Creazione_Op, Anno_Creazione_Op, Data_Creazione_Op, Mese_Creazione_Op ")
            stb.AppendLine("         , Utente_Creazione_Imp, Data_Creazione_Imp, Anno_Creazione_Imp, Mese_Creazione_Imp ")
            stb.AppendLine("         , Utente_Creazione_Azienda, Data_Creazione_Azienda, Anno_Creazione_Azienda, Mese_Creazione_Azienda ")

            stb.AppendLine(" HAVING sum(case when Id_Agenda is null then 0 else 1 end) > 0")

            'stb.AppendLine(" ORDER BY ")
            'stb.AppendLine(" colture_CTE.Data_Movimento DESC,")
            'stb.AppendLine(" colture_CTE.Id_Agenda DESC,")
            'stb.AppendLine(" App_Nome ASC ")
            'stb.AppendLine(" ;")

            stb.AppendLine(" ")
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED;")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objp_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objp_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Estrazione_Impianti(ByVal inData As LeggiAgendaStatistiche,
                                        ByRef objp_Server As AgronicaCoreParametri,
                                        ByRef objp_Utenti As AgronicaCoreParametri,
                                        Optional ByVal pivaList As String = "",
                                        Optional ByVal specieCodList As String = "",
                                        Optional ByVal filtroOperazioni As String = "",
                                        Optional ByVal filtroLavorazioni As String = "",
                                        Optional ByVal referentiList As String = "",
                                        Optional ByVal filtroNazioni As String = "",
                                        Optional ByVal filtroRegioni As String = "",
                                        Optional ByVal filtroProvince As String = "",
                                        Optional ByVal filtroComuni As String = "",
                                        Optional ByVal filtro_visiblita_utente As Boolean = True) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgendaStatistiche_R.Estrazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")

            stb.AppendLine(" with ")
            If inData.Piva_aziende.Length > 0 Then
                stb.AppendLine(" PIVE_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(pivaList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Aziende_referenti.Length > 0 Then
                stb.AppendLine(" PIVE_REFERENTI_CTE as (")
                stb.AppendLine(" select DISTINCT PIVA")
                stb.AppendLine(" from Imprese")
                stb.AppendLine(" WHERE PIVA IN " & Agro_SQL_Save_Clausola_IN(referentiList, True) & " ")
                stb.AppendLine(" ), ")
            End If

            If inData.Specie_vegetale.Length > 0 Then
                stb.AppendLine(" SPECIE_CTE as (")
                stb.AppendLine(" select Veg_Cod, Veg_Des")
                stb.AppendLine(" from SpecieVegetali")
                stb.AppendLine(" WHERE Veg_Cod IN " & Agro_SQL_Save_Clausola_IN(specieCodList) & " ")
                stb.AppendLine(" ), ")
            End If

            stb.AppendLine(" LOCALITA_CTE as (")
            stb.AppendLine(" Select ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice, ")
            stb.AppendLine(" ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, ")
            stb.AppendLine(" Lista_Regioni.REG, ")
            stb.AppendLine(" Lista_Regioni.Regione_Des, ")
            stb.AppendLine(" Lista_Province.PROVINCIA, ")
            stb.AppendLine(" ISTAT.PROV, ISTAT.COM, ")
            stb.AppendLine(" ISTAT.LOCALITA ")
            stb.AppendLine(" FROM ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            stb.AppendLine(" JOIN Lista_Regioni on Lista_Regioni.Stato_Country = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ")
            stb.AppendLine(" JOIN Lista_Province on Lista_Province.REG = Lista_Regioni.REG ")
            stb.AppendLine(" JOIN ISTAT on ISTAT.PROV = Lista_Province.PROV ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" indirizzi_imprese_CTE as ( ")
            stb.AppendLine("select Piva, MAX(cod_indirizzo) As Cod_Indirizzo from ImpresexIndirizzi ")
            stb.AppendLine("group by Piva ")
            stb.AppendLine("), ")

            stb.AppendLine(" utenti_CTE as (")
            stb.AppendLine(" select cognome, nome, rag_soc, codfisc")
            stb.AppendLine(" from " + objp_Utenti.Recupera_NomeDB + ".dbo.Utenti_Dettagli")
            stb.AppendLine(" ), ")

            If inData.Estrazione = 0 OrElse inData.Estrazione = 3 Then
                stb.AppendLine(Estrazione_Impianti_CTE(inData, objp_Server, filtroNazioni, filtroRegioni, filtroProvince,
                                                       filtroComuni, filtro_visiblita_utente, True))
            End If

            If inData.Estrazione = 0 Then
                stb.Append(",")
            End If

            If inData.Estrazione = 0 OrElse inData.Estrazione = 4 Then
                stb.AppendLine(Estrazione_Impianti_CTE(inData, objp_Server, filtroNazioni, filtroRegioni, filtroProvince,
                                                       filtroComuni, filtro_visiblita_utente, False))
            End If

            stb.AppendLine(" ")

            If inData.Estrazione = 0 OrElse inData.Estrazione = 3 Then
                stb.AppendLine(Estrazione_Impianti_Select(True))
            End If

            If inData.Estrazione = 0 Then
                stb.Append("UNION")
            End If

            If inData.Estrazione = 0 OrElse inData.Estrazione = 4 Then
                stb.AppendLine(Estrazione_Impianti_Select(False))
            End If

            stb.AppendLine(" ")
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED;")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objp_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objp_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function Estrazione_Impianti_CTE(ByVal inData As LeggiAgendaStatistiche,
                                             ByRef objp_Server As AgronicaCoreParametri,
                                             ByVal filtroNazioni As String,
                                             ByVal filtroRegioni As String,
                                             ByVal filtroProvince As String,
                                             ByVal filtroComuni As String,
                                             ByVal filtro_visiblita_utente As Boolean,
                                             ByVal soloSenza As Boolean) As String

        Dim stb As New StringBuilder
        Dim nomeCte As String = If(soloSenza, "colture_CTE_senza", "colture_CTE_con")

        stb.AppendLine(" " + nomeCte + " as ( ")
        stb.AppendLine(" SELECT    i.Piva, Reg_Impianti.Sa_Cod, 0 AS Id_Agenda ")
        stb.AppendLine("         , 0 AS id_Mov, 0 AS Id_Mov_Det ")
        stb.AppendLine("         , 0 AS Lav_Cod, '' AS Des_Lib ")
        stb.AppendLine("         , CAST(null As DateTime) AS Data_Movimento, CAST(null as DATETIME) AS Ora, 0 AS Anno_Movimento ")
        stb.AppendLine("         , 0 AS Mese_Movimento, '' AS Mov_Desc ")
        stb.AppendLine("         , i.Username_Creazione, i.Data_Creazione, '' AS Cau_Mov ")
        stb.AppendLine("         , Reg_Impianti.CUL_COD AS Cul_Cod ")
        stb.AppendLine("         , SpecieVegetali.Veg_Cod AS Veg_Cod ")
        stb.AppendLine("         , SpecieVegetali.Veg_Des AS Veg_Des ")
        stb.AppendLine("         , 0 AS Raccoglitore_Cod ")
        stb.AppendLine("         , ISNULL((utenti.Cognome + ' ' + utenti.Nome + ' ' + utenti.Rag_Soc), 'N.D.') AS Tecnico ")
        stb.AppendLine("         , -1 AS Tipo_Destinazione ")
        stb.AppendLine("         , 0 AS Appezza ")
        stb.AppendLine("         , Appezzamento.App_Nome AS App_Nome ")
        stb.AppendLine("         , ISNULL((utentiAzienda.Cognome + ' ' + utentiAzienda.Nome + ' ' + utentiAzienda.Rag_Soc), 'N.D.') As Utente_Creazione_Azienda  ")
        stb.AppendLine("         , i.Data_Creazione As Data_Creazione_Azienda ")
        stb.AppendLine("         , ISNULL(YEAR(i.Data_Creazione), 0) As Anno_Creazione_Azienda ")
        stb.AppendLine("         , ISNULL(MONTH(i.Data_Creazione), 0) As Mese_Creazione_Azienda")
        stb.AppendLine("         , '' As Utente_Creazione_Op ")
        stb.AppendLine("         , CAST(null As DateTime) As Data_Creazione_Op ")
        stb.AppendLine("         , 0 As Anno_Creazione_Op ")
        stb.AppendLine("         , 0 As Mese_Creazione_Op ")
        stb.AppendLine("         , ISNULL((utentiImp.Cognome + ' ' + utentiImp.Nome + ' ' + utentiImp.Rag_Soc), 'N.D.') As Utente_Creazione_Imp ")
        stb.AppendLine("         , Reg_Impianti.Data_Creazione As Data_Creazione_Imp ")
        stb.AppendLine("         , ISNULL(YEAR(Reg_Impianti.Data_Creazione), 0) As Anno_Creazione_Imp ")
        stb.AppendLine("         , ISNULL( MONTH(Reg_Impianti.Data_Creazione), 0) As Mese_Creazione_Imp")
        stb.AppendLine("         , 0 AS Elem_Cod ")
        stb.AppendLine("         , 0 AS Mat_Cod ")
        stb.AppendLine("         , 0 AS Pro_Cod ")
        stb.AppendLine("         , '' as TipoProdotto ")
        stb.AppendLine("         , '' as NomeProdotto ")
        stb.AppendLine("         , '' AS Cod_Articolo ")
        stb.AppendLine("         , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Reg_Impianti.Sa_Cod and Centri_Aziendali.piva = Reg_Impianti.piva ) , '') AS sa_nome ")
        stb.AppendLine("         , i.rag_soc + ' (' + i.PIVA + ')' as rag_soc ")
        stb.AppendLine("         , '' AS lav_des ")
        stb.AppendLine("         , '' AS gru_Des ")
        stb.AppendLine("         , '' as tipo ")
        stb.AppendLine("         , Cultivar.CUL_DES AS cul_des ")
        stb.AppendLine("         , '' AS campo_des ")
        stb.AppendLine("         , Reg_Impianti.ID_REG AS IdImpianto")
        stb.AppendLine("         , Reg_Impianti.Sup_Imp AS SupApp")
        stb.AppendLine(" 		   , CAST(0 AS float) AS QtaImp")
        stb.AppendLine("         , CAST(0 AS float) AS SupTrattata ")
        stb.AppendLine("         , '' AS LottoImpianto ")
        stb.AppendLine("         , 0 AS DestinazioneTerreniNudi_Cod ")
        stb.AppendLine("         , '' AS DestinazioneTerreniNudi_Des ")
        stb.AppendLine("         , CAST(null As DateTime) AS Data_Ultima_Modifica_Intervento ")
        stb.AppendLine("         , CONVERT(DateTime,'1900/01/01',120) AS validita_inizio_destinazione ")
        stb.AppendLine("         , 0 AS Qta_Extra_Totale")
        stb.AppendLine("         , 0 AS QtaProd")
        stb.AppendLine("         , 0 AS QTA_EXTRA")
        stb.AppendLine("         , 0 As UdmProd")
        stb.AppendLine("         , '' AS UdmProdSim")
        stb.AppendLine("         , 0 AS UdmExtra")
        stb.AppendLine("         , '' AS UdmExtraSim")
        stb.AppendLine("         , ap.Piva AS PIVA_Padre, ap.rag_soc + ' (' + ap.PIVA + ')' AS Azienda_Padre")
        stb.AppendLine("         , LOCALITA_CTE.Descrizione as STATO, LOCALITA_CTE.Regione_Des AS REGIONE, LOCALITA_CTE.PROVINCIA, LOCALITA_CTE.LOCALITA")
        stb.AppendLine("         , g.ElementoGrafico_Cod ")

        ' Imprese
        stb.AppendLine(" FROM imprese i ")

        ' Finestra temporale
        stb.AppendLine(" LEFT JOIN Utenti u on u.[USER] = " & Agro_SQL_SaveText_NULL(objp_Server.UtenteUsername) & "  ")

        ' Impianti
        If Not soloSenza Then 'Solo aziende con impianti
            stb.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = i.PIVA ")
        Else
            stb.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti.PIVA = i.PIVA ")
            If inData.Data_inizio_impianto > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= " & Agro_SQL_SaveDate(inData.Data_inizio_impianto))
            End If
            If inData.Data_fine_impianto < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione <= " & Agro_SQL_SaveDate(inData.Data_fine_impianto))
            End If
            If inData.Periodo_da_data_Impianto > -1 Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= CONVERT(DateTime, GETDATE() - " + inData.Periodo_da_data_Impianto.ToString + ", 120) ")
            End If
        End If

        ' Utenti
        stb.AppendLine(" LEFT JOIN utenti_CTE utenti ON utenti.CodFisc = i.Username_Creazione ")

        ' Appezzamenti
        stb.AppendLine(" LEFT JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND ")
        stb.AppendLine("                           Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        stb.AppendLine("                           Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")

        ' ImpresexIndirizzi
        stb.AppendLine(" LEFT JOIN indirizzi_imprese_CTE ON indirizzi_imprese_CTE.PIVA = i.PIVA ")

        ' Indirizzi
        stb.AppendLine(" LEFT JOIN Indirizzi ON Indirizzi.cod_indirizzo = indirizzi_imprese_CTE.cod_Indirizzo ")

        ' Varieta
        stb.AppendLine(" LEFT JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

        ' Specie
        stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

        ' Campi 
        stb.AppendLine(" LEFT JOIN Campi On Campi.Piva = Appezzamento.Piva And ")
        stb.AppendLine("                    Campi.Sa_Cod = Appezzamento.Sa_Cod And ")
        stb.AppendLine("                    Campi.Campo_Cod = Appezzamento.Campo_Cod ")

        ' ImpiantiCodici
        stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici On Reg_Impianti.piva = Reg_Impianti_Codici.piva And ")
        stb.AppendLine("                                  Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod And")
        stb.AppendLine("                                  Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And ")
        stb.AppendLine("                                  Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg And ")
        stb.AppendLine("                                  Reg_Impianti.Cul_Cod=0 And ")
        stb.AppendLine("                                  Reg_Impianti_Codici.id_cod BETWEEN 3000 And 3999 ")
        ' CodiciAnagrafe
        stb.AppendLine(" LEFT JOIN Codici_Anagrafe On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
        stb.AppendLine(" LEFT JOIN (select i.PIVA, i.rag_soc, gi.figlio from GerarchiaImprese gi join imprese i on i.piva = gi.padre) ap on ap.figlio = i.PIVA ")

        ' FILTRI CTE
        If inData.Piva_aziende.Length > 0 Then
            stb.AppendLine(" JOIN PIVE_CTE pcte on pcte.Piva = i.piva ")
        End If

        If inData.Aziende_referenti.Length > 0 Then
            stb.AppendLine(" INNER JOIN PIVE_REFERENTI_CTE prcte on prcte.Piva = ap.piva")
        End If

        stb.AppendLine(" LEFT JOIN LOCALITA_CTE on LOCALITA_CTE.Codice = Indirizzi.stato AND LOCALITA_CTE.PROV = Indirizzi.pro_cod_istat AND LOCALITA_CTE.COM = Indirizzi.com_cod_istat ")

        If Not soloSenza Then
            If filtro_visiblita_utente Then
                ' Visibilità imprese
                stb.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio On i.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND u.[USER] = Utenti_Visibilita_Appoggio.Username ")
            End If
        End If

        ' Poligoni
        stb.AppendLine(" LEFT JOIN GIS_Entita e on e.Piva = Reg_Impianti.PIVA ")
        stb.AppendLine("                       and e.Sa_Cod = Reg_Impianti.SA_COD ")
        stb.AppendLine("                       and e.Appezza = Reg_Impianti.APPEZZA ")
        stb.AppendLine("                       and e.Id_Imp = Reg_Impianti.ID_REG ")
        stb.AppendLine("                       and e.TipoEntita_Cod In (19,20,21,22,23) ")
        stb.AppendLine(" LEFT JOIN GIS_ElementiGrafici g on g.Entita_Cod = e.Entita_Cod ")
        stb.AppendLine(" LEFT JOIN Utenti_CTE utentiImp on utentiImp.CodFisc = Reg_Impianti.Username_Creazione ")
        stb.AppendLine(" LEFT JOIN Utenti_CTE utentiAzienda on utentiAzienda.CodFisc = i.Username_Creazione ")

        stb.AppendLine(" ")

        ' FILTRI
        stb.AppendLine(" WHERE 1=1")
        If Not soloSenza Then 'Solo aziende con impianti
            If inData.Data_inizio_impianto > CostantiPersonalizzate.AGRODATAINIZIO Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= " & Agro_SQL_SaveDate(inData.Data_inizio_impianto))
            End If
            If inData.Data_fine_impianto < CostantiPersonalizzate.AGRODATAFINE Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione <= " & Agro_SQL_SaveDate(inData.Data_fine_impianto))
            End If
            If inData.Periodo_da_data_Impianto > -1 Then
                stb.AppendLine(" And Reg_Impianti.Data_Creazione >= CONVERT(DateTime, GETDATE() - " + inData.Periodo_da_data_Impianto.ToString + ", 120) ")
            End If
            ' Finestra temporale
            stb.AppendLine(" AND Reg_Impianti.Validita_Inizio <= u.FinestraTemp_Fine ")
            stb.AppendLine(" AND Reg_Impianti.Validita_Fine >= u.FinestraTemp_Inizio ")
        End If

        If inData.Specie_vegetale.Length > 0 Then
            stb.AppendLine(" And SpecieVegetali.Veg_Cod in (select veg_cod from SPECIE_CTE) ")
        End If

        If inData.Nazioni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.Codice IN " & Agro_SQL_Save_Clausola_IN(filtroNazioni, True) & " ")
        End If

        If inData.Regioni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.REG IN " & Agro_SQL_Save_Clausola_IN(filtroRegioni, True) & " ")
        End If

        If inData.Province.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.PROV IN " & Agro_SQL_Save_Clausola_IN(filtroProvince, True) & " ")
        End If

        If inData.Comuni.Length > 0 Then
            stb.AppendLine(" And LOCALITA_CTE.COM IN " & Agro_SQL_Save_Clausola_IN(filtroComuni, True) & " ")
        End If

        stb.AppendLine(" ) ")

        Return stb.ToString

    End Function

    Private Function Estrazione_Impianti_Select(ByVal soloSenza As Boolean) As String
        Dim stb As New StringBuilder
        Dim nomeCte As String = If(soloSenza, "colture_CTE_senza", "colture_CTE_con")

        stb.AppendLine(" select DISTINCT *,  ")
        stb.AppendLine(" sum(case when App_Nome is null then 0 else 1 end) num_imp, ")
        stb.AppendLine(" sum(case when Id_Mov is null then 0 else 1 end) num_operazioni, ")
        stb.AppendLine(" sum(case when ElementoGrafico_Cod is null then 0 else 1 end) num_pol, ")
        stb.AppendLine(" sum(case when App_Nome is null then 0 else 1 end - case when ElementoGrafico_Cod is null then 0 else 1 end) num_pol_manc ")
        stb.AppendLine(" from " + nomeCte + " ")

        stb.AppendLine(" group by Piva, Sa_Cod, Id_Agenda ,id_Mov, Id_Mov_Det , Lav_Cod, Des_Lib ")
        stb.AppendLine("         , Data_Movimento, Ora, Anno_Movimento, Mese_Movimento, Mov_Desc ")
        stb.AppendLine("         , Username_Creazione, Data_Creazione, Cau_Mov ")
        stb.AppendLine("         , Cul_Cod , Veg_Cod , Veg_Des , Raccoglitore_Cod , Tecnico ")
        stb.AppendLine("         , Tipo_Destinazione , Appezza , App_Nome , Elem_Cod ")
        stb.AppendLine("         , Mat_Cod , Pro_Cod  ")
        stb.AppendLine("         , TipoProdotto, NomeProdotto, Cod_Articolo ")
        stb.AppendLine("         , sa_nome , rag_soc , lav_des , gru_Des ")
        stb.AppendLine("         , tipo , cul_des , campo_des , IdImpianto")
        stb.AppendLine("         , SupApp, QtaImp, SupTrattata , LottoImpianto")
        stb.AppendLine("         , DestinazioneTerreniNudi_Cod , DestinazioneTerreniNudi_Des")
        stb.AppendLine("         , Data_Ultima_Modifica_Intervento , validita_inizio_destinazione")
        stb.AppendLine("         , Qta_Extra_Totale, QtaProd, QTA_EXTRA, UdmProd")
        stb.AppendLine("         ,  UdmProdSim, UdmExtra,  UdmExtraSim")
        stb.AppendLine("         , PIVA_Padre, Azienda_Padre")
        stb.AppendLine("         , STATO, REGIONE, PROVINCIA, LOCALITA ")
        stb.AppendLine("         , Utente_Creazione_Op, Data_Creazione_Op, Anno_Creazione_Op, Mese_Creazione_Op ")
        stb.AppendLine("         , Utente_Creazione_Imp, Data_Creazione_Imp, Anno_Creazione_Imp, Mese_Creazione_Imp ")
        stb.AppendLine("         , Utente_Creazione_Azienda, Data_Creazione_Azienda, Anno_Creazione_Azienda, Mese_Creazione_Azienda, ElementoGrafico_Cod ")

        If soloSenza Then 'Solo aziende senza impianti
            stb.AppendLine(" HAVING sum(case when App_Nome is null then 0 else 1 end) = 0")
        Else 'Solo aziende con impianti
            stb.AppendLine(" HAVING sum(case when App_Nome is null then 0 else 1 end) > 0")
        End If

        Return stb.ToString
    End Function

End Class
