var permessoBrogliaccio;
var Exp_Tracciabilita;
var Exp_Tracciabilita_Modalita;
var Exp_Tracciabilita_DestinazionePath;
var Exp_Tracciabilita_LinkWSEsterno;
var Exp_Tracciabilita_WSEsternoParametri;


var TipiVisualizzazione = [
    { text: "Tabellare", value: "1" },
    { text: "Grafo", value: "2" }
];

//DOCUMENT READY
jQuery(function () {

    docReady();

});

async function docReady() {

    $.logThis("DocReady: INIZIO");

    //inizializzazione della pagina la prima volta che viene caricata

    // Caricamento della DropDown Certificazione
    var certificazioneDS = new kendo.data.DataSource({
        transport: {
            read: RicercaCertificazione
        }
    });

    $("#Cmb_TipoVisualizzazione").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: TipiVisualizzazione,
        index: 0
    });

    $('input[name$="ddlCertif"]').kendoDropDownList({
        dataSource: certificazioneDS,
        dataTextField: "val_des",
        dataValueField: "val_cod"
    });

    //Pulsanti
    $("#btn_ricerca").click(function () {
        Cerca();
    });

    $("#btn_ricerca_righe_doc").click(function () {
        RicercaRigheDocumenti();
    });



    $("#btnRintraccia").click(function () {
        doTrack();
    });

    $("#btnEsportaPOC").click(function () {
        exportToPOC();
    });

    if (Exp_Tracciabilita !== true) {
        $("#btnEsportaPOC").hide();
    } else {
        $("#btnEsportaPOC").show();
    }

    //fine pulsanti

    //inizializzazione in base al tipo di pagina
    $.logThis("PageMode:=" + PageMode);

    //if (PageMode == "track") {
    //} else {
    //}

    permessoBrogliaccio = await Agro_LeggiPermessoUtente(usernameLoggato, 355, 2);
    var paramTrackCode = getParameterByName("lot");
    var paramTrackAlgorith = getParameterByName("al");

    if (paramTrackCode != undefined) {
        $("#txt_TrackCode").val(paramTrackCode);
        if (paramTrackAlgorith != undefined) {
            $(iTipoLotto).val(paramTrackAlgorith);
        }

        $("#btnRintraccia").click();
    }

    $(function () {
        $(window).keydown(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        });
    })

}