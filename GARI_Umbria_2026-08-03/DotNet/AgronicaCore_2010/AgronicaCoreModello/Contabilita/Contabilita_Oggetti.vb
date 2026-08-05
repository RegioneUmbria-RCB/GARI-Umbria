Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtility

''' <summary>
''' Racchiude i campi che si trovano in Agenda e Movimenti (Intestazione=4000, Carico=7300, Scarico=7350)
''' </summary>
Public Class Contabilita_Testata

    Public Property Piva As String
    Public Property SaCod As Integer?
    Public Property IdAgenda As Integer?
    Public Property IdMov As Integer?
    Public Property LavCod As Integer?
    Public Property DescrizioneAgenda As String
    Public Property BloccoFlag As Integer?
    Public Property BloccoData As Date?
    Public Property BloccoUsername As String
    Public Property BloccoUtente As String
    Public Property DataMovimento As Date?
    Public Property ContabilizzazioneManuale As Integer?
    Public Property TipoAccettazione As Integer?

    Public Property StatoExport2 As Integer?

    ''' <summary>
    ''' Extra_Int in Movimento = 4000 quando Lav_Cod = 1000,1001 (Fatture)
    ''' </summary>
    Public Property Accompagnatoria As Integer?
    Public Property RagSoc As String
    Public Property CodRisUm As Integer?
    Public Property CodIndirizzoRisUm As Integer?
    Public Property CodDestinazione As Integer?
    Public Property CodIndirizzoDestinazione As Integer?
    Public Property CodRisUmAltro As Integer?
    Public Property CodRisUmAggiuntivo As Integer?
    Public Property CodIndirizzoAggiuntivo As Integer?

    ''' <summary>
    ''' Extra_Int in Movimento = 4000 quando Lav_Cod = 1054 (Caso Fruttagel)
    ''' </summary>
    Public Property SecondaCooperativa As Integer?

    ''' <summary>
    ''' Mov_Desc in Movimento = 4000
    ''' </summary>
    Public Property NoteIntestazione As String

    Public Property DocNumeroSin As String
    Public Property DocNumero As Integer?
    Public Property DocNumeroDes As String
    Public Property DocNumeroVisualizzato As String
    Public Property DocNumeroLunghezza As Integer?
    Public Property DocNumeroCarattereFormattazione As String
    Public Property DocNumeroLock As Boolean
    Public Property TotaleDocumento As Decimal?

    ''' <summary>
    ''' Trasporto a cura del
    ''' </summary>
    Public Property Mezzo As Integer?

    Public Property CodVettore As Integer?
    Public Property CodIndirizzoVettore As Integer?
    Public Property CausaleTrasporto As String
    Public Property CausaleTrasportoCod As Integer?
    Public Property Aspetto As String
    Public Property TipoPeso As Integer?
    Public Property Peso As Decimal?
    Public Property Colli As Integer?
    Public Property OraSpedizione As DateTime?
    Public Property NaturaBeni As String
    Public Property TaraVeicolo As Decimal?
    Public Property TaraImballi As Decimal?
    Public Property ProgrProtocollo As Integer?
    Public Property ProgrRegistrazione As Integer?
    Public Property DataRegistrazione As Date?
    Public Property SezionaleCod As Integer?

    Public Property DataEvasionePrevista As Date?
    Public Property DataSpedizionePrevista As DateTime?

    ''' <summary>
    ''' Extra_Int in Movimento = 4000 quando Lav_Cod = 2002,2004 (Ordini)
    ''' </summary>
    Public Property ScadenzaUnica As Boolean
    ''' <summary>
    ''' Extra_Int in Movimento = 4000 quando Lav_Cod = 2002,2004 (Ordini)
    ''' </summary>
    Public Property EvasioneTassativa As Boolean

    'Gestione Workflow
    Public Property PraticaCod As Integer

    Public Property StatoCodPratica As Integer

    'TODO: bisogna aggiungere tutti i campi di testata presenti in Mov_dettagli_tecnico_extra

    Public Property AgenteCod As Integer?
    Public Property AgenteProvvigione As Decimal?
    Public Property CapoAreaCod As Integer?
    Public Property CapoAreaProvvigione As Decimal?

    ''' <summary>
    ''' salvato in TipoTrasporto di Mov_Det_Tecnico_Extra di Testata
    ''' </summary>
    Public Property ModalitaTrasporto As Integer?

    Public Property UnitaTrasporto As Integer?
    Public Property GestioneVettore As Integer?

    Public Property TipoDocumento As Integer?


    Public Property MacCodTrasporto As Integer?
    Public Property Targa As String
    Public Property DescrizioneMezzo As String
    Public Property NumImmatricolazioneRimorchio As String
    Public Property NumAutorizzazioneTrasporto As String
    Public Property DataRilascioAutorizzazione As Date?
    Public Property PesoTaraTrasporto As Decimal?

    '------------------------------------------------------------
    Public Property NumDocOrdineCliente As String
    Public Property DataDocOrdineCliente As Date?
    Public Property NumDocOrdineEnte As String
    Public Property AnnoDocOrdineEnte As Integer?
    '------------------------------------------------------------

    Public Property ChkLayOut_Bypass_Fatturato As Integer?
    Public Property ChkLayOut_Join_Prodotti As Integer?
    Public Property ChkLayOut_Litri As Integer?

    Public Property Layout_FormatiStampa As enum_LayoutFormatiStampaDoc

    Public Property ChkFiltro_Varietale As Integer?
    Public Property Disciplinare_PubblicoPrivato As Integer?

    '------------------------------------------------------------
    Public Property DocumentoAccettazione As Contabilita_Accettazione
    '------------------------------------------------------------
    Public Property DocumentoContrattoAffitto As Contabilita_Contratto_Affitto
    '------------------------------------------------------------
    Public Property MovimentoCarico As Contabilita_Chiave_Mov
    Public Property MovimentoScarico As Contabilita_Chiave_Mov
    Public Property ConteggiTotali As Contabilita_Totali_Testata
    Public Property PesoNettoProd As Decimal?
    Public Property ImballiVuoti As Decimal?
    '------------------------------------------------------------

    Public Property ModuloGias As enum_Omni_Modulo_Generazione
    Public Property DataOraUltimaLettura As DateTime?
    Public Property UsernameModifica As String

    'Public Property Lavorazioni_Associate As List(Of Contabilita_Riferimento)
    'Public Property Documenti_Allegati As List(Of Contabilita_Riferimento)

    Public Property Lavorazioni_Associate As String
    Public Property Documenti_Allegati As String
    Public Property DistanzaTrasportoUdm As Integer
    Public Property DistanzaTrasporto As Decimal
    Public Property DescrizioneAggiuntiva As String
    '------------------------------------------------------------
    ''''''''''Public Property DocumentoRighe As List(Of Contabilita_Righe)
    '------------------------------------------------------------

    Public Property N_Nota_Fattura As String
    Public Property Data_Nota_Fattura As Date?

    Public Property N_Nota_DDT_Reso_SDI As String
    Public Property Data_Nota_DDT_Reso_SDI As Date?
    Public Property N_Nota_Riga_DDT_Reso_SDI As String

    Public Property CodPagamento As Integer
    Public Property ModalitaPagamento As Integer

End Class

Public Class Contabilita_Riferimento
    Public Property Des_Lib As String
    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Id_Agenda As Integer
    Public Property Id_Mov As Integer
    Public Property Id_Mov_Det As Integer
    Public Property Lav_Cod As Integer
    Public Property Cau_Mov As String
    Public Property Qta As Decimal
    Public Property Validita_Inizio As Date
    Public Property Validita_Fine As Date
    Public Property Data_Creazione As DateTime
End Class

Public Class Contabilita_Accettazione
    Public Property IdMov As Integer?
    Public Property DataMovimento As Date?
    Public Property DocNumeroSin As String
    Public Property DocNumero As Integer?
    Public Property DocNumeroDes As String
End Class

Public Class Contabilita_Contratto_Affitto
    Public Property IdMov As Integer?
    Public Property DocNumeroSin As String
    Public Property DocNumero As Integer?
    Public Property DocNumeroDes As String
    Public Property DataInizioValidita As DateTime?
    Public Property DataFineValidita As DateTime?
    Public Property AltriLocatori As String
    Public Property RiferimentoOrdini As String
End Class

Public Class Contabilita_Chiave_Mov
    Public Property Piva As String
    Public Property IdAgenda As Integer?
    Public Property SaCod As Integer?
    Public Property IdMov As Integer?
    Public Property CauMov As String
End Class

Public Class Contabilita_Righe_MPC
    Public Property Tipo As String
    Public Property Tipo_Cod As Integer?
    Public Property Val_Cod As String

End Class

Public Class Contabilita_Riga

    'Public Property Ora As DateTime?

    Public Property ChiaveRiga As String
    Public Property Piva As String
    Public Property SaCod As Integer?
    Public Property IdAgenda As Integer?
    Public Property IdMov As Integer?
    Public Property IdMovDet As Integer?

    'Public Property RifMovDettaglio As Movimento_Dettaglio_Riferimento
    Public Property ListRifMovDettaglio As List(Of Movimento_Dettaglio_Riferimento)

    Public Property DestinazioneScarico As Contabilita_Magazzino
    Public Property DestinazioneCarico As Contabilita_Magazzino

    'Public Property Appezza As Integer
    Public Property OrdineDet As Integer?
    Public Property Pendente As Integer?
    Public Property ElemCod As Integer?
    Public Property PuaRegolamento As Integer?
    Public Property ProCod As Integer?
    Public Property MatCod As Integer?
    Public Property MatDes As String
    Public Property MatCodAlias As Integer?
    Public Property VegCod As Integer?
    Public Property CulCod As Integer?
    Public Property N As Decimal?   'Usato solo da FERTILIZZANTI in CAU_CARICO
    Public Property P2O5 As Decimal? 'Usato solo da FERTILIZZANTI in CAU_CARICO
    Public Property K2O As Decimal? 'Usato solo da FERTILIZZANTI in CAU_CARICO
    Public Property Cu As Decimal?  'Usato solo da FERTILIZZANTI in CAU_CARICO
    Public Property BeniStrumentali As String
    Public Property Note As String
    Public Property Lotto As String
    ''''''''''''''TODO DocContabileRighePublic Property AggregaLottoImpianto   '" + getKendoSwitch("chkAggregaLottoImpianto") + "', " + // $('input[name$="chkAggregaLottoImpianto"]').data("kendoMobileSwitch").check()
    Public Property LottoAccettazione As String
    ''''''''''''''TODO DocContabileRighe Public Property ChkParametroQualitativo '" + getKendoSwitch("chkParametroQualitativo") + "', " + // $('input[name$="chkParametroQualitativo"]').data("kendoMobileSwitch").check() 
    Public Property Extra_Str As String
    Public Property jolly_int As Integer?
    Public Property UdM As Integer?
    Public Property ExtraInt As Integer?

    Public Property ExtraDate As Date?
    ''''''''''''''TODO DocContabileRighe Public Property DoseEtichetta: " + KendoNumTB("txtDoseEtichetta") + ", " +
    'Public Property MPC As List(Of Contabilita_Righe_MPC)
    Public Property Quantita As Decimal?
    Public Property NumConfezioni As Decimal?
    Public Property NumContenitori As Decimal?
    Public Property NumImballi As Decimal?
    Public Property KgLordi As Decimal?
    Public Property Tara As Decimal?
    Public Property KgNetti As Decimal?
    Public Property Degrado As Decimal?
    'Public Property KgLordiRiscontrati As Decimal?
    'Public Property TaraRiscontrata As Decimal?
    'Public Property KgNettiRiscontrati As Decimal?
    Public Property PrezzoRiferitoA As enum_PrezzoLivello?
    Public Property Prezzo As Decimal?
    Public Property PrezzoNetto As Decimal?
    Public Property PrezzoEffettivoKgL As Decimal?
    ''''''''''''''TODO DocContabileRighe Public Property Listino: " + KendoDDL("ddlListino").value() + ", " +
    ''' 
    Public Property ScontoModalita As Integer?
    Public Property ScontoBase As Decimal?
    Public Property MaggiorazioneBase As Decimal?
    Public Property ScontoMaggiorazioneBase As Decimal?
    Public Property ScontoBaseEuro As Decimal?

    Public Property ScontoAddiz1 As Decimal?
    Public Property ScontoAddiz2 As Decimal?
    Public Property ScontoAddiz3 As Decimal?
    Public Property ScontoAddizTotalePerc As Decimal?
    Public Property ScontoAddizTotaleEuro As Decimal?
    Public Property ScontoAddizTesto As String

    Public Property ScontoCalcolato As Decimal?
    Public Property ScontoCalcolatoEuro As Decimal?
    Public Property ScontoMagg As String

    Public Property CodIva As Integer?
    Public Property AliquotaIva As Decimal?
    Public Property Iva As Decimal?
    Public Property ForzaIva As Boolean


    ''' <see cref="AgronicaCoreDataProvider.TipiEnumerativi.enum_ModalitaIva"/>
    'In realtà è ricavata dal lavCod, quindi sarebbe un'informazione di testata
    Public Property IvaModalita As Integer?

    Public Property IvaIndetraibilePerc As Decimal?
    Public Property IvaIndetraibile As Decimal?
    Public Property IvaCreditoDebito As Decimal?

    Public Property ImponibileTotale As Decimal?
    Public Property ImponibileTotaleNetto As Decimal?
    Public Property ImportoUnitario As Decimal?
    Public Property ImportoTotale As Decimal?

    Public Property ValoreRiferimentoPrezzo As enum_EditImporto?
    Public Property Anno As Integer?
    Public Property ContoEconomico As Integer?
    Public Property ContoPatrimoniale As Integer?
    Public Property ProvvigionePercAgente As Decimal?
    Public Property ProvvigioneAgente As Decimal?
    Public Property ProvvigionePercCapoArea As Decimal?
    Public Property ProvvigioneCapoArea As Decimal?

    'Sono info di testata usate dall'ordine ma è necessario che vengano scritte uguali anche su tutte le righe
    Public Property N_Doc_Cliente As String
    Public Property Data_Doc_Cliente As Date?

    Public Property CalCod As Integer?
    Public Property MateriePrimeCampionature As List(Of Materia_Prima_Campionatura)

    Public Property GHG_Registrazioni As List(Of GHG_Registrazione)

    Public Property Cod_Progetto As Integer?
    Public Property Udm_Cod_Extra As Integer?
    Public Property Qta_Extra As Decimal?

    Public Property Ric_Cod As Integer?
    Public Property Contabilizzato As Integer?

    Public Property DataOraUltimaLettura As DateTime?

    'TODO: Parcheggiati qui (temporaneamente, capire dove infilare)
    Public Property LblAsteriscoImportoUnitario As String
    Public Property LblAsteriscoImportoTotale As String

    'In realtà è ricavata dal sezionale, quindi sarebbe un'informazione di testata
    Public Property EsigibilitaIva As Integer?

    'Utilizzate per parcheggiare dei dati per la scrittura su db (i 210 in FF vanno salvati in maniera diversa, cambiate rispetto a quello che vedo, ecc..)
    Public Property DB_Qta As Decimal?
    Public Property DB_Qta_Extra As Decimal?
    Public Property DB_Udm_Cod As Integer?
    Public Property DB_Udm_Cod_Extra As Integer?
    Public Property DB_Prezzo_Unitario As Decimal?
    Public Property DB_Prezzo_Unitario_Netto As Decimal?

    'Dati non specificatamente di riga, ma necessari per salvataggio o ricalcolo
    Public Property ModuloGias As enum_Omni_Modulo_Generazione
    Public Property Fornitore As Integer?

    Public Property Confezionamenti As List(Of Contabilita_Riga_Confezionamento)

    'Per raccolta collegata ad accettazione
    Public Property RigheImpianti As List(Of Contabilita_Impianti_Raccolta)
    Public Property ChkImpiantiIndefiniti As Boolean
    Public Property PivaConferente As String
    Public Property NoteRaccolta As String

    'Valori riscontrati/fatturati
    Public Property Riscontrati_PesoNetto As Decimal?
    Public Property Riscontrati_PesoLordo As Decimal?

    Public Property Rif_Esterno As String
    Public Property Rif_Esterno_2 As String


    ''' <summary>
    ''' Usato per il conferimento speciale del pomodoro
    ''' </summary>
    Public Property Conferimento_Speciale As Contabilita_Conferimento_Speciale

    Public Property Raccolte As List(Of Movimento_Dettaglio)

    ''' <summary>
    ''' Indica, in caso di conferimento, se le raccolte collegate sono create automaticamente oppure se sono pre-esistenti e semplicemente associate
    ''' </summary>
    ''' <returns></returns>
    Public Property Tipo_Associazione As Integer

    'Terzetto di proprietà appartenenti a mov_dettaglio_tecnico_extra. Si usano per indicare un eventuale riferimento ad un documento del gestionale esterno
    Public Property N_Nota_DDT As String
    Public Property N_Nota_Riga_DDT As String
    Public Property Data_Nota_DDT As Date?


    ''' <summary>
    ''' Copia i valori di una riga origine, azzerando le chiavi di Id_Mov_Det
    ''' </summary>
    Public Function Clona(Optional ByVal numRigaConfezionamento As Integer = -1
                          ) As Contabilita_Riga

        Dim clone As Contabilita_Riga = Me.DeepCloneObject()

        clone.IdMovDet = 0

        'TODO: devo azzerare anche il valore di Id_Mov_Det (e altre chiavi) all'interno della stringa chiave
        'clone.ChiaveRiga = ""

        'Devo azzerare e rigenerare anche un nuovo cal_cod
        clone.CalCod = 0
        For Each materiaPrimaCampionatura As Materia_Prima_Campionatura In clone.MateriePrimeCampionature
            materiaPrimaCampionatura.Progressivo = 0
        Next

        If numRigaConfezionamento <> -1 Then
            'So che devo mantenere solo una certa riga (numRigaConfezionamento) dalla tabella dei confezionamenti

            clone.Confezionamenti = New List(Of Contabilita_Riga_Confezionamento) From {clone.Confezionamenti(numRigaConfezionamento)}

            'Devo aggiornare tutti i contatori che stanno su Contabilita_Riga che derivano direttamente dal contenuto di .Confezionamenti
            clone.NumImballi = If(clone.Confezionamenti(0).NrImballaggi, 0)
            clone.NumContenitori = If(clone.Confezionamenti(0).NrContenitori, 0)
            clone.NumConfezioni = If(clone.Confezionamenti(0).NrConfezioni, 0)

        End If

        Return clone

    End Function


End Class

Public Class Contabilita_Conferimento_Speciale
    Public Property FaseCodContratto As Integer?
    Public Property TagliandoPesa As String
    Public Property PremioComplessivo As Decimal?
    Public Property CodVarieta As Integer?
    Public Property DescAppezzamenti As String
End Class

Public Class Contabilita_Impianti_Raccolta
    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Appezza As Integer
    Public Property Id_Reg As Integer
    Public Property Veg_Des As String
    Public Property Qta As Decimal
    Public Property Sup_Imp As Decimal
End Class

Public Class Contabilita_Riga_Confezionamento
    Public Property FF_imballaggio_Tipo_Cod As Integer?
    Public Property FF_imballaggio_Descrizione As String
    Public Property NrImballaggi As Integer?
    Public Property FF_imballaggio_Codice_Generazione_Link As Integer?
    Public Property FF_imballaggio_Mat_Cod_Generazione_Link As Integer?
    Public Property FF_imballaggio_Mat_Cod As Integer?
    Public Property FF_imballaggio_Tara_Campionatura As Decimal?
    Public Property Num_Imballi_Riscontrati As Integer?
    Public Property Tara_Unit_Imballo_Riscontrata As Decimal?

    Public Property FF_contenitore_Tipo_Cod As Integer?
    Public Property FF_contenitore_Descrizione As String
    Public Property FF_contenitore_Codice_Generazione_Link As Integer?
    Public Property FF_contenitore_Mat_Cod_Generazione_Link As Integer?
    Public Property FF_contenitore_Mat_Cod As Integer?
    Public Property NrContenitori As Integer?
    Public Property FF_contenitore_Tara_Campionatura As Decimal?
    Public Property Num_Colli_Riscontrati As Integer?
    Public Property Tara_Unit_Collo_Riscontrata As Decimal?

    Public Property FF_confezione_Tipo_Cod As Integer?
    Public Property FF_confezione_Descrizione As String
    Public Property FF_confezione_Codice_Generazione_Link As Integer?
    Public Property FF_confezione_Mat_Cod_Generazione_Link As Integer?
    Public Property FF_confezione_Mat_Cod As Integer?
    Public Property NrConfezioni As Integer?
    Public Property FF_confezione_Tara_Campionatura As Decimal?
    Public Property Num_Conf_Riscontrate As Integer?
    Public Property Tara_Unit_Conf_Riscontrata As Decimal?

    'Public Property ContPerImb As Decimal?
    'Public Property ConfPerCont As Decimal?

End Class

Public Class Contabilita_Magazzino
    Public Property Appezza As Integer?
    Public Property TipoDestinazione As Integer?
    Public Property SaCod As Integer?
    Public Property IdDestinazione As Integer?

    Public Sub New()
        Appezza = 0
        TipoDestinazione = 0
        SaCod = 0
        IdDestinazione = 0
    End Sub

End Class

Public Class Contabilita_Listino
    Public Property ListinoCod As Integer
    Public Property Prezzo As Decimal
    Public Property UdmCod As Integer
    Public Property ScontoCondizione1 As Integer
    Public Property ScontoCondizioneValore1 As Decimal
    Public Property ScontoAdd1 As Decimal
    Public Property ScontoCondizione2 As Integer
    Public Property ScontoCondizioneValore2 As Decimal
    Public Property ScontoAdd2 As Decimal
    Public Property ScontoCondizione3 As Integer
    Public Property ScontoCondizioneValore3 As Decimal
    Public Property ScontoAdd3 As Decimal
    Public Property CodIva As Integer
    Public Property TipoIvaDet As enum_TipoIVAListino

    Public Sub New()
        'ListinoCod = 0
        Prezzo = 0
        UdmCod = 0
        ScontoCondizione1 = 0
        ScontoCondizioneValore1 = 0
        ScontoAdd1 = 0
        ScontoCondizione2 = 0
        ScontoCondizioneValore2 = 0
        ScontoAdd2 = 0
        ScontoCondizione3 = 0
        ScontoCondizioneValore3 = 0
        ScontoAdd3 = 0
        CodIva = -1
        TipoIvaDet = enum_TipoIVAListino.IVA_Esclusa
    End Sub
End Class

Public Class Contabilita_Confezionamento
    Public Property SaCod As Integer
    Public Property Tipo As enum_OTabelle
    Public Property Quantita As Integer
    Public Property ElemCod As Integer
    Public Property Codice As Integer
    Public Property Descrizione As String
    Public Property Tara_Unitaria As Decimal

    Public Sub New()
        SaCod = 0
        Tipo = enum_OTabelle.Nessuno
        Quantita = 0
        ElemCod = 0
        Codice = 0
        Descrizione = ""
        Tara_Unitaria = 0
    End Sub
End Class

Public Class Contabilita_Totali_Testata
    Public Property Piva As String
    Public Property IdAgenda As Integer
    Public Property IdMov As Integer
    'Public Property IdRegDettaglio As Integer?
    'Public Property Peso As Decimal
    'Public Property TipoPeso As Integer
    Public Property Colli As Decimal
    Public Property PesoNettoProd As Decimal
    Public Property TaraImballi As Decimal
    Public Property PesoLordoProd As Decimal
    Public Property ImballiVuoti As Decimal
    Public Property TaraVeicolo As Decimal
    Public Property PesoLordoDoc As Decimal?
    Public Property ProvvigioneAgente As Decimal
    Public Property ProvvigioneCapoArea As Decimal
    Public Property RiepilogoImporti As Contabilita_Riepilogo_Importi
    Public Property RiepilogoCastelletto As String


    Public Sub New()
        Colli = 0
        PesoNettoProd = 0
        TaraImballi = 0
        PesoLordoProd = 0
        ImballiVuoti = 0
        TaraVeicolo = 0
    End Sub

End Class

Public Class Contabilita_Riepilogo_Importi
    Public Property ImponibileLordo As Decimal
    Public Property Variazioni As Decimal
    Public Property ImponibileNetto As Decimal
    Public Property Imposta As Decimal
    Public Property TotaleDocumento As Decimal
End Class

Public Class Contabilita_Evasione_Ordine
    Public Property Stato_Cod As enum_StatoOrdine
    Public Property Stato_Des As String

    Public Sub New()
        Stato_Cod = enum_StatoOrdine.INDEFINITO
        Stato_Des = ""
    End Sub
End Class

Public Class Contabilita_Castelletto_Iva
    Public Property Cod_Iva As Integer
    Public Property Aliquota_Iva As Decimal
    Public Property Iva_Des As String
    Public Property Imponibile_Lordo As Decimal
    Public Property Imponibile_Netto As Decimal
    Public Property Iva As Decimal
    Public Property Imponibile_Campioni_Omaggio_Detrazione As Decimal
    Public Property ImponibilexImposta_Campioni_Omaggio_Detrazione As Decimal

    Public Sub New()
        Cod_Iva = -1
        Aliquota_Iva = 0D
        Iva_Des = ""
        Imponibile_Lordo = 0D
        Imponibile_Netto = 0D
        Iva = 0D
        Imponibile_Campioni_Omaggio_Detrazione = 0D
        ImponibilexImposta_Campioni_Omaggio_Detrazione = 0D
    End Sub
End Class

''' <summary>
''' Mi serve per passarmi vari parametri al termine della scrittura, come i vari id assegnati
''' </summary>
Public Class Contabilita_Output
    Public Property Risultato As Boolean
    Public Property MsgError As String
    Public Property Time As DateTime
    Public Property Id_Agenda As Integer?
    Public Property Id_Mov_Testata As Integer?
    Public Property Id_Mov_Secondario As Integer?
    Public Property Id_Mov_Carico As Integer?
    Public Property Id_Mov_Scarico As Integer?
    Public Property Id_Mov_Det As Integer?
    Public Property Des_Lib As String
    Public Property Doc_Numero As Integer?
    Public Property Doc_Numero_Visualizzato As String
    Public Property MovimentoMagazzino As Contabilita_Chiave_Mov
    Public Property ConteggiTotali As Contabilita_Totali_Testata
    Public Property Progr_Protocollo As Integer?

    Public Sub New()
        Risultato = False
        MsgError = ""
        Time = Now
        Des_Lib = ""
    End Sub

End Class

Public Class Contabilita_Output_ModData
    Public Property MsgError As String
    Public Property PivaProduttore As String
    Public Property AgendeProduttore As List(Of Integer)

    Public Sub New()
        MsgError = ""
        PivaProduttore = ""
        AgendeProduttore = New List(Of Integer)
    End Sub
End Class

Public Class Contabilita_Impostazioni

    ''' <see cref="enum_Impostazioni_Utenti.SUPERUSER_COD_IVA_DEFAULT"/>
    Public Property SUPERUSER_COD_IVA_DEFAULT As Integer
    Public Property ConfigurazioneCategorie As List(Of Contabilita_Configurazione_Categoria)

    Public Sub New()
        SUPERUSER_COD_IVA_DEFAULT = -1
    End Sub

    Public Sub ValorizzaCodIvaDefault(ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim objImpoR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim valCod As String = objImpoR.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_IVA_DEFAULT,
                                                                                 objParametriUtenti, 2)

        If valCod <> "" Then
            SUPERUSER_COD_IVA_DEFAULT = CInt(valCod)
        End If
    End Sub

    Public Sub ValorizzaConfigurazioneCategorie(ByVal piva As String,
                                                ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'TODO: richiama DAL di lettura Categorie_Configurazione (da scrivere)
        'TODO: implementare lettura di tabella Categorie_Configurazione (in EF o SQL [sql non esiste])
    End Sub
End Class

Public Class Contabilita_Configurazione_Categoria
    Public Property ElemCod As Integer
    Public Property UdmCodDefault As Integer
    Public Property IvaCodDefault As Integer

    Public Sub New()
        UdmCodDefault = 0
        IvaCodDefault = -1
    End Sub
End Class

Public Class Raccolta_Aggiorna
    Public Property Piva As String
    Public Property Id_Agenda As Integer
    Public Property Movimento_Campagna As Movimento
    Public Property Movimento_Carico As Movimento
    Public Property Mov_Dettagli_Campagna As Movimento_Dettaglio
    Public Property Mov_Dettagli_Carico As Movimento_Dettaglio
    Public Property Mov_Destinazioni_Campagna As List(Of Movimento_Destinazione)
    Public Property Mov_Destinazioni_Carico As Movimento_Destinazione
End Class

Public Class Raccolta_Group_Qta
    Public Property Id_Agenda As Integer
    Public Property Mat_Cod As Integer
    Public Property Lotto As String
    Public Property QuotaDistribuzione As Decimal
    Public Property VecchiaQtaRaccolta As Decimal
    Public Property NuovaQtaRaccoltaConf As Decimal
    Public Property QtaDetCarico As Decimal
End Class