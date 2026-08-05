// ----------------------
//      VALIDAZIONE
// ----------------------
function RiempicmbValidazione(options) {
    var elenco_validazioni = [
        { "Validazione_Cod": 0, "Validazione_Des": TraduzioneMultiResx(scadCreaModItemResx, "DocumentoDaValidare", "Documento da validare") },
        { "Validazione_Cod": -1, "Validazione_Des": TraduzioneMultiResx(scadCreaModItemResx, "DocumentoNonValidoUfficio", "Documento non valido ufficio") },
        { "Validazione_Cod": -2, "Validazione_Des": TraduzioneMultiResx(scadCreaModItemResx, "DocumentoNonValidoAutocontrollo", "Documento non valido autocontrollo") },
        { "Validazione_Cod": 1, "Validazione_Des": TraduzioneMultiResx(scadCreaModItemResx, "DocumentoValidoUfficio", "Documento valido ufficio") },
        { "Validazione_Cod": 2, "Validazione_Des": TraduzioneMultiResx(scadCreaModItemResx, "DocumentoValidoAutocontrollo", "Documento valido autocontrollo") }
    ];

    options.success(elenco_validazioni);
}

function cmbValidazione_change(e) {

    if (usaUploadMultiplo === "") {
        switch (KendoDDL("cmbValidazione").dataItem().Validazione_Cod) {

            case 0: //Da Validare            

                $("#btn_Rimuovi").show();
                break;

            default:
                $("#btn_Rimuovi").hide();
                break;
        }
    } else {
        switch (KendoDDL("cmbValidazione" + usaUploadMultiplo).dataItem().Validazione_Cod) {
            case 0: //Da Validare
                $(".k-dropzone").show()
                $(".k-action-buttons").show()
                $("button[name='rimuoviElemento']").show()
                break;

            default:
                $(".k-dropzone").hide()
                $(".k-action-buttons").hide()
                $("button[name='rimuoviElemento']").hide()
                break;
        }
    }
}


// ----------------------
//      SALVA
// ----------------------
function Salva(FlagEsci) {

    SalvaDocumento($(cModalita).val(), FlagEsci);

}


// #region ALLEGATO
// IL BOTTONE FINTO MI SIMULA IL CLICK DEL PULSANTE VERO
function openFileDialogFinto() {
    $("#File_Allegato").click();
}

// SCRIVO IL PERCORSO DEL FILE SULLA TEXTBOX
function scriviPercorsoFileSuTxt() {
    var nomeFile = $("#File_Allegato").val().replace("C:\\fakepath\\", "");
    $('#Txt_Documento_Allegato').val(nomeFile);
    bAllegato_Modificato = true; //Effettuato modifica ad allegato
    CompressoDaGIAS = false;
    AggiornaUsernameUpload();
    ImpostaEstensione();
    $("#pnlDatiAllegato").show();
}

var handleFileSelect = function (evt) {
    var file = evt.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            $('#File_Caricato').val(btoa(binaryString));
        };
        reader.readAsBinaryString(file);
    }
};

function ImpostaEstensione() {

    if (usaUploadMultiplo === "") {
        Estensione = ""; //Reset
        //Impostazione Estensione
        if ($("#Txt_Documento_Allegato").val() !== undefined && $("#Txt_Documento_Allegato").val() !== "") {

            var partsArray = $("#Txt_Documento_Allegato").val().toString().split('.');
            if (partsArray.length > 0) {
                Estensione = partsArray[1];

            }
        }
    } else {

    }
}

function RimuoviAllegato() {

    $('#Txt_Num_Documento').val("");
    $('#Txt_Documento_Allegato').val("");
    $('#File_Caricato').val("");
    $("#pnlDatiAllegato").hide();
    $("#pnlApriAllegato").hide();

}
// #endregion

// #region CATEGORIA
function ddlArea_Load(LoadTipologie) {

    $('#ddlArea').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlArea } },
        dataTextField: "nome",
        dataValueField: "id_area",
        optionLabel: { "nome": "", "id_area": "" },
        autoWidth: true,
        dataBound: ddlArea_OnDataBound
    });

    //Caricamento di Tutte le Tipologie
    if (LoadTipologie !== false) {
        ddlTipologia_Load();
    }
}

function ddlArea_OnDataBound(e) {
    var ds = this.dataSource.data();
    id_tipologia_old = 0;
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        nascondiPnlAccessori();
        ddlArea.onchange(); //forzo l'evento di onchange
    }
}

function ddlArea_Change() {

    let id_area_selezionato = $('#ddlArea').data("kendoDropDownList").value();
    if (id_area_selezionato != id_area_old) {
        pulisciDdlTipologie();

        //Se entro in inserimento da menu documenti e scelgo categoria UMA non ho in querystring il codice richiesta.
        //E quindi blocco l'inserimento.
        if (jQuery.isNumeric(id_area_selezionato) && parseInt(id_area_selezionato) === 7 && $(cRichiesta_Cod).val() === "0") {
            $('#ddlArea').data("kendoDropDownList").select("");
            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "NonepossibileInserireDocumentoUMA", "Non è possibile inserire un documento UMA direttamente dalla gestione documenti. Utilizzare le apposite funzioni nella sezione UMA."));
        } else {
            ddlTipologia_Load();
        }

        id_area_old = id_area_selezionato
    }
    //Pulisco i controlli
}
// #endregion

// #region TIPOLOGIA
function ddlTipologia_Load() {

    //creaKendoDropDownList("ddlTipologia", { read: RiempiDdlTipologia }, "nome", "Key1").bind("change", cmbDestinazione_change);

    $('#ddlTipologia').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlTipologia } },
        dataTextField: "nome",
        dataValueField: "id_tipologia",
        optionLabel: { "nome": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "id_tipologia": "" },
        autoWidth: true,
        dataBound: ddlTipologia_OnDataBound
    });
}

function ddlTipologia_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length === 1) {
        this.select(1); //seleziono l'elemento 
        ddlTipologia.onchange(); //forzo l'evento di onchange
    }
}

function ddlTipologia_Change() {

    var id_area = $('#ddlArea').val();
    var ddl = KendoDDL("ddlTipologia");

    if (ddl.dataItem() !== undefined) {

        var id_area2 = ddl.dataItem().id_area;

        //Se entro in inserimento da menu documenti e scelgo categoria UMA non ho in querystring il codice richiesta.
        //E quindi blocco l'inserimento.
        if (jQuery.isNumeric(id_area2) && parseInt(id_area2) === 7 && $(cRichiesta_Cod).val() === "0") {
            ddl.select("");
            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "NonepossibileInserireDocumentoUMA", "Non è possibile inserire un documento UMA direttamente dalla gestione documenti. Utilizzare le apposite funzioni nella sezione UMA."));
        } else {

            //popolo le combo in base alla tipologia selezionata
            var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

            if (id_tipologia == "") {
                //Cliccato su Seleziona --> Pulizia                
                $('#id_indici_list div').html('');
                return 0;
            }


            //Controllo Effettiva Variazione
            if (parseInt(id_tipologia) !== 0 && id_tipologia_old !== parseInt(id_tipologia)) {

                pulisciDdlAccessorie();
                //Nascondo i pannelli
                nascondiPnlAccessori();

                //Impostazione Descrizione = Tipologia in caso di nuovo documento
                //let descrizione = $("#txbDescrizione").val();
                //if (descrizione === "" || descrizione === undefined || descrizione === null) {
                if ($('#txbID').val() == undefined || $('#txbID').val() == "") {
                    $('#txbDescrizione').val(ddl.dataItem().nome);
                }

                if (jQuery.isNumeric(id_area) === false || id_area !== id_area2) {

                    //Selezione Area
                    $('#ddlArea').data("kendoDropDownList").value(id_area2);

                }


                //mostro i campi accessori in base alla tipologia selezionata
                Leggi_Tipo_Entita_Chiavi();


                id_area = $('#ddlArea').val();
                //indexGrigliaDaMostrare_TipoEntitaSecondario = 0; //Nel casoK di ricette = 1
                var vPiva = $(cPiva).val();
                if (vPiva == "") {
                    vPiva = $('#ddlAzienda').val();
                }

                switch (id_area) {
                    case "1": //Contatti
                        ddlContatto_Load();

                        break;

                    case "2": //Macchine
                        ddlMacchina_Load(id_tipologia);

                        break;

                    case "7": //Uma_Carburanti --> Richieste
                        ddlUma_Carburanti_Load();

                        break;

                    case "3"://Analisi
                        ddlAnalisi_Load();

                        break;

                    case "10"://Documenti Contabili  

                        griglia_Riferimenti_Load(vPiva, id_area, $(cIdAgenda).val(), parseInt(id_tipologia));

                        break;

                    case "13"://Carichi e Scarichi Magazzino  
                        indexGrigliaDaMostrare_TipoEntitaSecondario = 0;
                        griglia_Riferimenti_Load(vPiva, id_area, $(cIdAgenda).val(), parseInt(id_tipologia));


                        break;

                    case "11": //QDC
                        //se siamo in inserimento con tipologia = QDC, allora imposto come pannello principale le operazioni
                        //in caso di oagina richiesta da Agenda, Brogliaccio, RIcette.. se Ricetta_Operazione_Cod è valorizzato imposto il pannello principale 1 (RicetteBrogliaccio)
                        if (hfId_Elenco_val == "" || hfId_Elenco_val == "-1") {
                            indexGrigliaDaMostrare_TipoEntitaSecondario = 0;

                            if ($(cRicetta_Operazione_Cod).val() != 0) {
                                indexGrigliaDaMostrare_TipoEntitaSecondario = 1;
                            }
                        }

                        griglia_Attivita_RicetteBrogliaccio_Visite_Load(vPiva, id_area, $(cRicetta_Operazione_Cod).val(), $(cIdAgenda).val(), parseInt(id_tipologia), indexGrigliaDaMostrare_TipoEntitaSecondario);

                        break;

                    case "12": //CATASTO
                        if (id_tipologia == -27) { //Possesso Particelle

                            griglia_ParticelleCatastali_Load(vPiva, parseInt(id_tipologia));
                        } else { //Al momento non esiste uno switch fra entita Rifeirmento e PArticella Catastale,
                            //diamo per scontato che qualsiasi tipologia creata dall'utente sotto l'area catasto sia di collegata ad un'Agenda (quindi alla griglia Riferimenti)
                            griglia_Riferimenti_Load(vPiva, id_area, $(cIdAgenda).val(), parseInt(id_tipologia));
                        }
                        break;
                }

                //TODO Mouad if id_tipologia in (-18,-19,-20,-21)
                //Todo DDT della Agenda filtrato per tipologia;
                /* ddlAgenda_Load($(cPiva).val(),,);*/
                //creaKendoDropDownList("ddlAgenda", { read: RiempicmbValidazione }, "Validazione_Des", "Validazione_Cod").bind("change", cmbValidazione_change);


                CreaIndici(id_area, id_tipologia);

                //if ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0)) {
                //    //se creo documento dalla pagina dei DDT / QDC  (quindi con idAgenda / ricetta_destinazione_cod già valorizzati), riempio gli indici protetti
                //    selezionaRiga_Riferimenti()
                //}

                id_area_old = parseInt(id_area);
                id_tipologia_old = parseInt(id_tipologia);

                leggiDataScadenza_daTipologia();

                if (DataDefault.substring(6, 10) == "1900") {
                    let annoCorrente = new Date();
                    annoCorrente = annoCorrente.getFullYear();

                    DataDefault = DataDefault.substring(0, 6) + annoCorrente;
                }

                if (id_area == 10 || id_area == 11) {
                    //ddlAgenda_OnDataBound(); anna to do 
                }

                //solo se si tratta di contratti d'affitto, propone la data scadenza
                if (id_tipologia != -26)
                    $('#Txt_Data_Scadenza').val(DataDefault);
                $('#hf_FlagDataScadenzaObbligatoria').val(FlagDataScadenzaObbligatoria);

                if (FlagDataScadenzaObbligatoria) {
                    //Se il flag è true ma non è stata inserita una data scadenza, la rendo visibile E obbligatoria
                    $("#Lbl_Data_Scadenza").addClass("lbl_required");
                } else {
                    $("#Lbl_Data_Scadenza").removeClass("lbl_required");
                }

                //Anna 29/04/22: aggiunto multiselect tipologie al documento
                // in caso di documento > utente abilitato > inserimento, pulisco la select e faccio partire il caricamento della multi tipologia                
                if ($(cModalita).val() === "doc" && UtenteAbilitatoScrittura &&
                    ($("input[id*='hfId_Elenco']").val() === "" || $("input[id*='hfId_Elenco']").val() === "-1")) {
                    $("#divMultiTipologia").show();

                    multiSelectTipologia_Load();
                }

                //Marco: Impostazione Indici Passati da Audit
                if ($(cIdAgenda).val() == "0" && $(cArea_Provenienza).val() == "110") {
                    ddlTrasportiValorizzaIndici();
                }

                //Carlo: Imposta Indice Fornitore passato da Audit
                if ($(cIdAgenda).val() == "0" && $(cSito_Provenienza).val() == "7" && $(cArea_Provenienza).val() == "101") {
                    let indici = $(cIndici).val().toString().split(',');
                    if (indici[0] == "Fornitore" && indici.length > 1) {
                        let indice = indici[1].split('-');
                        if (indice.length > 1) {
                            // KendoDDL("ddlArea").enable(false);
                            KendoDDL("ddlTipologia").enable(false);
                            Set_KendoDDLValue("ddl2_" + indice[0], indice[1]);
                            KendoDDL("ddl2_" + indice[0]).enable(false);
                        }
                    }
                }

            }
        }
    }
}
// #endregion

// ----------------------
//  DDL OPERAZIONI (agenda) !!NON USATA!! --> SOSTITUITA DA GRIGLIA
// ----------------------
function ddlAgenda_Load(piva, idArea, idAgenda, idTipologia) {
    $('#ddlAgenda').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: function (options) {
                    RiempiDdlAgenda(options, piva, idArea, idAgenda, idTipologia);
                }
            }
        },
        dataTextField: "des_lib",
        dataValueField: "id_agenda",
        optionLabel: { "des_lib": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "id_agenda": "" },
        autoWidth: true
    });
    /*    aggiornaIndice(dataDocumento, nrDocumento, contattoDocumento);*/
}

function ddlAgenda_Change() {

    var item = KendoDDL("ddlAgenda").dataItem();

    if (item != null && item.Id_Agenda != null) {
        var dataDocumento = item.Data_Movimento;
        var nrDocumento = "";
        if (item.Doc_Numero_Visualizzato != "") {
            nrDocumento = item.Doc_Numero_Visualizzato;
        } else {
            nrDocumento = item.Doc_Numero_Sin + item.Doc_Numero + item.Doc_Numero_Des;
        }
        var contattoDocumento = item.Cod_Contatto;

        $('#txbDescrizione').val($('#ddlTipologia').data("kendoDropDownList").text() + " - " + item.des_lib);

        //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
        //if (hfId_Elenco_val === "" || hfId_Elenco_val === "-1") {
        //    aggiornaIndice(dataDocumento, nrDocumento, contattoDocumento);
        //}
    }
}

function ddlAgenda_OnDataBound(e) {
    var ds = KendoDDL('ddlAgenda').dataSource.data();
    var dropdownlist = $("#ddlAgenda").data("kendoDropDownList");
    if (ds.length == 1) {
        dropdownlist.select(1); //seleziono l'elemento
        ddlAgenda_Change();
    }
}

function aggiornaIndice(dataDocumento, nrDocumento, contattoDocumento) {
    if (dataDocumento != null && $("[id$='-1000']") != null && $("[id$='-1000']") != undefined) {
        if ($('input[id$="-1000"]').data("kendoDatePicker") != null && $('input[id$="-1000"]').data("kendoDatePicker") != undefined) {
            $('input[id$="-1000"]').data("kendoDatePicker").value(formattedDate(new Date(dataDocumento), "/"))
        }
    }
    if (nrDocumento != null && $("[id$='-1001']") != null && $("[id$='-1001']") != undefined) {
        $("[id$='-1001']").val(nrDocumento);
    }
    if (contattoDocumento != null && $("[id$='-1002']") != null && $("[id$='-1002']") != undefined) {
        /*$("[id$='-1002']").data('kendoDropDownList').value(contattoDocumento);*/
        if ($("[name$='-1002']").data("kendoDropDownList") != null && $("[name$='-1002']").data("kendoDropDownList") != undefined) {
            $("[name$='-1002']").data("kendoDropDownList").value(contattoDocumento);
        }
    }
}


// ----------------------
//  DDL RICETTA (agenda) !!NON USATA!! --> SOSTITUITA DA GRIGLIA
// ----------------------
function ddlRicetta_Load(piva, idArea, idAgenda, RicettaOperazioneCod) {
    $('#ddlRicetta').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: function (options) {
                    RiempiDdlRicetta(options, piva, idArea, RicettaOperazioneCod, idTipologia);
                }
            }
        },
        dataTextField: "des_lib",
        dataValueField: "id_agenda",
        optionLabel: { "des_lib": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "id_agenda": "" },
        autoWidth: true
    });
}


// #region AZIENDA
async function ddlAzienda_Load(piva) {

    let optionLabel = { "rag_soc": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "piva": "" }
    let dati = await CaricaDatiDdlAzienda(piva);
    //let t = {read: function (options) { RiempiDdlAzienda(options, piva); }}
    let t = { read: function (options) { options.success(dati); } }
    creaKendoDropDownList('ddlAzienda', t, "rag_soc", "piva", undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true, optionLabel);
    //$('#ddlAzienda').kendoDropDownList({
    //    filter: "contains",
    //    dataSource: {
    //        transport:
    //        {
    //            read: function (options) {
    //                RiempiDdlAzienda(options, piva);
    //            }
    //        }
    //    },
    //    dataTextField: "rag_soc",
    //    dataValueField: "piva",
    //    optionLabel: { "rag_soc": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "piva": "" },
    //    autoWidth: true,
    //    dataBound: ddlAzienda_OnDataBound
    //});


    // Se c'è solo un valore ed il parametro qualitativo è obbligatorio lo imposto in automatico
    if (KendoDDL("ddlAzienda").dataSource._data.length === 1) {
        await Set_KendoDDLValueVirtual("ddlAzienda", KendoDDL("ddlAzienda").dataSource._data[0].piva);
        ddlAzienda_Change();
    }
}

function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    }
}

function ddlAzienda_Change() {

    //Pulisco i controlli
    pulisciDdlCategorie();
    pulisciDdlAccessorie();

    //Nascondo i pannelli
    nascondiPnlAccessori();

    if (KendoDDL("ddlAzienda").value() !== "") {
        //Carico le Aree
        elencoaree = ricercaDdlArea()
        ddlArea_Load();
        //ddlArea_Load(false);
    }
}
// #endregion

// #region CONTATTO
function ddlContatto_Load() {

    $('#ddlContatto').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlContatto } },
        dataTextField: "nome",
        dataValueField: "cod_contatto",
        optionLabel: { "nome": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "cod_contatto": "" },
        autoWidth: true,
        dataBound: ddlContatto_OnDataBound
    });

}

function ddlContatto_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlContatto.onchange(); //forzo l'evento di onchange
    }
}
// #endregion

// #region MACCHINA
function ddlMacchina_Load(id_tipologia) {

    $('#ddlMacchina').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlMacchina } },
        dataTextField: "nome",
        dataValueField: "chiave",
        optionLabel: { "nome": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "chiave": "" },
        autoWidth: true,
        dataBound: ddlMacchina_OnDataBound,
        change: function (e) {

            ddlMacchina_Change();

        }
    });

}

function ddlMacchina_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlMacchina_Change(); //forzo l'evento di onchange
    }
}

function ddlMacchina_Change() {

    // Imposto la descrizione
    var piva = $('#ddlAzienda').val();
    var mac_cod = -1;

    let chiave = $('#ddlMacchina').val().split('|');
    if (chiave.length > 0) {
        mac_cod = chiave[chiave.length - 1]
    }
    var parametri = kendo.stringify({ "piva": piva, "mac_cod": mac_cod });

    ajaxAgronicaSync(indirizzohttp + "/LeggiMacchina",
        parametri,
        false,
        function (risposta) {
            $('#txbDescrizione').val(risposta.RispostaStringa);
        }, null);
}

// #endregion

// #region UMA
function ddlUma_Carburanti_Load() {

    $('#ddlUma_Carburanti').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlUma_Carburanti } },
        dataTextField: "Numero",
        dataValueField: "richiesta_Cod",
        optionLabel: { "Numero": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "richiesta_Cod": "" },
        autoWidth: true,
        dataBound: ddlUma_Carburanti_OnDataBound
    });

    //Ripristino valore default bloccato
    if ($(cRichiesta_Cod).val() !== "0") {
        Set_KendoDDLValue("ddlUma_Carburanti", $(cRichiesta_Cod).val());
    }
}

function ddlUma_Carburanti_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlUma_Carburanti.onchange(); //forzo l'evento di onchange
    }
}
// #endregion

// #region ANALISI
function ddlAnalisi_Load() {

    $('#ddlAnalisi').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiDdlAnalisi } },
        dataTextField: "nome",
        dataValueField: "analisi_testata_cod",
        optionLabel: { "nome": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "analisi_testata_cod": "" },
        autoWidth: true,
        dataBound: ddlAnalisi_OnDataBound
    });

}

function ddlAnalisi_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlAnalisi.onchange(); //forzo l'evento di onchange
    }
}
// #endregion

// #region PIANO CONCIMAZIONE
function ddlPianoConcimazione_Load() {

    //$('#ddlPianoConcimazione').kendoDropDownList({
    //    filter: "contains",
    //    dataSource: { transport: { read: RiempiDdlPianoConcimazione } },
    //    dataTextField: "nome",
    //    dataValueField: "pc_testata_cod",
    //    optionLabel: { "pc_testata_des": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "pc_testata_cod": "" },
    //    autoWidth: true,
    //    dataBound: ddlPianoConcimazione_OnDataBound
    //});
}

function ddlPianoConcimazione_OnDataBound(e) {
    //var ds = this.dataSource.data();
    //if (ds.length == 1) {
    //    this.select(1); //seleziono l'elemento 
    //    //ddlPianoConcimazione.onchange(); //forzo l'evento di onchange
    //}
}
// #endregion

// #region PUA
function ddlPua_Load() {

    //$('#ddlPua').kendoDropDownList({
    //    filter: "contains",
    //    dataSource: {transport: { read: RiempiDdlPua }},
    //    dataTextField: "nome",
    //    dataValueField: "pua_cod",
    //    optionLabel: { "nome": TraduzioneMultiResx(scadCreaModItemResx, "Seleziona", "Seleziona").toUpperCase() + "...", "pua_cod": "" },
    //    autoWidth: true,
    //    dataBound: ddlPua_OnDataBound
    //});

}

function ddlPua_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlPua.onchange(); //forzo l'evento di onchange
    }
}
// #endregion

// #region PARTICELLE CATASTALI
function griglia_ParticelleCatastali_Load(piva, id_tipologia) { //to do
    impostaDateRiferimenti();

    if (hfId_Elenco_val == "" || hfId_Elenco_val == "-1") {
        $("#filtroDate").hide();
        $("#cercaParticelleCatastali").hide();
    }

    ricercaParticelleCatastali(piva); //to do

}
// #endregion

// #region RIFERIMENTI
function griglia_Riferimenti_Load(piva, idArea, idAgenda, id_tipologia) {
    impostaDateRiferimenti();

    let dataDa = $("#txt_data_da").val();
    let dataA = $("#txt_data_a").val();

    if (hfId_Elenco_val == "" || hfId_Elenco_val == "-1" && $(cIdAgenda).val() == 0) {
        $("#filtroDate").show();
        $("#cercaRiferimenti").show();
    }

    ricercaRiferimenti(piva, idAgenda, id_tipologia, dataDa, dataA);

}
// #endregion

// #region GRIGLIE ATTIVITA, RICETTE/BROGLIACCIO, VISITE 
function griglia_Attivita_RicetteBrogliaccio_Visite_Load(piva, idArea, cRicetta_Operazione_Cod, idAgenda, id_tipologia, indexGrigliaDaMostrare) {
    if ((idAgenda != 0 && cRicetta_Operazione_Cod != 0)) {
        mostraEntrambeGriglie_Riferimenti_RicetteBrogliaccio = true;
    }

    //if ($(cRicetta_Operazione_Cod).val() == 0 && $(cIdAgenda).val() == 0)  
    //if (hfId_Elenco_val == "" || hfId_Elenco_val == "-1" && ($(cRicetta_Operazione_Cod).val() == 0 && $(cIdAgenda).val() == 0)) {
    //impostaDateRiferimenti("", "");
    impostaDateRiferimenti();
    //}
    //}

    let dataDa = $("#txt_data_da").val();
    let dataA = $("#txt_data_a").val();

    //if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {
    //    if (indexGrigliaDaMostrare == 0) {
    //        ricercaOperazioneSelezionata(piva, idArea, idAgenda, id_tipologia, dataDa, dataA);
    //    } else {
    //        ricercaRicettaBrogliaccioSelezionata(idArea, 0, dataDa, dataA);
    //    }
    //}
    if (mostraEntrambeGriglie_Riferimenti_RicetteBrogliaccio == true) {
        ricercaAttivitaVisite(piva, idArea, idAgenda, id_tipologia, dataDa, dataA);
        ricercaRicetteODLBrogliaccio(idArea, cRicetta_Operazione_Cod, dataDa, dataA) //idricetta

        $("#TipoOutput_Riferimenti_RicetteODLBrogliaccio_Visite").hide();
    } else {

        $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").kendoButtonGroup({
            index: indexGrigliaDaMostrare,
            selection: "single",
            select: caricaGriglie_Attivita_RicetteODLBrogliaccio_Visite
        }).data("kendoButtonGroup");
        $("#filtroDate").show()

        if (indexGrigliaDaMostrare == 0) {
            $("#Attivita").show();

            ricercaAttivitaVisite(piva, idArea, idAgenda, id_tipologia, dataDa, dataA);

        } else if (indexGrigliaDaMostrare == 2) {
            $("#Visite").show();

            ricercaAttivitaVisite(piva, idArea, idAgenda, id_tipologia, dataDa, dataA);

        } else {
            $("#RicetteODLBrogliaccio").show();

            ricercaRicetteODLBrogliaccio(idArea, cRicetta_Operazione_Cod, dataDa, dataA) //idricetta
        }

        if ((idAgenda != 0 && cRicetta_Operazione_Cod == 0) || (idAgenda == 0 && cRicetta_Operazione_Cod != 0)) {
            $("#TipoOutput_Riferimenti_RicetteODLBrogliaccio_Visite").hide();
        }
    }
}

function impostaDateRiferimenti() { //impostaDateRiferimenti(dataDa, dataA)
    //if (dataDa !== "" || dataA !== "") {
    //    //$("#txt_data_da").kendoDatePicker().val(dataDa);
    //    //$("#txt_data_a").kendoDatePicker().val(dataA);
    //} else {
    if (hfId_Elenco_val == "" || hfId_Elenco_val == "-1" && ($(cRicetta_Operazione_Cod).val() == 0 && $(cIdAgenda).val() == 0)) {
        let oggi = new Date(new Date().getTime()); //'en-GB' aggiunge gli zeri davanti a singole cifre
        let unMesePrima = new Date(new Date().getTime() - 31 * 24 * 60 * 60 * 1000);
        $("#txt_data_da").data("kendoDatePicker").value(unMesePrima);
        $("#txt_data_a").data("kendoDatePicker").value(oggi);
    }
}

function aggiornaIndiciRiferimenti(dataDocumento, nrDocumento, contattoDocumento) {
    //resetto elementi
    $('input[id$="-1000"]').val("");
    $("[id$='-1001']").val("");
    $("[name$='-1002']").val("");

    if (dataDocumento != null && $("[id$='-1000']") != null && $("[id$='-1000']") != undefined) {
        if ($('input[id$="-1000"]').data("kendoDatePicker") != null && $('input[id$="-1000"]').data("kendoDatePicker") != undefined) {
            $('input[id$="-1000"]').data("kendoDatePicker").value(formattedDate(new Date(dataDocumento), "/"))
        }
    }
    if (nrDocumento != null && $("[id$='-1001']") != null && $("[id$='-1001']") != undefined) {
        $("[id$='-1001']").val(nrDocumento);
    }
    if (contattoDocumento != null && $("[id$='-1002']") != null && $("[id$='-1002']") != undefined) {
        /*$("[id$='-1002']").data('kendoDropDownList').value(contattoDocumento);*/
        if ($("[name$='-1002']").data("kendoDropDownList") != null && $("[name$='-1002']").data("kendoDropDownList") != undefined) {
            $("[name$='-1002']").data("kendoDropDownList").value(contattoDocumento);
        }
    }
}
// #endregion

//#region FUNZIONI UTILITY
function nascondiPnlAccessori() {
    var listaPnl = ["pnlCentro", "pnlAppezzamento", "pnlMacchina", "pnlContatto", "pnlAnalisi", "pnlPianoConcimazione", "pnlPua", "pnlUMA",
        "pnlRiferimentiAgenda_Ricette_Catasto", "filtroDate", "cercaRiferimenti", "TipoOutput_Attivita_RicetteODLBrogliaccio_Visite",
        "Riferimenti", "Attivita", "RicetteODLBrogliaccio", "Visite", "ParticelleCatastali", "cercaParticelleCatastali"];

    for (var i in listaPnl) {
        $('#' + listaPnl[i]).hide();
    }

    $('#id_indici_list div').html('');

    bcheckcontatto = false;
    bcheckmacchina = false;
    bcheckuma = false;
    bcheckanalisi = false;
    bcheckagenda = false;
    bcheckricette = false;
}

function pulisciDdlCategorie() {
    var listaDdl = ["ddlArea", "ddlTipologia", "ddlAgenda"];

    for (var i in listaDdl) {
        $('#' + listaDdl[i]).kendoDropDownList({ dataSource: [] });
    }

    $('#txbDescrizione').val("");

    if (multiCmbTipologia) {
        multiCmbTipologia.value([])
        multiCmbTipologia.dataSource = []
        multiCmbTipologia = null
    }
    $("#divMultiTipologia").hide();
}

function pulisciDdlAccessorie() {
    var listaDdl = ["ddlCentro", "ddlAppezzamento", "ddlMacchina", "ddlContatto", "ddlAnalisi", "ddlPianoConcimazione", "ddlPua", "ddlUma_Carburanti"];

    for (var i in listaDdl) {
        $('#' + listaDdl[i]).kendoDropDownList({ dataSource: [] });
    }

    var listaGriglie = ['griglia_Riferimenti', 'griglia_Attivita', 'griglia_RicetteODLBrogliaccio', 'griglia_Visite', 'griglia_ParticelleCatastali']

    for (var i in listaGriglie) {
        $('#' + listaGriglie[i]).empty();
    }

    resettaGriglie()
}

function pulisciDdlTipologie() {
    var listaDdl = ["ddlTipologia"];

    if (KendoDDL('ddlTipologia').dataSource._data.length > 1) {
        for (var i in listaDdl) {
            $('#' + listaDdl[i]).kendoDropDownList({ dataSource: [] });
        }

        $('#txbDescrizione').val("");

        $('#cmbTipologia').dataSource = [];
        $("#divMultiTipologia").hide();


        pulisciDdlAccessorie();
        //Nascondo i pannelli
        nascondiPnlAccessori();
    }
}

function solaLetturaDdl() {
    var listaDdl = ["ddlAzienda", "ddlArea", "ddlTipologia", "ddlAgenda", "ddlCentro", "ddlAppezzamento", "ddlMacchina", "ddlContatto", "ddlAnalisi", "ddlPianoConcimazione", "ddlPua", "ddlUma_Carburanti"];

    for (var i in listaDdl) {
        $('#' + listaDdl[i]).data("kendoDropDownList").enable(false);
    }
}

function AggiornaUsernameUpload() {

    $('input[name$="Txt_Username' + usaUploadMultiplo + '"]').val($(cUsername).val());
    $('input[name$="Txt_Data_Upload' + usaUploadMultiplo + '"]').val(formattedDate(new Date(), '/'));

}

function RiempicmbElenco_Valore(options) {
    options.success(Elenco_Valore);
}

function bloccoControlloDettagli(bloccaAncheAreaTipologia) {
    if (bloccaAncheAreaTipologia == true && $(cMac_Cod).val() == "0")
    {
        KendoDDL("ddlArea").enable(false);
        KendoDDL("ddlTipologia").enable(false);
    }
}

function rimuoviElementiDaDDLArea() {
    //Impletanto con sblocco dell'Area in modifica di un documento:
    // possibile selezionare solo aree con tipoentita_cod = 0
    var area = KendoDDL("ddlArea").dataItem()
    let idArea = KendoDDL("ddlArea").value()
    let idTipologia = KendoDDL("ddlTipologia").value()

    //In modifica di un documento: se l'area selezionata ha tipoentita_cod =0, carico la ddlArea con altre aree aventi tipoentita_cod == 0
    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1" && area.tipoentita_cod == 0) {
        var dummy = elencoaree.slice()
        elencoaree = dummy.filter(function (x) { return x.tipoentita_cod == "0" })
        ddlArea_Load()
    }

    //Caso particolare, area Catasto...
    if (idArea == 12) {
        bloccoControlloDettagli(true)
    }
}

function bloccaControlloAzienda() {
    KendoDDL("ddlAzienda").enable(false);
}

function sbloccaControlliDettagli() {
    //premendo il pulsante Salva e nuovo dalla modifica doc, i controlli rimanevano bloccati
    KendoDDL("ddlAzienda").enable(true);
    KendoDDL("ddlArea").enable(true);
    KendoDDL("ddlTipologia").enable(true);
}

function bloccaTuttiControlli() {

    //KendoDDL("ddlAgenda").enable(false);

    KendoDDL("cmbValidazione" + usaUploadMultiplo).enable(false);

    KendoSwitch("ChkStorico" + usaUploadMultiplo).enable(false)

    $('#txbDescrizione').attr('disabled', 'disabled');
    KendoDate("Txt_Data_Scadenza").enable(false);
    $('#txbNote').attr('disabled', 'disabled');

    $('#Txt_Num_Documento' + usaUploadMultiplo).attr('disabled', 'disabled');
    KendoDate("Txt_Data_Allegato").enable(false);
    KendoDate("Txt_Data_Upload" + usaUploadMultiplo).enable(false);
    KendoSwitch("ChkStorico" + usaUploadMultiplo).enable(false);


    //KendoUpload
    if (usaUploadMultiplo == "") {
        $("#btn_Rimuovi").hide();
        $("#Btn_Sfoglia_Allegato").attr('disabled', 'disabled');
    } else {
        $(".k-dropzone").hide()
        $(".k-action-buttons").hide()
        $("button[name='rimuoviElemento']").hide()
    }
}

function bloccaControlli_AgendaAnalisiMacchina() {
    KendoDDL("ddlMacchina").enable(false);
    KendoDDL("ddlContatto").enable(false);
    KendoDDL("ddlAnalisi").enable(false);
}

function bloccaSbloccaCategoria() {
    var id_area = $('#ddlArea').data("kendoDropDownList").value();
    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();

    if (jQuery.isNumeric(id_tipologia) === false) {
        return;
    }

    //Mostro i pannelli solo se sono le nostre categorie 'speciali'
    if (id_area !== "7" && id_area !== "11" && id_tipologia >= 0) {
        return;
    }

    var parametri = kendo.stringify({ "objP_server": objP_server, "id_tipologia": id_tipologia });
    var controller = "";
    tipoTipoEntitaCod = [];

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Tipo_Entita_Chiavi.asmx/Leggi_Tipo_Entita_Chiavi",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            nascondiPnlAccessori();

            for (var i in risp) {
                controller = "";
                switch (risp[i]["nome_chiave"].toLowerCase()) {
                    case "sa_cod":
                        //$("#pnlCentro").show();
                        break;
                    case "appezza":
                        //$("#pnlAppezzamento").show();
                        break;
                    case "cod_contatto":
                        $("#pnlContatto").show();
                        bcheckcontatto = true;
                        controller = "bcheckcontatto";
                        break;
                    case "mac_cod":
                        $("#pnlMacchina").show();
                        bcheckmacchina = true;
                        controller = "bcheckmacchina";
                        break;
                    case "analisi_testata_cod":
                        $("#pnlAnalisi").show();
                        bcheckanalisi = true;
                        controller = "bcheckanalisi";
                        break;
                    case "pc_testata_cod":
                        //$("#pnlPianoConcimazione").show();
                        break;
                    case "pua_cod":
                        //$("#pnlPua").show();
                        break;
                    case "id_agenda":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckagenda = true;
                        controller = "bcheckagenda";
                        break;
                    case "richiesta_cod":
                        $("#pnlUMA").show();
                        bcheckuma = true;
                        controller = "bcheckuma";
                        break;
                    case "ricetta_operazione_cod":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckricette = true;
                        controller = "bcheckricette";
                        break;
                    case "id_impresexparticelle":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckparticellecatastali = true;
                        controller = "bcheckparticellecatastali";
                        break;
                }

                tipoTipoEntitaCod.push([risp[i]["nome_chiave"].toLowerCase(), "Primario", controller, 0]);
            }
        }, null);


    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreScadenziario/Tipo_Entita_Chiavi.asmx/Leggi_Tipo_Entita_Chiavi_Secondario",
        parametri, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            for (var i in risp) {
                controller = "";
                switch (risp[i]["nome_chiave"].toLowerCase()) {
                    case "sa_cod":
                        //$("#pnlCentro").show();
                        break;
                    case "appezza":
                        //$("#pnlAppezzamento").show();
                        break;
                    case "cod_contatto":
                        $("#pnlContatto").show();
                        bcheckcontatto = true;
                        controller = "bcheckcontatto";
                        break;
                    case "mac_cod":
                        $("#pnlMacchina").show();
                        bcheckmacchina = true;
                        controller = "bcheckmacchina";
                        break;
                    case "analisi_testata_cod":
                        $("#pnlAnalisi").show();
                        bcheckanalisi = true;
                        controller = "bcheckanalisi";
                        break;
                    case "pc_testata_cod":
                        //$("#pnlPianoConcimazione").show();
                        break;
                    case "pua_cod":
                        //$("#pnlPua").show();
                        break;
                    case "id_agenda":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckagenda = true;
                        controller = "bcheckagenda";
                        break;
                    case "richiesta_cod":
                        $("#pnlUMA").show();
                        bcheckuma = true;
                        controller = "bcheckuma";
                        break;
                    case "ricetta_operazione_cod":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckricette = true;
                        controller = "bcheckricette";
                        break;
                    case "id_impresexparticelle":
                        $("#pnlRiferimentiAgenda_Ricette_Catasto").show();
                        bcheckparticellecatastali = true;
                        controller = "bcheckparticellecatastali";
                        break;
                }

                tipoTipoEntitaCod.push([risp[i]["nome_chiave"].toLowerCase(), "Secondario", controller, 0]);
            }
        }, null);
}
// #endregion

//#region INDICI
// ----------------------
function CreaIndici(id_area, id_tipologia) {

    // Pulizia del DIV 
    $('#id_indici_list div').html('');
    let container = document.getElementById("id_indici_list");
    let contaRighe = 0;
    let nrRighe = 0;
    let newRowDiv = null;
    let foundParamQual = false;
    var piva = $('#ddlAzienda').data("kendoDropDownList").value();
    var id_elenco = "";
    var id_alert_entita = "";
    var classeCSS = "";
    var bloccaControllo = (tipoPermessoDaControllare == 2) ? (true) : (false);  //tipoPermessoDaControllare == 2 --> solo lettura

    elencoindici = RicercaIndicixTipologia(id_area, id_tipologia);

    if ($('#txbID').val() !== undefined && $('#txbID').val() !== "") {
        id_elenco = $('#txbID').val();
        id_alert_entita = $('input[id*="hfId_Alert_Entita"]').val();
        elencoindicidb = LeggiEntitaxIndici(id_alert_entita);
    }



    for (var iindice = 0; iindice < elencoindici.length; iindice++) {

        classeCSS = "input-group-addon ";

        if (elencoindici[iindice].ID_Indice !== 0) {
            if (elencoindici[iindice].TipoCampo == 0 ||
                elencoindici[iindice].TipoCampo == 1 ||
                elencoindici[iindice].TipoCampo == 2) {

                // 3 colonne per riga
                if (contaRighe == 1) {
                    contaRighe = 0;
                }

                if (contaRighe == 0) {
                    if (newRowDiv !== null) {
                        container.appendChild(newRowDiv);
                    }
                    nrRighe++;
                    newRowDiv = creaNewRowDiv("id" + nrRighe.toString, "row xo-p-x-15px");
                }


                var newColumnDiv = creaNewColumnBS(12, 12, 12);
                var newDivInputGroup = creaDIV("input-group");
                var newKey = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                //Impostazione Titolo Indice
                var titoloindice = elencoindici[iindice].TitoloIndice;
                var styleObbligatorio = ""

                if (parseInt(elencoindici[iindice].ChkObbligatorio) == 1 || parseInt(elencoindici[iindice].ChkObbligatorio_Tipologia) == 1) {
                    titoloindice = titoloindice + "*";
                    classeCSS += "lbl_required";
                    styleObbligatorio = "font-weight: bold;"
                }


                let tipo_param = "";
                if (parseInt(elencoindici[iindice].TipoCampo) == 0) {
                    //libera imputazione                        


                    switch (elencoindici[iindice].TipoDato) {

                        case "string":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeCSS, tipo_param + newKey, styleObbligatorio));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control"));

                            break;

                        case "date":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeCSS, tipo_param + newKey, styleObbligatorio));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoCalendar"));

                            break;

                        case "numeric":

                            tipo_param = "txt";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeCSS, tipo_param + newKey, styleObbligatorio));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control"));

                            break;

                        case "boolean":

                            tipo_param = "chk";
                            newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeCSS, tipo_param + newKey, styleObbligatorio));
                            newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "kendoSwitch"));

                            break;
                    }
                } else {
                    tipo_param = "ddl";

                    newDivInputGroup.appendChild(creaLabel("lbl" + newKey, titoloindice, classeCSS, tipo_param + newKey, styleObbligatorio));
                    newDivInputGroup.appendChild(creaInputGenerico(tipo_param + newKey, tipo_param + newKey, "form-control"));
                }

                newColumnDiv.appendChild(newDivInputGroup);
                newRowDiv.appendChild(newColumnDiv);

                contaRighe++;
                //}
            }
        }

        // ultimo giro
        if (iindice == elencoindici.length - 1 && newRowDiv !== null) {
            container.appendChild(newRowDiv);
        }
    }


    //Creazione DDL Kendo
    for (var iindice2 = 0; iindice2 < elencoindici.length; iindice2++) {
        if (elencoindici[iindice2].ID_Indice !== 0) {

            var chiave = elencoindici[iindice2].TipoCampo + "_" + elencoindici[iindice2].ID_Indice;

            switch (parseInt(elencoindici[iindice2].TipoCampo)) {

                case 0:

                    switch (elencoindici[iindice2].TipoDato) {

                        case "string":
                            if (bloccaControllo == true) {
                                $("#txt" + chiave).attr('disabled', 'disabled');
                            }
                            break;

                        case "date":
                            var controllo = "txt" + chiave + "";
                            $('#' + controllo).kendoDatePicker({
                                footer: "#: kendo.toString(data, 'd')#",
                                max: new Date(2100, 11, 31)
                            });

                            if (bloccaControllo == true) {
                                KendoDate(controllo).enable(false);
                            }

                            break;

                        case "numeric":
                            var controllo = "txt" + chiave + "";
                            $('#' + controllo).kendoNumericTextBox();

                            if (bloccaControllo == true) {
                                KendoNumericBox(controllo).enable(false)
                            }

                            break;

                        case "boolean":
                            var controllo = "chk" + chiave + "";
                            creaKendoSwitch(controllo);

                            if (bloccaControllo == true) {
                                KendoSwitch(controllo).enable(false)
                            }
                            break;
                    }
                    break;

                default:
                    elencodettagli = RicercaDDLIndice(elencoindici[iindice2].ID_Indice, elencoindici[iindice2].TipoCampo, elencoindici[iindice2].Elenco_Tipo, elencoindici[iindice2].Elenco_Cod, elencoindici[iindice2].Elenco_Cod_String);
                    creaKendoDropDownList("ddl" + chiave, { read: RiempiDDLDettagli }, "Valore_Des", "Valore_Cod");
                    //$("ddl" + chiave).autoWidth = true;

                    if (parseInt(elencoindici[iindice2].Elenco_Tipo) == 1 && piva !== undefined && id_elenco == "") {

                        //in caso di impresa gias --> default = impresa
                        Set_KendoDDLValue("ddl" + chiave, piva);
                    }

                    let ddl = "ddl" + chiave
                    if (classeCSS.includes("lbl_required")) {
                        if (KendoDDL(ddl).dataSource._data.length === 2) {
                            Set_KendoDDLValue(ddl, KendoDDL(ddl).dataSource._data[1].Valore_Cod);
                        }
                    }

                    if (elencoindici[iindice2].Elenco_Tipo == 4 && elencoindici[iindice2].ChkObbligatorio == 1 && $(cIndici).val().split("*")[0] == 20) {
                        Set_KendoDDLValue("ddl" + chiave, $(cIndici).val().split("*")[1]);
                    }

                    if (bloccaControllo == true) {
                        KendoDDL(ddl).enable(false)
                    }
                    break;
            }
        }
    }


    $("#id_indici_list").show();


    // Impostare i valori nei controlli creati sopra
    if (elencoindici !== null && elencoindici.length !== 0 && elencoindicidb !== null && elencoindicidb.length !== 0) {
        for (let iindice3 = 0; iindice3 < elencoindicidb.length; iindice3++) {
            if (elencoindicidb[iindice3].ID_Indice !== 0) {
                if (elencoindicidb[iindice3].TipoCampo == 0 ||
                    elencoindicidb[iindice3].TipoCampo == 1 ||
                    elencoindicidb[iindice3].TipoCampo == 2) {


                    for (let iindice4 = 0; iindice4 < elencoindici.length; iindice4++) {
                        if (elencoindici[iindice4].ID_Indice !== 0) {
                            if (elencoindici[iindice4].TipoCampo == 0 ||
                                elencoindici[iindice4].TipoCampo == 1 ||
                                elencoindici[iindice4].TipoCampo == 2) {

                                if (parseInt(elencoindici[iindice4].ID_Indice) == parseInt(elencoindicidb[iindice3].ID_Indice) &&
                                    parseInt(elencoindici[iindice4].TipoCampo) == parseInt(elencoindicidb[iindice3].TipoCampo)) {

                                    var chiave2 = elencoindici[iindice4].TipoCampo + "_" + elencoindici[iindice4].ID_Indice;

                                    switch (parseInt(elencoindici[iindice4].TipoCampo)) {

                                        case 0: //Valore Libero

                                            switch (elencoindici[iindice4].TipoDato) {

                                                case "numeric":

                                                    Set_KendoNumTBValue("txt" + chiave2, elencoindicidb[iindice3].Valore_Des);
                                                    break;

                                                case "boolean":

                                                    let Valore_Boolean = elencoindicidb[iindice3].Valore_Des === "True";

                                                    setKendoSwitch("chk" + chiave2, Valore_Boolean);
                                                    break;

                                                default:

                                                    $("#txt" + chiave2).val(elencoindicidb[iindice3].Valore_Des);
                                                    break;

                                            }

                                            break;

                                        case 1: //Valori Dettagli

                                            Set_KendoDDLValue("ddl" + chiave2, elencoindicidb[iindice3].ID_Indice_Det);
                                            break;

                                        case 2: //Elenco

                                            Set_KendoDDLValue("ddl" + chiave2, elencoindicidb[iindice3].Elenco_Val);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

function RiempiDDLDettagli(options) {
    options.success(elencodettagli);
}

// Leggere i valori dai controlli per salvarli su DB
function AggiungiValoriIndici() {

    let arrParams = [];

    if (elencoindici !== undefined && elencoindici !== null && elencoindici.length !== 0) {


        for (var iindice = 0; iindice < elencoindici.length; iindice++) {
            if (elencoindici[iindice].ID_Indice !== 0) {
                if (elencoindici[iindice].TipoCampo == 0 ||
                    elencoindici[iindice].TipoCampo == 1 ||
                    elencoindici[iindice].TipoCampo == 2) {

                    var chiave3 = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                    let vID_Indice = parseInt(elencoindici[iindice].ID_Indice);
                    let vID_Indice_Det = 0;
                    let vElenco_Val = 0;
                    let vValore_Des = "";
                    let vTipoCampo = elencoindici[iindice].TipoCampo;


                    switch (parseInt(elencoindici[iindice].TipoCampo)) {

                        case 0: //Valore Libero

                            switch (elencoindici[iindice].TipoDato) {

                                case "boolean":

                                    vValore_Des = getKendoSwitch("chk" + chiave3)
                                    break;

                                default:

                                    vValore_Des = $('input[name$="txt' + chiave3 + '"]').val();
                                    break;
                            }

                            break;

                        case 1: //Dettaglio

                            vID_Indice_Det = Get_KendoDDLValue("ddl" + chiave3);
                            break;

                        case 2: //Elenco

                            vElenco_Val = Get_KendoDDLValue("ddl" + chiave3);
                            break;
                    }


                    //verifico se è già presente nell'array
                    let elems = arrayLookup(arrParams, "ID_Indice", chiave3);

                    //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                    if (elems !== undefined && elems !== null) {

                        elems.ID_Indice = vID_Indice;
                        elems.ID_Indice_Det = vID_Indice_Det;
                        elems.Elenco_Val = vElenco_Val;
                        elems.Valore_Des = vValore_Des;
                        elems.TipoCampo = vTipoCampo;

                    } else {
                        let res = {
                            ID_Indice: vID_Indice,
                            ID_Indice_Det: vID_Indice_Det,
                            Elenco_Val: vElenco_Val,
                            Valore_Des: vValore_Des,
                            TipoCampo: vTipoCampo

                        };
                        arrParams.push(res);
                    }
                }
            }
        }
    }

    return arrParams;

}
// #endregion

//#region SALVA / ESCI
function CheckDatiNecessariInseriti() {

    bDatiNecessariInseriti = true;

    //Controllo Azienda
    if (KendoDDL("ddlAzienda").value() == "") {
        kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "AziendaNonImpostataCorrettamente", "Azienda non impostata correttamente."));
        bDatiNecessariInseriti = false;
        return 0;
    }

    //Controllo Tipologia
    if (KendoDDL("ddlTipologia").value() == 0) {
        kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "TipologiaNonImpostataCorrettamente", "Tipologia non impostata correttamente."));
        bDatiNecessariInseriti = false;
        return 0;
    }

    if (CheckDatiNecessariInseriti_suEntita() == false) {
        bDatiNecessariInseriti = false;
        return 0;
    }

    //Controllo Scadenza
    let data_scadenza = kendo.parseDate($("#Txt_Data_Scadenza").val());

    if (data_scadenza == null && $(cModalita).val() == "") {
        kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "DataScadenzaNonImpostataCorrettamente", "Data scadenza non impostata correttamente."));
        bDatiNecessariInseriti = false;
        return 0;
    }

    //Controllo Scadenza sui documenti (tipologia con scadenza oobbligatoria)
    if (FlagDataScadenzaObbligatoria) {
        if (data_scadenza === null || data_scadenza === "") {
            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "DataScadenzaObbligatoriaPerQuestaTipologiaDocumento", "Data scadenza obbligatoria per questa tipologia documento."));
            bDatiNecessariInseriti = false;
            return 0;
        }
    }

    //Controllo Allegato
    if ($(cModalita).val() == "doc") {
        if (usaUploadMultiplo == "") {
            if (($('#Txt_Documento_Allegato').val() == "" || $('#Txt_Documento_Allegato').val() == undefined)) {
                kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireAllegato", "Inserire l'Allegato."));
                bDatiNecessariInseriti = false;
                return 0;
            }

            //if (($('#Txt_Documento_Allegato').val() == "" || $('#Txt_Documento_Allegato').val() == undefined) && data_scadenza == null) {
            //    kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireScadenzaOAllegatoPerSalvare", "Inserire la Scadenza o l'Allegato Prima di Richiedere il salvataggio."));
            //    bDatiNecessariInseriti = false;
            //    return 0;
            //}
        } else {
            if (listaDocumenti.length == 0) {
                kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireAllegato", "Inserire l'Allegato."));
                bDatiNecessariInseriti = false;
                return 0;
            }
        }
    }
    //else {
    //    if (usaUploadMultiplo == "") {
    //        if (($('#Txt_Documento_Allegato').val() == "" || $('#Txt_Documento_Allegato').val() == undefined) && data_scadenza == null) {
    //            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireScadenzaOAllegatoPerSalvare", "Inserire la Scadenza o l'Allegato Prima di Richiedere il salvataggio."));
    //            bDatiNecessariInseriti = false;
    //            return 0;
    //        }
    //    } else {
    //        if ((listaDocumenti.length == 0) && data_scadenza == null) {
    //            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireScadenzaOAllegatoPerSalvare", "Inserire la Scadenza o l'Allegato Prima di Richiedere il salvataggio."));
    //            bDatiNecessariInseriti = false;
    //            return 0;
    //        }
    //    }
    //}    


    let descrizione = $("#txbDescrizione").val();
    if (descrizione === "" || descrizione === undefined || descrizione === null) {
        kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "InserireDescrizione", "Inserire la descrizione."));
        bDatiNecessariInseriti = false;
        return 0;
    }

    let antiVirus_isOn = checkAntiVirus_isON(objP_super_server)
    if (antiVirus_isOn == true) {
        if (checkAntivirus() == false) {
            bDatiNecessariInseriti = false;
            return 0;
        }
    }

    // Dati Necessari Schema
    var errore = DatiNecessariSchema();

    if (errore !== "") {
        bDatiNecessariInseriti = false
        kendo.alert(errore);
    }


    // let tipo_doc = KendoDDL("Cmb_TipoDocumento").value();
    //if (tipo_doc === "" || tipo_doc === undefined || tipo_doc === null) {
    //    kendo.alert("Inserire il tipo documento. ");
    //    return false;
    //}

    //let num_doc = $("#Txt_Num_Documento").val();
    //if (num_doc === "" || num_doc === undefined || num_doc === null) {
    //    kendo.alert("Inserire il numero documento. ");
    //    bDatiNecessariInseriti = false;
    //    return 0;
    //}

    //let id_elenco = nuovo?0:$("#Txt_ID_Elenco").val();
    //let data = gridAllegati.dataSource.data();
    //let data_inizio = data_rilascio != null ? data_rilascio : new Date(1900, 1, 1);
    //let data_fine = data_scadenza != null ? data_scadenza : new Date(2100, 11, 31);
    //for (var i = 0; i < data.length; i++) {
    //    if (data[i].ID_Tipologia == tipo_doc && data[i].ID_Elenco != id_elenco) {
    //        if (data_inizio <= data[i].Data_Scadenza && data[i].Validazione_Data <= data_fine) {
    //            kendo.alert("L'intervallo di date inserite si sovrappone con uno già presente.");
    //            return false;
    //        }
    //    }
    //}

    //if (data_rilascio != null && data_scadenza != null && data_scadenza < data_rilascio) {
    //    kendo.alert("La data scadenza deve essere successiva alla data rilascio.");
    //    bDatiNecessariInseriti = false;
    //    return 0;
    //}


    if (elencoindici !== undefined && elencoindici !== null && elencoindici.length !== 0) {

        for (var iindice = 0; iindice < elencoindici.length; iindice++) {
            if (elencoindici[iindice].ID_Indice !== 0 && (parseInt(elencoindici[iindice].ChkObbligatorio) == 1) || parseInt(elencoindici[iindice].ChkObbligatorio_Tipologia) == 1) {
                if (elencoindici[iindice].TipoCampo == 0 ||
                    elencoindici[iindice].TipoCampo == 1 ||
                    elencoindici[iindice].TipoCampo == 2) {

                    var chiave3 = elencoindici[iindice].TipoCampo + "_" + elencoindici[iindice].ID_Indice;

                    var valore_cod = 0;
                    var valore_des = "";

                    switch (parseInt(elencoindici[iindice].TipoCampo)) {

                        case 0: //Valore Libero
                            switch (elencoindici[iindice].TipoDato) {

                                case "date":
                                    valore_des = kendo.parseDate($("#txt" + chiave3).val());
                                    if (valore_des == null) {
                                        valore_des = "";
                                    }
                                    break;

                                case "boolean":
                                    valore_des = "ok"; //sempre valorizzato
                                    break;

                                default:
                                    valore_des = $('input[name$="txt' + chiave3 + '"]').val();
                                    break;
                            }
                            break;

                        default:
                            valore_cod = Get_KendoDDLValue("ddl" + chiave3);
                            break;
                    }


                    if ((valore_cod == 0 || valore_cod == "") && valore_des == "") {
                        kendo.alert(
                            kendo.format(
                                TraduzioneMultiResx(scadCreaModItemResx, "ValoreObbligatorioXNonImpostato", "Valore obbligatorio {0} non impostato correttamente"),
                                elencoindici[iindice].TitoloIndice
                            ),
                            "DIV_Messaggi"
                        );
                        //MessaggioErrore_Bootstrap("Valore obbligatorio " + elencoindici[iindice].TitoloIndice + " non impostato correttamente.", "DIV_Messaggi");
                        bDatiNecessariInseriti = false;
                        return 0;
                    }
                }
            }
        }
    }

    if (checkDataDocumentoObbligatoria) {
        data = $('#Txt_Data_Allegato').kendoDatePicker().data("kendoDatePicker").value();
        if (data == null || data == "") {
            bDatiNecessariInseriti = false;
            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "DataAllegatoMancante", "Data allegato mancante")) 
        }
    }

    return 0;

}

function CheckDatiNecessariInseriti_suEntita() {

    var msgError = "";
    var countTipoEntitaCod = tipoTipoEntitaCod.length;
    var passaControlli = false;
    var hasSecondario = false;

    let idArea = $("#ddlArea").val()
    let idTipologia = $("#ddlTipologia").val()

    for (let i = 0; i < countTipoEntitaCod; i++) {
        if (tipoTipoEntitaCod[i][2] != null && tipoTipoEntitaCod[i][2] != "") {
            if (tipoTipoEntitaCod[i][1] != null && tipoTipoEntitaCod[i][1] != "" && tipoTipoEntitaCod[i][1] == "Secondario") {
                hasSecondario = true
            }

            if (idArea == 12) {
                hasSecondario = false
            }

            switch (tipoTipoEntitaCod[i][2]) {
                case "bcheckcontatto":
                    //Controllo Entità
                    if ($('#ddlContatto').data("kendoDropDownList").value() == 0 && bcheckcontatto) {
                        msgError += TraduzioneMultiResx(scadCreaModItemResx, "ContattoNonImpostatoCorrettamente", "Contatto non impostato correttamente.") + "</br>";
                    } else {
                        passaControlli = true;
                    }
                    break;
                case "bcheckmacchina":
                    if ($('#ddlMacchina').data("kendoDropDownList").value() == 0 && bcheckmacchina) {
                        msgError += TraduzioneMultiResx(scadCreaModItemResx, "MacchinaNonImpostataCorrettamente", "Macchina non impostata correttamente.") + "</br>";
                    } else {
                        passaControlli = true;
                    }
                    break;
                case "bcheckuma":
                    if ($('#ddlUma_Carburanti').data("kendoDropDownList").value() == 0 && bcheckuma) {
                        msgError += TraduzioneMultiResx(scadCreaModItemResx, "RichiestaUmaCarburantiNonImpostataCorrettamente", "Richiesta Uma Carburanti non impostata correttamente.") + "</br>";
                    } else {
                        passaControlli = true;
                    }
                    break;
                case "bcheckanalisi":
                    if ($('#ddlAnalisi').data("kendoDropDownList").value() == 0 && bcheckanalisi) {
                        msgError += TraduzioneMultiResx(scadCreaModItemResx, "AnalisiNonImpostataCorrettamente", "Analisi non impostata correttamente.") + "</br>";
                    } else {
                        passaControlli = true;
                    }
                    break;
                case "bcheckagenda":
                    var elementoSelezionato;
                    var messaggio = ""

                    var griglia_Attivita = $("#griglia_Attivita").data("kendoGrid"),
                        griglia_Visite = $("#griglia_Visite").data("kendoGrid"),
                        griglia_Riferimenti = $("#griglia_Riferimenti").data("kendoGrid");

                    if (idArea == 11) {

                        if (griglia_Attivita != undefined) {
                            elementoSelezionato = griglia_Attivita.dataItem(griglia_Attivita.select());
                        }
                        if (elementoSelezionato == null) {
                            messaggio = TraduzioneMultiResx(scadCreaModItemResx, "AttivitaNonImpostataCorrettamente", "Attività non impostata correttamente.") + "</br>";
                        }

                        //Se non è stata scelta un attività, controllo le Visite
                        if (elementoSelezionato == null) {
                            if (griglia_Visite != undefined) {
                                elementoSelezionato = griglia_Visite.dataItem(griglia_Visite.select());
                            }
                            if (elementoSelezionato == null) {
                                messaggio += TraduzioneMultiResx(scadCreaModItemResx, "VisitaNonImpostataCorrettamente", "Visita non impostata correttamente.") + "</br>";
                            }
                        }
                    } else if (idArea == 12 && idTipologia == -27) {
                        //Se siamo nell'area Catasto (12) e ho selezionato la tipologia Possesso Particelle (-27), ignoro la griglia riferimenti
                    } else {
                        if (griglia_Riferimenti != undefined) {
                            elementoSelezionato = griglia_Riferimenti.dataItem(griglia_Riferimenti.select());
                        }
                        if (elementoSelezionato == null) {
                            messaggio = TraduzioneMultiResx(scadCreaModItemResx, "RiferimentoNonImpostatoCorrettamente", "Riferimento non impostato correttamente.") + "</br>";
                        }
                    }

                    if (elementoSelezionato == null && bcheckagenda) {
                        msgError += messaggio
                    } else {
                        passaControlli = true;
                    }

                    break;
                case "bcheckricette":
                    var griglia_RicetteODLBrogliaccio = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid");
                    var ricettaBrogliaccioSelezionata;

                    if (griglia_RicetteODLBrogliaccio != undefined) {
                        ricettaBrogliaccioSelezionata = griglia_RicetteODLBrogliaccio.dataItem(griglia_RicetteODLBrogliaccio.select());
                    }

                    if (ricettaBrogliaccioSelezionata == null && bcheckricette) { //to do
                        msgError += TraduzioneMultiResx(scadCreaModItemResx, "RicettaODLBrogliaccioNonImpostatiCorrettamente", "Ricetta/ODL o Brogliaccio non impostati correttamente.") + "</br>";
                    } else {
                        passaControlli = true;
                    }
                    break;

                case "bcheckparticellecatastali":
                    if (idArea == 12 && idTipologia == -26) {
                        //Se siamo nell'area Catasto (12) e ho selezionato la tipologia Contratti Affitto (-26), ignoro la griglia PArticelle Catastali
                    } else {
                        var griglia_ParticelleCatastali = $("#griglia_ParticelleCatastali").data("kendoGrid");
                        var particellaCatastaleSelezionata;

                        if (griglia_ParticelleCatastali != undefined) {
                            particellaCatastaleSelezionata = griglia_ParticelleCatastali.dataItem(griglia_ParticelleCatastali.select());
                        }

                        if (particellaCatastaleSelezionata == null && bcheckparticellecatastali) { //to do
                            msgError += TraduzioneMultiResx(scadCreaModItemResx, "ParticellaCatastaleNonImpostataCorrettamente", "Particella catastale non impostata correttamente.") + "</br>";
                        } else {
                            passaControlli = true;
                        }
                    }
                    break;
            }
        }
    }


    if (countTipoEntitaCod > 0 && passaControlli == false) {
        if (hasSecondario) {
            msgError = msgError + "</br>" + "(" + TraduzioneMultiResx(scadCreaModItemResx, "ObbligatorioValorizzareElemento", "Obbligatorio valorizzare un elemento.") + ")";
        }
        kendo.alert(msgError)
        return false;
    } else {
        return true
    }
}

function checkAntivirus() {
    if (usaUploadMultiplo == "") {

        //Antivirus  in caso di virus verrà segnalato e rimosso l'allegato      
        var nome_file = $('#Txt_Documento_Allegato').val();
        var file_check = $('#File_Caricato').val();

        if (file_check !== undefined && file_check !== null && file_check !== "") {

            var esito = checkVirus(nome_file, file_check, objP_super_server);

            if (esito !== "") {
                kendo.alert(esito);

                RimuoviAllegato();

                /*bDatiNecessariInseriti = false;*/
                return false
            }
        }
    } else {
        var esito = [];
        var allegatiDaEliminare = [];
        for (let i = 0; i < listaDocumenti.length; i++) {
            //Antivirus  in caso di virus verrà segnalato e rimosso l'allegato      
            var nome_file = listaDocumenti[i].nome;
            var file_check = listaDocumenti[i].file;

            if (file_check !== undefined && file_check !== null && file_check !== "") {

                let risultato = checkVirus(nome_file, file_check, objP_super_server);

                if (risultato !== "") {

                    esito.push(risultato);
                    allegatiDaEliminare.push(listaDocumenti[i].UID);
                    $("#" + listaDocumenti[i].UID).trigger("click");

                }
            }
        }

        if (allegatiDaEliminare.length > 0) {
            for (let i = 0; i < allegatiDaEliminare.length; i++) {
                $("#" + allegatiDaEliminare[i].UID).trigger("click");
            }
        }

        if (esito.length > 0) {
            kendo.alert(esito.join(", "));
            //bDatiNecessariInseriti = false;
            return false
        }
    }

    return true
}

function Azione_Indietro_Scadenzario() {
    if ($(paginaRedirect).val() !== undefined && $(paginaRedirect).val() !== "") {
        window.location.href = $(paginaRedirect).val();
        //window.location = "./Scad_Lista.aspx?type=" + modalita;  
    }
}
// #endregion

// #region MULTISELECT TIPOLOGIA      
//Anna 29/04/22: aggiunto multiselect tipologie al documento
function multiSelectTipologia_Load() {

    //creaKendoDropDownList("ddlTipologia", { read: RiempiDdlTipologia }, "nome", "Key1").bind("change", cmbDestinazione_change);

    if (!multiCmbTipologia) {
        multiCmbTipologia = $('#cmbTipologia').kendoMultiSelect({
            filter: "contains",
            dataSource: { transport: { read: RiempiMultiSelectTipologia } },
            dataTextField: "nome",
            dataValueField: "id_tipologia",
            autoWidth: true,
            autoClose: false
        }).data('kendoMultiSelect');
    } else {
        multiCmbTipologia.dataSource.read();
    }

    if (elementimultiCmbTipologia < 1) { // < 1 && elencoindici.length < 1) {
        $("#divMultiTipologia").hide();
    }
}

function RiempicmbTipologia(options) {
    options.success(Elenco_Tipologie);
}
// #endregion

//#region KENDO UPLOAD
function onSelect(evt) {
    //EVENTO SELECT

    controllaNome(evt);
    controllaEstensione(evt);
    controllaElementiDuplicati(evt);

    allegatoModificato();

    if (evt.files.length > 0) {
        var file = evt.files;//document.querySelector('input[type=file]').files;
        //dummy_UID = evt.files;

        for (let i = 0; i < file.length; i++) {
            if (this.getFiles().length > 1 && this.getFiles().some(file => file.name === evt.files[0].name)) {
                //quando viene rimosso l'ultimo initialFile (pre-esistente entrando in modifica),
                //il componente impazzisce ed il primo nuovo allegato aggiunto scatena l'evento select due volte
                //  this.getFiles() sono i file in coda
                //  this.getFiles().some(file => file.name === evt.files[0].name) verifica se il file che sta aggiungendo è già in coda
                //Se è già in coda annullo l'evento
                evt.preventDefault()
                return
            }

            if (file[i]) {
                //let nome;
                //let estensione;
                var reader = new FileReader();
                reader.onload = function (readerEvt) {

                    var binaryString = readerEvt.target.result;
                    var allegato = new Object();

                    allegato.nome = file[i].name;
                    allegato.file = btoa(binaryString);

                    let estensione = file[i].name.split('.');
                    if (estensione.length > 0) {
                        allegato.estensione = estensione[estensione.length - 1]
                    } else {
                        allegato.estensione = "";
                        //To do 19/05/22.... alert messaggio estensione non valida :)
                    }

                    allegato.UID = file[i].uid;
                    allegato.initial = false;
                    listaDocumenti.push(allegato);

                };

                reader.readAsBinaryString(file[i].rawFile);

            }
        }
    }

    $("#pnlDatiAllegato" + usaUploadMultiplo).show();

}

function controllaNome(evt) {
    //CONTROLLI SU CARICAMENTO FILE

    for (let i = 0; i < evt.files.length; i++) {
        let nomeSenzaPunti = "";
        let estensione = "";

        //Divido 
        let nome = evt.files[i].name.split('.');

        if (nome.length > 0) {
            estensione = nome[nome.length - 1]
        }

        nome.pop(); //rimuovo l'ultimo elemento. ovvero l'estensione
        nomeSenzaPunti = nome.join(" ") + "." + estensione; //ricompongo il nome senza punti ed aggiungo l'estensione 

        evt.files[i].name = nomeSenzaPunti
    }
}
function controllaEstensione(evt) {
    //Se estensione = "" è una cartella

    let listaNonValidi = [];
    for (let i = 0; i < evt.files.length; i++) {
        if (evt.files[i].extension === "") {
            listaNonValidi.push(evt.files[i].name);
            evt.files.splice(i, 1);
        }
    }

    if (listaNonValidi.length > 0) {
        kendo.alert((TraduzioneMultiResx(scadCreaModItemResx, "ImpossibileCaricareCartella", "Impossibile caricare un'intera cartella: ")) + listaNonValidi.join(", "))
    }
}
function controllaElementiDuplicati(evt) {


    let listaDuplicati = [];

    for (let i = 0; i < listaDocumenti.length; i++) {
        for (let j = 0; j < evt.files.length; j++) {
            if (listaDocumenti[i].nome === evt.files[j].name) {
                evt.files.splice(j, 1);
                listaDuplicati.push(listaDocumenti[i].nome);
            }
        }
    }

    if (listaDuplicati.length > 0) {
        kendo.alert((TraduzioneMultiResx(scadCreaModItemResx, "AllegatiConStessoNomeEstensione", "Sono già stati caricati allegati con lo stesso nome ed estensione: ")) + listaDuplicati.join(", "))
    }
}

function Apri_Doc_Allegato(nomeFile) {
    //CLICK ICONA DOCUMENTO

    let allegato = [];
    allegato = listaDocumenti.slice();

    //let allegatoFile = "";
    for (let i = 0; i < allegato.length; i++) {
        if (nomeFile === allegato[i].nome) {
            let UID = allegato[i].UID;
            let nome = allegato[i].nome
            let file = allegato[i].file;
            let estensione = allegato[i].estensione;

            if (UID.indexOf("-uidFileCaricato") > 0) {
                file = atob(file)
            }

            SaveAndOpenFileByteArray(nome, file, estensione);
            break;
        }
    }

    //    let allegato = null;
    //    //let allegatoFile = "";
    //    for (let i = 0; i < listaDocumenti.length; i++) {
    //        if (nomeFile === listaDocumenti[i].nome) {
    //            allegato = listaDocumenti[i];    
    //            break;
    //        }
    //    }

    //    let UID = "";
    //    UID = allegato.UID;
    //    if (UID.indexOf("-uidFileCaricato") > 0) {
    //        allegato.file = atob(allegato.file)
    //    }

    //    SaveAndOpenFileByteArray(allegato.nome, allegato.file, allegato.estensione);
}

function grandezzaFile(bytes) {
    //DIMENSIONE FILE PER DESCRIZIONE ALLEGATO

    let result = "";
    let KB = Math.round((bytes / 1024) * 100) / 100;
    let MB = Math.round((KB / 1024) * 100) / 100;

    if (MB >= 1) {
        result = MB + " MB";
    } else if (KB >= 1) {
        result = KB + " KB";
    } else {
        result = bytes + " bytes";
    }

    return result;
}

function onRemove(e) {
    //EVENTO REMOVE SINGOLO FILE

    let nomeFile = e.files[0].name;
    rimuoviElemento(nomeFile);

    allegatoModificato();

    // NON ATTIVA!
    // !! 20/05/22 SCOMMENTARE PER ABILITARE LA CONFERMA CANCELLAZIONE SINGOLO ELEMENTO !!
    //if (cancellaSingoloDocumento === false) {

    //    UID_elementoCancellazione = e.files[0].uid;

    //    e.preventDefault();
    //    kendo.confirm(TraduzioneMultiResx(scadCreaModItemResx, "ProcedereEliminazioneSingoloDocumento", "Procedo con l'eliminazione dell'allegato: ") + nomeFile + "?").then(function () {

    //        // execute logic
    //        cancellaSingoloDocumento = true;
    //        $("#" + UID_elementoCancellazione).trigger("click");

    //    }, function () {

    //        // do nothing
    //        UID_elementoCancellazione = "";
    //        cancellaSingoloDocumento = false;

    //    });

    //} else if (e.files[0].uid === UID_elementoCancellazione) {
    //    UID_elementoCancellazione = ""; //resetto variabile GLOBALE

    //    rimuoviElemento(nomeFile); //rimuovo elemento dalla lista
    //    cancellaSingoloDocumento = false;
    //} else {
    //    e.preventDefault();
    //    cancellaSingoloDocumento = false;
    //}
}

async function rimuoviElemento(nomeFile) {
    //RIMOZIONE FILE DA LISTA DOCUMENTI

    for (let i = 0; i < listaDocumenti.length; i++) {
        if (nomeFile == listaDocumenti[i].nome && listaDocumenti[i].initial == true) {
            rimuoviInitialFile = 0;
            break;
        }
    }

    if (initialFiles.length > 0 && rimuoviInitialFile == 0) {
        for (let i = 0; i < initialFiles.length; i++) {
            if (nomeFile === initialFiles[i].name) {
                initialFiles.splice(i, 1);
                break;
            }
        }

        listaDocumenti = $.grep(listaDocumenti, function (elem) {
            return elem.UID.indexOf("-uidFileCaricato") < 0;
        });


        await carica_InitialFiles();

    } else {
        for (let i = 0; i < listaDocumenti.length; i++) {
            if (nomeFile === listaDocumenti[i].nome) {
                listaDocumenti.splice(i, 1);
            }
        }

        if (listaDocumenti.length === 0) {
            $("#pnlDatiAllegato" + usaUploadMultiplo).hide();
        }
    }
}

function onClear(e) {
    //RIMOZIONE TUTTI FILE CARICATI

    allegatoModificato();

    if (cancellaTuttiDocumenti === false) {
        e.preventDefault();
        kendo.confirm(TraduzioneMultiResx(scadCreaModItemResx, "ProcedereEliminazioneTuttiDocumenti", "Procedo con l'eliminazione di tutti gli allegati caricati? ")).then(function () {
            // execute logic
            cancellaTuttiDocumenti = true;
            listaDocumenti = [];
            $(".k-clear-selected").click();
        }, function () {
            // do nothing
            cancellaTuttiDocumenti = false;
        });
    } else {
        cancellaTuttiDocumenti = false;
        $("#pnlDatiAllegato" + usaUploadMultiplo).hide();
    }
}

function compressaFile() {
    //COMPRESSIONE ALLEGATI SE count > 1

    if (listaDocumenti.length > 1) {
        CompressoDaGIAS = true;

        for (let i = 0; i < listaDocumenti.length; i++) {
            if (listaDocumenti[i].initial == true) {
                listaDocumenti[i].file = atob(listaDocumenti[i].file);
                delete listaDocumenti[i].initial;
            }
        }

        fileCompresso = btoa(pako.deflate(JSON.stringify(listaDocumenti)));

        riempiHiddenField_singolo_0_compresso_1(true, fileCompresso);

    } else if (listaDocumenti.length == 1) {
        CompressoDaGIAS = false;
        riempiHiddenField_singolo_0_compresso_1(false, "");
    }
}

function riempiHiddenField_singolo_0_compresso_1(multiAllegato, fileCompresso) {
    //RIEMPIMENTO HIDDEN FIELD PER SALVATAGGIO CONTNUTO E NOME ALLEGATO

    if (multiAllegato == false) {
        $('#Txt_Documento_Allegato' + usaUploadMultiplo).val(listaDocumenti[0].nome);

        let file = listaDocumenti[0].file;
        if (listaDocumenti[0].initial == true) {
            file = atob(listaDocumenti[0].file);
        }
        $('#File_Caricato' + usaUploadMultiplo).val(file);
    } else {
        $('#Txt_Documento_Allegato' + usaUploadMultiplo).val(listaDocumenti[0].nome.split('.')[0] + ".zip");
        $('#File_Caricato' + usaUploadMultiplo).val(fileCompresso);
    }
}

function decompressaFile(fileCompresso) {
    //DECOMPRESSIONE SE CompressoDaGIAS = TRUE

    let binaryArray = _base64ToArrayBuffer(fileCompresso);
    let fileDecompresso = pako.inflate(binaryArray, { to: 'string' })
    imposta_ListaInitialFiles_da_FileDecompresso(JSON.parse(fileDecompresso));
}
function _base64ToArrayBuffer(base64) {
    //CONVERSIONE FILE COMPRESSO IN ARRAY BINARIO PER LA DECOMPRESSIONE

    var binary_string = window.atob(base64).split(",");
    var len = binary_string.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = parseInt(binary_string[i]);
    }
    return bytes.buffer;
}
function imposta_ListaInitialFiles_da_FileDecompresso(fileDecompresso) {
    //AGGIUNTA DEI FILE DECOMPRESSI ALLA LISTA PER GLI INITIALFILES

    for (i = 0; i < fileDecompresso.length; i++) {
        var allegato = new Object();
        allegato.name = fileDecompresso[i].nome;
        allegato.extension = fileDecompresso[i].estensione;
        allegato.size = fileDecompresso[i].file.length;
        allegato.rawFile = new File([fileDecompresso[i].file], fileDecompresso[i].nome);

        allegato.uid = generaFileUID();

        initialFiles.push(allegato);
    }
}

function imposta_InitialFile_singolo(elem) {
    //AGGIUNTA FILE SINGOLO A INITIALFILES

    let fileUID = generaFileUID();

    initialFiles = [
        {
            name: elem["File_Name"],
            extension: "." + elem["Allegati_Documenti_Estensione"],
            size: (elem["File_Allegato_DB"]).length,
            rawFile: new File([(elem["File_Allegato_DB"])], elem["File_Name"]),
            uid: fileUID
        }
    ];
}

function generaFileUID() {
    //GENERO L'UID DEL FILE PER POTER USARE IL REMOVE SU SINGOLO ELEMENTO

    if (initialFiles_UID === undefined || initialFiles_UID === null) {
        initialFiles_UID = 1;
    }

    let fileUID = initialFiles_UID + "-uidFileCaricato"
    initialFiles_UID++;

    return fileUID;
}

async function carica_InitialFiles() {
    //CARICAMENTO INITIALFILES NEL KENDOUPLOAD

    //if (initialFiles.length == 0 && rimuoviInitialFile == 0 && listaDocumenti.length == 0) {
    if (rimuoviInitialFile == 0 && listaDocumenti.length == 0) {
        rimuoviInitialFile = 1;
        //Quando un initial file viene rimosso ed era anche unico, viene distrutto anche l'input file...
        //quindi distruggo tutto il div e lo ricreo, come se fossimo appena entrati nella pagina

        //Quando un initial file di N viene rimosso devo rigenerare tutto il componente

        let copyDivKendo = divKendoUpload_InitialFiles.clone().slice();
        $("#paperino").remove();
        $(copyDivKendo).insertBefore($("#pnlDatiAllegato_kendoUpload"));
    }

    let upload = inizializza_KendoUpload();
    let sourceInput;

    if (UploadMultiploAllegatiAbilitato == true) {
        for (let i = 0; i < initialFiles.length; i++) {
            var name = $.map(initialFiles[i], function (item) {
                return item.name;
            }).join(", ");

            sourceInput = upload._module.element.find("input[type='file']").last();
            let dummy = [];
            dummy[0] = initialFiles[i];
            var file = upload._enqueueFile(name, {
                relatedInput: sourceInput,
                fileNames: dummy
            });
            upload._fileAction(file, "remove");
        }
    }

    await aggiungi_InitialFile_A_ListaDocumenti();
}

function aggiungi_InitialFile_A_ListaDocumenti() {
    //AGGIUNTA INITIALFILES ALLA LISTADOCUMENTI

    return new Promise((resolve, reject) => {
        let arrProm = [];
        for (let i = 0; i < initialFiles.length; i++) {
            if (initialFiles[i]) {
                arrProm.push(leggiFile(initialFiles[i]));
            }
        }

        Promise.all(arrProm).then((values) => {
            if (listaDocumenti.length > 0) {
                $(".k-action-buttons").show();
            }
            resolve();
        });

    })

    //for (let i = 0; i < initialFiles.length; i++) {
    //    if (initialFiles[i]) {
    //        //let nome;
    //        //let estensione;
    //        var reader = new FileReader();
    //        reader.onload = function (readerEvt) {
    //            var binaryString = readerEvt.target.result;
    //            var allegato = new Object();

    //            allegato.nome = initialFiles[i].name;
    //            allegato.file = btoa(binaryString);

    //            let estensione = initialFiles[i].name.split('.');
    //            if (estensione.length > 0) {
    //                allegato.estensione = estensione[estensione.length - 1]
    //            } else {
    //                allegato.estensione = "";
    //                //To do 19/05/22.... alert messaggio estensione non valida :)
    //            }

    //            allegato.UID = initialFiles[i].uid;
    //            listaDocumenti.push(allegato);

    //            if (listaDocumenti.length > 0) {
    //                $(".k-action-buttons").show();
    //            }
    //        };

    //        reader.readAsBinaryString(initialFiles[i].rawFile);
    //    }
    //}

    //if (listaDocumenti.length > 0) {
    //    $(".k-action-buttons").show();
    //}
}

function leggiFile(file) {
    return new Promise((resolve, reject) => {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            var allegato = new Object();

            allegato.nome = file.name;
            allegato.file = btoa(binaryString);

            let estensione = file.name.split('.');
            if (estensione.length > 0) {
                allegato.estensione = estensione[estensione.length - 1]
            } else {
                allegato.estensione = "";
                //To do 19/05/22.... alert messaggio estensione non valida :)
            }

            allegato.UID = file.uid;
            allegato.initial = true; //Mi serve per capire se il file esisteva già nel documento oppure no, per poterlo tradurre al tipo corretto al salvataggio

            listaDocumenti.push(allegato);
            resolve();
        };

        reader.readAsBinaryString(file.rawFile);
    });
}

function resetKendoUpload() {
    //quando clicco su Salva e Nuovo

    $('#Txt_Num_Documento' + usaUploadMultiplo).val("");
    $('#Txt_Documento_Allegato' + usaUploadMultiplo).val("");
    $('#File_Caricato' + usaUploadMultiplo).val("");
    $("#pnlDatiAllegato" + usaUploadMultiplo).hide();

    CompressoDaGIAS = false;

    initialFiles = [];
    initialFiles_UID = null;


    for (let i = 0; i < listaDocumenti.length; i++) {
        $("#" + listaDocumenti[i].UID).trigger("click");
    }

    reset_kendoUpload()
}

function reset_kendoUpload() {
    let copyDivKendo = divKendoUpload_InitialFiles.clone().slice();
    $("#paperino").remove();
    $(copyDivKendo).insertBefore($("#pnlDatiAllegato_kendoUpload"));
    inizializza_KendoUpload();
}

function allegatoModificato() {
    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {
        bAllegato_Modificato = true;
    }
}

async function zippaFiles() {

    let allegati = listaDocumenti.slice();

    if (UploadMultiploAllegatiAbilitato == false) {
        for (let i = 0; i < allegati.length; i++) {
            allegati[i].file = atob(allegati[i].file);
        }

    }

    var zip = new JSZip();
    deferreds = [];
    allegati.forEach(function (allegati) {
        if (UploadMultiploAllegatiAbilitato == false) {
            deferreds.push(zip.file(allegati.nome, (allegati.file), { base64: true }));
        } else {
            deferreds.push(zip.file(allegati.nome, atob(allegati.file), { base64: true }));
        }
    });

    //jszip 2.6.1
    var content = zip.generate({ type: "blob" });
    saveAs(content, allegati[0].nome.split(".")[0]);

    //jszip 3.10 commentare sopra e scommentare questo
    //$.when.apply($, deferreds).then(function () {
    //    zip.generateAsync({ type: "blob" }).then(function (content) {
    //        saveAs(content, listaDocumenti[0].nome.split(".")[0]); //FileSaver.js
    //    });
    //});
}

function creaScaricaZip(Allegati_Documenti_Cod) {
    getFileCompressoDaGIAS(Allegati_Documenti_Cod);
}
// #endregion

//#region ATTIVITA RICETTE/ODL/BROGLIACCIO VISITE
function popolagriglia_Attivita_RicetteBrogliaccio_Visite(IDControllo) {

    var funzioniCRUD = {
        //funzioneRead: kReadValorizzazione_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null
    };

    let index = selezionaIndexGrigliaDaMostrare();
    var idModel = "";

    if (index == 0 || index == 2) {
        idModel = "Id_Agenda";
    } else if (index == 1) {
        idModel = "Ricetta_Operazione_Cod";
    } else {
        if (IDControllo == "griglia_Riferimenti") {
            idModel = "Id_Agenda";
            index = 0;
        } else if (IDControllo == "griglia_Visite") {
            idModel = "Id_Agenda";
            index = 2;
        } else {
            idModel = "Ricetta_Operazione_Cod";
            index = 1;
        }
    }

    var campiKendoModel, colonneKendoGrid

    if (index == 0) {
        funzioniCRUD.funzioneRead = Attivita_kReadValorizzazione_rows;
        funzioniCRUD.checkBoxFunction = Attvita_kEventoSelezionaRiga;

        campiKendoModel = Attivita_kReadValorizzazione_mod();
        colonneKendoGrid = Attivita_kReadValorizzazione_col();
    } else if (index == 2) {
        funzioniCRUD.funzioneRead = Visite_kReadValorizzazione_rows;
        funzioniCRUD.checkBoxFunction = Visite_kEventoSelezionaRiga;

        campiKendoModel = Visite_kReadValorizzazione_mod();
        colonneKendoGrid = Visite_kReadValorizzazione_col();
    } else {
        funzioniCRUD.funzioneRead = RicetteBrogliaccio_kReadValorizzazione_rows;
        funzioniCRUD.checkBoxFunction = RicetteBrogliaccio_kEventoSelezionaRiga;

        campiKendoModel = RicetteBrogliaccio_kReadValorizzazione_mod();
        colonneKendoGrid = RicetteBrogliaccio_kReadValorizzazione_col();
    }

    //var campiKendoModel = kReadValorizzazione_mod();
    //var colonneKendoGrid = kReadValorizzazione_col();

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
    };


    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe };


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

    $("#griglia_Attivita-header-chb").hide();
    $("#griglia_RicetteODLBrogliaccio-header-chb").hide();
    $("#griglia_Visite-header-chb").hide();

    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {
        //se siamo in modifica rendo la griglia grigetta
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("background", "rgb(0 0 0 / 3%)")
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("color", "black")
    }
}
// #endregion


//#region ATTIVITA 
function Attivita_kReadValorizzazione_rows(options) {
    var data = $('#hdGrigliaAttivita_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);
    var risultato = jSonParsed_Kendo.kendo_rows;


    if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") || ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0))) {
        risultato[0].Selected = true
        nascondiPannelli(0)
    }

    options.success(risultato);

}

function Attivita_kReadValorizzazione_col() {

    var data = $('#hdGrigliaAttivita_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function Attivita_kReadValorizzazione_mod() {

    var data = $('#hdGrigliaAttivita_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function Attvita_kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid,
        dataItem

    grid = $("#griglia_Attivita").data("kendoGrid")

    dataItem = grid.dataItem(row)
    dataItem.Selected = checked;

    if (checked === true) {
        limitaAdUnSoloElementoGriglia(grid, e, dataItem);
    }

    //Modifica Descrizione
    $('#txbDescrizione').val(dataItem.des_lib);

    rowKendoGridSelected(row, checked)
}
// #endregion

//#region VISITE
function Visite_kReadValorizzazione_rows(options) {
    var data = $('#hdGrigliaVisite_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);
    var risultato = jSonParsed_Kendo.kendo_rows;


    if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") || ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0))) {
        risultato[0].Selected = true
        nascondiPannelli(2)
    }

    options.success(risultato);

}

function Visite_kReadValorizzazione_col() {

    var data = $('#hdGrigliaVisite_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function Visite_kReadValorizzazione_mod() {

    var data = $('#hdGrigliaVisite_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function Visite_kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid,
        dataItem

    grid = $("#griglia_Visite").data("kendoGrid")

    dataItem = grid.dataItem(row)
    dataItem.Selected = checked;

    if (checked === true) {
        limitaAdUnSoloElementoGriglia(grid, e, dataItem);
    }

    rowKendoGridSelected(row, checked)
}
// #endregion

//#region RICETTE / ODL / BROGLIACCIO
function RicetteBrogliaccio_kReadValorizzazione_rows(options) {
    var data = $('#hdGrigliaRicetteODLBrogliaccio_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);
    var risultato = jSonParsed_Kendo.kendo_rows;


    if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") || ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0))) {
        risultato[0].Selected = true
        nascondiPannelli(1)
    }

    options.success(risultato);

}

function RicetteBrogliaccio_kReadValorizzazione_col() {

    var data = $('#hdGrigliaRicetteODLBrogliaccio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function RicetteBrogliaccio_kReadValorizzazione_mod() {

    var data = $('#hdGrigliaRicetteODLBrogliaccio_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function RicetteBrogliaccio_kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid,
        dataItem

    grid = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid")

    dataItem = grid.dataItem(row)
    dataItem.Selected = checked;

    if (checked === true) {
        limitaAdUnSoloElementoGriglia(grid, e, dataItem);
    }

    var dataDocumento = ""
    var nrDocumento = ""
    var contattoDocumento = ""

    rowKendoGridSelected(row, checked)
}
// #endregion

//#region PARTICELLE CATASTALI
function popolagriglia_ParticelleCatastali(IDControllo) {

    var funzioniCRUD = {
        //funzioneRead: kReadValorizzazione_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null
    };

    var idModel = "chiave";
    var campiKendoModel, colonneKendoGrid

    funzioniCRUD.funzioneRead = ParticelleCatastali_kReadValorizzazione_rows;
    funzioniCRUD.checkBoxFunction = ParticelleCatastali_kEventoSelezionaRiga;

    campiKendoModel = ParticelleCatastali_kReadValorizzazione_mod();
    colonneKendoGrid = ParticelleCatastali_kReadValorizzazione_col();

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe };

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

    $("#griglia_ParticelleCatastali-header-chb").hide();

    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {
        //se siamo in modifica rendo la griglia grigetta
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("background", "rgb(0 0 0 / 3%)")
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("color", "black")
    }
}

function ParticelleCatastali_kReadValorizzazione_col() {

    var data = $('#hdGrigliaParticelleCatastali_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function ParticelleCatastali_kReadValorizzazione_mod() {

    var data = $('#hdGrigliaParticelleCatastali_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function ParticelleCatastali_kReadValorizzazione_rows(options) {
    var data = $('#hdGrigliaParticelleCatastali_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);
    var risultato = jSonParsed_Kendo.kendo_rows;


    if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") || ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0))) {
        risultato[0].Selected = true
        nascondiPannelli(0)
    }

    options.success(risultato);

}

function ParticelleCatastali_kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid,
        dataItem

    grid = $("#griglia_ParticelleCatastali").data("kendoGrid")

    dataItem = grid.dataItem(row)
    dataItem.Selected = checked;

    if (checked === true) {
        limitaAdUnSoloElementoParticelleCatastali(grid, e, dataItem);
    }

    var dataScadenza = $("#Txt_Data_Scadenza").data("kendoDatePicker");

    rowKendoGridSelected(row, checked)
    if (checked) {
        let dataFineLimite = new Date(2099, 11, 31);
        let validitaFine = dataItem.Validita_Fine;

        if (validitaFine > dataFineLimite) {
            dataScadenza.value(dataFineLimite);
        } else {
            dataScadenza.value(validitaFine);
        }

        dataScadenza.trigger("change");

        //dataDocumento = dataItem.Data_Movimento
        //nrDocumento = dataItem.Doc_Numero_Visualizzato
        //contattoDocumento = dataItem.Cod_Contatto

    } else {
        dataScadenza.value(null);
        dataScadenza.trigger("change");
    }

    //aggiornaIndiciRiferimenti(dataDocumento, nrDocumento, contattoDocumento)
}

function limitaAdUnSoloElementoParticelleCatastali(grid, e, dataItem) {
    let allRows = grid.tbody.children();
    for (let j = 0; j < allRows.length; j++) {
        let rowLoop = $(allRows[j]);
        let dataItemLoop = grid.dataItem(rowLoop);
        if (dataItemLoop.chiave !== dataItem.chiave && dataItemLoop.Selected == true) {
            rowLoop.find("input[type='checkbox'].checkbox-selectionRow").prop("checked", false);
            rowLoop.toggleClass(GIAS_K_STATE_SELECTED);
        }
    }

    let kDataSource = grid.dataSource
    for (let i = 0; i < kDataSource.data().length; i++) {
        if (kDataSource.at(i).chiave !== dataItem.chiave && kDataSource.at(i).Selected == true) {
            kDataSource.at(i).Selected = false;
        }
    }

}

function button_cercaParticelleCatastali() {
    grigliaParticelleCatastali_caricata = false;

    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();
    var piva = $(cPiva).val();
    if (piva == "") {
        piva = $('#ddlAzienda').val();
    }

    ricercaParticelleCatastali(piva);
}
// #endregion

//#region RIFERIMENTI (DOC CONTABILI)
function popolagriglia_Riferimenti(IDControllo) {

    var funzioniCRUD = {
        //funzioneRead: kReadValorizzazione_rows,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null
    };

    var idModel = "Id_Agenda";
    var campiKendoModel, colonneKendoGrid

    funzioniCRUD.funzioneRead = Riferimenti_kReadValorizzazione_rows;
    funzioniCRUD.checkBoxFunction = Riferimenti_kEventoSelezionaRiga;

    campiKendoModel = Riferimenti_kReadValorizzazione_mod();
    colonneKendoGrid = Riferimenti_kReadValorizzazione_col();

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        excel: false,
        groupable: false,
        reorderable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe };

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

    $("#griglia_Riferimenti-header-chb").hide();

    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {
        //se siamo in modifica rendo la griglia grigetta
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("background", "rgb(0 0 0 / 3%)")
        $(".k-master-row." + GIAS_K_STATE_SELECTED).css("color", "black")
    }
}

function Riferimenti_kReadValorizzazione_col() {

    var data = $('#hdGrigliaRiferimenti_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function Riferimenti_kReadValorizzazione_mod() {

    var data = $('#hdGrigliaRiferimenti_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function Riferimenti_kReadValorizzazione_rows(options) {
    var data = $('#hdGrigliaRiferimenti_Valorizzazione').val();

    jSonParsed_Kendo = JSON.parse(data);
    var risultato = jSonParsed_Kendo.kendo_rows;

    var dataScadenza = $("#Txt_Data_Scadenza").data("kendoDatePicker");

    if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") || ((hfId_Elenco_val == "" || hfId_Elenco_val == "-1") && ($(cRicetta_Operazione_Cod).val() != 0 || $(cIdAgenda).val() != 0))) {
        risultato[0].Selected = true
        nascondiPannelli(0)

        if (risultato[0].Lav_Cod == 2006) {
            //propone la data scadenza in base a quella dell'unico record selezionato (se è un contratto d'affitto)
            let dataFineLimite = new Date(2099, 11, 31)
            let dd = parseInt(risultato[0].Scadenza_Contratto.split(' ')[0].split('/')[0]);
            let mm = parseInt(risultato[0].Scadenza_Contratto.split(' ')[0].split('/')[1]);
            let yyyy = parseInt(risultato[0].Scadenza_Contratto.split(' ')[0].split('/')[2]);

            if (new Date(yyyy, mm, dd) > dataFineLimite) {
                dataScadenza.value(new Date(2099, 11, 31));
            } else {
                dataScadenza.value(new Date(yyyy, mm, dd));
            }

            dataScadenza.trigger("change");

        }
    }

    options.success(risultato);

}

function Riferimenti_kEventoSelezionaRiga(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid,
        dataItem

    grid = $("#griglia_Riferimenti").data("kendoGrid")

    dataItem = grid.dataItem(row)
    dataItem.Selected = checked;

    if (checked === true) {
        limitaAdUnSoloElementoGriglia(grid, e, dataItem);
    }

    var dataDocumento = ""
    var nrDocumento = ""
    var contattoDocumento = ""

    var dataScadenza = $("#Txt_Data_Scadenza").data("kendoDatePicker");

    rowKendoGridSelected(row, checked);

    if (dataItem.Lav_Cod === 2006) {

        if (checked) {
            //propone la data scadenza in base a quella del record selezionato
            let dataFineLimite = new Date(2099, 11, 31)
            let dd = dataItem.Scadenza_Contratto.getDate();
            let mm = dataItem.Scadenza_Contratto.getMonth();
            let yyyy = dataItem.Scadenza_Contratto.getFullYear();

            if (new Date(yyyy, mm, dd) > dataFineLimite) {
                dataScadenza.value(dataFineLimite);
            } else {
                dataScadenza.value(new Date(yyyy, mm, dd));
            }

            dataScadenza.trigger("change");

            //dataDocumento = dataItem.Data_Movimento
            //nrDocumento = dataItem.Doc_Numero_Visualizzato
            //contattoDocumento = dataItem.Cod_Contatto

        } else {
            dataScadenza.value(null);
            dataScadenza.trigger("change");
        }

    }

    //Modifica Descrizione
    $('#txbDescrizione').val(dataItem.des_lib);  

    //aggiornaIndiciRiferimenti(dataDocumento, nrDocumento, contattoDocumento)

}

function button_cercaRiferimenti() {
    grigliaRiferimenti_caricata = false;

    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();
    var piva = $(cPiva).val();
    if (piva == "") {
        piva = $('#ddlAzienda').val();
    }

    let prosegui = true;
    prosegui = controllaRangeDate()

    if (prosegui == true) {
        let daAggiornato = $("#txt_data_da").val();
        let aAggiornato = $("#txt_data_a").val();

        ricercaRiferimenti(piva, $(cIdAgenda).val(), id_tipologia, daAggiornato, aAggiornato);
    }
}
// #endregion

//#region CONTROLLI su GRIGLIE
function controllaRangeDate() {
    let da = $("#txt_data_da").val();
    let a = $("#txt_data_a").val();

    if ($("#txt_data_da").data("kendoDatePicker").value() == null) {
        kendo.alert("Campo 'Data da' non impostato correttamente. <br> Usare il formato gg/mm/aaaa.");
        return false
    }
    if ($("#txt_data_a").data("kendoDatePicker").value() == null) {
        kendo.alert("Campo 'Data a' non impostato correttamente. <br> Usare il formato gg/mm/aaaa.");
        return false
    }

    if ($("#txt_data_da").data("kendoDatePicker").value() > $("#txt_data_a").data("kendoDatePicker").value()) {
        kendo.alert("Il campo 'Data da' non può essere maggiore di 'Data a'");
        return false
    }

    let giorno = setGiorno(da);
    let mese = setMese(da);
    let anno = setAnno(da);
    let unMeseDopo = new Date(anno, mese - 1, giorno);
    unMeseDopo = aggiungiUnMese(unMeseDopo)

    //Per comparare le date con gli operatori < >, queste devono avere il formato MM/gg/YYYY
    let unMese_DopoFormattato = formattaData(unMeseDopo)
    let dataDa_formattata = formattaData(da)
    let dataA_formattata = formattaData(a)

    if (dataA_formattata > unMese_DopoFormattato) {
        kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "PeriodoMassimoSelezionabile31gg", "E' possibile selezionare un periodo con durata massima 31gg."))

        return false
    }
    return true
}

function aggiungiUnMese(data) {
    data.setDate(data.getDate() + 31)
    let unMeseDopo = data.toLocaleDateString('en-GB');
    return unMeseDopo
}
function formattaData(data) {
    let giorno = setGiorno(data);
    let mese = setMese(data);
    let anno = setAnno(data);

    return anno + '-' + mese + '-' + giorno
}
function setGiorno(Data) {
    return Data.slice(0, 2)
}
function setMese(Data) {
    return Data.slice(3, 5)
}
function setAnno(Data) {
    return Data.slice(6, 10)
}

function limitaAdUnSoloElementoGriglia(grid, e, dataItem) {
    let allRows = grid.tbody.children();
    for (let j = 0; j < allRows.length; j++) {
        let rowLoop = $(allRows[j]);
        let dataItemLoop = grid.dataItem(rowLoop);
        if (dataItemLoop.Id_Agenda !== dataItem.Id_Agenda && dataItemLoop.Selected == true) {
            rowLoop.find("input[type='checkbox'].checkbox-selectionRow").prop("checked", false);
            rowLoop.toggleClass(GIAS_K_STATE_SELECTED);
        }
    }

    let kDataSource = grid.dataSource
    for (let i = 0; i < kDataSource.data().length; i++) {
        if (kDataSource.at(i).Id_Agenda !== dataItem.Id_Agenda && kDataSource.at(i).Selected == true) {
            kDataSource.at(i).Selected = false;
        }
    }

    let idArea = $("#ddlArea").val()

    if (idArea == 11) {
        let index = selezionaIndexGrigliaDaMostrare();
        let grid;

        if (index == 0) {
            grid = $("#griglia_Visite").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);
            grid = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);

        } else if (index == 1) {
            grid = $("#griglia_Attivita").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);
            grid = $("#griglia_Visite").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);

        } else if (index == 2) {
            grid = $("#griglia_Attivita").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);
            grid = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid")
            uncheckAltreGriglie(grid, dataItem);

        }
    }
}

function uncheckAltreGriglie(grid, dataItem) {
    if (grid != undefined) {
        let allRows = grid.tbody.children();
        for (let j = 0; j < allRows.length; j++) {
            let rowLoop = $(allRows[j]);
            let dataItemLoop = grid.dataItem(rowLoop);
            if (dataItemLoop.Selected == true) {
                rowLoop.find("input[type='checkbox'].checkbox-selectionRow").prop("checked", false);
                rowLoop.toggleClass(GIAS_K_STATE_SELECTED);
            }
        }

        let kDataSource = grid.dataSource
        for (let i = 0; i < kDataSource.data().length; i++) {
            if (kDataSource.at(i).Selected == true) {
                kDataSource.at(i).Selected = false;
            }
        }
    }
}

function nascondiPannelli(index) {
    let idArea = $("#ddlArea").val()
    let idTipologia = $("#ddlTipologia").val()
    if (idArea == 11) { //  qdc
        if (index == 0) {
            $("#griglia_Attivita").data("kendoGrid").hideColumn(0);
        } else if (index == 1) {
            $("#griglia_RicetteODLBrogliaccio").data("kendoGrid").hideColumn(0);
        } else {
            $("#griglia_Visite").data("kendoGrid").hideColumn(0);
        }
    } else if (idArea == 12 && idTipologia == -27) {
        $("#griglia_ParticelleCatastali").data("kendoGrid").hideColumn(0);
    } else {
        if (index == 0) {
            $("#griglia_Riferimenti").data("kendoGrid").hideColumn(0);
        } else {
            $("#griglia_RicetteODLBrogliaccio").data("kendoGrid").hideColumn(0);
        }
    }

    $(".k-pager-wrap.k-grid-pager.k-widget.k-floatwrap").hide()
    $(".k-toolbar.k-grid-toolbar").hide()

    $("#filtroDate").hide()
    $("#cercaRiferimenti").hide()
    $("#cercaParticelleCatastali").hide()
    $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").hide()

}

function selezionaGriglia() {
    let index = selezionaIndexGrigliaDaMostrare()

    if (index == 0) {
        return $('#hdGrigliaRiferimenti_Valorizzazione').val();
    } else {
        return $('#hdGrigliaRicetteODLBrogliaccio_Valorizzazione').val();
    }
}

function selezionaIndexGrigliaDaMostrare() {
    let buttongroup = $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").data("kendoButtonGroup");

    if (buttongroup == undefined) {
        return -1
    }

    let index = buttongroup.current().index();

    return index
}

function selezionaRiga_Riferimenti() {
    let idArea = $('#ddlArea').val();
    let index = selezionaIndexGrigliaDaMostrare();
    let grid;

    if (idArea == 10) {
        grid = $("#griglia_Riferimenti").data("kendoGrid")

        let dataDocumento = grid.dataSource.data()[0].Data_Movimento
        let nrDocumento = grid.dataSource.data()[0].Doc_Numero_Visualizzato
        let contattoDocumento = grid.dataSource.data()[0].Cod_Contatto

        aggiornaIndiciRiferimenti(dataDocumento, nrDocumento, contattoDocumento)
    }

    //to do:a ggiornare se vengono aggiunti indici protetti
}

function caricaGriglie_Attivita_RicetteODLBrogliaccio_Visite(e) {

    if (controllaRangeDate() == true) {

        var index = this.current().index();

        let dataDa = $("#txt_data_da").val();
        let dataA = $("#txt_data_a").val();
        var idArea = $('#ddlArea').val();

        var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();
        var piva = $(cPiva).val();
        if (piva == "") {
            piva = $('#ddlAzienda').val();
        }

        //if (buttonGroupSelected_TipoEntitaSecondario !== index) {

        buttonGroupSelected_TipoEntitaSecondario = index;

        // OPERAZIONI
        if (index == 0) {
            $("#Attivita").show();

            $("#RicetteODLBrogliaccio").hide();
            $("#Visite").hide();

            if (grigliaAttivita_caricata == false) {
                ricercaAttivitaVisite(piva, idArea, $(cIdAgenda).val(), id_tipologia, dataDa, dataA);
            }
        }

        if (index == 2) {
            $("#Visite").show();

            $("#RicetteODLBrogliaccio").hide();
            $("#Attivita").hide();

            if (grigliaVisite_caricata == false) {
                ricercaAttivitaVisite(piva, idArea, $(cIdAgenda).val(), id_tipologia, dataDa, dataA);
            }

        }
        // RICETTE / BROGLIACCIO
        if (index == 1) {
            $("#RicetteODLBrogliaccio").show();

            $("#Visite").hide();
            $("#Attivita").hide();

            if (grigliaRicetteBrogliaccio_caricata == false) {
                ricercaRicetteODLBrogliaccio(idArea, $(cRicetta_Operazione_Cod).val(), dataDa, dataA); //cIdRicetta
            }
        }
        //}
    }
}

function onChange_Data() {
    resettaGriglie()
}

function resettaGriglie() {
    grigliaAttivita_caricata = false;
    grigliaRicetteBrogliaccio_caricata = false;
    grigliaRiferimenti_caricata = false;
    grigliaVisite_caricata = false;
    grigliaParticelleCatastali_caricata = false;

}

//NON USATE 
//function fixFormatoData(index) {
//    if (index == 0) {
//        operazioneSelezionataModifica[0].Data_Movimento = $("#txt_data_da").val();
//    } else {
//        ricettaBrogliaccioSelezionataModifica[0].Validita_Inizio = $("#txt_data_da").val();
//    }
//}
//function ricercaOperazioneSelezionata(piva, idArea, idAgenda, idTipologia, dataDa, dataA) {
//    var idArea = $('#ddlArea').val();
//    let index = selezionaIndexGrigliaDaMostrare();

//    var parametri = "";
//    var pathCaricaCmb = "";

//    if (idArea == 10) {

//        $("#TipoOutput_Riferimenti_RicetteODLBrogliaccio_Visite").hide();

//        if (piva !== undefined && piva !== null && piva !== "") {
//            parametri = kendo.stringify({
//                "Piva": piva,
//                "idAgenda": idAgenda,
//                "idTipologia": idTipologia,
//                "objP_server": objP_server,
//                "dataDa": dataDa,
//                "dataA": dataA,
//                "escludiIdAgenda": false
//            });
//            pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaDDT_toKendoGrid";
//        }
//    } else if (idArea == 11) {
//        $("#TipoOutput_Attivita_RicetteODLBrogliaccio_Visite").show();
//        //TO DO QDC TIPO AGENDA CON FILTRO QDC = TRUE
//        parametri = kendo.stringify({
//            "Piva": piva,
//            "idAgenda": idAgenda,
//            "Sa_Cod": 0,
//            "objP_server": objP_server,
//            "dataDa": dataDa,
//            "dataA": dataA,
//            "filtraQDC": true
//        });
//        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaGenerica_toKendoGrid";
//    } else {
//        parametri = kendo.stringify({
//            "Piva": piva,
//            "idAgenda": idAgenda,
//            "Sa_Cod": 0,
//            "objP_server": objP_server,
//            "dataDa": dataDa,
//            "dataA": dataA,
//            "filtraQDC": false
//        });
//        pathCaricaCmb = pathCoreWS + "Agenda/Agenda.asmx/LeggiAgendaGenerica_toKendoGrid";
//    }

//    ajaxAgronicaSync(pathCaricaCmb,
//        parametri,
//        false,
//        function (risposta) {
//            $('#hdGrigliaRiferimenti_Valorizzazione').val(risposta.RispostaStringa);
//            getElemento();
//        }, null);
//}
//function getElemento() {
//    var data = selezionaGriglia();

//    jSonParsed_Kendo = JSON.parse(data);
//    operazioneSelezionataModifica = jSonParsed_Kendo.kendo_rows;

//    $("#txt_data_da").data("kendoDatePicker").val(operazioneSelezionataModifica[0].Data_Movimento);
//    onChange_filtroDateGriglia("da");
//}
//function onChange_filtroDateGriglia(chiamante) {
//    grigliaOperazioni_caricata = false;
//    grigliaRicetteBrogliaccio_caricata = false;
//    //let rileggiGriglia = true;

//    var id_tipologia = $('#ddlTipologia').data("kendoDropDownList").value();
//    var piva = $(cPiva).val();
//    if (piva == "") {
//        piva = $('#ddlAzienda').val();
//    }

//    var giorno, mese, anno, dataCompleta, data;

//    var idArea = $('#ddlArea').val();

//    let da = $("#txt_data_da").data("kendoDatePicker").val();
//    let a = $("#txt_data_a").data("kendoDatePicker").val();

//    if (chiamante == "da") {
//        data = da
//    } else {
//        data = a
//    }

//    giorno = setGiorno(data);
//    mese = setMese(data);
//    anno = setAnno(data);
//    dataCompleta = new Date(anno, mese - 1, giorno);

//    if (chiamante == "da") {
//        aggiungiUnMese(dataCompleta)
//    } else {
//        togliUnMese(dataCompleta);
//    }

//    //Aggiorno le variabili
//    let daAggiornato = $("#txt_data_da").data("kendoDatePicker").val();
//    let aAggiornato = $("#txt_data_a").data("kendoDatePicker").val();



//    //if ((hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1")) {
//    //    if (daAggiornato != da) {
//    //        operazioneSelezionataModifica = undefined;
//    //        ricettaBrogliaccioSelezionataModifica = undefined;
//    //    } else {
//    //        rileggiGriglia = false;
//    //    }
//    //}

//    //if (rileggiGriglia == true) {
//    let index = selezionaIndexGrigliaDaMostrare();
//    if (index == 0) {
//        ricercaRiferimenti(piva, idArea, $(cIdAgenda).val(), id_tipologia, daAggiornato, aAggiornato);
//    } else {
//        ricercaRicetteODLBrogliaccio(idArea, $(cRicetta_Operazione_Cod).val(), daAggiornato, aAggiornato); //cIdRicetta
//    }
//    //}
//}
//function old_aggiungiUnMese(data) {
//    data.setDate(data.getDate() + 31)
//    let unMeseDopo = data.toLocaleDateString('en-GB');
//    $("#txt_data_a").data("kendoDatePicker").val(unMeseDopo);
//}
//function togliUnMese(data) {
//    data.setDate(data.getDate() - 30)
//    let unMesePrima = data.toLocaleDateString('en-GB');
//    $("#txt_data_da").data("kendoDatePicker").val(unMesePrima);
//}
//function old_limitaAdUnSoloElementoGriglia(e, grid, dataItem) {
//    //questa funzione dava un messaggio se un elemento della geiglia era stato già selezionato, teneva il valore precedente
//    let counter = 0;

//    $.each(grid.dataSource.view(), function () {
//        if (this['Selected'] == true) {
//            counter++
//        }
//    });

//    if (mostraEntrambeGriglie_Riferimenti_RicetteBrogliaccio == true) {
//        let index = selezionaIndexGrigliaDaMostrare();
//        if (index == 0) {
//            grid = $("#griglia_RicetteODLBrogliaccio").data("kendoGrid")
//        } else {
//            grid = $("#griglia_Riferimenti").data("kendoGrid")
//        }
//        if (grid != undefined) {
//            $.each(grid.dataSource.view(), function () {
//                if (this['Selected'] == true) {
//                    counter++
//                }
//            });
//        }
//    }

//    if (counter === 1) {
//        if (mostraEntrambeGriglie_Riferimenti_RicetteBrogliaccio == true) {
//            kendo.alert(TraduzioneMultiResx(scadCreaModItemResx, "Riferimento_RicettaBrogliaccioGiaSelezionata", "E' già stato selezionato un riferimento o una ricetta / brogliaccio."))
//        } else {
//            kendo.alert((scadCreaModItemResx, "PossibileSelezionareUnSoloElemento", "E' possibile selezionare un solo elemento."))
//        }


//        dataItem.Selected = false;

//        $(this).parents("tr").removeClass(GIAS_K_STATE_SELECTED);
//        e.preventDefault();
//        return false;
//    }
//    return true
//}
function onDataBoundRighe(e) {

    //Marco: Impostazione Des_Lib default
    if ($(cIdAgenda).val() !== "0") {
        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");
        $('#txbDescrizione').val(grid._data[0].des_lib);
    }

}
// #endregion

function ddlTipologiaValorizzata() {
    if ($('#ddlTipologia').data("kendoDropDownList").value() == undefined || $('#ddlTipologia').data("kendoDropDownList").value().toString() == "") {
        kendo.alert((scadCreaModItemResx, "UtenteNonHaPermessoUtilizzareTipologia", "L’utente non ha il permesso di utilizzare questa tipologia."))
    }
}

function ddlTrasportiValorizzaIndici() {

    var partsArray = $(cIndici).val().toString().split('|');
    for (let iindice = 0; iindice < partsArray.length; iindice++) {

        var partsIndice = partsArray[iindice].split('-');

        var indice_cod = partsIndice[0];
        var indice_valore = partsIndice[1];

        if (indice_cod !== undefined) {
            // Impostare i valori nei controlli creati sopra
            if (elencoindici !== null && elencoindici.length !== 0) {
                for (let iindice3 = 0; iindice3 < elencoindici.length; iindice3++) {
                    if (elencoindici[iindice3].ID_Indice !== 0) {
                        if (elencoindici[iindice3].TipoCampo == 0 ||
                            elencoindici[iindice3].TipoCampo == 1 ||
                            elencoindici[iindice3].TipoCampo == 2) {

                            if (parseInt(elencoindici[iindice3].ID_Indice) == indice_cod) {

                                var chiave2 = elencoindici[iindice3].TipoCampo + "_" + indice_cod;

                                switch (parseInt(elencoindici[iindice3].TipoCampo)) {

                                    case 0: //Valore Libero

                                        switch (elencoindici[iindice3].TipoDato) {

                                            case "numeric":

                                                Set_KendoNumTBValue("txt" + chiave2, indice_valore);
                                                break;

                                            case "boolean":

                                                let Valore_Boolean = indice_valore === "True";

                                                setKendoSwitch("chk" + chiave2, Valore_Boolean);
                                                break;

                                            default:

                                                $("#txt" + chiave2).val(indice_valore.toUpperCase());
                                                break;

                                        }

                                        break;

                                    case 1: //Valori Dettagli

                                        Set_KendoDDLValue("ddl" + chiave2, indice_valore);
                                        break;

                                    case 2: //Elenco

                                        Set_KendoDDLValue("ddl" + chiave2, indice_valore);
                                        break;
                                }

                            }
                        }
                    }
                }
            }
        }
    }

}