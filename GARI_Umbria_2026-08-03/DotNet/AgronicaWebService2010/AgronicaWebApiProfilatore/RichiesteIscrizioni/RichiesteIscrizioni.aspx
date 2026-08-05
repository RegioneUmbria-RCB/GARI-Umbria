<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Profilatore_Bootstrap.Master" CodeBehind="RichiesteIscrizioni.aspx.vb" Inherits="AgronicaWebApiProfilatore.RichiesteIscrizioni" %>
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
                    <%--<label class="input-group-addon" style="width:100%">Registrazione Utente</label>--%>
                    <div style="text-align: center; padding: 10px" >Fornisci i tuoi dati per ottenere il tuo codice personale di accesso valido per accedere all'app Agronica GIAS</div>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" id="lbl_txtNome">Nome</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="text" name="txtNome" id="txtNome" class="form-control" />
                        </div>
 <%--                       <div class="col-lg-2 col-md-2 col-sm-12 col-xs-12">
                            <label class="input-group-addon " for="txtCapitolatoCodiceCliente">Codice Capitolato (Cliente)</label>
                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
                            <input type="text" name="txtCapitolatoCodiceCliente" id="txtCapitolatoCodiceCliente" class="form-control" />
                        </div>--%>
                    </div>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" for="txtCognome" id="lbl_txtCognome">Cognome</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="text" name="txtCognome" id="txtCognome" class="form-control" />
                        </div>
                    </div>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" for="txtRagioneSociale" id="lbl_txtRagioneSociale">RagioneSociale</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="text" name="txtRagioneSociale" id="txtRagioneSociale" class="form-control" />
                        </div>
                    </div>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" for="txtPartitaIva" id="lbl_txtPartitaIva">Partita Iva</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="text" name="txtPartitaIva" id="txtPartitaIva" class="form-control" />
                        </div>
                    </div>
                    <div class="row"> 
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <label class="input-group-addon" for="txtEmail" id="lbl_txtEmail">Email</label>
                        </div>
                        <div class="col-lg-10 col-md-10 col-sm-12">
                            <input type="mail" name="txtEmail" id="txtEmail" class="form-control" />
                        </div>
                    </div>
                     <div class="row" style="text-align: center">
                        <div class="col-lg-8 col-md-8 col-sm-12 text-left">
                            <div class="btn btn-success submit" id="btnRgistrazione" onclick="Registrazione()">
                                <i class="fa fa-floppy-o" aria-hidden="true"></i>Ottieni il tuo codice
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

   <%-- <div id="panelArea" class="panel-group searchArea">
      
       <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">

            <div class="jumbotron" style="padding-bottom: 0;">
                <div class="container" style="width: 100%;">
                    <div class="container_tabUtenti" style="padding: 0; /*margin-bottom: 70px*/">
                       
                        <div class="panel-group Indice">

                           <div class="row">                               

                                <div class="col-lg-3 col-md-3 col-sm-12">
					                <div class="input-group">
						                <label class="input-group-addon" id="lblChkFiltro" for="ChkFiltro">
                                             <asp:Localize  runat="server">Attiva Filtri</asp:Localize>                                           
                                        </label>
						                <input type = "checkbox" name="ChkFiltro" id="ChkFiltro" Class="kendoSwitch" />
					                </div>
				                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </div>
    </div>--%>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteIscrizioni_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteIscrizioni_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiesteIscrizioni_ws_client.js") %>"></script>

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
