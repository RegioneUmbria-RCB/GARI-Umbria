function get_permessi(permesso_cod) {
    return new Promise((resolve, reject) => {
        ws_get_permessi(permesso_cod, function (r) {
            let p = JSON.parse(r.RispostaStringa);
            resolve(p);
        });
    });
}

function get_stalla() {
    return new Promise((resolve, reject) => {
        ws_get_stalla(function (r) {
            let p = JSON.parse(r.RispostaStringa);
            resolve(p);
        });
    });
}

function get_raggruppamento_stalla() {
    return new Promise((resolve, reject) => {
        if (obj_agenda.Tipo_Operazione !== "1") {
            ws_get_raggruppamento_stalla(function (r) {
                let p = JSON.parse(r.RispostaStringa);
                resolve(p);
            });
        } else {
            resolve({
                PIVA: obj_agenda.Piva,
                sa_cod: obj_agenda.Sa_Cod,
                STA_NUM: obj_agenda.Fabbricato,
                Raggruppamento_Cod: 0
            });
        }
    });
}

function get_lista_Tipi() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        var Cmb_Tipo = $("#cmb_tipo").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Raggruppamento_Des",
            dataValueField: "Raggruppamento_Cod",
            dataSource: { transport: { read: CaricaComboCmb_tipo } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_stalla_raggruppamento !== undefined && obj_stalla_raggruppamento.Raggruppamento_Tipo !== undefined) {
                    if (onLoad) {
                        this.value(obj_stalla_raggruppamento.Raggruppamento_Tipo);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve(this);
                }
            },
            change: function (e) {
                obj_stalla_raggruppamento.Raggruppamento_Tipo = this.value();
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");

        Cmb_Tipo.enable(obj_agenda.Tipo_Operazione == "0" ? false : true);
    });
}

function imposta_TxtValiditaInizio() {
    TxtValiditaInizio = $("#TxtValiditaInizio").kendoDatePicker({
        //value: kendo.parseDate(obj_Distinta_Selezionata.Validita_Inizio),
        dateInput: true,
        change: function () {
            if (this.value() !== null) {
                obj_stalla_raggruppamento.Validita_Inizio = this.value();
            } else {
                obj_stalla_raggruppamento.Validita_Inizio = new Date(1900, 0, 1);
            }
        }
    }).data("kendoDatePicker");
    //TxtValiditaInizio.value("");
    //TxtValiditaInizio.trigger("change");
    if (obj_stalla_raggruppamento.Validita_Inizio === undefined || kendo.parseDate(obj_stalla_raggruppamento.Validita_Inizio).toJSON() === AGRODATAINIZIO.toJSON()) {
        TxtValiditaInizio.value("");
    } else {
        TxtValiditaInizio.value(obj_stalla_raggruppamento.Validita_Inizio);
    }
    TxtValiditaInizio.trigger("change");

    if (obj_agenda.Tipo_Operazione == "0") {
        TxtValiditaInizio.enable(false);
    }
}

function imposta_TxtValiditaFine() {
    TxtValiditaFine = $("#TxtValiditaFine").kendoDatePicker({
        //value: kendo.parseDate(obj_Distinta_Selezionata.Validita_Inizio),
        dateInput: true,
        change: function () {
            if (this.value() !== null) {
                obj_stalla_raggruppamento.Validita_Fine = this.value();
            } else {
                obj_stalla_raggruppamento.Validita_Fine = new Date(2100, 12, 0);
            }
        }
    }).data("kendoDatePicker");

    if (obj_stalla_raggruppamento.Validita_Fine === undefined || kendo.parseDate(obj_stalla_raggruppamento.Validita_Fine).toJSON() === AGRODATAFINE.toJSON()) {
        TxtValiditaFine.value("");
    } else {
        TxtValiditaFine.value(obj_stalla_raggruppamento.Validita_Fine);
    }
    TxtValiditaFine.trigger("change");

    if (obj_agenda.Tipo_Operazione == "0") {
        TxtValiditaFine.enable(false);
    }
}

function imposta_txt_nome() {
    if (obj_stalla_raggruppamento.Raggruppamento_Des !== undefined) {
        $("#txt_nome").val(obj_stalla_raggruppamento.Raggruppamento_Des);
    }

    $("#txt_nome").change(function () {
        obj_stalla_raggruppamento.Raggruppamento_Des = $("#txt_nome").val();
    })

    if (obj_agenda.Tipo_Operazione == "0") {
        $("#txt_nome")[0].disabled = true;
    }
}

function imposta_txt_codice() {
    if (obj_stalla_raggruppamento.Codice !== undefined) {
        $("#txt_codice").val(obj_stalla_raggruppamento.Codice);
    }

    $("#txt_codice").change(function () {
        obj_stalla_raggruppamento.Codice = $("#txt_codice").val();
    })

    if (obj_agenda.Tipo_Operazione == "0") {
        $("#txt_codice")[0].disabled = true;
    }
}

function imposta_TxtMq() {
    if (obj_stalla_raggruppamento.Mq !== undefined) {
        $("#TxtMq").val(obj_stalla_raggruppamento.Mq);
    }

    $("#TxtMq").change(function () {
        obj_stalla_raggruppamento.Mq = $("#TxtMq").val();
    })

    if ($("#TxtMq").val() === 0) {
        obj_stalla_raggruppamento.Mq = 0;
    }

    if (obj_agenda.Tipo_Operazione == "0") {
        $("#TxtMq")[0].disabled = true;
    }
}

function imposta_CheckBdn() {
    flagBDN_switch = $("#switchBdn").kendoSwitch({
        change: function (e) {
            if (flagBDN_switch.value() == true) {
                obj_stalla_raggruppamento.Flag_BDN = 1;
            }
            else {
                obj_stalla_raggruppamento.Flag_BDN = 0;
            }
        }
    }).data("kendoSwitch");

    if (obj_stalla_raggruppamento.Flag_BDN == 1) {
        flagBDN_switch.check(true);
    }

    if (!permesso_bdn_read) {
        flagBDN_switch.check(false);
        $("#checkBDN").hide();
    }
    else {
        $("#checkBDN").show();
    }

    if (!permesso_bdn_write || obj_agenda.Tipo_Operazione == "0") {
        flagBDN_switch.enable(false);
    }
}

async function ValidaxSubmit(retOp) {
    if (validator.validate()) {
        await salva(retOp);
    } else {
        kendo.alert("I dati inseriti sono incompleti e/o non validi.");
    }
}

async function salva(retOp) {
    let risposta = await ws_salva(retOp)
    kendo.alert(risposta.RispostaStringa);
    switch (retOp) {
        case 0:
            window.location = "../MenuAnagrafica/Menubs_anagrafica.aspx?visibilita=2"
            break;
        case 1:
            await impostaTipoOperazioneObj_Agenda(1);
            location.reload();
            break;
        case 2:
            await impostaTipoOperazioneObj_Agenda(2);
            location.reload();
            break;
    }
}

function Azione_Indietro() {
    window.location = "../MenuAnagrafica/Menubs_anagrafica.aspx?visibilita=2"
}

function impostaTipoOperazioneObj_Agenda(tipoOperazione) {
    return new Promise((resolve, reject) => {
        ws_impostaTipoOperazioneObj_Agenda(tipoOperazione, function (risposta) {
            resolve();
        });
    });
}

function get_Cmb_Specie() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        var Cmb_Specie = $("#Cmb_Specie").kendoDropDownList({
            //filter: "contains",
            autoBind: true,
            dataTextField: "SPE_DES",
            dataValueField: "SPE_COD",
            dataSource: { transport: { read: CaricaComboCmb_Specie } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value(obj_stalla.s.SPE_COD);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: async function (e) {
                obj_stalla_raggruppamento.SPE_COD = this.value();
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");

        Cmb_Specie.enable(obj_agenda.Tipo_Operazione == "0" ? false : true);
    });
}

function get_Cmb_Razza() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        var Cmb_Razza = $("#Cmb_Razza").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "RAZ_DES",
            dataValueField: "RAZ_COD",
            dataSource: { transport: { read: CaricaComboCmb_Razza } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    if (obj_stalla_raggruppamento.RAZ_COD !== undefined) {
                        this.value(obj_stalla_raggruppamento.RAZ_COD);
                    } else {
                        this.value(obj_stalla.s.RAZ_COD);
                    }
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                obj_stalla_raggruppamento.RAZ_COD = this.value();
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");

        Cmb_Razza.enable(obj_agenda.Tipo_Operazione == "0" ? false : true);
    });
}

function get_Cmb_Stato() {
    Cmb_Stato = $("#Cmb_Stato").kendoDropDownList({
        //filter: "contains",
        autoBind: true,
        dataTextField: "Stato_Des",
        dataValueField: "STATO_COD",
        dataSource: { transport: { read: CaricaComboCmb_StatoAccrescimento } },
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            kendoDropDownAdjustWidth(e);
            if (obj_stalla_raggruppamento.STATO_COD !== undefined) {
                this.value(obj_stalla_raggruppamento.STATO_COD);
                this.trigger("change");
            }
        },
        change: function (e) {
            obj_stalla_raggruppamento.STATO_COD = this.value();
        },
        optionLabel: 'SELEZIONA'
    }).data("kendoDropDownList");

    Cmb_Stato.enable(obj_agenda.Tipo_Operazione == "0" ? false : true);
}