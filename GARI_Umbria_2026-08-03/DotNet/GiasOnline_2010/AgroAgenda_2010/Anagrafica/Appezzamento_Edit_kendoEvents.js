/**
*  Creazione 2017-04-20
*  solo Kendo
*/


/**
 * Kendo griglia particelle
 */
var jSonParsed_Kendo_Particelle;

function kendoRefresh(jQuerySelector) {
    $(jQuerySelector).data("kendoGrid").refresh();
}

function Kendo_Particelle_leggi(options) {
    options.success(jSonParsed_Kendo_Particelle.kendo_rows);
}

function Aggiorna_Particelle(options) {

}

function onDataBoundRigheParticelle() {

    selezionaParticelleImpiegate();
    //kendoRefresh("#kendo_Particelle");

}

function postDataBoundRigheParticelle() {
    //richiamare una funzione dalla master

    kendo_AggiustaDimensioneColonne("#kendo_Particelle");
    // var grid = $("#kendo_Particelle").data("kendoGrid");
    // for (var i = 0; i < grid.columns.length; i++) {
    //     grid.autoFitColumn(i);
    // }   

    //kendoRefresh("#kendo_Particelle");
    //selezionaRighe("#kendo_Particelle");


}

function kReadParticelle_mod() {
    return jSonParsed_Kendo_Particelle.kendo_model;

}

function kReadParticelle_col() {
    var columns = jSonParsed_Kendo_Particelle.kendo_columns;
    /* Qui aggiungo le colonne che mi interessano in lettura aggiuntive */
    kendo_Colonne_estendi(
        jSonParsed_Kendo_Particelle,
        "SuperficieImpiegata",
        TraduzioneMultiResx(appezzamentoEditResx, "SuperficieImpiegata", "Superficie Impiegata") + " [Ha]",
        2,
        "SuperficieImpiegata",
        numberEditor4decimals,
        "Sup_Coinvolta"
    );
    return columns;
}

function postSelectedRigheParticelle() {
    // Metto la colonna checked a 1
    // abilito la modifica della superificie impiegata
    // 
}

var kEventoSelezionaRiga_Sup_Impiegata = 0;

function ValoreSuperficieCoinvolta_Particella(Oggetto) {

    return kGetSuperficie(Oggetto, "SuperficieImpiegata");

}

function onEditKendoSup_Impiegata(e) {
    var Sup_Disponibile = e.sender._editContainer.parent().find('.Sup_Disponibile').html().replace(",", ".");

    if (e.values.SuperficieImpiegata <= Sup_Disponibile) {

        // se posso la sup_impiegata è corretta eseguo i calcoli per aggiornare la superficie totale dell'appezza
        kEventoSelezionaRiga_Sup_Impiegata = e.values.SuperficieImpiegata;
        e.model.SuperficieImpiegata = kEventoSelezionaRiga_Sup_Impiegata;

        //calcoli finali ..
        RicalcolaSuperficieAppezzamento();
        //RicalcolaSuperficieCoinvolta();
        //Aggiorna_hdKendo_Impianti_Selezione(e);

        //AggiornaDopo_SupTrattata();

    } else {

        //e.preventDefault();
        kEventoSelezionaRiga_Sup_Impiegata = e.values.SuperficieImpiegata;
        e.model.SuperficieImpiegata = kEventoSelezionaRiga_Sup_Impiegata;

        //calcoli finali ..
        RicalcolaSuperficieAppezzamento();
        MessaggioErrore(TraduzioneMultiResx(appezzamentoNuovoResx, "InseritaSuperficieImpiegoSuperioreDisponibile", "Hai inserito una superficie d'impiego superiore alla superficie disponibile"));
    }
}

function kEventoSelezionaRiga(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#kendo_Particelle").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    if (checked) {
        //-select the row
        row.addClass(GIAS_K_STATE_SELECTED);
        dataItem.ChkSelezionaParticella = "True";
        if (dataItem.SuperficieCondottaDisponibile > 0) {
            if (dataItem.SuperficieImpiegata === 0) {
                dataItem.SuperficieImpiegata = dataItem.SuperficieCondottaDisponibile;
            }
            row.find('.Sup_Coinvolta').click();
            //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text())+ parseFloat(dataItem.SuperficieImpiegata));
        }
        //se diverso da zero, vuol dire che esiste una superficie che arriva dalla lettura dei dati lato server.
        // if (kEventoSelezionaRiga_Sup_Coninvolta != 0) {

        //     Sup_Imp_help_Format = kEventoSelezionaRiga_Sup_Coninvolta.toString().replace(",", ".");
        //     dataItem.Sup_Imp_help = kEventoSelezionaRiga_Sup_Coninvolta;                    
        //     kEventoSelezionaRiga_Sup_Coninvolta = 0;

        // } else {

        //     Sup_Imp_help_Format = kendo.toString(dataItem.Sup_Imp_help, "n4").replace(",", ".");
        //     dataItem.Sup_Imp_help = dataItem.Sup_Imp;
        //     if (Esegui_CalcoliFinali)             
        //         kendoRefresh("#kendo_Particelle");

        // }

    } else {
        //-remove selection
        row.removeClass(GIAS_K_STATE_SELECTED);
        dataItem.ChkSelezionaParticella = "False";
        //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text())- parseFloat(dataItem.SuperficieImpiegata));
        dataItem.SuperficieImpiegata = 0;

    }

    //informo che la riga è cambiata..
    dataItem.dirty = true;


    //calcoli finali ..
    // if (Esegui_CalcoliFinali) {
    //     RicalcolaSuperficieTotale();
    //     RicalcolaSuperficieCoinvolta();
    //     Aggiorna_hdKendo_Impianti_Selezione();
    //     AggiornaDopo_SupTrattata();
    // }
    RicalcolaSuperficieAppezzamento();
}


/* creazione funzione Kendo_multiSelect */
function GrigliaKendoParticelle(div) {

    var funzioniCRUD = {
        funzioneRead: Kendo_Particelle_leggi
        //, funzioneUpdate: Aggiorna_Particelle
        , checkBoxFunction: client_operazione ? kEventoSelezionaRiga : null
    };
    var idModel = "chiave"; /*todo*/
    var campiKendoModel = kReadParticelle_mod(); //kendo_model
    var colonneKendoGrid = kReadParticelle_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {
        sort: {
            field: "SuperficieImpiegata",
            dir: "desc"
        }
    };
    if (!client_operazione) {
        parametriDataSource.filter = { field: "SuperficieImpiegata", operator: "gt", value: 0 };
    }

    //editable: client_operazione

    var parametriKendoGrid = {
        groupable: false,
        //scrollable: true,
        editable: true,
        resizable: true,
        columMenu: false,
        pdf: false,
        excel: false,

        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        filterable: { mode: "menu" },
        checkSelezioneRiga: { filterable: false, field: null, width: '30px' }
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: postDataBoundRigheParticelle,
        funzioneDaChiamarePrimaDelDataBound: onDataBoundRigheParticelle,
        funzioneDaChiamareDopoSelectAllRows: postSelectedRigheParticelle,
        funzioneDaChiamareDopoSave: onEditKendoSup_Impiegata
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(div, // rappresenta l'ID del div a cui si associa la griglia
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
    //var griglia = $('#' + div).data("kendoGrid");

    //griglia.autoFitColumn("Data2");
}




///////
/////// INDIRIZZI
///////


function caricaIndirizzi(JSON_Indirizzi) {
    var div = "kendoIndirizzi";
    var funzioniCRUD = {
        funzioneRead: function (options) {
            options.success(JSON_Indirizzi.kendo_rows);
        },
        funzioneInsert: function (options) {
            AggiornaIndirizzi();
        },
        funzioneUpdate: function (options) {
            AggiornaIndirizzi();
        },
        funzioneDelete: function (options) {
            AggiornaIndirizzi();
        }
    };
    var idModel = "chiave"; /*todo*/
    var campiKendoModel = kReadIndirizzi_mod(JSON_Indirizzi); //kendo_model
    var colonneKendoGrid = kReadIndirizzi_col(JSON_Indirizzi); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        groupable: false,
        editable: {
            mode: "popup",
            window: {
                title: TraduzioneMultiResx(appezzamentoEditResx, "ModificaIndirizzo", "Modifica Indirizzo")
            }
        },
        resizable: true,
        columMenu: false,
        pdf: false,
        excel: false,
        pageable: {
            pageSizes: [5, 10, 20, 50, 100, "all"] },
        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        name: "edit",
                        text: {
                            edit: "",
                            update: TraduzioneMultiResx(appezzamentoEditResx, "ConfermaDati", "Conferma Dati"),
                            cancel: TraduzioneMultiResx(appezzamentoEditResx, "Annulla", "Annulla")
                        }
                    },
                    {
                        name: "destroy",
                        text: "",
                        className: "k-custom-delete"
                    }
                ],
                title: TraduzioneMultiResx(appezzamentoEditResx, "Operazioni", "Operazioni"),
                width: "165px"
            }
        ]
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Ind_onEdit,
        funzioneDaChiamareDopoAnnulla: onAnnullaInd
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(div, // rappresenta l'ID del div a cui si associa la griglia
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

//function Kendo_Indirizzi_leggi(JSON_Indirizzi) {
//    options.success(JSON_Indirizzi.kendo_rows);
//}

function kReadIndirizzi_mod(JSON_Indirizzi) {
    return JSON_Indirizzi.kendo_model;
}

function kReadIndirizzi_col(JSON_Indirizzi) {
    let jSonParsed_Kendo = JSON_Indirizzi.kendo_columns;
    kendo_Colonne_estendi(JSON_Indirizzi, "pro_cod_istat", TraduzioneMultiResx(appezzamentoEditResx, "Provincia", "Provincia"), 5, "pro_cod", IstatProv_Template);
    kendo_Colonne_estendi(JSON_Indirizzi, "com_cod_istat", TraduzioneMultiResx(appezzamentoEditResx, "Comune", "Comune"), 6, "com_des", IstatCom_Template);
    kendo_Colonne_estendi(JSON_Indirizzi, "stato", TraduzioneMultiResx(appezzamentoEditResx, "Stato", "Stato"), 3, "stato_des", Stato_Template);

    return jSonParsed_Kendo;
}

function IstatProv_Template(container, options) {

    valori_stato = options.model.stato;    
    RiempiDdlProvince();

    $('<input id ="cmbIstatProv" data-text-field="Provincia_Des" data-value-field="Istat_Prov" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Provincia_Des",
            dataValueField: "Istat_Prov",
            dataSource: province_di_uno_stato,
            //dataSource: {
            //    transport: { read: RiempiDdlProvince },
            //    group: { field: "regione_des", value: "regione_des" }
            //},
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {


                //leggo la chiave ed aggiorno le diverse chiavi del model...        

                var dataItem = e.sender.dataItem();

                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato            

                var model = $("#kendoIndirizzi").data("kendoGrid").editable.options.model;

                //model.Istat_Prov = dataItem.Istat_Prov;
                //model.Provincia_Des = dataItem.Provincia_Des;
                model.set("pro_cod_istat", dataItem.Istat_Prov);
                model.set("pro_cod", dataItem.Provincia_Des);

                valori_provincia = dataItem.Istat_Prov;
                RiempiDdlComuni(valori_provincia);

                var ddl = $("#cmbIstatCom").data("kendoDropDownList");
                ddl.dataSource.data(comuni_di_una_provincia);
                ddl.value("");
            }

        });

}



function IstatCom_Template(container, options) {

    valori_provincia = options.model.pro_cod_istat;
    RiempiDdlComuni(valori_provincia);

    $('<input id ="cmbIstatCom" data-text-field="Comune_Des" data-value-field="Istat_Com" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Comune_Des",
            dataValueField: "Istat_Com",
            dataSource: comuni_di_una_provincia,
            //dataSource: { transport: { read: RiempiDdlComuni }, group: { field: "provincia_des" } },        

            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                var dataItem = e.sender.dataItem();
                var model = $("#kendoIndirizzi").data("kendoGrid").editable.options.model;
                model.set("com_cod_istat", dataItem.Istat_Com);
                model.set("com_des", dataItem.Comune_Des);

                //model.com_cod_istat = dataItem.Istat_Com;
                //model.com_des = dataItem.Comune_Des;

            }

        });

}

function Stato_Template(container, options) {

    stato = options.model.stato;   

    $('<input id ="cmbStato" data-text-field="descrizione" data-value-field="codice" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "descrizione",
            dataValueField: "codice",
            dataSource: { transport: { read: RiempiDdlStati } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                var model = $("#kendoIndirizzi").data("kendoGrid").editable.options.model;
                if (model.stato == undefined || model.stato == "") {
                    $("#cmbStato").data("kendoDropDownList").select("IT");
                    $("#cmbStato").data("kendoDropDownList").trigger("change");
                } else {
                /*if (model.stato == "IT" || model.stato == "ITALIA") {*/

                    
                    if (KendoDDL("cmbStato").dataItem().gestione_gerarchia_geografica == 1) {

                        /*$("#cmbStato").data("kendoDropDownList").trigger("change");*/

                        $("div[data-container-for='pro_cod_istat']").show();
                        $("div[data-container-for='com_cod_istat']").show();
                        //$("#cmbStato").data("kendoDropDownList").trigger("change");
                    } else {
                        $("div[data-container-for='pro_cod_istat']").hide();
                        $("div[data-container-for='com_cod_istat']").hide();
                    }
                }
                kendoDropDownAdjustWidth(e);
            },
            change: function (e) {


                //leggo la chiave ed aggiorno le diverse chiavi del model...        

                var dataItem = e.sender.dataItem();

                if (dataItem == undefined) {
                    $("div[data-container-for='pro_cod_istat']").hide();
                    $("div[data-container-for='com_cod_istat']").hide();
                    return false;
                }
                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato            

                var model = $("#kendoIndirizzi").data("kendoGrid").editable.options.model;

                //model.Istat_Prov = dataItem.Istat_Prov;
                //model.Provincia_Des = dataItem.Provincia_Des;
                model.set("stato", dataItem.codice);
                model.set("stato_des", dataItem.descrizione);

            /*if (dataItem.codice != "IT") {*/
                if (KendoDDL("cmbStato").dataItem().gestione_gerarchia_geografica == 0) {
                    $("div[data-container-for='pro_cod_istat']").hide();
                    $("div[data-container-for='com_cod_istat']").hide();
                } else {

                    valori_stato = dataItem.codice;
                    RiempiDdlProvince();
                  
                    var ddl = $("#cmbIstatProv").data("kendoDropDownList");
                    ddl.dataSource.data(province_di_uno_stato);
                    ddl.value("");

                    comuni_di_una_provincia = new Array();
                    objVuoto = { "Istat_Com": "", "Comune_Des": "" };
                    comuni_di_una_provincia.unshift(objVuoto);

                    var ddl = $("#cmbIstatCom").data("kendoDropDownList");
                    ddl.dataSource.data(comuni_di_una_provincia);
                    ddl.value("");

                    $("div[data-container-for='pro_cod_istat']").show();
                    $("div[data-container-for='com_cod_istat']").show();
                }

            }

        });

}


function RiempiDdlStati(options) {
    var params = {
        objP_Server: objP_server
    };
    ajaxAgronica(pathCoreWS + "Metaschema/ISTAT.asmx/GetStati",
        JSON.stringify(params),
        function (risposta) {
            var p = JSON.parse(risposta.RispostaStringa);
            options.success(p);
        }, null);

}


function RiempiDdlProvince(options) {

    var stato = "IT"

    if (KendoDDL("cmbStato").dataItem() !== undefined) {
        stato = KendoDDL("cmbStato").dataItem().codice;
    }

    province_di_uno_stato = new Array();
    objVuoto = { "Istat_Prov": "", "Provincia_Des": "" };
    province_di_uno_stato.unshift(objVuoto);

    var params = {
        valori_regioni: "",
        stato: valori_stato,
        objP_Server: objP_server
    };
    ajaxAgronicaSync(pathCoreWS + "Metaschema/ISTAT.asmx/GetProvincie",
        JSON.stringify(params), false,
        function (risposta) {
            province_di_uno_stato = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "Istat_Prov": "", "Provincia_Des": "" };
            province_di_uno_stato.unshift(objVuoto);
            /*var p = JSON.parse(risposta.RispostaStringa);*/
            //options.success(p);
        }, null);

}

function RiempiDdlComuni(options) {
    var params = {
        valori_provincia: valori_provincia,
        objP_Server: objP_server
    };
    if (valori_provincia == "") {

        comuni_di_una_provincia = new Array();
        objVuoto = { "Istat_Com": "", "Comune_Des": "" };
        comuni_di_una_provincia.unshift(objVuoto);

    } else {

        ajaxAgronicaSync(pathCoreWS + "Metaschema/ISTAT.asmx/GetComuni",
            JSON.stringify(params), false,
            function (risposta) {
                comuni_di_una_provincia = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "Istat_Com": "", "Comune_Des": "" };
                comuni_di_una_provincia.unshift(objVuoto);
            }, null);
    }

}

function Ind_onEdit(e) {
    if (e.model.isNew() && e.model.chiave == 0) {
        var chiave = 1;
        var grid = $("#kendoIndirizzi").data("kendoGrid");
        var currentData = grid.dataSource.data();

        for (var i = 0; i < currentData.length; i++) {
            if (currentData[i].chiave >= chiave) {
                chiave = currentData[i].chiave;
                chiave += 1;
            }
        }

        e.model.chiave = chiave;
        e.model.Cod_Indirizzo = 0;
        e.model.Tipo_Indirizzo = 1;
        e.model.Tipo_Indirizzo_Des = "";
        //e.model.stato_des = "Italia"; //i18n
        //e.model.stato = "IT";

        var gestione_gerarchia_geografica = 0
        if (KendoDDL("cmbStato").dataItem() !== undefined) {
            gestione_gerarchia_geografica = KendoDDL("cmbStato").dataItem().gestione_gerarchia_geografica == 1
        }

        if (gestione_gerarchia_geografica == 1) {
            e.model.stato = KendoDDL("cmbStato").dataItem().codice;
            e.model.stato = KendoDDL("cmbStato").dataItem().descrizione;
        }
        else {
            e.model.stato = "";
            e.model.stato = "";
        }

        var ddl = $("#cmbStato").data("kendoDropDownList");
        ddl.value(e.model.stato);

    }
}

function onAnnullaInd(e) {
    //$('#kendo_Appezzamenti').data("kendoGrid").refresh();
    setTimeout(function () { $("#kendoIndirizzi").data("kendoGrid").refresh(); }, 0);
}