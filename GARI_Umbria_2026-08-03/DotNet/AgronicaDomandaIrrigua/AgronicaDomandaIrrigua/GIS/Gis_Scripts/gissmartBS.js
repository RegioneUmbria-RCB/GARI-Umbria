

/*
    gissmartBS.js


    Per attivare sul telefono la modalità sviluppatore:

    1. premere 3 volte sull'icona di agronica
    2. digitare la password: "v"
    3. selezionare ok



*/

var GisSmartBsBackGround = false;
var ContaXDevMode = 0;
var DeveloperMode = false;
var DeveloperModeGPSLog = false;
var DeveloperModeGPSLog_MaxMSG = 7;
var DeveloperModeGPSLog_curMSG = 0;
var DeveloperModeCoord = true;
var ListaImpreseInizializzata = false;

var isSementi = false;
var milliSecondiPollingStatoOnLine = 7000;
var LavoraOnLine = false;

var initRecuperaStatoOnLine = true;
var RecuperaStatoOnLine = true;
var intervalRecuperaStatoOnLine = 0;
var TimeoutRecuperaStatoOnLine = 30000;
var ForzaStatoOffLine = 0;
var ForzaStatoOffLine_NumeroClick = 2;
var ForzaStatoOffLine_Incrementa = true;

var GPS_Minimum_Age = 30000;
var GPS_Timeout = 27000;


var posizione = "";
var intervalGPS = 0;
var TimeOutGPS = 10000;

var aggiornaTimeoutInizializza = 1500;

//tipo enumerativo per lo stato del disegno.
var statoDisegno = {
    Nessuno: { value: 0, name: "Nessuno", code: "N" },
    InCorso: { value: 1, name: "In Corso", code: "C" }
}

var enumOggettiDisegnabili = {
    Punto: { value: 0, name: "Punto", code: "P" },
    Poligono: { value: 1, name: "Poligono", code: "G" }
}


/**
 * @typedef {Object} gisSmartBsCfg
 * @property {enumOggettiDisegnabili} tipoOggettoGisDaDisegnare usare: enumOggettiDisegnabili
 */
var gisSmartBsCfg = {
    tipoOggettoGisDaDisegnare: enumOggettiDisegnabili.Poligono
}


//tipo enumerativo per lo stato del disegno in google maps.
var enumStatoDisegnoGoogle = {
    Nessuno: { value: 0, name: "Nessuno", code: "N" },
    InCorso: { value: 1, name: "In Corso", code: "C" },
    Completato: { value: 2, name: "Completato", code: "CC" }
}


function scrollTop() {
    try {
        $("html, body").animate({ scrollTop: 0 }, "slow");
    }
    catch (e) {
    }
}
function alertOk(messaggio) {

    scrollTop();
    MessaggioTuttoOK_Bootstrap(messaggio, "DIV_Messaggi");
}

function alertErr(messaggio) {
    scrollTop();
    MessaggioErrore_Bootstrap(messaggio, "DIV_Messaggi");
}

function alertWarn(messaggio) {
    scrollTop();
    MessaggioWarning_Bootstrap(messaggio, "DIV_Messaggi");
}

function AttivaDisattivaMenuDevMode() {
    ContaXDevMode++;

    if (ContaXDevMode == 3) {
        $("#MenuDevMode").show();
        ContaXDevMode = 0;
    }
}

function AttivaDevModePass() {

    var pp = $("#txt_DevPass").val();

    if (pp == "v" || pp == "V") {
        $("#MenuDevMode2").show();
    }

}


function AttivaDevModePass2() {

    DeveloperMode = $("#ckDeveloperMode").is(':checked');
    DeveloperModeGPSLog = $("#ckDeveloperModeGPSLog").is(':checked');
    DeveloperModeCoord = $("#ckDeveloperModeCoord").is(':checked');
    setDeveloperMode();
    $("#MenuDevMode").hide();
    $("#MenuDevMode2").hide();
}

function LG(testo) {

    $("#GPSLog").html(
        $("#GPSLog").html() + "<br />" + testo
    );

    if (DeveloperModeGPSLog_curMSG >= DeveloperModeGPSLog_MaxMSG) {
        DeveloperModeGPSLog_curMSG = 0;
        LG_Clear();
    } else {
        DeveloperModeGPSLog_curMSG++;
    }


}
function LG_Clear() {
    $("#GPSLog").html("");
}

function LG_CurGPS_Pos(c) {
    $(".CurGPS_Pos").html(c);
}

function setDeveloperMode() {
    if (!DeveloperMode) {
        //nascondo un po' di cose...
        //$("#btn_EliminaTuttoQuanto").hide();
        $("#btn_MostraTuttoQuanto").hide();
        $(".Cancellapoligono").hide();
        $("#NuovoPuntoPoligonon").hide();
        $("#idmodifica").hide();
    } else {

        //$("#btn_EliminaTuttoQuanto").show();
        $("#btn_MostraTuttoQuanto").show();
        $(".Cancellapoligono").show();
        $("#NuovoPuntoPoligonon").show();
        $("#idmodifica").show();

    }

    if (!DeveloperModeGPSLog) {
        $("#GPSLog").hide();
    } else {
        $("#GPSLog").show();
    }

    if (!DeveloperModeCoord) {
        $(".CurGPS_Pos").hide();
    } else {
        $(".CurGPS_Pos").show();
    }

}

var statoDisegnoCorrente = statoDisegno.Nessuno;
var statoDisegnoCorrenteGoogle = enumStatoDisegnoGoogle.Nessuno;



var arrayPoligoni;
var numeroPoligoni;
var arrayDescrizione;
var poligonoCacheEntitaCod;
var entitaCod;


/*

gestione cookies

*/

function cookieNumeroPoligoni() {

    $.removeCookie('numeroPoligoni', numeroPoligoni);
    $.cookie('numeroPoligoni', numeroPoligoni, { expires: 7 });

}

function cookieDescrizione() {
    $.removeCookie('arrayDescrizione', "");
    $.cookie('arrayDescrizione', arrayDescrizione, { expires: 7 });
}


function cookiePoligonoCacheEntitaCod() {
    $.removeCookie('poligonoCacheEntitaCod', "");
    $.cookie('poligonoCacheEntitaCod', JSON.stringify(poligonoCacheEntitaCod), { expires: 7 });
}





//per pagina bootstrap
function inizializzaTutto() {




    $('#idmodifica').html('0');

    localizzaDaRisorse();

    

}



function PulsanteNuovoPunto(emulazioneViaMappa) {


    if (
        !emulazioneViaMappa &&
        ImpiantoCaricaDatiElaboraPunti_Pulisci
        && statoDisegnoCorrente == statoDisegno.Nessuno) {

        // sto prendendo punti da GPS
        RimuoviPoligonoInFaseDisegno();
        ImpiantoCaricaDatiElaboraPunti_Pulisci = false;
        EditViaPunto();
    }


    switch (statoDisegnoCorrente) {

        case statoDisegno.Nessuno:

            var t = $("#txt_descrizione").val()
            if (t == undefined || t == "") {
                alert("Descrizione obbligatoria");
                break;
            }

            ImpostaNuovoDisegno();
            AcquizionePunto();
            statoDisegnoCorrente = statoDisegno.InCorso;
            break;

        case statoDisegno.InCorso:
            AcquizionePunto();
            break;

        default:
            break;
    }
}

function AcquizionePunto() {

    BlinkPulsanteOn();

    //sleep(3000);

    aggiungiPuntoaPoligono();
    inizializzaPoligoni(true);

    //sleep(3000);
    BlinkPulsanteOff();

}



function GestioneSalvataggio() {

    if (statoDisegnoCorrente == statoDisegno.InCorso) {

        if (LavoraOnLine) {

            SalvaPoligonoEsistenteBS();

        } else {

            statoDisegnoCorrente = statoDisegno.Nessuno;

            //ora gestito impostando valore vuoto nella combo dei poligoni.
            ddl_Recupera.value("");
            RecuperaPoligono();
            //$(".rigapoligono").hide();
            //$("#txt_descrizione").val("");
            alert("Memorizzato.");

        }


        //vavava
        //$(".InCorso").show();

        EditTipo_Azzera();

    }
}


function RecuperaPoligono() {

    //var sel = $("#ddl_recupera option:selected").val();
    var sel = ddl_Recupera.value();

    $(".rigapoligono").hide();

    if (sel != undefined && sel != "") {

        var poligonoCorrente = parseInt(sel);
        poligonoCorrente = poligonoCorrente - 1;

        $('#idmodifica').html(sel);
        $(".rigapoligono_" + poligonoCorrente).show();

        $("#txt_descrizione").val(ddl_Recupera.text());

        statoDisegnoCorrente = statoDisegno.InCorso;

    } else {
        //imposto nessun poligono in modifica
        $('#idmodifica').html('0');
        $('#hidden_ID_poligono_x_salvataggio').val("");
    }


}

function CancellaUltimoPunto(cosa) {
    //da cancellare
    var da_cancellare = $(cosa).attr('id').split("_")[2];
    var nuovalistapoligoni = '';
    var mySplit = arrayPoligoni.split("/");
    for (i = 0; i < mySplit.length - 1; i++) {
        if (i != da_cancellare) {
            nuovalistapoligoni = nuovalistapoligoni + mySplit[i] + "/";
        } else {
            //copio tutto eccetto gli ultimi punti
            var listaP = mySplit[i].split("|");

            //se è l'ultimo punto allora cancello tutto il poligono...
            if (listaP.length == 2) {
                Cancellapoligono($("#Cancellapoligono_" + da_cancellare));
                return;
            }

            for (var j = 0; j < (listaP.length / 2) - 1; j++) {
                if (j > 0) nuovalistapoligoni = nuovalistapoligoni + "|";

                nuovalistapoligoni = nuovalistapoligoni + listaP[j * 2] + "|" + listaP[j * 2 + 1];
            }
            if (listaP.length > 3) nuovalistapoligoni = nuovalistapoligoni + "/";
        }
    }


    arrayPoligoni = nuovalistapoligoni;
    $.cookie('arrayPoligoni', arrayPoligoni, { expires: 7 });
    inizializzaPoligoni(true);
}

function Cancellapoligono(cosa) {

    var da_cancellare = parseInt($(cosa).attr('id').split("_")[1]);
    var nuovalistapoligoni = '';
    var nuovalistadescrizioni = '';
    var mySplit = arrayPoligoni.split("/");
    var mySplitDes = arrayDescrizione.split("§");
    for (i = 0; i < mySplit.length; i++) {
        if (i != da_cancellare) {
            nuovalistapoligoni = nuovalistapoligoni + mySplit[i] + "/";
            nuovalistadescrizioni = nuovalistadescrizioni + mySplitDes[i] + "§";
        }
    }

    arrayPoligoni = nuovalistapoligoni;
    arrayDescrizione = nuovalistadescrizioni;

    numeroPoligoni = numeroPoligoni - 1;
    cookieNumeroPoligoni();

    if (numeroPoligoni == 0) {
        arrayDescrizione = "";
        arrayPoligoni = "";
    }

    $.cookie('arrayDescrizione', arrayDescrizione, { expires: 7 });
    $.cookie('arrayPoligoni', arrayPoligoni, { expires: 7 });
    
    var o = poligonoCacheEntitaCodRicercaXNumero(da_cancellare);
    var index = poligonoCacheEntitaCod.indexOf(o[0]);
    if (index > -1) {
        poligonoCacheEntitaCod.splice(index, 1);
    }

    cookiePoligonoCacheEntitaCod();


    //$("#ddl_recupera option[value='" + (da_cancellare + 1) + "']").remove();
    //$("#ddl_recupera").selectpicker("refresh");

    kendoDropDown_removeByValue("#ddl_recupera", (da_cancellare + 1).toString(), "Recupera_Cod");

    //vavava
    //$(".InCorso").show();

    statoDisegnoCorrente = statoDisegno.Nessuno;

    inizializzaPoligoni(true);

    EditTipo_Azzera();

    
}


var BlinkPulsante_txt = "";
function BlinkPulsanteOn() {
    BlinkPulsante_txt = $("#btn_nuovoPunto").html();
    $("#btn_nuovoPunto").html("Attendi ...");
    $("#btn_nuovoPunto").addClass("offlineGray");
    //WaitFrame.show();
}

function BlinkPulsanteOff() {
    $("#btn_nuovoPunto").removeClass("offlineGray");
    $("#btn_nuovoPunto").html(BlinkPulsante_txt);
    BlinkPulsante_txt = "";
    //WaitFrame.hide();
}

function ImpostaNuovoDisegno() {

    numeroPoligoni++;
    $('#idmodifica').html(numeroPoligoni);
    cookieNumeroPoligoni();



    var a = "";
    a = numeroPoligoni.toString();

    var escapeDesc = $("#txt_descrizione").val();
    escapeDesc = JsonEscape(escapeDesc);

    var selectValues = JSON.parse('{"' + a + '" : "' + escapeDesc + '" }');
    jQueryAddSelect("#ddl_recupera", selectValues, "Recupera_Cod", "Recupera_Des");

    //$("#ddl_recupera").val(a);
    ddl_Recupera.value(a);

    memorizzaPoligonoCacheEntitaCod(entitaCod);

    //la chiamata potrebbe avvenire da pagina non bootstrap...
    try {
        $('#ddl_recupera').selectpicker('refresh');
    } catch (e) {
    }

    //vavava
    //$(".InCorso").hide();

}


function ChiudiDisegno() {
    $("#btn_nuovoPunto").html("Nuovo Elemento");
}


function mostraRecuperaPunti() {
    $(".inCorso").show();
}

function nascondiRecuperaPunti() {
    $(".inCorso").hide();
}


//testa lo stato se sono online.. altrimenti esce.
setInterval(function () {


    if (!initRecuperaStatoOnLine) {

        initRecuperaStatoOnLine = false;
        if (!LavoraOnLine && !RecuperaStatoOnLine) {

            return;
        }
    }

    testOnOffLine();

}, milliSecondiPollingStatoOnLine);


//Decide se recuperare lo stato online in maniera forzata se internet è Ok.
intervalRecuperaStatoOnLine = window.setInterval(
    function () {
        if (ForzaStatoOffLine < ForzaStatoOffLine_NumeroClick) {
            RecuperaStatoOnLine = true;
        } else {
            RecuperaStatoOnLine = false;
            ForzaStatoOffLine_Incrementa = false;
        }
    }
, TimeoutRecuperaStatoOnLine);




// funzionalità di test per rete ...
function testOnOffLine() {

    $("#lbl_StatoRete").removeClass("offlineGray");
    $("#lbl_StatoRete").removeClass("offline");
    $("#lbl_StatoRete").removeClass("online");


    if (navigator.onLine) {
        if (!initRecuperaStatoOnLine || (!LavoraOnLine && RecuperaStatoOnLine)) {
            OnOffLine(true, false);

        }
    }

    $("#lbl_StatoRete").addClass(navigator.onLine ? 'online' : 'offline');

}

function formOnline() {
    $(".formGisSmartBSOnline").show();
    $(".formGisSmartBSOffline").hide();
}

function formOffLine() {
    $(".formGisSmartBSOnline").hide();
    $(".formGisSmartBSOffline").show();

}

function OnOffLine(stato, ChiamataDaPulsante) {

    LavoraOnLine = stato;
    RecuperaStatoOnLine = stato;

    if (stato) {
        $("#lbl_StatoRete").html("Rete");
        $("#btn_Off_OnLine").html("Lavora OffLine");
        $("#btn_Off_OnLine").removeClass("online");
        $("#btn_Off_OnLine").addClass("offline");
        formOnline();
        $("#btn_Salva").html("Salva");
        inizializzaFormOnLine();
    } else {

        if (ForzaStatoOffLine_Incrementa && ChiamataDaPulsante)
            ForzaStatoOffLine = ForzaStatoOffLine + 1;

        $("#lbl_StatoRete").html("Stai lavorando offline");
        $("#lbl_StatoRete").removeClass("offline");
        $("#lbl_StatoRete").removeClass("online");
        $("#lbl_StatoRete").addClass("offlineGray");
        $("#btn_Off_OnLine").html("Lavora OnLine");
        $("#btn_Off_OnLine").addClass("online");

        $("#btn_Salva").html("Salva in memoria");

        formOffLine();
    }
}

function inizializzaFormOnLine() {

    //' VAnni: 7/4/2017: Logica di caricamento imprese modificata.....
    //if (!ListaImpreseInizializzata) {
    //    CaricaAziendaBS("");
    //}

}


//testa il GPS
//var interval;
//requestPosition();


//function requestPosition() {

//    $("#lbl_StatoGPS").html("Verifico disp. GPS");

//    var nav = window.navigator;    
//    var geoloc = nav.geolocation;
//    if (geoloc !== null) {

//        interval = window.setInterval(function () {
//            geoloc.watchPosition(successCallback, errorCallback, {
//                timeout: 4000,
//                enableHighAccuracy: true
//            });

//        }, 5000);

//    }
//}

function GPS_successCallback(position) {

    $("#lbl_StatoGPS").html("Stato GPS");
    $("#lbl_StatoGPS").removeClass("offlineGray");
    $("#lbl_StatoGPS").removeClass("offline");
    $("#lbl_StatoGPS").addClass("online");
}

function GPS_errorCallback(error) {

    $("#lbl_StatoGPS").html("GPS Offline");
    $("#lbl_StatoGPS").removeClass("offlineGray");
    $("#lbl_StatoGPS").removeClass("online");
    $("#lbl_StatoGPS").addClass("offline");
}




var wpid;

/**
 * Richiede la posizione GPS
 * @param {any} onGeoSuccess Funzione da chiamare appena ottenute le coordinate, che saranno passate ai due parametri lat, lng
 */
function requestPosition(onGeoSuccess, onGeoError) {

    $("#lbl_StatoGPS").html("Verifico GPS");
    $("#lbl_StatoGPS").addClass("offlineGray");


    if (navigator.geolocation) {
    } else {
        alertErr("GPS non supportato dal dispositivo. ");
        return;
    }

    if (intervalGPS == 0) {

        if (DeveloperModeGPSLog) {
            LG("Inizializza... intervalGPS = ");
        }

        intervalGPS = window.setInterval(function () {

            LG();

            if (wpid === undefined) {


                wpid = navigator.geolocation.watchPosition(
                    function geo_success(position) {
                        posizione = position.coords.latitude + "|" + position.coords.longitude;
                        if (DeveloperModeCoord) {

                            LG_CurGPS_Pos(posizione);
                            GPS_successCallback(posizione);

                            if (onGeoSuccess !== undefined) {
                                onGeoSuccess(position.coords.latitude, position.coords.longitude);
                            }

                        }

                        if (DeveloperModeGPSLog) {
                            LG("Coord Ok..");
                        }

                    },
                    function error(msg) {
                        var msg11 = "";
                        if (DeveloperModeGPSLog) {
                            LG(msg.message);
                        }
                        GPS_errorCallback(msg);

                        if (onGeoError !== undefined) {
                            onGeoError(msg);
                        }

                    }, {
                        enableHighAccuracy: true,
                        maximumAge: GPS_Minimum_Age,
                        timeout: GPS_Timeout
                    });
            }
        }, TimeOutGPS);


    }

}