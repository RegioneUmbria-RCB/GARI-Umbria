var permessiCentro;
var obj_agenda;
var obj_stalla;
var obj_stalla_raggruppamento;
var cmb_tipo;
var Cmb_Specie;
var Cmb_Razza;
var Cmb_Stato;
var validator;
var TxtValiditaInizio;
var TxtValiditaFine;
var txt_nome;
var valid;
var permesso_bdn_read;
var permesso_bdn_write;

jQuery(document).ready(function () {

    docReady();

});

async function docReady() {

    WaitFrame.show();
    valid = false;
    obj_agenda = JSON.parse(objP_agenda);

    var promises = new Array();

    promises.push(get_permessi(57));
    promises.push(get_stalla());
    promises.push(get_raggruppamento_stalla());
    promises.push(Agro_LeggiPermessoUtente(usernameLoggato, 99, 0));
    promises.push(Agro_LeggiPermessoUtente(usernameLoggato, 99, 2));

    validator = $("#aspnetForm").kendoValidator().data("kendoValidator");

    let resp = await Promise.all(promises);
    permessiCentro = resp[0];
    obj_stalla = resp[1];
    obj_stalla_raggruppamento = resp[2];
    permesso_bdn_read = resp[3];
    permesso_bdn_write = resp[4];

    get_lista_Tipi().then((cmb) => {
        cmb_tipo = cmb;
    });

    $("#LblCentro").html(obj_stalla.sa_nome);
    $("#LblStalla").html(obj_stalla.s.STA_DES);

    $("#tabstrip").kendoTabStrip({
        select: function (e) {
            var text = $(e.item).find("> .k-link").text();
        },
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
    kendoTab = $("#tabstrip").data("kendoTabStrip");
    kendoTab.select(0);

    if (permessiCentro.Scrittura === true && obj_agenda.Tipo_Operazione !== "0") {
        $("#divSalva").show();
        if (obj_agenda.Tipo_Operazione === "1") {
            $("#btnSalvaScrivi").show();
            //$("#btnSalvaScrividd").show();
        }
    }

    imposta_CheckBdn();
    imposta_TxtValiditaInizio();
    imposta_TxtValiditaFine();
    imposta_txt_codice();
    imposta_TxtMq();
    imposta_txt_nome();
    get_Cmb_Specie();
    get_Cmb_Razza();
    get_Cmb_Stato();

    WaitFrame.hide();
}