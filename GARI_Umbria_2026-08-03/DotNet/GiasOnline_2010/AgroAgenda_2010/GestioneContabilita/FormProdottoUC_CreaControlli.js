var resxFormProdottoUC = [];


function creaControlli_FormProdottoUC() {

    // -----------------------------------------------------------------
    // INIZIO Creazione di tutti i controlli della pagina di dettaglio
    // -----------------------------------------------------------------

    if (resxObj !== null && resxObj !== undefined) {
        resxFormProdottoUC = resxObj;
        resxFormProdottoUC.unshift(readResxFile("GestioneContabilita/App_LocalResources/FormProdottoUC.ascx.resx"));
    }
    else {
        resxFormProdottoUC.push(readResxFile("GestioneContabilita/App_LocalResources/FormProdottoUC.ascx.resx"));
        resxFormProdottoUC.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx"));
    }

    // INIZIO CAMPO PRODOTTO
    CreaDdlProdottoDes();
    // FINE CAMPO PRODOTTO 

    // Alias del prodotto
    let kddlProdAlias = creaKendoDropDownList("ddlProdAlias", { read: ddlProdAlias_read }, "Mat_Des_Alias", "Mat_Cod_Alias", null, null, null, false).data("kendoDropDownList");
    kddlProdAlias.bind("dataBound", ddlProdAlias_databound);

    //KendoSwitch Vari
    creaKendoSwitch("chkAggregaLottoImpianto");
    creaKendoSwitch("chkParametroQualitativo").bind("change", chkParametroQualitativo_change);

    //KendoDDL Varie
    creaKendoDropDownList("ddlCausale_Riga", { read: RiempiCausale_Riga }, "DescrCausale", "KeyCausale").bind("change", ddlCausale_Riga_change);
    creaKendoDropDownList("ddlUbicProvenienza", { read: RiempiElencoCelleEMagazzini }, "Ubic_Des", "key_Dest").bind("change", ddlUbicProvenienza_change);
    creaKendoDropDownList("ddlUbicDestinazione", { read: RiempiElencoCelleEMagazzini }, "Ubic_Des", "key_Dest").bind("change", ddlUbicDestinazione_change);

    // Confezionamento Lotto
    creaKendoDropDownList("ddlConfezionamentoLotto", { read: RiempiElencoConfezionamentoLotto }, "mat_des", "cod_articolo").bind("change", ddlConfezionamentoLotto_change);
    KendoDDL("ddlConfezionamentoLotto").bind("open", ddlConfezionamentoLotto_open);

    // Imposto valore di default del magazzino
    var keyMag = "";
    if (xFabbricato_Cod !== 0 && parseInt(Qs_SaCod) !== 0) {
        for (let x = 0; x < elencoCelleMagazzini.length; x++) {
            // TODO sarebbe da testare anche il tipo magazzino in testa
            var tipoMagazzino = "";
            if (elencoCelleMagazzini[x].key_Dest.endsWith(tipoMagazzino + "_" + Qs_SaCod + "_" + xFabbricato_Cod)) {
                keyMag = elencoCelleMagazzini[x].key_Dest;
                break;
            }
        }
    } else {
        if (elencoCelleMagazzini.length === 2) {
            for (let x = 0; x < elencoCelleMagazzini.length; x++) {
                // TODO sarebbe da testare anche il tipo magazzino in testa
                if (elencoCelleMagazzini[x].key_Dest !== "") {
                    keyMag = elencoCelleMagazzini[x].key_Dest;
                    break;
                }
            }
        }
    }

    if (keyMag !== "") {
        Set_KendoDDLValueNoDef("ddlUbicProvenienza", keyMag);
        Set_KendoDDLValueNoDef("ddlUbicDestinazione", keyMag);
        $("#ddlUbicProvenienza").trigger('change');
        $("#ddlUbicDestinazione").trigger('change');
    }

    creaKendoDropDownList("ddlCategorieMagazzino", { read: RiempiElencoCategorieMagazzino }, "NomeComune", "Elem_Cod").bind("change", ddlCategorieMagazzino_change);

    // TODO sia per key e value che per caricamento in generale che per onchange
    creaKendoDropDownList("ddlLottoImpianto", { read: RiempiLotto }, "NomeComune", "Elem_Cod").bind("change", ddlLottoImpianto_change);
    // TODO sia per key e value che per caricamento in generale che per onchange
    creaKendoDropDownList("ddlLottoAccettazione", { read: RiempiLottoAccettazione }, "Lotto", "Lotto_Cod", null, null, $("#aggiuntaLottoAccettazioneTemplate").html()).bind("change", ddlLottoAccettazione_change);
    // TODO sia per key e value che per caricamento in generale che per onchange
    creaKendoDropDownList("ddlCalibro", { read: RiempiCalibro }, "Cal_Des", "Cal_Cod").bind("change", ddlCalibro_change);
    // TODO sia per key e value di default che per onchange
    creaKendoDropDownList("ddlUM", { read: RiempiElencoCategorieXUnitaMisuraOptimized }, "Udm_Des", "Udm_Cod").bind("change", ddlUM_change);
    // TODO sia per key e value di default che per onchange
    creaKendoDropDownList("ddlPUARegolamento", { read: RiempiElencoPUA_Regolamenti }, "Regolamento_DES", "Regolamento_Cod").bind("change", ddlPUARegolamento_change);

    if (lavCodAccettazione) {
        let w_txtLottoAccettazione = $("#txtLottoAccettazione");
        w_txtLottoAccettazione.bind("change", txtLottoAccettazione_change);
        w_txtLottoAccettazione.bind("focus", txtLottoAccettazione_focus);


        if (raccolteXConferimenti_AbilitazioneGenerale()) {
            let btnAssociaRaccolte = $("#raccolteXConferimenti_btnAssocia");
            btnAssociaRaccolte.on("click", raccolteXConferimenti_btnAssociaClick);

            raccolteConfUC_rimuoviEventoNessunaRaccoltaSel(btnAssociaRaccolte);
            raccolteConfUC_registraEventoNessunaRaccoltaSel(btnAssociaRaccolte, raccolteXConferimenti_btnAssociaNoRaccolteSel);

            raccolteConfUC_rimuoviEventoPrimaRaccoltaSel(btnAssociaRaccolte);
            raccolteConfUC_registraEventoPrimaRaccoltaSel(btnAssociaRaccolte, raccolteXConferimenti_btnAssociaPrimaRaccoltaSel);

            raccolteConfUC_rimuoviEventoRaccoltaRimossa(btnAssociaRaccolte);
            raccolteConfUC_registraEventoRaccoltaRimossa(btnAssociaRaccolte, raccolteXConferimenti_btnAssociaRaccoltaRimossa);
        }
    }

    $("#idTxt_N").kendoNumericTextBox({ decimals: 3, format: "0.###", min: 0 });
    $("#idTxt_P2O5").kendoNumericTextBox({ decimals: 3, format: "0.###", min: 0 });
    $("#idTxt_K2O").kendoNumericTextBox({ decimals: 3, format: "0.###", min: 0 });
    $("#idTxt_Cu").kendoNumericTextBox({ decimals: 3, format: "0.###", min: 0 });
    KendoNumTB("idTxt_N").enable(false);
    KendoNumTB("idTxt_P2O5").enable(false);
    KendoNumTB("idTxt_K2O").enable(false);
    KendoNumTB("idTxt_Cu").enable(false);

    $('input[name$="idPrezzo"]').kendoNumericTextBox({ decimals: 6, format: "0.00####€ ", change: idPrezzo_change });
    $('input[name$="idPrezzoNetto"]').kendoNumericTextBox({ decimals: 6, format: "0.00####€ " });
    KendoNumTB("idPrezzoNetto").enable(false);

    //creaKendoDropDownList("ddlListino", { read: RiempiElencoListini }, "descrizione", "id_anagrafica");
    if (!lavCodAccettazionePomodoro) {
        $("#btn_ricerca_prezzo_listino").show();
        $("#btn_ricerca_prezzo_listino").click(function () {
            Riepilogo_Prezzi_Listini(false);
        });
    }

    $('input[name$="idDegradoPerc"]').kendoNumericTextBox({ change: idDegradoPerc_change });

    $('input[name$="idScontoBase"]').kendoNumericTextBox({ decimals: 2, format: "0.00\\%", min: 0, max: 100, change: idScontoBase_change });
    $('input[name$="idScontoAddiz1"]').kendoNumericTextBox({ decimals: 2, format: "0.00\\%", min: 0, max: 100, change: idScontoAddiz1_change });
    $('input[name$="idScontoAddiz2"]').kendoNumericTextBox({ decimals: 2, format: "0.00\\%", min: 0, max: 100, change: idScontoAddiz2_change });
    $('input[name$="idScontoAddiz3"]').kendoNumericTextBox({ decimals: 2, format: "0.00\\%", min: 0, max: 100, change: idScontoAddiz3_change });
    $('input[name$="idScontoCalcolato"]').kendoNumericTextBox({ decimals: 3, format: "0.00\\%", min: 0, max: 100 });
    $('input[name$="idScontoCalcolatoEuro"]').kendoNumericTextBox({ decimals: 2, format: "0.00€ " });
    creaKendoDropDownList("ddlScontoModalita", { read: RiempiElencoScontoModalita }, "Sconto_Modalita_Descr", "Sconto_Modalita").bind("change", ddlScontoModalita_change);
    creaKendoDropDownList("ddlScontoMagg", { read: RiempiElencoScontoMaggiorazione }, "ScontoMaggiorazione_Descr", "ScontoMaggiorazione").bind("change", ddlScontoMaggiorazione_change);

    $('input[name$="idImponibileTotale"]').kendoNumericTextBox({ decimals: 2, format: "0.00€ ", change: idImponibileTotale_change });
    $('input[name$="idImponibileTotaleNetto"]').kendoNumericTextBox({ decimals: 2, format: "0.00€ ", change: idImponibileTotaleNetto_change });
    // TODO IVA e Conti
    let filtroDdlIva = [{ field: "Sigla_IVA" }, { field: "NaturaEsclusione_2" }];
    creaKendoDropDownList("ddlCodIva", { read: RiempiElencoIVA_Aliquote }, "Sigla_IVA", "Cod_IVA", "contains", filtroDdlIva, null, true, kendo.template($("#ddlCodIvaTemplate").html()), kendo.template($("#ddlCodIvaTemplate").html())).bind("change", ddlCodIva_change);
    $('input[name$="idIva"]').kendoNumericTextBox({ decimals: 2, format: "0.00€ ", change: idIva_change });

    creaKendoSwitch("chkForzaIva", "", "", false, chkForzaIva_change);

    $('input[name$="idImportoUnitario"]').kendoNumericTextBox({ decimals: 6, format: "0.00####€ ", change: idImportoUnitario_change });
    $('input[name$="idImportoTotale"]').kendoNumericTextBox({ decimals: 2, format: "0.00€ ", change: idImportoTotale_change });
    creaKendoDropDownList("ddlValoreRiferimento", { read: RiempiElencoValoreRiferimento }, "TempoCarenza_Descr", "TempoCarenza").bind("change", ddlValoreRiferimento_change);
    Set_KendoDDLValueNoDef("ddlValoreRiferimento", 0);
    KendoNumTB("idPrezzo").enable(true);
    KendoNumTB("idImportoUnitario").enable(false);
    KendoNumTB("idImportoTotale").enable(false);
    KendoNumTB("idImponibileTotale").enable(false);
    KendoNumTB("idImponibileTotaleNetto").enable(false);
    KendoNumTB("idScontoCalcolato").enable(false);
    KendoNumTB("idScontoCalcolatoEuro").enable(false);

    if (is_FF_FormProdottoUC()) {
        creaKendoDropDownList("ddlPrezzoRiferitoA", { read: RiempiElencoPrezzoLivelloFF }, "Prezzo_Livello_Descr", "Prezzo_Livello").bind("change", ddlPrezzoRiferitoA_change);
        Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", -1);
    } else {
        // TODO seconda UM gestita
        // if (!secondaUMGestita)
        creaKendoDropDownList("ddlPrezzoRiferitoA", { read: RiempiElencoPrezzoLivelloSoloQta }, "Prezzo_Livello_Descr", "Prezzo_Livello").bind("change", ddlPrezzoRiferitoA_change);
        // else
        //      creaKendoDropDownList("ddlPrezzoRiferitoA", { read: RiempiElencoPrezzoLivelloNoFF }, "Prezzo_Livello_Descr", "Prezzo_Livello").bind("change", ddlPrezzoRiferitoA_change);
        Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", 0);
    }

    creaKendoDropDownList("ddlAnno", { read: RiempiElencoAnniApertiConti }, "Anno_Descr", "Anno");
    // TODO IVA e Conti
    creaKendoDropDownList("ddlContoEconomico", { read: RiempiElencoContoEconomico }, "Descr_Conto", "Cod_Conto");
    creaKendoDropDownList("ddlContoPatrimoniale", { read: RiempiElencoContoPatrimoniale }, "Descr_Conto_Pat", "Cod_Conto_Pat");
    $('input[name$="idProvvigioni"]').kendoNumericTextBox();


    creaCampiFreshAndFood_FormProdottoUC();

    // disabilito / nascondo campi non richiesti da conferimento pomodoro
    if (lavCodAccettazionePomodoro) {
        KendoDDL("ddlCategorieMagazzino").enable(false);
        $("#txtTagliandoPesa").prop('required', true);
        $("#groupTagliandoPesa").show();
        KendoNumTB("idDegradoPerc").enable(false);
        KendoNumTB("idPrezzo").enable(false);
        KendoDDL("ddlPrezzoRiferitoA").enable(false);
        KendoDDL("ddlValoreRiferimento").enable(false);
        // $("#lblImponibileTotale").html("Prezzo Totale:");
        // $("#id_importi").hide();
        $("#btn_scelta_da_giacenza_formProdottoUC").hide();
        $("#cardIva").hide();
        $("#cardSconti").hide();
    }

    $("#btnImpostaValoriRiscontrati").on("click", btnImpostaPesiRiscontratiClick);
    $("#btnResettaValoriRiscontrati").on("click", btnResettaPesiRiscontratiClick);

    $("#dpDataScadenza").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),
        format: "dd/MM/yyyy",
        start: "decade",
        change: dpDataScadenza_change
    });

    // -----------------------------------------------------------------
    // Tenere questo per ultimo perché l'onChange delle categorie provoca pulizie di altri campi che vengono creati sopra
    // -----------------------------------------------------------------
    if ($('input[name$="hf_Categoria_Magazzino_Dft"]').val() !== undefined) {
        Set_KendoDDLValueNoDef("ddlCategorieMagazzino", $('input[name$="hf_Categoria_Magazzino_Dft"]').val());
        //$("#ddlCategorieMagazzino").trigger("change");
    }

    // -----------------------------------------------------------------
    // FINE Creazione di tutti i controlli della pagina di dettaglio
    // -----------------------------------------------------------------


    // Inizio - per selezionare il contenuto quando si clicca sul campo o ci si arriva con il tab
    $(".k-numerictextbox input[type=text]").on("focus", function () {
        var input = $(this);
        clearTimeout(input.data("selectTimeId")); //stop started time out if any

        var selectTimeId = setTimeout(function () {
            input.select();
            // To make this work on iOS, too, replace the above line with the following one. Discussed in https://stackoverflow.com/q/3272089
            // input[0].setSelectionRange(0, 9999);
        });

        input.data("selectTimeId", selectTimeId);
    }).blur(function (e) {
        clearTimeout($(this).data("selectTimeId")); //stop started timeout
    });
    // Fine - per selezionare il contenuto quando si clicca sul campo o ci si arriva con il tab
} 

function CreaDdlProdottoDes() {

    if (KendoDDL("ddlProdottoDes") !== undefined) {
        KendoDDL("ddlProdottoDes").destroy();
    }

    current_Prodotto_Cod = "";

    // N.B.  Viene ricreato ad ogni entrata sul dettaglio prodotto perché il Server Filtering diversamente in alcune occasioni non scatta (ad es. se entro su
    // nuova riga, effettuo una ricerca, faccio Annulla e poi rientro su nuova riga la ricerca non scatta più)

    switch (true) {

        case PropostaInnescoDaTrappola():
            $("#lblInserireCaratteri").hide();
            creaKendoDropDownList("ddlProdottoDes", { read: RicercaProdottiCompleto_InneschiDaTrappole }, "Prodotto_Des", "Prodotto_Cod", null, null, null, true, null, null, false).bind("change", ddlProdottoDes_change);
            break;

        case lavCodAccettazionePomodoro:
            $("#lblInserireCaratteri").hide();
            //creaKendoDropDownList(IDControllo, t, textfield, valuefield, filterType, filteringArray, noDataTemplate, autoBind, template, valueTemplate, autoWidth)
            creaKendoDropDownList("ddlProdottoDes", { read: RicercaProdottiCompleto_DocContGenerico }, "Prodotto_Des", "Prodotto_Cod", null, null, null, true, null, null, false).bind("change", ddlProdottoDes_change);
            break;

        default:
            //creaKendoDropDownListServerFiltering(NameControllo, _dataTextField, _dataValueField, functionRead, functionChange, minLength, defaultValue, defaultText, functionClose, autoWidth)
            creaKendoDropDownListServerFiltering("ddlProdottoDes", "Prodotto_Des", "Prodotto_Cod", RicercaProdottiCompleto_DocContGenerico, ddlProdottoDes_change,
                LunghezzaMinimaFiltroProdotto, "",
                TraduzioneMultiResx(resxFormProdottoUC, "SelezionaUnProdotto", "--- Seleziona un prodotto ---"),
                null,
                null,
                kendo.format(TraduzioneMultiResx(resxFormProdottoUC, "InserireNCaratteriNessunRisultatoPerDato", ""), LunghezzaMinimaFiltroProdotto));
            KendoDDL("ddlProdottoDes").bind("filtering", ddlProdottoDes_filtering);
            KendoDDL("ddlProdottoDes").bind("open", ddlProdottoDes_open);
            NoDataTemplateDefaultProdotto = KendoDDL("ddlProdottoDes").noDataTemplate;
            SeVisualizzaInserireCaratteri();
            break;

    }

}
 

// ---------------------------------
// --- Inizio Riempimento DDL ----
// ----------------------------------
function RiempiElencoCelleEMagazzini(options) {

    // Questo era preso dalla Form prodotto vecchia
    // Per ora sospeso in attesa di verificare se serve qualcosa del genere
    //if (Qs_CodContatto !== "" && isContattoImpresaGias) {
    //    var obj = {
    //        "key_Dest": " _ _ ",
    //        "Tipo_Destinazione": 0,
    //        "Sa_Cod": 0,
    //        "Id_Destinazione": 0,
    //        "Ubic_Des": "Nessuna Gestione del Magazzino"
    //    };
    //    elencoCelleMagazzini.push(obj);
    //}

    let elencoCelleMagazziniDaRestituire = elencoCelleMagazzini

    if (KendoDDL("ddlCategorieMagazzino") !== undefined && KendoDDL("ddlCategorieMagazzino").dataSource !== undefined) {

        if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length !== 0) {

            let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

            if (ddlCategorieMagazzinoValue !== 0) {

                if (is_Trasf_Veg_Anim_FormProdottoUC()) {

                    elencoCelleMagazziniDaRestituire = elencoCelle;

                } else {

                    elencoCelleMagazziniDaRestituire = elencoMagazzini;

                }

            }

        }

    }

    options.success(elencoCelleMagazziniDaRestituire);

}

function RiempiElencoTuttiProdotti(options) {
    options.success(elencoTuttiProdotti);
}

function RiempiElencoImballaggi(options) {
    options.success(elencoImballaggi);
}

function RiempiElencoContenitori(options) {
    options.success(elencoContenitori);
}

function RiempiElencoConfezioni(options) {
    options.success(elencoConfezioni);
}

function RiempiElencoIVA_Aliquote(options) {
    options.success(elencoIVA_Aliquote);
}

function RiempiElencoAnniApertiConti(options) {
    options.success(elencoAnniApertiConti);
}

function RiempiElencoContoEconomico(options) {
    options.success(elencoContoEconomico);
}

function RiempiElencoContoPatrimoniale(options) {
    options.success(elencoContoPatrimoniale);
}

function RiempiElencoCategorieMagazzino(options) {
    //options.success(elencoCategorie_Magazzino);
    // Vengono impostate lato server per considerare preferenze utente e default
    options.success(JSON.parse($('input[name$="hf_Categorie_Magazzino"]').val()));
}

function RiempiElencoCategorieXUnitaMisura(options) {
    options.success(elencoCategorieXUnitaMisura);
}

function RiempiElencoCategorieXUnitaMisuraOptimizedRegolamento(options) {
    options.success(elencoUdmOptimizedRegolamento);
}

function RiempiElencoCategorieXUnitaMisuraOptimized(options) {
    options.success(elencoUdmOptimized);
}

function RiempiElencoPUA_Regolamenti(options) {
    options.success(elencoPUA_Regolamenti);
}

function RiempiCausale_Riga(options) {
    options.success(elencoCausali_Riga);
}

function RiempiLotto(options) {
    // TODO
    //options.success(JSON.parse($('input[name$="hf_Categorie_Magazzino"]').val()));
    var ar = JSON.parse("[{}]");
    var objVuoto = {
        "NomeComune": "",
        "Elem_Cod": ""
    };
    ar.unshift(objVuoto);
    options.success(ar);
    //options.success(JSON.parse("[" + objVuoto + "]"));
}

function RiempiLottoAccettazione(options) {
    if (elencoLottiAccettazione_FormProdottoUC === null)
        elencoLottiAccettazione_FormProdottoUC = [];
    options.success(elencoLottiAccettazione_FormProdottoUC);
}

function RiempiCalibro(options) {
    // TODO
    //options.success(JSON.parse($('input[name$="hf_Categorie_Magazzino"]').val()));

    var ar = JSON.parse("[{}]");
    var objVuoto = {
        "Cal_Des": "",
        "Cal_Cod": 0
    };
    ar.unshift(objVuoto);
    options.success(ar);
}

// ------------------------------
// --- Fine Riempimento DDL ----
// ------------------------------ 