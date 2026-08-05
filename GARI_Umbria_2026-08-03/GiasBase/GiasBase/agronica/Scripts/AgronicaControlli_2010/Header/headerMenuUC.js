const SpecialNavigation = Object.freeze({
    None: '0',
    OnIframe: '1',
    OnNewWindow: '2'
});

var headerResx = [];
var resxArrPathHeader = [
    //"App_GlobalResources/AgronicaAgenda_2010.resx",
    "AgronicaCoreDataProvider.dll/AgronicaCoreDataProvider.Gias"
];


async function asyncLoad() {
    loadHeaderResources();

    //Il comportamento della pagina cambia in base a se sono in una pagina in cui la navigazione e' attiva
    //o se la header e' attiva, per esempio i kendopopup/modali bootstrap dentro le pagine hanno la header spenta
    let specialNavigationCookie = getCookie("AgronicaSpecialNavigation");
    if (!Object.values(SpecialNavigation).some(n => n === specialNavigationCookie)) {
        specialNavigationCookie = SpecialNavigation.None;
    }

    switch (specialNavigationCookie) {
        case SpecialNavigation.OnIframe:
            if (!isActive("sidebar")) { //Casistica in cui da Demetra apro un secondo tab di gias su cui viene disattivata anche la sidebar
                $("#headerDashboard").hide();
                return;
            }

            let promiseBreadcrumb = creaBreadCrumb();
            let promiseAlberoMenu = GetAlberoMenu(false, true);

            rendiMenuAttivo(promiseBreadcrumb, promiseAlberoMenu);
            //promiseBreadcrumb.then((value) => $("#breadcrumb-menu > a").hide());
            $("#headerColRight").hide();
            //$("#headerColCenter").hide();
            $("#hContentLogo").hide();
            $("#leftside-footer").hide();

            break;
        case SpecialNavigation.OnNewWindow:
            if (!isActive("sidebar")) {
                $("#headerDashboard").hide();
                return;
            }

            let breadcrumb = creaBreadCrumb();
            let alberoMenu = GetAlberoMenu(false, true);

            rendiMenuAttivo(breadcrumb, alberoMenu);

            $("#leftside-footer").hide();
            $("#navbar_user").hide();
            document.getElementById('aNavbarCompany').onclick = function () { return; }
            document.querySelector('i.fa-chevron-down').hidden = true;

            break;
        case SpecialNavigation.None:
        default:
            if (isActive("header") && isActive("modal", "true")) {
                let promiseBreadcrumb = creaBreadCrumb();
                informazioniUtente();
                impostaStatoLicenza();

                if (isActive("sidebar")) {
                    let promiseAlberoMenu = GetAlberoMenu();
                    OttieniUltimeAziendeSelezionate();
                    informazioniAssistenza();
                    rendiMenuAttivo(promiseBreadcrumb, promiseAlberoMenu);
                } else {
                    //Se la navigazione e' spenta, nascondo la sidebar, e cambio il testo di un paio di cose, si da per 
                    //scontanto che se la navigazione e' spenta vuol dire che e' il secondo tab aperto
                    $("#hContentBars").css('visibility', 'hidden');
                    $("#navbar_company").css('pointer-events', 'none');
                    $("#navbar_company i").hide();
                    $("#navbar_company a").unbind();

                    $("#testoLogo").text("Chiudi Finestra");
                    $("#pulsanteEsci").text("Chiudi Finestra");

                    $("#hContentLogo .linkLogo").attr("onclick", "window.close()");
                    $("#pulsanteEsci").attr("onclick", "window.close()");
                }
            } else {
                $("#headerDashboard").hide();
            }

            break;
    }
}

function ottieniValoreQueryString(nomePametro) {
    try {
        let params = (new URL(document.location)).searchParams;
        let value = params.get(nomePametro);
        return value;
    }
    catch (ex) {
        return "";
    }
}

function isActive(queryValue, falseValue = "off") {
    return ottieniValoreQueryString(queryValue) != falseValue;
}

function loadHeaderResources() {
    if (Array.isArray(resxArrPathHeader) && resxArrPathHeader.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPathHeader.forEach(function (resxSinglePath) {
            headerResx.unshift(loadHeaderResxFile(resxSinglePath));
        });
    }
}

function loadHeaderResxFile(resxToRead) {
    var resxObj = null;
    let readEnpoint = pathCoreWS + "Localization.asmx/CaricaFileRisorse";
    let params = JSON.stringify(
        {
            objP_Server: objP_server,
            objP_Utenti: objP_utenti,
            files: resxToRead
        });

    ajaxAgronicaSync(readEnpoint, params, false,
        function (risposta) {
            try {
                resxObj = JSON.parse(risposta.RispostaStringa);
            } catch (e) {
            }

        }, function (risposta) {
            //if (typeof risposta === "string") {
            //    // In questo caso ho un errore js...
            //} else {
      
            //}
    }, undefined, false);


    return resxObj;
}

$(document).ready(function () {
    asyncLoad();
    document.addEventListener("click", function (evnt) {
        //Serve a chiudere i menu in automatico quando premo fuori
        const element = event.target;
        //Both false or both true, close the nav
        if (isActive("sidebar") && (element.closest('#GiasSidenav') != null) === (element.closest('#openMenu') != null)) {
            closeNav();
        }

        const companyDrop = element.closest('#navbar_company');
        if (isActive("sidebar") && companyDrop == null && document.getElementById('hCompaniesDrop').classList.contains('show')) {
            showToggleDropdown('hCompaniesDrop');
        }

        const userDrop = element.closest('#navbar_user');
        if (userDrop == null && document.getElementById('userAccount').classList.contains('show')) {
            showToggleDropdown('userAccount');
        }
    });
});