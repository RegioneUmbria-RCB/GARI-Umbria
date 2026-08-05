//////////////////////////////////////////////////////////
//      WS CLIENT
//////////////////////////////////////////////////////////
var URL_corews_varieta = "Metaschema/Cultivar.asmx";
var URL_corews_gruppovar = "Metaschema/GruppoVarietale.asmx";
var URL_corews_regolamento = "Metaschema/Regolamenti.asmx";
var URL_pagina = "./CreaProdottoFast.aspx";  



//////////////////////////////////////////////////////////
//      CORE WS
//////////////////////////////////////////////////////////

function cw_CaricaComboTipologieSementi(options) {

   // if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "") {
    var parametri = kendo.stringify({ "FiltroAggiuntivo": "", "Ordinamento": "", "objP_server": objP_server });

        ajaxAgronica(pathCoreWS + "Metaschema/TipologieSementi.asmx/CaricaComboTipologieSementi",
            parametri,
            function (risposta) {
                let str_risp = JSON.parse(risposta.RispostaStringa);
                options.success(str_risp);
            }, null, null, false);
    //} else {
    //    options.success([]);
    //}

}


function cw_CaricaComboSpecie_Semente(options) {

    //'carico nella combo tutte le specie vegetali che e possibile seminare con il tipo di semente scelto
    //CaricaCombo_SpecieVegetale_Semente(Server, Session, Page, _
    //                                            Me.cmb_SpecieVegetale, _
    //                                            Me.cmb_TipologiaSemente.SelectedItem.Value, _
    //                                            0, _
    //                                            True, , , _
    //                                            , , _
    //                                            , , _
    //                                            True, _
    //                                            Veg_Cod, _
    //                                            Sem_Cod)

    if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== 0 && obj_Impianto.veg_cod !== "") {
        var parametri = kendo.stringify({ "FiltroAggiuntivo": "", "Ordinamento": "", "objP_server": objP_server });

        //ByVal Sem_Cod As Integer,
        //ByVal objP_server As String,
        //ByVal objP_utenti As String,
        //Optional ByVal Veg_Cod As Integer = 0,
        //Optional ByVal Flag_PrimaRiga As Boolean = True,
        //Optional ByVal Testo_PrimaRiga As String = "",
        //Optional ByVal Cod_PrimaRiga As String = "",
        //Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
        //Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
        //Optional ByVal FiltroAggiuntivo As String = "",
        //Optional ByVal Ordinamento As String = "",
        //Optional ByVal Flag_FiltroUtente As Boolean = False,
        //Optional ByVal Veg_Cod_daModificare As Integer = 0,
        //Optional ByVal SemCod_Rif_VegCod_daModificare As Integer = 0)

        ajaxAgronica(pathCoreWS + "Metaschema/TipologieSementi.asmx/CaricaCombo_SpecieVegetale_Semente",
            parametri,
            function (risposta) {
                let str_risp = JSON.parse(risposta.RispostaStringa);
                options.success(str_risp);
            }, null, null, false);
    } else {
        options.success([]);
    }

}