<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master"
    CodeBehind="Registro_RiBa.aspx.vb" Inherits="AgronicaStampe_2010.Registro_RiBa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- ordinamento -->
    <script src="../../../APP_Scripts/jquery.tablesorter_data_grid.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <!-- localizzazione italiana per il datapicker -->
    <script src="../../../App_Scripts/jquery.ui.datepicker-it.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <style type="text/css">
        th.header.headerSortDown
        {
            background-image: url(../../../App_Scripts/themes/blue/desc.gif);
            background-color: #3399FF;
        }
        
        th.header.headerSortUp
        {
            background-image: url(../../../App_Scripts/themes/blue/asc.gif);
            background-color: #3399FF;
        }
        
        th.header
        {
            background-image: url(../../../App_Scripts/themes/blue/bg.gif);
            cursor: pointer;
            font-weight: bold;
            background-repeat: no-repeat;
            background-position: center left;
            padding-left: 20px;
            border-right: 1px solid #dad9c7;
            margin-left: -1px;
            background-color: #5C9CCC;
            color: white;
        }
        .txtUI
        {
        }
        .style2
        {
            width: 464px;
        }
    </style>
    <script type="text/javascript">

        jQuery.logThis = function (text) {
            if ((window['console'] != undefined)) {
                console.log(text);
            }
        }

        function SelezionaDeselezionaTutti() {
            if ($('#ckSelezionaTuttiDettagli').is(':checked')) {
                //seleziono tutto
                $('.ChkSeleziona').each(function () {
                    $(this).children('input').attr('checked', 'checked');
                });
            }
            else {
                //seleziono tutto
                $('.ChkSeleziona').each(function () {
                    $(this).children('input').removeAttr('checked');
                });
            }
        }

        $(document).ready(function () {

            $('#<%=txt_ValiditaInizio.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $("#<%=gwRiba.ClientID %>").tablesorter({ headers: { 0: { sorter: false }, 1: { sorter: false }} });

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
            $.datepicker.regional['it'];


            $('#ckSelezionaTuttiDettagli').click(function () {
                SelezionaDeselezionaTutti();
            });

            $('#ckfiltroOnOff').click(function () {
                fckfiltroOnOff();
            });

        });

        function fckfiltroOnOff() {
            
            if ( $("#ckfiltroOnOff").is(':checked') ) {
                $("#filtroAvanzato").removeClass("displaynone");
            }
            else {
                $("#filtroAvanzato").addClass("displaynone");
            }

        }
        
        
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">
    <div>
        <div id="testata">
            <div class="padDxSx3px">
                <div class="boxColore padDxSx3px" style="width: 70%; float: left">
                    <div class="floatSX">
                        <div class="bold floatSX padDxSx3px" style="vertical-align: top">
                            1. Filtra per:</div>
                        <div class="bold floatSX" style="vertical-align: top">
                            <asp:ImageButton ImageUrl="~/AB_immagini/Icone32/Lente.ico" ID="btnRefreshFiltri"
                                runat="server" /></div>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="clear"></div>
                    <div class="floatSX" style="min-width: 420px">
                        <div>
                            <span class="floatSX padDxSx3px" style="min-width: 129px">Istituto di credito:</span>
                            <asp:DropDownList ID="Cmb_IstitutoCredito" runat="server" CssClass="myCombo floatSX">
                            </asp:DropDownList>
                        </div>
                        <div class="clear"></div>
                        <div>
                            <span style="min-width: 129px" class="floatSX padDxSx3px">Scadenza Ri.Ba.:</span>
<%--                            <asp:DropDownList ID="cmbScadenza" runat="server" CssClass="floatSX">
                                <asp:ListItem Value="0">Nessun Filtro</asp:ListItem>
                                <asp:ListItem Value="1">=</asp:ListItem>
                                <asp:ListItem Value="2">&lt;=</asp:ListItem>
                                <asp:ListItem Value="3">&lt;</asp:ListItem>
                                <asp:ListItem Value="4">&gt;=</asp:ListItem>
                                <asp:ListItem Value="5" Selected="True">&gt;</asp:ListItem>
                            </asp:DropDownList>--%>
                            <span class="floatSX padDxSx3px">Da:</span>
                            <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="txtUI datepicker floatSX"
                                MaxLength="10" Style="margin-left: 5px;" ToolTip="Data inizio" Width="77px">
                            </asp:TextBox>
                            <span class="floatSX padDxSx3px">A:</span>
                            <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="txtUI datepicker floatSX"
                                MaxLength="10" Style="margin-left: 5px;" ToolTip="Data Fine" Width="77px">
                            </asp:TextBox>
                            </div>
                            <div class="clear"></div>
                            <div>
                            <span style="min-width: 129px" class="floatSX padDxSx3px">Stato (causale CBI):</span>
                            <asp:DropDownList class="floatSX" ID="Cmb_cbiCausale" runat="server" CssClass="myCombo">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div id="filtroOnOff">
                    <input type="checkbox" id="ckfiltroOnOff" onclick="fckfiltroOnOff()" />
                    <label id="aa">Filtro Avanzato</label>
                    </div>
                    <div style="min-width: 300px" id="filtroAvanzato" class="displaynone">
                        <div style="vertical-align: top">
                            <span style="min-width: 129px" class="floatSX padDxSx3px">Numero Documento:</span>
                            <asp:DropDownList class="padDxSx3px" ID="cmbNDoc" runat="server">
                                <asp:ListItem Value="0" Selected="True">Nessun Filtro</asp:ListItem>
                                <asp:ListItem Value="1">=</asp:ListItem>
                                <asp:ListItem Value="2">&lt;=</asp:ListItem>
                                <asp:ListItem Value="3">&lt;</asp:ListItem>
                                <asp:ListItem Value="4">&gt;=</asp:ListItem>
                                <asp:ListItem Value="5">&gt;</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtNumeroDocumento" runat="server" CssClass="txtUI padDxSx3px" MaxLength="10"
                                Style="margin-left: 5px; width: 77px">
                            </asp:TextBox>
                        </div>
                        <div>
                            <span style="min-width: 129px" class="floatSX">Anno Documento:</span>
                            <asp:DropDownList class="floatSX" ID="Cmb_AnnoContabile" runat="server" CssClass="">
                            </asp:DropDownList>
                        </div>
                    </div>                    
                </div>
                <div class="floatSX boxColore padDxSx3px">
                    <div class="bold padDxSx3px" style="height: 30px">
                        3. Importa un file con gli esiti delle operazioni:</div>
                    <asp:FileUpload ID="RibaUp" runat="server" /><br />
                    <asp:ImageButton ID="btnLeggiCBI" runat="server" ImageUrl="~/AB_Immagini/Icone32/clip32.ico" />
                    <asp:Label CssClass="" ID="lblLeggiRiba" runat="server" Text="Carica a sistema File con supporti CBI Ri.Ba." />
                </div>
                <div class="clear">
                </div>
                <div class="boxColore">
                    <div class="bold padDxSx3px" style="height: 30px">
                        2. Operazione che si possono eseguire:</div>
                    <div class="floatSX">
                        <asp:ImageButton ID="btnStampa" runat="server" ImageUrl="~/AB_Immagini/Icone32/stampa.ico" />
                        <asp:Label ID="lblStampa" runat="server" Text="Stampa distinta richiesta anticipazioni Ri.Ba."
                            CssClass="padDxSx3px" />
                    </div>
                    <div class="floatSX">
                        <asp:ImageButton ID="BtnPDF" runat="server" ImageUrl="~/AB_Immagini/Icone32/PDF.ico" />
                        <asp:Label ID="lblPdf" runat="server" Text="PDF distinta richiesta anticipazioni Ri.Ba."
                            CssClass="padDxSx3px" />
                    </div>
                    <div class="floatSX">
                        <asp:ImageButton ID="btnEsportaCBI" runat="server" ImageUrl="~/AB_Immagini/Icone32/CertificatoSI.ico" />
                        <asp:Label ID="lblPredisponiFile" runat="server" Text="Esporta file con supporti CBI Ri.Ba. per elementi selezionati"
                            CssClass="padDxSx3px" />
                    </div>
                    <div class="floatSX">
                        <asp:ImageButton ID="btnAnnullaPresentazione" runat="server" ImageUrl="~/AB_Immagini/Icone32/ValidazioneNO.ico" />
                        <asp:Label ID="lblAnnullaPresentazione" runat="server" Text="Annulla Presentazione supporti CBI per elementi selezionati"
                            CssClass="padDxSx3px" />
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <div class="clear">
                </div>
                
            </div>
        </div>
        <div class="clear">
        </div>
        <div id="Div1" class="red" style="font-weight: bold">
            <asp:Label CssClass="" ID="lblEsitoRiba" runat="server" />
            <asp:HyperLink ID="lnkApriCartella" runat="server" Visible="false" />
            <asp:Button ID="btnApriFile" runat="server" Visible="false" Text="Apri" />
            <input type="hidden" runat="server" id="zfileToSaveTo" />
        </div>
        <div id="div2">
            <asp:ImageButton ID="btnConferma" runat="server" Visible="false" ImageUrl="../../../AB_immagini/Icone32/ValidazioneSI.ico" />
            <asp:Label CssClass="" ID="lblConferma" runat="server" Visible="false" Text="Conferma caricamento a sistema per gli elementi selezionati" />
        </div>
        <div class="clear">
        </div>
        <div id="dettaglio" class="boxColore">
            <asp:GridView ID="gwRiba" AutoGenerateColumns="False" runat="server" CellPadding="5"
                CssClass="ui-widget-content" AllowSorting="True" Style="margin-top: 5px; background-image: none;
                font-size: 0,7em">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <input type="checkbox" id="ckSelezionaTuttiDettagli" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="ckDettaglio" runat="server" CssClass="ChkSeleziona" />
                        </ItemTemplate>
                        <HeaderStyle Width="20px" />
                        <ItemStyle Width="20px" />
                        <FooterStyle Width="20px" />
                        <ControlStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Cod_Pagamento">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Stato">
                        <ItemTemplate>
                            <span style='color: <%#Eval("Colore")%>'>
                                <%#Eval("CBI_Causali_DesBreve")%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CBI_Causali_NuovoStato_Cod">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Stato di destinazione">
                        <ItemTemplate>
                            <span style='color: <%#Eval("CBI_Causali_NuovoStato_Colore")%>'>
                                <%#Eval("CBI_Causali_NuovoStato_DESBreve")%></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CBI_Causali_NuovoStato_DataScadenza">
                        <ItemStyle CssClass="displaynone" />
                        <HeaderStyle CssClass="displaynone" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Piva_Contatto" HeaderText="P.iva/c.f. Debitore" />
                    <asp:BoundField DataField="Rag_Soc_Contatto" HeaderText="Ragione sociale Debitore" />
                    <asp:BoundField DataField="Des_lib" HeaderText="Riferimenti Fattura" />
                    <asp:TemplateField HeaderText="IBAN Debitore e Banca Domiciliataria">
                        <ItemTemplate>
                            <%#Eval("Istituto_Des_Avere")%>
                            <br />
                            <span style="font-style: italic">
                                <%#Eval("Nazione_Avere")%>
                                <%#Eval("Cifre_Controllo_Avere")%>
                                <%#Eval("Cin_Avere")%>
                                <%#Eval("Abi_Avere")%>
                                <%#Eval("Cab_Avere")%>
                                <%#Eval("Numero_Avere")%>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Scadenza" HeaderText="Scadenza" DataFormatString="{0:dd/M/yyyy}" />
                    <asp:BoundField DataField="Num_Protocollo" HeaderText="Importo" />
                </Columns>
                <HeaderStyle CssClass="ui-widget-header" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
