<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="headerMenuUC.ascx.vb" Inherits="AgroAgenda_2010.headerMenuUC" %>


 <%--<link href="./assets/css/font-awesome.css" rel="stylesheet">--%>
 <link href="<%= ResolveClientUrl("~/Master/header/assets/css/font-awesome.css").ToString %>" rel="stylesheet">
<link href="<%= ResolveClientUrl("~/Master/header/assets/css/headers.css").ToString %>" rel="stylesheet">
<script type="text/javascript">
    if (navigator.userAgent.includes("Safari") && !navigator.userAgent.includes("Chrome")) {
        var head  = document.getElementsByTagName('head')[0];
        var link  = document.createElement('link');
        link.id   = 'safari-css';
        link.rel  = 'stylesheet';
        link.type = 'text/css';
        link.href = '<%= ResolveClientUrl("~/Master/header/assets/css/safari.css").ToString %>';
        link.media = 'all';
        head.appendChild(link);
    }
</script>

<header>
    <div id="headerColLeft" class="headerCol-2">
        <div id="hContentBars">
            <a class="menuBars " onclick="openNav();" id="openMenu"><i class="fa fa-bars" aria-hidden="true"></i></a>
            <a class="menuBars hide" onclick="closeNav();" id="closeMenu"><i class="fa fa-bars" aria-hidden="true"></i></a>
        </div>
        <div id="hContentLogo">
            <a class="linkLogo" href="/">
                <img src="<%= ResolveClientUrl("~/Master/header/assets/images/navbar/logo_agronica.svg").ToString %>" alt="Agronica" width="34" height="34" class="logo_navbar">
            </a>
        </div>
    </div>
    <div id="headerColCenter" class="headerCol-7">
        &nbsp;     
        <div id="breadcrumb-menu" class="menuGias">
            <div class="item-icon">
                <i class="fa fa_gias_colors fa_gias_qualita_tracciabilita"></i>
            </div>
            <div class="item">
                <span class="dropdown-el input-border-area-product dropdown-el-disabled">
                    <select name="step1" class="breadSelect" disabled>
                        <option value="test1" selected>Qualità e tracciabilità</option>
                    </select>
                </span>
            </div>
            <div class="item">
                <span class="dropdown-el input-border-area-product">
                    <select name="step2" class="breadSelect">
                        <option value="test1" selected>Gestione Laboratori</option>
                        <option value="lab1">Lab 1</option>
                        <option value="lab2">Lab 2</option>
                    </select>
                </span>
            </div>
            <div class="item">
                <span class="dropdown-el input-border-area-product">
                    <select name="step3" class="breadSelect">
                        <option value="test1" selected>Piani Produttivi</option>
                        <option value="test2">Gestione Analisi</option>
                    </select>
                </span>
            </div>
            <div class="item">
                <span class="dropdown-el input-border-area-product">
                    <select name="step4" class="breadSelect">
                        <option value="test1" selected>Test 1</option>
                        <option value="test2">Test 2</option>
                    </select>
                </span>
            </div>

            <div class="item">
                <span class="dropdown-el input-border-area-product">
                    <select name="step4" class="breadSelect">
                        <option value="test1" selected>Test 1</option>
                        <option value="test2">Test 2</option>
                    </select>
                </span>
            </div>

        </div>
    </div>
    <div id="headerColRight" class="headerCol-3">
        <div id="navbar_company" class="hRightElement dropdown-container hContainerCompany">
            <a onclick="showToggleDropdown('hCompaniesDrop');" class="dropbtn">

                <span id="Azienda2" runat="server" class="hCurrentCompanyName text_align_right"> <span class="tooltiptext">ABOCA S.P.A. SOCIET&Agrave; AGRICOLA - SANSEPOLCRO</span>  </span>
                <i class="fa fa-chevron-down" aria-hidden="true"></i>
            </a>
            <div id="hCompaniesDrop" class="dropdown-content">
                <span class="dropTitleRed text_red label_text_small text_uppercase">Ultime aziende gestite:</span>
                <table class="hCompanyTableSelect">
                    <tr>
                        <td class="tdCompanyName text_medium text_blue">ABOCA S.P.A. SOCIETA AGRICOLA - SANSEPOLCRO</td>
                        <td class="tdCheck text_align_right">
                            <img src="<%= ResolveClientUrl("~/Master/header/assets/images/checkbox/checkbox_on.svg").ToString %>" alt="Selezionato" width="24" height="24" class="check_on"><span class="tooltiptext">Azienda selezionata</span></td>
                    </tr>
                    <tr>
                        <td class="tdCompanyName text_medium text_blue">ABOCA S.P.A. SOCIETA AGRICOLA - UZIEWIZ</td>
                        <td class="tdCheck text_align_right">
                            <img src="<%= ResolveClientUrl("~/Master/header/assets/images/checkbox/checkbox_off.svg").ToString %>" alt="Non selezionato" width="24" height="24" class="check_off"><span class="tooltiptext">Cambia azienda</span></td>
                    </tr>
                    <tr>
                        <td class="tdCompanyName text_medium text_blue">ABOCA S.P.A. SOCIETA AGRICOLA - XYZ</td>
                        <td class="tdCheck text_align_right">
                            <img src="<%= ResolveClientUrl("~/Master/header/assets/images/checkbox/checkbox_off.svg").ToString %>" alt="Non selezionato" width="24" height="24" class="check_off"><span class="tooltiptext">Cambia azienda</span></td>
                    </tr>
                    <tr>
                        <td class="tdCompanyName text_medium text_blue">ABOCA S.P.A. SOCIETA AGRICOLA - TEST</td>
                        <td class="tdCheck text_align_right">
                            <img src="<%= ResolveClientUrl("~/Master/header/assets/images/checkbox/checkbox_off.svg").ToString %>" alt="Non selezionato" width="24" height="24" class="check_off"><span class="tooltiptext">Cambia azienda</span></td>
                    </tr>
                </table>
                <hr>
                <span class="dropTitleRed text_red label_text_small text_uppercase">Ricerca veloce:</span>
                <div class="hBoxSearch">
                    <div class="container-search">
                        <span class="icon"><i class="fa fa-search"></i></span>
                        <input type="search" id="search" placeholder="Cerca Azienda..." />
                    </div>
                </div>
                <hr>
                <div class="hInternalLink">
                    <span class="dropTitleRed text_small text_red text_uppercase text_align_center"><a href="#" class="text_align_center ic_arrow navbar_ic_arrow_right_type02">Ricerca avanzata</a></span>
                </div>
            </div>
        </div>

        <div id="navbar_favorite" class="hRightElement hContainerFavorite">
            <a class="hIcon overbtn" href="#">
                <div class="hFavoriteContent icon-favorite-edit"></div>
                <span class="tooltiptext">Modifica i tuoi preferiti</span>
            </a>
            <!-- To be showed instead the previous anchor tag when this routing is in place -->
            <!-- <a class="hIcon overbtn" href="#">
                <div class="hFavoriteContent icon-favorite-edit-active"></div>
            </a> -->
        </div>

        <div id="navbar_notice" class="hRightElement hContainerBell">
            <a onclick="showToggleDropdown('hNotificationDrop');" class="dropbtn notification">
                <i class="fa fa-bell" aria-hidden="true"></i>
                <span class="badge text_align_center label_text_small">1</span>
            </a>
            <div id="hNotificationDrop" class="dropdown-content">
                <div class="hnText">
                    <p class="text_medium text_align_left text_gray_dark2">Lorem Ipsum è un testo segnaposto utilizzato nel settore della tipografia e della stampa. Lorem Ipsum è considerato il testo segnaposto standard sin dal sedicesimo secolo, quando un anonimo tipografo prese una cassetta di caratteri e li assemblò per preparare un testo campione.</p>
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
                <a href="#" class="goToUserProfile ic_arrow_small navbar_ic_arrow_right_type01">
                    <span id="Nome_RagioneSociale" runat="server" class="hUserProfileName text_blue label_text_normal text_weight_400"></span>
                    <span id="emailField" runat="server" class="hUserProfileEmail text_gray_light3 text_small text_weight_400"></span>
                </a>
                <hr />
                <div class="hUserVisibility">
                    <div class="label_text_small text_gray_light3">VISIBILIT&Agrave;:</div>
                    <div id="visibilita" runat="server" class="text_small text_blue"></div>
                </div>
                <div class="hUserVisibility">
                    <div  class="label_text_small text_gray_light3">ULTIMO ACCESSO:</div>
                    <div id="ultimoAccesso" runat="server" class="text_small text_blue"></div>
                </div>
                <hr />
                <div class="hIconLink">
                    <div class="hIcon text_align_left"><a href="#" class="text_align_left"><i class="fa fa-pencil-square-o" aria-hidden="true"></i></a></div>
                    <div class="hIconText text_medium text_blue text_align_left"><a href="#">Gestione Profilo</a></div>
                </div>
                <hr />
                <div class="hIconLink">
                    <div class="hIcon text_align_left"><a href="#" class="text_align_left"><i class="fa fa-sign-out" aria-hidden="true"></i></a></div>
                    <div class="hIconText text_medium text_blue text_align_left"><a href="#">Esci</a></div>
                </div>
            </div>
        </div>
    </div>
</header>

<%--<script src="https://code.jquery.com/jquery-1.12.4.min.js" integrity="sha256-ZosEbRLbNQzLpnKIkEdrPv7lOy9C27hHQ+Xp8a4MxAQ=" crossorigin="anonymous"></script>--%>
<script src="<%= ResolveClientUrl("~/Master/header/assets/js/breadcrumbSelector.min.js").ToString %>"></script>
<script src="<%= ResolveClientUrl("~/Master/header/assets/js/script.js").ToString %>""></script>
<script src="<%= ResolveClientUrl("~/Master/header/headerMenuUC_ws_client.js").ToString %>""></script>
<script src="<%= ResolveClientUrl("~/Master/header/headerMenuUC_jQueryDocReady.js").ToString %>""></script>
