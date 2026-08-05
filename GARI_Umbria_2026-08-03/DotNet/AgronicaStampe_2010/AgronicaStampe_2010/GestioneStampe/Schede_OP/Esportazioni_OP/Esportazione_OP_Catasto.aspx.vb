Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class Esportazione_OP_Catasto
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private Qs_Data As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Qs_Data = Stringa_Decodifica(Request.QueryString("data").ToString, _
                               AgroKey_EncoderDecoder, _
                               Server)

        Dim Messaggio As String = ""
        Dim Dt As New DataTable
        Dim strFiltroImpianti As String
        Dim i As Integer
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim strXmlVariabilistampe As String
        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Dim Query1_TempTableCreazione As String = ""
        Dim Query2_TempTableIndice As String = ""
        Dim Query3_TempTableFill As String = ""
        Dim Query4_TempTableJoin As String = ""
        Dim stbQ As New System.Text.StringBuilder

        If XmlDoc.HasChildNodes Then

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(XML_VariabiliStampe.GetAttribute("piva")) + "'," + Agro_SQL_SaveNum(XML_VariabiliStampe.GetAttribute("sa_cod")) + "," + Agro_SQL_SaveNum(XML_VariabiliStampe.GetAttribute("appezza")) + "," + Agro_SQL_SaveNum(XML_VariabiliStampe.GetAttribute("id_reg")) + ")  " & vbCrLf

            Next


            If Query3_TempTableFill <> "" Then

                Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                Query1_TempTableCreazione += "   INTO #tempimpianti   "
                Query1_TempTableCreazione += "       FROM Reg_Impianti "
                Query1_TempTableCreazione += "           WHERE 1 = 0   "
                Query1_TempTableCreazione += vbCrLf

                Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "

                '----------------------------------------------------------
                stbQ.AppendLine(" SELECT DISTINCT CASE WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Impresa.partitaIvaReale END AS Piva ")
                stbQ.AppendLine(" , ISNULL(Imprese.rag_soc, ' ') AS rag_soc ")
                stbQ.AppendLine(" ,  '' AS Tipo_Socio ")
                stbQ.AppendLine(" ,  ISNULL(Imprese_1.Piva, ' ') AS PIVA_padre ")
                stbQ.AppendLine(" ," & CDate(Qs_Data).Year.ToString & " AS Anno ")
                stbQ.AppendLine(", ISNULL(Codifica_SpecieVegetali_Dogane.NC_Cod, ' ') AS Codice_Prodotto ")
                stbQ.AppendLine(", ISNULL(Codifica_SpecieVegetali_Dogane.NC_Des, ' ') AS Descrizione_Prodotto ")
                '(05/10/2015 fede) spostata lettura codici x evitare doppioni righe (es. pesco mappato con 3 codici specie)
                'stbQ.Append(", ISNULL(xDecoder_Specie_OP2007.Specie_Cod, ' ') AS Codice_Specie ")
                'stbQ.Append(", ISNULL(xDecoder_Specie_OP2007.Specie_Des, ' ') AS Descrizione_Specie ")
                'stbQ.Append(", ISNULL(xDecoder_Varieta_OP2007.Varieta_Cod, ' ') AS Codice_Varieta ")
                'stbQ.Append(", ISNULL(xDecoder_Varieta_OP2007.Varieta_Des, ' ') AS Descrizione_Varieta ")
                stbQ.AppendLine(", ' ' AS Codice_Specie ")
                stbQ.AppendLine(", ' ' AS Descrizione_Specie ")
                stbQ.AppendLine(", ' ' AS Codice_Varieta ")
                stbQ.AppendLine(", ' ' AS Descrizione_Varieta ")

                stbQ.AppendLine(", cultivar.CUL_COD,cultivar.veg_cod ")

                stbQ.AppendLine(", ISNULL(GruppoVegetale.Gru_Des,' ') AS Gru_Des ")

                stbQ.AppendLine(", ISNULL(year(Reg_Impianti.Validita_Inizio), ' ') AS anno_impianto ")

                stbQ.AppendLine(", ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod  ")
                stbQ.AppendLine("                FROM Reg_Impianti_Codici  ")
                stbQ.AppendLine("                WHERE(Reg_Impianti_Codici.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_TraFila_Maschio) + ") " & vbCrLf)
                stbQ.AppendLine(" ) , 0)   ")
                stbQ.AppendLine(" + ' x ' + ")
                stbQ.AppendLine(" ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod  ")
                stbQ.AppendLine("                FROM Reg_Impianti_Codici  ")
                stbQ.AppendLine("                WHERE(Reg_Impianti_Codici.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0)  ")
                stbQ.AppendLine(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_SuFila_Maschio) + ") " & vbCrLf)
                stbQ.AppendLine(" ) , 0)  AS Sesto, ")

                stbQ.AppendLine(" ROUND(ISNULL(( SELECT TOP 1  Imprese_Progetti.P_HA  ")
                stbQ.AppendLine("                FROM Imprese_Progetti  ")
                stbQ.AppendLine("                WHERE(Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine(" AND     Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine(" AND     Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine(" ) , 0) * ISNULL(AppezzamentiXParticelle.AREA, 0),0) AS N_piante  ")

                stbQ.AppendLine(", ISNULL(( SELECT    TOP 1 Regolamenti.Reg_Des  ")
                stbQ.AppendLine("                FROM Imprese_Progetti  ")
                stbQ.AppendLine("             INNER JOIN  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod  ")
                stbQ.AppendLine("                WHERE(Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine("           AND     Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine("           AND     Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine(" ) , ' ') AS Regolamento  ")

                stbQ.AppendLine(",  ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des ")
                stbQ.AppendLine(",  ISNULL(Copertura.Cop_Des, 'Nessuna') AS cop_des ")

                stbQ.AppendLine(", CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato' ")
                stbQ.AppendLine("                                        WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
                stbQ.AppendLine("                                        WHEN 5 THEN 'In conto terzi' ELSE 'Altro' END AS titolo_possesso ")

                stbQ.AppendLine(", ISNULL(( SELECT    TOP 1 CONVERT(varchar(10), Imprese_Progetti.Validita_Inizio, 103) ")
                stbQ.AppendLine("                FROM Imprese_Progetti  ")
                stbQ.AppendLine("                WHERE(Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine("               AND     Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine("               AND     Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine(" ) , ' ') AS inizio_esercizio   ")
                stbQ.AppendLine(", ISNULL(( SELECT    TOP 1 CONVERT(varchar(10), Imprese_Progetti.Validita_fine, 103) ")
                stbQ.AppendLine("                FROM Imprese_Progetti  ")
                stbQ.AppendLine("                WHERE(Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)  ")
                stbQ.AppendLine("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)  ")
                stbQ.AppendLine("           AND     Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine("           AND     Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Qs_Data) & "   ")
                stbQ.AppendLine(" ) , ' ') AS fine_esercizio   ")

                stbQ.AppendLine(", ISNULL(( SELECT     TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des " & vbCrLf)
                stbQ.AppendLine(" FROM     Reg_Impianti_Codici " & vbCrLf)
                stbQ.AppendLine(" INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod" & vbCrLf)
                stbQ.AppendLine(" WHERE  (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
                stbQ.AppendLine(" AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
                stbQ.AppendLine(" AND (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
                stbQ.AppendLine(" AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
                stbQ.AppendLine(" AND (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
                stbQ.AppendLine(" AND (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) + ") " & vbCrLf)
                stbQ.AppendLine(" AND (CAC_Codifica_InfoAggiuntive.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & "') " & vbCrLf)
                stbQ.AppendLine(" AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 2) " & vbCrLf)
                stbQ.AppendLine(" ) , '') AS dett_specie_pers, " & vbCrLf)

                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.SEZIONE, ' ') AS Sezione,  ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.FOGLIO, ' ') AS Foglio,  ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.NUMERO, ' ') AS Particella,  ")
                stbQ.AppendLine(" ISNULL(AppezzamentiXParticelle.SUBALTERNO, ' ') AS Sub,  ")

                stbQ.AppendLine("  CAST(convert(float, (cast (isnull(ParticelleCatastali.ETTARI,0) as varchar(100))+ '.' + " & vbCrLf)
                stbQ.AppendLine("     right('000' + cast (isnull(ParticelleCatastali.Are,0) as varchar(100)), 2)  +  " & vbCrLf)
                stbQ.AppendLine("     right('000' + cast (isnull(ParticelleCatastali.CentiAre,0) as varchar(100)), 2))    " & vbCrLf)
                stbQ.AppendLine("     ) As decimal(18,4))   as Sup_Cat,  " & vbCrLf)

                'stbQ.Append(" ISNULL(ParticelleCatastali.ETTARI, 0) AS ETTARI,  ")
                'stbQ.Append(" ISNULL(ParticelleCatastali.are, 0) AS ARE,  ")
                'stbQ.Append(" ISNULL(ParticelleCatastali.centiare, 0) AS CENTIARE,  ")
                stbQ.AppendLine(" CAST(ISNULL(AppezzamentiXParticelle.AREA, 0) As decimal(18,4))  AS AREA, ")
                'stbQ.Append(" cast(CAST(ISNULL(AppezzamentiXParticelle.AREA, 0) As decimal(18,4)) as varchar(100) )  AS AREA, ")

                stbQ.AppendLine(" ISNULL(istat.localita, ' ') AS COMUNE,  ")
                stbQ.AppendLine(" ISNULL(istat.comuni_prov, ' ') AS PROVINCIA, ")

                stbQ.AppendLine(" ISNULL(IndCentro.ind_des, ' ') AS indirizzo ")
                'resa ha
                stbQ.AppendLine(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
                stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
                stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
                stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
                stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
                stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Qs_Data) & " " & vbCrLf)
                stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Qs_Data) & " " & vbCrLf)
                stbQ.AppendLine("), 0)/1000 AS ResaHa ")

                'resa x util
                stbQ.AppendLine(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
                stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
                stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
                stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
                stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
                stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Qs_Data) & " " & vbCrLf)
                stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Qs_Data) & " " & vbCrLf)
                stbQ.AppendLine("), 0)/1000 * ISNULL(AppezzamentiXParticelle.AREA, 0) AS ResaxUtil ")

                stbQ.AppendLine("   FROM Reg_Impianti ")
                stbQ.AppendLine("  INNER JOIN Imprese Impresa ")
                stbQ.AppendLine("  ON Reg_Impianti.Piva = Impresa.Piva  ")
                stbQ.AppendLine("  LEFT OUTER JOIN AppezzamentiXParticelle  ")
                stbQ.AppendLine("  ON Reg_Impianti.Piva = AppezzamentiXParticelle.Piva  ")
                stbQ.AppendLine("  And Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod  ")
                stbQ.AppendLine("  And Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza  ")
                stbQ.AppendLine("  LEFT OUTER JOIN ParticelleCatastali  ")
                stbQ.AppendLine("  ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV  ")
                stbQ.AppendLine("  And ParticelleCatastali.COM = AppezzamentiXParticelle.COM  ")
                stbQ.AppendLine("  And ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE  ")
                stbQ.AppendLine("  And ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO  ")
                stbQ.AppendLine("  And ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO  ")
                stbQ.AppendLine("  And ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO  ")

                stbQ.AppendLine("  LEFT OUTER JOIN Istat  ")
                stbQ.AppendLine("  on ParticelleCatastali.PROV = istat.prov And ParticelleCatastali.com = istat.com  ")

                stbQ.AppendLine("  LEFT OUTER JOIN Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod  ")
                stbQ.AppendLine("  LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod  ")
                stbQ.AppendLine("  LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod  ")
                stbQ.AppendLine("  LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")
                stbQ.AppendLine("  LEFT OUTER JOIN SpecieVegetali  ")
                stbQ.AppendLine("  ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
                stbQ.AppendLine("  LEFT OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod  ")
                stbQ.AppendLine("  INNER JOIN Imprese ON Reg_Impianti.Piva = Imprese.Piva  ")

                stbQ.AppendLine(" INNER JOIN CentrixIndirizzi " & vbCrLf)
                stbQ.AppendLine(" ON Reg_Impianti.Piva = CentrixIndirizzi.Piva " & vbCrLf)
                stbQ.AppendLine(" And Reg_Impianti.sa_cod = CentrixIndirizzi.sa_cod " & vbCrLf)
                stbQ.AppendLine(" INNER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo " & vbCrLf)

                stbQ.AppendLine("  LEFT OUTER JOIN Imprese Imprese_1  ")
                stbQ.AppendLine("  INNER JOIN GerarchiaImprese ON Imprese_1.Piva = GerarchiaImprese.Padre ON Imprese.Piva = GerarchiaImprese.Figlio ")

                stbQ.AppendLine("   INNER JOIN  #tempimpianti  ")
                stbQ.AppendLine("   ON #tempimpianti.Piva = Reg_Impianti.Piva And Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod And  Reg_Impianti.Appezza = #tempimpianti.Appezza And  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg ")

                stbQ.AppendLine("   LEFT OUTER JOIN  ImpreseXParticelle ON AppezzamentiXParticelle.PIVA = ImpreseXParticelle.PIVA And AppezzamentiXParticelle.SA_COD = ImpreseXParticelle.sa_cod And   ")
                stbQ.AppendLine("   AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV And AppezzamentiXParticelle.COM = ImpreseXParticelle.COM And  ")
                stbQ.AppendLine("   AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE And AppezzamentiXParticelle.FOGLIO = ImpreseXParticelle.FOGLIO And  ")
                stbQ.AppendLine("   AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO And  ")
                stbQ.AppendLine("   AppezzamentiXParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO  ")

                'stbQ.Append("   LEFT OUTER JOIN xDecoder_Varieta_OP2007 ON Cultivar.Cul_Cod = xDecoder_Varieta_OP2007.Cul_Cod LEFT OUTER JOIN ")
                'stbQ.Append("   xDecoder_Specie_OP2007 ON SpecieVegetali.Veg_Cod = xDecoder_Specie_OP2007.Veg_Cod LEFT OUTER JOIN ")
                stbQ.AppendLine("   LEFT OUTER JOIN  Codifica_SpecieVegetali_Dogane ON SpecieVegetali.Veg_Cod = Codifica_SpecieVegetali_Dogane.Veg_cod ")

                ' stbQ.Append("   ORDER BY coop_padre Asc ")

                Query4_TempTableJoin = stbQ.ToString

                '/**************************************************
                '         NUOVO METODO CON TABELLA TEMPORANEA  
                Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
                Dt = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione,
                                                                        Query2_TempTableIndice,
                                                                        Query3_TempTableFill,
                                                                        Query4_TempTableJoin,
                                                                         objParametri_Server.StringaConnessione,
                                                                         Messaggio)

                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    '--------------------------------------------------------------
                    'assegna le codifiche specie e varieta OP
                    Dim objCodificheOP As New AgronicaCoreMetaSchemaDAL.Tabelle_OP_R
                    Dim DtSpecieOP As DataTable
                    Dim DtVarietaOP As DataTable
                    Dim strVarietaNONMappate As String
                    Dim Flag_VarietaValorizzata As Boolean = False
                    Dim Flag_SpecieValorizzata As Boolean = False

                    DtSpecieOP = objCodificheOP.xDecoder_Specie_OP2007_Leggi(0, 0, "", 0, "", 0, "", 0, "", "", "", objParametri_Server)
                    DtVarietaOP = objCodificheOP.xDecoder_Varieta_OP2007_Leggi_2(0, 0, "", 0, "", 0, "", CDate(Qs_Data), "", "", objParametri_Server)

                    For i = 0 To Dt.Rows.Count - 1

                        If Not IsNothing(Dt.Rows(i).Item("cul_cod")) AndAlso Not IsDBNull(Dt.Rows(i).Item("cul_cod")) Then
                            Flag_VarietaValorizzata = True
                        Else
                            Flag_VarietaValorizzata = False
                        End If
                        If Not IsNothing(Dt.Rows(i).Item("veg_cod")) AndAlso Not IsDBNull(Dt.Rows(i).Item("veg_cod")) Then
                            Flag_SpecieValorizzata = True
                        Else
                            Flag_SpecieValorizzata = False
                        End If

                        If Flag_VarietaValorizzata = True Or Flag_SpecieValorizzata = True Then

                            If Flag_VarietaValorizzata = True Then

                                Dim DrVarieta() As DataRow = DtVarietaOP.Select("cul_cod=" & Dt.Rows(i).Item("cul_cod"))
                                If Not DrVarieta Is Nothing AndAlso DrVarieta.Length > 0 Then
                                    Dt.Rows(i).Item("Codice_Specie") = DrVarieta(0).Item("Specie_Cod")
                                    Dt.Rows(i).Item("Descrizione_Specie") = DrVarieta(0).Item("Specie_Des")
                                    Dt.Rows(i).Item("Codice_Varieta") = DrVarieta(0).Item("Varieta_Cod")
                                    Dt.Rows(i).Item("Descrizione_Varieta") = DrVarieta(0).Item("Varieta_Des")
                                Else
                                    If Flag_SpecieValorizzata = True Then
                                        Dim DrSpecie() As DataRow = DtSpecieOP.Select("veg_cod=" & Dt.Rows(i).Item("veg_cod"))
                                        If Not DrSpecie Is Nothing AndAlso DrSpecie.Length > 0 Then
                                            Dt.Rows(i).Item("Codice_Specie") = DrSpecie(0).Item("Specie_Cod")
                                            Dt.Rows(i).Item("Descrizione_Specie") = DrSpecie(0).Item("Specie_Des")
                                            Dt.Rows(i).Item("Codice_Varieta") = 999
                                            Dt.Rows(i).Item("Descrizione_Varieta") = "ALTRE VARIETA"
                                        End If
                                        strVarietaNONMappate &= "veg_cod=" & Dt.Rows(i).Item("veg_cod") & " - cul_cod=" & Dt.Rows(i).Item("cul_cod") & vbCrLf
                                    End If
                                End If
                            Else
                                'varietà non presente
                                If Flag_SpecieValorizzata = True Then
                                    Dim DrSpecie() As DataRow = DtSpecieOP.Select("veg_cod=" & Dt.Rows(i).Item("veg_cod"))
                                    If Not DrSpecie Is Nothing AndAlso DrSpecie.Length > 0 Then
                                        Dt.Rows(i).Item("Codice_Specie") = DrSpecie(0).Item("Specie_Cod")
                                        Dt.Rows(i).Item("Descrizione_Specie") = DrSpecie(0).Item("Specie_Des")
                                        Dt.Rows(i).Item("Codice_Varieta") = 999
                                        Dt.Rows(i).Item("Descrizione_Varieta") = "ALTRE VARIETA"
                                    End If
                                    strVarietaNONMappate &= "veg_cod=" & Dt.Rows(i).Item("veg_cod") & " - cul_cod=" & Dt.Rows(i).Item("cul_cod") & vbCrLf
                                End If
                            End If
                        Else
                            Dt.Rows(i).Item("Codice_Specie") = ""
                            Dt.Rows(i).Item("Descrizione_Specie") = ""
                            Dt.Rows(i).Item("Codice_Varieta") = ""
                            Dt.Rows(i).Item("Descrizione_Varieta") = ""
                        End If

                    Next

                    'rimuovo le colonne cul_cod, veg_cod gias
                    Dt.Columns.RemoveAt(11)
                    Dt.Columns.RemoveAt(12)


                    For i = 0 To Dt.Columns.Count - 1
                        Select Case Dt.Columns(i).ColumnName.ToLower
                            Case "rag_soc".ToLower
                                Dt.Columns(i).ColumnName = "Ragione Sociale Impresa"
                            Case "piva".ToLower
                                Dt.Columns(i).ColumnName = "Partita Iva Impresa"
                            Case "tipo_socio".ToLower
                                Dt.Columns(i).ColumnName = "Tipo Impresa"
                            Case "piva_padre".ToLower
                                Dt.Columns(i).ColumnName = "Partita Iva Padre"
                            Case "codice_prodotto".ToLower
                                Dt.Columns(i).ColumnName = "Prodotto"
                            Case "codice_specie".ToLower
                                Dt.Columns(i).ColumnName = "Specie"
                            Case "codice_varieta".ToLower
                                Dt.Columns(i).ColumnName = "Varieta'"
                Case "gru_des".ToLower
                                Dt.Columns(i).ColumnName = "Tipo Prodotto"
                            Case "n_piante".ToLower
                                Dt.Columns(i).ColumnName = "Numero Piante"
                            Case "regolamento".ToLower
                                Dt.Columns(i).ColumnName = "Tipologia Produzione"
                            Case "grfi_des".ToLower
                                Dt.Columns(i).ColumnName = "Tipologia Prodotto"
                            Case "cop_des".ToLower
                                Dt.Columns(i).ColumnName = "Tipologia Coltivazione"
                            Case "titolo_possesso".ToLower
                                Dt.Columns(i).ColumnName = "Titolo Possesso"
                            Case "inizio_esercizio".ToLower
                                Dt.Columns(i).ColumnName = "Impegnativa dal"
                            Case "fine_esercizio".ToLower
                                Dt.Columns(i).ColumnName = "Impegnativa al"
                            Case "dett_specie_pers".ToLower
                                Dt.Columns(i).ColumnName = "N° Ciclo"
                            Case "area".ToLower
                                Dt.Columns(i).ColumnName = "Superficie Coltivata [ha]"
                            Case "sup_cat".ToLower
                                Dt.Columns(i).ColumnName = "Superficie Catastale [ha]"
                            Case "Descrizione_Prodotto".ToLower
                                Dt.Columns(i).ColumnName = "Descrizione Prodotto"
                            Case "Descrizione_Specie".ToLower
                                Dt.Columns(i).ColumnName = "Descrizione Specie"
                            Case "Descrizione_varieta".ToLower
                                Dt.Columns(i).ColumnName = "Descrizione Varieta"
                            Case "anno_impianto".ToLower
                                Dt.Columns(i).ColumnName = "Anno Impianto"
                            Case "ResaHA".ToLower
                                Dt.Columns(i).ColumnName = "Resa/Ha [t]"
                            Case "ResaxUtil".ToLower
                                Dt.Columns(i).ColumnName = "Resa x Utilizzo [t]"
                        End Select
                    Next



                    AgronicaCoreGestioneRichieste.Esporta.EsportaExcel(Dt, "EsportazioneOPCatasto", Page)

                End If

            End If

        End If

    End Sub

End Class