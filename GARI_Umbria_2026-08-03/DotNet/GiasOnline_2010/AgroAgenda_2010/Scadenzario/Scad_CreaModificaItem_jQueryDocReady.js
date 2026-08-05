
//DOCUMENT READY
async function wrapDocReady() {


    //---------------------------
    //      TRADUZIONI RESX
    //---------------------------
    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            scadCreaModItemResx.unshift(readResxFile(resxSinglePath, "Scad_CreaModificaItem_jQueryDocReady.js"));
        });
    }


    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True";
    //Se l'utente è abilitato alla scrittura aggiungo i pulsanti per l'inserimento di nuovi elementi
    if (UtenteAbilitatoScrittura == true) {
        $(".add").show();
    }



    //---------------------------
    //      COMPONENTI KENDO
    //---------------------------
    //Applico il calendario            
    $("#Txt_Data_Scadenza").kendoDatePicker();
    $("#Txt_Data_Allegato").kendoDatePicker();

    $('html,body').css('cursor', 'wait');

    if ($("input[name$='hf_UploadMultiploAllegatiAbilitato']").val() == "True") {
        usaUploadMultiplo = "_kendoUpload"
        UploadMultiploAllegatiAbilitato = true;
    }
    inizializzaComponenti();

    //Carico la combo delle aziende
    //ddlAzienda_Load();

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        document.getElementById('File_Allegato' + usaUploadMultiplo).addEventListener('change', handleFileSelect, false);
    } else {
        kendo.alert('Il tuo browser non supporta alcune API necessarie al caricamento degli allegati.'); // i18n Non tradotto in quanto messaggio tecnico
    }


    //Inizializzo il controllo della tipologia (senza dati)
    //$('#ddlTipologia"]').kendoDropDownList({ dataSource: [] });

    //Se l'utente è abilitato aggiungo il pulsante per il salvataggio delle Scadenze
    if ($("input[id*='hf_UtenteAbilitatoScrittura']").val() == "True") {
        $("#btnSalva").show();
        $("#btnNuovo").show();
        tipoPermessoDaControllare = 1;
    }
    else {
        $("#btnSalva").hide();
        $("#btnNuovo").hide();
        tipoPermessoDaControllare = 2;
    }


    //---------------------------
    //    CARICAMENTO PAGINA
    //---------------------------

    Pagina_Inizializzata = false;

    //Se è una scadenza già esistente, la carico
    //var hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    hfId_Elenco_val = $("input[id*='hfId_Elenco']").val();
    if (hfId_Elenco_val !== "" && hfId_Elenco_val !== "-1") {

        //Inizializzo tutti i controlli figli
        pulisciDdlCategorie();
        pulisciDdlAccessorie();

        await caricaScadDaIdElenco(hfId_Elenco_val);

        bloccaControlloAzienda();
        bloccoControlloDettagli(false);
        rimuoviElementiDaDDLArea();

        if (tipoPermessoDaControllare == 2) {
            bloccaTuttiControlli(); //sola lettura intero documento
        }

        //Anna 29/04/22: aggiunto multiselect tipologie al documento
        //Multiselect in modifica sempre nascosto
        $("#divMultiTipologia").hide();

        //Se workflow abilitato, non consento la modifica dell'area
        if ($(workFlow_Abilitato)[0].value === "True") {
            KendoDDL("ddlArea").enable(false);
        }

    } else { //NUOVO DOCUMENTO / SCADENZA

        AggiornaUsernameUpload();

        inizializza_KendoUpload([]);

        await ddlAzienda_Load($(cPiva).val());

        //Controllo Parametri Passati Di Default
        
        if ($(cPiva).val() !== "") {

            await Set_KendoDDLValueVirtual("ddlAzienda", $(cPiva).val());
            ddlAzienda_Change();

            //Blocco Controllo
            KendoDDL("ddlAzienda").enable(false);

        }

        //Anna 06/05/22 imposto data_creazione ad oggi
        data_creazione = new Date().toLocaleDateString()



        if ($(cArea_Provenienza).val() !== "0") {
            Set_KendoDDLValue("ddlArea", $(cArea_Provenienza).val());
            ddlArea_Change();
        }


        if ($(cArea_Provenienza).val() == "108" || $(cArea_Provenienza).val() == "109" || $(cArea_Provenienza).val() == "110" || $(cArea_Provenienza).val() == "101") {
            //Impostazione da edit audit con tipologia scelta
            if ($(cId_Tipologia).val() !== "0") {
                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologia_Change();
                Provienienza_Edit_Audit = true;
            }
        }

        // Pagina documento richiesta da UMA
        if ($(cRichiesta_Cod).val() !== "0") {

            Set_KendoDDLValue("ddlArea", 7);
            ddlArea_Change();

            Set_KendoDDLValue("ddlUma_Carburanti", $(cRichiesta_Cod).val());

            if ($(cId_Tipologia).val() !== "0") {

                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologia_Change();

            }
        }

        if ($(cAnalisi_Testata_Cod).val() !== "0") {

            Set_KendoDDLValue("ddlArea", 3);
            ddlArea_Change();

            if ($(cId_Tipologia).val() !== "0") {

                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologia_Change();
            }

            if ($(cData_Scadenza_Analisi).val() !== "" && $(cData_Scadenza_Analisi).val() !== "31/12/2100") {

                $('#Txt_Data_Scadenza').val($(cData_Scadenza_Analisi).val());

            }

            Set_KendoDDLValue("ddlAnalisi", $(cAnalisi_Testata_Cod).val());

        }


        if ($(cIdAgenda).val() !== "0" || $(cRicetta_Operazione_Cod).val() !== "0") {

            Set_KendoDDLValue("ddlArea", $(cArea_Provenienza).val());
            ddlArea_Change();

            if ($(cId_Tipologia).val() !== "0") {

                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologiaValorizzata();
                ddlTipologia_Change();
            }
        }

       
        

        //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
        if ($(cCod_Contatto).val() !== "") {

            Set_KendoDDLValue("ddlArea", $(cArea_Provenienza).val());
            ddlArea_Change();

            if ($(cId_Tipologia).val() !== "0") {

                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologiaValorizzata();
                ddlTipologia_Change();
            }
        }

    }

    if (tipoPermessoDaControllare != 2) {
        cmbValidazione_change();
    }


    // Pagina documento richiesta da UMA, ANALISI, QDC...
    if ($(cRichiesta_Cod).val() !== "0" || $(cAnalisi_Testata_Cod).val() !== "0" || $(cIdAgenda).val() !== "0" || $(cRicetta_Operazione_Cod).val() !== "0" || $(cCod_Contatto).val() !== "" || $(cArea_Provenienza).val() !== "0") {
        //L'azienda la blocco perchè viene passata in QS
        bloccaControlloAzienda()

        //Se l'ID_Tipologia è < 0, blocco il campo
        if (KendoDDL("ddlTipologia").value() < 0 && KendoDDL("ddlArea").value() !=="2") {
            bloccoControlloDettagli(true);
        }

        if ($(cArea_Provenienza).val() !== "0")
            KendoDDL("ddlArea").enable(false);

        bloccaControlli_AgendaAnalisiMacchina();
        //KendoDDL("ddlAzienda").enable(false);
        if ($(cId_Tipologia).val() !== "0" && KendoDDL("ddlArea").value() == "10") {
            if (KendoDDL("ddlTipologia").value() !== undefined && KendoDDL("ddlTipologia").value() !== "") {
                KendoDDL("ddlTipologia").enable(false);
            }
        }

        // Salvatore Zammataro 16-01-2024, modifiche per inserimento allegati da catasto Angular
        // Se area di provenienza CATASTO
        if ($(cArea_Provenienza).val() === "12") {
            if ($(cId_Tipologia).val() !== "0") {
                Set_KendoDDLValue("ddlTipologia", parseInt($(cId_Tipologia).val()));
                ddlTipologiaValorizzata();
                ddlTipologia_Change();
            }
        }
        //--------------------------- fine modifiche Salvatore Zammataro ---------------------------


        $("#btnNuovo").hide();
    }

    if ($(cAnalisi_Testata_Cod).val() !== "0" || $(cIdAgenda).val() !== "0" || $(cRichiesta_Cod).val() !== "0" || $(cRicetta_Operazione_Cod).val() !== "0" || $(cCod_Contatto).val() !== "") {

        if ($(cIdAgenda).val() !== "0") {
            //    if ($("[id$='-1000']") != null && $("[id$='-1000']") != undefined) {
            //        if ($('input[id$="-1000"]').data("kendoDatePicker") != null && $('input[id$="-1000"]').data("kendoDatePicker") != undefined) {
            //            $('input[id$="-1000"]').data("kendoDatePicker").enable(false);
            //        }
            //    }
            //    if ($("[id$='-1001']") != null && $("[id$='-1001']") != undefined) {
            //        $("input[name$='-1001']").prop("disabled", true);
            //    }
            //    if ($("[id$='-1002']") != null && $("[id$='-1002']") != undefined && $("[name$='-1002']").data("kendoDropDownList") != null && $("[name$='-1002']").data("kendoDropDownList") != undefined) {
            //        $("[name$='-1002']").data("kendoDropDownList").enable(false);
            //    }
        }
        if ($(cRichiesta_Cod).val() !== "0") {
            if ($('#ddlUma_Carburanti').data("kendoDropDownList").value() !== undefined && $('#ddlUma_Carburanti').data("kendoDropDownList").value() !== "") {
                KendoDDL("ddlUma_Carburanti").enable(false);
            }
        } else if ($(cAnalisi_Testata_Cod).val() !== "0") {
            if ($('#ddlAnalisi').data("kendoDropDownList").value() !== undefined && $('#ddlAnalisi').data("kendoDropDownList").value() !== "") {
                KendoDDL("ddlAnalisi").enable(false);
            }
        }
        //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
        else if ($(cCod_Contatto).val() !== "") {
            Set_KendoDDLValue("ddlContatto", $(cCod_Contatto).val());
        }

        bloccaControlli_AgendaAnalisiMacchina();
    }


    // In caso di documenti l'allegato può essere modificato ma non rimosso poichè necessario
    if ($(cModalita).val() == "doc") {
        $("#btnRimuovi").hide();
    }

    //Anna 29/04/22: aggiunto multiselect tipologie al documento
    // di defualt il div è semore nascosto, lo mostro solo:
    // se siamo in inserimento > se l'utente  ha i permessi di scrittura
    $("#divMultiTipologia").hide();


    //eventi di click pulsanti
    $("#btn_Rimuovi").click(function () { RimuoviAllegato(); });

    $('html,body').css('cursor', 'default');

    if ($(workFlow_Abilitato)[0].value === "True") {
        $("#id_validazione_kendoUpload").hide()
        $("#stato_des").show()
    }
    else {
        $("#id_validazione_kendoUpload").show()
        $("#stato_des").hide()
    }


    Pagina_Inizializzata = true;
}

$(document).ready(async function () {


    $.logThis("DocReady: INIZIO");
    WaitFrame.show();
    await wrapDocReady();
    WaitFrame.hide();
    $.logThis("DocReady: FINE");

});


//----------------------------------
//     COMPONENTI PAGINA 
//----------------------------------
function inizializzaComponenti() {

    $("#pnlAllegato" + usaUploadMultiplo).show();

    $("#Txt_Data_Upload" + usaUploadMultiplo).kendoDatePicker();
    //AggiornaUsernameUpload();

    creaKendoDropDownList("cmbValidazione" + usaUploadMultiplo, { read: RiempicmbValidazione }, "Validazione_Des", "Validazione_Cod").bind("change", cmbValidazione_change);

    creaKendoSwitch("ChkStorico" + usaUploadMultiplo, TraduzioneMultiResx(scadCreaModItemResx, "Si", "Sì"), TraduzioneMultiResx(scadCreaModItemResx, "No", "No"), false, function (e) { });

    //Visibilità Allegato --> nascondo entrambi i pannelli        
    if ($(cAllegato_Permesso).val() === "False") {
        $("#pnlAllegato_kendoUpload").hide();
        $("#pnlAllegato").hide();
    }

    //Visualizzazione Validazione
    if ($(cAllegato_Validazione_Visibilita).val() == "False") {
        //$("#lblValidazione").hide();
        //KendoDDL("cmbValidazione").wrapper.hide();   
        $("#id_validazione" + usaUploadMultiplo).hide();
    }

    //Modifica Validazione
    if ($(cAllegato_Validazione).val() == "False") {
        KendoDDL("cmbValidazione" + usaUploadMultiplo).enable(false);
    }

    //Casadei: Controlla se si arriva dalla checklist EUDR per bloccare la validazione e rendere obbligatoria la data del documento
    let indiceEUDR = $(cIndici).val().toString().split(',')
    if (indiceEUDR[indiceEUDR.length - 1] == "21") {
        KendoDDL("cmbValidazione" + usaUploadMultiplo).enable(false);
        checkDataDocumentoObbligatoria = true;
    }

    //Modifica Storicizzazione
    if ($(cAllegato_Permesso_Storicizzazione).val() == "False") {
        $("#ChkStorico" + usaUploadMultiplo).data("kendoSwitch").enable(false);
    }


    $("#pnlDatiAllegato_kendoUpload").hide();
    $("#pnlDatiAllegato").hide();
    $("#pnlApriAllegato").hide();

    $("#txt_data_a").kendoDatePicker();
    $("#txt_data_da").kendoDatePicker();

}

function inizializza_KendoUpload(files) {
    let $upload = $("#files");

    var upload = $upload.kendoUpload({

        //Per poter caricare > 1 allegato

        multiple: true,
        //Per poter dropppare intere cartelle (prende il contento) --> 20/05/22 HA UN BUG! se si fa il drag and drop di una cartella, successivamente non è possibile fare il drag and drop di un singolo file
        //directoryDrop: true,

        //Proprietà obbligatorie di un allegato caricato
        validation: {
            maxFileSize: 20000000
            //minFileSize: 
            //allowedExtensions: []
        },

        //Funzioni personalizzate per eventi
        select: onSelect,
        clear: onClear,
        remove: onRemove,

        //Template personalizzato kendo upload
        template: kendo.template($('#fileTemplate').html()),

        //Proprietà obbligatoria per poter usare il trascinamento e pulsante Rimuovi Tutto
        async: {
            saveUrl: "save",
            removeUrl: "remove",
            autoUpload: false // = true --> ogni file viene caricato nel server / con la ajaxcall dichiarati nelle proprietà sopra
        },

        //Area attiva trascinamento
        dropZone: ".dropZoneElement",

        //Labels/Pulsanti personalizzate
        localization: {
            select: TraduzioneMultiResx(scadCreaModItemResx, "ScegliFile", "Scegli file"),
            dropFilesHere: TraduzioneMultiResx(scadCreaModItemResx, "TrascinaFilePerCaricarli", "Trascina qui i file per caricarli"),
            clearSelectedFiles: TraduzioneMultiResx(scadCreaModItemResx, "RimuoviTutto", "Rimuovi tutto")
        },

        files: files
    }).getKendoUpload();

    return upload;
}