<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="StampaPassaporto.aspx.vb" Inherits="AgronicaStampe_2010.StampaPassaporto" %>

<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_Piva" />
    <asp:HiddenField runat="server" ID="hf_Operazioni_Cod" />    
    <div class="row">
        
        <div class="col-md-2" style="display: none">
            <div class="Des_e_control" >
                <div class="padMain lbl ">
                    <asp:Label ID="lblPrinter" runat="server">Stampante: </asp:Label>
                </div>
                <div class="padMain">
                    <asp:DropDownList ID="ddlPrinter" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        
         <div class="col-md-2" style = "display:none">
             <div class='btn btn-info btnInfo' style='display:block;width:130px;border:0px;' onclick=StampaTutto()>Stampa</div>
        </div>

    </div>
    
    <div id ="kendoRegistroPassaportiStampa"></div>
    <input type="hidden" id="hdKendoRegistroPassaporti" />

    <!-- frame per anteprima di stampa -->
    <div class="modal fade" id="divAnteprimaStampaPassaporto">
        <div class="modal-dialog" style="width: 98%; height: 98%">
            <div class="modal-content" style="height: 95%; border-radius: 0">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h5 class="modal-title">
                        <div style="font-weight: bold" id="lblAnteprimaStampaTitlePassaporto">
                        </div>
                    </h5>
                </div>
                <div class="modal-body" style="height: 80%; border-radius: 0">
                    <iframe src="" id="iframedivAnteprimaStampaPassaporto" style="width: 100%; height: 100%; display: block;
                                                                                                                                                                                                                                                                                                                                                                         border: 0" frameborder="0"></iframe>
                </div>
                <div class="modal-footer">
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">

        var pivaVivaistaRiferimento = $("#<%= hf_Piva.ClientID() %>").val();
        var operazioniCod = $("#<%= hf_Operazioni_Cod.ClientID() %>").val();

    </script>
    

    <script src="StampaPassaporto.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="StampaPassaportoKendo.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="StampaPassaporto_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    

</asp:Content>
