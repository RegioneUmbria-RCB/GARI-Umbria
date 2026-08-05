<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GestioneCelleUC.ascx.vb" Inherits="AgroAgenda_2010.GestioneCelleUC" %>

<div class="panel-group anagArea" style="display: none;">

    <!-- Griglia reparti piani -->
    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
        <div id="tab_celle" class="gias-table-plain"></div>
    </div>
</div>

<input type="hidden" id="hdData" runat="server" />

<!-- fine container -->
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneCelleUC_jQueryDocReady.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneCelleUC.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneCelleUC_ws_client.js") %>" ></script>
<script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("./ConferimentoCelle/GestioneCelleUC_globali.js") %>" ></script>
<script type="text/javascript">
    var hfData = "#<%=hdData.ClientID() %>";
</script>