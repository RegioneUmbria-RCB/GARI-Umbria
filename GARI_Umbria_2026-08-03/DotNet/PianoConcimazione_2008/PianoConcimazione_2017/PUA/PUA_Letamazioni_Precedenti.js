function popolaGrigliaLetamazioniPrecedenti(IDControllo) {

    $("#" + IDControllo).html("");

    var funzioniCRUD = {
        funzioneRead: LetP_kReadValorizzazione_rows,
        funzioneInsert: LetP_kWriteValorizzazione_rows,
        funzioneUpdate: LetP_kWriteValorizzazione_rows,
        funzioneDelete: LetP_kWriteValorizzazione_rows,
        gestisciSalvataggioFinaleAParte: true
    };

    var idModel = "id";
    var campiKendoModel = LetP_kReadValorizzazione_mod();
    var colonneKendoGrid = LetP_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        aggregate: [{ field: "n_residuo", aggregate: "sum" }]
    };

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        btnEliminaTuttiFiltri: false,
        //toolbarCommands: ["templateBtn_DisponibiltaConferimentiEsterni"],
        editable: {
            mode: "popup",
            window: {
                title: "Modifica Dati Effluenti"
            }
        },
        cancel: function () {
            $("#" + IDControllo).data('kendoGrid').refresh();

            let Eff_Cod = Request_QueryString("Eff_Cod");
            if (Eff_Cod !== null) {
                //PianoColturaleRientraIframe(undefined, false);
            }

        },
        colonneCustomKendoGrid: [
            {
                command: [{
                    name: "edit",
                    text: {
                        edit: "",
                        update: "Conferma Dati",
                        cancel: "Annulla"
                    },
                    iconClass: "fa fa-pencil-square-o fa-2x",
                    className: "btn2icon"
                },
                {
                    name: "destroy",
                    text: "",
                    className: "k-custom-delete btn2icon",
                    iconClass: "fa fa-trash-o fa-2x"
                }
                ],
                title: "Operazioni",
                width: "90px"
            }
        ]
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: LetP_onEdit,
        funzioneDaChiamareDopoDataBound: LetP_dopoDataBound
        //funzioneDaChiamarePrimaDelSave: LetP_primaDelSave
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

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

function LetP_kReadValorizzazione_rows(options) {

    var data = $('#' + id_HD_LetamazioniPrecedenti).val();
    var jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);

}

function LetP_kWriteValorizzazione_rows(e) {
    //if (e.data.models[0].perc_zootecnico < 0 || e.data.models[0].perc_zootecnico > 100) {
    //    kendo.alert("% Zootecnico deve essere compreso tra 0 e 100");
    //    return false;
    //}

    e.success(e.data.models);
}

function LetP_kReadValorizzazione_mod() {

    var data = $("#" + id_HD_LetamazioniPrecedenti).val();
    var jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;

}

function LetP_kReadValorizzazione_col() {

    var data = $("#" + id_HD_LetamazioniPrecedenti).val();
    var jSonParsed_Kendo = JSON.parse(data);

    kendo_Colonne_estendi(jSonParsed_Kendo, "eff_cod", "Effluente", 0, "eff_des", eff_Template);
    //kendo_Colonne_estendi(jSonParsed_Kendo, "qta", "Qta", 4, "qta", Qta_NumericEditor);
    //kendo_Colonne_estendi(jSonParsed_Kendo, "n_titolo", "Titolo [kg/Unita Misura]", 5, "n_titolo", NTitolo_NumericEditor);
    kendo_Colonne_estendi(jSonParsed_Kendo, "id_fre", "Frequenza", 4, "frequenza_des", fre_Template);
    //kendo_Colonne_estendi(jSonParsed_Kendo, "n_riduzione", "Riduzione [%]", 7, "n_riduzione", NRiduz_NumericEditor);
    //kendo_Colonne_estendi(jSonParsed_Kendo, "n_residuo", "N Residuo [kg/Unita Misura]", 8, "n_residuo", NResiduo_NumericEditor);
    
    return jSonParsed_Kendo.kendo_columns;
}

function LetP_onEdit(e) {

    //aggiornaControlliMascheraPopup(e.model);

    let model = $("#kendo_LetamazioniPrecedenti").data("kendoGrid").editable.options.model;

    let qta = $("input[name='qta']").data("kendoNumericTextBox");
    let titolo = $("input[name='n_titolo']").data("kendoNumericTextBox");
    let riduz = $("input[name='n_riduzione']").data("kendoNumericTextBox");


    let udm_cod = e.model.udm_cod;

    qta.bind("change", function (e) {
        let res = roundNumber(this.value() * titolo.value() * riduz.value(), 2);
        if (udm_cod === 19) {
            res = res * 10;
        }
        model.set("n_residuo", res);
    });
    titolo.bind("change", function (e) {
        let res = roundNumber(this.value() * qta.value() * riduz.value(), 2);
        if (udm_cod === 19) {
            res = res * 10;
        }
        model.set("n_residuo", res);
    });
    riduz.bind("change", function (e) {
        let res = roundNumber(this.value() * titolo.value() * qta.value(), 2);
        if (udm_cod === 19) {
            res = res * 10;
        }
        model.set("n_residuo", res);
    });


    
    $("input[name='udm_sim']").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);


    KendoNumTB("n_riduzione").enable(false);
    KendoNumTB("n_residuo").enable(false);    
}

function LetP_dopoDataBound(e) {
    Calcola();
}

//function LetP_primaDelSave(e) {
//    //Controllo che non esista un altro elemento in griglia con lo stesso Effluente
//    let att_eff_cod = e.model.eff_cod;
//    let att_uid = e.model.uid;

//    let data = e.sender.dataSource.data();

//    for (let i = 0; i < data.length; i++) {
//        if (data[i].eff_cod === att_eff_cod && data[i].uid !== att_uid) {
//            e.preventDefault();
//            kendo.alert("Errore: l'effluente selezionato è già stato impostato");
//            break;
//        }
//    }

//}

var eff_di_un_tipo_eff;
var fre_di_un_eff_cod;

function eff_Template(container, options) {

    RiempiDdlEff();

    $('<input id ="cmbeff" data-text-field="eff_des" data-value-field="eff_cod" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            dataTextField: "eff_des",
            dataValueField: "eff_cod",
            dataSource: eff_di_un_tipo_eff,
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                var dataItem = e.sender.dataItem();

                model = $("#kendo_LetamazioniPrecedenti").data("kendoGrid").editable.options.model;

                model.set("eff_cod", dataItem.eff_cod);
                model.set("eff_des", dataItem.eff_des);

                model.set("udm_cod", dataItem.udm_cod);
                model.set("udm_sim", dataItem.udm_sim);

                model.set("n_titolo", dataItem.n);
                $("input[name='n_titolo']").data("kendoNumericTextBox").trigger("change");

                RiempiDdlFre(dataItem.eff_cod);

                var ddl = $("#cmbfre").data("kendoDropDownList");
                ddl.dataSource.data(fre_di_un_eff_cod);
                if (fre_di_un_eff_cod.length > 0) {
                    ddl.select(0);
                    ddl.trigger("change");
                }
            }

        });

}

function RiempiDdlEff() {
    var params = {
        regolamento_cod: parseInt($(cIdRegCod).val())
    };
    ajaxAgronicaSync("PUA_Letamazioni_Precedenti.aspx/LeggiEffluenti",
        JSON.stringify(params), false,
        function (risposta) {
            //var p = JSON.parse(risposta.RispostaStringa);
            //options.success(p);
            eff_di_un_tipo_eff = JSON.parse(risposta.RispostaStringa);
        }, null);

}

function fre_Template(container, options) {

    if (options.model.eff_cod !== 0) {
        RiempiDdlFre(options.model.eff_cod);
    }
    else {
        RiempiDdlFre(1);
    }

    $('<input id ="cmbfre" data-text-field="frequenza_des" data-value-field="id_fre" data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            dataTextField: "frequenza_des",
            dataValueField: "id_fre",
            dataSource: fre_di_un_eff_cod,
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth,
            change: function (e) {

                var dataItem = e.sender.dataItem();

                model = $("#kendo_LetamazioniPrecedenti").data("kendoGrid").editable.options.model;

                model.set("id_fre", dataItem.id_fre);
                model.set("frequenza_des", dataItem.frequenza_des);

                model.set("n_riduzione", dataItem.rid);
                $("input[name='n_riduzione']").data("kendoNumericTextBox").trigger("change");
                
                let qta = kendo.parseFloat(model.qta);
                let titolo = kendo.parseFloat(model.n_titolo);
                let riduz = kendo.parseFloat(model.n_riduzione);
                let res = roundNumber(qta * titolo * riduz, 2);

                model.set("n_residuo", res);
                $("input[name='n_residuo']").data("kendoNumericTextBox").trigger("change");

                

            }

        });


}

function RiempiDdlFre(eff_cod) {
    var params = {
        regolamento_cod: parseInt($(cIdRegCod).val()),
        eff_cod: eff_cod
    };
    ajaxAgronicaSync("PUA_Letamazioni_Precedenti.aspx/LeggiEffluentiXFrequenza",
        JSON.stringify(params), false,
        function (risposta) {
            fre_di_un_eff_cod = JSON.parse(risposta.RispostaStringa);
        }, null);

}


//function Qta_NumericEditor(container, options) {

//    $('<input id="txtQta" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
//        .appendTo(container)
//        .kendoNumericTextBox({
//            decimals: 2,
//            format: options.format,
//            spinners: false
//        }).off("keydown");

//    var myInput = container.find("#txtQta");
//    myInput.bind("change", changeQta);
//}


//function changeQta(e) {
//    var dataItem = e.currentTarget;

//    if (dataItem !== undefined && dataItem !== null) {
//        var model = $("#kendo_LetamazioniPrecedenti").data("kendoGrid").editable.options.model;

//        if (dataItem.value === undefined || dataItem.value === null || dataItem.value === "") {
//            dataItem.value = 0;
//        }

//        model.set("qta", kendo.parseFloat(dataItem.value));
        
//        let qta = kendo.parseFloat(dataItem.value);
//        let titolo = kendo.parseFloat(model.n_titolo);
//        let riduz = kendo.parseFloat(model.n_riduzione);
//        let res = roundNumber(qta * titolo * riduz, 2);

//        model.set("n_residuo", res);
        
//    }
//}

//function NTitolo_NumericEditor(container, options) {

//    $('<input id="txtNTitolo" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
//        .appendTo(container)
//        .kendoNumericTextBox({
//            decimals: 2,
//            format: options.format,
//            spinners: true
//        }).off("keydown");

//    var myInput = container.find("#txtNTitolo");
//    myInput.bind("change", changeNTitolo);
//}


//function changeNTitolo(e) {
//    var dataItem = e.currentTarget;

//    if (dataItem !== undefined && dataItem !== null) {
//        var model = $("#kendo_LetamazioniPrecedenti").data("kendoGrid").editable.options.model;
//        if (dataItem.value === undefined || dataItem.value === null || dataItem.value === "") {
//            dataItem.value = 0;
//        }
        
//        model.set("n_titolo", kendo.parseFloat(dataItem.value));

//        let qta = kendo.parseFloat(model.qta);
//        let titolo = kendo.parseFloat(dataItem.value);
//        let riduz = kendo.parseFloat(model.n_riduzione);

//        let res = roundNumber(qta * titolo * riduz, 2);
//        model.set("n_residuo", res);

//    }
//}


//function NRiduz_NumericEditor(container, options) {

//    $('<input id="txtNRiduz" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
//        .appendTo(container)
//        .kendoNumericTextBox({
//            decimals: 2,
//            format: options.format,
//            spinners: false
//        }).off("keydown");

//}

//function NResiduo_NumericEditor(container, options) {

//    $('<input id="txtNResiduo" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
//        .appendTo(container)
//        .kendoNumericTextBox({
//            decimals: 2,
//            format: options.format,
//            spinners: false
//        }).off("keydown");

//}

function Calcola() {

    var grid = $("#kendo_LetamazioniPrecedenti").data("kendoGrid");
    var data = grid.dataSource.data();
      
    var TotN = 0;

    for (var i = 0; i < data.length; i++) {
        TotN += data[i].n_residuo;
    }

    $('#Txt_Azoto').val(kendo.format('{0:n2}', roundNumber(TotN, 4)));

}

function Salva() {

    var grid = $("#kendo_LetamazioniPrecedenti").data("kendoGrid");
    var data = grid.dataSource.data();

    var regolamento_cod = parseInt($(cIdRegCod).val());
    var pua_cod = parseInt($(cIdPuaCod).val());
    var id = parseInt($(cIdAnagrafeVincoli).val());
    var piva = $(cIdPiva).val();
    var sa_cod = parseInt($(cIdSaCod).val());
    var appezza = parseInt($(cIdAppezza).val());
    var id_reg = parseInt($(cIdIdReg).val());
    var progetto_cod = parseInt($(cIdProgettoCod).val());

    var params = {
        regolamento_cod: regolamento_cod,
        pua_cod: pua_cod,
        id: id,
        piva: piva,
        sa_cod: sa_cod,
        appezza: appezza,
        id_reg: id_reg,
        progetto_cod: progetto_cod,
        dati: JSON.stringify(data)
    };

    let tuttook = false;
    ajaxAgronicaSync("PUA_Letamazioni_Precedenti.aspx/Salva",
        JSON.stringify(params), false,
        function (risposta) {
            let NTot = risposta.ParametroDue_stringa;
            parent.N_fertilizzazioniPrecedenti = kendo.parseFloat(NTot);
            tuttook = true;
        }, function (risposta) {
            alert(risposta.Errore);
        });

    if (tuttook === true) {
     parent.chiudiLetamazioni();
    }

}






function popolaGrigliaLetamazioniPrecedentiQdC(IDControllo) {

    $("#" + IDControllo).html("");

    var funzioniCRUD = {
        funzioneRead: LetPQdC_kReadValorizzazione_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null
    };

    var idModel = "id";
    var campiKendoModel = LetPQdC_kReadValorizzazione_mod();
    var colonneKendoGrid = LetPQdC_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        aggregate: [{ field: "n_residuo", aggregate: "sum" }]
    };

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        btnEliminaTuttiFiltri: false,
        editable: false
    };


    var funzioniPrimaDopoEventi = {};

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

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


function LetPQdC_kReadValorizzazione_rows(options) {
    var data = $('#' + id_HD_LetamazioniPrecedentiQdC).val();
    var jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function LetPQdC_kReadValorizzazione_mod() {
    var data = $("#" + id_HD_LetamazioniPrecedentiQdC).val();
    var jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function LetPQdC_kReadValorizzazione_col() {
    var data = $("#" + id_HD_LetamazioniPrecedentiQdC).val();
    var jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;
}