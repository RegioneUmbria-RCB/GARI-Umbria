<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.master" 
    CodeBehind="Scad_Impostazioni.aspx.vb" Inherits="AgroAgenda_2010.Scad_Impostazioni" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    .evidenziato td, .watable tbody tr.evidenziato:hover td {
        background-color: yellow !important;
    }
    
    .buttonClass {
        margin: 0 0 10px 1px;
    }

</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <input id="ID_Avviso" type="hidden" value="" />
    
    <div class="panel-group addEditArea" style="display:none;">
        <div class="panel-body" style="padding-top:30px;">

            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="LblArea" for="DdlArea">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Categoria %>" runat="server">Categoria</asp:Localize>
                        </label>
                        <select id="DdlArea" data-live-search="true" aria-describedby="LblArea" data-container="body" class="form-control"></select>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-12" id="id_tipologia">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <label class="input-group-addon" for="cmbTipologia">
                                    <asp:Localize meta:resourcekey="TipologiaDocumento" runat="server">Tipologia</asp:Localize>:
                                </label>
                                <select name="cmbTipologia" multiple="multiple" id="cmbTipologia" class="form-control"></select>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="row">
             <div class="col-lg-6 col-md-6 col-sm-12" id="id_rapcon">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" for="cmbRapCon">
                                <asp:Localize meta:resourcekey="RapportoContabile" runat="server">Rapporto Contabile</asp:Localize>:
                            </label>
                            <select name="cmbRapCon" multiple="multiple" id="cmbRapCon" class="form-control"></select>
                        </div>
                    </div>
                </div>
             </div>
           </div>
           

            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="LblGGAttesa" for="TxbGGAttesa">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GiorniDiAttesa %>" runat="server">GG di Attesa</asp:Localize>
                        </label>
                        <input id="TxbGGAttesa" type="text" aria-describedby="LblGGAttesa" class="form-control" />
                        <label class="custom_val error" style="top:29px;">
                            <asp:Localize meta:resourcekey="MessaggioGiorniDiAttesa" runat="server">Per ricevere la mail 7 GG prima della scadenza scrivere "-7". Per riceverla 30 GG dopo scrivere "30".</asp:Localize>
                        </label>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="LblMailMittente" for="TxbMailMittente">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MailMittente %>" runat="server">Mail: Mittente</asp:Localize>
                        </label>
                        <input id="TxbMailMittente" type="text" aria-describedby="LblMailMittente" class="form-control" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12" id="id_MailA">
                    <div class="input-group">
                        <label class="input-group-addon" id="LblMailA" for="TxbMailA">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MailDestinatari %>" runat="server">Mail: A</asp:Localize>
                        </label>
                        <input id="TxbMailA" type="text" aria-describedby="LblMailA" class="form-control" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="input-group">
                        <label class="input-group-addon" id="LblMailCC" for="TxbMailCC">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MailCopiaConoscenza %>" runat="server">Mail: CC</asp:Localize>
                        </label>
                        <input id="TxbMailCC" type="text" aria-describedby="LblMailCC" class="form-control" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-9 col-md-9 col-sm-12">
                    <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SeparareGliIndirizziEmailConVirgole %>" runat="server">NB: separare gli indirizzi email con delle virgole.</asp:Localize></b>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-12">
                    <div class="btn btn-success xi-btn-primary btn-block" id="BtnAvviso_Edit" style="display:none;" onclick="modificaAvviso();">
                        <span class="fa fa-save"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Modifica %>" runat="server">Modifica</asp:Localize></span>
                    </div>
                    <div class="btn btn-success xi-btn-primary btn-block buttonClass" id="BtnAvviso_Add" onclick="aggiungiAvviso();">
                        <span class="fa fa-save"> </span> <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AggiungiComeNuovo %>" runat="server">Aggiungi come nuovo</asp:Localize>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!--GRIGLIA CON GLI AVVISI-->
    <input type="hidden" id="hdKendo_Valorizzazione"/>
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 125px;">
        <div id="tabella_avvisi"></div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Impostazioni_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Impostazioni.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Impostazioni_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Impostazioni_ws_client.js") %>"></script>

</asp:Content>
