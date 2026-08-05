<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master" CodeBehind="PUA_Piano_Distribuzione.aspx.vb" Inherits="PianoConcimazione_2017.PUA_Piano_Distribuzione" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <title>PUA</title>

    <style type="text/css">
        .btn2icon {
            border: 0px;
            background-color: inherit;
            min-width: 0px !important;
            padding: 0px;
        }

        .btnWidth {
            width: 80px;
        }

        .allineadestra {
            text-align: right !important;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="container" style="margin-bottom: 50px;">

        <!-- TESTATA -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title"><b></b></h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-5 col-md-5 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Regolamento</span>
                                        <asp:DropDownList ID="ddlRegolamento" runat="server" CssClass="form-control selectpicker required">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3 col-md-3 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Metodo</span>
                                        <asp:DropDownList ID="ddlMetodo" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>

                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="col-lg-2 col-md-2 col-xs-12 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Inizio</span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-2 col-xs-12 ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Validita Fine</span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control DatePicker" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>
                    <div class="row">
                        <div class="col-lg-8 col-md-8 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Centro Aziendale</span>
                                        <asp:DropDownList ID="ddlCentriAziendali" runat="server" CssClass="form-control selectpicker required stato_group"
                                            data-live-search="true">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-xs-12 ">
                            <div id="BtnVaiBilancio" class='btn btn-warning btn_100' onclick='VaiAPianoDistribuzioneVerificaIndiciBilancio(1);'>Vai a Verifica Indici Bilancio</div>
                            <div id="BtnVaiPianoDistibuzione" class='btn btn-warning btn_100' onclick='VaiAPianoDistribuzioneVerificaIndiciBilancio(0);'>Vai a Piano Distribuzione</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- PIANO COLTURALE -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>PIANO COLTURALE</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                                    <div id="gridPD" style="display: block;"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- EFFLUENTI -->
        <div class="panel panel-primary" id="DivEffluenti" runat="server" style="display: none">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <b>EFFLUENTI IMPIEGATI</b>
                </h4>
            </div>
            <div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12 nopadding">
                            <div class="row">
                                <div id="kendo_Effluenti"></div>
                                <input type="hidden" id="HD_Effluenti" name="HD_Effluenti" runat="server" />
                                <input type="hidden" id="HD_Effluenti_Dettagli" name="HD_Effluenti_Dettagli" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- MEDIE AZIENDALI -->
        <div class="panel panel-primary" id="DivMedieAziendali" runat="server" style="display: none">
            <div class="panel-heading">
                <h4 class="panel-title"></h4>
            </div>
            <div>
                <div class="panel-body">                  

                    <div class="row" id="rigabilancio" runat="server" style="display: none">
                    
                        <h4 class="panel-title"><b>BILANCIO AZIENDALE</b></h4>                    
                        <br />

                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Bilancio Azotato Utile (Max 30) [Kg/ha]</span>
                                        <asp:TextBox ID="Txt_NUtile" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Bilancio Azotato Totale (Max 50) [Kg/ha]</span>
                                        <asp:TextBox ID="Txt_NTotale" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Indice di Efficienza Azotata media (Min 50) [%]</span>
                                        <asp:TextBox ID="Txt_IndiceEff" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <h4 class="panel-title"><b>SUPERFICI [HA]</b></h4>
                    <br />

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona ZVN</span>
                                        <asp:TextBox ID="Txt_Ha_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_Ha_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_Ha_Tot" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Ciclo Secondario Zona ZVN</span>
                                        <asp:TextBox ID="Txt_Ha_ZVN_SecondoRaccolto" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Ciclo Secondario Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_Ha_Ord_SecondoRaccolto" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Ciclo Secondario Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_Ha_Tot_SecondoRaccolto" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata Zona ZVN</span>
                                        <asp:TextBox ID="Txt_Ha_ZVN_Fertilizzati" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_Ha_Ord_Fertilizzati" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_Ha_Tot_Fertilizzati" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata con Effluenti Zootecnici Zona ZVN</span>
                                        <asp:TextBox ID="Txt_Ha_ZVN_Fertilizzati_Zoo" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata con Effluenti Zootecnici Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_Ha_Ord_Fertilizzati_Zoo" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Fertilizzata con Effluenti Zootecnici Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_Ha_Tot_Fertilizzati_Zoo" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <h4 class="panel-title"><b>AZOTO ZOOTECNICO DISTRIBUITO [KG]</b></h4>
                    <br />

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona ZVN</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Totale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Media" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Zona ZVN</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Let_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Let_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Totale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Let_Media" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Zona ZVN</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Liq_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Zona Ordinaria</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Liq_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Totale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NZoo_Tot_Liq_Media" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>



                    <h4 class="panel-title"><b>MEDIE APPORTO AZOTO [KG/HA]</b></h4>
                    <br />

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zootecnico Zona ZVN (Max 170)</span>
                                        <asp:TextBox ID="Txt_NZoo_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zootecnico Zona Ordinaria (Max 340)</span>
                                        <asp:TextBox ID="Txt_NZoo_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Zootecnico Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NZoo_Media" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Zona ZVN (Max 170)</span>
                                        <asp:TextBox ID="Txt_NLetame_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Zona Ordinaria (Max 340)</span>
                                        <asp:TextBox ID="Txt_NLetame_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Letami Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NLetame_Tot" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Zona ZVN (Max 170)</span>
                                        <asp:TextBox ID="Txt_NLiquame_ZVN" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Zona Ordinaria (Max 240)</span>
                                        <asp:TextBox ID="Txt_NLiquame_Ord" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-12 col-xs-12  ">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon alert-info">Da Liquami Aziendale (zvn + ordinaria)</span>
                                        <asp:TextBox ID="Txt_NLiquame_Tot" runat="server" CssClass="form-control" ReadOnly="true" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div id="analisiWindowContainer"></div>

        <div id="ModificaMultipla" style="display: none">
            <div id="scelta_parametro">
                <b>
                    <p id="avvertimentoModificaMultipla"></p>
                </b>
                <div class="k-edit-label">
                    <label>Parametri da Modificare</label>
                </div>
                <div data-container-for="Unità Produttiva" class="k-edit-field">
                    <input type="text" id="Cmb_Parametri" />
                </div>
            </div>

            <div id="modifica_ciclo">
                <div class="k-edit-label">
                    <label>Ciclo</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_Ciclo" />
                </div>
            </div>

            <div id="modifica_precessione">
                <div class="k-edit-label">
                    <label>Precessione</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_Precessione" />
                </div>
            </div>

            <!--
            <div id="modifica_fertOrganico">
                <div class="k-edit-label"><label>Fert. Organico</label></div>
                <div class="k-edit-field"><input type="text" id="Cmb_Mod_FertOrganico" /></div>
                
                <div class="k-edit-label"><label>Frequenza</label></div>
                <div class="k-edit-field"><input type="text" id="Cmb_Mod_Frequenza" /></div>
                
                <div class="k-edit-label"><label>N [Kg/Ha]</label></div>
                <div class="k-edit-field"><input type="text" id="Txt_Mod_N" /></div>
            </div>
            -->

            <div id="modifica_ubicazione">
                <div class="k-edit-label">
                    <label>Ubicazione</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_Ubicazione" />
                </div>
            </div>

            <div id="modifica_acqua">
                <div class="k-edit-label">
                    <label>Acqua di Irrigazione</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_TipoAcqua" />
                </div>
            </div>

            <div id="modifica_analisi">
                <div class="k-edit-label">
                    <label>Analisi</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_Analisi" />
                </div>
            </div>

            <div id="modifica_resa">
                <div class="k-edit-label">
                    <label>Resa</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Txt_Mod_Resa" />
                </div>
            </div>

            <div id="modifica_bperc">
                
                <div class="k-edit-label">
                    <label>Tipologia</label>
                </div>
                <div class="k-edit-field">
                    <input type="text" id="Cmb_Mod_FinalitaRer" />
                </div>

                <div id="div_bperc">
                    <div class="k-edit-label">
                        <label>Coeff. B[Kg/Ha]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_BPerc" />
                    </div>
                </div>

                <div id="div_mas">
                    <div class="k-edit-label">
                        <label>N Massimo da Mas [Kg/Ha]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_Mas" />
                    </div>
                    <div class="k-edit-label">
                        <label>Resa Rif. [t/Ha]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_ResaRif" />
                    </div>
                    <div class="k-edit-label">
                        <label>Fattore Correttivo [Kg N/Ha]</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_FattoreCorrettivo" />
                    </div>
                </div>

            </div>


            <input type="hidden" id="hdPiva" runat="server" />
            <input type="hidden" id="hdSaCod" runat="server" />
            <input type="hidden" id="hdPuaCod" runat="server" />
            <input type="hidden" id="hdPuaTipo" runat="server" />
            <input type="hidden" id="hdRegCod" runat="server" />
            <input type="hidden" id="hdRicettaCod" runat="server" />
            <input type="hidden" id="hdModalita" runat="server" />
            <input type="hidden" id="hdDataInizio" runat="server" />
            <input type="hidden" id="hdDataFine" runat="server" />
            <input type="hidden" id="hdBloccoFlag" runat="server" />

            <input type="hidden" id="hdImportaDaQdC" runat="server" />
            <input type="hidden" id="hdVisualizzaAnalisi" runat="server" />
            <input type="hidden" id="hdUsaAnalisiNG" runat="server" />

            <input type="hidden" id="hdAssegnaDefault" runat="server" />

        </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="PUA_Piano_Distribuzione.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="PUA_Piano_Distribuzione_jQueryDocReady.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="PUA_Piano_Distribuzione_ws_client.js?<% =Application("GiasVersioneCorrente")%>"></script>


    <script id="templateBtn_ApriBrogliaccio" type="text/x-kendo-template">
        <div class="btn btn-info" id="Btn_ApriBrogliaccio">
            <span class="lampeggiante">Fertilizzazioni Pianificate</span>
        </div>
    </script>

    <script id="templateBtn_ApriQdC" type="text/x-kendo-template">
        <div class="btn btn-info" id="Btn_ApriQdC">
            <span class="lampeggiante">Importa Fertilizzazioni da Registro</span>
        </div>
    </script>

    <script id="templateBtn_ApriAnalisi" type="text/x-kendo-template">
        <div class="btn btn-warning" id="Btn_ApriAnalisi">
            <span class="lampeggiante">Analisi</span>
        </div>
    </script>

    <script id="templateBtn_ModificaMultipla" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_ModificaMultipla">
            <span class="lampeggiante">Modifica Multipla Righe selezionate</span>
        </div>
    </script>

    <script id="templateBtn_AssegnaDefault" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_AssegnaDefault">
            <span class="lampeggiante">Aggiorna Letamazioni Precedenti</span>
        </div>
    </script>

    <script id="templateBtn_AssociaN" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_AssociaN">
            <span class="lampeggiante">Associa <b>N Massimo PUA</b> agli Appezzamenti selezionati</span>
        </div>
    </script>

    <script id="templateBtn_AssociaNMas" type="text/x-kendo-template">
        <div class="btn btn-success" id="Btn_AssociaNMas">
            <span class="lampeggiante">Associa <b>N Massimo <b id="DivMAS" >MAS</b></b> agli Appezzamenti selezionati</span>
        </div>
    </script>

    <script type="text/javascript">

        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdSaCod = "#<%=hdSaCod.ClientID() %>";
        var cIdPuaCod = "#<%=hdPuaCod.ClientID() %>";
        var cIdPuaTipo = "#<%=hdPuaTipo.ClientID() %>";
        var cIdRegCod = "#<%=hdRegCod.ClientID() %>";
        var cIdDataInizio = "#<%=hdDataInizio.ClientID() %>";
        var cIdDataFine = "#<%=hdDataFine.ClientID() %>";
        var cIdRicettaCod = "#<%=hdRicettaCod.ClientID() %>";
        var cIdModalita = "#<%=hdModalita.ClientID() %>";
        var cIdBloccoFlag = "#<%=hdBloccoFlag.ClientID() %>";
        var cIdAssegnaDefault = "#<%=hdAssegnaDefault.ClientID() %>";

        var id_HD_Effluenti = "<%= HD_Effluenti.ClientID%>";
        var id_HD_Effluenti_Dettagli = "<%= HD_Effluenti_Dettagli.ClientID%>";

        var cIdImportaDaQdC = "#<%=hdImportaDaQdC.ClientID() %>";
        var cIdVisualizzaAnalisi = "#<%=hdVisualizzaAnalisi.ClientID() %>";
        var cIdUsaAnalisiNG = "#<%=hdUsaAnalisiNG.ClientID() %>";

        let a_url = location.pathname.split("/");
        let url = a_url[a_url.length - 1];



    </script>

    <%--template kendo--%>

    <script id="Valutazione_Template_NUtile" type="x-kendo-template">
        #if (Valutazione_NUtile === 1) { #
            <i class="fa fa-circle fa-2x" style="color:yellowgreen;"></i>
        # } else if (Valutazione_NUtile === 2) { #
            <i class="fa fa-circle fa-2x" style="color:red;"></i> 
        # } else if (Valutazione_NUtile === 3) { #
            <i class="fa fa-circle fa-2x" style="color:orange;"></i> 
        # }#
    </script>
    <script id="Valutazione_Template_NTotale" type="x-kendo-template">
        #if (Valutazione_NTotale === 1) { #
            <i class="fa fa-circle fa-2x" style="color:yellowgreen;"></i>
        # } else if (Valutazione_NTotale === 2) { #
            <i class="fa fa-circle fa-2x" style="color:red;"></i> 
        # } else if (Valutazione_NTotale === 3) { #
            <i class="fa fa-circle fa-2x" style="color:orange;"></i> 
        # }#
    </script>
    <script id="Valutazione_Template_Efficienza" type="x-kendo-template">
        #if (Valutazione_Efficienza === 1) { #
            <i class="fa fa-circle fa-2x" style="color:yellowgreen;"></i>
        # } else if (Valutazione_Efficienza === 2) { #
            <i class="fa fa-circle fa-2x" style="color:red;"></i> 
        # } else if (Valutazione_Efficienza === 3) { #
            <i class="fa fa-circle fa-2x" style="color:orange;"></i> 
        # }#
    </script>
    <%--fine template kendo--%>
</asp:Content>
