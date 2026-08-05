<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.master" 
    CodeBehind="Scad_Anagrafiche.aspx.vb" Inherits="AgroAgenda_2010.Scad_Anagrafiche" %>

<%--<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_Scad.Master" %>--%>

<%@ Register TagPrefix="uc1" TagName="CategorieTipologieUC" Src="~/Scadenzario/UserControl/Scad_CategorieTipologie_UC.ascx" %>
<%@ Register TagPrefix="uc2" TagName="IndiciRicercaUC" Src="~/Scadenzario/UserControl/Ricerca_Indici_Documentale_UC.ascx" %>
<%@ Register TagPrefix="uc3" TagName="IndiciXTipologieUC" Src="~/Scadenzario/UserControl/IndiciXTipologie_UC.ascx" %>
<%@ Register TagPrefix="uc4" TagName="CategTipolDocumentoXUtentiUC" Src="~/Scadenzario/UserControl/CategTipolDocumentoXUtenti_UC.ascx" %>
<%@ Register TagPrefix="uc5" TagName="SchemaDocumentiUC" Src="~/Scadenzario/UserControl/SchemaDocumenti_UC.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    } 
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" /> 
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <%--<input type="hidden" id="hdKendo_risultatiLettura" runat="server" />--%>
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdTabDefault" runat="server" />
   
    <!-- Anna 23/07/21: Aggiunta button per spostare documenti su DB -->
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoExportSuDB" />

    <!-- Anna 17/05/22: aggiunto limite alla cancellazione di legame tra Tipologie x Indici riservati (solo SuperUser) -->
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoModificaTipologiexIndiciProtetti" />
    

    <div id="panelArea" class="panel-group searchArea" style="opacity: 0;">
         <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                    
                    <ul class="nav nav-tabs" role="tablist" id="tabs">
                        <li class="active"><a href="#tabCategorieTipologie" data-toggle="tab" id="a_tabCategorieTipologie">
                            <asp:Localize meta:resourcekey="CategorieETipologie" runat="server">Categorie e Tipologie</asp:Localize>
                        </a></li>                    
                        <li><a href="#tabIndici" data-toggle="tab" id="a_tabIndici">
                            <asp:Localize meta:resourcekey="IndiciDiRicerca" runat="server">Indici di ricerca</asp:Localize>
                        </a></li>
                        <li><a href="#tabIndiciXTipologie" data-toggle="tab" id="a_tabIndiciXTipologie">
                            <asp:Localize meta:resourcekey="AssociazioniTipologieIndici" runat="server">Associaz. tipologie - indici</asp:Localize>
                        </a></li>
                        <li><a href="#tabCategTipolDocumentoXUtenti" data-toggle="tab" id="a_tabCategTipolDocumentoXUtenti">
                            <asp:Localize meta:resourcekey="AutorizzazioniPerCategoriaTipologia" runat="server">Autoriz. per categoria / tipologia</asp:Localize>
                        </a></li>
                         <li><a href="#tabSchemaDocumenti" data-toggle="tab" id="a_tabSchemaDocumenti">
                            <asp:Localize meta:resourcekey="SchemaDocumenti" runat="server">Schema Documenti</asp:Localize>
                        </a></li>
                    </ul>

                    <div class="tab-content">
                        <!-- tab Categorie e Tipologie -->
                            <div class="tab-pane fade active in" id="tabCategorieTipologie" style="overflow: auto; margin-bottom: 70px;">                        
                                <div class="tab-pane fade in active" id="CategorieTipologie" style="overflow: auto; margin-bottom: 70px;">
                                    <uc1:CategorieTipologieUC id="CategorieTipologieUC" runat="server" />
                                </div>
                            </div>
                        <!-- tab Indici -->
                            <div class="tab-pane fade in " id="tabIndici" style="overflow: auto; margin-bottom: 70px;">
                                 <div class="tab-pane fade in" id="IndiciRicerca" style="overflow: auto; margin-bottom: 70px;">
                                    <uc2:IndiciRicercaUC id="IndiciRicercaUC" runat="server" />
                                </div>
                            </div>
                        <!-- tab associazione Indici a Tipologie -->
                            <div class="tab-pane fade in " id="tabIndiciXTipologie" style="overflow: auto; margin-bottom: 70px;">
                                <div class="tab-pane fade in" id="IndiciRicercaXTipologie" style="overflow: auto; margin-bottom: 70px;">
                                    <uc3:IndiciXTipologieUC id="IndiciXTipologieUC1" runat="server" />
                                </div>
                            </div>
                        <!-- tab associazione Indici a Tipologie -->
                            <div class="tab-pane fade in " id="tabCategTipolDocumentoXUtenti" style="overflow: auto; margin-bottom: 70px;">
                                <div class="tab-pane fade in" id="CategTipolDocumentoXUtenti" style="overflow: auto; margin-bottom: 70px;">
                                    <uc4:CategTipolDocumentoXUtentiUC id="CategTipolDocumentoXUtenti_UC" runat="server" />
                                </div>
                            </div>
                        <!-- tab schema Documenti -->
                             <div class="tab-pane fade in " id="tabSchemaDocumenti" style="overflow: auto; margin-bottom: 70px;">
                                <div class="tab-pane fade in" id="SchemaDocumenti" style="overflow: auto; margin-bottom: 70px;">
                                    <uc5:SchemaDocumentiUC id="SchemaDocumentiUC" runat="server" />
                                </div>
                            </div>
                    </div>

                </div>
          </div>
        </div>                 
    </div>
    
    
    <!--dialogs varie-->
    <div id="confermaEliminazioneDialog"></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Anagrafiche_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Anagrafiche.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Scad_Anagrafiche_jQueryDocReady.js") %>"></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cTabDefault = "#<%=hdTabDefault.ClientID() %>";
        <%--var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";--%>
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
    </script>
</asp:Content>
