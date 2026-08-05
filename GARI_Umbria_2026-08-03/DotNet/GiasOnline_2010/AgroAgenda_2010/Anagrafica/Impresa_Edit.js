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


function ValidaxSubmit() {

    btn_salva_clicked = true;
    var flag = controlla_form();

    if (flag) {

        var nome_comune = $('#ddl_comune').text;

        var stato = $('#cmb_Stato').val();
        var piva = $('#TxtPiva').val().trim();

        if (piva == "") {
            kendo.alert("Inserire una partita IVA valida");
            return false;
        }

        if (stato == "IT" && (piva.length != 11 && piva.length != 16)) {
            kendo.alert("Inserire una partita IVA valida");
            return false;
        }

        //$.ajax({
        //    type: 'POST',
        //    url: 'Impresa_Edit.aspx/Set_Comune',
        //    data: "{comune:'" + $('#ddl_comune').val() + "'}",
        //    contentType: 'application/json; charset=utf-8',
        //    cache: false,
        //    dataType: 'json', async: true,
        //    success: function (r) {
        //    }

        //});

        $('#ImgBtn_SalvaTutto').click();

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

    // Controllo Webservice se la partita iva inserita esiste già
    if (obj_Agenda.TipoOperazioneAgenda == "1" && $('#TxtPiva').val() != "") {
        $.ajax({
            type: 'POST',
            url: 'Impresa_Edit.aspx/WS_ControllaPiva',
            data: "{Piva:'" + $('#TxtPiva').val() + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                if (r.d.RispostaOK == false) {
                    $('#TxtPiva').parent().append('<label id="TxtPiva-error" class="custom_val error" for="TxtPiva">' + r.d.Errore + '</label>');
                    $('#TxtPiva').closest("input").css('border', '1px solid #D41E1A');
                    flag = false;
                    n_inv++;
                }

            }
        });
    }


    if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {    
        
        
        var cap = $('#Txt_CAP').val();

        if(cap.length < 5 && $('#cmb_Stato').val() == "IT") {
            $('#Txt_CAP').parent().append('<label id="Txt_CAP-error" class="custom_val error" for="Txt_CAP">' + Traduzione(impresaEditResx, 'ControlloLunghezzaCampoCAP', 'Il campo deve essere di 5 caratteri') + '</label>');
            $('#Txt_CAP').closest("input").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#dll_Provincia').val() == "") {
            $('#dll_Provincia').parent().append('<label id="dll_Provincia-error" class="custom_val error" for="dll_Provincia">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
            $('#dll_Provincia').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

        if ($('#ddl_comune').val() == null || $('#ddl_comune').val() == "") {
            $('#ddl_comune').parent().append('<label id="ddl_comune-error" class="custom_val error" for="ddl_comune">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
            $('#ddl_comune').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

    }

    if ($('#TxtRagioneSociale').val() == "") {
        $('#TxtRagioneSociale').parent().append('<label id="TxtRagioneSociale-error" class="custom_val error" for="TxtRagioneSociale">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#TxtRagioneSociale').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    if ($('#TxtPiva').val() == "") {
        $('#TxtPiva').parent().append('<label id="TxtPiva-error" class="custom_val error" for="TxtPiva">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#TxtPiva').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    if ($('#TxtCodiceFiscale').val() == "") {
        $('#TxtCodiceFiscale').parent().append('<label id="TxtCodiceFiscale-error" class="custom_val error" for="TxtCodiceFiscale">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#TxtCodiceFiscale').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    if ($('#Txt_Via').val() == "") {
        $('#Txt_Via').parent().append('<label id="Txt_Via-error" class="custom_val error" for="Txt_Via">' + Traduzione(impresaEditResx, 'IlCampoNonPuòEssereVuoto', 'Il campo non può essere vuoto') + '</label>');
        $('#Txt_Via').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    // Controllo se è stato inserito almeno 1 padre
    //if($('#tabGerarchia .watable >tbody >tr').length == 0)
    //{   
    //    $('#tabGerarchia').append('<label id="tabGerarchia-error" class="custom_val error" for="tabGerarchia" style="padding: 5px 0">Deve essere inserito almeno un Padre</label>');
    //    flag = false;
    //    n_inv++;
    //}


    // Controllo se le date sono corrette
    var TxtValiditaInizio = $('#TxtValiditaInizio').val().split("/");
    var TxtValiditaFine = $('#TxtValiditaFine').val().split("/");

    ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
    fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

    if (ini > fin) {
        $('#TxtValiditaInizio').parent().append('<label id="TxtValiditaInizio-error" class="custom_val error" for="TxtValiditaInizio">' + Traduzione(impresaEditResx, 'DataInizioNonPuòEssereMaggioreDiDataFine', 'La data di inizio non può essere maggiore di quella di fine') + '</label>');
        $('#TxtValiditaInizio').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;

    }


    // Controllo se il cap contiene valori non numerici
    if ($('#Txt_CAP').val().match(/[a-z]/i)) {
        $('#Txt_CAP').parent().append('<label id="Txt_CAP-error" class="custom_val error" for="Txt_CAP">' + Traduzione(impresaEditResx, 'IlCampoPuòcontenereSoloNumeri', 'Il campo può contenere solo numeri') + '</label>');
        $('#Txt_CAP').closest("input").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }


    //// Controllo se Forma giuridica è settato
    //if ($('#<%=Cmb_FormaGiuridica.ClientID %>').val() == "") {
    //    $('#<%=Cmb_FormaGiuridica.ClientID %>').parent().append('<label id="<%=Cmb_FormaGiuridica.ClientID %>-error" class="custom_val error" for="<%=Cmb_FormaGiuridica.ClientID %>">Deveessere selezionata una voce</label>');
    //    $('#<%=Cmb_FormaGiuridica.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}


    ///////////////////////////////


    if (v.valid() && flag) {

        $('.nav-tabs li.active a .error-tab').remove(".error-tab");
        return true
    }
    else {
        if (!btn_salva_clicked) {
            var err_message = "";
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li.active a').append(err_message);
        }

        btn_salva_clicked = false;

        return false;
    }

}


function EliminaGerarchiaPadre(obj) {
    var Piva = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Impresa_Edit.aspx/EliminaGerarchiaPadre',
        data: "{Piva:'" + Piva + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true) {
                AggiornaTabGerarchia(JSON.parse(r.d.RispostaStringa));

                if (JSON.parse(r.d.RispostaStringa).rows == "") {
                    controlla_form();
                }
            }
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function EliminaCodice(obj) {
    var Id_Cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Impresa_Edit.aspx/EliminaCodice',
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

function AggiornaTabGerarchia(d) {
    AgroWA_Table_sistemaDati(d);

    $('#tabGerarchia').html('');
    watableGerarchia = $("#tabGerarchia").WATable({
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

    InitWaTable("#tabGerarchia", watableGerarchia);
}


function AggiornaTabCodici(d) {

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
}


//ingresso jquery
$(function () {



    $('body').on('keyup', '#TxtPiva', function (e) {

        //controllo se viene digitato un numero
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }

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
                        $('#TxtVerificaPiva').html(Traduzione(impresaEditResx, "PartitaIvaGiàInserita", "Partita iva già inserita"));
                        $('#TxtVerificaPiva').addClass('bg-danger');
                    }


                }
            });
        }
        else
            $('#TxtVerificaPiva').html('');
            
    });

});