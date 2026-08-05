<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DomandaIrrigua.aspx.vb" 
    Inherits="AgronicaDomandaIrrigua.DomandaIrrigua" MasterPageFile="~/Master/DomandaIrriguaBootstrap.Master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="container" style="margin-bottom: 70px">
        <div class="row">
            <div class="col-lg-2 col-md-2 col-sm-2">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group" id="groupAnno">
                            <label class="input-group-addon" for="txtAnno" id="lblAnno">Anno: </label>
                            <input type="text" name="txtAnno" id="txtAnno" class="form-control" readonly/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-2 col-sm-2">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group" id="groupID">
                            <label class="input-group-addon" for="txtID" id="lblID">Seq. domanda: </label>
                            <input type="text" name="txtID" id="txtID" class="form-control" readonly/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-8 col-md-8 col-sm-8">
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
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div>
                    <div class="form-group">
                        <div class="k-card" style="margin: 8px" id ="dati_anagrafici">
                            <label class="input-group-addon" style="width:100%">Dati anagrafici</label>
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
                                                <label class="input-group-addon" for="txtPIVA" id="lblPIVA">Partita IVA</label>
                                                <input type="text" name="txtPIVA" id="txtPIVA" class="form-control" readonly/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-2 col-md-2 col-sm-2">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group" id="groupCFCUAA">
                                                <label class="input-group-addon" for="txtCFCUAA" id="lblCFCUAA">CF/CUAA</label>
                                                <input type="text" name="txtCFCUAA" id="txtCFCUAA" class="form-control" readonly/>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="k-card" style="margin: 8px" id ="indirizzo">
                                    <label class="input-group-addon" style="text-align:left;width:100%;">Indirizzo</label>
                                    <div class="row"> 
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupVia">
                                                        <label class="input-group-addon " for="txtVia" id="lbl_Via">Via</label>
                                                        <input type="text" name="txtVia" id="txtVia" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupFrazione">
                                                        <label class="input-group-addon " for="txtFrazione" id="lbl_Frazione">Frazione</label>
                                                        <input type="text" name="txtFrazione" id="txtFrazione" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupProvincia">
                                                        <label class="input-group-addon " for="txtProvincia" id="lbl_Provincia">Provincia</label>
                                                        <input type="text" name="txtProvincia" id="txtProvincia" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row"> 
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupComune">
                                                        <label class="input-group-addon " for="txtComune" id="lbl_Comune">Comune</label>
                                                        <input type="text" name="txtComune" id="txtComune" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupCAP">
                                                        <label class="input-group-addon " for="txtCAP" id="lbl_CAP">CAP</label>
                                                        <input type="text" name="txtCAP" id="txtCAP" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupStato">
                                                        <label class="input-group-addon " for="txtStato" id="lbl_Stato">Stato</label>
                                                        <input type="text" name="txtStato" id="txtStato" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="k-card" style="margin: 8px" id ="contatti">
                                    <label class="input-group-addon" style="text-align:left;width:100%;">Contatti</label>
                                    <div class="row"> 
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupTelefono">
                                                        <label class="input-group-addon " for="txtTelefono" id="lbl_Telefono">Telefono</label>
                                                        <input type="text" name="txtTelefono" id="txtTelefono" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <%--<div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupPEC">
                                                        <label class="input-group-addon " for="txtPEC" id="lbl_PEC">PEC</label>
                                                        <input type="text" name="txtPEC" id="txtPEC" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupSDI">
                                                        <label class="input-group-addon " for="txtSDI" id="lbl_SDI">Codice Unico\SDI</label>
                                                        <input type="text" name="txtSDI" id="txtSDI" class="form-control" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>--%>
                                    </div>
                                </div>
                                <div class="k-card" style="margin: 8px" id ="comparto_irriguo">
                                    <label class="input-group-addon" style="text-align:left;width:100%;">Comparto irriguo</label>
                                    <div class="row"> 
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupImpianto">
                                                        <label class="input-group-addon " for="ddlImpianto" id="lbl_Impianto">Impianto</label>
                                                        <input name="ddlImpianto" id="ddlImpianto"  class="form-control" style="width:-webkit-fill-available" readonly/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-4 col-sm-4">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group"id="groupPEC">
                                                        <label class="input-group-addon " for="txtLago" id="lbl_Lago">Lago</label>
                                                        <input type="text" name="txtLago" id="txtLago" class="form-control" readonly/>
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

                <!--TAB DATI-->         
                <div id="tab-dati">
                    <div class="col-lg-9 col-md-9 col-sm-9">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div id="divKendoOut">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-2 col-md-2 col-sm-2">
                        <div class="form-group">
                            <div class="input-group" id="groupSupTot">
                                <label class="input-group-addon " for="txtSupTotale" id="lbl_SupTotale"">Superficie Totale (Ha): </label>
                                <input type="text" name="txtSupTotale" id="txtSupTotale" class="form-control" readonly/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="confermaUscitaDialog"></div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdIdDomanda" runat="server" />
    <input type="hidden" id="hdEnableMod" runat="server" />
    <input type="hidden" id="hdDatiDomanda" runat="server" />
    <input type="hidden" id="hdAnno" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />
</asp:content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("DomandaIrrigua.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("DomandaIrrigua_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("DomandaIrrigua_jQueryDocReady.js")) %>"></script>


    <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdDatiDomanda = "#<%=hdDatiDomanda.ClientID() %>";
        var cIdEnableMod = "#<%=hdEnableMod.ClientID() %>";
        var cIdIDDomanda = "#<%=hdIdDomanda.ClientID() %>";
        var cIdPaginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
        var cIdPaginaRedirectCodificata = "#<%=hdPaginaRedirect_Codificata.ClientID() %>";
        var cIdAnno = "#<%=hdAnno.ClientID() %>";
    </script>
</asp:Content>