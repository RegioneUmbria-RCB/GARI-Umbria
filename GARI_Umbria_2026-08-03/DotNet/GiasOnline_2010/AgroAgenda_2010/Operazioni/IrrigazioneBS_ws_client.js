var indirizzohttp = "./IrrigazioneBS.aspx";

function Leggi_CentriAziendali(options)
{
    let param = kendo.stringify(
        {
            piva: $(cIdPiva).val()
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_CentriAziendali",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Leggi_TipoIrrigazione(options) {

    let param = kendo.stringify(
        {
            tipo_ddl_irr:"normale"
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_TipoIrrigazione",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Leggi_TipoIrrigazioneKendoGridImpianti(options) {

    let param = kendo.stringify(
        {
            tipo_ddl_irr: "grid_impianti"
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_TipoIrrigazione",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function Leggi_CambiaValoreTipoIrrigazioneKendoGridImpianti(options) {

    let param = kendo.stringify(
        {
            tipo_ddl_irr: "cambia_valore_grid_impianti"
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_TipoIrrigazione",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function Leggi_Specie(options) {

    let Operazione = parseInt($(TipoOperazione).val());

    if ($("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === undefined ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === null ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === "" ||
        $("#Data_Irrigazione").data("kendoDatePicker") === undefined ||
        $("#Data_Irrigazione").data("kendoDatePicker") === null ||
        $("#Data_Irrigazione").data("kendoDatePicker") === "")
        return;

    let Veg_Cod = 0;

    let Id_Cod = 0;

    if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== undefined &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== null &&
        $("#ddl_Specie_Irrigazione").data("kendoDropDownList") !== "" &&
        Get_KendoDDLValue("ddl_Specie_Irrigazione") !== "") {

        let array_dd_Specie_Value = Get_KendoDDLValue("ddl_Specie_Irrigazione").split("/");

        if (array_dd_Specie_Value && array_dd_Specie_Value.length >= 1) {
            if (array_dd_Specie_Value.length === 2) {
                Id_Cod = array_dd_Specie_Value[1];
            } else {
                Veg_Cod = array_dd_Specie_Value[0];
            }
        }
    }

    if (Veg_Cod === 0 &&
        Id_Cod === 0 &&
        $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod") !== undefined &&
        (Operazione === enum_tipoOperazione.Lettura || Operazione === enum_tipoOperazione.Modifica)) {

        let array_dd_Specie_Value = $("#ddl_Specie_Irrigazione").attr("last_selected_veg_cod").split("/");

        if (array_dd_Specie_Value && array_dd_Specie_Value.length >= 1) {
            if (array_dd_Specie_Value.length === 2) {
                Id_Cod = array_dd_Specie_Value[1];
            } else {
                Veg_Cod = array_dd_Specie_Value[0];
            }
        }

    }


    let param = kendo.stringify(
        {
            piva: $(cIdPiva).val(),
            sa_cod: parseInt(Get_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione")),
            data: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_Irrigazione").data("kendoDatePicker").value())),
            tipo_operazione: parseInt($(TipoOperazione).val()),
            veg_cod: Veg_Cod,
            id_cod: Id_Cod
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}


function Leggi_Udm(options) {

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Udm",
        "",
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function Controlla_Sportello() {

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    let Data_Agenda = "";

    if (Dati !== null && Dati !== undefined && Dati !== "") {
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);
        Data_Agenda = objParametriAgenda.Data;
    }

    if (Data_Agenda === "")
        return;


    if ($("#Data_Irrigazione").data("kendoDatePicker") === undefined ||
        $("#Data_Irrigazione").data("kendoDatePicker") === null ||
        $("#Data_Irrigazione").data("kendoDatePicker") === "")
        return;

    if ($("#Data_Irrigazione").data("kendoDatePicker").value() === undefined ||
        $("#Data_Irrigazione").data("kendoDatePicker").value() === null ||
        $("#Data_Irrigazione").data("kendoDatePicker").value() === "")
        return;

    let param = kendo.stringify(
        {
            data: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_Irrigazione").data("kendoDatePicker").value())),
            data_agenda: formattedReverseDate(Data_Agenda),
            tipo_operazione: parseInt($(TipoOperazione).val()),
            piva: $(cIdPiva).val()
        });

    ajaxAgronicaSync(indirizzohttp + "/Controlla_Sportello",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            if (risp !== undefined && risp !== null && risp !== "") {

                if (risp.modificateDate === true || risp.modificateDate === "true") {
                    
                    $("#Data_Irrigazione").data("kendoDatePicker").value(kendo.parseDate(risp.Data_Irrigazione, "MM/dd/yyyy"));

                    MessaggioErrore(risp.messsaggio_errore, "DIV_Messaggi");
                }
            }

        }, null);

}

function GestioneTabDivNote() {

    var TipoOperazioneDB = parseInt($(TipoOperazione).val());

    var elenco_CheckBox = "";

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    if (Dati !== null && Dati !== undefined && Dati !== "")
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);

    if (objParametriAgenda.Note === undefined || objParametriAgenda.Note === null)
        return;

    var Lav_cod = 0;
    var Veg_cod = 0;
    var Id_cod = 0;
    if ((TipoOperazioneDB === enum_tipoOperazione.Scrittura) &&
        ((objParametriAgenda.Lav_Cod === undefined || objParametriAgenda.Lav_Cod === null) ||
            ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") === undefined || $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === null || $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === ""))) {
        return;
    }
    else {
        Lav_cod = objParametriAgenda.Lav_Cod;

        let array_dd_Specie_Value = Get_KendoDDLValue("ddl_Specie_Irrigazione").split("/");

        if (array_dd_Specie_Value && array_dd_Specie_Value.length >= 1) {
            if (array_dd_Specie_Value.length === 2) {
                Id_cod = array_dd_Specie_Value[1];
            } else {
                Veg_Cod = array_dd_Specie_Value[0];
            }
        }
    }


    let param = kendo.stringify(
        {
            data: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_Irrigazione").data("kendoDatePicker").value())),
            piva: $(cIdPiva).val(),
            veg_cod: Veg_cod,
            id_cod: Id_cod,
            lav_cod: Lav_cod,
            tipooperazioneagenda: parseInt(objParametriAgenda.TipoOperazioneAgenda),
            objParametriAgendaNote: kendoEscapeOggetto(objParametriAgenda.Note),
            tipooperazione: TipoOperazioneDB
        });

    ajaxAgronicaSync(indirizzohttp + "/GestioneTabDivNote",
        param,
        false,
        function (risposta) {
            elenco_CheckBox = JSON.parse(risposta.RispostaStringa);

            if (elenco_CheckBox !== undefined && elenco_CheckBox !== null && elenco_CheckBox !== "" && elenco_CheckBox.length > 0) {
                if (elenco_CheckBox[0].Titolo_Tab_Giustificazioni !== undefined &&
                    elenco_CheckBox[0].Titolo_Tab_Giustificazioni !== null && 
                    elenco_CheckBox[0].Titolo_Tab_Giustificazioni !== "") {
                    //Cambia Nome alla  prima Tab
                    $("#a_tabGiust").text(elenco_CheckBox[0].Titolo_Tab_Giustificazioni);

                    elenco_CheckBox.splice(0, 1);

                }


                Elenco_Checkbox_Note = elenco_CheckBox;

                //Se ci sono delle tab da nascondere le nascondo
                if (risposta.ParametroDue !== "" && (risposta.ParametroDue === "True" || risposta.ParametroDue === true)) {

                    let elenco_Tab = JSON.parse(risposta.ParametroDue_stringa);

                    //Mostro solo i tab che hanno flag visibile === 1
                    for (let y = 0; y < elenco_Tab.length; y++) {

                        if (elenco_Tab[y].NotaGruppo_Des === enum_Note_Intervento_Gruppi.Giustificazioni) {
                            $("#checkbox_a_tabGiust").html('');
                            $("#checkbox_a_tabGiust").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabGiust").show();
                                $("#a_tabGiust").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabGiust").hide();
                                $("#a_tabGiust").hide();
                            }
                        }

                        if ($("#a_tabMeteo")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {
                            $("#checkbox_a_tabMeteo").html('');
                            $("#checkbox_a_tabMeteo").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabMeteo").show();
                                $("#a_tabMeteo").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabMeteo").hide();
                                $("#a_tabMeteo").hide();
                            }
                        }

                        if ($("#a_tabVentoIntensita")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {
                            $("#checkbox_a_tabVentoIntensita").html('');
                            $("#checkbox_a_tabVentoIntensita").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabVentoIntensita").show();
                                $("#a_tabVentoIntensita").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabVentoIntensita").hide();
                                $("#a_tabVentoIntensita").hide();
                            }
                        }


                        if ($("#a_tabVentoDirezione")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {
                            $("#checkbox_a_tabVentoDirezione").html('');
                            $("#checkbox_a_tabVentoDirezione").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabVentoDirezione").show();
                                $("#a_tabVentoDirezione").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabVentoDirezione").hide();
                                $("#a_tabVentoDirezione").hide();
                            }
                        }


                        if ($("#a_tabTemperatura")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {
                            $("#checkbox_a_tabTemperatura").html('');
                            $("#checkbox_a_tabTemperatura").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabTemperatura").show();
                                $("#a_tabTemperatura").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabTemperatura").hide();
                                $("#a_tabTemperatura").hide();
                            }
                        }


                        if ($("#a_tabOrario")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {

                            $("#checkbox_a_tabOrario").html('');
                            $("#checkbox_a_tabOrario").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabOrario").show();
                                $("#a_tabOrario").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabOrario").hide();
                                $("#a_tabOrario").hide();
                            }

                        }


                        if ($("#a_tabMotivazioni")[0].innerHTML === elenco_Tab[y].NotaGruppo_Des) {

                            $("#checkbox_a_tabMotivazioni").html('');
                            $("#checkbox_a_tabMotivazioni").empty();

                            if (elenco_Tab[y].visibile === 1) {
                                $("#checkbox_a_tabMotivazioni").show();
                                $("#a_tabMotivazioni").show();
                            }
                            else if (elenco_Tab[y].visibile === 0) {
                                $("#checkbox_a_tabMotivazioni").hide();
                                $("#a_tabMotivazioni").hide();
                            }
                        }
                    }


                    for (let x = 0; x < elenco_CheckBox.length; x++) {
                        var content_dialog = "";

                        //################### TEXTBOX E CHECKBOX TAB ###################//
                        content_dialog += "<div class='row'>";
                        content_dialog += "<div class='col-lg-6 col-md-6 col-sm-12'>";
                        content_dialog += "<div style='padding-top: 10px'>";                    
                        //content_dialog += "<input id='chk" + elenco_CheckBox[x].Nota_Des.replace(/ /g, "") + "' type='checkbox' class='k-checkbox' />";
                        //content_dialog += "<label class='k-checkbox-label' for='chk" + elenco_CheckBox[x].Nota_Des.replace(/ /g, "") + "'>"+elenco_CheckBox[x].Nota_Des+"</label>";
                        content_dialog += "<input id='chk" + elenco_CheckBox[x].Nota_Cod + "' type='checkbox' class='k-checkbox' />";
                        content_dialog += "<label class='k-checkbox-label' for='chk" + elenco_CheckBox[x].Nota_Cod + "'>" + elenco_CheckBox[x].Nota_Des + "</label>";
                        content_dialog += "</div>";
                        content_dialog += "</div>";
                        content_dialog += "</div>";
                        //#########################################################//

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Giustificazioni) {
                            $("#checkbox_a_tabGiust").html($("#checkbox_a_tabGiust").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Meteo) {
                            $("#checkbox_a_tabMeteo").html($("#checkbox_a_tabMeteo").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Motivazione) {
                            $("#checkbox_a_tabMotivazioni").html($("#checkbox_a_tabMotivazioni").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Orario) {
                            $("#checkbox_a_tabOrario").html($("#checkbox_a_tabOrario").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Temperatura) {
                            $("#checkbox_a_tabTemperatura").html($("#checkbox_a_tabTemperatura").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Vento_Direzione) {
                            $("#checkbox_a_tabVentoDirezione").html($("#checkbox_a_tabVentoDirezione").html() + content_dialog);
                        }

                        if (elenco_CheckBox[x].NotaGruppo_Cod === enum_Note_Intervento_Gruppi.Vento_Intensita) {
                            $("#checkbox_a_tabVentoIntensita").html($("#checkbox_a_tabVentoIntensita").html() + content_dialog);
                        }

                        //Aggiungo l'attributo Nota_Cod alla checkbox per sapere qual'è il suo codice
                        //$("#chk" + elenco_CheckBox[x].Nota_Des.replace(/ /g, "")).attr("Nota_Cod", elenco_CheckBox[x].Nota_Cod);
                        $("#chk" + elenco_CheckBox[x].Nota_Cod).attr("Nota_Cod", elenco_CheckBox[x].Nota_Cod);

                    }
                }

                for (let x = 0; x < elenco_CheckBox.length; x++) {
                    if (elenco_CheckBox[x].Checkato === true || elenco_CheckBox[x].Checkato === "true")
                        $("#chk" + elenco_CheckBox[x].Nota_Cod).prop("checked", true);
                        //$("#chk" + elenco_CheckBox[x].Nota_Des.replace(/ /g, "")).prop("checked", true);
                }

                //Se la prima tab "Giustificazioni" non è visibile sposto la classe "active" alla seconda tab Note
                if ($("#a_tabGiust").is(':hidden') === true &&
                    $("#li_tabGiust").hasClass("active") === true &&
                    $("#li_tabNote").hasClass("active") === false &&
                    $("#tabGiust").hasClass("tab-pane fade in active") === true &&
                    $("#tabNote").hasClass("tab-pane fade in active") === false) {

                    $("#tabGiust").removeClass("active");
                    $("#tabGiust").addClass("tab-pane fade in");
                    $("#li_tabGiust").removeClass("active");
                    $("#tabNote").removeClass("tab-pane fade in");
                    $("#tabNote").addClass("tab-pane fade in active");
                    $("#li_tabNote").addClass("active");
                }
                else if ($("#a_tabGiust").is(':hidden') === true &&
                        $("#li_tabGiust").hasClass("active") === false &&
                        $("#li_tabNote").hasClass("active") === true &&
                        $("#tabGiust").hasClass("tab-pane fade in active") === false &&
                        $("#tabNote").hasClass("tab-pane fade in active") === true) {

                    $("#tabGiust").removeClass("tab-pane fade in");
                    $("#tabGiust").addClass("tab-pane fade in active");
                    $("#li_tabGiust").addClass("active");
                    $("#tabNote").removeClass("active");
                    $("#tabNote").addClass("tab-pane fade in");
                    $("#li_tabNote").removeClass("active");
                }
            }
            

        }, null);
}


function Salva_TabDivNote() {
    //Ottengo le checkbox selezionate della varie tab.

    var ElencoCheckBox = [];


    $('div#checkbox_a_tabGiust input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });



    $('div#checkbox_a_tabMeteo input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });


    $('div#checkbox_a_tabVentoIntensita input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });


    $('div#checkbox_a_tabVentoDirezione input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });


    $('div#checkbox_a_tabTemperatura input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });


    $('div#checkbox_a_tabOrario input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });


    $('div#checkbox_a_tabMotivazioni input[type=checkbox]').each(function () {
        if ($(this).is(":checked")) {
            ElencoCheckBox.push($(this).attr('Nota_Cod'));
        }
    });

    return ElencoCheckBox;
}


function popolaMenuRicette() {

    let ricetta_cod_appoggio = parseInt($(Ricetta_Cod).val());
    let ricetta_operazione_cod_appoggio = 0
    if ($.isNumeric($(Operazione_Ricetta).val())) {
        ricetta_operazione_cod_appoggio = parseInt($(Operazione_Ricetta).val());
    }
    var parametri = kendo.stringify({
        ricetta_cod: ricetta_cod_appoggio,
        ricetta_operazione_cod: ricetta_operazione_cod_appoggio
    });

    ajaxAgronicaSync(indirizzohttp + "/caricaRicetteOperazioni_Irrigazione",
        parametri,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);

            $("#ulMenuRicette_Irrigazione").html('');
            for (let i = 0; i < risp.length; i++) {
                if (!(risp[i].ribaltata == true)) {
                    let elem = "<li class='btn-success' onClick=\"BottoneSalvaRicetta(" + risp[i].tipo + ", " + risp[i].Ricetta_Cod + ", " + risp[i].Ricetta_Operazione_Cod + ",'" + risp[i].data + "')\">";
                    elem += risp[i].des;
                    elem += "</li>";
                    $("#ulMenuRicette_Irrigazione").append(elem);
                } else {
                    let elem = "<li class='btn btn-secondary' disabled>";
                    elem += '<i class="fa fa-times-circle" style="color:red;"></i>' + risp[i].des;
                    elem += "</li>";
                    $("#ulMenuRicette_Irrigazione").append(elem);
                }

            }
        }, null);

}


function Ricetta_Irrigazione_numero_default() {

    var ricetta_numero='';

    if ($("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === undefined ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === null ||
        $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === "")
        return ricetta_numero;

    var parametri = kendo.stringify({
        piva: $(cIdPiva).val(),
        sa_cod: parseInt(Get_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione"))
    });

    ajaxAgronicaSync(indirizzohttp + "/ricetta_numero_default",
        parametri,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            ricetta_numero = risp;
        }, null);

    return ricetta_numero;

}

function caricaTestataRicetta_Irrigazione(ricetta_cod) {

    var datiTestata;

    var parametri = kendo.stringify({
        ricetta_cod: ricetta_cod
    });

    ajaxAgronicaSync(indirizzohttp +"/caricaTestataRicetta_Irrigazione",
        parametri,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            datiTestata = risp;
        }, null);

    return datiTestata;
}


function CreaKendoGrid_Impianti_Irrigazione() {

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = JSON.parse(Dati.objParametriAgenda);

    let dd_Specie_Value = Get_KendoDDLValue("ddl_Specie_Irrigazione");

    let Veg_Cod = 0;

    let Id_cod = 0;

    if (dd_Specie_Value === undefined || dd_Specie_Value === null || dd_Specie_Value === "") {
        return;
    } else {

        let array_dd_Specie_Value = dd_Specie_Value.split("/");

        if (array_dd_Specie_Value && array_dd_Specie_Value.length >= 1) {
            if (array_dd_Specie_Value.length === 2) {
                Id_cod = array_dd_Specie_Value[1];
            } else {
                Veg_Cod = array_dd_Specie_Value[0];
            }
        }
    }


    let ddl_Centro_Aziendale_Value = Get_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione");

    if (ddl_Centro_Aziendale_Value === undefined || ddl_Centro_Aziendale_Value === null || ddl_Centro_Aziendale_Value === "")
        return;

    let param = kendo.stringify(
        {
            piva: $(cIdPiva).val(),
            veg_cod: Veg_Cod,
            id_cod: Id_cod,
            cul_cod: objParametriAgenda.Cul_Cod,
            sa_cod: parseInt(ddl_Centro_Aziendale_Value),
            data: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_Irrigazione").data("kendoDatePicker").value())),
            tipo_operazione: parseInt($(TipoOperazione).val()),
            tipo_operazione_agenda: objParametriAgenda.TipoOperazioneAgenda,
            lav_cod: objParametriAgenda.Lav_Cod,
            disciplinare: objParametriAgenda.Disciplinare,
            irrigazioni: Irrigazioni,
            objParamAgendaImpianti: kendoEscapeOggetto(objParametriAgenda.Impianti)
        });

    ajaxAgronicaSync(indirizzohttp + "/CreaKendoGrid_Impianti_Irrigazione",
        param,
        false,
        function (risposta) {
            $(KendoGridmpianti_Irrigazione).val(risposta.RispostaStringa);

            if (risposta.ParametroDue !== "" && (risposta.ParametroDue === "True" || risposta.ParametroDue === true))
                $(ElencoColonneKendoGrid_Impianti_Irrigazione).val(risposta.ParametroDue_stringa);
 
        }, null);
}


function CreaKendoGrid_RilieviPioggie_Irrigazione() {

    if ($("#Data_Da_Irrigazione").data("kendoDatePicker") === undefined ||
        $("#Data_Da_Irrigazione").data("kendoDatePicker") === null ||
        $("#Data_Da_Irrigazione").data("kendoDatePicker") === "" ||
        $("#Data_A_Irrigazione").data("kendoDatePicker") === undefined ||
        $("#Data_A_Irrigazione").data("kendoDatePicker") === null ||
        $("#Data_A_Irrigazione").data("kendoDatePicker") === "")
        return;

    if ($("#Data_Da_Irrigazione").data("kendoDatePicker").value() === null &&
        $("#Data_A_Irrigazione").data("kendoDatePicker").value() === null) {

        MessaggioErrore("Inserire un formato di data valido per l'intervallo temporale. <br> gg/mm/aaaa Es: 30/10/2012 .", "DIV_Messaggi");
        
        return;
    }
    else if ($("#Data_Da_Irrigazione").data("kendoDatePicker").value() === null ||
             $("#Data_A_Irrigazione").data("kendoDatePicker").value() === null) {

            MessaggioErrore("E ' necessario specificare entrambe le date dell'intervallo temporale. <br> Se si desidera un solo giorno, specificare la stessa data in entrambe le caselle.", "DIV_Messaggi");

        return;
    }

    var grid = $("#grid_RilieviPioggie_Irrigazione").data("kendoGrid");

    if (grid !== undefined && grid !== null && grid !== "") {
        //Distruggo e ricarico la grid prima di leggere i dati
        grid.destroy();
        $("#grid_RilieviPioggie_Irrigazione").empty();
    }

    let param = kendo.stringify(
        {
            piva: $(cIdPiva).val(),
            data_da: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_Da_Irrigazione").data("kendoDatePicker").value())),
            data_a: formattedReverseDate(sistemaDataInBaseAllaCulture($("#Data_A_Irrigazione").data("kendoDatePicker").value()))
        });

    ajaxAgronicaSync(indirizzohttp + "/CreaKendoGrid_RilieviPioggie_Irrigazione",
        param,
        false,
        function (risposta) {
            $(KendoGridRilieviPioggie_Irrigazione).val(risposta.RispostaStringa);

            kendoGrid_RilieviPioggie_Irrigazione("grid_RilieviPioggie_Irrigazione");
        }, function (risposta) {
            MessaggioErrore(risposta.Errore, "DIV_Messaggi");
        });
}

function SalvaImpostazioniColonneGrid_Impianti_Irrigazione() {

    let elenco_check = $("#elenco_colonneGrid_Impianti_Irrigazione").find(".k-checkbox-label");

    let Colonne_Visibili = [];

    if (elenco_check.length !== 0) {

        //Ottengo solo le colonne già checkate 
        for (var x = 0; x < elenco_check.length; x++) {

            let id_check = elenco_check[x].htmlFor;

            if ($("#"+id_check).is(":checked") === true) {
                Colonne_Visibili.push(elenco_check[x].outerText);
            }

        }

    }

    let param = kendo.stringify(
        {
            colonne_visibili: Colonne_Visibili.join("|")
        });

    ajaxAgronicaSync(indirizzohttp + "/Salva_ImpostazioniKendoGrid_Impianti_Irrigazione",
        param,
        false,
        function (risposta) {

            if (risposta.RispostaOK === true || risposta.RispostaOK === "True") {
                //Distruggo e ricarico la grid.

                if (Controlli_Prima_Di_creare_la_kendoGrid_Impianti_Irrigazione() === true) {
                    $("#grid_Impianti_Irrigazione").data("kendoGrid").destroy();
                    $("#grid_Impianti_Irrigazione").empty();

                    $("#grid_Impianti_Irrigazione").attr("dataBounded", "false");
                    kendoGrid_Impianti_Irrigazione("grid_Impianti_Irrigazione");
                }

            }

        }, null);

}


function Salva_Irrigazione(tiposalvataggio, obj_dettaglio_Irrigazione) {

    if (tiposalvataggio === undefined || tiposalvataggio === null || tiposalvataggio === "")
        return;

    let Dati = JSON.parse($(DatiLetti).val());

    let objParametriAgenda = "";

    if (Dati !== undefined && Dati !== null && Dati !== "") 
        objParametriAgenda = JSON.parse(Dati.objParametriAgenda);

    if ((objParametriAgenda !== "") &&
        (objParametriAgenda.Id_Agenda === undefined || objParametriAgenda.Lav_Cod === undefined ||
        objParametriAgenda.Lav_Des === undefined || objParametriAgenda.Cau_Mov === undefined ||
        objParametriAgenda.TargetOperazione === undefined || objParametriAgenda.PaginaSitoOrigine === undefined ||
        objParametriAgenda.Campo_Cod === undefined || objParametriAgenda.Cul_Cod === undefined ||
        objParametriAgenda.TipoOperazioneAgenda === undefined || objParametriAgenda.Programmazione_Cod === undefined ||
        objParametriAgenda.TipoRicetta === undefined || objParametriAgenda.SitoOrigine === undefined))
        return;

    if ($("#grid_Impianti_Irrigazione").data("kendoGrid") === undefined || $("#grid_Impianti_Irrigazione").data("kendoGrid") === null)
        return;
    else
        $("#grid_Impianti_Irrigazione").data("kendoGrid").saveChanges();


    if ($("#Data_Irrigazione").data("kendoDatePicker") === undefined || $("#Data_Irrigazione").data("kendoDatePicker") === null)
        return;

    if ($("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === undefined || $("#ddl_Centro_Aziendale_Irrigazione").data("kendoDropDownList") === null)
        return;

    if ($("#ddl_Specie_Irrigazione").data("kendoDropDownList") === undefined || $("#ddl_Specie_Irrigazione").data("kendoDropDownList") === null)
        return;

    if ($("#ddl_Tipo_Irrigazione").data("kendoDropDownList") === undefined || $("#ddl_Tipo_Irrigazione").data("kendoDropDownList") === null)
        return;

    if ($("#ddl_Udm_Irrigazione").data("kendoDropDownList") === undefined || $("#ddl_Udm_Irrigazione").data("kendoDropDownList") === null)
        return;

    var TipoOperazioneAgenda = parseInt(objParametriAgenda.TipoOperazioneAgenda);
    var PaginaSitoOrigine = parseInt(objParametriAgenda.PaginaSitoOrigine);
    var SitoOrigine = parseInt(objParametriAgenda.SitoOrigine);

    var NoteModel = new Object();
    NoteModel.Txt_Note = $("#Txt_Note").val();
    NoteModel.ElencoNoteSelezionate = Salva_TabDivNote();

    var CampiGrigliaIrrigazioneModel = new Object();
    CampiGrigliaIrrigazioneModel.RigheInserite = "[]";
    CampiGrigliaIrrigazioneModel.RigheModificate = "[]";
    CampiGrigliaIrrigazioneModel.RigheEliminate = "[]";
    CampiGrigliaIrrigazioneModel.TutteLeRighe = righeTutteGrid_Impianti_Irrigazione;

    var RicettaTestataModel = null;

    if (TipoOperazioneAgenda === enum_tipoOperazione_Agenda.Ricetta ||
                TipoOperazioneAgenda === enum_tipoOperazione_Agenda.RicettaBrogliaccio) {

        RicettaTestataModel = new Object();

        RicettaTestataModel.Ricetta_Descrizione = $("#Txt_Ricetta_Descrizione_Irrigazione").val();
        RicettaTestataModel.Ricetta_Numero = $("#Txt_Ricetta_Numero_Irrigazione").val();

        if ($("#Data_Inizio_ricetta_Irrigazione").data("kendoDatePicker") === undefined ||
            $("#Data_Inizio_ricetta_Irrigazione").data("kendoDatePicker") === null ||
            $("#Data_Inizio_ricetta_Irrigazione").data("kendoDatePicker") === "")
            RicettaTestataModel.Data_Inizio_Ricetta = null;
        else
            RicettaTestataModel.Data_Inizio_Ricetta = $("#Data_Inizio_ricetta_Irrigazione").data("kendoDatePicker").value();

        if ($("#Data_Fine_ricetta_Irrigazione").data("kendoDatePicker") === undefined ||
            $("#Data_Fine_ricetta_Irrigazione").data("kendoDatePicker") === null ||
            $("#Data_Fine_ricetta_Irrigazione").data("kendoDatePicker") === "")
            RicettaTestataModel.Data_Fine_Ricetta =  null;
        else
            RicettaTestataModel.Data_Fine_Ricetta = $("#Data_Fine_ricetta_Irrigazione").data("kendoDatePicker").value();

        RicettaTestataModel.Ricetta_Nota = $("#Txt_Ricetta_Nota_Irrigazione").val();
    }

    var SalvaIrrigazioneModel = new Object();

    SalvaIrrigazioneModel.TipoOperazioneDB = parseInt($(TipoOperazione).val());
    SalvaIrrigazioneModel.Piva = $(cIdPiva).val();
    SalvaIrrigazioneModel.Data = $("#Data_Irrigazione").data("kendoDatePicker").value();
    SalvaIrrigazioneModel.Id_Agenda = objParametriAgenda.Id_Agenda;
    SalvaIrrigazioneModel.Lav_Cod = objParametriAgenda.Lav_Cod;
    SalvaIrrigazioneModel.Lav_Des = objParametriAgenda.Lav_Des;
    SalvaIrrigazioneModel.TargetOperazione = objParametriAgenda.TargetOperazione;
    SalvaIrrigazioneModel.Cau_Mov = objParametriAgenda.Cau_Mov;
    SalvaIrrigazioneModel.PaginaSitoOrigine = PaginaSitoOrigine;
    SalvaIrrigazioneModel.SitoOrigine = SitoOrigine;
    SalvaIrrigazioneModel.Campo = objParametriAgenda.Campo_Cod;
    SalvaIrrigazioneModel.Varieta = objParametriAgenda.Cul_Cod;
    SalvaIrrigazioneModel.TipoOperazioneAgenda = TipoOperazioneAgenda;
    SalvaIrrigazioneModel.Programmazione_Cod = objParametriAgenda.Programmazione_Cod;
    SalvaIrrigazioneModel.TipoRicetta = objParametriAgenda.TipoRicetta;
    SalvaIrrigazioneModel.Centro_Aziendale = Get_KendoDDLValue("ddl_Centro_Aziendale_Irrigazione");
    SalvaIrrigazioneModel.Specie = Get_KendoDDLValue("ddl_Specie_Irrigazione");
    SalvaIrrigazioneModel.Specie_Des = $("#ddl_Specie_Irrigazione").data("kendoDropDownList").text();
    SalvaIrrigazioneModel.Tipo_Irrigazione = Get_KendoDDLValue("ddl_Tipo_Irrigazione");
    SalvaIrrigazioneModel.Unita_di_Misura_Dose = Get_KendoDDLValue("ddl_Udm_Irrigazione");
    SalvaIrrigazioneModel.Verifica_Compatibilita_MicroIrrigazione = $("#chkMicroirr").is(':checked').toString();
    SalvaIrrigazioneModel.Irrigazione = CampiGrigliaIrrigazioneModel;
    SalvaIrrigazioneModel.Note = NoteModel;
    SalvaIrrigazioneModel.RicettaTestata = RicettaTestataModel;

    //Per modifica dettaglio di una Ricetta di Irrigazione
    SalvaIrrigazioneModel.tipo_salva_parametri_Irrigazione = "";

    if (obj_dettaglio_Irrigazione !== undefined &&
        obj_dettaglio_Irrigazione !== null)
        SalvaIrrigazioneModel.tipo_salva_parametri_Irrigazione = JSON.stringify(obj_dettaglio_Irrigazione);

    var salvaModelEscaped = kendoEscapeOggetto(SalvaIrrigazioneModel);

    var param = "{model:'" + salvaModelEscaped + "', TipoSalvataggio:'" + tiposalvataggio + "', Qs_Operazione_Ricetta:'" + $(Operazione_Ricetta).val() + "', Qs_Ricetta_Cod:'" + $(Ricetta_Cod).val() +"'}"

    ajaxAgronicaSync(indirizzohttp + "/Salva_Irrigazione",
        param,
        false,
        function (risposta) {

            let risp = JSON.parse(risposta.RispostaStringa);

            if (risp.MsgRegistrazioneEffetuata !== "") {

                ScritturaOK(risp.MsgRegistrazioneEffetuata, "DIV_Messaggi");


                if (risposta.ParametroDue === true && risposta.ParametroDue_stringa !== "") {

                    if (TipoOperazioneAgenda === enum_tipoOperazione_Agenda.QuadernoDiCampagna) {

                        if (enum_tipoSalvataggio.Salva_Esci === tiposalvataggio && parseInt(risp.Id_Agenda) !== 0) {
                            window.parent.registrazioneAgenda(parseInt(risp.Id_Agenda));
                        }

                    }
                    else if (TipoOperazioneAgenda === enum_tipoOperazione_Agenda.Ricetta ||
                        TipoOperazioneAgenda === enum_tipoOperazione_Agenda.RicettaBrogliaccio) {


                        switch (tiposalvataggio) {

                            case enum_tipoSalvataggio.Salva_Esci:
                                if (PaginaSitoOrigine === enum_PagineAgenda_2010.Gis) {
                                    window.parent.registrazioneAgenda(parseInt(risp.Id_Agenda));
                                }
                                else if (SitoOrigine === enum_SiteRedirector.GiasLan) {
                                    window.close();
                                }

                                window.parent.registrazioneAgenda(parseInt(risp.Id_Agenda));
                                break;

                            case enum_tipoSalvataggio.Salva_Duplica:
                            case enum_tipoSalvataggio.Salva_Ricetta_NuovoDettaglio:
                                Abilita_Disabilita_Resto();
                                break;

                        }
                    }

                    let link = risposta.ParametroDue_stringa

                    window.location = link;

                }

            } 
        }, function (risposta) {
            if (risposta.Errore !== "")
                MessaggioErrore(risposta.Errore, "DIV_Messaggi");
        });
}

function Leggi_DSS_Irrigazione(options) {

    let risp = null;

    let param = kendo.stringify(
        {
            data: $("#Data_Irrigazione").data("kendoDatePicker").value(),
            piva: options.model.PIVA,
            sa_cod: options.model.SA_COD,
            appezza: options.model.APPEZZA,
            id_reg: options.model.ID_REG,
            progetto_cod: options.model.Progetto_Cod,
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_DSS_Irrigazione",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            risp.unshift(Empty_obj_DSS_Irrigazione);
        }, null);

    return risp;
}

function Leggi_Turni_Salvati(ID_DSS_Irrigazione) {

    let risp = null;

    let param = kendo.stringify(
        {
            id_dss: ID_DSS_Irrigazione
        });

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Turni_Salvati",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp;
}