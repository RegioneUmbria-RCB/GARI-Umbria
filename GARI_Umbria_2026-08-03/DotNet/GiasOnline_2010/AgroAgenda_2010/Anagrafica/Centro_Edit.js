
//'  Vanni, 28/05/2014 15:04:00: gestione del log in console...
//        jQuery.logThis = function (text) {
//            if ((window['console'] != undefined)) {
//                console.log(text);
//            }
//        }

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

var btn_salva_clicked = false;
var centroEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Centro_Edit.aspx.resx"
];

function ValidaxSubmit() {

    btn_salva_clicked = true;
    var flag = controlla_form();

    if (flag)
    {
        // Passo i valori selezionati di OTE in session (per il salvataggio)
        var OTE_values = "";

        $("#cblOTE option").each(function () {
            if($(this).is(':selected'))
                OTE_values = OTE_values + "-" + $(this).val();
        });
        OTE_values = OTE_values.substring(1, OTE_values.length);

        $.ajax({
            type: 'POST',
            url: 'Centro_Edit.aspx/Salva_cblOTE',
            data: "{valori:'" + OTE_values + "'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json', async: true,
            success: function (r) {
            }
        });

        $.ajax({
            type: 'POST',
            url: 'Centro_Edit.aspx/Set_Comune',
            data: "{comune:'" + $('#Cmb_Comune').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
            }

        });



        $('#ImgBtn_SalvaTutto').click();
        AggiornaTabRubrica(initRubrica);
    }


}

function controlla_form() {

    var flag = true;
    var n_inv = 0;

    v.resetForm();

    // Azzero tutte le label custom_val
    $( ".custom_val" ).each(function() {
        $( this ).remove();
        $(this).closest( "input" ).css('border','1px solid #ccc');
    });

    // Gestione della selezione della nazione
    if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {

        var cap = $('#Txt_CAP').val();

        if (cap.length < 5) {
            $('#Txt_CAP').parent().append('<label id="Txt_CAP-error" class="custom_val error" for="Txt_CAP">' + TraduzioneMultiResx(centroEditResx, 'ControlloLunghezzaCampoCAP', 'Il campo deve essere di 5 caratteri') + '</label>');
            $('#Txt_CAP').closest("input").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Cmb_Provincia').val() == "") {
            $('#Cmb_Provincia').parent().append('<label id="Cmb_Provincia-error" class="custom_val error" for="Cmb_Provincia">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
            $('#Cmb_Provincia').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#Cmb_Comune').val() == null || $('#Cmb_Comune').val() == "") {
            $('#Cmb_Comune').parent().append('<label id="Cmb_Comune-error" class="custom_val error" for="Cmb_Comune">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
            $('#Cmb_Comune').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }
    }

    if($('#TxtDenominazione').val() =="")
    {   
        $('#TxtDenominazione').parent().append('<label id="TxtDenominazione-error" class="custom_val error" for="TxtDenominazione">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#TxtDenominazione').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;
    }
            
    if($('#Txt_Via').val() =="")
    {   
        $('#Txt_Via').parent().append('<label id="Txt_Via-error" class="custom_val error" for="Txt_Via">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Txt_Via').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;
    }
            
    if($('#Cmb_Tipologia').val() =="")
    {   
        $('#Cmb_Tipologia').parent().append('<label id="Cmb_Tipologia-error" class="custom_val error" for="Cmb_Tipologia">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Cmb_Tipologia').css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    // Controllo se è stato inserito almeno 1 padre
    /*
    if($('#tabGerarchia .watable >tbody >tr').length == 0)
    {   
        $('#tabGerarchia').append('<label id="tabGerarchia-error" class="custom_val error" for="tabGerarchia" style="padding: 5px 0">Deve essere inserito almeno un Padre</label>');
        flag = false;
        n_inv++;
    }
    */

    // Controllo se le date sono corrette
    var TxtValiditaInizio = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine = $('#TxtValiditaFine').val().split("/");
    var TxtBoundInizio = $('#lbl_azienda_data_inizio').text().split("/");
    var TxtBoundFine = $('#lbl_azienda_data_fine').text().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    b_ini = new Date(TxtBoundInizio[2], TxtBoundInizio[1] - 1, TxtBoundInizio[0]);
    b_fin = new Date(TxtBoundFine[2], TxtBoundFine[1] - 1, TxtBoundFine[0]);

    if(ini > fin){

        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + TraduzioneMultiResx(centroEditResx, 'DataInizioNonPuòEssereMaggioreDiDataFine', 'La data di inizio non può essere maggiore di quella di fine') + '</label>');
        $('#TxtValiditaInizio').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;

    }

    if((b_ini > ini) || (fin > b_fin)){
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + TraduzioneMultiResx(centroEditResx, 'LeDateImmesseDevonoEssereCompreseNellaValiditàAziendale', 'Le date immesse devono essere comprese nella validità Aziendale') + '</label>');
        $('#TxtValiditaInizio').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    // Controllo se il cap contiene valori non numerici
    if ($('#Txt_CAP').val().match(/[a-z]/i)) {
        $('#Txt_CAP').parent().append('<label id="Txt_CAP-error" class="custom_val error" for="Txt_CAP">' + TraduzioneMultiResx(centroEditResx, 'IlCampoPuòcontenereSoloNumeri', 'Il campo può contenere solo numeri') + '</label>');
        $('#Txt_CAP').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    ////////////////////////////////////////

    if(v.valid() && flag){
        $('.nav-tabs li.active a .error-tab').remove( ".error-tab" );
        return true
    }
    else
    {
        if (!btn_salva_clicked){
            var err_message = "";
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li.active a').append(err_message);
        }

        btn_salva_clicked = false;

        return false;
    }

}


function InitiImpianto() {
    GetOTE();
}

function duplicaRefresh(obj) {
    obj.selectpicker('refresh').selectpicker('refresh');
}


//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');

            

    if(tabella == 'tabCodici')
    {
        $('#' + tabella + ' table thead tr').find('th:nth-child(5)').attr('data-hide', 'all');
        $('#' + tabella + ' table thead tr').find('th:nth-child(6)').attr('data-hide', 'all');
        //$('#' + tabella + ' table thead tr').find('th:nth-child(2)').attr('data-hide', 'phone, tablet');
    }

            

            
    setTimeout(function () {
        $('#' + tabella + ' table').footable({ breakpoints: {
            phone: 480,
            tablet: 700
        }
        });
    }, 100);


}


function AggiornaTabRubrica(d) {
    $('#tabRubrica').html('');
    watableRubrica = $("#tabRubrica").WATable({
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

    InitWaTable("#tabRubrica", watableRubrica);
}

function AggiornaTabCodici(d) {

    $('#tabCodici').html('');
    watableCodici = $("#tabCodici").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: false,
        preFill: false,
        tableCreated: function (data) { applicaFooTable('tabCodici'); },
        checkboxes: false,
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



function GetOTE() {
    $.ajax({
        type: 'POST',
        url: '../Ajax/x_filtrone.aspx/GetOTE',
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json', async: true,
        success: function (r) {
            var p = JSON.parse(r.d);
            var s = "";
            for (var i = 0; i < p.length; i++) {
                s = s + '<option value="' + p[i].valore + '">' + p[i].stringa + "</option>";
            }
            $('#cblOTE').html(s);
            duplicaRefresh($('#cblOTE'));
            //controllaCaricamento();

            // Carico i valori selezionati
            $.ajax({
                type: 'POST',
                url: 'Centro_Edit.aspx/Riempi_cblOTE',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json', async: true,
                success: function (r) {
                    // var codici = r.d;
                    //alert(r.d);
                    //                                if(codici){
                    //                                   codici = codici.substring(1, codici.length);
                    //                                   var vals=codici.split('-');

                    //                                   jQuery.each( vals, function( i, val ) {
                    //                                        //alert(val);
                    //                                        $("#cblOTE > option").each(function() {
                    //                                            //alert(this.text + ' ' + this.value);
                    //                                            if($(this).val() == val)
                    //                                                $(this).attr('selected','selected');
                    //                                        });
                    //                                    });
                    //                                    duplicaRefresh($('#cblOTE'));

                    //                               }
                    $('#cblOTE').append(r.d);
                    $('#cblOTE').selectpicker('refresh');

                }
            });

        }
    });
}


jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            centroEditResx.push(readResxFile(resxSinglePath, "CentroEdit.js"));
        });
    }

    $('#aspnetForm').change(function () {
        controlla_form();
    });

    $('.datepicker').on('changeDate', function(ev){                 
        $(this).datepicker('hide');
        controlla_form();
    });


    $('.tab_dati_accessori').click(function(){
        //var elem_hide = new Array(4, 5);
        applicaFooTable('tabCodici');
    });


    //carico eventualmente la tabella di Rubrica
    //var initRubrica;
    //var initCodici;
            
    //initRubrica = $("#initRubrica").val();
    if ( initRubrica!="")
    {
        AggiornaTabRubrica(JSON.parse(initRubrica));
    }

    //initCodici = $("#initCodici").val();
    if ( initCodici!="")
    {
        AggiornaTabCodici(JSON.parse(initCodici));
    }

    // Gestione della selezione della nazione
    if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
        $("#div_prov_com").show();
        $("#lbl_frazione").text(TraduzioneMultiResx(centroEditResx, "Frazione", "Frazione"));  
    }
    else{
        $("#div_prov_com").hide();
        $("#lbl_frazione").text(TraduzioneMultiResx(centroEditResx, "Città", "Città"));  
    }

    $('#cmb_Stato').change(function (e) {
        // Gestione della selezione della nazione
        if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
            $("#div_prov_com").show();
            $("#lbl_frazione").text(TraduzioneMultiResx(centroEditResx, "Frazione", "Frazione"));  
        }
        else{
            $("#div_prov_com").hide();
            $("#lbl_frazione").text(TraduzioneMultiResx(centroEditResx, "Città", "Città"));  
        }
    });



    // Colorazione riga selezionata in watable
    $("body").on("click", ".watable tbody tr td:not('.footable-row-detail-cell')", function () {

        $(".watable tbody tr.success").each(function (i) {
            $(this).removeClass("success");
        });

        // soottolineo la riga selezionata...
        $(this).parent().addClass('success');
    });



    // Gestione click del bottone Aggiungi in Codici
    $('#btn_aggiungi_codice').click(function (e) {

        var codice = $('#tipo_codici button').text();
        var valore = $('#TxtCodiceValore').val();
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
                url: 'Centro_Edit.aspx/Aggiungi_Codice_Centro',
                data: "{codice:'" + codice + "', valore:'" + valore + "', codice_id:'" + codice_id + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
                    else
                        alert(r.d.Errore);
                }
            });
        }
    });


    // Gestione click del bottone Aggiungi in Rubrica
    $('.btn_aggiungi_rubrica').click(function (e) {

        var tipo = $('#Cmb_Rubrica_Descrizione').val();
        var valore = $('#TxtRubrica_Numero').val();

        // Controllo se è stato immesso del testo nel input del Cerca
        if ((tipo != "") && (valore != "")) {
            //tipo = tipo.slice(0, -1);

            $.ajax({
                type: 'POST',
                url: 'Centro_Edit.aspx/Aggiungi_Rubrica',
                data: "{tipo:'" + tipo + "', valore:'" + valore + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true)
                        AggiornaTabRubrica(JSON.parse(r.d.RispostaStringa));
                    else
                        alert(r.d.Errore);
                         
                }
            });
        }
    });



    // Quando cambia Provincia, mostro Comuni relativi
    $("#Cmb_Provincia").change(function () {
    
        $('#Cmb_Comune').parent().children().attr("disabled",false);
                
        provincia = $('#Cmb_Provincia').val();

        if ($('#Cmb_Provincia').val() != "") {
            //$('.riga_referente').show();

            $.ajax({
                type: 'POST',
                url: 'Centro_Edit.aspx/Carica_Comuni',
                data: "{provincia:'" + provincia + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_Comune').empty();
                    $('#Cmb_Comune').append(r.d[0]);
                    $('#Txt_ProCodIstat').val(r.d[1]);
                    $('#Txt_ProvinciaSigla').val(provincia);
                            
                    $('.selectpicker').selectpicker('refresh'); 
                }
            });
        }
        else
        {
            $('#Cmb_Comune').empty();
            $('#Cmb_Comune').parent().children().attr("disabled",true);
            $('.selectpicker').selectpicker('refresh'); 
        }

        if ($("#Cmb_Provincia").val() != "") {
            $('.voce_5').hide();
            fl5 = true;
        }
        else {
            $('.voce_5').show();
            fl5 = false;
        }

        nascondi_riepilogo_error();
    });


    // Quando cambia Comune
    $('#Cmb_Comune').change(function () {

        comune = $('#Cmb_Comune').val();
        //$("#<%=ddl_comune.ClientID %> select").val(comune);
        $("#Cmb_Comune > option").each(function () {
            if (this.value == comune) {
                var nome_comune = this.text
                //$(this).attr('selected', 'selected');
                $.ajax({
                    type: 'POST',
                    url: 'Centro_Edit.aspx/Set_Comune',
                    data: "{comune:'" + comune + "', nome_comune:'" + nome_comune + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {

                        var partsArray = $('#Cmb_Comune').val().toString().split('|');
                        var Prov = partsArray[0];
                        var Com = partsArray[1];

                        ottieniCAP(Prov, Com, function (resp) {
                            //$("#Txt_CAP").text(resp.RispostaStringa);
                            $("#Txt_CAP").val(resp.RispostaStringa);
                            $("#Txt_CAP").trigger("change");
                        });
                    }
                });

            }
        });

        var partsArray = comune.split('|');
        $('#Txt_ComCodIstat').val(partsArray[2]);

        if ($("#Cmb_Comune").val() != null && $("#Cmb_Comune").val() != "") {
            $('.voce_6').hide();
            fl6 = true;
        }
        else {
            $('.voce_6').show();
            fl6 = false;
        }

        nascondi_riepilogo_error();

    });


    // Forzo il valore NON definito della select Tipologia a VUOTO
    //$('#Cmb_Tipologia.ClientID%> option:eq(0)').val('');

    //Disabilito il cambio di tab se la validazione è fallita
    //            $('.nav-tabs > li > a').click(function (e) {
    //                if ($('.error-tab').length)
    //                    return false;
    //                else
    //                    return true;
    //            });

    var fl1 = false;
    var fl2 = false;
    var fl3 = false;
    var fl4 = false;
    var fl5 = false;
    var fl6 = false;


    nascondi_riepilogo_error();

    // Gestione Riepilogo Errori (Validazione)
    $("#TxtDenominazione").keyup(function() {
        if($("#TxtDenominazione").val() != "")
        {
            $('.voce_1').hide();
            fl1 = true;
        }
        else{
            $('.voce_1').show();
            fl1 = false;
        }
                
        nascondi_riepilogo_error();
    });

    $("#Cmb_Tipologia").change(function() {
        if($("#Cmb_Tipologia").val() != "")
        {
            $('.voce_2').hide();
            fl2 = true;
        }
        else{
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Via").keyup(function() {
        if($("#Txt_Via").val() != "")
        {
            $('.voce_3').hide();
            fl3 = true;
        }
        else{
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_CAP").keyup(function() {
        if($("#Txt_CAP").val() != "")
        {
            $('.voce_4').hide();
            fl4 = true;
        }
        else{
            $('.voce_4').show();
            fl4 = false;
        }

        nascondi_riepilogo_error();
    });

  
   
    $("#cmb_Stato").change(function () {
        // Gestione della selezione della nazione
        if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
            $('.voce_4').hide();
            fl4 = true;
            $('.voce_5').hide();
            fl5 = true;
            $('.voce_6').hide();
            fl6 = true;



            stato = $('#cmb_Stato').val();

            if ($('#cmb_Stato').val() != "") {
                $('#Cmb_Provincia').parent().children().attr("disabled", false);

                $.ajax({
                    type: 'POST',
                    url: 'Centro_Edit.aspx/Carica_Province',
                    data: "{stato:'" + stato + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        //alert(r.d);
                        $('#Cmb_Provincia').empty();
                        $('#Cmb_Provincia').append(r.d[0]);
                        $('#Txt_ProCodIstat').val(r.d[1]);
                        /*$('#Txt_ProvinciaSigla').val(provincia);*/
                        $('#Cmb_Comune').empty();


                        $('.selectpicker').selectpicker('refresh');
                    }
                });

            }


        } else {
            if ($("#Txt_CAP").val() == "") {
                $('.voce_4').show();
                fl4 = false;
            }
            if ($("#Cmb_Provincia").val() == "") {
                $('.voce_5').show();
                fl5 = false;
            }
            if ($("#Cmb_Comune").val() == null || $("#Cmb_Comune").val() == "") {
                $('.voce_6').show();
                fl6 = false;
            }
        }

        nascondi_riepilogo_error();
    });

    function nascondi_riepilogo_error(){
        if(fl1)
            $('.voce_1').hide();

        if(fl2)
            $('.voce_2').hide();

        if(fl3)
            $('.voce_3').hide();

        if(fl4)
            $('.voce_4').hide();

        if(fl5)
            $('.voce_5').hide();

        if(fl6)
            $('.voce_6').hide();

        if((fl1) && (fl2) && (fl3) && (fl4) && (fl5) && (fl6))
            $('#div_riepilogo_error').hide();
        else
            $('#div_riepilogo_error').show();
    }


    /////////////////////


});

//ingresso jquery
$(function () {


    $('body').on('keyup', '#TxtPiva', function () {
        var testoPiva = $(this).val();
        if (testoPiva.length > 10) {
            $.ajax({
                type: 'POST',
                url: 'impresa_edit.aspx/VerificaPivaPresente',
                data: "{testoPiva:'" + testoPiva + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json', async: true,
                success: function (r) {
                    var p = JSON.parse(r.d);
                    if (p == false) {
                        $('#TxtVerificaPiva').html('');
                        $('#TxtVerificaPiva').removeClass('bg-danger');
                    }
                    else {
                        $('#TxtVerificaPiva').html(TraduzioneMultiResx(centroEditResx, "PartitaIvaGiàInserita", "Partita iva già inserita"));
                        $('#TxtVerificaPiva').addClass('bg-danger');
                    }


                }
            });
        }
        else
            $('#TxtVerificaPiva').html('');

    });


    // Inizializzo le strutture client
    InitiImpianto();


});



function ricercaIndirizzo()
{
    // Inizializzazione validazione
    var flag = true;

    // Azzero tutte le label custom_val
    $( ".custom_val" ).each(function() {
        $( this ).remove();
        $(this).closest( "input" ).css('border','1px solid #ccc');
    });

    // Validazione per Geolocalizzazione
    if($('#Txt_Via').val() =="")
    {   
        $('#Txt_Via').parent().append('<label id="Txt_Via-error" class="custom_val error" for="Txt_Via">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Txt_Via').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
    }

    if($('#Txt_CAP').val() =="")
    {   
        $('#Txt_CAP').parent().append('<label id="Txt_CAP-error" class="custom_val error" for="Txt_CAP">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Txt_CAP').closest( "input" ).css('border','1px solid #D41E1A');
        flag = false;
    }

    if($('#Cmb_Provincia').val() =="")
    {   
        $('#Cmb_Provincia').parent().append('<label id="Cmb_Provincia-error" class="custom_val error" for="Cmb_Provincia">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Cmb_Provincia').parent().children( ".required" ).css('border','1px solid #D41E1A');
        flag = false;
    }


    if($('#Cmb_Comune').val() =="")
    {   
        $('#Cmb_Comune').parent().append('<label id="Cmb_Comune-error" class="custom_val error" for="Cmb_Comune">' + TraduzioneMultiResx(centroEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Cmb_Comune').parent().children( ".required" ).css('border','1px solid #D41E1A');
        flag = false;
    }

    ////////

    if(flag)
    {
        var address = "";
        // Gestione della selezione della nazione
        if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
            address = $('#Txt_Via').val() + "," + $('#Txt_CAP').val() + "," + $('#Cmb_Provincia').val();

            if ($('#Cmb_Comune').val() != "") {


                $("#Cmb_Comune option").each(function () {
                    if (this.value == $('#Cmb_Comune').val()) {
                        //alert(this.text);
                        address = address + "," + this.text.substring(8);
                    }
                });
            }

            //                if($('#<=Txt_Stato').val() !="")
            //                    address = address +","+ $('#<=Txt_Stato.ClientID%>').val();
            if ($('#cmb_Stato').val() != "")
                address = address + "," + $('#cmb_Stato').val();
        } else {
            address = $('#Txt_Via').val() + "," + $('#Txt_Frazione').val() + "," + $('#cmb_Stato').val();
        }


        //*******************************************************
        //SOLO DEBUG
        //*******************************************************
        //let lat = 44.2718913686223
        //let lng = 11.8310898

        //$('#Txt_Latitude').val(lat.toLocaleString(undefined, { minimumFractionDigits: 9 }));
        //$('#Txt_Longitude').val(lng.toLocaleString(undefined, { minimumFractionDigits: 9 }));

        //$.ajax({
        //    type: 'POST',
        //    url: 'Centro_Edit.aspx/CalcolaXYdaLatLong_webservice',
        //    data: "{Latitudine:'" + lat + "', Longitudine:'" + lng + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json', async: true,
        //    success: function (r) {
        //        //$('#Txt_CoordinataX').val(r.d[0]);
        //        //$('#Txt_CoordinataY').val(r.d[1]);
        //    }
        //});
        //*******************************************************

        var geocoder = new google.maps.Geocoder;
        geocoder.geocode({
            'address': address
        }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var l =  results[0].geometry.location;
                if(l.lat() != "" && l.lng() != ""){
                    //console.log(l.H + "   " + l.L);

                    //GABRIELE
                    $('#Txt_Latitude').val(l.lat().toLocaleString(undefined, { minimumFractionDigits: 9 }));
                    $('#Txt_Longitude').val(l.lng().toLocaleString(undefined, { minimumFractionDigits: 9 }));

                    $.ajax({
                        type: 'POST',
                        url: 'Centro_Edit.aspx/CalcolaXYdaLatLong_webservice',
                        data: "{Latitudine:'" + l.lat() + "', Longitudine:'" + l.lng() + "'}",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json', async: true,
                        success: function (r) {
                            //$('#Txt_CoordinataX').val(r.d[0]);
                            //$('#Txt_CoordinataY').val(r.d[1]);
                        }
                    });

                }
                else
                    alert(TraduzioneMultiResx(centroEditResx, 'GeolocalizzazioneNonAvvenuta', 'Geolocalizzazione non avvenuta'));
                
            } else {
                alert("Geocode was not successful for the following reason: " + status);
            }
        });
    }
        
}

function EliminaCodice(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Centro_Edit.aspx/Rimuovi_Codice_Centro',
        data: "{Id_Cod:'" + Id_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function ModificaCodice(obj) {

    var Id_Cod = $(obj).attr('chiave');

    var i;

    for (i = 0; i < watableCodici.getData().rows.length; i++) {
        var q = watableCodici.getData().rows[i];
        if (q.Tool == Id_Cod) {
            var codice = q.Codice;
            //$(' .selectpicker')


            var valore = q.Valore;
            $('#TxtCodiceValore').val(valore);

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


function EliminaRubrica(obj) {
    var tipo = $(obj).attr('chiave');
    console.log("TIPO:" + tipo);
    $.ajax({
        type: 'POST',
        url: 'Centro_Edit.aspx/EliminaRubrica',
        data: "{ tipo:'" + tipo + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true)
                AggiornaTabRubrica(JSON.parse(r.d.RispostaStringa));
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function ModificaRubrica(obj) {

    var tipo = $(obj).attr('chiave');

    var i;

    for (i = 0; i < watableRubrica.getData().rows.length; i++) {
        var q = watableRubrica.getData().rows[i];
        if (q.Tipo == tipo) {
            var codice = q.Codice;
            //$(' .selectpicker')


            var valore = q.Valore;
            $('#TxtRubrica_Numero').val(valore);

            switch (q.Tipo) {
                case "1":
                    $('#Cmb_Rubrica_Descrizione').val("Telefono");
                    break;
                case "2":
                    $('#Cmb_Rubrica_Descrizione').val("Cellulare");
                    break;
                case "3":
                    $('#Cmb_Rubrica_Descrizione').val("Fax");
                    break;
                case "4":
                    $('#Cmb_Rubrica_Descrizione').val("Email");
                    break;
                case "5":
                    $('#Cmb_Rubrica_Descrizione').val("Social");
                    break;
                case "6":
                    $('#Cmb_Rubrica_Descrizione').val("Web");
                    break;
            }
        }
    }

    //                 EliminaCodice(obj);                

}

function ottieniCAP(ISTAT_Prov, ISTAT_Com, callback) {
    var parametri = kendo.stringify({
        "ISTAT_Prov": ISTAT_Prov,
        "ISTAT_Com": ISTAT_Com
    });

    ajaxAgronica("Impresa_Edit.aspx/OttieniCAP",
        parametri,
        function (risposta) {
            callback(risposta);
        }, null, null, false);
}