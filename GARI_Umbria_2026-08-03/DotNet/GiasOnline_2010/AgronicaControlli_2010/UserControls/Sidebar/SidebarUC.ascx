<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SidebarUC.ascx.vb" ClassName="SidebarUC" Inherits="AgronicaControlli_2010.SidebarUC" %>

<div class="container container-gias-general" id="main-container">
    <div id="GiasSidenav" class="sidenav bg_blue_nav">
        <aside class="sidebar">
            <div id="leftside-header">
                <div class="leftside-header-row">
                    <div class="leftside-header-tabs">
                        <div id="leftsideTabPreferiti" class="leftside-header-tab active" onclick="openLeftsideTab('preferiti')">
                            REPLACE_MIEI_PREFERITI
                        </div>
                        <div id="leftsideTabServizi" class="leftside-header-tab" onclick="openLeftsideTab('servizi')">
                            REPLACE_TUTTI_SERVIZI
                        </div>
                    </div>
                </div>
                <div class="container-search">
                    <span class="icon"><i class="fa fa-search" aria-hidden="true"></i></span>
                    <input autocomplete="off" type="text"  onkeyup="ricercaAlberoMenu(this.value)" id="searchService" class="searchSideBar" placeholder="REPLACE_CERCA_PLACEHOLDER">
                </div>
            </div>
            <div id="leftside-navigation" class="nano">

                <!-- START "i miei preferiti" tab content -->
                <div id="leftsideTabcontentPreferiti" class="leftside-tabcontent">
                    <%--<div class="container-search">
                        <span class="icon"><i class="fa fa-search"></i></span>
                        <input autocomplete="off" type="text"  onkeyup="ricercaAlberoMenu(this.value)" id="searchService" class="searchSideBar" placeholder="REPLACE_CERCA_PLACEHOLDER">
                    </div>--%>

                    <ul class="nano-content">
                    </ul>
                </div>
                <!-- END "i miei preferiti" tab content -->

                <!-- START "tutti i servizi" tab content -->
                <div id="leftsideTabcontentServizi" class="leftside-tabcontent hide">
                    <%--<div class="container-search">
                        <span class="icon"><i class="fa fa-search"></i></span>
                        <input autocomplete="off" type="text" onkeyup="ricercaAlberoMenu(this.value)" id="searchService" class="searchSideBar" placeholder="REPLACE_CERCA_PLACEHOLDER">
                    </div>--%>

                    <ul class="nano-content">
                    </ul>
                </div>
                <!-- END "tutti i servizi" tab content -->
            </div>
            <div id="leftside-footer">

                <div class="row-footer">
                    <div class="text-footer">
                        <span>REPLACE_CONTATTACI</span>
                        <a href="" id="mail" class="footer-link-mail">&nbsp;</a>
                        <a href="" id="phone" class="footer-link-phone">&nbsp;</a>
                    </div>
                </div>

                <div class="row-footer" id="footerAssistenza">
                    <div class="text-footer">
                        <span>REPLACE_ASSISTENZA</span>
                        <a href="" id="whatsapp" class="footer-link-whatsapp">&nbsp;</a>
                    </div>
                </div>

            </div>
        </aside>
    </div>

</div>
