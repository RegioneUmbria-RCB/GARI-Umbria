

/* GST_Menu.js */
var jSonParsed_Kendo_Variazioni;

function ajax_url(ws) {
    let url = location.pathname;
    //splitto su / e prendo la parte finale
    let url_split = url.split("/")
    url = url_split[url_split.length - 1];

    return url + "/" + ws;
}

function modificaStatoInterferenza(lista_chiavi_interferenza) {
    let checked = $("input[name='opzione_" + lista_chiavi_interferenza + "']:checked");
    if (checked.length === 0) {
        return;
    }

    let val = $(checked[0]).val();

    $.ajax({
        type: "POST",
        url: ajax_url("SalvaModificheInterferenza"),
        data: "{ Lista_InterferenzaCod:'" + lista_chiavi_interferenza + "', Valore:'" + val + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d === "true") {

                mostraMessaggi();

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });


}

function vai_a_gis_da_interferenze(strEntitaCod) {

    if (typeof strEntitaCod !== "string")
        return;
    if (strEntitaCod === "")
        return;

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()
    let PrevCons = $("#lbl_preventivo_consuntivo").attr("data");

    $.ajax({
        type: "POST",
        url: ajax_url("GisDaInterferenza"),
        data: "{ EntitaCod: '" + strEntitaCod + "', Sportello: '" + Sportello + "', PrevCons: '" + PrevCons + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            open_window("Anteprima Interferenza", msg.d);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function vai_a_gis_da_log(strEntitaCod) {

    if (typeof strEntitaCod !== "string")
        return;
    if (strEntitaCod === "")
        return;

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()
    let PrevCons = $("#lbl_preventivo_consuntivo").attr("data");

    $.ajax({
        type: "POST",
        url: ajax_url("GisDaInterferenza"),
        data: "{ EntitaCod: '" + strEntitaCod + "', Sportello: '" + Sportello + "', PrevCons: '" + PrevCons + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            open_window("Localizzazione", msg.d);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function vai_a_gis_da_coord(strLat, strLng) {

    if (typeof strLat !== "string" || typeof strLng !== "string")
        return;
    if (strLat === "" || strLng === "")
        return;

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()
    let PrevCons = $("#lbl_preventivo_consuntivo").attr("data");

    $.ajax({
        type: "POST",
        url: ajax_url("GisDaCoordinate"),
        data: "{ Lat: '" + strLat + "', Lng: '" + strLng + "', Sportello: '" + Sportello + "', PrevCons: '" + PrevCons + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            open_window("Localizzazione", msg.d);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function open_window(titolo, url) {

    let winElem_id = "GST_KendoWindow";
    let winElem = document.createElement("div");
    winElem.style.cssText = "padding:0px;";
    winElem.id = winElem_id; 
    document.body.appendChild(winElem);
    let $win_el = $("#" + winElem_id);

    let frameH = window.innerHeight * 0.9;

    let frameElem = document.createElement("iframe");
    frameElem.id = "GST_IFrame";
    frameElem.style.cssText = "width:-webkit-fill-available; height:" + frameH + "px;";
    frameElem.setAttribute('frameborder', '0');     
    //frameElem.setAttribute('src', url);     
    winElem.appendChild(frameElem);

    $win_el.kendoWindow({
        title: titolo,
        width: "90%",
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        actions: [
            "Close"
        ],
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            e.sender.element.css("opacity", "0");
            $("body").addClass("body_overflow_hidden");
        },
        activate: function (e) {
            e.sender.element.css("opacity", "1");
        },
        close: function (e) {
            $("body").removeClass("body_overflow_hidden");
            this.destroy();
        }
    });

    //let parent = $win_el.parent();
    //parent.find('.k-window-title').css('text-align', 'center');
    //parent.css('padding-top', '48px');
    //let titlebar = parent.find('.k-window-titlebar');
    //titlebar.css({
    //    "margin-top": "-48px",
    //    "font-size": "large"
    //});

    frameElem.setAttribute('src', url);

    $win_el.data("kendoWindow").center().open();
}

function open_window_content(titolo, content) {

    let winElem_id = "GST_KendoWindow";
    let winElem = document.createElement("div");
    winElem.style.cssText = "padding:0px;";
    winElem.id = winElem_id;
    document.body.appendChild(winElem);
    let $win_el = $("#" + winElem_id);

    let divH = window.innerHeight * 0.9;

    let divElem = document.createElement("div");
    divElem.id = "GST_Content";
    divElem.style.cssText = "width:-webkit-fill-available; max-height:" + divH + "px;";
    divElem.innerHTML = content;
    winElem.appendChild(divElem);

    $win_el.kendoWindow({
        title: titolo,
        width: "90%",
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        actions: [
            "Close"
        ],
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            e.sender.element.css("opacity", "0");
            $("body").addClass("body_overflow_hidden");
        },
        activate: function (e) {
            e.sender.element.css("opacity", "1");
        },
        close: function (e) {
            $("body").removeClass("body_overflow_hidden");
            this.destroy();
        }
    });

    $win_el.data("kendoWindow").center().open();
}

function stampaElenco() {

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()

    $.ajax({
        type: "POST",
        url: ajax_url("StampaElenco"),
        data: "{ Sportello: '" + Sportello + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {
                window.location.href = msg.d;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function nuovoImpianto(flagSportello) {

    let Sportello = "-1";
    let PrevCons = "";
    let SportelloDdlValue = $("#ddl_sportello").data("kendoDropDownList").value();

    if (flagSportello) {
        Sportello = SportelloDdlValue;
        PrevCons = $("#lbl_preventivo_consuntivo").attr("data");
    } 

    $.ajax({
        type: "POST",
        url: ajax_url("NuovoImpianto"),
        data: "{ Sportello: '" + Sportello + "', PrevCons: '" + PrevCons + "', SportelloDdlValue: '" + SportelloDdlValue + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {
                window.location.href = msg.d;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

}

function salvaVariazioni() {
    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value();
    SalvaVariazioniWS(Sportello, jsonParsed_Kendo_Variazioni);
}

function verificaInterferenze() {

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()
    let PrevCons = $("#lbl_preventivo_consuntivo").attr("data");

    $.ajax({
        type: "POST",
        url: ajax_url("VerificaInterferenze"),
        data: "{ Sportello: '" + Sportello + "', PrevCons: '" + PrevCons + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {
                window.location.href = msg.d;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

}

function distanzeMinime() {

    $.ajax({
        type: "POST",
        url: ajax_url("TabellaDistanze"),
        data: JSON.stringify(new Object()),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {

                //open_window_content("Distanze minime isolamenti", msg.d);

                _mostraDistanzeMinime(JSON.parse(msg.d));
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function _mostraDistanzeMinime(obj_json) {

    if (obj_json === null) {
        return;
    }

    let winElem = document.createElement("div");
    winElem.style.padding = "0px";
    document.body.appendChild(winElem);

    let $win_el = $(winElem);

    let divH = window.innerHeight * 0.9;

    let contentElem = document.createElement("div");
    contentElem.style.width = "-webkit-fill-available";
    contentElem.style.maxHeight = divH + "px";
    contentElem.style.padding = "20px";

    winElem.appendChild(contentElem);

    let gridElem = document.createElement("div");
    gridElem.style.display = "grid";
    gridElem.style.border = "1px solid #ccc";

    contentElem.appendChild(gridElem);

    let curr_col = 1;

    let livello2 = [];

    let first_col = " first-column";

    $.each(obj_json.header, function (i, c) {

        let cell = document.createElement("div");
        cell.className = "grid-item grid-header first-row" + first_col;
        cell.textContent = c.title;

        first_col = "";

        if (c.columns) {

            cell.style.gridColumn = curr_col + " / span 2";
            cell.style.justifyContent = "center";

            $.each(c.columns, function (j, cc) {

                let cell2 = document.createElement("div");
                cell2.className = "grid-item grid-header";
                cell2.style.gridColumn = "" + (curr_col + j);
                cell2.textContent = cc.title;

                livello2.push(cell2);
            });

            curr_col += c.columns.length;

        } else {

            cell.style.gridColumn = curr_col + " / span 1";
            cell.style.gridRow = "1 / span 2";

            if (curr_col === 1) {

                cell.style.justifyContent = "center";
            }            

            curr_col++;
        }

        gridElem.appendChild(cell);
    });

    $.each(livello2, function (i, e) {
        gridElem.appendChild(e);
    });

    $.each(obj_json.rows, function (i, r) {

        first_col = " first-column";

        $.each(obj_json.header, function (j, c) {

            if (j === 0) {

                let cell = document.createElement("div");
                cell.className = "grid-item grid-header" + first_col;
                cell.style.justifyContent = "flex-start";
                cell.textContent = r[c.field];

                first_col = "";

                gridElem.appendChild(cell);

            } else {

                if (c.columns) {

                    if ($.isPlainObject(r[c.field])) {

                        $.each(c.columns, function (h, cc) {

                            let cell = document.createElement("div");
                            cell.className = "grid-item";
                            cell.textContent = kendo.toString(r[c.field][cc.field], "0");

                            gridElem.appendChild(cell);
                        });

                    } else {

                        let cell = document.createElement("div");
                        cell.className = "grid-item";
                        cell.style.gridColumn = "auto / span " + c.columns.length;
                        cell.style.justifyContent = "center";
                        cell.textContent = kendo.toString(r[c.field], "0");

                        gridElem.appendChild(cell);
                    }

                } else {

                    let cell = document.createElement("div");
                    cell.className = "grid-item";
                    cell.textContent = kendo.toString(r[c.field], "0");

                    gridElem.appendChild(cell);
                }
            }
        });
    });



    $win_el.kendoWindow({
        title: "Distanze minime isolamenti",
        width: "90%",
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        actions: [
            "Close"
        ],
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            e.sender.element.css("opacity", "0");
            $("body").addClass("body_overflow_hidden");
        },
        activate: function (e) {
            e.sender.element.css("opacity", "1");
        },
        close: function (e) {
            $("body").removeClass("body_overflow_hidden");
            this.destroy();
        }
    });
   
    $win_el.getKendoWindow().center().open();
}

function codificaSpecie() {
    $.ajax({
        type: "POST",
        url: ajax_url("TabellaCodifica"),
        data: "{ }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {

                open_window_content("Codifica specie vegetali", msg.d);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function leggiDDLSportello() {

    let rval = [];

    let sportelli_precedenti = $("#ddl_sportello").attr("sportelli-precedenti");

    $.ajax({
        type: "POST",
        url: ajax_url("ElencoSportelli"),
        data: "{ SportelliPrecedenti: '" + sportelli_precedenti + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            rval = JSON.parse(msg.d);

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

    return rval;
}

function impostaPreventivoConsuntivo(sportello) {

    $.ajax({
        type: "POST",
        url: ajax_url("ImpostaPreventivoConsuntivo"),
        data: "{ Sportello: '" + sportello + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            let text = "";
            let data = "";
            let info = "";
            let enable = false;

            if (msg.d !== "") {

                let obj = JSON.parse(msg.d);

                text = obj.text;
                data = obj.data;
                info = obj.info;
                enable = true;
            }

            if (text.trim() === "") {
                text = "\xa0"; //&nbsp;
            }


            $("#lbl_preventivo_consuntivo").removeClass("TestoRosso");
            if (data !== "") {
                if (data.split("|")[4] == -1) {
                    $("#lbl_preventivo_consuntivo").addClass("TestoRosso");
                }
            }
            $("#lbl_preventivo_consuntivo").text(text);
            $("#lbl_preventivo_consuntivo").attr("data", data);
            $("#id_info_sportello").html(info);
            $("#btnVerificaInterferenze").data("kendoButton").enable(enable);

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

}

function mostraMessaggi() {

    let Sportello = $("#ddl_sportello").data("kendoDropDownList").value()

    $.ajax({
        type: "POST",
        url: ajax_url("MostraMessaggi"),
        data: "{ Sportello: '" + Sportello + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            notificaInterferenze(msg.d.Interferenze);
            //$("#placeInterferenze").html(msg.d.Interferenze);

            //$("#tabs").css("margin-top", "5px");
            //$("#tabs").kendoTabStrip({
            //    animation: false,
            //    scrollable: false
            //});

            //ridimensionaTabs();
            jsonParsed_Kendo_Variazioni = msg.d.Notifiche;
            mostraNotizie(msg.d.Notifiche);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

}

//function sportelliPrecedenti(checked) {

//    $("#ddl_sportello").attr("sportelli-precedenti", (checked ? "1" : "0"));

//    $("#ddl_sportello").data("kendoDropDownList").setDataSource(leggiDDLSportello());
//}

function gestioneSportello() {
    $.ajax({
        type: "POST",
        url: ajax_url("GestioneSportello"),
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {
            if (msg.d !== "") {
                window.location.href = msg.d;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function InfoSportello() {

    let sportello = $("#ddl_sportello").data("kendoDropDownList").value()

    let content = "";

    $.ajax({
        type: "POST",
        url: ajax_url("InfoSportello"),
        data: "{ Sportello: '" + sportello + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                let elencoFasi = JSON.parse(msg.d);

                content += "<div style='padding-left: 15px; padding-right: 15px; display: grid; grid-template-columns: repeat(8, auto); grid-gap: 10px 20px;'>";
                content += "<div style='font-weight:bold;'></div>";
                content += "<div style='font-weight:bold;'>Fase</div>";
                content += "<div style='font-weight:bold;'>Data inizio</div>";
                content += "<div style='font-weight:bold;'>Data fine</div>";
                content += "<div style='justify-self:center; font-weight:bold;'>Visibilità impianti</div>";
                content += "<div style='justify-self:center; font-weight:bold;'>Operazioni permesse</div>";
                content += "<div style='justify-self:center;font-weight:bold;'>Comunica operazioni</div>";
                content += "<div style='justify-self:center;font-weight:bold;'>Notifiche attive</div>";

                let clr_style;
                $.each(elencoFasi, function (idx, elem) {

                    clr_style = "color: #aaa;";
                    content += "<div>";
                    if (elem.attiva) {

                        content += "<span class='fa fa-hand-o-right fa-lg'></span>";
                        clr_style = "";
                    }
                    content += "</div>";
                    content += "<div style='" + clr_style + "'>" + elem.fase + "</div>";
                    content += "<div style='" + clr_style + "'>" + kendo.toString(new Date(elem.dataInizio), "dd MMMM yyyy") + "</div>";
                    content += "<div style='" + clr_style + "'>" + kendo.toString(new Date(elem.dataFine), "dd MMMM yyyy") + "</div>";
                    content += "<div style='justify-self: center; " + clr_style + "'>" + elem.visImpianti + "</div>";
                    content += "<div style='justify-self: center;" + clr_style + "'>" + elem.operPermesse + "</div>";
                    content += "<div style='justify-self: center;" + clr_style + "'>" + elem.logOperazioni + "</div>";
                    content += "<div style='justify-self: center;" + clr_style + "'>" + elem.notificaInterferenze + "</div>";

                });
                content += "</div>";
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

    if (content === "") {
        return;
    }

    let win_el = document.createElement("div");
    win_el.id = "id_Info_Sportello_kendoDlg";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    $win_el.kendoDialog({
        title: "Informazioni sportello",
        closable: false,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: 'OK'
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}


function notificaInterferenze(json_notifiche) {

    $("#tabs").remove();

    let notifiche = JSON.parse(json_notifiche);

    let tabs = document.createElement("div");
    tabs.id = "tabs";
    tabs.className = "trasparente";
    tabs.style.marginTop = "5px";

    if (notifiche.globali !== undefined) {

        let ul = document.createElement("ul");
        let li = document.createElement("li")
        li.className = GIAS_K_STATE_ACTIVE;
        li.textContent = "Interferenze";
        ul.appendChild(li);

        tabs.appendChild(ul);

        let container = document.createElement("div");
        container.className = "interferenze-group-container";

        tabs.appendChild(container);

        $.each(notifiche.globali, function (idx, elem) {

            _mostra_notifica(container, elem, true, true);
        });

    } else {

        let ul = document.createElement("ul");
        let li_prop = document.createElement("li")
        li_prop.className = GIAS_K_STATE_ACTIVE;
        li_prop.textContent = "Proprietario";

        let li_interf = document.createElement("li")
        li_interf.textContent = "Interferente";

        ul.appendChild(li_prop);
        ul.appendChild(li_interf);

        tabs.appendChild(ul);

        let container_prop = document.createElement("div");
        container_prop.className = "interferenze-group-container";

        tabs.appendChild(container_prop);

        $.each(notifiche.proprietario, function (idx, elem) {

            _mostra_notifica(container_prop, elem, true, false);
        });



        let container_interf = document.createElement("div");
        container_interf.className = "interferenze-group-container";

        tabs.appendChild(container_interf);

        $.each(notifiche.interferente, function (idx, elem) {

            _mostra_notifica(container_interf, elem, false, false);
        });
    }

    document.getElementById("placeInterferenze").appendChild(tabs);

    $("#tabs").kendoTabStrip({
        animation: false,
        scrollable: false
    });

    ridimensionaTabs();

    $(tabs).removeClass("trasparente");
}

function _mostra_notifica(container, notifica, proprietario, superuser) {

    let wrapper = document.createElement("div");
    wrapper.className = "interferenze-group";
    container.appendChild(wrapper);

    let data_entita = "" + notifica.richiedente.entita_cod;
    $.each(notifica.interferenti, function (idx, elem) {
        data_entita += "|" + elem.entita_cod;
    });

    $(wrapper).data("entita", data_entita);

    let needLayer = proprietario || superuser;

    let row1 = _add_notifica_row(wrapper, "display:flex; align-items:center; justify-content:space-between;");
    let div1 = document.createElement("div");
    row1.appendChild(div1);

    if (needLayer) {

        _notifica_info(div1, "Richiedente", notifica.richiedente.layer, "font-size: 1.5em;");
    } else {

        _notifica_indirizzo(div1, notifica.richiedente, "Campo in via");
    }

    let btn_gis = _notifica_btn(row1, "fa fa-globe fa-lg");
    btn_gis.onclick = function () {

        let params = $(this).closest(".interferenze-group").data("entita");

        if (params) {

            vai_a_gis_da_interferenze(params);
        }
    }

    if (needLayer) {
        let row1_ = _add_notifica_row(wrapper);
        _notifica_indirizzo(row1_, notifica.richiedente, "Campo in via");
    }

    _notifica_row_coord(wrapper, notifica.richiedente.coord);
    _notifica_row_specie(wrapper, notifica.richiedente);

    let divLista = document.createElement("div");
    divLista.className = "interferenti-lista";

    wrapper.appendChild(divLista);

    needLayer = !proprietario || superuser;

    $.each(notifica.interferenti, function (idx, elem) {

        let interferente = document.createElement("div");
        interferente.className = "interferente";

        divLista.appendChild(interferente)

        let number = document.createElement("div");
        number.className = "interferente-number";
        let span_number = document.createElement("span");
        span_number.textContent = idx + 1;
        number.appendChild(span_number);
        interferente.appendChild(number);

        let content_status = document.createElement("div");
        content_status.className = "interferente-content status-" + elem.stato.cod;

        interferente.appendChild(content_status);

        if (!elem.attiva) {
            
            let overlay = document.createElement("div");
            overlay.className = "interferenze-non-attive-overlay";

            content_status.appendChild(overlay);
        }

        let content = document.createElement("div");
        content.style.padding = "5px 10px";
        content_status.appendChild(content);

        if (needLayer) {
            let row0 = _add_notifica_row(content);
            _notifica_info(row0, "Notificato a", elem.layer, "font-size: 1.3em;");
        }

        let row1 = _add_notifica_row(content);
        _notifica_indirizzo(row1, elem, "");

        _notifica_row_coord(content, elem.coord);
        _notifica_row_specie(content, elem);

        let row2 = _add_notifica_row(content);

        if (elem.data_creazione) {

            _notifica_info(row2, "Data", kendo.toString(new Date(elem.data_creazione), "dd/MM/yyyy"));
        }

        let strDist = "Non disponibile";

        if (elem.distanza > 0) {

            strDist = kendo.toString(elem.distanza, "0") + " mt.";
        }

        _notifica_info(row2, "Distanza", strDist);

        let div_stato = null;
        let css = "";

        if (elem.stato.cod === 1 && !needLayer) {

            css = "display:flex; align-items:center;"
            div_stato = document.createElement("div");
            div_stato.style.flexGrow = "1";
        }

        let row3 = _add_notifica_row(content, css);

        if (div_stato != null) {

            row3.appendChild(div_stato);
        } else {

            div_stato = row3;
        }

        _notifica_info(div_stato, "Stato", elem.stato.descr);

        if (elem.stato.cod === 1 && !needLayer) {


            let divActions = document.createElement("div");
            divActions.className = "btns-accetta-rifiuta";
            row3.appendChild(divActions);

            let btn_accetta = _notifica_btn(divActions, "accetta fa fa-fw fa-thumbs-up", "Accetta");
            $(btn_accetta).addClass(GIAS_K_STATE_DISABLED).data("interferenza_cod", elem.interferenza_cod);

            let btn_rifiuta = _notifica_btn(divActions, "rifiuta fa fa-fw fa-thumbs-down", "Rifiuta");
            $(btn_rifiuta).addClass(GIAS_K_STATE_DISABLED).data("interferenza_cod", elem.interferenza_cod);

            btn_accetta.onclick = function () {
                azioneInterferenza(this, true);
            }
           
            btn_rifiuta.onclick = function () {
                azioneInterferenza(this, false);
            }

            let btn_unlock = _notifica_btn(divActions, "fa fa-fw fa-unlock");
            btn_unlock.onclick = function () {

                attivaAzione(this);
            }

        } else {

            if (elem.stato.cod !== 1 && elem.data_conferma) {

                _notifica_info(row3, "in data", kendo.toString(new Date(elem.data_conferma), "dd/MM/yyyy"));
            }
        }
    });
}

function _notifica_btn(parent_div, fa_class, text) {

    let btn = document.createElement("div");
    btn.className = "k-button";
    let icon = document.createElement("span");
    icon.className = fa_class;
    btn.appendChild(icon);

    if (text) {
        let span = document.createElement("span");
        span.textContent = text;
        btn.appendChild(span);
    }

    parent_div.appendChild(btn);

    return btn;
}

function _add_notifica_row(parent_div, style_css) {

    let div_row = document.createElement("div");
    div_row.className = "interferenze-row";

    if (style_css) {
        div_row.style.cssText = style_css;
    }

    if (parent_div) {
        parent_div.appendChild(div_row);
    }

    return div_row;
}

function _notifica_info(parent_div, label, text, style_css) {

    if (label != "") {

        let spanLabel = document.createElement("span");
        spanLabel.className = "etichetta";
        spanLabel.textContent = label;

        parent_div.appendChild(spanLabel);
    }

    let spanInfo = document.createElement("span");
    spanInfo.className = "info";
    spanInfo.textContent = text;
    if (style_css) {

        spanInfo.style.cssText = style_css;
    }

    parent_div.appendChild(spanInfo);
}

function _notifica_indirizzo(parent_div, obj, label) {

    if (label != "") {
        let spanLabel = document.createElement("span");
        spanLabel.className = "etichetta";
        spanLabel.textContent = label;

        parent_div.appendChild(spanLabel);
    }

    if (obj.indirizzo != "") {

        let span = document.createElement("span");
        span.className = "info";
        span.textContent = obj.indirizzo;
        parent_div.appendChild(span);

        return;
    }

    let span = null;
    if (obj.indirizzo_centro.via != "") {

        span = document.createElement("span");
        span.className = "info";
        span.textContent = obj.indirizzo_centro.via;

        parent_div.appendChild(span);
    }

    let str = obj.indirizzo_centro.pr;
    if (obj.indirizzo_centro.com != "") {

        if (str != "") {

            str = " - " + str;
        }

        str = obj.indirizzo_centro.com + str;
    }

    if (str != "") {

        str = "(" + str + ")";
    }

    if (obj.indirizzo_centro.fraz != "") {

        if (str != "") {

            str = " " + str;
        }

        str = obj.indirizzo_centro.fraz + str;
    }

    if (str != "") {

        let span2 = document.createElement("span");

        if (span != null) {

            span2.style.marginLeft = "0.5em";
        }

        span2.textContent = str;

        parent_div.appendChild(span2);
    }
}

function _notifica_row_coord(parent_div, obj_coord) {

    if (obj_coord.lat > 0 && obj_coord.lng > 0) {

        let row = _add_notifica_row(parent_div);

        _notifica_info(row, "Lat", kendo.toString(obj_coord.lat, "0.000000"));
        _notifica_info(row, "Lng", kendo.toString(obj_coord.lng, "0.000000"));
    }
}

function _notifica_row_specie(parent_div, obj) {

    if (obj.specie != "" || obj.tipologia != "") {

        let row = _add_notifica_row(parent_div);

        if (obj.specie != "") {

            _notifica_info(row, "Specie", obj.specie);
        }

        if (obj.tipologia != "") {

            _notifica_info(row, "Tipologia", obj.tipologia);
        }
    }
}

function attivaAzione(sender) {

    $(sender).closest(".btns-accetta-rifiuta").find(".k-button." + GIAS_K_STATE_DISABLED).each(function (idx, elem) {

        $(elem).removeClass(GIAS_K_STATE_DISABLED);
    });

    $(sender).addClass(GIAS_K_STATE_DISABLED);
}

function azioneInterferenza(sender, flagAccetta) {

    let interferenza_cod = $(sender).data("interferenza_cod");
    if (!interferenza_cod) {
        return;
    }

    $.ajax({
        type: "POST",
        url: ajax_url("AccettaRifiutaInterferenza"),
        data: JSON.stringify({ Interferenza_Cod: interferenza_cod, FlagAccetta: flagAccetta }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d === "true") {

                mostraMessaggi();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function hidden_log(class_name, text)
{
    let span = document.createElement("span")
    span.className = class_name;
    span.textContent = text;

    return span;
}

function mostraNotizie(json_notizie) {

    //$("#placeNotifiche").html(notizie);

    $(".notizie-content").remove();

    let notizie = JSON.parse(json_notizie);

    let wrapper = document.createElement("div");
    wrapper.className = "notizie-content"; 
    document.getElementById("placeNotifiche").appendChild(wrapper);

    $.each(notizie, function (i, n) {

        let divNotizia_parent = document.createElement("div");
        divNotizia_parent.className= "notizia";
        wrapper.appendChild(divNotizia_parent);



        let hidden_div = document.createElement("div");
        hidden_div.className = "Sementieri--log";
        hidden_div.style.display = "none";
        divNotizia_parent.appendChild(hidden_div);

        hidden_div.appendChild(hidden_log("log--entita-cod", n.entita_cod));
        hidden_div.appendChild(hidden_log("log--ditta", n.utente));
        hidden_div.appendChild(hidden_log("log--rag-soc", n.rag_soc));

        let hidden_indirizzo = "";

        if (n.indirizzo != "") {

            hidden_indirizzo = n.indirizzo;

        } else {

            hidden_indirizzo = n.indirizzo_centro.via;
            if (n.indirizzo_centro.fraz != "")
            {
                hidden_indirizzo += " " + n.indirizzo_centro.fraz;
            }
            if (n.indirizzo_centro.com != "")
            {
                hidden_indirizzo += " - " + n.indirizzo_centro.com;
            }
            if (n.indirizzo_centro.prov != "") {
                hidden_indirizzo += " (" + n.indirizzo_centro.prov + ")";
            }
        }
        hidden_div.appendChild(hidden_log("log--indirizzo", hidden_indirizzo.trim()));
        hidden_div.appendChild(hidden_log("log--regione", n.regione));
        hidden_div.appendChild(hidden_log("log--specie", n.specie));
        hidden_div.appendChild(hidden_log("log--tipologia", n.tipologia));
        hidden_div.appendChild(hidden_log("log--nome-scientifico", n.nome_scientifico));
        hidden_div.appendChild(hidden_log("log--superficie", kendo.toString(n.superficie, "0.000000")));
        hidden_div.appendChild(hidden_log("log--latitudine", kendo.toString(n.coord.lat, "0.000000")));
        hidden_div.appendChild(hidden_log("log--longitudine", kendo.toString(n.coord.lng, "0.000000")));
        let hidden_variazione = "inserimento";
        if (n.tipo_operazione === 2) {
            hidden_variazione = "modifica";
        } else {
            if (n.tipo_operazione === 3)
            {
                hidden_variazione = "eliminazione";
            }
        }
        hidden_div.appendChild(hidden_log("log--variazione", hidden_variazione));



        let divNotizia = document.createElement("div");
        divNotizia.className = "notizia-padded";
        divNotizia_parent.appendChild(divNotizia);

        let operazione = "Operazione sconosciuta";
        let icon_class = "fa-question";
        switch (n.tipo_operazione) {
            case 1:
                operazione = "Nuovo appezzamento";
                icon_class = "fa-plus";
                break;
            case 2:
                operazione = "Modificato appezzamento";
                icon_class = "fa-asterisk";
                break;
            case 3:
                operazione = "Eliminato appezzamento";
                icon_class = "fa-minus";
                break;
        }

        let divIcon = document.createElement("div");
        divIcon.className = "notizia-icon";
        let spanIcon = document.createElement("span");
        spanIcon.className = "fa fa-fw " + icon_class;
        divIcon.appendChild(spanIcon);
        divNotizia.appendChild(divIcon);

        let row1 = _notizia_row(divNotizia);
        row1.style.cssText = "display: grid; grid-template-columns: auto auto 1fr auto; align-items: baseline; grid-gap: 15px;";

        let span_data = document.createElement("span");
        span_data.textContent = kendo.toString(new Date(n.data), "dd/MM/yyyy HH:mm");
        span_data.style.fontWeight = "bold";
        let span_uten = document.createElement("span");
        span_uten.textContent = n.utente;
        span_uten.style.fontSize = "1.3em";
        let span_oper = document.createElement("span");
        span_oper.textContent = operazione;
        span_oper.style.justifySelf = "end";

        row1.appendChild(span_data);
        row1.appendChild(span_uten);
        row1.appendChild(span_oper);

        let btn_gis = document.createElement("div");
        btn_gis.className = "k-button";
        btn_gis.style.padding = "4px 8px";
        let globe = document.createElement("span");
        globe.className = "fa fa-globe fa-lg";
        btn_gis.appendChild(globe);
        row1.appendChild(btn_gis);

        if (n.tipo_operazione != 3) {

            $(btn_gis).data("entita-cod", n.entita_cod);
        } else {

            $(btn_gis).data("lat-lng", [n.coord.lat, n.coord.lng]);
        }

        btn_gis.onclick = function () {

            let data = $(this).data("entita-cod");

            if (data) {

                vai_a_gis_da_log("" + data);

            } else {

                data = $(this).data("lat-lng");

                if (data) {

                    vai_a_gis_da_coord("" + data[0], "" + data[1]);
                }
            }
        }


        let row2 = _notizia_row(divNotizia);

        if (n.indirizzo != "") {

            _notizia_info(row2, n.indirizzo, "font-weight: bold;");

        } else {

            if (n.indirizzo_centro.via != "") {

                _notizia_info(row2, n.indirizzo_centro.via, "font-weight: bold;");
            }

            let str = n.indirizzo_centro.prov;
            if (n.indirizzo_centro.com != "") {

                if (str != "") {

                    str = " - " + str;
                }

                str = n.indirizzo_centro.com + str;
            }

            if (str != "") {

                str = "(" + str + ")";
            }

            if (n.indirizzo_centro.fraz != "") {

                if (str != "") {

                    str = " " + str;
                }

                str = n.indirizzo_centro.fraz + str;

            }
            if (str != "") {

                _notizia_info(row2, str);
            }
        }

        if (n.coord.lat > 0 && n.coord.lng > 0) {

            let row3 = _notizia_row(divNotizia);
            _notizia_info_label(row3, "Lat", kendo.toString(n.coord.lat, "0.000000"));
            _notizia_info_label(row3, "Lng", kendo.toString(n.coord.lng, "0.000000"));
        }

        let row4 = _notizia_row(divNotizia);
        _notizia_info_label(row4, "Specie", n.specie);
        _notizia_info_label(row4, "Tipologia", n.tipologia);

    });
}

function _notizia_row(parent_div) {

    let row = document.createElement("div");
    row.className = "notizia-row";
    parent_div.appendChild(row);

    return row;
}

function _notizia_info(parent_div, text, css_style) {

    let span = document.createElement("span");
    span.className = "notizia-info";
    if (css_style) {

        span.style.cssText = css_style;
    }
    span.textContent = text
    parent_div.appendChild(span);

    return span;
}

function _notizia_info_label(parent_div, label, info) {

    let span = document.createElement("span");
    span.className = "notizia-info";
    parent_div.appendChild(span);

    let span_label = document.createElement("span");
    span_label.className = "notizia-info-label";
    span_label.textContent = label;
    span.appendChild(span_label);

    let span_info = document.createElement("span");
    span_info.className = "notizia-info-text";
    span_info.textContent = info;
    span.appendChild(span_info);

    return span;
}

