<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Operazione.master"
    ValidateRequest="false" CodeBehind="Raccolta.aspx.vb" Inherits="AgronicaDomandaIrrigua.Raccolta" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.ui.timepicker.js?" & Application("GiasVersioneCorrente").ToString) %>"> </script>
    <link href="../Styles/jquery.ui.timepicker.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .classComboFertilizzanti .ui-autocomplete-input
        {
            min-width: 525px;
            width: 90%;
        }
        .change_per_load
        {
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {

        });

        function pageLoad() {

            //al caricamento della pagina, ma prima di $(window).load
            //clicktab();
            // alert();
        }

        $("#<%=tabs.ClientID %>").tabs({
            activate: function (event, ui) {
            }
        });

        //        $(function () {
        //            alert(1);
        //            $("#tabs").tabs();
        //        });

        //        $(window).on("load", function () {
        //            alert(2);
        //            $("#tabs").tabs({
        //                activate: function (event, ui) {
        //                    //ad ogni attivazione di un tab
        //                    //potrei richiamare clicktab() invece di clicktab2(); poi
        //                }
        //            });

        //            //            $("a[href$=#tabs-2]").click(function () {
        //            //                //clicktab2();
        //            //            });
        //            //            $("a[href$=#tabs-3]").click(function () {
        //            //                //clicktab3();
        //            //            });

        //            //alla prima inizializzazione visualizzo gli elementi per il tab 2
        //            //clicktab2();
        //        });




        //per la Gestione dell'Eliminazione 
        function DoPostBack_Combo_Slave($_combo, valoreOpt) {
            //postBack Fertilizzanti
            if ($_combo.attr("id").endsWith("ComboFertilizzanti")) {

            }   
        }
          
        function DoPostBack_ControlliSiNo(key) {
            if (key == 'Giacenze') {
                $("#<%=Giacenza_SI_NO.ClientID %>").val("OK");
                $("#<%=Master_Operazione.Property_ImgBtn_Salva.ClientID %>").click();
            }
            if (key == 'Salva') {
                $("#<%=HiddenVarie.ClientID %>").val("OK");
                $("#<%=Master_Operazione.Property_ImgBtn_Salva.ClientID %>").click();
            }

        }

        /////////////////////////////////////////////////////////////
        //per collegarsi al sito del profitosan
        function Info() { }
        function AcquaTotChecked() { }
        function Abilita_Disabilita_ACQUA() { }
        function DoseHAChecked() { }
        function QtaTOTChecked() { }
        function Abilita_Disabilita_DOSI() { }
        function CalcolaCostiAccessori() {
            var flag = $(".CostiAperti").children().is(':checked');
            if (flag == true) {
                var sup = $('#<%=Txt_Suptrattata.ClientId %>').val().replace(',', '.');
                if (sup > 0) {
                    aggiornaCostiSuServer(sup);
                    $('.GridViewCostiAccessoriVisibili').find('.UdmCosti').each(function () {
                        var udm = $(this).val();
                        if (udm == 1) {
                            InserisciQtaCosti($(this), sup)
                            var CostoUnitario = ValoreCostoUnitario($(this));
                            if (CostoUnitario != 0) {
                                var tot = CostoUnitario * sup
                                InserisciCosto($(this), tot);
                            }
                        }
                        else if (udm == 2) {

                            var minuti = Number($(this).parent().parent().children('.ore').html());
                            minuti = minuti * 60;
                            minuti = minuti + Number($(this).parent().parent().children('.minuti').html());
                            minuti = minuti * sup;

                            //Grilli: ho aggiunto l'IF perché altrimenti ripulisce sempre tutto anche se non è stato impostato
                            if (minuti > 0) {
                                var ore = Math.floor(minuti / 60);
                                var resto = minuti - (ore * 60);
                                // InserisciQtaCosti
                                var OreDecimal = ore.toString() + "," + (Math.floor(resto * 100 / 60)).toString()
                                $(this).parent().parent().find('.QtaCosti').val(OreDecimal);

                                var CostoUnitario = ValoreCostoUnitario($(this));
                                if (CostoUnitario != 0) {
                                    var tot = CostoUnitario * Number(OreDecimal.replace(",", "."));
                                    InserisciCosto($(this), tot);
                                }
                            }
                        };
                    });
                };
            };
        }
        function InserisciQtaCosti(Oggetto, valore) {
            var sup = roundNumber(valore, 4) + "";
            sup = sup.replace(".", ",");
            $(Oggetto).parent().parent().find('.QtaCosti').val(sup);
        }
        function InserisciCosto(Oggetto, valore) {
            var tot = roundNumber(valore, 4) + "";
            tot = tot.replace(".", ",");
            $(Oggetto).parent().parent().find('.Costo').html(tot);
        }
        function ValoreCostoUnitario(Oggetto) {
            return $(Oggetto).parent().parent().children('.CostoUnitario').html().replace(',', '.');
        }
        function ValoreUDMCostoUnitario(Oggetto) {
            return $(Oggetto).parent().parent().children('.UdmCosti').val();
        }
        function aggiornaCostiSuServer(sup) {
            var Attesa;
            $.ajax({
                type: "POST",
                url: "trattamenti_2.aspx/Update_Sup_Costi",
                data: "{ sup: '" + sup + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
            });
        }
        function SupTrattata() {
            return $('#<%=Txt_Suptrattata.ClientId %>').val().replace(',', '.');
        }
        function InserisciSupTrattata(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_Suptrattata.ClientId %>').val(app);
        }
        function SupTotale() {
            return $('#<%=Txt_SupSelezionata.ClientId %>').val().replace(',', '.');
        }
        function InserisciSupTotale(valore) {
            var app = roundNumber(valore, 4) + "";
            app = app.replace(".", ",");
            $('#<%=Txt_SupSelezionata.ClientId %>').val(app);
        }
        function AcquaTot() {
        }
        function InserisciAcquaTot(valore) {
        }
        function AcquaHA() {
        }
        function InserisciAcquaHA(valore) {
        }
        function DoseHA() {
        }
        function InserisciDoseHA(valore) {
        }
        function DoseHL() {
        }
        function InserisciDoseHL(valore) {
        }
        function TotHA() {
        }
        function InserisciTotHA(valore) {
        }
        function TotHL() {
        }
        function InserisciTotHL(valore) {
        }
        function AggiornaACQUA() {
        }
        function AggiornaDOSI() {
        }
        function AcquaTot_Keyup() {
        }
        function AcquaHA_Keyup() {
        }
        function AggiornaDopo_SupTrattata() {
            //Agggiorno i costi accessori
            CalcolaCostiAccessori();
        }
        function PulisciGrigliaAv_GrAv() {
        }


        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlacexTRATTAMENTI" runat="server">
    <input type="hidden" id="HiddenVarie" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <!-- hidden per si no  -->
    <asp:UpdatePanel ID="updateGiacenze_si_no" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="Giacenza_SI_NO" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Dati Superficie-->
    <table width="100%" aria-hidden="true">
        <tr>
            <td width="20%" style="background-color: #cccccc;">
                <asp:Label ID="lblSupHaSelezionata" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"><b>Sup. [Ha]</b> Selezionata:</asp:Label>
            </td>
            <td width="10%" style="background-color: #cccccc;">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficie"
                    id="Txt_SupSelezionata" disabled="disabled" readonly="readonly" value="0" />
            </td>
            <td style="text-align: right">
            </td>
            <td>
            </td>
            <td style="width: 10%">
            </td>
        </tr>
        <tr>
            <td style="background-color: #FFC0C0; ">
                <asp:Label ID="lblSupHaTrattata" runat="server" meta:resourcekey="lblSupHaTrattataResource1"><b>Sup. [Ha]</b> Trattata:</asp:Label>
            </td>
            <td style="background-color: #FFC0C0; ">
                <input type="text" runat="server" style="width: 60px" class="txtUI SommaSuperficieTrattata"
                    id="Txt_SupTrattata" value="0" />
            </td>
            <td>
            </td>
            <td style="text-align: right">
            </td>
            <td>
            </td>
        </tr>
    </table>
    <!-- DATI -->
    <div class="clear">
    </div>
    <div id="tabs" runat="server">
        <ul>
            <li><a href="#tabs-1">
                <asp:Label ID="lbltab1" runat="server" Text="Raccolta"></asp:Label></a></li>
            <li><a href="#tabs-2">
                <asp:Label ID="lbltab2" runat="server" Text="Campionatura"></asp:Label></a></li>
            <li><a href="#tabs-3">
                <asp:Label ID="lbltab3" runat="server" Text="Post Raccolta"></asp:Label></a></li>
        </ul>
        <div id="tabs-1">
            <div class="box" style="margin: 10px;">
                <div style="width: 100%;">
                    <asp:UpdatePanel ID="UpdatePanelTab1" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <!-- Riga 1 -->
                            <div  >
                                <div style="width: 100%; margin-top: 5px;">
                                    <asp:Label runat="server" ID="Label2" BackColor="#EAF4FD">Opzioni</asp:Label>
                                </div>
                                
                                 <div class="box" style="float: left; margin-top: 5px; width:1000px ;"   runat="server"  id="Opzioni_Impianti" >
                                        <div class="valoriinput" style="float: left; width:380px " >
                                             <asp:RadioButtonList ID="rbl_Chiusura" runat="server">
                                                <asp:ListItem Text="Lasciare gli Impianti attivi in previsione di futuri interventi colturali" Value="0" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Chiusura Esercizi" Value="1" Selected="False"></asp:ListItem>
                                                <asp:ListItem Text="Chiusura Impianti Colturali ed Esercizi" Value="2" Selected="False"></asp:ListItem>
                                                <asp:ListItem Text="Chiusura Appezzamenti, Impianti Colturali ed Esercizi" Value="3" Selected="False"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div class="valoriinput" style=" padding-top: 40px; width:520px;"     >
                                            <asp:CheckBox ID="Chk_NuovoEsercizio" Text="e conseguente Apertura nuovi Esercizi"
                                                 Checked="true" runat="server" ></asp:CheckBox>
                                                <br />
                                                   <asp:CheckBox ID="Chk_NuovoImpianto" Text="e conseguente Apertura nuovi Impianti (Terreno Nudo - Anticipazioni Colturali)"
                                                Checked="true" runat="server" ></asp:CheckBox>
                                        </div>
                                       
                                    </div>
                                      <div style=" clear:both"></div>

                                
                                <div class="box" id="DivOpzioni" runat="server">
                                    <div class="" style="float: left; width: 445px;">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="Label5" runat="server">Ripartizione Raccolto</asp:Label>
                                        </div>
                                        <div class="">
                                            <asp:RadioButtonList ID="RadioButtonListRipartizione" runat="server" AutoPostBack="true">
                                                <asp:ListItem Text="Automatica sulle superici impiegate" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Automatica Sulle Piante/Impianto" Value="2" Selected="False"></asp:ListItem>
                                                <asp:ListItem Text="Manuale" Value="3" Selected="False"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 350px;">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="lbl1" runat="server">Modalità</asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <asp:RadioButtonList ID="RadioButtonListMeccanicaManuale" runat="server">
                                                <asp:ListItem Text="Raccolta Meccanica" Selected="True" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Raccolta Manuale" Selected="False" Value="1"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 250px;">
                                        <div class="valoriinput">
                                           <%-- <asp:CheckBox ID="CheckBoxCarenza" Text="Verifica Tempi di Carenza" runat="server" />--%>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                            <!-- Riga 2 -->
                            <div id="DivMagazzino" runat="server">
                                <div style="width: 100%; margin-top: 5px;">
                                    <asp:Label runat="server" ID="Label3" BackColor="#EAF4FD">Magazzino</asp:Label>
                                </div>
                                <div class="box">
                                    <div class="" style="float: left; width: 400px;">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="Label23" runat="server">Magazzino Destinazione </asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <cc1:ComboMagazzini ID="ComboMagazzinoDestinazione" runat="server" Fabbricato_Cod="0"
                                                Flag_CodCentroFabbricato="True" Flag_GestioneMagazziniImpresaPadre="False" Sa_Cod="0"
                                                TipoMagazzino="20" />
                                        </div>
                                    </div>
                                    <div style="float: left; width: 190px;">
                                        <div class="descrizione" style="width: 70px">
                                            <asp:Label ID="lbltxt_DataMagazzino" runat="server">Data Ingresso</asp:Label></div>
                                        <div class="valoriinput" style="width: 90px">
                                            <asp:TextBox ID="txt_DataMagazzino" Style="width: 90px" runat="server" CssClass="txtUI datepicker"
                                                meta:resourcekey="txt_DataMagazzinoResource1"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 150px;">
                                        <div class="descrizione" style="width: 70px">
                                            <asp:Label ID="lbltxt_OraMagazzino" runat="server">Ora Ingresso</asp:Label></div>
                                        <div class="valoriinput" style="width: 55px">
                                            <asp:TextBox ID="txt_OraMagazzino" CssClass="txtUI datepicker" runat="server" Style="width: 50px">00:00</asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 250px;">
                                        <div class="descrizione" style="width: 76px">
                                            <asp:Label ID="Label1" runat="server">Lotto Di Produzione</asp:Label></div>
                                        <div class="valoriinput" style="width: 76px">
                                            <asp:TextBox ID="TextBoxLotto" CssClass="txtUI" runat="server" Width="150px"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 180px;">
                                        <div class="valoriinput" style="width: 178px">
                                            <asp:RadioButtonList ID="RadioButtonNomeLotto" runat="server" AutoPostBack="true">
                                                <asp:ListItem Text="Manuale" Value="0" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Genera dalla data" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Genera Id Univoco" Value="2"></asp:ListItem>
                                            </asp:RadioButtonList>
                                            <%--                                            <asp:CheckBox ID="CheckBoxNomeLotto" Text="Genera il Lotto dalla data" Selected="False"
                                                runat="server" AutoPostBack="true"></asp:CheckBox>--%>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                            <!-- Riga 3 -->
                            <div id="DivProdotto" runat="server">
                                <div style="width: 100%; margin-top: 5px;">
                                    <asp:Label runat="server" ID="Label4" BackColor="#EAF4FD">Prodotto</asp:Label>
                                </div>
                                <div class="box">
                                    <div class="" style="float: left; width: 557px;">
                                        <div class="descrizione" style="width: 122px">
                                            <asp:Label ID="Label11" runat="server">Prodotto</asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <asp:DropDownList ID="cmb_Prodotto" runat="server" CssClass="txtUI" AutoPostBack="True">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 260px;">
                                        <%--                        <div class="descrizione" style="width: 80px">
                            <asp:Label ID="Label3" runat="server">Opzioni</asp:Label>
                        </div>--%>
                                        <div class="valoriinput" style="width: 250px">
                                            <asp:CheckBox ID="CheckBoxSemilavorati" Text="Lega Prodotto al Singolo Impianto"
                                                Selected="False" runat="server" AutoPostBack="true"></asp:CheckBox>
                                            <%--                            <asp:CheckBox Text="Raccolta MultiVarietale" Selected="False" ></asp:ListItem>
                                <asp:CheckBox Text="Applica Filtro Varietale" Selected="true"></asp:ListItem>--%>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <div class="" style="float: left; width: 544px;">
                                        <div class="descrizione" style="width: 122px">
                                            <asp:Label ID="Label12" runat="server">Quantità totale</asp:Label></div>
                                        <div class="valoriinput" style="width: 150px">
                                            <asp:TextBox ID="QtaTot" Width="110px" CssClass="txtUI QtaTotale" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="valoriinput" style="width: 76px">
                                            <asp:DropDownList ID="CmbUdm" runat="server" CssClass="txtUI">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                    <div>
                                        <asp:GridView ID="GridView_Rilievo" runat="server" AutoGenerateColumns="False" Width="100%"
                                            CellPadding="5" CssClass="ui-widget-content" Caption="Dati Raccolta">
                                            <Columns>
                                                <asp:BoundField DataField="Descrizione" HeaderText="Provenienza" SortExpression="Descrizione">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Sa_nome" HeaderText="Centro Aziendale" SortExpression="Sa_nome"
                                                    meta:resourcekey="BoundFieldResource1">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="App_nome" HeaderText="Appezzamento" SortExpression="App_nome"
                                                    meta:resourcekey="BoundFieldResource2">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Cul_Des" HeaderText="Varietà" SortExpression="Cul_Des"
                                                    meta:resourcekey="BoundFieldResource3">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Sup" HeaderText="Superficie [Ha]" SortExpression="Sup"
                                                    meta:resourcekey="BoundFieldResource4">
                                                    <ItemStyle CssClass="Sup" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="P_Ha" HeaderText="Piante/Ha" SortExpression="P_Ha">
                                                    <ItemStyle CssClass="P_Ha" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="P_Tot" HeaderText="N. Piante" SortExpression="P_Tot">
                                                    <ItemStyle CssClass="P_Tot" HorizontalAlign="Right" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Qta Raccolta" SortExpression="Qta_Raccolta" meta:resourcekey="TemplateFieldResource1">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Dose" runat="server" CssClass="QtaRiga txtUI" ToolTip="Quantità di prodotto raccolto"
                                                            Text='0' meta:resourcekey="Txt_DoseResource1" />
                                                    </ItemTemplate>
                                                    <ControlStyle Width="80px" />
                                                    <HeaderStyle CssClass="IntestazDose" />
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="true" HeaderText="1a Raccolta Utile" SortExpression="Carenza">
                                                    <ItemStyle CssClass="Carenza" HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                        </asp:GridView>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div id="tabs-2">
            <div class="box" style="margin: 10px;">
                <div style="width: 100%;">
                    <asp:UpdatePanel ID="UpdatePanelTab2" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            In sviluppo...
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div id="tabs-3">
            <div class="box" style="margin: 10px;">
                <div style="width: 100%;">
                    <asp:UpdatePanel ID="UpdatePanelTab3" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <!-- Riga 1 -->
                            <div id="Div1" runat="server">
                                <div style="width: 100%; margin-top: 5px;">
                                    <asp:Label runat="server" ID="Label6" BackColor="#EAF4FD">Lavorazione Post Raccolta</asp:Label>
                                </div>
                                <div class="box">
                                    <div class="box33" style="float: left; min-width: 350px">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="Label7" runat="server">Lavorazione</asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <asp:DropDownList ID="ComboLavorazione" Style="width: 250px" runat="server" CssClass="txtUI"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="" style="float: left; width: 550px;">
                                        <div class="descrizione" style="width: 190px">
                                            <asp:Label ID="Label8" runat="server">Modalità di Trasferimento del Prodotto Raccolto</asp:Label>
                                        </div>
                                        <div class="">
                                            <asp:DropDownList ID="ComboTipoTrasferimento" Enabled="false" Style="width: 250px"
                                                runat="server">
                                                <asp:ListItem Text="Carico/scarico Diretto" Value="1" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="Conferimento" Value="2" Selected="False"></asp:ListItem>
                                                <asp:ListItem Text="Scarico e DDT" Value="3" Selected="False"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                            <!-- Riga 2 -->
                            <div id="Div2" runat="server" visible="False">
                                <div style="width: 100%; margin-top: 5px;">
                                    <asp:Label runat="server" ID="Label9" BackColor="#EAF4FD">Opzioni </asp:Label>
                                </div>
                                <div class="box">
                                    <div class="box33" style="min-width: 350px">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="Label20" runat="server">UDS</asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <asp:DropDownList ID="ComboUDS" CssClass="txtUI" Style="width: 250px" runat="server"
                                                AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="box33" style="min-width: 350px">
                                        <div class="descrizione" style="width: 80px">
                                            <asp:Label ID="lbl55" runat="server">Centro Di Cura</asp:Label>
                                        </div>
                                        <div class="valoriinput">
                                            <asp:DropDownList ID="ComboCentroCura" CssClass="txtUI" Style="width: 250px" runat="server"
                                                AutoPostBack="true" />
                                        </div>
                                    </div>
                                    <div class="clear">
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <%--    <div visible="false">
        <table width="100%">
            <tr>
                <td colspan="3" align="center">
                    <div style="font-size: 10px; width: 100%" class="sfondoverde" id="Div1" runat="server">
                        <div style="width: 100px; height: 32px;">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/AB_Immagini/icone32/frecciadn.ico"
                                Style="width: 32px; display: none;" />
                            <div class="clear">
                                <br />
                                <asp:ImageButton ID="ImageButton2" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaDN.ico"
                                    Width="32px" />
                                <asp:ImageButton ID="ImageButton3" runat="server" Height="23px" ImageUrl="~/AB_Immagini/Icone32/FrecciaUP.ico"
                                    Width="32px" />
                                <br />
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <asp:UpdatePanel ID="UpdatePanelRilievo" runat="server">
            <ContentTemplate>
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label13" BackColor="#EAF4FD" meta:resourcekey="LabelInfoResource1">Rilievo e Campionatura</asp:Label>
                        </td>
                        <td>
                            <asp:Image ID="Image2" runat="server" ImageUrl="~/AB_Immagini/Icone16/Esclamativo16.ico"
                                Style="width: 14px" meta:resourcekey="ImageInfo2Resource1" />
                            <asp:Label runat="server" ID="Label14" meta:resourcekey="LabelInfo2Resource1">   Info2   </asp:Label>
                        </td>
                        <td style="text-align: right">
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
                <div style="overflow: auto; width: 100%;">
                    <div class="box50" style="min-width: 820px; min-height: 386px; width: 100%;">
                        <asp:UpdatePanel ID="UpdatePanelOpzioni" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="clear">
                                </div>
                                <!-- Riga2 -->
                                <div class="box50" style="min-width: 350px" visible="false">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label2" runat="server">Riferimenti</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:ListBox ID="ListBox1" Style="width: 90%" runat="server"></asp:ListBox>
                                    </div>
                                </div>
                                <div class="box50" style="min-width: 350px" visible="false">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label4" runat="server">Documenti Allegati</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:ListBox ID="ListBox2" Style="width: 90%" runat="server"></asp:ListBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <!-- Riga3 -->
                                <div class="box50" style="min-width: 350px">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label7" runat="server">Verifica</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:CheckBoxList ID="CheckBoxList3" runat="server">
                                            <asp:ListItem Text="Verifica Tempi Di Carenza" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Lettura Raccolte Precedenti" Selected="False"></asp:ListItem>
                                            <asp:ListItem Text="Campionatura Indefinita" Selected="true"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                                <div class="box50" style="min-width: 350px">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label8" runat="server">Opzioni</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:Button ID="BtnOperazionePref1" ToolTip="Reset Superfici Raccolta" runat="server"
                                            Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                            Width="100%" />
                                        <asp:Button ID="BtnOperazionePref2" ToolTip="Reimposta Superfici Raccolta" runat="server"
                                            Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                            Width="100%" />
                                        <asp:Button ID="BtnOperazionePref3" ToolTip="Partizionamento Verticale" runat="server"
                                            Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                            Width="100%" />
                                        <asp:Button ID="BtnOperazionePref4" ToolTip="Determina Superficie Residua" runat="server"
                                            Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                            Width="100%" />
                                        <asp:Button ID="Button1" ToolTip="Reset Campionatura" runat="server" Visible="False"
                                            Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF" Width="100%" />
                                        <asp:Button ID="Button2" ToolTip="PArtizionamento Qualitativo-Varietale" runat="server"
                                            Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                            Width="100%" />
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <!-- Riga4 -->
                                <div class="box50" style="min-width: 350px">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label9" runat="server">Paramtri Qualitativi (indici di maturità e Danni RAccolta</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:GridView ID="Paramtri_Qualitativi" runat="server" AutoGenerateColumns="False"
                                            Width="100%" CellPadding="5" CssClass="ui-widget-content" Caption="Paramtri Qualitativi">
                                            <Columns>
                                                <asp:BoundField DataField="Sa_nome" HeaderText="Centro Aziendale" SortExpression="Sa_nome"
                                                    meta:resourcekey="BoundFieldResource1"></asp:BoundField>
                                                <asp:BoundField DataField="App_nome" HeaderText="Appezzamento" SortExpression="App_nome"
                                                    meta:resourcekey="BoundFieldResource2"></asp:BoundField>
                                                <asp:BoundField DataField="Cul_Des" HeaderText="Varietà" SortExpression="Cul_Des"
                                                    meta:resourcekey="BoundFieldResource3"></asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div class="box50" style="min-width: 350px">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label10" runat="server">Documenti Allegati</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:RadioButtonList ID="RadioButtonList3" runat="server">
                                            <asp:ListItem Text="Peso Lordo Kg" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Peso Netto Kg" Selected="False"></asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox><asp:TextBox ID="TextBox5"
                                            runat="server"></asp:TextBox>
                                        <asp:Label ID="Label6" runat="server" Text="Tara Kg"></asp:Label><asp:TextBox ID="TextBox6"
                                            runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>--%>
</asp:Content>
