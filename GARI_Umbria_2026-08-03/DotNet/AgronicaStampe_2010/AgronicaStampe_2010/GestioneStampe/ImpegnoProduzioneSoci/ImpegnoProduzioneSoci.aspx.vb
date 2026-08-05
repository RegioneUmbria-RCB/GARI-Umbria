Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreStampeDAL

Public Class ImpegnoProduzioneSoci
    Inherits System.Web.UI.Page

    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private rptImpegnoProdSoci As Rpt_ImpegnoProduzioneSoci_3
    Private subrptParticelleCatastali As Rpt_ParticelleCatastali
    Private subrptSuperficieSpecie As Rpt_SuperficieSpecie
    Private Log_Errori As String
    Private nomeDoc As String

    '----- Gestione Querystring
    Private Piva As String
    Dim rag_soc, anno As String
    Dim TipoArchivio, TipoIntervalloTemp As Integer
    Dim validita_inizio, validita_fine As Date

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rptImpegnoProdSoci = New Rpt_ImpegnoProduzioneSoci_3
        subrptParticelleCatastali = New Rpt_ParticelleCatastali
        subrptSuperficieSpecie = New Rpt_SuperficieSpecie
        Log_Errori = ""
        nomeDoc = ""

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        rag_soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                                 AgroKey_EncoderDecoder,
                                 Server)

        TipoArchivio = CInt(Stringa_Decodifica(Request.QueryString("ta").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server))

        TipoIntervalloTemp = CInt(Stringa_Decodifica(Request.QueryString("tit").ToString,
                                     AgroKey_EncoderDecoder,
                                     Server))

        Dim strValiditaInizio As String = Stringa_Decodifica(Request.QueryString("vi").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        If Not Date.TryParse(strValiditaInizio, validita_inizio) Then
            validita_inizio = Date.Now
        End If

        anno = validita_inizio.Year
        validita_fine = New Date(anno, 12, 31)

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        nomeDoc = "ImpegnoProduzioneSoci"

        If Not Me.IsPostBack Then

            Try
                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_ProdSoci()

            Catch ex As Exception
                Log_Errori &= "- Lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                'NOTA Come miglioramento, creare una enum_CategorieDocumenti specifica. Non è obbligatorio in quanto il file generato lo elimino
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                Dim rnd As New Random
                Dim nomeDoc_Estensione = nomeDoc & "_" & rnd.Next() & ".pdf"

                'Salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptImpegnoProdSoci,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               nomeDoc_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)
                'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
                'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)
                rptImpegnoProdSoci.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = nomeDoc & ", Partita Iva = " & CStr(Piva) & vbCrLf & vbCrLf & Log_Errori

                Nome_File_Log = "Log_Errori_" & nomeDoc & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_ImpegnoProduzione",
                                                 Nome_File_Log,
                                                 Session("ASG_Utente_Username"),
                                                 nomeDoc,
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                            "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(nomeDoc, AgroKey_EncoderDecoder, Server))


        End If


    End Sub


    Private Sub Stampa_ProdSoci()

        Dim dsReportParticelleCatastali As New DS_ParticelleCatastali()
        Dim dsReportSuperficieSpecie As New DS_SuperficieSpecie()
        Dim Vet_Intestazione(25) As String
        Dim handleStampeOP As New Stampe_OP()
        Dim dtParticelle As New DataTable()
        Dim handleParticelle As New ParticelleCatastali_R()
        Dim dtSuperficieSpecie As New DataTable()
        Dim pivaReale As String

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessione(False, objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            pivaReale = objImp.Leggi_PivaReale(Piva, objParametri_Server)

            'prelevo i dati dell'intestazione
            handleStampeOP.DatiIntestazioneSocio(Piva, rag_soc, Vet_Intestazione, Log_Errori, objParametri_Server)

            'prelevo i dati per i sottoreport
            'Carica_DS_ParticelleCatastali(dsParticelleCatastali)
            dtParticelle = handleParticelle.Anagrafica_Particelle_Leggi(Piva, 0, validita_inizio, validita_fine, objParametri_Server)

            Select Case TipoArchivio
                Case 0 'AGRONICA PLANNING
                    dtSuperficieSpecie = handleStampeOP.SuperficieSpeciePrevisionale(objParametri_Server, Piva, TipoIntervalloTemp, validita_inizio, validita_fine)

                Case 1 'GIAS
                    dtSuperficieSpecie = handleStampeOP.SuperficieSpecieConsuntivo(objParametri_Server, Piva, TipoIntervalloTemp, validita_inizio, validita_fine)
            End Select

        Catch ex As Exception
            Log_Errori &= "- lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        Finally
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End Try


        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------
        Try

            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextSuperUser"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(23).ToUpper 'Contiene la ragione sociale dell'impresa superuser
            'CType(rptImpegnoProdSoci.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = data_stampa
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = anno
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextRapprLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(1).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextComNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(2).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextProvNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(3).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextDataNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(4).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextIndResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(5).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextComResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(6).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextProvResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(7).ToUpper
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextQualifica"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(8).ToUpper

            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(10)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextIndImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(11)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextComImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(12)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextProvImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(13)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextCooperativa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(15)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextIndCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(16)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextCapCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(17)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextComCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(18)
            CType(rptImpegnoProdSoci.Section1.ReportObjects("TextProvCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(19)
            'Sostituiti dai parametri:
            'CType(rptImpegnoProdSoci.Section1.ReportObjects("TextNumIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(20)
            'CType(rptImpegnoProdSoci.Section1.ReportObjects("TextDataIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(21)

            Dim drReport As DataRow

            For Each dr As DataRow In dtParticelle.Rows
                drReport = dsReportParticelleCatastali.DT_ParticelleCatastali.NewRow

                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.PIVAColumn) = pivaReale
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.PROVColumn) = dr.Field(Of String)("PROV")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.COMColumn) = dr.Field(Of String)("COM")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.LOCALITAColumn) = dr.Field(Of String)("LOCALITA")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.COMUNI_PROVColumn) = dr.Field(Of String)("COMUNI_PROV")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.CAPColumn) = dr.Field(Of String)("CAP")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.SEZIONEColumn) = dr.Field(Of String)("SEZIONE")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.FOGLIOColumn) = dr.Field(Of Integer)("FOGLIO")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.NUMEROColumn) = dr.Field(Of Integer)("NUMERO")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.SUBALTERNOColumn) = dr.Field(Of String)("SUBALTERNO")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.TitoloPossessoColumn) = dr.Field(Of Integer)("TitoloPossesso")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.TitoloPossessoDescColumn) = dr.Field(Of String)("TitoloPossesso_Des") & " (" & dr.Field(Of String)("Sa_Nome") & ")"
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.ETTARIColumn) = dr.Field(Of Double)("ETTARI")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.AREColumn) = dr.Field(Of Integer)("ARE")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.CENTIAREColumn) = dr.Field(Of Integer)("CENTIARE")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.sa_codColumn) = dr.Field(Of Integer)("sa_cod")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.Part_CodColumn) = dr.Field(Of Integer)("Part_Cod")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.Validita_InizioColumn) = AGRODATAINIZIO 'Date di Validità di ParticelleCatastali, non sono usate dall'rpt
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.Validita_FineColumn) = AGRODATAFINE
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.Validita_Inizio_2Column) = dr.Field(Of Date)("Validita_Inizio") 'Date di Validità di ImpreseXParticelle, non sono usate dall'rpt
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastali.Validita_Fine_2Column) = dr.Field(Of Date)("Validita_Fine")

                dsReportParticelleCatastali.DT_ParticelleCatastali.Rows.Add(drReport)
            Next

            For Each dr As DataRow In dtSuperficieSpecie.Rows
                drReport = dsReportSuperficieSpecie.DT_SuperficieSpecie.NewRow

                drReport(dsReportSuperficieSpecie.DT_SuperficieSpecie.veg_desColumn) = dr.Field(Of String)("Veg_Des")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpecie.sup_specieColumn) = dr.Field(Of Double)("Sup_Specie")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpecie.str_sup_specieColumn) = Format(dr.Field(Of Double)("Sup_Specie"), "0.0000")

                dsReportSuperficieSpecie.DT_SuperficieSpecie.Rows.Add(drReport)
            Next

        Catch ex As Exception
            Log_Errori &= "- valorizzazione dati in report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try


        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            subrptParticelleCatastali.SetDataSource(dsReportParticelleCatastali)
            subrptSuperficieSpecie.SetDataSource(dsReportSuperficieSpecie)

            rptImpegnoProdSoci.OpenSubreport("Rpt_ParticelleCatastali.rpt").SetDataSource(dsReportParticelleCatastali)
            rptImpegnoProdSoci.OpenSubreport("Rpt_SuperficieSpecie.rpt").SetDataSource(dsReportSuperficieSpecie)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try


        '--------------------------------------------
        ' IMPOSTAZIONE PARAMETRI
        '(va fatto dopo il SetDataSource altrimenti da errore! )
        '--------------------------------------------
        Try
            rptImpegnoProdSoci.SetParameterValue("LibroSociCodice", Vet_Intestazione(20))
            rptImpegnoProdSoci.SetParameterValue("LibroSociData", Vet_Intestazione(21))
        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

    End Sub

    ''' <summary>
    ''' Query importata da Stampe2003, mantenuta per riferimento
    ''' </summary>
    ''' <param name="DSParticelleCatastali"></param>
    Private Sub Carica_DS_ParticelleCatastali(ByRef DSParticelleCatastali As DS_ParticelleCatastali)

        Dim handleParticelleCatastali As New ParticelleCatastali_R
        Dim stbQ As New System.Text.StringBuilder

        stbQ.AppendLine(" SELECT    ParticelleCatastali.PROV, ParticelleCatastali.COM, ISTAT.LOCALITA, ISTAT.COMUNI_PROV,  ISTAT.CAP,  ")
        stbQ.AppendLine("           ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO,  ")
        stbQ.AppendLine("           ImpreseXParticelle.TitoloPossesso, ' ' AS TitoloPossessoDesc, ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ")
        stbQ.AppendLine("           ParticelleCatastali.CENTIARE, ImpreseXParticelle.PIVA,  ImpreseXParticelle.sa_cod, ParticelleCatastali.PART_COD, ")
        stbQ.AppendLine("           ParticelleCatastali.Validita_Inizio, ParticelleCatastali.Validita_Fine, ImpreseXParticelle.Validita_Inizio AS Validita_Inizio_2, ImpreseXParticelle.Validita_Fine AS Validita_Fine_2")
        stbQ.AppendLine(" FROM      ParticelleCatastali INNER JOIN ")
        stbQ.AppendLine("           ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM INNER JOIN ")
        stbQ.AppendLine("           ImpreseXParticelle ON ParticelleCatastali.PROV = ImpreseXParticelle.PROV AND ParticelleCatastali.COM = ImpreseXParticelle.COM AND ")
        stbQ.AppendLine("           ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE AND ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO AND ")
        stbQ.AppendLine("           ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO AND ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")
        stbQ.AppendLine(" WHERE     ImpreseXParticelle.PIVA = '" & UtilityProvider.Agro_SQL_SaveText(Piva) & "' ")
        stbQ.AppendLine(" AND       ImpreseXParticelle.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validita_fine))
        stbQ.AppendLine(" AND       ImpreseXParticelle.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validita_inizio))
        stbQ.AppendLine(" ORDER BY  ParticelleCatastali.PROV, ParticelleCatastali.COM, ParticelleCatastali.SEZIONE, ParticelleCatastali.FOGLIO, ParticelleCatastali.NUMERO, ParticelleCatastali.SUBALTERNO ")

        Dim dt = handleParticelleCatastali.EseguiQuery_Lettura(objParametri_Server, stbQ.ToOrigin, "")

        For Each dr As DataRow In dt.Rows

            Dim drReport = DSParticelleCatastali.DT_ParticelleCatastali.NewRow

            'verifico il titolo di possesso e inserisco la descrizione
            Select Case CInt(dr.Item("TitoloPossesso"))

                Case 0
                    dr.Item("TitoloPossessoDesc") = "Altro"

                Case 1
                    dr.Item("TitoloPossessoDesc") = "Proprietà"

                Case 2
                    dr.Item("TitoloPossessoDesc") = "Comodato d'uso"

                Case 3
                    dr.Item("TitoloPossessoDesc") = "Affitto con contratto"

                Case 4
                    dr.Item("TitoloPossessoDesc") = "Affitto senza contratto"

                Case 5
                    dr.Item("TitoloPossessoDesc") = "In conto terzi"

            End Select

            drReport.ItemArray = dr.ItemArray
            DSParticelleCatastali.DT_ParticelleCatastali.Rows.Add(drReport)

        Next

    End Sub

End Class