<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RichiestaDocumenti.ascx.vb" Inherits="AgronicaUMA.RichiestaDocumenti" %>
<div class="row">
    <div class="" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_griglia_documenti">

            <div id="grdRichiestaDocumenti"></div>

            <!-- hidden fields -->
            <input type="hidden" id="hfKendo_risultatiLettura" runat="server" />
            <input type="hidden" id="hfPaginaRedirect" runat="server" />
            <input type="hidden" id="hfPaginaRedirect_Codificata" runat="server" />
            <input type="hidden" id="hf_UtenteAbilitatoLettura" runat="server" />
            <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />

        </div>
    </div>
</div>

<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaDocumenti.js")) %>"></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/CarburantiUMA/RichiestaDocumenti_ws_client.js")) %>"></script>