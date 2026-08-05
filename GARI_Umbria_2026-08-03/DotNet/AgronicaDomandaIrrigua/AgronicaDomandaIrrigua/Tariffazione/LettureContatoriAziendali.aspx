<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LettureContatoriAziendali.aspx.vb" 
    Inherits="AgronicaDomandaIrrigua.LettureContatoriAziendali"  MasterPageFile="~/Master/DomandaIrriguaBootstrap.Master"%>

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
            <div class="col-lg-6 col-md-6 col-sm-6">
                <div class="form-horizontal">
                    <div class="form-group" style="display:flex; justify-content: flex-end;">
                        <div class="input-group" id="groupButton">
                            <div class="btn btn-success submit" id="btnSearchData" onclick="Ricerca()">
                                <i class="fa fa-search-o"></i>Ricerca
                            </div>
                            <div class="btn btn-success submit" id="btnSaveData" onclick="Salva()">
                                <i class="fa fa-floppy-o"></i>Salva
                            </div>
                            <div class="btn btn-danger" id="btnExitNoSave" onclick="Esci()">
                                <i class="fa fa-reply"></i><span id="btnExitNoSaveTxt">Esci</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div>
                    <div class="form-group">
                        <div class="k-card" style="margin: 8px" id ="dati_anagrafici">
                            <div class="row"> 
                                <div class="col-lg-8 col-md-8 col-sm-8">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" id="groupRagSoc">
                                                <label class="input-group-addon" for="txtRagSoc" id="lblRagSoc">Ragione Sociale</label>
                                                <input type="text" name="txtRagSoc" id="txtRagSoc" class="form-control" readonly/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-2 col-md-2 col-sm-2">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" id="groupPiva">
                                                <label class="input-group-addon" for="txtAnno" id="lblPIVA">Anno</label>
                                                <input type="text" name="txtAnno" id="txtAnno" class="form-control"/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6">
                                    <div class="k-card" style="margin: 8px" id ="dati Letture">
                                        <label class="input-group-addon" style="text-align:left;width:100%;">Letture</label>
                                        <div class="row">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupLetture">
                                                        <div id="divKendoGridLetture"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6">
                                    <div class="k-card" style="margin: 8px" id ="rifContatore">
                                        <label class="input-group-addon" style="text-align:left;width:100%;">Dati anagrafici contatori</label>
                                        <div class="row">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupContatori">
                                                        <div id="divKendoGridAnagContatori"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>                                
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="confermaUscitaDialog"></div>
    <div id="confermaEliminazioneDialog"></div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdIdContatore" runat="server" />
    <input type="hidden" id="hdAnno" runat="server" />
    <input type="hidden" id="hdDatiLetture" runat="server" />
    <input type="hidden" id="hdDatiUltimaLetturaAnnoPrecedente" runat="server" />
    <input type="hidden" id="hdDatiPrimaLetturaAnnoSuccessivo" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
</asp:content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("LettureContatoriAziendali.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("LettureContatoriAziendali_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("LettureContatoriAziendali_jQueryDocReady.js")) %>"></script>

    <script id="customToolbar" type="text/x-kendo-template">
        <div class="k-button k-grid-pulsantegenerico" id="btnNuovaLettura" onclick="AggiungiNuovaLettura();">Nuova Lettura</div>
	</script>

    <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdIdContatore = "#<%=hdIdContatore.ClientID() %>";
        var cIdAnno = "#<%=hdAnno.ClientID() %>";
        var cIdDatiLetture = "#<%=hdDatiLetture.ClientID() %>";
        var cIdUtenteAbilitatoScrittura = "#<%=hf_UtenteAbilitatoScrittura.ClientID() %>";
        var cIdDatiUltimaLetturaAnnoPrecedente = "#<%=hdDatiUltimaLetturaAnnoPrecedente.ClientID() %>";
        var cIdDatiPrimaLetturaAnnoSuccessivo = "#<%=hdDatiPrimaLetturaAnnoSuccessivo.ClientID() %>";
    </script>
</asp:Content>