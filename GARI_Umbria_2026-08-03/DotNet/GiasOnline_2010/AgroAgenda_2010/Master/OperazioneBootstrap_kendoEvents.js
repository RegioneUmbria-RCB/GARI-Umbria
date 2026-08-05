
// OperazioneBootsrap_kendoEvents.js


//#Region template per colonne con combo
function Udm_Template(container, options) {

    $('<input required data-text-field="Udm_Des" data-value-field="Udm_Cod" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        dataSource: Udm_Leggi(),
        /*open: kendoDropDownAdjustWidth,
        dataBound: kendoDropDownAdjustWidth,*/
        change: function (e) {

            // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
            var dataItem = e.sender.dataItem();
            var grid = $("#kendo_CostiAccessori").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));

            model.Udm_Cod = dataItem.Udm_Cod;
            model.Udm_Des = dataItem.Udm_Des;

            //Se l'udm è Ettaro allora prendo gli ha selezionati
            if (dataItem.Udm_Cod === 1) {
                let supTrattata = SupTrattata();
                //let datiGriglia = grid.dataSource.data();

                grid.editCell(this.element.closest("tr").find("td").eq(6).first());
                //let di = grid.dataSource.getByUid(datiGriglia[i].uid);
                //kendo_imposta_valore(di, "Qta_Ril", supTrattata, null);
                model.Qta_Ril = supTrattata;
                grid.closeCell();
                
            }

            //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)

        }

    });

}

function Udm_Leggi() {

    return [
        { Udm_Cod: -1, Udm_Des: "indefinito" },
        { Udm_Cod: 1, Udm_Des: "ha" },
        { Udm_Cod: 2, Udm_Des: "ora" }
    ];

}


function Attivita_Template(container, options) {

    $('<input required data-text-field="Attivita_Des" data-value-field="Id_attivita" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataSource: Attivita_Leggi(),
        open: kendoDropDownAdjustWidth,
        dataBound: kendoDropDownAdjustWidth,
        change: function (e) {

            // Imposto la descrizione ed il codice nel modello dei dati leggendo dal'elemento selezionato
            var dataItem = e.sender.dataItem();
            var grid = $("#kendo_CostiAccessori").data("kendoGrid"),
                model = grid.dataItem(this.element.closest("tr"));

            model.Id_attivita = dataItem.Id_attivita;
            model.Attivita_Des = dataItem.Attivita_Des;


            //Sviluppare qui quello che si vuol fare al change (es.: scremare altre griglie)
            AggiornaCostoUnitario(model.Turno_Cod, dataItem.Id_attivita, model);
        }

    });

}

function Attivita_Leggi() {

    //per chiamata Standard Ajax
    if (obj_hdKendo_CostiAccessori_Attività_ComboHelper == undefined) {
        ajaxAgronicaSync(svrUrl + "/Attivita_PopolaCombo", JSON.stringify({}), true,
                            function (risposta) {
                                obj_hdKendo_CostiAccessori_Attività_ComboHelper = JSON.parse(risposta.RispostaStringa);
                            }, null);
    }
    
    //kendo_AggiustaDimensioneColonne("#kendo_CostiAccessori");
    return obj_hdKendo_CostiAccessori_Attività_ComboHelper;

}


function Turno_Template(container, options) {

    $('<input required data-text-field="Turno_Des" data-value-field="Turno_Cod" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataSource: Turno_Leggi(),
        open: kendoDropDownAdjustWidth,
        dataBound: kendoDropDownAdjustWidth,
        change: function (e) {


            //Sviluppare quello che si vuol fare al change
            //leggo la chiave ed aggiorno le diverse chiavi del model...

            var dataItem = e.sender.dataItem();
            var grid = $("#kendo_CostiAccessori").data("kendoGrid"),
                model = grid.dataItem(this.element.closest("tr"));

            model.Turno_Cod = dataItem.Turno_Cod;
            model.Turno_Des = dataItem.Turno_Des;

            //Occorre aggiornare il costo unitario.
            AggiornaCostoUnitario(dataItem.Turno_Cod, model.Id_attivita, model);
        }

    });

}

function Turno_Leggi() {

    //in questo caso l'oggetto non deve essere aggiornato, quindi basta leggerlo una volta sola.
    if (obj_hdKendo_CostiAccessori_Turno_ComboHelper == undefined) {

        ajaxAgronicaSync(svrUrl + "/Turno_PopolaCombo", JSON.stringify({}), true,
            function (risposta) {

                obj_hdKendo_CostiAccessori_Turno_ComboHelper = JSON.parse(risposta.RispostaStringa);
            }, null);

    }

    //kendo_AggiustaDimensioneColonne("#kendo_CostiAccessori");
    return obj_hdKendo_CostiAccessori_Turno_ComboHelper;

}


function AggiornaCostoUnitario(Turno_Cod, Attivita_Cod, model) {
    var DataAttivita = $(id_txt_DataOperazione).val();
            
    ajaxAgronica(svrUrl + "/cmbCostiAccessori_AggiornaCostoUnitario", JSON.stringify({ Turno_Cod: Turno_Cod, Attivita_Cod: Attivita_Cod, DataAttivita: DataAttivita }),
        function (risposta) {

            //imposto il nuovo costo unitario e ricalcolo.
            
            var NuovoCostoUnitario = risposta.RispostaStringa;
            var nNuovoCostoUnitario = 0;
            try {
                nNuovoCostoUnitario = parseFloat(NuovoCostoUnitario);
            } catch (e) {
                consol.warning(e);
            }

            var totale = 0;
            totale = nNuovoCostoUnitario * model.Qta_Ril;

            kendo_imposta_valore(model, "Costo_Unitario", NuovoCostoUnitario, "n2");

            kendo_imposta_valore(model, "Costo", totale, "n2");

        }, null);
}
        

function Risorsa_Template(container, options) {

    $('<input required data-text-field="Risorsa_Des" data-value-field="Risorsa_Cod" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        filter: "contains",
        dataSource: Risorsa_Leggi(),
        //open: kendoDropDownAdjustWidth,
        //dataBound: kendoDropDownAdjustWidth,
        select: function (e) {

            //Verifico che non ci siano dei doppioni
            var dataItem = this.dataItem(e.item.index());
            var vecchioVal = this.value();
            var nuovoVal = dataItem.Risorsa_Cod;

            var grid = $("#kendo_CostiAccessori").data("kendoGrid");
            var data = grid.dataSource.data();

            for (var i = 0; i < data.length; i++) {
                if (data[i].kendoKey !== "" && data[i].Risorsa_Cod === nuovoVal) {
                    var arr = nuovoVal.split("*");
                    //Le persone (-2) le posso inserire più di una volta SOLO se hanno turno/attività/qualifica diversa. Da affinare...
                    if (arr[0] !== "-2") {
                        e.preventDefault();
                        alert("Risorsa già inserita", "DIV_Messaggi");
                        return;
                    }
                }
            }

        },
        change: function (e) {

            //leggo la chiave ed aggiorno le diverse chiavi del model...

            var dataItem = e.sender.dataItem();
            var grid = $("#kendo_CostiAccessori").data("kendoGrid");
            var model = grid.dataItem(this.element.closest("tr"));

            model.Risorsa_Cod = dataItem.Risorsa_Cod;
            model.Risorsa_Des = dataItem.Risorsa_Des;

            //'Todo: _Tariffa_Cod andrà in chiave .... inoltre occorre tenere conto delle chiavi ID_Attività e Turno_Cod.
            // la chiave della risorsa parte con il centro_Cod
            // Udm_Cod.ToString & "-" & 
            // Id_Attivita.ToString & "-" & 
            // Turno_Cod.ToString & "-" &
            // Centro_Cod & "-" & 
            // elem_Cod.ToString & "-" & 
            // Pro_Cod.ToString & "-" &
            // Mat_Cod.ToString  


            var risorsa_Cod_splitted = dataItem.Risorsa_Cod.split("*");
            model.kendoKey = "0*0*0*" + dataItem.Risorsa_Cod;
            model.Centro_Cod = risorsa_Cod_splitted[0];
            model.Elem_Cod = risorsa_Cod_splitted[1];
            model.Pro_Cod = risorsa_Cod_splitted[2];
            model.Mat_Cod = risorsa_Cod_splitted[3];
            model.Costo_Unitario = dataItem.Costo_Unitario;
            model.Udm_Cod = dataItem.Udm_Cod;
            model.Udm_Des = dataItem.Udm_Des;
            model.Cod_Rapporto = dataItem.Cod_Rapporto;

            //Se l'udm è Ettaro allora prendo gli ha selezionati
            if (dataItem.Udm_Cod === "1") {
                let supTrattata = SupTrattata();
                supTrattata = parseFloat(supTrattata);
                kendo_imposta_valore(model, "Qta_Ril", supTrattata, null);
            }

            //Ricalcolo il costo totale sulla base della Qta_Ril
            var costoTot = parseFloat(model.Qta_Ril.toString().replace(",", ".")) * parseFloat(model.Costo_Unitario.toString().replace(",", "."));
            kendo_imposta_valore(model, "Costo", costoTot, "n2");

            grid.refresh();
            //kendo_AggiustaDimensioneColonne("#kendo_CostiAccessori");
        }

    });

}


function Risorsa_Leggi(options) {

    //var Risorse = [

    //    {
    //        Risorsa_Cod: "-1*1*0*94",
    //        Risorsa_Des: "Irroratrice per colture arboree tipo A 500 lt"
    //    }, {
    //        Risorsa_Cod: "-2*0*0*12989",
    //        Risorsa_Des: "Gianfranco Calai"
    //    }
    //];


    //' VAnni: 6/4/2017: ricarico sempre.
    //if (obj_hdKendo_CostiAccessori_ComboHelper == undefined)

    obj_hdKendo_CostiAccessori_ComboHelper = JSON.parse($(id_hdKendo_CostiAccessori_ComboHelper).val());

    return obj_hdKendo_CostiAccessori_ComboHelper;

}

//#End Region template per colonne con combo