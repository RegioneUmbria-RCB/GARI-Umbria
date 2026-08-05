Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Registro_Corrispettivi_2
    Inherits System.Web.UI.Page

    Private rptRegCorrispettivi As Rpt_RegistroCorrispettivi_2
    Private Log_Errori As String

    Dim Piva As String
    Dim Data_Inizio As String
    Dim Data_Fine As String
    Dim Log As String = ""
    Dim Sezionale_Cod As Integer
    Dim Num_Pagina As Integer

    Dim cod_iva_1, cod_iva_2, cod_iva_3, cod_iva_4 As Integer
    Dim Cod_iva_non_imp_1, Cod_iva_non_imp_2, Cod_iva_escl_iva_1, Cod_iva_escl_iva_2 As Integer
    Dim FlagStampaNote, FlagStampaRegistroIva As Boolean

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " Registro Corrispettivi "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        rptRegCorrispettivi = New Rpt_RegistroCorrispettivi_2

    End Sub

#End Region

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                  AgroKey_EncoderDecoder, Server)

        Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")),
                                         AgroKey_EncoderDecoder, Server)

        Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")),
                                       AgroKey_EncoderDecoder, Server)

        Sezionale_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("sez")),
                                                AgroKey_EncoderDecoder, Server))

        Num_Pagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")),
                                             AgroKey_EncoderDecoder, Server))

        cod_iva_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci1")),
                                            AgroKey_EncoderDecoder, Server))

        cod_iva_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci2")),
                                            AgroKey_EncoderDecoder, Server))

        cod_iva_3 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci3")),
                                            AgroKey_EncoderDecoder, Server))

        cod_iva_4 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ci4")),
                                            AgroKey_EncoderDecoder, Server))

        Cod_iva_non_imp_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ni1")),
                                                    AgroKey_EncoderDecoder, Server))

        Cod_iva_non_imp_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ni2")),
                                                    AgroKey_EncoderDecoder, Server))

        Cod_iva_escl_iva_1 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ei1")),
                                                     AgroKey_EncoderDecoder, Server))

        Cod_iva_escl_iva_2 = CInt(Stringa_Decodifica(CStr(Request.QueryString("ei2")),
                                                     AgroKey_EncoderDecoder, Server))

        FlagStampaNote = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsn")),
                                                  AgroKey_EncoderDecoder, Server))

        FlagStampaRegistroIva = CBool(Stringa_Decodifica(CStr(Request.QueryString("ri")),
                                                         AgroKey_EncoderDecoder, Server))

        'Cod_RisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")),
        '                               AgroKey_EncoderDecoder, Server)

        'Cod_Rapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")),
        '                                  AgroKey_EncoderDecoder, Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "Registro_Corrispettivi"
        Dim IdentificazioneDocumento As String

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim DSRegCorr As New DS_RegistroCorrispettivi

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_RegistroCorrispettivi(DSRegCorr)

                Try

                    Dim str_intervallo As String = "Dal " + Data_Inizio + " al " + Data_Fine
                    CType(rptRegCorrispettivi.Section1.ReportObjects("TxtIntervallo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_intervallo

                    '--------------------------------------------
                    ' AGGANCIO DATI
                    '--------------------------------------------
                    'sorgente dati.....
                    rptRegCorrispettivi.SetDataSource(DSRegCorr)

                Catch ex As Exception
                    Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
                End Try


            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = Data_Inizio
                Data_Fine_Allegati = Data_Fine
                IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptRegCorrispettivi,
                                           enum_CategorieDocumenti.RegistroCorrispettivi_Vendita,
                                           Sottocartella,
                                           Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf",
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                                 enum_CategorieDocumenti.RegistroCorrispettivi_Vendita,
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


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptRegCorrispettivi.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            DSRegCorr.Dispose()
            DSRegCorr = Nothing

            rptRegCorrispettivi.Close()
            rptRegCorrispettivi.Dispose()
            rptRegCorrispettivi = Nothing

            GC.Collect()

            Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")),
                                                AgroKey_EncoderDecoder,
                                                Server)

            If PDF = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                    "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
            End If

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Piva) +
                            ", Data Inizio = " + CStr(Data_Inizio) + ", Data Fine = " + CStr(Data_Fine) +
                            vbCrLf + vbCrLf + Log_Errori

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Piva) +
                               vbCrLf + vbCrLf + Log_Errori


                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Contabilita",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "Registro_Corrispettivi_2.aspx",
                                                 Log_Errori)

            End If
        End If
    End Sub

    '#####################################################################################################
    Private Sub Stampa_RegistroCorrispettivi(ByRef DSRegCorr As DS_RegistroCorrispettivi)


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
        Dim Num_Protocollo As Double = 0
        Dim edit_importo As enum_EditImporto

        ' Dim Tot_Imponibile_4 As Double = 0
        ' Dim Tot_Imponibile_10 As Double = 0
        ' Dim Tot_Imponibile_12 As Double = 0
        ' Dim Tot_Imponibile_21 As Double = 0

        ' Dim Tot_Iva_4 As Double = 0
        ' Dim Tot_Iva_10 As Double = 0
        ' Dim Tot_Iva_12 As Double = 0
        ' Dim Tot_Iva_21 As Double = 0

        ' Dim Tot_Importo_4 As Double = 0
        ' Dim Tot_Importo_10 As Double = 0
        ' Dim Tot_Importo_12 As Double = 0
        ' Dim Tot_Importo_21 As Double = 0

        ' Dim Importo4 As Double
        ' Dim Importo10 As Double
        ' Dim Importo12 As Double
        ' Dim Importo21 As Double

        ' Dim Imponibile4 As Double
        ' Dim Imponibile10 As Double
        ' Dim Imponibile12 As Double
        ' Dim Imponibile21 As Double

        ' Dim Imposta4 As Double
        ' Dim Imposta10 As Double
        ' Dim Imposta12 As Double
        'Dim Imposta21 As Double

        'Dim Esente As Double
        'Dim Tot_Esente As Double

        'Dim Totale_Riga_Importi As Double
        'Dim Totale_Importi As Double

        '  Dim IdAgenda_Memo As Integer = 0
        Dim Num_Documenti As Integer = 0

        '  Dim DrR As DS_Registri_Corrispettivi.RegistriCorrispettiviRow

        Dim DT_Note As DataTable
        Dim DT_Iva_Aliquote As DataTable

        '########################################################################

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab

        Try

            Dim objMS As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
            Dim filtro As String

            filtro = " codice IN ( " & _
                                   CStr(cod_iva_1) & ", " & _
                                   CStr(cod_iva_2) & ", " & _
                                   CStr(cod_iva_3) & ", " & _
                                   CStr(cod_iva_4) & ", " & _
                                   CStr(Cod_iva_non_imp_1) & ", " & _
                                   CStr(Cod_iva_non_imp_2) & ", " & _
                                   CStr(Cod_iva_escl_iva_1) & ", " & _
                                   CStr(Cod_iva_escl_iva_2) & " " & _
                                    ") "

            DT_Iva_Aliquote = objMS.Leggi(0, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                                          filtro, "", objParametri_Server)

            Valorizza_Label_Colonne(DT_Iva_Aliquote)

        Catch ex As Exception
            Log_Errori += "- IVA aliquote: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try

            DT_Query = objStampe.RegistroCorrispettivi_Importi_2(0, _
                                                                Piva, _
                                                                Sezionale_Cod, _
                                                                Data_Inizio, _
                                                                Data_Fine, _
                                                                "", _
                                                                objParametri_Server)
        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
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
                Log_Errori += "- Lettura delle note: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
            End Try

        End If

        Try

            If Not IsNothing(DT_Query) AndAlso DT_Query.Rows.Count > 0 Then

                Num_Documenti = DT_Query.Rows.Count

                DT_IVA_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()

                DT_Globale = CaricaGriglia_DtGlobale()

                Try
                    DT_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

                Catch ex As Exception
                    Throw New Exception("DT_Dettagli_Round, errore nella creazione: " + vbCrLf + ex.Message)
                End Try

                Try
                    DT_IVA_Round = objLanRound.CaricaGriglia_DtIva

                Catch ex As Exception
                    Throw New Exception("DT_IVA_Round, errore nella creazione: " + vbCrLf + ex.Message)
                End Try

                For i = 0 To Num_Documenti - 1

                    id_agenda = DT_Query.Rows(i).Item("id_agenda")

                    Lav_Cod = DT_Query.Rows(i).Item("Lav_Cod")

                    Num_Protocollo = DT_Query.Rows(i).Item("Num_Protocollo")

                    edit_importo = DT_Query.Rows(i).Item("tipo_sconto")

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
                Elabora_Dt_Globale(DSRegCorr, DT_Globale, DT_Note)

            Else
                'non ci sono dati
                debug = True
            End If 'dt_query

            Dim DSSottoReportIVA_Vend As New DS_SottoReportIVA_Duplicato

            Try
                'VENDITE
                If Not IsNothing(DT_IVA_Generale) Then
                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE(DSSottoReportIVA_Vend, DT_IVA_Generale, "RIEPILOGO IVA Registro Corrispettivi")
                End If
            Catch ex As Exception
                Log_Errori += "- Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                rptRegCorrispettivi.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSSottoReportIVA_Vend)

            Catch ex As Exception
                Log_Errori += "- OpenSubreport: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If FlagStampaRegistroIva = True Then
                'stampo il riepilogo
            Else
                rptRegCorrispettivi.ReportFooterSection1.SectionFormat.EnableSuppress = True
            End If


        Catch ex As Exception
            Log_Errori += "- Elaborazione dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        '########################################################################
        '########################################################################

    End Sub

    '########################################################################
    Private Sub DtDettRound_InserisciDettaglio(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                                    ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                    ByVal Lav_Cod As Integer, _
                                                    ByRef DT_Dettagli_Round As DataTable, _
                                                    ByRef DrQuery As DataRow)

        Dim x_IVA, x_Imponibile, x_Imponibile_Netto As Double

        With DrQuery

            x_IVA = CDbl(.Item("iva"))
            x_Imponibile = CDbl(.Item("imponibile"))
            x_Imponibile_Netto = CDbl(.Item("imponibile_netto"))

            objLanRound.InserisciRiga_DtDettagli(DT_Dettagli_Round, _
                                                .Item("Id_Mov_Det"), _
                                                .Item("ChkLayOut_Hide"), _
                                                 .Item("Sconto_Modalita"), _
                                                 .Item("Sconto"), _
                                                 .Item("Sconto_listino"), _
                                                .Item("qta"), _
                                                .Item("Prezzo_Unitario"), _
                                                .Item("Prezzo_Unitario_Netto"), _
                                                .Item("Imponibile"), _
                                                .Item("Imponibile_Netto"), _
                                                .Item("Cod_IVA"), _
                                                .Item("IVA"), _
                                                .Item("Aliquota"), _
                                                .Item("Sigla_IVA"), _
                                                0, _
                                                .Item("Iva_Indetraibile"), _
                                                .Item("Iva_Indetraibile_Perc"), _
                                                0)

        End With

    End Sub

    '########################################################################
    Private Sub ElaboraDettagli_x_Riepilogo_IVA(ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                ByVal DT_Dettagli_Round As DataTable, _
                                                ByRef DT_IVA_Round As DataTable, _
                                                ByRef Num_Protocollo As Double, _
                                                ByVal Lav_Cod As Integer, _
                                                ByVal Edit_Importo As enum_EditImporto)

        '  ByRef Riepilogo_Importo As Double)

        'questi servono per il riepilogo a fine fattura, non servono quindi in questo report
        Dim Riepilogo_ImponibileLordo As Double = 0
        Dim Riepilogo_Variazioni As Double = 0
        Dim Riepilogo_ImponibileNetto As Double = 0
        Dim Riepilogo_Imposta As Double = 0
        Dim Riepilogo_Importo As Double = 0

        'modifica del 05/08/2015: metto true per gestire l'iva in compensazione
        'Dim FlagLiqIva As Boolean = False
        Dim FlagLiqIva As Boolean = True

        DT_IVA_Round = objLanRound.FormAggiornaImporto(objParametri_Server, _
                                                        DT_Dettagli_Round, _
                                                        Riepilogo_ImponibileLordo, _
                                                        Riepilogo_Variazioni, _
                                                        Riepilogo_ImponibileNetto, _
                                                        Riepilogo_Imposta, _
                                                        Riepilogo_Importo, _
                                                        Edit_Importo, _
                                                        FlagLiqIva, _
                                                        True)

        'enum_TipoSconto.PrezzoUnitario

     
        ''verifico in math.abs perchè le fatture emesse sono salvate con totale negativo, menter la FormAggiornaImporto lavora in positivo
        ''il controllo è per verificare l'arrotondamento, quindi non importa verificare il segno
        'If Math.Abs(Riepilogo_Importo) <> Math.Abs(Num_Protocollo) Then
        '    Log_Errori += " Riepilogo_Importo=" & CStr(Math.Abs(Riepilogo_Importo)) & " <> Num_Protocollo=" & CStr(Math.Abs(Num_Protocollo)) & vbCrLf & vbCrLf
        'End If

    End Sub

    '########################################################################################
    Public Function CaricaGriglia_DtGlobale() As DataTable

        Dim DtGlobale As New DataTable

        DtGlobale.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        DtGlobale.Columns.Add(New DataColumn("data", GetType(Date)))
        DtGlobale.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("iva", GetType(Double)))
        DtGlobale.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        DtGlobale.Columns.Add(New DataColumn("aliquota_des", GetType(String)))
        DtGlobale.Columns.Add(New DataColumn("imponibile_netto", GetType(Double)))
        DtGlobale.Columns.Add(New DataColumn("importo", GetType(Double)))

        Return DtGlobale

    End Function

    '########################################################################################
    Public Sub InserisciRiga_DtGlobale(ByRef DT_Globale As DataTable, _
                                        ByVal rag_soc As String, _
                                        ByVal data As Date, _
                                        ByVal cod_iva As Integer, _
                                        ByVal iva As Double, _
                                        ByVal aliquota_iva As Double, _
                                        ByVal aliquota_des As String, _
                                        ByVal imponibile_netto As Double)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = DT_Globale.NewRow

        Dr.Item("rag_soc") = rag_soc
        Dr.Item("data") = data
        Dr.Item("cod_iva") = cod_iva
        Dr.Item("iva") = iva
        Dr.Item("aliquota_iva") = aliquota_iva
        Dr.Item("aliquota_des") = aliquota_des
        Dr.Item("imponibile_netto") = imponibile_netto

        Dim importo As Double = iva + imponibile_netto
        Dr.Item("importo") = importo

        'Associo alla tabella la nuova riga creata
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

                InserisciRiga_DtGlobale(DT_Globale, _
                                        DT_Query.Rows(i_operazione).Item("Impresa"), _
                                         DT_Query.Rows(i_operazione).Item("Data_Movimento"), _
                                        DT_IVA_Round.Rows(j).Item("cod_iva"), _
                                        DT_IVA_Round.Rows(j).Item("iva"), _
                                        DT_IVA_Round.Rows(j).Item("aliquota_iva"), _
                                        DT_IVA_Round.Rows(j).Item("aliquota_des"), _
                                        DT_IVA_Round.Rows(j).Item("imponibile_netto"))

            Next 'righe dt_iva_round

        End If 'dt_iva_round

    End Sub

    '########################################################################
    Private Sub Elabora_Dt_Globale(ByRef DSRegCorr As DS_RegistroCorrispettivi, _
                                        ByVal DT_Globale As DataTable, _
                                        ByVal DT_Note As DataTable)

        Dim z As Integer
        Dim note As String
        Dim data, data_memo, rag_soc As String
        Dim cod_iva As Integer

        Dim col_1, col_2, col_3, col_4 As String

        Dim Totale_Importo_Data As Double

        Dim Tot_Importo_Col1 As Double
        Dim Tot_Importo_Col2 As Double
        Dim Tot_Importo_Col3 As Double
        Dim Tot_Importo_Col4 As Double

        Dim Tot_Imposta_Col1 As Double
        Dim Tot_Imposta_Col2 As Double
        Dim Tot_Imposta_Col3 As Double
        Dim Tot_Imposta_Col4 As Double

        Dim Tot_Imponibile_Col1 As Double
        Dim Tot_Imponibile_Col2 As Double
        Dim Tot_Imponibile_Col3 As Double
        Dim Tot_Imponibile_Col4 As Double

        Dim Tot_Non_Imp_Col1 As Double
        Dim Tot_Non_Imp_Col2 As Double
        Dim Tot_Esenz_Col1 As Double
        Dim Tot_Esenz_Col2 As Double


        Dim Riepilogo_Importo_Data As Double
        Dim Riepilogo_Importo_Col1 As Double
        Dim Riepilogo_Importo_Col2 As Double
        Dim Riepilogo_Importo_Col3 As Double
        Dim Riepilogo_Importo_Col4 As Double
        Dim Riepilogo_Imposta_Col1 As Double
        Dim Riepilogo_Imposta_Col2 As Double
        Dim Riepilogo_Imposta_Col3 As Double
        Dim Riepilogo_Imposta_Col4 As Double
        Dim Riepilogo_Imponibile_Col1 As Double
        Dim Riepilogo_Imponibile_Col2 As Double
        Dim Riepilogo_Imponibile_Col3 As Double
        Dim Riepilogo_Imponibile_Col4 As Double
        Dim Riepilogo_Non_Imp_Col1 As Double
        Dim Riepilogo_Non_Imp_Col2 As Double
        Dim Riepilogo_Esenz_Col1 As Double
        Dim Riepilogo_Esenz_Col2 As Double

        Dim Iva As Double
        Dim Imponibile As Double
        Dim Importo As Double

        'ordino per data il datatable
        Dim Dv As New DataView
        DT_Globale.TableName = "Corrispettivi"
        Dv.Table = DT_Globale
        Dv.Sort = "data"

        data_memo = ""

        For z = 0 To Dv.Count - 1

            rag_soc = Dv.Item(z).Item("rag_soc")
            data = Dv.Item(z).Item("data")

            cod_iva = Dv.Item(z).Item("cod_iva")

            Iva = Dv.Item(z).Item("iva")
            Imponibile = Dv.Item(z).Item("imponibile_netto")
            Importo = Dv.Item(z).Item("importo")

            If data_memo = "" Or data_memo <> data Then
                'primo giorno o è cambiato il giorno

                If data_memo <> "" Then

                    note = Recupera_Note(DT_Note, data_memo)

                    Riepilogo_Importo_Data += Totale_Importo_Data
                    Riepilogo_Importo_Col1 += Tot_Importo_Col1
                    Riepilogo_Importo_Col2 += Tot_Importo_Col2
                    Riepilogo_Importo_Col3 += Tot_Importo_Col3
                    Riepilogo_Importo_Col4 += Tot_Importo_Col4
                    Riepilogo_Imposta_Col1 += Tot_Imposta_Col1
                    Riepilogo_Imposta_Col2 += Tot_Imposta_Col2
                    Riepilogo_Imposta_Col3 += Tot_Imposta_Col3
                    Riepilogo_Imposta_Col4 += Tot_Imposta_Col4
                    Riepilogo_Imponibile_Col1 += Tot_Imponibile_Col1
                    Riepilogo_Imponibile_Col2 += Tot_Imponibile_Col2
                    Riepilogo_Imponibile_Col3 += Tot_Imponibile_Col3
                    Riepilogo_Imponibile_Col4 += Tot_Imponibile_Col4
                    Riepilogo_Non_Imp_Col1 += Tot_Non_Imp_Col1
                    Riepilogo_Non_Imp_Col2 += Tot_Non_Imp_Col2
                    Riepilogo_Esenz_Col1 += Tot_Esenz_Col1
                    Riepilogo_Esenz_Col2 += Tot_Esenz_Col2

                    'è cambiato il giorno, inserisco la riga nel dataset per il giorno precedente
                    Dataset_InserisciRighe(DSRegCorr, _
                                            Piva, _
                                              rag_soc, _
                                              data_memo, _
                                             Format(Totale_Importo_Data, "##,###,##0.00"), _
                                              note, _
                                              Num_Pagina, _
                                             Format(Tot_Importo_Col1, "##,###,##0.00"), _
                                             Format(Tot_Imposta_Col1, "##,###,##0.00"), _
                                             Format(Tot_Imponibile_Col1, "##,###,##0.00"), _
                                             Format(Tot_Importo_Col2, "##,###,##0.00"), _
                                             Format(Tot_Imposta_Col2, "##,###,##0.00"), _
                                             Format(Tot_Imponibile_Col2, "##,###,##0.00"), _
                                             Format(Tot_Importo_Col3, "##,###,##0.00"), _
                                             Format(Tot_Imposta_Col3, "##,###,##0.00"), _
                                             Format(Tot_Imponibile_Col3, "##,###,##0.00"), _
                                             Format(Tot_Importo_Col4, "##,###,##0.00"), _
                                             Format(Tot_Imposta_Col4, "##,###,##0.00"), _
                                             Format(Tot_Imponibile_Col4, "##,###,##0.00"), _
                                             Format(Tot_Non_Imp_Col1, "##,###,##0.00"), _
                                             Format(Tot_Non_Imp_Col2, "##,###,##0.00"), _
                                             Format(Tot_Esenz_Col1, "##,###,##0.00"), _
                                             Format(Tot_Esenz_Col2, "##,###,##0.00"))

                End If

                'azzero i totali

                Totale_Importo_Data = 0

                Tot_Importo_Col1 = 0
                Tot_Importo_Col2 = 0
                Tot_Importo_Col3 = 0
                Tot_Importo_Col4 = 0

                Tot_Imposta_Col1 = 0
                Tot_Imposta_Col2 = 0
                Tot_Imposta_Col3 = 0
                Tot_Imposta_Col4 = 0

                Tot_Imponibile_Col1 = 0
                Tot_Imponibile_Col2 = 0
                Tot_Imponibile_Col3 = 0
                Tot_Imponibile_Col4 = 0

                Tot_Non_Imp_Col1 = 0
                Tot_Non_Imp_Col2 = 0
                Tot_Esenz_Col1 = 0
                Tot_Esenz_Col2 = 0

                data_memo = data

            Else
                'stesso giorno
            End If

            'sommo per i totali

            Totale_Importo_Data += Importo

            Select Case cod_iva

                Case cod_iva_1
                    Tot_Importo_Col1 += Importo
                    Tot_Imposta_Col1 += Iva
                    Tot_Imponibile_Col1 += Imponibile
                    '----------------------------
                Case cod_iva_2
                    Tot_Importo_Col2 += Importo
                    Tot_Imposta_Col2 += Iva
                    Tot_Imponibile_Col2 += Imponibile
                    '----------------------------
                Case cod_iva_3
                    Tot_Importo_Col3 += Importo
                    Tot_Imposta_Col3 += Iva
                    Tot_Imponibile_Col3 += Imponibile
                    '----------------------------
                Case cod_iva_4
                    Tot_Importo_Col4 += Importo
                    Tot_Imposta_Col4 += Iva
                    Tot_Imponibile_Col4 += Imponibile
                    '----------------------------

                Case Cod_iva_non_imp_1
                    Tot_Non_Imp_Col1 += Importo
                    '----------------------------
                Case Cod_iva_non_imp_2
                    Tot_Non_Imp_Col2 += Importo
                    '----------------------------

                Case Cod_iva_escl_iva_1
                    Tot_Esenz_Col1 += Importo
                    '----------------------------
                Case Cod_iva_escl_iva_2
                    Tot_Esenz_Col2 += Importo
                    '----------------------------

            End Select

        Next 'dt globale

        note = Recupera_Note(DT_Note, data)

        Riepilogo_Importo_Data += Totale_Importo_Data
        Riepilogo_Importo_Col1 += Tot_Importo_Col1
        Riepilogo_Importo_Col2 += Tot_Importo_Col2
        Riepilogo_Importo_Col3 += Tot_Importo_Col3
        Riepilogo_Importo_Col4 += Tot_Importo_Col4
        Riepilogo_Imposta_Col1 += Tot_Imposta_Col1
        Riepilogo_Imposta_Col2 += Tot_Imposta_Col2
        Riepilogo_Imposta_Col3 += Tot_Imposta_Col3
        Riepilogo_Imposta_Col4 += Tot_Imposta_Col4
        Riepilogo_Imponibile_Col1 += Tot_Imponibile_Col1
        Riepilogo_Imponibile_Col2 += Tot_Imponibile_Col2
        Riepilogo_Imponibile_Col3 += Tot_Imponibile_Col3
        Riepilogo_Imponibile_Col4 += Tot_Imponibile_Col4
        Riepilogo_Non_Imp_Col1 += Tot_Non_Imp_Col1
        Riepilogo_Non_Imp_Col2 += Tot_Non_Imp_Col2
        Riepilogo_Esenz_Col1 += Tot_Esenz_Col1
        Riepilogo_Esenz_Col2 += Tot_Esenz_Col2

        'ultimo giorno, inserisco la riga nel dataset per il giorno precedente
        Dataset_InserisciRighe(DSRegCorr, _
                                Piva, _
                                rag_soc, _
                                data, _
                                Format(Totale_Importo_Data, "##,###,##0.00"), _
                                note, _
                                Num_Pagina, _
                                Format(Tot_Importo_Col1, "##,###,##0.00"), _
                                Format(Tot_Imposta_Col1, "##,###,##0.00"), _
                                Format(Tot_Imponibile_Col1, "##,###,##0.00"), _
                                Format(Tot_Importo_Col2, "##,###,##0.00"), _
                                Format(Tot_Imposta_Col2, "##,###,##0.00"), _
                                Format(Tot_Imponibile_Col2, "##,###,##0.00"), _
                                Format(Tot_Importo_Col3, "##,###,##0.00"), _
                                Format(Tot_Imposta_Col3, "##,###,##0.00"), _
                                Format(Tot_Imponibile_Col3, "##,###,##0.00"), _
                                Format(Tot_Importo_Col4, "##,###,##0.00"), _
                                Format(Tot_Imposta_Col4, "##,###,##0.00"), _
                                Format(Tot_Imponibile_Col4, "##,###,##0.00"), _
                                Format(Tot_Non_Imp_Col1, "##,###,##0.00"), _
                                Format(Tot_Non_Imp_Col2, "##,###,##0.00"), _
                                Format(Tot_Esenz_Col1, "##,###,##0.00"), _
                                Format(Tot_Esenz_Col2, "##,###,##0.00"))


        CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImporti"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Importo_Data, "##,###,##0.00")

        If cod_iva_1 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImportoCOL1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Importo_Col1, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtIvaCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imposta_Col1, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImponibileCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imponibile_Col1, "##,###,##0.00")
        End If

        If cod_iva_2 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImportoCOL2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Importo_Col2, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtIvaCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imposta_Col2, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImponibileCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imponibile_Col2, "##,###,##0.00")
        End If

        If cod_iva_3 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImportoCOL3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Importo_Col3, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtIvaCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imposta_Col3, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImponibileCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imponibile_Col3, "##,###,##0.00")
        End If

        If cod_iva_4 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImportoCOL4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Importo_Col4, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtIvaCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imposta_Col4, "##,###,##0.00")
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtImponibileCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Imponibile_Col4, "##,###,##0.00")
        End If

        If Cod_iva_non_imp_1 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtNonImpCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Non_Imp_Col1, "##,###,##0.00")
        End If
        If Cod_iva_non_imp_2 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtNonImpCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Non_Imp_Col2, "##,###,##0.00")
        End If
        If Cod_iva_escl_iva_1 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtEsenteCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Esenz_Col1, "##,###,##0.00")
        End If
        If Cod_iva_escl_iva_2 <> -999 Then
            CType(rptRegCorrispettivi.Section4.ReportObjects("TxtEsenteCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Riepilogo_Esenz_Col2, "##,###,##0.00")
        End If


    End Sub

    '#######################################################################
    Private Function Recupera_Note(ByVal DT_Note As DataTable, _
                                ByVal data As String) As String

        Dim note As String = ""

        Try
            If Not IsNothing(DT_Note) AndAlso DT_Note.Rows.Count > 0 Then
                Dim riga_note() As DataRow
                riga_note = DT_Note.Select("Data = '" + CDate(data) + "'")
                If Not IsNothing(riga_note) AndAlso riga_note.Length > 0 Then
                    note = riga_note(0).Item("Note")
                End If
            End If

        Catch ex As Exception
            Log_Errori += "- Elaborazione delle note: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Return note

    End Function


    '########################################################################
    Private Sub Valorizza_Label_Colonne(ByVal DT_Iva_Aliquote As DataTable)

        Dim sigla1, sigla2, sigla3, sigla4 As String
        Dim nonimp1, nonimp2, esente1, esente2 As String
        Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        sigla1 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", cod_iva_1)
        sigla2 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", cod_iva_2)
        sigla3 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", cod_iva_3)
        sigla4 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", cod_iva_4)

        nonimp1 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", Cod_iva_non_imp_1)
        nonimp2 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", Cod_iva_non_imp_2)
        esente1 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", Cod_iva_escl_iva_1)
        esente2 = objHLP.Des_from_Cod(DT_Iva_Aliquote, "Codice", "sigla", Cod_iva_escl_iva_2)


        Dim TxtTitoloCol1, TxtTitoloCol2, TxtTitoloCol3, TxtTitoloCol4 As String
        Dim TxtTitoloCol5, TxtTitoloCol6, TxtTitoloCol7, TxtTitoloCol8 As String

        TxtTitoloCol1 = "Aliquota " & sigla1
        TxtTitoloCol2 = "Aliquota " & sigla2
        TxtTitoloCol3 = "Aliquota " & sigla3
        TxtTitoloCol4 = "Aliquota " & sigla4

        TxtTitoloCol5 = nonimp1
        TxtTitoloCol6 = nonimp2
        TxtTitoloCol7 = esente1
        TxtTitoloCol8 = esente2

        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol1
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol2
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol3
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol4
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol5
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol6"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol6
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol7"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol7
        CType(rptRegCorrispettivi.Section2.ReportObjects("TxtTitoloCol8"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTitoloCol8

        '---

        Dim LblImportoCol1, LblImportoCol2, LblImportoCol3, LblImportoCol4 As String

        LblImportoCol1 = "Importo " & sigla1
        LblImportoCol2 = "Importo " & sigla2
        LblImportoCol3 = "Importo " & sigla3
        LblImportoCol4 = "Importo " & sigla4

        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImportoCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImportoCol1
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImportoCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImportoCol2
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImportoCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImportoCol3
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImportoCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImportoCol4

        '---

        Dim LblImponibileCol1, LblImponibileCol2, LblImponibileCol3, LblImponibileCol4 As String

        LblImponibileCol1 = "Imponib. " & sigla1
        LblImponibileCol2 = "Imponib. " & sigla2
        LblImponibileCol3 = "Imponib. " & sigla3
        LblImponibileCol4 = "Imponib. " & sigla4

        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImponibileCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImponibileCol1
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImponibileCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImponibileCol2
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImponibileCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImponibileCol3
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblImponibileCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblImponibileCol4


        '---

        Dim LblIvaCol1, LblIvaCol2, LblIvaCol3, LblIvaCol4 As String

        LblIvaCol1 = "Iva " & sigla1
        LblIvaCol2 = "Iva " & sigla2
        LblIvaCol3 = "Iva " & sigla3
        LblIvaCol4 = "Iva " & sigla4

        CType(rptRegCorrispettivi.Section4.ReportObjects("LblIvaCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblIvaCol1
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblIvaCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblIvaCol2
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblIvaCol3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblIvaCol3
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblIvaCol4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblIvaCol4


        '---

        Dim LblNonImpCol1, LblNonImpCol2 As String

        LblNonImpCol1 = nonimp1
        LblNonImpCol2 = nonimp2

        CType(rptRegCorrispettivi.Section4.ReportObjects("LblNonImpCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblNonImpCol1
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblNonImpCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblNonImpCol2


        '---

        Dim LblEsenteCol1, LblEsenteCol2 As String

        LblEsenteCol1 = esente1
        LblEsenteCol2 = esente2

        CType(rptRegCorrispettivi.Section4.ReportObjects("LblEsenteCol1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblEsenteCol1
        CType(rptRegCorrispettivi.Section4.ReportObjects("LblEsenteCol2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = LblEsenteCol2


    End Sub



    '########################################################################
    Private Sub Dataset_InserisciRighe(ByRef DSRegCorr As DS_RegistroCorrispettivi, _
                                       ByVal Piva As String, _
                                       ByVal rag_soc As String, _
                                       ByVal Data As String, _
                                       ByVal Totale As String, _
                                       ByVal Note As String, _
                                       ByVal Num_Pagina As Integer, _
                                       ByVal Col1_Importo As String, _
                                       ByVal Col1_Imposta As String, _
                                       ByVal Col1_Imponibile As String, _
                                       ByVal Col2_Importo As String, _
                                       ByVal Col2_Imposta As String, _
                                       ByVal Col2_Imponibile As String, _
                                       ByVal Col3_Importo As String, _
                                       ByVal Col3_Imposta As String, _
                                       ByVal Col3_Imponibile As String, _
                                       ByVal Col4_Importo As String, _
                                       ByVal Col4_Imposta As String, _
                                       ByVal Col4_Imponibile As String, _
                                       ByVal Col1_NonImponibile As String, _
                                       ByVal Col2_NonImponibile As String, _
                                       ByVal Col1_EsclIva As String, _
                                       ByVal Col2_EsclIva As String)

        Dim DrR As DS_RegistroCorrispettivi.RegistriCorrispettiviRow

        DrR = DSRegCorr.RegistriCorrispettivi.NewRow

        DrR.Num_Pagina = Num_Pagina
        DrR.piva = Piva
        DrR.rag_soc = rag_soc

        DrR.Data = Data
        DrR.Totale = Totale
        DrR.Note = Note

        If cod_iva_1 <> -999 Then
            DrR.Col1_Importo = Col1_Importo
            DrR.Col1_Imposta = Col1_Imposta
            DrR.Col1_Imponibile = Col1_Imponibile
        Else
            DrR.Col1_Importo = ""
            DrR.Col1_Imposta = ""
            DrR.Col1_Imponibile = ""
        End If

        If cod_iva_2 <> -999 Then
            DrR.Col2_Importo = Col2_Importo
            DrR.Col2_Imposta = Col2_Imposta
            DrR.Col2_Imponibile = Col2_Imponibile
        Else
            DrR.Col2_Importo = ""
            DrR.Col2_Imposta = ""
            DrR.Col2_Imponibile = ""
        End If

        If cod_iva_3 <> -999 Then
            DrR.Col3_Importo = Col3_Importo
            DrR.Col3_Imposta = Col3_Imposta
            DrR.Col3_Imponibile = Col3_Imponibile
        Else
            DrR.Col3_Importo = ""
            DrR.Col3_Imposta = ""
            DrR.Col3_Imponibile = ""
        End If

        If cod_iva_4 <> -999 Then
            DrR.Col4_Importo = Col4_Importo
            DrR.Col4_Imposta = Col4_Imposta
            DrR.Col4_Imponibile = Col4_Imponibile
        Else
            DrR.Col4_Importo = ""
            DrR.Col4_Imposta = ""
            DrR.Col4_Imponibile = ""
        End If

        If Cod_iva_non_imp_1 <> -999 Then
            DrR.Col1_NonImponibile = Col1_NonImponibile
        Else
            DrR.Col1_NonImponibile = ""
        End If
        If Cod_iva_non_imp_2 <> -999 Then
            DrR.Col2_NonImponibile = Col2_NonImponibile
        Else
            DrR.Col2_NonImponibile = ""
        End If

        If Cod_iva_escl_iva_1 <> -999 Then
            DrR.Col1_EsclIva = Col1_EsclIva
        Else
            DrR.Col1_EsclIva = ""
        End If
        If Cod_iva_escl_iva_2 <> -999 Then
            DrR.Col2_EsclIva = Col2_EsclIva
        Else
            DrR.Col2_EsclIva = ""
        End If

        DSRegCorr.RegistriCorrispettivi.Rows.Add(DrR)

    End Sub



End Class
