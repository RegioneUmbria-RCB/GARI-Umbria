// #region PIANO DISTRIBUZIONE
var indirizzohttp = "./PianoConcimazione_MenuBS.aspx";

function NuovoPD() {
    if (current_PC != null) {
        creaPianoDistr(current_PC);
    }
    else {
        alert(TraduzioneMultiResx(MenuBSResx, "ErroreChiudereElencoRiprovare.", "Errore. Chiudere l'elenco e riprovare."));
    }
}

async function BloccaSbloccaPUA(blocca) {
    WaitFrame.show();
    var kendoGrid = $("#kendo_PianiConcimazione").data("kendoGrid")
    var dataSource = kendoGrid.dataSource.data();
    let arrPUA = new Array();
    for (var i = 0; i < dataSource.length; i++) {
        if (dataSource[i].Selected == true) {
            arrPUA.push(dataSource[i].PC_Testata_Cod + "_" + dataSource[i].Regolamento_Cod);
        }
    }
    await ws_BloccaSbloccaPUA(arrPUA, blocca);
    Aggiorna();
    WaitFrame.hide();
    if (blocca == 0) {
        kendo.alert(TraduzioneMultiResx(MenuBSResx, "PUASbloccati", "I PUA sono stati sbloccati"))
    } else {
        kendo.alert(TraduzioneMultiResx(MenuBSResx, "PUABloccati", "I PUA sono stati bloccati"))
    }
}

function GrigliaKendoDistribuzione(div) {

    var funzioniCRUD = {
        funzioneRead: kendo_PianiDistribuzione_Leggi
        //,funzioneUpdate: Aggiorna
    };

    var idModel = "kendoKey";
    var campiKendoModel = kReadPianoDistribuzione_mod(); //kendo_model
    var colonneKendoGrid = kReadPianoDistribuzione_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        sortable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        filterable: {
            mode: "row"
        },
        //                sortable: true, 
        //                resizable: false,
        //                columnMenu: true,
        //                filterable: true,
        groupable: false,
        scrollable: false,
        pdf: false, excel: false
        //                ,toolbarCommands: ["templateBtnFiltraColonne"]
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBinding: onDataBindingRighePianoDistribuzione
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

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

function onDataBindingRighePianoDistribuzione(e) {
    var grid = $("#kendo_PianoDistribuzione").data('kendoGrid');
    var items = e.sender.items();
    items.each(function (index) {
        var dataItem = grid.dataItem(this);
        if (dataItem.blocco_flag == 1) {
            this.className = "k-master-row kendoRiga_AgendaOperazBloccata dpiOn";
        }
    });
}

function kReadPianoDistribuzione_mod() {
    return jSonParsed_Kendo_PianiDistribuzione.kendo_model;
}

function kReadPianoDistribuzione_col() {

    var columns = jSonParsed_Kendo_PianiDistribuzione.kendo_columns;

    if (UtenteAbilitatoScritturaPC) {
        columns.unshift(
            {
                command: [{
                    template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPD(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='modifica' onclick=modificaPD(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-trash-o fa-2x del_elem' title='elimina' onclick=eliminaPD(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPD(this.closest('tr'),this.closest('.k-grid'))></span>"
                }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "200px"
            }
        );
    }

    else {
        columns.unshift(
            {
                command: [{
                    template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPD(this.closest('tr'),this.closest('.k-grid'))></span>" +
                        "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPD(this.closest('tr'),this.closest('.k-grid'))></span>"
                }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "85px"
            }
        );
    }
    return columns;
}

function kendo_PianiDistribuzione_Leggi(options) {

    options.success(jSonParsed_Kendo_PianiDistribuzione.kendo_rows);

}


// #endregion

// #region PIANO CONCIMAZIONE / PIANO NUTRIZIONALE
function GrigliaKendoConcimazione(div) {

    var funzioniCRUD = {
        funzioneRead: kendo_PianiConcimazione_Leggi
        //,funzioneUpdate: Aggiorna
    };

    if (permessoBloccaPC && TipologiaPagina == PianoConcimazione) {
        funzioniCRUD.checkBoxFunction = function () {
            var checked = this.checked;
            var row = $(this).parents("tr");
            var grid = $('#kendo_PianiConcimazione').data("kendoGrid");
            var dataItem = grid.dataItem(row);
            dataItem.Selected = checked;
            dataItem.dirty = true;
        };
    }

    if (permessoBloccaPUA && TipologiaPagina == PUA) {
        funzioniCRUD.checkBoxFunction = function () {
            var checked = this.checked;
            var row = $(this).parents("tr");
            var grid = $('#kendo_PianiConcimazione').data("kendoGrid");
            var dataItem = grid.dataItem(row);
            dataItem.Selected = checked;
            dataItem.dirty = true;
        };
    }

    //14/10/214 Anna: Piano Nutrizionale
    //if (permessoBloccaPianoNutrizionale && TipologiaPagina == 3) {
    //    funzioniCRUD.checkBoxFunction = function () {
    //        var checked = this.checked;
    //        var row = $(this).parents("tr");
    //        var grid = $('#kendo_PianiConcimazione').data("kendoGrid");
    //        var dataItem = grid.dataItem(row);
    //        dataItem.Selected = checked;
    //        dataItem.dirty = true;
    //    };
    //}

    var template;
    if (TipologiaPagina == PianoConcimazione) {
        template = ["templatePianoConcimazione"]
    }

    if (TipologiaPagina == PUA) {
        template = ["templatePUA"]
    }

    //14/10/214 Anna: Piano Nutrizionale
    //if (TipologiaPagina == 3) {
    //    template = ["templatePianoNutrizionale"]
    //}

    var idModel = "PC_Testata_Cod";
    var campiKendoModel = kReadPianoConcimazione_mod(); //kendo_model
    var colonneKendoGrid = kReadPianoConcimazione_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        sortable: true,
        reorderable: true,
        resizable: true,
        columnMenu: true,
        filterable: {
            //mode: "row"
        },
        //                sortable: true, 
        //                resizable: false,
        //                columnMenu: true,
        //                filterable: true,
        groupable: false,
        scrollable: false,
        toolbarCommands: template,
        pdf: false,
        excel: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }

        //                ,toolbarCommands: ["templateBtnFiltraColonne"]
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDataBound: onDataBoundRighePianoConcimazione,
        funzioneDaChiamareDopoDataBound: postDataBoundRighePianoConcimazione
    };

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

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

    if (permessoBloccaPC && TipologiaPagina == PianoConcimazione) {
        $("#btnSbloccaPC").show();
        $("#btnBloccaPC").show();
    }

    if (permessoBloccaPUA && TipologiaPagina == PUA) {
        $("#btnSbloccaPUA").show();
        $("#btnBloccaPUA").show();
    }

    //11/10/214 Anna: Piano Nutrizionale
    //if (permessoBloccaPianoNutrizionale && TipologiaPagina == 4) {
    //    $("#btnSbloccaPianoNutrizionale").show();
    //    $("#btnBloccaPianoNutrizionale").show();
    //}
}

async function BloccaSbloccaPC(blocca) {
    WaitFrame.show();
    var kendoGrid = $("#kendo_PianiConcimazione").data("kendoGrid")
    var dataSource = kendoGrid.dataSource.data();
    let arrPC = new Array();
    for (var i = 0; i < dataSource.length; i++) {
        if (dataSource[i].Selected == true) {
            arrPC.push(dataSource[i].PC_Testata_Cod);
        }
    }
    await ws_BloccaSbloccaPC(arrPC, blocca);
    Aggiorna();
    WaitFrame.hide();
    if (blocca == 0) {
        kendo.alert(TraduzioneMultiResx(MenuBSResx, "PianiConcimazioneSbloccati", "I Piani di Concimazione sono stati sbloccati"))
    } else {
        kendo.alert(TraduzioneMultiResx(MenuBSResx, "PianiConcimazioneBloccati", "I Piani di Concimazione sono stati Bloccati"))
    }
}

function postDataBoundRighePianoConcimazione(e) {
    aggiornaStyle();
    //alert('postDataBindingRighePianoConcimazione');
}

function kReadPianoConcimazione_mod() {
    return jSonParsed_Kendo_PianiConcimazione.kendo_model;
}

function onDataBoundRighePianoConcimazione(e) {

    var grid = $("#kendo_PianiConcimazione").data('kendoGrid');
    var items = e.sender.items();
    items.each(function (index) {
        var dataItem = grid.dataItem(this);
        if (dataItem.blocco_flag == 1) {
            this.className = "k-master-row kendoRiga_AgendaOperazBloccata dpiOn";
        }
    });

    if (TipologiaPagina === PianoNutrizionale || TipologiaPagina === PianoNutrizionale_IBF) {
        //Per le righe del PianoNutrizionale_IBF, nascondo ALLEGATI E STAMPE
        items.each(function (index) {
            var dataItem = grid.dataItem(this);
            if (dataItem.Regolamento_Tipo == 5) {
                var azioni = this.firstChild.childNodes
                azioni.forEach(function (button) {
                    if (button.classList.contains('print_elem')) {
                        button.remove()
                    }
                })
                azioni.forEach(function (button) {
                    if (button.classList.contains('alleg_elem')) {
                        button.remove()
                    }
                })
            }
        });
        grid.autoFitColumn()
    }

}

function kReadPianoConcimazione_col() {

    var columns = jSonParsed_Kendo_PianiConcimazione.kendo_columns;

    if (TipologiaPagina === PUA) { // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF

        if (UtenteAbilitatoScritturaPUA) {

            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-trash-o fa-2x del_elem' title='elimina' onclick=eliminaPC(this.closest('tr'),this.closest('.k-grid'),2)></span>" +
                            "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'),2)></span>" // +
                        // "<span class='fa fa-print fa-2x print_elem' title='stampa registro fertilizzazioni' onclick=stampaRegistroFertilizzazioni(this.closest('tr'),this.closest('.k-grid'))></span>"
                    }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "85px"
                }
            );

        } else {

            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'),2)></span>"
                    }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "25px"
                }
            );
        }

    } else if (TipologiaPagina === PianoNutrizionale || TipologiaPagina === PianoNutrizionale_IBF) {  // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF
        // 18/10/21 Anna: Piano Nutrizionale
        if (UtenteAbilitatoScritturaPianoNutrizionale) {
            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='modifica' onclick=modificaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-trash-o fa-2x del_elem' title='elimina' onclick=eliminaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-link fa-2x alleg_elem' title='allegato' onclick=apriAllegatoPC(this.closest('tr'),this.closest('.k-grid'))></span>"
                    }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "175px"
                }
            );

        } else {

            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-link fa-2x alleg_elem' title='allegato' onclick=apriAllegatoPC(this.closest('tr'),this.closest('.k-grid'))></span>"
                    },], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "100px"
                }
            );

        }

    } else {

        if (UtenteAbilitatoScritturaPC) {

            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='modifica' onclick=modificaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-trash-o fa-2x del_elem' title='elimina' onclick=eliminaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-link fa-2x alleg_elem' title='allegato' onclick=apriAllegatoPC(this.closest('tr'),this.closest('.k-grid'))></span>"
                    }], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "175px"
                }
            );

        } else {

            columns.unshift(
                {
                    command: [{
                        template: "<span class='fa fa-info fa-2x info_elem' title='info' onclick=infoPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-print fa-2x print_elem' title='stampa' onclick=stampaPC(this.closest('tr'),this.closest('.k-grid'))></span>" +
                            "<span class='fa fa-link fa-2x alleg_elem' title='allegato' onclick=apriAllegatoPC(this.closest('tr'),this.closest('.k-grid'))></span>"
                    },], title: TraduzioneMultiResx(MenuBSResx, "Azioni", "Azioni"), width: "100px"
                }
            );

        }
    }

    if (TipologiaPagina === PUA) { // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF

        columns.push(
            { field: "Tipo_Des", title: TraduzioneMultiResx(MenuBSResx, "Metodo", "Metodo"), template: template_tipo_pua, editable: false },
            {
                command: [
                    { text: TraduzioneMultiResx(MenuBSResx, "DefinizioneConsistenza", "Definizione Consistenza"), click: defConsistenze, className: "btnWidth" },
                    { text: TraduzioneMultiResx(MenuBSResx, "PianoDistribuzione", "Piano Distribuzione"), click: apriElencoPianoDistr, className: "btnWidth" },
                    { text: TraduzioneMultiResx(MenuBSResx, "VerificaIndiciBilancio", "Verifica Indici Bilancio"), click: verificaIndiciBilancio, className: "btnWidth" }
                ], title: "", width: "200px"
            }
        );

    } else if (TipologiaPagina === PianoNutrizionale || TipologiaPagina === PianoNutrizionale_IBF) {  // 1 = Piano concimazione; 2 = PUA; 4 = Piano Nutrizionale Coprob (PRIVATO); 5 = Piano Nutrizionale IBF

        columns.push(
            { field: "Tipo_Des", title: TraduzioneMultiResx(MenuBSResx, "Tipo", "Tipo"), template: template_tipo_pn, editable: false }
        );

    } else {

        columns.push(
            { field: "Tipo_Des", title: TraduzioneMultiResx(MenuBSResx, "Tipo", "Tipo"), template: template_tipo, editable: false },
            {
                command: [
                    { text: TraduzioneMultiResx(MenuBSResx, "PianoDistribuzione", "Piano Distribuzione"), click: apriElencoPianoDistr, className: "btnWidth" }
                ], title: "", width: "200px"
            }
        );
    }

    return columns;
}

function kendo_PianiConcimazione_Leggi(options) {

    options.success(jSonParsed_Kendo_PianiConcimazione.kendo_rows);

}

function stampaPC(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    stampaPC_daDataItem(dataItem);
}





// #endregion

function SalvaSuCookieCriteriDiRicerca() {

    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    //var date = 0;

    var data_inizio = start.value();
    if (data_inizio !== undefined && data_inizio !== "") {
        $.removeCookie("PianoConcimazione_MenuBS.FiltroDataInizio");
        $.cookie("PianoConcimazione_MenuBS.FiltroDataInizio", data_inizio.toJSON(), { expires: date, path: '/' });
    }

    var data_fine = end.value();
    if (data_fine !== undefined && data_fine !== "") {
        $.removeCookie("PianoConcimazione_MenuBS.FiltroDataFine");
        $.cookie("PianoConcimazione_MenuBS.FiltroDataFine", data_fine.toJSON(), { expires: date, path: '/' });
    }

}

function ripristinaPreferenzeFiltriDateDaCookie(start, end) {

    var preferenzaDataInizio = $.cookie("PianoConcimazione_MenuBS.FiltroDataInizio");
    if (preferenzaDataInizio !== undefined) {
        start.value(new Date(preferenzaDataInizio));
        if (start.value() === "") {
            $.removeCookie("PianoConcimazione_MenuBS.FiltroDataInizio");
        }
    }

    var preferenzaDataFine = $.cookie("PianoConcimazione_MenuBS.FiltroDataFine");
    if (preferenzaDataFine !== undefined) {
        end.value(new Date(preferenzaDataFine));
        if (end.value() === "") {
            $.removeCookie("PianoConcimazione_MenuBS.FiltroDataFine");
        }
    }

}

function onClose() {
    //undo.fadeIn();
    jSonParsed_Kendo_PianiDistribuzione = null;
    current_PC = null;
}

function Aggiorna() {
    WaitFrame.show();

    SalvaSuCookieCriteriDiRicerca();
    kendo_PianiConcimazione_LeggiAjax();
    GrigliaKendoConcimazione('kendo_PianiConcimazione');

    WaitFrame.hide();
}

function stampaRegistroFertilizzazioni(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);

    //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    stampaPC_daDataItem(dataItem, regolamento_tipo);
}

function caricaAllegato(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    console.log(datiRiga.Allegati_Documenti_Cod);
}

function onDataBoundedAllegati(e) {

}

function errore() {
    alert(TraduzioneMultiResx(MenuBSResx, "ErrorDuranteCaricamentoGriglia", "Errore durante il caricamento della griglia"));
}

function controllaData(sender) {
    var data = sender.value;
    var espressione = /^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/;
    if (!espressione.test(data)) {
        //alert("Formato Sbagliato");
        sender.value = ""
    } else {
        //alert("Formato Corretto");
    }
}

function Tipo_Des_datoTipoCod(PC_Tipo) {
    switch (PC_Tipo) {
        case Enum_Metodo.Bilancio.value:
            return Enum_Metodo.Bilancio.name;
        case Enum_Metodo.Schede.value:
            return Enum_Metodo.Schede.name;
    }
}

function Tipo_Des_datoTipoCod_PUA(PC_Tipo) {
    switch (PC_Tipo) {
        case Enum_Metodo.Bilancio.value:
            return Enum_Metodo.Bilancio.name;
        case Enum_Metodo.Schede.value:
            return Enum_Metodo.Schede.name;
    }
}

function Tipo_Des_datoTipoCod_PN(PC_Tipo, Regolamento_Tipo) {
    switch (PC_Tipo) {
        case Enum_Metodo.Bilancio.value:
            if (Regolamento_Tipo == PianoNutrizionale) {
                return Enum_Metodo.Bilancio.name + " " + TraduzioneMultiResx(MenuBSResx, "BarbabietolaZucchero", "Barbabietola da Zucchero");
            } else {
                return Enum_Metodo.Bilancio.name
            }
        case Enum_Metodo.Schede.value:
            if (Regolamento_Tipo == PianoNutrizionale) {
                return Enum_Metodo.Schede.name + " " + TraduzioneMultiResx(MenuBSResx, "BarbabietolaZucchero", "Barbabietola da Zucchero");
            } else {
                return Enum_Metodo.Schede.name
            }
    }
}

function kendoRefresh(jQuerySelector) {
    $(jQuerySelector).data("kendoGrid").refresh();
}

function aggiornaStyle() {

    var datiGriglia = $("#kendo_PianiConcimazione").data("kendoGrid");

    var elemVisibiliHtml = datiGriglia.tbody.find("tr");
    var elemVisibiliDati = datiGriglia._data;
    //var elemVisibiliHtml = datiGriglia.table[0].rows; <-- QUESTO NON VA BENE DELLE PERCHè le righe non corrispondono

    if (elemVisibiliDati != null) {

        for (i = 0; i < elemVisibiliDati.length; i++) {

            if (TipologiaPagina === 1) { //1=Piano concimazione; 2 = PUA

                if (elemVisibiliDati[i].Allegati_Documenti_Cod == 0) {
                    var row = $(elemVisibiliHtml[i]);
                    var commandCell = row.find(".alleg_elem");
                    commandCell.removeClass("fa-link");
                    commandCell.addClass("fa-chain-broken");
                    commandCell.prop('title', "nessun allegato");
                }
            }

            if (elemVisibiliDati[i].nPianiDistr > 0) {
                var row = $(elemVisibiliHtml[i]);
                var commandCell = row.find(".k-grid-PianoDistribuzione");
                commandCell.addClass("btn-warning");
                if (elemVisibiliDati[i].nPianiDistr == 1) {
                    if (TipologiaPagina === 1)
                        commandCell.prop('text', elemVisibiliDati[i].nPianiDistr.toString() + " " + TraduzioneMultiResx(MenuBSResx, "PianoDistribuzioneAssociato", "Piano Distribuzione associato"));
                    else
                        commandCell.prop('text', " " + TraduzioneMultiResx(MenuBSResx, "PianoDistribuzioneAssociato", "Piano Distribuzione associato"));
                }
                else {
                    commandCell.prop('text', elemVisibiliDati[i].nPianiDistr.toString() + " " + TraduzioneMultiResx(MenuBSResx, "PianiDistribuzioneAssociati", "Piani Distribuzione associati"));
                }
            }

            if (elemVisibiliDati[i].blocco_flag > 0) {
                var row = $(elemVisibiliHtml[i]);
                var commandCelldel = row.find(".del_elem");
                commandCelldel.hide();
                var commandCelledit = row.find(".edit_elem");
                commandCelledit.hide();
            }
        }
    }
}

function FiltraImpiantiConFiltroRicercaNG() {

    var param = kendo.stringify({
        "piva": currentPiva
    });

    ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
        param,
        function (risposta) {
            apriFinestraFiltroRicercaNG(risposta.RispostaStringa)
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        });
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra Impianti",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        CreaEntitaDaChiavi(event.data.inData.chiavi)

        kWin.close();
    }
}

function CreaEntitaDaChiavi(chiavi) {

    var param = kendo.stringify({
        "chiavi": chiavi
    });

    ajaxAgronica(indirizzohttp + "/CreaEntitaDaChiavi",
        param,
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, true);
}