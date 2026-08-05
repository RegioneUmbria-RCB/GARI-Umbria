
var UtilityWorker = null;

function MostraIndicatori(div, indicatori) {

    if (indicatori === undefined || indicatori === null || indicatori.length === 0) {
        //non ci sono consigli irrigui disponibili...
        return;
    }

    //elimino i non transcodificati...
    let idx = 0;
    while (idx < indicatori.length) {

        let oIndic = indicatori[idx];

        if (oIndic.Irri_Veg_Cod < 0) {

            indicatori.splice(idx, 1);

        } else {

            idx++;
        }
    }

    if (indicatori.length === 0) {
        return;
    }

    let allowPersistence = false;

    ajaxAgronicaSync(window.location.href.split("?")[0] + "/IrriframeAllowPersitence",
        JSON.stringify({}),
        false,
        function (risposta) {

            let res = JSON.parse(risposta.RispostaStringa);
            allowPersistence = res.AllowPersistence;
        },
        function () {
        },
        null,
        false
    );


    let wrapper = document.getElementById(div);

    let Centro = "";
    let NumCentri = 0;
    for (let i0 = 0; i0 < indicatori.length; i0++) {
        if (indicatori[i0].Centro != Centro) {
            NumCentri++;
            Centro = indicatori[i0].Centro;
        }
    }

    let divIndics;
    let RitardaCalcolo = NumCentri > 2;
    let classCentroContainer = "";
    let classCentroCollapsible = "";
    if (RitardaCalcolo) {
        classCentroContainer += " collapsed";
    } else {
        classCentroCollapsible += " in";
    }

    let stazioniXcentro = [];

    Centro = "";
    for (let i0 = 0; i0 < indicatori.length; i0++) {

        let indicatore = indicatori[i0];

        if (indicatore.Centro != Centro) {

            let idIndicCentro = "idIndicCentro" + (i0 + 1).toString();

            let divCentro = createDiv("indic-hdr");
            let divCentroContainer = createDiv("k-block", "background-color: #fcfcfc;");
            let divCentroCollapsible = createDiv("indic-collapsible" + classCentroContainer);
            divCentroCollapsible.setAttribute("data-toggle", "collapse");
            divCentroCollapsible.setAttribute("data-target", "#" + idIndicCentro);

            let spanIcon = document.createElement("span");
            spanIcon.className = "collapse-icon fa fa-chevron-down";
            let spanHome = document.createElement("span");
            spanHome.className = "fa fa-home fa-fw";
            let spanText = document.createElement("span");
            spanText.innerHTML = indicatore.Centro;

            divCentroCollapsible.appendChild(spanIcon);
            divCentroCollapsible.appendChild(spanHome);
            divCentroCollapsible.appendChild(spanText);

            let divStazioneWrapper = createDiv("", "flex-grow: 1; font-size: 13px; margin-left: 10px; color: #9e9e9e;");
            let divStazione = createDiv("", "float: right;");
            divStazione.id = idIndicCentro + "-stazione";
            divStazioneWrapper.appendChild(divStazione);
            divCentroCollapsible.appendChild(divStazioneWrapper);

            divCentroContainer.appendChild(divCentroCollapsible);

            divCentro.appendChild(divCentroContainer);

            wrapper.appendChild(divCentro);

            let divIndics0 = createDiv("collapse" + classCentroCollapsible);
            divIndics0.id = idIndicCentro;
            divIndics = createDiv("indic-grid");
            divIndics0.appendChild(divIndics);
            divCentroContainer.appendChild(divIndics0);

            Centro = indicatore.Centro;

            //*** li creo collassati
            //attivo le richiesta solo quando espando il centro...
            if (RitardaCalcolo) {
                let ev = "show.bs.collapse";//"shown.bs.collapse"
                $("#" + idIndicCentro).on(ev, function () {
                    $("#" + idIndicCentro + " > .indic-grid > .indic-elem ").each(function (idx, elem) {
                        let plugin = $(elem).data("WBResult");
                        if (plugin !== undefined) {
                            plugin.run();
                        }
                    });
                    $("#" + idIndicCentro).off(ev);
                });
            }

            if (indicatore.Stazioni_Meteo !== undefined && indicatore.Stazioni_Meteo !== null && indicatore.Stazioni_Meteo.length > 0) {

                stazioniXcentro.push({
                    elem_id: divStazione.id,
                    sorgente: indicatore.Stazioni_Meteo[0].Sorgente,
                    stazione: indicatore.Stazioni_Meteo[0].Stazione
                });
            }
        }

        let indWrapper = createDiv("k-block k-shadow indic-elem");
        divIndics.appendChild(indWrapper);

        $(indWrapper).WBResult(indicatore, allowPersistence);

        if (!RitardaCalcolo) {
            $(indWrapper).data("WBResult").run();
        }
    }

    $("#" + div).kendoTooltip({
        filter: ".indic-tooltip",
        showOn: "mouseenter",
        position: "top", 
        content: function (e) {
            let target = e.target; 
            return target.attr("data-tooltip");
        }
    });

    $("#" + div).kendoTooltip({
        filter: ".indic-parametri",
        showOn: "click",
        position: "bottom", 
        show: function (e) {
            this.popup.element.addClass("info-tooltip");
        },
        content: function (e) {
            let target = e.target; // the element for which the tooltip is shown
            let info = JSON.parse(target.attr("data-tooltip"));

            let content = "<div style='display:grid; grid-template-columns:auto auto; align-items:center; grid-gap: 10px; white-space:nowrap;'>";
            $.each(info, function (i, e) {
                let value = e.value;
                let style = "font-weight: bold;"
                if (value === "") {
                    value = "Non disponibile";
                    style = "opacity: 0.5;"
                }
                content += "<div style='justify-self: end;'>" + e.text + "</div>";
                content += "<div style='" + style + "'>" + value + "</div>";
            });
            content += "</div>";

            return content;
        }
    });

    if (stazioniXcentro.length > 0) {

        if (typeof window.Worker === "function") {

            Utility_Worker = new Worker("DSS_APP_Utility_Worker.js");

            $(window).bind('beforeunload', function () {

                Utility_Worker.terminate();
            });

            Utility_Worker.addEventListener("message", function (e) {

                $.each(e.data.elem_id, function (i, elem_id) {

                    let elem = document.getElementById(elem_id);
                    if (elem !== null) {

                        let nome = document.createElement("span");
                        nome.innerText = e.data.stazione.stazione_name;
                        elem.appendChild(nome);

                        let forn = document.createElement("span");
                        forn.style.marginLeft = "5px";
                        forn.style.fontSize = "10px";
                        forn.innerText = e.data.stazione.rif_fornitore
                        elem.appendChild(forn);
                    }
                });
            });

            Utility_Worker.postMessage({
                id: 1,
                baseurl: url_meteows,
                stazioniXcentro: stazioniXcentro
            });
        }
    }
}

function createDiv(_class, _style) {
    let div = document.createElement("div");
    if (typeof _class === "string" && _class !== "") {
        div.className = _class;
    }
    if (typeof _style === "string" && _style !== "") {
        div.style.cssText = _style;
    }
    return div;
}

function FiltraIndicatori(vegArray) {

    $(".indic-grid > .indic-elem").each(function (idx, elem) {
        let plugin = $(elem).data("WBResult");
        if (plugin !== undefined) {

            plugin.filter(vegArray);
        }
    });

    $(".indic-hdr .indic-grid").each(function (idx, elem) {
        let all_filtered = ($(elem).find(".indic-elem:not(.indic-filtered-out)").length === 0);
        let indic_hdr = $(elem).closest(".indic-hdr");
        if (all_filtered) {
            indic_hdr.addClass("indic-filtered-out");
        } else {
            indic_hdr.removeClass("indic-filtered-out");
        }
    });
}


const Irrigazioni_GIAS = 1;
const Irrigazioni_IF = 2;
const Bilancio_IF = 3;


(function ($) {

    $.WBResult = function (elem, indic, persistent) {

        var _creaDiv = function(_class, _style, _id, _txt) {
            let div = document.createElement("div");
            if (typeof _class === "string" && _class !== "") {
                div.className = _class;
            }
            if (typeof _style === "string" && _style !== "") {
                div.style.cssText = _style;
            }
            if (typeof _id === "string" && _id !== "") {
                div.id = _id;
            }
            if (typeof _txt === "string" && _txt !== "") {
                div.innerHTML = _txt;
            }
            return div;
        };

        var _clearElement = function(elem) {
            while (elem.firstChild) {
                elem.removeChild(elem.firstChild);
            }
        };

        var _getElemAndClear = function (selector) {

            let elem = plugin.$body.find(selector);
            if (elem.length == 0) {
                return null;
            }
            elem = elem[0];

            _clearElement(elem);

            $(elem).removeClass("valid");
            $(elem).removeClass("not-valid");

            return elem;
        };

        var _creaBtn = function (parent, classname, fa_icon, text) {
            let btn = _creaDiv("k-button k-button-md k-rounded-md " + classname);
            btn.appendChild(_creaDiv("", "", "", "<span class='fa " + fa_icon + " fa-lg fa-fw'></span>"));
            btn.appendChild(_creaDiv("overflow-ellipsis", "", "", text));
            parent.appendChild(btn);
            return btn;
        }

        var _sayLatLng = function () {

            let elem = _getElemAndClear(".lat-lng");
            if (elem == null) {
                return;
            }

            if (plugin.indicatore.LatLng === null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "indic-tooltip", "fa-map-marker", "Geolocalizza");
                let indirizzo = "";
                if (plugin.indicatore.hasOwnProperty("Indirizzo")) {
                    if (plugin.indicatore.Indirizzo != "") {
                        indirizzo = "<div style='margin-top:10px; font-style:italic;'>" + plugin.indicatore.Indirizzo + "</div>";
                    }
                }
                btn.setAttribute("data-tooltip", "<div style='text-align:center;'><div>Geolocalizza da indirizzo appezzamento/centro</div>" + indirizzo + "</div>");
                btn.onclick = function () { _geolocalizza(); };

                return;
            }

            $(elem).addClass("label-small").addClass("valid");

            let colLat = _creaDiv();
            elem.appendChild(colLat);
            colLat.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.indicatore.LatLng.Lat, "0.0000000") + "</span>"));
            colLat.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lat</span>"));

            let colLng = _creaDiv();
            elem.appendChild(colLng);
            colLng.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.indicatore.LatLng.Lng, "0.0000000") + "</span>"));
            colLng.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lng</span>"));
        };

        var _sayStart = function () {

            let elem = _getElemAndClear(".data-start");
            if (elem == null) {
                return;
            }

            if (plugin.indicatore.DataFaseStart == null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "", "fa-calendar", "Imposta data start")
                btn.onclick = function () { _impostaStart(); };

                return;
            }

            let start = kendo.toString(plugin.indicatore.DataFaseStart, "d");
            $(elem).addClass("label-small valid");
            elem.appendChild(_creaDiv("overflow-ellipsis", "", "", start));
            let divQuestion = _creaDiv("indic-tooltip", "color: #337ab7;", "", "<span class='fa fa-question-circle fa-lg fa-fw'></span>");
            divQuestion.setAttribute("data-tooltip", "Data semina, trapianto o risveglio vegetativo");
            elem.appendChild(divQuestion);
        };

        let divWrapper = _creaDiv("indic-wrapper");
        elem.appendChild(divWrapper);

        let divBody = _creaDiv("indic-body");
        divWrapper.appendChild(divBody);

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.gridElement = elem;
        plugin.$wrapper = $(divWrapper);
        plugin.wrapper = divWrapper;
        plugin.$body = $(divBody);  
        plugin.body = divBody;
        plugin.persistent = persistent
        plugin.indicatore = indic;
        plugin.indicatore.UsaMeteo = false;
        plugin.indicatore.IrriChoice = Irrigazioni_GIAS;

        if (plugin.indicatore.DataFaseStart != null) {
            plugin.indicatore.DataFaseStart = new Date(plugin.indicatore.DataFaseStart);
        }

        let campo = plugin.indicatore.Campo;
        if (campo != "") {
            campo += " - ";
        }
        let divTitle = _creaDiv("label-big overflow-ellipsis", "font-weight: bold;", "", campo + plugin.indicatore.Impianto);

        let varieta = plugin.indicatore.Varieta;
        if (varieta != "") {
            varieta = " - " + varieta;
        }
        let divSpecie = _creaDiv("label-big overflow-ellipsis", "overflow: hidden, position: relative", "", "");
        let divLblSpecie = _creaDiv("scrolling-text", "", "", plugin.indicatore.Coltura + varieta)
        divSpecie.appendChild(divLblSpecie);

        divBody.appendChild(divTitle);
        divBody.appendChild(divSpecie);

        let divParams = _creaDiv("grid-params")

        divBody.appendChild(divParams);

        let divFooter = _creaDiv("", "margin-top: 5px; padding-top: 5px; border-top: 1px solid #cccccc;");
        divBody.appendChild(divFooter);
        //divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-globe fa-lg fa-fw'></span>"));
        //let divCoord = _creaDiv("lat-lng");
        //divParams.appendChild(divCoord);
        //_sayLatLng();

        //divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-leaf fa-lg fa-fw'></span>"));
        //let divStart = _creaDiv("data-start");
        //divParams.appendChild(divStart);
        _sayStart();

        let warn = false;
        let fa = "fa-info-circle";
        let ipar = 0;
        while (!warn && ipar < plugin.indicatore.Info.length) {
            if (plugin.indicatore.Info[ipar].value === "") {
                warn = true;
                fa = "fa-exclamation-triangle";
            }
            ipar++;
        }
        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-tags fa-lg fa-fw'></span>"));
        let divPlusParams = _creaDiv("indic-parametri label-small", "cursor: pointer;");
        divParams.appendChild(divPlusParams);
        divPlusParams.appendChild(_creaDiv("overflow-ellipsis", "", "", "Ambientali/Impianto"));
        divPlusParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='fa " + fa + " fa-lg fa-fw'></span>"));
        divPlusParams.setAttribute("data-tooltip", JSON.stringify(plugin.indicatore.Info));

        plugin.result = _creaDiv();
        divWrapper.appendChild(plugin.result);


        let AllowPropMeteo = false;
        //if (plugin.indicatore.Stazioni_Meteo !== undefined && plugin.indicatore.Stazioni_Meteo != null && plugin.indicatore.Stazioni_Meteo.length > 0) {
        //    AllowPropMeteo = true;
        //    plugin.indicatore.UsaMeteo = true;
        //}
        if (divLblSpecie.scrollWidth > divSpecie.offsetWidth) {
            divLblSpecie.classList.add('scrolling');
        }

        /*
        let label = document.createElement("label");
        label.className = "toggle-switch";
        divFooter.appendChild(label);
        let checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        label.appendChild(checkbox);
        let back = _creaDiv("toggle-back"); 
        label.appendChild(back);
        let toggle = _creaDiv("toggle");
        let lbl_on = _creaDiv("toggle-label on", "", "", "Dati meteo proprietari"); 
        let lbl_off = _creaDiv("toggle-label off", "", "", "Dati meteo pubblici"); 
        back.appendChild(toggle);
        back.appendChild(lbl_on);
        back.appendChild(lbl_off);

        if (!AllowPropMeteo) {

            checkbox.disabled = true;

        } else {

            checkbox.checked = plugin.indicatore.UsaMeteo;
        }

        $(checkbox).on('change', function () {

            let value = this.checked;

            if (plugin.indicatore.UsaMeteo === value) {
                return;
            }

            plugin.indicatore.UsaMeteo = value;

            plugin.run();
        });
        */
        plugin.indicatore.UsaMeteo = false;


        //let divFooter2 = _creaDiv("", "padding-top: 5px");
        //elem.appendChild(divFooter2);
        //let label2 = document.createElement("label");
        //label2.className = "toggle-switch";
        //divFooter2.appendChild(label2);
        //let checkbox2 = document.createElement("input");
        //checkbox2.type = "checkbox";
        //label2.appendChild(checkbox2);
        //let back2 = _creaDiv("toggle-back");
        //label2.appendChild(back2);
        //let toggle2 = _creaDiv("toggle");
        //let lbl2_on = _creaDiv("toggle-label on", "", "", "Irrigazioni registrate");
        //let lbl2_off = _creaDiv("toggle-label off", "", "", "Irrigazioni da modello");
        //back2.appendChild(toggle2);
        //back2.appendChild(lbl2_on);
        //back2.appendChild(lbl2_off);

        //checkbox2.checked = plugin.indicatore.IsIrriUser;

        //$(checkbox2).on('change', function () {

        //    let value = this.checked;

        //    if (plugin.indicatore.IsIrriUser === value) {
        //        return;
        //    }

        //    plugin.indicatore.IsIrriUser = value;

        //    plugin.run();
        //});

        let irriOpts = [
            {
                text: "Irrigazioni registrate",
                value: Irrigazioni_GIAS
            },
            {
                text: "Bilancio ideale",
                value: Bilancio_IF
            }
        ];
        if (plugin.persistent) {
            irriOpts = [
                {
                    text: "Irrigazioni GIAS",
                    value: Irrigazioni_GIAS
                },
                {
                    text: "Irrigazioni IF",
                    value: Irrigazioni_IF
                },
                {
                    text: "Bilancio ideale",
                    value: Bilancio_IF
                }
            ];
        }

        /*
        let divFooter3 = _creaDiv("", "padding-top: 5px");
        elem.appendChild(divFooter3);
        $(divFooter3).MultiStateSwitch({
            options: irriOpts,
            onchange: function (state) {

                if (plugin.indicatore.IrriChoice === state) {
                    return;
                }

                plugin.indicatore.IrriChoice = state;

                plugin.run();
            }
        });

        $(divFooter3).data("MultiStateSwitch").setValue(plugin.indicatore.IrriChoice);
        */

        plugin.indicatore.IrriChoice = Irrigazioni_GIAS;

        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------

        plugin.run = function() {

            if (_can_run()) {

                _remove_table(plugin.$wrapper.parents(".indic-elem"));
                _show();
            }
        };


        plugin.filter = function(vegArray) {

            if (vegArray.length === 0) {

                $(elem).removeClass("indic-filtered-out");
            } else {

                let idx = 0;
                let found = false;
                while (!found && idx < vegArray.length) {
                    found = (plugin.indicatore.Veg_Cod == vegArray[idx]);
                    idx++;
                }
                if (found) {

                    $(elem).removeClass("indic-filtered-out");
                } else {

                    $(elem).addClass("indic-filtered-out");
                    _remove_table(elem);
                }
            }
        };

        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------

        var _can_run = function () {

            let ris = true;
            let msg = "";
            let fa = "";
            if (plugin.indicatore.DataFaseStart === null) {

                ris = false;
                msg = "Data inizio non impostata";
                fa = "fa-calendar";

            } else {

                if (plugin.indicatore.LatLng === null && !plugin.indicatore.UsaMeteo) {

                    ris = false;
                    msg = "Poligono impianto mancante";
                    fa = "fa-map-marker";
                }
            }

            if (!ris) {

                _clearElement(plugin.result);

                let divCont = _creaDiv("k-block k-error-colored", "height: 100%; text-align: center; display: grid; grid-template-rows: auto 1fr auto;");
                divCont.appendChild(_creaDiv("", "padding-top: 5px;", "", "<span class='fa fa-warning fa-2x'></span>"));
                divCont.appendChild(_creaDiv("", "align-self: center;", "", "<span>" + msg + "</span>"));
                let divIcon = _creaDiv();
                let stack = document.createElement("span");
                stack.className = "fa-stack fa-lg";
                let i0 = document.createElement("i");
                i0.className = "fa fa-square fa-stack-2x";
                i0.style.cssText = "margin: 0px;";
                let i1 = document.createElement("i");
                i1.className = "fa " + fa + " fa-stack-1x fa-inverse";
                i1.style.cssText = "margin: 0px;";
                stack.appendChild(i0);
                stack.appendChild(i1);
                divIcon.appendChild(stack);
                divCont.appendChild(divIcon);

                plugin.result.appendChild(divCont);
            }

            return ris;
        };

        var _impostaStart = function() {

            let divWin = _creaDiv("", "display: none;", "irri-win-data");

            plugin.wrapper.appendChild(divWin);

            let divWrapper = _creaDiv("", "display: flex; flex-flow: column; align-items: stretch; row-gap: 10px; max-width: 300px;", "irri-win-data-wrapper");
            divWin.appendChild(divWrapper);

            let divCalendar = _creaDiv("", "align-self: center;", "irri-calendar");
            divWrapper.appendChild(divCalendar);

            $("#irri-calendar").kendoCalendar({
                value: new Date(),
                footer: false
            });

            let divMsg = _creaDiv("k-block k-info-colored", "padding: 5px 10px; text-align: justify;");
            divMsg.innerHTML = "Registrare data di Semina / Trapianto / Rilievo fase fenologica opportuna nel quaderno di campagna per impostare la data definitivamente.";
            divWrapper.appendChild(divMsg);

            let divBtnSet = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base", "", "", "<span>Imposta temporaneamente</span>");
            divWrapper.appendChild(divBtnSet);
            divBtnSet.onclick = function() { _actImpostaStart(false); };

            //let divBtnRec = _creaDiv("k-button", "margin-top: 10px;", "", "<span>Imposta e registra</span>");
            //divWrapper.appendChild(divBtnRec);
            //divBtnRec.onclick = function () { _actImpostaStart(true); }

            let $kendomodal = $("#irri-win-data");

            $kendomodal.kendoWindow({
                title: "Imposta data start",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                    $("body").addClass("overflow_hidden");
                    $("#irri-win-data-wrapper").css("opacity", "0");
                },
                activate: function () {
                    $("#irri-win-data-wrapper").css("opacity", "1");
                },
                close: function(e) {
                    $("body").removeClass("overflow_hidden");
                    $kendomodal.data("kendoWindow").destroy();
                }
            });
            $kendomodal.data("kendoWindow").center().open();
        };

        var _actImpostaStart = function(flagRec) {

            let calendar = $("#irri-calendar").data("kendoCalendar");

            let dt = new Date(calendar.value());
            dt.setHours(0, 0, 0, 0);

            //todo: verificare validità data

            if (flagRec) {
                //todo: chiamare WS per registrazione semina/trapianto/inizio fase vegetativa
            }

            plugin.indicatore.DataFaseStart = dt;

            _sayStart();

            let win = $("#irri-win-data").data("kendoWindow");

            win.close();

            plugin.run();
        };

        var _geolocalizza = function () {

            if (!plugin.indicatore.hasOwnProperty("Indirizzo")) {
                return;
            }

            if (plugin.indicatore.Indirizzo === "") {
                return;
            }

            let geocoder = new google.maps.Geocoder;

            geocoder.geocode({
                address: plugin.indicatore.Indirizzo
            },
                function (results, status) {
                    if (status === google.maps.GeocoderStatus.OK) {

                        let l = results[0].geometry.location;

                        if (l.lat() !== "" && l.lng() !== "") {

                            plugin.indicatore.LatLng = {
                                Lat: l.lat(),
                                Lng: l.lng()
                            };

                            //_sayLatLng();

                            //if (!plugin.indicatore.UsaMeteo) {
                            //  plugin.run();
                            //}

                            plugin.run();
                        }
                    } else {
                        console.log("Geocode was not successful for the following reason: " + status);
                    }
                }
            );
        };

        var _show_table = function () {

            if (plugin.table === undefined) {
                return;
            }

            let grid_elem = plugin.$wrapper.parents(".indic-elem");
            if ($(grid_elem).hasClass("indic-elem-shown")) {
                return;
            }

            let unique_id = 1;
            $.each(document.getElementsByClassName("irri-tabs"), function (idx, elem) {
                unique_id = Math.max(parseInt($(elem).attr("data-id")) + 1, unique_id);
            });
            let divWinId = "win-show-" + unique_id;

            $(grid_elem).toggleClass("k-shadow");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            //$(grid_elem).toggleClass("k-inset");
            $(grid_elem).addClass("indic-elem-shown");
            $(grid_elem)[0].setAttribute("data-win-id", divWinId);

            let insert_after = grid_elem;
            let all_elem = grid_elem.parent().children();
            if (all_elem.length > 1) {
                let top = grid_elem.position().top;
                let idx = 0;
                let found = false;
                while (idx < all_elem.length && !found) {
                    if ($(all_elem[idx]).position().top > top) {
                        found = true;
                    } else {
                        insert_after = $(all_elem[idx]);
                    }
                    idx++;
                }
            }

            insert_after = insert_after[0];
            while ($(insert_after.nextSibling).hasClass("grid-span-all")) {
                insert_after = insert_after.nextSibling;
            }

            let divWin = _creaDiv("k-block k-shadow grid-span-all", "background-color: #fff; position: relative; z-index: 2;", divWinId);

            let modal = document.getElementById("modal")
            modal.appendChild(divWin);
            modal.style.display = "block";

            let title = plugin.indicatore.Campo;
            if (title != "") {
                title += " - ";
            }
            title += plugin.indicatore.Impianto;
            let varieta = plugin.indicatore.Varieta;
            if (varieta != "") {
                varieta = " - " + varieta;
            }
            title += " (" + plugin.indicatore.Coltura + varieta + ")";

            let divHdr = _creaDiv("k-header");
            let divHdrCont = _creaDiv("", "display: flex; align-items: center;");
            let divTitle = _creaDiv("", "width: 100%; padding: 4px 8px; font-size: large;", "", title);
            let divClose = _creaDiv("k-button k-button-md k-rounded-md k-button-flat k-button-flat-base k-icon-button k-window-action", "", "", "<span class='k-button-icon k-icon k-i-close' style='font-size: large;'></span>");

            let obfuscator = document.getElementById("obfuscator")
            function close() {
                _remove_table(grid_elem);
                obfuscator.removeEventListener("click", close);
                $('body').css("overflow", "auto");
            }
            $('body').css("overflow", "hidden");

            obfuscator.addEventListener("click", close);
            divClose.onclick = close;

            divHdrCont.appendChild(divTitle);
            divHdrCont.appendChild(divClose);            
            divHdr.appendChild(divHdrCont);
            divWin.appendChild(divHdr);

            let divBody = _creaDiv();
            divWin.appendChild(divBody);

            let unique_id_tabs = "irri-tabs-" + unique_id;
            let unique_id_chart = "irri-kendo-chart-" + unique_id;
            let unique_id_grid = "irri-kendo-grid-" + unique_id;

            //let divTabs = _creaDiv("irri-tabs", "background-color:#fff; box-shadow:none;", unique_id_tabs);
            //divTabs.setAttribute("data-id", unique_id);
            //divBody.appendChild(divTabs);

            //unique_id_tabs = "#" + unique_id_tabs;
            //$(unique_id_tabs).kendoTabStrip({
            //    animation: false,
            //    scrollable: false
            //});

            //let tabstrips = $(unique_id_tabs).data("kendoTabStrip");

            /*
            $.each([{ text: "Grafico", id: unique_id_chart }, { text: "Tabella", id: unique_id_grid }], function (index, obj) {
                tabstrips.append({
                    text: obj.text,
                    content: "<div style='overflow:hidden;'><div id='" + obj.id + "'></div></div>"
                });
            });

            tabstrips.tabGroup.css("font-size", "larger");

            tabstrips.select(0);

            */
            let grid = _creaDiv("", "display:none", unique_id_grid, "");
            divBody.appendChild(grid);
            _creaGrid(unique_id_grid, plugin.table);
            unique_id_grid = "#" + unique_id_grid;

            let grid_ds = $(unique_id_grid).data("kendoGrid").dataSource.data();
            let chart = _creaDiv("", "", unique_id_chart, "")
            divBody.appendChild(chart);
            _creaChart(unique_id_chart, grid_ds, plugin.table.kendo_columns);
            unique_id_chart = "#" + unique_id_chart;
            /*
            tabstrips.bind("activate", function (e) {
                if ($(e.contentElement).index() === 2) {

                    let chartH = 0;
                    $(unique_id_chart + " .chart-wrapper").each(function () {
                        chartH += $(this).outerHeight() + $(this).find(".k-chart").height();
                    });
                    let hdrH = $(unique_id_grid + " .k-grid-header").outerHeight();
                    let btw = parseFloat($(unique_id_grid).css("border-top-width"));
                    let bbw = parseFloat($(unique_id_grid).css("border-bottom-width"));

                    $(unique_id_grid + " .k-grid-content").css("height", (chartH - hdrH - btw - bbw) + "px");

                    tabstrips.unbind("activate");
                }
            });
            */

            divWin.scrollIntoView();
        };

        var _remove_table = function(elem) {

            if (!$(elem).hasClass("indic-elem-shown")) {
                return;
            }

            let divWinId = $(elem).attr("data-win-id");
            let divWin = $("#" + divWinId);
            if (divWin.length > 0) {
                divWin.remove();
            }

            $(elem).removeClass("indic-elem-shown");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            $(elem).toggleClass("k-shadow");
            document.getElementById("modal").style.display = "none";
            //$(elem).toggleClass("k-inset");
        };

        var _creaGrid = function (div_id, kendodata) {

            let objConfig = {
                dataSource: {
                    data: kendodata.kendo_rows,
                    schema: {
                        model: {
                            fields: kendodata.kendo_model
                        }
                    }
                },
                columns: kendodata.kendo_columns,
                resizable: true,
                pageable: false
            };

            $("#" + div_id).kendoGrid(objConfig);
        };

        var _creaChart = function (div_id, ds, columns) {

            let hasUmTerr = false;
            let c = 0;
            while (c < columns.length && !hasUmTerr) {
                if (columns[c].field.startsWith("UT_")) {
                    hasUmTerr = true;
                }
                c++;
            }

            let chartWrapper = document.getElementById(div_id);

            let style = "";
            if (hasUmTerr) {
                style = "padding-bottom: 10px;";
            }
            let chartWrapper1 = _creaDiv("chart-wrapper", style);

            let chart_id_1 = div_id + "-1";
            let chart1 = _creaDiv("", "", chart_id_1);
            chartWrapper1.appendChild(chart1);

            chartWrapper.appendChild(chartWrapper1);

            let max_perc = 100;
            let d = new Date()
            let initialData = new Date()
            initialData.setDate(d.getDate() - 10)
            let endData = new Date()
            endData.setDate(d.getDate() + 15)
            let chart_ds = ds.filter((x) => (x.Data >= initialData && x.Data <= endData) );


            if (plugin.rundate !== undefined) {

                chart_ds = ds.map(function (x) {

                    max_perc = Math.max(max_perc, x.Umid, x.SoSup);

                    if (x.Data >= plugin.rundate) {

                        x.Umid_Post = x.Umid;
                        x.SoInf_Post = x.SoInf;
                        x.SoSup_Post = x.SoSup;
                        x.SoInfPerc_Post = x.SoInfPerc;
                        x.SoSupPerc_Post = x.SoSupPerc;

                        if (x.Data > plugin.rundate) {

                            delete x.Umid;
                            delete x.SoInf;
                            delete x.SoSup;
                            delete x.SoInfPerc;
                            delete x.SoSupPerc;

                        } else {

                            x.DataCalcolo = 0;

                        }
                    }

                    return x;
                });

            }

            max_perc = (Math.floor(max_perc / 10) + 1) * 10;

            let vertAxes = [
                {
                    title: {
                        text: "Valori di bilancio in % di acqua disponibile"
                    }, 
                    min: -10,
                    max: max_perc,
                    axisCrossingValue: Number.NEGATIVE_INFINITY,
                    plotBands: [
                        {
                            from: -10,
                            to: 0,
                            color: "red",
                            opacity: 0.3
                        },
                        {
                            from: 100,
                            to: max_perc,
                            color: "green",
                            opacity: 0.3
                        }
                    ]
                },
                {
                    name: "Apporti_Axis",
                    title: {
                        text: "Apporti in mm"
                    }
                }
            ];

            let series = [
                {
                    name: "Umidità terreno",
                    field: "Umid",
                    color: "rgb(255, 140, 0)",
                    gruppo: 1
                },
                {
                    name: "Soglia di intervento",
                    field: "SoInf",
                    color: "rgb(178, 34, 34)",
                    gruppo: 2
                },
                {
                    name: "Soglia superiore",
                    field: "SoSup",
                    color: "rgb(0, 128, 0)",
                    gruppo: 3
                },
                {
                    name: "Piogge",
                    type: "column",
                    field: "Prec",
                    gap: 1,
                    spacing: 0,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 128, 255)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    name: "Irrigazioni",
                    type: "column",
                    field: "Irri",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    field: "Umid_Post",
                    dashType: "dash",
                    gruppo: 1
                },
                {
                    field: "SoInf_Post",
                    dashType: "dash",
                    gruppo: 2
                },
                {
                    field: "SoSup_Post",
                    dashType: "dash",
                    gruppo: 3
                },
                {
                    field: "IrriPrev",
                    type: "column",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    opacity: 0.5,
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                }
            ];

            //-------------------------------------------------------------------------------------
            // oggetto per controllare una riga verticale
            // posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
            //-------------------------------------------------------------------------------------
            let horzAxisCfg = {
                field: "Data",
                vertIndicator: {
                    field: "DataCalcolo",
                    label: {
                        text: "Data Calcolo",
                        color: "#808080" //"#800080"
                    },
                    line: {
                        color: "#808080",
                        width: 1,
                        dashType: "longDash"
                    }
                }
            };

            let titolo = "Bilancio idrico Irriframe";
            if (!plugin.indicatore.UsaMeteo) {
                if (typeof plugin.indicatore.InfoProvider === "string") {
                    if (plugin.indicatore.InfoProvider !== "") {

                        titolo += " - ";
                        titolo += plugin.indicatore.InfoProvider;
                    }
                }
            }

            creaKendoChart(titolo, horzAxisCfg, vertAxes, series, chart_ds, chart_id_1);

            if (hasUmTerr) {

                let chartWrapper2 = _creaDiv("chart-wrapper", "padding-top: 10px;");
                let chart_id_2 = div_id + "-2";
                let chart2 = _creaDiv("", "", chart_id_2);

                chartWrapper2.appendChild(chart2);

                chartWrapper.appendChild(chartWrapper2);

                let vertAxes2 = [
                    {
                        title: {
                            text: "Umidità terreno (%)"
                        }//,
                        //min: -10,
                        //max: max_perc,
                        //axisCrossingValue: Number.NEGATIVE_INFINITY,
                        //plotBands: [
                        //    {
                        //        from: -10,
                        //        to: 0,
                        //        color: "red",
                        //        opacity: 0.3
                        //    },
                        //    {
                        //        from: 100,
                        //        to: max_perc,
                        //        color: "green",
                        //        opacity: 0.3
                        //    }
                        //]
                    }//,
                    //{
                    //    name: "Apporti_Axis",
                    //    title: {
                    //        text: "Apporti in mm"
                    //    }
                    //}
                ];

                let series2 = [
                    {
                        name: "Soglia inferiore",
                        field: "SoInfPerc",
                        color: "rgb(178, 34, 34)",
                        gruppo: 1
                    },
                    {
                        name: "Soglia superiore",
                        field: "SoSupPerc",
                        color: "rgb(0, 128, 0)",
                        gruppo: 2
                    },
                    //{
                    //    name: "Piogge",
                    //    type: "column",
                    //    field: "Prec",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 128, 255)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    name: "Irrigazioni",
                    //    type: "column",
                    //    field: "Irri",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 206, 209)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    field: "Umid_Post",
                    //    dashType: "dash",
                    //    gruppo: 1
                    //},
                    {
                        field: "SoInfPerc_Post",
                        dashType: "dash",
                        gruppo: 1
                    },
                    {
                        field: "SoSupPerc_Post",
                        dashType: "dash",
                        gruppo: 2
                    }
                ];

                let gruppo = 3;

                $.each(columns, function (i, e) {
                    if (e.field.startsWith("UT_")) {
                        series2.splice(0, 0,
                            {
                                name: e.title,
                                field: e.field,
                                //color: "rgb(255, 140, 0)",
                                gruppo: gruppo++
                            }
                        );
                    }
                });

                ////-------------------------------------------------------------------------------------
                //// oggetto per controllare una riga verticale
                //// posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
                ////-------------------------------------------------------------------------------------
                //let horzAxisCfg = {
                //    field: "Data",
                //    vertIndicator: {
                //        field: "DataCalcolo",
                //        label: {
                //            text: "Data Calcolo",
                //            color: "#808080", //"#800080",
                //        },
                //        line: {
                //            color: "#808080",
                //            width: 1,
                //            dashType: "longDash"
                //        }
                //    }
                //};

                //let titolo = "Bilancio idrico Irriframe";
                //if (typeof plugin.indicatore.InfoProvider === "string") {
                //    if (plugin.indicatore.InfoProvider !== "") {

                //        titolo += " - ";
                //        titolo += plugin.indicatore.InfoProvider;
                //    }
                //}

                creaKendoChart("", "Data", vertAxes2, series2, chart_ds, chart_id_2);
            }
        };

        var _showLoader = function () {

            _clearElement(plugin.result);
            
            let divRel = _creaDiv("k-block", "position: relative; width: 100%; height: 100%;");
            plugin.result.appendChild(divRel);
            $(divRel).StyleLoader();
        };

        var _creaAdv = function(msg, bkg, clr, icon) {

            let advStyle = "border-radius: 5px; border: 1px solid #aeaeae; display: flex; flex-grow: 1; ";
            if (bkg !== "") {
                advStyle += " background-color: " + bkg + ";";
            }
            if (clr !== "") {
                advStyle += " color: " + clr + ";";
            }
            //let msgStyle = "font-size: smaller; width: 100%; align-self: flex-end; text-align: center;";
            //let msgStyle = "width: 100%; align-self: center; text-align: center;";
            /*
            <div class="fa fa-smile-o fa-lg" style="color: red;"></div>
            <div style="font-size: smaller;">02/05</div>                          
            */

            // TODO add icon
            let containerDiv = _creaDiv("", "display: flex; flex-direction: column");
            let divIcon = _creaDiv("fa " + icon, "align-self: center; margin-bottom: 5px");
            containerDiv.appendChild(divIcon);
            let divAdv = _creaDiv("", advStyle);
            containerDiv.appendChild(divAdv);
            let divMsg = _creaDiv("", "width: 100%; align-self: center; text-align: center;", "", msg);
            divAdv.appendChild(divMsg);

            return containerDiv;
        };

        var _show = function () {

            _showLoader();
            
            let url = window.location.href.split("?")[0] + "/CalcolaWBResult";
            ajaxAgronica(url,
                JSON.stringify({inPar: JSON.stringify(plugin.indicatore)}),
                function (risposta) {

                    let res = JSON.parse(risposta.RispostaStringa);

                    if (res.IrriframeData && res.IrriframeData.PlotId > 0 && res.IrriframeData.CropId > 0) {
                        plugin.indicatore.PersistData.Irri_PlotId = res.IrriframeData.PlotId;
                        plugin.indicatore.PersistData.Irri_CropId = res.IrriframeData.CropId;
                    }

                    _showResult(res);
                },
                function (risposta) {
                    console.log("ERRORE DSS Irrigazione (WBResult)");
                },
                null,
                false
            );
        };

        var _showResult = function (res) {

            _clearElement(plugin.result);

            if (res.Status !== 0) {

                let divMsg = _creaDiv("k-block", "height: 100%; text-align: center; display: flex; background-color: #f9f9f9; color: #aeaeae;");
                let span = document.createElement("span");
                span.style.cssText = "align-self: center;";
                span.innerHTML = res.Message;
                divMsg.appendChild(span);
                plugin.result.appendChild(divMsg);

                $(divMsg).on("click", function (e) {
                    if (e.ctrlKey) {
                        _impostaRunData();
                    }
                });
                return;
            }

            plugin.indicatore.InfoProvider = res.InfoProvider;
            plugin.indicatore.IrrigazionePrevista = null;

            let divContent = _creaDiv("", "height: 100%; grid-gap: 3px; grid-template-rows: 3.5em auto;");

            let msgIrri = "NO";
            let msgData = "";
            let msgVol = "";
            let msgCons = "";
            let msgs = [];

            let dataEsecuzione = Date.parseLocale(res.DataEsecuzione);

            if (!isNaN(parseFloat(res.ConsumoMM))) {
                msgCons = kendo.toString(res.ConsumoMM, "0.00") + " mm";
            }

            let arrAdv = [
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" }
            ];


            if (typeof res.NextFertiDate === "string") {
                msgIrri = "SI";
                msgData = res.NextFertiDate;
            }

            if (typeof res.FertiRecipeN === "number") {
                msgs.push({ name: 'Fabbisogno N', value: res.FertiRecipeN });
            }

            if (typeof res.FertiRecipeP === "number") {
                msgs.push({ name: 'Fabbisogno P', value: res.FertiRecipeP });
            }

            if (typeof res.FertiRecipeK === "number") {
                msgs.push({ name: 'Fabbisogno K', value: res.FertiRecipeK });
            }

            if (typeof res.FertiGivenP === "number") {
                msgs.push({ name: 'N Somministrato', value: res.FertiGivenP });
            }

            if (typeof res.FertiGivenP === "number") {
                msgs.push({ name: 'P Somministrato', value: res.FertiGivenP });
            }

            if (typeof res.FertiGivenK === "number") {
                msgs.push({ name: 'K Somministrato', value: res.FertiGivenK });
            }

            if (typeof res.FertiResidualN === "number") {
                msgs.push({ name: 'N Residuo', value: res.FertiResidualN });
            }
            if (typeof res.FertiResidualP === "number") {
                msgs.push({ name: 'P Residuo', value: res.FertiResidualP });
            }

            if (typeof res.FertiResidualK === "number") {
                msgs.push({ name: 'K Residuo', value: res.FertiResidualK });
            }



            /*
            if (typeof res.IrrigazioniPreviste === "object") {

                if (res.IrrigazioniPreviste.length > 0) {

                    msgIrri = "SI";

                    let idx = 0;
                    let cnt = Math.min(arrAdv.length - 1, res.IrrigazioniPreviste.length);

                    while (idx < cnt) {

                        let irri = res.IrrigazioniPreviste[idx];
                        let dt = new Date(irri.Data);
                        dt.setHours(0, 0, 0, 0);

                        if (plugin.indicatore.IrrigazionePrevista === null) {
                            plugin.indicatore.IrrigazionePrevista = {
                                Data: dt,
                                VolumeMM: irri.VolumeMM
                            }
                        }

                        if (idx === 0) {
                            msgData = kendo.toString(dt, "ddd dd-MM"); //VEN 27-03-2020
                            msgVol = kendo.toString(irri.VolumeMM, "0.00") + " mm";
                        }

                        arrAdv[idx + 1].msg = kendo.toString(dt, "dd/MM");

                        let diff_days = Math.floor((dt.getTime() - dataEsecuzione.getTime()) / (1000 * 60 * 60 * 24));
                        if (diff_days <= 3) {
                            arrAdv[idx + 1].bkg = "#f00000"; //rosso
                            arrAdv[idx + 1].clr = "#ffffff";
                        } else if (diff_days <= 7) {
                            arrAdv[idx + 1].bkg = "#fff000"; //giallo   "#ff8000" 'arancio
                        } else {
                            arrAdv[idx + 1].bkg = "#008000"; //verde
                            arrAdv[idx + 1].clr = "#ffffff";
                        }

                        idx++;
                    }
                }
            }
            */


            if (typeof res.Table === "string") {
                if (res.Table !== "") {

                    let obj_table = JSON.parse(res.Table);

                    if (typeof obj_table.kendo_rows === "object") {
                        if (obj_table.kendo_rows.length > 0) {

                            plugin.table = obj_table;

                            let idx = plugin.table.kendo_rows.length - 1;

                            while (idx >= 0) {

                                let elem = plugin.table.kendo_rows[idx];

                                if (elem.Irri > 0) {

                                    let elem_dt = kendo.parseDate(elem.Data);

                                    arrAdv[0].msg = kendo.toString(elem_dt, "dd/MM");
                                    arrAdv[0].bkg = "#00ced1"; //"#00bfff"; //
                                    arrAdv[0].clr = "#ffffff";

                                    if (elem_dt.getTime() > dataEsecuzione.getTime()) {
                                        //Irrigazione registrata nel futuro... (Devo farlo notare???)
                                    }

                                    idx = 0;
                                }
                                idx--;
                            }
                        }
                    }
                }
            }

            //let divRow1 = _creaDiv("", "height: 3.5em; display: grid; grid-template-columns: 1fr 1fr 1fr; grid-gap: 3px;");

            //divRow1.appendChild(_creaAdv(arrAdv[0].msg, arrAdv[0].bkg, arrAdv[0].clr, "fa-history"));
            // flex-grow: 1;divRow1.appendChild(_creaDiv("", "font-size: 6px; align-self: center;", "", "<span class='fa fa-circle' style='margin: 0px;'></span>"));
            //divRow1.appendChild(_creaAdv(arrAdv[1].msg, arrAdv[1].bkg, arrAdv[1].clr, "fa-arrow-down"));
            //divRow1.appendChild(_creaAdv(arrAdv[2].msg, arrAdv[2].bkg, arrAdv[2].clr, "fa-forward"));

            let divRow2 = _creaDiv("k-block k-info-colored ripple-cont", "font-size: smaller; display: grid; grid-template-rows: 1fr 1fr " + "1fr ".repeat(msgs.length) + "; align-items: center; ");
            let divIrri = _creaDiv();
            divIrri.innerHTML = "<span>FERTIRRIGAZIONE:</span><span style='float: right; font-weight: bold;'>" + msgIrri + "</span>";
            let divData = _creaDiv();
            divData.innerHTML = "<span>DATA:</span><span style='float: right; font-weight: bold;'>" + msgData + "</span>";

            divRow2.appendChild(divIrri);
            divRow2.appendChild(divData);

            for (const msg of msgs) {
                let div = _creaDiv();
                div.innerHTML = `<span>${msg.name}:</span><span style='float: right; font-weight: bold;'>${msg.value}</span>`;
                divRow2.appendChild(div);
            }

            //let divRow3 = _creaDiv("", "display: flex;");
            //let btnReg = _creaDiv("k-button indic-tooltip", "margin-right: 2px; flex: auto;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            //let btnReg = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button indic-tooltip", "flex: auto; background-color: #f5f5f5;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            //btnReg.setAttribute("data-tooltip", "Registra irrigazione");
            //divRow3.appendChild(btnReg);

            //let btnPia = _creaDiv("k-button indic-tooltip", "margin-left: 2px; flex: auto;", "", "<span class='fa fa-calendar fa-lg'></span>");
            //btnPia.setAttribute("data-tooltip", "Pianifica");
            //divRow3.appendChild(btnPia);

            //divContent.appendChild(divRow1);
            divContent.appendChild(divRow2);
            // divContent.appendChild(divRow3);

            plugin.result.appendChild(divContent);

            if (plugin.table !== undefined) {

                if (res.DataEsecuzione !== undefined) {

                    plugin.rundate = new Date(res.DataEsecuzione);
                    plugin.rundate.setHours(0, 0, 0, 0);
                }

                let ripple_cont = $(plugin.result).find(".ripple-cont");
                if (ripple_cont.length > 0) {

                    ripple_cont.addClass("irri-ripple");
                    ripple_cont[0].onclick = _show_table;
                    ripple_cont[0].insertBefore(_creaDiv("ripple"), ripple_cont[0].firstChild);
                }
            }

            /*
            if (plugin.indicatore.IrrigazionePrevista !== null) {

                btnReg.onclick = function () {
                    //_openIrriWin();
                    _querySaveAdvice();
                };
            } else {

                $(btnReg).addClass(GIAS_K_STATE_DISABLED);
            }
            */
        };

        var _querySaveAdvice = function () {

            if (plugin.indicatore.IrrigazionePrevista === null) {
                //se non ho irrigazioni previste non devo salvare
                return;
            }

            let queryGIAS = true;
            let queryIF = true;

            if (!plugin.persistent) {

                queryIF = false;

            } else {

                if (typeof plugin.indicatore.PersistData !== "object") {
                    //se non ho la chiave irriframe non so dove salvare l'irrigazione
                    return;
                }

                if (plugin.indicatore.PersistData.Irri_PlotId <= 0 || plugin.indicatore.PersistData.Irri_CropId <= 0) {
                    //se non ho la chiave irriframe non so dove salvare l'irrigazione in Irriframe

                    queryIF = false;

                }
            }

            let win_el = document.createElement("div");
            document.body.appendChild(win_el);
            let $win_el = $(win_el);

            let content = "<div style='grid-template-columns: auto 1fr; gap: 5px 10px;'>";
            content += "<div>Data</div>";
            content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.Data, "dddd d MMMM") + "</div>";
            content += "<div>Volume</div>";
            content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.VolumeMM, "0.00") + " mm</div>";
            content += "</div>";
            if (queryGIAS && queryIF) {

                content += "<div style='margin-top: 20px;'>";
                content += "    <div style='padding-bottom: 5px;'>";
                content += "        <input type='checkbox' id='__chk_gias' class='k-checkbox' style='margin: 0px; outline: none;'>";
                content += "        <label class='k-checkbox-label' for='__chk_gias'>Registra irrigazione in GIAS</label>";
                content += "    </div>";
                content += "    <div style='padding-top: 5px;'>";
                content += "        <input type='checkbox' id='__chk_if' class='k-checkbox' style='margin: 0px; outline: none;'>";
                content += "        <label class='k-checkbox-label' for='__chk_if'>Registra irrigazione in Irriframe</label>";
                content += "    </div>";
                content += "</div>";
            }

            $win_el.kendoDialog({
                title: "Registrazione irrigazione",
                closable: false,
                modal: true,
                visible: false,
                content: content,
                //open: function () {
                //},
                actions: [
                    {
                        text: "OK",
                        action: function (e) {

                            if (queryGIAS && queryIF) {

                                let chk_gias = $("#__chk_gias").prop("checked");
                                let chk_if = $("#__chk_if").prop("checked");

                                if (!chk_gias && !chk_if) {
                                    return false;
                                }

                                _saveAdvice(chk_gias, chk_if);

                            } else {

                                _saveAdvice(queryGIAS, queryIF);
                            }
                            return true;
                        }
                    },
                    {
                        text: "Annulla"
                    }
                ],
                close: function (e) {
                    this.destroy();
                }
            });

            $win_el.data("kendoDialog").open();
        };

        var _saveAdvice = function (save_gias, save_if) {

            let irri = {
                Data: plugin.indicatore.IrrigazionePrevista.Data,
                VolumeMM: plugin.indicatore.IrrigazionePrevista.VolumeMM
            };

            if (save_gias) {

                irri.GIAS_key = {
                    PIVA: plugin.indicatore.PersistData.GIAS_Piva,
                    SaCod: plugin.indicatore.PersistData.GIAS_SaCod,
                    Appezza: plugin.indicatore.PersistData.GIAS_Appezza,
                    IdReg: plugin.indicatore.PersistData.GIAS_IdReg,
                    VegCod: plugin.indicatore.Veg_Cod,
                    CulCod: plugin.indicatore.Cul_Cod,
                    SupImp: plugin.indicatore.Sup_Imp
                };
            }

            if (save_if) {

                irri.IF_key = {
                    IdPlot: plugin.indicatore.PersistData.Irri_PlotId,
                    IdCrop: plugin.indicatore.PersistData.Irri_CropId
                };
            }

            $.ajax({
                async: false,
                type: "POST",
                url: location.pathname + "/SalvaIrrigazione",
                data: JSON.stringify({ irri: irri }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    if (msg.d.RispostaOK) {

                        let objRisp = JSON.parse(msg.d.RispostaStringa);

                        if (objRisp.GIAS_Saved) {
                            plugin.indicatore.Irrigazioni.push(objRisp.Irrigazione);
                        }

                        if (objRisp.GIAS_Saved || objRisp.IF_Saved) {
                            //Ricalcolo il consiglio irriguo
                            _show();
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.status);
                    //alert(thrownError);
                }
            });
        };

        var _openIrriWin = function () {

            $.ajax({
                async: false,
                type: "POST",
                url: location.pathname + "/PreparaPaginaIrrigazione",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    if (msg.d == 'SessioneScaduta') {

                        $('#dialogSessioneScaduta').dialog("open");
                    }
                    else {

                        if (msg.d.RispostaOK) {

                            let winElem = _creaDiv("", "padding:0px;");
                            document.body.appendChild(winElem);

                            let frameH = window.innerHeight * 0.9;
                            let frameElem = document.createElement("iframe");
                            frameElem.id = "Irri_IFrame";
                            frameElem.style.cssText = "width:-webkit-fill-available; height:" + frameH + "px; opacity:0;";
                            frameElem.setAttribute('frameborder', '0');
                            frameElem.setAttribute("src", "");
                            winElem.appendChild(frameElem);

                            let $win_el = $(winElem);

                            $win_el.kendoWindow({
                                title: "Registra irrigazione",
                                width: "90%",
                                draggable: false,
                                visible: false,
                                modal: true,
                                resizable: false,
                                actions: [
                                    "Close"
                                ],
                                open: function (e) {
                                    e.sender.element.css("opacity", "0");
                                    //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                                    $("body").addClass("overflow_hidden");
                                },
                                activate: function (e) {
                                    e.sender.element.css("opacity", "1");
                                    $("#Irri_IFrame").css("opacity", "1");
                                },
                                close: function (e) {
                                    $("body").removeClass("overflow_hidden");
                                    this.destroy();
                                }
                            });

                            //let parent = $win_el.parent();
                            //parent.find('.k-window-title').css('text-align', 'center');
                            //parent.css('padding-top', '48px');
                            //let titlebar = parent.find('.k-window-titlebar');
                            //titlebar.css({
                            //    "margin-top": "-48px",
                            //    "height": "35px",
                            //    "line-height": "35px",
                            //    "vertical-align": "middle"
                            //});

                            frameElem.setAttribute("src", msg.d.RispostaStringa);

                            $win_el.data("kendoWindow").center().open();
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        };

        var _impostaRunData = function () {

            let divWin = _creaDiv("", "display: none;");
            plugin.wrapper.appendChild(divWin);
            let divCalendar = _creaDiv();
            divWin.appendChild(divCalendar);
            let divBtns = _creaDiv("", "display: flex; gap: 5px;");
            divWin.appendChild(divBtns);
            let divBtnOk = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#5cb85c;", "", "<span class='fa fa-check fa-lg'></span>");
            divBtns.appendChild(divBtnOk);
            let divBtnCancel = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#d9534f;", "", "<span class='fa fa-times fa-lg'></span>");
            divBtns.appendChild(divBtnCancel);

            $(divCalendar).kendoCalendar({
                value: new Date(),
                footer: false
            });

            $(divWin).kendoWindow({
                title: "Imposta data esecuzione",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                actions: []
            });

            let kendoCalendar = $(divCalendar).getKendoCalendar();

            let kendoWindow = $(divWin).getKendoWindow();

            kendoWindow.bind("open", function (e) {
                //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                $("body").addClass("overflow_hidden");
            });
            kendoWindow.bind("close", function (e) {
                $("body").removeClass("overflow_hidden");
                kendoWindow.destroy();
            });

            kendoWindow.center().open();

            divBtnOk.onclick = function () {
                let dt = new Date(kendoCalendar.value());

                kendoWindow.close();

                dt.setUTCHours(0, 0, 0, 0);
                plugin.indicatore.RunData = dt.toISOString().split("T")[0];
                plugin.run();
            };

            divBtnCancel.onclick = function () {
                kendoWindow.close();
            };
        };

        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------

        //$(...).data("WBResult").run();

        //-----------------------------------------------------------------------------------------

    }; // WBResult

    //Add the plugin to the jQuery.fn object
    $.fn.WBResult = function (indicatore, persistent) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('WBResult')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.WBResult(this, indicatore, persistent);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('WBResult', plugin);
            }
        });
    };

})(jQuery);





(function ($) {

    $.MultiStateSwitch = function (elem, options) {

        var plugin = this;

        let divWrapper = document.createElement("div");
        divWrapper.className = "multistateswitch";
        divWrapper.style.cssText = "grid-template-columns: repeat(" + options.options.length + ", minmax(0, 1fr));";
        elem.appendChild(divWrapper);

        let id_postfix = document.getElementsByClassName("multistateswitch").length;
        plugin.radio_name = "switch_option_" + id_postfix;

        $.each(options.options, function (i, o) {

            let label = document.createElement("label");

            let radio = document.createElement("input");
            radio.type = "radio";
            radio.name = plugin.radio_name;
            radio.value = o.value;
            label.appendChild(radio);

            let insideLabel = document.createElement("div");
            insideLabel.textContent = o.text;
            label.appendChild(insideLabel);

            divWrapper.appendChild(label);
        });

        $("input:radio[name=" + plugin.radio_name + "]").change(function () {

            if (options.onchange) {
                options.onchange(parseInt(this.value));
            }
        });

        plugin.$elem = $(divWrapper);

        plugin.setValue = function (v) {

            plugin.$elem.find("input:radio[name=" + plugin.radio_name + "]").val(["" + v + ""]);
        }

    }; // MultiStateSwitch

    $.fn.MultiStateSwitch = function (options) {

        let stylesheet = document.getElementById("multistateswitch-stylesheet");
        if (!stylesheet) {

            let css = ".multistateswitch { ";
            css += "display: grid; ";
            css += "overflow: hidden; ";
            css += "background: #f5f5f5; ";
            css += "color: #a9a9a9; ";
            css += "border: 1px solid #ddd; ";
            css += "border-radius: 4px; ";
            css += "} ";

            css += ".multistateswitch input[type=radio] { display: none; } ";

            css += ".multistateswitch label { ";
            css += "width: 100%; ";
            css += "cursor: pointer; ";
            css += "margin: 0px; ";
            css += "height: 100%; ";
            css += "text-align: center; ";
            css += "vertical-align: middle; ";
            css += "} ";

            css += ".multistateswitch label > input[type=radio]:checked + div { ";
            css += "color: #333; ";
            css += "background-color: #fdfdfd; ";
            css += "} ";

            css += ".multistateswitch label:hover { background-color: #f9f9f9; } ";

            css += ".multistateswitch:hover { ";
            //css += "background: #ebebeb; ";
            css += "border-color: #aeaeae; ";
            css += "} ";

            css += ".multistateswitch label > div { ";
            css += "padding: 5px 5px; ";
            css += "overflow: hidden; ";
            css += "white-space: nowrap; ";
            css += "text-overflow: ellipsis;";
            css += "transition: all 100ms linear; ";
            css += "} ";

            let style = document.createElement('style');
            style.id = "multistateswitch-stylesheet";
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }

        return this.each(function () {

            if (undefined == $(this).data('MultiStateSwitch')) {

                var plugin = new $.MultiStateSwitch(this, options);

                $(this).data('MultiStateSwitch', plugin);
            }
        });
    };

})(jQuery);

