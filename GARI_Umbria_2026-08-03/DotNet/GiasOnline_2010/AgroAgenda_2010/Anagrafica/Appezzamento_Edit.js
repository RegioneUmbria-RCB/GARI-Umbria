/**
 *  Creazione 2017-04-20
 *
 */



/**
 * Funzioni Custom
 */

//'  Vanni, 28/05/2014 15:04:00: gestione del log in console...
jQuery.logThis = function (text) {
    if ((window['console'] != undefined)) {
        console.log(text);
    }
}


// disabilitazione del tasto BACK
document.onkeypress = function (event) {
    if (typeof window.event != 'undefined') { // ie
        event = window.event;
        event.target = event.srcElement; // make ie confirm to standards !!
    }
    var kc = event.keyCode;
    var tt = event.target.type;

    if ((kc != 8) || (tt == 'text') || (tt == 'password') || (tt == 'textarea'))
        return true;
    alert('Disabilitato il tasto BACK della tastiera');
    return false;
}

if (typeof window.event != 'undefined') // ie
    document.onkeydown = document.onkeypress; // Trap bksp in ie. !! Note: does not trap enter, but onkeypress does !!

//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide, elem_hide_all) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');



    if (tabella == "tabCodici") {

        $('#' + tabella + ' table thead tr').find('th:nth-child(1)').attr('data-toggle', 'true');


        if (elem_hide.length != 0) {
            jQuery.each(elem_hide, function (i, val) {
                $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
            });
        }

        if (elem_hide_all.length != 0) {
            jQuery.each(elem_hide_all, function (i, val) {
                $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'all');
            });
        }

    }

    $('#' + tabella + ' table').footable({
        breakpoints: {
            phone: 480,
            tablet: 700
        }
    });
}

function ValidaxSubmit() {

    var flag = controlla_form();

    var prosegui = WS_controllaMovimenti_CdG_xModificaValidita()
    switch (prosegui.TipoMessaggioRitorno) {
        case "alert":
            WaitFrame.hide();
            kendo.alert(prosegui.Messaggio);
            break;
        case "procedi":
            if (flag) {

                // Salvo le particelle dell'appezzamento
                if ($('#RBL_Catasto_Con').is(':checked')) {

                    // Tipo del salvataggio (scelte delle checkbox)
                    var type = 0;
                    if ($('#Chk_Macrousi').is(':checked'))
                        type = 1;

                    if ($('#Chk_Utilizzi').is(':checked'))
                        type = 2;

                    if ($('#Chk_Utilizzi').is(':checked') && $('#Chk_Macrousi').is(':checked'))
                        type = 3;

                    if ($('#Chk_Utilizzi').is(':checked') && $('#Chk_Macrousi').is(':checked') && $('#Chk_Varieta').is(':checked'))
                        type = 4;

                    var risultato = "";

                    var dataIni = $('#TxtValiditaInizio').val();
                    var dataFin = $('#TxtValiditaFine').val();

                    // ricalcolo la sup totale delle particelle
                    RicalcolaSuperficieAppezzamento();

                    //metto il valore della sup_app nella textbox TxtSuperficie
                    $('#TxtSuperficie').val($('#LblSuperficie_Con_Catasto').text());

                    // pusho tutte le particelle selezionate in una variabile Json
                    $('#hidden_pushedParticelleDaSalvare').val(JSON.stringify(kGetElementiSelezionatiParticelle("#kendo_Particelle")));
                    $('#hidden_tipoParticelleDaSalvare').val(type);
                    // Salvo tutto!
                }

                $('#ImgBtn_SalvaTutto').click();
            }
            break;
    }
}


function controlla_form() {

    var var_contr = new customValidator();

    //v.resetForm();

    // Azzero tutte le label custom_val
    $(".custom_val").each(function () {
        $(this).prev().closest("input").css('border', '1px solid #428BCA');
        //$(this).closest("input").css('border', '1px solid #428BCA');
        $(this).remove();
    });




    //Controlla_required("TxtValiditaInizio", var_contr, 1);

    if ($('#TxtValiditaFine').val() == "") {
        $('#TxtValiditaFine').val('31/12/2100');
        //  $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">Il campo non può essere vuoto</label>');
        //  $('#TxtValiditaFine').closest("input").css('border', '1px solid #D41E1A');
        // flag = false;
        //  n_inv++;
    }

    if ($('#TxtValiditaInizio').val() == "") {
        $('#TxtValiditaInizio').val('01/01/1900');
        //  $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">Il campo non può essere vuoto</label>');
        //  $('#TxtValiditaFine').closest("input").css('border', '1px solid #D41E1A');
        // flag = false;
        //  n_inv++;
    }

    // Controllo se le date sono corrette
    var TxtValiditaInizio = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine = $('#TxtValiditaFine').val().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    if (ini > fin) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + TraduzioneMultiResx(appezzamentoEditResx, 'DataInizioNonPuòEssereMaggioreDiDataFine', 'La data di inizio non può essere maggiore di quella di fine') + '</label>');
        $('#TxtValiditaInizio').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv++;
    }

    // Controllo se le date sono conformi a quelle del centro
    var InizioCentro = $('#InizioCentro').val().split("/");
    var FineCentro = $('#FineCentro').val().split("/");

    ini2 = new Date(InizioCentro[2], InizioCentro[1] - 1, InizioCentro[0]);
    fin2 = new Date(FineCentro[2], FineCentro[1] - 1, FineCentro[0]);

    if (ini < ini2) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">'
            + TraduzioneMultiResx(appezzamentoEditResx, 'DatainizioMinoreDelCentro', 'La data di inizio non può essere minore di quella del Centro')
            + '</label>');
        $('#TxtValiditaInizio').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv++;
    }


    if (fin > fin2) {
        $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">'
            + TraduzioneMultiResx(appezzamentoEditResx, 'DataFineMaggioreDelCentro', 'La data di fine non può essere maggiore di quella del Centro')
            + '</label>');
        $('#TxtValiditaFine').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv++;
    }


    if (!$('#Opt_Convenzionale').is(':checked')) {

        Controlla_required("Txt_NumeroAppBio", var_contr, 3);

        //Controlla_required("Txt_ConfiniRischio",var_contr,3);

    }

    // Controllo se il pendenza contiene valori non numerici

    if (Controlla_numero('TxtPendenza', var_contr, 1)) {
        if ($('#TxtPendenza').val() > 100) {
            $('#TxtPendenza').parent().append('<label id="TxtPendenza-error" class="custom_val error" for="TxtPendenza">'
                + TraduzioneMultiResx(appezzamentoEditResx, 'LaPendenzaPuòEssereAlMassimoCentoPerCento', 'La pendenza può essere al massimo 100%')
                + '</label>');
            $('#TxtPendenza').closest("input").css('border', '1px solid #D41E1A');
            var_contr.flag = false;
            var_contr.n_inv++;
        }
    }

    // Controllo se il coord X contiene valori non numerici
    Controlla_numero('TxtCoordinataX', var_contr, 1);

    // Controllo se il coord Y contiene valori non numerici
    Controlla_numero('TxtCoordinataY', var_contr, 1);

    // Controllo se il coord Z contiene valori non numerici
    Controlla_numero('TxtCoordinataZ', var_contr, 1);

    Controlla_numero('txtDistBZ_CorpiIdrici', var_contr, 1);
    Controlla_numero('txtDistBZ_AreeResPub', var_contr, 1);
    Controlla_numero('txtDistBZ_Allevamenti', var_contr, 1);
    Controlla_numero('txtDistBZ_VegNatNonColt', var_contr, 1);
    Controlla_numero('txtSupBZ_Riduzione', var_contr, 1);

    if ($('#RBL_Catasto_Con').is(':checked')) {
        if (Controlla_required_label("LblSuperficie_Con_Catasto", var_contr, 2)) {
            Controlla_numero_label('LblSuperficie_Con_Catasto', var_contr, 2);
        }
    }
    if ($('#RBL_Catasto_Senza').is(':checked')) {
        if (Controlla_required("TxtSuperficie_SenzaCatasto", var_contr, 1))
            Controlla_numero('TxtSuperficie_SenzaCatasto', var_contr, 1);
    }


    //v.valid() && 
    if (var_contr.flag) {

        $('.nav-tabs li a .error-tab').remove(".error-tab");
        return true;
    }
    else {
        var err_message = "";
        if (var_contr.n_inv > 0) {
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li:nth-child(1) a').append(err_message);
        }

        if (var_contr.n_inv2 > 0) {
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li:nth-child(2) a').append(err_message);
        }

        if (var_contr.n_inv3 > 0) {
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li:nth-child(3) a').append(err_message);
        }

        return false;
    }

}



function ModificaCodice(obj) {

    // Pulisco le strutture
    $('#CmbCodice').val('');
    $('.selectpicker').selectpicker('refresh');
    $('#TxtCodiceValore2').val('');
    $('#TxtValiditaInizioCodice').val('');
    $('#TxtValiditaFineCodice').val('');


    var Id_Cod = $(obj).attr('chiave');

    var i;

    for (i = 0; i < watableCodici.getData().rows.length; i++) {
        var q = watableCodici.getData().rows[i];
        if (q.Tool == Id_Cod) {
            var codice = q.Codice;

            $("#CmbCodice option").each(function () {

                if (Id_Cod == $(this).val()) {
                    $(this).prop('selected', true);
                    $('.selectpicker').selectpicker('refresh');
                }

                i = i + 1;

            });


            var valore = q.Valore;
            $('#TxtCodiceValore2').val(valore);

            var Dal = q.Dal;
            if (Dal == "...")
                Dal = "";
            $('#TxtValiditaInizioCodice').val(Dal);

            var Al = q.Al;
            if (Al == "...")
                Al = "";
            $('#TxtValiditaFineCodice').val(Al);

            break;
        }
    }

    //                 EliminaCodice(obj);

}

function nascondi_col_calcoli(col) {
    // nascondo l'ultima colonna della tabella Particella
    //$('#tabParticelle .watable th:nth-child(12)').hide();
    $('#tabParticelle .watable th:nth-child(' + col + ')').hide();

    //$('#tabParticelle .watable tr td:nth-child(12)').each(function () {
    $('#tabParticelle .watable tr td:nth-child(' + col + ')').each(function () {
        $(this).hide();
    });

    // $('#tabParticelle .watable th:nth-child('+col+')').attr('data-ignore','true');

    //nascondi sempre l'ultima colonna
    $('#tabParticelle .watable th:nth-last-child(1)').hide();

    //$('#tabParticelle .watable tr td:nth-child(12)').each(function () {
    $('#tabParticelle .watable tr td:nth-last-child(1)').each(function () {
        $(this).hide();
    });

    //////////

}

function colora_righe() {

    $('#tabParticelle .watable tbody tr').each(function () {
        if (parseFloat($(this).find('td.pos_sup_con_disp').text().replace(/,/g, ".")) < 0) {
            $(this).removeClass("odd")
            $(this).find('td').css('background-color', 'rgba(216, 47, 43, 0.4)');

        }

    });
}


var watableCodici;
var watableUtilizzo;
var watableParticelle;

/**
 *
 * @param {*} d
 */
function AggiornaTabCodici(d) {

    AgroWA_Table_sistemaDati(d);

    //var elem_hide = new Array(4,5);
    var elem_hide = new Array(5, 6);
    var elem_hide_all = [];


    $('#tabCodici').html('');
    watableCodici = $("#tabCodici").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
            applicaFooTable('tabCodici', elem_hide, elem_hide_all);
        },
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabCodici", watableCodici);
}



/**
 *
 * @param {*} d
 */
function AggiornaTabUtilizzo(d) {

    //AgroWA_Table_sistemaDati(d);

    $('#tabUtilizzo').html('');
    watableUtilizzo = $("#tabUtilizzo").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        tableCreated: function (data) {
            //applicaFooTable('tabCodici');
        },
        checkboxes: false,
        //tableCreated: function (data) {
        //    //                                    coloraMovimenti();
        //}
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabUtilizzo", watableUtilizzo);
}


//Aggiornamento della superficie Appezzamento
function RicalcolaSuperficieAppezzamento() {
    var SupTot = 0.00;
    //sel la checkbox è chekkata

    var filteredData = kGetElementiSelezionatiParticelle("#kendo_Particelle");

    $.each(filteredData, function (idx, dataItem) {

        var app = ValoreSuperficieCoinvolta_Particella($(dataItem));

        //se = "" --> significa che provengo da un evento di salvataggio del valore della cella. recupero il valore da variabile appoggio.
        /*if (app == "") {
            app = kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".");
            kEventoSelezionaRiga_Sup_Impiegata = 0;
        }
        if (kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".") > 0) {
            app = kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".");
            kEventoSelezionaRiga_Sup_Impiegata = 0;
        }*/
        if (app != "") SupTot += parseFloat(app.toString().replace(",", "."));
    });
    InserisciSupTotale(SupTot);
}

function kGetElementiSelezionatiParticelle(jquery_Selector) {
    var rval = new Array();

    var grid = $(jquery_Selector).closest("[data-role=grid]").data("kendoGrid");
    if (grid == undefined) {
        return rval;
    }

    var dataSource = grid.dataSource;
    var allData = dataSource.data();


    $.each(allData, function (idx, dataItem) {
        if (dataItem.Selected || dataItem.ChkSelezionaParticella)
            rval.push(dataItem);
    });

    return rval;
}

function selezionaParticelleImpiegate() {

    var datiGriglia = $("#kendo_Particelle").data('kendoGrid');
    var elemVisibiliHtml = datiGriglia.tbody.find("tr");
    var elemVisibiliDati = datiGriglia.dataSource.view();

    elemVisibiliDati.forEach(function (element, ind) {
        if (element.ChkSelezionaParticella === "True") {
            element.Selected = true;
            var row = $(elemVisibiliHtml[ind]);
            row.addClass(GIAS_K_STATE_SELECTED)
                .find(".checkbox")
                .attr("checked", "checked");
            //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text()) + element.SuperficieImpiegata);
            //checkedIds[dataItem.id] = true;
            //console.log(checkedIds[dataItem.id]);
            //row = $(element).parents("tr");
            //row
            //datiGriglia.select(row);
        }
    }, this);
}

function InserisciSupTotale(valore) {
    var app = roundNumber(valore, 4) + "";
    app = app.replace(".", ",");
    $('#LblSuperficie_Con_Catasto').text(app).trigger('change');
}


/////////////////////////////////////////////////////
/////////////////// SUPERFICI ///////////////////////
/////////////////////////////////////////////////////


function kGetSuperficie(OggettoSelezionato, tiposuperficie) {

    return OggettoSelezionato[0][tiposuperficie];

}

function kSetSuperficie(OggettoSelezionato, tiposuperficie, valore) {
    OggettoSelezionato.parent().parent().find("." + tiposuperficie).text(valore);
}