function kendoZoo_onDataBoundedRighe(e) {
    riduciAltezzaRighe("#divKendoZoo", 1);
}

function kendoZoo_inizializza(divKendo, keys) {

    let funzioniCRUD = {
        funzioneRead: kReadValorizzazioneZoo_rows,
        checkBoxFunction: KendoOperazioni_zoo
    };

    let capiSelezionatiModificaMultipla;

    let idModel = "chiave";

    let campiKendoModel = kReadValorizzazioneZoo_mod();
    let colonneKendoGrid = kReadValorizzazioneZoo_col();

    let parametriPerLettura = [];
    let parametriDataSource = { pagesize: 50 };

    let templateCommands = "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoZoo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaZoo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>"
    let widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Info", "Info") + '" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoZoo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + '" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaZoo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>'
        widthAzioni = "140px";
    }

    let parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        checkSelezioneRiga: { filterable: false, field: null, width: "35px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: templateCommands
                }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
            }
        ]

    };

    let funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            //mostraColonnaUnicaSeInMobile(e, 1);
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            let arrKeys = new Array();
            let objParametri_Agenda = JSON.parse(objP_agenda);
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Fabbricato)
            let grid = $("#" + divKendo).data("kendoGrid");
            let data = grid.dataSource.data();
            for (let i = 0; i < arrKeys.length; i++) {
                for (let j = 0; j < data.length; j++) {
                    if (data[j].chiave == arrKeys[i]) {
                        let rowUid = data[j].uid;
                        let row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                    }
                }
            }

            Dati_Relativi_Percorso_Selezione2(8);
            //NascondiBottoni(grid, e);
        },
        funzioneDaChiamareDopoChange: function (e) {
            let grid = $("#" + divKendo).data("kendoGrid");
            let selectedRow = grid.select();
            let dataItem = grid.dataItem(selectedRow);
            ImpostaObjP_Agenda(7, dataItem.chiave, false, function () {
                Dati_Relativi_Percorso_Selezione2(8);
            });
        }
    };
    let mostraRigheCancellate = true;
    let colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
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

    CreaToolBarZoo();
}

function kReadValorizzazioneZoo_rows(options) {

    let data = $('#hdKendoZoo_Valorizzazione').val();
    let jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneZoo_col() {

    return [
        {
            "field": "Progetto",
            "title": Traduzione(menuBSAnagraficaResx, "Progetto", "Progetto"),
            "width": "110px",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Matricola",
            "title": Traduzione(menuBSAnagraficaResx, "Matricola", "Matricola"),
            "width": "130px",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Tag",
            "title": Traduzione(menuBSAnagraficaResx, "Tag", "Codice Elettronico"),
            "width": "110px",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Matricola_Breve",
            "title": Traduzione(menuBSAnagraficaResx, "MatricolaBreve", "Matricola Breve"),
            "width": "110px",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Matricola_Breve4",
            "title": Traduzione(menuBSAnagraficaResx, "MatricolaBreve4", "Matricola Breve"),
            "width": "110px",
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Progetto_Nome",
            "title": Traduzione(menuBSAnagraficaResx, "DistintaNome", "Distinta Nome"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Codice_Distinta",
            "title": Traduzione(menuBSAnagraficaResx, "Lotto", "Lotto"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Metodo_Produzione",
            "title": Traduzione(menuBSAnagraficaResx, "MetodoProduzione", "Metodo Produzione"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Lotto_Fornitore",
            "title": Traduzione(menuBSAnagraficaResx, "LottoFornitore", "Lotto Fornitore"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        //{
        //    "field": "Cod_Contatto",
        //    "title": "C.F. Fornitore",
        //    "width": "120px",
        //    "filterable": { "multi": true, "search": true }
        //},
        {
            "field": "Nome",
            "title": Traduzione(menuBSAnagraficaResx, "Nome", "Nome"),
            "width": "100px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Sesso",
            "title": Traduzione(menuBSAnagraficaResx, "Sesso", "Sesso"),
            "width": "80px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Validato",
            "title": Traduzione(menuBSAnagraficaResx, "Validato", "Validato"),
            "width": "80px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Tipo_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Tipo", "Tipo"),
            "width": "90px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Dat_Nascita",
            "title": Traduzione(menuBSAnagraficaResx, "DataNascita", "Data Nascita"),
            "width": "110px",
            filterable: { ui: "datepicker" },
            template: "#= (kendo.toString(Dat_Nascita, 'dd/MM/yyyy' ) == '01/01/1900') ? '' : kendo.toString(Dat_Nascita, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "SPE_DES",
            "title": Traduzione(menuBSAnagraficaResx, "Specie", "Specie"),
            "width": "90px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "RAZ_DES",
            "title": Traduzione(menuBSAnagraficaResx, "Razza", "Razza"),
            "width": "110px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "IPRO_DES",
            "title": Traduzione(menuBSAnagraficaResx, "IndirizzoProduttivo", "Indirizzo Produttivo"),
            "width": "110px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Stato_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Stato", "Stato"),
            "width": "110px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "sa_nome",
            "title": Traduzione(menuBSAnagraficaResx, "Centro", "Centro"),
            "width": "100px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "STA_DES",
            "title": Traduzione(menuBSAnagraficaResx, "Stalla", "Stalla"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Raggruppamento_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Gruppo", "Gruppo"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Mat_Madre",
            "title": Traduzione(menuBSAnagraficaResx, "MatricolaMadre", "Matricola Madre"),
            "width": "140px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "RazDes_Madre",
            "title": Traduzione(menuBSAnagraficaResx, "RazzaMadre", "Razza Madre"),
            "width": "140px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Mat_Padre",
            "title": Traduzione(menuBSAnagraficaResx, "MatricolaPadre", "Matricola Padre"),
            "width": "140px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "RazDes_Padre",
            "title": Traduzione(menuBSAnagraficaResx, "RazzaPadre", "Razza Padre"),
            "width": "140px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "FlagBDN",
            "title": Traduzione(menuBSAnagraficaResx, "PresenteBDN", "Presente in BDN"),
            "width": "140px",
            "hidden": !permesso_bdn_read,
            "menu": permesso_bdn_read,
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Id_Capo_BDN",
            "title": Traduzione(menuBSAnagraficaResx, "IdentifCapoAllevBDN", "ID Capo BDN"),
            "width": "140px",
            "hidden": !permesso_bdn_read,
            "menu": permesso_bdn_read,
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "CF_PROPRIETARIO",
            "title": Traduzione(menuBSAnagraficaResx, "CodFiscProprietario", "C.F. Proprietario"),
            "width": "130px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "CF_DETENTORE",
            "title": Traduzione(menuBSAnagraficaResx, "CodFiscDetentore", "C.F. Detentore"),
            "width": "130px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "AUSL_AZI_NASCITA",
            "title": Traduzione(menuBSAnagraficaResx, "CodAUSLAziendaNascita", "Codice Azienda Nascita"),
            "width": "130px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Certificato",
            "title": Traduzione(menuBSAnagraficaResx, "CertificatoIntra", "Certificato Intra"),
            "width": "130px",
            "hidden": !permesso_bdn_read,
            "menu": permesso_bdn_read,
            "filterable": { "multi": true, "search": true }
        },
        //{
        //    "field": "Rag_Soc",
        //    "title": "Fornitore",
        //    "width": "120px",
        //    "filterable": { "multi": true, "search": true }
        //},
        {
            "field": "Modello4_Ingresso_Numero",
            "title": Traduzione(menuBSAnagraficaResx, "NumModello4Ingresso", "N. Modello 4 Ingresso"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Modello4_Ingresso_Prenotazione",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceModello4Ingresso", "Codice Modello 4 Ingresso"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        //{
        //    "field": "Data_Documento_Ingresso",
        //    "title": "Data Documento Ingresso",
        //    "width": "120px",
        //    filterable: { ui: "datepicker" },
        //    template: "#= ((kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) == null) || Data_Documento_Ingresso <= AGRODATAINIZIO) ? '' : kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) #"
        //},
        {
            "field": "Modello4_Uscita_Numero",
            "title": Traduzione(menuBSAnagraficaResx, "NumModello4Uscita", "N. Modello 4 Uscita"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Modello4_Uscita_Prenotazione",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceModello4Uscita", "Codice Modello 4 Uscita"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        //{
        //    "field": "Data_Documento_Uscita",
        //    "title": "Data Modello 4 Uscita",
        //    "width": "120px",
        //    filterable: { ui: "datepicker" },
        //    template: "#= ((kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) == null) || Data_Documento_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) #"
        //},
        {
            "field": "Codice_Azienda_Uscita",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceAziendaUscita", "Codice Azienda Uscita"),
            "width": "120px",
            "filterable": { "multi": true, "search": true }
        },
        {
            "field": "Validita_Inizio",
            "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
            "width": "110px",
            filterable: { ui: "datepicker" },
            template: "#= (kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) == '01/01/1900') ? '' : kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Validita_Fine",
            "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
            "width": "110px",
            filterable: { ui: "datepicker" },
            template: "#= (kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) == '31/12/2100') ? '' : kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Codice_Azienda_Fornitore",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceAziendaFornitore", "Codice Azienda Fornitore"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RagSoc_FornFatt",
            "title": Traduzione(menuBSAnagraficaResx, "FornitoreFatturazione", "Fornitore Fatturazione"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RagSoc_FornProv",
            "title": Traduzione(menuBSAnagraficaResx, "FornitoreProvenienza", "Fornitore Provenienza"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "N_Bolla_Fornitore",
            "title": Traduzione(menuBSAnagraficaResx, "NumeroDDTIngresso", "Numero DDT Ingresso"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_DDT_Ingresso",
            "title": Traduzione(menuBSAnagraficaResx, "DataDDTIngresso", "Data DDT Ingresso"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px",
            template: "#= ((kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) == null) || Data_DDT_Ingresso <= AGRODATAINIZIO) ? '' : kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "N_Bolla_Uscita",
            "title": Traduzione(menuBSAnagraficaResx, "NumeroDDTUscita", "Numero DDT Uscita"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_DDT_Uscita",
            "title": Traduzione(menuBSAnagraficaResx, "DataDDTUscita", "Data DDT Uscita"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px",
            template: "#= ((kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) == null) || Data_DDT_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "RagSoc_StallaSvezz",
            "title": Traduzione(menuBSAnagraficaResx, "StallaSvezzamento", "Stalla Svezzamento"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Incremento_Teorico",
            "title": Traduzione(menuBSAnagraficaResx, "IncrementoTeoricoKg", "Incremento Teorico Kg"),
            "filterable": { "multi": true, "search": true },
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "DataModifica", "Data Modifica"),
            "width": "110px",
            filterable: { ui: "datepicker" },
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteModifica", "Utente Modifica"),
            "width": "130px",
            "filterable": { "multi": true, "search": true },
        },
        {
            "field": "Data_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "DataCreazione", "Data Creazione"),
            "width": "110px",
            filterable: { ui: "datepicker" },
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteCreazione", "Utente Creazione"),
            "width": "130px",
            "filterable": { "multi": true, "search": true }
        }
    ];

}

function kReadValorizzazioneZoo_mod() {

    let data = $('#hdKendoZoo_Valorizzazione').val();
    let jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaZoo(keys) {

    let parametri = { };

    //if ($("#filterData").val() == "") {
    //    kendo.alert("Non è stata impostata una data di validità.");
    //}
    //else {
    ajaxAgronica(indirizzohttp + "/CaricaZoo",
                JSON.stringify(parametri),
                function(risposta) {
                    $('#hdKendoZoo_Valorizzazione').val(risposta.RispostaStringa);
                    kendoZoo_inizializza("divKendoZoo", keys);
                },
                null);
    //}
}


function infoZoo(tr_elem, grid_elem) {

    let datiGriglia = $(grid_elem).data('kendoGrid');
    let datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    let chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoZoo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Zoo/Zoo_Animali_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaZoo(tr_elem, grid_elem) {

    let datiGriglia = $(grid_elem).data('kendoGrid');
    let datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    let chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditZoo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Zoo/Zoo_Animali_Edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaZoo(tr_elem, grid_elem) {

    let datiGriglia = $(grid_elem).data('kendoGrid');
    let datiRiga = datiGriglia.dataItem(tr_elem);

    let xTipoNodo = 22;
    let chiave = datiRiga.chiave;

    let streelemento = "Fabbricato: <b>" + datiRiga.chiave + "</b>";

    Popup_delete(streelemento, xTipoNodo, chiave);
}

function KendoOperazioni_zoo() {

    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $('#divKendoZoo').data("kendoGrid");
    let dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked);

}

function CreaToolBarZoo() {

    let gridTB = $("#divKendoZoo").find(".k-grid-toolbar");

    //if (permesso_bdn_write) {
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="gestioneCapiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Capi BDN" title = "Gestione Capi BDN">Gestione Capi BDN</div></div> ');
    //}

    //if (permesso_bdn_write) {
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="carichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Carichi BDN" title = "Gestione Carichi BDN">Gestione Carichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="scarichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Scarichi BDN" title = "Gestione Scarichi BDN">Gestione Scarichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="modello4BDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Invio Modello 4" title = "Invio Modello 4">Invio Modello 4</div></div> ');
    //}

    //if (permesso_bdn_write) {
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="carichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Carichi BDN" title = "Gestione Carichi BDN">Gestione Carichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="scarichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Scarichi BDN" title = "Gestione Scarichi BDN">Gestione Scarichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="modello4BDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Invio Modello 4" title = "Invio Modello 4">Invio Modello 4</div></div> ');
    //}

    //if (permesso_bdn_write) {
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="carichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Carichi BDN" title = "Gestione Carichi BDN">Gestione Carichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="scarichiBDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Gestione Scarichi BDN" title = "Gestione Scarichi BDN">Gestione Scarichi BDN</div></div> ');
    //    gridTB.append('<div class="xi-grid-button-group">' +
    //        '<div id="modello4BDN" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="Invio Modello 4" title = "Invio Modello 4">Invio Modello 4</div></div> ');
    //}

    if (permesso_modificaMultipla) {
        if (GiasVersioneMaster === "2022") {
            gridTB.append('<div id="btn_Redirect_ModificaMultipla_Zoo" class="k-button k-button-icontext k-grid--button" data-title="' + Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_modificaMultipla', 'Modifica Multipla') + '"><i class="k-icon k-i-list-unordered"></i></div>');
        } else {
            gridTB.append('<div id="btn_Redirect_ModificaMultipla_Zoo" class="k-button k-button-icontext"><i class="k-icon k-i-list-unordered"></i>' + Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_modificaMultipla', 'Modifica Multipla') + '</div>');
        }
    }

    $("#btn_Redirect_ModificaMultipla_Zoo").click(function (e) {
        ModificaMultiplaRedirect();
    });

    $("#gestioneCapiBDN").click(function () {
        gestioneCapiBDN();
    });

    $("#carichiBDN").click(function () {
        gestioneCarichiBDN();
    });

    $("#scarichiBDN").click(function () {
        gestioneScarichiBDN();
    });

    $("#modello4BDN").click(function () {
        //gestioneCapiBDN();
    });

}

function gestioneCapiBDN() {
    let capiSelezionati = capi_selezionati();
    if (capiSelezionati.length == 0) {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "SelezionareAlmenoUnAnimale", "Selezionare almeno un capo animale."));
        return;
    }
    let parametri = {
        chiave: JSON.stringify(capiSelezionati)
    }
    ajaxAgronica(indirizzohttp + "/getURLGestioneCapiBDN",
                JSON.stringify(parametri),
                function(risposta) {
                    window.location = risposta.RispostaStringa;
                },
                null);
}

function gestioneCarichiBDN() {
    ajaxAgronica(indirizzohttp + "/getURLGestioneCarichiBDN",
                JSON.stringify(parametri),
                function(risposta) {
                    window.location = risposta.RispostaStringa;
                },
                null);
}

function gestioneScarichiBDN() {
    ajaxAgronica(indirizzohttp + "/getURLGestioneScarichiBDN",
                JSON.stringify(parametri),
                function(risposta) {
                    window.location = risposta.RispostaStringa;
                },
                null);
}

async function ModificaMultiplaRedirect() {
    capiSelezionatiModificaMultipla = capi_selezionati_modifica_multipla();
    if (capiSelezionatiModificaMultipla.length == 0) {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "SelezionareAlmenoUnAnimale", "Selezionare almeno un capo animale."));
    } else {
        const urlParams = new URLSearchParams(window.location.search);
        const qsVisibilita = urlParams.get('visibilita');
        var rispostaRedirect = await WS_rispostaRedirectModificaMultipla(qsVisibilita);
        window.location = rispostaRedirect;
    }
}

function WS_rispostaRedirectModificaMultipla(qsVisibilita) {
    let obj_impianto_str = kendo.stringify(obj_Impianto);
    // TO DO verificare che i valori passati siano effettivamente corretti nel codebehind
    // TO DO valutare se aggiungere colonne non visibili alla kendo grid per piva sa_cod e progetto_cod
    return new Promise((resolve, reject) => {
    
        var parametri = kendo.stringify({
            obj_Impianto_str: obj_impianto_str,
            capiSelezionatiModificaMultipla: capiSelezionatiModificaMultipla,
            Qs_Visibilita: qsVisibilita
        });
    
        ajaxAgronica("MenuBS_Anagrafica.aspx/modificaMultiplaRedirect",
            parametri,
            function (risposta) {
                let r = risposta.ParametroDue_stringa;
                resolve(r);
            }, null, null, false);
    });
}

function capi_selezionati() {

    let datiGriglia = $("#divKendoZoo").data('kendoGrid');
    let dataAppezza = datiGriglia.dataSource.data();
    let appezzaSelezionati = new Array();
    for (let i = 0; i < dataAppezza.length; i++) {
        if (dataAppezza[i].Selected == true) {
            appezzaSelezionati.push(dataAppezza[i].chiave);
        }
    }

    return appezzaSelezionati;
}

function capi_selezionati_modifica_multipla() {

    let datiGriglia = $("#divKendoZoo").data('kendoGrid');
    let dataCapo = datiGriglia.dataSource.data();
    let capiSelezionati = new Array();
    for (let i = 0; i < dataCapo.length; i++) {
        if (dataCapo[i].Selected == true) {
            capiSelezionati.push(dataCapo[i]);
            //appezzaSelezionati.push(dataAppezza[i].chiave);
        }
    }

    return capiSelezionati;
}