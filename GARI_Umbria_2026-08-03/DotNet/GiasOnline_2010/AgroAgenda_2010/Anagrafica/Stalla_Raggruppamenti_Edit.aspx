<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Stalla_Raggruppamenti_Edit.aspx.vb" Inherits="AgroAgenda_2010.Stalla_Raggruppamenti_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveClientUrl("Stalla_Raggruppamenti_Edit.css?" & Application("GiasVersioneCorrente").ToString) %>"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="col-lg-12 col-md-12 gias-zootecnia-anagrafiche-raggruppamento-stalla-header">
        <div class="row">
            <div class="col-lg-10 col-md-10 gias-zootecnia-anagrafiche-raggruppamento-stalla-header-data" style="padding: 10px 15px; line-height: 1.6;">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>: <b id="LblCentro"></b>
                <br />
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stalla %>" runat="server">Stalla</asp:Localize>: <b id="LblStalla"></b>
            </div>
            <div class="col-lg-2 col-md-2 text-right gias-zootecnia-anagrafiche-raggruppamento-stalla-header-buttons" id="divSalva" style="display: none;">
                <div class="btn btn-success xi-btn-primary" id="btnSalva" onclick="ValidaxSubmit(0);">
                    <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                </div>
                <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" id="btnSalvaScrivi"
                        aria-haspopup="true" aria-expanded="false" style="margin-right: 8px !important;">
                    <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                </button>
                <ul class="dropdown-menu" style="right: 0; left: auto !important;" id="btnSalvaScrividd">
                    <li><a href="#" onclick="ValidaxSubmit(0)"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize></a></li>
                    <li><a href="#" onclick="ValidaxSubmit(1)"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaENuovo %>" runat="server">Salva e Nuovo</asp:Localize></a></li>
                    <li><a href="#" onclick="ValidaxSubmit(2)"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize></a></li>
                </ul>
            </div>
        </div>
    </div>

    <div class="col-lg-12 gias-zootecnia-anagrafiche-raggruppamento-stalla-tabs gias-container-margin-x" id="tabs" style="margin-bottom: 100px;">
        <ul id="tabstrip">
            <li class="k-state-active k-active"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiGenerali %>" runat="server">Dati Generali</asp:Localize></li>
        </ul>

        <div class="gias-zootecnia-anagrafiche-raggruppamento-stalla-tab-dati-generali gias-full-width-with-margin-x" id="tab_dati_generali">
            <div class="jumbotron gias-content-padding-x gias-content-padding-y">
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_nome"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nome %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="txt_nome" name="Nome" class="form-control gias-default-font-size" 
                                        placeholder="<asp:Localize meta:resourcekey='NomeDelGruppo' runat='server'></asp:Localize>" required />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_codice">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="txt_codice" name="Codice" class="form-control gias-default-font-size" 
                                        placeholder="<asp:Localize meta:resourcekey='CodiceDelGruppo' runat='server'></asp:Localize>" required />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_tipo"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipo %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="cmb_tipo" name="Tipo" class="form-control" required />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <div class="row" id="checkBDN">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RaggruppamentoDefaultIngressiBDN %>" runat="server"></asp:Localize>&nbsp;&nbsp;
                                        <input type="checkbox" id="switchBdn" aria-label="Raggruppamento default ingressi da BDN" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_inizio"><i class="fa fa-calendar"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataInizio %>" runat="server"></asp:Localize>
                                    </span>
                                    <input type="text" id="TxtValiditaInizio" class="form-control kendoDate" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_validita_fine"><i class="fa fa-calendar"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataFine %>" runat="server"></asp:Localize>
                                    </span>
                                    <input type="text" id="TxtValiditaFine" class="form-control kendoDate required" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_mq">Mq</span>
                                    <input type="text" id="TxtMq" class="form-control gias-default-font-size" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_specie"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Specie %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="Cmb_Specie" class="form-control gias-default-font-size" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_razza"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Razza %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="Cmb_Razza" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_stato"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Stato %>" runat="server"></asp:Localize></span>
                                    <input type="text" id="Cmb_Stato" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Stalla_Raggruppamenti_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Stalla_Raggruppamenti_Edit_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Stalla_Raggruppamenti_Edit_ws_client.js") %>" ></script>

</asp:Content>
