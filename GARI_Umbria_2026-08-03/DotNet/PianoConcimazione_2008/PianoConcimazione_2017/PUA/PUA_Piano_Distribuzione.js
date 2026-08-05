
//indica il tipo di operazione da aprire
var Enum_DistrLavCodLavorazione = {
    Organica: { value: 124, name: "Organica", code: 124 },
    Chimica: { value: 14, name: "Chimica", code: 14 }
};

//indica il tipo di operazione da aprire
var Enum_Modalita = {
    PianoDistribuzione: 0,
    Verifica: 1
};

//indica il tipo di operazione da aprire
var Enum_PuaTipo = {
    Completo: 1,
    Semplificato: 2
};

function Letamazioni(e) {

    let datiRiga = $(e.currentTarget).closest("div.k-grid").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));

    let regolamento_cod = parseInt($(cIdRegCod).val());
    let pua_cod = parseInt($(cIdPuaCod).val());
    let id = datiRiga.Id_AnagrafeVincoli;

    let param = JSON.stringify({
        regolamento_cod: regolamento_cod,
        pua_cod: pua_cod,
        id: id,
        piva: datiRiga.Piva,
        sa_cod: datiRiga.Sa_Cod,
        appezza: datiRiga.appezza,
        id_reg: datiRiga.id_reg,
        progetto_cod: datiRiga.Progetto_Cod,
        data_inizio: $(cIdDataInizio).val(),
        data_fine: $(cIdDataFine).val()
    });

    $.ajax({
        type: "POST",
        url: indirizzohttp + "/apriLetamazioni",
        data: param,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            apriLetamazioni(msg.d, datiRiga);
        }
    });

}

var N_fertilizzazioniPrecedenti = null;

function apriLetamazioni(url, datiRiga) {
    $(document.body).append('<div id="letamazioniWindow"></div>');
    $('#letamazioniWindow').kendoWindow({
        title: "Letamazioni Precedenti",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        close: function () {
            //il popup ha eseguito un salvataggio
            if (N_fertilizzazioniPrecedenti !== undefined && N_fertilizzazioniPrecedenti !== null) {

                //se il nuovo N è uguale al precedente, è inutile che aggiorno la riga
                if (parseFloat(N_fertilizzazioniPrecedenti) !== parseFloat(datiRiga.N_FertilizzazioniPrecedenti)) {

                    //update di quel valore sul model
                    let dataItem = gridPD.dataSource.getByUid(datiRiga.uid);
                    kendo_imposta_valore(dataItem, "N_FertilizzazioniPrecedenti", N_fertilizzazioniPrecedenti, "n2");

                    //Richiamo funzione di aggiornamento calcolo n fabbisogno (e n fabb complessivo) e li aggiorno sul model
                    let risp = calcolaFabbisognoN(datiRiga);

                    if (risp !== undefined && risp !== null && risp === true) {
                        //rileggo la riga modificata dal model
                        dataItem = gridPD.dataSource.getByUid(datiRiga.uid);

                        //Chiamo funzione apposita per aggiornare solo n fabbisogno organico in reg_impianti_codici
                        //let risp2 = 
                        aggiornaN_Organico(dataItem.uid,
                            dataItem.N_Fabbisogno,
                            dataItem.Sa_Cod,
                            dataItem.appezza,
                            dataItem.id_reg,
                            dataItem.Progetto_Cod);

                        //if (risp2 !== undefined && risp2 !== null && risp2 === true) {
                        //    //Dopo aver fatto update devo ridisegnare la riga e togliere il dirty...
                        //    let row = gridPD.tbody.find("tr[data-uid='" + uid + "']");
                        //    kendoFastRedrawRow(gridPD, row);
                        //    dataItem.dirtyFields = {};
                        //    dataItem.dirty = false;

                        //    //TODO: mi serve sta roba?!?
                        //    let hasChan = gridPD.dataSource.hasChanges();
                        //    gridPD.refresh();
                        //}
                    }
                }

            }

            $('#letamazioniWindow').kendoWindow('destroy');

        }
    }).data("kendoWindow").center();
}

function chiudiLetamazioni() {
    $('#letamazioniWindow').data('kendoWindow').close();
}

//Inizio Kendo

function popolaGrigliaPianoDistribuzione(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: RicercaGrigliaPianoDistribuzione
        //UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        //UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        //gestisciSalvataggioFinaleAParte: true
    };

    if (modalita === Enum_Modalita.PianoDistribuzione) {
        funzioniCRUD.funzioneUpdate = UpdatePianoDistribuzione;
        funzioniCRUD.checkBoxFunction = KendoOperazioni_checkedPlan;
    }

    var idModel = "Chiave";
    var campiKendoModel = {
        Chiave: { editable: false, type: "string" },

        Pua_Cod: { editable: false, type: "number" },
        Regolamento_Cod: { editable: false, type: "number" },
        Data_Pua: { editable: false, type: "date" },
        Id_AnagrafeVincoli: { editable: false, type: "number" },

        Appezzamento: { editable: false, type: "string" },
        Veg_Cod: { editable: false, type: "number" },

        //IMPORTANTE: lasciare editable = true nei campi che devono subire modifiche (come sabbia ed argilla), 
        // anche se la colonna non è visibile, perché altrimenti non si aggiornano i dati!!!

        StatoImpiantoCod: { editable: false, type: "number" },
        StatoImpiantoDes: { editable: false, type: "string" },
        Ciclo: { editable: true, type: "number" },
        CicloDes: { editable: true, type: "string" },

        Grfi_Cod_Concimazione: { editable: true, type: "number" },
        Grfi_Des_Concimazione: { editable: true, type: "string" },
        B_Perc: { editable: true, type: "number" },

        PrecessioneCod: { editable: true, type: "number" },
        PrecessioneDes: { editable: true, type: "string" },
        AnalisiTestataCod: { editable: true, type: "number" },
        AnalisiTestataDes: { editable: true, type: "string" },
        So: { editable: true, type: "number" },
        Sabbia: { editable: true, type: "number" },
        Argilla: { editable: true, type: "number" },

        N_Distribuito: { editable: true, type: "number" },
        N_FertilizzazioniPrecedenti: { editable: true, type: "number" },

        UbicazioneCod: { editable: true, type: "number" },
        UbicazioneDes: { editable: true, type: "string" },
        TipoAcquaCod: { editable: true, type: "number" },
        TipoAcquaDes: { editable: true, type: "string" },

        Resa: { editable: true, type: "number" },
        Resa_Rif: { editable: true, type: "number" },
        FattoreCorrettivo_N: { editable: true, type: "number" },

        Assorbimento: { editable: true, type: "number" },

        Catasto: { editable: false, type: "string" },
        Superficie: { editable: false, type: "number" },
        DurataColtura: { editable: false, type: "string" },
        ZVN: { editable: false, type: "boolean" },
        LimiteMas: { editable: true, type: "number" },
        N_Fabbisogno_Database: { editable: false, type: "number" },
        N_Fabbisogno: { editable: true, type: "number" },
        N_FabbisognoComplessivo: { editable: true, type: "number" },
        N_FabbisognoSoddisfatto: { editable: false, type: "number" },
        N_Zootecnico: { editable: false, type: "number" },
        N_Zootecnico_Letame: { editable: false, type: "number" },
        N_Zootecnico_Liquame: { editable: false, type: "number" },
        N_TotaleSoddisfatto: { editable: false, type: "number" },
        N_BilancioAzotato_Utile: { editable: false, type: "number" },
        Valutazione_NUtile: { editable: true, type: "number" },
        N_BilancioAzotato_Totale: { editable: false, type: "number" },
        Valutazione_NTotale: { editable: true, type: "number" },
        Indice_Efficienza_Azotata: { editable: false, type: "number" },
        Valutazione_Efficienza: { editable: true, type: "number" },
        ValiditaInizio: { editable: false, type: "date" },
        ValiditaFine: { editable: false, type: "date" }
    };
    var colonneKendoGrid = [
        //{ field: "Chiave", title: "Chiave", hidden: true},
        { field: "Appezzamento", title: "Appezzamento", width: "96px", filterable: { multi: true, search: true } },
        { field: "Catasto", title: "Catasto", template: "#=splitString(Catasto, '|')#", width: "200px", filterable: { multi: true, search: true } },
        { field: "Superficie", title: "Superficie [Ha]", format: "{0:n4}", attributes: { style: "text-align:right;" }, width: "102px", aggregates: ["sum"], footerTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n4')#</div>", groupHeaderColumnTemplate: "<div style='text-align: right'>#= kendo.toString(sum, 'n2')#</div>" }
    ];


    if (modalita === Enum_Modalita.PianoDistribuzione) {

        if (pua_tipo === Enum_PuaTipo.Completo) {

            colonneKendoGrid.push(
                { field: "DurataColtura", title: "Durata coltura", template: "#=splitString(DurataColtura, '|')#", width: "91px", filterable: { multi: true, search: true } },
                { field: "StatoImpiantoDes", title: "Stato impianto", width: "94px", filterable: { multi: true, search: true } },
                { field: "CicloDes", title: "Ciclo", editor: Ciclo_DropDownEditor, width: "113px" },
                { field: "AnalisiTestataDes", title: "Analisi", editor: AnalisiTerreno_DropDownEditor, width: "113px" },
                { field: "Resa", title: "Resa [t/Ha]", format: "{0:n2}", editor: Resa_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "78px" },
                { field: "Grfi_Des_Concimazione", title: "Tipologia", editor: FinalitaRER_DropDownEditor, width: "113px" },
                { field: "B_Perc", title: "Coefficiente B", template: "#= B_Perc >= 0 ? kendo.format('{0:n2}', B_Perc) : '' #", editor: BPerc_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px" },
                { field: "PrecessioneDes", title: "Precessione", editor: Precessione_DropDownEditor, width: "112px" },
                {
                    command: [
                        {
                            name: "LetamazioniPrecedenti", text: "Letamazioni Precedenti", className: "LetamazioniPrecedenti",
                            click: function (e) {
                                Letamazioni(e);
                            }
                        }
                    ],
                    title: "", width: "130px"
                },
                { field: "N_FertilizzazioniPrecedenti", title: "N Letamazioni Precedenti [Kg/Ha]", format: "{0:n2}", editor: NFertilizzazioniPrecedenti_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "89px" },
                { field: "UbicazioneDes", title: "N Atmosferico - Ubicazione", editor: Ubicazione_DropDownEditor, width: "107px" },
                { field: "TipoAcquaDes", title: "N Irriguo - Acqua utilizzata", editor: TipoAcqua_DropDownEditor, width: "120px" },
                {
                    field: "LimiteMas",
                    title: "N Massimo da " + lblMAS + " [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b> <br> da <b>" + lblMAS + "</b> [Kg/Ha]</span>",
                    template: "#= LimiteMas >= 0 ? kendo.format('{0:n2}', LimiteMas) : '' #", editor: LimiteMas_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_Fabbisogno_Database",
                    title: "N Massimo distribuibile da Piano Concimazione [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b><br> distribuibile <br>da <b>Piano Concimazione</b> [Kg/Ha]</span>",
                    template: "#= N_Fabbisogno_Database >= 0 ? kendo.format('{0:n2}', N_Fabbisogno_Database) : '' #", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_Fabbisogno",
                    title: "N Massimo distribuibile da PUA [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b> <br>distribuibile<br> da <b>PUA</b> [Kg/Ha]</span>",
                    format: "{0:n2}", editor: NFabbisogno_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                //{

                //    command: [
                //        { name: "associaN", text: "Associa N Massimo PUA all'Appezzamento", click: sovrascriviN }
                //    ],
                //    title: "", width: "147px"
                //},
                //    { field: "N_FabbisognoComplessivo", title: "Fabbisogno di N complessivo (Kg)", format: "{0:n2}", editor: NFabbisognoComplessivo_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "114px" },
                {
                    field: "N_FabbisognoSoddisfatto",
                    title: "N Utile ((kc*Fc)+(Ko*Fo)) distribuito [Kg/Ha]",
                    headerTemplate: "<span><b>N Utile</b><br> ((kc*Fc)+(Ko*Fo))<br> distribuito [Kg/Ha]</span>",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_Zootecnico",
                    title: "N Zootecnico distribuito [Kg/Ha]",
                    headerTemplate: "<span><b>N Zootecnico</b><br> distribuito [Kg/Ha]</span>",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "106px"
                }
            );
        }
        else {
            colonneKendoGrid.push(
                { field: "DurataColtura", title: "Durata coltura", template: "#=splitString(DurataColtura, '|')#", width: "91px", filterable: { multi: true, search: true } },
                { field: "StatoImpiantoDes", title: "Stato impianto", width: "94px", filterable: { multi: true, search: true } },
                { field: "CicloDes", title: "Ciclo", editor: Ciclo_DropDownEditor, width: "113px" },
                { field: "Grfi_Des_Concimazione", title: "Tipologia", editor: FinalitaRER_DropDownEditor, width: "113px" },
                { field: "Resa_Rif", title: "Resa Rif. [t/Ha]", format: "{0:n2}", template: "#= Resa_Rif >= 0 ? kendo.format('{0:n2}', Resa_Rif) : '' #", editor: ResaRif_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "78px" },
                { field: "Resa", title: "Resa [t/Ha]", format: "{0:n2}", editor: Resa_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "78px" },
                { field: "FattoreCorrettivo_N", title: "Fattore Correttivo [Kg N/t]", format: "{0:n2}", template: "#= FattoreCorrettivo_N >= 0 ? kendo.format('{0:n2}', FattoreCorrettivo_N) : '' #", editor: FattoreCorrettivoN_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "78px" },
                {
                    field: "LimiteMas",
                    title: "N Massimo da " + lblMAS + " [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b> <br> da <b>" + lblMAS + "</b> [Kg/Ha] </span>",
                    template: "#= LimiteMas >= 0 ? kendo.format('{0:n2}', LimiteMas) : '' #", editor: LimiteMas_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                { field: "PrecessioneDes", title: "Precessione", editor: Precessione_DropDownEditor, width: "112px" },
                {
                    command: [
                        {
                            name: "LetamazioniPrecedenti", text: "Letamazioni Precedenti", className: "LetamazioniPrecedenti",
                            click: function (e) {
                                Letamazioni(e);
                            }
                        }
                    ],
                    title: "", width: "130px"
                },
                { field: "N_FertilizzazioniPrecedenti", title: "N Letamazioni Precedenti [Kg/Ha]", format: "{0:n2}", editor: NFertilizzazioniPrecedenti_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "89px" },
                {
                    field: "N_Fabbisogno_Database",
                    title: "N Massimo distribuibile da Piano Concimazione [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b><br> distribuibile <br>da <b>Piano Concimazione</b> [Kg/Ha]</span>",
                    template: "#= N_Fabbisogno_Database >= 0 ? kendo.format('{0:n2}', N_Fabbisogno_Database) : '' #", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_Fabbisogno",
                    title: "N Massimo distribuibile da PUA [Kg/Ha]",
                    headerTemplate: "<span><b>N Massimo</b> <br>distribuibile<br> da <b>PUA</b> [Kg/Ha]</span>",
                    format: "{0:n2}", editor: NFabbisogno_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_FabbisognoSoddisfatto",
                    title: "N Utile ((kc*Fc)+(Ko*Fo)) distribuito [Kg/Ha]",
                    headerTemplate: "<span><b>N Utile</b><br> ((kc*Fc)+(Ko*Fo))<br> distribuito [Kg/Ha]</span>",
                    format: "{0:n2}",
                    headerAttributes: { style: "text-align: right" },
                    attributes: { style: "text-align:right;" },
                    width: "110px"
                },
                {
                    field: "N_Zootecnico",
                    title: "N Zootecnico distribuito [Kg/Ha]",
                    headerTemplate: "<span><b>N Zootecnico</b> distribuito [Kg/Ha]</span>",
                    format: "{0:n2}",
                    headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "106px"
                }
            );
        }


    } else {

        if (pua_tipo === Enum_PuaTipo.Completo) {
            colonneKendoGrid.push(
                {
                    field: "Assorbimento",
                    title: "Asporto (Y*b)",
                    headerTemplate: "<b>Asporto</b> <br>(Y*b)",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_Fabbisogno",
                    title: "N Massimo distribuibileda PUA [Kg/Ha]",
                    headerTemplate: "<b>N Massimo</b> <br>distribuibile<br> da <b>PUA</b> [Kg/Ha]",
                    format: "{0:n2}", editor: NFabbisogno_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_FabbisognoSoddisfatto",
                    title: "N Utile ((kc*Fc)+(Ko*Fo)) distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Utile</b><br> ((kc*Fc)+(Ko*Fo))<br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_BilancioAzotato_Utile",
                    title: "Bilancio N Utile ((kc*Fc)+(Ko*Fo))-((Y*b)-(Nc+Nf+Nn+Norg)) [Kg/Ha]",
                    headerTemplate: "<b>Bilancio N Utile</b><br> ((kc*Fc)+(Ko*Fo))-((Y*b)-(Nc+Nf+Nn+Norg))<br> [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "Valutazione_NUtile",
                    title: "Valutazione N Utile",
                    headerTemplate: "Valutazione<br><b>N Utile</b>",
                    width: "94px", template: kendo.template($("#Valutazione_Template_NUtile").html())
                },
                {
                    field: "N_TotaleSoddisfatto",
                    title: "N Totale (Fc+Fo) distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Totale</b><br> (Fc+Fo)<br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_BilancioAzotato_Totale",
                    title: "Bilancio N Totale  ((Fc+Fo)-((Y*b)-(Nc+Nf+Nn+Norg)) [Kg/Ha]",
                    headerTemplate: "<b>Bilancio N Totale</b><br>  ((Fc+Fo)-((Y*b)-(Nc+Nf+Nn+Norg))<br> [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "Valutazione_NTotale",
                    title: "Valutazione N Totale",
                    headerTemplate: "Valutazione<br><b>N Totale</b>",
                    width: "94px", template: kendo.template($("#Valutazione_Template_NTotale").html())
                },
                {
                    field: "Indice_Efficienza_Azotata",
                    title: "Indice Eff. Azotata media [%]",
                    headerTemplate: "<b>Indice Eff. Azotata media</b> [%]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "Valutazione_Efficienza",
                    title: "Valutazione Indice Eff.",
                    headerTemplate: "Valutazione<br><b>Indice Eff.</b>",
                    width: "94px", template: kendo.template($("#Valutazione_Template_Efficienza").html())
                },
                {
                    field: "N_Zootecnico",
                    title: "N Zootecnico distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Zootecnico</b><br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "106px"
                }
            );
        }
        else {
            colonneKendoGrid.push(
                {
                    field: "N_Fabbisogno",
                    title: "N Massimo distribuibile da PUA [Kg/Ha]",
                    headerTemplate: "<b>N Massimo</b> <br>distribuibile<br> da <b>PUA</b> [Kg/Ha]",
                    format: "{0:n2}", editor: NFabbisogno_NumericEditor, headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_FabbisognoSoddisfatto",
                    title: "N Utile ((kc*Fc)+(Ko*Fo)) distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Utile</b><br> ((kc*Fc)+(Ko*Fo))<br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_BilancioAzotato_Utile",
                    title: "Bilancio N Utile ((kc*Fc)+(Ko*Fo))-(MAS) [Kg/Ha]",
                    headerTemplate: "<b>Bilancio N Utile</b><br> ((kc*Fc)+(Ko*Fo))-(MAS)<br> [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "Valutazione_NUtile",
                    title: "Valutazione N Utile",
                    headerTemplate: "Valutazione<br><b>N Utile</b>",
                    width: "94px", template: kendo.template($("#Valutazione_Template_NUtile").html())
                },
                {
                    field: "N_TotaleSoddisfatto",
                    title: "N Totale (Fc+Fo) distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Totale</b><br> (Fc+Fo)<br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "N_BilancioAzotato_Totale",
                    title: "Bilancio N Totale  (Fc+Fo)-(MAS) [Kg/Ha]",
                    headerTemplate: "<b>Bilancio N Totale</b><br>  (Fc+Fo)-(MAS)<br> [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "110px"
                },
                {
                    field: "Valutazione_NTotale",
                    title: "Valutazione N Totale",
                    headerTemplate: "Valutazione<br><b>N Totale</b>",
                    width: "94px", template: kendo.template($("#Valutazione_Template_NTotale").html())
                },
                {
                    field: "N_Zootecnico",
                    title: "N Zootecnico distribuito [Kg/Ha]",
                    headerTemplate: "<b>N Zootecnico</b><br> distribuito [Kg/Ha]",
                    format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;" }, width: "106px"
                }
            );
        }
    }

    var parametriPerLettura = null;
    var parametriDataSource = { aggregate: [{ field: "Superficie", aggregate: "sum" }] };
    var parametriKendoGrid = {
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable:
        {
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        filterable: true,
        //toolbarCommands: ["templateBtn_ApriBrogliaccio", "templateBtn_ApriQdC", "templateBtn_ApriAnalisi"],
        toolbarCommands: ["templateBtn_ApriAnalisi", "templateBtn_ApriBrogliaccio"],
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS }, non eliminare perchè stessa griglia utilizzata per 2 pagine diverse
        editable: {
            mode: "popup",
            window: {
                title: "Modifica"
            }
        }
    };

    if ((modalita === Enum_Modalita.PianoDistribuzione) && (pua_bloccato == 0)) {


        if (($(cIdImportaDaQdC).val()) === 'True') {
            parametriKendoGrid.toolbarCommands.push("templateBtn_ApriQdC");
        }

        //if (pua_tipo === Enum_PuaTipo.Completo) {
        parametriKendoGrid.toolbarCommands.push("templateBtn_ModificaMultipla");
        parametriKendoGrid.toolbarCommands.push("templateBtn_AssegnaDefault");
        //}

        parametriKendoGrid.toolbarCommands.push("templateBtn_AssociaN");
        parametriKendoGrid.toolbarCommands.push("templateBtn_AssociaNMas");
        parametriKendoGrid.checkSelezioneRiga = { filterable: false, field: null, width: "30px" };

        parametriKendoGrid.colonneCustomKendoGrid = [
            {
                command: [
                    {
                        className: "ModificaApp btnWidth",
                        name: "edit",
                        text: {
                            edit: "Modifica",
                            update: "Conferma Dati",
                            cancel: "Annulla"
                        },
                        iconClass: ""
                        //iconClass: "fa fa-pencil-square-o fa-2x",
                        //className: "btn2icon"
                    },
                    {
                        //iconClass: "fa fa-external-link fa-lg",//fa-pencil-square-o fa-external-link
                        className: "e_link btnWidth",
                        name: "e_link",
                        text: "Fert. Chimica",
                        click: function (e) {
                            NuovaFertilizzazione(Enum_DistrLavCodLavorazione.Chimica, e);
                        }
                    },
                    {
                        //iconClass: "fa fa-external-link fa-lg",//fa-pencil-square-o fa-external-link
                        className: "a_link btnWidth",
                        name: "a_link",
                        text: "Fert. Organica",
                        click: function (e) {
                            NuovaFertilizzazione(Enum_DistrLavCodLavorazione.Organica, e);
                        }
                    }
                ],
                title: "Operazioni",
                width: "98px"
            }
        ];
    }


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: PianoDistribuzione_onDataBoundRighe,
        funzioneDaChiamareDopoEdit: PianoDistribuzione_onEdit
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    gridPD = $("#" + IDControllo).data("kendoGrid");
}

function PianoDistribuzione_onDataBoundRighe(e) {

    if (modalita === Enum_Modalita.PianoDistribuzione) {
        nascondiPulsantiTerreniNudi(e);
    }



}

function nascondiPulsantiTerreniNudi(eventArgs) {

    var grid = eventArgs.sender;
    var items = eventArgs.sender.items();

    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        if (dataItem.Veg_Cod <= 0) {
            $(this.querySelector(".ModificaApp")).hide();
            $(this.querySelector(".e_link")).hide();
            $(this.querySelector(".a_link")).hide();
            $(this.querySelector(".LetamazioniPrecedenti")).hide();
        }

    });

}


function PianoDistribuzione_onEdit(e) {
    necessarioRicalcolo = false;

    PopupTemplatePopulation(e);

    addRicalcolaButton(e);

    //Nascondo il checkbox di selezione
    e.container.prevObject.find("input[id='gridPD_" + e.model.id + "']").parent().hide();
    e.container.prevObject.find("label[for='null']").parent().hide();

    let FertilizzazioniPrecedentiNumTB = KendoNumTB("txtNFertilizzazioniPrecedenti");
    if (FertilizzazioniPrecedentiNumTB !== undefined && FertilizzazioniPrecedentiNumTB !== null)
        FertilizzazioniPrecedentiNumTB.enable(false);

    let NFabbisognoNumTB = KendoNumTB("txtNFabbisogno");
    if (NFabbisognoNumTB !== undefined && NFabbisognoNumTB !== null)
        NFabbisognoNumTB.enable(false);

    $("#txtNFabbisognoComplessivo").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

    $("#txtBPerc").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);
    $("#txtN").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);
    
    let ResaRifNumTB = KendoNumTB("txtResaRif");
    if (ResaRifNumTB !== undefined && ResaRifNumTB !== null) 
        ResaRifNumTB.enable(false);

    let FattoreCorrettivoNNumTB = KendoNumTB("txtFattoreCorrettivoN");
    if (FattoreCorrettivoNNumTB !== undefined && FattoreCorrettivoNNumTB !== null)
        FattoreCorrettivoNNumTB.enable(false);
}

function addRicalcolaButton(e) {
    let grid = e.sender;
    let table = grid.table;
    let uid = e.model.uid;

    $('<a role="button" class="k-button k-button-icontext calcola" href="#" onclick="ricalcola()"><span class="k-icon k-i-calculator"></span>Calcola Fabbisogno</a>').insertBefore(".k-grid-update");

    //$(".calcola").click(function (e) {
    //    e.preventDefault();
    //    var elem = $(table).find("tr[data-uid='" + uid + "']");
    //    //grid.removeRow(elem);
    //});
    //$(".calcola").click(ricalcola(e));
}

function ricalcola(e) {

    let table = gridPD.table;

    if (necessarioRicalcolo === true) {
        let model = gridPD.editable.options.model;
        let risp = calcolaFabbisognoN(model);

        if (risp !== undefined && risp !== null && risp === true)
            necessarioRicalcolo = false;
    }
}

function AggiornaModelloPerN(uid, objN) {

    let dataItem = gridPD.dataSource.getByUid(uid);

    kendo_imposta_valore(dataItem, "N_Fabbisogno", objN.Fabbisogno_N, "n2");
    kendo_imposta_valore(dataItem, "N_FabbisognoComplessivo", objN.Fabbisogno_N_Complessivo, "n2");
}

function PopupTemplatePopulation(e) {

    $("#" + cmbCicloNome).data("kendoDropDownList").dataSource.read();
    $("#" + cmbPrecessioneNome).data("kendoDropDownList").dataSource.read();
    $("#" + cmbFinalitaRERNome).data("kendoDropDownList").dataSource.read();

    if (pua_tipo === Enum_PuaTipo.Completo) {
        $("#" + cmbAnalisiTerrenoNome).data("kendoDropDownList").dataSource.read();
        $("#" + cmbUbicazioneNome).data("kendoDropDownList").dataSource.read();
        $("#" + cmbTipoAcquaNome).data("kendoDropDownList").dataSource.read();
    }
}

function KendoOperazioni_checkedPlan(e) {
    let checked = this.checked;
    let row = $(this).parents("tr");
    //let grid = $('#kendo_Appezzamenti').data("kendoGrid");
    let dataItem = gridPD.dataItem(row);
    dataItem.Selected = checked;
    //dataItem.dirty = true;
    if (checked) {
        console.log("CHECK");
        row.addClass(GIAS_K_STATE_SELECTED);
    } else {
        console.log("UNCHECK");
        row.removeClass(GIAS_K_STATE_SELECTED);
    }
}

function Ciclo_DropDownEditor(container, options) {

    creaDropDownEditorId(container, "Ciclo_Des", "Ciclo_Cod", ddlCiclo, changeCiclo,
        cmbCicloNome, "Ciclo");

}

function changeCiclo(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("Ciclo", parseInt(dataItem.Ciclo_Cod));
        model.set("CicloDes", dataItem.Ciclo_Des);

        necessarioRicalcolo = true;
    }
}

function AnalisiTerreno_DropDownEditor(container, options) {

    let piva = $(cIdPiva).val();
    let saCod = options.model.Sa_Cod;
    let campoCod = options.model.Campo_Cod;
    let appezza = options.model.appezza;
    let idReg = options.model.id_reg;
    let filtroParticelle = JSON.stringify(options.model.ParticelleVincoli);

    ddlAnalisiTerreno = riempiDdlAnalisiTerreno(piva, saCod, campoCod, appezza, idReg, filtroParticelle);

    creaDropDownEditorId(container, "nome", "analisi_testata_cod", ddlAnalisiTerreno, changeAnalisiTerreno,
        cmbAnalisiTerrenoNome, "AnalisiTestataCod");

}

function changeAnalisiTerreno(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("AnalisiTestataCod", parseInt(dataItem.analisi_testata_cod));
        model.set("AnalisiTestataDes", dataItem.nome);

        model.set("Argilla", dataItem.Argilla);
        model.set("Sabbia", dataItem.Sabbia);
        model.set("So", dataItem.SostanzaOrganica);

        necessarioRicalcolo = true;
    }
}



function FinalitaRER_DropDownEditor(container, options) {

    let regCod = parseInt($(cIdRegCod).val());
    let vegCod = options.model.Veg_Cod;
    let grfiCod = options.model.Grfi_Cod;

    ddlFinalitaRER = riempiDdlFinalitaRER(regCod, vegCod, grfiCod);

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlFinalitaRER, changeFinalitaRER,
        cmbFinalitaRERNome, "Grfi_Cod_Concimazione");

    //changeFinalitaRER(e);
}



function changeFinalitaRER(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {

        let model = gridPD.editable.options.model;

        model.set("Grfi_Cod_Concimazione", parseInt(dataItem.Codice));
        model.set("Grfi_Des_Concimazione", dataItem.Descrizione);
        model.set("B_Perc", parseFloat(dataItem.BPerc));
        model.set("LimiteMas", parseFloat(dataItem.N));
        model.set("Resa_Rif", parseFloat(dataItem.Resa));
        model.set("FattoreCorrettivo_N", parseFloat(dataItem.FattoreCorrettivoN));

        necessarioRicalcolo = true;
    }
}

function changeFinalitaRER_Mod(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {

        if (Txt_Mod_BPerc === undefined) {
            Txt_Mod_BPerc = $("#Txt_Mod_BPerc").kendoNumericTextBox({
                decimals: 2,
                spinners: false
            }).data("kendoNumericTextBox");
        }
        Txt_Mod_BPerc.value(dataItem.BPerc);
        KendoNumTB("Txt_Mod_BPerc").enable(false);
        // $("#Txt_Mod_BPerc").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

        if (Txt_Mod_Mas === undefined) {
            Txt_Mod_Mas = $("#Txt_Mod_Mas").kendoNumericTextBox({
                decimals: 2,
                spinners: false
            }).data("kendoNumericTextBox");
        }
        Txt_Mod_Mas.value(dataItem.N);
        KendoNumTB("Txt_Mod_Mas").enable(false);
        // $("#Txt_Mod_Mas").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

        if (Txt_Mod_ResaRif === undefined) {
            Txt_Mod_ResaRif = $("#Txt_Mod_ResaRif").kendoNumericTextBox({
                decimals: 2,
                spinners: false
            }).data("kendoNumericTextBox");
        }
        Txt_Mod_ResaRif.value(dataItem.Resa);
        KendoNumTB("Txt_Mod_ResaRif").enable(false);
        // $("#Txt_Mod_ResaRif").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

        if (Txt_Mod_FattoreCorrettivo === undefined) {
            Txt_Mod_FattoreCorrettivo = $("#Txt_Mod_FattoreCorrettivo").kendoNumericTextBox({
                decimals: 2,
                spinners: false
            }).data("kendoNumericTextBox");
        }
        Txt_Mod_FattoreCorrettivo.value(dataItem.FattoreCorrettivoN);
        KendoNumTB("Txt_Mod_FattoreCorrettivo").enable(false);
        // $("#Txt_Mod_FattoreCorrettivo").prop("disabled", true).addClass(GIAS_K_STATE_DISABLED);

    }

}

function Precessione_DropDownEditor(container, options) {

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlPrecessione, changePrecessione,
        cmbPrecessioneNome, "PrecessioneCod");

}

function changePrecessione(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("PrecessioneCod", parseInt(dataItem.Codice));
        model.set("PrecessioneDes", dataItem.Descrizione);

        necessarioRicalcolo = true;
    }
}




function FertilizzanteOrganico_DropDownEditor(container, options) {

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlFertilizzanteOrganico, changeFertilizzanteOrganico,
        cmbFertilizzanteOrganicoNome, "FertOrganicoCod");

}

function changeFertilizzanteOrganico(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("FertOrganicoCod", parseInt(dataItem.Codice));
        model.set("FertOrganicoDes", dataItem.Descrizione);

        necessarioRicalcolo = true;
    }
}

function Frequenza_DropDownEditor(container, options) {

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlFrequenza, changeFrequenza,
        cmbFrequenzaNome, "FrequenzaCod");

}

function changeFrequenza(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("FrequenzaCod", parseInt(dataItem.Codice));
        model.set("FrequenzaDes", dataItem.Descrizione);

        necessarioRicalcolo = true;
    }
}

function Ubicazione_DropDownEditor(container, options) {

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlUbicazione, changeUbicazione,
        cmbUbicazioneNome, "UbicazioneCod");

}

function changeUbicazione(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("UbicazioneCod", parseInt(dataItem.Codice));
        model.set("UbicazioneDes", dataItem.Descrizione);

        necessarioRicalcolo = true;
    }
}

function TipoAcqua_DropDownEditor(container, options) {

    let tipoZona = "";
    if (options.model.ZVN === true) {
        tipoZona = "v";
    } else {
        tipoZona = "n";
    }

    creaDropDownEditorId(container, "Descrizione", "Codice", ddlTipoAcqua[tipoZona], changeTipoAcqua,
        cmbTipoAcquaNome, "TipoAcquaCod");

}

function changeTipoAcqua(e) {
    let dataItem = e.sender.dataItem();

    if (dataItem !== undefined && dataItem !== null) {
        // Imposto la descrizione ed il codice nel modello dei dati leggendo dall'elemento selezionato            
        let model = gridPD.editable.options.model;

        model.set("TipoAcquaCod", parseInt(dataItem.Codice));
        model.set("TipoAcquaDes", dataItem.Descrizione);

        necessarioRicalcolo = true;
    }
}

function Resa_NumericEditor(container, options) {

    //editKendoNumericTextBox(container, options);

    $('<input id="txtResa" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: true
            //change: changeResa
        }).off("keydown");

    let myInput = container.find("#txtResa");
    myInput.bind("change", changeResa);
}

function changeResa(e) {
    let dataItem = e.currentTarget;

    if (dataItem !== undefined && dataItem !== null) {
        let model = gridPD.editable.options.model;
        if (dataItem.value === undefined || dataItem.value === null || dataItem.value === "") {
            dataItem.value = 0;
        }

        //model.Resa = parseFloat(dataItem.value);
        model.set("Resa", parseFloat(dataItem.value));

        necessarioRicalcolo = true;
        //calcolaFabbisognoN(model);
    }
}

function NDistribuito_NumericEditor(container, options) {

    //editKendoNumericTextBox(container, options);

    $('<input id="txtNDistribuito" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: true
            //change: changeNDistribuito
        }).off("keydown");

    let myInput = container.find("#txtNDistribuito");
    myInput.bind("change", changeNDistribuito);
}

function changeNDistribuito(e) {
    let dataItem = e.currentTarget;

    if (dataItem !== undefined && dataItem !== null) {
        let model = gridPD.editable.options.model;
        if (dataItem.value === undefined || dataItem.value === null || dataItem.value === "") {
            dataItem.value = 0;
        }

        //model.N_Distribuito = parseFloat(dataItem.value);
        model.set("N_Distribuito", parseFloat(dataItem.value));

        necessarioRicalcolo = true;
        //calcolaFabbisognoN(model);
    }
}

function NFertilizzazioniPrecedenti_NumericEditor(container, options) {

    //editKendoNumericTextBox(container, options);

    $('<input id="txtNFertilizzazioniPrecedenti" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
            //change: changeNFertilizzazioniPrecedenti
        }).off("keydown");

    //let myInput = container.find("#txtNFertilizzazioniPrecedenti");
    //myInput.bind("change", changeNFertilizzazioniPrecedenti);
}

function NFabbisogno_NumericEditor(container, options) {

    //editKendoNumericTextBox(container, options);

    $('<input id="txtNFabbisogno" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
            //change: changeNDistribuito
        }).off("keydown");

}

function NFabbisognoComplessivo_NumericEditor(container, options) {

    //editKendoNumericTextBox(container, options);

    $('<input id="txtNFabbisognoComplessivo" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
            //change: changeNDistribuito
        }).off("keydown");

    //let myInput = container.find("#txtNFabbisognoComplessivo");
    //myInput.bind("change", changeNFabbisogno);
}

function BPerc_NumericEditor(container, options) {

    $('<input id="txtBPerc" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
        }).off("keydown");

}

function ResaRif_NumericEditor(container, options) {

    $('<input id="txtResaRif" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
        }).off("keydown");

}
function FattoreCorrettivoN_NumericEditor(container, options) {

    $('<input id="txtFattoreCorrettivoN" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
        }).off("keydown");

}
function LimiteMas_NumericEditor(container, options) {

    $('<input id="txtLimiteMas" data-type="number" data-bind="value:' + options.field + '" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: options.format,
            spinners: false
        }).off("keydown");

}

//Fine Kendo

function splitString(field, separator) {
    if (field === null)
        return "";
    field = field.trim();
    if (field.length === 0)
        return "";
    let errs = field.split(separator);

    if (errs.length > 1) {
        return errs.join("<br />");
    } else {
        return field;
    }
}



/**
 * Nuova ricetta
 * @param {any} Lavorazione lav_Cod
 * @param {any} e kendo  
 */
function NuovaFertilizzazione(Lavorazione, e) {

    let datiRiga = $(e.currentTarget).closest("div.k-grid").data("kendoGrid").dataItem($(e.currentTarget).closest("tr"));

    if (Lavorazione.value === Enum_DistrLavCodLavorazione.Organica.value && datiRiga.AnalisiTestataCod === 0 && pua_tipo === Enum_PuaTipo.Completo) {
        kendo.alert("Attenzione, prima di eseguire una fertilizzazione organica è necessario impostare l'analisi di riferimento");
        return;
    }

    ajaxAgronica(url + "/NuovaFertilizzazione", JSON.stringify({ Lav_Cod: Lavorazione.value, ChiaveSelezione: datiRiga.Chiave, RicettaRaccoglitore_Cod: $(cIdRicettaCod).val(), Regolamento_Cod: $(cIdRegCod).val(), Pua_Cod: $(cIdPuaCod).val() }),
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);

}

function MostraBrogliaccio() {

    ajaxAgronica(url + "/MostraBrogliaccio", JSON.stringify({ piva: $(cIdPiva).val(), RicettaRaccoglitore_Cod: $(cIdRicettaCod).val() }),
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);

}

function MostraQdC() {

    ajaxAgronica(url + "/MostraQdC", JSON.stringify({ piva: $(cIdPiva).val(), RicettaRaccoglitore_Cod: $(cIdRicettaCod).val() }),
        function (risposta) {
            window.location = risposta.RispostaStringa;
        }, null);

}

function open_window(idDiv, titolo, url) {

    let win_el = $("#" + idDiv);

    win_el.append("<div id='windowFrame'></div>");

    let myWindow = $("#windowFrame").kendoWindow({
        title: titolo,
        width: "90%",
        height: "90%",
        draggable: false,
        visible: false,
        modal: { preventScroll: true },
        resizable: false,
        iframe: true,
        content: url,
        actions: [
            "Close"
        ],
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            e.sender.element.css("opacity", "0");
        },
        activate: function (e) {
            e.sender.element.css("opacity", "1");
        },
        close: function (e) {
            //this.destroy();
        },
        deactivate: function (e) {
            this.destroy();
        }
    }).data("kendoWindow");

    myWindow.center().open();
}

function addAnalisiTerrenoNGListener() {
    if (($(cIdUsaAnalisiNG).val()) === 'True') {
        window.addEventListener('message', event => {
            if (verificaOriginSecondaria(window, window.origin, event) &&
                (event != null && event.data != null) && (event.data.messaggio != null) &&
                event.data.messaggio.includes("chiudiWindowGiasNG")) {
                $('#windowFrame').data('kendoWindow').close();
            }
        });
    }
}

function associaN() {
    let data = gridPD.dataSource.data();
    let impossibileAggiornare = false;
    let almenoUnaRiga = false;
    let messageError = "";

    for (let i = 0; i < data.length; i++) {
        let dataItem = data[i];
        if (dataItem.Selected === true) {
            almenoUnaRiga = true;

            let nFabbisogno = parseFloat(dataItem.N_Fabbisogno);
            let nFabbisognoDatabase = parseFloat(dataItem.N_Fabbisogno_Database);
            let nMas = parseFloat(dataItem.LimiteMas);

            let FattoreCorrettivo_N = parseFloat(dataItem.FattoreCorrettivo_N);
            let Resa = parseFloat(dataItem.Resa);
            let Resa_Rif = parseFloat(dataItem.Resa_Rif);

            if (FattoreCorrettivo_N > 0 && Resa > Resa_Rif) {
                nMas += (Resa - Resa_Rif) * FattoreCorrettivo_N;
            }

            if (nFabbisognoDatabase === -1 || nFabbisognoDatabase === 0 || nFabbisogno <= nFabbisognoDatabase) {

                if (nFabbisogno > nMas && nMas > -1) {
                    impossibileAggiornare = true;
                    messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: L' N Massimo [" + kendo.format("{0:n2}", nFabbisogno) + " Kg/Ha]" +
                        " calcolato da PUA è superiore al Limite Massimo previsto da " + lblMAS + " [" + kendo.format("{0:n2}", nMas) + " Kg/Ha]</li>";
                } else {
                    let uid = dataItem.uid;
                    let ris = aggiornaN(uid, nFabbisogno, dataItem.Sa_Cod, dataItem.appezza, dataItem.id_reg, dataItem.Progetto_Cod);
                    if (ris !== "") {
                        impossibileAggiornare = true;
                        messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: Errore Aggiornamento: " + ris;
                    }
                }
            }
            else if (nFabbisogno > nFabbisognoDatabase) {
                impossibileAggiornare = true;
                messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: Il valore di N Massimo [" + kendo.format("{0:n2}", nFabbisogno) + " Kg/Ha]" +
                    " calcolato da PUA è superiore a quello attualmente memorizzato [" + kendo.format("{0:n2}", nFabbisognoDatabase) + " Kg/Ha]</li>";
            }

        }
    }

    if (almenoUnaRiga === false) {
        kendo.alert("Selezionare almeno una riga");
    } else if (impossibileAggiornare === true) {
        let message = "Impossibile sovrascrivere i seguenti dati: <ul>" + messageError + "</ul>";
        kendo.alert(message);
    }

    //elimino selezione (dovrebbero essere rimaste selezionate solo le righe con errori)
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {

            data[i].Selected = false;

            let row = gridPD.tbody.find("tr[data-uid='" + data[i].uid + "']");
            let chk = $("#gridPD_" + data[i].id);
            if ($(chk).closest('tr').is(GIAS_K_STATE_SELECTED)) {
                $(chk).click();
            }
            row.removeClass(GIAS_K_STATE_SELECTED);
        }
    }
    $("#gridPD .header-chb")[0].checked = false;
}

function associaNMas() {
    let data = gridPD.dataSource.data();
    let impossibileAggiornare = false;
    let almenoUnaRiga = false;
    let messageError = "";

    for (let i = 0; i < data.length; i++) {
        let dataItem = data[i];
        if (dataItem.Selected === true) {
            almenoUnaRiga = true;

            let nFabbisogno = parseFloat(dataItem.N_Fabbisogno);
            let nFabbisognoDatabase = parseFloat(dataItem.N_Fabbisogno_Database);
            let nMas = parseFloat(dataItem.LimiteMas);

            if (nFabbisognoDatabase === -1 || nFabbisognoDatabase === 0 || nMas <= nFabbisognoDatabase) {

                if (nMas > nFabbisogno && nMas > -1 && dataItem.ZVN === true) {
                    impossibileAggiornare = true;
                    messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: Il Limite Massimo di N previsto da " + lblMAS + " [" + kendo.format("{0:n2}", nMas) + " Kg/Ha]" +
                        " è superiore a quello calcolato da PUA [" + kendo.format("{0:n2}", nFabbisogno) + " Kg/Ha]</li>";
                } else {
                    let uid = dataItem.uid;
                    let ris = aggiornaN(uid, nMas, dataItem.Sa_Cod, dataItem.appezza, dataItem.id_reg, dataItem.Progetto_Cod);
                    if (ris !== "") {
                        impossibileAggiornare = true;
                        messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: Errore Aggiornamento: " + ris;
                    }
                }
            }
            else if (nMas > nFabbisognoDatabase) {
                impossibileAggiornare = true;
                messageError += "<li>Appezzamento <b>" + dataItem.Appezzamento + "</b>: Il Limite Massimo di N previsto da " + lblMAS + " [" + kendo.format("{0:n2}", nMas) + " Kg/Ha]" +
                    " è superiore a quello attualmente memorizzato [" + kendo.format("{0:n2}", nFabbisognoDatabase) + " Kg/Ha]</li>";
            }

        }
    }

    if (almenoUnaRiga === false) {
        kendo.alert("Selezionare almeno una riga");
    } else if (impossibileAggiornare === true) {
        let message = "Impossibile sovrascrivere i seguenti dati: <ul>" + messageError + "</ul>";
        kendo.alert(message);
    }

    //elimino selezione (dovrebbero essere rimaste selezionate solo le righe con errori)
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {

            data[i].Selected = false;

            let row = gridPD.tbody.find("tr[data-uid='" + data[i].uid + "']");
            let chk = $("#gridPD_" + data[i].id);
            if ($(chk).closest('tr').is('.' + GIAS_K_STATE_SELECTED)) {
                $(chk).click();
            }
            row.removeClass(GIAS_K_STATE_SELECTED);
        }
    }
    $("#gridPD .header-chb")[0].checked = false;
}

function Calcola_MedieAziendali_NZoo() {

    let data = gridPD.dataSource.data();

    let NUtile_Tot = 0;
    let NTotale_Tot = 0;
    let Ind_Eff_Tot = 0;

    let Ha_ZVN = 0;
    let Ha_Ord = 0;
    let Ha_Tot = 0;

    let Ha_ZVN_SecondoRaccolto = 0;
    let Ha_Ord_SecondoRaccolto = 0;
    let Ha_Tot_SecondoRaccolto = 0;

    let Ha_ZVN_Fertilizzati = 0;
    let Ha_Ord_Fertilizzati = 0;
    let Ha_Tot_Fertilizzati = 0;
    let Ha_ZVN_Fertilizzati_Zoo = 0;
    let Ha_Ord_Fertilizzati_Zoo = 0;
    let Ha_Tot_Fertilizzati_Zoo = 0;


    let NZoo_ZVN = 0;
    let NZoo_Ord = 0;
    let NZoo_Tot = 0;
    let Media_NZoo_ZVN = 0;
    let Media_NZoo_Ord = 0;
    let Media_NZoo_Tot = 0;

    let NZoo_Let_ZVN = 0;
    let NZoo_Let_Ord = 0;
    let NZoo_Let_Tot = 0;
    let Media_NZoo_Let_ZVN = 0;
    let Media_NZoo_Let_Ord = 0;
    let Media_NZoo_Let_Tot = 0;

    let NZoo_Liq_ZVN = 0;
    let NZoo_Liq_Ord = 0;
    let NZoo_Liq_Tot = 0;
    let Media_NZoo_Liq_ZVN = 0;
    let Media_NZoo_Liq_Ord = 0;
    let Media_NZoo_Liq_Tot = 0;

    for (let i = 0; i < data.length; i++) {
        if (data[i].ZVN === true) {
            NZoo_ZVN += data[i].N_Zootecnico * data[i].Superficie;
            NZoo_Liq_ZVN += data[i].N_Zootecnico_Liquame * data[i].Superficie;
            NZoo_Let_ZVN += data[i].N_Zootecnico_Letame * data[i].Superficie;
            //escludo dal calcolo della sup aziendale le colture secondarie
            if (data[i].Ciclo === 0) {
                if (data[i].ValiditaInizio > data[i].Data_Pua) {
                    Ha_ZVN_SecondoRaccolto += data[i].Superficie;
                } else {
                    Ha_ZVN += data[i].Superficie;
                    if (data[i].N_TotaleSoddisfatto !== 0) {
                        Ha_ZVN_Fertilizzati += data[i].Superficie;
                    }
                    if (data[i].N_Zootecnico !== 0) {
                        Ha_ZVN_Fertilizzati_Zoo += data[i].Superficie;
                    }
                }
            } else {
                Ha_ZVN_SecondoRaccolto += data[i].Superficie;
            }
        }
        else {
            NZoo_Ord += data[i].N_Zootecnico * data[i].Superficie;
            NZoo_Liq_Ord += data[i].N_Zootecnico_Liquame * data[i].Superficie;
            NZoo_Let_Ord += data[i].N_Zootecnico_Letame * data[i].Superficie;
            //escludo dal calcolo della sup aziendale le colture secondarie
            //if (data[i].Ciclo === 0) {
            //    Ha_Ord += data[i].Superficie;
            //}
            if (data[i].Ciclo === 0) {
                if (data[i].ValiditaInizio > data[i].Data_Pua) {
                    Ha_Ord_SecondoRaccolto += data[i].Superficie;
                } else {
                    Ha_Ord += data[i].Superficie;
                    if (data[i].N_TotaleSoddisfatto !== 0) {
                        Ha_Ord_Fertilizzati += data[i].Superficie;
                    }
                    if (data[i].N_Zootecnico !== 0) {
                        Ha_Ord_Fertilizzati_Zoo += data[i].Superficie;
                    }
                }
            } else {
                Ha_Ord_SecondoRaccolto += data[i].Superficie;
            }
        }
        NZoo_Tot += data[i].N_Zootecnico * data[i].Superficie;
        NZoo_Let_Tot += data[i].N_Zootecnico_Letame * data[i].Superficie;
        NZoo_Liq_Tot += data[i].N_Zootecnico_Liquame * data[i].Superficie;
        //escludo dal calcolo della sup aziendale le colture secondarie
        //if (data[i].Ciclo === 0) {
        //    Ha_Tot += data[i].Superficie;
        //}
        if (data[i].Ciclo === 0) {
            if (data[i].ValiditaInizio > data[i].Data_Pua) {
                Ha_Tot_SecondoRaccolto += data[i].Superficie;
            } else {
                Ha_Tot += data[i].Superficie;
                if (data[i].N_TotaleSoddisfatto !== 0) {
                    Ha_Tot_Fertilizzati += data[i].Superficie;
                }
                if (data[i].N_Zootecnico !== 0) {
                    Ha_Tot_Fertilizzati_Zoo += data[i].Superficie;
                }
            }
        } else {
            Ha_Tot_SecondoRaccolto += data[i].Superficie;
        }

        NUtile_Tot += data[i].N_BilancioAzotato_Utile;
        NTotale_Tot += data[i].N_BilancioAzotato_Totale;
        Ind_Eff_Tot += data[i].Indice_Efficienza_Azotata;
    }

    if (Ha_ZVN > 0) {

        if (NZoo_ZVN > 0) {
            Media_NZoo_ZVN = NZoo_ZVN / Ha_ZVN;
        }
        if (NZoo_Let_ZVN > 0) {
            Media_NZoo_Let_ZVN = NZoo_Let_ZVN / Ha_ZVN;
        }
        if (NZoo_Liq_ZVN > 0) {
            Media_NZoo_Liq_ZVN = NZoo_Liq_ZVN / Ha_ZVN;
        }
    }

    if (Ha_Ord > 0) {

        if (NZoo_Ord > 0) {
            Media_NZoo_Ord = NZoo_Ord / Ha_Ord;
        }
        if (NZoo_Let_Ord > 0) {
            Media_NZoo_Let_Ord = NZoo_Let_Ord / Ha_Ord;
        }
        if (NZoo_Liq_Ord > 0) {
            Media_NZoo_Liq_Ord = NZoo_Liq_Ord / Ha_Ord;
        }
    }

    if (Ha_Tot > 0) {

        if (NZoo_Tot > 0) {
            Media_NZoo_Tot = NZoo_Tot / Ha_Tot;
        }
        if (NZoo_Let_Tot > 0) {
            Media_NZoo_Let_Tot = NZoo_Let_Tot / Ha_Tot;
        }
        if (NZoo_Liq_Tot > 0) {
            Media_NZoo_Liq_Tot = NZoo_Liq_Tot / Ha_Tot;
        }
    }


    $('#Txt_NUtile').val(kendo.format('{0:n2}', roundNumber(NUtile_Tot, 4)));
    $('#Txt_NTotale').val(kendo.format('{0:n2}', roundNumber(NTotale_Tot, 4)));
    $('#Txt_IndiceEff').val(kendo.format('{0:n2}', roundNumber(Ind_Eff_Tot, 4)));

    $('#Txt_Ha_ZVN').val(kendo.format('{0:n4}', roundNumber(Ha_ZVN, 4)));
    $('#Txt_Ha_Ord').val(kendo.format('{0:n4}', roundNumber(Ha_Ord, 4)));
    $('#Txt_Ha_Tot').val(kendo.format('{0:n4}', roundNumber(Ha_Tot, 4)));

    $('#Txt_Ha_ZVN_SecondoRaccolto').val(kendo.format('{0:n4}', roundNumber(Ha_ZVN_SecondoRaccolto, 4)));
    $('#Txt_Ha_Ord_SecondoRaccolto').val(kendo.format('{0:n4}', roundNumber(Ha_Ord_SecondoRaccolto, 4)));
    $('#Txt_Ha_Tot_SecondoRaccolto').val(kendo.format('{0:n4}', roundNumber(Ha_Tot_SecondoRaccolto, 4)));

    $('#Txt_Ha_ZVN_Fertilizzati').val(kendo.format('{0:n4}', roundNumber(Ha_ZVN_Fertilizzati, 4)));
    $('#Txt_Ha_Ord_Fertilizzati').val(kendo.format('{0:n4}', roundNumber(Ha_Ord_Fertilizzati, 4)));
    $('#Txt_Ha_Tot_Fertilizzati').val(kendo.format('{0:n4}', roundNumber(Ha_Tot_Fertilizzati, 4)));
    $('#Txt_Ha_ZVN_Fertilizzati_Zoo').val(kendo.format('{0:n4}', roundNumber(Ha_ZVN_Fertilizzati_Zoo, 4)));
    $('#Txt_Ha_Ord_Fertilizzati_Zoo').val(kendo.format('{0:n4}', roundNumber(Ha_Ord_Fertilizzati_Zoo, 4)));
    $('#Txt_Ha_Tot_Fertilizzati_Zoo').val(kendo.format('{0:n4}', roundNumber(Ha_Tot_Fertilizzati_Zoo, 4)));

    $('#Txt_NZoo_Tot_ZVN').val(kendo.format('{0:n2}', roundNumber(NZoo_ZVN, 4)));
    $('#Txt_NZoo_Tot_Ord').val(kendo.format('{0:n2}', roundNumber(NZoo_Ord, 4)));
    $('#Txt_NZoo_Tot_Media').val(kendo.format('{0:n2}', roundNumber(NZoo_Tot, 4)));
    $('#Txt_NZoo_Tot_Let_ZVN').val(kendo.format('{0:n2}', roundNumber(NZoo_Let_ZVN, 4)));
    $('#Txt_NZoo_Tot_Let_Ord').val(kendo.format('{0:n2}', roundNumber(NZoo_Let_Ord, 4)));
    $('#Txt_NZoo_Tot_Let_Media').val(kendo.format('{0:n2}', roundNumber(NZoo_Let_Tot, 4)));
    $('#Txt_NZoo_Tot_Liq_ZVN').val(kendo.format('{0:n2}', roundNumber(NZoo_Liq_ZVN, 4)));
    $('#Txt_NZoo_Tot_Liq_Ord').val(kendo.format('{0:n2}', roundNumber(NZoo_Liq_Ord, 4)));
    $('#Txt_NZoo_Tot_Liq_Media').val(kendo.format('{0:n2}', roundNumber(NZoo_Liq_Tot, 4)));

    $('#Txt_NZoo_ZVN').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_ZVN, 4)));
    $('#Txt_NZoo_Ord').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Ord, 4)));
    $('#Txt_NZoo_Media').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Tot, 4)));
    $('#Txt_NLiquame_ZVN').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Liq_ZVN, 4)));
    $('#Txt_NLiquame_Ord').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Liq_Ord, 4)));
    $('#Txt_NLiquame_Tot').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Liq_Tot, 4)));
    $('#Txt_NLetame_ZVN').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Let_ZVN, 4)));
    $('#Txt_NLetame_Ord').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Let_Ord, 4)));
    $('#Txt_NLetame_Tot').val(kendo.format('{0:n2}', roundNumber(Media_NZoo_Let_Tot, 4)));


}



function modificaMultipla() {

    let data = gridPD.dataSource.data();

    selected_add = new Array();

    let Veg_Cod = "";
    let TipoZona = "";
    let veg_cod_uguale = true;
    let TipoZona_uguale = true;

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {

            if (TipoZona === "") {
                TipoZona = data[i].ZVN;
            }
            if (TipoZona !== data[i].ZVN) {
                TipoZona_uguale = false;
            }

            if (Veg_Cod === "") {
                Veg_Cod = data[i].Veg_Cod;

            }
            if (Veg_Cod !== data[i].Veg_Cod) {
                veg_cod_uguale = false;
            }

            selected_add.push(data[i]);
        }
    }

    if (selected_add.length > 1) {
        obj_ModificaMultipla = {};
        if (TipoZona_uguale) {
            obj_ModificaMultipla.ZVN = selected_add[0].ZVN;
            if (selected_add[0].ZVN === true) {
                obj_ModificaMultipla.TipoZona = "v";
            } else {
                obj_ModificaMultipla.TipoZona = "n";
            }
            //obj_ModificaMultipla.Veg_Cod = selected_add[0].Veg_Cod;
            obj_ModificaMultipla.Validita_Inizio = selected_add[0].Validita_Inizio;
        }
        if (veg_cod_uguale) {
            obj_ModificaMultipla.Veg_Cod = selected_add[0].Veg_Cod;
            obj_ModificaMultipla.Validita_Inizio = selected_add[0].Validita_Inizio;
        }
        Window_Modifica.open();
    } else {
        kendo.alert("Selezionare almeno 2 appezzamenti.");
    }
}

function openModificaMultipla() {

    let data = new Array();
    let isDeleted;

    var strAvviso = '';

    if (obj_ModificaMultipla.Veg_Cod !== undefined) {
        isDeleted = false;
        data.push({ des: "Resa", value: "7", isDeleted: isDeleted });
        if (pua_tipo === Enum_PuaTipo.Completo) {
            data.push({ des: "Tipologia, Coefficiente B", value: "3", isDeleted: isDeleted });
        }
        else {
            data.push({ des: "Tipologia, N Mass, Resa Riferimento, Fattore Correttivo N", value: "3", isDeleted: isDeleted });
        }
    } else {
        isDeleted = true;
        strAvviso = "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati appezzamenti con utilizzi (specie vegetali) diversi.";
    }

    data.push({ des: "Ciclo", value: "1", isDeleted: false });
    data.push({ des: "Precessione", value: "2", isDeleted: false });

    if (pua_tipo === Enum_PuaTipo.Completo) {

        data.push({ des: "Ubicazione", value: "4", isDeleted: false });

        if (obj_ModificaMultipla.TipoZona !== undefined) {
            isDeleted = false;
            data.push({ des: "Acqua di Irrigazione", value: "5", isDeleted: isDeleted });
        } else {
            isDeleted = true;
            if (strAvviso === '') {
                strAvviso = "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati appezzamenti con particelle in diverse zone di vulnerabilità.";
            } else {
                strAvviso += " Altri parametri non sono modificabili in quanto sono stati selezionati appezzamenti con particelle in diverse zone di vulnerabilità";
            }
        }
    }

    $("#avvertimentoModificaMultipla").text(strAvviso);

    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Parametri = $("#Cmb_Parametri").kendoDropDownList({
            autoBind: true,
            dataTextField: "des",
            dataValueField: "value",
            dataSource: data,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value("");
                this.trigger("change");
            },
            change: async function (e) {
                WaitFrame.show();
                switch (this.value()) {
                    case "":
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        break;
                    case "1": //Ciclo
                        $("#modifica_ciclo").show();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        Cmb_Mod_Ciclo = await get_Cmb_Mod_Ciclo();
                        break;
                    case "2": //Precessione
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").show();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        Cmb_Mod_Precessione = await get_Cmb_Mod_Precessione();
                        break;
                    case "3": //Finalita RER, Coeff B
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").show();
                        if (pua_tipo === Enum_PuaTipo.Completo) {
                            $("#div_mas").hide();
                            $("#div_bperc").show();
                        } else {
                            $("#div_bperc").hide();
                            $("#div_mas").show();
                        }
                        Cmb_Mod_FinalitaRer = await get_Cmb_Mod_FinalitaRer();
                        //Txt_Mod_BPerc = await get_Txt_Mod_BPerc();
                        break;
                    case "4": //Ubicazione
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").show();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        Cmb_Mod_Ubicazione = await get_Cmb_Mod_Ubicazione();
                        break;
                    case "5": //TipoAcqua
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").show();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        Cmb_Mod_TipoAcqua = await get_Cmb_Mod_TipoAcqua();
                        break;
                    case "6": //Analisi
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").show();
                        $("#modifica_resa").hide();
                        $("#modifica_bperc").hide();
                        Cmb_Mod_Analisi = await get_Cmb_Mod_Analisi();
                        break;
                    case "7": //Resa
                        $("#modifica_ciclo").hide();
                        $("#modifica_precessione").hide();
                        $("#modifica_ubicazione").hide();
                        $("#modifica_acqua").hide();
                        $("#modifica_analisi").hide();
                        $("#modifica_resa").show();
                        $("#modifica_bperc").hide();
                        Txt_Mod_Resa = await get_Txt_Mod_Resa();
                        break;

                }
                WaitFrame.hide();
                resolve(this);
            },
            optionLabel: "SELEZIONA"
            //template: kendo.template($("#template_Cmb_Parametri").html())
        }).data("kendoDropDownList");
    });

}

function applicaModifiche() {
    switch (Cmb_Parametri.value()) {
        case "":
            kendo.alert("Selezionare un parametro su cui eseguire delle modifiche.");
            return false;
        //break;
        case "1":
            var CicloCod = Cmb_Mod_Ciclo.value();
            var CicloDes = Cmb_Mod_Ciclo.text();
            if (CicloCod === undefined || CicloCod === "") {
                kendo.alert("Selezionare il ciclo da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Ciclo = parseInt(CicloCod);
                    selected_add[i].CicloDes = CicloDes;
                }
            }
            break;
        case "2":
            var PrecessioneCod = Cmb_Mod_Precessione.value();
            var PrecessioneDes = Cmb_Mod_Precessione.text();

            if (PrecessioneCod === undefined || PrecessioneCod === "" || PrecessioneCod === "0" || PrecessioneCod === 0) {
                kendo.alert("Selezionare la precessione da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].PrecessioneCod = parseInt(PrecessioneCod);
                    selected_add[i].PrecessioneDes = PrecessioneDes;
                }
            }
            break;
        case "3":
            var FinalitaRerCod = Cmb_Mod_FinalitaRer.value();
            var FinalitaRerDes = Cmb_Mod_FinalitaRer.text();

            var b = Txt_Mod_BPerc.value();
            var mas = Txt_Mod_Mas.value();
            var resarif = Txt_Mod_ResaRif.value();
            var fattorecorrettivo = Txt_Mod_FattoreCorrettivo.value();

            if (FinalitaRerCod === undefined || FinalitaRerCod === "" || FinalitaRerCod === "0" || FinalitaRerCod === 0) {
                kendo.alert("Selezionare la Tipologia da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Grfi_Cod_Concimazione = parseInt(FinalitaRerCod);
                    selected_add[i].Grfi_Des_Concimazione = FinalitaRerDes;
                    selected_add[i].B_Perc = parseFloat(b);
                    selected_add[i].LimiteMas = parseFloat(mas);
                    selected_add[i].Resa_Rif = parseFloat(resarif);
                    selected_add[i].FattoreCorrettivoN = parseFloat(fattorecorrettivo);
                }
            }
            break;
        case "4":
            var UbicazioneCod = Cmb_Mod_Ubicazione.value();
            var UbicazioneDes = Cmb_Mod_Ubicazione.text();
            if (UbicazioneCod === undefined || UbicazioneCod === "" || UbicazioneCod === "0" || UbicazioneCod === 0) {
                kendo.alert("Selezionare l'ubicazione da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].UbicazioneCod = parseInt(UbicazioneCod);
                    selected_add[i].UbicazioneDes = UbicazioneDes;
                }
            }
            break;
        case "5":
            var TipoAcquaCod = Cmb_Mod_TipoAcqua.value();
            var TipoAcquaDes = Cmb_Mod_TipoAcqua.text();
            if (TipoAcquaCod === undefined || TipoAcquaCod === "") {
                kendo.alert("Selezionare l'acqua di irrigazione da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].TipoAcquaCod = parseInt(TipoAcquaCod);
                    selected_add[i].TipoAcquaDes = TipoAcquaDes;
                }
            }
            break;
        case "6":
            var AnalisiTestataCod = Cmb_Mod_Analisi.value();
            var AnalisiTestataDes = Cmb_Mod_Analisi.text();
            if (AnalisiTestataCod === undefined || AnalisiTestataCod === "" || AnalisiTestataCod === "0" || AnalisiTestataCod === 0) {
                kendo.alert("Selezionare un'analisi da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].AnalisiTestataCod = parseInt(AnalisiTestataCod);
                    selected_add[i].AnalisiTestataDes = AnalisiTestataDes;
                }
            }
            break;
        case "7":
            var resa = Txt_Mod_Resa.value();
            if (resa === "" || resa === null) {
                kendo.alert("Impostare una resa da applicare agli appezzamenti selezionati.");
                return false;
            } else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Resa = resa;
                }
            }
            break;
    }
    Window_Modifica.close();
    let data = gridPD.dataSource.data();
    for (let i = 0; i < data.length; i++) {
        data[i].Selected = false;
    }

    let ris = SalvaModificaMultipla(selected_add);
    if (ris == true) {
        kendo.alert("Gli appezzamenti sono stati modificati.");
        return true;
    } else {
        return false;
    }
}

function get_Cmb_Mod_Ciclo() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Ciclo = $("#Cmb_Mod_Ciclo").kendoDropDownList({
            //filter: "contains",
            autoBind: true,
            dataTextField: "Ciclo_Des",
            dataValueField: "Ciclo_Cod",
            dataSource: ddlCiclo,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value("");
                this.trigger("change");
            },
            change: function (e) {
                resolve(this);
            },
            optionLabel: "SELEZIONA"
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_Precessione() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Precessione = $("#Cmb_Mod_Precessione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: ddlPrecessione,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            optionLabel: "SELEZIONA"
        }).data("kendoDropDownList");
    });
}


function get_Cmb_Mod_Ubicazione() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Ubicazione = $("#Cmb_Mod_Ubicazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: ddlUbicazione,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            optionLabel: "SELEZIONA"
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_TipoAcqua() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_TipoAcqua = $("#Cmb_Mod_TipoAcqua").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: ddlTipoAcqua[obj_ModificaMultipla.TipoZona],
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            optionLabel: "SELEZIONA"
        }).data("kendoDropDownList");
    });
}

function get_Txt_Mod_Resa() {
    return new Promise((resolve, reject) => {
        if (Txt_Mod_Resa === undefined) {
            Txt_Mod_Resa = $("#Txt_Mod_Resa").kendoNumericTextBox({
                decimals: 2
            }).data("kendoNumericTextBox");
        }
        Txt_Mod_Resa.value(null);
        resolve(Txt_Mod_Resa);
    });
}


function get_Cmb_Mod_FinalitaRer() {

    let regCod = parseInt($(cIdRegCod).val());

    ddlFinalitaRER = riempiDdlFinalitaRER(regCod, obj_ModificaMultipla.Veg_Cod, 0);

    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_FinalitaRer = $("#Cmb_Mod_FinalitaRer").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: ddlFinalitaRER,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: changeFinalitaRER_Mod,
            optionLabel: "SELEZIONA"
        }).data("kendoDropDownList");
    });
}

function VaiAPianoDistribuzioneVerificaIndiciBilancio(modalita) {

    var param = JSON.stringify({
        PC_Testata_Cod: parseInt($(cIdPuaCod).val()),
        PC_Dettagli_PIVA: $(cIdPiva).val(),
        PC_Tipo: parseInt($(cIdPuaTipo).val()),
        Regolamento_Cod: parseInt($(cIdRegCod).val()),
        Ricetta_Cod: parseInt($(cIdRicettaCod).val()),
        blocco_flag: parseInt($(cIdBloccoFlag).val()),
        data_inizio: $(cIdDataInizio).val(),
        data_fine: $(cIdDataFine).val(),
        modalita: modalita
    });

    ajaxAgronicaSync("PUA_Piano_Distribuzione.aspx/ApriVerificaIndiciBilancio", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                console.log(risposta.ParametroDue_stringa);
                window.location = risposta.RispostaStringa;
            }
            else {
                console.log(risposta);
                alert(risposta.Errore);
            }
        }, function (risposta) {
            alert(risposta.Errore);
        });

}



function popolaGrigliaEffluenti(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Eff_kReadValorizzazione_rows
    };

    var idModel = "eff_cod";
    var campiKendoModel = Eff_kReadValorizzazione_mod();
    var colonneKendoGrid = Eff_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        aggregate: [{ field: "azoto_qta", aggregate: "sum" }]
    };

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        btnEliminaTuttiFiltri: false
    };


    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoDataBound: Eff_onDataBoundRighe,
        //funzioneDaChiamareDopoEdit: Eff_onEdit,
        funzioneDaChiamarePrimaDelDetailInit: detailInit
        //funzioneDaChiamareDopoAnnulla: onAnnulla
        // funzioneDaChiamareDopoDataBound: Eff_dopoDataBound,
        //funzioneDaChiamarePrimaDelSave: Eff_primaDelSave
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );



}

function Eff_kReadValorizzazione_rows(options) {

    var data = $('#' + id_HD_Effluenti).val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);

}


function Eff_kReadValorizzazione_mod() {

    var data = $('#' + id_HD_Effluenti).val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;

}

function Eff_kReadValorizzazione_col() {

    var data = $('#' + id_HD_Effluenti).val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;

}

function detailInit(e) {

    var eff_cod_selezionato = e.data.eff_cod;
    var id_div = "GrigliaDettagli" + eff_cod_selezionato.toString();
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    popolaGrigliaEffluentiDettagli(id_div, e.data.eff_cod);

}

function popolaGrigliaEffluentiDettagli(IDControllo, eff_cod) {

    var funzioniCRUD = { funzioneRead: Dett_kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
    var idModel = "id";
    var campiKendoModel = Dett_kReadValorizzazione_mod();
    var colonneKendoGrid = Dett_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = { filter: [{ field: "eff_cod", operator: "eq", value: eff_cod }] };
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: false,
        btnEliminaTuttiFiltri: false
    };
    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoDataBound: Dett_onDataBoundRighe
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function Dett_kReadValorizzazione_rows(options) {

    var data = $('#' + id_HD_Effluenti_Dettagli).val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);

}

function Dett_kReadValorizzazione_col() {

    var data = $('#' + id_HD_Effluenti_Dettagli).val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;

}

function Dett_kReadValorizzazione_mod() {

    var data = $('#' + id_HD_Effluenti_Dettagli).val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;

}



