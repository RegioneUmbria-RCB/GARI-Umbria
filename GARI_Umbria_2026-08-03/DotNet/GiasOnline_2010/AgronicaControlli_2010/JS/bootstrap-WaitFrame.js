
var WaitFrame;
WaitFrame = WaitFrame || (function () {
    var pleaseWaitDivAttivo = false;
    var $pleaseWaitDiv = $(
        '<div id="pleaseWaitDialog" class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-to: 15%; overflow-y: visible;">' +
        '<div class="modal-dialog modal-m">' +
        '<div class="modal-header" style="background-color: #fff"><h3></h3></div>' +
        '<div class="modal-body" style="background-color: #fff">' +
        '<div class="progress progress-striped active" style="margin-bottom:0;"><div class="progress-bar" style="width: 100%;"></div></div>' +
        '</div></div></div>');
    
    var $kendoWaitFrame = $('<div id="kendoWaitFrame" style="position: absolute; width: 100vw; height:100vh; top:0px; left:0px; z-index: 10020 !important"></div>');
    var $kendoWaitFrameHead = $("<style>" +
        ".k-loading-mask .k-loading-image { background-size: 75px; background-repeat: no-repeat; background-position: center;    background-color: rgba(0,0,0,0.7); background-image: url('__img__');}" +
        ".k-loading-image::before { display: none; }" +
        "</style>");

    return {
        show: function (messaggio) {

            ////se esiste un altro dialog (capire come generalizzare la cosa) allora esco.
            //if ($('#dialog_errore').is(":visible")) {
            //    console.log("bootstrap-waitframe.js - Blocco visualizzazione")
            //    return true;
            //}


            if (typeof(kendo) !== "undefined") {

                if ($("#kendoWaitFrame").length === 0 ) {
                    $(document.body).append($kendoWaitFrame);
                    $(document.head).append($kendoWaitFrameHead);
                }

                $("#kendoWaitFrame").css("width", "100vw");
                $("#kendoWaitFrame").css("height","100vh");                
                $("#kendoWaitFrame").css("top", parseInt(window.scrollY).toString() + "px");    
                $(document.body).css("overflow", "hidden");
                kendo.ui.progress($("#kendoWaitFrame"), true);
                return true;
            }

            //imposto il messaggio da mostrare in base al parametro (default="Attendere...")
            if (typeof (messaggio) === 'undefined') {
                messaggio = "Attendere ...";
            }
            $pleaseWaitDiv.find("h3").text(messaggio);

            if ($('#pleaseWaitDialog').is(":visible")) {
                //console.log("se il wait è già visibile esco...(e non l'aggiungo di nuovo)");
                return true;
            }

            //la 2 volta che viene richiamata, mostro il wait
            if (pleaseWaitDivAttivo) {
                //console.log("la 2 volta che viene richiamata, mostro il wait");
                $('#pleaseWaitDialog').modal('show');
                return true;
            }

            console.log("Inizializzazione waitframe .. ");
            //la 1 volta che viene richiamata, aggiungo al form la finestra modale del wait
            pleaseWaitDivAttivo = true;
            $pleaseWaitDiv.modal();
        },
        hide: function () {

            if (typeof (kendo) !== "undefined") {
                $("#kendoWaitFrame").width(0);
                $("#kendoWaitFrame").height(0);
                $(document.body).css("overflow", "auto");
                kendo.ui.progress($("#kendoWaitFrame"), false);
                return true;
            }


            $pleaseWaitDiv.removeClass('fade')
            $pleaseWaitDiv.modal('hide');



            //$('body').removeClass('modal-open');
            //$('.modal-backdrop').remove();

            setTimeout(function () {
                //vanni, 22/05/2017: se per un qualche motivo la chiamata hide non è riuscita forzo la chiusura in questa maniera ... 
                if ($('#pleaseWaitDialog').is(":visible")) {
                    console.warn("pleaseWaitDialog ... nascosto da settimeout ..");
                    $('#pleaseWaitDialog').hide();
                }
            }, 1000);



        },
        debug: function () {
            return pleaseWaitDivAttivo;
        }

    };
})();



function ModificaArray(arr, aggiungi, val) {
    if (aggiungi == true)
        arr.push(val);
    else {
        //cerco 
        var index = arr.indexOf(val);
        if (index > -1) {
            arr.splice(index, 1);
        }
    }
}