

//AnteprimaEtichette.js



var indirizzohttp = "./AnteprimaEtichette.aspx"

var LabelPrintSeparator = ",";
var LabelPrint = "|";
var SeparatoreCodiceQRY = "-";

var cTipoStampa_PerConfigurazione = 1;
var cTipoStampa_PerStampante = 2;


function StampaTutto_tipo() {
    switch ($("#" + hLav_Cod_ClientID).val()) {
        case "5001":
            return cTipoStampa_PerStampante;
        default:
            return cTipoStampa_PerConfigurazione;
    }
}

function StampaTutto() {

    var id_agenda = $("#" + txtID_Agenda_ClientID).val();
    var ConfigurazioneDaStampare = "";
    var tipo = StampaTutto_tipo();

    if (tipo == cTipoStampa_PerStampante)
        ConfigurazioneDaStampare = LeggiTutteConfigurazioniSelezionate2(id_agenda);
    else
        ConfigurazioneDaStampare = LeggiTutteConfigurazioniSelezionate(id_agenda);

    var url = indirizzohttp + "/Anteprima_o_Stampa";
    var parametriChiamata = { ConfigurazioneDaStampare: ConfigurazioneDaStampare, StampaDiretta: 'true' };

    Stampa(url, parametriChiamata);

}

function LeggiTutteConfigurazioniSelezionate(id_agenda) {
    var data = waTable.getData(true, false);
    var cfgStampaTutto = "";
    for (var i = 0; i < data.rows.length; i++) {
        cfgStampaTutto += LeggiConfigurazioniStampa(id_agenda, data.rows[i].Sel) + LabelPrintSeparator + data.rows[i].FF_Stampa_Dettagli_COD + LabelPrint;
    }

    return cfgStampaTutto;
}

function LeggiTutteConfigurazioniSelezionate2(id_agenda) {
    var data = waTable.getData(true, false);
    var cfgStampaTutto = "";
    for (var i = 0; i < data.rows.length; i++) {
        cfgStampaTutto += LeggiConfigurazioniStampa2(id_agenda, data.rows[i].Sel) + LabelPrintSeparator + data.rows[i].FF_Stampanti_cod_FF_Stampa_Dettagli_COD + LabelPrint;
    }

    return cfgStampaTutto;
}

function LeggiConfigurazioniStampa2(id_agenda, codice) {


    var id_mov_det = "";
    var ff_stampante_cod = "";
    ff_stampante_cod = codice; //codice.toString().split(SeparatoreCodiceQRY)[0];
    id_mov_det = $("#ddl_movDet_" + codice).val();

    var nEtich = "";
    nEtich = getNumeroEtichetteDato_Dettaglio2(1, codice);

    return $("#" + cmbOModuli_Referenze_Config_Testata_ClientID).val() + LabelPrintSeparator + 
          id_agenda.toString() + LabelPrintSeparator +
          id_mov_det + LabelPrintSeparator +
          $("#ddl_layout_" + codice).val() + LabelPrintSeparator +
          ff_stampante_cod + LabelPrintSeparator +
          getStampanteDiv(codice) + LabelPrintSeparator +
          $("#ddl_lingua_" + codice).val() + LabelPrintSeparator +
          nEtich;
}


function LeggiConfigurazioniStampa(id_agenda, mID, ff_stampante_cod) {

    var nEtich = "";
    nEtich = getNumeroEtichetteDato_Dettaglio(1, mID);

    return $("#" + cmbOModuli_Referenze_Config_Testata_ClientID).val() + LabelPrintSeparator + 
          id_agenda.toString() + LabelPrintSeparator +
          mID.toString() + LabelPrintSeparator +
          $("#ddl_layout_" + mID).val() + LabelPrintSeparator +
          $("#ddl_stampante_" + mID).val() + LabelPrintSeparator +
          $("#ddl_stampante_" + mID + " option:selected").text() + LabelPrintSeparator +
          $("#ddl_lingua_" + mID).val() + LabelPrintSeparator +
          nEtich;
}

function Stampa(url, parametriChiamata) {
    console.log("Chiamata ad anteprima o stampa");
    ajaxAgronica(url, JSON.stringify(parametriChiamata),
        function (risposta) {
            console.log(risposta);
            var msg_d = risposta.UrlLink;
            if (risposta.IsLink) {

                if (risposta.Lista_FF_Stampa_Dettagli_Cod !== null)
                    Riporta_FF_Dettagli_Cods(risposta.Lista_FF_Stampa_Dettagli_Cod);

                $("#iframedivAnteprimaStampa").attr("src", msg_d);
                $("#lblAnteprimaStampaTitle").html("Anteprima Etichetta");
                $("#divAnteprimaStampa").modal('toggle');
            }
            else {
                MessaggioAttenzione_Bootstrap(msg_d, "DIV_Messaggi");
            }
        }, null);
}

function Riporta_FF_Dettagli_Cods(lista) {

    var a = $(".watable-col-FF_Stampa_Dettagli_COD");
    var vvv = lista.split(",");
    var data = waTable.getData(true, false);

    for (var i = 0; i < $(".watable-col-Sel").length; i++) {
        if (data.rows[i] !== undefined) {
            $(a[i]).text(vvv[i]);
            data.rows[i].FF_Stampa_Dettagli_COD = vvv[i];
        }
    }


}

function anteprima_dettaglio2(id_agenda, codice) {

    
    console.log("anteprima_dettaglio, id_agenda: " + id_agenda);
    console.log("anteprima_dettaglio, codice: " + codice);

    var codice1 = codice.id.split("_")[2];

    var ConfigurazioneDaStampare =
        LeggiConfigurazioniStampa2(id_agenda, codice1) + ",-1";


    var url = indirizzohttp + "/Anteprima_o_Stampa";
    var parametriChiamata = { ConfigurazioneDaStampare: ConfigurazioneDaStampare, StampaDiretta: 'false' };

    Stampa(url, parametriChiamata);
}


function anteprima_dettaglio(id_agenda, id_mov_det) {

    var mID = "";
    mID = id_mov_det.toString();


    var ConfigurazioneDaStampare =
        LeggiConfigurazioniStampa(id_agenda, id_mov_det, mID) + ",-1";


    var url = indirizzohttp + "/Anteprima_o_Stampa";
    var parametriChiamata = { ConfigurazioneDaStampare: ConfigurazioneDaStampare, StampaDiretta: 'false' };

    Stampa(url, parametriChiamata);
            
}

function getNumeroEtichetteDato_Dettaglio2(tiporeport, Codice) {

    var rval = "";
    rval = $("#N_Etichette_" + Codice).val();
    return rval;

}

function getNumeroEtichetteDato_Dettaglio(tiporeport, id_mov_det) {

    var rval = "";
    rval = $("#N_Etichette_" + id_mov_det).val();
    return rval;

}



function RipristinaConfigurazioneDaDettagli_Cod() {    

    var FF_Stampa_Dettagli_Cod = "";

    var data = waTable.getData(false, false, true);

    for (var i = 0; i < data.rows.length; i++) {
        FF_Stampa_Dettagli_Cod += data.rows[i].FF_Stampanti_cod_FF_Stampa_Dettagli_COD + ",";
    }
    FF_Stampa_Dettagli_Cod = FF_Stampa_Dettagli_Cod.substring(0, FF_Stampa_Dettagli_Cod.length - 1);


    ajaxAgronica(
        indirizzohttp + "/LeggiConfigurazioneStampa_DatoCod",
        JSON.stringify({ FF_Stampa_Dettagli_Cod: FF_Stampa_Dettagli_Cod }),
        function (risposta) {
            var rsDatiCfg = risposta.RispostaStringa;
            for (var i = 0; i < rsDatiCfg.length; i++) {
                $("#ddl_layout_" + data.rows[i].Sel).val(rsDatiCfg[i].layout_cod);
                $("#ddl_layout_" + data.rows[i].Sel).selectpicker('refresh');

                $("#ddl_lingua_" + data.rows[i].Sel).val(rsDatiCfg[i].lingua_cod);
                $("#ddl_lingua_" + data.rows[i].Sel).selectpicker('refresh');

                $("#ddl_movDet_" + data.rows[i].Sel).val(rsDatiCfg[i].id_mov_det);
                $("#ddl_movDet_" + data.rows[i].Sel).selectpicker('refresh');

                if (rsDatiCfg[i].id_mov_det == 0)
                    $(".watable tr:eq(" + (i + 1).toString() + ") .unique").click();

                //                $("#ddl_stampante_" + data.rows[i].Sel).val(rsDatiCfg[i].stampante_cod);
                //                $("#ddl_stampante_" + data.rows[i].Sel).selectpicker('refresh');

                ImpostaNumeroDiCopie(rsDatiCfg.TipoReport, data.rows[i].Sel, i);                


            }

        }, null);



    }

    function RiportaNumeroDiCopie() {
        var data = waTable.getData(false, false, true);
        for (var i = 0; i < data.rows.length; i++) {
            ImpostaNumeroDiCopie(1, data.rows[i].Sel, i)
        }
    }

    function ImpostaNumeroDiCopie(TipoReport, Codice, i) {
        switch (TipoReport) {
            case 1:
                $("#N_Etichette_" + Codice).val(
                            $($(".watable-col-Numero_di_Etichette_Pedana input")[i]).val()
                        );
                break;
            case 2:
                $("#N_Etichette_" + Codice).val(
                            $($(".watable-col-Numero_di_Etichette_Imballo input")[i]).val()
                        );
                break;
            case 3:
                $("#N_Etichette_" + Codice).val(
                            $($(".watable-col-Numero_di_Etichette_Imballo input")[i]).val()
                        );
                break;
            default:
                $("#N_Etichette_" + Codice).val(1);
                break;
        }
    }


    function getStampanteDiv(id_stampante) {
        var FF_Stampa_Dettagli_Cod = "";

        var data = waTable.getData(true, false);

        for (var i = 0; i < data.rows.length; i++) {
            if (data.rows[i].Sel == id_stampante) {
                return data.rows[i].Nome_Per_Stampa;
            }
        }

        return "";
    }
