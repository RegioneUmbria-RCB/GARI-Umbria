<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SchemaDocumenti_UC.ascx.vb" Inherits="AgroAgenda_2010.SchemaDocumenti_UC" %>

    <div id="grdSchemaDocumenti"></div>

    <!-- hidden fields -->
    <input type="hidden" id="hfKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hfPaginaRedirect" runat="server" />
    <input type="hidden" id="hfPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/SchemaDocumenti_UC.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/SchemaDocumenti_UC_jQueryDocReady.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/Scadenzario/UserControl/SchemaDocumenti_UC_ws_client.js")) %>"></script>

