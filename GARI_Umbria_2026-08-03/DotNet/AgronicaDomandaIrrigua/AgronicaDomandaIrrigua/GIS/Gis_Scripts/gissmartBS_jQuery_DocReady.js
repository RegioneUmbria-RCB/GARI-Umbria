

var ddl_AziendaDelay = 1200;


//DOCUMENT READY
jQuery(function () {


    kendoGisSmartBsDialog("#" + GisSmartBsClientID);
    kendoGisSmartBsDialogAnteprima("#" + gissmartBsAnteprimaSuMappaClientID);
        

    if (MapsSeparator == "_") {
        $("#btn_ApriGoogleMaps").text("App Bing Maps (su SmartPhone)");
    }

    //controllo se ci sono già delle coordinate
    arrayPoligoni = $.cookie('arrayPoligoni');
    arrayDescrizione = $.cookie('arrayDescrizione');
    numeroPoligoni = $.cookie('numeroPoligoni');
    var sCookiePoligonoCacheEntitaCod = $.cookie('poligonoCacheEntitaCod');
    if (sCookiePoligonoCacheEntitaCod != "" && sCookiePoligonoCacheEntitaCod != undefined) {
        poligonoCacheEntitaCod = JSON.parse(sCookiePoligonoCacheEntitaCod);
    } else {
        poligonoCacheEntitaCod = new Array();
    }
    //fine controllo se ci sono già delle coordinate


    $(".gias_header_azienda").hide();
    $(".gias_header_utente").hide();

    $("#rowGGAnnullaDisegno").hide();

    if (numeroPoligoni == undefined) numeroPoligoni = 0;
    if (numeroPoligoni == null) numeroPoligoni = 0;
    if (numeroPoligoni == "") numeroPoligoni = 0;

    if (arrayDescrizione == undefined) arrayDescrizione = "";
    if (arrayDescrizione == null) arrayDescrizione = "";


    if (arrayPoligoni == undefined) arrayPoligoni = "";
    if (arrayPoligoni == null) arrayPoligoni = "";


    $("#MenuDevMode").hide();
    $("#MenuDevMode2").hide();

    

    //inizializzo e mi metto on line.
    testOnOffLine();
    inizializzaTutto();

    $(document).on("click", ".k-window-titlebar", function () {
        AttivaDisattivaMenuDevMode();
    });

    $(document).on("click", ".StatoGPS", function () {
        requestPosition();
    });


    $(document).on("click", ".CancellaUltimoPunto", function () {
        CancellaUltimoPunto(this);
    });

    $(document).on("click", ".Cancellapoligono", function () {
        Cancellapoligono(this);
    });


    $(".inCorso").hide();


   

    //$("#ddl_Azienda").parent().find("input").keyup(function() {
    //    delay_KeyUp(function () {
    //        var testo = $("#ddl_Azienda").parent().find("input").val();
    //        CaricaAziendaBS(testo);
    //    }, 2000);
    //});



});

/**
 * Inizializza il modulo Gis
 * @param {gisSmartBsCfg} cfg configurazione da istanziare
 */
function gisSmartInizializzazione(cfg) {

    //decidi cosa salvare, da configurazione
    gisSmartBsCfg = cfg;

    var PoligonoCorrente = parseInt(ddl_Recupera.value());
    PoligonoCorrente = PoligonoCorrente - 1;

    PulsantiGestione(PoligonoCorrente);
}

/**
 * Avvia il gos
 */
function gisSmartAvviaGPS() {
    requestPosition();
}

function gisSmartGestioneCombo() {

    gisSmartGestioneComboKendo();


    //$(document).on("change", "#ddl_Azienda", function () {
    //    CaricaCentroAziendaleBS();
    //});

    //$(document).on("change", "#ddl_Centro", function () {
    //    CaricaSpecieDaCentroBS();
    //});

    //$(document).on("change", "#ddl_Specie", function () {        
    //    CaricaImpiantiEsistentiBS();
    //});

    //$(document).on("change", "#ddl_Impianto", function () {
    //    CambiaImpiantoBS();
    //});

    //$(document).on("change", "#ddl_recupera", function () {
    //    RecuperaPoligono();
    //});
}


function gisSmartGestioneComboKendo() {
    

    ddl_Azienda = $("#ddl_Azienda").kendoDropDownList({        
        filter: "contains",
        dataTextField: "Rag_Soc",
        dataValueField: "Piva",        
        optionLabel: lblSelezionaAziendaOScriviPerCercarla,
        delay: ddl_AziendaDelay, // wait 1 second before clearing the user input
        open: kendoDropDownFixWidth,
        dataBound: kendoDropDownFixWidth,
        filtering: function (e) {

            //get filter descriptor
            var filter = e.filter;
            CaricaAziendaBS(filter.value);

        },
        change: function(e) {

            if (ddl_Centro !== undefined) {
                CaricaCentroAziendaleBS();               
            }

        }

    }).data("kendoDropDownList");
    

    if (pivaSelezionataDaMenu != "") {
        kendoDropDown_addNew("#ddl_Azienda", pivaSelezionataDaMenu, ragSocSelezionataDaMenu, "Piva", "Rag_Soc");
        ddl_Azienda.value(pivaSelezionataDaMenu);
        CaricaCentroAziendaleBS();
    }

    ddl_Centro = $("#ddl_Centro").kendoDropDownList({        
        dataTextField: "Sa_Nome",
        dataValueField: "Piva_Sa_Cod",        
        open: kendoDropDownFixWidth,
        dataBound: kendoDropDownFixWidth,
        optionLabel: lblSelezionaCentroAziendale,
        change: function(e) {
            
            if (ddl_Impianto !== undefined) {

                CaricaImpiantiEsistentiBS();                

            }

        }

    }).data("kendoDropDownList");


    

    ddl_Impianto = $("#ddl_Impianto").kendoDropDownList({
        filter: "contains",
        dataTextField: "Reg_Descr",
        dataValueField: "Id_Reg",        
        optionLabel: lblSelezionaImpianto,
        open: kendoDropDownFixWidth,
        dataBound: kendoDropDownFixWidth,
        change: function(e) {

            CambiaImpiantoBS();

        }

    }).data("kendoDropDownList");


    ddl_Recupera = $("#ddl_recupera").kendoDropDownList({
        autoBind: true,
        filter: null,        
        dataTextField: "Recupera_Des",
        dataValueField: "Recupera_Cod",
        dataSource: undefined,
        optionLabel: lblSelezionaPunto,
        open: kendoDropDownFixWidth,
        dataBound: kendoDropDownFixWidth,
        change: function(e) {
            RecuperaPoligono();
        }

    }).data("kendoDropDownList");

}

