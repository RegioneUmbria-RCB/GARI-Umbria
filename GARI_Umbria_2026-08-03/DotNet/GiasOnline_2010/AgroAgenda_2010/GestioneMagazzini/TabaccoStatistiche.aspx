<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="TabaccoStatistiche.aspx.vb" Inherits="AgroAgenda_2010.TabaccoStatistiche" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="AnalisiRitiriTabacco.css?<%=Application("GiasVersioneCorrente")%>" rel="stylesheet" type="text/css" media="screen" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <%--SEZIONE DOVE SCRIVERE I MESSAGGI D'ERRORE--%>
        <div id="DIV_Messaggi" style="margin-top:10px;"></div>

        <div id="ricercami" class="jumbotron" style="padding-top: 3px; padding-bottom: 3px">
            <!--  -->
            <div id="frmDati" class="form-horizontal">
                <div class="row">
                    <div id="frmTestata" class="form-group" style="margin-top: 15px;">
                        <div class="row">
                            <%--<label for="lbl_TipoElenco" class="col-md-2 control-label">
                            </label>--%>
                            <div id="RadioButtonListEsporta">
                                <div class="col-md-2">
                                    <label>
                                        <input type="radio" id="q128" name="list_esporta" value="1" />Tutti i Cartoni ritirati
                                    </label> 
                                    <!--<label>
                                        <input type="radio" id="q133" name="list_esporta" value="0" />Tutti i Cartoni (ritirati e non)
                                    </label>-->
                                </div>
                                <div class="col-md-5">
                                    <label>
                                        <input type="radio" id="q129" name="list_esporta" checked="checked" value="2" />Cartoni accorpati per Appezzamento
                                    </label> 
                                    <label>
                                        <input type="radio" id="q130" name="list_esporta" value="3" />Cartoni accorpati per Corona Fogliare
                                    </label> 
                                    <label>
                                        <input type="radio" id="q131" name="list_esporta" value="4" />Cartoni accorpati per Corona Fogliare e suddivisi per Colore
                                    </label> 
                                    <label>
                                        <input type="radio" id="Radio3" name="list_esporta" value="-1" />Sfornature
                                    </label> 
                                </div>
                                <div class="col-md-5">
                                    <label>
                                        <input type="radio" id="q132" class="filtro_report" name="list_esporta" value="5" />Filtro Report (x azienda - 1 foglio per appezzamento)
                                    </label>
                                    <label>
                                        <input type="radio" id="Radio1" class="filtro_report" name="list_esporta" value="6" />Filtro Report (x varietà - trasversale alle aziende)
                                    </label>
                                    <label>
                                        <input type="radio" id="Radio2" class="filtro_report" name="list_esporta" value="7" />Filtro Report (x tecnico + varietà - trasversale alle aziende)
                                    </label> 
                                </div>
                            </div>
                            <div class="col-md-12 text-right">
                                <div class="btn btn-success" id="btn_ricerca">
                                    <span class="fa fa-search lampeggiante"></span><span class="lampeggiante"> Ricerca</span>
                                </div>
                            </div>
                     <%--       <div class="col-md-1">
                            </div>--%>
                        </div>
                        <%--<div class="row" style="margin-top:10px;">
                            <label>Anno</label>
                            <asp:DropDownList ID="ddlAnno" data-live-search="true" data-width="20%" data-container="body" CssClass="selectpicker" runat="server"></asp:DropDownList>
                            <label style="margin-left:20px;">Azienda</label>
                            <asp:DropDownList ID="ddlAzienda" data-live-search="true" data-width="60%" data-container="body" CssClass="selectpicker" runat="server"></asp:DropDownList>
                        </div>
                        <div class="row" style="margin-top:10px;">
                            <label>Varietà</label>
                            <asp:DropDownList ID="ddlVarieta" data-live-search="true" data-width="40%" data-container="body" CssClass="selectpicker" runat="server"></asp:DropDownList>
                            <label style="margin-left:20px;">Tecnico</label>
                            <asp:DropDownList ID="ddlTecnico"  data-live-search="true" data-width="40%" data-container="body" CssClass="selectpicker" runat="server"></asp:DropDownList>
                        </div>--%>
                        <div class="row" style="margin-top:20px;display: none;" id="more_search">
                            <div class="col-lg-3 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Anno" for="ddlAnno">Anno</span>
                                            <asp:DropDownList ID="ddlAnno" data-live-search="true" data-container="body" CssClass="form-control selectpicker" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-9 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Azienda" for="ddlAzienda">Azienda</span>
                                            <asp:DropDownList ID="ddlAzienda" data-live-search="true" data-container="body" CssClass="form-control selectpicker" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Varieta" for="ddlVarieta">Varietà</span>
                                            <asp:DropDownList ID="ddlVarieta" data-live-search="true" data-container="body" CssClass="form-control selectpicker" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                          
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Tecnico" for="ddlTecnico">Tecnico</span>
                                            <asp:DropDownList ID="ddlTecnico" data-live-search="true" data-container="body" CssClass="form-control selectpicker" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
    
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <div style="width: 100%; margin-top: 20px; margin-bottom: 125px;">
            <div style="overflow-x: auto;">
                <div id="tabellaEsitoRicerca">
                </div>
            </div>

        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
       // var cRadioButtonListEsporta = "<=RadioButtonListEsporta.ClientID  %>";
        var anno = "<%=ddlAnno.ClientID %>";
        var azienda = "<%=ddlAzienda.ClientID %>";
        var varieta = "<%=ddlVarieta.ClientID %>";
        var tecnico = "<%=ddlTecnico.ClientID %>";

        var objP_server = '<%=objparametri_server_string %>';
        var objP_utenti = '<%=objparametri_utenti_string %>';

    </script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("TabaccoStatistiche.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("TabaccoStatistiche_jQueryDocReady.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("TabaccoStatistiche_ws_client.js") %>" ></script>
</asp:Content>