var chartType = "column";
var chartTheme = "bootstrap";
var collapsed = { columns: [], rows: [] };

function kendoPivot_DataBound(e) {


    var grid = $('#pivot_tab_riepilogo').data('kendoPivotGrid');
    var dataSource = grid.dataSource;
    var pers = {}
    if (personalizzazioniPivot !== null && personalizzazioniPivot !== "") 
        pers = JSON.parse(personalizzazioniPivot)
    pers.columns = dataSource.columns();
    pers.rows = dataSource.rows();
    pers.measures = dataSource.measures();
    personalizzazioniPivot = kendo.stringify(pers)

    
    var fields = jQuery('span[data-name]');
    var udmProdNum = 0;
    fields.each(function (_, item) {
            item = $(item);
            var text = item[0].children[0].innerText 

            switch (text) {
                
                case "Piva":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "PartitaIvaAbbr", "P.IVA");
                    break;

                case "Des_Lib":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "DesLib", "Descrizione Operazione");
                    break;

                case "Data_Movimento":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "DataMovimento", "Data operazione");
                    break;

                case "Ora":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Ora", "Ora");
                    break;

                case "Mov_Desc":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "DescMovimento", "Note operazione");
                    break;

                case "Username_Creazione":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "UtenteCreazione", "Utente Creazione");
                    break;

                case "Veg_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "SpecieImp", "Specie impianto");
                    break;

                case "Tecnico":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "TecnicoReferente", "Tecnico");
                    break;

                case "App_Nome":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Appezzamento", "Appezzamento");
                    break;

                case "Fr_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Formulati", "formulati");
                    break;

                case "Fer_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Fertilizzanti", "Fertilizzanti");
                    break;

                case "Trap_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Trappole", "Trappole");
                    break;

                case "Ins_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Insetti", "Insetti utili");
                    break;

                case "Mat_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "MateriePrime", "Materie Prime");
                    break;

                case "sa_nome":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "CentroAziendale", "Centro aziendale");
                    break;

                case "rag_soc":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "RagioneSociale", "Ragione sociale");
                    break;

                case "lav_des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Operazione", "Operazione");
                    break;

                case "gru_des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "GruppoOperazioni", "Gruppo operazione");
                    break;

                case "tipo":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "TipoOperazione", "Tipo Operazione");
                    break;

                case "cul_des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "VarietàImp", "Varieta impianto");
                    break;

                case "campo_des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Campo", "Campo");
                    break;

                case "LottoImpianto":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Lotto", "Lotto");
                    break;

                case "DestinazioneTerreniNudi_Des":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "DestTerreniNudi", "Destinazione terreni nudi");
                    break;

                case "Data_Ultima_Modifica_Intervento":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "UltimaModificaIntervento", "Data ultima modifica intervento");
                    break;

                case "validita_inizio_destinazione":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "InizioValiditaDest", "Inizio validita destinazione");
                    break;

                case "UdmProdSim":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Unità di misura prodotto simbolo", "Unità di misura prodotto simbolo");
                    break;

                case "UdmExtraSim":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Unità di misura prodotto simbolo", "Unità di misura prodotto simbolo");
                    break;

                case "Azienda_Padre":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "AziendaReferente", "Azienda referente");
                    break;

                case "Piva_Padre":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "PivaReferente", "Piva referente");
                    break;

                case "Stato":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Nazione", "Nazione");
                    break;

                case "Regione":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Regione", "Regione");
                    break;

                case "Provincia":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Provincia", "Provincia");
                    break;

                case "Localita":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Comune", "Comune");
                    break;

                case "Anno_movimento":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "AnnoMovimento", "Anno movimento");
                    break;

                case "Mese_movimento":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "MeseMovimento", "Mese movimento");
                    break;

                case "Data_creazione":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "DataCreazione", "Data creazione");
                    break;

                case "Anno_Creazione_Azienda":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Azienda", "Anno Creazione Azienda");
                    break;

                case "Anno_Creazione_Imp":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Imp", "Anno Creazione Impianto");
                    break;

                case "Anno_Creazione_Op":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Op", "Anno Creazione Operazione");
                    break;

                case "Data_Creazione_Azienda":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Azienda", "Data Creazione Azienda");
                    break;

                case "Data_Creazione_Imp":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Imp", "Data Creazione Impianto");
                    break;

                case "Data_Creazione_Op":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Op", "Data Creazione Operazione");
                    break;

                case "Mese_Creazione_Azienda":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Azienda", "Mese Creazione Azienda");
                    break;

                case "Mese_Creazione_Imp":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Imp", "Mese Creazione Impianto");
                    break;

                case "Mese_Creazione_Op":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Op", "Mese Creazione Operazione");
                    break;

                case "Utente_Creazione_Azienda":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Azienda", "Utente Creazione Azienda");
                    break;

                case "Utente_Creazione_Imp":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Imp", "Utente Creazione Impianto");
                    break;

                case "Utente_Creazione_Op":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Op", "Utente Creazione Operazione");
                    break;

                case "TipoProdotto":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "TipoProdotto", "Tipo Prodotto");
                    break;

                case "NomeProdotto":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "NomeProdotto", "Nome Prodotto");
                    break;

                case "UdmProd":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "UdmProd", "Unità di misura Prodotto");
                    break;

                case "UdmImpSim":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "UdmProdImp", "Unità di misura Prodotto");
                    break;

                case "UdmExtra":
                    item[0].children[0].innerText = TraduzioneMultiResx(reportQdCResx, "UdmExtra", "Unità di Misura Prodotto Totale");
                    break;

                default: break; // "Unknown Field"
            }

            
        });
}

function popolaPivotReportQdC(IDPivotConfigurator, IDPivotGrid) {
    var funzioneReadPivotGrid = RicercaReportQdC;
    var modelKendoPivot = {
        Piva: { type: "string" },
        Sa_Cod: { type: "number" },
        Id_Agenda: { type: "number" },
        Id_Mov: { type: "number" },
        Id_Mov_Det: { type: "number" },
        Lav_Cod: { type: "number" },
        Des_Lib: { type: "string" },
        Data_Movimento: { type: "date" },
        Ora: { type: "date" },  
        Mov_Desc: { type: "string" },
        Username_Creazione: { type: "string" },
        Data_creazione: { type: "date" },
        Cau_Mov: { type: "string" },
        Cul_Cod: { type: "number" },
        Veg_Cod: { type: "number" },
        Veg_Des: { type: "string" },
        Raccoglitore_Cod: { type: "number" },
        Tecnico: { type: "string" },
        Tipo_Destinazione: { type: "number" },
        Appezza: { type: "number" },
        App_Nome: { type: "string" },
        Elem_Cod: { type: "number" },
        Mat_Cod: { type: "number" },
        Pro_Cod: { type: "number" },
        TipoProdotto: { type: "string" },
        NomeProdotto: { type: "string" },
        Cod_Articolo: { type: "number" },
        sa_nome: { type: "string" },
        rag_soc: { type: "string" },
        lav_des: { type: "string" },
        gru_des: { type: "string" },
        tipo: { type: "string" },
        cul_des: { type: "string" },
        campo_des: { type: "string" },
        IdImpianto: { type: "number" },
        LottoImpianto: { type: "string" },
        DestinazioneTerreniNudi_Cod: { type: "number" },
        DestinazioneTerreniNudi_Des: { type: "string" },
        Data_Ultima_Modifica_Intervento: { type: "date" },
        validita_inizio_destinazione: { type: "date" },
        UdmProdSim: { type: "string" },
        UdmImp: { type: "string" },
        UdmExtraSim: { type: "string" },
        Azienda_Padre: { type: "string" },
        Piva_Padre: { type: "string" },
        Stato: { type: "string" },  
        Regione: { type: "string" },
        Provincia: { type: "string" },
        Localita: { type: "string" },
        Anno_movimento: { type: "number" },
        Mese_movimento: { type: "number" },
        SupApp: { type: "number" },
        QtaImp: { type: "number" },
        SupTrattata: { type: "number" },
        Qta_Extra_Totale: { type: "number" },
        QtaProd: { type: "number" },
        QTA_EXTRA: { type: "number" },
        UdmProd: { type: "string" },
        UdmExtra: { type: "string" },
        num_impianti: { type: "number" },
        num_operazioni: { type: "number" },
        num_poligoni: { type: "number" },
        num_poligoni_mancanti: { type: "number" },
        Anno_Creazione_Azienda: { type: "number" },
        Anno_Creazione_Imp: { type: "number" },
        Anno_Creazione_Op: { type: "number" },
        Data_Creazione_Azienda: { type: "date" },
        Data_Creazione_Imp: { type: "date" },
        Data_Creazione_Op: { type: "date" },
        Mese_Creazione_Azienda: { type: "number" },
        Mese_Creazione_Imp: { type: "number" },
        Mese_Creazione_Op: { type: "number" },
        Utente_Creazione_Azienda: { type: "string" },
        Utente_Creazione_Imp: { type: "string" },
        Utente_Creazione_Op: { type: "string" }

    };

    var cubeDimensionsKendoPivot = KendoDDL("id_selTipoAnalisi").value() == 1 ? ElencoDimensioniImpianti :
        getKendoSwitch("SwitchEstraiImpianti") ? ElencoDimensioniOperazioniImpianti : ElencoDimensioniOperazioni;
    
    var cubeMeasuresKendoPivot = KendoDDL("id_selTipoAnalisi").value() == 1 ? ElencoMisureImpianti : ElencoMisureOperazioni;

    if (getKendoSwitch("SwitchEstraiImpianti")) {
        cubeMeasuresKendoPivot = Object.assign({}, cubeMeasuresKendoPivot, ElencoMisureOperazioniImpianti)
    }

    if (getKendoSwitch("SwitchEstraiProdotti")) {
        cubeDimensionsKendoPivot = Object.assign({}, cubeDimensionsKendoPivot, ElencoDimensioniProdotti)
        if (!getKendoSwitch("SwitchEstraiImpianti")) {
            cubeMeasuresKendoPivot = Object.assign({}, cubeMeasuresKendoPivot, ElencoMisureOperazioniProdotti)
            cubeDimensionsKendoPivot = Object.assign({}, cubeDimensionsKendoPivot, ElencoDimensioniOperazioniProdottiNoImpianti)
        }
    }

    if (getKendoSwitch("SwitchEstraiImpianti") && getKendoSwitch("SwitchEstraiProdotti")) {
        cubeMeasuresKendoPivot = Object.assign({}, cubeMeasuresKendoPivot, ElencoMisureOperazioniProdottiImpianti)
        cubeDimensionsKendoPivot = Object.assign({}, cubeDimensionsKendoPivot, ElencoDimensioniProdottiImpianti)
    }

    var parametriPerLettura = null;
    var parametriKendoPivotConfigurator = { height: 500 };
    var parametriDataSourcePivotGrid = {};
    var parametriKendoPivotGrid = {
        // chartCfg: { divchart: "#grafico_tab_riepilogo", group: "column", category: "row", format: "{0}", type: "column", sort: true },
        sortable: true,
        pdf: false,
        filterable: true,
        rowHeaderTemplate: $("#rowTemplate").html(),
        columnHeaderTemplate: $("#columnTemplate").html()
    };
    var funzioniPrimaDopoEventiPivotGrid = {       
        collapseMember: collapseMember, expandMember: expandMember, funzioneDaChiamareDopoDataBound: kendoPivot_DataBound
    };

    var colonneDefaultKendoPivotGrid = [];
    var righeDefaultKendoPivotGrid = [];
    var misureDefaultKendoPivotGrid = [];

    if (personalizzazioniPivot && personalizzazioniPivot !== "") {
        var options = JSON.parse(personalizzazioniPivot);
        colonneDefaultKendoPivotGrid = options.columns;
        righeDefaultKendoPivotGrid = options.rows;
        misureDefaultKendoPivotGrid = options.measures;
        if (options.filters !== undefined) {
            parametriDataSourcePivotGrid.filters = options.filters;
        }
        //personalizzazioniPivot = null;
    } else {
        // impostazioni di default
        colonneDefaultKendoPivotGrid = [];
        righeDefaultKendoPivotGrid = [{ name: "Azienda_Padre", expand: true }, { name: "rag_soc", expand: false}];
        misureDefaultKendoPivotGrid = KendoDDL("id_selTipoAnalisi").value() == 0 ? [TraduzioneMultiResx(reportQdCResx, "NumOperazioni", "Nr. Operazioni")] : [TraduzioneMultiResx(reportQdCResx, "NumImpianti", "Numero Impianti")];
    }

    creaKendoPivotGrid(IDPivotConfigurator, // rappresenta l'ID del div a cui si associa la griglia
        parametriKendoPivotConfigurator,
        IDPivotGrid,  // ID Pivot grid
        funzioneReadPivotGrid,  //funzioni js da chiamare per read, insert, update, delete
        modelKendoPivot,
        cubeDimensionsKendoPivot,
        cubeMeasuresKendoPivot,
        colonneDefaultKendoPivotGrid,
        righeDefaultKendoPivotGrid,
        misureDefaultKendoPivotGrid,
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSourcePivotGrid, // parametri data source { chiave - valore}
        parametriKendoPivotGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventiPivotGrid
    );

    $("#exportPdf").hide();
}

function Esegui_Report() {
    trovatoErrore = false;

    dataDaControllare = $('input[name$="Txt_DataOpDal"]').val();
    dataValida = true;
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("Da data movimento non valida", "DIV_Messaggi");
        trovatoErrore = true;
    }

    dataDaControllare = $('input[name$="Txt_DataOpAl"]').val();
    dataValida = true;
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida) {
        MessaggioErrore_Bootstrap("A data movimento non valida", "DIV_Messaggi");
        trovatoErrore = true;
    }

    if (!trovatoErrore) {

        switch (parseInt(Get_KendoDDLValue("TipoOutput", 0))) {

            //case 0:
            //    // Griglia
            //    $(".pivotGridArea").hide();
            //    $(".gridArea").show();
            //    $(".reportArea").hide();
            //    popolaGrigliaReportVendite("tab_testata_griglia_report_vendite");
            //    ////var gridVendite = $("#tab_testata_griglia_report_vendite").data("kendoGrid");
            //    ////if (gridVendite !== null && gridVendite !== undefined)
            //    ////{
            //    ////    gridVendite.bind("columnMenuOpen", grid_columnMenuOpen);
            //    ////    gridVendite.bind("columnMenuInit", grid_columnMenuInit);
            //    ////}
            //    break;
            
            case 1:
                // Pivot
                $(".pivotGridArea").show();
                popolaPivotReportQdC("configuratore_tab_riepilogo", "pivot_tab_riepilogo");
                $(".gridArea").hide();
                $(".reportArea").hide();
                break;
            
            //case 2:
            //    // Report PDF
            //    var errMsg = checkReportPDF();

            //    if (errMsg === "")
            //        Lancia_Report_PDF();
            //    else
            //        MessaggioErrore_Bootstrap(errMsg, "DIV_Messaggi");
            //    break;

        }

    }
}

function checkReportPDF() {
    var errMsg = "";

    if (KendoDDL("sel_tipo_report_vendite").value() === "-1") {
        errMsg += "Selezionare il TIPO REPORT da lanciare <br/>";
    }

    if ($("#livelliScelti").data("kendoListBox").dataSource._data.length === 0) {
        errMsg += "Scegliere almeno un livello da stampare <br/>";
    } 

    if ($("#TxtTitolo").val()  === "") {
        errMsg += "Digitare il titolo <br/>";
    }

    if ($("#Txt_da_mese").val() === 0 ||
        $("#Txt_da_mese").val() === "" ||
        $("#Txt_da_anno").val() === 0 ||
        $("#Txt_da_anno").val() === "" ||
        $("#Txt_a_mese").val() === 0 ||
        $("#Txt_a_mese").val() === "")
    {
        errMsg += "Impostare MESE INIZIO / FINE / ANNO DI RIFERIMENTO <br/>";
    }
    else
    {
        var meseDa = parseInt($("#Txt_da_mese").val());
        var meseA = parseInt($("#Txt_a_mese").val());
        if (meseA < meseDa)
        {
            errMsg += "Il MESE FINE del periodo di riferimento non può essere minore del MESE INIZIO <br/>";
        }
    }

    if (getKendoSwitch("cb_confronto_anno")) {
        if ($("#Txt_confronto_anno").val() === 0 ||
            $("#Txt_confronto_anno").val() === "") {
            errMsg += "Impostare il campo A PARTIRE DALL'ANNO <br/>";
        }
    } else {
        $("#Txt_confronto_anno").val(0);
    }

    if (KendoDDL("sel_tipo_valore").value() === "-1") {
        errMsg += "Selezionare il VALORE DA CONSIDERARE <br/>";
    } else {

        if (KendoDDL("sel_tipo_report_vendite").value() === enum_Tipo_Report_PDF.Statistica_mese_singolo_anno_su_quantità_altro_Valore) {
            if (KendoDDL("sel_tipo_secondo_valore").value() === "-1") {
                errMsg += "Selezionare il SECONDO VALORE DA CONSIDERARE <br/>";
            }
        }

        // Controllo che non venga scelta la quantità generica (che potrebbe contenere quantitù relative a unità di misura
        // non sommabili fra loro se non viene scelto il livello prodotto
        if ($("#livelliScelti").data("kendoListBox").dataSource._data.length !== 0 && 
            KendoDDL("sel_tipo_valore").value() === "0") {
            var foundProdotto = false;
            for (var y = 0; y < $("#livelliScelti").data("kendoListBox").dataSource._data.length; y++) {
                    if ( $("#livelliScelti").data("kendoListBox").dataSource._data[y].value  === "Prodotto") {
                        foundProdotto = true;
                    }
                }
            
            if (!foundProdotto)
                errMsg += "Non è possibile scegliere come VALORE DA CONSIDERARE la quantità venduta se non si include il Prodotto fra i livelli da stampare <br/>";
                 
        }     
    }



    if (KendoDDL("sel_tipo_report_vendite").value() === "1") {
        if ($("#Txt_mese_prev_scost").val() === 0 ||
            $("#Txt_mese_prev_scost").val() === "") {
            errMsg += "Impostare il campo MESE FINO A CUI CONTROLLARE LO SCOSTAMENTO <br/>";
        }
    }
    
    return errMsg;
}

// click stampa documento
function StampaDocumento(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var pivaImpresa = dataItem.PIVA;
    var Id_Agenda = dataItem.Id_Agenda;
    var Lav_Cod = dataItem.Lav_Cod;
    var Modulo = dataItem.Modulo;
    var Tipo_Accettazione = dataItem.Tipo_Accettazione;
    stampa_documento(pivaImpresa, Id_Agenda, Lav_Cod, Modulo, Tipo_Accettazione);
}

function onExportExcel(e) {
    var rows = e.workbook.sheets[0].rows;
    for (var ri = 0; ri < rows.length; ri++) {
        var row = rows[ri];
        if (row.type === "group-footer" || row.type === "footer") {
            for (var ci = 0; ci < row.cells.length; ci++) {
                var cell = row.cells[ci];
                /* if (Object.prototype.toString.call(cell.value) === "[object Date]") {
                    var d = new Date(cell.value);
                    cell.value = kendo.toString(data, "dd/MM/yyyy");
                } */
                if (cell.value) {
                    // Use jQuery.fn.text to remove the HTML and get only the text
                    cell.value = $(cell.value).text();
                    // Set the alignment
                    cell.hAlign = "right";
                }
            }
        }
    }

}

function kEventoSelezionaRiga(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#tab_testata_griglia_report_vendite").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function onDataBoundingReportVendite(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }

    var _wrapper = e.sender.wrapper,
        _header = _wrapper.find(".k-grid-header");

    var objEv = {
        data: {
            wrapper: _wrapper,
            header: _header
        }
    };

    resizeFixedStatistiche(objEv);
    $(window).on("resize", { wrapper: _wrapper, header: _header }, resizeFixedStatistiche);
    $(window).on("scroll", { wrapper: _wrapper, header: _header }, scrollFixedStatistiche);
}

function resizeFixedStatistiche(ev) {
    var header = ev.data.header;
    var wrapper = ev.data.wrapper;

    var wrapperWidth = wrapper.width();
    if (wrapperWidth !== 0) {
        var paddingRight = parseInt(header.css("padding-right"));
        header.css("width", wrapperWidth - paddingRight);
    }
    else {
        // Nel caso l'evento venga eseguito mentre la griglia è nascosta, imposto una variabile per ricalcolare correttamente la width
        // alla prima occorrenza dell'evento "scroll" in quanto più frequente
        header.css("width", "auto");
        header.data("fix_width", 1);
    }
}

function scrollFixedStatistiche(ev) {
    var header = ev.data.header;
    var wrapper = ev.data.wrapper;

    // Nel caso l'evento venga eseguito mentre la griglia è nascosta, rimuovo la classe
    var wrapperHeight = wrapper.height();

    if (header.data("fix_width") === 1 && wrapperHeight !== 0) {
        var paddingRight = parseInt(header.css("padding-right"));
        header.css("width", wrapper.width() - paddingRight);
        header.data("fix_width", 0);
    }

    var offset = $(this).scrollTop(),
        tableOffsetTop = wrapper.offset().top,
        tableOffsetBottom = tableOffsetTop + wrapperHeight - header.height();
    if (offset < tableOffsetTop || offset > tableOffsetBottom || wrapperHeight === 0) {
        header.removeClass("fixed-header");
    } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
        header.addClass("fixed-header");
    }
}

function SpecieChange(e) {

    //var filtro_specie = KendoMultisel("multiselSpecie").value().join(",");

    //if (filtro_specie !== "")
    //    Elenco_Varieta = Leggi_Varieta(filtro_specie);
    //else
    //    Elenco_Varieta = "";

    //var multiselVarieta = KendoMultisel("multiselVarieta");
    //multiselVarieta.autoBind = false;

    //RiempiVarieta(multiselVarieta.dataSource);

}

function TipoOperazioneChange(e) {
    Leggi_Operazioni(KendoMultisel("selTipoOperazione").value());
    KendoMultisel("multiselOperazione").setDataSource(new kendo.data.DataSource({ data: Elenco_Operazioni }));
}

function NazioneChange(e) {
    LeggiRegioni();
    KendoMultisel("multiselContea").setDataSource(new kendo.data.DataSource({ data: Elenco_Regioni }));
    //creaKendoMultiselect("multiselContea", { read: RiempiRegioni, data: { REG: -1 } }, "Regione_Des", "REG", null, null, null, RegioneChange);
}

function RegioneChange(e) {
    LeggiProvince();
    KendoMultisel("multiselSottocontea").setDataSource(new kendo.data.DataSource({ data: Elenco_Province }));
    //creaKendoMultiselect("multiselSottocontea", { read: RiempiProvince, data: { PROV: -1 } }, "PROVINCIA", "PROV", null, null, null, ProvinciaChange);
}

function ProvinciaChange(e) {
    LeggiComuni();
    KendoMultisel("multiselDistretto").setDataSource(new kendo.data.DataSource({ data: Elenco_Comuni }));
    //creaKendoMultiselect("multiselDistretto", { read: RiempiComuni, data: { COM: -1 } }, "LOCALITA", "COM", null, null, null, null);
}

function ReferenteChange(e) {
    Elenco_Aziende = LeggiAziende();
    var aziende = KendoMultisel("multiselAzienda");
    aziende.dataSource.read();
    aziende.refresh();
}

function AziendaChange(e) {
    LeggiCentri();
    KendoMultisel("multiselCentroAzienda").setDataSource(new kendo.data.DataSource({ data: Elenco_Centri }));
    //creaKendoMultiselect("multiselCentroAzienda", { read: RiempiCentri, data: { PivaSa: -1 } }, "sa_nome", "PivaSa", null, null, null, null);
}

function RiempiSpecie(options) {
    options.success(Elenco_Specie);
}

function RiempiTipoOperazioni(options) {
    options.success(ddlTipoOperazioni);
}

function RiempiOperazioni(options) {
    options.success(Elenco_Operazioni);
}

function RiempiNazioni(options) {
    options.success(Elenco_Nazioni);
}

function RiempiRegioni(options) {
    options.success(Elenco_Regioni);
}

function RiempiProvince(options) {
    options.success(Elenco_Province);
}

function RiempiComuni(options) {
    options.success(Elenco_Comuni);
}

function RiempiReferenti(options) {
    options.success(Elenco_Referenti);
}

function RiempiAziende(options) {
    options.success(Elenco_Aziende);
}

function RiempiCentri(options) {
    options.success(Elenco_Centri);
}

function Svuota_Parametri() {
    KendoDDL("id_selTipoAnalisi").value(0);
    KendoDDL("id_selTipoAnalisi").trigger("change")

    KendoDDL("id_selEstrazione").value(0);
    KendoDDL("id_selEstrazione").trigger("change")

    SvuotaParametriImpianti();

    KendoMultisel("id_multiselCentroAzienda").value(-1);

    KendoMultisel("id_multiselAzienda").value(-1);
    KendoMultisel("id_multiselAzienda").trigger("change");

    KendoMultisel("id_multiselReferente").value(-1);
    KendoMultisel("id_multiselReferente").trigger("change");

    KendoMultisel("id_msDistretto").value(-1);

    KendoMultisel("id_msSottocontea").value(-1);
    KendoMultisel("id_msSottocontea").trigger("change");

    KendoMultisel("id_msContea").value(-1);
    KendoMultisel("id_msContea").trigger("change");

    KendoMultisel("id_msNazione").value(-1);
    KendoMultisel("id_msNazione").trigger("change");

    personalizzazioniPivot = null;

}

function SvuotaParametriImpianti() {

    KendoDDL("ddlFiltroDateImpianto").value(0);
    KendoDDL("ddlFiltroDateImpianto").trigger("change")

    KendoDDL("ddlFiltroDateOperazione").value(0);
    KendoDDL("ddlFiltroDateOperazione").trigger("change")

    $("#periodoGiorni").val(-1)
    $("#periodoGiorniOperazioni").val(-1)
    $("#periodoGiorni")[0].defaultValue = -1;
    $("#periodoGiorniOperazioni")[0].defaultValue = -1;

    $("#Txt_DataImpDal").data("kendoDatePicker").value("");
    $("#Txt_DataImpAl").data("kendoDatePicker").value("");

    KendoMultisel("multiselSpecie").value(-1);

}

function Personalizza_Report(parametri) {
    KendoDDL("id_selTipoAnalisi").value(parametri._tipoAnalisi);
    KendoDDL("id_selTipoAnalisi").trigger("change")

    KendoDDL("id_selEstrazione").value(parametri._tipoEstrazione);
    KendoDDL("id_selEstrazione").trigger("change")
   
    setKendoSwitch("SwitchEstraiImpianti", parametri._impianti);
    if (getKendoSwitch("SwitchEstraiImpianti"))
        $("#a_tabFiltroImpianti").show();
    else
        $("#a_tabFiltroImpianti").hide();
    setKendoSwitch("SwitchEstraiProdotti", parametri._prodotti);

    KendoDDL("ddlFiltroDateImpianto").value(parametri._filtroDate);
    KendoDDL("ddlFiltroDateImpianto").trigger("change")

    KendoDDL("ddlFiltroDateOperazione").value(parametri._filtroDateOperazioni);
    KendoDDL("ddlFiltroDateOperazione").trigger("change")

    $("#periodoGiorni").val(parametri._periodoGiorni)
    $("#periodoGiorniOperazioni").val(parametri._periodoGiorniOperazioni)

    set_data("Txt_DataImpDal", parametri._dataImpDal, null);
    set_data("Txt_DataImpAl", parametri._dataImpAl, null);

    set_data("Txt_DataOpDal", parametri._dataOpDal, null);
    set_data("Txt_DataOpAl", parametri._dataOpAl, null);

    KendoMultisel("multiselSpecie").value(parametri._specie.split("|"));

    if (parametri._referenti !== undefined && parametri._referenti !== "") {
        KendoMultisel("id_multiselReferente").value(parametri._referenti.split("|"));
        KendoMultisel("id_multiselReferente").trigger("change");
    }
    if (parametri._aziende !== undefined && parametri._aziende !== "") {
        KendoMultisel("id_multiselAzienda").value(parametri._aziende.split("|"));
        KendoMultisel("id_multiselAzienda").trigger("change");
    }
    if (parametri._centri !== undefined && parametri._centri !== "")
        KendoMultisel("id_multiselCentroAzienda").value(parametri._centri.split("|"));
    if (parametri._nazioni !== undefined && parametri._nazioni !== "") {
        KendoMultisel("id_msNazione").value(parametri._nazioni.split("|"));
        KendoMultisel("id_msNazione").trigger("change");
    }
    if (parametri._regioni !== undefined && parametri._regioni !== "") {
        KendoMultisel("id_msContea").value(parametri._regioni.split("|"));
        KendoMultisel("id_msContea").trigger("change");
    }
    if (parametri._province !== undefined && parametri._province !== "") {
        KendoMultisel("id_msSottocontea").value(parametri._province.split("|"));
        KendoMultisel("id_msSottocontea").trigger("change");
    }
    if (parametri._comuni !== undefined && parametri._comuni !== "")
        KendoMultisel("id_msDistretto").value(parametri._comuni.split("|"));

    KendoDDL("TipoOutput").value(parametri.tipoOutput === "Pivot" ? 1 : 0)

    var ddlOutput = KendoDDL("TipoOutput");
    ddlOutput.enable(false);
    ddlOutput.trigger("change");

    personalizzazioni = parametri.personalizzazioni;
    personalizzazioniPivot = parametri.personalizzazioniPivot;

}
