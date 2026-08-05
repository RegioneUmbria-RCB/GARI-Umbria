<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master"
    CodeBehind="Selezione_SchedaCampagna.aspx.vb" Inherits="AgronicaStampe_2010.Selezione_SchedaCampagna" %>

    <%@ MasterType VirtualPath="~/Master/Stampe.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function SelezionaDeselezionaTutti() {

            if ($('#chkSelezionaTutteImprese').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaImpresa').each(function () {
                    $(this).children('input').attr('checked', 'checked');

                });
            }
            else {

                //deseleziono tutto
                $('.ChkSelezionaImpresa').each(function () {
                    $(this).children('input').removeAttr('checked');
                });
            }
        }


        // ----------------------------------------------
        // Vedi pageload prima del postback lato Server
        // 
        // ----------------------------------------------
        function SelezionaDeselezionaFito() {

            if ($('#checkTratt').children('input').is(':checked')) {
                //seleziono tutto
                $('#checkFito').children('input').attr('checked', 'checked');
                $('#checkFito').children('input').removeAttr('disabled');
            }
            else {
                $('#checkFito').children('input').removeAttr('checked');
                $('#checkFito').children('input').attr('disabled', 'disabled');
            }
        }

        function SelezionaDeselezionaSoloDifesa() {

            if ($('#checkMacch').children('input').is(':checked')) {
                //seleziono tutto
                $('#checkSoloMacchDif').children('input').attr('checked', 'checked');
                $('#checkSoloMacchDif').children('input').removeAttr('disabled');
            }
            else {
                $('#checkSoloMacchDif').children('input').removeAttr('checked');
                $('#checkSoloMacchDif').children('input').attr('disabled', 'disabled');
            }
        }

        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
            //           $(".datepicker").datepicker({ format: 'dd/mm/yyyy' }); //bootstrap version
            $('#chkSelezionaTutteImprese').click(function () {
                SelezionaDeselezionaTutti();
            });


            // ----------------------------------------------
            // Vedi pageload prima del postback lato Server
            // 
            // ----------------------------------------------

            $('#checkTratt').click(function () {
                SelezionaDeselezionaFito();
            });


            if ($('#checkTratt').children('input').is(':checked')) {
                $('#checkFito').children('input').removeAttr('disabled');
            }

            //            $('#checkMacch').click(function () {
            //                SelezionaDeselezionaSoloDifesa();
            //            });

            //           SelezionaDeselezionaFito();
            //            SelezionaDeselezionaSoloDifesa();
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">
    <input id="txtReport" type="hidden" runat="server">
    <table aria-hidden="true" id="Tabella_Filtro" runat="server">
        <tr>
            <td style="width: 450px;">
            </td>
            <td style="width: 150px;">
            </td>
            <td style="width: 350px;">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table aria-hidden="true" class="boxColore" style="width: 600px; height: 200px;" runat="server" id="Riepilogo_Azienda">
                    <tr>
                        <td style="padding-left: 5px;">
                            <b>RIEPILOGO</b>
                            <br />
                            E' stata richiesta la stampa della Scheda di Campagna per l'impresa :
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b style="margin-left: 5px;">Partita IVA</b>:
                            <asp:Label ID="Lbl_Piva" runat="server" Style="margin-left: 5px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b style="margin-left: 5px;">Ragione Sociale</b>:
                            <asp:Label ID="Lbl_Impresa" runat="server" Style="margin-left: 5px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b style="margin-left: 5px;">Centro Aziendale</b>:
                            <asp:Label ID="Lbl_Centro" runat="server" Style="margin-left: 5px; max-width: 400px;
                                display: none" />
                            <asp:DropDownList ID="Cmb_CentroAziendale" runat="server" Style="max-width: 400px;"
                                CssClass="form-control" data-live-search="true" aria-describedby="lbl_Centro">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;">
                            In particolare si è scelto di stampare i dati sulla specie vegetale
                            <asp:Label ID="Lbl_Specie_Vegetale" runat="server" Style="margin-left: 5px; max-width: 400px;
                                display: none" Font-Bold="True" />
                            <asp:DropDownList ID="Cmb_Specie" runat="server" Style="max-width: 400px;" CssClass="form-control"
                                data-live-search="true" aria-describedby="Lbl_Specie_Vegetale">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;">
                            Il periodo di attivita' di tale centro va <b>dal </b>
                            <asp:Label ID="Lbl_Validita_Inizio" runat="server" Style="margin-left: 5px;" Font-Bold="True" />
                            <b>&nbsp;al </b>
                            <asp:Label ID="Lbl_Validita_Fine" runat="server" Style="margin-left: 5px;" Font-Bold="True" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px; width: 650px;">
                            Impostando una data esterna all'intervallo, si potrebbe ottenere una stampa nulla.
                        </td>
                    </tr>
                </table>
                <table aria-hidden="true" class="boxColore" style="width: 600px;" runat="server" id="Riepilogo_Aziende">
                    <tr>
                        <td style="padding-left: 5px;">
                            <b>RIEPILOGO</b>
                            <br />
                            E' stata richiesta la stampa della Scheda di Campagna per le imprese :
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="DataGrid_Imprese" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                CssClass="ui-widget-content" Style="margin-top: 5px; background-image: none;
                                height: 200px">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelezionaTutteImprese" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ChkSeleziona" runat="server" CssClass="ChkSelezionaImpresa" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle Width="20px" />
                                        <FooterStyle Width="20px" />
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Piva" HeaderText="Partita IVA"></asp:BoundField>
                                    <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale"></asp:BoundField>
                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table aria-hidden="true" class="boxColore" style="width: 350px; height: 200px;">
                    <tr>
                        <td style="padding-left: 5px;" style="width: 350px;">
                            <b>OPZIONI DI ARROTONDAMENTO</b>
                            <br />
                            Seleziona il tipo di arrotondamento
                            <asp:RadioButtonList ID="Rbl_Arrotondamento" runat="server">
                                <asp:ListItem Value="-1">Nessuno</asp:ListItem>
                                <asp:ListItem Value="0">Unit&#224;</asp:ListItem>
                                <asp:ListItem Value="1">1 Decimale</asp:ListItem>
                                <asp:ListItem Value="2">2 Decimali</asp:ListItem>
                                <asp:ListItem Value="3" Selected="True">3 Decimali</asp:ListItem>
                                <asp:ListItem Value="4">4 Decimali</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <asp:UpdatePanel ID="Update_Sezioni" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <table aria-hidden="true" class="boxColore" style="width: 450px; height: 450px;">
                            <tr>
                                <td colspan="2">
                                    <b>Selezionare le sezioni che si desiderano stampare :</b>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <asp:CheckBoxList ID="CBL_Sezioni" runat="server" Width="190px">
                                        <asp:ListItem Value="a">Frontespizio</asp:ListItem>
                                        <asp:ListItem Value="s">Personale con Patentino</asp:ListItem>
                                        <asp:ListItem Value="o">Dati Catastali</asp:ListItem>
                                        <asp:ListItem Value="p">Semine / Trapianti</asp:ListItem>
                                        <asp:ListItem Value="b">Fertilizzazioni</asp:ListItem>
                                        <asp:ListItem Value="c">Trattamenti</asp:ListItem>
                                        <asp:ListItem Value="t" Enabled="false">Fitoregolatori</asp:ListItem>
                                        <asp:ListItem Value="d">Osservazioni Fasi Fenologiche</asp:ListItem>
                                        <asp:ListItem Value="e">Trappole Installate</asp:ListItem>
                                        <asp:ListItem Value="f">Rilievi Avversit&#224; nelle Trappole</asp:ListItem>
                                        <asp:ListItem Value="g">Rilievi Avversit&#224; in Campo</asp:ListItem>
                                        <asp:ListItem Value="h">Irrigazione</asp:ListItem>
                                        <asp:ListItem Value="i">Altre Operazioni Colturali</asp:ListItem>
                                        <asp:ListItem Value="l">Indici di Maturit&#224;</asp:ListItem>
                                        <asp:ListItem Value="n">Rilievo Produzione e Data Raccolta</asp:ListItem>
                                        <asp:ListItem Value="m">Piogge</asp:ListItem>
                                        <asp:ListItem Value="q">Informazioni/Dichiarazioni</asp:ListItem>
                                        <asp:ListItem Value="y" Enabled="false">Manutenzione Macchinari</asp:ListItem>
                                        <%--FUTURO PER STAMPARE TUTTE LE MACCHINE -- anche js in alto
                                        <asp:ListItem Value="z" Enabled="false">Solo Macchine Difesa</asp:ListItem>--%>
                                        <asp:ListItem Value="r" Enabled="false">Visite Ispettive</asp:ListItem>
                                        <asp:ListItem Text="Trattamenti Post Raccolta" Value="1" />
                                    </asp:CheckBoxList>
                                    <!--					            <asp:CheckBoxList id="CBL_SezioniVeneto" runat="server" Visible="False" >
						            <asp:ListItem Value="a">Scheda A</asp:ListItem>
						            <asp:ListItem Value="b">Scheda B</asp:ListItem>
						            <asp:ListItem Value="c">Scheda C</asp:ListItem>
						            <asp:ListItem Value="d">Scheda D</asp:ListItem>
						            <asp:ListItem Value="e">Scheda E</asp:ListItem>
					            </asp:CheckBoxList>
-->
                                    <br />
                                    <asp:RadioButtonList ID="RBL_SezioniVeneto" runat="server" Visible="False">
                                        <asp:ListItem Value="a">Scheda A</asp:ListItem>
                                        <asp:ListItem Value="b">Scheda B</asp:ListItem>
                                        <asp:ListItem Value="c">Scheda C</asp:ListItem>
                                        <asp:ListItem Value="d">Scheda D</asp:ListItem>
                                        <asp:ListItem Value="e">Scheda E</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td rowspan="2" valign="top">
                                    <asp:ImageButton ID="ImgBtnSelezionaTutte" runat="server" ImageUrl="../../AB_Immagini/icone24/ValidazioneSI_24.ico">
                                    </asp:ImageButton>
                                    <span>Seleziona tutte le Sezioni</span>
                                    <br />
                                    <asp:ImageButton ID="ImgBtnEliminaSelezione" runat="server" ImageUrl="../../AB_Immagini/icone24/ValidazioneNO_24.ico">
                                    </asp:ImageButton>
                                    <span>Deseleziona tutte le Sezioni</span>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:CheckBox ID="Chk_LogoRegione" runat="server" Width="250px" Text="Stampa il logo della regione">
                                    </asp:CheckBox>
                                    <asp:DropDownList TabIndex="7" ID="Cmb_Regioni" runat="server" Height="22px" Width="184px"
                                        AutoPostBack="True">
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="Lbl_ImpostaSezioni" runat="server">Imposta sezioni da stampare anche in caso siano vuote :</asp:Label>
                                    <img src='../../AB_Immagini/icone16/plus.png' alt='Imposta Sezioni Vuote' id='BtnStampeVuote'
                                        style="margin-left: 10px" runat="server" />
                                    <asp:TextBox ID="Txt_StampeVuote" runat="server" BackColor="PaleTurquoise" Height="1px"
                                        Width="1px" CssClass="Testo_12_Blue_Bold" MaxLength="10" BorderStyle="None"></asp:TextBox>
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <asp:CheckBox ID="Chk_Avversita_Qta" runat="server" Text="Visualizza le Quantità rilevate in Campo" Visible="false"
                                        Width="250px"></asp:CheckBox>
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <asp:CheckBox ID="Chk_Tutte_Raccolte" runat="server" Text="Visualizza tutte le Raccolte"
                                        Width="250px"></asp:CheckBox>
                                    <br />
                                    <asp:CheckBox ID="Chk_Qta_Raccolte" runat="server" Width="250px" Text="Visualizza le Quantità raccolte">
                                    </asp:CheckBox>
                                    <br />
                                    <asp:CheckBox ID="Chk_Data_UltimaRaccolta" runat="server" Text="Visualizza la Data Ultima Raccolta">
                                    </asp:CheckBox>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td colspan="2" valign="top" style="padding-left: 5px;">
                <table aria-hidden="true" class="boxColore" style="width: 500px; height: 200px;">
                    <tr>
                        <td style="width: 300px; vertical-align: top;">
                            <asp:CheckBox ID="Chk_Priorita_ColturePrecedenti" runat="server" Text="Visualizza in Scheda le colture inserite manualmente">
                            </asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_VisualizzaTipologieVarietali" runat="server" CssClass="Testo_08_Blue"
                                Text="Visualizza Tipologie Varietali"></asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_VisualizzaCapitolatoPrivato" runat="server" CssClass="Testo_08_Blue"
                                Text="Visualizza Capitolato Privato" Visible="False"></asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_VisualizzaFinalita" runat="server" CssClass="Testo_08_Blue"
                                Text="Visualizza Finalità"></asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_RaggruppaXCampo" runat="server" Visible="False" Text="Raggruppa per Intervento">
                            </asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_VisualizzaAcquaHa" runat="server" Text="Visualizza Acqua/ha">
                            </asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_StampaAnnoImpiantoPluriennali" runat="server" Text="Stampa anno impianto colture pluriennali" Checked="true">
                            </asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_MostraValoriSignificativiNeiRilievi" runat="server" Text="Mostra nei rilievi anche valori a zero" Checked="true">
                            </asp:CheckBox><br />
                            <asp:CheckBox ID="Chk_MostraDataOdiernaStampa" runat="server" Text="Mostra data di stampa">
                            </asp:CheckBox><br />
                            <asp:CheckBox visible="false" checked="false" ID="Chk_MostraFirmaODC" runat="server" Text="Mostra firma ODC">
                            </asp:CheckBox><br />
                        </td>
                        <td style="width: 250px; vertical-align: top;">
                            <div id="DivRotazione" runat="server">
                                <asp:ImageButton ID="Imgbtn_RotazioneColturale" runat="server" ImageUrl="../../AB_Immagini/icone32/registro.ico">
                                </asp:ImageButton>
                                <span>Inserisci Manualmente le Colture Precedenti</span>
                            </div>
                            <br />
                            <br />
                            <div id="DivGlobalGap" runat="server" visible="false">
                                <asp:ImageButton ID="ImgBtn_DatiGLOBALGAP" runat="server" ImageUrl="../../AB_Immagini/icone32/registro.ico">
                                </asp:ImageButton>
                                <span>Inserisci i dati GLOBAL - GAP</span>
                                <br />
                                <br />
                                <asp:CheckBox ID="CheckTitoloGlobal" Checked="true" runat="server" Visible="false"
                                    Text="Visualizza titolo GLOBAL - GAP"></asp:CheckBox>
                            </div>
                            <br />
                            <div id="DivRegolamenti" runat="server" visible="false">
                                <asp:CheckBox ID="Chk_RegCondizionalita" runat="server" CssClass="Testo_08_Blue"
                                    Text="Stampa Rif. Reg. Condizionalità" Checked="true"></asp:CheckBox>
                                <br />
                                <asp:CheckBox ID="Chk_RegPSR" runat="server" CssClass="Testo_08_Blue" Text="Stampa Rif. Reg. P.S.R."
                                    Checked="true"></asp:CheckBox>
                                <br />
                                <asp:CheckBox ID="Chk_RegMisura10" runat="server" CssClass="Testo_08_Blue"
                                    Text="Stampa Rif. Misura 10 lotta Integrata" Checked="true"></asp:CheckBox>
                                <br />
                                <br />
                            </div>
                            <div id="DivOrdinamento" runat="server" visible="false">
                                <asp:RadioButtonList ID="Rbl_Ordinamento" runat="server">
                                    <asp:ListItem Selected="True" Value="0">Ordina per data</asp:ListItem>
                                    <asp:ListItem Value="1">Ordina per coltura</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="middle">
                            <div id="DivTempoRientro" runat="server" visible="false">
                                <div style="float: left; vertical-align: middle;">
                                    <b>Tempo di rientro</b></div>
                                <div style="float: left;">
                                    <asp:TextBox ID="Txt_TempoRientro" runat="server" Width="32px" CssClass="txtUI" MaxLength="25"
                                        BorderStyle="None">48</asp:TextBox>
                                </div>
                                <div style="float: left;">
                                    <b>ore</b></div>
                                <div class="clear" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:RadioButtonList ID="Rbl_Superfici" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="i" Selected="True">Stampa la sup. impianti</asp:ListItem>
                                <asp:ListItem Value="a">Stampa la sup. appezzamenti</asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <asp:CheckBox ID="Chk_IntestazioneOrgref" runat="server" Text="Imposta l'organismo referente come intestatario (Scheda Interventi Agronomici)">
                            </asp:CheckBox>
                        </td>
                    </tr>
                    <tr id="RigaFascicoli" runat="server" visible="false">
                        <td colspan="2">
                            <b>Scheda Validazione :</b><asp:DropDownList ID="Cmb_Fascicoli" runat="server" Height="22px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="Pannello_Date" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <table aria-hidden="true" class="boxColore" style="width: 500px; height: 75px; margin-top: 5px; vertical-align: top;">
                            <tr>
                                <td colspan="2">
                                    <asp:RadioButtonList ID="rblStampa" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="0">Giorno</asp:ListItem>
                                        <asp:ListItem Value="1" Selected="True">Intervallo di tempo</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td id="Cella_Giorno" runat="Server">
                                    <asp:TextBox ID="TxtStampa" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                        ToolTip="Data di riferimento" Width="77px">
                                    </asp:TextBox>
                                </td>
                                <td id="Cella_Frecce" runat="server">
                                    <asp:ImageButton ID="ImgBtn_AnnataPrecedente" runat="server" Height="32px" Width="32px"
                                        ImageUrl="../../AB_Immagini/icone32/frecciasx.ico" ToolTip="Annata Precedente">
                                    </asp:ImageButton>
                                    <asp:ImageButton ID="ImgBtn_AnnataSuccessiva" runat="server" Height="32px" Width="32px"
                                        ImageUrl="../../AB_Immagini/icone32/frecciadx.ico" ToolTip="Annata Successiva">
                                    </asp:ImageButton>
                                </td>
                            </tr>
                            <tr id="Riga_Intervallo" runat="server">
                                <td>
                                    <b>Dal</b>
                                    <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                        Style="margin-left: 5px;" ToolTip="Data di riferimento" Width="77px">
                                    </asp:TextBox>
                                </td>
                                <td>
                                    <b>Al</b>
                                    <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                        Style="margin-left: 5px;" ToolTip="Data di riferimento" Width="77px">
                                    </asp:TextBox>
                                </td>
                                <td colspan="2">
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <table aria-hidden="true" class="boxColore" style="width: 500px; height: 75px; margin-top: 5px;">
                    <tr>
                        <td colspan="2">
                            <b>Stampa di Campagna:</b>
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td align="center" style="display: none">
                            Verifica Conformita
                            <br />
                            <br />
                            <asp:ImageButton ID="ImgBtn_VerificaConformita" CssClass="btn_per_load" runat="server"
                                ImageUrl="../../AB_Immagini/Icone32/certificatosi.ico"></asp:ImageButton>
                        </td>
                        <td align="center">
                            Visualizza elenco report
                            <br />
                            <br />
                            <asp:ImageButton ID="ImgBtn_Elenco" CssClass="btn_per_load" runat="server" ImageUrl="../../AB_Immagini/Icone32/PianoConti.ico">
                            </asp:ImageButton>
                        </td>
                        <td align="center">
                            Stampa i dati selezionati
                            <br />
                            <br />
                            <asp:ImageButton ID="ImgBtn_Stampa" CssClass="btn_per_load" runat="server" ImageUrl="../../AB_Immagini/Icone32/stampa.ico">
                            </asp:ImageButton>
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RblStampaProva" runat="server" AutoPostBack="false" RepeatDirection="Vertical"
                                ToolTip="La stampa definitiva prevede il salvataggio del file" BorderStyle="None">
                                <asp:ListItem Value="0" Selected="True">Stampa di prova</asp:ListItem>
                                <asp:ListItem Value="1">Stampa definitiva</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
                <table aria-hidden="true" class="boxColore" style="width: 500px; height: 75px; margin-top: 5px;">
                    <tr>
                        <td colspan="2">
                            <b>Stampe di Magazzino:</b>
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td>
                            <asp:RadioButtonList ID="RblStampaMagazzino" runat="server" AutoPostBack="false"
                                RepeatDirection="Vertical" BorderStyle="None">
                                <%--<asp:ListItem Value="9">Scheda Movimenti</asp:ListItem>
                                <asp:ListItem Value="10">Scheda Giacenze</asp:ListItem>--%>
                                <asp:ListItem Value="11">Scheda Fertilizzanti</asp:ListItem>
                                <asp:ListItem Value="12" Selected="True">Scheda Prodotti Fitosanitari</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td align="center">
                            <asp:DropDownList ID="ddlMagazzini" runat="server" Style="max-width: 280px;">
                            </asp:DropDownList>
                            <br />
                            <br />
                            <asp:ImageButton ID="ImgBtn_StampaMagazzino" CssClass="btn_per_load" runat="server"
                                ImageUrl="../../AB_Immagini/Icone32/stampa.ico"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table aria-hidden="true">
        <tr>
            <td>
                <table aria-hidden="true" id="Tabella_Rotazione" runat="server" class="BoxColore">
                    <tr>
                        <td style="width: 600px">
                            N.B. Le Colture inserite in questa tabella verranno visualizzate nella Scheda di
                            Campagna solamente se in archivio non esistono dati relativi a Colture Precedenti...
                        </td>
                        <td style="width: 50px">
                            <asp:ImageButton ID="ImgBtnAnnulla2" runat="server" BackColor="#E6F4FF" ImageUrl="~/AB_Immagini/Icone32/Esci.bmp">
                            </asp:ImageButton>
                        </td>
                        <td style="width: 275px">
                            Torna al filtro
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Gli impianti visualizzati, sono impianti attivi nell'intervallo temporale selezionato
                            nel filtro di stampa
                        </td>
                        <td>
                            <asp:ImageButton ID="Imgbtn_AssegnaColture" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico">
                            </asp:ImageButton>
                        </td>
                        <td>
                            Salva i dati per Stamparli poi..
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="coveragesContainer">
                            <asp:Panel ID="Pannello_Rotazione" runat="server" ScrollBars="Vertical" BackColor="WhiteSmoke">
                                <asp:UpdatePanel ID="Update_Rotazione" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="GridView_Rotazione" runat="server" AutoGenerateColumns="False"
                                            CellPadding="1" CellSpacing="2" CssClass="ui-widget-content ">
                                            <Columns>
                                                <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" />
                                                <asp:BoundField DataField="Campo_Des" HeaderText="Campo" />
                                                <asp:BoundField DataField="App_Nome" HeaderText="App." />
                                                <asp:BoundField DataField="Veg_Des" HeaderText="Specie" />
                                                <asp:BoundField DataField="Cul_Des" HeaderText="Varietà" />
                                                <asp:BoundField DataField="Dest_Uso" HeaderText="Dest. Uso" />
                                                <asp:BoundField DataField="Sup_Imp" HeaderText="Sup.Imp. [ha]">
                                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Validita" HeaderText="Validità Impianto" />
                                                <asp:TemplateField HeaderText="Coltura Precedente">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Coltura1" Width="90" runat="server" CssClass="SupIntersezione txtui" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Coltura Precedente 2 anno">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Coltura2" Width="90" runat="server" CssClass="SupIntersezione txtui" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Coltura Precedente 3 anno">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Coltura3" Width="90" runat="server" CssClass="SupIntersezione txtui" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Coltura Precedente 4 anno">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Coltura4" Width="90" runat="server" CssClass="SupIntersezione txtui" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table aria-hidden="true" id="Tabella_GlobalGap" runat="server" class="BoxColore">
                    <tr>
                        <td style="width: 650px">
                            <b style="margin-left: 5px;">Revisione</b>:
                            <asp:TextBox ID="TxtRevisione" runat="server" Style="margin-left: 5px; width: 500px" />
                        </td>
                        <td style="width: 50px">
                            <asp:ImageButton ID="ImgBtn_SalvaDatiGLOBALGAP" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                                Style="margin-left: 15px;" />
                        </td>
                        <td style="width: 100px">
                            Salva i dati per stamparli poi...
                        </td>
                        <td style="width: 50px">
                            <asp:ImageButton ID="ImgBtnAnnulla3" runat="server" ImageUrl="~/AB_Immagini/Icone32/Esci.bmp"
                                Style="margin-left: 15px;" />
                        </td>
                        <td style="width: 100px">
                            Torna al filtro
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="dialogSezioniVuote" title="Sezioni Vuote">
        <asp:UpdatePanel ID="UpdatePanelSezioniVuote" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div>
                    <!--- Bottoni Salvattaggio e Annulla -->
                    <asp:Button ID="SalvaSezioniVuote" runat="server" Style="display: none;" />
                    <asp:Button ID="AnnullaSezioniVuote" runat="server" Style="display: none;" />
                    <asp:CheckBoxList ID="ListaSezioniVuote" runat="server">
                        <asp:ListItem Text="Frontespizio" Value="a" />
                        <asp:ListItem Text="Fertilizzazioni" Value="b" />
                        <asp:ListItem Text="Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori"
                            Value="c" />
                        <asp:ListItem Text="Osservazioni Fasi Fenologiche" Value="d" />
                        <asp:ListItem Text="Trappole Installate" Value="e" />
                        <asp:ListItem Text="Rilievi Avversità nelle Trappole" Value="f" />
                        <asp:ListItem Text="Rilievi Avversità in Campo" Value="g" />
                        <asp:ListItem Text="Irrigazione" Value="h" />
                        <asp:ListItem Text="Altre Operazioni Colturali" Value="i" />
                        <asp:ListItem Text="Indice di Maturità e Raccolta" Value="l" />
                        <asp:ListItem Text="Piogge" Value="m" />
                        <asp:ListItem Text="Rilievo Produzione e data Raccolta" Value="n" />
                        <asp:ListItem Text="Trattamenti Post Raccolta" Value="1" />
                    </asp:CheckBoxList>
                    <!--- Modificare di seguito per l'ultima lettera registrata x -->
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
