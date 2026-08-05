
const Irrigazioni_GIAS = 1;
const Irrigazioni_IF = 2;
const Bilancio_IF = 3;

var _getKeyGIAS = function (Obj) {
    return Obj.GIAS_PIva + "_" + Obj.GIAS_SaCod + "_" + Obj.GIAS_Appezza + "_" + Obj.GIAS_IdReg + "_" + Obj.GIAS_ProgettoCod;
};

(function ($) {

    $.WBResult_V2 = function (elem, header, coltura, arrIndic, persistent) {

        var _indic_2_WBInput = function (indic) {

            let wbInput = new Object;

            wbInput.CUAA = indic.CUAA;
            wbInput.Decription = indic.Impianto;
            wbInput.Superficie_ha = indic.Sup_Imp;
            wbInput.Pendenza = indic.Pendenza;
            wbInput.Sabbia = indic.Sabbia;
            wbInput.Argilla = indic.Argilla;
            wbInput.GruppoVegetale = indic.GruppoVegetale;
            wbInput.TraFila = indic.TraFila;
            wbInput.SuFila = indic.SuFila;
            wbInput.CondTraFila = indic.CondTraFila;
            wbInput.Vigoria = indic.Vigoria;
            wbInput.DataInizioImpianto = indic.DataInizioImpianto;
            wbInput.Irri_Veg_Cod = indic.Irri_Veg_Cod;
            wbInput.Irri_Imp_Cod = indic.Irri_Imp_Cod;
            wbInput.Irri_Plot_Id = indic.Irri_Plot_Id;
            wbInput.Irri_Crop_Id = indic.Irri_Crop_Id;
            wbInput.PersistData = {
                GIAS_PIva: indic.GIAS_PIva,
                GIAS_SaCod: indic.GIAS_SaCod,
                GIAS_Appezza: indic.GIAS_Appezza,
                GIAS_IdReg: indic.GIAS_IdReg,
                GIAS_ProgettoCod: indic.GIAS_ProgettoCod,
                Irri_PlotId: indic.Irri_Plot_Id,
                Irri_CropId: indic.Irri_Crop_Id
            }
            wbInput.StazioneMeteo = indic.StazioneMeteo;
            wbInput.Irrigazioni = indic.Irrigazioni;
            wbInput.Fertilizzazioni = indic.Fertilizzazioni;
            wbInput.FertiRecipe = indic.FertiRecipe;
            wbInput.Cop_Cod_Irriframe = indic.Cop_Cod_Irriframe;
            wbInput.Mac_Cod = indic.Mac_Cod;
            wbInput.Portata = indic.Portata;
            wbInput.DataFaseStart = null;
            wbInput.RunData = null;

            wbInput.Veg_Cod = indic.Veg_Cod;
            wbInput.Cul_Cod = indic.Cul_Cod;
            wbInput.Sup_Imp = indic.Sup_Imp;
            wbInput.Coord = null;
            wbInput.Indirizzo = "";
            wbInput.Info = [];

            //controllare che GruppoVegetale.ToLower.Trim = "arboree" ???
            if (indic.DataRilievo !== undefined && indic.DataRilievo !== null) {

                wbInput.DataFaseStart = new Date(indic.DataRilievo);

            } else {

                if (indic.DataSemina !== undefined && indic.DataSemina !== null) {

                    wbInput.DataFaseStart = new Date(indic.DataSemina);

                } else {

                    if (indic.DataSeminaPrevista !== undefined && indic.DataSeminaPrevista !== null) {

                        wbInput.DataFaseStart = new Date(indic.DataSeminaPrevista);
                    }
                }
            }

            if (indic.Lat !== undefined && indic.Lat !== null && indic.Lng !== undefined && indic.Lng !== null) {

                wbInput.Coord = { Lat: indic.Lat, Lng: indic.Lng };
            }

            if (indic.Indirizzo !== undefined && indic.Indirizzo !== null) {
                let strArr = [
                    indic.Indirizzo.Indirizzo,
                    indic.Indirizzo.Fraz,
                    indic.Indirizzo.CAP,
                    indic.Indirizzo.Comune,
                    indic.Indirizzo.Prov,
                    "IT"
                ];

                wbInput.Indirizzo = strArr.filter((s) => s != null && s.length > 0).join(" ");
            }

            let infoValue = ((indic.DataInizioImpianto != null) ? (new Date(indic.DataInizioImpianto)).toLocaleDateString() : "");
            wbInput.Info.push({ text: "Inizio impianto", value: infoValue });

            if (indic.GruppoVegetale.toLowerCase() === "arboree") {

                infoValue = ((indic.TraFila !== null) ? kendo.toString(indic.TraFila, "0.0") : "");
                wbInput.Info.push({ text: "Distanza tra fila", value: infoValue });

                infoValue = ((indic.SuFila !== null) ? kendo.toString(indic.SuFila, "0.0") : "");
                wbInput.Info.push({ text: "Distanza su fila", value: infoValue });

                infoValue = "";
                if (indic.CondTraFila !== null && indic.CondTraFila.length > 0) {
                    infoValue = indic.CondTraFila;
                    if (infoValue.toLowerCase() != "inerbito") {
                        infoValue = "Lavorato";
                    }
                }
                wbInput.Info.push({ text: "Conduzione interfilare", value: infoValue });

                infoValue = "";
                if (indic.Vigoria !== null) {
                    switch (indic.Vigoria) {
                        case 1: infoValue = "Debole"; break;
                        case 2: infoValue = "Medio"; break;
                        case 3: infoValue = "Vigoroso"; break;
                        case 4: infoValue = "Molto vigoroso"; break;
                    }
                }

                wbInput.Info.push({ text: "Classe vigore", value: infoValue });
            }

            infoValue = (indic.Irri_Imp_Des != null) ? indic.Irri_Imp_Des : "";
            wbInput.Info.push({ text: "Impianto irriguo", value: infoValue });

            infoValue = (indic.Pendenza != null) ? kendo.toString(indic.Pendenza, "0") : "";
            wbInput.Info.push({ text: "Pendenza", value: infoValue });

            infoValue = (indic.Sabbia != null) ? kendo.toString(indic.Sabbia, "0.0") + "%" : "";
            wbInput.Info.push({ text: "Sabbia", value: infoValue });

            infoValue = (indic.Argilla != null) ? kendo.toString(indic.Argilla, "0.0") + "%" : "";
            wbInput.Info.push({ text: "Argilla", value: infoValue });





            //    ColturaProtetta = False
            //    Id_GreenHouseType = 0
            //    Dim cop = row("Cop_Cod")
            //    Dim cop_mappato = row("Cop_Cod_Irriframe")
            //    If Not IsDBNull(cop) Then
            //        Dim copNum = CInt(cop)
            //        If Not IsDBNull(cop_mappato) Then
            //            ColturaProtetta = True
            //            Id_GreenHouseType = CInt(cop_mappato)
            //        Else
            //            If (Not {0, 3, 4, 5, 6}.Contains(copNum)) Then
            //                ColturaProtetta = True
            //                Errore = "Mappatura copertura non trovata"
            //            End If
            //        End If
            //    End If
            //End Sub




            return wbInput
        }

        var _creaDiv = function (_class, _style, _id, _txt) {
            let div = document.createElement("div");
            if (typeof _class === "string" && _class !== "") {
                div.className = _class;
            }
            if (typeof _style === "string" && _style !== "") {
                div.style.cssText = _style;
            }
            if (typeof _id === "string" && _id !== "") {
                div.id = _id;
            }
            if (typeof _txt === "string" && _txt !== "") {
                div.innerHTML = _txt;
            }
            return div;
        };

        var _clearElement = function (elem) {
            while (elem.firstChild) {
                elem.removeChild(elem.firstChild);
            }
        };

        var _getElemAndClear = function (selector) {

            let elem = plugin.$body.find(selector);
            if (elem.length == 0) {
                return null;
            }
            elem = elem[0];

            _clearElement(elem);

            $(elem).removeClass("valid");
            $(elem).removeClass("not-valid");

            return elem;
        };

        var _creaBtn = function (parent, classname, fa_icon, text) {
            let btn = _creaDiv("k-button k-button-md k-rounded-md " + classname);
            btn.appendChild(_creaDiv("", "", "", "<span class='fa " + fa_icon + " fa-lg fa-fw'></span>"));
            btn.appendChild(_creaDiv("overflow-ellipsis", "", "", text));
            parent.appendChild(btn);
            return btn;
        }

        var _sayLatLng = function () {

            let elem = _getElemAndClear(".lat-lng");
            if (elem == null) {
                return;
            }

            if (plugin.wbInput.Coord === null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "indic-tooltip", "fa-map-marker", "Geolocalizza");
                let indirizzo = "";
                if (plugin.wbInput.Indirizzo != "") {
                    indirizzo = "<div style='margin-top:10px; font-style:italic;'>" + plugin.wbInput.Indirizzo + "</div>";
                }
                btn.setAttribute("data-tooltip", "<div style='text-align:center;'><div>Geolocalizza da indirizzo appezzamento/centro</div>" + indirizzo + "</div>");
                btn.onclick = function () { _geolocalizza(); };

                return;
            }

            $(elem).addClass("label-small").addClass("valid");

            let colLat = _creaDiv();
            elem.appendChild(colLat);
            colLat.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.wbInput.Coord.Lat, "0.0000000") + "</span>"));
            colLat.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lat</span>"));

            let colLng = _creaDiv();
            elem.appendChild(colLng);
            colLng.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.wbInput.Coord.Lng, "0.0000000") + "</span>"));
            colLng.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lng</span>"));
        };

        var _sayStart = function () {

            let elem = _getElemAndClear(".data-start");
            if (elem == null) {
                return;
            }

            if (plugin.wbInput.DataFaseStart == null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "", "fa-calendar", "Imposta data start")
                btn.onclick = function () { _impostaStart(); };

                return;
            }

            let start = kendo.toString(plugin.wbInput.DataFaseStart, "d");
            $(elem).addClass("label-small valid");
            elem.appendChild(_creaDiv("overflow-ellipsis", "", "", start));
            let divQuestion = _creaDiv("indic-tooltip", "color: #337ab7;", "", "<span class='fa fa-question-circle fa-lg fa-fw'></span>");
            //let divQuestion = _creaDiv("indic-tooltip", "color: #337ab7;", "", "<span class='k-icon k-i-question-circle'></span>");
            divQuestion.setAttribute("data-tooltip", "Data semina, trapianto o risveglio vegetativo");
            elem.appendChild(divQuestion);
        };

        var _getIdBtnSaveDSS = function (withGate) {
            if (withGate) {
                return "#BtnSaveDSS_" + _getKeyGIAS(plugin.wbInput.PersistData);
            } else {
                return "BtnSaveDSS_" + _getKeyGIAS(plugin.wbInput.PersistData);
            }

        }

        var _getIdBtnExportDSS = function (withGate) {
            if (withGate) {
                return "#BtnExportDSS_" + _getKeyGIAS(plugin.wbInput.PersistData);
            } else {
                return "BtnExportDSS_" + _getKeyGIAS(plugin.wbInput.PersistData);
            }

        }

        var _enableBtnSaveDSS = function () {
            $(_getIdBtnSaveDSS(true)).getKendoButton().enable(true);
        };

        var _disableBtnSaveDSS = function () {
            $(_getIdBtnSaveDSS(true)).getKendoButton().enable(false);
        };

        var _creaBtnSaveDSS = function (parent, text) {
            let div = _creaDiv("", "display:inline-block;");
            let btn = _creaDiv("k-button k-button-md k-rounded-md", "font-size: 12px;", _getIdBtnSaveDSS(false));
            btn.appendChild(_creaDiv("overflow-ellipsis", "", "", text));
            div.appendChild(btn);
            parent.appendChild(div);
            return btn;
        }

        var _enableBtnExportDSS = function () {
            $(_getIdBtnExportDSS(true)).getKendoButton().enable(true);
        };

        var _disableBtnExportDSS = function () {
            $(_getIdBtnExportDSS(true)).getKendoButton().enable(false);
        };

        var _creaBtnExportDSS = function (parent, text) {
            let div = _creaDiv("", "display:inline-block;padding-left: 5px;");
            let btn = _creaDiv("k-button k-button-md k-rounded-md", "font-size: 12px;", _getIdBtnExportDSS(false));
            btn.appendChild(_creaDiv("overflow-ellipsis", "", "", text));
            div.appendChild(btn);
            parent.appendChild(div);
            return btn;
        }

        var _enableBtnsDSS = function () {
            _enableBtnExportDSS();
            _enableBtnSaveDSS();
        }

        var _disableBtnsDSS = function () {
            _disableBtnExportDSS();
            _disableBtnSaveDSS();
        }

        var _getIdDivLabelStampaConsiglio = function (withGate) {
            if (withGate) {
                return "#DivLabelStampaConsiglio_" + _getKeyGIAS(plugin.wbInput.PersistData);
            } else {
                return "DivLabelStampaConsiglio_" + _getKeyGIAS(plugin.wbInput.PersistData);
            }

        }

        var _creaLabel = function (_parent, _txt) {

            let divlabel = _creaDiv("", "padding-top: 5px;margin: auto;text-align:center;display: none;", _getIdDivLabelStampaConsiglio(false));

            let label = document.createElement("label");

            let data_stampa = new Date();

            label.innerHTML = "Data di Stampa: " + data_stampa.toLocaleDateString();

            label.style = "font-size: 14px;font-weight: bold;";

            divlabel.appendChild(label);

            return divlabel;
        };

        let divWrapper = _creaDiv("indic-wrapper");
        elem.appendChild(divWrapper);

        let divBody = _creaDiv("indic-body");
        divWrapper.appendChild(divBody);

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        //plugin.gridElement = elem;
        plugin.$wrapper = $(divWrapper);
        plugin.wrapper = divWrapper;
        plugin.$body = $(divBody);
        plugin.body = divBody;
        plugin.persistent = persistent
        plugin.header = header;
        plugin.coltura = coltura;
        //Convertire tutti gli elementi dell'array???
        plugin.wbInput = _indic_2_WBInput(arrIndic[0]);
        plugin.wbInput.UsaMeteo = false;
        plugin.wbInput.IrriChoice = Irrigazioni_GIAS;

        let divTitle = _creaDiv("label-big overflow-ellipsis", "font-weight: bold;", "", plugin.header);
        let divSpecie = _creaDiv("label-big overflow-ellipsis", "", "", plugin.coltura);

        divBody.appendChild(divTitle);
        divBody.appendChild(divSpecie);

        let divParams = _creaDiv("grid-params")

        divBody.appendChild(divParams);

        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-globe fa-lg fa-fw'></span>"));
        let divCoord = _creaDiv("lat-lng");
        divParams.appendChild(divCoord);
        _sayLatLng();

        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-leaf fa-lg fa-fw'></span>"));
        let divStart = _creaDiv("data-start");
        divParams.appendChild(divStart);
        _sayStart();

        let fa = "fa-info-circle";
        //let fa = "k-i-info-circle";
        if (plugin.wbInput.Info.some((el) => el.value === "")) {
            fa = "fa-exclamation-triangle";
            //fa = "k-i-exclamation-circle";
        }

        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-tags fa-lg fa-fw'></span>"));
        let divPlusParams = _creaDiv("indic-parametri label-small", "cursor: pointer;");
        divParams.appendChild(divPlusParams);
        divPlusParams.appendChild(_creaDiv("overflow-ellipsis", "", "", "Ambientali/Impianto"));
        divPlusParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='fa " + fa + " fa-lg fa-fw'></span>"));
        //divPlusParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='k-icon " + fa + "'></span>"));
        divPlusParams.setAttribute("data-tooltip", JSON.stringify(plugin.wbInput.Info));

        plugin.result = _creaDiv();
        divWrapper.appendChild(plugin.result);

        let AllowPropMeteo = false;
        if (plugin.wbInput.StazioneMeteo !== undefined && plugin.wbInput.StazioneMeteo != null) {
            AllowPropMeteo = true;
            plugin.wbInput.UsaMeteo = true;
        }

        let divFooter = _creaDiv("", "margin-top: 5px; padding-top: 5px; border-top: 1px solid #cccccc;");
        elem.appendChild(divFooter);

        let label = document.createElement("label");
        label.className = "toggle-switch";
        divFooter.appendChild(label);
        let checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        label.appendChild(checkbox);
        let back = _creaDiv("toggle-back");
        label.appendChild(back);
        let toggle = _creaDiv("toggle");
        let lbl_on = _creaDiv("toggle-label on", "", "", "Dati meteo proprietari");
        let lbl_off = _creaDiv("toggle-label off", "", "", "Dati meteo pubblici");
        back.appendChild(toggle);
        back.appendChild(lbl_on);
        back.appendChild(lbl_off);

        if (!AllowPropMeteo) {

            checkbox.disabled = true;

        } else {

            checkbox.checked = plugin.wbInput.UsaMeteo;
        }

        $(checkbox).on('change', function () {

            let value = this.checked;

            if (plugin.wbInput.UsaMeteo === value) {
                return;
            }

            plugin.wbInput.UsaMeteo = value;

            plugin.run();
        });



        ////let divFooter2 = _creaDiv("", "padding-top: 5px");
        ////elem.appendChild(divFooter2);
        ////let label2 = document.createElement("label");
        ////label2.className = "toggle-switch";
        ////divFooter2.appendChild(label2);
        ////let checkbox2 = document.createElement("input");
        ////checkbox2.type = "checkbox";
        ////label2.appendChild(checkbox2);
        ////let back2 = _creaDiv("toggle-back");
        ////label2.appendChild(back2);
        ////let toggle2 = _creaDiv("toggle");
        ////let lbl2_on = _creaDiv("toggle-label on", "", "", "Irrigazioni registrate");
        ////let lbl2_off = _creaDiv("toggle-label off", "", "", "Irrigazioni da modello");
        ////back2.appendChild(toggle2);
        ////back2.appendChild(lbl2_on);
        ////back2.appendChild(lbl2_off);

        ////checkbox2.checked = plugin.indicatore.IsIrriUser;

        ////$(checkbox2).on('change', function () {

        ////    let value = this.checked;

        ////    if (plugin.indicatore.IsIrriUser === value) {
        ////        return;
        ////    }

        ////    plugin.indicatore.IsIrriUser = value;

        ////    plugin.run();
        ////});

        let irriOpts = [
            {
                text: "Irrigazioni registrate",
                value: Irrigazioni_GIAS
            },
            {
                text: "Bilancio ideale",
                value: Bilancio_IF
            }
        ];

        if (plugin.persistent) {
            irriOpts = [
                {
                    text: "Irrigazioni GIAS",
                    value: Irrigazioni_GIAS
                },
                {
                    text: "Irrigazioni IF",
                    value: Irrigazioni_IF
                },
                {
                    text: "Bilancio ideale",
                    value: Bilancio_IF
                }
            ];
        }

        let divFooter3 = _creaDiv("", "padding-top: 5px");
        elem.appendChild(divFooter3);
        $(divFooter3).MultiStateSwitch({
            options: irriOpts,
            onchange: function (state) {

                if (plugin.wbInput.IrriChoice === state) {
                    return;
                }

                plugin.wbInput.IrriChoice = state;

                plugin.run();
            }
        });

        $(divFooter3).data("MultiStateSwitch").setValue(plugin.wbInput.IrriChoice);

        let divBtnsDSS = _creaDiv("", "padding-top: 5px;margin: auto;text-align:center;");
        elem.appendChild(divBtnsDSS);

        /**
         * Btn Salva DSS Irrigazione
         */

        let btnSaveDSS = _creaBtnSaveDSS(divBtnsDSS, "Salva Consiglio");

        $(_getIdBtnSaveDSS(true)).kendoButton({
            click: function () { _checkDSS() },
            enable: false
        });

        /**
         * Btn Esporta Consiglio in PDF
         */

        let btnExportDSS = _creaBtnExportDSS(divBtnsDSS, "Stampa Consiglio");

        $(_getIdBtnExportDSS(true)).kendoButton({
            click: function () { _ExportDSS() },
            enable: false
        });

        let divlbl = _creaLabel();
        elem.appendChild(divlbl);

        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------

        plugin.run = function () {

            _disableBtnsDSS();

            if (_can_run()) {

                _remove_table(plugin.$wrapper.parents(".indic-elem"));
                _show();
            }
        };

        plugin.filter = function (vegArray) {

            if (vegArray.length === 0) {

                $(elem).removeClass("indic-filtered-out");

            } else {

                if (vegArray.find((cod) => parseInt(cod) === plugin.wbInput.Veg_Cod)) {

                    $(elem).removeClass("indic-filtered-out");

                } else {

                    $(elem).addClass("indic-filtered-out");
                    _remove_table(elem);
                }
            }
        };

        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------

        var _can_run = function () {

            let ris = true;
            let msg = "";
            let fa = "";
            if (plugin.wbInput.DataFaseStart === null) {

                ris = false;
                msg = "Data start non impostata";
                fa = "fa-calendar";

            } else {

                if (plugin.wbInput.Coord === null && !plugin.wbInput.UsaMeteo) {

                    ris = false;
                    msg = "Geolocalizzazione non disponibile";
                    //***APP*** msg = "Poligono impianto mancante";
                    fa = "fa-map-marker";
                }
            }

            if (!ris) {

                _clearElement(plugin.result);

                let divCont = _creaDiv("k-block k-error-colored", "height: 100%; text-align: center; display: grid; grid-template-rows: auto 1fr auto;");
                divCont.appendChild(_creaDiv("", "padding-top: 5px;", "", "<span class='fa fa-warning fa-2x'></span>"));
                divCont.appendChild(_creaDiv("", "align-self: center;", "", "<span>" + msg + "</span>"));
                let divIcon = _creaDiv();
                let stack = document.createElement("span");
                stack.className = "fa-stack fa-lg";
                let i0 = document.createElement("i");
                i0.className = "fa fa-square fa-stack-2x";
                i0.style.cssText = "margin: 0px;";
                let i1 = document.createElement("i");
                i1.className = "fa " + fa + " fa-stack-1x fa-inverse";
                i1.style.cssText = "margin: 0px;";
                stack.appendChild(i0);
                stack.appendChild(i1);
                divIcon.appendChild(stack);
                divCont.appendChild(divIcon);

                plugin.result.appendChild(divCont);
            }

            return ris;
        };

        var _impostaStart = function () {

            let divWin = _creaDiv("", "display: none;", "irri-win-data");

            plugin.wrapper.appendChild(divWin);

            let divWrapper = _creaDiv("", "display: flex; flex-flow: column; align-items: stretch; row-gap: 10px; padding: 5px;", "irri-win-data-wrapper");
            divWin.appendChild(divWrapper);

            let divCalendar = _creaDiv("", "align-self: center;", "irri-calendar");
            divWrapper.appendChild(divCalendar);

            $("#irri-calendar").kendoCalendar({
                value: new Date(),
                footer: false
            });

            let divMsg = _creaDiv("k-block k-info-colored", "padding: 5px 10px; text-align: justify; max-width: 25em;");
            divMsg.innerHTML = "Registrare data di Semina / Trapianto / Rilievo fase fenologica opportuna nel quaderno di campagna per impostare la data definitivamente.";
            divWrapper.appendChild(divMsg);

            let divBtnSet = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base", "", "", "<span>Imposta temporaneamente</span>");
            divWrapper.appendChild(divBtnSet);
            divBtnSet.onclick = function () { _actImpostaStart(false); };

            let $kendomodal = $("#irri-win-data");

            $kendomodal.kendoWindow({
                title: "Imposta data start",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                    $("body").addClass("overflow_hidden");
                    $("#irri-win-data-wrapper").css("opacity", "0");
                },
                activate: function () {
                    $("#irri-win-data-wrapper").css("opacity", "1");
                },
                close: function (e) {
                    $("body").removeClass("overflow_hidden");
                    $kendomodal.data("kendoWindow").destroy();
                }
            });
            $kendomodal.data("kendoWindow").center().open();
        };

        var _actImpostaStart = function (flagRec) {

            let calendar = $("#irri-calendar").data("kendoCalendar");

            let dt = new Date(calendar.value());
            dt.setHours(0, 0, 0, 0);

            //todo: verificare validità data

            if (flagRec) {
                //todo: chiamare WS per registrazione semina/trapianto/inizio fase vegetativa
            }

            plugin.wbInput.DataFaseStart = dt;

            _sayStart();

            let win = $("#irri-win-data").data("kendoWindow");

            win.close();

            plugin.run();
        };

        var _geolocalizza = function () {

            if (plugin.wbInput.Indirizzo.length === 0) {
                return;
            }

            let geocoder = new google.maps.Geocoder;

            geocoder.geocode({
                address: plugin.wbInput.Indirizzo
            },
                function (results, status) {

                    if (status === google.maps.GeocoderStatus.OK) {

                        let l = results[0].geometry.location;

                        if (l.lat() !== "" && l.lng() !== "") {

                            plugin.wbInput.Coord = {
                                Lat: l.lat(),
                                Lng: l.lng()
                            };

                            _sayLatLng();

                            //if (!plugin.indicatore.UsaMeteo) {
                            //  plugin.run();
                            //}

                            plugin.run();
                        }
                    } else {
                        console.log("Geocode was not successful for the following reason: " + status);
                    }
                }
            );
        };

        var _show_table = function () {

            if (plugin.table === undefined) {
                return;
            }

            let grid_elem = plugin.$wrapper.parents(".indic-elem");
            if ($(grid_elem).hasClass("indic-elem-shown")) {
                return;
            }

            let unique_id = 1;
            $.each(document.getElementsByClassName("irri-tabs"), function (idx, elem) {
                unique_id = Math.max(parseInt($(elem).attr("data-id")) + 1, unique_id);
            });
            let divWinId = "win-show-" + unique_id;

            $(grid_elem).toggleClass("k-shadow");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            //$(grid_elem).toggleClass("k-inset");
            $(grid_elem).addClass("indic-elem-shown");
            $(grid_elem)[0].setAttribute("data-win-id", divWinId);

            let insert_after = grid_elem;
            let all_elem = grid_elem.parent().children();
            if (all_elem.length > 1) {
                let top = grid_elem.position().top;
                let idx = 0;
                let found = false;
                while (idx < all_elem.length && !found) {
                    if ($(all_elem[idx]).position().top > top) {
                        found = true;
                    } else {
                        insert_after = $(all_elem[idx]);
                    }
                    idx++;
                }
            }

            insert_after = insert_after[0];
            while ($(insert_after.nextSibling).hasClass("grid-span-all")) {
                insert_after = insert_after.nextSibling;
            }

            let divWin = _creaDiv("k-block k-shadow grid-span-all", "background-color: #fff;", divWinId);
            insert_after.parentNode.insertBefore(divWin, insert_after.nextSibling);

            let title = plugin.header + " (" + plugin.coltura + ")";
            let divHdr = _creaDiv("k-header");
            let divHdrCont = _creaDiv("", "display: flex; align-items: center;");
            let divTitle = _creaDiv("", "width: 100%; padding: 4px 8px; font-size: large;", "", title);
            let divClose = _creaDiv("k-button k-button-md k-rounded-md k-button-flat k-button-flat-base k-icon-button k-window-action", "", "", "<span class='k-button-icon k-icon k-i-close' style='font-size: large;'></span>");
            divClose.onclick = function () {
                _remove_table(grid_elem);
            };

            divHdrCont.appendChild(divTitle);
            divHdrCont.appendChild(divClose);
            divHdr.appendChild(divHdrCont);
            divWin.appendChild(divHdr);

            let divBody = _creaDiv("", "padding: 8px;");
            divWin.appendChild(divBody);

            let unique_id_tabs = "irri-tabs-" + unique_id;
            let unique_id_chart = "irri-kendo-chart-" + unique_id;
            let unique_id_grid = "irri-kendo-grid-" + unique_id;

            let divTabs = _creaDiv("irri-tabs", "background-color:#fff; box-shadow:none;", unique_id_tabs);
            divTabs.setAttribute("data-id", unique_id);
            divBody.appendChild(divTabs);

            unique_id_tabs = "#" + unique_id_tabs;
            $(unique_id_tabs).kendoTabStrip({
                animation: false,
                scrollable: false
            });

            let tabstrips = $(unique_id_tabs).data("kendoTabStrip");

            $.each([{ text: "Grafico", id: unique_id_chart }, { text: "Tabella", id: unique_id_grid }], function (index, obj) {
                tabstrips.append({
                    text: obj.text,
                    content: "<div style='overflow:hidden;'><div id='" + obj.id + "'></div></div>"
                });
            });

            tabstrips.tabGroup.css("font-size", "larger");

            tabstrips.select(0);

            _creaGrid(unique_id_grid, plugin.table);
            unique_id_grid = "#" + unique_id_grid;

            let grid_ds = $(unique_id_grid).data("kendoGrid").dataSource.data();

            _creaChart(unique_id_chart, grid_ds, plugin.table.kendo_columns);
            unique_id_chart = "#" + unique_id_chart;

            tabstrips.bind("activate", function (e) {
                if ($(e.contentElement).index() === 2) {

                    let chartH = 0;
                    $(unique_id_chart + " .chart-wrapper").each(function () {
                        chartH += $(this).outerHeight() + $(this).find(".k-chart").height();
                    });
                    let hdrH = $(unique_id_grid + " .k-grid-header").outerHeight();
                    let btw = parseFloat($(unique_id_grid).css("border-top-width"));
                    let bbw = parseFloat($(unique_id_grid).css("border-bottom-width"));

                    $(unique_id_grid + " .k-grid-content").css("height", (chartH - hdrH - btw - bbw) + "px");

                    tabstrips.unbind("activate");
                }
            });

            divWin.scrollIntoView();
        };

        var _remove_table = function (elem) {

            if (!$(elem).hasClass("indic-elem-shown")) {
                return;
            }

            let divWinId = $(elem).attr("data-win-id");
            let divWin = $("#" + divWinId);
            if (divWin.length > 0) {
                divWin.remove();
            }

            $(elem).removeClass("indic-elem-shown");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            $(elem).toggleClass("k-shadow");
            //$(elem).toggleClass("k-inset");
        };

        var _creaGrid = function (div_id, kendodata) {

            let objConfig = {
                dataSource: {
                    data: kendodata.kendo_rows,
                    schema: {
                        model: {
                            fields: kendodata.kendo_model
                        }
                    }
                },
                columns: kendodata.kendo_columns,
                resizable: true,
                pageable: false
            };

            $("#" + div_id).kendoGrid(objConfig);
        };

        var _creaChart = function (div_id, ds, columns) {

            let hasUmTerr = columns.findIndex((col)=> col.field.startsWith("UT_")) >= 0;

            let chartWrapper = document.getElementById(div_id);

            let style = "";
            if (hasUmTerr) {
                style = "padding-bottom: 10px;";
            }
            let chartWrapper1 = _creaDiv("chart-wrapper", style);

            let chart_id_1 = div_id + "-1";
            let chart1 = _creaDiv("", "", chart_id_1);
            chartWrapper1.appendChild(chart1);

            chartWrapper.appendChild(chartWrapper1);

            let max_perc = 100;
            let chart_ds = ds;
            if (plugin.rundate !== undefined) {

                chart_ds = ds.map(function (x) {

                    max_perc = Math.max(max_perc, x.Umid, x.SoSup);

                    if (x.Data >= plugin.rundate) {

                        x.Umid_Post = x.Umid;
                        x.SoInf_Post = x.SoInf;
                        x.SoSup_Post = x.SoSup;
                        x.SoInfPerc_Post = x.SoInfPerc;
                        x.SoSupPerc_Post = x.SoSupPerc;

                        if (x.Data > plugin.rundate) {

                            delete x.Umid;
                            delete x.SoInf;
                            delete x.SoSup;
                            delete x.SoInfPerc;
                            delete x.SoSupPerc;

                        } else {

                            x.DataCalcolo = 0;

                        }
                    }

                    return x;
                });
            }

            max_perc = (Math.floor(max_perc / 10) + 1) * 10;

            let vertAxes = [
                {
                    title: {
                        text: "Valori di bilancio in % di acqua disponibile"
                    },
                    min: -10,
                    max: max_perc,
                    axisCrossingValue: Number.NEGATIVE_INFINITY,
                    plotBands: [
                        {
                            from: -10,
                            to: 0,
                            color: "red",
                            opacity: 0.3
                        },
                        {
                            from: 100,
                            to: max_perc,
                            color: "green",
                            opacity: 0.3
                        }
                    ]
                },
                {
                    name: "Apporti_Axis",
                    title: {
                        text: "Apporti in mm"
                    }
                }
            ];

            let series = [
                {
                    name: "Umidità terreno",
                    field: "Umid",
                    color: "rgb(255, 140, 0)",
                    gruppo: 1
                },
                {
                    name: "Soglia di intervento",
                    field: "SoInf",
                    color: "rgb(178, 34, 34)",
                    gruppo: 2
                },
                {
                    name: "Soglia superiore",
                    field: "SoSup",
                    color: "rgb(0, 128, 0)",
                    gruppo: 3
                },
                {
                    name: "Piogge",
                    type: "column",
                    field: "Prec",
                    gap: 1,
                    spacing: 0,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 128, 255)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    name: "Irrigazioni",
                    type: "column",
                    field: "Irri",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    field: "Umid_Post",
                    dashType: "dash",
                    gruppo: 1
                },
                {
                    field: "SoInf_Post",
                    dashType: "dash",
                    gruppo: 2
                },
                {
                    field: "SoSup_Post",
                    dashType: "dash",
                    gruppo: 3
                },
                {
                    field: "IrriPrev",
                    type: "column",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    opacity: 0.5,
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                }
            ];

            //-------------------------------------------------------------------------------------
            // oggetto per controllare una riga verticale
            // posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
            //-------------------------------------------------------------------------------------
            let horzAxisCfg = {
                field: "Data",
                vertIndicator: {
                    field: "DataCalcolo",
                    label: {
                        text: "Data Calcolo",
                        color: "#808080" //"#800080"
                    },
                    line: {
                        color: "#808080",
                        width: 1,
                        dashType: "longDash"
                    }
                }
            };

            let titolo = "Bilancio idrico Irriframe";
            if (!plugin.wbInput.UsaMeteo &&
                typeof plugin.wbInput.InfoProvider === "string" &&
                plugin.wbInput.InfoProvider !== "") {

                titolo += " - " + plugin.wbInput.InfoProvider;                
            }

            creaKendoChart(titolo, horzAxisCfg, vertAxes, series, chart_ds, chart_id_1);

            if (hasUmTerr) {

                let chartWrapper2 = _creaDiv("chart-wrapper", "padding-top: 10px;");
                let chart_id_2 = div_id + "-2";
                let chart2 = _creaDiv("", "", chart_id_2);

                chartWrapper2.appendChild(chart2);

                chartWrapper.appendChild(chartWrapper2);

                let vertAxes2 = [
                    {
                        title: {
                            text: "Umidità terreno (%)"
                        }//,
                        //min: -10,
                        //max: max_perc,
                        //axisCrossingValue: Number.NEGATIVE_INFINITY,
                        //plotBands: [
                        //    {
                        //        from: -10,
                        //        to: 0,
                        //        color: "red",
                        //        opacity: 0.3
                        //    },
                        //    {
                        //        from: 100,
                        //        to: max_perc,
                        //        color: "green",
                        //        opacity: 0.3
                        //    }
                        //]
                    }//,
                    //{
                    //    name: "Apporti_Axis",
                    //    title: {
                    //        text: "Apporti in mm"
                    //    }
                    //}
                ];

                let series2 = [
                    {
                        name: "Soglia inferiore",
                        field: "SoInfPerc",
                        color: "rgb(178, 34, 34)",
                        gruppo: 1
                    },
                    {
                        name: "Soglia superiore",
                        field: "SoSupPerc",
                        color: "rgb(0, 128, 0)",
                        gruppo: 2
                    },
                    //{
                    //    name: "Piogge",
                    //    type: "column",
                    //    field: "Prec",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 128, 255)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    name: "Irrigazioni",
                    //    type: "column",
                    //    field: "Irri",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 206, 209)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    field: "Umid_Post",
                    //    dashType: "dash",
                    //    gruppo: 1
                    //},
                    {
                        field: "SoInfPerc_Post",
                        dashType: "dash",
                        gruppo: 1
                    },
                    {
                        field: "SoSupPerc_Post",
                        dashType: "dash",
                        gruppo: 2
                    }
                ];

                let gruppo = 3;

                $.each(columns, function (i, e) {
                    if (e.field.startsWith("UT_")) {
                        series2.splice(0, 0,
                            {
                                name: e.title,
                                field: e.field,
                                //color: "rgb(255, 140, 0)",
                                gruppo: gruppo++
                            }
                        );
                    }
                });

                ////-------------------------------------------------------------------------------------
                //// oggetto per controllare una riga verticale
                //// posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
                ////-------------------------------------------------------------------------------------
                //let horzAxisCfg = {
                //    field: "Data",
                //    vertIndicator: {
                //        field: "DataCalcolo",
                //        label: {
                //            text: "Data Calcolo",
                //            color: "#808080", //"#800080",
                //        },
                //        line: {
                //            color: "#808080",
                //            width: 1,
                //            dashType: "longDash"
                //        }
                //    }
                //};

                //let titolo = "Bilancio idrico Irriframe";
                //if (typeof plugin.indicatore.InfoProvider === "string") {
                //    if (plugin.indicatore.InfoProvider !== "") {

                //        titolo += " - ";
                //        titolo += plugin.indicatore.InfoProvider;
                //    }
                //}

                creaKendoChart("", "Data", vertAxes2, series2, chart_ds, chart_id_2);
            }
        };

        var _showLoader = function () {

            _clearElement(plugin.result);

            let divRel = _creaDiv("k-block", "position: relative; width: 100%; height: 100%;");
            plugin.result.appendChild(divRel);
            $(divRel).StyleLoader();
        };

        var _creaAdv = function (msg, bkg, clr) {

            let advStyle = "border-radius: 5px; border: 1px solid #aeaeae; display: flex;";
            if (bkg !== "") {
                advStyle += " background-color: " + bkg + ";";
            }
            if (clr !== "") {
                advStyle += " color: " + clr + ";";
            }

            let divAdv = _creaDiv("", advStyle);
            let divMsg = _creaDiv("", "width: 100%; align-self: center; text-align: center;", "", msg);
            divAdv.appendChild(divMsg);

            return divAdv;
        };

        var _show = function () {

            _showLoader();

            let url = window.location.href.split("?")[0] + "/CalcolaWBResult_V2";
            ajaxAgronica(url,
                JSON.stringify({ input: plugin.wbInput }),
                function (risposta) {

                    let res = JSON.parse(risposta.RispostaStringa);
                    if (res.Status === 0) {

                        if (res.Result.IrriframeData &&
                            res.Result.IrriframeData.PlotId > 0 &&
                            res.Result.IrriframeData.CropId > 0) {
                            plugin.wbInput.PersistData.Irri_PlotId = res.Result.IrriframeData.PlotId;
                            plugin.wbInput.PersistData.Irri_CropId = res.Result.IrriframeData.CropId;
                        }

                        if (res.Result.TurniIrriguiPrevisti !== null && res.Result.TurniIrriguiPrevisti.length > 0) {
                            plugin.ShowTurniIrrigui = true;
                        }

                        _showResult(res.Result);

                    } else {

                        _showError(res.Message);
                    }

                },
                function (risposta) {
                    console.log("ERRORE DSS Irrigazione (WBResult_V2)");
                },
                null,
                false
            );
        };

        var _showResult = function (res) {

            _clearElement(plugin.result);

            plugin.wbInput.InfoProvider = res.InfoProvider;
            plugin.wbInput.IrrigazionePrevista = null;
            plugin.wbInput.TurniIrriguiPrevisti = null;

            let msgIrri = "NO";
            let msgData = "";
            let msgVol = "";
            let msgCons = "";

            let dataEsecuzione = Date.parseLocale(res.DataEsecuzione);

            if (!isNaN(parseFloat(res.ConsumoMM))) {
                msgCons = kendo.toString(res.ConsumoMM, "0.00") + " mm";
            }

            let arrAdv = [
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" }
            ];

            let hasTurniIrrigui = res.TurniIrriguiPrevisti !== undefined && res.TurniIrriguiPrevisti !== null && typeof res.TurniIrriguiPrevisti === "object" && res.TurniIrriguiPrevisti.length > 0;

            if (typeof res.IrrigazioniPreviste === "object") {

                if (res.IrrigazioniPreviste.length > 0) {

                    msgIrri = "SI";

                    let idx = 0;
                    let cnt = Math.min(arrAdv.length - 1, res.IrrigazioniPreviste.length);

                    while (idx < cnt) {

                        let irri = res.IrrigazioniPreviste[idx];
                        let dt = new Date(irri.Data);
                        dt.setHours(0, 0, 0, 0);

                        if (plugin.wbInput.IrrigazionePrevista === null) {
                            plugin.wbInput.IrrigazionePrevista = {
                                Data: dt,
                                VolumeMM: irri.VolumeMM
                            }

                            _enableBtnsDSS();
                        }

                        if (idx === 0) {
                            msgData = kendo.toString(dt, "ddd dd-MM"); //VEN 27-03-2020
                            msgVol = kendo.toString(irri.VolumeMM, "0.00") + " mm";
                        }

                        arrAdv[idx + 1].msg = kendo.toString(dt, "dd/MM");

                        let diff_days = Math.floor((dt.getTime() - dataEsecuzione.getTime()) / (1000 * 60 * 60 * 24));
                        if (diff_days <= 3) {
                            arrAdv[idx + 1].bkg = "#f00000"; //rosso
                            arrAdv[idx + 1].clr = "#ffffff";
                        } else if (diff_days <= 7) {
                            arrAdv[idx + 1].bkg = "#fff000"; //giallo   "#ff8000" 'arancio
                        } else {
                            arrAdv[idx + 1].bkg = "#008000"; //verde
                            arrAdv[idx + 1].clr = "#ffffff";
                        }

                        idx++;
                    }
                }
            }

            if (hasTurniIrrigui) {

                if (plugin.wbInput.TurniIrriguiPrevisti === null) {
                    plugin.wbInput.TurniIrriguiPrevisti = [];

                    for (let i = 0; i < res.TurniIrriguiPrevisti.length; i++) {
                        plugin.wbInput.TurniIrriguiPrevisti.push({
                            Data: new Date(res.TurniIrriguiPrevisti[i].Data),
                            DurataM: res.TurniIrriguiPrevisti[i].DurataM,
                            VolumeMM: res.TurniIrriguiPrevisti[i].VolumeMM
                        })
                    }
                }
            }

            if (typeof res.Table === "string" && res.Table !== "") {

                //let objTable = JSON.parse(res.Table);
                let objTable = (hasTurniIrrigui && plugin.ShowTurniIrrigui && typeof res.TableTurni === "string" && res.TableTurni !== "") ? JSON.parse(res.TableTurni) : JSON.parse(res.Table);

                if (typeof objTable.kendo_rows === "object" && objTable.kendo_rows.length > 0) {

                    plugin.table = objTable;

                    //ultima irrigazione effettuata...
                    let lastIdx = plugin.table.kendo_rows.findLastIndex((row) => row.Irri > 0);
                    if (lastIdx >= 0) {

                        let dt = kendo.parseDate(plugin.table.kendo_rows[lastIdx].Data);
                        arrAdv[0].msg = kendo.toString(dt, "dd/MM");
                        arrAdv[0].bkg = "#00ced1"; //"#00bfff"; //
                        arrAdv[0].clr = "#ffffff";
                        if (dt.getTime() > dataEsecuzione.getTime()) {
                            //Irrigazione registrata nel futuro... (Devo farlo notare???)
                        }
                    }
                }
            }

            //********************************************************************
            //********************************************************************
            //********************************************************************
            // Per il pulsante Salva Irrigazione
            //********************************************************************
            //********************************************************************
            //********************************************************************
            //let divContent = _creaDiv("", "height: 100%; display: grid; grid-gap: 3px; grid-template-rows: 2.5em auto 2.5em;");
            let divContent = _creaDiv("", "height: 100%; display: grid; grid-gap: 3px; grid-template-rows: 2.5em auto;");
            //********************************************************************
            //********************************************************************
            //********************************************************************

            let divRow1 = _creaDiv("", "height: 2.5em; display: grid; grid-template-columns: 1fr auto 1fr 1fr; grid-gap: 3px;");

            divRow1.appendChild(_creaAdv(arrAdv[0].msg, arrAdv[0].bkg, arrAdv[0].clr));
            divRow1.appendChild(_creaDiv("", "font-size: 6px; align-self: center;", "", "<span class='fa fa-circle' style='margin: 0px;'></span>"));
            divRow1.appendChild(_creaAdv(arrAdv[1].msg, arrAdv[1].bkg, arrAdv[1].clr));
            divRow1.appendChild(_creaAdv(arrAdv[2].msg, arrAdv[2].bkg, arrAdv[2].clr));

            let divCheckPartiz = null;
            if (hasTurniIrrigui) {

                divCheckPartiz = _creaDiv();
                divCheckPartiz.innerHTML = "<input type=\"checkbox\" value=\"\" id=\"chkPartizTurni\">";
            }

            let divRow2 = _creaDiv("k-block k-info-colored ripple-cont", "padding: 3px; font-size: smaller; display: grid; grid-template-rows: 1fr 1fr 1fr 1fr; align-items: center; ");
            
            if (hasTurniIrrigui && plugin.ShowTurniIrrigui) {

                divRow2.style.maxHeight = "200px";
                divRow2.style.overflowY = "scroll";

                for (let i = 0; i < res.TurniIrriguiPrevisti.length; i++) {
                    let msgTurnoData = kendo.toString(new Date(res.TurniIrriguiPrevisti[i].Data), "g")
                    let msgTurnoDurata = kendo.toString(res.TurniIrriguiPrevisti[i].DurataM, "n0") + " minuti"
                    let divTurno = _creaDiv();
                    divTurno.innerHTML = "<span>TURNO:</span><span style='float: right; font-weight: bold;'>" + msgTurnoData + "</span><br>" +
                        "<span>DURATA:</span><span style='float: right; font-weight: bold;'>" + msgTurnoDurata + "</span><br><br>";
                    divRow2.appendChild(divTurno);
                }
            } else {

                let divIrri = _creaDiv();
                divIrri.innerHTML = "<span>IRRIGAZIONE:</span><span style='float: right; font-weight: bold;'>" + msgIrri + "</span>";
                let divData = _creaDiv();
                divData.innerHTML = "<span>DATA:</span><span style='float: right; font-weight: bold;'>" + msgData + "</span>";
                let divVol = _creaDiv();
                divVol.innerHTML = "<span>VOLUME:</span><span style='float: right; font-weight: bold;'>" + msgVol + "</span>";
                let divCons = _creaDiv();
                divCons.innerHTML = "<span>CONSUMO:</span><span style='float: right; font-weight: bold;'>" + msgCons + "</span>";
                divRow2.appendChild(divIrri);
                divRow2.appendChild(divData);
                divRow2.appendChild(divVol);
                divRow2.appendChild(divCons);
            }

            //********************************************************************
            //********************************************************************
            //********************************************************************
            //let divRow3 = _creaDiv("", "display: flex;");
            ////let btnReg = _creaDiv("k-button indic-tooltip", "margin-right: 2px; flex: auto;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            //let btnReg = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button indic-tooltip", "flex: auto; background-color: #f5f5f5;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            //btnReg.setAttribute("data-tooltip", "Registra irrigazione");
            //divRow3.appendChild(btnReg);

            ////let btnPia = _creaDiv("k-button indic-tooltip", "margin-left: 2px; flex: auto;", "", "<span class='fa fa-calendar fa-lg'></span>");
            ////btnPia.setAttribute("data-tooltip", "Pianifica");
            ////divRow3.appendChild(btnPia);
            //********************************************************************
            //********************************************************************
            //********************************************************************

            divContent.appendChild(divRow1);
            if (divCheckPartiz !== null) {
                divContent.appendChild(divCheckPartiz);
            }
            divContent.appendChild(divRow2);
            //********************************************************************
            //********************************************************************
            //********************************************************************
            //divContent.appendChild(divRow3);
            //********************************************************************
            //********************************************************************
            //********************************************************************

            plugin.result.appendChild(divContent);

            if (divCheckPartiz !== null) {
                $("#chkPartizTurni").kendoCheckBox({
                    label: "Visualizza turni",
                    checked: (plugin.ShowTurniIrrigui === true),
                    change: function (e) {
                        plugin.ShowTurniIrrigui = e.checked;
                        _showResult(res);
                    }
                });
            }  

            if (plugin.table !== undefined) {

                if (res.DataEsecuzione !== undefined) {

                    plugin.rundate = new Date(res.DataEsecuzione);
                    plugin.rundate.setHours(0, 0, 0, 0);
                }

                let ripple_cont = $(plugin.result).find(".ripple-cont");
                if (ripple_cont.length > 0) {

                    ripple_cont.addClass("irri-ripple");
                    ripple_cont[0].onclick = _show_table;
                    ripple_cont[0].insertBefore(_creaDiv("ripple"), ripple_cont[0].firstChild);
                }
            }          

            //if (plugin.wbInput.IrrigazionePrevista !== null) {

                //********************************************************************
                //********************************************************************
                //********************************************************************
                //btnReg.onclick = function () {
                //    //_openIrriWin();
                //    _querySaveAdvice();
                //};
                //********************************************************************
                //********************************************************************
                //********************************************************************
            //} else {

                //********************************************************************
                //********************************************************************
                //********************************************************************
                //$(btnReg).addClass(GIAS_K_STATE_DISABLED);
                //********************************************************************
                //********************************************************************
                //********************************************************************
            //}
        };



        var _showError = function (msg) {

            _clearElement(plugin.result);

            let divMsg = _creaDiv("k-block", "display: flex; height: 100%; justify-content: center; text-align: center; background-color: #f9f9f9; color: #aeaeae;");
            let span = document.createElement("span");
            span.style.cssText = "align-self: center;";
            span.innerHTML = msg;
            divMsg.appendChild(span);
            plugin.result.appendChild(divMsg);

            $(divMsg).on("click", function (e) {
                if (e.ctrlKey) {
                    _impostaRunData();
                }
            });
        };

        var _querySaveAdvice = function () {

        //    if (plugin.indicatore.IrrigazionePrevista === null) {
        //        //se non ho irrigazioni previste non devo salvare
        //        return;
        //    }

        //    let queryGIAS = true;
        //    let queryIF = true;

        //    if (!plugin.persistent) {

        //        queryIF = false;

        //    } else {

        //        if (typeof plugin.indicatore.PersistData !== "object") {
        //            //se non ho la chiave irriframe non so dove salvare l'irrigazione
        //            return;
        //        }

        //        if (plugin.indicatore.PersistData.Irri_PlotId <= 0 || plugin.indicatore.PersistData.Irri_CropId <= 0) {
        //            //se non ho la chiave irriframe non so dove salvare l'irrigazione in Irriframe

        //            queryIF = false;

        //        }
        //    }

        //    let win_el = document.createElement("div");
        //    document.body.appendChild(win_el);
        //    let $win_el = $(win_el);

        //    let content = "<div style='display: grid; grid-template-columns: auto 1fr; gap: 5px 10px;'>";
        //    content += "<div>Data</div>";
        //    content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.Data, "dddd d MMMM") + "</div>";
        //    content += "<div>Volume</div>";
        //    content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.VolumeMM, "0.00") + " mm</div>";
        //    content += "</div>";
        //    if (queryGIAS && queryIF) {

        //        content += "<div style='margin-top: 20px;'>";
        //        content += "    <div style='padding-bottom: 5px;'>";
        //        content += "        <input type='checkbox' id='__chk_gias' class='k-checkbox' style='margin: 0px; outline: none;'>";
        //        content += "        <label class='k-checkbox-label' for='__chk_gias'>Registra irrigazione in GIAS</label>";
        //        content += "    </div>";
        //        content += "    <div style='padding-top: 5px;'>";
        //        content += "        <input type='checkbox' id='__chk_if' class='k-checkbox' style='margin: 0px; outline: none;'>";
        //        content += "        <label class='k-checkbox-label' for='__chk_if'>Registra irrigazione in Irriframe</label>";
        //        content += "    </div>";
        //        content += "</div>";
        //    }

        //    $win_el.kendoDialog({
        //        title: "Registrazione irrigazione",
        //        closable: false,
        //        modal: true,
        //        visible: false,
        //        content: content,
        //        //open: function () {
        //        //},
        //        actions: [
        //            {
        //                text: "OK",
        //                action: function (e) {

        //                    if (queryGIAS && queryIF) {

        //                        let chk_gias = $("#__chk_gias").prop("checked");
        //                        let chk_if = $("#__chk_if").prop("checked");

        //                        if (!chk_gias && !chk_if) {
        //                            return false;
        //                        }

        //                        _saveAdvice(chk_gias, chk_if);

        //                    } else {

        //                        _saveAdvice(queryGIAS, queryIF);
        //                    }
        //                    return true;
        //                }
        //            },
        //            {
        //                text: "Annulla"
        //            }
        //        ],
        //        close: function (e) {
        //            this.destroy();
        //        }
        //    });

        //    $win_el.data("kendoDialog").open();
        };

        var _saveAdvice = function (save_gias, save_if) {

        //    let irri = {
        //        Data: plugin.indicatore.IrrigazionePrevista.Data,
        //        VolumeMM: plugin.indicatore.IrrigazionePrevista.VolumeMM
        //    };

        //    if (save_gias) {

        //        irri.GIAS_key = {
        //            PIVA: plugin.indicatore.PersistData.GIAS_Piva,
        //            SaCod: plugin.indicatore.PersistData.GIAS_SaCod,
        //            Appezza: plugin.indicatore.PersistData.GIAS_Appezza,
        //            IdReg: plugin.indicatore.PersistData.GIAS_IdReg,
        //            VegCod: plugin.indicatore.Veg_Cod,
        //            CulCod: plugin.indicatore.Cul_Cod,
        //            SupImp: plugin.indicatore.Sup_Imp
        //        };
        //    }

        //    if (save_if) {

        //        irri.IF_key = {
        //            IdPlot: plugin.indicatore.PersistData.Irri_PlotId,
        //            IdCrop: plugin.indicatore.PersistData.Irri_CropId
        //        };
        //    }

        //    $.ajax({
        //        async: false,
        //        type: "POST",
        //        url: location.pathname + "/SalvaIrrigazione",
        //        data: JSON.stringify({ irri: irri }),
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (msg) {

        //            if (msg.d.RispostaOK) {

        //                let objRisp = JSON.parse(msg.d.RispostaStringa);

        //                if (objRisp.GIAS_Saved) {
        //                    plugin.indicatore.Irrigazioni.push(objRisp.Irrigazione);
        //                }

        //                if (objRisp.GIAS_Saved || objRisp.IF_Saved) {
        //                    //Ricalcolo il consiglio irriguo
        //                    _show();
        //                }
        //            }
        //        },
        //        error: function (xhr, ajaxOptions, thrownError) {
        //            //alert(xhr.status);
        //            //alert(thrownError);
        //        }
        //    });
        };

        var _openIrriWin = function () {

        //    $.ajax({
        //        async: false,
        //        type: "POST",
        //        url: location.pathname + "/PreparaPaginaIrrigazione",
        //        data: "{}",
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (msg) {

        //            if (msg.d == 'SessioneScaduta') {

        //                $('#dialogSessioneScaduta').dialog("open");
        //            }
        //            else {

        //                if (msg.d.RispostaOK) {

        //                    let winElem = _creaDiv("", "padding:0px;");
        //                    document.body.appendChild(winElem);

        //                    let frameH = window.innerHeight * 0.9;
        //                    let frameElem = document.createElement("iframe");
        //                    frameElem.id = "Irri_IFrame";
        //                    frameElem.style.cssText = "width:-webkit-fill-available; height:" + frameH + "px; opacity:0;";
        //                    frameElem.setAttribute('frameborder', '0');
        //                    frameElem.setAttribute("src", "");
        //                    winElem.appendChild(frameElem);

        //                    let $win_el = $(winElem);

        //                    $win_el.kendoWindow({
        //                        title: "Registra irrigazione",
        //                        width: "90%",
        //                        draggable: false,
        //                        visible: false,
        //                        modal: true,
        //                        resizable: false,
        //                        actions: [
        //                            "Close"
        //                        ],
        //                        open: function (e) {
        //                            e.sender.element.css("opacity", "0");
        //                            //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
        //                            $("body").addClass("overflow_hidden");
        //                        },
        //                        activate: function (e) {
        //                            e.sender.element.css("opacity", "1");
        //                            $("#Irri_IFrame").css("opacity", "1");
        //                        },
        //                        close: function (e) {
        //                            $("body").removeClass("overflow_hidden");
        //                            this.destroy();
        //                        }
        //                    });

        //                    //let parent = $win_el.parent();
        //                    //parent.find('.k-window-title').css('text-align', 'center');
        //                    //parent.css('padding-top', '48px');
        //                    //let titlebar = parent.find('.k-window-titlebar');
        //                    //titlebar.css({
        //                    //    "margin-top": "-48px",
        //                    //    "height": "35px",
        //                    //    "line-height": "35px",
        //                    //    "vertical-align": "middle"
        //                    //});

        //                    frameElem.setAttribute("src", msg.d.RispostaStringa);

        //                    $win_el.data("kendoWindow").center().open();
        //                }
        //            }
        //        },
        //        error: function (xhr, ajaxOptions, thrownError) {
        //            alert(xhr.status);
        //            alert(thrownError);
        //        }
        //    });
        };

        var _impostaRunData = function () {

            let divWin = _creaDiv("", "display: none;");
            plugin.wrapper.appendChild(divWin);
            let divContainer = _creaDiv("", "padding: 10px;")
            divWin.appendChild(divContainer);
            let divCalendar = _creaDiv();
            divContainer.appendChild(divCalendar);
            let divBtns = _creaDiv("", "display: flex; gap: 5px; padding: 0px 10px 10px;");
            divWin.appendChild(divBtns);
            let divBtnOk = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#5cb85c;", "", "<span class='fa fa-check fa-lg'></span>");
            divBtns.appendChild(divBtnOk);
            let divBtnCancel = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#d9534f;", "", "<span class='fa fa-times fa-lg'></span>");
            divBtns.appendChild(divBtnCancel);

            $(divCalendar).kendoCalendar({
                value: new Date(),
                footer: false
            });

            $(divWin).kendoWindow({
                title: "Imposta data esecuzione",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                actions: []
            });

            let kendoCalendar = $(divCalendar).getKendoCalendar();

            let kendoWindow = $(divWin).getKendoWindow();

            kendoWindow.bind("open", function (e) {
                //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                $("body").addClass("overflow_hidden");
            });
            kendoWindow.bind("close", function (e) {
                $("body").removeClass("overflow_hidden");
                kendoWindow.destroy();
            });

            kendoWindow.center().open();

            divBtnOk.onclick = function () {
                let dt = new Date(kendoCalendar.value());

                kendoWindow.close();

                dt.setUTCHours(0, 0, 0, 0);
                plugin.wbInput.RunData = dt.toISOString().split("T")[0];
                plugin.run();
            };

            divBtnCancel.onclick = function () {
                kendoWindow.close();
            };
        };

        var _saveDSS = function () {

            if (plugin.wbInput.TurniIrriguiPrevisti !== undefined && plugin.wbInput.TurniIrriguiPrevisti !== null && plugin.wbInput.TurniIrriguiPrevisti.length > 0) {

                const url = window.location.href.split("?")[0] + "/SalvaDSSIrrigazioneConTurni";
                
                ajaxAgronica(url,
                    JSON.stringify({ irri: _generateIrri2SaveObj(true, true), irriTurni: _generateIrriTurni2SaveObj() }),
                    function (risposta) {
                        MessaggioTuttoOK_Bootstrap("Consiglio Salvato", "DIV_Messaggi");
                    },
                    function (risposta) {
                        console.log("ERRORE DSS Irrigazione (SalvaDSSIrrigazioneConTurni)");
                    },
                    null,
                    true
                );
            }
            else {

                const url = window.location.href.split("?")[0] + "/SalvaDSSIrrigazione";
                
                ajaxAgronica(url,
                    JSON.stringify({ irri: _generateIrri2SaveObj(true, true) }),
                    function (risposta) {
                        MessaggioTuttoOK_Bootstrap("Consiglio Salvato", "DIV_Messaggi");
                    },
                    function (risposta) {
                        console.log("ERRORE DSS Irrigazione (SalvaDSSIrrigazione)");
                    },
                    null,
                    true
                );
            }

        };

        var _checkDSS = function () {

            if (plugin.wbInput.IrrigazionePrevista !== undefined && plugin.wbInput.IrrigazionePrevista !== null) {

                const url = window.location.href.split("?")[0] + "/ControllaDSSIrrigazione";

                ajaxAgronica(url,
                    JSON.stringify({ irri: _generateIrri2SaveObj(true, false) }),
                    function (risposta) {

                        if (risposta.RispostaOK) {
                            if (parseInt(risposta.RispostaStringa) > 0) {

                                const impianto_des = plugin.wbInput.Decription;

                                const runData = new Date(plugin.rundate).toLocaleDateString();

                                const message = "Consiglio già presente per l'Impianto " + impianto_des + " con Data Esecuzione " + runData + ", procedere comunque con il salvataggio?";

                                kendo.confirm(message).then(function () {
                                    _saveDSS();
                                }, function () {

                                });

                            } else {
                                _saveDSS();
                            }
                        }
                    },
                    function (risposta) {
                        console.log("ERRORE DSS Irrigazione (ControllaDSSIrrigazione)");
                    },
                    null,
                    true
                );

            }

        }

        var _generateIrri2SaveObj = function (save_gias, save_if) {
            let irri = {
                Data: plugin.wbInput.IrrigazionePrevista.Data,
                VolumeMM: plugin.wbInput.IrrigazionePrevista.VolumeMM,
                RunData: plugin.rundate
            };

            if (save_gias) {

                irri.GIAS_key = {
                    PIVA: plugin.wbInput.PersistData.GIAS_PIva,
                    SaCod: plugin.wbInput.PersistData.GIAS_SaCod,
                    Appezza: plugin.wbInput.PersistData.GIAS_Appezza,
                    IdReg: plugin.wbInput.PersistData.GIAS_IdReg,
                    ProgettoCod: plugin.wbInput.PersistData.GIAS_ProgettoCod,
                    VegCod: plugin.wbInput.Veg_Cod,
                    CulCod: plugin.wbInput.Cul_Cod,
                    SupImp: plugin.wbInput.Sup_Imp
                };
            }

            if (save_if) {

                irri.IF_key = {
                    IdPlot: plugin.wbInput.PersistData.Irri_PlotId !== null ? plugin.wbInput.PersistData.Irri_PlotId : 0,
                    IdCrop: plugin.wbInput.PersistData.Irri_CropId !== null ? plugin.wbInput.PersistData.Irri_CropId : 0
                };
            }

            return irri;
        }

        var _generateIrriTurni2SaveObj = function () {
            let irriTurni = [];

            for (let i = 0; i < plugin.wbInput.TurniIrriguiPrevisti.length; i++) {
                let turno = plugin.wbInput.TurniIrriguiPrevisti[i];

                let irriTurno = {
                    Data: turno.Data,
                    DurataM: turno.DurataM,
                    VolumeMM: turno.VolumeMM
                };

                irriTurni.push(irriTurno);
            }

            return irriTurni;
        }

        var _ExportDSS = function () {
            if (plugin.wbInput.IrrigazionePrevista !== undefined && plugin.wbInput.IrrigazionePrevista !== null) {

                $(_getIdDivLabelStampaConsiglio(true)).show();

                WaitFrame.show();

                kendo.drawing.drawDOM($("#" + _getKeyGIAS(plugin.wbInput.PersistData)))
                    .then(function (group) {
                        // Render the result as a PDF file
                        return kendo.drawing.exportPDF(group, {
                            paperSize: "auto",
                            margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" }
                        });
                    })
                    .done(function (data) {

                        $(_getIdDivLabelStampaConsiglio(true)).hide();

                        WaitFrame.hide();

                        // Save the PDF file
                        kendo.saveAs({
                            dataURI: data,
                            fileName: "DSS_Irrigazione_" + _getKeyGIAS(plugin.wbInput.PersistData) +".pdf"
                        });
                    });
            }
        }


        //-----------------------------------------------------------------------------------------
        // fire up the plugin!
        //-----------------------------------------------------------------------------------------

        //$(...).data("WBResult_V2").run();

        //-----------------------------------------------------------------------------------------

    }; // WBResult_V2

    //Add the plugin to the jQuery.fn object
    $.fn.WBResult_V2 = function (header, coltura, arrIndic, persistent) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('WBResult_V2')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.WBResult_V2(this, header, coltura, arrIndic, persistent);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('WBResult_V2', plugin);
            }
        });
    };

})(jQuery);





(function ($) {

    $.WBResult_APP_V2 = function (elem, header, coltura, arrIndic, persistent) {

        var _indic_2_WBInput = function (indic) {

            let wbInput = new Object;

            wbInput.CUAA = indic.CUAA;
            wbInput.Decription = indic.Impianto;
            wbInput.Superficie_ha = indic.Sup_Imp;
            wbInput.Pendenza = indic.Pendenza;
            wbInput.Sabbia = indic.Sabbia;
            wbInput.Argilla = indic.Argilla;
            wbInput.GruppoVegetale = indic.GruppoVegetale;
            wbInput.TraFila = indic.TraFila;
            wbInput.SuFila = indic.SuFila;
            wbInput.CondTraFila = indic.CondTraFila;
            wbInput.Vigoria = indic.Vigoria;
            wbInput.DataInizioImpianto = indic.DataInizioImpianto;
            wbInput.Irri_Veg_Cod = indic.Irri_Veg_Cod;
            wbInput.Irri_Imp_Cod = indic.Irri_Imp_Cod;
            wbInput.Irri_Plot_Id = indic.Irri_Plot_Id;
            wbInput.Irri_Crop_Id = indic.Irri_Crop_Id;
            wbInput.PersistData = {
                GIAS_PIva: indic.GIAS_PIva,
                GIAS_SaCod: indic.GIAS_SaCod,
                GIAS_Appezza: indic.GIAS_Appezza,
                GIAS_IdReg: indic.GIAS_IdReg,
                Irri_PlotId: indic.Irri_Plot_Id,
                Irri_CropId: indic.Irri_Crop_Id
            }
            wbInput.StazioneMeteo = indic.StazioneMeteo;
            wbInput.Irrigazioni = indic.Irrigazioni;
            wbInput.Fertilizzazioni = indic.Fertilizzazioni;
            wbInput.FertiRecipe = indic.FertiRecipe;
            wbInput.Cop_Cod_Irriframe = indic.Cop_Cod_Irriframe;
            wbInput.Mac_Cod = indic.Mac_Cod;
            wbInput.Portata = indic.Portata;
            wbInput.DataFaseStart = null;
            wbInput.RunData = null;

            wbInput.Veg_Cod = indic.Veg_Cod;
            wbInput.Cul_Cod = indic.Cul_Cod;
            wbInput.Sup_Imp = indic.Sup_Imp;
            wbInput.Coord = null;
            wbInput.Indirizzo = "";
            wbInput.Info = [];

            //controllare che GruppoVegetale.ToLower.Trim = "arboree" ???
            if (indic.DataRilievo !== undefined && indic.DataRilievo !== null) {

                wbInput.DataFaseStart = new Date(indic.DataRilievo);

            } else {

                if (indic.DataSemina !== undefined && indic.DataSemina !== null) {

                    wbInput.DataFaseStart = new Date(indic.DataSemina);

                } else {

                    if (indic.DataSeminaPrevista !== undefined && indic.DataSeminaPrevista !== null) {

                        wbInput.DataFaseStart = new Date(indic.DataSeminaPrevista);
                    }
                }
            }

            if (indic.Lat !== undefined && indic.Lat !== null && indic.Lng !== undefined && indic.Lng !== null) {

                wbInput.Coord = { Lat: indic.Lat, Lng: indic.Lng };
            }

            if (indic.Indirizzo !== undefined && indic.Indirizzo !== null) {
                let strArr = [
                    indic.Indirizzo.Indirizzo,
                    indic.Indirizzo.Fraz,
                    indic.Indirizzo.CAP,
                    indic.Indirizzo.Comune,
                    indic.Indirizzo.Prov,
                    "IT"
                ];

                wbInput.Indirizzo = strArr.filter((s) => s != null && s.length > 0).join(" ");
            }

            let infoValue = ((indic.DataInizioImpianto != null) ? (new Date(indic.DataInizioImpianto)).toLocaleDateString() : "");
            wbInput.Info.push({ text: "Inizio impianto", value: infoValue });

            if (indic.GruppoVegetale.toLowerCase() === "arboree") {

                infoValue = ((indic.TraFila !== null) ? kendo.toString(indic.TraFila, "0.0") : "");
                wbInput.Info.push({ text: "Distanza tra fila", value: infoValue });

                infoValue = ((indic.SuFila !== null) ? kendo.toString(indic.SuFila, "0.0") : "");
                wbInput.Info.push({ text: "Distanza su fila", value: infoValue });

                infoValue = "";
                if (indic.CondTraFila !== null && indic.CondTraFila.length > 0) {
                    infoValue = indic.CondTraFila;
                    if (infoValue.toLowerCase() != "inerbito") {
                        infoValue = "Lavorato";
                    }
                }
                wbInput.Info.push({ text: "Conduzione interfilare", value: infoValue });

                infoValue = "";
                if (indic.Vigoria !== null) {
                    switch (indic.Vigoria) {
                        case 1: infoValue = "Debole"; break;
                        case 2: infoValue = "Medio"; break;
                        case 3: infoValue = "Vigoroso"; break;
                        case 4: infoValue = "Molto vigoroso"; break;
                    }
                }

                wbInput.Info.push({ text: "Classe vigore", value: infoValue });
            }

            infoValue = (indic.Irri_Imp_Des != null) ? indic.Irri_Imp_Des : "";
            wbInput.Info.push({ text: "Impianto irriguo", value: infoValue });

            infoValue = (indic.Pendenza != null) ? kendo.toString(indic.Pendenza, "0") : "";
            wbInput.Info.push({ text: "Pendenza", value: infoValue });

            infoValue = (indic.Sabbia != null) ? kendo.toString(indic.Sabbia, "0.0") + "%" : "";
            wbInput.Info.push({ text: "Sabbia", value: infoValue });

            infoValue = (indic.Argilla != null) ? kendo.toString(indic.Argilla, "0.0") + "%" : "";
            wbInput.Info.push({ text: "Argilla", value: infoValue });





            //    ColturaProtetta = False
            //    Id_GreenHouseType = 0
            //    Dim cop = row("Cop_Cod")
            //    Dim cop_mappato = row("Cop_Cod_Irriframe")
            //    If Not IsDBNull(cop) Then
            //        Dim copNum = CInt(cop)
            //        If Not IsDBNull(cop_mappato) Then
            //            ColturaProtetta = True
            //            Id_GreenHouseType = CInt(cop_mappato)
            //        Else
            //            If (Not {0, 3, 4, 5, 6}.Contains(copNum)) Then
            //                ColturaProtetta = True
            //                Errore = "Mappatura copertura non trovata"
            //            End If
            //        End If
            //    End If
            //End Sub




            return wbInput
        }

        var _creaDiv = function (_class, _style, _id, _txt) {
            let div = document.createElement("div");
            if (typeof _class === "string" && _class !== "") {
                div.className = _class;
            }
            if (typeof _style === "string" && _style !== "") {
                div.style.cssText = _style;
            }
            if (typeof _id === "string" && _id !== "") {
                div.id = _id;
            }
            if (typeof _txt === "string" && _txt !== "") {
                div.innerHTML = _txt;
            }
            return div;
        };

        var _clearElement = function (elem) {
            while (elem.firstChild) {
                elem.removeChild(elem.firstChild);
            }
        };

        let divWrapper = _creaDiv("indic-wrapper");
        elem.appendChild(divWrapper);

        let divBody = _creaDiv("indic-body");
        divWrapper.appendChild(divBody);

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.gridElement = elem;
        plugin.$wrapper = $(divWrapper);
        plugin.wrapper = divWrapper;
        plugin.$body = $(divBody);
        plugin.body = divBody;
        plugin.persistent = persistent
        plugin.header = header;
        plugin.coltura = coltura;
        //Convertire tutti gli elementi dell'array???
        plugin.wbInput = _indic_2_WBInput(arrIndic[0]);
        plugin.wbInput.UsaMeteo = (plugin.wbInput.StazioneMeteo !== undefined && plugin.wbInput.StazioneMeteo != null);
        plugin.wbInput.IrriChoice = Irrigazioni_GIAS;

        let divTitle = _creaDiv("label-big overflow-ellipsis", "font-weight: bold;", "", plugin.header);
        let divSpecie = _creaDiv("label-big overflow-ellipsis", "", "", plugin.coltura);

        divBody.appendChild(divTitle);
        divBody.appendChild(divSpecie);

        plugin.result = _creaDiv("indic-result");
        divBody.appendChild(plugin.result);


        //if (divLblSpecie.scrollWidth > divSpecie.offsetWidth) {
        //    divLblSpecie.classList.add('scrolling');
        //}

        //let irriOpts = [
        //    {
        //        text: "Irrigazioni registrate",
        //        value: Irrigazioni_GIAS
        //    },
        //    {
        //        text: "Bilancio ideale",
        //        value: Bilancio_IF
        //    }
        //];
        //if (plugin.persistent) {
        //    irriOpts = [
        //        {
        //            text: "Irrigazioni GIAS",
        //            value: Irrigazioni_GIAS
        //        },
        //        {
        //            text: "Irrigazioni IF",
        //            value: Irrigazioni_IF
        //        },
        //        {
        //            text: "Bilancio ideale",
        //            value: Bilancio_IF
        //        }
        //    ];
        //}

        //plugin.indicatore.IrriChoice = Irrigazioni_GIAS;

        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------

        plugin.run = function () {

            if (_can_run()) {

                _show();
            }
        };


        plugin.filter = function (vegArray) {

            if (vegArray.length === 0) {

                $(elem).removeClass("indic-filtered-out");

            } else {

                if (vegArray.find((cod) => parseInt(cod) === plugin.wbInput.Veg_Cod)) {

                    $(elem).removeClass("indic-filtered-out");

                } else {

                    $(elem).addClass("indic-filtered-out");
                }
            }
        };

        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------

        var _can_run = function () {

            let ris = true;
            let msg = "";
            let fa = "";
            if (plugin.wbInput.DataFaseStart === null) {

                ris = false;
                msg = "Data inizio non impostata";
                fa = "fa-calendar";

            } else {

                if (plugin.wbInput.Coord === null && !plugin.wbInput.UsaMeteo) {

                    ris = false;
                    msg = "Poligono impianto mancante";
                    fa = "fa-map-marker";
                }
            }

            if (!ris) {

                _clearElement(plugin.result);

                let divCont = _creaDiv("k-block k-error-colored indic-result-stop");
                divCont.appendChild(_creaDiv("", "", "", "<span class='fa fa-warning fa-2x'></span>"));
                divCont.appendChild(_creaDiv("", "align-self: center;", "", "<span>" + msg + "</span>"));
                let divIcon = _creaDiv();
                let stack = document.createElement("span");
                stack.className = "fa-stack fa-lg";
                let i0 = document.createElement("i");
                i0.className = "fa fa-square fa-stack-2x";
                i0.style.cssText = "margin: 0px;";
                let i1 = document.createElement("i");
                i1.className = "fa " + fa + " fa-stack-1x fa-inverse";
                i1.style.cssText = "margin: 0px;";
                stack.appendChild(i0);
                stack.appendChild(i1);
                divIcon.appendChild(stack);
                divCont.appendChild(divIcon);

                plugin.result.appendChild(divCont);
            }

            return ris;
        };

        var _show_table = function () {

            if (plugin.table === undefined) {
                return;
            }

            let divWin = _creaDiv("wb-result-table k-block k-shadow grid-span-all", "background-color: #fff; position: relative; z-index: 2;");

            let modal = document.getElementById("modal")
            modal.appendChild(divWin);
            modal.style.display = "block";

            let title = plugin.header + " (" + plugin.coltura + ")";
            let divHdr = _creaDiv("k-header");
            let divHdrCont = _creaDiv("", "display: flex; align-items: center;");
            let divTitle = _creaDiv("", "width: 100%; padding: 4px 8px; font-size: large;", "", title);
            let divClose = _creaDiv("k-button k-button-md k-rounded-md k-button-flat k-button-flat-base k-icon-button k-window-action", "", "", "<span class='k-button-icon k-icon k-i-close' style='font-size: large;'></span>");

            let obfuscator = document.getElementById("obfuscator")
            function close() {

                document.getElementById("modal").style.display = "none";

                let divWin = $(".wb-result-table");
                if (divWin.length > 0) {
                    divWin.remove();
                }

                obfuscator.removeEventListener("click", close);
                $('body').css("overflow", "auto");

            }
            $('body').css("overflow", "hidden");

            obfuscator.addEventListener("click", close);
            divClose.onclick = close;

            divHdrCont.appendChild(divTitle);
            divHdrCont.appendChild(divClose);
            divHdr.appendChild(divHdrCont);
            divWin.appendChild(divHdr);

            let divBody = _creaDiv();
            divWin.appendChild(divBody);

            let divChart = _creaDiv();
            divBody.appendChild(divChart);
            _creaChart(divChart, plugin.table.kendo_rows, plugin.table.kendo_columns);

            divWin.scrollIntoView();
        };

        var _creaChart = function (divChart, ds, columns) {

            let hasUmTerr = columns.findIndex((col) => col.field.startsWith("UT_")) >= 0;

            let style = "";
            if (hasUmTerr) {
                style = "padding-bottom: 10px;";
            }
            let chartWrapper1 = _creaDiv("chart-wrapper", style);

            let chart_id_1 = "irri-chart-1";
            let chart1 = _creaDiv("", "", chart_id_1);
            chartWrapper1.appendChild(chart1);

            divChart.appendChild(chartWrapper1);

            let max_perc = 100;
            let d = new Date()
            let initialData = new Date()
            initialData.setDate(d.getDate() - 10)
            let endData = new Date()
            endData.setDate(d.getDate() + 15)
            let chart_ds = ds.filter((x) => (x.Data >= initialData && x.Data <= endData));


            if (plugin.rundate !== undefined) {

                chart_ds = ds.map(function (x) {

                    max_perc = Math.max(max_perc, x.Umid, x.SoSup);

                    if (x.Data >= plugin.rundate) {

                        x.Umid_Post = x.Umid;
                        x.SoInf_Post = x.SoInf;
                        x.SoSup_Post = x.SoSup;
                        x.SoInfPerc_Post = x.SoInfPerc;
                        x.SoSupPerc_Post = x.SoSupPerc;

                        if (x.Data > plugin.rundate) {

                            delete x.Umid;
                            delete x.SoInf;
                            delete x.SoSup;
                            delete x.SoInfPerc;
                            delete x.SoSupPerc;

                        } else {

                            x.DataCalcolo = 0;

                        }
                    }

                    return x;
                });
            }

            max_perc = (Math.floor(max_perc / 10) + 1) * 10;

            let vertAxes = [
                {
                    title: {
                        text: "Valori di bilancio in % di acqua disponibile"
                    },
                    min: -10,
                    max: max_perc,
                    axisCrossingValue: Number.NEGATIVE_INFINITY,
                    plotBands: [
                        {
                            from: -10,
                            to: 0,
                            color: "red",
                            opacity: 0.3
                        },
                        {
                            from: 100,
                            to: max_perc,
                            color: "green",
                            opacity: 0.3
                        }
                    ]
                },
                {
                    name: "Apporti_Axis",
                    title: {
                        text: "Apporti in mm"
                    }
                }
            ];

            let series = [
                {
                    name: "Umidità terreno",
                    field: "Umid",
                    color: "rgb(255, 140, 0)",
                    gruppo: 1
                },
                {
                    name: "Soglia di intervento",
                    field: "SoInf",
                    color: "rgb(178, 34, 34)",
                    gruppo: 2
                },
                {
                    name: "Soglia superiore",
                    field: "SoSup",
                    color: "rgb(0, 128, 0)",
                    gruppo: 3
                },
                {
                    name: "Piogge",
                    type: "column",
                    field: "Prec",
                    gap: 1,
                    spacing: 0,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 128, 255)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    name: "Irrigazioni",
                    type: "column",
                    field: "Irri",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    field: "Umid_Post",
                    dashType: "dash",
                    gruppo: 1
                },
                {
                    field: "SoInf_Post",
                    dashType: "dash",
                    gruppo: 2
                },
                {
                    field: "SoSup_Post",
                    dashType: "dash",
                    gruppo: 3
                },
                {
                    field: "IrriPrev",
                    type: "column",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    opacity: 0.5,
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                }
            ];

            //-------------------------------------------------------------------------------------
            // oggetto per controllare una riga verticale
            // posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
            //-------------------------------------------------------------------------------------
            let horzAxisCfg = {
                field: "Data",
                vertIndicator: {
                    field: "DataCalcolo",
                    label: {
                        text: "Data Calcolo",
                        color: "#808080" //"#800080"
                    },
                    line: {
                        color: "#808080",
                        width: 1,
                        dashType: "longDash"
                    }
                }
            };

            let titolo = "Bilancio idrico Irriframe";
            if (!plugin.wbInput.UsaMeteo) {
                if (typeof plugin.wbInput.InfoProvider === "string") {
                    if (plugin.wbInput.InfoProvider !== "") {

                        titolo += " - ";
                        titolo += plugin.wbInput.InfoProvider;
                    }
                }
            }

            creaKendoChart(titolo, horzAxisCfg, vertAxes, series, chart_ds, chart_id_1);

            if (hasUmTerr) {

                let chartWrapper2 = _creaDiv("chart-wrapper", "padding-top: 10px;");
                let chart_id_2 = "irri-chart-2";
                let chart2 = _creaDiv("", "", chart_id_2);

                chartWrapper2.appendChild(chart2);

                divChart.appendChild(chartWrapper2);

                let vertAxes2 = [
                    {
                        title: {
                            text: "Umidità terreno (%)"
                        }//,
                        //min: -10,
                        //max: max_perc,
                        //axisCrossingValue: Number.NEGATIVE_INFINITY,
                        //plotBands: [
                        //    {
                        //        from: -10,
                        //        to: 0,
                        //        color: "red",
                        //        opacity: 0.3
                        //    },
                        //    {
                        //        from: 100,
                        //        to: max_perc,
                        //        color: "green",
                        //        opacity: 0.3
                        //    }
                        //]
                    }//,
                    //{
                    //    name: "Apporti_Axis",
                    //    title: {
                    //        text: "Apporti in mm"
                    //    }
                    //}
                ];

                let series2 = [
                    {
                        name: "Soglia inferiore",
                        field: "SoInfPerc",
                        color: "rgb(178, 34, 34)",
                        gruppo: 1
                    },
                    {
                        name: "Soglia superiore",
                        field: "SoSupPerc",
                        color: "rgb(0, 128, 0)",
                        gruppo: 2
                    },
                    //{
                    //    name: "Piogge",
                    //    type: "column",
                    //    field: "Prec",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 128, 255)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    name: "Irrigazioni",
                    //    type: "column",
                    //    field: "Irri",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 206, 209)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    field: "Umid_Post",
                    //    dashType: "dash",
                    //    gruppo: 1
                    //},
                    {
                        field: "SoInfPerc_Post",
                        dashType: "dash",
                        gruppo: 1
                    },
                    {
                        field: "SoSupPerc_Post",
                        dashType: "dash",
                        gruppo: 2
                    }
                ];

                let gruppo = 3;

                $.each(columns, function (i, e) {
                    if (e.field.startsWith("UT_")) {
                        series2.splice(0, 0,
                            {
                                name: e.title,
                                field: e.field,
                                //color: "rgb(255, 140, 0)",
                                gruppo: gruppo++
                            }
                        );
                    }
                });

                ////-------------------------------------------------------------------------------------
                //// oggetto per controllare una riga verticale
                //// posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
                ////-------------------------------------------------------------------------------------
                //let horzAxisCfg = {
                //    field: "Data",
                //    vertIndicator: {
                //        field: "DataCalcolo",
                //        label: {
                //            text: "Data Calcolo",
                //            color: "#808080", //"#800080",
                //        },
                //        line: {
                //            color: "#808080",
                //            width: 1,
                //            dashType: "longDash"
                //        }
                //    }
                //};

                //let titolo = "Bilancio idrico Irriframe";
                //if (typeof plugin.indicatore.InfoProvider === "string") {
                //    if (plugin.indicatore.InfoProvider !== "") {

                //        titolo += " - ";
                //        titolo += plugin.indicatore.InfoProvider;
                //    }
                //}

                creaKendoChart("", "Data", vertAxes2, series2, chart_ds, chart_id_2);
            }
        };

        var _showLoader = function () {

            _clearElement(plugin.result);

            let divRel = _creaDiv("k-block indic-loader");
            plugin.result.appendChild(divRel);
            $(divRel).StyleLoader();
        };

        var _creaAdv = function (msg, bkg, clr, icon) {

            let advStyle = "border-radius: 5px; border: 1px solid #aeaeae; display: flex; flex-grow: 1; ";
            if (bkg !== "") {
                advStyle += " background-color: " + bkg + ";";
            }
            if (clr !== "") {
                advStyle += " color: " + clr + ";";
            }

            // TODO add icon
            let containerDiv = _creaDiv("", "display: flex; flex-direction: column");
            let divIcon = _creaDiv("fa " + icon, "align-self: center; margin-bottom: 5px");
            containerDiv.appendChild(divIcon);
            let divAdv = _creaDiv("", advStyle);
            containerDiv.appendChild(divAdv);
            let divMsg = _creaDiv("", "width: 100%; align-self: center; text-align: center;", "", msg);
            divAdv.appendChild(divMsg);

            return containerDiv;
        };

        var _show = function () {

            _showLoader();
            
            let url = window.location.href.split("?")[0] + "/CalcolaWBResult_V2";
            ajaxAgronica(url,
                JSON.stringify({ input: plugin.wbInput }),
                function (risposta) {

                    let res = JSON.parse(risposta.RispostaStringa);
                    if (res.Status === 0) {

                        if (res.Result.IrriframeData &&
                            res.Result.IrriframeData.PlotId > 0 &&
                            res.Result.IrriframeData.CropId > 0) {
                            plugin.wbInput.PersistData.Irri_PlotId = res.Result.IrriframeData.PlotId;
                            plugin.wbInput.PersistData.Irri_CropId = res.Result.IrriframeData.CropId;
                        }

                        if (res.Result.TurniIrriguiPrevisti !== null && res.Result.TurniIrriguiPrevisti.length > 0) {
                            plugin.ShowTurniIrrigui = true;
                        }

                        _showResult(res.Result);

                    } else {

                        _showError(res.Message);
                    }
                },
                function (risposta) {
                    console.log("ERRORE DSS Irrigazione (WBResult)");
                },
                null,
                false
            );
        };

        var _showResult = function (res) {

            _clearElement(plugin.result);

            plugin.wbInput.InfoProvider = res.InfoProvider;
            plugin.wbInput.IrrigazionePrevista = null;

            let msgIrri = "NO";
            let msgData = "";
            let msgVol = "";
            let msgCons = "";

            let dataEsecuzione = Date.parseLocale(res.DataEsecuzione);

            if (!isNaN(parseFloat(res.ConsumoMM))) {
                msgCons = kendo.toString(res.ConsumoMM, "0.00") + " mm";
            }

            let arrAdv = [
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" }
            ];

            let hasTurniIrrigui = res.TurniIrriguiPrevisti !== undefined && res.TurniIrriguiPrevisti !== null && typeof res.TurniIrriguiPrevisti === "object" && res.TurniIrriguiPrevisti.length > 0;

            if (typeof res.IrrigazioniPreviste === "object") {

                if (res.IrrigazioniPreviste.length > 0) {

                    msgIrri = "SI";

                    let idx = 0;
                    let cnt = Math.min(arrAdv.length - 1, res.IrrigazioniPreviste.length);

                    while (idx < cnt) {

                        let irri = res.IrrigazioniPreviste[idx];
                        let dt = new Date(irri.Data);
                        dt.setHours(0, 0, 0, 0);

                        if (plugin.wbInput.IrrigazionePrevista === null) {
                            plugin.wbInput.IrrigazionePrevista = {
                                Data: dt,
                                VolumeMM: irri.VolumeMM
                            }
                        }

                        if (idx === 0) {
                            msgData = kendo.toString(dt, "ddd dd-MM"); //VEN 27-03-2020
                            msgVol = kendo.toString(irri.VolumeMM, "0.00") + " mm";
                        }

                        arrAdv[idx + 1].msg = kendo.toString(dt, "dd/MM");

                        let diff_days = Math.floor((dt.getTime() - dataEsecuzione.getTime()) / (1000 * 60 * 60 * 24));
                        if (diff_days <= 3) {
                            arrAdv[idx + 1].bkg = "#f00000"; //rosso
                            arrAdv[idx + 1].clr = "#ffffff";
                        } else if (diff_days <= 7) {
                            arrAdv[idx + 1].bkg = "#fff000"; //giallo   "#ff8000" 'arancio
                        } else {
                            arrAdv[idx + 1].bkg = "#008000"; //verde
                            arrAdv[idx + 1].clr = "#ffffff";
                        }

                        idx++;
                    }
                }
            }

            if (typeof res.Table === "string") {
                if (res.Table !== "") {

                    //let obj_table = JSON.parse(res.Table);
                    let obj_table = (hasTurniIrrigui && plugin.ShowTurniIrrigui && typeof res.TableTurni === "string" && res.TableTurni !== "") ? JSON.parse(res.TableTurni) : JSON.parse(res.Table);

                    if (typeof obj_table.kendo_rows === "object") {
                        if (obj_table.kendo_rows.length > 0) {

                            plugin.table = obj_table;

                            let idx = plugin.table.kendo_rows.length - 1;

                            while (idx >= 0) {

                                let elem = plugin.table.kendo_rows[idx];

                                if (elem.Irri > 0) {

                                    let elem_dt = kendo.parseDate(elem.Data);

                                    arrAdv[0].msg = kendo.toString(elem_dt, "dd/MM");
                                    arrAdv[0].bkg = "#00ced1"; //"#00bfff"; //
                                    arrAdv[0].clr = "#ffffff";

                                    if (elem_dt.getTime() > dataEsecuzione.getTime()) {
                                        //Irrigazione registrata nel futuro... (Devo farlo notare???)
                                    }

                                    idx = 0;
                                }
                                idx--;
                            }
                        }
                    }
                }
            }

            let divContent = _creaDiv("indic-result-valid");

            let fa = "fa-info-circle";
            //let fa = "k-i-info-circle";
            if (plugin.wbInput.Info.some((el) => el.value === "")) {
                fa = "fa-exclamation-triangle";
                //fa = "k-i-exclamation-circle";
            }

            let divParams = _creaDiv("indic-parametri label-small", "cursor: pointer;");
            divParams.appendChild(_creaDiv("overflow-ellipsis", "", "", "Ambientali/Impianto"));
            divParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='fa " + fa + " fa-lg fa-fw'></span>"));
            //divPlusParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='k-icon " + fa + "'></span>"));
            divParams.setAttribute("data-tooltip", JSON.stringify(plugin.wbInput.Info));

            divContent.appendChild(divParams);

            let divIrri = _creaDiv("result-irri");//, "height: 3.5em; display: grid; grid-template-columns: 1fr 1fr 1fr; grid-gap: 3px;");
            divIrri.appendChild(_creaAdv(arrAdv[0].msg, arrAdv[0].bkg, arrAdv[0].clr, "fa-history"));
            divIrri.appendChild(_creaAdv(arrAdv[1].msg, arrAdv[1].bkg, arrAdv[1].clr, "fa-arrow-down"));
            divIrri.appendChild(_creaAdv(arrAdv[2].msg, arrAdv[2].bkg, arrAdv[2].clr, "fa-forward"));

            divContent.appendChild(divIrri);

            let divCheckPartiz = null;
            if (hasTurniIrrigui) {

                divCheckPartiz = _creaDiv();
                divCheckPartiz.innerHTML = "<input type=\"checkbox\" value=\"\" id=\"chkPartizTurni\">";

                divContent.appendChild(divCheckPartiz);
            }

            let divAdvice = _creaDiv("k-block k-info-colored ripple-cont result-advice");
            //divAdvice.appendChild(_creaDiv("", "", "", "IRRIGAZIONE:"));
            //divAdvice.appendChild(_creaDiv("value", "", "", msgIrri));
            //divAdvice.appendChild(_creaDiv("", "", "", "DATA:"));
            //divAdvice.appendChild(_creaDiv("value", "", "", msgData));
            //divAdvice.appendChild(_creaDiv("", "", "", "VOLUME:"));
            //divAdvice.appendChild(_creaDiv("value", "", "", msgVol));
            //divAdvice.appendChild(_creaDiv("", "", "", "CONSUMO:"));
            //divAdvice.appendChild(_creaDiv("value", "", "", msgCons));

            if (hasTurniIrrigui && plugin.ShowTurniIrrigui) {

                divAdvice.style.maxHeight = "200px";
                divAdvice.style.overflowY = "scroll";

                for (let i = 0; i < res.TurniIrriguiPrevisti.length; i++) {
                    let msgTurnoData = kendo.toString(new Date(res.TurniIrriguiPrevisti[i].Data), "g")
                    let msgTurnoDurata = kendo.toString(res.TurniIrriguiPrevisti[i].DurataM, "n0") + " minuti"

                    divAdvice.appendChild(_creaDiv("", "", "", "TURNO:"));
                    divAdvice.appendChild(_creaDiv("value", "", "", msgTurnoData));
                    divAdvice.appendChild(_creaDiv("", "", "", "DURATA:"));
                    divAdvice.appendChild(_creaDiv("value", "", "", msgTurnoDurata));
                    divAdvice.appendChild(_creaDiv("", "", "", ""));
                    divAdvice.appendChild(_creaDiv("value", "", "", "<br>"));
                }
            } else {

                divAdvice.appendChild(_creaDiv("", "", "", "IRRIGAZIONE:"));
                divAdvice.appendChild(_creaDiv("value", "", "", msgIrri));
                divAdvice.appendChild(_creaDiv("", "", "", "DATA:"));
                divAdvice.appendChild(_creaDiv("value", "", "", msgData));
                divAdvice.appendChild(_creaDiv("", "", "", "VOLUME:"));
                divAdvice.appendChild(_creaDiv("value", "", "", msgVol));
                divAdvice.appendChild(_creaDiv("", "", "", "CONSUMO:"));
                divAdvice.appendChild(_creaDiv("value", "", "", msgCons));
            }

            divContent.appendChild(divAdvice);

            plugin.result.appendChild(divContent);

            if (divCheckPartiz !== null) {
                $("#chkPartizTurni").kendoCheckBox({
                    label: "Visualizza turni",
                    checked: (plugin.ShowTurniIrrigui === true),
                    change: function (e) {
                        plugin.ShowTurniIrrigui = e.checked;
                        _showResult(res);
                    }
                });
            }

            if (plugin.table !== undefined) {

                if (res.DataEsecuzione !== undefined) {

                    plugin.rundate = new Date(res.DataEsecuzione);
                    plugin.rundate.setHours(0, 0, 0, 0);
                }

                let ripple_cont = $(plugin.result).find(".ripple-cont");
                if (ripple_cont.length > 0) {

                    ripple_cont.addClass("irri-ripple");
                    ripple_cont[0].onclick = _show_table;
                    ripple_cont[0].insertBefore(_creaDiv("ripple"), ripple_cont[0].firstChild);
                }
            }
        };

        var _showError = function (msg) {

            _clearElement(plugin.result);

            let divMsg = _creaDiv("k-block indic-result-error");
            let span = document.createElement("span");
            span.style.cssText = "text-align: center;";
            span.innerHTML = msg;
            divMsg.appendChild(span);
            plugin.result.appendChild(divMsg);
        };

        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------

        //$(...).data("WBResult").run();

        //-----------------------------------------------------------------------------------------

    }; // WBResult

    //Add the plugin to the jQuery.fn object
    $.fn.WBResult_APP_V2 = function (header, coltura, arrIndic, persistent) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('WBResult_APP_V2')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.WBResult_APP_V2(this, header, coltura, arrIndic, persistent);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('WBResult_APP_V2', plugin);
            }
        });
    };
})(jQuery);





(function ($) {

    $.WBResult = function (elem, indic, persistent) {

        var _creaDiv = function (_class, _style, _id, _txt) {
            let div = document.createElement("div");
            if (typeof _class === "string" && _class !== "") {
                div.className = _class;
            }
            if (typeof _style === "string" && _style !== "") {
                div.style.cssText = _style;
            }
            if (typeof _id === "string" && _id !== "") {
                div.id = _id;
            }
            if (typeof _txt === "string" && _txt !== "") {
                div.innerHTML = _txt;
            }
            return div;
        };

        var _clearElement = function (elem) {
            while (elem.firstChild) {
                elem.removeChild(elem.firstChild);
            }
        };

        var _getElemAndClear = function (selector) {

            let elem = plugin.$body.find(selector);
            if (elem.length == 0) {
                return null;
            }
            elem = elem[0];

            _clearElement(elem);

            $(elem).removeClass("valid");
            $(elem).removeClass("not-valid");

            return elem;
        };

        var _creaBtn = function (parent, classname, fa_icon, text) {
            let btn = _creaDiv("k-button k-button-md k-rounded-md " + classname);
            btn.appendChild(_creaDiv("", "", "", "<span class='fa " + fa_icon + " fa-lg fa-fw'></span>"));
            btn.appendChild(_creaDiv("overflow-ellipsis", "", "", text));
            parent.appendChild(btn);
            return btn;
        }

        var _sayLatLng = function () {

            let elem = _getElemAndClear(".lat-lng");
            if (elem == null) {
                return;
            }

            if (plugin.indicatore.LatLng === null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "indic-tooltip", "fa-map-marker", "Geolocalizza");
                let indirizzo = "";
                if (plugin.indicatore.hasOwnProperty("Indirizzo")) {
                    if (plugin.indicatore.Indirizzo != "") {
                        indirizzo = "<div style='margin-top:10px; font-style:italic;'>" + plugin.indicatore.Indirizzo + "</div>";
                    }
                }
                btn.setAttribute("data-tooltip", "<div style='text-align:center;'><div>Geolocalizza da indirizzo appezzamento/centro</div>" + indirizzo + "</div>");
                btn.onclick = function () { _geolocalizza(); };

                return;
            }

            $(elem).addClass("label-small").addClass("valid");

            let colLat = _creaDiv();
            elem.appendChild(colLat);
            colLat.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.indicatore.LatLng.Lat, "0.0000000") + "</span>"));
            colLat.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lat</span>"));

            let colLng = _creaDiv();
            elem.appendChild(colLng);
            colLng.appendChild(_creaDiv("overflow-ellipsis", "", "", "<span>" + kendo.toString(plugin.indicatore.LatLng.Lng, "0.0000000") + "</span>"));
            colLng.appendChild(_creaDiv("lat-lng-label", "", "", "<span>Lng</span>"));
        };

        var _sayStart = function () {

            let elem = _getElemAndClear(".data-start");
            if (elem == null) {
                return;
            }

            if (plugin.indicatore.DataFaseStart == null) {

                $(elem).addClass("not-valid");

                let btn = _creaBtn(elem, "", "fa-calendar", "Imposta data start")
                btn.onclick = function () { _impostaStart(); };

                return;
            }

            let start = kendo.toString(plugin.indicatore.DataFaseStart, "d");
            $(elem).addClass("label-small valid");
            elem.appendChild(_creaDiv("overflow-ellipsis", "", "", start));
            let divQuestion = _creaDiv("indic-tooltip", "color: #337ab7;", "", "<span class='fa fa-question-circle fa-lg fa-fw'></span>");
            divQuestion.setAttribute("data-tooltip", "Data semina, trapianto o risveglio vegetativo");
            elem.appendChild(divQuestion);
        };

        let divWrapper = _creaDiv("indic-wrapper");
        elem.appendChild(divWrapper);

        let divBody = _creaDiv("indic-body");
        divWrapper.appendChild(divBody);

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;
        plugin.gridElement = elem;
        plugin.$wrapper = $(divWrapper);
        plugin.wrapper = divWrapper;
        plugin.$body = $(divBody);
        plugin.body = divBody;
        plugin.persistent = persistent
        plugin.indicatore = indic;
        plugin.indicatore.UsaMeteo = false;
        plugin.indicatore.IrriChoice = Irrigazioni_GIAS;

        if (plugin.indicatore.DataFaseStart != null) {
            plugin.indicatore.DataFaseStart = new Date(plugin.indicatore.DataFaseStart);
        }

        let campo = plugin.indicatore.Campo;
        if (campo != "") {
            campo += " - ";
        }
        let divTitle = _creaDiv("label-big overflow-ellipsis", "font-weight: bold;", "", campo + plugin.indicatore.Impianto);

        let varieta = plugin.indicatore.Varieta;
        if (varieta != "") {
            varieta = " - " + varieta;
        }
        let divSpecie = _creaDiv("label-big overflow-ellipsis", "", "", plugin.indicatore.Coltura + varieta);

        divBody.appendChild(divTitle);
        divBody.appendChild(divSpecie);

        let divParams = _creaDiv("grid-params")

        divBody.appendChild(divParams);

        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-globe fa-lg fa-fw'></span>"));
        let divCoord = _creaDiv("lat-lng");
        divParams.appendChild(divCoord);
        _sayLatLng();

        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-leaf fa-lg fa-fw'></span>"));
        let divStart = _creaDiv("data-start");
        divParams.appendChild(divStart);
        _sayStart();

        let warn = false;
        let fa = "fa-info-circle";
        let ipar = 0;
        while (!warn && ipar < plugin.indicatore.Info.length) {
            if (plugin.indicatore.Info[ipar].value === "") {
                warn = true;
                fa = "fa-exclamation-triangle";
            }
            ipar++;
        }
        divParams.appendChild(_creaDiv("left", "", "", "<span class='fa fa-tags fa-lg fa-fw'></span>"));
        let divPlusParams = _creaDiv("indic-parametri label-small", "cursor: pointer;");
        divParams.appendChild(divPlusParams);
        divPlusParams.appendChild(_creaDiv("overflow-ellipsis", "", "", "Ambientali/Impianto"));
        divPlusParams.appendChild(_creaDiv("icona-parametri", "", "", "<span class='fa " + fa + " fa-lg fa-fw'></span>"));
        divPlusParams.setAttribute("data-tooltip", JSON.stringify(plugin.indicatore.Info));

        plugin.result = _creaDiv();
        divWrapper.appendChild(plugin.result);


        let AllowPropMeteo = false;
        if (plugin.indicatore.Stazioni_Meteo !== undefined && plugin.indicatore.Stazioni_Meteo != null && plugin.indicatore.Stazioni_Meteo.length > 0) {
            AllowPropMeteo = true;
            plugin.indicatore.UsaMeteo = true;
        }

        let divFooter = _creaDiv("", "margin-top: 5px; padding-top: 5px; border-top: 1px solid #cccccc;");
        elem.appendChild(divFooter);

        let label = document.createElement("label");
        label.className = "toggle-switch";
        divFooter.appendChild(label);
        let checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        label.appendChild(checkbox);
        let back = _creaDiv("toggle-back");
        label.appendChild(back);
        let toggle = _creaDiv("toggle");
        let lbl_on = _creaDiv("toggle-label on", "", "", "Dati meteo proprietari");
        let lbl_off = _creaDiv("toggle-label off", "", "", "Dati meteo pubblici");
        back.appendChild(toggle);
        back.appendChild(lbl_on);
        back.appendChild(lbl_off);

        if (!AllowPropMeteo) {

            checkbox.disabled = true;

        } else {

            checkbox.checked = plugin.indicatore.UsaMeteo;
        }

        $(checkbox).on('change', function () {

            let value = this.checked;

            if (plugin.indicatore.UsaMeteo === value) {
                return;
            }

            plugin.indicatore.UsaMeteo = value;

            plugin.run();
        });



        //let divFooter2 = _creaDiv("", "padding-top: 5px");
        //elem.appendChild(divFooter2);
        //let label2 = document.createElement("label");
        //label2.className = "toggle-switch";
        //divFooter2.appendChild(label2);
        //let checkbox2 = document.createElement("input");
        //checkbox2.type = "checkbox";
        //label2.appendChild(checkbox2);
        //let back2 = _creaDiv("toggle-back");
        //label2.appendChild(back2);
        //let toggle2 = _creaDiv("toggle");
        //let lbl2_on = _creaDiv("toggle-label on", "", "", "Irrigazioni registrate");
        //let lbl2_off = _creaDiv("toggle-label off", "", "", "Irrigazioni da modello");
        //back2.appendChild(toggle2);
        //back2.appendChild(lbl2_on);
        //back2.appendChild(lbl2_off);

        //checkbox2.checked = plugin.indicatore.IsIrriUser;

        //$(checkbox2).on('change', function () {

        //    let value = this.checked;

        //    if (plugin.indicatore.IsIrriUser === value) {
        //        return;
        //    }

        //    plugin.indicatore.IsIrriUser = value;

        //    plugin.run();
        //});

        let irriOpts = [
            {
                text: "Irrigazioni registrate",
                value: Irrigazioni_GIAS
            },
            {
                text: "Bilancio ideale",
                value: Bilancio_IF
            }
        ];
        if (plugin.persistent) {
            irriOpts = [
                {
                    text: "Irrigazioni GIAS",
                    value: Irrigazioni_GIAS
                },
                {
                    text: "Irrigazioni IF",
                    value: Irrigazioni_IF
                },
                {
                    text: "Bilancio ideale",
                    value: Bilancio_IF
                }
            ];
        }

        let divFooter3 = _creaDiv("", "padding-top: 5px");
        elem.appendChild(divFooter3);
        $(divFooter3).MultiStateSwitch({
            options: irriOpts,
            onchange: function (state) {

                if (plugin.indicatore.IrriChoice === state) {
                    return;
                }

                plugin.indicatore.IrriChoice = state;

                plugin.run();
            }
        });

        $(divFooter3).data("MultiStateSwitch").setValue(plugin.indicatore.IrriChoice);

        //-------------------------------------------------------------------------------------------------
        // public methods
        //-------------------------------------------------------------------------------------------------

        plugin.run = function () {

            if (_can_run()) {

                _remove_table(plugin.$wrapper.parents(".indic-elem"));
                _show();
            }
        };


        plugin.filter = function (vegArray) {

            if (vegArray.length === 0) {

                $(elem).removeClass("indic-filtered-out");
            } else {

                let idx = 0;
                let found = false;
                while (!found && idx < vegArray.length) {
                    found = (plugin.indicatore.Veg_Cod == vegArray[idx]);
                    idx++;
                }
                if (found) {

                    $(elem).removeClass("indic-filtered-out");
                } else {

                    $(elem).addClass("indic-filtered-out");
                    _remove_table(elem);
                }
            }
        };

        //-------------------------------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------------------------------

        var _can_run = function () {

            let ris = true;
            let msg = "";
            let fa = "";
            if (plugin.indicatore.DataFaseStart === null) {

                ris = false;
                msg = "Data start non impostata";
                fa = "fa-calendar";

            } else {

                if (plugin.indicatore.LatLng === null && !plugin.indicatore.UsaMeteo) {

                    ris = false;
                    msg = "Geolocalizzazione non disponibile";
                    fa = "fa-map-marker";
                }
            }

            if (!ris) {

                _clearElement(plugin.result);

                let divCont = _creaDiv("k-block k-error-colored", "height: 100%; text-align: center; display: grid; grid-template-rows: auto 1fr auto;");
                divCont.appendChild(_creaDiv("", "padding-top: 5px;", "", "<span class='fa fa-warning fa-2x'></span>"));
                divCont.appendChild(_creaDiv("", "align-self: center;", "", "<span>" + msg + "</span>"));
                let divIcon = _creaDiv();
                let stack = document.createElement("span");
                stack.className = "fa-stack fa-lg";
                let i0 = document.createElement("i");
                i0.className = "fa fa-square fa-stack-2x";
                i0.style.cssText = "margin: 0px;";
                let i1 = document.createElement("i");
                i1.className = "fa " + fa + " fa-stack-1x fa-inverse";
                i1.style.cssText = "margin: 0px;";
                stack.appendChild(i0);
                stack.appendChild(i1);
                divIcon.appendChild(stack);
                divCont.appendChild(divIcon);

                plugin.result.appendChild(divCont);
            }

            return ris;
        };

        var _impostaStart = function () {

            let divWin = _creaDiv("", "display: none;", "irri-win-data");

            plugin.wrapper.appendChild(divWin);

            let divWrapper = _creaDiv("", "display: flex; flex-flow: column; align-items: stretch; row-gap: 10px; max-width: 300px;", "irri-win-data-wrapper");
            divWin.appendChild(divWrapper);

            let divCalendar = _creaDiv("", "align-self: center;", "irri-calendar");
            divWrapper.appendChild(divCalendar);

            $("#irri-calendar").kendoCalendar({
                value: new Date(),
                footer: false
            });

            let divMsg = _creaDiv("k-block k-info-colored", "padding: 5px 10px; text-align: justify;");
            divMsg.innerHTML = "Registrare data di Semina / Trapianto / Rilievo fase fenologica opportuna nel quaderno di campagna per impostare la data definitivamente.";
            divWrapper.appendChild(divMsg);

            let divBtnSet = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base", "", "", "<span>Imposta temporaneamente</span>");
            divWrapper.appendChild(divBtnSet);
            divBtnSet.onclick = function () { _actImpostaStart(false); };

            //let divBtnRec = _creaDiv("k-button", "margin-top: 10px;", "", "<span>Imposta e registra</span>");
            //divWrapper.appendChild(divBtnRec);
            //divBtnRec.onclick = function () { _actImpostaStart(true); }

            let $kendomodal = $("#irri-win-data");

            $kendomodal.kendoWindow({
                title: "Imposta data start",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                    $("body").addClass("overflow_hidden");
                    $("#irri-win-data-wrapper").css("opacity", "0");
                },
                activate: function () {
                    $("#irri-win-data-wrapper").css("opacity", "1");
                },
                close: function (e) {
                    $("body").removeClass("overflow_hidden");
                    $kendomodal.data("kendoWindow").destroy();
                }
            });
            $kendomodal.data("kendoWindow").center().open();
        };

        var _actImpostaStart = function (flagRec) {

            let calendar = $("#irri-calendar").data("kendoCalendar");

            let dt = new Date(calendar.value());
            dt.setHours(0, 0, 0, 0);

            //todo: verificare validità data

            if (flagRec) {
                //todo: chiamare WS per registrazione semina/trapianto/inizio fase vegetativa
            }

            plugin.indicatore.DataFaseStart = dt;

            _sayStart();

            let win = $("#irri-win-data").data("kendoWindow");

            win.close();

            plugin.run();
        };

        var _geolocalizza = function () {

            if (!plugin.indicatore.hasOwnProperty("Indirizzo")) {
                return;
            }

            if (plugin.indicatore.Indirizzo === "") {
                return;
            }

            let geocoder = new google.maps.Geocoder;

            geocoder.geocode({
                address: plugin.indicatore.Indirizzo
            },
                function (results, status) {
                    if (status === google.maps.GeocoderStatus.OK) {

                        let l = results[0].geometry.location;

                        if (l.lat() !== "" && l.lng() !== "") {

                            plugin.indicatore.LatLng = {
                                Lat: l.lat(),
                                Lng: l.lng()
                            };

                            _sayLatLng();

                            //if (!plugin.indicatore.UsaMeteo) {
                            //  plugin.run();
                            //}

                            plugin.run();
                        }
                    } else {
                        console.log("Geocode was not successful for the following reason: " + status);
                    }
                }
            );
        };

        var _show_table = function () {

            if (plugin.table === undefined) {
                return;
            }

            let grid_elem = plugin.$wrapper.parents(".indic-elem");
            if ($(grid_elem).hasClass("indic-elem-shown")) {
                return;
            }

            let unique_id = 1;
            $.each(document.getElementsByClassName("irri-tabs"), function (idx, elem) {
                unique_id = Math.max(parseInt($(elem).attr("data-id")) + 1, unique_id);
            });
            let divWinId = "win-show-" + unique_id;

            $(grid_elem).toggleClass("k-shadow");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            //$(grid_elem).toggleClass("k-inset");
            $(grid_elem).addClass("indic-elem-shown");
            $(grid_elem)[0].setAttribute("data-win-id", divWinId);

            let insert_after = grid_elem;
            let all_elem = grid_elem.parent().children();
            if (all_elem.length > 1) {
                let top = grid_elem.position().top;
                let idx = 0;
                let found = false;
                while (idx < all_elem.length && !found) {
                    if ($(all_elem[idx]).position().top > top) {
                        found = true;
                    } else {
                        insert_after = $(all_elem[idx]);
                    }
                    idx++;
                }
            }

            insert_after = insert_after[0];
            while ($(insert_after.nextSibling).hasClass("grid-span-all")) {
                insert_after = insert_after.nextSibling;
            }

            let divWin = _creaDiv("k-block k-shadow grid-span-all", "background-color: #fff;", divWinId);
            insert_after.parentNode.insertBefore(divWin, insert_after.nextSibling);

            let title = plugin.indicatore.Campo;
            if (title != "") {
                title += " - ";
            }
            title += plugin.indicatore.Impianto;
            let varieta = plugin.indicatore.Varieta;
            if (varieta != "") {
                varieta = " - " + varieta;
            }
            title += " (" + plugin.indicatore.Coltura + varieta + ")";

            let divHdr = _creaDiv("k-header");
            let divHdrCont = _creaDiv("", "display: flex; align-items: center;");
            let divTitle = _creaDiv("", "width: 100%; padding: 4px 8px; font-size: large;", "", title);
            let divClose = _creaDiv("k-button k-button-md k-rounded-md k-button-flat k-button-flat-base k-icon-button k-window-action", "", "", "<span class='k-button-icon k-icon k-i-close' style='font-size: large;'></span>");
            divClose.onclick = function () {
                _remove_table(grid_elem);
            };

            divHdrCont.appendChild(divTitle);
            divHdrCont.appendChild(divClose);
            divHdr.appendChild(divHdrCont);
            divWin.appendChild(divHdr);

            let divBody = _creaDiv();
            divWin.appendChild(divBody);

            let unique_id_tabs = "irri-tabs-" + unique_id;
            let unique_id_chart = "irri-kendo-chart-" + unique_id;
            let unique_id_grid = "irri-kendo-grid-" + unique_id;

            let divTabs = _creaDiv("irri-tabs", "background-color:#fff; box-shadow:none;", unique_id_tabs);
            divTabs.setAttribute("data-id", unique_id);
            divBody.appendChild(divTabs);

            unique_id_tabs = "#" + unique_id_tabs;
            $(unique_id_tabs).kendoTabStrip({
                animation: false,
                scrollable: false
            });

            let tabstrips = $(unique_id_tabs).data("kendoTabStrip");

            $.each([{ text: "Grafico", id: unique_id_chart }, { text: "Tabella", id: unique_id_grid }], function (index, obj) {
                tabstrips.append({
                    text: obj.text,
                    content: "<div style='overflow:hidden;'><div id='" + obj.id + "'></div></div>"
                });
            });

            tabstrips.tabGroup.css("font-size", "larger");

            tabstrips.select(0);

            _creaGrid(unique_id_grid, plugin.table);
            unique_id_grid = "#" + unique_id_grid;

            let grid_ds = $(unique_id_grid).data("kendoGrid").dataSource.data();

            _creaChart(unique_id_chart, grid_ds, plugin.table.kendo_columns);
            unique_id_chart = "#" + unique_id_chart;

            tabstrips.bind("activate", function (e) {
                if ($(e.contentElement).index() === 2) {

                    let chartH = 0;
                    $(unique_id_chart + " .chart-wrapper").each(function () {
                        chartH += $(this).outerHeight() + $(this).find(".k-chart").height();
                    });
                    let hdrH = $(unique_id_grid + " .k-grid-header").outerHeight();
                    let btw = parseFloat($(unique_id_grid).css("border-top-width"));
                    let bbw = parseFloat($(unique_id_grid).css("border-bottom-width"));

                    $(unique_id_grid + " .k-grid-content").css("height", (chartH - hdrH - btw - bbw) + "px");

                    tabstrips.unbind("activate");
                }
            });

            divWin.scrollIntoView();
        };

        var _remove_table = function (elem) {

            if (!$(elem).hasClass("indic-elem-shown")) {
                return;
            }

            let divWinId = $(elem).attr("data-win-id");
            let divWin = $("#" + divWinId);
            if (divWin.length > 0) {
                divWin.remove();
            }

            $(elem).removeClass("indic-elem-shown");

            let ripple_cont = $(plugin.result).find(".ripple-cont");
            if (ripple_cont.length > 0) {
                ripple_cont.toggleClass("irri-ripple");
            }

            $(elem).toggleClass("k-shadow");
            //$(elem).toggleClass("k-inset");
        };

        var _creaGrid = function (div_id, kendodata) {

            let objConfig = {
                dataSource: {
                    data: kendodata.kendo_rows,
                    schema: {
                        model: {
                            fields: kendodata.kendo_model
                        }
                    }
                },
                columns: kendodata.kendo_columns,
                resizable: true,
                pageable: false
            };

            $("#" + div_id).kendoGrid(objConfig);
        };

        var _creaChart = function (div_id, ds, columns) {

            let hasUmTerr = false;
            let c = 0;
            while (c < columns.length && !hasUmTerr) {
                if (columns[c].field.startsWith("UT_")) {
                    hasUmTerr = true;
                }
                c++;
            }

            let chartWrapper = document.getElementById(div_id);

            let style = "";
            if (hasUmTerr) {
                style = "padding-bottom: 10px;";
            }
            let chartWrapper1 = _creaDiv("chart-wrapper", style);

            let chart_id_1 = div_id + "-1";
            let chart1 = _creaDiv("", "", chart_id_1);
            chartWrapper1.appendChild(chart1);

            chartWrapper.appendChild(chartWrapper1);

            let max_perc = 100;
            let chart_ds = ds;
            if (plugin.rundate !== undefined) {

                chart_ds = ds.map(function (x) {

                    max_perc = Math.max(max_perc, x.Umid, x.SoSup);

                    if (x.Data >= plugin.rundate) {

                        x.Umid_Post = x.Umid;
                        x.SoInf_Post = x.SoInf;
                        x.SoSup_Post = x.SoSup;
                        x.SoInfPerc_Post = x.SoInfPerc;
                        x.SoSupPerc_Post = x.SoSupPerc;

                        if (x.Data > plugin.rundate) {

                            delete x.Umid;
                            delete x.SoInf;
                            delete x.SoSup;
                            delete x.SoInfPerc;
                            delete x.SoSupPerc;

                        } else {

                            x.DataCalcolo = 0;

                        }
                    }

                    return x;
                });
            }

            max_perc = (Math.floor(max_perc / 10) + 1) * 10;

            let vertAxes = [
                {
                    title: {
                        text: "Valori di bilancio in % di acqua disponibile"
                    },
                    min: -10,
                    max: max_perc,
                    axisCrossingValue: Number.NEGATIVE_INFINITY,
                    plotBands: [
                        {
                            from: -10,
                            to: 0,
                            color: "red",
                            opacity: 0.3
                        },
                        {
                            from: 100,
                            to: max_perc,
                            color: "green",
                            opacity: 0.3
                        }
                    ]
                },
                {
                    name: "Apporti_Axis",
                    title: {
                        text: "Apporti in mm"
                    }
                }
            ];

            let series = [
                {
                    name: "Umidità terreno",
                    field: "Umid",
                    color: "rgb(255, 140, 0)",
                    gruppo: 1
                },
                {
                    name: "Soglia di intervento",
                    field: "SoInf",
                    color: "rgb(178, 34, 34)",
                    gruppo: 2
                },
                {
                    name: "Soglia superiore",
                    field: "SoSup",
                    color: "rgb(0, 128, 0)",
                    gruppo: 3
                },
                {
                    name: "Piogge",
                    type: "column",
                    field: "Prec",
                    gap: 1,
                    spacing: 0,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 128, 255)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    name: "Irrigazioni",
                    type: "column",
                    field: "Irri",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                },
                {
                    field: "Umid_Post",
                    dashType: "dash",
                    gruppo: 1
                },
                {
                    field: "SoInf_Post",
                    dashType: "dash",
                    gruppo: 2
                },
                {
                    field: "SoSup_Post",
                    dashType: "dash",
                    gruppo: 3
                },
                {
                    field: "IrriPrev",
                    type: "column",
                    gap: 1,
                    spacing: 0,
                    gruppo: 4,
                    axis: "Apporti_Axis",
                    color: "rgb(0, 206, 209)",
                    opacity: 0.5,
                    tooltipTemplate: "0.00",
                    stack: "ApportiStack"
                }
            ];

            //-------------------------------------------------------------------------------------
            // oggetto per controllare una riga verticale
            // posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
            //-------------------------------------------------------------------------------------
            let horzAxisCfg = {
                field: "Data",
                vertIndicator: {
                    field: "DataCalcolo",
                    label: {
                        text: "Data Calcolo",
                        color: "#808080" //"#800080"
                    },
                    line: {
                        color: "#808080",
                        width: 1,
                        dashType: "longDash"
                    }
                }
            };

            let titolo = "Bilancio idrico Irriframe";
            if (!plugin.indicatore.UsaMeteo) {
                if (typeof plugin.indicatore.InfoProvider === "string") {
                    if (plugin.indicatore.InfoProvider !== "") {

                        titolo += " - ";
                        titolo += plugin.indicatore.InfoProvider;
                    }
                }
            }

            creaKendoChart(titolo, horzAxisCfg, vertAxes, series, chart_ds, chart_id_1);

            if (hasUmTerr) {

                let chartWrapper2 = _creaDiv("chart-wrapper", "padding-top: 10px;");
                let chart_id_2 = div_id + "-2";
                let chart2 = _creaDiv("", "", chart_id_2);

                chartWrapper2.appendChild(chart2);

                chartWrapper.appendChild(chartWrapper2);

                let vertAxes2 = [
                    {
                        title: {
                            text: "Umidità terreno (%)"
                        }//,
                        //min: -10,
                        //max: max_perc,
                        //axisCrossingValue: Number.NEGATIVE_INFINITY,
                        //plotBands: [
                        //    {
                        //        from: -10,
                        //        to: 0,
                        //        color: "red",
                        //        opacity: 0.3
                        //    },
                        //    {
                        //        from: 100,
                        //        to: max_perc,
                        //        color: "green",
                        //        opacity: 0.3
                        //    }
                        //]
                    }//,
                    //{
                    //    name: "Apporti_Axis",
                    //    title: {
                    //        text: "Apporti in mm"
                    //    }
                    //}
                ];

                let series2 = [
                    {
                        name: "Soglia inferiore",
                        field: "SoInfPerc",
                        color: "rgb(178, 34, 34)",
                        gruppo: 1
                    },
                    {
                        name: "Soglia superiore",
                        field: "SoSupPerc",
                        color: "rgb(0, 128, 0)",
                        gruppo: 2
                    },
                    //{
                    //    name: "Piogge",
                    //    type: "column",
                    //    field: "Prec",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 128, 255)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    name: "Irrigazioni",
                    //    type: "column",
                    //    field: "Irri",
                    //    gap: 1,
                    //    spacing: 0,
                    //    axis: "Apporti_Axis",
                    //    color: "rgb(0, 206, 209)",
                    //    tooltipTemplate: "0.00"
                    //},
                    //{
                    //    field: "Umid_Post",
                    //    dashType: "dash",
                    //    gruppo: 1
                    //},
                    {
                        field: "SoInfPerc_Post",
                        dashType: "dash",
                        gruppo: 1
                    },
                    {
                        field: "SoSupPerc_Post",
                        dashType: "dash",
                        gruppo: 2
                    }
                ];

                let gruppo = 3;

                $.each(columns, function (i, e) {
                    if (e.field.startsWith("UT_")) {
                        series2.splice(0, 0,
                            {
                                name: e.title,
                                field: e.field,
                                //color: "rgb(255, 140, 0)",
                                gruppo: gruppo++
                            }
                        );
                    }
                });

                ////-------------------------------------------------------------------------------------
                //// oggetto per controllare una riga verticale
                //// posizione della riga impostata in datatsource aggiungendo il campo field nella posizione voluta...
                ////-------------------------------------------------------------------------------------
                //let horzAxisCfg = {
                //    field: "Data",
                //    vertIndicator: {
                //        field: "DataCalcolo",
                //        label: {
                //            text: "Data Calcolo",
                //            color: "#808080", //"#800080",
                //        },
                //        line: {
                //            color: "#808080",
                //            width: 1,
                //            dashType: "longDash"
                //        }
                //    }
                //};

                //let titolo = "Bilancio idrico Irriframe";
                //if (typeof plugin.indicatore.InfoProvider === "string") {
                //    if (plugin.indicatore.InfoProvider !== "") {

                //        titolo += " - ";
                //        titolo += plugin.indicatore.InfoProvider;
                //    }
                //}

                creaKendoChart("", "Data", vertAxes2, series2, chart_ds, chart_id_2);
            }
        };

        var _showLoader = function () {

            _clearElement(plugin.result);

            let divRel = _creaDiv("k-block", "position: relative; width: 100%; height: 100%;");
            plugin.result.appendChild(divRel);
            $(divRel).StyleLoader();
        };

        var _creaAdv = function (msg, bkg, clr) {

            let advStyle = "border-radius: 5px; border: 1px solid #aeaeae; display: flex;";
            if (bkg !== "") {
                advStyle += " background-color: " + bkg + ";";
            }
            if (clr !== "") {
                advStyle += " color: " + clr + ";";
            }
            //let msgStyle = "font-size: smaller; width: 100%; align-self: flex-end; text-align: center;";
            //let msgStyle = "width: 100%; align-self: center; text-align: center;";
            ////*
            //<div class="fa fa-smile-o fa-lg" style="color: red;"></div>
            //<div style="font-size: smaller;">02/05</div>                          
            ////*

            let divAdv = _creaDiv("", advStyle);
            let divMsg = _creaDiv("", "width: 100%; align-self: center; text-align: center;", "", msg);
            divAdv.appendChild(divMsg);

            return divAdv;
        };

        var _show = function () {

            _showLoader();

            let url = window.location.href.split("?")[0] + "/CalcolaWBResult";
            ajaxAgronica(url,
                JSON.stringify({ inPar: JSON.stringify(plugin.indicatore) }),
                function (risposta) {

                    let res = JSON.parse(risposta.RispostaStringa);

                    if (res.IrriframeData && res.IrriframeData.PlotId > 0 && res.IrriframeData.CropId > 0) {
                        plugin.indicatore.PersistData.Irri_PlotId = res.IrriframeData.PlotId;
                        plugin.indicatore.PersistData.Irri_CropId = res.IrriframeData.CropId;
                    }

                    _showResult(res);
                },
                function (risposta) {
                    console.log("ERRORE DSS Irrigazione (WBResult)");
                },
                null,
                false
            );
        };

        var _showResult = function (res) {

            _clearElement(plugin.result);

            if (res.Status !== 0) {

                let divMsg = _creaDiv("k-block", "height: 100%; text-align: center; display: flex; background-color: #f9f9f9; color: #aeaeae;");
                let span = document.createElement("span");
                span.style.cssText = "align-self: center;";
                span.innerHTML = res.Message;
                divMsg.appendChild(span);
                plugin.result.appendChild(divMsg);

                $(divMsg).on("click", function (e) {
                    if (e.ctrlKey) {
                        _impostaRunData();
                    }
                });
                return;
            }

            plugin.indicatore.InfoProvider = res.InfoProvider;
            plugin.indicatore.IrrigazionePrevista = null;

            let divContent = _creaDiv("", "height: 100%; display: grid; grid-gap: 3px; grid-template-rows: 2.5em auto 2.5em;");

            let msgIrri = "NO";
            let msgData = "";
            let msgVol = "";
            let msgCons = "";

            let dataEsecuzione = Date.parseLocale(res.DataEsecuzione);

            if (!isNaN(parseFloat(res.ConsumoMM))) {
                msgCons = kendo.toString(res.ConsumoMM, "0.00") + " mm";
            }

            let arrAdv = [
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" },
                { msg: "", bkg: "#ebebeb", clr: "" }
            ];

            if (typeof res.IrrigazioniPreviste === "object") {

                if (res.IrrigazioniPreviste.length > 0) {

                    msgIrri = "SI";

                    let idx = 0;
                    let cnt = Math.min(arrAdv.length - 1, res.IrrigazioniPreviste.length);

                    while (idx < cnt) {

                        let irri = res.IrrigazioniPreviste[idx];
                        let dt = new Date(irri.Data);
                        dt.setHours(0, 0, 0, 0);

                        if (plugin.indicatore.IrrigazionePrevista === null) {
                            plugin.indicatore.IrrigazionePrevista = {
                                Data: dt,
                                VolumeMM: irri.VolumeMM
                            }
                        }

                        if (idx === 0) {
                            msgData = kendo.toString(dt, "ddd dd-MM"); //VEN 27-03-2020
                            msgVol = kendo.toString(irri.VolumeMM, "0.00") + " mm";
                        }

                        arrAdv[idx + 1].msg = kendo.toString(dt, "dd/MM");

                        let diff_days = Math.floor((dt.getTime() - dataEsecuzione.getTime()) / (1000 * 60 * 60 * 24));
                        if (diff_days <= 3) {
                            arrAdv[idx + 1].bkg = "#f00000"; //rosso
                            arrAdv[idx + 1].clr = "#ffffff";
                        } else if (diff_days <= 7) {
                            arrAdv[idx + 1].bkg = "#fff000"; //giallo   "#ff8000" 'arancio
                        } else {
                            arrAdv[idx + 1].bkg = "#008000"; //verde
                            arrAdv[idx + 1].clr = "#ffffff";
                        }

                        idx++;
                    }
                }
            }

            if (typeof res.Table === "string") {
                if (res.Table !== "") {

                    let obj_table = JSON.parse(res.Table);
                    

                    if (typeof obj_table.kendo_rows === "object") {
                        if (obj_table.kendo_rows.length > 0) {

                            plugin.table = obj_table;

                            let idx = plugin.table.kendo_rows.length - 1;

                            while (idx >= 0) {

                                let elem = plugin.table.kendo_rows[idx];

                                if (elem.Irri > 0) {

                                    let elem_dt = kendo.parseDate(elem.Data);

                                    arrAdv[0].msg = kendo.toString(elem_dt, "dd/MM");
                                    arrAdv[0].bkg = "#00ced1"; //"#00bfff"; //
                                    arrAdv[0].clr = "#ffffff";

                                    if (elem_dt.getTime() > dataEsecuzione.getTime()) {
                                        //Irrigazione registrata nel futuro... (Devo farlo notare???)
                                    }

                                    idx = 0;
                                }
                                idx--;
                            }
                        }
                    }
                }
            }

            let divRow1 = _creaDiv("", "height: 2.5em; display: grid; grid-template-columns: 1fr auto 1fr 1fr; grid-gap: 3px;");

            divRow1.appendChild(_creaAdv(arrAdv[0].msg, arrAdv[0].bkg, arrAdv[0].clr));
            divRow1.appendChild(_creaDiv("", "font-size: 6px; align-self: center;", "", "<span class='fa fa-circle' style='margin: 0px;'></span>"));
            divRow1.appendChild(_creaAdv(arrAdv[1].msg, arrAdv[1].bkg, arrAdv[1].clr));
            divRow1.appendChild(_creaAdv(arrAdv[2].msg, arrAdv[2].bkg, arrAdv[2].clr));

            let divRow2 = _creaDiv("k-block k-info-colored ripple-cont", "font-size: smaller; display: grid; grid-template-rows: 1fr 1fr 1fr 1fr; align-items: center; ");
            let divIrri = _creaDiv();
            divIrri.innerHTML = "<span>IRRIGAZIONE:</span><span style='float: right; font-weight: bold;'>" + msgIrri + "</span>";
            let divData = _creaDiv();
            divData.innerHTML = "<span>DATA:</span><span style='float: right; font-weight: bold;'>" + msgData + "</span>";
            let divVol = _creaDiv();
            divVol.innerHTML = "<span>VOLUME:</span><span style='float: right; font-weight: bold;'>" + msgVol + "</span>";
            let divCons = _creaDiv();
            divCons.innerHTML = "<span>CONSUMO:</span><span style='float: right; font-weight: bold;'>" + msgCons + "</span>";
            divRow2.appendChild(divIrri);
            divRow2.appendChild(divData);
            divRow2.appendChild(divVol);
            divRow2.appendChild(divCons);

            let divRow3 = _creaDiv("", "display: flex;");
            //let btnReg = _creaDiv("k-button indic-tooltip", "margin-right: 2px; flex: auto;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            let btnReg = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button indic-tooltip", "flex: auto; background-color: #f5f5f5;", "", "<span class='fa fa-floppy-o fa-lg'></span>");
            btnReg.setAttribute("data-tooltip", "Registra irrigazione");
            divRow3.appendChild(btnReg);

            //let btnPia = _creaDiv("k-button indic-tooltip", "margin-left: 2px; flex: auto;", "", "<span class='fa fa-calendar fa-lg'></span>");
            //btnPia.setAttribute("data-tooltip", "Pianifica");
            //divRow3.appendChild(btnPia);

            divContent.appendChild(divRow1);
            divContent.appendChild(divRow2);
            divContent.appendChild(divRow3);

            plugin.result.appendChild(divContent);

            if (plugin.table !== undefined) {

                if (res.DataEsecuzione !== undefined) {

                    plugin.rundate = new Date(res.DataEsecuzione);
                    plugin.rundate.setHours(0, 0, 0, 0);
                }

                let ripple_cont = $(plugin.result).find(".ripple-cont");
                if (ripple_cont.length > 0) {

                    ripple_cont.addClass("irri-ripple");
                    ripple_cont[0].onclick = _show_table;
                    ripple_cont[0].insertBefore(_creaDiv("ripple"), ripple_cont[0].firstChild);
                }
            }

            if (plugin.indicatore.IrrigazionePrevista !== null) {

                btnReg.onclick = function () {
                    //_openIrriWin();
                    _querySaveAdvice();
                };
            } else {

                $(btnReg).addClass(GIAS_K_STATE_DISABLED);
            }
        };

        var _querySaveAdvice = function () {

            if (plugin.indicatore.IrrigazionePrevista === null) {
                //se non ho irrigazioni previste non devo salvare
                return;
            }

            let queryGIAS = true;
            let queryIF = true;

            if (!plugin.persistent) {

                queryIF = false;

            } else {

                if (typeof plugin.indicatore.PersistData !== "object") {
                    //se non ho la chiave irriframe non so dove salvare l'irrigazione
                    return;
                }

                if (plugin.indicatore.PersistData.Irri_PlotId <= 0 || plugin.indicatore.PersistData.Irri_CropId <= 0) {
                    //se non ho la chiave irriframe non so dove salvare l'irrigazione in Irriframe

                    queryIF = false;

                }
            }

            let win_el = document.createElement("div");
            document.body.appendChild(win_el);
            let $win_el = $(win_el);

            let content = "<div style='display: grid; grid-template-columns: auto 1fr; gap: 5px 10px;'>";
            content += "<div>Data</div>";
            content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.Data, "dddd d MMMM") + "</div>";
            content += "<div>Volume</div>";
            content += "<div style='font-weight: bold; justify-self: end;'>" + kendo.toString(plugin.indicatore.IrrigazionePrevista.VolumeMM, "0.00") + " mm</div>";
            content += "</div>";
            if (queryGIAS && queryIF) {

                content += "<div style='margin-top: 20px;'>";
                content += "    <div style='padding-bottom: 5px;'>";
                content += "        <input type='checkbox' id='__chk_gias' class='k-checkbox' style='margin: 0px; outline: none;'>";
                content += "        <label class='k-checkbox-label' for='__chk_gias'>Registra irrigazione in GIAS</label>";
                content += "    </div>";
                content += "    <div style='padding-top: 5px;'>";
                content += "        <input type='checkbox' id='__chk_if' class='k-checkbox' style='margin: 0px; outline: none;'>";
                content += "        <label class='k-checkbox-label' for='__chk_if'>Registra irrigazione in Irriframe</label>";
                content += "    </div>";
                content += "</div>";
            }

            $win_el.kendoDialog({
                title: "Registrazione irrigazione",
                closable: false,
                modal: true,
                visible: false,
                content: content,
                //open: function () {
                //},
                actions: [
                    {
                        text: "OK",
                        action: function (e) {

                            if (queryGIAS && queryIF) {

                                let chk_gias = $("#__chk_gias").prop("checked");
                                let chk_if = $("#__chk_if").prop("checked");

                                if (!chk_gias && !chk_if) {
                                    return false;
                                }

                                _saveAdvice(chk_gias, chk_if);

                            } else {

                                _saveAdvice(queryGIAS, queryIF);
                            }
                            return true;
                        }
                    },
                    {
                        text: "Annulla"
                    }
                ],
                close: function (e) {
                    this.destroy();
                }
            });

            $win_el.data("kendoDialog").open();
        };

        var _saveAdvice = function (save_gias, save_if) {

            let irri = {
                Data: plugin.indicatore.IrrigazionePrevista.Data,
                VolumeMM: plugin.indicatore.IrrigazionePrevista.VolumeMM
            };

            if (save_gias) {

                irri.GIAS_key = {
                    PIVA: plugin.indicatore.PersistData.GIAS_Piva,
                    SaCod: plugin.indicatore.PersistData.GIAS_SaCod,
                    Appezza: plugin.indicatore.PersistData.GIAS_Appezza,
                    IdReg: plugin.indicatore.PersistData.GIAS_IdReg,
                    VegCod: plugin.indicatore.Veg_Cod,
                    CulCod: plugin.indicatore.Cul_Cod,
                    SupImp: plugin.indicatore.Sup_Imp
                };
            }

            if (save_if) {

                irri.IF_key = {
                    IdPlot: plugin.indicatore.PersistData.Irri_PlotId,
                    IdCrop: plugin.indicatore.PersistData.Irri_CropId
                };
            }

            $.ajax({
                async: false,
                type: "POST",
                url: location.pathname + "/SalvaIrrigazione",
                data: JSON.stringify({ irri: irri }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    if (msg.d.RispostaOK) {

                        let objRisp = JSON.parse(msg.d.RispostaStringa);

                        if (objRisp.GIAS_Saved) {
                            plugin.indicatore.Irrigazioni.push(objRisp.Irrigazione);
                        }

                        if (objRisp.GIAS_Saved || objRisp.IF_Saved) {
                            //Ricalcolo il consiglio irriguo
                            _show();
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.status);
                    //alert(thrownError);
                }
            });
        };

        var _openIrriWin = function () {

            $.ajax({
                async: false,
                type: "POST",
                url: location.pathname + "/PreparaPaginaIrrigazione",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                    if (msg.d == 'SessioneScaduta') {

                        $('#dialogSessioneScaduta').dialog("open");
                    }
                    else {

                        if (msg.d.RispostaOK) {

                            let winElem = _creaDiv("", "padding:0px;");
                            document.body.appendChild(winElem);

                            let frameH = window.innerHeight * 0.9;
                            let frameElem = document.createElement("iframe");
                            frameElem.id = "Irri_IFrame";
                            frameElem.style.cssText = "width:-webkit-fill-available; height:" + frameH + "px; opacity:0;";
                            frameElem.setAttribute('frameborder', '0');
                            frameElem.setAttribute("src", "");
                            winElem.appendChild(frameElem);

                            let $win_el = $(winElem);

                            $win_el.kendoWindow({
                                title: "Registra irrigazione",
                                width: "90%",
                                draggable: false,
                                visible: false,
                                modal: true,
                                resizable: false,
                                actions: [
                                    "Close"
                                ],
                                open: function (e) {
                                    e.sender.element.css("opacity", "0");
                                    //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                                    $("body").addClass("overflow_hidden");
                                },
                                activate: function (e) {
                                    e.sender.element.css("opacity", "1");
                                    $("#Irri_IFrame").css("opacity", "1");
                                },
                                close: function (e) {
                                    $("body").removeClass("overflow_hidden");
                                    this.destroy();
                                }
                            });

                            //let parent = $win_el.parent();
                            //parent.find('.k-window-title').css('text-align', 'center');
                            //parent.css('padding-top', '48px');
                            //let titlebar = parent.find('.k-window-titlebar');
                            //titlebar.css({
                            //    "margin-top": "-48px",
                            //    "height": "35px",
                            //    "line-height": "35px",
                            //    "vertical-align": "middle"
                            //});

                            frameElem.setAttribute("src", msg.d.RispostaStringa);

                            $win_el.data("kendoWindow").center().open();
                        }
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        };

        var _impostaRunData = function () {

            let divWin = _creaDiv("", "display: none;");
            plugin.wrapper.appendChild(divWin);
            let divCalendar = _creaDiv();
            divWin.appendChild(divCalendar);
            let divBtns = _creaDiv("", "display: flex; gap: 5px;");
            divWin.appendChild(divBtns);
            let divBtnOk = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#5cb85c;", "", "<span class='fa fa-check fa-lg'></span>");
            divBtns.appendChild(divBtnOk);
            let divBtnCancel = _creaDiv("k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button", "width:100%; color:#d9534f;", "", "<span class='fa fa-times fa-lg'></span>");
            divBtns.appendChild(divBtnCancel);

            $(divCalendar).kendoCalendar({
                value: new Date(),
                footer: false
            });

            $(divWin).kendoWindow({
                title: "Imposta data esecuzione",
                draggable: true,
                visible: false,
                modal: true,
                resizable: false,
                actions: []
            });

            let kendoCalendar = $(divCalendar).getKendoCalendar();

            let kendoWindow = $(divWin).getKendoWindow();

            kendoWindow.bind("open", function (e) {
                //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                $("body").addClass("overflow_hidden");
            });
            kendoWindow.bind("close", function (e) {
                $("body").removeClass("overflow_hidden");
                kendoWindow.destroy();
            });

            kendoWindow.center().open();

            divBtnOk.onclick = function () {
                let dt = new Date(kendoCalendar.value());

                kendoWindow.close();

                dt.setUTCHours(0, 0, 0, 0);
                plugin.indicatore.RunData = dt.toISOString().split("T")[0];
                plugin.run();
            };

            divBtnCancel.onclick = function () {
                kendoWindow.close();
            };
        };

        //-----------------------------------------------------------------------------------------
        // fire up the plugin! 
        //-----------------------------------------------------------------------------------------

        //$(...).data("WBResult").run();

        //-----------------------------------------------------------------------------------------

    }; // WBResult

    //Add the plugin to the jQuery.fn object
    $.fn.WBResult = function (indicatore, persistent) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('WBResult')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.WBResult(this, indicatore, persistent);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('WBResult', plugin);
            }
        });
    };

})(jQuery);





(function ($) {

    $.MultiStateSwitch = function (elem, options) {

        var plugin = this;

        let divWrapper = document.createElement("div");
        divWrapper.className = "multistateswitch";
        divWrapper.style.cssText = "grid-template-columns: repeat(" + options.options.length + ", minmax(0, 1fr));";
        elem.appendChild(divWrapper);

        let id_postfix = document.getElementsByClassName("multistateswitch").length;
        plugin.radio_name = "switch_option_" + id_postfix;

        $.each(options.options, function (i, o) {

            let label = document.createElement("label");

            let radio = document.createElement("input");
            radio.type = "radio";
            radio.name = plugin.radio_name;
            radio.value = o.value;
            label.appendChild(radio);

            let insideLabel = document.createElement("div");
            insideLabel.textContent = o.text;
            label.appendChild(insideLabel);

            divWrapper.appendChild(label);
        });

        $("input:radio[name=" + plugin.radio_name + "]").change(function () {

            if (options.onchange) {
                options.onchange(parseInt(this.value));
            }
        });

        plugin.$elem = $(divWrapper);

        plugin.setValue = function (v) {

            plugin.$elem.find("input:radio[name=" + plugin.radio_name + "]").val(["" + v + ""]);
        }

    }; // MultiStateSwitch

    $.fn.MultiStateSwitch = function (options) {

        let stylesheet = document.getElementById("multistateswitch-stylesheet");
        if (!stylesheet) {

            let css = ".multistateswitch { ";
            css += "display: grid; ";
            css += "overflow: hidden; ";
            css += "background: #f5f5f5; ";
            css += "color: #a9a9a9; ";
            css += "border: 1px solid #ddd; ";
            css += "border-radius: 4px; ";
            css += "} ";

            css += ".multistateswitch input[type=radio] { display: none; } ";

            css += ".multistateswitch label { ";
            css += "width: 100%; ";
            css += "cursor: pointer; ";
            css += "margin: 0px; ";
            css += "height: 100%; ";
            css += "text-align: center; ";
            css += "vertical-align: middle; ";
            css += "} ";

            css += ".multistateswitch label > input[type=radio]:checked + div { ";
            css += "color: #333; ";
            css += "background-color: #fdfdfd; ";
            css += "} ";

            css += ".multistateswitch label:hover { background-color: #f9f9f9; } ";

            css += ".multistateswitch:hover { ";
            //css += "background: #ebebeb; ";
            css += "border-color: #aeaeae; ";
            css += "} ";

            css += ".multistateswitch label > div { ";
            css += "padding: 5px 5px; ";
            css += "overflow: hidden; ";
            css += "white-space: nowrap; ";
            css += "text-overflow: ellipsis;";
            css += "transition: all 100ms linear; ";
            css += "} ";

            let style = document.createElement('style');
            style.id = "multistateswitch-stylesheet";
            style.type = 'text/css';
            style.innerHTML = css;

            document.getElementsByTagName('head')[0].appendChild(style);
        }

        return this.each(function () {

            if (undefined == $(this).data('MultiStateSwitch')) {

                var plugin = new $.MultiStateSwitch(this, options);

                $(this).data('MultiStateSwitch', plugin);
            }
        });
    };

})(jQuery);







































