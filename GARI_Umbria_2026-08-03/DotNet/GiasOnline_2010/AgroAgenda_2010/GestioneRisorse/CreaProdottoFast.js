// CARICA COMBO
var Cmb_TipSem;

function CaricaCmb_TipologiaSem(value) {  
        let onLoad = true;
        Cmb_TipSem = $("#Cmb_TipSem").kendoDropDownList({
            filter: "contains",
            dataTextField: "sem_des",
            dataValueField: "sem_cod",
            dataSource: { transport: { read: cw_CaricaComboTipologieSementi } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);              
            },
            change: function (e) {                
                obj_Prodotto.sem_cod = this.value();     
                //chiamare il caricamento della specie
            }
        }).data("kendoDropDownList");   
}




function CaricaComboCultivar_conFiltroUtente(options) {

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "") {
        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Veg_Cod": obj_Impianto.veg_cod, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

        ajaxAgronica(pathCoreWS + "Metaschema/Cultivar.asmx/CaricaComboCultivar_conFiltroUtente",
            parametri,
            function (risposta) {
                let specievegetali = JSON.parse(risposta.RispostaStringa);
                options.success(specievegetali);
            }, null, null, false);
    } else {
        options.success([]);
    }

}