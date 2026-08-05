
function LsbTipologie_onchange() {
    
    //Estraggo i controlli
    var obj_lsbTipologie = $("#LsbTipologie");
    var obj_txtTipologia = $("#TxtNomeTipologia");
    var obj_btnTipologieEdit = $("#BtnTipologie_Edit");
    var obj_btnTipologieDel = $("#BtnTipologie_Del");
    var obj_switchTipologie_utilizzabili_da_app = $("#Tipologie_utilizzabili_da_app");
    var obj_switchAccessori_tipologia = $("#accessori_tipologia");

    //Estraggo nome e valore di ciò che è selezionato
    var nomeTipologia = $(obj_lsbTipologie).find('option:selected').text();
    var valoreTipologia = $(obj_lsbTipologie).val();

    var utilizzo_giasappTipologia = false;
    //Anna
    var FlagDataScadenzaObbligatoria = false;
    var DataDefault;


    if (parseInt($(obj_lsbTipologie).find('option:selected').data('utilizzo_giasapp')) === 1)
        utilizzo_giasappTipologia = true;
    //Anna
    if (parseInt($(obj_lsbTipologie).find('option:selected').data('flagdatascadenzaobbligatoria')) === 1)
        FlagDataScadenzaObbligatoria = true;
    DataDefault = $(obj_lsbTipologie).find('option:selected').data('datadefault')


    if (valoreTipologia != null) {
        //Se è stato selezionato qualcosa...
        $(obj_txtTipologia).val(nomeTipologia);

        if (UtenteAbilitatoScrittura && valoreTipologia > 0) {
            $(obj_btnTipologieEdit).show();
            $(obj_btnTipologieDel).show();
        } else {
            $(obj_btnTipologieEdit).hide();
            $(obj_btnTipologieDel).hide();
        }

        $("#accessori_tipologia").show();

        $(obj_switchTipologie_utilizzabili_da_app).show();

        if (UtenteAbilitatoScrittura === false) {
            $("#switch_utilizzabile_da_app").data("kendoSwitch").enable(false);

            $("#FlagDataScadenzaObbligatoria").data("kendoSwitch").enable(false);
            $("#TxtNomeTipologia").attr("disabled", true);
            $("#DataDefault").data("kendoDatePicker").enable(false);
        }

        setKendoSwitch("switch_utilizzabile_da_app", utilizzo_giasappTipologia);
        //Anna
        setKendoSwitch("FlagDataScadenzaObbligatoria", FlagDataScadenzaObbligatoria);

        $("#DataDefault").val(DataDefault.substring(0, 5)); //tolgo l'anno

    }
    else {
        //Se non è stato selezionato nulla...
        $(obj_txtTipologia).val("");
        setKendoSwitch("switch_utilizzabile_da_app", false);

        $(obj_btnTipologieEdit).hide();
        $(obj_btnTipologieDel).hide();
        $(obj_switchTipologie_utilizzabili_da_app).hide();
        $(obj_switchAccessori_tipologia).hide();
    }

}

function conferma_cancellaTipologia() {

    //Chiedo conferma prima di cancellare la Tipologia

    var obj_lsbTipologie = $("#LsbTipologie");

    var ID_Tipologia = $(obj_lsbTipologie).val();

    var text_Tipologia = $(obj_lsbTipologie).find("option:selected").text();

    var mess = ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(0, ID_Tipologia);

    if (mess !== "") {
        msgTextDialog = "Eliminare definitivamente la Tipologia '" + text_Tipologia + "'? <br> <strong>Verrano eliminati anche:<br>" + mess + "</strong>";
    }
    else {
        msgTextDialog = "Eliminare definitivamente la Tipologia '" + text_Tipologia + "'?";
    }

    let container = document.getElementById("BtnTipologie_Del");
    let id_dialog = creaNewRowDiv("id_dialog_cancella_tipologia");
    container.appendChild(id_dialog);

    $("#id_dialog_cancella_tipologia").kendoDialog({
        title: "Conferma Cancellazione Tipologia",
        closable: false,
        modal: {
            preventScroll: true
        },
        content: msgTextDialog,
        actions: [
            {
                text: 'Conferma',
                primary: true,
                action: function (e) {

                    cancellaTipologia();

                }
            },
            {
                text: 'Annulla'
            }]
    });

}

function conferma_cancellaArea() {

    //Chiedo conferma prima di cancellare la Categoria

    var obj_lsbAree = $("#LsbAree");

    var ID_Area = $(obj_lsbAree).val();

    var text_Area = $(obj_lsbAree).find("option:selected").text();

    var mess = ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(ID_Area, 0);

    if (mess !== "") {
        msgTextDialog = "Eliminare definitivamente la Categoria '" + text_Area + "'? <br> <strong>Verrano eliminati anche:<br>" + mess + "</strong>";
    }
    else {
        msgTextDialog = "Eliminare definitivamente la Categoria '" + text_Area + "'?";
    }

    let container = document.getElementById("BtnAree_Del");
    let id_dialog = creaNewRowDiv("id_dialog_cancella_area");
    container.appendChild(id_dialog);

    $("#id_dialog_cancella_area").kendoDialog({
        title: "Conferma Cancellazione Categoria",
        closable: false,
        modal: {
            preventScroll: true
        },
        content: msgTextDialog,
        actions: [
            {
                text: 'Conferma',
                primary: true,
                action: function (e) {

                    cancellaArea();

                }
            },
            {
                text: 'Annulla'
            }]
    });

}

function pulisciControlli() {
    setKendoSwitch("switch_utilizzabile_da_app", false);
    $("#DataDefault").val("");
    setKendoSwitch("FlagDataScadenzaObbligatoria", false);
    $("#accessori_tipologia").hide();
}

//FUNZIONE PER CARICARE l'ELENCO DELLE AZIENDE
//function ddlAzienda_Load() {

//    var ds = new kendo.data.DataSource({
//        transport: { read: RiempiDdlAzienda }
//    });
//    $('#ddlAzienda').kendoDropDownList({
//        filter: "contains",
//        dataSource: ds,
//        dataTextField: "rag_soc",
//        dataValueField: "piva",
//        optionLabel: { "rag_soc": "SELEZIONA...", "piva": "" },
//        autoWidth: true,
//        dataBound: ddlAzienda_OnDataBound
//    });

//}

//function ddlAzienda_OnDataBound(e) {
//    var ds = this.dataSource.data();
//    if (ds.length == 1) {
//        this.select(1); //seleziono l'elemento 
//        ddlAzienda.onchange(); //forzo l'evento di onchange
//    }
//}
