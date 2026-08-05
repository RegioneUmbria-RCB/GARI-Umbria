<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="ReportPercorsi.aspx.vb" Inherits="AgroAgenda_2010.ReportPercorsi" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc2" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveClientUrl("~/Styles/visual.css") %>" type="text/css" />
    <link rel="stylesheet" href="ReportPercorsi.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- contenitore principale -->
    <div>
        <div class="container" style="margin-bottom: 70px">
            
            
            <%--report delle raccolte--%>
            <div class="row">
                <!-- nuova tool -->
                <div class="col-lg-12">
                    <div id="frmDati" class="form-horizontal">
                        <div class="row">
                            <div id="frmTestata" class="form-group">
                                <div class="row">
                                    <div class="col-lg-3">
                                        <div class="input-group" id="Data_DA">
                                            <label class="input-group-addon control-label " id="lbl_Data_DA" for="Txt_Data_DA">
                                                Dal:
                                            </label>
                                            <input class="form-control" id="Txt_Data_DA" aria-describedby="lbl_Data_DA" type="text" css="kendoCalendar" />
                                        </div>
                                    </div>
                                    <div class="col-lg-3">
                                        <div class="input-group" id="Data_A">
                                            <label class="input-group-addon control-label " id="lbl_Data_A" for="Txt_Data_A">
                                                Al:</label>
                                            <input class="form-control" id="Txt_Data_A" aria-describedby="lbl_Data_A" type="text" />
                                        </div>
                                    </div>
                                    <div class="col-lg-2" style="display:none">
                                        <div class="btn btn-success" id="btn_ricerca">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Report Percorsi</span>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="btn btn-success" id="btn_posizioniRilevate">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Posizioni Rilevate</span>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="btn btn-success" id="btn_PosizioneAttuale">
                                            <span class="fa fa-map-marker lampeggiante"></span><span class="lampeggiante">Ultima Posizione</span>
                                        </div>
                                    </div>
                                    
                                    <%If (MostraReportRaccolteGIS) Then%>
                                    
                                    <div class="col-lg-2">
                                        <div class="btn btn-success" id="btn_raccolte">
                                            <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Report Raccolte</span>
                                        </div>
                                    </div>
                                    <%end If %>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div id="exportXls" class="btn btn-info export_excel" style="display: block; margin-top: 10px;
                                margin-bottom: 10px;">
                                <i class="fa fa-file-excel-o"></i>Export su Excel
                            </div>
                            <div class="tab_dati">
                                <div id="tabellaPercorsi">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <%--report gpf--%>

            <input type="hidden" id="hdkendoGPF" />


            <div class="row">
                <div id="divkendoGPF"></div>                
            </div>

            <div class="row">
                <div id="divkendoPosizioniRilevate"></div>                
            </div>

            <div class="row">
                <div id="divkendoGPF_POS"></div>                
            </div>

             <div class="row">
                <div id="divKendoDettaglioGPF"></div>
                <input type="hidden" id="hdKendoDettaglioGPF" />
            </div>

        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        
    </script>

    <script id="templateBtn_ApriGIS" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_ApriGIS">
            <span class="lampeggiante">Apri su GIS</span>
        </div>
    </script>

    
    <script id="templateBtn_ApriDettaglio" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_ApriDettaglio" style="display: none">
            <span class="lampeggiante">Mostra Dettagli Percorsi Selezionati</span>
        </div>
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportPercorsi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportPercorsi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ReportPercorsi_ws_client.js") %>"></script>
</asp:Content>
