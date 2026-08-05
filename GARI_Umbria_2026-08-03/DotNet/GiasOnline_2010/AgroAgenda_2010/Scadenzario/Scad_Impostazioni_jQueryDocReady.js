
$(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            scadImpostazioniResx.unshift(readResxFile(resxSinglePath, "Scad_Impostazioni_jQueryDocReady.js"));
        });
    }

    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True";

    //Se l'utente è abilitato alla scrittura aggiungo i pulsanti per l'inserimento di nuovi elementi
    if (UtenteAbilitatoScrittura === true) {
        $(".addEditArea").show();
    }

    $('#TxbGGAttesa').kendoNumericTextBox({ decimals: 0, format: "n0" });

    //Carico la griglia degli avvisi
    eseguiRicercaAvvisi();

    //Carico le Aree
    ddlArea_Load();

    creaKendoDropDownList("cmbTipologia", { read: RiempicmbTipologia }, "Nome", "ID_Tipologia");

    //Carico i rapporti contabili
    Elenco_RapCon = Elenco_RapCon_Riempi(true);
    creaKendoMultiselect("cmbRapCon", { read: RiempiDdlRapCon }, "Rapporto_Des", "Cod_Rapporto", null, null, null, ddlRapCon_Change);
    
    

});
