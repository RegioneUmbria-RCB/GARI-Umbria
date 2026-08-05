<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="Ricette_Edit.aspx.vb" Inherits="AgroAgenda_2010.Ricette_Edit" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <style>
        .classComboFormulati .ui-autocomplete-input
        {
            min-width: 100px;
        }
        
        .classComboFertilizzanti .ui-autocomplete-input
        {
            min-width: 100px;
        }
        
        .ui-dialog .ui-dialog-title
        {
            width: 100%;
        }
    </style>
    <script type="text/javascript">

        var tipoRicetta = 0;
        
        function DoPostBack_ControlliSiNo(key) {
            if (key == 'si|Disciplinari') {
                $("#<%=Disciplinari_si_no.ClientID %>").val("OK");
                $("#<%=btn_Disciplinari.ClientID %>").click();
            }
            if (key == 'no|Disciplinari') {
                $("#<%=Disciplinari_si_no.ClientID %>").val("NO");
                $("#<%=btn_Disciplinari.ClientID %>").click();
            }
        }





        function DoPostBack_Combo($_combo, valoreOpt) {

            //postBack Specie
//            if ($_combo.attr("id").endsWith("ComboOperazioni")) {
//                $("#<=BTN_ComboOperazione.ClientID %>").click();
//                $('#WaitFrame').show();
//                return;
//            }

            if ($_combo.attr("id").endsWith("Cmb_Disciplinare")) {
                $("#<%=btn_Disciplinari.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }


            if ($_combo.attr("id").endsWith("Cmb_Specie")) {
                $("#<%=btn_Specie.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }

        }


        function CambiaDisciplinare() {
            $("#<%=btn_Disciplinari.ClientID %>").click();
        }

        function CambiaOperazione() {
            $("#<%=BTN_ComboOperazione.ClientID %>").click();
        }
        function CambiaFormulati() {
            $("#<%=BTN_ComboFormulati.ClientID %>").click();
        }
        function CambiaFertilizzanti() {
            $("#<%=BTN_ComboFertilizzanti.ClientID %>").click();
        }





        $(document).ready(function () {

            //trasformo gli altri controlli in comboricerca
            $('#<%=Cmb_Disciplinare.ClientID %>').combobox();
            $('#<%=Cmb_Epoca.ClientID %>').combobox();
            $('#<%=Cmb_Specie.ClientID %>').combobox();
            $('#<%=ComboFormulati.ClientID %>').combobox();


            $('#<%=Txt_DataInizio.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $('#<%=Txt_DataFine.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $('#<%=Txt_DataInizio_Irrigazione.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $('#<%=Txt_DataFine_Irrigazione.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $('#<%=Txt_Data_Interventi.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $.datepicker.regional['it'];

            Click_su_Giorno_Intervallo();
            //Calcola_Str_Descrizione();
            Click_su_RBL_Cultivar();
            Click_su_Singole_Gruppo();


            $('#<%=RBL_Data.ClientID %>').click(function () {
                Click_su_Giorno_Intervallo();
                Calcola_Str_Descrizione();
            });

            $('#<%=RBL_Cultivar.ClientID %>').click(function () {
                Click_su_RBL_Cultivar();
            });

            $('#<%=Cmb_Specie.ClientID %>').change(
                function () {
                    Calcola_Str_Descrizione();
                });

            $('#<%=Txt_DataFine.ClientID %>').change(
                function () {
                    Calcola_Str_Descrizione();
                });

            $('#<%=Txt_DataInizio.ClientID %>').change(
                function () {

                    //                    //se cambio la data inizio, che è la sola visualizzabile, reimposto la data fine
                    //                    $('#<%=Txt_DataFine.ClientID %>').val($('#<%=Txt_DataInizio.ClientID %>').val());

                    Calcola_Str_Descrizione();

                });


            $('#<%=RBL_Avversita.ClientID %>').click(function () {
                Click_su_Singole_Gruppo();
            });




        });

        function Click_su_Giorno_Intervallo() {
            if ($('#<%=RBL_Data.ClientID %> input:checked').val() == 0) {
                $('#Cella_DataFine1').hide();
            } else {
                $('#Cella_DataFine1').show();
            }
        }



        function Click_su_Singole_Gruppo() {
            if ($('#<%=RBL_Avversita.ClientID %> input:checked').val() == 0) {
                $('#<%=GridViewGruppiAvversita.ClientID %>').hide();
                $('#<%=GridViewAvversita.ClientID %>').show();
            } else {
                $('#<%=GridViewGruppiAvversita.ClientID %>').show();
                $('#<%=GridViewAvversita.ClientID %>').hide();
            }
        }

        function Click_su_RBL_Cultivar() {
            if ($('#<%=RBL_Cultivar.ClientID %> input:checked').val() == 0) {
                $('#div_cultivar').hide();
            } else {
                $('#div_cultivar').show();
            }
        }


        function Calcola_Str_Descrizione() {
            var Testo;
            if ($('#<%=Lbl_TipoRicetta.ClientID %>').text() == "6")
                Testo = "Piano Distribuzione " + $('#<%=Cmb_Specie.ClientID %> option:selected').text();
            else
                Testo = $('#<%=Cmb_Specie.ClientID %> option:selected').text();
            if (Testo.length > 2) {
                if ($('#<%=RBL_Data.ClientID %> input:checked').val() == 0) {
                    Testo = Testo + "(" + $('#<%=Txt_DataInizio.ClientID %>').val() + ")";
                } else {
                    Testo = Testo + "(" + $('#<%=Txt_DataInizio.ClientID %>').val() + " - " + $('#<%=Txt_DataFine.ClientID %>').val() + ")";
                }
                $('#<%=Txt_Ricetta_Des.ClientID %>').val(Testo);
            }
        }

        function funz_prova(obj) {
            $('#btn_prova').click();
        }

        function funz_maschera_bottone(obj) {
            $("#<%=Btn_Nuovo_Consiglio.ClientID() %>").click();
        }

    </script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#tabTestata").on("click", function () { $("#<%=imgBtn_Testata.ClientID() %>").click(); });
            $("#tabDettagli").on("click", function () { $("#<%=imgBtn_Dettagli.ClientID() %>").click(); });
            //$("#Btn_Nuovo_Consiglio_Maschera").on("click", function () { $("#<%=Btn_Nuovo_Consiglio.ClientID() %>").click(); });
        });

       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="Script_Panel" runat="server" UpdateMode="Always">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="updatePanelDisciplinari_si_no" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="Disciplinari_si_no" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="contenitoreBody" style="padding-left: 3px; padding-right: 3px">
        <asp:UpdatePanel runat="server" ID="upButtons">
            <ContentTemplate>
                <asp:ImageButton ID="ImgBtn_Testata" runat="server" Style="display: none" />
                <asp:ImageButton ID="ImgBtn_Dettagli" runat="server" Style="display: none" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="clear">
        </div>
        <div>

            <div class="cento boxColore" style="padding: 10px; margin-bottom:10px;">
                <asp:UpdatePanel ID="upTabs_1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="Chk_RicettaPubblica" Text="Ricetta valida per tutte le Imprese"
                                        runat="server" Font-Bold="true" />
                                </td>
                                <td>
                                    <b>Codice :</b>
                                    <asp:TextBox ID="Txt_Ricetta_Numero" runat="server" CssClass="txtUI"> 
                                    </asp:TextBox>
                                    <asp:Label ID="Lbl_Ricetta_Cod" runat="server" CssClass="displaynone" ForeColor="Red"></asp:Label>
                                    <asp:Label ID="Lbl_Riepilogo" runat="server" CssClass="displaynone"></asp:Label>
                                    <asp:Label ID="Lbl_Id_Agenda" runat="server" CssClass="displaynone"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 200px">
                                    <asp:RadioButtonList ID="RBL_Data" runat="server" CssClass="txtUI" Width="300px"
                                        RepeatDirection="Horizontal">
                                        <asp:ListItem Value="0" Selected="True">Giorno</asp:ListItem>
                                        <asp:ListItem Value="1">Intervallo di tempo</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                    <table aria-hidden="true">
                                        <tr>
                                            <td>
                                                <b>Data Inizio:</b>
                                                <asp:TextBox ID="Txt_DataInizio" runat="server" CssClass="txtUI datepicker" style="width: 80px"> 
                                                </asp:TextBox>
                                            </td>
                                            <td id="Cella_DataFine1" style="margin-left: 5px">
                                                <b>Data Fine:</b>
                                                <asp:TextBox ID="Txt_DataFine" runat="server" CssClass="txtUI datepicker" style="width: 80px"> 
                                                </asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="Lbl_Specie" Font-Bold="true">Specie Vegetale : </asp:Label>
                                </td>
                                <td>
                                    <asp:Panel ID="Pnl_Specie" runat="server">
                                        <asp:DropDownList ID="Cmb_Specie" runat="server" CssClass="txtUI change_per_load"
                                            Style="max-width: 200px">
                                        </asp:DropDownList>
                                        <asp:Button ID="btn_Specie" runat="server" Style="display: none" />
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:RadioButtonList ID="RBL_Cultivar" runat="server" RepeatDirection="Horizontal"
                                        CssClass="txtUI">
                                        <asp:ListItem Value="0" Selected="True">Ricetta Valida per tutte le Varietà</asp:ListItem>
                                        <asp:ListItem Value="1">Ricetta Valida per Varietà Specifiche</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div style="overflow: auto; height: 250px" class="txtUI" id="div_cultivar">
                                        <asp:CheckBoxList ID="CBL_Cultivar" runat="server">
                                        </asp:CheckBoxList>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="Lbl_Ricetta_Des" Font-Bold="true">Descrizione : </asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="Txt_Ricetta_Des" runat="server" CssClass="txtUI" MaxLength="255"
                                        Style="min-width: 300px;" TextMode="MultiLine"></asp:TextBox>
                                    <asp:Label runat="server" ID="Lbl_TipoRicetta" style="display:none" ></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Note : </b>
                                </td>
                                <td>
                                    <asp:TextBox ID="Txt_Note" runat="server" CssClass="txtUI" MaxLength="4000" Style="min-width: 300px;"
                                        TextMode="MultiLine"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="RigaNote" runat="server">
                                <td colspan="2">
                                    <asp:panel style="overflow: auto; height: 50px" class="txtUI" runat="server" ID="Pnl_CBL_Note">
                                        <asp:CheckBoxList ID="CBL_Note" Height="50px" runat="server">
                                        </asp:CheckBoxList>
                                    </asp:panel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="cento boxColore" style="padding: 10px; margin-bottom:10px;" id="divDettagli">
                <asp:UpdatePanel ID="upTabs_2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%" aria-hidden="true">
                            <tr>
                                <td>
                                    <table aria-hidden="true">
                                        <tr>
                                            <td id="cella_operazione" runat="server">
                                                <b>Operazioni :</b><cc1:ComboOperazioni ID="ComboOperazione" runat="server" />
                                                <asp:Button ID="BTN_ComboOperazione" runat="server" Text="Button" Style="display: none" />
                                            </td>
                                            <td id="cella_planning" runat="server">
                                                <b>Pianificazioni :</b><cc1:ComboPianificazioni ID="ComboPianificazioni" runat="server" />
                                                <asp:Button ID="BTN_ComboPianificazioni" runat="server" Text="Button" Style="display: none" />
                                            </td>
                                            <td>
                                                <input type="button" value="NUOVA OPERAZIONE" id="Btn_Nuovo_Consiglio_Maschera" style="margin-left: 5px;
                                                    margin-right: 5px; color: red; z-index: 100" onclick="funz_maschera_bottone()" class="bottone btn_per_load"/> <!--onclick="buttonClicked();" -->
                                                <asp:Button ID="Btn_Nuovo_Consiglio" runat="server" Text="NUOVA OPERA"  Style="display: none"  />
                                            </td>
                                            <td>
                                                <asp:Button ID="Btn_FiltraImpianti" ToolTip="Seleziona gli Impianti su cui applicare la Ricetta"
                                                    runat="server" Text="FILTRA IMPIANTI" Style="margin-left: 5px; margin-right: 5px;
                                                    color: red" CssClass="bottone btn_per_load" />
                                            </td>
                                            <td>
                                                <asp:Button ID="Btn_FiltraInterventi" ToolTip="Seleziona gli Interventi da cui creare la Ricetta"
                                                    runat="server" Text="FILTRA INTERVENTI" Style="margin-left: 5px; margin-right: 5px;
                                                    color: red" CssClass="bottone btn_per_load" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="Riga_Impianti" runat="server">
                                <td>
                                    <table aria-hidden="true">
                                        <tr>
                                            <td>
                                                <asp:GridView ID="GridView_Impianti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                                    CssClass="ui-widget-content" Caption="Selezionare gli Impianti :">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <input type="checkbox" id="chkSelezionaTuttiImpianti" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="ChkSelezionaImpianto" runat="server" CssClass="ChkSelezionaImpianto" />
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="20px" />
                                                            <ItemStyle Width="20px" />
                                                            <FooterStyle Width="20px" />
                                                            <ControlStyle Width="20px" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Campo_Cod" HeaderText="Campo_Cod" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Appezza" HeaderText="Appezza" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Id_Reg" HeaderText="Id_Reg" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Veg_Cod" HeaderText="Veg_Cod" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Cul_Cod" HeaderText="Cul_Cod" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Grfi_Cod" HeaderText="Grfi_Cod" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Regolamento" HeaderText="Regolamento" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Finanziamento" HeaderText="Finanziamento" HtmlEncode="false">
                                                            <ItemStyle CssClass="displaynone" />
                                                            <HeaderStyle CssClass="displaynone" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" SortExpression="Rag_Soc">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Sa_Nome" HeaderText="Centro Aziendale" SortExpression="Sa_Nome">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Campo_Des" HeaderText="Campo" Visible="false" SortExpression="Campo_Des">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="App_Nome" HeaderText="App." SortExpression="App_Nome">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Lotto" HeaderText="Progetto" SortExpression="Progetto">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Sup_Imp" HeaderText="Sup.[ha]" SortExpression="Sup_Imp">
                                                            <ItemStyle CssClass="Sup_Imp" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Validita_Inizio" HeaderText="Data Inizio Impianto" SortExpression="Validita_Inizio">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Validita_Fine" HeaderText="Data Fine Impianto" SortExpression="Validita_Fine">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="N_Max" HeaderText="N Max" SortExpression="N_Max" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="P_Max" HeaderText="P Max" SortExpression="P_Max" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="K_Max" HeaderText="K Max" SortExpression="K_Max" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="N_Distribuito" HeaderText="N Distribuito" SortExpression="N_Distribuito" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="P_Distribuito" HeaderText="P Distribuito" SortExpression="P_Distribuito" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="K_Distribuito" HeaderText="K Distribuito" SortExpression="K_Distribuito" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="N_Residuo" HeaderText="N Residuo" SortExpression="N_Residuo" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="P_Residuo" HeaderText="P Residuo" SortExpression="P_Residuo" Visible="false">
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="K_Residuo" HeaderText="K Residuo" SortExpression="K_Residuo" Visible="false">
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <HeaderStyle CssClass="ui-widget-header" />
                                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="GridView_Operazioni" runat="server" AutoGenerateColumns="False"
                                        CellPadding="5" CssClass="ui-widget-content btn_per_load" AllowPaging="false"
                                        Style="margin-top: 5px;">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="ChkSelezionaOperazione" runat="server" CssClass="ChkSelezionaOperazione"
                                                        AutoPostBack="True" OnCheckedChanged="chkSelected_CheckedChanged" />
                                                </ItemTemplate>
                                                <HeaderStyle Width="20px" />
                                                <ItemStyle Width="20px" />
                                                <FooterStyle Width="20px" />
                                                <ControlStyle Width="20px" />
                                            </asp:TemplateField>
                                            <asp:BoundField Visible="true" DataField="Ricetta_Operazione_Cod" HeaderText="Ricetta_Operazione_Cod"
                                                HtmlEncode="false">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField Visible="true" DataField="Lav_Cod" HeaderText="Lav_Cod" HtmlEncode="false">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField Visible="true" DataField="Xml_Operazione" HeaderText="Xml_Operazione"
                                                HtmlEncode="false">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField Visible="true" DataField="Ricetta_Operazione_Des" HeaderText="Intervento"
                                                HtmlEncode="false">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField Visible="true" DataField="Ricetta_Operazione_Data" HeaderText="Data Intervento"
                                                HtmlEncode="false">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField Visible="true" DataField="Id_Agenda" HeaderText="Id_Agenda" HtmlEncode="false">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ci.ico' border='0'&gt; "
                                                HeaderText="Info" CommandName="Info" />
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                                                HeaderText="Mod." CommandName="Modifica" />
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                HeaderText="Canc." CommandName="Elimina" />
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/MovimentiMacchine16.ico' border='0'&gt;"
                                                HeaderText="Registra&lt;br&gt;Intervento" CommandName="Agenda">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <HeaderStyle CssClass="ui-widget-header" />
                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                        <RowStyle CssClass="rigaRicetta" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Btn_Aggiungi_Dettaglio" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Btn_Annulla_Dettaglio" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Cmb_Specie" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Button ID="AggiornaCostiAccessori" runat="server" Style="display: none;" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!--- Bottoni Salvattaggio e Annulla -->
                <asp:Button ID="SalvaCostiAccessori" runat="server" Style="display: none;" />
                <asp:Button ID="AnnullaCostiAccessori" runat="server" Style="display: none;" />
                <!--Dialog per l'eliminazione -->
                <div id="dialogCostiAccessori">
                    <asp:UpdatePanel ID="UpdateCostiAccessori" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <table class='centerTab' id="tabellaCostiAccessori" runat="server" visible="False" aria-hidden="true">
                                <tr id="Tr1" runat="server">
                                    <td id="Td1" runat="server">
                                        <div style="max-height: 200px; overflow: auto; width: 190px">
                                            <asp:GridView ID="dgrCentriCosto" runat="server" AutoGenerateColumns="False" Width="170px"
                                                CellPadding="5" CssClass="ui-widget-content">
                                                <Columns>
                                                    <asp:ButtonField DataTextField="Fabbricato_Des" HeaderText="Provenienza" CommandName="Select" />
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td id="Td2" runat="server">
                                        <div style="max-height: 200px; overflow: auto; width: 685px">
                                            <asp:GridView ID="dgrMateriali" runat="server" AutoGenerateColumns="False" Width="665px"
                                                CellPadding="5" CssClass="ui-widget-content">
                                                <Columns>
                                                    <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/FrecciaRossa_S.ico' border='0'&gt;"
                                                        CommandName="AggiungiCosto" />
                                                    <asp:BoundField DataField="Col_0" HeaderText="Categoria Prodotto"></asp:BoundField>
                                                    <asp:BoundField DataField="Col_1" HeaderText="Prodotto"></asp:BoundField>
                                                    <asp:BoundField DataField="Col_2" HeaderText="Giacenza"></asp:BoundField>
                                                    <asp:BoundField DataField="Col_3" HeaderText="Codice_Materiale">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Col_5" HeaderText="Udm_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Col_6" HeaderText="Elem_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Col_7" HeaderText="Ditta_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Col_8" HeaderText="Mat_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Col_9" HeaderText="Costo Unitario [euro]"></asp:BoundField>
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="Tr2" runat="server">
                                    <td id="Td3" colspan="2" runat="server">
                                        <asp:GridView ID="dgrScarico" runat="server" AutoGenerateColumns="False" Width="100%"
                                            CellPadding="5" CssClass="ui-widget-content">
                                            <Columns>
                                                <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                    CommandName="EliminaCosto" />
                                                <asp:BoundField DataField="Centro" HeaderText="Centri di Costo" />
                                                <asp:BoundField DataField="Categoria_Des" HeaderText="Categoria Risorsa" />
                                                <asp:BoundField DataField="Risorsa_des" HeaderText="Risorsa" />
                                                <asp:TemplateField HeaderText="Unit&#224; Misura">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="Cmb_UdmCosti" runat="server">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quantità">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txt_Qta_Ril" runat="server" CssClass="txtUI">
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Costo_Unitario" HeaderText="Costo Unitario [euro]" />
                                                <asp:BoundField DataField="Costo" HeaderText="Costo [euro]" />
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="AggiornaCostiAccessori" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="SalvaCostiAccessori" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="AnnullaCostiAccessori" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div style="padding: 10px;">
                <asp:Button ID="Btn_Costi" runat="server" Visible="false" Text="COSTI" Style="margin-left: 5px; color: red" CssClass="bottone" />
                <asp:Button ID="Btn_Salva" runat="server" Text="SALVA" Style="color: red" CssClass="bottone" />
                <asp:Button ID="Btn_Salva_Stampa_RicAz" runat="server" Text="SALVA E STAMPA X AZIENDA" Style="color: red" CssClass="bottone" />
                <asp:Button ID="Btn_Stampa_Cert" runat="server" Text="STAMPA X CERTIFICAZIONE" Style="color: red" CssClass="bottone" />
                <asp:Button ID="Btn_Stampa_RicAz" runat="server" Text="STAMPA X AZIENDA" Style="color: red"  CssClass="bottone" />
            </div>

        </div>
    </div>
    <div id="dialogOperazione" style="width: auto;">
        <div id="dialogAvvInputData">
            <cc1:ComboMisuraXAvversitaInputData ID="ComboMisuraXAvversitaInputData1" runat="server" />
        </div>
        <asp:UpdatePanel ID="UpdatePanelOperazione" runat="server">
            <ContentTemplate>
                <asp:UpdateProgress ID="UpdateProgressOperazione" runat="server" DisplayAfter="100"
                    AssociatedUpdatePanelID="UpdatePanelOperazione">
                    <ProgressTemplate>
                        <asp:Image ID="ImageOperazione" ImageUrl="~/AB_Immagini/Messaggi/ajax-loader.gif"
                            runat="server" />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <table width="100%" aria-hidden="true">
                    <tr>
                        <td>
                            <table aria-hidden="true">
                                <tr>
                                    <td id="cella_salva_operazione" runat="server" valign="top" visible="false">
                                        <b>Data Registrazione Intervento :</b>
                                        <asp:TextBox ID="Txt_Data_Interventi" runat="server" CssClass="txtUI datepicker"> 
                                        </asp:TextBox>
                                    </td>
                                    <td id="cella_disciplinare" runat="server" visible="false">
                                        <b>Disciplinare :</b>
                                        <asp:DropDownList ID="Cmb_Disciplinare" runat="server" CssClass="txtUI change_per_load">
                                        </asp:DropDownList>
                                        <asp:Button ID="btn_Disciplinari" runat="server" Style="display: none" />
                                    </td>
                                    <td id="cella_epoca" runat="server" visible="false">
                                        <label id="Lbl_Epoca" runat="server">
                                            <b>Epoca : </b>
                                        </label>
                                        <asp:DropDownList ID="Cmb_Epoca" runat="server" CssClass="txtUI">
                                        </asp:DropDownList>
                                        <cc1:ComboEpocheFertilizzazione ID="ComboEpoche" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Riga_Lavorazioni" runat="server">
                        <td>
                            <asp:Panel ID="Pannello_Note_Lavorazioni" runat="server">
                                <!-- Note -->
                                <script type="text/javascript">
                                    $(function () {
                                        $("#tabs-l").tabs();
                                    });
                                </script>
                                <div style="min-width: 215px; float: left; width: 100%" id="tabs-l">
                                    <ul>
                                        <li><a href="#tabs-2-l">Giustificazioni</a></li>
                                        <li><a href="#tabs-1-l">Note</a></li>
                                    </ul>
                                    <div id="tabs-2-l" style="height: 76px; display: block; overflow: auto">
                                        <div>
                                            <asp:CheckBoxList ID="CBL_Consigli_Lavorazioni" CssClass="txtUI" Height="76px" runat="server"
                                                Width="100%">
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                    <div id="tabs-1-l">
                                        <div>
                                            <asp:TextBox ID="Txt_Note_Lavorazioni" runat="server" TextMode="MultiLine" CssClass="txtUI"
                                                Width="100%" Height="100%"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr id="Riga_RilievoAvvAus" runat="server">
                        <td>
                            <table id="tblContainer_RilievoAvvAus" aria-hidden="true">
                                <table width="100%" aria-hidden="true">
                                    <tr>
                                        <td>
                                            <cc1:ComboFaseFenologica ID="ddlFF1" runat="server" />
                                        </td>
                                        <td>
                                            <cc1:ComboFaseFenologica ID="ddlFF2" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                <!-- Gridview Impianti Selezionati -->
                                
                                <div style="overflow: auto; width: 100%">
                                    <asp:PlaceHolder ID="PlaceTabella" runat="server"></asp:PlaceHolder>
                                </div>
                                
                            </table>
                        </td>
                    </tr>
                    <!-- fine Riga_RilievoAvvAus -->
                    <tr id="Riga_Trattamento" runat="server">
                        <td>
                            <table aria-hidden="true">
                                <tr>
                                    <td id="CellaAvversita" runat="server" valign="top">
                                        <div>
                                            <asp:RadioButtonList ID="RBL_Avversita" runat="server" RepeatDirection="Horizontal"
                                                CssClass="txtUI">
                                                <asp:ListItem Value="0">Avversita&#39;</asp:ListItem>
                                                <asp:ListItem Value="1">Gruppi Avversita&#39;</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <!-- DataGridGruppiAvversita -->
                                        <div style="overflow: auto; max-height: 160px; width: 350px;">
                                            <asp:GridView ID="GridViewGruppiAvversita" runat="server" AutoGenerateColumns="False"
                                                Width="330px" CellPadding="5" CssClass="ui-widget-content">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChkSelezionaGruppoAvversita" runat="server" CssClass="ChkSelezionaAvversita_CheckedChanged ChkSelezionaGruppoAvversita" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Av_Gru_Des" HeaderText="Gruppi Avversita'" HtmlEncode="false">
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Str_Av_Cod" HeaderText="Str_Av_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>
                                        <!-- DataGridGruppiAvversita -->
                                        <div style="overflow: auto; max-height: 160px; width: 350px;">
                                            <asp:GridView ID="GridViewAvversita" runat="server" AutoGenerateColumns="False" Width="330px"
                                                CellPadding="5" CssClass="ui-widget-content">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="ChkSelezionaAvversita" runat="server" CssClass="ChkSelezionaAvversita_CheckedChanged ChkSelezionaAvversita" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod">
                                                        <ItemStyle CssClass="displaynone" />
                                                        <HeaderStyle CssClass="displaynone" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Av_Des" HeaderText="Avversita'" HtmlEncode="false"></asp:BoundField>
                                                    <asp:TemplateField HeaderText="Richiesta Soglia / Giustif.">
                                                        <ItemTemplate>
                                                            <asp:RadioButtonList ID="rbl_soglie_intervento" runat="server" HeaderText="Richiesta Soglia / Giustif."
                                                                CssClass="stileSoglie rblSoglie">
                                                            </asp:RadioButtonList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle CssClass="ui-widget-header" />
                                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td valign="top">
                                        <div style="float: left; margin-left: 5px">
                                            <b>Acqua [hl]:</b></div>
                                        <div style="float: left; margin-left: 5px">
                                            <asp:TextBox ID="Txt_Acqua_Difesa" runat="server" CssClass="txtUI" Width="40px"></asp:TextBox>
                                        </div>
                                        <div style="float: left; margin-left: 5px">
                                            <asp:RadioButtonList ID="RBL_Acqua_Formulati" runat="server" RepeatDirection="Horizontal"
                                                CssClass="txtUI" Width="145px">
                                                <asp:ListItem Value="1" Selected="true">[/Ha]</asp:ListItem>
                                                <asp:ListItem Value="0">[Tot.]</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div class="clear">
                                        </div>
                                        <div>
                                            <asp:DropDownList ID="Cmb_FormulatoClassificazioni" runat="server" CssClass="txtUI Cmb_FormulatoClassificazioni myCombo"
                                                Style="min-width: 150px" meta:resourcekey="Cmb_FormulatoClassificazioniResource1">
                                            </asp:DropDownList>
                                            <div class="clear">
                                            </div>
                                            <div style="float: left;">
                                                <div style="float: left; margin-left: 5px">
                                                    <b>Testo Ricerca: </b>
                                                </div>
                                                <div style="float: left; margin-left: 5px">
                                                    <asp:TextBox ID="Txt_Formulati" runat="server" CssClass="txtUI" Width="100px"></asp:TextBox>
                                                </div>
                                                <div style="float: left; margin-left: 5px">
                                                    <asp:ImageButton ID="ImgBtn_Cerca_Formulati" runat="server" ImageUrl="../AB_Immagini/icone32/lente.ico"
                                                        ToolTip="Premere il pulsante per cercare i Formulati" Style="border-width: 0;
                                                        float: right; margin-left: 5px; margin-right: 17px; width: 32px;" CssClass="btn_per_load" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="clear">
                                        </div>
                                        <div>
                                            <asp:Label ID="Lbl_Num_Formulati" runat="server" Style="color: red">
                                            </asp:Label><br />
                                            <cc1:ComboFormulati ID="ComboFormulati" runat="server" class="classComboFormulati" />
                                            <asp:Button ID="BTN_ComboFormulati" runat="server" Text="Button" Style="display: none"
                                                CssClass="btn_per_load" /><br />
                                            <asp:Label ID="Lbl_Dose_Etichetta" runat="server" ForeColor="red"></asp:Label>
                                        </div>
                                        <br />
                                        <div style="float: left">
                                            <asp:RadioButtonList ID="RBL_Dose_Formulati" runat="server" AutoPostBack="true" CssClass="txtUI"
                                                Style="width: 90px; float: left; margin-right: 5px">
                                                <asp:ListItem Value="1" Selected="true">Dose/Ha</asp:ListItem>
                                                <asp:ListItem Value="0">Dose/Hl</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:DropDownList ID="Cmb_Udm_Formulati" runat="server" CssClass="txtUI" Width="50px"
                                                Style="margin-right: 5px; float: left">
                                                <asp:ListItem Value="2">kg</asp:ListItem>
                                                <asp:ListItem Value="3">g</asp:ListItem>
                                                <asp:ListItem Value="29">l</asp:ListItem>
                                                <asp:ListItem Value="104">cc</asp:ListItem>
                                                <asp:ListItem Value="101">ml</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox ID="Txt_Dose_Formulati" runat="server" CssClass="txtUI" Style="width: 70px;
                                                margin-right: 5px"> 
                                            </asp:TextBox>
                                        </div>
                                        <div style="float: right; margin-right: 10px">
                                            <asp:ImageButton ID="ImgBtn_DoseInserisci_Formulati" runat="server" CssClass="btn_per_load"
                                                ToolTip="Inserisci i formulati nella miscela" ImageUrl="../AB_Immagini/icone32/FrecciaDN.ico"
                                                Style="width: 32px" />
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </td>
                                    <td valign="top">
                                        <asp:Panel ID="Pannello_Note_Difesa" runat="server">
                                            <!-- Note -->
                                            <script type="text/javascript">
                                                $(function () {
                                                    $("#tabs-d").tabs();
                                                });
                                            </script>
                                            <div style="min-width: 215px; float: left; width: 100%" id="tabs-d">
                                                <ul>
                                                    <li><a href="#tabs-2-d">Giustificazioni</a></li>
                                                    <li><a href="#tabs-1-d">Note</a></li>
                                                </ul>
                                                <div id="tabs-2-d" style="height: 76px; display: block; overflow: auto">
                                                    <div>
                                                        <asp:CheckBoxList ID="CBL_Consigli_Difesa" CssClass="txtUI" Height="100%" runat="server"
                                                            Width="100%">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <div id="tabs-1-d">
                                                    <div>
                                                        <asp:TextBox ID="Txt_Note_Difesa" runat="server" TextMode="MultiLine" CssClass="txtUI"
                                                            Width="100%" Height="100%"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:GridView ID="GridView_Dosi_Difesa" runat="server" AutoGenerateColumns="False"
                                            Width="100%" CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" Caption="Riepilogo della miscela di formulati da utilizzare nel trattamento">
                                            <Columns>
                                                <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                                                    HeaderText="Mod." CommandName="Modifica" />
                                                <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                    HeaderText="Canc." CommandName="Elimina" />
                                                <asp:BoundField DataField="Av_Cod" HeaderText="Av_Cod">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Av_Gru" HeaderText="Av_Gru">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Av_Des" HeaderText="Avversità / Gruppi Avversità" HtmlEncode="false">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Soglia_Value" HeaderText="Soglia_Value" HtmlEncode="false">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Soglia_Des" HeaderText="Richiesta Soglia / Giustif." HtmlEncode="false">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Fr_Cod" HeaderText="N. Reg. Min"></asp:BoundField>
                                                <asp:BoundField DataField="Fr_Des" HeaderText="Formulato"></asp:BoundField>
                                                <asp:BoundField DataField="Carenza" HeaderText="Carenza"></asp:BoundField>
                                                <asp:BoundField DataField="Dose_Etichetta" HeaderText="Dose Etichetta" HtmlEncode="false">
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Dose_Etichetta_Max" HeaderText="Dose_Etichetta_Max">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Udm_Des" HeaderText="Unità di Misura"></asp:BoundField>
                                                <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Dose" HeaderText="Dose"></asp:BoundField>
                                                <asp:BoundField DataField="Dose_Fittizia" HeaderText="Dose Reale">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Qta_Tot" HeaderText="Quantità Totale Distribuita">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="strPA_Cod" HeaderText="strPA_Cod" HtmlEncode="false">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="strCLTOSS_COD" HeaderText="strCLTOSS_COD">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            <RowStyle CssClass="rigaImpianti" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Riga_Concimazione" runat="server">
                        <td>
                            <table aria-hidden="true">
                                <tr>
                                    <td style="vertical-align: middle">
                                        <div style="width: 80%">
                                            <div style="height: 10px; float: left">
                                                <b>Testo di Ricerca: </b>
                                            </div>
                                            <asp:TextBox ID="Txt_Fertilizzanti" runat="server" CssClass="txtUI" Style="float: left;
                                                margin-left: 5px"></asp:TextBox>
                                            <asp:ImageButton ID="ImgBtn_Cerca_Fertilizzanti" runat="server" ImageUrl="../AB_Immagini/icone32/lente.ico"
                                                ToolTip="Premere il pulsante per cercare i Fertilizzanti" Style="border-width: 0;
                                                float: left; margin-left: 5px; margin-right: 17px; width: 32px;" CssClass="btn_per_load" />
                                            <div class="clear">
                                            </div>
                                        </div>
                                    </td>
                                    <td id="Cella_Acqua" runat="server" colspan="2">
                                        <div style="float: left; margin-left: 5px">
                                            <b>Acqua [ha]:</b></div>
                                        <div style="float: left; margin-left: 5px">
                                            <asp:TextBox ID="Txt_Acqua_Concimazione" runat="server" CssClass="txtUI" Width="70px"></asp:TextBox>
                                        </div>
                                        <div style="float: left; margin-left: 5px">
                                            <asp:RadioButtonList ID="RBL_Acqua_Concimazione" runat="server" RepeatDirection="Horizontal"
                                                CssClass="txtUI" Width="158px">
                                                <asp:ListItem Value="1" Selected="true">[/ha]</asp:ListItem>
                                                <asp:ListItem Value="0">[Tot.]</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div class="clear">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Lbl_Num_Fertilizzanti" runat="server" Style="color: red">
                                        </asp:Label><br />
                                        <cc1:ComboFertilizzanti ID="ComboFertilizzanti" runat="server" class="classComboFertilizzanti" />
                                        <asp:Button ID="BTN_ComboFertilizzanti" runat="server" Text="Button" Style="display: none"
                                            CssClass="btn_per_load" />
                                        <br />
                                        <br />
                                        <span style="margin-right: 10px"><strong>N</strong>:
                                            <asp:TextBox ID="Txt_N" runat="server" CssClass="txtUI" Width="40"> 
                                            </asp:TextBox>
                                        </span><span style="margin-right: 10px"><strong>P2O5</strong>:<asp:TextBox ID="Txt_P2O5"
                                            runat="server" CssClass="txtUI" Width="40"> 
                                                  
                                        </asp:TextBox>
                                        </span><span style="margin-right: 10px"><strong>K2O</strong>:<asp:TextBox ID="Txt_K2O"
                                            runat="server" CssClass="txtUI" Width="40"> 
                                        </asp:TextBox>
                                        </span><span style="margin-right: 10px"><strong>MgO</strong>:<asp:TextBox ID="Txt_MgO"
                                            runat="server" CssClass="txtUI" Width="40"> 
                                        </asp:TextBox>
                                        </span><span style="margin-right: 10px"><strong>Efficienza</strong>:<asp:TextBox
                                            ID="Txt_Efficienza" runat="server" CssClass="txtUI" Width="40"> 
                                        </asp:TextBox></span> <span><strong>N Utile</strong>:<asp:TextBox ID="Txt_N_Utile"
                                            runat="server" CssClass="txtUI" Width="40" Enabled="false"> 
                                        </asp:TextBox>
                                        </span>
                                        <br />
                                        <br />
                                        <asp:RadioButtonList ID="Rbl_Dosi_Fertilizzanti" runat="server" CssClass="txtUI"
                                            Width="100px" Style="float: left; margin-right: 5px">
                                            <asp:ListItem Value="1" Selected="true">Dose/ha</asp:ListItem>
                                            <asp:ListItem Value="0">Dose/Hl</asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:DropDownList ID="Cmb_UdM_Fertilizzanti" runat="server" CssClass="txtUI" Width="50px"
                                            Style="float: left; margin-right: 5px">
                                            <asp:ListItem Value="2">kg</asp:ListItem>
                                            <asp:ListItem Value="3">g</asp:ListItem>
                                            <asp:ListItem Value="4">q</asp:ListItem>
                                            <asp:ListItem Value="29">l</asp:ListItem>
                                            <asp:ListItem Value="104">cc</asp:ListItem>
                                            <asp:ListItem Value="101">ml</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="Txt_Dose_Fertilizzanti" runat="server" CssClass="txtUI" Style="min-width: 70px;
                                            float: left; margin-right: 5px"> 
                                        </asp:TextBox>
                                        <asp:ImageButton ID="ImgBtn_DoseInserisci_Fertilizzanti" runat="server" CssClass="btn_per_load"
                                            ToolTip="Inserisci i fertilizzanti nella miscela" ImageUrl="../AB_Immagini/icone32/FrecciaDN.ico"
                                            Style="width: 32px; float: left; margin-right: 5px" />
                                        <div class="clear">
                                        </div>
                                    </td>
                                    <td colspan="2">
                                        <asp:Panel ID="Pannello_Note_Concimazione" runat="server">
                                            <!-- Note -->
                                            <script type="text/javascript">
                                                $(function () {
                                                    $("#tabs-c").tabs();
                                                });
                                            </script>
                                            <div style="min-width: 215px; float: left; width: 100%" id="tabs-c">
                                                <ul>
                                                    <li><a href="#tabs-2-c">Giustificazioni</a></li>
                                                    <li><a href="#tabs-1-c">Note</a></li>
                                                </ul>
                                                <div id="tabs-2-c" style="height: 76px; display: block; overflow: auto">
                                                    <div>
                                                        <asp:CheckBoxList ID="CBL_Consigli_Concimazione" CssClass="txtUI" Height="76px" runat="server"
                                                            Width="100%">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <div id="tabs-1-c">
                                                    <div>
                                                        <asp:TextBox ID="Txt_Note_Concimazione" runat="server" TextMode="MultiLine" CssClass="txtUI"
                                                            Width="100%" Height="100%"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:GridView ID="GridView_Dosi_Concimazione" runat="server" AutoGenerateColumns="False"
                                            Width="100%" CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" Caption="Riepilogo della miscela di fertilizzanti da utilizzare">
                                            <Columns>
                                                <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/ce.ico' border='0'&gt; "
                                                    HeaderText="Mod." CommandName="Modifica" />
                                                <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                    HeaderText="Canc." CommandName="Elimina" />
                                                <asp:BoundField DataField="Fer_Cod" HeaderText="Fer_Cod">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Fer_Des" HeaderText="Fertilizzante"></asp:BoundField>
                                                <asp:BoundField DataField="Efficienza" HeaderText="Efficienza"></asp:BoundField>
                                                <asp:BoundField DataField="N" HeaderText="N"></asp:BoundField>
                                                <asp:BoundField DataField="N_Utile" HeaderText="N Utile"></asp:BoundField>
                                                <asp:BoundField DataField="P" HeaderText="P2O5"></asp:BoundField>
                                                <asp:BoundField DataField="K" HeaderText="K2O"></asp:BoundField>
                                                <asp:BoundField DataField="Mg" HeaderText="MgO"></asp:BoundField>
                                                <asp:BoundField DataField="Udm_Des" HeaderText="Unità di Misura"></asp:BoundField>
                                                <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Dose" HeaderText="Dose"></asp:BoundField>
                                                <asp:BoundField DataField="Dose_Fittizia" HeaderText="Dose Reale">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Qta_Tot" HeaderText="Quantità Totale Distribuita">
                                                    <ItemStyle CssClass="displaynone" />
                                                    <HeaderStyle CssClass="displaynone" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="ui-widget-header" />
                                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                            <RowStyle CssClass="rigaImpianti" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Riga_Irrigazione" runat="server">
                        <td>
                            <table aria-hidden="true">
                                <tr>
                                    <td>
                                        <table aria-hidden="true">
                                            <tr>
                                                <td>
                                                    <b>Unita' di Misura :</b>
                                                </td>
                                                <td>
                                                    <b>Dose :</b>
                                                </td>
                                                <td>
                                                    <b>Irrigazione Utilizzata</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_Udm_Irrigazione" runat="server" CssClass="txtUI" Width="100px"
                                                        Style="margin-right: 5px; float: left">
                                                        <asp:ListItem Value="18">millimetri</asp:ListItem>
                                                        <asp:ListItem Value="90">metri cubi/ha</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_Dose_Irrigazione" runat="server" CssClass="txtUI" Style="min-width: 50px;
                                                        float: left; margin-right: 5px"> 
                                                    </asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_ModifTipoIrrig" runat="server" CssClass="txtUI" Style="max-width: 200px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Data Inizio Microirrigazione :</b>
                                                </td>
                                                <td>
                                                    <b>Data Fine Microirrigazione :</b>
                                                </td>
                                                <td>
                                                    <b>Frequenza :</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="Txt_DataInizio_Irrigazione" runat="server" CssClass="txtUI datepicker"> 
                                                    </asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_DataFine_Irrigazione" runat="server" CssClass="txtUI datepicker"> 
                                                    </asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_Frequenza_Irrigazione" runat="server" CssClass="txtUI" Style="min-width: 50px;
                                                        float: left; margin-right: 5px"> 
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Panel ID="Pannello_Note_Irrigazione" runat="server">
                                            <!-- Note -->
                                            <script type="text/javascript">
                                                $(function () {
                                                    $("#tabs-i").tabs();
                                                });
                                            </script>
                                            <div style="min-width: 215px; float: left; width: 100%" id="tabs-i">
                                                <ul>
                                                    <li><a href="#tabs-2-i">Giustificazioni</a></li>
                                                    <li><a href="#tabs-1-i">Note</a></li>
                                                </ul>
                                                <div id="tabs-2-i" style="height: 76px; display: block; overflow: auto">
                                                    <div>
                                                        <asp:CheckBoxList ID="CBL_Consigli_Irrigazione" CssClass="txtUI" Height="76px" runat="server"
                                                            Width="100%">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <div id="tabs-1-i">
                                                    <div>
                                                        <asp:TextBox ID="Txt_Note_Irrigazione" runat="server" TextMode="MultiLine" CssClass="txtUI"
                                                            Width="100%" Height="100%"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Riga_Trappole" runat="server">
                        <td>
                            <table aria-hidden="true">
                                <tr>
                                    <td>
                                        <table width="100%" aria-hidden="true">
                                            <tr>
                                                <td>
                                                    <b>Dispenser</b>:
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmb_Trappola" runat="server" CssClass="txtUI change_per_load"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="width: 50px">
                                                </td>
                                                <td>
                                                    <b>Durata Feromone [gg.]</b>:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Txt_GiorniFeromone" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Avversità</b>:
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmb_Avversita" runat="server" CssClass="txtUI change_per_load"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                    <b>Codice Avversità</b>:
                                                </td>
                                                <td>
                                                    <asp:Label ID="Txt_CodAvversita" runat="server" CssClass="txtUI" Style="min-width: 100px"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Ditta Fornitrice</b>:
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="cmb_Ditte" runat="server" CssClass="txtUI">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                </td>
                                                <td>
                                                    <b>Numero/Ha </b>:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_NumeroTrappole" runat="server" CssClass="txtUI"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Panel ID="Pannello_Note_Trappole" runat="server">
                                            <!-- Note -->
                                            <script type="text/javascript">
                                                $(function () {
                                                    $("#tabs-t").tabs();
                                                });
                                            </script>
                                            <div style="min-width: 215px; float: left; width: 100%" id="tabs-t">
                                                <ul>
                                                    <li><a href="#tabs-2-t">Giustificazioni</a></li>
                                                    <li><a href="#tabs-1-t">Note</a></li>
                                                </ul>
                                                <div id="tabs-2-t" style="height: 76px; display: block; overflow: auto">
                                                    <div>
                                                        <asp:CheckBoxList ID="CBL_Consigli_Trappole" CssClass="txtUI" Height="76px" runat="server"
                                                            Width="100%">
                                                        </asp:CheckBoxList>
                                                    </div>
                                                </div>
                                                <div id="tabs-1-t">
                                                    <div>
                                                        <asp:TextBox ID="Txt_Note_Trappole" runat="server" TextMode="MultiLine" CssClass="txtUI"
                                                            Width="100%" Height="100%"> </asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="Riga_Costi" runat="server">
                        <td>
                            <asp:UpdatePanel ID="UpdatePanelCostiAccessoriVisibili" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:GridView ID="GridViewCostiAccessoriVisibili" runat="server" AutoGenerateColumns="False"
                                        Width="100%" CellPadding="5" CellSpacing="0" CssClass="ui-widget-content" Visible="true"
                                        Caption="Altre Risorse Impiegate:">
                                        <Columns>
                                            <asp:ButtonField Text="&lt;img src='../AB_Immagini/icone16/gomma16.ico' border='0'&gt;"
                                                CommandName="EliminaCosto"></asp:ButtonField>
                                            <asp:BoundField DataField="Centro" HeaderText="Centri di Costo" />
                                            <asp:BoundField DataField="Categoria_Des" HeaderText="Categoria Risorsa" />
                                            <asp:BoundField DataField="Risorsa_des" HeaderText="Risorsa" />
                                            <asp:TemplateField HeaderText="Unit&#224; Misura">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="Cmb_UdmCosti" runat="server" Enabled="false">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Quantit&#224;">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Txt_Qta_Ril" runat="server" CssClass="txtUI" ReadOnly="true">
                                                    </asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Costo_Unitario" HeaderText="Costo Unitario [euro]">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Costo" HeaderText="Costo [euro]">
                                                <ItemStyle CssClass="displaynone" />
                                                <HeaderStyle CssClass="displaynone" />
                                            </asp:BoundField>
                                        </Columns>
                                        <HeaderStyle CssClass="ui-widget-header" />
                                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                    </asp:GridView>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="AggiornaCostiAccessori" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="SalvaCostiAccessori" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="AnnullaCostiAccessori" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Button ID="Btn_Salva_Operazione" runat="server" Text="SALVA OPERAZIONE" Style="margin-left: 5px;
                                margin-right: 5px; color: red" CssClass="bottone" OnClick="Salva_Operazione" />
                            <asp:Button ID="Btn_Aggiungi_Dettaglio" runat="server" Text="Aggiungi Dettaglio"
                                Style="margin-left: 5px; margin-right: 5px; color: red" CssClass="bottone" OnClick="Aggiungi_Dettaglio" />
                            <asp:Button ID="Btn_Annulla_Dettaglio" runat="server" Text="Annulla" Style="margin-left: 5px;
                                margin-right: 5px; color: red" CssClass="bottone" OnClick="Annulla_Dettaglio" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <script type="text/javascript">
        function SalvaAccessori() {
            $("#<%=SalvaCostiAccessori.ClientID %>").click(); //effettua il post back per eliminazione dei dati
            $('#dialogCostiAccessori').dialog("close");
        };
        $(document).ready(function () {
            // messaggi errore
            $('#dialogCostiAccessori').dialog({
                autoOpen: false,
                modal: true,
                width: 900,
                buttons: {
                    "Aggiungi": function () {
                        $(this).dialog("close");
                        SalvaAccessori();
                    },
                    "Annulla": function () {
                        $(this).dialog("close");
                        $("#<%=AnnullaCostiAccessori.ClientID %>").click();
                        return false;
                    }
                }
            });
        });
    </script>
</asp:Content>
