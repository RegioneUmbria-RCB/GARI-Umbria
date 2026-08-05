<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="headerMenuUC.ascx.vb" ClassName="headerMenuUC" Inherits="AgronicaControlli_2010.headerMenuUC" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc2" %>

<link href="__basePath__agronica/Header/assets/font-awesome-4.7/css/font-awesome.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet">
<link href="__basePath__agronica/Styles/headers.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet">
<script type="text/javascript">

    var linkHref = "__basePath__agronica/Styles/headers.css?<% =Application("GiasVersioneCorrente")%>";

    var pathCoreAPI = '__pathCoreAPI__';
    var idSezioneDashBoard = 'REPLACE_IDSezione';
    var GiasBase_Domain = '__basePath__';
    var AZIENDACORRENTE = "AZIENDA_CORRENTE";
    var PIVACORRENTE = "PIVA_CORRENTE";
    var Menu_BS2017 = "REPLACE_Menu_BS17";
    var breadcrum_Info = "REPLACE_breadcrum_Info";
    var azioneIndietro = "REPLACE_azioneIndietro";
    var idIndietro = "REPLACE_idIndietro";
    var flagLicenzaScaduta = REPLACE_FLAG_LICENZA;
    if (navigator.userAgent.includes("Safari") && !navigator.userAgent.includes("Chrome")) {
        var head = document.getElementsByTagName('head')[0];
        var link = document.createElement('link');
        link.id = 'safari-css';
        link.rel = 'stylesheet';
        link.type = 'text/css';
        link.href = linkHref;
        link.media = 'all';
        head.appendChild(link);
    }
</script>

<script language="javascript" type="text/javascript">

    function Esci_Da_Gias_Centralizzato() {

        var dlg = $("<div></div>").kendoConfirm({
            content: TraduzioneMultiResx(headerResx, "ConfermaUscitaGias", "Sei sicuro di voler uscire?"),
            messages: { okText: TraduzioneMultiResx(headerResx, "Sì", "Sì"), cancel: Traduzione(headerResx, "No", "No") },
            title: TraduzioneMultiResx(headerResx, "Conferma", "Conferma")
        }).data("kendoConfirm");

        dlg.result.done(function () { LogoutGestione(); });
        dlg.open();

    }
</script>

<style type="text/css">
    .hidePanelRigaTitolo {
        display: none;
    }
</style>

<header id="headerDashboard">
    <div id="headerColLeft" class="headerCol-2">
        <div id="hContentBars">
            <a class="menuBars " onclick="openNav();" id="openMenu"><i class="fa fa-bars" aria-hidden="true"></i></a>
            <a class="menuBars hide" onclick="closeNav();" id="closeMenu"><i class="fa fa-bars" aria-hidden="true"></i></a>
        </div>
        <div id="hContentLogo">
            <a class="linkLogo" onclick="redirectNG(999)">
                <span class="tooltiptext" id="testoLogo">REPLACE_TORNA_ALLA_DASHBOARD</span>
                <img src="PATH_LOGO" id="logoGias" width="34" height="34" class="logo_navbar" alt="">
            </a>
        </div>
    </div>
    <div id="headerColCenter" class="headerCol-7">
        &nbsp;     
        <div id="breadcrumb-menu" class="menuGias">
        </div>
    </div>
    <div id="headerColRight" class="headerCol-3">
        <div id="navbar_company" class="hRightElement dropdown-container hContainerCompany">
            <a id="aNavbarCompany" onclick="showToggleDropdown('hCompaniesDrop');" class="dropbtn">

                <span class="hCurrentCompanyName text_align_right">AZIENDA_CORRENTE<span class="tooltiptext">AZIENDA_CORRENTE</span> </span>
                <i class="fa fa-chevron-down" aria-hidden="true"></i>
            </a>
            <div id="hCompaniesDrop" class="dropdown-content">
                <span class="dropTitleRed text_red label_text_small text_uppercase">REPLACE_ULTIME_AZIENDE_GESTITE</span>
                <table id="ultimeAziendeSelezionateTable" class="hCompanyTableSelect" aria-hidden="true">
                    <tbody>
                    </tbody>
                </table>
                <hr>
                <span class="dropTitleRed text_red label_text_small text_uppercase">REPLACE_RICERCA_VELOCE</span>
                <div class="hBoxSearch">
                    <div class="container-search">
                        <span class="icon"><i class="fa fa-search" aria-hidden="true"></i></span>
                        <input autocomplete="off" type="text" id="search" onkeydown="if (event.keyCode == 13) return false;" onkeyup="if (event.keyCode == 13 || this.value.length >= 3) RicercaAzienda(this.value); else $('#ricercaAziendaBody').empty();" placeholder="REPLACE_CERCA_AZIENDA..." />
                    </div>
                </div>
                <table id="ricercaAziendaTable" class="hCompanyTableSelect" aria-hidden="true">
                    <tbody class="company-table-select-max-height" id="ricercaAziendaBody">

                    </tbody>
                </table>
                <hr>
                <div class="hInternalLink">
                    <span class="dropTitleRed text_small text_red text_uppercase text_align_center"><a href="#" onclick="filtroAvanzato()" id="ricercaAvanzata" class="text_align_center ic_arrow navbar_ic_arrow_right_type02">REPLACE_RICERCA_AVANZATA</a></span>
                </div>
            </div>
        </div>

        <div id="navbar_notice" class="hRightElement hContainerBell" style="display: none">
            <a onclick="showToggleDropdown('hNotificationDrop');" class="dropbtn notification">
                <i class="fa fa-bell" aria-hidden="true"></i>
                <span class="badge text_align_center label_text_small">1</span>
            </a>
            <div id="hNotificationDrop" class="dropdown-content">
                <div class="hnText">
                    <p class="text_medium text_align_left text_gray_dark2">Agronica</p>
                </div>
                <div class="hBtnLink text_align_left">
                    <a href="#" class="text_red text_uppercase text_small ic_arrow navbar_ic_arrow_right_type02">Vai alla notifica
                    </a>
                </div>
            </div>
        </div>

        <div id="navbar_user" class="hRightElement dropdown-container hFloatRight hContainerUser">
            <a onclick="showToggleDropdown('userAccount');" class="dropbtn"><i class="fa fa-chevron-down" aria-hidden="true"></i><i class="fa fa-user-circle-o" aria-hidden="true"></i></a>
            <div id="userAccount" class="dropdown-content">
                <a href="#" class="goToUserProfile inactiveLink">
                    <span id="Nome_RagioneSociale" class="hUserProfileName text_blue label_text_normal text_weight_400" style="display: block;">

                    </span>
                    <span id="Username_Commerciale" class="hUserProfileName text_blue label_text_normal text_weight_400" style="display: block;">
                    </span>
                    <hr />
                    <span id="emailField" class="hUserProfileEmail text_gray_light3 text_small text_weight_400"></span>
                    <hr />
                    <span id="usernameField" class="hUserProfileEmail text_gray_light3 text_small text_weight_400"></span>
                </a>
                <hr />
                <div class="hUserVisibility">
                    <div class="label_text_small text_gray_light3">REPLACE_VISIBILITA</div>
                    <div id="visibilita" class="text_small text_blue"></div>
                </div>
                <div class="hUserVisibility">
                    <div class="label_text_small text_gray_light3">REPLACE_ULTIMO_ACCESSO</div>
                    <div id="ultimoAccesso" class="text_small text_blue"></div>
                </div>
                <hr />
                <div class="hIconLink" style="display: none;">
                    <div class="hIcon text_align_left"><a href="#" class="text_align_left"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></div>
                    <div class="hIconText text_medium text_blue text_align_left"><a href="#">REPLACE_GESTIONE_PROFILO</a></div>
                </div>
                <div id="smsLicenza" class="hIconLink" style="display: none;">
                    <div class="hIconText text_medium text_red text_align_left"><a href="#" style="display: inline-block;pointer-events: none;">REPLACE_TESTO_LICENZA_SCADUTA</a></div>
                </div>
                <%--<hr />--%>
                <div class="hIconLink">
                    <div class="hIcon text_align_left"><a href="#" class="text_align_left"><i class="fa fa-sign-out" aria-hidden="true"></i></a></div>
                    <div class="hIconText text_medium text_blue text_align_left"><a id="pulsanteEsci" href="#" onclick="Esci_Da_Gias_Centralizzato();">REPLACE_ESCI</a></div>
                </div>
            </div>
        </div>
    </div>
</header>
<cc2:sidebarHelper id="sideBar" runat="server"></cc2:sidebarHelper>
<script src="__basePath__agronica/Scripts/AgronicaControlli_2010/Header/script.js?<% =Application("GiasVersioneCorrente")%>"></script>
<script src="__basePath__agronica/Scripts/AgronicaControlli_2010/Header/headerMenuUC.js?<% =Application("GiasVersioneCorrente")%>"></script>
<script src="__basePath__agronica/Scripts/AgronicaControlli_2010/Header/headerMenuUC_ws_client.js?<% =Application("GiasVersioneCorrente")%>"></script>
<script src="__basePath__agronica/Scripts/AgronicaControlli_2010/Header/breadcrumbSelector.js?<% =Application("GiasVersioneCorrente")%>"></script>
<script id="menu_script"></script>
<br />
<br />

