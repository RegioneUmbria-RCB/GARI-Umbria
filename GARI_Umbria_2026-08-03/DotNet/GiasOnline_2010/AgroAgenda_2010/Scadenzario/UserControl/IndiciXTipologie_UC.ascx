<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="IndiciXTipologie_UC.ascx.vb" Inherits="AgroAgenda_2010.IndiciXTipologie_UC" %>


<div id="pnlTipologieIndici" class="panel-group" style="padding-top: 3px">
    <div class="panel panel-primary">
        <div class="panel-heading" style="padding: 25px 7px 15px 7px; text-transform: uppercase">
            <h3 class="panel-title" style="font-size: 24px; padding-left: 15px;">
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologie %>" runat="server">Tipologie</asp:Localize> - 
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Indici %>" runat="server">Indici</asp:Localize>
            </h3>
        </div>

        <div class="panel-body">

            <%--DropDown per la scelta dell 'azienda
        <%--<div class="row" style="margin-bottom: 20px;">
                <div class="col-lg-12 col-md-12 col-xs-12">
                    <div class="input-group">
                        <label class="input-group-addon control-label alert-info" id="InXTip_UC_Azienda" for="InXTip_UC_ddlAzienda">Azienda</label>
                        <input type="text" id="InXTip_UC_ddlAzienda" name="InXTip_UC_ddlAzienda" class="form-control" aria-describedby="InXTip_UC_Azienda" />
                    </div>
                    <span style="color: red">NB: Le categorie create per un'azienda saranno visibili anche per quelle gerarchicamente figlie (se presenti).</span>
                </div>
            </div>--%>

            </div>

            <%--Categorie--%>
            <div class="row">
                 <div class="form-horizontal" style="margin-top: 5px;">
                    <div class="form-group">
                <%--<div class="col-lg-2 col-md-2 col-sm-12">--%>

                    <%--</div>--%>
                     <div class="col-lg-4 col-md-4 col-sm-12">
                      <label class="input-group-addon lbl_required" for="InXTip_UC_LsbAree" id="lblCategoria">
                          <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categoria %>" runat="server">Categoria</asp:Localize>
                      </label>
                     <select id="InXTip_UC_LsbAree" size="9"  style="width: 100%" onchange="InXTip_UC_LsbAree_onchange();"></select>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                     <label class="input-group-addon lbl_required"  for="InXTip_UC_LsbTipologie" id="lblTipologia">
                         <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize>
                     </label>
                    <select id="InXTip_UC_LsbTipologie" size="9"  style="width: 100%" onchange="InXTip_UC_LsbTipologie_onchange();"></select>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    &nbsp;
                </div>
              </div>
             </div>
            </div>
            <%--Tipologie--%>
            <div class="row" >
                  <div class="form-horizontal" style="margin-top: 20px;">
                    <div class="form-group">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <label class="input-group-addon lbl_required" for="Indici" id="lblIndici">
                                <asp:Localize meta:resourcekey="SceltaIndici" runat="server">Scelta Indici</asp:Localize>
                            </label>
                            <%--listBox Indici--%>
                            <select id="Indici" style="width: 100%" ></select>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <label class="input-group-addon lbl_required" for="IndiciXTipo" id="lblIndiciXTipo">
                                <asp:Localize meta:resourcekey="IndiciSceltiPerTipologia" runat="server">INDICI SCELTI PER TIPOLOGIA</asp:Localize>
                            </label>
                            <%--listBox IndiciTipologie--%>
                            <select  id="IndiciXTipo" style="width: 100%"></select>
                        </div>

                      <%--Kendo Switch per Campo Obbligatorio--%>
                        <div id="Switch_Campo_Obbligatorio" class="col-lg-4 col-md-3 col-sm-12" >
                            <label class="lbl_required switch" id="lbl_cb_obbligatorio_liv" for="cb_obbligatorio_liv" style="text-transform: uppercase;">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CampoObbligatorio %>" runat="server">Campo obbligatorio</asp:Localize>
                            </label>
                            <input type="checkbox" data-bind="visible: isVisible" id="cb_obbligatorio_liv"  name="cb_obbligatorio_liv" class="kendoSwitch"/>
                        </div>
                    </div>
                </div>
            </div>
            
            <br><br><br><br>    
            <%--Bottone per mostrare la Griglia grid_tipologiexindice--%>
            <div class="row">
                <div class="col-lg-4 col-md-12 col-lg-offset-8" >     
                  <div class="btn btn-success btn-block xonne-btn-primary" ID="btn_mostra_grid_tipologiexindice" >
                    <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">
                          <asp:Localize meta:resourcekey="MostraElencoAreeTipologieIndici" runat="server">MOSTRA ELENCO AREE, TIPOLOGIE ED INDICI</asp:Localize>
                      </span>
                 </div>
                <br><br><br><br>         
             </div>

           <%--Grid per visualizzare tutte le tipologie associate ad un indice--%>
            <div class="row" style="margin-top: 90px; margin-left: 18px;  margin-right: 18px;">
                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                        <div id="grid_tipologiexindice"></div>
                    </div>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/IndiciXTipologie_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/IndiciXTipologie_UC_ws_client.js")) %>"></script>
