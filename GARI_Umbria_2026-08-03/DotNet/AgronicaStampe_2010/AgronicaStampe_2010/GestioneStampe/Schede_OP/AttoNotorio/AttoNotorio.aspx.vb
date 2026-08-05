Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AttoNotorio
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_AttoNotorio
    Private DSAttoNotorio As DS_AttoNotorio

    '----- Gestione Querystring
    Dim Piva As String
    Dim anno, validita_inizio, validita_fine As String
    Dim Str_FiltroImpianti As String

    Dim LinkPaginaStampa As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_AttoNotorio

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim Vet_Intestazione(25) As String
        Dim Log As String
        Dim rag_soc As String

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

        rag_soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

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

        'filtro impianti con AND
        Str_FiltroImpianti = Session("strParametri").ToString()



        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer


        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            Dim DSAttoNotorio As New DS_AttoNotorio

            'carico i dati nel datatable 
            Carica_DSAttoNotorio(DSAttoNotorio)

            PulisciDoppi(DSAttoNotorio)

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim pivaReale As String = objImp.Leggi_PivaReale(Piva, objParametri_Server)

            'prelevo i dati dell'intestazione
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.DatiIntestazioneSocio(Piva, rag_soc, Vet_Intestazione, Log, objParametri_Server, False, False, True)

            'CType(rptStampa.Section1.ReportObjects("TextSuperUser"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(23).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = anno

            CType(rptStampa.Section1.ReportObjects("TextRapprLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(1).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextComNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(2).ToUpper

            If Vet_Intestazione(3).ToUpper <> "0" Then
                CType(rptStampa.Section1.ReportObjects("TextProvNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(3).ToUpper
            End If

            CType(rptStampa.Section1.ReportObjects("TextDataNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(4).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextIndResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(5).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextComResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(6).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextProvResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(7).ToUpper
            CType(rptStampa.Section1.ReportObjects("TextQualifica"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(8).ToUpper

            CType(rptStampa.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(10)
            CType(rptStampa.Section1.ReportObjects("TextIndImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(11)
            CType(rptStampa.Section1.ReportObjects("TextComImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(12)
            CType(rptStampa.Section1.ReportObjects("TextProvImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(13)
            CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale

            CType(rptStampa.Section1.ReportObjects("TextCUAA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(25)

            CType(rptStampa.Section1.ReportObjects("TextCooperativa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(15)
            CType(rptStampa.Section1.ReportObjects("TextIndCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(16)
            CType(rptStampa.Section1.ReportObjects("TextCapCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(17)
            CType(rptStampa.Section1.ReportObjects("TextComCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(18)
            CType(rptStampa.Section1.ReportObjects("TextProvCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(19)
            CType(rptStampa.Section1.ReportObjects("TextNumIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(20)
            CType(rptStampa.Section1.ReportObjects("TextDataIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(21)

            If Vet_Intestazione(22) <> "00069880391" Then
                rptStampa.Section1.ReportObjects("LogoTerremerse").ObjectFormat.EnableSuppress = True
            Else
                CType(rptStampa.Section1.ReportObjects("TextCooperativa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "OP TERREMERSE SEZIONE ORTOFRUTTA"
            End If

            If DSAttoNotorio.DT_AttoNotorio.Rows.Count = 0 Then
                rptStampa.Section3.SectionFormat.EnableSuppress = True
            End If


            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                rptStampa.SetDataSource(DSAttoNotorio)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try


            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.AttoNotorio, "", "", objParametri_Server)
            objCatDoc = Nothing

            Dim Nome_Documento As String = "AttoNotorio"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa,
                                               enum_CategorieDocumenti.AttoNotorio,
                                               Sottocartella,
                                               Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer


            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                                     enum_CategorieDocumenti.AttoNotorio,
                                                                     "Atto Notorio",
                                                                     Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                                                     Sottocartella,
                                                                     "", "", "", "",
                                                                     CDate("01/01/" & anno),
                                                                     CDate("31/12/" & anno),
                                                                     objParametri_Server)


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + anno + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "AttoNotorio"
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_OP"
                End If

                Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = Path_Errore,
                    .LogFileName = Nome_File
                }

                'objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Qs_Piva.Page_Load", Log_Errori)
                objLog.Scrivi_LOG(objParametri_Server, "Qs_Piva.Page_Load", Log_Errori, CustomLOGParams:=CustomLOGParams)

            End If
            '-----------------------------------------

            Session("Report") = rptStampa
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


        End If

    End Sub
    Private Sub PulisciDoppi(ByRef DSAttoNotorio As DS_AttoNotorio)
        Dim i As Integer
        Dim precedente As String = ""
        For i = 0 To DSAttoNotorio.DT_AttoNotorio.Rows.Count - 1
            Dim app As String
            app = DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("piva") &
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sa_cod") &
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("appezza") &
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("id_reg")
            If app <> precedente Then
                precedente = app
            Else
                DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("copia") = "1"
            End If
        Next
    End Sub

    Private Sub PulisciDoppi(ByRef DSSchedaAziendale As DS_SchedaAziendale)
        Dim i As Integer
        Dim precedente As String = ""
        For i = 0 To DSSchedaAziendale.DT_SchedaAziendale.Rows.Count - 1
            Dim app As String
            app = DSSchedaAziendale.DT_SchedaAziendale.Rows(i).Item("piva") &
                        "_" & DSSchedaAziendale.DT_SchedaAziendale.Rows(i).Item("sa_cod") &
                        "_" & DSSchedaAziendale.DT_SchedaAziendale.Rows(i).Item("appezza") &
                        "_" & DSSchedaAziendale.DT_SchedaAziendale.Rows(i).Item("id_reg")
            If app <> precedente Then
                precedente = app
            Else
                DSSchedaAziendale.DT_SchedaAziendale.Rows(i).Item("copia") = "1"
            End If
        Next
    End Sub


    '#########################################################################################################
    Private Sub Carica_DSAttoNotorio(ByRef DSAttoNotorio As DS_AttoNotorio)

        Dim strErr As String
        Dim stbQ As New System.Text.StringBuilder

        Dim i As Integer

        '------------------------------------------------
        'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
        '------------------------------------------------
        stbQ.AppendLine(" ( ")
        stbQ.AppendLine(" SELECT DISTINCT 0 as copia, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Imprese.partitaIvaReale END AS Piva,")
        stbQ.AppendLine(" Reg_Impianti.SA_COD AS Sa_Cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza, Reg_Impianti.Id_Reg as Id_Reg,")
        stbQ.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.AppendLine(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE,")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.AppendLine(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.AppendLine(" AppezzamentiXParticelle.AREA, AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.AppendLine(" Appezzamento.Validita_Inizio AS Validita_Inizio, Appezzamento.Validita_Fine AS Validita_Fine, ")
        stbQ.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des, ")
        stbQ.AppendLine(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  Reg_Impianti.Sup_Imp, ")

        stbQ.AppendLine(" '' AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, ISNULL(Reg_Impianti.COP_COD, 0) AS COP_COD, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, ISNULL(Reg_Impianti.IMP_COD,0) AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
        stbQ.AppendLine(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.AppendLine("           FROM         reg_impianti_codici ")
        stbQ.AppendLine("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.AppendLine("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1063), 0)  AS SU_FILA, ")
        stbQ.AppendLine(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.AppendLine("           FROM         reg_impianti_codici ")
        stbQ.AppendLine("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.AppendLine("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1061), 0)  AS TRA_FILA, ")
        stbQ.AppendLine(" ISNULL((SELECT     Foral_Des ")
        stbQ.AppendLine("           FROM     FormeAllevamento  ")
        stbQ.AppendLine("            WHERE   Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod),'') AS FormaAllevamento, ")

        'PIANTE
        'ISNULL(Reg_Impianti.P_HA, 0) AS P_HA, 
        stbQ.AppendLine(" ISNULL((SELECT TOP 1 Imprese_Progetti.p_ha FROM Imprese_Progetti ")
        stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.AppendLine("), 0) AS P_HA ")

        stbQ.AppendLine(", convert(float, (cast (ParticelleCatastali.ETTARI as varchar(100))+ '.' + " & vbCrLf)
        stbQ.AppendLine("     right('000' + cast (ParticelleCatastali.Are as varchar(100)), 2)  +  " & vbCrLf)
        stbQ.AppendLine("     right('000' + cast (ParticelleCatastali.CentiAre as varchar(100)), 2))    " & vbCrLf)
        stbQ.AppendLine("     )   as Sup_Cat  " & vbCrLf)

        'resa
        stbQ.AppendLine(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
        stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.AppendLine("), 0) AS Resa ")


        stbQ.AppendLine(" FROM SpecieVegetali INNER JOIN ")
        stbQ.AppendLine(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod RIGHT OUTER JOIN ")
        stbQ.AppendLine(" ParticelleCatastali INNER JOIN ")
        stbQ.AppendLine(" AppezzamentiXParticelle ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV AND  ")
        stbQ.AppendLine(" ParticelleCatastali.COM = AppezzamentiXParticelle.COM AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO AND  ")
        stbQ.AppendLine(" ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO AND  ")
        stbQ.AppendLine(" ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO INNER JOIN ")
        stbQ.AppendLine(" ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM INNER JOIN ")
        stbQ.AppendLine(" Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND  ")
        stbQ.AppendLine(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND  ")
        stbQ.AppendLine(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
        stbQ.AppendLine(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        stbQ.AppendLine(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
        stbQ.AppendLine(" INNER JOIN Imprese ON Reg_Impianti.PIVA = Imprese.PIVA ")
        stbQ.AppendLine(" INNER JOIN  ImpreseXParticelle ")
        stbQ.AppendLine(" ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA And ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD ")
        stbQ.AppendLine(" And ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV And ImpreseXParticelle.COM = AppezzamentiXParticelle.COM ")
        stbQ.AppendLine(" And ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE And  ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO ")
        stbQ.AppendLine(" And ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO And ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ")

        stbQ.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA And ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA And CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.AppendLine(" WHERE AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        stbQ.AppendLine(Str_FiltroImpianti)

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND AppezzamentiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
        'End If

        stbQ.AppendLine(" AND AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" AND Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" ) ")


        '------------------------------------------------
        'PARTICELLE ASSOCIATE A CAMPI 
        '------------------------------------------------
        stbQ.AppendLine(" UNION ")

        stbQ.AppendLine(" ( ")
        stbQ.AppendLine(" SELECT DISTINCT 0 as copia, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN CampiXParticelle.PIVA ELSE Imprese.partitaIvaReale END AS Piva,")
        stbQ.AppendLine(" CampiXParticelle.SA_COD as Sa_Cod, CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg,")
        stbQ.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.AppendLine(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.AppendLine(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.AppendLine("  CampiXParticelle.AREA, CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.AppendLine(" Campi.Validita_Inizio AS Validita_Inizio, Campi.Validita_Fine AS Validita_Fine, ")
        stbQ.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des,  ")
        stbQ.AppendLine(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto, Reg_Impianti.Sup_Imp, ")

        stbQ.AppendLine(" Campi.Campo_Des AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, ISNULL(Reg_Impianti.COP_COD, 0) AS COP_COD, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, ISNULL(Reg_Impianti.IMP_COD,0) AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
        stbQ.AppendLine(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.AppendLine("           FROM         reg_impianti_codici ")
        stbQ.AppendLine("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.AppendLine("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1063), 0)  AS SU_FILA, ")
        stbQ.AppendLine(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.AppendLine("           FROM         reg_impianti_codici ")
        stbQ.AppendLine("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.AppendLine("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1061), 0)  AS TRA_FILA, ")
        stbQ.AppendLine(" ISNULL((SELECT    Foral_Des ")
        stbQ.AppendLine("           FROM     FormeAllevamento  ")
        stbQ.AppendLine("            WHERE   Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod),'') AS FormaAllevamento, ")

        'PIANTE
        'ISNULL(Reg_Impianti.P_HA, 0) AS P_HA, 
        stbQ.AppendLine(" ISNULL((SELECT TOP 1 Imprese_Progetti.p_ha FROM Imprese_Progetti ")
        stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.AppendLine("), 0) AS P_HA ")

        stbQ.AppendLine(", convert(float, (cast (ParticelleCatastali.ETTARI as varchar(100))+ '.' + " & vbCrLf)
        stbQ.AppendLine("     right('000' + cast (ParticelleCatastali.Are as varchar(100)), 2)  +  " & vbCrLf)
        stbQ.AppendLine("     right('000' + cast (ParticelleCatastali.CentiAre as varchar(100)), 2))    " & vbCrLf)
        stbQ.AppendLine("     )   as Sup_Cat  " & vbCrLf)

        'resa
        stbQ.AppendLine(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
        stbQ.AppendLine("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.AppendLine("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.AppendLine("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.AppendLine("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.AppendLine("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.AppendLine("), 0) AS Resa ")

        stbQ.AppendLine(" FROM ParticelleCatastali ")
        stbQ.AppendLine(" INNER JOIN CampiXParticelle ON ParticelleCatastali.PROV = CampiXParticelle.PROV ")
        stbQ.AppendLine(" AND ParticelleCatastali.COM = CampiXParticelle.COM ")
        stbQ.AppendLine(" AND ParticelleCatastali.sezione = CampiXParticelle.sezione ")
        stbQ.AppendLine(" AND ParticelleCatastali.foglio = CampiXParticelle.foglio ")
        stbQ.AppendLine(" AND ParticelleCatastali.numero = CampiXParticelle.numero ")
        stbQ.AppendLine(" AND ParticelleCatastali.subalterno = CampiXParticelle.subalterno ")
        stbQ.AppendLine(" INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV ")
        stbQ.AppendLine(" AND ParticelleCatastali.COM = ISTAT.COM ")
        stbQ.AppendLine(" INNER JOIN Campi ON CampiXParticelle.PIVA = Campi.PIVA ")
        stbQ.AppendLine(" AND CampiXParticelle.SA_COD = Campi.SA_COD ")
        stbQ.AppendLine(" AND CampiXParticelle.Campo_Cod = Campi.Campo_Cod ")
        stbQ.AppendLine(" INNER JOIN  ImpreseXParticelle ")
        stbQ.AppendLine(" ON ImpreseXParticelle.PIVA = CampiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = CampiXParticelle.SA_COD ")
        stbQ.AppendLine(" AND ImpreseXParticelle.PROV = CampiXParticelle.PROV AND ImpreseXParticelle.COM = CampiXParticelle.COM ")
        stbQ.AppendLine(" AND ImpreseXParticelle.SEZIONE = CampiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = CampiXParticelle.FOGLIO ")
        stbQ.AppendLine(" AND ImpreseXParticelle.NUMERO = CampiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = CampiXParticelle.SUBALTERNO ")
        stbQ.AppendLine(" LEFT OUTER JOIN Appezzamento ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND  Appezzamento.Campo_Cod = Campi.Campo_Cod ")
        stbQ.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ")
        stbQ.AppendLine(" INNER JOIN Imprese ON Reg_Impianti.PIVA = Imprese.PIVA ")
        stbQ.AppendLine(" INNER JOIN  Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
        stbQ.AppendLine(" INNER JOIN  SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")

        stbQ.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA And ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA And CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.AppendLine(" WHERE CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        stbQ.AppendLine(Str_FiltroImpianti)

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND CampiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
        'End If

        stbQ.AppendLine(" AND CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" AND Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND Campi.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" AND CampiXParticelle.Campo_Cod NOT IN ")
        stbQ.AppendLine(" (SELECT DISTINCT Appezzamento.Campo_Cod ")
        stbQ.AppendLine(" FROM  AppezzamentiXParticelle INNER JOIN ")
        stbQ.AppendLine(" Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND ")
        stbQ.AppendLine(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND ")
        stbQ.AppendLine(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA ")
        stbQ.AppendLine(" WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND AppezzamentiXParticelle.Sa_Cod = " & SQL_SaveNum(Qs_Sa_Cod) & " ")
        'End If
        stbQ.AppendLine("  ) ")

        stbQ.AppendLine(" ) ")
        'nuovo ordinamento
        stbQ.AppendLine(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.Append(" ORDER BY sa_cod, veg_des, cul_des, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.Append(" ORDER BY sa_cod, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")

        strErr = Nothing

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, "AttoNotorio.CaricaDsAttoNotorio", DSAttoNotorio, DSAttoNotorio.DT_AttoNotorio.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            '(12/12/14) fede visualizzo sempre tutto su indicazione di Valerio
            ''===================================================================================================
            ''se l'impianto è lo stesso elimino i dati nelle righe successive
            ''per farlo metto cul_des=-1 ed elimino dopo le stringhe
            'For i = 1 To DSAttoNotorio.DT_AttoNotorio.Rows.Count - 1

            '    If DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("piva") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("piva") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sa_cod") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("sa_cod") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("appezza") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("appezza") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("id_reg") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("id_reg") Then

            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("veg_des") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("cul_des") = ""

            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("su_fila") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("tra_fila") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("formaallevamento") = ""

            '        'metto poi le formule nel report per omettere i campi sup_imp, @numpiante, @anno
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("p_ha") = 0
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sup_imp") = -1
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("inizio_impianto") = #1/1/1900#

            '    End If

            'Next

        End If


    End Sub

    '#########################################################################################################
    Private Sub Carica_DSSchedaAziendale(ByRef DSSchedaAziendale As DS_SchedaAziendale)

        Dim strErr As String
        Dim stbQ As New System.Text.StringBuilder

        Dim i As Integer

        '------------------------------------------------
        'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
        '------------------------------------------------
        stbQ.Append(" ( ")
        stbQ.Append(" SELECT DISTINCT 0 as copia, Reg_Impianti.PIVA AS Piva, Reg_Impianti.SA_COD AS Sa_Cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza, Reg_Impianti.Id_Reg as Id_Reg,")
        stbQ.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.Append(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE,")
        stbQ.Append(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.Append(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.Append(" AppezzamentiXParticelle.AREA, AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.Append(" Appezzamento.Validita_Inizio AS Validita_Inizio, Appezzamento.Validita_Fine AS Validita_Fine, ")
        stbQ.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des, ")
        stbQ.Append(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  Reg_Impianti.Sup_Imp, ")

        stbQ.Append(" '' AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.Append(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, ISNULL(Reg_Impianti.COP_COD, 0) AS COP_COD, ")
        stbQ.Append(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, ISNULL(Reg_Impianti.IMP_COD,0) AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
        stbQ.Append(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.Append("           FROM         reg_impianti_codici ")
        stbQ.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1063), 0)  AS SU_FILA, ")
        stbQ.Append(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.Append("           FROM         reg_impianti_codici ")
        stbQ.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1061), 0)  AS TRA_FILA, ")
        stbQ.Append(" ISNULL((SELECT     Foral_Des ")
        stbQ.Append("           FROM     FormeAllevamento  ")
        stbQ.Append("            WHERE   Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod),'') AS FormaAllevamento, ")

        'PIANTE
        'ISNULL(Reg_Impianti.P_HA, 0) AS P_HA, 
        stbQ.Append(" ISNULL((SELECT TOP 1 Imprese_Progetti.p_ha FROM Imprese_Progetti ")
        stbQ.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.Append("), 0) AS P_HA ")

        stbQ.Append(", convert(float, (cast (ParticelleCatastali.ETTARI as varchar(100))+ '.' + " & vbCrLf)
        stbQ.Append("     right('000' + cast (ParticelleCatastali.Are as varchar(100)), 2)  +  " & vbCrLf)
        stbQ.Append("     right('000' + cast (ParticelleCatastali.CentiAre as varchar(100)), 2))    " & vbCrLf)
        stbQ.Append("     )   as Sup_Cat  " & vbCrLf)

        'resa
        stbQ.Append(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
        stbQ.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.Append("), 0) AS Resa ")


        stbQ.Append(" FROM SpecieVegetali INNER JOIN ")
        stbQ.Append(" Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod RIGHT OUTER JOIN ")
        stbQ.Append(" ParticelleCatastali INNER JOIN ")
        stbQ.Append(" AppezzamentiXParticelle ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV AND  ")
        stbQ.Append(" ParticelleCatastali.COM = AppezzamentiXParticelle.COM AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ")
        stbQ.Append(" ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO AND  ")
        stbQ.Append(" ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO AND  ")
        stbQ.Append(" ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO INNER JOIN ")
        stbQ.Append(" ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM INNER JOIN ")
        stbQ.Append(" Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND  ")
        stbQ.Append(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND  ")
        stbQ.Append(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA INNER JOIN ")
        stbQ.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  ")
        stbQ.Append(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
        stbQ.Append(" INNER JOIN  ImpreseXParticelle ")
        stbQ.Append(" ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD ")
        stbQ.Append(" AND ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV AND ImpreseXParticelle.COM = AppezzamentiXParticelle.COM ")
        stbQ.Append(" AND ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO ")
        stbQ.Append(" AND ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ")

        stbQ.Append(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.Append(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.Append(" WHERE AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        stbQ.Append(Str_FiltroImpianti)

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND AppezzamentiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
        'End If

        stbQ.Append(" AND AppezzamentiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.Append(" AND AppezzamentiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.Append(" AND Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.Append(" AND Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.Append(" ) ")


        '------------------------------------------------
        'PARTICELLE ASSOCIATE A CAMPI 
        '------------------------------------------------
        stbQ.Append(" UNION ")

        stbQ.Append(" ( ")
        stbQ.Append(" SELECT DISTINCT  0 as copia, CampiXParticelle.PIVA as Piva, CampiXParticelle.SA_COD as Sa_Cod, CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg,")
        stbQ.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.Append(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ")
        stbQ.Append(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.Append(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.Append("  CampiXParticelle.AREA, CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.Append(" Campi.Validita_Inizio AS Validita_Inizio, Campi.Validita_Fine AS Validita_Fine, ")
        stbQ.Append(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Terreno Nudo') AS veg_des, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des,  ")
        stbQ.Append(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto, Reg_Impianti.Sup_Imp, ")

        stbQ.Append(" Campi.Campo_Des AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.Append(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, ISNULL(Reg_Impianti.COP_COD, 0) AS COP_COD, ")
        stbQ.Append(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, ISNULL(Reg_Impianti.IMP_COD,0) AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
        stbQ.Append(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.Append("           FROM         reg_impianti_codici ")
        stbQ.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1063), 0)  AS SU_FILA, ")
        stbQ.Append(" ISNULL((SELECT  TOP 1   Val_Cod ")
        stbQ.Append("           FROM         reg_impianti_codici ")
        stbQ.Append("            WHERE     Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD  ")
        stbQ.Append("           AND  Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG AND id_cod = 1061), 0)  AS TRA_FILA, ")
        stbQ.Append(" ISNULL((SELECT    Foral_Des ")
        stbQ.Append("           FROM     FormeAllevamento  ")
        stbQ.Append("            WHERE   Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod),'') AS FormaAllevamento, ")

        'PIANTE
        'ISNULL(Reg_Impianti.P_HA, 0) AS P_HA, 
        stbQ.Append(" ISNULL((SELECT TOP 1 Imprese_Progetti.p_ha FROM Imprese_Progetti ")
        stbQ.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.Append("), 0) AS P_HA ")

        stbQ.Append(", convert(float, (cast (ParticelleCatastali.ETTARI as varchar(100))+ '.' + " & vbCrLf)
        stbQ.Append("     right('000' + cast (ParticelleCatastali.Are as varchar(100)), 2)  +  " & vbCrLf)
        stbQ.Append("     right('000' + cast (ParticelleCatastali.CentiAre as varchar(100)), 2))    " & vbCrLf)
        stbQ.Append("     )   as Sup_Cat  " & vbCrLf)

        'resa
        stbQ.Append(" , ISNULL((SELECT TOP 1 Imprese_Progetti.Produzione_Prevista FROM Imprese_Progetti ")
        stbQ.Append("        WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        stbQ.Append("	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        stbQ.Append("	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        stbQ.Append("	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG ")
        stbQ.Append("       AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & " " & vbCrLf)
        stbQ.Append("       AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio) & " " & vbCrLf)
        stbQ.Append("), 0) AS Resa ")

        stbQ.Append(" FROM ParticelleCatastali ")
        stbQ.Append(" INNER JOIN CampiXParticelle ON ParticelleCatastali.PROV = CampiXParticelle.PROV ")
        stbQ.Append(" AND ParticelleCatastali.COM = CampiXParticelle.COM ")
        stbQ.Append(" AND ParticelleCatastali.sezione = CampiXParticelle.sezione ")
        stbQ.Append(" AND ParticelleCatastali.foglio = CampiXParticelle.foglio ")
        stbQ.Append(" AND ParticelleCatastali.numero = CampiXParticelle.numero ")
        stbQ.Append(" AND ParticelleCatastali.subalterno = CampiXParticelle.subalterno ")
        stbQ.Append(" INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV ")
        stbQ.Append(" AND ParticelleCatastali.COM = ISTAT.COM ")
        stbQ.Append(" INNER JOIN Campi ON CampiXParticelle.PIVA = Campi.PIVA ")
        stbQ.Append(" AND CampiXParticelle.SA_COD = Campi.SA_COD ")
        stbQ.Append(" AND CampiXParticelle.Campo_Cod = Campi.Campo_Cod ")
        stbQ.Append(" INNER JOIN  ImpreseXParticelle ")
        stbQ.Append(" ON ImpreseXParticelle.PIVA = CampiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = CampiXParticelle.SA_COD ")
        stbQ.Append(" AND ImpreseXParticelle.PROV = CampiXParticelle.PROV AND ImpreseXParticelle.COM = CampiXParticelle.COM ")
        stbQ.Append(" AND ImpreseXParticelle.SEZIONE = CampiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = CampiXParticelle.FOGLIO ")
        stbQ.Append(" AND ImpreseXParticelle.NUMERO = CampiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = CampiXParticelle.SUBALTERNO ")
        stbQ.Append(" LEFT OUTER JOIN Appezzamento ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND  Appezzamento.Campo_Cod = Campi.Campo_Cod ")
        stbQ.Append(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA  ")
        stbQ.Append(" INNER JOIN  Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
        stbQ.Append(" INNER JOIN  SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")

        stbQ.Append(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.Append(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.Append(" WHERE CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        stbQ.Append(Str_FiltroImpianti)

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND CampiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
        'End If

        stbQ.Append(" AND CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.Append(" AND CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.Append(" AND Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.Append(" AND Campi.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.Append(" AND CampiXParticelle.Campo_Cod NOT IN ")
        stbQ.Append(" (SELECT DISTINCT Appezzamento.Campo_Cod ")
        stbQ.Append(" FROM  AppezzamentiXParticelle INNER JOIN ")
        stbQ.Append(" Appezzamento ON AppezzamentiXParticelle.PIVA = Appezzamento.PIVA AND ")
        stbQ.Append(" AppezzamentiXParticelle.SA_COD = Appezzamento.SA_COD AND ")
        stbQ.Append(" AppezzamentiXParticelle.APPEZZA = Appezzamento.APPEZZA ")
        stbQ.Append(" WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.Append(" AND AppezzamentiXParticelle.Sa_Cod = " & SQL_SaveNum(Qs_Sa_Cod) & " ")
        'End If
        stbQ.Append("  ) ")

        stbQ.Append(" ) ")
        'nuovo ordinamento
        stbQ.Append(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.Append(" ORDER BY sa_cod, veg_des, cul_des, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.Append(" ORDER BY sa_cod, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")

        strErr = Nothing

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, "AttoNotorio.CaricaDSSchedaAziendale", DSSchedaAziendale, DSSchedaAziendale.DT_SchedaAziendale.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            '(12/12/14) fede visualizzo sempre tutto su indicazione di Valerio
            ''===================================================================================================
            ''se l'impianto è lo stesso elimino i dati nelle righe successive
            ''per farlo metto cul_des=-1 ed elimino dopo le stringhe
            'For i = 1 To DSAttoNotorio.DT_AttoNotorio.Rows.Count - 1

            '    If DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("piva") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("piva") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sa_cod") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("sa_cod") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("appezza") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("appezza") And _
            '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("id_reg") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("id_reg") Then

            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("veg_des") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("cul_des") = ""

            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("su_fila") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("tra_fila") = ""
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("formaallevamento") = ""

            '        'metto poi le formule nel report per omettere i campi sup_imp, @numpiante, @anno
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("p_ha") = 0
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sup_imp") = -1
            '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("inizio_impianto") = #1/1/1900#

            '    End If

            'Next

        End If


    End Sub


End Class