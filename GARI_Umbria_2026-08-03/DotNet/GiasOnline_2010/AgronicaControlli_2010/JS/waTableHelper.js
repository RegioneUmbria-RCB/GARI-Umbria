
/*
Agronica WATable Helper


*/



//function objectFindByKey(array, key, value) {
//    //    $.logThis("cerco: " + value + " sul campo " + key);
//    //    $.logThis(array);
//    //    $.logThis('len ' + array.length);

//    for (var i = 0; i < array.length; i++) {
//        //        $.logThis("da vett.:" + array[i][key]);
//        //        $.logThis("cerco:" + value);


//        if (array[i][key] === value) {
//            //            $.logThis("trovato!");
//            return array[i];
//        }

//    }
//    return null;
//}

//var flag_footer = 0;


String.prototype.aTrimEnd = function (c) {
    c = c ? c : ' ';
    var i = this.length - 1;
    for (; i >= 0 && this.charAt(i) == c; i--);
    return this.substring(0, i + 1);
}

String.prototype.aContains = function (it) { return this.indexOf(it) != -1; };


function waReImpostaFiltri(waTable_ClientId, FiltriDaImpostare) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <param name="FiltriDaImpostare" type="string">Filtri da impostare</param>

}
function waEsportaFiltri(waTable_ClientId) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <returns type="String">String in formato json con i filtri impostati nelle caselle di testo</returns>

    var selFiltro = "";
    $(waTable_ClientId).find('input.filter').each(function () {
        var dt = $(this).attr('dt');
        if (dt !== undefined) {
            if ($(this).is(':checkbox')) { //se è un booleano
                if ($(this).hasClass("indeterminate")) //se il bool è in stato indefinito (li dà tutti)
                    selFiltro += dt + ":;";
                else { //se il bool ha un valore
                    if ($(this).is(':checked')) //se il bool è selezionato
                        selFiltro += dt + ":true;";
                    else //se il bool è selezionato
                        selFiltro += dt + ":false;";
                }
            }
            else { // se è un altro tipo di dato
                selFiltro += dt + ":" + $(this).val() + ";";
            }
        }
    });

    if (selFiltro.slice(-1) == ';') {
        selFiltro = selFiltro.slice(0, -1);
    }

    return selFiltro.aTrimEnd(";");
}

function waEsportaColonneVisibili(waTable_ClientId) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <returns type="String">String in formato json con i filtri impostati nelle caselle di testo</returns>

    var listaCol = "";
    $(waTable_ClientId).find('input.filter').each(function () {
        var dt = $(this).attr('dt');
        var nColonna = $(this).attr('nColonna');
        if (dt !== undefined) {
            listaCol = listaCol + dt + ":" + nColonna + ";";
        }
    });

    if (listaCol.slice(-1) == ';') {
        listaCol = listaCol.slice(0, -1);
    }

    return listaCol.aTrimEnd(";");
}


function InitWaTable(waTable_ClientId, waTable) {
    NomeColonnaDataTable(waTable_ClientId, waTable);

    if(waTable.option('filter'))
        AddClearButton(waTable_ClientId);

    addIconnCheckWaTable(waTable_ClientId, waTable);

   

    var oldFuncTableCreated = waTable.option('tableCreated');
    waTable.option('tableCreated', function () {
        // Verifico che non sia il primo giro (waTablePrecedenti ancora non esiste) e che non ci sia nColonna (l'evento scatta anche al cambio pagina)
        if (waTable != undefined && !!!$($(waTable_ClientId + ' input.filter')[0]).attr('nColonna')) {
            NomeColonnaDataTable(waTable_ClientId, waTable);
        }

        // Disabilito il Return sul filtro di tutte le Watable
        $('.watable input.filter').keypress(function (event) {
            if (event.keyCode == 10 || event.keyCode == 13)
                event.preventDefault();

        });

        //eseguo la funzione solo se esiste
        if (oldFuncTableCreated != undefined) {
            oldFuncTableCreated();
        }
    });

    //traduciInItaliano(waTable_ClientId);

    //ricalcolaPosizioneFooter();

}

function InitWaTableExport(waTable_ClientId, waTable, nomeVarSessionDt, prefissoNomeFile, url, objP_server) {
    InitWaTable(waTable_ClientId, waTable);

    //aggiungo l'evento table created per gestire la cancellazione delle nColonna.
    //se però la waTable aveva già un suo evento, accodo il suo al mio.
    var oldFuncTableCreated = waTable.option('tableCreated');
    waTable.option('tableCreated', function () {
        //verifico che non sia il primo giro (waTablePrecedenti ancora non esiste) e che non ci sia nColonna (l'evento scatta anche al cambio pagina)
        if (waTable != undefined && !!!$($(waTable_ClientId + ' input.filter')[0]).attr('nColonna')) {
            NomeColonnaDataTable(waTable_ClientId, waTable);
        }

        // Disabilito il Return sul filtro di tutte le Watable
        $('.watable input.filter').keypress(function (event) {
            if (event.keyCode == 10 || event.keyCode == 13)
                event.preventDefault();

        });

        AddExportButton(waTable_ClientId, waTable, nomeVarSessionDt, prefissoNomeFile, url, objP_server);

        //eseguo la funzione solo se esiste
        if (oldFuncTableCreated != undefined) {
            oldFuncTableCreated();
        }
    });

    

   //ricalcolaPosizioneFooter();

}

function traduciInItaliano(waTable_ClientId) {
    //Pulsante Rows
    var testoRighe = $(waTable_ClientId + " .pagesize button").html().replace("Rows", "Righe");
    $(waTable_ClientId + " .pagesize button").html(testoRighe);

    //Pulsante Columns
    var testoColonne = $(waTable_ClientId + " .columnpicker button").html().replace("Columns", "Colonne");
    $(waTable_ClientId + " .columnpicker button").html(testoColonne);

    //Indicazione numero righe Rows 1-10 of 114
    var testoRangeRighe = $(waTable_ClientId + " tfoot td:first-child p:first-child").html().replace("Rows", "Righe").replace("of", "di");
    $(waTable_ClientId + " tfoot td:first-child p:first-child").html(testoRangeRighe);
}

function addIconnCheckWaTable(waTable_ClientId, waTable) {

    var d = waTable.getData();

    $(waTable_ClientId).find('input.filter').each(function () {
        var ncolonna = $(this).attr('ncolonna');
        var dt = $(this).attr('dt');
        
        //controllo
        if (!!ncolonna)
        if (d.cols[ncolonna].filtrabilecheck == true) {
            var str = '<div class="popover-markup" data-placement="bottom"><div  class="trigger" data-placement="bottom">' +
                    '<span class="fa fa-check  selezionaWATABLE" ncolonna="' + ncolonna + '" dt="' + dt + '" data-placement="bottom" ></span>' +
                    '</div>' +
                    '<div class="content hide">' +
                    '<form role="form">';
            //valori distinct

            var lista = [];
            $.each(d.rows, function (index, value) {
                if ($.inArray(value[ncolonna], lista) == -1) {
                    lista.push(value[ncolonna]);

                    str = str + '<div class="checkbox"><label> ';
                    str = str + '<input type="checkbox" value="' + value[ncolonna] + '"  chk_dt="' + dt + '" onclick="FiltraCheck(this,' + "'" + waTable_ClientId + "'" + ');" />' + value[ncolonna];
                    str = str + '</label></div>';

                }
            });

            str = str + '</form>' +
                    '</div>';


            $(this).parent().append(str);
        }
    });
    initPopoup();
}

////Per ricalcolare il posizionamento del footer in seguito all'applicazioen del Watable
//function ricalcolaPosizioneFooter() {
//    var h = $(window).height();


//    // if ((h - $('#aspnetForm').height() <= 75) && (h - $('#aspnetForm').height() > 0)) {
//    if ((h - $('#aspnetForm').height() <= 75)) {
//        $('#footer').animate({
//            'marginTop': "+=75px" //moves down
//        });
//    }
//    //$('#footer').css("transition", "translateY(100px)");


//}

function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function FiltraCheck(o, waTable_ClientId) {
    //identifico i valori selezionati
    
    var str = "";
    $(o).parent().parent().parent().find(":checked").each(function () {
        if (str == "")
            str = "?" + escapeRegExp( $(this).val() );
        else
            str = str + "|" + escapeRegExp ( $(this).val() );
    });

    var n = $(o).attr('chk_dt');
    $(waTable_ClientId + ' [dt="' + n + '"]').val(str);
    if (n != "") {
        var e = jQuery.Event("keyup");
        e.which = 50; // # Some key code value
        $(waTable_ClientId + ' [dt="' + n + '"]').trigger(e);
    }

}



function initPopoup() {
    $('.popover-markup>.trigger').popover({
        html: true,
        content: function () {
            return $(this).parent().find('.content').html();
        }
    });
};


function AddClearButton(waTable_ClientId) {
    $(waTable_ClientId).prepend('<div class="btn btn-default btn-clear-tabella" onclick="cleanFiltriJson(' + "'" + waTable_ClientId + "'" + ');" style="margin: 10px 5px !important;"><span class="fa fa-eraser"></span> Pulisci Filtri</div>');
}

function AddExportButton(waTable_ClientId, waTable, nomeVarSessionDt, prefissoNomeFile, url, objP_Server) {
    $(waTable_ClientId + " table tfoot .btn-toolbar").append('<div class="btn btn-default btn-export-tabella" style="float: left;" onclick="exportWatable(' + "'" + waTable_ClientId + "','" + nomeVarSessionDt + "','" + prefissoNomeFile + "','" + url + "','" + objP_Server + "'" + ');"><span class="fa fa-file-excel-o"></span> Esporta su Excel</div>');
}


//funzione che imposta le proprietà nColonna e dt sul WaTable a partire dal JSON
function NomeColonnaDataTable(waTable_ClientId, waTable) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <param name="nColonna" type="string">Nome della colonna da impostare</param>

    //Metto in un array l'elenco dei nomi dei campi da noi aggiunti
    var dataDaCercare = waTable.getData(true, false);
    var arr = [];
    for (var prop in dataDaCercare.cols) {
        if(dataDaCercare.cols[prop].filter.toString() !='false' )
            arr.push(dataDaCercare.cols[prop]);
    }

    //ciclo per ogni colonna renderizzata
    var ElemTestata = $(waTable_ClientId + " thead tr:nth-of-type(1) a");
    var nElemTestata = ElemTestata.length;

    var InputElem = $(waTable_ClientId + " thead input.filter");
    var offSet2 =  nElemTestata - InputElem.length;

    for (i = 0; i < nElemTestata; i++) {
        //estraggo il nome visualizzato della colonna
        //var nomeInTestata = $(waTable_ClientId + " thead tr:nth-of-type(1) th:nth-child(" + i + ") a").text();
        var nomeInTestata = $(ElemTestata[i]).text();

        //cerco nell'array il dato con quel nome di colonna
        $(arr).each(function (index, obj) {
            if (obj.column == nomeInTestata) { //se trovo l'elemento in array...
                //salvo in varibili per semplificare il debug
                var elem = InputElem[i - offSet2];
                var dt = obj.dt;
                var column = obj.column;

                //...copio i dati
                $(elem).attr("dt", dt);
                $(elem).attr("nColonna", column);
            }
        });
    }
}

function waImpostaFiltro(waTable_ClientId, nColonna, valoreDaImpostare, bScatenaEvento) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <param name="nColonna" type="string">Nome della colonna da impostare (così come arriva da datatable lato server)</param>
    /// <param name="valoreDaImpostare" type="string">Testo Da impostare come filtro</param>
    /// <param name="bScatenaEvento" type="string">true = Esegue il filtro (se false si limita ad impostare i dati di filtro)</param>

    $(waTable_ClientId + ' [dt="' + nColonna + '"]').val(valoreDaImpostare);
    $(waTable_ClientId + ' [dt="' + nColonna + '"]').focus();

    if (bScatenaEvento) {
        var e = jQuery.Event("keyup");
        e.which = 50; // # Some key code value
        $(waTable_ClientId + ' [dt="' + nColonna + '"]').trigger(e);
    }

}



function getWaDataIntoString(myAgrWaTable, fieldName, separatore, checked) {
    /// <param name="myAgrWaTable" type="watable">Tabella da cui estrarre lista di elementi selezionati</param>
    /// <param name="fieldName" type="string">Nome del campo da cui estrarre gli ID</param>
    /// <param name="separatore" type="string">Carattere di separazione</param>
    /// <param name="checked" type="boolean">solo selezionati</param>
    /// <returns type="String">Elementi Selezionati separati da separatore</returns>

    var rval = "";
    var data = myAgrWaTable.getData(checked, false);

    if (data === undefined) {
        $.logThis("Tabella non definita o dati non selezionati");
        return "Errore."
    }


    for (var i = 0; i < data.rows.length; i++) {

        var adata = data.rows[i][fieldName];

        if (data.rows[i][fieldName] === undefined) {
            $.logThis("Colonna [" + fieldName + "] in  Tabella non definita..: elenco colonne..:" + getElencoColonne(data.rows[i]));
            return "Errore."
        }

        //$.logThis("data.rows[i][fieldName] = " + adata);
        if (!rval.aContains(adata))
            rval = rval + adata + separatore;
    }

    return rval.aTrimEnd(separatore);

}

function getCheckedWaDataIntoString(myAgrWaTable, fieldName, separatore) {
    /// <param name="myAgrWaTable" type="watable">Tabella da cui estrarre lista di elementi selezionati</param>
    /// <param name="fieldName" type="string">Nome del campo da cui estrarre gli ID</param>
    /// <param name="separatore" type="string">Carattere di separazione</param>
    /// <returns type="String">Elementi Selezionati separati da separatore</returns>

    var rval = "";
    var data = myAgrWaTable.getData(true, false);

    if (data === undefined) {
        $.logThis("Tabella non definita o dati non selezionati");
        return "Errore."
    }


    for (var i = 0; i < data.rows.length; i++) {

        var adata = data.rows[i][fieldName];

        if (data.rows[i][fieldName] === undefined) {
            $.logThis("Colonna [" + fieldName + "] in  Tabella non definita..: elenco colonne..:" + getElencoColonne(data.rows[i]));
            return "Errore."
        }

        //$.logThis("data.rows[i][fieldName] = " + adata);
        rval = rval + adata + separatore;
    }

    return rval.aTrimEnd(separatore);

}


function getElencoColonne(array) {
    var rval = "";

    $.each(array, function (k, v) {
        //display the key and value pair
        rval = rval + '[' + k + '] -';
    });

    return rval;
}

function generaTabellaDataTabella(tabellaFrom, tabellaTo, divTo, Colonne, filter, preFill, checkboxes) {


    var dataTo =
JSON.parse(
JSON.stringify(
tabellaFrom.getData(true, false)
)
);

    $(divTo).html('');

    tabellaTo = $(divTo).WATable({
        pageSize: 100,
        pageSizes: [10, 20, 30, 40, 50],
        filter: filter,
        preFill: preFill,
        checkboxes: checkboxes,
        tableCreated: function (data) {
            impostaTabella(divTo);
        }

    }).data('WATable').setData(dataTo);
    InitWaTable(divTo, tabellaTo);


    return tabellaTo;
}


function clickAll(divTabella) {
    /// <param name="divTabella" type="string">div che contiene la tabella dove selezionare tutti gli elementi</param>
    /// <returns type="String">Elementi Selezionati separati da separatore</returns>

    divTabella = AddHashIfNotExists(divTabella);
    $(divTabella + " .watable .checkToggle").click();
}

function AddHashIfNotExists(divTabella) {
    return AddPreIfNotExists(divTabella, "#");
}

function AddDotIfNotExists(divTabella) {
    return AddPreIfNotExists(divTabella, ".")
}

function AddPreIfNotExists(divTabella, pre) {
    var pre1 = "";
    if (!(divTabella[0] == pre))
        pre1 = pre;
    return pre1 + divTabella;
}

function nascondiColonnaTabellaByName(divTabella, nomeColonna) {

    var i = getIndexByName(divTabella, nomeColonna);
    impostaColTabella(divTabella, i);

}


function getIndexByName(divTabella, nomeColonna) {

    var tt = $(divTabella + " .watable thead tr");
    var i = 0;

    tt.each(function (index) {

        $.logThis("$(this) = ");
        $.logThis($(this));
        $.logThis("$(this).val() = " + $(this).val() + "  nomeColonna = " + nomeColonna + ", i = " + i);
        if ($(this).val() == nomeColonna) {

            return i;
        }
        i++;
    });

    $.logThis("iiii= " + i);
    return i;

}


function nascondiColonnaTabella(divTabella, nomeColonna) {


    impostaColTabella(divTabella, nomeColonna);

}



function impostaTabella(divTabella) {

    var divTabella = AddHashIfNotExists(divTabella);


    var idx = 1;

    if ($(divTabella + " .watable thead tr th .checkToggle").val() === undefined) {
        idx = 0;
    }

    impostaColTabella(divTabella, idx);

}

function impostaColTabella(divTabella, idx) {
    var headerRows = $(divTabella + " .watable thead tr"); // skip the header row
    var bodyRows = $(divTabella + " .watable tbody tr"); // skip the header row
    var footerRows = $(divTabella + " .watable tfoot tr"); // skip the header row

    impostaVisTabella(headerRows, 'th', idx);
    impostaVisTabella(bodyRows, 'td', idx);
    impostaVisTabella(footerRows, 'td', idx);
}


function impostaVisTabella(riga, intestazioneColonna, colonna) {


    //$.logThis("impostaVisTabella(" + riga + ", " + intestazioneColonna + " , " + colonna + ")");

    //scorre le colonne e nasconde quella con intestazioneColonna
    riga.each(function (index) {
        $(this).find(intestazioneColonna + ':eq(' + colonna + ')').hide();
    });

}



//   !!!!!    NON Funziona, da completare ..    !!!!!!!
function sommaValoriColonna_ImpostaUltimaRiga(divTabella, intestazioneColonna, controlclass) {
    /// <param name="divTabella" type="string">div che contiene la tabella dove selezionare tutti gli elementi</param>
    /// <returns type="String">Somma Elementi</returns>


    //scorre le righe, le colonne e somma i dati parziali (presenti in un unico controllo con classe specificata)

    divTabella = AddHashIfNotExists(divTabella);

    var righe = $(divTabella + " .watable tr");

    var somma;
    var vC = 0;

    righe.each(function (index) {

        var vC = $(this).find('td:eq(' + intestazioneColonna + ') .' + controlclass).val();

        somma = somma + vC;
        $.logThis(vC);


    });

    somma = somma - vC;

    $(divTabella + ' .watable tr:last td:eq(' + intestazioneColonna + ') .' + controlclass).val(somma);

    return somma;

}




function sommaValoriColonna_ImpostaUltimaRiga_loc(divTabella, controlclass) {

    $.logThis("sommaValoriColonna_ImpostaUltimaRiga_loc");

    divTabella = AddHashIfNotExists(divTabella);
    controlclass = AddDotIfNotExists(controlclass);

    var somma = 0;
    var vC = 0;

    var last;

    $(controlclass).each(function (index) {

        last = $(this);
        vC = parseFloat(last.val().toString().replace(',', '.'));
        somma = somma + vC;
        $.logThis("sommatoria = " + somma.toString());

    });


    somma = somma - vC;

    $(last).val(somma.toString().replace('.', ','));


    $.logThis(somma);

    return somma;

}




/*
Marco G. 10/03/2015, gestione tipi dato non stringa
*/
function AgroWA_Table_sistemaDati(json) {
    //console.log(json);
    if (json == null) //se non ho dati, esco direttamente
        return

    var colNum = new Array();
    var colBool = new Array();
    var colDate = new Array();

    for (var p in json.cols) {
        if (json.cols.hasOwnProperty(p)) {
            if (json.cols[p].type == "number")
                colNum.push(p);
            else if (json.cols[p].type == "bool")
                colBool.push(p);
            else if (json.cols[p].type == "date") {
                colDate.push(p);
            }

        }
    }

    for (var i = 0; i < json.rows.length; i++) {
        //sistemo i numeri
        for (var j = 0; j < colNum.length; j++) {
            json.rows[i][colNum[j]] = json.rows[i][colNum[j]].toString().replace(",", ".");
        }
        //sistemo i booleani
        for (var j = 0; j < colBool.length; j++) {
            if (json.rows[i][colBool[j]] == "True")
                json.rows[i][colBool[j]] = 1;
            else if (json.rows[i][colBool[j]] == "False")
                json.rows[i][colBool[j]] = 0;
        }
        //sistemo le date
        for (var j = 0; j < colDate.length; j++) {
            if (json.rows[i][colDate[j]] != "") {
                //controllo . o :
                var data;
                var r = json.rows[i][colDate[j]].match(/^\s*([0-9]+)\s*\/\s*([0-9]+)\s*\/\s*([0-9]+)(.*)$/);
                //su javascript i mesi vanno da 0 a 11 quindi bisogna fare -1 per ottenere il mese giusto
                if (json.rows[i][colDate[j]].split(':').length > 1) {
                    data = new Date(r[3], parseInt(r[2]) - 1, r[1], r[4].split(':')[0], r[4].split(':')[1], r[4].split(':')[2], 0);
                } else {
                    data = new Date(r[3], parseInt(r[2]) - 1, r[1], r[4].split('.')[0], r[4].split('.')[1], r[4].split('.')[2], 0);
                }

                json.rows[i][colDate[j]] = data.getTime();
            }
        }
    }
}
/*fine gestione tipo dato non stringa*/

/*
Marco G. 10/03/2015, tooltip per tipi di dato
*/
function AgroWA_Table_Tooltip_Date() {
    return 'Oggi:<br/>0..1<br/>Tutti eccetto oggi:<br/>!0..1<br/>Ultima settimana oggi escluso:<br/>-7..0'
}
function AgroWA_Table_Tooltip_Number() {
    return 'Valori da 10 a 20:<br/>10..20<br/>Tutti i valori tranne quelli tra 10 e 20:<br/>!10..20<br/>Esattamente 50:<br/>=50'
}
function AgroWA_Table_Tooltip_Bool() {
    return 'Passa tra:<br/>indeterminato,<br/>presente,<br/>assente'
}
function AgroWA_Table_Tooltip_String() {
    return 'Trova Mario Rossi:<br/>Mario Rossi<br/>Trova Mario e Luca Rossi(Regex):<br/>?Mario Rossi|Luca Rossi<br/>Trova tutti tranne Mario Rossi:<br/>!Mario Rossi'
}

/*
Marco G. 12/03/2015, funzione che dato il watable.GetData() ritorna il file XLS (XML)
*/
function EsportaLatoClient(waTableGetData) {
    //http://jsfiddle.net/kmqz9/262/
    WaitFrame.show();

    var dati = waTableGetData;
    var testTypes = new Array;
    var testJson = dati.rows;

    //sistemo le colonne con le date portandole ad un formato più umano su XLS
    var colDate = new Array()
    for (var p in dati.cols) {
        if (dati.cols.hasOwnProperty(p)) {
            if (dati.cols[p].type == "date")
                colDate.push(p);
        }
    }

    for (var i = 0; i < dati.rows.length; i++) {
        for (var j = 0; j < colDate.length; j++) {
            if (dati.rows[i][colDate[j]] != "") {
                var data = new Date(dati.rows[i][colDate[j]])
                data = new Date(dati.rows[i][colDate[j]] - data.getTimezoneOffset() * 60000); //sistemo il timezone perché isostring lo ignora
                //data.setHours(0, -data.getTimezoneOffset(), 0, 0); 
                dati.rows[i][colDate[j]] = data.toISOString();
            }
        }
    }

    //creo l'elenco delle colonne
    for (var d in dati.cols) {
        if (dati.cols.hasOwnProperty(d)) {
            var tipo = dati.cols[d].type
            if (tipo == "bool") { tipo = "boolean"; }
            if (tipo == "date") { tipo = "DateTime"; }
            testTypes[(dati.cols[d].column)] = tipo[0].toUpperCase() + tipo.substring(1);

        }
    }

    //funzione che scrive la parte iniziale del file XLS
    emitXmlHeader = function () {
        return '<?xml version="1.0"?>\n' +
           '<ss:Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">\n' +
           '<ss:Styles>\n' +
           ' <ss:Style ss:ID="s1">\n' +
           '  <ss:Font ss:Bold="1"/>\n' +
           ' </ss:Style>\n' +
           ' <ss:Style ss:ID="s2">\n' +
           '  <ss:NumberFormat ss:Format="Short Date"/>\n' +
           ' </ss:Style>\n' +
           '</ss:Styles>\n' +
           '<ss:Worksheet ss:Name="Foglio1">\n' +
           '<ss:Table>\n\n';
    };

    //funzione che scrive la parte finale del file XLS
    emitXmlFooter = function () {
        return '\n</ss:Table>\n' +
           '</ss:Worksheet>\n' +
           '</ss:Workbook>\n';
    };

    //funzione che scrive il file XLS
    jsonToSsXml = function (jsonObject) {
        var row, col, xml;
        var data = typeof jsonObject != "object" ? JSON.parse(jsonObject) : jsonObject;

        xml = emitXmlHeader();

        //se una colonna è di tipo data, aggiungo lo stile
        var i = 1
        for (col in testTypes) {
            if (testTypes[col] == "DateTime") {
                xml += '  <ss:Column ss:Index="' + i + '" ss:StyleID="s2"/>\n';
            }
            i++;
        }

        //scrivo i nomi delle colonne
        xml += '<ss:Row ss:StyleID="s1">\n';
        for (col in testTypes) {
            xml += '  <ss:Cell>\n';
            xml += '    <ss:Data ss:Type="String">';
            xml += col + '</ss:Data>\n';
            xml += '  </ss:Cell>\n';
        }
        xml += '</ss:Row>\n';

        //scrivo le righe
        for (row = 0; row < data.length; row++) {
            xml += '<ss:Row>\n';

            for (col in data[row]) {
                //faccio l'if, perché quando una colonna è filtrata, il getData ritorna una colonna agiguntiva che termina con "Format"
                if (col.indexOf("Format", col.length - ("Format").length) === -1) {
                    xml += '  <ss:Cell>\n';
                    xml += '    <ss:Data ss:Type="' + testTypes[col] + '">';
                    xml += data[row][col] + '</ss:Data>\n';
                    xml += '  </ss:Cell>\n';
                }
            }
            xml += '</ss:Row>\n';
        }
        xml += emitXmlFooter();
        return xml;
    };

    var ssxml = jsonToSsXml(testJson)

    //faccio pulizia
    testJson = "";
    dati = "";
    testTypes = "";
    testJson = "";

    return ssxml;
}





function waEsportaFiltriJSON(waTable_ClientId) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <returns type="String">String in formato json con i filtri impostati nelle caselle di testo</returns>
    var jsonRisposta = {};
    $(waTable_ClientId).find('input.filter').each(function () {
        jsonRisposta[$(this).attr('dt')] = $(this).val();
    });

    //salvo in localStorage
    var nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + waTable_ClientId;
    localStorage[nomeChiave] = JSON.stringify(jsonRisposta);
    return jsonRisposta;
}

function waReImpostaFiltriJSON(waTable_ClientId) {
    /// <param name="waTable_ClientId" type="string">Client ID del div contente il WaTable</param>
    /// <returns type="String">String in formato json con i filtri impostati nelle caselle di testo</returns>
    var nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + waTable_ClientId;

    var jsonFiltriDaImpostare = localStorage[nomeChiave];
    if (!!jsonFiltriDaImpostare) {
        jsonFiltriDaImpostare = JSON.parse(jsonFiltriDaImpostare);

        $(waTable_ClientId).find('input.filter').each(function () {
            $(this).val(jsonFiltriDaImpostare[$(this).attr('dt')]);
            var n = $(this).attr('dt');
            if (n != "") {
                var e = jQuery.Event("keyup");
                e.which = 50; // # Some key code value
                $(waTable_ClientId + ' [dt="' + n + '"]').trigger(e);
            }
        });
        //cancello dopo il ripristino
        localStorage.removeItem(nomeChiave);
    }
}

function cleanFiltriJson(waTable_ClientId) {
    var nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + waTable_ClientId;
    localStorage.removeItem(nomeChiave);
    $(waTable_ClientId).find('input.filter').each(function () {
        var n = $(this).attr('dt');
        if (n != "") {
            $(this).val('');
            var e = jQuery.Event("keyup");
            e.which = 50; // # Some key code value
            $(waTable_ClientId + ' [dt="' + n + '"]').trigger(e);
        }
    });
}

//ESPORTA SU EXCEL
function exportWatable(waTable_ClientId, nomeVarSessionDt, prefissoNomeFile, url, objP_server) {

    if (objP_server == undefined) {
        objP_server = "";
    }

    //estrapolo i filtri del Watable
    var listaFiltriStr = waEsportaFiltri(waTable_ClientId);
    //estrapolo i nomi delle colonne visibili (i nomi sono quelli del datatable non quelli visualizzati)
    var listaColonneVisibili = waEsportaColonneVisibili(waTable_ClientId);
    var parametri = "{ listaFiltriStr: '" + listaFiltriStr + "', listaColonneVisibili: '" + listaColonneVisibili + "', nomeVarSessionDt: '" + nomeVarSessionDt + "', prefissoNomeFile: '" + prefissoNomeFile + "', objP_server: '" + objP_server + "'}"

    ajaxAgronica(url + "/EsportaSuExcel",
                    parametri,
                    function (risposta) {
                        //window.open(risposta.RispostaStringa);

                        //La funzione mi ritorna un array di byte che faccio scaricare al volo
                        var datiEsportati = risposta.RispostaStringa;
                        datiEsportati = new Uint8Array(datiEsportati);
                        var contentType = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
                        var blob = new Blob([datiEsportati], { 'type': contentType });
                        var data = new Date(Date.now());
                        var nomeFile = prefissoNomeFile + data.getFullYear() + '-' + (parseInt(data.getMonth()) + 1) + '-' + data.getDate() + '_' + data.getHours() + '-' + data.getMinutes() + '-' + data.getSeconds() + '-' + data.getMilliseconds() + '.xlsx';

                        //in base al tipo di browser faccio scaricare
                        var isIE = /*@cc_on!@*/false || !!document.documentMode;
                        if (isIE) { //se Ã¨ IE
                            navigator.msSaveOrOpenBlob(blob, nomeFile)
                        }
                        else { //Se sono gli altri...Chrome...
                            var aLink = document.createElement('a');
                            //var evt = document.createEvent("HTMLEvents");
                            //evt.initEvent("click", true, false);
                            aLink.href = window.URL.createObjectURL(blob);
                            aLink.download = nomeFile
                            //aLink.dispatchEvent(evt);
                            aLink.click();
                        }
                    }, null);
}


//*** @Paolo: funzioni per l'aggiunta di bottoni in watable ***//

function addWatableButton(id_btn, id_watable, watable, funzione) {

    // memorizzo i dati della watable
    var data = watable.getData();

    var colonne = "";
    // Creo array colonne reali
    $('#' + id_watable + ' thead .sort th a').each(function () {
        //Controllo se la colonna è visibile
        if ($(this).parent().is(':visible')) {

            //Controllo se la colonna contiene le operazioni
            if ($(this).text() != "Operazioni" && $(this).text() != "Mostra Movimenti")
                colonne += $(this).text() + ",";
        }

        

    });
    colonne = colonne.slice(0, -1);

    switch (funzione) {
        case 'exportExcel':
            // Aggiungo il bottone alla tabella
            $('#' + id_watable + '').find('.btn-toolbar').append('<button type="button" class="btn btn-default" id="' + id_btn + '" style="margin-top: 0")"><i class="fa fa-file-excel-o"></i> Export in Excel</button>');

            break;

    }

    // gestione click bottone
    $('#' + id_btn + '').click(function () {
        // controllo se sono stati abilitati dei filtri sulla watable
        var data_filtered = watable.getData(false, true);
        if (data_filtered.rows.length > 0)
            data = data_filtered;

        var result = gestioneFunzioniSuWatable(funzione, data, colonne);
    });

}

function gestioneFunzioniSuWatable(func, data, colonne) {
    var result;
    var base = "";

    var url = window.location.href;
   // var res = url.split(".");

    res = url.split("/");

    for (i = 0; i < res.length - 2; i++) {
        if (i == res.length - 3)
            base = base + res[i];
        else
            base = base + res[i] +"/";
    }
    //base = base.replace(/http:/g, 'http://');
    //alert(base + "/PannelloDiControllo/PannelloDiControllo_ScriptService.asmx/EsportaSuExcel2");

    switch (func) {
        case 'exportExcel':
            var data_string = JSON.stringify(data.rows);
            var prefissoNomeFile = "Export_";

            // WebService per l'esportazione della watable in excel

            //ajaxAgronica("../../../PannelloDiControllo/PannelloDiControllo_ScriptService.asmx/EsportaSuExcel2",
            ajaxAgronica(base + "/PannelloDiControllo/PannelloDiControllo_ScriptService.asmx/EsportaSuExcel2",
                    "{data:'" + data_string + "', colonne:'" + JSON.stringify(colonne) + "'}",
                    function (risposta) {
                        //window.open(risposta.RispostaStringa);

                        //La funzione mi ritorna un array di byte che faccio scaricare al volo
                        var datiEsportati = risposta.RispostaStringa;
                        datiEsportati = new Uint8Array(datiEsportati);
                        var contentType = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,';
                        var blob = new Blob([datiEsportati], { 'type': contentType });
                        var data = new Date(Date.now());
                        var nomeFile = prefissoNomeFile + data.getFullYear() + '-' + (parseInt(data.getMonth()) + 1) + '-' + data.getDate() + '_' + data.getHours() + '-' + data.getMinutes() + '-' + data.getSeconds() + '-' + data.getMilliseconds() + '.xlsx';

                        //in base al tipo di browser faccio scaricare
                        var isIE = /*@cc_on!@*/false || !!document.documentMode;
                        if (isIE) { //se Ã¨ IE
                            navigator.msSaveOrOpenBlob(blob, nomeFile);
                        }
                        else { //Se sono gli altri...Chrome...
                            var aLink = document.createElement('a');
                            aLink.setAttribute("id", nomeFile);
                            var evt = document.createEvent("HTMLEvents");
                            evt.initEvent("click", true, false);
                            aLink.href = window.URL.createObjectURL(blob);
                            aLink.download = nomeFile;
                            //aLink.dispatchEvent(evt);
                            aLink.click();
                        }
                    }, null);

            break;

    }
    return result;
}

//////////////////////////////