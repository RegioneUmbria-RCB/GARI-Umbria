<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_StampeCantine.aspx.vb"
    Inherits="AgronicaStampe_2010.Filtro_StampeCantine" MasterPageFile="~/Master/StampeBootstrap.Master" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/StampeBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Filtro Stampe Conferimenti</title>
    <style>
        #ui-datepicker-div
        {
            z-index: 10000;
        }
    </style>
    <style type="text/css">
        .titoli-pannelli
        {
            text-transform: uppercase;
            font-size: 14px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            //$(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-lg-10">
            </div>
            <div class="col-lg-2" style="float: right; margin-right: 10px; text-align: right;">
                <asp:ImageButton ID="ImgBtn_Stampa" runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico">
                </asp:ImageButton>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6 col-md-6 col-sm-6">
                <asp:Label ID="lbl_TipoOperazione" runat="server" CssClass="titoli-pannelli">Selezionare l'operazione da eseguire:</asp:Label>
            </div>
        </div>
        <div class="row" id="Row_TipoOperazione" style="margin-bottom: 20px">
            <div class="col-lg-6 col-md-6 col-sm-6">
                <asp:DropDownList ID="ddl_TipoOperazione" runat="server" AutoPostBack="true" CssClass="form-control">
                    <asp:ListItem Value="5">Stampa Consistenze Enologiche</asp:ListItem>
                    <asp:ListItem Value="6">Brogliaccio Movimenti Completo</asp:ListItem>
                    <asp:ListItem Value="7">Brogliaccio Movimenti Pdf Semplificato</asp:ListItem>
                    <asp:ListItem Value="8">Riepilogo Imbottigliamenti</asp:ListItem>
                </asp:DropDownList>
                <%--            <asp:ListItem Value="1">Stampa Registri di Cantina (OBSOLETI)</asp:ListItem>
                    <asp:ListItem Value="2">Stampa Verifica Registri di Cantina (OBSOLETI)</asp:ListItem>
                    <asp:ListItem Value="3">Stampa Copertina Registro</asp:ListItem>
                    <asp:ListItem Value="4">Stampa Registro Vuoto</asp:ListItem> --%>
            </div>
        </div>
        <!-- 0 Registri -->
        <div class="row" runat="server" id="Div_SelRegistri" style="margin-bottom: 15px">
            <div class="col-lg-6 col-md-6 col-sm-6">
                <asp:Label ID="LABEL1" runat="server" CssClass="titoli-pannelli">Selezionare il registro:</asp:Label>
                <%-- <asp:RadioButtonList id="Rbl_Report" runat="server"   AutoPostBack="true"> </asp:RadioButtonList>
                --%>
                <asp:RadioButtonList ID="Rbl_Report" runat="server" AutoPostBack="False">
                    <asp:ListItem Value="3" Selected="True">Riepilogo Imbottigliamenti</asp:ListItem>
                    <asp:ListItem Value="0">---</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6">
                <asp:Panel ID="Pannello_Partita" runat="server" Visible="False">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span9" for="Cmb_LineaProduzione">Vino</span>
                                        <asp:DropDownList ID="Cmb_LineaProduzione" runat="server" AutoPostBack="True" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="height: 5px" id="b">
                    </div>
                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-sm-8">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span10" for="Cmb_Partita">Partita</span>
                                        <asp:DropDownList ID="Cmb_Partita" runat="server" AutoPostBack="True" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-4">
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!-- 1 Periodo -->
        <div class="row" id="Div_Periodo" runat="server" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Periodo" runat="server">
                    <asp:Label ID="Label14" runat="server" CssClass="titoli-pannelli">Periodo:</asp:Label>
                    <div class="row" id="Div1">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:RadioButtonList ID="Rbl_MeseIntervallo" runat="server" CssClass="testo_08_nero"
                                AutoPostBack="True" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0" Selected="True">Mese&nbsp&nbsp&nbsp</asp:ListItem>
                                <asp:ListItem Value="1">Intervallo Temporale</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row" id="Div4">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Panel ID="Pannello_AnnoMese" runat="server">
                                <div class="row">
                                    <div class="col-lg-5 col-md-5 col-sm-5">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="LABEL29" for="Cmb_Anno">Seleziona l'anno:</span>
                                                    <asp:DropDownList ID="Cmb_Anno" runat="server" AutoPostBack="False" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-7 col-md-7 col-sm-7">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-5 col-md-5 col-sm-5">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="LABEL28" for="Cmb_Mese">Seleziona il
                                                        mese:</span>
                                                    <asp:DropDownList ID="Cmb_Mese" runat="server" AutoPostBack="False" CssClass="form-control">
                                                        <asp:ListItem Value="1">Gennaio</asp:ListItem>
                                                        <asp:ListItem Value="2">Febbraio</asp:ListItem>
                                                        <asp:ListItem Value="3">Marzo</asp:ListItem>
                                                        <asp:ListItem Value="4">Aprile</asp:ListItem>
                                                        <asp:ListItem Value="5">Maggio</asp:ListItem>
                                                        <asp:ListItem Value="6">Giugno</asp:ListItem>
                                                        <asp:ListItem Value="7">Luglio</asp:ListItem>
                                                        <asp:ListItem Value="8">Agosto</asp:ListItem>
                                                        <asp:ListItem Value="9">Settembre</asp:ListItem>
                                                        <asp:ListItem Value="10">Ottobre</asp:ListItem>
                                                        <asp:ListItem Value="11">Novembre</asp:ListItem>
                                                        <asp:ListItem Value="12">Dicembre</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-7 col-md-7 col-sm-7">
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="row" id="Div14">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Panel ID="Pannello_Date" runat="server">
                                <div class="row">
                                    <div class="col-lg-5 col-md-5 col-sm-5">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span18" for="Txt_DataInizio">Data Inizio</span>
                                                    <asp:TextBox ID="Txt_DataInizio" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue datepicker form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-7 col-md-7 col-sm-7">
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-5 col-md-5 col-sm-5">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span19" for="Txt_DataFine">Data Fine</span>
                                                    <asp:TextBox ID="Txt_DataFine" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue datepicker form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-7 col-md-7 col-sm-7">
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!-- 2 Categoria -->
        <div class="row" id="Div_Categoria" runat="server" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_Categorie" runat="server">
                    <asp:Label ID="Lbl_Categoria" runat="server" CssClass="titoli-pannelli">Categoria:</asp:Label>
                    <asp:CheckBoxList ID="CBL_Categoria" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="19" Selected="True">Docg&nbsp&nbsp&nbsp</asp:ListItem>
                        <asp:ListItem Value="20" Selected="True">Dop&nbsp&nbsp&nbsp</asp:ListItem>
                        <asp:ListItem Value="21" Selected="True">Igp&nbsp&nbsp&nbsp</asp:ListItem>
                        <asp:ListItem Value="22" Selected="True">Tavola&nbsp&nbsp&nbsp</asp:ListItem>
                    </asp:CheckBoxList>
                </asp:Panel>
            </div>
        </div>
        <!-- 3 Conto Lavorazione -->
        <div class="row" id="Div_ContoTerzi" runat="server" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_ContoTerzi" runat="server">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Label ID="LABEL37" runat="server" CssClass="titoli-pannelli">Conto Lavorazione:</asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:RadioButtonList ID="Rbl_ContoTerzi" runat="server" CssClass="Testo_08_Nero"
                                AutoPostBack="True">
                                <asp:ListItem Value="1">Stampa il registro complessivo</asp:ListItem>
                                <asp:ListItem Value="2">Stampa un registro unico (voci di riepilogo separate per c/lav - reg. vinificazione)</asp:ListItem>
                                <asp:ListItem Value="4">Stampa registro per solo i c/lavoro (reg. imbottigliamento)</asp:ListItem>
                                <asp:ListItem Value="3" Selected="True">Stampa un registro separato per ogni c/lav</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:DropDownList ID="Cmb_ContattiContoTerzi" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!-- 4 Intestazione -->
        <div class="row" id="Div_Intestazione" runat="server" style="margin-bottom: 10px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:CheckBox ID="Chk_StampaIntestazione" runat="server" Text="Stampa Intestazione"
                    Checked="false"></asp:CheckBox>
            </div>
        </div>
        <!-- Obsoleto -->
        <div class="row" id="Div30" visible="False">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:CheckBox ID="Chk_StampaRiporti" runat="server" Text="Stampa Riporti" Visible="False">
                </asp:CheckBox>
            </div>
        </div>
        <!-- 5 Centro aziendale -->
        <div class="row" runat="server" id="Div_CentroAziendale" style="margin-bottom: 15px">
            <div class="col-lg-6 col-md-6 col-sm-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="LABEL7" for="Cmb_CentroAziendale">Centro
                                Aziendale:</span>
                            <asp:DropDownList ID="Cmb_CentroAziendale" runat="server" AutoPostBack="True" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6">
            </div>
        </div>
        <!-- 6 Verifica Registri -->
        <div class="row" id="Div_VerificaRegistri" runat="server" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_Debug" runat="server">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Label ID="LABEL9" runat="server" CssClass="titoli-pannelli">Verifica Registri:</asp:Label>
                        </div>
                    </div>
                    <div class="row" id="Div19">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:RadioButtonList ID="Rbl_Verifica" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0">Linea&nbsp&nbsp&nbsp</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Voce Riep.</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:RadioButtonList ID="Rbl_CauMov" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
                                <asp:ListItem Value="0" Selected="True">Tutti&nbsp&nbsp&nbsp</asp:ListItem>
                                <asp:ListItem Value="7300">Carichi&nbsp&nbsp&nbsp</asp:ListItem>
                                <asp:ListItem Value="7350">Scarichi</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row" id="Div20">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Panel ID="Pannello_VoceRiepilogo" runat="server" Visible="False">
                                <div class="row" id="Div21">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span1" for="Cmb_VociRiepilogo">Seleziona
                                                        voce di riepilogo:</span>
                                                    <asp:DropDownList ID="Cmb_VociRiepilogo" runat="server" AutoPostBack="True" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row" id="Div22">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span2" for="Cmb_MateriePrime">Seleziona
                                                        un prodotto:</span>
                                                    <asp:DropDownList ID="Cmb_MateriePrime" runat="server" AutoPostBack="True" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row" id="Div2">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span3" for="cmb_LineeByMat_Cod">Seleziona
                                                        una linea:</span>
                                                    <asp:DropDownList ID="cmb_LineeByMat_Cod" runat="server" AutoPostBack="True" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="row" id="Div23">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span4" for="Cmb_MagazzinoVasca">Mag./vasca:</span>
                                        <asp:DropDownList ID="Cmb_MagazzinoVasca" runat="server" AutoPostBack="True" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" id="Div24">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:Panel ID="Pannello_Linee" runat="server" Visible="False">
                                <div class="row" id="Div15">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span5" for="Cmb_Linee">Seleziona una
                                                        linea:</span>
                                                    <asp:DropDownList ID="Cmb_Linee" runat="server" AutoPostBack="True" CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row" id="Div17">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon alert-info" id="Span6" for="Cmb_MateriePrimeByLinee">
                                                        Seleziona un prodotto:</span>
                                                    <asp:DropDownList ID="Cmb_MateriePrimeByLinee" runat="server" AutoPostBack="True"
                                                        CssClass="form-control">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!--7 Copertina Registro -->
        <div class="row" runat="server" id="Div_CopertinaRegistro" style="margin-bottom: 15px">
            <div class="row" style="height: 5px">
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_Copertina" runat="server">
                    <asp:Label ID="LABEL3" runat="server" CssClass="titoli-pannelli">Copertina Registro:</asp:Label>
                    <div class="row" id="Div6">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:CheckBox ID="Chk_Pagina1" runat="server" Text="Visualizzare il numero di pagina (1)">
                            </asp:CheckBox>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:CheckBox ID="Chk_SaNome" runat="server" Text="Stampa nome Centro Aziendale">
                            </asp:CheckBox>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div7">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:Label ID="LABEL30" runat="server">Specificare il numero totale delle pagine (TOT):</asp:Label>
                            <asp:TextBox ID="Txt_Copertina_TotPagine" runat="server" BackColor="#FFFFFF" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:Label ID="LABEL19" runat="server">Indicare il progressivo e la sigla del registro (facoltativo):</asp:Label>
                            <asp:TextBox ID="Txt_Frontespizio" runat="server" BackColor="#FFFFFF" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div8">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span7" for="Cmb_Indirizzo">Indirizzo
                                            sede cantina:</span>
                                        <asp:DropDownList ID="Cmb_Indirizzo" runat="server" AutoPostBack="True" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!--8 Registro vuoto -->
        <div class="row" runat="server" id="Div_RegistroVuoto" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_RegistroVuoto" runat="server">
                    <asp:Label ID="LABEL6" runat="server" CssClass="titoli-pannelli">Registro Vuoto:</asp:Label>
                    <div class="row" id="Div10">
                        <div class="col-lg-7 col-md-7 col-sm-7">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span8" for="Txt_ProgrSigla_Registro">
                                            Progressivo e sigla del registro (facoltativo):</span>
                                        <asp:TextBox ID="Txt_ProgrSigla_Registro" runat="server" BackColor="#FFFFFF" CssClass="form-control"
                                            BorderStyle="None"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div11">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="row" id="Div18">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <asp:Label ID="LABEL11" runat="server">Specificare la numerazione delle pagine</asp:Label>
                                </div>
                            </div>
                            <div class="row" id="Div25">
                                <div class="col-lg-4 col-md-4 col-sm-4">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span20" for="Txt_NumPagineDa">Da</span>
                                                <asp:TextBox ID="Txt_NumPagineDa" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue  form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-4">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span21" for="Txt_NumPagineA">A</span>
                                                <asp:TextBox ID="Txt_NumPagineA" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue  form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-4">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon alert-info" id="Span22" for="Txt_NumPagine">Tot</span>
                                                <asp:TextBox ID="Txt_NumPagine" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue  form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <!--9 Consistenze Enologiche -->
        <div class="row" runat="server" id="Div_ConsistenzeEnologiche" style="margin-bottom: 15px">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <asp:Panel ID="Pannello_ConsEnologiche" runat="server">
                    <asp:Label ID="LABEL8" runat="server" CssClass="titoli-pannelli">Consistenze Enologiche:</asp:Label>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div13">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span17" for="Txt_DataConsistenze">Data
                                            di stampa</span>
                                        <asp:TextBox ID="Txt_DataConsistenze" runat="server" BackColor="#FFFFFF" CssClass="Testo_08_Blue datepicker form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:CheckBox ID="Chk_StampaRiepilogo" runat="server" Text="Stampa il riepilogo">
                            </asp:CheckBox>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div5">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:CheckBox ID="Chk_VascheNoMov" runat="server" Text="Stampa anche le vasche non movimentate">
                            </asp:CheckBox>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <asp:CheckBox ID="Chk_ConsEnoZero" runat="server" Text="Stampa anche le vasche con consistenze =0">
                            </asp:CheckBox>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div16">
                        <div class="col-lg-4 col-md-4 col-sm-4">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span15" for="Cmb_Piano">Piano</span>
                                        <asp:DropDownList ID="Cmb_Piano" runat="server" AutoPostBack="True" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-8 col-md-8 col-sm-8">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span16" for="Cmb_LineeConsistenze">Linea
                                            produttiva</span>
                                        <asp:DropDownList ID="Cmb_LineeConsistenze" runat="server" AutoPostBack="False" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div9">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span11" for="Cmb_Vasca">Vasca</span>
                                        <asp:DropDownList ID="Cmb_Vasca" runat="server" AutoPostBack="False" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span12" for="Cmb_Ordinamento">Ordinamento</span>
                                        <asp:DropDownList ID="Cmb_Ordinamento" runat="server" AutoPostBack="False" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="height: 5px">
                    </div>
                    <div class="row" id="Div12">
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span13" for="Cmb_Categoria">Categoria</span>
                                        <asp:DropDownList ID="Cmb_Categoria" runat="server" AutoPostBack="False" CssClass="form-control">
                                            <asp:ListItem Value="0">No filtro</asp:ListItem>
                                            <asp:ListItem Value="19">Docg</asp:ListItem>
                                            <asp:ListItem Value="20">Dop</asp:ListItem>
                                            <asp:ListItem Value="21">Igp</asp:ListItem>
                                            <asp:ListItem Value="22">Tavola</asp:ListItem>
                                            <asp:ListItem Value="163">Varietale</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-6 col-md-6 col-sm-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info" id="Span14" for="Cmb_Semilavorati">Tip. semilavorati</span>
                                        <asp:DropDownList ID="Cmb_Semilavorati" runat="server" AutoPostBack="False" CssClass="form-control">
                                            <asp:ListItem Value="0">No filtro</asp:ListItem>
                                            <asp:ListItem Value="55">Vino</asp:ListItem>
                                            <asp:ListItem Value="433">Vino Arricchito</asp:ListItem> 
                                            <asp:ListItem Value="54">Vino Atto a Divenire</asp:ListItem>
                                            <asp:ListItem Value="780">Vino Atto a Divenire Arricchito</asp:ListItem>
                                            <asp:ListItem Value="53">VNAF</asp:ListItem>
                                            <asp:ListItem Value="434">VNAF Arricchito</asp:ListItem>
                                            <asp:ListItem Value="52">MostoPF</asp:ListItem>
                                            <asp:ListItem Value="51">Mosto</asp:ListItem>                                                                                       
                                            <asp:ListItem Value="231">Vino in Frizzantatura</asp:ListItem>
                                            <asp:ListItem Value="247">Vino in Spumantizzazione</asp:ListItem>
                                            <asp:ListItem Value="319">Vino Atto in Frizzantatura </asp:ListItem>
                                            <asp:ListItem Value="248">Vino Atto in Spumantizzazione </asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" id="DivLotto">
                        <div class="col-lg-2 col-md-2 col-sm-12">
                            <asp:Label ID="LABEL4" runat="server">Filtro sul lotto:</asp:Label>
                        </div>
                        <div class="col-lg-3 col-md-3 col-sm-12">
                            <asp:RadioButtonList ID="rbl_Lotto" runat="server" CssClass="Testo_08_Nero" AutoPostBack="False"
                                RepeatDirection="Horizontal">
                                <asp:ListItem Value="1" Selected="True">Contiene:</asp:ListItem>
                                <asp:ListItem Value="2">Non contiene:</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="col-lg-7 col-md-7 col-sm-12">
                            <asp:TextBox ID="Txt_Lotto" runat="server" Visible="true" CssClass="form-control"
                                BackColor="#FFFFFF"></asp:TextBox>
                        </div>
                
                    </div>
            </div>
            </asp:Panel>
        </div>
    </div>
    <!-- Campo ad uso interno: viene passato nella query string -->
    <div class="row" runat="server" id="Div3">
        <div class="col-lg-6 col-md-6 col-sm-6">
            <asp:TextBox ID="Txt_RagSoc" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <!-- 100 Etichette -->
    <div class="row" runat="server" id="Div_Etichette" style="margin-bottom: 15px">
        <div class="col-lg-6 col-md-6 col-sm-6">
            <asp:Panel ID="Pannello_EtichetteVasche" runat="server">
                <asp:Label ID="LABEL47" runat="server">Seleziona le informazioni che  si desidera stampare:</asp:Label>
                <asp:CheckBoxList ID="CBL_InfoEtichetteVasche" runat="server">
                    <asp:ListItem Value="0" Selected="True">&nbsp Identificativo</asp:ListItem>
                    <asp:ListItem Value="1" Selected="True">&nbsp Capacit&#224; HL</asp:ListItem>
                    <asp:ListItem Value="10" Selected="True">&nbsp Lotto</asp:ListItem>
                    <asp:ListItem Value="6" Selected="True">&nbsp Anno di produzione</asp:ListItem>
                    <asp:ListItem Value="5" Selected="True">&nbsp Colore</asp:ListItem>
                    <asp:ListItem Value="3" Selected="True">&nbsp Grado Babo e/o Indice di Refrazione</asp:ListItem>
                    <asp:ListItem Value="7" Selected="True">&nbsp Regolamento</asp:ListItem>
                    <asp:ListItem Value="8" Selected="True">&nbsp Atto di approvazione doc/docg</asp:ListItem>
                    <asp:ListItem Value="4" Selected="True">&nbsp Fornitore del c/lavorazione</asp:ListItem>
                    <asp:ListItem Value="9" Selected="True">&nbsp Quantit&#224; attuale litri</asp:ListItem>
                </asp:CheckBoxList>
                <!--<asp:ListItem Value="2" Selected="True">&nbsp Linea di produzione</asp:ListItem>-->
                <br />
                <asp:RadioButtonList ID="Rbl_OptEtichetta" runat="server" CssClass="testo_08_nero"
                    AutoPostBack="True" RepeatDirection="Vertical">
                    <asp:ListItem Value="0">Nessuno</asp:ListItem>
                    <asp:ListItem Value="1" Selected="True">Linea di produzione</asp:ListItem>
                    <asp:ListItem Value="2">Semilavorato</asp:ListItem>
                </asp:RadioButtonList>
            </asp:Panel>
        </div>
        <div class="col-lg-6 col-md-6 col-sm-6">
            <div class="row" runat="server" id="Div26">
                <asp:Label ID="LABEL48" runat="server">Seleziona le vasche di cui si desidera stampare l'etichetta:</asp:Label>
            </div>
            <div class="row" runat="server" id="Div27">
                <div class="col-lg-6 col-md-6 col-sm-6">
                    <asp:Label ID="Lbl_SelezionaTutteSpecie" runat="server">Seleziona Tutto</asp:Label>
                    <asp:ImageButton ID="ImgBtn_SelezionaTutti" runat="server" ImageUrl="../../AB_Immagini/icone24/ValidazioneSI_24.ico">
                    </asp:ImageButton>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6">
                    <asp:Label ID="Lbl_DeselezionaTutteSpecie" runat="server">Deseleziona Tutto</asp:Label>
                    <asp:ImageButton ID="ImgBtn_DeselezionaTutti" runat="server" ImageUrl="../../AB_Immagini/icone24/ValidazioneNO_24.ico">
                    </asp:ImageButton>
                </div>
            </div>
            <div class="row" runat="server" id="Div29">
                <asp:CheckBoxList ID="CBL_VascheElenco" runat="server">
                </asp:CheckBoxList>
            </div>
        </div>
    </div>
    <div style="clear: both;">
    </div>
    <div class="row" id="Div28" style="margin-bottom: 15px">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <asp:ImageButton ID="btnSalvaCacheRegistro" runat="server" ImageUrl="../../AB_Immagini/Icone32/salva.ico">
            </asp:ImageButton>
            <asp:ImageButton ID="btnApriCache" runat="server" ImageUrl="../../AB_Immagini/Icone32/doc5.ico">
            </asp:ImageButton>
        </div>
    </div>
    <div class="row" style="margin-bottom: 125px;">
    </div>
    </div>
</asp:Content>
