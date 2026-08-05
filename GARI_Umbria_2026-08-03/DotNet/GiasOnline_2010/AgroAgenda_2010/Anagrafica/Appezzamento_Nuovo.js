function ValidaxSubmit() {

    var flag = controlla_form();

    // Se il form è validato...
    if (flag) {
        //... Webservice, salvo in Session le variabili che mi servono
        var cmb_finalita = $('#Cmb_Finalita').val();
        var cmb_specie = $('#Cmb_Specie').val();
        var Cmb_CodiciTerreno = $('#Cmb_CodiciTerreno').val();

        if ($('#ChkTerrenoNudo').is(':checked')) {
            cmb_finalita = "";
            cmb_specie = "";
        } else {
            Cmb_CodiciTerreno = "";
        }

        $.ajax({
            type: 'POST',
            url: 'Appezzamento_Nuovo.aspx/Variabili_In_Session',
            data: "{cmb_finalita:'" + cmb_finalita + "', cmb_specie:'" + cmb_specie + "', Cmb_CodiciTerreno:'" + Cmb_CodiciTerreno + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                $('#ImgBtn_SalvaTutto').click();
            }
        });
    }
}

function ValidaxSubmit_Procedi1(dettagli_colturali) {

    var flag = controlla_form();

    // Se il form è validato...
    if (flag) {
        //... Webservice, salvo in Session le variabili che mi servono
        var cmb_finalita = $('#Cmb_Finalita').val();
        var cmb_specie = $('#Cmb_Specie').val();
        var Cmb_CodiciTerreno = $('#Cmb_CodiciTerreno').val();

        if ($('#ChkTerrenoNudo').is(':checked')) {
            cmb_finalita = "";
            cmb_specie = "";
        } else {
            Cmb_CodiciTerreno = "";
        }

        var RigheSelezionate = new Array();
        var filteredData = kGetElementiSelezionatiParticelle("#kendo_Particelle");

        $.each(filteredData, function (idx, dataItem) {
            RigheSelezionate.push(dataItem);
        });

        if (RigheSelezionate.length == 0) {
            kendo.alert(TraduzioneMultiResx(appezzamentoNuovoResx, "SelezionareAlmenoUnaParticella", "Selezionare almeno una particella"));
            return false;
        }

        impostaTipoFiltro();

        $("#hParticelleSelezionate").val(JSON.stringify(RigheSelezionate));
        $("#hParticelleSelezionate").trigger("change");

        $.ajax({
            type: 'POST',
            url: 'Appezzamento_Nuovo.aspx/Variabili_In_Session',
            data: "{cmb_finalita:'" + cmb_finalita + "', cmb_specie:'" + cmb_specie + "', Cmb_CodiciTerreno:'" + Cmb_CodiciTerreno + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                $("#hParticelleSelezionate").trigger("change");
                $("#TxtSuperficie").text($("#TxtSuperficie").val());
                if (dettagli_colturali) {
                    $('#ImgBtn_SalvaTutto_Procedi3').click();
                } else {
                    $('#ImgBtn_SalvaTutto_Procedi1').click();
                }
            }
        });
    }
}

function ValidaxSubmit_Procedi2() {

    // TODO In caso di disabilitazione TxtSuperficie
    //if ($('#TxtSuperficie').prop('disabled')) {
    //    return ValidaxSubmit_Procedi2(true);
    //}

    var flag = controlla_form();

    // Se il form è validato...
    if (flag) {
        //... Webservice, salvo in Session le variabili che mi servono
        var cmb_finalita = $('#Cmb_Finalita').val();
        var cmb_specie = $('#Cmb_Specie').val();
        var Cmb_CodiciTerreno = $('#Cmb_CodiciTerreno').val();

        if ($('#ChkTerrenoNudo').is(':checked')) {
            cmb_finalita = "";
            cmb_specie = "";
        } else {
            Cmb_CodiciTerreno = "";
        }

        $.ajax({
            type: 'POST',
            url: 'Appezzamento_Nuovo.aspx/Variabili_In_Session',
            data: "{cmb_finalita:'" + cmb_finalita + "', cmb_specie:'" + cmb_specie + "', Cmb_CodiciTerreno:'" + Cmb_CodiciTerreno + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                $('#ImgBtn_SalvaTutto_Procedi2').click();
            }
        });
    }
}


function controlla_form() {

    var flag = true;
    var n_inv = 0;

    v.resetForm();

    // Azzero tutte le label custom_val
    $(".custom_val").each(function (i, obj) {
        $(this).css('border', '1px solid #ccc');
        $(this).parent().children().css('border-color', '#ccc');
        $(this).remove();
    });

    
    if ($('#TxtSuperficie').val() == "" || $('#TxtSuperficie').val() == null) {
        $('#TxtSuperficie').parent().append('<label id="TxtSuperficie-error" class="custom_val error" for="TxtSuperficie">' + TraduzioneMultiResx(appezzamentoNuovoResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
        $('#TxtSuperficie').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    if ($('#TxtValiditaInizio').val() == "" || $('#TxtValiditaInizio').val() == null) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + TraduzioneMultiResx(appezzamentoNuovoResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
        $('#TxtValiditaInizio').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    //if ($('#TxtValiditaFine').val() == "") {
    //    $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">Il campo deve essere compilato</label>');
    //    $('#TxtValiditaFine').parent().children(".required").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}


    // Controllo se il Terreno Nudo è stato selezionato
    if ($('#ChkTerrenoNudo').is(':checked')) {

        if ($('#Cmb_CodiciTerreno').val() == "" || $('#Cmb_CodiciTerreno').val() == null) {
            $('#Cmb_CodiciTerreno').parent().append('<label id="Cmb_CodiciTerreno-error" class="custom_val error" for="Cmb_CodiciTerreno">' + TraduzioneMultiResx(appezzamentoNuovoResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $('#Cmb_CodiciTerreno').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }
    }
    else {

        if ($('#Cmb_Specie').val() == "" || $('#Cmb_Specie').val() == "0" || $('#Cmb_Specie').val() == null) {
            $('#Cmb_Specie').parent().append('<label id="Cmb_Specie-error" class="custom_val error" for="Cmb_Specie">' + TraduzioneMultiResx(appezzamentoNuovoResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $('#Cmb_Specie').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Cmb_Finalita').val() == "" || $('#Cmb_Finalita').val() == null) {
            $('#Cmb_Finalita').parent().append('<label id="Cmb_Finalita-error" class="custom_val error" for="Cmb_Finalita">' + TraduzioneMultiResx(appezzamentoNuovoResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato') + '</label>');
            $('#Cmb_Finalita').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

    }



    if (v.valid() && flag) {
        return true
    }
    else {
        var err_message = "";
        err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red'>" + n_inv + "</div>";
        $('.nav-tabs li.active a').append(err_message);

        return false;
    }


}

async function AggiungiDettaglioCatastale() {
    WaitFrame.show();
    $("#DettagliCatastali").show();
    $("#btn_salva1").hide();
    $("#btn_salva3").hide();
    $("#TxtSuperficie").attr("disabled", "disabled");
    await CaricaGrigliaParticelle();
    WaitFrame.hide();
}

$('#checkMacrousi').change(async function () {
    WaitFrame.show();
    impostaTipoFiltro();
    await CaricaGrigliaParticelle();
    WaitFrame.hide();
});

$('#checkUtilizzi').change(async function () {
    WaitFrame.show();
    await CaricaGrigliaParticelle();
    WaitFrame.hide();
});

$('#TxtValiditaInizio').change(async function () {
    if ($("#DettagliCatastali").is(":visible")) {
        WaitFrame.show();
        await CaricaGrigliaParticelle();
        WaitFrame.hide();
    }
});

$('#TxtValiditaFine').change(async function () {
    if ($("#DettagliCatastali").is(":visible")) {
        WaitFrame.show();
        await CaricaGrigliaParticelle();
        WaitFrame.hide();
    }
});

async function CaricaGrigliaParticelle() {
    let objAgenda = JSON.parse(objP_agenda);
    let Campo_Cod = objAgenda.Campo_Cod;
    let Appezza = 0;
    let flag_Macrousi = $('#checkMacrousi').is(":checked");
    let flag_Utilizzi = $('#checkUtilizzi').is(":checked");
    let flag_Varieta = false
    let DataValiditaInizio = "01/01/1900";
    if ($("#TxtValiditaInizio").val() !== "" && $("#TxtValiditaInizio").val() !== undefined) {
        DataValiditaInizio = $("#TxtValiditaInizio").val()
    }
    let DataValiditaFine = "31/12/2100";
    if ($("#TxtValiditaFine").val() !== "" && $("#TxtValiditaFine").val() !== undefined) {
        DataValiditaFine = $("#TxtValiditaFine").val()
    }
    let flag_Modifica = false;
    let objParam = {
        Campo_Cod: Campo_Cod,
        Appezza: Appezza,
        flag_Macrousi: flag_Macrousi,
        flag_Utilizzi: flag_Utilizzi,
        flag_Varieta: flag_Varieta,
        DataValiditaInizio: DataValiditaInizio,
        DataValiditaFine: DataValiditaFine,
        flag_Modifica: flag_Modifica
    }
    let param = JSON.stringify(objParam);
    var resp = await CaricaKendo_Particelle(param);
    jSonParsed_Kendo_Particelle = JSON.parse(resp);
    GrigliaKendoParticelle("kendo_Particelle");
}

function impostaTipoFiltro() {
    var tipoFiltro = 0;
    if ($('#checkMacrousi').is(":checked")) {
        tipoFiltro = 1;
    }

    if ($('#checkUtilizzi').is(":checked")) {
        tipoFiltro = 2;
    }

    $("#hTipoFiltro").val(tipoFiltro);

}

//Aggiornamento della superficie Appezzamento
function RicalcolaSuperficieAppezzamento() {
    var SupTot = 0.00;
    //sel la checkbox è chekkata

    var filteredData = kGetElementiSelezionatiParticelle("#kendo_Particelle");

    $.each(filteredData, function (idx, dataItem) {

        var app = ValoreSuperficieCoinvolta_Particella($(dataItem));

        //se = "" --> significa che provengo da un evento di salvataggio del valore della cella. recupero il valore da variabile appoggio.
        /* if (app == "") {
            app = kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".");
            kEventoSelezionaRiga_Sup_Impiegata = 0;
        }
        if (kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".") > 0) {
            app = kEventoSelezionaRiga_Sup_Impiegata.toString().replace(",", ".");
            kEventoSelezionaRiga_Sup_Impiegata = 0;
        } */
        if (app!="") SupTot += parseFloat(app.toString().replace(",", "."));
    });
    InserisciSupTotale(SupTot);
}

function InserisciSupTotale(valore) {
    var app = roundNumber(valore, 4) + "";
    app = app.replace(".", ",");
    $('#LblSuperficie_Con_Catasto').text(app);
    
    $("#TxtSuperficie").val(app);
    $("#TxtSuperficie").trigger("change");
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
        if (dataItem.Selected || dataItem.ChkSelezionaParticella == "True")
            rval.push(dataItem);
    });

    return rval;
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

/**
 * Kendo griglia particelle
 */
var jSonParsed_Kendo_Particelle;

function kendoRefresh(jQuerySelector) {
    $(jQuerySelector).data("kendoGrid").refresh();
}

function Kendo_Particelle_leggi(options) {
    options.success(jSonParsed_Kendo_Particelle.kendo_rows);
}

function Aggiorna_Particelle(options) {

}

function onDataBoundRigheParticelle() {

    //selezionaParticelleImpiegate();
    //kendoRefresh("#kendo_Particelle");

}

function postDataBoundRigheParticelle() {
    //richiamare una funzione dalla master

    kendo_AggiustaDimensioneColonne("#kendo_Particelle");
    // var grid = $("#kendo_Particelle").data("kendoGrid");
    // for (var i = 0; i < grid.columns.length; i++) {
    //     grid.autoFitColumn(i);
    // }   

    //kendoRefresh("#kendo_Particelle");
    //selezionaRighe("#kendo_Particelle");


}

function kReadParticelle_mod() {
    return jSonParsed_Kendo_Particelle.kendo_model;

}

function kReadParticelle_col() {
    var columns = jSonParsed_Kendo_Particelle.kendo_columns;
    /* Qui aggiungo le colonne che mi interessano in lettura aggiuntive */
    
    kendo_Colonne_estendi(jSonParsed_Kendo_Particelle, "SuperficieImpiegata", TraduzioneMultiResx(appezzamentoNuovoResx, "SuperficieImpiegata", "Superficie Impiegata") + " [Ha]", 2, "SuperficieImpiegata", numberEditor4decimals, "Sup_Coinvolta");
    return columns;
}

function postSelectedRigheParticelle() {
    // Metto la colonna checked a 1
    // abilito la modifica della superificie impiegata
    // 
}

var kEventoSelezionaRiga_Sup_Impiegata = 0;

function ValoreSuperficieCoinvolta_Particella(Oggetto) {

    return kGetSuperficie(Oggetto, "SuperficieImpiegata");

}

function onEditKendoSup_Impiegata(e) {
    var Sup_Disponibile = e.model.SuperficieImpiegata + e.model.SuperficieCondottaDisponibile;



    if (e.values.SuperficieImpiegata <= Sup_Disponibile) {

        // se posso la sup_impiegata è corretta eseguo i calcoli per aggiornare la superficie totale dell'appezza
        kEventoSelezionaRiga_Sup_Impiegata = e.values.SuperficieImpiegata;
        e.model.SuperficieImpiegata = kEventoSelezionaRiga_Sup_Impiegata;

        //calcoli finali ..
        RicalcolaSuperficieAppezzamento();
        //RicalcolaSuperficieCoinvolta();
        //Aggiorna_hdKendo_Impianti_Selezione(e);

        //AggiornaDopo_SupTrattata();

    } else {

        e.preventDefault();

        kEventoSelezionaRiga_Sup_Impiegata = e.values.SuperficieImpiegata;
        e.model.SuperficieImpiegata = kEventoSelezionaRiga_Sup_Impiegata;

        //calcoli finali ..
        RicalcolaSuperficieAppezzamento();
        MessaggioErrore(TraduzioneMultiResx(appezzamentoNuovoResx, "InseritaSuperficieImpiegoSuperioreDisponibile", "Hai inserito una superficie d'impiego superiore alla superficie disponibile"));
    }

    e.model.SuperficieCondottaDisponibile = Sup_Disponibile - e.values.SuperficieImpiegata;

    var grid = $("#kendo_Particelle").data("kendoGrid");
    grid.refresh();
}

function kEventoSelezionaRiga(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#kendo_Particelle").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    if (checked) {
        //-select the row
        row.addClass(GIAS_K_STATE_SELECTED);
        dataItem.ChkSelezionaParticella = "True";
        if (dataItem.SuperficieCondottaDisponibile > 0) {
            if (dataItem.SuperficieImpiegata === 0) {
                dataItem.SuperficieImpiegata = dataItem.SuperficieCondottaDisponibile;
                dataItem.SuperficieCondottaDisponibile = dataItem.SuperficieCondottaDisponibile - dataItem.SuperficieImpiegata;
            }
            row.find('.Sup_Coinvolta').click();
            //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text())+ parseFloat(dataItem.SuperficieImpiegata));
        }
        //se diverso da zero, vuol dire che esiste una superficie che arriva dalla lettura dei dati lato server.
        // if (kEventoSelezionaRiga_Sup_Coninvolta != 0) {

        //     Sup_Imp_help_Format = kEventoSelezionaRiga_Sup_Coninvolta.toString().replace(",", ".");
        //     dataItem.Sup_Imp_help = kEventoSelezionaRiga_Sup_Coninvolta;                    
        //     kEventoSelezionaRiga_Sup_Coninvolta = 0;

        // } else {

        //     Sup_Imp_help_Format = kendo.toString(dataItem.Sup_Imp_help, "n4").replace(",", ".");
        //     dataItem.Sup_Imp_help = dataItem.Sup_Imp;
        //     if (Esegui_CalcoliFinali)             
        //         kendoRefresh("#kendo_Particelle");

        // }

    } else {
        //-remove selection
        row.removeClass(GIAS_K_STATE_SELECTED);
        dataItem.ChkSelezionaParticella = "False";
        //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text())- parseFloat(dataItem.SuperficieImpiegata));

        dataItem.SuperficieCondottaDisponibile = dataItem.SuperficieCondottaDisponibile + dataItem.SuperficieImpiegata;
        dataItem.SuperficieImpiegata = 0;

    }

    //informo che la riga è cambiata..
    dataItem.dirty = true;


    //calcoli finali ..
    // if (Esegui_CalcoliFinali) {
    //     RicalcolaSuperficieTotale();
    //     RicalcolaSuperficieCoinvolta();
    //     Aggiorna_hdKendo_Impianti_Selezione();
    //     AggiornaDopo_SupTrattata();
    // }
    RicalcolaSuperficieAppezzamento();
    grid.refresh();
}


/* creazione funzione Kendo_multiSelect */
function GrigliaKendoParticelle(div) {

    var funzioniCRUD = {
        funzioneRead: Kendo_Particelle_leggi
        //, funzioneUpdate: Aggiorna_Particelle
        , checkBoxFunction: kEventoSelezionaRiga
    };
    var idModel = "chiave"; /*todo*/
    var campiKendoModel = kReadParticelle_mod(); //kendo_model
    var colonneKendoGrid = kReadParticelle_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {
        //sort: {
        //    field: "SuperficieImpiegata",
        //    dir: "desc"
        //}
    };
    //if (!client_operazione) {
        //parametriDataSource.filter = { field: "SuperficieImpiegata", operator: "gt", value: 0 };
    //}
    var parametriKendoGrid = {
        groupable: false,
        //scrollable: true,
        editable: true,
        resizable: true,
        columMenu: false,
        pdf: false,
        excel: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "menu" },
        checkSelezioneRiga: { filterable: false, field: null, width: '30px' }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: postDataBoundRigheParticelle,
        funzioneDaChiamarePrimaDelDataBound: onDataBoundRigheParticelle,
        funzioneDaChiamareDopoSelectAllRows: postSelectedRigheParticelle,
        funzioneDaChiamareDopoSave: onEditKendoSup_Impiegata
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

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
    //var griglia = $('#' + div).data("kendoGrid");

    //griglia.autoFitColumn("Data2");
}