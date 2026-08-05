<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="PassaggioDiStato.aspx.vb" Inherits="AgroAgenda_2010.PassaggioDiStato" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #lbl_StatoAttuale {
            margin: 5px 0 10px 0;
        }

        #warningNessunPassaggioDiStato {
            color: red;
            display: none;
            margin: 15px 0 0 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="kendoDialogPassaggioStato">
        <div class="">
            <input type="hidden" id="HDPassaggioStato_Pratica_Cod" />
            <input type="hidden" id="HDPassaggioStato_PassaggioDiStato" />
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <h4 id="lbl_Servizio"></h4>
                </div>
            </div>
            <div class="row" id="rowProcedura" style="display:none">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="input-group">
                            <span class="input-group-addon alert-info">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Procedura %>" runat="server">Procedura</asp:Localize>:
                            </span>
                            <input type="text" id="CmbProcedura" name="Data" class="form-control" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <h5 id="lbl_StatoAttuale"></h5>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StatoDestinazione %>" runat="server">Stato di Destinazione</asp:Localize>:
                                </span>
                                <input type="text" id="Cmb_Stato_Destinazione" name="Data" class="form-control" />
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="row" style="display: none;">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRiferimento %>" runat="server">Data Riferimento</asp:Localize>
                                </span>
                                <input type="text" id="Txt_Data_Riferimento" name="Data" class="form-control" />
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
                                <span class="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server">Note</asp:Localize>
                                </span>
                                <textarea id="Txt_Note" class="form-control"></textarea>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="divDSS">
                <br/>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <h4>DSS</h4>
                        </div>
                    </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PacchettiModelliDSSAcquistati %>" runat="server">Pacchetti modelli DSS acquistati</asp:Localize>
                                    </span>
                                    <input type="text" id="DSS_cmb_pacchettiAcquistati" class="form-control" />
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
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataScadenza %>" runat="server">Data Scadenza</asp:Localize>
                                    </span>
                                    <input type="text" id="Txt_Data_Scadenza" name="Data" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                        <div id="btn_AggiungiDSS" class="btn btn-info">
                            <i class="fa fa-arrow-down" aria-hidden="true"></i>
                            <span id="lbl_CercaPratiche">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AggiungiPacchettoSelezionato %>" runat="server">Aggiungi pacchetto selezionato</asp:Localize>
                            </span>
                            <i class="fa fa-arrow-down" aria-hidden="true"></i>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                        <div id="kendoDSS"></div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 text-center">
                    <div id="btnConferma" class="btn btn-info btn_100">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Conferma %>" runat="server"></asp:Localize>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12" id="warningNessunPassaggioDiStato">
                    <span>
                        <label class="myLabelBold">
                            <i class="fa fa-info-circle"></i>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, UlterioriAvanzamentiDiStatoNonDisponibili %>" runat="server"></asp:Localize>
                        </label>
                    </span>
                </div>
            </div>
        </div>
        <div id="dialogAvanz">
        </div>
    </div>
    <!-- Hidden Controls -->
    <input type="hidden" id="hdPraticaCod" runat="server" />
    <input type="hidden" id="hdPassaggioDiStatoCod" runat="server" />
    <input type="hidden" id="hdUsername" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="PassaggioDiStato.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="PassaggioDiStato_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="PassaggioDiStato_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>

    <script type="text/javascript">
        var cIdPraticaCod = "#<%= hdPraticaCod.ClientID() %>";
        var cIdPassaggioDiStatoCod = "#<%= hdPassaggioDiStatoCod.ClientID() %>";
        var cIdUsername = "#<%= hdUsername.ClientID() %>";
        var pratica_cod = $(cIdPraticaCod).val();
        var passaggiodistato_cod = $(cIdPassaggioDiStatoCod).val();
        var id_HD_Username = $(cIdUsername).val();
    </script>

</asp:Content>
