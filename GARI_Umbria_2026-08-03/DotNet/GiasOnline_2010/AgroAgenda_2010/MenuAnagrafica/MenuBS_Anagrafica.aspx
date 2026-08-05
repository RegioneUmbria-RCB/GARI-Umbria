<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="MenuBS_Anagrafica.aspx.vb" Inherits="AgroAgenda_2010.MenuBS_Anagrafica" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>
<%@ Register TagPrefix="ucProd" TagName="Prodotto_Edit_UC" Src="~/Anagrafica/UserControl/Prodotto_Edit_UC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- PER ALBERO NUOVO -->
    <asp:PlaceHolder ID="gisHeader" runat="server"></asp:PlaceHolder>

    <link rel="stylesheet" href="<%= ResolveClientUrl("MenuBS_Anagrafica.css?" & Application("GiasVersioneCorrente").ToString) %>" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!--include per l'albero-->
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.jstree.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Appezzamento.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Azienda.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Campo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Catasto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Centro.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Contatto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Fabbricato.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Impianto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Esercizio.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Macchina.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_jQueryDocReady.js") %>"></script>
    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_AlberoAnagrafica.js") %>"></script>--%>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_RaggruppamentoStalla.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Stalla.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Zoo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_Meteo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_ModificaMultipla.js") %>"></script>

    <!-- indispensabile per connessione con oggetti GIS -->
    <input type="hidden" name="HiddenSelezioneAlberoAnagraficaAlberoAnagrafica" id="HiddenSelezioneAlberoAnagraficaAlberoAnagrafica">
    <asp:HiddenField ID="hdAlberoAnagrafica2017cfg" runat="server" />
    <asp:HiddenField ID="hidden_modificaMultipla" runat="server" />
    <cc1:AlberoAnagrafica2017 ID="AlberoAnagrafica2017" runat="server" />

    <div style="display: none">
        <div id="windowOperazione">
            <div class="row">
                <!--<div class="col-lg-1 col-md-1"></div>-->
                <div class="col-lg-10 col-md-10">
                    <div class="input-group">
                        <span class="input-group-addon" id="lbl_operazione" for="Cmb_Operazioni">Operazione</span>
                        <input type="text" id="Cmb_Operazioni" class="form-control" />
                    </div>
                </div>
                <!--<div class="col-lg-1 col-md-1"></div>-->
            </div>
            <div class="row">
                <div class="col-lg-5 col-md-5"></div>
                <div class="col-lg-2 col-md-2">
                    <div class="btn btn-success" id="btnCreaOp" onclick="CreaOp();">
                        <i class="fa fa-plus-square-o"></i>Inserisci
                    </div>
                </div>
                <div class="col-lg-5 col-md-5"></div>
            </div>
        </div>
    </div>

    <!-- a sx l albero a destra tool e tabella -->
    <div class="container gias-zootecnia-anagrafiche-container gias-full-width-with-margin-x">

        <div id="slide-in-share" class="slideInShareCommon" style="top: 100px; z-index: 100; display: none;">

            <div id="slide-in-handle" class="btn btn-info slideInHandleCommon">
                <div style="position: absolute; top: 50%; transform: translateY(-50%) rotate(270deg) translateX(-50%); transform-origin: left top; /*backface-visibility: hidden; */">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Anagrafiche %>" runat="server">Anagrafiche</asp:Localize>
                </div>
            </div>

             <div id="container">
                <div id="top-panel">
                    <div class="k-window" style="background-color: white; padding: 9px; border-top-right-radius: 0px;">
                                
                        <div id="ricercaRegion" style="display: grid; grid-template-columns: 2fr auto auto; grid-gap: 5px; margin-bottom: 10px;">
                            <asp:DropDownList name="AlberoCentriDropdown" ID="AlberoCentriDropdown" runat="server" ClientIDMode="Static" CssClass="form-control selectpicker"
                                            AutoPostBack="False" data-live-search="true">
                            </asp:DropDownList>


                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" id="chk_showHideCatasto" onclick="alberoFiltriCambiati(true)" />
                                <label class="form-check-label" for="chk_showHideCatasto">
                                  <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaCatasto %>" runat="server">Visualizza Catasto</asp:Localize>
                                </label>
                            </div>
                        </div>

                        <div id="filterRegion" style="display: grid; grid-template-columns: 2fr auto auto; grid-gap: 5px; margin-bottom: 10px;">
                            <%--<div><input id="filterData" type="text" placeholder="Valido alla data" /></div>--%>
                            <div>
                                <span id="filterContainer" class="k-textbox k-space-right" style="background-color: #fff; width: 100%;">

                                    <%--<input id="filterText" type="text" placeholder="<%$ Resources: AgronicaAgenda_2010, CercaAnagrafica %>">--%>
                                    <input id="filterText" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, CercaAnagrafica %>' runat='server'></asp:Localize>">

                                    <a id="RicercaSempliceRipulisci" class="k-icon k-i-close clearFilter" style="cursor: pointer;"></a>
                                    <span class="fa fa-refresh fa-lg fa-spin filter-loader filter-loader-hidden" style="position: absolute; right: 2em; margin: -6px 0 0; top: 50%;"></span>
                                </span>
                            </div>
                            <div class="k-button" id="gisMenuRicercaSempliceAvvia">
                                <span class="fa fa-search fa-lg search-or-loader"></span>
                                <span class="fa fa-refresh fa-lg fa-spin search-or-loader search-or-loader-hidden"></span>
                            </div>
                            <div class="k-button" style="" id="gisMenuRicercaAvanzata">
                                <span class="fa fa-bars"></span>
                                <span class="caret"></span>
                            </div>
                        </div>

                        <div id="RicercaAvanzataCatasto" style="display: grid; grid-template-columns: 1fr 2fr 2fr 2fr auto auto; grid-gap: 5px; margin-bottom: 10px;">
                            <div id="filterAvanzatoProvContainer" class="k-textbox" style="width: 100%;">
                                <%--<input id="filterAvanzatoProvText" type="text" placeholder="<%$ Resources: AgronicaAgenda_2010, ProvinciaAbbr %>">--%>
                                <input id="filterAvanzatoProvText" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, ProvinciaAbbr %>' runat='server'></asp:Localize>">
                            </div>
                            <div id="filterAvanzatoComContainer" class="k-textbox" style="width: 100%;">
                                <%--<input id="filterAvanzatoComText" type="text" placeholder="<%$ Resources: AgronicaAgenda_2010, Comune %>">--%>
                                <input id="filterAvanzatoComText" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Comune %>' runat='server'></asp:Localize>">
                            </div>
                            <div id="filterAvanzatoFoglioContainer" class="k-textbox" style="width: 100%;">
                                <%--<input id="filterAvanzatoFoglioText" type="text" placeholder="<%$ Resources: AgronicaAgenda_2010, Foglio %>">--%>
                                <input id="filterAvanzatoFoglioText" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Foglio %>' runat='server'></asp:Localize>">
                            </div>
                            <div id="filterAvanzatoParticellaContainer" class="k-textbox" style="width: 100%;">
                                <%--<input id="filterAvanzatoParticellaText" type="text" placeholder="<%$ Resources: AgronicaAgenda_2010, Particella %>">--%>
                                <input id="filterAvanzatoParticellaText" type="text" placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Particella %>' runat='server'></asp:Localize>">
                            </div>
                            <div class="k-button" id="gisMenuRicercaAvanzataAvvia">
                                <span class="fa fa-search fa-lg search-or-loader"></span>
                                <span class="fa fa-refresh fa-lg fa-spin search-or-loader search-or-loader-hidden"></span>
                            </div>
                            <div class="k-button" id="gisMenuRicercaAvanzataChiudi">
                                <span class="fa fa-close fa-lg"></span>
                            </div>
                        </div>

                        <div id="gis_treeview" style="height: 75vh; min-height: 500px; overflow-y: auto; border: 1px solid #d9d9d9;"></div>

                    </div>
                </div>
                <div id="bottom-panel">
                   <div id="drag" class="k-window"></div>
                </div>
            </div>
        </div>

        <%--<div id="windowAlbero">
            <div id="gis_treeview" style="height: 500px; overflow-y: auto; border: 1px solid #d9d9d9;"></div>
        </div>--%>

        <asp:HiddenField ID="hidden_azienda" runat="server" />
        <asp:HiddenField ID="hidden_sa_cod" runat="server" />
        <asp:HiddenField ID="hidden_objP_agenda" runat="server" />
        <asp:HiddenField ID="hidden_gest_esercizi" runat="server" />
        <asp:HiddenField ID="hidden_config_albero" runat="server" />
        <asp:HiddenField ID="hidden_ereditatore" runat="server" />
        <asp:HiddenField ID="hidden_impedisci_eliminazione_contatti" runat="server" />

        <input type="hidden" id="rigaSelezionataAlbero" name="rigaSelezionataAlbero" value="" />

        <!-- DIALOG TEST -->
        <!--<div>
            <div id="dialog"></div>

            <button type="button" class="btn xonne-btn-primary xonne-dialog-open" data-type="success">Dialog Success</button>
            <button type="button" class="btn xonne-btn-primary xonne-dialog-open" data-type="warning">Dialog Warning</button>
            <button type="button" class="btn xonne-btn-primary xonne-dialog-open" data-type="info">Dialog Info</button>
            <button type="button" class="btn xonne-btn-primary xonne-dialog-open" data-type="error">Dialog Error</button>
        </div>-->
        <!-- DIALOG TEST -->

        <!-- NOTIFICATION TEST -->
        <!-- <div>
            <button type="button" class="btn xonne-btn-primary xonne-notification-open" data-type="success">Notifica Success</button>
            <button type="button" class="btn xonne-btn-primary xonne-notification-open" data-type="warning">Notifica Warning</button>
            <button type="button" class="btn xonne-btn-primary xonne-notification-open" data-type="info">Notifica Info</button>
            <button type="button" class="btn xonne-btn-primary xonne-notification-open" data-type="error">Notifica Error</button>
        </div> -->
        <!-- NOTIFICATION TEST -->

        <div>
            <div class="gias-zootecnia-anagrafiche-header">
                <div>
                    <div class="col-lg-12">
                        <p class="text-primary">
                            <b>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltriSullaNavigazione %>" runat="server">Filtri sulla Navigazione</asp:Localize></b>
                        </p>
                    </div>
                </div>
                <div class="row">

                    <div class="col-lg-12">

                        <!--<div class="col-lg-7 col-md-12 col-xs-12 text-left"></div>-->

                        <!-- filtro data validita -->
                        <div class="col-lg-9 col-md-12 col-sm-12 col-xs-12">
                            <div id="menu_label_navigazione">

                                <nav aria-label="breadcrumb">
                                    <ol class="breadcrumb">
                                    </ol>
                                </nav>
                            </div>
                        </div>

                        <!-- bottoni d'operazione -->
                        <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12 gias-zootecnia-anagrafiche-header-buttons">
                            <div class="input-group ">
                                <label class="input-group-addon" id="lbl_filter_data" for="filterData">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValidiAllaData %>" runat="server">Validi alla data:</asp:Localize></label>
                                <input type="text" id="filterData" class="form-control kendoDate" placeholder="" />
                            </div>
                            <div class="btn-group pull-right" role="group">
                                <div class="btn btn-success xonne-btn-primary" onclick="resetFiltrototale();">
                                    <span class="k-icon k-i-filter-clear"></span>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PulisciFiltri %>" runat="server">Pulisci Filtri</asp:Localize>
                                </div>
                                <div class="btn btn-success xonne-btn-primary" onclick="NuovoElemento();">
                                    <i class="k-icon k-i-file-txt k-i-txt"></i>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server">Nuovo</asp:Localize>
                                </div>
                                <!--&nbsp;&nbsp;-->
                                <div id="btnStampa" class="pull-right">
                                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span> <span class="fa fa-print"></span>Stampa
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="#" onclick="StampaReport('7')"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StampaQuadroP %>" runat="server">Stampa Quadro P</asp:Localize></a></li>                                        
                                        <li><a href="#" onclick="StampaReport('18')"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiepilogoSuperficiUtilizzate %>" runat="server">Riepilogo Superfici Utilizzate</asp:Localize></a></li>
                                    </ul>
                                </div>
                            </div>
                            <%--<div id="menu_princ_operazioni" class="text-right" style="display:none;">
                            <div class="btn btn-default btn_menu_anagrafica_1" id="btn_info_s">
                                <i class="fa fa-info fa-2x" title="Info selezione"></i>
                            </div>
                            <div class="btn btn-default btn_menu_anagrafica_1" id="btn_modifica_s">
                                <i class="fa fa-pencil fa-2x" title="Modifica selezione"></i>
                            </div>
                            <div class="btn btn-danger btn_menu_anagrafica_1" id="btn_cancella_s">
                                <i class="fa fa-trash fa-2x" title="Cancella selezione"></i>
                            </div>
                        </div>--%>
                        </div>

                        <!-- bottoni fissi 
                    <div class="col-lg-1 col-md-2 col-sm-2 col-xs-4 text-right">
                    </div>-->

                    </div>
                    <input type="hidden" id="ElementoSelezionato" />
                </div>
            </div>


            <div class="row gias-zootecnia-anagrafiche-content">
                <div class="col-lg-12">
                    <div id="tabstrip" style="display: none;">
                        <!-- Nav tabs -->
                        <ul>
                            <li>
                                <i class="icon-tab icon-tab-azienda"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Azienda %>" runat="server">Azienda</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-centri"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Centri %>" runat="server">Centri</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-catasto"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Catasto %>" runat="server">Catasto</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-campi"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CampiSerre %>" runat="server">Campi/Serre</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-appezzamenti"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamenti %>" runat="server">Appezzamenti</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-impianti"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Impianti %>" runat="server">Impianti</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-esercizi"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Esercizi %>" runat="server">Esercizi</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-fabbricati"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Fabbricati %>" runat="server">Fabbricati</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-contatti"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contatti %>" runat="server">Contatti</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-macchine"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Macchine %>" runat="server">Macchine</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-stalle"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stalle %>" runat="server">Stalle</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-gruppi"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Gruppi %>" runat="server">Gruppi</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-capi"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Capi %>" runat="server">Capi</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-dss"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DSSeMeteo %>" runat="server">DSS e Meteo</asp:Localize></li>
                            <li>
                                <i class="icon-tab icon-tab-prodotti"></i>
                                Prodotti
                            </li>
                        </ul>
                            <div id="tb_azienda" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_aziende" />
                                <input type="hidden" id="hdKendoAzienda_Valorizzazione" />
                                <div id="divKendoAzienda" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_centri" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_centri" />
                                <input type="hidden" id="hdKendoCentro_Valorizzazione" />
                                <div id="divKendoCentro" style="overflow: auto;" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_catasto" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_catasto" />
                                <input type="hidden" id="hdKendoCatasto_Valorizzazione" />
                                <div id="divKendoCatasto" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_campi" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_campo" />
                                <input type="hidden" id="hdKendoCampo_Valorizzazione" />
                                <div id="divKendoCampo" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_appezzamenti" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_appezzamento" />
                                <input type="hidden" id="hdKendoAppezzamento_Valorizzazione" />
                                <div id="divKendoAppezzamento" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_impianti" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_impianto" />
                                <input type="hidden" id="hdKendoImpianto_Valorizzazione" />
                                <div id="divKendoImpianto" style="overflow: auto;" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_esercizi" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_esercizio" />
                                <input type="hidden" id="hdKendoEsercizio_Valorizzazione" />
                                <div id="divKendoEsercizio" style="overflow: auto;" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_fabbricati" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_fabbricati" />
                                <input type="hidden" id="hdKendoFabbricato_Valorizzazione" />
                                <div id="divKendoFabbricato" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_contatti" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_contatti" />
                                <input type="hidden" id="hdKendoContatto_Valorizzazione" />
                                <div id="divKendoContatto" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_macchine" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_macchina" />
                                <input type="hidden" id="hdKendoMacchina_Valorizzazione" />
                                <div id="divKendoMacchina" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_stalle" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_stalle" />
                                <input type="hidden" id="hdKendoStalle_Valorizzazione" />
                                <div id="divKendoStalle" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_raggruppamentiStalle" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_RaggruppamentiStalle" />
                                <input type="hidden" id="hdKendoRaggruppamentiStalle_Valorizzazione" />
                                <div id="divKendoRaggruppamentiStalle" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_Zoo" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_Zoo" />
                                <input type="hidden" id="hdKendoZoo_Valorizzazione" />
                                <div id="divKendoZoo" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_StazioniMeteo" class="row gias-content-padding-x gias-content-padding-y">
                                <input type="hidden" id="chiave_StazioniMeteo" />
                                <input type="hidden" id="hdKendoStazioniMeteo_Valorizzazione" />
                                <div id="divKendoStazioniMeteo" class="border_si__no_shadow"></div>
                            </div>
                            <div id="tb_prodotti" class="row gias-content-padding-x gias-content-padding-y">
                                <ucProd:Prodotto_Edit_UC id="prodottoUC" runat="server" />
                            </div>
                    </div>
                </div>
            </div>

        </div>

    </div>

    <!-- Dialog Nuovo -->
    <div class="modal fade gias-zootecnia-anagrafiche-modal-new-item" id="modalNuovo" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <form id="form_nuovo_elemento" method="get" action="">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="lbl_new_item">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CreazioneNuovoElemento %>" runat="server">Creazione Nuovo Elemento</asp:Localize>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <%--select riempita via js in jquery doc ready--%>
                        <div class="form-group" style="background-color: #D2130F; padding: 15px 0;">
                            <label for="recipient-name" class="control-label" style="color: #fff;">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SelezionaTipoNuovoElemento %>" runat="server">Seleziona la tipologia del nuovo elemento</asp:Localize></label>
                            <select id="tipoNuovo" style="width: 100%; text-transform: uppercase;">
                                <%--<option value="1">AZIENDA</option>
                                <option value="2">CENTRO AZIENDALE</option>
                                <option value="6">PARTICELLA CATASTALE</option>
                                <option value="3">CAMPO / SERRA</option>
                                <option value="4">APPEZZAMENTO</option>
                                <option value="5">IMPIANTO</option>               
                                <option value="10">FABBRICATO</option>
                                <option value="7">RAGGRUPPAMENTO STALLA</option>
                                <option value="9">ANIMALE</option>
                                <option value="11">CONTATTO</option>
                                <option value="12">MACCHINA</option>
                                <option value="13">DSS E METEO</option>--%>
                            </select>
                            <!--<div id="tipoNuovo"></div>-->
                        </div>
                        <div class="form-group datiDaNascondere datiAppezza datiImpianto datiCatasto datiFabbricato datiMeteoDSS datiRaggruppamento">
                            <h4 class="modal-title" id="lbl_riferimenti">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ElementiDiRiferimento %>" runat="server">Elementi di riferimento</asp:Localize>
                            </h4>
                        </div>
                        <div class="form-group" style="display: none">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Azienda %>" runat="server">Azienda</asp:Localize>:
                            </label>
                            <input id="txt_nuovoPiva" class="form-control">
                        </div>
                        <div class="form-group datiDaNascondere datiAppezza datiImpianto datiCatasto datiFabbricato datiRaggruppamento datiMeteoDSS">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoCentro" class="form-control required" style="width: 100%"></select>
                            <input type="hidden" id="txt_latcentro" value="" />
                            <input type="hidden" id="txt_loncentro" value="" />
                        </div>
                        <div class="form-group datiDaNascondere datiCampo">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoCentro_Campo" class="form-control required" style="width: 100%"></select>
                            <br />
                            <br />
                            <input type="checkbox" id="chk_serra" />
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Serra %>" runat="server">Serra</asp:Localize>
                        </div>
                        <div class="form-group datiDaNascondere datiAppezza datiImpianto">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Campo %>" runat="server">Campo</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoCampo" class="form-control" style="width: 100%"></select>
                        </div>
                        <div class="form-group datiDaNascondere datiImpianto">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamento %>" runat="server">Appezzamento</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoAppezzamento" class="form-control" style="width: 100%"></select>
                        </div>
                        <div class="form-group datiDaNascondere datiRaggruppamento">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stalla %>" runat="server">Stalla</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoFabbricatoStalla" class="form-control required" style="width: 100%"></select>
                        </div>
                        <div class="form-group datiDaNascondere datiMeteoDSS">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaSorgenteDati %>" runat="server">Categoria Sorgente Dati</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoMeteoDssSorgenteDati" class="form-control required" style="width: 100%"></select>
                        </div>

                        <div class="form-group datiDaNascondere datiMeteoDSS">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrigineDatiMeteo %>" runat="server">Origine Dati Meteo</asp:Localize>:
                            </label>
                            <select id="ddl_nuovoMeteoDssOrigine" class="form-control required" style="width: 100%"></select>
                        </div>
                        <div class="form-group datiDaNascondere datiProdotto">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotto %>" runat="server">Categoria Prodotto:</asp:Localize>:
                            </label>
                            <select id="ddl_categorie_prodotti" class="form-control required" style="width: 100%"></select>
                        </div>
                        <div class="form-group datiDaNascondere datiMeteoDSS">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ModelliPrevisionaliAssociatiOrigineDatiMeteo %>" runat="server">Modelli previsionali associati di Default all'Origine Dati Meteo:</asp:Localize>
                            </label>

                            <input id="kendoDropDownTree_nuovoMeteoDssModelliPrevisionali" style="width: 100%;" />
                        </div>
                        <div class="form-group datiDaNascondere datiAppezza">
                            <i class="fa fa-info-circle"></i>
                            <small>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AppezzamentoRiferibileACampo %>" runat="server">L'Appezzamento può far riferimento a un Campo.</asp:Localize>
                            </small>
                            <br />
                            <i class="fa fa-info-circle"></i>
                            <small>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SeCampoVuotoCreoAppezzamentoLibero %>" runat="server">Se il Campo viene lasciato vuoto, si crea un Appezzamento libero.</asp:Localize>
                            </small>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <!--<button type="button" class="btn btn-default" data-dismiss="modal">
                            <i class="fa fa-times"></i>
                            Chiudi</button>-->
                        <button type="button" class="btn btn-success gias-btn-primary" id="btn_nuovo">
                            <i class="fa fa-plus"></i>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Crea %>" runat="server">Crea</asp:Localize>
                        </button>
                    </div>
                </form>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>

    <!-- Info Catasto -->
    <div id="dialogInfoCatasto" style="display: none">
        <div class="row">
            <div class="col-lg-12">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Macrousi %>" runat="server">Macrousi</asp:Localize>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div id="kendoMacrousiCatasto"></div>
            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Utilizzi %>" runat="server">Utilizzi</asp:Localize>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div id="kendoUtilizziCatasto"></div>
            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Classamento %>" runat="server">Classamento</asp:Localize>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div id="kendoClassamentoCatasto"></div>
            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Appezzamenti %>" runat="server">Appezzamenti</asp:Localize>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div id="kendoAppezzamentiCatasto"></div>
            </div>
        </div>
        <br />
    </div>

    <!-- Modifica Multipla -->
    <div id="winModificaMultipla" style="display: none">

        <div class="window-content">

            <div class="row">
                <div id="scelta_parametro">
                    <p id="avvertimentoModificaMultipla" style="font-weight: bold"></p>
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriDaModificare %>" runat="server">Parametri da Modificare</asp:Localize></label>
                    </div>
                    <div data-container-for="Unità Produttiva" class="k-edit-field">
                        <input type="text" id="Cmb_Parametri" />
                    </div>
                </div>

                <div id="modifica_esercizi">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ApplicaA %>" runat="server">Applica a</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Esercizi" style="width: 300px" />
                    </div>

                </div>

                <div id="modifica_esercizi_data">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Data %>" runat="server">Data</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Esercizi" />
                    </div>
                </div>
            </div>

            <div class="row">
                <hr class="style1" />
                <div id="modifica_finalita">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Finalità %>" runat="server">Finalità</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Finalita" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Regolamento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server">Regolamento</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Regolamento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_DPI">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Disciplinare %>" runat="server">Disciplinare</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Disciplinare" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_disciplinare">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Disciplinare %>" runat="server">Disciplinare</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Disciplinare" style="width: 300px" />
                    </div>

                    <div class="k-edit-label">
                        <label>IAF</label>
                        <!-- i18n -->
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_IAF" />
                    </div>

                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Tipologia" style="width: 300px" />
                    </div>

                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StatoImpianto %>" runat="server">Stato Impianto</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_StatoImpianto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_n">
                    <div class="k-edit-label">
                        <label>N (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_N" />
                    </div>
                </div>

                <div id="modifica_p">
                    <div class="k-edit-label">
                        <label>P (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_P" />
                    </div>
                </div>

                <div id="modifica_k">
                    <div class="k-edit-label">
                        <label>K (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_K" />
                    </div>
                </div>

                <div id="modifica_varieta">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server">Varietà</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Varieta" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Grva">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GruppoVarietale %>" runat="server">Gruppo Varietale</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Grva" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_CapitolatoPrivato">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CapitolatoPrivato %>" runat="server">Capitolato Privato</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_CapitolatoPrivato" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Certificazione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Certificazione %>" runat="server">Certificazione</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Certificazione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_OrganismoReferente">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrganismoReferente %>" runat="server">Organismo Referente</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_OrganismoReferente" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_MagazzinoConferimento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MagazzinoConferimento %>" runat="server">Magazzino Conferimento</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_MagazzinoConferimento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_metodo_produzione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MetodoProduzione %>" runat="server">Metodo Produzione</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Metodo_Produzione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Data_Fine_Appezzamento">
                    <div class="k-edit-label">
                        <label>Chiusura Appezzamento</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Fine_Appezzamento" />
                    </div>
                </div>

                <div id="modifica_Data_Fine_Impianto">
                    <div class="k-edit-label">
                        <label>Chiusura Impianto</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Fine_Impianto" />
                    </div>
                </div>

                <div id="modifica_Copertura">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Copertura %>" runat="server">Copertura</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Copertura" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_FormaAllevamento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FormaAllevamento %>" runat="server">Forma Allevamento</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_FormaAllevamento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Portinnesto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Portinnesto %>" runat="server">Portinnesto</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Portinnesto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Data_Inizio_Portinnesto">
                    <div class="k-edit-label">
                        <label>Messa a dimora Portinnesto</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Inizio_Portinnesto" />
                    </div>
                </div>

                <div id="modifica_SuFila">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SuFila %>" runat="server">Distanza Su Fila</asp:Localize>
                            [m]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_SuFila" />
                    </div>
                </div>

                <div id="modifica_TraFila">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TraFila %>" runat="server">Distanza Tra Fila</asp:Localize>
                            [m]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_TraFila" />
                    </div>
                </div>

                <div id="modifica_Resa">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ResaPrevista %>" runat="server">Resa Prevista</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Resa" />
                    </div>
                </div>

                <div id="modifica_Data_Semina">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataSemina %>" runat="server">Data Semina Prevista</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Semina" />
                    </div>
                </div>

                <div id="modifica_Data_Raccolta">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRaccolta %>" runat="server">Data Raccolta Prevista</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Raccolta" />
                    </div>
                </div>

                <div id="modifica_Data_Fioritura">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataFioritura %>" runat="server">Data Fioritura Prevista</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Fioritura" />
                    </div>
                </div>

                <div id="modifica_ImpIrrigazione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImpiantoIrrigazione %>" runat="server">Impianto Irrigazione</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_ImpIrrigazione" style="width: 300px" />
                    </div>
                </div>
                
                <div id="modifica_FlagSecondoRaccolto">
                    <div class="k-edit-label">
                        <label>Secondo Raccolto</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="FlagSecondoRaccolto" />
                    </div>
                </div>

            </div>
        </div>

        <div class="window-footer">
            <button type="button" class="k-primary k-button" id="btn_ApplicaModifiche" onclick="applicaModifiche();">
                <span class="k-icon k-i-check"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ApplicaModifiche %>" runat="server">Applica Modifiche</asp:Localize>
            </button>
            <button type="button" class="k-button" id="btn_AnnullaModifiche" onclick="chiudiModificaMultipla();">
                <span class="k-icon k-i-cancel"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Annulla %>" runat="server">Annulla</asp:Localize>
            </button>
        </div>

    </div>

</asp:Content>


<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
   <div id="cont_script"></div>
    <script id="templateKendoCentri" type="text/x-kendo-template">
        <!--<div class="btn btn-success" onclick="resetFiltro(2, 'divKendoCentro');">
            <i class="fa fa-times">Reset Filtro Centro</i>
        </div>-->
    </script>

    <script id="templateKendoCampi" type="text/x-kendo-template">
        <!--<div class="btn btn-success" onclick="resetFiltro(3, 'divKendoCampo');">
            <i class="fa fa-times">Reset Filtro Campo</i>
        </div>-->
    </script>

    <script id="templateKendoAppezzamenti" type="text/x-kendo-template">
        <!--<div class="btn btn-success" onclick="resetFiltro(4, 'divKendoAppezzamento');">
            <i class="fa fa-times">Reset Filtro Appezzamento</i>
        </div>-->
    </script>

    <script id="templateKendoImpianti" type="text/x-kendo-template">
        <!--<div class="btn btn-success" onclick="resetFiltro(5, 'divKendoImpianto');">
            <i class="fa fa-times">Reset Filtro Impianto</i>
        </div>-->
    </script>

    <script id="templateKendoEsercizi" type="text/x-kendo-template">
        # if (GiasVersioneMaster === "2022") { 
            var titleChiusuraApEsercizi = Traduzione(menuBSAnagraficaResx, 'ChiusuraAperturaEsercizi', 'Chiusura/Apertura Esercizi'); #
            <div id="btnGestioneEsercizi" class="k-button k-button-icontext k-grid--button" onclick="caricaGestioneEsercizi(true);" data-title="#= titleChiusuraApEsercizi#">
                <span class="k-icon k-i-logout"></span>
            </div>
        # } else { #
            <div id="btnGestioneEsercizi" class="k-button k-button-icontext" onclick="caricaGestioneEsercizi(true);">
                <span class="k-icon k-i-logout"></span>
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,ChiusuraAperturaEsercizi %>" runat="server">Chiusura/Apertura Esercizi</asp:Localize>
            </div>
        # } #
    </script>

    <script src="../Scripts/bootbox.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script type="text/javascript">

        var hidden_azienda_ClientID = "<%=hidden_azienda.ClientID %>";
        var hidden_sa_cod_ClientID = "<%=hidden_sa_cod.ClientID %>";
        var hidden_objP_agenda_ClientID = "<%=hidden_sa_cod.ClientID %>";
        var username_master = "<%= Master.agroMasterPage_UtenteUsername %>";
        var gestione_esercizi = $('#<%=hidden_gest_esercizi.ClientID %>').val();
        var config_albero = $('#<%=hidden_config_albero.ClientID %>').val();
        var hdAlberoAnagrafica2017cfg_ClientID = "<%= hdAlberoAnagrafica2017cfg.ClientID %>";
        var ereditatore = $('#<%=hidden_ereditatore.ClientID %>').val();
        var modificaMultipla = $('#<%=hidden_ModificaMultipla.ClientID %>').val();
        var obj_Impianto = <%= objImpianto.ToString  %>;
        var impedisci_eliminazione_contatti = "#<%=hidden_impedisci_eliminazione_contatti.ClientID %>";

        var qsVisibilita;
    </script>

</asp:Content>
