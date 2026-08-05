

Public Class TipiEnumerativi

    Public Enum enum_Classi_Attivita
        LavorazioneBase = 1
        LavorazioneConProdotto = 2
        Trattamento = 3
        Fertilizzazione = 4
        Rilievo = 5
        SeminaTrapianto = 6
        Raccolta = 7
    End Enum

    Public Enum enum_TipoMezzo
        Indefinito = -1
        Ettolitro = 0
        Ettaro = 1
        Ora = 2
        Mensile = 3
        Complessivo = 4
        Chilometro = 5
    End Enum

    Public Enum enum_TipoModalitaDistribuzione
        Totale = 10
        Dose = 11
    End Enum

    Public Enum enum_TipoDebug

        Off = 0
        Soft = 1
        Verbose = 2

    End Enum

    Public Enum enum_Udm_Base
        NonSpecificata = 0
        kg = 2
        litri = 29
    End Enum

    Public Enum enum_RilevamentoMagazzinoTipo
        carico = 1
        scarico = -1
        giacenza = 0
    End Enum

    Public Enum enum_TipoRicetta_DB
        Standard = 0
        Standard_Destinazioni = 5
    End Enum

    Public Enum enum_TipoVisita_DB
        Standard = 0
        Rilievo = 1
    End Enum

    Public Enum enum_Impostazioni
        UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO = 81
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI = 181
        UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP = 183

        SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE = 819
        SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA = 820
        SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI = 821
        SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA = 822
        SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA = 832
        SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA = 833

        SUPERUSER_GiasAPP_NUOVA_RICETTA = 827
        SUPERUSER_GiasAPP_NUOVO_INTERVENTO = 828
        SUPERUSER_GiasAPP_INTERVENTI_DA_FARE = 829
        SUPERUSER_GiasAPP_SCARICO_ORE = 830
        SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = 831
        SUPERUSER_GiasAPP_MAX_AZIENDE = 835
        SUPERUSER_GiasAPP_ENTRATAUSCITA = 850
        SUPERUSER_GiasAPP_LAMIAPOSIZIONE = 851
        SUPERUSER_GiasAPP_VISITE = 852
        SUPERUSER_GiasAPP_FREQUENZARILIEVO_MINUTI = 853
        SUPERUSER_GiasAPP_FREQUENZASINCRO_MINUTI = 854
        SUPERUSER_GiasAPP_IMPOSTAZIONI = 860
        SUPERUSER_GiasAPP_DOCUMENTI = 861
        SUPERUSER_GiasAPP_RILIEVI = 862
        SUPERUSER_GiasAPP_GIS = 863
        SUPERUSER_GiasAPP_InCab = 864
        SUPERUSER_GiasAPP_Permessi = 867

    End Enum

    Public Enum enum_TipoRicetta_APP
        Ricette = 1
        InterventiScaricatiDaServer = 2
        InterventiScaricatiDaServerEdApplicati = 3
        InterventiNatiSuAPP = 4
        InterventiScaricatiDaServerEdApplicati_o_NatiSuAPP = 5
        RilieviNatiSuAPP = 6
        NatiSuAPP = 7
    End Enum

    Public Enum enum_TipoOperazioneDB
        Lettura = 0
        Scrittura = 1
        Modifica = 2
        Cancellazione = 3
        Trasferimento = 4
        Copia = 10
    End Enum

    Public Enum enum_CategorieMagazzino

        RIGA_DESCRIZIONE_LIBERA = 502
        ALTRI_BENI = 501
        SERVIZI = 555

        CORPI_ESTRANEI = -50
        CALI_LAVORAZIONE = -1

        ELEMCOD_MANODOPERA = 0
        MACCHINE = 1
        CARBURANTI = 2

        RIFIUTI = 4

        FERTILIZZANTI = 3
        FORMULATI = 191
        COADIUVANTI = 195
        INSETTI = 196
        TRAPPOLE = 197
        INNESCHI = 198

        SEMENTI = 10
        ALTRE_MATERIE = 200
        ZOO_CONSISTENZA = 300

        SEMILAVORATI_VEGETALI = 201
        MATERIE_VEGETALI = 204
        BENI_CONFEZ_VEGETALE = 205
        TRASFORMATI_VEGETALI = 210

        SEMILAVORATI_ANIMALI = 301
        MATERIE_ANIMALI = 304
        BENI_CONFEZ_ANIMALE = 305
        TRASFORMATI_ANIMALI = 310

        MANGIMI = 306
        FARMACI = 307

        CONFEZIONI_PRODOTTI = 400
        RICAMBI = 401
        CAT_MAG_SERVIZI_PROFESSIONALI = 700

    End Enum

    Public Enum enum_WWorflow_WAnagraficaStati
        Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire = 300
        Esecuzione_ed_avanzamento_delle_ricette_Eseguita = 301
    End Enum

    Public Enum enum_ModalitaGestione
        InterventiDefault = 0
        InterventiMagazzino = 1
    End Enum


    Public Enum enum_TipoNodo
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
        precision = 64
    End Enum

    Public Enum enum_Dati_App
        Attivita = 10
        AttivitaCdG = 11
        Ricette = 12
        Rilievi = 20
        Visite = 30
        Documenti = 40
        Movimenti = 50
        Acquisti = 51
        Manutenzioni = 60
    End Enum

    Public Enum enum_Tipo_Operazione_Agenda_Target
        Reale = 1
        Planning = 2
    End Enum

    Public Enum enum_Tipo_Operazione_Agenda
        QuadernoDiCampagna = 1
        Ricetta = 2
        RicettaBrogliaccio = 3
    End Enum

    Public Enum enum_MagazzinoEsterno_Tipo
        Agenzia = 1
        Uso_da_Terzi = 2
    End Enum

End Class
