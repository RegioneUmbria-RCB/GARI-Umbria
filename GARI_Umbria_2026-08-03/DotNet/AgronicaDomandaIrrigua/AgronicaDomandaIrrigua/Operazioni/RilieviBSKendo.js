
function CategorieVisiteComboInizializza() {

    $("#comboCategoriaLiv1").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "nome",
        dataValueField: "val",
        dataSource: { transport: { read: ComboCategorieVisiteLiv1Popola }, group: { field: "gruppo" } }//,
        //change: function (e) {
            //CaricaValoriComboMisuraXAvversitaInputData();
        //}
    });

    $("#comboCategoriaLiv2").kendoMultiSelect({
        filter: "contains",
        autoBind: true,
        dataTextField: "nome",
        dataValueField: "val",
        dataSource: { transport: { read: ComboCategorieVisiteLiv2Popola }, group: { field: "gruppo" } },
        change: function (e) {

            var valori = e.sender.value();

            if (jQuery.inArray(parseInt(LAVCOD_FASIFENOLOGICHE), valori) >= 0)
                $(panelBarFasiFenologiche_clientID).show();
            else
                $(panelBarFasiFenologiche_clientID).hide();

            if (jQuery.inArray(parseInt(LAVCOD_RILIEVOAVVERSITAINCAMPO), valori) >= 0)
                $(panelBarAvversitaInCampo_clientID).show();
            else
                $(panelBarAvversitaInCampo_clientID).hide();

            if (jQuery.inArray(parseInt(LAVCOD_RILIEVOERBEINFESTANTI), valori) >= 0)
                $(panelBarErbeInfestanti_clientID).show();
            else
                $(panelBarErbeInfestanti_clientID).hide();

            if (jQuery.inArray(parseInt(LAVCOD_RILIEVOINDICIMATURITA), valori) >= 0)
                $(panelBarIndiciMaturita_clientID).show();
            else
                $(panelBarIndiciMaturita_clientID).hide();

            if (jQuery.inArray(parseInt(LAVCOD_RILIEVOINDICIRESERACCOLTA), valori) >= 0)
                $(panelBarIndiciReseRaccolta_clientID).show();
            else
                $(panelBarIndiciReseRaccolta_clientID).hide();
            

            //"5007|111" = visite + codice visita personalizzata
            $(panelBarVisitePersonalizzate_clientID).hide();
            for (i = 0; i < valori.length; i++) {
                if (valori[i].toString().startsWith(LAVCOD_ALTRELAVORAZIONI + "|")) {
                    $(panelBarVisitePersonalizzate_clientID).show();
                    caricaComboVisitePersonalizzate();
                    break;
                }
            }

        }
    });
}

function caricaComboVisitePersonalizzate() {

    var ddl = $("#comboCategoriaLiv2").data("kendoMultiSelect");
    var selezione = ddl.dataItems();
    var ds = JSON.parse("[]");

    for (i = 0; i < selezione.length; i++) {
        if (selezione[i].val.toString().startsWith(LAVCOD_ALTRELAVORAZIONI + "|")) {
            var nome = selezione[i].nome;
            //var val = selezione[i].val.toString().replace( LAVCOD_ALTRELAVORAZIONI + "|", "");
            var val = selezione[i].val;
            ds.push({ "val": val, "nome": nome });
        }
    }

    $("#comboVisitePers").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "nome",
        dataValueField: "val",
        dataSource: ds
    });
}

function selezionaCategorieVisite() {
    var multiselect = $("#comboCategoriaLiv2").data("kendoMultiSelect");
    var preselezione = JSON.parse($(hdSelezioneCategorieVisite_clientID).val());

    multiselect.value(preselezione);
    multiselect.trigger("change");
}

/* Rilievi BS Kendo */

function RilievoAvversitaInCampoComboInizializza() {

    var templateNoSoglia = "<div><span class='k-state-default'>#: des # </span></div>";
    var templateSoglia = "<div style='background-color:yellow'><span class='k-state-default'  >* #: des #</span></div>";

    $("#comboRilievoAvversitaInCampo").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",        
        dataSource: { transport: { read: ComboRilievoAvversitaInCampoPopola } },
        template: "#if(soglia === '1'){#" + templateSoglia + "#}else{#" + templateNoSoglia + "#}#",
        valueTemplate: '<span>#: des #</span>',
        change: function (e) {
            CaricaValoriComboMisuraXAvversitaInputData();
        }
    });
}

function RilievoErbeInfestantiComboInizializza() {

    $("#comboRilievoErbeInfestanti").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",
        dataSource: { transport: { read: ComboRilievoErbeInfestantiPopola } }//,
        //change: function (e) {
        //    CaricaValoriComboMisuraXAvversitaInputData();
        //}
    });
}

function RilievoDanniRaccoltaComboInizializza() {

    $("#comboRilievoDanniRaccolta").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",
        dataSource: { transport: { read: ComboRilievoDanniRaccoltaPopola } }//,
        //change: function (e) {
        //    CaricaValoriComboMisuraXAvversitaInputData();
        //}
    });
}

function RilievoIndiciMaturitaComboInizializza() {

    $("#comboRilievoIndiciMaturita").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",
        dataSource: { transport: { read: ComboRilievoIndiciMaturitaPopola } },
        change: function (e) {
            CaricaValoriComboMisuraXIndiciMaturitaInputData();
        }
    });
}


function RilievoFasiFenologicheComboInizializza() {

    var templateSenzaFioritura = "<div><span class='k-state-default'>#: des  # ( BBCH #: stadio  # ) </span></div>";
    var templateConFioritura = "<div style='background-color:pink'><span class='k-state-default'  >#: des  # ( BBCH #: stadio  # ) </span></div>";

    $("#comboRilievoFasiFenologiche").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",
        dataSource: { transport: { read: ComboRilievoFasiFenologichePopola } },
        template: "#if(fioritura === '1'){#" + templateConFioritura + "#}else{#" + templateSenzaFioritura + "#}#",
        valueTemplate: '<span>#: des  # ( BBCH #: stadio  # )</span>'
        //change: function (e) {
        //    CaricaValoriComboMisuraXAvversitaInputData();
        //}
    });
}

function RilievoIndiciReseRaccoltaComboInizializza() {

    $("#comboRilievoIndiciReseRaccolta").kendoDropDownList({
        filter: "contains",
        autoBind: false,
        dataTextField: "des",
        dataValueField: "cod",
        dataSource: { transport: { read: ComboRilievoIndiciReseRaccoltaPopola } }//,
        //change: function (e) {
        //    CaricaValoriComboMisuraXAvversitaInputData();
        //}
    });
}


function letturaTabella_Kendo() {
    
    var param = "{ param: '' }";

    ajaxAgronica(indirizzohttp + "/CaricaTabella_Kendo",
        param,
        function (risposta) {
            if (risposta.RispostaOK) {
                $(hdRilievi_clientID).val(risposta.RispostaStringa);
                popolaGriglia("divRilievi");
                AggiornaDopo_SupTrattata();
            }
            else {
                
                alert(risposta.Errore);
            }
        }, null);
}


//-----------------------------------------------------------------------------------------------------------------------------------
//KENDO
//-----------------------------------------------------------------------------------------------------------------------------------

function popolaGriglia(IDControllo) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
    var idModel = "Codice";
    var campiKendoModel = kReadValorizzazione_mod();
    var colonneKendoGrid = kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        filterable: true,
        pageable: false,
        scrollable: false,
        editable: true,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-danger' style='display:block; width:70px; border:0px;' onclick=cancellaElemento(this.closest('tr'),this.closest('.k-grid'))> "
                        + Traduzione(rilieviBSResxLocal, "Cancella", "Cancella") + "</div>"
                }, title: Traduzione(rilieviBSResxLocal, "Azioni", "Azioni"), width: "97px"//, widthfisso: true //width: "135px" - no style in span
            }
        ]

    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kendo_Rilievi_onDataBoundedRighe};
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

function kReadValorizzazione_rows(options) {

    var data = $(hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazione_col() {

    var data = $(hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    kendo_Colonne_estendi(jSonParsed_Kendo, "Dato", Traduzione(rilieviBSResxLocal, "QuantitàRilevataAbbr", "Qtà Rilevata"), 7, "Dato_Testuale", RilieviEditor_Template);

    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazione_mod() {

    var data = $(hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function kendo_Rilievi_onDataBoundedRighe(e) {
    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 1);
}

function mostraColonnaUnicaSeInMobile(eventArgs, nColonneDaSaltare) {

    //Controllo se sono su mobile
    if ($(window).width() <= 767) {
        var grid = eventArgs.sender;

        for (var i = nColonneDaSaltare; i < grid.columns.length; i++) {
            grid.hideColumn(i);
        }
        grid.showColumn('Descrizione_Unica');
        grid.showColumn('Dato');//dato combo
    }
}

function AggiornaRiga() {
}

function cancellaElemento(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    datiGriglia.removeRow(tr_elem);
}

function RilieviEditor_Template(container, options) {

    var av_cod = options.model.av_cod;
    var udm_cod = options.model.udm_cod;
    var veg_cod = options.model.Veg_Cod;
    var ind_mat_cod = options.model.ind_mat_cod;
    var lav_cod = options.model.lav_cod;

    //Controllo che i campi selezionati abbiano valore
    if (!$.isNumeric(veg_cod) || !$.isNumeric(lav_cod)) {
        return;
    }

    var funzione = "";
    var parametri = "";

    switch (lav_cod) {
        case LAVCOD_RILIEVOINDICIMATURITA:
            funzione = "/CaricaComboMisuraXIndiciMaturitaInputData";
            parametri = kendo.stringify({ 'Ind_Mat_Cod': ind_mat_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
        case LAVCOD_RILIEVOINDICIRESERACCOLTA:
            //funzione = "/CaricaComboMisuraXIndiciMaturitaInputData";
            //parametri = kendo.stringify({ 'Ind_Mat_Cod': ind_mat_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
        case LAVCOD_RILIEVOAVVERSITAINCAMPO:
            funzione = "/CaricaComboMisuraXAvversitaInputData";
            parametri = kendo.stringify({ 'MxAV_Cod': '0', 'av_cod': av_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
        case LAVCOD_FASIFENOLOGICHE:
            //funzione = "/CaricaComboMisuraXAvversitaInputData";
            //parametri = kendo.stringify({ 'MxAV_Cod': '0', 'av_cod': av_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
        case LAVCOD_RILIEVOERBEINFESTANTI:
            //funzione = "/CaricaComboMisuraXAvversitaInputData";
            //parametri = kendo.stringify({ 'MxAV_Cod': '0', 'av_cod': av_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
        case LAVCOD_RILIEVODANNIALLARACCOLTA:
            //funzione = "/CaricaComboMisuraXAvversitaInputData";
            //parametri = kendo.stringify({ 'MxAV_Cod': '0', 'av_cod': av_cod, 'udm_cod': udm_cod, 'veg_cod': veg_cod });
            break;
    }

    if (funzione !== "") {

        ajaxAgronicaSync(indirizzohttp + funzione, parametri, true,
        function (risposta) {

            var x = risposta.RispostaStringa;
            x = x.replace(new RegExp(escapeRegExp("anag_des"), 'g'), "Dato_Testuale");
            x = x.replace(new RegExp(escapeRegExp("anag_valore"), 'g'), "Dato");
            var ds = JSON.parse(x);

            if (ds.length > 0) {

                //Creo la combo
                $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                    .appendTo(container)
                    .kendoDropDownList({
                        dataSource: ds,
                        autoBind: true,
                        dataTextField: "Dato_Testuale",
                        dataValueField: "Dato",
                        change: function (e) {
                            // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                            var dataItem = e.sender.dataItem();
                            var grid = $("#divRilievi").data("kendoGrid");
                            var model = grid.dataItem(this.element.closest("tr"));

                            model.Dato = dataItem.Dato;
                            model.Dato_Testuale = dataItem.Dato_Testuale;

                        }
                    });

            } else {

                if (lav_cod === LAVCOD_FASIFENOLOGICHE) {

                    $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                        .appendTo(container)
                        .kendoDatePicker({
                            culture: "it-IT",
                            change: function (e) {
                                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                                var grid = $("#divRilievi").data("kendoGrid");
                                var model = grid.dataItem(this.element.closest("tr"));

                                model.Dato = kendo.toString(e.sender.value(), 'dd/MM/yyyy');
                                model.Dato_Testuale = kendo.toString(e.sender.value(), 'dd/MM/yyyy');

                            }
                        });

                } else if (lav_cod === LAVCOD_VISITA) {

                    //Mostro la textbox (maccio una masked senza mask :-) )
                    $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                        .appendTo(container)
                        .kendoMaskedTextBox({
                            change: function (e) {
                                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                                var grid = $("#divRilievi").data("kendoGrid");
                                var model = grid.dataItem(this.element.closest("tr"));

                                model.Dato = e.sender.value();
                                model.Dato_Testuale = e.sender.value();

                            }
                        });

                }

                else {

                    //Mostro la textbox
                    $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                        .appendTo(container)
                        .kendoNumericTextBox({
                            change: function (e) {
                                // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                                var grid = $("#divRilievi").data("kendoGrid");
                                var model = grid.dataItem(this.element.closest("tr"));

                                model.Dato = e.sender.value();
                                model.Dato_Testuale = e.sender.value();

                            }
                        });

                }

            }

        }, null);

    } else {

        if (lav_cod === LAVCOD_FASIFENOLOGICHE) {

            $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                .appendTo(container)
                .kendoDatePicker({
                    culture: "it-IT",
                    change: function (e) {
                        // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                        var grid = $("#divRilievi").data("kendoGrid");
                        var model = grid.dataItem(this.element.closest("tr"));

                        model.Dato = kendo.toString(e.sender.value(),'dd/MM/yyyy');
                        model.Dato_Testuale = kendo.toString(e.sender.value(), 'dd/MM/yyyy');

                    }
                });

        } else if (lav_cod === LAVCOD_VISITA)  {

            //Mostro la textbox (maccio una masked senza mask :-) )
            $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                .appendTo(container)
                .kendoMaskedTextBox({
                    change: function (e) {
                        // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                        var grid = $("#divRilievi").data("kendoGrid");
                        var model = grid.dataItem(this.element.closest("tr"));

                        model.Dato = e.sender.value();
                        model.Dato_Testuale = e.sender.value();

                    }
                });

        }

        else {

            //Mostro la textbox
            $('<input required data-text-field="Dato_Testuale" data-value-field="Dato" data-bind="value:' + options.field + '" style="max-width:150px"/>')
                .appendTo(container)
                .kendoNumericTextBox({
                    change: function (e) {
                        // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
                        var grid = $("#divRilievi").data("kendoGrid");
                        var model = grid.dataItem(this.element.closest("tr"));

                        model.Dato = e.sender.value();
                        model.Dato_Testuale = e.sender.value();

                    }
                });

        }

    }


}

function MacchinaInserita() { }

function SupTrattata() { }