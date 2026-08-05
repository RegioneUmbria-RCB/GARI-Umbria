<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CalcoloTariffazione.aspx.vb" 
    Inherits="AgronicaDomandaIrrigua.CalcolaTariffazione" MasterPageFile="~/Master/DomandaIrriguaBootstrap.Master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="container" style="margin-bottom: 70px">
        <div id="id_MainContainer" class="container" style="padding: 0px;">
            <div id="tabstrip">
                <ul>
                    <li id="parametri" class="k-state-active k-active">Parametri calcolo</li>
                    <li id="risultato">Risultato tariffazione
                </ul>
                <div id="tab_search">
                    <div class="responsive-container-centered">
                        <div class="responsive-content">
                            <div class="col-lg-8 col-md-8 col-sm-8">
                                <div>
                                    <div class="form-group">
                                        <div class="k-card" style="margin: 8px" id ="kparametri">
                                            <label class="input-group-addon" style="width:100%">Parametri di lancio elaborazione</label>
                                            <div class="row">
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupAnnoElaborazione">
                                                                <label class="input-group-addon" for="ddlYear" id="lblAnno">Anno di elaborazione</label>
                                                                <input type="text" name="ddlYear" id="ddlYear" class="form-control"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupImportoTotale">
                                                                <label class="input-group-addon" for="txtImportoTotale" id="lblRagSoc">Spesa complessiva</label>
                                                                <input type="text" name="txtImportoTotale" id="txtImportoTotale" class="form-control"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupImportoTotaleNote">
                                                                <i class="fa fa-info-circle">
                                                                    E' la spesa complessiva sostenuta da AFOR a fine stagione irrigua
                                                                </i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupBaseImp">
                                                                <label class="input-group-addon" for="txtBaseImp" id="lblBaseImp">Importo fisso per ettaro</label>
                                                                <input type="text" name="txtBaseImp" id="txtBaseImp" class="form-control"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupDeltaT2">
                                                                <label class="input-group-addon" for="txtDeltaImportoT2" id="lblDeltaImportoT2">Incremento fascia 2</label>
                                                                <input type="text" name="txtDeltaImportoT2" id="txtDeltaImportoT2" class="form-control"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupDeltaT2Note">
                                                                <i class="fa fa-info-circle">
                                                                    Incremento di prezzo per i consumi compresi tra 1500 e 3000 mc (da sommarsi alla tariffa base)
                                                                </i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupDeltaT3">
                                                                <label class="input-group-addon" for="txtDeltaImportoT3" id="lblDeltaImportoT3">Incremento fascia 3</label>
                                                                <input type="text" name="txtDeltaImportoT3" id="txtDeltaImportoT3" class="form-control"/>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-5 col-md-5 col-sm-5">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="groupDeltaT3Note">
                                                                <i class="fa fa-info-circle">
                                                                    Incremento di prezzo per i consumi superiori a 3000 mc (da sommarsi alla tariffa base e all'incremento inserito in fascia 2)
                                                                </i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div>
                                    <div class="form-group">
                                        <div class="k-card" style="margin: 8px" id ="kbuttons">
                                            <div class="btn btn-success" id="btn_elabora" style="width: -webkit-fill-available;margin-top: 25px;">
                                                <span class="fa fa-cogs"></span>
                                                <span>
                                                    Elabora calcolo tariffazione
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="tab_result">
                    <div id="divKendoOutput" />
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="hdDatiCalcoloTariffazione" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
</asp:content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("CalcoloTariffazione.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("CalcoloTariffazione_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("CalcoloTariffazione_jQueryDocReady.js")) %>"></script>

    <script type="text/javascript">
        var cIdDatiCalcoloTariffazione = "#<%=hdDatiCalcoloTariffazione.ClientID() %>";
        var cIdUtenteAbilitatoScrittura = "#<%=hf_UtenteAbilitatoScrittura.ClientID() %>";
    </script>
</asp:Content>
