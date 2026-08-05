
function formattedDate(date, sep) {

    var d = new Date(date || Date.now()),
        month = "" + (d.getMonth() + 1),
        day = "" + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = "0" + month;
    if (day.length < 2) day = "0" + day;

    return [day, month, year].join(sep);

}

function sistemaDataInBaseAllaCulture(data) {
    if (kendo.culture().name === "it-IT") {
        if (data.length === 10) {
            data = data.substring(3, 5) + "/" + data.substring(0, 2) + "/" + data.substring(6, 10);
        }
    }

    return data;
}



function isNumeric(n) {
    var myN = n.replace(",", ".");
    return !isNaN(parseFloat(myN)) && isFinite(myN);
}

jQuery.logThis = function (text) {
    if (window["console"] !== undefined) {
        console.log(text);
    }
};

function getParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return "";
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function impostaRedirect() {
    var m = "<p>Pagina di login ... (" + sec.toString() + ")</p>";
    $("#lblAutoLogin").html(m);
    if (sec === 0) {
        window.location = "../GST_Autenticazione/Autenticazione.aspx";
        window.clearInterval(cdID);
    }

    --sec;

}

function impostaRedirectStart() {
    cdID = window.setInterval("impostaRedirect();", 1000);
}

jQuery(".numbersOnly").keyup(function () {
    this.value = this.value.replace(/[^0-9\.]/g, '');
});

/**
 * Ricava l'elemento di un array di oggetti, cercandolo per il valore della sua proprietà
 * @param {Array<Object.<string, any>>} array array in cui cercare l'oggetto
 * @param {string} nomeProp nome della proprietà desiderata
 * @param {any} value valore della proprietà desiderata
 * @returns {any} primo elemento dell'array trovato (null se non presente)
 */
function arrayLookup(array, nomeProp, value) {
    for (var i = 0, len = array.length; i < len; i++)
        if (array[i] && array[i][nomeProp] === value) return array[i];
    return null;
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(escapeRegExp(find), "g"), replace);
}

/**
 * Ricava il valore di una proprietà di un oggetto memorizzato in stringa come JSON
 * @param {string} stringJson stringa contenete Json
 * @param {string} nomeProp nome della proprietà desiderata
 * @returns {any} valore della proprietà (undefined se prop non presente)
 */
function GetPropertyFromJson(stringJson, nomeProp) {
    var obj = JSON.parse(stringJson);
    var res = obj[nomeProp];
    return res;
}

// INIZIO Funzioni legate ai parametri qualitativi

function creaKendoDDL_ParametriQualitativi(IDControllo, piva, tab_cod, tab_nome) {
    creaKendoDropDownList(IDControllo, { read: LeggiValoriParametriQualitativi, data: { Piva: piva, Tabella_Cod: tab_cod, Tabella_Nome: tab_nome } }, "val_des", "val_cod");
}

function LeggiValoriParametriQualitativi(options) {
    var Tabella_ID = this.data.Tabella_Cod;
    var piva = this.data.Piva;
    options.success(RicercaValoriParametriQualitativi(Tabella_ID, piva));
}

function LeggiValoriParametriQualitativiFiltratiSpecieVarieta(options) {
    var elencoValori = this.data.elencoValori;
    options.success(elencoValori);
}

function prendiParametroQualitativo(id_ddl) {
    var ddl = KendoDDL(id_ddl);
    var res = new Object();
    if (ddl === undefined) {

        res.Tabella_Nome = "";
        res.Param_ID = 0;
        res.Tara = 0;

    } else {

        res.Tabella_Nome = ddl.dataSource.transport.data.Tabella_Nome;
        res.Param_ID = parseInt(ddl.value());
        res.Tara = parseFloat(ddl.dataItem().tara);

    }
    return res;
}

function creaArrayCampionature(rigaKendo) {
    var arrParams = [];
    var tipiConfezionamento = ["oimballaggio", "ocontenitore", "oconfezione"];

    for (var chiave in rigaKendo) {

        if (rigaKendo.hasOwnProperty(chiave)) {

            if (chiave.startsWith("FF_")) {

                var otipo = "o" + chiave.split("_")[1];

                if (chiave.endsWith("_Tipo_Cod")) {
                    var tipoCod = rigaKendo[chiave];
                    if (tipoCod === "")
                        tipoCod = 0;

                    //verifico se è già presente nell'array, sennò creo con otipo + tipo_cod
                    var elems = arrayLookup(arrParams, "Tabella_Nome", otipo);

                    //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                    if (elems !== undefined && elems !== null) {
                        elems.Param_ID = parseInt(tipoCod);
                    } else {
                        var res = new Object();
                        res.Tabella_Nome = otipo;
                        res.Param_ID = parseInt(tipoCod);
                        arrParams.push(res);
                    }

                } else if ($.inArray(otipo, tipiConfezionamento) !== -1 && chiave.endsWith("_Tara_Campionatura")) {
                    var tara = rigaKendo[chiave];
                    // sono in presenza di un confezionamento, quindi devo prelevare la Tara_Campionatura

                    //verifico se è già presente nell'array, sennò creo con otipo + tara_campionatura
                    var elemt = arrayLookup(arrParams, "Tabella_Nome", otipo);

                    //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                    if (elemt !== undefined && elemt !== null) {
                        elemt.Tara = parseFloat(tara);
                    } else {
                        var resu = new Object();
                        resu.Tabella_Nome = otipo;
                        resu.Tara = parseFloat(tara);
                        arrParams.push(resu);
                    }
                }
            }
        }
    }

    //devo verificare che non mi sia venuto qualche elemento sgaffo, con tara ma non tipo_cod
    //(è un caso che non dovrebbe mai verificarsi), se è presente lo rimuovo
    for (var i = 0, len = arrParams.length; i < len; i++)
        if (arrParams[i].Param_ID === undefined) arrParams.splice(i, 1);

    return arrParams;
}

// FINE Funzioni legate ai parametri qualitativi

// Creazione dinamica elementi Html
function creaNewRowDiv(id) {
    var newRowDiv = document.createElement("div");
    newRowDiv.className = "row";
    newRowDiv.id = id;
    return newRowDiv;
}

function creaNewColumnBS(colLG, colMD, colSM) {
    var newColumnDiv = document.createElement("div");
    if (colLG !== null && colLG !== undefined && colLG >= 1 && colLG <= 12)
        newColumnDiv.className += newColumnDiv.className ? " col-lg-" + colLG : "col-lg-" + colLG;
    if (colMD !== null && colMD !== undefined && colMD >= 1 && colMD <= 12)
        newColumnDiv.className += newColumnDiv.className ? " col-md-" + colMD : "col-md-" + colMD;
    if (colSM !== null && colSM !== undefined && colSM >= 1 && colSM <= 12)
        newColumnDiv.className += newColumnDiv.className ? " col-sm-" + colSM : "col-sm-" + colSM;

    return newColumnDiv;
}

function creaDIV(classe) {
    var newDiv = document.createElement("div");
    newDiv.className = classe;
    return newDiv;
}

function creaLabel(id, testo, classe, labelFor) {
    var label = document.createElement("label");
    if (id !== null && id !== undefined)
        label.id = id;
    if (testo !== null && testo !== undefined)
        label.innerHTML = testo;
    if (classe !== null && classe !== undefined)
        label.className = classe;
    if (labelFor !== null && labelFor !== undefined) {
        label.labelFor = labelFor;
        label.htmlFor = labelFor;
    }

    return label;
}

function creaSpan(id, testo, classe, labelFor) {
    var span = document.createElement("span");
    if (id !== null && id !== undefined)
        span.id = id;
    if (testo !== null && testo !== undefined)
        span.innerHTML = testo;
    if (classe !== null && classe !== undefined)
        span.className = classe;
    if (labelFor !== null && labelFor !== undefined)
        span.labelFor = labelFor;

    return span;
}

function creaInputGenerico(id, name, classe, functionChange, required, miostile) {
    var inputGenerico = document.createElement("input");
    if (id !== null && id !== undefined)
        inputGenerico.id = id;
    if (name !== null && name !== undefined)
        inputGenerico.name = name;
    if (classe !== null && classe !== undefined)
        inputGenerico.className = classe;
    if (functionChange !== null && functionChange !== undefined)
        inputGenerico.onchange = functionChange;
    if (required !== null && required !== undefined)
        inputGenerico.required = required;
    if (miostile !== null && miostile !== undefined)
        inputGenerico.style = miostile;

    return inputGenerico;
}

function creaInputText(id, name, value, classe, style, maxLenght, functionChange, required) {
    var input = document.createElement("input");
    if (id !== null && id !== undefined)
        input.id = id;
    input.type = "text";
    if (name !== null && name !== undefined)
        input.name = name;
    if (value !== null && value !== undefined)
        input.value = value;
    if (classe !== null && classe !== undefined)
        input.className = classe;
    if (maxLenght !== null && maxLenght !== undefined)
        input.MaxLenght = maxLenght;
    if (functionChange !== null && functionChange !== undefined)
        input.onchange = functionChange;
    if (required !== null && required !== undefined)
        input.required = required;

    return input;
}

function creaInputCheckbox(id, name, classe, style, functionChange, required, checked, disabled) {
    var input = document.createElement("input");
    if (id !== null && id !== undefined)
        input.id = id;
    input.type = "checkbox";
    if (name !== null && name !== undefined)
        input.name = name;
    if (classe !== null && classe !== undefined)
        input.className = classe;
    if (style !== null && style !== undefined)
        input.style = style;
    if (functionChange !== null && functionChange !== undefined)
        input.onchange = functionChange;
    if (required !== null && required !== undefined)
        input.required = required;
    if (checked !== null && checked !== undefined)
        input.checked = checked;
    if (disabled !== null && disabled !== undefined)
        input.disabled = disabled;

    return input;
}

// Creazione dinamica elementi Html
function checkVirus(nome_file, file_allegato, objP_super_server) {

    var risultato = "";

    //chiamo il web service
    var strObjJSON = JSON.stringify({
        "nome_file": nome_file,
        "file_allegato": file_allegato
    });

    var parametri = JSON.stringify({ "objP_super_server": objP_super_server, "strObjJSON": strObjJSON });

    ajaxAgronicaSync(pathCoreWS + "AntiVirus/AntiVirus.asmx/CheckVirus",
        parametri,
        false,
        function (risposta) {
            risultato = risposta.RispostaStringa;

        }, function (risposta) {
            risultato = risposta.Errore;
        });


    return risultato;

}

function checkAntiVirus_isON(objP_super_server) {

    var AntiVirus_On = false;

    var parametri = JSON.stringify({
        "objP_super_server": objP_super_server
    });

    ajaxAgronicaSync(pathCoreWS + "AntiVirus/AntiVirus.asmx/CheckAntiVirus_isON",
        parametri,
        false,
        function (risposta) {
            AntiVirus_On = risposta.RispostaStringa;;
        });

    AntiVirus_On = Boolean(AntiVirus_On)
    return (AntiVirus_On);
}