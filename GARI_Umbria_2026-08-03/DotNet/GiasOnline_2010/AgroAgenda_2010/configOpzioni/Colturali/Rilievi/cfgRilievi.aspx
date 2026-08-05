<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="cfgRilievi.aspx.vb" Inherits="AgroAgenda_2010.cfgRilievi" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content" style="margin-bottom: 70px">

        <div class="row">
            <div class="col-md-12 tabs" id="tabs_kendo_Configurazioni">
                <div role="tabpanel">
                    <ul class="nav nav-tabs" role="tablist">

                        <li id="misuraAvversita" role="presentation" class="active tabMisuraAvversita">
                            <a href="#tabs-misuraAvversita" aria-controls="home" role="tab" data-toggle="tab" aria-expanded="true">
                                <span id="lblMisuraAvversita">Rilievi Avversità
                                </span>
                            </a>
                        </li>

                        <li id="fasiFenologiche" role="presentation" class="tabFasiFenologiche">
                            <a href="#tabs-fasiFenologiche" aria-controls="home" role="tab" data-toggle="tab" aria-expanded="false">
                                <span id="lblFasiFenologiche">Fasi Fenologiche
                                </span>
                            </a>
                        </li>
                    </ul>

                    <!-- Tab panes -->
                    <div class="tab-content">
                        
                        <div role="tabpanel" class="tab-pane active" id="tabs-misuraAvversita">

                            <input type="hidden" id="hdCfgRilievi" />
                            <input type="hidden" id="hdcfgRilieviAnag" />
                            <input type="hidden" id="hdCurrentCOD" />
                            <input type="hidden" id="hdAnagrafiche" />

                            <div id="divCfgRilievi"></div>
                            
                            <!-- dialog modale -->
                            <div class="modal fade" id="mTabellaDettagli">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                            <h4 class="modal-title">
                                                <asp:Label ID="lblmTabellaDettagli" runat="server" Text="Valori per selezione"></asp:Label></h4>
                                        </div>
                                        <div class="modal-body">
                                            <div id="divcfgRilieviAnag"></div>
                                        </div>
                                        <div class="modal-footer">                                            
                                        </div>
                                    </div>
                                    <!-- /.modal-content -->
                                </div>
                                <!-- /.modal-dialog -->
                            </div>
                            <!-- /.modal -->



                        </div>
                        
                        
                        <div role="tabpanel" class="tab-pane active" id="tabs-fasiFenologiche">

                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript">
        var id_lblmTabellaDettagli = "#<%=lblmTabellaDettagli.ClientID %>";
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("cfgRilievi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("cfgRilievi_KendoTemplate.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("cfgRilievi_Kendo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("cfgRilievi_JQueryDocReady.js") %>"></script>
</asp:Content>
