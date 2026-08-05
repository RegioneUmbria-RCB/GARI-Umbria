<%@ Page Title="Ricerca Trasferimenti" Language="vb" AutoEventWireup="false" CodeBehind="RicercaTrasferimenti.aspx.vb"
    Inherits="AgroAgenda_2010.RicercaTrasferimenti" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%--<%@ Register TagPrefix="uc" TagName="TrasferimentiMenuUC" Src="./TrasferimentiMenuUC.ascx" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }

        .text-uppercase {
            text-transform: uppercase;
        }

        .km-widget.km-buttongroup {
            margin: 0 auto;
        }

        .panel-body {
            display: none;
        }

        #tabstrip_Filtri .jumbotron {
            margin-bottom: 0;
            padding-bottom: 0;
        }

        #TipoOutput {
            font-size: 12px;
            margin-top: 5px;
        }

        #gridDettagli {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <%--<uc:TrasferimentiMenuUC id="TrasferimentiMenuUC1" runat="server" />--%>

    <div class="panel-group searchArea">
        <div class="panel-body">

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div id="tabstrip_Filtri">

                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li class="k-state-active k-active" id="tab1">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltriGenerali %>" runat="server">Filtri Generali</asp:Localize></li>
                            <li id="tab2">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltroProdotti %>" runat="server">Filtro Prodotti</asp:Localize></li>
                        </ul>

                        <div class="tab-pane fade in" id="tabGenerale" style="overflow: auto">
                            <div class="jumbotron">
                                <div class="row">
                                    <div class="form-group col-lg-3 col-sm-5">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_DataDal" for="txt_DataDal">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDal %>" runat="server">Data dal</asp:Localize>
                                            </span>
                                            <input id="txt_DataDal" name="txt_DataDal" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-3 col-sm-5">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_DataAl" for="txt_DataAl">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataAl %>" runat="server">Data al</asp:Localize>:
                                            </span>
                                            <input id="txt_DataAl" name="txt_DataAl" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="form-group col-lg-5 col-sm-10">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_des_TestataGriglia" for="txt_des_TestataGriglia">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server">Descrizione</asp:Localize>:
                                            </span>
                                            <asp:TextBox ID="txt_des_TestataGriglia" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="tab-pane fade in tabFiltroProdotti" id="tabFiltroProdotti" style="overflow: auto">
                            <div class="jumbotron">
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="id_multiselCategorie">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categorie %>" runat="server">Categorie</asp:Localize>:
                                                </label>
                                                <select name="multiselCategorie" multiple="multiple" id="id_multiselCategorie" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="id_multiselProdotti">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotti %>" runat="server">Prodotti</asp:Localize>:
                                                </label>
                                                <select name="multiselProdotti" multiple="multiple" id="id_multiselProdotti" class="form-control"></select>
                                                <label class="input-group-addon ">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserireTreCaratteriDellaDescrizione %>" runat="server">(Inserire 3 caratteri della descrizione)</asp:Localize>
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-5 col-md-5 col-sm-12">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="id_multiselSpecie">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server">Specie</asp:Localize>:
                                                </label>
                                                <select name="multiselSpecie" multiple="multiple" id="id_multiselSpecie" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-addon lbl_required" for="id_multiselVarieta">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server">Varietà</asp:Localize>:
                                                </label>
                                                <select name="multiselVarieta" multiple="multiple" id="id_multiselVarieta" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 xonne-ricerca-trasferimenti-tab">
                    <div class="xonne-btn-right">
                        <div class="btn btn-success xonne-btn-primary" id="btn_ricerca">
                            <span class="fa fa-search lampeggiante xonne-search"></span><span class="lampeggiante">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Ricerca %>" runat="server">Ricerca</asp:Localize></span>
                        </div>
                    </div>
                    
                    <div class="btn buttonClass text-uppercase xonne-tab-group" id="TipoOutput">
                        <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaTestate %>" runat="server">Visualizza Testate</asp:Localize></span>
                        <span class="xonne-tab xonne-tab-50"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, VisualizzaDettagli %>" runat="server">Visualizza Dettagli</asp:Localize></span>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <div id="gridGenerale" class="panel-group elencoLavorazioniArea">
        <!--Griglia generale-->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="tab_testata_griglia_trasferimenti"></div>
        </div>
    </div>

    <div id="gridDettagli" class="panel-group elencoLavorazioniArea">
        <!--Griglia dettagli-->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="tab_testata_griglia_trasferimenti_dettagli"></div>
        </div>
    </div>

    <!-- fine container -->

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdIdTrasferimenti" runat="server" value="" />
    <input type="hidden" id="hdKendo_Trasferimenti" runat="server" />
    <input type="hidden" id="hdKendo_TrasferimentiDettagli" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdTrasferimenti = "#<%=hdIdTrasferimenti.ClientID() %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ricercaTrasferimenti_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ricercaTrasferimenti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ricercaTrasferimenti_jQueryDocReady.js") %>"></script>

</asp:Content>
