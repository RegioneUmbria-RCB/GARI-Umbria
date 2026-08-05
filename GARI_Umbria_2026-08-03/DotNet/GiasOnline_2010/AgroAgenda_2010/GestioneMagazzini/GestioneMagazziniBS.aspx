<%@ Page Title="Gestione Magazzino" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="GestioneMagazziniBS.aspx.vb" Inherits="AgroAgenda_2010.GestioneMagazziniBS" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="GestioneMagazziniBS.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .k-state-giallo td {
            color: #000000 !important;
            background-color: #FCF8E3 !important;
        }

        .k-state-verde td {
            color: #000000 !important;
            background-color: #DFF0D8 !important;
        }

        .k-state-rosso td {
            color: #000000 !important;
            background-color: #F2DEDE !important;
        }

        #frmInputRicerca {
            margin: 20px 10px;
        }

        .btnAlienaGiacenze, .btnTrasferimentoDDT {
            cursor: pointer;
            margin-left: 15px;
        }

        /* Inizio stili per test menu filtri laterale */
        #DivFiltri {
            margin-top: 0 !important;
        }

     .xo-ricerca-doc-contabili-filtri-action-container {
         margin-top: 1rem;
         padding-top: 1rem;
         border-top: 1px solid #aaa;
         margin-right: 20px;
         margin-left: 20px;
     }

     #xoGestioneMagazziniTabHeaderFiltri {
         display: none;
     }

     .xoGestioneMagazziniTabPanelFiltri {
        list-style: none;
        padding-left: 0;
     }

    </style>
    <script>
        function toggleMenu() {
            const sidebar = document.getElementById('DivFiltri');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoRicercaDocToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestionePrezziLettura" />

    <%--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE--%>
    <div id="DIV_Messaggi" style="margin-top: 10px;">
    </div>

    <!-- contenitore principale -->
    <div>
        <div class="container" style="margin-bottom: 100px;">
            <div class="row" id="DivToolbar">
                <!-- nuova tool -->
                <div class="col-lg-12">
                    <nav class="navbar navbar-default">
                        <div class="container-fluid">
                            <!-- Brand and toggle get grouped for better mobile display -->
                            <div class="navbar-header">
                                <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#div_menu_tool">
                                    <span class="sr-only">Toggle navigation</span>
                                    <span class="icon-bar"></span>
                                    <span class="icon-bar"></span>
                                    <span class="icon-bar"></span>
                                </button>
                            </div>
                            <!-- Collect the nav links, forms, and other content for toggling -->
                            <div class="collapse navbar-collapse" id="div_menu_tool">
                                <ul class="nav navbar-nav" style="width: 100%">
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura = True) Then%>
                                    <li onclick="Gestione_Operazione_Menu('50');">
                                        <a onclick="return 0">
                                            <img src="../AB_Immagini/icone32/magazzinocarico.ico" />
                                            <span class="hidden-none margin-r"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Carico %>" runat="server">Carico</asp:Localize></span>
                                        </a>
                                    </li>
                                    <%End If%>
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura = True) Then%>
                                    <li onclick="Gestione_Operazione_Menu('51');">
                                        <a onclick="return 0">
                                            <img src="../AB_Immagini/icone32/magazzinoscarico.ico" />
                                            <span class="hidden-none margin-r"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Scarico %>" runat="server">Scarico</asp:Localize></span>
                                        </a>
                                    </li>
                                    <%End If%>
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura = True) Then%>
                                    <li onclick="Gestione_Operazione_Menu('52');">
                                        <a onclick="return 0">
                                            <img src="../AB_Immagini/icone32/MagazzinoTrasferimento.ico" />
                                            <span class="hidden-none margin-r"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Trasferimento %>" runat="server">Trasferimento</asp:Localize></span>
                                        </a>
                                    </li>
                                    <%End If%>                                       
                                </ul>
                            </div>
                            <!-- /.navbar-collapse -->
                        </div>
                        <!-- /.container-fluid -->
                    </nav>
                </div>
                <!--sezione form filtri-->
            </div>

            <% If Master.Master_versione = "2022" Then %>
            
            <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
            <div id="xoRicercaDocToggleFiltri" class="btn gias-btn-primary gias-btn-toggle-filter-sidebar hidden-sidebar" onclick="toggleMenu()">
                <span>
                    <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
                </span>
            </div>
            <div class="row gias-right-sidebar-filters hidden-sidebar" id="DivFiltri">
            <% Else %>
            <div class="row" id="DivFiltri">
            <% End If %>

                <div class="col-md-12">

                    <!-- Pannello in alto, input filtri -->
                    <ul id="AccordMenu" <% If Master.Master_versione = "2022" Then %> class="xoGestioneMagazziniTabPanelFiltri" <% End If %>>
                        <li class="k-state-active k-active">
                            <span <% If Master.Master_versione = "2022" Then %> id="xoGestioneMagazziniTabHeaderFiltri" <% End If %>
                                class="k-link k-state-selected k-selected"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Ricerca %>" runat="server">Ricerca</asp:Localize></span>
                        <!-- input Ricerca Giacenze -->
                        <div id="frmInputRicerca" class="form-horizontal">
                            
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <!-- Centro -->
                                    <div class="input-group" id="div_centro">
                                        <%--<label class="input-group-addon control-label alert-info" id="lbl_centro" for="ComboCentroAziendale">--%>
                                        <label class="input-group-addon" id="lbl_centro" for="ddlCentroAziendale">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>
                                        </label>
                                        <input id="ddlCentroAziendale" name="ddlCentroAziendale" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <!-- Magazzino -->
                                    <div class="input-group" id="div_magazzino">
                                        <%--<label class="input-group-addon control-label alert-info" id="lbl_magazzino" for="ComboMagazzini">--%>
                                        <label class="input-group-addon" id="lbl_magazzino" for="ddlMagazzini">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Magazzino %>" runat="server">Magazzino</asp:Localize>
                                        </label>
                                        <input id="ddlMagazzini" name="ddlMagazzini" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 10px;">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <!-- Categoria -->
                                    <div class="input-group" id="div_categoria">
                                        <%--<label class="input-group-addon control-label alert-info" id="Label1" for="cmb_Categoria">--%>
                                        <label class="input-group-addon" id="lbl_categoria" for="ddlCategoria">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CategoriaProdotto %>" runat="server">Categoria Prodotto</asp:Localize>
                                        </label>
                                        <input id="ddlCategoria" name="ddlCategoria" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <!-- Nome prodotto -->
                                    <div class="input-group" id="div_Nome_Prodotto">
                                        <label class="input-group-addon" id="lbl_Nome_Prodotto" for="TxtProdotto">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,NomeProdotto %>" runat="server">Nome Prodotto</asp:Localize>
                                        </label>
                                        <input id="TxtProdotto" name="TxtProdotto" type="text" class="form-control" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- Cod Articolo -->
                                    <div class="input-group" id="div_Cod_Articolo">
                                        <label class="input-group-addon" id="lbl_Cod_Articolo" for="TxtCodArticolo">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiceArticoloAbbr %>" runat="server">Cod Articolo</asp:Localize>
                                        </label>
                                        <input id="TxtCodArticolo" name="TxtCodArticolo" type="text" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- Codice Prodotto -->
                                    <div class="input-group" id="div_Cod_Prodotto">
                                        <label class="input-group-addon" id="lbl_Cod_Prodotto" for="TxtCodProdotto">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,CodiceProdotto %>" runat="server">Codice Prodotto</asp:Localize>
                                        </label>
                                        <input id="TxtCodProdotto" name="TxtCodProdotto" type="text" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- Lotto -->
                                    <div class="input-group" id="div_Lotto">
                                        <label class="input-group-addon" id="lbl_Lotto" for="TxtLotto">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Lotto %>" runat="server">Lotto</asp:Localize>
                                        </label>
                                        <input id="TxtLotto" name="TxtLotto" type="text" class="form-control" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- Check Filtra per Lotto -->
                                    <div class="xi-form-action mt-30">
                                        <input id="CheckBoxFiltraPerLotto" name="CheckBoxFiltraPerLotto" type="checkbox" class="kendoSwitch" />
                                        <label id="lbl_chk_Filtra Per_Lotto" class="k-checkbox-label" for="CheckBoxFiltraPerLotto">
                                            <asp:Localize Text="<%$ Resources:FiltraPerLotto %>" runat="server">Filtra per lotto</asp:Localize>
                                        </label>
                                    </div>
                                </div>
                            </div>

                            <div class="row" style="margin-top: 10px;">
                                <div class="col-lg-6 col-md-6 col-sm-12">
                                    <div class="input-group" id="arrotondamento">
                                        <label class="input-group-addon" id="lbl_arrotondamento" for="ddlArrotondamento">
                                            <asp:Localize Text="<%$ Resources:Arrotonda %>" runat="server">Arrotonda</asp:Localize>
                                        </label>
                                        <input id="ddlArrotondamento" name="ddlArrotondamento" class="form-control" />
                                        <%--<asp:DropDownList ID="dll_arrotondamento" runat="server" CssClass="form-control selectpicker"
                                                                      data-live-search="true" aria-describedby="lbl_arrotondamento" data-container="body">
                                                        <asp:ListItem Value="0" meta:resourceKey="AdIntero">All&#39;Intero</asp:ListItem>
                                                        <asp:ListItem Value="1" meta:resourceKey="UnaCifra">1 cifra (0.1)</asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="2" meta:resourceKey="DueCifre">2 cifre (0.01)</asp:ListItem>
                                                        <asp:ListItem Value="3" meta:resourceKey="TreCifre">3 cifre (0.001)</asp:ListItem>
                                                        <asp:ListItem Value="4" meta:resourceKey="QuattroCifre">4 cifre (0.0001)</asp:ListItem>
                                                    </asp:DropDownList>--%>
                                    </div>
                                </div>
                            </div>

                            <div class="row" id="Div_FiltriGiacenze" style="margin-top: 10px;">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="input-group" id="Data_Giacenza">
                                        <label class="input-group-addon" id="lbl_Data_Giacenza" for="txt_DataOperazioneAlGiorno">
                                            <asp:Localize Text="<%$ Resources:GiacenzaAlGiorno %>" runat="server">Giacenza al giorno</asp:Localize>
                                        </label>
                                        <input id="txt_DataOperazioneAlGiorno" name="txt_DataOperazioneAlGiorno" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- check giacenza zero -->
                                    <div class="xi-form-action mt-30">
                                        <input id="CheckBoxGiacenze0" name="CheckBoxGiacenze0" type="checkbox" class="kendoSwitch" />
                                        <label id="lbl_chk_Visualizza_Giacenza_Zero" class="k-checkbox-label" for="CheckBoxGiacenze0">
                                            <asp:Localize Text="<%$ Resources:VisualizzaAncheGiacenzeZero %>" runat="server">Visualizza anche le Giacenze 0</asp:Localize>
                                        </label>
                                    </div>
                                </div>
                                <%--If (permessi.getPermesso(enum_Security_Attivita.Gestione_Prezzi).Lettura = True) Then--%>
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <!-- check valorizza prodotto -->
                                    <div id="groupValorizzaProdotto" class="xi-form-action mt-30">
                                        <input id="CheckBoxValorizzaProdotto" name="CheckBoxValorizzaProdotto" type="checkbox" class="kendoSwitch" />
                                        <label id="lbl_chk_Valorizza_Prodotto" class="k-checkbox-label" for="CheckBoxValorizzaProdotto">
                                            Valorizza Prodotto <%--i18n--%>
                                        </label>
                                    </div>
                                </div>
                                <%-- End If --%>
                            </div>
                            <div id="Div_Filtri_Movimenti">
                                <div class="row" style="margin-top: 10px;">
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                        <div class="input-group">
                                            <label class="input-group-addon" id="lbl_validita_inizio_codice" for="txt_DataOperazioneDa">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Dal %>" runat="server">Dal</asp:Localize>
                                            </label>
                                            <input id="txt_DataOperazioneDa" name="txt_DataOperazioneDa" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                        <div class="input-group">
                                            <label class="input-group-addon" id="lbl_validita_fine_codice" for="txt_DataOperazioneA">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Al %>" runat="server">Al</asp:Localize>
                                            </label>
                                            <input id="txt_DataOperazioneA" name="txt_DataOperazioneA" class="kendoCalendar" type="date" style="width: 100%;" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="col-lg-2 col-md-2 col-sm-12">
                                        <div class="xi-form-action mt-30">
                                            <div class="btn btn-default" id="Btn_AnnataPrecedente" onclick="SpostaAnnata(-1)">
                                                <span class="fa fa-arrow-left"></span>
                                            </div>
                                            <div class="btn btn-default" id="Btn_AnnataSuccessiva" onclick="SpostaAnnata(1)">
                                                <span class="fa fa-arrow-right"></span>
                                            </div>
                                         </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                        <!-- check carichi -->
                                        <div id="DivCarichi">
                                            <input id="CheckBoxCarichi" name="CheckBoxCarichi" type="checkbox" checked="checked" class="kendoSwitch" />
                                            <label id="lbl_visualizza_carichi" class="k-checkbox-label" for="CheckBoxCarichi" style="/*border: 0 !important; -webkit-box-shadow: none !important; -moz-box-shadow: none !important; box-shadow: none !important; */">
                                                <asp:Localize Text="<%$ Resources:VisualizzaCarichi %>" runat="server">Visualizza Carichi</asp:Localize>
                                            </label>
                                        </div>
                                    </div>
                                    <div class="col-lg-3 col-md-3 col-sm-12">
                                        <!-- check scarichi -->
                                        <div id="DivScarichi">
                                            <input id="CheckBoxScarichi" name="CheckBoxScarichi" type="checkbox" checked="checked" class="kendoSwitch" />
                                            <label id="lbl_visualizza_scarichi" class="k-checkbox-label" for="CheckBoxScarichi">
                                                <asp:Localize Text="<%$ Resources:VisualizzaScarichi %>" runat="server">Visualizza Scarichi</asp:Localize>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row" id="Div_FiltriGiacenze2">
                                <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                                    <div class="btn btn-default xonne-btn-primary" onclick="ClearParametri();">
                                        <% If Master.Master_versione = "2022" %>
	                                       <span class="k-icon k-i-filter-clear"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,PulisciFiltri %>" runat="server">Pulisci Filtri</asp:Localize>                                                
                                        <% Else %>
                                           <span class="fa fa-eraser"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,PulisciFiltri %>" runat="server">Pulisci Filtri</asp:Localize>                                       
                                        <% End If %>	
                                    </div>
                                    <div class="btn btn-success xonne-btn-primary" id="btn_ricerca_giacenze" onclick="btn_ricerca_giacenze_click()">
                                        <span class="fa fa-search xonne-search xo-agronica-style"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CercaGiacenze %>" runat="server">Cerca Giacenze</asp:Localize>
                                    </div>
                                </div>
                            </div>
                            <div class="row" id="Div_Filtri_Movimenti2">
                                <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                                    <div class="btn btn-default" onclick="ClearParametri();">
                                        <span class="fa fa-eraser"></span><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,PulisciFiltri %>" runat="server">Pulisci Filtri</asp:Localize>
                                    </div>
                                    <div class="btn btn-success xonne-btn-primary" id="btn_ricerca_movimenti" onclick="btn_ricerca_movimenti_click()">
                                        <span class="fa fa-search xonne-search xo-agronica-style"></span><asp:Localize Text="<%$ Resources:CercaMovimenti %>" runat="server">Cerca Movimenti</asp:Localize>
                                    </div>
                                </div>
                            </div>

                        </div>
                        </li>
                    </ul>

                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-horizontal" style="margin-top: 25px;">
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li class="active"><a href="#tabGiacenze" data-toggle="tab" id="a_tabGiacenze" onclick="MostraFiltriGiacenze()">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Giacenze %>" runat="server">Giacenze</asp:Localize>
                            </a></li>
                            <li><a href="#tabMovimenti" data-toggle="tab" id="a_tabMovimenti" onclick="MostraFiltriMovimenti()">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Movimenti %>" runat="server">Movimenti</asp:Localize>
                            </a></li>
                        </ul>
                        <div class="tab-content">
                            <!-- tab Giacenze -->
                            <div class="tab-pane fade active in" id="tabGiacenze" style="overflow: auto">
                                <div id="tabElencoGiacenze">
                                </div>
                            </div>
                            <!-- tab Movimenti -->
                            <div class="tab-pane fade in" id="tabMovimenti" style="overflow: auto">
                                <div id="tabElencoMovimenti">
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdVisualizzazioneMode" runat="server" />
                    <asp:HiddenField ID="hdTabRichiesto" runat="server" />

                </div>
            </div>
        </div>

        <!-- frame operazione agenda -->
        <div class="modal fade" id="divOperazioneAgenda">
            <div class="modal-dialog" style="width: 98%; height: 98%">
                <div class="modal-content" style="height: 95%; border-radius: 0">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">
                            <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                        <h5 class="modal-title">
                            <div style="font-weight: bold" id="lblOperazioneAgendaTitle">
                            </div>
                        </h5>
                    </div>
                    <div class="modal-body" style="height: 80%; border-radius: 0">
                        <iframe src="" id="iframedivOperazioneAgenda" style="width: 100%; height: 100%; display: block; border: 0" frameborder="0"></iframe>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Chiudi %>" runat="server">Chiudi</asp:Localize>
                        </button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>

        <!-- dialog Trasferimento fabbricato -->
        <div class="modal fade" id="dialogTrasferimentoMovContabili">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">
                            <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                        <h5 class="modal-title">
                            <asp:Localize Text="<%$ Resources:TrasferimentoCarichiDaDocumentoFiscale %>" runat="server">Trasferimento Carichi da Documento Fiscale</asp:Localize>
                        </h5>
                        <span style="color: Red;">
                            <asp:Localize Text="<%$ Resources:AttenzioneTrasferimentoNonControllaScarichiPrecedenti %>" runat="server">Attenzione: il trasferimento non controlla eventuali scarichi già avvenuti.</asp:Localize>
                        </span>
                    </div>
                    <div class="modal-body" id="contenitoreDivMagazzini">
                        <div class="input-group" style="width: 100%; max-width: 100%">
                            <b><asp:Localize Text="<%$ Resources:MagazzinoDestinazione %>" runat="server">Magazzino Destinazione</asp:Localize>:</b><br />
                            <%--<cc1:combomagazzini id="ComboMagazzini_1" runat="server" fabbricato_cod="0" flag_codcentrofabbricato="True"
                                appendto="" flag_gestionemagazziniimpresapadre="False" meta:resourcekey="ComboMagazziniResource1"
                                sa_cod="0" tipomagazzino="20" aria-describedby="lbl_magazzino" bootstrap="true"
                                data-mobile="true" />--%>
                            <div id="ddlMagazzini_Trasferimento_Wrapper">
                                <input id="ddlMagazzini_Trasferimento" name="ddlMagazzini_Trasferimento" class="form-control" />
                            </div>
                        </div>
                        <br />
                        <b><asp:Localize Text="<%$ Resources:MagazzinoProvenienza %>" runat="server">Magazzino Provenienza</asp:Localize>:</b><br />
                        <div id="fabbricato_selezionato"></div>
                        <br />
                        <b><asp:Localize Text="<%$ Resources:MovimentiDaTrasferire %>" runat="server">Movimenti da trasferire</asp:Localize>:</b><br />
                        <div id="nomiMovimentidaspostare"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal" id="chiudiTrasferimentoMovContabili">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Annulla %>" runat="server">Annulla</asp:Localize></button>
                        <button type="button" class="btn btn-success xi-btn-primary" onclick="TrasferimentoMovContabili();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Trasferisci %>" runat="server">Trasferisci</asp:Localize></button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
    </div>


    <!-- Dialog Kendo -->
    <div id="dialogErrorKendo"></div>
    <div id="dialogOkKendo"></div>

    <!-- Hidden Controls -->
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdQsDataGiacenza" runat="server" />
    <input type="hidden" id="hdQsDataOpDa" runat="server" />
    <input type="hidden" id="hdQsDataOpA" runat="server" />
    <input type="hidden" id="hdDefaults" runat="server" />
    <input type="hidden" id="hf_Moduli_Anagrafe_Log" runat="server" />

    <script id="tmplAlienaGiacenze" type="text/x-kendo-template">
        <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura = True) Then%>
        # if (GiasVersioneMaster === "2022") { #
            <div onclick="Gestione_Alienazione_Giacenze();" class="btn-aliena-giacenze k-button k-grid--button" 
                 data-title='<asp:Localize Text="<%$ Resources:AlienaGiacenze %>" runat="server">Aliena Giacenze</asp:Localize>' 
                 title='<asp:Localize Text="<%$ Resources:AlienaGiacenze %>" runat="server">Aliena Giacenze</asp:Localize>'>
                <span class="button-icon"></span>
            </div>
        # } else { #
            <li onclick="Gestione_Alienazione_Giacenze();" class="btnAlienaGiacenze">
                <a onclick="return 0">
                    <img src="../AB_Immagini/icone32/MagazzinoScarico.ico" />
                    <span class="hidden-none margin-r">
                        <asp:Localize Text="<%$ Resources:AlienaGiacenze %>" runat="server">Aliena Giacenze</asp:Localize></span>
                </a>
            </li>
        # } #
        <%End If%>
    </script>
    
    <script id="tmplTrasferimentoDDT" type="text/x-kendo-template">
        <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura = True And Num_Magazzini > 1) Then%>
            # if (GiasVersioneMaster === "2022") { #
                <div onclick="Gestione_Trasferimento_DettagliContabili();" class="btn-trasf-ddt k-button k-grid--button" 
                     data-title='<asp:Localize Text="<%$ Resources:TrasferimentoDDT %>" runat="server">Trasferimento DDT</asp:Localize>' 
                     title='<asp:Localize Text="<%$ Resources:TrasferimentoDDT %>" runat="server">Trasferimento DDT</asp:Localize>'>
                    <span class="button-icon"></span>
                </div>
            # } else { #
                <li onclick="Gestione_Trasferimento_DettagliContabili();" class="btnTrasferimentoDDT">
                    <a onclick="return 0">
                        <img src="../AB_Immagini/icone32/MagazzinoTrasferimento.ico" />
                        <span class="hidden-none margin-r">
                            <asp:Localize Text="<%$ Resources:TrasferimentoDDTAbbr %>" runat="server">Trasf. DDT</asp:Localize></span>
                    </a>
                </li>
            # } #
        <%End If%>
    </script>
        
    <script id="tmplStampaGiacenze" type="text/x-kendo-template">
        <div>            
            <button id="StampaGiacenze">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Stampa %>" runat="server">STAMPA</asp:Localize>
                <span class="caret"></span>
            </button>
        </div>    
    </script>

    <script id="tmplDataGiacenze" type="text/x-kendo-template">
         <div>            
             <h4>Data: </h4><h4 id='lbl_dataGiacenze'></h4>
         </div>    
     </script>

    <script id="tmplDataMovimenti" type="text/x-kendo-template">
     <div>            
         <h4>Date: </h4><h4 id='lbl_dataMovimenti'></h4>
     </div>    
 </script>

    <script id="tmplStampaMovimenti" type="text/x-kendo-template">
        <div>            
            <button id="StampaMovimenti">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Stampa %>" runat="server">STAMPA</asp:Localize>
                <span class="caret"></span>
            </button>
        </div>    
    </script>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var filtriDefault = JSON.parse($("#<%=hdDefaults.ClientID() %>").val());
        var moduliAnagrafe = JSON.parse($("#<%=hf_Moduli_Anagrafe_Log.ClientID() %>").val());

        var cIdPiva = "#<%=hdPiva.ClientID() %>";

        var cIdSa_Cod = "#ddlCentroAziendale";
        var cIdFabbricato_Cod = "#ddlMagazzini";
        var cIdPro_Cod = "#TxtCodProdotto";
        var cIdDataInizio = "#txt_DataOperazioneDa";
        var cIdDataFine = "#txt_DataOperazioneA";
        var cIdDataOperazione = "#txt_DataOperazioneAlGiorno";

        var cIdhdVisualizzazioneMode = "#<%=hdVisualizzazioneMode.ClientID %>";
        var cIdhdTabRichiesto = "#<%=hdTabRichiesto.ClientID %>";


        var permesso_Rintraccio_Operazione_Di_Cura = false
        <%If (permessi.getPermesso(enum_Security_Attivita.Rintraccio_Operazione_Di_Cura).Lettura = True) Then%>
            permesso_Rintraccio_Operazione_Di_Cura = true
        <% End If%>
        </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneMagazziniBS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneMagazziniBS_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneMagazziniBS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneMagazziniBS_jQueryDocReady.js") %>"></script>

</asp:Content>
