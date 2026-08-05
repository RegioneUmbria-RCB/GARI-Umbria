
		// ----------------------------------------------
		// 
		// Creato il 2017-07-05
		// ----------------------------------------------


/**
 * Click a Info della riga della tabella Progetti => Riempio le strutture del Tab 3
 * @param {any} obj
 */
function InfoProgetti(obj) {

    progetto_cod = $(obj).attr('chiave');
    caricaProgetto(progetto_cod);
}

function caricaProgetto(progetto_cod, deferred) {
    try{
        mode_op = "modifica";
        var i;
        var deferreds = new Array();

        for (i = 0; i < watableImpianti.getData().rows.length; i++) {
            var q = watableImpianti.getData().rows[i];
            if (q.Tool == progetto_cod) {
                var codice = q.Codice;
                //$(' .selectpicker')
                var inizio = new Date(q['Data Inizio']);
                var fine = new Date(q['Data Fine']);

                $('#Txt_ValiditaInizio_Distinta').val(inizio.format("dd/MM/yyyy"));
                $('#Txt_ValiditaFine_Distinta').val(fine.format("dd/MM/yyyy"));

                //dati principali
                deferreds.push(riempiInfoImpianto(progetto_cod));
                //var d1 = riempiInfoImpianto(progetto_cod);

                //codici distinsta
                deferreds.push(riempiTabCodici(progetto_cod));
                //var d2 = riempiTabCodici(progetto_cod);

                //particelle distinta
                deferreds.push(riempiTabParticelle(progetto_cod));
                //var d3 = riempiTabParticelle(progetto_cod);


                break;
            }
        }

        if (deferreds.length > 0) {
            $.when.apply($, deferreds).then(function () {
                deferred.resolve();
            }).fail(function () {
                deferred.reject();
            });
        } else {
            deferred.resolve();
        }
    } catch (err) {
        deferred.reject();
    }

}


function ModificaCodice(obj) {

    var progetto_cod = $(obj).attr('chiave');

    var i;

    for (i = 0; i < watableCodici.getData().rows.length; i++) {
        var q = watableCodici.getData().rows[i];
        if (q.Tool == progetto_cod) {
            var codice = q.Codice;
            //$(' .selectpicker' 
            //var i = 0;

            $("#CmbCodice option").each(function () {

                if (codice == $(this).text()) {
                    $(this).prop('selected', true);
                    $('.selectpicker').selectpicker('refresh');
                }

                i = i + 1;

            });


            var valore = q.Valore;
            $('#TxtCodiceValore').val(valore);
            $('.selectpicker').selectpicker('refresh');

            //                    var Dal = q.Dal;
            //                    if (Dal == "...") 
            //                        Dal= "";
            //                    $('#<=TxtValiditaInizioCodice.ClientID %>').val(Dal);

            //                    var Al= q.Al;
            //                    if (Al == "...") 
            //                        Al= "";
            //                    $('#<=TxtValiditaFineCodice.ClientID %>').val(Al);

            break;
        }
    }

    //                 EliminaCodice(obj);                

}


function ValidaxDistinta() {

    var flag = controlla_distinta();

    //alert(flag);

    if (flag) {
        $('#ImgBtn_SalvaDistinta').click();
    }

}


//var btn_salva_clicked = false

function ValidaxSubmit() {

    //btn_salva_clicked = true;
    var flag = controlla_form();

    if (flag) {


        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Regolamento',
        //    data: "{valore:'" + $('#Cmb_Regolamento').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});


        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Specie',
        //    data: "{valore:'" + $('#Cmb_Specie').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Cultivar',
        //    data: "{valore:'" + $('#Cmb_Cultivar').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Finalita',
        //    data: "{valore:'" + $('#Cmb_Finalita').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_TipologiaVarietale',
        //    data: "{valore:'" + $('#Cmb_TipologiaVarietale').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Copertura',
        //    data: "{valore:'" + $('#Cmb_Copertura').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_FormaAllevamento',
        //    data: "{valore:'" + $('#Cmb_FormaAllevamento').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Portinnesto',
        //    data: "{valore:'" + $('#Cmb_Portinnesto').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_DettaglioVarietaPersonalizzato',
        //    data: "{valore:'" + $('#Cmb_DettaglioVarietaPersonalizzato').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_CodiciTerreno',
        //    data: "{valore:'" + $('#Cmb_CodiciTerreno').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_ImpIrrigazione',
        //    data: "{valore:'" + $('#Cmb_ImpIrrigazione').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_ProvenienzaSeme',
        //    data: "{valore:'" + $('#Cmb_ProvenienzaSeme').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_SeminaTrapianto',
        //    data: "{valore:'" + $('#Cmb_SeminaTrapianto').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});


        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Stato',
        //    data: "{valore:'" + $('#Cmb_Stato').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_Disciplinare',
        //    data: "{valore:'" + $('#Cmb_Disciplinare').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_RegolamentoConc',
        //    data: "{valore:'" + $('#Cmb_RegolamentoConc').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_OrganismoReferente',
        //    data: "{valore:'" + $('#Cmb_OrganismoReferente').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_MagazzinoConferimento',
        //    data: "{valore:'" + $('#Cmb_MagazzinoConferimento').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impianto_Edit.aspx/Salva_Cmb_CapitolatoPrivato',
        //    data: "{valore:'" + $('#Cmb_CapitolatoPrivato').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: false,
        //    success: function (r) {

        //    }
        //});


        $('#ImgBtn_SalvaTutto').click();

    }

}


function controlla_distinta() {
    
    var var_contr = new customValidator();
    

    // Azzero tutte le label custom_val
    $(".scheda_distinta").find('input .custom_val').each(function (i, obj) {
        $(this).prev().closest("input").css('border', '1px solid #428BCA');
        //$(this).closest("input").css('border', '1px solid #428BCA');
        $(this).remove();
    });


    Controlla_required("Txt_ValiditaInizio_Distinta", var_contr, 3)
    

    if ($('#Txt_ValiditaFine_Distinta').val() == "") {
        $('#Txt_ValiditaFine_Distinta').parent().append('<label id="Txt_ValiditaFine_Distinta-error" class="custom_val error" for="Txt_ValiditaFine_Distinta">Il campo deve essere compilato</label>');
        $('#Txt_ValiditaFine_Distinta').parent().children(".required").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv3++;
    }

    // Controllo se le date sono corrette temporalmente
    var TxtValiditaInizio = $('#Txt_ValiditaInizio_Distinta').val().split("/");
    var TxtValiditaFine = $('#Txt_ValiditaFine_Distinta').val().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    if (ini > fin) {
        $('#Txt_ValiditaInizio_Distinta').parent().append('<label id="Txt_ValiditaInizio_Distinta-error" class="custom_val error" for="Txt_ValiditaInizio_Distinta">La data di inizio non può essere maggiore di quella di fine</label>');
        $('#Txt_ValiditaInizio_Distinta').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv3++;
    }


    // Controllo se le date della Distinta sono dentro le date dell'Impianto
    var TxtValiditaInizio_Imp = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine_Imp = $('#TxtValiditaFine').val().split("/");

    ini_imp = new Date(TxtValiditaInizio_Imp[2], TxtValiditaInizio_Imp[1] - 1, TxtValiditaInizio_Imp[0]);
    fin_imp = new Date(TxtValiditaFine_Imp[2], TxtValiditaFine_Imp[1] - 1, TxtValiditaFine_Imp[0]);

    if ((ini < ini_imp) || (ini > fin_imp)) {
        $('#Txt_ValiditaInizio_Distinta').parent().append('<label id="Txt_ValiditaInizio_Distinta-error" class="custom_val error" for="Txt_ValiditaInizio_Distinta">La data di inizio della Distinta deve essere compresa nel periodo di validità dell Impianto</label>');
        $('#Txt_ValiditaInizio_Distinta').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv3++;
    }

    if (fin < ini_imp || fin > fin_imp) {
        $('#Txt_ValiditaFine_Distinta').parent().append('<label id="Txt_ValiditaFine_Distinta-error" class="custom_val error" for="Txt_ValiditaFine_Distinta">La data di fine della Distinta deve essere compresa nel periodo di validità dell Impianto</label>');
        $('#Txt_ValiditaFine_Distinta').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv3++;
    }

    ///////////////////////////////

    // Controllo se Azoto contiene valori non numerici
    Controlla_numero('TxtN', var_contr, 3);

    //if ($('#TxtN').val().match(/[a-z]/i)) {
    //    $('#TxtN').parent().append('<label id="TxtN-error" class="custom_val error" for="TxtN">Il campo può contenere solo numeri</label>');
    //    $('#TxtN').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    // Controllo se Fosforo contiene valori non numerici
    Controlla_numero('TxtP2O5', var_contr, 3);

    //if ($('#TxtP2O5').val().match(/[a-z]/i)) {
    //    $('#TxtP2O5').parent().append('<label id="TxtP2O5-error" class="custom_val error" for="TxtP2O5">Il campo può contenere solo numeri</label>');
    //    $('#TxtP2O5').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    // Controllo se Potassio contiene valori non numerici
    Controlla_numero('TxtK2O', var_contr, 3);


    //if ($('#TxtK2O').val().match(/[a-z]/i)) {
    //    $('#TxtK2O').parent().append('<label id="TxtK2O-error" class="custom_val error" for="TxtK2O">Il campo può contenere solo numeri</label>');
    //    $('#TxtK2O').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    // Controllo se Magnesio contiene valori non numerici
    Controlla_numero('TxtMgO', var_contr, 3);

    //if ($('#TxtMgO').val().match(/[a-z]/i)) {
    //    $('#TxtMgO').parent().append('<label id="TxtMgO-error" class="custom_val error" for="TxtMgO">Il campo può contenere solo numeri</label>');
    //    $('#TxtMgO').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    // Controllo se Resa prevista contiene valori non numerici
    Controlla_numero('Txt_ResaPrevista', var_contr, 3);

    //if ($('#Txt_ResaPrevista').val().match(/[a-z]/i)) {
    //    $('#Txt_ResaPrevista').parent().append('<label id="Txt_ResaPrevista-error" class="custom_val error" for="Txt_ResaPrevista">Il campo può contenere solo numeri</label>');
    //    $('#Txt_ResaPrevista').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    // Controllo se Resa prevista contiene valori non numerici
    Controlla_numero('TxtPianteHa', var_contr, 3);

    //if ($('#TxtPianteHa').val().match(/[a-z]/i)) {
    //    $('#TxtPianteHa').parent().append('<label id="TxtPianteHa-error" class="custom_val error" for="TxtPianteHa">Il campo può contenere solo numeri</label>');
    //    $('#TxtPianteHa').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    ///////////////////////////////////////

    var mode;
    if (mode_op == "scrittura")
        mode = 1;
    else if (mode_op == "modifica")
        mode = 2;

    controlla_Date_Distinta_Su_Storico($('#Txt_ValiditaInizio_Distinta').val(), $('#Txt_ValiditaFine_Distinta').val(), mode, progetto_cod, var_contr);
    
    // Controllo se necessita oraginsmo referente
    esiste_Obbligo_SalvataggioOrganismoReferente(var_contr);
    

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


function controlla_form() {

    var var_contr = new customValidator();

    //v.resetForm();

    // Azzero tutte le label custom_val
    $(".custom_val").each(function () {
        $(this).prev().closest("input").css('border', '1px solid #428BCA');
        //$(this).closest("input").css('border', '1px solid #428BCA');
        $(this).remove();
    });
    //$(".custom_val").each(function (i, obj) {
    //    $(this).css('border', '1px solid #ccc');
    //    $(this).parent().children().css('border-color', '#ccc');
    //    $(this).remove();
    //});

    Controlla_required("TxtValiditaInizio", var_contr, 1);
    
    if ($('#TxtValiditaFine').val() == "") {
        $('#TxtValiditaFine').val('31/12/2100');
        //$('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">Il campo deve essere compilato</label>');
        //$('#TxtValiditaFine').parent().children(".required").css('border', '1px solid #D41E1A');
        //flag = false;
        //n_inv++;
    }


    // Controllo se le date sono corrette
    var TxtValiditaInizio = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine = $('#TxtValiditaFine').val().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    if (ini > fin) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">La data di inizio non può essere maggiore di quella di fine</label>');
        $('#TxtValiditaInizio').closest("input").css('border', '1px solid #D41E1A');
        var_contr.flag = false;
        var_contr.n_inv++;
    }


    //Controllo se le date sono ok rispetto a quello del CENTRO e APPEZZAMENTO
    //#CoiPiedi la seguente parte è fatta con un allucione bello grande
    var centro_dal
    var centro_al
    var app_dal
    var app_al

    if ($('#lbl_centro_data_inizio').text() == "")
        centro_dal = new Date("01/01/1900");
    else {
        values = $('#lbl_centro_data_inizio').text().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        centro_dal = new Date(final);
    }

    if ($('#lbl_centro_data_fine').text() == "")
        centro_al = new Date("12/31/2100");
    else {
        values = $('#lbl_centro_data_fine').text().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        centro_al = new Date(final);
    }


    if ($('#lbl_appezza_data_inizio').text() == "")
        app_dal = new Date("01/01/1900");
    else {
        values = $('#lbl_appezza_data_inizio').text().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        app_dal = new Date(final);
    }

    if ($('#lbl_appezza_data_fine').text() == "")
        app_al = new Date("12/31/2100");
    else {
        values = $('#lbl_appezza_data_fine').text().split('/');
        var final = values[1] + "/" + values[0] + "/" + values[2];
        app_al = new Date(final);
    }



    if ((ini < centro_dal) || (ini > centro_al)) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">La data deve essere compresa nel periodo attivo del Centro Aziendale</label>');
        $('#TxtValiditaInizio').parent().children(".required").css('border', '1px solid #D41E1A');

        var_contr.flag = false;
        var_contr.n_inv++;
    }

    if ((fin < centro_dal) || (fin > centro_al)) {
        $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">La data deve essere compresa nel periodo attivo del Centro Aziendale</label>');
        $('#TxtValiditaFine').parent().children(".required").css('border', '1px solid #D41E1A');

        var_contr.flag = false;
        var_contr.n_inv++;
    }

    if ((ini < app_dal) || (ini > app_al)) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">La data deve essere compresa nel periodo attivo dell\'Appezzamento</label>');
        $('#TxtValiditaInizio').parent().children(".required").css('border', '1px solid #D41E1A');

        var_contr.flag = false;
        var_contr.n_inv++;
    }

    if ((fin < app_dal) || (fin > app_al)) {
        $('#TxtValiditaFine').parent().append('<label id="TxtValiditaFine-error" class="custom_val error" for="TxtValiditaFine">La data deve essere compresa nel periodo attivo dell\'Appezzamento</label>');
        $('#TxtValiditaFine').parent().children(".required").css('border', '1px solid #D41E1A');

        var_contr.flag = false;
        var_contr.n_inv++;
    }

    ///////////////////////////

    // Controllo se esistono altri impianti sull'appezzamento
    if (($('#TxtValiditaInizio').val() !== "") && ($('#TxtValiditaFine').val() !== "")) {

        esistonoAltriImpiantiSuAppezzamento($('#TxtValiditaInizio').val(), $('#TxtValiditaFine').val(), var_contr);
    }
    ///////////////////////////

    if (Controlla_required("TxtSuperficie", var_contr, 1)) {
        Controlla_numero("TxtSuperficie", var_contr, 1);
    }

    // Controllo se il Terreno Nudo è stato selezionato
    if ($('#ChkTerrenoNudo').is(':checked')) {

        Controlla_required("Cmb_CodiciTerreno", var_contr, 1);

        //if ($('#Cmb_CodiciTerreno').val() === "") {
        //    $('#Cmb_CodiciTerreno').parent().append('<label id="Cmb_CodiciTerreno-error" class="custom_val error" for="Cmb_CodiciTerreno">Il campo deve essere compilato</label>');
        //    $('#Cmb_CodiciTerreno').parent().children(".required").css('border', '1px solid #D41E1A');
        //    flag = false;
        //    n_inv++;
        //}
    }
    else {

        Controlla_required("Cmb_Specie", var_contr, 1);

        Controlla_required("Cmb_Finalita", var_contr, 1);

        Controlla_required("Cmb_Cultivar", var_contr, 1);

        //if ($('#Cmb_Specie').val() === "") {
        //    $('#Cmb_Specie').parent().append('<label id="Cmb_Specie-error" class="custom_val error" for="Cmb_Specie">Il campo deve essere compilato</label>');
        //    $('#Cmb_Specie').parent().children(".required").css('border', '1px solid #D41E1A');
        //    flag = false;
        //    n_inv++;
        //}

        //if ($('#Cmb_Finalita').val() === "") {
        //    $('#Cmb_Finalita').parent().append('<label id="Cmb_Finalita-error" class="custom_val error" for="Cmb_Finalita">Il campo deve essere compilato</label>');
        //    $('#Cmb_Finalita').parent().children(".required").css('border', '1px solid #D41E1A');
        //    flag = false;
        //    n_inv++;
        //}

        //if ($('#Cmb_Cultivar').val() === "") {
        //    $('#Cmb_Cultivar').parent().append('<label id="Cmb_Cultivar-error" class="custom_val error" for="Cmb_Cultivar">Il campo deve essere compilato</label>');
        //    $('#Cmb_Cultivar').parent().children(".required").css('border', '1px solid #D41E1A');
        //    flag = false;
        //    n_inv++;
        //}

    }




    /*
    Devo inserire gli altri controlli che ho tolto da webservice
    */

    Controlla_numero('TxtSuperficie2', var_contr, 2);
    Controlla_numero('Txt_DistanzaSuFila_M', var_contr, 2);
    Controlla_numero('Txt_DistanzaTraFila_M', var_contr, 2);
    Controlla_numero('Txt_Interbina', var_contr, 2);
    Controlla_numero('Txt_Germinabilita', var_contr, 2);
    Controlla_numero('TxtPianteHa', var_contr, 2);
    Controlla_numero('TxtPianteImpianto', var_contr, 2);
    Controlla_numero('Txt_DistanzaSuFila_F', var_contr, 2);
    Controlla_numero('Txt_DistanzaTraFila_F', var_contr, 2);

    if (var_contr.flag) {
        //v.valid() && 

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


//to CHECK serve?????
function ConfermaDelete(streelemento, xChiave) {

    bootbox.dialog({
        message: streelemento,
        title: "Sei sicuro di voler eliminare questo elemento?",
        buttons: {
            annulla: {
                label: "Annulla",
                className: "btn-primary",
                callback: function () {
                }
            },
            Ok: {
                label: "Si sono sicuro",
                className: "btn-danger",
                callback: function () {
                    $.ajax({
                        type: 'POST',
                        url: './Impianto_Edit.aspx/DeleteDistinta',
                        data: "{xChiave:'" + xChiave + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        //dataType: 'json', async: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            alert(r.d);
                            AggiornaDati();
                        }
                    });
                }
            }
        }
    });
}


var WaitFrame;
WaitFrame = WaitFrame || (function () {
    var attivo = false;
    var $pleaseWaitDiv = $(
        '<div id="pleaseWaitDialog" class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-to: 15%; overflow-y: visible;">' +
        '<div class="modal-dialog modal-m">' +
        '<div class="modal-header" style="background-color: #fff"><h3></h3></div>' +
        '<div class="modal-body" style="background-color: #fff">' +
        '<div class="progress progress-striped active" style="margin-bottom:0;"><div class="progress-bar" style="width: 100%;"></div></div>' +
        '</div></div></div>');

    return {
        show: function (messaggio) {
            //imposto il messaggio da mostrare in base al parametro (default="Attendere...")
            if (typeof (messaggio) === 'undefined') {
                messaggio = _MSG_ELABORAZIONE;
            }
            $pleaseWaitDiv.find("h3").text(messaggio);

            //se il wait è già visibile esco...(e non l'aggiungo di nuovo)
            if ($('#pleaseWaitDialog').is(":visible"))
                return true;

            //la 2 volta che viene richiamata, mostro il wait
            if (attivo === true) {
                $('#pleaseWaitDialog').modal('show');
                return true;
            }

            //la 1 volta che viene richiamata, aggiungo al form la finestra modale del wait
            attivo = true;
            $pleaseWaitDiv.modal();
        },
        hide: function () {
            $pleaseWaitDiv.modal('hide');
        },
        debug: function () {
            return attivo;
        }

    };
})();


var mode_op;
var progetto_cod;

//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');

    $('#' + tabella + ' table thead tr').find('th:nth-child(2)').attr('data-toggle', 'true');

    jQuery.each(elem_hide, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
    });


    setTimeout(function () {
        $('#' + tabella + ' table').footable({ breakpoints: {
            phone: 480,
            tablet: 700
        }
        });
    }, 100);


}


var watableImpianti;
var watableCodici;
var watableParticelle;


function AggiornaTabParticelle(d) {

    AgroWA_Table_sistemaDati(d);

    var elem_hide = new Array(4, 5);

    $('#tabParticelle').html('');
    watableParticelle = $("#tabParticelle").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: true,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
            applicaFooTable('tabParticelle', elem_hide);
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

    InitWaTable("#tabParticelle", watableParticelle);
}


function AggiornaTabImpianti(d, deferred) {
    try{
        AgroWA_Table_sistemaDati(d);

        var elem_hide = new Array(4, 5);

        $('#tabImpianti').html('');
        watableImpianti = $("#tabImpianti").WATable({
            pageSize: 50,
            pageSizes: [50],
            filter: true,
            preFill: false,
            checkboxes: false,
            tableCreated: function (data) {

                applicaFooTable('tabImpianti', elem_hide);

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

        InitWaTable("#tabImpianti", watableImpianti);
        deferred.resolve();
    } catch (err) {
        deferred.reject();
    }
}


function AggiornaTabCodici(d, deferred) {

    try {

        AgroWA_Table_sistemaDati(d);

        $('#tabCodici').html('');
        watableCodici = $("#tabCodici").WATable({
            pageSize: 50,
            pageSizes: [50],
            filter: false,
            preFill: false,
            checkboxes: false,
            tableCreated: function (data) {
                //                                    coloraMovimenti();
            }
                    , pageChanged: function (data) {
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

        deferred.resolve();
    } catch (err) {
        deferred.reject();
    }
}


function AggiornaTabParticelle(d, deferred) {
    try {

        AgroWA_Table_sistemaDati(d);

        $('#tabParticelle').html('');
        watableParticelle = $("#tabParticelle").WATable({
            pageSize: 50,
            pageSizes: [50],
            filter: false,
            preFill: false,
            checkboxes: false,
            tableCreated: function (data) {
                //   coloraMovimenti();
            }
                    , pageChanged: function (data) {
                        //                                    coloraMovimenti();
                    }
                , types: {
                    string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
                    date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
                    number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
                    bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
                }
        }).data('WATable').setData(d);

        InitWaTable("#tabParticelle", watableParticelle);

        deferred.resolve();
    } catch (err) {
        deferred.reject();
    }
}


function inizializzaKendoDropDown(idDiv, filter) {
    if (filter) {
        $("#" + idDiv).kendoDropDownList({
            filter: "contains",
            change: function (e) {
                alert("change!");
            }
        });
    } else {
        $("#" + idDiv).kendoDropDownList({});
    }
    let kddl = $("#" + idDiv).data("kendoDropDownList");
    return kddl;
}