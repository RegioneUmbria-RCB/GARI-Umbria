<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master"
    CodeBehind="PianoNutrizionale_IBF.aspx.vb" Inherits="PianoConcimazione_2017.PianoNutrizionale_IBF" %>



<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>
        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, PianoNutrizionaleBilancio %>" runat="server">Piano Nutrizionale a Bilancio</asp:Localize>
    </title>

    <style>
        .k-list-item-text {
            width: 100%
        }
    </style>
    <script type="text/javascript">

        var Qs_Piva;
        var Qs_Operazione;
        var Qs_Tipo;
        var QS_AnalisiNG;

        var hdPiva = "#<%=hd_Piva.ClientID %>";
        var hdOperazione = "#<%=hd_Operazione.ClientID %>";
        var hdTipo = "#<%=hd_Tipo.ClientID %>";
        var hdAnalisiNG = "#<%=hd_usaAnalisiModelloNG.ClientID %>"; 

        var ddlRegolamento = "#<%=ddlRegolamento.ClientID %>"
        var ddlMasS = "#<%=ddlMasS.ClientID %>"
        var ddlMasB = "#<%=ddlMasB.ClientID %>"

        var DDL_K2O = "#<%=DDL_K2O.ClientID %>"
        var DDL_P2O5 = "#<%=DDL_P2O5.ClientID %>"

        var ddlDoseK = "#<%=ddlDoseK.ClientID %>"
        var ddlDoseP = "#<%=ddlDoseP.ClientID %>"
        var ddlDoseP = "#<%=ddlDoseP.ClientID %>"

        var Txt_Descrizione = '#<%=Txt_Descrizione.ClientId %>'
        var Txt_ValiditaInizio = '#<%=Txt_ValiditaInizio.ClientId %>'
        var Txt_ValiditaFine = '#<%=Txt_ValiditaFine.ClientId %>'
        var Txt_Anno = '#<%=Txt_Anno.ClientId %>'

        var Txt_TotDecrementi = '#<%=Txt_TotDecrementi.ClientId %>'
        var Txt_TotDecrementiP = '#<%=Txt_TotDecrementiP.ClientId %>'
        var Txt_TotDecrementiK = '#<%=Txt_TotDecrementiK.ClientId %>'

        var Txt_TotIncrementi = '#<%=Txt_TotIncrementi.ClientId %>'
        var Txt_TotIncrementiP = '#<%=Txt_TotIncrementiP.ClientId %>'
        var Txt_TotIncrementiK = '#<%=Txt_TotIncrementiK.ClientId %>'

        var Txt_MaxIncrementi = '#<%=Txt_MaxIncrementi.ClientId %>'

        var Txt_DoseStandard = '#<%=Txt_DoseStandard.ClientId %>'
        var Txt_DoseStandardP = '#<%=Txt_DoseStandardP.ClientId %>'
        var Txt_DoseStandardK = '#<%=Txt_DoseStandardK.ClientId %>'

        var Txt_DoseRicalcolata = '#<%=Txt_DoseRicalcolata.ClientId %>'
        var Txt_DoseRicalcolataP = '#<%=Txt_DoseRicalcolataP.ClientId %>'
        var Txt_DoseRicalcolataK = '#<%=Txt_DoseRicalcolataK.ClientId %>'

        var N_Ammesso = '#<%=N_Ammesso.ClientId %>'
        var P_Ammesso = '#<%=P_Ammesso.ClientId %>'
        var K_Ammesso = '#<%=K_Ammesso.ClientId %>'

        var Txt_Pioggia = '#<%=Txt_Pioggia.ClientId %>'

        var Meteo_ChkAgenda = '#<%=Meteo_ChkAgenda.ClientId %>'
        var Meteo_Sorgente = '#<%=Meteo_Sorgente.ClientId %>'
        var Meteo_TipoSorgente_Real = '#<%=Meteo_TipoSorgente_Real.ClientId %>'
        var Meteo_TipoSorgente = '#<%=Meteo_TipoSorgente.ClientId %>'

        var Cmb_Centro = '#<%=Cmb_Centro.ClientId %>'

        var Btn_Bilancio = '#<%=Btn_Bilancio.ClientId %>'

        var Txt_K = '#<%=Txt_K.ClientId %>'
        var Txt_P = '#<%=Txt_P.ClientId %>'

        var cmb_PeriodoSeminaColturaPrincipale = '#<%=cmb_PeriodoSeminaColturaPrincipale.ClientId %>'
        var cmb_PeriodoRaccoltaColturaPrincipale = '#<%=cmb_PeriodoRaccoltaColturaPrincipale.ClientId %>'
        var cmb_PeriodoInterramentoResiduiPrecessione = '#<%=cmb_PeriodoInterramentoResiduiPrecessione.ClientId %>'
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField id="hd_Piva" runat="server" />
    <asp:HiddenField id="hd_Operazione" runat="server" />
    <asp:HiddenField id="hd_Tipo" runat="server" />
    <asp:HiddenField ID="hd_usaAnalisiModelloNG" runat="server" />
    <div class="container">
        <!-- TESTATA -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-testata gias-mt-1">
            <div class="panel-heading gias-section-title">
                <h4 class="panel-title"></h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Regolamento %>" runat="server">Regolamento</asp:Localize>
                                        </span>
                                        <asp:DropDownList ID="ddlRegolamento" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Descrizione %>" runat="server">Descrizione</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="Txt_Descrizione" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-lg-3 col-md-3 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ValiditaInizio %>" runat="server">Validita Inizio</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-lg-3 col-md-3 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ValiditaFine %>" runat="server">Validita Fine</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-lg-2 col-md-2 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Anno %>" runat="server">Anno</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="Txt_Anno" runat="server" CssClass="form-control "></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-8 col-md-6 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Note %>" runat="server">Note</asp:Localize>
                                        </span>
                                        <asp:TextBox ID="Txt_Note" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-6 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <% If Master.Master_versione <> "Agronica" Then %>
                                    <span class="input-group-addon alert-info gias-check-container-no-border">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, DichiarazioneNonUtilizzoFertilizzanti %>" runat="server">Dichiarazione di Non Utilizzo Fertilizzanti</asp:Localize>
                                    </span>
                                    <% End If %>
                                    <div class="input-group gias-asp-check switch">
                                        <% If Master.Master_versione = "Agronica" Then %>
                                        <span class="input-group-addon alert-info gias-check-container-no-border">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, DichiarazioneNonUtilizzoFertilizzanti %>" runat="server">Dichiarazione di Non Utilizzo Fertilizzanti</asp:Localize>
                                        </span>
                                        <span class="form-control" style="display: inline-block; width: 30px;">
                                            <% End If %>
                                            <asp:CheckBox ID="Chk_NonUtilizzo_Fertilizzanti" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                            <% If Master.Master_versione <> "Agronica" Then %>
                                            <label for="Chk_NonUtilizzo_Fertilizzanti" class="switch-label">Switch</label>
                                            <% Else %>
                                        </span>
                                        <% End If %>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- COLTURA -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-coltura">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b style="text-transform: uppercase">
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, DatiColturaUbicazione %>" runat="server">DATI COLTURA E UBICAZIONE</asp:Localize>
                    </b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CentroAziendale %>" runat="server">Centro Aziendale</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="Cmb_Centro" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="input-group">
                                        <asp:RadioButtonList ID="RBL_Specie" runat="server" Style="float: left;" Font-Size="10px"
                                            CellPadding="0" CellSpacing="0" RepeatDirection="Horizontal" AutoPostBack="true">
                                            <asp:ListItem Value="0" Selected="True" meta:resourcekey="VisualizzaSpecieVegetaliPrevistePianoConcimazione">
                                            </asp:ListItem>
                                            <asp:ListItem Value="1" meta:resourcekey="VisualizzaSpecieVegetaliPresentiPianoColturale">
                                            </asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-6 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ColturaPrincipale %>" runat="server">Coltura Principale</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="Cmb_Specie_ColturaPrincipale" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-4 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Finalita %>" runat="server">Finalità</asp:Localize>
                                                </span>
                                                <asp:DropDownList ID="ddlFinalitaRer_ColturaPrincipale" runat="server" CssClass="form-control selectpicker required stato_group"
                                                    data-live-search="true" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>                                
                            </div>
                        </div>

                        <div class="col-md-12 nopadding">
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" style="text-transform: uppercase">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ResaDichiarataTHa %>" runat="server">RESA DICHIARATA [t/Ha]</asp:Localize>
                                                </span>
                                                <asp:TextBox ID="Txt_Resa" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-12">
                                    <div class="form-group">
                                        <span class="col-form-label" style="text-transform: uppercase">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ResaRiferimentoTHa %>" runat="server">RESA RIFERIMENTO [T/HA]</asp:Localize>
                                        </span>
                                        <label id="Lbl_ResaRiferimento" runat="server" style="font-weight: bold; margin-left: 10PX"></label>
                                    </div>
                                </div>
                                
                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, PeriodoSemina %>" runat="server">Periodo semina</asp:Localize>
                                                </span>
                                                <input type="text" name="ddlPeriodoSeminaColturaPrincipale" id="ddlPeriodoSeminaColturaPrincipale" value="" style="width: -webkit-fill-available;" />
                                                <input type="hidden" id="cmb_PeriodoSeminaColturaPrincipale" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-6 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, PeriodoRaccolta %>" runat="server">Periodo raccolta</asp:Localize>                                                    
                                                </span>
                                                <input type="text" name="ddlPeriodoRaccoltaColturaPrincipale" id="ddlPeriodoRaccoltaColturaPrincipale" value="" style="width: -webkit-fill-available;" />
                                                <input type="hidden" id="cmb_PeriodoRaccoltaColturaPrincipale" runat="server" />
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

        <!-- SUOLO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-suolo">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title" style="text-transform: uppercase">
                    <b>
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CaratteristicheSuolo %>" runat="server">CARATTERISTICHE SUOLO</asp:Localize>
                    </b>
                </h4>
            </div>

            <div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-lg-9 col-md-9 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">
                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Analisi %>" runat="server">Analisi</asp:Localize>
                                        </span>
                                        <asp:DropDownList ID="ddlAnalisi" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-3 col-xs-12" style="display:flex;align-items:center">
                            <asp:Button ID="BtnAnalisi" runat="server" Style="width: 100%" class="btn btn-info" meta:resourcekey="Analisi" Text="ANALISI" />
                            <asp:Button ID="Btn_hidden_CaricaAnalisi" ClientIDMode="Static" runat="server" CssClass="hidden" />
                            <asp:Button ID="BtnVisualizza" runat="server" Style="width: 100%" class="btn btn-info" />
                            <div style="width:5px;height:auto;display:inline-block"></div>
                            <asp:Button ID="BtnRicerca" runat="server" Style="width: 100%" class="btn btn-info" Text="<%$ Resources: PianoConcimazione_2017, GestioneAnalisi %>" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SabbiaPerc %>" runat="server">Sabbia [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_Sabbia" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ArgillaPerc %>" runat="server">Argilla [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_Argilla" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, LimoPerc %>" runat="server">Limo [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_Limo" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, pH %>" runat="server">pH</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_PH" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CalcTotPerc %>" runat="server">Calc.Tot. [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_CalcTot" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CalcAttPerc %>" runat="server">Calc.Att. [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_CalcAtt" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SOPerc %>" runat="server">S.O. [%]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_SO" runat="server" CssClass="form-control">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CN %>" runat="server">C/N</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_CN" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Ngkg %>" runat="server">N [g/kg]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_N" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:DropDownList ID="DDL_P2O5" runat="server" Style="border: 0px; background-color: #d9edf7">
                                                    <asp:ListItem Value="0" meta:resourcekey="P2O5ppm"></asp:ListItem>
                                                    <asp:ListItem Value="1" meta:resourcekey="Pppm"></asp:ListItem>
                                                </asp:DropDownList>
                                            </span>
                                            <asp:TextBox ID="Txt_P" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:DropDownList ID="DDL_K2O" runat="server" Style="border: 0px; background-color: #d9edf7">
                                                    <asp:ListItem Value="0" meta:resourcekey="K2Oppm"></asp:ListItem>
                                                    <asp:ListItem Value="1" meta:resourcekey="Kppm"></asp:ListItem>
                                                </asp:DropDownList>
                                            </span>
                                            <asp:TextBox ID="Txt_K" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MgOppm %>" runat="server">MgO [ppm]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_Mg" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CSCmeq100g %>" runat="server">C.S.C. [meq/100 g]</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_CSC" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                             <div class="col-lg-5 col-md-3 col-sm-12">
                             </div>
                            
                            <div class="col-lg-4 col-md-3 col-sm-12" id="Div_SalvaAnalisi" runat="server">
                                <asp:Button ID="Btn_SalvaAnalisi" runat="server" Style="width: 100%; text-align: center; font-size: small;" meta:resourcekey="SalvaNuovaAnalisi" class="btn btn-warning" />
                                <br />
                                <label style="font-size: xx-small">
                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SeSelezionatoCentroAziendaleAnalisiAssociataCentroAltrimentiAzienda %>" runat="server">(*) Se è stato selezionato un Centro Aziendale l'analisi verrà associata a tale Centro, altrimenti verrà associata all'intera Azienda</asp:Localize>
                                </label>
                                <br />
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info">
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, DescrizioneAnalisi %>" runat="server">Descrizione Analisi</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="Txt_AnalisiDes" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- PRATICHE AGRONIMICHE -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-pratiche-agronomiche">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title" style="text-transform: uppercase">
                    <b>
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, PraticheAgronomichePrecessione %>" runat="server">PRATICHE AGRONOMICHE PRECESSIONE</asp:Localize>
                    </b>
                </h4>
            </div>
            
            <div class="panel-body">
                <div class="row">

                    <div class="col-lg-3 col-md-4 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Coltura %>" runat="server">Coltura</asp:Localize>
                                    </span>
                                    <asp:DropDownList ID="Cmb_Specie_Precessione" runat="server" CssClass="form-control selectpicker required stato_group"
                                        data-live-search="true" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                     
                    <div class="col-lg-3 col-md-4 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Finalita %>" runat="server">Finalità</asp:Localize>
                                    </span>
                                    <asp:DropDownList ID="ddlFinalitaRer_Precessione" runat="server" CssClass="form-control selectpicker required stato_group"
                                        data-live-search="true" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                        
                    <div class="col-lg-2 col-md-2 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <% If Master.Master_versione <> "Agronica" Then %>
                                <span class="input-group-addon alert-info gias-check-container-no-border">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ResiduiAsportati %>" runat="server">Residui Asportati</asp:Localize>
                                </span>
                                <% End If %>
                                <div class="input-group gias-asp-check switch">
                                    <% If Master.Master_versione = "Agronica" Then %>
                                    <span class="input-group-addon alert-info gias-check-container-no-border">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ResiduiAsportati %>" runat="server">Residui Asportati</asp:Localize>
                                    </span>
                                    <span class="form-control" style="display: inline-block; width: 30px;">
                                        <% End If %>
                                        <asp:CheckBox ID="Chk_ResiduiPrecessione" runat="server" ClientIDMode="Static"></asp:CheckBox>
                                        <% If Master.Master_versione <> "Agronica" Then %>
                                        <label for="Chk_ResiduiPrecessione" class="switch-label">
                                            Switch
                                        </label>
                                        <% Else %>
                                    </span>
                                    <% End If %>
                                </div>
                            </div>
                        </div>
                    </div>
                        
                    <div class="col-lg-4 col-md-6 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, PeriodoInterramentoResidui %>" runat="server">Periodo interramento residui</asp:Localize>                                            
                                    </span>
                                    <input type="text" name="ddlPeriodoInterramentoResiduiPrecessione" id="ddlPeriodoInterramentoResiduiPrecessione" value="" style="width: -webkit-fill-available;" />
                                    <input type="hidden" id="cmb_PeriodoInterramentoResiduiPrecessione" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">                            
                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" style="text-transform: uppercase">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ResaStoricaPrecessioneTHA %>" runat="server">RESA STORICA PRECESSIONE [t/Ha]</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="Txt_ResaStoricaPrecessione" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-3 col-md-5 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ConcimeOrganico %>" runat="server">Concime Organico</asp:Localize>
                                    </span>
                                    <asp:DropDownList ID="ddlConcimeOrganico" runat="server" CssClass="form-control selectpicker required stato_group"
                                        data-live-search="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-2 col-md-2 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, QtaNKgHa %>" runat="server">Qta N [Kg/Ha]</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="Txt_QtaN_KGHa" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>             
                        
                    <div class="col-lg-4 col-md-5 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, EpocaModalitaDistribuzione %>" runat="server">Epoca e Modalità Distribuzione</asp:Localize>
                                    </span>
                                    <asp:DropDownList ID="ddlEpocaModalitaDistribuzione" runat="server" CssClass="form-control selectpicker required stato_group"
                                        data-live-search="true" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- METEO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-meteo">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title" style="text-transform: uppercase">
                    <b>
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Meteo %>" runat="server">METEO</asp:Localize>
                    </b>
                </h4>
            </div>
            <div>
                <input type="hidden" id="Meteo_ChkAgenda" runat="server" />
                <input type="hidden" id="Meteo_TipoSorgente" runat="server" />
                <input type="hidden" id="Meteo_TipoSorgente_Real" runat="server" />
                <input type="hidden" id="Meteo_Sorgente" runat="server" />
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Sorgente" for="TxtSorgente">
                                    <asp:Localize meta:resourcekey="CategoriaSorgenteDati" runat="server">Categoria Sorgente Dati</asp:Localize>
                                </label>
                                <input type="text" name="cmbTipoSorgente" id="cmbTipoSorgente" value="" style="width: -webkit-fill-available;" />
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group calc-width">
                                <label class="input-group-addon alert-info" id="lbl_Origine" for="TxtTipologiaDti">
                                    <asp:Localize meta:resourcekey="OrigineDatiMeteo" runat="server">Origine Dati Meteo</asp:Localize>
                                </label>
                                <input type="text" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />
                                <%--<input type="text" class="form-control" name="cmbOrigineDati" id="cmbOrigineDati" value="" style="width: -webkit-fill-available;" />--%>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <input type="checkbox" id="opRegGiasChk" class="k-checkbox" data-bind="checked: checkboxChecked, events: { change: clickHandler }">
                            <label class="k-checkbox-label" for="opRegGiasChk">
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, LeggiDatiOperazioniRegistrateGIAS %>" runat="server">Leggi i dati delle operazioni registrate</asp:Localize>
                            </label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-3 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_data_fine_piogge" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="Precipitazioni0110_2802"></asp:Label>
                                <asp:TextBox ID="Txt_Pioggia" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                         <div class="col-lg-5 col-md-4 col-sm-12">
                         </div>

                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <button id="Btn_Meteo" class="btn btn-info" style="width: 100%;" onclick="return CaricaPiogge()">
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CaricaPiogge %>" runat="server">Carica Piogge</asp:Localize>
                            </button>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-3 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_AvgTemperatura_ColturaInCampo" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="TemperaturaMediaColturaInCampo"></asp:Label>
                                <asp:TextBox ID="Txt_AvgTemperatura_ColturaInCampo" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_AvgTemperatura_MeseSemina_Febbraio" runat="server" CssClass="input-group-addon alert-info" meta:resourcekey="TemperaturaMediaMeseSemina_2802"></asp:Label>
                                <asp:TextBox ID="Txt_AvgTemperatura_MeseSemina_Febbraio" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_Txt_PercUmiditaColturaPrincipale" runat="server" CssClass="input-group-addon alert-info" >% Umidità prevista coltura principale</asp:Label>
                                <asp:TextBox ID="Txt_PercUmiditaColturaPrincipale" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label_Txt_PercUmiditaRaccoltaPrecessione" runat="server" CssClass="input-group-addon alert-info" >% Umidità alla raccolta precessione</asp:Label>
                                <asp:TextBox ID="Txt_PercUmiditaRaccoltaPrecessione" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BOTTONE VISUALIZZA BILANCIO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-bottone-bilancio" id="Div_BtnVisualizza" runat="server" style="background-color: none">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title">
                    <b></b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-info btn_100" style="text-transform: uppercase" onclick="$('#<%=Btn_Bilancio.ClientID %>').click();">
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, CalcolaBilancio %>" runat="server">CALCOLA BILANCIO</asp:Localize>
                            </div>
                            <asp:Button ID="Btn_Bilancio" runat="server" Style="display: none" />

                            <input type="hidden" id="Testata_Cod" runat="server" />

                            <input type="hidden" id="N_Ammesso" runat="server" />
                            <input type="hidden" id="K_Ammesso" runat="server" />
                            <input type="hidden" id="P_Ammesso" runat="server" />

                            <!--  <input type="hidden" id="N_MAS" runat="server" /> -->
                            <input type="hidden" id="Fattore_Correttivo_N_Resa" runat="server" />

                            <input type="hidden" id="ResaBassa" runat="server" />
                            <input type="hidden" id="ResaAlta" runat="server" />
                            <input type="hidden" id="ResaBassaDes" runat="server" />
                            <input type="hidden" id="ResaAltaDes" runat="server" />

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- BILANCIO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-bilancio" id="Div_Bilancio" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title" style="text-transform: uppercase">
                    <b>
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Bilancio %>" runat="server">BILANCIO</asp:Localize>
                    </b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row">
                        <asp:GridView ID="grdNecessita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                            <columns>
                                <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                <asp:BoundField DataField="Descrizione" HeaderText="Necessita'" meta:resourcekey="Necessita" />
                                <asp:BoundField DataField="N" HeaderText="N [Kg/Ha]" meta:resourcekey="NKgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
<%--                                <asp:BoundField DataField="P" HeaderText="P2O5 [Kg/Ha]" meta:resourcekey="P2O5KgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="K" HeaderText="K2O [Kg/Ha]" meta:resourcekey="K2OKgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />--%>
                            </columns>
                            <headerstyle cssclass="ui-widget-header" />
                            <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                        </asp:GridView>
                    </div>
                    <div class="row">
                        <asp:GridView ID="GrdDisponibilita" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                            AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                            <columns>
                                <asp:BoundField DataField="Indice" HeaderText="" Visible="false" />
                                <asp:BoundField DataField="Descrizione" meta:resourcekey="Disponibilita" HeaderText="Disponibilita'" />
                                <asp:BoundField DataField="N" HeaderText="N [Kg/Ha]" meta:resourcekey="NKgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
<%--                                <asp:BoundField DataField="P" HeaderText="P2O5 [Kg/Ha]" meta:resourcekey="P2O5KgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="K" HeaderText="K2O [Kg/Ha]" meta:resourcekey="K2OKgHa" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />--%>
                            </columns>
                            <headerstyle cssclass="ui-widget-header" />
                            <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                        </asp:GridView>
                    </div>

                    <div class="row" style="margin-top: 10px">
                        <div>
                            <asp:DropDownList ID="ddlMasB" runat="server" CssClass="myCombo" Style="margin-bottom: 5px;" ForeColor="Red"></asp:DropDownList>
                            <asp:Label ID="LblAttenzione" runat="server" CssClass="txtUI" ForeColor="Red"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- SCHEDE -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-schede" id="Div_Schede" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title"></h4>
            </div>
            <div>
                <div class="panel-body">
                    <!-- AZOTO -->
                    <div class="row">
                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title" style="text-transform: uppercase">
                                    <b>
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Azoto %>" runat="server">AZOTO</asp:Localize>
                                    </b>
                                </h4>
                            </div>
                            <div>
                                <div class="panel-body">
                                    <div id="tab_Scheda_N" runat="server" class="row">
                                        <div class="row">
                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="float: left; padding: 10px;">
                                                    <asp:Button ID="RicaricaGrid" runat="server" Style="display: none;" />
                                                    <asp:GridView ID="grdDecrementi" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                        AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                        <columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate>
                                                                    <asp:CheckBox ID="Chk_SelDecrementi_N" CssClass="Chk_SelDecrementi_N" meta:resourcekey="Seleziona" ToolTip="Seleziona"
                                                                        runat="server" />
                                                                </itemtemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" meta:resourcekey="Decrementi" />
                                                            <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                                <itemstyle cssclass="Variazione" />
                                                            </asp:BoundField>
                                                        </columns>
                                                        <headerstyle cssclass="ui-widget-header" />
                                                        <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                                    </asp:GridView>
                                                </div>
                                            </div>

                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="float: left; padding: 10px;">
                                                    <asp:GridView ID="grdIncrementi" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                        AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True">
                                                        <columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <itemtemplate>
                                                                    <asp:CheckBox ID="Chk_SelIncrementi_N" meta:resourcekey="Seleziona" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_N" />
                                                                </itemtemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Fattore_Cod">
                                                                <itemstyle cssclass="Codice displaynone" />
                                                                <headerstyle cssclass="displaynone" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" meta:resourcekey="Incrementi" />
                                                            <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                                <itemstyle cssclass="Variazione" />
                                                            </asp:BoundField>
                                                        </columns>
                                                        <headerstyle cssclass="ui-widget-header" />
                                                        <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                                    </asp:GridView>
                                                </div>
                                            </div>

                                            <div class="col-lg-4 col-md-4 col-xs-12  ">
                                                <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px;">
                                                    <div>
                                                        <asp:Label ID="TextBox1" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="DoseStandard" Text="Dose standard:" Width="135px" Height="20px">:</asp:Label>
                                                        <asp:TextBox ID="Txt_DoseStandard" runat="server" CssClass="txtui"
                                                            MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="LabelMAS" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="LimiteMAS" Text="Limite MAS :" Width="135px" Height="20px">:</asp:Label><br />
                                                        <asp:DropDownList ID="ddlMasS" runat="server" CssClass="myCombo" Width="215px" Style="margin-bottom: 5px;"></asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label5" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="MaxIncrementi" Text="Max Incrementi :" Width="135px" Height="20px">:</asp:Label>
                                                        <asp:TextBox ID="Txt_MaxIncrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            meta:resourcekey="MassimoValoreConsentitoIncrementi" Style="margin-left: 5px; pointer-events: none" ToolTip="Massimo valore consentito per gli incrementi"
                                                            Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label1" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="TotaleIncrementi" Text="Totale incrementi :" Width="135px" Height="20px">:</asp:Label>
                                                        <asp:TextBox ID="Txt_TotIncrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label2" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="TotaleDecrementi" Text="Totale decrementi :" Width="135px" Height="20px">:</asp:Label>
                                                        <asp:TextBox ID="Txt_TotDecrementi" runat="server" CssClass="txtui" MaxLength="250"
                                                            Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="Label3" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            meta:resourcekey="DoseRicalcolata" Text="Dose ricalcolata :" Width="135px" Height="20px">:</asp:Label>
                                                        <asp:TextBox ID="Txt_DoseRicalcolata" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                            meta:resourcekey="DoseNConsentita" Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di N consentita" Width="60px">
                                                        </asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:Label ID="LabelMAS_Nota" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                            Text="" Height="20px"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- FOSFORO -->
                    <div class="row">
                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title" style="text-transform: uppercase">
                                    <b>
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Fosforo %>" runat="server">FOSFORO</asp:Localize>
                                    </b>
                                </h4>
                            </div>
                            <div>
                                <div class="panel-body">
                                    <div id="tab_Scheda_P" runat="server" class="row">
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px">
                                                <asp:GridView ID="grdDecrementiP" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                                                    <columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <itemtemplate>
                                                                <asp:CheckBox ID="Chk_SelDecrementi_P" CssClass="Chk_SelDecrementi_P" meta:resourcekey="Seleziona" ToolTip="Seleziona"
                                                                    runat="server" />
                                                            </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" meta:resourcekey="Decrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                            <itemstyle cssclass="Variazione" />
                                                        </asp:BoundField>
                                                    </columns>
                                                    <headerstyle cssclass="ui-widget-header" />
                                                    <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                                </asp:GridView>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">
                                                <asp:GridView ID="grdIncrementiP" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="5" CellSpacing="5" EnableModelValidation="True" Style="width: 100%">
                                                    <columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <itemtemplate>
                                                                <asp:CheckBox ID="Chk_SelIncrementi_P" meta:resourcekey="Seleziona" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_P" />
                                                            </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Cod">
                                                            <itemstyle cssclass="Codice displaynone" />
                                                            <headerstyle cssclass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" meta:resourcekey="Incrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                            <itemstyle cssclass="Variazione" />
                                                        </asp:BoundField>
                                                    </columns>
                                                    <headerstyle cssclass="ui-widget-header" />
                                                    <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                                </asp:GridView>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px;">
                                                <div>
                                                    <asp:Label ID="Label7" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="SelezionareDose" Text="Selezionare la dose" Width="135px" Height="20px"></asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="ddlDoseP" runat="server" CssClass="myCombo" Width="215px"
                                                        Style="margin-bottom: 5px;">
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label6" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="DoseStandard" Text="Dose standard :" Width="135px" Height="20px">:</asp:Label>
                                                    <asp:TextBox ID="Txt_DoseStandardP" runat="server" CssClass="txtui"
                                                        MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label8" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="TotaleIncrementi" Text="Totale incrementi :" Width="135px" Height="20px">:</asp:Label>
                                                    <asp:TextBox ID="Txt_TotIncrementiP" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label9" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="TotaleDecrementi" Text="Totale decrementi :" Width="135px" Height="20px">:</asp:Label>
                                                    <asp:TextBox ID="Txt_TotDecrementiP" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label10" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="DoseRicalcolata" Text="Dose ricalcolata :" Width="135px" Height="20px">:</asp:Label>
                                                    <asp:TextBox ID="Txt_DoseRicalcolataP" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di P consentita" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <!-- POTASSIO -->
                    <div class="row">
                        <div class="panel panel-primary">
                            <div class="panel-heading" align="center">
                                <h4 class="panel-title" style="text-transform: uppercase">
                                    <b>
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Potassio %>" runat="server">POTASSIO</asp:Localize>
                                    </b>
                                </h4>
                            </div>

                            <div>
                                <div class="panel-body">
                                    <div id="tab_Scheda_K" runat="server" class="row">
                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">
                                                <asp:GridView ID="grdDecrementiK" runat="server" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="10" CellSpacing="10">
                                                    <columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <itemtemplate>
                                                                <asp:CheckBox ID="Chk_SelDecrementi_K" CssClass="Chk_SelDecrementi_K" meta:resourcekey="Seleziona" ToolTip="Seleziona"
                                                                    runat="server" />
                                                            </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Decrementi" meta:resourcekey="Decrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                            <itemstyle cssclass="Variazione" />
                                                        </asp:BoundField>
                                                    </columns>
                                                    <headerstyle cssclass="ui-widget-header" />
                                                    <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />

                                                </asp:GridView>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="float: left; padding: 10px;">
                                                <asp:GridView ID="grdIncrementiK" runat="server" CssClass="ui-widget-content" AllowPaging="false"
                                                    AutoGenerateColumns="False" CellPadding="10" CellSpacing="10" EnableModelValidation="True" Width="100%">
                                                    <columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <itemtemplate>
                                                                <asp:CheckBox ID="Chk_SelIncrementi_K" meta:resourcekey="Seleziona" ToolTip="Seleziona" runat="server" CssClass="Chk_SelIncrementi_K" />
                                                            </itemtemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Fattore_Cod">
                                                            <itemstyle cssclass="Codice displaynone" />
                                                            <headerstyle cssclass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Fattore_Des" HeaderText="Incrementi" meta:resourcekey="Incrementi" />
                                                        <asp:BoundField DataField="Valore" HeaderText="Valore" meta:resourcekey="Valore" ItemStyle-HorizontalAlign="center">
                                                            <itemstyle cssclass="Variazione" />
                                                        </asp:BoundField>
                                                    </columns>
                                                    <headerstyle cssclass="ui-widget-header" />
                                                    <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                                                </asp:GridView>
                                            </div>
                                        </div>

                                        <div class="col-lg-4 col-md-4 col-xs-12  ">
                                            <div style="min-height: 150px; float: left; min-width: 250px; padding: 10px; width: 100%">
                                                <div>
                                                    <asp:Label ID="Label11" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="SelezionareDose" Text="Selezionare la dose" Width="135px" Height="20px"></asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="ddlDoseK" runat="server" CssClass="myCombo" Width="215px"
                                                        Style="margin-bottom: 5px;">
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label12" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="DoseStandard" Text="Dose standard :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_DoseStandardK" runat="server" CssClass="txtui"
                                                        MaxLength="250" Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label13" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="TotaleIncrementi" Text="Totale incrementi :" Width="135px" Height="20px">:
                                                    </asp:Label>
                                                    <asp:TextBox ID="Txt_TotIncrementiK" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label14" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="TotaleDecrementi" Text="Totale decrementi :" Width="135px" Height="20px"></asp:Label>
                                                    <asp:TextBox ID="Txt_TotDecrementiK" runat="server" CssClass="txtui" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" Width="60px">
                                                    </asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:Label ID="Label15" runat="server" CssClass="txtui" Font-Bold="True" Style="margin-left: 5px;"
                                                        meta:resourcekey="DoseRicalcolata" Text="Dose ricalcolata:" Width="135px" Height="20px">:
                                                    </asp:Label>
                                                    <asp:TextBox ID="Txt_DoseRicalcolataK" runat="server" CssClass="txtui" Font-Bold="True" MaxLength="250"
                                                        Style="margin-left: 5px; pointer-events: none" ToolTip="Dose di K consentita" Width="60px">
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


        <!-- BOTTONE SALVA PIANO -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-salva-piano" id="Div_BTNSalva" runat="server">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b></b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-success btn_100 " onclick="$('#<%=Btn_Salva.ClientID %>').click();" style="text-transform: uppercase">
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SalvaPC %>" runat="server">SALVA PIANO CONCIMAZIONE</asp:Localize>
                            </div>
                            <asp:Button ID="Btn_Salva" runat="server" Style="display: none" />
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- IMPIANTI -->
        <div class="panel panel-primary gias-section gias-section-pcb-inserimento-impianti" id="Div_Appezzamenti" runat="server" style="margin-bottom: 75px;">
            <div class="panel-heading">
                <h4 class="panel-title gias-section-title" style="text-transform: uppercase">
                    <b>
                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Appezzamenti %>" runat="server">APPEZZAMENTI</asp:Localize>
                    </b>
                </h4>
            </div>

            <div>
                <div class="panel-body">
                    <div class="row" id="Div_NPK_Calcolati" runat="server">
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label17" runat="server" CssClass="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NKgHa %>" runat="server">N [Kg/Ha]</asp:Localize>
                                </asp:Label>
                                <asp:TextBox ID="Txt_N_Da_Applicare" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
<%--                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label18" runat="server" CssClass="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, P2O5KgHa %>" runat="server">P2O5 [Kg/Ha]</asp:Localize>
                                </asp:Label>
                                <asp:TextBox ID="Txt_P_Da_Applicare" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="input-group">
                                <asp:Label ID="label19" runat="server" CssClass="input-group-addon alert-info">
                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, K2OKgHa %>" runat="server">K2O [Kg/Ha]</asp:Localize>
                                </asp:Label>
                                <asp:TextBox ID="Txt_K_Da_Applicare" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>--%>
                    </div>
                    <div class="row" id="Div_BtnApplica" runat="server" style="margin-bottom: 10px">
                        <div class="col-lg-12 col-md-12 col-xs-12  ">
                            <div class="btn btn-success btn_100" onclick="$('#<%=Btn_Applica.ClientID %>').click();" style="text-transform: uppercase">
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, AssociaNAppezzaSelezionati %>" runat="server">ASSOCIA N AGLI APPEZZAMENTI SELEZIONATI</asp:Localize>
                            </div>
                            <asp:Button ID="Btn_Applica" runat="server" Style="display: none" />
                        </div>
                    </div>
                    <div class="row" style="overflow: scroll;">
                        <asp:GridView ID="GridView_Impianti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                            CssClass="ui-widget-content">
                            <columns>
                                <asp:TemplateField>
                                    <headertemplate>
                                        <input type="checkbox" id="chkSelezionaTuttiImpianti" />
                                    </headertemplate>
                                    <itemtemplate>
                                        <asp:CheckBox ID="ChkSelezionaImpianto" runat="server" CssClass="ChkSelezionaImpianto" />
                                    </itemtemplate>
                                    <headerstyle width="20px" />
                                    <itemstyle width="20px" />
                                    <footerstyle width="20px" />
                                    <controlstyle width="20px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Campo_Cod" HeaderText="Campo_Cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Appezza" HeaderText="Appezza" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Id_Reg" HeaderText="Id_Reg" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="progetto_cod" HeaderText="progetto_cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Veg_Cod" HeaderText="Veg_Cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cul_Cod" HeaderText="Cul_Cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Grfi_Cod" HeaderText="Grfi_Cod" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="rag_soc" HeaderText="rag_soc" HtmlEncode="false">
                                    <itemstyle cssclass="displaynone" />
                                    <headerstyle cssclass="displaynone" />
                                </asp:BoundField>

                                <asp:BoundField DataField="Sa_Nome" meta:resourcekey="CentroAziendale" HeaderText="Centro Aziendale" SortExpression="Sa_Nome"></asp:BoundField>
                                <asp:BoundField DataField="Campo_Nome" meta:resourcekey="Campo" HeaderText="Campo" SortExpression="Campo_Nome"></asp:BoundField>

                                <asp:BoundField DataField="App_Nome" meta:resourcekey="App" HeaderText="App." SortExpression="App_Nome"></asp:BoundField>
                                <asp:BoundField DataField="Catasto" meta:resourcekey="Catasto" HeaderText="Catasto" SortExpression="Catasto" HtmlEncode="false"></asp:BoundField>

                                <asp:BoundField DataField="Descrizione" meta:resourcekey="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione"></asp:BoundField>
                                <asp:BoundField DataField="Sup_Imp" meta:resourcekey="SupImp" HeaderText="Sup.[ha]" SortExpression="Sup_Imp">
                                    <itemstyle cssclass="Sup_Imp" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Validita_Inizio" meta:resourcekey="DataInizioImpianto" HeaderText="Data Inizio Impianto" SortExpression="Validita_Inizio"></asp:BoundField>
                                <asp:BoundField DataField="Validita_Fine" meta:resourcekey="DataFineImpianto" HeaderText="Data Fine Impianto" SortExpression="Validita_Fine"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxN" meta:resourcekey="QtaNMaxKgHa" HeaderText="Qta N Max [Kg/Ha]" SortExpression="QtaMaxN"></asp:BoundField>
<%--                                <asp:BoundField DataField="QtaMaxP2O5" meta:resourcekey="QtaPMaxKgHa" HeaderText="Qta P Max [Kg/Ha]" SortExpression="QtaMaxP2O5"></asp:BoundField>
                                <asp:BoundField DataField="QtaMaxK2O" meta:resourcekey="QtaKMaxKgHa" HeaderText="Qta K Max [Kg/Ha]" SortExpression="QtaMaxK2O"></asp:BoundField>--%>

                            </columns>
                            <headerstyle cssclass="ui-widget-header" />
                            <pagerstyle cssclass="ui-widget-header" horizontalalign="Center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <asp:UpdatePanel ID="UpdatePanelPerScript" runat="server">
            <contenttemplate>
            </contenttemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoNutrizionale_IBF.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoNutrizionale_IBF_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoNutrizionale_IBF_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoNutrizionale_IBF_globali.js") %>"></script>
</asp:Content>
