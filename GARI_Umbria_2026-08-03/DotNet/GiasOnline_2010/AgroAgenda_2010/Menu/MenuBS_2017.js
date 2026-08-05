
//MenuBS_2017.js

// -------------------------------------------
// -------------- GLOBAL VAR -----------------
// -------------------------------------------

var scriviModifica = "scrivi";
var gestionePreferiti = false;
var listaPreferiti = "";
var macroSezioneID = "";
var macroSezione = "";

// -------------------------------------------
// ------------- WEB SERVICES ----------------
// -------------------------------------------

var url_MenuBS_WS = "./MenuBS_2017.aspx";

//Gestione pulsante
function AgroPulsante(idSezionePadre, RedirectURL) {

    if (RedirectURL !== "") {
        window.location = RedirectURL;
        return;
    }

    LeggiSezioni("2", idSezionePadre);
}

//per la lettura delle macrosezioni
function LeggiMacrosezioni() {
    LeggiSezioni("1", "0");
}

// per la lettura delle sezioni
function LeggiSezioni(IDTipoSezione, IDSezionePadre) {
    gestionePreferiti = false;
    $("#preferiti_ricerca_voce").show();
    $("#preferiti_ricerca_voce").val("");
    $('#gestione_preferiti_bottone_dashboard').hide();
    $('#preferiti_bottone_dashboard').show();
    $("#contenitore_preferiti").html("");
    $('#contenitore_sezioni').html("");
    ajaxAgronica(url_MenuBS_WS + "/LeggiSezioni",
        '{ "IDTipoSezione": ' + IDTipoSezione + ', "IDSezionePadre": ' + IDSezionePadre + ', "Ricerca": "", "Preferiti": "false"}',
        function (risposta) {
            GeneraHtml(risposta);
        }, null);
}

// per la ricerca delle sezioni
function CercaSezioni(Ricerca, Preferiti) {
    if (Preferiti !== "true") {
        $('#gestione_preferiti_bottone_dashboard').hide();
        $('#preferiti_bottone_dashboard').show();
    }
    // resetto stato macrosezione
    if (macroSezioneID !== "") {
        $("#" + macroSezioneID).css('color', 'black');
        macroSezioneID = "";
        macroSezione = "";
    }
    $("#contenitore_preferiti").html("");
    $('#contenitore_sezioni').html("");
    ajaxAgronica(url_MenuBS_WS + "/LeggiSezioni",
        '{ "IDTipoSezione": 2, "IDSezionePadre": 0, "Ricerca": "' + Ricerca + '", "Preferiti":"' + Preferiti + '"}',
        function (risposta) {
            GeneraHtml(risposta);
            if (gestionePreferiti && Ricerca === "") {
                listaPreferiti = "";
                $(".buttonpreferitielementi").each(function () {
                    if ($(this).html().includes("<i class=\"fa fa-star-o\" aria-hidden=\"true\"></i>")) {
                        $(this).css('color', 'white');
                        listaPreferiti = listaPreferiti + $(this).attr("aria-idsezione") + "|";
                    }
                });
            }
        }, null);
}

// per generare l'html di risposta e comporre cosi il div delle sezioni
function GeneraHtml(risposta) {

    var firstTime = true;

    for (var i = 0; i < risposta.RispostaStringa.length; i++) {

        var sel = "#" + risposta.RispostaStringa[i].CollocazioneJQuerySelector;
        var newHtml = risposta.RispostaStringa[i].Contenutohtml;

        // se sto aprendo delle macrosezioni il contenuto precedente deve essere svuotato
        if (risposta.RispostaStringa[i].CollocazioneJQuerySelector.includes("contenitore_sezioni")) {
            if (gestionePreferiti) {
                newHtml = newHtml.replace("gestioneMacrosezioneSezione(this);", "gestionePreferitoSezione(this);");
                sel = "#contenitore_preferiti";
            }
            if (firstTime) {
                firstTime = false;
                $(sel).html("");
            }

        }

        var oldHtml = $(sel).html();

        // devo sovrascrivere altrimenti mi tiene anche la vecchia sezione
        $(sel).html(oldHtml + newHtml);
    }
}

//per la lettura dei widget degli allarmi
function LeggiWidgetAllarmi() {
    ajaxAgronica(url_MenuBS_WS + "/LeggiWidgetAllarmi",
        "{}",
        function (risposta) {

            var html = risposta.RispostaStringa;

            // impostazioni massime del widget meteo
            $('#widget_meteo').css('height', '100px !important');
            $('#widget_meteo').css('max-height', '100px !important');
            $('#widget_meteo').css('min-height', '100px !important');
            // carico il widget per gli allarmi
            $('#widget_allarmi').html(html);          

        }, null);
}

// -------------------------------------------
// -------------------------------------------
// ---------------- PULSANTI -----------------
// -------------------------------------------
// -------------------------------------------

// -------------------------------------------
// --------- APRIRE DASHBOARD METEO ----------
// -------------------------------------------
function apriDashboardMeteo() {

    // apro il banner di caricamento
    WaitFrame.show();

    // richiamo la dashboard del meteo
    BarraTitolo(Traduzione(menuBS2017Resx, "DashboardMeteo", "Dashboard Meteo"), "#002F5F", true);
    apriMeteoLarge();

    // chiudo il frame di caricamento
    WaitFrame.hide();
}

// -------------------------------------------
// --------- GESTIONE PREFERITI ------------
// -------------------------------------------
function GestionePreferiti() {
    gestionePreferiti = true;
    $("#contenitore_principale").hide();
    $("#gestione_preferiti").show();
    BarraTitolo(Traduzione(menuBS2017Resx, "GestionePreferiti", "Gestione Preferiti"), "#aeaeae", true);
    $('#contenitore_sezioni').html("");
    $("#contenitore_preferiti").html("");
    ajaxAgronica(url_MenuBS_WS + "/LeggiSezioniPreferiti",
        JSON.stringify({ IDSezionePadre: 0, Ricerca: "" }),
        function (risposta) {
            GeneraHtml(risposta);
            listaPreferiti = "";
            $(".buttonpreferitielementi").each(function () {
                if ($(this).html().includes("<i class=\"fa fa-star-o\" aria-hidden=\"true\"></i>")) {
                    $(this).css('color', 'white');
                    listaPreferiti = listaPreferiti + $(this).attr("aria-idsezione") + "|";
                }
            });
        }, null);
}

function CercaPreferiti(ricerca) {
    $(".buttonpreferitielementi").each(function () {
        if (ricerca == "" || $(this).text().toLowerCase().includes(ricerca.toLowerCase())) {
            $(this).css('display','block');
        } else {
            $(this).css('display', 'none');
        }
    });
}

// gestione stato preferito
function gestionePreferitoSezione(obj) {
    
    var id = $(obj).attr("id");
    var idSezione = $(obj).attr("aria-idsezione");

    if ($(obj).html().includes("<i class=\"fa fa-star-o\" aria-hidden=\"true\"></i>")) {
        $(obj).html($(obj).html().replace(" <i class=\"fa fa-star-o\" aria-hidden=\"true\"></i>", ""));
        $(obj).css('color', 'black');
        var temp = "|" + listaPreferiti;
        listaPreferiti = temp.replace("|" + idSezione + "|", "|").substring(1);
    } else {
        $(obj).html($(obj).html() + " <i class=\"fa fa-star-o\" aria-hidden=\"true\"></i>");
        $(obj).css('color', 'white');
        listaPreferiti = listaPreferiti + idSezione + "|";
    }
    
    // mando la lista dei preferiti, separati da pipe, al database
    var param = "{ listaPreferiti: '" + (listaPreferiti === "" ? "nothing" : listaPreferiti) + "', modalitaScrittura: 'aggiorna' }";

    // salvataggio dei preferiti
    ajaxAgronica(url_MenuBS_WS + "/SalvaPreferiti",
        param,
        function (risposta) {
            var html = risposta.RispostaStringa;
        }, null);

    // modificaPreferiti();
}

// per i pulsanti della macrocategoria da aprire
function gestioneMacrosezioneSezione(obj) {

    // recupero i dati relativi al pulsante premuto
    var id = $(obj).attr("id");
    var value = $(obj).text();
    var classe = $(obj).attr("class");
    var style = $(obj).attr("style");
    // dallo stile recupero il colore
    var splitted = style.split(" ");
    var color = splitted[1];
    color = " " + color.replace(";", "");

    var RedirectURL = $(obj).attr("aria-RedirectURL");
    var sitoRichiesto = $(obj).attr("aria-sitoRichiesto");
    var idSezione = $(obj).attr("aria-idSezione");
    var paginaRichiesta = $(obj).attr("aria-paginaRichiesta");
    var aziendaRichiesta = $(obj).attr("aria-RichiedeAziendaSelezionata");

    if (id === "menu_profitosan") {

        GoToProfitosan();
    
    } else if (id === "menu_bollettini") {

        GoToBollettini();

    } else if (id === "menu_giasapp") {

        window.location = RedirectURL;

    } else if (RedirectURL !== "" || (sitoRichiesto !== "" && sitoRichiesto !== "0")) {

        // controllo se è richiesta l'azienda per la voce di menu selezionata
        if (aziendaRichiesta !== "true" || pivaAziendaSelezionata !== "") {

            // prima di fare il redirect mi salvo la barra del titolo
            ajaxAgronica(url_MenuBS_WS + "/salvaTitoloSezioneConGestioneRedirect",
                "{ IDSezione: " + idSezione + "  }",
                gestioneRedirect, null);

        } else {

            /* $("<div></div>").kendoAlert({
                title: "Operazione non consentita",
                content: "E' necessario selezionare un'azienda prima di procedere."
            }).data("kendoAlert").bind("close", function (e) { GoToFiltrino(idSezione); }).open(); */

            GoToFiltrino(idSezione);
        }

    } else {

        // se non sono in una macrosezione ma sono in una sezione il testo del titolo deve essere
        // composto da : titolomacrosezione -> titolosezione
        // altrimenti significa che ho cambiato macrosezione quindi cambio testo e colore
        if (id.includes("preferiti_bottone") && macroSezione !== "") {
            value = macroSezione + " <i class='fa fa-arrow-right'></i> " + $(obj).text();
        } else {
            if (macroSezioneID !== "") $("#" + macroSezioneID).css('color', 'black');
            macroSezioneID = id;
            $("#"+id).css('color', 'white');
            macroSezione = id.includes("macrocategoria")?value:"";
        }

        // visualizza titolo macro sezione
        // BarraTitolo(value, color, false);

        if (id.includes("macrocategoria")) {
            LeggiSezioni("2", $(obj).attr("aria-idSezione"));
        } else if (gestionePreferiti) {
            LeggiPreferitiDash();
        }
    }
}

// gestione dei preferiti da posizionare in dashboard
function LeggiPreferitiDash() {
    // chiamata alla sezione di scelta dei preferiti
    gestionePreferiti = false;
    $("#gestione_preferiti_bottone_dashboard").show();
    $("#preferiti_bottone_dashboard").hide();
    $("#preferiti_ricerca_voce").show();
    $("#preferiti_ricerca_voce").val("");

    // visualizza barra del titolo
    BarraTitolo(Traduzione(menuBS2017Resx, "MenuPrincipale", "Menu Principale"), "#002F5F", false);
    CercaSezioni("", "true");
}

// visualizza barra del titolo
function BarraTitolo(testo, colore, home) {
    if (home) $("#header_azioni_menu").html("<a href='" + url_MenuBS_WS + "'><i title='Home' class='fa fa-home' style='color:white;margin:5px;font-size:36px;'></i></a>");
    $(".titolo_sezione").css("background-color", colore);
    $(".titolo_sezione").html(CercaAziende()+testo);
    $(".titolo_sezione").show();
}

// funzione di ricerca aziende
function CercaAziende() {
    if (!flagFiltroAziende) return "";
    var html = "<div class='pull-left' style='margin-top:3px;margin-right:10px;'>";
    html += "<a href='#' onclick='cercaAziende();'><i title='" + Traduzione(menuBS2017Resx, "CercaAzienda", "Cerca Azienda") + "' id='cerca_aziende_icon' class='iconmenusezioni fa fa-search' style='cursor:pointer; font-size:30px;'></i></a>";
    html += "<a href='#' onclick='Azione_Cambia_Impresa();'><i title='" + Traduzione(menuBS2017Resx, "FiltroAziende", "Filtro Aziende") + "' id='cambia_aziende_icon' class='iconmenusezioni fa fa-search-plus' style='cursor:pointer; font-size:30px;'></i></a>";
    html += "</div><div class='pull-left' style='width:0px;'><nav style='position:relative;top:40px;right:80px;width:400px;height:0px;z-index:100;'>";
    html += "<input autocomplete='off' onkeydown='if (event.keyCode == 13) return false;'  onkeyup='if (event.keyCode == 13 || this.value.length >= lenFiltroAziende) leggiImprese(this.value,0); else alertImprese(lenFiltroAziende);' type='search' type='text' placeholder='" + Traduzione(menuBS2017Resx, "CercaAzienda", "Cerca Azienda") + "' class='ricercavocemenu' id='cerca_aziende_text' style='display:none;'>";
    html += "<ul id ='cerca_aziende' class='nav' style='display:none;overflow: hidden; line-height:40px !important;padding: 0px;'></ul></nav></div>";
    return html;
}

function alertImprese(lenFiltro) {
    $("#cerca_aziende").empty();
    var html = "<div class='alert alert-info' style='font-size:12px; line-height:0px; height:30px; border-radius:10px; border: solid 2px rgba(0,0,0,0.21);'>Inserire almeno " + lenFiltro + " caratteri</div>";
    $("#cerca_aziende").append('<li style="margin-top: 5px;">' + html + '</li>');
}

// chiamata ws per ricerca aziende
function leggiImprese(ricerca, idSezione) {
    if (ricerca === "") {
        $("#cerca_aziende").empty();
    } else {
        ajaxAgronica(url_MenuBS_WS + "/LeggiImprese",
            JSON.stringify({ ricerca: ricerca, idSezione: idSezione }),
            function (risposta) {
                $("#cerca_aziende").empty();
                for (var i = 0; i < risposta.RispostaStringa.length; i++) {
                    var html = risposta.RispostaStringa[i].Contenutohtml;
                    $("#cerca_aziende").append('<li style="border: 0px !important;">' + html + '</li>');
                }
            }, null);
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

// gestione click su impresa 
function gestioneCambioImpresa(obj) {
    var idSezione = $(obj).attr("aria-idSezione");
    var PivaSelezionata = $(obj).attr("aria-PivaSelezionata");
    var AziendaSelezionata = $(obj).attr("aria-AziendaSelezionata");
    ajaxAgronica(url_MenuBS_WS + "/cambiaImpresaConGestioneRedirect",
        JSON.stringify({ Piva: PivaSelezionata, Azienda: AziendaSelezionata, IDSezione: idSezione }),
        gestioneRedirect, null);
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
                window.open(targetUrl);
            }
        } else {
            targetUrl = risposta.RispostaStringa;
            window.location = targetUrl;
        }
    }
}

function GoToFiltrino(idSezione) {
    $.ajax({
        type: 'POST',
        url: url_MenuBS_WS + '/GetFiltroAziende',
        data: '{ "IDSezione": ' + idSezione + ' }',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = r.d;
        }
    });
}

function GoToBollettini() {
    var win = window.open("http://4bio.agronicagroup.it", "Bollettini GAP", "noopener");
    win.focus();
}

function GoToProfitosan() {
    $.ajax({
        type: 'POST',
        url: url_MenuBS_WS + '/GetProfitosan',
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            var win = window.open(r.d, 'Profitosan');
            win.focus();
        }
    });
}

function GoToMenuPrecedente() {
    var dlg = $("<div></div>").kendoConfirm({
        content: Traduzione(menuBS2017Resx, "VuoiPassareAlMenuPrecedenteDiGias", "Vuoi passare al menu precedente di GIAS?"),
        messages: { okText: Traduzione(menuBS2017Resx, "Si", "Sì"), cancel: Traduzione(menuBS2017Resx, "No", "No") },
        title: Traduzione(menuBS2017Resx, "Conferma", "Conferma")
    }).data("kendoConfirm");

    dlg.result.done(function () {
        $.ajax({
            type: 'POST',
            url: url_MenuBS_WS + '/GetMenuPrecedente',
            data: "{}",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                window.location = r.d;
            }
        });
    });

    dlg.open();
    
}