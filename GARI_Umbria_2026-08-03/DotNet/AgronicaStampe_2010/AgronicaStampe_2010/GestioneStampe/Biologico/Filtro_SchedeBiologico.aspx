<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master"
    CodeBehind="Filtro_SchedeBiologico.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_SchedeBiologico" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
        });
    </script>
    <style type="text/css">
        .style1
        {
            width: 426px;
        }
        .style2
        {
            width: 137px;
        }
        .style3
        {
            width: 185px;
        }
        .style4
        {
            width: 42px;
        }
        .style5
        {
            width: 105px;
        }
        .style6
        {
            width: 33px;
        }
        .style7
        {
            width: 465px;
        }
        .style8
        {
            width: 250px;
        }
        .style9
        {
            width: 43px;
        }
        .style10
        {
            width: 537px;
        }
        .style11
        {
            width: 211px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">
    <table aria-hidden="true" id="Pannello_Filtri" runat="server">
        <%--prima riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_TipoReport" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td style="padding-left: 5px;">
                            <b>Seleziona la scheda da stampare:</b>
                        </td>
                        <td>
                            <asp:RadioButtonList ID="OptionList_SchedaBiologico" runat="server" Width="260px"
                                AutoPostBack="True" RepeatLayout="Flow">
                                <asp:ListItem Value="21" Selected="True">Scheda Materie Prime</asp:ListItem>
                                <asp:ListItem Value="22">Scheda Vendite</asp:ListItem>
                                <asp:ListItem Value="153">Registro Preparazioni</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImgBtn_Stampa" CssClass="btn_per_load" runat="server" ImageUrl="../../AB_Immagini/Icone32/stampa.ico">
                            </asp:ImageButton>
                            &nbsp;&nbsp;&nbsp; Stampa la scheda
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--seconda riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_ImpresaCentro" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td style="padding-left: 5px;" class="style2">
                            <b>Ricerca Impresa:</b>
                        </td>
                        <td class="style1">
                            <asp:TextBox ID="Txt_Impresa" runat="server" CssClass="txtUI"></asp:TextBox>
                            &nbsp; &nbsp; &nbsp;
                            <asp:ImageButton ID="ImgBtn_Cerca" CssClass="btn_per_load" runat="server" ImageUrl="../../AB_Immagini/Icone32/lente.ico">
                            </asp:ImageButton>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;" class="style2">
                            <b>Impresa:</b>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="Cmb_Impresa" runat="server" Height="22px" Width="410px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            Trovate:<b><asp:Label ID="Lbl_NumImprese" runat="server" Style="margin-left: 5px;" /></b>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;" class="style2">
                            <b>Centro Aziendale:</b>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="Cmb_CentroAziendale" runat="server" Height="22px" Width="410px"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <i>Centro Aziendale dal quale prelevare il Codice Operatore Bio, l'Organismo di Controllo e l'Indirizzo per l'intestazione</i>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;" class="style2">
                            <b>Magazzino:</b>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="Cmb_Magazzino" runat="server" Height="22px" Width="410px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <i>Lasciare 'tutti i Magazzini' per stampare tutti i magazzini
                                <br />
                                dell'Azienda</i>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--terza riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_Date" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td style="padding-left: 5px;" class="style3">
                            <b>Intervallo temporale:</b>
                        </td>
                        <td class="style4">
                            <b>Dal</b>
                        </td>
                        <td class="style5">
                            <asp:TextBox ID="TxtDataDa" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                Style="margin-left: 5px;" ToolTip="Data di riferimento" Width="77px">
                            </asp:TextBox>
                        </td>
                        <td class="style6">
                            <b>Al</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TxtDataA" runat="server" CssClass="txtui datepicker" MaxLength="10"
                                Style="margin-left: 5px;" ToolTip="Data di riferimento" Width="77px">
                            </asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--quarta riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_Categorie" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td class="style7" style="padding-left: 5px;">
                            <b>Filtro Categorie Magazzino:</b>
                            <p>
                                <asp:CheckBoxList ID="ChkList_Categorie" runat="server" Width="424px" Height="6px">
                                </asp:CheckBoxList>
                            </p>
                        </td>
                        <td>
                            <p>
                                <b>Seleziona il tipo di arrotondamento</b> (Scheda Materie Prime):
                                <br />
                                <asp:RadioButtonList ID="Rbl_Arrotondamento" runat="server" CssClass="Testo_08_Blue">
                                    <asp:ListItem Value="-1">Nessuno</asp:ListItem>
                                    <asp:ListItem Value="0">Unit&#224;</asp:ListItem>
                                    <asp:ListItem Value="1">1 Decimale</asp:ListItem>
                                    <asp:ListItem Value="2" Selected="True">2 Decimali</asp:ListItem>
                                    <asp:ListItem Value="3">3 Decimali</asp:ListItem>
                                    <asp:ListItem Value="4">4 Decimali</asp:ListItem>
                                </asp:RadioButtonList>
                            </p>
                            <b>Seleziona la modalità di stampa del lotto</b>
                            <br />
                            <asp:RadioButtonList ID="Rbl_StampaLotto" runat="server" CssClass="Testo_08_Blue">
                                <asp:ListItem Value="0" Selected="True">Stampa sempre il lotto</asp:ListItem>
                                <asp:ListItem Value="1">Stampa in base alla configurazione del prodotto</asp:ListItem>
                            </asp:RadioButtonList>
                            </p>
                            <p>
                                <b>Stampa del codice articolo:</b>
                                <br />
                                <asp:CheckBox ID="Chk_StampaCodArticolo" runat="server" Text="Stampa cod. articolo per sementi, semilavorati, trasformati"
                                    Checked="False" />
                            </p>
                            <p>
                                <b>Filtro Classi Prodotto:</b>
                                <br />
                                <asp:DropDownList ID="Cmb_ClassiProdotto" runat="server" Height="22px" Width="410px">
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:CheckBox ID="Chk_ConsistenzeVasca" runat="server" Text="Leggi Consistenze di Vasca" /><br />
                                <asp:CheckBox ID="Chk_MostraDataOdiernaStampa" runat="server" Text="Mostra data di stampa" />
                            </p>
                            <p>
                                <a href="mailto:banchedati@agronica.it?subject=Segnalazione: fitofarmaci/fertilizzanti biologici ma visualizzati nella scheda materie prime come convenzionali">
                                    <img id="assistenza" border="0" name="assistenza" alt="Invia segnalazione a BancheDati"
                                        src="../../AB_Immagini/icone32/Posta32.ico"></a> <i>Segnala fitofarmaci/fertilizzanti
                                            biologici ma visualizzati come convenzionali nella Scheda Materie Prime Bio</i>
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--quinta riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_xVendite" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td class="style8" style="padding-left: 5px;">
                            <b>Filtro su un singolo Prodotto:</b>
                            <br />
                            <br />
                            Cod. Articolo:
                            <asp:TextBox ID="Txt_CodArticolo" runat="server" CssClass="txtUI"></asp:TextBox>
                            <br />
                            Descrizione:
                            <asp:TextBox ID="Txt_MatDes" runat="server" CssClass="txtUI"></asp:TextBox>
                        </td>
                        <td class="style9">
                            <asp:ImageButton ID="ImgBtn_CercaProdotto" CssClass="btn_per_load" runat="server"
                                ImageUrl="../../AB_Immagini/Icone32/lente.ico"></asp:ImageButton>
                        </td>
                        <td>
                            <i>Vengono cercati i prodotti biologici (delle categorie e della classe prodotto selezionati
                                nel pannello superiore) che soddisfano il criterio di ricerca impostato qui a sinistra
                            </i>
                        </td>
                        <td>
                            <asp:DropDownList ID="Cmb_ProdottoxVendite" runat="server" Height="22px" Width="410px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style8" style="padding-left: 5px;">
                            <b>Filtro Contatti:</b>
                            <br />
                            <br />
                            P.IVA/C.F.:
                            <asp:TextBox ID="Txt_CodContatto" runat="server" CssClass="txtUI"></asp:TextBox>
                            <br />
                            Rag.Soc/Nome:
                            <asp:TextBox ID="Txt_ContattoDes" runat="server" CssClass="txtUI"></asp:TextBox>
                        </td>
                        <td class="style9">
                            <asp:ImageButton ID="ImgBtn_CercaContatto" CssClass="btn_per_load" runat="server"
                                ImageUrl="../../AB_Immagini/Icone32/lente.ico"></asp:ImageButton>
                        </td>
                        <td colspan="2">
                            <asp:DropDownList ID="Cmb_Contatti" runat="server" Height="22px" Width="650px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--sesta riga tabella generale--%>
        <tr>
            <td>
                <table aria-hidden="true" id="Pannello_Preparati" class="boxColore" runat="server" style="width: 950px;">
                    <tr>
                        <td class="style10" colspan="2" style="padding-left: 5px;">
                            <b>Seleziona il Semilavorato o il Trasformato:</b>
                            <br />
                            <br />
                            <asp:DropDownList ID="Cmb_Prodotti" runat="server" Height="22px" Width="150px" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <i>Elenco prodotti biologici abilitati per la stampa</i>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr>
                        <td class="style10" colspan="2" style="padding-left: 5px;">
                            <b>Seleziona la Linea di Produzione o la Preparazione:</b>
                            <br />
                            <br />
                            <asp:DropDownList ID="Cmb_Preparazioni" runat="server" Height="22px" Width="150px"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <i>Elenco delle Linee Produzione e/o delle Preparazioni del prodotto selezionato</i>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr>
                        <td class="style11" style="padding-left: 5px;">
                            <b>Seleziona le sezioni:</b>
                        </td>
                        <td>
                            <asp:CheckBoxList ID="ChkList_SezioniPrep" runat="server" Width="241px" Height="6px"
                                RepeatDirection="Horizontal">
                                <asp:ListItem Value="0" Selected="True">Sezione A</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Sezione B</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td>
                            <asp:CheckBox ID="Chk_LogoRegione" runat="server" Width="197px" Text="Stampa il logo della regione"
                                Checked="True"></asp:CheckBox>&nbsp;&nbsp;&nbsp;
                            <asp:DropDownList ID="Cmb_Regioni" runat="server" Height="22px" Width="150px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--settima riga tabella generale--%>
        <tr id="Riga_regione">
            <td class="style10" style="padding-left: 5px;" width="100%">
                <asp:CheckBox ID="chk_Regione2" runat="server" Width="323px" Text="Seleziona la regione di riferimento"
                    Checked="True"></asp:CheckBox>&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="Cmb_Regioni2"                        runat="server" Height="22px" Width="150px" />
            </td>
        </tr>
    </table>
</asp:Content>
