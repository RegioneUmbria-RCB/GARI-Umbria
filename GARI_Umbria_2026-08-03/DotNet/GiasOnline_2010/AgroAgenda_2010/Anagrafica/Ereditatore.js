

/*
* JS per Ereditatore
*
* Galassi: creazione 2017-04-11
*/

/**
 * Funzioni custom
 */

function proseguiEred(){
    leggi_ImpiantiConProprieta($("#kendo_ElencoProp").data("kendoMultiSelect"));
    GrigliaKendoEreditatore("kendo_EredImpianti");
}

function convertDate(inputFormat) {
  function pad(s) { return (s < 10) ? '0' + s : s; }
  var d = new Date(inputFormat);
  return [pad(d.getDate()), pad(d.getMonth()+1), d.getFullYear()].join('/');
}


/**
* Parte kendoMultiSelect
*
*
*/
var jSonParsed_Kendo_ElencoProp;

function kReadElencoPropr_mod() {
    return jSonParsed_Kendo_ElencoProp.kendo_model;
}

function kendo_ElencoProprieta_Leggi(options) {
    options.success(jSonParsed_Kendo_ElencoProp.kendo_rows);
}


/**
 * 
 * @param {string} div Elemento HTML
 */
function MultiKendoProprieta(div) {

    var funzioniCRUD = {
        funzioneRead: kendo_ElencoProprieta_Leggi
    };
    var idModel = "value";
    var textField = "text";
    var valueField = "value";
    var campiKendoModel = kReadElencoPropr_mod(); //kendo_model
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        autoClose: false,
        height: 400,
        placeholder: "Selezionare le proprietà desiderate.."
    };
    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamarePrimaDelDataBound: onDataBoundRighePianoConcimazione,
        //funzioneDaChiamareDopoDataBound: postDataBoundRighePianoConcimazione
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];
    CreaKendoMultiselect(
        div,
        funzioniCRUD,
        idModel,
        campiKendoModel,
        textField,
        valueField);
}


function KendoProprieta(div) {

    $("#" + div).kendoMultiSelect({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: jSonParsed_Kendo_ElencoProp,
        autoClose: false,
        height: 200,
        placeholder: "Selezionare i campi da modificare.."
    });
}


/**
* Parte KendoGrid
*
*/
var jSonParsed_Kendo_EredImpianti;

function kendoRefresh(jQuerySelector) {
    $(jQuerySelector).data("kendoGrid").refresh();
}

function Kendo_EredImpianti_leggi(options) {
    options.success(jSonParsed_Kendo_EredImpianti.kendo_rows);
}

function Aggiorna_EredImpianti(options) {

}

function onDataBoundRigheEreditatore(){
    //richiamare una funzione dalla master
    kendo_AggiustaDimensioneColonne("#kendo_EredImpianti");
}

function postDataBoundRigheEreditatore(){

}

function kReadEreditatore_mod() {
    return jSonParsed_Kendo_EredImpianti.kendo_model;

}

function kReadEreditatore_col() {
    var columns = jSonParsed_Kendo_EredImpianti.kendo_columns;
    /* Qui aggiungo le colonne che mi interessano in lettura aggiuntive */

    return columns;
}


/* creazione funzione Kendo_multiSelect */
function GrigliaKendoEreditatore(div) {

    var funzioniCRUD = {
        funzioneRead: Kendo_EredImpianti_leggi
        , funzioneUpdate: Aggiorna_EredImpianti
    };
    var idModel = "kendoKey"; /*todo*/
    var campiKendoModel = kReadEreditatore_mod(); //kendo_model
    var colonneKendoGrid = kReadEreditatore_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        groupable: false,
        scrollable: true,
        resizable: false,
        filterable: false,
        columMenu: false,
        pdf: false,
        excel: false,
        pagesize: 50,
        editable: true
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBound: onDataBoundRigheEreditatore,
        funzioneDaChiamareDopoDataBound: postDataBoundRigheEreditatore
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = ["kendoKey"];

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



/**
 * Funzione MultiSelect
 * 
 * OBBLIGATORI
 * 
 * @param  {string} IDControllo
 * @param  {object} funzioniCRUD
 * @param  {string} idModel
 * @param  {object} campiKendoModel
 * @param  {string} textField
 * @param  {string} valueField
 * 
 * OPZIONALI
 * 
 * @param  {object} parametriPerLettura
 * @param  {object} parametriDataSource
 * @param  {object} parametriKendoMultiSelect
 * @param  {object} funzioniPrimaDopoEventi
 */
function CreaKendoMultiselect(
    //Parametri obbligatori
    IDControllo,
    funzioniCRUD,
    idModel,
    campiKendoModel,
    textField,
    valueField,
    //Parametri Facoltativi
    parametriPerLettura,
    parametriDataSource,
    parametriKendoMultiSelect,
    funzioniPrimaDopoEventi) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene la griglia");
        return;
    }

    if (funzioniCRUD == null || funzioniCRUD.funzioneRead == null) {
        alert("Non mi hai passato la funzione da chiamare in lettura");
        return;
    }

    if (idModel == null) {
        alert("Non mi hai passato la chiave della riga della griglia");
        return;
    }

    if (campiKendoModel == null) {
        alert("Non mi hai passato i campi del modello della griglia");
        return;
    }

    if (textField == null) {
        alert("Non mi hai passato il campo nome della griglia");
        return;
    }

    if (valueField == null) {
        alert("Non mi hai passato il campo valore della griglia");
        return;
    }



    if (funzioniCRUD != null) {
        errFound = false;
        for (var k in funzioniCRUD) {
            if (k != "funzioneRead") {
                errFound = true;
                alert("Fra le funzioni CRUD mi hai passato la chiave " + k + " che non è gestita");
            }
        }
        if (errFound)
            return;

        // Se il parametro non viene passato si assume che l'utente abbia i permessi
        funzioniCRUD.UtenteAbilitatoInserimentoModifica = (typeof funzioniCRUD.UtenteAbilitatoInserimentoModifica === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoInserimentoModifica;
        funzioniCRUD.UtenteAbilitatoCancellazione = (typeof funzioniCRUD.UtenteAbilitatoCancellazione === 'undefined') ? true : funzioniCRUD.UtenteAbilitatoCancellazione;

    }


    funzioniPrimaDopoEventi = (typeof funzioniPrimaDopoEventi === 'undefined') ? {} : funzioniPrimaDopoEventi;
    errFound = false;
    for (var k in funzioniPrimaDopoEventi) {
        if (k != "funzioneDaChiamarePrimaDelDataBinding" &&
            k != "funzioneDaChiamareDopoDataBinding" &&
            k != "funzioneDaChiamarePrimaDelDataBound" &&
            k != "funzioneDaChiamareDopoDataBound" &&
            k != "funzioneDaChiamarePrimaDelSave" &&
            k != "funzioneDaChiamarePrimaDiSelectAllRows" &&
            k != "funzioneDaChiamareDopoSelectAllRows" &&
            k != "funzioneDaChiamareDopoEdit" &&
            k != "funzioneDaChiamarePrimaDiSaveChangesKendomultiSelect" &&
            k != "funzioneDaChiamareDopoSave") {
            errFound = true;
            alert("Fra le funzioni da chiamare prima o dopo agli eventi mi hai passato la chiave " + k + " che non è gestita");
        }
    }
    if (errFound)
        return;

    // Per le funzioni di accesso al database è necessario fare delle funzioni anonime 
    // altrimenti le chiama subito nel momento in cui crea la griglia
    var readFunction = null;
    if (funzioniCRUD.funzioneRead != null) {
        if (parametriPerLettura != null && parametriPerLettura.length > 0) {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options, parametriPerLettura.toString());

                //var risp = funzioniCRUD.funzioneRead(parametriPerLettura.toString());
                //options.success(risp);
            };
        }
        else {
            readFunction = function (options) {
                funzioniCRUD.funzioneRead(options);

            };
        }
    }

    parametriDataSource = (typeof parametriDataSource === 'undefined' || parametriDataSource == null) ? {} : parametriDataSource;
    parametriKendoMultiSelect = (typeof parametriKendoMultiSelect === 'undefined' || parametriKendoMultiSelect == null) ? {} : parametriKendoMultiSelect;
    parametriKendoMultiSelect.placeholder = (typeof parametriKendoMultiSelect.placeholder === 'undefined') ? "" : parametriKendoMultiSelect.placeholder;
    parametriKendoMultiSelect.height = (typeof parametriKendoMultiSelect.height === 'undefined') ? null : parametriKendoMultiSelect.height;
    parametriKendoMultiSelect.autoClose = (typeof parametriKendoMultiSelect.placeholder === 'undefined') ? true : parametriKendoMultiSelect.autoClose;

    var dataSourceMultiSelect = new kendo.data.DataSource({
        transport: {
            read: readFunction,
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    return { models: kendo.stringify(options.models) };
                }
            }
        },
        batch: true,
        schema: {
            model: {
                id: idModel,
                fields: campiKendoModel
            }
        },
        aggregate: parametriDataSource.aggregate
    });

    var multiSelect = $("#" + IDControllo).data("kendoMultiSelect");
    if (multiSelect != null) {
        multiSelect.destroy();
        $("#" + IDControllo).empty();
    }

    var kendo_multiselect = $("#" + IDControllo).kendoMultiSelect({
        dataSource: dataSourceMultiSelect,
        placeholder: parametriKendoMultiSelect.placeholder,
        datatextField: textField,
        datavalueField: valueField,
        height: parametriKendoMultiSelect.height,
        autoClose: parametriKendoMultiSelect.autoClose
    });
}