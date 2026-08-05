<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GST_Sportello.aspx.vb" Inherits="AgroAgenda_2010.GST_Sportello" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="GST_Sportello.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id ="grid-container">
        
        <div id="GridSportelli" style="height:100%;"></div>
        
        <input type="hidden" id="hdIdSportello" runat="server" /> 
        <input type="hidden" id="hdFasiSportello" runat="server" /> 
        <input type="hidden" id="hdFasiDDL" runat="server" />
    
        <div id="EditSportello" class="hidden">
            
            <div style="display:grid; grid-template-columns:1fr auto auto; grid-gap:20px;">
                <div>
                    <div class="input-group">
                        <label class="input-group-addon" for="inDescrizione">Descrizione:</label>
                        <input name="inDescrizione" id="inDescrizione" class="form-control k-textbox" />
                    </div>
                </div>
                <div>
                    <div class="input-group">
                        <label class="input-group-addon" for="inValiditaInizio">Validità inizio:</label>
                        <input name="inValiditaInizio" id="inValiditaInizio" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                    </div>
                </div>
                <div>
                    <div class="input-group">
                        <label class="input-group-addon" for="inValiditaFine">Validità fine:</label>
                        <input name="inValiditaFine" id="inValiditaFine" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                    </div>
                </div>
            </div>
            
            <div>
                <div class="input-group">
                    <label class="input-group-addon" for="inSpecie">Specie vegetali associate:</label>
                    <input name="inSpecie" id="inSpecie" class="form-control" />
                </div>
            </div>

            <div>
                <div id="GridFasiSportello"></div>
            </div>

        </div>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <%--inclusioni--%>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_SportelloJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Sportello_ws_client.js") %>"></script>

    <script id="template_AddSportello" type="text/x-kendo-template">
        <div class="k-button k-grid-pulsantegenerico" onclick="editSportello()">Aggiungi nuovo sportello</div>
	</script>

    <script type="text/javascript">
        var cIdSportello = "#<%=hdIdSportello.ClientID() %>";
        var cFasiDDL = "#<%=hdFasiDDL.ClientID() %>";
        var cFasiSportello = "#<%=hdFasiSportello.ClientID() %>";
    </script>

</asp:Content>
