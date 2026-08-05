Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreStampeDAL
Imports System.IO

Public Class ImpegnoProduzioneSocixCentri
    Inherits System.Web.UI.Page

    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private rptImpegnoProdSociDivxCentri As Rpt_ImpegnoProduzioneSoci_DivisoxCentri
    Private subrptParticelleCatastali As Rpt_ParticelleCatastali
    Private subrptSuperficieSpecie As Rpt_SuperficieSpecie
    Private Log_Errori As String
    Private nomeDoc As String

    '----- Gestione Querystring
    Dim rag_soc, anno As String
    Dim TipoIntervalloTemp As Integer
    Dim validita_inizio, validita_fine As Date
    Dim Tipo_Selezione As String
    Dim fronteRetro As Boolean

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rptImpegnoProdSociDivxCentri = New Rpt_ImpegnoProduzioneSoci_DivisoxCentri
        subrptParticelleCatastali = New Rpt_ParticelleCatastali
        subrptSuperficieSpecie = New Rpt_SuperficieSpecie
        Log_Errori = ""
        nomeDoc = ""

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        rag_soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                                 AgroKey_EncoderDecoder,
                                 Server)

        'Commentato, ma andrà fornito dalla nuova pagina che chiamerà questa stampa.

        'TipoIntervalloTemp = CInt(Stringa_Decodifica(Request.QueryString("tit").ToString,
        '                             AgroKey_EncoderDecoder,
        '                             Server))

        'Valore temporaneo per far funzionare la pagina.
        TipoIntervalloTemp = 0

        'Dim strValiditaInizio As String = Stringa_Decodifica(Request.QueryString("vi").ToString,
        '                    AgroKey_EncoderDecoder,
        '                    Server)

        'If Not Date.TryParse(strValiditaInizio, validita_inizio) Then
        'End If

        anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)


        Tipo_Selezione = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        fronteRetro = Stringa_Decodifica(Request.QueryString("fronteretro").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        validita_inizio = New Date(anno, 1, 1)
        validita_fine = New Date(anno, 12, 31)

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim Str_FiltroImpianti = Session("strParametri").ToString()

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        nomeDoc = "Autocertificazione"

        If Not Me.IsPostBack Then

            Try
                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_ProdSociDivisoxCentri()

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
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptImpegnoProdSociDivxCentri,
                                            enum_CategorieDocumenti.RegistriCampagna,
                                            Sottocartella,
                                            nomeDoc_Estensione,
                                            objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)
                'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
                'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)
                rptImpegnoProdSociDivxCentri.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = nomeDoc & vbCrLf & vbCrLf & Log_Errori

                Nome_File_Log = "Log_Errori_" & nomeDoc & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                "Stampe_ImpegnoProduzioneDivisoxCentri",
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

    '''<summary>
    '''Query per versione alternativa report, con divisione per centri.
    '''</summary>
    Private Sub Stampa_ProdSociDivisoxCentri()

        Dim dsReportCentri As New DS_CentriConLogo()
        Dim dsReportParticelleCatastali As New DS_ParticelleCatastalixCentri()
        Dim dsReportSuperficieSpecie As New DS_SuperficieSpeciexCentri()
        Dim dsLoghi As New Ds_Loghi()
        Dim DSImprese As New Ds_Imprese_StampaMassiva
        Dim handleStampeOP As New Stampe_OP()
        Dim dtParticelle As New DataTable()
        Dim dtCentri As New DataTable()
        Dim handleCentri As New CentriAziendali_Read()
        Dim handleParticelle As New ParticelleCatastali_R()
        Dim dtSuperficieSpecie As New DataTable()
        Dim strParametri As Object

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessione(False, objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'prelevo i dati dell'intestazione

            'INTESTAZIONE

            Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            Dim dsImp As New DataSet
            dsImp.Tables.Add("listaPive")
            objStampe.Imprese_StampaMassiva(dsImp, dsImp.Tables(0).TableName, strFiltroImprese, Tipo_Selezione, objParametri_Server)

            Dim listaPive As List(Of String) = dsImp.Tables(0).AsEnumerable().AsParallel().
                Where(Function(row) Not row.IsNull("Piva")).
                Select(Function(row) row.Field(Of String)("Piva")).
                ToList()

            objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)

            dtCentri = handleCentri.LeggiInfoCentri_x_StampaAutocertificazione(ListaPive, 0, "", "", objParametri_Server, False)

            Dim listaChiaviCentri As List(Of (String, Integer)) = dtCentri.AsEnumerable().AsParallel().
            Where(Function(row) Not row.IsNull("Piva") AndAlso Not row.IsNull("Sa_Cod")).
            Select(Function(row) (row.Field(Of String)("Piva"), row.Field(Of Integer)("Sa_Cod"))).Distinct().ToList()

            dtParticelle = handleParticelle.Anagrafica_Particelle_Leggi_Per_Centri(listaChiaviCentri, validita_inizio, validita_fine, objParametri_Server)

            dtSuperficieSpecie = handleStampeOP.SuperficieSpecieConsuntivoxCentriMultiPiva(objParametri_Server, strParametri, ListaPive, TipoIntervalloTemp, validita_inizio, validita_fine, Tipo_Selezione)

            Session("strParametri") = Nothing

            CaricaDs_Loghi(dsLoghi)
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

            Dim drReport As DataRow

            For Each drCentri As DataRow In dtCentri.Rows

                drReport = dsReportCentri._DS_CentriConLogo.NewRow

                drReport(dsReportCentri._DS_CentriConLogo.PivaColumn) = drCentri.Field(Of String)("PivaReale")
                drReport(dsReportCentri._DS_CentriConLogo.IndirizzoColumn) = drCentri.Field(Of String)("ind_des") + " - " + drCentri.Field(Of String)("CAP") + ", " + drCentri.Field(Of String)("com_des") + " - " + drCentri.Field(Of String)("pro_cod")
                drReport(dsReportCentri._DS_CentriConLogo.Sa_CodColumn) = drCentri.Field(Of Integer)("sa_cod")
                drReport(dsReportCentri._DS_CentriConLogo.Sa_NomeColumn) = drCentri.Field(Of String)("sa_nome")
                drReport(dsReportCentri._DS_CentriConLogo.CAPColumn) = ""
                drReport(dsReportCentri._DS_CentriConLogo.CodiceColumn) = ""
                drReport(dsReportCentri._DS_CentriConLogo.ComuneColumn) = ""
                drReport(dsReportCentri._DS_CentriConLogo.ProvinciaColumn) = ""
                drReport(dsReportCentri._DS_CentriConLogo.RecapitiColumn) = ""

                drReport(dsReportCentri._DS_CentriConLogo.Blob_LogoColumn) = dsLoghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

                dsReportCentri._DS_CentriConLogo.Rows.Add(drReport)

            Next

            For Each drParticelle As DataRow In dtParticelle.Rows


                drReport = dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.NewRow

                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.PIVAColumn) = drParticelle.Field(Of String)("PivaReale")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.PROVColumn) = drParticelle.Field(Of String)("PROV")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.COMColumn) = drParticelle.Field(Of String)("COM")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.LOCALITAColumn) = drParticelle.Field(Of String)("LOCALITA")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.COMUNI_PROVColumn) = drParticelle.Field(Of String)("COMUNI_PROV")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.CAPColumn) = drParticelle.Field(Of String)("CAP")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.SEZIONEColumn) = If(drParticelle.Field(Of String)("SEZIONE") = "0", " ", drParticelle.Field(Of String)("SEZIONE"))
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.FOGLIOColumn) = drParticelle.Field(Of Integer)("FOGLIO")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.NUMEROColumn) = drParticelle.Field(Of Integer)("NUMERO")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.SUBALTERNOColumn) = If(drParticelle.Field(Of String)("SUBALTERNO") = "0", " ", drParticelle.Field(Of String)("SUBALTERNO"))
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.TitoloPossessoColumn) = drParticelle.Field(Of Integer)("TitoloPossesso")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.TitoloPossessoDescColumn) = drParticelle.Field(Of String)("TitoloPossesso_Des")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.sa_codColumn) = drParticelle.Field(Of Integer)("sa_cod")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Part_CodColumn) = drParticelle.Field(Of Integer)("Part_Cod")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Validita_InizioColumn) = AGRODATAINIZIO 'Date di Validità di ParticelleCatastali, non sono usate dall'rpt
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Validita_FineColumn) = AGRODATAFINE
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Validita_Inizio_2Column) = drParticelle.Field(Of Date)("Validita_Inizio") 'Date di Validità di ImpreseXParticelle, non sono usate dall'rpt
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Validita_Fine_2Column) = drParticelle.Field(Of Date)("Validita_Fine")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.ETTARIColumn) = drParticelle.Field(Of Double)("ETTARI")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.AREColumn) = drParticelle.Field(Of Integer)("ARE")
                drReport(dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.CENTIAREColumn) = drParticelle.Field(Of Integer)("CENTIARE")

                dsReportParticelleCatastali.DT_ParticelleCatastalixCentri.Rows.Add(drReport)
            Next

            For Each drSuperficie As DataRow In dtSuperficieSpecie.Rows

                drReport = dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.NewRow
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.pivaColumn) = drSuperficie.Field(Of String)("PivaReale")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.veg_desColumn) = drSuperficie.Field(Of String)("Veg_Des")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.sup_specieColumn) = drSuperficie.Field(Of Double)("Sup_Specie")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.str_sup_specieColumn) = Format(drSuperficie.Field(Of Double)("Sup_Specie"), "0.0000")
                drReport(dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.sa_codColumn) = drSuperficie.Field(Of Integer)("Sa_Cod")

                dsReportSuperficieSpecie.DT_SuperficieSpeciexCentri.Rows.Add(drReport)

            Next

        Catch ex As Exception
            Log_Errori &= "- valorizzazione dati in report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        CType(rptImpegnoProdSociDivxCentri.GroupHeaderSection1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = anno

        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            subrptParticelleCatastali.SetDataSource(dsReportParticelleCatastali)
            subrptSuperficieSpecie.SetDataSource(dsReportSuperficieSpecie)

            rptImpegnoProdSociDivxCentri.Database.Tables("DS_CentriConLogo").SetDataSource(dsReportCentri)
            rptImpegnoProdSociDivxCentri.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

            rptImpegnoProdSociDivxCentri.OpenSubreport("Rpt_ParticelleCatastalixCentri.rpt").SetDataSource(dsReportParticelleCatastali)
            rptImpegnoProdSociDivxCentri.OpenSubreport("Rpt_SuperficieSpeciexCentri.rpt").SetDataSource(dsReportSuperficieSpecie)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try


        '--------------------------------------------
        ' IMPOSTAZIONE PARAMETRI
        '(va fatto dopo il SetDataSource altrimenti da errore! )
        '--------------------------------------------

        ' imposto il parametro del fronte-retro
        rptImpegnoProdSociDivxCentri.SetParameterValue("fronteRetro", fronteRetro)

        Try

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

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
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
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