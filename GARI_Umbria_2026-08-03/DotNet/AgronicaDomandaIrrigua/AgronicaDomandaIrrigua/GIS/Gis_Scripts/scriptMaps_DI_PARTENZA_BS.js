


/********************************************************************/
/********************** INIT BOOTSRAP *******************************/
/********************************************************************/


/* inizializzazione della mappa */
function initialize() {
    
    
    mappa.isKws = isKws();
    mappa.inizializza();
    
    mappa.initNewMap();

    AgganciaEventiPulsanti();

    AgganciaEventiControlli();

    AgganciaEventiGoogle();

    makeToolBar();

}

/**
 * Apre una kendo window con all'interno la pagina iframe richiesta dal parametro indirizzo
 * @param {any} indirizzo
 * @param {any} descrizione
 * @param {any} functionOnClose
 * @param {any} width
 * @param {any} height
 * @param {any} left
 * @param {any} top
 */
function KendoWindowGenericApri(indirizzo, descrizione, functionOnClose, width, height, left, top) {

    
    $("#kendoWindowPaginaGeneric").attr("src", "about:blank");
    $("#kendoWindowPaginaGeneric").attr("src", indirizzo);

    $("#kendoWindowiFrameGeneric").data("kendoWindow").title(descrizione);

    if (width !== undefined) {
        $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
            width: width        
        });
    }


    if (height !== undefined) {
        $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
            height: height
        });
    }

    $("#kendoWindowiFrameGeneric").data("kendoWindow").setOptions({
        position: {
            top: top, // or "100px"
            left: left
        }
    });
    
    
    ApriWindow("#kendoWindowiFrameGeneric");
    

}

/**
 * Apre una pagina modale in stile bootstrap, in un iframe
 * @param {string} indirizzo indirizzo per iframe
 * @param {string} descrizione descrizione della pagina modale
 * @param {function} functionOnClose funzione da richiamare in chiusura. 
 * @param {string} left se si passa la stringa "calcola" questo viene calcolato
 */
function ModalBootstrapApri(indirizzo, descrizione, functionOnClose, ridimensionabile, width, height, left, top) {

    utility.log("redir BS to: " + indirizzo);

    $("#PaginaGeneric").attr("src", "about:blank");
    $("#PaginaGeneric").attr("src", indirizzo);
    $("#iFrameGeneric h4 span").html(descrizione);
    $("#iFrameGeneric").modal('toggle');         

    if (ridimensionabile) {

    }

    if (width !== undefined) {
        $("#iFrameGeneric").css("width", width);
    }


    if (height !== undefined) {

    }


    if (left !== undefined) {
        if (left === "calcola") {

        } else {
            $("#iFrameGeneric").css("left", left);
        }
    }


    if (top !== undefined) {

    }

    if (functionOnClose !== undefined) {
        $('#iFrameGeneric').on('hidden.bs.modal', functionOnClose);
    }

}

/**
 * Chiude una pagina modale bootstrap
 */
function ModalBootstrapChiudi() {
    $("#PaginaGeneric").attr("src", "about:blank");
    $("#iFrameGeneric").modal('hide'); 
}



/***************************************************************************************************
 *
 **************************************************************************************************/

function ModalKendoApri(indirizzo, descrizione, functionOnClose, ridimensionabile, width, height, left, top) {

    $('<div id="idKendoModal_iFrameGeneric" style="display: none;"></div>').appendTo('body');
    let $kendomodal = $("#idKendoModal_iFrameGeneric")

    let locWidth = "90%";
    let locHeight = "90%";

    if (width !== undefined) {
        locWidth = width;
    }

    if (height !== undefined) {
        locHeight = height;
    }

    $kendomodal.kendoWindow({
        //actions: [],
        title: descrizione,
        height: locHeight,
        width: locWidth,
        draggable: false,
        visible: false,
        modal: true,
        resizable: false,
        content: indirizzo,
        iframe: true,
        open: function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
            $("body").addClass("ob-no-scroll");
        },
        close: function (e) {

            $("body").removeClass("ob-no-scroll");

            $kendomodal.data("kendoWindow").destroy();

            if (functionOnClose !== undefined) {
                functionOnClose();
            }

        }
/*
        activate: function(e) {
            var h = $("#idKendoModal_iFrameGeneric").height();
            var footH = $("#idKendoModal_iFrameGeneric .window-footer").outerHeight(true);
            var contH = h - footH;
            $("#idKendoModal_iFrameGeneric .container").height(contH).css("overflow", "auto");
        }
*/
    });

    let parent = $kendomodal.parent();
    parent.find('.k-window-title').css('text-align', 'center');
    parent.css('padding-top', '48px');
    let titlebar = parent.find('.k-window-titlebar');
    titlebar.css({
        "margin-top": "-48px",
        "height": "35px",
        "line-height": "35px",
        "vertical-align": "middle"
    });

    $kendomodal.data("kendoWindow").center().open();
}


function ModalKendoChiudi() {
    let $kendomodal = $("#idKendoModal_iFrameGeneric");
    let kendoWindow = $kendomodal.data("kendoWindow");
    if (kendoWindow != undefined) {
        kendoWindow.close();
    }        
}
