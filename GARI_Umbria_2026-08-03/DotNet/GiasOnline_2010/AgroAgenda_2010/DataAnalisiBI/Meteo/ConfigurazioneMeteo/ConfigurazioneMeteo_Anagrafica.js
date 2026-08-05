
function _ridimensiona_AnagraficaTabstrip(h) {

    $("#anagArea").height(h);

    $("#anagArea > div").each(function (i, e) {
        $(e).height(h);
    });
}

function _anagrafica_jQueryDocReady() {

    let stazTemplate = "<div class='list-view-item not-clickable'>";
    stazTemplate += "<div class='k-block anag-stazione-item'>";
    stazTemplate += "   <div class='stazione-hdr'>"
    stazTemplate += "       <div class='stazione-container clickable'>";
    stazTemplate += "           <div class='stazione-nome'><span>#:Staz_Nome#</span></div>";
    stazTemplate += "           <div class='stazione-fornitore'><span>#:Fornitore# [#:RifFornitore#]</span></div>";
    stazTemplate += "           <div class='staz-feat-container'>";
    stazTemplate += "#if (!FlagReale) {#";
    stazTemplate += "               <div class='staz-feat'><span class='fa fa-magic fa-fw'></span></div>";
    stazTemplate += "#}#";
    stazTemplate += "               <div class='staz-feat'><span class='#= Proprietario ? 'fa fa-unlock fa-fw' : 'fa fa-lock fa-fw' #'></span></div>";
    stazTemplate += "           </div>";
    stazTemplate += "       </div>";
    stazTemplate += "#if (Proprietario) {#";
    stazTemplate += "       <div class='k-button fa-btn btn-edit hidden-if-not-selected'><span class='fa fa-pencil'></span></div>";
    stazTemplate += "#}#";
    stazTemplate += "#if (!FlagReale) {#";
    stazTemplate += "       <div class='k-button fa-btn btn-trash hidden-if-not-selected'><span class='fa fa-trash-o'></span></div>";
    stazTemplate += "#}#";
    stazTemplate += "   </div>";
    stazTemplate += "   <div class='stazione-posizione-cont'>";
    stazTemplate += "       <div class='k-button fa-btn btn-globe #= data.Staz_Geo ? '' : '" + GIAS_K_STATE_DISABLED + "' #'><span class='fa fa-globe'></span></div>";
    stazTemplate += "       <div class='stazione-posizione #= data.Staz_Geo ? '' : '" + GIAS_K_STATE_DISABLED + "' #'>";
    stazTemplate += "#if (data.Staz_Geo) {#";
    stazTemplate += "           <div class='lat-lng'><span>#= kendo.toString(Staz_Geo.Lat, '0.0000000')#</span><span class='etichetta'>Lat</span></div>";
    stazTemplate += "           <div class='lat-lng'><span>#= kendo.toString(Staz_Geo.Lng, '0.0000000')#</span><span class='etichetta'>Lng</span></div>";
    stazTemplate += "#} else {#";
    stazTemplate += "           <span>" + TraduzioneMultiResx(datiMeteoResx, "PosizioneNonDisponibile", "Posizione non disponibile") + "</span>";
    stazTemplate += "#}#";
    stazTemplate += "       </div>";
    stazTemplate += "   </div>";

    stazTemplate += "<div class='collapsible-section collapsed'>";

    stazTemplate += "<div class='k-block stazione-sensori-cont sensore-background'>";// hidden-if-not-selected'>";
    stazTemplate += "#if (data.Sensori) {#";
    stazTemplate += "#for (let s = 0, len = Sensori.length; s < len; s++) {#";
    stazTemplate += "#let sens = Sensori[s];#";
    stazTemplate += "   <div class='list-view-item sensore stoppable-pointer-events'>";
    stazTemplate += "       <div class='draggable-container sensore-background'>";
    stazTemplate += "           <div class='drag-handle hide-placeholder'>";
    stazTemplate += "               <div class='handler'>";
    //stazTemplate += "                   <span class='dragging-off fa fa-ellipsis-v'></span>";
    //stazTemplate += "                   <span class='dragging-off fa fa-ellipsis-v'></span>";
    stazTemplate += "                   <span class='dragging-off k-icon k-i-handler-drag'></span>";
    stazTemplate += "                   <span class='dragging-on k-icon k-i-drag-and-drop'></span>";
    stazTemplate += "               </div>";
    stazTemplate += "           </div>";
    stazTemplate += "           <div class='draggable-full-width'>";
    stazTemplate += "               <div class='sensore-text'>";
    stazTemplate += "                   <span>#:sens.Sensore#</span>";
    stazTemplate += "                   <span>#:sens.Tipo_Sensore# #= sens.UM != '' ? '(' + sens.UM + ')' : '' #</span>";

    stazTemplate += "                   <div class='hide-drag-hint hide-placeholder spacer'></div>";
    stazTemplate += "                   <div class='hide-drag-hint hide-placeholder indicatore-sensore #=sens.Colore#'>";
    stazTemplate += "                       <div class='pointer-left'>";
    stazTemplate += "#if (sens.Last_Update) {#";
    stazTemplate += "                           <span>Aggiornato al #:sens.Last_Update#</span>";
    stazTemplate += "#} else {#";
    stazTemplate += "                           <span>Dati assenti</span>";
    stazTemplate += "#}#";
    stazTemplate += "                       </div>";
    stazTemplate += "                   </div>";

    stazTemplate += "               </div>";
    stazTemplate += "#if (sens.Id_StazioneOrigine > 0) {#";
    stazTemplate += "               <div class='stazione-rif'>#:sens.StazioneOrigine#</div>";
    stazTemplate += "#}#";
    stazTemplate += "           </div>";
    stazTemplate += "       </div>";
    stazTemplate += "   </div>";
    stazTemplate += "#}#";
    stazTemplate += "#}#";

    stazTemplate += "</div>";

    stazTemplate += "</div>";
    stazTemplate += "</div>";
    stazTemplate += "</div>";

    $("#stazSrc").kendoListView({
        dataSource: {
            transport: {
                read: LeggiStazioni
            },
            sort: { field: "Staz_Nome", dir: "asc" }
        },
        autoBind: false,
        selectable: true,
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(stazTemplate),
        dataBound: function (e) {
            let children = e.sender.content.children();

            $.each(children, function (i, child) {

                $(child).find(".btn-globe").each(function (i, e) {
                    e.onclick = function (ev) {
                        _anagStazioneGIS(ev);
                    }
                });

                $(child).find(".btn-edit").each(function (i, e) {
                    e.onclick = function () {
                        _editStazione();
                    }
                });

                $(child).find(".btn-trash").each(function (i, e) {
                    e.onclick = function () {
                        _deleteStazione();
                    }
                });

                let sensori_cont = $(child).find(".stazione-sensori-cont");
                if (sensori_cont.length > 0) {
                    sensori_cont = sensori_cont[0];

                    $(sensori_cont).find(".draggable-container").each(function (j, s) {

                        $("#sensDst").data("listViewDropTarget").addDraggable(s, _callbackDragstart);
                    });
                }
            });

            $.each(e.sender.dataSource.data(), function (i, staz) {
                $.each(staz.Sensori, function (j, sens) {
                    sens.Trashable = !staz.FlagReale;
                });
            });

            if (children.length > 0) {
                e.sender.select(children.first());
                e.sender.trigger("change");
            }
        },
        change: function (e) {

            let itemOut = $(e.sender.content).find(".stazione-state-selected").first();
            let itemIn = $(e.sender.select()[0]);
            let $itemOut = null;
            let $itemIn = null;
            if (itemOut.length > 0) {

                $itemOut = $(itemOut);
            }
            if (itemIn.length > 0) {

                $itemIn = $(itemIn);

                $itemIn.removeClass(GIAS_K_STATE_SELECTED);
            }

            if ($itemOut != null && $itemIn != null && $itemOut.index() === $itemIn.index()) {
                return;
            }

            if ($itemOut != null) {

                $itemOut.removeClass("stazione-state-selected");
                $itemOut.find(".anag-stazione-item").each(function (i, e) {
                    $(e).removeClass("k-shadow");
                });

                $itemOut.find(".collapsible-section").addClass("collapsed");
            }

            if ($itemIn != null) {

                $itemIn.addClass("stazione-state-selected");
                $itemIn.find(".anag-stazione-item").each(function (i, e) {
                    $(e).addClass("k-shadow");
                });

                let h = $itemIn.find(".stazione-sensori-cont").outerHeight(true);
                $itemIn.find(".collapsible-section").height(h);
                $itemIn.find(".collapsible-section").removeClass("collapsed");

                //$itemIn[0].scrollIntoView();
            }
        }
    });

    $("#anagStazioniFilter").filtroStazioni({
        placeholder: "Filtra stazioni...",
        listView: "#stazSrc",
        filter: {
            logic: "or",
            filters: [
                { field: "Staz_Nome", operator: "contains" },
                { field: "RifFornitore", operator: "contains" }
            ]
        }
    });



    let sensDstTemplate = "<div class='list-view-item sensore'>";
    sensDstTemplate += "    <div class='k-block draggable-container sensore sensore-background padded'>";
    sensDstTemplate += "        <div class='drag-handle hide-placeholder stoppable-pointer-events'>";
    sensDstTemplate += "            <div class='handler'>";
    sensDstTemplate += "                <span class='dragging-off k-icon k-i-kpi'></span>";
    sensDstTemplate += "                <span class='dragging-on k-icon k-i-drag-and-drop'></span>";
    sensDstTemplate += "            </div>";
    sensDstTemplate += "        </div>";
    sensDstTemplate += "        <div class='draggable-full-width'>";
    sensDstTemplate += "            <div class='sensore-text'>";
    sensDstTemplate += "                <span>#:Sensore#</span>";
    sensDstTemplate += "                <span>#:Tipo_Sensore# #= UM != '' ? '(' + UM + ')' : '' #</span>";
    sensDstTemplate += "#if (OutputConfig.serie) {#";
    sensDstTemplate += "                <div class='sensore-outputconfig' style='color: #:OutputConfig.colore#;'>";
    sensDstTemplate += "                <span class='fa fa-fw #=OutputConfig.serie == 'line' ? 'fa-line-chart' : 'fa-bar-chart'#'>";
    sensDstTemplate += "                </div>";
    sensDstTemplate += "#}#";
    sensDstTemplate += "            </div>";
    sensDstTemplate += "            <div class='stazione-rif'>#:StazioneOrigine#</div>";
    sensDstTemplate += "        </div>";
    sensDstTemplate += "        <div class='hide-placeholder hide-drag-hint' style='display: inline-flex;'>";
    sensDstTemplate += "            <div class='k-button fa-btn-small btn-edit'><span class='fa fa-pencil'></span></div>";
    sensDstTemplate += "#if (Trashable) {#";
    sensDstTemplate += "            <div class='k-button fa-btn-small btn-trash gias-btn-delete-white'><span class='fa fa-trash-o'></span></div>";
    sensDstTemplate += "#}#";
    sensDstTemplate += "        </div>";
    sensDstTemplate += "    </div>";
    sensDstTemplate += "</div>";


    $("#sensDst").kendoListView({
        dataSource: {
            data: []
        },
        layout: "flex",
        flex: {
            direction: "column",
            wrap: "nowrap"
        },
        template: kendo.template(sensDstTemplate),
        dataBinding: function (e) {

            $(e.sender.element).find(".list-view-no-data").each(function (i, e) {
                e.remove();
            });

            if (e.sender.dataSource.data() && e.sender.dataSource.data().length === 0) {
                let noDataElem = document.createElement('div');
                noDataElem.className = "list-view-no-data";
                noDataElem.style.cssText = "position: absolute; top: 50%; left: 50%; transform: translateX(-50%) translateY(-50%); color: #efefef; font-size: 20px; user-select: none;";
                noDataElem.innerHTML = TraduzioneMultiResx(datiMeteoResx, "TrascinaQuiUnSensore", "Trascina qui un sensore");
                e.sender.element.append(noDataElem);
            }
        },
        dataBound: function (e) {

            let ds = e.sender.dataSource;

            e.sender.content.find(".btn-trash").each(function (i, el) {
                el.onclick = function (ev) {

                    let uid = $(ev.target.closest(".list-view-item.sensore")).attr("data-uid");
                    let dataItem = ds.getByUid(uid);
                    if (dataItem !== undefined) {
                        ds.remove(dataItem);
                    }
                }
            });

            e.sender.content.find(".btn-edit").each(function (i, el) {
                el.onclick = function () {
                    _editSensore(el);
                }
            });
        }
    });


    $("#sensDst").listViewDropTarget({
        sourceElement: $("#stazSrc")
    });

    document.getElementById("btnEditConferma").onclick = function () { Stazioni_stazioneEditConferma(); };
    document.getElementById("btnEditAnnulla").onclick = Stazioni_stazioneEditAnnulla;

    $("#input-nome-stazione").kendoTextBox({
        placeholder: TraduzioneMultiResx(datiMeteoResx, "placeholderNuovaStazione", "Nome stazione...")
    });

    $("#input-pos-stazione").geoPosEdit({
        mapAction: function (sender) {

            let edit_id = parseInt($("#" + IdStazioneEdit).val());

            let arrGeo = [];
            let staz_ds = $("#stazSrc").getKendoListView().dataSource;

            $.each(staz_ds.view(), function (i, s) {

                if (s.Staz_Id !== edit_id && s.Staz_Geo !== undefined) {

                    arrGeo.push({
                        label: s.Staz_Nome,
                        lat: s.Staz_Geo.Lat,
                        lng: s.Staz_Geo.Lng
                    });
                }
            });

            openMapCallback(sender, arrGeo);
        }
    });

    //$("#stazSrc").data("kendoListView").dataSource.read()

    Stazione_ImpostaEditMode(null);

    $("#notifica-edit").kendoNotification({
        appendTo: "#container-notifica-edit",
        animation: {
            open: {
                effects: "zoom:in" //"expand:vertical"
            },
            close: false
        },
        //templates: [{
        //    type: "customError",
        //    template: "<div class='k-block k-error-colored'>#:message#</div>"
        //}],
        autoHideAfter: 0
    });

    let loader = document.createElement("div");
    loader.className = "list-loader";
    loader.style.cssText = "position:absolute; left:50%; top:50%; transform:translate(-50%, -50%);";
    document.getElementById("stazSrc").appendChild(loader);
    $(loader).StyleLoader();
}

function LeggiStazioni(options) {

    let stazArr = [];
    let param = {};
    ajaxAgronicaSync(url_meteows + "/LeggiAnagraficaStazioni",
        JSON.stringify(param),
        false,
        function (risposta) {

            let risp = risposta.RispostaStringa;
            stazArr = JSON.parse(risp);

        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        },
        null,
        false
    );

    $.each(stazArr, function (i, staz) {
        $.each(staz.Sensori, function (j, sens) {
            if (sens.hasOwnProperty("Last_Update")) {
                let upd = new Date(sens.Last_Update);
                let hours = Math.abs((new Date()).getTime() - upd.getTime()) / (60 * 60 * 1000);

                if (hours > 120) {
                    sens.Colore = "rosso";
                } else if (hours > 24) {
                    sens.Colore = "giallo";
                } else {
                    sens.Colore = "verde";
                }

                if (upd.getTime() < (new Date(1970, 0, 1)).getTime()) {

                    sens.Colore = "viola";
                    sens.Last_Update = "";

                } else {

                    sens.Last_Update = kendo.toString(upd, "g");
                }
            }
        });
    });

    $("#stazSrc").find(".list-loader").remove();

    options.success(stazArr);
}

function Stazione_ImpostaEditMode(stazItem) {

    $("#lblEditAnnulla").html(TraduzioneMultiResx(datiMeteoResx, "labelEditAnnullaTutto", "Annulla tutto"));

    let staz_id = 0;
    let label = TraduzioneMultiResx(datiMeteoResx, "labelNuovaStazione", "Nuova stazione");
    let nome_stazione = "";
    let geopos = null;
    let allowChange = true;
    let sensData = [];

    if (stazItem !== null) {

        $("#notifica-edit").getKendoNotification().hide();

        staz_id = stazItem.Staz_Id;
        label = TraduzioneMultiResx(datiMeteoResx, "labelModificaStazione", "Modifica stazione");
        nome_stazione = stazItem.Staz_Nome;
        if (stazItem.Staz_Geo !== undefined) {

            geopos = stazItem.Staz_Geo;
        }

        allowChange = !stazItem.FlagReale

        $.each(stazItem.Sensori, function (i, srcItem) {
            //let dstItem = jQuery.extend(true, {}, srcItem);
            let dstItem = Object.assign({}, srcItem); //Shallow copy... need Deep copy per OutputConfig...
            if (srcItem.OutputConfig) {
                dstItem.OutputConfig = Object.assign({}, srcItem.OutputConfig);
            }
            if (dstItem.Id_StazioneOrigine === 0) {
                dstItem.StazioneOrigine = stazItem.Staz_Nome;
            }
            sensData.push(dstItem);
        });
    }

    $("#" + IdStazioneEdit).val(staz_id);
    $("#dstEditMode").html(label);
    $("#input-nome-stazione").getKendoTextBox().value(nome_stazione);
    if (geopos !== null) {

        $("#input-pos-stazione").data("geoPosEdit").setLatLngDec(geopos.Lat, geopos.Lng);

    } else {

        $("#input-pos-stazione").data("geoPosEdit").clear();
    }

    $("#sensDst").data("listViewDropTarget").allowChange(allowChange);

    $("#sensDst").getKendoListView().dataSource.data(sensData);
}

function Stazioni_getSelected() {

    let stazLV = $("#stazSrc").data("kendoListView");
    let selected = $(stazLV.element).find(".stazione-state-selected").first();
    if (selected.length > 0) {

        let uid = selected.attr("data-uid");
        let staz = stazLV.dataSource.getByUid(uid);

        if (typeof staz === "object") {

            return staz;
        }
    }
    return null;
}

function _callbackDragstart(index) {

    let stazObj = Stazioni_getSelected();
    if (stazObj === null) {
        return null;
    }

    if (stazObj.Sensori === undefined) {
        return null;
    }

    if (index >= stazObj.Sensori.length) {
        return null;
    }

    let src_item = stazObj.Sensori[index];
    let dst_item = Object.assign({}, src_item);
    if (dst_item.Id_StazioneOrigine === 0) {
        dst_item.StazioneOrigine = stazObj.Staz_Nome;
    }
    dst_item.Trashable = true;

    return dst_item;
}

function _anagStazioneGIS(ev) {

    ev.stopPropagation();

    let uid = $(ev.currentTarget).closest(".list-view-item").attr("data-uid");
    let stazLV = $("#stazSrc").data("kendoListView");
    let objStaz = stazLV.dataSource.getByUid(uid);
    if (typeof objStaz !== "object") {
        return;
    }
    if (objStaz.Staz_Geo === undefined) {
        return;
    }

    map_wnd([
        {
            label: objStaz.Staz_Nome,
            geo: {
                lat: objStaz.Staz_Geo.Lat,
                lng: objStaz.Staz_Geo.Lng
            }
        }
    ]);
}

function _deleteStazione() {

    let objStaz = Stazioni_getSelected();
    if (objStaz === null) {
        return;
    }

    if (objStaz.FlagReale) {
        return;
    }

    //Verifico che non sia in editing...
    let edit_id = parseInt($("#" + IdStazioneEdit).val());
    if (edit_id === objStaz.Staz_Id) {

        info_dlg(
            TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgEliminaStazione", "Elimina stazione"),
            "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgEliminaStazione1", "La stazione è in modifica, annullare l'operazione per proseguire") + "</div>"
        );

        return;
    }

    yesno_dlg(
        TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEliminaStazione", "Elimina stazione"),
        "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraEliminaStazione2", "Eliminare definitivamente la stazione?") + "</div>",
        function () {
            _execDeleteStazione(objStaz);
        }
    );
}

function _execDeleteStazione(objStaz) {

    let canDelete = false;
    ajaxAgronicaSync(url_meteows + "/ControllaEliminaStazione",
        JSON.stringify({ Id_Stazione: objStaz.Staz_Id }),
        false,
        function (risposta) {

            let risp = JSON.parse(risposta.RispostaStringa);

            canDelete = (risp.Error === 0);
        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    if (!canDelete) {

        info_dlg(
            TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgEliminaStazione", "Elimina stazione"),
            "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgEliminaStazione3", "La stazione non può essere eliminata, potrebbe essere associata ad una applicazione") + "</div>"
        );
        return;
    }

    let klv = $("#stazSrc").getKendoListView();
    klv.dataSource.remove(objStaz);
}

function _editStazione() {

    if ($("#sensDst").getKendoListView().dataSource.data().length > 0) {

        info_dlg(
            TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgModificaStazione", "Modifica stazione"),
            "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgModificaStazione", "L'elenco in modifica non è vuoto, svuotare l'elenco per proseguire") + "</div>"
        );
        return;
    }

    let objStaz = Stazioni_getSelected();
    if (objStaz === null) {
        return;
    }

    if (!objStaz.Proprietario) {
        return;
    }

    Stazione_ImpostaEditMode(objStaz);
}

function _editSensore(elem) {

    let uid = elem.closest(".list-view-item.sensore").getAttribute("data-uid");
    let sensoreItem = $("#sensDst").getKendoListView().dataSource.getByUid(uid);
    if (sensoreItem === undefined) {
        return;
    }

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let content = "<div style='display: grid; grid-template-columns: auto 1fr; grid-gap: 10px 5px; align-items: baseline;'>";
    content += "<label for='input-etichetta'>" + TraduzioneMultiResx(datiMeteoResx, "labelEtichettaSensore", "Etichetta:") + "</label>";
    content += "<input id='input-etichetta' type='text' class='k-textbox' style='width: 30em;'>";
    content += "<div><span>" + TraduzioneMultiResx(datiMeteoResx, "labelChartSensore", "Serie grafico:") + "</span></div>";
    content += "<div style='display: flex'>";
    content += "<div style='flex: 1; padding-right: 5px;'><input id='input-chartserie' style='width: 100%;'></div>";
    content += "<div style='flex: 1; padding-left: 5px;'><input id='input-chartcolor'></div>";
    content += "</div>";

    content += "</div>";

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEditSensore", "Modifica sensore"),
        closable: false,
        modal: true,
        visible: false,
        content: content,
        open: function () {

            $("#input-etichetta").val(sensoreItem.Sensore);
            $("#input-chartserie").kendoDropDownList({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: [
                    { text: "Linea", value: "line", icon: "fa-line-chart" }, //"k-i-line-stacked" },
                    { text: "Barre", value: "column", icon: "fa-bar-chart" } //"k-i-column-clustered" }
                ],
                template: "<span class='fa #:data.icon# fa-lg fa-fw'></span>",
                valueTemplate: "<span class='fa #:data.icon# fa-lg fa-fw'></span>"
            });
            $("#input-chartcolor").kendoColorPicker({
                buttons: true,
                messages: {
                    apply: "OK",
                    cancel: "Annulla"
                }
            });

            let cp_wrapper = $("#input-chartcolor").getKendoColorPicker().wrapper;
            cp_wrapper.css("width", "100%");
            cp_wrapper.find(".k-selected-color").css("width", "100%");

            let serie = "line";
            let color = "#DDDDDD";
            if (sensoreItem.OutputConfig.hasOwnProperty("serie")) {
                serie = sensoreItem.OutputConfig.serie;
            }
            if (sensoreItem.OutputConfig.hasOwnProperty("colore")) {
                color = sensoreItem.OutputConfig.colore;
            }

            $("#input-chartserie").getKendoDropDownList().value(serie);
            $("#input-chartcolor").getKendoColorPicker().value(color);
        },
        actions: [
            {
                text: 'OK',
                action: function () {

                    let etichetta = $("#input-etichetta").val();
                    etichetta = etichetta.trim();

                    sensoreItem.Sensore = etichetta;

                    sensoreItem.OutputConfig.serie = $("#input-chartserie").getKendoDropDownList().value();
                    let hexColor = $("#input-chartcolor").getKendoColorPicker().value();
                    sensoreItem.OutputConfig.colore = "rgb(" + hexColor.replace(/^#?([a-f\d])([a-f\d])([a-f\d])$/i
                        , (m, r, g, b) => '#' + r + r + g + g + b + b)
                        .substring(1).match(/.{2}/g)
                        .map(x => parseInt(x, 16))
                        .join(",") + ")";

                    $("#sensDst").getKendoListView().refresh();
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

function Stazioni_stazioneEditConferma(liv) {

    $("#notifica-edit").getKendoNotification().hide();

    if (!liv) {
        liv = 0;
    }

    let stazNome = $("#input-nome-stazione").getKendoTextBox().value();
    let stazGeoPos = $("#input-pos-stazione").data("geoPosEdit").getLatLngDec();
    let sensori = $("#sensDst").getKendoListView().dataSource.data();

    if (liv === 0) {

        if (stazNome == "") {
            //info_dlg(
            //    TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgSalvataggioStazione", "Salvataggio stazione"),
            //    "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioStazioneNomeVuoto", "Il nome della stazione non può essere vuoto...") + "</div>"
            //);
            $("#notifica-edit").getKendoNotification().error(
                TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioStazioneNomeVuoto", "Il nome della stazione non può essere vuoto...")
            );
            return;
        }

        if (sensori.length === 0) {
            //info_dlg(
            //    TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgSalvataggioStazione", "Salvataggio stazione"),
            //    "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioStazioneSensoriVuoto", "L'elenco dei sensori della stazione non può essere vuoto...") + "</div>"
            //);
            $("#notifica-edit").getKendoNotification().error(
                TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioStazioneSensoriVuoto", "L'elenco dei sensori della stazione non può essere vuoto...")
            );
            return;
        }

        if (stazGeoPos === null) {

            yesno_dlg(
                TraduzioneMultiResx(datiMeteoResx, "titoloFinestraMsgSalvataggioStazione", "Salvataggio stazione"),
                "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioStazioneNoGeoPos", "La stazione non è geolocalizzata, proseguire ugualmente?") + "</div>",
                function () {

                    Stazioni_stazioneEditConferma(1);
                }
            );

            return;

        } else {

            liv = 1;
        }
    }

    if (liv === 1) {

        let staz_id = parseInt($("#" + IdStazioneEdit).val());
        if (isNaN(staz_id)) {
            return;
        }

        let StazItem = {
            Staz_Id: staz_id,
            Staz_Nome: stazNome,
            Sensori: []
        };
        if (stazGeoPos !== null) {
            StazItem.Staz_Geo = { Lat: stazGeoPos.lat, Lng: stazGeoPos.lng };
        }
        $.each(sensori, function (i, s) {
            StazItem.Sensori.push({
                Id_Sensore: s.Id_Sensore,
                Etichetta: s.Sensore,
                OutputConfig: s.OutputConfig
            });

        });

        let loader = document.createElement("div");
        loader.className = "editAnag-loader";
        loader.style.cssText = "position:absolute; width:100%; height:100%; background-color:rgba(255, 255, 255, 0.75); z-index:100;";
        document.getElementById("anagEditArea").appendChild(loader);
        $(loader).StyleLoader();

        setTimeout(function () {
            _writeStazione(StazItem);
            $("#anagEditArea").find(".editAnag-loader").remove();
        }, 500);
    }
}

function _writeStazione(StazItem) {

    let oper = ["inserimento", "inserita"];
    if (StazItem.Staz_Id > 0) {
        oper = ["modifica", "modificata"];
    }

    let anagrafica = JSON.stringify(StazItem);
    StazItem = null;

    ajaxAgronicaSync(url_meteows + "/AggiornaAnagraficaStazione",
        JSON.stringify({ jsonAnag: anagrafica }),
        false,
        function (risposta) {

            let risp = risposta.RispostaStringa;
            StazItem = JSON.parse(risp);

        },
        function (risposta) {
            //meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );

    if (StazItem === null) {
        //Errore in aggiornamento DB
        $("#notifica-edit").getKendoNotification().error(
            TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioErrore", "Errore in " + oper[0] + " stazione")
        );
        return;
    }

    $("#anagStazioniFilter").data("filtroStazioni").clearFilter();

    let StazId2Select = StazItem.Staz_Id;

    let klv = $("#stazSrc").getKendoListView();

    klv.dataSource.read();

    let sort = klv.dataSource.sort();
    if (sort !== undefined) {
        klv.dataSource.sort(sort);
    }

    let children = klv.content.children();
    let selected = null;
    let ichild = 0;
    while (selected === null && ichild < children.length) {

        let child = children[ichild];
        let uid = $(child).attr("data-uid");
        let dataItem = klv.dataSource.getByUid(uid);

        if (dataItem !== undefined) {
            if (dataItem.Staz_Id === StazId2Select) {
                selected = child;
            }
        }
        ichild++;
    }

    if (selected !== null) {
        klv.select(selected);
        klv.trigger("change");
        selected.scrollIntoView();
    }

    Stazione_ImpostaEditMode(null);

    $("#stazSrc").attr("data-reload", "1");

    $("#notifica-edit").getKendoNotification().success(
        TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraMsgSalvataggioOK", "Stazione " + oper[1] + " correttamente")
    );

    //setTimeout(function () {
    //    $("#notifica-edit").getKendoNotification().hide();
    //}, 5000);
}

function Stazioni_stazioneEditAnnulla() {

    $("#notifica-edit").getKendoNotification().hide();

    if ($("#sensDst").getKendoListView().dataSource.data().length === 0) {

        Stazione_ImpostaEditMode(null);
        return;
    }

    yesno_dlg(
        TraduzioneMultiResx(datiMeteoResx, "titoloFinestraEditAnnulla", "Modifica stazione"),
        "<div>" + TraduzioneMultiResx(datiMeteoResx, "contenutoFinestraEditAnnulla", "Annullare le modifiche?") + "</div>",
        function () {

            Stazione_ImpostaEditMode(null);
        }
    )
}

