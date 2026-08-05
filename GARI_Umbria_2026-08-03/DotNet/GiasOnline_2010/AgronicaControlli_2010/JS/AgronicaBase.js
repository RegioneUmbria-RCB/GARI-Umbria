//AgronicaBase.js

//indica il tipo di debug sulla console del browser
var Enum_debugMode = {
    Off: { value: 0, name: "Off", code: 0 },
    Soft: { value: 1, name: "Soft", code: 1 },
    Verbose: { value: 2, name: "Verbose", code: 2 }
}

//indica il tipo di messaggi in comunicazione fra le pagine
var Enum_comunicazioneTipoMessaggio = {
    CambioAzienda: { value: 1, name: "CambioAzienda", code: 1 },
    AggiornaListaOperazioni: { value: 2, name: "AggiornaListaOperazioni", code: 2 },
    SelezionaOperazione: { value: 3, name: "SelezionaOperazione", code: 3 }
}

//equivalente dei tipi enumerativi
var Enum_TipoOperazioneDB = {
   Lettura : { value: 0 , name: "Lettura", code: 0 },
   Scrittura : { value: 1 , name: "Scrittura", code: 1 },
   Modifica : { value: 2 , name: "Modifica", code: 2 },
   Cancellazione : { value: 3, name: "Cancellazione", code: 3 },
   Trasferimento : { value: 4, name: "Trasferimento", code: 4 },
   Copia : { value: 10, name: "Copia", code: 10},
}

// Gestione refresh token
var refreshTokenInProgress = false;
var requestsQueue = [];

/**
* Gestione del Logout
* 
*/
function LogoutGestione() {

    // Variabili globali valorizzate in AgronicaBase.vb
    let paginaLoginRedirect = AgronicaBasePaginaLoginRedirectDaSessione;

    if (paginaLoginRedirect !== undefined && paginaLoginRedirect !== "") {

        if (paginaLoginRedirect.indexOf("?pivasuperuser") === -1) {
            paginaLoginRedirect += "?" + AgronicaBasePaginaLoginRedirectDaSessioneQueryStringPivaSuperUser;
        }

        //se ho un url, allora redirigo!
        window.location = paginaLoginRedirect;

    } else {
        window.close();
    }

}

/**
* Gestione del Logout
* 
*/
function SessioneScadutaGestione(divMessaggi) {

    // Variabili globali valorizzate in AgronicaBase.vb
    let paginaLoginRedirect = AgronicaBasePaginaLoginRedirectDaSessione;

    if (paginaLoginRedirect !== undefined && paginaLoginRedirect !== "") {

        paginaLoginRedirect = SessioneScadutaAccodaQueryString(paginaLoginRedirect);

        //se ho un url, allora redirigo!
        window.location = paginaLoginRedirect;

    } else {
        MessaggioErrore_Bootstrap("Sessione scaduta, si prega di eseguire nuovamente il login.", divMessaggi);
    }

}

function SessioneScadutaAccodaQueryString(s) {

    if (SessioneScadutaGestioneCerca(s, "?pivasuperuser")) {
        return s + "&" + AgronicaBasePaginaLoginRedirectDaSessioneQueryStringSessione;
    } else {
        return s + "?" + AgronicaBasePaginaLoginRedirectDaSessioneQueryStringPivaSuperUser + "&" + AgronicaBasePaginaLoginRedirectDaSessioneQueryStringSessione;
    }

}

function SessioneScadutaGestioneCerca(s, cosa) {
    return s.indexOf(cosa) !== -1;
}



//#region "per sessione"
var AgronicaBaseSec = 4;
var AgronicaBaseCdID;

function AgronicaBaseImpostaRedirect(divMessaggi) {

    var m = "... (" + AgronicaBaseSec.toString() + ")";
    $('#AgronicaBaseTimeOutSessione').html(m);

    if (AgronicaBaseSec == 0) {

        window.clearInterval(AgronicaBaseCdID);
        SessioneScadutaGestione(divMessaggi);
        
    }

    --AgronicaBaseSec;

}

function AgronicaBaseImpostaRedirectStart(divMessaggi) {
    AgronicaBaseCdID = window.setInterval("AgronicaBaseImpostaRedirect('" + divMessaggi + "');", 1000);
}


/**
* Wrapper su Chiamata Ajax
* esempio
* ajaxAgronica("test.aspx/cricaAziende_js", JSON.stringify({ parametro_1: "sdsadsa" }),
* function (risposta) {
* console.log(risposta);
* alert(risposta);
* }, null);
*
* @param {string} url Indirizzo Url da chiamare
* @param {string} parametri parametri in formato json
* @param {function} successo funzione script che sarà chiamata se non ci sono errori
* @param {function} errore funzione script che sarà chiamata in caso di errori
* @param {string} accessToken Token per autenticazione OAuth2 (sarà aggiunto agli header)
* @param {boolean|any} gestioneWaitFrame se false è compito del chiamante gestire waitframe (default true)
* @param {any} deferred 
* @param {boolean|any} compressione .
*/
var ajaxAgronica = function (url, parametri, successo, errore, accessToken, gestioneWaitFrame, deferred, compressione) {
    ajaxAgronicaGenerica(url, parametri, "DIV_Messaggi", true, false, successo, errore, accessToken, gestioneWaitFrame, deferred, compressione);
}

var ajaxAgronicaSync = function (url, parametri, async, successo, errore, accessToken, gestioneWaitFrame, deferred, compressione) {
    ajaxAgronicaGenerica(url, parametri, "DIV_Messaggi", false, false, successo, errore, accessToken, gestioneWaitFrame, deferred, compressione);
}


/**
  * Gestione wait
  * @param {boolean} showHide true = mostra
  * @returns {} 
  */
function ajaxAgronicaWaitFrame(showHide) {

    //try catch perchè potrebbe essere chiamato da GIS o altrove dove non c'è il waitframe

    var eseguito = false;

    try {

        if (WaitFrame.show !== undefined) {
            if (showHide) {
                WaitFrame.show();
            } else {
                WaitFrame.hide();
            }
            eseguito = true;
        }

        //GIS
        if (window.interfaccia !== undefined && !eseguito) {
            interfaccia.loading(showHide);
        }

    } catch (e) {
        console.error("e(waitframe.show):=" + e.message);
    }
}

//Overload se si vuole passare anche il div di dove scrivere i messaggi: per le finestre modali
var ajaxAgronicaGenerica = function (url, parametri, divMessaggiClientId, asyncCall, cache, successo, errore, accessToken, gestioneWaitFrame, deferred, compressione) {

    ////let convertionService = null;
    ////let tzCustomeHeader = null;

    ////if (String(typeof (AGRO_JS_TIMEZONE)).toLowerCase() === 'object')
    ////    convertionService = new AGRO_JS_TIMEZONE.Data_Conversion_Service();

    // CLIENT-TO_SERVER -> Trasformazione valori parametri richiesta data ora dal fuso orario locale a quello del server
    ////if (convertionService !== null) {
    ////    convertionService.Init();
    ////    tzCustomeHeader = convertionService.CLIENT_TIME_ZONE_INFO();

    ////    parametri = convertionService.Manage_TZ_From_Client_Data(parametri);
    ////}
        

    $.ajax({
        type: "POST",
        url: url,
        data: parametri,
        dataType: "json",
        async: asyncCall,
        timeout: 0,
        beforeSend: function (xhr) {

            if (accessToken !== null && accessToken !== undefined)
                xhr.setRequestHeader("Authorization", "BEARER " + accessToken);

            if (compressione === null || compressione === undefined)
                compressione = true;

            if (typeof pako === 'undefined' ||  pako === undefined || pako === null) {
                compressione = false;
                console.warn('pako non caricato. URL richiesto: ' + url);
            }

            xhr.setRequestHeader("x-compressione", compressione);

            //if (tzCustomeHeader !== null)
            //    xhr.setRequestHeader("x-timezone", JSON.stringify(tzCustomeHeader));

            if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                if (gestioneWaitFrame == true) {
                    ajaxAgronicaWaitFrame(true);
                }
            } else {
                ajaxAgronicaWaitFrame(true);
            }


            console.log("ajax call, async = " + asyncCall);

        },
        cache: cache,
        contentType: "application/json; charset=utf-8",
        error: function (xhr, textStatus, errorThrown) {            

            //if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
            //    if (gestioneWaitFrame == true) {
            //        ajaxAgronicaWaitFrame(false);
            //    }
            //} else {
            //    ajaxAgronicaWaitFrame(false);
            //}
            ajaxAgronicaWaitFrame(false);

            var msgError = textStatus;
            if (msgError === "error" && errorThrown !== undefined && errorThrown !== null && errorThrown !== "") {
                //Visto che è generico, uso errorThrown
                msgError = errorThrown;
            }

            if (errore != null) {
                if (xhr !== undefined && xhr.status === 400 && xhr.responseJSON !== undefined && xhr.responseJSON.d !== undefined) {
                    errore(xhr.responseJSON.d);
                } else {
                    errore(msgError);
                }                
                return;
            }

            try {

                if (window.MessaggioErrore_Bootstrap !== undefined) {
                    MessaggioErrore_Bootstrap("Si e' verificato un problema prima della chiamata in Ajax: " + msgError, divMessaggiClientId);
                } else {
                    alert("Si e' verificato un problema prima della chiamata in Ajax: " + msgError);
                }

                if (deferred != undefined) {
                    deferred.reject();
                }

            } catch (e) {
                console.error("e(ajaxError):=" + e.message);
            }

        },
        success: function (msg) {

            if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                if (gestioneWaitFrame == true) {
                    ajaxAgronicaWaitFrame(false);
                }
            } else {
                ajaxAgronicaWaitFrame(false);
            }

            //try catch perchè potrebbe essere chiamato da GIS o altrove dove non c'è il waitframe
            try {

                //$.logThis(msg.d);

                if (!msg.d.Sessione) {

                    if (window.MessaggioErrore_Bootstrap !== undefined) {
                        ajaxAgronicaWaitFrame(false);
                        MessaggioErrore_Bootstrap("Sessione scaduta. <a href='javascript:SessioneScadutaGestione()'>Fare click QUI se non si viene indirizzati automaticamente</a> in <span id='AgronicaBaseTimeOutSessione'></span>", divMessaggiClientId);
                        AgronicaBaseImpostaRedirectStart(divMessaggiClientId);
                    } else {
                        alert("Sessione scaduta. ");
                    }

                }
                else {
                    if (msg.d.Compressa) {
                        if (msg.d.RispostaCompressa !== undefined && msg.d.RispostaCompressa !== null && msg.d.RispostaCompressa.length > 0) {
                            msg.d.RispostaStringa = pako.inflate(msg.d.RispostaCompressa, { to: 'string' });
                            msg.d.RispostaCompressa = null;
                        }
                        else
                            msg.d.RispostaStringa = "";
                    }

                    if (msg.d.RispostaOK) {

                        // SERVER-TO-CLIENT -> Trasformazione valori data ora dal fuso orario del server a quello del client
                        //if (convertionService !== null) 
                        //    msg.d.RispostaStringa = convertionService.Manage_TZ_From_Server_Data(msg.d.RispostaStringa);
                        successo(msg.d);
                    }
                    else {
                        if (errore != null) {
                            errore(msg.d);
                        } else if (window.MessaggioErrore_Bootstrap !== undefined) {
                            ajaxAgronicaWaitFrame(false);
                            MessaggioErrore_Bootstrap("Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore, divMessaggiClientId);
                        } else {
                            ajaxAgronicaWaitFrame(false);
                            alert("Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore);
                        }
                    }

                }

            } catch (e) {
                console.error("e(ajaxSuccess):=" + e.message);
            }


        }
    });


}
/// Start AjaxAgronicageneriche per API CoreSTD

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

var ajaxAgronicaApiCoreStdGetSync = function (url, parametri, successo, errore, gestioneWaitFrame, deferred) {
    return ajaxAgronicaApiCoreStdGenerica("GET", url, parametri, "DIV_Messaggi", false, false, successo, errore, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiCoreStdGetAsync = function (url, parametri, successo, errore, gestioneWaitFrame, deferred) {
    return ajaxAgronicaApiCoreStdGenerica("GET", url, parametri, "DIV_Messaggi", true, false, successo, errore, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiCoreStdPostSync = function (url, parametri, successo, errore, gestioneWaitFrame, deferred) {
    return ajaxAgronicaApiCoreStdGenerica("POST", url, parametri, "DIV_Messaggi", false, false, successo, errore, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiCoreStdPostAsync = function (url, parametri, successo, errore, gestioneWaitFrame, deferred) {
    return ajaxAgronicaApiCoreStdGenerica("POST", url, parametri, "DIV_Messaggi", true, false, successo, errore, gestioneWaitFrame, deferred);
}


var ajaxAgronicaApiCoreStdGenerica = function (type, url, parametri, divMessaggiClientId, asyncCall, cache, successo, errore, gestioneWaitFrame, deferred) {

    ////let convertionService = null;
    ////let tzCustomeHeader = null;

    ////if (String(typeof (AGRO_JS_TIMEZONE)).toLowerCase() === 'object')
    ////    convertionService = new AGRO_JS_TIMEZONE.Data_Conversion_Service();

    // CLIENT-TO_SERVER -> Trasformazione valori parametri richiesta data ora dal fuso orario locale a quello del server
    ////if (convertionService !== null) {
    ////    convertionService.Init();
    ////    tzCustomeHeader = convertionService.CLIENT_TIME_ZONE_INFO();

    ////    parametri = convertionService.Manage_TZ_From_Client_Data(parametri);
    ////}
        
    let beforeSendFunction = function (xhr) {

        //xhr.setRequestHeader("Authorization", getCookie("Authorization"));

        //if (tzCustomeHeader !== null)
        //    xhr.setRequestHeader("x-timezone", JSON.stringify(tzCustomeHeader));

        if (WaitFrame !== undefined) {
            if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                if (gestioneWaitFrame == true) {
                    ajaxAgronicaWaitFrame(true);
                }
            } else {
                ajaxAgronicaWaitFrame(true);
            }
        }

        console.log("ajax call, async = " + asyncCall);

    }

    let successFunction = function (msg) {

        if (WaitFrame !== undefined) {
            if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                if (gestioneWaitFrame == true) {
                    ajaxAgronicaWaitFrame(false);
                }
            } else {
                ajaxAgronicaWaitFrame(false);
            }
        }

        //try catch perché potrebbe essere chiamato da GIS o altrove dove non c'è il waitframe
        try {

            //$.logThis(msg.d);

            if (msg !== undefined && msg.d !== undefined && !msg.d.Sessione) {

                if (window.MessaggioErrore_Bootstrap !== undefined) {
                    if (WaitFrame !== undefined) {
                        ajaxAgronicaWaitFrame(false);
                    }
                    MessaggioErrore_Bootstrap("Sessione scaduta. <a href='javascript:SessioneScadutaGestione()'>Fare click QUI se non si viene indirizzati automaticamente</a> in <span id='AgronicaBaseTimeOutSessione'></span>", divMessaggiClientId);
                    AgronicaBaseImpostaRedirectStart(divMessaggiClientId);
                } else {
                    alert("Sessione scaduta. ");
                }

            }
            else {

                if (msg !== undefined && msg.d !== undefined) {
                    if (msg.d.RispostaOK) {
                        successo(msg.d);
                    } else {
                        if (errore != null) {
                            errore(msg.d);
                        } else if (window.MessaggioErrore_Bootstrap !== undefined) {
                            ajaxAgronicaWaitFrame(false);
                            MessaggioErrore_Bootstrap(
                                "Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore,
                                divMessaggiClientId);
                        } else {
                            ajaxAgronicaWaitFrame(false);
                            alert("Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore);
                        }
                    }
                } else {

                    ////if (msg !== undefined &&  msg.RispostaStringa !== undefined) {
                    ////    // SERVER-TO-CLIENT -> Trasformazione valori data ora dal fuso orario del server a quello del client
                    ////    if (convertionService !== null)
                    ////        msg.RispostaStringa = convertionService.Manage_TZ_From_Server_Data(msg.RispostaStringa);
                    ////}
                    successo(msg);
                }
            }

        } catch (e) {
            console.error("e(ajaxSuccess):=" + e.message);
        }
    }
    

    let ajaxSettings = {
        type: type,
        url: url,
        data: parametri,
        dataType: "json",
        async: asyncCall,
        timeout: 0,
        xhrFields: {
            withCredentials: true
        },
        beforeSend: beforeSendFunction,
        cache: cache,
        contentType: "application/json; charset=utf-8",
        error: function(xhr, textStatus, errorThrown) {
            if (xhr.status === 401) {
                if (!refreshTokenInProgress) {
                    refreshTokenInProgress = true;

                    refreshToken().done(function (newToken) {
                        processQueue(newToken);
                        refreshTokenInProgress = false;
                    }).fail(function () {
                        refreshTokenInProgress = false;
                    });
                }

                var deferred = $.Deferred();
                requestsQueue.push({
                    settings: ajaxSettings,
                    deferred: deferred
                });

                return deferred.promise();
            } else {
                errorFunctionCoreAPI(xhr, textStatus, errorThrown);
            }
        },
        success: successFunction
    }

    return $.ajax(ajaxSettings);
}

var errorFunctionCoreAPI = function (xhr, textStatus, errorThrown) {
    if (textStatus == "abort") {
        //Visto che gl abort sono volontari, non mostro quel errore
        return;
    }


    var msgError = textStatus;
    if (msgError === "error" && errorThrown !== undefined && errorThrown !== null && errorThrown !== "") {
        //Visto che è generico, uso errorThrown
        msgError = errorThrown;
    }

    if (WaitFrame !== undefined) {
        ajaxAgronicaWaitFrame(false);
    }

    if (errore != null) {
        errore(msgError, xhr.status);
        return;
    }

    try {

        if (window.MessaggioErrore_Bootstrap !== undefined) {
            MessaggioErrore_Bootstrap("Si e' verificato un problema prima della chiamata in Ajax: " + msgError, divMessaggiClientId);
        } else {
            alert("Si e' verificato un problema prima della chiamata in Ajax: " + msgError);
        }

        if (deferred != undefined) {
            deferred.reject();
        }

    } catch (e) {
        console.error("e(ajaxError):=" + e.message);
    }

}


function processQueue(newToken) {
    while (requestsQueue.length > 0) {
        var request = requestsQueue.shift();
        $.ajax(request.settings).then(request.deferred.resolve, request.deferred.reject);
    }
}
function refreshToken() {
    var deferred = $.Deferred();

    $.ajax({
        url: pathCoreAPI + "/Login/RefreshToken",
        method: "POST",
        xhrFields: {
            withCredentials: true
        }
    }).done(function (response) {
        deferred.resolve(response.newToken);
    }).fail(function (xhr, textStatus, errorThrown) {
        errorFunctionCoreAPI(xhr, textStatus, errorThrown);
        deferred.reject(xhr, textStatus, errorThrown);
    });

    return deferred.promise();
}

/// End AjaxAgronicageneriche per API CoreSTD


/// Start AjaxAgronicageneriche

var ajaxAgronicaApiGetSync = function (url, parametri, successo, errore, accessToken, gestioneWaitFrame, deferred) {
    ajaxAgronicaApiGenerica("GET", url, parametri, "DIV_Messaggi", false, false, successo, errore, accessToken, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiGetAsync = function (url, parametri, successo, errore, accessToken, gestioneWaitFrame, deferred) {
    ajaxAgronicaApiGenerica("GET", url, parametri, "DIV_Messaggi", true, false, successo, errore, accessToken, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiPostSync = function (url, parametri, successo, errore, accessToken, gestioneWaitFrame, deferred) {
    ajaxAgronicaApiGenerica("POST", url, parametri, "DIV_Messaggi", false, false, successo, errore, accessToken, gestioneWaitFrame, deferred);
}

var ajaxAgronicaApiPostAsync = function (url, parametri, successo, errore, accessToken, gestioneWaitFrame, deferred) {
    ajaxAgronicaApiGenerica("POST", url, parametri, "DIV_Messaggi", true, false, successo, errore, accessToken, gestioneWaitFrame, deferred);
}


var ajaxAgronicaApiGenerica = function (type, url, parametri, divMessaggiClientId, asyncCall, cache, successo, errore, accessToken, gestioneWaitFrame, deferred) {
    $.ajax({
        type: type,
        url: url,
        data: parametri,
        dataType: "json",
        async: asyncCall,
        timeout: 0,
        beforeSend: function (xhr) {

            if (accessToken !== null && accessToken !== undefined) {
                xhr.setRequestHeader("Authorization", accessToken);
            }

            if (WaitFrame !== undefined) {
                if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                    if (gestioneWaitFrame == true) {
                        ajaxAgronicaWaitFrame(true);
                    }
                } else {
                    ajaxAgronicaWaitFrame(true);
                }
            }

            console.log("ajax call, async = " + asyncCall);

        },
        cache: cache,
        contentType: "application/json; charset=utf-8",
        error: function (xhr, textStatus, errorThrown) {

            var msgError = textStatus;
            if (msgError === "error" && errorThrown !== undefined && errorThrown !== null && errorThrown !== "") {
                //Visto che è generico, uso errorThrown
                msgError = errorThrown;
            }

            if (WaitFrame !== undefined) {
                ajaxAgronicaWaitFrame(false);
            }

            if (errore != null) {
                errore(msgError, xhr.status);
                return;
            }

            try {

                if (window.MessaggioErrore_Bootstrap !== undefined) {
                    MessaggioErrore_Bootstrap("Si e' verificato un problema prima della chiamata in Ajax: " + msgError, divMessaggiClientId);
                } else {
                    alert("Si e' verificato un problema prima della chiamata in Ajax: " + msgError);
                }

                if (deferred != undefined) {
                    deferred.reject();
                }

            } catch (e) {
                console.error("e(ajaxError):=" + e.message);
            }

        },
        success: function (msg) {

            if (WaitFrame !== undefined) {
                if (gestioneWaitFrame !== null && gestioneWaitFrame !== undefined) {
                    if (gestioneWaitFrame == true) {
                        ajaxAgronicaWaitFrame(false);
                    }
                } else {
                    ajaxAgronicaWaitFrame(false);
                }
            }

            //try catch perché potrebbe essere chiamato da GIS o altrove dove non c'è il waitframe
            try {

                //$.logThis(msg.d);

                if (msg !== undefined && msg.d !== undefined && !msg.d.Sessione) {

                    if (window.MessaggioErrore_Bootstrap !== undefined) {
                        if (WaitFrame !== undefined) {
                            ajaxAgronicaWaitFrame(false);
                        }
                        MessaggioErrore_Bootstrap("Sessione scaduta. <a href='javascript:SessioneScadutaGestione()'>Fare click QUI se non si viene indirizzati automaticamente</a> in <span id='AgronicaBaseTimeOutSessione'></span>", divMessaggiClientId);
                        AgronicaBaseImpostaRedirectStart(divMessaggiClientId);
                    } else {
                        alert("Sessione scaduta. ");
                    }

                }
                else {

                    if (msg !== undefined && msg.d !== undefined) {
                        if (msg.d.RispostaOK) {
                            successo(msg.d);
                        } else {
                            if (errore != null) {
                                errore(msg.d);
                            } else if (window.MessaggioErrore_Bootstrap !== undefined) {
                                ajaxAgronicaWaitFrame(false);
                                MessaggioErrore_Bootstrap(
                                    "Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore,
                                    divMessaggiClientId);
                            } else {
                                ajaxAgronicaWaitFrame(false);
                                alert("Si e' verificato un problema lato server (Errore 500): " + msg.d.Errore);
                            }
                        }
                    } else {
                        successo(msg);
                    }
                }

            } catch (e) {
                console.error("e(ajaxSuccess):=" + e.message);
            }


        }
        //error: function (xhr, ajaxOptions, thrownError) {
        //    alert(xhr.status);
        //    alert(thrownError);
        //},
        //success: function (msg) {
        //    $(Controls.PIVA).val(msg.d);
        //    $(Controls.PIVA).trigger("keyup");
        //}
    });
}

/// End AjaxAgronicageneriche




//*******************************************************
//*** SALVATAGGIO / RIPRISTINO valori dentro a div/ul ***
//*******************************************************

// da gestire option

function SalvaParametriDiv(div, _default) {

    var p = {};

    var nomeChiave = OttieniNomeChiave(div, _default);

    $(div).find('select').each(function () {
        var nome = $(this).attr('id');
        var value = $(this).val();
        p[nome] = value;
    });

    $(div).find('input').each(function () {
        var nome = $(this).attr('id');
        var value = $(this).val();
        if ($(this).is('[type="button"]') == false) {
            if ($(this).is('[type="checkbox"]')) {
                if ($(this).is(':checked')) {
                    p[nome] = true;
                } else {
                    p[nome] = false;
                }                 
            }
            else {
                p[nome] = value;
            }
        }
    });

    //salvo parametri div
    localStorage[nomeChiave] = JSON.stringify(p);

    return p;

}

function SetParametriDiv(div, _default) {

    var nomeChiave = OttieniNomeChiave(div, _default);

    var p = localStorage[nomeChiave];

    if (!!p) {
        p = JSON.parse(p);

        $(div).find("select").each(function () {
            var nome = $(this).attr("id");
            if (!!p[nome]) {
                $(this).val(p[nome]);
            }
        });
        $(".selectpicker").selectpicker("render");

        $(div).find("input").each(function () {
            var nome = $(this).attr("id");

            try {
                if ($(this).is('[type="checkbox"]')) {
                    if ($(this).data("kendoSwitch") !== undefined && $(this).data("kendoSwitch") !== null) {
                        $(this).data("kendoSwitch").value(p[nome]);
                        $(this).change();    //forzo il change
                    } else {
                        $(this).prop("checked", p[nome]);
                        $(this).change();    //forzo il change
                    }
                } else if ($(this).data("kendoDropDownList") !== undefined && $(this).data("kendoDropDownList") !== null) {
                    $(this).data("kendoDropDownList").value(p[nome]);
                    $(this).change();    //forzo il change
                } else {
                    $(this).val(p[nome]);
                }
            }
            catch (err) {
            }
        });
    }

    //cancello parametri div dopo il ripristino
    if (!_default)
        localStorage.removeItem(nomeChiave);

}

function SalvaParametriUl(ul, _default, piva, paginaCorrente) {

    var p = {};

    var nomeChiave = OttieniNomeChiave(ul, _default);

    $(ul).find('li').each(function () {
        var figlio = $(this)[0].firstElementChild;
        var nome = $(figlio).attr('id');
        var value = $(this).attr('class');
        p[nome] = value;

        var tabellaKendo = OttieniNomeTabella(nome);

        if (KendoGrid(tabellaKendo) !== undefined && KendoGrid(tabellaKendo).dataSource !== undefined) {

            //Memorizzo pagina corrente
            var nomePaginaTabella = OttieniNomePaginaTabella(tabellaKendo);
            var valuePaginaTabella = 0;
            if (paginaCorrente) {
                valuePaginaTabella = paginaCorrente;
            } else {
                valuePaginaTabella = KendoGrid(tabellaKendo).dataSource.page();
            };            
            p[nomePaginaTabella] = valuePaginaTabella;

            //Memorizzo dimensione pagina corrente
            var nomeDimensionePaginaTabella = OttieniNomeDimensionePaginaTabella(tabellaKendo);
            var valueDimensionePaginaTabella = KendoGrid(tabellaKendo).dataSource.pageSize();
            p[nomeDimensionePaginaTabella] = valueDimensionePaginaTabella;

            //Memorizzo data salvataggio pagina corrente            
            var nomeDataSalvataggioPaginaTabella = OttieniNomeDataSalvataggioPaginaTabella(tabellaKendo);
            var valueDataSalvataggioPaginaTabella = DataOra_DataIta(new Date());
            p[nomeDataSalvataggioPaginaTabella] = valueDataSalvataggioPaginaTabella;

            //Memorizzo piva pagina corrente
            var nomePivaPaginaTabella = OttieniNomePivaPaginaTabella(tabellaKendo);
            var valuePivaPaginaTabella = OttieniPivaCorrente(piva);
            p[nomePivaPaginaTabella] = valuePivaPaginaTabella;

        }

    });

    //salvo parametri ul
    localStorage[nomeChiave] = JSON.stringify(p);

    return p;

}

function SetParametriUl(ul, _default, piva) {

    var nomeChiave = OttieniNomeChiave(ul, _default);

    var p = localStorage[nomeChiave];

    const classeAttiva = 'active';

    if (!!p) {
        p = JSON.parse(p);

        $(ul).find('li').each(function () {
            var figlio = $(this)[0].firstElementChild;
            var nome = $(figlio).attr('id');
            var collegamento = $(figlio).attr('href');

            if (!!p[nome] && p[nome] === classeAttiva) {

                $(this)[0].classList.add(classeAttiva);

                if (collegamento) {
                    $(collegamento)[0].classList.add(classeAttiva);
                }

                $(figlio).click();

                var tabellaKendo = OttieniNomeTabella(nome);

                if (IsRiposizionaPagina(tabellaKendo, piva, p)) {

                    let nomePaginaTabella = OttieniNomePaginaTabella(tabellaKendo);
                    let nomeDimensionePaginaTabella = OttieniNomeDimensionePaginaTabella(tabellaKendo);
                    riposizionamento_pagina(tabellaKendo, p[nomePaginaTabella], p[nomeDimensionePaginaTabella]);

                }

            } else {

                $(this)[0].classList.remove(classeAttiva);

                if (collegamento) {
                    $(collegamento)[0].classList.remove(classeAttiva);
                }

            }

        });

    }

    //cancello parametri ul dopo il ripristino
    if (!_default)
        localStorage.removeItem(nomeChiave);

}

function OttieniNomeChiave(elemento, _default) {

    var nomeChiave;

    if (_default) {
        nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + elemento + "_default";
    }
    else {
        nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + elemento;
    }

    return nomeChiave;

}

function OttieniNomeTabella(nome) {

    return nome.substring(2);

}

function OttieniNomePaginaTabella(tabella) {

    return tabella.concat("_paginaCorrente")

}

function OttieniNomeDimensionePaginaTabella(tabella) {

    return tabella.concat("_dimensionePaginaCorrente")

}

function OttieniNomeDataSalvataggioPaginaTabella(tabella) {

    return tabella.concat("_dataSalvataggioPaginaCorrente")

}

function OttieniNomePivaPaginaTabella(tabella) {

    return tabella.concat("_PivaPaginaCorrente")

}

function OttieniPivaCorrente(piva) {

    var pivaCorrente = "";

    if (piva) {
        pivaCorrente = piva;
    };

    return pivaCorrente;

};

function IsRiposizionaPagina(tabellaKendo, piva, p) {

    let nomePaginaTabella = OttieniNomePaginaTabella(tabellaKendo);

    let nomeDataSalvataggioPaginaTabella = OttieniNomeDataSalvataggioPaginaTabella(tabellaKendo);
    let dataCorrente = DataOra_DataIta(new Date());

    let nomePivaPaginaTabella = OttieniNomePivaPaginaTabella(tabellaKendo);
    let pivaCorrente = OttieniPivaCorrente(piva);

    //Posso riposizionare la pagina solo se tutte le seguenti condizioni sono vere:
    //- le chiavi nomePaginaTabella, nomeDataSalvataggioPaginaTabella e nomePivaPaginaTabella sono definite
    //- la data salvata è uguale a quella corrente
    //- la piva salvata è uguale a quella corrente

    if (!!p[nomePaginaTabella] &&
        !!p[nomeDataSalvataggioPaginaTabella] &&
        !!p[nomePivaPaginaTabella] &&
        p[nomeDataSalvataggioPaginaTabella] === dataCorrente &&
        p[nomePivaPaginaTabella] === pivaCorrente) {

        return true;

    } else {

        return false;

    }

};

function ClearParametriDiv(div) {
    SetParametriDiv(div, true);
}

function ClearParametriUl(ul) {
    SetParametriUl(ul, true);
}

function CancellaSediversoDa_paginaprecedente(div, nomepagina) {
    var nomeChiave;
    nomeChiave = window.location.href.substr(window.location.href.lastIndexOf("/") + 1).split('.')[0] + "_" + div;

    if ((document.referrer.toLowerCase().indexOf('menu.aspx'.toLowerCase()) > 0) || (document.referrer.toLowerCase().indexOf('MenuBS_Agenda_Nuovo.aspx'.toLowerCase()) > 0)) {
        localStorage.removeItem(nomeChiave);
    }
}

//*******************************************************


function isValidDate(s) {
    // format D(D)/M(M)/(YY)YY
    var dateFormat = /^\d{1,4}[\.|\/|-]\d{1,2}[\.|\/|-]\d{1,4}$/;

    if (dateFormat.test(s)) {
        // remove any leading zeros from date values
        s = s.replace(/0*(\d*)/gi, "$1");
        var dateArray = s.split(/[\.|\/|-]/);

        // correct month value
        dateArray[1] = dateArray[1] - 1;

        // correct year value
        if (dateArray[2].length < 4) {
            // correct year value
            dateArray[2] = (parseInt(dateArray[2]) < 50) ? 2000 + parseInt(dateArray[2]) : 1900 + parseInt(dateArray[2]);
        }

        var testDate = new Date(dateArray[2], dateArray[1], dateArray[0]);
        if (testDate.getDate() !== parseInt(dateArray[0]) || testDate.getMonth() !== parseInt(dateArray[1]) || testDate.getFullYear() !== parseInt(dateArray[2])) {
            return false;
        } else {
            return true;
        }
    } else {
        return false;
    }
}


function DataOra_DataIta(data) {

    var dd = data.getDate();
    var mm = data.getMonth() + 1; //January is 0!

    var yyyy = data.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    return dd + '/' + mm + '/' + yyyy;
}


function DataOra_Oggi() {
    var today = new Date();
    return DataOra_DataIta(today);
}


function DataOra_AggiungiGiorni(date, days) {

    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;

}


var delay_KeyUp = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();


function Request_QueryString(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}


function JsonEscape(s) {

    //' VAnni: 30/11/2017: l'escape dell'apice non è necessario...
    // https://stackoverflow.com/questions/19176024/how-to-escape-special-characters-in-building-a-json-string
    //.replace(/[\']/g, "\\\'")

    return s
    .replace(/[\\]/g, '\\\\')
    .replace(/[\"]/g, '\\\"')    
    .replace(/[\/]/g, '\\/')
    .replace(/[\b]/g, '\\b')
    .replace(/[\f]/g, '\\f')
    .replace(/[\n]/g, '\\n')
    .replace(/[\r]/g, '\\r')
    .replace(/[\t]/g, '\\t');


}

function ScomponiUdm(UnitaMisura) {


    var UDM_radice, perHa_hl;

    switch (UnitaMisura) {

        case 2016:
            UDM_radice = 101;
            perHa_hl = 2121;
            break;
        //---------------------
        // a HL
        case 21:
            //cc/hl
            UDM_radice = 104;
            perHa_hl = 2121;
            break;
        case 23:
            //g/hl
            UDM_radice = 3;
            perHa_hl = 2121;
            break;
        case 126:
            //mg/hl
            UDM_radice = 2032;
            perHa_hl = 2121;
            break;
        case 164:
            //ml/hl
            UDM_radice = 101;
            perHa_hl = 2121;
            break;
        case 173:
            //l/hl
            UDM_radice = 29;
            perHa_hl = 2121;
            break;
        case 175:
            //kg/hl
            UDM_radice = 2;
            perHa_hl = 2121;

            break;
        //---------------------
        //a HA
        case 20:
            //g/ha
            UDM_radice = 3;
            perHa_hl = 2123;
            break;
        case 22:
            //l/ha
            UDM_radice = 29;
            perHa_hl = 2123;
            break;
        case 88:
            //kg/ha
            UDM_radice = 2;
            perHa_hl = 2123;

            break;
        case 89:
            //unita/ha
            UDM_radice = -1;
            perHa_hl = 2123;

            break;
        case 90:
            //m3/ha
            UDM_radice = -1;
            perHa_hl = 2123;

            break;
        case 163:
            //ml/ha
            UDM_radice = 101;
            perHa_hl = 2123;
            break;
        case 176:
            //n° u/ha
            perHa_hl = 2123;

            break;
        case 2098:
            //t/ha
            UDM_radice = 304;
            perHa_hl = 2123;

            break;
        case 2112:
            //t/ha spighe
            UDM_radice = 304;
            perHa_hl = 2123;

            break;
        case 2120:
            //q/ha
            UDM_radice = 4;
            perHa_hl = 2123;

            break;

        //--------------------
        case 2:
            //kg
            UDM_radice = 2;
            perHa_hl = 0;
            break;
        case 4:
            //q
            UDM_radice = 4;
            perHa_hl = 0;
            break;
        case 29:
            //l
            UDM_radice = 29;
            perHa_hl = 0;
            break;
        case 101:
            //ml
            UDM_radice = 101;
            perHa_hl = 0;
            break;
        case 3:
            //g
            UDM_radice = 3;
            perHa_hl = 0;
            break;
        case 104:
            //cc
            UDM_radice = 104;
            perHa_hl = 0;
            break;
        case 304:
            //t
            UDM_radice = 304;
            perHa_hl = 0;
            break;
        case 2032:
            //mg
            UDM_radice = 2032;
            perHa_hl = 0;

            break;

        //--------------------
        //concianti
        case 2004:
            //l/100 kg di seme
            UDM_radice = 29;
            perHa_hl = 0;
            break;
        case 2005:
            //ml/100 kg di seme	
            UDM_radice = 101;
            perHa_hl = 0;
            break;
        case 2017:
            //ml/100 kg di semi	
            UDM_radice = 101;
            perHa_hl = 0;
            break;
        case 2021:
            //ml/kg di semente	
            UDM_radice = 101;
            perHa_hl = 0;
            break;
        case 5001003:
            //ml/unità di seme	
            UDM_radice = 101;
            perHa_hl = 0;
            break;
        case 2006:
            //g/unita' di seme	
            UDM_radice = 3;
            perHa_hl = 0;
            break;
        case 2007:
            //g/100 kg di semente	
            UDM_radice = 3;
            perHa_hl = 0;
            break;
        case 2010:
            //kg/100 kg di seme
            UDM_radice = 2;
            perHa_hl = 0;
            break;
        case 2026:
            //kg/1 tonnellata di semente
            UDM_radice = 2;
            perHa_hl = 0;

            break;


        case 2030:
            //litri/1.000 piante
            UDM_radice = 29;
            perHa_hl = 0;
            break;
        case 2018:
            //grammi/pianta
            UDM_radice = 3;
            perHa_hl = 0;
            break;
        case 170:
            //ml/pianta
            UDM_radice = 101;
            perHa_hl = 0;

            break;

    }

    var rval = {
        UDM_radice: UDM_radice,
        perHa_hl: perHa_hl
    }

    return rval;
}


/**
 * Genera un file lato client con il contenuto passato, imposta un link
 * @param {any} nomeFile nome del file
 * @param {any} datiEsportati dati contenuti nel file
 * @param {any} contentType Testo, excel, ecc..
 * @param {any} divDoveInserireTagHref Div dove sarà inserito il file
 */
function GeneraFileClientSide(nomeFile, datiEsportati, contentType, divDoveInserireTagHref, Descrizione) {


    var blob = new Blob([datiEsportati], { 'type': contentType });

    var isIE = !!document.documentMode;
    if (isIE) { //se è IE
        navigator.msSaveOrOpenBlob(blob, nomeFile)
    }
    else { //Se sono gli altri...Chrome...
        var aLink = document.createElement('a');
        var evt = document.createEvent("HTMLEvents");
        evt.initEvent("click", true, false);
        aLink.href = window.URL.createObjectURL(blob);
        aLink.download = nomeFile
        if (Descrizione === undefined) {
            Descrizione = "Fare click per scaricare il file ... !"
        }   
        aLink.textContent = Descrizione;

        if (divDoveInserireTagHref !== "") {
            $("#" + divDoveInserireTagHref).html("");
            document.getElementById(divDoveInserireTagHref).appendChild(aLink);
            aLink.dispatchEvent(evt);
        }


    }

}

/**
 * Scarica un file lato clienti al click sull'icona rappresentante il documento 
 * @param {any} nomeFile nome del file
 * @param {any} fileDati dati contenuti nel file
 * @param {any} contentType Testo, excel, ecc..
 */
function SaveAndOpenFileByteArray(nomeFile, fileDati, estensione) {

    if (estensione === null || estensione === undefined)
        estensione = "";

    let contentType = 'text/html';
    let contentDisposition = '';
    if (estensione === "pdf") {
        contentType = 'application/pdf';
        contentDisposition = 'inline';
    } else if (estensione === "png" || estensione === "bmp") {
        contentType = 'image/' + estensione;
        contentDisposition = 'inline';
    } else if (estensione === "jpeg" || estensione === "jpe" || estensione === "jpg") {
        contentType = 'image/' + "jpeg";
        contentDisposition = 'inline';
    }

    let byteCharacters = atob(fileDati);
    let byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);  

    var downloadLink = document.createElement('a');
    downloadLink.target = '_blank';
     
    var blob = null;
    if (estensione !== "pdf" && estensione !== "png" && estensione !== "jpeg" && estensione !== "jpg" && estensione !== "bmp") {
        downloadLink.download = nomeFile;
        blob = new Blob([byteArray], { type: contentType });
    } else {
        blob = new Blob([byteArray], { type: contentType, contentDisposition: contentDisposition, filename: nomeFile });
    }
        
    var URL = window.URL || window.webkitURL;
    downloadLink.href = URL.createObjectURL(blob);

    document.body.append(downloadLink);

    downloadLink.click();

    // cleanup: remove element and revoke object URL
    document.body.removeChild(downloadLink);
    URL.revokeObjectURL(URL);
};

/**
 * Genera un file lato client con il contenuto passato, imposta un link
 * @param {any} estensioneFile estensione del file
 */
function CreaIconaDownloadDocumento(estensioneFile) {
    let icon = "";

    switch (estensioneFile) {
        case "pdf":
            icon = "fa-file-pdf-o";
            break;

        case "doc":
        case "docx":
            icon = "fa-file-word-o";
            break;

        case "excel":
        case "xls":
        case "xlsx":
            icon = "fa-file-excel-o";
            break;

        case "ppt":
        case "pptx":
            icon = "fa-file-powerpoint-o";
            break;

        case "zip":
            icon = "fa-file-archive-o";
            break;

        case "txt":
        case "xml":
            icon = "fa-file-text-o";
            break;

        case "bmp":
        case "jpg":
        case "jpe":
        case "jpeg":
        case "png":
            icon = "fa-file-image-o";
            break;

        default:
            icon = "fa-file-text-o";
            break;
    } 
    return icon;
}

/**
 * Comunica un messaggio a tutti i frame
 * @param {object} o messaggio in formato agronica
 */
function MessaggioBroadcastAllFrames(o) {

    var a = window.location.protocol + "//" + window.location.host;
    if (window.frames.count !== 0) {
        for (var i = 0; i < window.frames.length; i++) {
            window.frames[i].postMessage(o, a);
        }
    }
   
}

//'#Region "Gestione localizzazione risorse"


/**
 * * cicla tutti gli elementi che hanno un attributo id e recupera la traduzione utilizzando l'id del controllo html, che deve combaciare con quanto indicato nel vettore
 * @param {Array} ArrayConTraduzioni Array di chiave/valore con le traduzioni
 */
function TraduzioneViaJQuery(ArrayConTraduzioni) {

    $('*[id]').each(function () {

        var elem = $(this);
        var elem_id = $(this).attr("id");
        var testo = Traduzione(ArrayConTraduzioni, elem_id);

        if (testo === "") {
            testo = Traduzione(ArrayConTraduzioni, elem_id + ".Text");
        }

        var a = elem.prop('nodeName');
        if (a && testo !== "") {
            switch (a.toLowerCase()) {
                case "span":
                    elem.text(testo);
                    break;
                case "label":
                    elem.text(testo);
                    break;
                case "option":
                    elem.text(testo);
                    break;
                case "a":
                    elem.text(testo);
                    break;
                default:
            }
        }
    });

}

/**
 * Dato un oggetto con proprietà "Key", "Value" restituisce il valore corrispondente alla chiave
 * @param {object} obj
 * @param {string} chiave
 * @param {string} TestoAlternativo se non viene trovato nulla restituisce questo testo
 */
function Traduzione(obj, chiave, TestoAlternativo) {

    if (TestoAlternativo === undefined) {
        TestoAlternativo = "";
    }

    if (obj === null || obj === undefined || typeof obj !== 'object' || typeof chiave !== 'string' || Object.getOwnPropertyNames(obj).length === 0) {
        //console.log("Oggetto non valido o chiave non valida. obj-> %o - chiave-> %o", obj, chiave);
        return TestoAlternativo;
    }

    var a1 = Object.hasOwn(obj, chiave) ? obj[chiave] : "";

    if (a1 === "") {
        return TestoAlternativo;
    } else {
        return a1;
    }
}

/**
 * Dato un array di oggetti con "Key", "Value" restituisce il valore corrispondente alla prima occorrenza della chiave
 * @param {Array} arrayMultiResx
 * @param {string} chiave
 * @param {string} testoAlternativo se non viene trovato nulla restituisce questo testo
 */
function TraduzioneMultiResx(arrayMultiResx, chiave, testoAlternativo) {

    if (testoAlternativo === undefined) {
        testoAlternativo = "";
    }

    var traduzione = testoAlternativo;

    if (arrayMultiResx.length > 0) {
        for (var i = 0; i < arrayMultiResx.length; i++) {
            traduzione = Traduzione(arrayMultiResx[i], chiave, "");
            if (traduzione !== "") {
                break;
            }
        }

        if (traduzione === "") {
            //console.log("Chiave %s non trovata", chiave);
            traduzione = testoAlternativo;
        }
    }

    return traduzione;

}

/**
 * Restituisce come oggetto anonimo le risorse per la lingua corrente contenute nel file specificato (dichiarare nelle pagine master dei vari siti la variabile localizationPageUrl )
 * @param {string} resxToRead Percorso al file resx da restituire
 * @param {string} fileNameToLog Nome del file da mostrare nel log
 */
function readResxFile(resxToRead, fileNameToLog) {
    var resxObj = null;
    fileNameToLog = typeof fileNameToLog === "string" && fileNameToLog.length > 0 ? fileNameToLog + " - " : "";
    // Avendo necessità di poter risalire in ogni file alla pagina Localization.aspx la quale dev'essere nella root del progetto,
    // sfrutto le proprietà dell'oggetto document per creare un percorso assoluto. La proprietà origin restituisce la prima parte del
    // percorso, es "http://localhost", pathname le parti successive es "/AgronicaAgenda/Anagrafica/Campo_edit.aspx" delle quali prendo solo la prima
    // var urlLocalization = document.location.origin + "/" + document.location.pathname.split("/")[1] + "/Localization.aspx/RitornaRisorseBS";
    ajaxAgronicaSync(localizationPageUrl + "/RitornaRisorseBS", JSON.stringify({ files: resxToRead }), false,
        function (risposta) {
            try {
                resxObj = JSON.parse(risposta.RispostaStringa);
                console.log(kendo.format(fileNameToLog + "File '{0}' letto correttamente.", resxToRead));
            } catch (e) {
                console.log(kendo.format(fileNameToLog + "si è verificato il seguente errore in fase di parse del json del file '{0}' -> {1}",
                    resxToRead, e));
            }

        }, function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js...
                console.log(fileNameToLog + "si è verificato un errore nell'esecuzione della chiamata ajax, controllare i parametri passati");
            } else {
                // ...mentre in questo c'è stato un errore lato server
                console.log(kendo.format(fileNameToLog + "si è verificato il seguente errore in fase di lettura del file '{0}' -> {1}",
                    resxToRead, risposta.Errore));
            }
        }, undefined, false);
    return resxObj;
};
//'#END Region "Gestione localizzazione risorse"


//'#Region "utility per i form"

/**
 * Trova un elemento risalendo in maniera ricorsiva il DOM partendo dal controllo jQuery indicato ed eseguendo la ricerca dell'elemento in ciascun figlio
 * @param {object} controlloJQuery oggetto jQuery
 * @param {string} strElementoDaTrovare selettore jQuery dell'elemento da trovare
 * @returns {object} elemento se trovato oppure undefined
 */
function jQueryTrovaElementoRicorsivo(controlloJQuery, strElementoDaTrovare) {

    var elem = controlloJQuery;
    var risultato = new Array;
    while (elem.length !== 0 && risultato.length == 0) {
        elem = elem.parent();
        risultato = elem.find(strElementoDaTrovare);
    }

    if (risultato.length == 0) {
        return undefined
    } else {
        return risultato;
    }

}

function VersionJS() {
    try {
        console.log("jQuery: " + jQuery.fn.jquery);
        console.log("Bootstrap: " + $.fn.tooltip.Constructor.VERSION);
        console.log("kendo: " + kendo.version);
    } catch (e) {
    }
    
}

/**
 * Rende obbligatorio (o meno) un campo nel form (se utilizzato lo stardard agronica), imposta la classe nell'elemento label riferito con "for" ed aggiunge un asterisco che indica l'obbligatorietà
 * @param {boolean} obbligatorio indica se il campo è obbligatorio o no
 * @param {string} jQuerySelector selettore jQuery con cui ricercare l'elemento da elaborare (per ID) es.: "#cmbSpecie"
 * @param {string} classeDaAggiungereAdEtichetta (opzionale) classe che evidenzia l'elemento obbligatorio, se non passata usa un default
 * @returns {boolean} false se non trova il controllo (o etichetta associata con attributo "for")
 */
function RendiCampoObbligatoriConGestioneHtml(obbligatorio, jQuerySelector, classeDaAggiungereAdEtichetta) {

    if (!jQuerySelector.includes("#")) {
        jQuerySelector = "#" + jQuerySelector;
    }

    let controllo = $(jQuerySelector);

    //previene errori js successivi se non trovato
    if (controllo.length == 0) {
        console.warn("RendiCampoObbligatoriConGestioneHtml: il selettore " + jQuerySelector + " non ha recuperato alcun elemento.");
        return false;
    }

    let idCtrl = controllo.attr("id");
    let etichetta = $('label[for="' + idCtrl + '"]');
    if (etichetta.length == 0) {
        console.warn("RendiCampoObbligatoriConGestioneHtml: il selettore " + jQuerySelector + " non è stato usato nel campo for di nessuna etichetta.");
        return false;
    }
    let h = etichetta.html();

    if (!classeDaAggiungereAdEtichetta) {
        classeDaAggiungereAdEtichetta = "obbligatorio";
    }

    if (obbligatorio) {
        controllo.attr("required", "");
        if (!h.includes("*")) {
            h = h + " *";
        }
        etichetta.html(h);
        etichetta.addClass(classeDaAggiungereAdEtichetta);
    } else {
        controllo.removeAttr("required");
        h = h.replace("*", "");
        etichetta.html(h);
        etichetta.removeClass(classeDaAggiungereAdEtichetta);
    }

    return true;
}
//'#End Region "utility per i form"


//'#Region "utility per Storage"

const STORAGE_TYPE = "sessionStorage"

/**
 * Crea l'istanza di Storage
 * @param {string} type indica il tipo di storage da usare: "localStorage" o "sessionStorage"
 * @returns {Storage} Restituisce lo storage
 */
function getStorage(type) {
    var storage;
    try {
        storage = window[type];
        var x = '__storage_test__';
        storage.setItem(x, x);
        storage.removeItem(x);
        return storage;
    }
    catch (e) {
        return undefined;
    }
}

var storage = getStorage(STORAGE_TYPE);

/**
 * Restituisce l'oggetto salvato in storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 * @returns {Object} Oggetto memorizzato
 */
function storageGetItem(key) {
    if (storage !== undefined) {
        try {
            let item = storage.getItem(key);
            if (item !== undefined) {
                return JSON.parse(item);
            } else {
                return undefined
            }
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Restituisce l'oggetto salvato in storage
 * @param {string} key Chiave dove memorizzare l'oggetto
 * @param {Object} value Oggetto da memorizzare
 */
function storageSetItem(key, value) {
    if (storage !== undefined) {
        try {
            if (value !== undefined) {
                storage.setItem(key, JSON.stringify(value));
            }
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Elimina l'oggetto salvato in storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 */
function storageRemoveItem(key) {
    if (storage !== undefined) {
        try {
            storage.removeItem(key);
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Elimina tutti gli oggetti salvati in storage
 */
function storageClear() {
    if (storage !== undefined) {
        try {
            storage.clear();
        } catch (e) {
            return undefined;
        }
    }
}

/**
 * Verifica l'esistenza della chiave nello storage
 * @param {string} key Chiave dove è memorizzato l'oggetto
 * @returns {Boolean} Presenza della chiave
 */
function storageExistItem(key) {
    if (storage !== undefined) {
        try {
            if (storage.getItem(key) !== undefined && storage.getItem(key) !== null) {
                return true;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    }
    return false;
}
//'#End Region "utility per Storage"

//#Region "Funzioni Utili Utente"

/**
 * Legge permesso utente, devono esistere le variabili: objP_utenti e pathCoreWS
 * @param {string} username Username utente
 * @param {number} id_attivita Riferimento alla tabella TB_Attività
 * @param {number} id_operazione Tipo operazione 1 Lettura, 2 Lettura + Scrittura
 * @param {Function} callback Funzione da richiamare se non si utilizza la Promise
 * @returns {Boolean} Presenza della chiave
 */
function Agro_LeggiPermessoUtente(username, id_attivita, id_operazione, callback) {
    return new Promise((resolve, reject) => {
        let storage_key = "LeggiPermessoUtente_" + username + "_" + id_attivita + "_" + id_operazione;

        if (!storageExistItem(storage_key)) {
            var ActualDate = new Date()
            var stringData = ActualDate.toLocaleDateString();
            var param = {
                UserName: username,
                Id_Servizio: 5,
                Id_Attivita: id_attivita,
                Id_Operazione: id_operazione,
                DataOraControllo: ActualDate,
                xFiltroAggiuntivo: '',
                objP_utenti: objP_utenti
            }
            ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Controlla_Permessi_UtenteR", JSON.stringify(param),
                function (risposta) {
                    if (risposta.RispostaStringa.toLowerCase() == "true") {
                        //window.sessionStorage.setItem(storage_key, JSON.stringify(true));
                        storageSetItem(storage_key, true);
                        if (callback != undefined && callback != null) {
                            callback(true);
                        }
                        resolve(true);
                    } else {
                        //window.sessionStorage.setItem(storage_key, JSON.stringify(false));
                        storageSetItem(storage_key, false);
                        if (callback != undefined && callback != null) {
                            callback(false);
                        }
                        resolve(false);
                    }
                }, null, null, false);
        } else {
            //resolve(JSON.parse(window.sessionStorage.getItem(storage_key)));
            let resp = storageGetItem(storage_key)
            if (callback != undefined && callback != null) {
                callback(resp);
            }
            resolve(resp);
        }

    });
}


//#End Region "Funzioni Utili Utente"

/**
 * Utilizzare questa funzione per definire l'argomento targetOrigin della funzione postMessage.
 * Presuppone che le window coinvolte siano appartenenti alla stessa origin
 * @param {any} windowSecondaria
 */
function ottieniTargetOrigin(windowSecondaria) {

    let targetOrigin = windowSecondaria.location.origin;

    if (windowSecondaria.location.hostname === "localhost") {
        targetOrigin = "*";
    }

    return targetOrigin;
}

/**
 * Utilizzare questa funzione in quella di eventListener dell'evento 'message' che permette di ascoltare i dati mandati attraverso la funzione postMessage.
 * Controlla che la origin indicata nell'oggetto messaggio coincida con quella di strUrlSecondaria.
 * Se strUrlSecondaria è relativa la completa con origin di windowPrimaria.
 * @param {Window} windowPrimaria la window che ascolta per l'evento 'message'
 * @param {string} strUrlSecondaria la url che windowPrimaria aveva chiamato inizialmente e da cui si aspetta di ricevere il messaggio (iframe, ecc...)
 * @param {MessageEvent} messaggio l'oggetto evento passato alla funzione di eventListener
 */
function verificaOriginSecondaria(windowPrimaria, strUrlSecondaria, messaggio) {

    let originCorretta = false;

    let urlSecondaria = null;

    if (strUrlSecondaria !== undefined && strUrlSecondaria !== null && strUrlSecondaria !== "") {
        if (URL.canParse(strUrlSecondaria)) {
            // percorso assoluto
            urlSecondaria = new URL(strUrlSecondaria);
        }
        else if (URL.canParse(strUrlSecondaria, windowPrimaria.location.origin)) {
            // percorso relativo
            urlSecondaria = new URL(strUrlSecondaria, windowPrimaria.location.origin);
        }
    }

    if (windowPrimaria.location.hostname === "localhost" || (urlSecondaria !== null && messaggio.origin === urlSecondaria.origin)) {
        originCorretta = true;
    }

    return originCorretta;
}

//FUNZIONE PRESENTE ANCHE NEL GIASBASE controlli_form.js
function SanitizeTesto_MantieniVirgoletteECaratteriAccentati(testo) {
    // 1. Caratteri pericolosi in HTML e JavaScript Injection:
    //    Tag HTML: <, >.
    //    Caratteri speciali HTML: & (entità HTML come & lt;), ' e " (delimitatori di attributi).
    //    Caratteri JavaScript: ()(funzioni), {}, [](strutture di codice), ;, = (assegnazioni).
    // 2. Caratteri pericolosi in SQL Injection:
    //    Separatori e commenti: ;, --, /, *.
    //    Delimitatori stringhe: ', ".
    // 3. Caratteri di escape:
    //     Backslash: \ (utilizzato per fare escaping).

    let pattern = /[^a-zA-Z0-9àèéìòùÀÈÉÌÒÙçÇ .,?!\-_]/g;
    testo = testo.replace(pattern, '');

    return testo;
}