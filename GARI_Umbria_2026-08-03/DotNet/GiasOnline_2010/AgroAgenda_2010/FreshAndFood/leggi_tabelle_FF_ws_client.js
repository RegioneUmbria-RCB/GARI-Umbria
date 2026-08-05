/* N.B.   Il percorso è stato impostato così perchè: 
con ../../../ avremmo dovuto passare questo pezzo da ogni pagina perchè è rispetto alla pagina chiamante
con tilde non lo prende
con solo /FreshAndFood/FreshAndFood.asmx prende la porta dell'agenda e non quella del WsCore
mettendo in Configurazione_Siti l'url assoluto completo http:// .... non funzionano le chiamate dal Lan
Quindi nel caso in cui abbiamo i CoreWs pubblicati su una porta diversa dalla porta 80 occorre mettere in configurazione siti
':porta/agronicacorews/'
*/

var indirizzohttp_Leggi_Tabelle_WS = "FreshAndFood/FreshAndFood.asmx"; 
var indirizzohttp_ParamEntrataXSpecieVarieta = "Anagrafica/ParamEntrataXSpecieVarieta.asmx";

var elencoLiquidazioni = null;

function RicercaLiquidazioni(flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoLiquidazioni === null) {

        var param = "{objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti + "', piva: '" + $(cIdPiva).val() + "' }";

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_WS + "/LeggiLiquidazioni",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "id_anagrafica": 0,
                        "descrizione": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoLiquidazioni = risp;
            }, null);

    }

    return elencoLiquidazioni;
}


function RicercaDegrado(piva, veg_cod, cul_cod, reg_cod, data_movimento) {

    let degrado = 0;

    if (veg_cod !== 0) {
        var param = kendo.stringify({
            Piva: piva,
            Veg_Cod: veg_cod,
            Cul_Cod: cul_cod,
            Reg_Cod: reg_cod,
            Data_Riferimento: data_movimento,
            xFiltroAggiuntivo: "",
            objP_server: objP_server
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_ParamEntrataXSpecieVarieta + "/Leggi_Percentuale_Degrado",
                param,
                false,
                function (risposta) {
                    degrado = kendo.parseFloat(risposta.RispostaStringa);
                }, null);
    }

    return degrado;
}

function RicercaTipoRiferimentoPrezzi() {
    let rifPrezzi = [{
        Riferimento_Prezzi_Cod: "S",
        Riferimento_Prezzi_Des:"Data Semina"
    }, {
        Riferimento_Prezzi_Cod: "E",
        Riferimento_Prezzi_Des: "Data Entrata"
        }
    ]
    return rifPrezzi;
}
