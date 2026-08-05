var indirizzohttp = "./DatiReteAcqua_WS.aspx";

function Leggi_Anni(options) {
    var obj =
        [
            { "year_cod": 2018 },
            { "year_cod": 2019 },
            { "year_cod": 2020 },
            { "year_cod": 2021 },
            { "year_cod": 2022 },
            { "year_cod": 2023 },
            { "year_cod": 2024 },
            { "year_cod": 2025 },
            { "year_cod": 2026 },
            { "year_cod": 2027 },
            { "year_cod": 2028 },
            { "year_cod": 2029 },
            { "year_cod": 2030 }
        ];
    options.success(obj);
}

function RicercaElencoGruppiConsegna(options) {
    var parametri;
    var url = indirizzohttp;
    if (lat === null && lng === null) {
        parametri = kendo.stringify({ "piva": "ACMO", "latcentro": 0, "lngcentro": 0});
        
    } else {
        parametri = kendo.stringify({ "piva": "ACMO", "latcentro": lat, "lngcentro": lng });
    }
    url += "/RicercaElencoGruppiConsegna";

    ajaxAgronicaSync(url,
        parametri,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function LocalizzazGruppoConsegna(options) {
    let id_disp = KendoDDL('ddlElencoGruppiConsegna').value();
    if (id_disp !== undefined && id_disp > 0) {
        var parametri = kendo.stringify({ "id_dispositivo": id_disp, "needSensors" : false });;
        var url = indirizzohttp;
        url += "/LocalizzaDispositivo";

        ajaxAgronicaSync(url,
            parametri,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                //var chk = $("#geo-pos-edit").data("geoPosEdit").getLatLngDec();
                //if (chk === undefined || chk === null) {
                lat = risp.coordinates.lat;
                lng = risp.coordinates.lng;
                $("#geo-pos-edit").data("geoPosEdit").setLatLngDec(risp.coordinates.lat, risp.coordinates.lng);
                if (getKendoSwitch('chkUseLatLngContatore') === true) {
                    $("#geo-pos-edit-meteo").data("geoPosEdit").setLatLngDec(risp.coordinates.lat, risp.coordinates.lng);
                    KendoDDL("ddlTipoSorgente").trigger('change');
                }
            }, null);
    }   
}

function ReadAnagrafe(options) {
    let id_device = KendoDDL('ddlElencoGruppiConsegna').value();

    var parametri = kendo.stringify({"id_device" : id_device});

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Anagrafe",
        parametri,
        false,
        function (risposta) {
            //let risp = JSON.parse(risposta.RispostaStringa);
            ResetImage();
            if (risposta.RispostaStringa.image !== "") {
                $("#deviceImage").html("<img id='_img' style='height: 150px;' src='data:image/png;base64," + risposta.RispostaStringa.image + "' >");
            }
            options.success(risposta.RispostaStringa.data);
        }, null);
}


function ElaboraDati() {
    let dataInizio = $("#txt_DataDa").data("kendoDatePicker").value();
    let dataFine = $("#txt_DataA").data("kendoDatePicker").value();
    let gruppoConsegna = KendoDDL('ddlElencoGruppiConsegna').value();
    let gruppoConsegnaDesc = KendoDDL('ddlElencoGruppiConsegna').dataItem().nome_dispositivo;

    let freq = "O"        //recupero i dati in formato orario per post elaborarli

    let param = {
        DataDa: kendo.toString(dataInizio, "d"), 
        DataA: kendo.toString(dataFine, "d"),
        FrequenzaDati: freq,
        TipoSorgente: 1,
        Sorgente: gruppoConsegna,
        SorgenteDesc: gruppoConsegnaDesc
    };

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Dati",
        JSON.stringify(param),
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoColumsClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_columns));
            $("#" + hdKendoRowsClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_rows));
            $("#" + hdKendoModelClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_model));
            if (risp.Chart_Type === "water") {
                createWaterChart(risp.Meteo_Charts, gruppoConsegnaDesc);
                enable_calcolo=true;
            } else {
                createPressureChart(risp.Meteo_Charts, gruppoConsegnaDesc);
                enable_calcolo = false;
            }
        },
        function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
            $("#" + hdKendoColumsClientID).val("");
            $("#" + hdKendoRowsClientID).val("");
            $("#" + hdKendoModelClientID).val("");
        });

}

function LeggiUltimiDati() {
    let currentDate = new Date();
    let gruppoConsegna = KendoDDL('ddlElencoGruppiConsegna').value();
    let gruppoConsegnaDesc = KendoDDL('ddlElencoGruppiConsegna').dataItem().nome_dispositivo;

    let freq = "O"        //recupero i dati in formato orario per post elaborarli

    let param = {
        DataDa: kendo.toString(currentDate, "d"),
        DataA: kendo.toString(currentDate, "d"),
        FrequenzaDati: freq,
        TipoSorgente: 1,
        Sorgente: gruppoConsegna,
        SorgenteDesc: gruppoConsegnaDesc
    };

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Dati",
        JSON.stringify(param),
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoColumsLastClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_columns));
            $("#" + hdKendoRowsLastClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_rows));
            $("#" + hdKendoModelLastClientID).val(JSON.stringify(JSON.parse(risp.Meteo_Table).kendo_model));
        }, function (risposta) {
            kendo.alert("Errore: " + risposta.Errore);
        });

}


function AggiornaCalcolo() {
    let yearVal = $('#ddlYear').val();

    let dataFine = new Date(yearVal,11,31);
    let dataInizio = new Date(yearVal, 0, 1);
    let gruppoConsegna = KendoDDL('ddlElencoGruppiConsegna').value();
    let id_stazione_user = 0;
    let storico = (getKendoSwitch("chkDatiStorici") == true ? true : false);
    /*if (lat === null && lng === null) {*/
    if (KendoDDL('ddlOrigineDati').dataItem() === undefined) {
        kendo.alert('Selezionare la stazione meteo prima di procedere con il calcolo');
        return;
    }
    id_stazione_user = KendoDDL('ddlOrigineDati').dataItem().id_stazione;
    //}
    let param;

    if (lat === null && lng === null) {
        param = {
            piva: $('#' + hdPivaClientID).val(),
            id_contatore: gruppoConsegna,
            id_stazione_user: id_stazione_user,
            latcentro: 0,
            longcentro: 0,
            DataDa: kendo.toString(dataInizio, "yyyy-MM-dd"),
            DataA: kendo.toString(dataFine, "yyyy-MM-dd"),
            VisualizzaStorico: storico
        };
    } else {
        param = {
            piva: $('#' + hdPivaClientID).val(),
            id_contatore: gruppoConsegna,
            id_stazione_user: id_stazione_user,
            latcentro: lat,
            longcentro: lng,
            DataDa: kendo.toString(dataInizio, "yyyy-MM-dd"),
            DataA: kendo.toString(dataFine, "yyyy-MM-dd"),
            VisualizzaStorico: storico
        };
    }

    ajaxAgronicaSync(indirizzohttp + "/CalcolaConfrontoPrelieviOsservatiAttesi",
        JSON.stringify(param),
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            $("#" + hdKendoColumsCalcoloClientID).val(JSON.stringify(JSON.parse(risp.Table).kendo_columns));
            $("#" + hdKendoRowsCalcoloClientID).val(JSON.stringify(JSON.parse(risp.Table).kendo_rows));
            $("#" + hdKendoModelCalcoloClientID).val(JSON.stringify(JSON.parse(risp.Table).kendo_model));
            popolaGrigliaRisultatiCalcolo("divKendoCalcoloOut");
            let chartPrel = risp.Charts.find(el => el.ChartID === "modello");
            if (chartPrel !== null) {
                createChartPrelievi(chartPrel.ChartData);
            }

            let chartVol = risp.Charts.find(el => el.ChartID === "volume");
            if (chartVol !== null) {
                createChartVolume(chartVol.ChartData);
            }

            ShowTab(2, true);
        }, function (risposta) {
            kendo.alert(risposta.Errore);
            ShowTab(2, false);
        });

}

//---- RICHIAMI AL METEO ----//
var indirizzoMeteoHttp = "../Meteo/MeteoWS.aspx";

function LoadSorgentiMeteo(options) {

    let pars = JSON.stringify({});

    ajaxAgronicaSync(indirizzoMeteoHttp + "/SorgentiMeteo",
        pars,
        false,
        function (risposta) {
            options.success(JSON.parse(risposta.RispostaStringa));
        },
        function (risposta) {
            let sorgenti = [
                {
                    sorgente_cod: -1,
                    sorgente_des: "Non disponibile"
                }
            ];
            options.success = sorgenti;
        });
}

function LeggiStazioniXSorgente(options) {
    let tipoSorgente = KendoDDL("ddlTipoSorgente").value();
    let param = {
        TipoSorgente: tipoSorgente,
        Lat: 0,
        Lng: 0
    };

    if (tipoSorgente == 4) {
        var coords = $("#geo-pos-edit-meteo").data("geoPosEdit").getLatLngDec();

        if (coords !== undefined && coords !== null) {
            param.Lat = coords.lat;
            param.Lng = coords.lng;
        }
    }

    //if (lat !== null && lng !== null) {
    //    param.Lat = lat;
    //    param.Lng = lng;
    //}

    ajaxAgronicaSync(indirizzoMeteoHttp + "/LeggiStazioniXSorgente",
        JSON.stringify(param),
        false,
        function (risposta) {
            let sorgenti = JSON.parse(risposta.RispostaStringa);
            if (sorgenti === undefined || sorgenti === null || sorgenti.length<=0) {
                sorgenti = [
                    {
                        id_stazione: 0,
                        nome_stazione: ""
                    }
                ];
            }

            options.success(sorgenti);
        },
        function (risposta) {
            let sorgenti = [
                {
                    id_stazione: -1,
                    nome_stazione: "Non disponibile"
                }
            ];
            options.success = sorgenti;
        });
}

//---- RICHIAMI AL METEO ----//