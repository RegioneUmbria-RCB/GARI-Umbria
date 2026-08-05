
var indirizzohttp = "./Giacenze_Magazzino.aspx";   

//DOCUMENT READY
$(document).ready(function () {
    
    //$.logThis("DocReady: INIZIO");


    creaKendoSwitch("chkGiacenzePositive");
    setKendoSwitch("chkGiacenzePositive", true); 

    ////KENDO Tabstrip***********************************************
    $(".kendoTabStrip_GiacenzeMagazzino").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
    ////*************************************************************
    var tabstrip_GiacenzeMagazzino = $("#tabstrip_GiacenzeMagazzino").data("kendoTabStrip");
    tabstrip_GiacenzeMagazzino.select(0);

    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    //UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";
    //UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });
    set_data("txt_DataRif_GiacenzeMagazzino", formattedDate(new Date(), '/'), null);

    $(".areaGiacenzeMagazzino").show();
       
    //eventi di click pulsanti
    $("#btn_ricerca_GiacenzeMagazzino").click(function () {

        let dataDaControllare = $('input[name$="txt_DataRif_GiacenzeMagazzino"]').val();
        let dataValida = true;
        if (dataDaControllare !== "")
            dataValida = isValidDate(dataDaControllare);
        if (!dataValida) {
            MessaggioErrore_Bootstrap("Data movimento non valida", "DIV_Messaggi");
        } else {
            RicercaGiacenzeMagazzino(indirizzohttp, false);
        }
    });
    
    creaKendoDDL_ParametriQualitativi("idCalibro", $(cIdPiva).val(), 1, "ocalibro");
    creaKendoDDL_ParametriQualitativi("idQualita", $(cIdPiva).val(), 3, "oqualità");
    creaKendoDDL_ParametriQualitativi("idCertificazione", $(cIdPiva).val(), 12, "ocertificazioni");
    //creaKendoDDL_ParametriQualitativi("idRugginosita", $(cIdPiva).val(), 22, "orugginosita");
    creaKendoDDL_ParametriQualitativi("idImballaggio", $(cIdPiva).val(), 4, "oimballaggio");
    creaKendoDDL_ParametriQualitativi("idContenitore", $(cIdPiva).val(), 8, "ocontenitore");
    creaKendoDDL_ParametriQualitativi("idConfezione", $(cIdPiva).val(), 5, "oconfezione");
    $("#idCentro").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "sa_nome",
        dataValueField: "sa_cod",
        dataSource: { transport: { read: LeggiCentri } },
        open: kendoDropDownAdjustWidth,
        optionLabel:
        {
            sa_nome: "Tutti",
            sa_cod: 0
        }
    }).data("kendoDropDownList");

    var ds = new kendo.data.DataSource({ transport: { read: RiempiSpecie } });
    $('input[name$="ddlSpecie"]').kendoDropDownList({
        filter: "contains",
        dataSource: ds,
        dataTextField: "Veg_Des",
        dataValueField: "Veg_Cod"
    }).bind("change", ddlSpecie_change);


    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Veg_Cod: -1 } }, "Cul_Des", "Cul_Cod");

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });
    
    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    $('#a_tabGiacenzeMagazzino').click(function (e) { return tabClick(e, this); });
    
    $.logThis("DocReady: FINE");

});