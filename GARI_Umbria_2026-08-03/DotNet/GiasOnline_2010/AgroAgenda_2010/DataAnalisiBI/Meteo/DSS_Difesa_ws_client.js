

function elaboraModello() {

    $("#divKendoOut").data("meteoOutput").clear();
    GlobalMeteoTabstrip.showTabDati(false);

    let sorgenteMeteo = GlobalMeteoSourceSelector.sorgenteMeteo()

    if (sorgenteMeteo.tipo === "" || sorgenteMeteo.stazione === "") {
        return;
    }

    let range = elabPeriodoPlugin().getRange();

    if (range.start === undefined || range.end === undefined) {
        return;
    }

    setMeteoStorage(
        {
            DataInizio: range.start,
            DataFine: range.end,
            TipoSorgente: sorgenteMeteo.tipo,
            Sorgente: sorgenteMeteo.stazione
        }
    );

    let Veg_Cod = parseInt($("#cmbSpecieVegetale").getKendoDropDownList().value());
    if (isNaN(Veg_Cod)) {
        return;
    }

    let ddl_Modelli = $("#cmbAvModAlg").getKendoDropDownList();
    let modello = ddl_Modelli.dataItem();
    if (modello === undefined) {
        return;
    }

    let parametriAggiuntivi = LeggiParametriXModello(modello.Mod_Cod);

    let param = {
        DataDa: kendo.toString(range.start, "d"),
        DataA: kendo.toString(range.end, "d"),
        TipoSorgente: sorgenteMeteo.tipo,
        Sorgente: sorgenteMeteo.stazione,
        ModelloPrevisionale: modello.Mod_Cod,
        Veg_Cod: Veg_Cod,
        Av_Cod: modello.Avv_Cod,
        Algoritmo: modello.Alg_Cod,
        ParametriAggiuntivi: JSON.stringify(parametriAggiuntivi)
    };

    ajaxAgronica(url_meteo_ws + "/ModelliPrevisionali_Elabora",    
        JSON.stringify(param),
        function (risposta) {

            let risp = risposta.RispostaStringa;

            let meteoOutput = $("#divKendoOut").data("meteoOutput");

            if ((risp.Modello_Titolo != null && risp.Modello_Titolo != "") || (risp.Modello_Descrizione != null && risp.Modello_Descrizione != "")) {

                if (risp.Modello_Titolo === undefined)
                    risp.Modello_Titolo = "";

                let flagDescr = (risp.Modello_Descrizione != null && risp.Modello_Descrizione != "");

                let divHeader = meteoOutput.creaDiv("meteoTitolo");
                let divWrapper = document.createElement("div");
                divWrapper.style.cssText = "margin-bottom: 10px; cursor:default; border-radius: 5px; border: 1px solid #ccc; background-color: #eee;";
                document.getElementById(divHeader).appendChild(divWrapper);
                let divTitolo = document.createElement("div");
                divTitolo.style.cssText = "padding: 5px; text-align: center; font-size: large;";
                if (flagDescr) {
                    divTitolo.style.cssText += "cursor: pointer;";
                    divTitolo.setAttribute("data-toggle", "collapse");
                    divTitolo.setAttribute("data-target", "#idModelloRisultati");
                }
                let divTitolo2 = document.createElement("div");
                divTitolo2.className = "hvr-icon-pop";
                divTitolo2.style.cssText = "position:relative;";
                divTitolo2.innerHTML = risp.Modello_Titolo;

                if (flagDescr) {
                    let divInfo = document.createElement("div");
                    divInfo.className = "fa fa-info-circle fa-lg hvr-icon";
                    divInfo.style.cssText = "position:absolute; left:0; top:0; margin:3px 0px 0px;";
                    divTitolo2.appendChild(divInfo);
                }

                divTitolo.appendChild(divTitolo2);
                divWrapper.appendChild(divTitolo);

                if (flagDescr) {
                    let divModelloRisultati = document.createElement("div");
                    divModelloRisultati.id = "idModelloRisultati";
                    divModelloRisultati.className = "collapse";
                    let divDescr = document.createElement("div");
                    divDescr.style.cssText = "padding: 15px 10px 10px; border-radius: 0px 0px 5px 5px; background-color: #fafafa; border-top: 1px solid #ccc;";
                    divDescr.innerHTML = risp.Modello_Descrizione;
                    divModelloRisultati.appendChild(divDescr);
                    divWrapper.appendChild(divModelloRisultati);
                }

            }

            if (risp.Modello_WarningMsg != null && risp.Modello_WarningMsg != "") {
                var divWarn = meteoOutput.creaDiv();
                $("#" + divWarn).html(risp.Modello_WarningMsg);
            }

            creaOutputModello(modello.Mod_Cod, risp.Modello_Tabella1, 1, risp.Modello_EngineId, risp.Modello_CodiceEsterno);
            creaOutputModello(modello.Mod_Cod, risp.Modello_Tabella2, 2, risp.Modello_EngineId, risp.Modello_CodiceEsterno);
            creaOutputModelloMeteo(risp.Modello_TabellaMeteo);

            if (risp.Modello_Disclaimer != null && risp.Modello_Disclaimer != "") {
                var divDisc = meteoOutput.creaDiv();
                $("#" + divDisc).html(risp.Modello_Disclaimer);
            }

            GlobalMeteoTabstrip.showTabDati(true);
        },
        function (risposta) {

            let messaggio = risposta.Errore;
            if (messaggio === "") {
                messaggio = TraduzioneMultiResx(datiMeteoResx, "DatiMeteoNonDisponibiliNelPeriodo", "Dati meteo non disponibili nel periodo selezionato...");
            }

            meteoAlert(TraduzioneMultiResx(datiMeteoResx, "ElaborazioneModelloPrevisionale", "Elaborazione modello previsionale"), messaggio);
        }
    );
}



//*************************************************************************************************
//*************************************************************************************************
//*************************************************************************************************



function creaOutputModello(modello, strTabellaKendo, indice, engineId, codiceEsterno) {

    if (strTabellaKendo === null || strTabellaKendo === undefined || strTabellaKendo === "") {
        return;
    }

    var kendodata = JSON.parse(strTabellaKendo);

    if (engineId == 1) {
        creaOutputModello_DssDifesaEngine(codiceEsterno, kendodata, indice)
    }
    else {
        creaOutputModello_LegacyEngine(modello, kendodata, indice)
    }
}

function creaOutputModello_LegacyEngine(modello, kendodata, indice) {

    switch (parseInt(modello)) {

        case Enum_ModelloPrevisionale.RitardoVariabile:
            creaOutput_RitardoVariabile(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__BatteriosiKiwi_PSA:
            creaOutput_Agronomica_30__BatteriosiKiwi_PSA(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Ticchiolatura_del_Melo:
            creaOutput_Agronomica_30__Ticchiolatura_del_Melo(kendodata, indice);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Maculatura_del_Pero:
            creaOutput_Agronomica_30__Maculatura_del_Pero(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Peronospora_della_Vite:
            creaOutput_Agronomica_30__Peronospora_della_Vite(kendodata, indice);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Oidio_della_Vite:
            creaOutput_Agronomica_30__Oidio_della_Vite(kendodata, indice);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Botrite_della_Vite:
            creaOutput_Agronomica_30__Botrite_della_Vite(kendodata, indice);
            break;

        case Enum_ModelloPrevisionale.Racca__PeroPom:
        case Enum_ModelloPrevisionale.Racca__AlterPom:
        case Enum_ModelloPrevisionale.Racca__OidioPom:
        case Enum_ModelloPrevisionale.Racca__BotriPom:
        case Enum_ModelloPrevisionale.Racca__PeroBiet:
        case Enum_ModelloPrevisionale.Racca__OidioBiet:
        case Enum_ModelloPrevisionale.Racca__CercoBiet:
        case Enum_ModelloPrevisionale.Racca__PeroPat:
        case Enum_ModelloPrevisionale.Racca__AlterPat:
        case Enum_ModelloPrevisionale.Racca__ScleroSoia:
        case Enum_ModelloPrevisionale.Racca__BrusoneRiso:
        case Enum_ModelloPrevisionale.Racca__ElmintosporiosiMais:
        case Enum_ModelloPrevisionale.Racca__BipolarisMaidis:
        case Enum_ModelloPrevisionale.Racca__AntracnosiOlivo:
            creaOutput_Racca(kendodata);
            break;

        case Enum_ModelloPrevisionale.Racca__MicotoxMais:
            creaOutput_RaccaMicotox(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__MRV_Eulia:
        case Enum_ModelloPrevisionale.Agronomica_30__MRV_CydiaMolesta:
        case Enum_ModelloPrevisionale.Agronomica_30__MRV_Carpocapsa:
        case Enum_ModelloPrevisionale.Agronomica_30__MRV_Helicoverpa:
        case Enum_ModelloPrevisionale.Agronomica_30__MRV_Tignoletta:
        case Enum_ModelloPrevisionale.Agronomica_30__MRV_MoscaOlivo:
        case Enum_ModelloPrevisionale.MRV_PiralideMais:
        case Enum_ModelloPrevisionale.MRV_NottuaMais:
            creaOutput_Agronomica_30__MRV(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__MISP_IPI_Pomodoro:
            creaOutput_Agronomica_30__MISP_IPI_Pomodoro(kendodata, indice);
            break;

        case Enum_ModelloPrevisionale.UniCatt__AFLA_Mais:
            creaOutput_UniCatt__AFLA_Mais(kendodata);
            break;

        case Enum_ModelloPrevisionale.UniCatt__FER_Mais:
            creaOutput_UniCatt__FER_Mais(kendodata);
            break;

        case Enum_ModelloPrevisionale.UniCatt__Fusariosi_Frumento:
            creaOutput_UniCatt__Fusariosi_Frumento(kendodata);
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__Colpo_di_fuoco:
            creaOutput_Agronomica_30__ColpoDiFuoco(kendodata);
            break;

        case Enum_ModelloPrevisionale.BetaCoProB__Cercosporiosi:
            creaOutput_BetaCoProB__Cercosporiosi(kendodata);
            break;

        case Enum_ModelloPrevisionale.RaccaFrumento_RuggineBruna:
        case Enum_ModelloPrevisionale.RaccaFrumento_RuggineGialla:
        case Enum_ModelloPrevisionale.RaccaFrumento_RuggineNera:
        case Enum_ModelloPrevisionale.RaccaFrumento_Stagonosporiosi:
        case Enum_ModelloPrevisionale.RaccaFrumento_Septoria:
        case Enum_ModelloPrevisionale.RaccaFrumento_Fusariosi:
        case Enum_ModelloPrevisionale.RaccaFrumento_FusariosiSpiga:
        case Enum_ModelloPrevisionale.RaccaFrumento_Fusariosi2:
        case Enum_ModelloPrevisionale.RaccaFrumento_Fusariosi3:
        case Enum_ModelloPrevisionale.RaccaFrumento_MarciumeRosa:
            creaOutput_RaccaFrumento(kendodata);
            break;
    }
}

function creaOutputModello_DssDifesaEngine(codiceEsterno, kendodata, indice) {

    if (codiceEsterno === null || codiceEsterno === undefined || codiceEsterno === "") {
        return;
    }

    switch (codiceEsterno) {

        case "batteriosi-kiwi_v0":
            creaOutput_Agronomica_30__BatteriosiKiwi_PSA(kendodata);
            break;

        case "botrite-vite_v0":
            creaOutput_Agronomica_30__Botrite_della_Vite(kendodata, indice);
            break;

        case "colpo-fuoco_v0":
            creaOutput_Agronomica_30__ColpoDiFuoco(kendodata);
            break;

        case "fusariosi-frumento_v0":
            creaOutput_UniCatt__Fusariosi_Frumento(kendodata);
            break;

        case "maculatura-pero_v0":
            creaOutput_Agronomica_30__Maculatura_del_Pero(kendodata);
            break;

        case "misp-ipi_v0":
            creaOutput_Agronomica_30__MISP_IPI_Pomodoro(kendodata, indice);
            break;

        case "oidio-vite_v0":
            creaOutput_Agronomica_30__Oidio_della_Vite(kendodata, indice);
            break;

        case "peronospora-vite_v0":
            creaOutput_Agronomica_30__Peronospora_della_Vite(kendodata, indice);
            break;

        case "alternaria-patata_v0":
        case "alternaria-pomodoro_v0":
        case "antracnosi-olivo_v0":
        case "bipolaris-maidis_v0":
        case "botrite-pomodoro_v0":
        case "brusone-riso_v0":
        case "cercospora-bietola_v0":
        case "elmintosporiosi-mais_v0":
        case "fusariosi-avenaceum_v0":
        case "fusariosi-culmorum_v0":
        case "fusariosi-graminearum_v0":
        case "fusariosi-poae_v0":
        case "marciume-rosa - invernale_v0":
        case "oidio-bietola_v0":
        case "oidio-pomodoro_v0":
        case "peronospora-bietola_v0":
        case "peronospora-patata_v0":
        case "peronospora-pomodoro_v0":
        case "ruggine-bruna_v0":
        case "ruggine-gialla_v0":
        case "ruggine-nera_v0":
        case "sclerotinia - soia_v0":
        case "septoria_v0":
        case "stagonosporiosi_v0":
            creaOutput_Racca(kendodata);
            break;

        case "micotox-mais_v0":
            creaOutput_RaccaMicotox(kendodata);
            break;

        case "fusariosi-avenaceum_v0":
        case "fusariosi-culmorum_v0":
        case "fusariosi-graminearum_v0":
        case "fusariosi-poae_v0":
        case "marciume-rosa-invernale_v0":
        case "ruggine-bruna_v0":
        case "ruggine-gialla_v0":
        case "ruggine-nera_v0":
        case "septoria_v0":
        case "stagonosporiosi_v0":
            creaOutput_RaccaFrumento(kendodata);
            break;

        case "carpocapsa-melo_v0":
        case "eulia-vite_v0":
        case "mosca-olivo_v0":
        case "nottua-mais_v0":
        case "nottua-pomodoro_v0":
        case "piralide-mais_v0":
        case "tignola-pesco_v0":
        case "tignoletta-vite_v0":
            creaOutput_RitardoVariabile(kendodata);
            break;

        case "ticchiolatura-melo_v0":
            creaOutput_Agronomica_30__Ticchiolatura_del_Melo(kendodata, indice);
            break;
    }
}


function creaOutputModelloMeteo(strTabellaKendo) {

    if (strTabellaKendo === null || strTabellaKendo === undefined || strTabellaKendo === "") {
        return;
    }

    let kendodata = JSON.parse(strTabellaKendo);

    kendodata.title = TraduzioneMultiResx(datiMeteoResx, "TitoloTabellaDatiMeteo", "Dati meteo");

    kendoGrid_Inizializza(kendodata);
}


function creaOutput_RitardoVariabile(kendodata) {

    kendodata.group_columns = { css_group: "groupheader" };
    let divKendoGrid = kendoGrid_Inizializza(kendodata);

    let kendogrid = $("#" + divKendoGrid).data("kendoGrid");

    let mrvPresenza = new Array;
    let mrvCumulo = new Array;
    let rcols = new Array;
    for (let lev0 = 0; lev0 < kendogrid.columns.length; lev0++) {

        let column_0 = kendogrid.columns[lev0];

        if (column_0.columns === undefined) {
            rcols.push({
                field: column_0.field,
                add: 1
            });
        } else {

            let title = column_0.title;
            //Serie "finta" solo per la legenda...
            mrvCumulo.push({
                name: title,
                field: "Gen_" + lev0,
                gruppo: lev0
            });
            mrvPresenza.push({
                name: title,
                field: "Gen_" + lev0,
                gruppo: lev0
            });

            for (let lev1 = 0; lev1 < column_0.columns.length; lev1++) {

                let column_1 = column_0.columns[lev1];

                title = column_0.title + " " + column_1.title;

                for (let lev2 = 0; lev2 < column_1.columns.length; lev2++) {

                    let column_2 = column_1.columns[lev2];

                    rcols.push({
                        field: column_2.field,
                        add: 0
                    });

                    if (column_2.field.indexOf("Cumulo") >= 0) {

                        mrvCumulo.push({
                            name: title,
                            field: column_2.field,
                            tooltipTemplate: { showSeriesName: true, format: "0\\\\%" },
                            visibleInLegend: false,
                            gruppo: lev0
                        });
                    }
                    if (column_2.field.indexOf("Presenza") >= 0) {

                        let dashType = "solid";
                        if (column_2.field.indexOf("Uova") >= 0) {
                            dashType = "dot";
                        } else if (column_2.field.indexOf("Larve") >= 0) {
                            dashType = "dash";
                        } else if (column_2.field.indexOf("Pupe") >= 0) {
                            dashType = "longDash";
                        }

                        mrvPresenza.push({
                            name: title,
                            field: column_2.field,
                            tooltipTemplate: { showSeriesName: true, format: "0\\\\%" },
                            visibleInLegend: false,
                            dashType: dashType,
                            gruppo: lev0
                        });
                    }
                }
            }
        }
    }

    let grid_ds = kendogrid.dataSource.data();
    let chart_ds = new Array
    for (let i = 0; i < grid_ds.length; i++) {

        let obj = new Object;

        for (let c = 0; c < rcols.length; c++) {

            if (rcols[c].add === 1) {
                obj[rcols[c].field] = grid_ds[i][rcols[c].field];
            } else if (rcols[c].add === 0) {
                let grid_val = grid_ds[i][rcols[c].field];
                let val = +(Math.round(grid_val + "e+2") + "e-2");
                if (val > 0) {
                    obj[rcols[c].field] = grid_val;
                    if (val == 100) {
                        rcols[c].add = -1;
                    }
                    chart_ds.push(obj);
                }
            }
        }
    }

    creaKendoChart(TraduzioneMultiResx(datiMeteoResx, "RitardoVariabileCumulo", "Ritardo variabile (cumulo)"), "Data", { max: 105 }, mrvCumulo, chart_ds);
    creaKendoChart(TraduzioneMultiResx(datiMeteoResx, "RitardoVariabilePresenza", "Ritardo variabile (presenza)"), "Data", "", mrvPresenza, chart_ds);
}


function creaOutput_Agronomica_30__BatteriosiKiwi_PSA(kendodata) {

    let chartDiv = creaChartDiv();

    kendodata.title = TraduzioneMultiResx(datiMeteoResx, "CalcoloIndiceDiRischioMoltiplicazioneBattericaPSA", "Calcolo indice di rischio moltiplicazione batterica di PSA - Pseudomonas syringae var. actinidiae");
    kendodata.tooltip = {
        column: "M_Orario",
        content: '<div style="font-size: larger;"><span>'
            + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica1', 'Indice moltiplicazione batterica per ogni ora con bagnatura fogliare')
            + '</span></div><div style="margin-top: 15px; margin-bottom: 10px;"><span>'
            + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica2', 'Se')
            + ' <i>' + TraduzioneMultiResx(datiMeteoResx, 'TooltipIndiceMoltiplicazioneBatterica3', 'Bagnatura fogliare') + '</i></span></div>'
            + '<div style="padding-left: 1em;"><span>M = -0.000003*temp<sup>4</sup>-0.00011*temp<sup>3</sup>+0.00201*temp<sup>2</sup>+0.0541*temp+0.247</span></div>'
    };

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var giorno = 0;
    var sumPrecip = 0;
    var psa_data = grid_ds.map(function (x) {

        var riga = new Object;

        riga.DataOra = x.DataOra;
        if (x.Risk_Index > 0)
            riga.Risk_Index = x.Risk_Index;

        var x_gg = x.DataOra.getDate();
        if (x_gg != giorno) {
            if (giorno > 0) {
                riga.Precip = sumPrecip;
            }
            giorno = x_gg;
            sumPrecip = 0;
        }

        sumPrecip += x.Precipitazione;

        return riga;
        /*
                if (x['Risk_Index'] > 0)
                    return { DataOra: x['DataOra'], Risk_Index: x['Risk_Index'] };
                else
                    return { DataOra: x['DataOra'] };
        */
    });

    var vertAxes = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio")
            }
        },
        {
            name: "P_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "PioggiaMM", "Pioggia (mm)")
            }
        }
    ];

    var series = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio"),
            color: "rgb(255, 0, 0)",
            field: "Risk_Index"
        },
        {
            type: "column",
            name: TraduzioneMultiResx(datiMeteoResx, "PioggiaMM", "Pioggia (mm)"),
            color: "rgb(0, 128, 255)",
            gap: 0,
            spacing: 0,
            field: "Precip",
            axis: "P_Axis"
        }
    ];

    if (typeof kendodata.plotBands === "object") {
        vertAxes = {
            axes: vertAxes,
            bands: kendodata.plotBands
        };
    }

    creaKendoChart("", { field: "DataOra", baseUnit: "hours" }, vertAxes, series, psa_data, chartDiv);

}


function creaOutput_Agronomica_30__Ticchiolatura_del_Melo(kendodata, indice) {

    if (indice == 1) {

        let seriesPAT123 = [
            {
                name: "PAT'",
                color: "rgb(0, 128, 255)",
                field: "PAT1",
                tooltipTemplate: "0.00"
            },
            {
                name: "PAT",
                color: "rgb(192, 192, 192)",
                field: "PATest",
                tooltipTemplate: "0.00"
            },
            {
                name: "PAT\"",
                color: "rgb(255, 128, 0)",
                field: "PAT2",
                tooltipTemplate: "0.00"
            }
        ];

        creaKendoChart2("", "Data", { title: { text: "PAT" }, max: 1.01 }, seriesPAT123, kendodata.kendo_rows);

        let serieDeltaPAT = [
            {
                name: "DeltaPAT - potenziale rilascio ascosporico",
                color: "rgb(218, 165, 32)",
                field: "DeltaPATDinamico",
                tooltipTemplate: "0.00"
            }
        ];

        creaKendoChart2("", "Data", TraduzioneMultiResx(datiMeteoResx, "TitoloAsseDeltaPAT", "DeltaPAT - potenziale rilascio ascosporico (%)"), serieDeltaPAT, kendodata.kendo_rows);

    } else if (indice == 2) {

        kendodata.tooltip = [
            {
                column: "DataInizio",
                content: TraduzioneMultiResx(datiMeteoResx, "DataInizioRilascioAscospore", "<div>Data inizio rilascio ascospore dopo pioggia utile.</div>")
            },
            {
                column: "DataFine",
                content: TraduzioneMultiResx(datiMeteoResx, "DataFineRilascioDipendenteDaMeteo", "<div>Data di fine rilascio dipendente dalle condizioni meteo.</div>")
            },
            {
                column: "Lwcorr",
                content: TraduzioneMultiResx(datiMeteoResx, "OreBagnaturaDuranteEventoRilascio", "<div>Ore di bagnatura durante evento di rilascio.</div>")
            },
            {
                column: "dur_min_bagn",
                content: TraduzioneMultiResx(datiMeteoResx, "OreBagnaturaCausantiInfezione", "<div>Ore di bagnatura necessarie per causare infezione (Stensvand).</div>")
            },
            {
                column: "PAT3",
                content: TraduzioneMultiResx(datiMeteoResx, "PotenzialeDiAscosporeRilasciabiliStagione", "<div>Potenziale di ascospore mature rilasciabili nella stagione (0÷1).</div>")
            },
            {
                column: "deltaPAT",
                content: TraduzioneMultiResx(datiMeteoResx, "ValoreIndiceProporzioneAscospore", "<div>Rappresenta la proporzione di ascospore rilasciate nell'aria durante l'evento di rilascio.</div>")
            },
            {
                column: "INF",
                content: TraduzioneMultiResx(datiMeteoResx, "ValoreIndiceInfettivitàAscospore", "<div>Valore indice (0÷1): Infettività delle ascospore depositate. Indica la quantità di ascospore rilasciate che penetrano nei tessuti vegetali.</div>")
            },
            {
                column: "Tsens",
                content: TraduzioneMultiResx(datiMeteoResx, "ValoreIndiceTessutiVegetaliSensibiliAllaMalattia", "<div>Valore indice (0÷1): Indica la presenza di tessuti vegetali sensibili alla malattia. I tessuti vegetali sensibili sono quelli giovani della rosetta fiorale e dei getti.</div>")
            },
            {
                column: "RISK",
                content: TraduzioneMultiResx(datiMeteoResx, "IndiceRischioGlobale", "<div>Indice di rischio globale, dipendente da entità rilascio, infettività, presenza di tessuti sensibili.</div>")
            },
            {
                column: "Perc_Inc",
                content: TraduzioneMultiResx(datiMeteoResx, "PercentualeIncubazione", "<div>Indica la % di incubazione o la data di fine periodo di incubazione (inizio comparsa sintomi su foglie).</div>")
            }
        ];

        kendodata.group_columns = { css_group: "groupheader" };

        $.each(kendodata.indicators, function (idx, indic) {
            if (indic.type === "numbers") {
                if (indic.fieldVal === "deltaPAT") {
                    indic.class = "fungo";
                } else if (indic.fieldVal === "INF") {
                    indic.class = "bersaglio";
                } else if (indic.fieldVal === "Tsens") {
                    indic.class = "foglia";
                }
            }
        });

        let divGrid = kendoGrid_Inizializza(kendodata);
        let grid_ds = $("#" + divGrid).getKendoGrid().dataSource;

        $("#" + divGrid + " .k-grid-content").find("tr").each(function (i, el) {
            $(el).removeClass("k-alt")
            let uid = $(el).attr("data-uid");
            let obj = grid_ds.getByUid(uid);
            if (obj !== undefined) {
                if (obj.Valido === 1) {
                    //$(el).addClass("k-alt");
                    $(el).css("background-color", "#f5f5f5");
                } else {
                    //$(el).addClass("k-state-disabled");
                    $(el).css({ "background-color": "#fff", "color": "#ccc" });
                }
            }
        });
    }
}


function creaOutput_Agronomica_30__Maculatura_del_Pero(kendodata) {

    kendodata.tooltip = [
        {
            column: "Data",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "Temp",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "Prec",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "UmRel",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "LW",
            content: TraduzioneMultiResx(datiMeteoResx, "DatoSfasatoDiOttoOreIndietro", "<div>Dato sfasato di 8 ore indietro.</div>")
        },
        {
            column: "BSPspor",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceSporulazione", "<div>Indice sporulazione</div>")
        },
        {
            column: "BSPspor3gg",
            content: TraduzioneMultiResx(datiMeteoResx, "MediaUltimiTreGiorniBSPspor", "<div>Media mobile ultimi 3 giorni di BSPspor</div>")
        },
        {
            column: "BSPcast",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceCondizioniAmbientaliFavorevoliPerInfezioni", "<div>Indice condizioni ambientali favorevoli per le infezioni</div>")
        },
        {
            column: "BSPcast3gg",
            content: TraduzioneMultiResx(datiMeteoResx, "MediaUltimiTreGiorniBSPcast", "<div>Media mobile ultimi 3 giorni di BSPcast</div>")
        },
        {
            column: "Global_risk",
            content: TraduzioneMultiResx(datiMeteoResx, "IndiceCombinatoBSPsporEBSPcast", "<div>Indice combinato tra BSPspor e BSPcast, tiene conto sia delle condizioni che influenzano la sporulazione sia delle condizioni climatiche favorevoli all'infezione</div>")
        }
    ];

    $.each(kendodata.indicators, function (idx, indic) {
        if (indic.type === "numbers") {
            if (indic.fieldVal === "BSPspor3gg") {
                indic.class = "fungo";
            } else if (indic.fieldVal === "BSPcast3gg") {
                indic.class = "goccia";
            }
        }
    });

    var chartDiv = creaChartDiv();

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var vertAxes = [
        {
            title: {
                text: "BSP"
            }
        },
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            },
            axisCrossingValue: 0,
            name: "Temp_Axis"
        }
    ];

    var series = [
        {
            name: "BSPspor3gg",
            field: "BSPspor3gg",
            color: "rgb(0, 0, 192)",
            tooltipTemplate: "0.00"
        },
        {
            name: "BSPcast3gg ",
            field: "BSPcast3gg",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "Temp",
            //type: "area",
            //opacity: 0.25,
            //color: "rgb(105, 105, 105)", width: 1, opacity: 1, style: "smooth" },
            color: "rgb(105, 105, 105)",
            width: 1,
            axis: "Temp_Axis",
            tooltipTemplate: "0.00"
        }

    ];

    if (typeof kendodata.plotBands === "object") {
        vertAxes = {
            axes: vertAxes,
            bands: kendodata.plotBands
        };
    }

    creaKendoChart("", "Data", vertAxes, series, grid_ds, chartDiv);
}


function creaOutput_Agronomica_30__Peronospora_della_Vite(kendodata, indice) {
    if (indice == 1) {

        let keys = Object.keys(kendodata.kendo_model);
        let bars = [];
        //let lines = [];
        let sumObj = {};
        let numCoorte = 1;
        for (let k = 0; k < keys.length; k++) {
            if (kendodata.kendo_model[keys[k]].type === "number") {

                let found = kendodata.kendo_columns.some(function (x) { return x.field === keys[k]; });

                if (found) {

                    //lines.push({
                    //    field: keys[k],
                    //    type: "line",
                    //    visibleInLegend: false
                    //});

                } else {

                    let valField = keys[k].replace("Bar", "Val");

                    let ttTemplate = "<div>";
                    ttTemplate += "<div style='margin-bottom:5px;'>"
                        + TraduzioneMultiResx(datiMeteoResx, "NumeroCoorteOospore_", "Numero coorte oospore:") + " "
                        + kendo.toString(numCoorte++, '0')
                        + " - " + TraduzioneMultiResx(datiMeteoResx, "Intensità_", "Intensità:") + " "
                        + "#: kendo.toString(dataItem['" + keys[k] + "'], '0.00')#%</div>";
                    ttTemplate += "<div>#: kendo.toString(category, 'dd/MM/yyyy') #"
                        + " - " + TraduzioneMultiResx(datiMeteoResx, "Germinazione_", "Germinazione:") + " "
                        + "#: kendo.toString(dataItem['" + valField + "'] * 100, '0.0')#%</div>";
                    ttTemplate += "</div>";

                    bars.push({
                        field: keys[k],
                        sizeField: valField,
                        colorField: keys[k].replace("Bar", "Clr"),
                        type: "column",
                        color: "rgb(192, 192, 192)",
                        tooltipTemplate: { template: ttTemplate },
                        stack: true,
                        gap: 0,
                        spacing: 0,
                        visibleInLegend: false,
                        visual: visualColumnSeries
                    });

                    sumObj[keys[k]] = kendodata.kendo_rows[valField];
                }
            }
        }

        let sum = 0;
        //let bands = [];
        //let notes_data = [];
        //let from = 0;
        //let clrs = ["#ccc", "#fff"];
        keys = Object.keys(sumObj);
        if (kendodata.kendo_rows.length > 0) {
            let row = kendodata.kendo_rows[kendodata.kendo_rows.length - 1];
            $.each(keys, function (idx, key) {
                sum += row[key];
                //bands.push({
                //    from: from,
                //    to: sum,
                //    color: clrs[idx % 2],
                //    opacity: 0.25
                //});
                //notes_data.push({ value: sum });
                //from = sum;
            });
            sum = Math.ceil(sum) + 1;
        }

        let axis = {
            title: { text: TraduzioneMultiResx(datiMeteoResx, "IntensitàCoorteOosporeCumulo", "Intensità coorte oospore (cumulo)") },
            labels: { visible: false },
            majorTicks: { visible: false },
            majorGridLines: { visible: false },
            max: Math.max(100, sum),
            name: "dynamic_zoom"
            //plotBands: bands
            //notes: {
            //    data: notes_data,
            //    visual: function (e) {
            //        let this_chart = e.sender;
            //        if (this_chart !== undefined) {

            //            let bbox = this_chart.plotArea().backgroundVisual.bbox();
            //            let targetPoint = { x: e.rect.origin.x, y: e.rect.center().y };
            //            let line = new kendo.drawing.Path({
            //                stroke: {
            //                    width: 1,
            //                    color: "#cccccc",
            //                    dashType: "dash"
            //                }
            //            }).moveTo(targetPoint.x, targetPoint.y).lineTo(targetPoint.x + bbox.size.width, targetPoint.y);
            //            return new kendo.drawing.Group().append(line);

            //        }
            //    }
            //}
        };

        creaKendoChart(
            TraduzioneMultiResx(datiMeteoResx, "IntervalliTemporaliPrevistiGerminazioneCoorti", "Intervalli temporali previsti per la germinazione delle diverse coorti presenti"),
            "Data", axis, bars, kendodata.kendo_rows, undefined, 550);

        //let num_coorti = Math.round((bars.length / 5) + 0.4) * 5;
        //creaKendoChart("Intervalli temporali previsti per la germinazione (GER da 0 a 1) delle diverse coorti presenti", "Data", { max: num_coorti }, bars, kendodata.kendo_rows);
        //creaKendoChart("Evoluzione della germinazione (GER da 0 a 1) delle diverse coorti presenti", "Data", { max: 1.05 }, lines, kendodata.kendo_rows);

    } else if (indice == 2) {

        kendodata.group_columns = { css_group: "groupheader" };

        let fungo = "url(images/Indicatori/fungo-pieno.png)";

        let arr_tooltip = [
            {
                column: "PMO",
                content: {
                    title: "Intensità coorte oospore (%)",
                    content: "<span>Ogni evento piovoso innesca la germinazione di una coorte di oospore. L'intensità potenziale di una coorte dipende dalla quantità di spore mature (</span>" +
                        "<span style='background-image: " + fungo + "; background-size: contain; background-repeat: no-repeat; padding-left: 1.5em;'>scarsa, </span>" +
                        "<span style='background-image: " + fungo + ", " + fungo + "; background-position: 0 top, 1.5em top;background-size: contain; background-repeat: no-repeat; padding-left: 3em;'>moderata, </span>" +
                        "<span style='background-image: " + fungo + ", " + fungo + ", " + fungo + "; background-position: 0 top, 1.5em top, 3em top; background-size: contain; background-repeat: no-repeat; padding-left: 4.5em;'>elevata</span>" +
                        "<span>).</span>"
                }
            },
            {
                column: "GER0",
                content: {
                    title: "Data inizio germinazione oospore",
                    content: "Il processo di germinazione delle oospore innescato da una pioggia dipende dalla temperatura e dalla bagnatura della lettiera. Il modello simula la data in cui le coorti di oospore sono pronte a germinare."
                }
            },
            {
                column: "GER1",
                content: {
                    title: "% Germinazione / Data emissione sporangi", 
                    content: "<div>Al termine del processo di germinazione le oospore producono i macrozoosporangi che, in condizioni di bagnatura favorevoli, sono in grado di rilasciare zoospore nella lettiera.</div>" +
                        "<div style='margin-top: 15px; margin-bottom: 5px; font-weight: bold;'>" + TraduzioneMultiResx(datiMeteoResx, "SopravvivenzaMacrozoosporangi", "Sopravvivenza dei macrozoosporangi") + "</div>" +
                        "<div>Il modello simula la data di emissione dei macrozoosporangi e ne calcola il periodo di sopravvivenza. Superato questo periodo, in assenza di un velo d'acqua a determinate condizioni di temperatura e umidità relativa i macrozoosporangi muoiono (non si verifica alcuna infezione).</div>"
                }
            },
            {
                column: "ZRE",
                content: {
                    title: "Data rilascio zoospore",
                    content: "In condizioni favorevoli (presenza di un film d'acqua a temperatura e umidità relativa elevate) i macrozoosporangi rilasciano zoospore nella lettiera."
                }
            },
            {
                column: "ZDI",
                content: {
                    title: "Data dispersione zoospore per pioggia", 
                    content: "In questa fase le zoospore, molto delicate, nuotano nel film liquido e si devitalizzano solo se esposte a condizioni climatiche sfavorevoli (assenza di bagnatura della lettiera). Al contrario, le piogge sono in grado di veicolare le zoospore sulla vegetazione suscettibile (tramite gli schizzi d'acqua)."
                }
            },
            {
                column: "info",
                content: {
                    title: "Destino coorte",
                    content: "<div style='margin-bottom: 3px;'>In sintesi, le coorti possono avere destini diversi:</div>" +
                        "<ul style='padding-left: 20px;'>" +
                        "<li>in assenza di un'adeguata bagnatura della lettiera i macrozoosporangi muoiono senza rilasciare zoospore (<i>Morte sporangi</i>)</li>" +
                        "<li>una volta rilasciate le zoospore, un'interruzione di bagnatura della lettiera ne provoca la morte (<i>Morte zoospore</i>)</li>" +
                        "</ul>" +
                        "<div style='margin-bottom: 3px; margin-top: 5px;'>Ogni pioggia è in grado di veicolare zoospore vitali sul tessuto fogliare suscettibile:</div>" +
                        "<ul style='padding-left: 20px;'>" +
                        "<li>in assenza di adeguate condizioni di temperatura e bagnatura fogliare il processo si interrompe prima di poter innescare un'infezione (<i>No infezione</i>);</li>" +
                        "<li>in condizioni favorevoli di temperatura e bagnatura fogliare il processo infettivo si innesca (<i>INFEZIONE</i>).</li>" +
                        "</ul>",
                    max_width: "45em"
                }
            },
            {
                column: { column: "incub2", parent: true },
                content: {
                    title: "% Incubazione / Data comparsa sintomi",
                    content: "Nel caso in cui l'evento sia infettivo, il modello stima la percentuale di incubazione e le date di inizio e fine comparsa sintomi su foglie."
                }
            }
        ];

        kendodata.tooltip = [];

        $.each(arr_tooltip, function (i, e) {

            if (!e.content.max_width) {

                max_width = "40em";

            } else {

                max_width = e.content.max_width;
            }

            let content = "<div style='text-align: center; font-size: larger; margin-bottom: 10px;'>" + e.content.title + "</div>";
            content += "<div style='max-width: " + max_width + "; white-space: normal; text-align: justify;'>"
            content += e.content.content;
            content += "</div>";

            kendodata.tooltip.push({ column: e.column, content: content });
        });

        $.each(kendodata.indicators, function (idx, indic) {
            if (indic.type === "numbers") {
                indic.class = "fungo";
            }
        });

        kendoGrid_Inizializza(kendodata);

    }
}


function creaOutput_Agronomica_30__Oidio_della_Vite(kendodata, indice) {

    if (indice === 1) {
        kendodata.title = TraduzioneMultiResx(datiMeteoResx, "RilasciInfettantiAscospore", "Rilasci infettanti di ascospore");

        $.each(kendodata.indicators, function (idx, indic) {
            if (indic.type === "numbers") {
                indic.class = "fungo";
            }
        });

    } else if (indice === 2) {
        kendodata.title = TraduzioneMultiResx(datiMeteoResx, "InfezioniSecondarie", "Infezioni secondarie");
    }

    var chartDiv = creaChartDiv();

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var kendogrid = $("#" + divKendoGrid).data("kendoGrid");

    if (indice === 1) {
        kendogrid.dataSource.filter({ field: "filtro", operator: "eq", value: true });
    }

    var grid_ds = kendogrid.dataSource.data();

    var vertAxes;
    var series;
    var titolo = kendodata.title;

    if (indice === 1) {

        var par_title = title_for_field(kendogrid, "PAR");
        var delta_par_title = title_for_field(kendogrid, "deltaPAR");
        var pioggia_title = title_for_field(kendogrid, "pioggia");
        vertAxes = [
            {
                title: {
                    text: par_title
                },
                labels: {
                    format: "{0}%"
                },
                max: 105
            },
            {
                name: "Axis3",
                title: {
                    text: pioggia_title
                }
            },
            {
                name: "Axis2",
                title: {
                    text: delta_par_title
                }
            }
        ];

        series = [
            {
                name: par_title,
                color: "rgb(255, 192, 0)",
                field: "PAR",
                tooltipTemplate: "0\\\\%"
            },
            {
                type: "column",
                name: delta_par_title,
                color: "rgb(192, 192, 192)",
                gap: 0,
                spacing: 0,
                field: "deltaPAR",
                axis: "Axis2",
                tooltipTemplate: "0.00",
                visual: function (e) {

                    if (e.value == null)
                        return;

                    var group = new kendo.drawing.Group();
                    group.append(e.createVisual());

                    if (e.dataItem.COLONIA2 != null) {

                        var color = e.dataItem.COLONIA2_IND;
                        var center = e.rect.center();
                        center.y = e.rect.origin.y;

                        group.append(new kendo.drawing.Circle(new kendo.geometry.Circle(center, 5),
                            {
                                fill: {
                                    color: color,
                                    opacity: 0.75
                                },
                                stroke: {
                                    color: e.options.color,
                                    width: 2,
                                    opacity: 0.75
                                }
                            }));
                    }

                    return group;
                }
            },
            {
                type: "area",
                line: {
                    style: "step"
                },
                missingValues: "zero",
                name: pioggia_title,
                color: "rgb(0, 128, 255)",
                field: "pioggia",
                axis: "Axis3",
                tooltipTemplate: "0.00"
            }
        ];

    } else {

        var somma3gg = title_for_field(kendogrid, "somma_index_3gg");
        titolo += " - (" + somma3gg + ")";

        vertAxes = "";

        series = [
            {
                name: somma3gg,
                color: "rgb(192, 192, 192)",
                field: "somma_index_3gg",
                colorField: "somma_index_3gg_IND",
                type: "column",
                gap: 0,
                spacing: 0,
                visual: visualColumnSeries,
                visibleInLegend: false,
                tooltipTemplate: "0.00"
            }
        ];
    }

    creaKendoChart(titolo, "Data", vertAxes, series, grid_ds, chartDiv);

}


function creaOutput_Agronomica_30__Botrite_della_Vite(kendodata, indice) {

    if (indice == 2) {
        kendodata.group_columns = { css_group: "groupheader" };
        kendodata.tooltip = [
            {
                column: "SEV1",
                content: '<div><span>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'SeveritàInfezioneInStadiBBCHDaA', 'Severità infezione nello stadio BBCH da {0} a {1}'), 53, 73) +
                    '<br/>' + TraduzioneMultiResx(datiMeteoResx, 'ValoreCumulatoInfettivitàRelativa', 'Valore cumulato dell\'infettività relativa nelle fasi da infiorescenze a giovani grappoli') + '</span></div>' +
                    //'<div style="font-size: larger; text-align: center;">BBCH Riproduttivo</div>' +
                    //'<div style="font-size: smaller;">' +
                    '<div>' +
                    '<dl class="table-display">' +
                    '<dt>53</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'InflorescenzaVisibile', 'Inflorescenza chiaramente visibile') + '</dd>' +
                    '<dt>55</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PrimiBoccioliVisibili', 'Primi boccioli visibili (poco sviluppati)') + '</dd>' +
                    '<dt>59</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'BoccioliSviluppatiConPetali', 'Boccioli sviluppati con petali visibili') + '</dd>' +
                    '<dt>60</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PrimiFioriAperti', 'Primi fiori aperti') + '</dd>' +
                    '<dt>61</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '10%') + '</dd>' +
                    '<dt>62</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '20%') + '</dd>' +
                    '<dt>63</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '30%') + '</dd>' +
                    '<dt>64</dt><dd>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'PercentualeFioriAperti', '{0} dei fiori aperti'), '40%') + '</dd>' +
                    '<dt>65</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'PienaFiorituraPiùDiMetàDeiFioriAperti', 'Piena fioritura: almeno 50% dei fiori aperti') + '</dd>' +
                    '<dt>67</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FioriAppassiti', 'Fiori per lo più appassiti') + '</dd>' +
                    '<dt>69</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FineFiorituraPetaliCaduti', 'Fine della fioritura: tutti i petali caduti') + '</dd>' +
                    '<dt>71</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'IngrossamentoOvari', 'Ingrossamento degli ovari: frutti al 10% delle dimensioni finali') + '</dd>' +
                    '<dt>72</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'FruttiVentiPerCentoDimensioni', 'Frutti al 20% delle dimensioni finali') + '</dd>' +
                    '<dt>73</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'AciniDimensioniMignolatura', 'Acini delle dimensioni di un granello di pepe (mignolatura)') + '</dd>' +
                    '</dl>' +
                    '</div>'
            },
            {
                column: { column: "SEVtot", parent: true },
                content: '<div><span>' + kendo.format(TraduzioneMultiResx(datiMeteoResx, 'SeveritàInfezioneInStadiBBCHDaA', 'Severità infezione nello stadio BBCH da {0} a {1}'), 79, 89) +
                    '<br/>' + TraduzioneMultiResx(datiMeteoResx, 'InfettivitàConidiEMicelio', 'Infettività dovuta a conidi e da micelio passante da acino ad acino') + '</span></div>' +
                    //'<div style="font-size: larger; text-align: center;">BBCH Riproduttivo</div>' +
                    //'<div style="font-size: smaller;">' +
                    '<dl class="table-display">' +
                    '<dt>79</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'MaggioranzaAciniAdiacenti', 'La maggior parte degli acini si tocca') + '</dd>' +
                    '<dt>81</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'InizioMaturazioneBacche', 'Inizio della maturazione: le bacche iniziano a manifestare il colore tipico della cultivar') + '</dd>' +
                    '<dt>85</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'MaturazioneAvanzata', 'Maturazione avanzata') + '</dd>' +
                    '<dt>89</dt><dd>' + TraduzioneMultiResx(datiMeteoResx, 'GrappoliProntiPerLaRaccolta', 'Grappoli pronti per la raccolta') + '</dd>' +
                    '</dl>' +
                    '</div>'
            },
            {
                column: { column: "SEVtot" },
                content: TraduzioneMultiResx(datiMeteoResx, 'SommaInF2InfettivitàConidiEMicelio', '<div>Somma in F2 dei valori dell\'infettività da conidi e da micelio</div>')
            }
        ];

        $.each(kendodata.indicators, function (idx, indic) {
            if (indic.type === "numbers") {
                indic.class = "fungo";
            }
        });

    }

    var chartDiv = creaChartDiv();

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var kendogrid = $("#" + divKendoGrid).data("kendoGrid");
    var chart_ds = kendogrid.dataSource.data();

    var vertAxes = [];
    var series = [];
    var titolo = "";

    //Genero i marker lato server
    //let arrBBCH = [53, 60, Number.MAX_SAFE_INTEGER];
    //chart_ds = chart_ds.map(function (x) {
    //    if (x.BBCH === arrBBCH[0]) {
    //        arrBBCH.shift();
    //        x.BBCH_ = 1;
    //    }
    //    return x;
    //});

    if (indice == 1) {

        titolo = TraduzioneMultiResx(datiMeteoResx, "RischioInfettivo", "Rischio infettivo");

        vertAxes.push({
            name: "value_axis",
            pane: { name: "MainPane" }
        });

        series.push({
            color: "rgb(192,192,192)",
            field: "SEV",
            colorField: "SEV_IND",
            type: "column",
            gap: 0,
            spacing: 0,
            visibleInLegend: false,
            tooltipTemplate: "0.00",
            visual: visualColumnSeries
        });

    } else {

        vertAxes.push({
            title: { text: TraduzioneMultiResx(datiMeteoResx, "AbbondanzaRelativaConidi", "Abbondanza relativa dei conidi") },
            labels: { format: "{0}%" },
            max: 105,
            pane: { name: "MainPane" }
        });
        vertAxes.push({
            name: "Axis2",
            title: { text: TraduzioneMultiResx(datiMeteoResx, "SeveritàInfezione", "Severità infezione") },
            pane: "MainPane"
        });

        series.push({
            type: "column",
            name: title_for_field(kendogrid, "CISO"),
            color: "rgb(192, 192, 192)",
            gap: 0,
            spacing: 0,
            field: "CISO",
            tooltipTemplate: "0\\\\%"
        });
        series.push({
            name: title_for_field(kendogrid, "SEV1"),
            color: "rgb(192, 0, 0)",
            field: "SEV1",
            axis: "Axis2",
            tooltipTemplate: "0.00"
        });
        series.push({
            name: title_for_field(kendogrid, "SEV2"),
            color: "rgb(255, 128, 0)",
            field: "SEV2",
            axis: "Axis2"
        });
        series.push({
            name: title_for_field(kendogrid, "SEV3"),
            color: "rgb(0, 128, 255)",
            field: "SEV3",
            axis: "Axis2"
        });
        series.push({
            name: title_for_field(kendogrid, "SEVtot"),
            color: "rgb(128, 0, 128)",
            field: "SEVtot",
            axis: "Axis2",
            tooltipTemplate: "0.00"
        });
    }

    vertAxes.push({
        pane: { name: "BBCHPane", height: 32 }, //, background: "#eee" },
        name: "bbch_axis",
        min: 0,
        max: 2,
        labels: { visible: false },
        majorTicks: { visible: false },
        majorGridLines: { visible: false },
        line: { visible: false }
    });

    let ttTemplate = "<div>";
    ttTemplate += "<div style='font-size:larger; margin-bottom:5px; text-align:center;'>#: kendo.toString(category, 'd MMMM')#</div><div style='width:198px; height:198px; background: url(&quot;Images/BBCH/BBCH_#: dataItem['BBCH']#.jpg&quot;) 0 0 / contain no-repeat white;'></div>";
    ttTemplate += "</div>";

    series.push({
        axis: "bbch_axis",
        field: "BBCH_marker",
        color: "#ddd",
        highlight: {
            toggle: function (e) {
                // Don't create a highlight overlay, we'll modify the existing visual instead
                e.preventDefault();

                let visual = e.visual;
                let transform = null;
                if (e.show) {
                    let center = visual.rect().center();
                    transform = kendo.geometry.transform().scale(1.25, 1.25, center);
                }
                visual.transform(transform);
            }
        },
        markers: {
            visible: true,
            size: 32,
            visual: function (e) {

                let src = kendo.format("Images/BBCH/BBCH_{0}_32.jpg", e.dataItem.BBCH);
                let image = new kendo.drawing.Image(src, e.rect);
                return image;
            }
        },
        tooltipTemplate: { template: ttTemplate }
    });

    creaKendoChart(titolo, "Data", vertAxes, series, chart_ds, chartDiv, 500);
}


function creaOutput_Racca(kendodata) {

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var vertAxes = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "LAIRelativoIndiceDiRischio", "LAI (rel.) - Indice di rischio")
            }
        },
        {
            name: "IndiceRischioMediato_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischioMediatoSullaLatenza", "Indice di rischio mediato sulla latenza")
            }
        }
    ];

    var series = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "LAIRelativo", "LAI (rel.)"),
            color: "rgb(0, 176, 80)",
            field: "LAI"
        },
        {
            type: "column",
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio"),
            color: "rgb(255, 192, 0)",
            gap: 0,
            spacing: 0,
            field: "IndiceRischio"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischioMediatoSullaLatenza", "Indice di rischio mediato sulla latenza"),
            color: "rgb(255, 0, 0)",
            field: "IndiceRischioMediato",
            axis: "IndiceRischioMediato_Axis"
        }
    ];

    if (typeof kendodata.plotBands === "object") {
        vertAxes = {
            axes: vertAxes,
            bands: kendodata.plotBands
        };
    }

    creaKendoChart("", "Data", vertAxes, series, grid_ds);

    var vertAxesMeteo = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            }
        },
        {
            name: "Precipitazioni_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni") + " [mm]"
            }
        },
        {
            name: "UmRel_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa") + " [%]"
            },
            max: 105
        }
    ];

    var seriesMeteo = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "Temperatura",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
            type: "column",
            field: "Pioggia",
            gap: 0,
            spacing: 0,
            axis: "Precipitazioni_Axis",
            color: "rgb(0, 128, 255)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa"),
            field: "UmiditaRelativa",
            axis: "UmRel_Axis",
            color: "rgb(192, 192, 0)",
            tooltipTemplate: "0.00"
        }
    ];

    creaKendoChart("", "Data", vertAxesMeteo, seriesMeteo, grid_ds);

}


function creaOutput_RaccaMicotox(kendodata) {

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var vertAxes = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "LAIRelativoIndiceDiRischio", "LAI (rel.) - Indice di rischio")
            }
        },
        {
            name: "UmGranella_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàGranella", "Umidità granella") + " (%)"
            }
        }
    ];

    var series = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "LAIRelativo", "LAI (rel.)"),
            color: "rgb(0, 176, 80)",
            field: "LAI"
        },
        {
            type: "area",
            line: { style: "smooth" },
            name: TraduzioneMultiResx(datiMeteoResx, "MassimoIndiceDiRischioCinqueGiorni", "Massimo indice di rischio (5 giorni)"),
            color: "rgb(192, 80, 77)",
            field: "Max5gg"
        },
        {
            type: "area",
            line: { style: "smooth" },
            name: TraduzioneMultiResx(datiMeteoResx, "MediaIndiceDiRischioCinqueGiorni", "Media indice di rischio (5 giorni)"),
            color: "rgb(255, 192, 0)",
            field: "Media5gg"//,
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàGranella", "Umidità granella") + " R2-R6",
            width: 1,
            color: "rgb(47, 79, 79)",
            dashType: "longDash",
            field: "UmGranella",
            axis: "UmGranella_Axis"
        }
    ];

    creaKendoChart("", "Data", vertAxes, series, grid_ds);

    var vertAxesMeteo = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            }
        },
        {
            name: "Precipitazioni_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni") + " [mm]"
            }
        },
        {
            name: "UmRel_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa") + " [%]"
            },
            max: 105
        }
    ];

    var seriesMeteo = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "Temperatura",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
            type: "column",
            field: "Pioggia",
            gap: 0,
            spacing: 0,
            axis: "Precipitazioni_Axis",
            color: "rgb(0, 128, 255)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa"),
            field: "UmiditaRelativa",
            axis: "UmRel_Axis",
            color: "rgb(192, 192, 0)",
            tooltipTemplate: "0.00"
        }
    ];

    creaKendoChart("", "Data", vertAxesMeteo, seriesMeteo, grid_ds);
}


function creaOutput_Agronomica_30__MRV(kendodata) {

    let chartDiv = creaChartDiv();

    kendodata.group_columns = { css_group: "groupheader" };

    let divKendoGrid = kendoGrid_Inizializza(kendodata);

    let kendogrid = $("#" + divKendoGrid).data("kendoGrid");

    let Columns = new Array;
    let StadiDict = {};
    let BarsFields = {};
    let Bars = new Array;

    let def_bar = {
        type: "column",
        color: "none",
        stack: true,
        gap: 0,
        spacing: 0,
        visibleInLegend: false,
        //visual: visualColumnSeries
        visual: function (e) {
            if (e.value == null)
                return;

            let color = e.dataItem[e.series.colorField];
            if (color == "" || color == undefined)
                return;

            let r1 = e.rect;
            let path2 = null;
            let color2 = e.dataItem[e.series.colorField2];
            if (color2 !== undefined) {

                let r2 = r1.clone();

                let h2 = r1.height() / 2;
                r2.origin.y += h2;
                r2.size.height -= h2
                r1.size.height = h2;

                path2 = new kendo.drawing.Path.fromRect(r2,
                    {
                        fill: { color: color2 },
                        stroke: { color: color2 }
                    }
                );
            }

            let path = new kendo.drawing.Path.fromRect(r1,
                {
                    fill: { color: color },
                    stroke: { color: color }
                }
            );

            let group = new kendo.drawing.Group().append(path);

            if (path2 !== null) {
                group.append(path2);
            }

            return group;
        }
    };
    let def_clrs = [{ r: 255, g: 215, b: 0 }, { r: 255, g: 69, b: 0 }, { r: 220, g: 20, b: 60 }, { r: 139, g: 0, b: 0 }];

    let rgb_grad = function (rgb, f) {

        let r = 220 * (1 - f) + rgb.r * f;
        let g = 220 * (1 - f) + rgb.g * f;
        let b = 220 * (1 - f) + rgb.b * f;
        //let hexclr = "#";
        //hexclr += ('00' + Math.round(r).toString(16).toUpperCase()).slice(-2);
        //hexclr += ('00' + Math.round(g).toString(16).toUpperCase()).slice(-2);
        //hexclr += ('00' + Math.round(b).toString(16).toUpperCase()).slice(-2);

        return "rgb(" + r.toFixed() + ", " + g.toFixed() + ", " + b.toFixed() + ")";
    };

    let bar_ht = 20;

    $.each(kendogrid.columns, function (i_g, group) {

        if (group.columns === undefined) {

            Columns.push({
                field: group.field,
                add: true
            });

        } else {

            $.each(group.columns, function (i_c, col) {

                let stadio = "";
                let color = "";
                let start = col.field.toUpperCase().indexOf("STADIO");
                if (start >= 0) {

                    stadio = col.field.substring(start);

                    if (!StadiDict.hasOwnProperty(stadio)) {

                        let cnt = Object.keys(StadiDict).length;

                        StadiDict[stadio] = { name: col.title, color: def_clrs[cnt % def_clrs.length], index: cnt };

                        BarsFields["vuoto_" + cnt] = bar_ht * (cnt === 0 ? 0.25 : 0.5);
                        BarsFields[stadio] = bar_ht;

                        Bars.push($.extend({ field: "vuoto_" + cnt }, def_bar));

                        let ttt = "<div>";
                        ttt += "<div style='text-align:center; margin-bottom:6px;'>#: kendo.toString(category, 'dd/MM/yyyy') # - " + col.title + "</div>";
                        ttt += "<div style='display:grid; grid-template-columns: auto auto; gap: 3px 3px;'>"
                        ttt += "<div>#: dataItem['" + stadio + "_Gen']#:</div>";
                        ttt += "<div style='justify-self:end;'>#: kendo.toString(dataItem['" + stadio + "_Val'], '0.0')#%</div>";
                        ttt += "#if (dataItem['" + stadio + "_Gen2']) {#";
                        ttt += "<div>#: dataItem['" + stadio + "_Gen2']#:</div>";
                        ttt += "<div style='justify-self:end;'>#: kendo.toString(dataItem['" + stadio + "_Val2'], '0.0')#%</div>";
                        ttt += "#}#";
                        ttt += "</div>";
                        ttt += "</div>";

                        Bars.push($.extend({ field: stadio, colorField: stadio + "_Clr", colorField2: stadio + "_Clr2", tooltipTemplate: { template: ttt } }, def_bar));
                    }

                    color = StadiDict[stadio].color;
                }

                Columns.push({
                    gen: group.title,
                    stadio: stadio,
                    color: color,
                    field: col.field,
                    add: false
                });

            });
        }
    });

    let grid_ds = kendogrid.dataSource.data();
    let chart_ds = new Array;

    $.each(grid_ds, function (i, grid_elem) {

        let chart_elem = new Object();
        let push = false;

        $.each(Columns, function (c, col) {

            if (col.add) {

                chart_elem[col.field] = grid_elem[col.field];

            } else {

                let grid_val = grid_elem[col.field];
                if (grid_val) {

                    if (col.stadio !== "") {

                        if (chart_elem.hasOwnProperty(col.stadio + "_Clr")) {

                            //Sovrapposizione stesso stadio per generazioni diverse...
                            chart_elem[col.stadio + "_Clr2"] = rgb_grad(col.color, grid_val / 100.0);
                            chart_elem[col.stadio + "_Gen2"] = col.gen;
                            chart_elem[col.stadio + "_Val2"] = grid_val;

                        } else {

                            chart_elem[col.stadio + "_Clr"] = rgb_grad(col.color, grid_val / 100.0);
                            chart_elem[col.stadio + "_Gen"] = col.gen;
                            chart_elem[col.stadio + "_Val"] = grid_val;
                        }
                    }

                    push = true;
                }
            }
        });

        if (push) {
            chart_ds.push($.extend(chart_elem, BarsFields));
        }
    });

    let lblTemplate = "#var band = Math.trunc(value / " + (bar_ht * 1.5) + "); # " +
        "#var lbl = ''; # " +
        "#switch (band) { ";
    $.each(Object.values(StadiDict), function (i, s) {
        lblTemplate += "case " + s.index + ": lbl = '" + s.name + "'; break; ";
    });
    lblTemplate += "}# " +
        "#=lbl#";

    let axis = {
        //title: { text: "Uova Larve Pupe Adulti" },
        //name: "dynamic_zoom",
        //reverse: true,
        max: Object.keys(StadiDict).length * bar_ht * 1.5,
        majorUnit: bar_ht * 0.75,
        majorGridLines: { skip: 2, step: 2, dashType: "dash" },
        majorTicks: { skip: 2, step: 2, visible: false },
        labels: {
            skip: 1,
            step: 2,
            template: kendo.template(lblTemplate),
            font: "16px Arial, Helvetica, sans-serif",
            rotation: {
                angle: 270
            }
        }
    };

    creaKendoChart(TraduzioneMultiResx(datiMeteoResx, "SviluppoFenologico", "Sviluppo fenologico"), "DataOra", axis, Bars, chart_ds, chartDiv);
}


function creaOutput_Agronomica_30__MISP_IPI_Pomodoro(kendodata, indice) {

    if (indice === 1) {

        //let chartDiv = creaChartDiv();

        //kendoGrid_Inizializza(kendodata);

        let ipiTemplate = "<div>";
        ipiTemplate += "<div>#: kendo.toString(category, 'dd/MM/yyyy') #</div>";
        ipiTemplate += "<div style='margin-top:5px;'>" + TraduzioneMultiResx(datiMeteoResx, "IPISetteGiorni_", "IPI 7gg:") + " "
            + "#: kendo.toString(dataItem.IPI_7_gg, '0.00')#</div>";
        ipiTemplate += "</div>";

        let evTemplate = "<div>";
        evTemplate += "<div>#: kendo.toString(category, 'dd/MM/yyyy') #</div>";
        evTemplate += "<div style='margin-top:5px;'>" + TraduzioneMultiResx(datiMeteoResx, "EventoInfettivoNumero_", "Evento infettivo n.") + " "
            + "#: kendo.toString(dataItem.NumInfez, '0')#</div>";
        evTemplate += "</div>";

        var vertAxes = [
            { title: { text: TraduzioneMultiResx(datiMeteoResx, "IndiceSetteGiorniConMISP", "Indice 7 gg e MISP") } },
            { title: { text: TraduzioneMultiResx(datiMeteoResx, "PioggiaMM", "Pioggia (mm)") }, name: "P_Axis" },
            { name: "Infez_Axis", max: 1.03, visible: false }
        ];

        var series = [
            {
                name: TraduzioneMultiResx(datiMeteoResx, "IndiceSetteGiorni", "Indice 7 gg "),
                color: "rgb(25, 25, 112)",
                field: "IPI_7_gg",
                markers: {
                    visible: true,
                    background: "#CCC",
                    visual: function (e) {
                        let circleGeometry = new kendo.geometry.Circle(e.rect.center(), 4);
                        let circle = new kendo.drawing.Circle(circleGeometry, {
                            fill: { color: e.dataItem.IPI_7_gg_ind },
                            stroke: { color: e.dataItem.IPI_7_gg_ind }
                        });
                        return circle;
                    }
                },
                //tooltip: { visible: true, border: { color: "#808080" }, background: "#808080", color: "#FFF", template: ipiTemplate }
                tooltip: { visible: true, template: ipiTemplate }
            },
            {
                type: "column",
                name: TraduzioneMultiResx(datiMeteoResx, "PioggiaMM", "Pioggia (mm)"),
                color: "rgb(0, 128, 255)",
                gap: 0,
                spacing: 0,
                field: "Pioggia",
                axis: "P_Axis",
                tooltipTemplate: "0.00"
            },
            {
                name: TraduzioneMultiResx(datiMeteoResx, "EventoInfettivo", "Evento infettivo"),
                color: "rgb(220, 20, 60)",
                field: "Infez",
                width: 0,
                markers: {
                    visible: true,
                    //background: "rgb(255, 0, 0)" },
                    type: "square",
                    rotation: 45
                },
                highlight: {
                    visible: true
                },
                axis: "Infez_Axis",
                tooltip: { visible: true, template: evTemplate }
                //tooltipTemplate: { template: evTemplate }
            }
        ];

        creaKendoChart("", "Data", vertAxes, series, kendodata.kendo_rows);

    } else if (indice === 2) {

        //MISP

        //let chartDiv = creaChartDiv();
        //let chart_ds = [];
        //let series = [];

        //let num = 0;
        //let r = 0;
        //while (r < kendodata.kendo_rows.length) {

        //    if (kendodata.kendo_rows[r]["NumInf"] !== num) {

        //        num = kendodata.kendo_rows[r]["NumInf"];
        //        r++;

        //        series.push({
        //            name: "Infezione " + num,
        //            field: "Valore_" + num
        //        });

        //    } else {

        //        let elem = { DataOra: kendo.parseDate(kendodata.kendo_rows[r]["DataOra"]) };
        //        elem["Valore_" + num] = kendodata.kendo_rows[r]["Valore"];

        //        if (chart_ds.length === 0) {
        //            //vuoto...
        //            chart_ds.push(elem);
        //        } else {

        //            let t0 = elem.DataOra.getTime();
        //            let t1 = chart_ds[chart_ds.length - 1].DataOra.getTime();
        //            if (t0 > t1) {
        //                //maggiore dell'ultimo...
        //                chart_ds.push(elem);
        //            } else {

        //                t1 = chart_ds[0].DataOra.getTime();
        //                if (t0 < t1) {
        //                    //minore del primo...
        //                    chart_ds.splice(0, 0, elem);
        //                } else {

        //                    let start = 0;
        //                    let end = chart_ds.length;
        //                    let mid = Math.floor((start + end) / 2);
        //                    let found = false;
        //                    while (!found && end > start) {
        //                        t1 = chart_ds[mid].DataOra.getTime();
        //                        if (t0 < t1) {
        //                            end = mid;
        //                        } else {
        //                            if (t0 > t1) {
        //                                start = mid + 1;
        //                            } else {
        //                                chart_ds[mid] = $.extend(chart_ds[mid], elem);
        //                                found = true;
        //                            }
        //                        }
        //                        mid = Math.floor((start + end) / 2);
        //                    }

        //                    if (!found) {
        //                        //inserisco l'elemento...
        //                        chart_ds.splice(mid, 0, elem);
        //                    }
        //                }
        //            }
        //        }

        //        kendodata.kendo_rows.splice(r, 1);
        //    }
        //}

        kendodata.group_columns = { css_group: "groupheader" };

        kendoGrid_Inizializza(kendodata);

        //creaKendoChart("Main Infection and Sporulation Period", { field: "DataOra", baseUnit: "hours" }, "", series, chart_ds, chartDiv);
    }
}


function creaOutput_UniCatt__AFLA_Mais(kendodata) {

    creaOutput_UniCatt_Mais(kendodata);
}


function creaOutput_UniCatt__FER_Mais(kendodata) {

    creaOutput_UniCatt_Mais(kendodata);
}


function creaOutput_UniCatt_Mais(kendodata) {

    let divKendoGrid = kendoGrid_Inizializza(kendodata);

    let grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    let vertAxes = [
        {
            title: {
                text: ""
            }
        }
    ];

    if (typeof kendodata.plotBands === "object") {
        vertAxes = {
            axes: vertAxes,
            bands: kendodata.plotBands
        };
    }

    let series = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UniCatt_Mais_Probability", "Probabilità"),
            color: "rgb(100, 149, 237)",
            field: "Probability"
        }
    ];

    creaKendoChart("", "Data", vertAxes, series, grid_ds);



    let vertAxesMeteo = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            }
        },
        {
            name: "UmRel_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa") + " [%]"
            },
            max: 105
        },
        {
            name: "Precipitazioni_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni") + " [mm]"
            }
        }
    ];

    let seriesMeteo = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "Temperatura",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
            type: "column",
            field: "Precipitazioni",
            gap: 0,
            spacing: 0,
            axis: "Precipitazioni_Axis",
            color: "rgb(0, 128, 255)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa"),
            field: "UmiditaRelativa",
            axis: "UmRel_Axis",
            color: "rgb(192, 192, 0)",
            tooltipTemplate: "0.00"
        }
    ];

    creaKendoChart("", "Data", vertAxesMeteo, seriesMeteo, grid_ds);
}


function creaOutput_UniCatt__Fusariosi_Frumento(kendodata) {

    let divKendoGrid = kendoGrid_Inizializza(kendodata);

    let grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    let series = [
        {
            name: "TOX Fg",
            color: "rgb(255, 102, 0)",
            field: "TOX_Fg"
        },
        {
            name: "TOX Fc",
            color: "rgb(153, 204, 0)",
            field: "TOX_Fc"
        },
        {
            name: "TOX Tot",
            color: "rgb(0, 0, 0)",
            dashType: "dash",
            field: "TOX_Tot"
        },
    ];

    let vertAxes = [
        {
            title: {
                text: "FHB-Tox"
            },
            //plotBands: [
            //    {
            //        from: 0.07,
            //        to: 0.35,
            //        color: "#008000",
            //        opacity: 0.2
            //    },
            //    {
            //        from: 0.35,
            //        to: 0.7,
            //        color: "#FFF000",
            //        opacity: 0.2
            //    },
            //    {
            //        from: 0.7,
            //        to: 1.5,
            //        color: "#F00000",
            //        opacity: 0.2
            //    }
            //],
        }
    ]

    creaKendoChart("", "Data", vertAxes, series, grid_ds);


    let vertAxesMeteo = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            }
        },
        {
            name: "UmRel_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa") + " [%]"
            },
            min: 0,
            max: 105
        },
        {
            name: "Precipitazioni_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni") + " [mm]"
            }
        },
        {
            name: "LW_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "BagnaturaFogliare", "Bagnatura fogliare") + " [h]"
            },
            min: 0,
            max: 25
        }
    ];

    let seriesMeteo = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "T",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
            type: "column",
            field: "R",
            gap: 0,
            spacing: 0,
            axis: "Precipitazioni_Axis",
            color: "rgb(0, 128, 255)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa"),
            field: "UR",
            axis: "UmRel_Axis",
            color: "rgb(192, 192, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "BagnaturaFogliare", "Bagnatura fogliare"),
            field: "LW",
            type: "area",
            line: {
                style: "smooth"
            },
            axis: "LW_Axis",
            color: "rgb(204, 230, 152)",
            tooltipTemplate: "0"
        }
    ];

    creaKendoChart("", "Data", vertAxesMeteo, seriesMeteo, grid_ds);
}


function creaOutput_Agronomica_30__ColpoDiFuoco(kendodata) {

    let divChart_Caso1 = creaChartDiv();
    let divChart_Caso2 = creaChartDiv();
    let divChart_Caso3 = creaChartDiv();

    let col = "";
    let icol = 0
    while (col === "" && icol < kendodata.kendo_columns.length) {
        if (kendodata.kendo_columns[icol].field.startsWith("TRV")) {
            col = kendodata.kendo_columns[icol].field;
        }
        icol++;
    }

    let tooltip = [];
    if (col !== "") {
        let hh = col.replace("TRV_", "");
        tooltip.push({
            column: col,
            content: "<div>Indice di rischio: Somma mobile " + hh + " ore dell'indice TRV (Temperature Risk Value)</div>"
        });
    }

    tooltip.push({
        column: "Rischio1",
        content: "<div>Caso 1: Colpo di fuoco non presente nel frutteto l'anno precedente</div>"
    });
    tooltip.push({
        column: "Rischio2",
        content: "<div>Caso 2: Colpo di fuoco presente nel frutteto l'anno precedente</div>"
    });
    tooltip.push({
        column: "Rischio3",
        content: "<div>Caso 3: Colpo di fuoco attivo</div>"
    });

    kendodata.tooltip = tooltip

    let divGrid = kendoGrid_Inizializza(kendodata);

    let grid_ds = $("#" + divGrid).data("kendoGrid").dataSource;

    $("#" + divGrid + " .k-grid-content").find("tr").each(function (i, el) {
        let uid = $(el).attr("data-uid");
        let obj = grid_ds.getByUid(uid);
        if (obj !== undefined) {
            if (obj.Prec > 0) {
                $(el).css("background-color", "rgba(255, 255, 0, 0.3)");
            }
        }
    });

    let field_chart = [
        {
            field: "Rischio1",
            title: "Caso 1. Colpo di fuoco non presente l'anno predecente",
            div: divChart_Caso1
        },
        {
            field: "Rischio2",
            title: "Caso 2. Colpo di fuoco presente l'anno predecente",
            div: divChart_Caso2
        },
        {
            field: "Rischio3",
            title: "Caso 3. Colpo di fuoco attivo",
            div: divChart_Caso3
        }
    ];

    $.each(field_chart, function (i, elem) {

        let vert_axes = [
            {
                title: {
                    text: "Indice di rischio TRV"
                }
            },
            {
                title: {
                    text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni")
                },
                name: "Prec_Axis"
            }
        ];

        let series = [
            {
                name: "Indice di rischio TRV",
                field: elem.field,
                color: "rgb(169, 169, 169)",
                tooltipTemplate: "0.00"
            },
            {
                name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
                field: "Prec",
                type: "column",
                gap: 0,
                spacing: 0,
                color: "rgb(0, 128, 255)",
                axis: "Prec_Axis",
                tooltipTemplate: "0.00"
            }
        ];

        if (typeof kendodata.plotBands === "object") {
            let idx = 0;
            let bands = null;
            while (idx < kendodata.plotBands.length && bands === null) {
                if (kendodata.plotBands[idx].Field === elem.field) {
                    bands = kendodata.plotBands[idx];
                }
                idx++;
            }

            if (bands !== null) {
                vert_axes = {
                    axes: vert_axes,
                    bands: bands
                };
            }
        }

        creaKendoChart(elem.title, { field: "DataOra", baseUnit: "hours" }, vert_axes, series, grid_ds.data(), elem.div);
    });
}


function creaOutput_BetaCoProB__Cercosporiosi(kendodata) {

    let chartDiv = creaChartDiv();

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var vertAxes = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio")
            }
        },
        {
            name: "A_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "AndamentoStagionale", "Andamento stagionale")
            }
        }
    ];

    var series = [
        {
            type: "column",
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio"),
            color: "#D3D3D3",
            gap: 0,
            spacing: 0,
            field: "Inf_2GG",
            colorField: "Inf_2GG_IND",
            visual: visualColumnSeries
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "AndamentoStagionale", "Andamento stagionale"),
            color: "#4169E1",
            field: "IG_Cum",
            axis: "A_Axis"
        }
    ];

    creaKendoChart("", "Data", vertAxes, series, grid_ds, chartDiv);

}


function creaOutput_RaccaFrumento(kendodata) {

    var divKendoGrid = kendoGrid_Inizializza(kendodata);

    var grid_ds = $("#" + divKendoGrid).data("kendoGrid").dataSource.data();

    var vertAxes = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "LAIRelativoIndiceDiRischio", "LAI (rel.) - Indice di rischio")
            }
        },
        {
            name: "IndiceRischioMediato_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischioMediatoSullaLatenza", "Indice di rischio mediato sulla latenza")
            }
        }
    ];

    var series = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "LAIRelativo", "LAI (rel.)"),
            color: "rgb(0, 176, 80)",
            field: "LAI"
        },
        {
            type: "column",
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischio", "Indice di rischio"),
            color: "rgb(255, 192, 0)",
            gap: 0,
            spacing: 0,
            field: "IndiceRischio"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "IndiceDiRischioMediatoSullaLatenza", "Indice di rischio mediato sulla latenza"),
            color: "rgb(255, 0, 0)",
            field: "IndiceRischioMediato",
            axis: "IndiceRischioMediato_Axis"
        }
    ];

    if (typeof kendodata.plotBands === "object") {
        vertAxes = {
            axes: vertAxes,
            bands: kendodata.plotBands
        };
    }

    creaKendoChart("", "Data", vertAxes, series, grid_ds);

    var vertAxesMeteo = [
        {
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura") + " [°C]"
            }
        },
        {
            name: "Precipitazioni_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni") + " [mm]"
            }
        },
        {
            name: "UmRel_Axis",
            title: {
                text: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa") + " [%]"
            },
            max: 105
        }
    ];

    var seriesMeteo = [
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Temperatura", "Temperatura"),
            field: "Temperatura",
            color: "rgb(192, 0, 0)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "Precipitazioni", "Precipitazioni"),
            type: "column",
            field: "Pioggia",
            gap: 0,
            spacing: 0,
            axis: "Precipitazioni_Axis",
            color: "rgb(0, 128, 255)",
            tooltipTemplate: "0.00"
        },
        {
            name: TraduzioneMultiResx(datiMeteoResx, "UmiditàRelativa", "Umidità relativa"),
            field: "UmiditaRelativa",
            axis: "UmRel_Axis",
            color: "rgb(192, 192, 0)",
            tooltipTemplate: "0.00"
        }
    ];

    creaKendoChart("", "Data", vertAxesMeteo, seriesMeteo, grid_ds);
}

function LeggiDSSForecast() {
    let risp = null;
    let params = {
        piva: $(cIdPiva).val()
    }
    ajaxAgronicaSync("DSS_Difesa.aspx/LeggiDSSForecast",
        kendo.stringify(params), false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        },
        function (risposta) {
            console.log("ERRORE DSS Difesa: lettura impostazione impresa SUPERUSER_ELABORAZIONE_DSS_FORECAST: " + risposta.Errore);
        });
    return risp;
}

