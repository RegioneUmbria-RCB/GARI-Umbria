<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GestioneEsercizi.aspx.vb" Inherits="AgroAgenda_2010.GestioneEsercizi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .k-grid-toolbar { display: block }
        .k-grid-search { margin-left: auto; margin-right: 0; float: right }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel-group searchArea" id="searchArea" style="padding-top:10px;">
        <div class="panel-body" style="padding:5px;padding-top:15px;">
            <div class="row">
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
					        <div class="input-group">
						        <label class="input-group-addon lbl_required" id="lblAzioneEsercizio" for="ddlAzioneEsercizio">
                                    <asp:Localize meta:resourcekey="Azione" runat="server">Azione</asp:Localize>:
                                </label>
						        <input name="ddlAzioneEsercizio" id="ddlAzioneEsercizio" class="form-control" />
					        </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-5 col-md-5 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <label class="input-group-addon lbl_required" id="lblDataChiusura" for="txtDataChiusura">
                                    <i class="fa fa-calendar"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataChiusura %>" runat="server">Data Chiusura</asp:Localize>
                                </label>
                                <input type="text" name="txtDataChiusura" id="txtDataChiusura" class="form-control kendoDate" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="btn btn-success xi-btn-primary xi-form-action" id="btn_esegui">
                        <span class="fa fa-check"></span>Esegui
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <input type="checkbox" id="checkApertura" class="k-checkbox">
                    <label class="k-checkbox-label" for="checkApertura"><asp:Localize meta:resourcekey="AperturaAutomaticaFinestra" runat="server">Apertura automatica della finestra</asp:Localize></label>
                    <input type="checkbox" id="checkPoliennali" class="k-checkbox" checked="checked">
                    <label class="k-checkbox-label" for="checkPoliennali"><asp:Localize meta:resourcekey="SoloEserciziPoliennali" runat="server">Solo esercizi poliennali</asp:Localize></label>
                    <input type="checkbox" id="checkArboree" class="k-checkbox" checked="checked">
                    <label class="k-checkbox-label" for="checkArboree"><asp:Localize meta:resourcekey="SoloArboree" runat="server">Solo arboree</asp:Localize></label>      
                </div>
            </div>        
        </div>

        <!-- griglia gestione esercizi -->
        <div id="tab_esercizi" style="overflow: auto; margin-top: 10px; margin-bottom: 10px; width:100%;"></div>

    </div> 

    <input type="hidden" id="hdPiva" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneEsercizi.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneEsercizi_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneEsercizi_ws_client.js") %>" ></script>
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
    </script>

</asp:Content>
