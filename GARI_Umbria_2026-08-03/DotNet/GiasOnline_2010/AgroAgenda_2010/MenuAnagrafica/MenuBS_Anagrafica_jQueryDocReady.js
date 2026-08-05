let tabStrip;
var kendoDialogInfoCatasto;

var dataFiltroValidita = new Date();

var permesso_impresa_read;
var permesso_impresa_write;

var permesso_centro_read;
var permesso_centro_write;

var permesso_campo_read;
var permesso_campo_write;

var permesso_appezzamento_read;
var permesso_appezzamento_write;

var permesso_impianto_read;
var permesso_impianto_write;

var permesso_fabbricato_read;
var permesso_fabbricato_write;

var permesso_particelle_read;
var permesso_particelle_write;

var permesso_contatti_read;
var permesso_contatti_write;

var permesso_prodotti_read;
var permesso_prodotti_write;

var permesso_macchine_read;
var permesso_macchine_write;

var permesso_sblocca;

var permesso_modifica_contatti_pubblici_read;
var permesso_modifica_contatti_pubblici_write;

var permesso_gestione_macchine_pubbliche_read;
var permesso_gestione_macchine_pubbliche_write;

var permesso_appezzamento_ereditatore;
var permesso_impianto_ereditatore;

var flag_disciplinareprivato;

var permesso_bdn_read;
var permesso_bdn_write;

var permesso_modificaMultipla;

let treeView;

var menuBSAnagraficaResx; // Oggetto che viene valorizzato con le traduzioni per la lingua corrente

$(document).ready(function () {

    docR();

});


function setIdRigaSelezionataDellAlbero(value) {
    $('#rigaSelezionataAlbero').val(value);
}

function getIdRigaSelezionataDellAlbero() {
    return $('#rigaSelezionataAlbero').val();
}

(function ($) {
    $.fn.regex = function (pattern, fn, fn_a) {
        var fn = fn || $.fn.text;
        return this.filter(function () {
            return pattern.test(fn.apply($(this), fn_a));
        });
    };
})(jQuery);

function testCatasto1(contenutoDaCercare, test1) {

    let prov = $("#filterAvanzatoProvText").val().toString();
    let com = $("#filterAvanzatoComText").val().toString();
    let foglio = $("#filterAvanzatoFoglioText").val().toString();
    let particella = $("#filterAvanzatoParticellaText").val().toString();


    //{AR : A291 : ANGHIARI : __ : ____79 : ___120 : __} ..... 1,5165 [ha] ..... Proprietà

    let vContenutoDaCercare = contenutoDaCercare.split(":");

    if (!(vContenutoDaCercare.length >= 6 && contenutoDaCercare[0] === "{")) {
        return false;
    }

    let rval = true;
    var re;
    if (prov !== "") {
        re = new RegExp(prov, "i");
        rval = rval && re.test(vContenutoDaCercare[0]);
    }

    if (com !== "") {
        re = new RegExp(com, "i");
        rval = rval && re.test(vContenutoDaCercare[1] + " : " + vContenutoDaCercare[2]);
    }

    if (foglio !== "") {
        re = new RegExp(foglio, "i");
        rval = rval && re.test(vContenutoDaCercare[4]);
    }

    if (particella !== "") {
        re = new RegExp(particella, "i");
        rval = rval && re.test(vContenutoDaCercare[5]);
    }

    return rval;

}

(function ($) {
    $.fn.testCatasto = function (fn, fn_a) {
        var fn = fn || $.fn.text;
        return this.filter(function () {
            return testCatasto1(fn.apply($(this), fn_a));
        });
    };
})(jQuery);


(function ($) {
    var GisTreeView = kendo.ui.TreeView.extend({
        options: {
            name: "GisTreeView",
            visible: false,
            needLoad: true
        },
        setVisible: function (visible) {
            this.options.visible = visible;
            if (this.options.visible && this.options.needLoad) {
                this.load();
            }
        },
        checkFromId: function (id) {

            let treeitem = this.dataSource.get(id);
            if (treeitem === undefined) {
                return false;
            }

            treeitem.set("checked", true);
            let li = this.findByUid(treeitem.uid);
            this.select(li);

            //scroll dell'albero fino ad elemento selezionato
            try {
                let eleTop = $(li).offset().top;
                let treeScrollTop = this.element.scrollTop();
                let treeTop = this.element.offset().top;
                this.element.animate({ scrollTop: (treeScrollTop + eleTop) - treeTop });
            }
            catch (e) {

            }

            return true;
        },
        uncheckAll: function (nodes) {
            if (nodes == undefined) {
                nodes = this.dataSource.view();
            }

            for (var i = 0; i < nodes.length; i++) {
                nodes[i].set("checked", false);
                if (nodes[i].hasChildren) {
                    this.uncheckAll(nodes[i].children.view());
                }
            }
        },
        changeLabel: function (id, text) {

            id = id.toString().replace(' ', '');
            let treeitem = this.dataSource.get(id);
            if (treeitem === undefined) {
                return;
            }

            let node = this.findByUid(treeitem.uid);
            this.text(node, text);
        },
        load: function () {

            if (this.options.visible) {

                this.dataSource.read();
                this.options.needLoad = false;

            } else {
                this.options.needLoad = true;
            }
        }
    });
    kendo.ui.plugin(GisTreeView);
})(jQuery);

function AlberoAnagrafica() {
    return $("#gis_treeview").data("kendoGisTreeView");
}

function AggiornaAlberoAnagrafica() {
    let treeView = AlberoAnagrafica();
    if (treeView != undefined) treeView.load();
}

function GISRicercaSempliceAvvia() {
    let filterText = $("#filterText").val();
    RicercaAlberoAnagrafica(filterText, 0);
}

function treeViewEspansioneRicorsiva(filterText, treeView) {
    $("#gis_treeview .k-in:contains(" + filterText + ")").each(function () {
        $(this).parents("ul, li").each(function () {
            treeView.expand($(this).parents("li"));
            $(this).show();
            //var regEx = new RegExp($(filterText).val(), "ig");
            //$(this).html($(this).html().replace(regEx, "<span style='lightgreen'>" + $(filterText).val() + '</span>'));
        });
        $(this).parent().siblings(1).find(".k-in").parents("ul, li").each(function () {
            treeView.expand($(this).parents("li"));
            $(this).show();
        });
    });
    $("#gis_treeview .k-group .k-in:ci_contains(" + filterText + ")").each(function () {
        $(this).parents("ul, li").each(function () {
            treeView.expand($(this).parents("li"));
            $(this).show();
            //var regEx = new RegExp($(filterText).val(), "ig");
            //$(this).html($(this).html().replace(regEx, "<span style='lightgreen'>" + $(filterText).val() + '</span>'));
        });
        $(this).parent().siblings(1).find(".k-in").parents("ul, li").each(function () {
            treeView.expand($(this).parents("li"));
            $(this).show();
        });
    });
}

var filterTimer = 0;

/**
 * 
 * @param {string} filterText
 */
function RicercaAlberoAnagrafica(filterText, tipoRicerca) {

    clearTimeout(filterTimer);

    switch (tipoRicerca) {
        case 0:


            if (filterText !== "") {

                filterTimer = setTimeout(function () {

                    $("#filterContainer .filter-loader").removeClass("filter-loader-hidden");

                    setTimeout(function () {

                        $("#gis_treeview .k-group .k-group .k-in").closest("li").hide();
                        $("#gis_treeview .k-group").closest("li").hide();

                        let treeView = AlberoAnagrafica();

                        treeViewEspansioneRicorsiva(filterText, treeView);

                        $("#filterContainer .filter-loader").addClass("filter-loader-hidden");

                    }, 10);

                }, 1000);

            } else {

                $("#gis_treeview .k-group").find("li").show();
                let nodes = $("#gis_treeview > .k-group > li");

                $.each(nodes, function (i, elem) {
                    if (elem.getAttribute("data-expanded") == null) {
                        $(elem).find("li").hide();
                    }
                });
            }
            break;

        case 1:

            if (filterText === "clear") {
                $("#gis_treeview .k-group").find("li").show();
                let nodes = $("#gis_treeview > .k-group > li");

                $.each(nodes, function (i, elem) {
                    if (elem.getAttribute("data-expanded") == null) {
                        $(elem).find("li").hide();
                    }
                });
            } else {
                filterTimer = setTimeout(function () {

                    $(".search-or-loader").toggleClass("search-or-loader-hidden");

                    setTimeout(function () {

                        $("#gis_treeview .k-group .k-group .k-in").closest("li").hide();
                        $("#gis_treeview .k-group").closest("li").hide();

                        let treeView = AlberoAnagrafica();

                        //let pattern = CatastoGeneraRegExPattern();

                        $("#gis_treeview .k-in").testCatasto().each(function () {
                            $(this).parents("ul, li").each(function () {
                                treeView.expand($(this).parents("li"));
                                $(this).show();
                            });
                        });

                        $(".search-or-loader").toggleClass("search-or-loader-hidden");

                    }, 10);

                }, 1000);
            }


        default:
            break;
    }
}

async function docR() {

    if (modificaMultipla != "" && modificaMultipla != "0") {
        permesso_modificaMultipla = true;
    } else {
        permesso_modificaMultipla = false;
    }

    //nasconde la tabstrip finchè non verrà impostata
    $("#tabstrip").hide();

    $("#" + hidden_objP_agenda_ClientID).change(function () {
        objP_agenda = $(this).html();
    });

    if (!menuBSAnagraficaResx) {
        MenuBSAnagraficaResxLeggi();
    }

    $('#txt_nuovoPiva').val(JSON.parse(objP_agenda).Piva);

    $("#ddl_nuovoCentro").data("kendoDropDownList");
    $("#ddl_nuovoCentro_Campo").data("kendoDropDownList");
    $("#ddl_nuovoCampo").data("kendoDropDownList");
    $("#ddl_nuovoAppezzamento").data("kendoDropDownList");

    // Nascondo il menu con le operazioni principali
    // $('#menu_princ_operazioni').hide();

    $('#btn_nuovo').hide();

    $("body").on("click", "#tabDati_due li a", function () {
        $('#tabDati').find('.active').removeClass('active');
    });
    $("body").on("click", "#tabDati li a", function () {
        $('#tabDati_due').find('.active').removeClass('active');
    });

    tabStrip = $("#tabstrip").kendoTabStrip({
        select: onSelectKendoTab,
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    }).data("kendoTabStrip");

    let hds = new kendo.data.HierarchicalDataSource({
        transport: {
            read: GetNodesAlberoAnagrafe
        },
        schema: {
            model: {
                id: "id",
                //hasChildren: function (e) {
                //    return e.items.length > 0;
                //},
                hasChildren: true,
                children: "items"
            }
        }
    });

    let catastoCheckbox = $.cookie("MenuBS_Anagrafica.filtriCatastoCheckbox");
    if(catastoCheckbox != null)
        $('#chk_showHideCatasto').prop('checked', catastoCheckbox === 'true');

    $("#AlberoCentriDropdown").change(function () {
        alberoFiltriCambiati(false);
    });

    treeView = $("#gis_treeview").kendoGisTreeView({
        autoBind: false,
        loadOnDemand: false,
        animation: false,
        template: '<span class="#: item.style #">#: item.text #</span>',
        /* checkboxes: {
            template: kendo.template(
                "# if (item.id !== '') { #" +
                "<input type='checkbox' class='k-checkbox' #= item.checked ? checked='checked' : '' # />" +
                "<span class='k-checkbox-label k-no-text checkbox-span'></span>" +
                "# } #"
            )
        }, */
        dataSource: hds,
        select: function (e) {
            var item = this.dataItem(e.node);
            let elemId = ImpostaSelezioneAlbero(item.id);
            $("#slide-in-handle").trigger("click");
            $("html,body").scrollTop(0);
            navigateToTabUsingElemId(elemId);



            //let checked = this.dataItem(e.node).checked;
            //if (checked === true) {
            //    checked = false;
            //} else {
            //    checked = true;
            //}
            // this.dataItem(e.node).set("checked", checked);
            // this.trigger("check", { node: e.node });
        }/* ,
        check: function (e) {
            let id = this.dataItem(e.node).id;
            let checked = this.dataItem(e.node).checked;
            AlberoAnagraficaCheck(id, checked);
        } */
    });

    // apriAlberoAnagrafica();

    $("#filterContainer").css("width", "-webkit-fill-available");
    if (config_albero != "2") $("#gisMenuRicercaAvanzata").hide();
    $("#RicercaAvanzataCatasto").hide();

    $("#gisMenuRicercaAvanzata").click(function () {
        if (!$("#RicercaAvanzataCatasto").is(":visible")) {
            $("#gis_treeview").height($("#gis_treeview").height() - 40);
        }
        $("#RicercaAvanzataCatasto").show();
    });

    $("#gisMenuRicercaAvanzataChiudi").click(function () {
        RicercaAlberoAnagrafica("clear", 1);
        if ($("#RicercaAvanzataCatasto").is(":visible")) {
            $("#gis_treeview").height($("#gis_treeview").height() + 40);
        }
        $("#RicercaAvanzataCatasto").hide();

    });

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

    await ImpostaObjP_Agenda(0, dataFiltroValidita, false, function () { });

    $("#filterData").on('keydown', function (e) {
        if (e.keyCode === 13) { AggiornaAlberoAnagrafica(); e.preventDefault(); return false; }
    });
    $("#filterText").on('keydown', function (e) {
        if (e.keyCode === 13) { e.preventDefault(); return false; }
    });

    $("#filterAvanzatoProvText").on('keydown', function (e) {
        if (e.keyCode === 13) { RicercaAlberoAnagrafica("", 1); e.preventDefault(); return false; }
    });
    $("#filterAvanzatoComText").on('keydown', function (e) {
        if (e.keyCode === 13) { RicercaAlberoAnagrafica("", 1); e.preventDefault(); return false; }
    });
    $("#filterAvanzatoFoglioText").on('keydown', function (e) {
        if (e.keyCode === 13) { RicercaAlberoAnagrafica("", 1); e.preventDefault(); return false; }
    });
    $("#filterAvanzatoParticellaText").on('keydown', function (e) {
        if (e.keyCode === 13) { RicercaAlberoAnagrafica("", 1); e.preventDefault(); return false; }
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
    /*
    $("body").on("change", "#ddl_nuovoCentro", function () {
        $('#ddl_nuovoCampo').empty();
        $('#ddl_nuovoAppezzamento').empty();
        if (!$('#ddl_nuovoCentro').val() == "")
            CambioCentro();
    });
    $("body").on("change", "#ddl_nuovoCampo", function () {
        //CaricaCampi();
        $('#ddl_nuovoAppezzamento').empty();
        CaricaAppezzamento($('#ddl_nuovoAppezzamento'), $('#ddl_nuovoCentro').val(), $('#ddl_nuovoCampo').val())
    });
    */
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

    promises.push(LeggiDisciplinarePrivato());

    let resp = await Promise.all(promises);

    permesso_impresa_read = resp[0];
    permesso_impresa_write = resp[1];

    permesso_centro_read = resp[2];
    permesso_centro_write = resp[3];

    permesso_campo_read = resp[4];
    permesso_campo_write = resp[5];

    permesso_appezzamento_read = resp[6];
    permesso_appezzamento_write = resp[7];

    permesso_impianto_read = resp[8];
    permesso_impianto_write = resp[9];

    permesso_fabbricato_read = resp[10];
    permesso_fabbricato_write = resp[11];

    permesso_particelle_read = resp[12];
    permesso_particelle_write = resp[13];

    permesso_contatti_read = resp[14];
    permesso_contatti_write = resp[15];

    permesso_macchine_read = resp[16];
    permesso_macchine_write = resp[17];

    permesso_sblocca = resp[18];

    permesso_modifica_contatti_pubblici_read = resp[19];
    permesso_modifica_contatti_pubblici_write = resp[20];

    permesso_gestione_macchine_pubbliche_read = resp[21];
    permesso_gestione_macchine_pubbliche_write = resp[22];

    permesso_prodotti_read = resp[23];
    permesso_prodotti_write = resp[24];

    permesso_bdn_read = resp[25];
    permesso_bdn_write = resp[26];

    flag_disciplinareprivato = resp[27];

    if (ereditatore != "" && ereditatore != "0") {
        permesso_appezzamento_ereditatore = permesso_appezzamento_write;
        permesso_impianto_ereditatore = permesso_impianto_write;
    } else {
        permesso_appezzamento_ereditatore = false;
        permesso_impianto_ereditatore = false;
    }

    //IMPOSTO LA VISIBILITA PER LA VISUALIZZAZIONE DELLE TAB

    if (!permesso_impresa_read) {
        $(tabStrip.items()[0]).attr("style", "display:none");

        if (!permesso_centro_read) {
            $(tabStrip.items()[1]).attr("style", "display:none");

            if (!permesso_campo_read) {
                $(tabStrip.items()[3]).attr("style", "display:none");

                if (!permesso_appezzamento_read) {
                    $(tabStrip.items()[4]).attr("style", "display:none");
                    if (!permesso_impianto_read) {
                        $(tabStrip.items()[5]).attr("style", "display:none");
                        $(tabStrip.items()[6]).attr("style", "display:none");
                    }
                }

            }

            if (!permesso_fabbricato_read) {
                $(tabStrip.items()[7]).attr("style", "display:none");
            }

            if (!permesso_particelle_read) {
                $(tabStrip.items()[2]).attr("style", "display:none");
            }

        }

    }

    if (!permesso_contatti_read) {
        $(tabStrip.items()[8]).attr("style", "display:none");
    }

    if (!permesso_macchine_read) {
        $(tabStrip.items()[9]).attr("style", "display:none");
    }

    if (!permessi_pagina.PermessiZoo) {
        $(tabStrip.items()[10]).attr("style", "display:none");
        $(tabStrip.items()[11]).attr("style", "display:none");
        $(tabStrip.items()[12]).attr("style", "display:none");
        //$("#btnConfiguraZoo").hide();
    }


    if (!permessi_pagina.PermessiMeteoDSS) {
        $(tabStrip.items()[13]).attr("style", "display:none");
        //$("#btnConfiguraZoo").hide();
    }

    if (!permesso_prodotti_read) {
        $(tabStrip.items()[14]).attr("style", "display:none");
    }

    //finito di impostare la tabstrip la mostra
    $("#tabstrip").show();

    var qsVisibilita = Request_QueryString("visibilita");

    // IMPOSTO LA VISIBILITA' PER LA CREAZIONE DI NUOVI ELEMENTI
    if (permesso_impresa_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="1">' + Traduzione(menuBSAnagraficaResx, 'Azienda', 'AZIENDA') + '</option>');
    }

    if (permesso_centro_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="2">' + Traduzione(menuBSAnagraficaResx, 'CentroAziendale', 'CENTRO AZIENDALE') + '</option>');
    }

    if (permesso_particelle_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="6">' + Traduzione(menuBSAnagraficaResx, 'ParticellaCatastale', 'PARTICELLA CATASTALE') + '</option>');
    }

    if (permesso_campo_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="3">' + Traduzione(menuBSAnagraficaResx, 'Campo', 'CAMPO') + ' / ' + Traduzione(menuBSAnagraficaResx, 'Serra', 'SERRA') + '</option>');
    }

    if (permesso_appezzamento_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="4">' + Traduzione(menuBSAnagraficaResx, 'Appezzamento', 'APPEZZAMENTO') + '</option>');
    }

    if (permesso_impianto_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="5">' + Traduzione(menuBSAnagraficaResx, 'Impianto', 'IMPIANTO') + '</option>');
    }

    if (permesso_fabbricato_write && qsVisibilita !== "3") {
        if (qsVisibilita == "2") {
            $('#tipoNuovo').append('<option value="10">' + Traduzione(menuBSAnagraficaResx, 'Stalla', 'STALLA') + '</option>');
        }
        else {
            $('#tipoNuovo').append('<option value="10">' + Traduzione(menuBSAnagraficaResx, 'Fabbricato', 'FABBRICATO') + '</option>');
        }
    }

    if (permessi_pagina.PermessiZoo && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="7">' + Traduzione(menuBSAnagraficaResx, 'RaggruppamentoStalla', 'RAGGRUPPAMENTO STALLA') + '</option>');
        $('#tipoNuovo').append('<option value="9">' + Traduzione(menuBSAnagraficaResx, 'Animale', 'ANIMALE') + '</option>');
    }

    if (permesso_contatti_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="11">' + Traduzione(menuBSAnagraficaResx, 'Contatto', 'CONTATTO') + '</option>');
    }

    if (permesso_macchine_write && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="12">' + Traduzione(menuBSAnagraficaResx, 'Macchina', 'MACCHINA') + '</option>');
    }


    if (permessi_pagina.PermessiMeteoDSS && qsVisibilita !== "2" && qsVisibilita !== "3") {
        $('#tipoNuovo').append('<option value="13">' + Traduzione(menuBSAnagraficaResx, 'DSSeMeteo', 'DSS E METEO') + '</option>');
    }

    if (permesso_prodotti_write && qsVisibilita !== "2") {
        $('#tipoNuovo').append('<option value="15">' + Traduzione(menuBSAnagraficaResx, 'Prodotto', 'PRODOTTO') + '</option>');
    }

    var qsGestione = Request_QueryString("gestione");

    // anagrafica agricola
    if (qsVisibilita !== "2" && qsVisibilita !== "3") {

        var qsAlbero = Request_QueryString("albero");
        var qsVisibilita = Request_QueryString("visibilita");

        // albero anagrafica
        if ((qsAlbero !== null && qsAlbero == "1") || (config_albero != "" && config_albero != "0")) {
            $("#slide-in-share").show();
            slideVisible = true;
        }

        // x gestione esercizi
        if ((qsGestione !== null && qsGestione == "1") || (gestione_esercizi != "" && gestione_esercizi != "0")) {
            if (permesso_impianto_write && qsVisibilita != "2") {
                // $.removeCookie('MenuBS_Anagrafica.gestioneEserciziWindow');
                var auto = $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow");
                if (auto == undefined) { // && gestione_esercizi == "1"
                    $.cookie("MenuBS_Anagrafica.gestioneEserciziWindow", "1", { path: "/", expires: 10000 });
                    auto = "1";
                }
                if (auto == "1") caricaGestioneEsercizi(false);
            }
        }

    }


    if (qsVisibilita !== null && qsVisibilita !== "") {
        switch (qsVisibilita) {
            case "1": //Anagrafica Agricola
                $(tabStrip.items()[10]).attr("style", "display:none");
                $(tabStrip.items()[11]).attr("style", "display:none");
                $(tabStrip.items()[12]).attr("style", "display:none");
                break;
            case "2": //Anagrafica Zootecnica
                $(tabStrip.items()[3]).attr("style", "display:none");
                $(tabStrip.items()[4]).attr("style", "display:none");
                $(tabStrip.items()[5]).attr("style", "display:none");
                $(tabStrip.items()[6]).attr("style", "display:none");
                $(tabStrip.items()[7]).attr("style", "display:none");
                $(tabStrip.items()[2]).attr("style", "display:none");
                $(tabStrip.items()[8]).attr("style", "display:none");
                $(tabStrip.items()[9]).attr("style", "display:none");
                $(tabStrip.items()[13]).attr("style", "display:none");
                $(tabStrip.items()[14]).attr("style", "display:none");

                $("#btnStampa").attr("style", "display:none");

                break;
            case "3": //Prodotti
                $(tabStrip.items()[0]).attr("style", "display:none");
                $(tabStrip.items()[1]).attr("style", "display:none");
                $(tabStrip.items()[2]).attr("style", "display:none");
                $(tabStrip.items()[3]).attr("style", "display:none");
                $(tabStrip.items()[4]).attr("style", "display:none");
                $(tabStrip.items()[5]).attr("style", "display:none");
                $(tabStrip.items()[6]).attr("style", "display:none");
                $(tabStrip.items()[7]).attr("style", "display:none");
                $(tabStrip.items()[8]).attr("style", "display:none");
                $(tabStrip.items()[9]).attr("style", "display:none");
                $(tabStrip.items()[10]).attr("style", "display:none");
                $(tabStrip.items()[11]).attr("style", "display:none");
                $(tabStrip.items()[12]).attr("style", "display:none");
                $(tabStrip.items()[13]).attr("style", "display:none");
                //$(tabStrip.items()[14]).attr("style", "display:none");
                break;
        }
    }

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

        //ImpostaObjP_Agenda(0, dataFiltroValidita, false, function () {
        if (qsVisibilita !== null && qsVisibilita !== "" && qsVisibilita == "3") {
            navigateToTabUsingElemId("15");
        } else {
            navigateToTabUsingElemId(tab);
        }   
    }

    function navigateToTabUsingElemId(elemId) {
        elemId = elemId + "";
        if (elemId === 'undefined')
            elemId = undefined;
        if (elemId != undefined) {
            switch (elemId) {
                case "1":
                    tabStrip.select(0);
                    break;
                case "2":
                    tabStrip.select(1);
                    break;
                case "6":
                    tabStrip.select(2);
                    break;
                case "5":
                    tabStrip.select(5);
                    break;
                case "3":
                    tabStrip.select(3);
                    break;
                case "4":
                    tabStrip.select(4);
                    break;
                case "10":
                    tabStrip.select(7);
                    break;
                case "8":
                    tabStrip.select(10);
                    break;
                case "7":
                    tabStrip.select(11);
                    break;
                case "9":
                    tabStrip.select(12);
                    break;
                case "11":
                    tabStrip.select(8);
                    break;
                case "12":
                    tabStrip.select(9);
                    break;
                case "13":
                    tabStrip.select(13);
                    break;
                case "14":
                    tabStrip.select(6);
                    break;
                case "15":
                    tabStrip.select(14);
                    break;
            }
        } else {
            tabStrip.select(0);
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

        tabStrip.select(tab);

    });

    function ImpostaSelezioneAlbero(v) {

        var chiave = "";

        switch (parseInt(v.split("§")[0])) {
            case 2:

                //impresa
                $('#tabDati a[href="#divKendoAzienda"]').tab('show');

                if (v.split("§")[1] != 0) {
                    ImpostaObjP_Agenda(1, v.split("§")[1], false, function () {
                        Dati_Relativi_Percorso_Selezione2(1);
                    });
                }

                ImpostaSelezione(1, 1);

                return 1;
            case 3:
                //centro
                $('#tabDati a[href="#divKendoCentro"]').tab('show');

                // Gestione selezione singolo record
                if (v.split("§")[2] != 0) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2];
                    ImpostaObjP_Agenda(2, chiave, false, function () {
                        Dati_Relativi_Percorso_Selezione2(2);
                    });
                }

                ImpostaSelezione(2, 1);

                return 2;
            case 4:
                //campo
                $('#tabDati a[href="#divKendoCampo"]').tab('show');

                // Gestione selezione singolo record
                if (v.split("§")[3] != 0) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[3];
                    ImpostaObjP_Agenda(3, chiave, false, function () {
                        Dati_Relativi_Percorso_Selezione2(3);
                    });
                }

                ImpostaSelezione(3, 1);

                return 3;
            case 5:
                //appezzamento
                $('#tabDati a[href="#divKendoAppezzamento"]').tab('show');

                // Gestione selezione singolo record
                if (v.split("§")[4] != 0) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[4];
                    ImpostaObjP_Agenda(4, chiave + "_" + v.split("§")[3], false, function () {
                        Dati_Relativi_Percorso_Selezione2(4);
                    });
                }

                ImpostaSelezione(4, 1);

                return 4;
            case 6:
            case 7:
            case 8:
            case 9:
                //impianti
                $('#tabDati a[href="#divKendoImpianto"]').tab('show');

                // Gestione selezione singolo record
                if ((v.split("§")[4] != 0) && (v.split("§")[5] != 0)) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[4] + "_" + v.split("§")[5];
                    ImpostaObjP_Agenda(5, chiave, false, function () {
                        Dati_Relativi_Percorso_Selezione2(5);
                    });
                }

                ImpostaSelezione(5, 1);

                return 5;
            case 47:
                //esercizi
                $('#tabDati a[href="#divKendoEsercizio"]').tab('show');

                // Gestione selezione singolo record
                if ((v.split("§")[4] != 0) && (v.split("§")[5] != 0)) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[4] + "_" + v.split("§")[5];
                    ImpostaObjP_Agenda(5, chiave, false, function () {
                        Dati_Relativi_Percorso_Selezione2(5);
                    });
                }

                ImpostaSelezione(14, 1);

                return 14;
            case 18:
            case 10:
                //catasto
                $('#tabDati a[href="#divKendoCatasto"]').tab('show');

                // Gestione selezione singolo record
                if ((v.split("§")[6] != 0) && (v.split("§")[7] != 0) && (v.split("§")[8] != 0) && (v.split("§")[10] != 0) && (v.split("§")[11] != 0)) {
                    chiave = v.split("§")[1] + "_" + v.split("§")[2] + "_" + v.split("§")[7] + "_" + v.split("§")[8] + "_" + v.split("§")[9] + "_" + v.split("§")[10] + "_" + v.split("§")[11] + "_" + v.split("§")[12] + "_" + v.split("§")[6];
                    setIdRigaSelezionataDellAlbero(chiave);
                    ImpostaObjP_Agenda(6, chiave, false, function () {
                        Dati_Relativi_Percorso_Selezione2(6);
                    });
                }
                else {
                    setIdRigaSelezionataDellAlbero("");
                }

                ImpostaSelezione(6, 1);

                return 6;

            case 35:
                // Macchine
                $('#tabDati a[href="#divKendoMacchina"]').tab('show');
                ImpostaSelezione(12, 1);
                return 12;
            case 36:
                // Contatti
                $('#tabDati a[href="#divKendoContatto"]').tab('show');
                ImpostaSelezione(11, 1);
                return 11;
            case 37:
                // Fabbricati
                $('#tabDati a[href="#divKendoFabbricato"]').tab('show');
                ImpostaSelezione(10, 1);
                return 10;
        }

    }

    // @Paolo
    // Gestione click dei pulsanti del menu principale (info, modifica, cancella)
    // INFO
    $('#btn_info_s').click(function () {
        var chiave = $(this).attr('chiave');
        var tipo = $(this).attr('tipo');

        switch (tipo) {

            case "3":
                //centro
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoCentro',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Centro_Edit.aspx";
                    }
                });
                break;

            case "10":
                //catasto
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoParticelle',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Particella_Edit.aspx";
                    }
                });

                break;

            case "4":
                //campo/serra
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoCampo',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Campo_Edit.aspx";
                    }
                });

                break;

            case "5":
                // appezzamento
                WaitFrame.show();

                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoAppezzamento',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Appezzamento_Edit.aspx";
                    }
                });

                break;

            case "6":
            case "7":
            case "8":
                //impianto
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoImpianto',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Impianto_Edit.aspx";
                    }
                });

                break;

            case "22":
                //fabbricato
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/InfoFabbricato',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Fabbricato_Edit.aspx";
                    }
                });

                break;
        }

    });

    // MODIFICA
    $('#btn_modifica_s').click(function () {
        var chiave = $(this).attr('chiave');
        var tipo = $(this).attr('tipo');

        switch (tipo) {
            case "2":
                //impresa
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditImpresa',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Impresa_edit.aspx";
                    }
                });
                break;

            case "3":
                //centro
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditCentro',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Centro_Edit.aspx";
                    }
                });
                break;

            case "10":
                //catasto
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditParticelle',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Particella_Edit.aspx";
                    }
                });

                break;

            case "4":
                //campo/serra
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditCampo',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Campo_Edit.aspx";
                    }
                });

                break;

            case "5":
                // appezzamento
                WaitFrame.show();

                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditAppezzamento',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Appezzamento_Edit.aspx";
                    }
                });

                break;

            case "6":
            case "7":
            case "8":
                //impianto
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditImpianto',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Impianto_Edit.aspx";
                    }
                });

                break;

            case "22":
                //fabbricato
                WaitFrame.show();
                $.ajax({
                    type: 'POST',
                    url: './MenuBs_Anagrafica.aspx/EditFabbricato',
                    data: "{chiave: '" + chiave + "' }",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        window.location = "../Anagrafica/Fabbricato_Edit.aspx";
                    }
                });

                break;
        }

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

    kendoDialogInfoCatasto = $("#dialogInfoCatasto").kendoWindow({
        //width: "670px",
        //heigth: "467px",
        modal: true,
        resizable: true,
        title: Traduzione(menuBSAnagraficaResx, "InfoCatasto", "Info Catasto"),
        closable: true,
        visible: false,
        actions: ["Maximize", "Close"]
    }).data("kendoWindow");

}

function ProcediNuovo() {
    var v = parseInt($('#tipoNuovo').val());
    var flag = false;

    // Prodotto
    if (v !== 15)
        flag = ValidaxNuovo();
    else
        flag = true;

    if (flag) {
        var chiave;

        switch (v) {
            case 1:
                //impresa
                gotoNewElement(v, '');
                break;
            case 2:
                //centro
                gotoNewElement(v, '');
                break;
            case 3:
                //campo
                var centro = $('#ddl_nuovoCentro_Campo').val();
                var serra = 0;

                if ($('#chk_serra').is(':checked'))
                    serra = 1;

                chiave = centro + "-" + serra;
                gotoNewElement(v, chiave);
                break;
            case 4:
                //Appezza
                var azienda = $('#txt_nuovoPiva').val();
                var centro = $('#ddl_nuovoCentro').val();
                var campo = $('#ddl_nuovoCampo').val();
                chiave = azienda + "-" + centro + "-" + campo;
                gotoNewElement(v, chiave);
                break;
            case 5:
                //impianto
                var azienda = $('#txt_nuovoPiva').val();
                var centro = $('#ddl_nuovoCentro').val();
                var campo = $('#ddl_nuovoCampo').val();
                var appezza = $('#ddl_nuovoAppezzamento').val();
                chiave = azienda + "-" + centro + "-" + campo + "-" + appezza;
                gotoNewElement(v, chiave);
                break;
            //case 7:
            //    var azienda = $('#txt_nuovoPiva').val();
            //    var centro = $('#ddl_nuovoCentro').val();
            //    var campo = $('#ddl_nuovoCampo').val();
            //    var appezza = $('#ddl_nuovoAppezzamento').val();
            //    chiave = azienda + "-" + centro + "-";
            //    gotoNewElement(v, chiave);
            //    break;
            case 6:
                //catasto
                chiave = $('#ddl_nuovoCentro').val();
                gotoNewElement(v, chiave);
                break;
            case 7:
                var azienda = $('#txt_nuovoPiva').val();
                var centro = $('#ddl_nuovoCentro').val();
                var fabbr = $('#ddl_nuovoFabbricatoStalla').val();
                chiave = azienda + "-" + centro + "-" + fabbr;
                gotoNewElement(v, chiave).val();
                break;
            case 9:
                var azienda = $('#txt_nuovoPiva').val();
                var centro = $('#ddl_nuovoCentro').val();
                var fabbr = $('#ddl_nuovoFabbricatoStalla').val();
                chiave = azienda + "-" + centro + "-" + fabbr;
                gotoNewElement(v, chiave).val();
                break;
            case 10:
                //fabbricato
                chiave = $('#ddl_nuovoCentro').val();
                gotoNewElement(v, chiave);
                break;
            case 11:
                //contatto
                gotoNewElement(v, '');
                break;
            case 12:
                //macchina
                gotoNewElement(v, chiave);
                break;
            case 13:
                //Dss e Meteo
                gestioneNewElementDssMeteo();
                break;
            case 15:
                // Prodotto
                chiave = Get_KendoDDLValue("ddl_categorie_prodotti");
                gotoNewElement(v, chiave);
                break;
        }

    }

}

function Permessi_Pagina() {
    return new Promise((resolve, reject) => {
        let storage_key = "Permessi_Pagina";

        if (!storageExistItem(storage_key)) {
            var parametri = kendo.stringify({});

            ajaxAgronica("MenuBS_Anagrafica.aspx/Permessi_Pagina",
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


//function LeggiPermessoUtente(username, id_attivita, id_operazione) {
//    return new Promise((resolve, reject) => {
//        let storage_key = "LeggiPermessoUtente_" + username + "_" + id_attivita + "_" + id_operazione;

//        if (!storageExistItem(storage_key)) {
//            var ActualDate = new Date()
//            var stringData = ActualDate.toLocaleDateString();
//            var param = {
//                UserName: username,
//                Id_Servizio: 5,
//                Id_Attivita: id_attivita,
//                Id_Operazione: id_operazione,
//                DataOraControllo: ActualDate,
//                xFiltroAggiuntivo: '',
//                objP_utenti: objP_utenti
//            }
//            ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Controlla_Permessi_UtenteR", JSON.stringify(param),
//                function (risposta) {
//                    if (risposta.RispostaStringa.toLowerCase() == "true") {
//                        //window.sessionStorage.setItem(storage_key, JSON.stringify(true));
//                        storageSetItem(storage_key, true);
//                        resolve(true);
//                    } else {
//                        //window.sessionStorage.setItem(storage_key, JSON.stringify(false));
//                        storageSetItem(storage_key, false);
//                        resolve(false);
//                    }
//                }, null, null, false);
//        } else {
//            //resolve(JSON.parse(window.sessionStorage.getItem(storage_key)));
//            resolve(storageGetItem(storage_key));
//        }

//    });
//}

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

        ajaxAgronica("MenuBS_Anagrafica.aspx/dateValiditaAgenda",
            parametri,
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    });
}