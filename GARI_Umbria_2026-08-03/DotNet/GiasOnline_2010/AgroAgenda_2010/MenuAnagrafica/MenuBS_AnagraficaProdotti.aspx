<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="MenuBS_AnagraficaProdotti.aspx.vb" Inherits="AgroAgenda_2010.MenuBS_AnagraficaProdotti" %>


<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- PER ALBERO NUOVO -->
    <asp:PlaceHolder ID="gisHeader" runat="server"></asp:PlaceHolder>

    <link rel="stylesheet" href="<%= ResolveClientUrl("MenuBS_Anagrafica.css?" & Application("GiasVersioneCorrente").ToString) %>" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!--include per l'albero-->
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_AnagraficaProdotti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_AnagraficaProdotti_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_AnagraficaProdotti_ws_client.js") %>"></script>
    <%--<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Anagrafica_AlberoAnagrafica.js") %>"></script>--%>

    <!-- indispensabile per connessione con oggetti GIS -->
    <input type="hidden" name="HiddenSelezioneAlberoAnagraficaAlberoAnagrafica" id="HiddenSelezioneAlberoAnagraficaAlberoAnagrafica">
    <asp:HiddenField ID="hdAlberoAnagrafica2017cfg" runat="server" />
    <asp:HiddenField ID="hidden_modificaMultipla" runat="server" />
    <cc1:AlberoAnagrafica2017 ID="AlberoAnagrafica2017" runat="server" />

    <style type="text/css">
    .errorClass {
        border-color: #D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    .buttonClass {
        margin: 0 0 10px 1px;
    }

    .jumbotron {
        margin-bottom: 0 !important;
    }

    .erroreCampiObbigatori {
        border: 2px solid #D41E1A;
    }

    /*Larghezza calendario come gli altri campi di input dei Parametri Qualitativi*/
    .kendoCalendar {
        width: 100%;
    }

    .table_prodotto_Edit_UC {
        border-collapse: collapse;
        border-spacing: 0;
        border: 0 none;
        border: 0;
        height: auto;
        width: auto;
        POSITION: absolute;
    }

        .table_prodotto_Edit_UC td {
            border: 0 none;
            padding: 1px;
            text-align: left;
        }
        /* Inizio stili per test menu filtri laterale */
        #xoGestioneCompletaCdGFiltriContainer {
            transition: transform 0.3s ease, opacity 0.3s ease;
            width: 50vw;
            height: calc(100vh - 50px);
            position: fixed;
            top: 50px;
            right: 0;
            background-color: white;
            z-index: 10000;
            overflow-y: scroll;
        }
         #xoGestioneCompletaCdGFiltriContainer.hidden-sidebar {
            opacity: 0;
            transform: translateX(100%);
         }

         #xoGestioneCompletaCdGToggleFiltri {
            transition: transform 0.3s ease, opacity 0.3s ease;
            top: 120px;
            position: fixed;
            z-index: 10000;
            right: 50vw;
         }
         #xoGestioneCompletaCdGToggleFiltri.hidden-sidebar {
            top: 120px;
            position: fixed;
            z-index: 10000;
            right: 0;
         }
         #xoGestioneCompletaCdGToggleFiltri span svg {
             width: 16px;
         }

         .btn.xonne-btn-filter {
            height: 40px !important;
            border-radius: 0 !important;
            border-top-left-radius: 16px !important;
            border-bottom-left-radius: 16px !important;
            border-color: white !important;
         }

         @media only screen and (max-width: 991px) {
            #xoGestioneCompletaCdGFiltriContainer {
                top: 100px;
                height: calc(100vh - 100px);
            }
         }
        /* Fine stili per menu laterale */
</style>
    <script>
        function toggleMenu() {
            //debugger;
            const sidebar = document.getElementById('xoGestioneCompletaCdGFiltriContainer');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoGestioneCompletaCdGToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>
    
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
    <div class="container">



        <%--<div id="windowAlbero">
            <div id="gis_treeview" style="height: 500px; overflow-y: auto; border: 1px solid #d9d9d9;"></div>
        </div>--%>

        <asp:HiddenField ID="hidden_azienda" runat="server" />
        <asp:HiddenField ID="hidden_sa_cod" runat="server" />
        <asp:HiddenField ID="hidden_objP_agenda" runat="server" />
        <asp:HiddenField ID="hidden_gest_esercizi" runat="server" />
        <asp:HiddenField ID="hidden_config_albero" runat="server" />
        <asp:HiddenField ID="hidden_ereditatore" runat="server" />

        <input type="hidden" id="rigaSelezionataAlbero" name="rigaSelezionataAlbero" value="" />

        <div class="container_Ricerca" style="padding: 0; /*margin-bottom: 70px*/">

            <% If Master.Master_versione = "2022" Then %>
            <div id="xoGestioneCompletaCdGToggleFiltri" class="btn xonne-btn-primary xonne-btn-filter" onclick="toggleMenu()">
                <span>
                    <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
                </span>
            </div>

            <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
            <div id="xoGestioneCompletaCdGFiltriContainer" class="panel-group Ricerca xo-ricerca-doc-filtri-container">
            <% Else %>
            <div class="panel-group Ricerca">
            <% End If %>

                <div id="searchArea" class="panel-group searchArea">
                    <div class="panel-body" style="padding-top: 5px;">
                        <div class="row">
                            <div class="col-lg-12">
                                <br />
                                <%-- <ucProd:Prodotto_Edit_UC id="Prodotto_Edit_UC1" runat="server" />--%>
                                <div class="panel-group prodotto_Edit_UC_searchArea" style="display: none;">
                                    <div class="jumbotron" style="padding-top: 30px;">
                                        <div class="row">
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <label class="input-group-addon alert-info" for="id_ddl_prodotto_Edit_UC_Categorie">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotti %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input type="text" id='ddl_prodotto_Edit_UC_Categorie' class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">

                                            <div class="col-lg-3 col-md-3 col-sm-12">
                                                <%--<div class="row-radio-group" id="FiltroRegGroup">--%>
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <label class="input-group-addon alert-info" id="lbl_filtro_reg" for="ddl_Regolamento">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <input type="text" id="ddl_Regolamento" name="ddl_Regolamento" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-3 col-md-3 col-sm-12">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon alert-info" id="lbl_filtro_visibilita" for="ddl_Visibilita">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visibilità %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <input type="text" id="ddl_Visibilita" name="ddl_Visibilita" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-3 col-md-3 col-sm-12">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon alert-info" id="lbl_filtro_valorizzati" for="ddl_Valorizzati">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Valorizzati %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <input type="text" id="ddl_Valorizzati" name="ddl_Valorizzati" class="form-control" />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="row">
                                            <%--Ricerca per descrizione--%>
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div class="input-group">
                                                            <span class="input-group-addon alert-info" id="lbl_descrizione_prodotto">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server"></asp:Localize>/
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceArticoloAbbr %>" runat="server"></asp:Localize>:
                                                            </span>
                                                            <input type="text" id="txt_descrizione_prodotto" class="form-control " />
                                                            <label id="lbl_prodotto_Edit_UC_descrizione_3_caratteri" for="txt_descrizione_prodotto" class="input-group-addon ">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server"></asp:Localize>
                                                            </label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-5 col-md-5 col-sm-12" style="padding-top: 30px;">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div id="ddl_Ricerca_XCategCommle" class="input-group">
                                                            <label class="input-group-addon alert-info " for="multiselCategCommle">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaCommerciale %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <select name="multiselCategCommle" multiple="multiple" id="multiselCategCommle" class="form-control"></select>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div id="ddl_Ricerca_XSpecie" class="input-group">
                                                            <label class="input-group-addon alert-info " for="multiselSpecie">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <select name="multiselSpecie" multiple="multiple" id="multiselSpecie" class="form-control"></select>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-5 col-md-5 col-sm-12">
                                                <div class="form-horizontal">
                                                    <div class="form-group">
                                                        <div id="ddl_Ricerca_XVarieta" class="input-group">
                                                            <label class="input-group-addon alert-info " for="multiselVarieta">
                                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server"></asp:Localize>:
                                                            </label>
                                                            <select name="multiselVarieta" multiple="multiple" id="multiselVarieta" class="form-control"></select>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-sm-12 text-right">
                                                <div class="btn btn-success xonne-btn-primary" onclick="resetFiltrototale();">
                                                    <span class="k-icon k-i-filter-clear"></span>
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PulisciFiltri %>" runat="server">Pulisci Filtri</asp:Localize>
                                                </div>
                                                <div class="btn btn-success xonne-btn-primary" id="prodotto_Edit_UC_Ricerca" >
                                                    <i class="fa fa-search"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server"></asp:Localize>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>


                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



        <div style="margin-top: 20px;">
            <div>


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
                        <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">

                            <div class="btn-group pull-right" role="group">
                                
                                <div class="btn btn-success xonne-btn-primary" onclick="NuovoElemento();">
                                    <i class="k-icon k-i-file-txt k-i-txt"></i>
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server">Nuovo</asp:Localize>
                                </div>

                            </div>

                        </div>

                    </div>
                    <input type="hidden" id="ElementoSelezionato" />
                </div>
            </div>

        </div>
    </div>

        <div class="panel-body" style="padding-top: 5px;">
            <div class="row">
                <div class="col-lg-12">
                    <div class="panel-group prodotto_Edit_UC_gridArea" style="display: none;">
                        <!--Griglia-->
                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                            <input type="hidden" id="chiave_prodotti" />
                            <input type="hidden" id="hdKendoProdotto_Valorizzazione" />
                            <div id="divKendoProdotto"></div>
                        </div>
                    </div>
                </div>
            </div>
         </div>

    <!-- Dialog Nuovo -->
    <div class="modal fade" id="modalNuovo" data-backdrop="static" data-keyboard="false">
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
                        <div class="form-group" style="background-color: #D2130F; padding: 15px 0;" hidden="hidden">
                            <label for="recipient-name" class="control-label" style="color: #fff;">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SelezionaTipoNuovoElemento %>" runat="server">Seleziona la tipologia del nuovo elemento</asp:Localize></label>
                            <select id="tipoNuovo" style="width: 100%; text-transform: uppercase;" disabled>
                            </select>
                            <!--<div id="tipoNuovo"></div>-->
                        </div>
                        <div class="form-group datiDaNascondere datiAppezza datiImpianto datiCatasto datiFabbricato datiMeteoDSS datiRaggruppamento">
                            <h4 class="modal-title" id="lbl_riferimenti">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ElementiDiRiferimento %>" runat="server">Elementi di riferimento</asp:Localize>
                            </h4>
                        </div>

                        <div class="form-group datiDaNascondere datiProdotto">
                            <label for="recipient-name" class="control-label">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaProdotto %>" runat="server">Categoria Prodotto:</asp:Localize>:
                            </label>
                            <select id="ddl_categorie_prodotti" class="form-control required" style="width: 100%"></select>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" id="btn_nuovo">
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


</asp:Content>



<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <div id="cont_script"></div>
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

        var qsVisibilita;
    </script>
    <input type="hidden" id="hf_Piva" runat="server" />

<script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

<script>
    var cIdPiva = "#<%=hf_Piva.ClientID %>";
</script>

</asp:Content>


