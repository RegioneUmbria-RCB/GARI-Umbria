/*
* Creato il 2017-07-18
* 
* 
* Contiene tutti i WS alla pagina Impianto_edit.vb
*
* Author: Galassi Simone
*/



/**
 * Chiamo il Webservice per trovare il progetto selezionato e riempire i dati sottostanti
 * @param {any} progetto_cod
 */
function riempiTabCodici(progetto_cod) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Riempi_Tab_Codici',
        data: "{progetto_cod:'" + progetto_cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            var d1 = $.Deferred();
            AggiornaTabCodici(JSON.parse(r.d[0]), d1);

            $('#Cmb_CapitolatoPrivato').val(r.d[1]);
            $('.selectpicker').selectpicker('refresh');

            $('#Cmb_OrganismoReferente').val(r.d[2]);

            $.when(riempi_MagazzinoConferimento(r.d[2], r.d[3])).done(function () {

                d.resolve();

            });

            //riempi_MagazzinoConferimento(r.d[2], r.d[3]);
            //$('#Cmb_MagazzinoConferimento').val(r.d[3]);
        },
        error: function () {
            d.reject();
        }
    });
    return d;
}

/**
 * Chiamo il Webservice per trovare il progetto selezionato e riempire i dati sottostanti
 * @param {any} progetto_cod
 */
function riempiTabParticelle(progetto_cod) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Riempi_Tab_Particelle',
        data: "{progetto_cod:'" + progetto_cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            AggiornaTabParticelle(JSON.parse(r.d), d);
        },
        error: function () {
            d.reject();
        }
    });
    return d;
}

/**
 * chiamo il WS che preleva i dati da inserire nella form del progetto
 * @param {any} progetto_cod
 */
function riempiInfoImpianto(progetto_cod) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Riempi_Info_Impianto',
        data: "{progetto_cod:'" + progetto_cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {

            $('#TxtPianteImpianto2').val(r.d[0]);
            $('#Txt_semina_prevista').val(r.d[1]);
            $('#Txt_fioritura_prevista').val(r.d[2]);
            $('#Txt_raccolta_prevista').val(r.d[3]);
            $('#Txt_ResaPrevista').val(r.d[4]);
            $('#Txt_Resa1').val(r.d[5]);
            $('#Cmb_Disciplinare').val(r.d[6] + "/1");
            $('.selectpicker').selectpicker('refresh');
            $('#Cmb_Regolamento').val(r.d[7]);
            $('.selectpicker').selectpicker('refresh');
            $('#Cmb_RegolamentoConc').val(r.d[8]);
            $('.selectpicker').selectpicker('refresh');
            $('#Cmb_Stato').val(r.d[9]);
            $('.selectpicker').selectpicker('refresh');
            $('#Txt_Lotto').val(r.d[10]);
            d.resolve();
        },
        error: function () {
            d.reject();
        }
    });
    return d;
}

/**
 * Elimina la distinta selezionata
 * @param {any} obj
 */
function EliminaProgetti(obj) {
    progetto_cod = $(obj).attr('chiave');

    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/EliminaProgetto',
        data: "{progetto_cod:'" + progetto_cod + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true) {
                AggiornaTabImpianti(JSON.parse(r.d.RispostaStringa));
                $('#ImgBtn_Cancella_Distinta').click();
            }
            else {
                alert(r.d.Errore);
            }
        }
    });
}

/**
 * Controlla la presenza di altri impianti nello stesso appezzamento all'interno delle date scelte
 * @param {any} data_inizio
 * @param {any} data_fine
 * @param {any} var_contr
 */
function esistonoAltriImpiantiSuAppezzamento(data_inizio, data_fine, var_contr) {

    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Esistono_Impianti_Su_Appezzamenti',
        data: "{data_inizio:'" + data_inizio + "', data_fine:'" + data_fine + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK === false) {
                alert(r.d.Errore);
                $('#TxtValiditaInizio').val('');
                $('#TxtValiditaFine').val('');
                var_contr.flag = false;
                var_contr.n_inv++;
            }
        }
    });

}

/**
 * 
 * @param {any} parametro
 * @param {any} val_selected
 */
function riempi_MagazzinoConferimento(parametro, val_selected) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_MagazzinoConferimento',
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            $('#Cmb_MagazzinoConferimento').empty();
            $('#Cmb_MagazzinoConferimento').html(r.d);
            $('#Cmb_MagazzinoConferimento').val(val_selected);
            $('.selectpicker').selectpicker('refresh');
            d.resolve();
        },
        error: function () {
            d.reject();
        }
    });


    return d;
}

/**
 * controlla le date della distinta che si vuole salvare con quelle già presenti
 * @param {any} inizio_distinta
 * @param {any} fine_distinta
 * @param {Numeric} mode scrittura=1 modifica=2
 * @param {any} progetto_cod
 * @param {any} var_contr
 */
function controlla_Date_Distinta_Su_Storico(inizio_distinta, fine_distinta, mode, progetto_cod, var_contr) {
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Controlla_Date_Distinta_Su_Storico',
        data: "{'data_inizio':'" + inizio_distinta + "', 'data_fine':'" + fine_distinta + "', 'modalita':'" + mode + "', 'chiave':'" + progetto_cod + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json', async: false,
        success: function (r) {
            if (r.d == "") {
                $('#Txt_ValiditaInizio_Distinta').parent().append('<label id="Txt_ValiditaInizio_Distinta-error" class="custom_val error" for="Txt_ValiditaInizio_Distinta">Le date della nuova distinta si intersecano con una già presente</label>');
                $('#Txt_ValiditaInizio_Distinta').closest("input").css('border', '1px solid #D41E1A');
                var_contr.flag = false;
                var_contr.n_inv3++;
            }
        }
    });
}

function esiste_Obbligo_SalvataggioOrganismoReferente(var_contr) {

    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Esiste_Obbligo_SalvataggioOrganismoReferente',
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json', async: false,
        success: function (r) {
            if (r.d == "") {
                $('#Cmb_OrganismoReferente').parent().append('<label id="Cmb_OrganismoReferente-error" class="custom_val error" for="Cmb_OrganismoReferente">E obbligatorio impostare l Organismo Referente</label>');
                $('#Cmb_OrganismoReferente').closest("input").css('border', '1px solid #D41E1A');
                var_contr.flag = false;
                var_contr.n_inv3++;
            }
        }
    });
}

function Carica_Select_Cultivar(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_Cultivar',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}

function Carica_Select_Finalita(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_Finalita',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}

function Carica_Select_TipologiaVarietale(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_TipologiaVarietale',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}

function Carica_Select_Allevamenti(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_Allevamenti',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}

function Carica_Select_Portinnesto(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_Portinnesto',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}

function Carica_Select_Copertura(parametro, callback) {
    var d = $.Deferred();
    $.ajax({
        type: 'POST',
        url: 'Impianto_Edit.aspx/Carica_Select_Copertura',
        data: "{parametro:'" + parametro + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            //alert(r.d);
            callback(r);
            d.resolve();
        },
        error: function (err) {
            d.reject();
        }
    });
    return d;
}