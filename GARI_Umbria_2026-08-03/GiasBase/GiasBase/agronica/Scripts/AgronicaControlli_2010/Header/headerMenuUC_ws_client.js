
var elencoAziendeSelezionate = null;
var menus = null;
var menusPreferiti = null;
var dashboard = `<li class="">
                    <a href="#" onClick="redirectNG(999)" ><i class="fa fa_gias_dashboard"></i><span>Dashboard</span></a>
                 </li>`
var configuraPreferiti = `<li class="active hRightElement-side">
                    <a href="#" onClick="redirectNG(1000)" class="" ><i class="icon-favorite-edit-sidemenu"></i><span>Configura</span></a>
                 </li>`
let queueChiamateRicercaAziende = [];
var primaRichiesta = true;
//UTILITY

function compareString(stringa1, stringa2) {
    stringa1 = stringa1.toLowerCase();
    stringa2 = stringa2.toLowerCase();

    stringa1 = stringa1.replaceAll(" ", "");
    stringa2 = stringa2.replaceAll(" ", "");
    return stringa1 == stringa2;
}

function ottieniClassi(classi) {
    try {
        if (classi == undefined || classi == null) {
            return "";
        }


        return classi.split(".").join(" ");
    }
    catch {
        console.log("Errore funzione: ottieniClassi, headerMenuUC_ws_client");
    }
}

//FINE UTILITY






//SIDEBAR
function generaFigli(figli, stringaRicerca) {
    //Genera i Figli della sidebar
    let x = ""
    figli.forEach((figlio) => {
        if (figlio.testo.toLowerCase().includes(stringaRicerca.toLowerCase()) || stringaRicerca == "") {
            let classi = "";
            x += `<li><a href="#" id-Sezione=${figlio.idSezione} tipo-apertura=${figlio.enum_TipoAperturaPagina} class="${classi}" onClick="redirect(event)"><span id-Sezione=${figlio.idSezione} tipo-apertura=${figlio.enum_TipoAperturaPagina}>${figlio.testo}</span></a></li>`
        }
    });
    return x
}

function creaSubMenu(menu, classeLI, classi, stringaRicerca) {
    //Genera l'elemento padre nella sidebar(quelli che quando vengono cliccati si espando)
    //Parametri, Menu un json che contiene il padre e il vari figli
    //classiLi, stringa che e' o vuota o "active". Serve per rendere "attivo" la sezione dove sono
    //classi, classi per l'icona

    htmlSubMenu = `<li idSezionePadre="${menu.idSezione}" class="sub-menu${classeLI}">
                <a href="javascript:void(0);"><i class="iconaMacro fa ${classi}"></i><span>${menu.testo}</span><i class="arrow fa fa-angle-down pull-down"></i></a>
                <ul>
                    ${generaFigli(menu.Figli, stringaRicerca)}
                </ul>
            </li>`

    return htmlSubMenu;

}


function rendiMenuClickabile() {
    //FUNZIONE DELLA XONNE, rende cliccabili e espandibili i bottoni nella sidebar
    $("#leftside-navigation .sub-menu > a").click(function (e) {

        // manage li class on open element
        $('#leftside-navigation .sub-menu').removeClass('open');
        $(this).parent().addClass('open');
        
        // manage icon rotation
        if ($(this).find("i").hasClass("flipIcon180")) {
            $("#leftside-navigation .sub-menu > a i").removeClass('flipIcon180');
        } else {
            $("#leftside-navigation .sub-menu > a i").removeClass('flipIcon180');
            $(this).find("i").addClass("flipIcon180");
        }

        $("#leftside-navigation ul ul").slideUp();
        // al termine dell'animazione rendo visibile il sottomenu se non lo è
        $(this).next().is(":visible") || $(this).next().slideDown({
            duration: 200,
            complete: function () {
                var $parentElement = $(this).parent();
                if ($parentElement.length && !isElementFullyVisible($parentElement.get(0))) {
                    $parentElement.get(0).scrollIntoView(false);
                }
            }
        });

        e.stopPropagation();
    });

}

function isElementFullyVisible(element) {
    var rect = element.getBoundingClientRect();
    return rect.top >= 0 && rect.bottom <= window.innerHeight;
}


async function rendiMenuAttivo(promiseBreadcrumb, promiseAlberoMenu) {
    //Serve a rendere il menu "attivo", esempio se sei in "Menu Visite", allora nella sidebar il padre(Audit, checklist, visite)
    //Deve essere bianco e l'icona rossa
    //Prende due promesse. Quella del breadcrumb serve per ottenere idSezionePadre, mentre quella del albero e' per essere sicuro che la sidebar sia pronta.

    let tab = localStorage.getItem(SIDE_MENU_TABS_KEY);
    if (tab != null) {
        openLeftsideTab(tab);
    }

    let data = await Promise.all([promiseBreadcrumb, promiseAlberoMenu]);
    let [idSezionePadre, statoAlbero] = data;

    let padre = document.querySelector(`[idsezionepadre="${idSezionePadre}"]`);
    if (padre == null)
        return;

    padre.classList.add("active");
}

function ricercaAlberoMenu(stringaRicerca) {
    //funzione che fa la ricerca su menus e menusPreferiti
    for (const elem of $(".searchSideBar")) {
        elem.value = stringaRicerca;
    }
    $("#leftsideTabcontentServizi > ul").empty();
    $("#leftsideTabcontentPreferiti > ul").empty();


    $("#leftsideTabcontentServizi > ul").append(dashboard);
    $("#leftsideTabcontentPreferiti > ul").append(configuraPreferiti);
    $("#leftsideTabcontentPreferiti > ul").append(dashboard);


    stringaRicerca = stringaRicerca.toLowerCase();
    menus.forEach((menu) => {
        copyStringaRicerca = stringaRicerca
        if (menu.testo.toLowerCase().includes(stringaRicerca)) {
            copyStringaRicerca = ""
        }
        if (copyStringaRicerca == ""  || menu.Figli.some(figlio => figlio.testo.toLowerCase().includes(copyStringaRicerca))) {
            let classi = ottieniClassi(menu.classeCssIcona);
            let classeLI = ""
            subMenu = creaSubMenu(menu, classeLI, classi, copyStringaRicerca);

            $("#leftsideTabcontentServizi > ul").append(subMenu);
        }
    });

    menusPreferiti.forEach((menu) => {
        copyStringaRicerca = stringaRicerca
        if (menu.testo.toLowerCase().includes(stringaRicerca)) {
            copyStringaRicerca = ""
        }
        if (copyStringaRicerca == ""  || menu.Figli.some(figlio => figlio.testo.toLowerCase().includes(copyStringaRicerca))) {
            let classi = ottieniClassi(menu.classeCssIcona);
            let classeLI = ""
            subMenu = creaSubMenu(menu, classeLI, classi, copyStringaRicerca);

            $("#leftsideTabcontentPreferiti > ul").append(subMenu);
        }
    });

    rendiMenuClickabile();

    //Parte che apre il menu se ne e' rimasto solo uno
    let idTabs = [["#leftsideTabServizi", "#leftsideTabcontentServizi"], ["#leftsideTabPreferiti", "#leftsideTabcontentPreferiti"]]
    idTabs.filter(idTab => document.querySelector(idTab[0]).classList.contains("active")).forEach(idTab => {
        let figli = document.querySelectorAll(`${idTab[1]} > .nano-content > .sub-menu`);
        if (figli.length == 1 && (["none", ""].includes(figli[0].querySelector("ul").style.display ) )) { //Se c'e' un solo figlio E non e' gia aperto, allora lo apro.
            figli[0].querySelector("a").click();
        }
    })

}


async function GetAlberoMenu(appendDashboard = true, appendConfigurazionePreferiti = true) {
    //funzione che prende AlberoMenu e li mette nella sidebar
    let risp = null;
    try {


        let promise = new Promise((resolve, reject) => {
            ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/AlberoMenu",
                null,
                function (risposta) {
                    risp = risposta.RispostaStringa;
                    menus = risp.Menus
                    menusPreferiti = risp.MenusPreferiti

                    if (appendDashboard) {
                        $("#leftsideTabcontentServizi > ul").append(dashboard);
                        if (appendConfigurazionePreferiti) {
                            $("#leftsideTabcontentPreferiti > ul").append(configuraPreferiti);
                        }
                        $("#leftsideTabcontentPreferiti > ul").append(dashboard);
                    } else if (appendConfigurazionePreferiti) {
                        $("#leftsideTabcontentPreferiti > ul").append(configuraPreferiti);
                    }

                    menus.forEach((menu) => {
                        let classi = ottieniClassi(menu.classeCssIcona)
                        let classeLI = ""

                        subMenu = creaSubMenu(menu, classeLI, classi, "");

                        $("#leftsideTabcontentServizi > ul").append(subMenu);
                    });



                    menusPreferiti.forEach((menu) => {
                        let classi = ottieniClassi(menu.classeCssIcona)
                        let classeLI = ""
                        if (breadcrum_Info.includes("_")) {
                            if (compareString(breadcrum_Info.split("_")[0], menu.testo)) {
                                classeLI = " active"
                            }
                        }
                        subMenu = creaSubMenu(menu, classeLI, classi, "");

                        $("#leftsideTabcontentPreferiti > ul").append(subMenu);
                    });


                    rendiMenuClickabile();
                    resolve(true);
                }, null, false, null)
        });



        return promise;

    } catch (e) {

    }



}
function ottieniLinkFuoriAgenda(idSezione) {
    return new Promise((resolve, reject) => {
        if (primaRichiesta) {
            primaRichiesta = false;
            resolve(menu_url + "&IDSezione=" + idSezione);
        } else {
            ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/ottieniUnidPerRedirect",
                `piva=${PIVACORRENTE}`,
                function (risposta) {
                    resolve(risposta.RispostaStringa + "&IDSezione=" + idSezione);
                }, null, false, null);
        }
    });
}

function linkPerIlRedirect(idSezione) {
    if (menu_corews) {
        return menu_url + "&IDSezione=" + idSezione
    } else {
        return pathMenuWS + "/salvaTitoloSezioneConGestioneRedirect"
    }
}

async function redirect(event) {
    const APRI_IN_FINESTRA_CORRENTE = 0
    const APRI_IN_NUOVA_FINESTRA = 1
    const APRI_IN_POPUP = 2
    const APRI_IN_KENDO_WINDOW = 3
    try {
        let e = event || window.event;
        let target = e.target || e.srcElement;
        let idSezione = target.getAttribute("id-Sezione")
        let testo = target.innerText

        enum_TipoAperturaPagina = parseInt(target.getAttribute("tipo-apertura"))

        //Uhalid 23/02/23 Al momento della scrittura codice esistono 4 tipi di apertura
        //      0: apre nella scheda corrente
        //      1: Apre in una nuova scheda
        //      2: PopUp
        //      3: Kendo Window




        let parametriAgenda = JSON.stringify({
            IDSezione: idSezione
        });

        let linkFuoriAgenda = menu_url + "&IDSezione=" + idSezione;

        if (enum_TipoAperturaPagina == APRI_IN_FINESTRA_CORRENTE) {
            if (menu_corews) {
                window.location = linkFuoriAgenda
            } else {
                ajaxAgronica(pathMenuWS + "/salvaTitoloSezioneConGestioneRedirect", parametriAgenda, gestioneRedirect, null);
            }
        }

        const tipiDiAperturaSenzaNagivazione = [APRI_IN_NUOVA_FINESTRA, APRI_IN_POPUP, APRI_IN_KENDO_WINDOW]
        if (tipiDiAperturaSenzaNagivazione.includes(enum_TipoAperturaPagina)) {
            if (menu_corews) {
                //Uso il parametroi sidebar off per spegnere la navigazione nella nuova scheda che apro
                //Poi ci pensare il load nel menubs_2017 ad aprire in una nuova/popup/kendo
                window.location = linkFuoriAgenda + "&sidebar=off"
            } else {
                ajaxAgronica(pathMenuWS + "/salvaTitoloSezioneConGestioneRedirect", parametriAgenda, gestioneRedirect, null);
            }
        }




    }
    catch (ex) {
        console.log(ex);
    }


}


function redirectNG(paginaRichiesta) {

    try {
        if (menu_corews) {
            window.location = menu_url + "&IDSezione=" + paginaRichiesta;
        } else {
            ajaxAgronica(pathMenuWS + "/redirectGiasNG", "{paginaRichiesta : " + paginaRichiesta + "}", gestioneRedirect, null);
        }
    }
    catch (ex) { }

}

async function informazioniAssistenza() {
    //Ottengo le infromazioniAssistenza, e nascondo le sezioni di cui non ho informazioni
    let risp = null;
    try {



        ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/ottieniInformazioniAssistenza",
            null,
            function (risposta) {
                assistenza = risposta.RispostaStringa;
                //console.log(assistenza);
                cambiaHref_O_Nascondi("#mail", assistenza.email, "mailto:");
                cambiaHref_O_Nascondi("#phone", assistenza.tel, "tel:");
                cambiaHref_O_Nascondi("#whatsapp", assistenza.whatsapp, "https://wa.me/+");

                if (assistenza.whatsapp == "") {
                    $("#footerAssistenza").hide();
                }

            }, null, false, null);

        return risp;

    } catch (e) {

    }
}



function cambiaHref_O_Nascondi(idd, value, prefix) {
    if (value != "") {
        $(idd).attr("href", prefix + value);
    } else {
        $(idd).hide();
    }

}



//FINE SIDEBAR








//Header Azienda

async function OttieniUltimeAziendeSelezionate() {

    if (elencoAziendeSelezionate !== null && elencoAziendeSelezionate !== undefined)
        return;

    try {



        ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/ottieniUltimeAziendeSelezionate",
            null,
            function (risposta) {
                elencoAziendeSelezionate = risposta.RispostaStringa;
                //console.log(elencoAziendeSelezionate);


                rightIcon = '<td onClick="cambioAzienda(\'PIVA\',\'RAG_SOC\')" class="tdCheck text_align_right"> <img src="' + GiasBase_Domain + 'agronica/Styles/images/dashboard/checkbox/checkbox_off.svg" alt = "Non selezionato" width = "24" height = "24" class="check_off"> <span class="tooltiptext">Cambia azienda</span></td >'
                rightIconSelezionata = '<td  class="tdCheck text_align_right"><img src="' + GiasBase_Domain + 'agronica/Styles/images/dashboard/checkbox/checkbox_on.svg" alt="Selezionato" width="24" height="24" class="check_on"><span class="tooltiptext">Azienda selezionata</span></td>'



                elencoAziendeSelezionate.forEach(impresa => {
                    let element = '<tr> <td style="cursor: pointer"  onClick="cambioAzienda(\'PIVA\',\'RAG_SOC\')" class="tdCompanyName text_medium text_blue" > ';
                    impresa.ragioneSociale = impresa.ragioneSociale.replaceAll("'", "").replaceAll("\"", "")
                    element += impresa.ragioneSociale + '</td >';
                    //controllo se l'azienda dove sono ora e' presente nella ultime aziende selezionate, se si metto l'icona selezionato
                    if (impresa.partitaIva == PIVACORRENTE) {
                        element += rightIconSelezionata + '</tr>'
                    } else {
                        element += rightIcon + '</tr>'
                    }
                    element = element.replaceAll("PIVA", impresa.partitaIva).replaceAll("RAG_SOC", impresa.ragioneSociale);
                    $("#ultimeAziendeSelezionateTable > tbody").append(element);
                });

            }, null, false, null);

        return;





    } catch (e) {
        console.log("Errore UltimeAziendeSelezionate")
    }

}


function filtroAvanzato() {
    //Ottiene il link per ricerca
    let idSez = idSezioneDashBoard != 0 && idSezioneDashBoard != -1 ? idSezioneDashBoard : 0;
    try {
        GoToFiltrino(idSez);
    } catch (ex) {

    }
    //console.log(idSez);
    //$.ajax({
    //    type: 'POST',
    //    url: Menu_BS2017 + '/GetFiltroAziende',
    //    data: '{ "IDSezione": ' + idSez + ' }',
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json',
    //    async: true,
    //    success: function (r) {
    //        $("#ricercaAvanzata").attr("href", r.d);
    //        window.location.replace(r.d);
    //    }
    //});
}


async function RicercaAzienda(ricerca, tastoPremuto) {
    if (ricerca === "") {
        $("#cerca_aziende").empty();

    } else {
        queueChiamateRicercaAziende.forEach(chiamata => chiamata.abort());
        queueChiamateRicercaAziende = []
        let chiamataDaAggiungere = ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/ricercaAzienda",
            `stringaRicerca=${ricerca}`,
            function (risposta) {
                $("#ricercaAziendaBody").empty();
                elencoAziende = risposta.RispostaStringa;
                //console.log(risposta);

                rightIcon = '<td  onClick="cambioAzienda(\'PIVA\',\'RAG_SOC\')" class="tdCheck text_align_right"> <img src="' + GiasBase_Domain + 'agronica/Styles/images/dashboard/checkbox/checkbox_off.svg" alt = "Non selezionato" width = "24" height = "24" class="check_off"> <span class="tooltiptext">Cambia azienda</span></td >'
                rightIconSelezionata = '<td onClick="cambioAzienda(\'PIVA\',\'RAG_SOC\')" class="tdCheck text_align_right"><img src="' + GiasBase_Domain + 'agronica/Styles/images/dashboard/checkbox/checkbox_on.svg" alt="Selezionato" width="24" height="24" class="check_on"><span class="tooltiptext">Azienda selezionata</span></td>'



                elencoAziende.forEach(impresa => {
                    let element = '<tr style="cursor: pointer"  onClick="cambioAzienda(\'PIVA\',\'RAG_SOC\')"  class="testCSS"> <td class="tdCompanyName text_medium text_blue" > ';
                    impresa.ragioneSociale = impresa.ragioneSociale.replaceAll("'", "").replaceAll("\"", "")
                    element += impresa.ragioneSociale + '</td >';
                    if (impresa.partitaIva == PIVACORRENTE) {
                        element += rightIconSelezionata + '</tr>'
                    } else {
                        element += rightIcon + '</tr>'
                    }
                    element = element.replaceAll("PIVA", impresa.partitaIva).replaceAll("RAG_SOC", impresa.ragioneSociale)
                    $("#ricercaAziendaBody").append(element);
                });
                queueChiamateRicercaAziende = [];
            }, null);
        queueChiamateRicercaAziende.push(chiamataDaAggiungere);
    }

}


function cambioAzienda(piva, rag_soc) {
    //funzione chimata quando cambio azienda
    try {
        AggiornaAttivitaNavigazioneAziende(piva);
        gestioneCambioImpresa2(piva, rag_soc);
    } catch (ex) {

    }
}

function gestioneCambioImpresa2(PivaSelezionata, AziendaSelezionata) {
    if (menu_corews) {
        window.location = menu_url + "&IDSezione=" + idSezioneDashBoard + "&PivaSelezionata=" + PivaSelezionata;
    } else {
        ajaxAgronica(pathMenuWS + "/cambiaImpresaConGestioneRedirect", JSON.stringify({
            Piva: PivaSelezionata,
            Azienda: AziendaSelezionata,
            IDSezione: idSezioneDashBoard
        }), gestioneRedirect, null);
    }
}


function AggiornaAttivitaNavigazioneAziende(pivaa) {
    //Serve a tenere traccia delle ultime aziende selezionate.
    ajaxAgronicaApiCoreStdPostSync(pathCoreAPI + "/Menu/aggiornaAttivitaNavigazioneAziende",
        JSON.stringify({ piva: pivaa }),
        function (risposta) {
            risp = risposta.RispostaStringa;

        }, null, false, null);


}


//Fine Header Azienda




//Header Utenti
async function informazioniUtente() {

    let risp = null;
    try {

        ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/informazioniUtente",
            null,
            function (risposta) {
                risp = risposta.RispostaStringa;
                nome_finale = ""
                if (risp.Nome != "")
                    nome_finale += risp.Nome + " "
                if (risp.Cognome != "")
                    nome_finale += risp.Cognome + " "
                if (risp.Rag_Soc != "")
                    nome_finale += risp.Rag_Soc
                    
                $("#Nome_RagioneSociale").text(nome_finale + "\n\n");
                $("#Username_Commerciale").text("\n" + risp.UserNameCommerciale + "\n");
                $("#emailField").text("mail: " + risp.Email);
                $("#usernameField").text("username: " + risp.UserName);
                $("#visibilita").text(risp.Visibilita);
                $("#ultimoAccesso").text(risp.UltimoAccesso);


            }, null, false, null);

        return risp;

    } catch (e) {

    }


}



function impostaStatoLicenza() {  
    if (flagLicenzaScaduta) { // dati impostati dl AgronicaControlli_2010 UserControls/Headers/headerHelper.vb 
        $("#smsLicenza").css("display", "");
    }
}
//Fine Header Utenti





//BreadCrumb


function changeColorIfEmpty(color) {
    DEFAULT_COLOR = "#000000";
    if (color == null || color == undefined || color == "") {
        return DEFAULT_COLOR;
    }
    return color;
}

async function creaBreadCrumb() {
    $("#breadcrumb-menu > .item").remove();
    $("#breadcrumb-menu > .item-icon").remove();

    let breadcrumInfo = null;
    let testoPadre = null;
    let testoFiglio = null;
    let colore = null;
    let cssIcona = null;
    let idSezionePadre = -1;


    if (idSezioneDashBoard > 0) {
        let promise = new Promise((resolve, reject) => {
            ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/InformazioniBreadcrumbs",
                `Id_Sezione=${idSezioneDashBoard}`,
                function (risposta) {
                    let risp = null;
                    risp = risposta.RispostaStringa;
                    //console.log(risp);
                    resolve(risp);
                }, null, false, null);
        }).then((value) => { breadcrumInfo = value });
        await promise
        testoPadre = breadcrumInfo.TestoPadre;
        testoFiglio = breadcrumInfo.TestoFiglio;

        if (breadcrum_Info !== undefined && breadcrumInfo !== '')
        {
            if (breadcrum_Info.includes("_"))
            {
                splittedText = breadcrum_Info.split("_");
                testoFiglio = splittedText[1];
            }
        }

        colore = changeColorIfEmpty(breadcrumInfo.Colore);
        cssIcona = ottieniClassi(breadcrumInfo.ClasseCssIcona);
        idSezionePadre = breadcrumInfo.IDSezionePadre;
        style = `background-color: ${colore} !important;`;
    } else if (idSezioneDashBoard == -1 && breadcrum_Info.includes("_")) {
        //In alcuni casi particolari, per esempio le pagine "figlie" tipo apri modifica da un altra pagina, il testo arriva direttamente dallo usercontrol e non dal api.

        splittedText = breadcrum_Info.split("_");
        testoPadre = splittedText[0];
        testoFiglio = splittedText[1];

        let promise = new Promise((resolve, reject) => {
            ajaxAgronicaApiCoreStdGetAsync(pathCoreAPI + "/Menu/ottieniIconaDaTesto",
                `testo=${testoPadre}`,
                function (risposta) {
                    let risp = null;
                    risp = risposta.RispostaStringa;
                    //console.log(risp);
                    resolve(risp);
                }, null, false, null);
        }).then((value) => { breadcrumInfo = value });
        await promise;
        colore = changeColorIfEmpty(breadcrumInfo.Colore);
        cssIcona = ottieniClassi(breadcrumInfo.ClasseCssIcona);
        idSezionePadre = breadcrumInfo.idSezionePadre;
        style = `background-color: ${colore} !important;`;
    } else if (idSezionePadre == -1 && breadcrum_Info.includes("~")) {
        //In altri casi, potrei, attualmente, non avere il testo padre. Esempio apertura di un link dai widget in angular.
        testoFiglio = breadcrum_Info.split("~")[1];
        colore = "#000000";
    }

    if (testoPadre != "" && testoPadre != null) {

        var iconaB = `<div class="item-icon" >
                            <i class="fa fa_gias_colors ${cssIcona}" style="${style}"></i>
                        </div>`

        var padre = `<div class="item double_arrow">
            <span class="dropdown-el input-border-area-product  dropdown-el-disabled inactiveLink" >
                <div class="select-selected" style="border-left: 3px solid !important; border-color: ${colore} !important" title="${testoPadre}" >
                    ${iconaB}
                    <span class="tooltiptext">${testoPadre}</span>
                    <span class="breadcrumbs-info">${testoPadre}</span>
                </div>
            </span>
        </div>`
        $("#breadcrumb-menu").append(padre);

    }

    if (testoFiglio != "" && testoFiglio != null) {
        var figlio = `<div class="item">
                <span class="dropdown-el input-border-area-product dropdown-el-disabled inactiveLink">
                    <div class="select-selected" style="border-color: ${colore} !important" title="${testoFiglio}">
                        <span class="tooltiptext">${testoFiglio}</span>
                        <span class="breadcrumbs-info">${testoFiglio}</span>
                    </div>
                </span>
            </div>`
        $("#breadcrumb-menu").append(figlio);
    }

    if (azioneIndietro != "") {
        var backButton = `<a class="hIcon overbtn" onclick="${azioneIndietro}" "href = "#" >
                                <div class="hBackContent icon-back"></div>
                                    <span class="tooltiptext">Indietro</span>
                              </a>`
        $("#breadcrumb-menu").append(backButton);

    }

    return new Promise((resolve, reject) => {
        resolve(idSezionePadre);
    });
}

