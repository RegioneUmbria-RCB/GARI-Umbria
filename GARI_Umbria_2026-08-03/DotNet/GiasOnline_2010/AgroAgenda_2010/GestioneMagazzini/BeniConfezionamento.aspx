<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BeniConfezionamento.aspx.vb"
    Inherits="AgroAgenda_2010.BeniConfezionamento" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="uc1" TagName="BeniconfezionamentoUC" Src="./BeniconfezionamentoUC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />


    <div id="panelArea" class="panel-group searchArea" style="opacity: 1;">

        <div class="panel-group preArea">
            <div class="panel-body" style="padding-top: 30px;">

                <div class="row">

                    <div class="col-lg-3 col-md-3 col-sm-5">
                        <div class="form-horizontal">

                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_operazione" for="txt_operazione">Operazione:</span>
                                    <input id="txt_operazione" name="txt_operazione" style="width: 100%;" maxlength="10" />
                                </div>
                            </div>


                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_dataop" for="txt_dataop">Data Operazione:</span>
                                    <input id="txt_DataRif" name="txt_dataop" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="btn btn-success" id="btn_salva">
                        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                    </div>


                    <%-- <div class="btn btn-warning" id="btn_indietro">
                                <span class="fa fa-search lampeggiante"></span><span class="lampeggiante">Indietro</span>
                            </div>--%>
                </div>

            </div>

        </div>

        <!-- tab Beni Confezionamento -->
        <div class="tab-pane fade in active" id="tabPaneBeniConfezionamento" style="overflow: auto; margin-bottom: 70px;">
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                        <!-- ul = UNORDERED LIST -->
                        <ul class="nav nav-tabs" role="tablist" id="tabs">
                            <li class="active"><a href="#tabBeniConfezionamento" data-toggle="tab" id="a_tabBeniConfezionamento">BENI DI CONFEZIONAMENTO</a></li>
                        </ul>

                        <div class="tab-content">

                            <!-- tab Beni Confezionamento -->
                            <div class="tab-pane fade active in" id="tabBeniConfezionamento" style="overflow: auto; margin-bottom: 70px;">
                                <uc1:BeniConfezionamentoUC id="BeniConfezionamentoUC1" runat="server" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--<div class="btn btn-success" id="saveChanges">
        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
    </div>--%>

    <!--dialogs varie-->
    <div id="confermaEliminazioneDialog"></div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdIdAgenda" runat="server" />
    <input type="hidden" id="hdId_Mov_Testata" runat="server" />
    <input type="hidden" id="hdId_Mov_BC_Principale" runat="server" />
    <input type="hidden" id="hdCau_Mov_BC_Principale" runat="server" />
    <input type="hidden" id="hdModalita" runat="server" />
    <input type="hidden" id="hdDes_Lib" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
    <input type="hidden" id="hdData" runat="server" />
    <input type="hidden" id="hdLavCod" runat="server" />
    <input type="hidden" id="hdCod_Risum_Scarico" runat="server" />

    <input type="hidden" id="hdPiva_Scarico" runat="server" />
    <input type="hidden" id="hdId_Agenda_Scarico" runat="server" />
    <input type="hidden" id="hdId_Mov_Testata_Scarico" runat="server" />
    <input type="hidden" id="hdId_Mov_Scarico" runat="server" />
    <input type="hidden" id="hdLav_Cod_Scarico" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("BeniConfezionamento_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript">

        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdLavCod = parseInt($("#<%=hdLavCod.ClientID() %>").val());
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";
        var cId_Mov_Testata = "#<%=hdId_Mov_Testata.ClientID() %>";
        var cId_Mov_BC_Principale = "#<%=hdId_Mov_BC_Principale.ClientID() %>";
        var cCau_Mov_BC_Principale = "#<%=hdCau_Mov_BC_Principale.ClientID() %>";
        var cModalita = "#<%=hdModalita.ClientID() %>";

        var cDes_Lib = "#<%=hdDes_Lib.ClientID() %>";
        var cData = "#<%=hdData.ClientID() %>";
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";

        var cPiva_Scarico = "#<%=hdPiva_Scarico.ClientID() %>";
        var cId_Agenda_Scarico = "#<%=hdId_Agenda_Scarico.ClientID() %>";
        var cId_Mov_Testata_Scarico = "#<%=hdId_Mov_Testata_Scarico.ClientID() %>";
        var cId_Mov_Scarico = "#<%=hdId_Mov_Scarico.ClientID() %>";
        var cLav_Cod_Scarico = "#<%=hdLav_Cod_Scarico.ClientID() %>";
        var cCod_Risum_Scarico = "#<%=hdCod_Risum_Scarico.ClientID() %>";

    </script>

</asp:Content>
