var tipoRicerca = 'T'; // T → Testate; D → Dettagli
var Elenco_Varieta = "";
var Elenco_Specie = "";
var TRASFORMATI_VEGETALI = 210;
var TRASFORMATI_ANIMALI = 310;

function CambiaTipoOutput(e) {
    var index = this.current().index();
    switch (index) {
        case 0:
            $("#tab1").click();
            $("#tab2").hide();
            tipoRicerca = 'T';

            $("#gridGenerale").show();
            $("#gridDettagli").hide();

            break;
        case 1:
            $("#tab2").show();
            tipoRicerca = 'D';

            $("#gridGenerale").hide();
            $("#gridDettagli").show();

            break;
    }
}

function popolaTestateTrasferimenti(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: kReadTrasferimenti_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: kDeleteTrasferimenti_rows,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Id_Agenda";
    var campiKendoModel = kReadTrasferimenti_mod();
    var colonneKendoGrid = kReadTrasferimenti_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
    };

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-lg", //fa-pencil-square-o fa-external-link
                    className: "e_link",
                    name: "e_link",
                    text: "&nbsp",
                    click: modificaElemento
                },
                {
                    iconClass: "fa fa-trash fa-lg",
                    className: "destroy",
                    name: "destroy",
                    text: "&nbsp"
                }
            ],
            title: TraduzioneMultiResx(ricercaTrasferimentiResx, "Operazioni", "Operazioni")
        }
    ];

    colonneKendoGrid.unshift(colCustKendoGrid[0]);

    var parametriKendoGrid = {
        editable: { mode: "inline" },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        pdf:false
       
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRigheTrasferimenti };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

}

function kDeleteTrasferimenti_rows(options) {

    var piva = $(cIdPiva).val();
    var idAgenda = options.data.models[0]["Id_Agenda"];

    var ris = CancellaTotaleTrasferimento(piva, idAgenda);

    if (ris !== undefined && ris !== null && ris.RispostaOK === true) {
        
        RicercaTrasferimenti();

        MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "EliminazioneEffettuataCorrettamente", "Eliminazione effettuata correttamente"), "DIV_Messaggi");


    } else if (ris !== undefined && ris.RispostaOK === false) {

        let msgError = "";
        let risp = JSON.parse(ris.RispostaStringa);

        if (risp.MsgError !== "" && ris.Errore !== "") {
            msgError = risp.MsgError + "<br/>" + TraduzioneMultiResx(ricercaTrasferimentiResx, "ErroreDuePunti_", "Errore: ") + ris.Errore;
        } else if (risp.MsgError !== "") {
            msgError = risp.MsgError;
        } else if (ris.Errore !== "") {
            msgError = ris.Errore;
        }

        MessaggioErrore_Bootstrap(msgError, "DIV_Messaggi");
    } else {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "ErroreEliminazione", "Errore eliminazione"), "DIV_Messaggi");
    }

}

function kReadTrasferimenti_rows(options) {

    var data = $('input[name$="hdKendo_Trasferimenti"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadTrasferimenti_col() {

    var data = $('input[name$="hdKendo_Trasferimenti"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadTrasferimenti_mod() {

    var data = $('input[name$="hdKendo_Trasferimenti"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}


function onDataBoundRigheTrasferimenti(e) {

    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");


    grid.autoFitColumn(0);

}


function popolaTestateTrasferimentiDettagli(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: kReadTrasferimentiDettagli_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: kDeleteTrasferimenti_rows,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Id_Agenda";
    var campiKendoModel = kReadTrasferimentiDettagli_mod();
    var colonneKendoGrid = kReadTrasferimentiDettagli_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "NrImballaggi", aggregate: "sum" },
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
            { field: "KgLordi", aggregate: "sum" },
            { field: "KgNetti", aggregate: "sum" }
        ]
    };

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-lg", //fa-pencil-square-o fa-external-link
                    className: "e_link",
                    name: "e_link",
                    text: "&nbsp",
                    click: modificaElemento
                },
                {
                    iconClass: "fa fa-trash fa-lg",
                    className: "destroy",
                    name: "destroy",
                    text: "&nbsp"
                }
            ],
            title: TraduzioneMultiResx(ricercaTrasferimentiResx, "Operazioni", "Operazioni")
        }
    ];

    colonneKendoGrid.unshift(colCustKendoGrid[0]);

    var parametriKendoGrid = {
        editable: { mode: "inline" },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        pdf: false

    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoDataBound: onDataBoundRigheTrasferimentiDettagli*/ };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

}

function kReadTrasferimentiDettagli_rows(options) {

    var data = $('input[name$="hdKendo_TrasferimentiDettagli"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadTrasferimentiDettagli_col() {

    var data = $('input[name$="hdKendo_TrasferimentiDettagli"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadTrasferimentiDettagli_mod() {

    var data = $('input[name$="hdKendo_TrasferimentiDettagli"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}


//MODIFICA DELL'ELEMENTO
function modificaElemento(e) {
    var di = this.dataItem($(e.currentTarget).closest("tr"));
    var chiave = di.Id_Agenda;
    $('input[name$="hdIdTrasferimenti"]').val(chiave);
    //var piva = getParameterByName("p");
    //window.location = "./Trasferimento.aspx?p=" + piva + "&Id_Agenda=" + chiave;

    ModificaTrasferimento($(cIdPiva).val(), chiave, 2);

}


function SpecieChange(e) {

    var filtro_specie = KendoMultisel("multiselSpecie").value().join(",");

    if (filtro_specie !== "")
        Elenco_Varieta = Leggi_Varieta(filtro_specie);
    else
        Elenco_Varieta = "";

    var multiselVarieta = KendoMultisel("multiselVarieta");
    multiselVarieta.autoBind = false;

    RiempiVarieta(multiselVarieta.dataSource);

}

function RiempiSpecie(options) {
    options.success(Elenco_Specie);
}

function RiempiVarieta(options) {
    var elencoVuoto = [{}];
    if (Elenco_Varieta !== "") {
        options.success(Elenco_Varieta);
    } else options.success(elencoVuoto);
}

function RiempiCategorie(options) {
    var elencoCategorie = Leggi_Categorie_Magazzino();
    options.success(elencoCategorie);
}

function RicercaProdottiCompleto(options) {

    if (options.data.filter == undefined || options.data.filter.filters.length !== 0) {
        let Elem_Cod = 0;
        let soloInGiacenza = false;
        let Sa_Cod = 0;
        let Fabbricato_Cod = 0;
        let TipoDestinazione = 0;
        let Cau_Mov = "7300";
        let xPUARegolamento = 0;
        let xLottoAccettazione = "";
        let Data_Movimento = formattedDate(new Date(), "/");
        let Flag_QtaNoZero = false;
        let xTipoPUARegolamento = 0;

        let elemCodArray = [TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI];

        let elencoProdottiCompleto = RicercaElencoCompletoProdottiMultiCategoria(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
            Sa_Cod, Fabbricato_Cod, TipoDestinazione,
            elemCodArray, null, null,
            soloInGiacenza, JSON.stringify(options.data.filter.filters),
            "", Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, false, Flag_QtaNoZero,
            xTipoPUARegolamento, "");

        var risp = JSON.parse(elencoProdottiCompleto);

        let elencoProdottiSoloLinea = [];

        // In tutti i casi  considero solo i Trasformati Vegetali / Animali legati a linea e con partita iva = piva
        // dell'azienda che sta emettendo il documento
        // più i i prodotti non legati a linea
        if (Array.isArray(risp)) {
            elencoProdottiSoloLinea = risp.filter(function (x) {
                return (
                    (x.LegatoALinea === 1 && $(cIdPiva).val() === x.Piva && x.Elem_Cod === TRASFORMATI_VEGETALI) ||
                    (x.LegatoALinea !== 1)
                );
            });
        }

        let elencoProdottiFinale = [];
        for (let i = 0; i < elencoProdottiSoloLinea.length; i++) {
            let found = false;
            if (elencoProdottiSoloLinea[i].Elem_Cod === TRASFORMATI_VEGETALI &&
                (elencoProdottiSoloLinea[i].Mat_Cod_OMNI === 0 ||
                    elencoProdottiSoloLinea[i].Mat_Cod_OMNI === elencoProdottiSoloLinea[i].Prodotto_Cod * - 1)) {
                // Ho trovato un prodotto OMNI
                for (let y = 0; y < elencoProdottiSoloLinea.length; y++) {
                    if (elencoProdottiSoloLinea[y].Elem_Cod === TRASFORMATI_VEGETALI &&
                        elencoProdottiSoloLinea[i].Prodotto_Cod !== elencoProdottiSoloLinea[y].Prodotto_Cod &&
                        elencoProdottiSoloLinea[i].Prodotto_Cod === elencoProdottiSoloLinea[y].Mat_Cod_OMNI * - 1) {
                        // Devo scartare il prodotto perché c'è una referenza con lo stesso prodotto
                        found = true;
                        break;
                    }

                }

            }

            if (!found)
                elencoProdottiFinale.push(elencoProdottiSoloLinea[i]);

        }

        // Essendo una multiselect ripasserà solo la chiave, quindi devo avere anche Elem_Cod per poter poi fare la lettura corretta
        for (let i = 0; i < elencoProdottiFinale.length; i++) {
            elencoProdottiFinale[i].Prodotto_Cod = elencoProdottiFinale[i].Elem_Cod + "_" + elencoProdottiFinale[i].Prodotto_Cod;
        }
        options.success(elencoProdottiFinale);

    }
}

function multiselProdotti_filtering(e) {
    var filter = e.filter;

    if (filter === undefined || !filter.value || filter.value.length < 3) {
        //prevent filtering if the filter does not value
        e.preventDefault();
    }
}