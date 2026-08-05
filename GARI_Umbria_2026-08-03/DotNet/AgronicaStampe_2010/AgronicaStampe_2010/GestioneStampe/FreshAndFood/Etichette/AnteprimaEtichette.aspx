<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master"
    CodeBehind="AnteprimaEtichette.aspx.vb" Inherits="AgronicaStampe_2010.AnteprimaEtichette" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .padMain
        {
            padding-left: 10px;
            padding-right: 10px;
        }
        .t10b10
        {
            padding-bottom: 10px;
            padding-top: 10px;
        }
        .fl
        {
            float: left;
        }
        .Des_e_control
        {
            float: left;
            min-width: 150px;
        }
        
        .numero_etichette
        {
            width: 40px;
        }
        
        .watable tfoot
        {
            display: none;
        }
        
        .lbl
        {
            font-style: italic;
        }
        .selectpicker_layout
        {
            font-size: smaller;
        }
        .selectpicker_lingua
        {
            font-size: smaller;
        }
        .selectpicker_stampa
        {
            font-size: smaller;
        }
    </style>
    <script type="text/javascript">


        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
        });




    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script language="javascript" type="text/javascript">

        var cmbOModuli_Referenze_Config_Testata_ClientID = "<%=cmbCFGTabelle.clientID  %>";
        var txtID_Agenda_ClientID = "<%=txtID_Agenda.clientID %>";
        var txtID_Mov_Det = "<%=txtID_Mov_Det.clientID %>";
        var hLav_Cod_ClientID = "<%=hLav_Cod.clientID %>";
        var hDataToPrint_clientID = "<%=hDataToPrint.clientID %>";
        var hTipoReport_clientID = "<%=hTipoReport.clientID %>";

        var btnAnteprimaEtichetteDettaglio_ClientID = "<%=btnAnteprimaEtichetteDettaglio.clientID %>";


        var isDebug = <%=isDebugAtt %>;

    </script>

    <script src="AnteprimaEtichette_Globali.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="AnteprimaEtichette_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="AnteprimaEtichette_Ricerca.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>    
    <script src="AnteprimaEtichette.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-md-2">
                <asp:ImageButton ID="btnPassaAnteprima" runat="server" ImageUrl="~/AB_Immagini/Icone32/Stampa.ico"
                    Visible="false" />
                <asp:Button ID="btnAnteprimaEtichette" OnClientClick="getDataToPrint(1)" runat="server"
                    Text="Anteprima Etichette di pedana" />
                <asp:Button ID="btnStampaDirettaEtichette" OnClientClick="getDataToPrint(1)" runat="server"
                    Text="Stampa Tutto"  CssClass="displaynone" />
                <asp:Button ID="btnAnteprimaEtichetteImballo" OnClientClick="getDataToPrint(2)" runat="server"
                    Text="Anteprima Etichette Imballi" />
                <asp:Button ID="btnAnteprimaEtichetteConfezione" OnClientClick="getDataToPrint(2)" runat="server"
                    Text="Anteprima Etichette Confezione" />
                <asp:Button ID="btnAnteprimaEtichetteDettaglio" OnClientClick="getDataToPrint(2)" runat="server"
                    Text="Anteprima Etichette Dettaglio" CssClass="displaynone" />

                <asp:Button ID="btnPickinglist" runat="server" Text="Stampa Picking List" />

                <input type="button" id="btnStampaTutto" onclick = "StampaTutto()" value="Stampa" />

            </div>
            <div class="col-md-3">
                <div id="divCFGTabelle" class="Des_e_control">
                    <div class="padMain lbl ">
                        <asp:Label ID="lblCFGTabelle" runat="server">Configurazione Tabelle: </asp:Label>
                    </div>
                    <div class="padMain">
                        <asp:DropDownList ID="cmbCFGTabelle" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="col-md-1">
                <div class="Des_e_control" style="display: none">
                    <div class="padMain lbl ">
                        <asp:Label ID="lbllayout" runat="server">Layout Etichetta: </asp:Label>
                    </div>
                    <div class="padMain">
                        <asp:DropDownList ID="ddlayout" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="Des_e_control" style="display: none">
                    <div class="padMain lbl ">
                        <asp:Label ID="lblPrinter" runat="server">Stampante: </asp:Label>
                    </div>
                    <div class="padMain">
                        <asp:DropDownList ID="ddlPrinter" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="Des_e_control" style="display: none">
                    <div class="padMain lbl">
                        <asp:Label ID="lbl_n_etich" runat="server">Lingua: </asp:Label>
                    </div>
                    <div>
                        <asp:DropDownList ID="ddlingua" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
        <div style="height: 30px">
            &nbsp;</div>
        <div class="row">
            <div class="col-md-12">
                <div id="htab_lotti lbl">
                    <asp:Label ID="lTabella" runat="server">Seleziona cosa stampare:</asp:Label></div>
                <div id="tab_lotti" class="t10b10" style="margin-bottom: 125px">
                </div>
            </div>
        </div>
    </div>
    <div>
    </div>

    <!-- frame per anteprima di stampa -->
    <div class="modal fade" id="divAnteprimaStampa">
        <div class="modal-dialog" style="width: 98%; height: 98%">
            <div class="modal-content" style="height: 95%; border-radius: 0">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        <span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h5 class="modal-title">
                        <div style="font-weight: bold" id="lblAnteprimaStampaTitle">
                        </div>
                    </h5>
                </div>
                <div class="modal-body" style="height: 80%; border-radius: 0">
                    <iframe src="" id="iframedivAnteprimaStampa" style="width: 100%; height: 100%; display: block;
                        border: 0" frameborder="0"></iframe>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">
                        Chiudi</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>

    <input type="hidden" runat="server" id="txtID_Agenda" />
    <input type="hidden" runat="server" id="txtID_Mov_det" />
    <input type="hidden" runat="server" id="hDataToPrint" />
    <input type="hidden" runat="server" id="hLav_Cod" />
    <input type="hidden" runat="server" id="hTipoReport" />
</asp:Content>
