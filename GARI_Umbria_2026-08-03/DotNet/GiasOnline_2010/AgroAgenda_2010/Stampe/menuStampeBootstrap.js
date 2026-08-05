
function onSelectKendoMenu(e) {
    text = $(e.item).children(".k-link").text().split('|')[0]
    idStampa = dizionarioStampe[text]

    //Per evitare che cliccando su una macrocategoria si provi ad avviare una stampa
    if (idStampa != undefined)
        gestisciStampa(idStampa)
}

function caricaDDLRicercaRapida(option) {

    option.success(stampeAutorizzate)
}
function controllaDataValida() {
    try {
        let isDataSet = get_data("anno") != null;
        let isDataNeeded = false;
        switch (intIdStampa) {
            case enumStampe.Atto_Notorio:
                isDataNeeded = true;
                break
            case enumStampe.Atto_Notorio * - 1:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_Etico_Ambientale:
                isDataNeeded = true;
                break
            case enumStampe.Tenuta_Scheda_Campagna:
                isDataNeeded = true;
                break
            case enumStampe.Codice_Condotta:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_DPI:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Eurep:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_QC:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Confusione_Sessuale:
                isDataNeeded = true;
                break
            case enumStampe.Allegato_CatastoeValorizzazioni:
                isDataNeeded = true;
                break
            case enumStampe.Mandato_Trasmissione_Telematica_Dati:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Orticole_Gest_Annuale:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Orticole_Gest_Breve:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Fagiolino_Mercato_Fresco:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Orticole_Industria:
                isDataNeeded = true;
                break
            case enumStampe.Impegnativa_Pomodoro_Industria:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_ModuloGrasp:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_ProtocolloGlobalGAP:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_NurtureModule:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_Despar:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_Conad:
                isDataNeeded = true;
                break
            case enumStampe.Accordo_Responsabilita_di_Filiera:
                isDataNeeded = true;
                break
            case enumStampe.Dichiarazione_di_Responsabilita:
                isDataNeeded = true;
                break
            case enumStampe.Fitoregolatori_Kiwi:
                isDataNeeded = true;
                break
            case enumStampe.Adesione_StandardLeaf:
                isDataNeeded = true;
                break
            case enumStampe.SchedaAziendale:
                isDataNeeded = true;
                break
            case enumStampe.ImpegnoProduzioneSociDivisoxCentri:
                isDataNeeded = true;
                break
            case enumStampe.ImpegnativaColtivazioneConferimento:
                isDataNeeded = true;
                break
            default:
                isDataNeeded = false;
        }

        if (isDataNeeded && !isDataSet) {
            MessaggioErrore_Bootstrap("Indica l'anno", "DIV_Messaggi")
            return false;
        }
        return true;
    } catch (x) {
        return true;
    }

}

function templateItem_stampe() {
    let t = "";
    t += "<div class='list-view-item stampa'>";
    t += "    <div class='row k-block item-block'>";
    t += "        <div class='col-lg-5 col-md-5 col-sm-5 stampa-gruppo'><span>#:group#</span></div>";
    t += "        <div class='col-lg-5 col-md-5 col-sm-5 stampa-nome'><span>#:text#</span></div>";
    t += "        <div class='col-lg-2 col-md-2 col-sm-2 k-button fa-btn btn-star btn-star-pref'><span class='fa fa-star'></span></div>";
    t += "    </div>";
    t += "</div>";
    return t;
}

function templateItem_stampe_2024() {

    let t = "";
    t += "<div class='list-view-item stampa #:favClass#'>";
    t += "    <div class='row k-block item-block'>";

    t += "      <div class='stampa-item-container'>";
    // qui vengono utilizzate le funzionalità condizionali del template di kendo per gestire le icone
    // attenzione perchè la chiamata al serve è gestita in un altro punto
    t += "        <div class='fa-btn btn-star-pref item-action'><span class='#if (fav==1) {# fa fa-check-square #} else {# fa fa-square-o #}#'></span></div>";
    t += "        <div class='list-text '><div class='col-lg-6 col-md-6 col-sm-6 stampa-gruppo'><span>#:group#</span></div>";
    t += "        <div class='col-lg-6 col-md-6 col-sm-6 stampa-nome'><span>#:text#</span></div></div>";

    t += "      </div>";
    t += "    </div>";
    t += "</div>";
    return t;
}

function templateItem_preferiti() {
    let t = "";
    t += "<div class='list-view-item pref'>";
    t += "    <div class='k-block item-block'>";
    t += "        <div class='col-lg-9 col-md-9 col-sm-9 pref-nome'><span>#:text#</span></div>";
    t += "        <div class='col-lg-3 col-md-3 col-sm-3 k-button fa-btn btn-trash btn-thrash-pref'><span class='fa fa-trash'></span></div>";
    t += "    </div>";
    t += "</div>";
    return t;
}

function templateItem_preferiti_2024() {
    let t = "";
    t += "<div class='list-view-item-pref-2024'>"
    t += "  <div class='fa-btn btn-thrash-pref item-action' btn-thrash-pref><span class='fa fa-minus-square'></span></div>";
    t += "  <div class='list-view-item stampa gias-fav-true'>";
    t += "      <div class='row k-block item-block'>";
    t += "          <div class='stampa-item-container'>";
    t += "              <div class='stampa-nome'><span>#:text#</span></div></div>";
    t += "          </div>";
    t += "      </div>";
    t += "  </div>";
    t += "</div>";
    return t;
}

function imposta_txtRicercaPreferiti() {
    win_txtRicercaStampe = $("#win_txtRicercaStampe").kendoTextBox({
        placeholder: "Cerca Stampe",        
    }).data("kendoTextBox");

    $("#win_txtRicercaStampe").on("input", function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(filtraStampe, doneTypingInt);
    });
}

function popolaElencoStampe() {
    let template = (GiasVersioneMaster === "2022" ? templateItem_stampe_2024() : templateItem_stampe());

    win_listStampe = $('#win_listStampe').kendoListView({
        dataSource: { transport: { read: leggiStampe } },
        autoBind: true,
        scrollable: true,
        layout: "flex",
        flex: { direction: "column", wrap: "nowrap" },
        template: kendo.template(template),
        dataBound: function (e) {
            e.sender.content.find(".btn-star-pref").each(function (i, e) {
                e.onclick = function () {
                    let st = e.closest(".list-view-item");
                    if (st.classList.contains('gias-fav-true')) {
                        console.log("Do nothing..")
                    } else {
                        addPreferiti(st);
                    }
                }
            });
        }
    }).data("kendoListView");
}

function popolaElencoPreferiti() {
    let template = (GiasVersioneMaster === "2022" ? templateItem_preferiti_2024() : templateItem_preferiti());

    win_listPreferiti = $('#win_listPreferiti').kendoListView({
        dataSource: { transport: { read: leggiPreferiti } },
        autoBind: true,
        scrollable: true,
        layout: "flex",
        flex: { direction: "column", wrap: "nowrap" },
        template: kendo.template(template),
        dataBound: function (e) {
            e.sender.content.find(".btn-thrash-pref").each(function (i, e) {
                e.onclick = function () {
                    if (GiasVersioneMaster === "2022") {
                        let st = e.closest(".list-view-item-pref-2024");
                        removePreferiti(st);
                    } else {
                        let st = e.closest(".list-view-item");
                        removePreferiti(st);
                    }

                }
            });
        }
    }).data("kendoListView");
}

function filtraStampe() {
    let filtro = $("#win_txtRicercaStampe").val();
    if (filtro) {
        win_listStampe.dataSource.filter({
            logic: "or",
            filters: [{ field: "text", operator: "contains", value: filtro }]
        });
    } else {
        win_listStampe.dataSource.filter({});
    }
}

function leggiStampe(options) {
    caricaStampeAutorizzate();

    //setta il flag a seconda della presenza o meno della stampa nei preferiti
    stampeAutorizzate.forEach((st) => {
        if (stampePreferite.find(x => x.value == st.value)) {
            st.fav = 1;
            st.favClass = 'gias-fav-true';
        }
        else {
            st.fav = 0;
            st.favClass = 'gias-fav-false';
        }
    });
    stampeAutorizzate.sort((a, b) => {
        if (a.group.toLowerCase() < b.group.toLowerCase()) return -1;
        if (a.group.toLowerCase() > b.group.toLowerCase()) return 1;
        if (a.text.toLowerCase() < b.text.toLowerCase()) return -1;
        if (a.text.toLowerCase() > b.text.toLowerCase()) return 1;
        return 0;
    });
    options.success(stampeAutorizzate);
}

function leggiPreferiti(options) {
    caricaStampePreferite();
    mappingStampePref();

    stampePreferite.sort((a, b) => {
        if (a.group.toLowerCase() < b.group.toLowerCase()) return -1;
        if (a.group.toLowerCase() > b.group.toLowerCase()) return 1;
        if (a.text.toLowerCase() < b.text.toLowerCase()) return -1;
        if (a.text.toLowerCase() > b.text.toLowerCase()) return 1;
        return 0;
    });
    options.success(stampePreferite);
}

function mappingStampePref() {
    stampeAutorizzate.forEach(st => mappingGruppi[st.value] = st.group);
    stampePreferite.forEach(st => st.value in mappingGruppi ? st.group = mappingGruppi[st.value] : -1);
}

function addPreferiti(st) {
    WaitFrame.show();

    let uid = $(st).attr("data-uid");
    let dataItem = win_listStampe.dataSource.getByUid(uid);

    if (dataItem === undefined) { return; }

    if (!stampePreferite.find(x => x.value == dataItem.value)) {
        aggiungiPreferiti(dataItem.value);
        win_listPreferiti.dataSource.read();
        win_listStampe.dataSource.read();
    } else {
        kendo.alert("La stampa selezionata è già presente nei preferiti.");
    }

    WaitFrame.hide();
}

function removePreferiti(st) {
    WaitFrame.show();

    let uid = $(st).attr("data-uid");

    let dataItem = win_listPreferiti.dataSource.getByUid(uid);

    if (dataItem === undefined) { return; }

    if (stampePreferite.find(x => x.value == dataItem.value)) {
        eliminaPreferiti(dataItem.value);
        win_listPreferiti.dataSource.read();
        win_listStampe.dataSource.read();
    }

    WaitFrame.hide();
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    var title = String.format("Seleziona Entità per '{0}'", KendoDDL("ddRicercaRapida").text())

    $('#filtro_ricerca_ng').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        objRedirectStampe_FiltroRicercaNG.tipoMostra = event.data.inData.tipoEntita

        WS_Gestisci_Redirect_Stampe(event.data.inData.chiavi)

        kWin.close();
    }
}