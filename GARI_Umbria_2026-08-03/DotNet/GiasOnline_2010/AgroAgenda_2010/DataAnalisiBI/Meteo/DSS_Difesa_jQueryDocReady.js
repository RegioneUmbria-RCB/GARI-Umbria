var Enum_ModelloPrevisionale = {
    Ticchiolatura_AScab: 14,
    RitardoVariabile: 10,
    Agronomica_30__Peronospora_della_Vite: 16,
    Agronomica_30__BatteriosiKiwi_PSA: 17,
    Racca__PeroPom: 18,
    Agronomica_30__Oidio_della_Vite: 19,
    Agronomica_30__Botrite_della_Vite: 20,
    Agronomica_30__Ticchiolatura_del_Melo: 21,
    Racca__AlterPom: 22,
    Racca__OidioPom: 23,
    Racca__BotriPom: 24,
    Racca__PeroBiet: 25,
    Racca__OidioBiet: 26,
    Racca__CercoBiet: 27,
    Racca__PeroPat: 28,
    Racca__AlterPat: 29,
    Racca__ScleroSoia: 30,
    Racca__BrusoneRiso: 31,
    Agronomica_30__MRV_Eulia: 32,
    Agronomica_30__MRV_CydiaMolesta: 33,
    Agronomica_30__MRV_Carpocapsa: 34,
    Agronomica_30__MRV_Helicoverpa: 35,
    Agronomica_30__MRV_Tignoletta: 36,
    Agronomica_30__Maculatura_del_Pero: 37,
    Agronomica_30__MRV_MoscaOlivo: 38,
    Racca__ElmintosporiosiMais: 39,
    Racca__BipolarisMaidis: 40,
    Racca__AntracnosiOlivo: 41,
    Agronomica_30__MISP_IPI_Pomodoro: 42,
    Racca__MicotoxMais: 43,
    UniCatt__AFLA_Mais: 44,
    UniCatt__FER_Mais: 45,
    Agronomica_30__Colpo_di_fuoco: 46,
    BetaCoProB__Cercosporiosi: 47,
    UniCatt__Fusariosi_Frumento: 48,
    MRV_PiralideMais: 49,
    MRV_NottuaMais: 50,
    RaccaFrumento_RuggineBruna: 51,
    RaccaFrumento_RuggineGialla: 52,
    RaccaFrumento_RuggineNera: 53,
    RaccaFrumento_Stagonosporiosi: 54,
    RaccaFrumento_Septoria: 55,
    RaccaFrumento_Fusariosi: 56,
    RaccaFrumento_FusariosiSpiga: 57,
    RaccaFrumento_Fusariosi2: 58,
    RaccaFrumento_Fusariosi3: 59,
    RaccaFrumento_MarciumeRosa: 60
}

$(document).ready(function () {

    if (!Array.isArray(datiMeteoResx)) {
        datiMeteoResx = [];
    }
    datiMeteoResx.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "DSS_Difesa_jQueryDocReady.js"));

    if ($(".AgronicaFooter").is(":visible")) {
        let ftr_h = $(".AgronicaFooter").outerHeight();
        ftr_h = Math.ceil(ftr_h / 10) * 10;
        $("#id_MainContainer").css("margin-bottom", ftr_h + "px");
    }

    WaitFrame.show();

    GlobalMeteoTabstrip = new MeteoTabstrip("#tabstrip");

    GlobalMeteoSourceSelector = new MeteoSourceSelector($("#geo-pos-edit"), $("#cmbTipoSorgente"), $("#cmbOrigineDati"), url_meteo_ws);


    let lat = Request_QueryString("lat");
    let lng = Request_QueryString("lng");

    GlobalMeteoSourceSelector.setLatLng(lat, lng);

    $("#elab-periodo").elabPeriodo({
        changePeriod: function (start_doy, end_doy, year) {

            $("#main-grid").find(".param-modello").each(function () {

                $(this).find(":input").each(function () {

                    let doy_picker = $(this).data("kendoDoY_DatePicker");

                    if (doy_picker) {

                        doy_picker.setRange(start_doy, end_doy, year);
                    }
                });
            });
        },
        dayForecast: parseInt(LeggiDSSForecast())
    });

    let configModelli = document.getElementById("config-modelli");

    if (configModelli !== null) {

        let configBtn = document.createElement("div");
        configBtn.style.padding = "0px";
        configBtn.style.fontSize = "18px";
        configBtn.style.color = "#3f51b5";
        configBtn.style.height = "100%";
        configBtn.style.width = "3em"
        configModelli.appendChild(configBtn);

        $(configBtn).kendoButton({
            iconClass: "fa fa-calendar fa-fw",
            click: configurazioneModelli
        });
    }

    GestisciParametriModelli("main-grid");

    $("#cmbSpecieVegetale").kendoDropDownList({
        autoBind: false,
        autoWidth: true,
        value: vegCod ?? 0,
        dataTextField: "Veg_Des",
        dataValueField: "Veg_Cod",
        dataSource: {
            transport: {
                read: function (options) {

                    let data = [];

                    ajaxAgronicaSync(url_meteo_ws + "/LeggiModelliAutorizzati",
                        JSON.stringify({ Veg_Cod: vegCod ?? 0 }),
                        false,
                        function (risposta) {
                            data = JSON.parse(risposta.RispostaStringa);
                        },
                        function (risposta) {
                            meteoAlert(
                                TraduzioneMultiResx(datiMeteoResx, "ElaborazioneModelloPrevisionale", "Elaborazione modello previsionale"),
                                TraduzioneMultiResx(datiMeteoResx, "NoDataFoundAuthorizedModels", "Nessun dato trovato")
                            );
                            console.error(TraduzioneMultiResx(datiMeteoResx, "NoDataFoundAuthorizedModels", "Nessun dato trovato"));
                        },
                        null,
                        false
                    );

                    options.success(data);
                }
            }
        },
        change: function (e) {
            $("#cmbAvModAlg").getKendoDropDownList().dataSource.read();
        }
    });

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

    $("#cmbAvModAlg").kendoDropDownList({
        autoBind: false,
        autoWidth: true,
        dataValueField: "Mod_Cod",
        dataTextField: "Full_Des",
        valueTemplate: avvValueTmplt,
        template: avvTmplt,
        dataSource: {
            transport: {
                read: function (options) {
                    let data = [];
                    let vegItem = $("#cmbSpecieVegetale").getKendoDropDownList().dataItem();
                    if (vegItem) {
                        data = vegItem.Modelli;
                    }
                    options.success(data);
                }
            }
        },
        dataBound: function (e) {
            e.sender.select(0);
            e.sender.trigger("change");
        },
        change: function (e) {

            GestisciParametri("main-grid", e.sender.value());

            let modello = e.sender.dataItem();

            let range = { start: 1, end: 365 };
            if (modello) {
                range.start = modello.InizioPeriodo_gg;
                range.end = modello.FinePeriodo_gg;
            }

            elabPeriodoPlugin().setRange(range);
        }
    });

    let ddlSpecie = $("#cmbSpecieVegetale").getKendoDropDownList();
    if (ddlSpecie) {

        ddlSpecie.dataSource.read();
        ddlSpecie.select(0);
        ddlSpecie.trigger("change");
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    $("#btn_Modelli_Previsionali").click(function () {
        elaboraModello();
    });

    const cookieUseDateRangeName = `DSS.Indicatori.Periodo.${$(cIdPiva).val()}.UseDateRange`;
    const useDateRange = JSON.parse($.cookie(cookieUseDateRangeName) ?? 'false');
    const dateRange = useDateRange;

    $("#ID_Gauges").DSS_Summary({
        url_meteoWS: url_meteo_ws,
        piva: $(cIdPiva).val(),
        rag_soc: $(Rag_Soc).val(),
        summary: false,
        msg_summary: TraduzioneMultiResx(datiMeteoResx, "RiepilogoIndicatoriEmergenzeDSS", "Riepilogo indicatori emergenze DSS"),
        window_title: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSS", "Indicatori emergenze DSS"),
        msg_wait: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSInAttesaDiElaborazione", "Indicatori emergenze DSS - In attesa di elaborazione..."),
        msg_na: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSNonDisponibili", "Indicatori emergenze DSS non disponibili"),
        msg_err: TraduzioneMultiResx(datiMeteoResx, "IndicatoriEmergenzeDSSErroreInElaborazione", "Indicatori emergenze DSS - Errore in elaborazione!"),
        fun_callback: function (status) {

            let tabstrip = GlobalMeteoTabstrip.kendoTabstrip();

            if (status <= 0) {

                if (tabstrip.select().index() === 0) {
                    tabstrip.select(1);
                }

                $("#ID_Gauges").remove();
                tabstrip.remove(0);

            } else {

                $(tabstrip.items()[0]).find(".tab-loader").remove();
            }
        },
        gauge_click_callback: gauge_click,
        wait_callback: function () {

            let tabstrip = GlobalMeteoTabstrip.kendoTabstrip();
            let tabs = tabstrip.items();

            if (tabs.length > 0 && tabs[0].id === "Indicatori") {

                if ($(tabs[0]).find(".tab-loader").length === 0) {

                    let loader_container = document.createElement("div");
                    loader_container.className = "tab-loader-container"

                    let loader = document.createElement("div");
                    loader.className = "tab-loader";

                    loader_container.appendChild(loader)

                    tabs[0].appendChild(loader_container);

                    tabstrip.select(1);
                }
            }
        },
        date_range: dateRange
    });

    GlobalMeteoTabstrip.ready();

    WaitFrame.hide();

    $("#divKendoOut").meteoOutput();

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //Impostazioni default
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    let strParams = null;
    if ($(cIdExternalLoad).val().toLowerCase() === "true") {

        let parametri = $(cIdParametri).val();
        if (parametri !== null && parametri !== undefined && parametri != "")
            strParams = parametri;
    }
    else {
        strParams = Request_QueryString("Parametri");
    }

    if (strParams !== null) {
        if (gauge_click(JSON.parse(strParams))) {

            return;
        }
    }

    let dati = getMeteoStorage();

    if (dati) {
        //Non imposto DataInizio e DataFine, possono essere diversi da quelli richiesti dal modello
        GlobalMeteoSourceSelector.setSorgente(dati.TipoSorgente, dati.Sorgente);
    } else {
        GlobalMeteoSourceSelector.setDefault($(cIdPiva).val());
    }
});

function gauge_click(params) {
    let trigger_click = true;

    if (params.Veg_Cod != undefined) {
        let specie_ddl = $("#cmbSpecieVegetale").getKendoDropDownList();
        specie_ddl.value(params.Veg_Cod);
        specie_ddl.trigger("change");
    } else {
        trigger_click = false;
    }

    if (params.Mod_Cod != undefined) {
        let modello_ddl = $("#cmbAvModAlg").getKendoDropDownList();
        modello_ddl.value(params.Mod_Cod);
        let mod_cod = parseInt(modello_ddl.value());

        if (mod_cod === params.Mod_Cod) {
            GestisciParametri("main-grid", params.Mod_Cod, params.ParametriElaborazione);
        } else {
            modello_ddl.select(0);
            modello_ddl.trigger("change");

            trigger_click = false;
        }

    } else {
        trigger_click = false;
    }

    if (params.DataInizio != undefined && params.DataFine != undefined) {
        elabPeriodoPlugin().setRange({ start: new Date(params.DataInizio), end: new Date(params.DataFine) });
    } else {
        trigger_click = false;
    }

    if (!trigger_click) {
        return false;
    }

    GlobalMeteoTabstrip.kendoTabstrip().select("#Ricerca_Intestazione");

    setTimeout(function () {
        if (params.Tipo_Sorgente !== undefined && params.Stazione_Cod !== undefined) {
            GlobalMeteoSourceSelector.setSorgente(params.Tipo_Sorgente, params.Stazione_Cod, function (tipo, sorgente) {
                if (tipo == params.Tipo_Sorgente && sorgente == params.Stazione_Cod) {
                    $("#btn_Modelli_Previsionali").trigger("click");
                }
            });
        }
    }, 100);

    return true;
}

function GestisciParametriModelli(id_grid) {

    let grid = document.getElementById(id_grid);

    if (grid == null) {
        return;
    }

    //Ticchiolatura_AScab: 14,
    //RitardoVariabile: 10,
    //Agronomica_30__Peronospora_della_Vite: 16,
    //Agronomica_30__BatteriosiKiwi_PSA: 17,
    //Racca__PeroPom: 18,
    //Agronomica_30__Oidio_della_Vite: 19,
    //Agronomica_30__Botrite_della_Vite: 20,
    //Agronomica_30__Ticchiolatura_del_Melo: 21,
    //Racca__AlterPom: 22,
    //Racca__OidioPom: 23,
    //Racca__BotriPom: 24,
    //Racca__PeroBiet: 25,
    //Racca__OidioBiet: 26,
    //Racca__CercoBiet: 27,
    //Racca__PeroPat: 28,
    //Racca__AlterPat: 29,

    //*****************************************************************************************
    //Racca__ScleroSoia: 30,
    //*****************************************************************************************

    createLabel("Varietà", "mod-30", grid);
    createInput("select", "Varieta30", "mod-30", grid);

    $("#Varieta30").kendoDropDownList({
        dataTextField: "text",
        dataSource: [
            { text: "Precoce" },
            { text: "Media" },
            { text: "Tardiva" }
        ],
        index: 1
    });

    let now = new Date();

    //*****************************************************************************************
    //Racca__BrusoneRiso: 31,
    //*****************************************************************************************

    createLabel("Data semina", "mod-31", grid);
    createInput("text", "DataSemina31", "mod-31", grid);

    $("#DataSemina31").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 0, 7)
    });
    $("#DataSemina31").attr("readonly", true);

    createLabel("Varietà", "mod-31", grid);
    createInput("select", "Varieta31", "mod-31", grid);

    $("#Varieta31").kendoDropDownList({
        dataTextField: "text",
        dataSource: [
            { text: "Precoce" },
            { text: "Media" },
            { text: "Tardiva" }
        ],
        index: 1
    });

    //Agronomica_30__MRV_Eulia: 32,
    //Agronomica_30__MRV_CydiaMolesta: 33,
    //Agronomica_30__MRV_Carpocapsa: 34,
    //Agronomica_30__MRV_Helicoverpa: 35,
    //Agronomica_30__MRV_Tignoletta: 36,
    //Agronomica_30__Maculatura_del_Pero: 37,
    //Agronomica_30__MRV_MoscaOlivo: 38,
    //Racca__ElmintosporiosiMais: 39,
    //Racca__BipolarisMaidis: 40,
    //Racca__AntracnosiOlivo: 41,

    //*****************************************************************************************
    //Agronomica_30__MISP_IPI_Pomodoro: 42,
    //*****************************************************************************************

    createLabel("Data trapianto", "mod-42", grid);
    createInput("text", "DataTrapianto42", "mod-42", grid);

    $("#DataTrapianto42").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 3, 1)
    });
    $("#DataTrapianto42").attr("readonly", true);

    //Racca__MicotoxMais: 43,

    //*****************************************************************************************
    //UniCatt__AFLA_Mais: 44,
    //UniCatt__FER_Mais: 45,
    //*****************************************************************************************

    createLabel("Data emergenza", "mod-44 mod-45", grid);
    createInput("text", "DataEmergenza44_45", "mod-44 mod-45", grid);

    $("#DataEmergenza44_45").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 3, 15)
    });
    $("#DataEmergenza44_45").attr("readonly", true);

    //TODO salvo: inserire btn
    createLabel("", "mod-44 mod-45", grid);
    createBtn("btnElaboraTutteStazioni", "mod-44 mod-45", grid, "Elabora su tutte le stazioni meteo", evaluateOnAllStations);

    createLabel("", "mod-44 mod-45", grid);
    let divAttiva = document.createElement("div");
    divAttiva.className = "param-modello mod-44 mod-45 meteo-hidden";
    divAttiva.style.display = "grid";
    divAttiva.style.gridTemplateColumns = "1fr auto";
    divAttiva.style.alignItems = "center";
    divAttiva.style.border = "1px solid #ddd";
    divAttiva.style.borderRadius = "4px";
    divAttiva.style.padding = "2px 4px";
    grid.appendChild(divAttiva);

    let spanAttiva = document.createElement("span");
    spanAttiva.textContent = "Previsione presenza micotossine";
    spanAttiva.style.fontWeight = "bold";
    spanAttiva.style.color = "#9E9E9E";
    divAttiva.appendChild(spanAttiva);
    let switchAttiva = document.createElement("input");
    switchAttiva.type = "checkbox";
    switchAttiva.id = "Attiva44_45";
    divAttiva.appendChild(switchAttiva);

    $("#Attiva44_45").kendoSwitch({
        checked: true,
        messages: {
            checked: "",
            unchecked: ""
        },
        change: function (e) {
            if (e.checked) {
                $(".attivabile44_45").each(function () {
                    $(this).removeClass("dss-hidden");
                });
            } else {
                $(".attivabile44_45").each(function () {
                    $(this).addClass("dss-hidden");
                });
            }
        }
    });

    createLabel("Classe FAO", "mod-44 mod-45 attivabile44_45", grid);
    createInput("select", "Classe44_45", "mod-44 mod-45 attivabile44_45", grid);

    $("#Classe44_45").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [
            { text: "200-300", value: 1 },
            { text: "400", value: 2 },
            { text: "500", value: 3 },
            { text: "600-700", value: 4 }
        ],
        index: 0
    });

    createLabel("Coltura precedente", "mod-44 mod-45 attivabile44_45", grid);
    createInput("select", "Prev_Crop44_45", "mod-44 mod-45 attivabile44_45", grid);

    $("#Prev_Crop44_45").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [
            { text: "arable crops", value: 1 },
            { text: "small grain", value: 2 },
            { text: "maize", value: 3 }
        ],
        index: 0
    });

    createLabel("Semina/Raccolta", "mod-44 mod-45 attivabile44_45", grid);

    let semina_raccolta = document.createElement("div");
    semina_raccolta.className = "param-modello mod-44 mod-45 attivabile44_45 meteo-hidden";
    semina_raccolta.style.display = "grid";
    semina_raccolta.style.gridTemplateColumns = "1fr 1fr";
    semina_raccolta.style.gridGap = "10px";

    grid.appendChild(semina_raccolta);

    createInput("text", "DataSemina44_45", "mod-44 mod-45 attivabile44_45", semina_raccolta);

    $("#DataSemina44_45").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 3, 1)
    });
    $("#DataSemina44_45").attr("readonly", true);

    createInput("text", "DataRaccolta44_45", "mod-44 mod-45 attivabile44_45", semina_raccolta);

    $("#DataRaccolta44_45").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 7, 31)
    });
    $("#DataRaccolta44_45").attr("readonly", true);

    createLabel("Danno da piralide", "mod-44 mod-45 attivabile44_45", grid);
    createInput("select", "Piralide44_45", "mod-44 mod-45 attivabile44_45", grid);

    $("#Piralide44_45").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [
            { text: "No/Minor-damage", value: 1 },
            { text: "Medium damage", value: 2 },
            { text: "Severe damage", value: 3 }
        ],
        index: 0
    });

    createLabel("Umidità cariosside", "mod-44 mod-45 attivabile44_45", grid);
    createInput("text", "Cariosside44_45", "mod-44 mod-45 attivabile44_45", grid);

    $("#Cariosside44_45").kendoNumericTextBox({
        value: 20,
        min: 0,
        max: 100,
        decimals: 0,
        format: "0 \\%"
    });

    //Agronomica_30__Colpo_di_fuoco: 46,

    //*****************************************************************************************
    //BetaCoProB__Cercosporiosi: 47
    //*****************************************************************************************

    createLabel("Data partenza", "mod-47", grid);
    createInput("text", "DataPartenza47", "mod-47", grid);

    $("#DataPartenza47").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 4, 20)
    });
    $("#DataPartenza47").attr("readonly", true);

    //*****************************************************************************************
    //UniCatt__Fusariosi_Frumento: 48
    //*****************************************************************************************

    createLabel("Data spigatura", "mod-48", grid);
    createInput("text", "DataSpigatura48", "mod-48", grid);

    $("#DataSpigatura48").kendoDoY_DatePicker({
        value: new Date(now.getFullYear(), 4, 1)
    });
    $("#DataSpigatura48").attr("readonly", true);

    //*****************************************************************************************
    //RaccaFrumento_RuggineBruna: 51,
    //RaccaFrumento_RuggineGialla: 52,
    //RaccaFrumento_RuggineNera: 53,
    //RaccaFrumento_Stagonosporiosi: 54,
    //RaccaFrumento_Septoria: 55,
    //RaccaFrumento_Fusariosi: 56,
    //RaccaFrumento_FusariosiSpiga: 57,
    //RaccaFrumento_Fusariosi2: 58,
    //RaccaFrumento_Fusariosi3: 59,
    //RaccaFrumento_MarciumeRosa: 60
    //*****************************************************************************************
    let class_51_60 = "mod-51 mod-52 mod-53 mod-54 mod-55 mod-56 mod-57 mod-58 mod-59 mod-60"

    createLabel("Data semina", class_51_60, grid);
    createInput("text", "DataSemina51", class_51_60, grid);

    $("#DataSemina51").kendoDatePicker({
        format: "dd MMMM yyyy",
        value: new Date(now.getFullYear() - 1, 9, 1),
        footer: false
    });
    $("#DataSemina51").attr("readonly", true);

    createLabel("Data raccolta prevista", class_51_60, grid);
    createInput("text", "DataRaccolta51", class_51_60, grid);

    $("#DataRaccolta51").kendoDatePicker({
        format: "dd MMMM yyyy",
        value: new Date(now.getFullYear(), 6, 1),
        footer: false
    });
    $("#DataRaccolta51").attr("readonly", true);

    createLabel("Resistenza varietale", class_51_60, grid);
    createInput("select", "Resistenza51", class_51_60, grid);

    $("#Resistenza51").kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: [
            { text: "Suscettibile", value: 0 },
            { text: "Medio resistente", value: 1 },
            { text: "Resistente", value: 2 }
        ],
        index: 0
    });

}

function createLabel(text, mod_cod, parent_div) {
    let div = document.createElement("div");
    div.className = "form-label param-modello " + mod_cod + " meteo-hidden";
    let span = document.createElement("span");
    span.textContent = text;
    div.appendChild(span);

    if (parent_div) {
        parent_div.appendChild(div);
    }

    return div
}

function createInput(type, id, mod_cod, parent_div) {
    let div = document.createElement("div");
    div.className = "param-modello " + mod_cod + " meteo-hidden";
    let input = document.createElement("input");
    input.type = type;
    input.style.cssText = "width: 100%";
    input.id = id;
    div.appendChild(input);

    if (parent_div) {
        parent_div.appendChild(div);
    }

    return div;
}

function createBtn(id, mod_cod, parent_div, testo, clickFunction) {
    let div = document.createElement("div");
    div.className = "param-modello " + mod_cod + " meteo-hidden";
    div.style = "width: 100%";

    let divBtn = document.createElement("div");
    divBtn.innerHTML = testo;
    divBtn.style = "width: 100%";

    div.appendChild(divBtn);

    $(divBtn).kendoButton({
        click: clickFunction
    });

    if (parent_div) {
        parent_div.appendChild(div);
    }

    return div;
}

function evaluateOnAllStations() {

    let sorgentiMeteo = GlobalMeteoSourceSelector.sorgentiMeteoList();

    if (sorgentiMeteo.tipo === "" || sorgentiMeteo.stazione === "") {
        return;
    }

    let range = elabPeriodoPlugin().getRange();

    if (range.start === undefined || range.end === undefined) {
        return;
    }

    let Veg_Cod = parseInt($("#cmbSpecieVegetale").getKendoDropDownList().value());

    if (isNaN(Veg_Cod)) {
        return;
    }

    let modello = $("#cmbAvModAlg").getKendoDropDownList().dataItem();

    if (modello === undefined) {
        return;
    }

    let parametriAggiuntivi = LeggiParametriXModello(modello.Mod_Cod);

    let param = {
        DataDa: kendo.toString(range.start, "d"),
        DataA: kendo.toString(range.end, "d"),
        TipoSorgente: sorgentiMeteo.tipo,
        Sorgente: sorgentiMeteo.stazioni,
        ModelloPrevisionale: modello.Mod_Cod,
        Veg_Cod: Veg_Cod,
        Av_Cod: modello.Avv_Cod,
        Algoritmo: modello.Alg_Cod,
        ParametriAggiuntivi: JSON.stringify(parametriAggiuntivi)
    };

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let mapH = Math.round(window.outerHeight * 0.8);

    let map_el = document.createElement("div");
    map_el.style.cssText = "width:100%; height: " + mapH + "px;";
    map_el.id = "___mapContainer";
    win_el.appendChild(map_el);

    $win_el.kendoDialog({
        width: "90%",
        title: "",
        closable: true,
        modal: true,
        visible: false,
        show: function () {
            $("#___mapContainer").geoSquaresMap(param);
        },
        close: function () {
            $("#___mapContainer").data("geoSquaresMap")?.worker?.terminate();
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}

function GestisciParametri(id_grid, mod_cod, objParam) {

    $("#" + id_grid).find(".param-modello").each(function () {
        $(this).addClass("meteo-hidden");
    });

    let classname = "mod-" + mod_cod;
    $("#" + id_grid).find(".param-modello." + classname).each(function () {
        $(this).removeClass("meteo-hidden");
    });

    if (objParam === undefined || objParam === null) {

        return;
    }

    objParam = JSON.parse(objParam);

    switch (parseInt(mod_cod)) {

        case Enum_ModelloPrevisionale.Racca__ScleroSoia:

            if (objParam.Varieta !== undefined) {

                $("#Varieta30").getKendoDropDownList().select(function (dataItem) {
                    return dataItem.text.toUpperCase() === objParam.Varieta.toUpperCase();
                });
            }
            break;

        case Enum_ModelloPrevisionale.Racca__BrusoneRiso:

            if (objParam.DataSemina_gg !== undefined) {
                $("#DataSemina31").data("kendoDoY_DatePicker").doy(objParam.DataSemina_gg);
            }

            if (objParam.Varieta !== undefined) {
                $("#Varieta31").getKendoDropDownList().select(function (dataItem) {
                    return dataItem.text.toUpperCase() === objParam.Varieta.toUpperCase();
                });
            }
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__MISP_IPI_Pomodoro:

            if (objParam.DataTrapianto_gg !== undefined) {
                $("#DataTrapianto42").data("kendoDoY_DatePicker").doy(objParam.DataTrapianto_gg);
            }
            break;

        case Enum_ModelloPrevisionale.UniCatt__AFLA_Mais:
        case Enum_ModelloPrevisionale.UniCatt__FER_Mais:
            if (objParam.DataEmergenza_gg !== undefined) {
                $("#DataEmergenza44_45").data("kendoDoY_DatePicker").doy(objParam.DataEmergenza_gg);
            }
            break;

        case Enum_ModelloPrevisionale.BetaCoProB__Cercosporiosi:
            if (objParam.DataPartenza_gg !== undefined) {
                $("#DataPartenza47").data("kendoDoY_DatePicker").doy(objParam.DataPartenza_gg);
            }
            break;

        case Enum_ModelloPrevisionale.UniCatt__Fusariosi_Frumento:
            if (objParam.DataSpigatura_gg !== undefined) {
                $("#DataSpigatura48").data("kendoDoY_DatePicker").doy(objParam.DataSpigatura_gg);
            }
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
            if (objParam.DataSemina !== undefined) {
                $("#DataSemina51").data("kendoDatePicker").value(objParam.DataSemina);
            }

            if (objParam.DataRaccolta !== undefined) {
                $("#DataRaccolta51").data("kendoDatePicker").value(objParam.DataRaccolta);
            }

            if (objParam.ResistenzaVarietale !== undefined) {
                $("#Resistenza51").getKendoDropDownList().select(function (dataItem) {
                    return dataItem.value === objParam.ResistenzaVarietale;
                });
            }
            break;

    }
}

function LeggiParametriXModello(mod_cod) {
    let resParam = {};

    switch (parseInt(mod_cod)) {
        case Enum_ModelloPrevisionale.Racca__ScleroSoia:
            resParam = { Varieta: $("#Varieta30").getKendoDropDownList().text() };
            break;

        case Enum_ModelloPrevisionale.Racca__BrusoneRiso:
            resParam = {
                DataSemina_gg: $("#DataSemina31").data("kendoDoY_DatePicker").doy(),
                Varieta: $("#Varieta31").getKendoDropDownList().text()
            };
            break;

        case Enum_ModelloPrevisionale.Agronomica_30__MISP_IPI_Pomodoro:
            resParam = { DataTrapianto_gg: $("#DataTrapianto42").data("kendoDoY_DatePicker").doy() };
            break;

        case Enum_ModelloPrevisionale.UniCatt__AFLA_Mais:
        case Enum_ModelloPrevisionale.UniCatt__FER_Mais:
            resParam = { DataEmergenza_gg: $("#DataEmergenza44_45").data("kendoDoY_DatePicker").doy() };

            if ($("#Attiva44_45").getKendoSwitch().check()) {
                let previsione_micotox = {
                    classe_fao: parseInt($("#Classe44_45").getKendoDropDownList().value()),
                    prev_crop: parseInt($("#Prev_Crop44_45").getKendoDropDownList().value()),
                    data_semina_gg: $("#DataSemina44_45").data("kendoDoY_DatePicker").doy(),
                    data_raccolta_gg: $("#DataRaccolta44_45").data("kendoDoY_DatePicker").doy(),
                    danno_piralide: parseInt($("#Piralide44_45").getKendoDropDownList().value()),
                    umidita_cariosside: $("#Cariosside44_45").getKendoNumericTextBox().value()
                };

                resParam = $.extend({}, { previsione_micotox: previsione_micotox }, resParam);
            }
            break;

        case Enum_ModelloPrevisionale.BetaCoProB__Cercosporiosi:
            resParam = { DataPartenza_gg: $("#DataPartenza47").data("kendoDoY_DatePicker").doy() };
            break;

        case Enum_ModelloPrevisionale.UniCatt__Fusariosi_Frumento:
            resParam = { DataSpigatura_gg: $("#DataSpigatura48").data("kendoDoY_DatePicker").doy() };
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
            resParam = {
                DataSemina: kendo.toString($("#DataSemina51").data("kendoDatePicker").value(), "yyyy-MM-ddTHH:mm:ss"),
                DataRaccolta: kendo.toString($("#DataRaccolta51").data("kendoDatePicker").value(), "yyyy-MM-ddTHH:mm:ss"),
                ResistenzaVarietale: parseInt($("#Resistenza51").getKendoDropDownList().value()),
            };
            break;
    }

    return resParam;
}

(function ($) {
    $.geoSquaresMap = function (elem, opts) {
        var plugin = this;

        var _gmap = new google.maps.Map(elem, {
            zoomControl: false,
            streetViewControl: false,
            fullscreenControl: false,
            mapTypeControl: false,
            rotateControl: false,
            mapTypeId: google.maps.MapTypeId.HYBRID,
            tilt: 0,
            zoom: 3,
            center: new google.maps.LatLng(44, 12),
            styles: [
                {
                    featureType: "poi",
                    elementType: "labels",
                    stylers: [
                        { visibility: "off" }
                    ]
                }
            ]
        });

        //costruisco la griglia memorizzando i vertici
        let grid = { lat: new Array(), lng: new Array() }; 

        opts.Sorgente.forEach(s => {
            if (s.geo) {
                s.geo.lat = Math.round(s.geo.lat * 10000) / 10000;
                s.geo.lng = Math.round(s.geo.lng * 10000) / 10000;

                ["lat", "lng"].forEach(coord => {

                    let lower = 0;
                    let upper = grid[coord].length;
                    let index = Math.floor((lower + upper) / 2);
                    let found = false;

                    while (!found && upper > lower) {

                        if (s.geo[coord] < grid[coord][index]) {
                            upper = index;
                        } else {
                            if (Math.abs(s.geo[coord] - grid[coord][index]) < 0.0001) {
                                found = true;
                            }

                            lower = index + 1;
                        }

                        index = Math.floor((lower + upper) / 2);
                    }

                    if (!found) {
                        // insert the item
                        grid[coord].splice(index, 0, s.geo[coord]);
                    }
                });
            }
        });

        ["lat", "lng"].forEach(coord => {
            let last = grid[coord].length - 1;
            grid[coord].push(Math.round((grid[coord][last] + (grid[coord][last] - grid[coord][last - 1])) * 10000) / 10000);
        });

        let bbox = new google.maps.LatLngBounds({ lat: grid.lat[0], lng: grid.lng[0] }, { lat: grid.lat[grid.lat.length - 1], lng: grid.lng[grid.lng.length - 1] });

        _gmap.fitBounds(bbox);

        opts.Sorgente.forEach(s => {
            if (s.geo) {
                //il punto passato è il punto sud-ovest del quadrato

                let bounds = { lat: { lo: 0, hi: 0 }, lng: { lo: 0, hi: 0 } };

                ["lat", "lng"].forEach(coord => {
                    let lower = 0;
                    let upper = grid[coord].length - 1;
                    let middle = Math.floor((lower + upper) / 2);

                    while (upper - lower > 1) {
                        if (s.geo[coord] < grid[coord][middle]) {
                            upper = middle;
                        } else {
                            lower = middle;
                        }

                        middle = Math.floor((lower + upper) / 2);
                    }

                    bounds[coord].lo = grid[coord][lower];
                    bounds[coord].hi = grid[coord][upper];
                });

                let f = new google.maps.Data.Feature({
                    geometry: new google.maps.Data.Polygon([[
                        new google.maps.LatLng(bounds.lat.lo, bounds.lng.lo),
                        new google.maps.LatLng(bounds.lat.lo, bounds.lng.hi),
                        new google.maps.LatLng(bounds.lat.hi, bounds.lng.hi),
                        new google.maps.LatLng(bounds.lat.hi, bounds.lng.lo)
                    ]]),
                    id: s.id,
                    properties: {
                        descr: s.descr,
                        value: null
                    }
                });

                _gmap.data.add(f);
            }
        });

        _gmap.data.setStyle({
            fillColor: "#fff",
            strokeWeight: 0.2,
            fillOpacity: 0.3
        });

        var _info = new google.maps.InfoWindow();

        _gmap.data.addListener("click", (event) => {
            let bounds = new google.maps.LatLngBounds();
            event.feature.getGeometry().forEachLatLng(ll => {
                bounds.extend(ll);
            });

            let value = event.feature.getProperty("value");
            if (value != null) {
                value = kendo.toString(value, "0.00");
            } else {
                value = "";
            }

            let content = '<div style="font-size: smaller;">';
            content += '<div>' + event.feature.getProperty("descr") + '</div>';
            content += '<div style="text-align: center; font-weight: bold;">' + value + '</div>';
            content += '</div>';

            _info.setContent(content);
            _info.setPosition(bounds.getCenter());
            _info.open(_gmap);
        });

        const controlDiv = document.createElement("div");
        controlDiv.style.padding = "0px 2px 2px 0px";
        controlDiv.style.borderBottomRightRadius = "4px";
        controlDiv.style.backgroundColor = "#fff";
        controlDiv.style.userSelect = "none";

        const downloadBtn = document.createElement("div");
        downloadBtn.style.fontSize = "20px";

        controlDiv.appendChild(downloadBtn);

        $(downloadBtn).kendoButton({
            iconClass: "fa fa-file-excel-o",
            fillMode: "flat",
            themeColor: "success",
            enable: false,
            click: function () {
                let rows = [{
                    cells: [
                        { value: "Stazione", bold: true, background: "#E0E0E0" },
                        { value: "Latitudine", bold: true, background: "#E0E0E0" },
                        { value: "Longitudine", bold: true, background: "#E0E0E0" },
                        { value: "Valore", bold: true, background: "#E0E0E0" }
                    ]
                }];

                _gmap.data.forEach(f => {
                    let bounds = new google.maps.LatLngBounds();
                    f.getGeometry().forEachLatLng(ll => {
                        bounds.extend(ll);
                    });

                    let ll = bounds.getCenter();

                    rows.push({
                        cells: [
                            { value: f.getProperty("descr") },
                            { value: ll.lat(), format: "0.0000" },
                            { value: ll.lng(), format: "0.0000" },
                            { value: f.getProperty("value"), format: "0.00" }
                        ]
                    });
                });

                var workbook = new kendo.ooxml.Workbook({
                    sheets: [
                        {
                            columns: [
                                { autoWidth: true },
                                { autoWidth: true },
                                { autoWidth: true },
                                { autoWidth: true }
                            ],
                            rows: rows
                        }
                    ]
                });
                kendo.saveAs({dataURI: workbook.toDataURL(), fileName: "MappaStazioni.xlsx"});
            }
        });

        _gmap.controls[google.maps.ControlPosition.TOP_LEFT].push(controlDiv);

        var _downloadBtn = $(downloadBtn).getKendoButton();

        //const controlSliderDiv = document.createElement("div");
        //controlSliderDiv.style.padding = "10px 0px 10px 10px";
        //controlSliderDiv.style.borderTopLeftRadius = "4px";
        //controlSliderDiv.style.borderBottomLeftRadius = "4px";
        //controlSliderDiv.style.backgroundColor = "#fff";

        //const sliderInput = document.createElement("input");

        //controlSliderDiv.appendChild(sliderInput);

        //$(sliderInput).kendoSlider({
        //    min: 0,
        //    max: 100,
        //    value: 80,
        //    showButtons: false,
        //    tooltip: { enabled: false },
        //    orientation: "vertical",
        //    change: function (e) {
        //        _gmap.data.forEach((f) => {
        //            _gmap.data.overrideStyle(f,
        //                {
        //                    fillOpacity: e.value / 100.0
        //                }
        //            );
        //        });
        //    }
        //});

        //_gmap.controls[google.maps.ControlPosition.RIGHT_CENTER].push(controlSliderDiv);

        //var _slider = $(sliderInput).getKendoSlider();

        //_slider.enable(false);

        var _worker;

        if (typeof window.Worker === "function") {
            _worker = new Worker("DSS_Evaluate_Worker.js");

            $(window).bind('beforeunload', function () {
                _worker.terminate();
            });

            _worker.onmessage = function (event) {
                let data = event.data;
                if (data !== null) {  

                    let feature = _gmap.data.getFeatureById(data.id);
                    if (feature) {
                        _gmap.data.overrideStyle(feature, { fillColor: data.color, fillOpacity: 0.8 });
                        feature.setProperty("value", data.value);
                    }

                } else {
                    _downloadBtn.enable(true);
                    //_slider.enable(true);
                    //_slider.resize();
                    meteoAlert("Elaborazione mappa", "Elaborazione terminata...");
                }
            };

            _worker.postMessage({
                baseurl: url_meteo_ws,
                params: opts
            });

        }

        plugin.worker = _worker;
    }; // geoSquaresMap

    //Add the plugin to the jQuery.fn object
    $.fn.geoSquaresMap = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('geoSquaresMap')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.geoSquaresMap(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('geoSquaresMap', plugin);
            }
        });
    };
})(jQuery);