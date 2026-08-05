<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GestioneRepartiPianiUC.ascx.vb" Inherits="AgroAgenda_2010.GestioneRepartiPianiUC" %>

<div class="panel-group anagArea" style="display: none;">

    <!-- Griglia reparti piani -->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_celle_reparti_piani" class="gias-table-plain"></div>
    </div>
</div>

<input type="hidden" id="hdData" runat="server" />

<!-- fine container -->
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneRepartiPianiUC_jQueryDocReady.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneRepartiPianiUC.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneRepartiPianiUC_ws_client.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneRepartiPianiUC_globali.js") %>" ></script>
<script type="text/javascript">
    var hfData = "#<%=hdData.ClientID() %>";
</script>