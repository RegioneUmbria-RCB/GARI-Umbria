Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class LiquidazioneIVA_3
    Inherits System.Web.UI.Page

    Private rptLiquidazioneIVA As Rpt_LiquidazioneIVA_2
    Private rptIVACredito As Rpt_SottoReport_IVA
    Private rptIVADebito As Rpt_SottoReportIVA_Duplicato

    Dim Piva, Conto As String
    Dim Log_Errori As String = ""
    Dim Data_Inizio As String
    Dim Data_Fine As String
    Dim Iva_Precedente As Decimal
    Dim Acconto As Decimal
    ' Dim Credito As String
    'Dim Debito As String
    'Dim Saldo As String
    Dim SegnoSaldo As String
    Dim Sezionale_Des As String
    Dim RegimeIva As enum_RegimeIva
    Dim InteresseDebitoIva_Perc As Decimal
    'dim InteresseDebitoIva_Valore, SaldoFinale As String

    Dim Param_Intervallo_Date As String = ""
    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CodFiscale As String = ""
    Dim Param_Indirizzo As String = ""
    Dim Param_NumPagina As Integer = 0
    Dim Param_Flag_Stampa_Data As Boolean

    Dim Param_IvaCredito As String = ""
    Dim Param_IvaDebito As String = ""
    Dim Param_CompensazioneEstero As String = ""
    Dim Param_CreditoPrec As String = ""
    Dim Param_Acconto As String = ""
    Dim Param_Saldo As String = ""
    Dim Param_InteressiIvaDebito As String = ""
    Dim Param_LblInteressi As String = ""
    Dim Param_SaldoFinale As String = ""
    Dim Param_CreditoDebito As String = ""
    Dim Param_CreditoDebito2 As String = ""
    Dim Param_CreditoDebitoPREC As String = ""

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


        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Iva_Precedente = Stringa_Decodifica(CStr(Request.QueryString("ivaprec")), _
                                  AgroKey_EncoderDecoder, _
                                  Server)

        Acconto = Stringa_Decodifica(CStr(Request.QueryString("acc")), _
                          AgroKey_EncoderDecoder, _
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

        Param_NumPagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")), _
                    AgroKey_EncoderDecoder, _
                    Server))


        Param_Flag_Stampa_Data = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsd")), _
                      AgroKey_EncoderDecoder, _
                      Server))


        Sezionale_Des = Stringa_Decodifica(CStr(Request.QueryString("szd")), _
                        AgroKey_EncoderDecoder, _
                        Server)

        RegimeIva = Stringa_Decodifica(CStr(Request.QueryString("regiv")), _
                                     AgroKey_EncoderDecoder, _
                                     Server)

        InteresseDebitoIva_Perc = Stringa_Decodifica(CStr(Request.QueryString("idip")), _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        'InteresseDebitoIva_Valore = Stringa_Decodifica(CStr(Request.QueryString("idiv")), _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)

        'SaldoFinale = Stringa_Decodifica(CStr(Request.QueryString("saldofin")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        Param_Rag_Soc = Stringa_Decodifica(CStr(Request.QueryString("pmr")), _
                             AgroKey_EncoderDecoder, _
                             Server)

        Param_Piva_CodFiscale = Stringa_Decodifica(CStr(Request.QueryString("pmp")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        Param_Indirizzo = Stringa_Decodifica(CStr(Request.QueryString("pmi")), _
                               AgroKey_EncoderDecoder, _
                               Server)
        Param_Intervallo_Date = Stringa_Decodifica(CStr(Request.QueryString("pmd")), _
                             AgroKey_EncoderDecoder, _
                             Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim IdentificazioneDocumento As String = ""
        Dim Nome_Documento As String = "LiquidazioneIVA"

        If Not Me.IsPostBack Then

            Dim DSIvaCredito As New DS_SottoReport_IVA
            Dim DSIvaDebito As New DS_SottoReportIVA_Duplicato
            'Dim DSLiqIVA As New DS_LiquidazioneIVA

            Try

                StampaLiquidazioneIVA(rptLiquidazioneIVA, DSIvaCredito, DSIvaDebito)

            Catch ex As Exception
                Log_Errori &= "- Intestazione: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Dim nomeFilePdf As String = ""

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = Data_Inizio
                Data_Fine_Allegati = Data_Fine

                IdentificazioneDocumento &= "_" & Format(Data_Inizio_Allegati, "yyyy_MM_dd") & "_" & Format(Data_Fine_Allegati, "yyyy_MM_dd")
                nomeFilePdf = Nome_Documento & "_p" & Piva & "_" & IdentificazioneDocumento & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.LiquidazioneIVA, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptLiquidazioneIVA,
                                           enum_CategorieDocumenti.Bilancio,
                                           sottoCartella, nomeFilePdf,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(Piva,
                                                                                enum_CategorieDocumenti.LiquidazioneIVA,
                                                                                Nome_Documento,
                                                                                nomeFilePdf, sottoCartella,
                                                                                "", "", "", "",
                                                                                Data_Inizio_Allegati,
                                                                                Data_Fine_Allegati,
                                                                                objParametri_Server)
                
            Catch ex As Exception
                Log_Errori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptLiquidazioneIVA
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptLiquidazioneIVA.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            DSIvaCredito.Dispose()
            DSIvaCredito = Nothing
            DSIvaDebito.Dispose()
            DSIvaDebito = Nothing
            'DSLiqIVA.Dispose()
            ' DSLiqIVA = Nothing

            rptLiquidazioneIVA.Close()
            rptLiquidazioneIVA.Dispose()
            rptLiquidazioneIVA = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Piva) &
                             vbCrLf & vbCrLf & Log_Errori
                
                Dim Nome_File As String = "Log_Errori_" & Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Contabilita",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "LiquidazioneIVA_3.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder)

            If PDF = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            End If

        End If

    End Sub


    '#####################################################################################################
    Private Sub StampaLiquidazioneIVA(ByRef rptLiqIVA As Rpt_LiquidazioneIVA_2, _
                                      ByRef DSIvaCredito As DS_SottoReport_IVA, _
                                      ByRef DSIvaDebito As DS_SottoReportIVA_Duplicato)

        '   ByRef DSLiqIVA As DS_LiquidazioneIVA, _

        '20/03/2018: nuova intestazione su tutti i report contabili

        'Try

        '    Dim DT_Intestazione As DataTable
        '    Dim i As Integer
        '    Dim str_errore As String = ""
        '    Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        '    Dim objInt As New AgronicaCoreStampeDAL.DocContab
        '    DT_Intestazione = objInt.DatiIntestazioneImpresa(Piva, _
        '                                                     str_errore, _
        '                                                     "", "", _
        '                                                     objParametri_Server)


        '    If str_errore = "" Then

        '        If DT_Intestazione.Rows.Count <> 0 Then

        '            CType(rptLiqIVA.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "

        '            CType(rptLiqIVA.Section1.ReportObjects("TxtDataInizio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtDataFine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Fine
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtRegimeIva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Regime Iva: " & objHLP.RegimeIVA_Desc_from_Cod(RegimeIva)
        '            CType(rptLiqIVA.Section1.ReportObjects("TxtSezionale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sezionale: " & Sezionale_Des

        '        End If

        '    Else
        '        Log_Errori += str_errore & vbCrLf
        '    End If

        'Catch ex As Exception
        '    Log_Errori += "Intestazione: " & ex.Message & vbCrLf
        'End Try

        Try

            Dim objSottoIva As New cls_SottoReport_IVA
            Dim DT_IVA_acquisti, DT_IVA_vendite As DataTable

            DT_IVA_acquisti = Session("DT_IVA_Acquisti_Generale")
            DT_IVA_vendite = Session("DT_IVA_Vendite_Generale")

            Select Case RegimeIva

                Case enum_RegimeIva.Ordinario
                    'regime ordinario:
                    'iva a credito - iva a debito

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_ACQUISTI_NEW(DSIvaCredito, DT_IVA_acquisti, "IVA Acquisti")

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(DSIvaDebito, DT_IVA_vendite, "IVA Vendite", objParametri_Server)

                Case enum_RegimeIva.Speciale
                    'regime speciale:
                    'iva a debito - % compensazione

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_ACQUISTI_NEW(DSIvaCredito, DT_IVA_acquisti, "IVA Acquisti")

                    'carico il dataset vuoto per non mandare il errore il sottoreport
                    'objSottoIva.Carica_DSSottoReportIVA_vuoto_VENDITE(DSIvaCredito)

                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(DSIvaDebito, DT_IVA_vendite, "IVA Vendite", objParametri_Server)
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
            Dim Iva_Detraibile As Decimal = 0
            Dim Iva_NonCompensazione As Decimal = 0
            Dim Iva_CompensazioneEstero As Decimal = 0
            Dim Differenza As Decimal = 0
            Dim InteresseDebitoIva_Valore As Decimal = 0
            Dim SaldoFinale As Decimal = 0

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

            Param_IvaCredito = Format(Iva_Detraibile, "##,###,##0.00")
            Param_IvaDebito = Format(Iva_NonCompensazione, "##,###,##0.00")
            Param_CompensazioneEstero = Format(Iva_CompensazioneEstero, "##,###,##0.00")
            Param_CreditoPrec = Format(Iva_Precedente, "##,###,##0.00")
            Param_Acconto = Format(Acconto, "##,###,##0.00")
            Param_Saldo = Format(Differenza, "##,###,##0.00")
            Param_InteressiIvaDebito = Format(InteresseDebitoIva_Valore, "##,###,##0.00")
            Param_LblInteressi = "Interessi " & Format(InteresseDebitoIva_Perc, "##,###,##0.00") & "% su Iva a Debito"
            Param_SaldoFinale = Format(SaldoFinale, "##,###,##0.00")

            'CType(rptLiqIVA.Section4.ReportObjects("TxtIvaCredito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_Detraibile, "##,###,##0.00")
            'CType(rptLiqIVA.Section4.ReportObjects("TxtIvaDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_NonCompensazione, "##,###,##0.00")
            'CType(rptLiqIVA.Section4.ReportObjects("TxtCompensazioneEstero"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_CompensazioneEstero, "##,###,##0.00")

            'CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoPrec"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Iva_Precedente, "##,###,##0.00")
            'CType(rptLiqIVA.Section4.ReportObjects("TxtAcconto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Acconto, "##,###,##0.00")
            'CType(rptLiqIVA.Section4.ReportObjects("TxtSaldo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Differenza, "##,###,##0.00")

            'CType(rptLiqIVA.Section4.ReportObjects("TxtInteressiIvaDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(InteresseDebitoIva_Valore, "##,###,##0.00")
            'CType(rptLiqIVA.Section4.ReportObjects("LblInteressi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Interessi " & Format(InteresseDebitoIva_Perc, "##,###,##0.00") & "% su Iva a Debito"
            'CType(rptLiqIVA.Section4.ReportObjects("TxtSaldoFinale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(SaldoFinale, "##,###,##0.00")

            If CDec(Differenza) < 0 Then
                SegnoSaldo = "= (debito)"
            Else
                SegnoSaldo = "= (credito)"
            End If

            'CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo
            'CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebito2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo

            Param_CreditoDebito = SegnoSaldo
            Param_CreditoDebito2 = SegnoSaldo

            If CDec(Iva_Precedente) < 0 Then
                SegnoSaldo = "- (debito)"
            Else
                SegnoSaldo = "+ (credito)"
            End If

            'CType(rptLiqIVA.Section4.ReportObjects("TxtCreditoDebitoPREC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = SegnoSaldo

            Param_CreditoDebitoPREC = SegnoSaldo

        Catch ex As Exception
            Log_Errori += "Riepilogo: " & ex.Message & vbCrLf
        End Try

        'Try

        '    Dim DrR As DS_LiquidazioneIVA.DS_LiquidazioneIVARow
        '    DrR = DSLiqIVA.DS_LiquidazioneIVA.NewRow

        '    DrR.Num_Pagina = Num_Pagina

        '    DSLiqIVA.DS_LiquidazioneIVA.Rows.Add(DrR)

        'Catch ex As Exception
        '    Log_Errori += "Numero Pagina: " & ex.Message & vbCrLf
        'End Try


        'Try
        '    'imposto il dataset sul report principale
        '    rptLiqIVA.SetDataSource(DSLiqIVA)
        'Catch ex As Exception
        '    Log_Errori += "- SetDataSource: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        Try
            'imposto il dataset sul sottoreport credito
            rptIVACredito.SetDataSource(DSIvaCredito)
        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport credito: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'imposto il sottoreport credito sul report
            rptLiqIVA.OpenSubreport("Rpt_SottoReport_IVA.rpt").SetDataSource(DSIvaCredito)
        Catch ex As Exception
            Log_Errori += "- OpenSubreport credito: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'imposto il dataset sul sottoreport debito
            rptIVADebito.SetDataSource(DSIvaDebito)
        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport debito: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'imposto il sottoreport debito sul report
            rptLiqIVA.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSIvaDebito)
        Catch ex As Exception
            Log_Errori &= "- OpenSubreport debito: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################

        Try

            rptLiqIVA.SetParameterValue("Intervallo_Date", Param_Intervallo_Date)
            rptLiqIVA.SetParameterValue("Sezionale", Sezionale_Des)
            rptLiqIVA.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptLiqIVA.SetParameterValue("Piva_CodFiscale", Param_Piva_CodFiscale)
            rptLiqIVA.SetParameterValue("Indirizzo", Param_Indirizzo)
            rptLiqIVA.SetParameterValue("Num_Pagina", Param_NumPagina)
            rptLiqIVA.SetParameterValue("Flag_Stampa_Data", Param_Flag_Stampa_Data)

            rptLiqIVA.SetParameterValue("IvaCredito", Param_IvaCredito)
            rptLiqIVA.SetParameterValue("IvaDebito", Param_IvaDebito)
            rptLiqIVA.SetParameterValue("CompensazioneEstero", Param_CompensazioneEstero)
            rptLiqIVA.SetParameterValue("CreditoPrec", Param_CreditoPrec)
            rptLiqIVA.SetParameterValue("Acconto", Param_Acconto)
            rptLiqIVA.SetParameterValue("Saldo", Param_Saldo)
            rptLiqIVA.SetParameterValue("InteressiIvaDebito", Param_InteressiIvaDebito)
            rptLiqIVA.SetParameterValue("LblInteressi", Param_LblInteressi)
            rptLiqIVA.SetParameterValue("SaldoFinale", Param_SaldoFinale)
            rptLiqIVA.SetParameterValue("CreditoDebito", Param_CreditoDebito)
            rptLiqIVA.SetParameterValue("CreditoDebito2", Param_CreditoDebito2)
            rptLiqIVA.SetParameterValue("CreditoDebitoPREC", Param_CreditoDebitoPREC)

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class
