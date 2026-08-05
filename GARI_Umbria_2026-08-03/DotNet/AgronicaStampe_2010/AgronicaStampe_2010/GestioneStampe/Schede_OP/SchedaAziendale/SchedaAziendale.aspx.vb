Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreAnagrafeDAL
Imports System.IO

Public Class SchedaAziendale
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_SchedaAziendale
    Private DSSchedaAziendale As DS_SchedaAziendale

    '----- Gestione Querystring
    Dim Piva As String
    Dim anno, validita_inizio, validita_fine As String
    Dim Str_FiltroImpianti As String
    Dim Tipo_Selezione As String
    Dim fronteRetro As Boolean

    Dim LinkPaginaStampa As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_SchedaAziendale

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

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


        Tipo_Selezione = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        fronteRetro = Stringa_Decodifica(Request.QueryString("fronteretro").ToString,
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

            Dim DSSchedaAziendale As New DS_SchedaAziendale
            Dim DSSpeciexParticelle As New DS_SuperficieSpeciexParticelle
            Dim DSLoghi As New Ds_Loghi
            Dim DSImprese As New Ds_Imprese_StampaMassiva

            'carico i dati nel datatable 

            Carica_DSSuperficieSpeciexParticelle(DSSpeciexParticelle)

            Carica_DSSchedaAziendale(DSSchedaAziendale)

            PulisciDoppi(DSSchedaAziendale)

            CaricaDs_Loghi(DSLoghi)

            'INTESTAZIONE
            Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)

            Dim RowRimozione As New List(Of DataRow)

            For Each row In DSSpeciexParticelle.DT_SuperficieSpeciexParticelle.Rows
                If Not IsDBNull(row.Item("sez_cens")) AndAlso row.Item("sez_cens") = "0" Then
                    row.Item("sez_cens") = ""
                End If
                If Not IsDBNull(row.Item("sub")) AndAlso row.Item("sub") = "0" Then
                    row.Item("sub") = ""
                End If
                If row.Item("sup_specie") = 0 Then
                    RowRimozione.Add(row)
                End If
            Next

            For Each row In RowRimozione
                row.Delete()
            Next

            DSSpeciexParticelle.AcceptChanges()

            Dim loghiRow As DataRow = DSLoghi.Loghi.Rows(0)

            Dim groupedData = DSSchedaAziendale.DT_SchedaAziendale.AsEnumerable().
            GroupBy(Function(x) New With {Key .piva = x("piva"), Key .sa_cod = x("sa_cod")}).
            ToDictionary(Function(g) g.Key, Function(g) g.ToList())

            For Each row In DSSchedaAziendale.DT_SchedaAziendale.Rows
                row.Item("Localita") = row.Item("ind_des") & " - " & row.Item("CAP") & ", " & row.Item("Localita") & " - " & row.Item("comuni_prov")
                row.Item("blob_logo") = loghiRow.Field(Of Byte())("Blob_Logo")

                Dim key = New With {Key .piva = row.Item("piva"), Key .sa_cod = row.Item("sa_cod")}

                Dim supTot As Double = 0
                Dim supTotSeminativo As Double = 0
                Dim supTotArborei As Double = 0

                If groupedData.ContainsKey(key) Then
                    Dim records = groupedData(key)

                    ' Pre-filtered data for efficiency
                    Dim validAreas = records.Where(Function(r) Not IsDBNull(r("AREA"))).ToList()

                    supTot = validAreas.Sum(Function(r) Convert.ToDouble(r("AREA")))

                    supTotSeminativo = validAreas.Where(Function(r) Not IsDBNull(r("Gru_Cod")) AndAlso
                                                   (r("Gru_Cod") = CInt(enum_GruppoVegetale.Erbacee) OrElse r("Gru_Cod") = CInt(enum_GruppoVegetale.OrtoFloroVivaismo))).
                                      Sum(Function(r) Convert.ToDouble(r("AREA")))

                    supTotArborei = validAreas.Where(Function(r) Not IsDBNull(r("Gru_Cod")) AndAlso
                                               r("Gru_Cod") = CInt(enum_GruppoVegetale.Arboree)).
                                  Sum(Function(r) Convert.ToDouble(r("AREA")))
                End If

                row.Item("SupTotUtilizzata") = supTot.ToString()
                row.Item("SupTotSeminativo") = supTotSeminativo.ToString()
                row.Item("SupTotArborei") = supTotArborei.ToString()
                row.Item("Cop_Bool") = If(row.Item("cop_cod") = 1, "X", "")
                row.Item("Imp_Bool") = If(row.Item("imp_cod") = 1, "X", "")
            Next

            CType(rptStampa.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = anno

            Dim countByPiva As Dictionary(Of String, Integer) = DSSchedaAziendale.DT_SchedaAziendale.AsEnumerable() _
            .GroupBy(Function(row) row.Field(Of String)("piva")) _
            .ToDictionary(Function(g) g.Key, Function(g) g.Select(Function(row) row.Field(Of Integer)("sa_cod")).Distinct().Count())

            For Each rowImpresa In DSImprese.Imprese_StampaMassiva.Rows
                Dim piva As String = rowImpresa.Item("Piva").ToString()
                Dim numTerreni As Integer = If(countByPiva.ContainsKey(piva), countByPiva(piva), 0)
                rowImpresa.Item("N_Centri") = numTerreni
            Next

            For Each rowImpresa In DSImprese.Imprese_StampaMassiva.Rows

                Dim currentPiva = rowImpresa.Item("piva")

                If DSSchedaAziendale.DT_SchedaAziendale.Select("PIVA = ' " & currentPiva & "'").Length = 0 Then

                    Dim drReport = DSSchedaAziendale.DT_SchedaAziendale.NewRow

                    drReport(DSSchedaAziendale.DT_SchedaAziendale.pivaColumn) = currentPiva

                    drReport(DSSchedaAziendale.DT_SchedaAziendale.blob_logoColumn) = DSLoghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

                    DSSchedaAziendale.DT_SchedaAziendale.Rows.Add(drReport)

                End If

            Next

            If DSSchedaAziendale.DT_SchedaAziendale.Rows.Count = 0 Then
                For Each dr As DataRow In DSImprese.Imprese_StampaMassiva.Rows

                    Dim drReport = DSSchedaAziendale.DT_SchedaAziendale.NewRow

                    drReport(DSSchedaAziendale.DT_SchedaAziendale.pivaColumn) = dr.Field(Of String)("piva")

                    drReport(DSSchedaAziendale.DT_SchedaAziendale.blob_logoColumn) = DSLoghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

                    DSSchedaAziendale.DT_SchedaAziendale.Rows.Add(drReport)

                Next
                rptStampa.Section3.SectionFormat.EnableSuppress = True
                rptStampa.GroupHeaderSection3.SectionFormat.EnableSuppress = True
            End If

            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                rptStampa.Database.Tables("DT_SchedaAziendale").SetDataSource(DSSchedaAziendale)

                rptStampa.OpenSubreport("Rpt_SpecieParticelle.rpt").SetDataSource(DSSpeciexParticelle)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try

            ' imposto il parametro del fronte-retro
            rptStampa.SetParameterValue("fronteRetro", fronteRetro)

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.SchedaAziendale, "", "", objParametri_Server)

            objCatDoc = Nothing

            Dim Nome_Documento As String = "SchedaColtivazioneConferimentoAziendale"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa,
                                   enum_CategorieDocumenti.SchedaAziendale,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                         enum_CategorieDocumenti.SchedaAziendale,
                                                         "Scheda Aziendale",
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
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "SchedaAziendale"
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

            Session("Report") = rptStampa
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


        End If



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
    Private Sub Carica_DSSchedaAziendale(ByRef DSSchedaAziendale As DS_SchedaAziendale)

        Dim strErr As String
        Dim stbQ As New System.Text.StringBuilder

        Dim i As Integer
        Dim filtroImpianti As String = Str_FiltroImpianti.ToString()

        '------------------------------------------------
        'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
        '------------------------------------------------
        stbQ.AppendLine(" WITH LivelliColturali AS ( ")
        stbQ.AppendLine(" ( ")
        stbQ.AppendLine(" SELECT DISTINCT 0 as copia, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Imprese.partitaIvaReale END AS Piva,")
        stbQ.AppendLine(" Reg_Impianti.SA_COD AS Sa_Cod, 0 as Campo_Cod, Reg_Impianti.Appezza as Appezza, Reg_Impianti.Id_Reg as Id_Reg,")
        stbQ.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.AppendLine(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE,")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.AppendLine(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.AppendLine(" AppezzamentiXParticelle.AREA, AppezzamentiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, AppezzamentiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.AppendLine(" Appezzamento.Validita_Inizio AS Validita_Inizio, Appezzamento.Validita_Fine AS Validita_Fine, ")
        stbQ.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Nessuna Coltura') AS veg_des, SpecieVegetali.Gru_Cod as gru_cod, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des, ")
        stbQ.AppendLine(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto,  Reg_Impianti.Sup_Imp, ")

        stbQ.AppendLine(" '' AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, CASE WHEN ISNULL(Reg_Impianti.COP_COD, 0) NOT IN(0,1,3,4,5,6) THEN 1 ELSE 0 END AS COP_COD, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, ISNULL(Portinnesti.Port_Des, '') AS Port_Des, FasiCicloColturale_Anagrafiche.Fase_Des AS Fase_Des, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, CASE ISNULL(Reg_Impianti.IMP_COD,0) WHEN 0 THEN 0 ELSE 1 END AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
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
        stbQ.AppendLine(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND  ")
        stbQ.AppendLine(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
        stbQ.AppendLine(" LEFT JOIN Portinnesti ON Portinnesti.Port_Cod = Reg_Impianti.PORT_COD ")
        stbQ.AppendLine(" LEFT JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.PIVA AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND Reg_Impianti.APPEZZA = Imprese_Progetti.APPEZZA AND Reg_Impianti.ID_REG = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine(" LEFT JOIN FasiCicloColturale_Anagrafiche ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ")
        stbQ.AppendLine(" INNER JOIN  ImpreseXParticelle ")
        stbQ.AppendLine(" ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA AND ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD ")
        stbQ.AppendLine(" AND ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV AND ImpreseXParticelle.COM = AppezzamentiXParticelle.COM ")
        stbQ.AppendLine(" AND ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE AND  ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO ")
        stbQ.AppendLine(" AND ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO AND ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO ")

        stbQ.AppendLine(" INNER JOIN Imprese ON Imprese.PIVA = ImpreseXParticelle.PIVA ")
        stbQ.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        'stbQ.AppendLine(" WHERE AppezzamentiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        stbQ.AppendLine(filtroImpianti)

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.AppendLine(" AND AppezzamentiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
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
        stbQ.AppendLine(" SELECT DISTINCT  0 as copia, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN CampiXParticelle.PIVA ELSE Imprese.partitaIvaReale END AS Piva, ")
        stbQ.AppendLine(" CampiXParticelle.SA_COD as Sa_Cod, CampiXParticelle.Campo_Cod as Campo_Cod, 0 as Appezza, 0 as Id_Reg,")
        stbQ.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.AppendLine(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.AppendLine(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.AppendLine("  CampiXParticelle.AREA, CampiXParticelle.SAU_Convenz_Ettari AS ETTARI_Sup_Util, CampiXParticelle.SAU_Convenz_Are AS ARE_Sup_Util, CampiXParticelle.SAU_Convenz_Centiare AS CENTIARE_Sup_Util, ")
        stbQ.AppendLine(" Campi.Validita_Inizio AS Validita_Inizio, Campi.Validita_Fine AS Validita_Fine, ")
        stbQ.AppendLine(" ISNULL(SpecieVegetali.Veg_Cod, 0) AS veg_cod, ISNULL(SpecieVegetali.Veg_Des, 'Nessuna Coltura') AS veg_des, SpecieVegetali.Gru_Cod as gru_cod, ISNULL(Cultivar.Cul_cod, 0) AS cul_cod, ISNULL(Cultivar.Cul_Des, '') AS cul_des,  ")
        stbQ.AppendLine(" Reg_Impianti.Validita_Inizio AS Inizio_Impianto, Reg_Impianti.Validita_Fine AS Fine_Impianto, Reg_Impianti.Sup_Imp, ")

        stbQ.AppendLine(" Campi.Campo_Des AS CAMPO_Nome, Appezzamento.APP_NOME AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.FORAL_COD, 0) AS FORAL_COD, ISNULL(Reg_Impianti.GRFI_COD, 0) AS GRFI_COD, CASE WHEN ISNULL(Reg_Impianti.COP_COD, 0) NOT IN(0,1,3,4,5,6) THEN 1 ELSE 0 END AS COP_COD, ")
        stbQ.AppendLine(" ISNULL(Reg_Impianti.PORT_COD, 0) AS PORT_COD, Portinnesti.Port_Des AS Port_Des, FasiCicloColturale_Anagrafiche.Fase_Des AS Fase_Des, ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS GRVA_Cod_VEG, CASE ISNULL(Reg_Impianti.IMP_COD,0) WHEN 0 THEN 0 ELSE 1 END AS IMP_COD, ISNULL(Reg_Impianti.SETUP_COD,'') AS SETUP_COD,  ")
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
        stbQ.AppendLine(" INNER Join Imprese ON Imprese.PIVA = ImpreseXParticelle.PIVA ")
        stbQ.AppendLine(filtroImpianti)
        stbQ.AppendLine(" LEFT OUTER JOIN Appezzamento ON Appezzamento.PIVA = Campi.Piva AND Appezzamento.SA_COD = Campi.Sa_Cod AND  Appezzamento.Campo_Cod = Campi.Campo_Cod ")
        stbQ.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND Reg_Impianti.APPEZZA = Appezzamento.APPEZZA  ")
        stbQ.AppendLine(" LEFT JOIN  Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
        stbQ.AppendLine(" LEFT JOIN Portinnesti ON Portinnesti.Port_Cod = Reg_Impianti.PORT_COD ")
        stbQ.AppendLine(" LEFT JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.PIVA AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND Reg_Impianti.APPEZZA = Imprese_Progetti.APPEZZA AND Reg_Impianti.ID_REG = Imprese_Progetti.ID_REG ")
        stbQ.AppendLine(" LEFT JOIN FasiCicloColturale_Anagrafiche ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod ")
        stbQ.AppendLine(" LEFT JOIN  SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")

        stbQ.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.AppendLine(" LEFT JOIN AppezzamentiXParticelle AxP ON  AxP.SEZIONE = ParticelleCatastali.SEZIONE And AxP.FOGLIO = ParticelleCatastali.FOGLIO And AxP.NUMERO = ParticelleCatastali.NUMERO And AxP.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

        'stbQ.AppendLine(" WHERE CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.AppendLine(" AND CampiXParticelle.SA_COD = " & SQL_SaveNum(Qs_Sa_Cod))
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
        'stbQ.AppendLine(" WHERE AppezzamentiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

        'If Not Qs_Sa_Cod Is Nothing Then
        '    stbQ.AppendLine(" AND AppezzamentiXParticelle.Sa_Cod = " & SQL_SaveNum(Qs_Sa_Cod) & " ")
        'End If
        stbQ.AppendLine("  ) ")

        'DCA20260204 - task: 204065
        '#### INIZIO
        stbQ.AppendLine(" ) ")
        stbQ.AppendLine("), ")

        stbQ.AppendLine(" ParticelleNude AS ( ")

        stbQ.AppendLine(" SELECT DISTINCT 0 as copia, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN  ImpreseXParticelle.PIVA ELSE Imprese.partitaIvaReale END AS Piva,")
        stbQ.AppendLine(" ImpreseXParticelle.SA_COD as Sa_Cod, 0 as Campo_Cod, 0 as Appezza, 0 as Id_Reg, ")
        stbQ.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ISTAT.CAP, ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ImpreseXParticelle.Validita_Inizio AS Inizio_Part, ImpreseXParticelle.Validita_Fine AS Fine_Part, ")
        stbQ.AppendLine(" ParticelleCatastali.Part_Cod, ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ")
        stbQ.AppendLine(" ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO, ")
        stbQ.AppendLine(" ParticelleCatastali.ETTARI AS ETTARI_Sup_Cat, ParticelleCatastali.[ARE] AS ARE_Sup_Cat, ParticelleCatastali.CENTIARE AS CENTIARE_Sup_Cat, ")
        stbQ.AppendLine("  0 As Area, 0 AS ETTARI_Sup_Util, 0 AS ARE_Sup_Util, 0 AS CENTIARE_Sup_Util, ")
        stbQ.AppendLine(" " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AS Validita_Inizio, " & Agro_SQL_SaveDate(AGRODATAFINE) & " AS Validita_Fine, ")
        stbQ.AppendLine(" 0 AS veg_cod, '' AS veg_des, 0 as gru_cod, 0 AS cul_cod, '' AS cul_des, ")
        stbQ.AppendLine(" " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AS Inizio_Impianto, " & Agro_SQL_SaveDate(AGRODATAFINE) & " AS Fine_Impianto, 0 AS Sup_Imp, ")
        stbQ.AppendLine(" '' AS CAMPO_Nome, '' AS App_Nome, Centri_Aziendali.sa_nome, Centri_Aziendali.TitoloPossesso AS TitoloPossessoCentro, ' ' AS TitoloPossessoCentroDesc, Indirizzi.ind_des, Indirizzi.frz_des, ")
        stbQ.AppendLine(" 0 AS FORAL_COD, 0 AS GRFI_COD, 0 AS COP_COD, ")
        stbQ.AppendLine(" 0 AS PORT_COD, '' AS Port_Des,'' AS Fase_Des, 0 AS GRVA_Cod_VEG, 0 AS IMP_COD, '' AS SETUP_COD, ")
        stbQ.AppendLine(" '' AS SU_FILA, ")
        stbQ.AppendLine(" '' AS TRA_FILA, ")
        stbQ.AppendLine(" '' AS FormaAllevamento, ")
        stbQ.AppendLine(" '' AS P_HA, ")
        stbQ.AppendLine(" convert(float, (cast (ParticelleCatastali.ETTARI as varchar(100))+ '.' +  ")
        stbQ.AppendLine(" right('000' + cast (ParticelleCatastali.Are as varchar(100)), 2)  + ")
        stbQ.AppendLine(" right('000' + cast (ParticelleCatastali.CentiAre as varchar(100)), 2)) ")
        stbQ.AppendLine(" )   as Sup_Cat, ")
        stbQ.AppendLine(" 0 AS Resa ")
        stbQ.AppendLine(" FROM ParticelleCatastali ")
        stbQ.AppendLine(" INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV ")
        stbQ.AppendLine("  AND ParticelleCatastali.COM = ISTAT.COM ")
        stbQ.AppendLine(" INNER JOIN  ImpreseXParticelle ")
        stbQ.AppendLine("  ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM ")
        stbQ.AppendLine(" INNER JOIN Imprese ON Imprese.PIVA = ImpreseXParticelle.PIVA ")
        stbQ.AppendLine(" AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND  ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
        stbQ.AppendLine(" AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
        stbQ.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
        stbQ.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

        stbQ.AppendLine(filtroImpianti)

        stbQ.AppendLine(" AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQ.AppendLine(" AND NOT EXISTS (  ")
        stbQ.AppendLine("          SELECT 1  ")
        stbQ.AppendLine("          FROM LivelliColturali LC  ")
        stbQ.AppendLine("          WHERE LC.Piva = ImpreseXParticelle.PIVA  ")
        stbQ.AppendLine("            AND LC.Sa_Cod = ImpreseXParticelle.SA_COD  ")
        stbQ.AppendLine("            AND LC.PROV = ImpreseXParticelle.PROV  ")
        stbQ.AppendLine("            AND LC.COM = ImpreseXParticelle.COM  ")
        stbQ.AppendLine("            AND LC.SEZIONE = ImpreseXParticelle.SEZIONE  ")
        stbQ.AppendLine("            AND LC.FOGLIO = ImpreseXParticelle.FOGLIO  ")
        stbQ.AppendLine("            AND LC.NUMERO = ImpreseXParticelle.NUMERO  ")
        stbQ.AppendLine("            AND LC.SUBALTERNO = ImpreseXParticelle.SUBALTERNO  ")
        stbQ.AppendLine("      )  ")
        stbQ.AppendLine("  ) ")

        '#### FINE
        'nuovo ordinamento
        'stbQ.AppendLine(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, ParticelleCatastali.prov, ParticelleCatastali.com, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.AppendLine(" ORDER BY sa_cod, veg_des, cul_des, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        'stbQ.AppendLine(" ORDER BY sa_cod, ParticelleCatastali.prov, ParticelleCatastali.com, campo_cod, appezza, id_reg, ParticelleCatastali.sezione, ParticelleCatastali.foglio,  ParticelleCatastali.numero, ParticelleCatastali.subalterno ")
        stbQ.AppendLine(" SELECT *  ")
        stbQ.AppendLine(" FROM LivelliColturali  ")
        stbQ.AppendLine(" UNION  ")
        stbQ.AppendLine(" SELECT *  ")
        stbQ.AppendLine(" FROM ParticelleNude  ")
        stbQ.AppendLine(" ORDER BY sa_cod, veg_des, cul_des, campo_cod, appezza, id_reg, prov, com, sezione, foglio,  numero, subalterno ")

        strErr = Nothing

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, "SchedaAziendale.CaricaDsSchedaAziendale", DSSchedaAziendale, DSSchedaAziendale.DT_SchedaAziendale.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

        End If


    End Sub

    Private Sub Carica_DSSuperficieSpeciexParticelle(ByRef DSSuperficieSpeciexParticelle As DS_SuperficieSpeciexParticelle)

        Dim strErr As String
        Dim stbQ As New System.Text.StringBuilder
        Dim filtroParticelle = ""
        Dim i As Integer
        Select Case Tipo_Selezione
            Case "impianto"
                'Formatto il filtro per la query SQL in base agli impianti forniti
                filtroParticelle = Str_FiltroImpianti.ToString().Replace("Reg_Impianti", "ca")
                While filtroParticelle.Contains("ca.APPEZZA")
                    Dim sezione = filtroParticelle.Substring(filtroParticelle.IndexOf("ca.APPEZZA"), filtroParticelle.IndexOf(")", filtroParticelle.IndexOf("ca.APPEZZA")) - filtroParticelle.IndexOf("ca.APPEZZA"))
                    filtroParticelle = filtroParticelle.Replace(" AND " + sezione, "")
                End While
            Case "azienda"
                'Formatto il filtro per la query SQL in base alle aziende fornite
                filtroParticelle = Str_FiltroImpianti.ToString().Replace("Imprese", "ca")

        End Select

        '------------------------------------------------
        'PARTICELLE ASSOCIATE AD APPEZZAMENTI 
        '------------------------------------------------

        stbQ.AppendLine("WITH Cte_Catasto AS ( 	")
        stbQ.AppendLine("   SELECT 	")
        stbQ.AppendLine("       ixp.PIVA 	")
        stbQ.AppendLine("       , ixp.sa_cod  	")
        stbQ.AppendLine("       , ixp.PROV     	")
        stbQ.AppendLine("       , ISNULL(i.COMUNI_PROV, ISNULL(ipc.COMUNI_PROV, '')) AS prov_des 	")
        stbQ.AppendLine("       , ixp.COM 	")
        stbQ.AppendLine("       , ISNULL(i.LOCALITA, ISNULL(ipc.LOCALITA, '')) AS com_des 	")
        stbQ.AppendLine("       , ixp.SEZIONE AS sez_cens 	")
        stbQ.AppendLine("       , ixp.NUMERO AS partic 	")
        stbQ.AppendLine("       , ixp.FOGLIO AS foglio 	")
        stbQ.AppendLine("       , ixp.SUBALTERNO AS sub 	")
        stbQ.AppendLine("       , CASE ISNULL(ri.CUL_COD, 0) WHEN 0 THEN ISNULL(cod.descrizione, '') ELSE ISNULL(sv.Veg_Des, '') END AS Utilizzo 	")
        stbQ.AppendLine("       , CASE ixp.TitoloPossesso WHEN 1 THEN 'Proprietà' 	")
        stbQ.AppendLine("           WHEN 1 THEN 'Proprietà' 	")
        stbQ.AppendLine("           WHEN 2 THEN 'Comodato d''uso'        	")
        stbQ.AppendLine("           WHEN 3 THEN 'Affitto con contratto' 	")
        stbQ.AppendLine("           WHEN 4 THEN 'Affitto senza contratto'        	")
        stbQ.AppendLine("           WHEN 5 THEN 'In conto terzi' 	")
        stbQ.AppendLine("           WHEN 6 THEN 'In convenzione' ELSE 'Altro' 	")
        stbQ.AppendLine("       END AS possesso_des   	")
        stbQ.AppendLine("       , ISNULL(axp.AREA, 0) AS AREA        	")
        stbQ.AppendLine("   FROM Centri_Aziendali ca    	")
        stbQ.AppendLine("   INNER JOIN ImpreseXParticelle AS ixp  	")
        stbQ.AppendLine("   ON ixp.PIVA = ca.PIVA 	")
        stbQ.AppendLine("   AND ixp.sa_cod = ca.SA_COD    	")
        stbQ.AppendLine("   LEFT JOIN AppezzamentiXParticelle axp  	")
        stbQ.AppendLine("   ON axp.PIVA = ixp.PIVA 	")
        stbQ.AppendLine("   AND axp.SA_COD = ixp.sa_cod 	")
        stbQ.AppendLine("   AND axp.PROV = ixp.PROV 	")
        stbQ.AppendLine("   AND axp.COM = ixp.COM 	")
        stbQ.AppendLine("   AND axp.SEZIONE = ixp.SEZIONE 	")
        stbQ.AppendLine("   AND axp.FOGLIO = ixp.FOGLIO 	")
        stbQ.AppendLine("   AND axp.NUMERO = ixp.NUMERO 	")
        stbQ.AppendLine("   AND axp.SUBALTERNO = ixp.SUBALTERNO 	")
        stbQ.AppendLine("   LEFT JOIN Reg_Impianti ri 	")
        stbQ.AppendLine("   ON axp.PIVA = ri.PIVA 	")
        stbQ.AppendLine("   AND axp.SA_COD = ri.SA_COD 	")
        stbQ.AppendLine("   AND axp.APPEZZA = ri.APPEZZA  	")
        stbQ.AppendLine("   LEFT JOIN Cultivar cv  	")
        stbQ.AppendLine("   ON ri.CUL_COD = cv.Cul_Cod    	")
        stbQ.AppendLine("   LEFT JOIN SpecieVegetali sv  	")
        stbQ.AppendLine("   ON cv.Veg_Cod = sv.Veg_Cod    	")
        stbQ.AppendLine("   LEFT JOIN ISTAT i     	")
        stbQ.AppendLine("   ON i.PROV = axp.PROV")
        stbQ.AppendLine("   AND i.COM = axp.COM")
        stbQ.AppendLine("   LEFT JOIN ParticelleCatastali pc ")
        stbQ.AppendLine("   ON pc.PROV = ixp.PROV    	")
        stbQ.AppendLine("   AND pc.COM = ixp.COM    	")
        stbQ.AppendLine("   AND pc.SEZIONE = ixp.SEZIONE    	")
        stbQ.AppendLine("   AND pc.FOGLIO = ixp.FOGLIO    	")
        stbQ.AppendLine("   AND pc.NUMERO = ixp.NUMERO    	")
        stbQ.AppendLine("   AND pc.SUBALTERNO = ixp.SUBALTERNO    	")
        stbQ.AppendLine("   LEFT JOIN ISTAT ipc ")
        stbQ.AppendLine("   ON ipc.PROV = pc.PROV ")
        stbQ.AppendLine("   AND ipc.COM = pc.COM    	")
        stbQ.AppendLine("   LEFT JOIN Reg_Impianti_Codici ric  	")
        stbQ.AppendLine("   ON ric.PIVA = ri.PIVA 	")
        stbQ.AppendLine("   AND ric.SA_COD = ri.SA_COD 	")
        stbQ.AppendLine("   AND ric.APPEZZA = ri.APPEZZA 	")
        stbQ.AppendLine("   AND ric.Id_Reg = ri.ID_REG 	")
        stbQ.AppendLine("   AND ric.Progetto_Cod = 0 	")
        stbQ.AppendLine("   AND ric.id_cod BETWEEN 3000 AND 4000  	")
        stbQ.AppendLine("   LEFT JOIN Codici_Anagrafe cod  	")
        stbQ.AppendLine("   ON cod.codice = ric.id_cod  	")
        stbQ.AppendLine("   WHERE 1 = 1 	")
        stbQ.AppendLine(filtroParticelle)
        stbQ.AppendLine(") 	")
        stbQ.AppendLine("SELECT Imprese.partitaIvaReale AS PIVA, x.sa_cod, x.PROV, x.prov_des, x.COM, x.com_des,")
        stbQ.AppendLine("   x.sez_cens,	x.partic, x.foglio, x.sub, x.Utilizzo, x.possesso_des, x.sup_specie")
        stbQ.AppendLine("FROM ( 	")
        stbQ.AppendLine("   SELECT 	")
        stbQ.AppendLine("       Catasto.PIVA, 	")
        stbQ.AppendLine("       sa_cod, 	")
        stbQ.AppendLine("       Catasto.PROV, 	")
        stbQ.AppendLine("       Catasto.prov_des, 	")
        stbQ.AppendLine("       Catasto.COM, 	")
        stbQ.AppendLine("       Catasto.com_des, 	")
        stbQ.AppendLine("       sez_cens, 	")
        stbQ.AppendLine("       partic, 	")
        stbQ.AppendLine("       foglio, 	")
        stbQ.AppendLine("       sub, 	")
        stbQ.AppendLine("       Utilizzo, 	")
        stbQ.AppendLine("       possesso_des, 	")
        stbQ.AppendLine("       SUM(area) AS sup_specie   	")
        stbQ.AppendLine("   FROM Cte_Catasto AS Catasto   	")
        stbQ.AppendLine("   WHERE 1 = 1  	")
        stbQ.AppendLine("   GROUP BY PIVA, sa_cod, Catasto.prov, Catasto.prov_des, Catasto.COM, Catasto.com_des, sez_cens, partic, Catasto.foglio, sub, Catasto.Utilizzo, possesso_des 	")
        stbQ.AppendLine("UNION ")
        stbQ.AppendLine("   SELECT 	")
        stbQ.AppendLine("       Catasto.PIVA, 	")
        stbQ.AppendLine("       sa_cod, 	")
        stbQ.AppendLine("       Catasto.PROV, 	")
        stbQ.AppendLine("       Catasto.prov_des, 	")
        stbQ.AppendLine("       Catasto.COM, 	")
        stbQ.AppendLine("       Catasto.com_des, 	")
        stbQ.AppendLine("       sez_cens, 	")
        stbQ.AppendLine("       partic, 	")
        stbQ.AppendLine("       Catasto.foglio, 	")
        stbQ.AppendLine("       sub, 	")
        stbQ.AppendLine("       '' AS Utilizzo, 	")
        stbQ.AppendLine("       possesso_des, 	")
        stbQ.AppendLine("       convert(float, (cast (pc.ETTARI as varchar(100)) + '.' + right('000' + cast (pc.Are as varchar(100)), 2) + right('000' + cast (pc.CentiAre as varchar(100)), 2))) - SUM(catasto.area) AS sup_specie")
        stbQ.AppendLine("   FROM Cte_Catasto Catasto   	")
        stbQ.AppendLine("   LEFT JOIN ParticelleCatastali pc ")
        stbQ.AppendLine("   ON pc.PROV = Catasto.PROV 	")
        stbQ.AppendLine("   AND pc.COM = Catasto.COM 	")
        stbQ.AppendLine("   AND pc.SEZIONE = Catasto.sez_cens 	")
        stbQ.AppendLine("   AND pc.FOGLIO = Catasto.FOGLIO 	")
        stbQ.AppendLine("   AND pc.NUMERO = Catasto.partic 	")
        stbQ.AppendLine("   AND pc.SUBALTERNO = catasto.sub   	")
        stbQ.AppendLine("   GROUP BY PIVA, sa_cod, Catasto.prov, Catasto.prov_des, Catasto.COM, Catasto.com_des, sez_cens, partic, Catasto.foglio, sub, possesso_des,      	")
        stbQ.AppendLine("       convert(float, (cast(pc.ETTARI as varchar(100))+ '.' + right('000' + cast (pc.Are as varchar(100)), 2) + right('000' + cast (pc.CentiAre as varchar(100)), 2))) 	")
        stbQ.AppendLine(") as x 	")
        stbQ.AppendLine("JOIN Imprese ")
        stbQ.AppendLine("ON x.PIVA = Imprese.PIVA")
        stbQ.AppendLine("WHERE 1 = 1 	")
        stbQ.AppendLine("ORDER BY FOGLIO 	")

        strErr = Nothing

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQ.ToString, "SchedaAziendale.CaricaSuperficieSpeciexParticelle", DSSuperficieSpeciexParticelle, DSSuperficieSpeciexParticelle.DT_SuperficieSpeciexParticelle.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

        End If


    End Sub

    Sub CaricaDs_Loghi(ByVal DsLoghi As Ds_Loghi)

        'DsLoghi.LoghiDataTable.NewLoghiRow()
        'Dim r As Ds_Loghi.LoghiRow = DsLoghi.Loghi.NewLoghiRow()
        Dim r As DataRow = DsLoghi.Loghi.NewLoghiRow()
        Dim x(0) As Byte
        x(0) = 0
        Dim bmpFx As System.Drawing.Bitmap = New System.Drawing.Bitmap(1, 1)
        bmpFx.SetPixel(0, 0, System.Drawing.Color.White)
        Dim logox() As Byte
        Dim cx As New System.Drawing.ImageConverter
        logox = cx.ConvertTo(bmpFx, GetType(Byte()))
        r.Item("Blob_Logo") = logox


        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C: \AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                If Not objWebConfig.Path_Directory_Loghi_Cliente.EndsWith("\") Then
                    objWebConfig.Path_Directory_Loghi_Cliente &= "\"
                End If
                Dim inserito As Boolean = False
                ' metto l'immagine1

                Try
                    Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard.bmp"
                    Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                    Dim logo() As Byte
                    Dim c As New System.Drawing.ImageConverter
                    logo = c.ConvertTo(bmpF, GetType(Byte()))
                    Dim i As Integer = 0
                    'For i = 0 To DsLoghi.Loghi.Rows.Count - 1
                    'DsLoghi.Loghi.Rows(i).Item("Blob_Logo") = logo
                    r.Item("Blob_Logo") = logo
                    inserito = True
                    ' Next
                Catch ex As Exception

                End Try

                'If inserito Then
                DsLoghi.Loghi.Rows.Add(r)
                DsLoghi.Loghi.AcceptChanges()
                'End If

            End If

        Catch ex As Exception

        End Try

    End Sub

End Class