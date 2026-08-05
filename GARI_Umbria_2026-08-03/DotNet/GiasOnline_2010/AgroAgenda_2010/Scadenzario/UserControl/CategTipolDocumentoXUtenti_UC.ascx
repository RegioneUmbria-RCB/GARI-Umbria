<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CategTipolDocumentoXUtenti_UC.ascx.vb" Inherits="AgroAgenda_2010.CategTipolDocumentoXUtenti_UC" %>

<input type="hidden" id="hdRag_Soc_Azienda" runat="server" />
<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>

<div id="pnlCategTipolDocumentoXUtenti" class="panel-group" style="margin-top: 3px;">
    <div class="panel panel-primary">
        <div class="panel-heading" style="padding: 25px 7px 15px 7px; text-transform: uppercase">
            <h4 class="panel-title" style="font-size: 18px; padding-left: 15px;">Si Stanno Gestendo le Autorizzazioni dell'Azienda: <%=Rag_Soc_Azienda%>
            </h4>
        </div>
        <ul id="panelbarCategTipolDocumentoXUtenti">
            <li class="k-state-active k-active" id="panelBar_GridUtenti_GridCategoria">
                <span class="k-link k-state-selected k-selected">
                    <asp:Localize meta:resourcekey="SceltaAutorizzazioniCategoriaTipologia" runat="server">SCELTA AUTORIZZAZIONI PER CATEGORIA/TIPOLOGIA</asp:Localize>
                </span>
                <div class="row">
                    <div class="form-horizontal" style="margin-top: 20px; margin-bottom: 20px;">
                        <div id="btn_autorizza_grid_tipologiexindice">
                            <div class="col-lg-offset-2 col-lg-4 col-md-12 col-sm-12">
                                <div id="btn_autorizza_gestione_completa_grid_tipologiexindice" class="btn btn-success btn-block xonne-btn-primary" onclick="Autorizza_grid_tipologiexindice(<%=Tipo_Permesso_Documentale.Gestione_Completa%>);">
                                    <i class="fa fa-check"></i>
                                    <asp:Localize Text="<%$ Resources: AutorizzaGestioneCompleta %>" runat="server">AUTORIZZA GESTIONE COMPLETA</asp:Localize>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-12 col-sm-12">
                                <div id="btn_autorizza_sola_lettura_grid_tipologiexindice" class="btn btn-success btn-block xonne-btn-primary" onclick="Autorizza_grid_tipologiexindice(<%=Tipo_Permesso_Documentale.Lettura%>);">
                                    <i class="fa fa-check"></i>
                                    <asp:Localize Text="<%$ Resources: AutorizzaSolaLettura %>" runat="server">AUTORIZZA SOLA LETTURA</asp:Localize>
                                </div>                                
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-12 col-sm-12">
                            <div class="btn btn-danger btn-block" id="btn_togli_autorizza_grid_tipologiexindice">
                                <i class="fa fa-undo"></i>
                                <asp:Localize meta:resourcekey="TogliAutorizzazione" runat="server">TOGLI L'AUTORIZZAZIONE</asp:Localize>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div style="overflow: auto; margin-top: 20px; margin-bottom: 20px;">
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <%--Grid Utenti--%>
                            <div id="categtipodocxut_UC_griglia_utenti"></div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-12">
                            <%--Grid Categoria/Tipologia--%>
                            <div id="categtipodocxut_UC_griglia_CategTip"></div>
                        </div>
                    </div>
                </div>
            </li>

            <li class="k-state-active k-active" id="panelBar_GridUtentiCategTipo">
                <span class="k-link k-state-selected k-selected">
                    <asp:Localize meta:resourcekey="AutorizzazioniEsistenti" runat="server">AUTORIZZAZIONI GIÀ ESISTENTI</asp:Localize></span>
                <div class="row">
                    <div class="form-horizontal">
                        <div class="col-lg-12 col-md-12 col-sm-12" style="margin-top: 20px;">
                            <%--Grid Categoria/TipologiaxUtenti--%>
                            <div id="categtipodocxut_UC_griglia_CategTipxUtenti"></div>
                        </div>
                    </div>
                </div>
            </li>
        </ul>
    </div>
</div>



<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/CategTipolDocumentoXUtenti_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/CategTipolDocumentoXUtenti_UC_ws_client.js")) %>"></script>
<script type="text/javascript">
    var Rag_Soc_Azienda = "#<%=hdRag_Soc_Azienda.ClientID() %>";
</script>
