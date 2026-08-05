<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/GSTBootstrap.master" CodeBehind="GST_Stampe_Menu.aspx.vb" Inherits="AgroAgenda_2010.GST_Stampe_Menu" %>

<%@ MasterType VirtualPath="~/Master/GSTBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="GST_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="GST_MainContent" runat="server">

    <div style="display: grid; grid-template-columns: 1fr 1fr; margin-top: 8px">

        <asp:Panel ID="PANEL1" style="margin-right: 15px" runat="server" Height="192px" BorderWidth="2px" BorderColor="#0000C0" BorderStyle="Solid" BackColor="WhiteSmoke">

            <div style="margin-bottom: 8px">
                <asp:Label ID="LABEL1" runat="server"
                    Width="400px" Height="24px" BackColor="#C0FFC0" CssClass="Testo_12_Nero_Bold" Font-Italic="True"
                    ForeColor="Green">&nbsp;Comunicazione alla Regione Emilia Romagna</asp:Label>
            </div>

            <div style="display: flex; margin-right: 14px">
                <div>

                    <asp:Label ID="LABEL6" runat="server"
                        Width="112px" Height="8px" BackColor="WhiteSmoke" CssClass="Testo_08_Nero_Bold" Visible="False">Intestazione :</asp:Label>
                    <asp:TextBox ID="Txt_RPT_SpecieVegetale" TabIndex="3" runat="server" Width="200px" Height="17px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
                        BorderStyle="None" MaxLength="50" Visible="False">Ravanello</asp:TextBox>
                    <asp:Label ID="LABEL7" runat="server"
                        Width="112px" Height="8px" BackColor="WhiteSmoke" CssClass="Testo_08_Nero" Visible="False">Specie Vegetale :</asp:Label>
                    <asp:RadioButton ID="Opt_RPT_FormatoPDF"
                        runat="server" Width="320px" Height="24px" BackColor="WhiteSmoke" CssClass="Testo_08_Blue" Checked="True"
                        GroupName="TipoStampa" Text="Stampa in formato <b>PDF</b> per Adobe Acrobat Reader" Visible="False"></asp:RadioButton>
                    <asp:RadioButton ID="Opt_RPT_FormatoXLS"
                        runat="server" Width="320px" Height="24px" BackColor="WhiteSmoke" CssClass="Testo_08_Blue" GroupName="TipoStampa"
                        Text="Stampa in formato <b>XLS</b> per Microsoft Excel" Visible="False"></asp:RadioButton>
                    <asp:Label ID="LABEL8" runat="server"
                        Width="112px" Height="8px" BackColor="WhiteSmoke" CssClass="Testo_08_Nero" Visible="False">Provincia :</asp:Label>
                    <asp:TextBox ID="Txt_RPT_Provincia"
                        TabIndex="3" runat="server" Width="200px" Height="17px" BackColor="PaleTurquoise" CssClass="Testo_08_Blue"
                        BorderStyle="None" MaxLength="50" Visible="False">Forli-Cesena</asp:TextBox>


                    <asp:RadioButtonList ID="Rbl_InterferenzeInterne"
                        runat="server" Width="288px" BackColor="WhiteSmoke" CssClass="Testo_08_Blue" AutoPostBack="True">
                        <asp:ListItem Value="0">Visualizza Interferenze Interne</asp:ListItem>
                        <asp:ListItem Value="1" Selected="True">Non Visualizzare Interferenze Interne</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div>
                    <asp:ImageButton ID="ImgBtn_Stampa_RPT"
                        runat="server" ToolTip="Stampa Risultati"></asp:ImageButton>

                    <br />
                    <asp:Label ID="LABEL4" runat="server"
                        Width="56px" Height="8px" BackColor="WhiteSmoke" CssClass="Testo_08_Rosso_Bold">Stampa</asp:Label>
                </div>
            </div>












        </asp:Panel>

        <asp:Panel style="" ID="Pannello_Interferenze" runat="server" Height="192px" BackColor="WhiteSmoke"
            BorderStyle="Solid" BorderColor="#0000C0" BorderWidth="2px">


            <div style="margin-bottom: 8px">
                <asp:Label ID="Lbl_Interferenze"
                    runat="server" Width="352px" Height="24px" BackColor="#C0FFC0" CssClass="Testo_12_Nero_Bold"
                    Font-Italic="True" ForeColor="Green">&nbsp;Elenco Dettagliato delle Interferenze</asp:Label>

            </div>

            <div style="display: flex">
                <div>
                    <asp:CheckBox ID="Chk_LOG_FlagRigheVuote"
                        runat="server" Height="16px" BackColor="WhiteSmoke" CssClass="Testo_08_Blue"
                        Text="Inserimento <b>RIGHE VUOTE</b> per appunti nella stampa dei risultati."></asp:CheckBox>

                    <br />

                    <asp:CheckBox ID="Chk_LOG_FlagRiepilogo"
                        runat="server" Height="16px" BackColor="WhiteSmoke" CssClass="Testo_08_Blue"
                        Checked="True" Text="Inserimento <b>RIEPILOGO</b> all'inizio della stampa."></asp:CheckBox>
                </div>
                <div>
                    <asp:ImageButton ID="ImgBtn_Stampa_LOG"
                        runat="server" ToolTip="Stampa Risultati"></asp:ImageButton>
                    <br />
                    <asp:Label ID="LABEL2" runat="server"
                        Width="56px" Height="8px" BackColor="WhiteSmoke" CssClass="Testo_08_Rosso_Bold">Stampa</asp:Label>
                </div>
            </div>




        </asp:Panel>

    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="GST_ScriptContent" runat="server">
</asp:Content>
