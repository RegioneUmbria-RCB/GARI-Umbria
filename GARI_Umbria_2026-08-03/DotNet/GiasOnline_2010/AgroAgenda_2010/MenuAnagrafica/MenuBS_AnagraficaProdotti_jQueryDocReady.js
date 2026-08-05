let tabStrip;
var kendoDialogInfoCatasto;

var dataFiltroValidita = new Date();


var permesso_prodotti_read;
var permesso_prodotti_write;
let proprietario = false;


var permesso_sblocca;

var flag_disciplinareprivato;

var permesso_bdn_read;
var permesso_bdn_write;

var permesso_modificaMultipla;

let treeView;

var menuBSAnagraficaResx; // Oggetto che viene valorizzato con le traduzioni per la lingua corrente

$(document).ready(function () {

    docR();

});




(function ($) {
    $.fn.regex = function (pattern, fn, fn_a) {
        var fn = fn || $.fn.text;
        return this.filter(function () {
            return pattern.test(fn.apply($(this), fn_a));
        });
    };
})(jQuery);




function GISRicercaSempliceAvvia() {
    let filterText = $("#filterText").val();
    RicercaAlberoAnagrafica(filterText, 0);
}

var filterTimer = 0;



async function docR() {

    if (modificaMultipla != "" && modificaMultipla != "0") {
        permesso_modificaMultipla = true;
    } else {
        permesso_modificaMultipla = false;
    }

    $("#" + hidden_objP_agenda_ClientID).change(function () {
        objP_agenda = $(this).html();
    });

    if (!menuBSAnagraficaResx) {
        MenuBSAnagraficaResxLeggi();
    }

    /* Variabili globali definite in MenuBS_AnagraficaProdotti.js, valorizzo qua per le traduzioni: */
    JsnRegolamento = [
        { text: Traduzione(menuBSAnagraficaResx, "Tutti", "Tutti"), value: "0" },
        { text: Traduzione(menuBSAnagraficaResx, "Convenzionale", "Convenzionale"), value: "1" },
        { text: Traduzione(menuBSAnagraficaResx, "Biologico", "Biologico"), value: "4" }
    ];

    JsnVisibilita = [
        { text: Traduzione(menuBSAnagraficaResx, "Tutti", "Tutti"), value: "-99" },
        { text: Traduzione(menuBSAnagraficaResx, "Privato", "Privato"), value: "0" },
        { text: Traduzione(menuBSAnagraficaResx, "Pubblico", "Pubblico"), value: "-1" }
    ];

    JsnValorizzati = [
        { text: Traduzione(menuBSAnagraficaResx, "Tutti", "Tutti"), value: "-1" },
        { text: Traduzione(menuBSAnagraficaResx, "Si", "Si"), value: "1" },
        { text: Traduzione(menuBSAnagraficaResx, "No", "No"), value: "0" }
    ];

    $('#txt_nuovoPiva').val(JSON.parse(objP_agenda).Piva);

   
    // Nascondo il menu con le operazioni principali
    // $('#menu_princ_operazioni').hide();

    $('#btn_nuovo').hide();

    $("body").on("click", "#tabDati_due li a", function () {
        $('#tabDati').find('.active').removeClass('active');
    });
    $("body").on("click", "#tabDati li a", function () {
        $('#tabDati_due').find('.active').removeClass('active');
    });

   


    Prodotto_Edit_UC_DocReady_Lista_Anagrafica(null);

   

   
    $("#filterContainer").css("width", "-webkit-fill-available");
    if (config_albero != "2") $("#gisMenuRicercaAvanzata").hide();
  
    //anagrafica
    $("#slide-in-share").hide();
    var slide = kendo.fx($("#slide-in-share")).slideIn("left");
    var slideVisible = false;

    $("#slide-in-handle").click(function (e) {
        if (slideVisible) {
            slide.reverse();

            storeTreeResizeParams();
        } else {
            slide.play();
            resetResizeSettings();
        }
        slideVisible = !slideVisible;

        let AnagTree = AlberoAnagrafica();
        AnagTree.setVisible(!slideVisible);

        e.preventDefault();
    });

    $(window).resize(function () {
        if (!slideVisible) {
            slide.play();
            resetResizeSettings();

            slideVisible = !slideVisible;

            let AnagTree = AlberoAnagrafica();
            AnagTree.setVisible(!slideVisible);
        }
    });

    //Case insensitive JQuery contains
    jQuery.expr[':'].ci_contains = function (a, i, m) {
        return jQuery(a).text().toUpperCase()
            .indexOf(m[3].toUpperCase()) >= 0;
    };

    //if ($.cookie("MenuBS_Anagrafica.filterData") != undefined) {
    //    dataFiltroValidita = kendo.parseDate($.cookie("MenuBS_Anagrafica.filterData"));
    //}

    if (storageExistItem("MenuBS_Anagrafica.filterData")) {
        dataFiltroValidita = kendo.parseDate(storageGetItem("MenuBS_Anagrafica.filterData"));
    }

    let dateFinestraTemporale = await ws_dateValiditaAgenda();

    if (dataFiltroValidita <= kendo.parseDate(dateFinestraTemporale.Validita_Fine) && dataFiltroValidita >= kendo.parseDate(dateFinestraTemporale.Validita_Fine)) {

    } else if (dataFiltroValidita > kendo.parseDate(dateFinestraTemporale.Validita_Fine)) {
        dataFiltroValidita = dateFinestraTemporale.Validita_Fine
    } else if (dataFiltroValidita < kendo.parseDate(dateFinestraTemporale.Validita_Inizio)) {
        dataFiltroValidita = dateFinestraTemporale.Validita_Inizio
    }


    // filtra per data validita
    $("#filterData").kendoDatePicker({
        value: dataFiltroValidita,
        footer: "#: kendo.toString(data, 'd')#",
        max: dateFinestraTemporale.Validita_Fine,
        min: dateFinestraTemporale.Validita_Inizio,
        change: function () {
            let dataFiltro = kendo.toString(this.value(), 'd');
            if (dataFiltro == null) {
                this.value("");
            }
            storageSetItem("MenuBS_Anagrafica.filterData", dataFiltro)
            ImpostaObjP_Agenda(0, dataFiltro, false, function () {
                AggiornaDati();
                AggiornaAlberoAnagrafica();
            });

        }
    });

    
    $("#filterText").on('keydown', function (e) {
        if (e.keyCode === 13) { e.preventDefault(); return false; }
    });

    $("#RicercaSempliceRipulisci").click(function () {
        GISRicercaSempliceAvvia();
    });

    $("#gisMenuRicercaSempliceAvvia").click(function () {
        GISRicercaSempliceAvvia();
    });

    $("#gisMenuRicercaAvanzataAvvia").click(function () {
        RicercaAlberoAnagrafica("", 1);
    });

    $("#filterContainer .clearFilter").on("click", function (e) {
        $("#filterText").val("");
        setTimeout(function () {
            GISRicercaSempliceAvvia();
        }, 10);
    });
    $("body").on("click", ".aggiorna", function () {
        AggiornaDati();
    });

    let permessi_pagina = await Permessi_Pagina();

    let promises = new Array();
    promises.push(Agro_LeggiPermessoUtente(username_master, 56, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 56, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 57, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 57, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 58, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 58, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 59, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 59, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 60, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 60, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 61, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 61, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 62, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 62, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 63, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 63, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 64, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 64, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 65, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 201, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 201, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 246, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 246, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 414, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 414, 2));

    promises.push(Agro_LeggiPermessoUtente(username_master, 99, 0));
    promises.push(Agro_LeggiPermessoUtente(username_master, 99, 2));

 

    let resp = await Promise.all(promises);

   

    permesso_sblocca = resp[18];

   
    permesso_prodotti_read = resp[23];
    permesso_prodotti_write = resp[24];

    permesso_bdn_read = resp[25];
    permesso_bdn_write = resp[26];

    flag_disciplinareprivato = resp[27];

    

    

    var qsVisibilita = Request_QueryString("visibilita");

   

    if (permesso_prodotti_write && qsVisibilita !== "2") {
        $('#tipoNuovo').append('<option value="1">' + Traduzione(menuBSAnagraficaResx, 'Prodotto', 'PRODOTTO') + '</option>');
    }

    var qsGestione = Request_QueryString("gestione");

   



    var qsGruppoEdit = Request_QueryString("GruppoEdit");
    if (qsGruppoEdit !== null && qsGruppoEdit !== "") {
        ImpostaSelezioneVisibilita(qsGruppoEdit);
    } else {

        var tab;

        tab = getParameterByName("tab");

        if (tab == null) {
            let tabVisibilita = $.cookie("MenuBS_Anagrafica.tabDati" + qsVisibilita);
            tab = tabVisibilita;
        }

    }

    

    $("body").on("click", ".jstree-clicked", function () {
        //$('#menu_princ_operazioni').hide();

        var v = $(this).parent().attr('id');
        ultimaSelezioneAlbero = v;
        console.log(v);

        var valori = "";
        // Rimuovo la classe chiave nei bottoni
        $('#btn_info_s').removeAttr("chiave");
        $('#btn_modifica_s').removeAttr("chiave");
        $('#btn_cancella_s').removeAttr("chiave");
        // Rimuovo il tipo nei bottoni
        $('#btn_info_s').removeAttr("tipo");
        $('#btn_modifica_s').removeAttr("tipo");
        $('#btn_cancella_s').removeAttr("tipo");
        let tab = 0;
        
        switch (parseInt(v.split("§")[0])) {
            case 2:
                //impresa
                $('#tabDati a[href="#divKendoAzienda"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if (v.split("§")[1] != 0) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1];

                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 2);
                    $('#btn_modifica_s').attr("tipo", 2);
                    $('#btn_cancella_s').attr("tipo", 2);

                    chiave_selezionata = valori;

                }

                tab = ImpostaSelezione(1, 1);

                break;
            case 3:
                //centro
                $('#tabDati a[href="#divKendoCentro"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if (v.split("§")[2] != 0) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 3);
                    $('#btn_modifica_s').attr("tipo", 3);
                    $('#btn_cancella_s').attr("tipo", 3);

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(2, 1);

                break;
            case 4:
                //campo
                $('#tabDati a[href="#divKendoCampo"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if (v.split("§")[3] != 0) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[3];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 4);
                    $('#btn_modifica_s').attr("tipo", 4);
                    $('#btn_cancella_s').attr("tipo", 4);

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(3, 1);

                break;
            case 5:
                //appezzamento
                $('#tabDati a[href="#divKendoAppezzamento"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if (v.split("§")[4] != 0) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[4];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 5);
                    $('#btn_modifica_s').attr("tipo", 5);
                    $('#btn_cancella_s').attr("tipo", 5);

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(4, 1);

                break;
            case 6:
            case 7:
            case 8:
                //impianti
                $('#tabDati a[href="#divKendoImpianto"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if ((v.split("§")[4] != 0) && (v.split("§")[5] != 0)) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[4] + "_" + v.split("§")[5];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", parseInt(v.split("§")[0]));
                    $('#btn_modifica_s').attr("tipo", parseInt(v.split("§")[0]));
                    $('#btn_cancella_s').attr("tipo", parseInt(v.split("§")[0]));

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(5, 1);

                break;
            case 18:
            case 10:
                //catasto
                $('#tabDati a[href="#divKendoCatasto"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if ((v.split("§")[6] != 0) && (v.split("§")[7] != 0) && (v.split("§")[8] != 0) && (v.split("§")[10] != 0) && (v.split("§")[11] != 0)) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[7] + "_" + v.split("§")[8] + "_" + v.split("§")[9] + "_" + v.split("§")[10] + "_" + v.split("§")[11] + "_" + v.split("§")[12] + "_" + v.split("§")[6];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 10);
                    $('#btn_modifica_s').attr("tipo", 10);
                    $('#btn_cancella_s').attr("tipo", 10);

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(6, 1);

                break;

            case 36:
                // Contatti
                $('#tabDati a[href="#divKendoContatto"]').tab('show');
                tab = ImpostaSelezione(11, 1);
                break;
            case 22:
                //fabbricati
                $('#tabDati a[href="#divKendoFabbricato"]').tab('show');


                //@Paolo
                // Gestione selezione singolo record
                if (v.split("§")[14] != 0) {
                    //$('#menu_princ_operazioni').show();

                    valori = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[14];
                    // Aggiungo la chiave generata a ciascun bottone operazione
                    $('#btn_info_s').attr("chiave", valori);
                    $('#btn_modifica_s').attr("chiave", valori);
                    $('#btn_cancella_s').attr("chiave", valori);

                    // Aggiungo il tipo a ciascun bottone operazione
                    $('#btn_info_s').attr("tipo", 22);
                    $('#btn_modifica_s').attr("tipo", 22);
                    $('#btn_cancella_s').attr("tipo", 22);

                    chiave_selezionata = valori;
                }

                tab = ImpostaSelezione(10, 1);

                break;

            case 35:
                // Macchine
                $('#tabDati a[href="#divKendoMacchina"]').tab('show');
                tab = ImpostaSelezione(12, 1);
                break;
        }

       // tabStrip.select(tab);

    });


    $("body").on("click", "#btn_filtrino", function () {
        GotToFiltrino();
    });

    $("#btn_nuovo").click(function () {
        try {
            ProcediNuovo();
        } catch (e) {
            console.log("Errore:" + e.message);
        }
    });

   

}

function ProcediNuovo() {

    var v = parseInt($('#tipoNuovo').val());
    var chiave = Get_KendoDDLValue("ddl_categorie_prodotti");
    gotoNewElement(v, chiave);

}

function Permessi_Pagina() {
    return new Promise((resolve, reject) => {
        let storage_key = "Permessi_Pagina";

        if (!storageExistItem(storage_key)) {
            var parametri = kendo.stringify({});

            ajaxAgronica("MenuBS_AnagraficaProdotti.aspx/Permessi_Pagina",
                parametri,
                function (risposta) {
                    storageSetItem(storage_key, risposta.RispostaStringa);
                    resolve(risposta.RispostaStringa);
                }, null, null, false);
        } else {
            resolve(storageGetItem(storage_key));
        }

    });
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

window.mobilecheck = function () {
    var check = false;
    if (document.documentElement.clientWidth < 992) {
        check = true;
    }
    return check;
};

function autoFitSeMobile(e) {
    if (window.mobilecheck()) {
        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        for (var i = 0; i < grid.columns.length; i++) {
            grid.autoFitColumn(i);
        }
    }
}

function MenuBSAnagraficaResxLeggi() {
    var letturaRiuscita = false;
    ajaxAgronicaSync("../Localization.aspx/RitornaRisorseBS", JSON.stringify({ files: "App_GlobalResources/AgronicaAgenda_2010.resx" }), false,
        function (risposta) {
            try {
                menuBSAnagraficaResx = JSON.parse(risposta.RispostaStringa);
                letturaRiuscita = true;
                console.log("MenuBSAnagraficaResxLeggi...letto correttamente.");
            } catch (e) {
                console.log("MenuBSAnagraficaResxLeggi...errori in fase di parse del json.");
            }

        }, function (risposta) {
            console.log("MenuBSAnagraficaResxLeggi...errori in fase di lettura.");
        });
    return letturaRiuscita;
}

function ws_dateValiditaAgenda() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({});

        ajaxAgronica("MenuBS_AnagraficaProdotti.aspx/dateValiditaAgenda",
            parametri,
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    });
}





$(document).ready(function () {

});

function Prodotto_Edit_UC_DocReady_Prodotto() {




    $('.nav-tabs a').on('shown.bs.tab', function (event) {
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
    $('#txt_descrizione_prodotto').value = '';
    creaKendoDropDownListWithData("ddl_Regolamento", JsnRegolamento);
    creaKendoDropDownListWithData("ddl_Visibilita", JsnVisibilita);
    creaKendoDropDownListWithData("ddl_Valorizzati", JsnValorizzati);
    Set_KendoDDLValue("ddl_Regolamento", "0");
    Set_KendoDDLValue("ddl_Visibilita", "-99");
    Set_KendoDDLValue("ddl_Valorizzati", "-1");

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

        Set_KendoDDLValue("ddl_Regolamento", parseInt(regolamento));
        $.cookie("Prodotto_Edit_UC_Reg", null, { path: '/' });
    }
    else {
        Set_KendoDDLValue("ddl_Regolamento", "0");
    }

    if (visibilita !== undefined &&
        visibilita !== "null") {

        Set_KendoDDLValue("ddl_Visibilita", parseInt(visibilita));
        $.cookie("Prodotto_Edit_UC_Visi", null, { path: '/' });
    }
    else {
        Set_KendoDDLValue("ddl_Visibilita", "-99");
    }

    if (valorizzati !== undefined &&
        valorizzati !== "null") {

        Set_KendoDDLValue("ddl_Valorizzati", parseInt(valorizzati));

        $.cookie("Prodotto_Edit_UC_Valoriz", null, { path: '/' });
    }
    else {
        Set_KendoDDLValue("ddl_Valorizzati", "-1");
    }

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
    else if (e.which == '13' && $(".prodotto_Edit_UC_infoArea").is(":visible")) {
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
    } else if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) {
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
    } else if (Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno === true) {
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