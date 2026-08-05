var btn_salva_clicked = false;
var righeInseriteGrid_Costo = null;
var righeModificateGrid_Costo = null;
var righeEliminateGrid_Costo = null;
var righeTutteGrid_Costo = null;
var ElencoUnitadimisura = [{ "Mezzo": 1, "Udm_Des": "Ettaro" }, { "Mezzo": 2, "Udm_Des": "Ora" }];

function controlla_form() {

    var flag = true;
    var n_inv = 0;

    v.resetForm();

    // Azzero tutte le label custom_val
    $(".custom_val").each(function (i, obj) {
        $(this).css('border', '1px solid #ccc');
        $(this).parent().children().css('border-color', '#ccc');
        $(this).remove();
    });

    //if ($('#<%=Txt_DataInizioUtilizzo.ClientID %>').val() == "") {
    //    $('#<%=Txt_DataInizioUtilizzo.ClientID %>').parent().append('<label id="<%=Txt_DataInizioUtilizzo.ClientID %>-error" class="custom_val error" for="<%=Txt_DataInizioUtilizzo.ClientID %>">Il campo deve essere compilato</label>');
    //    $('#<%=Txt_DataInizioUtilizzo.ClientID %>').parent().children(".required").css('border', '1px solid #D41E1A');
    //    flag = false;
    //    n_inv++;
    //}

    if ($('#Txt_DataDismissione').val() != "") {

        // Controllo se le date sono corrette
        var TxtValiditaInizio = $('#Txt_DataInizioUtilizzo').val().split("/");
        var TxtValiditaFine = $('#Txt_DataDismissione').val().split("/");

        ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
        fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

        if (ini > fin) {
            $('#Txt_DataInizioUtilizzo').parent().append('<label id="Txt_DataInizioUtilizzo-error" class="custom_val error" for="Txt_DataInizioUtilizzo">'
                + TraduzioneMultiResx(macchinaEditResx, 'DataInizioMaggioreDiDataDismissione', 'La data di inizio non può essere maggiore di quella di dismissione')
                + '</label>');
            $('#Txt_DataInizioUtilizzo').closest("input").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }

    }

    ///////////////////////////

    if ($('#Cmb_CentriAziendali').val() == "") {
        $('#Cmb_CentriAziendali').parent().append('<label id="Cmb_CentriAziendali-error" class="custom_val error" for="Cmb_CentriAziendali">'
            + TraduzioneMultiResx(macchinaEditResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato')
            + '</label>');
        $('#Cmb_CentriAziendali').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    var finalita = $('#Cmb_Finalita').val();
    if (finalita == "") {
        $('#Cmb_Finalita').parent().append('<label id="Cmb_Finalita-error" class="custom_val error" for="Cmb_Finalita">'
            + TraduzioneMultiResx(macchinaEditResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato')
            + '</label>');
        $('#Cmb_Finalita').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    if (finalita == "0") {
        if ($('#Cmb_Tipo').val() == "") {
            $('#Cmb_Tipo').parent().append('<label id="Cmb_Tipo-error" class="custom_val error" for="Cmb_Tipo">'
                + TraduzioneMultiResx(macchinaEditResx, 'IlCampoDeveEssereCompilato', 'Il campo deve essere compilato')
                + '</label>');
            $('#Cmb_Tipo').parent().children(".required").css('border', '1px solid #D41E1A');
            flag = false;
            n_inv++;
        }
    }
    var costo = parseFloat($('#Txt_CostoAcquisto').val());

    if (($('#Txt_CostoAcquisto').val() == "") || (costo < 0)) {
        $('#Txt_CostoAcquisto').parent().append('<label id="Txt_CostoAcquisto-error" class="custom_val error" for="Txt_CostoAcquisto"> '
            + TraduzioneMultiResx(macchinaEditResx, 'IlCampoDeveEssereMaggioreDiZero', 'Il campo non può essere zero o negativo')
            + '</label>');
        $('#Txt_CostoAcquisto').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }

    var ammortamento = parseFloat($('#Txt_Ammortamento').val());

    if (($('#Txt_Ammortamento').val() == "") || (ammortamento < 0)) {
        $('#Txt_Ammortamento').parent().append('<label id="Txt_Ammortamento-error" class="custom_val error" for="Txt_Ammortamento">'
            + TraduzioneMultiResx(macchinaEditResx, 'IlCampoDeveEssereMaggioreDiZero', 'Il campo non può essere zero o negativo')
            + '</label>');
        $('#Txt_Ammortamento').parent().children(".required").css('border', '1px solid #D41E1A');
        flag = false;
        n_inv++;
    }



    if (v.valid() && flag) {
        return true;
    }
    else {
        //                var err_message = "";
        //                err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red'>" + n_inv + "</div>";
        //                $('.nav-tabs li.active a').append(err_message);


        MessaggioErrore_Bootstrap(err_message);

        if (!btn_salva_clicked) {
            var err_message = "";
            err_message = "<div class='error-tab' style='position: absolute; right: 0; top: 0; background-color: red; width: 10px; text-align: center;'>!</div>";
            $('.nav-tabs li.active a').append(err_message);
        }

        btn_salva_clicked = false;

        return false;
    }


}


function GestioneUscita_ModalBS() {
    try {
        if (window.parent !== undefined) {
            window.parent.ChiusuraModale();
        }
    } catch (e) {
        console.warn("macchine: errore su window parent");
    }
}

function ValidaxSubmit() {

    btn_salva_clicked = true;
    var flag = controlla_form();

    if (flag) {


        $('#ImgBtn_SalvaTutto').click();

    }

}


//*** Funzione che applica alla WaTable la Footable
function applicaFooTable(tabella, elem_hide) {
    //$('#' + tabella + ' table thead tr').find('th:nth-child(3)').attr('data-hide', 'phone, tablet');
    $('#' + tabella + ' table').removeClass('phone');
    $('#' + tabella + ' table').removeClass('breakpoint');
    $('#' + tabella + ' table').removeClass('footable-loaded');
    $('#' + tabella + ' table').removeClass('footable');

    $('#' + tabella + ' table thead tr').find('th:nth-child(2)').attr('data-toggle', 'true');

    jQuery.each(elem_hide, function (i, val) {
        $('#' + tabella + ' table thead tr').find('th:nth-child(' + val + ')').attr('data-hide', 'phone, tablet');
    });


    setTimeout(function () {
        $('#' + tabella + ' table').footable({
            breakpoints: {
                phone: 480,
                tablet: 700
            }
        });
    }, 100);


}

function AggiornaTabRevisioni(d) {

    // AgroWA_Table_sistemaDati(d);

    var elem_hide = new Array(4, 5);

    $('#tabRevisioni').html('');
    watableRevisioni = $("#tabRevisioni").WATable({
        pageSize: 50,
        pageSizes: [50],
        filter: true,
        preFill: false,
        checkboxes: false,
        tableCreated: function (data) {
            applicaFooTable('tabRevisioni', elem_hide);
        },
        pageChanged: function (data) {
            //                                    coloraMovimenti();
        }
        , types: {
            string: { placeHolder: '...', filterTooltip: AgroWA_Table_Tooltip_String() },
            date: { format: 'dd/MM/yyyy', filterTooltip: AgroWA_Table_Tooltip_Date() },
            number: { filterTooltip: AgroWA_Table_Tooltip_Number() },
            bool: { filterTooltip: AgroWA_Table_Tooltip_Bool() }
        }
    }).data('WATable').setData(d);

    InitWaTable("#tabRevisioni", watableRevisioni);
}


function AggiornaTabCosti(d) {

    //Caricamento griglia kendo Costo Unitario
    PopolaGrigliaCostoUnitario("griglia_costo_unitario");
}



$('#btn_Aggiungi_Costi').click(function () {

    var unita_cod = $('#Cmb_Udm').val();
    var unita_des = $('#Cmb_Udm option:selected').html();
    var prezzo = $('#Txt_Prezzo').val();
    var data_in = $('#Txt_ValiditaInizio').val();
    var data_out = $('#Txt_ValiditaFine').val();

    if (data_in == "") {
        data_in = '01/01/1900';
    }

    if (data_out == "") {
        data_out = '31/12/2100';
    }

    $.ajax({
        type: 'POST',
        url: 'Macchina_Edit.aspx/Aggiungi_Costo',
        data: "{unita_cod:'" + unita_cod + "', unita_des:'" + unita_des + "', prezzo:'" + prezzo + "', data_in:'" + data_in + "' , data_out:'" + data_out + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: false,
        success: function (r) {
            AggiornaTabCosti(JSON.parse(r.d.RispostaStringa));
        }
    });


});


function EliminaCosti(obj) {
    //progetto_cod = $(obj).attr('chiave');

    var row = $(obj).parent().parent().parent().index();

    $.ajax({
        type: 'POST',
        url: 'Macchina_Edit.aspx/EliminaCosto',
        data: "{row:" + row + "}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            if (r.d.RispostaOK == true) {
                AggiornaTabCosti(JSON.parse(r.d.RispostaStringa));
                //$('#<=ImgBtn_Cancella_Distinta.ClientID %>').click();
            }
            else {
                alert(r.d.Errore);
            }
        }
    });
}

function Finalita_Change() {
    var value = $(this).val();
    if (value == "2") {
        // Commercilae
        $("#lbl_Tipo").text(TraduzioneMultiResx(macchinaEditResx, "Tipo", "Tipo"));
        $("#Cmb_Tipo").parent().children(".required").css("border", "none");
        $("#Cmb_Tipo").parent().find("#Cmb_Tipo-error").remove();
    }
    else {
        $("#lbl_Tipo").text(TraduzioneMultiResx(macchinaEditResx, "Tipo", "Tipo") + " *");
    }

}

//Funzione per il caricamento griglia
function CaricaProdottiXGrigliaCostoUnitario(options) {

    var param = kendo.stringify({
        piva: $(Controls.xPiva).val(),
        mat_cod: parseInt($(Controls.xMat_Cod).val())
    });

    ajaxAgronica("Macchina_Edit.aspx/CaricaGrigliaCostoUnitario",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

//Funzione per il salvataggio griglia
function Salva_GrigliaCostoUnitario() {
    //Scatenare submit griglia costi e costruire I param con le righe della griglia al solito modo(Ins / Mod / Eli / Tutte)
    submit_Dati_Griglie();

    var CostiModel = null;
    CostiModel = new Object();
    CostiModel.RigheInserite = righeInseriteGrid_Costo;
    CostiModel.RigheModificate = righeModificateGrid_Costo;
    CostiModel.RigheEliminate = righeEliminateGrid_Costo;
    CostiModel.TutteLeRighe = righeTutteGrid_Costo;

    var param = kendo.stringify({
        righeInserite: righeInseriteGrid_Costo,
        righeModificate: righeModificateGrid_Costo,
        righeEliminate: righeEliminateGrid_Costo,
        tutteleRighe: righeTutteGrid_Costo
    });

    ajaxAgronica("Macchina_Edit.aspx/SalvaGrigliaCosto",
        param,
        null, null);
}

//Salva nelle variabili di sessione ogni modifica alla grid
function SalvaModificheGrigliaCosto() {
    //Scatenare submit griglia costi e costruire I param con le righe della griglia al solito modo(Ins / Mod / Eli / Tutte)
    submit_Dati_Griglie();

    var CostiModel = null;
    CostiModel = new Object();
    CostiModel.RigheInserite = righeInseriteGrid_Costo;
    CostiModel.RigheModificate = righeModificateGrid_Costo;
    CostiModel.RigheEliminate = righeEliminateGrid_Costo;
    CostiModel.TutteLeRighe = righeTutteGrid_Costo;

    var param = kendo.stringify({
        righeInserite: righeInseriteGrid_Costo,
        righeModificate: righeModificateGrid_Costo,
        righeEliminate: righeEliminateGrid_Costo,
        tutteleRighe: righeTutteGrid_Costo
    });

    ajaxAgronica("Macchina_Edit.aspx/SalvaModificheGrigliaCosto",
        param,
        null, null);
}

function PopolaGrigliaCostoUnitario(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hd_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hd_UtenteAbilitatoScrittura']").val() === "True";

    var tipoOperazione = parseInt($("input[name$='hd_TipoOperazione']").val());
    if (tipoOperazione === 0)
        UteAbilitatoCanc = false

    var funzioniCRUD = {
        funzioneRead: CaricaProdottiXGrigliaCostoUnitario,
        funzioneSubmit: { funzione: SubmitProdottiXGrigliaCostoUnitario, flagInsert: true, flagUpdate: true, flagDelete: UteAbilitatoCanc },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc,
        omettiPulsantiSalva: true
    };
    var idModel = "ID";
    var campiKendoModel = {
        ID: { editable: false, type: "number" },
        Udm_Cod: { editable: true, type: "number" },
        Udm_Des: { editable: true, type: "string" },
        Prezzo_Unitario: { editable: true, type: "number" },
        Pro_Cod: { editable: false, type: "number", defaultValue: 0 },
        Mezzo: { editable: true, type: "number" },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
    };

    var colonneKendoGrid = [
        {
            field: "Udm_Des", title: TraduzioneMultiResx(macchinaEditResx, "RisorsaUDM", "Unità di Misura"), editor: unita_misura_DropDownEditor
        },
        {
            field: "Prezzo_Unitario", title: TraduzioneMultiResx(macchinaEditResx, "Prezzo", "Prezzo")
        },
        {
            field: "Validita_Inizio", title: TraduzioneMultiResx(macchinaEditResx, "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}"
        },
        {
            field: "Validita_Fine", title: TraduzioneMultiResx(macchinaEditResx, "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}"
        }
    ];

    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditCostoUnitario };
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

    let gridCosti = $("#griglia_costo_unitario").data("kendoGrid");
    gridCosti.bind("cellClose", grid_cellClose);

    //Disabilito i bottoni "aggiungi" , "annulla"  e il bottone "cancella" della griglia se sono in INFO
    if (parseInt($("input[name$='hd_TipoOperazione']").val()) === 0) {
        let gridCosti = $("#griglia_costo_unitario").data("kendoGrid");
        $(".k-grid-add", "#griglia_costo_unitario").hide();
        $(".k-grid-cancel-changes", "#griglia_costo_unitario").hide();
        gridCosti.options.editable = false;
    }
}

function onEditCostoUnitario(e) {
    //Se sono in INFO chiudo le celle della griglia
    if (parseInt($("input[name$='hd_TipoOperazione']").val()) === 0) {
        e.sender.closeCell();
    }
}

function unita_misura_DropDownEditor(container) {
    creaDropDownEditor(container, "Udm_Des", "Mezzo", ElencoUnitadimisura, changeUnitaMisuraGrigliaCostoUnitario);
}

function changeUnitaMisuraGrigliaCostoUnitario(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#griglia_costo_unitario").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Mezzo = dataItem.Mezzo;
    model.Udm_Des = dataItem.Udm_Des;
    model.dirty = true;
}


function SubmitProdottiXGrigliaCostoUnitario(options) {

    var grid = $("#griglia_costo_unitario").data("kendoGrid");

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheCompletePerSubmitProdottiXGrigliaCostoUnitario(options.data.created) +
        controllaRigheCompletePerSubmitProdottiXGrigliaCostoUnitario(options.data.updated);

    if (nrErr > 0) {
        if (nrErr === 1)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(
                macchinaEditResx,
                "EsisteRigaIncompletaNellaGrigliaCostoUnitario",
                "Esiste una riga con dati non completi nella griglia Costo Unitario."
            ), "DIV_Messaggi");
        else
            MessaggioErrore_Bootstrap(kendo.format(
                TraduzioneMultiResx(
                    macchinaEditResx,
                    "EsistonoNRigheIncompleteNellaGrigliaCostoUnitario",
                    "Esistono {0} righe con dati non completi nella griglia Costo Unitario."
                ), nrErr
            ), "DIV_Messaggi");

        erroreSubmit(grid);
        return;
    }

    errMess = controllaRigheValidePerSubmitProdottiXGrigliaCostoUnitario(options.data.created, "");
    errMess = controllaRigheValidePerSubmitProdottiXGrigliaCostoUnitario(options.data.updated, errMess);

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

        erroreSubmitGriglia(grid);
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var righeNonCancellate = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        righeNonCancellate.push(currentData[i].toJSON());
        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        // Variabili globali
        righeInseriteGrid_Costo = kendoEscapeOggetto(newRecords);
        righeModificateGrid_Costo = kendoEscapeOggetto(updatedRecords);
        righeEliminateGrid_Costo = kendoEscapeOggetto(deletedRecords);
    }

    righeTutteGrid_Costo = kendoEscapeOggetto(righeNonCancellate);
}


function controllaRigheCompletePerSubmitProdottiXGrigliaCostoUnitario(righe) {

    var nrErr = 0;
    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return nrErr;
}

function controllaRigheValidePerSubmitProdottiXGrigliaCostoUnitario(righe, precMess) {

    var errMess = precMess;

    for (x = 0; x < righe.length; x++) {
        item = righe[x];
    }

    return errMess;
}

function submit_Dati_Griglie() {

    var griglia_costo = $("#griglia_costo_unitario").data("kendoGrid");
    if (griglia_costo != null && griglia_costo != undefined)
        griglia_costo.saveChanges();
}

function erroreSubmitGriglia(grid) {

    if (grid == null || grid == undefined)
        return;

    var dsSort = [];
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }

}

function grid_cellClose(e) {
    if (e.type == "save") {
        input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Validita_Fine = new Date("2100/12/31")
        }

        input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
        if (input != undefined) {
            if (input.value() == "" || input.value() == undefined || input.value() == null)
                e.model.Validita_Inizio = new Date("1900/01/01")
        }
    }
}

//Richiamo ancora questa funzione per evitare che durante il page load vangono perse le modifiche della griglia
function OnTabShow(tabId) {
    if (tabId !== "#tab_costi") {
        //Se non sono in INFO aggiorno le righe della griglia
        if (parseInt($("input[name$='hd_TipoOperazione']").val()) !== 0) {
            var grid = $("#griglia_costo_unitario").data("kendoGrid");
            var currentData = grid.dataSource.data();
            var updatedRecords = [];
            var newRecords = [];
            var deletedRecords = [];
            var righe = [];

            for (let i = 0; i < currentData.length; i++) {
                //currentData[i].deleted = false;
                righe.push(currentData[i].toJSON());
                if (currentData[i].isNew()) {
                    newRecords.push(currentData[i].toJSON());
                }
                else if (currentData[i].dirty) {
                    updatedRecords.push(currentData[i].toJSON());
                }
            }

            for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
                deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
            }

            if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

                // Variabili globali
                righeInseriteGrid_Costo = kendoEscapeOggetto(newRecords);
                righeModificateGrid_Costo = kendoEscapeOggetto(updatedRecords);
                righeEliminateGrid_Costo = kendoEscapeOggetto(deletedRecords);
            }

            righeTutteGrid_Costo = kendoEscapeOggetto(righe);

            SalvaModificheGrigliaCosto();
        }
    }
}
