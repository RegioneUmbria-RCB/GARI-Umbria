<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" 
    CodeBehind="Ereditatore.aspx.vb" Inherits="AgroAgenda_2010.Ereditatore" %>
    
<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- CSS -->

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- HTML -->
    
    <%--jumbotron--%>
    <div class="jumbotron" style="padding-left: 15px; padding-right: 15px;">
        <div class="row">
            <div class="row center-block">
                <div class="col-lg-6 col-xs-12">
                    <div class="center-block" style="max-width:500px;">
                        <div id="kendo_ElencoProp">
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-xs-12">
                    <div class="center-block" style="max-width:500px;">
                        <div id="filtroDistinte">
                            <div class="radio">
                              <label><input type="radio" name="optdistinte" value="0" checked/>Attive ad oggi</label>
                            </div>
                            <div class="radio">
                              <label><input type="radio" name="optdistinte" value="1"/>Tutte le distinte</label>
                            </div>
                            <div class="radio">
                              <label><input type="radio" name="optdistinte" value="2"/>Attive il </label>
                              <input id="datepicker_distinta" value="" style="width: 100%" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="clear" style="height:10px;">
            </div>
            <div class="row text-center">
                <div >
                    <div class="btn btn-success" onclick="proseguiEred();">
                    <i class="fa fa-refresh"></i>Prosegui</div>
                </div>
            </div>
            <div class="clear" style="height:10px;">
            </div>
        </div>
        <div class="row">
            <div class="col-12" style="overflow:scroll">
                <div id="kendo_EredImpianti">
                </div>
            </div>
        </div>
    </div>
    <div class="clear" style="height:10px;">
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <!-- JS -->
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Ereditatore.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Ereditatore_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Ereditatore_ws_client.js") %>" ></script>

    <script type="text/javascript">
        var UtenteAbilitatoScrittura = 
            <% If(permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impianto).Scrittura = True) Then %>
            true;
            <% else %>
            false;
            <% end if %>
    </script>

</asp:Content>
