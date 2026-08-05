
function ajax_url(ws) {
    let url = location.pathname;
    //splitto su / e prendo la parte finale
    let url_split = url.split("/")
    url = url_split[url_split.length - 1];

    return url + "/" + ws;
}

function CaricaGrigliaSportelli(options) {
    $.ajax({
        type: "POST",
        url: ajax_url("CaricaGrigliaSportelli"),
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                options.success(JSON.parse(msg.d));

            } else {

                options.success([]);

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function editSportello(id_sportello) {

    let multisel = $("#inSpecie").data("kendoMultiSelect");

    if (multisel.dataSource.data().length === 0) {
        $.ajax({
            type: "POST",
            url: ajax_url("CaricaElencoSpecie"),
            data: "{ }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {

                if (msg.d !== "") {

                    let ds = new kendo.data.DataSource({ data: JSON.parse(msg.d) });
                    multisel.setDataSource(ds);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    }

    if (typeof id_sportello !== "number") {
        id_sportello = 0;
    }

    $(cIdSportello).val(id_sportello);

    $.ajax({
        type: "POST",
        url: ajax_url("LeggiSportello"),
        data: "{id_sportello: " + id_sportello + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                let objSportello = JSON.parse(msg.d);

                $("#inDescrizione").val(objSportello.descrizione);
                $("#inValiditaInizio").data("kendoDatePicker").value(objSportello.validitaInizio);
                $("#inValiditaFine").data("kendoDatePicker").value(objSportello.validitaFine);

                let specie = [];
                $.each(objSportello.classi, function (i, cl) {
                    specie.push(cl.IdSpecie);
                });
                multisel.value(specie);

                $.each(objSportello.passaggi, function (i, pass) {

                    pass.Passaggio = {
                        IdPassaggio: pass.IdPassaggio,
                        DesPassaggio: pass.DesPassaggio,
                        VisImpianti: pass.VisImpianti,
                        OpPermesse: pass.OpPermesse,
                        LogOperazioni: pass.LogOperazioni,
                        Notifiche: pass.Notifiche
                    },
                    pass.DataInizio = new Date(pass.DataInizio);
                    pass.DataFine = new Date(pass.DataFine);

                    delete pass.DesPassaggio;
                    delete pass.VisImpianti;
                    delete pass.OpPermesse;
                    delete pass.LogOperazioni;
                    delete pass.Notifiche;
                });

                $(cFasiSportello).val(JSON.stringify(objSportello.passaggi));

                let gridPassaggi = $("#GridFasiSportello").data("kendoGrid");
                gridPassaggi.dataSource.read();

                let dlg = $("#EditSportello").data("kendoDialog");
                dlg.title((id_sportello === 0 ? "Nuovo sportello" : "Modifica sportello"));
                dlg.open();

            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}


function salvaSportello() {

    let id_sportello = $(cIdSportello).val();

    let title = "Modifica sportello";
    if (id_sportello == 0) {
        title = "Nuovo sportello";
    }

    let grid = $("#GridFasiSportello").data("kendoGrid");
    let rowInEditMode = $("#GridFasiSportello").find("tr.k-grid-edit-row");
    if (rowInEditMode.length > 0) {

        //var itemBeingEdited = $("#gridSupplierPaymentDue").data("kendoGrid").dataSource.getByUid(rowInEditMode.data("uid"));

        msgdlg(title, "Confermare o annullare le modifiche per la fase sportello.");
        return false; 
    }

    let descrizione = $("#inDescrizione").val();
    if (descrizione === "") {
        msgdlg(title, "Descrizione vuota...", function () {
            $("#inDescrizione").focus();
        });        
        return false; 
    }

    let dataInizio = $("#inValiditaInizio").data("kendoDatePicker").value();
    if (dataInizio === null) {
        msgdlg(title, "Validità inizio vuota...", function () {
            $("#inValiditaInizio").focus();
        });
        return false; 
    }

    let dataFine = $("#inValiditaFine").data("kendoDatePicker").value();
    if (dataFine === null) {
        msgdlg(title, "Validità fine vuota...", function () {
            $("#inValiditaFine").focus();
        });
        return false;
    }

    if (dataFine <= dataInizio) {
        msgdlg(title, "Validità fine non maggiore di validità inizio...", function () {
            $("#inValiditaFine").focus();
        });
        return false;
    }

    let specie = $("#inSpecie").data("kendoMultiSelect").value();

    if (specie.length === 0) {
        msgdlg(title, "Associare almeno una specie allo sportello...");
        return false;
    }

    let currentData = grid.dataSource.data();

    if (currentData.length === 0) {
        msgdlg(title, "Impostare le fasi dello sportello...");
        return false;
    }

    let passaggi = [];
    $.each(currentData, function (i, d) {
        let idPassaggio = d.Passaggio.IdPassaggio;
        passaggi.push({
            IdPassaggio: idPassaggio,
            DataInizio: d.DataInizio,
            DataFine: d.DataFine
        });
    });

    let param = {
        id_sportello: id_sportello,
        descrizione: descrizione,
        dataInizio: dataInizio,
        dataFine: dataFine,
        specie: specie.join("|"),
        passaggi: JSON.stringify(passaggi)
    };

    let result = true;
    $.ajax({
        type: "POST",
        url: ajax_url("SalvaSportello"),
        data: JSON.stringify(param),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                let ws_result = JSON.parse(msg.d);

                if (!ws_result.result) {

                    msgdlg(title, ws_result.error);
                    result = false;

                } else {

                    id_sportello = ws_result.id;

                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

    if (result) {

        let sportelli = $("#GridSportelli").data("kendoGrid");
        sportelli.dataSource.read();

        let selected = false;
        let idx = 0;
        let items = sportelli.items();
        while (!selected && idx < items.length) {
            let dataitem = sportelli.dataItem(items[idx]);
            if (dataitem.id_sportello === id_sportello) {
                sportelli.select(items[idx]);
                selected = true;
            }
            idx++;
        }
    }

    return result;
}


function eliminaSportello(item) {
    $.ajax({
        type: "POST",
        url: ajax_url("EliminaSportello"),
        data: "{id_sportello: " + item.id_sportello + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                let ws_result = JSON.parse(msg.d);

                if (!ws_result.result) {

                    msgdlg("Eliminazione sportello", ws_result.msg);

                } else {

                    let grid = $("#GridSportelli").getKendoGrid();
                    grid.dataSource.remove(item);
                }

            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}


function leggiPassaggi() {

    let data = [];
    $.ajax({
        type: "POST",
        url: ajax_url("LeggiPassaggi"),
        data: "{ }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            if (msg.d !== "") {

                data = JSON.parse(msg.d);

            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

    return data;
}

function submitPassaggio(options) {

    if (options.data.created.length === 0 && options.data.updated.length === 0 && options.data.destroyed.length === 0) {
        return;
    }
    let grid = $("#GridFasiSportello").data("kendoGrid");
    let currentData = grid.dataSource.data();
    let passaggi = [];
    let errore = 0;
    $.each(currentData, function (i, d) {
        let idPassaggio = d.Passaggio.IdPassaggio;
        passaggi.push({
            IdPassaggio: idPassaggio,
            Passaggio: d.Passaggio,
            DataInizio: new Date(d.DataInizio.setHours(0, 0, 0, 0)),
            DataFine: new Date(d.DataFine.setHours(0, 0, 0, 0))
        });
        if (errore === 0) {
            if (idPassaggio === 0) {
                errore = 4
            } else if (d.DataInizio === null) {
                errore = 2;
            } else if (d.DataFine === null) {
                errore = 3;
            } else if (d.DataFine < d.DataInizio) {
                errore = 1;
            }
        }
    });

    if (errore !== 0) {
        let msg = "<b>Data inizio</b> maggiore di <b>Data fine</b> non permessa.";
        if (errore === 2) {
            msg = "<b>Data inizio</b> vuota non permessa.";
        } else if (errore === 3) {
            msg = "<b>Data fine</b> vuota non permessa.";
        } else if (errore === 4) {
            msg = "Selezionare una fase.";
        }
        msgdlg("Modifica fase sportello", msg);
        return;
    }

    // riordino i passaggi in base alla DataInizio...
    passaggi.sort(function (p0, p1) {
        let d0 = p0.DataInizio;
        let d1 = p1.DataInizio;
        if (d0 < d1) {
            return -1;
        }
        if (d0 > d1) {
            return 1;
        }
        return 0;
    });

    if (passaggi.length > 1) {

        let currDataFine = passaggi[0].DataFine
        let validDates = true;
        let ip = 1;
        while (validDates && ip < passaggi.length) {
            validDates = (passaggi[ip].DataInizio > currDataFine);
            currDataFine = passaggi[ip].DataFine;
            ip++;
        }

        if (!validDates) {
            msgdlg("Modifica fase sportello", "Sequenza temporale fasi non corretta.");
            return;
        }
    }

    $(cFasiSportello).val(JSON.stringify(passaggi));

    grid.dataSource.read();

}

function confdlg(title, question, callback, paramCallback, chkConferma)
{

    let content = "<div style='padding: 0px 25px;'>";
    content += question;
    if (chkConferma === true) {
        content += "<div style='margin-top:25px;'>";
        content += "<input type='checkbox' id='chk-conferma' class='k-checkbox'>";
        content += "<label class='k-checkbox-label' for='chk-conferma'>Selezione per conferma</label>";
        content += "</div>";
    }
    content += "</div>";

    let dialog = $("<div></div>").kendoDialog({
        title: title,
        closable: false,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: 'Conferma',
                action: function (e) {

                    if ($("#chk-conferma").length > 0) {
                        if (!$("#chk-conferma").prop("checked")) {
                            return false;
                        }
                    }

                    if (typeof callback === "function") {
                        return callback(paramCallback);
                    }
                    return true;
                }
            },
            { text: 'Annulla' }
        ],
        close: function (e) {
            this.destroy();
        }
    });
    dialog.data("kendoDialog").open();

}


function msgdlg(title, msg, callback) {
    let dialog = $("<div></div>").kendoDialog({
        title: title,
        closable: false,
        modal: true,
        visible: false,
        content: "<div style='padding: 0px 25px;'>" + msg + "</div>",
        actions: [
            {
                text: "OK",
                primary: false,
                action: function (e) {
                    if (typeof callback === "function") {
                        callback();
                    }
                }
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });
    dialog.data("kendoDialog").open();
}

