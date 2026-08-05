Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class ReportAbilitazioni
    Inherits System.Web.UI.Page

    Private rptStampaAbilitazioneXSpecie As Rpt_AbilitazioniXSpecie
    Private rptStampaAbilitazioneXSocio As Rpt_AbilitazioniXSocio
    Private rptStampaAbilitazioneXCapitolato As Rpt_AbilitazioneXCapitolato
    Private DSAbilitazioni As DS_Abilitazioni

    '----- Gestione Querystring
    Dim Piva, veg_cod, grva_cod, cul_cod, anno, validita_inizio, validita_fine, LinkPaginaStampa, stringa_lista_pive, tipologiaReport As String

    Dim lista_pive As New List(Of String)

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampaAbilitazioneXSpecie = New Rpt_AbilitazioniXSpecie
        rptStampaAbilitazioneXSocio = New Rpt_AbilitazioniXSocio
        rptStampaAbilitazioneXCapitolato = New Rpt_AbilitazioneXCapitolato

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim Log As String

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

        stringa_lista_pive = Stringa_Decodifica(Request.QueryString("lp").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        If stringa_lista_pive <> "" Then
            lista_pive = stringa_lista_pive.Split(","c).ToList()
        End If

        veg_cod = Stringa_Decodifica(Request.QueryString("v").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        grva_cod = Stringa_Decodifica(Request.QueryString("g").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        cul_cod = Stringa_Decodifica(Request.QueryString("cc").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        Dim tr = Stringa_Decodifica(Request.QueryString("tr").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        Select Case tr
            Case "0"
                tipologiaReport = "xSpecie"
            Case "1"
                tipologiaReport = "xSocio"
            Case "2"
                tipologiaReport = "xCapitolato"
        End Select

        If Not IsNothing(Request.QueryString("vi")) Then
            validita_inizio = Stringa_Decodifica(Request.QueryString("vi").ToString,
                      AgroKey_EncoderDecoder, Server)
        Else
            validita_inizio = "01/01/" + CStr(anno)
        End If

        If Not IsNothing(Request.QueryString("vf")) Then
            validita_fine = Stringa_Decodifica(Request.QueryString("vf").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            validita_fine = "31/12/" + CStr(anno)
        End If

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            'carico i dati nel datatable 

            'INTESTAZIONE

            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                DSAbilitazioni = New DS_Abilitazioni

                Carica_DSAbilitazioni(lista_pive, DSAbilitazioni, objParametri_Server)

                rptStampaAbilitazioneXSpecie.SetDataSource(DSAbilitazioni)
                rptStampaAbilitazioneXSocio.SetDataSource(DSAbilitazioni)
                rptStampaAbilitazioneXCapitolato.SetDataSource(DSAbilitazioni)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try

            ' imposto il parametro del fronte-retro
            'rptStampa.SetParameterValue("fronteRetro", fronteRetro)

            rptStampaAbilitazioneXSpecie.SetParameterValue("FiltroRicerca", "")
            rptStampaAbilitazioneXSocio.SetParameterValue("FiltroRicerca", "")
            rptStampaAbilitazioneXCapitolato.SetParameterValue("FiltroRicerca", "")

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R


            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Stampa_Abilitazioni, "", "", objParametri_Server)

            objCatDoc = Nothing

            Dim Nome_Documento As String = "Stampa_Abilitazioni"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile

            Select Case tipologiaReport
                Case "xSpecie"
                    objGestFile.SalvaReportPdf(rptStampaAbilitazioneXSpecie,
                                   enum_CategorieDocumenti.Stampa_Abilitazioni,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + "anno" + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
                Case "xSocio"
                    objGestFile.SalvaReportPdf(rptStampaAbilitazioneXSocio,
                                   enum_CategorieDocumenti.Stampa_Abilitazioni,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + "anno" + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
                Case "xCapitolato"
                    objGestFile.SalvaReportPdf(rptStampaAbilitazioneXCapitolato,
                                   enum_CategorieDocumenti.Stampa_Abilitazioni,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + "anno" + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            End Select

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                         enum_CategorieDocumenti.Stampa_Abilitazioni,
                                                         "Stampa_Abilitazioni",
                                                         Nome_Documento + "_p" + Piva + "_d" + "anno" + ".pdf",
                                                         Sottocartella,
                                                         "", "", "", "",
                                                         CDate("01/01/" & "1900"),
                                                         CDate("31/12/" & "2100"),
                                                         objParametri_Server)

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + "anno" + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "Stampa_Abilitazioni"
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_OP"
                End If

                Dim CustomLOGParams As New CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = Path_Errore,
                    .LogFileName = Nome_File
                }

                'objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Qs_Piva.Page_Load", Log_Errori)
                objLog.Scrivi_LOG(objParametri_Server, "Qs_Piva.Page_Load", Log_Errori, CustomLOGParams:=CustomLOGParams)

            End If
            '-----------------------------------------

            Select Case tipologiaReport
                Case "xSpecie"
                    Session("Report") = rptStampaAbilitazioneXSpecie
                Case "xSocio"
                    Session("Report") = rptStampaAbilitazioneXSocio
                Case "xCapitolato"
                    Session("Report") = rptStampaAbilitazioneXCapitolato
            End Select

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        End If



    End Sub

    '#########################################################################################################
    Private Sub Carica_DSAbilitazioni(ByVal listaPiva As List(Of String), ByRef DSAbilitazioni As DS_Abilitazioni, objParametri As AgronicaCoreParametri)

        Dim stbQ As New System.Text.StringBuilder

        Dim NomeRoutine = "AgronicaStampe_2010.Abilitazioni.Carica_DSAbilitazioni()"

        Dim FiltraPive As Boolean = True

        Try

            stbQ.Length = 0

            If listaPiva Is Nothing OrElse listaPiva.Count = 0 Then
                FiltraPive = False
            End If

            If FiltraPive Then
                ConnessioniTransazioni.ApriConnessione(True, objParametri)

                TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(listaPiva, NomeRoutine, objParametri)
            End If

            stbQ.AppendLine("    WITH lsb AS ( ")
            stbQ.AppendLine("        SELECT     ")
            stbQ.AppendLine("              PDC_Sblocca.Piva ")
            stbQ.AppendLine("            , PDC_Sblocca.Sa_Cod ")
            stbQ.AppendLine("            , PDC_Sblocca.Appezza ")
            stbQ.AppendLine("            , PDC_Sblocca.Id_Reg ")
            stbQ.AppendLine("            , PDC_Sblocca.CapitolatoCliente_Cod ")
            stbQ.AppendLine("            , PDC_Sblocca.Esito ")
            stbQ.AppendLine("            , p.analisi_testata_des COLLATE latin1_general_ci_as AS analisi_testata_des ")
            stbQ.AppendLine("            , CASE  ")
            stbQ.AppendLine("                WHEN p.Analisi_Testata_Data_Inizio = '01/01/1900'  ")
            stbQ.AppendLine("                    THEN ''  ")
            stbQ.AppendLine("                ELSE COALESCE(CONVERT(VARCHAR(100), p.Analisi_Testata_Data_inizio, 103), '')  ")
            stbQ.AppendLine("              END COLLATE latin1_general_ci_as AS Analisi_Testata_Data_inizio  ")
            stbQ.AppendLine("            , CASE  ")
            stbQ.AppendLine("                WHEN p.Analisi_Testata_Data_Fine = '31/12/2100'  ")
            stbQ.AppendLine("                    THEN ''  ")
            stbQ.AppendLine("                ELSE COALESCE(CONVERT(VARCHAR(100), p.Analisi_Testata_Data_Fine, 103), '')  ")
            stbQ.AppendLine("              END COLLATE latin1_general_ci_as AS Analisi_Testata_Data_Fine ")
            stbQ.AppendLine("            , PDC_Sblocca.ID_PDC_Testata ")
            stbQ.AppendLine("            , lfo.lfo_des ")
            stbQ.AppendLine("            , PDC_Sblocca.PivaSuperUser ")
            stbQ.AppendLine("        FROM PDC_Sblocca  ")
            stbQ.AppendLine("        INNER JOIN PDC_Dettagli  ")
            stbQ.AppendLine("            ON PDC_Sblocca.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata  ")
            stbQ.AppendLine("            AND PDC_Sblocca.PivaSuperUser = PDC_Dettagli.PivaSuperUser  ")
            stbQ.AppendLine("            AND PDC_Sblocca.Piva = PDC_Dettagli.Piva  ")
            stbQ.AppendLine("            AND PDC_Sblocca.Sa_Cod = PDC_Dettagli.Sa_Cod  ")
            stbQ.AppendLine("            AND PDC_Sblocca.Appezza = PDC_Dettagli.Appezza  ")
            stbQ.AppendLine("            AND PDC_Sblocca.Id_Reg = PDC_Dettagli.Id_Reg ")
            stbQ.AppendLine("            AND PDC_Sblocca.Esito <> 0 ")
            stbQ.AppendLine("        LEFT JOIN PDC_LFO lfo  ")
            stbQ.AppendLine("            ON lfo.PivaSuperUser = PDC_Dettagli.PivaSuperUser ")
            stbQ.AppendLine("            AND lfo.ID_LFO = PDC_Dettagli.ID_LFO  ")
            stbQ.AppendLine("            AND lfo.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata ")
            stbQ.AppendLine("        LEFT JOIN ( ")
            stbQ.AppendLine("            SELECT     ")
            stbQ.AppendLine("                  A_T.Analisi_Testata_Des ")
            stbQ.AppendLine("                , A_T.Analisi_Testata_Data_inizio ")
            stbQ.AppendLine("                , A_T.Analisi_testata_data_fine ")
            stbQ.AppendLine("                , PDC_D.ID_LFO ")
            stbQ.AppendLine("                , PDC_D.ID_PDC_Testata ")
            stbQ.AppendLine("            FROM PDC_Analisi PDC_A  ")
            stbQ.AppendLine("            INNER JOIN PDC_Campioni PDC_C  ")
            stbQ.AppendLine("                ON PDC_A.PivaSuperUser = PDC_C.PivaSuperUser  ")
            stbQ.AppendLine("                AND PDC_A.ID_PDC_Testata = PDC_C.ID_PDC_Testata  ")
            stbQ.AppendLine("                AND PDC_A.ID_PDC_Dettagli = PDC_C.ID_PDC_Dettagli  ")
            stbQ.AppendLine("                AND PDC_A.ID_PDC_Campione = PDC_C.ID_PDC_Campione  ")
            stbQ.AppendLine("            INNER JOIN PDC_Dettagli PDC_D  ")
            stbQ.AppendLine("                ON PDC_C.PivaSuperUser = PDC_D.PivaSuperUser  ")
            stbQ.AppendLine("                AND PDC_C.ID_PDC_Testata = PDC_D.ID_PDC_Testata  ")
            stbQ.AppendLine("                AND PDC_C.ID_PDC_Dettagli = PDC_D.ID_PDC_Dettagli  ")
            stbQ.AppendLine("            INNER JOIN Analisi_Testata A_T  ")
            stbQ.AppendLine("                ON PDC_A.Analisi_Testata_Cod = A_T.Analisi_Testata_Cod  ")
            stbQ.AppendLine("            WHERE Mostra_in_Stampe <> 0 ")
            stbQ.AppendLine("        ) p  ")
            stbQ.AppendLine("            ON PDC_Dettagli.ID_LFO = p.ID_LFO ")
            stbQ.AppendLine("            AND PDC_Dettagli.ID_PDC_Testata = p.ID_PDC_Testata ")
            stbQ.AppendLine("),  ")
            stbQ.AppendLine("FilteredBase AS (  ")
            stbQ.AppendLine("    SELECT  ")
            stbQ.AppendLine("         sb.PivaSuperUser,  ")
            stbQ.AppendLine("         sb.Piva,  ")
            stbQ.AppendLine("         sb.Sa_Cod,  ")
            stbQ.AppendLine("         sb.Appezza,  ")
            stbQ.AppendLine("         sb.Id_Reg,  ")
            stbQ.AppendLine("         sb.Id_Pdc_Testata,  ")
            stbQ.AppendLine("         sb.CapitolatoCliente_Cod,  ")
            stbQ.AppendLine("         det.PIVA AS Det_PIVA,  ")
            stbQ.AppendLine("         det.rag_soc,  ")
            stbQ.AppendLine("         det.sa_nome,  ")
            stbQ.AppendLine("         det.APP_NOME,  ")
            stbQ.AppendLine("         det.Veg_Cod,  ")
            stbQ.AppendLine("         det.Cul_Cod,  ")
            stbQ.AppendLine("         det.GRVA_Cod,  ")
            stbQ.AppendLine("         lsb.analisi_testata_des AS Analisi_Testata_Des,  ")
            stbQ.AppendLine("         lsb.Analisi_Testata_Data_Fine,  ")
            stbQ.AppendLine("         tes.PDC_Data_istantanea,  ")
            stbQ.AppendLine("         tes.Validita_Inizio,  ")
            stbQ.AppendLine("         tes.Validita_Fine,  ")
            stbQ.AppendLine("         veg.veg_Des,  ")
            stbQ.AppendLine("         c.cul_Des,  ")
            stbQ.AppendLine("         grva.grva_des,  ")
            stbQ.AppendLine("         IC.val_cod AS CodiceSocio,  ")
            stbQ.AppendLine("         'LFO_' + COALESCE(IC.val_Cod, '') + '_' +  ")
            stbQ.AppendLine("             COALESCE(det.rag_soc, '') + '_' +  ")
            stbQ.AppendLine("             COALESCE(det.sa_nome, '') + '_' +  ")
            stbQ.AppendLine("             COALESCE(veg.veg_Des, '') + '_' +  ")
            stbQ.AppendLine("             c.cul_Des AS LFO,  ")
            stbQ.AppendLine("         CAST(CASE  ")
            stbQ.AppendLine("             WHEN el.capitolato_DES_Breve <> '' THEN el.capitolato_DES_Breve  ")
            stbQ.AppendLine("             ELSE el.capitolato_DES  ")
            stbQ.AppendLine("         END AS VARCHAR(8000)) AS capitolato_des,  ")
            stbQ.AppendLine("         ' (Analisi: ' +  ")
            stbQ.AppendLine("             COALESCE(  ")
            stbQ.AppendLine("                 CAST(lsb.analisi_testata_des AS VARCHAR(1000)) + ', ' +  ")
            stbQ.AppendLine("                 CONVERT(NVARCHAR(30), lsb.Analisi_Testata_Data_Fine, 103),  ")
            stbQ.AppendLine("             ' - ') + ')' AS DettagliAnalisi  ")
            stbQ.AppendLine("     FROM PDC_Sblocca sb      ")

            If FiltraPive Then
                stbQ.AppendLine("            INNER JOIN #TempPiva tmp ON tmp.Piva = sb.Piva COLLATE DATABASE_DEFAULT         ")
            End If

            stbQ.AppendLine("     INNER JOIN capitolatoCliente el  ")
            stbQ.AppendLine("         ON sb.CapitolatoCliente_Cod = el.Capitolato_COD  ")
            stbQ.AppendLine("     INNER JOIN PDC_dettagli det  ")
            stbQ.AppendLine("         ON sb.ID_PDC_Testata = det.ID_PDC_Testata  ")
            stbQ.AppendLine("         AND sb.PivaSuperUser = det.PivaSuperUser  ")
            stbQ.AppendLine("         AND sb.Piva = det.Piva  ")
            stbQ.AppendLine("         AND sb.Sa_Cod = det.Sa_Cod  ")
            stbQ.AppendLine("         AND sb.Appezza = det.Appezza  ")
            stbQ.AppendLine("         AND sb.Id_Reg = det.Id_Reg  ")
            stbQ.AppendLine("     INNER JOIN lsb  ")
            stbQ.AppendLine("         ON det.PIVA = lsb.PIVA  ")
            stbQ.AppendLine("         AND det.SA_COD = lsb.SA_COD  ")
            stbQ.AppendLine("         AND det.APPEZZA = lsb.APPEZZA  ")
            stbQ.AppendLine("         AND det.ID_REG = lsb.Id_Reg  ")
            stbQ.AppendLine("         AND det.ID_PDC_Testata = lsb.ID_PDC_Testata  ")
            stbQ.AppendLine("         AND sb.CapitolatoCliente_Cod = lsb.CapitolatoCliente_Cod  ")
            stbQ.AppendLine("     INNER JOIN PDC_Testata tes  ")
            stbQ.AppendLine("         ON tes.Id_PDC_Testata = lsb.ID_PDC_Testata  ")
            stbQ.AppendLine("     INNER JOIN specievegetali veg  ")
            stbQ.AppendLine("         ON veg.veg_Cod = det.Veg_Cod  ")
            stbQ.AppendLine("     INNER JOIN Cultivar c  ")
            stbQ.AppendLine("         ON c.Cul_Cod = det.Cul_Cod  ")
            stbQ.AppendLine("     LEFT JOIN GruppoVarietale grva  ")
            stbQ.AppendLine("         ON grva.grva_cod = det.GRVA_Cod  ")
            stbQ.AppendLine("     LEFT JOIN imprese_codici IC  ")
            stbQ.AppendLine("         ON det.piva = IC.piva  ")
            stbQ.AppendLine("         AND IC.id_cod = 1033  ")
            stbQ.AppendLine("     WHERE sb.CapitolatoCliente_Cod >= 0 ")

            If veg_cod <> "" Then
                stbQ.AppendLine("  AND veg.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
            End If

            If grva_cod <> "" Then
                stbQ.AppendLine("  AND grva.Grva_Cod = " & Agro_SQL_SaveNum(grva_cod) & " ")
            End If

            If cul_cod <> "" Then
                stbQ.AppendLine("  AND c.Cul_Cod = " & Agro_SQL_SaveNum(cul_cod) & " ")
            End If

            stbQ.AppendLine("           AND tes.PDC_Data_istantanea >=  " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            stbQ.AppendLine("           AND tes.PDC_Data_istantanea <=  " & Agro_SQL_SaveDate(validita_fine) & "  ")
            stbQ.AppendLine("),  ")
            stbQ.AppendLine("Ranked AS (  ")
            stbQ.AppendLine("    SELECT  ")
            stbQ.AppendLine("        *,  ")
            stbQ.AppendLine("        DENSE_RANK() OVER (  ")
            stbQ.AppendLine("            ORDER BY  ")
            stbQ.AppendLine("                PivaSuperUser,  ")
            stbQ.AppendLine("                Piva,  ")
            stbQ.AppendLine("                Sa_Cod,  ")
            stbQ.AppendLine("                Appezza,  ")
            stbQ.AppendLine("                Id_Reg,  ")
            stbQ.AppendLine("                Id_Pdc_Testata  ")
            stbQ.AppendLine("        ) AS RaggruppaCapitolato  ")
            stbQ.AppendLine("    FROM FilteredBase  ")
            stbQ.AppendLine("),  ")
            stbQ.AppendLine("CapitolatiConcat AS (  ")
            stbQ.AppendLine("    SELECT  ")
            stbQ.AppendLine("        RaggruppaCapitolato,  ")
            stbQ.AppendLine("        capitolato_des = STUFF((  ")
            stbQ.AppendLine("            SELECT ', ' + rb2.capitolato_des  ")
            stbQ.AppendLine("            FROM Ranked rb2  ")
            stbQ.AppendLine("            WHERE rb2.RaggruppaCapitolato = rb.RaggruppaCapitolato  ")
            stbQ.AppendLine("            ORDER BY rb2.capitolato_des  ")
            stbQ.AppendLine("            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')  ")
            stbQ.AppendLine("    FROM Ranked rb  ")
            stbQ.AppendLine("    GROUP BY RaggruppaCapitolato  ")
            stbQ.AppendLine(")  ")
            stbQ.AppendLine("SELECT  ")
            stbQ.AppendLine("    cc.capitolato_des,  ")
            stbQ.AppendLine("    MAX(r.Det_PIVA) AS PIVA,  ")
            stbQ.AppendLine("    MAX(r.rag_soc) AS rag_soc,  ")
            stbQ.AppendLine("    MAX(r.CodiceSocio) AS CodiceSocio,  ")
            stbQ.AppendLine("    '' AS LFO,  ")
            stbQ.AppendLine("    MAX(r.sa_nome) AS sa_nome,  ")
            stbQ.AppendLine("    '' AS APP_NOME,  ")
            stbQ.AppendLine("    MAX(r.veg_Des) AS Veg_Des,  ")
            stbQ.AppendLine("    MAX(r.cul_Des) AS Cul_Des,  ")
            stbQ.AppendLine("    MAX(r.grva_des) AS grva_des,  ")
            stbQ.AppendLine("    MAX(r.DettagliAnalisi) AS DettagliAnalisi,  ")
            stbQ.AppendLine("    MAX(YEAR(r.PDC_Data_istantanea)) AS PDC_Anno_DataIstantanea  ")
            stbQ.AppendLine("FROM CapitolatiConcat cc  ")
            stbQ.AppendLine("JOIN Ranked r  ")
            stbQ.AppendLine("    ON cc.RaggruppaCapitolato = r.RaggruppaCapitolato  ")
            stbQ.AppendLine("GROUP BY cc.capitolato_des  ")
            stbQ.AppendLine("ORDER BY cc.capitolato_des; ")

            Dim objSQL As New AgronicaCoreDataProvider.DataProvider

            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, "CaricaDsAbilitazioni", DSAbilitazioni, DSAbilitazioni.DT_Abilitazioni.TableName)

            If FiltraPive Then
                ' Eliminazione tabella temporanea
                TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

                'commit transazione
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            If FiltraPive Then
                'rollback transazione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            If FiltraPive Then
                ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
        End Try

    End Sub

End Class
