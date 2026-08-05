function TraduciLavorazioni(chiave, testoAlternativo) {
    return TraduzioneMultiResx(confUMAEditResx, chiave, testoAlternativo);
}

$(document).ready(function () {
    docR();
});


async function docR() {
    WaitFrame.show();

    //if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
    //    // Carico i files resx per le traduzioni
    //    resxArrPath.forEach(function (resxSinglePath) {
    //        confUMAEditResx.push(readResxFile(resxSinglePath, "ConfigurazioneUMA_jQueryDocReady.js"));
    //    });
    //}
    //$("#InizioValidita").kendoDatePicker();
    //$("#FineValidita").kendoDatePicker();
    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 12, 31)
    });
    var annoCorrente = new Date().getFullYear();

    if (elencoMacchine.length === 0 || elencoMacchine.length == undefined) {
        elencoMacchine = await getElencoTipoMacchine()
    }

    if (elencoStati.length === 0 || elencoStati.length == undefined) {
        elencoStati = await getElencoStati()
    }


    if (elencoGruppi.length === 0 || elencoGruppi.length == undefined) {
        elencoGruppi = await getElencoGruppiUtente()
    }

    // Inserisci la data nel tuo elemento HTML
    //$("#InizioValidita").kendoCalendar({
    //    value:     dataFormattata 
    //});
    //var data_filtro = $("#InizioValidita").data("kendoDatePicker");
    //data_filtro.value('01/01/' + annoCorrente);
    //var data_filtro = $("#FineValidita").data("kendoDatePicker");
    //data_filtro.value('31/12/' + annoCorrente);

    var startYearDate = formattedDate(new Date(new Date().getFullYear(), 0, 1), "/");
    set_data("InizioValidita", startYearDate, null);
    startYearDate = formattedDate(new Date(new Date().getFullYear(), 11, 31), "/");
    set_data("FineValidita", startYearDate, null);

    let tabstrip = $("#tabstrip_elenco").kendoTabStrip({
        select: onActivate,
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    }).data("kendoTabStrip");
    popolaGrigliaConfigurazioniUMA(grdConfigurazioniUMA);
    init_tabLavUMA = true;

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    //Nascondo Tab Linee Produzioni
    $("#tabstrip_elenco").show();

    WaitFrame.hide();
}


function onActivate(e) {
    let elem = $(e.item).index();
    SeleTab(elem);
}

function SeleTab(index) {
    var Inizio = $("#InizioValidita").val();
    var Fine = $("#FineValidita").val();

    var dataValida = true;
    if (Fine !== "")
        dataValida = isValidDate(Inizio);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("Da data Inizio validità non valida", "DIV_Messaggi");
        return;
    }
    if (Fine !== "")
        dataValida = isValidDate(Fine);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("Da data Fine validità non valida", "DIV_Messaggi");
        return;
    }
    if ($("#FineValidita").data("kendoDatePicker").value() < $("#InizioValidita").data("kendoDatePicker").value()) {
        MessaggioErrore_Bootstrap("intervallo date errato", "DIV_Messaggi");
        return;
    }      

    switch (index) {
        case 0:
            if (!init_tabLavUMA) {
                WaitFrame.show();
                popolaGrigliaConfigurazioniUMA(grdConfigurazioniUMA);
                dati_impianto_Loaded = true;
                WaitFrame.hide();
            }
            break;
        case 1:
            if (!init_tabLavAlt) {
                WaitFrame.show();
                LavAlt_popolaGriglia(grdLavorazioniAlternative);
                init_tabLavAlt = true;
                WaitFrame.hide();
            }
            break;
        case 2:
            if (!init_tabSetup) {
                WaitFrame.show();
                Setup_popolaGriglia(grdSetup);
                init_tabSetup = true;
                WaitFrame.hide();
            }
            break;
        case 3:
            if (!init_tabUMAConfigurazioneAllevamenti) {
                WaitFrame.show();
                UMAConfigurazioneAllevamenti_popolaGriglia(grdUMAConfigurazioneAllevamenti);
                init_tabUMAConfigurazioneAllevamenti = true;
                WaitFrame.hide();
            }
            break;
        case 4:
            if (!init_tabDateRendicontazioni) {
                WaitFrame.show();
                DateRendicontazioni_popolaGriglia("grdConfigurazioneDateRendicontazioni");
                init_tabDateRendicontazioni = true;
                WaitFrame.hide();
            }
            break
        case 5:
            if (!init_tabConfigurazioneUF) {
                WaitFrame.show();
                UF_popolaGriglia("grdConfigurazioneUF");
                init_tabConfigurazioneUF = true;
                WaitFrame.hide();
            }
            break;
        case 6:
            if (!init_tabConfigurazioneElencoMacrousi) {
                WaitFrame.show();
                ElencoMacrousi_popolaGriglia("grdElencoMacrousiUMA");
                init_tabConfigurazioneElencoMacrousi = true;
                WaitFrame.hide();
            }
            break;
        case 7:
            if (!init_tabConfigurazioneElencoLavorazioni) {
                WaitFrame.show();
                ElencoLavorazioni_popolaGriglia("grdElencoLavorazioniUMA");
                init_tabConfigurazioneElencoLavorazioni = true;
                WaitFrame.hide();
            }
            break;
        case 8:
            if (!init_tabConfigurazioneElencoAllevamenti) {
                WaitFrame.show();
                ElencoAllevamenti_popolaGriglia("grdElencoAllevamentiUMA");
                init_tabConfigurazioneElencoAllevamenti = true;
                WaitFrame.hide();
            }
            break;
        case 9:
            if (!init_tabConfigurazioneElencoAssociazioniMacrousi) {
                WaitFrame.show();
                //MacrousiUMA_Load();
                ElencoAssociazioniMacrousi_popolaGriglia("grdAssociazioniMacrousiUMA");
                init_tabConfigurazioneElencoAssociazioniMacrousi = true;
                WaitFrame.hide();
            }
            break;

    }

}

$("#btnModificaMassiva").click(
    function () {
        var row = $(this).parents("tr"),
            grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid"),
            selected = grid.dataSource.data().filter((el) => { return el.Selected == true });

        if (selected.length > 0) {
            //let div = document.createElement("div");
            $(document.body).append('<div id="ddlDialog"></div>');
            $("#ddlDialog").kendoDialog({
                content: '<div class="form-horizontal">' +
                    '<div class="form-group">' +
                    '<div class="input-group">' +
                    '<label class="input-group-addon control-label alert-info" id="lblMacrousoUMA" for="dllMacrousoUMA">' +
                    'Macrouso UMA' +
                    '</label>' +
                    '<input type="text" id="dllMacrousoUMA" name="dllMacrousoUMA" class="form-control">' +
                    '</div>' +
                    '</div>' +
                    '</div>',
                title: "Conferma aggiornamento massivo",
                closable: false,
                open: MacrousiUMA_Load,
                width: 400,
                actions: [
                    {
                        text: "Applica",
                        primary: true,
                        action: function (e) {
                            var dll = $("#dllMacrousoUMA").data("kendoDropDownList");
                            var macrouso = dll.dataItem();

                            if (macrouso.Macrouso_UMA_Cod != null) {
                                for (var i = 0; i < selected.length; i++) {
                                    console.log(selected[i]);
                                    selected[i].Macrouso_UMA_Cod = macrouso.Macrouso_UMA_Cod;
                                    selected[i].Macrouso_UMA_Des = macrouso.Macrouso_UMA_Des;
                                    selected[i].dirty = true;
                                    selected[i].Selected = false;
                                }
                                grid.refresh();
                                $("#btnModificaMassiva")[0].setAttribute("disabled", "");
                            }
                            else {
                                return false;
                            }
                            
                        }
                    },
                    {
                        text: "Annulla",
                        primary: false,
                        action: function (e) {
                            //$("#btnModificaMassiva")[0].setAttribute("disabled", "");
                        },
                    }
                ],
                close: function () {
                    $(this).empty();
                    $(this).remove();
                    $("#ddlDialog").remove();
                }
            })
                .data("kendoDialog")
                .open();
        }
    }
)