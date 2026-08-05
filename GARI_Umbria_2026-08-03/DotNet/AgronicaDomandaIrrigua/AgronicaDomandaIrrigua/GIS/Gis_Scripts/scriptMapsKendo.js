

/* scriptMapsKendo.js */


/**
 * Genera nuovo controllo kendo windows
 * @param {any} selector
 * @param {any} titolo
 * @param {any} widthPercent
 * @param {any} heigthPercent
 * @param {any} functionPin
 * @param {any} functionClose
 * @param {any} functionOpen
 * @param {any} functionResize
 * @param {any} actions
 */
function GeneraKendoWindowMaps(selector, titolo, widthPercent, heigthPercent, functionPin, functionClose, functionOpen, functionResize, actions) {

    let acts;
    if (!actions) {
        acts = [
            "Pin",
            "Minimize",
            "Maximize",
            "Close"
        ];
    } else {
        acts = actions;
    }

    $(selector).kendoWindow({
        width: widthPercent,
        height: heigthPercent,
        title: titolo,
        visible: false,
        close: functionClose,
        open: functionOpen,
        resize: functionResize,
        pin: functionPin,
        actions: acts
    }).data("kendoWindow");

}



/**
 * Genera un nuovo controllo kendo Dialog
 * @param {string} selector Selettore JQuery per il div da trasformare
 * @param {any} titolo Titolo della dialog
 * @param {any} conferma funzione di conferma
 * @param {any} closeEventFunction
 * @param {any} openEventFunction
 * @param {any} dialogWidth
 * @param {any} testoConferma
 */
function GeneraKendoDialogMaps(selector, titolo, conferma, closeEventFunction, openEventFunction, dialogWidth, testoConferma, actionsArray) {

    if (dialogWidth !== undefined) {
        dialogWidth = "400px";
    }

    var testoPulsanteConferma = "Conferma";
    if (testoConferma !== undefined) {
        testoPulsanteConferma = testoConferma;
    }

    var actions = undefined;
    var closable = true;
    if (testoConferma !== "nessuno") {
        actions = [
            { text: testoPulsanteConferma, action: conferma }
        ];
    } else {
        if (actionsArray) {
            actions = actionsArray;
            closable = false;
        }
    }




    $(selector).kendoDialog({
        width: dialogWidth,
        title: titolo,
        closable: closable,
        modal: true,
        visible: false,
        open: openEventFunction,
        close: closeEventFunction,
        actions: actions
    });

}


function mostraNascondiPanelBar(pannello, mostraNascondi) {

    if (mostraNascondi) {
        $(pannello).show();
    } else {
        $(pannello).hide();
    }

}


function KendoDialogStatoAperto(selector) {
    let kendoDlg = $(selector).data("kendoDialog");
    if (kendoDlg === undefined) {
        return false;
    }
    return !kendoDlg.element.is(":hidden");
}

function SliderKendo() {

    $(".aslider").hide();

    $('.kendoSlider').each(function () {

        var jSel = this.id.toString().replace("Kendo", "");


        var cSlider = $(this).kendoSlider({
            increaseButtonTitle: "+",
            decreaseButtonTitle: "-",
            min: 0,
            max: 100,
            smallStep: 10,
            largeStep: 10,
            tickPlacement: "none",
            showButtons: false,
            slide: function (e) {
                $("#" + jSel).val(parseFloat(e.value) / 100);
            }
        }).data("kendoSlider");

        var selettoreSlide = "#" + jSel;
        var valoreSlideIniziale = parseFloat($(selettoreSlide).val().toString().replace(",", ".") * 100);

        cSlider.value(valoreSlideIniziale);

    });

}

function ColorPickerKendo() {

    $(".tavolozza").each(function () {

        $(this).kendoColorPicker({
            value: $(this).val(),
            buttons: true,
            messages: {
                apply: "Ok",
                cancel: "Annulla"
            }
        });

    });

}


/**
 * Apre il Kendo Dialog indicato dal selettore JQUERY (includere il #)
 * @param {any} selector selettore jQuery
 */
function ApriKendoDialog(selector) {
    $(selector).data("kendoDialog").open();
}

function ChiudiKendoDialog(selector) {
    $(selector).data("kendoDialog").close();
}

function KendoDialogVerificaSeAperto(selector) {
    $(selector).data("kendoDialog").close();

}

function ApriWindow(selector, center) {
    if (center) {
        $(selector).data("kendoWindow").center().open();
    } else {
        $(selector).data("kendoWindow").open();
    }

}

function DropDownValoreSelezione(selector) {

    return $(selector).data("kendoDropDownList").value();

}

function kendoDlgMessage(titolo, contenuto, callback, callbackAnnulla) {

    //Gabriele 2019 09 20
    //se ho già un elemento con id...
    let cnt = 0;
    let id = "id_tmp_kendo_msg_" + cnt;
    let next_id = (document.getElementById(id) !== null);
    while (next_id && cnt < 10) {
        cnt++;
        id = "id_tmp_kendo_msg_" + cnt;
        next_id = (document.getElementById(id) !== null);
    }

    let elem = document.createElement("div");
    elem.id = id;
    document.body.appendChild(elem);
    let $elem = $("#" + elem.id);

    let w = $(window).width();

    let txt = "OK";

    if (callbackAnnulla) {
        txt = "Sì";
    }

    let act = [
        {
            text: txt,
            primary: false,
            action: function () {
                if (typeof callback === "function") {
                    callback();
                }
            }
        }
    ]

    if (callbackAnnulla) {
        act.push(
            {
                text: "No",
                primary: false,
                action: function () {
                    callbackAnnulla();
                }
            }
        );
    }

    $elem.kendoDialog({
        title: titolo,
        closable: false,
        modal: true,
        visible: false,
        minWidth: w * 0.25,
        maxWidth: w * 0.9,
        content: "<div style='text-align:center;'>" + contenuto + "</div>",
        //buttonLayout: "normal",
        actions: act,
        open: function () {
            //GABRIELE
            //Todo: Aggiungere icona nel titolo dell finestra...
            //let title = this.element.parent().find(".k-dialog-title");
            //if (title.length > 0) {
            //    let span = document.createElement("span");
            //    span.style.cssText = "margin-right: 15px;";
            //    //span.className = "fa fa-exclamation-circle";
            //    span.className = "fa fa-question-circle";
            //    //span.className = "fa fa-info-circle";
            //    title.prepend(span);
            //}
        },
        close: function (e) {
            this.destroy();
        }
    });

    $elem.data("kendoDialog").open();
}


/**
 * Messaggio di conferma
 * @param {any} titolo
 * @param {any} contenuto
 * @param {any} callbackOnOk
 * @param {any} callbackOnCancel
 */
function kendoDlgConfirm(titolo, contenuto, callbackOnOk, callbackOnCancel) {

    let elem = document.createElement("div");
    document.body.appendChild(elem);
    let $elem = $(elem);

    let w = $(window).width();

    $elem.kendoDialog({
        title: titolo,
        closable: false,
        modal: true,
        visible: false,
        minWidth: w * 0.25,
        maxWidth: w * 0.9,
        content: "<div style='text-align:center;'>" + contenuto + "</div>",
        //buttonLayout: "normal",
        actions: [
            {
                text: 'OK',
                action: function (e) {

                    if (typeof callbackOnOk === "function") {
                        callbackOnOk();
                    }
                    return true;
                },
                primary: false,
            },
            {
                text: 'Annulla',
                action: function (e) {

                    if (typeof callbackOnCancel === "function") {
                        callbackOnCancel();
                    }
                    return true;
                },
                primary: false,
            }
        ],
        open: function () {
            let title = this.element.parent().find(".k-dialog-title");
            if (title.length > 0) {
                let span = document.createElement("span");
                span.style.cssText = "margin-right: 15px;";
                span.className = "fa fa-question-circle fa-lg";
                title.prepend(span);
            }
        },
        close: function (e) {
            this.destroy();
        }
    });

    $elem.data("kendoDialog").open();
}