Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Sementieri_Sportello_InterferenzePerConferma_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDaCache(ByVal DatiSerializzati As String) As DataRow

        Dim vDati As String() = DatiSerializzati.Split("§")
        Dim colonne() As String = vDati(0).Split("|")
        ' VAnni: 28/3/2018: Se due impianti sono in interferenza e viene quindi memorizzato il record in [Sementieri_Sportello_InterferenzePerConferma] ed entrambi vengono eliminati allora viene scritto un record sporco nella colonna Descrizione_casella_conflitto
        ' in particolare il record contiene due volte l'intestazione.. in prima battuta seleziono i dati in base al count dello split.
        Dim riga() As String = vDati(vDati.Count - 1).Split("|")

        Dim dt As New DataTable
        Dim dr As DataRow = dt.NewRow
        Dim col As Integer = 0
        While col < colonne.Length

            dt.Columns.Add(colonne(col))

            If col < riga.Length Then
                dr.Item(colonne(col)) = riga(col)
            Else
                dr.Item(colonne(col)) = ""
            End If

            col += 1
        End While

        Return dr
    End Function

    Public Function Leggi(ByVal Entita_Cod As Integer,
                          ByVal Solo_Attivi As Boolean,
                          ByVal Codice_Fiscale_Tecnico As String,
                          ByVal SoloProprietario As Boolean,
                          ByVal id_specie As Integer,
                          ByVal Accoda_Mail As Boolean,
                          ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Leggi()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try

            Dim nomeDB_Utenti = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.AppendLine("SELECT DISTINCT ")
            stb.AppendLine("    ISNULL(Descrizione_Casella_Conflitto, '') AS Descrizione_Casella_Conflitto ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Interferenze_cod ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Flag_Attivo ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Stato_cod ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Data_Conferma ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Entita_Cod_Propietario ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Entita_Cod_Interferente ")
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.DistanzaEffettiva ")
            stb.AppendLine("    , Interferente_Reg_Impianti.CODICE_FISCALE_TECNICO AS Interferente_Reg_Impianti_CODICE_FISCALE_TECNICO ")
            stb.AppendLine("    , Propietario_Reg_Impianti.CODICE_FISCALE_TECNICO AS Propietario_Reg_Impianti_CODICE_FISCALE_TECNICO ")
            stb.AppendLine("    , Interferente_Reg_Impianti.SA_COD AS I_Reg_Impianti_Sa_Cod ")
            stb.AppendLine("    , Interferente_Reg_Impianti.PIVA AS I_Reg_Impianti_Piva ")
            stb.AppendLine("    , Propietario_Reg_Impianti.SA_COD AS P_Reg_Impianti_Sa_Cod ")
            stb.AppendLine("    , Propietario_Reg_Impianti.PIVA AS P_Reg_Impianti_Piva ")
            stb.AppendLine()
            stb.AppendLine("    , Propietario_Appezzamento.Via_Stringa AS P_Via_Stringa ")
            stb.AppendLine("    , Interferente_Appezzamento.Via_Stringa AS I_Via_Stringa ")
            stb.AppendLine()
            stb.AppendLine("	, Propietario_Specie.Veg_Des AS P_Veg_Des ")
            stb.AppendLine("	, Propietario_grva.Grva_Des AS P_Grva_Des ")
            stb.AppendLine("	, Interferente_Specie.Veg_Des AS I_Veg_Des ")
            stb.AppendLine("	, Interferente_grva.Grva_Des AS I_Grva_Des ")
            stb.AppendLine()
            stb.AppendLine("    , ISNULL(Propietario_GE.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) AS P_Lat ")
            stb.AppendLine("    , ISNULL(Propietario_GE.Poligono_GeoEntity.EnvelopeCenter().Long, 0) AS P_Lng ")
            stb.AppendLine("    , ISNULL(Interferente_GE.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) AS I_Lat ")
            stb.AppendLine("    , ISNULL(Interferente_GE.Poligono_GeoEntity.EnvelopeCenter().Long, 0) AS I_Lng ")
            stb.AppendLine()
            stb.AppendLine("	, ISNULL(Propietario_Layer.Gruppi_Utente_des, '') AS PropietarioLayer ")
            stb.AppendLine("	, ISNULL(Interferente_Layer.Gruppi_Utente_des, '') AS InterferenteLayer ")
            stb.AppendLine()
            stb.AppendLine("    , Propietario_Reg_Impianti.Validita_Inizio AS P_ValiditaInizio ")
            stb.AppendLine("    , Propietario_Reg_Impianti.Validita_Fine AS P_ValiditaFine ")
            stb.AppendLine("    , Interferente_Reg_Impianti.Validita_Inizio AS I_ValiditaInizio ")
            stb.AppendLine("    , Interferente_Reg_Impianti.Validita_Fine AS I_ValiditaFine ")
            stb.AppendLine()
            stb.AppendLine("    , Sementieri_Sportello_InterferenzePerConferma.Data_Creazione AS DataCreazione ")
            stb.AppendLine()
            stb.AppendLine("FROM Sementieri_Sportello_InterferenzePerConferma ")
            stb.AppendLine()
            stb.AppendLine("LEFT JOIN GIS_Entita            Propietario                 ON Sementieri_Sportello_InterferenzePerConferma.PivaSuperUser = Propietario.PivaSuperUser ")
            stb.AppendLine(" 											                AND Sementieri_Sportello_InterferenzePerConferma.Entita_Cod_Propietario = Propietario.Entita_Cod ")
            stb.AppendLine("LEFT JOIN GIS_ElementiGrafici   Propietario_GE              ON Propietario_GE.PivaSuperUser = Propietario.PivaSuperUser ")
            stb.AppendLine("														    AND Propietario_GE.Entita_Cod = Propietario.Entita_Cod ")
            stb.AppendLine("LEFT JOIN Reg_Impianti          Propietario_Reg_Impianti    ON Propietario.Piva = Propietario_Reg_Impianti.PIVA ")
            stb.AppendLine(" 														    AND Propietario.Sa_Cod = Propietario_Reg_Impianti.SA_COD ")
            stb.AppendLine(" 														    AND Propietario.Appezza = Propietario_Reg_Impianti.APPEZZA ")
            stb.AppendLine(" 														    AND Propietario.Id_Imp = Propietario_Reg_Impianti.ID_REG ")
            stb.AppendLine("LEFT JOIN Appezzamento	    	Propietario_Appezzamento	ON Propietario_Appezzamento.Piva = Propietario.PIVA ")
            stb.AppendLine("														    AND Propietario_Appezzamento.Sa_Cod = Propietario.SA_COD ")
            stb.AppendLine("														    AND Propietario_Appezzamento.Appezza = Propietario.APPEZZA ")
            stb.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente Propietario_Layer	 ON Propietario_Reg_Impianti.Codice_Fiscale_Tecnico = Propietario_Layer.Gruppi_Utente_Identificativo ")
            stb.AppendLine("LEFT JOIN Cultivar              P_Cultivar                  ON P_Cultivar.Cul_Cod = Propietario_Reg_Impianti.CUL_COD ")
            stb.AppendLine("LEFT JOIN SpecieVegetali	    Propietario_Specie	    	ON Propietario_Specie.Veg_Cod = P_Cultivar.Veg_Cod ")
            stb.AppendLine("LEFT JOIN GruppoVarietale	    Propietario_grva	    	ON Propietario_grva.grva_cod = ABS(Propietario_Reg_Impianti.GRVA_Cod_VEG) ")
            stb.AppendLine("LEFT JOIN Mappatura_Specie      P_R_I_Mappatura_Specie      ON P_Cultivar.Veg_Cod = P_R_I_Mappatura_Specie.Veg_Cod ")
            stb.AppendLine("                                                            And P_R_I_Mappatura_Specie.Grva_Cod = Propietario_grva.Grva_Cod")
            stb.AppendLine()
            stb.AppendLine("LEFT JOIN GIS_Entita            Interferente                On Sementieri_Sportello_InterferenzePerConferma.PivaSuperUser = interferente.PivaSuperUser ")
            stb.AppendLine(" 											                And Sementieri_Sportello_InterferenzePerConferma.Entita_Cod_Interferente = interferente.Entita_Cod ")
            stb.AppendLine("LEFT JOIN GIS_ElementiGrafici   Interferente_GE			    On Interferente_GE.PivaSuperUser = Interferente.PivaSuperUser ")
            stb.AppendLine("														    And Interferente_GE.Entita_Cod = Interferente.Entita_Cod")
            stb.AppendLine("LEFT JOIN Reg_Impianti          Interferente_Reg_Impianti   On Interferente.Piva = Interferente_Reg_Impianti.PIVA ")
            stb.AppendLine(" 														    And Interferente.Sa_Cod = Interferente_Reg_Impianti.SA_COD ")
            stb.AppendLine(" 														    And Interferente.Appezza = Interferente_Reg_Impianti.APPEZZA ")
            stb.AppendLine(" 														    And Interferente.Id_Imp = Interferente_Reg_Impianti.ID_REG ")
            stb.AppendLine("LEFT JOIN Appezzamento		    Interferente_Appezzamento	On Interferente_Appezzamento.Piva = Interferente.PIVA ")
            stb.AppendLine("														    And Interferente_Appezzamento.Sa_Cod = Interferente.SA_COD ")
            stb.AppendLine("														    And Interferente_Appezzamento.Appezza = Interferente.APPEZZA ")
            stb.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente Interferente_Layer	 ON Interferente_Reg_Impianti.Codice_Fiscale_Tecnico = Interferente_Layer.Gruppi_Utente_Identificativo ")
            stb.AppendLine("LEFT JOIN Cultivar              I_Cultivar                  On I_Cultivar.Cul_Cod = Interferente_Reg_Impianti.CUL_COD ")
            stb.AppendLine("LEFT JOIN SpecieVegetali	    Interferente_Specie			On Interferente_Specie.Veg_Cod = I_Cultivar.Veg_Cod ")
            stb.AppendLine("LEFT JOIN GruppoVarietale	    Interferente_grva			On Interferente_grva.grva_cod = ABS(Interferente_Reg_Impianti.GRVA_Cod_VEG) ")
            stb.AppendLine("LEFT JOIN Mappatura_Specie      I_R_I_Mappatura_Specie      On I_Cultivar.Veg_Cod = I_R_I_Mappatura_Specie.Veg_Cod ")
            stb.AppendLine("                                                            And I_R_I_Mappatura_Specie.Grva_Cod = Interferente_grva.Grva_Cod")
            stb.AppendLine()
            stb.AppendLine("WHERE 1=1 ")

            If Solo_Attivi = True Then
                stb.AppendLine("AND (Flag_Attivo <> 0 Or Flag_Attivo Is NULL) ")
            End If

            If Codice_Fiscale_Tecnico <> "" Then
                If SoloProprietario Then

                    stb.AppendLine("AND Propietario_Reg_Impianti.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' ")
                Else

                    stb.AppendLine("AND (Propietario_Reg_Impianti.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' OR ")
                    stb.AppendLine("Interferente_Reg_Impianti.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' )")
                End If
            End If

            If id_specie <> 0 Then
                stb.AppendLine("AND (I_R_I_Mappatura_Specie.ID_Specie = " & id_specie & " OR I_R_I_Mappatura_Specie.ID_Specie IS NULL) ")
                stb.AppendLine("AND (P_R_I_Mappatura_Specie.ID_Specie = " & id_specie & " OR P_R_I_Mappatura_Specie.ID_Specie IS NULL) ")
            End If

            If Entita_Cod > 0 Then
                stb.AppendLine("AND (Entita_Cod_Propietario = " & Entita_Cod & " OR Entita_Cod_Interferente = " & Entita_Cod & ") ")
            End If

            If Accoda_Mail Then
                stb.AppendLine("AND (Sementieri_Sportello_InterferenzePerConferma.StatoGestione = 0 OR Sementieri_Sportello_InterferenzePerConferma.StatoGestione IS NULL) ")
            End If

            stb.AppendLine()

            stb.AppendLine("ORDER BY Stato_cod, DataCreazione DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function


    Public Function Leggi_da_Propietario_Interferente(ByVal Entita_Cod_Propietario As Integer,
                                                      ByVal Entita_Cod_Interferente As Integer,
                                                      ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.Sportello_LeggiXData()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            stb.Append("SELECT     *  " & vbCrLf)

            stb.Append("FROM       Sementieri_Sportello_InterferenzePerConferma " & vbCrLf)

            stb.Append(" where 1=1 " & vbCrLf)


            If Entita_Cod_Propietario <> 0 Then
                stb.Append(" and Entita_Cod_Propietario =  " & Entita_Cod_Propietario & vbCrLf)
            End If

            If Entita_Cod_Interferente <> 0 Then
                stb.Append(" and Entita_Cod_Interferente =  " & Entita_Cod_Interferente & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function

    Public Function LeggiAttiviPerCodiceFiscaleTecnico(id_specie As Integer, Codice_Fiscale_Tecnico As String, DataInizioSportello As Date, DataFineSportello As Date, Accoda_Mail As Boolean, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.LeggiAttiviPerCodiceFiscaleTecnico()"

        Dim dt As DataTable = Nothing

        If id_specie = 0 Then

            Throw New ArgumentNullException("id_specie")
        End If

        Try
            Dim nomeDB_Utenti = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            Dim Sql As New StringBuilder

            Sql.Clear()

            Sql.AppendLine("DECLARE @CODICE_FISCALE_TECNICO AS VARCHAR(MAX) = '" & Codice_Fiscale_Tecnico & "'")
            Sql.AppendLine("DECLARE @VALIDITA_INIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizioSportello))
            Sql.AppendLine("DECLARE @VALIDITA_FINE DATETIME = " & Agro_SQL_SaveDateTime(DataFineSportello))
            Sql.AppendLine()
            Sql.AppendLine(";WITH mappatura_cte AS ")
            Sql.AppendLine("( ")
            Sql.AppendLine("SELECT DISTINCT Veg_Cod, Grva_Cod FROM Mappatura_Specie WHERE ID_Specie = " & id_specie.ToString)
            Sql.AppendLine("), ")
            Sql.AppendLine()
            Sql.AppendLine("impianti_cte AS ")
            Sql.AppendLine("( ")
            Sql.AppendLine("SELECT ")
            Sql.AppendLine("	entita.PivaSuperUser, entita.Entita_Cod ")
            Sql.AppendLine("	, impi.CODICE_FISCALE_TECNICO ")
            Sql.AppendLine("	, indirizzo = COALESCE(appezza.Via_Stringa, '') ")
            Sql.AppendLine("	, veg.Veg_Des ")
            Sql.AppendLine("	, grvar.Grva_Des ")
            Sql.AppendLine("    , lat = ISNULL(elem.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) ")
            Sql.AppendLine("    , lng = ISNULL(elem.Poligono_GeoEntity.EnvelopeCenter().Long, 0) ")
            Sql.AppendLine("	, layer = COALESCE(layer.Gruppi_Utente_des, '') ")
            Sql.AppendLine("	, ind_des = COALESCE(indir.ind_des, '') ")
            Sql.AppendLine("	, frz_des = COALESCE(indir.frz_des, '') ")
            Sql.AppendLine("	, com_des = COALESCE(ISTAT.LOCALITA, '') ")
            Sql.AppendLine("	, pr_des = COALESCE(lprov.PROVINCIA, '') ")
            Sql.AppendLine("FROM Reg_Impianti				impi ")
            Sql.AppendLine("INNER JOIN GIS_Entita			entita		ON entita.Piva = impi.PIVA AND entita.Sa_Cod = impi.SA_COD AND entita.Appezza = impi.APPEZZA AND entita.Id_Imp = impi.ID_REG ")
            Sql.AppendLine("INNER JOIN GIS_ElementiGrafici	elem		ON elem.PivaSuperUser = entita.PivaSuperUser AND elem.Entita_Cod = entita.Entita_Cod ")
            Sql.AppendLine("INNER JOIN Cultivar				culti		ON culti.Cul_Cod = impi.CUL_COD ")
            Sql.AppendLine("INNER JOIN Mappatura_cte		mappa		ON mappa.Veg_Cod = culti.Veg_Cod AND mappa.Grva_Cod = ABS(impi.GRVA_Cod_VEG) ")
            Sql.AppendLine("INNER JOIN Appezzamento			appezza		ON appezza.Piva = impi.PIVA AND appezza.Sa_Cod = impi.SA_COD AND appezza.Appezza = impi.APPEZZA ")
            Sql.AppendLine("INNER JOIN SpecieVegetali		veg			ON veg.Veg_Cod = culti.Veg_Cod ")
            Sql.AppendLine("INNER JOIN GruppoVarietale		grvar		ON grvar.grva_cod = ABS(impi.GRVA_Cod_VEG) ")
            Sql.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente layer	 ON impi.Codice_Fiscale_Tecnico = layer.Gruppi_Utente_Identificativo ")
            Sql.AppendLine()
            Sql.AppendLine("LEFT JOIN ( ")
            Sql.AppendLine("	SELECT PIVA, sa_cod, cod_indirizzo = MIN(cod_indirizzo) FROM CentrixIndirizzi GROUP BY PIVA, sa_cod ")
            Sql.AppendLine(") cxi ON cxi.PIVA = impi.PIVA AND cxi.sa_cod = impi.SA_COD ")
            Sql.AppendLine("LEFT JOIN Indirizzi				indir		ON cxi.Cod_Indirizzo = indir.Cod_Indirizzo ")
            Sql.AppendLine("LEFT JOIN						ISTAT		ON ISTAT.PROV = indir.pro_cod_istat AND ISTAT.COM = indir.com_cod_istat ")
            Sql.AppendLine("LEFT JOIN Lista_Province		lprov		ON lprov.PROV = indir.pro_cod_istat ")
            Sql.AppendLine()
            Sql.AppendLine("WHERE impi.Validita_inizio >= @VALIDITA_INIZIO AND impi.Validita_fine <= @VALIDITA_FINE ")
            Sql.AppendLine("), ")
            Sql.AppendLine()
            Sql.AppendLine("stato_lookup_cte AS ( ")
            Sql.AppendLine("	SELECT cod, descr FROM ( ")
            Sql.AppendLine("		VALUES ")
            Sql.AppendLine("			(1, 'In attesa'), ")
            Sql.AppendLine("			(2, 'Accettata'), ")
            Sql.AppendLine("			(3, 'Rifiutata') ")
            Sql.AppendLine("	) AS d(cod, descr) ")
            Sql.AppendLine(") ")
            Sql.AppendLine()
            Sql.AppendLine("SELECT ")
            Sql.AppendLine("	Proprietario = CASE WHEN proprietario.CODICE_FISCALE_TECNICO = @CODICE_FISCALE_TECNICO THEN 1 ELSE 0 END ")
            Sql.AppendLine("    , notifiche.Interferenze_cod ")
            Sql.AppendLine("	, Stato_cod ")
            Sql.AppendLine("	, Stato = stato.descr ")
            Sql.AppendLine("    , notifiche.Data_Creazione ")
            Sql.AppendLine("	, notifiche.Data_Conferma ")
            Sql.AppendLine("    , notifiche.DistanzaEffettiva ")
            Sql.AppendLine()
            Sql.AppendLine("    , [proprietario.codice_fiscale_tecnico] = proprietario.CODICE_FISCALE_TECNICO ")
            Sql.AppendLine("	, [proprietario.entita_cod] = notifiche.Entita_Cod_Propietario ")
            Sql.AppendLine("	, [proprietario.veg_des] = proprietario.Veg_Des ")
            Sql.AppendLine("	, [proprietario.grva_des] = proprietario.Grva_Des ")
            Sql.AppendLine("	, [proprietario.coord.lat] = proprietario.lat ")
            Sql.AppendLine("	, [proprietario.coord.lng] = proprietario.lng ")
            Sql.AppendLine("	, [proprietario.indirizzo] = proprietario.indirizzo ")
            Sql.AppendLine("	, [proprietario.layer] = proprietario.layer ")
            Sql.AppendLine("	, [proprietario.ind_des] = proprietario.ind_des ")
            Sql.AppendLine("	, [proprietario.frz_des] = proprietario.frz_des ")
            Sql.AppendLine("	, [proprietario.com_des] = proprietario.com_des ")
            Sql.AppendLine("	, [proprietario.pr_des] = proprietario.pr_des ")
            Sql.AppendLine()
            Sql.AppendLine("    , [interferente.codice_fiscale_tecnico] = interferente.CODICE_FISCALE_TECNICO ")
            Sql.AppendLine("	, [interferente.entita_cod] = notifiche.Entita_Cod_Interferente ")
            Sql.AppendLine("	, [interferente.veg_des] = interferente.Veg_Des ")
            Sql.AppendLine("	, [interferente.grva_des] = interferente.Grva_Des ")
            Sql.AppendLine("	, [interferente.coord.lat] = interferente.lat ")
            Sql.AppendLine("	, [interferente.coord.lng] = interferente.lng ")
            Sql.AppendLine("	, [interferente.indirizzo] = interferente.indirizzo ")
            Sql.AppendLine("	, [interferente.layer] = interferente.layer ")
            Sql.AppendLine("	, [interferente.ind_des] = interferente.ind_des ")
            Sql.AppendLine("	, [interferente.frz_des] = interferente.frz_des ")
            Sql.AppendLine("	, [interferente.com_des] = interferente.com_des ")
            Sql.AppendLine("	, [interferente.pr_des] = interferente.pr_des ")
            Sql.AppendLine()
            Sql.AppendLine("FROM Sementieri_Sportello_InterferenzePerConferma	notifiche ")
            Sql.AppendLine("INNER JOIN impianti_cte								proprietario	ON proprietario.PivaSuperUser = notifiche.PivaSuperUser AND proprietario.Entita_Cod = notifiche.Entita_Cod_Propietario ")
            Sql.AppendLine("INNER JOIN impianti_cte								interferente	ON interferente.PivaSuperUser = notifiche.PivaSuperUser AND interferente.Entita_Cod = notifiche.Entita_Cod_Interferente ")
            Sql.AppendLine("INNER JOIN stato_lookup_cte							stato			ON stato.cod = notifiche.Stato_cod ")
            Sql.AppendLine()
            Sql.AppendLine("WHERE (Flag_Attivo <> 0 OR Flag_Attivo IS NULL) ")
            Sql.AppendLine("AND (proprietario.Codice_Fiscale_Tecnico = @CODICE_FISCALE_TECNICO OR Interferente.Codice_Fiscale_Tecnico = @CODICE_FISCALE_TECNICO OR @CODICE_FISCALE_TECNICO = '') ")

            If Accoda_Mail Then
                Sql.AppendLine("AND (StatoGestione = 0 OR StatoGestione IS NULL) ")
            End If

            Sql.AppendLine()
            Sql.AppendLine("ORDER BY proprietario desc, Entita_Cod_Interferente ")

            dt = EseguiQuery_Lettura(objParametri_server, Sql.ToString, NomeRoutine)

        Catch ex As Exception


            Dim MessaggioErrore As String = ex.Message

            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function


    Public Function LeggiNonAttiviAttiviPerSportello(id_specie As Integer, DataInizioSportello As Date, DataFineSportello As Date, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_InterferenzePerConferma_R.LeggiNonAttiviAttiviPerSportello()"

        Dim dt As DataTable = Nothing

        If id_specie = 0 Then

            Throw New ArgumentNullException("id_specie")
        End If

        Try
            Dim nomeDB_Utenti = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            Dim Sql As New StringBuilder

            Sql.Clear()

            Sql.AppendLine("DECLARE @VALIDITA_INIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizioSportello))
            Sql.AppendLine("DECLARE @VALIDITA_FINE DATETIME = " & Agro_SQL_SaveDateTime(DataFineSportello))
            Sql.AppendLine()
            Sql.AppendLine(";WITH mappatura_cte AS ")
            Sql.AppendLine("( ")
            Sql.AppendLine("SELECT DISTINCT Veg_Cod, Grva_Cod FROM Mappatura_Specie WHERE ID_Specie = " & id_specie.ToString)
            Sql.AppendLine("), ")
            Sql.AppendLine()
            Sql.AppendLine("impianti_cte AS ")
            Sql.AppendLine("( ")
            Sql.AppendLine("SELECT ")
            Sql.AppendLine("	entita.PivaSuperUser, entita.Entita_Cod ")
            Sql.AppendLine("	, indirizzo = appezza.Via_Stringa ")
            Sql.AppendLine("	, veg.Veg_Des ")
            Sql.AppendLine("	, grvar.Grva_Des ")
            Sql.AppendLine("    , lat = ISNULL(elem.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) ")
            Sql.AppendLine("    , lng = ISNULL(elem.Poligono_GeoEntity.EnvelopeCenter().Long, 0) ")
            Sql.AppendLine("	, layer = COALESCE(layer.Gruppi_Utente_Des, '') ")
            Sql.AppendLine("	, ind_des = COALESCE(indir.ind_des, '') ")
            Sql.AppendLine("	, frz_des = COALESCE(indir.frz_des, '') ")
            Sql.AppendLine("	, com_des = COALESCE(ISTAT.LOCALITA, '') ")
            Sql.AppendLine("	, pr_des = COALESCE(lprov.PROVINCIA, '') ")
            Sql.AppendLine("FROM Reg_Impianti				impi ")
            Sql.AppendLine("INNER JOIN GIS_Entita			entita		ON entita.Piva = impi.PIVA AND entita.Sa_Cod = impi.SA_COD AND entita.Appezza = impi.APPEZZA AND entita.Id_Imp = impi.ID_REG ")
            Sql.AppendLine("INNER JOIN GIS_ElementiGrafici	elem		ON elem.PivaSuperUser = entita.PivaSuperUser AND elem.Entita_Cod = entita.Entita_Cod ")
            Sql.AppendLine("INNER JOIN Cultivar				culti		ON culti.Cul_Cod = impi.CUL_COD ")
            Sql.AppendLine("INNER JOIN Mappatura_cte		mappa		ON mappa.Veg_Cod = culti.Veg_Cod AND mappa.Grva_Cod = ABS(impi.GRVA_Cod_VEG) ")
            Sql.AppendLine("INNER JOIN Appezzamento			appezza		ON appezza.Piva = impi.PIVA AND appezza.Sa_Cod = impi.SA_COD AND appezza.Appezza = impi.APPEZZA ")
            Sql.AppendLine("INNER JOIN SpecieVegetali		veg			ON veg.Veg_Cod = culti.Veg_Cod ")
            Sql.AppendLine("INNER JOIN GruppoVarietale		grvar		ON grvar.grva_cod = ABS(impi.GRVA_Cod_VEG) ")
            Sql.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente layer	 ON impi.Codice_Fiscale_Tecnico = layer.Gruppi_Utente_Identificativo ")
            Sql.AppendLine()
            Sql.AppendLine("LEFT JOIN ( ")
            Sql.AppendLine("	SELECT PIVA, sa_cod, cod_indirizzo = MIN(cod_indirizzo) FROM CentrixIndirizzi GROUP BY PIVA, sa_cod ")
            Sql.AppendLine(") cxi ON cxi.PIVA = impi.PIVA AND cxi.sa_cod = impi.SA_COD ")
            Sql.AppendLine("LEFT JOIN Indirizzi				indir		ON cxi.Cod_Indirizzo = indir.Cod_Indirizzo ")
            Sql.AppendLine("LEFT JOIN						ISTAT		ON ISTAT.PROV = indir.pro_cod_istat AND ISTAT.COM = indir.com_cod_istat ")
            Sql.AppendLine("LEFT JOIN Lista_Province		lprov		ON lprov.PROV = indir.pro_cod_istat ")
            Sql.AppendLine()
            Sql.AppendLine("WHERE impi.Validita_inizio >= @VALIDITA_INIZIO AND impi.Validita_fine <= @VALIDITA_FINE ")
            Sql.AppendLine("), ")
            Sql.AppendLine()
            Sql.AppendLine("stato_lookup_cte AS ( ")
            Sql.AppendLine("	SELECT cod, descr FROM ( ")
            Sql.AppendLine("		VALUES ")
            Sql.AppendLine("			(1, 'In attesa'), ")
            Sql.AppendLine("			(2, 'Accettata'), ")
            Sql.AppendLine("			(3, 'Rifiutata') ")
            Sql.AppendLine("	) AS d(cod, descr) ")
            Sql.AppendLine(") ")
            Sql.AppendLine()
            Sql.AppendLine("SELECT ")
            Sql.AppendLine("	ISNULL(Descrizione_Casella_Conflitto, '') AS Descrizione_Casella_Conflitto  ")
            Sql.AppendLine("    , notifiche.Interferenze_cod ")
            Sql.AppendLine("	, Stato_cod ")
            Sql.AppendLine("	, Stato = stato.descr ")
            Sql.AppendLine("    , notifiche.Data_Creazione ")
            Sql.AppendLine("	, notifiche.Data_Conferma ")
            Sql.AppendLine("    , notifiche.DistanzaEffettiva ")
            Sql.AppendLine()
            Sql.AppendLine("	, [proprietario.entita_cod] = proprietario.Entita_Cod ")
            Sql.AppendLine("	, [proprietario.veg_des] = proprietario.Veg_Des ")
            Sql.AppendLine("	, [proprietario.grva_des] = proprietario.Grva_Des ")
            Sql.AppendLine("	, [proprietario.coord.lat] = proprietario.lat ")
            Sql.AppendLine("	, [proprietario.coord.lng] = proprietario.lng ")
            Sql.AppendLine("	, [proprietario.indirizzo] = proprietario.indirizzo ")
            Sql.AppendLine("	, [proprietario.layer] = proprietario.layer ")
            Sql.AppendLine("	, [proprietario.ind_des] = COALESCE(proprietario.ind_des, '') ")
            Sql.AppendLine("	, [proprietario.frz_des] = COALESCE(proprietario.frz_des, '') ")
            Sql.AppendLine("	, [proprietario.com_des] = COALESCE(proprietario.com_des, '') ")
            Sql.AppendLine("	, [proprietario.pr_des] = COALESCE(proprietario.pr_des, '') ")
            Sql.AppendLine()
            Sql.AppendLine("	, [interferente.entita_cod] = interferente.Entita_Cod ")
            Sql.AppendLine("	, [interferente.veg_des] = interferente.Veg_Des ")
            Sql.AppendLine("	, [interferente.grva_des] = interferente.Grva_Des ")
            Sql.AppendLine("	, [interferente.coord.lat] = interferente.lat ")
            Sql.AppendLine("	, [interferente.coord.lng] = interferente.lng ")
            Sql.AppendLine("	, [interferente.indirizzo] = interferente.indirizzo ")
            Sql.AppendLine("	, [interferente.layer] = interferente.layer ")
            Sql.AppendLine("	, [interferente.ind_des] = COALESCE(interferente.ind_des, '') ")
            Sql.AppendLine("	, [interferente.frz_des] = COALESCE(interferente.frz_des, '') ")
            Sql.AppendLine("	, [interferente.com_des] = COALESCE(interferente.com_des, '') ")
            Sql.AppendLine("	, [interferente.pr_des] = COALESCE(interferente.pr_des, '') ")
            Sql.AppendLine()
            Sql.AppendLine("FROM Sementieri_Sportello_InterferenzePerConferma	notifiche ")
            Sql.AppendLine("LEFT JOIN impianti_cte								proprietario	ON proprietario.PivaSuperUser = notifiche.PivaSuperUser AND proprietario.Entita_Cod = notifiche.Entita_Cod_Propietario ")
            Sql.AppendLine("LEFT JOIN impianti_cte								interferente	ON interferente.PivaSuperUser = notifiche.PivaSuperUser AND interferente.Entita_Cod = notifiche.Entita_Cod_Interferente ")
            Sql.AppendLine("INNER JOIN stato_lookup_cte							stato			ON stato.cod = notifiche.Stato_cod ")
            Sql.AppendLine()
            Sql.AppendLine("WHERE Flag_Attivo = 0 AND (proprietario.Entita_Cod IS NOT NULL OR interferente.Entita_Cod IS NOT NULL) ")

            dt = EseguiQuery_Lettura(objParametri_server, Sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message

            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

End Class


Public Class Sementieri_Sportello_InterferenzePerConferma_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Modifica(ByVal Interferenze_cod As Integer,
                             ByVal Stato_cod As Int32,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
            StrSQL.Append("    Data_Conferma     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,Stato_cod           =  " & Agro_SQL_SaveNum(Stato_cod) & " ")

            StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Interferenze_cod = " & Agro_SQL_SaveNum(Interferenze_cod) & " ")


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

    Public Function ModificaStatoGestione(
                                ByVal Interferenze_cod As Integer,
                                ByVal Stato_cod As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
            StrSQL.Append("    Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,StatoGestione           =  " & Agro_SQL_SaveNum(Stato_cod) & " ")

            StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Interferenze_cod = " & Agro_SQL_SaveNum(Interferenze_cod) & " ")


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

    Public Function Modifica_Flag(ByVal Interferenze_Cod As Integer,
                                  ByVal Flag_Attivo As Int32,
                                  ByVal setDescrToNull As Boolean,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
            StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))
            If setDescrToNull Then
                StrSQL.Append("    , Descrizione_casella_conflitto     =  NULL")
            End If
            StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Interferenze_Cod = " & Agro_SQL_SaveNum(Interferenze_Cod) & " ")

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


    'Public Function Modifica_Flag_Entita_Cod_Propietario( _
    '                            ByVal Entita_Cod_Propietario As Integer, _
    '                            ByVal Flag_Attivo As Int32, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Propietario = " & Agro_SQL_SaveNum(Entita_Cod_Propietario) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function

    'Public Function Modifica_Flag_Entita_Cod_Interferente( _
    '                        ByVal Entita_Cod_Interferente As Integer, _
    '                        ByVal Flag_Attivo As Int32, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
    '        StrSQL.Append("    Flag_Attivo     =  " & Agro_SQL_SaveNum(Flag_Attivo))

    '        StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Entita_Cod_Interferente = " & Agro_SQL_SaveNum(Entita_Cod_Interferente) & " ")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function



    Public Function ScriviInCache(ByVal interferenze_Cod As Integer,
                                  ByVal FlagAttivo As Integer,
                                  ByVal rigaXcache As DataRow,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        'non devo considerare il contenuto della celle relativa alla colonna Descrizione_Casella_Conflitto
        Dim strCols As String = ""
        Dim strCells As String = ""
        Dim first As Boolean = True
        For Each col In rigaXcache.Table.Columns
            If Not first Then
                strCols &= "|"
                strCells &= "|"
            End If
            strCols &= col.ColumnName
            If col.columnName <> "Descrizione_Casella_Conflitto" Then
                strCells &= rigaXcache.Item(col.ColumnName)
            End If
            first = False
        Next

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
            StrSQL.AppendLine("Descrizione_Casella_Conflitto = '" & Agro_SQL_SaveText(strCols & "§" & strCells) & "', ")
            StrSQL.AppendLine("Flag_Attivo = " & Agro_SQL_SaveNum(FlagAttivo))
            StrSQL.AppendLine("WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("AND interferenze_cod = " & Agro_SQL_SaveNum(interferenze_Cod) & " ")

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





    Public Function Modifica_Tutti_Interferenti( _
                                ByVal Entita_Cod_Interferente As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Sementieri_Sportello_InterferenzePerConferma SET ")
            StrSQL.Append("    flag_Attivo     =  " & Agro_SQL_SaveNum(False))

            StrSQL.Append(" WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Entita_Cod_Interferente = " & Agro_SQL_SaveNum(Entita_Cod_Interferente) & " ")


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


    Public Function Scrivi( _
                            ByVal Interferenze_Cod As Integer, _
                            ByVal Entita_Cod_Propietario As Integer, _
                            ByVal Entita_Cod_Interferente As Integer, _
                            ByVal Flag_Attivo As Integer, _
                            ByVal DistanzaEffettiva As Decimal, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_InterferenzePerConferma_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Sementieri_Sportello_InterferenzePerConferma (Pivasuperuser, ")
            StrSQL.Append("                    interferenze_cod, ")
            StrSQL.Append("                    stato_cod , ")
            StrSQL.Append("                    Entita_Cod_Propietario, ")
            StrSQL.Append("                    Entita_Cod_Interferente, ")
            StrSQL.Append("                    Flag_Attivo, ")
            StrSQL.Append("                    DistanzaEffettiva ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Interferenze_Cod))
            StrSQL.Append("         , 1 ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod_Propietario))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod_Interferente))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Flag_Attivo))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(DistanzaEffettiva))
            StrSQL.Append(")")
            '---------------------------------------------

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