<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GestoreCache.aspx.vb" Inherits="AgroAgenda_2010.GestoreCache" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">

        *, :before, :after
        {
            -webkit-box-sizing: content-box;
            -moz-box-sizing: content-box;
            box-sizing: content-box;
        }

        /* set a border-box model only to elements that need it */
        .form-control, /* if this class is applied to a Kendo UI widget, its layout may change */
        .container,
        .container-fluid,
        .row,
        .col-xs-1, .col-sm-1, .col-md-1, .col-lg-1,
        .col-xs-2, .col-sm-2, .col-md-2, .col-lg-2,
        .col-xs-3, .col-sm-3, .col-md-3, .col-lg-3,
        .col-xs-4, .col-sm-4, .col-md-4, .col-lg-4,
        .col-xs-5, .col-sm-5, .col-md-5, .col-lg-5,
        .col-xs-6, .col-sm-6, .col-md-6, .col-lg-6,
        .col-xs-7, .col-sm-7, .col-md-7, .col-lg-7,
        .col-xs-8, .col-sm-8, .col-md-8, .col-lg-8,
        .col-xs-9, .col-sm-9, .col-md-9, .col-lg-9,
        .col-xs-10, .col-sm-10, .col-md-10, .col-lg-10,
        .col-xs-11, .col-sm-11, .col-md-11, .col-lg-11,
        .col-xs-12, .col-sm-12, .col-md-12, .col-lg-12
        {
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }
        
        .window-content {
            overflow: auto;
            height: calc(100% - 90px);
            padding: 10px
          }

        .window-footer {
            position: absolute;
            bottom: 0;
            display: block;
            width: 95%;
            margin-top: 150px;
            padding: 19px 0 20px;
            text-align: right;
            border-top: 1px solid #e5e5e5;
        }

        div.k-grid tbody .k-button {
           min-width: 45px;
           padding: 5px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
        

            <div class="panel-group">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-3 col-md-12 col-sm-12">
                            <div class="form-group">
                                <label id="lbl_chkCacheEnabled" for="chkCacheEnabled">Abilita Cache Data Provider</label>     
                                <input type="checkbox" id="chkCacheEnabled" name="chkCacheEnabled" class="kendoSwitch" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-2 col-sm-2 col-xs-12">
                            <div class="btn btn-success" id="btn_pulisci_cache">
                                <i class="fa fa-eraser"></i> Pulisci Cache
                            </div>
                        </div>
                        <div class="col-lg-2 col-sm-2 col-xs-12">
                            <div class="btn btn-success" id="btn_pulisci_cache_permessi">
                                <i class="fa fa-eraser"></i> Pulisci Cache Permessi
                            </div>
                        </div>
                        <div class="col-lg-2 col-sm-2 col-xs-12">
                            <div class="btn btn-success" id="btn_pulisci_cache_impostazioni">
                                <i class="fa fa-eraser"></i> Pulisci Cache Impostazioni
                            </div>
                        </div>
                        <div class="col-lg-2 col-sm-2 col-xs-12">
                            <div class="btn btn-success" id="btn_aggiorna">
                                <i class="fa fa-refresh"></i> Aggiorna
                            </div>
                        </div>
                    </div>
                </div>
            </div>


    <!-- griglia MVV -->
    <div class="panel-group">
        <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
            <div id="gridCache"></div>
        </div>
    </div>

    <input type="hidden" id="hf_cacheAbilitata" runat="server" />

    <div id="dialogErrorKendo"></div>
    <div id="dialogOkKendo"></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cacheAbilitata = "#<%=hf_cacheAbilitata.ClientID %>";
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestoreCache_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestoreCache.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestoreCache_jQueryDocReady.js") %>"></script>

</asp:Content>
