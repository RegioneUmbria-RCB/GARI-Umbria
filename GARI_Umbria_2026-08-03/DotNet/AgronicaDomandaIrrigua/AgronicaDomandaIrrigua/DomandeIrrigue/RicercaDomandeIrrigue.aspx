<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RicercaDomandeIrrigue.aspx.vb" 
    Inherits="AgronicaDomandaIrrigua.RicercaDomandeIrrigue" MasterPageFile="~/Master/DomandaIrriguaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="container" style="margin-bottom: 70px">
        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group" id="groupSearchParams">
                            
                        </div>
                    </div>
                </div>
            </div>
<%--            <div class="col-lg-6 col-md-6 col-sm-6">
                <div class="form-horizontal">
                    <div class="form-group" style="display:flex; justify-content: flex-end;">
                        <div class="input-group" id="groupButton">
                            <div class="btn btn-success submit" id="btnSaveData" onclick="SalvaTutto()">
                                <i class="fa fa-floppy-o"></i>Salva
                            </div>
                            <div class="btn btn-success submit" id="btnSaveDataExit" onclick="SalvaEsci()">
                                <i class="fa fa-floppy-o"></i>Salva ed Esci
                            </div>
                            <div class="btn btn-danger" id="btnExitNoSave" onclick="EsciSenzaSalvare()">
                                <i class="fa fa-reply"></i><span id="btnExitNoSaveTxt">Esci</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>--%>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div>
                    <div class="form-group">
                        <div class="k-card" style="margin: 8px" id ="dati_anagrafici">
                            <label class="input-group-addon" style="width:100%">Criteri ricerca</label>
                            <div class="row"> 
                                <div class="col-lg-8 col-md-8 col-sm-8">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" id="groupAziende">
                                                <label class="input-group-addon" for="cmbAziende"> Azienda</label>
                                                <select name="cmbAziende" multiple="multiple" id="cmbAziende" class="form-control"></select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-4">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" id="groupPiva">
                                                <label class="input-group-addon" for="txtDaAnno" id="lblPIVA">Anno Da\A</label>
                                                <input type="text" name="txtDaAnno" id="txtDaAnno" class="form-control"/>
                                                <input type="text" name="txtAAnno" id="txtAAnno" class="form-control"/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6">
                                    <div class="form-horizontal">
                                        <div class="form-group" style="display:flex; justify-content: flex-start;">
                                            <div class="input-group" id="groupButton">
                                                <div class="btn btn-success submit" id="btnRicerca" onclick="Ricerca()">
                                                    <i class="fa fa-floppy-o"></i>Ricerca
                                                </div>
                                                <%--<div class="btn btn-success submit" id="btnSaveDataExit" onclick="SalvaEsci()">
                                                    <i class="fa fa-floppy-o"></i>Salva ed Esci
                                                </div>
                                                <div class="btn btn-danger" id="btnExitNoSave" onclick="EsciSenzaSalvare()">
                                                    <i class="fa fa-reply"></i><span id="btnExitNoSaveTxt">Esci</span>
                                                </div>--%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--TAB DATI-->         
                <div id="tab-dati">
                    <div id="divKendoOut">
                </div>
            </div>
        </div>
    </div>
    </div>

    <div id="confermaUscitaDialog"></div>

    <input type="hidden" id="hdElencoPiva" runat="server" />
    <input type="hidden" id="hdDaAnno" runat="server" />
    <input type="hidden" id="hdAAnno" runat="server" />

    <input type="hidden" id="hdDatiElencoDomande" runat="server" />
    <input type="hidden" id="hdDatiRicerca" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
</asp:content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("RicercaDomandeIrrigue.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("RicercaDomandeIrrigue_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("RicercaDomandeIrrigue_jQueryDocReady.js")) %>"></script>


    <script type="text/javascript">
        var cIdDatiElencoDomande = "#<%=hdDatiElencoDomande.ClientID() %>";
        var cIdElencoPiva = "#<%=hdElencoPiva.ClientID()  %>";
        var cIdDaAnno = "#<%=hdDaAnno.ClientID()  %>";
        var cIdAAnno = "#<%=hdAAnno.ClientID()  %>";
        var cIdDatiRicerca = "#<%=hdDatiRicerca.ClientID() %>";

    </script>
</asp:Content>
