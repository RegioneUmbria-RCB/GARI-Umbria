<%@ Page Title="Investimento Catasto" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="InvestimentoCatasto.aspx.vb" Inherits="AgroAgenda_2010.InvestimentoCatasto" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="InvestimentoCatasto.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_CurrentPiva" runat="server" />
    
    <input id="infoInvestimentoCatasto" type="hidden" value="" />

    <%-- Necessario alla corretta gestione del BtnElaboraDati --%>
    <div class="row">
        <input type="hidden" id="enableBtnElaboraDati" value="false" runat="server" ClientIDMode="Static" />
    </div>
    <%-- Necessario alla preimpostazione della ddl CategoriaEstratta del filtrone--%>
    <div class="row">
        <input type="hidden" id="estrazionePerCampi" value="false" runat="server" ClientIDMode="Static" />
    </div>   

    <div class="container" style="margin-bottom: 70px">
        <div class="row" style="margin-top: 15px">
            <div id="frmTestata" class="form-group">

                <div class="row">
                    <%-- Filtra Appezzamenti --%>
                    <div class="col-lg-3 col-md-2 col-sm-12 col-xs-12" id="divBtnFtiltraAppezzamenti" style="margin-bottom: 20px">
                        <div class="btn btn-success" id="BtnFtiltraAppezzamenti">
                            <span class="fa fa-search"></span>
                            <span class="">
                                <asp:Localize meta:resourcekey="lblBtnFtiltraAppezzamenti" runat="server">Seleziona: Impianti / Campi</asp:Localize>
                            </span>
                        </div>
                    </div>
                    <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12" >
                        <asp:Button ID="Btn_FiltraImpianti" ToolTip="Seleziona gli Impianti su cui copiare l'intervento"
                        runat="server" Text="FILTRO AVANZATO" Style="display: none; margin-left: 5px; margin-right: 5px; margin-bottom: 5px; color: green; font-size: 11px"
                        CssClass="bottone btn_per_load Btn_FiltraImpianti" />
                    </div>
                </div>

                <div class="row">

                    <div class="col-lg-5 col-md-5">
                        <div class="input-group" id="Data_A">
                            <label class="input-group-addon control-label " id="lbl_Data_A" for="Txt_Data_A">
                                <asp:Localize meta:resourcekey="lbl_Data_A" runat="server">Data Inizio Esercizio Precedente Al:</asp:Localize>
                            </label>
                            <input class="form-control kendoDatePicker" id="Txt_Data_A" aria-describedby="lbl_Data_A" type="text" />
                        </div>
                    </div>

                    <div class="col-lg-5 col-md-5">
                        <div class="input-group" id="Data_DA">
                            <label class="input-group-addon control-label " id="lbl_Data_DA" for="Txt_Data_DA">
                                <asp:Localize meta:resourcekey="lbl_Data_DA" runat="server">Data Fine Esercizio Successiva Al:</asp:Localize>
                            </label>
                            <input class="form-control kendoDatePicker" id="Txt_Data_DA" aria-describedby="lbl_Data_DA" type="text" />
                        </div>
                    </div>
                    
                </div>

                <div class="row">
                    <div class="col-lg-10 col-md-10 col-sm-10 col-xs-10">
                        <div class="input-group" id="filtroAvanzatoImpostato">
                            <label class="input-group-addon control-label " style="border: 1px solid #ccc; border-radius: 4px;" id="lblFiltroAvanzatoImpostato" for="">
                                <asp:Label runat="server" ID="lblFiltroImpostato" Text="Nessun Filtro Impostato"></asp:Label>
                            </label>
                        </div>
                    </div>
                </div>

                <%-- <div class="row">
                    <!-- Switch Appezzamenti/Campi -->
                    <div class="col-lg-4 col-md-6 col-sm-6 col-xs-6">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_Switch_App">Estrazione per: Appezzamenti</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_Switch">
                                <%--<label class="k-checkbox-label" for="lbl_Switch_App" style="padding-left: 12.6px;"></label>--%>
                            <%--</div>
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_Switch_Campi">Campi</span>
                        </div>
                    </div>
                </div> --%>

                <%-- Switch Appezzamenti/Campi --%>
                <div class="row">
                    <div style="padding: 10px 15px; margin-bottom: 10px; width: 81.2%">
                        <div style ="position: relative; height: 2.7em">
                            <label class="toggle-switch">
                                <input id="switch-app-campi" type="checkbox">
                                <div class="toggle-back">
                                    <div class="toggle"></div>
                                    <div class="toggle-label on">
                                        ESTRAZIONE PER CAMPI
                                    </div>
                                    <div class="toggle-label off">
                                        ESTRAZIONE PER APPEZZAMENTI
                                    </div>
                                </div>
                            </label> 
                        </div> 
                    </div>
                </div>

                <div class="row">
                    <!-- Appezzamenti Con Riparto -->
                    <div class="col-lg-3 col-md-4 col-sm-12 col-xs-12">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_RipartoAppezzamenti">Appezzamenti Con Riparto</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_RipartoAppezzamenti" class="k-checkbox" checked="checked">
                                <label class="k-checkbox-label" for="lbl_RipartoAppezzamenti" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>

                    <!-- Appezzamenti Senza Riparto -->
                    <div class="col-lg-3 col-md-4 col-sm-12 col-xs-12">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_AppezzaSenzaRiparto">Appezzamenti Senza Riparto</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_AppezzaSenzaRiparto" class="k-checkbox">
                                <label class="k-checkbox-label" for="lbl_AppezzaSenzaRiparto" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>


                    <!-- Particelle Senza Riparto -->
                    <div class="col-lg-3 col-md-4 col-sm-12 col-xs-12">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_ParticelleSenzaRiparto">Particelle in conduzione Senza Riparto</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_ParticelleSenzaRiparto" class="k-checkbox">
                                <label class="k-checkbox-label" for="lbl_ParticelleSenzaRiparto" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row">
                    <!-- Sintesi per CUAA ed estremi catastali -->
                    <div class="col-lg-3 col-md-5 col-sm-12 col-xs-12" id="div_sintesiCUAAEstremiCatastali">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_sintesiCUAAEstremiCatastali">Sintesi per CUAA ed estremi catastali</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_sintesiCUAAEstremiCatastali" class="k-checkbox">
                                <label class="k-checkbox-label" for="lbl_sintesiCUAAEstremiCatastali" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>
                    <!-- Tutto il catasto in archivio -->
                    <div class="col-lg-3 col-md-4 col-sm-12 col-xs-12" id="div_tuttoIlCatastoInArchivio">
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_tuttoIlCatastoInArchivio">Tutto il Catasto in Archivio</span>
                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_tuttoIlCatastoInArchivio" class="k-checkbox">
                                <label class="k-checkbox-label" for="lbl_tuttoIlCatastoInArchivio" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>
                    
                </div>

                <div class="row" id="viste">
                    <%-- Elabora --%>
                    <asp:Panel CssClass="col-lg-1 col-md-6 col-xs-12" ID="divBtnElaboraDati" runat="server">
                        <div class="btn btn-success" id="BtnElaboraDati">
                            <span class="fa fa-cog"></span>
                            <span class="">
                                <asp:Localize meta:resourcekey="lblBtnElaboraDati" runat="server">Elabora</asp:Localize>
                            </span>
                        </div>
                    </asp:Panel>

                    <%-- Viste --%>
                    <div class="col-lg-5 col-md-6 col-xs-12">
                        <div class="form-group">
                            <div class="input-group">
						    <label class="input-group-addon lbl_required" id="lbl_lista_viste" for="selListaViste">Viste salvate:</label>
						    <select name="selListaViste" ID="id_selListaViste" class="form-control" style="white-space: normal;" placeholder="Seleziona una vista..."></select>
                        </div>
                        </div>
					</div>
                    <div class="col-lg-5 col-md-6 col-xs-12">
                        <div class="btn btn-success buttonClass" id="btn_esegui" style="margin-left:10px;">
                            <span class="fa fa-search"></span>Esegui
                        </div>
                        <div class="btn btn-warning buttonClass" id="btn_salva" style="margin-left:10px;">
                            <span class="fa fa-floppy-o"></span>Salva
                        </div>
                        <div class="btn btn-danger buttonClass" id="btn_elimina" style="margin-left:10px;">
                            <span class="fa fa-trash"></span>Elimina
                        </div>
                    </div>					   
                </div>
            </div>
        </div>

        <div id="divInvestimentoCatastoKendoGrid">
        </div>

        <input type="hidden" id="hdInvestimentoCatastoKendoGrid" />

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script>

        var id_Btn_FiltraImpianti = "<%= Btn_FiltraImpianti.Clientid%>";
        var piva = "<%=piva %>";
        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var currentPiva = $('#<%=hd_CurrentPiva.ClientID %>').val();

    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("InvestimentoCatasto.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("InvestimentoCatasto_jQueryDocReady.js") %>"></script>

</asp:Content>

