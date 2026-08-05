/**
 *  Creazione 2017-04-20
 *  solo Ready 
 */

var appezzamentoEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Appezzamento_Edit.aspx.resx",
];

$(document).ready(function () {

    $(txtSuperficieJQuerySelector).on("change", function () {
        TestSuperficieCfrGis();
    });

    
    $("#LblSuperficie_Con_Catasto").on("change", function () {
        TestSuperficieCfrGis();
    });

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            appezzamentoEditResx.push(readResxFile(resxSinglePath, "Appezzamento_Edit_jQueryDocReady.js"));
        });
    }

    //tooltip
    $('a[data-toggle="tooltip"]').tooltip({
        animated: 'fade',
        placement: 'top',
        container: 'body'
    });

    $('#aspnetForm').change(function () {
        controlla_form();
    });

    $('.datepicker').on('changeDate', function (ev) {
        $(this).datepicker('hide');
        controlla_form();
    });

    // Gestione visibilità TAB Biologico
    if ($('#Txt_NumeroAppBio').val() != "" && $('#Txt_ConfiniRischio').val() != "") {
        $('#Opt_Biologico').attr('checked', true);
        $('.tab_biologico').show();
    }

    var dataInizio = $("#TxtValiditaInizio").val();
    var dataFine = $("#TxtValiditaFine").val();

    $('#aspnetForm').change(function () {
        controlla_form();
    });


    var initCodici;
    var initUtilizzo;
    var initParticelle;

    //carico eventualmente la tabella di Codici       
    caricaCodici();


    //carico eventualmente la tabella di Utilizzo Terreno
    caricaUtilizzi();

    caricaIndirizzi(initIndirizzi);

    //Lasciare prima su visibilità delle tab per poter decidere se forzare con catasto
    switch ($('#hidden_tipoCampo').val()) {
        case "1": //campo squadro
            $('#div_campo_ass').hide();
            //$('#RBL_Catasto_Con').attr('checked', 'checked');
            break;
        case "2": //campo aggregatore
            $('#div_campo_ass').hide();
            $('#div_lblCampo').hide();
            break;
        default:
            break;
    }

    // Init visibilità tab...
    if ($('#RBL_Catasto_Con').is(':checked')) {
        $('.tab_dati_catastali').show();
        $('#div_superficie').hide();
        $('#div_Superficie_Con').show();
        //$('#div_campo_ass').hide();
        leggiParticelle();
        kendoRefresh("#kendo_Particelle");
    }
    else {
        $('.tab_dati_catastali').hide();
        $('#div_superficie').show();
        $('#div_Superficie_Con').hide();
        //$('#div_campo_ass').show();
    }

    var convenzionaleChk = $('#Opt_Convenzionale').is(':checked');
    var conversioneChk = $('#Opt_InConversione').is(':checked');
    var biologicoChk = $('#Opt_Biologico').is(':checked');

    if (!convenzionaleChk && !conversioneChk && !biologicoChk) {
        $('#Opt_Convenzionale').prop('checked', true);
    }

    if ($('#Opt_Convenzionale').is(':checked'))
        $('.tab_biologico').hide();
    else
        $('.tab_biologico').show();


    $(".GestioneCatastale").change(function () {
        switch ($(this).children('input').attr('value')) {

            case 'RBL_Catasto_Con':
                $('.tab_dati_catastali').show();
                $('#div_Superficie_Con').show();
                $('#div_superficie').hide();
                leggiParticelle();
                kendoRefresh("#kendo_Particelle");
                //$('#div_campo_ass').hide();
                break;

            case 'RBL_Catasto_Senza':
                $('.tab_dati_catastali').hide();
                $('#div_superficie').show();
                $('#div_Superficie_Con').hide();
                //$('#div_campo_ass').show();
                break;

        }
    });


    $(".UtilizzoTerreno").change(function () {
        switch ($(this).children('input').attr('value')) {

            case 'Opt_Convenzionale':
                $('.tab_biologico').hide();
                break;

            case 'Opt_InConversione':
                $('.tab_biologico').show();
                break;

            case 'Opt_Biologico':
                $('.tab_biologico').show();
                break;

        }
    });

    var chk_macrousi = 0
    var chk_utilizzi = 0
    var chk_varieta = 0

    $('#Chk_Varieta').attr("disabled", true);

    // Controllo quando i check Dati catastali cambiano
    $("span[name='chk_datiCatastali[]'] input").change(function () {

        //... controllo quali check sono abilitati
        if ($("#Chk_Macrousi").is(':checked')) {
            chk_macrousi = 1;
        }
        else {
            chk_macrousi = 0;
        }

        if ($("#Chk_Utilizzi").is(':checked')) {

            $('#Chk_Varieta').attr("disabled", false);
            chk_utilizzi = 1;
        }
        else {

            $('#Chk_Varieta').attr("disabled", true);
            chk_utilizzi = 0;
        }

        if ($("#Chk_Varieta").is(':checked')) {
            chk_varieta = 1;
        }
        else {
            chk_varieta = 0;
        }


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

        leggiParticelle();
        kendoRefresh("#kendo_Particelle");
    });



    // Controllo quando le select Dati catastali cambiano
    $("select.cmb_datiCatastali").change(function () {

        //... controllo quali check sono abilitati
        if ($("#Chk_Macrousi").is(':checked')) {

            //$('#Cmb_Macrousi.ClientID').attr("disabled", false);
            chk_macrousi = 1;
            //val_macrousi = $("#Cmb_Macrousi.ClientID").val();
        }
        else {

            //$('#Cmb_Macrousi.ClientID').attr("disabled", true);
            chk_macrousi = 0;
            //val_macrousi = 0;
        }

        if ($("#Chk_Utilizzi").is(':checked')) {

            $('#Cmb_Utilizzo1').attr("disabled", false);
            $('#Chk_Varieta').attr("disabled", false);
            chk_utilizzi = 1;
            //val_utilizzi = $("#Cmb_Utilizzo1").val();
        }
        else {

            $('#Cmb_Utilizzo1').attr("disabled", true);
            $('#Chk_Varieta').attr("disabled", true);
            chk_utilizzi = 0;
            val_utilizzi = 0;
        }

        if ($("#Chk_Varieta").is(':checked')) {
            chk_varieta = 1;
        }
        else {
            chk_varieta = 0;
        }

    });



    // Gestione click del bottone Aggiungi in Rubrica
    $('#btn_Aggiungi_Codice').click(function (e) {

        var codice = $('#div_CmbCodice > div > button').text();
        var valore = $('#TxtCodiceValore2').val();
        var DataInizio = "";
        var DataFine = "";

        // Controllo se è stato immesso del testo nel input del Cerca
        if ((codice != "") && (valore != "")) {
            codice = codice.slice(0, -1);

            DataInizio = $('#TxtValiditaInizioCodice').val();
            DataFine = $('#TxtValiditaFineCodice').val();

            $("#CmbCodice option").each(function () {
                if (this.text == codice) {
                    //alert(this.value);
                    codice_id = this.value;
                }
            });

            $.ajax({
                type: 'POST',
                url: 'Appezzamento_Edit.aspx/Aggiungi_Codice',
                data: "{codice:'" + codice + "', valore:'" + valore + "', codice_id:'" + codice_id + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
                    else
                        alert(r.d.Errore);
                }
            });
        }
    });


    $('#txtValiditaInizio').datepicker({
        dateFormat: 'dd/mm/yy',
        disabled: false,
        changeMonth: true,
        changeYear: true
    });
    $('#txtValiditaFine').datepicker({
        dateFormat: 'dd/mm/yy',
        disabled: false,
        changeMonth: true,
        changeYear: true
    });

    $('#Txt_DataFineImpiego').datepicker({
        dateFormat: 'dd/mm/yy',
        disabled: false,
        changeMonth: true,
        changeYear: true
    });

    //		    $('.ChkSelezionaParticella').click(function () {
    //		        AggiornaSup($(this), $(this).find('input'));
    //		    });


    //		    $('.SupIntersezione').change(function () {
    //                    var SupTotale = parseFloat(0.0);
    //                    //seleziono tutto
    //                    $('.ChkSelezionaParticella').each(function () {
    //                        //alert($(this).children('input').is(':checked'));

    //                        if ($(this).children('input').is(':checked') == true) {
    //                            var Sup = $(this).children('input').parent().parent().parent().find('.SupIntersezione').val().replace(',', '.');
    //                            SupTotale = parseFloat(SupTotale) + parseFloat(Sup);
    //                        }

    //                    });

    //                    $('.SupTotale').val(String(roundNumber(SupTotale, 4)).replace(".", ","));
    //                });


    $('#TxtValiditaInizio').change(function () {
        var InizioCentro = $('#InizioCentro').val();
        var InizioAppezzamento = $('#TxtValiditaInizio').val();
        var FineAppezzamento = $('#TxtValiditaFine').val();

        //verifico che la data inizio appezzamento non sia successiva a quella della fine
        if ((controllo_data(InizioAppezzamento) == true) && (controllo_data(FineAppezzamento) == true)) {
            if (confronta_data(FineAppezzamento, InizioAppezzamento) == true) {
                $('#TxtValiditaInizio').val(FineAppezzamento);
                alert(TraduzioneMultiResx(appezzamentoEditResx, "InizioAppezzamentoSuccessivoCessazione", "L'inizio dell'appezzamento non può seguire la cessazione di se stesso")
                    + " (" + FineAppezzamento + ").");
                return;
            }
        }

        //verifico che la data inizio appezzamento non sia antecedente a quella del centro
        if ((controllo_data(InizioAppezzamento) == true) && (controllo_data(InizioCentro) == true)) {
            if (confronta_data(InizioAppezzamento, InizioCentro) == true) {
                $('#TxtValiditaInizio').val(InizioCentro);
                alert(TraduzioneMultiResx(appezzamentoEditResx, "InizioAppezzamentoPrecedenteCreazioneCentroAziendale",
                    "L'inizio dell'appezzamento non può precedere la creazione del Centro Aziendale") + " (" + InizioCentro + ").");
                return;
            }
        }
    });

    //verifico che la data fine impianto non sia successiva a quella dell'appezzamento
    $('#TxtValiditaFine').change(function () {
        var FineCentro = $('#FineCentro').val();
        var FineAppezzamento = $('#TxtValiditaFine').val();
        var InizioAppezzamento = $('#TxtValiditaInizio').val();

        //verifico che la data fine appezzamento non sia precedente a quella del'inizio
        if ((controllo_data(InizioAppezzamento) == true) && (controllo_data(FineAppezzamento) == true)) {
            if (confronta_data(FineAppezzamento, InizioAppezzamento) == true) {
                $('#TxtValiditaFine').val(InizioAppezzamento);
                alert(TraduzioneMultiResx(appezzamentoEditResx, "FineAppezzamentoPrecedenteCreazione", "La fine dell'appezzamento non può precedere la creazione di se stesso")
                    + " (" + InizioAppezzamento + ").");
                return;
            }
        }

        //verifico che la data fine appezzamento non sia successiva a quella del centro
        if ((controllo_data(FineAppezzamento) == true) && (controllo_data(FineCentro) == true)) {
            if (confronta_data(FineCentro, FineAppezzamento) == true) {
                $('#TxtValiditaFine').val(FineCentro);
                alert(TraduzioneMultiResx(appezzamentoEditResx, "FineAppezzamentoSuccessivaCessazioneCentroAziendale",
                    "La fine dell'appezzamento non può seguire la cessazione del Centro Aziendale") & " (" + FineCentro + ").");
                return;
            }
        }
    });


    //            check_particelle();

    //            nascondi_col_calcoli(14);



    // Gestione click del bottone Aggiungi in utilizzo terreno
    $('#btn_aggiungi_utilizzo').click(function (e) {

        var codice = $('#Cmb_Utilizzo').val();
        var valore = $('#Cmb_Utilizzo').val();

        $("#Cmb_Utilizzo option").each(function () {
            if (this.value == codice) {
                //alert(this.value);
                valore = this.text;
            }
        });

        $.ajax({
            type: 'POST',
            url: 'Appezzamento_Edit.aspx/Aggiungi_Utilizzo',
            data: "{codice_id:'" + codice + "', testo:'" + valore + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: false,
            success: function (r) {
                if (r.d.RispostaOK == true)
                    AggiornaTabUtilizzo(JSON.parse(r.d.RispostaStringa));
                else
                    alert(r.d.Errore);
            }
        });


    });

});