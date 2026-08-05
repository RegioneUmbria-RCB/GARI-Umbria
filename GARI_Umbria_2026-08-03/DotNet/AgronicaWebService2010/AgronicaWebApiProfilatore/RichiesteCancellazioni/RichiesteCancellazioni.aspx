<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Profilatore_Bootstrap.Master" CodeBehind="RichiesteCancellazioni.aspx.vb" Inherits="AgronicaWebApiProfilatore.RichiesteCancellazioni" %>
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
     <div id="a_tab_generale">
        <div>
            <div class ="form-group col-lg-6 col-md-6 col-sm-12 col-xs-12" >
            <%--<div class="form-group">--%>
                <div class="k-card" style="margin: 8px" id ="generale">
                    <label class="input-group-addon" style="width:100%">Cancellazione Utente</label>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" id="lbl_txtEmail">Email</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="text" name="txtEmail" id="txtEmail" class="form-control" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-sm-12 text-left">
                            <div class="btn btn-success submit" id="btnCancellazione" onclick="Cancellazione()">
                                <i class="fa fa-floppy-o" aria-hidden="true"></i>Richiedi cancellazione
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
        <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteCancellazioni_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteCancellazioni_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteCancellazioni_ws_client.js") %>"></script>

    <script type="text/javascript">
        <%--var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cId_Area = "#<%=hdId_Area.ClientID() %>";
        var cId_Indice = "#<%=hdId_Indice.ClientID() %>";
        var cRiservato = "#<%=hdRiservato.ClientID() %>";--%>
       <%-- var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";--%>

    </script>
</asp:Content>
