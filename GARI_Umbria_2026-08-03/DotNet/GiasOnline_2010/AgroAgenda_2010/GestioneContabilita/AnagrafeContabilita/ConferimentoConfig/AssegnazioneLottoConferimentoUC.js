function Carica_Controlli_Conferimento_LottiUC() {
    dsParametriLotto = LeggiParametriXConferimento_LottiUC();

    creaKendoDropDownList("ddlTipologia_Conferimento_LottiUC",
        { read: LeggiTipologieXConferimento_LottiUC },
        "Modulo_Des", "Modulo_Cod")
        .bind("change", ddlTipologia_Conferimento_LottiUC_change);

    creaKendoDropDownList("ddlSeparatoreParametri_Conferimento_LottiUC",
        { read: LeggiSeparatoreParametriXConferimento_LottiUC },
        "Separatore_Config_Des", "Separatore_Config")
        .bind("change", ddlSeparatoreParametri_Conferimento_LottiUC_change);

    creaKendoMultiselectParametriLottoList();
    creaKendoMultiselectParametriSceltiLottoList();

    Set_KendoDDLValue("ddlTipologia_Conferimento_LottiUC", Preparazione_Cod_Conferimento_LottiUC);
    Set_KendoDDLValue("ddlSeparatoreParametri_Conferimento_LottiUC", Separatore_Config_Conferimento_LottiUC);
    abilitaDisabilitaModifica();



    $("#divParametriLotto_Conferimento_LottiUC").show();
}

function abilitaDisabilitaModifica() {
    // abilito la lista dei parametri se:
    // 1. ho selezionato una categoria prodotto
    attivaModifica = (Preparazione_Cod_Conferimento_LottiUC != undefined);
    attivaModifica = attivaModifica && (Preparazione_Cod_Conferimento_LottiUC != null);
    attivaModifica = attivaModifica && (Preparazione_Cod_Conferimento_LottiUC > 0);

    // 2. ho selezionato un separatore
    //console.log(Separatore_Config_Conferimento_LottiUC)
    //attivaModifica = attivaModifica && (Separatore_Config_Conferimento_LottiUC != undefined);
    //attivaModifica = attivaModifica && (Separatore_Config_Conferimento_LottiUC != null);
    //attivaModifica = attivaModifica && (Separatore_Config_Conferimento_LottiUC > 0);

    let listboxParametriLotto = $("#lstParametriLotto_Conferimento_LottiUC").data("kendoListBox");
    listboxParametriLotto.enable(".k-list-item", attivaModifica);

    let listboxParametriLottoScelti = $("#lstParametriLottoScelti_Conferimento_LottiUC").data("kendoListBox");
    listboxParametriLottoScelti.enable(".k-list-item", attivaModifica);
}

function creaKendoMultiselectParametriLottoList() {
    let listboxParametriLotto = $("#lstParametriLotto_Conferimento_LottiUC").data("kendoListBox");

    if (listboxParametriLotto === undefined || listboxParametriLotto === null || listboxParametriLotto === "") {
        $("#lstParametriLotto_Conferimento_LottiUC").kendoListBox({
            dataSource: {
                data: dsParametriLotto
            },
            dataTextField: "Algoritmo_Config_Des",
            dataValueField: "Algoritmo_Config",
            toolbar: {
                tools: ["transferTo", "transferFrom", "transferAllTo", "transferAllFrom"]
            },
            selectable: "multiple",
            connectWith: "lstParametriLottoScelti_Conferimento_LottiUC"
        });
        listboxParametriLotto = $("#lstParametriLotto_Conferimento_LottiUC").data("kendoListBox");
    } else {
        listboxParametriLotto.setDataSource(new kendo.data.DataSource({ data: dsParametriLotto }));
    }

    listboxParametriLotto.dataSource.sort({ field: "Algoritmo_Config_Des", dir: "asc" });
    listboxParametriLotto.refresh();
}

function creaKendoMultiselectParametriSceltiLottoList() {
    var dsParametriLottoScelti = [];
    var Algoritmo_Config = $(hfData).val();

    if (Algoritmo_Config !== undefined && Algoritmo_Config !== null && Algoritmo_Config !== "") {
        let Array_Lotto = Algoritmo_Config.split("|");
        for (var x = 0; x < Array_Lotto.length; x++) {
            if ($.isNumeric(Array_Lotto[x]) === true) {
                let lotto = parseInt(Array_Lotto[x]);
                let scelto = dsParametriLotto.find(val => val.Algoritmo_Config == lotto);
                if (scelto != undefined && scelto != null)
                    dsParametriLottoScelti.push(scelto);
            }
        }
    }

    let listboxParametriLottoScelti = $("#lstParametriLottoScelti_Conferimento_LottiUC").data("kendoListBox");

    if (listboxParametriLottoScelti === undefined || listboxParametriLottoScelti === null || listboxParametriLottoScelti === "") {
        $("#lstParametriLottoScelti_Conferimento_LottiUC").kendoListBox({
            dataSource: {
                data: dsParametriLottoScelti
            },
            dataTextField: "Algoritmo_Config_Des",
            dataValueField: "Algoritmo_Config",
            toolbar: {
                tools: ["moveUp", "moveDown"]
            },
            selectable: "multiple",
            remove: OnRemovelstParametriLottoScelti_Conferimento_Lotti
        });
        listboxParametriLottoScelti = $("#lstParametriLottoScelti_Conferimento_LottiUC").data("kendoListBox");
    } else {
        listboxParametriLottoScelti.setDataSource(new kendo.data.DataSource({ data: dsParametriLottoScelti }));
        listboxParametriLottoScelti.refresh();
    }
}

function Conferma_Annulla_Configurazione_Lotto() {

    let container = document.getElementById("Conferimento_LottiUC_Salva");
    let id_dialog = creaNewRowDiv("id_dialog_conferma_annulla_modifiche_Configurazione_Lotto");
    container.appendChild(id_dialog);

    $("#id_dialog_conferma_annulla_modifiche_Configurazione_Lotto").kendoDialog({
        title: "Conferma Annulla Modifiche",
        closable: false,
        modal: {
            preventScroll: true
        },
        content: "Sei sicuro di voler annullare le Modifiche effettuate?",
        actions: [{
            text: 'No',
            primary: true,
        },
        {
            text: 'Sì',
            action: Annulla_Configurazione_Lotto,
        }]
    });
}

function Annulla_Configurazione_Lotto(e) {

    //Reimposto i valori di Tipologia di Lavorazione,Specie,Varietà e Regolamento
    //e ricreo la ListBox.

    Set_KendoDDLValue("ddlTipologia_Conferimento_LottiUC", Preparazione_Cod_Conferimento_LottiUC);

    //if (Preparazione_Cod_Lav_Ass_LottiUC !== 0) {

    //    $("#divParametriLotto_Lav_Ass_LottiUC").show();

    //} else if (Preparazione_Cod_Lav_Ass_LottiUC === 0) {

    //    $("#divParametriLotto_Lav_Ass_LottiUC").hide();
    //}

    Carica_Controlli_Conferimento_LottiUC();
}

function Controlli_Prima_Di_Salva_Configurazione_Lotto(ArrayParametriScelti, Tipologia_Selezionata) {

    let Msg_Errore = "";

    if (Tipologia_Selezionata === 0) {
        Msg_Errore += "Selezionare una Tipologia Di Lavorazione.";
    }

    if (ArrayParametriScelti.length === 0) {
        Msg_Errore += "Scegliere almeno un Parametro.";
    }

    return Msg_Errore;
}


