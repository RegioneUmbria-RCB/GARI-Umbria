<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BeniConfezionamentoUC.ascx.vb" Inherits="AgroAgenda_2010.BeniConfezionamentoUC" %>

<style type="text/css">
    .errorClass {
        border-color: #D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    }
</style>

<div id="panelAreaBeniConfezionamento" class="panel-group searchArea" style="opacity: 1;">
    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100%;">
                    <div class="container_tabBeniConfezionamento" style="padding: 0; /*margin-bottom: 70px*/">
                        <div id="tabstrip_tabBeniConfezionamento">
                            <ul>
                                <li id="tabImballiIN" class="text-uppercase">
                                    <asp:Localize Text="<%$ Resources: ImballiInEntrata %>" runat="server"></asp:Localize>
                                </li>
                                <li id="tabImballiOUT" class="k-state-active k-active text-uppercase">
                                    <asp:Localize Text="<%$ Resources: ImballiResi %>" runat="server"></asp:Localize>
                                </li>
                            </ul>

                            <!-- TAB Carico -->
                            <div class="panel-group Carico" id="a_tabBeniConf_Carico">
                                <div class="row" id="id_riga_destinazione">
                                    <div class="col-lg-8 col-md-10 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" for="cmbDestinazione">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Destinazione %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <select name="Destinazione" id="cmbDestinazione" class="form-control"></select>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row" id="id_riga_carico">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_carico"></div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row" style="margin-top: 20px;" id="id_riga_lblImballiEredita">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <label class="input-group-addon" id="lblImballiEredita">
                                            <asp:Localize Text="<%$ Resources: ImballiConsideratiNeiProdotti %>" runat="server"></asp:Localize>
                                        </label>
                                    </div>
                                </div>

                                <div class="row" id="id_riga_carico_eredita">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_carico_eredita"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- TAB Scarico -->
                            <div class="panel-group Scarico" id="a_tabBeniConf_Scarico">
                                <div class="row">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="input-group">
                                            <label class="input-group-addon" id="lbl_des_num_doc_scarico" for="inNumDocScarico">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NrDocumentoAbbr %>" runat="server"></asp:Localize>:
                                            </label>
                                            <div class="row" id="numDocTxtScaricoRow">
                                                <div class="col-lg-2 col-md-2 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocSinScarico" id="inNumDocSinScarico" class="form-control k-content" 
                                                        title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Prefisso %>' runat='server'></asp:Localize>" 
                                                        style="text-transform: uppercase;" onchange="inNumDocSinScarico_change()" />
                                                </div>
                                                <div class="col-lg-2 col-md-2 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocScarico" id="inNumDocScarico" class="form-control k-content" required />
                                                </div>
                                                <div class="col-lg-2 col-md-2 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocDesScarico" id="inNumDocDesScarico" class="form-control k-content" 
                                                        title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Suffisso %>' runat='server'></asp:Localize>" 
                                                        style="text-transform: uppercase;" onchange="inNumDocDesScarico_change()" />
                                                </div>
                                                <div class="col-lg-1 col-md-1 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocLockScarico" id="inNumDocLockScarico" type="checkbox" class="kendoSwitch" />
                                                </div>
                                                <div class="col-lg-3 col-md-3 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocDDLScarico" id="inNumDocDDLScarico" class="form-control k-content" />
                                                </div>
                                                <div class="col-lg-2 col-md-2 col-sm-12" style="padding-right: 7px;">
                                                    <input name="inNumDocShowScarico" id="inNumDocShowScarico" class="form-control k-content" 
                                                        title="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, VisualizzaNumDocCompleto %>' runat='server'></asp:Localize>" 
                                                        disabled="disabled" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row" id="id_riga_provenienza">
                                    <div class="col-lg-8 col-md-10 col-sm-12">
<%--                                        <div class="form-horizontal">
                                            <div class="form-group">--%>
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" for="cmbProvenienza">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Provenienza %>" runat="server"></asp:Localize>:
                                                    </label>
                                                    <select name="Provenienza" id="cmbProvenienza" class="form-control"></select>
                                                </div>
<%--                                            </div>
                                        </div>--%>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-lg-12 col-md-12 col-sm-12" id="id_riga_scarico">
                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="tab_griglia_scarico"></div>
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

<script id="templateNumDocDDLScarico" type="text/x-kendo-template">
    <span>
        #: Sigla # - <strong>#: PrefissoSuffisso_Des #</strong> <br>(Prefisso = <em>"#: Doc_Numero_Sin #"</em> <br>&nbsp;Suffisso = <em>"#: Doc_Numero_Des #"</em>)
    </span>
</script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiTabelle_ws_client.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/BeniConfezionamentoUC_globali.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/BeniConfezionamentoUC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/BeniConfezionamentoUC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/GestioneMagazzini/BeniConfezionamentoUC_ws_client.js")) %>"></script>
