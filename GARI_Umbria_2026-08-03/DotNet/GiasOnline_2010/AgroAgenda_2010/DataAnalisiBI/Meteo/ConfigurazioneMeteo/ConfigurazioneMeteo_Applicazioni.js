

(function ($) {
    $.menuBar = function (elem, opts) {

        var plugin = this;

        let css = ".hidden-page { display: none !important; } ";
        css += ".bar-menu { font-size: 16px; font-weight: bold; margin-bottom: 8px; border-bottom: 2px solid #428bca; display: flex; align-items: center; user-select: none; } ";
        css += ".bar-menu-icon { color: #428bca; font-size: 21px; } ";
        css += ".bar-menu-item { color: #428bca; font-size:1.5em; padding:4px 8px; box-shadow: none!important; } ";
        css += ".bar-menu-title { color: #428bca; padding-left: 10px; } ";
        css += ".bar-menu-pages-container { overflow: hidden; } ";
        css += ".bar-menu-page { height: 100%; overflow-y: auto; } ";

        let style = document.createElement('style');
        style.type = "text/css";
        style.innerHTML = css;
        elem.appendChild(style);

        var _bar = document.createElement("div");
        _bar.className = "bar-menu";
        elem.appendChild(_bar);
        var _barH = 0;

        var _menu = document.createElement("div");
        _menu.innerHTML = "<span class='k-icon k-i-menu bar-menu-icon'></span>";

        _bar.appendChild(_menu);

        var _titleElem = document.createElement("div");
        _titleElem.className = "bar-menu-title";
        _titleElem.innerHTML = "<span class='page-title'></span>";
        _bar.appendChild(_titleElem);

        var _pages = document.createElement("div");
        _pages.className = "bar-menu-pages-container";
        _pages.id = elem.id + "-pages";
        elem.appendChild(_pages);

        let items = [];

        $.each(opts.pages, function (i, p) {
            let elem = document.getElementById(p.id);
            $(elem).data("page-title", p.title);
            $(elem).addClass("bar-menu-page").addClass("hidden-page");
            $(elem).detach().appendTo("#" + _pages.id);

            items.push({ id: p.id, text: p.title });
        });

        $(_menu).kendoDropDownButton({
            fillMode: "flat",
            items: items,
            itemTemplate: "<span class=\"k-link k-menu-link bar-menu-item\">#:text#</span>",
            click: function (e) {
                _showPage($(e.target).index());
            },
            open: function (e) {
                $(e.target).children().toggleClass("k-i-menu");
                $(e.target).children().toggleClass("k-i-x");
            },
            close: function (e) {
                $(e.target).children().toggleClass("k-i-x");
                $(e.target).children().toggleClass("k-i-menu");
            }
        });

        var _showPage = function (idx) {
            let pages = $(_pages).children();
            if (idx < 0 || idx >= pages.length) {
                return;
            }
            if (!$(pages[idx]).hasClass("hidden-page")) {
                return;
            }

            let pageIn = $(pages[idx]);
            let pageOut = $(_pages).find(".bar-menu-page:not(.hidden-page)");

            let setTitle = function () {

                $(_titleElem).find(".page-title").each(function (i, e) {
                    e.innerHTML = pageIn.data("page-title");
                });
            };

            if (pageOut.length === 0) {

                pageIn.removeClass("hidden-page");
                setTitle();

            } else {

                pageOut = $(pageOut[0]);

                kendo.fx(pageIn.removeClass("hidden-page")).tile("left", pageOut.addClass("hidden-page")).duration(200).play();

                kendo.fx(_titleElem).fadeOut().duration(200).play().then(function () {
                    setTitle();
                    kendo.fx(_titleElem).fadeIn().duration(200).play();
                });
            }

            let ddb = $(_menu).getKendoDropDownButton();

            ddb.items().each(function (i, m) {
                ddb.enable(i != idx, "#" + m.id);
            });
        };

        _showPage(0);

        plugin.fitHeight = function (h) {
            _barH = Math.max(_barH, $(_bar).outerHeight(true));
            $(_pages).height(h - _barH);
        };
    };

    //Add the plugin to the jQuery.fn object
    $.fn.menuBar = function (opts) {
        return this.each(function () {

            if (undefined == $(this).data('menuBar')) {

                var plugin = new $.menuBar(this, opts);

                $(this).data('menuBar', plugin);
            }
        });
    };
})(jQuery);

function _ridimensiona_ApplicazioniTabstrip(h) {

    $("#applicArea").height(h);

    $("#applicArea > div").each(function (i, e) {
        $(e).height(h);
    });

    $("#applicMenu").data("menuBar").fitHeight(h);
}

function _applicazioni_stazioni_reload(sorgenti) {

    $("#applicDSSDifesa").getKendoListView().dataSource.read();
    $("#applicDSSIrriga").getKendoListView().dataSource.read();
    $("#applicMonitorHP").getKendoListView().dataSource.read();
    $("#applicMonitorSuolo").getKendoListView().dataSource.read();

    let ddl = $("#ddlTipoSorgente").getKendoDropDownList();
    let tipoSorgente = parseInt(ddl.value());

    if (sorgenti.indexOf(tipoSorgente) >= 0) {

        ddl.trigger("change");
    }
}

function _template_item_stazione(hdr_right, block_list) {

    let trascinaStazione = TraduzioneMultiResx(datiMeteoResx, "TrascinaQuiUnaStazione", "Trascina qui una stazione");

    let t = "";

    t += "<div class='list-view-item'>";
    t += "#if (data.key_stazione) {#";
    t += "  <div class='k-block item-block'>";
    t += "      <div class='applic-stazione-item-hdr'>";
    t += "          <div class='stazione-container'>";
    t += "              <div class='stazione-nome'><span>#:stazione_name#</span></div>";
    t += "              <div class='stazione-fornitore'><span>#:rif_fornitore#</span></div>";
    t += "              <div class='staz-feat-container'>";
    t += "  #if (!flag_reale) {#";
    t += "                  <div class='staz-feat'><span class='fa fa-magic fa-fw'></span></div>";
    t += "  #}#";
    t += "  #if (sensori_des.length > 0) {#";
    t += "                  <div class='staz-feat info-sensors show-tooltip' data-tooltip='#:sensori_des#'><span class='fa fa-sitemap fa-fw'></span></div>";
    t += "   #}#"
    t += "              </div>";
    t += "          </div>";
    t += hdr_right;
    t += "      </div>";
    t += block_list;
    t += "  </div>";
    t += "#} else {#";
    t += "  <div class='stazione-container applic-stazione-drop-target'><span class='fa fa-plus fa-lg'></span><span>" + trascinaStazione + "</span></div>";
    t += "#}#";
    t += "</div>";

    return t;
}

function _applicazioni_jQueryDocReady() {

    $("#applicMenu").menuBar({
        pages: [
            {
                id: "applicDSSDifesa",
                title: TraduzioneMultiResx(datiMeteoResx, "TitoloAppDSSDifesa", "Indicatori emergenze DSS difesa")
            },
            {
                id: "applicDSSIrriga",
                title: TraduzioneMultiResx(datiMeteoResx, "TitoloAppDSSIrriga", "DSS irrigazione")
            },
            {
                id: "applicMonitorHP",
                title: TraduzioneMultiResx(datiMeteoResx, "TitoloAppMonitorHP", "Riepilogo dati meteo")
            },
            {
                id: "applicMonitorSuolo",
                title: TraduzioneMultiResx(datiMeteoResx, "TitoloAppMonitorSuolo", "Monitoraggio suolo")
            }
        ]
    });

    //let tabTitle = [
    //    TraduzioneMultiResx(datiMeteoResx, "TitoloAppDSSDifesa", "Indicatori emergenze DSS difesa"),
    //    TraduzioneMultiResx(datiMeteoResx, "TitoloAppDSSIrriga", "DSS irrigazione"),
    //    TraduzioneMultiResx(datiMeteoResx, "TitoloAppMonitorHP", "Riepilogo dati meteo"),
    //    TraduzioneMultiResx(datiMeteoResx, "TitoloAppMonitorSuolo", "Monitoraggio suolo")
    //];

    //$("#applicTabstrip").find("ul").find("li").each(function (i, li) {
    //    let title = "...";
    //    if (i < tabTitle.length) {
    //        title = tabTitle[i];
    //    }
    //    li.innerText = title;
    //});

    $("#applicStazioniFilter").filtroStazioni({
        placeholder: "Filtra stazioni...",
        listView: "elencoStaz",
        filter: {
            logic: "or",
            filters: [
                { field: "Staz_Nome", operator: "contains" },
                { field: "RifFornitore", operator: "contains" }
            ]
        }
    });

    let btnLocator = document.createElement("div");
    btnLocator.className = "fa-btn";
    btnLocator.style.width = "100%";
    btnLocator.style.fontSize = "20px";
    btnLocator.style.color = "#D61355";
    let textLocator = document.createElement("span");
    textLocator.textContent = "Seleziona una posizione";
    textLocator.style.cssText = "font-size: 14px; padding-top: 4px; padding-bottom: 4px;";
    btnLocator.appendChild(textLocator);
    $("#applicStazioniLocator").append(btnLocator);
    $(btnLocator).kendoButton({
        iconClass: "fa fa-map-marker",
        click: function () {
            _locate();
        }
    });
    
    $("#ddlTipoSorgente").kendoDropDownList({
        autoBind: true,
        autoWidth: true,
        dataValueField: "sorgente_cod",
        dataTextField: "sorgente_des",
        dataSource: {
            transport: {
                read: function (options) {

                    let sorgenti = [];

                    ajaxAgronicaSync(url_meteows + "/SorgentiMeteo",
                        JSON.stringify({}),
                        false,
                        function (risposta) {
                            sorgenti = JSON.parse(risposta.RispostaStringa);
                        },
                        function (risposta) {
                        }
                    );

                    options.success(sorgenti);
                }
            }
        },
        change: function (e) {

            let val = parseInt(this.value());

            if (val !== 4) {

                $("#applicStazioniLocator").addClass("__hidden__");
                $("#applicStazioniFilter").removeClass("__hidden__");

            } else {

                //Stazioni pubbliche
                $("#applicStazioniLocator").removeClass("__hidden__");
                $("#applicStazioniFilter").addClass("__hidden__");

            }

            $("#applicStazioniFilter").data("filtroStazioni").clearFilter();
            $("#elencoStaz").getKendoListView().dataSource.read();
        }
    });

    let stazTemplate = "<div class='list-view-item stazione stoppable-pointer-events'>";
    stazTemplate += "   <div class='k-block item-block draggable-container'>";
    stazTemplate += "       <div class='drag-handle'>";
    stazTemplate += "           <div class='handler'>";
    //stazTemplate += "               <span class='fa fa-ellipsis-v'></span>";
    //stazTemplate += "               <span class='fa fa-ellipsis-v'></span>";
    //stazTemplate += "               <span class='dragging-off k-icon k-i-handler-drag'></span>";
    stazTemplate += "               <span class='k-icon k-i-handler-drag'></span>";
    stazTemplate += "           </div>";
    stazTemplate += "       </div>";
    stazTemplate += "       <div class='draggable-full-width stazione-container'>";
    stazTemplate += "           <div class='stazione-nome'><span>#:Staz_Nome#</span></div>";
    stazTemplate += "           <div class='stazione-fornitore'><span>#:RifFornitore#</span></div>";
    stazTemplate += "           <div class='staz-feat-container'>";
    stazTemplate += "#if (!FlagReale) {#";
    stazTemplate += "               <div class='staz-feat'><span class='fa fa-magic fa-fw'></span></div>";
    stazTemplate += "#}#";
    stazTemplate += "           </div>";
    stazTemplate += "       </div>";
    stazTemplate += "       <div class='#= data.GeoLoc ? 'k-button fa-btn btn-globe' : 'k-button fa-btn " + GIAS_K_STATE_DISABLED + "' #'><span class='fa fa-globe'></span></div>";
    stazTemplate += "   </div>";
    stazTemplate += "</div>";

    $("#elencoStaz").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioniElenco
            }
        },
        autoBind: false,
        selectable: false,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazTemplate),
        dataBound: function (e) {

            $.each(e.sender.items(), function (i, e) {
                $("#applicAreaTarget").data("stazioniDragDrop").addDraggable(e);
            });

            e.sender.content.find(".btn-globe").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item.stazione");
                    _stazioniApriGIS(staz);
                }
            });
        }
    });

    $("#btnStazioniGIS").on("click", function (e) {
        _stazioniApriGIS();
    });



    let modelli_tmplt = "#if (modelli.length > 0) {#";
    modelli_tmplt += "  #let veg_des = '';#"
    modelli_tmplt += "  <div class='stazione-modelli-grid'>";
    modelli_tmplt += "  #for (let m = 0; m < modelli.length; m++) {#";
    modelli_tmplt += "      #if (veg_des !== modelli[m].veg_des) {#";
    modelli_tmplt += "          #veg_des = modelli[m].veg_des;#";
    modelli_tmplt += "          <div class='specie-modello'><span>#:veg_des#</span></div>";
    modelli_tmplt += "      #}#";
    modelli_tmplt += "      <div style='font-weight: bold; padding-left: 10px;'><span>#:modelli[m].mod_name#</span></div>";
    modelli_tmplt += "      <div style='color: rgb(211, 211, 211);'><span>#:modelli[m].avv_name#</span></div>";
    modelli_tmplt += "      <div class='info-modello show-tooltip' data-tooltip='#:modelli[m].periodo_des#'><span class='fa fa-calendar-o'></span></div>";
    modelli_tmplt += "      <div class='info-modello show-tooltip' data-tooltip='#:modelli[m].validita_des#'><span class='fa fa-clock-o'></span></div>";
    modelli_tmplt += "      <div title='#:modelli[m].param_des#'><span>#:modelli[m].param_des#</span></div>";
    modelli_tmplt += "      <div class='k-button fa-btn btn-trash-mod gias-btn-delete-white' mod-idx='#:m#'><span class='fa fa-trash-o'></span></div>";
    modelli_tmplt += "  #}#";
    modelli_tmplt += "  </div>";
    modelli_tmplt += "#}#";

    let stazDifesaTemplate = _template_item_stazione("<div class='k-button fa-btn btn-check-mod'><span class='fa fa-star-o'></span></div>", modelli_tmplt);

    $("#applicDSSDifesa").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioniDSSDifesa
            }//,
            //sort: { field: "sa_nome", dir: "asc" }
        },
        autoBind: false,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazDifesaTemplate),
        dataBound: function (e) {

            e.sender.content.find(".applic-stazione-drop-target").each(function (i, e) {

                $("#applicAreaTarget").data("stazioniDragDrop").addDropTarget(e);
            });

            e.sender.content.find(".btn-check-mod").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniDSSDifesaAggiungiModello(staz);
                }
            });

            e.sender.content.find(".btn-trash-mod").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    let idx_modello = parseInt($(e).attr("mod-idx"));
                    _stazioniDSSDifesaEliminaModello(staz, idx_modello);
                }
            });
        }
    });



    let centri_tmplt = "#if (centri.length > 0) {#";
    centri_tmplt += "   <div class='elenco-centri'>";
    centri_tmplt += "   #for (let c = 0; c < centri.length; c++) {#";
    centri_tmplt += "       <div><span>#:centri[c].sa_nome#</span></div>";
    centri_tmplt += "   #}#";
    centri_tmplt += "   </div>";
    centri_tmplt += "#}#";

    stazIrrigaTemplate = _template_item_stazione("<div class='k-button fa-btn btn-check-centro'><span class='fa fa-star-o'></span></div>", centri_tmplt);

    $("#applicDSSIrriga").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioniDSSIrriga
            }//,
            //sort: { field: "sa_nome", dir: "asc" }
        },
        autoBind: false,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazIrrigaTemplate),
        dataBound: function (e) {

            e.sender.content.find(".applic-stazione-drop-target").each(function (i, e) {

                $("#applicAreaTarget").data("stazioniDragDrop").addDropTarget(e);
            });

            e.sender.content.find(".btn-check-centro").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniDSSIrrigaAggiungiCentro(staz);
                }
            });
        }
    });



    let stazMonitorTemplate = _template_item_stazione("<div class='k-button fa-btn btn-clock-staz'><span class='fa fa-clock-o'></span></div><div class='k-button fa-btn btn-trash-staz'><span class='fa fa-trash-o'></span></div>", "");

    $("#applicMonitorHP").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioniMonitorHP
            }//,
            //sort: { field: "Staz_Nome", dir: "asc" }
        },
        autoBind: false,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazMonitorTemplate),
        dataBound: function (e) {

            e.sender.content.find(".applic-stazione-drop-target").each(function (i, e) {

                $("#applicAreaTarget").data("stazioniDragDrop").addDropTarget(e);
            });

            e.sender.content.find(".btn-clock-staz").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniMonitorHPEdit(staz);
                }
            });

            e.sender.content.find(".btn-trash-staz").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniMonitorHPElimina(staz);
                }
            });
        }
    });


    let param_tmplt = "#if (data.parametri) {#";
    param_tmplt += "<div class='monitor-suolo-param-grid'>";
    param_tmplt += "<div class='monitor-suolo-param'><span>Soglia inferiore</span><span class='param-value'>#= kendo.toString(parametri.soglia_inf, '0.0')#</span><span>%</span></div>";
    param_tmplt += "<div class='monitor-suolo-param'><span>Soglia superiore</span><span class='param-value'>#= kendo.toString(parametri.soglia_sup, '0.0')#</span><span>%</span></div>";
    param_tmplt += "<div class='monitor-suolo-param'><span>Durata monitoraggio</span><span class='param-value'>#= kendo.toString(parametri.periodo_gg, '0')#</span><span>gg</span></div>";
    param_tmplt += "</div>";
    param_tmplt += "#}#";

    let stazTerrenoTemplate = _template_item_stazione("<div class='k-button fa-btn btn-param-staz'><span class='fa fa-cogs'></span></div><div class='k-button fa-btn btn-trash-staz'><span class='fa fa-trash-o'></span></div>", param_tmplt);

    $("#applicMonitorSuolo").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioniMonitorSuolo
            }//,
            //sort: { field: "Staz_Nome", dir: "asc" }
        },
        autoBind: false,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazTerrenoTemplate),
        dataBound: function (e) {

            e.sender.content.find(".applic-stazione-drop-target").each(function (i, e) {

                $("#applicAreaTarget").data("stazioniDragDrop").addDropTarget(e);
            });

            e.sender.content.find(".btn-param-staz").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniMonitorSuoloEdit(staz);
                }
            });

            e.sender.content.find(".btn-trash-staz").each(function (i, e) {
                e.onclick = function () {
                    let staz = e.closest(".list-view-item");
                    _stazioniMonitorSuoloElimina(staz);
                }
            });
        }
    });


    $("#applicAreaTarget").stazioniDragDrop({
        sourceListView: $("#elencoStaz").getKendoListView(),
        stazioneDropCallback: _stazioniDropCallback
    });

    $("#applicAreaTarget").kendoPopover({ 
        filter: ".show-tooltip",
        position: "left",
        showOn: "mouseenter",
        //showOn: "click",
        //toggleOnClick: true,
        body: function (e) {

            let content = "<div class='applic-popup'>";
            let data = e.target.attr("data-tooltip"); // the element for which the tooltip is shown
            if (data.indexOf("§") > -1) {
                let arr = data.split("§");
                $.each(arr, function (i, s) {
                    content += "<div><span>&#8718</span><span>" + s + "</span></div>";
                });
            } else {
                content += "<div><span>" + data + "</span></div>";
            }
            content += "</div>";

            return content;
        }
    });

    $("#applicDSSDifesa").getKendoListView().dataSource.read();
    $("#applicDSSIrriga").getKendoListView().dataSource.read();
    $("#applicMonitorHP").getKendoListView().dataSource.read();
    $("#applicMonitorSuolo").getKendoListView().dataSource.read();

    let ddl = $("#ddlTipoSorgente").getKendoDropDownList();
    let id_sorgente = ddl.dataItems().at(0).sorgente_cod;
    if (ddl.dataItems().some((el) => el.sorgente_cod === 1)) {
        id_sorgente = 1;
    }
    ddl.select(function (dataItem) {
        return dataItem.sorgente_cod === id_sorgente;
    });

    ddl.trigger("change");
}

function _stazioniApriGIS(staz) {

    let arrGeo = [];

    let staz_ds = $("#elencoStaz").getKendoListView().dataSource;

    if (staz !== undefined && staz !== null) {

        let uid = $(staz).attr("data-uid");
        let stazItem = staz_ds.getByUid(uid);
        if (stazItem !== undefined) {
            if (stazItem.GeoLoc !== undefined) {

                arrGeo.push({
                    label: stazItem.Staz_Nome,
                    geo: stazItem.GeoLoc
                })
            }
        }
    } else {

        $.each(staz_ds.view(), function (i, s) {

            if (s.GeoLoc !== undefined) {

                arrGeo.push({
                    label: s.Staz_Nome,
                    geo: s.GeoLoc
                })
            }
        });
    }

    if (arrGeo.length === 0) {
        info_dlg(
            TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgGISStazioniVuoto", "Visualizzazione GIS stazioni"),
            "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgGISStazioniVuoto", "Non ci sono stazioni con attributi geografici da visualizzare.") + "</div>"
        );

        return;
    }

    map_wnd(arrGeo);
}

function _stazioniDropCallback(dropTarget, stazDataItem) {

    let listview = dropTarget.closest(".k-listview").getKendoListView();
    if (listview === undefined) {
        return false;
    }

    //Verifico che non sia già presente...
    let data = listview.dataSource.data();
    let pos = data.length - 1;

    let i = 0;
    let found = false;
    while (!found && i < pos) {
        found = (data[i].key_stazione.tipo_sorgente === stazDataItem.TipoSorgente
            && data[i].key_stazione.stazione_cod === stazDataItem.Staz_Id);
        i++;
    }

    if (found) {
        return false;
    }

    let id = listview.element[0].id;

    if (id === "applicMonitorHP") {

        let result = false;

        ajaxAgronicaSync(url_meteows + "/RegistraStazioneXMonitor",
            JSON.stringify({ tipo_sorgente: stazDataItem.TipoSorgente, stazione_cod: stazDataItem.Staz_Id, monitor_ore: 48, is_new: true }),
            false,
            function (risposta) {

                listview.dataSource.read();

                result = true;
            },
            function (risposta) {
            },
            null,
            false
        );

        return result;
    }

    if (id === "applicMonitorSuolo") {

        let result = false;

        let params = {
            tipo_sorgente: stazDataItem.TipoSorgente,
            stazione_cod: stazDataItem.Staz_Id,
            parametri: JSON.stringify({
                soglia_inf: 0,
                soglia_sup: 0,
                periodo_gg: 7
            }),
            is_new: true
        };

        ajaxAgronicaSync(url_meteows + "/RegistraStazioneXMonitoraggioSuolo",
            JSON.stringify(params),
            false,
            function (risposta) {

                listview.dataSource.read();

                result = true;
            },
            function (risposta) {
            },
            null,
            false
        );

        return result;
    }

    let stazione = {
        key_stazione: {
            tipo_sorgente: stazDataItem.TipoSorgente,
            stazione_cod: stazDataItem.Staz_Id
        },
        stazione_name: stazDataItem.Staz_Nome,
        rif_fornitore: stazDataItem.RifFornitore,
        flag_reale: stazDataItem.FlagReale,
        sensori_des: ""
    };

    ajaxAgronicaSync(url_meteows + "/LeggiStazione",
        JSON.stringify({ tipo_sorgente: stazDataItem.TipoSorgente, stazione_cod: stazDataItem.Staz_Id }),
        false,
        function (risposta) {

            stazione = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        },
        null,
        false
    );


    if (id === "applicDSSDifesa") {
        stazione.modelli = [];
    }

    if (id === "applicDSSIrriga") {
        stazione.centri = [];
    }

    data.splice(pos, 0, stazione);

    return true;
}

function LeggiStazioniDSSDifesa(options) {

    let stazioni = _stazioniXModelli();

    stazioni.push({});

    options.success(stazioni);
}

function _stazioniXModelli(tipo_sorgente, stazione_cod) {

    let stazioni = [];

    if (tipo_sorgente === undefined) {
        tipo_sorgente = -1;
    }
    if (stazione_cod === undefined) {
        stazione_cod = -1;
    }

    let param = {
        tipo_sorgente: tipo_sorgente,
        stazione_cod: stazione_cod
    };

    ajaxAgronicaSync(url_meteows + "/LeggiStazioniXModelli",
        JSON.stringify(param),
        false,
        function (risposta) {

            stazioni = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    return stazioni;
}

function _stazioniDSSDifesaAggiungiModello(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicDSSDifesa").getKendoListView().dataSource.getByUid(uid)

    if (stazDataItem === undefined) {
        return;
    }

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let content = "<div>";
    //content += "<div id='__modelli-wrapper__' style='width: 500px;'></div>";
    content += "<div id='__modelli-wrapper__'></div>";
    content += "<div id='__container-notifica__' class='notifica-no-icon' style='padding-top: 10px;'></div>";
    content += "<div id='__notifica__'></div>";
    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "titoloFinestraAssociaModelloDSS", "Associa indicatore per modello DSS difesa"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        open: function () {

            $("#__modelli-wrapper__").pluginModelli({
                url_read: url_meteows + "/LeggiModelliAutorizzati",
                onError: function (err_str) {

                    $("#__notifica__").getKendoNotification().hide();
                    $("#__notifica__").getKendoNotification().error(err_str);
                }
            });

            $("#__notifica__").kendoNotification({
                appendTo: "#__container-notifica__",
                animation: {
                    open: {
                        effects: "zoom:in" //"expand:vertical"
                    },
                    close: false
                },
                autoHideAfter: 0
            });
        },
        actions: [
            {
                text: 'Aggiungi',
                cssClass: "btn-aggiungi",
                action: function (e) {

                    $("#__notifica__").getKendoNotification().hide();

                    let thisBtn = e.sender.element.closest(".k-dialog").find(".btn-aggiungi");
                    let savehtml = $(thisBtn).html();
                    $(thisBtn).html("<i class='fa fa-refresh fa-spin fa-fw fa-lg'></i><span class='sr-only'></span>");

                    let modello = $("#__modelli-wrapper__").data("pluginModelli").getModello();

                    setTimeout(function () {

                        if (modello !== null) {

                            let params = {
                                tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                                stazione_cod: stazDataItem.key_stazione.stazione_cod,
                                modello: modello
                            };

                            ajaxAgronicaSync(url_meteows + "/RegistraModelloXStazione",
                                JSON.stringify(params),
                                false,
                                function (risposta) {

                                    $("#__notifica__").getKendoNotification().success("Elemento aggiunto correttamente");

                                    stazDataItem.modelli = JSON.parse(risposta.RispostaStringa);
                                    $("#applicDSSDifesa").getKendoListView().refresh();
                                },
                                function (risposta) {

                                    $("#__notifica__").getKendoNotification().error("Elemento non aggiunto");
                                },
                                null,
                                false
                            );
                        }

                        $(thisBtn).html(savehtml);

                    }, 100);

                    return false;
                }
            },
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Chiudi", "Chiudi")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

function _stazioniDSSDifesaEliminaModello(staz, idx_modello) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicDSSDifesa").getKendoListView().dataSource.getByUid(uid);

    if (stazDataItem === undefined) {
        return;
    }

    if (idx_modello >= stazDataItem.modelli.length) {
        return;
    }

    yesno_dlg(
        TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEliminaModelloDSSDifesa", "Rimuovi modello"),
        "<div style='padding-bottom: 15px; font-weight: bold;'>" + stazDataItem.modelli[idx_modello].mod_name + "</div>" +
        "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraEliminaModelloDSSDifesa", "Rimuovere il modello dall'elenco?") + "</div>",
        function () {

            let params = {
                tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                stazione_cod: stazDataItem.key_stazione.stazione_cod,
                modello: stazDataItem.modelli[idx_modello].key_modello
            };

            ajaxAgronicaSync(url_meteows + "/EliminaModelloXStazione",
                JSON.stringify(params),
                false,
                function (risposta) {

                    stazDataItem.modelli = JSON.parse(risposta.RispostaStringa);
                    $("#applicDSSDifesa").getKendoListView().refresh();
                },
                function (risposta) {
                }
            );
        }
    )
}



function LeggiStazioniDSSIrriga(options) {

    let stazioni = _stazioniXIrriga();

    stazioni.push({});

    options.success(stazioni);
}

function _stazioniXIrriga(tipo_sorgente, stazione_cod) {

    let stazioni = [];

    if (tipo_sorgente === undefined) {
        tipo_sorgente = -1;
    }
    if (stazione_cod === undefined) {
        stazione_cod = -1;
    }

    let param = {
        tipo_sorgente: tipo_sorgente,
        stazione_cod: stazione_cod
    };

    ajaxAgronicaSync(url_meteows + "/LeggiStazioniXIrriga",
        JSON.stringify(param),
        false,
        function (risposta) {

            stazioni = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    return stazioni;
}

function _stazioniDSSIrrigaAggiungiCentro(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicDSSIrriga").getKendoListView().dataSource.getByUid(uid)

    if (stazDataItem === undefined) {
        return;
    }

    let win_el = document.createElement("div");
    win_el.id = "add-centri";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    let content = "<div style='width: 300px;'>";
    content += "<div id='dlg-centri' style='height: 300px; overflow-y: auto;'></div>";
    content += "<div id='__container-notifica__' class='notifica-no-icon' style='padding-top: 10px;'></div>";
    content += "<div id='__notifica__'></div>";
    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "titoloFinestraAssociazioneCentri", "Associazione centri aziendali"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        open: function () {

            $("#__notifica__").kendoNotification({
                appendTo: "#__container-notifica__",
                animation: {
                    open: {
                        effects: "zoom:in" //"expand:vertical"
                    },
                    close: false
                },
                autoHideAfter: 0
            });

            let centri = [];

            ajaxAgronicaSync(url_page + "/LeggiCentri",
                JSON.stringify({}),
                false,
                function (risposta) {

                    centri = JSON.parse(risposta.RispostaStringa);
                },
                function (risposta) {
                },
                null,
                false
            );

            $.each(centri, function (i, c) {

                let checked = false;
                let j = 0;
                while (!checked && j < stazDataItem.centri.length) {
                    checked = (c.sa_cod === stazDataItem.centri[j].sa_cod);
                    j++;
                }

                c.checked = checked;
            });

            let tmplt = "<div class='dlg-centro-item'>";
            tmplt += "  <input type='checkbox' id='id_#:sa_cod#' class='k-checkbox' style='margin: 0px;'>";
            tmplt += "  <label class='k-checkbox-label' for='id_#:sa_cod#'>#:sa_nome#</label>";
            tmplt += "</div>";
            $("#dlg-centri").kendoListView({
                dataSource: {
                    data: centri,
                    sort: { field: "sa_nome", dir: "asc" }
                },
                template: kendo.template(tmplt),
                dataBound: function (e) {

                    $.each(e.items, function (idx, item) {
                        if (item.checked) {
                            e.sender.content.find("#id_" + item.sa_cod).each(function (i, e) {
                                $(e).attr("checked", "checked");
                            });
                        }
                    });

                    e.sender.content.find(".k-checkbox").each(function (i, e) {
                        e.onfocus = function (ev) {
                            $(ev.currentTarget).blur();
                        };
                        e.onclick = function (ev) {
                            let checked = $(ev.currentTarget).is(":checked");
                            let lv_item = e.closest(".dlg-centro-item");
                            if (lv_item !== null) {
                                let uid = $(lv_item).attr("data-uid");
                                let data_item = $("#dlg-centri").getKendoListView().dataSource.getByUid(uid)
                                if (data_item !== undefined) {
                                    data_item.checked = checked;
                                }
                            }
                        };
                    });
                }
            });
        },
        actions: [
            {
                text: TraduzioneMultiResx(datiMeteoResx, "OK", "OK"),
                action: function (e) {

                    $("#__notifica__").getKendoNotification().hide();

                    let result = false;

                    let checked = [];
                    let listview = $("#dlg-centri").getKendoListView();

                    $.each(listview.dataSource.data(), function (i, n) {
                        if (n.checked) {
                            checked.push(n.sa_cod);
                        }
                    });

                    let params = {
                        tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                        stazione_cod: stazDataItem.key_stazione.stazione_cod,
                        centri: checked
                    };

                    ajaxAgronicaSync(url_meteows + "/RegistraCentriXStazione",
                        JSON.stringify(params),
                        false,
                        function (risposta) {

                            if (risposta.RispostaStringa.length > 0) {

                                let stazItem = JSON.parse(risposta.RispostaStringa);
                                stazDataItem.centri = stazItem.centri;

                                $("#applicDSSIrriga").getKendoListView().refresh();
                            }

                            result = true;
                        },
                        function (risposta) {

                            $("#__notifica__").getKendoNotification().error(
                                TraduzioneMultiResx(datiMeteoResx,
                                    "notificaErroreAssociazioneCentro",
                                    "<div style='text-align: center;'>" +
                                    "<div style='margin-bottom:10px;'>Un centro selezionato potrebbe essere già associato ad una stazione...</div>" +
                                    "<div style='font-weight: bold;'>Associazione non effettuata!</div>" +
                                    "</div>"
                                )
                            );
                        },
                        null,
                        false
                    );

                    return result;
                }
            },
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Annulla", "Annulla")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}



function LeggiStazioniMonitorHP(options) {

    let stazioni = [];

    let param = {
    };

    ajaxAgronicaSync(url_meteows + "/LeggiStazioniXMonitor",
        JSON.stringify(param),
        false,
        function (risposta) {

            stazioni = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    stazioni.push({});

    options.success(stazioni);
}

function _stazioniMonitorHPEdit(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicMonitorHP").getKendoListView().dataSource.getByUid(uid);

    if (stazDataItem === undefined) {
        return;
    }

    let win_el = document.createElement("div");
    win_el.id = "edit-monitor";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    let content = "<div style='display: flex; align-items: center;'>";
    content += "<div style='margin-right: .8em;'><span>";
    content += TraduzioneMultiResx(datiMeteoResx, "labelDurataFinestraEditMonitor", "Durata minima riepiologo dati meteo");
    content += "</span></div>";
    content += "<input id='__ore__' type='number' />";
    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEditMonitor", "Impostazioni riepilogo stazione"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        open: function () {

            $("#__ore__").kendoNumericTextBox({
                value: stazDataItem.monitor_ore,
                min: 24,
                max: 7 * 24,
                format: "0 ore"
            });
        },
        actions: [
            {
                text: TraduzioneMultiResx(datiMeteoResx, "OK", "OK"),
                action: function (e) {

                    let result = false;

                    let monitor_ore = $("#__ore__").getKendoNumericTextBox().value();

                    if (monitor_ore === stazDataItem.monitor_ore) {

                        result = true;

                    } else {

                        let params = {
                            tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                            stazione_cod: stazDataItem.key_stazione.stazione_cod,
                            monitor_ore: monitor_ore,
                            is_new: false
                        };

                        ajaxAgronicaSync(url_meteows + "/RegistraStazioneXMonitor",
                            JSON.stringify(params),
                            false,
                            function (risposta) {

                                stazDataItem.monitor_ore = monitor_ore;
                                result = true;
                            },
                            function (risposta) {
                            },
                            null,
                            false
                        );
                    }

                    return result;
                }
            },
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Annulla", "Annulla")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

function _stazioniMonitorHPElimina(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicMonitorHP").getKendoListView().dataSource.getByUid(uid);

    if (stazDataItem === undefined) {
        return;
    }

    yesno_dlg(
        TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEliminaStazioneMonitorHP", "Rimuovi stazione"),
        "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraEliminaStazioneMonitorHP", "Rimuovere la stazione dal monitoraggio in home page?") + "</div>",
        function () {

            let params = {
                tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                stazione_cod: stazDataItem.key_stazione.stazione_cod
            };

            ajaxAgronicaSync(url_meteows + "/EliminaStazioneXMonitor",
                JSON.stringify(params),
                false,
                function (risposta) {

                    $("#applicMonitorHP").getKendoListView().dataSource.read();
                },
                function (risposta) {
                }
            );
        }
    );
}



function LeggiStazioniMonitorSuolo(options) {

    let stazioni = [];

    let param = {
    };

    ajaxAgronicaSync(url_meteows + "/LeggiStazioniXMonitoraggioSuolo",
        JSON.stringify(param),
        false,
        function (risposta) {

            stazioni = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    stazioni.push({});

    options.success(stazioni);
}

function _stazioniMonitorSuoloEdit(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicMonitorSuolo").getKendoListView().dataSource.getByUid(uid);

    if (stazDataItem === undefined) {
        return;
    }

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let content = "<div style='font-size: larger; font-weight: bold; text-align: center; padding-bottom: 15px;'>" + stazDataItem.stazione_name + "</div>";
    content += "<div style='display: flex; justify-content: center;'>";
    content += "<div style='display: grid; grid-template-columns: 1fr auto; grid-gap: 10px; align-items: center; justify-items: end;'>";
    content += "<div><span>" + TraduzioneMultiResx(datiMeteoResx, "labelSogliaInfFinestraEditMonitorSuolo", "Soglia inferiore") + "</span></div>";
    content += "<div><input id='__s_inf__' type='number' /></div>";
    content += "<div><span>" + TraduzioneMultiResx(datiMeteoResx, "labelSogliaSupFinestraEditMonitorSuolo", "Soglia superiore") + "</span></div>";
    content += "<div><input id='__s_sup__' type='number' /></div>";
    content += "<div><span>" + TraduzioneMultiResx(datiMeteoResx, "labelDurataFinestraEditMonitorSuolo", "Durata monitoraggio") + "</span></div>";
    content += "<div><input id='__gg__' type='number' /></div>";
    content += "</div>";
    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEditMonitorSuolo", "Impostazioni monitoraggio suolo"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        open: function () {

            $("#__s_inf__").kendoNumericTextBox({
                value: stazDataItem.parametri.soglia_inf,
                min: 0,
                max: 100,
                format: "0.0 \\%"
            });

            $("#__s_sup__").kendoNumericTextBox({
                value: stazDataItem.parametri.soglia_sup,
                min: 0,
                max: 100,
                format: "0.0 \\%"
            });

            $("#__gg__").kendoNumericTextBox({
                value: stazDataItem.parametri.periodo_gg,
                min: 1,
                max: 180,
                format: "0 giorni"
            });
        },
        actions: [
            {
                text: TraduzioneMultiResx(datiMeteoResx, "OK", "OK"),
                action: function (e) {

                    let result = false;

                    let soglia_inf = $("#__s_inf__").getKendoNumericTextBox().value();
                    let soglia_sup = $("#__s_sup__").getKendoNumericTextBox().value();
                    let periodo_gg = $("#__gg__").getKendoNumericTextBox().value();

                    if (Math.abs(soglia_inf - stazDataItem.parametri.soglia_inf) < 0.01 &&
                        Math.abs(soglia_sup - stazDataItem.parametri.soglia_sup) < 0.01 &&
                        periodo_gg === stazDataItem.parametri.periodo_gg) {

                        result = true;

                    } else {

                        if (soglia_sup > soglia_inf) {

                            let params = {
                                tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                                stazione_cod: stazDataItem.key_stazione.stazione_cod,
                                parametri: JSON.stringify({
                                    soglia_inf: soglia_inf,
                                    soglia_sup: soglia_sup,
                                    periodo_gg: periodo_gg
                                }),
                                is_new: false
                            };

                            ajaxAgronicaSync(url_meteows + "/RegistraStazioneXMonitoraggioSuolo",
                                JSON.stringify(params),
                                false,
                                function (risposta) {

                                    stazDataItem.parametri.soglia_inf = soglia_inf;
                                    stazDataItem.parametri.soglia_sup = soglia_sup;
                                    stazDataItem.parametri.periodo_gg = periodo_gg;

                                    result = true;
                                },
                                function (risposta) {
                                },
                                null,
                                false
                            );

                            if (result) {

                                $("#applicMonitorSuolo").getKendoListView().refresh();
                            }

                        } else {
                            //Errore
                        }
                    }

                    return result;
                }
            },
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Annulla", "Annulla")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

function _stazioniMonitorSuoloElimina(staz) {

    let uid = $(staz).attr("data-uid");
    let stazDataItem = $("#applicMonitorSuolo").getKendoListView().dataSource.getByUid(uid);

    if (stazDataItem === undefined) {
        return;
    }

    yesno_dlg(
        TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEliminaStazioneTerreno", "Rimuovi stazione"),
        "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraEliminaStazioneTerreno", "Rimuovere la stazione dal monitoraggio suolo?") + "</div>",
        function () {

            let params = {
                tipo_sorgente: stazDataItem.key_stazione.tipo_sorgente,
                stazione_cod: stazDataItem.key_stazione.stazione_cod
            };

            ajaxAgronicaSync(url_meteows + "/EliminaStazioneXControllo",
                JSON.stringify(params),
                false,
                function (risposta) {

                    $("#applicMonitorSuolo").getKendoListView().dataSource.read();
                },
                function (risposta) {
                }
            );
        }
    )
}



function LeggiStazioniElenco(options) {

    let stazioni = [];

    let tipoSorgente = parseInt($("#ddlTipoSorgente").getKendoDropDownList().value());

    if (!isNaN(tipoSorgente) && tipoSorgente >= 0) {

        let lat = 0;
        let lng = 0;

        if (Object.keys(options.data).length !== 0) {

            if (options.data.hasOwnProperty("lat") && options.data.hasOwnProperty("lng")) {

                lat = options.data.lat;
                lng = options.data.lng;
            }
        }

        let param = {
            TipoSorgente: tipoSorgente,
            Lat: lat,
            Lng: lng
        };

        ajaxAgronicaSync(url_meteows + "/LeggiStazioniXSorgente",
            JSON.stringify(param),
            false,
            function (risposta) {

                let staz = JSON.parse(risposta.RispostaStringa);

                $.each(staz, function (i, s) {

                    let rif_fornitore = s.fornitore;
                    if (rif_fornitore !== "") {
                        if (s.rif_fornitore !== "") {
                            rif_fornitore += " [" + s.rif_fornitore + "]";
                        }
                    }

                    let ostaz = {
                        TipoSorgente: tipoSorgente,
                        Staz_Id: parseInt(s.id_stazione),
                        Staz_Nome: s.nome_stazione,
                        RifFornitore: rif_fornitore,
                        FlagReale: s.flag_reale
                    };
                    if (s.geo !== undefined) {
                        ostaz.GeoLoc = s.geo;
                    }
                    stazioni.push(ostaz);
                });
            },
            function (risposta) {
            }
        );
    }

    options.success(stazioni);
}

var _saveLocation = null;

function _locate() {

    let smap = $("#g-map-wrapper").data("simpleMap");

    var _decode = function (addr) {

        if (addr == "") {
            return;
        }

        let geocoder = new google.maps.Geocoder;

        geocoder.geocode(
            {
                address: addr
            },
            function (results, status) {

                if (status === google.maps.GeocoderStatus.OK) {

                    let l = results[0].geometry.location;

                    if (l.lat() !== "" && l.lng() !== "") {

                        smap.moveDraggableMarker(l.lat(), l.lng(), _setMapCoord);

                        $("#addrInput").getKendoTextBox().value(results[0].formatted_address);
                    }
                } else {
                    console.log("Geocode was not successful for the following reason: " + status);
                }
            }
        );
    };

    var _mapControl = {
        coord: null,
        latElem: null,
        lngElem: null
    };

    var _coordElem = function (parent, lbl) {

        let div = document.createElement("div");
        div.style.display = "flex";
        div.style.alignItems = "center";
        parent.appendChild(div);

        let outElem = document.createElement("span");
        outElem.style.flexGrow = "1";
        div.appendChild(outElem);

        let lbl_span = document.createElement("span");
        lbl_span.style.fontSize = "10px";
        lbl_span.style.marginLeft = "3px";
        lbl_span.style.color = "#aeaeae";
        lbl_span.style.alignSelf = "flex-end";
        lbl_span.innerText = lbl;
        div.appendChild(lbl_span);

        return outElem;
    };

    const controlDiv = document.createElement("div");
    controlDiv.style.padding = "0px 5px 5px";
    controlDiv.style.borderBottomLeftRadius = "4px";
    controlDiv.style.borderBottomRightRadius = "4px";
    controlDiv.style.backgroundColor = "#fff";
    controlDiv.style.display = "flex";
    controlDiv.style.columnGap = "5px";
    controlDiv.style.userSelect = "none";
    controlDiv.style.width = "50%";

    let style = document.createElement("style");
    style.type = "text/css";
    style.innerHTML = ".k-input.k-textbox { box-shadow: none !important; } .k-button-solid-base { box-shadow: none !important; }";
    controlDiv.appendChild(style);

    var addrDiv = document.createElement("div");
    addrDiv.style.width = "100%"
    addrDiv.style.fontSize = "larger";
    controlDiv.appendChild(addrDiv);

    var addrInput = document.createElement("input");
    addrInput.id = "addrInput";
    addrDiv.appendChild(addrInput);

    $(addrInput).kendoTextBox({
        placeholder: "Indirizzo..."
    });

    $(addrInput).on("keydown", function (e) {
        if (e.keyCode === 13) {
            _decode(this.value);
        }
    });

    let findButton = document.createElement("span");
    findButton.className = "k-clear-value";
    findButton.style.boxShadow = "none !important";
    findButton.setAttribute("unselectable", "on");
    findButton.setAttribute("role", "button");
    findButton.setAttribute("tabindex", "-1");

    $(addrInput).getKendoTextBox().wrapper.append(findButton);

    let findIcon = document.createElement("span");
    findIcon.className = "k-icon k-i-search";

    findButton.appendChild(findIcon);

    findButton.onclick = function () {
        _decode($(addrInput).getKendoTextBox().value());
    };

    const divCoord = document.createElement("div");
    divCoord.className = "k-block";
    divCoord.style.textAlign = "right";
    divCoord.style.display = "grid";
    divCoord.style.gridTemplateColumns = "8em 8em";
    divCoord.style.gridGap = "10px";
    divCoord.style.padding = "3px 6px";
    divCoord.style.fontSize = "16px";
    controlDiv.appendChild(divCoord);

    const acceptBtn = document.createElement("div");
    acceptBtn.style.fontSize = "20px";
    acceptBtn.style.color = "#008000";
    acceptBtn.style.padding = "0px";
    acceptBtn.style.height = "auto";
    acceptBtn.style.width = "70px";
    controlDiv.appendChild(acceptBtn);

    $(acceptBtn).kendoButton({
        iconClass: "fa fa-check fa-fw'",
        click: function () {
            if (_mapControl.coord === null) {
                return;
            }

            this.enable(false);

            this.element.find(".k-loader").toggleClass("__hidden__");
            this.element.find(".k-button-icon").toggleClass("__hidden__");

            let that = this;
            setTimeout(function () {

                smap.close();

                _saveLocation = _mapControl.coord;

                $("#elencoStaz").getKendoListView().dataSource.read(_mapControl.coord);

                that.element.find(".k-loader").toggleClass("__hidden__");
                that.element.find(".k-button-icon").toggleClass("__hidden__");

                that.enable(true);

            }, 100);
        }
    });

    const divLoader = document.createElement("div");
    divLoader.className = "__hidden__";
    $(acceptBtn).getKendoButton().element[0].appendChild(divLoader);
    $(divLoader).kendoLoader({ size: "small" });

    _mapControl.coord = _saveLocation;
    _mapControl.latElem = _coordElem(divCoord, "Lat");
    _mapControl.lngElem = _coordElem(divCoord, "Lng");


    let arrGeo = [];
    let staz_ds = $("#elencoStaz").getKendoListView().dataSource;

    $.each(staz_ds.view(), function (i, s) {

        if (s.GeoLoc !== undefined) {

            arrGeo.push({
                label: s.Staz_Nome,
                geo: s.GeoLoc
            })
        }
    });

    let showLabel = (arrGeo.length <= 20);

    $.each(arrGeo, function (i, m) {

        smap.addMarker(m.geo.lat, m.geo.lng, m.label, showLabel);
    });

    smap.open([
        {
            position: google.maps.ControlPosition.TOP_CENTER,
            control: controlDiv
        }
    ]);

    var _setMapCoord = function (latlng) {

        _mapControl.coord = { lat: latlng.lat(), lng: latlng.lng() };

        _mapControl.latElem.innerText = kendo.toString(_mapControl.coord.lat, "0.0000000°");
        _mapControl.lngElem.innerText = kendo.toString(_mapControl.coord.lng, "0.0000000°");
    };

    if (_mapControl.coord === null) {

        smap.listenForMarker(_setMapCoord);

    } else {

        smap.addDraggableMarker(_mapControl.coord.lat, _mapControl.coord.lng, _setMapCoord);
    }

    smap.showMarkers(true);

    smap.fitBounds();
}



(function ($) {

    $.pluginModelli = function (elem, opts) {

        var plugin = this;

        //plugin.settings = {};
        plugin.element = elem;

        var _errorfun = function () { };
        if (typeof opts.onError === "function") {
            _errorfun = opts.onError;
        }

        var _data = [];

        ajaxAgronicaSync(opts.url_read,
            JSON.stringify({ Veg_Cod: 0 }),
            false,
            function (risposta) {

                _data = JSON.parse(risposta.RispostaStringa);
            },
            function (risposta) {
            },
            null,
            false
        );



        let style = document.createElement("style");
        style.type = "text/css";
        style.innerHTML = ".__hidden__ { display: none; }";
        plugin.element.appendChild(style);

        let grid = document.createElement("div");
        grid.style.cssText = "display:grid; grid-gap: 5px; grid-template-columns: auto 1fr; align-items: center;";

        plugin.element.appendChild(grid);

        let lblSpecie = document.createElement("div");
        lblSpecie.style.cssText = "justify-self: end;";
        lblSpecie.innerHTML = "<span>Specie vegetale</span>";
        let divSpecie = document.createElement("div");
        let inputSpecie = document.createElement("input");
        inputSpecie.id = "__veg__";
        inputSpecie.style.cssText = "width: -webkit-fill-available;";
        divSpecie.appendChild(inputSpecie);

        let lblModello = document.createElement("div");
        lblModello.style.cssText = "justify-self: end;";
        lblModello.innerHTML = "<span>Modello</span>";
        let divModello = document.createElement("div");
        let inputModello = document.createElement("input");
        inputModello.id = "__mod__";
        //inputModello.style.cssText = "width: -webkit-fill-available;";
        inputModello.style.cssText = "width: 50em;";
        divModello.appendChild(inputModello);

        let lblPeriodo = document.createElement("div");
        lblPeriodo.style.cssText = "justify-self: end;";
        lblPeriodo.innerHTML = "<span>Periodo calcolo</span>";
        let divPeriodo = document.createElement("div");
        divPeriodo.style.cssText = "display: flex;";
        let divPeriodo0 = document.createElement("div");
        divPeriodo0.style.cssText = "flex-grow: 1; margin-right: 5px;";
        let divPeriodo1 = document.createElement("div");
        divPeriodo1.style.cssText = "flex-grow: 1; margin-left: 5px;";
        divPeriodo.appendChild(divPeriodo0);
        divPeriodo.appendChild(divPeriodo1);

        let inputIniPeriodo = document.createElement("input");
        inputIniPeriodo.id = "__ini_period__";
        //inputIniPeriodo.style.cssText = "width: -webkit-fill-available;";
        let inputFinPeriodo = document.createElement("input");
        inputFinPeriodo.id = "__fin_period__";
        //inputFinPeriodo.style.cssText = "width: -webkit-fill-available;";
        divPeriodo0.appendChild(inputIniPeriodo);
        divPeriodo1.appendChild(inputFinPeriodo);

        let lblValidita = document.createElement("div");
        lblValidita.style.cssText = "justify-self: end;";
        lblValidita.innerHTML = "<span>Validità (hh:mm)</span>";
        let divValidita = document.createElement("div");
        let inputValidita = document.createElement("input");
        inputValidita.id = "__validita__";
        inputValidita.style.cssText = "width: -webkit-fill-available;";
        divValidita.appendChild(inputValidita);

        grid.appendChild(lblSpecie);
        grid.appendChild(divSpecie);
        grid.appendChild(lblModello);
        grid.appendChild(divModello);
        grid.appendChild(lblPeriodo);
        grid.appendChild(divPeriodo);
        grid.appendChild(lblValidita);
        grid.appendChild(divValidita);

        //*****************************************************************************************
        //Controlli per parametri modello
        //*****************************************************************************************

        //Ticchiolatura_AScab: { value: 14, name: "Ticchiolatura_AScab", code: 14 },
        //RitardoVariabile: { value: 10, name: "RitardoVariabile", code: 10 },
        //Agronomica_30__Peronospora_della_Vite: { value: 16, name: "Agronomica_30__Peronospora_della_Vite", code: 16 },
        //Agronomica_30__BatteriosiKiwi_PSA: { value: 17, name: "Agronomica_30__BatteriosiKiwi_PSA", code: 17 },
        //Racca__PeroPom: { value: 18, name: "Racca__PeroPom", code: 18 },
        //Agronomica_30__Oidio_della_Vite: { value: 19, name: "Agronomica_30__Oidio_della_Vite", code: 19 },
        //Agronomica_30__Botrite_della_Vite: { value: 20, name: "Agronomica_30__Botrite_della_Vite", code: 20 },
        //Agronomica_30__Ticchiolatura_del_Melo: { value: 21, name: "Agronomica_30__Ticchiolatura_del_Melo", code: 21 },
        //Racca__AlterPom: { value: 22, name: "Racca__AlterPom", code: 22 },
        //Racca__OidioPom: { value: 23, name: "Racca__OidioPom", code: 23 },
        //Racca__BotriPom: { value: 24, name: "Racca__BotriPom", code: 24 },
        //Racca__PeroBiet: { value: 25, name: "Racca__PeroBiet", code: 25 },
        //Racca__OidioBiet: { value: 26, name: "Racca__OidioBiet", code: 26 },
        //Racca__CercoBiet: { value: 27, name: "Racca__CercoBiet", code: 27 },
        //Racca__PeroPat: { value: 28, name: "Racca__PeroPat", code: 28 },
        //Racca__AlterPat: { value: 29, name: "Racca__AlterPat", code: 29 },
        //Racca__ScleroSoia: { value: 30, name: "Racca__ScleroSoia", code: 30 },
        //Agronomica_30__MRV_Eulia: { value: 32, name: "Agronomica_30__MRV_Eulia", code: 32 },
        //Agronomica_30__MRV_CydiaMolesta: { value: 33, name: "Agronomica_30__MRV_CydiaMolesta", code: 33 },
        //Agronomica_30__MRV_Carpocapsa: { value: 34, name: "Agronomica_30__MRV_Carpocapsa", code: 34 },
        //Agronomica_30__MRV_Helicoverpa: { value: 35, name: "Agronomica_30__MRV_Helicoverpa", code: 35 },            
        //Agronomica_30__MRV_Tignoletta: { value: 36, name: "Agronomica_30__MRV_Tignoletta", code: 36 },
        //Agronomica_30__Maculatura_del_Pero: { value: 37, name: "Agronomica_30__Maculatura_del_Pero", code: 37 },
        //Agronomica_30__MRV_MoscaOlivo: { value: 38, name: "Agronomica_30__MRV_MoscaOlivo", code: 38 },
        //Racca__ElmintosporiosiMais: { value: 39, name: "Racca__ElmintosporiosiMais", code: 39 },
        //Racca__BipolarisMaidis: { value: 40, name: "Racca__BipolarisMaidis", code: 40 },
        //Racca__AntracnosiOlivo: { value: 41, name: "Racca__AntracnosiOlivo", code: 41 },
        //Racca__MicotoxMais: { value: 43, name: "Racca__MicotoxMais", code: 43 },

        //*****************************************************************************************
        // Modello Racca Brusone Riso (31)
        //*****************************************************************************************
        let lblDataSemina31 = document.createElement("div");
        lblDataSemina31.style.cssText = "justify-self: end;";
        lblDataSemina31.innerHTML = "<span>Data semina</span>";
        let divDataSemina31 = document.createElement("div");
        let inputDataSemina31 = document.createElement("input");
        inputDataSemina31.id = "__datasemina31__";
        inputDataSemina31.style.cssText = "width: -webkit-fill-available;";
        divDataSemina31.appendChild(inputDataSemina31);

        lblDataSemina31.className = "param-modello mod-31";
        divDataSemina31.className = "param-modello mod-31";

        grid.appendChild(lblDataSemina31);
        grid.appendChild(divDataSemina31);

        $("#__datasemina31__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(),
            footer: false
        });
        $("#__datasemina31__").attr("readonly", true);

        let lblVarieta31 = document.createElement("div")
        lblVarieta31.style.cssText = "justify-self: end;";
        lblVarieta31.innerHTML = "<span>Varietà</span>";
        let divVarieta31 = document.createElement("div");
        let inputVarieta31 = document.createElement("select");
        inputVarieta31.id = "__varieta31__";
        inputVarieta31.style.cssText = "width: -webkit-fill-available;";
        //inputVarieta31.innerHTML = "<option>Precoce</option><option>Media</option><option>Tardiva</option>";
        divVarieta31.appendChild(inputVarieta31);

        lblVarieta31.className = "param-modello mod-31";
        divVarieta31.className = "param-modello mod-31";

        grid.appendChild(lblVarieta31);
        grid.appendChild(divVarieta31);

        $("#__varieta31__").kendoDropDownList({
            dataTextField: "text",
            dataSource: [
                { text: "Precoce" },
                { text: "Media" },
                { text: "Tardiva" }
            ],
            index: 1
        });
        //*****************************************************************************************


        //*****************************************************************************************
        // Modello Agronomica30 MISP IPI Pomodoro (42)
        //*****************************************************************************************
        let lblDataTrapianto = document.createElement("div");
        lblDataTrapianto.style.cssText = "justify-self: end;";
        lblDataTrapianto.innerHTML = "<span>Data trapianto</span>";
        let divDataTrapianto = document.createElement("div");
        let inputDataTrapianto = document.createElement("input");
        inputDataTrapianto.id = "__datatrapianto42__";
        inputDataTrapianto.style.cssText = "width: -webkit-fill-available;";
        divDataTrapianto.appendChild(inputDataTrapianto);

        lblDataTrapianto.className = "param-modello mod-42";
        divDataTrapianto.className = "param-modello mod-42";

        grid.appendChild(lblDataTrapianto);
        grid.appendChild(divDataTrapianto);

        $("#__datatrapianto42__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(),
            footer: false
        });
        $("#__datatrapianto42__").attr("readonly", true);
        //*****************************************************************************************


        //*****************************************************************************************
        // Modello UniCatt Mais AFLA/FER (44, 45)
        //*****************************************************************************************
        let lblDataEmergenza = document.createElement("div");
        lblDataEmergenza.style.cssText = "justify-self: end;";
        lblDataEmergenza.innerHTML = "<span>Data emergenza</span>";
        let divDataEmergenza = document.createElement("div");
        let inputDataEmergenza = document.createElement("input");
        inputDataEmergenza.id = "__dataemergenza44_45__";
        inputDataEmergenza.style.cssText = "width: -webkit-fill-available;";
        divDataEmergenza.appendChild(inputDataEmergenza);

        lblDataEmergenza.className = "param-modello mod-44 mod-45";
        divDataEmergenza.className = "param-modello mod-44 mod-45";

        grid.appendChild(lblDataEmergenza);
        grid.appendChild(divDataEmergenza);

        $("#__dataemergenza44_45__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(),
            footer: false
        });
        $("#__dataemergenza44_45__").attr("readonly", true);
        //*****************************************************************************************


        //*****************************************************************************************
        // Modello BetaCoProB Cercosporiosi (47)
        //*****************************************************************************************
        let lblDataPartenza = document.createElement("div");
        lblDataPartenza.style.cssText = "justify-self: end;";
        lblDataPartenza.innerHTML = "<span>Data partenza</span>";
        let divDataPartenza = document.createElement("div");
        let inputDataPartenza = document.createElement("input");
        inputDataPartenza.id = "__datapartenza47__";
        inputDataPartenza.style.cssText = "width: -webkit-fill-available;";
        divDataPartenza.appendChild(inputDataPartenza);

        lblDataPartenza.className = "param-modello mod-47";
        divDataPartenza.className = "param-modello mod-47";

        grid.appendChild(lblDataPartenza);
        grid.appendChild(divDataPartenza);

        $("#__datapartenza47__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(),
            footer: false
        });
        $("#__datapartenza47__").attr("readonly", true);
        //*****************************************************************************************


        //*****************************************************************************************
        //UniCatt Fusariosi Frumento (48)
        //*****************************************************************************************
        let lblDataSpigatura = document.createElement("div");
        lblDataSpigatura.style.cssText = "justify-self: end;";
        lblDataSpigatura.innerHTML = "<span>Data spigatura</span>";
        let divDataSpigatura = document.createElement("div");
        let inputDataSpigatura = document.createElement("input");
        inputDataSpigatura.id = "__dataspigatura48__";
        inputDataSpigatura.style.cssText = "width: -webkit-fill-available;";
        divDataSpigatura.appendChild(inputDataSpigatura);

        lblDataSpigatura.className = "param-modello mod-48";
        divDataSpigatura.className = "param-modello mod-48";

        grid.appendChild(lblDataSpigatura);
        grid.appendChild(divDataSpigatura);

        $("#__dataspigatura48__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(),
            footer: false
        });
        $("#__dataspigatura48__").attr("readonly", true);
        //*****************************************************************************************


        //*****************************************************************************************
        //Racca Frumento (51 - 60)
        //*****************************************************************************************
        let now = new Date();

        let class_51_60 = "mod-51 mod-52 mod-53 mod-54 mod-55 mod-56 mod-57 mod-58 mod-59 mod-60"

        let lblDataSemina51 = document.createElement("div");
        lblDataSemina51.style.cssText = "justify-self: end;";
        lblDataSemina51.innerHTML = "<span>Data semina</span>";
        let divDataSemina51 = document.createElement("div");
        let inputDataSemina51 = document.createElement("input");
        inputDataSemina51.id = "__datasemina51__";
        inputDataSemina51.style.cssText = "width: -webkit-fill-available;";
        divDataSemina51.appendChild(inputDataSemina51);

        lblDataSemina51.className = "param-modello " + class_51_60 ;
        divDataSemina51.className = "param-modello " + class_51_60;

        grid.appendChild(lblDataSemina51);
        grid.appendChild(divDataSemina51);

        $("#__datasemina51__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(now.getFullYear() - 1, 9, 1),
            footer: false
        });
        $("#__datasemina51__").attr("readonly", true);

        let lblDataRaccolta51 = document.createElement("div");
        lblDataRaccolta51.style.cssText = "justify-self: end;";
        lblDataRaccolta51.innerHTML = "<span>Data raccolta</span>";
        let divDataRaccolta51 = document.createElement("div");
        let inputDataRaccolta51 = document.createElement("input");
        inputDataRaccolta51.id = "__dataraccolta51__";
        inputDataRaccolta51.style.cssText = "width: -webkit-fill-available;";
        divDataRaccolta51.appendChild(inputDataRaccolta51);

        lblDataRaccolta51.className = "param-modello " + class_51_60;
        divDataRaccolta51.className = "param-modello " + class_51_60;

        grid.appendChild(lblDataRaccolta51);
        grid.appendChild(divDataRaccolta51);

        $("#__dataraccolta51__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(now.getFullYear(), 6, 1),
            footer: false
        });
        $("#__dataraccolta51__").attr("readonly", true);

        let lblResistenza51 = document.createElement("div");
        lblResistenza51.style.cssText = "justify-self: end;";
        lblResistenza51.innerHTML = "<span>Resistenza varietale</span>";
        let divResistenza51 = document.createElement("div");
        let inputResistenza51 = document.createElement("input");
        inputResistenza51.id = "__resistenza51__";
        inputResistenza51.style.cssText = "width: -webkit-fill-available;";
        divResistenza51.appendChild(inputResistenza51);

        lblResistenza51.className = "param-modello " + class_51_60;
        divResistenza51.className = "param-modello " + class_51_60;

        grid.appendChild(lblResistenza51);
        grid.appendChild(divResistenza51);

        $("#__resistenza51__").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: [
                { text: "Suscettibile", value: 0 },
                { text: "Medio resistente", value: 1 },
                { text: "Resistente", value: 2 }
            ],
            index: 0
        });

        //*****************************************************************************************

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        var _gestisciParametri = function (mod_cod) {

            $(plugin.element).find(".param-modello").each(function () {
                $(this).addClass("__hidden__");
            });

            let classname = "mod-" + mod_cod;
            $(plugin.element).find(".param-modello." + classname).each(function () {
                $(this).removeClass("__hidden__");
            });
        }

        let _doy = function (d) {

            let day = d.getDate();
            let month = d.getMonth();

            let ms = 1000 * 60 * 60 * 24;

            let d1 = Math.floor((new Date(1999, month, day)).getTime() / ms); //1999 Anno non bisestile
            let d0 = Math.floor((new Date(1999, 0, 1)).getTime() / ms);

            return d1 - d0 + 1;
        }

        let _d = function (doy) {

            let d = new Date((new Date(1999, 0, 1)).getTime() + ((doy - 1) * 24 * 60 * 60 * 1000));
            let year = (new Date()).getFullYear();
            let month = d.getMonth();
            let day = d.getDate();

            return new Date(year, month, day);
        }

        var _parametriModello = function (mod_cod) {

            let resParam = null;

            switch (parseInt(mod_cod)) {
                case 31:
                    let d1 = $("#__datasemina31__").getKendoDatePicker().value();
                    let v1 = $("#__varieta31__").getKendoDropDownList().text()

                    resParam = { DataSemina_gg: _doy(d1), Varieta: v1 };
                    break;

                case 42:
                    let d2 = $("#__datatrapianto42__").getKendoDatePicker().value();

                    resParam = { DataTrapianto_gg: _doy(d2) };
                    break;

                case 44:
                case 45:
                    let d3 = $("#__dataemergenza44_45__").getKendoDatePicker().value();

                    resParam = { DataEmergenza_gg: _doy(d3) };
                    break;

                case 47:
                    let d4 = $("#__datapartenza47__").getKendoDatePicker().value();

                    resParam = { DataPartenza_gg: _doy(d4) };
                    break;

                case 48:
                    let d5 = $("#__dataspigatura48__").getKendoDatePicker().value();

                    resParam = { DataSpigatura_gg: _doy(d5) };
                    break;

                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                    //kendo.toString($("#__datasemina51__").data("kendoDatePicker").value(), "yyyy-MM-ddTHH:mm:ss"),'
                    resParam = {
                        DoYSemina: _doy($("#__datasemina51__").data("kendoDatePicker").value()),
                        DoYRaccolta: _doy($("#__dataraccolta51__").data("kendoDatePicker").value()),
                        ResistenzaVarietale: parseInt($("#__resistenza51__").getKendoDropDownList().value()),
                    };
                    break;
            }

            return resParam;
        }

        let oggi = new Date();

        $("#__ini_period__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(oggi.getFullYear(), 0, 1),
            footer: false
        });

        $("#__fin_period__").kendoDatePicker({
            dateInput: true,
            format: "dd MMMM",
            value: new Date(oggi.getFullYear(), 11, 31),
            footer: false
        });

        let tdata = [];
        for (m = 30; m <= 12 * 60; m += 30) {
            let hh = Math.floor(m / 60);
            let mm = m - (hh * 60);
            tdata.push({
                text: kendo.format("{0:00}:{1:00}", hh, mm),
                value: m
            });
        }

        $("#__validita__").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: tdata,
            value: 60
        });

        $("#__veg__").kendoDropDownList({
            autoBind: false,
            autoWidth: true,
            dataTextField: "Veg_Des",
            dataValueField: "Veg_Cod",
            dataSource: {
                data: _data
            },
            change: function (e) {
                $("#__mod__").getKendoDropDownList().dataSource.read();
            }
        });

        var _leggiModelliXSpecie = function (options) {

            options.success($("#__veg__").getKendoDropDownList().dataItem().Modelli);
        };


        let tmpltCont = "<div style='display:flex; flex-direction:column; row-gap:7px;'>";
        tmpltCont += "<div style='font-weight:bold; display:contents;'>";
        tmpltCont += "<span style='overflow:hidden; text-overflow:ellipsis; white-space:nowrap;'>#:data.Full_Des#</span>";
        tmpltCont += "</div>";
        tmpltCont += "<div style='font-size:11px;'>";
        tmpltCont += "<span>#= data.Avv_Des ? data.Avv_Des : '&nbsp' #</span>";
        tmpltCont += "</div>";
        tmpltCont += "</div>";

        let avvValueTmplt = "<div style='width:100%; display:inline-block; max-width:750px;'>";
        avvValueTmplt += tmpltCont;
        avvValueTmplt += "</div>";

        let avvTmplt = "<div style='width:100%; display:inline-block; max-width:750px; padding-top:2px; padding-bottom:2px;'>";
        avvTmplt += tmpltCont;
        avvTmplt += "</div>";

        $("#__mod__").kendoDropDownList({
            autoBind: false,
            autoWidth: true,
            dataValueField: "Mod_Cod",
            dataTextField: "Full_Des",
            valueTemplate: avvValueTmplt,
            template: avvTmplt,
            dataSource: {
                transport: {
                    read: _leggiModelliXSpecie
                }
            },
            dataBound: function (e) {
                e.sender.select(0);
                e.sender.trigger("change");
            },
            change: function (e) {
                _gestisciParametri(e.sender.value());

                let inizio_doy = e.sender.dataItem().InizioPeriodo_gg;
                let fine_doy = e.sender.dataItem().FinePeriodo_gg;
                $("#__ini_period__").getKendoDatePicker().value(_d(inizio_doy));
                $("#__fin_period__").getKendoDatePicker().value(_d(fine_doy));
            }
        });

        let ddlSpecie = $("#__veg__").getKendoDropDownList();
        ddlSpecie.dataSource.read();
        ddlSpecie.select(0);
        ddlSpecie.trigger("change");

        plugin.getModello = function () {

            let vegCod = $("#__veg__").getKendoDropDownList().value();
            let modItem = $("#__mod__").getKendoDropDownList().dataItem();

            if (modItem === undefined) {
                return null;
            }

            let inizio_gg = _doy($("#__ini_period__").getKendoDatePicker().value());
            let fine_gg = _doy($("#__fin_period__").getKendoDatePicker().value());

            //controllo che il periodo di calcolo sia interno al periodo per la lettura dei dati meteo...
            let calc_0 = inizio_gg;
            let calc_1 = fine_gg;
            if (calc_1 < calc_0) {
                calc_1 += 365;
            }
            let meteo_0 = modItem.InizioPeriodo_gg;
            let meteo_1 = modItem.FinePeriodo_gg;
            if (meteo_1 < meteo_0) {
                meteo1 += 365;
            }
            if (calc_0 < meteo_0 || calc_1 > meteo_1) {
                let d0 = kendo.toString(_d(modItem.InizioPeriodo_gg), "d MMMM");
                let d1 = kendo.toString(_d(modItem.FinePeriodo_gg), "d MMMM");
                _errorfun("Il <b>periodo calcolo</b> è incompatibile con le impostazioni del modello.<br>Il periodo deve essere incluso nell'intervallo <b>" + d0 + "</b> - <b>" + d1 + "</b>.");
                return null;
            }

            let val_min = parseInt($("#__validita__").getKendoDropDownList().value());
            if (isNaN(val_min)) {
                val_min = 60;
            }

            let obj_modello = {
                veg_cod: vegCod,
                mod_cod: modItem.Mod_Cod,
                alg_cod: modItem.Alg_Cod,
                avv_cod: modItem.Avv_Cod,
                inizioperiodo_gg: inizio_gg,
                fineperiodo_gg: fine_gg,
                validita_minuti: val_min
            };

            let param = _parametriModello(modItem.Mod_Cod);
            if (param !== null) {
                obj_modello.param = param;
            }

            return obj_modello;
        }

    }; // pluginModelli

    //Add the plugin to the jQuery.fn object
    $.fn.pluginModelli = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('pluginModelli')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.pluginModelli(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('pluginModelli', plugin);
            }
        });
    };

})(jQuery);


