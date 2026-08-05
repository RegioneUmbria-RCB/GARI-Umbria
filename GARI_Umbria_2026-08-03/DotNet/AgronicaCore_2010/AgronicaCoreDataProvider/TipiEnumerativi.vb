Public Class TipiEnumerativi

    Public Shared Function GetEnumInt(Of T)(enumVal As T) As Integer
        Return Convert.ToInt32(enumVal)
    End Function

    '======================================================
    'TIPO IMPRESA GERARCHIA:
    'TipoImpresaGerarchia = 1 ----> IMPRESA
    'TipoImpresaGerarchia = 2 ----> COOPERATIVA
    'TipoImpresaGerarchia = 3 ----> CONSORZIO
    'TipoImpresaGerarchia = 4 ----> OP
    '======================================================
    'TITOLO POSSESSO: enum_titolopossesso
    '=======================================================
    'INDIRIZZI

    'Persona fisica
    'Codice            Tipo indirizzo
    '2	                Domicilio
    '3	                Residenza
    '4	                Residenza Estiva
    '5	                Luogo di nascita

    'Persona giuridica
    'Codice            Tipo indirizzo
    '1	                Sede operativa
    '101	            Sede legale
    '102	            Sede aziendale
    '103	            Stabilimento
    '201                Stabile Organizzazione (indirizzo italiano per contatto estero)
    '=======================================================

    Public Enum enum_TipiIndirizzi

        TipoIndirizzoDefaultAnagrafica = 1
        'PF
        Domicilio = 2
        Residenza = 3
        ResidenzaEstiva = 4
        LuogoDiNascita = 5
        'PG
        SedeOperativa = 1
        SedeLegale = 101
        SedeAziendale = 102
        Stabilimento = 103
        StabileOrganizzazione = 201
    End Enum

    Public Enum enum_Sql_TipoTroncamentoData
        Nessuno = 0
        Ora = 1
        SoloParteData = 2
    End Enum

    Public Enum enum_TipoDebug

        Off = 0
        Soft = 1
        Verbose = 2

    End Enum

    Public Enum enum_Security_Attivita

        Nessuna = 0

        '############################################################
        '------------ GIASONLINE  ------------

        Gest_Menu = 1

        Gest_AvvisiMessaggi = 2
        Gest_Messaggi_AccessoMenu = 2

        Gest_UtentiPermessi = 3

        Gest_AnagraficaAzienda = 4
        Anagrafica_AccessoMenu = 4

        Gest_Contabilita = 5

        Sementieri_UtenteVedeTutto = 6

        Gest_Stampe = 7
        Gest_Certificazioni = 8
        Gest_VerificaScadenze = 9
        Gest_Magazzino = 10
        Agenda_AccessoMenu = 11
        Gest_CartografiaAziendale = 12
        Gest_UtentiImpostazioni = 13

        'BUCO 14 15 16 17

        Gest_ImpiantiVegetali = 18 'vecchio!!! non va più usato!!!
        Gest_Prodotti = 19

        Gest_Stalle = 20
        Gest_AnalisiCosti = 21
        Gest_ValidazioneDati = 22

        Gest_Certificati_AccessoMenu = 23
        Gest_Certificati_AttivaDisattiva = 24
        Gest_Certificati_M005 = 25
        Gest_Certificati_M006 = 26
        Gest_Certificati_M007 = 27
        Gest_Certificati_M014 = 28

        Gest_CartellaAziendale_PAP_Vegetale = 29
        Gest_CartellaAziendale_Notifiche = 30
        Gest_CartellaAziendale_VisiteIspettive = 31
        Gest_CartellaAziendale_Irregolarita = 32

        Gest_Ingredienti = 33

        Gest_CartellaAziendale_DelibereComm = 34
        Gest_CartellaAziendale_PAP_Zootecnico = 35
        Gest_CartellaAziendale_PAP_Preparazioni = 36
        Gest_CartellaAziendale_Deroghe = 37
        Gest_CartellaAziendale_AccessoMenu = 38
        Gest_CartellaAziendale_CompartoFiliera = 39

        Gest_Messaggi_SegreteriaTecnica = 40
        '??????????????
        ' Ricezione_Messaggi_UST = 40

        'ManutenzioneArchivi_CoefficientiUBA = 41
        Gest_CartellaAziendale_PeriodiControllo = 42

        ManutenzioneArchivi_SuperficieParticelle = 43
        ManutenzioneArchivi_AccessoMenu = 44

        'BUCO 45

        ManutenzioneArchivi_ModificaPIVA = 46
        ManutenzioneArchivi_GestioneVarieta = 47
        ManutenzioneArchivi_GestioneFormulati = 48
        ManutenzioneArchivi_GestioneDisciplinari = 49
        'ManutenzioneArchivi_ImportiSpecieVegetali = 50
        'ManutenzioneArchivi_GestioneFasiFenologiche = 51
        'ManutenzioneArchivi_GestioneCalibriFrutti = 52
        'ManutenzioneArchivi_ImportiQuoteControllo = 53

        'BUCO 54

        'ManutenzioneArchivi_GestioneIndiciMaturita = 55

        Anagrafica_Impresa = 56
        Anagrafica_CentroAziendale = 57
        Anagrafica_Campo = 58
        Anagrafica_Appezzamento = 59
        Anagrafica_Impianto = 60
        Anagrafica_Fabbricato = 61
        Anagrafica_ParticellaCatastale = 62
        Anagrafica_Contatto = 63
        Anagrafica_ParcoMacchine = 64

        ManutenzioneArchivi_SbloccaAnagrafe = 65
        'ManutenzioneArchivi_GestioneSpecieAnimali = 66

        Anagrafica_GestioneAllegati = 67

        'ManutenzioneArchivi_PianificazioneInterventi = 68
        Agenda_OperazioniMultiAziendali = 68

        ManutenzioneArchivi_MultiCancellazioneInterventi = 69
        ManutenzioneArchivi_GestioneFertilizzanti = 70

        Gest_Analisi_AccessoMenu = 71
        Gest_Analisi_Cartografia = 72

        'ManutenzioneArchivi_GestioneTipologieVarietali = 73
        ManutenzioneArchivi_GestioneSpecieVegetali = 74

        Stampe_Esportazione_OP_Inv = 75

        Stampe_Esportatore_Universale = 76

        SupportoDecisioni_AccessoMenu = 77
        SupportoDecisioni_PianoConcimazione = 78

        Agenda_Operazioni_Blocco = 79
        Agenda_Operazioni_Sblocco = 80

        ManutenzioneArchivi_GestioneMigrazionePoliennale = 81

        'Report Riconversione Varietale x AgriBologna
        Stampe_Riconversione_Varietale = 82

        VerificaConformita_Richieste = 83
        VerificaConformita_Gestione = 84

        ManutenzioneArchivi_RevisioneDB = 85

        'Esportazione Rintraccio x ARP
        Stampe_Esportazione_Rintraccio = 86

        'Report Impegno Produzione Soci x AgriBologna
        Stampe_Impegno_Produzione_Soci = 87

        Stampe_Esportazione_OP_Gest = 88

        ManutenzioneArchivi_ImportaAnagrafiche_XLS2GIAS = 89

        Gest_Ricette = 90

        Anagrafica_RapportiContabili = 91

        'Stampe_Bolle_Fatture = 92
        Stampe_Contabilita = 92

        ACC_Configurazione = 93

        ACC_Promozioni = 94

        ACC_Vetrina = 95

        ACC_AccessoMenu = 96

        ManutenzioneArchivi_CodificaProdottiAziendali = 97

        ManutenzioneArchivi_ModificaCodContatto = 98

        ManutenzioneArchivi_ImportaAnagraficheAnimali_XLS2GIAS = 99

        Gest_CartellaAziendale_Condizionalita = 100

        ManutenzioneArchivi_Esporta_CodificheCultivar = 101

        ManutenzioneArchivi_Esporta_CodificheSpecieVegetali = 102

        Gest_Pianificazione_Produzione_Vegetale = 103

        Gest_PUA = 104

        Stampe_Esportazione_OP_Gest_Coop = 105

        Registri_Cantina = 106

        ManutenzioneArchivi_Importa_DDTRicevuti = 107
        ManutenzioneArchivi_Importa_RaccolteConferimenti_DaRintraccio = 108
        ManutenzioneArchivi_Importa_DDTRicevuti_AgriOK = 109
        ManutenzioneArchivi_Importa_Anagrafe_EmiliaRomagna = 110
        ManutenzioneArchivi_EsportaXMLAnagrafiche_GIAS = 111
        ManutenzioneArchivi_EsportaConferimenti_JDEdwards = 112
        ManutenzioneArchivi_Importa_AGREA = 113
        ManutenzioneArchivi_Importa_AVEPA = 114


        Contabilita_Operazioni_Blocco = 115
        Contabilita_Operazioni_Sblocco = 116

        ManutenzioneArchivi_ImpostazionePasswordServizi = 117

        ManutenzioneArchivi_Importa_DDT_Pianocolturale = 118

        Raccolta_Dati_PAC_UMA_Brogliaccio = 119

        ManutenzioneArchivi_Importa_Anagrafe_BA = 120

        CheckList_Sicurezza_Lavoro = 121

        Report_Accettazione_DaDiversi = 122

        ManutenzioneArchivi_EsportaConferimentiPomodoro_Agrea = 123

        Gest_CartellaAziendale_MonitoraggioCE = 124

        ManutenzioneArchivi_BolleAccettazione2AltroCliente = 125

        Stampe_RegistroCaricoScarico_Pomodoro = 126

        ProfilazioneImpresa = 127

        Utilizzo_Dati_Meteo = 128

        ManutenzioneArchivi_Importa_UfficiZona = 129

        ManutenzioneArchivi_Importa_AGREA_Massiva = 130

        PianiCampionamento_GestioneLotti = 131
        PianiCampionamento_GestioneWorkFlow = 132
        PianiCampionamento_GestioneAnalisi = 133
        PianiCampionamento_InterrogazioneBloccoSblocco = 134

        Anagrafica_MultiModificaSupImp = 135

        ManutenzioneArchivi_SincronizzazioneHarvard = 136


        Profilazione_DefaultSpecie_Globali = 137

        Gest_CartellaAziendale_GlobalGap = 138
        ManutenzioneArchivi_Importa_AGREA_OP = 139

        ManutenzioneArchivi_Import_DDTFatture_Seled = 140

        ManutenzioneArchivi_SincronizzazioneArcView = 141

        ManutenzioneArchivi_Importa_Giacenze = 142

        ManutenzioneArchivi_Importa_AVEPA_Vino = 143

        ManutenzioneArchivi_ImportDateRaccolta_Harvard = 144

        Statistiche_Sito = 145

        ManutenzioneArchivi_Import_Anagrafiche_Seled = 146

        ManutenzioneArchivi_Importa_Notifica_AgriBio = 147

        ManutenzioneArchivi_SincroRaccoltaCCCI = 148

        Anagrafica_Appezzamento_CancellazioneMultipla = 149

        PianiSemina_Tabelle = 150
        PianiSemina_Gestione = 151

        Meteo_Tabelle = 152

        Gestione_Etichette = 153

        PianiCampionamento_GestioneAnalisiXLaboratori = 154

        ManutenzioneArchivi_SincroCatastoImportPianificazioneAccessOP = 155

        Gest_CartellaAziendale_CheckCOOP = 156

        PianiCampionamento_GestioneImpostazioni = 157
        PianiCampionamento_GestionePianoProduttivo = 158
        PianiCampionamento_GestionePianoCampioni = 159
        PianiCampionamento_ControlloValiditaCapitolati = 160
        PianiCampionamento_Menu = 161
        PianiCampionamento_ControlloValiditaQDC = 162
        PianiCampionamento_GestioneBloccoSblocco = 163
        PianiCampionamento_GestioneLaboratori = 164

        SMART_RegistrazioneSmart = 165
        SMART_AgendaOperazioniColturali = 166
        SMART_GiasProfitosan = 167
        SMART_GIS = 168
        SMART_gestioneAnagrafica = 169
        SMART_NuovaAzienda = 170
        SMART_Nuovo_Centro = 171
        SMART_NuovoAppezzamento = 172
        SMART_NuovoContatto = 173
        SMART_Gias = 174
        SMART_Menu = 175
        SMART_GestioneAnagrafica_Nodo = 176

        ManutenzioneArchivi_AllineaCarenze = 177

        manutenzioneArchivi_Esportazione_SIGPA = 178

        Rilievo_Attivita = 179

        Blocco_Modifica_PianificazioneVegetale = 180

        Creazione_Automatica_CentriAziendali = 181

        PianiCampionamento_Filtrone = 182

        Budget_Menu = 183

        Budget_Testata = 184

        Budget_Import_Da_Anagrafe = 185

        Budget_Dettagli_del_Piano = 186

        Report_Incongruenze_CatastoVSAgrea = 187

        Gestione_Servizi = 188

        Stampa_Inglese = 189

        Stampe_Italiano = 190

        ManutenzioneArchivi_Importazione_Massiva_Anagrafe_BA = 191

        ManutenzioneArchivi_Gias_2_Gias = 192

        LinkGiasStrandard = 193

        ManutenzioneArchivi_ImportaMagazzino_XmlPubblico = 194

        ManutenzioneArchivi_Sincronizzatore_Apofruit = 195

        Rilevamento_Smart_Gis = 196

        ManutenzioneArchivi_Sincronizzatore_Valdoca = 197

        Scadenziario_Menu = 198

        Precision_Farming = 199

        Link_AgronicaSementi = 200

        Modifica_Contatti_Pubblici = 201

        Cartografia_Catasto = 202

        Cartografia_Esporta_Dati = 203

        ManutenzioneArchivi_Importazione_Anagrafiche_Agenda_Seled = 204

        ManutenzioneArchivi_Importazione_Anagrafiche_Viticolo_Avepa = 205

        SupportoDecisioni_PianoConcimazioneMassivo = 206

        PrenotazionePiante_Menu = 207

        Creazione_Automatica_SemilavoratiVegetali = 208

        ManutenzioneArchivi_Importazione_Catasto_Uniforma = 209

        Consultazione_Meteo_Dati_Stazioni_Metos = 210

        ManutenzioneArchivi_Importazione_Catasto_CantinaSoave = 211
        'ManutenzioneArchivi_Importazione_Catasto_OPTA = 212

        PROFITOSAN_Elenco_Prodotti_DPI = 213

        Gest_CartellaAziendale_Check_SchedaTecnicaTTI = 214
        Gest_CartellaAziendale_Check_SchedaControlliTTI = 215

        manutenzioneArchivi_VerificaEsportazioneAgendaVerso_SIGPA = 216

        ManutenzioneArchivi_Importazione_OptaMacchine = 217
        ManutenzioneArchivi_Importazione_OptaParticelle = 218

        Gest_Pianificazione_Produzione_Vegetale_Ribalta = 219

        ManutenzioneArchivi_Importa_Anagrafe_BA_Veneto = 220

        Esportazione_RIBA_CBI = 221

        NonConformita = 222

        Agenda_Operazione_Di_Cura = 223

        ManutenzioneArchivi_Importazione_Anagrafiche_Brogliaccio_SIAN = 224

        SupportoDecisioni_LaboratorioControlloQualita = 225

        Stampe_Esportazione_OP_Produttori = 226
        Stampe_Esportazione_OP_Catasto = 227

        Rintraccio_Operazione_Di_Cura = 228

        ManutenzioneArchivi_Importa_AVEPA_Catasto = 229

        Stampa_MovimentiMagazziniExcel = 230

        SupportoDecisioni_LaboratorioControlloQualita_CreaApriDoc = 231
        SupportoDecisioni_LaboratorioControlloQualita_Impostazioni = 232
        SupportoDecisioni_LaboratorioControlloQualita_FiltraEsporta = 233

        ManutenzioneArchivi_Importazione_Anagrafiche_COFRUTA = 234

        Gest_CartellaAziendale_Check_AnalisiDatiCura = 235

        ManutenzioneArchivi_Importazione_Anagrafiche_APOL = 236

        AnalisiSchedeRilievi = 237

        Rintracciabilità = 238

        Stampa_SchedaCampagna_Massiva = 240

        Gestione_Ordini_Piante = 241

        ManutenzioneArchivi_Importazione_Anagrafiche_APOT = 242

        Gest_UtentiProfili = 243
        Gest_UtentiGruppi = 244

        ManutenzioneArchivi_Importazione_Anagrafiche_SISCO = 245

        Macchine_Assegnazione_Pubblica = 246

        Gest_UtentiAgendaBlocchi = 247

        ManutenzioneArchivi_Importazione_Anagrafiche_Casalasco = 248

        ReportPercorsi = 249

        EsportazioneZespri = 250

        EsportazioneSQNPI = 251

        CheckList_Pratiche_Ecologiche_APOT = 252

        CheckList_Formazione = 253

        Gestione_Rifiuti = 254

        ManutenzioneArchivi_MultiModificaInterventi = 255

        NonConformita_Impostazioni = 256
        NonConformita_FiltraEsporta = 257
        NonConformita_Anagrafiche = 258

        Cartografia_BufferZone = 259

        Gest_UtentiImpostazioni_Avanzate = 265
        ManutenzioneArchivi_Importa_ARPEA = 279
        FiltraEdEsporta_PianiColturali = 280
        ManutenzioneArchivi_Importa_ARTEA = 288
        SupportoDecisioni_VerificaConformitaIAF = 289
        Esportazione_RegioneER = 290
        Importazione_SIARL = 291
        ManutenzioneArchivi_Importazione_Anagrafiche_CIO = 292
        ManutenzioneArchivi_EsportazioneConferimentiFF = 293
        ManutenzioneArchivi_Importa_AGEA_RealTime = 296
        ManutenzioneArchivi_Importa_AGEA_Coordinamento_Massiva = 298
        ManutenzioneArchivi_Importa_AGEA_GIS = 299
        '############################################################
        '------------ F&F Campionamento e Liquidazioni ------------
        FrashAndFood = 260
        FF_Conferimenti = 261
        FF_CampionamentoLiquidazioni_Anag = 262
        FF_CampionamentoLiquidazioni_Movimenti = 263
        FF_CampionamentoLiquidazioni_Liquidazioni = 264
        FF_CampionamentoLiquidazioni_ValorUnaTantumAUltimoListino = 422
        '------------ F&F Campionamento e Liquidazioni ------------

        NonConformita_Testata_Crea = 266
        NonConformita_Testata_Modifica = 267
        NonConformita_Testata_Chiudi = 268
        NonConformita_Testata_Elimina = 269
        NonConformita_Fase_Crea = 270
        NonConformita_Fase_Modifica = 271
        NonConformita_Fase_Chiudi = 272
        NonConformita_Fase_Elimina = 273

        Analisi_Dati_Meteo = 274

        Analisi_Curve_Maturazione = 275

        NonConformita_Lista = 276
        NonConformita_Lista_AncheDiAltriUtenti = 277

        Analisi_Modelli_Previsionali = 278

        GestioneAvanzataETabelleLookUp = 281

        ReportSostenibilità = 282

        Scadenzario_Lista = 283
        Scadenzario_IndiciRicerca = 284

        Visite_Lista = 285
        Visite_Anagrafiche = 286

        invioSMS = 287

        Scadenzario_Impostazioni = 294

        Gest_Prodotti_VisibilitaPubblica = 295

        ManutenzioneArchivi_Import_Utenti_Da_Excel = 297

        '############################################################
        '------------ AGRONICA MANUTENZIONE ------------
        'i permessi dell'Agronica Manutenzione vanno dal 300 al 349
        AgronicaManutenzione_AccessoMenu = 300
        AgronicaManutenzione_SpecieVegetali = 301
        AgronicaManutenzione_Varieta = 302
        AgronicaManutenzione_CalibriFrutti = 303
        AgronicaManutenzione_IndiciMaturita = 304
        AgronicaManutenzione_FasiFenologiche = 305
        AgronicaManutenzione_TipologieVarietali = 306
        AgronicaManutenzione_GruppoFinalita = 307
        AgronicaManutenzione_FormeAllevamento = 308
        AgronicaManutenzione_ImpiantiIrrigazione = 309
        AgronicaManutenzione_Portinnesti = 310
        AgronicaManutenzione_UnitaMisura = 311
        AgronicaManutenzione_MisuraAvversita = 312
        AgronicaManutenzione_Fertilizzanti = 313
        AgronicaManutenzione_Formulati = 314
        AgronicaManutenzione_Disciplinari = 315
        AgronicaManutenzione_CoefficientiUBA = 316
        AgronicaManutenzione_SpecieAnimali = 317
        'ECC...
        '------------ AGRONICA MANUTENZIONE ------------


        '############################################################
        ' Fresh & Food Magazzino
        FF_Magazzino = 350
        FF_Lavorazioni_PC = 351
        FF_Lavorazioni_Terminalino = 352
        '------------ Fresh & Food Magazzino ------------

        ManutenzioneArchivi_ImportazioneHarvard = 353

        ReportRaccolteGIS = 354

        Brogliaccio = 355

        ManutenzioneArchivi_ImportazioneRicetteDaInterscambioApp = 356
        ManutenzioneArchivi_ImportazioneAnagraficheZespri = 357
        ManutenzioneArchivi_ImportazioneQDCZespri = 358
        ManutenzioneArchivi_EsportazioneAGEA = 359
        GIS_SAT_AnalisiDatiSatellitari = 360

        ' Fatturazione Elettronica
        Contabilita_FattElettronica = 361

        ' gestione di permessi per chiamate a web service di provisioning (AgronicaWebApiProfilatore)
        Provisioning_agronica = 362

        ManutenzioneArchivi_EsportazioneAgendaSISCO = 363
        ManutenzioneArchivi_EsportazioneEuresys = 364
        ManutenzioneArchivi_ConfigurazioneServizi = 365

        Gest_PUA_2 = 366

        G2GFiltraDatiPerInvioGias2Gias = 367

        GIS_VisualizzazioneGestioneWMS = 368

        Gestione_Listini = 369
        Ordini_Acquisto = 370
        Consegne_Acquisto = 371
        Fatture_Acquisto = 372
        Ordini_Vendita = 373
        Consegne_Vendita = 374
        Fatture_Vendita = 375
        Correlazione_Righe_Vendita = 376
        Statistiche_Vendita = 377
        Consegne_Conferimento = 378
        Numerazione_Documenti = 379
        Gestione_Imballaggi = 380
        Nuovo_Conferimento = 381
        Nuovo_DDT_Vendita = 382

        ManutenzioneArchivi_ImportazioneAnagraficheJDE = 383

        ManutenzioneArchivi_ImportazionePianoColturaleExcel = 384

        ManutenzioneArchivi_ImportazioneBizerba = 385

        Gestione_MenuControlliALP = 386
        Gestione_MenuControlliALP_Admin = 387

        Blocca_Sblocca_Pratiche = 388

        Importazione_AGREA_CSV = 389

        Importazione_AGREA_Grafico = 390

        Report_Analisi_PdC = 391

        Audit_BIO_COPROB = 392
        Audit_SQNPI_COPROB = 393

        Stime_Analisi_Produzione = 394

        Trasferimenti_Magazzino = 395

        '############################################################
        ' Controllo di gestione
        Gestione_Anagrafiche_CdG = 396
        Inserimento_CostiRicavi_Da_QdC_CdG = 397
        Inserimento_CostiRicavi_CdG = 398
        Gestione_Report_CdG = 399
        Gestione_Completa_CdG = 400
        '############################################################

        VivaiAttivitaAdempimenti = 401

        FiltraEdEsporta_PianiConcimazionePUA = 402

        NuovaVisitaDaFiltroDiRicerca = 403

        Pratiche_Filtri_Avanzati = 404

        Gest_PUA_2_ImportaFertilizzazioniDaRegistro = 405

        Gestione_Prezzi = 406

        Scadenzario_Inser = 407
        Scadenzario_Canc = 408
        Documentale_Lista = 409
        Documentale_Inser = 410
        Documentale_Canc = 411
        Documentale_Valid = 412
        Documentale_Storicizzazione = 413

        Angrafica_Prodotti = 414
        Valori_Economici_legati_alle_Attivita = 415

        Progetto_Piante = 416
        ReportPercorsi_Amministrtore = 417

        Prenotazione_Piante_OrdineDaRichiesta = 418
        Prenotazione_Piante_SoloOrdini = 419
        Prenotazione_Piante_RichiestaMaterialeVivaistico = 420

        Importazione_Anagrafiche_Zespri = 421

        Meteo_Modifica_creazioneStazioniProprietaVirtuali = 423

        Biologico_ReportBio = 424

        ManutenzioneArchivi_ImportazioneAnalisiCampioniMaselli = 425
        ManutenzioneArchivi_CodificaProdottiDaInterscambioApp = 426

        Prenotazione_Piante_nuovo_ordine_a_vivaio = 427
        Prenotazione_Piante_nuova_richiesta_materiale_vivaistico = 428

        PianiCampionamento_MarketAccess = 429

        Gestione_Contratti_Conferimento = 430

        Gis_Gestione_Di_SR_e_Trasfromazioni_Fra_SR = 431

        PianiCampionamento_RichiestaAnalisiMultiple = 432

        Nuovo_Conferimento_Pomodoro = 433

        PUA_Blocca_Sblocca = 434
        PianoConcimazione_Blocca_Sblocca = 435

        Contabilita_MVVElettronico = 436

        PianiCampionamento_MarketAccess_Globale = 437

        Contabilita_Clienti = 438
        Contabilita_Fornitori = 439

        Interferenze_MenuPrincipale_Accesso = 440   ' Ex TipiEnumerativiSementieri MenuPrincipale_Accesso = 1
        Interferenze_UtentiPermessi_Gestione = 441  ' Ex TipiEnumerativiSementieri UtentiPermessi_Gestione = 2
        Interferenze_Configurazione_Distanze = 442  ' Ex TipiEnumerativiSementieri Interferenze_Configurazione_Distanze = 3
        Interferenze_Configurazione_Colore = 443    ' Ex TipiEnumerativiSementieri Interferenze_Configurazione_Colore = 4
        Interferenze_Visualizzazione_Ridotta = 444  ' Ex TipiEnumerativiSementieri Interferenze_Visualizzazione_Ridotta = 5
        Interferenze_Visualizzazione_Estesa = 445   ' Ex TipiEnumerativiSementieri Interferenze_Visualizzazione_Estesa = 6

        Audit_Budwood_Projects = 446

        Menu_Agenda_Visualizzazione_Dettagli_Operazioni = 447

        Anagrafiche_Conferimento = 448

        Configurazione_Lavorazioni = 449

        Undo_Pratiche = 450

        Gias2JohnDeere = 451

        Statistiche_Acquisto = 452

        ManutenzioneArchivi_ImportazioneConferimentiPomodoro = 453

        Gestione_Carburanti_UMA = 454
        Richiesta_UMA = 455
        Rendicontazione_UMA = 456
        Approvazione_Richiesta_UMA = 457
        Approvazione_Rendicontazione_UMA = 458

        Import_Anagrafiche_COPROB = 459

        Configurazione_UMA = 460

        Ordini_Lavorazioni = 461

        Utility_Cambio_CF_Utente = 462


        UMA_Vendite_Carburanti = 463

        Esportazione_Trapianti_Agribologna = 464

        PianiCampionamento_RisultatiAnalisiInGrigliaAnalisi = 465

        Configurazione_Modelli_Previsionali = 466

        Riepilogo_UMA = 467

        ImportazionePcgAvepa = 468

        Piano_Nutrizionale = 469

        Visibilita_Aziende_UMA = 470

        Contabilita_DAA_Elettronico = 471

        Anagrafica_MultiModifica_PianoColturale = 472

        Contabilita_CarichiScarichi_Magazzino = 473

        Budget = 474
        Budget_Gestione_Anagrafiche_CdG = 475
        Budget_Inserimento_CostiRicavi = 476
        Budget_Gestione_Report = 477
        Budget_Valori_Economici_legati_alle_Attivita = 478
        Budget_Anagrafiche_Colturali = 479

        Audit_Convenzionale_Greenyard = 480
        Audit_Biologico_Greenyard = 481

        GIS_GestioneLayerPersonalizzati = 482


        Esportazione_Enogis = 483
        Esportazone_Artea = 484

        PianiCampionamento_CompilazioneCapitolatiCliente = 485

        DSS_Irrigazione = 486
        Gruppi_Merce = 487

        Contratti_Affitto = 488

        EstrazioneCatastoAffitti = 489

        Esportazone_xFarm = 490

        ReteAcqua_Permessi = 491
        ReteAcqua_Parametri = 492
        ReteAcqua_Storico = 493
        ReteAcqua_AnalisiDati = 494

        Blocco_Particelle_UMA = 495

        Cartografia_VisualizzazioneTotale = 496
        Cartografia_SetupVisualizzazione = 497

        Importazione_ParmaFrance = 498

        Interferenze_ScaricoDati = 499
        Interferenze_ScaricoDati_Consolida = 500
        Interferenze_ScaricoDati_Report_Completo = 501

        Agenda_AccessoMenu_NG = 502
        Gest_CartografiaAziendale_NG = 503
        Gest_AnagraficaAzienda_NG = 504

        ZooNogmo = 505

        Anagrafica_Appezzamento_CopiaSposta = 506

        Caricamento_Utilizzo_Mappe_Prescrizione_Personalizzate = 507

        AggiornamentoMatricoleMadri = 508

        SmartTractors = 509
        SmartTractors_Parametrizzazione = 510
        SmartTractors_InvioRicette = 511

        GIS_Configurazione_Algoritmi_Cartografici = 512
        GIS_Gestione_Parametri_Maschere_Raster = 513
        Profilazione_NG = 514

        Visite_Lista_NG = 515

        Gestione_GHG = 516
        Configurazione_Operazioni_Colturali = 517


        '--- domanda irrigua e connessi - inizio ---'
        DomandaIrrigua = 518
        DomandaIrrigua_Scheda = 519
        DomandaIrrigua_Ricerca = 520

        LettureContatoriAziendali = 521
        ElaboraCalcoloTariffazione = 522
        '--- domanda irrigua e connessi - inizio ---'

        Valutazioni_Rischio = 523 'TODO: Creare voce su Migra

        Gruppi_Raccolta_NG = 524

        AttivitaInterne_Inserimento_Costi = 525

        RequisitiStabilimentoNg = 526
        AnalisiProduttivita = 527

        ImportPianoColturaleDaKOBO = 528
        ImportPianoColturaleDaShapeFile = 529

        ReportCampagna = 530

        Consultazione_TimeSheet_Personale_CdG = 531

        Gestione_Squadre_CdG = 532

        GisBulkExportSuLayer = 533

        Budget_Ribaltamento_Su_Reale = 534

        Dati_Previsionali_Colture = 535
        BDN_GestioneConsorzio = 537

        PianiCampionamento_Zootecnia = 538

        Analisi_Correzione_Parametri = 540
        Visualizzazione_SincroStalla = 541

        Accetta_Ordine_Esolver = 542

        Planning_RiportaOrdineStatoInserito = 543

        Esportazone_Horta = 544

        Confronto_Piani_Colturali = 545

        WidgetMultiAziendali = 546

        Menu_Precedente = 547

        UMA_Report_Controllo = 548

        Visibilita_Viste_Grid = 549

        Visibilita_Aziende_UMA_GestioneVisibilitaCompleta = 550

        Menu_Stampe_New = 551

        Filiera_Trasporti_COPROB = 552

        FiltroRicerca_NG = 553

        Esportazione_Agea_NG = 554

        Statistometro_New = 555

        Gestione_Servizi_NEW = 556

        RegistrazioneMassiva_BDN = 557

        UMA_Ricerca_Macrousi_Lavorazioni = 558

        SincronizzazioneMassivaStalleVetInfo = 559

        Visualizza_Comandi_Avanzati_Esportazione_Agea_NG = 560

        ProfilazioneImpresa_NG = 561

        ImpostazioniImprese_NG = 562

        Contabilita_BilancioDiMassa = 563

        ''' <summary>
        ''' Permette la modifica di tutti gli utenti (non solo sé stessi).
        ''' </summary>
        AmministrazioneUtenti = 564

        Enquete_certification_Hevea_brasiliensis = 565
        Banca_Cambiano_Azienda = 566
        ImportazioneMassivaModelli4 = 567

        CalcoloMassivo_Compliance_ISCC_AziendeInVisibilita = 568
        InserisciDocNonConformeISCC = 569
        ReportImpiegoProdottiFitosanitari = 570

        LettureContatoriAziendali_NG = 571
        GestioneAssociazioneAppezzamentiXParcoMacchine = 572
        GestioneCache = 573
        MenuZooNG = 575
        WidgetPrevisioniCostiRicaviAI = 576
        MenuRilievi = 577

        GestioneAppezzamentiTessitura = 578
        GestioneAppezzamentiPendenza = 579
        ModificaOperazioneinVerifica = 580
        GestioneContributiACA = 581

        TrattamentoZoo = 582
        ZooProtocolliTerapeutici = 583
        ZooIndicazioniTerapeutiche = 584

        Audit_BIO_FILENI = 585
        Audit_Fornitori_EUDR = 586
        ManutenzioneArchivi_GestioneSistemi_Esterni = 587
        Report_Abilitazione_PdC = 588

        Invia_Stat_Matomo = 589

        PianiCampionamento_PubblicazioneAnalisi = 590

        AnagraficaGestioneRateiIrriguiMacchine = 591

        Importazione_Farmacie = 592

        Controllo_DPI_DeMatteis = 593

		''' <summary>
		''' Gestisce la possibilità di generare automaticamente il codice fiscale per gli utenti.
		''' </summary>
		UtentiGenerazioneCF = 594

        Report_Zootecnia = 595
        VerificaConformitaNG = 596
        GIS_Configurazione_Algoritmi_Clustering = 597
        ConfigurazioneModalitaPagamento = 598

        WidgetReportChecklist = 599

        InvioTrattamentiMassivoGiasVetInfo = 600

        ZooTerapie = 601

        GestioneCarro = 602
        GestioneCarroConfig = 603
        StatistometroReportDettagliServiziAzienda = 604

        GestioneEserciziVincoli = 609
        ZooPesatureAccrescimento = 610
        DSS_Nutrizione = 611

        'andare avanti da qui.....

        '############################################################
        ' GIASAPP

        GiasAPP_Permessi = 800
        GiasAPP_NUOVA_RICETTA = 801
        GiasAPP_NUOVO_INTERVENTO = 802
        GiasAPP_INTERVENTI_DA_FARE = 803
        GiasAPP_SCARICO_ORE = 804
        GiasAPP_ENTRATAUSCITA = 805
        GiasAPP_LAMIAPOSIZIONE = 806
        GiasAPP_VISITE = 807
        GiasAPP_DOCUMENTI = 808
        GiasAPP_RILIEVI = 809
        GiasAPP_GIS = 810
        GiasAPP_InCab = 811
        GiasAPP_PianoColturale = 812
        GiasAPP_Magazzini = 813
        GiasAPP_Macchine = 814
        GiasAPP_Manutenzioni = 815
        GiasAPP_DDT_Movimenti = 816
        GiasAPP_Gias = 817
        GiasAPP_Aziende = 818
        GiasAPP_Centri = 819
        GiasAPP_Isolamenti = 820
        GiasAPP_DSS_Difesa = 821
        GiasAPP_ConsultaSincroDatiAppDaWeb = 822 'Utilizzato solo lato web
        GiasAPP_Consiglio_Irriguo = 823
        GiasAPP_Consiglio_Fertirriguo = 824
        GiasAPP_Precision_Farming = 825
        GiasAPP_Monitoraggio_Meteo = 826

        ' nuovi permessi APP (Demetra)
        GiasAPP_Lavoratori = 827
        GiasAPP_Widget_Rischi_Meteo = 828
        GiasAPP_Widget_Rischi_Difesa = 829
        GiasAPP_Widget_Consiglio_Semina = 830
        GiasAPP_Widget_Consiglio_Nutrizione = 831
        GiasAPP_Widget_Consiglio_Irriguo = 832
        GiasAPP_Widget_Consiglio_Raccolta = 833
        GiasAPP_Chatbot = 834
        GiasAPP_Allarmi_Widget = 835
        GiasAPP_Dati_Meteo_Storici = 836
        GiasAPP_Indici_Satellitari = 837
        GiasAPP_Geofoto = 838
        GiasAPP_Consultazione_Dati_Sensori = 839
        GiasAPP_Squadre_Lavoratori = 840
        GiasAPP_Creazione_Fornitore = 841
        GiasAPP_Gestione_Tracce = 842
        GiasAPP_Gestione_Fasi_Fenologiche = 843
        GiasAPP_Gestione_Trappole = 844
        GiasAPP_Notizie_Coldiretti = 845
        GiasAPP_Messaggi = 846
        GiasAPP_Promemoria = 847
        GiasAPP_Registrazione_Rilievi_Pedologici = 848
        GiasAPP_Modulo_BeLeaf = 849


        '############################################################
        ' PROFITOSAN

        Profitosan_Home = 1000

        Profitosan_Prodotto_Testata = 1001
        Profitosan_Prodotto_Dettagli = 1002
        Profitosan_Prodotto_Etichetta = 1003
        Profitosan_Prodotto_SchedaSicurezza = 1004

        Profitosan_RicercaProdotti = 1005           'codice/nome prodotto
        Profitosan_RicercaProdottiAvanzata = 1006   'campi impiego, sostanze attive, ditte, RMA
        Profitosan_RicercaProdottiSimili = 1007

        Profitosan_Disciplinari = 1008

        Profitosan_RMA = 1009

        Profitosan_Prodotto_Decreto = 1010

        Profitosan_Tunnel_Prodotto = 1011

        Profitosan_Tunnel_HomeLogin = 1012

        SmartTractor_FullAccess = 1013

        '############################################################
        ' PianoConcimazione

        PianoConcimazione_CalcoloBilancio = 2000

        '############################################################

    End Enum

    Public Enum enum_Impostazioni_Utenti

        '=====================================================================================
        'DOCUMENTAZIONE: \\Rubino2\documentazione\GIAS --- Utenti\Utenti_Impostazioni.xlsx
        '=====================================================================================

        '-------------------------------------------------------------------------------------
        'Impostazioni GENERICHE
        '-------------------------------------------------------------------------------------

        'La visibilità di queste impostazioni non viene indicata rigidamente nel nome,
        'ma documentata tramite summary.

        ''' <summary>
        ''' Visibilità: CENTRO, IMPRESA, SUPERUSER
        ''' </summary>
        ScriviAppezza_RipartoCatastoDaEntita = 1079

        ''' <summary>
        ''' Visibilità: IMPRESA
        ''' </summary>
        DocContabili_GestioneWorkFlow = 1063

        ''' <summary>
        ''' Visibilità: IMPRESA
        ''' </summary>
        Default_GruppoMerce_CategoriaProdotto = 1064

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Raccolta_Con_Carico_Magazzino = 1065

        ''' <summary>
        ''' Visibilità: SUPERUSER
        ''' </summary>
        Degrado_Visibilita_Obbligatorieta = 1066

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Conferimento_Da_Raccolta = 1068

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        StampaDocAcquisto_CodiceSDI = 1069

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Indirizzi_Obbligatorieta_Valorizzazione_Gerarchia_Geografica = 1070

        ''' <summary>
        ''' Visibilità: Utente
        ''' </summary>
        StampaCampagna_Default_Vedi_AvversitaQta = 1071

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Mostra_DDT_MenuAgenda = 1072

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        GruppoMerce_Controllo = 1073

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        CdC_Wbs_Controllo = 1074

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER Specifica per Iniziative Biometano
        ''' </summary>
        IB_RisorseUmane_SettoreDes_Controllo = 1075

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Collega_Solo_Ordini_Inviati = 1076

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER Specifica per Iniziative Biometano
        ''' </summary>
        IB_Funzione_Controlli_PreInvio = 1077

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        MenuAgenda_Visibilita_Doc_Contabili = 1078

        ''' <summary>
        ''' Visibilità: SUPERUSER
        ''' </summary>
        Workflow_GruppiUtente_PermessiStato = 1080

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Imputazione_Impianti_Raccolta_Conf = 1085

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Scarico_Da_Raccolta = 1086

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        Aggiorna_Peso_Raccolta_Da_Conf = 1087

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        StampaArticolo62ElemCod = 1092

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        ImpedisciCreazioneCarichiMultiriga = 1094

        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        DSS_Difesa_Riepilogo_Copertura_Trattamento = 1103

        '-------------------------------------------------------------------------------------
        'Impostazioni SUPERUSER
        '-------------------------------------------------------------------------------------

        SUPERUSER_COD_DISTRIBUZIONE_ACQUA = 1
        SUPERUSER_COD_IMPIANTO_DISTINTA = 2
        SUPERUSER_COD_BLOCCO_PARTICELLE = 10
        SUPERUSER_COD_ANALISI_COSTI = 12

        SUPERUSER_Blocca_Impianti_Smart = 73
        SUPERUSER_Smart_NuovoImpianto_OrganismoReferente = 74
        SUPERUSER_Smart_NuovoImpianto_DestinazioneUso = 75
        SUPERUSER_Smart_NuovaImpresa_ImpresaPadre = 76
        SUPERUSER_Smart_NuovoImpianto_DefaultOrganismoReferente = 77

        SUPERUSER_COD_FILE_IMPORTAZIONE_DATI = 500
        SUPERUSER_COD_FILE_ESPORTAZIONE_DATI = 501
        SUPERUSER_COD_FILE_ESPORTAZIONE_DATI_2 = 502
        SUPERUSER_COD_IP_BILANCIA = 600
        SUPERUSER_COD_TARA_BILANCIA = 601
        SUPERUSER_COD_FILTRO_MATERIE_PRIME = 666
        SUPERUSER_COD_COOP_REFERENTE_OBBLIGATORIA = 670
        SUPERUSER_COD_ORG_REFERENTE_OBBLIGATORIO = 670
        SUPERUSER_COD_ORG_REFERENTE_DA_PADRE = 671
        SUPERUSER_COD_IMPOSTAZIONE_IMBALLAGGI = 700
        SUPERUSER_COD_IMPOSTAZIONE_CONTENITORI = 701
        SUPERUSER_COD_CONTABILITA_MULTIPLA = 710
        SUPERUSER_COD_IVA_DEFAULT = 712
        SUPERUSER_COD_CAUSALE_TRASPORTO_DEFAULT = 713
        SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA = 714
        SUPERUSER_COD_NOTE_DEFAULT_FATTURA_EMESSA = 715
        SUPERUSER_COD_LISTINI_PRODUZIONE = 720
        UTENTE_RapportoContabileDefault = 722
        SUPERUSER_COD_MODALITA_TRASPORTO_DEFAULT = 723
        SUPERUSER_COD_Aspetto_Beni_Default = 724
        SuperUser_LayOut_Peso_DDT = 727
        SuperUser_LayOut_Prezzo_DDT = 728
        SuperUser_LayOut_Riscontrato_DDT = 729
        SUPERUSER_COD_GESTIONE_AGENTI = 734
        SUPERUSER_COD_GESTIONE_SEZIONALI = 746
        SUPERUSER_COD_GEST_AUTO_CONSISTENZE_ENOLOGICHE = 750
        SUPERUSER_COD_ZONA_VITICOLA = 751
        SUPERUSER_COD_GESTIONE_REG_VINIFICAZIONE = 756
        SuperUser_StampaCapacitaEffettivaVascaRegCantina = 757
        SuperUser_StampaIndentificativoVascaRegCantina = 758

        SUPERUSER_COD_VISUAL_CODARTICOLO_DOCUMENTI = 764

        SuperUser_ContributoConai = 766
        SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI = 767
        SUPERUSER_COD_PERMETTI_MODIFICA_RICETTE_CON_OPERAZIONI_REGISTRATE = 768
        SuperUser_NoteIntegrative1DDT = 770
        SuperUser_NoteIntegrative2DDT = 771
        SuperUser_NoteIntegrative3DDT = 772
        SuperUser_Articolo62DDT = 773
        SuperUser_NoteIntegrative1DDTCorrispettivi = 774
        SuperUser_NoteIntegrative2DDTCorrispettivi = 775
        SuperUser_NoteIntegrative3DDTCorrispettivi = 776
        SuperUser_Articolo62DDTCorrispettivi = 777
        SuperUser_NoteIntegrative1Ordini = 778
        SuperUser_NoteIntegrative2Ordini = 779
        SuperUser_NoteIntegrative3Ordini = 780
        SuperUser_Articolo62Ordini = 781
        SuperUser_NoteIntegrative1Fatture = 782
        SuperUser_NoteIntegrative2Fatture = 783
        SuperUser_NoteIntegrative3Fatture = 784
        SuperUser_Articolo62Fatture = 785
        SuperUser_GestioneVisualNumVascaRegImbott = 791
        SUPERUSER_COD_TIPO_DOC_ACCETTAZIONE_DEFAULT = 795
        SUPERUSER_COD_CHKCOGE_MANUALE_DEFAULT = 797
        SuperUser_StampaLottoTrasformazioneRegCantina = 801
        SUPERUSER_COD_GESTIONE_CAPOAREA = 807
        SuperUser_ApriFiltroFattura = 810
        SuperUser_ApriFiltroDDT = 811
        SuperUser_StPersDDTAccetta = 812
        SuperUser_StRifOrdine = 813
        SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI = 814
        SUPERUSER_COD_SQPNI_FILTROSPECIE = 815
        SUPERUSER_COD_SQPNI_FILTROMACRO_USI = 816
        SUPERUSER_COD_SQPNI_FILTRODESTINAZIONE_USO = 817

        SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE = 819
        SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA = 820
        SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI = 821
        SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA = 822
        SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA = 832
        SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA = 833

        SUPERUSER_COD_TOLLERANZA_PESO_GIACENZE_DISTINTA = 825
        SUPERUSER_COD_NUOVA_MODALITA_ARROTONDAMENTO_LAN = 826

        SUPERUSER_GiasAPP_NUOVA_RICETTA = 827
        SUPERUSER_GiasAPP_NUOVO_INTERVENTO = 828
        SUPERUSER_GiasAPP_INTERVENTI_DA_FARE = 829
        SUPERUSER_GiasAPP_SCARICO_ORE = 830
        SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = 831

        SUPERUSER_COD_RICETTE_CREA_UNA_OPERAZIONE_PER_OGNI_IMPIANTO = 834
        SUPERUSER_GiasAPP_MAX_AZIENDE = 835
        SUPERUSER_ArrotondamentoIVA4Dec_DataAttivazione = 836
        SUPERUSER_Consenti_ContattoCod_Duplicato = 838
        SUPERUSER_PRODOTTOCOD_MAX_LENGTH = 839
        SUPERUSER_ContattoCod_Max_Lenght = 840
        SUPERUSER_StampaAlcolTotale = 841
        SUPERUSER_ConsentiNumDocDuplicati = 842
        SUPERUSER_JoinCacPivaSuperUser = 843

        SUPERUSER_SoloLottiDisponibiliInOrdineVendita = 844
        SUPERUSER_PrezzoNulloInListino = 845
        SUPERUSER_TipoValorizzazioneCostiCdG = 846
        SUPERUSER_Applica_Listini_Non_Associati = 847

        SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO = 848
        SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB = 849

        SUPERUSER_GiasAPP_ENTRATAUSCITA = 850
        SUPERUSER_GiasAPP_LAMIAPOSIZIONE = 851
        SUPERUSER_GiasAPP_VISITE = 852
        SUPERUSER_GiasAPP_FREQUENZARILIEVO_MINUTI = 853
        SUPERUSER_GiasAPP_FREQUENZASINCRO_MINUTI = 854
        SUPERUSER_Licenza_Giorni_Franchigia = 855

        SUPERUSER_INTESTAZIONE_STAMPA_CENTROAZIENDALEPARTENZA = 877

        SUPERUSER_GESTIONE_SPOSTAMENTO_ANIMALI = 878

        SUPERUSER_Gestione_GHG = 872



        SUPERUSER_EDIT_LOTTO = 990
        SUPERUSER_OPERATORE_ACCETTAZIONE = 998
        SUPERUSER_Consenti_ProdottoCod_Duplicato = 999

        SuperUser_StampaLitriDDTFatture = 1050
        SuperUser_CessionariAggiuntivi = 1051
        SuperUser_RifDoc_EnteConsorzio = 1052
        SuperUser_PesiColli_Riscontrati = 1053
        SuperUser_TipoDestinazione_Default = 1054

        SUPERUSER_LIVELLO_GESTIONE_CONTABILITA = 1058
        SUPERUSER_ACCETTAZIONE_CON_GERARCHIA = 1059
        SUPERUSER_DocContabili_SceltaImputazione = 1061
        IMPRESA_DocContabili_Cod_Tipo_Imputazione = 1062

        SUPERUSER_Creazione_Prodotti = 1081


        SUPERUSER_NR_ORE_VISITA = 1084

        SUPERUSER_ZOO_IN_VISITA = 1088

        SUPERUSER_STATO_VISITA = 1089

        GIS_PARAMETRI_SETUP = 1090

        SUPERUSER_CARICO_ZOO_GRIGLIA = 1091
        ''' <summary>
        ''' Visibilità: IMPRESA e SUPERUSER
        ''' </summary>
        SUPERUSER_ELABORAZIONE_DSS_DA_IMPIANTI = 1093
        SUPERUSER_ELABORAZIONE_DSS_FORECAST = 1095

        SUPERUSER_CHECKLIST_TRASPORTI = 1097
        DSS_IRRIGAZIONE_RAGGRUPPAMENTO_X_CAMPI = 1098
        RICERCA_CON_FILTRO_CESSIONARIO_DOC_CONT = 1099

        SUPERUSER_COD_IMPORTAZIONE_AGREA = 3001
        SUPERUSER_COD_IMPORTAZIONE_ANAGRAFEEMILIAROMAGNA = 3002
        SUPERUSER_COD_IMPORTAZIONE_AVEPA = 3003

        SUPERUSER_CALO_PESO_DEFAULT_CONTATTO = 1101
        SUPERUSER_COD_REGISTRA_ALIMENTAZIONE_DATA_SINGOLA_ZOO = 1102
        SUPERUSER_ModalitaVerificaConformita = 1106

        '-------------------------------------------------------------------------------------
        'Impostazioni UTENTE
        '-------------------------------------------------------------------------------------

        UTENTE_COD_FILTRO_GRUPPI_VEGETALI = 3
        UTENTE_COD_FILTRO_VARIETA = 4
        UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_AVVERSITA = 5   'valore: 88=gruppi 77=singole
        UTENTE_COD_FILTRO_SPECIE_VEGETALI = 6
        UTENTE_COD_FILTRO_LAVORAZIONI = 7    'aka. filtro operazioni
        UTENTE_COD_FILTRO_GRUPPI_OPERAZIONI = 8
        UTENTE_COD_MAGAZZINO_RIFERIMENTO = 9
        UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_INFESTANTI = 11   'valore: 88=gruppi 77=singole
        UTENTE_COD_DEFAULT_DPI = 13   'valore: 0=nessun DPI 1=con DPI
        UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = 14   'valore: 0=nessun blocco 1=blocco salvataggio
        UTENTE_COD_DEFAULT_QTA_PRODOTTO = 15   'valore: 88=totale 77=ettaro
        UTENTE_COD_DEFAULT_QTA_ACQUA = 16        'valore: 88=totale 77=ettaro
        UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO = 18     '0=nessun magazzino  1=con magazzino
        UTENTE_COD_DEFAULT_FILTRO_PRODOTTI = 19     '0 = nessuno - 1 = coltura - 2 = coltura/ avversità/ infestante

        UTENTE_COD_DEFAULT_FILTRO_PRODOTTI_RICETTE = 176     '0 = nessuno - 1 = coltura - 2 = coltura/ avversità/ infestante

        UTENTE_COD_ColonneVisibili_TabellaImpianti_Semina = 24
        UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda = 26
        UTENTE_COD_ColonneVisibili_PAN_PianoColturale = 57
        UTENTE_COD_FILTRO_STAMPA_RICETTA_NUMERO_RICETTA = 27
        UTENTE_COD_FILTRO_STAMPA_RICETTA_MACCHINE = 28
        UTENTE_COD_FILTRO_STAMPA_RICETTA_OPERATORI = 29
        UTENTE_COD_FILTRO_STAMPA_RICETTA_TECNICO_AUTORIZZANTE = 30
        UTENTE_COD_FILTRO_STAMPA_RICETTA_FIRMA_AGRICOLTORE = 31
        UTENTE_COD_FILTRO_STAMPA_RICETTA_DATA_ULTIMA_MANUTENZIONE = 44

        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_ARROTONDA_ACQUA = 32
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_MACCHINE = 33
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_OPERATORI = 34
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_TECNICO_AUTORIZZANTE = 35
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DATA_FIRMA = 36
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_NUMERO_RICETTA = 42
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DATA_ULTIMA_MANUTENZIONE = 45
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FASE_EPOCA_ETICHETTA = 117
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FILTRA_FASCICOLO = 112
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_VISUALCAMPO_SOLOFRONTESPIZIO = 177

        UTENTE_COD_SCHEDA_CAMPAGNA_NUM_APPEZZAMENTO = 109

        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_ARROTONDA_ACQUA = 37
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_MACCHINE = 38
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_OPERATORI = 39
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_TECNICO_AUTORIZZANTE = 40
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DATA_FIRMA = 41
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_NUMERO_RICETTA = 43
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DATA_ULTIMA_MANUTENZIONE = 46
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_FASE_EPOCA_ETICHETTA = 118

        'blocchi di etichetta inserimento
        UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA = 47
        UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA = 48
        UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA = 49
        UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA = 50
        UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA = 51
        UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA = 52
        UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA = 53
        UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA = 169

        'Livello di Controllo del DPI --> 1 livello base 2 livello avanzato
        UTENTE_COD_LIVELLO_CHK_DPI = 54 'Registrazione Rilievo SOGLIA
        UTENTE_COD_TUTTI_I_CENTRI = 55
        UTENTE_COD_CHILI_LITRI = 56

        SUPERUSER_DEFAULT_NOTE_CAMPIONI = 57
        UTENTE_MODALITA_NERO = 58

        UTENTE_GESTIONE_MAGAZZINO_2 = 59    '0=nessuna gestione  1= carica magazzini super user 2= carica magazzini impresa padre

        UTENTE_OPERAZIONI_QDC_PREFERITE = 60 'Lav_cod separati da |

        UTENTE_COD_ColonneVisibili_Planning = 61
        UTENTE_COD_ColParticelleVisibili_Planning = 91

        SUPERUSER_Codice_Campione_Progressivo_Inizio_Anno = 62

        SUPERUSER_Label_Codice_Campione = 63

        UTENTE_NumAppezza_Progr_Modalita = 64
        UTENTE_MultiModificaImpianti_FiltroProprieta = 65

        UTENTE_COD_ColonneVisibili_PDC_da_campagna = 66
        UTENTE_COD_ColonneVisibili_PDC_acquisti = 67

        UTENTE_COD_DEFAULT_DPI_PREDEFINITO = 68   'valore: es 34/1 emilia romagna 2012

        UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni = 69 '0= non salva il riferimento in ricettaxagenda, 1 o non impostato= salva il riferimento

        UTENTE_StampaPDF_RicFiscali_ConSenzaPreview = 70 '0= con preview,1=senza preview

        UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA = 71 'Tipo gruppo separati da | (E,P,C,Z,E6,E10) tipo E separato in E6, E!= per magazzino e contabili))

        UTENTE_Nome_Stampante_Ricevuta_A5 = 72 'Tipo gruppo separati da | (E,P,C,Z,E6,E10) tipo E separato in E6, E!= per magazzino e contabili))

        UTENTE_COD_FILTRO_STAMPA_RICETTA_ARROTONDA_ACQUA = 78

        UTENTE_AlberoAnagrafica_visualizzaRiferimentoAlfanumericoImpianto = 79

        UTENTE_AlberoAnagrafica_ordinaDataUltimoImpianto = 80

        UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO = 81
        UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = 82
        UTENTE_COD_BLOCCA_SE_SENZA_DISCIPLINARE = 168
        UTENTE_COD_BLOCCA_SE_DATA_FATTURA_SUPERA_ALLEGATI = 83

        SUPERUSER_Label_Codice_Analisi = 84
        SUPERUSER_Label_Codice_Griglia = 85

        UTENTE_COD_ColonneVisibili_PDC_AltreAnalisi = 86

        SUPERUSER_Stampa_Rapida_WorkFlow = 87

        UTENTE_LINK_PREFERITE = 88
        UTENTE_LINK_PREFERITE_MENUBS2017 = 171

        UTENTE_COD_AZIENDA_PREDEFINITA_ALL_AVVIO = 182

        SUPERUSER_Codice_Campione_Zani = 92

        UTENTE_LINK_PREFERITE_MENU_ONLINE = 119

        'CATEGORIE MAGAZZINO
        UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT = 711
        UTENTE_COD_CATEGORIAMAGAZZINO_COSTI_DEFAULT = 721
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO = 89   'Elem_cod separati da |
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT = 90   'Elem_cod_Udm_Cod separati da |
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE = 180   'Elem_cod_Valore separati da |
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI = 181   'Elem_cod_Valore separati da |
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP = 183   'Per APP Elem_cod_Valore separati da |

        UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS = 93  ' Blocca se vengono superati i limiti di azoto, fosforo, potassio e magnesio dell'impianto

        UTENTE_COD_DEFAULT_ProdottiTossiciPatentinoMovimenti = 94 '0=permetti 1=avvisa 2=blocca
        UTENTE_COD_BLOCCO_TARATURA_ATOMIZZATORE_SCADUTA = 212 '0=permetti 1=avvisa 2=blocca
        UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI = 213 '0=no 1=sì
        UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO = 214 '0=no 1=sì

        UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA = 95

        UTENTE_Menu_Agenda_Selezione_Tipo_Operazioni = 96
        UTENTE_Menu_Agenda_Selezione_GruppoOperazioni = 97

        UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE = 211

        SUPERUSER_Codice_Campione_TERREMERSE = 98

        UTENTE_COD_ColonneVisibili_Analisi_PDC = 99

        ' Nitrati Piano di concimazione
        UTENTE_Nitrati_PC_Analisi = 100     'Analisi_Entita_Cod separati da |

        UTENTE_STAMPE_PREFERITE = 101 'cod_report, enum_CodificaStampe, tabella StampeReport  separati da |

        UTENTE_Nitrati_RegolamentoPC_Default = 102

        UTENTE_FILTRONE_PDC = 103

        SUPERUSER_ComportamentoComboFF = 104

        UTENTE_InizioFineAnnataAgraria = 105

        UTENTE_Planning_Date = 106
        UTENTE_Planning_NValidazioneNome = 107
        UTENTE_Ribaltamento_CreaCampi = 108

        UTENTE_COD_UTILIZZA_SUP_APP_AGENDA = 151

        UTENTE_COD_RACCOLTA_TIPO = 110
        UTENTE_COD_RACCOLTA_TIPOLOGIA_PRODOTTO = 111

        UTENTE_TABACCO_EXPORT_ALL = 113
        UTENTE_TABACCO_EXPORT_APPEZZA = 114
        UTENTE_TABACCO_EXPORT_CORONA = 115
        UTENTE_TABACCO_EXPORT_GRADO = 116
        UTENTE_TABACCO_EXPORT_BUCHI = 121

        UTENTE_Attiva_Configurazione_Pratica = 172

        UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA = 120

        UTENTE_COD_BLOCCA_SEMINA_SE_SENZA_QTA = 122

        'blocchi  etichetta/dpi salvataggio
        UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NONCONFORME = 125

        UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO = 126
        UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO = 127
        UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO = 128
        UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO = 129
        UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = 130
        UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO = 131
        UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO = 132
        UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = 133

        UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO = 134
        UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO = 135

        UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO = 136
        UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO = 137
        UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO = 138
        UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO = 139
        UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO = 140

        UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO = 141
        UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = 142

        UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = 143
        UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO = 174

        UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = 144
        UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = 145

        UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = 146

        UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO = 170

        UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE = 163
        UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE = 164
        UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE = 165

        UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO = 173

        UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = 206
        UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO = 209
        UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO = 210
        SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA = 208

        ' Impostazioni Campagna e GLobal
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_CODICE_PRODUTTORE = 147
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_TECNICO_RIFERIMENTO = 148
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_CODICE_PRODUTTORE = 149
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_TECNICO_RIFERIMENTO = 150
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_DOSE_ETICHETTA = 161
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_DOSE_ETICHETTA = 162
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_GLOBAL_NASCONDI_CAMPO = 166
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_NASCONDI_CAMPO = 167

        ''impostazioni BIO
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_MACCHINE = 152
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_OPERATORI = 153
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_TECNICO_AUTORIZZANTE = 154
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_FIRMA = 155
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_ULTIMA_MANUTENZIONE = 156
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_CAMPO = 157
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO = 158
        UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO_BIO = 159
        UTENTE_COD_SCHEDA_CAMPAGNA_BIO_NUM_APPEZZAMENTO = 160

        UTENTE_COD_FILTRO_STAMPA_PIANOCONCIMAZIONE_CAMPAGNA_DATA_FIRMA = 205

        UTENTE_COD_SEMINA_TIPO = 175

        UTENTE_WS_SCARICO_FASCICOLO = 178 'enum_WS_Esterni separati da |
        UTENTE_PRATICHE_DA_ATTIVARE_SCARICO_FASCICOLO = 179 'servizi separati da |

        UTENTE_WS_GESIONE_AZIENDE = 189

        UTENTE_COD_FORMULATI_STATO = 184

        Utente_SchedaColtBio_VisualizzaLottoSemine = 185
        Utente_SchedaColtBio_VisualizzaLottoRaccolte = 186

        SuperUser_FiltroSQL_MateriePrime = 187

        SuperUser_Gestione_Esercizi = 188
        SuperUser_Albero_Anagrafiche = 190

        UTENTE_GESTORE_AZIENDE = 191

        SuperUser_Ereditatore = 192

        SuperUser_KPIN_BlockName = 193

        SuperUser_Visualizza_Codici_Anagrafici = 194

        Codice_Univoco_Appezzamento = 195
        Codice_Univoco_Impianto = 196
        Codice_Univoco_Progetto = 197

        SuperUser_Impostazioni_PDC = 198
        SuperUser_Gestione_PDC_Analisi = 199

        Utente_Lingua_Stampa = 200

        UTENTE_COD_FINALITA = 201
        UTENTE_COD_REGOLAMENTO = 202
        UTENTE_NUOVA_PARTICELLA = 203
        UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO = 204

        UTENTE_OPERAZIONI_ZOO_PREFERITE = 207

        Visualizza_UdmAggiuntiva_xSup = 215
        SUPERUSER_NuovoImpianto_DefaultTecnico = 216

        UTENTE_InterpretazioneInizioFineAnnataAgraria = 217 'in riferimento a UTENTE_InizioFineAnnataAgraria (105)

        Utente_OpzioneChiusuraAbbattimenti = 218

        '=================================================

        UTENTE_DAA = 753
        Utente_ChkAccettazioneDaDiversi = 761

        UTENTE_COD_TIPO_ALLERTA_PREZZO_0_VENDITA = 798
        UTENTE_COD_TIPO_FATTURA_DEFAULT = 799

        UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO = 818
        UTENTE_COD_PREFERITI_MENU = 823
        UTENTE_COD_MODALITA_STAMPA = 824
        UTENTE_COD_PREFERITI_REPORT_VENDITE = 837

        '''costanti per il proxy
        UTENTE_COD_PROXY = 1000
        UTENTE_COD_USERNAME_PROXY = 1001
        UTENTE_COD_PASSWORD_PROXY = 1002
        UTENTE_COD_HOST_PROXY = 1003

        UTENTE_UDM_Area_COD = 1010
        UTENTE_CATASTO_PercentoSogliaDifferenzeEvidenziate = 1011

        UTENTE_GIS = 1012

        'Impostazioni CDG
        UTENTE_CONTROLLO_SCARICO_CDG = 1055
        UTENTE_MODALITA_SCARICO_PRODOTTI_CDG = 1055

        UTENTE_STATI_PANEL_BAR = 1056
        UTENTE_FILTRI_RICERCA_DOC_CONTABILI = 1057
        UTENTE_FILTRI_RICERCA_DOCUMENTALE = 1067

        UTENTE_FILTRI_RICERCA_BILANCIO_DI_MASSA = 1100

        UTENTE_FILTRI_RICERCA_FILTRONE_VISITE = 1060

        UTENTE_Operazione_Predefinita_Da_Impianto = 856

        SUPERUSER_Livello_Applicazione_Listini = 857

        UTENTE_COD_PREFERITI_REPORT_ACQUISTI = 859

        SUPERUSER_GiasAPP_IMPOSTAZIONI = 860
        SUPERUSER_GiasAPP_DOCUMENTI = 861
        SUPERUSER_GiasAPP_RILIEVI = 862
        SUPERUSER_GiasAPP_GIS = 863
        SUPERUSER_GiasAPP_InCab = 864

        SUPERUSER_Conferimento_Ripartizione_Impianti = 865
        SUPERUSER_Conferimento_Ripartizione_Obbligatoria = 866

        SUPERUSER_GiasAPP_Permessi = 867

        SUPERUSER_Mod_Ricerca_Impresa = 868

        UTENTE_COD_PREFERITI_REPORT_ANALISI_PROGETTI = 870
        UTENTE_COD_PREFERITI_VISTE_INVESTIMENTO_CATASTO = 871

        UTENTE_GIASAPP_LAVORAZIONI_IN_CAMPO = 873
        UTENTE_GIASAPP_GESTIONE_POSIZIONE = 874

        UTENTE_COD_PREFERITI_REPORT_SOSTENIBILITA = 875
        UTENTE_COD_PREFERITI_REPORT_STATISTICHE_QDC = 876
        UTENTE_COD_PREFERITI_REPORT_RIEPILOGO_ANALISI_PDC_ZOO = 879
        UTENTE_COD_PREFERITI_REPORT_CONSISTENZE_ZOO = 880

        UTENTE_COD_PLANNING_INVIA_A_ESOLVER = 881

        UTENTE_COD_PREFERITI_REPORT_TRATTAMENTI_ZOO = 882
        UTENTE_COD_PREFERITI_REPORT_SENZA_TRATTAMENTI_ZOO = 883
        UTENTE_COD_PREFERITI_REPORT_STAZIONAMENTO_ZOO = 884
        UTENTE_COD_PREFERITI_REPORT_SCARICO_ZOO = 885
        UTENTE_COD_PREFERITI_REPORT_CARICO_ZOO = 899

        SUPERUSER_IMPEDISCI_ELIMINAZIONE_CONTATTI_E_RISORSE_UMANE = 886

        Operazioni_Contab_Collegate_QdC = 888

        SUPERUSER_IMPEDISCI_INS_MOD_BROGLIACCIO = 889

        UTENTE_COD_PREFERITI_REPORT_PIANO_COLTURALE_CATASTO_GRID = 890

        SUPERUSER_Mod_Filtro_Ricerca = 891

        SUPERUSER_Mod_Analisi_Terreno = 892

        ' Gestisce se il check mostra firma ODC e' visibile o no, raggiona sulle 3 stampe bio: Op colturali, Materie Prime e Vendite
        UTENTE_Cod_MostraFirmaODCBio = 893
        ' Gestisce se il check mostra data e' precheckato o no, raggiona sulle 3 stampe bio: Op colturali, Materie Prime e Vendite
        Utente_Cod_DefaultMostraDataBio = 894
        ' Gestisce se il check mostra data e' precheckato o no, raggiona sulla stampa Registri ACA
        Utente_Cod_DefaultMostraDataACA = 895

        UTENTE_COD_PREFERITI_REPORT_ANALISI_ZOO = 896

        SUPERUSER_NuovaAzienda_ProprietaContatti = 897
        SUPERUSER_NuovaAzienda_ContattiPubblici = 898
        SUPERUSER_NuovaAzienda_DefaultRappContabile = 903

        FiltroDataScadenzaFarmaco = 900

        SogliaMinimaPioggeGiornaliere = 901

        ModificaMultiplaZoo = 902

        UTENTE_COD_PREFERITI_CHECKLIST = 1104
        ImpresaPadreDefault = 1105

        CREATA_PER_ERRORE_NON_USARE = 1004

        UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI = 1107
        UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI = 1108
        UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI = 1109
        UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI = 1110
        UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI = 1111
        UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO = 1112
        UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA = 1113
        UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA = 1114
        UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI = 1115
        UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI = 1116
        UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI = 1117
        UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO = 1118

        SUPERUSER_ModalitaDemetra = 1119

        '=====================================================================================
        ' AGGIORNARE IL FILE \\rubino2\DOCUMENTAZIONE\GIAS --- Utenti\Utenti_Impostazioni.xlsx
        '=====================================================================================

    End Enum

    Public Enum enum_AreaGIAS
        Anagrafiche = 7
        Banchedati_DPI = 5
        Biologico = 10
        Cantine = 20
        CheckList = 30
        Conferimento = 35
        Contabilita = 37
        ControlloDiGestione = 40
        DAA = 244
        Documentale = 50
        DSS = 60
        FatturazioneElettronica = 65
        FF = 70
        ImportazioneFascicoli = 75
        GiasAPP = 90
        GiasToGias = 100
        GIS = 110
        IntegrazioneconSistemiEsterni = 120
        IrriFrame = 130
        Magazzino = 140
        ManagerFramework = 145
        ManagerCliente = 146
        Meteo = 310
        MVV = 242
        NonConformita = 150
        PathFinder = 160
        PianiDiCampionamento = 170
        PianiDiConcimazione = 180
        PianiSemina = 185
        Planning = 190
        Pratiche = 200
        PUA = 210
        QdC = 220
        Qualita = 230
        Sementieri = 320
        Sian = 240
        Statistiche = 250
        Storicizzazioni = 330
        Tracciabilita = 260
        Utenti = 265
        Utility = 340
        Vinificazione = 270
        Visite = 280
        Zoo = 290
        ALTRE = 300
        SysAdmin = 400

    End Enum

    Public Enum enum_GestoreAzienda
        Coldiretti = 3001
        Confagricoltura = 3002
        CIA = 3003
        Ordine_degli_Agronomi = 3004
        Perito_Agrario = 3005
        Servizi_Agricoli_Europei = 3006
        SOLECO = 3007
        Liberi_Agricoltori = 3008
        Privato = 3009
        COPAGRI = 3010
        CAF_AGRI = 3011
        UNICAA = 3012

    End Enum

    Public Enum enum_Gestione_Lotti
        Nessuna = 0 'default
        Obbligatoria = 1
        Facoltativa = 2
    End Enum

    Public Enum enum_Gestione_Giacenze
        SoloMovimentati = 0 'default
        SoloPresenti = 1
        TuttiProdotti = 2
    End Enum

    Public Enum enum_SEMINA_TIPO
        'Fast = 10
        'Leggera = 20
        'Leggera_Con_Dettagli_Magazzino = 30
        Solo_Semina_Default = 30 'solo semina (con o senza magazzino)
        Semina_e_Modifica_Appezzamenti_Default = 40 'semina (con o senza magazzino) + modifica appezzamento
        Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default = 50 'frazionamento appezzamento + semina sui singoli (con magazzino)
        Solo_Semina_Vincolo = 60 'solo semina (con o senza magazzino)
        Semina_e_Modifica_Appezzamenti_Vincolo = 70 'semina (con o senza magazzino) + modifica appezzamento
        Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo = 80 'frazionamento appezzamento + semina sui singoli (con magazzino)
    End Enum

    Public Enum enum_RACCOLTA_TIPO
        Fast = 10                           'data e impianti
        Leggera = 20                        'data, impianti e qta prodotto
        Leggera_Con_Dettagli_Magazzino = 30 'data, impianti, qta prodotto e carico magazzino
        Standard = 40
        Raccolta_e_Cura = 50
    End Enum

    Public Enum RACCOLTA_TIPOLOGIA_PRODOTTO
        Non_Specificata = 1
        Default_Su_Semilavorati = CostantiPersonalizzate.SEMILAVORATI_VEGETALI
        Default_Su_Trasormati = CostantiPersonalizzate.TRASFORMATI_VEGETALI
        Fissa_Su_Semilavorati = CostantiPersonalizzate.SEMILAVORATI_VEGETALI
        Fissa_Su_Trasormati = CostantiPersonalizzate.TRASFORMATI_VEGETALI
    End Enum

    Public Enum enum_Fattura_TxtReport

        Titolo = 1
        FatturatoA = 2
        Destinatario = 3
        DataEmissione = 4
        Numero = 5
        Scadenza = 6
        Valuta = 7
        Pagina = 8

        ModalitaPagamento = 10
        Note = 11

        DataSpedizione = 20
        Colli = 21
        Peso = 22
        'non gestito, lasciato LitriLabel1 con dicitura fissa LT
        'Litri = 23  
        Aspetto = 24
        Causale = 25
        Trasporto = 26
        GestioneVettore = 27
        Vettore = 28

        DataConsegna = 30
        FirmaDestinatario = 31
        FirmaConducente = 32

        DescrizioneBeni = 40
        Qta = 41
        Prezzo = 42
        Sconto = 43
        Importo = 44
        AliqIVA = 45
        UnitaMisura = 46

        TotImplordo = 50
        TotSconti = 51
        TotImpNetto = 52
        TotImposta = 53
        Tot = 54 'TxtTotale
        TotDocumento = 55 'TxtTotaleDaPagare non è gestita

        CalcoloImposta = 60
        IVA_imponibile = 61
        IVA_aliquota = 62
        IVA_imposta = 63

        SEO = 70
        Articolo62 = 71
        Privacy = 72
        Conai = 73


    End Enum

    Public Enum enum_CodificaStampe

        Nessuna = 0

        SchedaCampagna_2078 = 1
        RegistroTrattamenti = 2
        SchedaRegistrazione = 3
        SchedaCampagna_Biologico = 4

        Bolle = 5
        Fatture = 6

        Quadro_P = 7

        PianoRaccolta = 8

        SchedaMagazzinoMovimenti = 9
        SchedaMagazzinoGiacenze = 10
        SchedaMagazzinoFertilizzanti = 11
        SchedaMagazzinoProdottiFitosanitari = 12

        SchedaCampagna_2078_Semplificata = 13
        RegistroTrattamenti_Semplificata = 14
        SchedaRegistrazione_Semplificata = 15
        SchedaCampagna_Biologico_Semplificata = 16

        ReportRisultatoFilrone = 17

        RiepilogoImpiegoSuperfici = 18

        Eurep_Gap = 19

        SchedaColturale_Biologico = 20
        SchedaMateriePrime_Biologico = 21
        SchedaVendite_Biologico = 22

        Esporta_GiasToSap = 23

        PAP_Vegetale = 24

        SchedaTracciabilita = 25

        ReportConserveItalia = 26

        SchedaCampagna_ConserveItalia = 27

        RapportinoStrube = 28

        Eurep_Gap_Semplificata = 29

        DatiAnelloFilieraIngresso = 30
        DatiAnelloFilieraLegameLotti = 31

        GestioneAllegati = 32

        AnalisiCosti_XLS = 33
        AnalisiRicavi_XLS = 34
        MarginiEconomici_XLS = 36

        SchedaTracciabilita_ByLotto = 35

        EstrattoreDatiGrafici = 37

        LibroConferimenti = 38

        PianoColturale = 39

        SchedaCampagna_Pizzoli = 40

        Esportazione_OP_Inv = 41

        Esportazione_OP_Gest = 52

        Esportatore_Universale_Impianti = 42
        Esportatore_Universale_Imprese = 43
        Esportatore_Universale_Centri = 44
        Esportatore_Universale_Appezza = 45
        Esportatore_Universale_Agenda = 46
        'Esportazione Rintraccio
        Esportatore_Universale_Rintraccio = 49

        AnalisiTerreno = 47

        'Report Riconversione Varietale x AgriBologna
        Report_RiconversioneVarietale = 48

        'Impegno Produzione Soci x AgriBologna
        Report_ImpegnoProduzioneSoci = 51

        Verifica_Conformita = 50

        ImportaAnagrafiche_XLS2GIAS = 53

        GestioneRicette_Lista = 54

        Registro_FattureAcquisto = 55
        Registro_FattureVendita = 56
        Registro_Corrispettivi = 57
        Registro_AltriCosti = 58
        Registro_AltriRicavi = 59
        Registro_PrimaNota = 60

        Lista_InsolutiClienti = 61
        Lista_InsolutiFornitori = 62

        LiquidazionePeriodica_IVA = 63
        Bilancio_Civilistico = 64
        PianoDeiConti = 65
        Bolle_Conferimento_Soci = 67
        Bolle_Conferimento_Diversi = 68

        Report_NPK_Totali = 66

        PAP_Zootecnico = 69

        Costo_Manodopera_XLS = 70
        Costo_ParcoMacchine_XLS = 71

        Analisi_Vino = 72

        RicevuteFiscali = 73

        Atto_Notorio = 74

        Adesione_Etico_Ambientale = 75
        Tenuta_Scheda_Campagna = 76

        Adesione_DPI = 77
        Impegnativa_Eurep = 78
        Impegnativa_QC = 79
        Impegnativa_Confusione_Sessuale = 80
        Codice_Condotta = 81

        Modello4 = 82
        Registro_Stalla = 83

        ImportaAnagraficheAnimali_XLS2GIAS = 84

        VasiVinari = 85

        GestioneCondizionalita = 86

        Esportatore_Codifiche_Cultivar = 87
        Esportatore_Codifiche_SpecieVegetali = 88

        Produzioni_XLS = 89

        FiltroStrube = 90

        Esportazione_AnagraficaProdotti = 91

        PacchettoIgiene_RegistroFornitori = 92

        PacchettoIgiene_RegistroClienti = 93

        PacchettoIgiene_SchedaUsoAlimentiOGM = 94

        PacchettoIgiene_RegistroAlimentazioneStalla = 95

        PacchettoIgiene_RegistroRazionamento = 96

        PacchettoIgiene_RegistroAnalisiNonConformi = 97

        'BUCO NEL 98

        Esportazione_AnagraficaContatti = 99
        Esportazione_CellulariContatti = 100

        Analisi_Vino_Derivanti_Da_Travasi = 101

        Bilanci_DiVerifica_Confronto = 102

        Nota_Accredito = 103

        Mastrino = 104

        Esportazione_OP_Gest_Coop = 105

        Registri_Preparazioni = 106

        EtichetteVascheEnologiche = 107

        'BUCO X NICOLETTA 108

        Bilancio_Fertilizzazioni = 109

        Programmazione_Vegetale = 110

        Esporta_GiasToXMLPubblico = 111

        Esporta_Conferimenti_JDEdwards = 112

        Buono_Accettazione_Diversi = 113 'stampa bolla del giaslan

        Buono_Accettazione = 114

        Registro_Fertilizzazioni = 115

        SchedaTracciabilita_Animale = 116

        RaccoltaDatiAnagrafici = 117

        Importa_RaccolteConferimenti_DaRintraccio = 118

        ADD_Filtro_Report_Accettazione_DaDiversi = 119
        ADD_Riepilogo_Conf_XSpecie = 120
        ADD_EC_Bolle_Accettazione_DaDiversi = 121
        ADD_EC_Imballi = 122
        ADD_Saldo_Imballi = 123
        ADD_Export_Bolle_Accettazione_DaDiversi = 124
        ADD_Export_Traportatori = 125

        Importazione_DDT_PianoColturale = 126

        Mandato_Trasmissione_Telematica_Dati = 127

        DOCO = 128
        DAA = 129

        Impegnativa_Orticole_Gest_Annuale = 130
        Impegnativa_Orticole_Gest_Breve = 131
        Impegnativa_Orticole_Industria = 132
        Impegnativa_Pomodoro_Industria = 133
        Impegnativa_Fagiolino_Mercato_Fresco = 134

        ImportaContatti_XLS2GIAS = 135


        Certificato_Pomodoro = 136
        Certificato_Pomodoro_Interno = 137
        Certificato_Pomodoro_Esterno = 138
        Registro_CaricoScarico_Pomodoro = 139
        Esporta_ConferimentiPomodoro_Agrea = 140
        Esporta_BolleAccettazione2AltroClienteGias = 141

        ExportExcel_MonitoraggioCE = 142

        ADD_ExportExcel_CertificatiPomodoro = 143

        SchedaCampagna_Ricette = 144

        GestioneRicette_Edit = 145

        ExportExcel_MonitoraggioCE_Aggregata = 146

        GestioneEtichette_Trasformati = 147

        LibroConferimenti_XLS = 148

        RegistroTrattamenti_Veneto = 149

        SchedaCampagna_Multicentro = 150

        RiepilogoImpiegoSuperfici_Multiazienda = 151

        Eurep_Gap_Multicentro = 152

        SchedaPreparati_Biologico = 153

        Notifica_Biologico = 154

        RiBa_Report_Presentazione = 155
        RiBa_Export_CBI = 156

        Listini_XLS = 157

        DDT_Contabilizzato_Emesso = 158

        Ordine = 159

        AgentiProvvigioni_XLS = 160

        Eurep_Gap_Multicentro_Immediata = 161

        Report_Incongruenze_CatastoVSAgrea = 162

        Preventivo_Vendita = 163

        Scheda_Rilievi = 164

        PianoColturaleCatasto = 166

        GiornaleContabile = 167

        Ordine_Acquisto = 168

        FF_Etichette = 169

        Liquidazione_Soci = 170

        Allegato_CatastoeValorizzazioni = 171

        Esportazione_OP_Produttori = 172
        Esportazione_OP_Catasto = 173

        ADD_ExcelTracciabilitaConferimenti = 174

        FreshFood_BollaAccettazione = 175
        FreshFood_DistintaCarico_Accettazione = 176
        FreshFood_AutoDDT_Accettazione = 177

        SchedaMagazzinoMovimentiExcel = 178

        ' OLD Buono_Accettazione_Diversi_DDTRicevuto = 165
        ConferimentoUva_DDTRicevuto = 165
        ConferimentoUva_DistintaCarico = 179
        ConferimentoUva_AutoDDT = 180

        Registro_Fertilizzazioni_Massivo = 181
        Registro_Trattamenti_Massivo = 182

        Conf_FiltroStampe = 183
        Conf_EC_Imballi = 184
        Conf_Saldo_Imballi = 185
        Conf_Riepilogo_Conferimenti = 186
        Conf_Tracciabilita = 187

        Bilancio_SezioniContrapposte = 188
        EstrattoConto_Contatti = 189

        Cespiti_FiltroStampe = 190

        SchedaCampagna_ProvAut_Trento = 191

        SchedaCampagna_Multi_Lombardia = 192

        FreshFood_BollaCampionatura = 193

        RisultatoAnalisiConformita = 194

        FreshFood_FatturaLiquidazioneSoci = 195
        FreshFood_PagatiSuConferito = 196
        FreshFood_AutofatturaLiquidazioneSoci = 197
        FreshFood_RiepilogoLiquidazioneSoci = 198
        FreshFood_PagatiSuCampionato = 199
        Analisi_Progetti = 200

        SchedaInterventiAgronomici = 201

        EsportazioneAgeaTxtCSV = 202

        RegistroAziendaleUnico = 203

        SchedaCatastoeUtilizzi = 204

        RegistroTrattamentiVeneto_StdCondizionalita = 205


        RiepiloghiAccise = 206
        DAA_GaranzieCircolanti = 207
        DAA_PariteSospensione = 208

        MVV = 209

        Report_Vendita_PDF = 210

        ReportRisultatoFiltroneG2G = 211

        EsportazionePomodoroIndustriaOINordItalia = 212

        RiepilogoProdottiUtilizzati = 213

        PassaportoMaterialeVivaistico = 214

        ReportRisultatoFiltroneIncludiVisita = 215

        Bilancio_Fertilizzazioni_Dettagliato = 216

        Conf_Esportazione_BolleFF_XLS = 217

        Filtro_StampeBiologico = 218

        Conf_EsportazionexTrasportatori_XLS = 219
        Conf_RiepilogoxArticolo = 220
        Conf_EC_Bolle = 221
        Conf_Certificato_Pomodoro = 222
        Conf_Esportazione_CertificatiPomodoro_XLS = 223

        UMA_RichiestaCarbPrevisioneLav = 224
        UMA_VerbaleIstruttoriaRichCarb = 225
        UMA_RendicontazioneCarb = 226
        UMA_IstruttoriaRendCarb = 227

        EstrazioneCatastoAffitti = 228

        Conf_ComunicazioneCredito = 229

        CredenzialiPrivacy = 230

        Report_OrdiniVivaio = 231
        Report_PianoColturalePreventivoVivaio = 232

        Impegnative_capitolati = 233
        Adesione_ModuloGrasp = 234
        Adesione_ProtocolloGlobalGAP = 235
        Adesione_NurtureModule = 236
        Adesione_Despar = 237
        Adesione_Conad = 238
        Accordo_Responsabilita_di_Filiera = 239
        Dichiarazione_di_Responsabilita = 240
        Fitoregolatori_Kiwi = 241
        Adesione_StandardLeaf = 242
        SchedaAziendale = 243

        ImpegnoProduzioneSociDivisoxCentri = 244


        ObiettivoDiProduzioneAsipo = 245

        ImpegnativaColtivazioneConferimento = 246

        QuestionarioValutazioneAzienda_Aggiornamento = 247

        PianoColturaleCatastoGrid = 248

        Statistometro = 249

        SchedaCampagna_Multicentro_ACA = 250

        Stampa_Analisi_Fitofarmaci = 254

        Stampa_Etichetta = 255

        Stampa_Rapporto_di_Prova = 256

        Stampa_Checklist_Global_GAP = 257
        Stampa_Abilitazioni = 258

        Stampa_Zoo_Sintesi_Partite = 259
        Stampa_Zoo_Dettaglio_Partita = 260

        '... ... ULTIMO ... ...


        'ricordare di aggiornare il documento Codici Stampe.doc:

        'su \\Rubino2\documentazione\PROGETTO --- AgronicaStampe

        'Per le nuove stampe create nel sito AgronicaStampe_2010,
        'aggiornare lo script Gias_Configurazione_Siti.sql nella soluzione ScriptGIAS
        'aggiungendo la chiave nella variabile enumStampe2010

        '!!!!!
        'Per le stampe che passano per il Filtrodi Ricerca, aggiungere le entita gestite qui: AgronicaCoreFiltroneBIZ:FiltroRicerca.estraiTipoMostraGestitixStampa()
    End Enum


    Public Enum enum_TipoFiltrone

        Stampa = 1
        Utenti = 2
        Agenda = 3
        AnalisiCosti = 4
        ModificaImpianti = 5
        PianificazioneInterventi = 6
        EliminaInterventi = 7
        Esportazione_OP_Inv = 8
        Esportazione_OP_Gest = 17
        Esportatore_Universale_Impianti = 9
        Esportatore_Universale_Imprese = 10
        Esportatore_Universale_Centri = 11
        Esportatore_Universale_Appezza = 12
        Esportatore_Universale_Agenda = 13
        Esportatore_Universale_Rintraccio = 16
        Blocco_OperazioniAgenda = 14
        Richiesta_Verifica_Conformita = 15
        Associa_Ricetta_Impianti = 18
        Esportazione_CellulariTecnici = 19
        Esportazione_OP_Gest_Coop = 20
        Bilancio_Fertilizzazioni = 21
        MultiModificaImpianti = 22
        Esportatore_Contatti = 23
        Associa_Ricetta_Interventi = 24
        MultiModificaAgenda = 25
        'FiltroStrube = 20
        FiltroStrube = 26
        PianiCampionamento_AggiungiImpianti = 27
        CancellaAppezzamenti = 28
        Gestione_Servizi = 29
        PianoConcimazione = 30
        ExportSigpa = 31
        ModificaAperturaCampi = 32
        OperazioniMultiAziendali = 33
        Esportazione_AgeaTXTCSV = 34
        EsportazionePomodoroIndustriaOINordItalia = 35
        StimeProduzione = 36
        Modifica_Multipla_PianoColturale = 37
        DuplicaOperazione = 38

    End Enum

    Public Enum enum_PosizioniNodo
        TipoNodo = 0
        Piva = 1
        Sa_Cod = 2
        Campo_Cod = 3
        Appezza = 4
        Id_Imp = 5
        p_Part_Cod = 6
        p_Provincia_Cod = 7
        p_Comune_Cod = 8
        p_Sezione = 9
        p_Foglio = 10
        p_Numero = 11
        p_Subalterno = 12
        Cod_Fiscale = 13
        Fabbricato_Cod = 14
        Prodotto_Cod = 15
        Data_Lavorazione = 16
        Analisi_Certificato_Cod = 17
        Analisi_Testata_Cod = 18
        Analisi_Dettaglio_Cod = 19
        Analisi_Campione_Cod = 20
        PianoConcimazione_Testata_Cod = 21
        Progetto_Cod = 22
        Programmazione_Cod = 23
        Programmazione_Entita_Cod = 24
        id_agenda = 25
        PivaPadre = 26


    End Enum

    Public Enum enum_TipoNodo

        Indefinito = 0

        Utente = 1
        Impresa = 2
        Centro = 3
        Campo = 4
        Serra = 41
        Appezzamento = 5
        ImpiantoNudo = 6
        ImpiantoArborea = 7
        ImpiantoErbacea = 8
        ImpiantoOrticola = 9
        Particella = 10
        Persona = 11

        CatastoAziendale = 18
        Impianto_Generico = 19
        Fabbricato_Generico = 20

        f_Abitazione = 21
        f_Magazzino = 22
        f_Silos = 23
        f_CellaFrigorifera = 24
        f_ImpiantoLavorazione = 25
        f_Stalla = 26
        f_Fienile = 33
        f_Essiccatoio = 10034

        p_PortafoglioProdotti = 27
        p_Prodotto = 28
        p_Preparazione = 29

        x_ConsistenzeAnimali = 30
        x_MovimentiMagazzino = 31
        x_PreparazioniAlimentari = 32
        x_GiacenzeMagazzino = 34
        x_ParcoMacchine = 35
        x_Contatti = 36
        x_ListaFabbricatiAziendali = 37

        x_Cooperativa = 38
        x_Consorzio = 39
        x_OP = 40

        Analisi_Certificato = 42
        Analisi_Testata = 43
        Analisi_Dettaglio = 44
        Analisi_Campione = 45

        PianoConcimazione_Testata = 46

        DistintaDiProduzione = 47

        x_VariazioniConsistenzeAnimali = 48

        Cantine_Piani = 49
        Cantine_Vasche = 50

        PlanningTestata = 51
        PlanningEntita = 52
        PlanningEntitaImpianto = 53

        Agenda = 54

        ricette_Testata = 60
        ricette_dettaglio = 61
        Anagrafica_Generica = 62

        AgendaDestinazioni = 63
        Precision = 64

        PlanningTotale = 65

    End Enum

    Public Enum enum_TipoFiltroImprese
        xNessuno = 0
        xSequenzaCaratteri = 1
        xTipoConformita = 2
        xPeriodoNotificaScaduto = 3
        xPeriodoNotificaNonScaduto = 4
        xProvincia = 5
        xPartitaIVA = 6
        xRegione = 7
        xValidato = 8
        xNonValidato = 9

        f_Nessuno = 10
        f_RagioneSociale = 11
        f_PartitaIVA = 12
        f_CUAA = 13
        f_CodiceOperatore = 14
        f_RappresentanteLegale = 15
        f_Provincia = 16
        f_Regione = 17
        f_NormaRiferimento = 18
        f_ValidazioneImprese = 19
        f_Specie = 20
        f_Comune = 21
        f_Codici = 22

    End Enum

    ' codici corrispondenti a quanto contenuto nella tabella Codici_Anagrafe di Metaschema
    Public Enum enum_CodiciAnagrafe

        Codice_Ausl = 4

        Centro_Sede_Legale = 101
        Centro_Sede_Aziendale = 102
        Centro_Stabilimento = 103

        TipoAttivita = 1000
        DataFineControllo = 1001
        CodiceCentro_Precedente = 1002
        CodiceCentro_Attuale = 1003 'codice operatore bio
        PartitaIva_Fittizia = 1004
        Associazione_CODEX = 1005
        Conformita_Biologico = 1006
        Conformita_FilieraControllata = 1007

        OTE = 1009
        CodiceCUAA = 1010
        DataFineImpiegoPNC = 1014
        OrientamentoProduttivo = 1015
        TitoloPossesso = 1016
        MetodoDiProduzione = 1018
        pers_LegaleRappresentante = 1019
        pers_Conduttore = 1020
        pers_Delegato = 1021
        pers_Detentore = 1022

        ImpresaPAT = 1278

        Codice_Socio = 1033

        Codice_Certificazione = 1341

        Impianto_LimiteN = 1050
        Impianto_LimiteN_Organico = 1316
        Impianto_LimiteP = 1051
        Impianto_LimiteK = 1052
        Impianto_LimiteMg = 1053
        Impianto_Ibrido = 1054
        Impianto_CodiceB_Maschio = 1055
        Impianto_CodiceB_Femmina = 1056
        Impianto_Genetica_Maschio = 1057
        Impianto_Genetica_Femmina = 1058
        Impianto_OffType_Maschio = 1059
        Impianto_OffType_Femmina = 1060
        Impianto_TraFila_Maschio = 1061
        Impianto_TraFila_Femmina = 1062
        Impianto_SuFila_Maschio = 1063
        Impianto_SuFila_Femmina = 1064
        Impianto_Interbina = 1065
        Impianto_Germinabilita = 1066
        Impianto_PianoSemina = 1071
        Impianto_LottoFitosanitarioOmogeneo = 1072
        Impianto_GroverCode = 1076
        Impianto_Nr_domanda_ACA = 1359

        Appezzamento_CodiceContratto = 1073
        Impianto_Cooperativa = 1074
        Appezzamento_ConfiniRischio = 1075

        Organismo_Referente = 1074

        GestioneDocumentiAllegati = 1077

        CodiceRigaRiferimentoQuadroP = 1078

        Coltura_Precedente_1 = 1079
        Coltura_Precedente_2 = 1080
        Coltura_Precedente_3 = 1081
        Coltura_Precedente_4 = 1082

        Impianto_Taglio_Tuberi_Patate = 1083
        Impianto_Parti_Tuberi_Patate = 1084

        Codice_Appezza_Biologico = 1085

        Codice_Libro_Soci = 1086
        Data_Iscrizione_Libro_Soci = 1087
        Tecnico = 1088
        Codice_Cliente = 1089
        Codice_Fornitore = 1090
        Codice_Fornitore_2 = 1091
        Codice_Fornitore_3 = 1092

        Organismo_di_Controllo = 1205
        Mandato = 1206

        Codice_Capitolato_Privato = 1093

        Codice_Residuo = 1340

        Codice_Certificazione_Prodotto = 1344

        Riferimento_Alfanumerico_Appezzamento = 1104

        Magazzino_Conferimento = 1122

        Codice_Unione_OP = 1125

        Certificazione = 1130

        Codice_GlobalGap = 1261
        Codice_GlobalGap2 = 1314

        Tribunale_di_registrazione = 1275
        Regolamento_Aziendale_Default = 1294
        Disciplinare_Aziendale_Default = 1295

        Impianto_IAF_ImpegniAggiuntiviFacoltativi = 1296

        Codice_Destinazione = 1297

        Programmazione_Impianto_Pratica_Cod = 1298

        INDICODE_EDI_Euritmo_Codice_punto_consegna_NAD = 4013
        INDICODE_EDI_Euritmo_Codice_magazzino_emissione_ordine_NAB = 4014
        INDICODE_EDI_Euritmo_Codice_fornitore_NAS_CodForn = 4015
        INDICODE_EDI_Euritmo_Tipo_Codice_fornitore_NAS_QCodForn = 4016

        GS1_CompanyPrefix = 1276
        BNDOO_BancaDatiOperatoriOrtofrutticoli = 1277

        GestioneContabile_DataInizio = 1284
        GestioneContabile_ConsideraSaldiIniziali = 1285

        Tipo_Gestione_Impresa = 1286

        Zespri_Codice_kPIN = 1287
        Zespri_Block_Name = 1288
        Zepri_Grower_Number = 1317
        Zepri_Identificativo_Flex2B = 1329

        Default_Sincro_DDT_Terremerse = 1289

        CampoSperimentaleBZ = 3000
        SetAside = 3001
        IntercalareAzione3 = 3002
        Azione09_SiepieBoschetti = 3003
        Azione10_Laghetti = 3004
        AzioneG = 3005
        ForestazioneL2080 = 3006
        Agricampeggio = 3007
        Lago = 3008
        Alberatura = 3009
        AzioneF3 = 3010
        AzioneD1 = 3011
        AzioneF1 = 3012
        VivaioOrnamentali = 3013
        Azione_Agroambientale = 3014
        Tara_improduttiva = 3015
        Prato = 3016
        Affitto = 3017
        Misura_2H = 3018
        Orto = 3019
        Biomassa = 3020
        Colture_a_perdere_per_fauna_selvatica = 3021
        Frutteto = 3022
        Erbaio = 3023
        Bosco = 3024
        NessunaMappaturaConGias = 3078
        UtilizzoImpianto_DaDefinire = 3103



        Riferimento_Alfanumerico_Campo = 1279
        Coltura_Appezzamento_Precedente_1 = 1280
        Coltura_Appezzamento_Precedente_2 = 1281
        Coltura_Appezzamento_Precedente_3 = 1282
        Coltura_Appezzamento_Precedente_4 = 1283

        Coltura_Campo_Precedente_1 = 1290
        Coltura_Campo_Precedente_2 = 1291
        Coltura_Campo_Precedente_3 = 1292
        Coltura_Campo_Precedente_4 = 1293

        Capitolato_Privato = 1093

        Gruppo_Varietale_1 = 1094
        Gruppo_Varietale_2 = 1095
        Gruppo_Varietale_3 = 1096
        Gruppo_Varietale_4 = 1097
        Gruppo_Varietale_5 = 1098
        Gruppo_Varietale_6 = 1099
        Gruppo_Varietale_7 = 1100
        Gruppo_Varietale_8 = 1101
        Gruppo_Varietale_9 = 1102
        Gruppo_Varietale_10 = 1103
        Latitudine = 1105
        Longitudine = 1106

        Dettaglio_Specie_Personalizzato = 1108

        Centro_Aziendale_Esterno_Collegato = 1337

        UTE = 1338

        PivaSuperUser_Origine_Dato = 1107

        Codice_ICQ = 1109
        Impianto_Codice_Programmazione = 1110

        CodiceREA = 1112
        CodiceISO = 1111
        NumIscrAlboSocCoop = 1113
        NumRegImprese = 1119

        CodiceStabilimento = 1114 'sul fabbricato
        CodiceConferente = 1115
        CodiceProduttore = 1116
        CodiceCooperativa = 1117

        Codice_Zona = 1118 'riferito all'impresa

        Flag_Smart_Impianto = 1131
        Flag_Smart_Appezzamento = 1132


        Codice_Specie_Agea = 1133
        Codice_Cultivar_Agea = 1134

        Isola = 1320

        codice_RUOP = 1321

        Codice_Sito_Vivaio = 1323

        Contratto_Produzione = 1324
        Sup_Contratto = 1325
        Filiera = 1326

        Riferimento_Trasferimento_Dati = 1327

        Data_Inizio_Portinnesto = 1328 'dal 05/21 la data d'inzio del portinnesto viene salvata sul nuovo campo Reg_Impianti.Data_Inizio_Portinnesto (anzichè  in reg_impianti_codici)

        MacchinaCertificazioneTaratura = 1358

        CodiceCliente_FRUTTAGEL = 2025

        TracciaEsportazioneSIGPA_Fabbricati_XFertilizzanti = 5007
        TracciaEsportazioneSIGPA_Fabbricati_XFormulati = 5017

        TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFertilizzanti = 5008
        TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFormulati = 5018

        CodiceCliente_GranfruttaZani = 2029
        CodiceCliente_Apofruit = 2013
        CodiceCliente_Orogel = 2039
        CodiceCliente_Apot = 2042

        CodiceCliente_Tecnoterr = 2063
        CodiceCliente_Casalasco = 2064

        acciseDAA_CodiceAccisa_Mittente = 4003
        acciseDAA_CodiceAccisa_UfficioDoganale = 4005
        acciseDAA_CodiceAccisa_Destinatario = 4006
        acciseDAA_CodiceAccisa_codiceUA = 4008
        acciseDAA_Accise_Conto_Garanzia = 4009
        acciseDAA_Codice_Accise_Prefisso = 4010
        acciseDAA_VIDIMA_RegistroElettronica = 4011

        Contatto_OrganismoDiControllo_BIO = 4018

        'CBI corporate interbancario
        CBI_CodiceSIA = 1264
        Codice_Centro = 1265

        'spesometro
        CodiceAteco2007 = 1266
        codiceFiscaleProduttoreSoftware = 1267

        Spesometro_intermediario_CodFisc = 1268
        Spesometro_intermediario_AlboCAF = 1269



        SISPAC_Codice_Anagrafico = 4007
        '----CODICI APOFRUIT SIAGR
        'inizio 1135

        'impresa
        'V01CON00_VCCOD = 1213 'Codice Conferente
        'V01CON00_VCCOD = Codice_Socio 'Codice Socio (Conferente) uso quello presente in gias, visualizzato SOCIO
        V01CON00_VCDNA = 1135 'Data di nascita
        V01CON00_VCLNA = 1136 'Località di nascita
        V01CON00_VCZON = 1137 'Zona
        V01CON00_VCSUA = 1138 'Superficie agricola
        V01CON00_VCQTE = 1139 ' Codice socio Precedente ex Qta tassa altre coop
        V01CON00_VCGRU = 1140 'Gruppo trasportatori
        V01CON00_VCTRA = 1141 'Codice trasportatore
        V01CON00_VCNIS = 1142 'Numero inscrizione
        V01CON00_VCDIS = 1143 'Data iscrizione
        V01CON00_VCAIC = 1144 'Anno inizio conferimento
        V01CON00_VCIFA = 1145 'Flag Fattura/Autofattura
        V01CON00_VCLIS = 1146 'Codice Listino
        V01CON00_VCMEZ = 1147 'Flag Mezzadria
        V01CON00_VCVAL = 1240 '% meszzadro
        V01CON00_VCPRP = 1148 'Codice proprietario
        V01CON00_VCAT1 = 1149 'Attributo 1
        V01CON00_VCAT2 = 1150 'Attributo 2
        V01CON00_VCAT3 = 1151 'Attributo 3
        V01CON00_VCAT4 = 1152 'Attributo 4
        V01CON00_VCAT5 = 1153 'Attributo 5
        V01CON00_VCAT6 = 1154 'Attributo 6
        V01CON00_VCAT7 = 1155 'Attributo 7
        V01CON00_VCAT8 = 1156 'Attributo 8
        V01CON00_VCAT9 = 1157 'Attributo 9
        V01CON00_VCAT0 = 1158 'Attributo 10
        V01CON00_VCDUM = 1175 'Data ultima variazione
        V01CON00_VCANN = 1160 'Flag annullamento
        V01CON00_VTPCON = 1161 'Tipo conferimento
        V01CON00_VCCOIS = 1162 'Centro di conferimento
        Stabilimento = 1163 'Stabilimento di conferimento (FARLO PUBBLICO)
        V01CON00_VANAN = 1164 'Anni anzianità
        V01CON00_VANUL = 1165 'Anno ultima liquidazione
        V01CON00_VANULI = 1166 'Anno ultimo invecchiamento
        V01CON00_VLIATR = 1167 'Linea addebito trasporto
        V01CON00_VLIRTR = 1168 'Linea rimborso trasporto
        V01CON00_VCFOR = 1169 'Codice Gruppo

        'impresadettagli
        CATCON0F_CCRAP = 1170 'Codice Rappresentante
        CATCON0F_CCEST = 1171 'Superficie totale
        CATCON0F_CCRIF = 1172 'Rif. nucleo familiare
        CATCON0F_CCSES = 1173 'Sesso
        CATCON0F_CCDTN = 1174 'Data notifica
        'CATCON0F_CCDTV = 1175 'Data variazione/Taratura Atomiz. non lo sal,co, lo uso per la prima importaz e basta

        'centro
        CATANA0F_CAFON = 1262 'Numero Fondo
        CATANA0F_CAZON = 1176 'zona/frazione
        CATANA0F_CAZOP = 1177 'produttivita zona/frazione
        CATANA0F_CACON = 1241 'Note/Confinanti
        CATANA0F_CARES = 1242 'Responsabile Centro
        CATANA0F_CAFPR = 1243 'Flag Proprietario
        CATANA0F_CATCO = 1178 'Tipo Conduzione
        CATANA0F_CASUT = 1244 'Sup. Totale
        CATANA0F_CASUP = 1245 'Sup. Pianura
        CATANA0F_CASUC = 1246 'Sup. Collina
        CATANA0F_CATAR = 1247 'Sup. Tara
        CATANA0F_CASUI = 1248 'Sup. Incolta
        CATANA0F_CASUF = 1249 'Sup. Frutteto
        CATANA0F_CASUO = 1250 'Sup. Orticola
        CATANA0F_CASUV = 1251 'Sup. Vigneto
        CATANA0F_CASUS = 1252 'Sup. Seminativo
        CATANA0F_CASUA = 1253 'Sup. Altri
        CATANA0F_CAALT = 1254 'Altitudine
        CATANA0F_CAFLI = 1255 'Flag Irriguo
        CATANA0F_CAFLH = 1256 'Flag Acqua
        CATANA0F_CAFLR = 1257 'Reperibilità acqua
        CATANA0F_CAREA = 1258 'Reddito agrario
        CATANA0F_CARED = 1259 'Reddito Dominicale
        CATANA0F_CAPCA = 1260 'Perc. Abbattimento

        CATANA0F_CAA01 = 1179 'tecnico di riferimento zona
        CATANA0F_CAA02 = 1180 'azienda eurepgap
        CATANA0F_CAA03 = 1181 'sup totale per scaglioni
        CATANA0F_CAA04 = 1182 'sup ortofrutta per scaglioni
        CATANA0F_CAA05 = 1183 'tecnico di riferimento RER
        CATANA0F_CAA06 = 1184 'Prospettiva Aziendale
        CATANA0F_CAA07 = 1185 'Aumento-Diminuzione prodotti
        CATANA0F_CAA08 = 1186 'Fascicolo Aziendale

        'impianto
        CATIMP0F_CINPR = 1214 'Numero Progressivo Impianto
        CATIMP0F_CIFPC = 1187 'flag pianura collina
        CATIMP0F_CIFIR = 1188 'flag irriguo
        CATIMP0F_CISER = 1189 'flag serra
        'CATIMP0F_CILIN = 1190 'codice lotta intagrata, (uso capitolato)
        CATIMP0F_CIRAC = 1191 'raccolta manuale/meccanica
        'CATIMP0F_CIPIN = 1192 'Portinnesto
        'CATIMP0F_CIALL = 1193 'Allevamento
        CATIMP0F_CIIRR = 1194 'Irrigazione
        'CATIMP0F_CITPR = 1195 ' tpo produzione-regolamento bio
        'CATIMP0F_CINAP = 1195 'numero appezzamento per consociazione (utilizzato enum_CodiciAnagrafe.Codice_Appezza_Biologico)
        'CATIMP0F_CIQTP = 1196 'raccolta ottimale
        'CATIMP0F_CIQTC = 1197 'raccolta corretta
        CATIMP0F_CIC02 = 1198 'Fumigazione
        Contributi = 1199 'contributi
        'CATIMP0F_CITCO = 1200 'Tipo copertura
        CATIMP0F_CIDAL = 1201 'Data ammissione LI
        CATIMP0F_CIDEL = 1202 'Data esclusione LI
        CATIMP0F_CITIM = 1203 'Impianto consociato - successione
        CATIMP0F_CISEQ = 1204 'Sequenza successione - consociazione
        CATIMP0F_CISPE_CIVAR = 1205 'salvo la specie e varieta (loro codice) 
        CATIMP0F_VarietaContratto = 1206 'Segnalo se varieta a contratto 0/1 campo VVAT4

        'impresa note codificate
        ' V01NOC00_NCPRO_9000 = 1207 'codice ente certificazione bio
        V01NOC00_NCPRO_9001 = 1208 'Numero etichetta
        'V01NOC00_NCPRO_9002 = 1209 'Cellulare del socio
        'V01NOC00_NCPRO_9003 = 1210 'codice operatore bio
        V01NOC00_NCPRO_9004 = 1211 'passaggio al codice
        V01NOC00_NCPRO_9005 = 1212 'Coordinate gps
        'impresa note generiche
        V01NOC00_NCPRO_1 = 1215
        V01NOC00_NCPRO_2 = 1216
        V01NOC00_NCPRO_3 = 1217
        V01NOC00_NCPRO_4 = 1218
        V01NOC00_NCPRO_5 = 1219
        V01NOC00_NCPRO_6 = 1220
        V01NOC00_NCPRO_7 = 1221
        V01NOC00_NCPRO_8 = 1222
        V01NOC00_NCPRO_9 = 1223
        V01NOC00_NCPRO_10 = 1224
        V01NOC00_NCPRO_11 = 1225
        V01NOC00_NCPRO_12 = 1226
        V01NOC00_NCPRO_13 = 1227
        V01NOC00_NCPRO_14 = 1218
        V01NOC00_NCPRO_15 = 1229
        V01NOC00_NCPRO_16 = 1230
        V01NOC00_NCPRO_17 = 1231
        V01NOC00_NCPRO_18 = 1232
        V01NOC00_NCPRO_19 = 1233
        V01NOC00_NCPRO_20 = 1234
        V01NOC00_NCPRO_21 = 1235
        V01NOC00_NCPRO_22 = 1236
        V01NOC00_NCPRO_23 = 1237
        V01NOC00_NCPRO_24 = 1238
        V01NOC00_NCPRO_25 = 1239

        V01NOC00_NCPRO_9006 = 1261 'CGN

        '----FINE CODICI APOFRUIT SIAGRultimo 1262
        ' 

        ORGANISMO_DI_CONTROLLO_BIO = 1263 'organismo controllo salvato sui codici del centro, collegato a tabella BIO_Dati_OrganismiControllo

        Fabbricato_Forno_Combustibile = 1270
        Fabbricato_Forno_Fiamma = 1271
        Fabbricato_Forno_Cantiere = 1272
        Fabbricato_Forno_Umidificazione = 1273
        Fabbricato_Forno_Tipo = 1274

        OrganismoRefIntestatarioQDC = 1299
        Codice_Impianto = 1300
        Distinta_Chiusa = 1301
        Codice_Impianto_Ribaltato = 1311
        Algoritmo_Codifica = 1312

        CodiceCatalogoAgeaDemetra = 2315

        ScontoContattoDefault = 4000
        ListinoPrezziAcquistoDefault = 4001
        ListinoPrezziVenditaDefault = 4002
        CodiceAccisa = 4003
        ModalitaPagamentoDefault = 4004
        Codice_Ufficio_Doganale = 4005
        Codice_Magazzino_Fiscale = 4006
        Codice_Accise_UA = 4008
        Codice_Accise_Conto_Garanzia = 4009
        IBANDefault = 4012
        NumeroIscrizioneAlboAutotrasportatori = 4017
        PEC = 4019
        SDI = 4020

        ' --- Per Fattura Elettronica
        CapitaleSociale = 1123

        ''' <see cref="enumTipoSocieta"/>
        TipoSocieta = 1302      'Spa, Sapa, Srl, Altro
        UfficioRea = 1304
        NumeroRea = 1305

        ''' <see cref="enumNumeroSoci"/>
        NumeroSoci = 1306

        ''' <see cref="enumStatoLiquidazione"/>
        StatoLiquidazione = 1307
        DataAttivazioneEFattura = 1309
        DataUltimaRicezioneEFattura = 1310

        ''' <see cref="enumTipoContattoFattura"/>
        TipoContattoFattura = 1303
        PecContatto = 4019
        CodiceSDI = 4020
        RappresentanteFiscale = 4021

        ' codici aggiunti per dichiarazione intenti in e-fatt
        DichiarazioneIntentoNumeroProtocollo = 4022
        DichiarazioneIntentoDataRicezione = 4023

        ReferenteConferimento = 4024

        Visibile_da_App = 1308

        UfficioICQRF = 1313

        Gestione_Vettore_Default = 1315

        CodiceParticella = 1318

        Finalita_Concimazione_Impianto = 1319

        Centro_CodiceStabilimento = 1322

        Zespri_Fasi_Fase = 1330

        Zespri_Fasi_Tipo = 1331

        Zespri_Fasi_Grower = 1332

        Num_Piante_Femmine = 1333

        Num_Piante_Maschi = 1334

        Impresa_Pubblica = 1335

        TipologiaDIInnestoTrapianto = 1336

        Potenziale_Metanigeno = 1339

        GiasAPP_Dati_Impresa = 1342
        GiasAPP_Dati_Centro = 1343

        Terreno_Inutilizzato = 1345
        Terreno_Degradato = 1346
        Low_ILUC = 1347
        Certificate_Number = 1348
        CodiceAgenzia = 1349
        CodiceZona = 1350 'riferito all'impianto (sviluppo per fruttagel)
        Fabbricato_Uso_da_Terzi = 1351
        Lavorazione = 1352
        Specifica = 1353
        Sau_Tot_Azienda_ha = 1354
        Ultima_Verifica_Ispettiva = 1355
        Agea_EffluentiZootecnici = 1356
        Agea_TipoDiSemina = 1357
        Certificazione_Taratura_Ugello = 1358
        Nr_Domanda_Aca = 1359
        Appezzamento_AGEA = 1360
        Impianto_AGEA = 1361
        Codice_Campagna = 1362
        Modalita_Liquidazione = 1363
        Origine_Prodotto = 1364
        Chiave_CUAA_Demetra = 1365

        CaloPesoDefault_daContatto = 4025
        CoeffCaloPesoContatto = 4026
    End Enum

    Public Enum enum_Fabbricato_Forno_Combustibile
        Gasolio = 1
        Metano = 2
        Gas = 3
        Cippato = 4
        Altro = 100
    End Enum

    Public Enum enum_Fabbricato_Forno_Fiamma
        Diretta = 1
        Indiretta = 2
        Recupero = 3
    End Enum

    Public Enum enum_Fabbricato_Forno_Cantiere
        Cassone = 1
        Telaini = 2
    End Enum

    Public Enum enum_Fabbricato_Forno_Umidificazione
        Acqua = 1
        Vapore = 2
    End Enum

    Public Enum enum_TipoOperazioneDB
        Lettura = 0
        Scrittura = 1
        Modifica = 2
        Cancellazione = 3
        Trasferimento = 4
        Copia = 10
    End Enum

    Public Enum enum_TipoPermesso
        LETTURA = 0
        DISABILITATO = 1
        LETTURA_SCRITTURA = 2
    End Enum

    Public Enum enum_TipoFormulato

        Tutti = 0
        Antiparassitari = 1
        Diserbanti = 2
        Fitoregolatori = 3
        Coadiuvanti = 4
        Concianti = 5
        Disseccanti = 6
        Geodisinfestanti = 7
        Antiparassitari_Concianti = 8
        Diserbanti_Disseccanti = 9
        Antiparassitari_Geodisinfestanti = 10
        Corroboranti_Fisiofarmaci = 11
        PostRaccolta = 12
        Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci = 13
        ConfusioneSessuale = 14
        DisorientamentoSessuale = 15
        ConfusioneDisorientamentoSessuale = 16
        InstallazioneTrappoleCattureMassa = 17

    End Enum

    Public Enum enum_CodificaDecodifica
        Codifica = 1
        Decodifica = 2
    End Enum

    Public Enum enum_Security_Operazione
        Lettura = 0
        Scrittura = 1
        Modifica = 2
        Cancellazione = 3
        Esecuzione = 4
        Stampa = 5
    End Enum

    Public Enum enum_VersioneAlberoImprese
        Albero_AnagraficaAziendale = 0
        Albero_ImpiantiVegetali = 1
        Albero_Prodotti = 2
        Albero_Magazzini = 3
        Albero_Stalle = 4
    End Enum

    Public Enum enum_TipoIngredienti
        IngredienteNonDefinito = 0
        AusiliareDiFabbricazione = 1
        AgricolturaBiologica = 2
        AgricolturaConvenzionale = 3
        NonAgricolo = 4
    End Enum

    Public Enum enum_ProdottoClasse
        NonDefinito = 0
        Convenzionale = 1
        InConversione = 2
        Biologica = 3
        BiologicaRegCEE = 4
        UtilizzabileInAgricolturaBio = 5
        DaUvaBiologica = 6
        DaUvaConversione = 7
    End Enum

    Public Enum enum_TipoAgricoltura
        Convenzionale = 1
        InConversione = 2
        Biologica = 3
    End Enum

    Public Enum enum_TipologiaGiorniSettimana
        Feriali = 1
        Festivi = 2
        Sabato = 3
    End Enum

    Public Enum enum_Agenda_Causali

        'Profili Utenti
        PROFILI_UTENTI = 900

        'Anagrafe
        IMPRESA = 1050
        STRUTTURA = 1100
        APPEZZAMENTO = 1200
        IMPIANTO = 1300
        CATASTO = 1500
        STALLA = 1600

        'Operazioni Colturali
        TRATTAMENTO = 2050
        RILIEVO_CAMPO = 2100
        RILIEVO_RACCOLTA = 2200
        LAVORAZIONE = 2300
        COSTI_ACCESSORI = 2600

        'Operazioni Zootecniche
        ANIMALE = 3001
        ANALISI_LATTE = 3100
        ALIMENTAZIONE = 3200
        LETTIERE = 3300
        MUNGITURA = 3400
        MACELLAZIONE = 3450
        RILIEVI_PRODUZIONI = 3500
        EVENTI = 3550
        VISUALIZZAZIONE_CONSISTENZE = 3600
        CARICO_CONSISTENZE = 3700
        SCARICO_CONSISTENZE = 3750
        PESATURA = 3800

        'Operazioni Contabili
        REGISTRAZIONI = 4000
        CONFERIMENTO = 4100
        CONFERIMENTO_DIVERSI = 4200

        'Cartografia
        CARTOGRAFIA = 5001

        'Contatti
        CONTATTO = 6001
        RAPPORTO_CONTABILE = 6100
        CORRISPETTIVO = 6200
        MOVIMENTO_CONTABILE = 6300
        MOVIMENTO_NON_CONTABILE = 6400
        STATISTICHE = 6500
        CLIENTE = 6600
        FORNITORE = 6610
        DIPENDENTE = 6620
        TERZISTA = 6630
        LEGALE = 6640
        IMPUTAZIONE_MANODOPERA = 6800  'Utilizzo di manodopera
        IMPUTAZIONE_TERZISTI = 6850  'Utilizzo dei Terzi

        'Magazzini
        MAGAZZINO = 7001
        VISUALIZZAZIONE_GIACENZE = 7100
        VISUALIZZAZIONE_INVESTIMENTO = 7200
        CARICO = 7300
        SCARICO = 7350
        TRASFERIMENTO = 7380
        IMPUTAZIONE_UTILIZZO_PRODOTTI = 7400
        PRODOTTI_AZIENDALI = 7800
        ACCETTAZIONE_BENI = 7900

        'Parco Macchine
        ANAGRAFE_PARCOMACCHINE = 8001
        IMPUTAZIONE_PARCOMACCHINE = 8100  'Utilizzo del Parco Macchine

        'Progetti
        PROGETTO = 9001
        Progetto_Produzione_Agricola = 9100
        PROGETTO_ZOOTECNICO = 9150
        PROGETTO_TECNICO = 9200
        CONTRATTO_COLTURALE = 9300
        CONTRATTO_CONFERIMENTO_COLTURALE_AZIENDA_AGRICOLA = 9301
        ORDINE = 9320
        PROGRAMMA_PRODUZIONE = 9325
        CENTROCOSTO = 9350
        IMPUTAZIONE_COSTISTANDARD = 9400

        'Linee Produzione
        LINEA_PRODUZIONE = 10001
        LINEA_VEGETALE = 10100
        LINEA_ANIMALE = 10200

    End Enum

    'Costanti per i tipi produzione
    Public Enum enum_TipoRisorsa
        PRODUZIONE_VEGETALE = 1
        PRODUZIONE_ANIMALE = 2
        PARCO_MACCHINE = 3
        ALTRE_MATERIE_AZIENDALI = 4
        MANGIMI = 5
        FARMACI = 6
        CONTATTI = 7
        CALI_DI_LAVORAZIONE = -1
        CONFEZIONI_PRODOTTO = 8
        SERVIZI = 9
    End Enum

    'Tipi di Mezzo
    Public Enum enum_TipoMezzo
        Indefinito = -1      'Mezzo per le Materie Prime
        Ettolitro = 0
        Ettaro = 1
        Ora = 2
        Mensile = 3
        Complessivo = 4
        Chilometro = 5
        Quintale = 6
    End Enum

    Public Enum enum_TipoModalitaDistribuzione
        Totale = 10
        Dose = 11
    End Enum

    'Variabili piani Semina
    Public Enum enum_PianiSemina_Variabili
        n_gg_Eccedenza = 1
        n_gg_Anticipo_Raccolta = 2
    End Enum

    'Tipi di Modalità Applicazione Sconto
    Public Enum enum_TipoRiscossione
        Insoluto = 0
        Parziale = 1
        Riscossa = 2
    End Enum

    'Costanti per il tipo di modalità di utilizzo della pagina FiltroMovContabili 
    'e delle form del lan: FormTrovaImpresa, FormDocumenti
    Public Enum enum_TipoModalita
        ModRicerca = 0
        ModPermessi = 1
        ModContatto = 2
        ModAssociazione = 3
        ModGerarchia = 4
        ModAnalisi_Costi = 5
        ModAgenda = 6
        ModAssociazione_Alternativa = 7
        ModStandard = 8
        ModAccise = 9
        ModSpesometro = 10
        ModLiquidazioneSoci = 11
        ModassociazioneOrdine_Progetto = 12
    End Enum

    'Movimentazione Di Magazzino
    Public Enum enum_TipoMovimentazioneMagazzino
        NonImpostato = -1
        MagazzinoMovimentato = 0
        MagazzinoNONMovimentato = 1 'Dettaglio che non comporta movimentazione di magazzino. Si verifica cioè una delle seguenti ipotesi:
        '1.Movimento che riguarda Servizi o Parco Macchine
        '2.Movimento di Fattura Allegata a Bolla di Accompagnamento già movimentata precedentemente
    End Enum

    Public Enum enum_Pendenza

        DocBolla = 0                'Movimento Allegato a Bolla di accompagnamento
        DocFattura = 1              'Movimento Allegato a Fattura
        MovPendente = 2             'Movimento Pendente
        MovESENTE = 3               'Movimento Esente
        MovGiustificato = 4         'Movimento Giustificato
        MovForzato = 5              'Movimento Non Giustificato e Forzato dall'Utente
        GiacenzeIniziali = 6        'Giacenze Iniziali
        Conferimento = 7            'Materia Prima/Lavorato in Conferimento
        AutoProduzione = 8          'Materia Prima/Lavorato Autoprodotto
        AutoConsumo = 9             'Materia Prima/Lavorato Autoconsumato
        Smaltimento = 10            'Materia Prima/Lavorato Smaltimento / perdita di lavorazione
        ZooConsistenzeIniziali = 11 'Consistenze Iniziali Zootecniche
        Trasferimento = 12          'Trasferimento Merci
        DocRicevuta = 13            'Movimento Allegato a Ricevuta Fiscale
        Resi_Acquisti = 14          'Scarico Giustificato da Resi su Acquisti
        Resi_Vendite = 15           'Carico Giustificato da Resi su Vendite

        'Fino a qui sono uguali tra LAN ed Online

        Furto = 16                  'scarico giustificato da furto
        ScaricoFuoriRegione = 17    'Scarico Fuori Regione
        ResoFornitore = 18          'Reso a Fornitore

        'Questi sono i valori presenti nel LAN:

        'DocAccettazione = 16       'Movimento Allegato a Buono Di Accettazione
        'DocDoco = 17               'Movimento Allegato a Doco
        'DocDAA = 18                'Movimento Allegato a D.A.A.
        DocFattura_ProForma = 19    'Movimento Allegato a Fattura ProForma
        DocCorrispettivo = 20       'Movimento Allegato a Corrispettivo
        DocOrdine = 21              'Movimento Allegato a Ordine Vendita
        'Furto = 22                 'Movimento Allegato a Furto                    --> non utilizzabile perchè l'online ha già il furto
        DocPreventivo_Vendita = 23  'Movimento Allegato a Preventivo Vendita
        DocMVV = 24                 'Movimento Allegato a MVV
        DocProcLiqS = 25            'Movimento Allegato a Liquidazione soci
        Alienazione_Pendenza = 26   'Movimento Giustificato da Alineazione
        Rottura = 27                'Movimento Giustificato da Rottura
        Altra_Pendenza = 99         'Altro --> Des_Lib Editabile

        ZooAcquistoAnimali = 31
        ZooNascita = 32

    End Enum

    Public Enum enum_GerarchiaImpresa_Elementi
        Gerarchia_Impresa = 1
        Gerarchia_Centro = 2
        Gerarchia_Campo = 3
        Gerarchia_Appezzamento = 4
        Gerarchia_Impianto = 5
    End Enum

    Public Enum enum_EntitaAlberoImprese
        NonDefinito = 0
        Impresa = 1
        Centro = 2
        Campo = 3
        Appezzamento = 4
        Impianto = 5
        Fabbricato = 6
        Particella = 7
        EntitaGrafica = 8
        Catasto = 9
        Vasca = 10
        Distinta = 11
        Contatto = 12
    End Enum

    Public Enum enum_Entita_Analisi
        NonDefinito = 0
        Impresa = 1
        Centro = 2
        Campo = 3
        Appezzamento = 4
        Impianto = 5
        Fabbricato = 6
        Particella = 7
        EntitaGrafica = 8
    End Enum

    Public Enum enum_Entita_PianoConcimazione
        NonDefinito = 0
        Impresa = 1
        Centro = 2
        Campo = 3
        Appezzamento = 4
        Impianto = 5
        Fabbricato = 6
        Particella = 7
        EntitaGrafica = 8
    End Enum

    Public Enum enum_CodificaPagBio

        Notifica = 1
        PAP_Vegetale = 2
        PAP_Zootecnico = 3

    End Enum

    Public Enum enum_CodificaPagPlanning

        PianificazioneVegetale = 1
        FiltroPianificazioneVegetale = 2
        ImportazioneDaAgrea = 3
        ImportazioneDaAnagrafeEmiliaRomagna = 4
        RaccoltaDatiAnagraficiAzienda = 5
        ImportazioneDaAvepa = 6
        ImportazioneDaAnagrafeBA = 7
        ImportazioneDaAgreaMassiva = 8

        Budget_Lista = 9

        Prenotazioni_Piante = 10

        PianificazioneVegetaleBS = 11

        ConfrontaPC = 12
        FiltraEdEsporta = 13

        Prenotazioni_Piante_BS_EditPrenotazione = 14

    End Enum

    Public Enum TipoImportazioneExcel

        ImportazioneDaAvepa = 1
        ImportazioneSisco = 2
        ImportazioneB1 = 3
        ImportazioneBrogliacinoSIAN = 4
        ImportazioneArpea = 5
        ImportazioneArtea = 6
        ImportazioneSIARL = 7
        ImportazioneAGREA = 8
        ImportazioneAGEA_RT = 9
        ImportazioneXmlPubblicoAgronica = 10
        ImportazioneXmlPubblicoAgronicaZespri = 11
        ImportazioneQDCAgronicaZespri = 12
        ImportazioneAGEA_UMBRIA = 13
        ImportazioneQDCAgronicaGenerico = 14
        ImportazioneUmbriaMassiva = 15
        ImportazioneAGEA_RT_Massiva = 16
        ImportazioneAGEA_GIS = 17
        ImportazioneARPEAB1 = 18
        ImportazioneMandatiAPOCONERPO = 19
        G2GReverse = 20
        ImportazioneAnagraficheZespri = 21
        ImportazioneAnagraficheCOPROB = 22
        ImportazioneUmaRichieste = 23
        ImportazioneUmaPraticheApprovate = 24
        ImportazioneUmaVendite = 25
        ImportazioneAVEPA_SHP = 26
        ImportazioneSiarMacchine = 27
        ImportazioneAGEA_APPSIAN = 28
        ImportazionePianoColturaleDaShapeFile = 29
        ImportazioneZoo_Test = 30
        ImportazioneParmaFrance = 31
        ImportazionePianoColturaleDaKOBO = 32
        ImportazioneFarmaciPrincipiAttivi = 33
        ImportazioniAggiornamentoMatricoleMadri = 34
        ImportazioniFileAnalisiZoo = 35
        ImportazioneFileFarmacie = 36
    End Enum

    Public Enum enum_CodificaPagPianoConcimazione

        Menu = 1
        InserimentoTestata = 2
        BilancioMultiAziendale = 3
        MenuBS = 4

        'ReportFosforoPotassio = 3

    End Enum

    'Public Enum enum_CodificaPagProfilazione

    '    GestioneProfilazione = 1
    '    Statistiche = 2

    'End Enum

    Public Enum enum_AnalisiTipo

        Analisi_Terreno = 1
        Analisi_Terreno_Fanghi = 2
        Analisi_Acqua = 3
        Analisi_Residui_Fitofarmaci_Vegetali = 4
        Analisi_Latte = 5
        Analisi_Vino = 6
        Analisi_Residui = 7
        Analisi_Fitofarmaci = 8
        Analisi_Organolettiche = 9
        Analisi_Varie = 10
        Analisi_Merceologiche = 11
        Analisi_Del_Sangue = 12

    End Enum

    Public Enum enum_AnalisiTipologia_Schema
        Piano_Concimazione = -1
        Area_Omogenea = -2
    End Enum

    '''Public Enum enum_PUATipo

    '''    PUA_Vegetale = 3
    '''    PUA = 2

    '''End Enum

    '''Public Enum enum_AuditTipo

    '''    Analisi_Condizionalita = 1
    '''    Analisi_Pua = 2
    '''    SicurezzaLavoro = 3
    '''    GlobalGAP = 4
    '''    COOP = 5

    '''End Enum

    '''Public Enum enum_AuditTipi

    '''    AuditTipi_Condizionalita = 1
    '''    AuditTipi_SicurezzaLavoro = 3
    '''    AuditTipi_GlobalGap = 4
    '''    AuditTipi_COOP = 5

    '''End Enum

    Public Enum enum_AuditPuaTipo

        Audit_Condizionalita = 1
        Audit_SicurezzaLavoro = 3
        Audit_GlobalGap = 4
        Audit_COOP = 5
        Audit_SchedaTecnicaTTI = 6
        Audit_SchedaControlliTTI = 7

        Audit_PannelloDiControllo = 8

        Audit_AnalisiDatiDiCura = 9
        Audit_SQNPI = 10
        Audit_SchedaControlliALP = 11
        Audit_BIO_COPROB = 12
        Audit_SQNPI_COPROB = 13
        Audit_Budwood_Projects = 14
        Audit_Convenzionale_Greenyard = 15
        Audit_Biologico_Greenyard = 16
        Audit_Filiera_Trasporti_COPROB = 17
        Audit_Enquete_certification_Hevea_brasiliensis = 18
        Audit_Azienda_Banca_Cambiano = 19
        Audit_BIO_FILENI = 20
        Audit_Fornitori_EUDR_INALCA = 21
        Audit_Controllo_DPI_DeMatteis = 22

        PUA = 2
        PUA_Vegetale = 3


    End Enum

    Public Enum enum_AuditTipi

        AuditTipi_Condizionalita = 1
        AuditTipi_SicurezzaLavoro = 3
        AuditTipi_GlobalGap = 4
        AuditTipi_CheckListCOOP = 5
        AuditTipi_SchedaTecnicaTTI = 6
        AuditTipi_SchedaControlliTTI = 7
        AuditTipi_PraticheEcologicheAPOT = 8
        AuditTipi_Formazione = 9
        AuditTipi_BIO_COPROB = 12
        AuditTipi_SQNPI_COPROB = 13
        AuditTipi_BIO_FILENI = 20

    End Enum

    Public Enum enum_AuditSezioni

        AuditSezioni_ElementiVerifica = 1
        AuditSezioni_Deroghe = 2
        AuditSezioni_Segnalazione = 3
        AuditSezioni_Portata = 4
        AuditSezioni_Gravita = 5
        AuditSezioni_Durata = 6
        AuditSezioni_PropostaCorrettiva = 7
        AuditSezioni_Note = 8
        AuditSezioni_CasiParticolari = 9
        AuditSezioni_InadempienzePortataMinore = 10
        AuditSezioni_Intenzionalita = 11
        AuditSezioni_Reiterazione = 12
        AuditSezioni_ImpegniRipristino = 13
        AuditSezioni_PortataGravitaDurata = 14


    End Enum

    Public Enum enum_PUARegolamenti

        PUA_ER_2001 = 1
        PUA_ER_2012 = 2
        PianoComcimazione = 3
        PianoConcimazione_2012 = 4
        CBPA_Umbria = 5
        PAN_2012_Veneto = 6
        PUA_ER_2016 = 7
        PianoConcimazione_2016 = 8
        PianoConcimazione_Trentino_2016 = 9
        PianoConcimazione_2017 = 11
        PUA_ER_2018 = 55

    End Enum

    Public Enum enum_PUAParametri

        Competenza_Inizio_Giorno = 1
        Competenza_Inizio_Mese = 2
        Competenza_Fine_Giorno = 3
        Competenza_Fine_Mese = 4
        Metodo_Semplificato = 5
        Metodo_Completo = 6
        Considera_CoefficienteTempo_Efficienza = 7
        Considera_CoefficienteTempo_CalcoloFabbisognoN_Nm = 8
        Considera_CoefficienteTempo_CalcoloFabbisognoN_Na = 9
        Considera_CoefficienteTempo_CalcoloFabbisognoN_Ns = 10
        Leggi_Nm_DaTabella = 11

    End Enum

    Public Enum enum_PUARegolamenti_Tipo

        PianoComcimazione = 1
        PUA = 2
        Pan = 3
        PianoNutrizionale = 4
        PianoNutrizionale_IBF = 5

    End Enum

    Public Enum enum_PagineProFitoSan

        HomePage = 0
        SchedaPFS = 1
        FiltroRicercaSchedaPFS = 2
        EsportaSostanze_DPI = 3
        Elenco_DPI = 4
    End Enum

    Public Enum enum_PagineAgronicaUMA

        UMA_Nuova_Richiesta = 1
        UMA_Nuova_Richiesta_Terzista = 2
        UMA_Elenco_Richieste = 3
        UMA_Approvazione_Richieste = 4
        UMA_Vendite_Carburante = 5
        UMA_Nuova_Rendicontazione = 6
        UMA_Nuova_Rendicontazione_Terzista = 7
        UMA_Elenco_Rendicontazioni = 8
        UMA_Approvazione_Rendicontazioni = 9
        UMA_Configurazione = 10
        UMA_Sintesi = 11
        UMA_Richiesta_Anticipo = 12
        UMA_Nuova_Richiesta_Cooperativa = 13
        UMA_Nuova_Rendicontazione_Cooperativa = 14
        UMA_Blocco_Particelle = 15
        UMA_Richiesta_Anticipo_Terzista = 16
        UMA_Richiesta_Anticipo_Cooperativa = 17
        UMA_Report_Controllo = 18
        UMA_Ricerca_Macrousi_Lavorazioni = 19
        UMA_Visibilita_Aziende = 20
    End Enum

    Public Enum enum_PaginePianiCampionamento
        Menu = 0
        AggiungiImpianti = 1
        ReportAnalisi = 2
        GestionePDC = 3
        CapitolatoCliente = 4
        GestionePDC_Zootecnico = 5
        GestioneAnalisi = 6
        GestioneLaboratori = 7
        GestioneAcquisti = 8
    End Enum

    Public Enum enum_PaginePianoConcimazione_2017

        MenuBS = 0

        PCB_Inserimento = 1
        PCB_InserimentoMultiplo = 2

        PUA_Dichiarazione_Effluenti = 3

        MenuBS_PUA = 4

        PUA_Piano_Distribuzione = 5

        FiltraEdEsporta = 6

        MenuBs_Piano_Nutrizionale = 7
        Piano_Nutrizionale = 8
        Piano_Nutrizionale_IBF = 9

    End Enum

    Public Enum enum_PagineAgronicaSincro
        MenuPrincipale = 0
        Sincro_Harvard = 1
        Sincro_Agrea_OP = 2
        Import_DDT_Seled = 3
        Importazione_Giacenze = 4
        MenuAnagrafiche = 5
        MenuMagazzino = 6
        ImportazioneDaAgrea = 7
        ImportazioneDaAnagrafeEmiliaRomagna = 8
        ImportazioneDaAvepa = 9
        ImportazioneDaAnagrafeBA = 10
        ImportazioneDaAgreaMassiva = 11
        Import_Anagrafiche_Seled = 12
        Import_Notifica_AgriBio = 13

        <Obsolete("Non più utilizzata")>
        SincroCatasto_ImportPC_AccessOP = 14

        Export_Sigpa = 15
        ImportaContatti_XLS2GIAS = 16
        ImportaImprese_XLS2GIAS = 17
        ImportazioneMassivaDaAnagrafeBA = 18
        Sincronizzatore_Apofruit = 19
        ImportazioneMagazzinoXMLPubblico = 20
        esportazioneSpesometro = 21
        Import_Anagrafiche_Agenda_Seled = 22
        ImportazioneDaAvepa_Viticolo = 23
        esportazioneProfis = 24
        Importatore_UNIFORMA = 25
        ImportazioneDaAnagrafeBA_Veneto = 26
        ImportazioneDaBrogliaccioSIAN = 27
        INDICODE_Edi_Euritmo = 28
        OPTA_ImportaRicevimenti_TracciaColli = 29 'non c'è una pagina, sono in agenda AnalisiRitiriTabacco.aspx, RintracciaLotto.aspx e nel sincro  Import_Macchine_Forni.aspx
        ImportazioneDaAvepaCatasto_Excel = 30
        FFConferimenti_AltriDB = 31
        EsportazioneBolleAccettazione2AltroCliente = 32
        EsportazioneConferimentiPomodoro_Agrea = 33
        EsportazioneGias2JDEdwards = 34
        ImportazioneAPOT = 35
        ImportazioneSISCO = 36
        ImportazioneCASALASCO = 37
        EsportazioneSQNPI = 38
        EsportazioneZESPRI = 39
        Import_Arpea = 40
        Import_Artea = 41
        ImportazionePC_Anteprima = 42
        Import_SIARL = 43
        ImportazioneCIO = 44
        EsportazioneConferimentiFF = 45
        Import_AGEA = 46
        EsportazioneAGEATxtCsv = 47
        Import_AGREA_CSV = 48
        EsportazioneSISCO = 49
        Export_EURESYS = 50
        Configurazione_Servizi = 51
        Import_Agea_Coordinamento_Massiva = 52
        Import_Agea_GIS = 53
        Import_Agrea_Grafico = 54
        Import_Bizerba = 55
        Import_JDE = 56
        EsportazioneGias2JDEdwards_BS = 57
        Sincronizzatore_ArcView = 58
        Import_dbwin_DateRaccolta = 59
        Import_Anagrafiche_Zespri = 60
        EsportazioneBolleAccettazione2AltroCliente_BS = 61
        ImportazioneCASALASCO_Raccolte = 62
        Piano_Prenotazione_Piante = 63
        Import_AnalisiCampioniMaselli = 64
        Codifica_ProdottiAPP = 65
        Import_Conferimenti_Pomodoro = 66
        Export_Conferimenti_Pomodoro = 67
        Import_Anagrafiche_COPROB = 68
        Esportazione_Trapianti_Agribologna = 69
        Codifica_Prodotti_Aziendali = 70
        Codifica_Specie_Vegetali = 71
        Codifica_Varieta = 72
        ImportazionePcgAvepa = 73
        Esportazione_Enogis = 74
        Esportazione_ARTEA = 75
        ImportPianoColturaleDaShapeFile = 76
        Esportazione_xFarm = 77
        SincronizzatoreBDN = 78
        ImportazioneParmaFrance = 79
        ImportPianoColturaleDaKOBO = 80
        ZooNogmo = 81
        ImportMatricoleMadri = 82
        ImportazioneUfficiZona = 83
        G2GReverse = 84
        ConsorzioBDN = 85
        VisualizzatoreSincro = 86
        Esportazione_Horta = 87
        ImportazioneFileAnalisiZoo = 88
        RegistrazioneMassivaBDN_Ingressi = 89
        RegistrazioneMassivaBDN_Uscite = 90
        SincronizzazioneMassimaStalleVetInfo = 91
        SincronizzazioneStalleBDN = 92
        SincronizzazioneMassivaStalleBDN = 93
        Esportazione_Horta_Orzo = 94
        InvioTrattamentiZooVetInfo = 95
        ImportazioneFarmacie = 96
        InvioTrattamentiMassivoZooVetInfo = 97
    End Enum

    Public Enum enum_PagineAgronicaDomandaIrrigua
        MenuPrincipale = 0
        NuovaDomanda = 1
        ElencoDomande = 2
        LettureContatoreAziendale = 3
        CalcoloTariffazione = 400

        ' Pagine operazione portate da AgroAgenda_2010.
        ' I valori sono identici a quelli di enum_PagineAgenda_2010 per mantenere la compatibilità
        ' col payload che Angular invia (per le operazioni continua a usare i codici "storici").
        Pagina_Installazione_Trapppole = 4
        Pagina_Reinnesco_Trappole = 6
        Pagina_RilievoPiogge = 9
        Pagina_Distribuzione_Insetti = 19
        Pagina_Operazione_Di_Cura = 21
        Pagina_Raccolta = 22
        Pagina_Trattamenti_B = 31
        Pagina_TrattamentiPostRaccolta = 94
        Pagina_RilieviBS = 95
    End Enum

    Public Enum enum_Id_Servizio
        'I valori si trovano anche in CostantiPersonalizzate.Id_Servizio_*
        Nessuno = 0
        Agronica = 1
        GiasPcGiasPro = 2
        NetFruit = 3
        SitoAgronica_2003 = 4
        GiasOnline = 5
        ManutenzioneTabelle = 6
        GiasOnlineCodex = 7
        GiasLAN = 8
        SitoAgronicaOld = 9
        AgronicaSementi = 11
        Profitosan = 20
        ProfitosanTunnel = 21
        BancheDatiEsterni = 30
        PianoConcimazione = 31
        AgroGSB = 40
        GiasAPP = 100
    End Enum

    Public Enum enum_Tipi_Servizi_Background

        'Gli enum sono stati copiati da enum_PagineAgronicaSincro
        'per ora attivato solo ImportazioneMassivaDaAnagrafeBA (al 20/11/2013
        'Attenzione a non confonderli con enum_PagineAgronicaSincro


        ' '''MenuPrincipale = 0
        'Sincro_Harvard = 1
        'Sincro_Agrea_OP = 2
        'Import_DDT_Seled = 3
        'Importazione_Giacenze = 4
        ' '''MenuAnagrafiche = 5
        ' '''MenuMagazzino = 6
        'ImportazioneDaAgrea = 7
        'ImportazioneDaAnagrafeEmiliaRomagna = 8
        'ImportazioneDaAvepa = 9
        'ImportazioneDaAnagrafeBA = 10
        'ImportazioneDaAgreaMassiva = 11
        'Import_Anagrafiche_Seled = 12
        'Import_Notifica_AgriBio = 13
        'SincroCatasto_ImportPC_AccessOP = 14
        'Export_Sigpa = 15
        'ImportaContatti_XLS2GIAS = 16
        'ImportaImprese_XLS2GIAS = 17
        ImportazioneMassivaDaAnagrafeBA = 18
        Importatore_UNIFORMA = 25
        Sincronizzatore_Apofruit = 19
        Sincronizzatore_Web = 26


        'attivi per test
        ImportazioneMagazzinoXMLPubblico = 20
        esportazioneSpesometro = 21

        ImportazioneDatiStazioniMeteo_Metos = 22

        Importazione_Risposta_Analisi_Laboratorio = 23

        OPTA_ImportaRicevimenti_TracciaColli = 29

        Indicode_EDI_Euritmo = 30
        Importazione_Risposta_Analisi_Laboratorio_EmailAllegati = 31

        ImportazioneMassivaDaAnagrafeAVEPA = 32

        Indicode_EDI_BMI_Esportazione = 33

        Teleregistri = 34

        iMotion = 35

        ImportazioneCalibri = 36

        SQNPI = 37

        ImportazioneFascicoli = 38

        Indicode_EDI_BMI_Importazione = 39
        Indicode_EDI_BMI_CMag = 40
        Indicode_EDI_BMI_SMag = 41

        GestoreFascicoli = 42

        InvioMail = 43

        ImportazioneDatiStazioniMeteo_WiNet = 44

        InvioSMS = 45

        ImportazioneDatiStazioniMeteo_CoDiMa = 46

        AccodamentoEmailInterferenzeXConferma = 47

        Interscambio_Anagrafica_Importazione = 48
        Interscambio_Prodotti_Importazione = 49
        Interscambio_Progetti_Esportazione = 50
        Interscambio_OP_Esportazione = 51
        Interscambio_Utilizzo_Ore_Esportazione = 52
        Interscambio_Consumo_Materiali_Esportazione = 53

        'ImportazioneRicetteDaInterscambioApp = 54 DT: non più usato, confluito nel 113 SincronizzazioneDatiApp

        CreazioneAutomaticaMateriePrimeVeg = 55

        Interscambio_OP_Chiuso_Esportazione = 56

        Interscambio_SuperTask_Importazione = 57
        Interscambio_SuperTask_Esportazione = 58

        ImportazioneDatiStazioniMeteo_AgriculturalSupport = 59

        DSS_ModelliPrevisionali_Elaborazione = 60

        ImportatoreFascicoli_AGEA_UMBRIA = 61

        EFattura_Generazione_XML = 62
        EFattura_Invio_XML_Attivi = 63
        EFattura_Verifica_Esiti = 64
        EFattura_Ricevi_XML_Passivi = 65

        EAccise_Spedizione_DAA_Creati = 70

        Sentinel2Sincro = 66
        Sentinel2Elaborazioni = 67

        Interscambio_Anagrafica_Esportazione = 68
        Interscambio_Prodotti_Esportazione = 69
        Interscambio_Invoice_Esportazione = 71

        Interscambio_Euresys_Esportazione = 72

        Servizio_ImportExport_Riunite = 73

        ImportazioneDatiStazioniMeteo_NT_WiNet = 74
        ImportazioneDatiStazioniMeteo_NT_AS = 75
        ImportazioneDatiStazioniMeteo_NT_NetSens = 76

        Aggiornamento_CDG_DW = 77

        Creazione_Dati_BI_Aboca = 78

        Importazione_RMA_UE = 79

        Importazione_RMA_Homologa = 80

        Importazione_COOPSole = 81
        Importazione_Bizerba = 82

        DSS_PreElaborazioneImpianti = 83

        ImportazioneAnagraficheJDE = 84

        ImportazioneDatiStazioniMeteo_NT_Pessl = 85

        ImportazioneDatiStazioniMeteo_NT_GreenPlanet = 86

        myticoAllineamentoTimbrature = 87

        SincronizzazioneDatiGisDaServerAgronica = 88

        ImportazioneDatiStazioniMeteo_NT_A2ASmartCity = 89

        ImportazioneDatiStazioniMeteo_NT_DigiFarm = 90

        Importazione_RMA_Zespri = 91

        Elaborazione_Indicatori_DSS = 92

        ImportazioneDatiStazioniMeteo_NT_CoDiMa = 93

        JDeere_Download_ShapeFile_produzione = 94

        SincronizzazionePerformaTFS = 95

        ImportAnagraficheCOPROB = 96

        ImportConferimentiPomodoro = 97

        Integrazione_Macchine_Lavorazione = 98

        '99 = test ... non usare

        ImportConferimentiAgribologna = 100

        ImportazioneMassivaPlanningAGEA = 101
        ImportQDC_Documentale = 102

        Interscambio_ExportMovimenti = 103

        ImportazioneDatiStazioniMeteo_NT_Agrismart = 104

        Export_SDS = 105

        ImportazioneDatiStazioniMeteo_NT_Horta = 106

        ImportazioneTaskData_SDF = 107

        Interscambio_FatturePdf_Importazione = 108

        Interscambio_CdcWbs_Importazione = 109

        Interscambio_VerificaStatoPagamentoFatture = 110

        Indicode_EDI_BMI_Importazione_RIPROCESSA = 111

        ImportazioneDatiStazioniMeteo_NT_RadarMeteoFTP = 112

        SincronizzazioneDatiApp = 113

        ImportazioneDatiStazioniMeteo_NT_Acmotec = 114

        AggiornaChecklistCOPROB = 115

        ImportazioneDatiStazioniMeteo_NT_Hypermeteo = 116
        StoricizzazioneMeteo_NT = 117
        ImportazioneDatiStazioniMeteo_NT_HypermeteoPrevisionale = 118
        SincronizzatoreBDN = 119
        EsportazioneWMSOnPlant = 120

        EsportazioneENI_SAP = 121

        'HubIoT - SmartTractors
        HubIot_InvioRicette = 122
        HubIot_RecuperaDatiProduzione = 123
        'HubIoT - SmartTractors

        ImportazioneCAI_Agenzie = 124
        ImportazioneCAI_Giacenze = 125
        ImportazioneCAI_Aziende = 126
        ProgrammazioneNotifichePush = 127
        InvioNotifichePush = 128
        ImportazioneCAI_Acquisti_Imprese = 129
        EsportazioneCAI_Aziende = 130
        CalcoloGHG = 131
        SincronizzazioneEntitaInattive = 132
        SincronizzazioneEsecuzioniConfigurazioneProiezione = 133
        SincroBDN_Mail = 134
        EsecuzioneAlgoritmiProiezione = 135
        AccodamentoServizioInScandenza = 136

        ImportazioneDati_rete_acqua_Acmotec = 137
        Cai_Invio_Email = 138

        Demetra_Abaco_Import_Creazione = 139

        Sincro_Timesheet_Calendar = 140

        CDG_Motorino_Visite = 141
        Sincronizza_Utenti_Visibilita = 142

        Demetra_Abaco_Import_Aggiornamento = 143
        GestioneTraduzione = 144

        EsportazioneMaterialeVivaisticoOrdini = 145
        EsportazioneMaterialeVivaisticoOrdiniVerificaStato = 146

        Esportazione_Visite_Analisi_Commerciali = 147

        EsecuzioneAlgoritmiSenzaConfigurazione = 148

        Esportazione_To_Demetra_Fabbricati = 149

        Interscambio_BIOrogel = 150
        Importazione_Risposta_Analisi_Zootecnia = 151

        Esportazione_To_Demetra_AnalisiTerreno = 152
        Esportazione_To_Demetra_Attivita = 153
        SuperTask_Esportazione_To_Demetra = 154

        Esportazione_To_Demetra_Macchine = 155

        Esportazione_To_Demetra_Contatti = 156
        Gias_Duplicate_Resx = 157
        Esportazione_MovimentiMag_SistemiEsterni = 158

        Esportazione_LogInterscambio_ElasticSearch = 159
        Integrazione_Performa_Zucchetti = 160

        Servizio_IsAlive = 161

        Aggiornamento_ISCC_Conferimenti = 162
        Creazione_ConferimentiDaRaccolte = 163

        ElaborazioneComplianceISCC_Async = 164
        Import_DPI_Abaco = 165

        PuliziaTabelleLog = 166

        ImportazionePendenzeAGEA = 167
        ImportGAP_Documentale = 168

        PuliziaTabelleAPP = 169

        ImportazioneAcquistiFarmacie = 170

        ExportStazioniMeteoInfragri = 171

        VerificaConformitaMassiva = 172
        Esportazione_To_Demetra_Attivita_V2 = 173

        Esportazione_To_Demetra_Squadre = 174
    End Enum

    Public Enum enum_Manager_TipiDatiSQLVariant
        system_integer = 1
        system_string = 2
        system_dateTime = 3
        system_Real = 4
    End Enum

    Public Enum enum_Stato_Guida_Servizi_Background
        Disabilitato = -1
        Spento = 0
        Attivo = 1
        SempreAttivo = 2
        InEsecuzione = 3
        Errore = 99
    End Enum

    Public Enum enum_Sentinel2_cfgFlagStatoElaborazioni

        Tutto = -1
        NonElaborato = 0
        Elaborato = 1
        ElaboratoInDataSuccessiva = 2

    End Enum

    Public Enum enum_Sentinel2_cfgFlagAttivo

        Attivo = 0
        Disattivato = -1

    End Enum

    Public Enum enum_Sentinel2_cfgFlagTipoVisibilita

        Attivo = 1
        Disattivato = 0

    End Enum

    Public Enum enum_PagineAgronicaMeteo
        Menu = 0
        ImportaBdfCCCI = 1
    End Enum

    Public Enum enum_MeteoStatoElaborazioneDatoOriginale
        NuoviDati = 0
        ElaboratoDefinitivo = 1
        RichiestaNuovaElaborazione = 2
        NuoviDati_inCorso = 3
        RichiestaNuovaElaborazione_inCorso = 4
    End Enum

    Public Enum enum_Meteo_AggregazioneDatiTipo
        NessunaAggregazione = 0
        Oraria = 1
        Giornaliera = 2
    End Enum

    Public Enum enum_PagineAnalisi_2010
        Pagina_Analisi = 1
        Pagina_Gestione_Laboratori = 2
        Stampa_Analisi_Fitofarmaci = 3
        Gestione_Analisi_NEW = 4
        Gestione_Laboratori_NEW = 5
        Visualizza_Analisi_Zoo = 6
    End Enum

    Public Enum enum_PagineProfilazione_2010
        Pagina_Home = 1
        Pagina_Default_Specie_Varieta = 2
        Pagina_Statistiche = 3
        Pagina_Utenti_Lista = 4
        Pagina_Utenti_Visibilita = 5
        Pagina_Servizi_Lista = 6
        Pagina_Utenti_Impostazioni = 7
        Pagina_Utenti_Edit = 8
        Pagina_Privacy = 9
        Pagina_PassaggioDiStato = 10
        Pagina_Visibilita_Aziende = 11
    End Enum

    Public Enum enum_PagineGiasOnline

        'gestire le pagine dentro alla funzione
        'PaginaAspx_from_TipoEnumPagina

        MenuContab = 0
        FiltroMovContabili = 1
        PianoConti = 2
        DocTrasporto = 3
        Fattura = 4
        AltriCostiRicavi = 5
        LiquidazioneIVA = 6
        PrimaNota = 7
        FormProdotto = 8
        MagazziniGiacenze_Info = 9
        MagazziniMovimenti_Info = 10
        StalleVariazioniConsistenze_Info = 11
        DDT = 12

        AlberoImprese = 30
        MenuAgenda = 31
        MenuPrincipale = 32
        MenuStampe = 33
        MenuArchivi = 34

        GestioneDisciplinari_Verifica_ApportiMassimi = 35
        GestioneDisciplinari_Consultazione_Disciplinari = 36
        GestioneDisciplinari_Verifica_Disciplinare = 37

        Agenda_CopiaIncolla = 38

        FiltroImpresa_new4 = 39
        Agenda_Semina = 40
        Gestione_Pianificazione_Gestione_Pianificazione = 41

        PianiSemina = 42
        Operazione_Delete = 43

        Agenda_Raccolta = 44

        CostiAccessori_Eredita = 45
        FiltroImpresa_new4_E_CostiAccessori_Eredita = 46

        MenuMagazzini = 47

        Utenti_Menu = 48
        Utenti_Edit = 49
        Utenti_Profili = 50

        FiltroImpresa_new4_Utenti_Visibilita = 51

        FiltroImpresa_new4_Ricetta = 52

        FiltroImpresa_new4_Servizi = 53

        Utenti_Impostazioni = 54

        MenuAnalisiCosti = 55

        Nuovo_Contatto = 56

        PaginaGrafica = 57

        MenuSupportoDecisioni = 58

        FiltroImpresa_new4_PianoConcimazione = 59

        FiltrinoImprese2010 = 60

        MenuAnalisiLaboratori = 61

        Menu_PianiCampionamento = 62

        Contatto_MODIFICA = 63
        Contatto_MENU = 64
        Contatti_Home = 65

        FiltroImpresa_new4_E_Multiimpresa = 66

        Menu_principale = 67

        GestioneRisorse = 68

        Codifica_SpecieVegetali = 69
        Codifica_Cultivar = 70
        Codifica_ProdottiAziendali = 71

        Filtro_RiepilogoUtilizzoSuperfici = 72
        Filtro_Manodopera = 73
        Filtro_ParcoMacchine = 74
        StalleConsistenze_Info = 75
        FiltroImpresa_new4_menustampeagenda = 76

        FiltroImpresa_new4_ExportSigpa = 77

        ParcoMacchine_Edit = 78

        MenuCartellaAziendale = 79

        MenuAnagrafica = 80

        FiltroImpresa_new4_AnalisiCosti = 81

        GestioneRisorse_Edit = 82
        CodificaOperazioni = 83
        CorpiEstranei = 84

        MenuAnagraficaProdotti = 85

    End Enum

    Public Enum enum_PagineGiasOnline_2010
        Menu = 1
        Cartografia = 2
        RegistazioneSmart = 3
        Nuova_Scandenza = 4
        Contatto = 5
        ElencoAnagrafiche_x_Contatti = 6
        concimazione_lite = 7
        trattamenti_lite = 8
        giassmart_punti = 9
        Step_1 = 10
    End Enum

    Public Enum enum_Area_Visibilita
        UMA = 0
    End Enum

    Public Enum enum_TipoUtilizzoFiltroneInG2G
        NessunUtilizzo = 0
        ApplicaFiltroneUsaRisultati = 1
    End Enum

    Public Enum enum_TipoSelect_FiltroneSuperNova

        Base = 0
        ImpreseAlbero = 1
        Imprese = 2
        Imprese_Visibilita_Appoggio = 13
        CentriAziendali = 3
        CentriAziendali_Visibilita_Appoggio = 14
        Appezzamenti = 4
        Impianti = 5
        Movimenti = 6
        Contatti = 7
        Campi = 8
        Imprese_Codici = 9
        Esercizi = 10
        AppezzamentiRipartoCatasto = 11
        Fabbricati = 12
        Imprese_APP = 15
        PianoColturale = 16
        PianoColturaleBudget = 17
    End Enum

    Public Enum enum_TipoMacchineAlimentazione

        NonDefinita = 0
        Benzina = 1
        Gasolio = 2
        Metano = 3
        Gpl = 4
        Elettricita = 5
        Olio_Combustibile = 6
        Petrolio = 7

    End Enum

    Public Enum enum_TipoCentro
        Sede_Legale = 101
        Sede_Aziendale = 102
        Stabilimento = 103
    End Enum

    Public Enum enum_AgronicaCore_FlagVisibilita
        Visibilita_Solo_NON_Cancellati = 1 'default
        Visibilita_Solo_Cancellati = 2
        Visibilita_Tutti = 3
    End Enum

    Public Enum enum_TipoMovimentoContabile
        'vedi:
        'CaricaCombo_TipoCausale
        'TipoCausale_from_LavCod_e_Tipo
        MovEconomiciPagamenti = 0
        MovEconomici = 1
        Pagamenti = 2
        MovFinanziari = 3
        BolleDDT = 4
    End Enum

    Public Enum enum_PagamentiCausali
        NonImpostato = 0
        RiBa = 1
        Bonifico = 2
        Contanti = 3
        RimessaDiretta = 4
        Assegno = 5
        CartaCredito = 6
        Bancomat = 7
        RID = 8
        MAV = 9
        Contrassegno = 10
    End Enum

    Public Enum enum_Pagamento
        NonDefinito = -1
        Previsto = 0
        Avvenuto = 1
    End Enum

    Public Enum enum_Cod_Regolamento

        Non_Specificato = 0
        Regolamento_Nessuno = 1
        Regolamento_bio = 4
        Regolamento_ProduzioneIntegrata = 10

    End Enum

    Public Enum enum_Stato_Impianto

        Impianto_Allevamento = 101
        Impianto_Produzione = 102
        Anno_Impianto = 103
        Allevamento_arboreo_1_anno = 104
        Allevamento_arboreo_2_piu_anni = 105
        Allevamento_olivo_3_anno = 106
        Allevamento_olivo_4_anno = 107
        Allevamento_olivo_5_anno = 108
        anno_1 = 109
        anno_2 = 110
        anno_3_piu_anni = 111


    End Enum

    Public Enum enum_AnalisiParametri

        AnalisiParametri_NonDefinito = 1
        AnalisiParametri_pH = 2
        AnalisiParametri_Sabbia = 3
        AnalisiParametri_Limo = 4
        AnalisiParametri_Argilla = 5
        AnalisiParametri_CaCO3 = 6
        AnalisiParametri_CaCO3_Attivo = 7
        AnalisiParametri_CSC = 8
        AnalisiParametri_SostanzaOrganica = 9
        AnalisiParametri_Ntot = 12
        AnalisiParametri_Norg = 98
        'AnalisiParametri_P2O5 = 16
        'AnalisiParametri_K2O = 19
        AnalisiParametri_P2O5_assimilabile = 18
        AnalisiParametri_K2O_assimilabile = 20
        AnalisiParametri_Mg_assimilabile = 23
        AnalisiParametri_K2O_scambiabile = 85

        AnalisiParametri_CarbonatiTotali = 84        ' è il AnalisiParametri_CaCO3, stessa cosa

        AnalisiParametri_RapportoCN = 11

        AnalisiParametri_K_scambiabile = 21
        AnalisiParametri_P_assimilabile = 17

        AnalisiParametri_HGB = 233
        AnalisiParametri_MCV = 234
        AnalisiParametri_Ematocr = 235
        AnalisiParametri_RDW_SD = 237
        AnalisiParametri_FePl = 238

    End Enum

    Public Enum enum_TipoPianificazione

        Pianificazione_Annuale = 0 'utilizzata da Coldiretti, Agribologna, Agrisfera, Terremerse, Cab, clienti con import agrea
        Pianificazione_Mensile = 1 'utilizzata da Agrisfera
        Pianificazione_Quindicinale = 2 'utilizzata da Agribologna
        Pianificazione_DaNotificaBio = 3 'utilizzata da Coldiretti
        Pianificazione_Budget = 4 'utilizzato da Terremerse
        Pianificazione_PrecisionFarming = 10 'utilizzata da Agrisfera
        Pianificazione_PrenotazionePiante = 11 'utilizzata da Prenotazione Piante Apofruit
        Pianificazione_OrdinePiante = 12 'utilizzata da Prenotazione Piante Apofruit
        Pianificazione_RichiestaPiante = 13 'Impianti per richiesta piante Sincro nuovo

    End Enum

    Public Enum enum_Programmazione_EntitaXProgrammazione_EntitaTipoRelazione
        ProgrammazionePiante = 1
    End Enum

    Public Enum enum_TipoArchivioCliente

        Archivio_Agrea = 1
        Archivio_Uniforma = 2

    End Enum

    Public Enum enum_Zone

        B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc = -47
        A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE = -46

        Pianura = -31
        Montagna = -30
        Collina = -29
        ZVN = -17
        ZPS = -3
        SIC = -2

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Azienda

        Rag_Soc = 0
        Piva = 1
        Codice_Fiscale = 2
        CUAA = 3
        Indirizzo = 4
        Frazione = 5
        CAP = 6
        Provincia = 7
        Comune = 8
        Telefono = 9
        Cellulare = 10
        Fax = 11
        Email = 12
        Note = 13

    End Enum

    Public Enum enum_IndirizzoTipo
        'Persona giuridica
        'Codice            Tipo indirizzo
        '1	                Sede operativa
        '101	            Sede legale
        '102	            Sede aziendale
        '103	            Stabilimento
        '201                Stabile Organizzazione
        SedeOperativa = 1
        SedeLegale = 101
        SedeAziendale = 102
        Stabilimento = 103
        StabileOrganizzazione = 201

        'Persona fisica
        'Codice            Tipo indirizzo
        '2	                Domicilio
        '3	                Residenza
        '4	                Residenza Estiva
        '5	                Luogo di nascita
        Domicilio = 2
        Residenza = 3
        ResidenzaEstiva = 4
        LuogoNascita = 5

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Rappr_Legale

        Cognome = 0
        Nome = 1
        Codice_Fiscale = 2
        Sesso = 3
        Data_Nascita = 4
        Provincia_Nas = 5
        Comune_Nas = 6
        Indirizzo_Res = 7
        Frazione_Res = 8
        Cap_Res = 9
        Provincia_Res = 10
        Comune_Res = 11
        Indirizzo_Dom = 12
        Frazione_Dom = 13
        Cap_Dom = 14
        Provincia_Dom = 15
        Comune_Dom = 16
        Telefono = 17
        Cellulare = 18
        Fax = 19
        Email = 20

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Macchine

        pippo = 0

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_PianoColturale

        pippo = 0

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Fabbricati

        pippo = 0

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Allevamenti

        pippo = 0

    End Enum

    Public Enum enum_DatiAnagraficiAzienda_Catasto

        pippo = 0

    End Enum

    Public Enum enum_DatiAnagrafici_CodiciAnagrafe

        Azienda = 5000
        Rappr_Legale = 5001
        Macchine = 5002
        PianoColturale = 5003
        Fabbricati = 5004
        Allevamenti = 5005
        Catasto = 5006

    End Enum

    Public Enum enum_Eleggibilita_Particelle

        Seminativo = 1
        Arboree_Specializzate = 3
        Manufatti = 5
        Pascolo_Polifita = 8
        Serre_Fisse = 9
        Bosco = 10
        Acque = 11
        Pascolo_Magro = 12
        Uso_Non_Agricolo = 17
        Pascolo_Magro_50perc = 18

    End Enum

    Public Enum enum_Server_Economico_Macrovoci

        SPESE_GENERALI = 1
        Interessi_capitale_anticipazione = 2
        Prezzo_uso_capitale_fondiario = 3
        Semina = 4
        Concimazione = 5
        Irrigazioni = 6
        Raccolta = 7
        Assicurazioni = 8
        IMPOSTE_TASSE_CONTRIBUTI_CONSORTILI = 9
        Trattamenti_Fitosanitari = 10
        Lavorazioni_Terreno_E_Sovescio = 11


    End Enum

    Public Enum enum_TipoRegistroCaricoScaricoPomodoro

        Contrattato = 0
        NonContrattato = 1

    End Enum

    Public Enum enum_DanniRaccolta

        Marcio = 1
        Verde = 2
        Inerti = 3
        FruttiSchiacciati = 4
        FruttiImmaturi = 5
        FruttiScottati = 6
        FruttiLesionati = 7

    End Enum

    Public Enum enum_CodiceGIAS_Clienti

        NonDefinito = 0
        Gentili = 1
        Agronica = 2
        CerealiRomagna = 507
        Agrisfera = 519
        Spreafico = 527
        AgriBologna = 553
        ApoConerpo = 557
        Italfrutta = 558
        CabBagnacavallo = 560
        CabCampiano = 561
        CabCervia = 562
        CabFusignano = 563
        CabMassari = 564
        CabTerra = 565
        Terremerse = 588
        Costea = 598
        FattoriaIlMonte = 600
        CaBruciata = 616
        FerrariFranco = 626
        Tomasini = 633
        Fruttagel = 636
        ChieregatoProtti = 637
        Altavita = 643
        IlPozzo = 644
        ColdirettiNazionale = 651
        FattoriaMonticinoRosso = 652
        IlPratello = 653
        VillaPapiano = 655
        IlRegnoDelMarrone = 661
        SocAgrCasamento = 662
        FreddiGianni = 663
        FiorentinaDiSopra = 664
        AzAgrMura = 665
        BaroneLuigi = 666
        MasiLuigi = 667
        MagaBetaTester = 671
        PodereVecciano = 673
        Guarini = 674
        Fiammetta = 678
        Bonzara = 686
        AgriSanPolo = 691
        GranfruttaZani = 692
        TenutaCasali = 698
        Apofruit = 571
        Agrisol = 700
        IlMioCasale = 701
        Zuffa = 729
        Cavaliera = 730
        Martelli = 731
        VillaVenti = 732
        Fisiomed = 734
        Pedroni = 741
        PoderePalazzo = 742
        Folicello = 743
        TenutaPalazzona = 744
        Bordona = 746
        Ancarani = 747
        SoleBio = 752
        Molinelli = 753
        Valmorri = 754
        TerreNaldi = 756
        BasagliaRenzo = 757
        BoniLuigi = 759
        Magagnini = 760
        Pelliconi = 761
        Valpiani = 762
        BoschiMauro = 763
        TenutaPennita = 765
        SantaLucia = 766
        Diamanti = 767
        Cunial = 768
        Montegrande = 769
        TenutaCroci = 770
        TorreDelPoggio = 771
        PodereAngelo = 772
        Negrella = 774
        TenutaFrassineto = 775
        CasaZanni = 777
        Conde = 778
        TomEGio = 779
        Crocizia = 780
        BiniDenny = 781
        SapaBrighi = 782
        TenutaMara = 783
        Zucchi = 785
        BabbiniLoris = 786
        Giovannini = 788
        TenutaGodenza = 789
        TenutaColleAngeli = 790
        CaseMori = 791
        Balugani = 794
        Valdoca = 795
        Orogel = 796
        BonificaLamone = 797
        Pastocchi = 798
        Tiraferri = 799
        SanRoccoTonni = 800
        PodereBianchi = 801
        LeColture = 802
        CantinaSoave = 804
        CaPruccolo = 805
        Garuti = 806
        TenutaBarbon = 808
        Marsuret = 810
        CaDiSopra = 812
        FragolaDeBosc = 813
        PaoloBea = 814
        AlCanevon = 815
        ValmoriEsmeraldo = 816
        TenutaFolesano = 818
        LaSpinosa = 819
        CollinaDeiPoeti = 820
        Randi = 822
        AnelliMatteo = 823
        LeCalastre = 824
        Baldetti = 825
        AnelliFilippo = 826
        ColVetoraz = 827
        CantinaForliPredappio = 828
        Matassoni = 829
        Colombarda = 830
        ConsAgrRavenna = 831
        ProgettoNatura = 832
        CTR = 833
        LaPiana = 835
        Francesconi = 836
        FruitModena = 846
        Fiorini = 847
        Bagnolini = 838
        Qualitoscana = 845
        LaRizzola = 848
        Ruggeri = 851
        ConsorzioConeglianoValdobbiadeneProsecco = 852
        OPTA = 853
        PoggioRegini = 854
        TenutaNeri = 855
        LaCastellana = 856
        Gandolfi = 857
        LaBuonaRomagna = 861
        DeRiz = 862
        MartiniFrancesco = 864
        CampanacciSanMamante = 866
        AzAgrAndreola = 867
        FattorieGiannozzi = 868
        OrtofruttaGrosseto = 869
        GalloNero = 872
        Monticino = 873
        Genagricola = 875
        PoderiDami = 876
        Cazzola = 878
        Quarticello = 879
        Istine = 880
        CompagniaPrimizie = 881
        RicciGuardigli = 882
        Eliso = 883
        Tomisa = 885
        Gallese = 886
        CoFruTa = 887
        Trequanda = 895
        OminaRomana = 897
        Scandolera = 902
        DeFaveri = 906
        Copag = 908
        Apot = 909
        CaDeiZago = 910
        PiazzaRoberto = 911
        Tomasi = 912
        MaioranoRaffaele = 914
        Trombin = 915
        Grains = 920
        MaioranoFormaggio = 923
        Agrintesa = 924
        BeleCasel = 929
        Lorenzato = 931
        TenutaGalvana = 934
        Bartolini = 935
        Gazzola = 938
        Zanasi = 942
        SBTF = 946
        OPCOP = 953
        Sandrin = 965
        Aboca = 992
        REFOOD = 1003
        Minguzzi = 1019
        FruttaC2 = 1035
        CoopSole = 1059
        'lasciarli in ordine numerico

    End Enum

    Public Enum enum_CodiceAnagrafe_Clienti

        GranfruttaZani = 2029
        Apofruit = 2013
        Valdoca = 2036
        FruitModena = 2042
        CantinaSoave = 2045
        OPTA = 2046
        Italfrutta = 2005
        Ruggeri = 2044
        ApoConerpo = 2004
        CoFruTa = 2051
        Costea = 2016
        Casalasco = 2064
        SBTF = 2065
        Ferrarini = 2066
        AgriBologna = 2069
        Demetra = 2201

    End Enum

    Public Enum enum_TipoImpresaGerarchia
        'TIPO IMPRESA GERARCHIA:
        'TipoImpresaGerarchia = 1 ----> IMPRESA
        'TipoImpresaGerarchia = 2 ----> COOPERATIVA
        'TipoImpresaGerarchia = 3 ----> CONSORZIO
        'TipoImpresaGerarchia = 4 ----> OP
        Impresa = 1
        Cooperativa = 2
        Consorzio = 3
        OP = 4
        DittaIndividuale = 5
    End Enum

    Public Enum enum_Rilevamento_Corpi_Estranei
        Aereoseparatore = 1
        Cernitrice_Ottica = 2
        Cernita_Manuale = 3
    End Enum

    Public Enum enum_Pericolosita_Corpi_Estranei
        Medio_Bassa = 0
        Alta = 1
    End Enum

    Public Enum enum_TipoInvio_ExportBolleAccettazione
        PrimoInvio = 1
        SecondoInvioDopoModifica = 2
    End Enum

    Public Enum enum_SequenzaProgressiviTipi

        FattureEmesse = 1
        RicevuteEmesse = 2
        FattRicevuteEmesse = 3
        DDTEmessi = 4
        BolleAccettDaDiversi = 5
        NumPaginaRegCaricoScaricoPomo = 6
        NumRigaRegCaricoScaricoPomo = 7
        NumPaginaRegCaricoScaricoPomoNoContrattato = 8
        NumRigaRegCaricoScaricoPomoNoContrattato = 9
        CertificatiPomodoro = 10
        FattureProforma = 11
        NumeroProtocolloFattureAcquisto = 12
        NumeroProtocolloFattureVendita = 13
        NumeroRegistrazioneOperazioniContabili = 14
        NumeroRegistrazioneCorrispettivoVendita = 15
        OrdineVendita = 16
        PreventivoVendita = 17
        DDTRicevutiAccettazione = 18
        Spesometro = 19
        Interscambio_File_XML = 20
        ProgressiviGiornalieriGenerici = 21
        CodiciProgettoZootecnici = 22
        FatturaElettronica = 23
        CodiciProgettoAgricoli = 24
        CodiciOPAgriZoo = 25
        PrenotazionePiante_Richieste = 26
        PrenotazionePiante_Ordini = 27
        Pratiche_UMA_Richieste = 28
        Pratiche_UMA_Rendicontazioni = 29

    End Enum

    Public Enum enum_TipoIVAListino
        IVA_Esclusa = 0
        IVA_Sconto_Inclusi = 1
        IVA_Inclusa = 2
    End Enum

    Public Enum enModalitaSconto
        Percentuale = 0
        Sconto_Merce = 1
        Omaggio_SenzaRivalsaIva = 2
        Campioni_Gratuiti = 3
        Omaggio_ConRivalsaIva = 4
    End Enum

    Public Enum enum_ModalitaFattura
        'nel giaslan: enModalitaLiquidazioneFattura
        NonDefinito = -1
        Fattura_Unica = 0
        Fattura_Acconto = 1
        Fattura_Saldo = 2
        Fattura_Rimborso = 3
        Fattura_AcquistiIntracom = 4
        Fattura_Acconto_Soci = 5
    End Enum

    Public Enum enum_TipoStampaFattura
        NonDefinito = 0
        Autofattura = 1
        Fattura_Immediata = 2
        Fattura_Differita = 3
        Fattura_Unica = 4
        Fattura_Acconto = 5
        Fattura_Saldo = 6
        Fattura_Rimborso = 7
        Fattura_AcquistiIntracom = 8
        Fattura_Acconto_Soci = 9
    End Enum

    Public Enum enum_LayoutFormatiStampaDoc
        Standard = 0
        Peso = 1
        RiscontratoTotale = 2
        Riscontrato = 3
        Prezzo = 4
    End Enum

    Public Enum enum_SpecieVegetali

        Cicoria = 14
        Spinacio = 60
        Bietola_Coste = 69
        Bietola_Foglia = 107
        Cime_Rapa = 5000322

    End Enum

    Public Enum enum_Macchine_TipoAlimentazione

        NonDefinita = 0
        Benzina = 1
        Gasolio = 2
        Metano = 3
        Gpl = 4
        Elettricita = 5
        Olio_Combustibile = 6
        Petrolio = 7

    End Enum

    Public Enum enum_Macchine_TipoTarga

        NonDefinito = 0
        SenzaTarga = 1
        Stradale = 2
        Rimorchio = 3
        Triangolare = 4

    End Enum

    Public Enum enum_Macchine_TipoTrazione

        NonDefinito = 0
        Ruote = 1
        Cingoli = 2
        SemiCingoli = 3
        Doppia_Trazione = 4
        Fisso = 5
        Mobile = 6

    End Enum

    Public Enum enum_AgroReportisticaTipi
        RegistriCantina = 1
        RegistriBio = 2
    End Enum

    Public Enum enum_AgroReportistica
        'in realtà non sono solo i registri di cantina
        'sono i report della tabella agro_reportistica
        Vinificazione_DOC = 1
        Vinificazione_ViniTavola = 2 'obsoleto, è tutto inglobato nell'1
        Imbottigliamento = 3
        Commercializzazione = 4
        PreparazioniBio = 5 'non è un registro di cantina, ma registro bio che utilizza le preparazioni
        Frizzanti = 6
        Spumanti = 7
        'valorizzare anche enum_RegistriCantina
    End Enum

    Public Enum enum_HL_HA
        HL = 1
        HA = 2
    End Enum

    Public Enum enum_UnitaMisura
        KG = 2              ' 1 
        Grammi = 3          '0,001 kg
        Quintali = 4        '100 kg
        Milligrammi = 2032
        Tonnellate = 304

        Millilitri = 101        '
        CentimetriCubi = 104    '
        Litri = 29
        Metri_Cubi = 19         ' 1000 litri
        Ettolitro = 2121


        Grammi__HA = 20    '
        KG__HA = 88
        UNITA__HA = 89
        METRI3__HA = 90
        Tonnellate__HA = 2098
        NumUnita__HA = 176
        QUINTALI__HA = 2120
        Tonnellate__HA_Spighe = 2112

        Litro__HA = 22
        Millilitri__Ha = 163


        CC__HL = 21
        Grammi__HL = 23    '
        Milligrammi__HL = 126

        Millilitri__HL = 164
        Litri__HL = 173
        Millilitri__Litro = 2016
        Chilogrammi__HL = 175
        Grammi__Litro = 2003
        Milligrammi__Litro = 5001006

        Millilitri__Quintale = 165
        KG__Quintale = 169
        Grammi__Quintale = 174
        Litri__Quintale = 303

        Num_Piante = 92
        Unita_Seme = 93
        Confezioni = 1003

        Ettaro = 2123

        Metri = 25
        MetriQuadri = 28
        Minuti = 131
        Ore = 141
        Giorni = 1002

        Numero_Trappole = 11
        Numero_Inneschi = 38

        Numero_Diffusori_HA = 119

        Numero = 38

        Anno = 2019
        StagioneColturale = 2024
        CicloColturale = 2025

        Montegradi = 5001040

        Millimetri = 18
        Chilometri = 305
        Miglia = 306

        UNITA = 5001053

        Numero_Diffusori = 5001052

        Percentuale = 1
        Grammi__dL = 5001055
        femtoLitri = 5001056
        Numero_Adulti_Individui_Trappola = 5
        Presenza = 168
    End Enum

    Public Enum enum_TipoRicetta
        Non_Filtrare = -1
        Standard = 0
        Costi = 1
        PUA = 2
        Budget_Globale = 3
        Budget_Utente = 4
        Standard_Destinazioni = 5
        PianoDistribuzioneConcimi = 6
        ControlloDiGestione = 7
        Standard_Destinazioni_Planning = 8
        PianoDistribuzionePua = 9
        RichiestaUMA = 10
        Zoo = 11
    End Enum

    Public Enum Enum_SiteRedirector
        GiasLan = 0
        Sito_AgronicaStampe = 1
        Sito_AgronicaBio = 2
        Sito_AgronicaView = 3
        Sito_PianoConcimazione = 4
        Sito_AgronicaAnalisi = 5
        Sito_GiasOnline = 6
        Sito_AgronicaAudit = 7
        Sito_AgronicaPlanning = 8
        Sito_AgronicaPUA = 9
        Sito_AgronicaManutenzione = 10
        Sito_AgronicaSicurezzaLavoro = 11
        Sito_AgronicaProfilazione = 12
        Sito_AgronicaStampe_NewMode = 13
        Sito_AgronicaPianiCampionamento = 14
        Sito_AgronicaAgenda_2010 = 15
        Sito_AgronicaSincronizzatore = 16
        Sito_AgronicaAnalisi_2010 = 17
        Sito_AgronicaGlobalGAP = 18
        Sito_AgronicaPianiSemina = 19
        Sito_GiasOnline_2010 = 20
        Sito_AgronicaMeteo = 21
        Sito_AgronicaCheckCOOP = 22
        Ws_GiasPalmWebService_2008 = 23
        Sito_AgronicaStampe_2010 = 24
        Sito_AgronicaSementi = 25
        Sito_AgronicaLabQualita = 26
        Sito_PianoConcimazione_2017 = 27
        Sito_AgronicaWebApiProfilatore = 28
        Sito_AgronicaUma = 29
        Sito_AgronicaDomandaIrrigua = 30

        Sito_AgronicaPortal = 31

        Site_AgronicaSecurity = 99
        GiasAPP = 100
        AgroGSB = 200
        GiasNG = 300
    End Enum

    Public Enum Enum_CodiciDefault
        Semina = 1
        Fioritura = 2
        Raccolta = 3
        Resa = 4
        Dosi_Ha = 17
        Dosi_Ha_Udm = 18

        Unita_Calore = 5
        GG = 6
        Cal_1 = 7
        Cal_2 = 8
        Cal_3 = 9
        Cal_4 = 10
        Cal_5 = 11
        Cal_6 = 12


        tipo_maturazione = 13
        Soglia_Minima = 14
        resa_stabilimento = 15
        peso_sgocciolato = 16
    End Enum

    Public Enum enum_PagineGiasNG
        Pagina_Corrente = 0
        Pagina_Menu_Anagrafica = 1
        Pagina_Menu_Anagrafica_Imprese = 10
        Pagina_Menu_Anagrafica_Centri = 11
        Pagina_Menu_Anagrafica_Catasto = 12
        Pagina_Menu_Anagrafica_Campi = 13
        Pagina_Menu_Anagrafica_Impianti = 14
        Pagina_Menu_Anagrafica_Macchine = 15
        Pagina_Menu_Anagrafica_Contatti = 16
        Pagina_Menu_Anagrafica_Fabbricati = 17
        Pagina_Edit_Impresa = 2
        Pagina_Edit_Centro = 3
        Pagina_Edit_Campo = 4
        Pagina_Edit_Catasto = 5
        Pagina_Edit_AppezzamentoGlobal = 6
        Pagina_Edit_Fabbricato = 7
        Pagina_Edit_Contatto = 8
        Pagina_Edit_Macchina = 9
        Pagina_Menu_Agenda = 20
        Pagina_Edit_Attivita = 21
        Pagina_Configurazione_Operazioni_Culturali = 22
        Pagina_Menu_Profilazione = 30
        Pagina_Profilazione_Import = 31
        Pagina_Menu_Gruppi_Merce = 40
        Pagina_Menu_Gruppi_Merce_Autorizzazioni = 41
        Pagina_AmministrazioneSistema_ConsultaSincroDatiApp = 50
        Pagina_Menu_Visite = 60
        Pagina_Edit_Visite = 61
        Pagina_Valutazioni = 70
        Pagina_Valutazioni_Edit = 71
        Pagina_PianoConti = 80
        Pagina_Menu_Zoo = 90
        Pagina_Budget_Menu_Anagrafica_Imprese = 100
        Pagina_Budget_Menu_Anagrafica_Centri = 110
        Pagina_Budget_Menu_Anagrafica_Catasto = 120
        Pagina_Budget_Menu_Anagrafica_Campi = 130
        Pagina_Budget_Menu_Anagrafica_Impianti = 140
        Pagina_Budget_Menu_Anagrafica_Macchine = 150
        Pagina_GIS = 200
        Pagina_GIS_cfg_proiezioni = 201
        Pagina_Dashboard = 999
        Pagina_Gestione_Preferiti = 1000
        Pagina_Gruppi_Raccolta = 300
        Pagina_Requisiti_Stabilimento = 301
        Pagina_Requisiti_Stabilimento_PianoColturale = 302
        Pagina_Requisiti_Stabilimento_Contratti = 303
        Pagina_Dati_Previsionali_Colture = 305
        Pagina_Requisiti_Stabilimento_VD_PianoColturale = 306
        Pagina_Requisiti_Stabilimento_VD_Contratti = 307
        Pagina_Confronto_Piano_Colturale = 308
        Pagina_Filtro_Ricerca = 309
        Pagina_Export_QdC_To_Agea = 310
        Pagina_Profilazione_Imprese = 311
        Pagina_Analisi_Terreno = 312
        Pagina_Analisi_Terreno_Edit = 313
        Pagina_Report_Impiego_Prodotti_Fitosanitari = 314
        Pagina_Lettura_Contatori = 315
        Pagina_Report_Abilitazione_PdC = 316
        Pagina_Domanda_Irrigua = 320
        Pagina_Terapie = 321
        Pagina_Menu_Rilievi = 322
        Pagina_Trattamento_Zoo = 323
        Pagina_Codifiche_Sistemi_Esterni = 324
        Pagina_Terapia_Zoo = 325
        Pagina_Exe_Terapia_Zoo = 326
        Pagina_Gestione_Carro = 327
        Pagina_SostenibitaCO2_SelezionePerimetro = 329
        Pagina_SostenibitaCO2_CreazioneToken = 331
        Pagina_RischiH20_SelezionePerimetro = 332
        Pagina_Scadenza_Reinnesco_Trappole = 333
        Pagina_PesateAccrescimento = 334
        Pagina_DSS_Nutrizione = 335
    End Enum

    Public Enum enum_PagineAgenda_2010

        Menu = 1

        Pagina_Trattamenti = 2
        Pagina_Fertilizzazione = 3
        Pagina_Installazione_Trapppole = 4
        Pagina_Lavorazioni = 5
        Pagina_Reinnesco_Trapppole = 6
        Pagina_Rilievi = 7
        Pagina_Irrigazione = 8
        Pagina_RilievoPiogge = 9
        Pagina_Semina_Trapianto = 10

        Pagina_Ricette_Lista = 11
        Pagina_Ricette_Edit = 12

        Pagina_DocumentoContabileGenerico = 13

        Pagina_AnalisiCosti = 14
        Pagina_FormProdotto = 15
        Pagina_FiltrinoImprese = 16
        Pagina_GestioneMagazzini = 17
        Pagina_GestioneContatti = 18

        Pagina_Distribuzione_Insetti = 19

        Pagina_MonitoraggioCorpiEstranei = 20

        Pagina_Operazione_Di_Cura = 21
        Pagina_Raccolta = 22

        Pagina_Liquidazione_soci = 23
        Pagina_Filtrone = 24
        Pagina_MenuStampe = 25

        Menu_BS = 26
        Menu_BS_TrackMode = 27

        AnalisiDatiCura = 28

        BI_SchedeRilievi = 29

        Pagina_GestioneMagazziniBS = 30


        Pagina_Trattamenti_B = 31

        Pagina_NonCOnformita_Crea = 32

        Pagina_Manutenzione_Macchine = 33

        Pagina_DuplicaOperazione = 34

        Pagina_Verifica_Conformita = 35

        Report_Percorsi = 36

        Pagina_Menu_Liquidazione_soci = 37

        Pagina_NonConformita_Lista = 38

        Pagina_Analisi_Progetti = 39

        Pagina_Analisi_Meteo = 40
        Pagina_RicetteStampa = 41

        Pagina_Anagrafica_Ereditatore = 42

        Pagina_index = 43

        Pagina_Anagrafica_Appezzamento = 44
        Pagina_Anagrafica_Campo = 45
        Pagina_Anagrafica_Impianto = 46
        Pagina_Anagrafica_Impresa = 47
        Pagina_Anagrafica_Contatto = 48
        Pagina_Anagrafica_Fabbricato = 49
        Pagina_Anagrafica_Macchina = 50
        Pagina_Anagrafica_Particella = 51
        Pagina_Anagrafica_Centro = 52

        Pagina_Scadenzario_Lista = 53
        Pagina_Scadenzario_CreaModifica = 54

        Pagina_Anagrafica_Menu = 55

        Pagina_reportSostenibilita = 56 'Verifica_Sostenibilita
        Pagina_Menu_Lavorazioni = 57

        visite = 58
        Gis = 59

        Pagina_Visite_Lista = 60
        Pagina_Visite_ListaDettagli = 61
        Pagina_Gestione_Costi = 62

        FormProdotto = 63

        Pagina_FatturaElettronica = 64
        Pagina_ReportVendite = 65
        Pagina_KendoEditor = 66

        Pagina_Zoo_Animale = 67
        Pagina_Zoo_Carico = 68
        Pagina_Zoo_Configurazione = 69
        Pagina_Zoo_Control_Panel = 70
        Pagina_Zoo_Pesatura = 71
        Pagina_Zoo_Scarico = 72
        Pagina_Zoo_Spostamento = 73
        Pagina_Zoo_Trattamento = 106
        Pagina_AttivitaXCentri_Aziendali = 74

        Pagina_RicercaDocContabili = 75

        Pagina_GestioneAllegati = 76

        DSSWidget = 77

        Pagina_Anagrafica_Contatto_New = 78

        'TODO: ATTENZIONE Questi stanno riciclando gli stessi numeri delle analisi: è voluto?!?
        Pagina_NonConformita_Anagrafiche = 39
        Pagina_NonConformita_Impostazioni = 40

        Pagina_Stime_Produzione = 79

        Pagina_RicercaTrasferimenti = 80
        Pagina_TrasferimentoMagazzino = 81

        Pagina_MVVElettronico = 82

        Pagina_InvestimentoCatasto = 83

        Pagina_UMA_Richieste = 84
        Pagina_UMA_Elenco = 85

        Pagina_DSS_Difesa = 86
        Pagina_Analisi_Rilievi = 87

        Pagina_Documentale_Lista = 88
        Pagina_Documentale_CreaModifica = 89

        Pagina_Modifica_Multipla_PianoColturale = 90

        Pagina_Gestione_Esercizi = 91

        Pagina_DAA_Elettronico = 92

        Pagina_DocContabile = 93

        Pagina_TrattamentiPostRaccolta = 94

        Pagina_RilieviBS = 95

        Pagina_IrrigazioneBS = 96

        Pagina_GestioneRifiuti = 97

        Pagina_ZooAlimentazione = 98

        Pagina_ZooAltreLavorazioni = 99

        Pagina_Verifica_DoseConsigliataBS = 100

        Pagina_RilievoEffluenti = 101

        Pagina_Gis_StrumentoDiRipartoCatasto = 102
        Pagina_Gis_ImportazioneDati = 103
        Pagina_Gis_EsportazioneDati = 104
        Pagina_SementiInterferenzeSmall = 105

        Pagina_Anagrafica_Menu_Zoo = 107
        Pagina_DatiReteAcqua = 108

        Pagina_DSS_Irrigazione_App = 109
        Pagina_ConsiglioFertirriguo_App = 110

        Pagina_Analisi_Correzione_Testate = 111

        Pagina_PlugIn_Riepilogo_Meteo = 112

        Pagina_PlugIn_Monitoraggio = 113

        Pagina_PlugIn_IndicatoriDSS = 114
        Pagina_Gestione_Servizi = 115
        Pagina_Passaggio_Stato = 116
        Pagina_Zoo_Trattamenti_Capo = 117

        Pagina_BilancioDiMassa = 118

        Pagina_Operazioni_Zootecniche = 119

        AppWeatherSummary = 120
        Pagina_DSS_Irrigazione = 121

        DSSWidget_ConRdir = 122
    End Enum

    Public Enum enum_AnagraficaNgTabs
        imprese = 1
        centri = 2
        fabbricati = 3
        catasto = 4
        campi = 5
        impianti = 6
        contatti = 7
        macchine = 8
    End Enum

    Public Enum enum_TitoloPossesso
        Altro = 0
        Proprieta = 1
        Comodato = 2
        AffittoContratto = 3
        AffittoSenzaContratto = 4
        InContoTerzi = 5
        InConvenzione = 6
        InCompartecipazione = 7
    End Enum

    Public Enum enum_FabbricatiTipi
        FabbricatoUsoAbitativo = 10
        MagazzinoAziendale = 20
        SiloAziendale = 30
        CellaFigoriferaAziendale = 40
        CellaFrigoriferaProdVegetali = 41
        CellaFrigoriferaProdZoo = 42
        RicoveroAnimali = 70
        ImpiantoPrepAlimentariLavUva = 61
        ImpiantoPrepAlimentariLavOlive = 62
        RicoveroAnimaliBox = 176
        FienileAziendale = 180
        essiccatoio = 222
        Altro = 1000
    End Enum

    Public Enum enum_OrientamentoProduttivo
        Cerealicolo = 10
        Orticolo = 20
        Frutticolo = 30
        Viticolo = 40
        Olivicolo = 50
        FloricoloVivaistico = 60
        ColtureIndustriali = 70
        Foraggero = 80
        Zootecnico = 90
        Altro = 99
    End Enum

    Public Enum enum_MetodoProduzione
        Integrato = 1
        InConversione = 2
        Biologico = 3
    End Enum

    Public Enum enum_TipologiaColtura
        ArboreaPura = 1
        Arboreaconsociata = 2
        ErbaceaPura = 3
        ErbaceaConsociata = 4
        Promiscua = 5
    End Enum

    Public Enum enum_Blocco_Flag
        NonBloccato = 0
        Bloccato = 1
        DaVerificare = 2
        Standby = 1000      'Usato per mettere in standby una fattura per cui non si può generare l'xml della fattura elettronica
    End Enum

    Public Enum enum_Accettazione_Tipo
        Nessuno = 0
        PREcampionatura = 1
        POSTcampionatura = 2
    End Enum

    Public Enum enum_TipoListinoClasse
        ListinoIndefinito = 0
        ListinoAcquisto = 1
        ListinoVendita = 2
    End Enum

    Public Enum enum_OTabelle
        Nessuno = 0
        Calibro = 1
        Marca = 2
        Qualita = 3
        Imballaggio = 4
        Confezione = 5
        Declassamento = 6
        MaterialeConfezionamento = 7
        Contenitore = 8
        Grado = 9
        Tipo = 10
        Certificazioni = 12
        Provenienza = 13
        Grammatura = 14
        Uscita = 15
        Punteggio = 16
        GriglieCampionamento = 17
        CalibroCampionamento = 18
        StadioLavorazione = 19
        GruppoFatturazione = 20
        GruppoVarietale = 21
        Rugginosita = 22
        Caratteristica = 24
        AltreCaratteristiche = 25
    End Enum

    Public Enum enum_Omni_Modulo_Generazione
        Nessuno = 0
        Cantine = 1
        FreshFood = 2
        Tabacco = 3
        Olio = 4
        Zoo = 5
    End Enum

    Public Enum enum_Omni_TipoDefault_Preparazioni

        PassaggioRegistri = 1
        ModificaLottoProduzione = 2
        Etichettatura = 3
        CantinaSociale = 5
        RilevamentoCaliPerdite = 6
        Frizzantatura = 7
        Confezionamenti = 8
        Vinificazione = 9
        Manipolazioni = 10
        AggiuntaProdottiEnologici = 11
        Travasi = 12
        TagliSeparazioni = 13
        RilieviRettificheVasca = 15
        ApprovazioniCambiDesignazione = 123
        Spumantizzazione = 256

    End Enum

    Public Enum enum_Omni_Preparazione_Cod
        'CODICI DELLE PREPARAZIONI
        'sono salvati nel campo Codice_Generazione della tabella Linee_Preparazioni
        'sono negativi 
        'e linkano preparazione_cod della tabella Linee_Preparazioni con piva ='AAAAAAAAAAA'

        Passaggio_RegVinificazione_Commercializzazione = -11

        Classificazione = -5
        Riclassificazione = -62
        ClassificazioneBottiglieVinoAttoInLinee = -64
        RiclassificazioneBottiglieVinoAttoInLinee = -65
        ClassificazioneInLinee = -74
        ClassificazioneBottiglieVinoAtto = -90
        ClassificazioneBottiglieVinoAttoSenzaEtichetta = -109

        AggiuntaAcidificanti = -12

        Dolcificazione = -47
        AggiuntaSciroppoDosaggio = -128
        ArricchimentoConSaccarosio = -121
        ArricchimentoConMCR = -27

        Centrifugazione = -122
        Filtrazione = -29
        Sboccatura = -48

        Imbottigliamento = -42
        ImbottigliamentoInLinee = -44
        ImbottigliamentoDaLinee = -45
        ImbottigliamentoVinoAttoADivenire = -63
        ImbottigliamentoSenzaEtichettatura = -106
        ImbottigliamentoSenzaEtichettaturaAttoADivenire = -108

        ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione = -94
        ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione = -80
        ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione = -95
        ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione = -88

        FineFrizzantaturaBottiglia = -81
        FineFrizzantaturaBottigliaViniQualita = -89

        InizioFrizzantaturaAutoclave_RegCommercializzazione = -91
        InizioFrizzantaturaAutoclave_RegVinificazione = -78
        InizioFrizzantaturaAutoclaveMPF_RegVinificazione = -138
        InizioFrizzantaturaAutoclaveMPFxVinoAttoADivenire_RegVinificazione = -144
        InizioFrizzantaturaAutoclave_VinoAttoA_RegCommercializzazione = -101
        InizioFrizzantaturaAutoclave_VinoAttoA_RegVinificazione = -102
        InizioFrizzantaturaAutoclaveVNAF_RegVinificazione = -139
        InizioFrizzantaturaAutoclaveVNAFxVinoAttoADivenire_RegVinificazione = -145

        FineFrizzantaturaAutoclave_RegCommercializzazione = -79
        FineFrizzantaturaAutoclave_RegVinificazione = -123
        FineFrizzantaturaAutoclaveMPFoVNAF_xVinoAttoADivenire_RegCommercializzazione = -151
        FineFrizzantaturaAutoclaveMPFoVNAF_xVinoAttoADivenire_RegVinificazione = -149
        FineFrizzantaturaAutoclaveMPFoVNAF_RegVinificazione = -143
        FineFrizzantaturaAutoclaveVinoAttoADivenire_RegCommercializzazione = -103
        FineFrizzantaturaAutoclaveVinoAttoADivenire_RegVinificazione = -124


        Frizzantatura_Imbottigliamento_RegCommercializzazione = -97
        Frizzantatura_Imbottigliamento_RegVinificazione = -77
        FrizzantaturaAnidrideCarbonica = -73

        ImbottigliamentoInizioSpumantizzazioneBottiglia_RegCommercializzazione = -96
        ImbottigliamentoInizioSpumantizzazioneBottiglia_RegVinificazione = -84
        ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegCommercializzazione = -129
        ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegVinificazione = -130


        InizioSpumantizzazioneAutoclave_RegCommercializzazione = -92
        InizioSpumantizzazioneAutoclave_RegVinificazione = -82
        InizioSpumantizzazioneAutoclaveMPF_RegVinificazione = -140
        InizioSpumantizzazioneAutoclaveVNAF_RegVinificazione = -141
        InizioSpumantizzazioneAutoclaveMPF_XVinoAttoADivenire_RegVinificazione = -146
        InizioSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione = -93
        InizioSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione = -86
        InizioSpumantizzazioneAutoclaveVNAF_XVinoAttoADivenire_RegVinificazione = -147

        FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegCommercializzazione = -150
        FineSpumantizzazioneAutoclaveMPFoVNAF_xVinoAttoADivenire_RegVinificazione = -148
        FineSpumantizzazioneAutoclave_RegCommercializzazione = -83
        FineSpumantizzazioneAutoclave_RegVinificazione = -125
        FineSpumantizzazioneAutoclaveMPFoVNAF_RegVinificazione = -142
        FineSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione = -104
        FineSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione = -126

        ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegCommercializzazione = -87
        ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegVinificazione = -127

        FineSpumantizzazioneBottiglia = -85
        FineSpumantizzazioneBottigliavinoAtto = -131

        ConfezionamentoBagInBox = -43
        ConfezionamentoBagInBoxDaLinee = -71

        RilevamentoProdottiSfusiVasca = -38
        RilevamentoCaloTrasformazione = -39
        RilevamentoPerditaLavorazione = -70

    End Enum

    Public Enum enum_Omni_Tipo_Generazione

        Colore = 1
        Categorie = 2 'doc, igp, tavola
        Classificazioni = 3 'fermo, frizz, ecc
        SemilavoratiMateriePrime = 4 'Uve Fresche,Mosto PF,Vino In Frizzantatura...
        ProdottoFinito = 5 ' bottiglie
        Condizionati = 6 'Damigiana, Fusto...
        CaliLavorazione = 7 'Calo di Conversione,Calo di Imbottigliamento,Calo di Trasformazione,Perdita di Lavorazione
        BeniConfezionamento = 8 'Bottiglia Vuota 1,5 l, Tappo, capscula....
        AltreMateriePrime = 9 'Acidificanti,Metabisolfito di Potassio,...
        TipiPreparazioni = 10 'Passaggio da Mosto a Mosto PF, frizzantatura
        Preparazioni = 11
        ParametriDpi = 12
        Utility = 13
        LottiProduzione = 14 'Anno Produzione Vino in Bottiglia, Anno Vendemmia
        MagazziniRecipienti = 15 'Reparto Uve
        AnalisiParametri = 16 'Acidita' Fissa, Solforosa Totale...
        Scarti = 17 'raspi, Prodotto Feccioso, vinacce
        TipiPiano = 18 'Classificazione/Declassamento, confezionamento
        Dicitura = 19 'nessuna, riserva
        Denominazioni = 20 'bosco eliceo, forlì, ecc
        BagInBox = 21 'Bag in Box 3 l...
        TipologieProdottiFiniti = 22 'Bottiglie,Condizionati,Bag in Box
        Caratteristiche = 23
        GradiLiberta = 24
        TipiProtocollo = 25

    End Enum

    Public Enum enum_Omni_Gradi_Liberta

        'colore = 1
        GL1_Bianco = 16
        GL1_Rosso = 17
        GL1_Rosato = 18
        GL1_Grigio = 678

        'categoria = 2
        GL2_Docg = 19
        GL2_Dop = 20
        GL2_Igp = 21
        GL2_Tavola = 22
        GL2_Varietale = 163
        GL2_Nessuno = 386

        'classificazioni = 3
        GL3_Fermo = 23
        GL3_Frizzante = 24
        GL3_Spumante = 25
        GL3_Passito = 26

        'diciture = 4
        GL4_Nessuna = 140
        GL4_Riserva = 143

        'caratteristiche 5
        GL5_Nessuno = 358
        GL5_AttoAlTaglioDocg = 359
        GL5_AttoAlTaglioDop = 492
        GL5_SuperoProduzioneDocg = 360

    End Enum

    Public Enum enum_Omni_Codice_Generazione

        'tipo   SemilavoratiMateriePrime = 4 
        Tipo4_Mosto = 51
        Tipo4_MostoPF = 52
        Tipo4_Uve_Fresche = 50
        Tipo4_UveFrescheDaSupero = 326
        Tipo4_Uve_Passite = 131
        Tipo4_Uve_Stramature = 165
        Tipo4_Vino = 55
        Tipo4_VinoArricchito = 433
        Tipo4_VinoAttoADivenire = 54
        Tipo4_VinoAttoInFrizzantatura = 319
        Tipo4_VinoAttoInSpumantizzazione = 248
        Tipo4_VinoInFrizzantatura = 231
        Tipo4_VinoInSpumantizzazione = 247
        Tipo4_VNAF = 53
        Tipo4_VNAFarricchito = 434

        Tipo4_Brillato = 685
        Tipo4_Calibrato = 395
        Tipo4_Confezionato = 396
        Tipo4_Grezzo = 683
        Tipo4_Miscelato = 671
        Tipo4_Natura = 394
        Tipo4_Pallettizzato = 407
        Tipo4_PallettizzatoMisto = 409
        Tipo4_Pulito = 682
        Tipo4_Sbiancato = 684
        Tipo4_Scarto = 426

        'tipo prodotti finiti =5
        Tipo5_Bottiglia075 = 87
        Tipo5_BottInFrizz020 = 346
        Tipo5_BottInFrizz0375 = 239
        Tipo5_BottInFrizz050 = 240
        Tipo5_BottInFrizz075 = 241
        Tipo5_BottInFrizz150 = 242
        Tipo5_BottInFrizz300 = 243
        Tipo5_BottInFrizzAtto020 = 344
        Tipo5_BottInFrizzAtto0375 = 276
        Tipo5_BottInFrizzAtto050 = 278
        Tipo5_BottInFrizzAtto075 = 277
        Tipo5_BottInFrizzAtto150 = 279
        Tipo5_BottInFrizzAtto300 = 280


        'tipo  CaliLavorazione = 7
        Tipo7_CaloConversione = 101
        Tipo7_CaloImbottigliamento = 102
        Tipo7_CaloTrasformazione = 103
        Tipo7_PerditaLavorazione = 132

        'tipo  Beni Confezionamento = 8
        Tipo8_Capsula = 83
        Tipo8_Etichetta = 85
        Tipo8_Fascetta = 84
        Tipo8_Tappo = 82
        Tipo8_Cartone6 = 92
        Tipo8_Cartone12 = 93
        Tipo8_BottigliaVuota075 = 80

        'tipo   AltreMateriePrime = 9
        Tipo9_MetabisolfitoDiPotassio = 67
        Tipo9_MCR = 68
        Tipo9_CompostiLiquidiBaseDiSolfiti = 112
        Tipo9_Saccarosio = 254


        'tipo_default delle preparazioni 
        'è salvato nel campo Tipo_Default della tabella Linee_Preparazioni
        'e linka il codice generazione della tabella OGenerazioni_Anagrafe
        'con tipo_generazione = 10
        Tipo10_Pigiatura = 1
        Tipo10_Passaggio_Mosto_MostoPF = 2
        Tipo10_Passaggio_MostoPF_VNAF = 3
        Tipo10_Passaggio_VNAF_Destinato = 4
        Tipo10_Classificazione = 5
        Tipo10_Declassamento = 6
        Tipo10_Frizzantatura = 7
        Tipo10_Imbottigliamento = 8
        Tipo10_Condizionamento = 9
        Tipo10_ArricchimentoDolcificazione = 10
        Tipo10_Aggiunta_Prodotti_Enologici = 11
        Tipo10_Travaso = 12
        Tipo10_TaglioAccorpamento = 13
        Tipo10_Separazione = 14
        Tipo10_Rettifica_Consistenze_Enologiche = 15
        Tipo10_Passaggio_Vinificazione_Standard = 118
        Tipo10_Cambio_Designazione = 123
        Tipo10_Frizzantatura_Imbottigliamento = 230
        Tipo10_Spumantizzazione = 256

        'tipo  lotti = 14
        Tipo14_AnnoVendemmia = 108
        Tipo14_AnnoProduzioneBagInBox = 199
        Tipo14_AnnoProduzioneVinoBottiglia = 104
        Tipo14_AnnoProduzioneVinoSfuso = 105

        'tipo  magazzini = 15
        Tipo15_RepartoUve = 46
        Tipo15_RepartoProdottiEnologici = 47
        Tipo15_RepartoSottoprodotti = 48
        Tipo15_RepartoConfezionato = 49
        Tipo15_VascaStoccaggio = 121
        Tipo15_RepartoConfezionatoSenzaEtichetta = 337
        Tipo15_MagazzinoOrtofrutta = 391
        Tipo15_MagazzinoTabacco = 459
        Tipo15_MagazzinoLatte = 578

        'tipo  Scarti = 17
        Tipo17_Raspi = 98
        Tipo17_ProdottoFeccioso = 99
        Tipo17_Vinacce = 100
        Tipo17_Fecce = 427

    End Enum

    Public Enum enum_RegistroContoTerzi
        Nessuno = 0
        RegistroGlobale = 1
        RegistroUnicoDiversificato = 2 '(utilizzato solo nel registro di vinificazione, attivo per chi ha l'accettazione/conferimento uva)
        RegistroSeparatoContoTerzi = 3
        RegistroSoloContoLavoro = 4 '(utilizzato solo nel registro di imbottigliamento)
    End Enum

    Public Enum enum_CategorieDocumenti

        CartaIdentità = 1
        PatentinoProdottiFosanitari = 2
        Patente = 3
        RegistroTrattamenti = 4
        RegistriFertilizzazioni = 5
        ElencoFornitori = 6
        ElencoClienti = 7
        AnalisiFitofarmaci = 8
        DomandaFascicolo = 9
        CredenzialiPrivacy = 10
        AnalisiTerreno = 11
        TaraturaUgelli = 12
        PianoConcimazione = 13
        PUA = 14
        Fattura_Emessa = 15
        NotaAccredito_Emessa = 16
        DDT_Contabilizzato_Emesso = 17
        Ordine_Vendita = 18
        DDT_Emesso = 19
        Conferimento = 20
        RicevutaFiscale = 21
        QuadroP = 22
        Preventivo_Emesso = 23
        Bilancio = 24
        ScadenziarioPagamentiClienti = 25
        ScadenziarioPagamentiFornitori = 26
        LiquidazioneIVA = 27
        RegistriIVA_Vendite = 28
        RegistriIVA_Acquisti = 29
        RegistroCorrispettivi_Vendita = 30
        PresentazioneRiba = 31
        Biologico_MateriePrime = 32
        Biologico_Vendite = 33
        Biologico_Colturale = 34
        Biologico_Preparazioni = 35
        Magazzino_Giacenze = 36
        Magazzino_Movimenti = 37
        Magazzino_Fertilizzanti = 38
        Magazzino_Fitosanitari = 39
        RegistriCampagna = 40
        LibroGiornale = 412
        Ordine_Acquisto = 42

        AttoNotorio = 43
        AdesioneEticoAmbientale = 44
        CodiceCondotta = 45
        TenutaSchedaCampagna = 46
        AdesioneDPI = 47
        ImpegnativaGLOBAL = 48
        ImpegnativaQC = 49
        ImpegnativaConfusioneSessuale = 50
        MandatoTrasmissioneTelematica = 51
        ImpegnativaOrticoleGestioneAnnuale = 52
        ImpegnativaOrticoleGestioneBreve = 53
        ImpegnativaOrticoleIndustria = 54
        ImpegnativaPomodoroIndustria = 55
        ImpegnativaFagiolinoMercatoFresco = 56
        AllegatoCatastoValorizzazioni = 57

        AccettazioneDaDiversi_Bolla = 58
        AccettazioneDaDiversi_CertificatoPomodoro = 59
        AccettazioneDaDiversi_ReportImballi = 60
        AccettazioneDaDiversi_Report = 61

        Cantina_ConsistenzeEnologiche = 62
        Cantina_RegistroVinificazione = 63
        Cantina_RegistroCommercializzazione = 64
        Cantina_RegistroImbottigliamento = 65
        Cantina_RegistroFrizzanti = 66
        Cantina_RegistroSpumanti = 67
        Cantina_BrogliaccioMovimenti = 68

        FF_BollaCampionatura = 69
        FF_FatturaSoci_Liquidazione = 70
        FF_PagatiSuConferito_Liquidazione = 71
        FF_PagatiSuCampionato_Liquidazione = 72
        FF_RiepilogoSoci_Liquidazione = 73
        FF_AutofatturaSoci_Liquidazione = 74

        PrecisionFarming_MappaPrescrizione = 75
        PrecisionFarming_MappaProduzione = 76
        PrecisionFarming_FileBordoMacchina = 77

        AdesioneNurtureModule = 80
        AdesioneDespar = 81
        AdesioneConad = 82
        DichiarazioneResponsabilita = 83
        AccordoResponsabilitaFiliera = 84
        FitoregolatoriKiwi = 85
        AdesioneStandardLeaf = 86
        SchedaAziendale = 87
        ImpegnoProduzioneSociDivisoxCentri = 88
        ImpegnativaColtivazioneConferimento = 89
        QuestionarioValutazioneAzienda_Aggiornamento = 90
        Stampa_Abilitazioni = 91
        AdesioneModuloGrasp = 92
        AdesioneProtocolloGlobalGAP = 93

        'ATTENZIONE!
        '1) mettere la insert nel migra
        '2) gestire il tipo enumerativo in AgronicaCoreScadenziario_BIZ.Allegati.SalvaAllegato()

    End Enum

    '################################################################################################
    '#####   Tipo enumerativo per le espressioni regolari standard
    '################################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostato dal modulo Input_Controllato
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Enum enum_EspressioniRegolari

        RegExp_Nessuna = 0
        RegExp_Email = 1
        RegExp_CodiceFiscale = 2
        RegExp_PartitaIVA = 3
        RegExp_Username = 4
        RegExp_Password = 5
        RegExp_CAP = 6

    End Enum

    Public Enum enum_TrappoleUso

        Monitor = 1
        CattureDiMassa = 2
        ConfusioneSessuale = 3
        Disorientamento = 4

    End Enum

    Public Enum enum_Tipo_Salvataggio
        Salva_e_Esci = 1 'default
        Salva_e_Nuovo = 2
        Salva_e_Duplica = 3
        Salva_e_Vai_ai_Costi = 4
    End Enum

    Public Enum enum_MeteoFonte
        SMR = 1
        StazioneLocale = 2
        ZonaByUtente = 3
    End Enum

    Public Enum enum_MeteoAlgoritmiNormalizzazione
        LeggiDatiPrecedenti = 1
        LeggiDatoPiuVicinoSpazialeInCascata = 2
        LeggiDatoSuSerieStoriche = 3
        LeggiDatoDaMedieGiornaliere = 4
    End Enum

    Public Enum enum_MeteoFlagGiornoOora
        Giorno = 1
        Ora = 2
    End Enum

    Public Enum enum_Tipo_Importazione
        Da_Access = 1
        Da_Excel = 2
        Da_WS = 3
    End Enum

    Public Enum enum_Opzioni_Laboratori
        Bool_Mail_Automatica_Invio = 1
        Mail_Automatica_A = 2
        Mail_Automatica_CC = 3
        Mail_Rispondi_A = 4

        Bool_Mail_Automatica_Invio_ALERT = 5
        Mail_Automatica_A_ALERT = 6
        Mail_Automatica_CC_ALERT = 7
        Mail_Rispondi_A_ALERT = 8
        Mail_Oggetto = 9
        Mail_Firma = 10

        ''' <summary>
        ''' 0/Assente = Nessun automatismo, 1 = Invio Mail(creare query sul migra per valorizzazione iniziale), 2 = Rest API
        ''' </summary>
        Tipo_Automatismo_Richiesta_Analisi = 11
        Richiesta_Rest_TipoAPI = 12     'API tipo 1 = Zespri/Pedonlab/Agrolab, ...
        Richiesta_Rest_Endpoint = 13    'URL per invio REST richiesta analisi
        Richiesta_Rest_Authorization = 14

        ''' <summary>
        ''' 0/Assente = Nessun automatismo, 1 = FTP/Invio Mail(creare query sul migra per valorizzazione iniziale??), 2 = Rest API
        ''' </summary>
        Tipo_Automatismo_Risultato_Analisi = 15
        Risultato_Rest_TipoAPI = 16     'API tipo 1 = Zespri/Pedonlab/Agrolab, ...
        Mail_Lingua = 17 ' lingua usata nella mail

    End Enum

    Public Enum enum_Tipo_Operazione_Agenda
        QuadernoDiCampagna = 1
        Ricetta = 2
        RicettaBrogliaccio = 3
    End Enum

    Public Enum enum_Tipo_Operazione_Agenda_Target
        Reale = 1
        Planning = 2
    End Enum

    Public Enum enum_TipoStampaRicevutaFiscale
        PdfA4Singola = 0
        WordA5Personalizzabile = 1
        PdfA5 = 2
        PdfA4ACapoAutomatico = 3
    End Enum

    Public Enum enum_StampaConSenzaPreview
        ConPreview = 0
        SenzaPreview = 1
    End Enum

    Public Enum enum_OrdinamentoStampaConsEnologiche
        Nessuno = 0
        Piano = 1
        IdentificativoVasca = 2
        NumeroSerie = 3
        Materiale = 4
        Prodotto = 5
        Lotto = 6
        LineaProduttiva = 7
    End Enum

    'Public Enum enum_Lavorazioni

    '    CONCIA_SEME = 13
    '    DISERBO = 18
    '    TRATTAMENTO_ANTIPARASSITARIO = 74
    '    TRATTAMENTO_FITOREGOLATORE = 103
    '    DISTRIBUZIONE_INSETTI = 116
    '    CONFUSIONE_SESSUALE = 118
    '    DISORIENTAMENTO_SESSUALE = 121
    '    CATTURE_MASSA = 122
    '    GEODISINFESTAZIONE = 155
    '    DISSECCAMENTO = 158
    'End Enum

    Public Enum enum_PianoConcimazione_Tipo

        Bilancio = 1
        Schede = 2


    End Enum

    Public Enum enum_PianoConcimazione_FattoriCorrettivi

        N_Dose_Standard = 6

        P_Dose_Standard = 7
        P_Dose_Standard_MoltoBassa = 10
        P_Dose_Standard_Bassa = 8
        P_Dose_Standard_Elevata = 9

        K_Dose_Standard = 11
        K_Dose_Standard_MoltoBassa = 14
        K_Dose_Standard_Bassa = 12
        K_Dose_Standard_Elevata = 13

        N_Inc_Max = 47
        P_Inc_Max = 54
        K_Inc_Max = 59

        N_Max_Intervento = 66

        Resa_Standard = 5
        Resa_Bassa = 15
        Resa_Alta = 34

        N_Inc_Resa = 35
        P_Inc_Resa = 48
        K_Inc_Resa = 55
        N_Dec_Resa = 16
        P_Dec_Resa = 24
        K_Dec_Resa = 29

        N_I_anno_allevamento = 60
        N_II_anno_allevamento = 61
        P_I_anno_allevamento = 62
        P_II_anno_allevamento = 63
        K_I_anno_allevamento = 64
        K_II_anno_allevamento = 65


    End Enum

    Public Enum enum_PianoConcimazione_Dotazione

        MoltoBassa = 1
        Bassa = 2
        Media = 3
        Elevata = 4
        MoltoElevata = 5

    End Enum

    Public Enum enum_PianoConcimazione_FaseCicloColturale

        Arb_PreImpianto = 1
        Arb_I_Anno_Allevamento = 2
        Arb_II_Anno_Allevamento = 3
        Arb_Produzione = 4

    End Enum

    Public Enum enum_MultiModificaImpianti_Proprieta

        'NB: quando si aggiungono tipi enum, agigornare MultiModificaImpiantiProprieta_Des_from_Cod

        Organismo_Referente = 0
        Magazzino_Conferimento = 1
        Sup_Impianto = 2
        Capitolato_Privato = 3
        Disciplinare = 4
        DataSeminaPrevista = 5
        DataRaccoltaPrevista = 6
        ResaPrevista = 7
        DataFiorituraPrevista = 8
        Certificazione = 9
        Regolamento_Fertilizzazioni = 10
        Finalita = 11
        Stato_Impianto = 12
        Regolamento = 13

        Cultivar = -16
        ChiusuraImpianti = -10
        ChiusuraAppezzamenti = -9
        TipologiaVarietale = -8
        SestoImpianto = -7
        ChiusuraLotti = -6
        AperturaLotti = -5
        AperturaCampo = -4
        LottoInterno = -15


        Pro_N = enum_CodiciAnagrafe.Impianto_LimiteN
        Pro_P2O5 = enum_CodiciAnagrafe.Impianto_LimiteP
        Pro_K2O = enum_CodiciAnagrafe.Impianto_LimiteK
        Pro_MgO = enum_CodiciAnagrafe.Impianto_LimiteMg


        AppBio = enum_CodiciAnagrafe.Codice_Appezza_Biologico '1085
        '+ tutti i codici Anagrafe

        Org_Referente


    End Enum

    'Tipi di Warning
    Public Enum enTipoWarning_Verifica
        Epoche = 1
        Numero_Interventi = 5
        Numero_Interventi_xProdotto = 6
        Intervallo_Trattamenti_xProdotto = 7
        Soglie = 10
        DoseRame_Anno_xBio = 20
        DoseRame_5Anni_xBio = 21
        Prodotto_Sa_Bio = 30
        DoseDPI_MaxAnno = 2
        Numero_Interventi_Min = 3

        Dose_MaxAnno = 8
        Dose_MaxAnno_xAvversita_Infestante = 9
        Dose_MaxAnno_GPAI = 11

    End Enum

    Public Enum enTipoErrCode_Verifica

        ErroreRoutine = -1
        ProdottoNonUtilizzabileXData = 0

        ProdottoNonRegistratoSuColtura = 8

        AvversitaNonGiustificataDPI = 1
        AvversitaNonTrattabileDPI = 2

        ProdottoNonGiustificatoSuAvversitaDPI = 3
        ProdottoNonGiustificatoSuAvversita = 7

        ProdottoNonUtilizzabileXDisciplinare = 10
        ProdottoNonUtilizzabileXDisciplinarexStatoImpianto = 11

        BufferZoneNonRispettata = 4

        SogliaNonRegistrata = 5

        ProdottoNonBiologico = 6
        FitofarmacoNonBiologico = 150

        MixPolveruentiENon = 12

        InterventoNonConsentito = 9

        DoseNonDisponibile = 100

        UnitaMisuraNonCompatibile = 101
        DoseEccessiva = 102
        AcquaNonCorretta = 103
        DoseEccessivaRame = 104
        DoseEccessivaSADpi_Anno = 105
        DoseEccessivaRame5Anni = 106

        DoseEccessivaRameDpi = 111
        Engine_QuantitaAcquaEccessiva = 112
        Engine_AcquaObbligatoria = 113

        DoseInsufficiente = 107

        SuperatoVolumeMaxAcquaDpi = 108
        DoseEccessivaEtichetta_Anno = 109
        DoseEccessivaEtichettaAvversita_Anno = 110


        SuperatoNumMaxInterventiPA = 201
        SuperatoNumMaxInterventiProdotto = 202
        IntervalloInterventiNonRispettato = 203
        IncompatibilitaTraSostanze = 204
        NonRaggiuntoNumMinInterventiPA = 205

        Epoca_PreRaccolta = 301
        Epoca_PreSemina = 302
        Epoca_Etichetta = 303

        DataInterventoMin = 350
        DataInterventoMax = 351

        ImpiantiNonCoerentiDPI = 700
        ImpiantiRaggruppamentoColturaleNonOmogeneo = 701
        ImpiantiCoperturaNonOmogeneo = 702
        ImpiantiDPINonImpostato = 703 'non gestito
        ImpiantiDpiNonOmogeneo = 704
        ImpiantiVincoloPiuRestrittivo = 705

        DoseDiserboEccessiva = 801

        ProdottoVincolatoFormulato = 802

        Superato_N_Max = 1050
        Superato_P_Max = 1051
        Superato_K_Max = 1052
        Superato_M_Max = 1053
        Superato_N_Max_Intervento = 1054

        CarenzaNonRispettata = 7777
        Engine_CarenzaNonRispettataRaccolta = 7778
    End Enum

    Public Enum enum_Applicabilita_GPAI
        Specificato = 0 'Se il Gpai_PA_Ausiliari_Cod è presente viene considerato
        NonSpecificato = 1 'Manca l'implementazione. Il Gpai_PA_Ausiliari_Cod non viene considerato, se è presente viene ignorato
        NonApplicabile = 2 ' Il Gpai_PA_Ausiliari_Cod non viene considerato, se è presente viene ignorato
    End Enum

    Public Enum enStatoControlli_VerificaWS
        NonVerificato = 0
        Verificato = 1
        DaVerificare = 2
        NonVerificabile = 4
    End Enum

    Public Enum enum_TipoImportazioneDaExcel
        Importa_Imprese = 1
        Importa_Contatti = 2
    End Enum

    Public Enum enum_GestioneVettore
        Nessuno = 0
        FrancoPartenza = 1
        FrancoArrivo = 2
        FrancoSpedizioniere = 3
        FrancoLungoBordo = 4
        FrancoABordo = 5
        CostoENolo = 6
        CostoAssicurazioneENolo = 7
        TrasportoPagatoFinoA = 8
        TrasportoEAssicurazionePagatiFinoA = 9
        ResoFrontiera = 10
        ResoExShip = 11
        ResoBanchina = 12
        ResoNonSdoganato = 13
        ResoSdoganato = 14
    End Enum

    Public Enum enum_TrasportoACaricoDi
        Cedente = 0
        Cessionario = 1
        Vettore = 2
    End Enum

    Public Enum enum_FatturaTipo
        'campo extra_int di movimenti
        Differita = 0
        Immediata = 1
    End Enum

    Public Enum enum_RegimeIva
        NonImpostato = -1
        Ordinario = 0
        Speciale = 1
        EsenzioneIva = 2
    End Enum

    Public Enum enum_LiquidazioneIva
        NonImpostato = -1
        Mensile = 0
        Trimestrale = 1
    End Enum

    Public Enum enum_TipologiaIva
        Aliquota = 1
        NonImponibile = 2
        EsclusioneIva = 3
    End Enum

    Public Enum enum_GIS2012_TipoEntita
        Nessuno = -1
        NON_DEFINITO = 0
        APPEZZAMENTI = 1
        STALLE = 2
        CATASTO = 3
        TESTO = 4
        TEMALIBERO = 5
        VULNERABILITA = 6
        ZONAZIONEPAC = 7
        AREEOMOGENEE = 8
        UTILIZZODEISUOLI = 9
        SVILUPPORURALE = 10
        DATI_IMPORTATI = 11
        ETTARI_EQUIVALENTI = 12
        CAMPIONAMENTI = 13
        CAMPIONIANALISI = 17
        CAMPI = 18
        IMPIANTI_ORTICOLA = 19
        IMPIANTI_ERBACEA = 20
        IMPIANTO_NUDO = 21
        IMPIANTO_ARBOREA = 22
        IMPIANTO_GENERICO = 23
        PLANNING = 33
        PLANNING_IMPIANTI_PIANIFICATI = 53
        AGEDNA = 50
        OpAgenda = 54
        Mappe_Prescrizione = 51
        Dettaglio_Ricetta = 60
        Destinazione_Agenda = 63
        LINEE_GUIDAPrecisionFarming = 55
        CentriAziendali = 67
        Indici_Rischio_Produttivita = 91
        Indice_Erosione = 92
        Indice_CO2 = 93
        Indice_Rischio_Meteo = 94
        RASTER = 100
    End Enum

    Public Shared Function MultiModificaImpiantiProprieta_Des_from_Cod(ByVal Codice As enum_MultiModificaImpianti_Proprieta) As String

        Dim Des As String

        Select Case Codice

            Case enum_MultiModificaImpianti_Proprieta.Organismo_Referente
                Des = "Organismo Referente"
            Case enum_MultiModificaImpianti_Proprieta.Magazzino_Conferimento
                Des = "Magazzino di Conferimento"
            Case enum_MultiModificaImpianti_Proprieta.Sup_Impianto
                Des = "Superficie Impianto"
            Case enum_MultiModificaImpianti_Proprieta.Capitolato_Privato
                Des = "Capitolato Privato"
            Case enum_MultiModificaImpianti_Proprieta.Disciplinare
                Des = "Disciplinare"
            Case enum_MultiModificaImpianti_Proprieta.DataSeminaPrevista
                Des = "Data Semina Prevista"
            Case enum_MultiModificaImpianti_Proprieta.DataRaccoltaPrevista
                Des = "Data Raccolta Prevista"
            Case enum_MultiModificaImpianti_Proprieta.ResaPrevista
                Des = "Resa Prevista"
            Case enum_MultiModificaImpianti_Proprieta.DataFiorituraPrevista
                Des = "Data Fioritura Prevista"
            Case enum_MultiModificaImpianti_Proprieta.Certificazione
                Des = "Certificazione"
            Case enum_MultiModificaImpianti_Proprieta.Regolamento_Fertilizzazioni
                Des = "Regolamento Fertilizzazioni"
            Case enum_MultiModificaImpianti_Proprieta.Finalita
                Des = "Finalità Produttiva"
            Case enum_MultiModificaImpianti_Proprieta.Stato_Impianto
                Des = "Stato Impianto"
            Case enum_MultiModificaImpianti_Proprieta.Regolamento
                Des = "Regolamento"
            Case enum_MultiModificaImpianti_Proprieta.Pro_K2O
                Des = "Massimo Apporto Potassio (K2O) in kg/ha"
            Case enum_MultiModificaImpianti_Proprieta.Pro_MgO
                Des = "Massimo Apporto Ossido di Magnesio (MgO) in kg/ha"
            Case enum_MultiModificaImpianti_Proprieta.Pro_N
                Des = "Massimo Apporto Azoto (N) in kg/ha"
            Case enum_MultiModificaImpianti_Proprieta.Pro_P2O5
                Des = "Massimo Apporto Anidride Fosforosa (P2O5) in kg/ha"
            Case enum_MultiModificaImpianti_Proprieta.ChiusuraAppezzamenti
                Des = "Chiusura Appezzamento"
            Case enum_MultiModificaImpianti_Proprieta.ChiusuraImpianti
                Des = "Chiusura Impianto Colturale"
            Case enum_MultiModificaImpianti_Proprieta.ChiusuraLotti
                Des = "Chiusura Progetto (Distinta)"
            Case enum_MultiModificaImpianti_Proprieta.AperturaLotti
                Des = "Apertura Progetto (Distinta)"
            Case enum_MultiModificaImpianti_Proprieta.AppBio
                Des = "Numero Appezzamento Biologico"
            Case enum_MultiModificaImpianti_Proprieta.Cultivar
                Des = "Varietà"
            Case enum_MultiModificaImpianti_Proprieta.TipologiaVarietale
                Des = "Tipologia Varietale"
            Case enum_MultiModificaImpianti_Proprieta.LottoInterno
                Des = "Lotto Impianto Colturale"
            Case enum_MultiModificaImpianti_Proprieta.SestoImpianto
                Des = "Sesto Impianto"

            Case Else
                Try
                    Dim tempEnum As enum_MultiModificaImpianti_Proprieta = CType(Codice, enum_MultiModificaImpianti_Proprieta)
                    Des = [Enum].GetName(GetType(enum_MultiModificaImpianti_Proprieta), tempEnum)
                    Des = Des.Replace("_", " ")
                Catch ex As Exception
                    Des = "Non gestito"
                End Try
        End Select

        Return Des

    End Function

    Public Enum enum_TipiCapitolati
        LMR_UE = 0
        Sblocco = -1
        CTRL_Qualita = -2
        Documentazione = -3
    End Enum

#Region "WWorkflow, servizi e stati"
    Public Enum enum_Servizi

        QuadernoCampagnaStd = 1
        QuadernoCampagnaGlobal = 2
        RegistroTrattamenti = 3
        RegistroFertilizzazioni = 4
        RegistriCaricoScaricoMagazzino = 5
        QuadernoCampagnaBio = 6
        RegistroVenditeBio = 7
        RegistroMateriePrimeBio = 8
        PAPVegetale = 9
        PAPZootecnico = 10
        PAPPreparazioni = 11
        NotificaBio = 12
        PUA = 13
        PianoConcimazione = 14
        CheckListCondizionalita = 15
        CheckListSicurezzaLavoro = 16
        CheckListGlobal = 17
        Teleregistri = 18
        Sistema_Qualità_Nazionale_Produzione_Integrata = 19
        Esportazione_in_formato_Zespri = 20
        Esecuzione_ed_avanzamento_delle_ricette = 21
        CAIImpresaQdCAttivo = 23

        DocContabili_Ordine_Acquisto = 100
        DocContabili_Bolla_Ricevuta = 101
        DocContabili_Bolla_Emessa = 102
        DocContabili_Accettazione = 103

        Profitosan = 1001
        QStandard = 1002
        QPlus = 1003
        QBio = 1004
        QFert = 1005
        QMaps = 1006
        Condizionalita2018 = 1007
        Profitosan_serverSide = 1008
        QDemetra = 1017
        QDemetraQdCBluarancio = 1014

        RBase = 1030
        RPlus = 1031

        Workflow_di_attivazione_aziende_GIAS = 1050
        Registro_Trattamenti = 2001
        Registro_Trattamenti_Bio = 2002
        Quaderno_Campagna_Azienda = 2003
        Quaderno_Campagna_Caa = 2004
        Registro_Fertilizzazioni_PUA = 2005
        Quaderno_Campagna_CBPA = 2006
        Gestione_UMA = 2007

        Azienda_NO_SQNPI_PUA = 2008

        ACA_2 = 2009
        ACA_4 = 2010
        ACA_12 = 2011
        ACA_13 = 2012
        ACA_24_01 = 2013 ' Azione 1
        ACA_24_02 = 2014 ' Azione 2

        Gestione_Azienda = 3000
    End Enum

    Public Enum enum_Servizi_Stati

        Pratica_Aperta = 1
        Pratica_Validata = 2
        Pratica_Chiusa = 3
        In_fase_di_invio_a_SIGPA = 4
        Rilievo_giacenze_in_corso = 5
        Rilievo_giacenze_completato = 6
        Bloccato_PerDatiNonCoerenti = 9
        Ultimo_invio_sigpa = 10

        Pratica_Aperta_QdC = 22
        Pratica_Validata_QdC = 23
        Pratica_Chiusa_QdC = 24

    End Enum


    Public Enum enum_WWorflow

        Approvazione_Formulati = 10
        Attivazione_Aziende_Agrarie_in_GIAS = 1002
        Compilazione_Quaderno_di_Campagna = 5
        Completamento_Giacenze = 2
        DAA_Telematico = 21000001
        Esportazione_Zespri = 252
        Invio_dei_dati_a_SIGPA = 3
        Lettura_iMotion = 20
        Esecuzione_ed_avanzamento_delle_ricette = 21
        Predisposizione_dei_Dati = 1
        Procedure_di_liquidazione_dei_soci = 4
        Servizi_Agronica_2017 = 1001
        Sistema_Qualità_Nazionale_Produzione_Integrata = 251
        Teleregistri = 18
        Verifica_Quaderno_Campagna = 2001
        ImpresaQdCAttivo = 6
        DocContabili_Approvazione = 7

    End Enum

    Public Enum enum_WWorflow_WAnagraficaStati



        Approvazione_Allegati_Formulati_Allegati_Verificati_e_Validati = 20
        Approvazione_Allegati_Formulati_Verifica_allegati_in_corso = 21
        Approvazione_DPi_In_Fase_di_Verifica = 17
        Approvazione_DPi_Approvato__da_pubblicare = 18
        Approvazione_DPi_Pubblicato = 19
        Approvazione_Formulati_In_Fase_di_Verifica = 15
        Approvazione_Formulati_Verificato_e_Validato = 16
        Attivazione_Aziende_Agrarie_in_GIAS_Nessun_Dato_Memorizzato = 1050
        Attivazione_Aziende_Agrarie_in_GIAS_Anagrafica_Impresa_Memorizzata = 1051
        Attivazione_Aziende_Agrarie_in_GIAS_Piano_Colturale_Confermato = 1052
        Attivazione_Aziende_Agrarie_in_GIAS_Audit_iniziale_completato = 1053
        Attivazione_Aziende_Agrarie_in_GIAS_Impresa_correttamente_profilata = 1054
        Attivazione_Aziende_Agrarie_in_GIAS_Piano_Colturale_in_fase_di_compilazione = 1055
        Compilazione_Quaderno_di_Campagna_Pratica_Aperta = 22
        Compilazione_Quaderno_di_Campagna_Pratica_Validata = 23
        Compilazione_Quaderno_di_Campagna_Pratica_Chiusa = 24
        QdCAttivo = 25
        QdCChiuso = 26
        Completamento_Giacenze_Rilievo_giacenze_in_corso = 5
        Completamento_Giacenze_Rilievo_giacenze_completato = 6
        DAA_Telematico_Aperto = 21000001
        DAA_Telematico_DAA_proposto_IE815_inviato_a_sistema = 21000002
        DAA_Telematico_DAA_proposto_IE815_vagliato_correttamente = 21000003
        DAA_Telematico_DAA_proposto_IE815_contenente_errori = 21000004
        DAA_Telematico_DAA_IE801_ricevuto = 21000005
        DAA_Telematico_Nota_di_Ricevimento_IE818_Merce_ricevuta__accettata_e_soddisfacente = 21000006
        DAA_Telematico_Nota_di_Ricevimento_IE818_Merce_ricevuta_ma_rifiutata_parzialmente_o_totalmente = 21000007
        DAA_Telematico_Cambio_destinazione_IE813_inviato_a_sistema = 21000008
        DAA_Telematico_Annullamento_del_DAA = 21000009
        DAA_Telematico_Rientro_in_deposito = 21000010
        DAA_Telematico_Nota_di_Ricevimento_IE818_Inviato_a_sistema = 21000011
        DAA_Telematico_Nota_di_Ricevimento_IE818_Vagliato_correttamente = 21000012
        DAA_Telematico_DAA_Annullato_IE819_ricevuto = 21000013
        DAA_Telematico_DAA_Annullato_IE819_contenente_errori = 21000014
        DAA_Telematico_Cambio_destinazione_IE813_Vagliato_Correttamente = 21000015
        DAA_Telematico_Cambio_destinazione_IE813_contenente_errori = 21000016
        DAA_Telematico_Nota_di_Ricevimento_IE818_contenente_errori = 21000017
        DAA_Telematico_Richiesta_Creata = 21000050
        DAA_Telematico_File_Firmato_Caricato_a_sistema = 21000051
        DAA_Telematico_File_Firmato_Inviato = 21000052
        DAA_Telematico_Risposta_Positiva_Ricevuta = 21000053
        DAA_Telematico_Risposta_Negativa_Ricevuta = 21000054
        DAA_Telematico_In_fase_di_verifica = 21000055
        DAA_Telematico_In_fase_di_preparazione = 21000101
        DAA_Telematico_Riepilogo_Caricato_a_sistema = 21000102
        DAA_Telematico_Riepilogo_vagliato_correttamente = 21000103
        DAA_Telematico_Riepilogo_contenente_errori = 21000104
        DocContabili_Inserito = 110
        DocContabili_RDA_Completato = 120
        DocContabili_RDA_Rifiutato = 130
        DocContabili_ODA = 140
        DocContabili_DaInviare = 150
        DocContabili_Inviato = 160
        DocContabili_InvioFallito = 170
        Esportazione_XML_Universale_Operazione_Agenda_Creata = 251
        Esportazione_XML_Universale_Operazione_Esportabile = 252
        Esportazione_XML_Universale_Operazione_Esportazione_In_Corso = 253
        Esportazione_XML_Universale_Operazione_Esportata = 254
        Esportazione_XML_Universale_Operazione_Modificata = 255
        Esportazione_Zespri_In_fase_di_preparazione = 21000501
        Esportazione_Zespri_Valido_per_esportazione = 21000502
        Esportazione_Zespri_Consegnato = 21000503
        Importazione_XML_Universale_File_Importazione_In_Corso = 261
        Importazione_XML_Universale_File_Importazione_OK = 262
        Importazione_XML_Universale_File_Importazione_Errore = 263
        Invio_dei_dati_a_SIGPA_In_fase_di_invio_a_SIGPA = 4
        Invio_dei_dati_a_SIGPA_Ultimo_invio_a_sigpa = 10
        Lettura_iMotion_Scaricato__in_fase_di_valutazione = 20000001
        Lettura_iMotion_Associato_a_Raccolta = 20000002
        Lettura_iMotion_Non_associato_a_raccolta = 20000003
        Lettura_iMotion_Importato_in_GIS__In_fase_di_valutazione = 20000004
        Predisposizione_dei_Dati_Pratica_Aperta = 1
        Predisposizione_dei_Dati_Pratica_Validata = 2
        Predisposizione_dei_Dati_Pratica_Chiusa = 3
        Predisposizione_dei_Dati_Errori_Riscontrati_da_SIGPA = 7
        Predisposizione_dei_Dati_Bloccato_per_dati_non_coerenti = 9
        Procedure_di_liquidazione_dei_soci_Non_liquidato = 11
        Procedure_di_liquidazione_dei_soci_Calcolato = 12
        Procedure_di_liquidazione_dei_soci_Liquidabile = 13
        Procedure_di_liquidazione_dei_soci_Fatturato = 14
        Servizi_Agronica_2017_Non_Attivo = 1001
        Servizi_Agronica_2017_Attivo__Pagante = 1002
        Servizi_Agronica_2017_Attivo__Pagante_PROMO_Natale_2017 = 1003
        Servizi_Agronica_2017_Attivo__in_demo_30_gg = 1004
        Servizi_Agronica_2017_Attivo__in_demo_60_gg = 1005
        Servizi_Agronica_2017_Attivo__Gratuito = 1006
        Servizi_Agronica_2017_Scaduto = 1007
        Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Aperta = 270
        Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Validata = 271
        Sistema_Qualità_Nazionale_Produzione_Integrata_In_Fase_di_verifica_per_errori_formali = 272
        Sistema_Qualità_Nazionale_Produzione_Integrata_Errori_formali_su_tracciato_xml = 273
        Sistema_Qualità_Nazionale_Produzione_Integrata_Inviata_correttamente__in_valutazione_per_errori_sostanziali = 274
        Sistema_Qualità_Nazionale_Produzione_Integrata_Errori_sostanziali_riscontrati_su_xml = 275
        Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_acquisita_correttamente_nel_SQNPI = 276
        Sistema_Qualità_Nazionale_Produzione_Integrata_In_fase_di_invio = 277
        Sistema_Qualità_Nazionale_Produzione_Integrata_Bloccato_per_dati_non_coerenti = 278
        Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire = 300
        Esecuzione_ed_avanzamento_delle_ricette_Eseguita = 301
        Teleregistri_Creata = 18000001
        Teleregistri_Valida_per_linvio = 18000002
        Teleregistri_Non_valida_per_linvio = 18000003
        Teleregistri_Autorizzata_per_linvio = 18000004
        Teleregistri_Invio_in_corso = 18000005
        Teleregistri_Invio_effettuato_correttamente = 18000006
        Teleregistri_Invio_non_riuscito = 18000007
        Teleregistri_Errori_rilevati_dal_SIAN = 18000008
        Teleregistri_Valida_nel_SIAN = 18000009
        Teleregistri_In_fase_di_verifica = 18000010
        Quaderno_Campagna_In_Compilazione = 2001
        Quaderno_Campagna_Verifica_in_corso = 2002
        Quaderno_Campagna_Verifica_Completata = 2003
        Quaderno_Campagna_Verifica_Completata_Con_Riserva = 2004
        Quaderno_Campagna_Compilazione_Alla_Data_Completata_e_Verificata = 2007

        Gestione_UMA_Rinuncia = 2009

        Registro_Carico_Scarico_Passaporti_Vivaisti_DaConfermare = 320
        Registro_Carico_Scarico_Passaporti_Vivaisti_Confermato = 321

        QdC_Non_Definito = 0
        QdC_Da_Eseguire = 400
        QdC_Eseguito = 401

        QdC_Bluarancio_Demetra_Azienda_Attivata = 101403
        QdC_Bluarancio_Demetra_Servizio_Impresa_Verde = 101404
        QdC_Bluarancio_Demetra_Servizio_4_Mani = 101405
    End Enum

#End Region

    Public Enum enum_GruppiUtentiSigpa
        Utente_generico_coldiretti_umbria = 1
        Utente_revisore_sigpa = 2
        Utente_amministratore_sigpa = 3
        Utente_esportazione_dati_full_sigpa = 4
        Utente_esportazione_dati_semplice_sigpa = 5
        Regione_Veneto = 6
        Regione_Umbria = 7
    End Enum

    Public Enum enum_Tipo_DB
        TUTTI = 0
        GIAS_SERVER = 1
        GIAS_UTENTI = 2
        GIAS_METASCHEMA = 3
        GIAS_PianoConcimazione = 4
    End Enum

    Public Enum enum_TipoOperazioneProgrammazioneEntita

        Nessuna = 0
        Confermato = 1
        Nuovo_Appezzamento = 2
        Nuovo_Impianto = 3
        Nuova_Distinta = 4
        Modifica_Semplice = 5
        Modifica_Superficie = 6
        Unione = 7
        Frazionamento = 8
        Chiudi_Appezzamento = 9

    End Enum

    Public Enum enum_AnalisiCosti_OrigineCosto

        Listini = 1
        ProdottiCosti = 2
        Giacenza = 3
        MediaPonderata = 4

    End Enum

    Public Enum enum_Tipo_CAC_Codifica_ProdottiAziendali
        'attuali clienti che hanno importatori/esportatori che utlizzano questa tabella
        NonDefinito = 0
        Agrisol_Seled = 1
        Terremerse = 2
        ConsAgrRavenna = 3
        Coldiretti_ConsAgrPerugia = 4
        Coldiretti_RegioneUmbria = 5
        Agrintesa = 6
        FruitModena_Seled = 7
        Francesconi = 8
        Coldiretti_RegioneUmbria_Fitosanitari = 9
        BonificaLamone = 10
        RicciEGuardigli = 11
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_Specie
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco_DBWIN = 2 ' db frescodb
        Orogel_Surgelato_DBWIN = 3 ' db conf_orocoop
        Orogel_Policoro = 4 ' db policorodb
        Orogel_Rovigo = 5 ' db rovigodb
        Orogel_Fresco_WMS = 12
        Orogel_Surgelato_WMS = 13
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_Varieta
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco_DBWIN = 2 ' db frescodb
        Orogel_Surgelato_DBWIN = 3 ' db conf_orocoop
        Orogel_Policoro = 4
        Orogel_Rovigo = 5
        Orogel_Fresco_WMS = 12
        Orogel_Surgelato_WMS = 13
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_InfoAggiuntive
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco = 2 ' db frescodb
        Orogel_Surgelato = 3 ' db conf_orocoop
        Orogel_Policoro = 4
        Orogel_Rovigo = 5
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_FormeAllevamento
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco_DBWIN = 2 ' db frescodb
        Orogel_Surgelato_DBWIN = 3 ' db conf_orocoop
        Orogel_Policoro = 4
        Orogel_Rovigo = 5
        Orogel_Fresco_WMS = 12
        Orogel_Surgelato_WMS = 13
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_ImpiantiIrrigazioni
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco_DBWIN = 2 ' db frescodb
        Orogel_Surgelato_DBWIN = 3 ' db conf_orocoop
        Orogel_Policoro = 4
        Orogel_Rovigo = 5
        Orogel_Fresco_WMS = 12
        Orogel_Surgelato_WMS = 13
    End Enum

    Public Enum enum_Tipo_CAC_Codifica_Portinnesti
        NonDefinito = 0
        Apofruit_Siagr = 1
        Orogel_Fresco_DBWIN = 2 ' db frescodb
        Orogel_Surgelato_DBWIN = 3 ' db conf_orocoop
        Orogel_Policoro = 4
        Orogel_Rovigo = 5
        Orogel_Fresco_WMS = 12
        Orogel_Surgelato_WMS = 13
    End Enum

    Public Enum enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod
        CapitolatoPrivato = 1
        DettaglioSpeciePersonalizzato = 2
        CAA_Agrea = 3
        CAA_Anagrafe = 4
        Piano_Semina = 5
        Revisione_Reportistica = 6
        Residuo = 7
        Certificazione_Prodotto = 8
        CodificaRazzeTRACESNT = 9
        Lavorazione = 10
        Specifica = 11
        ModalitaLiquidazione = 12
        OrigineProdotto = 13
    End Enum

    Public Enum enum_TipoForm_Sincronizzatore_Apoconerpo
        Nessuno = 0
        Form_Importa_Seled_QDC_Agrintesa = 1
        Form_Importa_Seled_Catasto = 2
        Form_Importa_Uniforma = 3
    End Enum

    Public Enum enum_TipoImportazioneAnagrafe
        Agrea_WebService = 1
        Agrea_AccessOP = 2
        Agrea_Excel = 3
        AnagrafeER_WebService = 4
    End Enum

    Public Enum enum_Tipo_Operazione_Sincronizzatore
        Simula = 0
        Importa = 1
        Sincronizza = 2
        Importa_Tutto = 3
        Importa_Nuovi = 4
        Sincronizza_Tutto = 3
        Sincronizza_Nuovi = 4
    End Enum

    Public Enum enum_ImportMagazzinoXMLPubblicoMode
        Nessuno = -1
        Analizza = 0
        Importa = 1
    End Enum

    Public Enum enum_ImportMagazzinoXMLPubblicoCase
        NonImpostato = 0
        ConsAgrPerugia = 1
        Cab_Terremerse = 2
        Francesconi = 3
    End Enum

    Public Enum enum_IstCredito_PerRisorsa
        ContoCorrente = 1
        BancoPosta = 2
    End Enum

    Public Enum enum_Liquidita_CauRisorsa
        Nessuna = 0
        RisorsaFinanziaria = 1
        LiquiditaImmediata = 2
    End Enum

    Public Enum enum_ReportInsoluti_Ordinamento
        Scadenza = 0
        ClienteData = 1
        Data = 2
        Numero = 3
        ClienteScadenza = 4
    End Enum

    Public Enum enum_RegistriIva_Ordinamento
        NumeroDoc = 1
        NumeroProtocollo = 2
    End Enum

    Public Enum enum_RegImbottigliamento_NumVasca
        Nessuna = 0
        NumVascaParametrico = 1 'verrà stampata la reale provenienza del vino duranete i condizionamento
        CampoNoteLibero = 2 'verrà stampato un campo note identico a quanto visto per l'idonetià della classificazione e gestito nella stessa maniera).
    End Enum

    Public Enum enum_TipoEntita
        Impresa = 1
        Centro = 2
        Campo = 3
        Appezzamento = 4
        Impianto = 5
        Particella = 6
        Macchina = 7
        Contatto = 8
        AnalisiTerreno = 9
        PianoConcimazione = 10
        PUA = 11
        Fabbricato = 12
        Ricetta_Destinazione = 13
        OperazioneDiAgenda = 14
        Uma_Carburanti_Richiesta = 15
        Ricetta = 16
        Particella_Impresa = 17
    End Enum

    Public Structure enum_TipoEntita_Des
        Const Imprese = "Imprese"
        Const CentriAziendali = "Centri_Aziendali"
        Const CantinaVasche = "Cantina_Vasche"
        Const Appezza = "Appezzamento"
        Const AppezzamentiXIndirizzi = "AppezzamentiXIndirizzi"
        Const Budget_Appezza = "Budget_Appezzamento"
        Const Impianti = "Reg_Impianti"
        Const Budget_Impianti = "Budget_Reg_Impianti"
        Const Progetti = "Imprese_Progetti"
        Const Budget_Progetti = "Budget_Imprese_Progetti"
        Const Fabbricati = "Fabbricati"
        Const Campi = "Campi"
        Const CampiCodici = "Campi_Codici"
        Const Indirizzi = "Indirizzi"
        Const UtentiXAppezzamenti = "UtentiXAppezzamenti"
        Const AppezzamentiXParticelle = "AppezzamentiXParticelle"
        Const AppezzamentiXParticelleXMacrousi = "AppezzamentiXParticellexMacrousi"
        Const AppezzamentiXParticelleXMacrousiXUtilizzo = "AppezzamentiXParticellexMacrousixUtilizzo"
        Const ZooAnimali = "Zoo_Animali"
        Const ZooAnimaliDistinte = "Zoo_Animali_Distinte"
        Const ParcoMacchine = "Parco_Macchine"
        Const ProdottiCosti = "Prodotti_Costi"
        Const ParticelleCatastali = "ParticelleCatastali"
        Const ImpreseXParticelle = "ImpreseXParticelle"
        Const GruppiRaccolta = "Gruppo_Raccolta"
        Const DataPublish = "DataPublish"
        Const Allegati = "Allegati"
        Const Zoo_Animali = "Zoo_Animali"
        Const Squadre = "Squadre"
    End Structure

    Public Enum enum_ID_Area_Alert
        Contatti = 1
        Macchine = 2
        Analisi = 3
        PianiConcimazione = 4
        Nitrati = 5
        AgricolturaDiPrecisione = 6
        UMA_Carburanti = 7
        Richiesta_Iscrizione_GIAS = 8
        Report_Gias = 9
        Documenti_Contabili = 10
        Operazioni_Campagna_QDC = 11
        Catasto = 12
        Carichi_Scarichi_Magazzino = 13
    End Enum

    Public Enum enum_ID_Area_Tipologia
        Patentino_trattamenti = -1
        Taratura_ugelli = -2
        Carta_Identita = -3
        Analisi_terreno = -4
        Piano_Concimazione = -5
        PUA = -6
        Laboratorio = -7
        OrganismoDiControllo = -8
        AgricolturaDiPrecisione_MappaPrescrizione = -9
        AgricolturaDiPrecisione_MappaProduzione = -10
        AgricolturaDiPrecisione_FileBordoMacchina = -11
        Doc_Template_Richiesta_Iscrizione_GIAS = -12
        Richiesta_Iscrizione_GIAS = -13
        Scheda_Campagna_Completa = -14
        UMA_dichiarazione_pre_assegnazione = -15
        Fatture_Attive_Da_Sistema_Esterno = -16
        Fatture_Passive_Da_Sistema_Esterno = -17
        Ordini_Acquisto = -18
        DDT_Ricevuti = -19
        Conferimenti = -20
        DDT_Emessi = -21
        Ordini_Vendita = -22
        Fatture_Passive = -23
        Fatture_Attive = -24
        Operazioni_Campagna_QDC = -25
        Contratti_Affitto = -26
        Possesso_Particelle = -27
        Carichi_Magazzino = -28
        Scarichi_Magazzino = -29
        Stazione_Meteo_Infragri = -30
    End Enum

    Public Enum enum_Fasi_UMA
        UMA_prima_richiesta = -16
        UMA_approvazione_prima_richiesta = -17
        UMA_richiesta_integrativa = -18
        UMA_approvazione_richiesta_integrativa = -19
        UMA_rendicontazione = -20
        UMA_approvazione_rendicontazione = -21
    End Enum

    Public Enum enum_Ambiti_UMA
        UMA_Carburanti = -14
        Check_List = -15
    End Enum

    Public Enum CBI_Riba_Causali
        Nessuno = -1
        NonaAncoraEsportata = 0
        InFaseDiPresentazione = 30000
        Ricevuta_resa_al_carico = 30006 '(la banca assuntrice restituisce la ricevuta al creditore mittente perché formalmente irregolare o comunque inidonea al trattamento);
        Ricevuta_non_correttamente_domiciliata = 30007
        Ricevuta_richiamata = 30008 '(a seguito di richiesta da parte del creditore mittente per le ricevute ancora in portafoglio della banca assuntrice); 
        ricevuta_resa_al_carico_poiché_presentata_ceduta_oltre_i_termini_dell_accordo = 30009 '" " (la banca domiciliataria restituisce immediatamente la ricevuta alla la banca assuntrice restituisce immediatamente la ricevuta in quanto presentata transitata in SIA al di fuori dei termini previsti dalla procedura; la SIA in questo caso appone apposita segnalazione - pos. 110 del record 70) ;
        ricevuta_pagata = 30010 '(la banca assuntrice trasmette la segnalazione dell’avvenuto accredito della presentazione a seguito del regolamento interbancario. Per ogni dettaglio ulteriore si faccia riferimento agli accordi bilaterali banca-cliente); (la banca assuntrice trasmette previo accordo con il cliente creditore la segnalazione di pagato al cliente creditore stesso a seguito di comunicazione da parte della banca domiciliataria);
        Ricevuta_nonPagata = 42010 'la banca domiciliataria rinvia la ricevuta alla banca assuntrice per mancato adempimento del debitore
    End Enum

    Public Enum enum_Giorno_Settimana

        Indifferente = -1
        Domenica = System.DayOfWeek.Sunday
        Lunedi = System.DayOfWeek.Monday
        Martedi = System.DayOfWeek.Tuesday
        Mercoledi = System.DayOfWeek.Wednesday
        Giovedi = System.DayOfWeek.Thursday
        Venerdi = System.DayOfWeek.Friday
        Sabato = System.DayOfWeek.Saturday

    End Enum

    Public Enum enum_GSB_Livello_Log

        NessunLog = 0
        Tutto = 1
        WarningPiuErrori = 2
        Errori = 3

    End Enum

    Public Enum enumTipoConto
        Economico = 0
        Patrimoniale = 1
    End Enum

    Public Enum enum_Conti_Patrimoniali
        NonUsare_FondiAmmortamento = -1
        IvaACredito = 103
        IvaADebito = 104
        IvaACreditoAcqIntra = 105
        IvaADebitoAcqIntra = 106
        CreditiVersoClienti = 34
        DebitiVersoFornitori = 75
        DepositiBancariPostali = 46
        DenaroValoriInCassa = 48
        ErarioRitenuteLavoroAutonomo = 94
        DebitiVsEnasarco = 100
    End Enum

    Public Enum enum_Conti_Economici
        RicavixIVAincompensazione = 64
        OmaggiAllaClientela = 65
        RicavixIVAincompensazioneEstero = 66
    End Enum

    Public Enum enum_Note_Intervento_Utilizzo
        Ricetta = -1
        QuadernoCampagna = -2
        PianoConcimazione = -3
    End Enum

    Public Enum enum_Note_Intervento_Gruppi
        Meteo = -1
        Vento_Intensita = -2
        Vento_Direzione = -3
        Temperatura = -4
        Orario = -5
        Motivazione = -6
    End Enum

    Public Enum enum_ChkCoGe
        OperazioniFinanziarieIndipendenti = -1
        CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA = 0
        CoGe_CONTABILIZZAZIONE_MANUALE = 1
        CoGe_CONTABILIZZAZIONE_AUTOMATICA = 2
        CoGe_PD_COLLEGATAaDOCUMENTO = 3
        CoGe_NON_CONTABILIZZATA = 4
    End Enum

    Public Enum fasiFenologicheTipo
        Nascosta = -1
        generico = 0
        BBCH_Foglia = 1
        BBCH_Grappolo = 2
    End Enum

    Public Enum enum_Sorgenti_idriche
        Pozzo = 1
        Cisterna = 2
        Canale = 3

    End Enum

    Public Enum enum_Tipo_Dettaglio_Audit
        Fabbricati = 1
        Macchine = 2
        Elementi = 3
        Appezzamenti = 4

    End Enum

    Public Enum enum_PrenotazionePiante_ripasso
        NO = 0
        Si_Fallanza = 1
        Si_Gratuito = 2
    End Enum

    Public Enum enum_PrenotazionePiante_Stato
        o_opzionato = 0
        o_parziale = 4
        o_completato = 1
        o_da_controllare = 99
        o_evaso = 10
        o_accettato = 11
        o_rifiutato = 12
        o_inviato = 13
        o_ordine_creato_esolver = 14
        o_da_inviare = 15
        o_cancellato = 16


        p_non_abbinato = 0
        p_abbinato = 1
        p_consegnato = 3
        p_copiato = 2
        p_da_controllare = 99
        p_evaso = 10
        p_cancellato = 16
    End Enum

    Public Enum enum_TipoCodiceAppezzamento_SchedaCampagna
        NonSpecificato = 0
        RiferimentoAlfanumerico = 1
        NumeriInAppNome = 2
        AppezzaMenoBasecode = 3
        NomeAppezzamento = 4
    End Enum

    Public Enum enum_StampaDefinitivaDiProva
        DiProva = 0
        Definitiva = 1
    End Enum

    Public Enum enum_LCQ_TipoModelloStandard
        Tutti = 0
        DaAttivare = 1
        Attivo = 2
        AttivoODaAttivare = 3
        NonPiuValido = 4
    End Enum

    Public Enum Enum_Programmatore_Agronica
        costa = 1
        leandri = 2
        monti = 3
        gunelli = 4
        grilli = 5
        garavini = 6
        magnani = 7
        bravi = 7
        buggatore_ignoto = -1
    End Enum

    Public Enum Enum_Imprecazione
        cavolo = 1
        merda = 2
        cazzo = 3
        managerdimmerda = 4
        mavaffanculo = 5
        checchifo = 6
    End Enum

    Public Enum Enum_SerbatoioTipo
        VascaEnologica = 1
        Barrique = 2
        Tonneau = 3
        Barile = 4
        Botte = 5
        Caldaia = 6
    End Enum

    Public Enum enum_SerbatoioMateriale
        Altro = 0
        AcciaioInox = 1
        AcciaioSmaltato = 2
        Fiberglass = 3
        CementoArmato = 4
        PlasticaAlimentare = 5
        Rovere = 6
        Castagno = 7
        Ginepro = 8
        Gelso = 9
        Ciliegio = 10
        CementoFuoriTerraRivestitaconResineEpossidiche = 2222
        CementoFuoriTerraFrigor = 3333
        AcciaiooFerroRivestitoconResineEpossidiche = 4444
        AcciaiooFerroRivestitoFrigor = 5555
        Vetroresina = 6666
        AcciaioperElaborazioneViniFrizzanti = 7777
    End Enum

    Public Enum euno_SerbatoioAppoggio
        Altro = 0
        Serbatoiodistoccaggiosugambe = 1
        Serbatoiodistoccaggiosubasamento = 2
        CisternaMobile = 3
        CisternaMobilePallettizzata = 4
    End Enum

    Public Enum enum_SerbatoioTasca
        Anidodape = 1
        Acanalina = 2
    End Enum

    Public Enum enum_SerbatoioForma
        Circolare = 0
        Rettangolare = 1
    End Enum

    Public Enum TipoTabelleRintraccioOpta
        Rintraccio_X_Collo_OPTA = 1
        Rintraccio_X_Appezzamento = 2
        Rintraccio_X_CoronaAppezzamento_OPTA = 3
        Rintraccio_X_Grado_Opta = 4
        Rintraccio_X_Buchi = 0
    End Enum

    Public Enum enum_RapportiContabili_SaCod
        Tutti = 0
        PersoneGiuridiche = 1
        PersoneFisiche = 2
    End Enum

    Public Enum enum_Contatti_SaCod
        Privato = 0
        Pubblico = -1
        NoFiltro = -99
    End Enum

    Public Enum enum_Contatti_IdCf
        PersonaFisica = 0
        PersonaGiuridica = 1
        ContattoEstero = 2
        NoFiltro = -99
    End Enum

    'Tipi di Modalità Applicazione Sconto
    Public Enum enum_TipoSconto
        PrezzoUnitario = 0
        Imponibile = 1
        Totale = 2
    End Enum

    Public Enum enum_EditImporto
        PrezzoUnitario = 0
        Imponibile = 1
        Importo = 2
        Importo_Unitario = 3 'importo totale riga diviso qta (importo totale di un elemento)
    End Enum

    Public Enum enum_TipoPeso
        Peso_Lordo = 0
        Peso_Netto = 1
    End Enum

    Public Enum enum_PrezzoLivello
        Kg_Litri = -1
        Udm_principale = 0  'Nessuno
        Confezione = 5
        Contenitore = 8
        Imballo = 4
    End Enum

    Public Enum enum_ModalitaIva
        Nessuno = 0
        Indetraibile = 1
        Compensazione = 2
    End Enum

    Public Enum enum_TipologiaLayer
        NonSpecificato = 0
        Std = 1
        SpecieVegetale = 5 ' traduzione specieXlingua
        Avversita = 7 ' neutrali 
        Fenologia = 8 ' non gestite
        RilieviVegetoProduttivi = 9 ' da gestire
        Percorsi = 10 ' obsoleto
        AnalisiPeriodicitaAgenda = 11 ' obsoleto
        Cultivar = 15 ' cultivarXLingue
        Organizzazione = 100 ' ok
    End Enum

    Public Enum enum_PnlCtrl_CategorieTipi
        NonConformita = 1
        Scadenza = 2
        Allegati = 3
    End Enum

    Public Enum enum_PnlCtrl_CategorieGiasID
        Tabacco = -1
    End Enum

    Public Enum enum_PnlCtrl_Stati
        Aperto = 1
        Analisi = 2
        AzionePreventiva = 3
        AzioneCorrettiva = 4
        Verifica = 5
        Chiuso = 6
    End Enum

    Public Enum enum_NC_Tipi
        Nativo = 1
        DaArticoliFRUTTAGEL = 2
        DaChecklistSchedaControlliTTI = 3
        DaAnalisiResiduiPDC = 4
        DaChecklistSchedaControlliALP = 5
    End Enum

    Public Enum enum_NC_CategoriePubbliche
        AnalisiResiduiPDC = -1
    End Enum

    'Costanti per le proprietà default dei lotti
    Public Enum enum_TipoProprieta_Lotto
        Contenitore = 1
        Titolo_Alcol = 2
        Categoria_Omni = 3
    End Enum

    'NON USARE!!!
    'USARE enum_Planning_Fonte
    Public Enum enum_EnteValidadore
        Agea = 1
        Avepa = 2
        Agrea = 3
        Anagrafe_ER = 4
        RegioneUmbria_Sigpa = 5
        Artea = 6
    End Enum

    Public Enum enum_AWS_FunzioneRichiesta

        Fitofarmaci_FamigliePrincipiAttivi_Utilizzate_DT = 1
        Fitofarmaci_Formulati_Bio = 2
        Fitofarmaci_Formulati_PrincipiAttivi = 3
        Fitofarmaci_Formulati_PrincipiAttiviGruppiPrincipiAttivi = 4
        Fitofarmaci_Formulati_PrincipiAttivi_2 = 5
        Fitofarmaci_Formulati_PrincipiAttivi_Famiglie_DT = 6
        Fitofarmaci_Formulati_PrincipiAttivi_SpecieVegetali = 7
        Fitofarmaci_Formulati_SpecieVegetali = 8
        Fitofarmaci_Formulati_SpecieVegetali_Avversita = 9
        Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi = 10
        Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi_DT = 11
        Fitofarmaci_Formulati_SpecieVegetali_Infestanti = 12
        Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi = 13
        Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi_DT = 14
        Fitofarmaci_Formulato_Completo = 15
        Fitofarmaci_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita = 16
        Fitofarmaci_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_2 = 17
        Fitofarmaci_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_3 = 18
        Fitofarmaci_Leggi_FormulatiBio_ConDosi_DT = 19
        Fitofarmaci_Leggi_Formulati_ConDosi = 20
        Fitofarmaci_Leggi_Formulati_ConDosi_DT = 21
        Fitofarmaci_Leggi_Formulati_Info_DS = 22
        Fitofarmaci_Leggi_Formulati_Info_DT = 23
        Fitofarmaci_Leggi_Formulato_Info_con_UdM_DS = 24
        Fitofarmaci_Leggi_Formulato_UdM = 25
        Fitofarmaci_Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti = 26
        Fitofarmaci_Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_2 = 27
        Fitofarmaci_Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_3 = 28
        Fitofarmaci_Verifica_Collegamento = 29
        Fitofarmaci_Verifica_Collegamento_new = 30
        Fitofarmaci_Verifica_DifesaDiserboWS = 31
        Fitofarmaci_Verifica_DifesaDiserboWS_Bio = 32
        Fitofarmaci_Leggi_Formulati_ConDosi_DT_Profitosan = 33
        Fitofarmaci_Formulati_Classificazioni_DT_Profitosan = 34

        Disciplinari_DPI_Consultazione_Difesa = 100
        Disciplinari_DPI_Consultazione_Diserbo = 101
        Disciplinari_DPI_Verifica_DifesaDiserboWS = 102
        Disciplinari_DPI_Verifica_DifesaDiserboWS_Livello = 103
        Disciplinari_DPI_Verifica_DifesaDiserboWS_New = 104
        Disciplinari_Dose_Etichetta = 105
        Disciplinari_Leggi_Disciplinari = 106
        Disciplinari_Leggi_Disciplinari2 = 107
        Disciplinari_Leggi_Disciplinari_Elenco = 108
        Disciplinari_Leggi_Disciplinari_Elenco_xTestata = 109
        Disciplinari_Leggi_Disciplinari_xTestata = 110
        Disciplinari_Leggi_EpocheDifesa = 111
        Disciplinari_Leggi_EpocheDiserbo = 112
        Disciplinari_Leggi_Formulati_DPI = 113
        Disciplinari_Leggi_Formulati_DPI_2 = 114
        Disciplinari_Leggi_Formulati_DPI_DT = 115
        Disciplinari_Leggi_Formulati_DPI_DT_2 = 116
        Disciplinari_Leggi_Formulati_DPI_DT_PROFITOSAN = 117
        Disciplinari_Leggi_Infestanti = 118
        Disciplinari_Leggi_Infestanti_conDescrizioniAvversita = 119
        Disciplinari_Leggi_RaggruppamentiDPI = 120
        Disciplinari_Leggi_RaggruppamentiDPI2 = 121
        Disciplinari_Leggi_SoglieIntervento = 122
        Disciplinari_Leggi_SoglieInterventoxConsultazione = 123
        Disciplinari_Stringa_PA_Validi = 124
        Disciplinari_Verifica_Collegamento = 125
        Disciplinari_Verifica_Collegamento_new = 126
        Disciplinari_Leggi_Iaf = 127

        CapitolatoCliente_Verifica_Collegamento_new = 200
        CapitolatoCliente_get_LMR = 201
        CapitolatoCliente_get_ListaDiTabellaLMR = 203
        CapitolatoCliente_get_elenco_CapitolatoCliente = 204
        CapitolatoCliente_get_elenco_dpi_privati = 205
        CapitolatoCliente_get_elenco_dpi_pubblici = 206
        CapitolatoCliente_verifica_Analisi = 207

        Esterni_Disciplinari_Leggi_Disciplinari = 500
        Esterni_Fitofarmaci_Leggi_Formulati_Carico = 501
        Esterni_Fitofarmaci_Leggi_Formulati = 502
        Esterni_Fitofarmaci_Leggi_Formulati_Avversita = 503
        Esterni_Fitofarmaci_Leggi_Formulati_Avversita_Dosi = 504

        Esterni_Fitofarmaci_Leggi_Fertilizzanti = 505

        Esterni_Leggi_Formulati_Demetra = 506
        Esterni_Leggi_Fertilizzanti_Demetra = 507
        Esterni_Leggi_Avversita_Demetra = 508
        Esterni_Leggi_Avversita_Da_Codice_Demetra = 509
    End Enum

    Public Enum enum_Disciplinare_Tipo_Testata

        Difesa = 0
        Diserbo = 1
        Fitoregolatore = 2
        Fertilizzazione = 4

    End Enum

    Public Enum enum_Disciplinare_Operazione

        QuelloDellOperazione = 0
        Nessuno = -1
        Biologico = -2
        NessunDpiNessunaEtichetta = -999

    End Enum

    Public Enum enum_TipoOperazione_xAnalisiConformita

        Difesa_Diserbo = 1
        Fertilizzazioni = 2
        Raccolta = 3
        Magazzino = 4

    End Enum

    Public Enum enum_AWS_ApplicazioneRichiedente

        AgronicaAnagrafica = 1
        AgronicaAgenda = 2
        ProfitosanApp = 3

        QdC_BluArancio = 100

        Irrinet_PianoConcimazione = 101

        AgronicaWebApiProfilatore_Token = 102
        AgronicaWebApiProfilatore_Richieste = 103
        AgronicaWebApiProfilatore_Richieste_Estese = 104
        AgronicaWebApiProfilatore_G2G_API = 105

        AgronicaWidgetManager = 106
        NewAgri = 107
        SchedulerQuartz = 108

        'questo proviene dell'execel in \\rubino2\documentazione\AGRONICA --- Developer Vademecum\DistribuzionePorte_vsDevServer_ProgettiAgronica.xlsx
        Manager = 52540
        Agronica_Core_WS = 52548
        Agronica_Stampe_2010 = 52549
        GiasOnline_2010 = 52550
        AgronicaAgenda_2010 = 52551
        AgronicaSincronizzatoreWeb = 52552
        Manager_DPI = 52553
        Manager_Gias = 52554
        AgronicaAnalisi_2010 = 52555
        AgronicaPlanning = 52556
        AgronicaDPI = 52557
        AgronicaWebService = 52558
        PianoConcimazione_2017 = 52559
        AgronicaMeteoWebService = 52560
        AgronicaPianiSemina = 52562
        WS_ImportaGias_2014 = 52563
        AgronicaPianiCampionamento = 52564
        AgroProfilazione = 52565
        AgronicaWebServiceEsterni = 52566
        AgronicaProfitosan_Applicazione_2010 = 52567
        AgronicaSementi_5 = 52568
        AgronicaWebApiProfilatore = 52569
        WS_Agronica_Credenziali = 52570
        WS_Importa_Magazzino_2010 = 52571
        WS_AgroFascicoloBA = 52572
        AgronicaProfitosan_WS = 52572
        GiasBase = 52573
        GAP_Server = 52574
        GiasUpdateServer = 52575

        AgronicaQRWeb = 52580
        AgronicaUMA = 52581
        AgronicaPortal = 52582

    End Enum

    Public Enum enum_WS_Esterni

        WS_Fascicolo_Agrea = 1
        WS_Fascicolo_AnagrafeER = 2
        WS_Fascicolo_BA = 3
        WS_Fascicolo_Agea_Coordinamento = 4
        WS_Fascicolo_Agea_RealTime = 5
        WS_Fascicolo_Grafico_Artea = 6

    End Enum

    ''' <summary>
    ''' Basato su tabella Sistemi_Esterni di Matrice/Metaschema
    ''' </summary>
    Public Enum enum_SistemiEsterni
        ''' <summary>
        ''' Identifica i record creati da interfaccia utente di gias e quindi non provenienti da sistemi esterni
        ''' </summary>
        gias = -1
        enogis = 1
        artea = 2
        smarttractors = 3
        demetra = 4
        agea = 5
        CAI = 6
        NewAgri = 7
        Infragri = 8
        AntaresTrace = 9
        Engine_Sostenibilita = 10
        GiasAPP = 11
        ZootecniaAPP = 12
    End Enum

    Public Enum FF_Etichette_tipo

        DescrizionePerCombo = 999

        Interne = 1
        Imballo = 2
        Confezione = 3
        SSCC = 4

    End Enum

    Public Enum FF_Barcode_Algoritmo_Dati
        Nessuno = 0
        RiportaLotto = 1
        Cofruta = 2
        MiniFrutta = 3
    End Enum

    Public Enum enum_LiquiditaAbilitazione
        NessunaSoloConsultazione = 0
        PartitaDoppiaPagamentiIncassi = 1
        PartitaDoppia = 2
    End Enum

    Public Enum enum_Sincronizzatore_2010
        Sincronizzatore_APOT = 1
    End Enum

    Public Enum enum_WFlow_iMotion
        Scaricato_valutazione = 20000001
        Associato_Raccolta = 20000002
        Non_associato_raccolta = 20000003
    End Enum

    Public Enum enum_iMotion_tipoPercorsoGIS

        percorso = 0
        PuntoDiStop = 1
        UltimaPosizione = 2

    End Enum

    Public Enum enum_AlgoritmoCostiAccessori
        CAB = 1
        SBTF = 2
        BASE = 3
    End Enum

    Public Enum enum_Planning_Fonte

        AnagrafeER = 1
        Agrea = 2
        AvepaB1 = 3
        BluArancioAgea = 4
        BluArancioAvepa = 5
        NotificaBioER = 6
        Sisco = 7
        Brogliaccio_SIAN = 8
        Avepa = 9
        Arpea = 10
        Artea = 11
        AvepaOracle = 12
        SIARL = 13
        AGEA_Coordinamento = 14
        AGEA_RealTime = 15
        AvepaSHP = 16

    End Enum

    Public Enum enum_Programmazione_Entita_Stato_Ribaltamento

        NonDefinito = 0
        DaRibaltare = 1
        Ribaltato = 2

    End Enum

    Public Enum enum_NC_Eventi

        AperturaNC = -1
        ChiusuraNC = -2
        AperturaFaseNC = -3
        ChiusuraFaseNC = -4
        FaseNcInScadenza = -5

    End Enum

    Public Enum enum_Alert_Eventi

        ScadenzaInScadenza = -1

    End Enum

    Public Enum TipoFiltroKendo_colonne

        Menu = 1
        CasellaDiscesa = 2
        CasellaTesto = 3
        CasellaTesto_e_CasellaDiscesa = 4

    End Enum

    Public Enum enum_WFlow_Export_XML_Universale
        Nessuno = 0
        Operazione_Agenda_Creata = 251
        Operazione_Esportabile = 252
        Operazione_Esportazione_In_Corso = 253
        Operazione_Esportata = 254
        Operazione_Modificata = 255
    End Enum

    Public Enum enum_WFlow_XML_Log_Universale
        Nessuno = 0
        Avviato = 256      'Import-Export avviato su questo oggetto
        Completato = 257   'Import-Export completato su questo oggetto
        Errore = 258       'Import-Export non riuscito su questo ogetto
    End Enum

    Public Enum enum_WFlow_Import_XML_Universale
        Nessuno = 0
        File_Importazione_In_Corso = 261
        File_Importazione_OK = 262
        File_Importazione_Errore = 263
        File_Importazione_Sospesa = 264
    End Enum

    Public Enum enum_pf_Export_tipo
        Macchine = 1
        Operatori = 2
        Operazioni = 3
    End Enum

    Public Enum enum_shp_Export_tipo
        LetturaDaSessioneXml = 1
        LetturaDatabase = 2
    End Enum

    Public Enum enum_Gis_LayerElementiGrafici_std
        ANALISI_MAPPE_SATELLITARI = -2
        WMS = -1
        APPEZZAMENTI = 1
        Fabbricati = 2
        CATASTO = 3
        TESTO = 4
        TEMALIBERO = 5
        VULNERABILITA = 6
        ZONAZIONEPAC = 7
        AREEOMOGENEE = 8
        UTILIZZODEISUOLI = 9
        SVILUPPORURALE = 10
        INVISIBILE = 11
        ETTARI_EQUIVALENTI = 12
        CAMPIONAMENTI = 13
        CAMPIONIANALISI = 17
        CAMPI = 18
        IMPIANTI = 19
        Impianti_Pianificati_Entita = 33
        Pianificazioni_Testata = 34
        Dettagli_Precision_Farming = 50
        Mappe_Prescrizione = 51
        Op_Agenda = 54
        Precision = 55
        Dettaglio_Ricetta = 60
        AGENDA_RILIEVI = 63
        Trattori = 66
        Centri_Aziendali = 67
        Fasce_di_rispetto = 86
        Tecnici_in_campo = 87
        Corpi_idrici_superficiali = 88
        Stazioni_Meteo = 89
        MUZ = 90
        Indici_Rischio_Produttivita = 91
        Indice_Erosione = 92
        Indice_CO2 = 93
        Indice_Rischio_Meteo = 94
    End Enum

    Public Enum enum_Gis_LayerElementiGrafici_speciali
        Catasto = -1
        Impianti = -2
    End Enum

    Public Enum enum_Meteo_Tiposorgente
        Gias_RER = 0
        RetiPartner = 1
        Aziendali = 2
        Gias_RER_Quadranti = 3
        Pubbliche = 4
    End Enum

    Public Enum enum_Meteo_TipoVisibilità
        Aperta = 0
        Chiusa = 1
    End Enum

    Public Enum enum_Meteo_Agronica_Fornitori_Cod

        WiNet = 1
        Codima = 2
        Metos = 3
        Agronomica30_Test = 4
        Racca_Test = 5
        Agricultural_Support = 6
    End Enum

    Public Enum enum_Meteo_AgriculturalSupport_Nomi

        sistemi = 1
        unita = 2
        sensori = 3

    End Enum

    Public Enum enum_Meteo_Winet_Nomi

        Reti = 1
        Nodi = 2
        Sensori = 3

    End Enum

    Public Enum enum_MailTipo
        NonConformita_AperturaNC = 1
        NonConformita_ChiusuraNC = 2
        NonConformita_AperturaFase = 3
        NonConformita_ChiusuraFase = 4
        NonConformita_FaseInScadenza = 5
        NotificheSuInterferenze = 6
        Scadenze_InScadenza = 7
        ProfilazioneUtenti = 8
    End Enum

    Public Enum enum_ModelliPrevisionali_TipoVisibilita

        ModelloAutorizzato = 0
        VerificaUlterioreNecessaria = 1

    End Enum

    Public Enum enum_ModelliPrevisionali
        Comparsa_Ruggine = 1
        Sviluppo_Ruggine = 2
        Comparsa_Cercospora = 3
        Sviluppo_Cercospora = 4
        Comparsa_Peronospora = 5
        Comparsa_Oidio = 6
        Sviluppo_Oidio = 7
        Calcolo_Ipi = 8
        Calcolo_Peronospora = 9
        Ritardo_Variabile = 10
        Sommatoria_gradi_giorno_su_dati_orari = 11
        Sommatoria_gradi_giorno_su_dati_giornalieri = 12
        Somme_Unità_Termiche = 13
        Calcola_Ticchiolatura_A_Scab = 14
        Bugoff = 15

        Agronomica30_Peronospora = 16
        Agronomica30_BatteriosiKiwi_PSA = 17

        Racca_PeroPom = 18

        Agronomica30_OidioVite = 19
        Agronomica30_BotriteVite = 20

        Agronomica30_TicchiolaturaMelo = 21

        Racca_AlterPom = 22
        Racca_OidioPom = 23
        Racca_BotriPom = 24
        Racca_PeroBiet = 25
        Racca_OidioBiet = 26
        Racca_CercoBiet = 27
        Racca_PeroPat = 28
        Racca_AlterPat = 29
        Racca_ScleroSoia = 30
        Racca_BrusoneRiso = 31

        Agronomica30_MRV_Eulia = 32
        Agronomica30_MRV_CydiaMolesta = 33
        Agronomica30_MRV_Carpocapsa = 34
        Agronomica30_MRV_Helicoverpa = 35
        Agronomica30_MRV_Tignoletta = 36

        Agronomica30_MaculaturaPero = 37

        ProvvisorioOlivo = 38

        Racca_ElmintosporiosiMais = 39
        Racca_BipolarisMaidis = 40
        Racca_AntracnosiOlivo = 41

        Agronomica30_MISP_IPI_Pomodoro = 42

        Racca_MaisMicotox = 43

        UniCatt_Mais_AFLA = 44
        UniCatt_Mais_FER = 45

        Agronomica30_ColpoDiFuoco = 46

        BetaCoProB_Cercosporiosi = 47

        UniCatt_Frumento_Fusariosi = 48

        MRV_PiralideMais = 49
        MRV_NottuaMais = 50

        RaccaFrumento_RuggineBruna = 51     'Puccinia triticina (Ruggine bruna)
        RaccaFrumento_RuggineGialla = 52    'Puccinia striiformis (Ruggine gialla)
        RaccaFrumento_RuggineNera = 53      'Puccinia graminis (Ruggine nera)
        RaccaFrumento_Stagonosporiosi = 54  'Stagonospora nodorum (Stagonosporiosi)
        RaccaFrumento_Septoria = 55         'Zymoseptoria tritici (Septoria)
        RaccaFrumento_Fusariosi = 56        'Fusarium culmorum (Fusariosi)
        RaccaFrumento_FusariosiSpiga = 57   'Fusarium graminearum (Fusariosi della spiga)
        RaccaFrumento_Fusariosi2 = 58       'Fusarium avenaceum (Fusariosi)
        RaccaFrumento_Fusariosi3 = 59       'Fusarium poae (Fusariosi)
        RaccaFrumento_MarciumeRosa = 60     'Microdochium nivale (Marciume rosa invernale)

    End Enum

    Public Enum enum_Gis_FlagGPS

        DisegnatoSuCartografia = 0
        GiasPalm = 1
        CaricatoDaSorgenteEsterna = 2
        MisuratoSuSmartPhone = 3

    End Enum

    Public Enum TipoImportatore
        Importatore_Sconosciuto = 0
        Importatore_Calibratrice = 1
        Importatore_Campionatrice = 2
    End Enum

    Public Enum statoImportazione
        fileImportato = 19000001
        Verificata = 19000004
        Errori_in_Fase_di_Verifica = 19000005
        Importato_In_GIAS = 19000002
        Errori_Durante_Importazione_In_GIAS = 19000003
    End Enum

    'Errori gravi enum negativo, avvisi enum positivo
    Public Enum ErroriImportazione
        PercentualeNon100 = -7
        LottoImportato = -6
        CalibroNonTrovato = -5
        TestataGrigliaNonTrovata = -4
        RigaBollaNonTrovata = -3
        BollaNonTrovata = -2
        ProduttoreNonTrovato = -1
        NessunErrore = 0
        DettaglioNonEsiste = 1
        ProduttoreDiverso = 2
        LottoCampionato = 4
        CalibroNonImportato = 5
        CalibroDuplicato = 6
        OrdinamentoNonRispettato = 7
    End Enum

    Public Enum enum_SostenibilitaTipoAnalisi

        MassimalisuSingoliImpianti = 1
        AnalisiDistribuzioneTerritoriale = 2
        AnalisiPerProdottoCommerciale = 3
        AnalisiPerProdottiFertilizzanti = 4

    End Enum


    Public Enum enum_TipologieUtenti

        QDemetra = -17
        ProfitosanServerSide = -11
        Profitosan = -10
        QStandard = -1
        QPlus = -2
        QBio = -3
        QFert = -4
        QMaps = -5
        Condizionalita = -6
        RBase = -30
        RPlus = -31

    End Enum

    Public Enum enum_PC_Anteprima_wizardComportamentoWS

        LeggiWebService = 1
        LavoraDisconnnesso = 2

    End Enum

    Public Enum enum_Regioni
        Piemonte = 1
        Valle_d_Aosta = 2
        Lombardia = 3
        Trentino_Alto_Adige = 4
        Veneto = 5
        Friuli_Venezia_Giulia = 6
        Liguria = 7
        Emilia_Romagna = 8
        Toscana = 9
        Umbria = 10
        Marche = 11
        Lazio = 12
        Abruzzo = 13
        Molise = 14
        Campania = 15
        Puglia = 16
        Basilicata = 17
        Calabria = 18
        Sicilia = 19
        Sardegna = 20
    End Enum

    Public Enum enum_Modalita_Stampa

        MostraAnteprimaCrystal = 1
        MostraPDF = 2

    End Enum

    Public Enum enum_PersonalizzazioniCSS

        Coldiretti = 1000

    End Enum

    Public Enum enum_FormattazioneExcel
        FE_Non_Specificato = 0
        FE_Text = 1
        FE_Short_Date = 2          '01/03/1998
        FE_Long_Date = 3           '01-mar-1998
        FE_Number_NO_Decimal = 4
        FE_Number_1_Decimal = 5
        FE_Number_2_Decimal = 6
        FE_Number_3_Decimal = 7
        FE_Percent_NO_Decimal = 8
        FE_Percent_2_Decimal = 9
        FE_Short_Time = 10          '5:16
        FE_Medium_Time = 11         '5:16 am
        FE_Long_Time = 12           '5:16:21:00
    End Enum

    Public Enum enum_TipoCampionamento
        TC_Manuale = 0
        TC_Automatico = 1
        TC_MediaBolleImportate = 2
        TC_MediaReferenza = 3
    End Enum

    Public Enum enum_FormuleFisseLiquidazioneFF
        FFL_Nessuna = 0
        FFL_ResiduoSeccoBorlotto_45_50 = 1
    End Enum

    Public Enum agronicacoreparametri_tipoDB
        Server = 1
        Utenti = 2
        SuperServer = 3
    End Enum

    Public Enum agronicacoreparametri_tipologia
        Standard = 0
        WineMatch = 1
        Vinificazione = 2
        Gestionale = 3
    End Enum

    Public Enum enum_MeteoLetturaDati
        Orari = 0
        Giornalieri = 1
        Entrambi = 2
    End Enum

    Public Enum enum_PropagazioneSalvataggioPoligono
        NessunaPropagazione = 0
        Appezzamento = 1
        Impianto = 2
        ImpiantoEdAppezzamento = 3
        ImpiantoSuAppezzamentoEsistente = 4
    End Enum

    Public Enum enum_Gis_AssociaSuperficieA
        Nessuno = 1
        Impianto = 2
        ImpiantoEdAppezzamento = 3
        Appezzamento = 4
        Catasto = 5
    End Enum

    Public Enum enum_Gis_TipoOggetto
        NonSpecificato = 0
        Poligono = 1
        Punto = 2
        LineString = 3
        Mulitpolygon = 4
        Raster = 100
    End Enum

    Public Enum enum_TipoArrotondamentoFF
        Nessuno = 0
        Kg = 1
    End Enum

    Public Enum enum_Tipo_CausaliTrasporto
        Non_Impostato = -999
        Invisibile = -1
        Tutti_Doc_Contabili = 0
        Doc_Contabili_Attivi = 1
        Doc_Contabili_Passivi = 2
        Accettazione_Beni = 3
        MVV_Elettronico = 4
        Contratti_Affitto = 5
    End Enum

    Public Enum enum_CausaliTrasporto
        VENDITA = 1
        ACQUISTO = 2
        CONTO_ACQUISTO = 5
        CONTO_VENDITA = 6
        CONTO_CONFERIMENTO = 8
        CONTO_LAVORAZIONE = 10
        CONTO_ESSICAZIONE = 21
        RESO_DA_CONTO_ESSICAZIONE = 22
    End Enum

    Public Enum enum_Rapporti_Contabili_Standard
        Legale_Rappresentante = -1
        Cliente = -2
        Fornitore = -3
        Dipendente = -4
        Terzista = -5
        Tecnico = -6
        Centro_Revisione_Manutenzione_Macchine = -7
        Laboratorio_Analisi = -8
        Socio = -9
        Trasportatore = -10
        Tecnico_Responsabile = -12
        Agente = -13
        Referente_Aziendale = -14
        Consulente = -15
        Spedizioniere = -16
        Vivaio = -17
        Conferente = -18
        Operatore_Lab_Controllo_Qualita = -19
        Capo_Area = -20
        Avventizio = -21
        Smaltitore = -22
        Fornitore_Ortofrutta = -24
        Organismo_Referente = -25
        Rappresentante_Fiscale = -26
        Coadiuvante_Familiare = -27
        Organismo_Di_Controllo = -28
        Riferimento_Trasferimento_Dati = -29
        Fornitore_Agrofarmaci = -30
        Allevatore = -31
        Macello = -32
        Veterinario = -33
        Referente_Conferimento = -34
        Ditta_Sementiera = -35
        Trattorista = -36
        Dirigente = -37
        Impiegato_Amministrativo = -38
        Addetto_Punto_Vendita = -39
        Autista = -40
    End Enum

    Public Enum enum_PUA_Tipo

        Completo = 1
        Semplificato = 2

    End Enum

    Public Enum enum_PUA_TipoFertilizzante

        Nessuno = 1
        Ammendante = 2
        Compost = 3
        Palabile = 4
        Liquame = 5
        Organo_minerale = 6
        Lenta_cessione = 7
        Chimico = 8
        Correttivo = 9

    End Enum

    Public Enum enum_PUA_Effluente

        Liq_Zo_tq = 1
        Liq_Zo_ch = 2
        Amm = 3
        Pal_Zo = 4
        Liq_di_tq = 5
        Liq_di_ch = 6
        Pal_di = 7
        Comp = 8
        Cmb = 9

    End Enum

    Public Enum enum_PUA_TipoStampa

        TipoStampaPUA_Pianificazioni = 1
        TipoStampaPUA_Fertilizzazioni = 2

    End Enum

    Public Enum enum_PUA_Modalita
        Modalita_PianoDistribuzione = 0
        Modalita_Verifica = 1
    End Enum

    Public Enum enum_Deroga_Teleregistri
        Regola_Base = 0
        Trasporti_Vs_Propria_Cantina = 1
        Vendita_Riepilogativa_Condizionato = 2
        Corrispettivo = 3
        Trasferimento_Sfusi_Proprio_Comune = 4
        Trasferimento_Art_25 = 5
        Rettifica_Giacenze = 6
        Condizionamento_Aceti = 7
        Comunicazione_Non_Prevista = 8
        Prodotto_Destinato_Altri_Usi = 9
        Sbottigliamento = 10
        Operazione_Generica = 11
        Rettifica_31_Luglio = 12
    End Enum

    Public Enum enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione
        OperazioniDaQuaderno = 1
        AnalisiJoinPDC = 2
        AnalsiDelTerreno = 3
    End Enum

    Public Enum enum_FormatoZespriExport
        Zespri2016 = 1
        Zespri2018 = 2
    End Enum

    Public Enum enum_FormatoZespriTipoInvio
        EsportazioneCSV = 1
        ChiamataApiTest = 2
        ChiamataApiDefinitiva = 3
    End Enum

    Public Enum enumTipoSocieta
        ALTRO = 0
        SAPA = 1
        SPA = 2
        SRL = 3
    End Enum

    Public Enum enumNumeroSoci
        SU = 0          'Socio Unico
        SM = 1          'Società pluripersonale
    End Enum

    Public Enum enumStatoLiquidazione
        LS = 0          'Società in stato di liquidazione
        LN = 1          'Società non in liquidazione
    End Enum

    Public Enum enumTipoContattoFattura
        Non_Specificato = 0
        Privato = 1             'Contatto Privato
        PA = 2                  'Contatto Pubblica Amministrazione
    End Enum

    Public Enum enum_EsigibilitaIva
        Non_Specificata = 0
        Immediata = 1
        Differita = 2
        Scissione_Pagamenti = 3
        Inversione_Contabile = 4
    End Enum

    Public Enum enum_AP_TipoAccountManagerRequest

        Interna = 1
        Esterna = 2
        RichiestaEstesa = 3

    End Enum

    Public Enum enum_MenuBS_Anagrafica_GruppoEdit

        MeteoDSS = 1

    End Enum

    Public Enum enum_MenuBS_Agenda_PreselezioneTab

        TutteQdC = 1
        Colturali = 2
        ContabiliMagazzino = 3
        Audit = 4
        Zoo = 5
        Macchine = 6
        RicetteOdL = 7
        Brogliaccio = 8

    End Enum

    Public Enum enum_TipoRigaFattura
        NonSpecificato = 0
        DettaglioNormale = 1
        DettaglioRaggruppato = 2
        DescrizioneLibera = 3
        RiepilogoConfezioni = 4
        RiepilogoContenitori = 5
        RiepilogoImballaggi = 6
    End Enum

    Public Enum enum_TipoCdG
        NuovoTipo = 0
        VecchioTipo = 1
    End Enum

    Public Enum enum_OrigineAppCdG
        OreWeb = 0
        OreApp = 1
        OreAppRicongiunte = 2
        OreWebEsportate = 3
        OreAppEsportate = 4
    End Enum

    Public Enum enum_CAA
        Coldiretti = 1
        Confagricoltura = 2
        LegaCoop = 3
        CIA = 4
    End Enum

    Public Enum enum_TipoXml
        NON_SPECIFICATO = 0
        CONTATTI = 1
        PRODOTTI = 2
        PROGETTO = 3
        ORDINE_PRODUZIONE = 4
        UTILIZZO_ORE = 5
        UTILIZZO_ORE_SPLIT = 6
        CONSUMO_MATERIALI = 7
        CONSUMO_MATERIALI_SPLIT = 8
        ORDINE_ACQUISTO = 9
        DDT_RICEVUTO = 10
        DDT_EMESSO = 11
        FATTURE_DOCUMENTALE = 12
        CdC_WBS = 13
    End Enum

    Public Enum enum_TipoOrigineOp
        Nessuno = 0
        Agri = 1
        Zoo = 2
        Imputazioni = 3
    End Enum

    Public Enum enum_DirezioneFile
        Nessuno = -1
        S2G = 0     'SAP --> GIAS
        G2S = 1     'GIAS --> SAP (Aboca)
        G2A = 2     'GIAS --> ADHOC (Borgoluce)
        G2E = 3     'GIAS --> EURESYS (Aboca)
        M2G = 4     'Manara --> GIAS (anche tutti gli altri che passano da WS import contatti e movimenti usano questo codice)
        G2Z = 5     'GIAS --> Zuffellato (BF)
        Z2G = 6     'Zuffelato --> GIAS
        G2BC = 7    'GIAS --> Business Center (Iniziative Biometano)
        C2G = 8     'Cai --> GIAS
        G2D = 9     'Gias --> Demetra
        D2G = 10    'Demetra --> Gias
        F2G = 11    'Farmacie CSV --> Gias
    End Enum

    Public Enum enum_StatoMapping
        Fallito = -2
        ParzialmenteRiuscito = -1
        NonNecessario = 0
        Necessario = 1
        Riuscito = 2
    End Enum

    Public Enum enum_TipoNomeFile
        Nessuno = -1
        CONT = 0    'Anagrafica Contatti
        PROD = 1    'Anagrafica Prodotti
        MOVI = 2    'Movimenti
        UORE = 3    'Utilizzo Ore
        CNSM = 4    'Consumo Materiali
        PROG = 5    'Progetto
        ORPR = 6    'Ordine di Produzione
        ORDA = 7    'Ordine di Acquisto
        DDTR = 8    'DDT Ricevuto
        DDTE = 9    'DDT Emesso
        CONF = 10   'Conferimento
        FATT = 11   'Fatture
        CDCW = 12   'CDC / WBS
        PAGA = 13   'Pagamenti
    End Enum

    Public Enum enum_avpListTestata
        Nessuno = 0
        GESTIONE_VETTORE = 1
        ASPETTO = 2
        TOTALE_IMPONIBILE = 3
        TOTALE_IMPOSTE = 4
        STORNO = 5
        ID_DOC = 6
        ORDINE_CHIUSO = 7
        NOTE = 8
        DEFINITIVO = 9
    End Enum

    Public Enum enum_avpListAgente
        Nessuno = 0
        COMMISSIONE = 1
    End Enum

    Public Enum enum_avpListDettaglio
        Nessuno = 0
        IVA_COD = 1
        PREZZO_UNITARIO = 2
        PREZZO_UNITARIO_SCONTATO = 3
        IMPONIBILE_PRODOTTO = 4
        MAGAZZINO_PROVENIENZA = 5
        CELLA_PROVENIENZA = 6
        COD_OP = 7
        TIPO_RIGA = 8
        GRADO_ALCOLICO = 9
        COMMISSIONE_PERC = 10
        RIFERIMENTO_MOV = 11
        STORNO_RIFERIMENTO_MOV = 12
        COD_COMMESSA = 13
        COD_TRAVERSO = 14
        SUPERFICIE = 15
        DESCR_CATEGORIA = 16
        CdC = 17
        Commessa_Wbs = 18
        Fattura_Esterna = 19
        TIPO_PARAMETRO_1 = 20
        VALORE_PARAMETRO_1 = 21
        TIPO_PARAMETRO_2 = 22
        VALORE_PARAMETRO_2 = 23
        TIPO_PARAMETRO_3 = 24
        VALORE_PARAMETRO_3 = 25
    End Enum

    Public Enum enum_TipologieSementi

        Semente = 1
        Bulbi = 2
        TuberiRadici = 3
        PiantineDaOrto = 4
        Talee = 5
        PiantineFlorico = 6
        altre = 7
        Astoni = 8
        Selvatico = 9
        Barbatelle = 10
        GD = 11
        AstoneNudo = 12
        AstoneSperonato = 13
        Astone2 = 14
        AstoneRamificato = 15
        AstoneKnip = 16
        AstoneInnestato = 17
        AstoneTalea = 18
        AstoneMeristema1Anno = 19
        AstoneMeristema2Anno = 20
        Microinnesto = 21
        Microinnesto2 = 22
        VasoFermo = 23
        VasoVegetante = 24

    End Enum

    Public Enum enum_ModalitaDocContabile
        Nessuno = 0
        Ricevimento_Prodotto = 1
        Ricevimento_Ortofrutta = 2
    End Enum

    Public Enum enum_ModalitaProtetta
        Nessuna = 0
        Protetta = 1
    End Enum

    Public Enum enum_CodificaAgeaDaGias_livelloDettaglio
        NonDefinito = 0
        CultivarPiCodici = 1
        Cultivar = 2
        SpecieVegetale = 3
        CodiciAnagrafe = 4
    End Enum

    Public Enum enum_RMA_formatoImportazione
        MRL_UE = 1
        Homologa = 2
        Zespri = 3
    End Enum

    Public Enum enum_Livello_Gestione_Contabilita
        NonGestita = 0
        Fatturazione = 1
        Bilancio = 2
    End Enum

    ''' <summary>
    ''' Sul LAN è: enModalitaScadenzaOrdine
    ''' </summary>
    Public Enum enum_ModalitaScadenzaOrdine
        UNICA = 0
        UNICA_TASSATIVA = 1
        SCAGLIONATA = 2
        SCAGLIONATA_TASSATIVA = 3
    End Enum

    Public Enum enum_StatoOrdine
        INDEFINITO = 0
        INEVASO = 1
        EVASO = 2
        PARZIALMENTE_EVASO = 3
        NON_PRONTO = 4
        EVASO_FORZATAMENTE = 5
    End Enum

    Public Enum enum_TipoDocumento_Numerazione
        Precedente = 1
        Successivo = 2
    End Enum

    Public Enum enum_Fruttagel_CatCod
        OLD = -1
        MATERIA_PRIMA_AGRICOLA = 1  'Codice.StartsWith("60")
        IMBALLI_Conferimento = 2
        POMODORO = 3    'Codice.StartsWith("3")
        SURGELATI = 4   'Codice.StartsWith("4")
        IMBALLI = 5 'Codice.StartsWith("5")
        Codici61 = 6  'Codice.StartsWith("61")
        GADGET = 7 'Codice.StartsWith("710")
        LABCQ_codici_fittizi = 8 'Codice.StartsWith("CQ")
        VARIE = 9 'tutti gli altri
        BEVANDE = 10    'Codice.StartsWith("1")
        SL_acquistati = 25 'Codice.StartsWith("250")
    End Enum

    Public Enum enum_PassaportiVivaiTipoGriglia
        InserimentoDati = 1
        Stampa = 2
    End Enum

    Public Enum enum_Tipo_Stampa_Statistica

        Statistica_mese_anno_confronto_fra_anni = 0
        Statistica_mese_singolo_anno_su_quantità_altro_Valore = 1
        Statistica_anno_con_scostamento_e_previsioni = 2

    End Enum

    Public Enum enum_Tipo_Report_BIO
        Scheda_Materie_Prime = 21
        Scheda_Vendite = 22
        Registro_Preparazioni = 153
        Carichi_Scarichi = 1
        Report_Terzisti = 2
    End Enum

    ''' <summary>
    ''' Tabella Server PDC_Stato_Analisi
    ''' </summary>
    Public Enum enum_PDC_Stato_Analisi
        Analisi_In_Corso = 1
        Analizzata = 2
        Da_Inviare = 3
        In_Lavorazione = 4
    End Enum

    Public Enum enum_PDC_Stato_Validazione
        Non_Validata = 0
        Validata = 1
    End Enum

    Public Enum enum_PDC_Stato_Pubblicazione
        Non_Pubblicata = 0
        Da_Pubblicare = 1
        Pubblicata = 2
        Da_Rimuovere = 3
    End Enum

    Public Enum enum_PDC_Tipo_Risultato
        Singolo_PA = 1
        Famiglia_PA = 2
        Merceologica = 3
    End Enum

    Public Enum enum_PDC_Stato_Testata
        Nessun_Impianto_Selezionato = 0
        Impianti_Selezionati = 1
        Lotti_Fitosanitari_Selezionati = 2
        Impianti_Da_Campionare_Selezionati = 3
        Impianti_Da_Campionare_E_Lotti_Selezionati = 4
        Workflow_Analisi = 5
        Richiesta_Analisi_Zoo_Inviata = 6
        Analizzato = 7
    End Enum

    Public Enum enum_PDC_Stato_Campione
        Da_Campionare = 0
        Campione_Eseguito = 1
        Campione_Inviato_A_Laboratorio = 2
        Analisi_Campione_Eseguita = 3
    End Enum

    Public Enum enum_TipoEntitaImputazione
        COSTI = 1
        RICAVI = 2
        MACCHINE = 3
        LINEE_PRODUZIONE = 6
        IMPIANTI = 7
        MATERIE_PRIME = 8
        PRODOTTI = 9
        SEZIONALI = 10
        RISORSE_UMANE = 11
        MEZZI_TECNICI = 12
        ALTRE_RISORSE = 13
        BENI_CONFEZIONAMENTO = 14
        ALTRI_BENI_AMMORTIZABILI = 15
    End Enum

    Public Enum enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica
        AGRONICA = 0
        HOMOLOGA = 1
        ZESPRI = 2
    End Enum

    ''' <summary>
    ''' dalla tabella Lingue in matrice, mantenere allineata se si aggiungono lingue nuove
    ''' </summary>
    Public Enum enum_AgroLingue

        Italiano_it = 1
        inglese_en = 2
        francese_fr = 3
        Italiano__Svizzera__IT_ch = 4
        Portoghese_PT = 5

    End Enum

    ''' <summary>
    ''' dalla tabella GruppoVegetale di metaschema
    ''' </summary>
    Public Enum enum_GruppoVegetale
        NonDefinito = 0
        Arboree = 1
        Erbacee = 2
        OrtoFloroVivaismo = 3
    End Enum
    Public Enum enum_DataProvidersType
        Undefined = 0
        OleDbProvider = 1
        SqlDataProvider = 2
    End Enum

    Public Enum enum_SAML_RiconoscimentoUtente
        CodiceFiscale = 1
        email = 2
        email_nome_cognome = 3
        userID = 4
    End Enum

    Public Enum enum_Tipo_Produzione
        Prodotto_Commercializzato = 0
        Produzione_Propria = 1
    End Enum

    Public Enum enum_Livello_Log_Applicazioni
        Nessuno = 0
        LogSoloSuFile = 1
        LogSoloSuEmailEDB = 2
        LogFileEmailEDB = 3
        LogSoloDB = 4
        LogFileEDB = 5
        LogSoloEmail = 6
        LogFileEmail = 7
    End Enum

    Public Enum enum_TipoEntitaJohnDeere
        Account = 0
        Organization = 1
        Client = 2
        Farm = 3
        Field = 4
        Boundary = 5
        Asset = 6
        Machine = 7
        Operation = 8
        File = 9
    End Enum
    Public Enum enum_TipoChiaveJohnDeere
        ChiaveGias = 1
        ChiaveJD = 2
    End Enum

    Public Enum enum_StatoSincronizzazioneJohnDeere
        Sincronizzato = 1
        CampoJohnDeereinGIASnonSincronizzato = 2
        SoloInJohnDeere = 3
        AppezzamentoGIASNonSincronizzato = 4
    End Enum

    Public Enum enum_ShapeTypeMappaProduzioneJophnDeere
        Point = 1
        Polygon = 2
    End Enum

    Public Enum enum_ResolutionMappaProduzioneJophnDeere
        EashSection = 1
        EachSensor = 2
        OneHeartz = 3
    End Enum

    'Lotto Config Gias FF
    Public Enum enLotto_Config_FF
        lcff_CODICE_FORNITORE = 1
        lcff_DATA = 2
        lcff_ORA = 3
        lcff_LOTTO_IMPIANTO = 4
        lcff_DOCUMENTO = 5
        lcff_AZIENDA = 6
        lcff_CODICE_DESTINAZIONE = 7
        lcff_SIGLA_SPECIE_VARIETA = 8
        lcff_CONTATORE_UNIVOCO = 9
        lcff_ACCETTAZIONE = 10
        lcff_ANNO_AA = 11
        lcff_CODICE_LINEA = 12
        lcff_CODICE_ARTICOLO = 13
        lcff_CONTATORE_UNIVOCO_PARAMETRI = 14 'specie/varietà/calibro qualità
        lcff_ANNO_SOCIO_LINEA_GAP = 15
        lcff_NUMERO_SETTIMANA = 16
        lcff_GIORNO_GIULIANO = 17
        lcff_TABELLA_ASSEGNA = 18
        lcff_CODICE_APPEZZAMENTO = 19
        lcff_CODICE_SITO_PRODUZIONE = 20
    End Enum

    Public Enum enLotto_Config_Lavorazioni_FF
        lcffProd_CODICE_FORNITORE = 1
        lcffProd_DATA = 2
        lcffProd_CONTATORE_UNIVOCO = 5
        lcffProd_ANNO_AA = 6
        lcffProd_CODICE_LINEA = 7
        lcffProd_CODICE_ARTICOLO = 8
        lcffProd_NUMERO_SETTIMANA = 10
        lcffProd_GIORNO_GIULIANO = 11
        lcffProd_LOTTO_ENTRATA = 12
        lcffProd_CALIBRO = 13
        lcffProd_QUALITA = 14
        lcffProd_LOTTO_TESTATA = 15
        'Per ora commentati forse verrano ripristinati in futuro
        'lcffProd_CODICE_DESTINAZIONE = 3
        'lcffProd_SIGLA_SPECIE_VARIETA = 4
        'lcffProd_CONTATORE_UNIVOCO_PARAMETRI = 9
    End Enum

    Public Enum enum_BordoMacchinaFormati
        TrimbleEZGuide_250_500_750 = 1
        DeereGen4 = 2
        Deere2630 = 3
        Deere2600 = 4
        Deere1800 = 5
        DeereGen2_CommandCenter = 6
        AgGatewayIsoXml = 7
        AgGatewayApplicationDataModel = 8
    End Enum

    Public Enum enum_TipoEsportazioneGisPrecisionFarming
        PrecisionFarmingMonitorBordoMacchina = 1
        ShapeFieEsri = 2
        ReportDatiGrafici = 3
        Gias2JohnDeere = 4
    End Enum

    Public Enum emum_Manager_Performa_DocCommesse_Parametri
        Dati_Sez_Sottocommesse_PM_Assegnato_a_ = 300361 ' Assegnato a   sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_CodiceSottoCommessaTask = 1015 ' Codice sottocommessa  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_CodiceLotto = 1017 ' Codice Lotto  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Data_Apertura = 1021 ' Data Apertura  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Data_Chiusura = 1022 ' Data Chiusura  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Data_fine_chiamate = 130253 ' Data fine chiamate  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Data_Fine_Sviluppo = 1219 ' Data Fine Sviluppo  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_DescrizioneSottoCommessaTask = 1016 ' Descrizione sottocommessa  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_DescrizioneLotto = 1018 ' Descrizione Lotto  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Fatturazione = 300351 ' Fatturazione  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_gg_a_FinireTask = 301230 ' gg a Finire  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_gg_a_FinireTaskXUtente = 301231 ' gg a Finire  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_gg_Previste = 300363 ' gg Previste  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_gg_Stimati = 301229 ' gg Stimati  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Importo_Fatturato = 301131 ' Importo Fatturato  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Importo_iva = 300057 ' Importo+iva  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Importo = 300048 ' Importo  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_N_max_chiamate = 130254 ' N° max chiamate  sezione Sottocommesse
        'Dati_Sez_Sottocommesse_PM_Ore_Previste = 1023 ' Ore Previste  sezione Sottocommesse
        'Dati_Sez_Sottocommesse_PM_Ore_Previste = 300362 ' Ore Previste  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Responsabile = 1020 ' Responsabile  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_StatoSottoCommessaTask = 300350 ' Stato  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_StatoSottoCommessaTaskXUtente = 301228 ' Stato  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Tipo_Lotto = 130153 ' Tipo Lotto  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Tipologia_di_lotto = 260329 ' Tipologia di lotto  sezione Sottocommesse
        Dati_Sez_Sottocommesse_PM_Tipologia_di_Settore = 260328 ' Tipologia di Settore  sezione Sottocommesse
    End Enum

    Public Enum jDeereDataModel_RequestType
        Insert = 0
        Update = 1
    End Enum

    Public Enum FF_Track_Flusso
        Tutti = 0
        GiasWEB = 1
        WebApi = 2
    End Enum

    Public Enum Tipo_Permesso_Documentale
        Gestione_Completa = 1 'Lettura - Modifica - Cancellazione
        Lettura = 2
    End Enum

    Public Enum enum_StatoValidazioneQdc_MarketAccess
        Non_Definito = 0
        Assente = 1
        Non_Validato = 2
        Validazione_in_corso = 3
        Validato = 4
    End Enum

    Public Enum enum_GS1_TipoRigaFat
        NORMALE = 0
        ALTRO = 1       'Altri beni strumentali (501)
        DESCR = 2       'Riga descrizione libera (502)
    End Enum

    Public Enum enum_GS1_tipoScontoFat
        SCONTO_PERCENTUALE = 0
        SCONTO_MERCE = 1
        OMAGGIO_NO_RIVALSA_IVA = 2
        CAMPIONE_GRATUITO = 3
        OMAGGIO_CON_RIVALSA_IVA = 4
        SCONTO_FISSO = 5
    End Enum

    Public Enum enum_Contesto_Integrazione_Macchine_Lavorazione
        'Parametri_Export
        Invio_Primo_Ingresso_Lav = 1
        Invio_Ordine_Lav = 2
        Invio_Primo_Ingresso_Da_Lav_Colleg = 3
        'Parametri_Import
        Ricezione = 100
    End Enum

    Public Enum enum_CtrUnivIdenLav
        NessunControllo = 0
        UnivocoSoloDocAperti = 1
        UnivocoAssolouto = 2
    End Enum

    Public Enum enum_CtrLegameLav
        NessunControllo = 0
        SingoloSoloDocAperti = 1
        SingoloAssoluto = 2
    End Enum

    Public Enum enum_Classi_Attivita
        LavorazioneBase = 1
        LavorazioneConProdotto = 2
        Trattamento = 3
        Fertilizzazione = 4
        Rilievo = 5
        SeminaTrapianto = 6
        Raccolta = 7
    End Enum

    Public Enum enum_Attvita_Consuntivo_Budget
        SoloConsuntivo = 0
        SoloBudget = 1
        Entrambi = 2
    End Enum

    Public Enum enum_Sezioni_MenuBS_2017

        MacroCategoria_Magazzini = 11
        Anagrafiche = 69
        Anagrafiche_Zoo = 72
        Configurazione_Stazioni_Meteo_DSS = 78

        AUDIT_Corpi_Estranei = 30
        BUDGET_Inserimento_Costi = 254
        BUDGET_Analisi_CostiRicavi = 255
        BUDGET_Menu_CdG = 256
        BUDGET_Inserimento_Ricavi = 253

        CDG_Inserimento_Costi = 65
        CDG_Analisi_CostiRicavi = 66
        CDG_Menu_CdG = 67
        CDG_Time_Sheet = 68
        CDG_Split = 101
        CDG_Inserimento_Ricavi = 108
        CDG_Analisi_costi_diretti_1 = 175
        CDG_Time_Sheet_Personale = 316
        CDG_AttivitaInterne = 317
        CDG_Squadre = 319

        'Documenti Contabili - Nuovo
        DocContabile_Nuovo_Ordine_Acquisto = 122
        DocContabile_Nuovo_DDT_Ricevuto = 124
        DocContabile_Nuova_Distinta_Carico = 125
        DocContabile_Nuovo_AutoDDT_Emesso = 126
        DocContabile_Nuova_fattura_Ricevuta = 128
        DocContabile_Nuova_NotaCredito_Ricevuta = 129
        DocContabile_Nuovo_Ordine_Vendita = 131
        DocContabile_Nuovo_DDT_Emesso = 133
        DocContabile_Nuova_Fattura_Emessa = 135
        DocContabile_Nuova_NotaCredito_Emessa = 136
        DocContabile_Nuovo_Conferimento = 140
        DocContabile_Nuovo_Conferimento_Pomodoro = 212
        DocContabile_Nuovo_Carico_Magazzino = 258
        DocContabile_Nuovo_Scarico_Magazzino = 259
        DocContabile_Nuovo_Contratto_Affitto = 285

        'Documenti Contabili - Ricerca
        DocContabile_Ricerca_Ordine_Acquisto = 121
        DocContabile_Ricerca_DDT_Ricevuto = 123
        DocContabile_Ricerca_fattura_Ricevuta = 127
        DocContabile_Ricerca_Ordine_Vendita = 130
        DocContabile_Ricerca_DDT_Emesso = 132
        DocContabile_Ricerca_Fattura_Emessa = 134
        DocContabile_Ricerca_Conferimento = 139
        DocContabile_Ricerca_Contratto_Affitto = 284

        '---
        MAGAZZINI_Nuovo_Trasferimento = 193
        QDC_PROFITOSAN = 191
        QDC_Verifica_Conformita_Quaderno = 22
        QUALITA_Verifica_Conformita_Quaderno = 36
        MENU_NUOVA_VISITE = 91
        QDC_ReportCampagna = 318

        PianiCampionamentiAnalisi = 32
        PianiCampionamento_Zootecnici = 322
        Inserimento_Analisi_Laboratorio = 80

    End Enum

    Public Enum enum_Esportazioni_Sistema_Cod
        Enogis = 1
        Artea = 2
        BDN = 3
        XFarm = 4
        OnPlantImport = 5
        OnPlantExport = 6
        HubIoT = 7
        Nogmo = 8
        Fex = 9
        AsipoImport = 10
        AsipoOrdiniMVExport = 11
        AsipoOrdiniMVGetStato = 12
        Recap_Ricette_CAI = 20
        OrogelBI = 21

        Demetra_Import_Analisi = 22
        Demetra_Export_Analisi = 23

        Demetra_Import_Attivita = 24
        Demetra_Export_Attivita = 25

        Demetra_Import_Fabbricati = 26
        Demetra_Export_Fabbricati = 27

        Demetra_Import_LavoratoriQDC = 28
        Demetra_Export_LavoratoriQDC = 29

        Demetra_Import_Fornitori = 30
        Demetra_Export_Fornitori = 31

        Demetra_Import_Macchine = 32
        Demetra_Export_Macchine = 33

        Demetra_Import_MovimentiMag = 34
        Demetra_Export_MovimentiMag = 35

        Demetra_Import_DataPublish = 36
        Demetra_Export_DataPublish = 37

        Horta = 38

        CAI_Export_Visite = 39

        Horta_Orzo = 40

        Agea_Import_Pendenze = 41

        OnPlantImport_Conferimenti = 42

        Infragri_Export_Stazioni = 43
        AntaresTrace_Export_Masterdata = 44
        AntaresTrace_Export_Operazioni = 45

        App_Import_Attivita = 46

        Demetra_Import_Squadre = 47
        Demetra_Export_Squadre = 48

        App_Import_Ricette = 49
        Demetra_Import_Ricette = 50

    End Enum

    Public Enum enum_GIS_TipoAnalisiBs
        AnalisiMeteo = 1
        AnalisiModelli = 2
        AnalisiRilievi = 3
    End Enum

    Public Enum enum_Opzioni_Raccolta_Aggiornamento_Anagrafica
        LASCIA_ATTIVI = 0

        CHIUDI_ESERCIZI = 1
        CHIUDI_APRI_ESERCIZI = 4

        CHIUDI_IMPIANTI_ESERCIZI = 2
        CHIUDI_APRI_IMPIANTI_TERRENO_NUDO = 5

        CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI = 3
    End Enum

    Public Enum enum_TipoAzienda_UMA
        Azienda_Agricola_Privata = 1 'EX CONTO PROPRIO
        Azienda_Terzista = 2
        Cooperativa_Agricola = 3
        Azienda_Agricola_Istituzioni_Pubbliche = 4
        Consorzio_Bonifica_Irrigazione = 5
    End Enum

    Public Enum enum_TipoCarburante_UMA
        Gasolio_Agricolo = 1
        Gasolio = 2
        Benzina = 3
        Benzina_Agricola = 4
        GPL = 5
        Gas_Metano = 6
        Kerosene = 7
        Gasolio_Serra = 8
        Elettricita = 9
        Carburante_Non_Agricolo = 10
    End Enum

    Public Enum enum_FormeGiuridiche
        'PUBBLICHE
        Universita = 18
        Consorzio_Di_Bonifica = 19

        'PRIVATE
        Imprenditore_Individuale_Agricolo = 1
        Societa_Semplice = 2
        Societa_In_Nome_Collettivo = 3
        Societa_In_Accomandita_Semplice = 4
        Studio_Associato_E_Societa_Di_Professionisti = 5
        Societa_Per_Azioni = 6
        Societa_a_Responsabilita_limitata = 7
        Societa_a_Responsabilita_limitata_Con_Un_Unico_Socio = 8
        Societa_In_Accomandita_Per_Azioni = 9
        Societa_Cooperativa_a_Mutualita_Prevalente = 10
        Societa_Cooperativa_Diversa = 11
        Societa_Cooperativa_Sociale = 12
        Societa_Di_Mutua_Assicurazione = 13
        Societa_Consortile = 14
        Associazione_o_Raggruppamento_temporaneo_Di_Imprese = 15
        Gruppo_Europeo_Di_Interesse_Economico = 16
        Altra_Forma_Di_Ente_Privato_Con_Personalita_Giuridica = 17
    End Enum

    Public Enum enum_WAnagraficaStati
        In_Compilazione = 2001
        Compilazione_Alla_Data_Completata_e_Verificata = 2007

        Rinuncia = 2009

        Verifica_In_Corso = 2002
        Verifica_Completata = 2003 'Verifica completata indica che la fase di verifica si è conclusa ancora senza un verdetto (è precedente allo stato completata con successo)

        Verifica_Intermedia_Completata_Con_Riserva = 2004
        Verifica_Intermedia_Completata_Con_Successo = 2005
        Verifica_Intermedia_Non_Superata = 2006

        Inserimento_Completato_Per_Il_Periodo_Di_Competenza = 2008

        Verifica_In_Corso_Da_Assegnare = 2010
    End Enum

    Public Enum enum_ModificaMultiplaOperazioni

        AggiungiMacchina = -1
        AggiungiContatto = -2
        AggiungiMagazzino = -3
        EliminaImpiantiDoppiMantenendoQtaProdotto = -4
        ModificaxModificaSupImpMantenendoQtaTotaleProdotto = -5
        ModificaxModificaSupImpMantenendoQtaHaProdotto = -6
        EliminaMacchine = -7
        EliminaContatti = -8

    End Enum

    Public Enum enum_UMA_Avanzamento
        Richiesta_Anticipo = -1
        Richiesta = 0
        Rendicontazione = 1
    End Enum

    Public Enum enum_UMA_TipoRichiesta
        Conto_Terzi = -1
        Conto_Proprio = 0
    End Enum

    Public Enum enum_TipoParamQual
        CodiceNumerico = 1
        Numero = 3
        Stringa = 4
        Data = 5
    End Enum

    Public Enum enum_PreparazioneCod
        UnificazioneLotto = -208
    End Enum

    Structure enum_OrigineApp
        Const GiasApp = "APP"
        Const Demetra = "DEMETRA"

        ''' <summary>
        ''' Origine App per i dati provenienti da NewAgri
        ''' </summary>
        Const PUA = "PUA"
    End Structure

    Public Enum enum_IoT_Tiposorgente
        Pubbliche = 0
        Aziendali = 1
    End Enum

    Public Enum enum_HubIoTPlatformDestination
        None = 0
        JohnDeere = 1
        Agrirouter = 2
        AGCO_Trimble = 3
        CNH1 = 4
    End Enum

    Public Enum enum_HubIoT_EntitaOrigine
        None = 0
        Ricetta = 1
        Operazione_Pianificata = 2
        Consiglio_Irriguo = 3
    End Enum

    Public Enum enum_HubIoT_CategoriaLavorazione
        None = 0
        Tillage = 1
        Application = 2
        Seeding = 3
        Harvest = 4
    End Enum

    Public Enum enum_HubIoT_RegoleElaborazione
        None = 0
        InviaPoligonoSenzaShape = 1
        InviaPoligonoSuShape = 2
        InviaMappaDiPrescrizione = 3
        InviaMappaDiPrescrizioneConLineaGuida = 4
    End Enum

    Public Enum enum_HubIoT_StatoElaborazione
        Inserito = 0
        Elaborazione = 1
        Inviato = 2
        Ricevuto_ritorno_lavorazione = 3
        Sospeso = 4
    End Enum

    Public Enum enum_Servizi_Sottoscrizione_Notifiche
        RecuperoCoordinate = 1
    End Enum

    Public Enum Tipo_Importazione_FileShape
        Importazione_Agrea_Crea_Planning = 1
        Importazione_Trimble = 2
        Importazione_VecchiDatiGIAS = 3
        importaCatasto_DXF = 4
        importaGeneric_SHP = 5
        importazione_iMotion = 6
        Importa_KmlKmz = 7
        Importa_Gias2JohnDeere = 8
        Importa_Raster = 100
    End Enum

    Public Enum enum_AuditStatoSQNPI
        PraticaCaricoUfficio = 0
        PraticaProntaAutocontrollo = 3
        AutocontrolloInCorso = 4
        CertificazioneAccettata = 1
        CertificazioneRifiutata = 2
        Receduto = 5
        CertificazioneAccettataNC = 6
    End Enum

    Public Enum enum_AuditStatoBIO
        PraticaCaricoUfficio = 0
        PraticaProntaAutocontrollo = 3
        AutocontrolloInCorso = 4
        CertificazioneAccettata = 1
        CertificazioneRifiutata = 2
        Receduto = 5
        CertificazioneSospesa = 6
    End Enum

    Public Enum enum_AuditStatoWorkflow
        PraticaCaricoUfficio = 500
        PraticaProntaAutocontrollo = 501
        AutocontrolloInCorso = 502
        CertificazioneAccettata = 503
        CertificazioneRifiutata = 504
        Receduto = 506
        CertificazioneSospesa = 505
        CertificazioneAccettataNC = 507
    End Enum

    Public Enum enum_TipoRigaLayer
        LAYER1 = 1
        LAYER2 = 2
        LAYERRISULTATO = 3
    End Enum

    Public Enum enum_AlgoritmoProiezione
        Analisi_Mappe_Satellitari_GEE = 1
        Assessment_NoGo_Areas = 3
        Intact_Forest_Landscape = 4
        Soil_Erosion_by_Water = 7
        Assessment_Risk_Biodiversity = 10
        Potential_Deforestation_Yearly = 14
        Caricamento_Su_Piattaforma_GEE = 12
        Potential_Deforestation_Sync = 13
        EsportazioneBulkLayer = 15
        CalcoloPianoConcimazione = 16
        ProjectionLayerOverLayer = 17
        ProjectionLayerOverRasterGDAL = 18
        Caricamento_Catasto = 19
        Richiamo_WS_Mappe = 20
    End Enum

    Public Enum enum_TipoAlgoritmoProiezione
        Intersezione = 1
        Proiezione_Vettoriale_su_Raster = 2
        Caricamento_Su_Piattaforma_GEE = 4
        Analisi_Mappe_Satellitari_GEE = 5
        Potential_Deforestation_Sync = 7
        Potential_Deforestation_Sync_Yearly = 8
        EsportazioneBulkLayer = 9
        CalcoloPianoConcimazione = 10
        ProjectionOnLayers = 11
        Caricamento_Catasto = 12
        Richiamo_WS_Mappe = 20
    End Enum

    Public Enum enum_TipoOperatoreVisita
        Capo = 0
        Tecnico = 1
        CapoTecnico = 2
        Altro = -1
    End Enum

    Public Enum enum_TipoControllo
        UNDEFINED = 0
        CASELLA_TESTO = 1
        AREA_TESTO = 2
        MENU_DISCESA = 3
        CASELLA_SPUNTA = 4  'Checkbox o boolean switch
        CALENDARIO = 5
        ALLEGATO = 6
        LINK = 7
        PASSWORD = 8
        MENU_DISCESA_FILTRO = 9
        PULSANTE_SCELTA = 10  'Radio button
        IMMAGINE = 11
        DDL_ESTESA_CLIENT = 12
        GIS_VIEWER = 13
        DDL_ESTESA_SERVER_LIGHT = 14
        NUMERO_INTERO = 15
        NUMERO_DECIMALE = 16
        MULTISELECT = 17
    End Enum

    Public Structure enum_Menu_Agenda_NG_Mode
        Const Standard = ""
        Const PUA = "AggiungiAlPua"
    End Structure

    Public Enum enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO
        OLD_NON_APRIRE_NUOVI_ESERCIZI_NUOVI_IMPIANTI = 0 'Gestiti solo nella vecchia Raccolta.aspx
        OLD_APRI_NUOVI_ESERCIZI_NUOVI_IMPIANTI = 1 'Gestiti solo nella vecchia Raccolta.aspx
        LASCIARE_IMPIANTI_ATTIVI_DEFAULT = 10
        CHIUDI_ESERCIZI_DEFAULT = 20
        CHIUDI_APRI_ESERCIZI_DEFAULT = 30
        CHIUDI_IMPIANTI_ESERCIZI_DEFAULT = 40
        CHIUDI_APRI_IMPIANTI_ESERCIZI_DEFAULT = 50
        CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_DEFAULT = 60
        LASCIARE_IMPIANTI_ATTIVI_VINCOLO = 70
        CHIUDI_ESERCIZI_VINCOLO = 80
        CHIUDI_APRI_ESERCIZI_VINCOLO = 90
        CHIUDI_IMPIANTI_ESERCIZI_VINCOLO = 100
        CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO = 110
        CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO = 120
    End Enum

    Public Enum enum_Codifica_Zoo_Barcode
        ZOO_BARCODE_MATRICOLA = 1
        ZOO_BARCODE_DATA_NASCITA = 2
        ZOO_BARCODE_DATA_MOVIMENTO = 3
        ZOO_BARCODE_CERTIFICATO = 4
        ZOO_BARCODE_SESSO = 5
        ZOO_BARCODE_RAZZA = 6
        ZOO_BARCODE_RAZZA_PADRE = 7
        ZOO_BARCODE_RAZZA_MADRE = 8
    End Enum

    'enum per tab Metaschema BDN_Causali
    Public Enum enumCausaliBDN
        ImpAnagraficaBDN = 1
        ImpModello4Uscita = 2
        ImpMovIngressoBDN = 3
        ImpMovUscitaBDN = 4

        InvAnagraficaBDN = 5
        InvMovIngressoBDN = 6
        InvMovUscitaModello4 = 7
        InvMovUscitaBDN = 8
        InvMorteBDN = 9
        ImpModello4Ingresso = 10
    End Enum

    Public Enum enum_TipoPrescrizione
        UNDEFINED = 0
        Veterinaria = 1
        Protocollo_Terapeutico = 2
        Da_Protocollo = 3
        Indicazione_Terapeutica = 4
        Rifornimento_Scorta = 5
        Da_Protocollo_GIAS = 6
    End Enum

    Public Enum enum_StatoTrattamento
        UNDEFINED = 0
        Aperto = 1
        InCorso = 2
        Chiuso = 3
        Chiuso_Anomalia = 4
    End Enum

    Public Enum enum_TipoProdotto_Prescrizione
        UNDEFINED = 0
        Prodotto_Medicinale_AIC = 1
        Prodotto_Omeopatico = 2
        Prodotto_Galenico = 3
        Medicinale_Estero = 4
        Mangime_Completo = 5
        Mangime_Intermedio = 6
        Mangime_Estero = 7
        Mangime_Complementare = 8
    End Enum

    Public Enum enum_Operazioni_Contab_Collegate_QdC
        SCARICO_CARICO = 1
        DDT_EMESSO_DDT_RICEVUTO = 2
    End Enum

    Public Enum enum_MagazzinoEsterno_Tipo
        Agenzia = 1
        Uso_da_Terzi = 2
    End Enum

    Public Enum Enum_FiltroEsportazione_to_ElasticSearch
        Nessuno = -1
        Importati = 0
        Non_importati = 1
        Tutti = 2
    End Enum

    Public Enum enum_TipoAutorizzazioneWS
        Altro = 0
        OAuth2 = 1
        Token_Bearer = 2
        Basic_Authentication = 3
    End Enum

    Public Enum enum_doseQuantitaTotale
        Qta_Totale = 10
        Dose = 11
    End Enum

    Public Enum Enum_Audit_impostazione
        Documentale_GestioneWorkFlow = 1082
        Documentale_GestioneChecklist = 1083
        Audit_Json_Upload_Doc = 1096
        Checklist_Trasporti = 1097

    End Enum

    Public Enum Enum_GruppoOperazioni
        ' TIPO C
        Rilievi_in_Campo = 1
        Rilievi_alla_Raccolta = 2
        Trattamenti = 3
        Lavorazioni = 4
        Altre_Operazioni_Colturali_NonGestite = 5
        Linee_Produzione_Vegetale = 100

        ' TIPO E
        Registrazioni_Contabili = 6
        Gestione_beni_ammortizzabili = 7
        Entrate_ed_uscite = 8
        Scadenze = 9
        Registrazioni = 10
        Utilità = 11

        ' TIPO Z
        Analisi_del_latte = 12
        Anagrafe_animali = 13
        Rilievi_produzioni = 14
        Gestione_mungitura = 15
        Gestione_lettiere = 16
        Gestione_alimentazione = 17
        Altri_lavori_di_stalla = 18
        Linee_Produzione_Animale = 101

        ' TIPO P
        Gestione_Macchine_e_Attrezzature = 19

        ' TIPO V
        Audit_Monitoraggi = 20
    End Enum

    Public Enum enum_ComplianceISCC_Azienda_Controllo
        NotSet = -1
        Esclusa = 0
        Inclusa = 1
    End Enum

    Public Enum enum_GIS_CheckList_Type
        ISCC = 1
    End Enum

    Public Enum Enum_Contesto_PC_Frequenza
        Fertilizzazioni = 1 'JOIN PC_MatriciOrganichexfrequenza
        Precessione_Colturale = 2 'JOIN PC_PrecessioneColturalexFrequenza
    End Enum

    Public Enum enum_MezzoLavorazione
        Distribuzione_Hl = 0
        Distribuzione_Ha = 1
        Distribuzione_Ha_Automatica = 2
    End Enum

    Public Enum enum_DataPublish_Configurazione
        Demetra = 1
        RegioneUmbria = 2
    End Enum

    Public Enum enum_TipoControlloSottoscrizioneServizioQDC
        ControlloStatoPratica = 1
    End Enum

    Public Enum enum_TipoMappeIndiciRischio
        Indice1 = 1
        Indice2 = 2
        Indice3 = 3
        Indice4 = 4
        Indice5 = 5
    End Enum

#Region "Filtro Ricerca NEW"
    Public Enum Enum_TipoMostra_FiltroRicerca

        Aziende = 1
        CentriAziendali = 2
        Campi = 3
        Appezzamenti = 4
        Impianti = 5
        Esercizi = 6
        PianoColturale = 7
        PianoColturaleBudget = 8
        Fabbricati = 9
        Movimenti = 10

    End Enum

    Public Enum Enum_Entita_FiltroRicerca

        Azienda = 1
        CentroAziendale = 2
        Campo = 3
        Appezzamento = 4
        Impianto = 5
        Esercizio = 6
        Fabbricato = 7

    End Enum

    Public Enum Enum_FiltroDestinazioneUso_FiltroRicerca

        Tutto = 0
        SoloDestinazioniUso = 1
        EscludiDestinazioniUso = 2

    End Enum

    Public Enum Enum_FiltroPoligoni_FiltroRicerca

        Tutto = 0
        ConPoligoni = 1
        SenzaPoligoni = 2

    End Enum

    Public Enum Enum_FiltroRipartoCatasto_FiltroRicerca

        Tutto = 0
        ConRiparto = 1
        SenzaRiparto = 2

    End Enum

    Public Enum Enum_ColonnaData_FiltroRicerca

        ValiditaInizio = 0
        ValiditaFine = 1
        IntervalloValidita = 2
        DataCreazioneAnagrafica = 3

    End Enum

    Public Enum Enum_TipoConfronto_FiltroRicerca

        Maggiore = 0
        Minore = 1
        MaggioreUguale = 2
        MinoreUguale = 3
        CompresoFra = 4

    End Enum

    Public Enum Enum_ModalitaFiltroData_FiltroRicerca

        Manuale = 0
        AnnataAgraria = 1
        Oggi = 2

    End Enum

    Public Enum Enum_FiltroOperatoreLogico_FiltroRicerca

        OR_AlmenoUnaCondizioneTrue = 0
        AND_TutteLeCondizioniTrue = 1

    End Enum

    Public Enum Enum_FiltroOperazioniSelezionate_FiltroRicerca

        Tutto = 0
        ConOperazioni = 1
        SenzaOperazioni = 2

    End Enum

    Public Enum Enum_TipoComportamento_FiltroRicerca

        Ricerca = 0
        RicercaAvanzataAzienda = 1
        EsportaPdf = 2
        SelezionamentoEntita = 3
        EsportaExcel = 4

    End Enum

#End Region

    Public Enum enum_XSS_Detection_Config
        Nessun_Controllo = 0
        Logga_Chiamate_Malevole = 1
        Blocca_Chiamate_Malevole = 2
        Blocca_Logga_Chiamate_Malevole = 3
    End Enum

    Public Enum ContributeType
        Anything = 0
        ACA = 1
    End Enum

    ''' <summary>
    ''' Visual Basic errors are in the range 0-65535; the range 0-512 is reserved for system errors; the range 513-65535 is available for user-defined errors.
    ''' </summary>
    Public Enum GiasError As Integer
        googleGeolocationNoDataFound = 513
        googleGeolocationWrongLocation = 514
        googleMissingAPIKey = 515
    End Enum

    Public Enum Enum_ParametriModificaMultiplaPianoColturale
        dummy = -99

        IMP_Finalita = 1
        ESE_DisciplinareMassimaleNPK = 2
        IMP_Varieta = 3
        IMP_GruppoVarietale = 4
        APP_MetodoProduzione = 5
        IMP_Copertura = 6
        ESE_Resa = 7
        ESE_DataSemina = 8
        ESE_DataRaccolta = 9
        ESE_DataFioritura = 10
        ESE_CapitolatoPrivato = 12
        ESE_OrganismoReferente = 13
        ESE_MagazzinoConferimento = 14
        ESE_Certificazione = 15
        ESE_LimiteN = 16
        ESE_LimiteP = 17
        ESE_LimiteK = 18
        IMP_ImpIrrigazione = 19
        ESE_Regolamento = 20
        ESE_DPI = 21
        IMP_SuFila = 22
        IMP_TraFila = 23
        IMP_FormaAllevamento = 24
        IMP_Portinnesto = 25
        IMP_DataInizioPortinnesto = 26
        APP_DataFineAppezzamento = 27
        IMP_DataFineImpianto = 28
        APP_NrAppBio = 29

        ESE_FlagSecondoRaccolto = 30
        ESE_CertificazioneAziendale = 31
        ESE_Contributi = 32
        ESE_CertificazioneProdotto = 33
        ESE_Residuo = 34
        ESE_LicenzaColtivazione = 35
        ESE_RiferimentoTrasferimentoDati = 36
        ESE_Tecnico = 37
        ESE_PianoSemina = 38
        ESE_Prodotto = 39
        IMP_DataInizioImpianto = 40

    End Enum

    Public Enum Enum_EntitaModificaMultiplaPianoColturale
        Appezzamenti = 1
        Impianti = 2
        Esercizi = 3
    End Enum

    Public Enum Enum_ModEserciziModificaMultiplaPianoColturale
        AttiviAllaDataOdierna = 1
        TuttiGliEsercizi = 2
        EserciziAttiviAllaDataX = 3
    End Enum

    Public Enum Enum_ProprietarioContattoAzienda
        Azienda_SuperUser = 1
        Prima_Azienda_Padre_Gerarchia = 2
        Azienda_Corrente_Durante_Creazione = 3
    End Enum

    Public Enum Enum_ElencoTipoEntita
        Impresa = 1
        ContattoGenerico = 2
        ContattoSpecifico = 3
        SpecieVegetale = 4
        MacchinaGenerica = 5
        CentroAziendale = 6
        Campo = 7
        Appezzamento = 8
        MacchinaSpecifica = 9
    End Enum

    Public Enum Enum_AlgoritmoClusteringGis
        CanoPy = 1
    End Enum

    Public Enum Enum_OrigineRichiestaVerificaConformita
        verifica_massiva_webservice = 0
        verifica_massiva_engine = 1
        gsb_massivo_webservice = 2
        gsb_massivo_engine = 3
        operazione_edit_salvataggio_ng = 4
        operazione_edit_verifica_ng = 5
    End Enum

    Public Enum AnalysisRequestStatus
        RequestInError = -1
        ToProcess = 1
        Processing = 2
        Completed = 3
    End Enum
End Class

