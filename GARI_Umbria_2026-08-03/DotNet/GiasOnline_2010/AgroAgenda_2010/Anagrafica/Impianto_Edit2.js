
var txtSuperficie;

/////////////////////INIZIALIZZA TAB DATI IMPIANTO/////////////////////
function impostaDati_DatiImpianto() {
    return new Promise((resolve, reject) => {
        txtSuperficie = $("#TxtSuperficie").data("kendoNumericTextBox");

        $("#LblCentro").html(obj_Impianto.sa_nome);
        $("#LblCampo").html(obj_Impianto.campo_des);
        $("#LblAppezza").html(obj_Impianto.appezza_des);
        $("#LblSubAppezza").html(obj_Impianto.sup_appezza);
        $("#lbl_centro_data_inizio").html(checkVal(kendo.parseDate(obj_Impianto.centro_data_inizio).toLocaleDateString(), AGRODATAINIZIO));
        $("#lbl_centro_data_fine").html(checkVal(kendo.parseDate(obj_Impianto.centro_data_fine).toLocaleDateString(), AGRODATAFINE));
        $("#lbl_appezza_data_inizio").html(checkVal(kendo.parseDate(obj_Impianto.appezza_data_inizio).toLocaleDateString(), AGRODATAINIZIO));
        $("#lbl_appezza_data_fine").html(checkVal(kendo.parseDate(obj_Impianto.appezza_data_fine).toLocaleDateString(), AGRODATAFINE));
        //- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        //*Txt_Data_Inizio_Portinnesto.value(obj_Impianto.Data_Inizio_Portinnesto);*/
        Txt_DataInizioInnesto.value(obj_Impianto.Data_Inizio_Innesto);
        Txt_DataInizioProduzione.value(obj_Impianto.Data_Inizio_Produzione);
        //ChkMaschiSesto.value(obj_Impianto.Piante_Maschi_InSesto);
        //- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        //$("#TxtValiditaInizio").val(checkVal(kendo.parseDate(obj_Impianto.impianto_data_inizio).toLocaleDateString(), AGRODATAINIZIO));
        //$("#TxtValiditaFine").val(checkVal(kendo.parseDate(obj_Impianto.impianto_data_fine).toLocaleDateString(), AGRODATAFINE));
        ////$("#TxtSuperficie").value(objImpianto.sup_imp);
        //txtSuperficie.value(obj_Impianto.sup_imp);

        inizializzaDropDown_DatiImpianto().then(() => {
            resolve();
        });

        if (objParametri_Agenda.Tipo_Operazione !== 0) {
            if (obj_Impianto.cul_cod === 0) {
                $("#ChkTerrenoNudo").prop("checked", true);
                $("#ChkTerrenoNudo").trigger("change");
                $("#ChkTerrenoNudo").prop("disabled", false);
            } else {
                $("#ChkTerrenoNudo").prop("checked", false);
                $("#ChkTerrenoNudo").trigger("change");
            }
        } else {
            console.log("TODO");
        }

        switch (objParametri_Agenda.Tipo_Operazione) {
            case "0":
                $("#ChkTerrenoNudo").prop("checked", false);
                $("#ChkTerrenoNudo").prop("disabled", true);
                break;
            case "1":
                $("#ChkTerrenoNudo").prop("disabled", false);
                break;
            case "2":
                if (obj_Impianto.movimentipresenti == true) {
                    $("#ChkTerrenoNudo").prop("disabled", true);
                } else {
                    $("#ChkTerrenoNudo").prop("disabled", false);
                }
                break;
            case "3":
                $("#ChkTerrenoNudo").prop("disabled", true);
                break;
        }
    });
}

function inizializzaCampi_DatiImpianto() {
    return new Promise((resolve, reject) => {
        $("#TxtSuperficie").kendoNumericTextBox({
            format: "{0:##.####}",
            decimals: 8,
            min: 0,
            value: obj_Impianto.sup_imp,
            change: function () {
                obj_Impianto.sup_imp = this.value();
            }
        });
        TxtSuperficie = $("#TxtSuperficie").data("kendoNumericTextBox");

        TxtValiditaInizio = $("#TxtValiditaInizio").kendoDatePicker({
            value: kendo.parseDate(obj_Impianto.impianto_data_inizio),
            dateInput: false,
            change: function () {
                let val_ini_imp = kendo.parseDate(this.value());
                var defaultValueI = false;
                if (val_ini_imp == null || val_ini_imp.toString() == AGRODATAINIZIO.toString()) {
                    val_ini_imp = AGRODATAINIZIO;
                    this.value("");
                    defaultValueI = true;
                }
                let val_ini_app = kendo.parseDate(obj_Impianto.appezza_data_inizio);
                if (val_ini_imp !== null) {
                    if (val_ini_imp < val_ini_app) {
                        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataInizioImpiantoInferioreDataInizioAppezzamento_",
                            "La data di inizio dell'impianto non può essere inferiore alla data di inizio dell'appezzamento: ") + val_ini_app.toLocaleDateString());
                        this.value("");
                    } else {
                        if (defaultValueI == true) {
                            obj_Impianto.impianto_data_inizio = "01/01/1900";
                        } else {
                            obj_Impianto.impianto_data_inizio = kendo.toString(this.value(), 'd');
                        }
                    }
                } else {
                    this.value(val_ini_app);
                    if (defaultValueI == true) {
                        obj_Impianto.impianto_data_inizio = "01/01/1900";
                    } else {
                        obj_Impianto.impianto_data_inizio = kendo.toString(val_ini_app, 'd');
                    }
                }
            }
        }).data("kendoDatePicker");
        if (obj_Impianto.impianto_data_inizio == DEFAULTDATE || kendo.parseDate(obj_Impianto.impianto_data_inizio).toString() == DEFAULTDATE.toString()) {
            TxtValiditaInizio.value("");
        }
        TxtValiditaInizio.trigger("change");

        TxtValiditaFine = $("#TxtValiditaFine").kendoDatePicker({
            value: kendo.parseDate(obj_Impianto.impianto_data_fine),
            dateInput: false,
            max: new Date(2100, 11, 31),
            change: function () {
                let val_fin_imp = kendo.parseDate(this.value());
                var defaultValueF = false;
                if (val_fin_imp == null || val_fin_imp.toString() == AGRODATAFINE.toString()) {
                    val_fin_imp = AGRODATAFINE;
                    this.value("");
                    defaultValueF = true;
                }
                let val_fin_app = kendo.parseDate(obj_Impianto.appezza_data_fine);
                if (val_fin_imp !== null) {
                    if (val_fin_imp > val_fin_app) {
                        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataFineImpiantoSuperioreDataFineAppezzamento_",
                            "La data di fine dell'impianto non può essere superiore alla data di fine dell'appezzamento: ") + val_fin_app.toLocaleDateString());
                        this.value("");
                    } else {
                        if (defaultValueF == true) {
                            obj_Impianto.impianto_data_fine = "31/12/2100";
                        } else {
                            obj_Impianto.impianto_data_fine = kendo.toString(this.value(), 'd');
                        }
                    }
                } else {
                    this.value(val_fin_app);
                    if (defaultValueF == true) {
                        obj_Impianto.impianto_data_fine = "31/12/2100";
                    } else {
                        obj_Impianto.impianto_data_fine = kendo.toString(val_fin_app, 'd');
                    }

                }
            }
        }).data("kendoDatePicker");
        if (obj_Impianto.impianto_data_fine == DEFAULTDATE || kendo.parseDate(obj_Impianto.impianto_data_fine).toString() == DEFAULTDATE.toString()) {
            TxtValiditaFine.value("");
        }
        TxtValiditaFine.trigger("change");

        $("#ChkTerrenoNudo").change(function () {
            if ($("#ChkTerrenoNudo").prop("checked") === true) {
                $("#chk_no").hide();
                $("#chk_yes").show();
                if ($("#Cmb_CodiciTerreno").data("kendoDropDownList") === undefined) {
                    WaitFrame.show();
                    inizializzaCmb_CodiciTerreno().then(function (e) {
                        $("#Cmb_Specie").data("kendoDropDownList").select(0);
                        $("#Cmb_Finalita").data("kendoDropDownList").select(0);
                        $("#Cmb_Cultivar").data("kendoDropDownList").select(0);
                        WaitFrame.hide();
                    });
                } else {
                    try {
                        $("#Cmb_Specie").data("kendoDropDownList").select(0);
                        $("#Cmb_Finalita").data("kendoDropDownList").select(0);
                        $("#Cmb_Cultivar").data("kendoDropDownList").select(0);
                    } catch (err) {
                        console.log(err);
                    }

                }
            } else {
                $("#chk_no").show();
                $("#chk_yes").hide();
                $("#div_coltura_annuale").hide();
                $("#ChkColturaAnnuale").prop("checked", false);
                if ($("#Cmb_Specie").data("kendoDropDownList") === undefined) {
                    WaitFrame.show();
                    inizializzaCmb_Specie().then(function () {
                        $("#Cmb_CodiciTerreno").data("kendoDropDownList").select(0);
                        WaitFrame.hide();
                    });
                } else {
                    try {
                        $("#Cmb_CodiciTerreno").data("kendoDropDownList").select(0);
                    } catch (err2) {
                        console.log(err2);
                    }
                }
            }


        });

        //--- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - -
        Txt_DataInizioInnesto = $("#Txt_DataInizioInnesto").kendoDatePicker({
            value: kendo.parseDate(obj_Impianto.Data_Inizio_Innesto),
            dateInput: false,
            change: function () {
                obj_Impianto.Data_Inizio_Innesto = kendo.toString(this.value(), 'd');
            }
        }).data("kendoDatePicker");
        if (obj_Impianto.Data_Inizio_Innesto == null || obj_Impianto.Data_Inizio_Innesto == "" || kendo.parseDate(obj_Impianto.Data_Inizio_Innesto).toLocaleDateString() == AGRODATAINIZIO.toLocaleDateString()) {
            Txt_DataInizioInnesto.value("");
        }
        Txt_DataInizioInnesto.trigger("change")

        Txt_DataInizioProduzione = $("#Txt_DataInizioProduzione").kendoDatePicker({
            value: kendo.parseDate(obj_Impianto.Data_Inizio_Produzione),
            dateInput: false,
            change: function () {
                obj_Impianto.Data_Inizio_Produzione = kendo.toString(this.value(), 'd');
            }
        }).data("kendoDatePicker");
        if (obj_Impianto.Data_Inizio_Produzione == null || obj_Impianto.Data_Inizio_Produzione == "" || kendo.parseDate(obj_Impianto.Data_Inizio_Produzione).toLocaleDateString() == AGRODATAINIZIO.toLocaleDateString()) {
            Txt_DataInizioProduzione.value("");
        }
        Txt_DataInizioProduzione.trigger("change")
        //--- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - -

        resolve();
    });
}

function inizializzaDropDown_DatiImpianto() {
    return new Promise((resolve, reject) => {
        let promises = new Array();
        if (obj_Impianto.veg_cod !== 0) {
            promises.push(inizializzaCmb_Specie());
        } else {
            promises.push(inizializzaCmb_CodiciTerreno());
        }
        Promise.all(promises).then(value => {
            if (obj_Impianto.movimentipresenti || obj_Impianto.CollegamentoPianoConcimazione) {
                if (obj_Impianto.veg_cod !== 0) {
                    Cmb_Specie.enable(false);
                    //Cmb_Cultivar.enable(false);
                    if (obj_Impianto.trattamentipresenti) {
                        Cmb_Finalita.enable(false);
                    }
                } else {
                    Cmb_CodiciTerreno.enable(true);
                    $("#ChkTerrenoNudo").prop("disabled", false);
                }
            }
            resolve();
        }, reason => {
            reject();
        });
    });
}
/////////////////////////////

/////////////////////INIZIALIZZA TAB DATI ACCESSORI/////////////////////
function impostaDati_DatiAccessori() {
    return new Promise((resolve, reject) => {
        $("#ChkFilaBinata").change(function () {
            if ($("#ChkFilaBinata").prop("checked")) {
                Txt_Interbina.enable(true);
                Txt_Interbina.value(obj_Impianto.interbina);
            } else {
                Txt_Interbina.enable(false);
                TxtPianteHa.value("");
                TxtPianteImpianto.value("");
            }
        });

        $("#ChkConsociazione").change(function () {
            obj_Impianto.chkConsociazione_checked = $("#ChkConsociazione").prop("checked");
        });

        $("#ChkMonitorato").change(function () {
            obj_Impianto.ChkMonitorato = $("#ChkMonitorato").prop("checked");
        });

        // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        $("#ChkMaschiSesto").change(function () {
            obj_Impianto.ChkMaschiSesto = $("#ChkMaschiSesto").prop("checked");
        });
        // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        $("#ChkCoverCrops").change(function () {
            obj_Impianto.ChkCoverCrops = $("#ChkCoverCrops").prop("checked");
        });

        $("#ChkVarietaIbrida").change(function () {
            if ($("#ChkVarietaIbrida").prop("checked")) {
                obj_Impianto.impianto_ibrido = 1;
                $("#linea-femmina").show();
            } else {
                obj_Impianto.impianto_ibrido = 0;
                $("#linea-femmina").hide();
            }
        });

        Txt_DistanzaTraFila_M.value(obj_Impianto.tra_fila_m);
        Txt_DistanzaSuFila_M.value(obj_Impianto.su_fila_m);
        if (obj_Impianto.Chkinterbina == false) {
            $("#ChkFilaBinata").prop("checked", false);
            $("#ChkFilaBinata").trigger("change");
        } else {
            $("#ChkFilaBinata").prop("checked", true);
            $("#ChkFilaBinata").trigger("change");
        }
        Txt_Germinabilita.value(obj_Impianto.germinabilita);
        Txt_Data_Inizio_Portinnesto.value(obj_Impianto.Data_Inizio_Portinnesto);

        $("#ChkConsociazione").prop("disabled", !obj_Impianto.chkConsociazione_enabled);
        $("#ChkConsociazione").prop("checked", obj_Impianto.chkConsociazione_checked);

        $("#ChkCoverCrops").prop("checked", obj_Impianto.ChkCoverCrops);

        $("#ChkMonitorato").prop("checked", obj_Impianto.ChkMonitorato);

        //  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        $("#ChkMaschiSesto").prop("checked", obj_Impianto.ChkMaschiSesto);
        // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        if (obj_Impianto.impianto_ibrido == 0 && obj_Impianto.grva_cod >= 0) {
            $("#ChkVarietaIbrida").prop("checked", false);
            $("#ChkVarietaIbrida").trigger("change");
        } else {
            $("#ChkVarietaIbrida").prop("checked", true);
            $("#ChkVarietaIbrida").trigger("change");
        }

        //$("#TXT_UnitaVitata").val(obj_Impianto.unita_vitata);
        $("#Txt_CodBMBDBT_M").val(obj_Impianto.CodBMBDBT_M);
        $("#Txt_CodBMBDBT_M").change(function () {
            obj_Impianto.CodBMBDBT_M = $("#Txt_CodBMBDBT_M").val();
        });

        $("#Txt_CodBMBDBT_F").val(obj_Impianto.CodBMBDBT_F);
        $("#Txt_CodBMBDBT_F").change(function () {
            obj_Impianto.CodBMBDBT_F = $("#Txt_CodBMBDBT_F").val();
        });

        $("#Txt_Genetica_M").val(obj_Impianto.Genetica_M);
        $("#Txt_Genetica_M").change(function () {
            obj_Impianto.Genetica_M = $("#Txt_Genetica_M").val();
        });

        $("#Txt_Genetica_F").val(obj_Impianto.Genetica_F);
        $("#Txt_Genetica_F").change(function () {
            obj_Impianto.Genetica_F = $("#Txt_Genetica_F").val();
        });

        $("#Txt_OffType_M").val(obj_Impianto.OffType_M);
        $("#Txt_OffType_M").change(function () {
            obj_Impianto.OffType_M = $("#Txt_OffType_M").val();
        });

        $("#Txt_OffType_F").val(obj_Impianto.OffType_F);
        $("#Txt_OffType_F").change(function () {
            obj_Impianto.OffType_F = $("#Txt_OffType_F").val();
        });

        $("#Txt_CodiceImpianto").val(obj_Impianto.codice_impianto);
        $("#Txt_CodiceImpianto").attr("readonly", algoritmoCodifica !== "" && obj_Impianto.codice_impianto !== "");
        $("#Txt_CodiceImpianto").change(function () {
            obj_Impianto.codice_impianto = $("#Txt_CodiceImpianto").val();
        });

        // campi calcolati
        TxtPianteHa.enable(false);
        TxtPianteImpianto.enable(false);
        piante_calcola(Txt_DistanzaSuFila_M.value(), Txt_DistanzaTraFila_M.value(), Txt_Interbina.value(), Txt_Germinabilita.value());

        inizializzaDropDown_DatiAccessori().then(function () {
            resolve();
        });
    });
}

function inizializzaCampi_DatiAccessori() {

    //Anna 16/05/22: aggiunto button per  [rif. chiamata 18136] 
    //nascosti campi Piante/HA e Piante/Impianto da DENSITA' IMPIANTO 
    //(nell'esportazione degli impianti viene riportato quello della distinta e alcune aziende non lo compilavano) 
    if (!flag_schedaDatiAccessoriInizializzata) {
        //creato flag per inizzializzareuna volta sola i componenti della pagina accessori (necessari per il Calcola PianteImpianto)
        flag_schedaDatiAccessoriInizializzata = true;

        return new Promise((resolve, reject) => {
            Txt_DistanzaTraFila_M = $("#Txt_DistanzaTraFila_M").kendoNumericTextBox({
                value: obj_Impianto.tra_fila_m,
                min: 0,
                change: function () {
                    obj_Impianto.tra_fila_m = this.value();
                    piante_calcola(Txt_DistanzaSuFila_M.value(), Txt_DistanzaTraFila_M.value(), Txt_Interbina.value(), Txt_Germinabilita.value());
                }
            }).data("kendoNumericTextBox");

            Txt_DistanzaSuFila_M = $("#Txt_DistanzaSuFila_M").kendoNumericTextBox({
                value: obj_Impianto.su_fila_m,
                min: 0,
                change: function () {
                    obj_Impianto.su_fila_m = this.value();
                    piante_calcola(Txt_DistanzaSuFila_M.value(), Txt_DistanzaTraFila_M.value(), Txt_Interbina.value(), Txt_Germinabilita.value());
                }
            }).data("kendoNumericTextBox");

            Txt_Interbina = $("#Txt_Interbina").kendoNumericTextBox({
                value: obj_Impianto.interbina,
                min: 0,
                change: function () {
                    obj_Impianto.interbina = this.value();
                    piante_calcola(Txt_DistanzaSuFila_M.value(), Txt_DistanzaTraFila_M.value(), Txt_Interbina.value(), Txt_Germinabilita.value());
                }
            }).data("kendoNumericTextBox");

            Txt_Germinabilita = $("#Txt_Germinabilita").kendoNumericTextBox({
                format: "{0:###}",
                decimals: 3,
                min: 0,
                value: obj_Impianto.germinabilita,
                change: function () {
                    obj_Impianto.germinabilita = this.value();
                    piante_calcola(Txt_DistanzaSuFila_M.value(), Txt_DistanzaTraFila_M.value(), Txt_Interbina.value(), Txt_Germinabilita.value());
                }
            }).data("kendoNumericTextBox");

            Txt_Superficie2 = $("#TxtSuperficie2").kendoNumericTextBox({
                format: "{0:##.####}",
                decimals: 8,
                min: 0,
                value: obj_Impianto.superficie2,
                change: function () {
                    obj_Impianto.superficie2 = this.value();
                }
            }).data("kendoNumericTextBox");

            TxtPianteHa = $("#TxtPianteHa").kendoNumericTextBox({
                format: "{0:#######}",
                decimals: 8,
                min: 0,
                value: obj_Impianto.piante_ha,
                change: function () {
                    obj_Impianto.piante_ha = this.value();
                }
            }).data("kendoNumericTextBox");

            TxtPianteImpianto = $("#TxtPianteImpianto").kendoNumericTextBox({
                format: "{0:#######}",
                decimals: 8,
                min: 0,
                value: obj_Impianto.piante_impianto,
                change: function () {
                    obj_Impianto.piante_impianto = this.value();
                }
            }).data("kendoNumericTextBox");

            TxtCopDataInizio = $("#TxtCopDataInizio").kendoDatePicker({
                value: kendo.parseDate(obj_Impianto.cop_data_inizio),
                dateInput: false,
                change: function () {
                    obj_Impianto.cop_data_inizio = kendo.toString(this.value(), 'd');
                }
            }).data("kendoDatePicker");
            if (obj_Impianto.cop_data_inizio == null || obj_Impianto.cop_data_inizio == "" || kendo.parseDate(obj_Impianto.cop_data_inizio).toLocaleDateString() == AGRODATAINIZIO.toLocaleDateString()) {
                TxtCopDataInizio.value("");
            }
            TxtCopDataInizio.trigger("change");

            TxtCopDataFine = $("#TxtCopDataFine").kendoDatePicker({
                value: kendo.parseDate(obj_Impianto.cop_data_fine),
                dateInput: false,
                change: function () {
                    obj_Impianto.cop_data_fine = kendo.toString(this.value(), 'd');
                }
            }).data("kendoDatePicker");
            if (obj_Impianto.cop_data_fine == null || obj_Impianto.cop_data_fine == "" || kendo.parseDate(obj_Impianto.cop_data_fine).toLocaleDateString() == AGRODATAFINE.toLocaleDateString()) {
                TxtCopDataFine.value("");
            }
            TxtCopDataFine.trigger("change");

            Txt_Resa1 = $("#Txt_Resa1").kendoNumericTextBox({
                value: obj_Impianto.Resa_Prevista,
                min: 0,
                change: function () {
                    obj_Impianto.Resa_Prevista = this.value();
                }
            }).data("kendoNumericTextBox");

            Txt_Resa2 = $("#Txt_Resa2").kendoNumericTextBox({
                value: obj_Impianto.Resa_Effettiva,
                min: 0,
                change: function () {
                    obj_Impianto.Resa_Effettiva = this.value();
                }
            }).data("kendoNumericTextBox");

            Txt_DistanzaSuFila_F = $("#Txt_DistanzaSuFila_F").kendoNumericTextBox({
                value: obj_Impianto.DistanzaSuFila_F,
                min: 0,
                change: function () {
                    obj_Impianto.DistanzaSuFila_F = this.value();
                }
            }).data("kendoNumericTextBox");

            Txt_DistanzaTraFila_F = $("#Txt_DistanzaTraFila_F").kendoNumericTextBox({
                value: obj_Impianto.DistanzaTraFila_F,
                min: 0,
                change: function () {
                    obj_Impianto.DistanzaTraFila_F = this.value();
                }
            }).data("kendoNumericTextBox");

            Txt_PartiTuberi = $("#Txt_PartiTuberi").kendoNumericTextBox({
                format: "{0:#######}",
                decimals: 8,
                min: 0,
                value: obj_Impianto.partiTuberi,
                change: function () {
                    obj_Impianto.partiTuberi = this.value();
                }
            }).data("kendoNumericTextBox");

            TXT_UnitaVitata = $("#TXT_UnitaVitata").kendoNumericTextBox({
                format: "{0:###}",
                decimals: 3,
                min: 0,
                value: obj_Impianto.unita_vitata,
                change: function () {
                    obj_Impianto.unita_vitata = this.value();
                }
            }).data("kendoNumericTextBox");

            Txt_Data_Inizio_Portinnesto = $("#Txt_Data_Inizio_Portinnesto").kendoDatePicker({
                value: kendo.parseDate(obj_Impianto.Data_Inizio_Portinnesto),
                dateInput: false,
                change: function () {
                    obj_Impianto.Data_Inizio_Portinnesto = kendo.toString(this.value(), 'd');
                }
            }).data("kendoDatePicker");
            if (obj_Impianto.Data_Inizio_Portinnesto == "" || kendo.parseDate(obj_Impianto.Data_Inizio_Portinnesto).toLocaleDateString() == AGRODATAINIZIO.toLocaleDateString()) {
                Txt_Data_Inizio_Portinnesto.value("");
            }
            Txt_Data_Inizio_Portinnesto.trigger("change");


            resolve();
        });

    }


}

function inizializzaDropDown_DatiAccessori() {
    return new Promise((resolve, reject) => {
        let promises = new Array();
        //promises.push(inizializzaCmb_TipologiaVarietale());
        promises.push(inizializzaCmb_Copertura());
        promises.push(inizializzaCmb_ImpIrrigazione());
        promises.push(inizializzaCmb_FormaAllevamento());
        promises.push(inizializzaCmb_Portinnesto());
        promises.push(inizializzaCmb_SeminaTrapianto());
        promises.push(inizializzaCmb_ProvenienzaSeme());
        promises.push(inizializzaCmb_DettaglioVarietaPersonalizzato());
        promises.push(inizializzaCmb_ConduzioneTra());
        promises.push(inizializzaCmb_ConduzioneSu());
        promises.push(inizializzaCmb_TagliatoTuberi());
        promises.push(inizializzaCmb_CodiceZona());
        //promises.push(inizializzaCmb_CodiciTerreno());
        Promise.all(promises).then(value => {
            resolve();
        }, reason => {
            reject();
        });
    });
}
/////////////////////////////

/////////////////////INIZIALIZZA TAB DATI DISTINTE/////////////////////
function impostaDati_DatiDistinte() {
    return new Promise((resolve, reject) => {
        $("#kendoDistinte").val(obj_Impianto.dati_Distinte);
        inizializzaKendoDistinte('tabImpianti');

        inizializzaDropDown_DatiDistinte().then(function () {
            resolve();
        });
    });
}

function inizializzaCampi_DatiDistinte() {
    return new Promise((resolve, reject) => {
        /* Txt_Sup_Prog = $("#Txt_Sup_Prog").kendoNumericTextBox({
            format: "{0:##.####}",
            decimals: 8,
            min: 0,
            // value: obj_Distinta_Selezionata.Sup_Prog,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.Sup_Prog = this.value();
                }
            }
        }).data("kendoNumericTextBox");
        Txt_Sup_Prog.value("");
        Txt_Sup_Prog.trigger("change"); */

        Txt_ValiditaInizio_Distinta = $("#Txt_ValiditaInizio_Distinta").kendoDatePicker({
            //value: kendo.parseDate(obj_Distinta_Selezionata.Validita_Inizio),
            dateInput: true,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    let val_inizio_dist = this.value() !== null ? kendo.parseDate(this.value()) : new Date(1900, 0, 1);
                    let val_inizio_imp = kendo.parseDate(obj_Impianto.impianto_data_inizio);
                    if (val_inizio_dist !== null && val_inizio_imp !== null) {
                        if (val_inizio_dist < val_inizio_imp) {
                            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataInizioDistintaInferioreDataInizioImpianto_",
                                "La data di inizio della distinta non può essere inferiore alla data di inizio dell'impianto: ") + val_inizio_imp.toLocaleDateString());
                            this.value("");
                        } else {
                            obj_Distinta_Selezionata.Validita_Inizio = kendo.toString(this.value() !== null ? this.value() : val_inizio_dist, 'd');
                        }
                    }
                }
            }
        }).data("kendoDatePicker");
        Txt_ValiditaInizio_Distinta.value("");
        Txt_ValiditaInizio_Distinta.trigger("change");


        Txt_ValiditaFine_Distinta = $("#Txt_ValiditaFine_Distinta").kendoDatePicker({
            //value: kendo.parseDate(obj_Distinta_Selezionata.Validita_Fine),
            max: new Date(2100, 11, 31),
            dateInput: true,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    let val_fine_dist = this.value() !== null ? kendo.parseDate(this.value()) : new Date(2100, 11, 31);
                    let val_fine_imp = kendo.parseDate(obj_Impianto.impianto_data_fine);
                    if (val_fine_dist !== null && val_fine_imp !== null) {
                        if (val_fine_dist > val_fine_imp) {
                            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataFineDistintaSuperioreDataFineImpianto_",
                                "La data di fine della distinta non può essere superiore alla data di fine dell'impianto: ") + val_fine_imp.toLocaleDateString());
                            this.value("");
                        } else {
                            obj_Distinta_Selezionata.Validita_Fine = kendo.toString(this.value() !== null ? this.value() : val_fine_dist, 'd');
                        }
                    }
                }
            }
        }).data("kendoDatePicker");
        Txt_ValiditaFine_Distinta.value("");
        Txt_ValiditaFine_Distinta.trigger("change");

        Txt_PianteHa2 = $("#TxtPianteHa2").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                obj_Distinta_Selezionata.p_ha = this.value();
                Txt_PianteImpianto2.value(Math.round(obj_Distinta_Selezionata.p_ha * obj_Impianto.sup_imp));
            }
        }).data("kendoNumericTextBox");

        Txt_PianteImpianto2 = $("#TxtPianteImpianto2").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                Txt_PianteHa2.value(Math.round(this.value() / obj_Impianto.sup_imp));
                obj_Distinta_Selezionata.p_ha = Txt_PianteHa2.value();
            }
        }).data("kendoNumericTextBox");

        Txt_PianteHa_Femmina = $("#Txt_PianteHa_Femmina").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                obj_Distinta_Selezionata.P_HA_Femmine = this.value();
                Txt_PianteImpianto_Femmina.value(Math.round(obj_Distinta_Selezionata.P_HA_Femmine * obj_Impianto.sup_imp));
            }
        }).data("kendoNumericTextBox");

        Txt_PianteImpianto_Femmina = $("#Txt_PianteImpianto_Femmina").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                obj_Distinta_Selezionata.P_HA_Femmine = Txt_PianteHa_Femmina.value();
                Txt_PianteHa_Femmina.value(Math.round(this.value() / obj_Impianto.sup_imp));
            }
        }).data("kendoNumericTextBox");

        Txt_PianteHa_Maschio = $("#Txt_PianteHa_Maschio").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                obj_Distinta_Selezionata.P_HA_Maschi = this.value();
                Txt_PianteImpianto_Maschio.value(Math.round(obj_Distinta_Selezionata.P_HA_Maschi * obj_Impianto.sup_imp));
            }
        }).data("kendoNumericTextBox");

        Txt_PianteImpianto_Maschio = $("#Txt_PianteImpianto_Maschio").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.PianteImpianto,
            format: "{0:######}",
            decimals: 8,
            min: 0,
            change: function () {
                obj_Distinta_Selezionata.P_HA_Maschi = Txt_PianteHa_Maschio.value();
                Txt_PianteHa_Maschio.value(Math.round(this.value() / obj_Impianto.sup_imp));
            }
        }).data("kendoNumericTextBox");

        Txt_semina_prevista = $("#Txt_semina_prevista").kendoDatePicker({
            //value: kendo.parseDate(obj_Distinta_Selezionata.data_semina_prevista),
            dateInput: false,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.data_semina_prevista = kendo.toString(this.value(), 'd');
                }
            }
        }).data("kendoDatePicker");
        Txt_semina_prevista.value("");
        Txt_semina_prevista.trigger("change");

        Txt_fioritura_prevista = $("#Txt_fioritura_prevista").kendoDatePicker({
            //value: kendo.parseDate(obj_Distinta_Selezionata.data_fioritura_prevista),
            dateInput: false,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.data_fioritura_prevista = kendo.toString(this.value(), 'd');
                }
            }
        }).data("kendoDatePicker");
        Txt_fioritura_prevista.value("");
        Txt_fioritura_prevista.trigger("change");

        Txt_raccolta_prevista = $("#Txt_raccolta_prevista").kendoDatePicker({
            //value: kendo.parseDate(obj_Distinta_Selezionata.data_raccolta_prevista),
            dateInput: false,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.data_raccolta_prevista = kendo.toString(this.value(), 'd');
                }
            }
        }).data("kendoDatePicker");
        Txt_raccolta_prevista.value("");
        Txt_raccolta_prevista.trigger("change");

        Txt_ResaPrevista = $("#Txt_ResaPrevista").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.Resa_Prevista,
            min: 0,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.produzione_prevista = kendo.toString(this.value(), 'd');
                }
            }
        }).data("kendoNumericTextBox");

        TxtN = $("#TxtN").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.p_ha,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.TxtN = this.value();
                }
            }
        }).data("kendoNumericTextBox");

        TxtP2O5 = $("#TxtP2O5").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.p_ha,
            min: 0,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.TxtP2O5 = this.value();
                }
            }
        }).data("kendoNumericTextBox");

        TxtK2O = $("#TxtK2O").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.p_ha,
            min: 0,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.TxtK2O = this.value();
                }
            }
        }).data("kendoNumericTextBox");

        TxtMgO = $("#TxtMgO").kendoNumericTextBox({
            //value: obj_Distinta_Selezionata.p_ha,
            min: 0,
            change: function () {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.TxtMgO = this.value();
                }
            }
        }).data("kendoNumericTextBox");

        $("#Txt_Lotto").change(function () {
            if (obj_Distinta_Selezionata !== undefined) {
                obj_Distinta_Selezionata.Progetto_Nome = $("#Txt_Lotto").val();
            }
        });

        $("#Txt_Descrizione").change(function () {
            if (obj_Distinta_Selezionata !== undefined) {
                obj_Distinta_Selezionata.Progetto_Des = $("#Txt_Descrizione").val();
            }
        });

        chk_distinta_chiusa = creaKendoSwitch(
            "chk_distinta_chiusa",
            TraduzioneMultiResx(impiantoEditResx, "Si", "Si").toUpperCase(),
            TraduzioneMultiResx(impiantoEditResx, "No", "No").toUpperCase(),
            false,
            function (e) {
                if (e.checked) {
                    obj_Distinta_Selezionata.distinta_chiusa = "1";
                    if (replicaGIAS !== "") {
                        chk_distinta_replica.check(true);
                        chk_distinta_replica.enable(false);
                    }
                } else {
                    obj_Distinta_Selezionata.distinta_chiusa = "0";
                    if (replicaGIAS !== "") {
                        // chk_distinta_replica.check(false);
                        chk_distinta_replica.enable(true);
                    }
                }
            });

        chk_secondo_raccolto = creaKendoSwitch(
            "chk_secondo_raccolto",
            TraduzioneMultiResx(impiantoEditResx, "Si", "Si").toUpperCase(),
            TraduzioneMultiResx(impiantoEditResx, "No", "No").toUpperCase(),
            false,
            function (e) {
                if (e.checked) {
                    obj_Distinta_Selezionata.FlagSecondoRaccolto = "1";
                    if (replicaGIAS !== "") {
                        chk_secondo_raccolto.check(true);
                        chk_secondo_raccolto.enable(false);
                    }
                } else {
                    obj_Distinta_Selezionata.FlagSecondoRaccolto = "0";
                    if (replicaGIAS !== "") {
                        chk_secondo_raccolto.enable(true);
                    }
                }
            });

        if (replicaGIAS !== "") {
            chk_distinta_replica = creaKendoSwitch(
                "chk_distinta_replica",
                TraduzioneMultiResx(impiantoEditResx, "Si", "Si").toUpperCase(),
                TraduzioneMultiResx(impiantoEditResx, "No", "No").toUpperCase(),
                false,
                function (e) {
                    if (e.checked) {
                        obj_Distinta_Selezionata.distinta_replica = "1";
                    } else {
                        obj_Distinta_Selezionata.distinta_replica = "0";
                    }
                });
        }

        /* chk_distinta_chiusa = $("#chk_distinta_chiusa").kendoMobileSwitch({
            onLabel: "SI",
            offLabel: "NO",
            change: function () {
                if (chk_distinta_chiusa.check() === true) {
                    obj_Distinta_Selezionata.distinta_chiusa = "1";
                } else {
                    obj_Distinta_Selezionata.distinta_chiusa = "0";
                }
            }
        }).data("kendoMobileSwitch"); */

        resolve();
    });
}

function inizializzaDropDown_DatiDistinte() {
    return new Promise((resolve, reject) => {
        let promises = new Array();
        promises.push(inizializzaCmb_Regolamento());
        promises.push(inizializzaCmb_Disciplinare());
        promises.push(inizializzaCmb_IAF());
        promises.push(inizializzaCmb_CapitolatoPrivato());
        promises.push(inizializzaCmb_OrganismoReferente());
        // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        promises.push(inizializzaCmb_LicenzaColtivazione());
        // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
        promises.push(inizializzaCmb_Riferimento_Trasferimento_Dati());
        promises.push(inizializzaCmb_MagazzinoConferimento());
        promises.push(inizializzaCmb_RegolamentoConc());
        promises.push(inizializzaCmb_FinalitaConc());
        promises.push(inizializzaCmb_TipologiaVarietale());
        promises.push(inizializzaCmb_Stato());
        promises.push(inizializzaCmb_PianoSemina());
        Promise.all(promises).then(value => {
            resolve();
        }, reason => {
            reject();
        });
    });
}

/////////////////
function inizializzaKendoDistinte(idDiv) {

    var funzioniCRUD = {
        funzioneRead: dataGrigliaDistinte
    };

    var idModel = "Progetto_Cod";
    var campiKendoModel = modelGrigliaDistinte();
    var colonneKendoGrid = colonneGrigliaDistinte();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    //var template = kendo.template($("#popupApp_Template").html());

    var templateCommand = "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' " +
        "onclick=infoDistinta(this.closest('tr'),this.closest('.k-grid'))>" +
        TraduzioneMultiResx(impiantoEditResx, "Info", "Info") + "</div>";
    if (objParametri_Agenda.Tipo_Operazione !== "0") {
        templateCommand += "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' " +
            "onclick=modificaDistinta(this.closest('tr'),this.closest('.k-grid'))>" +
            TraduzioneMultiResx(impiantoEditResx, "Modifica", "Modifica") +
            "</div><div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;'" +
            " onclick=eliminaDistinta(this.closest('tr'),this.closest('.k-grid'))>" +
            TraduzioneMultiResx(impiantoEditResx, "RisorsaCancella", "Cancella") + "</div>";
    }

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        selectable: "row",
        filterable: { mode: "row" },
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: templateCommand
                }, title: TraduzioneMultiResx(impiantoEditResx, "Azioni", "Azioni"), width: "97px"
            }
        ]

    };


    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: App_onDataBoundDistinte };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
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
    gridDistinte = $("#" + idDiv).data("kendoGrid");
}

function nuovaDistinta(e) {
    let obj_distinta = new Object();

    if (Cmb_Specie !== undefined) {
        obj_distinta.Progetto_Des = TraduzioneMultiResx(impiantoEditResx, "Esercizio", "Esercizio") + ": " + obj_Impianto.appezza_des + " - " + Cmb_Specie.text() + " - " + Cmb_Cultivar.text();
        obj_distinta.Progetto_Nome = TraduzioneMultiResx(impiantoEditResx, "Esercizio", "Esercizio") + ": " + obj_Impianto.appezza_des + " - " + Cmb_Specie.text() + " - " + Cmb_Cultivar.text();
    } else {
        obj_distinta.Progetto_Des = TraduzioneMultiResx(impiantoEditResx, "Esercizio", "Esercizio") + ": " + obj_Impianto.appezza_des + " - " + Cmb_CodiciTerreno.text();
        obj_distinta.Progetto_Nome = TraduzioneMultiResx(impiantoEditResx, "Esercizio", "Esercizio") + ": " + obj_Impianto.appezza_des + " - " + Cmb_CodiciTerreno.text();
    }

    // date fine ultimo esercizio
    var data_ultimo_esercizio = AGRODATAINIZIO;
    var data = gridDistinte.dataSource.data();
    for (let i = 0; i < data.length; i++) {
        let data_fine_esercizio = kendo.parseDate(data[i].Validita_Fine);
        if (data_fine_esercizio > data_ultimo_esercizio) {
            data_ultimo_esercizio = data_fine_esercizio;
        }
    }

    // imposta date nuovo esercizio
    if (data_ultimo_esercizio < AGRODATAFINE) {
        let data_inizio = obj_Impianto.impianto_data_inizio != null ? kendo.parseDate(obj_Impianto.impianto_data_inizio) : AGRODATAINIZIO;
        let data_fine = obj_Impianto.impianto_data_fine != null ? kendo.parseDate(obj_Impianto.impianto_data_fine) : AGRODATAFINE;
        if (data_ultimo_esercizio > AGRODATAINIZIO) {
            data_inizio = new Date(data_ultimo_esercizio);
            data_fine = new Date(data_ultimo_esercizio);
            data_inizio.setDate(data_inizio.getDate() + 1);
            data_fine.setFullYear(data_fine.getFullYear() + 1);
        }
        obj_distinta.Validita_Inizio = kendo.toString(data_inizio, 'd');
        obj_distinta.Validita_Fine = kendo.toString(data_fine, 'd');
        let anno_inizio = data_inizio.getFullYear();
        let anno_fine = data_fine.getFullYear();
        if (anno_inizio != anno_fine) {
            obj_distinta.Progetto_Des += " " + anno_inizio + "-" + anno_fine;
            obj_distinta.Progetto_Nome += " " + TraduzioneMultiResx(impiantoEditResx, "Lotto", "Lotto") + " " + anno_inizio + "-" + anno_fine;
        } else {
            obj_distinta.Progetto_Des += " " + anno_inizio;
            obj_distinta.Progetto_Nome += " " + TraduzioneMultiResx(impiantoEditResx, "Lotto", "Lotto") + " " + anno_inizio;
        }
    }

    // imposta codice lotto vuoto se attivo algoritmo codifica
    if (algoritmoCodifica !== "") {
        obj_distinta.Progetto_Des = "";
        obj_distinta.Progetto_Nome = "";
    }

    obj_distinta.Progetto_Cod = 0;
    obj_distinta.obj_Codici_Distinta = " [ ] ";
    obj_distinta.obj_Particelle_Distinta = " [ ] ";
    impostaDatiDistinta(obj_distinta);
    impostaPermessiDatiDistinta(true);

}

function dataGrigliaDistinte(options) {
    var a = JSON.parse($("#kendoDistinte").val());
    options.success(a.kendo_rows);
}

function modelGrigliaDistinte(options) {
    var a = JSON.parse($("#kendoDistinte").val());
    return a.kendo_model;
}

function colonneGrigliaDistinte(options) {
    var a = JSON.parse($("#kendoDistinte").val());
    return a.kendo_columns;
}

function App_onDataBoundDistinte(e) {

}

function infoDistinta(tr_elem, grid_elem) {
    var datiRiga = gridDistinte.dataItem(tr_elem);
    impostaDatiDistinta(datiRiga);
    impostaPermessiDatiDistinta(false);
}

function modificaDistinta(tr_elem, grid_elem) {
    var datiRiga = gridDistinte.dataItem(tr_elem);
    impostaDatiDistinta(datiRiga);
    impostaPermessiDatiDistinta(true);
}

function eliminaDistinta(tr_elem, grid_elem) {
    var datiRiga = gridDistinte.dataItem(tr_elem);
    var data = gridDistinte.dataSource.data();

    var prosegui = WS_Controlla_DatixEliminazioneDistinta(kendo.stringify(datiRiga))

    switch (prosegui.TipoMessaggioRitorno) {
        case "alert":
            WaitFrame.hide();
            kendo.alert(prosegui.Messaggio);
            break;
        case "procedi":
            if (data.length > 1) {
                for (let i = 0; i < data.length; i++) {
                    if (data[i].uid == datiRiga.uid) {
                        data.splice(i, 1);
                    }
                }
                var objDistinte = JSON.parse($("#kendoDistinte").val());
                objDistinte.kendo_rows = $.extend({}, gridDistinte.dataSource.data());
                $("#kendoDistinte").val(JSON.stringify(objDistinte));
                obj_Impianto.dati_Distinte = $.extend({}, objDistinte);
                gridDistinte.destroy();
                inizializzaKendoDistinte('tabImpianti');
                impostaDatiDistinta(new Object());
                impostaPermessiDatiDistinta(false);
            } else {
                kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpossibileEliminareLUnicoEsercizioImpianto", "Non è possibile eliminare l'unico esercizio dell'impianto"));
            }
            break;
    }
}

function impostaDatiDistinta(obj_distinta) {
    obj_Distinta_Selezionata = undefined;
    obj_Distinta_Selezionata = $.extend({}, obj_distinta);
    //obj_Distinta_Selezionata = obj_distinta;
    inizializzaCmb_Regolamento();
    //inizializzaCmb_Disciplinare();
    inizializzaCmb_CapitolatoPrivato();
    inizializzaCmb_OrganismoReferente();
    // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
    inizializzaCmb_LicenzaColtivazione();
    // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
    inizializzaCmb_Riferimento_Trasferimento_Dati();
    inizializzaCmb_MagazzinoConferimento();
    inizializzaCmb_PianoSemina();
    //inizializzaCmb_RegolamentoConc();
    //inizializzaCmb_Stato();
    inizializzaCmb_CapitolatoPrivato();

    if (obj_Distinta_Selezionata.Validita_Inizio !== undefined && obj_Distinta_Selezionata.Validita_Inizio !== DEFAULTDATE.toLocaleDateString() && obj_Distinta_Selezionata.Validita_Inizio !== kendo.toString(AGRODATAINIZIO, 'd')) {
        Txt_ValiditaInizio_Distinta.value(obj_Distinta_Selezionata.Validita_Inizio);
    } else {
        Txt_ValiditaInizio_Distinta.value("");
    }

    if (obj_Distinta_Selezionata.Validita_Fine !== undefined && obj_Distinta_Selezionata.Validita_Fine !== DEFAULTDATE.toLocaleDateString() && obj_Distinta_Selezionata.Validita_Fine !== kendo.toString(AGRODATAFINE, 'd')) {
        Txt_ValiditaFine_Distinta.value(obj_Distinta_Selezionata.Validita_Fine);
    } else {
        Txt_ValiditaFine_Distinta.value("");
    }

    Txt_PianteHa2.value(obj_Distinta_Selezionata.p_ha);
    Txt_PianteImpianto2.value(Math.round(obj_Distinta_Selezionata.p_ha * obj_Impianto.sup_imp));

    // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
    Txt_PianteHa_Femmina.value(obj_Distinta_Selezionata.P_HA_Femmine);
    Txt_PianteImpianto_Femmina.value(Math.round(obj_Distinta_Selezionata.P_HA_Femmine * obj_Impianto.sup_imp));

    Txt_PianteHa_Maschio.value(obj_Distinta_Selezionata.P_HA_Maschi);
    Txt_PianteImpianto_Maschio.value(Math.round(obj_Distinta_Selezionata.P_HA_Maschi * obj_Impianto.sup_imp));
    // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


    if (obj_Distinta_Selezionata.data_semina_prevista !== undefined && obj_Distinta_Selezionata.data_semina_prevista !== DEFAULTDATE.toLocaleDateString() && obj_Distinta_Selezionata.data_semina_prevista !== kendo.toString(AGRODATAINIZIO, 'd')) {
        Txt_semina_prevista.value(obj_Distinta_Selezionata.data_semina_prevista);
    } else {
        Txt_semina_prevista.value("");
    }

    if (obj_Distinta_Selezionata.data_fioritura_prevista !== undefined && obj_Distinta_Selezionata.data_fioritura_prevista !== DEFAULTDATE.toLocaleDateString() && obj_Distinta_Selezionata.data_fioritura_prevista !== kendo.toString(AGRODATAINIZIO, 'd')) {
        Txt_fioritura_prevista.value(obj_Distinta_Selezionata.data_fioritura_prevista);
    } else {
        Txt_fioritura_prevista.value("");
    }

    if (obj_Distinta_Selezionata.data_raccolta_prevista !== undefined && obj_Distinta_Selezionata.data_raccolta_prevista !== DEFAULTDATE.toLocaleDateString() && obj_Distinta_Selezionata.data_raccolta_prevista !== kendo.toString(AGRODATAFINE, 'd')) {
        Txt_raccolta_prevista.value(obj_Distinta_Selezionata.data_raccolta_prevista);
    } else {
        Txt_raccolta_prevista.value("");
    }

    $("#Txt_Lotto").val(obj_Distinta_Selezionata.Progetto_Nome);
    // Txt_Sup_Prog.value(obj_Distinta_Selezionata.Sup_Prog);

    // imposta codice lotto non modificabile se attivo algoritmo codifica
    if (algoritmoCodifica !== "") $("#Txt_Lotto").attr("readonly", true);

    $("#Txt_Descrizione").val(obj_Distinta_Selezionata.Progetto_Des);
    $("#TxtCodice").val(obj_Distinta_Selezionata.Progetto_Cod);

    TxtN.value(obj_Distinta_Selezionata.TxtN);

    TxtP2O5.value(obj_Distinta_Selezionata.TxtP2O5);

    TxtK2O.value(obj_Distinta_Selezionata.TxtK2O);

    TxtMgO.value(obj_Distinta_Selezionata.TxtMgO);

    Txt_ResaPrevista.value(obj_Distinta_Selezionata.produzione_prevista);

    if (obj_Distinta_Selezionata.distinta_chiusa === "1") {
        chk_distinta_chiusa.check(true);
        chk_distinta_chiusa.enable(false);
    } else {
        chk_distinta_chiusa.check(false);
        chk_distinta_chiusa.enable(true);
    }

    if (obj_Distinta_Selezionata.FlagSecondoRaccolto === "1") {
        chk_secondo_raccolto.check(true);
        chk_secondo_raccolto.enable(false);
    } else {
        chk_secondo_raccolto.check(false);
        chk_secondo_raccolto.enable(true);
    }

    if (replicaGIAS !== "") {
        if (obj_Distinta_Selezionata.distinta_replica === "1") {
            chk_distinta_replica.check(true);
            chk_distinta_replica.enable(false);
        } else {
            chk_distinta_replica.check(false);
            chk_distinta_replica.enable(true);
        }
        chk_distinta_replica.enable(obj_Distinta_Selezionata.distinta_replica_codice === "");
    }

    if (algoritmoCodifica !== "") {
        if (obj_Distinta_Selezionata.Progetto_Cod === 0) {
            $("#btnReplica").hide();
        } else {
            $("#btnReplica").show();
        }
    }

    inizializzaKendoCodiciDistinte('tabCodici');

    inizializzaKendoParticelleDistinte('tabParticelle');

}

function impostaPermessiDatiDistinta(enabled) {
    Cmb_Regolamento.enable(enabled);
    Cmb_Disciplinare.enable(enabled);
    if (objPermessi_IAF) {
        Cmb_IAF.enable(enabled);
    } else {
        Cmb_IAF.enable(false);
    }
    Cmb_CapitolatoPrivato.enable(enabled);
    Cmb_OrganismoReferente.enable(enabled);
    // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
    Cmb_LicenzaColtivazione.enable(enabled);
    // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
    Cmb_Riferimento_Trasferimento_Dati.enable(enabled);
    Cmb_MagazzinoConferimento.enable(enabled);
    Cmb_PianoSemina.enable(enabled);
    Cmb_RegolamentoConc.enable(enabled);
    Cmb_FinalitaConc.enable(enabled);
    Cmb_Stato.enable(enabled);

    Txt_ValiditaInizio_Distinta.enable(enabled);
    Txt_ValiditaFine_Distinta.enable(enabled);
    Txt_PianteHa2.enable(enabled);
    Txt_PianteImpianto2.enable(enabled);
    // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
    Txt_PianteHa_Femmina.enable(enabled);
    Txt_PianteHa_Maschio.enable(enabled);
    Txt_PianteImpianto_Femmina.enable(enabled);
    Txt_PianteImpianto_Maschio.enable(enabled);
    // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
    Txt_semina_prevista.enable(enabled);
    Txt_fioritura_prevista.enable(enabled);
    Txt_raccolta_prevista.enable(enabled);
    //$("#Txt_Lotto").enable(enabled);
    $("#Txt_Lotto").prop('disabled', !enabled);
    // Txt_Sup_Prog.enable(enabled);
    // Txt_Sup_Prog.enable(false);
    TxtN.enable(enabled);
    TxtP2O5.enable(enabled);
    TxtK2O.enable(enabled);
    TxtMgO.enable(enabled);
    Txt_ResaPrevista.enable(enabled);
    $("#Txt_Descrizione").prop('disabled', !enabled);

    chk_secondo_raccolto.enable(enabled)

    if (algoritmoCodifica !== "") {
        if (enabled) $("#btnGeneraDescrizione").removeClass('disabled');
        else $("#btnGeneraDescrizione").addClass('disabled');
    }

    if (enabled) {
        btn_salva_distinta.show();
        btn_calcola_pianteImpianto.show();
        // btnSalva.hide();
    } else {
        btn_salva_distinta.hide();
        btn_calcola_pianteImpianto.hide();
    }
}
/////////////////

/////////////////
function inizializzaKendoCodiciDistinte(idDiv) {
    if (gridCodici !== undefined) {
        gridCodici.destroy();
    }
    var funzioniCRUD = {
        funzioneRead: dataGrigliaCodiciDistinte,
        funzioneInsert: CodiceDistinta,
        funzioneUpdate: CodiceDistinta,
        funzioneDelete: CodiceDistinta
    };

    var idModel = "id_cod";
    var campiKendoModel = modelGrigliaCodiciDistinte();
    var colonneKendoGrid = colonneGrigliaCodiciDistinte();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var template = kendo.template($("#popupCodici_Template").html());
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        editable: {
            mode: "popup",
            template: template,
            window: {
                title: TraduzioneMultiResx(impiantoEditResx, "CodiciDistinta", "Codici Distinta")
            }
        },
        cancel: function () {
            $("#" + IDControllo).data('kendoGrid').refresh();
        },
        colonneCustomKendoGrid: [
            {
                command: [{
                    name: "edit",
                    text: {
                        edit: "",
                        update: TraduzioneMultiResx(impiantoEditResx, "ConfermaDati", "Conferma Dati"),
                        cancel: TraduzioneMultiResx(impiantoEditResx, "Annulla", "Annulla")
                    }
                },
                {
                    name: "destroy",
                    text: "",
                    className: "k-custom-delete"//, iconClass: "fa fa-trash-o"
                }
                ],
                title: TraduzioneMultiResx(impiantoEditResx, "Operazioni", "Operazioni"),
                width: "80px"
            }
        ]
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundCodiciDistinte,
        funzioneDaChiamareDopoEdit: CodiciDistinta_onEdit,
        funzioneDaChiamareDopoAnnulla: onAnnullaCodiciDistinta
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
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
    gridCodici = $("#" + idDiv).data("kendoGrid");
}

function onAnnullaCodiciDistinta() {
    setTimeout(function () { gridCodici.refresh(); }, 0);
}

function CodiciDistinta_onEdit(e) {
    inizializzaCmb_Codici(e);

    if (!e.model.isNew()) {
        Cmb_Codici.enable(false);
    }

}

function dataGrigliaCodiciDistinte(options) {
    var a = JSON.parse(obj_Distinta_Selezionata.obj_Codici_Distinta);
    options.success(a);
}

function modelGrigliaCodiciDistinte(options) {
    var a = {
        "id_cod": {
            "editable": true,
            "type": "number"
        },
        "descrizione": {
            "editable": true,
            "type": "string"
        },
        "val_cod": {
            "editable": true,
            "type": "string"
        }
    };
    return a;
}

function colonneGrigliaCodiciDistinte(options) {
    var a = [{
        "field": "descrizione",
        "title": TraduzioneMultiResx(impiantoEditResx, "Descrizione", "Descrizione")
    },
    {
        "field": "val_cod",
        "title": TraduzioneMultiResx(impiantoEditResx, "Valore", "Valore")
    }];
    return a;
}

function CodiceDistinta(e) {
    let data = e.data.models;
    let id_codArr = new Array();
    let sameID = false;
    for (let i = 0; i < data.length; i++) {
        if (id_codArr.includes(data[i].id_cod)) {
            sameID = true;
        } else {
            id_codArr.push(data[i].id_cod);
        }
    }
    if (sameID) {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpossibileInserireDueCodiciUguali", "Non è possibile inserire 2 codici uguali."));
    } else {
        gridCodici.refresh();
        obj_Distinta_Selezionata.obj_Codici_Distinta = JSON.stringify(gridCodici.dataSource.data());
    }
}

function App_onDataBoundCodiciDistinte() {

}
/////////////////

/////////////////
function inizializzaKendoParticelleDistinte(idDiv) {
    if (gridParticelle !== undefined) {
        gridParticelle.destroy();
    }
    var funzioniCRUD = {
        funzioneRead: dataGrigliaParticelleDistinte,
        funzioneInsert: ParticellaDistinta,
        funzioneUpdate: ParticellaDistinta,
        funzioneDelete: ParticellaDistinta
    };

    var idModel = "id_cod";
    var campiKendoModel = modelGrigliaParticelleDistinte();
    var colonneKendoGrid = colonneGrigliaParticelleDistinte();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var template = kendo.template($("#popupParticelle_Template").html());
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        editable: {
            mode: "popup",
            template: template,
            window: {
                title: "Particelle Distinta"
            }
        },
        cancel: function () {
            $("#" + IDControllo).data('kendoGrid').refresh();
        },
        colonneCustomKendoGrid: [
            {
                command: [{
                    name: "edit",
                    text: {
                        edit: "",
                        update: "Conferma Dati",
                        cancel: "Annulla"
                    }
                },
                {
                    name: "destroy",
                    text: "",
                    className: "k-custom-delete"//, iconClass: "fa fa-trash-o"
                }
                ],
                title: TraduzioneMultiResx(impiantoEditResx, "Operazioni", "Operazioni"),
                width: "80px"
            }
        ]
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: App_onDataBoundParticelleDistinte,
        funzioneDaChiamareDopoEdit: CodiciParticelle_onEdit,
        funzioneDaChiamareDopoAnnulla: onAnnullaParticelleDistinta
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
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
    gridParticelle = $("#" + idDiv).data("kendoGrid");
}

function onAnnullaParticelleDistinta() {
    setTimeout(function () { gridParticelle.refresh(); }, 0);
}

function CodiciParticelle_onEdit(e) {
    inizializzaCmb_Particelle(e);
    inizializzaCmb_CodiciParticelle(e);

    if (!e.model.isNew()) {
        Cmb_Particelle.enable(false);
        Cmb_CodiciParticelle.enable(false);
    }

}

function dataGrigliaParticelleDistinte(options) {
    var a = JSON.parse(obj_Distinta_Selezionata.obj_Particelle_Distinta);
    options.success(a);
}

function modelGrigliaParticelleDistinte(options) {
    var a = {
        "testoProv": {
            "editable": true,
            "type": "string"
        },
        "valoreProv": {
            "editable": true,
            "type": "string"
        },
        "id_cod": {
            "editable": true,
            "type": "number"
        },
        "id_des": {
            "editable": true,
            "type": "string"
        },
        "val_cod": {
            "editable": true,
            "type": "string"
        }
    };
    return a;
}

function colonneGrigliaParticelleDistinte(options) {
    var a = [
        {
            "field": "testoProv",
            "title": TraduzioneMultiResx(impiantoEditResx, "Catasto", "Catasto")
        },
        {
            "field": "id_des",
            "title": TraduzioneMultiResx(impiantoEditResx, "Descrizione", "Descrizione")
        },
        {
            "field": "val_cod",
            "title": TraduzioneMultiResx(impiantoEditResx, "Valore", "Valore")
        }];
    return a;
}

function ParticellaDistinta(e) {
    let data = e.data.models;
    let id_codArr = new Array();
    let sameID = false;
    for (let i = 0; i < data.length; i++) {
        if (id_codArr.includes(data[i].id_cod + "_" + data[i].valoreProv)) {
            sameID = true;
        } else {
            id_codArr.push(data[i].id_cod + "_" + data[i].valoreProv);
        }
    }
    if (sameID) {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpossibileInserireDueCodiciUgualiPerLaStessaParticella",
            "Non è possibile inserire 2 codici uguali per la stessa particella."));
    } else {
        gridParticelle.refresh();
        obj_Distinta_Selezionata.obj_Particelle_Distinta = JSON.stringify(gridParticelle.dataSource.data());
    }
}

function App_onDataBoundParticelleDistinte() {

}
/////////////////

/////////////////////////////


/////////////////////FUNZIONI COMUNI/////////////////////
function checkVal(value, checkValue) {
    if (value === checkValue) {
        return "";
    } else {
        return value;
    }
}

function piante_calcola(dist_su, dist_tra, interb, germin) {
    let flag = true;
    // Controllo se i campi richiesti sono stati riempiti
    if (dist_su == 0) {
        flag = false;
    }

    if (dist_tra == 0) {
        flag = false;
    }

    if ($('#ChkFilaBinata').prop("checked")) {
        if (interb == 0) {
            flag = false;
        }
    }

    //if (germin == 0) {
    //    flag = false;
    //}
    ////

    if (flag) {
        var denominatore;

        if ($('#ChkFilaBinata').prop("checked")) {
            denominatore = Math.abs(dist_tra - interb) * dist_su;
            // denominatore = (interb / 2) * dist_su;
        } else {
            denominatore = dist_su * dist_tra;

        }
        var PianteHa = 0;
        if (germin != 0 && germin != undefined && germin != null) {
            PianteHa = 10000 / denominatore * (germin / 100);
        } else {
            PianteHa = 10000 / denominatore * (100);
        }

        var superficie = TxtSuperficie.value();

        var PianteImpianto = PianteHa * superficie;

        TxtPianteHa.value(parseInt(PianteHa));
        TxtPianteHa.trigger('change');

        TxtPianteImpianto.value(parseInt(PianteImpianto));
        TxtPianteImpianto.trigger('change');

    }
    else {

        TxtPianteImpianto.value(undefined);
        TxtPianteImpianto.trigger('change');

        TxtPianteHa.value(undefined);
        TxtPianteHa.trigger('change');

    }

}

function ValidaxDistinta() {
    let val_inizio = kendo.parseDate(obj_Distinta_Selezionata.Validita_Inizio);
    let val_fine = kendo.parseDate(obj_Distinta_Selezionata.Validita_Fine);
    if (val_inizio == undefined || val_fine == undefined) {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "InserireDateDiInizioEFineValide", "Inserire date di Inizio e Fine valide."));
        return false;
    }

    if (val_inizio >= val_fine) {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataFineEsercizioDeveEssereSuperioreDataInizio",
            "La data di fine esercizio deve essere superiore alla data di inizio."));
        return false;
    }

    let val_inizio_imp = kendo.parseDate(obj_Impianto.impianto_data_inizio);
    if (val_inizio !== null && val_inizio_imp !== null) {
        if (val_inizio < val_inizio_imp) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataInizioDistintaInferioreDataInizioImpianto_",
                "La data di inizio della distinta non può essere inferiore alla data di inizio dell'impianto: ") + val_inizio_imp.toLocaleDateString());
            return false;
        }
    }

    let val_fine_imp = kendo.parseDate(obj_Impianto.impianto_data_fine);
    if (val_fine !== null && val_fine_imp !== null) {
        if (val_fine > val_fine_imp) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DataFineDistintaSuperioreDataFineImpianto_",
                "La data di fine della distinta non può essere superiore alla data di fine dell'impianto: ") + val_fine_imp.toLocaleDateString());
            return false;
        }
    }

    // se attivo algoritmo codifica il lotto non è obbligatorio
    if (algoritmoCodifica === "") {
        let lotto = $("#Txt_Lotto").val();
        if (lotto === "" || lotto === undefined || lotto === null) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "InserireUnLotto", "Inserire un lotto."));
            return false;
        }
    }

    if (salvaDistinta()) {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "EsercizioImpostatoCorrettamenteSalvareImpiantoPerConsolidare",
            "Esercizio impostato correttamente. Salvare l'impianto per rendere effettive le modifiche effettuate."));
        btn_salva_distinta.hide();
        btnSalva.show();
        var objDistinte = JSON.parse($("#kendoDistinte").val());
        objDistinte.kendo_rows = $.extend({}, gridDistinte.dataSource.data());
        $("#kendoDistinte").val(JSON.stringify(objDistinte));
        obj_Impianto.dati_Distinte = $.extend({}, objDistinte);
        gridDistinte.destroy();
        setTimeout(function () {
            inizializzaKendoDistinte('tabImpianti');
        }, 0)
        return true;
    } else {
        //kendo.alert("Si sono verificati degli errori durante il salvataggio della distinta. ");
        return false;
    }

}

function salvaDistinta() {
    let salva = false;
    let trovata = false;
    var data = gridDistinte.dataSource.data();

    for (let i = 0; i < data.length; i++) {
        if (data[i].uid == obj_Distinta_Selezionata.uid) {
            data[i] = $.extend({}, obj_Distinta_Selezionata);
            trovata = true;
        }
    }

    // if (!trovata) {

    let dateSovrapposte = false;
    let data_i = kendo.parseDate(obj_Distinta_Selezionata.Validita_Inizio);
    let data_f = kendo.parseDate(obj_Distinta_Selezionata.Validita_Fine);

    for (let i = 0; i < data.length; i++) {
        if (obj_Distinta_Selezionata.uid !== data[i].uid) {
            let dataInizioDB = kendo.parseDate(data[i].Validita_Inizio);
            let dataFineDB = kendo.parseDate(data[i].Validita_Fine);
            if (data_i >= dataInizioDB && data_i <= dataFineDB) {
                dateSovrapposte = true;
            }
            if (data_f >= dataInizioDB && data_f <= dataFineDB) {
                dateSovrapposte = true;
            }
        }
    }

    if (!dateSovrapposte) {
        if (!trovata) data.push(obj_Distinta_Selezionata);
        salva = true;
    } else {
        kendo.alert(TraduzioneMultiResx(impiantoEditResx, "DateSelezionateSiSovrappongonoAdEserciziPrecedenti",
            "Le date selezionate si sovrappongono ad esercizi precedenti"));
        return false
    }

    if (obj_Distinta_Selezionata.Progetto_Cod != 0) {
        //Check CdG e Movimenti 
        var prosegui = WS_Controlla_DatixModificaDistinta(kendo.stringify(obj_Distinta_Selezionata));
        switch (prosegui.TipoMessaggioRitorno) {
            case "alert":
                WaitFrame.hide();
                kendo.alert(prosegui.Messaggio);
                return false;
            case "procedi":
                salva = true;
                break;
        }
    }

    return salva;
}

function ValidaxSubmit(Tipo_Salva) {
    if (operazione_DB === 1 || operazione_DB === 2) {
        if ((obj_Impianto.id_cod_terreno === 0 || obj_Impianto.id_cod_terreno === "") && (obj_Impianto.veg_cod === "" || obj_Impianto.veg_cod === "0")) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpostareSpecieVegetaleODestinazioneDUso", "Impostare la specie vegetale o la destinazione d'uso"));
            return false;
        }
        if (obj_Impianto.veg_cod !== 0) {
            if (obj_Impianto.cul_cod === "") {
                kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpostareVarietà", "Impostare varietà."));
                return false;
            }
            if (obj_Impianto.grfi_cod === "") {
                kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpostareFinalità", "Impostare finalità."));
                return false;
            }
        }
        if (obj_Impianto.sup_imp === 0) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "LaSuperficieNettaColtivataDeveEssereMaggioreDiZero",
                "La superficie netta coltivata deve essere maggiore di 0."));
            return false;
        }


        if (kendo.parseDate(obj_Impianto.impianto_data_inizio).toString() === DEFAULTDATE.toString()) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpostareValiditàInizioImpianto", "Impostare validità inizio impianto."));
            return false;
        }

        if (kendo.parseDate(obj_Impianto.impianto_data_fine).toString() === DEFAULTDATE.toString()) {
            kendo.alert(TraduzioneMultiResx(impiantoEditResx, "ImpostareValiditàFineImpianto", "Impostare validità fine impianto."));
            return false;
        }
        /*
        try {
            if (JSON.parse(obj_Impianto.dati_Distinte).kendo_rows.length === 0) {
                kendo.alert("Inserire almeno un esercizio di produzione.");
                return false;
            }
        } catch (e) {
            if (obj_Impianto.dati_Distinte.kendo_rows.length === 0) {
                kendo.alert("Inserire almeno un esercizio di produzione.");
                return false;
            }
        }
        */
        if (obj_Impianto.provenienzaseme === null) {
            obj_Impianto.provenienzaseme = 0;
        }
        if (obj_Impianto.cop_cod === null) {
            obj_Impianto.cop_cod = 0;
        }
        if (obj_Impianto.grva_cod === undefined) {
            obj_Impianto.grva_cod = 0;
        }
        if (obj_Impianto.setup_cod === undefined) {
            obj_Impianto.setup_cod = "";
        }
        if (obj_Impianto.ChkCoverCrops === undefined) {
            obj_Impianto.ChkCoverCrops = "";
        }
        if (obj_Impianto.ChkMonitorato === undefined) {
            obj_Impianto.ChkMonitorato = "";
        }
        if (obj_Impianto.codiceZona === undefined) {
            obj_Impianto.codiceZona = "";
        }
        // - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        if (obj_Impianto.ChkMaschiSesto === undefined) {
            obj_Impianto.ChkMaschiSesto = "";
        }
        //  - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        if (obj_Impianto.Resa_Prevista === undefined) {
            obj_Impianto.Resa_Prevista = "";
        }
        if (obj_Impianto.cop_data_inizio === undefined) {
            obj_Impianto.cop_data_inizio = "";
        }
        if (obj_Impianto.cop_data_fine === undefined) {
            obj_Impianto.cop_data_fine = "";
        }
        if (obj_Impianto.tra_fila_m === undefined) {
            obj_Impianto.tra_fila_m = "";
        }
        if (obj_Impianto.su_fila_m === undefined) {
            obj_Impianto.su_fila_m = "";
        }
        if (obj_Impianto.tecn_cod === undefined) {
            obj_Impianto.tecn_cod = "";
        }
        if (obj_Impianto.su_cod === undefined) {
            obj_Impianto.su_cod = "";
        }
        if (obj_Impianto.interbina === undefined) {
            obj_Impianto.interbina = "";
        }
        if (obj_Impianto.germinabilita === undefined) {
            obj_Impianto.germinabilita = "";
        }
        if (obj_Impianto.dettaglio_varieta_personalizzato === undefined) {
            obj_Impianto.dettaglio_varieta_personalizzato = "";
        }
        if (obj_Impianto.impianto_ibrido === undefined) {
            obj_Impianto.impianto_ibrido = "";
        }
        if (obj_Impianto.Unita_Vitata === undefined) {
            obj_Impianto.Unita_Vitata = "";
        }
        if (obj_Impianto.CodBMBDBT_M === undefined) {
            obj_Impianto.CodBMBDBT_M = "";
        }
        if (obj_Impianto.CodBMBDBT_F === undefined) {
            obj_Impianto.CodBMBDBT_F = "";
        }
        if (obj_Impianto.Genetica_M === undefined) {
            obj_Impianto.Genetica_M = "";
        }
        if (obj_Impianto.Genetica_F === undefined) {
            obj_Impianto.Genetica_F = "";
        }
        if (obj_Impianto.OffType_M === undefined) {
            obj_Impianto.OffType_M = "";
        }
        if (obj_Impianto.DistanzaSuFila_F === undefined) {
            obj_Impianto.DistanzaSuFila_F = "";
        }
        if (obj_Impianto.DistanzaTraFila_F === undefined) {
            obj_Impianto.DistanzaTraFila_F = "";
        }
        if (obj_Impianto.chkConsociazione_checked === undefined) {
            obj_Impianto.chkConsociazione_checked = "";
        }
        if (obj_Impianto.id_consociazione === undefined) {
            obj_Impianto.id_consociazione = "";
        }
        if (obj_Impianto.codiceZona === undefined) {
            obj_Impianto.codiceZona = "";
        }
    }
    let obj_impianto_str = kendo.stringify(obj_Impianto);

    // controllo su salvataggio dati
    if (btn_salva_distinta.is(":visible")) {
        kendo.confirm(TraduzioneMultiResx(impiantoEditResx, "ConfermaContinuareOperazioneCausaPerditaModificheEsercizio",
            "L'esercizio non è stato salvato, eventuali modifiche potrebbero andare perse. Continuo lo stesso?")).then(function () {
                WaitFrame.show();
                checkControlla_Dati(obj_impianto_str, false, false, Tipo_Salva);
            }, function () { });
    } else {
        WaitFrame.show();
        checkControlla_Dati(obj_impianto_str, false, false, Tipo_Salva);
    }

}

async function checkControlla_Dati(obj_impianto_str, saltaPrimoControllo, saltaSecondoControllo, Tipo_Salva) {

    let obj_risposta = await WS_Controlla_Dati(obj_impianto_str, saltaPrimoControllo, saltaSecondoControllo);
    obj_Impianto.id_consociazione = obj_risposta.Id_Consociazione;

    let Lav_Cod = 0
    if (Cmb_Operazioni !== undefined && Tipo_Salva == "2") {
        Lav_Cod = Cmb_Operazioni.value();
    }

    switch (obj_risposta.TipoMessaggioRitorno) {
        case "alert":
            WaitFrame.hide();
            kendo.alert(obj_risposta.Messaggio);
            break;
        case "conferma":
            WaitFrame.hide();
            kendo.confirm(obj_risposta.Messaggio).then(function () {
                if (obj_risposta.TipoControllo == 1) {
                    checkControlla_Dati(obj_impianto_str, true, false);
                } else {
                    scrivi(Tipo_Salva, Lav_Cod);
                }
            }, function () {

            });
            break;
        case "salva":
            if (obj_risposta.ProseguiSalvataggio === "true") {                
                var data = gridDistinte.dataSource.data();
                let procedi = true;
                if (data.length == 1) {
                    if (obj_Impianto.impianto_data_inizio > data[0].Validita_Inizio) {
                        data[0].Validita_Inizio = obj_Impianto.impianto_data_inizio
                    }
                    if (obj_Impianto.impianto_data_fine < data[0].Validita_Fine ) {
                        data[0].Validita_Fine = obj_Impianto.impianto_data_fine
                    }

                    ////aggiorno i dati
                    let objDistinte = JSON.parse($("#kendoDistinte").val());
                    objDistinte.kendo_rows = $.extend({}, data);
                    $("#kendoDistinte").val(JSON.stringify(objDistinte));
                    obj_Impianto.dati_Distinte = $.extend({}, objDistinte);

                } else {
                    for (let i = 0; i < data.length; i++) {
                        if (kendo.parseDate(obj_Impianto.impianto_data_inizio) > kendo.parseDate(data[i].Validita_Inizio)) {
                            kendo.alert("Non è possibile modificare la data inizio dell'impianto, perchè uno o più esercizi risultano ancora aperti in data antecedente al " + obj_Impianto.impianto_data_inizio);
                            procedi = false;
                            break;
                        }
                        if (kendo.parseDate(obj_Impianto.impianto_data_fine) < kendo.parseDate(data[i].Validita_Fine)) {
                            WaitFrame.hide();
                            kendo.alert("Non è possibile modificare la data fine dell'impianto, perchè uno o più esercizi risultano ancora aperti in data successiva al " + obj_Impianto.impianto_data_fine);
                            procedi = false;
                            break;
                        }
                    }
                }

                if (procedi == true)
                    scrivi(Tipo_Salva, Lav_Cod);
            }
            break;

    }

}

function scrivi(Tipo_Salva, Lav_Cod) {
    let obj_impianto_str = kendo.stringify(obj_Impianto);

    if (Tipo_Salva == undefined) {
        Tipo_Salva = "1"
    }

    if (Lav_Cod == undefined) {
        Lav_Cod = 0
    }

    let visibilita = 1;

    var parametri = kendo.stringify({
        obj_Impianto_str: obj_impianto_str,
        Tipo_Salva: Tipo_Salva,
        Lav_Cod: Lav_Cod,
        visibilita: visibilita
    });

    ajaxAgronica("Impianto_Edit2.aspx/Salva_tutto",
        parametri,
        function (risposta) {
            kendo.alert(risposta.RispostaStringa);
            window.location = risposta.ParametroDue_stringa;
        }, null);

}
/////////////////////////////

/////////////////////INIZIALIZZA COMBO/////////////////////

///////////////////////////////////
function inizializzaCmb_Specie(value) {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        if ($("#Cmb_Specie").data("kendoDropDownList") === undefined) {
            Cmb_Specie = $("#Cmb_Specie").kendoDropDownList({
                filter: "contains",
                autoBind: true,
                dataTextField: "veg_des",
                dataValueField: "veg_cod",
                dataSource: { transport: { read: RiempiCmb_Specie } },
                //open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    kendoDropDownAdjustWidth(e);
                    if (onLoad) {
                        this.value(obj_Impianto.veg_cod);
                        this.trigger("change");
                        onLoad = false;
                    }

                },
                change: function (e) {
                    if (obj_Impianto.veg_cod !== this.value() && this.value() !== "") {
                        WaitFrame.show();
                        obj_Impianto.veg_cod = this.value();
                        obj_Impianto.id_cod_terreno = 0;
                        let promises = new Array();
                        promises.push(inizializzaCmb_Finalita(obj_Impianto.grfi_cod));
                        promises.push(inizializzaCmb_Cultivar(obj_Impianto.cul_cod));
                        if (obj_Impianto.veg_cod !== null && obj_Impianto.veg_cod !== "") {
                            if (obj_Impianto.gru_cod === undefined || obj_Impianto.gru_cod === null) {
                                promises.push(imposta_gru_cod(obj_Impianto.veg_cod));
                            }
                        }
                        promises.push(inizializzaCmb_TipologiaVarietale());
                        if (dati_accessori_Loaded) {
                            promises.push(inizializzaCmb_Copertura());
                            promises.push(inizializzaCmb_ImpIrrigazione());
                            promises.push(inizializzaCmb_FormaAllevamento());
                            promises.push(inizializzaCmb_Portinnesto());
                            promises.push(inizializzaCmb_SeminaTrapianto());
                            promises.push(inizializzaCmb_ProvenienzaSeme());
                            promises.push(inizializzaCmb_DettaglioVarietaPersonalizzato());
                            promises.push(inizializzaCmb_CodiceZona());
                            promises.push(inizializzaCmb_ConduzioneTra());
                            promises.push(inizializzaCmb_ConduzioneSu());
                        }

                        if (obj_Impianto.veg_cod == 46) {
                            $("#pannelloTuberi").show();
                            promises.push(inizializzaCmb_TagliatoTuberi());
                        } else {
                            $("#pannelloTuberi").hide();
                            obj_Impianto.tagliato_tuberi = null;
                            obj_Impianto.partiTuberi = null;
                            if (Txt_PartiTuberi !== undefined) {
                                Txt_PartiTuberi.value("");
                                Txt_PartiTuberi.trigger("change");
                            }
                        }

                        Promise.all(promises).then(value => {
                            WaitFrame.hide();
                            resolve();
                        }, reason => {
                            kendo.alert(reason);
                            reject();
                        });
                    } else {
                        resolve();
                    }
                }
            }).data("kendoDropDownList");
        } else {
            resolve();
        }

    });
}

function inizializzaCmb_Finalita(value) {
    return new Promise((resolve, reject) => {
        Cmb_Finalita = $("#Cmb_Finalita").kendoDropDownList({
            autoBind: true,
            dataTextField: "grfi_des",
            dataValueField: "grfi_cod",
            dataSource: { transport: { read: CaricaComboFinalita } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Impianto.grfi_cod === 0 || obj_Impianto.grfi_cod === undefined || obj_Impianto.grfi_cod === null || obj_Impianto.grfi_cod === "") {
                    if (defaultFinalita != "") this.value(defaultFinalita); else this.select(0);
                } else {
                    this.value(obj_Impianto.grfi_cod);
                }
                // imposto default finalità se capo vuoto
                if (this.value() == "" && defaultFinalita != "") this.value(defaultFinalita);
                this.trigger("change");
            },
            change: function (e) {
                obj_Impianto.grfi_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Cultivar(value) {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Cultivar = $("#Cmb_Cultivar").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "cul_des",
            dataValueField: "cul_cod",
            dataSource: { transport: { read: CaricaComboCultivar_conFiltroUtente } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    if (obj_Impianto.cul_cod === 0 || obj_Impianto.cul_cod === undefined || obj_Impianto.cul_cod === null || obj_Impianto.cul_cod === "") {
                        this.select(0);
                    } else {
                        this.value(obj_Impianto.cul_cod);
                    }
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                obj_Impianto.cul_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_CodiciTerreno(value) {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_CodiciTerreno = $("#Cmb_CodiciTerreno").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "descrizione",
            dataValueField: "codice",
            dataSource: { transport: { read: RiempiCmb_CodiciTerreno } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value(obj_Impianto.id_cod_terreno);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: async function (e) {
                obj_Impianto.id_cod_terreno = this.value();
                obj_Impianto.veg_cod = 0;
                obj_Impianto.cul_cod = 0;
                obj_Impianto.grfi_cod = 0;
                await inizializzaCmb_Finalita(obj_Impianto.grfi_cod);
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

///////////////////////////////////
function imposta_gru_cod(veg_cod) {
    return new Promise((resolve, reject) => {
        get_gru_cod(veg_cod, function (gru_cod) {
            obj_Impianto.gru_cod = gru_cod;
            resolve();
        });
    });
}

function inizializzaCmb_TipologiaVarietale() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_TipologiaVarietale = $("#Cmb_TipologiaVarietale").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Grva_Des",
            dataValueField: "Grva_Cod",
            dataSource: { transport: { read: CaricaComboTipologiaVarietale } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value(obj_Impianto.grva_cod);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                obj_Impianto.grva_cod = this.value();
                if (obj_Impianto.grva_cod < 0) {
                    $("#ChkVarietaIbrida").prop("checked", true);
                    $("#ChkVarietaIbrida").trigger("change");
                } else if (obj_Impianto.grva_cod > 0) {
                    $("#ChkVarietaIbrida").prop("checked", false);
                    $("#ChkVarietaIbrida").trigger("change");
                } else {
                    $("#ChkVarietaIbrida").prop("checked", false);
                    $("#ChkVarietaIbrida").trigger("change");
                }
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Copertura() {
    return new Promise((resolve, reject) => {
        Cmb_Copertura = $("#Cmb_Copertura").kendoDropDownList({
            autoBind: true,
            dataTextField: "Cop_Des",
            dataValueField: "Cop_Cod",
            dataSource: { transport: { read: CaricaComboCmb_Copertura } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Impianto.cop_cod === 0 || obj_Impianto.cop_cod === undefined || obj_Impianto.cop_cod === null || obj_Impianto.cop_cod === "") {
                    this.select(0);
                } else {
                    this.value(obj_Impianto.cop_cod);
                }
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.cop_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_ImpIrrigazione() {
    return new Promise((resolve, reject) => {
        Cmb_ImpIrrigazione = $("#Cmb_ImpIrrigazione").kendoDropDownList({
            autoBind: true,
            dataTextField: "Imp_Des",
            dataValueField: "Imp_Cod",
            dataSource: { transport: { read: CaricaComboCmb_ImpIrrigazione } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.imp_cod2);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.imp_cod2 = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_FormaAllevamento() {
    return new Promise((resolve, reject) => {
        Cmb_FormaAllevamento = $("#Cmb_FormaAllevamento").kendoDropDownList({
            autoBind: true,
            dataTextField: "Foral_Des",
            dataValueField: "Foral_Cod",
            dataSource: { transport: { read: CaricaComboCmb_FormaAllevamento } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.foral_cod);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.foral_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Portinnesto() {
    return new Promise((resolve, reject) => {
        Cmb_Portinnesto = $("#Cmb_Portinnesto").kendoDropDownList({
            autoBind: true,
            dataTextField: "Port_Des",
            dataValueField: "Port_Cod",
            dataSource: { transport: { read: CaricaComboCmb_Portinnesto } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.port_cod);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.port_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_SeminaTrapianto() {
    return new Promise((resolve, reject) => {
        var seminaTrapianto;
        if (obj_Impianto.veg_cod !== 0) {
            seminaTrapianto = [
                { seminaTrapianto_Cod: "-1", seminaTrapianto_Des: "" },
                { seminaTrapianto_Cod: "Trapiantato", seminaTrapianto_Des: TraduzioneMultiResx(impiantoEditResx, "Trapiantato", "Trapiantato") },
                { seminaTrapianto_Cod: "Seminato", seminaTrapianto_Des: TraduzioneMultiResx(impiantoEditResx, "Seminato", "Seminato") }
            ];
        } else {
            seminaTrapianto = [];
        }


        Cmb_SeminaTrapianto = $("#Cmb_SeminaTrapianto").kendoDropDownList({
            autoBind: true,
            dataTextField: "seminaTrapianto_Des",
            dataValueField: "seminaTrapianto_Cod",
            dataSource: seminaTrapianto,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.setup_cod);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.setup_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_ProvenienzaSeme() {
    return new Promise((resolve, reject) => {

        var provenienzaSeme;
        if (obj_Impianto.veg_cod !== 0) {
            provenienzaSeme = [
                { provenienzaSeme_Cod: "0", provenienzaSeme_Des: "" },
                { provenienzaSeme_Cod: "1", provenienzaSeme_Des: TraduzioneMultiResx(impiantoEditResx, "Biologica", "Biologica") },
                { provenienzaSeme_Cod: "2", provenienzaSeme_Des: TraduzioneMultiResx(impiantoEditResx, "Convenzionale", "Convenzionale") },
                { provenienzaSeme_Cod: "2", provenienzaSeme_Des: TraduzioneMultiResx(impiantoEditResx, "Deroga", "Deroga") }
            ];
        } else {
            provenienzaSeme = [];
        }

        Cmb_ProvenienzaSeme = $("#Cmb_ProvenienzaSeme").kendoDropDownList({
            autoBind: true,
            dataTextField: "provenienzaSeme_Des",
            dataValueField: "provenienzaSeme_Cod",
            dataSource: provenienzaSeme,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.provenienzaseme);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.provenienzaseme = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_DettaglioVarietaPersonalizzato() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_DettaglioVarietaPersonalizzato = $("#Cmb_DettaglioVarietaPersonalizzato").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "InfoAgg_Des",
            dataValueField: "InfoAgg_Cod",
            dataSource: { transport: { read: CaricaComboCmb_DettaglioVarietaPersonalizzato } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value(obj_Impianto.dettaglio_varieta_personalizzato);
                    this.trigger("change");
                    onLoad = false;
                }

            },
            change: function (e) {
                obj_Impianto.dettaglio_varieta_personalizzato = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_CodiceZona() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_CodiceZona = $("#Cmb_CodiceZona").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "pro_des",
            dataValueField: "pro_cod",
            dataSource: { transport: { read: CaricaComboCmb_CodiceZona } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value(obj_Impianto.codiceZona);
                    this.trigger("change");
                    onLoad = false;
                }

            },
            change: function (e) {
                obj_Impianto.codiceZona = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_ConduzioneSu() {
    return new Promise((resolve, reject) => {

        Cmb_ConduzioneSu = $("#Cmb_ConduzioneSu").kendoDropDownList({
            autoBind: true,
            dataTextField: "Tecn_Des",
            dataValueField: "Tecn_Cod",
            dataSource: { transport: { read: CaricaComboCmb_ConduzioneSu } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.su_cod);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.su_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_ConduzioneTra() {
    return new Promise((resolve, reject) => {
        Cmb_ConduzioneTra = $("#Cmb_ConduzioneTra").kendoDropDownList({
            autoBind: true,
            dataTextField: "Tecn_Des",
            dataValueField: "Tecn_Cod",
            dataSource: { transport: { read: CaricaComboCmb_ConduzioneTra } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.tecn_cod);
                this.trigger("change");

            },
            change: function (e) {
                obj_Impianto.tecn_cod = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_TagliatoTuberi() {
    return new Promise((resolve, reject) => {
        var tagliatoTuberi = [
            { value: "", text: "" },
            { value: "T", text: TraduzioneMultiResx(impiantoEditResx, "Tagliato", "Tagliato") },
            { value: "I", text: TraduzioneMultiResx(impiantoEditResx, "Intero", "Intero") }
        ];

        Cmb_TagliatoTuberi = $("#Cmb_TagliatoTuberi").kendoDropDownList({
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: tagliatoTuberi,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                this.value(obj_Impianto.tagliato_tuberi);
                this.trigger("change");
            },
            change: function (e) {
                obj_Impianto.tagliato_tuberi = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

///////////////////////////////////
function inizializzaCmb_Regolamento() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Regolamento = $("#Cmb_Regolamento").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Reg_Des",
            dataValueField: "Reg_Cod",
            dataSource: { transport: { read: CaricaComboCmb_Regolamento } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Cod !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Regolamento_Cod);
                        if (this.value() !== "") {
                            this.trigger("change");
                        }
                        onLoad = false;
                    }
                } else {
                    if (defaultRegolamento != "") this.value(defaultRegolamento); else this.value(1);
                    this.trigger("change");
                }

            },
            change: function (e) {
                if (obj_Distinta_Selezionata !== undefined) {
                    obj_Distinta_Selezionata.Regolamento_Cod = this.value();
                    inizializzaCmb_Disciplinare();
                    inizializzaCmb_RegolamentoConc();
                    resolve();
                } else {
                    resolve();
                }
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Disciplinare() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Disciplinare = $("#Cmb_Disciplinare").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: CaricaComboCmb_Disciplinare } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Disciplinare_Cod !== undefined) {
                        let chiaveDisciplinare = obj_Distinta_Selezionata.Disciplinare_Cod + '/' + obj_Distinta_Selezionata.Disciplinare_PubblicoPrivato;
                        let chiaveVera = "";
                        for (let i = 0; i < Cmb_Disciplinare.dataSource.data().length; i++) {
                            if (Cmb_Disciplinare.dataSource.data()[i].value.split("/").length > 2) {
                                var valArr = Cmb_Disciplinare.dataSource.data()[i].value.split("/");
                                var val = valArr[0] + "/" + valArr[1];
                                if (val === chiaveDisciplinare) {
                                    chiaveVera = Cmb_Disciplinare.dataSource.data()[i].value;
                                }
                            }
                        }
                        if (chiaveVera !== "") {
                            this.value(chiaveVera);
                            this.trigger("change");
                        }
                        resolve();
                    } else {
                        resolve();
                    }
                    onLoad = false;
                }


            },
            change: function (e) {
                let keyArr = this.value().split("/");
                if (keyArr.length == 1) {
                    obj_Distinta_Selezionata.Disciplinare_Cod = keyArr[0];
                    obj_Distinta_Selezionata.Disciplinare_PubblicoPrivato = 0;
                } else {
                    obj_Distinta_Selezionata.Disciplinare_Cod = keyArr[0];
                    obj_Distinta_Selezionata.Disciplinare_PubblicoPrivato = keyArr[1];
                    obj_Distinta_Selezionata.id_tr = keyArr[3];
                }
                if (obj_Permessi_IAF) {
                    inizializzaCmb_IAF();
                }
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_IAF() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        if (Cmb_IAF !== undefined) {
            Cmb_IAF.dataSource.read();
        } else {
            Cmb_IAF = $("#Cmb_IAF").kendoMultiSelect({
                filter: "contains",
                autoBind: true,
                dataTextField: "text",
                dataValueField: "value",
                autoClose: false,
                dataSource: { transport: { read: CaricaComboCmb_IAF } },
                dataBound: function (e) {
                    if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Impianto_IAF_ImpegniAggiuntiviFacoltativi !== undefined) {
                        try {
                            this.value(JSON.parse(obj_Distinta_Selezionata.Impianto_IAF_ImpegniAggiuntiviFacoltativi));
                            this.trigger("change");
                        } catch (e) { }
                        resolve();
                    } else {
                        resolve();
                    }
                    onLoad = false;
                },
                change: function (e) {
                    obj_Distinta_Selezionata.Impianto_IAF_ImpegniAggiuntiviFacoltativi = this.value();
                }
            }).data("kendoMultiSelect");
        }
    });
}

function inizializzaCmb_CapitolatoPrivato() {
    let onLoad = true;
    return new Promise((resolve, reject) => {
        Cmb_CapitolatoPrivato = $("#Cmb_CapitolatoPrivato").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "InfoAgg_Des",
            dataValueField: "InfoAgg_Cod",
            dataSource: { transport: { read: CaricaComboCmb_CapitolatoPrivato } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Capitolato_Privato !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Capitolato_Privato);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }

            },
            change: function (e) {
                obj_Distinta_Selezionata.Capitolato_Privato = this.value();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_OrganismoReferente() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_OrganismoReferente = $("#Cmb_OrganismoReferente").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: CaricaComboCmb_OrganismoReferente } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Organismo_Referente !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Organismo_Referente);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }
            },
            change: function (e) {
                obj_Distinta_Selezionata.Organismo_Referente = this.value();
            }
        }).data("kendoDropDownList");
    });
}

// - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
function inizializzaCmb_LicenzaColtivazione() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_LicenzaColtivazione = $("#Cmb_LicenzaColtivazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "val_sigla",
            dataValueField: "val_cod",
            dataSource: { transport: { read: CaricaComboCmb_LicenzaColtivazione } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Licenza_Coltivazione !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Licenza_Coltivazione);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }
            },
            change: function (e) {
                obj_Distinta_Selezionata.Licenza_Coltivazione = this.value();
            }
        }).data("kendoDropDownList");
    });
}
// - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

function inizializzaCmb_Riferimento_Trasferimento_Dati() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Riferimento_Trasferimento_Dati = $("#Cmb_Riferimento_Trasferimento_Dati").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: CaricaComboCmb_Riferimento_Trasferimento_Dati } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Riferimento_Trasferimento_Dati !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Riferimento_Trasferimento_Dati);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }
            },
            change: function (e) {
                obj_Distinta_Selezionata.Riferimento_Trasferimento_Dati = this.value();
            }
        }).data("kendoDropDownList");
    });
}
function inizializzaCmb_MagazzinoConferimento() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_MagazzinoConferimento = $("#Cmb_MagazzinoConferimento").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: CaricaComboCmb_MagazzinoConferimento } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Magazzino_Conferimento !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Magazzino_Conferimento);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }
            },
            change: function (e) {
                obj_Distinta_Selezionata.Magazzino_Conferimento = this.value();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_RegolamentoConc() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_RegolamentoConc = $("#Cmb_RegolamentoConc").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Reg_Des",
            dataValueField: "Reg_Cod",
            dataSource: { transport: { read: CaricaComboCmb_RegolamentoConc } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Regolamento_Concimazione_Cod !== undefined) {
                    if (onLoad) {
                        this.value(obj_Distinta_Selezionata.Regolamento_Concimazione_Cod);
                        this.trigger("change");
                        onLoad = false;
                    }
                } else {
                    resolve();
                }
            },
            change: function (e) {
                obj_Distinta_Selezionata.Regolamento_Concimazione_Cod = this.value();
                if (!$("#Cmb_RegolamentoConc").prop("disabled")) calcolaNPK();
                inizializzaCmb_FinalitaConc();
                inizializzaCmb_Stato();
                resolve();
            }
        }).data("kendoDropDownList");
    });
}

function calcolaNPK() {
    var npkObject = impostaNPK(obj_Distinta_Selezionata.Regolamento_Concimazione_Cod, obj_Impianto.veg_cod, obj_Distinta_Selezionata.Finalita_Concimazione_Cod, obj_Distinta_Selezionata.stato_impianto);
    if (npkObject != null) {
        obj_Distinta_Selezionata.TxtN = npkObject.N;
        obj_Distinta_Selezionata.TxtP2O5 = npkObject.P;
        obj_Distinta_Selezionata.TxtK2O = npkObject.K;
    } else {
        obj_Distinta_Selezionata.TxtN = "";
        obj_Distinta_Selezionata.TxtP2O5 = "";
        obj_Distinta_Selezionata.TxtK2O = "";
    }
    TxtN.value(obj_Distinta_Selezionata.TxtN);
    TxtP2O5.value(obj_Distinta_Selezionata.TxtP2O5);
    TxtK2O.value(obj_Distinta_Selezionata.TxtK2O);
}

function inizializzaCmb_FinalitaConc() {
    return new Promise((resolve, reject) => {
        Cmb_FinalitaConc = $("#Cmb_FinalitaConc").kendoDropDownList({
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: { transport: { read: CaricaComboCmb_FinalitaConc } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Finalita_Concimazione_Cod !== undefined) {
                    this.value(obj_Distinta_Selezionata.Finalita_Concimazione_Cod);
                    this.trigger("change");
                } else {
                    resolve();
                }
            },
            change: function (e) {
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Finalita_Concimazione_Cod !== undefined) {
                    obj_Distinta_Selezionata.Finalita_Concimazione_Cod = this.value();
                    if (!$("#Cmb_FinalitaConc").prop("disabled")) calcolaNPK();
                    resolve();
                } else {
                    resolve();
                }
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Stato() {
    return new Promise((resolve, reject) => {
        Cmb_Stato = $("#Cmb_Stato").kendoDropDownList({
            autoBind: true,
            dataTextField: "grfi_des",
            dataValueField: "grfi_cod",
            dataSource: { transport: { read: CaricaComboCmb_Stato } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.stato_impianto !== undefined) {
                    this.value(obj_Distinta_Selezionata.stato_impianto);
                    this.trigger("change");
                } else {
                    resolve();
                }
            },
            change: function (e) {
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.stato_impianto !== undefined) {
                    if (this.value() !== "") {
                        obj_Distinta_Selezionata.stato_impianto = this.value();
                        if (!$("#Cmb_Stato").prop("disabled")) calcolaNPK();
                    }
                    resolve();
                } else {
                    resolve();
                }
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_PianoSemina() {
    return new Promise((resolve, reject) => {
        Cmb_PianoSemina = $("#Cmb_PianoSemina").kendoDropDownList({
            autoBind: true,
            dataTextField: "InfoAgg_Des",
            dataValueField: "InfoAgg_Cod",
            dataSource: { transport: { read: CaricaComboCmb_PianoSemina } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Impianto_PianoSemina !== undefined) {
                    this.value(obj_Distinta_Selezionata.Impianto_PianoSemina);
                    this.trigger("change");
                } else {
                    resolve();
                }
            },
            change: function (e) {
                if (obj_Distinta_Selezionata !== undefined && obj_Distinta_Selezionata.Impianto_PianoSemina !== undefined) {
                    if (this.value() !== "") {
                        obj_Distinta_Selezionata.Impianto_PianoSemina = this.value();
                    }
                    resolve();
                } else {
                    resolve();
                }
            }
        }).data("kendoDropDownList");
    });
}

////////////////////////////////
function inizializzaCmb_Codici(mod) {
    return new Promise((resolve, reject) => {
        let obj_codiciSpec = new Array();
        //if (mod.model.id_cod === 0) {
        //    let data = gridCodici.dataSource.data();
        //    let id_cod_delete = new Array();
        //    for (let i = 0; i < data.length; i++) {
        //        if (data[i].id_cod !== 0) {
        //            id_cod_delete.push(data[i].id_cod);
        //        }
        //    }
        //    //obj_codiciSpec = $.extend({}, obj_Codici);
        //    obj_codiciSpec = obj_Codici.slice();
        //    for (let i = 0; i < obj_Codici.length; i++) {
        //        if (id_cod_delete.includes(obj_Codici[i].value)) {
        //            obj_codiciSpec.splice(i, 1);
        //        }
        //    }

        //} else {
        //    obj_codiciSpec = obj_Codici;
        //}

        obj_codiciSpec = obj_Codici;
        Cmb_Codici = $("#Cmb_Codici").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: obj_codiciSpec,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                mod.model.id_cod = this.value();
                mod.model.descrizione = this.text();
            },
            change: function (e) {
                mod.model.id_cod = this.value();
                mod.model.descrizione = this.text();
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_Particelle(mod) {
    return new Promise((resolve, reject) => {
        let obj_particelleSpec = new Array();
        obj_particelleSpec = obj_Particelle;
        Cmb_Particelle = $("#Cmb_Particelle").kendoDropDownList({
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: obj_particelleSpec,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                mod.model.valoreProv = this.value();
                mod.model.testoProv = this.text();
                this.trigger("change");
            },
            change: function (e) {
                let val = this.value();
                let varArr = val.split("_");
                let prov = varArr[0];
                let com = varArr[1];
                let sezione = "";
                if (varArr[2] !== "") {
                    sezione = varArr[2];
                }
                let foglio = parseInt(varArr[3]);
                let numero = parseInt(varArr[4]);
                let subalterno = "";
                if (varArr[5] !== "") {
                    subalterno = varArr[5];
                }
                mod.model.valoreProv = val;
                mod.model.testoProv = this.text();
                mod.model.prov = prov;
                mod.model.com = com;
                mod.model.sezione = sezione;
                mod.model.foglio = foglio;
                mod.model.numero = numero;
                mod.model.subalterno = subalterno;

            }
        }).data("kendoDropDownList");
    });
}

function inizializzaCmb_CodiciParticelle(mod) {
    return new Promise((resolve, reject) => {
        let obj_codiciParticelleSpec = new Array();
        obj_codiciParticelleSpec = obj_CodiciParticelle;
        Cmb_CodiciParticelle = $("#Cmb_CodiciParticelle").kendoDropDownList({
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: obj_codiciParticelleSpec,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                mod.model.id_cod = this.value();
                mod.model.id_des = this.text();
            },
            change: function (e) {
                mod.model.id_cod = this.value();
                mod.model.id_des = this.text();
            }
        }).data("kendoDropDownList");
    });
}

async function GeneraProgressivo() {
    let resp = await WS_GeneraProgressivo();
    $("#Txt_Lotto").val(resp.RispostaStringa);
    $("#Txt_Lotto").attr("readonly", resp.RispostaStringa !== "");
    obj_Distinta_Selezionata.Progetto_Nome = $("#Txt_Lotto").val();
}

function GeneraDescrizione() {

    creaKendoDropDownList("descrizioneProdotto", { read: WS_LeggiDescrizioni }, "des", "val"); //.bind("change", CambiaDescrizione);

    /* $("#descrizioneProdotto").kendoDropDownList({
        dataTextField: "des",
        dataValueField: "val",
        dataSource: { transport: { read: WS_LeggiDescrizioni } },
        open: kendoDropDownAdjustWidth,
        dataBound: kendoDropDownAdjustWidth,
        change: function (e) {
        },
        optionLabel: "Seleziona"
    }); */

    $('#modalDescrizioneOP').modal('show');
}

function CambiaDescrizione() {
    var descrizione = KendoDDL("descrizioneProdotto").text();
    $("#Txt_Descrizione").val(descrizione);
    obj_Distinta_Selezionata.Progetto_Des = descrizione;
    // $("#Txt_Descrizione").attr("readonly", descrizione !== "");
    $('#modalDescrizioneOP').modal('hide');
}

function InserisciOperazione() {
    inizializzaWindowOperazioni();
    inizializzaCmb_Operazioni();
    windowOperazione.center();
    windowOperazione.open();
}

function inizializzaCmb_Operazioni() {
    return new Promise((resolve, reject) => {
        Cmb_Operazioni = $("#Cmb_Operazioni").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "lav_des",
            dataValueField: "lav_cod",
            dataSource: { transport: { read: CaricaComboCmb_Operazioni } },
            open: kendoDropDownAdjustWidth,
            dataBound: async function (e) {
                let op_Pred = await LeggiImpostazioniUtente(856);
                if (op_Pred !== "" && !isNaN(op_Pred)) {
                    this.value(op_Pred);
                }
                resolve();
            },
            change: function (e) {
                //Operazione_Sel = "0";
            }
        }).data("kendoDropDownList");
    });
}

function inizializzaWindowOperazioni() {
    if ($("#windowOperazione").data("kendoWindow") == undefined) {

        var windowOptions = {
            actions: ["Close"],
            draggable: false,
            resizable: false,
            width: "500px",
            title: "Seleziona Operazione"
        };

        windowOperazione = $("#windowOperazione").kendoWindow(windowOptions).data("kendoWindow");
    }
}


//Anna 16/05/22: aggiunto button per  [rif. chiamata 18136]
//nascosti campi Piante/HA e Piante/Impianto da DENSITA' IMPIANTO 
//(nell'esportazione degli impianti viene riportato quello della distinta e alcune aziende non lo compilavano) 
function calcola_pianteImpianto(e) {

    inizializzaCampi_DatiAccessori();

    let TxtPianteHa2 = $("#TxtPianteHa2").data("kendoNumericTextBox");
    let TxtPianteImpianto2 = $("#TxtPianteImpianto2").data("kendoNumericTextBox");

    let dist_su = $("#Txt_DistanzaSuFila_M").data("kendoNumericTextBox").value();
    let dist_tra = $("#Txt_DistanzaTraFila_M").data("kendoNumericTextBox").value();
    let interb = $("#Txt_Interbina").data("kendoNumericTextBox").value();
    let germin = $("#Txt_Germinabilita").data("kendoNumericTextBox").value();

    let flag = true;
    let messaggioErrore = "";

    // Controllo se i campi richiesti sono stati riempiti
    if (dist_su == 0 || dist_su == null) {
        flag = false;
    }

    if (dist_tra == 0 || dist_tra == null) {
        flag = false;
    }

    if ($('#ChkFilaBinata').prop("checked")) {
        if (interb == 0 || interb == null) {
            flag = false;
        }
    }

    if (flag) {
        var denominatore;

        if ($('#ChkFilaBinata').prop("checked")) {
            denominatore = Math.abs(dist_tra - interb) * dist_su;
            // denominatore = (interb / 2) * dist_su;
        } else {
            denominatore = dist_su * dist_tra;

        }
        var PianteHa = 0;

        /* Se la germinabilità viene rimossa o settata a zero, la considero comunque 100 */
        if (germin === 0 || germin === undefined || germin === null) {
            germin = 100;
        }

        if (germin != 0 && germin != undefined && germin != null) {
            PianteHa = 10000 / denominatore * (germin / 100);
        } else {
            PianteHa = 10000 / denominatore * (100);
        }

        var superficie = TxtSuperficie.value();

        var PianteImpianto = PianteHa * superficie;

        if (TxtPianteHa2.value() != 0 && TxtPianteHa2._oldText != Math.round(PianteHa)) {
            kendo.confirm(TraduzioneMultiResx(impiantoEditResx, "NumeroPianteCalcolatoDaSestoDifferisceDaNumeroAttualmenteInserito",
                "Il numero di piante calcolato in base al sesto d'impianto indicato differisce dal numero di piante attualmente inserito. Si desidera modificarlo?")).then(function () {
                    TxtPianteHa2.value(parseInt(PianteHa));
                    TxtPianteHa2.trigger('change');

                    TxtPianteImpianto2.trigger('change');
                }, function () { }
                );
        } else {
            TxtPianteHa2.value(parseInt(PianteHa));
            TxtPianteHa2.trigger('change');

            TxtPianteImpianto2.trigger('change');
        }
    }

    else {

        messaggioErrore = (TraduzioneMultiResx(impiantoEditResx, "MessaggioErroreCalcoloPianteDaSesto",
            "Per poter eseguire il calcolo è necessario compilare i campi: DISTANZA SU FILA [M], DISTANZA TRA FILA [M] e INTERBINA [M] (se Fila Binata)"))
        kendo.alert(messaggioErrore);

    }
}
