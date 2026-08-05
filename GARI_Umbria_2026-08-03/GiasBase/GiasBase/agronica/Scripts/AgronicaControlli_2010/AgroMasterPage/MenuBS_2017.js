function leggiImprese(ricerca, idSezione) {
    if (ricerca === "") {
        $("#cerca_aziende").empty();
    } else {
        var parametri = { ricerca: ricerca, idSezione: idSezione };
        if (menu_corews) {
            parametri.objP_server = objP_server;
            parametri.objP_utenti = objP_utenti;
        }
        ajaxAgronica(pathMenuWS + "/LeggiImprese",
            JSON.stringify(parametri),
            function (risposta) {
                $("#cerca_aziende").empty();
                for (var i = 0; i < risposta.RispostaStringa.length; i++) {
                    var html = risposta.RispostaStringa[i].Contenutohtml;
                    $("#cerca_aziende").append('<li>' + html + '</li>');
                }
            }, null);
    }
}

function leggiSezioni(idSezione, idSezionePadre) {
    var parametri = { IDTipoSezione: 2, IDSezionePadre: idSezionePadre, Ricerca: "", Preferiti: idSezionePadre == 0 ? "true" : "false" };
    if (menu_corews) {
        parametri.objP_server = objP_server;
        parametri.objP_utenti = objP_utenti;
    }
    ajaxAgronica(pathMenuWS + "/LeggiSezioni",
        JSON.stringify(parametri),
        function (risposta) {
            $("#menu_sezioni").empty();
            for (var i = 0; i < risposta.RispostaStringa.length; i++) {
                var html = risposta.RispostaStringa[i].Contenutohtml;
                if (html.includes("aria-idSezione='" + idSezione + "'")) {
                    html = html.replace("style='", "style='color:white;");
                }
                $("#menu_sezioni").append('<li>' + html + '</li>');
            }
        }, null);
}

// gestione del click menu sezioni
function menuSezioni(idSezione, idSezionePadre) {
    if (!menu_sezioni || !$("#menu_sezioni").is(":visible")) {
        menu_preferiti = false;
        menu_sezioni = true;
        $("#menu_sezioni").show();
        leggiSezioni(idSezione, idSezionePadre);
    } else {
        menu_sezioni = false;
        $("#menu_sezioni").hide();
    }
}

// gestione del click menu preferiti
function menuPreferiti(idSezione) {
    if (!menu_preferiti || !$("#menu_sezioni").is(":visible")) {
        menu_sezioni = false;
        menu_preferiti = true;
        $("#menu_sezioni").show();
        leggiSezioni(idSezione, 0);
    } else {
        menu_preferiti = false;
        $("#menu_sezioni").hide();
    }
}

// gestione del click cerca aziende
function cercaAziende() {
    if (!$("#cerca_aziende").is(":visible")) {
        $("#cerca_aziende").show();
        $("#cerca_aziende_text").show();
    } else {
        $("#cerca_aziende").hide();
        $("#cerca_aziende_text").hide();
    }
}

// gestione click su sezioni 
function gestioneMacrosezioneSezione(obj) {
    var idSezione = $(obj).attr("aria-idSezione");
    var RedirectURL = $(obj).attr("aria-RedirectURL");
    var sitoRichiesto = $(obj).attr("aria-sitoRichiesto");
    var paginaRichiesta = $(obj).attr("aria-paginaRichiesta");
    var aziendaRichiesta = $(obj).attr("aria-RichiedeAziendaSelezionata");
    if (RedirectURL !== "" || (sitoRichiesto !== "" && sitoRichiesto !== "0")) {
        // controllo se è richiesta l'azienda per la voce di menu selezionata
        if (aziendaRichiesta !== "true" || menu_piva !== "") {
            if (menu_corews) {
                window.location = menu_url + "&IDSezione=" + idSezione;
            } else {
                // salvo la barra del titolo + redirect alla pagina
                ajaxAgronica(pathMenuWS + "/salvaTitoloSezioneConGestioneRedirect",
                    JSON.stringify({ IDSezione: idSezione }),
                    gestioneRedirect, null);
            }
        } else {            
            GoToFiltrino(idSezione);
        }
    }
}

// gestione click su impresa 
function gestioneCambioImpresa(obj) {
    var idSezione = $(obj).attr("aria-idSezione");
    var PivaSelezionata = $(obj).attr("aria-PivaSelezionata");
    var AziendaSelezionata = $(obj).attr("aria-AziendaSelezionata");
    if (menu_corews) {
        window.location = menu_url + "&IDSezione=" + idSezione + "&PivaSelezionata=" + PivaSelezionata;
    } else {
        ajaxAgronica(pathMenuWS + "/cambiaImpresaConGestioneRedirect",
            JSON.stringify({ Piva: PivaSelezionata, Azienda: AziendaSelezionata, IDSezione: idSezione }),
            gestioneRedirect, null);
    }
}

// gestione redirect
function gestioneRedirect(risposta) {
    if (risposta.RispostaOK) {
        var targetUrl = null;
        if (risposta.Tipo === "1") {
            targetUrl = risposta.RispostaStringa;
            if (targetUrl.indexOf("<script") !== -1) {
                $('#menu_script').append(risposta.RispostaStringa);
            } else {
                window.open(targetUrl.replace("../", rootUrl));
            }
        } else {
            targetUrl = risposta.RispostaStringa;
            window.location = targetUrl.replace("../", rootUrl);
        }
    }
}

function GoToFiltrino(idSezione) {
    if (menu_corews) {
        window.location = menu_url + "&IDSezione=" + idSezione + "&FiltroAziende=";
    } else {               
        $.ajax({
            type: 'POST',
            url: pathMenuWS + '/GetFiltroAziende',
            data: '{ "IDSezione": ' + idSezione + ' }',
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                var targetUrl = r.d;
                window.location = targetUrl.replace("../", rootUrl);
            }
        });
    }
}
