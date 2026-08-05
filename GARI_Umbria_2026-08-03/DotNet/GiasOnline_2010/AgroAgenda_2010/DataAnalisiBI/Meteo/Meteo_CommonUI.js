


function meteoAlert(titolo, messaggio) {

    var alert = $('<div></div>').kendoAlert({
        //width: "90%",
        title: titolo,
        content: messaggio,
    }).data("kendoAlert");

    $(alert.wrapper).css('opacity', '0');

    alert.open();

    $(alert.wrapper).css('margin-left', '25px');
    $(alert.wrapper).css('margin-right', '25px');

    $(alert.wrapper).css('opacity', '1');

}


function setMeteoStorage(dati) {

    sessionStorage.setItem("@@@meteoSource@@@", JSON.stringify(dati));
}

function getMeteoStorage() {

    let dati = sessionStorage.getItem("@@@meteoSource@@@");

    if (dati != null) {

        dati = JSON.parse(dati);

        sessionStorage.removeItem("@@@meteoSource@@@");
    }

    return dati;
}


//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************


var GlobalMeteoTabstrip;

MeteoTabstrip.prototype = {

    ready: function () {

        let that = this;

        that.showTabDati(false);

        $(that.idTabstrip).removeClass("meteo-transparent");

        jQuery(window).on("resize", function (event) {
            that.resize();
        });

        that.resize();
    },

    kendoTabstrip: function () {
        return $(this.idTabstrip).getKendoTabStrip();
    },

    resize: function () {

        let win_h = $(window).height();
        let ftr_h = $(".AgronicaFooter").outerHeight();
        if (!$(".AgronicaFooter").is(":visible")) {
            ftr_h = 0;
        }

        let $tabstrip = $(this.idTabstrip);

        let tab_top = $tabstrip.position().top;
        let tab_h = $tabstrip.height() - $($(this.idTabstrip + " .k-content")[0]).height()

        let cont_h = (Math.floor((win_h - tab_top - tab_h - ftr_h) / 10) * 10) - 5;
        if (cont_h < 500) {
            cont_h = 500;
            document.body.style.overflowY = "auto";
        } else {
            document.body.style.overflowY = "hidden";
        }

        $(this.idTabstrip + " .k-content").height(cont_h);

        let tabstrip = $tabstrip.getKendoTabStrip();

        tabstrip.element.find(".meteoKendoChart").each(function (idx, elem) {
            let chart = $(elem).data("kendoChart");
            if (chart !== undefined) {
                chart.resize();
            }
        });
    },

    showTabDati: function (fShow) {

        let tabstrip = $(this.idTabstrip).getKendoTabStrip();
        let last = tabstrip.tabGroup.children().length - 1;
        let tabDati = tabstrip.tabGroup.children().eq(last);
        tabstrip.enable(tabDati, fShow);

        if (!fShow) {

            $(tabstrip.items()[last]).addClass("meteo-hidden");
        } else {

            $(tabstrip.items()[last]).removeClass("meteo-hidden");

            tabstrip.select(tabDati);

            let contentDati = tabstrip.contentElement(last);
            //$(contentDati).scrollTop(0);
            $(contentDati).animate({ scrollTop: 0 }, "fast");
        }
    }
};

function MeteoTabstrip(idTabstrip) {
    if (!(this instanceof MeteoTabstrip)) return new MeteoTabstrip();

    this.idTabstrip = idTabstrip;

    let $tabstrip = $(idTabstrip);

    $tabstrip.addClass("meteo-tabstrip");

    $tabstrip.kendoTabStrip({
        animation: false,
        scrollable: false
    });
}



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



var GlobalMeteoSourceSelector;

MeteoSourceSelector.prototype = {

    init: function () {
        let that = this;

        that.$geoPosEdit.geoPosEdit({
            change: function (pos) {
                let ddl = that.$sorgenteMeteo.getKendoDropDownList();
                let value = ddl.value();
                ddl.dataSource.read();
                ddl.value(value);

                if (isNaN(parseInt(ddl.value()))) {

                    ddl.select(0);
                    ddl.trigger("change");
                }
            }
        });

        that.default_Origine = 0;

        that.$tipoSorgenteMeteo.kendoDropDownList({
            autoBind: true,
            autoWidth: true,
            dataValueField: "sorgente_cod",
            dataTextField: "sorgente_des",
            dataSource: {
                transport: {
                    read: function (options) {
                        that.readSorgenteMeteoData(that, options);
                    }
                }
            },
            change: function () {

                let ddl = that.$sorgenteMeteo.getKendoDropDownList();

                ddl.dataSource.read().then(function (e) {

                    let value = 0;

                    if (that.default_Origine !== 0) {

                        value = that.default_Origine;
                        that.default_Origine = 0;

                    } else {

                        if (ddl.dataSource.data().length > 0) {

                            value = ddl.dataSource.data()[0].id_stazione;
                        }
                    }

                    ddl.value(value);
                });
            }
        });

        let tmpl_r1 = '<div style="display:flex; justify-content:space-between; align-items:center; column-gap:15px;">';
        tmpl_r1 += '<div style="font-weight:bold;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">';
        tmpl_r1 += '<span>#= (data.nome_stazione) ? data.nome_stazione : "&nbsp;"#</span>';
        tmpl_r1 += '</div>';
        tmpl_r1 += '<div style="font-size:11px; color:\\#9e9e9e;">';
        tmpl_r1 += '<span>#= (data.rif_fornitore) ? data.rif_fornitore : "&nbsp;"#</span>';
        tmpl_r1 += '</div>';
        tmpl_r1 += '</div>';

        let tmpl_r2 = '<div style="display:flex; justify-content:space-between; align-items:center; column-gap:15px; font-size:11px; margin-top:10px;">'
        tmpl_r2 += '<div><span>#= (data.ultimo_aggiornamento) ? data.ultimo_aggiornamento : "&nbsp;"#</span></div>';
        tmpl_r2 += '<div><span>#= (data.distanza) ? data.distanza : "&nbsp;"#</span></div>';
        tmpl_r2 += '</div>';

        let valueTemplate = '<div style="display:inline-block; width:-webkit-fill-available; user-select:none; max-width:750px;">';
        valueTemplate += tmpl_r1;
        valueTemplate += tmpl_r2;
        valueTemplate += '</div>';

        let listTemplate = '<div style="display:inline-block; width:100%;">';
        listTemplate += tmpl_r1;
        listTemplate += tmpl_r2;
        listTemplate += '</div>';

        that.$sorgenteMeteo.kendoDropDownList({
            autoBind: false,
            autoWidth: true,
            dataTextField: "nome_stazione",
            dataValueField: "id_stazione",
            filter: "contains",
            valueTemplate: valueTemplate,
            template: listTemplate,
            dataSource: {
                transport: {
                    read: function (options) {
                        that.readOrigineMeteoData(that, options);
                    }
                }
            },
            dataBound: function (e) {

                //per far funzionare il listTemplate
                $.each(this.items(), function (i, el) {
                    $(el).find(".k-list-item-text").css("width", "100%");
                });

                this.enable(true);

                let tipoSorgente = parseInt(that.$tipoSorgenteMeteo.getKendoDropDownList().value());

                if (tipoSorgente === 4 && this.dataSource.data().length === 0) {

                    this.enable(false);

                    let posAlert = document.createElement("div");
                    posAlert.style.position = "absolute";
                    posAlert.style.left = "0";
                    posAlert.style.width = "100%";
                    posAlert.style.top = "0";
                    posAlert.style.height = "100%";
                    posAlert.style.display = "flex";
                    posAlert.style.alignItems = "center";
                    posAlert.style.justifyContent = "center";
                    posAlert.style.color = "#9e9e9e";
                    posAlert.style.fontSize = "14px";
                    this.span.append(posAlert);

                    let spanText1 = document.createElement("span");
                    spanText1.innerText = "Clicca";
                    posAlert.appendChild(spanText1);

                    let spanIcon = document.createElement("span");
                    spanIcon.style.fontSize = "20px";
                    spanIcon.className = "fa fa-globe fa-fw";
                    posAlert.appendChild(spanIcon);

                    let spanText2 = document.createElement("span");
                    spanText2.innerText = "per geolocalizzare";
                    posAlert.appendChild(spanText2);
                }
            }
        });
    },

    sorgenteMeteo: function () {
        return {
            tipo: this.$tipoSorgenteMeteo.getKendoDropDownList().value(),
            stazione: this.$sorgenteMeteo.getKendoDropDownList().value()
        };
    },

    sorgentiMeteoList: function () {

        let stazioni = [];

        $.each(this.$sorgenteMeteo.getKendoDropDownList().dataSource.data(), function (i, el) {

            let staz = {
                id: el.id_stazione,
                descr: el.nome_stazione
            };

            if (el.hasOwnProperty("geo") && el.geo !== null) {
                staz.geo = {
                    lat: el.geo.lat,
                    lng: el.geo.lng 
                };
            }
            stazioni.push(staz);
        });

        return {
            tipo: this.$tipoSorgenteMeteo.getKendoDropDownList().value(),
            stazioni: stazioni
        };
    },

    readSorgenteMeteoData: function (that, options) {
        let sorgenti = [];

        ajaxAgronicaSync(that.url_ws + "/SorgentiMeteo",
            JSON.stringify({}),
            false,
            function (risposta) {

                sorgenti = JSON.parse(risposta.RispostaStringa);
            },
            function (risposta) {
                sorgenti = [
                    {
                        sorgente_cod: -1,
                        sorgente_des: "Non disponibile"
                    }
                ];
            }
        );

        options.success(sorgenti);
    },

    readOrigineMeteoData: function (that, options) {
        let stazioni = [];

        let tipoSorgente = parseInt(that.$tipoSorgenteMeteo.getKendoDropDownList().value());

        if (!isNaN(tipoSorgente) && tipoSorgente >= 0) {

            let param = {
                TipoSorgente: tipoSorgente,
                Lat: 0,
                Lng: 0
            };

            let pos = that.$geoPosEdit.data("geoPosEdit").getLatLngDec();
            if (pos !== null) {
                param.Lat = pos.lat;
                param.Lng = pos.lng;
            }

            ajaxAgronicaSync(that.url_ws + "/LeggiStazioniXSorgente",
                JSON.stringify(param),
                false,
                function (risposta) {

                    let arr_staz = JSON.parse(risposta.RispostaStringa);

                    let distanza = TraduzioneMultiResx(datiMeteoResx, "Distanza", "Distanza");
                    let distanza_na = distanza + " " + TraduzioneMultiResx(datiMeteoResx, "NonDisponibile", "non disponibile").toLowerCase();
                    let aggiornamento = TraduzioneMultiResx(datiMeteoResx, "Aggiornamento", "Aggiornamento");

                    $.each(arr_staz, function (s, staz) {

                        let rif_fornitore = staz.fornitore;

                        if (staz.rif_fornitore != "") {

                            rif_fornitore += " [" + staz.rif_fornitore + "]";
                        }

                        staz.rif_fornitore = rif_fornitore;

                        if (!isNaN(staz.distanza)) {

                            if (staz.distanza >= 1000) {

                                staz.distanza = distanza + " " + kendo.toString(staz.distanza * 0.001, "0") + " Km";

                            } else {

                                staz.distanza = distanza + " " + kendo.toString(staz.distanza, "0") + " mt";
                            }

                        } else {

                            staz.distanza = distanza_na;
                        }

                        if (staz.ultimo_aggiornamento) {

                            staz.ultimo_aggiornamento = aggiornamento + " " + kendo.toString(new Date(staz.ultimo_aggiornamento), "d");

                        } else {

                            staz.ultimo_aggionamento = "";
                        }

                        stazioni.push(staz);
                    });
                },
                function (risposta) {
                }
            );
        }

        options.success(stazioni);
    },

    setLatLng: function (lat, lng) {
        if (lat === null || lng === null) {
            return;
        }

        lat = parseFloat(lat.replace(",", "."));
        lng = parseFloat(lng.replace(",", "."));

        if (isNaN(lat) || isNaN(lng)) {
            return;
        }

        if (Math.abs(lat) < 0.00001 && Math.abs(lng) < 0.00001) {
            return;
        }

        this.$geoPosEdit.data("geoPosEdit").setLatLngDec(lat, lng);
    },

    setSorgente: function (tipo, sorgente, fCallback) {

        if (tipo == undefined) {
            return;
        }

        let ddlTipo = this.$tipoSorgenteMeteo.getKendoDropDownList();
        ddlTipo.value(tipo);
        ddlTipo.trigger("change");

        tipo = ddlTipo.value();

        if (sorgente == undefined) {
            return;
        }

        if (tipo == 4) {

            let that = this;

            ajaxAgronica(this.url_ws + "/LocalizzaStazione",
                JSON.stringify({ stazione_cod: parseInt(sorgente) }),
                function (risposta) {

                    let risp = risposta.RispostaStringa;
                    if (risp !== "") {

                        let jsonObj = JSON.parse(risp);

                        if (jsonObj.coordinates) {

                            if (jsonObj.coordinates.lat && jsonObj.coordinates.lng) {

                                let lat = parseFloat(jsonObj.coordinates.lat);
                                let lng = parseFloat(jsonObj.coordinates.lng);

                                if (!isNaN(lat) && !isNaN(lng)) {

                                    that.$geoPosEdit.data("geoPosEdit").setLatLngDec(lat, lng);
                                    let ddlSorgente = that.$sorgenteMeteo.getKendoDropDownList();
                                    ddlSorgente.dataSource.read().then(function (e) {

                                        ddlSorgente.select(0);

                                        if (typeof fCallback === "function") {
                                            fCallback(tipo, ddlSorgente.value());
                                        }
                                    });
                                }
                            }
                        }
                    }
                },
                function (risposta) {
                }
            );
        }
        else {

            let ddlSorgente = this.$sorgenteMeteo.getKendoDropDownList();
            ddlSorgente.value(sorgente);
            if (ddlSorgente.value() != sorgente) {

                ddlSorgente.select(0);
            }

            if (typeof fCallback === "function") {
                fCallback(tipo, ddlSorgente.value());
            }
        }
    },

    setDefault: function (piva) {

        let that = this;

        ajaxAgronicaSync(this.url_ws + "/StazioniDaAnagrafica",
            JSON.stringify({ piva: piva }),
            true,
            function (risposta) {

                let risp = risposta.RispostaStringa;
                if (risp !== "") {

                    let ddlTipoSorgente = that.$tipoSorgenteMeteo.getKendoDropDownList();

                    let jsonArr = JSON.parse(risp);
                    if (jsonArr.length > 0) {

                        let obj = jsonArr[0];

                        that.default_Origine = parseInt(obj.Stazione_Cod);

                        ddlTipoSorgente.value(obj.Tipo_Sorgente);

                        if (ddlTipoSorgente.value() != obj.Tipo_Sorgente) {

                            ddlTipoSorgente.select(0);
                        }

                    } else {

                        ddlTipoSorgente.value(4);
                        if (ddlTipoSorgente.value() != 4) {

                            ddlTipoSorgente.select(0);
                        }
                    }

                    ddlTipoSorgente.trigger("change");
                }
            },
            null,
            null,
            false
        );
    }

};

function MeteoSourceSelector($geoPosEdit, $tipoSorgenteMeteo, $sorgenteMeteo, url_ws) {
    if (!(this instanceof MeteoSourceSelector)) return new MeteoSourceSelector();

    this.$geoPosEdit = $geoPosEdit;
    this.$tipoSorgenteMeteo = $tipoSorgenteMeteo;
    this.$sorgenteMeteo = $sorgenteMeteo;
    this.url_ws = url_ws;

    this.init();
}



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



(function ($) {

    $.meteoOutput = function (elem, opts) {

        var plugin = this;

        //creo un contenitore per i dati...
        var _datiContainer = document.createElement("div");
        var _$datiContainer = $(_datiContainer);
        elem.appendChild(_datiContainer);

        //creo un contenitore per gli output...
        var _outputContainer = document.createElement("div");
        var _$outputContainer = $(_outputContainer);
        elem.appendChild(_outputContainer);

        plugin.clearCharts = function () {

            let charts = _outputContainer.children;
            for (let c = 0; c < charts.length; c++) {
                let kendoChart = $("#" + charts[c].id).data("kendoChart");
                if (kendoChart != undefined) {
                    kendoChart.destroy();
                }
            }

            _$outputContainer.find('.meteoKendoChart').remove();
        };

        plugin.clearGrids = function () {

            let grids = _outputContainer.children;
            for (let g = 0; g < grids.length; g++) {
                let kendoGrid = $("#" + grids[g].id).data("kendoGrid");
                if (kendoGrid != undefined) {
                    kendoGrid.destroy();
                }
            }

            _$outputContainer.find('.meteoKendoGrid').remove();
        };

        plugin.clear = function () {

            _$datiContainer.empty();

            plugin.clearCharts();

            plugin.clearGrids();

            _$outputContainer.empty();
        };

        plugin.creaDiv = function (dom_class) {

            let div_id = "divKendoOut_Child" + (_outputContainer.children.length + 1);
            var divChild = document.createElement('div');
            divChild.id = div_id;
            _outputContainer.appendChild(divChild);

            if (typeof dom_class === "string" && dom_class != "") {
                $("#" + div_id).addClass(dom_class);
            }

            let num_children = _outputContainer.children.length - _$outputContainer.find(".meteoTitolo").length;
            if (num_children > 1) {
                $("#" + div_id).css("margin-top", "25px");
            }

            return div_id;
        };

        plugin.creaDati = function (div_id) {

            let inputKendoDati = document.createElement('input');
            inputKendoDati.id = "hdKendoDati_" + div_id;
            inputKendoDati.type = "hidden";
            _datiContainer.appendChild(inputKendoDati);

            $("#" + div_id).attr("kendo-data", inputKendoDati.id);
        };

        plugin.getGridDiv = function () {

            let grids = _outputContainer.children;
            for (var g = 0; g < grids.length; g++) {
                var kendoGrid = $("#" + grids[g].id).data("kendoGrid");
                if (kendoGrid != undefined) {
                    return grids[g].id;
                }
            }
            return "";
        };

    }; // meteoOutput

    $.fn.meteoOutput = function (opts) {
        return this.each(function () {
            if (undefined == $(this).data('meteoOutput')) {

                var plugin = new $.meteoOutput(this, opts);

                $(this).data('meteoOutput', plugin);
            }
        });
    };
})(jQuery);



function creaChartDiv() {
    let meteoOutput = $("#divKendoOut").data("meteoOutput");
    return meteoOutput.creaDiv("meteoKendoChart");
}

function creaGridDiv() {
    let meteoOutput = $("#divKendoOut").data("meteoOutput");

    let div_id = meteoOutput.creaDiv("meteoKendoGrid");

    meteoOutput.creaDati(div_id);

    return div_id;
}

function kendoGetGridDiv() {
    let meteoOutput = $("#divKendoOut").data("meteoOutput");
    return meteoOutput.getGridDiv();
}

function kendoRiempiGridData(divKendoGrid, kendoRows, bRead) {

    let hdKendoDati = $("#" + divKendoGrid).attr("kendo-data");
    $("#" + hdKendoDati).val(JSON.stringify(kendoRows));

    if (bRead === true) {
        $("#" + divKendoGrid).data("kendoGrid").dataSource.read();
    }
}

