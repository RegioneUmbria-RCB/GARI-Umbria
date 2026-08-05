<%@ Page Title="Crea Anagrafica Prodotto" Language="vb" AutoEventWireup="false" CodeBehind="CreaProdottoFast.aspx.vb"
    Inherits="AgroAgenda_2010.CreaProdottoFast" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

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
    <div class="jumbotron">
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" for="multiselFornitori">Tipologia Semente:</label>
                            <select name="Cmb_TipSem"  id="Cmb_TipSem" class="form-control" data-placeholder=""></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" for="multiselFornitori">Specie:</label>
                            <select name="Cmb_Specie"  id="Cmb_Specie" class="form-control" data-placeholder=""></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" for="multiselFornitori">Varieta:</label>
                            <select name="Cmb_Varieta"  id="Cmb_Varieta" class="form-control" data-placeholder=""></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" for="multiselFornitori">Tipologia varietale:</label>
                            <select name="Cmb_GruppoVar"  id="Cmb_gruppoVar" class="form-control" data-placeholder=""></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" for="multiselFornitori">Regolamento:</label>
                            <select name="Cmb_Regolamento"  id="id_cmbregolamento" class="form-control" data-placeholder=""></select>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="hdElem_Cod" runat="server" />
    <input type="hidden" id="hdPiva" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
      var objP_server = '<%=objparametri_server_string %>';
        var objP_utenti = '<%=objparametri_utenti_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>"; 
        var obj_Prodotto = <%= objProdotto.ToString  %>;
    </script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("CreaProdottoFast_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("CreaProdottoFast.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("CreaProdottoFast_jQueryDocReady.js") %>"></script>

    <%--    <script id="templateBtnEsportaCSV" type="text/x-kendo-template">
        <div class="btn btn-success" id="btn_PulsanteEsportaCSV" style="margin-right: 3px;" onclick="Esporta_CSV()">
            <span class="lampeggiante">Esporta conferimenti</span>
        </div>
    </script>--%>
</asp:Content>
