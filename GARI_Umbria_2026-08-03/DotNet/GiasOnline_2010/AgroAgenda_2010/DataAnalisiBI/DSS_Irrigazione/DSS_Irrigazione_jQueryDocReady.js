

var DSS_Worker;
/*
 
    //let useWorker = false;
    //if (useWorker & typeof window.Worker === "function") {

    //    DSS_Worker = new Worker("DSS_Irrigazione_Worker.js");
    //    DSS_Worker.addEventListener("message", function (e) {

    //        let indic = e.data;

    //        console.log(indic);
    //    });

    //    $(window).bind('beforeunload', function () {

    //        DSS_Worker.terminate();
    //    });

    //    DSS_Worker.postMessage({
    //        baseurl: window.location.href.split("?")[0],
    //        indicatori: indicatori
    //    });


    //} else {

    //    eseguiNoWorker(div, indicatori);

    //}

 */

$(document).ready(function () {

    window_resize();

    let descrObj = DescrizioneServizio();

    if (descrObj === null) {

        $("#DSS_Irrigazione_Anbi").addClass("anbi-hidden");

    } else {

        if (descrObj.BannerHidden) {

            $("#DSS_Irrigazione_Anbi").addClass("anbi-hidden");

        } else {

            $("#__anbi__").find(".anbi-full").html(descrObj.MsgFull);
            $("#__anbi__").find(".anbi-short").html(descrObj.MsgShort);

            $("#__anbi__").on("click", function (e) {

                $(e.currentTarget).toggleClass("anbi-collapsed");
            });

            document.getElementById("DSS_Irrigazione_Anbi").style.opacity = 1;
        }
    }

    if (typeof window.Worker === "function") {

        // Uso il worker

        let divLoader = document.createElement("div");
        divLoader.style.cssText = "position:absolute; top:50%; left:50%; transform:translate(-50%, -50%);";
        document.getElementById("DSS_Irrigazione_Main").appendChild(divLoader);

        $(divLoader).StyleLoader();

        DSS_Worker = new Worker("DSS_Irrigazione_Worker.js");

        $(window).bind('beforeunload', function () {

            DSS_Worker.terminate();
        });

        DSS_Worker.addEventListener("message", function (e) {

            divLoader.remove();

            let indicatori = e.data;
            ElaboraIndicatori(indicatori)
        });

        DSS_Worker.postMessage({
            baseurl: window.location.href.split("?")[0]
        });

    } else {

        setTimeout(function () {
            let indicatori = CaricaIndicatori();
            ElaboraIndicatori(indicatori);
        }, 250);
    }
});

function ElaboraIndicatori(Centri) {

    MostraIndicatori_V2("DSS_Irrigazione_Indicatori", Centri);

    let vegArray = [];

    let seen_obj = {};
    $.each(Centri, function (idx, centro) {

        $.each(centro.Indicatori, function (idx, indic) {

            if (!seen_obj.hasOwnProperty("S_" + indic.Veg_Cod)) {
                seen_obj["S_" + indic.Veg_Cod] = 1;
                vegArray.push({ veg_cod: indic.Veg_Cod, veg_des: indic.Coltura });
            }
        });

        $.each(centro.Campi, function (idx, campo) {

            $.each(campo.Indicatori, function (idx, indic) {

                if (!seen_obj.hasOwnProperty("S_" + indic.Veg_Cod)) {
                    seen_obj["S_" + indic.Veg_Cod] = 1;
                    vegArray.push({ veg_cod: indic.Veg_Cod, veg_des: indic.Coltura });
                }
            });
        });
    });

    vegArray.sort((a, b) => {
        const desA = a.veg_des.toUpperCase(); 
        const desB = b.veg_des.toUpperCase(); 
        if (desA < desB) {
            return -1;
        }
        if (desA > desB) {
            return 1;
        }
        return 0;
    });

    $("#cmbSpecieVegetale").kendoMultiSelect({
        autoBind: true,
        autoWidth: true,
        dataTextField: "veg_des",
        dataValueField: "veg_cod",
        autoClose: true,
        placeholder: TraduzioneMultiResx(datiMeteoResx, "SpecieVegetale", "Filtro per specie vegetale"),
        dataSource: vegArray,
        change: function (e) {
            FiltraIndicatori(this.value());
        }
    });

    $("#filtroSpecie").removeClass("transparent");

    $(window).resize(window_resize);
}

function window_resize() {

    if (document.getElementById("IntestazioneMenuBS2017") === null) {
        $("#id_MainContainer").css("position","");
    }
    else {
        let top = $("#IntestazioneMenuBS2017").position().top + $("#IntestazioneMenuBS2017").outerHeight();
        let win_h = $(window).height();
        let ftr_h = 0;

        if (document.getElementsByClassName("AgronicaFooter").length > 0) {
           ftr_h = $(".AgronicaFooter").outerHeight() + 2;
        }
        let h = win_h - top - ftr_h;

        $("#id_MainContainer").css({
            "top": top + "px",
            "height": h + "px"
        });
    }

}

