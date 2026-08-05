<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="PianoColturaleCatasto_grid.aspx.vb" Inherits="AgronicaStampe_2010.PianoColturaleCatasto_grid" %>

<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron" style="margin-bottom: 70px">
        <div class="row">
            <div class="form-group col-lg-3 col-md-3 col-sm-6">
                <div class="input-group">
                    <span class="input-group-addon alert-info" id="lbl_multi_report_giacenze">Report salvati</span>
                    <select id="Cmb_Report" class="form-control" style="white-space: normal;"></select>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-6">
                <div class="btn btn-warning buttonClass mt-30 xonne-btn-primary" id="btn_salvaReport" style="margin-left: 10px;">
                    <span class="fa fa-floppy-o"></span>Salva
                </div>
                <div class="btn btn-danger buttonClass mt-30" id="btn_eliminaReport" style="margin-left: 10px;">
                    <span class="fa fa-trash"></span>Elimina
                </div>
            </div>

        </div>
        <div id="ppKendoPianoColturale"></div>
        <input type="hidden" id="hdKendoPianoColturale" />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">


    <script type="text/javascript">
        var str_vuota = <%= str_vuota.ToString.ToLower() %>;
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoColturaleCatasto_grid.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoColturaleCatasto_grid_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoColturaleCatasto_grid_ws_client.js") %>"></script>
</asp:Content>
