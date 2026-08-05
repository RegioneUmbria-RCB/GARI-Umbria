Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabHLP.Contabilita
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreVarieBIZ

Public Class ContabilitaHelper_Testata

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

    End Sub

    Public Function LeggiTestataDocumento(ByVal piva As String,
                                          ByVal saCod As Integer,
                                          ByVal idAgenda As Integer,
                                          ByVal lavCod As Integer,
                                          ByRef msgError As String
                                          ) As Contabilita_Testata

        Dim flagConnessione As Boolean = False

        Dim objTestata As Contabilita_Testata
        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim objAgenda As Operazione_Agenda
        Dim objMovT As Movimento
        Dim objMovTSec As Movimento
        Dim objMovC As Movimento
        Dim objMovS As Movimento
        Dim objMovTExtra As Movimento_Dettaglio_Tecnico_Extra

        Try

            Utility.VerificaApriConnessione(_objParametriServer, flagConnessione)

            objAgenda = agendaHelper.Leggi(piva, saCod, idAgenda, lavCod, _objParametriServer)

            If Not IsNothing(objAgenda) Then

                Dim cauMov = CauMovDaLavCod(lavCod)

                'Ricerco il movimento di Testata e carico/scarico
                If Not IsNothing(objAgenda.Movimenti) AndAlso objAgenda.Movimenti.Count > 0 Then
                    objMovT = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = cauMov)
                    objMovTSec = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA)
                    objMovC = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_CARICO)
                    objMovS = objAgenda.Movimenti.Find(Function(x) x.Cau_Mov = CAU_SCARICO)
                Else
                    Throw New Exception("L'operazione non contiene Movimenti.")
                End If

                'Non ho trovato il movimento di testata e carico/scarico
                If IsNothing(objMovT) Then
                    Throw New Exception("L'operazione non contiene il Movimento di Intestazione")
                End If

                'Mov_Dettaglio_Tecnico_Extra collegato alla testata
                objMovTExtra = objMovT.Movimenti_Dettagli_Tecnici_Extra.Find(Function(x) x.Id_Mov_Det = 0 AndAlso x.Id_Mov = objMovT.Id_Mov)

                Dim utenteBlocco As String = ""
                If objAgenda.Blocco_Flag = 1 AndAlso objAgenda.Blocco_Username <> "" Then
                    utenteBlocco = UtilityHelper.GetNomeUtenteFromCF(objAgenda.Blocco_Username, _objParametriUtenti)
                End If

                objTestata = New Contabilita_Testata With {
                    .Piva = objAgenda.Piva,
                    .SaCod = objAgenda.Sa_Cod,
                    .IdAgenda = objAgenda.Id_Agenda,
                    .IdMov = objMovT.Id_Mov,
                    .LavCod = objAgenda.Lav_Cod,
                    .StatoExport2 = objAgenda.Stato_Export_2,
                    .ModuloGias = objAgenda.Modulo,
                    .DescrizioneAgenda = objAgenda.Des_Lib,
                    .BloccoFlag = objAgenda.Blocco_Flag,
                    .BloccoData = objAgenda.Blocco_Data,
                    .BloccoUsername = objAgenda.Blocco_Username,
                    .BloccoUtente = utenteBlocco,
                    .DataMovimento = objAgenda.Data,
                    .TipoAccettazione = objAgenda.Tipo_Accettazione,
                    .ContabilizzazioneManuale = objAgenda.ChkCoge_Manuale,
                    .CodRisUm = objMovT.Cod_Risum,
                    .CodIndirizzoRisUm = objMovT.Cod_IndirizzoRisUm,
                    .CodDestinazione = objMovT.Cod_Destinazione,
                    .CodIndirizzoDestinazione = objMovT.Cod_IndirizzoDestinazione,
                    .NoteIntestazione = If(objMovT.Extra_Str IsNot Nothing AndAlso objMovT.Extra_Str <> "", objMovT.Extra_Str, objMovT.Mov_Desc),
                    .DocNumeroSin = objMovT.Doc_Numero_Sin,
                    .DocNumero = objMovT.Doc_Numero,
                    .DocNumeroDes = objMovT.Doc_Numero_Des,
                    .DocNumeroVisualizzato = objMovT.Doc_Numero_Visualizzato,
                    .TotaleDocumento = objMovT.Num_Protocollo_Decimal,
                    .Mezzo = objMovT.Mezzo,
                    .CodVettore = objMovT.Cod_Vettore,
                    .CodIndirizzoVettore = objMovT.Cod_IndirizzoVettore,
                    .CausaleTrasporto = objMovT.Causale_Trasporto,
                    .CausaleTrasportoCod = objMovT.Causale_Trasporto_Cod,
                    .Aspetto = objMovT.Aspetto,
                    .TipoPeso = objMovT.Tipo_Peso,
                    .Peso = objMovT.Peso,
                    .Colli = objMovT.Colli,
                    .OraSpedizione = objMovT.Ora,
                    .NaturaBeni = objMovT.Natura_Beni,
                    .TaraVeicolo = objMovT.Tara_Veicolo,
                    .TaraImballi = objMovT.Tara_Imballi,
                    .ProgrProtocollo = objMovT.Progr_Protocollo,
                    .ProgrRegistrazione = objMovT.Progr_Registrazione,
                    .DataRegistrazione = objMovT.Data_Registrazione,
                    .CodRisUmAltro = objMovT.Cod_RisUm_Altro,
                    .CodRisUmAggiuntivo = objMovT.Cod_RisUm_Aggiuntivo,
                    .CodIndirizzoAggiuntivo = objMovT.Cod_Indirizzo_Aggiuntivo,
                    .SezionaleCod = objMovT.Sezionale_Cod,
                    .TipoDocumento = objMovT.TipoDocumento,
                    .AgenteCod = If(objMovTExtra IsNot Nothing, objMovTExtra.Agente_Cod, 0),
                    .AgenteProvvigione = If(objMovTExtra IsNot Nothing, CDec(objMovTExtra.Provvigione), 0),
                    .CapoAreaCod = If(objMovTExtra IsNot Nothing, objMovTExtra.CapoArea_Cod, 0),
                    .CapoAreaProvvigione = If(objMovTExtra IsNot Nothing, CDec(objMovTExtra.Provvigione_CapoArea), 0),
                    .ModalitaTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.Tipo_Trasporto, 0),
                    .UnitaTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.Unita_Trasporto, 0),
                    .GestioneVettore = If(objMovTExtra IsNot Nothing, objMovTExtra.Id_Gestione_Vettore, 0),
                    .MacCodTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.Mac_Cod, 0),
                    .Targa = If(objMovTExtra IsNot Nothing, objMovTExtra.Targa, ""),
                    .DescrizioneMezzo = If(objMovTExtra IsNot Nothing, objMovTExtra.Mezzo_Trasporto, ""),
                    .NumImmatricolazioneRimorchio = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Immatricolazione_Rimorchio, ""),
                    .NumAutorizzazioneTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Autorizzazione_Trasporto, ""),
                    .DataRilascioAutorizzazione = If(objMovTExtra IsNot Nothing, objMovTExtra.Data_Rilascio_Autorizzazione, Now),
                    .PesoTaraTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.Peso, 0),
                    .DistanzaTrasportoUdm = If(objMovTExtra IsNot Nothing, objMovTExtra.DistanzaTrasportoUdm, 0),
                    .DistanzaTrasporto = If(objMovTExtra IsNot Nothing, objMovTExtra.DistanzaTrasporto, 0),
                    .Layout_FormatiStampa = GetLayoutFormatiStampa(objMovT.ChkLayOut_Peso, objMovT.ChkLayOut_Prezzo, objMovT.ChkLayOut_Riscontrato),
                    .DataOraUltimaLettura = Now,
                    .UsernameModifica = objAgenda.Username_Modifica,
                    .PraticaCod = objAgenda.Pratica_Cod,
                    .N_Nota_Fattura = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Nota_Fattura, ""),
                    .Data_Nota_Fattura = If(objMovTExtra IsNot Nothing, objMovTExtra.Data_Nota_Fattura, AGRODATAINIZIO),
                    .N_Nota_DDT_Reso_SDI = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Nota_DDT, ""),
                    .Data_Nota_DDT_Reso_SDI = If(objMovTExtra IsNot Nothing, objMovTExtra.Data_Nota_DDT, AGRODATAINIZIO),
                    .N_Nota_Riga_DDT_Reso_SDI = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Nota_Riga_DDT, ""),
                    .CodPagamento = If(objMovT.Pagamenti IsNot Nothing AndAlso objMovT.Pagamenti.Count > 0, objMovT.Pagamenti.Item(0).Cod_Pagamento, 0),
                    .ModalitaPagamento = If(objMovT.Pagamenti IsNot Nothing AndAlso objMovT.Pagamenti.Count > 0, objMovT.Pagamenti.Item(0).Cau_Pagamento, 0)
                }

                If objAgenda.Pratica_Cod > 0 Then

                    Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

                    objTestata.StatoCodPratica = objPratiche.LeggiStatoAttualePratica(objAgenda.Pratica_Cod, _objParametriServer)

                End If

                'TODO: DataRilascioAutorizzazione = Now?!? (il Lan quando salva, mette sempre Now!)

                Select Case objAgenda.Lav_Cod
                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                        objTestata.Accompagnatoria = objMovT.Extra_Int

                    Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO

                        objTestata.DataEvasionePrevista = objMovT.Extra_Date
                        GetScadenzaOrdine(objTestata.ScadenzaUnica, objTestata.EvasioneTassativa, objMovT.Extra_Int)
                        objTestata.DataSpedizionePrevista = objMovT.Ora

                        objTestata.NumDocOrdineCliente = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Doc_Cliente, "")
                        objTestata.DataDocOrdineCliente = If(objMovTExtra IsNot Nothing, objMovTExtra.Data_Doc_Cliente, AGRODATAINIZIO)
                        objTestata.NumDocOrdineEnte = If(objMovTExtra IsNot Nothing, objMovTExtra.N_Doc_Ente, "")
                        objTestata.AnnoDocOrdineEnte = If(objMovTExtra IsNot Nothing, objMovTExtra.Anno_Doc_Ente, AGRODATAFINE.Year)

                    Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_BOLLA_RICEVUTA,
                        LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                        LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                        objTestata.SecondaCooperativa = objMovT.Extra_Int
                End Select


                'TODO: ricalcolo anche i totali del netto del documento
                Dim dtTotali As DataTable = Nothing
                Dim objContabR As New AgronicaCoreContabDAL.Contabilita_R

                If Not UtilityHelper.IsMovimentoMagazzino(lavCod) Then
                    dtTotali = objContabR.ConteggioTotaliDocumento(piva, lavCod, idAgenda,
                                                                                "", _objParametriServer)
                End If

                If dtTotali IsNot Nothing AndAlso dtTotali.Rows.Count > 0 Then

                    If dtTotali.Rows.Count > 1 Then
                        Throw New Exception(String.Format("Sono presenti {0} righe di Conteggio Totali", dtTotali.Rows.Count))
                    End If

                    'TODO: in realtà quello che mi serve è solo la somma del netto

                    'Devo Aggiornare

                    Dim newNumColli As Decimal = CDec(dtTotali.Rows(0).Item("Num_Colli_Auto_Tot"))
                    Dim newPesoNettoProd As Decimal = CDec(dtTotali.Rows(0).Item("Netto_Prod_Tot"))
                    Dim newTaraImballi As Decimal = CDec(dtTotali.Rows(0).Item("Tara_Prod_Tot"))
                    Dim newImballiVuoti As Decimal = CDec(dtTotali.Rows(0).Item("Imballi_Vuoti_Tot"))
                    Dim newTaraVeicolo As Decimal = CDec(dtTotali.Rows(0).Item("Tara_Veicolo_DB"))
                    Dim pesoLordoDocDb As Decimal = CDec(dtTotali.Rows(0).Item("Peso_DB"))

                    Dim newPesoLordoProd As Decimal = newPesoNettoProd + newTaraImballi

                    'Se accettazione non devo ricalcolare il peso lordo totale del documento
                    Dim newPesoLordoDoc As Decimal? = Nothing
                    If UtilityHelper.IsAccettazione(lavCod) Then
                        newPesoLordoDoc = pesoLordoDocDb
                    Else
                        newPesoLordoDoc = newPesoLordoProd + newImballiVuoti + newTaraVeicolo
                    End If

                    'Questi sono dati che non sono scritti su db
                    objTestata.PesoNettoProd = newPesoNettoProd
                    objTestata.ImballiVuoti = newImballiVuoti

                    'devo passarmi indietro anche i totali, perché sono da aggiornare in interfaccia!!!
                    objTestata.ConteggiTotali = New Contabilita_Totali_Testata() With {
                        .Piva = piva,
                        .IdAgenda = idAgenda,
                        .IdMov = objTestata.IdMov,
                        .Colli = newNumColli,
                        .PesoNettoProd = newPesoNettoProd,
                        .TaraImballi = newTaraImballi,
                        .PesoLordoProd = newPesoLordoProd,
                        .ImballiVuoti = newImballiVuoti,
                        .TaraVeicolo = newTaraVeicolo,
                        .PesoLordoDoc = newPesoLordoDoc
                    }
                Else
                    objTestata.PesoNettoProd = 0
                    objTestata.ImballiVuoti = 0
                End If


                'FormCmbSelezionaOperatore(objMovT.Username_Creazione)
                'mData_Creazione = objTestata.DataMovimento

                'Select Case lavCod

                '    Case enModalitaDoc.Auto_DDT_Accettazione[1078], 
                '        enModalitaDoc.DDT_Ricevuto_Accettazione[1054], 
                '        enModalitaDoc.Carico_Ortofrutta_Accettazione[1076]

                '        TxtNumero(Intestatario).text = Format(objMovT.Doc_Numero, "00000")
                '        TxtNumero_Sin(Intestatario).text = objMovT.Doc_Numero_Sin
                '        TxtNumero_Des(Intestatario).text = objMovT.Doc_Numero_Des

                '    Case Else

                '        TxtDocNumero.text = Format(objMovT.Doc_Numero, "00000")
                '        TxtDocNumero_Sin.text = objMovT.Doc_Numero_Sin
                '        TxtDocNumero_Des.text = objMovT.Doc_Numero_Des

                'End Select

                'mTipo_Accettazione = enTipoAccettazione.ACCETTAZIONE_POST_CAMPIONATURA[2]

                '=======================================================================================
                'Spese Trasporto
                '---------------------------------------------------------------------------------------
                'TxtSpese_Trasporto = objMovT.Num_Protocollo

                'If objMovTExtra.Mac_Cod <> 0 Then
                '    ChkMacchinaGias(Trasportatore).value = ssChecked
                '    FormCmbSeleziona CmbTarga(Trasportatore), CLng(objMovTExtra.Mac_Cod)
                'End If

                'Impostazione Valori
                'TxtTarga(Trasportatore) = UCase(objMovTExtra.Targa)


                If objMovTSec IsNot Nothing Then

                    If UtilityHelper.IsContrattoAffitto(objAgenda.Lav_Cod) Then

                        objTestata.DocumentoContrattoAffitto = New Contabilita_Contratto_Affitto() With {
                        .IdMov = objMovTSec.Id_Mov,
                        .DocNumeroSin = objMovTSec.Doc_Numero_Sin,
                        .DocNumero = objMovTSec.Doc_Numero,
                        .DocNumeroDes = objMovTSec.Doc_Numero_Des,
                        .DataInizioValidita = objMovTSec.Data_Registrazione,
                        .DataFineValidita = objMovTSec.Scadenza,
                        .AltriLocatori = objMovTSec.Mov_Desc,
                        .RiferimentoOrdini = objMovTSec.Extra_Str
                        }

                    Else

                        objTestata.DocumentoAccettazione = New Contabilita_Accettazione() With {
                        .IdMov = objMovTSec.Id_Mov,
                        .DataMovimento = objMovTSec.Data,
                        .DocNumeroSin = objMovTSec.Doc_Numero_Sin,
                        .DocNumero = objMovTSec.Doc_Numero,
                        .DocNumeroDes = objMovTSec.Doc_Numero_Des
                        }

                    End If
                End If

                If Not IsNothing(objMovC) Then
                    objTestata.MovimentoCarico = New Contabilita_Chiave_Mov With {
                        .Piva = objMovC.Piva,
                        .IdAgenda = objMovC.Id_Agenda,
                        .SaCod = objMovC.Sa_Cod,
                        .IdMov = objMovC.Id_Mov,
                        .CauMov = objMovC.Cau_Mov
                    }
                End If

                If Not IsNothing(objMovS) Then
                    objTestata.MovimentoScarico = New Contabilita_Chiave_Mov With {
                        .Piva = objMovS.Piva,
                        .IdAgenda = objMovS.Id_Agenda,
                        .SaCod = objMovS.Sa_Cod,
                        .IdMov = objMovS.Id_Mov,
                        .CauMov = objMovS.Cau_Mov
                    }
                End If

                Dim LblAllegato As String = ""
                Dim LblRiferimento As String = ""
                GetDocumentiCollegati(piva, idAgenda, lavCod, LblRiferimento, LblAllegato)

                objTestata.Documenti_Allegati = LblAllegato
                objTestata.Lavorazioni_Associate = LblRiferimento

            Else
                Throw New Exception("L'operazione non esiste o non è un documento contabile.")
            End If

        Catch ex As Exception
            Throw New Exception("[ ContabilitaHelper.LeggiTestataDocumento() ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objTestata

    End Function

    Public Function AggiornaTestataDocumento(ByVal objContabTestata As Contabilita_Testata,
                                             ByVal messaggioDettagliato As Boolean
                                             ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.AggiornaTestataDocumento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim flagOggettoCorretto As Boolean

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim movHelper As New Agenda_Movimenti_Helper
        Dim movDetTecExW As New Mov_Dett_Tecnico_Ex_W

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataContabilita(objContabTestata, enum_TipoOperazioneDB.Modifica, _objParametriServer)

            If flagOggettoCorretto Then

                'Prima di procedere con la modifica devo verificare se nel frattempo il record dell'agenda è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""
                Dim dataMovimento As Date

                dataUltimaModifica = UtilityHelper.UltimaModificaAgenda(objContabTestata.Piva,
                                                                        objContabTestata.IdAgenda,
                                                                        usernameUltimaModifica,
                                                                        dataMovimento,
                                                                        _objParametriServer)

                If Not IsNothing(objContabTestata.DataOraUltimaLettura) AndAlso dataUltimaModifica > objContabTestata.DataOraUltimaLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    Dim docDuplicato As Boolean = False
                    Dim msgDocDuplicato As String = ""
                    Dim msgErrorCoerenzaDate As String = ""

                    'TODO: questo controllo è da fare sempre o solo se sono doc ricevuto oppure doc emesso + NO LOCK?!?
                    'Devo verificare se nel frattempo qualcuno ha già inserito un documento con lo stesso numero!

                    ' Controllo sa latare in caso di tipologia documento 28 (Acquisti da San Marino con IVA (fattura cartacea) 
                    If objContabTestata.TipoDocumento <> 28 Then
                        docDuplicato = UtilityHelper.ControllaEsistenzaNumDoc(objContabTestata.LavCod,
                                                                          objContabTestata.Piva,
                                                                          objContabTestata.IdAgenda,
                                                                          objContabTestata.DocNumeroSin,
                                                                          objContabTestata.DocNumero,
                                                                          objContabTestata.DocNumeroDes,
                                                                          objContabTestata.DataMovimento,
                                                                          objContabTestata.DataRegistrazione,
                                                                          objContabTestata.SezionaleCod,
                                                                          objContabTestata.CodRisUm,
                                                                          _objParametriServer,
                                                                          _objParametriUtenti,
                                                                          msgDocDuplicato)
                    End If

                    'Se il documento è duplicato, è inutile che faccio le verifiche successive
                    If docDuplicato = False AndAlso Not UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod) Then

                        Dim newNumDoc As Integer = 0
                        Dim newNumDocVisualizzato As String = ""

                        If objContabTestata.TipoDocumento = 28 Then
                            objContabTestata.DocNumeroVisualizzato = objContabTestata.DocNumero
                        Else
                            msgErrorCoerenzaDate = UtilityHelper.AlgoritmoAssegnazioneNumDoc(newNumDoc, newNumDocVisualizzato,
                                                                                         objContabTestata.LavCod,
                                                                                         objContabTestata.Piva,
                                                                                         objContabTestata.DataMovimento,
                                                                                         objContabTestata.DocNumeroSin,
                                                                                         objContabTestata.DocNumero,
                                                                                         objContabTestata.DocNumeroDes,
                                                                                         objContabTestata.DocNumeroLock,
                                                                                         objContabTestata.DocNumeroVisualizzato,
                                                                                         _objParametriServer,
                                                                                         objContabTestata.DocNumeroLunghezza,
                                                                                         objContabTestata.DocNumeroCarattereFormattazione)

                            objContabTestata.DocNumero = newNumDoc
                            objContabTestata.DocNumeroVisualizzato = newNumDocVisualizzato

                        End If

                    End If

                    If docDuplicato = True Then
                        msgError &= msgDocDuplicato
                    ElseIf msgErrorCoerenzaDate <> "" Then
                        msgError &= msgErrorCoerenzaDate
                    Else

                        'TODO: gestire casi particolari: cambio data

                        'TODO: devo quindi leggere almeno l'intero record di agenda per sapere se è cambiato la data

                        'TODO: se è cambiata la data, devo cambiarla su tutti i suoi figli a cascata e quindi salvare tutte le righe
                        'TODO: se cambia la data devo come minimo ricalcolare la giacenza (solo se ho impostazione che impedisce il sottogiacenza?!?) di tutti i prodotti del documento nella nuova data (eventualmente anche giacenza nel futuro?!?)

                        'TODO: manca tutto il controllo del documento

                        'TODO: con la nuova modalità che salva ad ogni singola riga, come mi comporto con la data ultima modifica dell'agenda? e Agronica_Log_Agenda?!?

                        'Ri-Creare Des_Lib
                        objContabTestata.DescrizioneAgenda = CreaDesLib(objContabTestata.LavCod,
                                                                    objContabTestata.DocNumeroSin, objContabTestata.DocNumero, objContabTestata.DocNumeroDes,
                                                                    objContabTestata.RagSoc,
                                                                    accompagnatoria:=If(objContabTestata.Accompagnatoria = 1, enum_FatturaTipo.Immediata, enum_FatturaTipo.Differita),
                                                                    numeroDocFormattato:=objContabTestata.DocNumeroVisualizzato,
                                                                    progrProtocollo:=objContabTestata.ProgrProtocollo,
                                                                    note:=objContabTestata.NoteIntestazione)

                        'TODO: Modulo e Tipo_Accettazione non li passo, perché una volta creati non dovrei più cambiarli
                        xRisp = agendaHelper.ModificaPuntuale(piva:=objContabTestata.Piva,
                                                          saCod:=0,
                                                          idAgenda:=objContabTestata.IdAgenda,
                                                          objParametri:=_objParametriServer,
                                                          desLib:=objContabTestata.DescrizioneAgenda,
                                                          validitaInizio:=objContabTestata.DataMovimento,
                                                          statoExport2:=objContabTestata.StatoExport2,
                                                          chkCogeManuale:=objContabTestata.ContabilizzazioneManuale,
                                                          usernameModifica:=objContabTestata.UsernameModifica,
                                                          bloccoFlag:=objContabTestata.BloccoFlag,
                                                          bloccoUsername:=objContabTestata.BloccoUsername,
                                                          bloccoData:=objContabTestata.BloccoData)

                        If xRisp = True Then

                            'L'Id_Mov della testata dovrebbe arrivarmi già...
                            If IsNothing(objContabTestata.IdMov) OrElse objContabTestata.IdMov = 0 Then
                                Throw New Exception("Non è presente il Movimento di Testata (Registrazione - " & CAU_REGISTRAZIONI & "). Impossibile Aggiornare")
                            End If

                            Dim extraInt As Integer? = Nothing
                            Dim extraDate As Date? = Nothing
                            Dim ora As DateTime? = Nothing

                            Select Case True

                                Case UtilityHelper.IsAccettazione(objContabTestata.LavCod)
                                    extraInt = objContabTestata.SecondaCooperativa
                                    ora = objContabTestata.OraSpedizione

                                Case UtilityHelper.IsFattura(objContabTestata.LavCod), UtilityHelper.IsNotaAccredito(objContabTestata.LavCod)
                                    extraInt = objContabTestata.Accompagnatoria
                                    ora = objContabTestata.OraSpedizione

                                Case UtilityHelper.IsOrdine(objContabTestata.LavCod)
                                    extraInt = CInt(SetScadenzaOrdine(objContabTestata.ScadenzaUnica, objContabTestata.EvasioneTassativa))
                                    extraDate = If(objContabTestata.DataEvasionePrevista, AGRODATAFINE)
                                    ora = If(objContabTestata.DataSpedizionePrevista, AGRODATAFINE)

                                Case UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod)
                                    ora = AGRODATAFINE

                                Case Else
                                    ora = objContabTestata.OraSpedizione

                            End Select

                            Dim chkLayOut_Peso As Integer = 0
                            Dim chkLayOut_Prezzo As Integer = 0
                            Dim chkLayOut_Riscontrato As Integer = 0
                            SetLayoutFormatiStampa(chkLayOut_Peso,
                                               chkLayOut_Prezzo,
                                               chkLayOut_Riscontrato,
                                               objContabTestata.Layout_FormatiStampa)

                            'TODO: movDesc:=objContabTestata.NoteIntestazione va messo se è una cosa definita dall'utente, sennò vuota o ricalcolata?!?

                            Dim objMovTestataLavCod = ModellaDatiTestataDaLavCod(objContabTestata)

                            Dim Leggi_Causali_Pagamento As New AgronicaCoreContabDAL.Pagamenti_Causali_R
                            Dim utilityHelperPagamento As New UtilityHelper 'Contiene la funzione Impostazione_Modalita_Pagamento
                            Dim dataScadenzaPagamento As Date? = Nothing

                            If objContabTestata.CodPagamento <> 0 Then

                                'Lettura della Causale di Pagamento per determinazione Scadenza               
                                Dim DT_Causali_Pagamento = Leggi_Causali_Pagamento.Leggi("", objContabTestata.ModalitaPagamento, "", "", _objParametriServer)

                                Dim Giorni_Scadenza, Opzione As Integer

                                'Controllo Presenza Record
                                If DT_Causali_Pagamento.Rows.Count > 0 Then

                                    'Determinazione Giorni Scadenza ed Opzione
                                    Giorni_Scadenza = DT_Causali_Pagamento(0).Item("Giorni_Scadenza")
                                    Opzione = DT_Causali_Pagamento(0).Item("Opzione")

                                End If


                                dataScadenzaPagamento = utilityHelperPagamento.CalcolaDataScadenzaPagamento(Opzione, Giorni_Scadenza, objContabTestata.DataMovimento)
                            End If

                            xRisp = movHelper.ModificaPuntuale(objContabTestata.Piva,
                                                           0,
                                                           objContabTestata.IdAgenda,
                                                           objContabTestata.IdMov,
                                                           _objParametriServer,
                                                           codRisUm:=objContabTestata.CodRisUm,
                                                           Cod_IndirizzoRisUm:=objContabTestata.CodIndirizzoRisUm,
                                                           Cod_Destinazione:=objContabTestata.CodDestinazione,
                                                           Cod_IndirizzoDestinazione:=objContabTestata.CodIndirizzoDestinazione,
                                                           movDesc:=objMovTestataLavCod.Mov_Desc,
                                                           dataMovimento:=objContabTestata.DataMovimento,
                                                           validitaInizio:=objContabTestata.DataMovimento,
                                                           docNumeroSin:=objContabTestata.DocNumeroSin,
                                                           Doc_Numero:=objContabTestata.DocNumero,
                                                           docNumeroDes:=objContabTestata.DocNumeroDes,
                                                           docNumeroVisualizzato:=objContabTestata.DocNumeroVisualizzato,
                                                           Num_Protocollo:=objContabTestata.TotaleDocumento,
                                                           mezzo:=objContabTestata.Mezzo,
                                                           codVettore:=objContabTestata.CodVettore,
                                                           codIndirizzoVettore:=objContabTestata.CodIndirizzoVettore,
                                                           causaleTrasporto:=objContabTestata.CausaleTrasporto,
                                                           causaleTrasportoCod:=objContabTestata.CausaleTrasportoCod,
                                                           aspetto:=objContabTestata.Aspetto,
                                                           tipoPeso:=objContabTestata.TipoPeso,
                                                           TipoDocumento:=objContabTestata.TipoDocumento,
                                                           peso:=objContabTestata.Peso,
                                                           colli:=objContabTestata.Colli,
                                                           extraStr:=objMovTestataLavCod.Extra_Str,
                                                           extraInt:=extraInt,
                                                           extraDate:=extraDate,
                                                           ora:=ora,
                                                           naturaBeni:=objContabTestata.NaturaBeni,
                                                           taraVeicolo:=objContabTestata.TaraVeicolo,
                                                           taraImballi:=objContabTestata.TaraImballi,
                                                           progrProtocollo:=objContabTestata.ProgrProtocollo,
                                                           progrRegistrazione:=objContabTestata.ProgrRegistrazione,
                                                           dataRegistrazione:=objContabTestata.DataRegistrazione,
                                                           codRisUmAltro:=objContabTestata.CodRisUmAltro,
                                                           codRisUmAggiuntivo:=objContabTestata.CodRisUmAggiuntivo,
                                                           codIndirizzoAggiuntivo:=objContabTestata.CodIndirizzoAggiuntivo,
                                                           sezionaleCod:=objContabTestata.SezionaleCod,
                                                           chkLayOutPeso:=chkLayOut_Peso,
                                                           chkLayOutPrezzo:=chkLayOut_Prezzo,
                                                           chkLayOutRiscontrato:=chkLayOut_Riscontrato,
                                                           Scadenza:=dataScadenzaPagamento,
                                                           usernameModifica:=objContabTestata.UsernameModifica)

                            If xRisp = True Then

                                Dim nDocCliente As String = Nothing
                                Dim dataDocCliente As Date? = Nothing
                                Dim nDocEnte As String = Nothing
                                Dim annoDocEnte As Integer? = Nothing

                                If UtilityHelper.IsOrdine(objContabTestata.LavCod) Then
                                    nDocCliente = If(objContabTestata.NumDocOrdineCliente, "")
                                    dataDocCliente = If(objContabTestata.DataDocOrdineCliente, AGRODATAINIZIO)
                                    nDocEnte = If(objContabTestata.NumDocOrdineEnte, "")
                                    annoDocEnte = If(objContabTestata.AnnoDocOrdineEnte, Year(AGRODATAFINE))

                                    'TODO: N_Doc_Cliente e Data_Doc_Cliente vanno ri-aggiornati anche su tutti i dettagli!!!
                                    'quindi li aggiorno per tutte le righe, a prescindere dall'id_mov (di fatto quelli della testata vengono aggiornati subito dopo)
                                    xRisp = movDetTecExW.ModificaPuntuale(objContabTestata.Piva,
                                                                      Sa_Cod:=0,
                                                                      Id_Agenda:=objContabTestata.IdAgenda,
                                                                      Id_Mov:=0,
                                                                      Id_Mov_Det:=0,
                                                                      Id_Reg_Dettaglio:=0,
                                                                      objParametri:=_objParametriServer,
                                                                      N_Doc_Cliente:=nDocCliente,
                                                                      Data_Doc_Cliente:=dataDocCliente,
                                                                      Username_Modifica:=objContabTestata.UsernameModifica)
                                End If

                                xRisp = movDetTecExW.ModificaPuntuale(objContabTestata.Piva,
                                                                  Sa_Cod:=0,
                                                                  Id_Agenda:=objContabTestata.IdAgenda,
                                                                  Id_Mov:=objContabTestata.IdMov,
                                                                  Id_Mov_Det:=0,
                                                                  Id_Reg_Dettaglio:=0,
                                                                  objParametri:=_objParametriServer,
                                                                  Agente_Cod:=objContabTestata.AgenteCod,
                                                                  Provvigione:=objContabTestata.AgenteProvvigione,
                                                                  CapoArea_Cod:=objContabTestata.CapoAreaCod,
                                                                  Provvigione_CapoArea:=objContabTestata.CapoAreaProvvigione,
                                                                  Tipo_Trasporto:=objContabTestata.ModalitaTrasporto,
                                                                  Unita_Trasporto:=objContabTestata.UnitaTrasporto,
                                                                  Id_Gestione_Vettore:=objContabTestata.GestioneVettore,
                                                                  Mac_Cod:=objContabTestata.MacCodTrasporto,
                                                                  Targa:=objContabTestata.Targa,
                                                                  Mezzo_Trasporto:=objContabTestata.DescrizioneMezzo,
                                                                  N_Immatricolazione_Rimorchio:=objContabTestata.NumImmatricolazioneRimorchio,
                                                                  N_Autorizzazione_Trasporto:=objContabTestata.NumAutorizzazioneTrasporto,
                                                                  Data_Rilascio_Autorizzazione:=objContabTestata.DataRilascioAutorizzazione,
                                                                  Peso:=objContabTestata.PesoTaraTrasporto,
                                                                  N_Doc_Cliente:=nDocCliente,
                                                                  Data_Doc_Cliente:=dataDocCliente,
                                                                  N_Doc_Ente:=nDocEnte,
                                                                  Anno_Doc_Ente:=annoDocEnte,
                                                                  Username_Modifica:=objContabTestata.UsernameModifica,
                                                                  Validita_Inizio:=objContabTestata.DataMovimento,
                                                                  DistanzaTrasportoUdm:=objContabTestata.DistanzaTrasportoUdm,
                                                                  DistanzaTrasporto:=objContabTestata.DistanzaTrasporto,
                                                                  N_Nota_Fattura:=objContabTestata.N_Nota_Fattura,
                                                                  Data_Nota_Fattura:=objContabTestata.Data_Nota_Fattura,
                                                                  N_Nota_DDT:=objContabTestata.N_Nota_DDT_Reso_SDI,
                                                                  Data_Nota_DDT:=objContabTestata.Data_Nota_DDT_Reso_SDI,
                                                                  N_Nota_Riga_DDT:=objContabTestata.N_Nota_Riga_DDT_Reso_SDI)

                            End If

                            If xRisp = True AndAlso objContabTestata.CodPagamento <> 0 Then
                                'Aggiorno il record di Pagamento

                                Dim objPagamentiHelper As New Agenda_Pagamenti_Helper 'È la classe di tipologia crud legata alla tabella Pagamento

                                'in objPagamentiHelper ho la funzione di Modifica, devo però adesso crearmi l'oggetto di tipologia Pagamento
                                'con gli id di quello salvato su database, potrei o
                                '1. farmelo passare da interfaccia, oppure
                                '2. rileggermelo al volo usando gli id della operazione, oppure
                                '3. ricrearlo qua con i soli campi che gestisco da interfaccia, al momento solo Cau_Pagamento
                                Dim pagamentodaMov As New Pagamento()
                                pagamentodaMov.Piva = objContabTestata.Piva
                                pagamentodaMov.Sa_Cod = objContabTestata.SaCod
                                pagamentodaMov.Id_Agenda = objContabTestata.IdAgenda
                                pagamentodaMov.Id_Mov = objContabTestata.IdMov
                                pagamentodaMov.Cod_Pagamento = objContabTestata.CodPagamento
                                pagamentodaMov.Cau_Pagamento = objContabTestata.ModalitaPagamento
                                pagamentodaMov.Anno = Year(dataScadenzaPagamento)

                                xRisp = objPagamentiHelper.Modifica(pagamentodaMov, _objParametriServer)
                            End If

                        End If

                        If xRisp = True Then

                            Select Case True

                                Case objContabTestata.DocumentoAccettazione IsNot Nothing

                                    xRisp = AggiornaTestataDocumentoAccettazione(objContabTestata, movHelper)

                                Case objContabTestata.DocumentoContrattoAffitto IsNot Nothing

                                    xRisp = AggiornaTestataDocumentoContrattoAffitto(objContabTestata, movHelper)

                                Case Else

                                    'TODO: se permettiamo il cambio documento in corso, potrebbe essere che prima esisteva il movimento secondario,
                                    'ma nel nuovo tipo di documento no, quindi si dovrebbe cancellare il movimento secondario
                                    'se esiste ed è incompatibile con il nuovo lav_cod

                                    'TODO: la cosa è da sistemare anche all'inverso (prima non esisteva il movimento secondario, ma con il nuovo lav_cod serve):
                                    'in questo caso andrà in errore la parte sopra perché non trova il movimento 4050, mentre dà per scontato che c'è:
                                    'gestire il caso facendo scrivere il movimento se non c'era già

                            End Select

                        End If

                    End If

                End If

            End If

            If xRisp = True Then

                'TODO: aggiornare se è cambiato Data_Movimento
                Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                objAgronicaLogAgendaW.Scrivi(objContabTestata.DataMovimento,
                                             enum_TipoOperazioneDB.Modifica,
                                             objContabTestata.DescrizioneAgenda,
                                             objContabTestata.IdAgenda,
                                             objContabTestata.Piva,
                                             0,
                                             objContabTestata.LavCod,
                                             CInt(enum_Id_Servizio.GiasOnline),
                                             _objParametriServer)

            End If


            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .Id_Agenda = objContabTestata.IdAgenda,
            .Id_Mov_Testata = objContabTestata.IdMov,
            .Id_Mov_Secondario = DeterminaIdMovSecondario(objContabTestata),
            .Doc_Numero = objContabTestata.DocNumero,
            .Doc_Numero_Visualizzato = objContabTestata.DocNumeroVisualizzato,
            .Des_Lib = objContabTestata.DescrizioneAgenda
        }

    End Function

    Private Function DeterminaIdMovSecondario(ByVal objContabTestata As Contabilita_Testata) As Integer

        Dim idMovSecondario As Integer = 0

        Select Case True

            Case objContabTestata.DocumentoAccettazione IsNot Nothing

                idMovSecondario = objContabTestata.DocumentoAccettazione.IdMov

            Case objContabTestata.DocumentoContrattoAffitto IsNot Nothing

                idMovSecondario = objContabTestata.DocumentoContrattoAffitto.IdMov

        End Select

        Return idMovSecondario

    End Function

    Private Function AggiornaTestataDocumentoAccettazione(ByVal objContabTestata As Contabilita_Testata,
                                                          ByRef movHelper As Agenda_Movimenti_Helper
                                                          ) As Boolean

        'Si tratta di un'accettazione dove esiste anche il movimento di registrazione secondario 4050

        If objContabTestata.DocumentoAccettazione.IdMov Is Nothing OrElse objContabTestata.DocumentoAccettazione.IdMov = 0 Then
            Throw New Exception("Non è presente il Movimento di Testata (Registrazione - " & CAU_REGISTRAZIONE_SECONDARIA & "). Impossibile Aggiornare")
        End If

        'TODO: ricalcolare il mov_desc
        'movDesc:=schedaBollaDes,

        Return movHelper.ModificaPuntuale(objContabTestata.Piva,
                                          0,
                                          objContabTestata.IdAgenda,
                                          objContabTestata.DocumentoAccettazione.IdMov,
                                          _objParametriServer,
                                          codRisUm:=objContabTestata.CodRisUm,
                                          Cod_IndirizzoRisUm:=objContabTestata.CodIndirizzoRisUm,
                                          Cod_Destinazione:=objContabTestata.CodDestinazione,
                                          Cod_IndirizzoDestinazione:=objContabTestata.CodIndirizzoDestinazione,
                                          docNumeroSin:=objContabTestata.DocumentoAccettazione.DocNumeroSin,
                                          Doc_Numero:=objContabTestata.DocumentoAccettazione.DocNumero,
                                          docNumeroDes:=objContabTestata.DocumentoAccettazione.DocNumeroDes,
                                          mezzo:=objContabTestata.Mezzo,
                                          codVettore:=objContabTestata.CodVettore,
                                          codIndirizzoVettore:=objContabTestata.CodIndirizzoVettore,
                                          causaleTrasporto:=objContabTestata.CausaleTrasporto,
                                          causaleTrasportoCod:=objContabTestata.CausaleTrasportoCod,
                                          aspetto:=objContabTestata.Aspetto,
                                          tipoPeso:=objContabTestata.TipoPeso,
                                          peso:=objContabTestata.Peso,
                                          colli:=objContabTestata.Colli,
                                          ora:=objContabTestata.OraSpedizione,
                                          naturaBeni:=objContabTestata.NaturaBeni,
                                          taraVeicolo:=objContabTestata.TaraVeicolo,
                                          taraImballi:=objContabTestata.TaraImballi,
                                          progrProtocollo:=objContabTestata.ProgrProtocollo,
                                          progrRegistrazione:=objContabTestata.ProgrRegistrazione,
                                          codRisUmAggiuntivo:=objContabTestata.CodRisUmAggiuntivo,
                                          codIndirizzoAggiuntivo:=objContabTestata.CodIndirizzoAggiuntivo,
                                          sezionaleCod:=objContabTestata.SezionaleCod,
                                          usernameModifica:=objContabTestata.UsernameModifica,
                                          dataMovimento:=objContabTestata.DocumentoAccettazione.DataMovimento,
                                          dataRegistrazione:=objContabTestata.DocumentoAccettazione.DataMovimento,
                                          validitaInizio:=objContabTestata.DataMovimento)

    End Function

    Private Function AggiornaTestataDocumentoContrattoAffitto(ByVal objContabTestata As Contabilita_Testata,
                                                              ByRef movHelper As Agenda_Movimenti_Helper
                                                              ) As Boolean

        'Si tratta di un contratto di affitto dove esiste anche il movimento di registrazione secondario 4050

        If objContabTestata.DocumentoContrattoAffitto.IdMov Is Nothing OrElse objContabTestata.DocumentoContrattoAffitto.IdMov = 0 Then
            Throw New Exception("Non è presente il Movimento di Testata (Registrazione - " & CAU_REGISTRAZIONE_SECONDARIA & "). Impossibile Aggiornare")
        End If

        Return movHelper.ModificaPuntuale(objContabTestata.Piva,
                                          0,
                                          objContabTestata.IdAgenda,
                                          objContabTestata.DocumentoContrattoAffitto.IdMov,
                                          _objParametriServer,
                                          docNumeroSin:=objContabTestata.DocumentoContrattoAffitto.DocNumeroSin,
                                          Doc_Numero:=objContabTestata.DocumentoContrattoAffitto.DocNumero,
                                          docNumeroDes:=objContabTestata.DocumentoContrattoAffitto.DocNumeroDes,
                                          dataMovimento:=objContabTestata.DataMovimento,
                                          dataRegistrazione:=objContabTestata.DocumentoContrattoAffitto.DataInizioValidita,
                                          Scadenza:=objContabTestata.DocumentoContrattoAffitto.DataFineValidita,
                                          movDesc:=objContabTestata.DocumentoContrattoAffitto.AltriLocatori,
                                          extraStr:=objContabTestata.DocumentoContrattoAffitto.RiferimentoOrdini)

    End Function

    Public Function ScriviTestataDocumento(ByVal objContabTestata As Contabilita_Testata,
                                           ByVal messaggioDettagliato As Boolean
                                           ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.ScriviTestataDocumento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim objAgenda As Operazione_Agenda
        Dim idAgenda As Integer = 0
        Dim idMovTestata As Integer = 0
        Dim idMovSecondario As Integer = 0
        Dim desLib As String = ""
        Dim newNumDoc As Integer = 0
        Dim newNumDocVisualizzato As String = ""
        Dim newProgrProtocollo As Integer = 0
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim flagOggettoCorretto As Boolean

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyTestataContabilita(objContabTestata, enum_TipoOperazioneDB.Scrittura, _objParametriServer)

            If flagOggettoCorretto Then

                Dim docDuplicato As Boolean = False
                Dim msgDocDuplicato As String = ""
                Dim msgErrorCoerenzaDate As String = ""

                'TODO: questo controllo è da fare sempre o solo se sono doc ricevuto oppure doc emesso + NO LOCK?!?
                'Devo verificare se nel frattempo qualcuno ha già inserito un documento con lo stesso numero!

                ' Controllo sa latare in caso di tipologia documento 28 (Acquisti da San Marino con IVA (fattura cartacea) 
                If objContabTestata.TipoDocumento <> 28 Then
                    docDuplicato = UtilityHelper.ControllaEsistenzaNumDoc(objContabTestata.LavCod,
                                                                      objContabTestata.Piva,
                                                                      objContabTestata.IdAgenda,
                                                                      objContabTestata.DocNumeroSin,
                                                                      objContabTestata.DocNumero,
                                                                      objContabTestata.DocNumeroDes,
                                                                      objContabTestata.DataMovimento,
                                                                      objContabTestata.DataRegistrazione,
                                                                      objContabTestata.SezionaleCod,
                                                                      objContabTestata.CodRisUm,
                                                                      _objParametriServer,
                                                                      _objParametriUtenti,
                                                                      msgDocDuplicato)
                End If


                'Se il documento è duplicato, è inutile che faccio le verifiche successive
                If docDuplicato = False AndAlso Not UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod) Then

                    If objContabTestata.TipoDocumento = 28 Then
                        objContabTestata.DocNumeroVisualizzato = objContabTestata.DocNumero
                    Else
                        msgErrorCoerenzaDate = UtilityHelper.AlgoritmoAssegnazioneNumDoc(newNumDoc, newNumDocVisualizzato,
                                                                                     objContabTestata.LavCod,
                                                                                     objContabTestata.Piva,
                                                                                     objContabTestata.DataMovimento,
                                                                                     objContabTestata.DocNumeroSin,
                                                                                     objContabTestata.DocNumero,
                                                                                     objContabTestata.DocNumeroDes,
                                                                                     objContabTestata.DocNumeroLock,
                                                                                     objContabTestata.DocNumeroVisualizzato,
                                                                                     _objParametriServer,
                                                                                     objContabTestata.DocNumeroLunghezza,
                                                                                     objContabTestata.DocNumeroCarattereFormattazione)

                        objContabTestata.DocNumero = newNumDoc
                        objContabTestata.DocNumeroVisualizzato = newNumDocVisualizzato

                    End If
                End If

                If docDuplicato = True Then

                    msgError &= msgDocDuplicato

                ElseIf msgErrorCoerenzaDate <> "" Then

                    msgError &= msgErrorCoerenzaDate

                Else

                    'Devo generare io l'Id_Mov perché me lo devo passare indietro (altrimenti non ho il controllo)
                    Dim objSequenze As New Agro_Sequenze
                    Dim idMovTestataTemp As Integer = objSequenze.NuovoId_Tabella("Movimenti", 0, 200000000, _objParametriServer)
                    Dim idMovSecondarioTemp As Integer = 0
                    If objContabTestata.DocumentoAccettazione IsNot Nothing OrElse
                   objContabTestata.DocumentoContrattoAffitto IsNot Nothing Then
                        idMovSecondarioTemp = objSequenze.NuovoId_Tabella("Movimenti", 0, 200000000, _objParametriServer)
                    End If

                    objAgenda = GeneraTestataContabilita(objContabTestata, idMovTestataTemp, idMovSecondarioTemp, newProgrProtocollo, _objParametriServer)

                    If Not IsNothing(objAgenda) Then

                        SeGeneraPratica(objAgenda)

                        Dim objAgendaHelper As New Agenda_Operazione_Helper
                        idAgenda = objAgendaHelper.Scrivi(objAgenda, _objParametriServer, flagUsaOraReale:=True)
                        If idAgenda > 0 Then
                            xRisp = True
                            idMovTestata = idMovTestataTemp
                            idMovSecondario = idMovSecondarioTemp
                            desLib = objAgenda.Des_Lib
                        End If
                    End If

                End If

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .Id_Agenda = idAgenda,
            .Id_Mov_Testata = idMovTestata,
            .Id_Mov_Secondario = idMovSecondario,
            .Des_Lib = desLib,
            .Doc_Numero = newNumDoc,
            .Doc_Numero_Visualizzato = newNumDocVisualizzato,
            .Progr_Protocollo = newProgrProtocollo
        }

    End Function

    Private Function GeneraTestataContabilita(ByRef objContabTestata As Contabilita_Testata,
                                              ByVal idMovTestata As Integer,
                                              ByVal idMovSecondario As Integer,
                                              ByRef newProgrProtocollo As Integer,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As Operazione_Agenda

        Const nomeRoutine = "ContabilitaHelper.GeneraTestataContabilita()"
        Dim objAgenda As Operazione_Agenda

        Try

            newProgrProtocollo = NuovoProgressivoProtocollo(objContabTestata)

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------

            objAgenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = objContabTestata.IdAgenda,
                .Data = CDate(objContabTestata.DataMovimento),
                .Piva = objContabTestata.Piva,
                .Sa_Cod = objContabTestata.SaCod,
                .Lav_Cod = objContabTestata.LavCod,
                .Des_Lib = CreaDesLib(objContabTestata.LavCod,
                                      objContabTestata.DocNumeroSin, objContabTestata.DocNumero, objContabTestata.DocNumeroDes,
                                      objContabTestata.RagSoc,
                                      accompagnatoria:=If(objContabTestata.Accompagnatoria = 1, enum_FatturaTipo.Immediata, enum_FatturaTipo.Differita),
                                      numeroDocFormattato:=objContabTestata.DocNumeroVisualizzato,
                                      progrProtocollo:=newProgrProtocollo,
                                      note:=objContabTestata.NoteIntestazione,
                                      descrizioneAggiuntiva:=objContabTestata.DescrizioneAggiuntiva),
                .Stato_Export_2 = If(objContabTestata.StatoExport2, 0),
                .ChkCoge_Manuale = objContabTestata.ContabilizzazioneManuale,
                .Modulo = If(UtilityHelper.IsAccettazione(objContabTestata.LavCod), objContabTestata.ModuloGias, 0),
                .Tipo_Accettazione = If(UtilityHelper.IsAccettazione(objContabTestata.LavCod), If(objContabTestata.TipoAccettazione = -1, -1, enum_Accettazione_Tipo.POSTcampionatura), 0),
                .Username_Creazione = objContabTestata.UsernameModifica,
                .Username_Modifica = objContabTestata.UsernameModifica,
                .Blocco_Flag = If(Not IsNothing(objContabTestata.BloccoFlag) AndAlso objContabTestata.BloccoFlag > 0, objContabTestata.BloccoFlag, 0),
                .Blocco_Data = If(Not IsNothing(objContabTestata.BloccoData) AndAlso objContabTestata.BloccoData <> AGRODATAINIZIO, objContabTestata.BloccoData, AGRODATAINIZIO),
                .Blocco_Username = If(Not String.IsNullOrEmpty(objContabTestata.BloccoUsername), objContabTestata.BloccoUsername, "")
            }

            '------------------------------------------------
            '----- PAGAMENTI
            '------------------------------------------------
            Dim ObjPagamento As New Pagamento
            Dim Data_Scadenza As DateTime = AGRODATAFINE
            Dim bPagamento_Impostato As Boolean = False

            'Prima di generare il movimento principale creo il pagamento in modo da avere la data di scadenza
            Select Case objContabTestata.LavCod

                Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                    Dim utilityHelperPagamento As New AgronicaCoreModello.UtilityHelper
                    ObjPagamento = utilityHelperPagamento.Impostazione_Modalita_Pagamento(objContabTestata.Piva,
                                                                                            "",
                                                                                            objContabTestata.CodRisUm,
                                                                                            objContabTestata.LavCod,
                                                                                            objContabTestata.ModalitaPagamento,
                                                                                            0,
                                                                                            0,
                                                                                            0,
                                                                                            objContabTestata.DataMovimento,
                                                                                            Data_Scadenza,
                                                                                            objParametri_Server)

                    bPagamento_Impostato = True

                Case Else


            End Select


            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------

            'MOVIMENTO DI TESTATA

            Dim objMovimentoT As Movimento = Nothing

            CaricaMovimentoTestata(objMovimentoT,
                                   objContabTestata,
                                   objAgenda,
                                   idMovTestata,
                                   newProgrProtocollo)

            SetLayoutFormatiStampa(objMovimentoT.ChkLayOut_Peso,
                                   objMovimentoT.ChkLayOut_Prezzo,
                                   objMovimentoT.ChkLayOut_Riscontrato,
                                   objContabTestata.Layout_FormatiStampa)

            Select Case True

                Case UtilityHelper.IsAccettazione(objContabTestata.LavCod)
                    objMovimentoT.Extra_Int = objContabTestata.SecondaCooperativa
                    objMovimentoT.Ora = objContabTestata.OraSpedizione

                Case UtilityHelper.IsFattura(objContabTestata.LavCod), UtilityHelper.IsNotaAccredito(objContabTestata.LavCod)
                    objMovimentoT.Extra_Int = objContabTestata.Accompagnatoria
                    objMovimentoT.Ora = objContabTestata.OraSpedizione

                Case UtilityHelper.IsOrdine(objContabTestata.LavCod)
                    objMovimentoT.Extra_Int = CInt(SetScadenzaOrdine(objContabTestata.ScadenzaUnica, objContabTestata.EvasioneTassativa))
                    objMovimentoT.Extra_Date = If(objContabTestata.DataEvasionePrevista, AGRODATAFINE)
                    objMovimentoT.Ora = If(objContabTestata.DataSpedizionePrevista, AGRODATAFINE)

                Case UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod)
                    objMovimentoT.Ora = AGRODATAFINE

                Case Else
                    objMovimentoT.Ora = objContabTestata.OraSpedizione
            End Select


            If bPagamento_Impostato Then

                'Imposto la Data di Scadenza
                objMovimentoT.Scadenza = Data_Scadenza

                'Aggancio il pagamento al movimento 
                objMovimentoT.Pagamenti.Add(ObjPagamento)

            End If


            '--------------------------------------------------
            'MOVIMENTO DI TESTATA SECONDARIO (4050)
            '--------------------------------------------------
            'Usato per:
            '1. Accettazione
            '2. Contratti Affitto
            '--------------------------------------------------

            Dim objMovimentoTSec As Movimento = Nothing

            If objContabTestata.DocumentoAccettazione IsNot Nothing Then

                CaricaMovimentoTestataSecondarioAccettazione(objMovimentoTSec,
                                                             objContabTestata,
                                                             objAgenda,
                                                             idMovSecondario)

            End If

            If objContabTestata.DocumentoContrattoAffitto IsNot Nothing Then

                CaricaMovimentoTestataSecondarioContrattoAffitto(objMovimentoTSec,
                                                                 objContabTestata,
                                                                 objAgenda,
                                                                 idMovSecondario)

            End If

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO EXTRA TESTATA
            '------------------------------------------------

            If Not UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod) AndAlso
               Not UtilityHelper.IsContrattoAffitto(objContabTestata.LavCod) Then

                CaricaMovimentoTecnicoExtraTestata(objMovimentoT,
                                                   objContabTestata)

            End If

            'Aggancio i movimenti sull'agenda
            objAgenda.Movimenti = New List(Of Movimento) From {
                objMovimentoT
            }

            If objMovimentoTSec IsNot Nothing Then
                objAgenda.Movimenti.Add(objMovimentoTSec)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objAgenda

    End Function

    Private Function NuovoProgressivoProtocollo(ByVal objContabTestata As Contabilita_Testata) As Integer

        Dim newProgrProtocollo As Integer = objContabTestata.ProgrProtocollo

        If newProgrProtocollo = 0 Then
            Dim objContab As New Contabilita_R
            newProgrProtocollo = objContab.NuovoProgressivoLavCod(objContabTestata.LavCod,
                                                                  objContabTestata.Piva,
                                                                  Year(CDate(objContabTestata.DataMovimento)),
                                                                  objContabTestata.SezionaleCod,
                                                                  0,
                                                                  _objParametriServer)
        End If

        Return newProgrProtocollo

    End Function



    Private Sub CaricaMovimentoTestata(ByRef objMovimentoT As Movimento,
                                       ByRef objContabTestata As Contabilita_Testata,
                                       ByRef objAgenda As Operazione_Agenda,
                                       ByVal idMovTestata As Integer,
                                       ByVal newProgrProtocollo As Integer)

        Dim objMovTestataLavCod = ModellaDatiTestataDaLavCod(objContabTestata)

        'If objContabTestata.ProgrRegistrazione = 0 Then
        '    Dim handleSequenzaProg As New Sequenza_Progressivi_R

        '    objContabTestata.ProgrRegistrazione = handleSequenzaProg.Nuovo_Progressivo_UpdateImmediato(objContabTestata.Piva,
        '        objContabTestata.DataMovimento.Value.Year,
        '        14, 'TODO Verificare se esiste un enumerativo per questo valore, lo imposta fisso nel javascript
        '        objContabTestata.DocNumeroSin,
        '        objContabTestata.DocNumeroDes,
        '        objContabTestata.SezionaleCod,
        '        _objParametriServer)
        'End If

        objMovimentoT = New Movimento With {
            .Piva = objAgenda.Piva,
            .Sa_Cod = objContabTestata.SaCod,
            .Id_Agenda = objAgenda.Id_Agenda,
            .Id_Mov = idMovTestata,
            .Lav_Cod = objAgenda.Lav_Cod,
            .Cau_Mov = objMovTestataLavCod.Cau_Mov,
            .Mov_Desc = objMovTestataLavCod.Mov_Desc,
            .Doc_Numero_Sin = objContabTestata.DocNumeroSin,
            .Doc_Numero = objContabTestata.DocNumero,
            .Doc_Numero_Des = objContabTestata.DocNumeroDes,
            .Doc_Numero_Visualizzato = objContabTestata.DocNumeroVisualizzato,
            .Data = objAgenda.Data,
            .Extra_Str = objMovTestataLavCod.Extra_Str,
            .Data_Registrazione = objContabTestata.DataRegistrazione,
            .Progr_Registrazione = objContabTestata.ProgrRegistrazione,
            .Progr_Protocollo = newProgrProtocollo,
            .Sezionale_Cod = objContabTestata.SezionaleCod,
            .Cod_Risum = objContabTestata.CodRisUm,
            .Cod_IndirizzoRisUm = objContabTestata.CodIndirizzoRisUm,
            .Cod_Destinazione = objContabTestata.CodDestinazione,
            .Cod_IndirizzoDestinazione = objContabTestata.CodIndirizzoDestinazione,
            .Cod_RisUm_Aggiuntivo = objContabTestata.CodRisUmAggiuntivo,
            .Cod_Indirizzo_Aggiuntivo = objContabTestata.CodIndirizzoAggiuntivo,
            .Cod_Vettore = objContabTestata.CodVettore,
            .Cod_IndirizzoVettore = objContabTestata.CodIndirizzoVettore,
            .Aspetto = objContabTestata.Aspetto,
            .Natura_Beni = objContabTestata.NaturaBeni,
            .Causale_Trasporto = objContabTestata.CausaleTrasporto,
            .Causale_Trasporto_Cod = objContabTestata.CausaleTrasportoCod,
            .Colli = objContabTestata.Colli,
            .Tipo_Peso = objContabTestata.TipoPeso,
            .Peso = objContabTestata.Peso,
            .Ora = AGRODATAFINE,
            .Mezzo = objContabTestata.Mezzo,
            .Tara_Veicolo = objContabTestata.TaraVeicolo,
            .Tara_Imballi = objContabTestata.TaraImballi,
            .Cod_RisUm_Altro = objContabTestata.CodRisUmAltro,
            .Num_Protocollo_Decimal = CDec(objContabTestata.TotaleDocumento),
            .TipoDocumento = IIf(IsNumeric(objContabTestata.TipoDocumento), objContabTestata.TipoDocumento, 0),
            .Username_Creazione = objContabTestata.UsernameModifica,
            .Username_Modifica = objContabTestata.UsernameModifica
        }

    End Sub

    Private Function ModellaDatiTestataDaLavCod(objContabTestata As Contabilita_Testata) As Movimento_Testata_LavCod

        Dim objMovTestataLavCod As New Movimento_Testata_LavCod

        objMovTestataLavCod.Cau_Mov = CauMovDaLavCod(objContabTestata.LavCod)

        If UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod) Then
            objMovTestataLavCod.Mov_Desc = objContabTestata.NoteIntestazione
            objMovTestataLavCod.Extra_Str = ""
        Else
            objMovTestataLavCod.Mov_Desc = ""
            objMovTestataLavCod.Extra_Str = objContabTestata.NoteIntestazione
        End If

        Return objMovTestataLavCod

    End Function

    Private Function CauMovDaLavCod(lavCod As Integer) As String

        Dim cauMov As String

        Select Case lavCod
            Case LAVCOD_CARICO
                cauMov = CAU_CARICO
            Case LAVCOD_SCARICO
                cauMov = CAU_SCARICO
            Case Else
                cauMov = CAU_REGISTRAZIONI
        End Select

        Return cauMov

    End Function

    Private Sub CaricaMovimentoTestataSecondarioAccettazione(ByRef objMovimentoTSec As Movimento,
                                                             ByRef objContabTestata As Contabilita_Testata,
                                                             ByRef objAgenda As Operazione_Agenda,
                                                             ByVal idMovSecondario As Integer)

        Dim lavCodAssociato As Integer = 0
        Dim schedaBollaDes As String = CreaDescDocAccettazione(objAgenda.Lav_Cod,
                                                               objContabTestata,
                                                               lavCodAssociato)

        objMovimentoTSec = New Movimento With {
            .Piva = objAgenda.Piva,
            .Sa_Cod = 0,
            .Id_Agenda = objAgenda.Id_Agenda,
            .Id_Mov = idMovSecondario,
            .Lav_Cod = objAgenda.Lav_Cod,
            .Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA,
            .Mov_Desc = schedaBollaDes,
            .Doc_Numero_Sin = objContabTestata.DocumentoAccettazione.DocNumeroSin,
            .Doc_Numero = objContabTestata.DocumentoAccettazione.DocNumero,
            .Doc_Numero_Des = objContabTestata.DocumentoAccettazione.DocNumeroDes,
            .Data = objContabTestata.DocumentoAccettazione.DataMovimento,
            .Data_Registrazione = objContabTestata.DocumentoAccettazione.DataMovimento,
            .Progr_Registrazione = objContabTestata.ProgrRegistrazione,
            .Progr_Protocollo = objContabTestata.ProgrProtocollo,
            .Sezionale_Cod = objContabTestata.SezionaleCod,
            .Cod_Risum = objContabTestata.CodRisUm,
            .Cod_IndirizzoRisUm = objContabTestata.CodIndirizzoRisUm,
            .Cod_Destinazione = objContabTestata.CodDestinazione,
            .Cod_IndirizzoDestinazione = objContabTestata.CodIndirizzoDestinazione,
            .Cod_RisUm_Aggiuntivo = objContabTestata.CodRisUmAggiuntivo,
            .Cod_Indirizzo_Aggiuntivo = objContabTestata.CodIndirizzoAggiuntivo,
            .Cod_Vettore = objContabTestata.CodVettore,
            .Cod_IndirizzoVettore = objContabTestata.CodIndirizzoVettore,
            .Aspetto = objContabTestata.Aspetto,
            .Natura_Beni = objContabTestata.NaturaBeni,
            .Causale_Trasporto = objContabTestata.CausaleTrasporto,
            .Causale_Trasporto_Cod = objContabTestata.CausaleTrasportoCod,
            .Colli = objContabTestata.Colli,
            .Tipo_Peso = objContabTestata.TipoPeso,
            .Peso = objContabTestata.Peso,
            .Ora = objContabTestata.OraSpedizione,
            .Mezzo = objContabTestata.Mezzo,
            .Tara_Veicolo = objContabTestata.TaraVeicolo,
            .Tara_Imballi = objContabTestata.TaraImballi,
            .Cod_RisUm_Altro = 0,
            .Num_Protocollo_Decimal = 0,
            .Username_Creazione = objContabTestata.UsernameModifica,
            .Username_Modifica = objContabTestata.UsernameModifica
        }

    End Sub

    Private Sub CaricaMovimentoTestataSecondarioContrattoAffitto(ByRef objMovimentoTSec As Movimento,
                                                                 ByRef objContabTestata As Contabilita_Testata,
                                                                 ByRef objAgenda As Operazione_Agenda,
                                                                 ByVal idMovSecondario As Integer)

        objMovimentoTSec = New Movimento With {
            .Piva = objAgenda.Piva,
            .Sa_Cod = 0,
            .Id_Agenda = objAgenda.Id_Agenda,
            .Id_Mov = idMovSecondario,
            .Lav_Cod = objAgenda.Lav_Cod,
            .Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA,
            .Mov_Desc = objContabTestata.DocumentoContrattoAffitto.AltriLocatori,
            .Extra_Str = objContabTestata.DocumentoContrattoAffitto.RiferimentoOrdini,
            .Doc_Numero_Sin = objContabTestata.DocumentoContrattoAffitto.DocNumeroSin,
            .Doc_Numero = objContabTestata.DocumentoContrattoAffitto.DocNumero,
            .Doc_Numero_Des = objContabTestata.DocumentoContrattoAffitto.DocNumeroDes,
            .Data = objContabTestata.DataMovimento,
            .Data_Registrazione = objContabTestata.DocumentoContrattoAffitto.DataInizioValidita,
            .Scadenza = objContabTestata.DocumentoContrattoAffitto.DataFineValidita,
            .Username_Creazione = objContabTestata.UsernameModifica,
            .Username_Modifica = objContabTestata.UsernameModifica
        }


    End Sub

    Private Sub CaricaMovimentoTecnicoExtraTestata(ByRef objMovimentoT As Movimento,
                                                   ByRef objContabTestata As Contabilita_Testata)

        Dim objMovTecExtraT = New Movimento_Dettaglio_Tecnico_Extra With {
            .Piva = objMovimentoT.Piva,
            .Sa_Cod = objContabTestata.SaCod,
            .Id_Agenda = objMovimentoT.Id_Agenda,
            .Id_Mov = objMovimentoT.Id_Mov,
            .Id_Mov_Det = 0,
            .Id_Reg_Dettaglio = 0,
            .Validita_Inizio = objMovimentoT.Data,
            .Agente_Cod = objContabTestata.AgenteCod,
            .Provvigione = objContabTestata.AgenteProvvigione,
            .CapoArea_Cod = objContabTestata.CapoAreaCod,
            .Provvigione_CapoArea = objContabTestata.CapoAreaProvvigione,
            .Tipo_Trasporto = objContabTestata.ModalitaTrasporto,
            .Unita_Trasporto = objContabTestata.UnitaTrasporto,
            .Id_Gestione_Vettore = objContabTestata.GestioneVettore,
            .Mac_Cod = objContabTestata.MacCodTrasporto,
            .Targa = objContabTestata.Targa,
            .Mezzo_Trasporto = objContabTestata.DescrizioneMezzo,
            .N_Immatricolazione_Rimorchio = objContabTestata.NumImmatricolazioneRimorchio,
            .N_Autorizzazione_Trasporto = objContabTestata.NumAutorizzazioneTrasporto,
            .Data_Rilascio_Autorizzazione = objContabTestata.DataRilascioAutorizzazione,
            .Peso = objContabTestata.PesoTaraTrasporto,
            .DistanzaTrasportoUdm = objContabTestata.DistanzaTrasportoUdm,
            .DistanzaTrasporto = objContabTestata.DistanzaTrasporto,
            .N_Nota_Fattura = objContabTestata.N_Nota_Fattura,
            .Data_Nota_Fattura = If(objContabTestata.Data_Nota_Fattura, AGRODATAINIZIO),
            .N_Nota_DDT = objContabTestata.N_Nota_DDT_Reso_SDI,
            .Data_Nota_DDT = If(objContabTestata.Data_Nota_DDT_Reso_SDI, AGRODATAINIZIO),
            .N_Nota_Riga_DDT = objContabTestata.N_Nota_Riga_DDT_Reso_SDI,
            .Username_Creazione = objContabTestata.UsernameModifica,
            .Username_Modifica = objContabTestata.UsernameModifica
        }

        If UtilityHelper.IsOrdine(objContabTestata.LavCod) Then
            objMovTecExtraT.N_Doc_Cliente = If(objContabTestata.NumDocOrdineCliente, "")
            objMovTecExtraT.Data_Doc_Cliente = If(objContabTestata.DataDocOrdineCliente, AGRODATAINIZIO)
            objMovTecExtraT.N_Doc_Ente = If(objContabTestata.NumDocOrdineEnte, "")
            objMovTecExtraT.Anno_Doc_Ente = If(objContabTestata.AnnoDocOrdineEnte, Year(AGRODATAFINE))
        End If

        'Aggancio il movimento dettaglio tecnico extra sul movimento di testata
        objMovimentoT.Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra) From {
            objMovTecExtraT
        }

    End Sub

    Private Sub SeGeneraPratica(ByRef objAgenda As Operazione_Agenda)

        Const nomeRoutine = "ContabilitaHelper.SeGeneraPratica()"

        Dim objImpImp As New Imprese_Impostazioni_R

        Dim dizionarioWorkflowDoc = objImpImp.LeggiDizionario_WorkflowDocContabili(objAgenda.Piva, _objParametriServer)

        If objImpImp.LavCod_Gestisce_Workflow(objAgenda.Lav_Cod, dizionarioWorkflowDoc) Then

            Dim r As RispostaStandard

            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            Dim servizioCod = objImpImp.Ottieni_ServizioCod_Da_LavCod(objAgenda.Lav_Cod, dizionarioWorkflowDoc)

            r = objPratiche.GeneraPratica(objAgenda.Piva,
                                          servizioCod,
                                          "",
                                          "",
                                          objAgenda.Data.ToString(),
                                          objAgenda.Des_Lib,
                                          _objParametriServer,
                                          _objParametriUtenti,
                                          ControllaEsistenzaPratica:=False,
                                          Pratica_Cod_Inserita:=objAgenda.Pratica_Cod)

            If Not r.RispostaOK And Not String.IsNullOrEmpty(r.Errore) Then
                Throw New Exception("[ " & nomeRoutine & " ] : " & r.Errore)
            End If

        End If

    End Sub

    Public Function SalvaTestataPiuRiga(ByVal objContabTestata As Contabilita_Testata,
                                        ByVal cauMov As String,
                                        ByVal idMovCS As Integer,
                                        ByVal idMovDet As Integer,
                                        ByVal messaggioDettagliato As Boolean,
                                        Optional ByVal objContabDettaglio As Contabilita_Riga = Nothing
                                        ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.SalvaTestataPiuRiga()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False

        Dim outputTestata As New Contabilita_Output
        Dim outputDettaglio As Contabilita_Output = Nothing

        Dim operazioneTestata As enum_TipoOperazioneDB
        Dim operazioneDettagli As enum_TipoOperazioneDB

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'devo capire se la testata è in aggiornamento o in scrittura
            If objContabTestata.IdAgenda = 0 OrElse objContabTestata.IdMov = 0 Then

                operazioneTestata = enum_TipoOperazioneDB.Scrittura
                outputTestata = ScriviTestataDocumento(objContabTestata, messaggioDettagliato)

                'se ok, devo impostare correttamente tutte le chiavi, in modo che funzioni tutto bene nel dettaglio
                If outputTestata.Risultato = True Then
                    objContabTestata.IdAgenda = outputTestata.Id_Agenda
                    objContabTestata.IdMov = outputTestata.Id_Mov_Testata

                    If outputTestata.Id_Mov_Secondario <> 0 Then

                        Select Case True

                            Case objContabTestata.DocumentoAccettazione IsNot Nothing
                                objContabTestata.DocumentoAccettazione.IdMov = outputTestata.Id_Mov_Secondario

                            Case objContabTestata.DocumentoContrattoAffitto IsNot Nothing
                                objContabTestata.DocumentoContrattoAffitto.IdMov = outputTestata.Id_Mov_Secondario

                        End Select

                    End If

                    'se movimento magazzino, il movimento di testata creato è già quello di carico/scarico
                    If UtilityHelper.IsMovimentoMagazzino(objContabTestata.LavCod) Then
                        idMovCS = outputTestata.Id_Mov_Testata
                    End If

                End If

            Else

                operazioneTestata = enum_TipoOperazioneDB.Modifica
                outputTestata = AggiornaTestataDocumento(objContabTestata, messaggioDettagliato)

            End If

            If outputTestata.Risultato = True Then

                Dim helperDettaglio As New ContabilitaHelper_Dettaglio(_objParametriServer, _objParametriUtenti)

                'devo capire se il dettaglio è in aggiornamento o in scrittura
                If objContabDettaglio IsNot Nothing AndAlso idMovDet > 0 Then

                    'Sono in modifica riga

                    'Determino i riferimento con lav_cod_rif 4700
                    Dim i As Integer = 0
                    Dim Id_Agenda_GHG As Integer = 0
                    Do While i < objContabDettaglio.ListRifMovDettaglio.Count
                        If objContabDettaglio.ListRifMovDettaglio(i).Lav_Cod_Rif = LAVCOD_GHG Then
                            If objContabDettaglio.ListRifMovDettaglio(i).Id_Agenda_Rif <> 0 Then
                                Id_Agenda_GHG = objContabDettaglio.ListRifMovDettaglio(i).Id_Agenda_Rif
                            End If
                        End If
                        i = i + 1
                    Loop







                    operazioneDettagli = enum_TipoOperazioneDB.Modifica
                    outputDettaglio = helperDettaglio.AggiornaSingolaRigaDocumento(objContabDettaglio,
                                                                                   objContabTestata.LavCod,
                                                                                   cauMov,
                                                                                   objContabTestata.IdMov,
                                                                                   idMovCS,
                                                                                   objContabTestata.DataMovimento,
                                                                                   objContabTestata.UsernameModifica,
                                                                                   messaggioDettagliato,
                                                                                   objContabTestata,
                                                                                   Id_Agenda_GHG)

                    xRisp = outputDettaglio.Risultato

                ElseIf objContabDettaglio IsNot Nothing AndAlso idMovDet = 0 Then

                    'Sono in nuova riga

                    'potrebbe essere che ho dovuto scrivere anche la testata, in questo caso devo impostare anche le chiavi corrette
                    If operazioneTestata = enum_TipoOperazioneDB.Scrittura Then
                        objContabDettaglio.IdAgenda = objContabTestata.IdAgenda
                    End If

                    operazioneDettagli = enum_TipoOperazioneDB.Scrittura
                    outputDettaglio = helperDettaglio.ScriviSingolaRigaDocumento(objContabDettaglio,
                                                                                 objContabTestata.LavCod,
                                                                                 cauMov,
                                                                                 objContabTestata.IdMov,
                                                                                 idMovCS,
                                                                                 objContabTestata.DataMovimento,
                                                                                 objContabTestata.UsernameModifica,
                                                                                 messaggioDettagliato,
                                                                                 objContabTestata)

                    xRisp = outputDettaglio.Risultato

                Else
                    operazioneDettagli = enum_TipoOperazioneDB.Lettura
                    xRisp = True
                End If

            End If

            If xRisp = True Then

                'TODO: Loggo solo una volta in Agronica_Log_Agenda

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        'TODO: l'output deve essere un'insieme dei 2

        Dim outputTotale As New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = outputTestata.MsgError,
            .Id_Agenda = objContabTestata.IdAgenda,
            .Id_Mov_Testata = objContabTestata.IdMov,
            .Id_Mov_Secondario = DeterminaIdMovSecondario(objContabTestata),
            .Des_Lib = outputTestata.Des_Lib,
            .Doc_Numero = outputTestata.Doc_Numero,
            .Doc_Numero_Visualizzato = outputTestata.Doc_Numero_Visualizzato,
            .Progr_Protocollo = outputTestata.Progr_Protocollo
        }

        If outputDettaglio IsNot Nothing Then
            outputTotale.MsgError = Trim(outputTotale.MsgError & " " & outputDettaglio.MsgError)
            outputTotale.MovimentoMagazzino = outputDettaglio.MovimentoMagazzino
            outputTotale.Id_Mov_Carico = outputDettaglio.Id_Mov_Carico
            outputTotale.Id_Mov_Scarico = outputDettaglio.Id_Mov_Scarico
            outputTotale.Id_Mov_Det = outputDettaglio.Id_Mov_Det
            outputTotale.ConteggiTotali = outputDettaglio.ConteggiTotali
        End If

        Return outputTotale

    End Function

    Private Sub GetDocumentiCollegati(ByVal piva As String, ByVal idAgenda As Integer, ByVal lavCod As Integer,
                                      ByRef LblRiferimento As String, ByRef LblAllegato As String)

        Dim objRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim dtRif As DataTable = objRifR.Recupera_DT_Rif_Unificato(piva, 0, idAgenda, 0, 0, lavCod, "", _objParametriServer)

        If dtRif IsNot Nothing AndAlso dtRif.Rows.Count > 0 Then

            Dim Id_Agenda_Allegato As Integer = 0
            Dim Id_Agenda_Conf_Allegato As Integer = 0

            Dim Id_Trapianto_Allegato As Integer = 0
            Dim mId_Agenda_Riferimento As Integer = 0

            For Each drRif As DataRow In dtRif.Rows

                Select Case CInt(drRif.Item("Lav_Cod_Risultato"))

                    Case LAVCOD_MOV_FINANZIARIO 'Contabilizzazione

                        'If Not mbCopia_Incolla Then

                        '    mId_Agenda_CoGe = CInt(drRif.Item("Id_Agenda_Risultato"))
                        '    mData_Creazione_Agenda_Coge = Format(drRif.Item("Data_Creazione"), "dd/mm/yyyy HH:MM")

                        '    'Cancellazione Riferimento CoGe Manuale
                        '    ReDim Preserve mRiferimenti(0 To 6, 0 To UBound(mRiferimenti, 2) + 1)

                        '    mRiferimenti(0, UBound(mRiferimenti, 2) - 1) = piva
                        '    mRiferimenti(1, UBound(mRiferimenti, 2) - 1) = mId_Agenda_CoGe
                        '    mRiferimenti(2, UBound(mRiferimenti, 2) - 1) = 0
                        '    mRiferimenti(3, UBound(mRiferimenti, 2) - 1) = 0

                        '    If Not mbContab_Auto Then

                        '        FormFinanziario.FormInizializza(piva, mRag_Soc, 0, "", 1032, mId_Agenda_CoGe, DbSalva, , , False, mbContab_Auto, idAgenda, mLav_Cod, SchedaDes, mCod_RisUm, TxtImporto.text, mDatiCoGe)

                        '        mId_Agenda_CoGe = FormFinanziario.Id_Agenda
                        '        mData_Coge = FormFinanziario.Data_Registrazione
                        '        mImporto_CoGe = Format(FormFinanziario.Importo_Riferimento, "###,###,###,##0.00")
                        '        mCod_RisUm_CoGe = FormFinanziario.Cod_RisUm_CoGe
                        '        mDatiCoGe = FormFinanziario.FormPreparaDati(DbSalva, mData_Creazione_Agenda_Coge)

                        '    End If

                        '    ChkCoge.value = ssChecked

                        'End If

                    Case LAVCOD_RACCOLTA

                        mId_Agenda_Riferimento = CInt(drRif.Item("Id_Agenda_Risultato"))

                        'Evidenzio il rilievo produzione
                        LblRiferimento &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))


                    Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO

                        'Evito i Duplicati
                        If Id_Trapianto_Allegato = 0 Or Id_Trapianto_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then

                            Id_Trapianto_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))

                            'Evidenzio la semina
                            LblRiferimento &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))

                            'If Stato_Corrente = DBmodifica Then
                            '    MsgBox("Attenzione. La fattura è associata al " & GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato")) &
                            '           "La modifica del documento comporta la perdita dell'associazione 'trapianto-fattura'.", vbInformation)
                            'End If

                        End If

                    Case LAVCOD_SCARICO, LAVCOD_CARICO 'Scarico Conferimento

                        'Do nothing--> scarico relativo alla raccolta (vedi sopra)

                    Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI,
                        LAVCOD_DOCO_EMESSO, LAVCOD_DOCO_RICEVUTO,
                        LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO,
                        LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                        LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                        LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                        'Bolla/Accettazione/Doco/MVV/Auto DDT Conferimento

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 OrElse Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        End If

                    Case LAVCOD_FATTURA_PROFORMA

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 OrElse Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        End If

                        'Impostazione Id_AgendaProforma
                        'mId_AgendaProForma = drRif.Item("Id_Agenda_Risultato")

                    Case LAVCOD_FATTURA_EMESSA

                        'Select Case Modalita

                        '    Case DOCO, MVV

                        '        'Evito i Duplicati
                        '        If Id_Agenda_Allegato = 0 Or Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then

                        '            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                        '            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))

                        '            If Stato_Corrente = DBmodifica Then

                        '                If Not mbModalita_Protetta Then

                        '                    If cCar.SMS(GiasSoftware, "Attenzione. il " & mStrLav & " è associato alla " & drRif.Item("Des_Lib_Risultato") & " del " & drRif.Item("Validita_Inizio_Risultato") & "." & Chr(13) &
                        '                                "Verrà consentita la modifica del documento in modalità protetta." & Chr(13) &
                        '                                "Si desidera visualizzare maggiori dettagli sulla modalità protetta?", vbYesNo) = vbYes Then

                        '                        cCar.FormVisualizzaLimitazioniModalitaProtetta

                        '                    End If

                        '                End If

                        '            End If

                        '            mbModalita_Protetta = True
                        '            bDettaglio_Protetto = True

                        '        End If

                        '    Case DAA

                        '        'Evito i Duplicati
                        '        If Id_Agenda_Allegato = 0 Or Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                        '            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                        '            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        '        End If

                        'End Select

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 OrElse Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        End If


                    Case LAVCOD_DAA_EMESSO

                        'mId_Agenda_DAA = CInt(drRif.Item("Id_Agenda_Risultato"))

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 Or Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then

                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))

                            'If Stato_Corrente = DBmodifica Then

                            '    If Not mbModalita_Protetta Then

                            '        If cCar.SMS(GiasSoftware, "Attenzione. La fattura commerciale è associata al " & drRif.Item("Des_Lib_Risultato") & " del " & drRif.Item("Validita_Inizio_Risultato") & "." & Chr(13) &
                            '                    "Verrà consentita la modifica del documento in modalità protetta." & Chr(13) &
                            '                    "Si desidera visualizzare maggiori dettagli sulla modalità protetta?", vbYesNo) = vbYes Then

                            '            cCar.FormVisualizzaLimitazioniModalitaProtetta()
                            '        End If

                            '    End If

                            'End If

                            'mbModalita_Protetta = True
                            'bDettaglio_Protetto = True

                        End If

                    Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI 'Bolla Conferimento Allegata

                        'Evito i Duplicati
                        If Id_Agenda_Conf_Allegato = 0 Or Id_Agenda_Conf_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then

                            Id_Agenda_Conf_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))

                            ''Evidenzio la raccolta associata alla bolla di conferimento
                            'RsRiferimento = cMagazzino.ListaRiferimentiNew(drRif.Item("Piva_Risultato"), , CInt(drRif.Item("Id_Agenda_Risultato")))

                            'If RsRiferimento.State <> 0 Then

                            '                   'filtro le raccolte
                            '                   RsRiferimento.Filter = "Lav_Cod_Risultato = 125"

                            '                   Do While Not RsRiferimento.EOF
                            '                       'Evidenzio il rilievo produzione
                            '                       LblRiferimento &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                            '                       RsRiferimento.MoveNext
                            '                   Loop

                            '               End If

                        End If

                    Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 Or Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        End If

                    Case LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI

                        'Evito i Duplicati
                        If Id_Agenda_Allegato = 0 Or Id_Agenda_Allegato <> CInt(drRif.Item("Id_Agenda_Risultato")) Then
                            Id_Agenda_Allegato = CInt(drRif.Item("Id_Agenda_Risultato"))
                            LblAllegato &= GetDesDocCollegato(drRif.Item("Des_Lib_Risultato"), drRif.Item("Validita_Inizio_Risultato"))
                        End If

                    Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA 'Note

                        '#############################################################################
                        '############################ NOTA CREDITO/DEBITO ############################
                        '#############################################################################

                        '      Select Case mStato_Nota

                        '          Case 1 'Associazione multi Nota di Accredito al Cliente

                        '              mId_Agenda_Nota = 0 'Nuova nota
                        '              mId_Mov_Registrazione_Nota = 0
                        '              mId_Mov_Abbuono_Nota = 0
                        '              mId_Mov_Nota = 0

                        '          Case Else

                        '              'Controllo che la nota sia quella di ingresso oppure che si stia visualizzando/modificando la fattura (mId_Agenda_Nota = 0)
                        '              If mId_Agenda_Nota = CInt(drRif.Item("Id_Agenda_Risultato")) Or mId_Agenda_Nota = 0 Then

                        '                  mConsenti_Modifica = True
                        '                  TbDettagli.Tools("Id_Nuovo").enabled = mConsenti_Modifica

                        '                  If Not bTool_Nuova_Consistenza_Removed Then
                        '                      TbDettagli.Tools("Id_Nuova_Consistenza").enabled = mConsenti_Modifica
                        '                  End If


                        '                  If Stato_Corrente = DBmodifica Then

                        '                      If Not mbModalita_Protetta Then

                        '                          If cCar.SMS(GiasSoftware, "Attenzione. La fattura commerciale è associata alla " & drRif.Item("Des_Lib_Risultato") & " del " & drRif.Item("Validita_Inizio_Risultato") & "." & Chr(13) &
                        '                                      "Verrà consentita la modifica del documento in modalità protetta." & Chr(13) &
                        '                                      "Si desidera visualizzare maggiori dettagli sulla modalità protetta?", vbYesNo) = vbYes Then

                        '                              cCar.FormVisualizzaLimitazioniModalitaProtetta

                        '                          End If

                        '                      End If

                        '                      mbModalita_Protetta = True
                        '                      bDettaglio_Protetto = True

                        '                  End If

                        '                  TbDettagli.Tools("Id_Cancella").enabled = mConsenti_Modifica
                        '                  TbDettagli.Tools("Id_Associazione").enabled = mConsenti_Modifica And Causale_Associazione
                        '                  TbDettagli.Tools("Id_Allega_Ordine_Vendita").enabled = mConsenti_Modifica And Causale_Ordini
                        '                  TbDettagli.Tools("Id_Fatturazione_Riepilogativa").enabled = mConsenti_Modifica And Causale_Associazione
                        '                  TbDettagli.Tools("Id_Accettazione").enabled = mConsenti_Modifica And Causale_Accettazione
                        '                  TbDettagli.Tools("Id_LiquidazioneSoci").enabled = mConsenti_Modifica And Causale_Accettazione 'vanni, ls

                        '                  mId_Agenda_Nota = drRif.Item("Id_Agenda_Risultato")

                        '                  '#################################################################
                        '                  '#########################  Lettura Nota   #######################
                        '                  '#################################################################

                        '                  Dim DatiNota = ObjFattura.Movimento_Leggi(CStr(piva),
                        '                                                        CLng(mSa_Cod),
                        '                                                        CLng(mId_Agenda_Nota),
                        '                                                        ,
                        '                                                        ,
                        '                                                        ,
                        '                                                        ,
                        '                                                        ,
                        '                                                        objConnessione,
                        '                                                        ,
                        '                                                        ConnessioneAlternativa)

                        '                  XmlDom.loadXML(DatiNota)
                        '                  Dim xDatiMovimenti = XmlDom.getElementsByTagName("DatiMovimenti")

                        '                  Dim i_DatiMovimento = 0

                        '                  'Check di esistenza Informazioni
                        '                  Do While i_DatiMovimento <xDatiMovimenti.length

                        ' Dim xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)
                        ' Dim xMovimenti = xDatiMovimento.getElementsByTagName("Movimento")

                        ' Dim i_Movimento = 0

                        '                      Do While i_Movimento < xMovimenti.length

                        '  'Prelevo l'i-esimo blocco di Movimenti

                        '  Dim xMovimento = xMovimenti.Item(i_Movimento)

                        '  Select Case CStr(xMovimento.getAttribute("cau_mov"))

                        '                              Case CAU_REGISTRAZIONI

                        '                                  mId_Mov_Registrazione_Nota = xMovimento.getAttribute("id_mov")

                        '                                  ChkNota.value = ssChecked

                        '                                  FormInizializzazione_Nota()

                        '                                  TxtNumero_Nota.text = xMovimento.getAttribute("doc_numero")
                        '                                  TxtNumero_Nota_Sin.text = Agro_SQL_SaveText(xMovimento.getAttribute("doc_numero_sin"))
                        '                                  TxtNumero_Nota_Des.text = Agro_SQL_SaveText(xMovimento.getAttribute("doc_numero_des"))
                        '                                  mScadenza_Nota = CDate(xMovimento.getAttribute("scadenza"))
                        '                                  TxtScadenza_Nota.text = Imposta_Validita(Format(mScadenza_Nota, "dd/mm/yyyy"))
                        '                                  TxtData_Nota.text = Format(xMovimento.getAttribute("data_movimento"), "dd/mm/yyyy")
                        '                                  LblNote_Nota.Caption = xMovimento.getAttribute("extra_str")
                        '                                  TxtColli_Nota = xMovimento.getAttribute("colli")


                        '                                  '=========================================================================================================
                        '                                  'Gestione dei Progressivi
                        '                                  '---------------------------------------------------------------------------------------------------------
                        '                                  TxtProgr_Protocollo_Nota.text = Format(Agro_SQL_SaveNum(xMovimento.getAttribute("progr_protocollo")), "000000000")
                        '                                  TxtProgr_Registrazione_Nota.text = Format(Agro_SQL_SaveNum(xMovimento.getAttribute("progr_registrazione")), "000000000")

                        '                                  TxtData_Registrazione_Nota.text = Format(xMovimento.getAttribute("data_registrazione"), "dd/mm/yyyy")
                        '                                  '=========================================================================================================

                        '                              Case CAU_ABBUONI

                        '                                  mId_Mov_Abbuono_Nota = xMovimento.getAttribute("id_mov")

                        '    '====================================================================================
                        '    'Prelevo le Informazioni sugli importi
                        '    '------------------------------------------------------------------------------------
                        '    Dim xDatiMovimenti_Dettagli = xMovimento.getElementsByTagName("DatiMovimenti_Dettagli")

                        '    Dim i_DatiMovimenti_Dettagli = 0

                        '                                  Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.length

                        '    'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                        '     Dim xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                        '     Dim xMovimenti_Dettagli = xDatiMovimento_Dettaglio.getElementsByTagName("Movimento_Dettaglio")

                        '     Dim i_DatiMovimento_Dettaglio = 0

                        '                                      Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.length

                        '    'Prelevo l'i-esimo Movimento Dettaglio
                        '    Dim xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                        '    Riga_Nota = FormRicavaRigaDettaglio(CLng(xMovimento_Dettaglio.getAttribute("elem_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("pro_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("mat_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cod_progetto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cal_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("fase_cod")),
                        '                                                                            CStr(xMovimento_Dettaglio.getAttribute("lotto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("udm_cod")),
                        '                                                                            CInt(xMovimento_Dettaglio.getAttribute("sconto_modalita")))


                        '                                          'Aggiornamento Dettagli Nota
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_NOTA) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_RESI) = 0
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IMPONIBILE_NOTA) = CDbl(mSegno & 1) * xMovimento_Dettaglio.getAttribute("imponibile")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_CHKIVA_MANUALE_NOTA) = Agro_SQL_SaveNum(xMovimento_Dettaglio.getAttribute("chkiva_manuale"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_IVA_NOTA) = xMovimento_Dettaglio.getAttribute("cod_iva")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IVA_NOTA) = ((mSegno & 1) * -1) * (xMovimento_Dettaglio.getAttribute("iva"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_DESCRIZIONE_NOTA) = xMovimento_Dettaglio.getAttribute("mov_det_des")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_QTA_NOTA) = xMovimento_Dettaglio.getAttribute("qta")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ANNO_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("anno")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_PAT_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto_pat")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ID_MOV_DET_NOTA) = xMovimento_Dettaglio.getAttribute("id_mov_det")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_MODALITA_PROTETTA) = 1

                        '                                          i_DatiMovimento_Dettaglio += 1

                        '                                      Loop

                        '                                      i_DatiMovimenti_Dettagli += 1

                        '                                  Loop

                        '                              Case CAU_CARICO

                        '                                  mId_Mov_Nota = xMovimento.getAttribute("id_mov")

                        '    '====================================================================================
                        '    'Prelevo le Informazioni sugli importi
                        '    '------------------------------------------------------------------------------------
                        '    Dim xDatiMovimenti_Dettagli = xMovimento.getElementsByTagName("DatiMovimenti_Dettagli")

                        '    Dim i_DatiMovimenti_Dettagli = 0

                        '                                  Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.length

                        '     'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                        '     Dim xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                        '     Dim xMovimenti_Dettagli = xDatiMovimento_Dettaglio.getElementsByTagName("Movimento_Dettaglio")

                        '     Dim i_DatiMovimento_Dettaglio = 0

                        '                                      Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.length

                        '    'Prelevo l'i-esimo Movimento Dettaglio
                        '    Dim xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                        '    Riga_Nota = FormRicavaRigaDettaglio(CLng(xMovimento_Dettaglio.getAttribute("elem_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("pro_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("mat_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cod_progetto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cal_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("fase_cod")),
                        '                                                                            CStr(xMovimento_Dettaglio.getAttribute("lotto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("udm_cod")),
                        '                                                                            CInt(xMovimento_Dettaglio.getAttribute("sconto_modalita")))

                        '                                          'Aggiornamento Dettagli Nota
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_NOTA) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_RESI) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IMPONIBILE_NOTA) = CDbl(mSegno & 1) * (xMovimento_Dettaglio.getAttribute("imponibile"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_CHKIVA_MANUALE_NOTA) = Agro_SQL_SaveNum(xMovimento_Dettaglio.getAttribute("chkiva_manuale"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_IVA_NOTA) = xMovimento_Dettaglio.getAttribute("cod_iva")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IVA_NOTA) = ((mSegno & 1) * -1) * (xMovimento_Dettaglio.getAttribute("iva"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_DESCRIZIONE_NOTA) = xMovimento_Dettaglio.getAttribute("mov_det_des")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_QTA_NOTA) = xMovimento_Dettaglio.getAttribute("qta")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ANNO_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("anno")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_PAT_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto_pat")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ID_MOV_DET_NOTA) = xMovimento_Dettaglio.getAttribute("id_mov_det")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_MODALITA_PROTETTA) = 1

                        '      Dim xMov_Destinazioni = xMovimento_Dettaglio.getElementsByTagName("Movimento_Destinazione")

                        '      Dim i_DatiMov_Destinazioni = 0

                        '                                          Do While i_DatiMov_Destinazioni < xMov_Destinazioni.length

                        '       'Prelevo l'i-esimo Movimento Destinazione
                        '       Dim xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                        '       MSDettagli.TextMatrix(Riga_Nota, COL_PIVA_PROVENIENZA) = xMov_Destinazione.getAttribute("piva")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_SA_COD_PROVENIENZA) = xMov_Destinazione.getAttribute("sa_cod")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_ID_DESTINAZIONE_PROVENIENZA) = xMov_Destinazione.getAttribute("id_destinazione")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_TIPO_PROVENIENZA) = xMov_Destinazione.getAttribute("tipo_destinazione")

                        '                                              i_DatiMov_Destinazioni += 1

                        '                                          Loop

                        '                                          i_DatiMovimento_Dettaglio += 1

                        '                                      Loop

                        '                                      i_DatiMovimenti_Dettagli += 1

                        '                                  Loop

                        '                              Case CAU_SCARICO

                        '                                  mId_Mov_Nota = xMovimento.getAttribute("id_mov")

                        '  '====================================================================================
                        '  'Prelevo le Informazioni sugli importi
                        '  '------------------------------------------------------------------------------------
                        '  Dim xDatiMovimenti_Dettagli = xMovimento.getElementsByTagName("DatiMovimenti_Dettagli")

                        '  Dim i_DatiMovimenti_Dettagli = 0

                        '                                  Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.length

                        '   'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                        '    Dim xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                        '    Dim xMovimenti_Dettagli = xDatiMovimento_Dettaglio.getElementsByTagName("Movimento_Dettaglio")

                        '    Dim i_DatiMovimento_Dettaglio = 0

                        '                                      Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.length

                        '    'Prelevo l'i-esimo Movimento Dettaglio
                        '    Dim xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                        '    Riga_Nota = FormRicavaRigaDettaglio(CLng(xMovimento_Dettaglio.getAttribute("elem_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("pro_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("mat_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cod_progetto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("cal_cod")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("fase_cod")),
                        '                                                                            CStr(xMovimento_Dettaglio.getAttribute("lotto")),
                        '                                                                            CLng(xMovimento_Dettaglio.getAttribute("udm_cod")),
                        '                                                                            CInt(xMovimento_Dettaglio.getAttribute("sconto_modalita")))


                        '                                          'Aggiornamento Dettagli Nota
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_NOTA) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_RESI) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IMPONIBILE_NOTA) = CDbl(mSegno & 1) * (xMovimento_Dettaglio.getAttribute("imponibile"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_CHKIVA_MANUALE_NOTA) = 1
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_IVA_NOTA) = xMovimento_Dettaglio.getAttribute("cod_iva")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_IVA_NOTA) = ((mSegno & 1) * -1) * (xMovimento_Dettaglio.getAttribute("iva"))
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_DESCRIZIONE_NOTA) = xMovimento_Dettaglio.getAttribute("mov_det_des")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_QTA_NOTA) = xMovimento_Dettaglio.getAttribute("qta")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ANNO_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("anno")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_COD_CONTO_PAT_NOTA) = xMovimento_Dettaglio.getAttribute("cod_conto_pat")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_ID_MOV_DET_NOTA) = xMovimento_Dettaglio.getAttribute("id_mov_det")
                        '                                          MSDettagli.TextMatrix(Riga_Nota, COL_MODALITA_PROTETTA) = 1

                        '        Dim xMov_Destinazioni = xMovimento_Dettaglio.getElementsByTagName("Movimento_Destinazione")

                        '        Dim i_DatiMov_Destinazioni = 0

                        '                                          Do While i_DatiMov_Destinazioni < xMov_Destinazioni.length

                        '       'Prelevo l'i-esimo Movimento Destinazione
                        '       Dim xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                        '       MSDettagli.TextMatrix(Riga_Nota, COL_PIVA_DESTINAZIONE) = xMov_Destinazione.getAttribute("piva")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_SA_COD_DESTINAZIONE) = xMov_Destinazione.getAttribute("sa_cod")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_ID_DESTINAZIONE_DESTINAZIONE) = xMov_Destinazione.getAttribute("id_destinazione")
                        '                                                                  MSDettagli.TextMatrix(Riga_Nota, COL_TIPO_DESTINAZIONE) = xMov_Destinazione.getAttribute("tipo_destinazione")

                        '                                              i_DatiMov_Destinazioni += 1

                        '                                          Loop

                        '                                          i_DatiMovimento_Dettaglio += 1

                        '                                      Loop

                        '                                      i_DatiMovimenti_Dettagli += 1

                        '                                  Loop

                        '                          End Select

                        '                          i_Movimento += 1

                        '                      Loop

                        '                      i_DatiMovimento += 1

                        '                  Loop

                        ''#################################################################################
                        ''################################    RISCOSSIONI    ##############################
                        ''#################################################################################


                        ''======================================================================================================
                        ''Lettura Insolvenza Nota
                        ''------------------------------------------------------------------------------------------------------
                        'Dim RsRiscossioni = cMagazzino.ListaRiscossioni(, piva, 0, mId_Agenda_Nota)

                        'mImportoPagato_Nota = 0

                        '                  If RsRiscossioni.State <> 0 Then

                        '                      Do While Not RsRiscossioni.EOF

                        '                          mImportoPagato_Nota += RsRiscossioni("Importo")

                        '                          RsRiscossioni.MoveNext
                        '                      Loop

                        '                  End If


                        '                  FormAggiornaImporto_Nota()

                        '                  FormImpostaStatoPagamento_Nota()
                        '                  '======================================================================================================

                        '              End If


                        '      End Select

                End Select

                Select Case lavCod
                    Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO
                        'TODO: Recupero la qta evasa (devo tenere memorizza la qta evasa per ogni dettaglio!!!)

                End Select
            Next

        End If

    End Sub

    Public Function VerificaModificaDataDocumento(ByVal piva As String,
                                                  ByVal idAgenda As Integer,
                                                  ByVal lavCod As Integer,
                                                  ByVal cauMov As String,
                                                  ByVal dataDoc As Date,
                                                  ByVal newDataDoc As Date
                                                  ) As Contabilita_Output_ModData

        Dim lavCodOrdini As Integer() = {LAVCOD_ORDINE_ACQUISTO, LAVCOD_ORDINE_VENDITA}

        Const nomeRoutine = "ContabilitaHelper.VerificaModificaDataDocumento()"
        Dim output As New Contabilita_Output_ModData

        Try

            'Verifica preliminare se è possibile salvare con nuova data

            Select Case True

                Case lavCod = LAVCOD_CONTRATTO_AFFITTO

                    'Non sono necessari controlli sulla giacenza.

                Case lavCodOrdini.Contains(lavCod)

                    'Non sono necessari controlli sulla giacenza.

                    'TODO: controllo se ordine presenta data maggiore al primo documento di evasione

                Case cauMov = CAU_CARICO

                    'Se nuova data precedente o uguale a quella attuale, posso effettuare lo spostamento CARICO.
                    'Se nuova data SUCCESSIVA a quella attuale, effettuo il controllo.

                    If newDataDoc > dataDoc Then

                        output.MsgError = VerificaModificaDataCarico(piva, idAgenda, lavCod, dataDoc, newDataDoc)

                    End If

                    If output.MsgError = "" AndAlso UtilityHelper.IsAccettazione(lavCod) AndAlso newDataDoc <> AGRODATAFINE Then

                        'Potrei avere delle raccolte collegate e quindi devo modificare le date anche di quelle.
                        'Devo fare query di verifica per vedere se nella nuova data c'è una distinta valida per
                        'ogni impianto di ogni agenda coinvolta.

                        output.MsgError = VerificaModificabilitaRaccolteCollegate(piva,
                                                                                  idAgenda,
                                                                                  lavCod,
                                                                                  newDataDoc,
                                                                                  output.PivaProduttore,
                                                                                  output.AgendeProduttore)

                        'Visto che faccio query di verifica, mi estraggo direttamente le id_agende da passare alla
                        'funzione che modifica effettivamente la data.

                    End If

                Case cauMov = CAU_SCARICO

                    'TODO: permetti modifica data doc anche per scarichi!
                    'test su uscita: il primo movimento di carico ora risulterebbe essere successivo allo scarico? ==> mi fermo 

                    '---------------------------------------------------------------------------------------------------------
                    'Al momento consentito solo per gli scarichi di magazzino; per i documenti contabili la problematica è
                    'più complessa, in quanto lo spostamento della data implica anche il controllo della corretta sequenza
                    'numero/data documento
                    '---------------------------------------------------------------------------------------------------------

                    'Se nuova data successiva o uguale a quella attuale, posso effettuare lo spostamento SCARICO.
                    'Se nuova data PRECEDENTE a quella attuale, effettuo il controllo.

                    If newDataDoc < dataDoc Then

                        output.MsgError = VerificaModificaDataScarico(piva, idAgenda, lavCod, dataDoc, newDataDoc)

                    End If

            End Select

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return output

    End Function

    Private Function VerificaModificaDataCarico(ByVal piva As String,
                                                ByVal idAgenda As Integer,
                                                ByVal lavCod As Integer,
                                                ByVal dataDoc As Date,
                                                ByVal newDataDoc As Date
                                                ) As String

        Dim messaggioErrore As String = ""

        'PER OGNI RIGA:
        'Se ci sono altre operazioni di SCARICO con stesso mat_cod - lotto - cal_cod,
        'significa che è stato movimentato e NON posso modificare data se posteriore
        'al primo SCARICO successivo.

        Dim escludiDdtResoConferimento As Boolean = UtilityHelper.IsAccettazione(lavCod)

        Dim listaElemCod = CaricaCategorieMagazzinoDaControllare()

        Dim movDetR As New Movimenti_Dettagli_R

        Dim dt As DataTable = movDetR.ProdottoMovimentatoInAltreOp(piva,
                                                                   idAgenda,
                                                                   0,
                                                                   CAU_SCARICO,
                                                                   _objParametriServer,
                                                                   dataVerifica:=dataDoc,
                                                                   escludiDdtResoConferimento:=escludiDdtResoConferimento,
                                                                   listaElemCod:=listaElemCod,
                                                                   soloMagazzinoMovimentato:=True)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            Dim dataPrimoScaricoSuccessivo As Date = CDate(dt.Compute("min(Validita_Inizio)", Nothing))

            If newDataDoc > dataPrimoScaricoSuccessivo Then

                messaggioErrore = DataSuccessivaMovimentiScaricoProdotto(dt, newDataDoc, dataPrimoScaricoSuccessivo)

            End If

        End If

        Return messaggioErrore

    End Function

    Private Function CaricaCategorieMagazzinoDaControllare() As List(Of Integer)

        Dim listaElemCod As List(Of Integer) = Nothing

        Dim parametriGiacenze = UtilityHelper.GetParametriGiacenze(_objParametriUtenti)

        'Se bloccaUtentePerSottogiacenza devo controllare tutte le categorie
        If parametriGiacenze.bloccaUtentePerSottogiacenza = False Then

            'Altrimenti prendo solo le categorie di tipo SoloPresenti
            listaElemCod = (From dizGestGiac In parametriGiacenze.dizionarioGestioneGiacenze
                            Where dizGestGiac.Value.Equals(enum_Gestione_Giacenze.SoloPresenti)
                            Select dizGestGiac.Key).ToList()

        End If

        Return listaElemCod

    End Function

    Private Function DataSuccessivaMovimentiScaricoProdotto(dt As DataTable,
                                                            newDataDoc As Date,
                                                            dataPrimoScaricoSuccessivo As Date) As String

        Dim msgError = "Non è possibile modificare la data in quanto SUCCESSIVA ai seguenti movimenti di scarico del prodotto: <br><ul>"

        Dim filtro = String.Format("Validita_Inizio >= #{0}# AND Validita_Inizio < #{1}#",
                                   dataPrimoScaricoSuccessivo.ToString("MM/dd/yyyy"),
                                   newDataDoc.ToString("MM/dd/yyyy"))

        Dim drFiltrato As DataRow() = dt.Select(filtro)

        For Each dr In drFiltrato
            msgError &= String.Format("<li>{0} del {1}</li>",
                                      dr.Item("Des_Lib"),
                                      CDate(dr.Item("Validita_Inizio")).ToShortDateString())
        Next

        msgError &= "</ul>"

        Return msgError

    End Function

    Private Function VerificaModificaDataScarico(ByVal piva As String,
                                                 ByVal idAgenda As Integer,
                                                 ByVal lavCod As Integer,
                                                 ByVal dataDoc As Date,
                                                 ByVal newDataDoc As Date
                                                 ) As String

        Dim messaggioErrore As String = ""

        'PER OGNI RIGA:
        'Se ci sono altre operazioni di CARICO con stesso mat_cod - lotto - cal_cod,
        'significa che è stato movimentato e NON posso modificare data se antecedente
        'al primo CARICO precedente.

        Dim escludiDdtResoConferimento As Boolean = UtilityHelper.IsAccettazione(lavCod)

        Dim listaElemCod = CaricaCategorieMagazzinoDaControllare()

        Dim movDetR As New Movimenti_Dettagli_R

        Dim dt As DataTable = movDetR.ProdottoMovimentatoInAltreOp(piva,
                                                                   idAgenda,
                                                                   0,
                                                                   CAU_CARICO,
                                                                   _objParametriServer,
                                                                   dataVerifica:=dataDoc,
                                                                   escludiDdtResoConferimento:=escludiDdtResoConferimento,
                                                                   listaElemCod:=listaElemCod,
                                                                   movimentiPrecedenti:=True,
                                                                   soloMagazzinoMovimentato:=True)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            Dim dataPrimoCaricoPrecedente As Date = CDate(dt.Compute("max(Validita_Inizio)", Nothing))

            If newDataDoc < dataPrimoCaricoPrecedente Then

                messaggioErrore = DataPrecedenteMovimentiCaricoProdotto(dt, newDataDoc, dataPrimoCaricoPrecedente)

            End If

        End If

        Return messaggioErrore

    End Function

    Private Function DataPrecedenteMovimentiCaricoProdotto(dt As DataTable,
                                                           newDataDoc As Date,
                                                           dataPrimoCaricoPrecedente As Date) As String

        Dim msgError = "Non è possibile modificare la data in quanto PRECEDENTE ai seguenti movimenti di carico del prodotto: <br><ul>"

        Dim filtro = String.Format("Validita_Inizio > #{0}# AND Validita_Inizio <= #{1}#",
                                   newDataDoc.ToString("MM/dd/yyyy"),
                                   dataPrimoCaricoPrecedente.ToString("MM/dd/yyyy"))

        Dim drFiltrato As DataRow() = dt.Select(filtro)

        For Each dr In drFiltrato
            msgError &= String.Format("<li>{0} del {1}</li>",
                                      dr.Item("Des_Lib"),
                                      CDate(dr.Item("Validita_Inizio")).ToShortDateString())
        Next

        msgError &= "</ul>"

        Return msgError

    End Function

    Private Function VerificaModificabilitaRaccolteCollegate(ByVal piva As String,
                                                             ByVal idAgenda As Integer,
                                                             ByVal lavCod As Integer,
                                                             ByVal newDataDoc As Date,
                                                             ByRef pivaProduttore As String,
                                                             ByRef listAgendeProduttore As List(Of Integer)
                                                             ) As String

        Const nomeRoutine = "ContabilitaHelper.VerificaModificabilitaRaccolteCollegate()"
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim movDetR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Try

            'TODO: Potrei avere delle raccolte collegate e quindi devo modificare le date anche di quelle
            'Devo fare query di verifica per vedere se nella nuova data c'è una distinta valida per ogni impianto di ogni agenda coinvolta

            Dim dt As DataTable = movDetR.RaccolteCollegate(piva, idAgenda, newDataDoc, _objParametriServer)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                'Ho raccolte collegate

                'La piva del contadino è sempre la stessa ==> tengo la prima
                pivaProduttore = dt.Rows(0).Item("Piva")

                For Each dr In dt.Rows
                    Dim idAgendaProduttore As Integer = CInt(dr.Item("Id_Agenda"))
                    If Not listAgendeProduttore.Contains(idAgendaProduttore) Then
                        listAgendeProduttore.Add(idAgendaProduttore)
                    End If

                    If CInt(dr.Item("Appezza")) <> 0 AndAlso CInt(dr.Item("Id_Destinazione")) <> 0 AndAlso
                       (IsDBNull(dr.Item("Progetto_Cod")) OrElse dr.Item("Progetto_Cod") = 0) Then
                        'Ho un impianto preciso, ma non non un progetto_cod ==> nella nuova data non c'è una distinta disponibile da usare nella raccolta
                        msgError = "Non è possibile modificare la data in quanto non è possibile modificare la data delle raccolte collegate"
                        Exit For
                    End If
                Next
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return msgError

    End Function

    Public Function ModificaDataDocPossibile(ByVal objContabTestata As Contabilita_Testata,
                                             ByVal piva As String,
                                             ByVal idAgenda As Integer,
                                             ByVal lavCod As Integer,
                                             ByVal cauMov As String,
                                             ByVal attualeDataDoc As Date,
                                             ByVal newDataDoc As Date
                                             ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.ModificaDataDocPossibile()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim risModificabilitaData As New Contabilita_Output_ModData

        Dim output As New Contabilita_Output

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'TODO: Verifica preliminare se è possibile salvare con nuova data

            ' se tipologia documento 28 (Acquisti da San Marino con IVA (fattura cartacea) ) salto il controllo
            If Not IsNothing(objContabTestata) AndAlso objContabTestata.TipoDocumento = 28 Then
                risModificabilitaData.MsgError = ""
            Else
                risModificabilitaData = VerificaModificaDataDocumento(piva, idAgenda, lavCod, cauMov, attualeDataDoc, newDataDoc)
            End If

            If risModificabilitaData IsNot Nothing AndAlso risModificabilitaData.MsgError = "" Then
                'Salvo la data su Movimenti.Data_Emissione e a cascata su Validita_Inizio di Agenda e tutti i suoi figli
                'TODO: + Agronica_Log_Agenda
                xRisp = ModificaComplessivaDataDoc(piva, idAgenda, lavCod, newDataDoc, _objParametriServer)

                If xRisp = True AndAlso UtilityHelper.IsAccettazione(lavCod) Then

                    'Modifica data raccolta collegata a conferimento
                    If risModificabilitaData.PivaProduttore <> "" AndAlso risModificabilitaData.AgendeProduttore.Count > 0 Then
                        For Each agendaProduttore In risModificabilitaData.AgendeProduttore
                            xRisp = ModificaComplessivaDataDoc(risModificabilitaData.PivaProduttore, agendaProduttore, LAVCOD_RACCOLTA, newDataDoc,
                                                               _objParametriServer, modificaOra:=True)
                        Next
                    End If

                    'Modifica data ddt reso imballi collegato a Conferimento
                    Dim movRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                    Dim dt As DataTable = movRifR.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_BOLLA_EMESSA, "",
                                                                  piva, 0, idAgenda, 0, 0,
                                                                  lavCod, "", "", "", _objParametriServer)

                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        'Ho dei ddt emessi di reso imballi collegati ==> cambio la data anche a loro
                        For Each dr In dt.Rows
                            xRisp = ModificaComplessivaDataDoc(dr.Item("Piva"), dr.Item("Id_Agenda"), LAVCOD_BOLLA_EMESSA, newDataDoc,
                                                               _objParametriServer, modificaOra:=True)
                        Next
                    End If

                End If
            End If

            'test solo su entrata: ci sono altre operazioni seguenti con stesso mat_cod - lotto - cal_cod? se sì, vuol dire che l'ho movimentato!
            ' ==> non posso modificare data

            'ENTRATA: quando so la nuova data: se ci sono impianti devo verificare che gli impianti saranno ancora validi alla nuova data
            ' ==> se lo sono devo cambiare anche quella data!!!



            'If docRicevutoCaricoMagazzino = True Then
            '    'Per i conferimenti e i DDT di acquisto controlliamo che le righe entrate poi non siano state più movimentate,
            '    'ma se viene facile possiamo fare passare la modifica se il primo movimento di scarico in ordine di tempo
            '    'è minore della data che si va ad indicare;
            '    'ma in questo caso il controllo non possiamo più farlo al click sul lucchetto ma all’onchange della data

            '    If isLavCodAccettazione = True Then
            '        'Potrei avere gli impianti...
            '        'Poi c’è il controllo scritto sotto per gli impianti
            '        'Impianti = occorre controllare che gli impianti scelti siano validi alla nuova data;
            '        'se non lo sono dare errore bloccante
            '    End If

            'Else
            '    'Per i DDT di uscita dovremmo cercare se il primo movimento di entrata in ordine di tempo
            '    'è maggiore della data che si va ad indicare

            '    'Se diventa troppo lunga dato che al momento non abbiamo tempo gestire solo l’entrata
            '    'e solo testando che non ci siano altri movimenti;
            '    'se è questione di qualche ora facciamolo addirittura per il verso
            'End If

            'Sì, i primi controlli li facciamo al click sul lucchetto; se non è possibile diamo messaggio
            'possibilmente dando uno specchietto del perché non possiamo permettere la modifica:
            '-	Non è possibile modificare la data in quanto esistono i movimenti “Lavorazione del ggmmaaa” /
            'DDT cliente XYZ nr nnn del ggmmaaa

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Dim outputTotale As New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = risModificabilitaData.MsgError,
            .Id_Agenda = idAgenda
        }


        '.Id_Mov_Testata = objContabTestata.IdMov,
        '.Id_Mov_Secondario = If(objContabTestata.DocumentoAccettazione Is Nothing, 0, objContabTestata.DocumentoAccettazione.IdMov),
        '.Des_Lib = outputTestata.Des_Lib,
        '.Doc_Numero = outputTestata.Doc_Numero,
        '.Doc_Numero_Visualizzato = outputTestata.Doc_Numero_Visualizzato

        Return outputTotale

    End Function

    Public Function ModificaComplessivaDataDoc(piva As String,
                                               idAgenda As Integer,
                                               lavCod As Integer,
                                               newDataDoc As Date,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal modificaOra As Boolean = False
                                               ) As Boolean

        Const nomeRoutine = "ContabilitaHelper.ModificaComplessivaDataDoc()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim cauMov As String

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            Dim desLib As String = UtilityHelper.GetDesLib(piva, idAgenda, lavCod, objParametri)

            Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W
            objAgendaW.ModificaPuntuale(piva, 0, idAgenda,
                                        objParametri,
                                        Validita_Inizio:=newDataDoc)

            Dim objMovW As New AgronicaCoreContabDAL.Movimenti_W
            objMovW.ModificaPuntuale(piva, 0, idAgenda, 0,
                                     objParametri,
                                     Validita_Inizio:=newDataDoc)

            '-------------------------------------------------------------------------------------------
            'Aggiornamento registrazione secondaria (Accettazione / Contratti Affitto)
            '-------------------------------------------------------------------------------------------
            '1. Se Accettazione, nella registrazione secondaria NON devo modificare la colonna
            '   data_movimento, ma solo validita_inizio.
            '   Quindi faccio una seconda query per modificare solo data_movimento dove mi serve.
            '2. Se Contratto Affitto, posso invece aggiornare la data_movimento.
            '-------------------------------------------------------------------------------------------

            Dim filtroAggiuntivo = " Cau_Mov NOT IN ( " & CAU_REGISTRAZIONE_SECONDARIA & ") "

            If lavCod = LAVCOD_CONTRATTO_AFFITTO Then

                filtroAggiuntivo = ""

            End If

            objMovW.ModificaPuntuale(piva, 0, idAgenda, 0,
                                     objParametri,
                                     xFiltroAggiuntivo:=filtroAggiuntivo,
                                     Data_Movimento:=newDataDoc)

            '-------------------------------------------------------------------------------------------
            'Aggiornamento data registrazione
            '-------------------------------------------------------------------------------------------
            'I nuovi movimenti di carico e scarico, a differenza deglia altri documenti, non hanno il rk
            'movimenti di tipo CAU_REGISTRAZIONI; la data registrazione viene memorizzata nel rk di tipo
            'CAU_CARICO o CAU_SCARICO a seconda del tipo di movimento.
            '-------------------------------------------------------------------------------------------

            Select Case lavCod

                Case LAVCOD_CARICO

                    cauMov = CAU_CARICO

                Case LAVCOD_SCARICO

                    cauMov = CAU_SCARICO

                Case Else

                    cauMov = CAU_REGISTRAZIONI

            End Select

            objMovW.ModificaPuntuale(piva, 0, idAgenda, 0,
                                     objParametri,
                                     xFiltroAggiuntivo:=" Cau_Mov IN ( " & cauMov & ") ",
                                     Data_Registrazione:=newDataDoc)

            '-------------------------------------------------------------------------------------------

            If modificaOra = True Then
                objMovW.ModificaPuntuale(piva, 0, idAgenda, 0,
                                         objParametri,
                                         Ora:=newDataDoc,
                                         Flag_Usa_Ora_Reale:=True)
            End If

            Dim objMovDetW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            objMovDetW.ModificaPuntuale(piva, 0, idAgenda, 0, 0,
                                        objParametri,
                                        Validita_Inizio:=newDataDoc)

            Dim objMovDetTecniciW As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
            objMovDetTecniciW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0,
                                               objParametri,
                                               Validita_Inizio:=newDataDoc)

            Dim objMovDetTecniciExtraW As New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W
            objMovDetTecniciExtraW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0,
                                                    objParametri,
                                                    Validita_Inizio:=newDataDoc)

            Dim objMovDetConferimentoW As New AgronicaCoreContabDAL.Mov_Dett_Conferimento_W
            objMovDetConferimentoW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0,
                                                    objParametri,
                                                    Validita_Inizio:=newDataDoc)

            Dim objMovDestW As New AgronicaCoreContabDAL.Mov_Destinazioni_W
            objMovDestW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0, 0,
                                         objParametri,
                                         Validita_Inizio:=newDataDoc)

            Dim objMovDetRifW As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W    'Solo da un verso!!!
            objMovDetRifW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, 0, 0, 0,
                                           objParametri,
                                           Validita_Inizio:=newDataDoc)

            'TODO: aggiornare se sono cambiati Des_Lib e Data_Movimento
            Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
            objAgronicaLogAgendaW.Scrivi(newDataDoc, enum_TipoOperazioneDB.Modifica,
                                         desLib, idAgenda, piva, 0, lavCod,
                                         CInt(enum_Id_Servizio.GiasOnline),
                                         objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)
            xRisp = True

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

#Region "Verifiche Preliminari"

    Private Shared Function CheckPropertyTestataContabilita(ByRef objContabTestata As Contabilita_Testata,
                                                            ByVal tipoOperazione As enum_TipoOperazioneDB,
                                                            ByRef objParametriServer As AgronicaCoreParametri
                                                            ) As Boolean

        Const nomeRoutine = "ContabilitaHelper.CheckPropertyTestataContabilita()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objContabTestata, Nothing, stringaErrore, "Piva")
            UtilityHelper.ControllaStringEmpty(objContabTestata, objParametriServer.UsernameOperazione, stringaErrore, "UsernameModifica")
            UtilityHelper.ControllaPresenza(objContabTestata, enum_Omni_Modulo_Generazione.Nessuno, stringaErrore, "ModuloGias")
            UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "TipoDocumento")

            If tipoOperazione = enum_TipoOperazioneDB.Modifica OrElse
               tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                UtilityHelper.ControllaNumZero(objContabTestata, Nothing, stringaErrore, "IdAgenda")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                UtilityHelper.ControllaNumZero(objContabTestata, Nothing, stringaErrore, "LavCod")
                UtilityHelper.ControllaPresenza(objContabTestata, Nothing, stringaErrore, "DataMovimento")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "SaCod")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "ContabilizzazioneManuale")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "NoteIntestazione")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "DocNumeroSin")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "DocNumeroDes")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "DocNumeroVisualizzato")
                UtilityHelper.ControllaPresenza(objContabTestata, objContabTestata.DataMovimento, stringaErrore, "DataRegistrazione")
                UtilityHelper.ControllaPresenza(objContabTestata, Nothing, stringaErrore, "ProgrRegistrazione")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "ProgrProtocollo")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "SezionaleCod")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "RagSoc")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodDestinazione")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodIndirizzoDestinazione")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodRisUmAggiuntivo")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodIndirizzoAggiuntivo")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodVettore")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodIndirizzoVettore")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "Aspetto")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "NaturaBeni")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "CausaleTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CausaleTrasportoCod")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "Colli")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "TipoPeso")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "Peso")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "Mezzo")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "TaraVeicolo")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "TaraImballi")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CodRisUmAltro")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "TotaleDocumento")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "AgenteCod")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "AgenteProvvigione")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "CapoAreaCod")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "CapoAreaProvvigione")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "ModalitaTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "UnitaTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "GestioneVettore")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "MacCodTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "Targa")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "DescrizioneMezzo")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "NumImmatricolazioneRimorchio")
                UtilityHelper.ControllaPresenza(objContabTestata, "", stringaErrore, "NumAutorizzazioneTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, Now, stringaErrore, "DataRilascioAutorizzazione")
                UtilityHelper.ControllaPresenza(objContabTestata, CDec(0), stringaErrore, "PesoTaraTrasporto")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "Accompagnatoria")
                UtilityHelper.ControllaPresenza(objContabTestata, 0, stringaErrore, "SecondaCooperativa")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'UtilityHelper.ControllaNumZero(objContabTestata, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaPresenza(objContabTestata, Nothing, stringaErrore, "DataOraUltimaLettura")
                'in realtà una volta fissato non lo dovrò più modificare ma mi serve perché così so che tipo di movimento sto aggiornando
                '(potrebbero esserci cose diverse da fare a seconda del lav cod)
                UtilityHelper.ControllaNumZero(objContabTestata, Nothing, stringaErrore, "LavCod")
            End If

            'If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
            '    UtilityHelper.ControllaStringEmpty(objContabTestata, Nothing, stringaErrore, "Descrizione")
            'End If

            If Not String.IsNullOrEmpty(stringaErrore) Then
                Throw New Exception(stringaErrore)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return True

    End Function

#End Region

#Region "Creazione Descrizioni e Messaggi"

    Private Shared Function CreaDescDocAccettazione(ByVal lavCodPrincipale As Integer,
                                                    ByRef objContabTestata As Contabilita_Testata,
                                                    ByRef lavCodAssociato As Integer
                                                    ) As String

        Const nomeRoutine = "ContabilitaHelper.CreaDescDocAccettazione"
        Dim descDoc As String = ""

        Try

            Select Case lavCodPrincipale
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    lavCodAssociato = LAVCOD_BOLLA_RICEVUTA
                Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                    lavCodAssociato = LAVCOD_DISTINTA_CARICO
                Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                    lavCodAssociato = LAVCOD_AUTO_DDT_EMESSO
            End Select

            descDoc = CreaDesLib(lavCodAssociato,
                                 objContabTestata.DocumentoAccettazione.DocNumeroSin,
                                 objContabTestata.DocumentoAccettazione.DocNumero,
                                 objContabTestata.DocumentoAccettazione.DocNumeroDes,
                                 objContabTestata.RagSoc)

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return descDoc

    End Function

    Private Shared Function GetDesDocCollegato(ByVal desLib As String, ByVal dataDoc As Date) As String
        Return desLib & " del " & dataDoc.ToShortDateString() & Chr(13)
    End Function

#End Region

    Private Class Movimento_Testata_LavCod

        Public Property Cau_Mov As String

        Public Property Mov_Desc As String

        Public Property Extra_Str As String

    End Class

End Class
