
var indirizzohttp = "./Selezione_SchedaCampagna_BS.aspx";

function Controlli_PreStampa_New(params, isStampaVeneto, isStampaMagazzino) {
    let controlliStampaOK = true;
    let messaggioRispostaControlliStampa = "";

    ajaxAgronicaSync(indirizzohttp + "/Controlli_Pre_StampaNew",
        params, true,
        function (risposta) {
        },
        function (risposta) {
            controlliStampaOK = false
            MessaggioErrore_Bootstrap("Errore: " + risposta.RispostaStringa, "DIV_Messaggi");
        });

    if (controlliStampaOK) {
        var targetUrl = "";

        if (isStampaMagazzino) {
            ajaxAgronicaSync(indirizzohttp + "/Stampa_MagazziniNew",
                params, true,
                function (risposta) {
                    targetUrl = risposta.RispostaStringa;
                },
                function (risposta) {
                });
        }
        else {
            if (isStampaVeneto) {
                ajaxAgronicaSync(indirizzohttp + "/Stampa_VenetoNew",
                    params, true,
                    function (risposta) {
                        targetUrl = risposta.RispostaStringa;
                    },
                    function (risposta) {
                    });
            }
            else {
                ajaxAgronicaSync(indirizzohttp + "/StampaNew",
                    params, true,
                    function (risposta) {
                        targetUrl = risposta.RispostaStringa;
                    },
                    function (risposta) {
                    });
            }
        }

        // TODO check se stampa veneto fare redirect su nuova finestra
        if (targetUrl !== "") {
            if (isStampaVeneto) {
                window.open(targetUrl, "_blank");
            } else {
                window.location.href = targetUrl;
            }
        }
    }
}
function Leggi_Centri_Aziendali(options) {
    var elencoCentriAziendali = null;

    var param = kendo.stringify({
        piva: $(cIds_piva).val(),
        listaSaCod: $(cId_Lista_Sa_Cod_Da_Variabili_Stampe).val(),
        strCodiciTerreno: $(cId_codiciTerreno).val(),
        hashDestinazioniUso: $(cId_hashDestUso).val(),
        objP_server: objP_server,
    });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Centri_Aziendali",
        param,
        false,
        function (risposta) {
            var r = risposta.RispostaStringa;
            var risp = JSON.parse(r);
            elencoCentriAziendali = risp;
        }, null);

    if (elencoCentriAziendali != null)
        options.success(elencoCentriAziendali);
    else
        options.success([]);

}
function Leggi_Magazzini(options) {
    var elencoMagazzini= null;

    var param = kendo.stringify({
        piva: $(cIds_piva).val(),
        objP_server: objP_server,
    });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Magazzini",
        param,
        false,
        function (risposta) {
            var r = risposta.RispostaStringa;
            var risp = JSON.parse(r);
            elencoMagazzini= risp;
        }, null);

    if (elencoMagazzini != null)
        options.success(elencoMagazzini);
    else
        options.success([]);

}

function Leggi_Specie_Vegetali(options) {
    var elencoSpecieVegetali = null;
    var param = kendo.stringify({
        listaVegCod: $(cId_Lista_Veg_Cod_Da_Variabili_Stampe).val(),
        strCodiciTerreno: $(cId_codiciTerreno).val(),
        hashDestinazioniUso: $(cId_hashDestUso).val(),
        objP_server: objP_server,
        objP_utenti: objP_utenti,
    });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie_Vegetali",
        param,
        false,
        function (risposta) {
            var r = risposta.RispostaStringa;
            var risp = JSON.parse(r);
            elencoSpecieVegetali = risp;
        }, null);

    if (elencoSpecieVegetali != null)
        options.success(elencoSpecieVegetali);
    else
        options.success([]);
}
function Leggi_Regione_Appartenenza() {
    var RegioneCod = null;
    var param = kendo.stringify({
        piva: $(cIds_piva).val(),
        sa_cod: $(cId_Sa_Cod).val(),
        objP_server: objP_server,
    });
    ajaxAgronicaSync(indirizzohttp + "/Leggi_Regione_Appartenenza",
        param,
        false,
        function (risposta) {
            RegioneCod = risposta.RispostaStringa;
            if (RegioneCod != undefined && RegioneCod != '')
                Set_KendoDDLValue("ddlRegioni", RegioneCod, "-1");
        }, null);
}
function SalvaDatiGlobalGap(params) {
    var strparam = '{strDati:"' + params + '"}';
    var StrRisposta;
    ajaxAgronicaSync(indirizzohttp + "/GlobalGap_SalvaDatiNew",
        strparam,
        false,
        function (risposta) {
            r = risposta.RispostaStringa;
            if (r != undefined && r != '')
                StrRisposta = r;
        },
        null);
    $("#globalGapWindow").data("kendoWindow").close();
}

function ScaricaFile(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var risposta_byte = null;

    var paramString = kendo.stringify(
        {
            NomeFile: datiRiga.NomeFile,
            NomeSottoCartella: datiRiga.SottoCartella
        });

    var StrRisposta;
    ajaxAgronicaSync(indirizzohttp + "/DownloadFileElencoReport",
        paramString,
        false,
        function (risposta) {
            risposta_byte = risposta.RispostaStringa;
        },
        null);
    if(risposta_byte != null && risposta_byte != undefined) {
        var blob = new Blob([ToArrayBuffer(risposta_byte)], { type: 'application/octet-stream' });
        var link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = datiRiga.NomeFile;
        link.click();
    }
}
function ToArrayBuffer(array) {
    return new Uint8Array(array);
}
function Leggi_ColturePrecedenti() {

    return new Promise((resolve, reject) => {

    var param = null;
    ajaxAgronica(indirizzohttp + "/Carica_Kendo_ColturePrecedenti",
        param,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            if (risp != undefined && risp != '') {
                //let jSonParsed_Kendo = JSON.parse(risp);
                $('#hdKendoTabellaRotazioneValore').val(risp);
                kendoCulture_inizializza("divKendoRotazione", risp);
                resolve(true);
            }
        }, null, undefined, true);
    })
}
function Leggi_ElencoReport() {

    var param = null;
    ajaxAgronicaSync(indirizzohttp + "/CaricaElenco",
        param,
        false,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            if (risp != undefined && risp != '') {
                //let jSonParsed_Kendo = JSON.parse(risp);
                $('#hdKendoTabellaElencoReport').val(risp);
                kendoElencoReport_inizializza("divKendoElencoReport", risp);
            }
        }, null);
}

function CaricaComboSpecieVegetaliDestinazioneUso() {


    var parametri = kendo.stringify({
        "objP_server": objP_server, "objP_utenti": objP_utenti, "Gru_Cod": 0, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": ""
    });
    let ElendoCodici = [];

    ajaxAgronicaSync(pathCoreWS + "Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetaliDestinazioneUso_conFiltroUtente",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            for (var x = 0; x < risp.length; x++) {
                //var elem = risp[x]["veg_cod"].split('|');
                //var CodiEff = 0;
                //if (elem[0] === '0') {
                //    CodiEff = -1 * elem[1];
                //}
                //else { CodiEff = elem[0] };
                ElendoCodici.push({ "CodColtura1": risp[x]["veg_des"], "Coltura1": risp[x]["veg_des"] });
            }
        
            //elenco = risp;
        },
       null);



    return ElendoCodici
}
