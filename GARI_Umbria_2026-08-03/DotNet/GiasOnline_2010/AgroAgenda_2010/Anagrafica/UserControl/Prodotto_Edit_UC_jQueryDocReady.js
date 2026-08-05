
$(document).ready(function () {

});

function Prodotto_Edit_UC_DocReady_Prodotto() {

    if (typeof resxObj !== "undefined" && resxObj !== undefined && resxObj !== null) {
        resxProdottoEditUC = resxObj;
        resxProdottoEditUC.unshift(readResxFile("Anagrafica/UserControl/App_LocalResources/Prodotto_Edit_UC.ascx.resx"));
    }
    else {
        resxProdottoEditUC.push(readResxFile("Anagrafica/UserControl/App_LocalResources/Prodotto_Edit_UC.ascx.resx"));
        resxProdottoEditUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var currentTabName = $(event.target).text();         // active tab
        var previousTabName = $(event.relatedTarget).text();  // previous tab
        var tabId = event.target.id;
        OnTabShow(tabId);
    });

    //Leggo il Modulo Anagrafe
    var listModuliAttivi_anagrafe_log = Leggi_Modulo_Anagrafe();

    if (listModuliAttivi_anagrafe_log.includes(enum_Omni_Modulo_Generazione.FreshFood))
        Modulo_FF = true;

    if (listModuliAttivi_anagrafe_log.includes(enum_Omni_Modulo_Generazione.Zoo))
        Modulo_Zoo = true;

    //Se sono in Lettura,Modifica o Copia carico tutta la pagina 
    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    $(".prodotto_Edit_UC_searchArea").hide();
    $(".prodotto_Edit_UC_modifyArea").show();
    $(".prodotto_Edit_UC_infoArea").show();
    $(".prodotto_Edit_UC_gridArea").hide();

    creaKendoDropDownList("ddl_prodotto_UC_categ_prod", { read: Carica_ddl_categ_prod }, "NomeComune", "Elem_Cod").bind("change", Prodotto_Edit_UC_ddl_prodotto_UC_categ_prod_change);

    // Verifica se prodotto aperto è una materia_prima
    var elem_cod = parseInt($(Controls.xElem_Cod).val());

    var isMateriaPrima = IsMateriaPrima(elem_cod);
    var proprietario = ($(Controls.xProprietario).val() === "True");
    var tipoOperazione = parseInt($(Controls.xTipoOperazione).val());
    var is_alias = ($(xIs_Alias).val() === "True");

    //Controlli della Tab Dati Generali 
    Prodotto_Edit_UC_ddl_prodotto_UC_tipo_varietale_Load(-1);
    Prodotto_Edit_UC_ddl_prodotto_UC_ditta_di_provenienza_Load();
    Prodotto_Edit_UC_ddl_prodotto_UC_Regolamento_Load();
    Prodotto_Edit_UC_ddl_prodotto_UC_specie_veg_Load(null);
    Prodotto_Edit_UC_ddl_prodotto_UC_varieta_Load(-1, 0);
    Prodotto_Edit_UC_ddl_prodotto_UC_unita_mis_def_Load(elem_cod);
    creaKendoDropDownList("ddl_prodotto_UC_mat_prima_priorita_cdg", { read: Ddl_prodotto_UC_mat_prima_priorita_cdg_Read }, "Des", "Val");

    var ddl_Categ_Ris = Prodotto_Edit_UC_ddl_prodotto_UC_categ_ris_Load();
    ddl_Categ_Ris.bind("change", Prodotto_Edit_UC_ddl_prodotto_UC_categ_ris_change);

    Prodotto_Edit_UC_ddl_prodotto_UC_linea_prod_Load(0);
    $("#ddl_prodotto_UC_linea_prod").attr("last_Prodotto_Edit_Linea_Cod", 0);

    creaKendoSwitch($("#cb_prodotto_UC_materia_prima").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);

    $("#cb_prodotto_UC_materia_prima").data("kendoSwitch").bind("change", Prodotto_Edit_UC_materia_prima_change);

    //Switch Alias(parte del GIAS LAN)
    creaKendoSwitch($("#cb_prodotto_UC_alias").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);

    $("#cb_prodotto_UC_alias").data("kendoSwitch").bind("change", Prodotto_Edit_UC_alias_change);

    creaKendoSwitch($("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);

    $("#cb_prodotto_UC_componi_descrizione").data("kendoSwitch").bind("change", Prodotto_Edit_UC_Genera_Descrizione_change);

    //Linea Produzione(parte del GIAS LAN)
    if (tipoOperazione === enum_tipoOperazione.Scrittura) {
        Prodotto_Edit_UC_ddl_prodotto_UC_final_prod_Load(0);

        //Per abilitare la Dropdown Prodotto Base di Riferimento bisogna che il prodotto sia :
        // TRASFORMATO VEGETALE e il Modulo generazione deve essere 2(Fresh and Food)
        if (elem_cod === categorieProdotti.TRASFORMATI_VEGETALI) {
            if (Modulo_FF === true && Is_OMNI === false) {
                Prodotto_Edit_UC_ddl_prodotto_UC_Prodottobase_Load(0, -1, 0);
                $("#ddl_prodotto_UC_Prodottobase").data("kendoDropDownList").enable(true);
            }
            else {
                $("#prodotto_base").hide();
            }
        }
    }

    if (isMateriaPrima === true) {
        if (elem_cod === categorieProdotti.SEMENTI) {
            Prodotto_Edit_UC_ddl_sementi_materiale_Load();
        }

        CaricaProdotto(tipoOperazione);

        if (proprietario === true) {
            //Mostro tutte le tab
        }
        else if (proprietario === false && tipoOperazione !== enum_tipoOperazione.Duplica) {
            //Mostro tutte le tab ma possono essere modificate solo "Storico Prezzi"  e "Dati Contabilità"
            SoloLetturaTab();
        }

        if (is_alias === true)
            Configuratore_Maschera_MateriaPrima_Alias();
    }
    else if (isMateriaPrima === false) {

            CaricaProdottoBancheDati(parseInt($(Controls.xTipoOperazione).val()));
            //Mostro e permetto la modifica solo delle tab "Storico Prezzi" e "Dati Contabilità", mostro
            //la textbox Descrizione e categoria prodotto disabilitate,
            //nascondo anche le dropdown della categoria risorsa, linea produzione e lo switch alias.
            $("#txt_prodotto_UC_Descrizione")[0].disabled = true;
            $("#txt_prodotto_UC_cod_prod")[0].disabled = true;
            $("#codice_esterno").hide();
            $(".categoria_risorsa").hide();
            $(".linea_produzione").hide();
            $(".alias").hide();
            NascondiTab();
    }

    // TODO -> campi obbligatori sempre (mod/ins)
    //Se sto creando un nuovo prodotto mostro l'errore con i campi obbligatori da completare
    if (parseInt($(Controls.xTipoOperazione).val()) !== enum_tipoOperazione.Lettura) {
        ErroreCampiObbligatoriNonCompletati();
    }
    else {
        $("#div_riepilogo_error").hide();
    }

    ImpostaControlliInBaseAOperzione(parseInt($(Controls.xTipoOperazione).val()));

}

function ImpostaControlliInBaseAOperzione(tipoOperazione) {
    $(".prodotto_Edit_UC_infoArea").show();

    //Altrimenti se sono in Scrittura carico solamente la ddl della categoria prodotto
    if (tipoOperazione === enum_tipoOperazione.Scrittura) {
        $(".tabTraduzioni").find("*").css("display", "none");
    }

    if (tipoOperazione != enum_tipoOperazione.Scrittura)
        //Se entro in modifica di un prodotto esistente SALVA & Nuovo non ci deve essere
        $("#prodotto_Edit_UC_SalvaNuovo").hide();
    else
        $("#prodotto_Edit_UC_SalvaNuovo").show();

    if (tipoOperazione === enum_tipoOperazione.Duplica)
        //Se entro in duplica nascondo il pulsante SALVA
        $("#prodotto_Edit_UC_Salva").hide();
    else
        $("#prodotto_Edit_UC_Salva").show();
}


function Prodotto_Edit_UC_DocReady_Lista_Anagrafica(keys) {
    $(".prodotto_Edit_UC_searchArea").show();
    $(".prodotto_Edit_UC_modifyArea").hide();

    Prodotto_Edit_UC_Keys = keys;

    if (Prodotto_Edit_UC_Elenco_Specie === null) 
        Prodotto_Edit_UC_Elenco_Specie = Ricerca_Specie();

    if (Prodotto_Edit_UC_Elenco_Categorie_Magazzino === null) {
        Prodotto_Edit_UC_Elenco_Categorie_Magazzino = CaricaCategorieMagazzinoXUtente();
    }


    if (Prodotto_Edit_UC_Elenco_Categorie_Commerciali === null)
        Prodotto_Edit_UC_Elenco_Categorie_Commerciali = Leggi_Categorie_Commerciali();


    if (KendoDDL("ddl_prodotto_Edit_UC_Categorie") === undefined ||
        KendoDDL("ddl_prodotto_Edit_UC_Categorie") === null ||
        KendoDDL("ddl_prodotto_Edit_UC_Categorie") === "") {

        creaKendoDropDownList("ddl_prodotto_Edit_UC_Categorie", { read: RiempiCategorie }, "NomeComune", "Elem_Cod").bind("change", CategoriaMagChange);
    }
    
    if (KendoMultisel("multiselSpecie") === undefined ||
        KendoMultisel("multiselSpecie") === null ||
        KendoMultisel("multiselSpecie") === "") {

        creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie }, "veg_des", "veg_cod", null, null, null, SpecieChange);
    }

    if (KendoMultisel("multiselVarieta") === undefined ||
        KendoMultisel("multiselVarieta") === null ||
        KendoMultisel("multiselVarieta") === "") {

        creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta }, "Cul_Des", "Cul_Cod", null, null, null, null);
    }

    if (KendoMultisel("multiselCategCommle") === undefined ||
        KendoMultisel("multiselCategCommle") === null ||
        KendoMultisel("multiselCategCommle") === "") {

        creaKendoMultiselect("multiselCategCommle", { read: RiempiCategorieCommerciali }, "Linea_Classe_Des", "Linea_Classe_Cod", null, null, null, null);
    }
 
    

    $("#multiselSpecie").data("kendoMultiSelect").value("");
    $("#multiselVarieta").data("kendoMultiSelect").value("");
    $("#multiselCategCommle").data("kendoMultiSelect").value("");

    var descrizione = $.cookie("Prodotto_Edit_UC_Descrizione");
    var categoria = $.cookie("Prodotto_Edit_UC_Categoria");

    if (descrizione !== undefined &&
        descrizione !== "null" &&
        categoria !== undefined &&
        categoria !== "null") {


        Set_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie", parseInt(categoria));
        $("#txt_descrizione_prodotto").val(descrizione);

        $.cookie("Prodotto_Edit_UC_Descrizione", null, { path: '/' });
        $.cookie("Prodotto_Edit_UC_Categoria", null, { path: '/' });
    }
    else {
        //Imposto la categoria prodotto di default per quello utente 
        Set_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie", Prodotto_Edit_UC_DefaultCategoriaProdotto);
        $("#txt_descrizione_prodotto").val("");
    }


    var specie = $.cookie("Prodotto_Edit_UC_Specie");
    var varieta = $.cookie("Prodotto_Edit_UC_Varieta");
    var categcommle = $.cookie("Prodotto_Edit_UC_CategCommle");
    var regolamento = $.cookie("Prodotto_Edit_UC_Reg");
    var visibilita = $.cookie("Prodotto_Edit_UC_Visi");
    var valorizzati = $.cookie("Prodotto_Edit_UC_Valoriz");

    if (specie !== undefined &&
        specie !== "null") {

        if (specie === "") {
            $("#multiselSpecie").data("kendoMultiSelect").value("");
        }
        else {
            $("#multiselSpecie").data("kendoMultiSelect").value(specie.split(","));

            Prodotto_Edit_UC_Elenco_Varieta = Leggi_Varieta(specie);

            $("#multiselVarieta").data("kendoMultiSelect").dataSource.read();

            $("#multiselVarieta").data("kendoMultiSelect").refresh();
        }


        $.cookie("Prodotto_Edit_UC_Specie", null, { path: '/' });
    }
    else {
        $("#multiselSpecie").data("kendoMultiSelect").value("");
    }

    if (varieta !== undefined &&
        varieta !== "null") {

        if (varieta === "") {
            $("#multiselVarieta").data("kendoMultiSelect").value("");
        }
        else {
            $("#multiselVarieta").data("kendoMultiSelect").value(varieta.split(","));
        }


        $.cookie("Prodotto_Edit_UC_Varieta", null, { path: '/' });
    }
    else {
        $("#multiselVarieta").data("kendoMultiSelect").value("");
    }

    if (categcommle !== undefined &&
        categcommle !== "null") {

        if (categcommle === "") {
            $("#multiselCategCommle").data("kendoMultiSelect").value("");
        }
        else {
            $("#multiselCategCommle").data("kendoMultiSelect").value(categcommle.split(","));
        }


        $.cookie("Prodotto_Edit_UC_CategCommle", null, { path: '/' });
    }
    else {
        $("#multiselCategCommle").data("kendoMultiSelect").value("");
    }

    if (regolamento !== undefined &&
        regolamento !== "null") {

        if (parseInt(regolamento)=== 0)
            $("#FiltroReg_TUTTI").attr('checked', 'checked');

        if (parseInt(regolamento) === 1)
            $("#FiltroReg_CONV").attr('checked', 'checked');

        if (parseInt(regolamento) === 4)
            $("#FiltroReg_BIO").attr('checked', 'checked');

        $.cookie("Prodotto_Edit_UC_Reg", null, { path: '/' });
    }
    else {
        $("#FiltroReg_TUTTI").attr('checked', 'checked');
    }

    if (visibilita !== undefined &&
        visibilita !== "null") {

        if (parseInt(visibilita) === -99)
            $("#FiltroVisi_TUTTI").attr('checked', 'checked');

        if (parseInt(visibilita) === 0)
            $("#FiltroVisi_Privato").attr('checked', 'checked');

        if (parseInt(visibilita) === -1)
            $("#FiltroVisi_Pubblico").attr('checked', 'checked');

        $.cookie("Prodotto_Edit_UC_Visi", null, { path: '/' });
    }
    else {
        $("#FiltroVisi_TUTTI").attr('checked', 'checked');
    }

    if (valorizzati !== undefined &&
        valorizzati !== "null") {

        if (parseInt(valorizzati) === -1)
            $("#FiltroValoriz_TUTTI").attr('checked', 'checked');

        if (parseInt(valorizzati) === 0)
            $("#FiltroValoriz_No").attr('checked', 'checked');

        if (parseInt(valorizzati) === 1)
            $("#FiltroValoriz_Si").attr('checked', 'checked');

        $.cookie("Prodotto_Edit_UC_Valoriz", null, { path: '/' });
    }
    else {
        $("#FiltroValoriz_TUTTI").attr('checked', 'checked');
    }

    //var urlParams = new URLSearchParams(window.location.search);
    //var TipoOperazione = urlParams.get('o');
    //var Descrizione = urlParams.get('prodotto_des');
    //var CategoriaProdotto = urlParams.get('elem_cod');

    //if (TipoOperazione !== null &&
    //    TipoOperazione !== undefined &&
    //    TipoOperazione !== "" &&
    //    Descrizione !== null &&
    //    Descrizione !== undefined &&
    //    Descrizione !== "" &&
    //    CategoriaProdotto !== null &&
    //    CategoriaProdotto !== undefined &&
    //    CategoriaProdotto !== "") {

    //    if (TipoOperazione === enum_tipoOperazione.Lettura ||
    //        TipoOperazione === enum_tipoOperazione.Modifica ||
    //        TipoOperazione === enum_tipoOperazione.Duplica) {

    //        $("#txt_descrizione_prodotto").val(Descrizione);
    //        Set_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie", parseInt(CategoriaProdotto));
    //    }
    //}
    //else {
    //    //Imposto la categoria magazzino di default per quello utente 
    //    Set_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie", Prodotto_Edit_UC_DefaultCategoriaProdotto);
    //    $("#txt_descrizione_prodotto").val("");
    //}


    if ($("#ddl_prodotto_Edit_UC_Categorie").data("kendoDropDownList") !== undefined &&
        $("#ddl_prodotto_Edit_UC_Categorie").data("kendoDropDownList") !== null &&
        $("#ddl_prodotto_Edit_UC_Categorie").data("kendoDropDownList") !== "") {


        if (categorieProdotti_FiltrabiliXSpecieVarieta.includes(parseInt(Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie")))) {
        

            $("#ddl_Ricerca_XSpecie").show();
            $("#ddl_Ricerca_XVarieta").show();

            $("#multiselSpecie").data("kendoMultiSelect").enable(true);
            $("#multiselVarieta").data("kendoMultiSelect").enable(true);
        }
        else {
            $("#ddl_Ricerca_XSpecie").hide();
            $("#ddl_Ricerca_XVarieta").hide();

            $("#multiselSpecie").data("kendoMultiSelect").enable(false);
            $("#multiselVarieta").data("kendoMultiSelect").enable(false);

            $("#multiselSpecie").data("kendoMultiSelect").value("");
            $("#multiselVarieta").data("kendoMultiSelect").value("");
        }

        if (categorieProdotti_FiltrabiliXCategoriaCommerciale.includes(parseInt(Get_KendoDDLValue("ddl_prodotto_Edit_UC_Categorie"))) &&
            $("#multiselCategCommle").data("kendoMultiSelect").dataSource.data().length > 0) {

            $("#ddl_Ricerca_XCategCommle").show();

            $("#multiselCategCommle").data("kendoMultiSelect").enable(true);
        }
        else {

            $("#ddl_Ricerca_XCategCommle").hide();

            $("#multiselCategCommle").data("kendoMultiSelect").enable(false);

            $("#multiselCategCommle").data("kendoMultiSelect").value("");
        }
    }

    // Se la funzione restituisce true, toggle mostra l'elemento, se restituisce false lo nasconde
    $("#lbl_prodotto_Edit_UC_descrizione_3_caratteri").toggle(FiltroMinCaratteriPerRicerca());

    $("#prodotto_Edit_UC_Ricerca").click(function (e) {

        Ricerca_Prodotti(Prodotto_Edit_UC_Keys);

    });

}

//TAB PRODOTTI
//Se premi invio parte la ricerca oppure la creazione di un nuovo prodotto
$(document).keypress(function (e) {
    if (e.which == '13' && $(".prodotto_Edit_UC_searchArea").is(":visible")) {
        if ($("#modalNuovo").is(":visible") &&
            $("#tipoNuovo").data("kendoDropDownList").value() === "15") {

            $("#btn_nuovo").trigger("click");
        }
        else {
            $("#prodotto_Edit_UC_Ricerca").trigger("click");
        }
        e.preventDefault();
    }
    else if (e.which == '13' && $(".prodotto_Edit_UC_infoArea").is(":visible")){
        e.preventDefault();
        return false;
    }
});

//Imposto in sola lettura i tab "Dati Generali",
//"Traduzioni", "Parametri Qualitativi" 
function SoloLetturaTab() {

    let elem_cod = parseInt($(Controls.xElem_Cod).val());

    Disabilita_Controlli_infoProdotto_Edit();
    OnTabShow("a_tab_prodotto_UC_dati_tecnici");

    if (elem_cod === categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE) {
        OnTabShow("a_tab_prodotto_UC_parametri_qualitativi");
    }

    OnTabShow("a_tab_prodotto_UC_traduzioni");
}

//Nascondo tutti i tab tranne "Storico Prezzi" e "Dati contabilità"
function NascondiTab() {
    $("#a_tab_prodotto_UC_parametri_qualitativi").hide();
    //Nascondo anche il tab Configurazione dato che è collegato alle Materie_Prime
    $("#a_tab_prodotto_UC_configurazione").hide();
    $("#a_tab_prodotto_UC_dati_tecnici").hide();
    $(".nav-tabs li.tabDatiTecnici.active").removeClass("active");
    $(".tab-pane.fade.in.active").removeClass("active");
    $("#a_tab_prodotto_UC_traduzioni").hide();
    $("#a_tab_prodotto_UC_alias").hide();
    $(".nav-tabs li.tabStoricoPrezzi").addClass("active");
    $(".tab-pane.StoricoPrezzi.fade.in").addClass("active");

    //Carico la griglia storico prezzi
    OnTabShow("a_tab_prodotto_UC_storico_prezzi");
}


function ErroreCampiObbligatoriNonCompletati() {

    nascondi_riepilogo_error();


    //Se il codice prodotto viene generato dal codice_esterno allora nascondo anche il suo messaggio di errore
    if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === false) {
        $("#txt_prodotto_UC_cod_prod").keyup(function () {

            if ($("#txt_prodotto_UC_cod_prod").val() != "") {
                $('.voce_1').hide();
                //$("#txt_prodotto_UC_cod_prod").parent().children(".required").css('border', '1px solid #428BCA');
                $('#txt_prodotto_UC_cod_prod').removeClass('erroreCampiObbigatori');
            }
            else {
                $('.voce_1').show();
                //$("#txt_prodotto_UC_cod_prod").parent().children(".required").css('border', '1px solid #D41E1A');
                $('#txt_prodotto_UC_cod_prod').addClass('erroreCampiObbigatori');
            }
            nascondi_riepilogo_error();
        });
    }else if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) {
        $("#txt_prodotto_UC_cod_est").keyup(function () {

            if ($("#txt_prodotto_UC_cod_est").val() != "") {
                $('.voce_3').hide();
                $('#txt_prodotto_UC_cod_est').removeClass('erroreCampiObbigatori');
            }
            else {
                $('.voce_3').show();
                $('#txt_prodotto_UC_cod_est').addClass('erroreCampiObbigatori');
            }
            nascondi_riepilogo_error();
        });
    }


    $("#txt_prodotto_UC_Descrizione").keyup(function () {
        if ($("#txt_prodotto_UC_Descrizione").val() != "") {
            $('.voce_2').hide();
            //$("#txt_prodotto_UC_Descrizione").parent().children(".required").css('border', '1px solid #428BCA');
            $('#txt_prodotto_UC_Descrizione').removeClass('erroreCampiObbigatori');
        }
        else {
            $('.voce_2').show();
            //$("#txt_prodotto_UC_Descrizione").parent().children(".required").css('border', '1px solid #D41E1A');
            $('#txt_prodotto_UC_Descrizione').addClass('erroreCampiObbigatori');
        }
        nascondi_riepilogo_error();
    });

}

function nascondi_riepilogo_error() {
    var disabilita_salvataggio = true;

    //Se il codice prodotto viene generato dal codice_esterno allora nascondo anche il suo messaggio di errore
    if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === false) {
        if ($("#txt_prodotto_UC_cod_prod").val() === "") {
            $("#div_riepilogo_error").show();
            $('.voce_1').show();
        }
        else {
            $(".voce_1").hide();
            //$("#txt_prodotto_UC_cod_prod").parent().children(".required").css('border', '1px solid #428BCA');
            $('#txt_prodotto_UC_cod_prod').removeClass('erroreCampiObbigatori');
        }
    }else if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) {
        if ($("#txt_prodotto_UC_cod_est").val() === "") {
            $("#div_riepilogo_error").show();
            $('.voce_3').show();
            $('#txt_prodotto_UC_cod_est').addClass('erroreCampiObbigatori');
        }
        else {
            $(".voce_3").hide();
            $('#txt_prodotto_UC_cod_est').removeClass('erroreCampiObbigatori');
        }
    }
 
    if ($("#txt_prodotto_UC_Descrizione").val() === "") {
        $("#div_riepilogo_error").show();
        $('.voce_2').show();
        //$("#txt_prodotto_UC_Descrizione").parent().children(".required").css('border', '1px solid #D41E1A');
        $('#txt_prodotto_UC_Descrizione').addClass('erroreCampiObbigatori');

    }
    else {
        $(".voce_2").hide();
        //$("#txt_prodotto_UC_Descrizione").parent().children(".required").css('border', '1px solid #428BCA');
        $('#txt_prodotto_UC_Descrizione').removeClass('erroreCampiObbigatori');
    }
    if (!$(".voce_1").is(":visible") && !$(".voce_2").is(":visible") && !$(".voce_3").is(":visible")) {
        $("#div_riepilogo_error").hide();
        disabilita_salvataggio = false;
    }
    else {
        $("#div_riepilogo_error").show();
    }
    return disabilita_salvataggio;
}