
/* RegCaricoScaricoPassaporti.js */


var enum_wAnagraficaStati_PP = {
    NonDefinito: { value: 0, name: "NonDefinito", code: 0 },
    DaConfermare: { value: 320, name: "To Check", code: 320 },
    Confermato: { value: 321, name: "Confirmed", code: 321 }
}

//i18n Traduzione ad ora non necessaria
var enum_StatiCodifiche = {
    MOV_REG_nn:  { value: 0, name: " Movimento GIAS - Mappato con Riga Registro - nessuno stato particolare", code: 0 },
    MOV_nn_DEL:  { value: 1, name: " Movimento GIAS - Non Esiste Riga Registro - stato DEL: proposta di aggiornamento registro rifiutata", code: 1 },
    MOV_REG_DEL: { value: 2, name: " Movimento GIAS - Mappato con Riga Registro - stato DEL: proposta di aggiornamento successivo del registro rifiutata", code: 2 },
    MOV_REG_MOD: { value: 3, name: " Movimento GIAS - Mappato con Riga Registro - stato MOD: riga del registro modificata a mano", code: 3 },
    nn_REG_DEL:  { value: 4, name: " Movimento GIAS Non Esiste - Riga Registro Esiste - stato DEL: proposta di cancellazione riga registro rifiutata", code: 4 }
}

var UltimoPulsantePremuto;
var enum_ultimoPulsantePremuto = {
    Aggiorna: { value: 0, name: "Aggiorna", code: 0 },
    Recupera: { value: 1, name: "Recupera", code: 1 }
}


function kReadKendoRegistroPassaportiPrd_mod() {

    var data = $('#hdKendoRegistroPassaporti').val();
    var jSonParsed_Kendo = JSON.parse(data);


    //console.log(data);
    return jSonParsed_Kendo.kendo_model;

}

function kReadKendoRegistroPassaportiPrd_col() {
    var data = $('#hdKendoRegistroPassaporti').val();
    var jSonParsed_Kendo = JSON.parse(data);

    var lblCaricoScarico = TraduzioneMultiResx(resxObj, "ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Carico_Scarico", "Carico/Scarico");
    var lblCausale = TraduzioneMultiResx(resxObj, "ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Causale", "Causale");
    var lblNomeBotanico = TraduzioneMultiResx(resxObj, "ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Nome_Botanico_Specie", "(A) - Specie e Cultivar");

    kendo_Colonne_estendi(jSonParsed_Kendo, "TipoZona", TraduzioneMultiResx(resxObj, "TipoZona", "Tipo Zona") + " *", 1, "TipoZonaDes", passaportiVivaiTipoZona_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "Cul_COD", lblNomeBotanico + " *", 2, "Veg_Des_Lat", passaportiVivaiNomeBotanico_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "CaricoScarico", lblCaricoScarico + " *", 11, "CaricoScaricoDes", passaportiVivaiCaricoScarico_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "Causale", lblCausale + " *", 12, "CausaleDes", passaportiVivaiCausale_Template);



    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

/**
 *
 * @param {any}  elemento oggetto html
 * @param {number} indice in base zero
 */
function kPulsanteDatoIndice(elemento, indice) {
    return $($($(elemento).children("td")[1]).children("div")[indice]);
};


function ppAttivaDisattivaPulsanti(giasStato, elemento) {
    var pulsanteStampaPp = kPulsanteDatoIndice(elemento, 0);
    var pulsanteConferma = kPulsanteDatoIndice(elemento, 1);
    var s = parseInt(giasStato);
    switch (s) {
        case enum_wAnagraficaStati_PP.NonDefinito.value:
            pulsanteStampaPp.hide();
            pulsanteConferma.hide();
            break;
        case enum_wAnagraficaStati_PP.DaConfermare.value:
            pulsanteStampaPp.hide();
            pulsanteConferma.show();
            break;
        case enum_wAnagraficaStati_PP.Confermato.value:
            pulsanteStampaPp.show();
            pulsanteConferma.hide();
            break;
        default:
    }
};
function coloraRigheImpostaPulsanti(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {

        var dataItem = grid.dataItem(this);

        //imposta il colore

        switch (dataItem.Azione) {      
            case "DEL":
                $(this).addClass("kendoRiga_pp_orange");
                ppAttivaDisattivaPulsanti(enum_wAnagraficaStati_PP.DaConfermare.value, this);
                break;
            case "PEN":
                $(this).addClass("kendoRiga_pp_green");
                ppAttivaDisattivaPulsanti(enum_wAnagraficaStati_PP.DaConfermare.value, this);
                break;
            case "ASS":
                $(this).addClass("kendoRiga_pp_red");
                ppAttivaDisattivaPulsanti(enum_wAnagraficaStati_PP.DaConfermare.value, this);
                break;
            case "MOD":
                $(this).addClass("kendoRiga_pp_yellow");
                ppAttivaDisattivaPulsanti(enum_wAnagraficaStati_PP.NonDefinito.value, this);
                $(this).find(".btn-Cancella").hide();
                break;
            case "":
            case "MODREG":
                if (dataItem.GIAS_Stato === enum_wAnagraficaStati_PP.DaConfermare.value) {
                    $(this).addClass("kendoRiga_pp_" + dataItem.statoColore);
                }
                ////attiva i pulsanti in base allo stato
                ppAttivaDisattivaPulsanti(dataItem.GIAS_Stato, this);
                break;
            default:
        }



    });


}

function pp_onDataBoundRighe(e) {
    coloraRigheImpostaPulsanti("#kendoRegistroPassaporti", e);
    kendoGridFlatResizeColonne();
}


function kendoGridFlatResizeColonne() {

    var grid = $("#kendoRegistroPassaporti").data("kendoGrid");

    for (i = 0; i < grid.columns.length; i++) {

        if (grid.columns[i].width === undefined) {

            grid.autoFitColumn(i);

        }

    }
}

function kendoGridFlatCaricoScaricoPassaporti(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: RicercaRigheRegistroPassaporti,
        funzioneSubmit: { funzione: cliPassaportoVivaiAggiornaDatiSrv }
    };
    var idModel = "ws_VivaiPassaporti_Operazione_cod";

    var campiKendoModel = kReadKendoRegistroPassaportiPrd_mod();
    var colonneKendoGrid = kReadKendoRegistroPassaportiPrd_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 20,
        // Giulia: 14/5/2020: commentata perché genera errore in minify e sembra che non venga usata da creaKendoGrid
        //columnMenuInit(e) {
        //    e.container.find('li[role="menuitemcheckbox"]:nth-child(0)').remove();
        //},
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        groupable: false,
        scrollable: true,
        sortable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        filterable: { multi: true, search: true },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: false,
        excel: true,
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='' style='display:block;border:0px;' onclick=stampaPassaporto(this.closest('tr'),this.closest('.k-grid'))><img style='width: 60px;height: 50px;margin-right: 7px;' src='" + PATH_GIASBASE + "agronica/ab_immagini/varie/Icona-Stampa-Etichetta-Passaporto.png' /></div>" +
                        "<div class='btn btn-info btnInfo' style='display:block;border:0px;' onclick=confermaVoceRegistro(this.closest('tr'),this.closest('.k-grid'))><span style='heigth: 40px'><span class='k-icon k-i-check'></span></span></div>"
                },
                title: TraduzioneMultiResx(resxObj, "Azioni", "Azioni"),
                width: "55px"
            }
        ]
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: pp_onDataBoundRighe };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["ws_VivaiPassaporti_Operazione_cod"];

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