Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class Catasto
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################
    Public Function SchedaCatastoUtilizzi(ByVal Piva As String,
                                            ByVal Validita_Inizio As String,
                                            ByVal Validita_Fine As String,
                                            ByVal Str_FiltroImpianti As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Catasto.SchedaCatastoUtilizzi"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim stbSelect As New StringBuilder
        Dim stbSelectAppezza As New StringBuilder
        Dim stbSelectCampi As New StringBuilder
        Dim stbJoin As New StringBuilder
        Dim stbjoinAppezza As New StringBuilder
        Dim stbjoinCampi As New StringBuilder
        Dim stbWhere As New StringBuilder
        Dim stbWhereAppezza As New StringBuilder
        Dim stbWhereCampi As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stbSelect.Append(" SELECT DISTINCT 0 as copia, Reg_Impianti.PIVA, " & vbCrLf)
            stbSelect.Append(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Imprese.partitaIvaReale END AS PivaReale," & vbCrLf)
            stbSelect.Append(" Reg_Impianti.SA_COD, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg," & vbCrLf)
            stbSelect.Append(" Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, " & vbCrLf)
            stbSelect.Append(" Indirizzi.ind_des, Indirizzi.frz_des, ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, " & vbCrLf)

            stbSelect.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des, " & vbCrLf)
            stbSelect.Append(" ISNULL((SELECT  TOP 1   Val_Cod " & vbCrLf)
            stbSelect.Append("           FROM    reg_impianti_codici " & vbCrLf)
            stbSelect.Append("            WHERE  Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stbSelect.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod >= 3000 and id_cod < 4000), '')  AS dest_uso, " & vbCrLf)

            stbSelect.Append(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  Reg_Impianti.Sup_Imp, " & vbCrLf)
            stbSelect.Append(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, ISNULL(Reg_Impianti.COP_COD, 0) AS COP_COD, " & vbCrLf)
            stbSelect.Append(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, ISNULL(Reg_Impianti.IMP_COD,0) AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  " & vbCrLf)
            stbSelect.Append(" ISNULL(GruppoFinalita.grfi_des, '') AS grfi_des, ISNULL(GruppoVarietale.grva_Des, '') AS grva_des,  " & vbCrLf)
            stbSelect.Append(" ISNULL(FormeAllevamento.foral_Des, '') AS foral_des, ISNULL(Portinnesti.port_Des, '') AS port_des,  " & vbCrLf)
            stbSelect.Append(" ISNULL(ImpiantiIrrigazioni.imp_Des, '') AS imp_des, ISNULL(Copertura.cop_Des, '') AS cop_des,  " & vbCrLf)

            stbSelect.Append(" ISNULL((SELECT  TOP 1   Val_Cod " & vbCrLf)
            stbSelect.Append("           FROM         reg_impianti_codici " & vbCrLf)
            stbSelect.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stbSelect.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1063), 0)  AS SU_FILA, " & vbCrLf)
            stbSelect.Append(" ISNULL((SELECT  TOP 1   Val_Cod " & vbCrLf)
            stbSelect.Append("           FROM         reg_impianti_codici " & vbCrLf)
            stbSelect.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stbSelect.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1061), 0)  AS TRA_FILA, " & vbCrLf)

            'dati distinta
            stbSelect.Append(" ISNULL((SELECT TOP 1 CONVERT(varchar(50),Imprese_Progetti.p_ha) + '§' + CONVERT(varchar(50),Produzione_Prevista) + '§' + CONVERT(varchar(50),regolamento_cod)  " & vbCrLf)
            stbSelect.Append("        FROM Imprese_Progetti " & vbCrLf)
            stbSelect.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA " & vbCrLf)
            stbSelect.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
            stbSelect.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA " & vbCrLf)
            stbSelect.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG " & vbCrLf)
            stbSelect.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            stbSelect.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)
            stbSelect.Append("	     ORDER BY Imprese_Progetti.Validita_Inizio DESC " & vbCrLf)
            stbSelect.Append("), '0§0§0') AS dati_distinta, " & vbCrLf)

            ''resa
            'stbSelect.Append(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti " & vbCrLf)
            'stbSelect.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA " & vbCrLf)
            'stbSelect.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod " & vbCrLf)
            'stbSelect.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA " & vbCrLf)
            'stbSelect.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG " & vbCrLf)
            'stbSelect.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            'stbSelect.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)
            'stbSelect.Append("	     ORDER BY Imprese_Progetti.Validita_Inizio DESC " & vbCrLf)
            'stbSelect.Append("), 0) AS Resa, " & vbCrLf)

            stbSelect.Append(" ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, " & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE," & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)

            'PARTE DI APPEZZAMENTI
            stbSelectAppezza.Append(" 0 as Campo_Cod, '' AS CAMPO_Nome, " & vbCrLf)
            stbSelectAppezza.Append(" AppezzamentiXParticelle.AREA, AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            stbSelectAppezza.Append(" Appezzamento.Validita_Inizio AS Validita_Inizio, Appezzamento.Validita_Fine AS Validita_Fine " & vbCrLf)

            'PARTE DI CAMPI
            stbSelectCampi.Append(" Campi.Campo_Cod, Campi.Campo_des AS CAMPO_Nome, " & vbCrLf)
            stbSelectCampi.Append("  CampiXParticelle.AREA, CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            stbSelectCampi.Append(" Campi.Validita_Inizio AS Validita_Inizio, Campi.Validita_Fine AS Validita_Fine " & vbCrLf)

            stbJoin.Append(" FROM Reg_Impianti " & vbCrLf)
            stbJoin.Append(" INNER JOIN Imprese ON Imprese.PIVA = Reg_Impianti.PIVA " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod   " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN FormeAllevamento ON FormeAllevamento.foral_cod = Reg_Impianti.foral_cod " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN Portinnesti ON Portinnesti.port_cod = Reg_Impianti.port_cod " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN ImpiantiIrrigazioni ON ImpiantiIrrigazioni.imp_cod = Reg_Impianti.imp_cod " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN Copertura ON Copertura.cop_cod = Reg_Impianti.cop_cod " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN GruppoFinalita ON GruppoFinalita.grfi_cod = Reg_Impianti.grfi_cod " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN GruppoVarietale ON GruppoVarietale.grva_cod = Reg_Impianti.grva_cod_veg " & vbCrLf)

            stbJoin.Append(" INNER JOIN Centri_Aziendali ON Reg_Impianti.PIVA = Centri_Aziendali.PIVA AND Reg_Impianti.sa_cod = Centri_Aziendali.sa_cod " & vbCrLf)
            stbJoin.Append(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod " & vbCrLf)
            stbJoin.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo " & vbCrLf)
            stbJoin.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)

            stbJoin.Append(" INNER JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  " & vbCrLf)
            stbJoin.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA  " & vbCrLf)

            stbjoinAppezza.Append(" INNER JOIN AppezzamentiXParticelle ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND  " & vbCrLf)
            stbjoinAppezza.Append(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND  " & vbCrLf)
            stbjoinAppezza.Append(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA " & vbCrLf)
            stbjoinAppezza.Append(" INNER JOIN ParticelleCatastali ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.COM = AppezzamentiXParticelle.COM AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)
            stbjoinAppezza.Append(" INNER JOIN  ImpreseXParticelle " & vbCrLf)
            stbjoinAppezza.Append(" ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV AND ImpreseXParticelle.COM = AppezzamentiXParticelle.COM " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)

            stbjoinCampi.Append(" INNER JOIN Campi ON Appezzamento.PIVA = Campi.PIVA " & vbCrLf)
            stbjoinCampi.Append(" AND Appezzamento.SA_COD = Campi.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN CampiXParticelle ON CampiXParticelle.PIVA = Campi.PIVA " & vbCrLf)
            stbjoinCampi.Append(" AND CampiXParticelle.SA_COD = Campi.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND CampiXParticelle.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN ParticelleCatastali ON ParticelleCatastali.PROV = CampiXParticelle.PROV " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.COM = CampiXParticelle.COM " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.sezione = CampiXParticelle.sezione " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.foglio = CampiXParticelle.foglio " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.numero = CampiXParticelle.numero " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.subalterno = CampiXParticelle.subalterno " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN  ImpreseXParticelle " & vbCrLf)
            stbjoinCampi.Append(" ON ImpreseXParticelle.PIVA = CampiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = CampiXParticelle.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.PROV = CampiXParticelle.PROV AND ImpreseXParticelle.COM = CampiXParticelle.COM " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.SEZIONE = CampiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = CampiXParticelle.FOGLIO " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.NUMERO = CampiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = CampiXParticelle.SUBALTERNO " & vbCrLf)


            stbWhere.Append(" WHERE Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbWhere.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhere.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)
            stbWhere.Append(Agro_SQL_Save_xFiltroAggiuntivo(Str_FiltroImpianti,, objParametri) & vbCrLf)

            stbWhereAppezza.Append(" AND AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereAppezza.Append(" AND AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereAppezza.Append(" AND Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereAppezza.Append(" AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereCampi.Append(" AND CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereCampi.Append(" AND Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND NOT EXISTS ( " & vbCrLf)
            stbWhereCampi.Append("      SELECT 1 " & vbCrLf)
            stbWhereCampi.Append("      FROM Appezzamento  " & vbCrLf)
            stbWhereCampi.Append("      INNER JOIN AppezzamentiXParticelle ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND " & vbCrLf)
            stbWhereCampi.Append("      AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            stbWhereCampi.Append("      AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA " & vbCrLf)
            stbWhereCampi.Append("      WHERE Appezzamento.Piva = Campi.piva " & vbCrLf)
            stbWhereCampi.Append("      AND Appezzamento.Sa_cod = Campi.sa_cod  " & vbCrLf)
            stbWhereCampi.Append("      AND Appezzamento.campo_cod = campi.campo_cod " & vbCrLf)
            stbWhereCampi.Append("  ) " & vbCrLf)


            '------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
            '------------------------------------------------
            stb.Append(" ( " & vbCrLf)
            stb.Append(stbSelect)
            stb.Append(stbSelectAppezza)
            stb.Append(stbJoin)
            stb.Append(stbjoinAppezza)
            stb.Append(stbWhere)
            stb.Append(stbWhereAppezza)
            stb.Append(" ) " & vbCrLf)

            stb.Append("  " & vbCrLf)
            stb.Append(" UNION ALL " & vbCrLf)
            stb.Append("  " & vbCrLf)

            '------------------------------------------------
            'PARTICELLE ASSOCIATE A CAMPI 
            '------------------------------------------------
            stb.Append(" ( " & vbCrLf)
            stb.Append(stbSelect)
            stb.Append(stbSelectCampi)
            stb.Append(stbJoin)
            stb.Append(stbjoinCampi)
            stb.Append(stbWhere)
            stb.Append(stbWhereCampi)
            stb.Append(" ) " & vbCrLf)

            stb.Append(" ORDER BY sa_nome, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
            'stb.Append(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################
    Public Function AttoNotorio_Semplificato(ByVal Piva As String,
                                            ByVal Validita_Inizio As String,
                                            ByVal Validita_Fine As String,
                                            ByVal Str_FiltroImpianti As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Catasto.AttoNotorio_Semplificato"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim stbSelect As New StringBuilder
        Dim stbSelectAppezza As New StringBuilder
        Dim stbSelectCampi As New StringBuilder
        Dim stbJoin As New StringBuilder
        Dim stbjoinAppezza As New StringBuilder
        Dim stbjoinCampi As New StringBuilder
        Dim stbWhere As New StringBuilder
        Dim stbWhereAppezza As New StringBuilder
        Dim stbWhereCampi As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stbSelect.Append(" SELECT DISTINCT 0 as copia, Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg," & vbCrLf)
            stbSelect.Append(" Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, " & vbCrLf)
            stbSelect.Append(" Indirizzi.ind_des, Indirizzi.frz_des, ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, " & vbCrLf)

            stbSelect.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des, " & vbCrLf)
            stbSelect.Append(" ISNULL((SELECT  TOP 1   Val_Cod " & vbCrLf)
            stbSelect.Append("           FROM    reg_impianti_codici " & vbCrLf)
            stbSelect.Append("            WHERE  Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  " & vbCrLf)
            stbSelect.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod >= 3000 and id_cod < 4000), '')  AS dest_uso, " & vbCrLf)

            stbSelect.Append(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  Reg_Impianti.Sup_Imp, " & vbCrLf)

            stbSelect.Append(" ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG,   " & vbCrLf)
            stbSelect.Append(" ISNULL(GruppoVarietale.grva_Des, '') AS grva_des,  " & vbCrLf)

            stbSelect.Append(" ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, " & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE," & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, " & vbCrLf)
            stbSelect.Append(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, " & vbCrLf)

            'PARTE DI APPEZZAMENTI
            stbSelectAppezza.Append(" 0 as Campo_Cod, '' AS CAMPO_Nome, " & vbCrLf)
            stbSelectAppezza.Append(" AppezzamentiXParticelle.AREA, AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            stbSelectAppezza.Append(" Appezzamento.Validita_Inizio AS Validita_Inizio, Appezzamento.Validita_Fine AS Validita_Fine " & vbCrLf)

            'PARTE DI CAMPI
            stbSelectCampi.Append(" Campi.Campo_Cod, Campi.Campo_des AS CAMPO_Nome, " & vbCrLf)
            stbSelectCampi.Append("  CampiXParticelle.AREA, CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, " & vbCrLf)
            stbSelectCampi.Append(" Campi.Validita_Inizio AS Validita_Inizio, Campi.Validita_Fine AS Validita_Fine " & vbCrLf)

            stbJoin.Append(" FROM Reg_Impianti " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod   " & vbCrLf)
            stbJoin.Append(" LEFT OUTER JOIN GruppoVarietale ON GruppoVarietale.grva_cod = Reg_Impianti.grva_cod_veg " & vbCrLf)

            stbJoin.Append(" INNER JOIN Centri_Aziendali ON Reg_Impianti.PIVA = Centri_Aziendali.PIVA AND Reg_Impianti.sa_cod = Centri_Aziendali.sa_cod " & vbCrLf)
            stbJoin.Append(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod " & vbCrLf)
            stbJoin.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo " & vbCrLf)
            stbJoin.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)

            stbJoin.Append(" INNER JOIN Appezzamento ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  " & vbCrLf)
            stbJoin.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA  " & vbCrLf)

            stbjoinAppezza.Append(" INNER JOIN AppezzamentiXParticelle ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND  " & vbCrLf)
            stbjoinAppezza.Append(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND  " & vbCrLf)
            stbjoinAppezza.Append(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA " & vbCrLf)
            stbjoinAppezza.Append(" INNER JOIN ParticelleCatastali ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.COM = AppezzamentiXParticelle.COM AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO AND  " & vbCrLf)
            stbjoinAppezza.Append(" ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  " & vbCrLf)
            stbjoinAppezza.Append(" INNER JOIN  ImpreseXParticelle " & vbCrLf)
            stbjoinAppezza.Append(" ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV AND ImpreseXParticelle.COM = AppezzamentiXParticelle.COM " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO " & vbCrLf)
            stbjoinAppezza.Append(" AND ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)

            stbjoinCampi.Append(" INNER JOIN Campi ON Appezzamento.PIVA = Campi.PIVA " & vbCrLf)
            stbjoinCampi.Append(" AND Appezzamento.SA_COD = Campi.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN CampiXParticelle ON CampiXParticelle.PIVA = Campi.PIVA " & vbCrLf)
            stbjoinCampi.Append(" AND CampiXParticelle.SA_COD = Campi.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND CampiXParticelle.Campo_Cod = Campi.Campo_Cod " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN ParticelleCatastali  ON ParticelleCatastali.PROV = CampiXParticelle.PROV " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.COM = CampiXParticelle.COM " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.sezione = CampiXParticelle.sezione " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.foglio = CampiXParticelle.foglio " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.numero = CampiXParticelle.numero " & vbCrLf)
            stbjoinCampi.Append(" AND ParticelleCatastali.subalterno = CampiXParticelle.subalterno " & vbCrLf)
            stbjoinCampi.Append(" INNER JOIN  ImpreseXParticelle " & vbCrLf)
            stbjoinCampi.Append(" ON ImpreseXParticelle.PIVA = CampiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = CampiXParticelle.SA_COD " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.PROV = CampiXParticelle.PROV AND ImpreseXParticelle.COM = CampiXParticelle.COM " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.SEZIONE = CampiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = CampiXParticelle.FOGLIO " & vbCrLf)
            stbjoinCampi.Append(" AND ImpreseXParticelle.NUMERO = CampiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = CampiXParticelle.SUBALTERNO " & vbCrLf)


            stbWhere.Append(" WHERE Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbWhere.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhere.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)
            stbWhere.Append(Str_FiltroImpianti & vbCrLf)

            stbWhereAppezza.Append(" AND AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereAppezza.Append(" AND AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereAppezza.Append(" AND Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereAppezza.Append(" AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereCampi.Append(" AND CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            stbWhereCampi.Append(" AND Campi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            stbWhereCampi.Append(" AND NOT EXISTS ( " & vbCrLf)
            stbWhereCampi.Append("      SELECT 1 " & vbCrLf)
            stbWhereCampi.Append("      FROM Appezzamento  " & vbCrLf)
            stbWhereCampi.Append("      INNER JOIN AppezzamentiXParticelle ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND " & vbCrLf)
            stbWhereCampi.Append("      AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            stbWhereCampi.Append("      AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA " & vbCrLf)
            stbWhereCampi.Append("      WHERE Appezzamento.Piva = Campi.piva " & vbCrLf)
            stbWhereCampi.Append("      AND Appezzamento.Sa_cod = Campi.sa_cod  " & vbCrLf)
            stbWhereCampi.Append("      AND Appezzamento.campo_cod = campi.campo_cod " & vbCrLf)
            stbWhereCampi.Append("  ) " & vbCrLf)


            '------------------------------------------------
            'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
            '------------------------------------------------
            stb.Append(" ( " & vbCrLf)
            stb.Append(stbSelect)
            stb.Append(stbSelectAppezza)
            stb.Append(stbJoin)
            stb.Append(stbjoinAppezza)
            stb.Append(stbWhere)
            stb.Append(stbWhereAppezza)
            stb.Append(" ) " & vbCrLf)

            stb.Append("  " & vbCrLf)
            stb.Append(" UNION ALL " & vbCrLf)
            stb.Append("  " & vbCrLf)

            '------------------------------------------------
            'PARTICELLE ASSOCIATE A CAMPI 
            '------------------------------------------------
            stb.Append(" ( " & vbCrLf)
            stb.Append(stbSelect)
            stb.Append(stbSelectCampi)
            stb.Append(stbJoin)
            stb.Append(stbjoinCampi)
            stb.Append(stbWhere)
            stb.Append(stbWhereCampi)
            stb.Append(" ) " & vbCrLf)

            stb.Append(" ORDER BY sa_nome, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
            'stb.Append(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class
