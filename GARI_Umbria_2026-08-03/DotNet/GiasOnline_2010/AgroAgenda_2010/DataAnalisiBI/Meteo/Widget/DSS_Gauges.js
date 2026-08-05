
class DSS_Gauges {
    constructor() {
    }

    static showGauges(rag_soc, indicatori, gaugesContainer, containerHeight, viewGrid, gaugeClickCallback,urlMeteoWs) {

        if (indicatori === undefined || indicatori === null || indicatori.length === 0)
            return;

        if (viewGrid) {
            this.#makeSwitch(gaugesContainer);
        }

        let hToggle = $(gaugesContainer).find(".__toggle").height()

        let divContainer = WidgetCommon.createElement("div");
        divContainer.className = "__toggle-container";
        gaugesContainer.appendChild(divContainer);

        $(divContainer).css("height", (containerHeight - hToggle) + "px");
        $(divContainer).css("overflow", "auto");

        let divGaugesWrapper = WidgetCommon.createElement("div");
        divGaugesWrapper.className = "__toggleable";
        divContainer.appendChild(divGaugesWrapper);


        this.#renderGauges(rag_soc, indicatori, divGaugesWrapper, gaugeClickCallback, urlMeteoWs);


        if (viewGrid) {
            //*********************************************************************************
            // Genero la griglia...
            //*********************************************************************************

            let divGridWrapper = WidgetCommon.createElement("div", "__toggleable ___hidden", "height: 100%;");
            divContainer.appendChild(divGridWrapper);

            this.#renderGrid(indicatori, divGridWrapper);
        }
    }

    static #makeSwitch(elem) {

        let divSwitch = WidgetCommon.createElement("div");
        elem.appendChild(divSwitch);

        let toggle = WidgetCommon.createElement("div", "__toggle");
        divSwitch.appendChild(toggle);

        $.each([{ text: "Visualizzazione dashboard", value: 1 }, { text: "Visualizzazione griglia", value: 2 }], function (i, s) {

            let label = WidgetCommon.createElement("label");

            let radio = document.createElement("input");
            radio.type = "radio";
            radio.name = "__vis";
            radio.value = s.value;
            label.appendChild(radio);

            let insideLabel = document.createElement("div");
            insideLabel.textContent = s.text;
            label.appendChild(insideLabel);

            toggle.appendChild(label);
        });

        $("input:radio[name=__vis]").val(["1"]);

        $("input:radio[name=__vis]").change(function () {

            $(".__toggle-container > .__toggleable").toggleClass("___hidden");

            if (this.value == 2) {

                let grid_elem = $(".__toggle-container > .__toggleable > .k-grid");

                if (!$(grid_elem).data("height")) {

                    let cont_h = $(".__toggle-container").height();
                    let grid_tb_h = Math.ceil(grid_elem.find(".k-grid-toolbar").outerHeight(true));
                    let grid_hdr_h = Math.ceil(grid_elem.find(".k-grid-header").outerHeight(true));
                    let grid_cont_h = Math.floor(cont_h - (grid_tb_h + grid_hdr_h) - 4);

                    grid_elem.find(".k-grid-content").css("height", grid_cont_h + "px");
                    $(grid_elem).data("height", true);
                }
            }
        });
    }

    static #renderGauges(rag_soc, indicatori, elem, gaugeClickCallback, urlMeteoWs) {

        //let groupFlag = false;
        //let Stazione = indicatori[0].Stazione;
        //for (let g = 1; (!groupFlag && g < indicatori.length); g++) {
        //    groupFlag = (indicatori[g].Stazione != Stazione);
        //}
        let Stazione = "";

        let divGauges;

        //if (!groupFlag) {
        //    divGauges = WidgetCommon.createElement("div", "indic-grid", "margin:5px;");
        //    elem.appendChild(divGauges);
        //}

        let that = this;

        $.each(indicatori, function (i, indic) {

            if (/*groupFlag && */indic.Stazione != Stazione) {

                divGauges = that.#makeGroup(elem, indic.Stazione)

                Stazione = indic.Stazione;
            }

            let divCard = that.#makeCard(divGauges, indic);

            if (indic.Risultato.Status === "outofrange" || indic.Risultato.Status === "error") {

                that.#renderError(divCard, indic);

            } else {

                that.#renderMeteoStatus(divCard, indic);

                if (indic.Risultato.Status === "progress") {

                    that.#renderProgress(divCard, indic);

                } else {

                    that.#renderGauge(divCard, rag_soc, indic, gaugeClickCallback, urlMeteoWs);
                }
            }
        });


        $(".mightOverflow").bind('mouseenter', function () {
            let $this = $(this);
            if (this.scrollWidth > this.offsetWidth) {
                $this.attr('title', $this.text());
            }
            $this.unbind('mouseenter');
        });

        if ($(".indic-meteo-status-container").length > 0) {

            $(elem).kendoPopover({
                filter: ".indic-meteo-status-container",
                body: function (e) {
                    return e.target.attr("meteo-msg");
                },
                position: "right"
            });
        }

        if ($(".warn-tooltip").length > 0) {

            // Gestione del tooltip...
            $(elem).kendoPopover({ //kendoTooltip({
                filter: ".warn-tooltip",
                //showOn: "mouseenter",
                position: "left",
                //autoHide: false,
                //animation: false,
                //content: function (e) {
                //    let target = e.target; // the element for which the tooltip is shown
                //    return target.attr("warn-msg");
                //},
                body: function (e) {
                    return e.target.attr("warn-msg");
                }//,
                //show: function (e) {
                //    this.popup.element.addClass("indic-tooltip");
                //}
            });

            //    let cssTooltip = " .indic-tooltip.k-widget.k-tooltip { ";
            //    cssTooltip += " padding: 3px 15px; ";
            //    cssTooltip += " white-space: nowrap; ";
            //    cssTooltip += " border: 2px solid #8a6d3b; ";
            //    cssTooltip += " background-color: #fcf8e3; ";
            //    cssTooltip += " color: #8a6d3b; ";
            //    cssTooltip += " } ";
            //    cssTooltip += " .indic-tooltip > .k-callout-n { border-bottom-color: transparent; } ";
            //    cssTooltip += " .indic-tooltip > .k-callout-s { border-top-color: transparent; } ";
            //    cssTooltip += " .indic-tooltip > .k-callout-e { border-left-color: transparent; } ";
            //    cssTooltip += " .indic-tooltip > .k-callout-w { border-right-color: transparent; } ";
            //    cssTooltip += " .indic-tooltip > .k-tooltip-content { ";
            //    cssTooltip += " font-size: larger; ";
            //    cssTooltip += " white-space: nowrap; ";
            //    cssTooltip += " } ";
            //    let ttStyle = document.createElement('style');        
            //    ttStyle.type = "text/css";
            //    ttStyle.innerHTML = cssTooltip;
            //    elem.appendChild(ttStyle);
        }
    }

    static #makeGroup(elem, groupName) {

        let divGroup = WidgetCommon.createElement("div", "indic-group");
        elem.appendChild(divGroup);

        let divHeader = WidgetCommon.createElement("div", "k-block k-shadow indic-group-header");
        let divStazioneText = WidgetCommon.createElement("div");
        divStazioneText.innerHTML = "<span>" + groupName + "</span>";

        divHeader.appendChild(divStazioneText);
        divGroup.appendChild(divHeader);

        let divGauges = WidgetCommon.createElement("div", "indic-grid");
        divGroup.appendChild(divGauges);

        return divGauges;
    }

    static #makeCard(elem, indic) {

        let divBlock = WidgetCommon.createElement("div", "k-block k-shadow indic", "", this.#_IdBlock(indic,false));
        elem.appendChild(divBlock);

        let divTitle = WidgetCommon.createElement("div", "k-block indic-title");
        let divAppezzaSpecie = WidgetCommon.createElement("div", "", "font-weight: bold; margin-bottom: 4px");
        if (indic.AuxData != undefined && indic.AuxData != null &&
            indic.AuxData.Appezzamento != undefined && indic.AuxData.Appezzamento != null) {
            let infoSpecieVarieta = "";
            if (indic.AuxData.Specie != undefined && indic.AuxData.Specie != null) {
                infoSpecieVarieta = " - " + indic.AuxData.Specie;
                if (indic.AuxData.Varieta != undefined && indic.AuxData.Varieta != null) {
                    infoSpecieVarieta = infoSpecieVarieta + ", " + indic.AuxData.Varieta
                }
            }
            divAppezzaSpecie.innerHTML = "<span>" + indic.AuxData.Appezzamento + infoSpecieVarieta + "</span>";
            divTitle.appendChild(divAppezzaSpecie);
        }
        else if (indic.Specie != undefined && indic.Specie != null && indic.Specie != "") {
            divAppezzaSpecie.innerHTML = "<span>" + indic.Specie + "</span>";
            divTitle.appendChild(divAppezzaSpecie);
        }
        divTitle.innerHTML = divTitle.innerHTML + "<span>" + indic.Modello + "</span>";
        let divAvversita = WidgetCommon.createElement("div", "", "font-style: italic;");
        divAvversita.innerHTML = "<span>" + indic.Avversita + "</span";
        divTitle.appendChild(divAvversita);
        let divParam = WidgetCommon.createElement("div");
        divParam.innerHTML = "<span>" + indic.DescrParametri + "</span";
        divTitle.appendChild(divParam);
        divBlock.appendChild(divTitle);

        let divCard = WidgetCommon.createElement("div", "indic-wrapper " + indic.Risultato.Status + "-indic", "", this.#_IdCard(indic, false));
        divBlock.appendChild(divCard);

        return divCard;
    }

    static #renderError(elem, indic) {

        let divMsg = WidgetCommon.createElement("div", "indic-elem indic-error-msg");
        elem.appendChild(divMsg);
        let msg = WidgetCommon.createElement("div", "vertical-center", "width: 100%;");
        msg.innerHTML = indic.Risultato.StatusMsg;
        divMsg.appendChild(msg);
    }

    static #renderMeteoStatus(elem, indic) {

        let risultato = indic.Risultato;

        if (risultato.MeteoStatus !== "info" && risultato.MeteoStatus !== "warning")
            return;

        let divMeteo = WidgetCommon.createElement("div", "indic-meteo-status-container");

        let classIcon = "indic-meteo-status-icon k-icon ";

        if (risultato.MeteoStatus === "info") {
            classIcon += "k-i-information";
        } else {
            classIcon += "k-i-exclamation-circle";
        }

        let spanIcon = WidgetCommon.createElement("span", classIcon)

        divMeteo.appendChild(spanIcon);

        elem.appendChild(divMeteo);

        divMeteo.setAttribute("meteo-msg", risultato.MeteoMsg);

    }

    static #renderProgress(elem, indic) {

        let divMsg = WidgetCommon.createElement("div", "indic-elem indic-progress-msg");
        elem.appendChild(divMsg);
        let msg = WidgetCommon.createElement("div", "vertical-center", "width: 100%;");
        msg.innerHTML = indic.Risultato.StatusMsg;
        divMsg.appendChild(msg);

        let divProgress = WidgetCommon.createElement("div", "indic-elem progress-gauge");
        elem.appendChild(divProgress);

        let h = $(divProgress).height() - 10;
        let divBar = WidgetCommon.createElement("div", "progress-gauge-bar-container", "height:" + h + "px;");
        divProgress.appendChild(divBar);

        divBar.appendChild(WidgetCommon.createElement("div", "bar-overlay"));

        let divIndic = WidgetCommon.createElement("div", "bar-progress", "width: " + Math.round(indic.Risultato.Value) + "%;");
        divBar.appendChild(divIndic);
    }

    static #renderGauge(elem, rag_soc, indic, clickCallback, urlMeteoWs) {

        let divMsg = WidgetCommon.createElement("div", "indic-elem indic-msg");
        elem.appendChild(divMsg);

        let msg = WidgetCommon.createElement("div", "vertical-center mightOverflow", "width:100%; overflow:hidden;");
        msg.innerHTML = indic.Risultato.AuxMsg;
        divMsg.appendChild(msg);

        this.#CreateDivButtonsDSS(indic, elem, urlMeteoWs);

        let divLblInfoPrint = this.#createLblInfoPrint(indic, rag_soc);
        elem.appendChild(divLblInfoPrint);

        let divGauge = WidgetCommon.createElement("div", "indic-elem indic-gauge", "", this.#_IdGauge(indic,false));
        elem.appendChild(divGauge);

        let divlbl = this.#createLblDatePrint(indic);
        elem.appendChild(divlbl);


        if (indic.Risultato.Status === "warning") {

            if (indic.Risultato.AuxMsg === "") {

                msg.innerHTML = indic.Risultato.StatusMsg;

            } else {

                msg.innerHTML = "";
                msg.classList.remove("mightOverflow");

                let padded = WidgetCommon.createElement("div", "", "padding:0px 30px");
                msg.appendChild(padded);

                let msg1 = WidgetCommon.createElement("div", "mightOverflow", "width:100%; overflow:hidden;");
                msg1.innerHTML = indic.Risultato.AuxMsg;
                padded.appendChild(msg1);
                let divWarn = WidgetCommon.createElement("div", "warn-tooltip vertical-center");
                //msg1.id = "gauge-msg-" + (i + 1);
                //divWarn.setAttribute("data-msg-id", msg1.id);
                divWarn.setAttribute("warn-msg", indic.Risultato.StatusMsg);
                //divWarn.setAttribute("aux-msg", indic.Risultato.AuxMsg);
                //divWarn.appendChild(WidgetCommon.createElement("span", "fa fa-exclamation", "margin-top: 4px;"));
                divWarn.appendChild(WidgetCommon.createElement("span", "k-icon k-i-comment", "font-size: 19px;"));
                divMsg.appendChild(divWarn);
            }
        }

        let rippleElem = WidgetCommon.createElement("div", "ripple");
        divGauge.appendChild(rippleElem);

        this.#renderBands(divGauge, indic.Risultato);

        divGauge.style.cursor = "pointer";
        divGauge.onclick = function (e) {

            if (typeof clickCallback === "function") {

                let callback_params = {
                    DataInizio: indic.Risultato.DataInizio,
                    DataFine: indic.Risultato.DataFine,
                    Tipo_Sorgente: indic.Parametri.Tipo_Sorgente,
                    Stazione_Cod: indic.Parametri.Stazione_Cod,
                    Mod_Cod: indic.Parametri.Mod_Cod,
                    Veg_Cod: indic.Parametri.Veg_Cod,
                    Avv_Cod: indic.Parametri.Avv_Cod,
                    Alg_Cod: indic.Parametri.Alg_Cod,
                    ParametriElaborazione: indic.Parametri.ParametriElaborazione
                }

                clickCallback(callback_params);
            }
        };

        CheckInfoTrattamentiAvversita(indic, elem, urlMeteoWs);
    }

    static #renderBands(gaugeElem, risultato) {

        let cntBands = risultato.Bands.length;
        if (cntBands < 2) {
            return;
        }

        let ledH = 36;
        let bandH = $(gaugeElem).height() - ledH;

        let divBands = WidgetCommon.createElement("div", "indic-gauge-bands-container", "height:" + bandH + "px;");
        let divLeds = WidgetCommon.createElement("div", "indic-gauge-leds-container", "height:" + ledH + "px;");

        gaugeElem.appendChild(divBands);
        gaugeElem.appendChild(divLeds);

        let scale_Max = risultato.Bands[cntBands - 2].Value * (10.0 / 9.0); //Rosso per l'ultimo 10% della banda
        let percFact = 100.0 / (scale_Max - risultato.Scale_Min);
        let valuePerc = Math.max(5, Math.min(95, (risultato.Value - risultato.Scale_Min) * percFact));

        divBands.appendChild(WidgetCommon.createElement("div", "indic-gauge-bands", "width:" + valuePerc + "%;"));
        divBands.appendChild(WidgetCommon.createElement("div", "indic-gauge-bands-overlay"));

        let currPerc = 0;
        let prevPerc = 0;
        let bandsBkgnd = [];
        $.each(risultato.Bands, function (ib, band) {

            let nclr = parseInt(band.Color.slice(1), 16);
            let rgba = "rgba(" + (nclr >> 16) + ", " + ((nclr >> 8) & 0x00FF) + ", " + (nclr & 0x0000FF);

            prevPerc = currPerc;
            currPerc = Math.round(Math.min(100.0, band.Value * percFact));

            if (prevPerc > 0) {
                bandsBkgnd.push(rgba + ", 0.4) " + prevPerc + "%")
            }

            if (ib < cntBands - 1) {
                bandsBkgnd.push(rgba + ", 0.4) " + currPerc + "%")
            }

            let divLed = WidgetCommon.createElement("div", "indic-gauge-led");

            divLed.style.left = ((prevPerc + currPerc) * 0.5) + "%";

            if (prevPerc < valuePerc && valuePerc <= currPerc) {

                divLed.style.backgroundColor = rgba + ", 1)";
                divLed.style.boxShadow = "rgba(0, 0, 0, 0.2) 0 -1px 7px 1px, inset #444444 0 -1px 6px, " + band.Color + " 0 2px 12px";

            } else {

                divLed.style.backgroundColor = rgba + ", 0.2)";
                divLed.style.boxShadow = "rgba(0, 0, 0, 0.2) 0 -1px 7px 0px, inset #888888 0 -1px 6px";
            }

            divLeds.appendChild(divLed);
        });

        divBands.style.backgroundImage = "linear-gradient(90deg, " + bandsBkgnd.join(", ") + ")";
    }

    static #renderGrid(indicatori, elem) {

        let divGrid = WidgetCommon.createElement("div");
        elem.appendChild(divGrid);

        let risk_tmplt = "# if (rischio != '') { # ";
        risk_tmplt += "#:rischio#";
        risk_tmplt += "<div class='___semaforo' style='background-color: #:colore#;'></div>";
        //risk_tmplt += "<div class='___semaforo' style='background-color: #:colore#; box-shadow: rgb(0 0 0 / 20%) 0px -1px 7px 1px, rgb(68 68 68) 0px -1px 6px inset, #:colore# 0px 2px 12px'></div>";
        risk_tmplt += " # } #";

        let kendo_columns = [
            {
                field: "stazione",
                title: "Stazione"
            },
            {
                field: "modello",
                title: "Modello"
            },
            {
                field: "settings",
                title: "Impostazioni calcolo"
            },
            {
                field: "rischio",
                title: "Rischio",
                headerAttributes: { "class": "___allineadestra" },
                attributes: { "class": "___allineadestra" },
                template: risk_tmplt
            },
            {
                field: "messaggio",
                title: "Messaggio"
            }
        ];

        let model = {};
        $.each(kendo_columns, function (i, c) {
            model[c.field] = 1;
        });

        $.each(indicatori, function (i, d) {

            if (d.Risultato.OutputGridValues) {

                $.each(d.Risultato.OutputGridValues, function (j, v) {

                    let field = v.field + "_" + v.valueType;

                    if (!model.hasOwnProperty(field)) {

                        model[field] = 1;

                        let column = { field: field, title: v.title };

                        if (String(v.valueType).toUpperCase() !== "STRING") {

                            column.headerAttributes = { "class": "___allineadestra" };
                            column.attributes = { "class": "___allineadestra" };

                            if (v.outputFormat !== "") {
                                column.format = "{0:" + v.outputFormat + "}";
                            }
                        }

                        kendo_columns.push(column);
                    }
                });
            }
        });

        let kendo_rows = [];

        $.each(indicatori, function (i, indic) {

            let indic_row = {
                stazione: indic.Stazione,
                modello: indic.Modello,
                settings: indic.DescrParametri,
                rischio: "",
                messaggio: ""
            };

            if (indic.Risultato.OutputGridValues) {

                $.each(indic.Risultato.OutputGridValues, function (j, v) {

                    let field = v.field + "_" + v.valueType;
                    indic_row[field] = v.value;
                });
            }

            if (indic.Risultato.Status === "outofrange" || indic.Risultato.Status === "error") {

                indic_row.messaggio = indic.Risultato.StatusMsg;

            } else {

                if (indic.Risultato.Status === "progress") {

                    indic_row.messaggio = indic.Risultato.StatusMsg + " (" + kendo.toString(indic.Risultato.Value, '0.0') + "%)";

                } else {

                    let msg = indic.Risultato.AuxMsg;

                    if (indic.Risultato.Status === "warning") {

                        if (indic.Risultato.AuxMsg === "") {

                            msg = indic.Risultato.StatusMsg;

                        } else {

                            msg = indic.Risultato.StatusMsg + " " + indic.Risultato.AuxMsg;
                        }
                    }

                    indic_row.messaggio = msg;

                    let riskAvail = ["Basso", "Medio", "Alto", "Estremo"];

                    let nBands = indic.Risultato.Bands.length;
                    let b = 0;
                    while (b < nBands - 1) {
                        if (indic.Risultato.Value < indic.Risultato.Bands[b].Value) {
                            indic_row.rischio = riskAvail[Math.min(b, riskAvail.length - 1)];
                            indic_row.colore = indic.Risultato.Bands[b].Color;
                            b = nBands;
                        }
                        b++;
                    }
                    if (b < nBands) {
                        indic_row.rischio = riskAvail[Math.min(b, riskAvail.length - 1)];
                        indic_row.colore = indic.Risultato.Bands[b].Color;
                    }

                    //if (indic_row.colore && indic_row.colore.startsWith("#")) {
                    //    let hex = indic_row.colore;
                    //    let hex2rgb = "rgb(" + hex.replace(/^#?([a-f\d])([a-f\d])([a-f\d])$/i
                    //        , (m, r, g, b) => '#' + r + r + g + g + b + b)
                    //        .substring(1).match(/.{2}/g)
                    //        .map(x => parseInt(x, 16))
                    //        .join(",") + ")";
                    //    indic_row.colore = hex2rgb;
                    //}
                }
            }

            kendo_rows.push(indic_row);
        });

        let idExportBtn = "dssGridExportBtn";
        let tbar_template = "<div id='" + idExportBtn + "' class='k-button'><span class='fa fa-file-excel-o' style='font-size:larger;'></span></div>";

        $(divGrid).kendoGrid({
            dataSource: {
                data: kendo_rows
            },
            columns: kendo_columns,
            resizable: true,
            pageable: false,
            toolbar: kendo.template(tbar_template),
            excel: {
                fileName: "RiepilogoIndicatoriDSS.xlsx",
                filterable: false
            },
            excelExport: function (e) {

                let sheet = e.workbook.sheets[0];
                let rd = 0;
                for (let r = 0; r < sheet.rows.length; r++) {

                    let row = sheet.rows[r];

                    row.cells[3].textAlign = "right";

                    if (row.type === "header") {

                        for (let c = 0; c < row.cells.length; c++) {
                            row.cells[c].background = "#428BCA";
                            row.cells[c].verticalAlign = "center";
                        }

                    } else if (row.type === "data") {

                        row.cells[3].color = e.data[rd].colore;
                        rd++;
                    }
                }
            }
        });

        $(divGrid).find(".k-grid-toolbar").css({ "padding": "5px" });
        //$(divGrid).find(".k-grid-content").css("height", "400px");

        $("#" + idExportBtn).click(function () {
            $(divGrid).data("kendoGrid").saveAsExcel();
        });
    }

    static #createLblInfoPrint(indic,rag_soc) {

        let divlabel = WidgetCommon.createElement("div", "indic-elem", "top: 30px;padding-top: 5px;text-align:center;display: none;", this.#_IdLblInfoPrint(indic, false));

        let label = WidgetCommon.createElement("label")

        if (rag_soc !== undefined && rag_soc !== null && rag_soc !== "") {
            label.innerHTML = rag_soc + " - " + indic.Stazione;
        } else {
            label.innerHTML = indic.Stazione;
        }

        label.style = "font-size: 14px;font-weight: bold;";

        divlabel.appendChild(label);

        return divlabel;
    };

    static #createLblDatePrint(indic) {

        let divlabel = WidgetCommon.createElement("div", "indic-elem", "top: 150px;padding-top: 5px;text-align:center;display: none;", this.#_IdLblDatePrint(indic, false));

        let label = WidgetCommon.createElement("label")

        let data_stampa = new Date();

        label.innerHTML = "Data di Stampa: " + data_stampa.toLocaleDateString();

        label.style = "font-size: 14px;font-weight: bold;";

        divlabel.appendChild(label);

        return divlabel;
    };

    static #_IdLblDatePrint(indic, withGate) {
        if (withGate) {
            return "#LblDatePrint_" + this.#_getKeyStation(indic);
        } else {
            return "LblDatePrint_" + this.#_getKeyStation(indic);
        }
    }

    static #_IdGauge(indic, withGate) {
        if (withGate) {
            return "#Gauge_" + this.#_getKeyStation(indic);
        } else {
            return "Gauge_" + this.#_getKeyStation(indic);
        }
    }

    static #_IdLblInfoPrint(indic, withGate) {
        if (withGate) {
            return "#LblInfoPrint_" + this.#_getKeyStation(indic);
        } else {
            return "LblInfoPrint_" + this.#_getKeyStation(indic);
        }
    }

    static #_IdExportDSS(indic, withGate) {
        if (withGate) {
            return "#ExportDSS_" + this.#_getKeyStation(indic);
        } else {
            return "ExportDSS_" + this.#_getKeyStation(indic);
        }
    }

    static #_IdBlock(indic, withGate) {
        if (withGate) {
            return "#Block_" + this.#_getKeyStation(indic);
        } else {
            return "Block_" + this.#_getKeyStation(indic);
        }
    }

    static #_IdCard(indic, withGate) {
        if (withGate) {
            return "#Card_" + this.#_getKeyStation(indic);
        } else {
            return "Card_" + this.#_getKeyStation(indic);
        }
    }

    static #_getKeyStation(indic) {
        return indic.Parametri.Stazione_Cod + "_" + indic.Parametri.Tipo_Sorgente + "_" + indic.Parametri.Avv_Cod + "_" + indic.Parametri.Veg_Cod;
    }

    static #_IdSaveDSS(indic, withGate) {
        if (withGate) {
            return "#SaveDSS_" + this.#_getKeyStation(indic);
        } else {
            return "SaveDSS_" + this.#_getKeyStation(indic);
        }
    }

    static #ExportDSS(indic) {

        if (gestioneWaitFrame === true && WaitFrame !== undefined)
            WaitFrame.show();

        let IdCard = this.#_IdCard(indic, true);

        let IdLblDatePrint = this.#_IdLblDatePrint(indic, true);

        let IdBlock = this.#_IdBlock(indic, true);

        let KeyStation = this.#_getKeyStation(indic);

        let IdLblInfoLabel = this.#_IdLblInfoPrint(indic, true);

        let IdGauge = this.#_IdGauge(indic, true);

        $(IdLblDatePrint).show();

        $(IdLblInfoLabel).show();

        let original_height_card = $(IdCard).height();

        let new_height_card = original_height_card + 65;

        $(IdCard).css("height", new_height_card + "px");

        let original_height_gauge = $(IdGauge).height();

        let new_height_gauge = original_height_gauge + 30;

        $(IdGauge).css("top", new_height_gauge + "px");

        kendo.drawing.drawDOM(IdBlock)
            .then(function (group) {
                // Render the result as a PDF file
                return kendo.drawing.exportPDF(group, {
                    paperSize: "auto",
                    margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" }
                });
            })
            .done(function (data) {

                $(IdLblDatePrint).hide();

                $(IdLblInfoLabel).hide();

                $(IdCard).css("height", "");

                $(IdGauge).css("top", "");

                if (gestioneWaitFrame === true && WaitFrame !== undefined)
                    WaitFrame.hide();

                // Save the PDF file
                kendo.saveAs({
                    dataURI: data,
                    fileName: "DSS_Difesa_" + KeyStation + ".pdf"
                });
            });
    }

    static #CreateDivButtonsDSS(indic, elem, urlMeteoWs) {

        let that = this;

        let styledivButton = $(".indic-meteo-status-container").length > 0 ? "padding-left: 27px;" : "";

        let divBtns = WidgetCommon.createElement("div", "div-btn-dss", styledivButton);

        let btnPrint = WidgetCommon.createElement("div", "", "cursor: pointer;", this.#_IdExportDSS(indic, false));

        btnPrint.onclick = function () {
            that.#ExportDSS(indic);
        };

        btnPrint.setAttribute("print-DSS-msg", "Stampa Consiglio");      

        let spandivButtonPrint = WidgetCommon.createElement("span", "indic-meteo-status-icon k-icon k-i-printer");

        btnPrint.appendChild(spandivButtonPrint);

        divBtns.appendChild(btnPrint);

        let btnSave = WidgetCommon.createElement("div", "", "cursor: pointer;", this.#_IdSaveDSS(indic, false));

        btnSave.onclick = function () {
            that.#CheckDSSDifesa(indic, urlMeteoWs);
        };

        btnSave.setAttribute("save-DSS-msg", "Salva Consiglio");

        let spandivButtonSave = WidgetCommon.createElement("span", "indic-meteo-status-icon k-icon k-i-floppy");

        btnSave.appendChild(spandivButtonSave);

        divBtns.appendChild(btnSave);

        elem.appendChild(divBtns);

        $(that.#_IdExportDSS(indic, true)).kendoPopover({
            body: function (e) {
                return e.target.attr("print-DSS-msg");
            },
            position: "right"
        });

        $(that.#_IdSaveDSS(indic, true)).kendoPopover({
            body: function (e) {
                return e.target.attr("save-DSS-msg");
            },
            position: "right"
        });
    }

    static #SaveDSSDifesa(indic, urlMeteoWs) {

        ajaxAgronica(urlMeteoWs + "/SalvaDSSDifesa",
            kendo.stringify({
                parametri: indic.Parametri,
                dataInizio: new Date(indic.Risultato.DataInizio),
                dataFine: new Date(indic.Risultato.DataFine),
                risultatoStr: kendo.stringify(indic.Risultato)
            }),
            function (risposta) {
                MessaggioTuttoOK_Bootstrap("Consiglio Salvato", "DIV_Messaggi");
            },
            function (risposta) {
                console.log("ERRORE DSS Difesa (SaveDSSDifesa)");
            },
            null,
            false
        );
    }

    static #CheckDSSDifesa(indic, urlMeteoWs) {

        let that = this;

        ajaxAgronica(urlMeteoWs + "/ControllaDSSDifesa",
            kendo.stringify({
                parametri: indic.Parametri
            }),
            function (risposta) {

                if (risposta.RispostaOK) {
                    if (parseInt(risposta.RispostaStringa) > 0) {

                        const stazione = indic.Stazione + " (" + indic.Modello + ")";

                        const data = new Date().toLocaleDateString();

                        const message = "Consiglio già presente per la Stazione " + stazione + " con Data Esecuzione " + data + ", procedere comunque con il salvataggio?";

                        kendo.confirm(message).then(function () {
                            that.#SaveDSSDifesa(indic, urlMeteoWs);
                        }, function () {

                        });

                    } else {
                        that.#SaveDSSDifesa(indic, urlMeteoWs);
                    }
                }
            },
            function (risposta) {
                console.log("ERRORE DSS Difesa");
            },
            null,
            gestioneWaitFrame//true
        );
    }
}

var indirizzohttp = "./DSS_Gauges.aspx";
function SetSessionParametri(params) {
    var parametri = kendo.stringify({
        params: JSON.stringify(params)
    });

    ajaxAgronica(indirizzohttp + "/PreparaParametriIndicatore",
        parametri,
        function (risposta) {
            window.parent.postMessage({ sourceId: parentWinId, callbackParams: risposta.RispostaStringa }, ottieniTargetOrigin(window));
        }, null,
        null, gestioneWaitFrame);
}

var letturaImpostazioniCopertura = false;
var visualizzaRiepilogoCoperturaDaImpostazioni = false;
var pathNetCoreApi = null;
var ggIntervalloCheckTrattamento = 30;
var sogliaMinimaPerPioggeCumulative = null;

async function CheckInfoTrattamentiAvversita(indic, parentElem, urlMeteoWs) {

    if (!letturaImpostazioniCopertura) {

        visualizzaRiepilogoCoperturaDaImpostazioni = LeggiImpostazioneVisualizzaRiepilogoCopertura(urlMeteoWs) == true;
        letturaImpostazioniCopertura = true;
    }

    if (visualizzaRiepilogoCoperturaDaImpostazioni &&
        indic.AuxData != undefined && indic.AuxData != null &&
        indic.AuxData.ChiaveImpianto != undefined && indic.AuxData.ChiaveImpianto != null &&
        indic.AuxData.ChiaveImpianto.Piva != "" &&
        indic.AuxData.ChiaveImpianto.Sa_Cod != 0 &&
        indic.AuxData.ChiaveImpianto.Appezza != 0 &&
        indic.AuxData.ChiaveImpianto.ID_Reg != 0) {

        SetInfoTrattamentiAvversita(indic, parentElem, urlMeteoWs);
    }
}

async function SetInfoTrattamentiAvversita(indic, parentElem, urlMeteoWs) {

    let infoTrattamentoElem = WidgetCommon.createElement("div", "k-block indic-avv");
    let parentElemH = parentElem.offsetHeight + 135;
    parentElem.style.height = parentElemH.toString() + "px"//"180px";
    parentElem.appendChild(infoTrattamentoElem);

    if (pathNetCoreApi === null) {
        pathNetCoreApi = LeggiPathNetCoreApi(urlMeteoWs);

        if (pathNetCoreApi === null) {
            infoTrattamentoElem.innerHTML = "Errore nella lettura dei dati trattamento per avversità."
            return;
        }
    }

    var rispTrattamenti = null;
    try {
        rispTrattamenti = await CheckUltimoTrattamentoAvversita(indic);
    }
    catch (error) {
        infoTrattamentoElem.innerHTML = "Errore nella lettura dei dati trattamento per avversità."
        return;
    }
    
    if (rispTrattamenti === undefined || rispTrattamenti === null) {
        infoTrattamentoElem.innerHTML = "Errore nella lettura dei dati trattamento per avversità."
        return;
    }

    if (rispTrattamenti.length == 0) {
        infoTrattamentoElem.innerHTML = "Nessun trattamento registrato negli ultimi " + ggIntervalloCheckTrattamento + " giorni."
        return;
    }

    var Data_Ultimo_Trattamento = rispTrattamenti[0].Data_Ultimo_Trattamento;

    var Fr_Des = "";
    var Lav_Des = "";
    var Intervallo_Trattamento = 0;

    for (let i = 0; i < rispTrattamenti.length; i++) {

        var DoseEtichetta_Value = rispTrattamenti[i].DoseEtichetta_Value;
        var arrDoseEtichetta = DoseEtichetta_Value.split("$");
        var FormulatiXAllegatiNormative_IDRiga = 0;
        switch (arrDoseEtichetta.length) {
            case 24:
                FormulatiXAllegatiNormative_IDRiga = arrDoseEtichetta[20];
                break;
            case 23:
                FormulatiXAllegatiNormative_IDRiga = arrDoseEtichetta[19];
                break;
            default:
                FormulatiXAllegatiNormative_IDRiga = 0;
                break;
        }

        var rispIntervalloTrattamento = null;
        try {
            rispIntervalloTrattamento = await LeggiIntervalloMinTrattamento(rispTrattamenti[i].Fr_Cod, indic.Parametri.Veg_Cod, indic.Parametri.Avv_Cod, Data_Ultimo_Trattamento, FormulatiXAllegatiNormative_IDRiga, urlMeteoWs);
        }
        catch (error) {
        }

        if (rispIntervalloTrattamento !== undefined && rispIntervalloTrattamento !== null && rispIntervalloTrattamento > Intervallo_Trattamento) {
            Fr_Des = rispTrattamenti[i].Fr_Des;
            Lav_Des = rispTrattamenti[i].Lav_Des;
            Intervallo_Trattamento = rispIntervalloTrattamento;
        }
        else if (i == 0) {
            Fr_Des = rispTrattamenti[i].Fr_Des;
            Lav_Des = rispTrattamenti[i].Lav_Des;
        }
    }

    infoTrattamentoElem.innerHTML = "Data ultimo trattamento: " + new Date(Data_Ultimo_Trattamento).toLocaleDateString() + " (prodotto: " + Fr_Des + ", tipo trattamento: " + Lav_Des + ").<br/>";
    if (Intervallo_Trattamento == 0) {
        infoTrattamentoElem.innerHTML += "Dati per giorni copertura prodotto mancanti.<br/>";
    }
    else {
        infoTrattamentoElem.innerHTML += "Giorni di copertura prodotto: " + Intervallo_Trattamento.toString() + ".<br/>";
    }

    if (sogliaMinimaPerPioggeCumulative === null) {
        sogliaMinimaPerPioggeCumulative = LeggiSogliaMinimaPerPioggeCumulative(urlMeteoWs);

        if (sogliaMinimaPerPioggeCumulative === null) {
            sogliaMinimaPerPioggeCumulative = 0;
        }
    }

    if (indic.Risultato !== undefined && indic.Risultato !== null &&
        indic.Risultato.EventiPioggia !== undefined && indic.Risultato.EventiPioggia !== null) {

        if (indic.Risultato.EventiPioggia.length > 0) {
            var lastEventoPioggia = null;

            for (let i = 0; i < indic.Risultato.EventiPioggia.length; i++) {
                var eventoPioggia = indic.Risultato.EventiPioggia[i];
                if (eventoPioggia.Cumulato_mm >= sogliaMinimaPerPioggeCumulative) {
                    if (lastEventoPioggia === null || eventoPioggia.InizioEvento > lastEventoPioggia.InizioEvento) {
                        lastEventoPioggia = eventoPioggia;
                    }
                }
            }

            if (lastEventoPioggia === null) {
                infoTrattamentoElem.innerHTML += "Le precipitazioni recenti sono inferiori alla soglia configurata di " + sogliaMinimaPerPioggeCumulative + " mm.";
            }
            else {
                var lastEventoPioggia_InizioEvento = new Date(lastEventoPioggia.InizioEvento).toLocaleDateString() + " " + new Date(lastEventoPioggia.InizioEvento).toLocaleTimeString(/*[], { hour: "2-digit" }*/)
                infoTrattamentoElem.innerHTML += "Ultima pioggia in data " + lastEventoPioggia_InizioEvento + " (cumulata: " + lastEventoPioggia.Cumulato_mm + " mm, durata: " + lastEventoPioggia.Durata_hh + " h).";
            }
        }
        else {
            infoTrattamentoElem.innerHTML += "Nessuna precipitazione recente rilevata.";
        }        
    }
    else {
        infoTrattamentoElem.innerHTML += "I dati pioggia non sono attualmente disponibili.";
    }
}

function LeggiImpostazioneVisualizzaRiepilogoCopertura(urlMeteoWs) {

    let risp = null;
    //var Params = {
    //    piva: piva
    //}
    ajaxAgronicaSync(urlMeteoWs + "/LeggiImpostazioneVisualizzaRiepilogoCopertura",
        kendo.stringify({}), false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        },
        function (risposta) {
            console.log("ERRORE DSS Difesa: impossibile ottenere indirizzo per chiamata API: " + risposta.Errore);
        },
        null, gestioneWaitFrame);
    return risp == "true";
}

function LeggiPathNetCoreApi(urlMeteoWs) {

    let risp = null;
    ajaxAgronicaSync(urlMeteoWs + "/LeggiPathNetCoreApi",
        kendo.stringify({}), false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        },
        function (risposta) {
            console.log("ERRORE DSS Difesa: impossibile ottenere indirizzo per chiamata API: " + risposta.Errore);
        },
        null, gestioneWaitFrame);
    return risp;
}

function LeggiSogliaMinimaPerPioggeCumulative(urlMeteoWs) {
    let risp = null;
    ajaxAgronicaSync(urlMeteoWs + "/LeggiSogliaMinimaPerPioggeCumulative",
        kendo.stringify({}), false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            console.log("ERRORE DSS Difesa: leggere valore parametro 'Soglia minima significativa per piogge cumulative (mm)': " + risposta.Errore);
        },
        null, gestioneWaitFrame);
    return risp;
}

async function CheckUltimoTrattamentoAvversita(indic) {

    let risp = null;    
    return new Promise((resolve, reject) => {        
        let urlEndpointParams = "Piva=" + indic.AuxData.ChiaveImpianto.PIva +
            "&Sa_Cod=" + indic.AuxData.ChiaveImpianto.Sa_Cod +
            "&APPEZZA=" + indic.AuxData.ChiaveImpianto.Appezza +
            "&ID_REG=" + indic.AuxData.ChiaveImpianto.Id_Reg +
            "&Av_Cod=" + indic.Parametri.Avv_Cod +
            "&Intervallo=" + ggIntervalloCheckTrattamento;        
        ajaxAgronicaApiCoreStdGetAsync(pathNetCoreApi + "/LeggiTrattamentiPerAvversita?" + urlEndpointParams,
            null,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp);
            }, function (risposta) {
                console.log("ERRORE DSS Difesa: chiamata ad API per lettura trattamenti per avversità in errore: " + risposta.Errore);
                //resolve(null);
                reject();
            },
            false, null);
    });
}

async function LeggiIntervalloMinTrattamento(Fr_Cod, Veg_Cod, Av_Cod, Data, FormulatiXAllegatiNormative_IDRiga, urlMeteoWs) {
    let risp = null;    
    return new Promise((resolve, reject) => {
        var Params = {
            Fr_Cod: Fr_Cod,
            Veg_Cod: Veg_Cod,
            Av_Cod: Av_Cod,
            Data: Data,
            FormulatiXAllegatiNormative_IDRiga: FormulatiXAllegatiNormative_IDRiga
        }
        ajaxAgronica(urlMeteoWs + "/LeggiIntervalloMinTrattamento",
            kendo.stringify(Params),
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp);
            },
            function (risposta) {
                console.log("ERRORE DSS Difesa: chiamata a WS per lettura intervallo minimo prodotto, da etichetta, in errore: " + risposta.Errore);
                //resolve(null);
                reject();
            },
            null,
            gestioneWaitFrame//true
        );
    });
}
