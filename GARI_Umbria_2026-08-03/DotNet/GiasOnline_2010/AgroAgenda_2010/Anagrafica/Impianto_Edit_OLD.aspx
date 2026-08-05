<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="Impianto_Edit_OLD.aspx.vb" Inherits="AgroAgenda_2010.Impianto_Edit_OLD" ClientIDMode="static" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        #TxtVerificaPiva
        {
            font-size: 16px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div data-toggle="validator" role="form" class="row">

        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                Centro: <b><asp:Label ID="LblCentro" runat="server"> </asp:Label></b>
                <br />
                 Campo: <b><asp:Label ID="LblCampo" runat="server"> </asp:Label></b>
                        <br />
                 Appezzamento: <b><asp:Label ID="LblAppezza" runat="server"> </asp:Label></b>
                 <br />
                 Sup. Appezzamento [Ha]: <b><asp:Label ID="LblSubAppezza" runat="server"> </asp:Label></b>
                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>
            
                        <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
              
                    <div class="btn btn-success" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                        <i class="fa fa-floppy-o"></i>Salva</div>
                          <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important;">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" style="right: 0; left: auto !important;">
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">Salva
                            ed Esci</a></li>
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">Salva
                            e Nuovo</a></li>
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(2); ValidaxSubmit();">Salva
                            e Continua</a></li>
                    </ul>
                     <% End If%>
                    <asp:HiddenField ID="tipo_salva" runat="server" />
                    <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                        Style="display: none" />
                    <asp:ImageButton ID="ImgBtn_SalvaDistinta" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                        Style="display: none" />
           
                    <% End If%>
                </div>
            </div>
        </div>
        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="Ul1" class="nav nav-tabs" data-tabs="tabs">
                <li class="active tab_dati_impianto" ><a href="#tab_dati_impianto" data-toggle="tab">Dati Impianto</a></li>
                <li class="tab_dati_accessori"><a href="#tab_dati_accessori" data-toggle="tab">Dati Accessori</a></li>
                <li class="tab_dist_prod"><a href="#tab_dist_prod" data-toggle="tab">Distinte di Produzione</a></li>    
                <li class="tab_gis_sps"><a href="#" data-toggle="tab">GIS/GPS</a></li>   
                <li class="tab_analisi_costi"><a href="#" data-toggle="tab">Analisi Costi</a></li>   
                <li class="tab_investimento"><a href="#" data-toggle="tab">Investimento</a></li>  
            </ul>
            <div id="my-tab-content" class="tab-content">
                <!-- TAB 1 -->
                <div class="tab-pane active" id="tab_dati_impianto">
                    <div class="jumbotron">

                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                         
                         <div class="row" id="div_riepilogo_error">
                            <div class="col-lg-12 col-md-12">
                                <b>I seguenti campi devono essere compilati:</b>
                                <br /><br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <ul id="div_riepilogo_error_elenco">
                                    <li class="voce_1">Il campo <b>Specie Vegetale</b> è da selezionare</li>
                                    <li class="voce_2">Il campo <b>Finalità</b> è da selezionare</li>
                                    <li class="voce_3">Il campo <b>Varietà</b> è da selezionare</li>
                                    <li class="voce_4">Il campo <b>Data Inizio</b> è da compilare</li>
                                    <li class="voce_5">Il campo <b>Data Fine</b> è da compilare</li>
        
                                </ul>
                            </div>
                         </div>

                         <% End If%>

                        <div class="row">
                            <div class="col-lg-12">
                                <asp:CheckBox ID="ChkTerrenoNudo" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                <label aria-describedby="ChkTerrenoNudo" id="lbl_TerrenoNudo"
                                    for="ChkTerrenoNudo">
                                    Terreno Nudo (Nessuna Coltura)
                                </label>
                                <input type="hidden" id="InizioAppezzamento" runat="server" name="InizioAppezzamento">
			                    <input type="hidden" id="FineAppezzamento" runat="server" name="FineAppezzamento">
                            </div>
                        </div>
                        <div class="row" id="chk_no">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Specie" for="Cmb_Specie">Specie Vegetale *</span>
                                            <asp:DropDownList ID="Cmb_Specie" runat="server" CssClass="form-control required stato_group"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Specie">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Finalita" for="Cmb_Finalita">Finalità *</span>
                                            <asp:DropDownList ID="Cmb_Finalita" runat="server" CssClass="form-control required stato_group"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Finalita">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Cultivar" for="Cmb_Cultivar">Varietà *</span>
                                            <asp:DropDownList ID="Cmb_Cultivar" runat="server" CssClass="form-control required"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Cultivar">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--<div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_TipologiaVarietale" for="Cmb_TipologiaVarietale">Tipologia Varietale</span>
                                            <asp:DropDownList ID="Cmb_TipologiaVarietale" runat="server" CssClass="form-control selectpicker required"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_TipologiaVarietale">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                          
                        </div>
                        <div class="row" id="chk_yes">
                            <div class="col-lg-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_CodiciTerreno" for="Cmb_CodiciTerreno">Destinazione d'uso *</span>
                                            <asp:DropDownList ID="Cmb_CodiciTerreno" runat="server" CssClass="form-control selectpicker required"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_CodiciTerreno">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12" id="div_coltura_annuale">
                                <asp:CheckBox ID="ChkColturaAnnuale" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                <label aria-describedby="ChkColturaAnnuale" id="lblColturaAnnuale"
                                    for="ChkColturaAnnuale">
                                    Coltura Annuale
                                </label>
                            </div>
                            <div class="col-lg-12">
                                <div><i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b>CENTRO validità: </b>
                                        <asp:Label ID="lbl_centro_data_inizio" runat="server"  CssClass="txtUI" BorderStyle="None"></asp:Label> 
                                        - 
                                        <asp:Label ID="lbl_centro_data_fine" runat="server"  CssClass="txtUI" BorderStyle="None"></asp:Label>
                                    </small>
                                </div>
                            </div>
                            <div class="col-lg-12">
                                <div><i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b>APPEZZAMENTO validità: </b>
                                        <asp:Label ID="lbl_appezza_data_inizio" runat="server"  CssClass="txtUI" BorderStyle="None"></asp:Label> 
                                        - 
                                        <asp:Label ID="lbl_appezza_data_fine" runat="server"  CssClass="txtUI" BorderStyle="None"></asp:Label>
                                    </small>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio"><i class="fa fa-calendar"></i>
 Data Inizio *
                                            </span>
                                            <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="form-control datepicker required"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine"><i class="fa fa-calendar"></i>
 Data Fine *
                                            </span>
                                            <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="form-control datepicker required"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <%--<div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Sup_App" for="TxtSup_App">Sup. appezzamento [Ha]
                                            </span>
                                            <asp:TextBox ID="TxtSup_App" runat="server" CssClass="form-control"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Superficie" for="TxtSuperficie">Sup. netta coltivata [Ha]
                                            </span>
                                            <asp:TextBox ID="TxtSuperficie" runat="server" CssClass="form-control"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- TAB 2 -->
                <div class="tab-pane" id="tab_dati_accessori">
                    <div class="jumbotron">
 
                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_TipologiaVarietale" for="Cmb_TipologiaVarietale">Tipologia Varietale</span>
                                            <asp:DropDownList ID="Cmb_TipologiaVarietale" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_TipologiaVarietale">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                             
                        </div>

                        <!-- Campi opzionali 1 in Tab Dati Accessori -->
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion1">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                            <h4 class="panel-title">
                                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion1" href="#pannello-1"><i class="fa fa-plus-circle"></i>Opzioni Aggiuntive</a>
                                                <div class="tag_opt"><div>Impianto</div><div>Densità impianto</div></div>
                                                <div style="clear: both;"></div>
                                            </h4>
                                        </div>
                                        <div id="pannello-1" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <asp:CheckBox ID="ChkConsociazione" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                        <label aria-describedby="ChkConsociazione" id="lbl_Consociazione"
                                                            for="ChkConsociazione">
                                                            Impianto Consociato
                                                        </label>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <asp:CheckBox ID="ChkCoverCrops" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                        <label aria-describedby="ChkCoverCrops" id="lbl_CoverCrops"
                                                            for="ChkCoverCrops">
                                                            Impianto Cover Crops (di copertura)
                                                        </label>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <asp:CheckBox ID="ChkMonitorato" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                        <label aria-describedby="ChkMonitorato" id="lbl_Monitorato"
                                                            for="ChkMonitorato">
                                                            Impianto Monitorato
                                                        </label>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ImpIrrigazione" for="Cmb_ImpIrrigazione">Imp. Irrigazione</span>
                                                                    <asp:DropDownList ID="Cmb_ImpIrrigazione" runat="server" CssClass="form-control selectpicker"
                                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ImpIrrigazione">
                                                                            </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_FormaAllevamento" for="Cmb_FormaAllevamento">Forma Allevamento</span>
                                                                    <asp:DropDownList ID="Cmb_FormaAllevamento" runat="server" CssClass="form-control selectpicker"
                                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_FormaAllevamento">
                                                                            </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_Portinnesto" for="Cmb_Portinnesto">Portinnesto</span>
                                                                    <asp:DropDownList ID="Cmb_Portinnesto" runat="server" CssClass="form-control selectpicker"
                                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Portinnesto">
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
                                                                    <span class="input-group-addon alert-info" id="lbl_SeminaTrapianto" for="Cmb_SeminaTrapianto">Semina / Trapianto</span>
                                                                    <asp:DropDownList ID="Cmb_SeminaTrapianto" runat="server" CssClass="form-control selectpicker"
                                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_SeminaTrapianto">
                                                                            </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <div class="input-group">
                                                                    <span class="input-group-addon alert-info" id="lbl_ProvenienzaSeme" for="Cmb_ProvenienzaSeme">Provenienza Seme</span>
                                                                    <asp:DropDownList ID="Cmb_ProvenienzaSeme" runat="server" CssClass="form-control selectpicker"
                                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ProvenienzaSeme">
                                                                            </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-lg-12">
                                                        <div class="row">
                                 
                                                            <div class="col-md-12 text-center">
                                                                    <h4 style="color: #052747; text-transform: uppercase;">
                                                                        densità impianto</h4>
                                                            </div>
                
                                                            <%--<div class="col-lg-6 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="lbl_ConduzioneSuFila" for="Cmb_ConduzioneSuFila">Su fila</span>
                                                                            <asp:DropDownList ID="Cmb_ConduzioneSuFila" runat="server" CssClass="form-control selectpicker"
                                                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ConduzioneSuFila">
                                                                                    </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="lbl_ConduzioneTraFila" for="Cmb_ConduzioneTraFila">Tra fila</span>
                                                                            <asp:DropDownList ID="Cmb_ConduzioneTraFila" runat="server" CssClass="form-control selectpicker"
                                                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ConduzioneTraFila">
                                                                                    </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            
                                                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                                <asp:CheckBox ID="ChkFilaBinata" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                                <label aria-describedby="ChkFilaBinata" id="lbl_FilaBinata"
                                                                    for="ChkFilaBinata">
                                                                    Fila Binata
                                                                </label>
                                                            </div>--%>

                                                            
                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                            <h5 style="color: #052747;">
                                                                                Sesto di Impianto</h5>
                                                                    </div>

                                                                    <div class="col-lg-4 col-md-12 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Superficie2" for="TxtSuperficie2">Sup. netta coltivata [Ha] *
                                                                                    </span>
                                                                                    <asp:TextBox ID="TxtSuperficie2" runat="server" CssClass="form-control impianto_input"
                                                                                        MaxLength="10">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaSuFila_M" for="Txt_DistanzaSuFila_M">Distanza su fila [m] *
                                                                                    </span>
                                                                                    <asp:TextBox ID="Txt_DistanzaSuFila_M" runat="server" CssClass="form-control impianto_input">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_DistanzaTraFila_M" for="Txt_DistanzaTraFila_M">Distanza tra fila [m] *
                                                                                    </span>
                                                                                    <asp:TextBox ID="Txt_DistanzaTraFila_M" runat="server" CssClass="form-control impianto_input">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-12">
                                                                        <asp:CheckBox ID="ChkFilaBinata" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                                        <label aria-describedby="ChkFilaBinata" id="lbl_FilaBinata"
                                                                            for="ChkFilaBinata">
                                                                            Fila Binata
                                                                        </label>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Interbina" for="Txt_Interbina">Interbina [m] *
                                                                                    </span>
                                                                                    <asp:TextBox ID="Txt_Interbina" runat="server" CssClass="form-control impianto_input">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_Germinabilita" for="Txt_Germinabilita">Germinabilità [%] *
                                                                                    </span>
                                                                                    <asp:TextBox ID="Txt_Germinabilita" runat="server" CssClass="form-control impianto_input">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                     

                                                            <div class="col-md-12">
                                                                <div class="row">
                                                                    <div class="col-md-12">
                                                                            <h5 style="color: #052747;">
                                                                                Calcolo Piante</h5>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_PianteHa" for="TxtPianteHa">Piante/Ha
                                                                                    </span>
                                                                                    <asp:TextBox ID="TxtPianteHa" runat="server" CssClass="form-control">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="form-horizontal">
                                                                            <div class="form-group">
                                                                                <div class="input-group">
                                                                                    <span class="input-group-addon alert-info" id="lbl_PianteImpianto" for="TxtPianteImpianto">Piante/Impianto
                                                                                    </span>
                                                                                    <asp:TextBox ID="TxtPianteImpianto" runat="server" CssClass="form-control">
                                                                                    </asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <%--<div class="col-lg-4 col-md-6 col-sm-12">
                                                                        <div class="btn btn-info" id="btn_piante_calcola">
                                                                            <i class="fa fa-calculator"></i>Calcola
                                                                        </div>
                                                                    </div>--%>
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
                       <%-- <div class="row">
                            <div class="col-lg-12">
                                <asp:CheckBox ID="ChkConsociazione" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                <label aria-describedby="ChkConsociazione" id="lbl_Consociazione"
                                    for="ChkConsociazione">
                                    Impianto Consociato
                                </label>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <asp:CheckBox ID="ChkCoverCrops" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                <label aria-describedby="ChkCoverCrops" id="lbl_CoverCrops"
                                    for="ChkCoverCrops">
                                    Impianto Cover Crops (di copertura)
                                </label>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <asp:CheckBox ID="ChkMonitorato" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                <label aria-describedby="ChkMonitorato" id="lbl_Monitorato"
                                    for="ChkMonitorato">
                                    Impianto Monitorato
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_ImpIrrigazione" for="Cmb_ImpIrrigazione">Imp. Irrigazione</span>
                                            <asp:DropDownList ID="Cmb_ImpIrrigazione" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ImpIrrigazione">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_FormaAllevamento" for="Cmb_FormaAllevamento">Forma Allevamento</span>
                                            <asp:DropDownList ID="Cmb_FormaAllevamento" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_FormaAllevamento">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Portinnesto" for="Cmb_Portinnesto">Portinnesto</span>
                                            <asp:DropDownList ID="Cmb_Portinnesto" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Portinnesto">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                 
                                    <div class="col-md-12 text-center">
                                            <h4 style="color: #052747; text-transform: uppercase;">
                                                Conduzione del terreno</h4>
                                    </div>
                
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon" id="lbl_ConduzioneSuFila" for="Cmb_ConduzioneSuFila">Su fila</span>
                                                    <asp:DropDownList ID="Cmb_ConduzioneSuFila" runat="server" CssClass="form-control selectpicker"
                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ConduzioneSuFila">
                                                            </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon" id="lbl_ConduzioneTraFila" for="Cmb_ConduzioneTraFila">Tra fila</span>
                                                    <asp:DropDownList ID="Cmb_ConduzioneTraFila" runat="server" CssClass="form-control selectpicker"
                                                                AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ConduzioneTraFila">
                                                            </asp:DropDownList>
                                                </div>
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
                                            <span class="input-group-addon" id="lbl_SeminaTrapianto" for="Cmb_SeminaTrapianto">Semina / Trapianto</span>
                                            <asp:DropDownList ID="Cmb_SeminaTrapianto" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_SeminaTrapianto">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_ProvenienzaSeme" for="Cmb_ProvenienzaSeme">Provenienza Seme</span>
                                            <asp:DropDownList ID="Cmb_ProvenienzaSeme" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_ProvenienzaSeme">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>--%>
                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info" id="lbl_Copertura" for="Cmb_Copertura">Copertura</span>
                                            <asp:DropDownList ID="Cmb_Copertura" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Copertura">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--  <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_DettaglioVarietaPersonalizzato" for="Cmb_DettaglioVarietaPersonalizzato">Dett. Specie Personalizzato</span>
                                            <asp:DropDownList ID="Cmb_DettaglioVarietaPersonalizzato" runat="server" CssClass="form-control selectpicker"
                                                        AutoPostBack="False" data-live-search="true" aria-describedby="lbl_DettaglioVarietaPersonalizzato">
                                                    </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_CopDataInizio" for="TxtCopDataInizio">Dal
                                            </span>
                                            <asp:TextBox ID="TxtCopDataInizio" runat="server" CssClass="form-control datepicker"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_CopDataFine" for="TxtCopDataFine">Al
                                            </span>
                                            <asp:TextBox ID="TxtCopDataFine" runat="server" CssClass="form-control datepicker"
                                                MaxLength="10">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
  
                        </div>
                        <div class="row">
                        <!-- Campi opzionali 2 in Tab Dati Accessori -->
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion2">
                                     <div class="panel panel-primary">
                                      <div class="panel-heading">
                                       <h4 class="panel-title">
                                       <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion2" href="#pannello-2"><i class="fa fa-plus-circle"></i>Opzioni Aggiuntive</a>
                                       <div class="tag_opt"><div>Periodo copertura</div></div>
                                                <div style="clear: both;"></div>
                                       </h4>
                                      </div>
                                      <div id="pannello-2" class="panel-collapse collapse">
                                          <div class="panel-body">
                                            <div class="row">
                                                <div class="col-lg-6 col-md-6 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_CopDataInizio" for="TxtCopDataInizio"><i class="fa fa-calendar"></i>
 Dal</span>
                                                                <asp:TextBox ID="TxtCopDataInizio" runat="server" CssClass="form-control datepicker"
                                                                    MaxLength="10">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-6 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_CopDataFine" for="TxtCopDataFine"><i class="fa fa-calendar"></i>
 Al</span>
                                                                <asp:TextBox ID="TxtCopDataFine" runat="server" CssClass="form-control datepicker"
                                                                    MaxLength="10">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-md-6 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_DettaglioVarietaPersonalizzato" for="Cmb_DettaglioVarietaPersonalizzato">Dett. Specie Personalizzato</span>
                                                                <asp:DropDownList ID="Cmb_DettaglioVarietaPersonalizzato" runat="server" CssClass="form-control selectpicker" data-container="body"
                                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_DettaglioVarietaPersonalizzato">
                                                                        </asp:DropDownList>
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
       
                        
                        <div class="row">
                            <!-- Campi opzionali 3 in Tab Dati Accessori -->
                            <div class="col-lg-12">
                                <div class="panel-group" id="accordion3">
                                    <div class="panel panel-primary">
                                        <div class="panel-heading">
                                           <h4 class="panel-title">
                                           <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion3" href="#pannello-3"><i class="fa fa-plus-circle"></i>Opzioni Aggiuntive</a>
                                           <div class="tag_opt"><div>Varietà e linee</div></div>
                                                <div style="clear: both;"></div>
                                           </h4>
                                        </div>
                                        <div id="pannello-3" class="panel-collapse collapse">
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <asp:CheckBox ID="ChkVarietaIbrida" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                        <label aria-describedby="ChkVarietaIbrida" id="lbl_VarietaIbrida"
                                                            for="ChkVarietaIbrida">
                                                            Varietà Ibrida
                                                        </label>
                                                    </div>
                                                <%--    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                                        <asp:CheckBox ID="ChkFilaBinata" runat="server" aria-label="..." style="float: left; padding-right: 15px;" />
                                                        <label aria-describedby="ChkFilaBinata" id="lbl_FilaBinata"
                                                            for="ChkFilaBinata">
                                                            Fila Binata
                                                        </label>
                                                    </div>--%>
                                                </div>
                                                <div class="row">
                                          
                                                    <div class="col-md-12 text-center">
                                                            <h4 style="color: #052747; text-transform: uppercase;">
                                                                Linea maschio</h4>
                                                    </div>
                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                    <h5 style="color: #052747;">
                                                                        Genetica</h5>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_CodBMBDBT_M" for="Txt_CodBMBDBT_M">Cod. BM-BD-BT
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_CodBMBDBT_M" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_Genetica_M" for="Txt_Genetica_M">Genetica
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_Genetica_M" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon alert-info" id="lbl_OffType_M" for="Txt_OffType_M">Off-Type
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_OffType_M" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                 <%--
                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                    <h5 style="color: #052747;">
                                                                        Sesto di Impianto</h5>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon lbl_required" id="lbl_DistanzaSuFila_M" for="Txt_DistanzaSuFila_M">Distanza su fila [m]
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_DistanzaSuFila_M" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon lbl_required" id="lbl_DistanzaTraFila_M" for="Txt_DistanzaTraFila_M">Distanza tra fila [m]
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_DistanzaTraFila_M" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon lbl_required" id="lbl_Interbina" for="Txt_Interbina">Interbina [m]
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_Interbina" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                      
                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                    <h5 style="color: #052747;">
                                                                        Germinabilità</h5>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon lbl_required" id="lbl_Germinabilita" for="Txt_Germinabilita">Germinabilità [%]
                                                                            </span>
                                                                            <asp:TextBox ID="Txt_Germinabilita" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                    <h5 style="color: #052747;">
                                                                        Calcolo Piante</h5>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="lbl_PianteHa" for="TxtPianteHa">Piante/Ha
                                                                            </span>
                                                                            <asp:TextBox ID="TxtPianteHa" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="form-horizontal">
                                                                    <div class="form-group">
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon" id="lbl_PianteImpianto" for="TxtPianteImpianto">Piante/Impianto
                                                                            </span>
                                                                            <asp:TextBox ID="TxtPianteImpianto" runat="server" CssClass="form-control">
                                                                            </asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-4 col-md-6 col-sm-12">
                                                                <div class="btn btn-info" id="btn_piante_calcola">
                                                                    <i class="fa fa-calculator"></i>Calcola
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>--%>
                                             
                                                

                                                    <div class="col-lg-12" id="linea-femmina">
                                                        <hr />
                                                        <div class="row">
                                                            <div class="col-md-12 text-center">
                                                                <h4 style="color: #052747; text-transform: uppercase;">
                                                                    Linea femmina</h4>
                                                        </div>

                                                            <div class="col-md-12">
                                                            <div class="row">
                                                                <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            Genetica</h5>
                                                                </div>
                                                                <div class="col-lg-4 col-md-6 col-sm-12">
                                                                    <div class="form-horizontal">
                                                                        <div class="form-group">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon alert-info" id="lbl_CodBMBDBT_F" for="Txt_CodBMBDBT_F">Cod. BM-BD-BT
                                                                                </span>
                                                                                <asp:TextBox ID="Txt_CodBMBDBT_F" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-4 col-md-6 col-sm-12">
                                                                    <div class="form-horizontal">
                                                                        <div class="form-group">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon alert-info" id="lbl_Genetica_M" for="Txt_Genetica_F">Genetica
                                                                                </span>
                                                                                <asp:TextBox ID="Txt_Genetica_F" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-4 col-md-6 col-sm-12">
                                                                    <div class="form-horizontal">
                                                                        <div class="form-group">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon alert-info" id="lbl_OffType_F" for="Txt_OffType_F">Off-Type
                                                                                </span>
                                                                                <asp:TextBox ID="Txt_OffType_F" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                            <div class="col-md-12">
                                                            <div class="row">
                                                                <div class="col-md-12">
                                                                        <h5 style="color: #052747;">
                                                                            Sesto di Impianto</h5>
                                                                </div>
                                                                <div class="col-lg-4 col-md-6 col-sm-12">
                                                                    <div class="form-horizontal">
                                                                        <div class="form-group">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon alert-info" id="lbl_DistanzaSuFila_F" for="Txt_DistanzaSuFila_F">Distanza su fila [m]
                                                                                </span>
                                                                                <asp:TextBox ID="Txt_DistanzaSuFila_F" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-4 col-md-6 col-sm-12">
                                                                    <div class="form-horizontal">
                                                                        <div class="form-group">
                                                                            <div class="input-group">
                                                                                <span class="input-group-addon alert-info" id="lbl_DistanzaTraFila_F" for="Txt_DistanzaTraFila_F">Distanza tra fila [m]
                                                                                </span>
                                                                                <asp:TextBox ID="Txt_DistanzaTraFila_F" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
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
                                        
                            </div>
                        </div>
                    </div>
                </div>

                <!-- TAB 3 -->
                <div class="tab-pane" id="tab_dist_prod">
                    <div class="jumbotron"> 
                        <div class="row">
                            <div class="col-lg-12 col-md-12 text-center">
                                    <h4 style="color: #052747; text-transform: uppercase;">
                                        Esercizi Impianti</h4>
                            </div>
                        </div>
                        <div class="row" style="min-height: 30px">
                            <div class="col-lg-12 col-md-12 text-right">
                                    <div class="btn btn-success" id="btn_nuova_distinta" runat="server">
                                        <i class="fa fa-plus"></i>Nuova Distinta
                                    </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="table-responsive">
                                    <div id="tabImpianti">
                                    </div>

                                </div>
                            </div>
                        </div>
                        <div id="wait" style="">Carico...</div>
                        <div class="scheda_distinta">
                            <div class="row">  
                                <div class="col-md-12">
                                        <h5 style="color: #052747;">
                                            Date di Esercizio</h5>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_ValiditaInizio_Distinta" for="Txt_ValiditaInizio_Distinta"><i class="fa fa-calendar"></i>
     Inizio *</span>
                                                <asp:TextBox ID="Txt_ValiditaInizio_Distinta" runat="server" CssClass="form-control datepicker">
                                                                        </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_ValiditaFine_Distinta" for="Txt_ValiditaFine_Distinta"><i class="fa fa-calendar"></i>
     Fine *</span>
                                                <asp:TextBox ID="Txt_ValiditaFine_Distinta" runat="server" CssClass="form-control datepicker">
                                                                        </asp:TextBox>
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
                                                <span class="input-group-addon alert-info" id="lbl_Lotto" for="Txt_Lotto">Lotto Impianto</span>
                                                <asp:TextBox ID="Txt_Lotto" runat="server" CssClass="form-control">
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
                                                <span class="input-group-addon alert-info" id="lbl_PianteImpianto2" for="TxtPianteImpianto2">Piante/Impianto Effettive</span>
                                                <asp:TextBox ID="TxtPianteImpianto2" runat="server" CssClass="form-control">
                                                                        </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>                                                
                            </div>

                            <div class="row">
                                <div class="col-lg-12 border_si">
                                    <div class="row">
                                        <div class="col-md-12">
                                                <h5 style="color: #052747;">
                                                    Dati in Previsione</h5>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_semina_prevista" for="Txt_semina_prevista"><i class="fa fa-calendar"></i>
     Data Semina</span>
                                                        <asp:TextBox ID="Txt_semina_prevista" runat="server" CssClass="form-control datepicker">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_raccolta_prevista" for="Txt_raccolta_prevista"><i class="fa fa-calendar"></i>
     Data Raccolta</span>
                                                        <asp:TextBox ID="Txt_raccolta_prevista" runat="server" CssClass="form-control datepicker">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_fioritura_prevista" for="Txt_fioritura_prevista"><i class="fa fa-calendar"></i>
     Data Fioritura</span>
                                                        <asp:TextBox ID="Txt_fioritura_prevista" runat="server" CssClass="form-control datepicker">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_ResaPrevista" for="Txt_ResaPrevista">Resa [Kg/Ha]</span>
                                                        <asp:TextBox ID="Txt_ResaPrevista" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row">
                                        <div class="col-md-12">
                                                <h5 style="color: #052747;">
                                                    Dati in Storico</h5>
                                        </div>
                                        <div class="col-lg-4 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Resa1" for="Txt_Resa1">Resa Prevista [Kg tot.]
                                                        </span>
                                                        <asp:TextBox ID="Txt_Resa1" runat="server" CssClass="form-control">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Resa2" for="Txt_Resa2">Resa Corretta [Kg tot.]
                                                        </span>
                                                        <asp:TextBox ID="Txt_Resa2" runat="server" CssClass="form-control">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-4 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_UnitaVitata" for="TXT_UnitaVitata">Unità Vitata
                                                        </span>
                                                        <asp:TextBox ID="TXT_UnitaVitata" runat="server" CssClass="form-control">
                                                        </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>



                            <!-- Campi opzionali 2 in Tab Distinte Produzioni -->
                          <%--  <div class="row">
                                <div class="col-lg-12">

                                    <div class="panel-group" id="accordion5">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                               <h4 class="panel-title">
                                               <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion5" href="#pannello-5"><i class="fa fa-plus-circle"></i>Opzioni Aggiuntive</a>
                                               <div class="tag_opt"><div>Fioritura e Resa prevista</div></div>
                                                    <div style="clear: both;"></div>
                                               </h4>
                                            </div>
                                            <div id="pannello-5" class="panel-collapse collapse">
                                                <div class="panel-body">
                                                    <div class="row">
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon" id="lbl_fioritura_prevista" for="Txt_fioritura_prevista">Data Fioritura Prevista</span>
                                                                        <asp:TextBox ID="Txt_fioritura_prevista" runat="server" CssClass="form-control datepicker">
                                                                                                </asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon" id="lbl_ResaPrevista" for="Txt_ResaPrevista">Resa Kg/Ha Prevista</span>
                                                                        <asp:TextBox ID="Txt_ResaPrevista" runat="server" CssClass="form-control">
                                                                                                </asp:TextBox>
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
                            </div>--%>

                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Regolamento" for="Cmb_Regolamento">Regolamento</span>
                                                <asp:DropDownList ID="Cmb_Regolamento" runat="server" CssClass="form-control selectpicker"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Regolamento">
                                                        </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="lbl_Disciplinare" for="Cmb_Disciplinare">Disciplinare</span>
                                                <asp:DropDownList ID="Cmb_Disciplinare" runat="server" CssClass="form-control selectpicker"
                                                            AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Disciplinare">
                                                        </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Campi opzionali 3 in Tab Distinte Produzioni -->
                            <div class="row">
                                <div class="col-lg-12">

                                    <div class="panel-group" id="accordion6">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                               <h4 class="panel-title">
                                               <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion6" href="#pannello-6"><i class="fa fa-plus-circle"></i>Opzioni Aggiuntive</a>
                                               <div class="tag_opt"><div>Altre Info</div></div>
                                                    <div style="clear: both;"></div>
                                               </h4>
                                            </div>
                                            <div id="pannello-6" class="panel-collapse collapse">
                                                <div class="panel-body">
                                                    <div class="row">
                                                       <div class="col-lg-12 col-md-12 col-sm-12">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon alert-info" id="lbl_CapitolatoPrivato" for="Cmb_CapitolatoPrivato">Capitolato Privato</span>
                                                                        <asp:DropDownList ID="Cmb_CapitolatoPrivato" runat="server" CssClass="form-control selectpicker" data-container="body"
                                                                                    AutoPostBack="False" data-live-search="true" aria-describedby="lbl_CapitolatoPrivato">
                                                                                </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon alert-info" id="lbl_OrganismoReferente" for="Cmb_OrganismoReferente">Organismo Referente</span>
                                                                        <asp:DropDownList ID="Cmb_OrganismoReferente" runat="server" CssClass="form-control"
                                                                                    AutoPostBack="False" data-live-search="true" aria-describedby="lbl_OrganismoReferente">
                                                                                </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-12 col-md-12 col-sm-12">
                                                            <div class="form-horizontal">
                                                                <div class="form-group">
                                                                    <div class="input-group">
                                                                        <span class="input-group-addon alert-info" id="lbl_MagazzinoConferimento" for="Cmb_MagazzinoConferimento">Magazzino Conferimento</span>
                                                                        <asp:DropDownList ID="Cmb_MagazzinoConferimento" runat="server" CssClass="form-control"
                                                                                    AutoPostBack="False" data-live-search="true" aria-describedby="lbl_MagazzinoConferimento">
                                                                                </asp:DropDownList>
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

                            <div class="row">
                                <div class="col-lg-12 border_si">
                                    <div class="row">
                                 
                                        <div class="col-md-12 text-center">
                                                <h4 style="color: #052747; text-transform: uppercase;">
                                                    Apporti Massimi di Macroelementi</h4>
                                        </div>
                     
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_RegolamentoConc" for="Cmb_RegolamentoConc">Reg. Fertilizzazioni</span>
                                                        <asp:DropDownList ID="Cmb_RegolamentoConc" runat="server" CssClass="form-control selectpicker stato_group"
                                                                    AutoPostBack="False" data-live-search="true" aria-describedby="lbl_RegolamentoConc">
                                                                </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-6 col-sm-12">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_Stato" for="Cmb_Stato">Stato Impianto</span>
                                                        <asp:DropDownList ID="Cmb_Stato" runat="server" CssClass="form-control selectpicker"
                                                                    AutoPostBack="False" data-live-search="true" aria-describedby="lbl_Stato">
                                                                </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_N" for="TxtN">N [kg/ha]</span>
                                                        <asp:TextBox ID="TxtN" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_P2O5" for="TxtP2O5">P2O5 [kg/ha]</span>
                                                        <asp:TextBox ID="TxtP2O5" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_K2O" for="TxtK2O">K2O [kg/ha]</span>
                                                        <asp:TextBox ID="TxtK2O" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="input-group">
                                                        <span class="input-group-addon alert-info" id="lbl_MgO" for="TxtMgO">MgO [kg/ha]</span>
                                                        <asp:TextBox ID="TxtMgO" runat="server" CssClass="form-control">
                                                                                </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <hr />

                            <div class="row">
                                <div class="col-lg-12 border_si">
                                    <div class="row">
                                        <div class="col-md-12 text-center">
                                            <h4 style="color: #052747; text-transform: uppercase;">
                                                Codici Anagrafici</h4>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-5">
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="div_CmbCodice">
                                                                <span class="input-group-addon alert-info" id="lbl_Codice" for="CmbCodice">Codice:</span>
                                                                <asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control selectpicker"
                                                                    data-live-search="true" aria-describedby="lbl_Codice">
                                                                </asp:DropDownList>
                                                                <%--<asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control"
                                                                    data-live-search="true" aria-describedby="lbl_Codice">
                                                                </asp:DropDownList>--%>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_valorecod" for="TxtCodiceValore">Valore:</span>
                                                                <asp:TextBox ID="TxtCodiceValore" runat="server" CssClass="form-control" aria-describedby="lbl_valorecod">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" id="div_add_codice">
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                                <div class="col-lg-12 text-right">
                                                    <div class="btn btn-info" id="btn_Aggiungi_Codice">
                                                        <i class="fa fa-plus"></i>Aggiungi
                                                    </div>
                                                    <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_Aggiungi_Codice.ClientID %>').click();">
                                                        <i class="fa fa-plus"></i>Aggiungi
                                                    </div>
                                                    <asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" Style="display: none" />--%>
                                                </div>
                                                <%End If%>
                                            </div>
                                        </div>
                                        <div class="col-lg-7">
                                            <div id="tabCodici">
                                            </div>
                                        </div>
                                   
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12 border_si">
                                    <div class="row">
                                        <div class="col-md-12 text-center">
                                            <h4 style="color: #052747; text-transform: uppercase;">
                                                Particelle</h4>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-5">
                                            <div class="row">
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group" id="div1">
                                                                <span class="input-group-addon alert-info" id="lbl_Particella" for="Cmb_Particella">Particella:</span>
                                                                <asp:DropDownList ID="Cmb_Particella" runat="server" CssClass="form-control selectpicker"
                                                                    data-live-search="true" aria-describedby="lbl_Particella">
                                                                </asp:DropDownList>
                                                                <%--<asp:DropDownList ID="CmbCodice" runat="server" CssClass="form-control"
                                                                    data-live-search="true" aria-describedby="lbl_Codice">
                                                                </asp:DropDownList>--%>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon alert-info" id="lbl_ParticellaValore" for="TxtParticellaValore">Progressivo:</span>
                                                                <asp:TextBox ID="TxtParticellaValore" runat="server" CssClass="form-control" aria-describedby="lbl_ParticellaValore">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row" id="div_add_particella">
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impresa).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>
                                                <div class="col-lg-12 text-right">
                                                    <div class="btn btn-info" id="btn_Aggiungi_Particella">
                                                        <i class="fa fa-plus"></i>Aggiungi
                                                    </div>
                                                    <%--<div class="btn btn-info" onclick="$('#<%=ImgBtn_Aggiungi_Codice.ClientID %>').click();">
                                                        <i class="fa fa-plus"></i>Aggiungi
                                                    </div>
                                                    <asp:ImageButton ID="ImgBtn_Aggiungi_Codice" runat="server" Style="display: none" />--%>
                                                </div>
                                                <%End If%>
                                            </div>
                                        </div>
                                        <div class="col-lg-7">
                                            <div id="tabParticelle">
                                            </div>
                                        </div>
                                   
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-12 text-right">
                                        <div class="btn btn-success" id="btn_salva_distinta" onclick="ValidaxDistinta();">
                                            <i class="fa fa-floppy-o"></i>Salva Distinta
                                        </div>
                                </div>
                            </div>


                        </div>

                    </div>
                </div><!-- fine TAB 3 -->
                                
                                
            </div>

        </div>

                
    </div>    
      
     
    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <asp:ImageButton ID="ImgBtn_Cancella_Distinta" runat="server" Style="display: none" />
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
    <br />
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" />
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" />

    
</asp:Content>

<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Impianto_Edit.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script src="Impianto_Edit_ws_client.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript">


        jQuery(document).ready(function () {

            //PRIMA ERA NEL FILE A PARTE

            //$('#wait').hide();
            WaitFrame.show();

            //var Cmb_Specie = inizializzaKendoDropDown("Cmb_Specie");
            //var Cmb_Finalita = inizializzaKendoDropDown("Cmb_Finalita");
            //var Cmb_Cultivar = inizializzaKendoDropDown("Cmb_Cultivar");

            $('.selectpicker').change(function(a, b, c){
                alert("seleckpicker change");
            });
            $('#aspnetForm').change(function () {
                controlla_form();
            });

            $('.datepicker').on('changeDate', function (ev) {
                $(this).datepicker('hide');
                controlla_form();
            });


            $('.tab_dist_prod').click(function () {
                // sottolineo la prima riga della watable degli esercizi
                $('#tabImpianti table tbody tr:first td').each(function () {
                    $(this).css('background-color', '#428bca');
                });


            });



            if ($('#ChkTerrenoNudo').is(':checked')) {
                $('#chk_no').hide();
                $('#chk_yes').show();
            }
            else {
                $('#chk_no').show();
                $('#chk_yes').hide();
            }


            // Disabilito tutte le select per la modalità Lettura
            // Disabilito tutte le select per la modalità Lettura
            if ($('#TxtValiditaInizio').is(":disabled")) {
                $('#Cmb_Finalita').attr('disabled', true);
                $('Cmb_Cultivar').attr('disabled', true);
                $('#Cmb_TipologiaVarietale').attr('disabled', true);
                $('#Cmb_FormaAllevamento').attr("disabled", true);
            }
            else {
                // Disabilito la select semina/trapianto
                if ($('#Cmb_Specie').val() === 6)
                    $('#Cmb_SeminaTrapianto').attr('disabled', false);
                else
                    $('#Cmb_SeminaTrapianto').attr('disabled', true);



                if ($('#Cmb_Specie').val() === "") {
                    $('#Cmb_Finalita').attr("disabled", true);
                    $('#Cmb_Cultivar').attr("disabled", true);
                }
                else {
                    $('#Cmb_Finalita').attr("disabled", false);
                    $('Cmb_Cultivar').attr("disabled", false);

                }


                if ($('#Cmb_Cultivar').val() != "") {

                    //$('#Cmb_Cultivar').attr("disabled", false);
                    //$('#<=Cmb_Finalita.ClientID %>').attr("disabled", false);
                    $('#Cmb_TipologiaVarietale').attr("disabled", false);


                    var testo;

                    $("#Cmb_Specie option").each(function () {
                        if (this.value === $('#Cmb_Specie').val()) {
                            //alert(this.value);
                            testo = this.text;
                        }
                    });

                    if (testo.indexOf("Arboree") >= 0) {
                        $('#Cmb_FormaAllevamento').attr("disabled", false);
                        $('#Cmb_Portinnesto').attr("disabled", false);
                        $('#Cmb_SeminaTrapianto').attr("disabled", false);
                    }
                    else {
                        $('#Cmb_FormaAllevamento').attr("disabled", true);
                        $('#Cmb_Portinnesto').attr("disabled", true);
                        $('#Cmb_SeminaTrapianto').attr("disabled", true);
                    }


                }
                else {
                    //$('#Cmb_Cultivar').attr("disabled", true);
                    //$('#<=Cmb_Finalita.ClientID %>').attr("disabled", true);
                    $('#Cmb_TipologiaVarietale').attr("disabled", true);
                    $('#Cmb_FormaAllevamento').attr("disabled", true);
                    $('#Cmb_Portinnesto').attr("disabled", true);
                    $('#Cmb_SeminaTrapianto').attr("disabled", true);
                }



            }

            // Disabilito la select semina/trapianto
            //            if($('#<=Cmb_Specie.ClientID %>').val() == 6)
            //                $('#<=Cmb_SeminaTrapianto.ClientID %>').attr('disabled', false);
            //            else
            //                $('#<=Cmb_SeminaTrapianto.ClientID %>').attr('disabled', true);


            $('.tab_dist_prod').click(function () {
                var elem_hide = new Array(4, 5);
                applicaFooTable('tabImpianti', elem_hide);
            });


            var gruppo;



            // Init strutture
            $('#div_coltura_annuale').hide();


            $('#ChkTerrenoNudo').change(function (e) {
                if ($('#ChkTerrenoNudo').is(':checked')) {
                    $('#chk_no').hide();
                    $('#chk_yes').show();
                }
                else {
                    $('#chk_no').show();
                    $('#chk_yes').hide();
                }
            });

            // Nascondo Linea Femmina
            $('#linea-femmina').hide();

            $('#ChkVarietaIbrida').change(function (e) {
                if ($('#ChkVarietaIbrida').is(':checked'))
                    $('#linea-femmina').show();
                else
                    $('#linea-femmina').hide();

            });

            $('#Txt_Interbina').attr("disabled", true);


            // Gestione icone Dati Aggiuntivi
            $('.accordion-toggle').click(function (e) {
                if ($(this).children("i").hasClass("fa-plus-circle"))
                    $(this).children("i").removeClass("fa-plus-circle").addClass("fa-minus-circle");
                else if ($(this).children("i").hasClass("fa-minus-circle"))
                    $(this).children("i").removeClass("fa-minus-circle").addClass("fa-plus-circle");
            });

            // Aumento altezza accordion per Cmb_DettaglioVarietaPersonalizzato
            $('.accordion-toggle').click(function (e) {
                if ($(this).children("i").hasClass("fa-plus-circle"))
                    $(this).children("i").removeClass("fa-plus-circle").addClass("fa-minus-circle");
                else if ($(this).children("i").hasClass("fa-minus-circle"))
                    $(this).children("i").removeClass("fa-minus-circle").addClass("fa-plus-circle");
            });
            
            // Carico la select Varieta' (on change Specie Vegetale) 
            $('#Cmb_Specie').change(function (e) {
                // Controllo se è stato immesso del testo nel input del Cerca
                if ($('#Cmb_Specie').val() != "") {
                    WaitFrame.show();
                    $('#Cmb_Cultivar').attr("disabled", false);
                    $('#Cmb_Finalita').attr("disabled", false);
                    $('#Cmb_TipologiaVarietale').attr("disabled", false);
                    $('#Cmb_FormaAllevamento').attr("disabled", false);
                    $('#Cmb_Portinnesto').attr("disabled", false);
                    $('#Cmb_SeminaTrapianto').attr("disabled", false);


                    // Gestione Gruppo Vegetale alla scelta della Specie (per mostrare Coltura Annuale !)
                    var specie = $('#Cmb_Specie').parent().children('div').children('div').children('.selectpicker').children('.selected').text();
                    gruppo = specie.split('---')[1];
                    gruppo = $.trim(gruppo);
                    gruppo = gruppo.substring(1, gruppo.length - 1);
                    //alert(gruppo);

                    if (gruppo !== "Arboree")
                        $('#div_coltura_annuale').show();
                    else
                        $('#div_coltura_annuale').hide();


                    var deferreds=new Array();

                    // Webservice per Varietà
                    deferreds.push(Carica_Select_Cultivar($('#Cmb_Specie').val(), function(r){
                        $('#Cmb_Cultivar').empty();
                        $('#Cmb_Cultivar').append(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));
                    

                    // Webservice per Finalità
                    deferreds.push(Carica_Select_Finalita($('#Cmb_Specie').val(), function(r){
                        $('#Cmb_Finalita').empty();
                        $('#Cmb_Finalita').html(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));

                    // Webservice per Tipologia Varietale
                    deferreds.push(Carica_Select_TipologiaVarietale($('#Cmb_Specie').val(), function(r){
                        $('#Cmb_TipologiaVarietale').empty();
                        $('#Cmb_TipologiaVarietale').html(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));

                    // Webservice per Allevamento
                    deferreds.push(Carica_Select_Allevamenti($('#Cmb_Specie').val(),function(r){
                        $('#Cmb_FormaAllevamento').empty();
                        $('#Cmb_FormaAllevamento').html(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));

                    // Webservice per Portinnesto
                    deferreds.push(Carica_Select_Portinnesto($('#Cmb_Specie').val(), function(r){
                        $('#Cmb_Portinnesto').empty();
                        $('#Cmb_Portinnesto').html(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));

                    // Webservice per Copertura (dipende dalla Specie vegetale)
                    var par;
                    switch (gruppo) {
                        case "Arboree":
                            par = 1;
                            break;
                        case "Erbacee":
                            par = 2;
                            break;
                        case "Orto-floro vivaismo":
                            par = 3;
                            break;
                    }

                    deferreds.push(Carica_Select_Copertura(par, function(r){
                        $('#Cmb_Copertura').empty();
                        $('#Cmb_Copertura').html(r.d);
                        $('.selectpicker').selectpicker('refresh');
                    }));


                    if (deferreds.length>0){
                        $.when.apply($, deferreds).then(function () {
                            WaitFrame.hide();
                        }).fail(function () {
                            WaitFrame.hide();
                        });
                    }else{
                        WaitFrame.hide();
                    }

                    // Modifico la visibilità della select SEMINA/TRAPIANTO (attiva solo su Barbabietola)
                    if ($('#Cmb_Specie').val() == 6)
                        $('#Cmb_SeminaTrapianto').attr('disabled', false);



                }


            });


            $('#TxtValiditaInizio').change(function (e) {
                if ($('#ChkColturaAnnuale').is(':checked'))
                    $('#Txt_ValiditaInizio_Distinta').val($('#TxtValiditaInizio').val());

            });

            $('#TxtValiditaFine').datepicker().on('changeDate', function (ev) {
                if ($('#ChkColturaAnnuale').is(':checked'))
                    $('#Txt_ValiditaInizio_Distinta').val($('#TxtValiditaInizio').val());
            });


            $('#TxtValiditaFine').change(function (e) {
                if ($('#ChkColturaAnnuale').is(':checked'))
                    $('#Txt_ValiditaFine_Distinta').val($('#TxtValiditaFine').val());

            });

            $('#TxtValiditaFine').datepicker().on('changeDate', function (ev) {
                if ($('#ChkColturaAnnuale').is(':checked'))
                    $('#Txt_ValiditaFine_Distinta').val($('#TxtValiditaFine').val());
            });




            //... Controllo se invece cambia valore
            $('#Cmb_OrganismoReferente').change(function (e) {
                if ($('#Cmb_OrganismoReferente').val() != "") {
                    riempi_MagazzinoConferimento($('#Cmb_OrganismoReferente').val(), "");

                } else
                    $('#Cmb_MagazzinoConferimento').empty();

            });



            $('#ChkFilaBinata').click(function (e) {
                if (!$('#ChkFilaBinata').is(':checked'))
                    $('#Txt_Interbina').attr("disabled", true);
                else
                    $('#Txt_Interbina').attr("disabled", false);
            });

            //... se cambia il check Impianto consociato
            $('#ChkConsociazione').change(function (e) {
                if ($('#ChkConsociazione').is(':checked')) {
                    $.ajax({
                        type: 'POST',
                        url: 'Impianto_Edit.aspx/Carica_Select_Regolamento',
                        data: "{parametro:'biologico'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            //alert(r.d);
                            $('#Cmb_Regolamento').empty();
                            $('#Cmb_Regolamento').html(r.d);
                            $('.selectpicker').selectpicker('refresh');
                        }
                    });
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: 'Impianto_Edit.aspx/Carica_Select_Regolamento',
                        data: "{parametro:''}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            //alert(r.d);
                            $('#Cmb_Regolamento').empty();
                            $('#Cmb_Regolamento').html(r.d);
                            $('.selectpicker').selectpicker('refresh');
                        }
                    });
                }
            });

            // Calcola piante
            //$('#btn_piante_calcola').click(function (e) {
            $('input.impianto_input').keyup(function () {

                var dist_su = $('#Txt_DistanzaSuFila_M').val();
                var dist_tra = $('#Txt_DistanzaTraFila_M').val();
                var interb = $('#Txt_Interbina').val();
                var germin = $('#Txt_Germinabilita').val();
                var flag = True;

                // Controllo se i campi richiesti sono stati riempiti
                if(dist_su == "") {
                    $('#Txt_DistanzaSuFila_M').parent().append('<label id="Txt_DistanzaSuFila_M-error" class="custom_val error" for="Txt_DistanzaSuFila_M">Riempire il campo per il calcolo</label>');
                    $('#Txt_DistanzaSuFila_M').parent().children(".required").css('border', '1px solid #D41E1A');
                    flag = False;
                }

                if(dist_tra == "") {
                    $('#Txt_DistanzaTraFila_M').parent().append('<label id="Txt_DistanzaTraFila_M-error" class="custom_val error" for="Txt_DistanzaTraFila_M">Riempire il campo per il calcolo</label>');
                    $('#Txt_DistanzaTraFila_M').parent().children(".required").css('border', '1px solid #D41E1A');
                    flag = False;
                }

                if($('#ChkFilaBinata').is(':checked')){
                    if(interb == "") {
                        $('#Txt_Interbina').parent().append('<label id="Txt_Interbina-error" class="custom_val error" for="Txt_Interbina">Riempire il campo per il calcolo</label>');
                        $('#Txt_Interbina').parent().children(".required").css('border', '1px solid #D41E1A');
                        flag = False;
                    }
                }

                if(germin == "") {
                    $('#Txt_Germinabilita').parent().append('<label id="Txt_Germinabilita-error" class="custom_val error" for="Txt_Germinabilita">Riempire il campo per il calcolo</label>');
                    $('#Txt_Germinabilita').parent().children(".required").css('border', '1px solid #D41E1A');
                    flag = False;
                }
                ////

                if(flag) {
                    var denominatore;

                    If($('#ChkFilaBinata').is(':checked'))
                    denominatore = dist_su * dist_tra;
                    Else
                    denominatore = (interb / 2) * dist_su;

                    var PianteHa = parseInt(10000 / denominatore * (germin / 100));

                    var superficie = $('#TxtSuperficie').val().replace(/\,/g, '.');;

                    var PianteImpianto = parseInt(PianteHa * superficie);

                    $('#TxtPianteHa').val(PianteHa);
                    $('#TxtPianteImpianto').val(PianteImpianto);
                }
                else{
                    $('#TxtPianteHa').val('');
                    $('#TxtPianteImpianto').val('');
                }


            });


            // Click su NUOVA DISTINTA  
            $('#btn_nuova_distinta').click(function () {

                // Setto la modalità di operazione
                mode_op = "scrittura";

                //Apro il div della scheda distinta
                $(".scheda_distinta").show();

                //Svuoto la selezione precedente sulla tabella...
                $("#tabImpianti .watable tbody tr").each(function () {
                    $(this).children('td, th').css("background-color", "");
                    $(this).children('td, th').css("color", "");
                });

                //... e svuoto anche dalla session... e salvo il tipo di Operazione (Scrittura)
                $.ajax({
                    type: 'POST',
                    url: 'Impianto_Edit.aspx/Svuota_Distinta',
                    data: "{}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function(r) {
                    }
                });

                //... e svuoto i campi
                $(".scheda_distinta").find("input").each(function () {
                    $(this).val("");
                });


                // Mostro i bottoni salva Codici e Particelle
                $('#btn_Aggiungi_Codice').show();
                $('#btn_Aggiungi_Particella').show();
                $('#CmbCodice').attr("disabled", false);
                $('#CmbCodice').removeClass("disabled");
                $('#CmbCodice').selectpicker('refresh');

                $('#TxtCodiceValore').attr("disabled", false);

                $('#Cmb_Particella').attr("disabled", false);
                $('#Cmb_Particella').removeClass("disabled");
                $('#Cmb_Particella').selectpicker('refresh');

                $('#TxtParticellaValore').attr("disabled", false);

            });


            ////On click sulle righe watable - MODIFICA DISTINTA
            //$('#tabImpianti .watable tbody tr td .watable-col-Tool').click(function () {
            //    WaitFrame.show();
            //    // Setto la modalità di operazione
            //    mode_op = "modifica";

            //    //Apro il div della scheda distinta
            //    $(".scheda_distinta").show();

            //    $("#tabImpianti .watable tbody tr").each(function () {
            //        $(this).children('td, th').css("background-color", "");
            //        $(this).children('td, th').css("color", "#000");
            //    });

            //    // attivo la riga
            //    InfoProgetti($(this).parent().children('.watable-col-Tool').children('div').children('span').first());


            //    //                $(this).children('td, th').css("background-color", "#428bca");
            //    //                $(this).children('td, th').css("color", "#fff");

            //    $(this).parent().find('td').each(function () {
            //        $(this).css("background-color", "#428bca");
            //        $(this).css("color", "#fff");
            //    });

            //    // Disabilito i pulsanti del salvataggio Codice e Particella
            //    $('#btn_Aggiungi_Codice').hide();
            //    $('#btn_Aggiungi_Particella').hide();
            //    $('#CmbCodice').attr("disabled", true);
            //    $('#TxtParticellaValore').attr("disabled", true);
            //    $('#Cmb_Particella').attr("disabled", true);
            //    $('#TxtParticellaValore').attr("disabled", true);
            //    WaitFrame.hide();

            //});


            // CLick per ELIMINARE una distinta
            $("body").on("click", ".del_elem", function () {

                var streelemento = "Distinta: <b>" + $(this).parent().parent().parent().find('td:nth-child(3)').text() + "</b>";
                var xChiave = $(this).attr('chiave');

                ConfermaDelete(streelemento, xChiave);
            });

            //On change stato_group
            //            $('.stato_group').change(function(){
            //                if(($('#<=Cmb_Specie.ClientID %>').val() != "")&&($('#<=Cmb_Finalita.ClientID %>').val() != "")&&($('#<=Cmb_RegolamentoConc.ClientID %>').val() != ""))
            //                {
            //                    var specie = $('#<=Cmb_Specie.ClientID %>').val();
            //                    var finalita = $('#<=Cmb_Finalita.ClientID %>').val();
            //                    var regolamento = $('#<=Cmb_RegolamentoConc.ClientID %>').val();

            //                    $.ajax({
            //                        type: 'POST',
            //                        url: 'Impianto_Edit.aspx/Carica_Select_StatoImpianto',
            //                        data: "{specie:'"+specie+"', finalita:'"+finalita+"', regolamento:'"+regolamento+"'}",
            //                        contentType: 'application/json; charset=utf-8',
            //                        cache: false,
            //                        dataType: 'json', async: true,
            //                        success: function (r) {
            //                            //alert(r.d);
            //                            $('#<=Cmb_Stato.ClientID %>').empty();
            //                            $('#<=Cmb_Stato.ClientID %>').html(r.d);
            //                            $('.selectpicker').selectpicker('refresh');
            //                        }
            //                    });

            //                }
            //            });



            // Gestione click del bottone Aggiungi in Codici
            $('#btn_Aggiungi_Codice').click(function (e) {

                var codice = $('#div_CmbCodice > div > button').text();
                var valore = $('#TxtCodiceValore').val();
                var DataInizio = "";
                var DataFine = "";

                // Controllo se è stato immesso del testo nel input del Cerca
                if ((codice != "") && (valore != "")) {
                    codice = codice.slice(0, -1);

                    //                    DataInizio = $('#<=TxtValiditaInizioCodice.ClientID%>').val();
                    //                    DataFine = $('#<=TxtValiditaFineCodice.ClientID%>').val();

                    $("#CmbCodice option").each(function () {
                        if (this.text == codice) {
                            //alert(this.value);
                            codice_id = this.value;
                        }
                    });

                    $.ajax({
                        type: 'POST',
                        url: 'Impianto_Edit.aspx/Aggiungi_Codice',
                        data: "{codice:'" + codice + "', valore:'" + valore + "', codice_id:'" + codice_id + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            if (r.d.RispostaOK == true)
                                AggiornaTabCodici(JSON.parse(r.d.RispostaStringa));
                            else
                                alert(r.d.Errore);
                        }
                    });
                }
            });



            // Gestione click del bottone Aggiungi in Codici
            $('#btn_Aggiungi_Particella').click(function (e) {

                var nome = $('#Cmb_Particella').text().replace(' ', '');
                var valore = $('#TxtParticellaValore').val();
                var DataInizio = "";
                var DataFine = "";

                // Controllo se è stato immesso del testo nel input del Cerca
                if ((nome != "") && (valore != "")) {
                    nome = nome.slice(0, -1);

                    //                    DataInizio = $('#<=TxtValiditaInizioCodice.ClientID%>').val();
                    //                    DataFine = $('#<=TxtValiditaFineCodice.ClientID%>').val();

                    $("#Cmb_Particella option").each(function () {
                        if (this.text == nome) {
                            //alert(this.value);
                            codice_id = this.value;
                        }
                    });

                    $.ajax({
                        type: 'POST',
                        url: 'Impianto_Edit.aspx/Aggiungi_Particella',
                        data: "{nome:'" + nome + "', valore:'" + valore + "', codice_id:'" + codice_id + "'}",
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'json', async: true,
                        success: function (r) {
                            if (r.d.RispostaOK == true)
                                AggiornaTabParticelle(JSON.parse(r.d.RispostaStringa));
                            else
                                alert(r.d.Errore);
                        }
                    });
                }
            });



            var fl1 = false
            var fl2 = false
            var fl3 = false
            var fl4 = false
            var fl5 = false


            nascondi_riepilogo_error();

            // Gestione Riepilogo Errori (Validazione)
            $("#Cmb_Specie").change(function () {
                if ($("#Cmb_Specie").val() != "") {
                    $('.voce_1').hide();
                    fl1 = true;
                }
                else {
                    $('.voce_1').show();
                    fl1 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#Cmb_Finalita").change(function () {
                if ($("#Cmb_Finalita").val() != "") {
                    $('.voce_2').hide();
                    fl2 = true;
                }
                else {
                    $('.voce_2').show();
                    fl2 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#Cmb_Cultivar").change(function () {
                if ($("#Cmb_Cultivar").val() != "") {
                    $('.voce_3').hide();
                    fl3 = true;
                }
                else {
                    $('.voce_3').show();
                    fl3 = false;
                }

                nascondi_riepilogo_error();
            });

            $("#TxtValiditaInizio").keyup(function () {
                if ($("#TxtValiditaInizio").val() != "") {
                    $('.voce_4').hide();
                    fl4 = true;
                }
                else {
                    $('.voce_4').show();
                    fl4 = false;
                }

                nascondi_riepilogo_error();
            });

            $('#TxtValiditaInizio').datepicker()
                .on('changeDate', function (e) {
                    $('.voce_4').hide();
                    fl4 = true;

                    nascondi_riepilogo_error();

                });

            $("#TxtValiditaFine").keyup(function () {
                if ($("#TxtValiditaFine").val() != "") {
                    $('.voce_5').hide();
                    fl5 = true;
                }
                else {
                    $('.voce_5').show();
                    fl5 = false;
                }

                nascondi_riepilogo_error();
            });

            $('#TxtValiditaFine').datepicker()
                .on('changeDate', function (e) {
                    $('.voce_5').hide();
                    fl5 = true;

                    nascondi_riepilogo_error();

                });


            function nascondi_riepilogo_error() {
                if (fl1)
                    $('.voce_1').hide();

                if (fl2)
                    $('.voce_2').hide();

                if (fl3)
                    $('.voce_3').hide();

                if (fl4)
                    $('.voce_4').hide();

                if (fl5)
                    $('.voce_5').hide();


                if ((fl1) && (fl2) && (fl3) && (fl4) && (fl5))
                    $('#div_riepilogo_error').hide();
                else
                    $('#div_riepilogo_error').show();
            }


            /////////////////////
    
            //PARTE DI CARICAMENTO ONLOAD NELL'aspx'

            //PRIMA ERA NEL FILE A PARTE



            var initImpianti;
            var initCodici;
            var initParticelle;

            //carico eventualmente la tabella di Codici
            <% if (jsCodici <> "") then %>
                initCodici = <%=jsCodici %>;
            <% else%>
                initCodici = "";
            <% end if %>

            //carico eventualmente la tabella delle Particelle
            <% if(jsParticelle <>"" ) then %>
                initParticelle = <%=jsParticelle %>;
            <% else%>
                initParticelle = "";
            <% end if %>

            //carico eventualmente la tabella di Impianti
            <% if(jsImpianti <> "") Then %> 
                initImpianti = <%=jsImpianti %>;
            <% Else%>
                initImpianti = "";
            <% end if %>

            var dImpianti = $.Deferred();
            var dCodici = $.Deferred();
            var dParticelle = $.Deferred();
            var dRegolamento = $.Deferred();
            var dProgetto = $.Deferred();
            $.when(dImpianti, dCodici, dParticelle, dRegolamento, dProgetto).done(function () {
                WaitFrame.hide();
                $('#wait').hide();
            }).fail(function() {
                WaitFrame.hide();
            })

            if (initImpianti != "") {
                AggiornaTabImpianti(initImpianti,dImpianti);
            } else{
                dImpianti.resolve();
            }

            if (initCodici != "") {
                AggiornaTabCodici(initCodici, dCodici);
            }else{
                dCodici.resolve();
            }

            if (initParticelle != "") {
                AggiornaTabParticelle(initParticelle, dParticelle);
            }else{
                dParticelle.resolve();
            }

            CaricaRegolamento(function(r){
                $('#Cmb_Regolamento').empty();
                $('#Cmb_Regolamento').html(r.d);
                $('.selectpicker').selectpicker('refresh');
            }, dRegolamento);


    //CARICO LA PRIMA DISTINTA IN ALTO (l'ultima in ordine temporale)
            if ($('#tabImpianti tbody tr').length > 0) {
                progetto_cod = $($('#tabImpianti tbody tr')[0]).find('span.info_elem').attr('chiave');
                if ($.isNumeric(progetto_cod)){
                    caricaProgetto(progetto_cod, dProgetto);
                }else{
                    dProgetto.resolve();
                }
            }else{
                dProgetto.resolve();
            }

        });

        function CaricaRegolamento(callback, deferred){
            // Carico il select Regolamento...
            // Webservice per Regolamento
            $.ajax({
                type: 'POST',
                url: 'Impianto_Edit.aspx/Carica_Select_Regolamento',
                data: "{parametro:''}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    callback(r);
                    deferred.resolve();
                },
                error: function(){
                    deferred.reject();
                }
            });
        }




    </script>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
