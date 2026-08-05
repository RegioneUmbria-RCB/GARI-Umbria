async function aggiornaReport() {

    WaitFrame.show();
    try {
        let resp = await ws_PrenotazionePiante_Riepilogo(Txt_Data.value());
        popolaGrigliaPrenotazionePianteRiepilogo(resp);


    } catch (e) {
        kendo.alert(e.stack);
    }

    WaitFrame.hide();

}

function popolaGrigliaPrenotazionePianteRiepilogo(resp) {

    strPrenotazionePianteRiepilogo = resp;

    let idDiv = "kendoRiepilogo";

    var funzioniCRUD = {
        funzioneRead: dataPrenotazionePianteRiepilogo
    };

    var idModel = "Cod_Progetto";
    var campiKendoModel = modelPrenotazionePianteRiepilogo();
    var colonneKendoGrid = colonnePrenotazionePianteRiepilogo();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    //var template = kendo.template($("#popupDistinte").html());
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        sortable: true,
        reorderable: true,
        pdf: false,
        excel: true,
        groupable: true,
        pageable:
        {
            pageSize: 50,
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        //filterable: { mode: "row" },
        colonneCustomKendoGrid: [
            /*{
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoZoo(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaZoo(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>"
                }, title: "Azioni", width: "97px"
            }*/
        ]

    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundPrenotazionePianteRiepilogo,
        funzioneDaChiamareDopoEdit: PrenotazionePianteRiepilogo_onEdit
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    GrigliaPrenotazionePianteRiepilogo = $("#" + idDiv).data("kendoGrid");

    //kendo_AggiustaDimensioneColonne($("#" + idDiv));
}

function dataPrenotazionePianteRiepilogo(options) {
    let a = JSON.parse(strPrenotazionePianteRiepilogo);
    return options.success(a);
}

function modelPrenotazionePianteRiepilogo(options) {
    return {
        "Piva_Padre": {
            "editable": false,
            "type": "string"
        },
        "Rag_soc_Padre": {
            "editable": false,
            "type": "string"
        },
        "Piva": {
            "editable": false,
            "type": "string"
        },
        "partitaIvaReale": {
            "editable": false,
            "type": "string"
        },
        "CUAA": {
            "editable": false,
            "type": "string"
        },
        "Codice_Socio": {
            "editable": false,
            "type": "string"
        },
        "rag_soc": {
            "editable": false,
            "type": "string"
        },
        "Impresa_ind_des": {
            "editable": false,
            "type": "string"
        },
        "Impresa_frz_des": {
            "editable": false,
            "type": "string"
        },
        "Impresa_CAP": {
            "editable": false,
            "type": "string"
        },
        "Impresa_pro_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Impresa_COMUNI_PROV": {
            "editable": false,
            "type": "string"
        },
        "Impresa_com_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Impresa_LOCALITA": {
            "editable": false,
            "type": "string"
        },
        "Impresa_Stato": {
            "editable": false,
            "type": "string"
        },
        "Impresa_Regione": {
            "editable": false,
            "type": "string"
        },
        "Programmazione_Cod": {
            "editable": false,
            "type": "string"
        },
        "Programmazione_Entita_Cod": {
            "editable": false,
            "type": "string"
        },
        "Entita_Des": {
            "editable": false,
            "type": "string"
        },
        "Veg_Cod": {
            "editable": false,
            "type": "number"
        },
        "Veg_Des": {
            "editable": false,
            "type": "string"
        },
        "Cul_Cod": {
            "editable": false,
            "type": "number"
        },
        "Cul_Des": {
            "editable": false,
            "type": "string"
        },
        "Superficie": {
            "editable": false,
            "type": "number"
        },
        "Num_Piante": {
            "editable": false,
            "type": "string"
        },
        "TRA_Fila": {
            "editable": false,
            "type": "string"
        },
        "SU_Fila": {
            "editable": false,
            "type": "string"
        },
        "Cop_Cod": {
            "editable": false,
            "type": "string"
        },
        "Cop_Des": {
            "editable": false,
            "type": "string"
        },
        "Data_Semina": {
            "editable": false,
            "type": "date"
        },
        "Data_Creazione": {
            "editable": false,
            "type": "date"
        },
        "Entita_ind_des": {
            "editable": false,
            "type": "string"
        },
        "Entita_frz_des": {
            "editable": false,
            "type": "string"
        },
        "Entita_CAP": {
            "editable": false,
            "type": "string"
        },
        "Entita_pro_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Entita_COMUNI_PROV": {
            "editable": false,
            "type": "string"
        },
        "Entita_com_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Entita_LOCALITA": {
            "editable": false,
            "type": "string"
        },
        "Entita_Stato": {
            "editable": false,
            "type": "string"
        },
        "Entita_Regione": {
            "editable": false,
            "type": "string"
        },
        "Pratica_Cod": {
            "editable": false,
            "type": "string"
        },
        "Pratica_Des": {
            "editable": false,
            "type": "string"
        },
        "Servizio_Cod": {
            "editable": false,
            "type": "string"
        },
        "Servizio_Des": {
            "editable": false,
            "type": "string"
        },
        "Stato_Cod": {
            "editable": false,
            "type": "string"
        },
        "WAnagraficaStati_Des": {
            "editable": false,
            "type": "string"
        },
        "Note": {
            "editable": false,
            "type": "string"
        },
        "Data_Modifica": {
            "editable": false,
            "type": "date"
        },
        "Nome": {
            "editable": false,
            "type": "string"
        },
        "Cognome": {
            "editable": false,
            "type": "string"
        },
        "Data_Nascita": {
            "editable": false,
            "type": "date"
        },
        "Indirizzo_Legale": {
            "editable": false,
            "type": "string"
        },
        "Frazione_Legale": {
            "editable": false,
            "type": "string"
        },
        "Stato_Legale": {
            "editable": false,
            "type": "string"
        },
        "Prov_Legale": {
            "editable": false,
            "type": "string"
        },
        "Com_Legale": {
            "editable": false,
            "type": "string"
        },
        "CAP_Legale": {
            "editable": false,
            "type": "string"
        },
        "Num_Piante_Richiesta": {
            "editable": false,
            "type": "number"
        },
        "Data_Prenotazione": {
            "editable": false,
            "type": "date"
        },
        "Num_Piante_Maschi": {
            "editable": false,
            "type": "number"
        },
        "Num_Piante_Femmine": {
            "editable": false,
            "type": "number"
        },
        "Codice_Prenotazione": {
            "editable": false,
            "type": "string"
        },
        "Num_Prenotazione": {
            "editable": false,
            "type": "string"
        },
        "Piva_Vivaio": {
            "editable": false,
            "type": "string"
        },
        "Rag_Soc_Vivaio": {
            "editable": false,
            "type": "string"
        },
        "KPIN": {
            "editable": false,
            "type": "string"
        },
        "Block_Name": {
            "editable": false,
            "type": "string"
        },
        "Descrizione_Progetto": {
            "editable": false,
            "type": "string"
        },
        "N_Marze": {
            "editable": false,
            "type": "number"
        },
        "Veg_Des_Ric": {
            "editable": false,
            "type": "string"
        },
        "Cul_Des_Ric": {
            "editable": false,
            "type": "string"
        },
        "ZespriFase_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriFase_Des": {
            "editable": false,
            "type": "string"
        },
        "ZespriGrower_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriGrower_Des": {
            "editable": false,
            "type": "string"
        },
        "ZespriTipo_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriTipo_Des": {
            "editable": false,
            "type": "string"
        },
        "TipologiaDiInnesto_Cod": {
            "editable": false,
            "type": "number"
        },
        "TipologiaDiInnesto_Des": {
            "editable": false,
            "type": "string"
        },
        "Port_Cod": {
            "editable": false,
            "type": "number"
        },
        "Port_Des": {
            "editable": false,
            "type": "string"
        }
    };
}

function colonnePrenotazionePianteRiepilogo(options) {
    return [
        {
            "field": "Piva_Padre",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "PivaReferente", "P.IVA Referente"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Rag_soc_Padre",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RagioneSocialeReferente", "Ragione Sociale Referente"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "partitaIvaReale",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "PartitaIvaAbbr", "P. IVA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "CUAA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CodiceUnicoAziendaAgricolaSigla", "CUAA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Codice_Socio",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CodiceSocio", "Codice Socio"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "rag_soc",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RagioneSociale", "Ragione Sociale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_ind_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "IndirizzoImpresa", "Indirizzo Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_frz_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "FrazioneImpresa", "Frazione Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_CAP",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CapImpresa", "CAP Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_pro_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaIstatImpresa", "Prov [ISTAT] Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_COMUNI_PROV",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAbbr", "Prov"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_com_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneIstatImpresa", "Comune [ISTAT] Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_LOCALITA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneImpresa", "Comune Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_Stato",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoImpresa", "Stato Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_Regione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RegioneImpresa", "Regione Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            title: TraduzioneMultiResx(riepilogoPrenPianteResx, "LegaleRappresentante", "Legale Rappresentante"),
            columns: [
                {
                    "field": "Nome",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Nome", "Nome"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Cognome",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Cognome", "Cognome"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Data_Nascita",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataNascita", "Data Nascita"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "120px",
                    template: '#= (kendo.toString(Data_Nascita, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Nascita, "dd/MM/yyyy" ) #'
                },
                {
                    "field": "Indirizzo_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Indirizzo", "Indirizzo"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                },
                {
                    "field": "Prov_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAbbr", "Prov"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "70px"
                },
                {
                    "field": "Com_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Comune", "Comune"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "CAP_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CAP", "CAP"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "70px"
                },
                {
                    "field": "Stato_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Stato", "Stato"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "75px"
                }
            ]
        },
        {
            "field": "Programmazione_Entita_Cod",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProgettoCodice", "Progetto Codice"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "AppNome", "App. Nome"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "KPIN",
            "title": "KPIN",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Block_Name",
            "title": "Block Name",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Descrizione_Progetto",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Progetto", "Progetto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Veg_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SpecieImp", "Specie Imp."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Cul_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "VarietàImp", "Varietà Imp."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Superficie",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SuperficieAbbr", "Sup.") + " [ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Num_Piante",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroPiante", "Num. Piante"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "TRA_Fila",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "TraFila", "Tra Fila") + " [m]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "SU_Fila",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SuFila", "Su Fila") + " [m]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Cop_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Copertura", "Copertura"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Data_Semina",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataDiImpianto", "Data di Impianto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= (kendo.toString(Data_Semina, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Semina, "dd/MM/yyyy" ) #',
            width: "200px"
        },
        {
            "field": "Data_Creazione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataCreazione", "Data Creazione"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Entita_ind_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "IndirizzoAppezzamento", "Indirizzo App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_frz_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "FrazioneAppezzamento", "Frazione App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_CAP",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CapAppezzamento", "CAP App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_pro_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaIstatAppezzamento", "Prov [ISTAT] App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_COMUNI_PROV",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAppezzamento", "Prov App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_com_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneIstatAppezzamento", "Com [ISTAT] App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_LOCALITA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneAppezzamento", "Comune App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Stato",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoAppezzamento", "Stato App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Regione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RegioneAppezzamento", "Stato App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Pratica_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Pratica", "Pratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Servizio_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Servizio", "Servizio"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "WAnagraficaStati_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoPratica", "Stato Pratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Note",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Note", "Note"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Data_Modifica",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataUltimaPratica", "DataUltimaPratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px",
            template: '#= ((kendo.toString(Data_Modifica, "dd/MM/yyyy" ) == "01/01/1900") || Data_Modifica == undefined) ? "" : kendo.toString(Data_Modifica, "dd/MM/yyyy" ) #'
        },
        {
            "field": "ZespriFase_Des",
            "title": "Phase",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "ZespriGrower_Des",
            "title": "Grower",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "ZespriTipo_Des",
            "title": "Type",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "TipologiaDiInnesto_Des",
            "title": "Tipologia Innesto",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Port_Des",
            "title": "Portinnesto",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            title: TraduzioneMultiResx(riepilogoPrenPianteResx, "PrenotazionePiante", "Prenotazione Piante"),
            columns: [
                {
                    "field": "Piva_Vivaio",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "PivaVivaio", "P.IVA Vivaio"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Rag_Soc_Vivaio",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Vivaio", "Vivaio"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                },
                {
                    "field": "N_Marze",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroGemme", "N. Gemme"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                },
                {
                    "field": "Num_Piante_Richiesta",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroPianteRichiesta", "N. Piante Richiesta"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Num_Piante_Maschi",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroPianteMaschi", "N. Piante Maschi"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Num_Piante_Femmine",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroPianteFemmine", "N. Piante Femmine"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "120px"
                },
                {
                    "field": "Data_Prenotazione",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataPrenotazione", "Data Prenotazione"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px",
                    template: '#= ((kendo.toString(Data_Prenotazione, "dd/MM/yyyy" ) == "01/01/1900") || Data_Prenotazione == undefined) ? "" : kendo.toString(Data_Prenotazione, "dd/MM/yyyy" ) #'
                },
                {
                    "field": "Codice_Prenotazione",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CodicePrenotazione", "Codice Prenotazione"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "100px"
                },
                {
                    "field": "Num_Prenotazione",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumPrenotazione", "Num Prenotazione"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "100px"
                },
                {
                    "field": "Veg_Des_Ric",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SpecieRichiesta", "Specie Richiesta"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                },
                {
                    "field": "Cul_Des_Ric",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "VarietaRichiesta", "Varietà Richiesta"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                }
            ]
        }
    ];
}

function App_onDataBoundPrenotazionePianteRiepilogo(e) {

    coloraRighe("#kendoRiepilogo", e);

}

function coloraRighe(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();
    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        if (dataItem.Stato_Cod !== "" && dataItem.Stato_Cod !== "0") {
            var colorObj = objColors.find(obj => { return obj.WAnagraficaStati_Cod == dataItem.Stato_Cod; });

            if (colorObj !== undefined) {
                if (colorObj.Colore !== "") {
                    $(this).css("background-color", colorObj.Colore);
                }
            }

        }
        //switch (dataItem.Stato_Cod) {
        //    case "1":
        //        this.className = "k-master-row kendoRiga_AgendaOperazBloccata dpiOn";
        //        break;
        //}

    });

}


function PrenotazionePianteRiepilogo_onEdit(e) {

}



async function aggiornaReportSintetico() {
    WaitFrame.show();
    try {
        let resp = await ws_PrenotazionePiante_RiepilogoSintetico(Txt_Data.value());
        popolaGrigliaPrenotazionePianteRiepilogoSintetico(resp);


    } catch (e) {
        kendo.alert(e.stack);
    }

    WaitFrame.hide();
}

function popolaGrigliaPrenotazionePianteRiepilogoSintetico(resp) {

    strPrenotazionePianteRiepilogoSintetico = resp;

    let idDiv = "kendoRiepilogoSintetico";

    var funzioniCRUD = {
        funzioneRead: dataPrenotazionePianteRiepilogoSintetico
    };

    var idModel = "Cod_Progetto";
    var campiKendoModel = modelPrenotazionePianteRiepilogoSintetico();
    var colonneKendoGrid = colonnePrenotazionePianteRiepilogoSintetico();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    //var template = kendo.template($("#popupDistinte").html());
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        sortable: true,
        reorderable: true,
        pdf: false,
        excel: true,
        groupable: true,
        pageable:
        {
            pageSize: 50,
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        //filterable: { mode: "row" },
        colonneCustomKendoGrid: [
            /*{
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoZoo(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaZoo(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>"
                }, title: "Azioni", width: "97px"
            }*/
        ]

    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundPrenotazionePianteRiepilogoSintetico,
        funzioneDaChiamareDopoEdit: PrenotazionePianteRiepilogo_onEditSintetico
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    GrigliaPrenotazionePianteRiepilogo = $("#" + idDiv).data("kendoGrid");

    //kendo_AggiustaDimensioneColonne($("#" + idDiv));
}

function dataPrenotazionePianteRiepilogoSintetico(options) {
    let a = JSON.parse(strPrenotazionePianteRiepilogoSintetico);
    return options.success(a);
}

function modelPrenotazionePianteRiepilogoSintetico(options) {
    return {
        "Piva_Padre": {
            "editable": false,
            "type": "string"
        },
        "Rag_soc_Padre": {
            "editable": false,
            "type": "string"
        },
        "Piva": {
            "editable": false,
            "type": "string"
        },
        "CUAA": {
            "editable": false,
            "type": "string"
        },
        "Codice_Socio": {
            "editable": false,
            "type": "string"
        },
        "rag_soc": {
            "editable": false,
            "type": "string"
        },
        "Impresa_ind_des": {
            "editable": false,
            "type": "string"
        },
        "Impresa_frz_des": {
            "editable": false,
            "type": "string"
        },
        "Impresa_CAP": {
            "editable": false,
            "type": "string"
        },
        "Impresa_pro_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Impresa_COMUNI_PROV": {
            "editable": false,
            "type": "string"
        },
        "Impresa_com_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Impresa_LOCALITA": {
            "editable": false,
            "type": "string"
        },
        "Impresa_Stato": {
            "editable": false,
            "type": "string"
        },
        "Impresa_Regione": {
            "editable": false,
            "type": "string"
        },
        "Programmazione_Cod": {
            "editable": false,
            "type": "string"
        },
        "Programmazione_Entita_Cod": {
            "editable": false,
            "type": "string"
        },
        "Entita_Des": {
            "editable": false,
            "type": "string"
        },
        "Veg_Cod": {
            "editable": false,
            "type": "number"
        },
        "Veg_Des": {
            "editable": false,
            "type": "string"
        },
        "Cul_Cod": {
            "editable": false,
            "type": "number"
        },
        "Cul_Des": {
            "editable": false,
            "type": "string"
        },
        "Superficie": {
            "editable": false,
            "type": "number"
        },
        "Num_Piante": {
            "editable": false,
            "type": "string"
        },
        "TRA_Fila": {
            "editable": false,
            "type": "string"
        },
        "SU_Fila": {
            "editable": false,
            "type": "string"
        },
        "Cop_Cod": {
            "editable": false,
            "type": "string"
        },
        "Cop_Des": {
            "editable": false,
            "type": "string"
        },
        "Data_Semina": {
            "editable": false,
            "type": "date"
        },
        "Data_Creazione": {
            "editable": false,
            "type": "date"
        },
        "Entita_ind_des": {
            "editable": false,
            "type": "string"
        },
        "Entita_frz_des": {
            "editable": false,
            "type": "string"
        },
        "Entita_CAP": {
            "editable": false,
            "type": "string"
        },
        "Entita_pro_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Entita_COMUNI_PROV": {
            "editable": false,
            "type": "string"
        },
        "Entita_com_cod_istat": {
            "editable": false,
            "type": "string"
        },
        "Entita_LOCALITA": {
            "editable": false,
            "type": "string"
        },
        "Entita_Stato": {
            "editable": false,
            "type": "string"
        },
        "Entita_Regione": {
            "editable": false,
            "type": "string"
        },
        "Pratica_Cod": {
            "editable": false,
            "type": "string"
        },
        "Pratica_Des": {
            "editable": false,
            "type": "string"
        },
        "Servizio_Cod": {
            "editable": false,
            "type": "string"
        },
        "Servizio_Des": {
            "editable": false,
            "type": "string"
        },
        "Stato_Cod": {
            "editable": false,
            "type": "string"
        },
        "WAnagraficaStati_Des": {
            "editable": false,
            "type": "string"
        },
        "Note": {
            "editable": false,
            "type": "string"
        },
        "Data_Modifica": {
            "editable": false,
            "type": "date"
        },
        "Nome": {
            "editable": false,
            "type": "string"
        },
        "Cognome": {
            "editable": false,
            "type": "string"
        },
        "Data_Nascita": {
            "editable": false,
            "type": "date"
        },
        "Indirizzo_Legale": {
            "editable": false,
            "type": "string"
        },
        "Frazione_Legale": {
            "editable": false,
            "type": "string"
        },
        "Stato_Legale": {
            "editable": false,
            "type": "string"
        },
        "Prov_Legale": {
            "editable": false,
            "type": "string"
        },
        "Com_Legale": {
            "editable": false,
            "type": "string"
        },
        "CAP_Legale": {
            "editable": false,
            "type": "string"
        },
        "KPIN": {
            "editable": false,
            "type": "string"
        },
        "Block_Name": {
            "editable": false,
            "type": "string"
        },
        "Descrizione_Progetto": {
            "editable": false,
            "type": "string"
        },
        "N_Marze": {
            "editable": false,
            "type": "number"
        },
        "Veg_Des_Ric": {
            "editable": false,
            "type": "string"
        },
        "Cul_Des_Ric": {
            "editable": false,
            "type": "string"
        },
        "ZespriFase_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriFase_Des": {
            "editable": false,
            "type": "string"
        },
        "ZespriGrower_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriGrower_Des": {
            "editable": false,
            "type": "string"
        },
        "ZespriTipo_Cod": {
            "editable": false,
            "type": "number"
        },
        "ZespriTipo_Des": {
            "editable": false,
            "type": "string"
        },
        "TipologiaDiInnesto_Cod": {
            "editable": false,
            "type": "number"
        },
        "TipologiaDiInnesto_Des": {
            "editable": false,
            "type": "string"
        },
        "Port_Cod": {
            "editable": false,
            "type": "number"
        },
        "Port_Des": {
            "editable": false,
            "type": "string"
        }
    };
}

function colonnePrenotazionePianteRiepilogoSintetico(options) {
    return [
        {
            "field": "Piva_Padre",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "PivaReferente", "P.IVA Referente"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Rag_soc_Padre",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RagioneSocialeReferente", "Ragione Sociale Referente"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "partitaIvaReale",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "PartitaIvaAbbr", "P. IVA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "CUAA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CodiceUnicoAziendaAgricolaSigla", "CUAA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Codice_Socio",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CodiceSocio", "Codice Socio"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "rag_soc",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RagioneSociale", "Ragione Sociale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_ind_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "IndirizzoImpresa", "Indirizzo Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_frz_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "FrazioneImpresa", "Frazione Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_CAP",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CapImpresa", "CAP Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_pro_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaIstatImpresa", "Prov [ISTAT] Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_COMUNI_PROV",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAbbr", "Prov"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_com_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneIstatImpresa", "Comune [ISTAT] Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_LOCALITA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneImpresa", "Comune Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_Stato",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoImpresa", "Stato Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Impresa_Regione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RegioneImpresa", "Regione Impresa"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            title: TraduzioneMultiResx(riepilogoPrenPianteResx, "LegaleRappresentante", "Legale Rappresentante"),
            columns: [
                {
                    "field": "Nome",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Nome", "Nome"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Cognome",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Cognome", "Cognome"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "Data_Nascita",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataNascita", "Data Nascita"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "120px",
                    template: '#= (kendo.toString(Data_Nascita, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Nascita, "dd/MM/yyyy" ) #'
                },
                {
                    "field": "Indirizzo_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Indirizzo", "Indirizzo"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "200px"
                },
                {
                    "field": "Prov_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAbbr", "Prov"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "70px"
                },
                {
                    "field": "Com_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Comune", "Comune"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "150px"
                },
                {
                    "field": "CAP_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CAP", "CAP"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "70px"
                },
                {
                    "field": "Stato_Legale",
                    "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Stato", "Stato"),
                    "filterable": {
                        "multi": true,
                        "search": true
                    },
                    width: "75px"
                }
            ]
        },
        {
            "field": "Programmazione_Entita_Cod",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProgettoCodice", "Progetto Codice"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "AppNome", "App. Nome"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "KPIN",
            "title": "KPIN",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Block_Name",
            "title": "Block Name",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Descrizione_Progetto",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Progetto", "Progetto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Veg_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SpecieImp", "Specie Imp."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Cul_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "VarietàImp", "Varietà Imp."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Superficie",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SuperficieAbbr", "Sup.") + " [ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Num_Piante",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "NumeroPiante", "Num. Piante"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "TRA_Fila",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "TraFila", "Tra Fila") + " [m]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "SU_Fila",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "SuFila", "Su Fila") + " [m]",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Cop_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Copertura", "Copertura"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Data_Semina",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataDiImpianto", "Data di Impianto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= (kendo.toString(Data_Semina, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Semina, "dd/MM/yyyy" ) #',
            width: "200px"
        },
        {
            "field": "Data_Creazione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataCreazione", "Data Creazione"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Entita_ind_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "IndirizzoAppezzamento", "Indirizzo App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_frz_des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "FrazioneAppezzamento", "Frazione App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_CAP",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "CapAppezzamento", "CAP App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_pro_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaIstatAppezzamento", "Prov [ISTAT] App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_COMUNI_PROV",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ProvinciaAppezzamento", "Prov App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_com_cod_istat",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneIstatAppezzamento", "Com [ISTAT] App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_LOCALITA",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "ComuneAppezzamento", "Comune App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Stato",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoAppezzamento", "Stato App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Entita_Regione",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "RegioneAppezzamento", "Stato App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Pratica_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Pratica", "Pratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Servizio_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Servizio", "Servizio"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "WAnagraficaStati_Des",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "StatoPratica", "Stato Pratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Note",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "Note", "Note"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Data_Modifica",
            "title": TraduzioneMultiResx(riepilogoPrenPianteResx, "DataUltimaPratica", "DataUltimaPratica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px",
            template: '#= ((kendo.toString(Data_Modifica, "dd/MM/yyyy" ) == "01/01/1900") || Data_Modifica == undefined) ? "" : kendo.toString(Data_Modifica, "dd/MM/yyyy" ) #'
        },
        {
            "field": "ZespriFase_Des",
            "title": "Phase",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "ZespriGrower_Des",
            "title": "Grower",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "ZespriTipo_Des",
            "title": "Type",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "Port_Des",
            "title": "Portinnesto",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        },
        {
            "field": "TipologiaDiInnesto_Des",
            "title": "Tipologia Innesto",
            "filterable": {
                "multi": true,
                "search": true
            },
            width: "200px"
        }
    ];
}

function App_onDataBoundPrenotazionePianteRiepilogoSintetico(e) {

    coloraRighe("#kendoRiepilogoSintetico", e);

}

function PrenotazionePianteRiepilogo_onEditSintetico(e) {

}