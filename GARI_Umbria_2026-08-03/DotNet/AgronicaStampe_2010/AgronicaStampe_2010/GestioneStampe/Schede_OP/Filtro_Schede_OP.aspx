<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_Schede_OP.aspx.vb"
    Inherits="AgronicaStampe_2010.Filtro_Schede_OP" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../App_Styles/AgronicaStyle.css" rel="stylesheet" />
    <link href="../../App_Styles/Site.min.css" rel="stylesheet" type="text/css" />
    <link href="../../App_Styles/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
  
    <script src="../../App_Scripts/jquery-1.11.1.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="../../APP_Scripts/min/jquery-ui-1.8.16.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript">

        function SelezionaDeselezionaTutti() {

            if ($('#chkSelezionaTuttiImpianti').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                    $(this).children('input').prop('checked', true);
                    
                });
            }
            else {

                //deseleziono tutto
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').removeAttr('checked');
                    $(this).children('input').prop('checked', false);
                });
            }
        }




        $(document).ready(function () {

            $('#chkSelezionaTuttiImpianti').click(function () {
                SelezionaDeselezionaTutti();
            });


            $('#<%=TxtValiditaInizio.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $('#<%=TxtValiditaFine.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
            $.datepicker.regional['it'];

        });

        
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <table aria-hidden="true">
        <tr id="RigaIntestazione" runat="server">
            <td>
                <img alt="" src="../../AB_Immagini/Logo/Logo_GiasOnline_Mini.jpg" />
            </td>
            <td style="width: 100%;">
                <table aria-hidden="true" id="TableTitolo" style="width: 100%;">
                    <tr>
                        <td>
                            <div style="width: 100%; height: 32px;" class="ui-widget-header">
                                <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                                    <asp:Label ID="LblTitolo" runat="server">Filtro Schede per le OP</asp:Label>
                                </div>
                            </div>
                            <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                                <div style="float: left">
                                    <asp:Label ID="Lbl_RagSoc" runat="server"></asp:Label></div>
                            </div>
                        </td>
                        <td style="width: 50px">
                            <asp:ImageButton ID="ImgBtnAnnullaTutto" runat="server" Height="32px" ImageUrl="../../AB_Immagini/Icone32/Esci.bmp"
                                Width="32px" Style="margin-left: 10px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table aria-hidden="true">
        <tr>
            <td style="width: 65%" class="boxColore">
                <table aria-hidden="true">
                    <tr>
                        <td valign="top" class="boxColore">
                            <table aria-hidden="true">
                                <tr>
                                    <td>
                                        <b>Filtri di Ricerca :</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Ragione Sociale :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="Txt_RagioneSociale" runat="server" CssClass="txtUI_middle" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Partita IVA :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="Txt_PIVA" runat="server" CssClass="txtUI_middle" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Codice Socio :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="Txt_Codice" runat="server" CssClass="txtUI_middle" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="Btn_CercaImpresa" Style="margin-top: 5px; margin-left: 20px" runat="server"
                                            Text="Cerca" Width="80px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" class="boxColore">
                            <b>Selezionare l'impresa :</b>
                            <asp:GridView ID="DataGrid_Imprese" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                CssClass="ui-widget-content" Style="margin-top: 5px; background-image: none"
                                OnRowCommand="DataGrid_Imprese_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="Piva" HeaderText="Partita IVA"></asp:BoundField>
                                    <asp:ButtonField DataTextField="Rag_Soc" HeaderText="Ragione Sociale" CommandName="Select" />
                                    <asp:BoundField DataField="Provincia" HeaderText="Prov"></asp:BoundField>
                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" class="boxColore">
                            <table aria-hidden="true">
                                <tr>
                                    <td>
                                        <b>Filtri di Ricerca :</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Centro Aziendale :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ComboCentri" runat="server" CssClass="txtUI" Width="170px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Specie Vegetale :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ComboSpecie" runat="server" CssClass="txtUI" AutoPostBack="true"
                                            Width="170px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Varietà :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ComboVarieta" runat="server" CssClass="txtUI" Width="170px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Capitolato Privato :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ComboCapitolato" runat="server" CssClass="txtUI" Width="170px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Validità Inizio :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                            ToolTip="Data di riferimento" Width="150px">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Validità Fine :
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="TxtValiditaFine" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                            ToolTip="Data di riferimento" Width="150px">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="Btn_CercaImpianti" Style="margin-top: 5px; margin-left: 20px" runat="server"
                                            Text="Cerca" Width="80px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" class="boxColore">
                            <b>Selezionare gli impianti :</b>
                            <asp:GridView ID="DataGridImpianti" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                CssClass="ui-widget-content" Style="margin-top: 5px; background-image: none">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" id="chkSelezionaTuttiImpianti" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="ChkSeleziona" runat="server" CssClass="ChkSelezionaImpianto" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle Width="20px" />
                                        <FooterStyle Width="20px" />
                                        <ControlStyle Width="20px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="piva" HeaderText="Piva">
                                        <HeaderStyle HorizontalAlign="Center" CssClass="displaynone"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Left" CssClass="displaynone"></ItemStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Appezza" HeaderText="Appezza">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Id_Reg" HeaderText="Id_Reg">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="rag_soc" HeaderText="Ragione Sociale">
                                        <HeaderStyle HorizontalAlign="Center" CssClass="displaynone" />
                                        <ItemStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Sa_nome" HeaderText="Centro Aziendale"></asp:BoundField>
                                    <asp:BoundField DataField="App_Nome" HeaderText="App.">
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Veg_Cod" HeaderText="Veg_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Cul_Cod" HeaderText="Cul_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Grfi_Cod" HeaderText="Grfi_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Grva_Cod" HeaderText="Grva_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Veg_Des" HeaderText="Specie Vegetale"></asp:BoundField>
                                    <asp:BoundField DataField="Cul_Des" HeaderText="Variet&#224;"></asp:BoundField>
                                    <asp:BoundField DataField="Grfi_Des" HeaderText="Finalit&#224;"></asp:BoundField>
                                    <asp:BoundField DataField="Grva_Des" HeaderText="Gruppo Varietale"></asp:BoundField>
                                    <asp:BoundField DataField="Dettaglio_Specie_Cod" HeaderText="Dettaglio_Specie_Cod">
                                        <ItemStyle CssClass="displaynone" />
                                        <HeaderStyle CssClass="displaynone" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dettaglio_Specie_Des" HeaderText="Dettaglio Specie"></asp:BoundField>
                                    <asp:BoundField DataField="Capitolato_Des" HeaderText="Capitolato Privato"></asp:BoundField>
                                    <asp:BoundField DataField="Anno_Imp" HeaderText="Anno Imp."></asp:BoundField>
                                    <asp:BoundField DataField="Sup_Imp" HeaderText="Sup. [Ha]"></asp:BoundField>
                                </Columns>
                                <HeaderStyle CssClass="ui-widget-header" />
                                <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top" style="width: 70%" class="boxColore">
                <table aria-hidden="true">
                    <tr>
                        <td>
                            Anno di Riferimento :
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="Txt_Anno" runat="server" CssClass="txtUI_middle"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButtonList ID="OptionList_Scheda" runat="server" CssClass="" RepeatLayout="Flow">
                                <asp:ListItem Value="74" Selected="True">Atto Notorio</asp:ListItem>
                                <asp:ListItem Value="-74">Atto Notorio (con riepilogo catasto semplificato)</asp:ListItem>
                                <asp:ListItem Value="171">Allegato Catasto e Valorizzazioni</asp:ListItem>
                                <asp:ListItem Value="77">Adesione DPI</asp:ListItem>
                                <asp:ListItem Value="75">Adesione Etico Ambientale</asp:ListItem>
                                <asp:ListItem Value="81">Codice di Condotta</asp:ListItem>
                                <asp:ListItem Value="78">Impegnativa GLOBALGAP</asp:ListItem>
                                <asp:ListItem Value="79">Impegnativa QC</asp:ListItem>
                                <asp:ListItem Value="80">Impegnativa Confusione Sessuale</asp:ListItem>
                                <asp:ListItem Value="76">Tenuta Scheda di Campagna</asp:ListItem>
                                <asp:ListItem Value="127">Mandato Trasmissione Telematica Dati</asp:ListItem>
                                <asp:ListItem Value="130">Impegnativa Orticole Gestione Annuale</asp:ListItem>
                                <asp:ListItem Value="131">Impegnativa Orticole Gestione Breve</asp:ListItem>
                                <asp:ListItem Value="132">Impegnativa Orticole Industria</asp:ListItem>
                                <asp:ListItem Value="133">Impegnativa Pomodoro Industria</asp:ListItem>
                                <asp:ListItem Value="134">Impegnativa Fagiolino Mercato Fresco </asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <br />
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Button ID="Btn_Stampa" Style="margin-top: 5px; margin-left: 20px" runat="server"
                                Text="STAMPA" Width="80px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
