
/* StampaPassaporto.js */


var indirizzohttp = "./StampaPassaporto.aspx";

function StampaKendo(tr_elem, grid_elem, tipo) {


    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var param = " { ConfigurazioneDaStampare: '', piva: '" + pivaVivaistaRiferimento + "', VivaiPassaporti_Operazione_cod: '" + datiRiga.ws_VivaiPassaporti_Operazione_cod.toString() + "', StampaDiretta: " + (false).toString() + " }";
    Stampa(indirizzohttp + "/Anteprima_o_Stampa", param);
}

function Riporta_FF_Dettagli_Cods(Lista_FF_Stampa_Dettagli_Cod) {
    
}

function Stampa(url, parametriChiamata) {
    console.log("Chiamata ad anteprima o stampa");
    ajaxAgronica(url, parametriChiamata,
        function (risposta) {
            console.log(risposta);
            var msg_d = risposta.UrlLink;
            if (risposta.IsLink) {

                if (risposta.Lista_FF_Stampa_Dettagli_Cod !== null)
                    Riporta_FF_Dettagli_Cods(risposta.Lista_FF_Stampa_Dettagli_Cod);

                $("#iframedivAnteprimaStampaPassaporto").attr("src", msg_d);
                $("#lblAnteprimaStampaTitlePassaporto").html("Anteprima Etichetta");
                $("#divAnteprimaStampaPassaporto").modal('toggle');
            }
            else {
                MessaggioAttenzione_Bootstrap(msg_d, "DIV_Messaggi");
            }
        }, null);
}

function StampaTutto() {

    var ConfigurazioneDaStampare = LeggiTutteConfigurazioniStampaPPSelezionate2();
    var param = " { ConfigurazioneDaStampare: '" + ConfigurazioneDaStampare + "', piva: '" + pivaVivaistaRiferimento + "', VivaiPassaporti_Operazione_cod: '', StampaDiretta: " + (true).toString() + " }";
    Stampa(indirizzohttp + "/Anteprima_o_Stampa", param);
}

function LeggiTutteConfigurazioniStampaPPSelezionate2() {

    
    // effettuate modifiche dopo l'implementazione della funzioneCRUD -> funzioneSubmit

    var grid = $("#kendoRegistroPassaportiStampa").data("kendoGrid");
    var rvalCfg = "";

    //get the new and the updated records
    var currentData = grid.dataSource.data();
    // controllo che non ci siano due righe uguali
    for (x = 0; x < currentData.length; x++) {

        var dati = currentData[x];
        rvalCfg =
            dati.FF_Stampanti_Cod.toString() +
            ',' +
            dati.FF_Stampanti_Des.toString() +
            ',' +
            dati.Lingua_Cod.toString() +
            ',' +
            dati.Qta2.toString() +
            ',' +
            dati.ws_VivaiPassaporti_Operazione_cod.toString() +
            "|";

    }

    return rvalCfg;

}

function PopolaListaPassaportiDaStampare() {

}



function RicercaRigheRegistroPassaportiStampa(options) {
    var risp = JSON.parse($("#hdKendoRegistroPassaporti").val());
    options.success(risp.kendo_rows);
}

/**
* chiamata server
* @param {any} options opzioni di chiamata
*/
function RicercaRigheRegistroPassaportiLetturaStampa() {


    var param = "{ piva: '" + pivaVivaistaRiferimento + "', operazioniCod:'" + operazioniCod + "' }";

    ajaxAgronica(indirizzohttp + "/RegistroPassaportiLettura",
        param,
        function (risposta) {

            $("#hdKendoRegistroPassaporti").val(risposta.RispostaStringa);

            kendoGridFlatCaricoScaricoPassaporti("kendoRegistroPassaportiStampa");

        }, null);
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

    //kendo_Colonne_estendi(jSonParsed_Kendo, "FF_Stampanti_Cod", "Stampante", 2, "FF_Stampanti_Des", passaportiVivaiStampanti_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "Lingua_Cod", "Lingua", 3, "Lingua_Des", passaportiVivaiLingue_Template);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}


function kendoGridFlatCaricoScaricoPassaporti(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: RicercaRigheRegistroPassaportiStampa
    };
    var idModel = "ws_VivaiPassaporti_Operazioni_cod";

    var campiKendoModel = kReadKendoRegistroPassaportiPrd_mod();
    var colonneKendoGrid = kReadKendoRegistroPassaportiPrd_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 20,
        groupable: false,
        scrollable: false,
        sortable: true,
        resizable: false,
        filterable: { mode: "row" },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        pdf: false,
        excel: true,
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:130px;border:0px;' onclick=StampaKendo(this.closest('tr'),this.closest('.k-grid'),1)>Anteprima</div>"
                },
                title: "Azioni",
                width: "130px"
            }
        ]
    };

    //<div class='btn btn-info btnInfo' style='display:block;width:130px;border:0px;' onclick=StampaKendo(this.closest('tr'),this.closest('.k-grid'),2)>Stampa</div>

    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["ws_VivaiPassaporti_Operazioni_cod"];

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