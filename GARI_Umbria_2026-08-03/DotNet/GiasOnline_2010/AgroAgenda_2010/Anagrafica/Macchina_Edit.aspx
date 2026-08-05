<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Macchina_Edit.aspx.vb" Inherits="AgroAgenda_2010.Macchina_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #TxtVerificaPiva {
            font-size: 16px;
        }

        #refreshModificaCosti, #refreshEliminaCosti {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">
        <div class="col-lg-12 text-right">
            <%-- <div class="btn btn-success" ng-click="log();" style="margin-bottom: 20px; display:none;">
                <i class="fa fa-floppy-o"></i>log
            </div>

            <div class="btn btn-success" ng-click="salvaMacchinaSuServer();" style="margin-bottom: 20px;" >
                <i class="fa fa-floppy-o"></i>Salva
            </div>--%>
            <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

            <div class="btn btn-success" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
            </div>
            <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
            <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important;" name="btn_SalvaContinua">
                <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
            </button>
            <ul class="dropdown-menu" style="right: 0; left: auto !important;" >
                <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                </a></li>
                <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                </a></li>
            </ul>
            <% End If%>
            <asp:HiddenField ID="tipo_salva" runat="server" ClientIDMode="Static" />
            <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                Style="display: none" />
            <asp:ImageButton ID="ImgBtn_SalvaDistinta" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                Style="display: none" />

            <% End If%>
        </div>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active tab_macchina"><a href="#tab_macchina" data-toggle="tab">
                    <asp:Localize meta:resourcekey="DatiMacchina" runat="server">Dati Macchina</asp:Localize>
                </a></li>
                <li class="tab_dati_tecnici"><a href="#tab_dati_tecnici" data-toggle="tab">
                    <asp:Localize meta:resourcekey="DatiTecnici" runat="server">Dati Tecnici</asp:Localize>
                </a></li>
                <li class="tab_costi"><a href="#tab_costi" data-toggle="tab">
                    <asp:Localize meta:resourcekey="CostiEAmmortamento" runat="server">Costi e Ammortamento</asp:Localize>
                </a></li>
            </ul>
            <div id="my-tab-content" class="tab-content">
                <div class="tab-pane active" id="tab_macchina">
                    <div class="jumbotron">
                        
                        <div class="row" id="rowContatto">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Contatto" for="ddlContatto">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contatto %>" runat="server">Contatto</asp:Localize>
                                            </span>
                                            <asp:DropDownList ID="ddlContatto" ClientIDMode="Static" class="form-control" aria-describedby="lbl_Contatto" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_CentriAziendali" for="Cmb_CentriAziendali">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Visibilità %>" runat="server">Visibilità</asp:Localize> *
                                            </span>
                                            <asp:DropDownList ID="Cmb_CentriAziendali" ClientIDMode="Static" class="form-control" aria-describedby="lbl_CentriAziendali" runat="server">
                                            </asp:DropDownList>
                                            <%--                                            <select id="Cmb_CentriAziendali" ng-model="macchina.Sa_Cod" class="form-control"
                                                aria-describedby="lbl_CentriAziendali" ng-options="i.valore as i.testo  for i in CentriAziendaliVisibilita">
                                            </select>--%>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Descrizione" for="Txt_Descrizione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Descrizione %>" runat="server">Descrizione</asp:Localize>
                                            </span>
                                            <%--                                        <input id="Txt_Descrizione" class="form-control" aria-describedby="lbl_Descrizione"
                                                ng-model="macchina.Descrizione" />--%>
                                            <asp:TextBox ID="Txt_Descrizione" ClientIDMode="Static" class="form-control" aria-describedby="lbl_Descrizione" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                        <div class="row">

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon  alert-info" id="lbl_Marca" for="ddlMarca">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Marca %>" runat="server">Marca</asp:Localize>
                                            </span>
                                            <asp:DropDownList ID="ddlMarca" ClientIDMode="Static" class="form-control" runat="server">
                                            </asp:DropDownList>
                                            <%--                                        <select id="ddlMarca" ng-model="macchina.Marca" class="form-control" ng-options="i.valore as i.testo  for i in Marche">
                                            </select>--%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon  alert-info" id="lbl_Modello" for="Txt_Modello">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Modello %>" runat="server">Modello</asp:Localize> *
                                            </span>
                                            <%--         <input id="Txt_Modello" class="form-control" aria-describedby="lbl_Modello" ng-model="macchina.Modello" />--%>
                                            <asp:TextBox ID="Txt_Modello" ClientIDMode="Static" class="form-control" aria-describedby="lbl_Modello" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Finalita" for="Cmb_Finalita">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Finalità %>" runat="server">Finalità</asp:Localize> *
                                            </span>
                                            <%--                                     <select id="Cmb_Finalita" ng-model="macchina.Finalita" class="form-control required"
                                                ng-change="cambioFinalita();" required ng-options="i.valore as i.testo  for i in Finalita">
                                            </select>--%>
                                            <asp:DropDownList ID="Cmb_Finalita" ClientIDMode="Static" class="form-control required" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Tipo" for="Cmb_Tipo">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipo %>" runat="server">Tipo</asp:Localize>
                                            </span>
                                            <%--                                            <select id="Cmb_Tipo" ng-model="macchina.Tipo.val" class="form-control" ng-change="cambioTipo()"
                                                ng-disabled="macchina.Finalita!=0" ng-options="i.valore as i.testo  for i in TipoMacchina">
                                            </select>
                                            <label class="errorAngular" for="Cmb_Tipo" ng-show="((macchina.Finalita==0) && (macchina.Tipo.val=='')) || ((macchina.Finalita==0) && (!macchina.Tipo.val))">
                                                E' necessario selezionare il tipo</label>--%>

                                            <asp:DropDownList ID="Cmb_Tipo" ClientIDMode="Static" class="form-control" runat="server" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Dettaglio1" for="Cmb_Dettaglio1">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dettaglio %>" runat="server">Dettaglio</asp:Localize> 1 *
                                            </span>
                                            <%--                                   <select id="Cmb_Dettaglio1" ng-model="macchina.Dettaglio_1.val" class="form-control"
                                                ng-change="CaricaDettaglio2()" ng-disabled="macchina.Finalita!=0" ng-options="i.valore as i.testo  for i in Dettaglio1">
                                            </select>
                                            <label class="errorAngular" for="Cmb_Dettaglio1" ng-show="macchina.Finalita==0 && Dettaglio1.length>1 && macchina.Dettaglio_1.val ==''">
                                                E' necessario selezionare il dettaglio 1</label>--%>

                                            <asp:DropDownList ID="Cmb_Dettaglio1" ClientIDMode="Static" class="form-control" runat="server" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Dettaglio2" for="Cmb_Dettaglio2">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Dettaglio %>" runat="server">Dettaglio</asp:Localize> 2 *
                                            </span>
                                            <%--                                            <select id="Cmb_Dettaglio2" ng-model="macchina.Dettaglio_2.val" class="form-control"
                                                ng-disabled="macchina.Finalita!=0" ng-options="i.valore as i.testo  for i in Dettaglio2">
                                            </select>
                                            <label class="errorAngular" for="Cmb_Dettaglio2" ng-show="macchina.Finalita==0 && Dettaglio2.length>1 && macchina.Dettaglio_2.val ==''">
                                                E' necessario selezionare il dettaglio 2</label>--%>

                                            <asp:DropDownList ID="Cmb_Dettaglio2" ClientIDMode="Static" class="form-control" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Codice" for="Txt_Codice">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_Codice" ClientIDMode="Static" class="form-control" aria-describedby="lbl_Codice" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="row">



                            <div class="row">
                                <div class="col-lg-12 text-center">
                                    <div class="radio-inline">
                                        <label>
                                            <%--  <input type="radio" name="attivo" ng-model="macchina.Macchina_Attiva" ng-value="true" />--%>
                                            <asp:RadioButton runat="server" ClientIDMode="Static" ID="Opt_Attivo" meta:resourcekey="Opt_Attivo" Text="Macchina/Attrezzatura Attiva" GroupName="StatoUtilizzo" AutoPostBack="true"></asp:RadioButton>
                                        </label>
                                    </div>
                                    <div class="radio-inline">
                                        <label>
                                            <%--  <input type="radio" name="attivo" ng-model="macchina.Macchina_Attiva" ng-value="false" />--%>
                                            <asp:RadioButton runat="server" ClientIDMode="Static" ID="Opt_Dismesso" meta:resourcekey="Opt_Dismesso" Text="Macchina/Attrezzatura Dismessa" GroupName="StatoUtilizzo" AutoPostBack="true"></asp:RadioButton>
                                        </label>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_DataInizioUtilizzo" for="Txt_DataInizioUtilizzo">
                                                    <asp:Localize meta:resourcekey="DataInizioUtilizzo" runat="server">Data Inizio Utilizzo</asp:Localize>
                                                </span>
                                                <%--<input id="Txt_DataInizioUtilizzo" class="form-control datepicker" aria-describedby="lbl_DataInizioUtilizzo"
                                                type="text" ng-model="macchina.Data_Inizio_Utilizzo" ng-change="AggiornaAmmortamento();" />--%>

                                                <asp:TextBox ID="Txt_DataInizioUtilizzo" ClientIDMode="Static" class="form-control datepicker" aria-describedby="lbl_DataInizioUtilizzo" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_DataDismissione" for="Txt_DataDismissione">
                                                    <asp:Localize meta:resourcekey="DataDismissione" runat="server">Data Dismissione</asp:Localize>
                                                </span>
                                                <%--                <input id="Txt_DataDismissione" class="form-control datepicker" aria-describedby="lbl_DataDismissione"
                                                type="text" ng-change="AggiornaAmmortamento();" ng-model="macchina.Data_Dismissione" 
                                                ng-disabled="macchina.Macchina_Attiva ==true"
                                                 />--%>
                                                <asp:TextBox ID="Txt_DataDismissione" class="form-control datepicker" aria-describedby="lbl_DataDismissione"
                                                    type="text" runat="server" ClientIDMode="Static"></asp:TextBox>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>



                            <div class="row" style="padding: 0">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_DtCarico" for="txtDtCarico">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataCarico %>" runat="server">Data Carico</asp:Localize>
                                                </span>
                                                <%--                                                    <input id="txtDtCarico" class="form-control datepicker" aria-describedby="lbl_DtCarico"
                                                        ng-model="macchina.Data_carico" />--%>
                                                <asp:TextBox ID="txtDtCarico" class="form-control datepicker" ClientIDMode="Static" aria-describedby="lbl_DtCarico" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_DtScarico" for="txtDtScarico">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataScarico %>" runat="server">Data Scarico</asp:Localize>
                                                </span>
                                                <%--                                          <input id="txtDtScarico" class="form-control datepicker" aria-describedby="lbl_DtScarico"
                                                        ng-model="macchina.Data_scarico" />--%>
                                                <asp:TextBox ID="txtDtScarico" class="form-control datepicker" ClientIDMode="Static" aria-describedby="lbl_DtScarico" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <hr />
                            <div class="row">

                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_TitoloPossesso" for="ddlTitoloPossesso">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TitoloDiPossesso %>" runat="server">Titolo di Possesso</asp:Localize>
                                                </span>
                                                <%--<select id="ddlTitoloPossesso" ng-model="macchina.Titolo_Possesso" class="form-control"
                                                ng-options="i.valore as i.testo  for i in Titolo_Possesso">
                                            </select>--%>
                                                <asp:DropDownList ID="ddlTitoloPossesso" class="form-control" ClientIDMode="Static" runat="server">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Proprietario" for="txtProprietario">
                                                    <asp:Localize meta:resourcekey="Proprietario" runat="server">Proprietario</asp:Localize>
                                                </span>
                                                <%--             <input id="txtProprietario" class="form-control" aria-describedby="lbl_Proprietario"
                                            ng-model="macchina.Proprietario" />--%>

                                                <asp:TextBox ID="txtProprietario" class="form-control" aria-describedby="lbl_Proprietario" ClientIDMode="Static" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_CUAAProp" for="txtCUAAProp">
                                                    <asp:Localize meta:resourcekey="CUAAProprietario" runat="server">CUAA proprietario</asp:Localize>
                                                </span>
                                                <%--  <input id="txtCUAAProp" class="form-control" aria-describedby="lbl_CUAAProp" ng-model="macchina.CUAA_Proprietario" />--%>
                                                <asp:TextBox ID="txtCUAAProp" class="form-control" aria-describedby="lbl_CUAAProp" ClientIDMode="Static" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="row">
                                <div class="col-lg-12 border_si">
                                    <div class="row">
                                        <div class="col-lg-12 text-center">
                                            <h4 style="color: #052747; text-transform: uppercase;"><asp:Localize meta:resourcekey="ManutenzioniERevisioni" runat="server">MANUTENZIONI E REVISIONI</asp:Localize></h4>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-5 col-md-5 col-sm-12">
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_DataUltimaManutenzione" for="Txt_DataUltimaManutenzione">
                                                                    <asp:Localize meta:resourcekey="DataUltimaManutenzione" runat="server">Data Ultima Manutenzione</asp:Localize>
                                                                </span>
                                                                <%--<input id="Txt_DataUltimaManutenzione" class="form-control  datepicker" aria-describedby="lbl_DataUltimaManutenzione"
                                                                ng-model="macchina.Data_Ultima_Manutenzione" />--%>
                                                                <asp:TextBox ID="Txt_DataUltimaManutenzione" class="form-control  datepicker" ClientIDMode="Static" aria-describedby="lbl_DataUltimaManutenzione" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_DataUltimaRevisione" for="Txt_DataUltimaRevisione">
                                                                    <asp:Localize meta:resourcekey="DataUltimaRevisione" runat="server">Data Ultima Revisione</asp:Localize>
                                                                </span>
                                                                <%--<input id="Txt_DataUltimaRevisione" class="form-control  datepicker" aria-describedby="lbl_DataUltimaRevisione"
                                                                ng-model="macchina.Data_Ultima_Revisione" />--%>
                                                                <asp:TextBox ID="Txt_DataUltimaRevisione" class="form-control  datepicker" aria-describedby="lbl_DataUltimaRevisione" ClientIDMode="Static" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                                <div class="col-lg-4 col-md-4 col-sm-4 col-lg-offset-8 col-md-offset-8">
                                                    <div class="btn btn-info btn_100" onclick="alert('a breve disponibile');">
                                                        <i class="fa fa-plus"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Aggiungi %>" runat="server">Aggiungi</asp:Localize>
                                                    </div>
                                                </div>
                                                <%End If%>
                                            </div>
                                        </div>
                                        <div class="col-lg-7 col-md-7 col-sm-12">
                                            <div id="tabRevisioni">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />


                    </div>
                </div>
                <div class="tab-pane" id="tab_dati_tecnici">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Targa" for="Txt_Targa">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Targa %>" runat="server">Targa</asp:Localize>
                                            </span>
                                            <%--                     <input id="Txt_Targa" class="form-control" aria-describedby="lbl_Targa" ng-model="macchina.Targa" />--%>
                                            <asp:TextBox ID="Txt_Targa" class="form-control" aria-describedby="lbl_Targa" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TipoTarga" for="ddlTipoTarga">
                                                <asp:Localize meta:resourcekey="TipoTarga" runat="server">Tipo Targa</asp:Localize>
                                            </span>
                                            <%--                                          <select name="tipoTarga" ng-model="macchina.Tipo_Targa" class="form-control" ng-options="i.valore as i.testo  for i in Tipo_Targa">
                                            </select>--%>

                                            <asp:DropDownList ID="ddlTipoTarga" name="tipoTarga" class="form-control" ClientIDMode="Static" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Telaio" for="Txt_Telaio">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Telaio %>" runat="server">Telaio</asp:Localize>
                                            </span>
                                            <%--         <input id="txt_telaio" class="form-control" aria-describedby="lbl_Telaio" ng-model="macchina.Telaio" />--%>
                                            <asp:TextBox ID="txt_telaio" class="form-control" aria-describedby="lbl_Telaio" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_NumImmatricolazione" for="Txt_NumImmatricolazione">
                                                <asp:Localize meta:resourcekey="NumeroImmatricolazione" runat="server">N° Immatricolazione</asp:Localize>
                                            </span>
                                            <%--<input id="Txt_NumImmatricolazione" class="form-control" aria-describedby="lbl_NumImmatricolazione"
                                                ng-model="macchina.Numero_Immatricolazione" />--%>
                                            <asp:TextBox ID="Txt_NumImmatricolazione" class="form-control" aria-describedby="lbl_NumImmatricolazione" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_DataImmatricolazione" for="Txt_DataImmatricolazione">
                                                <asp:Localize meta:resourcekey="DataImmatricolazione" runat="server">Data Immatricolazione</asp:Localize>
                                            </span>
                                            <%-- <input id="Txt_DataImmatricolazione" class="form-control datepicker" aria-describedby="lbl_DataImmatricolazione"
                                                ng-model="macchina.Data_Immatricolazione" />--%>
                                            <asp:TextBox ID="Txt_DataImmatricolazione" class="form-control datepicker" aria-describedby="lbl_DataImmatricolazione" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_NumImmatricolazioneRimorchio" for="Txt_NumImmatricolazioneRimorchio">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroImmatricolazioneRimorchio %>" runat="server">
                                                    N° Immatricolazione Rimorchio
                                                </asp:Localize>
                                            </span>
                                            <%--<input id="Txt_NumImmatricolazioneRimorchio" class="form-control" aria-describedby="lbl_NumImmatricolazioneRimorchio"
                                                ng-model="macchina.Numero_Immatricolazione_Rimorchio" />--%>
                                            <asp:TextBox ID="Txt_NumImmatricolazioneRimorchio" class="form-control" aria-describedby="lbl_NumImmatricolazioneRimorchio" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_NumAutorizzazione" for="Txt_NumAutorizzazione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroAutorizzazioneTrasporto %>" runat="server">
                                                    N° Autorizzazione Trasporto
                                                </asp:Localize>
                                            </span>
                                            <%--<input id="Txt_NumAutorizzazione" class="form-control" aria-describedby="lbl_NumAutorizzazione"
                                                ng-model="macchina.Numero_Autorizzazione_Trasporto" />--%>
                                            <asp:TextBox ID="Txt_NumAutorizzazione" class="form-control" aria-describedby="lbl_NumAutorizzazione" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_DataRilascioAutorizzazione" for="Txt_DataRilascioAutorizzazione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRilascioAutorizzazione %>" runat="server">
                                                    Data Rilascio Autorizzazione
                                                </asp:Localize>
                                            </span>
                                            <%--<input id="Txt_DataRilascioAutorizzazione" class="form-control datepicker" aria-describedby="lbl_DataRilascioAutorizzazione"
                                                ng-model="macchina.Data_Rilascio_Autorizzazione" />--%>
                                            <asp:TextBox ID="Txt_DataRilascioAutorizzazione" class="form-control datepicker" aria-describedby="lbl_DataRilascioAutorizzazione" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="row">
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Alimentazione" for="ddlAlimentazione">
                                                <asp:Localize meta:resourcekey="Alimentazione" runat="server">Alimentazione</asp:Localize>
                                            </span>
                                            <%--               <select id="ddlAlimentazione" ng-model="macchina.Alimentazione" class="form-control"
                                                ng-options="i.valore as i.testo  for i in Alimentazione">
                                            </select>--%>

                                            <asp:DropDownList ID="ddlAlimentazione" class="form-control" ClientIDMode="Static" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <h4><asp:Localize meta:resourcekey="TaraturaUgello" runat="server">Taratura Ugello</asp:Localize></h4>
                            <br />
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TaraUgello" for="txtTaraUgello">
                                                <asp:Localize meta:resourcekey="TaraturaUgelloEttolitriPerEttaro" runat="server">Tar. ugello Hl/Ha</asp:Localize>
                                            </span>
                                            <%--<input id="txtTaraUgello" class="form-control" aria-describedby="lbl_TaraUgello"
                                                type="number" ng-model="macchina.Taratura_Ugello" />--%>
                                            <asp:TextBox ID="txtTaraUgello" class="form-control" aria-describedby="lbl_TaraUgello" type="text" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TaraUgelloIni" for="txtDataTaraturaInizio">
                                                <asp:Localize meta:resourcekey="DataUltimaTaratura" runat="server">Data Ultima Taratura</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="txtDataTaraturaInizio" class="form-control datepicker" aria-describedby="lbl_TaraUgello" type="text" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TaraUgelloFin" for="txtDataTaraturaFine">
                                                <asp:Localize meta:resourcekey="ScadenzaTaratura" runat="server">Scadenza Taratura</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="txtDataTaraturaFine" class="form-control datepicker" aria-describedby="lbl_TaraUgello" type="text" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12" style="float: right">
                            <div class="row" style="padding: 0">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Potenza" for="Txt_Potenza">
                                                    <asp:Localize meta:resourcekey="Potenza" runat="server">Potenza</asp:Localize>
                                                </span>
                                                <%--<input id="Txt_Potenza" class="form-control" aria-describedby="lbl_Potenza" type="number"
                                                        ng-model="macchina.Potenza" />--%>
                                                <asp:TextBox ID="Txt_Potenza" class="form-control" aria-describedby="lbl_Potenza" type="text" ClientIDMode="Static" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_PotenzaUdm" for="ddlPotenzaUdm">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, UnitàDiMisura %>" runat="server">Unità di Misura</asp:Localize>
                                                </span>
                                                <%--<select id="ddlPotenzaUdm" ng-model="macchina.UDM_Potenza" class="form-control" ng-options="i.valore as i.testo  for i in Potenza_Udm">
                                                    </select>--%>
                                                <asp:DropDownList ID="ddlPotenzaUdm" class="form-control" ClientIDMode="Static" runat="server">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_StatoUtilizzo" for="Txt_StatoUtilizzo">
                                                <asp:Localize meta:resourcekey="StatoUtilizzo" runat="server">Stato Utilizzo</asp:Localize>
                                            </span>
                                            <%--<input id="Txt_StatoUtilizzo" class="form-control  " aria-describedby="lbl_StatoUtilizzo"
                                                ng-model="macchina.Stato_Utilizzo" />--%>
                                            <asp:TextBox ID="Txt_StatoUtilizzo" class="form-control  " aria-describedby="lbl_StatoUtilizzo" ClientIDMode="Static" runat="server"></asp:TextBox>
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
                                            <span class="input-group-addon alert-info" id="lbl_Note" for="Txt_Note">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Note %>" runat="server">Note</asp:Localize>
                                            </span>
                                            <%--        <input id="Txt_Note" class="form-control  " aria-describedby="lbl_Note" ng-model="macchina.Note" />--%>
                                            <asp:TextBox ID="Txt_Note" class="form-control  " aria-describedby="lbl_Note" ClientIDMode="Static" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                </div>
                <div class="tab-pane" id="tab_costi">
                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_CostoAcquisto" for="Txt_CostoAcquisto">
                                                        <asp:Localize meta:resourcekey="CostoAcquisto" runat="server">Costo Acquisto</asp:Localize> [&euro;] *
                                                    </span>
                                                    <%--<input id="Txt_CostoAcquisto" class="form-control  " aria-describedby="lbl_CostoAcquisto"
                                                        ng-change="AggiornaAmmortamento();" ng-model="macchina.Costo_Acquisto" />--%>
                                                    <asp:TextBox ID="Txt_CostoAcquisto" class="form-control  " aria-describedby="lbl_CostoAcquisto" ClientIDMode="Static" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Manutenzioni" for="Txt_Manutenzioni">
                                                        <asp:Localize meta:resourcekey="ManutenzioniERevisioni" runat="server">Manutenzioni/Revisioni</asp:Localize> [&euro;]
                                                    </span>
                                                    <%--<input id="Txt_Manutenzioni" class="form-control  " aria-describedby="lbl_Manutenzioni"
                                                        ng-model="macchina.Costo_Manutenzione_Revisione" />--%>
                                                    <asp:TextBox ID="Txt_Manutenzioni" class="form-control  " aria-describedby="lbl_Manutenzioni" ClientIDMode="Static" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Ammortamento" for="Txt_Ammortamento">
                                                        <asp:Localize meta:resourcekey="Ammortamento" runat="server">Ammortamento</asp:Localize> [%] *
                                                    </span>
                                                    <%--<input id="Txt_Ammortamento" class="form-control  " aria-describedby="lbl_Ammortamento"
                                                        ng-change="AggiornaAmmortamento();" ng-model="macchina.Ammortamento_Annuo_Percentuale" />--%>
                                                    <asp:TextBox ID="Txt_Ammortamento" class="form-control  " aria-describedby="lbl_Ammortamento" ClientIDMode="Static" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Utilizzo" for="Txt_Utilizzo">
                                                        <asp:Localize meta:resourcekey="AnniUtilizzo" runat="server">Anni Utilizzo</asp:Localize>
                                                    </span>
                                                    <%--<label id="Txt_Utilizzo" class="form-control" aria-describedby="lbl_Utilizzo">
                                                        {{macchina.Anni_Utilizzo}}</label>--%>

                                                    <asp:Label ID="Txt_Utilizzo" class="form-control" aria-describedby="lbl_Utilizzo" ClientIDMode="Static" runat="server">
                                                    </asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Ammortamento_Anno" for="Txt_Ammortamento_Anno">
                                                        <asp:Localize meta:resourcekey="AmmortamentoAnnuo" runat="server">Ammortamento Annuo</asp:Localize> [&euro;]
                                                    </span>
                                                    <%--<label id="Txt_Ammortamento_Anno" class="form-control" aria-describedby="lbl_Ammortamento_Anno">
                                                        {{macchina.Anni_Ammortamento}}</label>--%>
                                                    <asp:Label ID="Txt_Ammortamento_Anno" class="form-control" aria-describedby="lbl_Ammortamento_Anno" ClientIDMode="Static" runat="server">
                                                    </asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Ammortizzato" for="Txt_Ammortizzato">
                                                        <asp:Localize meta:resourcekey="ValoreAmmortizzato" runat="server">Valore Ammortizzato</asp:Localize> [&euro;]
                                                    </span>
                                                    <%--<label id="Txt_Ammortizzato" class="form-control" aria-describedby="lbl_Ammortizzato">
                                                        {{macchina.Valore_Ammortizzato}}</label>--%>
                                                    <asp:Label ID="Txt_Ammortizzato" class="form-control" aria-describedby="lbl_Ammortizzato" ClientIDMode="Static" runat="server">
                                                    </asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="lbl_Valutazione" for="Txt_Valutazione">
                                                        <asp:Localize meta:resourcekey="ValoreAttuale" runat="server">Valore Attuale</asp:Localize> [&euro;]
                                                    </span>
                                                    <%--<label id="Txt_Valutazione" class="form-control" aria-describedby="lbl_Valutazione">
                                                        {{macchina.Valutazione}}</label>--%>
                                                    <asp:Label ID="Txt_Valutazione" class="form-control" aria-describedby="lbl_Valutazione" ClientIDMode="Static" runat="server">
                                                    </asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <asp:CheckBox ID="chk_visibile_ctrl_gestione" class="form-control" aria-describedby="lbl_Ammortizzato" ClientIDMode="Static" runat="server" meta:resourcekey="chk_visibile_ctrl_gestione" Text="Visibile in controllo gestione" />
                                                    <%--<span class="input-group-addon alert-info" id="lbl_visibile_ctrl_gestione" for="Chk_ctrl_gestione">Valore Ammortizzato [&euro;]</span>--%>
                                                    <%--<label id="Txt_Ammortizzato" class="form-control" aria-describedby="lbl_Ammortizzato">
                                                        {{macchina.Valore_Ammortizzato}}</label>--%>
                                                    <%--<asp:Label ID="Label1" class="form-control" aria-describedby="lbl_Ammortizzato" runat="server"></asp:Label>--%>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                            <div class="col-lg-3 col-md-5 col-sm-6 col-lg-offset-9 col-md-offset-7 col-sm-offset-6" style="display: none;">
                                <%--           <div class="btn btn-info btn_100" ng-click="AggiornaAmmortamento();" id="AggiornaAmmortamento">
                                    <i class="fa fa-money"></i>Ricalcola costi
                                </div>--%>
                                <div class="btn btn-info btn_100" onclick="AggiornaAmmortamento();" id="AggiornaAmmortamento">
                                    <i class="fa fa-money"></i><asp:Localize meta:resourcekey="RicalcolaCosti" runat="server">Ricalcola costi</asp:Localize>
                                </div>
                                <%--<div class="btn btn-info btn_100" onclick="$('#<%=ImgBtn_RicalcolaCosti.ClientID %>').click();">
                                        <i class="fa fa-plus"></i>Aggiungi
                                    </div>
                                    <asp:ImageButton ID="ImgBtn_RicalcolaCosti" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                        Style="display: none" />--%>
                            </div>
                            <%End If%>
                        </div>

                        <!--Griglia Kendo Costi-->
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RisorsaCostoUnitario %>" runat="server">COSTO UNITARIO</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <!-- Griglia Kendo Costi -->
                                    <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                        <div id="griglia_costo_unitario"></div>
                                    </div>
                                </div>
                            </div>
                         </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- VARIE -->
        <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
        <input type="hidden" id="clickedTabUI" runat="server" clientidmode="Static" />
        <!-- salva la chiamata alla funzione per il postback -->
        <input type="hidden" id="fooName_PostedBack" runat="server" clientidmode="Static" />
        <!-- parametri le funzione di postback -->
        <input type="hidden" id="fooName_Param_PostedBack" runat="server" clientidmode="Static" />
        <br />
        <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" clientidmode="Static" />
        <input id="FineCentro" name="FineCentro" type="hidden" runat="server" clientidmode="Static" />
        <div class="row" data-toggle="validator" role="form">
            <div class="col-lg-12">
                <hr />
            </div>
        </div>
</asp:Content>
<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Macchina_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Macchina_Edit_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript">
        var jsRevisioni = <%=jsRevisioni%>;
        var jsCosti = <%=jsCosti%>;
    </script>

    <input type="hidden" id="hd_Contatto_Assegnato" runat="server" />

    <input type="hidden" id="hd_Mat_Cod" runat="server" />
    <input type="hidden" id="hd_Piva" runat="server" />
    <input type="hidden" id="hd_UtenteAbilitatoLettura" runat="server" />
    <input type="hidden" id="hd_UtenteAbilitatoScrittura" runat="server" />
    <input type="hidden" id="hd_TipoOperazione" runat="server" />

    <input type="hidden" id="hd_PopUpUMA" runat="server" /> 

     <script type="text/javascript">
         var xContatto_Assegnato = "#<%=hd_Contatto_Assegnato.ClientID() %>"
         var Controls = {
             xMat_Cod: "#<%=hd_Mat_Cod.ClientID() %>",
             xPiva:"#<%=hd_Piva.ClientID() %>"
         }

         var isPopUpUMA = "#<%=hd_PopUpUMA.ClientID() %>"
     </script>

    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
