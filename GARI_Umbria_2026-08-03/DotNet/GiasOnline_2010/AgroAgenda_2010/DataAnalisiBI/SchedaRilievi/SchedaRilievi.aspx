<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="SchedaRilievi.aspx.vb" Inherits="AgroAgenda_2010.SchedaRilievi" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc2" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%--<link rel="stylesheet" href="<%= ResolveClientUrl("~/Styles/visual.css") %>" type="text/css" />--%>
    <link rel="stylesheet" href="SchedaRilievi.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div class="container" style="margin-bottom: 70px">
        <div id="Ricerca">
            <div id="frmDati" class="form-horizontal">
                <div class="row">
                    <div id="frmTestata" class="form-group">
                        <div class="row">

                            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                <div class="input-group" id="divDataDa">
                                    <label class="input-group-addon control-label alert-info " id="lbl_txt_DataDa" for="txt_DataDa">
                                        Dal:
                                    </label>
                                    <input class="form-control" id="txt_DataDa" aria-describedby="txt_DataDa" type="text" />
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                <div class="input-group" id="divDataA">
                                    <label class="input-group-addon control-label alert-info " id="lbl_txt_DataA" for="txt_DataA">
                                        Al:
                                    </label>
                                    <input class="form-control" id="txt_DataA" aria-describedby="txt_DataA" type="text" />
                                </div>
                            </div>
                            
                            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12 form-check">
                                <div class="input-group" id="divSelezione">
                                    <label class="input-group-addon form-check-label alert-info " id="lbl_chkSelezione" for="lbl_chkSelezione">
                                        Mostra solo rilevati:
                                    </label>
                                    <input class="form-check-input" id="chkSelezione" aria-describedby="chkSelezione" checked="checked" type="checkbox"  />
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                <div class="btn btn-success" id="btn_ricerca">
                                    <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Ricerca</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div id="esitoRicerca">
            <input type="hidden" id="hidTabellaEsitoRicerca" />
            <div id="tabellaEsitoRicerca">
            </div>

        </div>


    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var lhideTabSelection = <%=hideTabSelection.tostring.tolower %>;
    </script>
    <cc2:jsonStat ID="jsonStat" runat="server" />
    <cc2:visual ID="visual1" runat="server" />
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilieviKendo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilievi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilieviPivot.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilievi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilievi_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("SchedaRilievi_JsonStat.js") %>"></script>
</asp:Content>

