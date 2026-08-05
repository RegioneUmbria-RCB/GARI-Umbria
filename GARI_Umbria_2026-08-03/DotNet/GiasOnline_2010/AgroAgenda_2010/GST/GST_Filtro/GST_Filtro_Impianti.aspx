<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/GSTBootstrap.master" CodeBehind="GST_Filtro_Impianti.aspx.vb" Inherits="AgroAgenda_2010.GST_Filtro_Impianti" %>

<%@ MasterType VirtualPath="~/Master/GSTBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="GST_HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="GST_MainContent" runat="server">

    <asp:UpdatePanel ID="update_panel" runat="server" style="padding-top:15px;">

        <ContentTemplate>

            <div>

                <%--griglia filtri ricerca--%>
                <div style="display: grid; grid-template-columns: 1fr 1fr 1fr 1fr">

                    <%--imprese referenti--%>
                    <div id="gstPanFiltroImprese" style="margin-right:4px">
                        <asp:Label ID="Label1" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 139px"
                            Text="Impresa Referente :"></asp:Label>
                        <asp:ImageButton ID="ImgBtn_Imprese_Seleziona" runat="server" 
                            Style="height: 24px; width: 24px; margin-top: -15px;" ToolTip="Seleziona tutti" />
                        <asp:ImageButton ID="ImgBtn_Imprese_DeSeleziona" runat="server" 
                            Style="height: 24px; width: 24px; margin-top: -15px;" ToolTip="Deseleziona tutti" />
                        <asp:Panel ID="Pannello_xImpreseReferenti" runat="server" BackColor="#FFE0E0" BorderColor="Gray"
                            BorderStyle="Solid" BorderWidth="1px" Height="240px" Style="width: 100%; padding: 5px"
                            ScrollBars="Vertical">
                            <asp:CheckBoxList ID="CBL_ImpreseReferenti" runat="server" BorderColor="#000000"
                                BorderWidth="0px" CssClass="Testo_07_Nero" Style="height: 40px; width: 100%">
                            </asp:CheckBoxList>
                        </asp:Panel>
                    </div>

                    <div id="gstFiltroDate" style="margin-right:4px">
                        <asp:Label ID="Label2" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 104px"
                            Text="Periodo di validità:"></asp:Label><br />
                        <asp:Panel ID="Panel2" runat="server" BackColor="#C0FFFF" BorderColor="Gray" BorderStyle="Solid"
                            BorderWidth="1px" Style="height: 240px; padding: 5px">
                            <asp:Label ID="Label8" runat="server" CssClass="Testo_07_Nero" Style="width: 150px"
                                Text="Data di <b>INIZIO</b> dell'impianto"></asp:Label>
                            <br />
                            <asp:RadioButton ID="Opt_Inizio_PrecedenteAl" runat="server" CssClass="Testo_07_Nero"
                                GroupName="Data_Inizio_Impianto" Style="width: 138px" Text="uguale o precedente al"
                                ToolTip="La data di INIZIO dell'impianto e' precedente o uguale alla data riportata nella casella" />
                            <br />
                            <asp:RadioButton ID="Opt_Inizio_SuccessivoAl" runat="server" Checked="True" CssClass="Testo_07_Nero"
                                GroupName="Data_Inizio_Impianto" Style="width: 138px" Text="uguale o successiva al"
                                ToolTip="La data di INIZIO dell'impianto e' successiva o uguale alla data riportata nella casella" />
                            <br />
                            <asp:TextBox ID="Txt_DataInizioImpianto" runat="server" CssClass="Testo_07_Blue datepicker"
                                Style="text-align: right; float: right; margin-right: 10px">01/01/1900</asp:TextBox>
                            <div style="clear: both">
                            </div>
                            <br />
                            <br />
                            <asp:Label ID="Label9" runat="server" CssClass="Testo_07_Nero" Style="width: 150px"
                                Text="Data di <b>FINE</b> dell'impianto"></asp:Label>
                            <br />
                            <asp:RadioButton ID="Opt_Fine_PrecedenteAl" runat="server" Checked="True" CssClass="Testo_07_Nero"
                                GroupName="Data_Fine_Impianto" Style="width: 138px" Text="uguale o precedente al"
                                ToolTip="La data di FINE dell'impianto e' precedente o uguale alla data riportata nella casella" />
                            <br />
                            <asp:RadioButton ID="Opt_Fine_SuccessivaAl" runat="server" CssClass="Testo_07_Nero"
                                GroupName="Data_Fine_Impianto" Style="width: 138px" Text="uguale o successiva al"
                                ToolTip="La data di FINE dell'impianto e' successiva o uguale alla data riportata nella casella" />
                            <br />
                            <asp:TextBox ID="Txt_DataFineImpianto" runat="server" CssClass="Testo_07_Blue datepicker"
                                Style="text-align: right; float: right; margin-right: 10px">01/01/1900</asp:TextBox>
                            <div style="clear: both">
                            </div>
                        </asp:Panel>
                    </div>


                    <div id="gstFiltroSementi" style="margin-right:4px">

                        <asp:Label ID="Label5" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 104px"
                            Text="Sementi :"></asp:Label><br />
                        <asp:Panel ID="Panel1" runat="server" BackColor="#B0FFB0" BorderColor="Gray" BorderStyle="Solid"
                            BorderWidth="1px" ScrollBars="Vertical" Style="height: 240px; padding: 5px">

                            <asp:RadioButtonList ID="RBL_Sementi" runat="server" BorderWidth="0px"
                                CssClass="Testo_07_Nero" ForeColor="Blue" AutoPostBack="True">
                            </asp:RadioButtonList>

                        </asp:Panel>

                    </div>

                    <div id="gstFiltroSpecie" style="margin-right:4px">
                        <asp:Label ID="Label6" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 124px"
                            Text="Specie Vegetale :"></asp:Label>
                        <asp:ImageButton ID="ImgBtn_Specie_DeSeleziona" runat="server" 
                            ToolTip="Deseleziona tutti" Style="margin-top: -15px;" />
                        <br />
                        <asp:Panel ID="Panel3" runat="server" BackColor="#E0FFE0" BorderColor="Gray" BorderStyle="Solid"
                            BorderWidth="1px" ScrollBars="Vertical" Style="height: 240px; padding: 5px">
                            <asp:CheckBoxList ID="CBL_SpecieVegetali" runat="server" BorderWidth="0px" CssClass="Testo_07_Nero"
                                Style="width: 100%; height: 40px" AutoPostBack="True">
                            </asp:CheckBoxList>
                        </asp:Panel>
                    </div>

                </div>

                <asp:Panel ID="Pannello_Risultato" runat="server" BorderStyle="none" BorderWidth="0px" Height="850px" Width="100%" Style="overflow: hidden;">
                    <div class="row" style="margin-top:20px" id="rowBtnEstrazioni">
                        <div class="col-lg-3">
                            <div style="display: grid; gap: 10px;">
                                <div id="btnSalvaPreventivo" class="k-button"><span class="fa fa-table fa-2x"></span>Consolida Preventivo</div>
                            </div>
                        </div>
                        <div class="col-lg-3">
                            <div style="display: grid; gap: 10px;">
                                <div id="btnSalvaConsuntivo" class="k-button"><span class="fa fa-table fa-2x"></span>Consolida Consuntivo</div>
                            </div>
                        </div>
                        <div class="col-lg-6">&nbsp;</div>
                    </div>
                    <div id="GrigliaRisultato_Container" style="padding-top:20px; height:100%;">
                        <div id="GrigliaRisultato">
                        </div>
                    </div>

                </asp:Panel>

            </div>

        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="GST_ScriptContent" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Filtro_ImpiantiJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Filtro_Impianti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Filtro_Impianti_ws_client.js") %>"></script>

</asp:Content>

