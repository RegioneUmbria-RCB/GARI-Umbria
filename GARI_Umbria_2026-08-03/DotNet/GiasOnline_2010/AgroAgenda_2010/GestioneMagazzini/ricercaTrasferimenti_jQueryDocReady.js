var ricercaTrasferimentiResx = [];
var arrPathResx = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    //File non creato perché ad ora non necessario "GestioneTrasferimenti/App_LocalResources/RicercaTrasferimenti.aspx.resx"
];

//DOCUMENT READY
$(document).ready(function () {

    // Evita l'utilizzo dell'invio
    // TODO Stefano
    //$(window).keydown(function (event) {
    //    if (event.keyCode == 13) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

//    $('#aspnetForm').change(function () {
//        controlla_form();
//    });
    
    $.logThis("DocReady: INIZIO");

    if (Array.isArray(arrPathResx) && arrPathResx.length > 0) {
        arrPathResx.forEach(function (singlePathResx) {
            ricercaTrasferimentiResx.unshift(readResxFile(singlePathResx, "RicercaTrasferimenti"));
        });
    }

    //$("#tab2").hide();
    $("#tabstrip_Filtri").kendoTabStrip({
        animation: false
    }).data("kendoTabStrip");

    $(".panel-body").show();

    $("#TipoOutput").kendoButtonGroup({
        index: 0,
        selection: "single",
        select: CambiaTipoOutput
    });

    Elenco_Specie = Leggi_Specie();
    creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie, data: { Veg_Cod: -1 } }, "veg_des", "veg_cod", null, null, null, SpecieChange);
    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Cul_Cod: -1 } }, "Cul_Des", "Cul_Cod", null, null, null, null);

    creaKendoMultiselect("multiselCategorie", { read: RiempiCategorie }, "Elem_Des", "Elem_Cod");
    Set_MultiselValue("multiselCategorie", "210|310");
    $("#id_multiselCategorie").data("kendoMultiSelect").enable(false);

    creaKendoMultiselectServerFiltering("multiselProdotti", { read: RicercaProdottiCompleto }, 3, "Prodotto_Des", "Prodotto_Cod");
    KendoMultisel("multiselProdotti").bind("filtering", multiselProdotti_filtering);

    creaKendoSwitch();
    // $(".kendoSwitch").kendoMobileSwitch({ onLabel: "SI", offLabel: "NO" });
    
    //Controllo se ha i permessi di lettura e scrittura sulla pagina
    //UtenteAbilitatoLettura = $("input[name$='hf_UtenteAbilitatoLettura']").val() === "True";
    //UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    $(".searchArea").show();
    $(".elencoTrasferimentiArea").hide();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    var today = new Date();
    var month = today.getMonth();
    var year = today.getFullYear();

    set_data("txt_DataDal", formattedDate(new Date(year, month, 1), '/'), null);
    set_data("txt_DataAl", formattedDate(today, '/'), null);

    //eventi di click pulsanti
    $("#btn_ricerca").click(function () {
        dataDaControllare = $('input[name$="txt_DataDal"]').val();
        dataValida = true;
        if (dataDaControllare !== "") 
            dataValida = isValidDate(dataDaControllare);  
        if (!dataValida)
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "DataDalNonValida", "Data dal non valida"), "DIV_Messaggi");
        else {
                dataDaControllare = $('input[name$="txt_DataAl"]').val();
                dataValida = true;
                if (dataDaControllare !== "")
                    dataValida = isValidDate(dataDaControllare);
                if (!dataValida)
                    MessaggioErrore_Bootstrap(TraduzioneMultiResx(ricercaTrasferimentiResx, "DataAlNonValida", "Data al non valida"), "DIV_Messaggi");
                else {
                    //RicercaTrasferimenti();
                    //$(".elencoTrasferimentiArea").show();

                    switch (tipoRicerca) {
                        case 'T': RicercaTrasferimenti(); break;
                        case 'D': RicercaTrasferimentiDettagli(); break;
                    }
                }
        }
    });



    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $("#azioni_LavorazioniFF").hide();

    $.logThis("DocReady: FINE");

});