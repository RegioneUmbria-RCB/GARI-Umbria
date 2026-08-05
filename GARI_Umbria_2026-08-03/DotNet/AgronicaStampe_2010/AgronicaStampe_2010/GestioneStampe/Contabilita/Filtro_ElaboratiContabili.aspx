<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Filtro_ElaboratiContabili.aspx.vb"
    Inherits="AgronicaStampe_2010.Filtro_ElaboratiContabili" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
<head>
    <title>Filtro Elaborati Contabili</title>
    <link rel="stylesheet" type="text/css" href="../../App_Styles/AgronicaStyle.css" />
    <link rel="stylesheet" type="text/css" href="../../App_Styles/Site.css" />
    <link rel="stylesheet" type="text/css" href="../../App_Styles/jquery-ui-1.10.0.custom.min.css" />
    <script type="text/javascript" src="../../App_Scripts/jquery-1.9.0.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../../App_Scripts/jquery-ui-1.10.0.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <style>
        #ui-datepicker-div { z-index: 10000; }        
        table { border: 0; }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
        });
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <!-- tabella intestazione -->
    <table aria-hidden="true" id="TableTitolo" style="background-color: #042649" width="800px">
        <tr>
            <td width="32px">
                <asp:Image ID="ImgIcona" runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico">
                </asp:Image>
            </td>
            <td>
                <asp:Label ID="LblTitolo" runat="server" CssClass="Testo_12_Bianco_Bold"> Filtro Elaborati Contabili</asp:Label>
            </td>
            <td width="32px">
                <asp:ImageButton ID="ImgBtnEsci" runat="server" Width="32px" Height="32px" ImageUrl="../../AB_Immagini/icone32/esci.bmp">
                </asp:ImageButton>
            </td>
        </tr>
    </table>
    <!-- tabella contenitore -->
    <table aria-hidden="true" id="Table_contenitore" class="Filtro" style="border: 1px solid #A6C9E2; color: #222222;
                    font-family: Verdana,Arial,sans-serif; outline: 0 none !important; padding-bottom: 5px;
                    padding-top: 5px; font-size: 12px;">
        <tr>
            <td>
                <!-- pannello generale-->
                <table aria-hidden="true" id="Table_Generale"  runat="server">

                       <!-- riga0 -->
                    <tr id="Riga0" runat="server">
                        <td>
                            <!-- tabella0 tipo report -->
                            <table aria-hidden="true" id="Table0" runat="server" border="0" borderstyle="None">
                                <tr borderstyle="None">
                                    <td>
                                                <asp:RadioButtonList ID="Rbl_PianoDeiConti" runat="server" border="0" CssClass="testo_08_nero"
                                                        AutoPostBack="True" RepeatDirection="Vertical">                                                        
                                                        <asp:ListItem Value="0" Selected="true">Piano dei Conti (senza saldi)</asp:ListItem>
                                                        <asp:ListItem Value="1" >Piano dei Conti con saldo (alla data selezionata)</asp:ListItem>
                                                    <asp:ListItem Value="2" >Piano dei Conti con saldo di inizio gestione contabile</asp:ListItem>
                                                    </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella0 -->
                        </td>
                    </tr>
                    <!-- fine riga0 -->
                    
                    
                    <!-- Esercizio -->
                    <tr id="RigaEsercizio" runat="server">
                        <td>
                            <!-- tabella_es -->
                            <table aria-hidden="true" id="TableEsercizio">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="TableEsercizio2" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Esercizio"></asp:Label>
                                                    <asp:DropDownList ID="cmb_Esercizio" runat="server" CssClass="testo_08_nero"
                                                        AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella19 -->
                        </td>
                    </tr>
                    <!-- fine Esercizio -->


                    <!-- riga1 -->
                    <tr id="Riga1" runat="server">
                        <td>
                            <!-- tabella1 anno e periodo temporale -->
                            <table aria-hidden="true" id="Table1" runat="server" border="0" borderstyle="None">
                                <tr borderstyle="None">
                                    <td>
                                        <!-- periodo temporale -->
                                        <table aria-hidden="true" id="Table_Temporale" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Lbl_DataInizio" runat="server" Text="Dal:"></asp:Label>
                                                    <asp:TextBox ID="Txt_DataInizio" runat="server" CssClass="txtUI datepicker">
                                                    </asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Lbl_DataFine" runat="server" Text="A:"></asp:Label>
                                                    <asp:TextBox ID="txt_DataFine" runat="server" CssClass="txtUI datepicker">
                                                    </asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImgBtn_Indietro" runat="server" ImageUrl="../../AB_Immagini/icone32/frecciasx.ico"
                                                        Height="32px" Width="32px" ToolTip="Annata Precedente"></asp:ImageButton><asp:ImageButton
                                                            ID="ImgBtn_Avanti" runat="server" ImageUrl="../../AB_Immagini/icone32/frecciadx.ico"
                                                            Height="32px" Width="32px" ToolTip="Annata Successiva"></asp:ImageButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella1 -->
                        </td>
                    </tr>
                    <!-- fine riga1 -->

                    <!-- riga1b -->
                    <tr id="Riga1b" runat="server">
                        <td>
                            <!-- tabella1b data  -->
                            <table aria-hidden="true" id="Table1b" runat="server" border="0" borderstyle="None">
                                <tr borderstyle="None">
                                    <td>
                                        <!-- data saldo -->
                                        <table aria-hidden="true" id="Table_Data" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Data: </b>
                                                    <asp:TextBox ID="Txt_Data" runat="server" CssClass="txtUI datepicker">
                                                    </asp:TextBox>
                                                </td>                                        
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella1b -->
                        </td>
                    </tr>
                    <!-- fine riga1b -->

                    <!-- riga1c -->
                    <tr id="Riga1C" runat="server">
                        <td>
                            <!-- tabella1c considera saldi-->
                            <table aria-hidden="true" id="Table1C">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="TableConsideraSaldiRip" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="Chk_ConsideraSaldiRip" runat="server" Checked="True" CssClass="Testo_08_nero"
                                                        Text="Considera i Saldi di Riporto a Inizio Gestione Contabile" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella1c -->
                        </td>
                    </tr>
                    <!-- fine riga1c -->

                    <!-- riga1d -->
                    <tr id="Riga1d" runat="server">
                        <td>
                            <!-- tabella1d considera saldi-->
                            <table aria-hidden="true" id="Table1d" border="0" borderstyle="None" runat="server">
                                <tr>
                                    <td>
                                     <table aria-hidden="true" id="TableEscludiIvaIndetraibile" runat="server">
                                            <tr>
                                                <td>
                                         <asp:CheckBox ID="Chk_EscludiIvaIndetraibile" runat="server" Checked="false" CssClass="Testo_08_nero"
                                          Text="Escludi dal conteggio l'importo dell'iva indetraibile" />
                                           </td>
                                </tr>
                            </table>
</td>
                                </tr>
                            </table>
                            <!-- tabella1c -->
                        </td>
                    </tr>
                    <!-- fine riga1c -->


                    <!-- riga2 -->
                    <tr id="Riga2" runat="server">
                        <td>
                            <!-- tabella2 sezionali-->
                            <table aria-hidden="true" id="Table2">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Sezionali" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Sezionale:</b><asp:DropDownList ID="Cmb_Sezionali" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella2 -->
                        </td>
                    </tr>
                    <!-- fine riga2 -->
                    <!-- riga2b -->
                    <tr id="Riga2B" runat="server">
                        <td>
                            <!-- tabella2b check note corrispettivi-->
                            <table aria-hidden="true" id="Table2B">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="TableNoteCorrispettivi" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="Chk_NoteCorrispettivi" runat="server" Checked="True" CssClass="Testo_08_nero"
                                                        Text="Stampa le note (numerazione ricevute fiscali e corrispettivi)" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella2b -->
                        </td>
                    </tr>
                    <!-- fine riga2b -->


                    <!-- riga19 -->
                    <tr id="riga19" runat="server">
                        <td>
                            <!-- tabella19 -->
                            <table aria-hidden="true" id="Table19">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_AnnoConti" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Anno Contabile:</b>
                                                    <asp:DropDownList ID="Cmb_AnnoContabile" runat="server" CssClass="testo_08_nero"
                                                        AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella19 -->
                        </td>
                    </tr>
                    <!-- fine riga19 -->

                             <!-- riga20 -->
                    <tr id="Riga20" runat="server">
                        <td>
                            <!-- tabella20 -->
                            <table aria-hidden="true" id="Table20">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_ContiEco" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Riclassificazione:</b><asp:DropDownList ID="Cmb_Riclassificazione_Eco" runat="server"
                                                        CssClass="testo_08_nero" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Conto Economico:</b><asp:DropDownList ID="Cmb_Conti_Eco" runat="server" CssClass="testo_08_nero" >
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella20 -->
                        </td>
                    </tr>
                    <!-- fine riga20 -->

                     <!-- riga20b -->
                    <tr id="Riga20b" runat="server">
                        <td>
                            <!-- tabella20b -->
                            <table aria-hidden="true" id="Table20b">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Conti_PiuAnni" runat="server">
                                             <tr>
                                                <td>
                                                    <b>Conto:</b>
                                                    <asp:TextBox ID="Txt_Cercaconto" runat="server" CssClass="txtUI">
                                                    </asp:TextBox>
                                                    <asp:ImageButton ID="ImgBtn_CercaConto" runat="server" ImageUrl="../../AB_Immagini/Icone32/Lente.ico">
                                                    </asp:ImageButton>
                                                    <br />
                                                    <asp:DropDownList ID="Cmb_Conti_PiuAnni" runat="server" 
                                                        CssClass="testo_08_nero" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella20b -->
                        </td>
                    </tr>
                    <!-- fine riga20 -->

                    <!-- riga21 -->
                    <tr id="riga21" runat="server">
                        <td>
                            <!-- tabella21 -->
                            <table aria-hidden="true" id="Table21">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_ContiPat" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Riclassificazione:</b><asp:DropDownList ID="Cmb_Riclassificazione_Pat" runat="server"
                                                        CssClass="testo_08_nero" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Conto Patrimoniale:</b><asp:DropDownList ID="Cmb_Conti_Pat" runat="server" CssClass="testo_08_nero" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella21 -->
                        </td>
                    </tr>
                    <!-- fine riga21 -->
                    <!-- riga3 -->
                    <tr id="Riga3" runat="server">
                        <td>
                            <!-- tabella3 contatti -->
                            <table aria-hidden="true" id="Table3">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Contatti" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Rapporto Contabile :</b><asp:DropDownList ID="Cmb_RappContabili" runat="server"
                                                        CssClass="testo_08_nero" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Contatto :</b>
                                                    <asp:TextBox ID="Txt_CercaContatto" runat="server" CssClass="txtUI">
                                                    </asp:TextBox>
                                                    <asp:ImageButton ID="ImgBtn_CercaContatti" runat="server" ImageUrl="../../AB_Immagini/Icone32/Lente.ico">
                                                    </asp:ImageButton>
                                                    <br />
                                                    <asp:DropDownList ID="Cmb_Contatti" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella3 -->
                        </td>
                    </tr>
                    <!-- fine riga3 -->

                       <!-- riga3 -->
                    <tr id="Riga3bis" runat="server">
                        <td>
                            <!-- tabella3bis contatti -->
                            <table aria-hidden="true" id="Table_Contatti2" border="0" borderstyle="None" runat="server">
                                <tr>
                                    <td>
                                                                            
                                               
                                                    <b>Ricerca per nome:</b>
                                                    <asp:TextBox ID="Txt_CercaNomeContatto" runat="server" CssClass="txtUI">
                                                    </asp:TextBox>
                                                    <b>Ricerca per P.Iva/C.F.:</b>
                                                    <asp:TextBox ID="Txt_CercaPivaContatto" runat="server" CssClass="txtUI">
                                                    </asp:TextBox>
                                                    <asp:ImageButton ID="ImgBtn_RicercaContatto2" runat="server" ImageUrl="../../AB_Immagini/Icone32/Lente.ico">
                                                    </asp:ImageButton>
                                                    <br />
                                                    <asp:DropDownList ID="cmb_Contatti2" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                            
                                                                               
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella3bis -->
                        </td>
                    </tr>
                    <!-- fine riga3bis -->

                    <!-- riga4 -->
                    <tr id="Riga4" runat="server">
                        <td>
                            <!-- tabella4 istituto credito del contatto-->
                            <table aria-hidden="true" id="Table4">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_IstitutoCreditoCliente" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Istituto di Credito del Contatto:</b>
                                                    <asp:DropDownList ID="Cmb_IstitutoCreditoContatto" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella4 -->
                        </td>
                    </tr>
                    <!-- fine riga4 -->
                    <!-- riga5 -->
                    <tr id="Riga5" runat="server">
                        <td>
                            <!-- tabella5 istituto credito -->
                            <table aria-hidden="true" id="Table5">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_IstitutoCredito" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Istituto di credito:</b>
                                                    <asp:DropDownList ID="Cmb_IstitutoCredito" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella5 -->
                        </td>
                    </tr>
                    <!-- fine riga5 -->
                    <!-- riga5b -->
                    <tr id="Riga5b" runat="server">
                        <td>
                            <!-- tabella5 risorse finanziarie -->
                            <table aria-hidden="true" id="Table5b">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_RisFinanza" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Risorse Finanziarie:</b>
                                                    <asp:DropDownList ID="Cmb_RisorseFinanza" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella5b -->
                        </td>
                    </tr>
                    <!-- fine riga5b -->
                    <!-- riga6 -->
                    <tr id="Riga6" runat="server">
                        <td>
                            <!-- tabella6 agenti -->
                            <table aria-hidden="true" id="Table6">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Agenti" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Agente:</b>
                                                    <asp:DropDownList ID="Cmb_Agenti" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella6 -->
                        </td>
                    </tr>
                    <!-- fine riga6 -->
                    <!-- riga7 -->
                    <tr id="Riga7" runat="server">
                        <td>
                            <!-- tabella7 registri iva -->
                            <table aria-hidden="true" id="Table7">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_RegistriIVA" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Anno:</b>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_Anno" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Tipo Registro:</b>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_MensileTrimestrale" runat="server" border="0" CssClass="testo_08_nero"
                                                        AutoPostBack="True" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0">Mensile</asp:ListItem>
                                                        <asp:ListItem Value="1">Trimestrale</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:RadioButtonList ID="Rbl_Selezione" runat="server" border="0" CssClass="testo_08_nero"
                                                        AutoPostBack="True" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0">Tutti</asp:ListItem>
                                                        <asp:ListItem Value="1">Seleziona:</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr id="riga_trimestre">
                                                <td>
                                                    <b>Trimestre :</b>
                                                </td>
                                                <td>
                                                    <asp:CheckBoxList ID="Cbl_Trimestri" runat="server" border="0" CssClass="testo_08_nero"
                                                        RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="1">I</asp:ListItem>
                                                        <asp:ListItem Value="2">II</asp:ListItem>
                                                        <asp:ListItem Value="3">III</asp:ListItem>
                                                        <asp:ListItem Value="4">IV</asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                            <tr id="riga_mensile">
                                                <td colspan="2">
                                                    <b>Mensile:</b>
                                                </td>
                                            </tr>
                                            <tr id="riga_mensile_da">
                                                <td>
                                                    da:
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_Mese_Da" runat="server" CssClass="testo_08_nero" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr id="riga_mensile_a">
                                                <td>
                                                    a:
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_Mese_A" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella7 -->
                        </td>
                    </tr>
                    <!-- fine riga7 -->
                    <!-- riga8 -->
                    <tr id="Riga8" runat="server">
                        <td>
                            <!-- tabella8  verifica corrispettivi-->
                            <table aria-hidden="true" id="Table8">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_VerificaCorrispettivi" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:Button ID="Btn_VerificaAliquoteCorrisp" runat="server" Text="Verifica Aliquote">
                                                    </asp:Button>
                                                </td>
                                                <td>
                                                    Verifica aliquote movimentate nell'intervallo
                                                </td>
                                            </tr>
                                            <tr>
                                            <td colspan="2">
                                                <asp:CheckBox ID="Chk_StampaRiepilogo"  runat="server" Checked="True" CssClass="testo_08_nero"
                                                        Text="Stampa riepilogo Iva"/>
                                            </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella8 -->
                        </td>
                    </tr>
                    <!-- fine riga8 -->
                    <!-- riga9 -->
                    <tr id="Riga9" runat="server">
                        <td>
                            <!-- tabella9 numero di pagina -->
                            <table aria-hidden="true" id="Table9">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_NumPagina" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Numero di pagina:</b>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_NumPagina" runat="server" CssClass="txtUI" MaxLength="10">1</asp:TextBox>
                                                </td>
                                                <td>
                                                    (dal quale iniziare la numerazione)
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella9 -->
                        </td>
                    </tr>
                    <!-- fine riga9 -->
                    <!-- riga9b -->
                    <tr id="riga9b" runat="server">
                        <td>
                            <!-- tabella9b numero di riga -->
                            <table aria-hidden="true" id="Table9b">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_NumRiga" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Numero di registrazione:</b>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_NumRiga" runat="server" CssClass="txtUI" MaxLength="10">1</asp:TextBox>
                                                </td>
                                                <td>
                                                    (dal quale iniziare la numerazione)
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella9b -->
                        </td>
                    </tr>
                    <!-- fine riga9b -->
                    <!-- riga10 -->
                    <tr id="Riga10" runat="server" border="0">
                        <td>
                            <!-- tabella10 riba fatture -->
                            <table aria-hidden="true" id="Table10">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_RibaFatture" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_RibaFatture" runat="server" border="0" CssClass="testo_08_nero"
                                                        RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0">RIBA</asp:ListItem>
                                                        <asp:ListItem Value="1">Anticipo Fatture</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella10 -->
                        </td>
                    </tr>
                    <!-- fine riga10 -->
                    <!-- riga11 -->
                    <tr id="Riga11" runat="server" border="0">
                        <td>
                            <!-- tabella11 filtro riscossioni -->
                            <table aria-hidden="true" id="Table11">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Riscossioni" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Filtro riscossioni:</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_Riscossioni" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Insoluti (non riscosse o parzialmente riscosse)</asp:ListItem>
                                                        <asp:ListItem Value="1">Solo non riscosse</asp:ListItem>
                                                        <asp:ListItem Value="2">Solo parzialmente riscosse</asp:ListItem>
                                                        <asp:ListItem Value="3">Solo riscosse</asp:ListItem>
                                                        <asp:ListItem Value="4">Tutte</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella11 -->
                        </td>
                    </tr>
                    <!-- fine riga11 -->
                    <!-- riga12 -->
                    <tr id="Riga12" runat="server">
                        <td>
                            <!-- tabella12 scadenza -->
                            <table aria-hidden="true" id="Table12">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Scadenza" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Scadenza :</b>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_Scadenza" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Nessun Filtro</asp:ListItem>
                                                        <asp:ListItem Value="1">=</asp:ListItem>
                                                        <asp:ListItem Value="2">&lt;=</asp:ListItem>
                                                        <asp:ListItem Value="3">&lt;</asp:ListItem>
                                                        <asp:ListItem Value="4">&gt;=</asp:ListItem>
                                                        <asp:ListItem Value="5">&gt;</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_Scadenza" runat="server" CssClass="txtUI datepicker">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella12 -->
                        </td>
                    </tr>
                    <!-- fine riga12 -->
                       <!-- riga12b -->
                    <tr id="Riga12b" runat="server">
                        <td>
                            <!-- tabella12b scadenza -->
                            <table aria-hidden="true" id="Tabella12b_TipoPagamento" runat="server" border="0">
                                <tr>
                                     <td>
                                                    <b>Tipologie pagamento :</b>
                                                </td>
                                                <td>
                                                    <asp:CheckBoxList ID="CBL_TipoPagamento" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="1">RiBa</asp:ListItem>
                                                        <asp:ListItem Value="2">Bonifico</asp:ListItem>
                                                        <asp:ListItem Value="3">Contanti</asp:ListItem>
                                                        <asp:ListItem Value="4">Rimessa Diretta</asp:ListItem>
                                                        <asp:ListItem Value="5">Assegno</asp:ListItem>
                                                        <asp:ListItem Value="6">Carta di Credito</asp:ListItem>
                                                        <asp:ListItem Value="7">Bancomat</asp:ListItem>
                                                        <asp:ListItem Value="8">RID</asp:ListItem>
                                                        <asp:ListItem Value="9">MAV</asp:ListItem>
                                                        <asp:ListItem Value="10">Contrassegno</asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </td>
                                               
                                </tr>
                            </table>
                            <!-- tabella12b -->
                        </td>
                    </tr>
                    <!-- fine riga12b -->
                    <!-- riga13 -->
                    <tr id="Riga13" runat="server">
                        <td>
                            <!-- tabella13 filtro numeri fattura -->
                            <table aria-hidden="true" id="Table13">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_NumeriDoc" runat="server">
                                            <tr>
                                                <td colspan="2">
                                                    <b>Numero Documento:</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_NumeroDoc" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Nessun Filtro</asp:ListItem>
                                                        <asp:ListItem Value="1">=</asp:ListItem>
                                                        <asp:ListItem Value="2">&lt;=</asp:ListItem>
                                                        <asp:ListItem Value="3">&lt;</asp:ListItem>
                                                        <asp:ListItem Value="4">&gt;=</asp:ListItem>
                                                        <asp:ListItem Value="5">&gt;</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="Txt_NumeroDoc" runat="server" CssClass="txtUI">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>Anno Documento:</b>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_AnnoFatture" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella13 -->
                        </td>
                    </tr>
                    <!-- fine riga13 -->
                    <!-- riga14 -->
                    <tr id="Riga14" runat="server">
                        <td>
                            <!-- tabella14 ordinamento -->
                            <table aria-hidden="true" id="Table14">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Ordinamento" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Ordinamento: </b>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_Ordinamento" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella14 -->
                        </td>
                    </tr>
                    <!-- fine riga14 -->
                    <!-- riga15a -->
                    <tr id="Riga15a" runat="server">
                        <td>
                            <!-- tabella15 filtro conti ue e conto economico stato patrimoniale -->
                            <table aria-hidden="true" id="Table15a">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_Bilancio" runat="server">
                                            <tr>
                                                <td borderstyle="none">
                                                    <asp:CheckBoxList ID="ChkList_Bilancio" runat="server" border="0" 
                                                        BorderStyle="none" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Conto Economico</asp:ListItem>
                                                        <asp:ListItem Value="1" Selected="True">Stato Patrimoniale</asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella15a -->
                        </td>
                    </tr>
                    <!-- fine riga15a -->
                    <!-- riga15b -->
                    <tr id="Riga15b" runat="server">
                        <td>
                            <!-- tabella15 filtro conti ue e conto economico stato patrimoniale -->
                            <table aria-hidden="true" id="Table15b">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_ContiUE" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_ContiUE" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Conti UE e personalizzati</asp:ListItem>
                                                        <asp:ListItem Value="1">Solo conti UE</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella15b -->
                        </td>
                    </tr>
                    <!-- fine riga15b -->

                    <!-- riga15c -->
                    <tr id="Riga15c" runat="server">
                        <td>
                            <!-- tabella15 filtro conti ue e conto economico stato patrimoniale -->
                            <table aria-hidden="true" id="Table15c">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_SinteticoAnalitico" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_SinteticoAnalitico" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Bilancio sintetico</asp:ListItem>
                                                        <asp:ListItem Value="1">Bilancio analitico</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella15c -->
                        </td>
                    </tr>
                    <!-- fine riga15c -->


                    <!-- TrDettagliAuto -->
                    <tr id="TrDettagliAuto" runat="server">
                        <td>
                            <!-- Table15DetAuto dettagli stampe patrimoniale -->
                            <table aria-hidden="true" id="Table15DetAuto">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_DettagliAuto" runat="server">
                                            <tr>
                                     <td>
                                                    <b>Dettagli Stato Patrimoniale:  </b>&nbsp;
                                                </td>
                                                <td>
                                                    <asp:CheckBoxList ID="CheckBoxListDettagliAuto" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0">Crediti Verso Clienti</asp:ListItem>
                                                        <asp:ListItem Value="1" Selected="True">Depositi Bancari e Postali</asp:ListItem>
                                                        <asp:ListItem Value="2" Selected="True">Denaro e Valori in Cassa</asp:ListItem>
                                                        <asp:ListItem Value="3">Debiti Verso Fornitori</asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- Table15DetAuto -->
                        </td>
                    </tr>
                    <!-- fine TrDettagliAuto -->

                    <!-- riga16 -->
                    <tr id="Riga16" runat="server">
                        <td>
                            <!-- tabella16 conti movimentati-->
                            <table aria-hidden="true" id="Table16">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_ContiMovimenti" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_ContiMovimenti" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Solo conti movimentati nell'intervallo temporale</asp:ListItem>
                                                        <asp:ListItem Value="1">Anche conti non movimentati nell'intervallo ma con saldo <> 0 </asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella16 -->
                        </td>
                    </tr>
                    <!-- fine riga16 -->

                    <!-- riga16 -->
                    <tr id="Riga16a" runat="server">
                        <td>
                            <!-- tabella16a conto economico-->
                            <table aria-hidden="true" id="Table16a">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_CE_Layout" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Layout Conto Economico:  </b>&nbsp;
                                                </td>

                                                <td>
                                                    <asp:RadioButtonList ID="Rbl_CE_Layout" runat="server" border="0" CssClass="testo_08_nero">
                                                        <asp:ListItem Value="0" Selected="True">Layout Bilancio Europeo</asp:ListItem>
                                                        <asp:ListItem Value="1">Layout Costi-Ricavi</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella16 -->
                        </td>
                    </tr>
                    <!-- fine riga16 -->

                      <!-- riga16ab -->
                    <tr id="Riga16ab" runat="server">
                        <td>
                            <!-- tabella16ab no conti saldo 0-->
                            <table aria-hidden="true" id="Table16ab">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_No_Saldo0" runat="server">
                                            <tr>
                                                <td>                                                  
                                                  <asp:CheckBox ID="Chk_No_Saldo0" runat="server" CssClass="testo_08_nero" Checked="true"
                                                        Text="Non stampare i conti con saldo = 0"></asp:CheckBox>                                                       
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella16ab -->
                        </td>
                    </tr>
                    <!-- fine riga16ab -->

                      <!-- riga16b -->
                    <tr id="Riga16b" runat="server">
                        <td>
                            <!-- tabella16b saldo 0conti-->
                            <table aria-hidden="true" id="Table16b">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_SaldoConti0" runat="server">
                                            <tr>
                                                <td>                                                  
                                                  <asp:CheckBox ID="Chk_Saldo0" runat="server" CssClass="testo_08_nero"
                                                        Text="Stampa anche i conti con saldo = 0"></asp:CheckBox>                                                       
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella16b -->
                        </td>
                    </tr>
                    <!-- fine riga16b -->

                    <!-- riga18 -->
                    <tr id="Riga18" runat="server">
                        <td>
                            <!-- tabella18 filtro conti -->
                            <table aria-hidden="true" id="Table18">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_FiltroConti" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Imposta un filtro sui conti :</b>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImgBtn_Conti" runat="server" ImageUrl="../../AB_Immagini/Icone32/Albero.ico"
                                                        Height="32px" Width="32px"></asp:ImageButton>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="Chk_Conti" runat="server" CssClass="testo_08_nero" Enabled="False"
                                                        Text="Filtro impostato"></asp:CheckBox><a href="JavaScript:PianoConti();"></a>
                                                    <input id="Txt_FiltroConti" name="Txt_FiltroConti" size="1" runat="server"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella18 -->
                        </td>
                    </tr>
                    <!-- fine riga18 -->
                    <!-- riga22 -->
                    <tr id="riga22" runat="server">
                        <td>
                            <!-- tabella22 anno confronto -->
                            <table aria-hidden="true" id="Table22">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="Table_AnnoConfronto" runat="server">
                                            <tr>
                                                <td>
                                                    <b>Seleziona l'anno contabile da confrontare :</b>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="Cmb_AnnoContabile_Confronto" runat="server" CssClass="testo_08_nero">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella22 -->
                        </td>
                    </tr>
                    <!-- fine riga22 -->

                    <!-- riga23 -->
                    <tr id="Riga23" runat="server">
                        <td>
                            <!-- tabella check data di stampa -->
                            <table aria-hidden="true" id="Table23">
                                <tr>
                                    <td>
                                        <table aria-hidden="true" id="TableDataStampa" runat="server">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="Chk_DataStampa" runat="server" Checked="False" CssClass="Testo_08_nero"
                                                        Text="Visualizza la data di stampa sul report" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- tabella -->
                        </td>
                    </tr>
                    <!-- fine riga -->

                </table>
                <!-- pannello generale -->
            </td>
            <td valign="top" id="Pannello_stampa"  runat="server">
                <table aria-hidden="true"> 
                    <tr><td>
                    <asp:ImageButton ID="ImgBtnStampa" runat="server" ImageUrl="../../AB_Immagini/Icone32/Stampa.ico">
                    </asp:ImageButton>
                    &nbsp;&nbsp;&nbsp;Stampa
                    </td></tr>
                    <tr><td>
                    <asp:ImageButton ID="ImgBtnPDF" runat="server" ImageUrl="../../AB_Immagini/Icone32/PDF.ico">
                </asp:ImageButton>
                &nbsp;&nbsp;&nbsp;PDF</td></tr>
                </table>
                
                <br />
                <br />
                <table aria-hidden="true" id="Table_StampaProvaDefinitiva" runat="server">
                    <tr>
                        <td>
                            <asp:RadioButtonList ID="Rbl_ProvaDefinitiva" runat="server" border="0"
                                AutoPostBack="false" >
                                <asp:ListItem Value="0" Selected="True">Di Prova</asp:ListItem>
                                <asp:ListItem Value="1">Definitiva</asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Visualizza elenco report
                            <br />
                            <asp:ImageButton ID="ImgBtn_Elenco" CssClass="btn_per_load" runat="server" ImageUrl="../../AB_Immagini/Icone32/PianoConti.ico">
                            </asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
