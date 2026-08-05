

/*

Funzioni comuni e cartografiche 

*/



var gisSmartIndirizzoHttp = "gissmart.aspx";
//gisSmartIndirizzoHttp = gisSmartIndirizzoHttp.split("/")[gisSmartIndirizzoHttp.split("/").length - 1];

var millisecondiReturnPosizione = -1;


var mostraTuttoQuantoStato = false;
function MostraTuttoQuanto() {


    if (mostraTuttoQuantoStato) {
        $(".rigapoligono").hide();
        mostraTuttoQuantoStato = false;
    } else {
        $(".rigapoligono").show();
        mostraTuttoQuantoStato = true;
    }

}


function EliminaTuttoQuanto() {
    $.removeCookie('arrayDescrizione', "");
    $.removeCookie('arrayPoligoni', "");
    $.removeCookie('numeroPoligoni', 0);
    $.removeCookie('poligonoCacheEntitaCod', "");

    $('#idmodifica').html('0');
    arrayPoligoni = "";
    numeroPoligoni = 0;
    arrayDescrizione = "";
    poligonoCacheEntitaCod = new Array();

    $('#listapoligonisalvati').html("");
    //$("#ddl_recupera").html("");
    //$("#ddl_recupera").selectpicker("refresh");

    ddl_Recupera.setDataSource( new kendo.data.DataSource({}) ); // clears dataSource
    ddl_Recupera.text(""); // clears visible text
    ddl_Recupera.value(""); // clears invisible value

    statoDisegnoCorrente = statoDisegno.Nessuno;

    inizializzaPoligoni(true);

    nascondiRecuperaPunti();
    
}

/******************* returnPosizione *************************/
function returnPosizione() {

    LG("Start returnPosizione...");

    //' VAnni: 13/4/2017: se ho un punto da mappa allora recupero da lì.
    if (posizione_DaMappa_Lat != "") {
        var mappa_rval = posizione_DaMappa_Lat + "|" + posizione_DaMappa_Lng;
        posizione_DaMappa_Lat = "";
        posizione_DaMappa_Lng = "";

        return mappa_rval;
    }
    

    if (millisecondiReturnPosizione > 0 ) {
        var lat = parseFloat(0.0);
        var long = parseFloat(0.0);
        for (var i = 0; i < 5; i++) {
            sleep(millisecondiReturnPosizione);
            lat = lat + parseFloat(posizione.toString().split("|")[0]);
            long = long + parseFloat(posizione.toString().split("|")[1]);

            LG("Lat: " + lat + " - long: " + long);

        }
        lat = lat / 5;
        long = long / 5;

        LG("END returnPosizione...");
        return lat + "|" + long;
    } else {
        LG("END returnPosizione...");
        return posizione;
    }
    

}

/******************* aggiungiPuntoaPoligono *************************/
function aggiungiPuntoaPoligono() {

    if (arrayPoligoni == undefined) {
        alert("Impossibile aggiungere un punto, verificare lo stato del GPS.");
        return;
    }

    var id_poligono = $('#idmodifica').html();
    var arrayAppoggio = '';
    var i;
    var mySplit = arrayPoligoni.split("/");
    for (i = 0; i < numeroPoligoni; i++) {
        if (i != (id_poligono - 1)) {
            arrayAppoggio = arrayAppoggio + mySplit[i] + "/";
        } else {
            if (mySplit[i] == "") {

                arrayAppoggio = arrayAppoggio + returnPosizione() + "/";
            } else {
                arrayAppoggio = arrayAppoggio + mySplit[i] + "|" + returnPosizione() + "/";
            }
        }
    }
    arrayPoligoni = arrayAppoggio;
    $.cookie('arrayPoligoni', arrayPoligoni, { expires: 7 });
    // $('#WaitFrame').hide();
}

/******************* Inizializzazione *************************/
function inizializzaPoligoni(bs) {

    
    var i = 0;
    if (arrayPoligoni == null) arrayPoligoni = "";
    var mySplit = arrayPoligoni.split("/");
    if (mySplit.length == 1) {
        if (mySplit[0] == "") {
            numeroPoligoni = 0;
        } else {
            numeroPoligoni = mySplit.length - 1;
        }
    } else {
        numeroPoligoni = mySplit.length - 1;
    }


    var listapoligoni = "";
    
    if (DeveloperMode) {
        lblListaPoligonipresentiincache + (mySplit.length - 1) + lblcoordinateseparateda + " <br>";
    }

    //var listapoligoni = "Lista Poligoni presenti in cache: " + (mySplit.length - 1) + " (coordinate separate da |)<br>";

    for (i = 0; i < numeroPoligoni; i++) {

        if (mySplit[i] != "") {
            listapoligoni = listapoligoni + getToolxPoligono(i, mySplit[i], bs);

        }
    }

    if (bs) {
        
        
        $('#listapoligonisalvati').html(listapoligoni);
        

        //var PoligonoCorrente = parseInt($("#ddl_recupera option:selected").val());
        var PoligonoCorrente = parseInt( ddl_Recupera.value() );
        PoligonoCorrente = PoligonoCorrente - 1;
        $(".rigapoligono").hide();
        $(".rigapoligono_" + PoligonoCorrente).show();

        SalvaDescrizioni();

        setDeveloperMode();

        PulsantiGestione(PoligonoCorrente);

    }

};

function PulsantiGestione(PoligonoCorrente) {

    if (isNaN(PoligonoCorrente)) {
        $("#btn_Salva").hide();
        $("#btn_nuovoPunto").show();
        return;
    }

    //disabilito il pulsante di salvataggio
    var numeroPuntiAcquisiti = $(".rigapoligono_" + PoligonoCorrente + " .coord").length;
    var puntoAcquisito = (numeroPuntiAcquisiti > 1 && gisSmartBsCfg.tipoOggettoGisDaDisegnare == enumOggettiDisegnabili.Punto);
    var puntoAcquisitoDisabilitaNuovoPunto = (numeroPuntiAcquisiti == 1 && gisSmartBsCfg.tipoOggettoGisDaDisegnare == enumOggettiDisegnabili.Punto);

    if (puntoAcquisitoDisabilitaNuovoPunto) {
        $("#btn_nuovoPunto").hide();
    } else {
        $("#btn_nuovoPunto").show();
    }

    if (numeroPuntiAcquisiti == 2 || puntoAcquisito) {
        $("#btn_Salva").hide();
        $("#btn_Anteprima").hide();
    } else {
        $("#btn_Salva").show();
        $("#btn_Anteprima").show();
    }
}

function SalvaDescrizioni() {

   

    var rval = "";
    //$("#ddl_recupera option").each(function () {

    var arr = $("#ddl_recupera").data("kendoDropDownList").dataSource._data;
    for (var i = 0; i < arr.length; i++) {        
    
        rval = rval + arr[i].Recupera_Cod + "^" + arr[i].Recupera_Des + "§";
        
    }

    if (rval != "") {
        arrayDescrizione = rval;
        cookieDescrizione();
    }

}



function ddlRecuperaReInizializza() {

    ddl_Recupera.setDataSource(new kendo.data.DataSource({})); // clears dataSource
    ddl_Recupera.text(""); // clears visible text
    ddl_Recupera.value(""); // clears invisible value

    inizializzaComboDescrizioni();

    if (numeroPoligoni !== undefined && numeroPoligoni !== null && numeroPoligoni > 0)
        ddl_Recupera.value(numeroPoligoni);
}

function inizializzaComboDescrizioni() {


    if (arrayDescrizione != "") {

        var SplitArrayDescrizioni = arrayDescrizione.split("§")

        for (var i = 0; i < SplitArrayDescrizioni.length - 1; i++) {

            var CodiceDescrizione = SplitArrayDescrizioni[i].split("^");
            var escapeDesc = CodiceDescrizione[1];
            escapeDesc = JsonEscape(escapeDesc);
            var jSApp = '{ "' + CodiceDescrizione[0] + '": "' + escapeDesc + '"}';

            var selectValues = JSON.parse(jSApp);
            jQueryAddSelect("#ddl_recupera", selectValues, "Recupera_Cod", "Recupera_Des");

        }

        try {
            $("#ddl_recupera").selectpicker("refresh");
        } catch (e) {
        }

    }

}


/******************* Tool Poligoni *************************/
function getToolxPoligono(i, stringa, bs) {

   
    var risp = "";

    //intestazione
    if (bs) {

        risp =
         "<div class='row  rigapoligono rigapoligono_" + i + "' >" +
         "           <div class='col-lg-6 col-md-6 col-sm-12'>" +
         "               <div class='form-horizontal'>" +
         "                   <div class='form-group'>" +
         "                       <div class='input-group'>" +
         "                          <div style='float:left; margin-top:10px'><b>" + lblPoligono + " n." + (i + 1) + "</b></div>" +
         "                          <div class='btn btn-danger btn_100 Cancellapoligono' id='Cancellapoligono_" + i + "' style='white-space: normal;'>" + lblEliminaPoligono + "</div>" +
         "                          <div class='punti'>";
         

    } else {

        risp = 
         "<div class='rigapoligono'>" +
         "<div style='float:left; margin-top:10px'><b>" + lblPoligono + " n." + (i + 1) + "</b></div>" +
         "<div id='icons' style='float:left; margin-left:10px;margin-top:5px'> " +
         "<div id='Modificapoligono_" + i + "' class='ui-state-default ui-corner-all Modificapoligono tool' title='" + lblModificaPoligono + "'><span class='ui-icon ui-icon-triangle-1-n'></span></div>" +
         "<div id='VisualizzaPoligono_" + i + "' class='ui-state-default ui-corner-all VisualizzaPoligono tool' title='" + lblVisualizzazioneRapidadelPoligono + "'><span class='ui-icon ui-icon-circle-zoomin'></span></div>" +
         "<div id='Cancellapoligono_" + i + "' class='ui-state-default ui-corner-all Cancellapoligono tool' title='" + lblEliminaPoligono + "'><span class='ui-icon ui-icon-trash'></span></div>" +
         "<div style='clear:both;'></div>" +
         "</div>" +
         "<div style='clear:both;'></div>" +
         "<div class='punti'>";
    }

    //coordinate
    var listaP = stringa.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        risp = risp + "<div class='coord'><b>N.</b> " + (j + 1) + ": <b>Lat.</b>" + listaP[j * 2] + " <b>Long.</b>" + listaP[j * 2 + 1] + "</div>";
    }

    //chiusura
    if (bs) {
        risp = risp + "<div class='btn btn-warning btn_100 CancellaUltimoPunto' id='btn_CancellaUltimoPunto_" + i + "' style='white-space: normal;'>" + lblCancellaUltimoPunto + "</div>";
        risp = risp + "</div></div></div></div></div></div>";
    } else  {
        risp = risp + "<div id='CancellaUltimoPunto_" + i + "' class='ui-state-default ui-corner-all CancellaUltimoPunto toolUltimo' title='" + lblCancellaUltimoPunto + "'><span class='ui-icon ui-icon-trash'></span>" + lblCancellaUltimoPunto + "</div>";
        if (listaP.length / 2 > 2) {
            risp = risp + "<div id='salvaPoligono_" + i + "' class='ui-state-default ui-corner-all salvaPoligono toolUltimo' title='" + lblSalvaPoligono + "'><span class='ui-icon ui-icon-disk'></span>" + lblSalvaPoligono + "</div>";
        }
        risp = risp + "<div class='clear'></div>";
        risp = risp + "</div></div></br></br>";
    }
    
    return risp;
}


/*****************************************************************/
/******************* AJAX AJAX AJAX AJAX *************************/
/*****************************************************************/

/******************* Carica Tipologia ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaTipologia(Veg_Cod, combo_tipologia, valoreSelezionato) {

    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaTipologia",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_tipologia).html(msg.d);
            }
            if (valoreSelezionato != "")
                $(combo_tipologia).val(valoreSelezionato);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

/******************* Carica Varieta ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaVarieta(combo_varieta, cul_cod) {

    var Veg_Cod;

    if (combo_varieta == '#pop_up_varieta') {
        Veg_Cod = $('#pop_up_specie').val();
    } else {
        //        pop_up_varieta_modifica
        Veg_Cod = $('#pop_up_specie_modifica').val();
    }

    $.logThis("Veg_CodVerieta :" + Veg_Cod);

    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaVarieta",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_varieta).html(msg.d);
                if (cul_cod != '') {
                    $(combo_varieta).val(cul_cod)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function r_ok() {
    response_ok = response_ok - 1;
    if (response_ok == 0) {
        $('#WaitFrame').hide();
    }
}


/******************* Carica Finalita ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaFinalita(combo_finalita, finalita) {

    if (combo_finalita == '#pop_up_finalita') {
        var Veg_Cod = $('#pop_up_specie').val();
    } else {
        //        pop_up_varieta_modifica
        var Veg_Cod = $('#pop_up_specie_modifica').val();
    }

    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaFinalita",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_finalita).html(msg.d);
                if (finalita != '') {
                    $(combo_finalita).val(finalita)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}





/******************* Carica CaricaAzienda ************************/
function GetSuperficie() {
    var Vertici = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;
    i = parseInt($('#hidden_ID_poligono_x_salvataggio').val()) + 1;
    var k = mySplit[i - 1];

    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            Vertici = Vertici + ",";
        Vertici = Vertici + "(" + listaP[j * 2] + ", " + listaP[j * 2 + 1] + ")";
    }
    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/GetSuperficie",
        data: "{Vertici:'" + Vertici + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if ($('#pop_up_sup_google').is(":visible") == true) {
                    //NUOVO
                    $('#pop_up_sup_google').val(msg.d);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


/******************* Carica CaricaAzienda ************************/
function CaricaAzienda() {
    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaAzienda",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if ($('#cmdImpresa').is(":visible") == true) {
                    //Esistente
                    $('#cmdImpresa').html(msg.d);
                    CaricaCentroAziendale();
                }
                else {
                    //nuovo
                    $('#pop_up_impianto_azienda').html(msg.d);
                    CaricaCentroAziendale();
                    CaricaSpecie("#pop_up_specie", '');
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


var CaricaAziendaBS_lenPrecTestoRicerca = 0;


function aziendaCaricaTutte() {

    CaricaAziendaBS("", true);
    
}

/******************* Carica CaricaAzienda ************************/
function CaricaAziendaBS(testoRicerca, caricaTutto) {

    if (caricaTutto) {

        CaricaAziendaBS_getData(testoRicerca);
        return true;
    }

    //non mi mandi nulla.. esco.
    if (testoRicerca === undefined)
        return false;

    //' VAnni: 7/4/2017: ho scritto troppo poco .. nessuna ricerca lato server..
    if (testoRicerca.length < 3)
        return false;

    //' VAnni: 7/4/2017: ho scritto di più .. nessuna ricerca lato server..
    if (testoRicerca.length > CaricaAziendaBS_lenPrecTestoRicerca && CaricaAziendaBS_lenPrecTestoRicerca > 0) {
        CaricaAziendaBS_lenPrecTestoRicerca = testoRicerca.length;
        return false;
    } else {

        CaricaAziendaBS_lenPrecTestoRicerca = testoRicerca.length;

        CaricaAziendaBS_getData(testoRicerca);


    }

    return true;
}

var CaricaAziendaBS_getData = function (testoRicerca) {
    
    ajaxAgronica(gisSmartIndirizzoHttp + "/CaricaAziendaBS",
        JSON.stringify({ testoRicerca: testoRicerca }),        
        function (risposta) {
            if (risposta.RispostaStringa != 'NULL') {

                
                var caricaAziendaBsDati = JSON.parse(risposta.RispostaStringa);

                var dataSource = new kendo.data.DataSource({
                    data: caricaAziendaBsDati
                });

                ddl_Azienda.setDataSource(dataSource);
                ddl_Azienda.refresh();

                alertOk("Aziende Caricate correttamente");
            }
            //' VAnni: 7/4/2017: non più necessario.
            //ListaImpreseInizializzata=true;
        },
        null);
       

}


/******************* Carica CentroAziendale ************************/
function CaricaCentroAziendale() {
    var azienda_selezionata;
    if ($('#cmdImpresa').is(":visible") == true) {
        azienda_selezionata = $('#cmdImpresa').val();
    } else {
        azienda_selezionata = $('#pop_up_impianto_azienda').val();
    }
    if (azienda_selezionata != "") {
        $.ajax({
            type: "POST",
            url: gisSmartIndirizzoHttp + "/CaricaCentroAziendale",
            data: "{Piva: '" + azienda_selezionata + "', isSementi: '" + isSementi + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {
                    if ($('#cmdCentro').is(":visible") == true) {
                        //Esistente
                        $('#cmdCentro').html(msg.d);
                        CaricaSpecieDaCentro();
                    }
                    else {
                        //nuovo
                        $('#pop_up_centro').html(msg.d);
                        if ($(".ddl_Sa_Cod_sx").length > 0) {
                            $('#pop_up_centro').val($('.ddl_Aziende').val() + "|" + $('.ddl_Sa_Cod_sx').val());
                        }
                    }
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}


//per chiamata Standard Ajax
function CaricaCentroAziendaleBS() {

    var azienda_selezionata;

    //' VAnni: 10/4/2017: ora azienda viene da kendo
    //azienda_selezionata = $('#ddl_Azienda').val();

    azienda_selezionata = ddl_Azienda.value();

    
    if (azienda_selezionata != "") {
        ajaxAgronica(gisSmartIndirizzoHttp + "/CaricaCentroAziendaleBS",
        JSON.stringify({ "Piva": azienda_selezionata }),        
        function (risposta) {            

            var ddl_Centro_DataSource = JSON.parse(risposta.RispostaStringa);

            var dataSource = new kendo.data.DataSource({
                data: ddl_Centro_DataSource
            });

            ddl_Centro.setDataSource(dataSource);
            ddl_Centro.refresh();                        

        }, null);

    }

    

}



//per chiamata Standard Ajax
function CaricaSpecieDaCentroBS() {

    //var centro_selezionato = $('#ddl_Centro').val();
    var centro_selezionato = ddl_Centro.value();
    var Piva = centro_selezionato.split("|")[0];
    var Sa_Cod = centro_selezionato.split("|")[1];

    var rval = [];

    if (Sa_Cod !== undefined && Sa_Cod != "") {
        ajaxAgronica(gisSmartIndirizzoHttp + "/CaricaSpecieDaCentroBS", JSON.stringify({ "Piva": Piva, "Sa_Cod": Sa_Cod, "isSementi": +isSementi }),
        function (risposta) {
            //$('#ddl_Specie').html("");
            //$('#ddl_Specie').html(risposta.RispostaStringa);
            //$('#ddl_Specie').selectpicker('refresh');

            rval = JSON.parse(risposta.RispostaStringa);

            //CaricaImpiantiEsistentiBS();
        }, null);
    }

    return rval;

}

/******************* Carica Specie ************************/
function CaricaSpecieDaCentro() {
    var centro_selezionato = $('#cmdCentro').val();
    var Piva = centro_selezionato.split("|")[0];
    var Sa_Cod = centro_selezionato.split("|")[1];
    if (Sa_Cod != "") {
        $.ajax({
            type: "POST",
            url: gisSmartIndirizzoHttp + "/CaricaSpecieDaCentro",
            data: "{Piva: '" + Piva + "',Sa_Cod: '" + Sa_Cod + "', isSementi: '" + isSementi + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {
                    $('#cmdSpecie').html(msg.d);
                    CaricaImpiantiEsistenti();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}

/******************* Carica CaricaImpiantiEsistenti ************************/
function CaricaImpiantiEsistenti() {
    var centro_selezionato = $('#cmdCentro').val();
    var Piva = centro_selezionato.split("|")[0];
    var Sa_Cod = centro_selezionato.split("|")[1];
    var Veg_Cod = $('#cmdSpecie').val();

    if (Veg_Cod != "" && Veg_Cod != "-1") {
        $.ajax({
            type: "POST",
            url: gisSmartIndirizzoHttp + "/CaricaImpiantiEsistenti",
            data: "{Piva: '" + Piva + "',Sa_Cod: '" + Sa_Cod + "',Veg_Cod: '" + Veg_Cod + "', isSementi: '" + isSementi + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {
                    $('#cmdAppezzamento').html(msg.d);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}

//per chiamata Standard Ajax
function CaricaImpiantiEsistentiBS() {

    //var centro_selezionato = $('#ddl_Centro').val();
    var centro_selezionato = ddl_Centro.value();

    if (centro_selezionato == "")
        return;

    var Piva = centro_selezionato.split("|")[0];
    var Sa_Cod = centro_selezionato.split("|")[1];

    //var Veg_Cod = $('#ddl_Specie').val();
    var Veg_Cod = "0"; // ddl_Specie.value();

    ddl_Impianto_DataSource = [];

    if (Veg_Cod != "" && Veg_Cod != "-1") {

        ajaxAgronica(
        gisSmartIndirizzoHttp + "/CaricaImpiantiEsistentiBS",
        JSON.stringify({ "Piva": Piva, "Sa_Cod": Sa_Cod, "Veg_Cod": Veg_Cod, "isSementi": isSementi }),        
        function (risposta) {
            

            var ddl_Impianto_DataSource = JSON.parse(risposta.RispostaStringa);
            var dataSource = new kendo.data.DataSource({
                data: ddl_Impianto_DataSource
            });

            ddl_Impianto.setDataSource(dataSource);
            ddl_Impianto.refresh();

        }, null);
    }

}


function CambiaImpiantoBS() {

    EditTipo_Azzera();

    //Rimuovo i punti caricati ..


    ImpiantoCaricaDati();

    $("#txt_descrizione").val(
        ddl_Azienda.text() + ' ' + 
        ddl_Centro.text() + ' ' + 
        ddl_Impianto.text()
    );
}

//function CambiaImpiantoBS() {
//    $("#txt_descrizione").val(
//        $('#ddl_Azienda option:selected').text() + ' ' +
//        $('#ddl_Centro option:selected').text() + ' ' +
//        $('#ddl_Specie option:selected').text() + ' ' +
//        $('#ddl_Impianto option:selected').text()
//    );
//}


var enum_Gis_FlagGPS = {
    
    

    DisegnatoSuCartografia: { value: 0, name: "DisegnatoSuCartografia", code: 0 },
    GiasPalm: { value: 1, name: "GiasPalm", code: 1 },
    CaricatoDaSorgenteEsterna: { value: 2, name: "CaricatoDaSorgenteEsterna", code: 2 },
    MisuratoSuSmartPhone: { value: 3, name: "MisuratoSuSmartPhone", code: 3 }
}





function SalvaPoligonoEsistenteBS() {


    var hiddenPunti_Nuovo = hiddenPuntiOttieni();
    var Area = "";

    //nuova modalità, salvataggio delegato ad altri oggetti/funzioni
    if (GisSmartDestinazioneChiaveAlbero !== "") {

        GisSmartDestinazioneHiddenPunti = hiddenPunti_Nuovo;
        
        if (GisSmartDestinazioneHiddenPuntiOnAfterUpdate !== undefined) {

            //al solo successo della chiamata ripulisco i punti...
            var DeferredExe = $.Deferred();
            $.when(DeferredExe).done(function () {
                SalvaPoligonoEsistenteBsClear();
            });

            $.when(DeferredExe).fail(function () {
                alert("Si sono verificati errori durante l'acquisizione del punto.")
            });

            //chiamo il salvataggio..
            GisSmartDestinazioneHiddenPuntiOnAfterUpdate(DeferredExe);            

        } else {
            alert("Attenzione: Funzione di salvataggio personalizzata non impostata .. !!");
        }
        
        return;
    }

    //modalità precedente, salvataggio con chiamata a web service

    var xTestChiaveAlbero = ddl_Impianto.value();   
    
    if (xTestChiaveAlbero == null) { xTestChiaveAlbero = "" }
    if (xTestChiaveAlbero == undefined) { xTestChiaveAlbero = "" }
    
    if (xTestChiaveAlbero == "") {
        alertErr("Selezionare un impianto.", "DIV_Messaggi");
        return;
    }
    
    
    var ChiaveAlbero = ddl_Impianto.value().toString().replace(/\\/g, '\\\\');
    
    if (ChiaveAlbero != "") {

        var tipoOperazione;
        if (entitaCod == 0)
            tipoOperazione = "/SalvaNuovoElementoGraficoDaChiaveAlberoBS";
        else
            tipoOperazione = "/ModificaElementoGraficoDaChiaveAlberoBS";

        ajaxAgronica(gisSmartIndirizzoHttp + tipoOperazione, JSON.stringify({ "ChiaveAlbero": ChiaveAlbero, "hiddenPunti_Nuovo": hiddenPunti_Nuovo, "Area": Area, "flag_gps": flagGps, "entitaCod": entitaCod }),
        function (risposta) {
            
            if (risposta.RispostaOK) {

                SalvaPoligonoEsistenteBsClear();

                //ricarico i dati per l'impianto selezionato.
                ImpiantoCaricaDati();

            }             

            alertOk(risposta.RispostaStringa);            

        }, null);
    
    }
}


function hiddenPuntiOttieni() {

    var hiddenPunti_Nuovo = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;

    //i = parseInt($('#hidden_ID_poligono_x_salvataggio').val()) + 1;
    //i = parseInt($("#ddl_recupera option:selected").val());
    i = parseInt(ddl_Recupera.value());

    var k = mySplit[i - 1];
    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            hiddenPunti_Nuovo = hiddenPunti_Nuovo + ",";
        hiddenPunti_Nuovo = hiddenPunti_Nuovo + "(" + listaP[j * 2] + ", " + listaP[j * 2 + 1] + ")";
    }
    return hiddenPunti_Nuovo;
}

function SalvaPoligonoEsistenteBsClear() {
    var i = 0;
    //i = parseInt($("#ddl_recupera option:selected").val());
    i = parseInt(ddl_Recupera.value());
    $('#Cancellapoligono_' + (i - 1) ).click();
            
    statoDisegnoCorrente = statoDisegno.Nessuno;
}

function SalvaPoligonoEsistente2() { 
    
    if (arrayPoligoni == undefined)
        return;

    if (arrayPoligoni == null)
        return;

    if (arrayPoligoni == '')
        return;

    var hiddenPunti_Nuovo = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;

    //i = parseInt($('#hidden_ID_poligono_x_salvataggio').val()) + 1;
    //i = parseInt($("#ddl_recupera option:selected").val());
    i = parseInt( ddl_Recupera.value() );

    var k = mySplit[i - 1];
    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            hiddenPunti_Nuovo = hiddenPunti_Nuovo + ",";
        hiddenPunti_Nuovo = hiddenPunti_Nuovo + "(" + listaP[j * 2] + ", " + listaP[j * 2 + 1] + ")";
    }
    
    $('#hiddenPunti_Nuovo').val(hiddenPunti_Nuovo);

}

function SalvaPoligonoEsistente() {
    var ChiaveAlbero = $('#cmdAppezzamento').val().toString().replace(/\\/g, '\\\\');

    var hiddenPunti_Nuovo = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;
    i = parseInt($('#hidden_ID_poligono_x_salvataggio').val()) + 1;
    var k = mySplit[i - 1];
    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            hiddenPunti_Nuovo = hiddenPunti_Nuovo + ",";
        hiddenPunti_Nuovo = hiddenPunti_Nuovo + "(" + listaP[j * 2] + ", " + listaP[j * 2 + 1] + ")";
    }
    var Area = "";
    if (ChiaveAlbero != "") {
        $.ajax({
            type: "POST",
            url: gisSmartIndirizzoHttp + "/SalvaNuovoElementoGraficoDaChiaveAlbero",
            data: "{ChiaveAlbero: '" + ChiaveAlbero + "', hiddenPunti_Nuovo: '" + hiddenPunti_Nuovo + "', Area: '" + Area + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {

                if (msg.d == 'SessioneScaduta') {
                    $('#dialogSessioneScaduta').dialog("open");
                }
                else {
                    alert(msg.d);
                    $('#Cancellapoligono_' + $('#hidden_ID_poligono_x_salvataggio').val()).click();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }
}



/******************* Carica Disciplinare ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaDisciplinare() {

    var Veg_Cod = $('#pop_up_specie').val();
    $('#WaitFrame').show();
    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaDisciplinare",
        data: "{ Veg_Cod: '" + Veg_Cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            r_ok();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $('#pop_up_disciplinare').html(msg.d);

            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


function CaricaDateDefaultDaVegCod(dataInizio, dataFine) {
    var veg_cod = $('#pop_up_specie').val();
    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaDateDefaultDaVegCod",
        data: "{Veg_Cod :'" + veg_cod + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                if (msg.d == '-1') { }
                else {

                    var DataInizioFine = msg.d.split('|');

                    $(dataInizio).val(DataInizioFine[0]);
                    $(dataFine).val(DataInizioFine[1]);
                    if ($(dataFine).val() == "") {
                        $(dataFine).removeAttr("disabled");
                    }
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}


/******************* Carica Specie ************************/
/* funzione per il caricamento tramite ajax delle specie  */
function CaricaSpecie(combo_specie, veg_cod) {
    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/CaricaSpecie",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {
                $(combo_specie).html(msg.d);
                if (veg_cod != '') {
                    $(combo_specie).val(veg_cod)
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}



function HiddenPuntiNuovo_GetFromArrayPoligoni() {
    
    var hiddenPunti_Nuovo = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;
    
    var sHidd = $('#hidden_ID_poligono_x_salvataggio').val();
    //var sHidd = parseInt(ddl_Recupera.value());
    if (sHidd == '')
        sHidd = '0';

    i = parseInt(sHidd) + 1;
    var k = mySplit[i - 1];
    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            hiddenPunti_Nuovo = hiddenPunti_Nuovo + ",";

        hiddenPunti_Nuovo = hiddenPunti_Nuovo + "{\"lat\": " + listaP[j * 2] + ", \"lng\": " + listaP[j * 2 + 1] + "}";
    }

    $.logThis("hiddenPunti_Nuovo :" + hiddenPunti_Nuovo);
    return "[" + hiddenPunti_Nuovo + "]";
}

/* funzione per la creazione di un nuvo impianto  */
function InviaDatiNuovoImpianto() {
    $('#WaitFrame').show();

    var hiddenPunti_Nuovo = "";
    var mySplit = arrayPoligoni.split("/");
    var i = 0;
    i = parseInt($('#hidden_ID_poligono_x_salvataggio').val()) + 1;
    var k = mySplit[i - 1];
    var listaP = k.split("|");
    for (var j = 0; j < listaP.length / 2; j++) {
        if (j > 0)
            hiddenPunti_Nuovo = hiddenPunti_Nuovo + ",";
        hiddenPunti_Nuovo = hiddenPunti_Nuovo + "(" + listaP[j * 2] + ", " + listaP[j * 2 + 1] + ")";
    }

    $.logThis("hiddenPunti_Nuovo :" + hiddenPunti_Nuovo);

    var oggetto = {
        "piva": $('#pop_up_impianto_azienda').val(),
        "rag_soc": $('#pop_up_impianto_azienda').find(":selected").text(),
        "sa_cod": $('#pop_up_centro').val(),
        "veg_cod": $('#pop_up_specie').val(),
        "sup_imp": $('#pop_up_sup_app').val(),
        "data_inizio": $('#pop_up_data_inizio').val(),
        "data_fine": $('#pop_up_data_fine').val(),
        "varieta": $('#pop_up_varieta').val(),
        "finalita": $('#pop_up_finalita').val(),
        "disciplinare": $('#pop_up_disciplinare').val(),
        "tipologia": $('#pop_up_tipologia').val(),
        "nome": $('#pop_up_nome').val(),
        "lotto": $('#pop_up_lotto').val(),
        "data_semina": $('#pop_up_d_semina').val(),
        "data_raccolta": $('#pop_up_d_raccolta').val(),
        "hiddenPunti_Nuovo": hiddenPunti_Nuovo,
        "layer_cod": 19,
        "elementografico_des": ""
    };

    if (oggetto.finalita == null) {
        oggetto.finalita = "";
    }
    if (oggetto.disciplinare == null) {
        oggetto.disciplinare = "";
    }
    if (oggetto.tipologia == null) {
        oggetto.tipologia = "";
    }


    //se sono tutte valorizzate allora posso procedere
    var blocca = false;

    if (oggetto.piva == "" || oggetto.sa_cod == "" || oggetto.veg_cod == "" || oggetto.sup_imp == "") {
        blocca = true;
    }

    if (isSementi == true && (oggetto.data_inizio == "" || oggetto.data_fine == "" || oggetto.tipologia == "")) {
        blocca = true;
    }

    // controllo che nono siano nulle
    if (oggetto.piva == null || oggetto.sa_cod == null || oggetto.veg_cod == null || oggetto.sup_imp == null) {
        alert("completare i null");
        return false;
    }

    if (blocca) {
        alert("completare i campi obbligatori");
        $('#WaitFrame').hide();
        return false;
    }

    $.ajax({
        type: "POST",
        url: gisSmartIndirizzoHttp + "/SalvaNuovoImpianto",
        data: "{ piva: '" + oggetto.piva + "', sa_cod: '" + oggetto.sa_cod + "', veg_cod: '" + oggetto.veg_cod + "', sup_imp: '" + oggetto.sup_imp + "', piva: '" + oggetto.piva + "', data_inizio: '" + oggetto.data_inizio + "', data_fine: '" + oggetto.data_fine + "', varieta: '" + oggetto.varieta + "', finalita: '" + oggetto.finalita + "', disciplinare: '" + oggetto.disciplinare + "', tipologia: '" + oggetto.tipologia + "', nome: '" + oggetto.nome + "', lotto: '" + oggetto.lotto + "', data_semina: '" + oggetto.data_semina + "', data_raccolta: '" + oggetto.data_raccolta + "', hiddenPunti_Nuovo: '" + oggetto.hiddenPunti_Nuovo + "', layer_Cod: '" + oggetto.layer_cod + "', elementografico_des: '" + oggetto.elementografico_des + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#responseInterferenze').width('0px');
            $('#responseInterferenze').html('');
            $("#pop_up_impianto").dialog("close");
            selectedShape.setMap(null);

            $('#WaitFrame').hide();
            if (msg.d == 'SessioneScaduta') {
                $('#dialogSessioneScaduta').dialog("open");
            }
            else {

                ImpostaAziendaAppenaSalvataSuCombo(oggetto.piva, oggetto.rag_soc);
                AggiornaTutto();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
function sleep(ms) {
    var dt = new Date();
    dt.setTime(dt.getTime() + ms);
    while (new Date().getTime() < dt.getTime());
}

function sleep2 (time) {
  return new Promise((resolve) => setTimeout(resolve, time));
}

