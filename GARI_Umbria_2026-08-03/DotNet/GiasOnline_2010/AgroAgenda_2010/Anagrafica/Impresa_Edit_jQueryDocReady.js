var watableGerarchia;

if (typeof window.event != 'undefined') // ie
    document.onkeydown = document.onkeypress; // Trap bksp in ie. !! Note: does not trap enter, but onkeypress does !!

var btn_salva_clicked = false;

var obj_Agenda;

var impresaEditResx;

var fl1 = false;
var fl2 = false;
var fl3 = false;
var fl4 = false;
var fl5 = false;
var fl6 = true;
var fl7 = false;
var fl8 = false;

function ImpresaEditResxLeggi() {
    var letturaRiuscita = false;
    ajaxAgronicaSync("../Localization.aspx/RitornaRisorseBS", JSON.stringify({ files: "App_GlobalResources/AgronicaAgenda_2010.resx" }), false,
        function (risposta) {
            try {
                impresaEditResx = JSON.parse(risposta.RispostaStringa);
                letturaRiuscita = true;
                console.log("ImpresaEditResxLeggi...letto correttamente.");
            } catch (e) {
                console.log("ImpresaEditResxLeggi...errori in fase di parse del json.");
            }

        }, function (risposta) {
            console.log("ImpresaEditResxLeggi...errori in fase di lettura.");
        });
    return letturaRiuscita;
}


jQuery(document).ready(function () {

    if (!impresaEditResx) {
        ImpresaEditResxLeggi();
    }

    obj_Agenda = JSON.parse(objP_agenda);

    $('#aspnetForm').change(function () {
        controlla_form();
    });


    //carico eventualmente la tabella di gerarchia padre
    var initGerarchia;
    //carico eventualmente la tabella di gerarchia codici
    var initCodici;

    if (jsPadre != "") {
        initGerarchia = jsPadre;
    } else {
        initGerarchia = "";
    }

    if (initGerarchia != "") {
        AggiornaTabGerarchia(initGerarchia);
    }

    if (jsCodici != "") {
        initCodici = jsCodici;
    } else {
        initCodici = "";
    }

    if (initCodici != "") {
        AggiornaTabCodici(initCodici);
    }

    //Disabilito il cambio di tab se la validazione è fallita
    //            $('.nav-tabs > li > a').click(function (e) {
    //                if ($('.error-tab').length)
    //                    return false;
    //                else
    //                    return true;
    //            });


    // Gestione della selezione della nazione
    if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {
        $("#div_prov_com").show();
        $("#lbl_frazione").text(Traduzione(impresaEditResx, "Frazione", "Frazione"));
    }
    else {
        $("#div_prov_com").hide();
        $("#lbl_frazione").text(Traduzione(impresaEditResx, "Città", "Città"));
    }

    $('#cmb_Stato').change(function (e) {

        if ($("#cmb_Stato :selected").attr('Gestione_Gerarchia_Geografica') == "1") {

            stato = $('#cmb_Stato').val();

            if ($('#cmb_Stato').val() != "") {
                $('#dll_Provincia').parent().children().attr("disabled", false);

                $.ajax({
                    type: 'POST',
                    url: 'Impresa_Edit.aspx/Carica_Province',
                    data: "{stato:'" + stato + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        //alert(r.d);
                        $('#dll_Provincia').empty();
                        $('#dll_Provincia').append(r.d[0]);
                        $('#Txt_ProCodIstat').val(r.d[1]);
                        /*$('#Txt_ProvinciaSigla').val(provincia);*/
                        $('#dll_comune').empty();


                        $('.selectpicker').selectpicker('refresh');
                    }
                });
            }
            else {
                // Disabilito i comuni se vuoto
                $('#dll_Provincia').empty();
                $('#dll_Provincia').parent().children().attr("disabled", true);
                $('.selectpicker').selectpicker('refresh');
            }

            $("#div_prov_com").show();
            $("#lbl_frazione").text(Traduzione(impresaEditResx, "Frazione", "Frazione"));
            $("#Txt_Frazione").val("");
        }
        else {
            $("#div_prov_com").hide();
            $("#lbl_frazione").text(Traduzione(impresaEditResx, "Città", "Città"));
            $("#Txt_Frazione").val("");
        }
    });






    // Nascondo/Visualizzo la parte addizionale in caso di Nuova Impresa
    if (($('#Chk_Centro').length > 0) && ($('#Chk_Magazzino').length > 0))
        $('#div-additional').show();


    // Gestione click del bottone Cerca in tab Gerarchia
    $('#btn_gerarchia_search').click(function (e) {
        // Controllo se è stato immesso del testo nel input del Cerca
        if ($('#txtPivaPadre').val() != "") {
            $('.riga_referente').show();

            $.ajax({
                type: 'POST',
                url: 'Impresa_Edit.aspx/Carica_Select_Padri',
                data: "{parametro:'" + $('#txtPivaPadre').val() + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#Cmb_Imprese').html(r.d);
                }
            });

        }
        else {
            $('#txtPivaPadre').focus();
        }

    });

    // se premo invio faccio partire la ricerca
    $("#txtPivaPadre").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#btn_gerarchia_search").click();
            return false;
        }
    });

    $('#BtnPiva_Fittizia').click(function (e) {
        if (!$('#TxtPiva').prop('readonly')) {
            $.ajax({
                type: 'POST',
                url: 'Impresa_Edit.aspx/GeneraRandom',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true) {
                        $("#TxtPiva").val(r.d.RispostaStringa);
                    }
                    else {
                        alert(r.d.Errore);
                    }

                }
            });
        }
    });


    // Gestione click del bottone Aggiungi in tab Dati Accessori
    $('#btn_Aggiungi_Codice').click(function (e) {

        var codice = $('#div_CmbCodice button').text();
        var valore = $('#TxtCodiceValore').val();
        var DataInizio = "";
        var DataFine = "";

        // Controllo se è stato immesso del testo nel input del Cerca
        if ((codice != "") && (valore != "")) {
            codice = codice.trim()

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
                url: 'Impresa_Edit.aspx/Aggiungi_Codice',
                data: "{codice:'" + codice + "', valore:'" + valore + "', codice_id:'" + codice_id + "', DataInizio:'" + DataInizio + "', DataFine:'" + DataFine + "'}",
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
        else {
            $('#Cmb_Imprese').focus();
        }


    });





   
    // Quando cambia Provincia, mostro Comuni relativi
    $('#dll_Provincia').change(function () {

        provincia = $('#dll_Provincia').val();

        if ($('#dll_Provincia').val() != "") {
            $('#ddl_comune').parent().children().attr("disabled", false);

            $.ajax({
                type: 'POST',
                url: 'Impresa_Edit.aspx/Carica_Comuni',
                data: "{provincia:'" + provincia + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#ddl_comune').empty();
                    $('#ddl_comune').append(r.d[0]);
                    $('#Txt_ProCodIstat').val(r.d[1]);
                    $('#Txt_ProvinciaSigla').val(provincia);

                    $('.selectpicker').selectpicker('refresh');
                }
            });
        }
        else {
            // Disabilito i comuni se vuoto
            $('#ddl_comune').empty();
            $('#ddl_comune').parent().children().attr("disabled", true);
            $('.selectpicker').selectpicker('refresh');
        }

        if ($("#dll_Provincia").val() != "") {
            $('.voce_6').hide();
            fl6 = true;
        }
        else {
            $('.voce_6').show();
            fl6 = false;
        }

        nascondi_riepilogo_error();
    });


    // Quando cambia Comune
    $('#ddl_comune').change(function (e) {

        comune = $('#ddl_comune').val();
        //$("#<%=ddl_comune.ClientID %> select").val(comune);
        $("#ddl_comune > option").each(function () {
            if (this.value == comune) {
                var nome_comune = this.text
                //$(this).attr('selected', 'selected');
                $.ajax({
                    type: 'POST',
                    url: 'Impresa_Edit.aspx/Set_Comune',
                    data: "{comune:'" + comune + "', nome_comune:'" + nome_comune + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {

                        var partsArray = $('#ddl_comune').val().toString().split('|');
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

        if ($("#ddl_comune").val() != "") {
            $('.voce_7').hide();
            fl7 = true;
        }
        else {
            $('.voce_7').show();
            fl7 = false;
        }

        nascondi_riepilogo_error();

    });



    // Conferma dell'eliminazione riga da qualsiasi Gridview
    $('.fa-times').click(function (e) {
        if (confirm(Traduzione(impresaEditResx, "SeiSicuroDiVolerEliminareLaRiga", "Sei sicuro di voler eliminare la riga?")) == false) {
            e.preventDefault();
        }
    });


    // CONTROLLO SU AZIENDE PADRE
    // Se è già presente un'azienda da inserire
    $('#btn_gerarchia_add').click(function (e) {

        let piva = $('#Cmb_Imprese').val();
        let rag_soc = $("#Cmb_Imprese option:selected").html();
        if (piva !== "") {

            let param = {
                p_iva: piva,
                rag_soc: rag_soc
            }

            $.ajax({
                type: 'POST',
                url: 'Impresa_Edit.aspx/Aggiungi_Padre',
                data: JSON.stringify(param),
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d.RispostaOK == true) {
                        AggiornaTabGerarchia(JSON.parse(r.d.RispostaStringa));
                    } else {
                        kendo.alert(r.d.Errore);
                    }
                }
            });
        } else {
            kendo.alert(Traduzione(impresaEditResx, "SelezionareElementoDaElenco", "Selezionare un elemento dall'elenco."));
        }

        //comune = $('#ddl_comune').val();
        ////$("#ddl_comune select").val(comune);
        //$("#ddl_comune > option").each(function () {
        //    if (this.value == comune) {
        //        var nome_comune = this.text
        //        //$(this).attr('selected', 'selected');
        //        $.ajax({
        //            type: 'POST',
        //            url: 'Impresa_Edit.aspx/Set_Comune',
        //            data: "{comune:'" + comune + "', nome_comune:'" + nome_comune + "'}",
        //            contentType: 'application/json; charset=utf-8',
        //            cache: false,
        //            dataType: 'json', async: true,
        //            success: function (r) {
        //            }
        //        });
        //    }
        //});

        //$('#Txt_ComCodIstat').val(comune.substring(3, 6));

    });

    nascondi_riepilogo_error();

    // Gestione Riepilogo Errori (Validazione)
    $("#TxtRagioneSociale").on('keyup keypress focusout', function () {
        if ($("#TxtRagioneSociale").val() != "") {
            $('.voce_1').hide();
            fl1 = true;
        }
        else {
            $('.voce_1').show();
            fl1 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Cmb_FormaGiuridica").change(function () {
        if ($("#Cmb_FormaGiuridica").val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#TxtPiva").on('keyup keypress focusout', function () {
        if ($("#TxtPiva").val() != "") {
            $('.voce_3').hide();
            fl3 = true;
        }
        else {
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#TxtCodiceFiscale").on('keyup keypress focusout', function () {
        if ($("#TxtCodiceFiscale").val() != "") {
            $('.voce_4').hide();
            fl4 = true;
        }
        else {
            $('.voce_4').show();
            fl4 = false;
        }

        nascondi_riepilogo_error();
    });

    $("#Txt_Via").on('keyup keypress focusout', function () {
        if ($("#Txt_Via").val() != "") {
            $('.voce_5').hide();
            fl5 = true;
        }
        else {
            $('.voce_5').show();
            fl5 = false;
        }

        nascondi_riepilogo_error();
    });

    


    $("#Txt_CAP").change(function () {
        if ($("#Txt_CAP").val() != "") {
            $('.voce_8').hide();
            fl8 = true;
        }
        else {
            $('.voce_8').show();
            fl8 = false;
        }

        nascondi_riepilogo_error();
    });

    generalCheck();
    /////////////////////


    if ($('#dll_Provincia').val() != undefined && $('#dll_Provincia').val() != "" && ($('#ddl_comune').val() == undefined || $('#ddl_comune').val() == "")) {
        provincia = $('#dll_Provincia').val();
        $.ajax({
            type: 'POST',
            url: 'Impresa_Edit.aspx/Carica_Comuni',
            data: "{provincia:'" + provincia + "'}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                //alert(r.d);
                $('#ddl_comune').empty();
                $('#ddl_comune').append(r.d[0]);
                $('#Txt_ProCodIstat').val(r.d[1]);
                $('#Txt_ProvinciaSigla').val(provincia);

                $('.selectpicker').selectpicker('refresh');
            }
        });
    }

});

function generalCheck() {
    if ($("#TxtRagioneSociale").val() != "") {
        $('.voce_1').hide();
        fl1 = true;
    }
    else {
        $('.voce_1').show();
        fl1 = false;
    }

    if ($("#Cmb_FormaGiuridica").val() != "") {
        $('.voce_2').hide();
        fl2 = true;
    }
    else {
        $('.voce_2').show();
        fl2 = false;
    }

    if ($("#TxtPiva").val() != "") {
        $('.voce_3').hide();
        fl3 = true;
    }
    else {
        $('.voce_3').show();
        fl3 = false;
    }

    if ($("#TxtCodiceFiscale").val() != "") {
        $('.voce_4').hide();
        fl4 = true;
    }
    else {
        $('.voce_4').show();
        fl4 = false;
    }


    if ($("#Txt_Via").val() != "") {
        $('.voce_5').hide();
        fl5 = true;
    }
    else {
        $('.voce_5').show();
        fl5 = false;
    }

    if ($("#dll_Provincia").val() != "") {
        $('.voce_6').hide();
        fl6 = true;
    }
    else {
        $('.voce_6').show();
        fl6 = false;
    }

    if ($("#ddl_comune").val() != "") {
        $('.voce_7').hide();
        fl7 = true;
    }
    else {
        $('.voce_7').show();
        fl7 = false;
    }


    if ($("#Txt_CAP").val() != "") {
        $('.voce_8').hide();
        fl8 = true;
    }
    else {
        $('.voce_8').show();
        fl8 = false;
    }

    nascondi_riepilogo_error();
}

function nascondi_riepilogo_error() {
    if (fl1)
        $('.voce_1').hide();

    if (fl2)
        $('.voce_2').hide();

    if (fl3)
        $('.voce_3').hide();

    if (fl4)
        $('.voce_4').hide();

    if (fl5)
        $('.voce_5').hide();

    if (fl6)
        $('.voce_6').hide();

    if (fl7)
        $('.voce_7').hide();

    if (fl8)
        $('.voce_8').hide();

    if ((fl1) && (fl2) && (fl3) && (fl4) && (fl5) && (fl6) && (fl7) && (fl8))
        $('#div_riepilogo_error').hide();
    else
        $('#div_riepilogo_error').show();
}