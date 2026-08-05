var lat;
var lng;

var valueTemplate;
var listTemplate;
var valueTemplate_disp;
var listTemplate_disp;
var enable_calcolo = true;

var dataFiltroValidita = new Date();

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");

    VisualizzaFinestra(false);

    $(".kendoCalendar").kendoDatePicker({
        footer: false //"#: kendo.toString(data, 'd') #"
    });

    let today = new Date();
    today.setHours(0, 0, 0, 0);
    //Primo giorno del mese corrente
    let month1 = new Date(today.valueOf());
    month1.setDate(1);

    let diffDays = Math.round((today.getTime() - month1.getTime()) / (1000 * 3600 * 24));
    if (diffDays < 3) {
        //Primo giorno del mese precedente
        month1.setMonth(month1.getMonth() - 1);
    }

    lat = Request_QueryString("lat");
    lng = Request_QueryString("lng");

    $("#geo-pos-edit").geoPosEdit({ change: function (pos) { } });
    $("#geo-pos-edit-meteo").geoPosEdit({
        change: function (pos) {
            if (KendoDDL("ddlTipoSorgente").value() === "4") {
                KendoDDL("ddlTipoSorgente").trigger('change');
            }
        }
    });


    $("#txt_DataDa").data("kendoDatePicker").value(month1);
    $("#txt_DataA").data("kendoDatePicker").value(today);

    let versioneKendo = kendo.version.split('.')[0]
    let tmplt_r1 = get_tmplt_r1(versioneKendo)
    let tmplt_r1_disp = get_tmplt_r1_disp(versioneKendo)
    let tmplt_r2 = get_tmplt_r2(versioneKendo)

    valueTemplate_disp = get_valueTemplate(versioneKendo, tmplt_r1_disp, tmplt_r2)
    listTemplate_disp = get_listTemplate(versioneKendo, tmplt_r1_disp, tmplt_r2)

    valueTemplate = get_valueTemplate(versioneKendo, tmplt_r1, tmplt_r2)
    listTemplate = get_listTemplate(versioneKendo, tmplt_r1, tmplt_r2)

    
    creaKendoDropDownList("ddlElencoGruppiConsegna", { read: RicercaElencoGruppiConsegna }, "nome_dispositivo", "id_dispositivo", null, null, null, null, valueTemplate_disp, listTemplate_disp);
    KendoDDL("ddlElencoGruppiConsegna").bind('change', onChange_GruppoConsegna);
    creaKendoDropDownList("ddlYear", { read: Leggi_Anni }, "year_cod", "year_cod");

    if (lat !== null && lng !== null) {
        $("#geo-pos-edit").data("geoPosEdit").setLatLngDec(lat, lng);
    }

    KendoDDL("ddlYear").value(dataFiltroValidita.getFullYear());

    var onActivate = function (e) {
        // access the selected item via e.item (Element)
        if (e.item.id === "Dati_PrelieviOsservatiEffettivi") {
            
        }

        // detach select event handler via unbind()
        tabStrip.unbind("activate", onActivate);
    };

    var tabStrip = $("#tabstrip").kendoTabStrip({
        animation: {
            open: {
                effects: "fadeIn"
            }
        },
        activate: onActivate
    });

    creaKendoSwitch("chkUseLatLngContatore", undefined, undefined, false, onChange_chkUseLatLngContatore);
    creaKendoSwitch("chkDatiStorici", undefined, undefined, false);
    //ShowMeteoSelector(false);
    praprazioneSelezioneMeteo();
    ShowTab(1, false);
    ShowTab(2, false);
    //ShowTab(3, false);
    ResetImage();

    $("#btn_ricerca").click(function () {
        if (UtenteAbilitatoLettura) {
            ElaboraDati();
            if ($('#' + hdKendoModelClientID).val() !== "") {
                //LeggiUltimiDati();
                popolaGriglieDatiAnagrafici("divDatiAnagrafe");
                popolaGrigliaRisultati("divKendoContatoriOut");
                ShowTab(1, true);
            }
        }
    });

    $("#btn_calcola").click(function () {
        if (enable_calcolo === true) {           
            if (KendoDDL("ddlTipoSorgente").value() === "4") {
                var c = $("#geo-pos-edit").data("geoPosEdit").getLatLngDec();
                if (c !== undefined && c !== null) {
                    $("#geo-pos-edit-meteo").data("geoPosEdit").setLatLngDec(c.lat, c.lng);
                    KendoDDL("ddlTipoSorgente").trigger('change');
                }
            }
            AggiornaCalcolo();
        } else {
            kendo.alert("Sono stati letti i dati dei pressostati. Il calcolo del confronto prelievi attesi-reali è disponibile solo per i contatori acqua");
        }
        
    });

    //if (lat === null && lng === null) {
    //    
    //}

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    VisualizzaFinestra(true);

    $.logThis("DocReady: FINE");
});
