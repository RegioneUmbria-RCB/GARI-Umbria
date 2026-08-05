Imports System.Text
Imports AgronicaCoreDataProvider
Imports InData.Operazione
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Rilievi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Rilievi_New(ByVal Piva As String,
                                      ByVal SaCod As String,
                                      ByVal VegCod As Integer,
                                      ByVal IdCod As Integer,
                                      ByVal LavCod As Integer,
                                      ByVal validitaInizio As DateTime,
                                      ByVal validitaFine As DateTime,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Operazione_Causale_R.Leggi_Rilievi_New()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.AppendLine(" SELECT gi.Padre AS Piva_Riferimento, I2.rag_soc AS Impresa_Riferimento, D.Piva_Rilievo as Piva, D.rag_soc, D.CUAA, D.Sa_Cod_Rilievo as Sa_Cod, D.Sa_Nome_Rilievo, D.Validita_Inizio_Rilievo, ")
            strSql.AppendLine("         D.Regione, D.Provincia, D.Localita, D.Indirizzo, D.Veg_Cod, D.Veg_Des, D.Cul_Cod, D.Cul_Des, D.Cod_DestUso, D.Des_DestUso, D.Blocco_Flag_Rilievo as Blocco_Flag, D.Lav_Cod_Rilievo as Lav_cod, ")
            strSql.AppendLine("         D.Lav_Des_Rilievo as Lav_Des, D.Appezza, D.APP_NOME, D.AppezzamentoID, D.Rilievo, D.Valore_Rilievo, D.Latitude, D.Longitude, D.SUP_APP, D.Allegati, D.Note, D.Superficie_Trattata, D.Causali_Rilievo, ")
            strSql.AppendLine("         D.UserCreazione, D.UserModifica, D.IdAgenda_Rilievo As id_agenda, D.Data2 ")

            strSql.AppendLine(" FROM ( ")

            strSql.AppendLine(getRilieviQueryString(objParametri_Utenti).ToString())

            If Piva <> "" Then
                strSql.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If SaCod <> "" Then
                strSql.AppendLine(" AND ca.sa_cod = '" & Agro_SQL_SaveText(SaCod) & "' ")
            End If

            If VegCod <> -1 Then
                If VegCod <> 0 Then
                    strSql.AppendLine(" AND SV.Veg_Cod = " & Agro_SQL_SaveNum(VegCod.ToString()) & " ")
                Else
                    strSql.AppendLine(" AND SV.Veg_Cod IS NULL AND Codici_Anagrafe.codice IS NULL ")
                End If
            Else
                If IdCod <> -1 Then
                    strSql.AppendLine(" AND Codici_Anagrafe.codice = " & Agro_SQL_SaveNum(IdCod.ToString()) & " ")
                End If
            End If

            If LavCod <> 0 Then
                strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LavCod.ToString()) & " ")
            End If

            strSql.AppendLine(" AND Movimenti.Data_Movimento between " + Agro_SQL_SaveDateTime(validitaInizio) + " AND " + Agro_SQL_SaveDateTime(validitaFine) + " ")

            strSql.AppendLine(" ) As D ")

            strSql.AppendLine("  LEFT OUTER JOIN GerarchiaImprese GI on D.Piva_Rilievo = GI.Figlio ")
            strSql.AppendLine("  LEFT OUTER JOIN Imprese I2 ON GI.Padre = I2.PIVA ")

            strSql.AppendLine(" ORDER BY Validita_Inizio_Rilievo DESC, id_agenda ")
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function getRilieviQueryString(ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As StringBuilder

        Dim strSql As New StringBuilder

        strSql.AppendLine(" SELECT DISTINCT ")
        strSql.AppendLine("     Agenda.Piva as Piva_Rilievo, Imprese.rag_soc, Imprese_Codici.val_cod as CUAA, ca.sa_cod as Sa_Cod_Rilievo, ")
        strSql.AppendLine("     ca.sa_nome as Sa_Nome_Rilievo, ISNULL(Lista_Regioni.regione_des, ' ') as Regione, ISNULL(Lista_Province.PROVINCIA, ' ') AS Provincia, ")
        strSql.AppendLine("     ISNULL(ISTAT.LOCALITA, ' ') AS Localita, Ind.ind_des AS Indirizzo, ")
        strSql.AppendLine("     Agenda.Validita_Inizio as Validita_Inizio_Rilievo, SV.Veg_Cod, SV.Veg_Des, c.Cul_Cod, c.Cul_Des, Codici_Anagrafe.codice as Cod_DestUso, Codici_Anagrafe.descrizione as Des_DestUso, ")
        strSql.AppendLine("     Agenda.Blocco_Flag As Blocco_Flag_Rilievo, Agenda.Lav_Cod As Lav_Cod_Rilievo, ")
        strSql.AppendLine("     Operazioni.LAV_DES as Lav_Des_Rilievo, Appezzamento.Appezza, CONCAT(imp.piva, '-', imp.SA_COD,'-', imp.APPEZZA, '-', imp.id_reg) AS AppezzamentoID, Appezzamento.APP_NOME, ")
        strSql.AppendLine("     CoordinateGIS.Latitude, CoordinateGIS.Longitude, Appezzamento.SUP_APP, ")
        strSql.AppendLine("     Mov_Destinazioni.Qta2 as Superficie_Trattata, ")
        strSql.AppendLine("     (SELECT STRING_AGG(AD.Allegati_Documenti_NomeFile, ', ')  ")
        strSql.AppendLine("       FROM Alert_Entita AE  ")
        strSql.AppendLine("        LEFT OUTER JOIN Allegati_Documenti AD ON AD.Allegati_Documenti_Cod=AE.Allegati_Documenti_Cod  ")
        strSql.AppendLine("      WHERE AE.PIVA=Agenda.PIVA AND AE.ID_Agenda = Agenda.ID_Agenda) AS Allegati, ")
        strSql.AppendLine("     Movimenti.Mov_Desc as Note, ")
        strSql.AppendLine("     CONCAT(UDC.Nome, ' ', UDC.Cognome) AS UserCreazione, ")
        strSql.AppendLine("     CONCAT(UDM.Nome, ' ', UDM.Cognome) AS UserModifica, ")
        strSql.AppendLine("     CASE ")
        strSql.AppendLine("         WHEN Agenda.Lav_Cod IN (" & LAVCOD_RILIEVO_AVVERSITA_CAMPO & ", " & LAVCOD_RILIEVO_ERBE_INFESTANTI & ") ")
        strSql.AppendLine("             THEN ")
        strSql.AppendLine("                 CASE WHEN av_Des_Vol IS NOT NULL")
        strSql.AppendLine("                     THEN av_Des_Vol + ' (' + UM.Udm_des COLLATE Latin1_General_CI_AS + ') ' ")
        strSql.AppendLine("                 ELSE ")
        strSql.AppendLine("                     av_gru_Des + ' (' + UM.Udm_des COLLATE Latin1_General_CI_AS + ') ' ")
        strSql.AppendLine("                 END ")
        strSql.AppendLine("         WHEN Agenda.Lav_Cod = " & LAVCOD_DANNI_RACCOLTA & " ")
        strSql.AppendLine("             THEN dr.DR_DES + ' (' + UM.Udm_des COLLATE Latin1_General_CI_AS + ') ' ")
        strSql.AppendLine("         WHEN Agenda.Lav_Cod = " & LAVCOD_FASI_FENOLOGICHE & " ")
        strSql.AppendLine("             THEN FaseFen.Descrizione ")
        strSql.AppendLine("         ELSE ")
        strSql.AppendLine("             IM.IND_MAT_DES + ' (' + UM.Udm_des COLLATE Latin1_General_CI_AS + ')' ")
        strSql.AppendLine("     END as Rilievo, ")
        strSql.AppendLine("     CASE ")
        strSql.AppendLine("         WHEN UM.TipoControllo_Cod = 5 OR Agenda.Lav_Cod = " & LAVCOD_FASI_FENOLOGICHE & " ")
        strSql.AppendLine("             THEN CONVERT(varchar, Mov_Destinazioni.validita_inizio, 103) ")
        strSql.AppendLine("         WHEN Agenda.Lav_Cod = " & LAVCOD_RILIEVO_INDICI_MATURITA & " ")
        strSql.AppendLine("             THEN CASE ")
        strSql.AppendLine("                 WHEN IM_A.Anag_des IS NULL ")
        strSql.AppendLine("                     THEN CASE ")
        strSql.AppendLine("                         WHEN UM.TipoControllo_Cod = 4 ")
        strSql.AppendLine("                             THEN CASE ")
        strSql.AppendLine("                                 WHEN Mov_Destinazioni.Qta = 1 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Sì & "' ")
        strSql.AppendLine("                                 WHEN Mov_Destinazioni.Qta = 0 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.No & "' ")
        strSql.AppendLine("                                 ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("                              END ")
        strSql.AppendLine("                         ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("                      END ")
        strSql.AppendLine("              ELSE IM_A.Anag_des ")
        strSql.AppendLine("         END ")
        strSql.AppendLine("         WHEN Agenda.Lav_Cod = " & LAVCOD_RILIEVO_AVVERSITA_CAMPO & " ")
        strSql.AppendLine("             THEN CASE ")
        strSql.AppendLine("                 WHEN IM_A.Anag_des IS NULL ")
        strSql.AppendLine("                     THEN CASE ")
        strSql.AppendLine("                         WHEN UM.TipoControllo_Cod = 4 ")
        strSql.AppendLine("                             THEN CASE ")
        strSql.AppendLine("                                 WHEN Mov_Destinazioni.Qta = 1 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Sì & "' ")
        strSql.AppendLine("                                 WHEN Mov_Destinazioni.Qta = 0 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.No & "' ")
        strSql.AppendLine("                                 ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("                              END ")
        strSql.AppendLine("                         ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("                      END ")
        strSql.AppendLine("              ELSE anag.Anag_des ")
        strSql.AppendLine("         END ")
        strSql.AppendLine("     ELSE ")
        strSql.AppendLine("       CASE ")
        strSql.AppendLine("         WHEN UM.TipoControllo_Cod = 4 ")
        strSql.AppendLine("             THEN CASE ")
        strSql.AppendLine("                 WHEN Mov_Destinazioni.Qta = 1 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Sì & "' ")
        strSql.AppendLine("                 WHEN Mov_Destinazioni.Qta = 0 THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.No & "' ")
        strSql.AppendLine("                 ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("             END ")
        strSql.AppendLine("         ELSE CAST(Mov_Destinazioni.Qta AS varchar) ")
        strSql.AppendLine("       END ")
        strSql.AppendLine("     END AS Valore_Rilievo, ")
        strSql.AppendLine("     ISNULL(Causali.ListaCausali, '') as Causali_Rilievo, ")
        strSql.AppendLine("     Agenda.Id_Agenda as IdAgenda_Rilievo, Movimenti.Data_Movimento as [Data2] ")
        strSql.AppendLine(" FROM Agenda ")
        strSql.AppendLine("  INNER JOIN Imprese on Agenda.PIVA = Imprese.PIVA ")
        strSql.AppendLine("  LEFT OUTER JOIN Imprese_Codici on Imprese.PIVA = Imprese_Codici.PIVA and Imprese_Codici.id_cod = 1010 ")
        strSql.AppendLine("  INNER JOIN ImpresexIndirizzi II ON II.PIVA = Imprese.PIVA ")
        strSql.AppendLine("  INNER JOIN Indirizzi Ind ON II.Cod_Indirizzo = Ind.Cod_Indirizzo ")
        strSql.AppendLine("  LEFT OUTER JOIN ISTAT ON Ind.pro_cod_istat = ISTAT.PROV AND Ind.com_cod_istat = ISTAT.COM ")
        strSql.AppendLine("  LEFT OUTER JOIN Lista_Province ON Ind.pro_cod_istat = Lista_Province.PROV ")
        strSql.AppendLine("  LEFT OUTER JOIN Lista_Regioni ON Lista_Province.REG = Lista_Regioni.Reg ")
        strSql.AppendLine("  INNER JOIN Centri_Aziendali ca on Agenda.PIVA = ca.PIVA and Agenda.Sa_Cod = ca.sa_cod ")
        strSql.AppendLine("  INNER JOIN Operazioni on Operazioni.Lav_Cod = Agenda.Lav_Cod ")
        strSql.AppendLine("  INNER JOIN Movimenti on  Agenda.PIVA = Movimenti.PIVA and  Agenda.Sa_Cod = Movimenti.Sa_Cod and Agenda.Id_Agenda = Movimenti.Id_Agenda ")
        strSql.AppendLine("  INNER JOIN Movimenti_dettagli on  Agenda.PIVA = Movimenti_dettagli.PIVA and  Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod and ")
        strSql.AppendLine("             Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda and Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
        strSql.AppendLine("  INNER JOIN Mov_Dettaglio_Tecnico on agenda.piva=Mov_Dettaglio_Tecnico.piva and agenda.id_agenda=Mov_Dettaglio_Tecnico.id_agenda ")
        strSql.AppendLine("             and Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov and Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
        strSql.AppendLine("  LEFT OUTER JOIN Mov_Destinazioni ON Mov_Dettaglio_Tecnico.Piva = Mov_Destinazioni.Piva AND Mov_Dettaglio_Tecnico.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
        strSql.AppendLine("              AND Mov_Dettaglio_Tecnico.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Mov_Dettaglio_Tecnico.Id_Mov = Mov_Destinazioni.Id_Mov ")
        strSql.AppendLine("              AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND Mov_Destinazioni.Tipo_Destinazione = 0 ")
        strSql.AppendLine("  LEFT OUTER JOIN Reg_Impianti imp on imp.piva = Mov_Destinazioni.piva AND imp.sa_Cod = Mov_Destinazioni.Sa_Cod AND imp.APPEZZA = Mov_Destinazioni.Appezza AND imp.ID_REG = Mov_Destinazioni.Id_Destinazione ")
        strSql.AppendLine("  LEFT OUTER JOIN Reg_Impianti_Codici ric ON ric.PIVA = Mov_Destinazioni.Piva AND ric.SA_COD = Mov_Destinazioni.Sa_Cod AND ric.APPEZZA = Mov_Destinazioni.Appezza AND ric.ID_REG = Mov_Destinazioni.Id_Destinazione AND ric.id_cod between 3000 and 3999 ")
        strSql.AppendLine("  LEFT OUTER JOIN cultivar c ON c.Cul_Cod = imp.CUL_COD ")
        strSql.AppendLine("  LEFT OUTER JOIN SpecieVegetali SV on ((Movimenti_dettagli.Dettaglio_VegCod <> 0 AND SV.Veg_Cod = Movimenti_dettagli.Dettaglio_VegCod) OR SV.Veg_Cod = c.veg_cod) ")
        strSql.AppendLine("  LEFT OUTER JOIN Codici_Anagrafe on ((Movimenti_dettagli.Dettaglio_IdCod <> 0 AND Codici_Anagrafe.codice = Movimenti_dettagli.Dettaglio_IdCod) OR Codici_Anagrafe.codice = ric.id_cod) ")
        strSql.AppendLine("  LEFT OUTER JOIN Appezzamento ON Mov_Destinazioni.PIVA = Appezzamento.Piva AND Mov_Destinazioni.SA_COD = Appezzamento.Sa_Cod AND Mov_Destinazioni.APPEZZA = Appezzamento.Appezza ")
        strSql.AppendLine("  LEFT OUTER JOIN Avversita on Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod and Mov_Dettaglio_Tecnico.av_cod<>0 ")
        strSql.AppendLine("  LEFT OUTER JOIN GruppoAvversita on GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru and Mov_Dettaglio_Tecnico.av_gru<>0 ")
        strSql.AppendLine("  LEFT OUTER JOIN SpecieVegetaliXStadiCrescita FaseFen on FaseFen.Cod_SS = Mov_Dettaglio_Tecnico.FF_Classe and SV.Veg_Cod = FaseFen.Veg_Cod ")
        strSql.AppendLine("  LEFT OUTER JOIN IndiciMaturita IM on IM.IND_MAT_COD = Mov_Dettaglio_Tecnico.FF_Classe ")
        strSql.AppendLine("  LEFT OUTER JOIN DanniRaccolta dr on dr.DR_COD = Mov_Dettaglio_Tecnico.FF_Classe ")
        strSql.AppendLine("  LEFT OUTER JOIN MisuraXIndiciMaturita_Anagrafiche IM_A on IM.IND_MAT_COD = IM_A.Ind_Mat_Cod and Mov_Destinazioni.Qta = IM_A.Anag_Valore AND IM_A.Udm_Cod = Mov_Dettaglio_Tecnico.Dett_Cod ")
        strSql.AppendLine("  LEFT OUTER JOIN MisuraxAvversita ma on ma.UDM_COD = Mov_Dettaglio_Tecnico.Dett_Cod AND ma.VEG_COD = c.Veg_Cod AND ma.AV_COD = Mov_Dettaglio_Tecnico.Av_Cod ")
        strSql.AppendLine("  LEFT OUTER JOIN MisuraXAvversita_Anagrafiche anag on anag.MxAV_Cod = ma.COD AND anag.Anag_valore = Mov_Destinazioni.Qta ")
        strSql.AppendLine("  LEFT OUTER JOIN UnitaMisura UM on UM.udm_cod = Mov_Dettaglio_Tecnico.Dett_Cod ")
        strSql.AppendLine("  LEFT OUTER JOIN (	SELECT mdCausale.Piva, mdCausale.Id_Agenda, mdCausale.Id_Mov, STRING_AGG(OC.Causale, ', ') As ListaCausali ")
        strSql.AppendLine("                      FROM agenda aCausale ")
        strSql.AppendLine("                     INNER JOIN Mov_Dettaglio_Tecnico mdCausale ON aCausale.Lav_Cod = 169 AND aCausale.Id_Agenda = mdCausale.Id_Agenda ")
        strSql.AppendLine("                     INNER JOIN Operazione_Causale OC on mdCausale.Id_Mov_Det = 0 AND OC.Id = mdCausale.Dett_Cod ")
        strSql.AppendLine("                     GROUP BY mdCausale.Piva, mdCausale.Id_Agenda, mdCausale.Id_Mov ")
        strSql.AppendLine("  ) Causali ON Causali.Piva = Mov_Dettaglio_Tecnico.piva AND Causali.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda and Causali.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")

        'query su GIS per coordinate
        strSql.AppendLine("  LEFT OUTER JOIN (	SELECT DISTINCT GIS_Entita.Piva, GIS_Entita.ID_Agenda, GIS_ElementiGrafici.Poligono_GeoEntity.Lat AS Latitude, GIS_ElementiGrafici.Poligono_GeoEntity.Long AS Longitude ")
        strSql.AppendLine("                      FROM GIS_Entita ")
        strSql.AppendLine("                     LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = 63 ")
        strSql.AppendLine("                     WHERE ID_Agenda > 0 ")
        strSql.AppendLine("  ) CoordinateGIS ON CoordinateGIS.Piva = Agenda.PIVA AND CoordinateGIS.ID_Agenda = Agenda.ID_Agenda ")

        strSql.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli UDC ON UDC.CodFisc = Agenda.Username_Creazione ")
        strSql.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli UDM ON UDM.CodFisc = Agenda.Username_Modifica ")

        strSql.AppendLine("  WHERE Agenda.Lav_Cod IN (" & STR_OP_RILIEVI & ")")

        Return strSql

    End Function

End Class
