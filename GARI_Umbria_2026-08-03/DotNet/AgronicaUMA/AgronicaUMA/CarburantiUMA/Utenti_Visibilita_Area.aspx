<%@ Page Title="" Language="vb" AutoEventWireup="false" CodeBehind="Utenti_Visibilita_Area.aspx.vb" 
    MasterPageFile="~/Master/UmaBootstrap.Master" Inherits="AgronicaUMA.Utenti_Visibilita_Area" %>

<%@ MasterType VirtualPath="~/Master/UmaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
       
        .errorCell {
            background-color: #DC143C;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="panelArea" class="panel-group searchArea jumbotron" style="opacity: 1;">
        <div class="panel-group filterArea">
            <div class="panel-body" style="padding-top: 30px;">
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <label class="input-group-addon control-label alert-info" id="CTRL_Area" for="ddlArea">
                                        Area di Visibilita'
                                    </label>
                                    <input type="text" id="ddlArea" name="ddlArea" class="form-control" aria-describedby="CTRL_Area">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12 text-center">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <div class="btn btn-success" id="btn_salva">
                                        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                <div class="jumbotron">
                    <div class="container" style="width: 100%; padding: 0">
                        <div class="container_griglia" style="padding: 0; /*margin-bottom: 70px*/">

                                <div class="panel-group visibilità_area">
                                    <div class="row">

                                        <!--Griglia-->
                                        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                            <div id="griglia_visibilita_area"></div>
                                        </div>
                                    </div>
                                </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="HD_Username" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="Utenti_Visibilita_Area.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Utenti_Visibilita_Area_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Utenti_Visibilita_Area_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script type="text/javascript">
        var QS_Area = "<%= QS_Area %>";
        var id_HD_Username = "<%= HD_Username %>";
    </script>

</asp:Content>
