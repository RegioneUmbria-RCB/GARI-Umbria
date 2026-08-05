
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

            Utility_Worker = new Worker("DSS_Utility_Worker.js");

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


function MostraIndicatori_V2(div, Centri) {

    if (Centri === undefined || Centri === null || Centri.length === 0) {
        //non ci sono consigli irrigui disponibili...
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

    let divIndics;
    let RitardaCalcolo = Centri.length > 2;
    let classCentroContainer = "";
    let classCentroCollapsible = "";
    if (RitardaCalcolo) {
        classCentroContainer += " collapsed";
    } else {
        classCentroCollapsible += " in";
    }


    let stazioniXcentro = [];

    for (c = 0; c < Centri.length; c++) {

        let centro = Centri[c];

        let idIndicCentro = "idIndicCentro" + (centro.IdCentro).toString();

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
        spanText.innerHTML = centro.DesCentro;

        divCentroCollapsible.appendChild(spanIcon);
        divCentroCollapsible.appendChild(spanHome);
        divCentroCollapsible.appendChild(spanText);

        let divStazioneWrapper = createDiv("", "flex-grow: 1; font-size: 13px; margin-left: 10px; color: #9e9e9e;");
        let divStazione = createDiv("", "float: right; padding-right: 5px;");
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

        //*** li creo collassati
        //attivo le richiesta solo quando espando il centro...
        if (RitardaCalcolo) {
            let ev = "show.bs.collapse";//"shown.bs.collapse"
            $("#" + idIndicCentro).on(ev, function () {
                $("#" + idIndicCentro + " > .indic-grid > .indic-elem ").each(function (idx, elem) {
                    let plugin = $(elem).data("WBResult_V2");
                    if (plugin !== undefined) {
                        plugin.run();
                    }
                });
                $("#" + idIndicCentro).off(ev);
            });
        }

        let indic = null;

        if (centro.Campi.length > 0) {

            indic = centro.Campi[0].Indicatori[0];

        } else {

            if (centro.Indicatori.length > 0) {

                indic = centro.Indicatori[0];
            }
        }

        if (indic != null) {

            if (indic.StazioneMeteo !== undefined && indic.StazioneMeteo !== null) {

                stazioniXcentro.push({
                    elem_id: divStazione.id,
                    sorgente: indic.StazioneMeteo.Sorgente,
                    stazione: indic.StazioneMeteo.Stazione
                });

            }
        }

        if (centro.Campi.length > 0) {

            centro.Campi.forEach(function (campo) {
                creaWidget(campo, null, divIndics, !RitardaCalcolo, allowPersistence);
            });

        } else {

            if (centro.Indicatori.length > 0) {

                centro.Indicatori.forEach(function (indic) {

                    creaWidget(null, indic, divIndics, !RitardaCalcolo, allowPersistence);
                });
            }
        }
    }
   
    $("#" + div).kendoTooltip({
        filter: ".indic-tooltip",
        showOn: "mouseenter",
        position: "top",
        content: function (e) {
            let target = e.target;
            return "<div style='white-space: nowrap;'>" + target.attr("data-tooltip") + "</div>";
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

            Utility_Worker = new Worker("DSS_Utility_Worker.js");

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


function creaWidget(campo, indic, parentDiv, runFlag, allowPersistence) {

    let arrIndic = [];

    if (campo !== null) {

        arrIndic = campo.Indicatori;

    } else {

        if (indic !== null) {

            arrIndic.push(indic);
        }
    }

    if (arrIndic.length === 0) {

        return;
    }

    let header = arrIndic[0].Campo;

    if (campo != null) {

        if (header.length == 0)
            header = arrIndic[0].Impianto

    } else {

        if (header.length != 0)
            header += " - ";

        header += arrIndic[0].Impianto;
    }

    let coltura = [arrIndic[0].Coltura, arrIndic[0].Varieta].filter((s) => s != null && s.length > 0).join(" - ");

    let indWrapper = createDiv("k-block k-shadow indic-elem", "", _getKeyGIAS(arrIndic[0]));
    parentDiv.appendChild(indWrapper);

    $(indWrapper).WBResult_V2(header, coltura, arrIndic, allowPersistence);

    if (runFlag)
        $(indWrapper).data("WBResult_V2").run();
}


function createDiv(_class, _style, _id) {
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
    return div;
}


function FiltraIndicatori(vegArray) {

    $(".indic-grid > .indic-elem").each(function (idx, elem) {
        let plugin = $(elem).data("WBResult_V2");
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

