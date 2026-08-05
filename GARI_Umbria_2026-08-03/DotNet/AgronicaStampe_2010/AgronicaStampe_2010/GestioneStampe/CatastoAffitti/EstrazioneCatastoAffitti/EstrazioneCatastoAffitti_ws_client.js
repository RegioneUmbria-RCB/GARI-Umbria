
function leggiDS_CatastoAffitti(piva, dataDal, dataAl, centriAziendali) {
    var catastoaffitti = []; //datasource

    var param = kendo.stringify({
        piva: piva,
        dataDal: dataDal,
        dataAl: dataAl,
        centriAziendali: centriAziendali
    });

    ajaxAgronicaSync("EstrazioneCatastoAffitti.aspx/caricaCatastoAffitti",
        param, false,
        function (risposta) {
            catastoaffitti = JSON.parse(risposta.RispostaStringa);
        }, null);

    return catastoaffitti;
}

//funzione per filtro su centri aziendali
function RiempiCentri(options) {

    var centriAziendali = [];
    var piva = $(cIdPiva).val();
    var parametri = kendo.stringify({ "objP_server": objP_server, "PrimaRiga_Flag": false, "PrimaRiga_Text": "", "PrimaRiga_Value": "", "Piva": piva, "Flag_SoloCentriAttivi": false, "Tipo_Value": 2 });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente",
        parametri, false,
        function (risposta) {
            centriAziendali = JSON.parse(risposta.RispostaStringa);
        }, null);

    options.success(centriAziendali);
}