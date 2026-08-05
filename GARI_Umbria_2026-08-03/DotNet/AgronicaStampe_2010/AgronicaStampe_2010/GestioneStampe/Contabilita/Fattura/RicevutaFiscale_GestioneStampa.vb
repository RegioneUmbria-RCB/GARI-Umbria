Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility.FileSystemHelper

Public Class RicevutaFiscale_GestioneStampa

    '#####################################################################################################
    Public Sub Stampa_RicevutaFiscale(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef DS As DS_RicevutaFiscaleA5,
                                      ByRef logErrori As String,
                                      ByVal tipoRicevuta As enum_TipoStampaRicevutaFiscale,
                                      ByRef IdentificazioneDocumento As String,
                                      ByVal Piva As String,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Lav_Cod As Integer,
                                      ByVal progressivoGias As Integer,
                                      ByVal NUM_DETTAGLI_GESTITI As Integer,
                                      ByRef Flag_SuperatoNumDettagliGestiti As Boolean,
                                      ByRef x_Data_Movimento As Date,
                                      ByVal Flag_StampaACapoAuto As Boolean,
                                      ByVal Flag_NuoviArrotondamenti As Boolean,
                                      ByRef objConfigStampe As AgronicaCoreStampeDAL.ConfigurazioneStampe)

        Dim Dr_Intestazione As DS_RicevutaFiscaleA5.IntestazioneRow
        Dim Dr_Dettagli As DS_RicevutaFiscaleA5.DettagliRow
        Dim msgErrore As String = ""
        Flag_SuperatoNumDettagliGestiti = False

        Try

            Dr_Intestazione = DS.Intestazione.NewIntestazioneRow

            '############################################################################################
            '############################ INTESTAZIONE DELLA RICEVUTA FISCALE ###########################
            '############################################################################################

            '------------------
            'Impresa

            Dim Int_Riga_1 As String
            Dim Int_Riga_2 As String
            Dim Int_Riga_3 As String
            Dim Int_Riga_4 As String
            Dim Int_Riga_5 As String

            Dim Intestazione_Riga3 As String = ""
            Dim Intestazione_Riga4 As String = ""
            Dim Intestazione_Riga5 As String = ""
            Dim Intestazione_Riga6 As String = ""
            Dim Intestazione_Riga7 As String = ""
            Dim Intestazione_Riga8 As String = ""
            Dim Intestazione_Riga9 As String = ""
            Dim Intestazione_Riga10 As String = ""
            Dim Intestazione_Riga11 As String = ""
            Dim Intestazione_Riga12 As String = ""
            Dim Intestazione_Riga13 As String = ""
            Dim Intestazione_Riga14 As String = ""
            Dim Intestazione_Riga15 As String = ""

            Dim x_RagSoc_Impresa As String = ""
            Dim x_CodContatto_Impresa As String = ""
            Dim x_CodiceFiscale_Impresa As String = ""
            Dim x_IndDes_Impresa As String = ""
            Dim x_FrzDes_Impresa As String = ""
            Dim x_Cap_Impresa As String = ""
            Dim x_Comune_Impresa As String = ""
            Dim x_Provincia_Impresa As String = ""
            Dim x_Numero As String = ""
            Dim x_Telefono As String = ""
            Dim x_Fax As String = ""
            Dim x_Cell As String = ""
            Dim x_Email As String = ""
            Dim x_SitoWeb As String = ""
            Dim x_Stato_Impresa As String = ""

            Dim Flag_StampaNumeroVasca As Boolean = False

            Dim numcolli_contenitori As Integer = 0
            Dim contenitore_cod As Integer = 0
            Dim descr_contenitore As String = ""
            Dim numcontenitori_imballi As Integer = 0
            Dim imballaggio_cod As Integer = 0
            Dim descr_imballo As String = ""
            Dim Flag_DDTallegati As Boolean = False
            Dim ChkConfezione As Integer = 0
            Dim ChkContenitore As Integer = 0
            Dim ChkImballaggio As Integer = 0

            Dim x_Livello_Prezzo As Integer = 0
            '------------------------------

            Try

                'verifica opzione stampa numero e capacità vasca
                Dim Flag_StampaCapacitaVasca As Boolean = False
                Dim OptGestVisualNumVascaRegImbott As Integer = 0
                Dim Flag_GestioneRegistroVinificazione As Integer = 0

                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                objUtenti.LeggiOpzioni_RegistriCantina(2,
                                                       objParametriUtenti,
                                                       Flag_StampaNumeroVasca,
                                                       Flag_StampaCapacitaVasca,
                                                       OptGestVisualNumVascaRegImbott,
                                                       Flag_GestioneRegistroVinificazione,
                                                       Nothing)


            Catch ex As Exception
                logErrori &= "- Lettura opzioni registri di cantina: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '############################################################################################
            '########################## Lettura della Ricevuta Fiscale ##################################
            '############################################################################################

            Dim Dt_Documento As DataTable = Nothing
            Dim x_Id_Mov_Contabile As Integer = 0
            Dim x_Mov_Desc_Contabile As String = ""
            'Dim x_Data_Movimento As Date = #1/1/1900#
            Dim x_Ora As String = Date.Now.ToString
            Dim x_Scadenza As Date = #12/31/2100#
            Dim x_Scadenza_Extra As Date = #12/31/2100#
            Dim x_Doc_Numero_Sin As String = ""
            Dim x_Doc_Numero As Integer = 0
            Dim x_Doc_Numero_Des As String = ""
            Dim x_Progr_Protocollo As Integer = 0
            Dim x_Progr_Registrazione As Integer = 0
            Dim x_Data_Registrazione As Date = #1/1/1900#
            Dim x_Num_Protocollo As Decimal = 0
            Dim x_Colli As Integer = 0
            Dim x_Peso As Decimal = 0
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
            Dim x_ChkFittizio_Vettore As Integer
            Dim x_NumReg_Vettore As String = ""
            Dim x_TargaMezzoVettore As String = ""
            Dim x_Agente_Cod As Integer = 0
            Dim x_Natura_Beni As String = ""
            Dim x_Modalita As Integer = 0
            Dim x_Tara_Veicolo As Decimal = 0
            Dim x_Tara_Imballi As Decimal = 0
            Dim x_Tipo_Peso As Integer = 0
            Dim Peso_Lordo As Decimal = 0
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

            Dim x_Ind_Des As String = ""
            Dim x_Frz_Des As String = ""
            Dim x_Cap As String = ""
            Dim x_Comune As String = ""
            Dim x_Provincia As String = ""
            Dim x_Stato As String = ""
            Dim Flag_StatoMembro As Boolean
            Dim Cliente_Indirizzo As String = ""

            Dim Dettaglio_Evasione As String = ""


            Try

                'Leggo i dati relativi al nodo Movimento (contabile)
                Leggi_Movimenti_DocContabile(objParametriServer,
                                             msgErrore,
                                             Piva,
                                             Id_Agenda,
                                             enum_CodificaStampe.RicevuteFiscali,
                                             Dt_Documento,
                                             x_Id_Mov_Contabile,
                                             x_RagSoc_Impresa,
                                             x_CodiceFiscale_Impresa,
                                             x_Mov_Desc_Contabile,
                                             x_Data_Movimento,
                                             x_Ora,
                                             x_Scadenza,
                                             x_Scadenza_Extra,
                                             x_Doc_Numero_Sin,
                                             x_Doc_Numero,
                                             x_Doc_Numero_Des,
                                             x_Progr_Protocollo,
                                             x_Progr_Registrazione,
                                             x_Data_Registrazione,
                                             x_Num_Protocollo,
                                             x_Colli,
                                             x_Peso,
                                             x_Aspetto,
                                             x_Causale_Trasporto,
                                             x_Edit_Importo,
                                             x_Cod_RisUm,
                                             x_Cod_Contatto,
                                             x_Rag_Soc,
                                             x_Codice_Fiscale,
                                             0,
                                             x_Cod_IndirizzoRisUm,
                                             x_Id_Cf_Cliente,
                                             x_Cod_Destinazione,
                                             x_CodContatto_Destinazione,
                                             x_RagSoc_Destinazione,
                                             x_CodiceFiscale_Destinazione,
                                                0,
                                             x_Cod_IndirizzoDestinazione,
                                             x_Id_Cf_Destinazione,
                                             x_Mezzo,
                                             x_Cod_Vettore,
                                             x_Cod_IndirizzoVettore,
                                             x_Id_Cf_Vettore,
                                             x_CodContatto_Vettore,
                                             x_RagSoc_Vettore,
                                             x_CodiceFiscale_Vettore,
                                             x_ChkFittizio_Vettore,
                                             x_NumReg_Vettore,
                                             x_TargaMezzoVettore,
                                             Nothing,
                                             x_Agente_Cod,
                                             x_Natura_Beni,
                                             x_Modalita,
                                             x_Tara_Veicolo,
                                             x_Tara_Imballi,
                                             x_Tipo_Peso,
                                             x_Username_Note,
                                             x_Extra_Str,
                                             x_Extra_Int,
                                             x_Extra_Date,
                                             x_ChkLayout_Bypass_Fatturato,
                                             x_ChkLayout_Join_Prodotti,
                                             x_ChkLayOut_Peso,
                                             x_ChkLayOut_Prezzo,
                                             x_Gestione_Vettore,
                                             Peso_Lordo,
                                             x_ChkLayOut_Litri,
                                             x_Cod_RisUm_Aggiuntivo,
                                             x_Cod_Indirizzo_Aggiuntivo,
                                             x_Id_Cf_Aggiuntivo,
                                             x_CodContatto_Aggiuntivo,
                                             x_RagSoc_Aggiuntivo,
                                             x_CodiceFiscale_Aggiuntivo,
                                             0,
                                             Nothing,
                                             x_ChkLayOut_Riscontrato,
                                            Nothing,
                                               Nothing,
                                               Nothing,
                                                Nothing,
                                               Nothing,
                                               Nothing,
                                               Nothing, _
                                               Nothing)

                If msgErrore <> "" Then
                    logErrori &= msgErrore
                End If

            Catch ex As Exception
                logErrori &= "- Lettura della Ricevuta Fiscale: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try

                '============================================
                '======== LETTURA DATI INTESTAZIONE==========
                '============================================

                Leggi_Intestazione_Impresa_2(objParametriServer,
                                             progressivoGias,
                                             False,
                                             False,
                                             x_Id_Cf_Cliente,
                                             logErrori,
                                             Piva,
                                             x_CodiceFiscale_Impresa,
                                             x_RagSoc_Impresa,
                                             x_IndDes_Impresa,
                                             x_FrzDes_Impresa,
                                             x_Cap_Impresa,
                                             x_Comune_Impresa,
                                             x_Provincia_Impresa,
                                             x_Stato_Impresa,
                                             Intestazione_Riga3,
                                             Intestazione_Riga4,
                                             Intestazione_Riga5,
                                             Intestazione_Riga6,
                                             Intestazione_Riga7,
                                             Intestazione_Riga8,
                                             Intestazione_Riga9,
                                             Intestazione_Riga10,
                                             Intestazione_Riga11,
                                             Intestazione_Riga12,
                                             Intestazione_Riga13,
                                             Intestazione_Riga14,
                                             Intestazione_Riga15,
                                             False, Nothing, "", False)

            Catch ex As Exception
                logErrori &= "- Lettura dei dati dell'intestazione dell'impresa: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try

                '============================================
                '======== VISUALIZZA DATI INTESTAZIONE ======
                '============================================

                Carica_ImmaginiDinamiche_Ricevuta(logErrori, Dr_Intestazione, Piva, tipoRicevuta)

                Dr_Intestazione.Rag_Soc = x_RagSoc_Impresa

                'Int_Riga_1 = x_IndDes_Impresa & " " & x_FrzDes_Impresa
                'Int_Riga_2 = x_Cap_Impresa & " " & x_Comune_Impresa & " " & "(" & x_Provincia_Impresa & ")"

                If Intestazione_Riga10 <> "" And Intestazione_Riga11 <> "" Then
                    'sede operativa
                    Int_Riga_1 = Intestazione_Riga10
                    Int_Riga_2 = Intestazione_Riga11
                Else
                    'sede legale
                    Int_Riga_1 = Intestazione_Riga5
                    Int_Riga_2 = Intestazione_Riga6
                End If

                ''modifica del 20/09/2010
                ''introduco la chiamata a questa funzione
                ''per la gestione del taroccamento piva x multi-attività
                'Ricava_Piva_Codicefiscale(Piva, x_CodiceFiscale_Impresa, Piva, x_CodiceFiscale_Impresa, Nothing)

                'Int_Riga_3 = "Part. IVA: " & Piva & " " & "Cod. fisc.: " & x_CodiceFiscale_Impresa

                If x_Id_Cf_Cliente = 2 Then
                    Int_Riga_3 = "VAT: IT" & Piva
                Else
                    Int_Riga_3 = "Part.IVA: " & Piva
                End If
                If x_CodiceFiscale_Impresa <> "" Then
                    Int_Riga_3 &= " " & "Cod. fisc.: " & x_CodiceFiscale_Impresa
                End If

                'If x_Telefono <> "" Then
                '    Int_Riga_4 &= "Tel: " & x_Telefono
                'End If
                'If x_Fax <> "" Then
                '    If Int_Riga_4 <> "" Then
                '        Int_Riga_4 &= " "
                '    End If
                '    Int_Riga_4 &= "Fax: " & x_Fax
                'End If
                'If x_Cell <> "" Then
                '    If Int_Riga_4 <> "" Then
                '        Int_Riga_4 &= " "
                '    End If
                '    Int_Riga_4 &= "Cell: " & x_Cell
                'End If

                Int_Riga_4 = Intestazione_Riga7

                'If x_Email <> "" Then
                '    Int_Riga_5 = x_Email
                'End If
                'If x_SitoWeb <> "" Then
                '    If Int_Riga_5 <> "" Then
                '        Int_Riga_5 &= " "
                '    End If
                '    Int_Riga_5 &= x_SitoWeb
                'End If

                Int_Riga_5 = Intestazione_Riga8

                Dr_Intestazione.Int_Riga1 = Int_Riga_1
                Dr_Intestazione.Int_Riga2 = Int_Riga_2
                Dr_Intestazione.Int_Riga3 = Int_Riga_3
                Dr_Intestazione.Int_Riga4 = Int_Riga_4
                Dr_Intestazione.Int_Riga5 = Int_Riga_5

            Catch ex As Exception
                logErrori &= "- Visualizzazione intestazione: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '============================================
            '======== VISUALIZZA DATI RICEVUTA ==========
            '============================================

            Dr_Intestazione.Data = CType(x_Data_Movimento, DateTime)
            Dr_Intestazione.Numero = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des
            Dr_Intestazione.Cliente_Riga1 = x_Rag_Soc

            'IdentificazioneDocumento = "n" & Dr_Intestazione.Numero & "_d" & Format(x_Data_Movimento, "dd-MM-yyyy")
            IdentificazioneDocumento = Format(x_Data_Movimento, "yyyy-MM-dd") & "_n" & Dr_Intestazione.Numero & "_" & x_Rag_Soc


            '=============================================================
            '------------------------ CLIENTE  ---------------------------

            If x_Cod_IndirizzoRisUm <> 0 Then

                Try

                    Leggi_Indirizzi(objParametriServer,
                                    x_Cod_Contatto,
                                    x_Cod_RisUm,
                                    x_Cod_IndirizzoRisUm,
                                    x_Ind_Des,
                                    x_Frz_Des,
                                    x_Cap,
                                    x_Comune,
                                    x_Provincia,
                                    x_Stato,
                                    Flag_StatoMembro,
                                    Nothing, _
                                    "")

                    Select Case x_Id_Cf_Destinazione
                        Case 2
                            If x_Comune <> "" And x_Provincia <> "" Then
                                Cliente_Indirizzo = x_Ind_Des & " " & x_Frz_Des & " "
                                Cliente_Indirizzo &= x_Cap & " " & x_Stato
                            End If
                        Case Else
                            If x_Comune <> "" And x_Provincia <> "" Then
                                Cliente_Indirizzo = x_Ind_Des & " " & x_Frz_Des & " "
                                Cliente_Indirizzo &= x_Cap & " " & x_Comune & " " & "(" & x_Provincia & ")"
                            End If
                    End Select

                Catch ex As Exception
                    'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
                    logErrori &= "- Lettura dell'indirizzo del contatto cliente: " & vbCrLf & ex.Message & vbCrLf
                End Try

            End If


            Dr_Intestazione.Cliente_Riga2 = Cliente_Indirizzo

            '=============================================================
            '--------------- RIEPILOGO A FONDO DOCUMENTO  ----------------

            Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
            Dim Dt_Dettagli_Round As DataTable = Nothing
            Dim Dt_Riepilogo_IVA As DataTable
            Dim Riepilogo_ImponibileLordo As Decimal = 0
            Dim Riepilogo_Variazioni As Decimal = 0
            Dim Riepilogo_ImponibileNetto As Decimal = 0
            Dim Riepilogo_Imposta As Decimal = 0
            Dim Riepilogo_Importo As Decimal = 0
            Dim Totale_Importi_Omaggio As Decimal = 0

            If x_Extra_Str <> "" Then
                Dr_Intestazione.Note = "Note: " & x_Extra_Str
            Else
                Dr_Intestazione.Note = ""
            End If

            Try

                '====================================================================================
                '---------------- Prelevo le Informazioni sui dettagli ----------------------------

                Dim i As Integer

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
                Dim x_Flag_Extra As Integer = 0
                Dim x_OTabella_Cod_Base As Integer = 0
                Dim x_Qta As Decimal = 0
                Dim x_Qta_Extra As Decimal = 0
                Dim x_Qta_Extra_Totale As Decimal = 0
                Dim x_Descrizione As String = ""
                Dim x_Prezzo_Unitario As Decimal = 0
                Dim x_Prezzo_Unitario_Netto As Decimal = 0
                Dim x_Imponibile As Decimal = 0
                Dim x_Imponibile_Netto As Decimal = 0
                Dim x_Cod_IVA As Integer = 0
                Dim x_Aliquota As String = ""
                Dim x_IVA As Double = 0
                Dim x_ChkIVA_Manuale As Integer = 0
                Dim x_Cod_IVAIndetraibile As Integer = 0
                Dim x_Sconto_Perc As Decimal = 0
                Dim x_Sconto_Perc_2 As Decimal = 0
                Dim x_Sconto_Testo As String = ""
                Dim x_Sconto As Decimal = 0
                Dim x_Sconto_Modalita As Integer = 0
                Dim x_Prezzo_Effettivo As Decimal = 0
                Dim x_Anno As Integer = 0
                Dim x_Ric_Cod As Integer = 0
                Dim x_Cod_Conto As Integer = 0
                Dim x_Conto As String = ""
                Dim x_Contabilizzato As Integer = 0
                Dim x_Pendente As Integer = 0
                Dim x_ChkLayOut_Hide As Integer = 0
                Dim x_Tara As Decimal = 0
                Dim x_Extra_Str_Dettagli As String = ""
                Dim x_Extra_Int_Dettagli As Integer = 0
                Dim x_Extra_Date_Dettagli As Date = #1/1/1900#

                Dim x_Veg_Cod As Integer = 0
                Dim x_Cul_Cod As Integer = 0

                Dim x_N_Conf_Riscontrate As Integer = -1
                Dim x_N_Colli_Riscontrati As Integer = -1
                Dim x_N_Imballi_Riscontrati As Integer = -1
                Dim x_Peso_Netto_Riscontrato As Decimal = 0
                Dim x_Peso_Lordo_Riscontrato As Decimal = 0
                Dim x_Tara_Unit_Collo_Riscontrata As Decimal = -1
                Dim x_Tara_Unit_Imballo_Riscontrata As Decimal = -1

                '------------------------------
                'Dim Riga_Qta As String
                'Dim Riga_Prezzo As String
                Dim Riga_Sconto As String = ""
                Dim Riga_Omaggio As String = ""
                Dim Riga_Importo As String = ""
                Dim Importo_Dettaglio As Decimal = 0
                Dim Imponibile_Dettaglio As Decimal = 0
                Dim Iva_Dettaglio As Decimal = 0
                'Dim Riga_Iva As String

                Dim Flag_Campioni_Omaggio As Boolean = False
                Dim Flag_Sconto_Merce As Boolean = False
                Dim Flag_Campioni_Gratuiti As Boolean = False

                Dim Riferimento_DocAllegato As String = ""
                Dim Nome_Calibro As String = ""
                'Dim Importo_Riga As String

                Dim Flag_Raggruppa As Boolean = False

                If Flag_StampaACapoAuto = False Then
                    If Dt_Documento.Rows.Count > NUM_DETTAGLI_GESTITI Then
                        logErrori &= "- La Ricevuta Fiscale ha un numero di dettagli superiore al numero di dettagli gestiti dalla stampa! " & vbCrLf & vbCrLf
                        Flag_SuperatoNumDettagliGestiti = True
                        Exit Sub
                    End If
                End If

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Piva, objParametriServer)

                For i = 0 To Dt_Documento.Rows.Count - 1

                    Try

                        Leggi_MovimentoDettaglio_DocContabile(objParametriServer,
                                                              objParametriUtenti,
                                                              moduliCliente,
                                                              logErrori,
                                                              Dt_Documento.Rows(i),
                                                              Piva,
                                                              Lav_Cod,
                                                              Flag_Raggruppa,
                                                              x_ChkLayOut_Peso,
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
                                                              Dt_Dettagli_Round,
                                                              False,
                                                              False,
                                                              Flag_StampaNumeroVasca,
                                                              Flag_DDTallegati,
                                                              numcolli_contenitori,
                                                              contenitore_cod,
                                                              descr_contenitore,
                                                              numcontenitori_imballi,
                                                              imballaggio_cod,
                                                              descr_imballo,
                                                              ChkConfezione,
                                                              ChkContenitore,
                                                              ChkImballaggio,
                                                              "", "",
                                                              x_Flag_Extra,
                                                              x_OTabella_Cod_Base,
                                                              x_Livello_Prezzo,
                                                              objConfigStampe,
                                                              Dettaglio_Evasione,
                                                              x_N_Conf_Riscontrate, x_N_Colli_Riscontrati, x_N_Imballi_Riscontrati,
                                                              x_Peso_Netto_Riscontrato, x_Peso_Lordo_Riscontrato,
                                                              x_Tara_Unit_Collo_Riscontrata, x_Tara_Unit_Imballo_Riscontrata)

                        'nelle ricevute fiscali non c'è la gestione dell'andata a capo per carattere §
                        'quindi reimposto la descrizione senza <br>
                        x_Descrizione = Replace(x_Descrizione, "<br>", " ")

                        If x_Elem_Cod <> RIGA_DESCRIZIONE_LIBERA Then

                            If Flag_NuoviArrotondamenti = True Then
                                Dt_Riepilogo_IVA = objLanRound.FormAggiornaImportoNEW(objParametriServer,
                                                                                      Dt_Dettagli_Round,
                                                                                      Riepilogo_ImponibileLordo,
                                                                                      Riepilogo_Variazioni,
                                                                                      Riepilogo_ImponibileNetto,
                                                                                      Riepilogo_Imposta,
                                                                                      Riepilogo_Importo,
                                                                                      x_Edit_Importo, _
                                                                                      False, _
                                                                                      False, _
                                                                                       enum_EsigibilitaIva.Non_Specificata)
                            Else
                                'ad ogni dettaglio ricalcolo gli importi e aggiorno il dtIva
                                Dt_Riepilogo_IVA = objLanRound.FormAggiornaImporto(objParametriServer,
                                                                                   Dt_Dettagli_Round,
                                                                                   Riepilogo_ImponibileLordo,
                                                                                   Riepilogo_Variazioni,
                                                                                   Riepilogo_ImponibileNetto,
                                                                                   Riepilogo_Imposta,
                                                                                   Riepilogo_Importo,
                                                                                   x_Edit_Importo)
                            End If

                            Importo_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("importo_totale")
                            Imponibile_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("imponibile_netto")
                            Iva_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("iva")

                            'Importo_Riga = Format(Importo_Dettaglio, "#,###,##0.00")
                        Else
                            Importo_Dettaglio = 0
                            Imponibile_Dettaglio = 0
                            Iva_Dettaglio = 0
                        End If
                        '===================================================================

                    Catch ex As Exception
                        logErrori &= "- Lettura dei dettagli della Ricevuta Fiscale: " & vbCrLf & ex.Message & vbCrLf
                    End Try


                    'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                    If x_ChkLayOut_Hide <> 1 Then

                        Select Case x_Sconto_Modalita

                            Case enModalitaSconto.Percentuale
                                '   CASO NORMALE
                                Riga_Importo = Format(Importo_Dettaglio, "#,###,##0.00")
                                Riga_Omaggio = ""
                                Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)
                                '=============================================

                            Case enModalitaSconto.Sconto_Merce
                                '   CASO SCONTO MERCE
                                Riga_Importo = "Sconto Merce"
                                Riga_Sconto = ""
                                Riga_Omaggio = "Sconto Merce"
                                Flag_Sconto_Merce = True
                                '=============================================

                            Case enModalitaSconto.Omaggio_SenzaRivalsaIva, enModalitaSconto.Omaggio_ConRivalsaIva
                                '   CASO CAMPIONI OMAGGIO
                                Riga_Importo = "Omaggio"
                                Riga_Omaggio = "Omaggio"
                                Riga_Sconto = ""
                                Flag_Campioni_Omaggio = True
                                Totale_Importi_Omaggio += Imponibile_Dettaglio + Iva_Dettaglio
                                '=============================================

                            Case enModalitaSconto.Campioni_Gratuiti
                                '   CASO CAMPIONI GRATUITI
                                'non esiste x fattura, ma solo per ddt
                                Riga_Importo = "Camp.Gratuito"
                                Riga_Omaggio = "Camp.Gratuito"
                                Riga_Sconto = ""
                                '=============================================
                        End Select

                        '===================================================================

                        'il dettaglio è visibile

                        If Riga_Sconto <> "" Then
                            x_Descrizione = x_Descrizione & " [Sconto " & Riga_Sconto & "]"
                        End If
                        If Riga_Omaggio <> "" Then
                            x_Descrizione = x_Descrizione & " [" & Riga_Omaggio & "]"
                        End If
                        If Riferimento_DocAllegato <> "" Then
                            x_Descrizione = x_Descrizione & Riferimento_DocAllegato
                        End If

                        Dr_Dettagli = DS.Dettagli.NewDettagliRow

                        If x_Elem_Cod = RIGA_DESCRIZIONE_LIBERA Then
                            Dr_Dettagli.Qta = ""
                            Dr_Dettagli.Udm_Sim = ""
                            Dr_Dettagli.Sconto = ""
                            Dr_Dettagli.Importo = ""
                        Else
                            Dr_Dettagli.Qta = Format(x_Qta, "#,###,##0.####")
                            Dr_Dettagli.Udm_Sim = x_Udm_Sim
                            Dr_Dettagli.Sconto = "" 'visualizzo lo sconto insieme alla descrizione

                            Dr_Dettagli.Importo = Riga_Importo
                        End If

                        Dr_Dettagli.Descrizione = x_Descrizione

                        DS.Dettagli.Rows.Add(Dr_Dettagli)

                    End If 'x_ChkLayOut_Hide -> visibilità del edttaglio

                Next 'dettagli

                '===================================================================

                Try

                    '   CASO CAMPIONI OMAGGIO
                    If Flag_Campioni_Omaggio = True Or Flag_Campioni_Gratuiti = True Then
                        Dr_Dettagli = DS.Dettagli.NewDettagliRow
                        Dr_Dettagli.Qta = ""
                        Dr_Dettagli.Udm_Sim = ""
                        Dr_Dettagli.Sconto = ""
                        Dr_Dettagli.Descrizione = "Cessione gratuita ai sensi dell'art.2, comma 2, n.4 del D.P.R. n.633/1972"
                        Dr_Dettagli.Importo = ""
                        DS.Dettagli.Rows.Add(Dr_Dettagli)
                        Dr_Dettagli = DS.Dettagli.NewDettagliRow
                        Dr_Dettagli.Qta = ""
                        Dr_Dettagli.Udm_Sim = ""
                        Dr_Dettagli.Sconto = ""
                        Dr_Dettagli.Descrizione = "senza esercizio di rivalsa dell'IVA, ai sensi dell'art.18, comma 3, del D.P.R. n.633/1972"
                        Dr_Dettagli.Importo = ""
                        DS.Dettagli.Rows.Add(Dr_Dettagli)
                        Dr_Dettagli = DS.Dettagli.NewDettagliRow
                        Dr_Dettagli.Qta = ""
                        Dr_Dettagli.Udm_Sim = ""
                        Dr_Dettagli.Sconto = ""
                        Dr_Dettagli.Descrizione = "Importo totale omaggi " & Format(Totale_Importi_Omaggio, "#,###,##0.00")
                        Dr_Dettagli.Importo = ""
                        DS.Dettagli.Rows.Add(Dr_Dettagli)
                    End If

                    '   CASO SCONTO MERCE
                    If Flag_Sconto_Merce = True Then
                        Dr_Dettagli = DS.Dettagli.NewDettagliRow
                        Dr_Dettagli.Qta = ""
                        Dr_Dettagli.Udm_Sim = ""
                        Dr_Dettagli.Sconto = ""
                        '23/05/2019: aggiornata dicitura su richiesta di BORGOLUCE
                        'Dr_Dettagli.Descrizione = vbCrLf & vbCrLf & "Sconto merce senza esercizio di rivalsa dell'IVA ai sensi dell'art.15 del D.P.R. n.633/1972"
                        Dr_Dettagli.Descrizione = vbCrLf & vbCrLf & "Sconto merce escluso dalla base imponibile ai sensi dell’art. 15/2 DPR 633/72"
                        Dr_Dettagli.Importo = ""
                        DS.Dettagli.Rows.Add(Dr_Dettagli)
                    End If

                Catch ex As Exception
                    logErrori &= "- Visualizzazione della nota sugli omaggi: " & vbCrLf & ex.Message & vbCrLf
                End Try

                '===================================================================

            Catch ex As Exception
                logErrori &= "- Visualizzazione dei dettagli della ricevuta Fiscale: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '====================================================================================

            '24/03/2015:
            'SPOSTATO QUI,
            'IN MODO DA AVERE IL TOTALE DEL DOCUMENTO GENERATO CON L'ALGORITMO LUCCHI
            '=============================================================
            '--------------------- PAGAMENTI --------------------------
            Dim Importo_Pagato As Decimal = 0
            Dim Importo_NonPagato As Decimal = 0

            Try

                Leggi_Importo_PagatoNonPagato(objParametriServer,
                                              logErrori,
                                              Piva,
                                              Id_Agenda,
                                              x_Id_Mov_Contabile,
                                              Riepilogo_Importo,
                                              Importo_Pagato,
                                              Importo_NonPagato)

            Catch ex As Exception
                logErrori &= "- Lettura dei pagamenti della ricevuta: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Dr_Intestazione.Corrispettivo_Pagato = Format(Importo_Pagato, "#,###,##0.00")
            Dr_Intestazione.Corrispettivo_NonPagato = Format(Importo_NonPagato, "#,###,##0.00")
            Dr_Intestazione.Totale = Format(Riepilogo_Importo, "#,###,##0.00")

            DS.Intestazione.Rows.Add(Dr_Intestazione)

        Catch ex As Exception
            logErrori &= "- Ricevuta Fiscale: " & vbCrLf & ex.Message & vbCrLf
        End Try


    End Sub

    '############################################################################
    Public Sub Carica_ImmaginiDinamiche_Ricevuta(ByRef logErrori As String,
                                                 ByRef dr As DS_RicevutaFiscaleA5.IntestazioneRow,
                                                 ByVal piva As String,
                                                 ByVal tipoRicevuta As enum_TipoStampaRicevutaFiscale)

        Dim nomeLogo As String
        Dim nomeLogoAlternativo As String
        Dim nomePromozione As String
        Dim nomePromozioneAlternativo As String
        Dim path As String
        Dim pathAlternativo As String

        Try

            'di default inserisco un'immagine bianca come logo
            Dim bmpImg As New Drawing.Bitmap(1, 1)
            bmpImg.SetPixel(0, 0, Drawing.Color.White)

            Dim cx As New Drawing.ImageConverter
            Dim logoBianco() As Byte
            logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))

            dr.Logo = logoBianco
            dr.ImmaginePromozionale = logoBianco

            '====================================================


            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

            Select Case tipoRicevuta

                Case enum_TipoStampaRicevutaFiscale.PdfA4Singola,
                    enum_TipoStampaRicevutaFiscale.PdfA4ACapoAutomatico

                    'LOGO
                    nomeLogo = piva & "_LogoRicevutaA4.jpg"
                    nomeLogoAlternativo = "LogoRicevutaA4.jpg" 'Usato internamente o per prova

                    '1 PROMOZIONE
                    nomePromozione = piva & "_LogoPromozioneRicevutaA4.jpg"
                    nomePromozioneAlternativo = "LogoPromozioneRicevutaA4"

                Case enum_TipoStampaRicevutaFiscale.PdfA5

                    nomeLogo = piva & "_LogoRicevutaA5.jpg"
                    nomeLogoAlternativo = "LogoRicevutaA5.jpg" 'Usato internamente o per prova

                    nomePromozione = piva & "_LogoPromozioneRicevutaA5.jpg"
                    nomePromozioneAlternativo = "LogoPromozioneRicevutaA5"

                Case Else

                    Exit Sub

            End Select

            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                objWebConfig.Path_Directory_Loghi_Cliente = AggiungiSlashSeNonEsiste(objWebConfig.Path_Directory_Loghi_Cliente)

                Try

                    path = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogo
                    pathAlternativo = objWebConfig.Path_Directory_Loghi_Cliente & nomeLogoAlternativo

                    If IO.File.Exists(path) = True Then
                        dr.Logo = My.Computer.FileSystem.ReadAllBytes(path)
                    Else
                        If IO.File.Exists(pathAlternativo) = True Then
                            dr.Logo = My.Computer.FileSystem.ReadAllBytes(pathAlternativo)
                        End If
                    End If

                Catch ex As Exception
                    logErrori &= "Carica_ImmaginiDinamiche_Ricevuta. Caricamento logo cliente: " & ex.Message
                End Try



                Try

                    path = objWebConfig.Path_Directory_Loghi_Cliente & nomePromozione
                    pathAlternativo = objWebConfig.Path_Directory_Loghi_Cliente & nomePromozioneAlternativo

                    If IO.File.Exists(path) = True Then
                        dr.ImmaginePromozionale = My.Computer.FileSystem.ReadAllBytes(path)
                    Else
                        If IO.File.Exists(pathAlternativo) = True Then
                            dr.ImmaginePromozionale = My.Computer.FileSystem.ReadAllBytes(pathAlternativo)
                        End If
                    End If

                Catch ex As Exception
                    logErrori &= "Carica_ImmaginiDinamiche_Ricevuta. Caricamento immagine promozionale: " & ex.Message
                End Try

            End If 'path

        Catch ex As Exception
            logErrori &= "Carica_ImmaginiDinamiche_Ricevuta: " & ex.Message
        End Try

    End Sub

    '#####################################################################################################
    Private Sub Leggi_Importo_PagatoNonPagato(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef logErrori As String,
                                              ByVal Piva As String,
                                              ByVal Id_Agenda As Integer,
                                              ByVal Id_Mov As Integer,
                                              ByRef Importo_Totale_Documento As Decimal,
                                              ByRef Importo_Pagato As Decimal,
                                              ByRef Importo_NonPagato As Decimal)

        Importo_Pagato = 0
        Importo_NonPagato = 0

        objParametriServer.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_R
        Dim dtPag As DataTable
        Dim i As Integer

        dtPag = objPagamenti.Leggi(Piva,
                                   0,
                                   Id_Agenda,
                                   Id_Mov,
                                   0,
                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                   " Previsto_Avvenuto = 1 ", "",
                                   objParametriServer)

        If Not IsNothing(dtPag) AndAlso dtPag.Rows.Count > 0 Then

            Dim x_Percentuale_Pagamento As Integer
            Dim x_Importo_Pagamento As Decimal
            Dim Importo_Pagato_Corrente As Decimal = 0

            For i = 0 To dtPag.Rows.Count - 1

                With dtPag.Rows(i)

                    x_Percentuale_Pagamento = .Item("percentuale")
                    x_Importo_Pagamento = CDec(.Item("importo"))

                    Select Case x_Percentuale_Pagamento
                        Case 0
                            'gestione importo fisso salvato dall'utente
                            Importo_Pagato_Corrente = x_Importo_Pagamento
                        Case 100
                            If x_Importo_Pagamento = 0 Then
                                'unica tranche per importo totale
                                Importo_Pagato_Corrente = Importo_Totale_Documento
                            Else
                                'modifica del 18/02/2013:
                                'anche se la percentuale è 100%, può essere che l'importo non coincide con il totale
                                'ad esempio la Molinelli aveva questo caso:
                                'totale= 930,47
                                '1 tranche =930,46
                                '2 tranche = 0,01
                                'Importo_Pagato_Corrente = x_Num_Protocollo
                                Importo_Pagato_Corrente = x_Importo_Pagamento
                            End If
                        Case Else
                            If x_Importo_Pagamento = 0 Then
                                'gestione con percentuali
                                'TODO: ARROTONDAMENTI - eliminare calcolo pagamento!!! --> forse non capita più
                                Importo_Pagato_Corrente = (x_Percentuale_Pagamento * Importo_Totale_Documento) / 100
                                Importo_Pagato_Corrente = ArrotondaVal_2(Importo_Pagato_Corrente)
                            Else
                                'gestione importo fisso salvato dall'utente
                                Importo_Pagato_Corrente = x_Importo_Pagamento
                            End If
                    End Select

                End With

                Importo_Pagato += Importo_Pagato_Corrente

            Next

            Importo_Pagato = ArrotondaVal_2(Importo_Pagato)

        End If

        Importo_NonPagato = Importo_Totale_Documento - Importo_Pagato

        objParametriServer.ResettaFinestra()


    End Sub

    '#####################################################################################################
    'Private Sub Leggi_Importo_PagatoNonPagato_OLD(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                                              ByRef Log_Errori As String,
    '                                              ByVal Piva As String,
    '                                              ByVal Id_Agenda As Integer,
    '                                              ByVal Id_Mov As Integer,
    '                                              ByVal x_Num_Protocollo As Double,
    '                                              ByRef Importo_Pagato As Double,
    '                                              ByRef Importo_NonPagato As Double)


    '    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)


    '    Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_R
    '    Dim DT_Pag As DataTable
    '    Dim i As Integer

    '    DT_Pag = objPagamenti.Leggi(Piva,
    '                                0,
    '                                Id_Agenda,
    '                                Id_Mov,
    '                                0,
    '                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
    '                                "", "",
    '                                objParametri_Server)

    '    If Not IsNothing(DT_Pag) AndAlso DT_Pag.Rows.Count > 0 Then

    '        Dim x_Percentuale_Pagamento As Integer
    '        Dim x_Importo_Pagamento As Double

    '        For i = 0 To DT_Pag.Rows.Count - 1

    '            With DT_Pag.Rows(i)

    '                x_Percentuale_Pagamento = .Item("percentuale")
    '                x_Importo_Pagamento = .Item("importo")

    '                If x_Percentuale_Pagamento = 0 Then

    '                    '--------------------------
    '                    'PAGAMENTO GIA' EFFETTUATO
    '                    '--------------------------

    '                    Importo_Pagato += x_Importo_Pagamento

    '                Else

    '                    '------------------------------
    '                    'PAGAMENTO ANCORA DA EFFETTUARE
    '                    '------------------------------

    '                    Importo_NonPagato += x_Percentuale_Pagamento * x_Num_Protocollo / 100

    '                End If

    '            End With

    '        Next

    '        Dim Somma_Importi As Double
    '        Somma_Importi = Importo_Pagato + Importo_NonPagato

    '        If x_Num_Protocollo <> (Somma_Importi) Then
    '            Log_Errori &= "Log debug: Importo_Pagato + Importo_NonPagato = " & CStr(Somma_Importi) & " <> x_Num_Protocollo = " & CStr(x_Num_Protocollo) & " " & vbCrLf & vbCrLf

    '            Dim Differenza As Double
    '            Differenza = Math.Abs(x_Num_Protocollo - Somma_Importi)
    '            Log_Errori &= "Log debug: differenza x_Num_Protocollo - Somma_Importi = " & CStr(Differenza) & " " & vbCrLf & vbCrLf
    '        End If


    '    End If


    '    objParametri_Server.ResettaFinestra()


    'End Sub

End Class
