<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ConfigurazioneImballiProdotto.aspx.vb"
    Inherits="AgroAgenda_2010.ConfigurazioneImballiProdotto" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%--<%@ Register TagPrefix="uc" TagName="GestioneLavorazioniMenuUC" Src="../GestioneLavorazioni/LavorazioneMenuUC.ascx" %>--%>
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
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <%--<uc:GestioneLavorazioniMenuUC id="GestioneLavorazioniMenuUC" runat="server" />--%>

    <%--<div class="panel-group searchArea" style="display: none;">
        <div class="panel-body" style="padding-top: 30px;">

            <div class="row">
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon lbl_required" id="lbl_DataRif" for="txt_DataRif">Data riferimento:</span>
                                <input ID="txt_DataRif" name="txt_DataRif" class="kendoCalendar" style="width: 100%;" MaxLength="10" />
                             </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon lbl_required" id="lbl_des_TestataGriglia" for="txt_des_TestataGriglia">Descrizione:</span>
                                <asp:TextBox ID="txt_des_TestataGriglia" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-2 col-md-2 col-sm-12">
                    <div class="btn btn-success" id="btn_ricerca">
                        <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Ricerca</span>
                    </div>
                </div>
            </div>
        </div>
    </div>--%>
        
    <div class="panel-group modifyArea" style="display:none;">
        <!--Griglia-->
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="tab_griglia_configurazioneimballiprodotto"></div>
        </div>
    </div>
    <!-- fine container -->
 

     
    <input type="hidden" id="hdPiva" runat="server" />


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneImballiProdotto_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneImballiProdotto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneImballiProdotto_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("configurazioneImballiProdotto_ws_client.js") %>"></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
    </script>
</asp:Content>
