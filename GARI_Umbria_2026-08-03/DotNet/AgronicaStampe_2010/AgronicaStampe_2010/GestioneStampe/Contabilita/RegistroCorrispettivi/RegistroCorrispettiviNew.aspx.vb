Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class RegistroCorrispettiviNew
    Inherits System.Web.UI.Page

    Private rptRegCorrispettivi As Rpt_RegistroCorrispettiviNew
    Private Log_Errori As String

    Dim Piva As String
    Dim Data_Inizio As String
    Dim Data_Fine As String
    Dim Log As String = ""
    Dim Sezionale_Cod As Integer
    Dim Sezionale_Des As String
    Dim EsigibilitaIva As Integer

    Dim Intervallo_Date As String = ""
    Dim Param_Filtri As String = ""
    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CodFiscale As String = ""
    Dim Param_Indirizzo As String = ""
    Dim Param_NumPagina As Integer = 0
    Dim Param_Flag_Stampa_Data As Boolean


    ' Dim cod_iva_1, cod_iva_2, cod_iva_3, cod_iva_4 As Integer
    ' Dim Cod_iva_non_imp_1, Cod_iva_non_imp_2, Cod_iva_escl_iva_1, Cod_iva_escl_iva_2 As Integer
    Dim FlagStampaNote, FlagStampaRegistroIva As Boolean

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " Registro Corrispettivi NEW"

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub RegistroCorrispettiviNew_AbortTransaction(sender As Object, e As System.EventArgs) Handles Me.AbortTransaction

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        rptRegCorrispettivi = New Rpt_RegistroCorrispettiviNew

    End Sub

#End Region

    '#####################################################################################
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

        Sezionale_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), _
                          AgroKey_EncoderDecoder, _
                          Server))

        Sezionale_Des = CStr(Stringa_Decodifica(CStr(Request.QueryString("szd")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        Param_NumPagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")), _
                      AgroKey_EncoderDecoder, _
                      Server))

        Param_Flag_Stampa_Data = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsd")), _
                      AgroKey_EncoderDecoder, _
                      Server))

        Param_Rag_Soc = Stringa_Decodifica(CStr(Request.QueryString("pmr")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        Param_Piva_CodFiscale = Stringa_Decodifica(CStr(Request.QueryString("pmp")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        Param_Indirizzo = Stringa_Decodifica(CStr(Request.QueryString("pmi")), _
                               AgroKey_EncoderDecoder, _
                               Server)
        Intervallo_Date = Stringa_Decodifica(CStr(Request.QueryString("pmd")), _
                             AgroKey_EncoderDecoder, _
                             Server)

        'cod_iva_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci1")), _
        '                    AgroKey_EncoderDecoder, _
        '                    Server))

        'cod_iva_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci2")), _
        '                  AgroKey_EncoderDecoder, _
        '                  Server))

        'cod_iva_3 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci3")), _
        '                  AgroKey_EncoderDecoder, _
        '                  Server))

        'cod_iva_4 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci4")), _
        '                  AgroKey_EncoderDecoder, _
        '                  Server))

        'Cod_iva_non_imp_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ni1")), _
        '              AgroKey_EncoderDecoder, _
        '              Server))

        'Cod_iva_non_imp_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ni2")), _
        '              AgroKey_EncoderDecoder, _
        '              Server))

        'Cod_iva_escl_iva_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ei1")), _
        '              AgroKey_EncoderDecoder, _
        '              Server))

        'Cod_iva_escl_iva_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ei2")), _
        '              AgroKey_EncoderDecoder, _
        '              Server))

        FlagStampaNote = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsn")), _
                      AgroKey_EncoderDecoder, _
                      Server))

        FlagStampaRegistroIva = CBool(Stringa_Decodifica(CStr(Request.QueryString("ri")), _
                  AgroKey_EncoderDecoder, _
                  Server))

        'Cod_RisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), _
        '                          AgroKey_EncoderDecoder, _
        '                          Server)

        'Cod_Rapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), _
        '                        AgroKey_EncoderDecoder, _
        '                        Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "Registro_Corrispettivi"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                ' Stampa_RegistroCorrispettivi()

                Dim objSez As New AgronicaCoreContabDAL.Imprese_Sezionali_R
                EsigibilitaIva = objSez.EsigibilitaIva_from_SezionaleCod(Piva, Sezionale_Cod, objParametri_Server)

                Stampa_RegistroCorrispettivi_ReleaseArrotondamenti2019()

                ' Stampa_RegistroCorrispettivi_ReleaseArrotondamenti2018()

            Catch exc As Exception
                Log_Errori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
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
                Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptRegCorrispettivi,
                                           enum_CategorieDocumenti.RegistroCorrispettivi_Vendita,
                                           sottoCartella, nomeFilePdf,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(Piva,
                                                                                enum_CategorieDocumenti.RegistroCorrispettivi_Vendita,
                                                                                Nome_Documento,
                                                                                nomeFilePdf,
                                                                                sottoCartella,
                                                                                "", "", "", "",
                                                                                Data_Inizio_Allegati,
                                                                                Data_Fine_Allegati,
                                                                                objParametri_Server)
                
            Catch ex As Exception
                Log_Errori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptRegCorrispettivi.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            'DSRegCorr.Dispose()
            'DSRegCorr = Nothing

            'rptRegCorrispettivi.Close()
            'rptRegCorrispettivi.Dispose()
            'rptRegCorrispettivi = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Piva) &
                             ", Data Inizio = " & CStr(Data_Inizio) & ", Data Fine = " & CStr(Data_Fine) &
                             vbCrLf & vbCrLf & Log_Errori

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Piva) &
                             vbCrLf & vbCrLf & Log_Errori
                
                Dim Nome_File As String = "Log_Errori_" & Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Contabilita",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "Registro_CorrispettiviNew.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder)

            If PDF = "1" Then
                'Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("True", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            End If

        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_RegistroCorrispettivi_ReleaseArrotondamenti2018()

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab
        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        Dim objSottoIva As New cls_SottoReport_IVA
        Dim debug As Boolean
        Dim i, Lav_Cod As Integer

        Dim DT_Note As DataTable
        Dim DT_Gruppo As DataTable
        Dim DT_TotaleGiornata As DataTable
        Dim DT_RiepilogoIva As DataTable
        Dim DT_IVA_Generale As DataTable

        Dim DS_TabAliquota As New DS_RegCorrispettivi_TabellaAliquota
        Dim DR_TabAliquota As DS_RegCorrispettivi_TabellaAliquota.TabellaAliquotaRow

        Dim DS_TotGiornata As New DS_RegCorrispettivi_TotaliGiornata
        Dim DR_TotGiornata As DS_RegCorrispettivi_TotaliGiornata.TotaliGiornataRow

        Try

            '///////////////////////////////////////////////
            '//////// GRUPPO PER OGNI ALIQUOTA/////////////////
            '///////////////////////////////////////////////
            Try

                DT_Gruppo = objStampe.RegistroCorrispettivi_ReleaseArrotondamenti2018(0, _
                                                                            Piva, _
                                                                            Sezionale_Cod, _
                                                                            Data_Inizio, _
                                                                            Data_Fine, _
                                                                            "", _
                                                                            objParametri_Server)
            Catch ex As Exception
                Log_Errori &= "query per popolare il gruppo (ogni aliquota): " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try

            If Not IsNothing(DT_Gruppo) AndAlso DT_Gruppo.Rows.Count > 0 Then

                For i = 0 To DT_Gruppo.Rows.Count - 1

                    DR_TabAliquota = DS_TabAliquota.TabellaAliquota.NewRow

                    DR_TabAliquota.Cod_Iva = DT_Gruppo.Rows(i).Item("Cod_Iva")
                    DR_TabAliquota.Sigla_Iva = DT_Gruppo.Rows(i).Item("Sigla_IVA")

                    DR_TabAliquota.Data = CDate(DT_Gruppo.Rows(i).Item("Data_Movimento")).ToShortDateString
                    DR_TabAliquota.Importo = DT_Gruppo.Rows(i).Item("Importo")
                    DR_TabAliquota.Imposta = DT_Gruppo.Rows(i).Item("Iva")
                    DR_TabAliquota.Imponibile = DT_Gruppo.Rows(i).Item("Imponibile_Netto")
                    DR_TabAliquota.ImportoOMAGGI = DT_Gruppo.Rows(i).Item("Importo_OMAGGI")
                    DR_TabAliquota.ImpostaOMAGGI = DT_Gruppo.Rows(i).Item("Iva_OMAGGI")
                    DR_TabAliquota.ImponibileOMAGGI = DT_Gruppo.Rows(i).Item("Imponibile_Netto_OMAGGI")
                    DR_TabAliquota.ImportoAUTOCONSUMO = DT_Gruppo.Rows(i).Item("Importo_AUTOCONSUMO")
                    DR_TabAliquota.ImpostaAUTOCONSUMO = DT_Gruppo.Rows(i).Item("Iva_AUTOCONSUMO")
                    DR_TabAliquota.ImponibileAUTOCONSUMO = DT_Gruppo.Rows(i).Item("Imponibile_Netto_AUTOCONSUMO")

                    DR_TabAliquota.TotaleImporti = DR_TabAliquota.Importo + DR_TabAliquota.ImportoOMAGGI + DR_TabAliquota.ImportoAUTOCONSUMO

                    DS_TabAliquota.TabellaAliquota.Rows.Add(DR_TabAliquota)

                Next 'dt_query

            Else
                'non ci sono dati
                debug = True
            End If
            '//////////////////////////////////////////////////////////////////////////////

            '///////////////////////////////////////////////
            '//////// TOTALI X GIORNATA   /////////////////
            '///////////////////////////////////////////////       
            Try

                DT_TotaleGiornata = objStampe.RegistroCorrispettivi_ReleaseArrotondamenti2018(2, _
                                                                        Piva, _
                                                                        Sezionale_Cod, _
                                                                        Data_Inizio, _
                                                                        Data_Fine, _
                                                                        "", _
                                                                        objParametri_Server)
            Catch ex As Exception
                Log_Errori &= "query per totale giornata: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try


            If FlagStampaNote = True Then

                Try

                    DT_Note = objStampe.RegistroCorrispettivi_RicavaNotaGiorno(Piva, _
                                                                                Sezionale_Cod, _
                                                                                Data_Inizio, _
                                                                                Data_Fine, _
                                                                                "", _
                                                                                objParametri_Server)

                Catch ex As Exception
                    Log_Errori &= "- Lettura delle note: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
                End Try

            End If

            If Not IsNothing(DT_TotaleGiornata) AndAlso DT_TotaleGiornata.Rows.Count > 0 Then

                For i = 0 To DT_TotaleGiornata.Rows.Count - 1

                    DR_TotGiornata = DS_TotGiornata.TotaliGiornata.NewRow

                    DR_TotGiornata.Data = CDate(DT_TotaleGiornata.Rows(i).Item("Data_Movimento")).ToShortDateString
                    DR_TotGiornata.Importo = DT_TotaleGiornata.Rows(i).Item("Importo")
                    DR_TotGiornata.ImportoAUTOCONSUMO = DT_TotaleGiornata.Rows(i).Item("Importo_AUTOCONSUMO")
                    DR_TotGiornata.ImportoOMAGGI = DT_TotaleGiornata.Rows(i).Item("Importo_OMAGGI")
                    DR_TotGiornata.TotaleImporti = DR_TotGiornata.Importo + DR_TotGiornata.ImportoOMAGGI + DR_TotGiornata.ImportoAUTOCONSUMO

                    DR_TotGiornata.Note = Recupera_Note(DT_Note, DR_TotGiornata.Data)

                    DS_TotGiornata.TotaliGiornata.Rows.Add(DR_TotGiornata)

                Next 'dt_query

            Else
                'non ci sono dati
                debug = True
            End If
            '////////////////////////////////////////////////////////////////////

            '///////////////////////////////////////////////
            '//////// RIEPILOGO IVA   /////////////////
            '///////////////////////////////////////////////      
            Try

                DT_RiepilogoIva = objStampe.RegistroCorrispettivi_ReleaseArrotondamenti2018(1, _
                                                                    Piva, _
                                                                    Sezionale_Cod, _
                                                                    Data_Inizio, _
                                                                    Data_Fine, _
                                                                    "", _
                                                                    objParametri_Server)
            Catch ex As Exception
                Log_Errori &= "query per popolare il DT_RiepilogoIva (c'è sia aliquota che % compensazione): " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try

            Dim Sum_Imponibile As Decimal
            Dim Sum_Iva As Decimal
            Dim Sum_Totale As Decimal
            Dim Aliquota_Des As String
            If Not IsNothing(DT_RiepilogoIva) AndAlso DT_RiepilogoIva.Rows.Count > 0 Then

                DT_IVA_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()

                For i = 0 To DT_RiepilogoIva.Rows.Count - 1

                    Sum_Imponibile = DT_RiepilogoIva.Rows(i).Item("Imponibile_Netto_AUTOCONSUMO") + DT_RiepilogoIva.Rows(i).Item("Imponibile_Netto_OMAGGI") + DT_RiepilogoIva.Rows(i).Item("Imponibile_Netto")
                    Sum_Iva = DT_RiepilogoIva.Rows(i).Item("Iva_AUTOCONSUMO") + DT_RiepilogoIva.Rows(i).Item("Iva_OMAGGI") + DT_RiepilogoIva.Rows(i).Item("Iva")
                    Sum_Totale = DT_RiepilogoIva.Rows(i).Item("Importo_AUTOCONSUMO") + DT_RiepilogoIva.Rows(i).Item("Importo_OMAGGI") + DT_RiepilogoIva.Rows(i).Item("Importo")

                    If DT_RiepilogoIva.Rows(i).Item("iva_indetraibile_perc") <> 0 Then
                        Aliquota_Des = CStr(DT_RiepilogoIva.Rows(i).Item("Sigla_IVA")) & " Compensazione " & CStr(DT_RiepilogoIva.Rows(i).Item("iva_indetraibile_perc")) & "%"
                    Else
                        Aliquota_Des = DT_RiepilogoIva.Rows(i).Item("Sigla_IVA")
                    End If

                    objSottoIva.InserisciRiga_DtIvaGenerale(DT_IVA_Generale, _
                                                                 Sum_Imponibile, _
                                                                 DT_RiepilogoIva.Rows(i).Item("cod_iva"), _
                                                                 Sum_Iva, _
                                                                 DT_RiepilogoIva.Rows(i).Item("aliquota"), _
                                                                 Aliquota_Des, _
                                                                 Sum_Totale, _
                                                                 DT_RiepilogoIva.Rows(i).Item("Iva_Indetraibile_Perc"), _
                                                                 DT_RiepilogoIva.Rows(i).Item("Iva_Indetraibile"), _
                                                                 0)
                Next

            End If


        Catch ex As Exception
            Log_Errori &= "- Elaborazione dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Dim DSSottoReportIVA_Vend As New DS_SottoReportIVA_Duplicato
        Try
            'VENDITE
            If Not IsNothing(DT_IVA_Generale) Then
                objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(DSSottoReportIVA_Vend, DT_IVA_Generale, "RIEPILOGO IVA Registro Corrispettivi", objParametri_Server)
            End If
        Catch ex As Exception
            Log_Errori &= "- Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            rptRegCorrispettivi.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSSottoReportIVA_Vend)

        Catch ex As Exception
            Log_Errori &= "- OpenSubreport Rpt_SottoReportIVA_Duplicato: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            rptRegCorrispettivi.OpenSubreport("SottoRpt_RegCorrispettivi_TotaliGiornata.rpt").SetDataSource(DS_TotGiornata)

        Catch ex As Exception
            Log_Errori &= "- OpenSubreport SottoRpt_RegCorrispettivi_TotaliGiornata.rpt: " & vbCrLf & ex.Message & vbCrLf
        End Try

        If FlagStampaRegistroIva = True Then
            'stampo il riepilogo
        Else
            rptRegCorrispettivi.ReportFooterSection1.SectionFormat.EnableSuppress = True
        End If

        Try

            '--------------------------------------------
            ' AGGANCIO DATI
            '--------------------------------------------
            rptRegCorrispettivi.SetDataSource(DS_TabAliquota)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################

        Try

            Param_Filtri = Intervallo_Date & "  -  Sezionale: " & Sezionale_Des
            rptRegCorrispettivi.SetParameterValue("Filtri", Param_Filtri)
            rptRegCorrispettivi.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptRegCorrispettivi.SetParameterValue("Piva_CodFiscale", Param_Piva_CodFiscale)
            rptRegCorrispettivi.SetParameterValue("Indirizzo", Param_Indirizzo)
            rptRegCorrispettivi.SetParameterValue("Num_Pagina", Param_NumPagina)
            rptRegCorrispettivi.SetParameterValue("Flag_Stampa_Data", Param_Flag_Stampa_Data)

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub


    'Try
    '    DT_IVA_Round = objLanRound.CaricaGriglia_DtIva

    'Catch ex As Exception
    '    Throw New Exception("DT_IVA_Round, errore nella creazione: " + vbCrLf + ex.Message)
    'End Try

    '          DT_Globale = CaricaGriglia_DtGlobale()

    'Try
    '    DT_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

    'Catch ex As Exception
    '    Throw New Exception("DT_Dettagli_Round, errore nella creazione: " + vbCrLf + ex.Message)
    'End Try


    ''per l'ultima operazione
    'ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
    '                                DT_Dettagli_Round, _
    '                                DT_IVA_Round, _
    '                                Num_Protocollo, _
    '                                Lav_Cod, _
    '                                edit_importo)

    '   objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Generale, False)

    '' per ogni riga dt iva round -> popolazione dt_globale (da usare poi per il popolamento dataset)
    'Popola_DtGlobale_ByDtIva(DT_Globale, _
    '                        DT_Query, _
    '                        DT_IVA_Round, _
    '                        i - 1)

    ''/////////////////////////////////////

    ''per ogni riga del dt globale 
    'Elabora_Dt_Globale(DSRegCorr, DT_Globale, DT_Note)

    '#################################################################################
    Private Sub Recupera_Importi_NelCasoDiOmaggi(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                                       ByVal DT_Query As DataTable, _
                                                        ByVal id_agenda As Integer, _
                                                        ByVal lav_cod As Integer, _
                                                        ByVal cod_iva As Integer, _
                                                        ByVal data As Date, _
                                                         ByVal TOT_Imponibile As Decimal, _
                                                       ByVal TOT_Iva As Decimal,
                                                       ByVal TOT_Importo As Decimal,
                                                        ByRef Imponibile_normale As Decimal, _
                                                       ByRef Iva_normale As Decimal,
                                                       ByRef Importo_normale As Decimal,
                                                       ByRef Imponibile_OMAGGI As Decimal,
                                                       ByRef Iva_OMAGGI As Decimal,
                                                        ByRef Importo_OMAGGI As Decimal)

        Dim dr() As DataRow
        Dim sconto_modalita As Integer
        Dim x_iva As Decimal = 0
        Dim x_imponibile As Decimal = 0
        Dim x_importo As Decimal = 0

        dr = DT_Query.Select("id_agenda = " & CStr(id_agenda) & " AND cod_iva = " & CStr(cod_iva) & " AND data_movimento = '" & CStr(data) & "'")

        If Not IsNothing(dr) AndAlso dr.Length > 0 Then

            For i = 0 To dr.Length - 1

                sconto_modalita = dr(i).Item("sconto_modalita")

                Select Case sconto_modalita

                    Case enModalitaSconto.Omaggio_ConRivalsaIva, enModalitaSconto.Omaggio_SenzaRivalsaIva

                    Case Else
                        'qui deve sommare tutte le modalità che non sono omaggio
                        x_iva += objContabHLP.Leggi_IVA_PositivaNegativa(dr(i).Item("Lav_Cod"), dr(i).Item("Iva"))
                        x_imponibile += objContabHLP.Leggi_Imponibile_PositivoNegativo(lav_cod, dr(i).Item("Imponibile_netto"))

                End Select

            Next

            'arrotondo a 2
            Imponibile_normale = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_2(x_imponibile)
            Iva_normale = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_2(x_iva)
            Importo_normale = Iva_normale + Imponibile_normale

            'calcolo gli importi omaggio per differenza
            Imponibile_OMAGGI = TOT_Imponibile - Imponibile_normale
            Iva_OMAGGI = TOT_Iva - Iva_normale
            Importo_OMAGGI = TOT_Importo - Importo_normale


        End If


    End Sub


    '#################################################################################
    Private Function Recupera_Note(ByVal DT_Note As DataTable, _
                                    ByVal data As String) As String

        Dim note As String = ""

        Try
            If Not IsNothing(DT_Note) AndAlso DT_Note.Rows.Count > 0 Then
                Dim riga_note() As DataRow
                riga_note = DT_Note.Select("Data = '" & CDate(data) & "'")
                If Not IsNothing(riga_note) AndAlso riga_note.Length > 0 Then
                    note = riga_note(0).Item("Note")
                End If
            End If

        Catch ex As Exception
            Log_Errori &= "- Elaborazione delle note: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        Return note

    End Function

    '#####################################################################################################
    Private Sub Stampa_RegistroCorrispettivi_ReleaseArrotondamenti2019()

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        Dim objSottoIva As New cls_SottoReport_IVA
        Dim id_agenda_memo As Integer = 0
        Dim id_agenda As Integer
        Dim DT_Query As DataTable
        Dim DT_Dettagli_Round As DataTable
        Dim DT_IVA_Round As DataTable
        Dim DT_IVA_Generale, DT_Globale As DataTable
        'Dim Dt_Mov As DataTable
        Dim debug As Boolean
        Dim i, Lav_Cod As Integer
        Dim Num_Protocollo As Decimal = 0
        Dim edit_importo As enum_EditImporto
        Dim Num_Documenti As Integer = 0
        Dim sconto_modalita As Integer

        Dim DT_Iva_Aliquote As DataTable
        Dim HT_Omaggi As New Hashtable

        Dim DS_TabAliquota As New DS_RegCorrispettivi_TabellaAliquota
        ' Dim DR_TabAliquota As DS_RegCorrispettivi_TabellaAliquota.TabellaAliquotaRow

        Dim DS_TotGiornata As New DS_RegCorrispettivi_TotaliGiornata
        ' Dim DR_TotGiornata As DS_RegCorrispettivi_TotaliGiornata.TotaliGiornataRow

        '########################################################################

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab

        Try

            DT_Query = objStampe.RegistroCorrispettivi_ReleaseArrotondamenti2019(Piva, _
                                                                                 Sezionale_Cod, _
                                                                                Data_Inizio, _
                                                                                Data_Fine, _
                                                                                 "", _
                                                                                objParametri_Server)
        Catch ex As Exception
            Log_Errori &= "- Lettura dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            If Not IsNothing(DT_Query) AndAlso DT_Query.Rows.Count > 0 Then

                Num_Documenti = DT_Query.Rows.Count

                DT_IVA_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()

                DT_Globale = CaricaGriglia_DtGlobale()

                Try
                    DT_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

                Catch ex As Exception
                    Throw New Exception("DT_Dettagli_Round, errore nella creazione: " & vbCrLf & ex.Message)
                End Try

                Try
                    DT_IVA_Round = objLanRound.CaricaGriglia_DtIva

                Catch ex As Exception
                    Throw New Exception("DT_IVA_Round, errore nella creazione: " & vbCrLf & ex.Message)
                End Try

                For i = 0 To Num_Documenti - 1

                    id_agenda = DT_Query.Rows(i).Item("id_agenda")

                    Lav_Cod = DT_Query.Rows(i).Item("Lav_Cod")

                    Num_Protocollo = DT_Query.Rows(i).Item("Num_Protocollo")
                    edit_importo = DT_Query.Rows(i).Item("tipo_sconto")

                    sconto_modalita = DT_Query.Rows(i).Item("sconto_modalita")

                    Select Case sconto_modalita
                        Case enModalitaSconto.Omaggio_ConRivalsaIva, enModalitaSconto.Omaggio_SenzaRivalsaIva
                            If Not HT_Omaggi.ContainsKey(id_agenda) Then
                                HT_Omaggi.Add(id_agenda, True)
                            End If
                    End Select

                    If id_agenda <> id_agenda_memo Then
                        'nuova operazione

                        'se non sono al primo giro
                        'devo elaborare il gruppo di dettagli dell'operazione precedente
                        If id_agenda_memo <> 0 Then
                            ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                            DT_Dettagli_Round, _
                                                            DT_IVA_Round, _
                                                            DT_Query.Rows(i - 1).Item("Num_Protocollo"), _
                                                            DT_Query.Rows(i - 1).Item("lav_cod"), _
                                                            DT_Query.Rows(i - 1).Item("tipo_sconto"))

                            objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Generale, False)

                            ' per ogni riga dt iva round -> popolazione dt_globale (da usare poi per il popolamento dataset)
                            Popola_DtGlobale_ByDtIva(DT_Globale, _
                                                        DT_Query, _
                                                        DT_IVA_Round, _
                                                        i - 1)

                        End If

                        'ad ogni operazione creo un nuovo dt_dettagli
                        DT_Dettagli_Round.Clear()
                        'svuoto anche dt_iva che va popolato al termine
                        DT_IVA_Round.Clear()

                        If DT_Dettagli_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_Dettagli_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If
                        If DT_IVA_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_IVA_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If

                        'salvo l'id_agenda
                        id_agenda_memo = id_agenda

                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                            objLanRound, _
                                                            Lav_Cod, _
                                                            DT_Dettagli_Round, _
                                                           DT_Query.Rows(i))

                    Else
                        'stessa operazione
                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                        objLanRound, _
                                                        Lav_Cod, _
                                                        DT_Dettagli_Round, _
                                                        DT_Query.Rows(i))

                    End If

                Next 'dt_query

                'per l'ultima operazione
                ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                DT_Dettagli_Round, _
                                                DT_IVA_Round, _
                                                Num_Protocollo, _
                                                Lav_Cod, _
                                                edit_importo)

                objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Generale, False)

                ' per ogni riga dt iva round -> popolazione dt_globale (da usare poi per il popolamento dataset)
                Popola_DtGlobale_ByDtIva(DT_Globale, _
                                        DT_Query, _
                                        DT_IVA_Round, _
                                        i - 1)

                '/////////////////////////////////////

                'per ogni riga del dt globale 
                Elabora_Dt_Globale(objContabHLP, DS_TabAliquota, DT_Globale, DT_Query, HT_Omaggi)

            Else
                'non ci sono dati
                debug = True
            End If 'dt_query

        Catch ex As Exception
            Log_Errori &= "- Elaborazione dei dati Tabelle per Aliquote: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        '#########################################################################

        'TOTALI X GIORNATA

        Try
            If Not IsNothing(DT_Globale) Then
                DS_TotaliGiornata_Riempi(DS_TotGiornata, DT_Globale)
            End If
        Catch ex As Exception
            Log_Errori &= "- Elaborazione dei dati Totali per giornata: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        Try
            rptRegCorrispettivi.OpenSubreport("SottoRpt_RegCorrispettivi_TotaliGiornata.rpt").SetDataSource(DS_TotGiornata)
        Catch ex As Exception
            Log_Errori &= "- OpenSubreport SottoRpt_RegCorrispettivi_TotaliGiornata.rpt: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '########################################################################

        '--------------------------------------------
        ' AGGANCIO DATI
        '--------------------------------------------

        Try

            rptRegCorrispettivi.SetDataSource(DS_TabAliquota)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################

        'SOTTOREPORT RIEPILOGO IVA

        Dim DSSottoReportIVA_Vend As New DS_SottoReportIVA_Duplicato

        Try
            'VENDITE
            If Not IsNothing(DT_IVA_Generale) Then
                objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(DSSottoReportIVA_Vend, DT_IVA_Generale, "RIEPILOGO IVA Registro Corrispettivi", objParametri_Server)
            End If
        Catch ex As Exception
            Log_Errori &= "- Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            rptRegCorrispettivi.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSSottoReportIVA_Vend)

        Catch ex As Exception
            Log_Errori &= "- OpenSubreport: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################




        '#########################################################

        'If FlagStampaRegistroIva = True Then
        '    'stampo il riepilogo
        'Else
        '    rptRegCorrispettivi.ReportFooterSection1.SectionFormat.EnableSuppress = True
        'End If

        Try

            Param_Filtri = Intervallo_Date & "  -  Sezionale: " & Sezionale_Des
            rptRegCorrispettivi.SetParameterValue("Filtri", Param_Filtri)
            rptRegCorrispettivi.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptRegCorrispettivi.SetParameterValue("Piva_CodFiscale", Param_Piva_CodFiscale)
            rptRegCorrispettivi.SetParameterValue("Indirizzo", Param_Indirizzo)
            rptRegCorrispettivi.SetParameterValue("Num_Pagina", Param_NumPagina)
            rptRegCorrispettivi.SetParameterValue("Flag_Stampa_Data", Param_Flag_Stampa_Data)

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '########################################################################
    Private Sub DtDettRound_InserisciDettaglio(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                                    ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                    ByVal Lav_Cod As Integer, _
                                                    ByRef DT_Dettagli_Round As DataTable, _
                                                    ByRef DrQuery As DataRow)

        Dim x_IVA, x_Iva_Indetraibile, x_Imponibile, x_Imponibile_Netto As Decimal

        With DrQuery

            'x_IVA = CDec(.Item("iva"))
            'x_Imponibile = CDec(.Item("imponibile"))
            'x_Imponibile_Netto = CDec(.Item("imponibile_netto"))

            x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa(.Item("Lav_Cod"), .Item("Iva"))
            x_Iva_Indetraibile = objContabHLP.Leggi_IVAIndet_PositivaNegativa(.Item("Lav_Cod"), .Item("Iva_Indetraibile"))
            x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, .Item("Imponibile"))
            x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, .Item("Imponibile_netto"))

            objLanRound.InserisciRiga_DtDettagli(DT_Dettagli_Round, _
                                                .Item("Id_Mov_Det"), _
                                                .Item("ChkLayOut_Hide"), _
                                                 .Item("Sconto_Modalita"), _
                                                 .Item("Sconto"), _
                                                 .Item("Sconto_listino"), _
                                                .Item("qta"), _
                                                .Item("Prezzo_Unitario"), _
                                                .Item("Prezzo_Unitario_Netto"), _
                                                x_Imponibile, _
                                                x_Imponibile_Netto, _
                                                .Item("Cod_IVA"), _
                                                x_IVA, _
                                                .Item("Aliquota"), _
                                                .Item("Sigla_IVA"), _
                                                0, _
                                                x_Iva_Indetraibile, _
                                                .Item("Iva_Indetraibile_Perc"), _
                                                0)

        End With

    End Sub

    '########################################################################
    Private Sub ElaboraDettagli_x_Riepilogo_IVA(ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                ByVal DT_Dettagli_Round As DataTable, _
                                                ByRef DT_IVA_Round As DataTable, _
                                                ByRef Num_Protocollo As Decimal, _
                                                ByVal Lav_Cod As Integer, _
                                                ByVal Edit_Importo As enum_EditImporto)

        '  ByRef Riepilogo_Importo As Decimal)

        'questi servono per il riepilogo a fine fattura, non servono quindi in questo report
        Dim Riepilogo_ImponibileLordo As Decimal = 0
        Dim Riepilogo_Variazioni As Decimal = 0
        Dim Riepilogo_ImponibileNetto As Decimal = 0
        Dim Riepilogo_Imposta As Decimal = 0
        Dim Riepilogo_Importo As Decimal = 0

        'modifica del 05/08/2015: metto true per gestire l'iva in compensazione
        'Dim FlagLiqIva As Boolean = False
        Dim FlagLiqIva As Boolean = True

        DT_IVA_Round = objLanRound.FormAggiornaImportoNEW(objParametri_Server, _
                                                        DT_Dettagli_Round, _
                                                        Riepilogo_ImponibileLordo, _
                                                        Riepilogo_Variazioni, _
                                                        Riepilogo_ImponibileNetto, _
                                                        Riepilogo_Imposta, _
                                                        Riepilogo_Importo, _
                                                        Edit_Importo, _
                                                        FlagLiqIva, _
                                                        True, _
                                                        EsigibilitaIva)

        'enum_TipoSconto.PrezzoUnitario

    End Sub


    '########################################################################################
    Public Function CaricaGriglia_DtGlobale() As DataTable

        Dim DtGlobale As New DataTable

        'DtGlobale.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        DtGlobale.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))

        DtGlobale.Columns.Add(New DataColumn("lav_cod", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("data", GetType(Date)))
        DtGlobale.Columns.Add(New DataColumn("sconto_modalita", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("aliquota_des", GetType(String)))

        DtGlobale.Columns.Add(New DataColumn("tot_imponibile_netto", GetType(Decimal)))
        DtGlobale.Columns.Add(New DataColumn("tot_iva", GetType(Decimal)))
        DtGlobale.Columns.Add(New DataColumn("tot_importo", GetType(Decimal)))

        Return DtGlobale

    End Function

    '########################################################################################
    Public Sub InserisciRiga_DtGlobale(ByRef DT_Globale As DataTable, _
                                       ByVal id_agenda As Integer, _
                                       ByVal lav_cod As Integer, _
                                        ByVal data As Date, _
                                        ByVal sconto_modalita As Integer, _
                                        ByVal cod_iva As Integer, _
                                        ByVal aliquota_iva As Decimal, _
                                        ByVal aliquota_des As String, _
                                        ByVal tot_imponibile_netto As Decimal, _
                                          ByVal tot_iva As Decimal)
        'ByVal rag_soc As String, _

        Dim Dr As DataRow

        Dr = DT_Globale.NewRow

        'Dr.Item("rag_soc") = rag_soc
        Dr.Item("id_agenda") = id_agenda
        Dr.Item("lav_cod") = lav_cod
        Dr.Item("data") = data
        Dr.Item("sconto_modalita") = sconto_modalita
        Dr.Item("cod_iva") = cod_iva
        Dr.Item("aliquota_iva") = aliquota_iva
        Dr.Item("aliquota_des") = aliquota_des

        Dr.Item("tot_imponibile_netto") = tot_imponibile_netto
        Dr.Item("tot_iva") = tot_iva
        Dim importo As Decimal = tot_iva + tot_imponibile_netto
        Dr.Item("tot_importo") = importo

        DT_Globale.Rows.Add(Dr)

    End Sub

    '########################################################################
    Private Sub Popola_DtGlobale_ByDtIva(ByRef DT_Globale As DataTable, _
                                            ByVal DT_Query As DataTable, _
                                            ByVal DT_IVA_Round As DataTable, _
                                            ByVal i_operazione As Integer)

        Dim j As Integer

        If Not IsNothing(DT_IVA_Round) AndAlso DT_IVA_Round.Rows.Count > 0 Then

            'per ogni aliquota dell'operazione in corso
            For j = 0 To DT_IVA_Round.Rows.Count - 1

                'non posso passare questo valore perchè sul dt_iva i dettagli sono sommati per aliquota
                'e non c'è quindi la distinzione su sconto_modalita
                'motivo per cui non posso usare interamente il meccanismo
                'DT_Query.Rows(i_operazione).Item("sconto_modalita")
                InserisciRiga_DtGlobale(DT_Globale, _
                                        DT_Query.Rows(i_operazione).Item("id_agenda"), _
                                        DT_Query.Rows(i_operazione).Item("lav_cod"), _
                                        DT_Query.Rows(i_operazione).Item("Data_Movimento"), _
                                        -666, _
                                        DT_IVA_Round.Rows(j).Item("cod_iva"), _
                                        DT_IVA_Round.Rows(j).Item("aliquota_iva"), _
                                        DT_IVA_Round.Rows(j).Item("aliquota_des"), _
                                        DT_IVA_Round.Rows(j).Item("imponibile_netto"), _
                                        DT_IVA_Round.Rows(j).Item("iva"))

            Next 'righe dt_iva_round

        End If 'dt_iva_round

    End Sub

    '########################################################################
    Private Sub Elabora_Dt_Globale(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                   ByRef DS_TabAliquota As DS_RegCorrispettivi_TabellaAliquota, _
                                        ByVal DT_Globale As DataTable, _
                                        ByVal DT_Query As DataTable, _
                                        ByVal HT_Omaggi As Hashtable)


        Dim z As Integer
        Dim note As String
        Dim data, data_memo As String
        Dim cod_iva, cod_iva_memo, sconto_modalita, lav_cod, id_agenda As Integer
        Dim Sigla_Iva, sigla_iva_memo As String

        Dim Iva As Decimal
        Dim Imponibile As Decimal
        Dim Importo As Decimal

        Dim Importo_OMAGGI As Decimal
        Dim Iva_OMAGGI As Decimal
        Dim Imponibile_OMAGGI As Decimal

        Dim Importo_AUTOCONSUMO As Decimal
        Dim Iva_AUTOCONSUMO As Decimal
        Dim Imponibile_AUTOCONSUMO As Decimal

        Dim Iva_normale As Decimal
        Dim Imponibile_normale As Decimal
        Dim Importo_normale As Decimal

        Dim TOT_Importo_OMAGGI As Decimal = 0
        Dim TOT_Iva_OMAGGI As Decimal = 0
        Dim TOT_Imponibile_OMAGGI As Decimal = 0

        Dim TOT_Importo_AUTOCONSUMO As Decimal = 0
        Dim TOT_Iva_AUTOCONSUMO As Decimal = 0
        Dim TOT_Imponibile_AUTOCONSUMO As Decimal = 0

        Dim TOT_Iva_normale As Decimal = 0
        Dim TOT_Imponibile_normale As Decimal = 0
        Dim TOT_Importo_normale As Decimal = 0


        'ordino il datatable per aliquota, per via dello split gruppo sull'aliquota
        Dim Dv As New DataView
        DT_Globale.TableName = "Corrispettivi"
        Dv.Table = DT_Globale
        Dv.Sort = "Cod_Iva, data"
        'Dv.Sort = "Cod_Iva, data, Sconto_Modalita"

        data_memo = ""

        For z = 0 To Dv.Count - 1

            ' rag_soc = Dv.Item(z).Item("rag_soc")
            id_agenda = Dv.Item(z).Item("id_agenda")
            lav_cod = Dv.Item(z).Item("lav_cod")
            data = Dv.Item(z).Item("data")

            cod_iva = Dv.Item(z).Item("cod_iva")
            Sigla_Iva = Dv.Item(z).Item("aliquota_des")
            If z = 0 Then
                cod_iva_memo = cod_iva
                sigla_iva_memo = Sigla_Iva
            End If


            'questo non è da considerare dal dt globale, perchè nel dt iva non c'è, essendoci i totali per aliquota
            'sconto_modalita = Dv.Item(z).Item("sconto_modalita")

            Iva = Dv.Item(z).Item("tot_iva")
            Imponibile = Dv.Item(z).Item("tot_imponibile_netto")
            Importo = Dv.Item(z).Item("tot_importo")

            Select Case lav_cod

                Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO

                    'nell'autoconsumo non c'è la gestione degli omaggi,
                    'si può andare tranquilli a prendere i vari importi

                    Imponibile_AUTOCONSUMO = Imponibile
                    Iva_AUTOCONSUMO = Iva
                    Importo_AUTOCONSUMO = Importo

                    Imponibile_OMAGGI = 0
                    Iva_OMAGGI = 0
                    Importo_OMAGGI = 0

                    Imponibile_normale = 0
                    Iva_normale = 0
                    Importo_normale = 0

                Case Else

                    If HT_Omaggi.ContainsKey(id_agenda) Then
                        'CONTIENE DEGLI OMAGGI -> 
                        'non posso usare i valori di dt_globale perchè sono totali per aliquota

                        Recupera_Importi_NelCasoDiOmaggi(objContabHLP, _
                                                        DT_Query, _
                                                         id_agenda, _
                                                         lav_cod, _
                                                         cod_iva, _
                                                         data, _
                                                          Imponibile, _
                                                        Iva,
                                                        Importo,
                                                         Imponibile_normale, _
                                                        Iva_normale,
                                                        Importo_normale,
                                                        Imponibile_OMAGGI,
                                                        Iva_OMAGGI,
                                                         Importo_OMAGGI)

                    Else
                        'NON HA OMAGGI
                        Imponibile_normale = Imponibile
                        Iva_normale = Iva
                        Importo_normale = Importo

                        Imponibile_AUTOCONSUMO = 0
                        Iva_AUTOCONSUMO = 0
                        Importo_AUTOCONSUMO = 0

                        Imponibile_OMAGGI = 0
                        Iva_OMAGGI = 0
                        Importo_OMAGGI = 0

                    End If

                    'non si può fare così!
                    'questo non è da considerare dal dt globale, perchè nel dt iva non c'è, essendoci i totali per aliquota
                    'Select Case sconto_modalita

                    '    Case enModalitaSconto.Omaggio_ConRivalsaIva, enModalitaSconto.Omaggio_SenzaRivalsaIva

                    '        Imponibile_OMAGGI = Imponibile
                    '        Iva_OMAGGI = Iva
                    '        Importo_OMAGGI = Importo

                    '        Imponibile_AUTOCONSUMO = 0
                    '        Iva_AUTOCONSUMO = 0
                    '        Importo_AUTOCONSUMO = 0

                    '        Imponibile_normale = 0
                    '        Iva_normale = 0
                    '        Importo_normale = 0


                    '    Case Else

                    '        Imponibile_normale = Imponibile
                    '        Iva_normale = Iva
                    '        Importo_normale = Importo

                    '        Imponibile_AUTOCONSUMO = 0
                    '        Iva_AUTOCONSUMO = 0
                    '        Importo_AUTOCONSUMO = 0

                    '        Imponibile_OMAGGI = 0
                    '        Iva_OMAGGI = 0
                    '        Importo_OMAGGI = 0

                    'End Select

            End Select


            If data_memo = "" OrElse data_memo <> data Then
                'primo giorno o è cambiato il giorno

                If data_memo <> "" Then

                    'è cambiato il giorno, inserisco la riga nel dataset per il giorno precedente
                    Dataset_InserisciRighe_TabellaAliquote(DS_TabAliquota, _
                                            cod_iva_memo, _
                                            sigla_iva_memo, _
                                            data_memo, _
                                            Format(TOT_Importo_normale, "##,###,##0.00"), _
                                            Format(TOT_Iva_normale, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_normale, "##,###,##0.00"), _
                                            Format(TOT_Importo_OMAGGI, "##,###,##0.00"), _
                                            Format(TOT_Iva_OMAGGI, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_OMAGGI, "##,###,##0.00"), _
                                           Format(TOT_Importo_AUTOCONSUMO, "##,###,##0.00"), _
                                            Format(TOT_Iva_AUTOCONSUMO, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_AUTOCONSUMO, "##,###,##0.00"))

                End If

                'azzero i totali
                TOT_Importo_OMAGGI = 0
                TOT_Iva_OMAGGI = 0
                TOT_Imponibile_OMAGGI = 0

                TOT_Importo_AUTOCONSUMO = 0
                TOT_Iva_AUTOCONSUMO = 0
                TOT_Imponibile_AUTOCONSUMO = 0

                TOT_Iva_normale = 0
                TOT_Imponibile_normale = 0
                TOT_Importo_normale = 0

                'aggiorno le memo
                data_memo = data
                cod_iva_memo = cod_iva
                sigla_iva_memo = Sigla_Iva

            Else
                'stesso giorno
                If cod_iva <> cod_iva_memo Then
                    'è lo stesso giorno, ma è cambiata l'aliquota
                    ' inserisco la riga nel dataset per stesso giorno ma nuova aliquota
                    Dataset_InserisciRighe_TabellaAliquote(DS_TabAliquota, _
                                            cod_iva_memo, _
                                            sigla_iva_memo, _
                                            data_memo, _
                                            Format(TOT_Importo_normale, "##,###,##0.00"), _
                                            Format(TOT_Iva_normale, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_normale, "##,###,##0.00"), _
                                            Format(TOT_Importo_OMAGGI, "##,###,##0.00"), _
                                            Format(TOT_Iva_OMAGGI, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_OMAGGI, "##,###,##0.00"), _
                                           Format(TOT_Importo_AUTOCONSUMO, "##,###,##0.00"), _
                                            Format(TOT_Iva_AUTOCONSUMO, "##,###,##0.00"), _
                                            Format(TOT_Imponibile_AUTOCONSUMO, "##,###,##0.00"))

                    TOT_Importo_OMAGGI = 0
                    TOT_Iva_OMAGGI = 0
                    TOT_Imponibile_OMAGGI = 0

                    TOT_Importo_AUTOCONSUMO = 0
                    TOT_Iva_AUTOCONSUMO = 0
                    TOT_Imponibile_AUTOCONSUMO = 0

                    TOT_Iva_normale = 0
                    TOT_Imponibile_normale = 0
                    TOT_Importo_normale = 0

                End If

                'aggiorno le memo
                data_memo = data
                cod_iva_memo = cod_iva
                sigla_iva_memo = Sigla_Iva

            End If

            'sommo i totali

            TOT_Importo_OMAGGI += Importo_OMAGGI
            TOT_Iva_OMAGGI += Iva_OMAGGI
            TOT_Imponibile_OMAGGI += Imponibile_OMAGGI

            TOT_Importo_AUTOCONSUMO += Importo_AUTOCONSUMO
            TOT_Iva_AUTOCONSUMO += Iva_AUTOCONSUMO
            TOT_Imponibile_AUTOCONSUMO += Imponibile_AUTOCONSUMO

            TOT_Iva_normale += Iva_normale
            TOT_Imponibile_normale += Imponibile_normale
            TOT_Importo_normale += Importo_normale

        Next 'dt globale

        'Riepilogo_Importo_Data += Totale_Importo_Data


        'ultimo giorno, inserisco la riga nel dataset per il giorno precedente
        Dataset_InserisciRighe_TabellaAliquote(DS_TabAliquota, _
                                   cod_iva, _
                                   Sigla_Iva, _
                                   data, _
                                     Format(TOT_Importo_normale, "##,###,##0.00"), _
                                    Format(TOT_Iva_normale, "##,###,##0.00"), _
                                    Format(TOT_Imponibile_normale, "##,###,##0.00"), _
                                    Format(TOT_Importo_OMAGGI, "##,###,##0.00"), _
                                    Format(TOT_Iva_OMAGGI, "##,###,##0.00"), _
                                    Format(TOT_Imponibile_OMAGGI, "##,###,##0.00"), _
                                    Format(TOT_Importo_AUTOCONSUMO, "##,###,##0.00"), _
                                    Format(TOT_Iva_AUTOCONSUMO, "##,###,##0.00"), _
                                    Format(TOT_Imponibile_AUTOCONSUMO, "##,###,##0.00"))


    End Sub

    '########################################################################
    Private Sub Dataset_InserisciRighe_TabellaAliquote(ByRef DS_TabAliquota As DS_RegCorrispettivi_TabellaAliquota, _
                                                       ByVal Cod_Iva As Integer, _
                                                       ByVal Sigla_Iva As String, _
                                                       ByVal Data As String, _
                                                       ByVal Importo As Decimal, _
                                                       ByVal Iva As Decimal, _
                                                       ByVal Imponibile As Decimal, _
                                                       ByVal Importo_OMAGGI As Decimal, _
                                                       ByVal Iva_OMAGGI As Decimal, _
                                                       ByVal Imponibile_OMAGGI As Decimal, _
                                                       ByVal Importo_AUTOCONSUMO As Decimal, _
                                                       ByVal Iva_AUTOCONSUMO As Decimal, _
                                                       ByVal Imponibile_AUTOCONSUMO As Decimal _
                                                      )

        Dim DR_TabAliquota As DS_RegCorrispettivi_TabellaAliquota.TabellaAliquotaRow

        DR_TabAliquota = DS_TabAliquota.TabellaAliquota.NewRow

        DR_TabAliquota.Cod_Iva = Cod_Iva
        DR_TabAliquota.Sigla_Iva = Sigla_Iva

        DR_TabAliquota.Data = CDate(Data).ToShortDateString
        DR_TabAliquota.Importo = Importo
        DR_TabAliquota.Imposta = Iva
        DR_TabAliquota.Imponibile = Imponibile
        DR_TabAliquota.ImportoOMAGGI = Importo_OMAGGI
        DR_TabAliquota.ImpostaOMAGGI = Iva_OMAGGI
        DR_TabAliquota.ImponibileOMAGGI = Imponibile_OMAGGI
        DR_TabAliquota.ImportoAUTOCONSUMO = Importo_AUTOCONSUMO
        DR_TabAliquota.ImpostaAUTOCONSUMO = Iva_AUTOCONSUMO
        DR_TabAliquota.ImponibileAUTOCONSUMO = Imponibile_AUTOCONSUMO

        'DR_TabAliquota.Cod_Iva = Cod_Iva")
        'DR_TabAliquota.Sigla_Iva = Sigla_IVA")

        'DR_TabAliquota.Data = CDate(Data_Movimento")).ToShortDateString
        'DR_TabAliquota.Importo = Importo")
        'DR_TabAliquota.Imposta = Iva")
        'DR_TabAliquota.Imponibile = Imponibile_Netto")
        'DR_TabAliquota.ImportoOMAGGI = Importo_OMAGGI")
        'DR_TabAliquota.ImpostaOMAGGI = Iva_OMAGGI")
        'DR_TabAliquota.ImponibileOMAGGI = Imponibile_Netto_OMAGGI")
        'DR_TabAliquota.ImportoAUTOCONSUMO = Importo_AUTOCONSUMO")
        'DR_TabAliquota.ImpostaAUTOCONSUMO = Iva_AUTOCONSUMO")
        'DR_TabAliquota.ImponibileAUTOCONSUMO = Imponibile_Netto_AUTOCONSUMO")

        DR_TabAliquota.TotaleImporti = DR_TabAliquota.Importo + DR_TabAliquota.ImportoOMAGGI + DR_TabAliquota.ImportoAUTOCONSUMO

        DS_TabAliquota.TabellaAliquota.Rows.Add(DR_TabAliquota)

    End Sub



    '########################################################################
    Private Sub DS_TotaliGiornata_Riempi(ByRef DS_TotGiornata As DS_RegCorrispettivi_TotaliGiornata, _
                                        ByVal DT_Globale As DataTable)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab
        Dim DT_Note As DataTable
        Dim DR_TotGiornata As DS_RegCorrispettivi_TotaliGiornata.TotaliGiornataRow

        If FlagStampaNote = True Then

            Try

                DT_Note = objStampe.RegistroCorrispettivi_RicavaNotaGiorno(Piva, _
                                                                            Sezionale_Cod, _
                                                                            Data_Inizio, _
                                                                            Data_Fine, _
                                                                            "", _
                                                                            objParametri_Server)

            Catch ex As Exception
                Log_Errori &= "- Lettura delle note: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try

        End If

        '////////////////////////////////////////////////////////////////////

        Dim z As Integer
        Dim note As String
        Dim data, data_memo As String

        Dim Importo As Decimal

        Dim TOT_Importo As Decimal = 0

        'ordino il datatable per data
        Dim Dv As New DataView
        DT_Globale.TableName = "Corrispettivi"
        Dv.Table = DT_Globale
        Dv.Sort = "data"

        data_memo = ""

        For z = 0 To Dv.Count - 1

            ' rag_soc = Dv.Item(z).Item("rag_soc")
            data = Dv.Item(z).Item("data")

            Importo = Dv.Item(z).Item("tot_importo")

            If data_memo = "" OrElse data_memo <> data Then
                'primo giorno o è cambiato il giorno

                If data_memo <> "" Then

                    'è cambiato il giorno, inserisco la riga nel dataset per il giorno precedente

                    DR_TotGiornata = DS_TotGiornata.TotaliGiornata.NewRow

                    DR_TotGiornata.Data = CDate(data_memo).ToShortDateString
                    DR_TotGiornata.TotaleImporti = TOT_Importo
                    DR_TotGiornata.Note = Recupera_Note(DT_Note, DR_TotGiornata.Data)

                    'questi non mi servono
                    DR_TotGiornata.Importo = 0
                    DR_TotGiornata.ImportoAUTOCONSUMO = 0
                    DR_TotGiornata.ImportoOMAGGI = 0

                    DS_TotGiornata.TotaliGiornata.Rows.Add(DR_TotGiornata)

                End If

                'azzero i totali
                TOT_Importo = 0

                'aggiorno le memo
                data_memo = data

            Else
                'stesso giorno
            End If

            'sommo i totali

            TOT_Importo += Importo

        Next 'dt globale

        'ultimo giorno, inserisco la riga nel dataset per il giorno precedente

        DR_TotGiornata = DS_TotGiornata.TotaliGiornata.NewRow

        DR_TotGiornata.Data = CDate(data).ToShortDateString
        DR_TotGiornata.TotaleImporti = TOT_Importo
        DR_TotGiornata.Note = Recupera_Note(DT_Note, DR_TotGiornata.Data)

        'questi non mi servono
        DR_TotGiornata.Importo = 0
        DR_TotGiornata.ImportoAUTOCONSUMO = 0
        DR_TotGiornata.ImportoOMAGGI = 0

        DS_TotGiornata.TotaliGiornata.Rows.Add(DR_TotGiornata)


    End Sub

End Class