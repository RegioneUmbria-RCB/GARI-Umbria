<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Appezzamento_Nuovo.aspx.vb" Inherits="AgroAgenda_2010.Appezzamento_Nuovo" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #TxtVerificaPiva {
            font-size: 16px;
        }

        .msgValidita{
            font-size: 10px!important;
            color:red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">

        <div class="tab-pane" style="background-color: #fff">
            <div class="jumbotron">
                <div class="row">

                    <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Centro %>" runat="server">Centro</asp:Localize>: <b>
                            <asp:Label ID="LblCentro" runat="server" ClientIDMode="Static"> </asp:Label></b>
                        <br />
                        <div id="div_lblCampo">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Campo %>" runat="server">Campo</asp:Localize>: <b>
                                <asp:Label ID="LblCampo" ClientIDMode="static" runat="server" Text="0"></asp:Label></b>
                        </div>
                        <div id="div_Superficie_Con">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> [Ha]: <b>
                                <asp:Label ID="LblSuperficie_Con_Catasto" runat="server" Text="0" EnableViewState="true" ClientIDMode="Static"> </asp:Label></b>
                        </div>
                        <asp:TextBox ID="TextBox1" runat="server" CssClass="txtUI " Width="100px" Style="display: none" />
                    </div>

                </div>

                <div class="row" id="div_riepilogo_error" style="margin-bottom: 25px">
                    <div class="col-lg-12 col-md-12">
                        <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CompilareISeguentiCampi %>" runat="server">I seguenti campi devono essere compilati</asp:Localize></b>
                        <br />
                        <br />
                    </div>
                    <div class="col-lg-12 col-md-12 col-sm-12">
                        <ul id="div_riepilogo_error_elenco">
                            <li class="voce_2"><asp:Localize meta:resourcekey="ObbligatorioSuperficie" runat="server">Il campo <b>Superficie</b> è da compilare</asp:Localize></li>
                            <li class="voce_3"><asp:Localize meta:resourcekey="ObbligatorioDataInizio" runat="server">Il campo <b>Data Inizio</b> è da compilare</asp:Localize></li>
                            <!--<li class="voce_4">Il campo <b>Data Fine</b> è da compilare</li>-->
                            <li class="voce_5"><asp:Localize meta:resourcekey="ObbligatorioSpecieVegetale" runat="server">Il campo <b>Specie Vegetale</b> è da selezionare</asp:Localize></li>
                            <li class="voce_6"><asp:Localize meta:resourcekey="ObbligatorioFinalità" runat="server">Il campo <b>Finalità</b> è da selezionare</asp:Localize></li>
                        </ul>
                    </div>
                </div>


                <div class="row">
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_AppNome" for="TxtAppNome">
                                        <asp:Localize meta:resourcekey="DenominazioneAppezzamento" runat="server">Denominazione Appezzamento</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtAppNome" runat="server" CssClass="form-control" ClientIDMode="Static"
                                        MaxLength="50">
                                    </asp:TextBox>
                                </div>
                                <!--<div><i class="fa fa-info-circle"></i><small>Se lasciato vuoto, verrà applicato un nome casuale.</small></div>-->
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_Superficie" for="TxtSuperficie">
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Superficie %>" runat="server">Superficie</asp:Localize> [Ha]
                                    </span>
                                    <asp:TextBox ID="TxtSuperficie" runat="server" CssClass="form-control required" ClientIDMode="Static"
                                        MaxLength="10">
                                    </asp:TextBox>
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
                                    <span class="input-group-addon lbl_required" id="lbl_validita_inizio" for="TxtValiditaInizio"><i class="fa fa-calendar"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataInizio %>" runat="server">Data Inizio</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control required" ClientIDMode="Static">
                                    </asp:TextBox>
                                    <div id="msgTxtValiditaInizio" class="msgValidita"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_validita_fine" for="TxtValiditaFine"><i class="fa fa-calendar"></i>
                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataFine %>" runat="server">Data Fine</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control" ClientIDMode="Static">
                                    </asp:TextBox>
                                    <div id="msgTxtValiditaFine" class="msgValidita"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <asp:CheckBox ID="ChkTerrenoNudo" runat="server" aria-label="..." Style="float: left; padding-right: 15px;" ClientIDMode="Static" />
                        <label aria-describedby="ChkTerrenoNudo" id="lbl_TerrenoNudo" for="ChkTerrenoNudo">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TerrenoNudoNessunaColtura %>" runat="server">Terreno Nudo (Nessuna Coltura)</asp:Localize>
                        </label>
                    </div>
                </div>
                <div class="row">
                    <div id="div_coltura">
                        <div class="col-lg-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_Specie" for="Cmb_Specie">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieVegetale %>" runat="server">Specie Vegetale</asp:Localize>
                                        </span>
                                        <asp:DropDownList ID="Cmb_Specie" runat="server" CssClass="form-control selectpicker required"
                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Specie" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_Finalita" for="Cmb_Finalita">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Finalità %>" runat="server">Finalità</asp:Localize>
                                        </span>
                                        <asp:DropDownList ID="Cmb_Finalita" runat="server" CssClass="form-control selectpicker required stato_group"
                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Finalita" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12" id="div_terreno_nudo">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon lbl_required" id="lbl_CodiciTerreno" for="Cmb_CodiciTerreno">
                                        <asp:Localize meta:resourcekey="UsoExtraAgricolo" runat="server">Uso Extra Agricolo</asp:Localize>
                                    </span>
                                    <asp:DropDownList ID="Cmb_CodiciTerreno" runat="server" CssClass="form-control selectpicker required"
                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_CodiciTerreno" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12 div_salva_btn">
                        <div class="btn btn-success btn_100" onclick="ValidaxSubmit();" runat="server" id="btn_salva1" ClientIDMode="Static" style="white-space: normal; min-height: 50px;">
                            <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                        </div>
                        <asp:HiddenField ID="tipo_salva" runat="server" />
                        <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static"
                            Style="display: none" />
                    </div>
                    <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12 div_salva_btn">
                        <div class="btn btn-info btn_100" onclick="AggiungiDettaglioCatastale();" runat="server" id="btn_salva2" ClientIDMode="Static" style="white-space: normal; min-height: 50px;">
                            <i class="fa fa-plus-circle"></i><asp:Localize meta:resourcekey="AggiungiDettaglioCatastale" runat="server">Aggiungi Dettaglio Catastale</asp:Localize>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12 div_salva_btn">
                        <div class="btn btn-info btn_100" onclick="ValidaxSubmit_Procedi2();" runat="server" id="btn_salva3" ClientIDMode="Static" style="white-space: normal; min-height: 50px;">
                            <i class="fa fa-plus-circle"></i><asp:Localize meta:resourcekey="AggiungiDettagliColturali" runat="server">Aggiungi Dettagli Colturali</asp:Localize>
                        </div>
                        <asp:HiddenField ID="tipo_salva2" runat="server" />
                        <asp:ImageButton ID="ImgBtn_SalvaTutto_Procedi2" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static" Style="display: none" />
                    </div>
                </div>

                <div id="DettagliCatastali" style="display: none">
                    <br />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <input type="checkbox" id="checkMacrousi" class="k-checkbox">
                            <label class="k-checkbox-label" for="checkMacrousi"><asp:Localize Text="<%$ Resources:AgronicaAgenda_2010,VisualizzaIMacrousi %>" runat="server">Visualizza i Macrousi</asp:Localize></label>
                            <input type="checkbox" id="checkUtilizzi" class="k-checkbox">
                            <label class="k-checkbox-label" for="checkUtilizzi"><asp:Localize Text="<%$ Resources:AgronicaAgenda_2010,VisualizzaGliUtilizzi %>" runat="server">Visualizza gli Utilizzi</asp:Localize></label>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <input type="hidden" ID="hTipoFiltro" runat="server" ClientIDMode="Static" />
                            <input type="hidden" ID="hParticelleSelezionate" runat="server" ClientIDMode="Static" />
                            <div id="kendo_Particelle"></div>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12"></div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                <div class="btn btn-success btn_100" onclick="ValidaxSubmit_Procedi1(false);" runat="server" id="Div1" style="white-space: normal; min-height: 50px;">
                                    <i class="fa fa-plus-circle"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                                </div>
                                <asp:HiddenField ID="tipo_salva1" runat="server" />
                                <asp:ImageButton ID="ImgBtn_SalvaTutto_Procedi1" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static" Style="display: none" />
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                <div class="btn btn-success btn_100" onclick="ValidaxSubmit_Procedi1(true);" runat="server" id="btn_salva4" ClientIDMode="Static" style="white-space: normal; min-height: 50px;">
                                    <i class="fa fa-plus-circle"></i><asp:Localize meta:resourcekey="SalvaEAggiungiDettagliColturali" runat="server">Salva e Aggiungi Dettagli Colturali</asp:Localize>
                                </div>                    
                                <asp:ImageButton ID="ImgBtn_SalvaTutto_Procedi3" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico" ClientIDMode="Static" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
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
    <script type="text/javascript">
        var FinestraTemporaleInizio = new Date(<%=objParametri_Server.FinestraTemporaleInizio.Year%>, <%=objParametri_Server.FinestraTemporaleInizio.Month%> - 1, <%=objParametri_Server.FinestraTemporaleInizio.Day%>);
        var FinestraTemporaleFine = new Date(<%=objParametri_Server.FinestraTemporaleFine.Year%>, <%=objParametri_Server.FinestraTemporaleFine.Month%> - 1, <%=objParametri_Server.FinestraTemporaleFine.Day%>);
    </script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Nuovo.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Nuovo_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Appezzamento_Nuovo_jQueryDocReady.js") %>" ></script>
</asp:Content>
