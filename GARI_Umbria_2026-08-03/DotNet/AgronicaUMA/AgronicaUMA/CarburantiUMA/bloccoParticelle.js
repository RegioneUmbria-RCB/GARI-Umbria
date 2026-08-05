var gestioneCarbResx = [{}];

$("#btn_carica_griglia").click(
    async function () {
        if (KendoDDL("ddlAzienda").value() === '') {

            let div = document.createElement("div");
            $(div).kendoDialog({
                content: "Selezionare un'impresa",
                title: "Attenzione",
                closable: false,
                actions: [
                    {
                        text: "Continua",
                        primary: true,
                        action: function (e) {

                        },
                    }
                ]
            }).data("kendoDialog").open();

        } else {
            ConfiguraGrigliaBlocco("tab_griglia_bloccoParticelle");
        }
    });

function ConfiguraGrigliaBlocco(IDControllo) {

    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;
    
    funzioneSubmitDaUsare = { funzione: Update_Blocco, flagInsert: false, flagUpdate: true, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: LeggiBloccoParticelle,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false
    };
    var idModel = "ID";
    var campiKendoModel = null;

    campiKendoModel = {
        ID: { editable: false, type: "string", validation: { required: true } },
        piva: { editable: false, type: "string", validation: { required: true } },
        Gruppo_Colturale_UMA: { editable: false, type: "string", validation: { required: true } },
        macrouso_UMA_Des: { editable: false, type: "string", validation: { required: true } },
        Programmazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Programmazione_Des: { editable: false, type: "string", validation: { required: true } },
        PROV: { editable: false, type: "string", validation: { required: true } },
        PROVINCIA: { editable: false, type: "string", validation: { required: true } },
        COM: { editable: false, type: "string", validation: { required: true } },
        COMUNE: { editable: false, type: "string", validation: { required: true } },
        SEZIONE: { editable: false, type: "string", validation: { required: true } },
        FOGLIO: { editable: false, type: "number", validation: { required: true } },
        NUMERO: { editable: false, type: "number", validation: { required: true } },
        SUBALTERNO: { editable: false, type: "string", validation: { required: true } },
        Sup_A: { editable: false, type: "number", validation: { required: true } },
        Sup_B: { editable: false, type: "number", validation: { required: true } },
        anno: { editable: false, type: "number", validation: { required: true } },
        Bloccato: { editable: false, type: "boolean" },
        Bloccato_Des: { editable: true, type: "string" },
        Validita_Inizio: { editable: false, type: "Date", validation: { required: true }}   
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneKendoGrid = [
        { field: "Programmazione_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Programmazione_Des", "Fascicolo"), width: 140, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "PROVINCIA", title: TraduzioneMultiResx(gestioneCarbResx, "PROVINCIA", "Provincia"), width: 90, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "COMUNE", title: TraduzioneMultiResx(gestioneCarbResx, "COMUNE", "Comune"), width: 90, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "SEZIONE", title: TraduzioneMultiResx(gestioneCarbResx, "SEZIONE", "Sezione"), width: 88, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "FOGLIO", title: TraduzioneMultiResx(gestioneCarbResx, "FOGLIO", "Foglio"), width: 71, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "NUMERO", title: TraduzioneMultiResx(gestioneCarbResx, "NUMERO", "Numero"), width: 82, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "SUBALTERNO", title: TraduzioneMultiResx(gestioneCarbResx, "SUBALTERNO", "Subalterno"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Sup_A", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_A", "Superficie pendenza A (Ha)"), width: 200, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Sup_B", title: TraduzioneMultiResx(gestioneCarbResx, "Sup_B", "Superficie pendenza B (Ha)"), width: 200, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "macrouso_UMA_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Macrouso_UMA_Des", "Gruppo Colturale"), width: 200, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Bloccato_Des", template: '#= (Bloccato) ? "Bloccato" : "Attivo" #', title: TraduzioneMultiResx(gestioneCarbResx, "Bloccato_Des", "Bloccato"), width: 90, headerAttributes: { style: styleElen }, editor: Bloccato_DropDownEditor },
    ];


    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

    var mostraRigheCancellate = true;
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

function Citta_Load() {
    WaitFrame.show();

    $('#citta').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiCitta } },
        dataTextField: "citta",
        dataValueField: "proCom",
        optionLabel: { "citta": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTE", "proCom": -1 },
        autoWidth: true,
        dataBound: ddlCitta_OnDataBound
    });

}

function ddlCitta_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    } else if (KendoDDL("prov").value() == -1){
        this.select(0);
    }
}

function Prov_Load() {

    $('#prov').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiProv } },
        dataTextField: "PROVINCIA",
        dataValueField: "PROV",
        //optionLabel: { "PROVINCIA": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + "...", "PROV": "" },
        autoWidth: true,
        dataBound: ddlProv_OnDataBound
    });

}

function ddlProv_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlProv.onchange(); //forzo l'evento di onchange
    }
}

function ddlProv_Change(e) {
    Citta_Load();
    KendoDDL("citta").value("-1");
    //$("#citta").show();
}

async function ddlAzienda_Load() {

    //var ddlAzienda = await RiempiDdlAzienda();
    $('#ddlAzienda').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: RiempiDdlAzienda
            }
        },
        dataTextField: "rag_soc",
        dataValueField: "piva",
        mapValueTo: "dataItem",
        dataBound: ddlAzienda_OnDataBound,
        virtual: {
            itemHeight: 26,
            valueMapper: function (options) {
                var val = options.value;
                if (val != "" && val != "-1") {
                    var a = KendoDDL("ddlAzienda").dataSource._pristineData.find((el) =>
                        el.piva == val
                    );
                    options.success(KendoDDL("ddlAzienda").dataSource._pristineData.indexOf(a));
                } else {
                    options.success("");
                }
            }
        },
    });

}

function RiempiDdlAzienda(options) {
    //return new Promise(function (resolve, reject) {
    var pathCaricaCmb = ""
    var parametri = ""
    if (true) {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "area": "UMA" });
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_Area";
    }
    else {
        parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti});
        pathCaricaCmb = "Anagrafica/Imprese.asmx/Carica_Cmb_Imprese";
    }
        ajaxAgronicaSync(pathCoreWS + pathCaricaCmb,
            parametri,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                objVuoto = { "piva": "", "rag_soc": "..." };
                if (QS_Piva != "") {
                    let tmp = risp.filter(x => x.piva == QS_Piva)[0];
                    if (tmp != undefined) {
                        risp.splice(risp.indexOf(tmp), 1);
                        risp.unshift(tmp);
                    }
                }
                risp.unshift(objVuoto);
                //options.success(risp.slice(0,100));
                options.success(risp);
                //resolve(risp);
            }, null);

    //});
}

function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    }
}

async function ddlAzienda_Change() {

    var piva = KendoDDL("ddlAzienda").value();

    if (piva == "" && QS_Piva != "") {
        piva = QS_Piva;
    }

    var pivaReale = await CercaPivaRealeB(piva);

    if (piva !== "" && piva !== "-1" && KendoDDL("ddlAzienda").text() !== "...") {

        var dettagliAzienda = CercaDettagliAzienda(piva);
        var esisteRichiesta = dettagliAzienda[0].cod > 0;
        var descrAzienda = $('#anno');
        descrAzienda[0].defaultValue = "2021";
        var descrAzienda = $('#nDichiarazione');
        if (descrAzienda[0].defaultValue == "")
            descrAzienda[0].defaultValue = esisteRichiesta ? dettagliAzienda[0].nDichiarazione : "";
        var descrAzienda = $('#piva');

        if (pivaReale != "") {
            descrAzienda[0].defaultValue = pivaReale;
        }
        else if (piva > 0) {
            descrAzienda[0].defaultValue = piva;
        }
        else {
            descrAzienda[0].defaultValue = " ";
        }
        /*var descrAzienda = $('#indirizzo');
        if (dettagliAzienda[0].indDes != "")
            descrAzienda[0].defaultValue = dettagliAzienda[0].indDes;
        else
            descrAzienda[0].defaultValue = " ";*/
        var descrAzienda = KendoDDL("prov");
        if (dettagliAzienda[0].pro_cod != "")
            descrAzienda.value(dettagliAzienda[0].pro_cod);
        else
            descrAzienda[0].defaultValue = " ";
        ddlProv_Change();
        var descrAzienda = KendoDDL("citta");
        if (dettagliAzienda[0].comDes != "" && KendoDDL("prov").value() == "-1")
            descrAzienda.value(dettagliAzienda[0].comDes);
        else
            descrAzienda.value(dettagliAzienda[0].comDesIstat);
        var descrAzienda = $('#CUAA');
        if (dettagliAzienda[0].CUAA != "")
            descrAzienda[0].defaultValue = dettagliAzienda[0].CUAA;
        else
            descrAzienda[0].defaultValue = " ";

    } else {
        $('#nDichiarazione')[0].defaultValue = " ";
        //$('#indirizzo')[0].defaultValue = " ";
        $('#piva')[0].defaultValue = " ";
        $('#CUAA')[0].defaultValue = " ";
        KendoDDL("citta").value("-1");
        KendoDDL("prov").value("-1");
    }
}

function Bloccato_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Bloccato_Des", "Bloccato", [{ Bloccato: false, Bloccato_Des: "Attivo" }, { Bloccato: true, Bloccato_Des: "Bloccato" }], changeBloccato);
}

function changeBloccato(e) {
    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.Bloccato = dataItem.Bloccato;
    model.Bloccato_Des = dataItem.Bloccato_Des;
    model.anno = parseInt($('#anno')[0].value);
    model.dirty = true;
}

async function Update_Blocco(e) {

    WaitFrame.show();

    if (e.data.updated.length > 0) {
        await ws_Aggiorna_Blocco(e.data.updated);
        ConfiguraGrigliaBlocco("tab_griglia_bloccoParticelle");
    }

    ConfiguraGrigliaBlocco("tab_griglia_bloccoParticelle");

    WaitFrame.hide();

}

function getYear() {
    if ($.cookie("UMA.Anno") != null && !isNaN($.cookie("UMA.Anno"))) {
        return parseInt($.cookie("UMA.Anno"));
    } else {
        var date = new Date();
        let anno = new Date().getFullYear();
        date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
        $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
        return anno;
    }
}

function setYear(anno) {
    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
}