Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class LiquidazioneIVA_2
    Inherits System.Web.UI.Page

    Private rptLiquidazioneIVA As Rpt_LiquidazioneIVA_2
    Private rptIVACredito As Rpt_SottoReport_IVA
    Private rptIVADebito As Rpt_SottoReportIVA_Duplicato

    Dim Piva, Conto As String
    Dim Log_Errori As String = ""
    Dim Data_Inizio As String
    Dim Data_Fine As String
    Dim Iva_Precedente As Double
    Dim Acconto As Double
    ' Dim Credito As String
    'Dim Debito As String
    'Dim Saldo As String
    Dim SegnoSaldo As String
    Dim Num_Pagina As Integer
    Dim Sezionale_Des As String
    Dim RegimeIva As enum_RegimeIva
    Dim InteresseDebitoIva_Perc As Double
    'dim InteresseDebitoIva_Valore, SaldoFinale As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " Liquidazione IVA "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        InitializeComponent()
        rptLiquidazioneIVA = New Rpt_LiquidazioneIVA_2
        rptIVACredito = New Rpt_SottoReport_IVA
        rptIVADebito = New Rpt_SottoReportIVA_Duplicato
    End Sub

#End Region

    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        Iva_Precedente = Stringa_Decodifica(CStr(Request.QueryString("ivaprec")),
                                  AgroKey_EncoderDecoder,
                                  Server)

        Acconto = Stringa_Decodifica(CStr(Request.QueryString("acc")),
                          AgroKey_EncoderDecoder,
                          Server)

        'credito = Stringa_Decodifica(CStr(Request.QueryString("cred")), _
        '          AgroKey_EncoderDecoder, _
        '          Server)

        'Debito = Stringa_Decodifica(CStr(Request.QueryString("deb")), _
        '          AgroKey_EncoderDecoder, _
        '          Server)

        'Saldo = Stringa_Decodifica(CStr(Request.QueryString("saldo")), _
        '          AgroKey_EncoderDecoder, _
        '          Server)

        Num_Pagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")),
                    AgroKey_EncoderDecoder,
                    Server))

        Sezionale_Des = Stringa_Decodifica(CStr(Request.QueryString("szd")),
                        AgroKey_EncoderDecoder,
                        Server)

        RegimeIva = Stringa_Decodifica(CStr(Request.QueryString("regiv")),
                                     AgroKey_EncoderDecoder,
                                     Server)

        InteresseDebitoIva_Perc = Stringa_Decodifica(CStr(Request.QueryString("idip")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        'InteresseDebitoIva_Valore = Stringa_Decodifica(CStr(Request.QueryString("idiv")), _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)

        'SaldoFinale = Stringa_Decodifica(CStr(Request.QueryString("saldofin")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer
        Dim IdentificazioneDocumento As String
        Dim Nome_Documento As String = "LiquidazioneIVA"

        If Not Me.IsPostBack Then

            ' CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim DSIvaCredito As New DS_SottoReport_IVA
            Dim DSIvaDebito As New DS_SottoReportIVA_Duplicato
            Dim DSLiqIVA As New DS_LiquidazioneIVA

            Try

                StampaLiquidazioneIVA(rptLiquidazioneIVA, DSLiqIVA, DSIvaCredito, DSIvaDebito)

            Catch ex As Exception
                Log_Errori += "- Intestazione: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = Data_Inizio
                Data_Fine_Allegati = Data_Fine

                IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.LiquidazioneIVA, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptLiquidazioneIVA,
                                           enum_CategorieDocumenti.Bilancio,
                                           Sottocartella,
                                           Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf",
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                                 enum_CategorieDocumenti.LiquidazioneIVA,
                                                                 Nome_Documento,
                                                                 Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf",
                                                                 Sottocartella,
                                                                 "", "", "", "",
                                                                 Data_Inizio_Allegati,
                                                                 Data_Fine_Allegati,
                                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptLiquidazioneIVA
            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptLiquidazioneIVA.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            DSIvaCredito.Dispose()
            DSIvaCredito = Nothing
            DSIvaDebito.Dispose()
            DSIvaDebito = Nothing
            DSLiqIVA.Dispose()
            DSLiqIVA = Nothing

            rptLiquidazioneIVA.Close()
            rptLiquidazioneIVA.Dispose()
            rptLiquidazioneIVA = Nothing

            GC.Collect()

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String
            '  Dim Nome_Documento As String = "LiquidazioneIVA"

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Piva) +
                               vbCrLf + vbCrLf + Log_Errori


                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Contabilita",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "LiquidazioneIVA_2.aspx",
                                                 Log_Errori)

            End If
        End If
    End Sub


    '#####################################################################################################
    Private Sub StampaLiquidazioneIVA(ByRef rptLiqIVA As Rpt_LiquidazioneIVA_2, _
                                      ByRef DSLiqIVA As DS_LiquidazioneIVA, _
                                      ByRef DSIvaCredito As DS_SottoReport_IVA, _
                                      ByRef DSIvaDebito As DS_SottoReportIVA_Duplicato)


        Try

            Dim DT_Intestazione As DataTable
            Dim i As Integer
            Dim str_errore As String = ""
            Dim objHLP As New AgronicaCoreContabHLP.Contabilita

            Dim objInt As New AgronicaCoreStampeDAL.DocContab
            DT_Intestazione = objInt.DatiIntestazioneImpresa(Piva, _
                                                             str_errore, _
                                                             "", "", _
                                                             objParametri_Server)


            If str_errore = "" Then

                If DT_Intestazione.Rows.Count <> 0 Then

                    CType(rptLiqIVA.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
                    CType(rptLiqIVA.Section1.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
                    CType(rptLiqIVA.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
                    CType(rptLiqIVA.Section1.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "

                    CType(rptLiqIVA.Section1.ReportObjects("TxtDataInizio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
                    CType(rptLiqIVA.Section1.ReportObjects("TxtDataFine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Fine
                    CType(rptLiqIVA.Section1.ReportObjects("TxtRegimeIva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Regime Iva: " & objHLP.RegimeIVA_Desc_from_Cod(RegimeIva)
                    CType(rptLiqIVA.Section1.ReportObjects("TxtSezionale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sezionale: " & Sezionale_Des

                End If

            Else
                Log_Errori += str_errore & vbCrLf
            End If

        Catch ex As Exception
            Log_Errori += "Intestazione: " & ex.Message & vbCrLf
        End Try

        Try

            Dim objSottoIva As New cls_SottoReport_IVA
            Dim DT_IVA_acquisti, DT_IVA_vendite As DataTable

            DT_IVA_acquisti = Session("DT_IVA_Acquisti_Generale")
            DT_IVA_vendite = Session("DT_IVA_Vendite_Generale")

            Select Case RegimeIva

                Case enum_RegimeIva.Ordinario
                    'regime ordinario:
                    'iva a credito - iva a debito

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale(DSIvaCredito, DT_IVA_acquisti, "IVA Acquisti")

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE(DSIvaDebito, DT_IVA_vendite, "IVA Vendite")

                Case enum_RegimeIva.Speciale
                    'regime speciale:
                    'iva a debito - % compensazione

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale(DSIvaCredito, DT_IVA_acquisti, "IVA Acquisti")

                    'carico il dataset vuoto per non mandare il errore il sottoreport
                    'objSottoIva.Carica_DSSottoReportIVA_vuoto_VENDITE(DSIvaCredito)

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE(DSIvaDebito, DT_IVA_vendite, "IVA Vendite")
                    'nascondo la sezione dell'iva a credito
                    'rptLiquidazioneIVA.ReportHeaderSection1.SectionFormat.EnableSuppress = True

                Case enum_RegimeIva.EsenzioneIva

                    objSottoIva.Carica_DSSottoReportIVA_vuoto_VENDITE(DSIvaCredito)

                    objSottoIva.Carica_DSSottoReportIVA_duplicato_vuoto(DSIvaDebito)

                Case enum_RegimeIva.NonImpostato
                    Throw New Exception("Non è stato inviato il filtro sul RegimeIva.")
            End Select



        Catch ex As Exception
            Log_Errori += "Liquidazione: " & ex.Message & vbCrLf
        End Try

        Try
            Dim Iva_Detraibile As Double = 0
            Dim Iva_NonCompensazione As Double = 0
            Dim Iva_CompensazioneEstero As Double = 0
            Dim Differenza As Double = 0
            Dim InteresseDebitoIva_Valore As Double = 0
            Dim SaldoFinale As Double = 0

            If Not IsNothing(DSIvaCredito) AndAlso DSIvaCredito.DT_SottoReport_IVA.Rows.Count > 0 Then
                Iva_Detraibile = DSIvaCredito.DT_SottoReport_IVA.Rows(DSIvaCredito.DT_SottoReport_IVA.Rows.Count - 1).Item("Detraibile_Imposta")
                'non devo usare tutta l'iva a credito, ma solo quella detraibile
            End If

            If Not IsNothing(DSIvaDebito) AndAlso DSIvaDebito.DT_SottoReportIVA_Duplicato.Rows.Count > 0 Then
                Iva_NonCompensazione = DSIvaDebito.DT_SottoReportIVA_Duplicato.Rows(DSIvaDebito.DT_SottoReportIVA_Duplicato.Rows.Count - 1).Item("NonCompensazione_Imposta")
                'non devo usare tutta l'iva a debito, ma solo quella non in compensazione

                Iva_CompensazioneEstero = DSIvaDebito.DT_SottoReportIVA_Duplicato.Rows(DSIvaDebito.DT_SottoReportIVA_Duplicato.Rows.Count - 1).Item("Compensazione_Export_Totale")
            End If

            Differenza = (Iva_Detraibile - Iva_NonCompensazione) + Iva_CompensazioneEstero + Acconto + Iva_Precedente

            If Differenza < 0 Then
                InteresseDebitoIva_Valore = (Differenza * InteresseDebitoIva_Perc) / 100
            End If

            SaldoFinale = Differenza + InteresseDebitoIva_Valore


            CType(rptLiqIVA.Section4.ReportObjects("TxtIvaCredito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_Detraibile, "##,###,##0.00")
            CType(rptLiqIVA.Section4.ReportObjects("TxtIvaDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_NonCompensazione, "##,###,##0.00")
            CType(rptLiqIVA.Section4.ReportObjects("TxtCompensazioneEstero"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_CompensazioneEstero, "##,###,##0.00")

            CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoPrec"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_Precedente, "##,###,##0.00")
            CType(rptLiqIVA.Section4.ReportObjects("TxtAcconto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Acconto, "##,###,##0.00")
            CType(rptLiqIVA.Section4.ReportObjects("TxtSaldo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Differenza, "##,###,##0.00")

            CType(rptLiqIVA.Section4.ReportObjects("TxtInteressiIvaDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(InteresseDebitoIva_Valore, "##,###,##0.00")
            CType(rptLiqIVA.Section4.ReportObjects("LblInteressi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Interessi " & Format(InteresseDebitoIva_Perc, "##,###,##0.00") & "% su Iva a Debito"
            CType(rptLiqIVA.Section4.ReportObjects("TxtSaldoFinale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(SaldoFinale, "##,###,##0.00")

            If CDbl(Differenza) < 0 Then
                SegnoSaldo = "= (debito)"
            Else
                SegnoSaldo = "= (credito)"
            End If

            CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo
            CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebito2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo

            If CDbl(Iva_Precedente) < 0 Then
                SegnoSaldo = "- (debito)"
            Else
                SegnoSaldo = "+ (credito)"
            End If

            CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebitoPREC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo


        Catch ex As Exception
            Log_Errori += "Riepilogo: " & ex.Message & vbCrLf
        End Try

        Try

            Dim DrR As DS_LiquidazioneIVA.DS_LiquidazioneIVARow
            DrR = DSLiqIVA.DS_LiquidazioneIVA.NewRow

            DrR.Num_Pagina = Num_Pagina

            DSLiqIVA.DS_LiquidazioneIVA.Rows.Add(DrR)

        Catch ex As Exception
            Log_Errori += "Numero Pagina: " & ex.Message & vbCrLf
        End Try


        Try
            'imposto il dataset sul report principale
            rptLiqIVA.SetDataSource(DSLiqIVA)
        Catch ex As Exception
            Log_Errori += "- SetDataSource: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il dataset sul sottoreport credito
            rptIVACredito.SetDataSource(DSIvaCredito)
        Catch ex As Exception
            Log_Errori += "- SetDataSource sottoreport credito: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il sottoreport credito sul report
            rptLiqIVA.OpenSubreport("Rpt_SottoReport_IVA.rpt").SetDataSource(DSIvaCredito)
        Catch ex As Exception
            Log_Errori += "- OpenSubreport credito: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il dataset sul sottoreport debito
            rptIVADebito.SetDataSource(DSIvaDebito)
        Catch ex As Exception
            Log_Errori += "- SetDataSource sottoreport debito: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il sottoreport debito sul report
            rptLiqIVA.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSIvaDebito)
        Catch ex As Exception
            Log_Errori += "- OpenSubreport debito: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub




End Class
