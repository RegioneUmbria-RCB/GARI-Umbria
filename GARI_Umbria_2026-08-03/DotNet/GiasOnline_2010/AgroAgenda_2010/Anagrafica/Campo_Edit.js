function creaAppezzamento() {
    kendo.confirm(TraduzioneMultiResx(
        campoEditResx,
        "ConfermaCreazioneAppezzamentoSalvandoDatiCampo",
        "Per procedere alla creazione dell'appezzamento verranno salvati i dati relativi al campo, continuare?"
    )).then(function () {
        $('#tipo_salva').val(2);
        ValidaxSubmit();
    }, function () {

    });
}

//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide, elem_hide_all) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');


    $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');
    //Calcolo dove mettere il bottone x espandere riga
    //            if ($('#' + tabella + ' table').width() <= 700) {
    //                if ($('#' + tabella + ' table').width() <= 480)
    //                    $('#' + tabella + ' table thead tr').find('th:nth-child(2)').attr('data-toggle', 'true');
    //                else
    //                    $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');
    //            }

    jQuery.each(elem_hide, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
    });

    jQuery.each(elem_hide_all, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'all');
    });


    $('#' + tabella + ' table').footable({
        breakpoints: {
            phone: 480,
            tablet: 700
        }
    });


}

var watableGestCat

function AggiornaTabGestCat(d) {

    AgroWA_Table_sistemaDati(d);

    $('#tabGestCat').html('');
    watableGestCat = $("#tabGestCat").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: true,
        preFill: false,
        tableCreated: function (data) {
            //applicaFooTable('tabGestCat'); 
            if ($('#Opt_Con_Catasto').is(':checked'))
                nascondiColonnaTabella("#tabGestCat", 0);
        },
        checkboxes: true,
        pageChanged: function (data) {
            //coloraMovimenti();
        },
        rowClicked: function (data) {
            var chk_rows = watableGestCat.getData('checked');
            var totale = 0;

            for (var i = 0; i < watableGestCat.getData('checked').rows.length; i++)
                totale += parseFloat(watableGestCat.getData('checked').rows[i]['Sup. [ha]'].replace(",", "."));

            $('#Txt_SuperficieSAU').val(totale.toString().replace(".", ","));
            $('#lblSupUtil').text(totale.toString().replace(".", ","));

        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabGestCat", watableGestCat);
}


var dati_particella;
var watableParticelle
var current_pag_particelle = 1;

function AggiornaTabParticelle(d) {

    AgroWA_Table_sistemaDati(d);

    $('#tabParticelle').html('');
    watableParticelle = $("#tabParticelle").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: true,
        preFill: false,
        tableCreated: function (data) {
            //                    $('.add_particella_val').each(function(i, obj) {
            //                        $(this).hide();
            //                    });
            nascondiColonnaTabella("#tabParticelle", 1);
            nascondiColonnaTabella("#tabParticelle", 10);
            nascondiColonnaTabella("#tabParticelle", 11);
            nascondiColonnaTabella("#tabParticelle", 12);
            colora_righe();
            if (event != undefined) {
                event.preventDefault();
            }

        },
        checkboxes: true,
        pageChanged: function (data) {
            current_pag_particelle = data.page;
        },
        rowClicked: function (data) {

            if (data.event.target.className == "unique") {
                if (data.checked)
                    aggiungi_particella_ws(dati_particella, 0);
                else
                    elimina_particella_ws(dati_particella, 0);
            }


        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    check_particelle();

    InitWaTable("#tabParticelle", watableParticelle);
}


function check_particelle() {

    var arr_chk = watableParticelle.getData('checked');
    for (var i = 0; i < watableParticelle.getData('checked').rows.length; i++) {
        //                if(arr.rows[i]['checked'] == 1)
        //                {
        var chiave = arr_chk.rows[i]['chiave']
        var res2 = chiave.split("_");

        var Sezione = res2[2];
        if (Sezione == 0)
            Sezione = ""
        var Foglio = res2[3];
        var Numero = res2[4];
        var Subalterno = res2[5];
        if (Subalterno == 0)
            Subalterno = ""
        //var Area = res2[6];

        //Check della riga particella
        $("#tabParticelle .watable tr").each(function () {
            if (($(this).find('td.pos_sez').text() == Sezione) && ($(this).find('td.pos_fogl').text() == Foglio) && ($(this).find('td.pos_num').text() == Numero) && ($(this).find('td.pos_sub').text() == Subalterno)) {
                // $(this).find('input[type=checkbox]').prop('checked', true);
                //$(this).find('.add_particella_val').attr('disabled', false);
                $(this).find('.add_particella_val').val(arr_chk.rows[i]['SuperficieIntersezione']);
            }

        });

        //               }

    }

}



function controlla_form() {

    var flag = true;
    var n_inv = 0;

    v.resetForm();

    if ($('#chk_serra').is(':checked')) {
        //                if(/^\d+$/.test($('#<=Txt_SuperficieCoperta.ClientID %>').val())) {
        //                }
        //                else
        //                {
        //                    $('#<=Txt_SuperficieCoperta.ClientID %>').parent().append('<label id="<=Txt_SuperficieCoperta.ClientID %>-error" class="custom_val error" for="<=Txt_SuperficieCoperta.ClientID %>">Il campo deve essere un numero</label>');
        //                    $('#<=Txt_SuperficieCoperta.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
        //                    flag = false;
        //                }

        $('#Txt_SuperficieCoperta').val($('#Txt_SuperficieCoperta').val().replace(/,/g, '.'));

        if ($('#Txt_SuperficieCoperta').val() < 0) {
            $('#Txt_SuperficieCoperta').parent().append('<label id="Txt_SuperficieCoperta-error" class="custom_val error" for="Txt_SuperficieCoperta">' + TraduzioneMultiResx(campoEditResx, 'IlCampoDeveEssereMaggioreDiZero', 'Il campo deve essere maggiore di 0') + '</label>');
            $('#Txt_SuperficieCoperta').parent().children(".required").css('border', '1px solid #D41E1A');
            n_inv++;
            flag = false;
        }

        if ($('#TxtDenominazione').val() == "") {
            $('#TxtDenominazione').parent().append('<label id="TxtDenominazione-error" class="custom_val error" for="TxtDenominazione">' + TraduzioneMultiResx(campoEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
            $('#TxtDenominazione').parent().children(".required").css('border', '1px solid #D41E1A');
            n_inv++;
            flag = false;
        }

    }



    //if ($('#TxtDataVariazioneAppAggr').val() == "") {
    //    $('#TxtDataVariazioneAppAggr').parent().append('<label id="TxtDataVariazioneAppAggr-error" class="custom_val error" for="TxtDataVariazioneAppAggr">Il campo deve essere compilato</label>');
    //    $('#TxtDataVariazioneAppAggr').parent().children(".required").css('border', '1px solid #D41E1A');
    //    n_inv++;
    //    flag = false;
    //}
    //            else
    //            {

    var dal
    if ($('#TxtValiditaInizio').val() == "")
        dal = new Date("01/01/1900");
    else {
        values = $('#TxtValiditaInizio').val().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        dal = new Date(final);
    }

    var al
    if ($('#TxtValiditaFine').val() == "")
        al = new Date("12/31/2100");
    else {
        values = $('#TxtValiditaFine').val().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        al = new Date(final);
    }

    values = $('#TxtDataVariazioneAppAggr').val().split('/');
    var final = values[1] + "/" + values[0] + "/" + values[2];


    var cur = new Date(final);

    //DRUDI 02-04-2020 eliminato controllo su data
    //if ((cur < dal) || (cur > al)) {
    //    $('#TxtDataVariazioneAppAggr').parent().append('<label id="TxtDataVariazioneAppAggr-error" class="custom_val error" for="TxtDataVariazioneAppAggr">La data deve essere compresa nel periodo attivo del Centro</label>');
    //    $('#TxtDataVariazioneAppAggr').css('border', '1px solid #D41E1A');
    //    n_inv++;
    //    flag = false;
    //}

    var centro_dal
    var centro_al

    if ($('#InizioCentro').val() == "")
        centro_dal = new Date("01/01/1900");
    else {
        values = $('#InizioCentro').val().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        centro_dal = new Date(final);
    }

    if ($('#FineCentro').val() == "")
        centro_al = new Date("12/31/2100");
    else {
        values = $('#FineCentro').val().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        centro_al = new Date(final);
    }


    if ((dal < centro_dal) || (dal > centro_al)) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">'
            + TraduzioneMultiResx(campoEditResx, 'DataDeveEssereCompresaNelPeriodoAttivoCentro', 'La data deve essere compresa nel periodo attivo del Centro')
            + '</label>');
        $('#TxtValiditaInizio').css('border', '1px solid #D41E1A');
        n_inv++;
        flag = false;
    }

    if ((al < centro_dal) || (al > centro_al)) {
        $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">'
            + TraduzioneMultiResx(campoEditResx, 'DataDeveEssereCompresaNelPeriodoAttivoCentro', 'La data deve essere compresa nel periodo attivo del Centro')
            + '</label>');
        $('#TxtValiditaFine').css('border', '1px solid #D41E1A');
        n_inv++;
        flag = false;
    }


    //           }




    if (v.valid() && flag) {
        return true
    }
    else {
        var err_message = "";
        err_message = "<div class='error-tab' style='position: absolute; right: 5px; top: 0; background-color: red; width: 10px; text-align: center;'><b>!</b></div>";
        //$('.nav-tabs li.active a').append(err_message);
        $('label.error').each(function () {
            var id = $(this).closest("div.tab-pane").attr('id');
            $('#navigator').find('.' + id + '').append(err_message);
        });
        //$('label.error').append(err_message);

        return false;
    }


}


function ValidaxSubmit() {

    var flag = controlla_form();

    if (flag) {

        // Memorizzo il valore delle specie vegetali in sessione (per salvataggio)
        $.ajax({
            type: 'POST',
            url: 'Campo_Edit.aspx/Salva_SpecieVegetali_In_Session',
            data: "{specie:'" + $('#Cmb_SpecieVegetale').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: false,
            success: function (r) {
            }
        });


        var app_arr = "";

        for (var i = 0; i < watableGestCat.getData('checked').rows.length; i++) {
            app_arr += watableGestCat.getData('checked').rows[i]['chiave'] + "|";

            //app_arr.push(watableGestCat.getData('checked').rows[i]['chiave']);
        }

        // salvo le righe checked (cambiate lato client) per il Salva Tutto
        $.ajax({
            type: 'POST',
            url: 'Campo_Edit.aspx/Salva_Appezzamenti_X_Salva_Tutto',
            data: "{dati: '" + app_arr + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: false,
            success: function (r) {
            }
        });

        let prosegui = Check_PianoConcimazione_EntitaxTestata()
        if (prosegui)
            $('#ImgBtn_SalvaTutto').click();

    }

}

function inizializzaKendoCodiciCampi(idDiv) {
    if (gridCodici !== undefined) {
        gridCodici.destroy();
    }
    var funzioniCRUD = {
        funzioneRead: dataGrigliaCodiciCampi,
        funzioneInsert: CodiceCampo,
        funzioneUpdate: CodiceCampo,
        funzioneDelete: CodiceCampo
    };

    var idModel = "Id_Cod";
    var campiKendoModel = modelGrigliaCodiciCampi();
    var colonneKendoGrid = colonneGrigliaCodiciCampi();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var template = kendo.template($("#popupCodici_Template").html());

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        editable: {
            mode: "popup",
            template: template,
            window: {
                title: TraduzioneMultiResx(campoEditResx, "CodiciCampo", "Codici Campo")
            }
        },
        cancel: function () {
            $("#" + IDControllo).data('kendoGrid').refresh();
        },
        colonneCustomKendoGrid: [
            {
                command: [{
                    name: "edit",
                    text: {
                        edit: "",
                        update: TraduzioneMultiResx(campoEditResx, "ConfermaDati", "Conferma Dati"),
                        cancel: TraduzioneMultiResx(campoEditResx, "Annulla", "Annulla")
                    }
                },
                {
                    name: "destroy",
                    text: "",
                    className: "k-custom-delete"//, iconClass: "fa fa-trash-o"
                }
                ],
                title: TraduzioneMultiResx(campoEditResx, "Operazioni", "Operazioni"),
                width: "80px"
            }
        ]
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundCodiciCampi,
        funzioneDaChiamareDopoEdit: CodiciCampi_onEdit,
        funzioneDaChiamareDopoAnnulla: onAnnullaCodiciCampi
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
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
    gridCodici = $("#" + idDiv).data("kendoGrid");
}

function CodiceCampo(e) {
    let data = e.data.models;
    let id_codArr = new Array();
    let sameID = false;
    for (let i = 0; i < data.length; i++) {
        if (id_codArr.includes(data[i].Id_Cod)) {
            sameID = true;
        } else {
            id_codArr.push(data[i].Id_Cod);
        }
    }
    if (sameID) {
        kendo.alert("Non è possibile inserire 2 codici uguali.");
    } else {
        gridCodici.refresh();
        jsCodici = JSON.stringify(gridCodici.dataSource.data());
        aggiornaCod(jsCodici);
    }
}

async function aggiornaCod(str) {
    await AggiornajsCodici(str);
}

function onAnnullaCodiciCampi() {
    setTimeout(function () { gridCodici.refresh(); }, 0);
}

function CodiciCampi_onEdit(e) {
    inizializzaCmb_Codici(e);

    if (!e.model.isNew()) {
        Cmb_Codici.enable(false);
    }

}

function dataGrigliaCodiciCampi(options) {
    //var a = JSON.parse(jsCodici);
    options.success(jsCodici);
}

function modelGrigliaCodiciCampi(options) {
    var a = {
        "Id_Cod": {
            "editable": true,
            "type": "number"
        },
        "Descrizione": {
            "editable": true,
            "type": "string"
        },
        "Val_Cod": {
            "editable": true,
            "type": "string"
        }
    };
    return a;
}

function colonneGrigliaCodiciCampi(options) {
    var a = [{
        "field": "Descrizione",
        "title": "Descrizione"
    },
    {
        "field": "Val_Cod",
        "title": "Valore"
    }];
    return a;
}

function App_onDataBoundCodiciCampi() {

}

function inizializzaCmb_Codici(mod) {
    return new Promise((resolve, reject) => {
        let obj_codiciSpec = new Array();

        obj_codiciSpec = obj_Codici;
        Cmb_Codici = $("#Cmb_Codici").kendoDropDownList({
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: obj_codiciSpec,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                mod.model.Id_Cod = this.value();
                mod.model.Descrizione = this.text();
            },
            change: function (e) {
                mod.model.Id_Cod = this.value();
                mod.model.Descrizione = this.text();
            }
        }).data("kendoDropDownList");
    });
}


async function filtra_watable_app() {
    var resp = await Aggiorna_Appezzamenti_DaDateWS($('#TxtValiditaInizio').val(), $('#TxtValiditaFine').val(), JSON.parse(objP_agenda).Piva, JSON.parse(objP_agenda).Sa_Cod, JSON.parse(objP_agenda).Campo_Cod)
    AggiornaTabGestCat(JSON.parse(resp));
}

function aggiorna_tot_sup_catasto() {
    var arr_chk = watableParticelle.getData('checked').rows;
    var tot = 0;
    $.each(arr_chk, function (key, value) {
        tot += parseFloat(value["Sup.Int"].replace(/,/g, "."));
    });

    $('#lblSupCatasto').text(tot);
}


function aggiungi_particella_ws(dati, tipo) {


    $.ajax({
        type: 'POST',
        url: 'Campo_Edit.aspx/Aggiungi_Particella_WS',
        data: "{dati:'" + dati + "', tipo:" + tipo + "}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: false,
        success: function (r) {
            if (tipo == 1)
                AggiornaTabParticelle(JSON.parse(r.d));

        }
    });

    aggiorna_tot_sup_catasto();

}

function elimina_particella_ws(dati, tipo) {


    $.ajax({
        type: 'POST',
        url: 'Campo_Edit.aspx/Elimina_Particella_WS',
        data: "{dati:'" + dati + "', tipo:" + tipo + "}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: false,
        success: function (r) {
            if (tipo == 1)
                AggiornaTabParticelle(JSON.parse(r.d));

        }
    });

    aggiorna_tot_sup_catasto();

}

function colora_righe() {

    $('#tabParticelle .watable tbody tr').each(function () {
        if (parseFloat($(this).find('td.pos_sup_disp').text().replace(/,/g, ".")) < 0) {
            $(this).removeClass("odd")
            $(this).find('td').css('background-color', 'rgba(216, 47, 43, 0.4)');

        }

    });
}

$("#TxtValiditaInizio").change(async function () {
    WaitFrame.show();
    var resp = await ControlloDataInizio($("#TxtValiditaInizio").val());
    if (resp) {
        ValiditaInizio_Old = $("#TxtValiditaInizio").val();
        await filtra_watable_app();
    } else {
        // i18n
        kendo.alert("Il Campo contiene appezzamenti con data iniziale inferiore alla data inserita, verranno aggiornate le date di questi appezzamenti");
    }
    WaitFrame.hide();
});

$("#TxtValiditaFine").change(async function () {
    WaitFrame.show();
    var resp = await ControlloDataFine($("#TxtValiditaFine").val());
    if (resp) {
        ValiditaFine_Old = $("#TxtValiditaFine").val();
        await filtra_watable_app();
    } else {
        // i18n
        kendo.alert("Il Campo contiene appezzamenti con data finale superiore alla data inserita, verranno aggiornate le date di questi appezzamenti");
    }
    WaitFrame.hide();
});