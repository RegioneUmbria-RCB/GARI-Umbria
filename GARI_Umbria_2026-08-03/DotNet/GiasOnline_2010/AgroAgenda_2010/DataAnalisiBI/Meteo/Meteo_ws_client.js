

function analisiMeteo() {

    $("#divKendoOut").data("meteoOutput").clear();
    GlobalMeteoTabstrip.showTabDati(false);

    let sorgenteMeteo = GlobalMeteoSourceSelector.sorgenteMeteo()

    if (sorgenteMeteo.tipo === "" || sorgenteMeteo.stazione === "") {
        return;
    }

    let dataInizio = $("#txt_DataDa").data("kendoDatePicker").value();
    let dataFine = $("#txt_DataA").data("kendoDatePicker").value();

    setMeteoStorage(
        {
            DataInizio: dataInizio,
            DataFine: dataFine,
            TipoSorgente: sorgenteMeteo.tipo,
            Sorgente: sorgenteMeteo.stazione
        }
    );

    let freq = "G";
    let freq_chkbox = document.getElementById("switch-freq-dati");
    if (freq_chkbox.checked) {
        freq = "O";
    }

    let param = {
        DataDa: kendo.toString(dataInizio, "d"),
        DataA: kendo.toString(dataFine, "d"),
        FrequenzaDati: freq,
        TipoSorgente: sorgenteMeteo.tipo,
        Sorgente: sorgenteMeteo.stazione,
        SogliaTemp: $("#txt_Soglia_Germinazione").getKendoNumericTextBox().value(),
        SogliaFabbisognoFreddo: $("#txt_Soglia_FabbisognoFreddo").getKendoNumericTextBox().value(),
        CfrStorico: $("#serie_storiche").getKendoMultiSelect().value().join("|")
    };

    ajaxAgronica(url_meteo_ws + "/DatiMeteo_Elabora",
        JSON.stringify(param),
        function (risposta) {

            let risp = risposta.RispostaStringa;

            if (risp.Meteo_RiepilogoPeriodo !== undefined && risp.Meteo_RiepilogoPeriodo !== null) {
                meteoRiepilogo(JSON.parse(risp.Meteo_RiepilogoPeriodo));
            }

            let kendodata = JSON.parse(risp.Meteo_Table);

            let cntStorico = $("#serie_storiche").getKendoMultiSelect().value().length;
            if (cntStorico > 0) {
                kendodata.group_columns = { css_group: "bordosinistro bordoinferiore groupheader grassetto", css_group_column: "bordosinistro" };
            }

            let divKendoGrid = kendoGrid_Inizializza(kendodata);

            let grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();
            let kendocharts = JSON.parse(risp.Meteo_Charts);

            if (cntStorico === 0) {

                $.each(kendocharts, function (idx, chart) {

                    $.each(chart.series, function (i, s) {

                        s.tooltipTemplate = "0.00";

                        if (s.type === "rangeArea") {

                            s.line = { style: "smooth" };

                        } else if (s.type === "column") {

                            s.gap = 0;
                            s.spacing = 0;

                        }
                    });

                    creaKendoChart2("", chart.horizAxis, chart.axis, chart.series, grid_ds);
                });

            } else {

                $.each(kendocharts, function (idx, chart) {

                    $.each(chart.series, function (i, s) {

                        //s.tooltipTemplate = "0.00";
                        delete s.color;

                        if (s.type === "rangeArea") {

                            s.line = { style: "smooth" };
                            s.opacity = 0.25;

                        } else if (s.type === "column") {

                            s.gap = 2;
                            s.spacing = 0;

                        }
                    });

                    let divChart = creaKendoChart2("", chart.horizAxis, chart.axis, chart.series, grid_ds);

                    if (divChart !== "" && chart.series.length > (cntStorico + 1)) {

                        let needBind = false;
                        $.each(chart.series, function (i, s) {
                            if (s.fromField) {
                                needBind = true;
                                return false;
                            }
                        });

                        if (needBind) {

                            let chart = $("#" + divChart).getKendoChart();

                            chart.one("dataBound", function (e) {

                                let colors = [];
                                for (var s = 0; s < this.options.series.length / 2; s++) {
                                    colors.push(this.options.series[s].color);
                                }
                                for (var s = 0; s < this.options.series.length / 2; s++) {
                                    this.options.series[(s * 2)].color = colors[s];
                                    this.options.series[(s * 2) + 1].color = colors[s];
                                }
                            });

                            chart.refresh();
                        }
                    }
                });

            }

            GlobalMeteoTabstrip.showTabDati(true);
        },
        function (risposta) {

            meteoAlert(TraduzioneMultiResx(datiMeteoResx, "DatiMeteo", "Dati meteo"), risposta.Errore);
        }
    );
}



function meteoRiepilogo(riepilogo) {

    if (riepilogo.length === 0)
        return;

    let meteoOutput = $("#divKendoOut").data("meteoOutput");
    let divRiep = meteoOutput.creaDiv("meteoRiepilogo");

    let element = document.getElementById(divRiep);
     
    let wrapper = document.createElement("div");
    wrapper.style.display = "flex";
    wrapper.style.marginBottom = "20px";
    element.appendChild(wrapper);

    let block = document.createElement("div");
    block.className = "k-block";
    wrapper.appendChild(block);

    let hdr = document.createElement("div");
    hdr.className = "k-header";
    hdr.style.textAlign = "center";
    hdr.innerHTML = TraduzioneMultiResx(datiMeteoResx, "RiepilogoDatiMeteo", "Riepilogo dati meteo");
    block.appendChild(hdr);

    let nrows = riepilogo.length;
    let cols_val = ["Valore"];

    let keys = Object.keys(riepilogo[0]);
    if (keys.indexOf("Valore") < 0) {

        let obj_cols = {};
        let idx = 0;
        while (idx < riepilogo.length) {

            keys = Object.keys(riepilogo[idx]);

            $.each(keys, function (ik, k) {

                if (k !== "Sensore" && k !== "Aggreg" && k !== "UM") {
                    if (!obj_cols.hasOwnProperty(k)) {
                        obj_cols[k] = true;
                    }
                }
            });
            idx++;
        }
        cols_val = Object.keys(obj_cols);
        nrows++;
    }

    let cont = [
        { prop: "Sensore" },
        { prop: "Aggreg", justify: "right", color: "#a9a9a9" }
    ];
    $.each(cols_val, function (c, col) {
        cont.push({ prop: col, justify: "right" });
    });
    cont.push({ prop: "UM", color: "#a9a9a9" });

    let grid = document.createElement("div");
    grid.style.display = "grid";
    grid.style.gridTemplateColumns = "repeat(" + cont.length + ", auto)";
    grid.style.gridTemplateRows = "repeat(" + nrows + ", 2em)";
    grid.style.alignItems = "center";
    grid.style.gridGap = "5px 25px";
    grid.style.padding = "5px 10px 5px";

    block.appendChild(grid);

    if (cols_val.length > 1) {

        $.each(cols_val, function (c, col) {

            let gcol = document.createElement("div");
            gcol.style.gridColumn = (c + 3) + " / span 1";
            gcol.style.justifySelf = "right";
            gcol.style.color = "#a9a9a9";
            gcol.innerHTML = col.replace("Valore_", "");
            grid.appendChild(gcol);
        });

        grid.appendChild(document.createElement("div"));
    }

    $.each(riepilogo, function (r, obj) {

        $.each(cont, function (idx, elem) {

            let text = obj[elem.prop];
            if (text === undefined) {

                text = "";
            } else {

                if (typeof text === "number") {
                    text = kendo.toString(text, '0.00');
                }
            }

            let cell = document.createElement("div");
            if (elem.color) {
                cell.style.color = elem.color;
            }
            if (elem.justify) {
                cell.style.justifySelf = elem.justify;
            }
            cell.innerHTML = text;
            grid.appendChild(cell);
        });
    });
}

