Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Gandini_DocContabile
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptGandiniDoc As Rpt_Gandini_DocContabile
    Private rptGandiniDoc_conLayout As Rpt_Gandini_DocContabile_ConLayout

    Dim Piva As String
    Dim Lav_Cod As Integer
    Dim Id_Agenda As Integer
    Dim Cod_Report As enum_CodificaStampe
    Dim IdentificazioneDocumento As String = ""
    Dim Flag_StampaDettagliTrasporto As Boolean
    Dim Flag_StampaDettagliEconomici As Boolean

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        rptGandiniDoc = New Rpt_Gandini_DocContabile
        rptGandiniDoc_conLayout = New Rpt_Gandini_DocContabile_ConLayout
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))

        Cod_Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder, Server))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Dim Nome_Documento As String = "GandiniDoc"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            Dim CatCod As Integer

            Log_Errori = ""

            Select Case Lav_Cod
                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_PROFORMA
                    Nome_Documento = "Fattura"
                    CatCod = enum_CategorieDocumenti.Fattura_Emessa
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    Nome_Documento = "NotaAccredito"
                    CatCod = enum_CategorieDocumenti.NotaAccredito_Emessa
                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    Nome_Documento = "DDT_Contabilizzato"
                    CatCod = enum_CategorieDocumenti.DDT_Contabilizzato_Emesso
                Case LAVCOD_ORDINE_VENDITA
                    Nome_Documento = "Ordine_Vendita"
                    CatCod = enum_CategorieDocumenti.Ordine_Vendita
                Case LAVCOD_ORDINE_ACQUISTO
                    Nome_Documento = "Ordine_Acquisto"
                    CatCod = enum_CategorieDocumenti.Ordine_Acquisto
                Case LAVCOD_PREVENTIVO_VENDITA
                    Nome_Documento = "Preventivo"
                    CatCod = enum_CategorieDocumenti.Preventivo_Emesso
                Case LAVCOD_BOLLA_EMESSA
                    Nome_Documento = "DDT"
                    CatCod = enum_CategorieDocumenti.DDT_Emesso
            End Select

            Try

                Stampa_Gandini_DocContabile(Log_Errori)

            Catch exc As Exception
                Log_Errori += "- Stampa_Gandini_Doccontabile: " + vbCrLf + exc.Message + vbCrLf
            End Try


            Try

                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri_Server)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptGandiniDoc_conLayout, _
                                           CatCod, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                                                                 CatCod, _
                                                                 Nome_Documento, _
                                                                 Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                                 Sottocartella, _
                                                                 Id_Agenda, "", "", "", _
                                                                 Session("Data_Movimento"), _
                                                                 Session("Scadenza"), _
                                                                 objParametri_Server)

            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", id_agenda = " + CStr(Id_Agenda) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + "_" + CStr(Session("ASG_Utente_Username")) + "_id" + CStr(Id_Agenda) + ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Contabilita", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "Fattura_NotaAccredito.aspx", _
                                                 Log_Errori)

            End If

            '----------------------------------------

            Session("Report") = rptGandiniDoc
            Response.Redirect("..\..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        End If

        '==================================================================


    End Sub

    '#####################################################################################################
    <Obsolete("Al momento non viene più usata. ormai disallineata su nuovi sviluppi")>
    Private Sub Stampa_Gandini_DocContabile(ByRef Log_Errori As String)

        Dim DS_Gandini As New DS_Gandini_DocContabile
        Dim Dr As DS_Gandini_DocContabile.DTGandiniDocContabileRow

        Dim i As Integer
        Dim Msg_Errore As String = ""

        '------------------

        ' Dim Parametro_PivaCliente As String
        'Dim Parametro_CFCliente As String
        Dim Parametro_TipoDocumento As String = ""
        Dim Parametro_NumDoc As String = ""
        Dim Parametro_Scadenza As String = ""
        Dim Parametro_DataDoc As String = ""
        Dim Parametro_Note As String = ""
        Dim Parametro_Aspetto As String = ""
        Dim Parametro_Colli As String = ""
        Dim Parametro_Causale_Trasporto As String = ""
        Dim Parametro_Gest_Vettore As String = ""
        Dim Parametro_DataOraRitiro As String = ""

        Dim Parametro_Destinatario_riga1 As String = ""
        Dim Parametro_Destinatario_riga2 As String = ""
        Dim Parametro_Destinatario_riga3 As String = ""
        Dim Parametro_Destinatario_riga4 As String = ""
        Dim Parametro_Destinatario_riga5 As String = ""

        Dim Parametro_DestDiversa_riga1 As String = ""
        Dim Parametro_DestDiversa_riga2 As String = ""
        Dim Parametro_DestDiversa_riga3 As String = ""
        Dim Parametro_DestDiversa_riga4 As String = ""

        Dim Parametro_Mezzo As String = ""

        Dim Parametro_Vettore_riga1 As String = ""
        Dim Parametro_Vettore_riga2 As String = ""
        Dim Parametro_Vettore_riga3 As String = ""
        Dim Parametro_Vettore_riga4 As String = ""

        Dim Parametro_Pagamenti_riga1 As String = ""
        Dim Parametro_Pagamenti_riga2 As String = ""

        Dim Parametro_Imponibile_riga1 As String = ""
        Dim Parametro_Iva_riga1 As String = ""
        Dim Parametro_Imponibile_riga2 As String = ""
        Dim Parametro_Iva_riga2 As String = ""

        Dim Parametro_Riepilogo_ImponibileNetto As String = ""
        Dim Parametro_Riepilogo_Imposta As String = ""
        Dim Parametro_Riepilogo_Importo As String = ""

        Dim Parametro_NoteArticolo62Conai As String = ""

        '------------------
        'Movimenti

        Dim x_Id_Mov_Contabile As Integer = 0
        Dim x_Mov_Desc_Contabile As String = ""
        Dim x_Data_Movimento As Date = #1/1/1900#
        Dim x_Ora As String = Date.Now.ToString
        Dim x_Scadenza As Date = #12/31/2100#
        Dim x_Scadenza_Extra As Date = #12/31/2100#
        Dim x_Doc_Numero_Sin As String = ""
        Dim x_Doc_Numero As Integer = 0
        Dim x_Doc_Numero_Des As String = ""
        Dim x_Progr_Protocollo As Integer = 0
        Dim x_Progr_Registrazione As Integer = 0
        Dim x_Data_Registrazione As Date = #1/1/1900#
        Dim x_Num_Protocollo As Double = 0
        Dim x_Colli As Integer = 0
        Dim x_Peso As Double = 0
        Dim x_Aspetto As String = ""
        Dim x_Causale_Trasporto As String = ""
        'Dim x_Tipo_Sconto As Integer = 0
        Dim x_Edit_Importo As enum_EditImporto = enum_EditImporto.PrezzoUnitario
        Dim x_Cod_RisUm As Integer = 0
        Dim x_Cod_Contatto As String = ""
        Dim x_Rag_Soc As String = ""
        Dim x_Codice_Fiscale As String = ""
        Dim x_Cod_IndirizzoRisUm As Integer = 0
        Dim x_Id_Cf_Cliente As Integer = 0
        Dim x_Cod_Destinazione As Integer = 0
        Dim x_CodContatto_Destinazione As String = ""
        Dim x_RagSoc_Destinazione As String = ""
        Dim x_CodiceFiscale_Destinazione As String = ""
        Dim x_Cod_IndirizzoDestinazione As Integer = 0
        Dim x_Id_Cf_Destinazione As Integer = 0

        Dim x_Cod_RisUm_Aggiuntivo As Integer = 0
        Dim x_Cod_Indirizzo_Aggiuntivo As Integer = 0
        Dim x_Id_Cf_Aggiuntivo As Integer = 0
        Dim x_CodContatto_Aggiuntivo As String = ""
        Dim x_RagSoc_Aggiuntivo As String = ""
        Dim x_CodiceFiscale_Aggiuntivo As String = ""

        Dim x_Mezzo As Integer = 0
        Dim x_Cod_Vettore As Integer = 0
        Dim x_Cod_IndirizzoVettore As Integer = 0
        Dim x_Id_Cf_Vettore As Integer = 0
        Dim x_CodContatto_Vettore As String = ""
        Dim x_RagSoc_Vettore As String = ""
        Dim x_CodiceFiscale_Vettore As String = ""
        Dim x_NumReg_Vettore As String = ""
        Dim x_TargaMezzo_Vettore As String = ""
        Dim x_Agente_Cod As Integer = 0
        Dim x_Natura_Beni As String = ""
        Dim x_Modalita As Integer = 0
        Dim x_Tara_Veicolo As Double = 0
        Dim x_Tara_Imballi As Double = 0
        Dim x_Tipo_Peso As Integer = 0
        Dim Peso_Lordo As Double = 0
        Dim x_Username_Note As String = ""
        Dim x_Extra_Str As String = ""
        Dim x_Extra_Int As Integer = 0
        Dim x_Extra_Date As Date = #1/1/1900#
        Dim x_ChkLayout_Bypass_Fatturato As Integer = 0
        Dim x_ChkLayout_Join_Prodotti As Integer = 0
        Dim x_ChkLayOut_Peso As Integer = 0
        Dim x_ChkLayOut_Prezzo As Integer = 0
        Dim x_ChkLayOut_Riscontrato As Integer = 0
        Dim x_Gestione_Vettore As String = ""

        Dim x_ChkLayOut_Litri As Integer = 0
        Dim Litri_Totali As Decimal = 0

        Dim Piva_Cliente As String
        Dim Codice_Fiscale_Cliente As String
        Dim x_Ind_Des As String = ""
        Dim x_Frz_Des As String = ""
        Dim x_Cap As String = ""
        Dim x_Comune As String = ""
        Dim x_Provincia As String = ""
        Dim x_Stato As String = ""
        Dim Flag_StatoMembro As Boolean = True

        Dim Piva_Dest As String
        Dim Codice_Fiscale_Dest As String
        Dim x_IndDes_Destinazione As String = ""
        Dim x_FrzDes_Destinazione As String = ""
        Dim x_Cap_Destinazione As String = ""
        Dim x_Comune_Destinazione As String = ""
        Dim x_Provincia_Destinazione As String = ""
        Dim x_Stato_Destinazione As String = ""

        Dim x_IndDes_Vettore As String = ""
        Dim x_FrzDes_Vettore As String = ""
        Dim x_Cap_Vettore As String = ""
        Dim x_Comune_Vettore As String = ""
        Dim x_Provincia_Vettore As String = ""
        Dim x_Stato_Vettore As String = ""

        '------------------------------
        'Movimenti Dettagli

        Dim x_Mov_Det_Des As String = ""
        Dim x_Elem_Cod As Integer = 0
        Dim x_Pro_Cod As Integer = 0
        Dim x_Mat_Cod As Integer = 0
        Dim x_Cod_Progetto As Integer = 0
        Dim x_Fase_Cod As Integer = 0
        Dim x_Lotto As String = ""
        Dim x_Cal_Cod As Integer = 0
        Dim x_Udm_Cod As Integer = 0
        Dim x_Udm_Sim As String = ""
        Dim x_Udm_Des As String = ""
        Dim x_Udm_Cod_Extra As Integer = 0
        Dim x_Udm_Sim_Extra As String = ""
        Dim x_Udm_Des_Extra As String = ""
        Dim x_Qta As Double = 0
        Dim x_Qta_Extra As Double = 0
        Dim x_Qta_Extra_Totale As Double = 0
        Dim x_Descrizione As String = ""
        Dim x_Prezzo_Unitario As Double = 0
        Dim x_Prezzo_Unitario_Netto As Double = 0
        Dim x_Imponibile As Double = 0
        Dim x_Imponibile_Netto As Double = 0
        Dim x_Cod_IVA As Integer = 0
        Dim x_Aliquota As String = ""
        Dim x_IVA As Double = 0
        Dim x_ChkIVA_Manuale As Integer = 0
        Dim x_Cod_IVAIndetraibile As Integer = 0
        Dim x_Sconto_Perc As Double = 0
        Dim x_Sconto_Perc_2 As Double = 0
        Dim x_Sconto_Testo As String = ""
        Dim x_Sconto As Double = 0
        Dim x_Sconto_Modalita As Integer = 0
        Dim x_Prezzo_Effettivo As Double = 0
        Dim x_Anno As Integer = 0
        Dim x_Ric_Cod As Integer = 0
        Dim x_Cod_Conto As Integer = 0
        Dim x_Conto As String = ""
        Dim x_Contabilizzato As Integer = 0
        Dim x_Pendente As Integer = 0
        Dim x_ChkLayOut_Hide As Integer = 0
        Dim x_Tara As Double = 0
        Dim x_Extra_Str_Dettagli As String = ""
        Dim x_Extra_Int_Dettagli As Integer = 0
        Dim x_Extra_Date_Dettagli As Date = #1/1/1900#

        Dim x_Veg_Cod As Integer = 0
        Dim x_Cul_Cod As Integer = 0

        Dim numcolli_contenitori As Integer = 0
        Dim contenitore_cod As Integer = 0
        Dim descr_contenitore As String = ""
        Dim numcontenitori_imballi As Integer = 0
        Dim imballaggio_cod As Integer = 0
        Dim descr_imballo As String = ""
        Dim Flag_DDTallegati As Boolean = False
        Dim Flag_ORDAllegati As Boolean = False
        Dim ChkConfezione As Integer = 0
        Dim ChkContenitore As Integer = 0
        Dim ChkImballaggio As Integer = 0

        '------------------------------
        'Pagamenti
        Dim x_Cod_Liquidita_Dare As Integer = 0
        Dim x_RisorsaFinanziaria_Dare As String = ""
        Dim x_Cod_Liquidita_Avere As Integer = 0
        Dim x_RisorsaFinanziaria_Avere As String = ""
        Dim x_Note_Pagamento As String = ""
        Dim x_Percentuale_Pagamento As Integer = 0
        Dim x_Importo_Pagamento As Double = 0
        Dim x_Data_Pagamento As Date = #1/1/1900#
        Dim x_Cau_Pagamento As Integer = 0
        Dim x_Cau_Pagamento_Des As String = ""
        Dim x_Cau_Pagamento_Sigla As String = ""
        Dim Pagamento_Riscosso As String = ""
        Dim Pagamento_Insoluto As String = ""
        'Dim Importo_Pagato As Double = 0

        '--------------------------------
        'Riferimenti Documento Allegato

        Dim LavCod_DocAllegato As Integer = 0
        Dim Piva_DocAllegato As String = ""
        Dim SaCod_DocAllegato As Integer = 0
        Dim IdAgenda_DocAllegato As Integer = 0
        Dim IdMov_DocAllegato As Integer = 0
        Dim IdMovDet_DocAllegato As Integer = 0
        Dim DocNumeroSin_DocAllegato As String = ""
        Dim DocNumero_DocAllegato As String = ""
        Dim DocNumeroDes_DocAllegato As String = ""
        Dim Data_DocAllegato As String = ""
        Dim Riferimento_DocAllegato As String = ""
        Dim Riferimento_DocAllegato_Prec As String = ""

        '------------------------------
        'Dati 
        Dim Riga_Sconto As String
        Dim Riga_Importo As String
        Dim Importo_Riga_DDT As String
        Dim Importo_Dettaglio As Double = 0
        'Dim Riga_Qta As String
        'Dim Riga_Prezzo As String
        'Dim Riga_Iva As String
        'Dim Tot_Imp, Tot_ImpNetto As Double
        'Dim Tot_IVA As Double
        'Dim Tot_Sconto As Double

        Dim Flag_Campioni_Omaggio As Boolean = False
        Dim Flag_Campioni_Omaggio_Rivalsa_Iva As Boolean = False
        Dim Flag_Campioni_Gratuiti As Boolean = False
        Dim Flag_Sconto_Merce As Boolean = False

        Dim Flag_AlmenoUnoProdottoAgroAlimentare As Boolean = False
        Dim Flag_ProdottoAgroAlimentare As Boolean = False
        Dim Flag_ClientePrivato As Boolean = False

        Dim Str_NoteIntegrative1 As String = ""
        Dim Str_NoteIntegrative2 As String = ""
        Dim Str_NoteIntegrative3 As String = ""
        Dim Str_Articolo62 As String = ""
        Dim Flag_contributoCONAI As Boolean = False
        Dim Flag_StampaRifOrdine As Boolean = False
        Dim Flag_GradoAlcolico As Boolean = False
        Dim Flag_GestMaterialeVivaistico As Boolean = False
        Dim Flag_CentroAziendalePartenza As Boolean = False

        Dim Dettaglio_Evasione As String

        '########################## Impostazioni Utente ###########################################
        Try

            ImpostazioniUtente_StampaDoc(objParametri_Utenti,
                                         Log_Errori,
                                         Lav_Cod,
                                         Str_NoteIntegrative1,
                                         Str_NoteIntegrative2,
                                         Str_NoteIntegrative3,
                                         Str_Articolo62,
                                         Flag_contributoCONAI,
                                         Flag_StampaRifOrdine,
                                         Flag_GradoAlcolico,
                                         Flag_GestMaterialeVivaistico,
                                         Flag_CentroAziendalePartenza)

            'If Flag_contributoCONAI = True Then
            '    CType(rptFattura.Section17.ReportObjects("TxtCONAI"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Contributo ambientale Conai assolto ove dovuto"
            'Else
            '    CType(rptFattura.Section17.ReportObjects("TxtCONAI"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            'End If

        Catch ex As Exception
            Log_Errori += "- Lettura impostazioni utente: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '############################################################################################
        '########################## Lettura della Fattura ###########################################
        '############################################################################################

        Dim Dt_Documento As DataTable


        Try
            'x_RagSoc_Impresa
            'x_CodiceFiscale_Impresa

            'Leggo i dati relativi al nodo Movimento (contabile)
            Leggi_Movimenti_DocContabile(objParametri_Server, _
                                            Msg_Errore, _
                                            Piva, _
                                            Id_Agenda, _
                                            Cod_Report, _
                                            Dt_Documento, _
                                            x_Id_Mov_Contabile, _
                                            "", _
                                            "", _
                                            x_Mov_Desc_Contabile, _
                                            x_Data_Movimento, _
                                            x_Ora, _
                                            x_Scadenza, _
                                            x_Scadenza_Extra, _
                                            x_Doc_Numero_Sin, _
                                            x_Doc_Numero, _
                                            x_Doc_Numero_Des, _
                                            x_Progr_Protocollo, _
                                            x_Progr_Registrazione, _
                                            x_Data_Registrazione, _
                                            x_Num_Protocollo, _
                                            x_Colli, _
                                            x_Peso, _
                                            x_Aspetto, _
                                            x_Causale_Trasporto, _
                                            x_Edit_Importo, _
                                            x_Cod_RisUm, _
                                            x_Cod_Contatto, _
                                            x_Rag_Soc, _
                                            x_Codice_Fiscale, _
                                            0,
                                            x_Cod_IndirizzoRisUm, _
                                            x_Id_Cf_Cliente, _
                                            x_Cod_Destinazione, _
                                            x_CodContatto_Destinazione, _
                                            x_RagSoc_Destinazione, _
                                            x_CodiceFiscale_Destinazione, _
                                            0,
                                            x_Cod_IndirizzoDestinazione, _
                                            x_Id_Cf_Destinazione, _
                                            x_Mezzo, _
                                            x_Cod_Vettore, _
                                            x_Cod_IndirizzoVettore, _
                                            x_Id_Cf_Vettore, _
                                            x_CodContatto_Vettore, _
                                            x_RagSoc_Vettore, _
                                            x_CodiceFiscale_Vettore, _
                                            0,
                                            x_NumReg_Vettore, _
                                            x_TargaMezzo_Vettore, _
                                            Nothing, _
                                            x_Agente_Cod, _
                                            x_Natura_Beni, _
                                            x_Modalita, _
                                            x_Tara_Veicolo, _
                                            x_Tara_Imballi, _
                                            x_Tipo_Peso, _
                                            x_Username_Note, _
                                            x_Extra_Str, _
                                            x_Extra_Int, _
                                            x_Extra_Date, _
                                            x_ChkLayout_Bypass_Fatturato, _
                                            x_ChkLayout_Join_Prodotti, _
                                            x_ChkLayOut_Peso, _
                                            x_ChkLayOut_Prezzo, _
                                            x_Gestione_Vettore, _
                                            Peso_Lordo, _
                                            x_ChkLayOut_Litri, _
                                            x_Cod_RisUm_Aggiuntivo, _
                                            x_Cod_Indirizzo_Aggiuntivo, _
                                            x_Id_Cf_Aggiuntivo, _
                                            x_CodContatto_Aggiuntivo, _
                                            x_RagSoc_Aggiuntivo, _
                                            x_CodiceFiscale_Aggiuntivo, _
                                            0,
                                            Nothing, _
                                            x_ChkLayOut_Riscontrato,
                                            Nothing,
                                               Nothing,
                                               Nothing,
                                                Nothing,
                                               Nothing,
                                               Nothing,
                                               Nothing, _
                                               Nothing)

            If Msg_Errore <> "" Then
                Log_Errori += Msg_Errore
            End If

        Catch ex As Exception
            Log_Errori += "- Lettura della Fattura: " + vbCrLf + ex.Message + vbCrLf
        End Try



        '=============================================================
        '-------------- CLIENTE -------------------

        If x_Cod_IndirizzoRisUm <> 0 Then

            Try

                Leggi_Indirizzi(objParametri_Server, _
                                   x_Cod_Contatto, _
                                    x_Cod_RisUm, _
                                    x_Cod_IndirizzoRisUm, _
                                    x_Ind_Des, _
                                    x_Frz_Des, _
                                    x_Cap, _
                                    x_Comune, _
                                    x_Provincia, _
                                    x_Stato, _
                                    Flag_StatoMembro,
                                    Nothing, _
                                    "")

            Catch ex As Exception
                'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
                Log_Errori += "- Lettura dell'indirizzo del contatto cliente: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        Dim Cliente_RagSoc As String
        Dim Cliente_Indirizzo As String
        Dim Cliente_Cap As String
        Dim Cliente_Frazione As String
        Dim Cliente_Comune As String
        Dim Cliente_Provincia As String
        Dim Cliente_Piva As String
        Dim Cliente_CF As String

        Dim Fornitore_RagSoc As String
        Dim Fornitore_Indirizzo As String
        Dim Fornitore_Cap As String
        Dim Fornitore_Frazione As String
        Dim Fornitore_Comune As String
        Dim Fornitore_Provincia As String
        Dim Fornitore_Piva As String
        Dim Fornitore_CF As String

        Select Case Lav_Cod

            Case LAVCOD_ORDINE_ACQUISTO
                'IL CLIENTE E' L'AZIENDA
                'Cliente_RagSoc = x_RagSoc_Impresa
                'Cliente_Indirizzo = x_IndDes_Impresa
                'Cliente_Cap = x_Cap_Impresa
                'Cliente_Frazione = x_FrzDes_Impresa
                'Cliente_Comune = x_Comune_Impresa
                'Cliente_Provincia = "(" & x_Provincia_Impresa & ")"
                'Cliente_Piva = Piva
                'Cliente_CF = x_CodiceFiscale_Impresa

                Fornitore_RagSoc = x_Rag_Soc

                Select Case x_Id_Cf_Cliente
                    Case 2
                        '----------------
                        '-----ESTERO
                        '----------------
                        Fornitore_Indirizzo = x_Ind_Des
                        Fornitore_Frazione = x_Frz_Des
                        Fornitore_Cap = x_Cap
                        Fornitore_Comune = x_Stato
                        Fornitore_Provincia = ""
                        Fornitore_CF = ""
                        If Flag_StatoMembro = True Then
                            Fornitore_Piva = "Codice VAT: " & x_Cod_Contatto
                        Else
                            Fornitore_Piva = ""
                        End If

                    Case Else
                        '----------------
                        '-----ITALIANO
                        '----------------
                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente, _
                                                  x_Cod_Contatto, _
                                                    x_Codice_Fiscale, _
                                                    0,
                                                    Fornitore_Piva, _
                                                    Fornitore_CF, _
                                                    Flag_ClientePrivato)

                        Fornitore_Piva = "Partita IVA: " & Fornitore_Piva
                        Fornitore_CF = "   Codice Fiscale: " & Fornitore_CF
                        Fornitore_Indirizzo = x_Ind_Des
                        Fornitore_Frazione = x_Frz_Des
                        Fornitore_Cap = x_Cap
                        Fornitore_Comune = x_Comune
                        Fornitore_Provincia = "(" & x_Provincia & ")"

                End Select

            Case Else
                'IL CLIENTE E' IL CLIENTE
                Cliente_RagSoc = x_Rag_Soc

                Piva_Cliente = ""
                Codice_Fiscale_Cliente = ""
                Select Case x_Id_Cf_Cliente
                    Case 2
                        '----------------
                        '-----ESTERO
                        '----------------
                        Cliente_Indirizzo = x_Ind_Des
                        Cliente_Frazione = x_Frz_Des
                        Cliente_Cap = x_Cap
                        Cliente_Comune = x_Stato
                        Cliente_Provincia = ""

                        Piva_Cliente = x_Cod_Contatto
                        Codice_Fiscale_Cliente = ""

                        If Flag_StatoMembro = True Then
                            Cliente_Piva = "Codice VAT:" & Piva_Cliente
                        Else
                            Piva_Cliente = ""
                            Piva_Cliente = ""
                        End If
                        'Parametro_CFCliente = ""

                    Case Else
                        '----------------
                        '-----ITALIANO
                        '----------------
                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente, _
                                                  x_Cod_Contatto, _
                                                    x_Codice_Fiscale, _
                                                    0,
                                                    Piva_Cliente, _
                                                    Codice_Fiscale_Cliente, _
                                                    Flag_ClientePrivato)

                        Cliente_Indirizzo = x_Ind_Des
                        Cliente_Frazione = x_Frz_Des
                        Cliente_Cap = x_Cap
                        Cliente_Comune = x_Comune
                        Cliente_Provincia = "(" & x_Provincia & ")"

                        If x_Id_Cf_Cliente = 0 Then
                            Piva_Cliente = ""
                        End If
                End Select

                If Piva_Cliente <> "" Then
                    Cliente_Piva = "P.Iva: " & CStr(Piva_Cliente)
                End If
                If Codice_Fiscale_Cliente <> "" Then
                    Cliente_CF = "C.F. : " & Codice_Fiscale_Cliente
                End If

        End Select

        '=============================================================
        '-------------- DESTINATARIO DOCUMENTO -------------------

        Dim Destinatario_RagSoc As String
        Dim Destinatario_Ind_Des As String
        Dim Destinatario_Frz_Des As String
        Dim Destinatario_Cap As String
        Dim Destinatario_Comune As String
        Dim Destinatario_Provincia As String

        'Destinazione Diversa impostata
        If x_Cod_Destinazione <> 0 Then

            Destinatario_RagSoc = x_RagSoc_Destinazione

            Try

                Leggi_Indirizzi(objParametri_Server, _
                                x_CodContatto_Destinazione, _
                                    x_Cod_Destinazione, _
                                    x_Cod_IndirizzoDestinazione, _
                                    x_IndDes_Destinazione, _
                                    x_FrzDes_Destinazione, _
                                    x_Cap_Destinazione, _
                                    x_Comune_Destinazione, _
                                    x_Provincia_Destinazione, _
                                    x_Stato_Destinazione, _
                                    Nothing,
                                    Nothing, _
                                    "")

            Catch ex As Exception
                Log_Errori += "- Lettura dell'indirizzo del destinatario: " + vbCrLf + ex.Message + vbCrLf
                'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
            End Try

        Else

            Select Case Lav_Cod

                Case LAVCOD_ORDINE_ACQUISTO
                    'il destinatario coincide con l'azienda
                    'Destinatario_RagSoc = x_RagSoc_Impresa

                    'Destinatario_Ind_Des = x_IndDes_Impresa
                    'Destinatario_Frz_Des = x_FrzDes_Impresa
                    'Destinatario_Cap = x_Cap_Impresa
                    'Destinatario_Comune = x_Comune_Impresa
                    'Destinatario_Provincia = "(" & x_Provincia_Impresa & ")"

                Case Else
                    'il destinatario coincide con il cliente
                    Destinatario_RagSoc = x_Rag_Soc

                    x_Id_Cf_Destinazione = x_Id_Cf_Cliente
                    x_RagSoc_Destinazione = x_Rag_Soc
                    x_CodContatto_Destinazione = x_Cod_Contatto
                    x_CodiceFiscale_Destinazione = x_Codice_Fiscale

                    x_IndDes_Destinazione = x_Ind_Des
                    x_FrzDes_Destinazione = x_Frz_Des
                    x_Cap_Destinazione = x_Cap
                    x_Comune_Destinazione = x_Comune
                    x_Provincia_Destinazione = x_Provincia
                    x_Stato_Destinazione = x_Stato

            End Select

        End If 'If x_Cod_Destinazione <> 0

        'nel caso in cui la destinazione non sia l'azienda
        If x_Cod_Destinazione <> 0 Or Lav_Cod <> LAVCOD_ORDINE_ACQUISTO Then

            Piva_Dest = ""
            Codice_Fiscale_Dest = ""

            Select Case x_Id_Cf_Destinazione
                Case 2
                    Destinatario_Ind_Des = x_IndDes_Destinazione
                    Destinatario_Frz_Des = x_FrzDes_Destinazione
                    Destinatario_Cap = x_Cap_Destinazione
                    Destinatario_Comune = x_Stato_Destinazione
                    Destinatario_Provincia = ""

                    Piva_Dest = x_CodContatto_Destinazione
                    Codice_Fiscale_Dest = ""

                Case Else

                    Ricava_Piva_Codicefiscale(x_Id_Cf_Destinazione, _
                                              x_CodContatto_Destinazione, _
                                              x_CodiceFiscale_Destinazione, _
                                              0,
                                              Piva_Dest, _
                                              Codice_Fiscale_Dest, _
                                              Nothing)

                    Destinatario_Ind_Des = x_IndDes_Destinazione
                    Destinatario_Frz_Des = x_FrzDes_Destinazione
                    Destinatario_Cap = x_Cap_Destinazione
                    Destinatario_Comune = x_Comune_Destinazione
                    Destinatario_Provincia = "(" & x_Provincia_Destinazione & ")"

                    If x_Id_Cf_Destinazione = 0 Then
                        Piva_Dest = ""
                    End If

            End Select

        End If 'nel caso in cui la destinazione non sia l'azienda


        '=============================================================
        '-------------- INTESTAZIONE DOCUMENTO -------------------

        Dim Intestazione_RagSoc As String
        Dim Intestazione_PivaCF As String
        Dim Intestazione_Indirizzo1 As String
        Dim Intestazione_Indirizzo2 As String
        Dim Intestazione_RegImprese As String
        Dim Intestazione_CapitaleSociale As String
        Dim Intestazione_REA As String
        Dim Intestazione_TelFaxCell As String
        Dim Intestazione_SitoEmail As String
        Dim Intestazione_Social As String

        Select Case Lav_Cod

            Case LAVCOD_ORDINE_ACQUISTO
                'CHI EMETTE E' IL FORNITORE
                Intestazione_RagSoc = Fornitore_RagSoc
                Intestazione_PivaCF = Fornitore_Piva & Fornitore_CF
                Intestazione_Indirizzo1 = Fornitore_Indirizzo & " " & Fornitore_Frazione
                Intestazione_Indirizzo2 = Fornitore_Cap & " " & Fornitore_Comune & " " & Fornitore_Provincia
                Intestazione_RegImprese = ""
                Intestazione_CapitaleSociale = ""
                Intestazione_REA = ""
                Intestazione_TelFaxCell = ""
                Intestazione_SitoEmail = ""
                Intestazione_Social = ""

            Case Else
                'CHI EMETTE E' L'AZIENDA
                'Intestazione_RagSoc = x_RagSoc_Impresa
                'Intestazione_PivaCF = str_Piva_CF
                'Intestazione_Indirizzo1 = "Sede legale: " + Indirizzo_Impresa1
                'Intestazione_Indirizzo2 = Indirizzo_Impresa2
                'Intestazione_RegImprese = "Iscr. al n. " + x_RegImprese + " del Reg. Imprese di " + x_Provincia_RegImprese
                'Intestazione_CapitaleSociale = x_CapitaleSociale
                'Intestazione_REA = str_REA_ISO
                'Intestazione_TelFaxCell = TelFaxCell
                'Intestazione_SitoEmail = Sito_Email
                'Intestazione_Social = x_SocialNetwork
        End Select

        '===============================================================
        '----------------------- DATI FATTURA --------------------------


        If x_Cod_Contatto = Piva Then
            Parametro_TipoDocumento = "AUTOFATTURA"
        Else
            'NON E' UN AUTOFATTURA

            Dim normativaAssolta = "Assolve agli obblighi dell'Art. 3 del D.Lgs 198/2021 e s.m.i"

            Select Case Lav_Cod
                'PER LA SEZIONE DEL VETTORE, VERIFICO DOPO SE VISUALIZZARLO O MENO

                Case LAVCOD_FATTURA_EMESSA

                    Parametro_NoteArticolo62Conai = normativaAssolta & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Contributo CONAI assolto ove dovuto"

                    If x_Modalita = enum_ModalitaFattura.Fattura_AcquistiIntracom Then
                        Parametro_TipoDocumento = "FATTURA ESTERA (ACQUISTI INTRACOM.)"
                        Flag_StampaDettagliTrasporto = False
                        Flag_StampaDettagliEconomici = True
                    Else
                        Select Case x_Extra_Int
                            Case enum_FatturaTipo.Differita ' 0 ' differita
                                Parametro_TipoDocumento = "FATTURA DIFFERITA"
                                Flag_StampaDettagliTrasporto = False
                                Flag_StampaDettagliEconomici = True

                            Case enum_FatturaTipo.Immediata '1 'immediata
                                Parametro_TipoDocumento = "FATTURA IMMEDIATA"
                                Flag_StampaDettagliTrasporto = True
                                Flag_StampaDettagliEconomici = True
                        End Select
                    End If

                Case LAVCOD_FATTURA_PROFORMA
                    Parametro_TipoDocumento = "FATTURA PRO-FORMA"
                    Flag_StampaDettagliTrasporto = False
                    Flag_StampaDettagliEconomici = True

                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    Parametro_TipoDocumento = "NOTA DI ACCREDITO"
                    Flag_StampaDettagliTrasporto = False
                    Flag_StampaDettagliEconomici = True

                    Parametro_NoteArticolo62Conai = normativaAssolta & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Contributo CONAI assolto ove dovuto"

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    Parametro_TipoDocumento = "D.D.T.   (D.P.R. n.472 del 14/08/1996)"
                    Flag_StampaDettagliTrasporto = True
                    Flag_StampaDettagliEconomici = True

                    Parametro_NoteArticolo62Conai = normativaAssolta & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cessione di beni con prezzo da determinare ai sensi art.2 D.M. 15/11/1975"

                Case LAVCOD_BOLLA_EMESSA
                    Parametro_TipoDocumento = "D.D.T.   (D.P.R. n.472 del 14/08/1996)"
                    Flag_StampaDettagliTrasporto = True
                    Flag_StampaDettagliEconomici = False

                    Parametro_NoteArticolo62Conai = normativaAssolta & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cessione di beni con prezzo da determinare ai sensi art.2 D.M. 15/11/1975"

                Case LAVCOD_ORDINE_VENDITA
                    Parametro_TipoDocumento = "ORDINE DI VENDITA"
                    Flag_StampaDettagliTrasporto = True
                    Flag_StampaDettagliEconomici = True

                Case LAVCOD_ORDINE_ACQUISTO
                    Parametro_TipoDocumento = "ORDINE DI ACQUISTO"
                    Flag_StampaDettagliTrasporto = True
                    Flag_StampaDettagliEconomici = True

                Case LAVCOD_PREVENTIVO_VENDITA
                    Parametro_TipoDocumento = "PREVENTIVO"
                    Flag_StampaDettagliTrasporto = True
                    Flag_StampaDettagliEconomici = True
            End Select

        End If


        Parametro_NumDoc = x_Doc_Numero_Sin + CStr(x_Doc_Numero) + x_Doc_Numero_Des

        IdentificazioneDocumento += "n" & Parametro_NumDoc


        If x_Scadenza = AGRODATAFINE Or x_Scadenza = AGRODATAINIZIO Then
            Parametro_Scadenza = ""
        Else
            Parametro_Scadenza = x_Scadenza
        End If


        Parametro_DataDoc = CType(x_Data_Movimento, DateTime)
        IdentificazioneDocumento += "_d" & Format(x_Data_Movimento, "dd-MM-yyyy")

        ' salvo in sessione perchè mi servono nel salvataggio dell'allegato
        Session("Scadenza") = x_Scadenza
        Session("Data_Movimento") = x_Data_Movimento

        Select Case Lav_Cod
            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA
                'nei ddt le note sono salvate diversamente
                Parametro_Note = x_Mov_Desc_Contabile 'note

            Case Else
                'in mov_desc c'è la descrizione della fattura, non le note
                Parametro_Note = x_Extra_Str 'note
        End Select

        Parametro_Aspetto = x_Aspetto
        Parametro_Colli = x_Colli
        Parametro_Causale_Trasporto = x_Causale_Trasporto
        Parametro_Gest_Vettore = x_Gestione_Vettore

        'riciclo il campo telefono per la data e ora consegna
        If CDate(CDate(x_Ora).ToShortDateString) = AGRODATAFINE Or CDate(CDate(x_Ora).ToShortDateString) = AGRODATAINIZIO Then
            Parametro_DataOraRitiro = ""
        Else
            Parametro_DataOraRitiro = Format(CDate(x_Ora), "dd/MM/yyyy HH:mm")
        End If

        'Select Case x_Peso
        '    Case 0
        '        DrIntestazioneNew.Peso = "n.d."
        '    Case Else
        '        DrIntestazioneNew.Peso = x_Peso
        'End Select

        '=============================================================
        '------------------------ DESTINATARIO  ---------------------------

        Parametro_Destinatario_riga1 = Cliente_RagSoc
        Parametro_Destinatario_riga2 = Cliente_Indirizzo
        Parametro_Destinatario_riga3 = Cliente_Cap
        Parametro_Destinatario_riga3 &= " " & Cliente_Frazione
        Parametro_Destinatario_riga4 = Cliente_Comune
        Parametro_Destinatario_riga4 &= Cliente_Provincia
        If Cliente_Piva <> "" Then
            Parametro_Destinatario_riga5 = Cliente_Piva
        End If
        If Cliente_CF <> "" Then
            Parametro_Destinatario_riga5 &= " " & Cliente_CF
        End If

        '=============================================================
        '--------------------- DESTINAZIONE DIVERSA --------------------------

        Parametro_DestDiversa_riga1 = Destinatario_RagSoc
        Parametro_DestDiversa_riga2 = Destinatario_Ind_Des
        Parametro_DestDiversa_riga3 = Destinatario_Cap
        Parametro_DestDiversa_riga3 &= " " & Destinatario_Frz_Des

        Parametro_DestDiversa_riga4 = Destinatario_Comune
        Parametro_DestDiversa_riga4 &= " " & Destinatario_Provincia


        '=============================================================
        '------------------------ VETTORE --------------------------

        Select Case x_Cod_Vettore

            Case 0

                If x_Mezzo = 0 Then
                    Parametro_Mezzo = "MITTENTE"
                Else
                    Parametro_Mezzo = "DESTINATARIO"
                End If

            Case Else


                Parametro_Mezzo = "VETTORE"

                Parametro_Vettore_riga1 = x_RagSoc_Vettore
                ' x_CodContatto_Vettore

                Try

                    Leggi_Indirizzi(objParametri_Server, _
                                    x_CodContatto_Vettore, _
                                       x_Cod_Vettore, _
                                       x_Cod_IndirizzoVettore, _
                                       x_IndDes_Vettore, _
                                       x_FrzDes_Vettore, _
                                       x_Cap_Vettore, _
                                       x_Comune_Vettore, _
                                       x_Provincia_Vettore, _
                                       x_Stato_Vettore, _
                                       Nothing,
                                       Nothing, _
                                       "")

                Catch ex As Exception
                    Log_Errori += "- Lettura dell'indirizzo del vettore: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Select Case x_Id_Cf_Vettore
                    Case 2
                        Parametro_Vettore_riga2 = x_IndDes_Vettore
                        Parametro_Vettore_riga3 = x_Cap_Vettore + " " + x_FrzDes_Vettore
                        Parametro_Vettore_riga4 = " " + x_Stato_Vettore

                    Case Else
                        Parametro_Vettore_riga2 = x_IndDes_Vettore
                        Parametro_Vettore_riga3 = x_Cap_Vettore + " " + x_FrzDes_Vettore
                        Parametro_Vettore_riga4 = x_Comune_Vettore + " (" & x_Provincia_Vettore & ")"
                End Select

        End Select

        '=============================================================================
        '---------------------- DOCUMENTI COLLEGATI ----------------

        If Lav_Cod = LAVCOD_NOTA_ACCREDITO_EMESSA Then

            '-------------------------------------------------
            '------------- FATTURA AGGANCIATA ----------------
            '-------------------------------------------------

            Riferimento_DocAllegato = Leggi_RiferimentoFattura_NEW(objParametri_Server, Id_Agenda)

            Parametro_TipoDocumento &= " " & Riferimento_DocAllegato

        End If

        '=============================================================
        '--------------------- PAGAMENTI --------------------------

        Try
            Dim Pagamento_Desc As String = ""
            Dim NSbanca As String = ""
            Dim VSbanca As String = ""

            Leggi_Pagamenti(objParametri_Server, _
                           Piva, _
                           Lav_Cod, _
                           Id_Agenda, _
                           x_Id_Mov_Contabile, _
                           x_Num_Protocollo, _
                           x_RisorsaFinanziaria_Dare, _
                           x_RisorsaFinanziaria_Avere, _
                           Pagamento_Desc)

            Select Case Lav_Cod
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    If x_RisorsaFinanziaria_Dare <> "" Then
                        NSbanca = " - VS banca: " + x_RisorsaFinanziaria_Dare
                    End If
                    If x_RisorsaFinanziaria_Avere <> "" Then
                        VSbanca = " - NS banca: " + x_RisorsaFinanziaria_Avere
                    End If

                Case Else
                    If x_RisorsaFinanziaria_Dare <> "" Then
                        NSbanca = " - NS banca: " + x_RisorsaFinanziaria_Dare
                    End If
                    If x_RisorsaFinanziaria_Avere <> "" Then
                        VSbanca = " - VS banca: " + x_RisorsaFinanziaria_Avere
                    End If
            End Select

            If Parametro_Scadenza <> "" Then
                Parametro_Scadenza = "Scadenza " & Parametro_Scadenza & " - "
            End If
            Parametro_Pagamenti_riga1 = Parametro_Scadenza & Pagamento_Desc
            Parametro_Pagamenti_riga2 = NSbanca + VSbanca

        Catch ex As Exception
            Log_Errori += "- Lettura dei pagamenti della fattura: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        Dim Dt_Dettagli_Round As DataTable
        Dim Dt_Riepilogo_IVA As DataTable
        Dim Riepilogo_ImponibileLordo As Double = 0
        Dim Riepilogo_Variazioni As Double = 0
        Dim Riepilogo_ImponibileNetto As Double = 0
        Dim Riepilogo_Imposta As Double = 0
        Dim Riepilogo_Importo As Double = 0

        Try

            '====================================================================================
            '---------------- Prelevo le Informazioni sui dettagli ----------------------------

            Dim Flag_Raggruppa As Boolean
            'Dim Hash_Gruppo As Hashtable
            'Dim Valore, Chiave As String
            'Dim Qta_Gruppo, Importo_Gruppo, IVA_Gruppo, ImportoRiga_Gruppo As Double
            Dim Nome_Calibro As String

            'If x_ChkLayout_Join_Prodotti = 1 Then
            '    Flag_Raggruppa = True
            '    Hash_Gruppo = New Hashtable
            'Else
            '    Flag_Raggruppa = False
            'End If

            For i = 0 To Dt_Documento.Rows.Count - 1

                Try 'dettaglio

                    Try 'leggi dettaglio

                        'azzero ad ogni giro
                        numcolli_contenitori = 0
                        contenitore_cod = 0
                        descr_contenitore = ""
                        numcontenitori_imballi = 0
                        imballaggio_cod = 0
                        descr_imballo = ""
                        Flag_DDTallegati = False
                        ChkConfezione = 0
                        ChkContenitore = 0
                        ChkImballaggio = 0

                        Leggi_MovimentoDettaglio_DocContabile_GANDINI(objParametri_Server,
                                                                      objParametri_Utenti,
                                                                      Log_Errori,
                                                                      Dt_Documento.Rows(i),
                                                                      Piva,
                                                                      Lav_Cod,
                                                                      Flag_Raggruppa,
                                                                      x_Mov_Det_Des,
                                                                      x_Elem_Cod,
                                                                      x_Pro_Cod,
                                                                      x_Mat_Cod,
                                                                      x_Cod_Progetto,
                                                                      x_Fase_Cod,
                                                                      x_Lotto,
                                                                      x_Cal_Cod,
                                                                      x_Udm_Cod,
                                                                      x_Udm_Sim,
                                                                      x_Udm_Des,
                                                                      x_Udm_Cod_Extra,
                                                                      x_Udm_Sim_Extra,
                                                                      x_Udm_Des_Extra,
                                                                      x_Qta,
                                                                      x_Qta_Extra,
                                                                      x_Qta_Extra_Totale,
                                                                      x_Descrizione,
                                                                      x_Prezzo_Unitario,
                                                                      x_Prezzo_Unitario_Netto,
                                                                      x_Imponibile,
                                                                      x_Imponibile_Netto,
                                                                      x_Cod_IVA,
                                                                      x_Aliquota,
                                                                      x_IVA,
                                                                      x_ChkIVA_Manuale,
                                                                      x_Cod_IVAIndetraibile,
                                                                      x_Sconto_Perc,
                                                                      x_Sconto_Perc_2,
                                                                      x_Sconto_Testo,
                                                                      x_Sconto,
                                                                      x_Sconto_Modalita,
                                                                      x_Prezzo_Effettivo,
                                                                      x_Anno,
                                                                      x_Ric_Cod,
                                                                      x_Cod_Conto,
                                                                      x_Conto,
                                                                      x_Contabilizzato,
                                                                      x_Pendente,
                                                                      x_ChkLayOut_Hide,
                                                                      x_Tara,
                                                                      x_Extra_Str_Dettagli,
                                                                      x_Extra_Int_Dettagli,
                                                                      x_Extra_Date_Dettagli,
                                                                      x_Veg_Cod,
                                                                      x_Cul_Cod,
                                                                      Nome_Calibro,
                                                                      Riferimento_DocAllegato,
                                                                      numcolli_contenitori,
                                                                      contenitore_cod,
                                                                      descr_contenitore,
                                                                      numcontenitori_imballi,
                                                                      imballaggio_cod,
                                                                      descr_imballo,
                                                                      Flag_DDTallegati,
                                                                      Flag_ORDAllegati,
                                                                      ChkConfezione,
                                                                      ChkContenitore,
                                                                      ChkImballaggio,
                                                                      Dettaglio_Evasione,
                                                                      Dt_Dettagli_Round,
                                                                      Flag_ProdottoAgroAlimentare)

                        If Lav_Cod = LAVCOD_FATTURA_EMESSA And Flag_DDTallegati = True Then
                            'se almeno un dettaglio è collegato a ddt
                            'passa da questo if e quindi il tipo documento viene rinominato in FATTURA DA BOLLA
                            Parametro_TipoDocumento = "FATTURA DA BOLLA"
                        End If


                        'Leggi_MovimentoDettaglio_DocContabile_FreshFood(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                        '                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                        '                                ByRef Log_Errori As String, _
                        '                                ByVal Dr As DataRow, _
                        '                                ByVal Piva As String, _
                        '                                ByVal Lav_Cod As Integer, _
                        '                                ByVal Flag_Raggruppa As Boolean, _
                        '                                ByRef x_Mov_Det_Des As String, _
                        '                                ByRef x_Elem_Cod As Integer, _
                        '                                ByRef x_Pro_Cod As Integer, _
                        '                                ByRef x_Mat_Cod As Integer, _
                        '                                ByRef x_Cod_Progetto As Integer, _
                        '                                ByRef x_Fase_Cod As Integer, _
                        '                                ByRef x_Lotto As String, _
                        '                                ByRef x_Cal_Cod As Integer, _
                        '                                ByRef x_Udm_Cod As Integer, _
                        '                                ByRef x_Udm_Sim As String, _
                        '                                ByRef x_Udm_Des As String, _
                        '                                ByRef x_Udm_Cod_Extra As Integer, _
                        '                                ByRef x_Udm_Sim_Extra As String, _
                        '                                ByRef x_Udm_Des_Extra As String, _
                        '                                ByRef x_Qta As Double, _
                        '                                ByRef x_Qta_Extra As Double, _
                        '                                ByRef x_Qta_Extra_Totale As Double, _
                        '                                ByRef x_Descrizione As String, _
                        '                                ByRef x_Prezzo_Unitario As Double, _
                        '                                ByRef x_Prezzo_Unitario_Netto As Double, _
                        '                                ByRef x_Imponibile As Double, _
                        '                                ByRef x_Imponibile_Netto As Double, _
                        '                                ByRef x_Cod_IVA As Integer, _
                        '                                ByRef x_Aliquota As String, _
                        '                                ByRef x_IVA As Double, _
                        '                                ByRef x_ChkIVA_Manuale As Integer, _
                        '                                ByRef x_Cod_IVAIndetraibile As Integer, _
                        '                                ByRef x_Sconto_Perc As Double, _
                        '                                ByRef x_Sconto_Perc_2 As Double, _
                        '                                ByRef x_Sconto_Testo As String, _
                        '                                ByRef x_Sconto As Double, _
                        '                                ByRef x_Sconto_Modalita As Integer, _
                        '                                ByRef x_Prezzo_Effettivo As Double, _
                        '                                ByRef x_Anno As Integer, _
                        '                                ByRef x_Ric_Cod As Integer, _
                        '                                ByRef x_Cod_Conto As Integer, _
                        '                                ByRef x_Conto As String, _
                        '                                ByRef x_Contabilizzato As Integer, _
                        '                                ByRef x_Pendente As Integer, _
                        '                                ByRef x_ChkLayOut_Hide As Integer, _
                        '                                 ByRef x_Tara As Double, _
                        '                                 ByRef x_Extra_Str_Dettagli As String, _
                        '                                 ByRef x_Extra_Int_Dettagli As Integer, _
                        '                                 ByRef x_Extra_Date_Dettagli As Date, _
                        '                                 ByRef Veg_Cod As Integer, _
                        '                                 ByRef Cul_Cod As Integer, _
                        '                                 ByRef Nome_Calibro As String, _
                        '                                 ByRef Riferimento_DocAllegato As String, _
                        '                                 ByRef numcolli_contenitori As Integer, _
                        '                                 ByRef contenitore_cod As Integer, _
                        '                                 ByRef descr_contenitore As String, _
                        '                                 ByRef numcontenitori_imballi As Integer, _
                        '                                 ByRef imballaggio_cod As Integer, _
                        '                                ByRef descr_imballo As String, _
                        '                                ByRef Flag_DDTallegati As Boolean, _
                        '                                 Optional ByRef Dt_Dettagli_Round As DataTable = Nothing, _
                        '                                 Optional ByRef Flag_ProdottoAgroAlimentare As Boolean = False)

                        'se il prodotto è agroalimentare ed è il primo che trovo
                        '(metto il controllo almeno uno, così evito di sovrascriverlo nel caso in fattura ci siano anche articoli non agroalimentari)
                        If Flag_ProdottoAgroAlimentare = True And Flag_AlmenoUnoProdottoAgroAlimentare = False Then
                            Flag_AlmenoUnoProdottoAgroAlimentare = True
                        End If

                        'ad ogni dettaglio ricalcolo gli importi e aggiorno il dtiva
                        'Dt_Riepilogo_IVA, _
                        Dt_Riepilogo_IVA = objLanRound.FormAggiornaImporto(objParametri_Server, _
                                                                            Dt_Dettagli_Round, _
                                                                            Riepilogo_ImponibileLordo, _
                                                                            Riepilogo_Variazioni, _
                                                                            Riepilogo_ImponibileNetto, _
                                                                            Riepilogo_Imposta, _
                                                                            Riepilogo_Importo, _
                                                                            x_Edit_Importo)

                        Importo_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("importo_totale")

                        Dim pippo As Double
                        pippo = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("imponibile_netto")
                        pippo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(pippo)
                        If pippo <> x_Imponibile_Netto Then
                            Log_Errori += "Log debug: x_Imponibile_Netto = " & CStr(x_Imponibile_Netto) & " <> imponibile_netto di Dt_Dettagli_Round = " & CStr(pippo) & " " + vbCrLf + vbCrLf
                        End If

                    Catch ex As Exception
                        Log_Errori += "- Lettura dei dettagli della Fattura: " + vbCrLf + ex.Message + vbCrLf
                    End Try

                    'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                    If x_ChkLayOut_Hide <> 1 Then

                        Select Case x_Sconto_Modalita

                            Case enModalitaSconto.Percentuale
                                '   CASO NORMALE
                                Riga_Importo = Format(x_Imponibile_Netto, "#,###,##0.00")

                                Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)

                                Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                '=============================================

                            Case enModalitaSconto.Sconto_Merce
                                '   CASO SCONTO MERCE
                                Riga_Importo = "Sconto Merce"
                                Riga_Sconto = ""
                                Flag_Sconto_Merce = True
                                '=============================================

                            Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                                '   CASO CAMPIONI OMAGGIO
                                Riga_Importo = "Omaggio"
                                Riga_Sconto = ""
                                Flag_Campioni_Omaggio = True

                                Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                '=============================================

                            Case enModalitaSconto.Omaggio_ConRivalsaIva
                                '   CASO CAMPIONI OMAGGIO CON RIVALSA DELL'IVA
                                Riga_Importo = "Omaggio con rivalsa IVA"
                                Riga_Sconto = ""
                                Flag_Campioni_Omaggio_Rivalsa_Iva = True

                                Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                '=============================================

                            Case enModalitaSconto.Campioni_Gratuiti
                                '   CASO CAMPIONI GRATUITI
                                'non esiste x fattura, ma solo per ddt
                                'modifica del 06/11/2012
                                'gestisco il caso di stampa di fattura agganciata a ddt con campioni gratuiti
                                Riga_Importo = "Gratuito"
                                Flag_Campioni_Gratuiti = True
                                '=============================================

                        End Select

                        '===================================================================
                        'il dettaglio è visibile

                        'se si vuole visualizzare i dettagli così come sono
                        If x_ChkLayout_Join_Prodotti <> 1 Then

                            '------------------------------------
                            '------ VISUALIZZAZIONE NORMALE -----
                            '------------------------------------

                            'Gandini non vuole nella stampa il riepilogo delle casse (contenitori)
                            If ChkContenitore <> 1 Then

                                x_Descrizione += Riferimento_DocAllegato

                                Dr = DS_Gandini.DTGandiniDocContabile.NewRow

                                Dr.Descrizione = x_Descrizione

                                If numcolli_contenitori <> 0 Then
                                    Dr.Num_Colli = numcolli_contenitori
                                Else
                                    Dr.Num_Colli = ""
                                End If

                                Dr.Udm = x_Udm_Sim

                                Dr.Qta = x_Qta

                                Select Case Lav_Cod

                                    Case LAVCOD_BOLLA_EMESSA

                                        If x_Prezzo_Unitario <> 0 Then
                                            Dr.Prezzo_Unitario = Format(x_Prezzo_Unitario, "#,###,##0.00##")
                                        Else
                                            Dr.Prezzo_Unitario = ""
                                        End If

                                        Dr.Importo = ""
                                        Dr.Aliquota_Iva = ""

                                    Case Else

                                        If x_Elem_Cod = BENI_CONFEZ_VEGETALE Then

                                            Dr.Prezzo_Unitario = ""
                                            Dr.Importo = ""
                                            Dr.Aliquota_Iva = ""

                                        Else
                                            'tutti gli altri prodotti
                                            Dr.Prezzo_Unitario = Format(x_Prezzo_Unitario, "#,###,##0.00##")

                                            Dr.Importo = Riga_Importo

                                            Dr.Aliquota_Iva = x_Aliquota
                                        End If

                                End Select

                                DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)

                            End If 'ChkContenitore

                        Else 'bisogna raggruppare i dettagli

                            ''------------------------------------
                            ''------ RAGGRUPPAMENTO DETTAGLI -----
                            ''------------------------------------

                            ''raggruppo per specie- varietà - calibro (non posso usare il codice perchè è un progressivo) - unità misura - prezzo - sconto - cod iva
                            'Chiave = CStr(x_Veg_Cod) + "|" + CStr(x_Cul_Cod) + "|" + Nome_Calibro + "|" + CStr(x_Udm_Cod) + "|" + CStr(x_Prezzo_Unitario) + "|" + CStr(x_Sconto_Perc) + "|" + CStr(x_Cod_IVA)

                            ''se non è già presente
                            'If Not Hash_Gruppo.Contains(Chiave) Then

                            '    'valore composto da 9 campi (da 0 a 8)
                            '    Valore = x_Descrizione + "|" + x_Udm_Sim + "|" + CStr(x_Qta) + "|" + _
                            '            CStr(x_Prezzo_Unitario) + "|" + CStr(x_Sconto_Perc) + "|" + _
                            '            CStr(x_Aliquota) + "|" + CStr(x_IVA) + "|" + CStr(x_Imponibile_Netto) + _
                            '            "|" + Riferimento_DocAllegato + "|" + CStr(Importo_Dettaglio)

                            '    'inserisco il dettaglio
                            '    Hash_Gruppo.Add(Chiave, Valore)

                            'Else
                            '    'il dettaglio è già presente
                            '    'devo incrementare la quantità

                            '    Riferimento_DocAllegato_Prec = CStr(Hash_Gruppo.Item(Chiave)).Split("|")(8)

                            '    'prelevo la quantità del dettaglio al momento salvata
                            '    Qta_Gruppo = CDbl(CStr(Hash_Gruppo.Item(Chiave)).Split("|")(2))

                            '    'aggiungo la qta del dettaglio ripetuto
                            '    Qta_Gruppo += x_Qta

                            '    'prelevo l'importo del dettaglio al momento salvato
                            '    Importo_Gruppo = CDbl(CStr(Hash_Gruppo.Item(Chiave)).Split("|")(7))
                            '    Importo_Gruppo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Importo_Gruppo)
                            '    'aggiungo l'imponibile del dettaglio ripetuto
                            '    Importo_Gruppo += x_Imponibile_Netto
                            '    Importo_Gruppo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Importo_Gruppo)

                            '    'prelevo l'importo totale del dettaglio 
                            '    ImportoRiga_Gruppo = CDbl(CStr(Hash_Gruppo.Item(Chiave)).Split("|")(9))
                            '    ImportoRiga_Gruppo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(ImportoRiga_Gruppo)
                            '    ImportoRiga_Gruppo += Importo_Dettaglio
                            '    ImportoRiga_Gruppo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(ImportoRiga_Gruppo)

                            '    'questo tanto non serve
                            '    IVA_Gruppo = CDbl(CStr(Hash_Gruppo.Item(Chiave)).Split("|")(6))
                            '    IVA_Gruppo += AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(x_IVA)

                            '    Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)

                            '    'preparo il nuovo valore
                            '    'valore composto da 9 campi (da 0 a 8)
                            '    Valore = x_Descrizione + "|" + x_Udm_Sim + "|" + CStr(Qta_Gruppo) + "|" + _
                            '       CStr(x_Prezzo_Unitario) + "|" + CStr(Riga_Sconto) + "|" + _
                            '       CStr(x_Aliquota) + "|" + CStr(IVA_Gruppo) + "|" + CStr(Importo_Gruppo) + _
                            '       "|" + Riferimento_DocAllegato_Prec + Riferimento_DocAllegato + "|" + CStr(ImportoRiga_Gruppo)

                            '    'aggiorno il valore
                            '    Hash_Gruppo.Item(Chiave) = Valore

                            'End If

                        End If 'raggruppamento dettagli

                    End If 'x_ChkLayOut_Hide

                Catch ex As Exception
                    Log_Errori += "- Errore dettaglio " + CStr(i + 1) + ": " + vbCrLf + ex.Message + vbCrLf
                End Try

            Next 'dettagli

            '===================================================================

            Try

                ''se impostato il raggruppamento
                'If x_ChkLayout_Join_Prodotti = 1 Then

                '    Dim Key As Object
                '    i = 0

                '    For Each Key In Hash_Gruppo.Keys

                '        DrDescrizioneNew = DS.Descrizione.NewDescrizioneRow

                '        DrDescrizioneNew.Contatore = i

                '        DrDescrizioneNew.SuperPiva = Piva

                '        DrDescrizioneNew.Descrizione = CStr(Hash_Gruppo.Item(Key)).Split("|")(0) + " " + CStr(Hash_Gruppo.Item(Key)).Split("|")(8)

                '        DrDescrizioneNew.Udm_Des = CStr(Hash_Gruppo.Item(Key)).Split("|")(1)

                '        DrDescrizioneNew.Qta = CStr(Hash_Gruppo.Item(Key)).Split("|")(2) 'somma delle qta

                '        DrDescrizioneNew.Prezzo = Format(CDbl(CStr(Hash_Gruppo.Item(Key)).Split("|")(3)), "#,###,##0.00##")

                '        DrDescrizioneNew.Sconto = CStr(Hash_Gruppo.Item(Key)).Split("|")(4)

                '        DrDescrizioneNew.Iva = CStr(Hash_Gruppo.Item(Key)).Split("|")(5)

                '        DrDescrizioneNew.IvaImposta = CStr(Hash_Gruppo.Item(Key)).Split("|")(6) 'non serve

                '        DrDescrizioneNew.Importo = Format(CDbl(CStr(Hash_Gruppo.Item(Key)).Split("|")(7)), "#,###,##0.00##")

                '        DrDescrizioneNew.Extra_Str_1 = Format(CDbl(CStr(Hash_Gruppo.Item(Key)).Split("|")(9)), "#,###,##0.00##")

                '        DS.Descrizione.Rows.Add(DrDescrizioneNew)

                '        i += 1

                '    Next

                'End If

            Catch ex As Exception
                Log_Errori += "- Gestione raggruppamento dettagli: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '======================================================================

            '===================================================================

            'NOTE AGGIUNTIVE NEI DETTAGLI
            Try

                If Str_NoteIntegrative1 <> "" Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = Str_NoteIntegrative1
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If
                If Str_NoteIntegrative2 <> "" Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = Str_NoteIntegrative2
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If
                If Str_NoteIntegrative3 <> "" Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = Str_NoteIntegrative3
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If

            Catch ex As Exception
                Log_Errori += "- Visualizzazione note aggiuntive nei dettagli: " + vbCrLf + ex.Message + vbCrLf
            End Try
            '-------------------------------------

            Try

                '   CASO CAMPIONI OMAGGIO
                If Flag_Campioni_Omaggio = True Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = vbCrLf + vbCrLf + "Cessione gratuita ai sensi dell'art.2, comma 2 del D.P.R. n.633/1972 " + _
                    "senza esercizio di rivalsa dell'IVA, ai sensi dell'art.18, comma 3, del D.P.R. n.633/1972"
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If

                '   CASO CAMPIONI OMAGGIO CON RIVALSA IVA
                If Flag_Campioni_Omaggio_Rivalsa_Iva = True Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = vbCrLf + vbCrLf + "Cessione gratuita ai sensi dell'art. 2, comma 2, n. 4, del D.P.R. n. 633/1972, con rivalsa dell'Iva."
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If

                'aggiunto in data 06/11/2012:
                '   CASO CAMPIONI GRATUITI 
                If Flag_Campioni_Gratuiti = True Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = vbCrLf + vbCrLf + "Cessione gratuita ai sensi dell'art.4 del D.P.R. n.633/1972 " + _
                    "senza esercizio della rivalsa dell'IVA, ai sensi dell'art.18, comma 3, del D.P.R. n.633/1972"
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If

                '   CASO SCONTO MERCE
                If Flag_Sconto_Merce = True Then
                    Dr = DS_Gandini.DTGandiniDocContabile.NewRow
                    Dr.Descrizione = vbCrLf + vbCrLf + "Sconto merce senza esercizio di rivalsa dell'IVA ai sensi dell'art.15 del D.P.R. n.633/1972"
                    Dr.Udm = ""
                    Dr.Qta = ""
                    Dr.Prezzo_Unitario = ""
                    Dr.Importo = ""
                    Dr.Aliquota_Iva = ""
                    DS_Gandini.DTGandiniDocContabile.Rows.Add(Dr)
                End If

            Catch ex As Exception
                Log_Errori += "- Visualizzazione della nota sugli omaggi: " + vbCrLf + ex.Message + vbCrLf
            End Try


        Catch ex As Exception
            Log_Errori += "- Visualizzazione globale dei dettagli della Fattura: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '=============================================================
        '---------- RIEPILOGO DOCUMENTO ----------

        Try

            ''IMPONIBILE LORDO
            'DrIntestazioneNew.Imponibile = Format(Riepilogo_ImponibileLordo, "#,###,##0.00")

            ''VARIAZIONI
            'DrIntestazioneNew.Sconti = Format(Riepilogo_Variazioni, "#,###,##0.00")

            'IMPONIBILE NETTO
            Parametro_Riepilogo_ImponibileNetto = Format(Riepilogo_ImponibileNetto, "#,###,##0.00")

            'IMPOSTA
            Parametro_Riepilogo_Imposta = Format(Riepilogo_Imposta, "#,###,##0.00")

            'TOTALE FATTURA
            Parametro_Riepilogo_Importo = Format(Riepilogo_Importo, "#,###,##0.00")

            'If Riepilogo_Importo <> x_Num_Protocollo Then
            '    Log_Errori += "Log debug: Riepilogo_Importo = " & CStr(Riepilogo_Importo) & " <> x_Num_Protocollo = " & CStr(x_Num_Protocollo) & " " + vbCrLf + vbCrLf
            'End If

            '=============================================================
            '      REPORT IVA
            '=============================================================

            Try
                If Not IsNothing(Dt_Riepilogo_IVA) Then
                    ' Dim DR_Iva As DS_Iva.DS_IvaRow
                    Dim str_iva, str_imponibile As String
                    For i = 0 To Dt_Riepilogo_IVA.Rows.Count - 1

                        str_imponibile = Format(Dt_Riepilogo_IVA.Rows(i).Item("imponibile_netto"), "#,###,##0.00")
                        str_iva = Dt_Riepilogo_IVA.Rows(i).Item("aliquota_des") & "  " & Format(Dt_Riepilogo_IVA.Rows(i).Item("iva"), "#,###,##0.00")

                        Select Case i
                            Case 0
                                Parametro_Imponibile_riga1 = str_imponibile
                                Parametro_Iva_riga1 = str_iva
                            Case 1
                                Parametro_Imponibile_riga2 = str_imponibile
                                Parametro_Iva_riga2 = str_iva
                            Case Else
                                Log_Errori += "- Riepilogo IVA, c'è un dettaglio che non viene stampato: " & str_imponibile & " " & str_iva + vbCrLf
                        End Select

                        'nuova riga
                        'DR_Iva = DSIva.DS_Iva.NewDS_IvaRow
                        'DR_Iva.Cod_Iva = Dt_Riepilogo_IVA.Rows(i).Item("cod_iva") 'cod_iva
                        'DR_Iva.Cod_Iva_2 = Dt_Riepilogo_IVA.Rows(i).Item("aliquota_des") 'aliquota
                        'DR_Iva.Imponibile = Format(Dt_Riepilogo_IVA.Rows(i).Item("imponibile_netto"), "#,###,##0.00")
                        'DR_Iva.Imponibile2 = Format(Dt_Riepilogo_IVA.Rows(i).Item("imponibile_netto"), "#,###,##0.00")
                        'DR_Iva.Imposta = Format(Dt_Riepilogo_IVA.Rows(i).Item("iva"), "#,###,##0.00")
                        'DR_Iva.Imposta2 = Format(Dt_Riepilogo_IVA.Rows(i).Item("iva"), "#,###,##0.00")
                        ''aggiungo riga
                        'DSIva.DS_Iva.Rows.Add(DR_Iva)
                    Next
                Else
                    Log_Errori += "- Dt_Riepilogo_IVA nothing! " + vbCrLf + vbCrLf
                End If

            Catch ex As Exception
                Log_Errori += "- Riepilogo IVA: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '====================================================================================

        Catch ex As Exception
            Log_Errori += "- Visualizzazione dei riepiloghi della Fattura: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il dataset sul report
            rptGandiniDoc.SetDataSource(DS_Gandini)
        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report CON layout: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try


            rptGandiniDoc.SetParameterValue("Destinatario_riga1", Parametro_Destinatario_riga1)
            rptGandiniDoc.SetParameterValue("Destinatario_riga2", Parametro_Destinatario_riga2)
            rptGandiniDoc.SetParameterValue("Destinatario_riga3", Parametro_Destinatario_riga3)
            rptGandiniDoc.SetParameterValue("Destinatario_riga4", Parametro_Destinatario_riga4)
            rptGandiniDoc.SetParameterValue("Destinatario_riga5", Parametro_Destinatario_riga5)

            rptGandiniDoc.SetParameterValue("DestDiversa_riga1", Parametro_DestDiversa_riga1)
            rptGandiniDoc.SetParameterValue("DestDiversa_riga2", Parametro_DestDiversa_riga2)
            rptGandiniDoc.SetParameterValue("DestDiversa_riga3", Parametro_DestDiversa_riga3)
            rptGandiniDoc.SetParameterValue("DestDiversa_riga4", Parametro_DestDiversa_riga4)

            rptGandiniDoc.SetParameterValue("TipoDocumento", Parametro_TipoDocumento)
            rptGandiniDoc.SetParameterValue("NumDoc", Parametro_NumDoc)
            'rptGandiniDoc.SetParameterValue("Scadenza", Parametro_Scadenza)
            rptGandiniDoc.SetParameterValue("DataDoc", Parametro_DataDoc)
            rptGandiniDoc.SetParameterValue("Note", Parametro_Note)

            If Flag_StampaDettagliTrasporto = True Then
                rptGandiniDoc.SetParameterValue("Aspetto", Parametro_Aspetto)
                rptGandiniDoc.SetParameterValue("Colli", Parametro_Colli)
                rptGandiniDoc.SetParameterValue("Causale_Trasporto", Parametro_Causale_Trasporto)
                rptGandiniDoc.SetParameterValue("Gest_Vettore", Parametro_Gest_Vettore)
                rptGandiniDoc.SetParameterValue("DataOraRitiro", Parametro_DataOraRitiro)
                rptGandiniDoc.SetParameterValue("Mezzo", Parametro_Mezzo)
                rptGandiniDoc.SetParameterValue("Vettore_riga1", Parametro_Vettore_riga1)
                rptGandiniDoc.SetParameterValue("Vettore_riga2", Parametro_Vettore_riga2)
                rptGandiniDoc.SetParameterValue("Vettore_riga3", Parametro_Vettore_riga3)
                rptGandiniDoc.SetParameterValue("Vettore_riga4", Parametro_Vettore_riga4)
            Else
                rptGandiniDoc.SetParameterValue("Aspetto", "")
                rptGandiniDoc.SetParameterValue("Colli", "")
                rptGandiniDoc.SetParameterValue("Causale_Trasporto", "")
                rptGandiniDoc.SetParameterValue("Gest_Vettore", "")
                rptGandiniDoc.SetParameterValue("DataOraRitiro", "")
                rptGandiniDoc.SetParameterValue("Mezzo", "")
                rptGandiniDoc.SetParameterValue("Vettore_riga1", "")
                rptGandiniDoc.SetParameterValue("Vettore_riga2", "")
                rptGandiniDoc.SetParameterValue("Vettore_riga3", "")
                rptGandiniDoc.SetParameterValue("Vettore_riga4", "")
            End If

            If Flag_StampaDettagliEconomici = True Then
                rptGandiniDoc.SetParameterValue("Pagamenti_riga1", Parametro_Pagamenti_riga1)
                rptGandiniDoc.SetParameterValue("Pagamenti_riga2", Parametro_Pagamenti_riga2)

                rptGandiniDoc.SetParameterValue("Imponibile_riga1", Parametro_Imponibile_riga1)
                rptGandiniDoc.SetParameterValue("Iva_riga1", Parametro_Iva_riga1)
                rptGandiniDoc.SetParameterValue("Imponibile_riga2", Parametro_Imponibile_riga2)
                rptGandiniDoc.SetParameterValue("Iva_riga2", Parametro_Iva_riga2)

                rptGandiniDoc.SetParameterValue("Riepilogo_ImponibileNetto", Parametro_Riepilogo_ImponibileNetto)
                rptGandiniDoc.SetParameterValue("Riepilogo_Imposta", Parametro_Riepilogo_Imposta)
                rptGandiniDoc.SetParameterValue("Riepilogo_Importo", Parametro_Riepilogo_Importo)

            Else
                rptGandiniDoc.SetParameterValue("Pagamenti_riga1", "")
                rptGandiniDoc.SetParameterValue("Pagamenti_riga2", "")

                rptGandiniDoc.SetParameterValue("Imponibile_riga1", "")
                rptGandiniDoc.SetParameterValue("Iva_riga1", "")
                rptGandiniDoc.SetParameterValue("Imponibile_riga2", "")
                rptGandiniDoc.SetParameterValue("Iva_riga2", "")

                rptGandiniDoc.SetParameterValue("Riepilogo_ImponibileNetto", "")
                rptGandiniDoc.SetParameterValue("Riepilogo_Imposta", "")
                rptGandiniDoc.SetParameterValue("Riepilogo_Importo", "")
            End If

            rptGandiniDoc.SetParameterValue("NoteArticolo62Conai", Parametro_NoteArticolo62Conai)

        Catch ex As Exception
            Log_Errori += "- impostazione parametri sul report SENZA layout: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            'imposto il dataset sul report
            rptGandiniDoc_conLayout.SetDataSource(DS_Gandini)
        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report CON layout: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try


            rptGandiniDoc_conLayout.SetParameterValue("Destinatario_riga1", Parametro_Destinatario_riga1)
            rptGandiniDoc_conLayout.SetParameterValue("Destinatario_riga2", Parametro_Destinatario_riga2)
            rptGandiniDoc_conLayout.SetParameterValue("Destinatario_riga3", Parametro_Destinatario_riga3)
            rptGandiniDoc_conLayout.SetParameterValue("Destinatario_riga4", Parametro_Destinatario_riga4)
            rptGandiniDoc_conLayout.SetParameterValue("Destinatario_riga5", Parametro_Destinatario_riga5)

            rptGandiniDoc_conLayout.SetParameterValue("DestDiversa_riga1", Parametro_DestDiversa_riga1)
            rptGandiniDoc_conLayout.SetParameterValue("DestDiversa_riga2", Parametro_DestDiversa_riga2)
            rptGandiniDoc_conLayout.SetParameterValue("DestDiversa_riga3", Parametro_DestDiversa_riga3)
            rptGandiniDoc_conLayout.SetParameterValue("DestDiversa_riga4", Parametro_DestDiversa_riga4)

            rptGandiniDoc_conLayout.SetParameterValue("TipoDocumento", Parametro_TipoDocumento)
            rptGandiniDoc_conLayout.SetParameterValue("NumDoc", Parametro_NumDoc)
            'rptGandiniDoc_conLayout.SetParameterValue("Scadenza", Parametro_Scadenza)
            rptGandiniDoc_conLayout.SetParameterValue("DataDoc", Parametro_DataDoc)
            rptGandiniDoc_conLayout.SetParameterValue("Note", Parametro_Note)

            If Flag_StampaDettagliTrasporto = True Then
                rptGandiniDoc_conLayout.SetParameterValue("Aspetto", Parametro_Aspetto)
                rptGandiniDoc_conLayout.SetParameterValue("Colli", Parametro_Colli)
                rptGandiniDoc_conLayout.SetParameterValue("Causale_Trasporto", Parametro_Causale_Trasporto)
                rptGandiniDoc_conLayout.SetParameterValue("Gest_Vettore", Parametro_Gest_Vettore)
                rptGandiniDoc_conLayout.SetParameterValue("DataOraRitiro", Parametro_DataOraRitiro)
                rptGandiniDoc_conLayout.SetParameterValue("Mezzo", Parametro_Mezzo)
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga1", Parametro_Vettore_riga1)
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga2", Parametro_Vettore_riga2)
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga3", Parametro_Vettore_riga3)
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga4", Parametro_Vettore_riga4)
            Else
                rptGandiniDoc_conLayout.SetParameterValue("Aspetto", "")
                rptGandiniDoc_conLayout.SetParameterValue("Colli", "")
                rptGandiniDoc_conLayout.SetParameterValue("Causale_Trasporto", "")
                rptGandiniDoc_conLayout.SetParameterValue("Gest_Vettore", "")
                rptGandiniDoc_conLayout.SetParameterValue("DataOraRitiro", "")
                rptGandiniDoc_conLayout.SetParameterValue("Mezzo", "")
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga1", "")
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga2", "")
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga3", "")
                rptGandiniDoc_conLayout.SetParameterValue("Vettore_riga4", "")
            End If

            If Flag_StampaDettagliEconomici = True Then
                rptGandiniDoc_conLayout.SetParameterValue("Pagamenti_riga1", Parametro_Pagamenti_riga1)
                rptGandiniDoc_conLayout.SetParameterValue("Pagamenti_riga2", Parametro_Pagamenti_riga2)

                rptGandiniDoc_conLayout.SetParameterValue("Imponibile_riga1", Parametro_Imponibile_riga1)
                rptGandiniDoc_conLayout.SetParameterValue("Iva_riga1", Parametro_Iva_riga1)
                rptGandiniDoc_conLayout.SetParameterValue("Imponibile_riga2", Parametro_Imponibile_riga2)
                rptGandiniDoc_conLayout.SetParameterValue("Iva_riga2", Parametro_Iva_riga2)

                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_ImponibileNetto", Parametro_Riepilogo_ImponibileNetto)
                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_Imposta", Parametro_Riepilogo_Imposta)
                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_Importo", Parametro_Riepilogo_Importo)

            Else
                rptGandiniDoc_conLayout.SetParameterValue("Pagamenti_riga1", "")
                rptGandiniDoc_conLayout.SetParameterValue("Pagamenti_riga2", "")

                rptGandiniDoc_conLayout.SetParameterValue("Imponibile_riga1", "")
                rptGandiniDoc_conLayout.SetParameterValue("Iva_riga1", "")
                rptGandiniDoc_conLayout.SetParameterValue("Imponibile_riga2", "")
                rptGandiniDoc_conLayout.SetParameterValue("Iva_riga2", "")

                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_ImponibileNetto", "")
                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_Imposta", "")
                rptGandiniDoc_conLayout.SetParameterValue("Riepilogo_Importo", "")
            End If

            rptGandiniDoc_conLayout.SetParameterValue("NoteArticolo62Conai", Parametro_NoteArticolo62Conai)


        Catch ex As Exception
            Log_Errori += "- impostazione parametri sul report CON layout: " + vbCrLf + ex.Message + vbCrLf
        End Try



    End Sub




End Class