
function apriFormDialog(url) {
    $("#PaginaGeneric").attr("src", url);
    $("#iFrameGeneric").modal('toggle');
}


function ChiusuraModale() {
    $('#iFrameGeneric').modal("hide");
}

function iFrame_Chiusura() {
    $("#PaginaGeneric").attr("src", "");
    //ricarico le analisi
    $("#Btn_hidden_CaricaAnalisi").click();
}

function SelezionaDeselezionaTutti() {

    if ($('#chkSelezionaTuttiImpianti').is(':checked')) {
        //seleziono tutto
        $('.ChkSelezionaImpianto').each(function () {
            $(this).children('input').prop('checked', 'checked');
        });
    }
    else {
        //seleziono tutto
        $('.ChkSelezionaImpianto').each(function () {
            $(this).children('input').prop('checked', false);
        });
    }
}

function AggiornaDescrizione() {
    var Desc = $($(ddlRegolamento) & ' option:selected').text();
    if (Desc !== "") {
        $(Txt_Descrizione).val(Desc);
    }
    var anno = Desc.match((/\d/g));

    if (anno == null || anno == undefined) {
        anno = (new Date).getFullYear().toString()
    } else {
        anno = anno.join("");
    }

    var data_inizio = '01/01/' + anno;
    var data_fine = '31/12/' + anno;
    $(Txt_ValiditaInizio).val(data_inizio);
    $(Txt_ValiditaFine).val(data_fine);
    $(Txt_Anno).val(anno);
}

function disableEnterKey(e) {
    var key;
    if (window.event)
        key = window.event.keyCode; //IE
    else
        key = e.which; //firefox

    return (key != 13);
}

function IsNumeric(val) {
    if (isNaN(parseFloat(val))) {
        return false;
    }
    return true;
}

/****************************************************************************************************************/
function Valore_FattoreVariazione(Oggetto) {
    return Oggetto.parent().parent().parent().children('.Variazione').html().replace(',', '.');
}

function Valore_FattoreCodice(Oggetto) {
    return Oggetto.parent().parent().parent().children('.Codice').html().replace(',', '.');
}


/********** AZOTO ***********************************************************************************************/
function Chk_SelDecrementi_N_Click(Oggetto, tutti) {
    RicalcolaDecrementoTotaleN();
    CalcolaDoseN();
}
function Chk_SelIncrementi_N_Click(Oggetto, tutti) {
    RicalcolaIncrementoTotaleN();
    CalcolaDoseN();
}

//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaDecrementoTotaleN() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelDecrementi_N').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotDecrementi).val(Tot);
}

//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaIncrementoTotaleN() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelIncrementi_N').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);

        var cod = Valore_FattoreCodice($(this));
        if (cod == 46) {
            var max = $(Txt_MaxIncrementi).val();
            max += parseFloat(app);
            $(Txt_MaxIncrementi).val(max);
        }
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotIncrementi).val(Tot);

}

function InserisciIncTotaleN(valore) {
    var app = valore;
    $(Txt_TotIncrementi).val(app);
}

function CalcolaDoseN() {

    var TotIncrementi = $(Txt_TotIncrementi).val();
    if (!(IsNumeric(TotIncrementi))) {
        TotIncrementi = 0;
    }

    var MaxIncrementi = $(Txt_MaxIncrementi).val();
    if (!(IsNumeric(MaxIncrementi))) {
        MaxIncrementi = 0;
    }

    var TotDecrementi = $(Txt_TotDecrementi).val();
    if (!(IsNumeric(TotDecrementi))) {
        TotDecrementi = 0;
    }

    var DoseStd = $(Txt_DoseStandard).val();
    //var DoseMas = $('#<%=Txt_MAS.ClientId %>').val();
    var DoseMas = parseFloat($($(ddlMasS) & ' option:selected').val());

    var DoseRic = 0;
    DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

    if (parseFloat(DoseRic) < 0) {
        DoseRic = 0;
    } else if (DoseRic > (parseFloat(DoseStd) + parseFloat(MaxIncrementi))) {
        DoseRic = parseFloat(DoseStd) + parseFloat(MaxIncrementi);
    }
    if (parseFloat(DoseMas) > 0) {
        if (parseFloat(DoseRic) > parseFloat(DoseMas)) {
            DoseRic = parseFloat(DoseMas)
        }
    }

    //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
    var fase = parseFloat($($(ddlFaseCiclo) & ' option:selected').val());
    switch (fase) {
        case 1: case 2: case 3:
            break;
        default:
            $(Txt_DoseRicalcolata).val(DoseRic);
            $(N_Ammesso).val(DoseRic);
    }
}


/********** FOSFORO ***********************************************************************************************/
function ddlDoseP_Click() {
    AggiornaDoseStandardP();
    RicalcolaIncrementoTotaleP();
    RicalcolaDecrementoTotaleP();
    CalcolaDoseP();
}

function Chk_SelDecrementi_P_Click(Oggetto, tutti) {
    RicalcolaDecrementoTotaleP();
    CalcolaDoseP();
}
function Chk_SelIncrementi_P_Click(Oggetto, tutti) {
    RicalcolaIncrementoTotaleP();
    CalcolaDoseP();
}

//aggiorno dose standard
function AggiornaDoseStandardP() {

    //aggiorno la dose standard solo se non sono in 1 e 2 anno allevamento
    var fase = parseFloat($($(ddlFaseCiclo) & ' option:selected').val());
    switch (fase) {
        case 1: case 2: case 3:
            break;
        default:
            var Dose = $($(ddlDoseP) & ' option:selected').val().split("|")[1];
            $(Txt_DoseStandardP).val(Dose);
    }
}


//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaDecrementoTotaleP() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelDecrementi_P').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotDecrementiP).val(Tot);
}

//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaIncrementoTotaleP() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelIncrementi_P').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotIncrementiP).val(Tot);

}

function CalcolaDoseP() {
    var TotIncrementi = $(Txt_TotIncrementiP).val();
    if (!(IsNumeric(TotIncrementi))) {
        TotIncrementi = 0;
    }

    var TotDecrementi = $(Txt_TotDecrementiP).val();
    if (!(IsNumeric(TotDecrementi))) {
        TotDecrementi = 0;
    }

    var DoseStd = $(Txt_DoseStandardP).val();
    var DoseRic = 0;
    DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

    //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
    var fase = parseFloat($($(ddlFaseCiclo) & ' option:selected').val());
    switch (fase) {
        case 1: case 2: case 3:
            break;
        default:
            $(Txt_DoseRicalcolataP).val(DoseRic);
            $(P_Ammesso).val(DoseRic);
    }
}


/********** POTASSIO ***********************************************************************************************/

function ddlDoseK_Click() {
    AggiornaDoseStandardK();
    RicalcolaIncrementoTotaleK();
    RicalcolaDecrementoTotaleK();
    CalcolaDoseK();
}

function Chk_SelDecrementi_K_Click(Oggetto, tutti) {
    RicalcolaDecrementoTotaleK();
    CalcolaDoseK();
}
function Chk_SelIncrementi_K_Click(Oggetto, tutti) {
    RicalcolaIncrementoTotaleK();
    CalcolaDoseK();
}

//aggiorno dose standard
function AggiornaDoseStandardK() {

    //aggiorno la dose standard solo se non sono in 1 e 2 anno allevamento
    var fase = parseFloat($($(ddlFaseCiclo) & ' option:selected').val());
    switch (fase) {
        case 1: case 2: case 3:
            break;
        default:
            var Dose = $($(ddlDoseK) & ' option:selected').val().split("|")[1];
            $(Txt_DoseStandardK).val(Dose);
    }
}

//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaDecrementoTotaleK() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelDecrementi_K').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotDecrementiK).val(Tot);
}

//Funzione per l'aggiornamento della riduzione Totale
function RicalcolaIncrementoTotaleK() {
    var Tot = 0.00;
    //sel la checkbox è chekkata
    $('.Chk_SelIncrementi_K').children('input:checked').each(function () {
        var app = Valore_FattoreVariazione($(this));
        Tot += parseFloat(app);
    });
    //InserisciIncTotaleN(SupTot);
    $(Txt_TotIncrementiK).val(Tot);

}

function CalcolaDoseK() {
    var TotIncrementi = $(Txt_TotIncrementiK).val();
    if (!(IsNumeric(TotIncrementi))) {
        TotIncrementi = 0;
    }

    var TotDecrementi = $(Txt_TotDecrementiK).val();
    if (!(IsNumeric(TotDecrementi))) {
        TotDecrementi = 0;
    }

    var DoseStd = $(Txt_DoseStandardK).val();
    var DoseRic = 0;
    DoseRic = parseFloat(DoseStd) + parseFloat(TotIncrementi) - parseFloat(TotDecrementi);

    //aggiorno il calcolato solo se non sono in 1 e 2 anno allevamento
    var fase = parseFloat($($(ddlFaseCiclo) & ' option:selected').val());
    switch (fase) {
        case 1: case 2: case 3:
            break;
        default:
            $(Txt_DoseRicalcolataK).val(DoseRic);
            $(K_Ammesso).val(DoseRic);
    }

}

function ddlP_Click() {
    var forma = parseFloat($($(DDL_P2O5) & ' option:selected').val());
    var txtP = parseFloat($(Txt_P).val());
    switch (forma) {
        case 1:
            $(Txt_P).val(txtP * 0.436);
            break;
        default:
            $(Txt_P).val(txtP * 2.291);
            break;
    }
}

function ddlK_Click() {
    var forma = parseFloat($($(DDL_K2O) & ' option:selected').val());
    var txtK = parseFloat($(Txt_K).val());
    switch (forma) {
        case 1:
            $(Txt_K).val(txtK * 0.83);
            break;
        default:
            $(Txt_K).val(txtK * 1.205);
            break;
    }
}
