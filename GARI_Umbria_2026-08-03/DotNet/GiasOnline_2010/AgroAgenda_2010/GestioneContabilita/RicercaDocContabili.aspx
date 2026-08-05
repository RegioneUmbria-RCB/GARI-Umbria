<%@ Page Title="Ricerca Documenti" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="RicercaDocContabili.aspx.vb" Inherits="AgroAgenda_2010.RicercaDocContabili" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    .errorClass {
        border-color:#D41E1A;
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

    .fixed-header {
        top:0;
        position:fixed;
        width:auto;
        z-index: 1;
    }

    #gridControlliInvio {
        height: calc(100% - 20px);
    }

    /* Inizio stili per test menu filtri laterale */
     .xo-ricerca-doc-contabili-filtri-action-container {
         margin-right: 20px;
         margin-left: 20px;
     }

     @media only screen and (min-width: 997px) {
         .fixed-header {
            top:50px;
            position:fixed;
            width:auto;
            z-index: 1;
         }
     }
      @media only screen and (max-width: 996px) {
         .fixed-header {
            top:100px;
            position:fixed;
            width:auto;
            z-index: 1;
         }
      }

    /* Fine stili per menu laterale */
    </style>

    <script>
        function toggleMenu() {
            const sidebar = document.getElementById('xoRicercaDocFiltriContainer');
            sidebar.classList.toggle('hidden-sidebar');
            const sidebarAction = document.getElementById('xoRicercaDocToggleFiltri');
            sidebarAction.classList.toggle('hidden-sidebar');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoPomodoro" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneNuovoAllegato" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneVisualizaAllegato" />

     <div id="searchArea" class="panel-group searchArea xo-ricerca-doc-contabili-container" style="display:none">
        <div class="panel-body" style="padding-top:5px;">

            <% If Master.Master_versione = "2022" Then %>
            <div id="xoRicercaDocToggleFiltri" class="btn gias-btn-primary gias-btn-toggle-filter-sidebar hidden-sidebar" onclick="toggleMenu()">
                <span>
                    <svg role="img" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="sliders" class="svg-inline--fa fa-sliders fa-lg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="currentColor" d="M0 416c0-17.7 14.3-32 32-32l54.7 0c12.3-28.3 40.5-48 73.3-48s61 19.7 73.3 48L480 384c17.7 0 32 14.3 32 32s-14.3 32-32 32l-246.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 448c-17.7 0-32-14.3-32-32zm192 0c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zM384 256c0-17.7-14.3-32-32-32s-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32zm-32-80c32.8 0 61 19.7 73.3 48l54.7 0c17.7 0 32 14.3 32 32s-14.3 32-32 32l-54.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 288c-17.7 0-32-14.3-32-32s14.3-32 32-32l246.7 0c12.3-28.3 40.5-48 73.3-48zM192 64c-17.7 0-32 14.3-32 32s14.3 32 32 32s32-14.3 32-32s-14.3-32-32-32zm73.3 0L480 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l-214.7 0c-12.3 28.3-40.5 48-73.3 48s-61-19.7-73.3-48L32 128C14.3 128 0 113.7 0 96S14.3 64 32 64l86.7 0C131 35.7 159.2 16 192 16s61 19.7 73.3 48z"></path></svg>
                </span>
            </div>

            <!-- FILTRI Matteo qui iniziano gli elementi che implementano i filtri -->
            <div id="xoRicercaDocFiltriContainer" class="row xo-ricerca-doc-filtri-container gias-right-sidebar-filters hidden-sidebar">
            <% Else %>
            <div class="row xo-ricerca-doc-filtri-container">
            <% End If %>
                <div class="col-lg-12 col-md-12 col-sm-12 xo-ricerca-doc-contabili-filtri-container">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;" >
                        
                            <div id="tabstrip_Filtri">
                            <ul  class="nav nav-tabs" rolw="tablist" id="tabs">
                                <li class="k-state-active k-active" id="tab1">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltriGenerali %>" runat="server"></asp:Localize>
                                </li>
                                <li id="tab2">
                                    <asp:Localize Text="<%$ Resources: FiltroContattiAgenti %>" runat="server"></asp:Localize>
                                </li>
                                <li id="tab3">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltroProdotti %>" runat="server"></asp:Localize>
                                </li>
                            </ul>
                        
                        
                        <%--  <ul class="nav nav-tabs" rolw="tablist" id="tabs">
                            <li class="active"><a href="#tabFiltroDocumento" data-toggle="tab" id="a_tabFiltroDocumento">Filtri Generali</a></li>
                            <li><a href="#tabFiltroClienti" data-toggle="tab" id="a_tabFiltroClienti">Filtro Clienti e Agenti</a></li>
                            <li><a href="#tabFiltroProdotti" data-toggle="tab" id="a_tabFiltroProdotti">Filtro Prodotti</a></li>
                        </ul>--%>


                        
                            <!-- tab Filtro Documenti -->
                            <div class="tab-pane fade active in" id="tabFiltroDocumento" style="overflow: auto">
                                <div class="jumbotron">   

                                        <div class="row" id="rowCentroAziendale">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" id="lbl_centro_aziendale" for="id_multiselCentroAziendale">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselCentroAziendale" multiple="multiple" ID="id_multiselCentroAziendale" class="form-control" 
                                                            data-placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Tutti %>' runat='server'></asp:Localize>"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row" id="rowGruppoDocumento">
                                        <div class="col-lg-9 col-sm-9">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" id="lbl_gruppo_documento" for="id_multiselGruppoDocumento">
                                                            <asp:Localize Text="<%$ Resources: GruppoDocumento %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselGruppoDocumento" multiple="multiple" ID="id_multiselGruppoDocumento" class="form-control" 
                                                            data-placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Tutti %>' runat='server'></asp:Localize>"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" id="rowTipiDocumento">
                                        <div class="col-lg-9 col-sm-9">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" id="lbl_causale_movimento" for="id_multiselCausale">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipiDocumento %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselCausale" multiple="multiple" ID="id_multiselCausale" class="form-control" 
                                                            data-placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Tutti %>' runat='server'></asp:Localize>"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" style="display:none;">
                                        <div class="col-lg-2 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_Nr" for="TxtDocNumeroSin">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Numero %>" runat="server"></asp:Localize>:
                                                        </span>
                                                        <asp:TextBox ID="TxtDocNumeroSin" runat="server" CssClass="form-control" MaxLength="255">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-1 col-sm-3">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <asp:TextBox ID="TxtDocNumero" runat="server" type="number" min="0" step="1" CssClass="form-control" MaxLength="8">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-1 col-sm-3">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <asp:TextBox ID="TxtDocNumeroDes" runat="server" CssClass="form-control" MaxLength="255">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_NrRiga" for="txt_NrRiga">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroRiga %>" runat="server"></asp:Localize>:
                                                        </span>
                                                        <asp:TextBox ID="TxtNrRiga" runat="server" CssClass="form-control" MaxLength="255">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        &nbsp;
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                        <div class="row">
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifDal" for="Txt_DataRegDal">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DaDataMovimento %>" runat="server"></asp:Localize>:
                                                        </span>
                                                        <input ID="Txt_DataRegDal" name="Txt_DataRegDal" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_DataRifAl" for="Txt_DataRegAl">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ADataMovimento %>" runat="server"></asp:Localize>:
                                                        </span>
                                                        <input ID="Txt_DataRegAl" name="Txt_DataRegAl" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                     
                                    <div class="row" id="rowDescrizioneDocumento">
                                        <div class="col-lg-9 col-sm-9">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon lbl_required" id="lbl_operazione_des" for="txt_operazione">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server"></asp:Localize>:
                                                        </span>
                                                        <asp:TextBox ID="txt_operazione" ClientIDMode="Static" runat="server" CssClass="form-control">  </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                        <div class="row">
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="div_chk_ddt_non_fatturati">
                                            <div class="form-group">
                                                <label class="lbl_required" id="lbl_ddt_non_fatturati" for="cb_ddt_non_fatturati">
                                                    <asp:Localize Text="<%$ Resources: SoloDdtNonFatturati %>" runat="server"></asp:Localize>
                                                </label>
                                                <input type="checkbox" id="cb_ddt_non_fatturati" name="cb_ddt_non_fatturati" class="kendoSwitch" />
                                            </div>
                                        </div>
                                            <div class="col-lg-4 col-md-12 col-sm-12" id="div_chk_ordini_non_spediti">
                                            <div class="form-group">
                                                <label class="lbl_required" id="lbl_ordini_non_spediti" for="cb_ordini_non_Spediti">
                                                    <asp:Localize Text="<%$ Resources: SoloOrdiniNonEvasi %>" runat="server"></asp:Localize>
                                                </label>
                                                <input type="checkbox" id="cb_ordini_non_Spediti" name="cb_ordini_non_Spediti" class="kendoSwitch" />
                                            </div>
                                        </div>
                                        </div>


                                </div>
                            </div>

                            <!-- tab Filtro cliente -->
                            <div class="tab-pane fade in" id="tabFiltroClienti" style="overflow: auto">
                                <div class="jumbotron">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" for="multiselContatti">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contatti %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <select name="multiselContatti" multiple="multiple" ID="multiselContatti" class="form-control"></select>
                                                    <label class="input-group-addon ">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server"></asp:Localize>
                                                    </label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <%-- <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="multiselClienti">Contatti::</label>
                                                        <select name="multiselClienti" multiple="multiple" ID="id_msClienti" class="form-control" data-placeholder="Tutti i contatti"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>--%>

                                    <div class="row">
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_msAgenti">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Agenti %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselAgenti" multiple="multiple" ID="id_msAgenti" class="form-control" 
                                                            data-placeholder="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, TuttiGliAgenti %>' runat='server'></asp:Localize>"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>                           

                            <!-- tab Filtro Prodotti -->
                            <div class="tab-pane fade in tabFiltroProdotti" id="tabFiltroProdotti" style="overflow: auto">
                                <div class="jumbotron">
                                    <div class="row" id="row_filtro_categorie">
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_multiselCategorie">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categorie %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselCategorie" multiple="multiple" ID="id_multiselCategorie" class="form-control"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_multiselProdotti">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotti %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselProdotti" multiple="multiple" ID="id_multiselProdotti" class="form-control"></select>
                                                        <label class="input-group-addon ">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server"></asp:Localize>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_multiselCategCommle">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CategoriaCommerciale %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselCategCommle" multiple="multiple" ID="id_multiselCategCommle" class="form-control"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_multiselSpecie">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselSpecie" multiple="multiple" ID="id_multiselSpecie" class="form-control"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <label class="input-group-addon lbl_required" for="id_multiselVarieta">
                                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server"></asp:Localize>:
                                                        </label>
                                                        <select name="multiselVarieta" multiple="multiple" ID="id_multiselVarieta" class="form-control"></select>
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

                <% If Master.Master_versione = "2022" Then %>
                <!-- Matteo, ho spostato i bottoni in questa posizione per metterli all'interno della side di filtro -->
                <div class="xo-ricerca-doc-contabili-filtri-action-container">
                    <div class="xonne-btn-right">
                        <div class="btn btn-success xonne-btn-primary" id="btn_ricerca">
                            <span class="fa fa-search lampeggiante xonne-search"></span><span class="lampeggiante">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize></span>
                        </div>
                        <div class="btn btn-danger" id="btn_prepara_nuovo_doc">
                            <span class="fa fa-plus"></span>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-warning" id="btn_pulisci_filtri">
                            <span class="fa fa-eraser"></span>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PulisciFiltri %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-success" id="btn_esegui_Fatturazione">
                            <span class="fa fa-success"></span>
                            <asp:Localize Text="<%$ Resources: GeneraFattura %>" runat="server"></asp:Localize>
                        </div>
                    </div>
                    <div class="btn buttonClass text-uppercase xonne-tab-group" id="TipoOutput">
                        <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaTestate %>" runat="server">Visualizza Testate</asp:Localize></span>
                        <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: VisualizzaRighe %>" runat="server"></asp:Localize></span>
                    </div>
                </div>
                <!-- Matteo, FINE ho spostato i bottoni in questa posizione per metterli all'interno della side di filtro -->
                <% End If %>
            </div>
            <!-- FILTRI Matteo fine elementi che implementano i filtri -->
            
            <!-- FILTRI-COMANDI Matteo qui iniziano i bottoni per attivare i filtri e per decidere se visualizzare testate o righe -->
            <div class="row xo-ricerca-doc-filtri-azioni">
                
                <% If Master.Master_versione = "2022" Then %>

                    <!-- Matteo questa parte dei bottoni è già sotto la direzione della master2022, questo per ricordarmi che 
                    il controllo sulla versione NON è stato messo per i filtri ma era già impostato prima -->
                    <div class="col-lg-12 col-md-12 col-sm-12 xonne-ricerca-trasferimenti-tab">
                        <%--<div class="xonne-btn-right">
                            <div class="btn btn-success xonne-btn-primary" id="btn_ricerca">
                                <span class="fa fa-search lampeggiante xonne-search"></span><span class="lampeggiante">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize></span>
                            </div>
                            <div class="btn btn-danger" id="btn_prepara_nuovo_doc">
                                <span class="fa fa-plus"></span>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server"></asp:Localize>
                            </div>
                            <div class="btn btn-warning" id="btn_pulisci_filtri">
                                <span class="fa fa-eraser"></span>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PulisciFiltri %>" runat="server"></asp:Localize>
                            </div>
                            <div class="btn btn-success" id="btn_esegui_Fatturazione">
                                <span class="fa fa-success"></span>
                                <asp:Localize Text="<%$ Resources: GeneraFattura %>" runat="server"></asp:Localize>
                            </div>
                        </div>--%>
                    
                        <%--<div class="btn buttonClass text-uppercase xonne-tab-group" id="TipoOutput">
                            <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaTestate %>" runat="server">Visualizza Testate</asp:Localize></span>
                            <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: VisualizzaRighe %>" runat="server"></asp:Localize></span>
                        </div>--%>
                    </div>

                <% Else %>

                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="btn btn-success buttonClass text-uppercase" id="TipoOutput" >
                            <span>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaTestate %>" runat="server"></asp:Localize>
                            </span>
                            <span>
                                <asp:Localize Text="<%$ Resources: VisualizzaRighe %>" runat="server"></asp:Localize>
                            </span>
                        </div>
                        <div class="btn btn-success buttonClass" id="btn_ricerca" style="margin-left:10px;">
                            <span class="fa fa-search xo-agronica-style"></span>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-danger buttonClass" id="btn_prepara_nuovo_doc">
                            <span class="fa fa-plus"></span>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nuovo %>" runat="server"></asp:Localize>
                        </div>
                        <div class="btn btn-warning buttonClass" style="margin-left: 2%" id="btn_pulisci_filtri">
                            <span class="fa fa-eraser"></span>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PulisciFiltri %>" runat="server"></asp:Localize>
                        </div>

                        <div class="btn btn-success buttonClass" style="margin-left: 2%" id="btn_esegui_Fatturazione">
                            <span class="fa fa-success"></span>
                            <asp:Localize Text="<%$ Resources: GeneraFattura %>" runat="server"></asp:Localize>
                        </div>
                    </div>

                <% End If %>

            </div>
            <!-- FILTRI-COMANDI Matteo fine bottoni per attivare i filtri e per decidere se visualizzare testate o righe -->
        </div>
        
        <!-- Griglia Report Vendite -->
        <div id="gridAreaTestata" class="panel-group gridAreaTestata" style="display: none;">
            <div class="panel-body" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                <div id="tab_testata_griglia_report_vendite"></div>
                <div id="tab_testata_griglia_report_acquisti"></div>
                <div id="tab_testata_griglia_report_conferimenti"></div>
                <div id="tab_testata_griglia_report_contratti"></div>
            </div>
        </div>

        <!-- Pivot Report Vendite -->
        <div id="girdAreaDettaglio" class="panel-group gridAreaDettaglio" style="display: none;">
            <div class="panel-body" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                <div id="tab_dettaglio_griglia_report_vendite"></div>
                <div id="tab_dettaglio_griglia_report_acquisti"></div>
                <div id="tab_dettaglio_griglia_report_conferimenti"></div>
                <div id="tab_dettaglio_griglia_report_contratti"></div>
            </div>
        </div>
    
    <!-- fine container -->
    </div>

    <!-- dialogs varie -->
    <div id="campionamentoWindow" class="panel-group" style="display:none;">
        <iframe class="k-content-frame" name="target_iframe" src="about:blank"></iframe>
    </div>

    <!-- Dialog Nuovo i18n: Attualmente non è mai visualizzato, quindi lascio indietro la traduzione -->
    <div class="modal fade" id="modalNuovo" data-backdrop="static" data-keyboard="false" >
        <div class="modal-dialog">
            <div class="modal-content">
                <form id="form_nuovo_elemento" method="get" action="">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="lbl_new_item">
                            Creazione Nuovo Documento
                        </h4>
                    </div>
                    <div class="modal-body">
                        <%--select riempita via js in jquery doc ready--%>
                        <div class="form-group" style="background-color: #D2130F; padding: 15px 0;">
                            <label for="recipient-name" class="control-label" style="color: #fff;">
                                Seleziona la tipologia del nuovo documento</label>
                            <div class="form-group">
                                <label for="ddl_nuovoOrdine" class="control-label">Ordini:</label>
                                <select id="ddl_nuovoOrdine" class="form-control required" style="width:100%"></select>
<%--                                <input type="hidden" id ="txt_latcentro" value="" />
                                <input type="hidden" id ="txt_loncentro" value="" />--%>
                            </div>
                             <div class="form-group">
                                <label for="ddl_nuovoDocVendita" class="control-label">Documenti di vendita:</label>
                                <select id="ddl_nuovoDocVendita" class="form-control required" style="width:100%"></select>
<%--                                <input type="hidden" id ="txt_latcentro" value="" />
                                <input type="hidden" id ="txt_loncentro" value="" />--%>
                            </div>
                            <div class="form-group">
                                <label for="ddl_nuovoDocAcquisto" class="control-label">Documenti di acquisto:</label>
                                <select id="ddl_nuovoDocAcquisto" class="form-control required" style="width:100%"></select>
<%--                             <input type="hidden" id ="txt_latcentro" value="" />
                                <input type="hidden" id ="txt_loncentro" value="" />--%>
                            </div>
                        </div>
<%--                        <div class="form-group datiDaNascondere datiAppezza">
                            <i class="fa fa-info-circle"></i><small>L'Appezzamento può far riferimento a un Campo.</small><br />
                            <i class="fa fa-info-circle"></i><small>Se il Campo viene lasciato vuoto, si crea un Appezzamento libero.</small>
                        </div>--%>
                    </div>
                    <div class="modal-footer">
                        <!--<button type="button" class="btn btn-default" data-dismiss="modal">
                            <i class="fa fa-times"></i>
                            Chiudi</button>-->
                        <button type="button" class="btn btn-success" id="btn_nuovo">
                            <i class="fa fa-plus"></i>Crea
                        </button>
                    </div>
                </form>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->

    <div id="windowControlliInvio" class="modal">
        <div id="gridControlliInvio"></div>
    </div>

    <script id="rowTemplate" type="text/x-kendo-template">
    # if (member.name.indexOf("Data_Movimento") === 0 && member.name !== "Data_Movimento") { #
        #: kendo.toString(kendo.parseDate(member.caption), "d") #
    # } else { #
        #: member.caption #
    # } #
    </script>

    <script id="columnTemplate" type="text/x-kendo-template">
    # if (member.name.indexOf("Data_Movimento") === 0 && member.name !== "Data_Movimento") { #
        #: kendo.toString(kendo.parseDate(member.caption), "d") #
    # } else { #
        #: member.caption #
    # } #
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <!--dialogs varie-->    
    <div id="confermaEliminazioneDialog"></div>
    <div id="confermaValorizzazioneConferimentiDialog"></div>
    <div id="confermaSbloccoDialog"></div>
    <div id="confermaBloccoDialog"></div>


    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdType" runat="server" />
    <input type="hidden" id="hdDocType" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoBlocco" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoSblocco" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoGestionePrezziLettura" runat="server" />
    <input type="hidden" id="hdContattiAcc4ConGerarchia" runat="server" />
    <input type="hidden" id="hf_filtroMateriePrimeConferimento" runat="server" />
    <input type="hidden" id="hf_Modulo_Anagrafe_Log" runat="server" />
    <input type="hidden" id="hf_ModalitaFatturazione" runat="server" />
    <input type="hidden" id="hf_GestioneWorkflow" runat="server" />
    <input type="hidden" id="hf_GestioneGruppiMerce" runat="server" />
    <input type="hidden" id="hf_IB_Controlli" runat="server" />
    <input type="hidden" id="hf_DS_RisControlli" />


    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cType = "#<%=hdType.ClientID() %>";
        var cDocType = "#<%=hdDocType.ClientID() %>";
        var cModalitaFatturazione = "#<%=hf_ModalitaFatturazione.ClientID() %>";
        var cUtenteAbilitatoBlocco = "#<%=hdUtenteAbilitatoBlocco.ClientID() %>";
        var cUtenteAbilitatoSblocco = "#<%=hdUtenteAbilitatoSblocco.ClientID() %>";
        var cUtenteAbilitatoGestionePrezziLettura = "#<%=hdUtenteAbilitatoGestionePrezziLettura.ClientID() %>";
        var ccontattiAcc4ConGerarchia = "#<%=hdContattiAcc4ConGerarchia.ClientID() %>";
        var hf_filtroMateriePrimeConferimento = "#<%=hf_filtroMateriePrimeConferimento.ClientID() %>";
        var modulo_anagrafe_log = $("#<%=hf_Modulo_Anagrafe_Log.ClientID() %>").val().split("|");
        var setupGestioneWorkflow = $("#<%=hf_GestioneWorkflow.ClientID() %>").val().split("|");
        var setupGestioneGruppiMerce = $("#<%=hf_GestioneGruppiMerce.ClientID() %>").val().split("|");
        var setupIB_Controlli = $("#<%=hf_IB_Controlli.ClientID() %>").val();
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaDocContabili_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaDocContabili_globali.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaDocContabili.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaDocContabili_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript" language="javascript">
        function ChiudiPopupCampionamento() {
            try {
                 let dialog = $("#campionamentoWindow").data("kendoWindow");
                 dialog.close();
            }
            catch (e) {
            }
        }

    </script>

    <script id="template_Kendo_Btn_Fatturazione_Testata" type="text/x-kendo-template">
        <div id="btn_avvia_fatturazione_testata" style="display:none;margin-left:20px;" class="btn btn-success" onclick="schedula_Fatturazione();">
            <span></span><asp:Localize Text="<%$ Resources: PreparazioneFatture %>" runat="server"></asp:Localize>
        </div>
    </script>
    <script id="template_Kendo_Btn_Fatturazione_Dettaglio" type="text/x-kendo-template">
        <div id="btn_avvia_fatturazione_dettaglio" style="display:none;margin-left:20px;" class="btn btn-success" onclick="schedula_Fatturazione();">
            <span></span><asp:Localize Text="<%$ Resources: PreparazioneFatture %>" runat="server"></asp:Localize>
        </div>
    </script>

    <script id="template_Kendo_Btn_PassaggioStato" type="text/x-kendo-template">
        <div class="btn btn-success btngrid_passaggiostato" style="display:none;margin-left:20px;" onclick="RichiamaPassaggioStato();">
            <span></span><asp:Localize Text="<%$ Resources: PassaggioDiStato %>" runat="server"></asp:Localize>
        </div>
    </script>

    <script id="template_Kendo_Btn_IBControlliInvio" type="text/x-kendo-template">
        <div class="btn btn-warning btngrid_ibcontrolli" style="display:none;margin-left:20px;" onclick="IB_Controlli_PreInvio();">
            <span>Verifica Pre-Esportazione</span>
        </div>
    </script>

    <script id="template_Kendo_Btn_ValorizzazioneConferimenti" type="text/x-kendo-template">
        <div class="btn btn-warning btngrid_valorconf" style="display:none;margin-left:20px;" onclick="ValorizzazioneConferimentiConferma();">
            <span>Valorizzazione Conferimenti</span>
        </div>
    </script>


</asp:Content>
