<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="AnalisiRitiriTabacco.aspx.vb" Inherits="AgroAgenda_2010.AnalisiRitiriTabacco" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <!--include per l'albero-->
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.jstree.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui.combobox.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <style type="text/css">
        .bordomenu
        {
            border-bottom-color: #444444;
            border-bottom-width: 1px;
            border-bottom-style: solid;
        }
    </style>
    <script type="text/javascript">


        function pageLoad() {
            //al caricamento della pagina, ma prima di $(window).load
            clicktab();
        }

        //$(window).load(function () {
        $(window).on('load', function () {

        });




        function DoPostBack_EliminazioneControlli(key) {

            if (key == 'DEL') {

            }
        }

        $(document).ready(function () {

            //metto a visibile la classe main
            $('#main').show();
            //nascondo il load
            $('#load_main').hide();

            $('input:submit').button();

            $('#WaitFrame').hide();

        });




        //Per invocare il postback sulle combo
        function DoPostBack_Combo($_combo, valoreOpt) {

            if ($_combo.attr("id").endsWith("ComboImprese")) {
                $("#<%=BTN_ComboImprese.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }


        }





    </script>
    <style type="text/css">
        .txtUI
        {
            border: 1px solid #8DB9DB;
            padding: 0.1 0 0 0.1em;
            font-size: 1em;
            margin-bottom: 5px;
            margin-left: 0;
            margin-right: 0;
            margin-top: 0;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="UpdatePanelscript" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Bottone per l'eliminazione di un operazione -->
    <div class="main  box" id="Div3" style="min-width: 1000px; margin-left: 15px; margin-right: 15px;">
       <div  id="Div1" style=" margin-left: 15px; margin-right: 15px;">
       
        <div class="box" style="min-width: 350px" id="DivReport" runat="server">
            <div>
                <asp:Label ID="Label9" runat="server" meta:resourcekey="lblAzienda">Report Analitico Impresa</asp:Label>
                <div class="clear">
                </div>
                <div class="box50" style="min-width: 350px">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="lblAzienda" runat="server" meta:resourcekey="lblAzienda">Azienda</asp:Label>
                    </div>
                    <div class="valoriinput">
                        <cc1:ComboImprese ID="ComboImprese" runat="server" />
                        <asp:Button ID="BTN_ComboImprese" runat="server" Text="Button" Style="display: none" />
                    </div>
                </div>
                <div class="box50" style="min-width: 350px" runat="server" id="ProvenienzaRisorse">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="Label8" runat="server" meta:resourcekey="lblAzienda">Genera Report</asp:Label>
                    </div>
                    <div class="valoriinput">
                        <asp:ImageButton ID="ImageButtonAnalizza_Impresa" CssClass="btn_per_load" Enabled="false"
                            runat="server" ImageUrl="../AB_Immagini/icone32/Grafico03.ico" Style="width: 42px" />
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="box" style="min-width: 350px" id="DivImportaRicevimenti" runat="server">
            <div>
                <asp:Label ID="Label10" runat="server" meta:resourcekey="lblAzienda">Importazione Ricevimenti</asp:Label>
                <div class="clear">
                </div>
                <div class="box50" style="min-width: 350px">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="Label11" runat="server">Seleziona quali ricevimenti:</asp:Label>
                    </div>
                    <div class="valoriinput">
                        <asp:RadioButtonList ID="RadioButtonListImporta" runat="server">
                            <asp:ListItem Value="1">Reimporta Tutto</asp:ListItem>
                            <asp:ListItem Value="2" Selected="true">Recupera i Nuovi</asp:ListItem>
                            <asp:ListItem Value="3">Recupera i buchi</asp:ListItem>
                            <asp:ListItem Value="4">Test di risposta web service</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div class="valoriinput">
                        <asp:CheckBox ID="chkCosaImportareRitiro" runat="server" Text="Ritiro" />
                        <asp:CheckBox ID="chkCosaImportareLavorazione" runat="server" Text="Lavorazione" />
                        <asp:CheckBox ID="chkCosaImportareProdotto" runat="server" Text = "Prodotto finito" />
                    </div>
                </div>
                <div class="box50" style="min-width: 350px">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="Label1" runat="server">Avvia Importazione: </asp:Label>
                    </div>
                    <div class="valoriinput">
                        <asp:ImageButton ID="ImageButtonImporta" CssClass="btn_per_load" runat="server" ImageUrl="../AB_Immagini/icone32/movimenti.png"
                            Style="width: 42px" />
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="box" style="min-width: 350px" id="DivRintraccia" runat="server">
            <div>
                <asp:Label ID="Label12" runat="server" meta:resourcekey="lblAzienda">Rintracciata dei Colli </asp:Label>
                <div class="clear">
                </div>
                <div class="box50" style="min-width: 350px">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="Label13" runat="server">Seleziona cosa rintracciare:</asp:Label>
                    </div>
                    <div class="valoriinput">
                        <asp:RadioButtonList ID="RadioButtonListRintraccia" runat="server">
                            <asp:ListItem Value="1">Tutti i Ricevimenti</asp:ListItem>
                            <asp:ListItem Value="2" Selected="true">Solo quelli non ancora Tracciati</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="box50" style="min-width: 350px">
                    <div class="descrizione" style="width: 80px">
                        <asp:Label ID="Label14" runat="server">Avvia Rintraccio: </asp:Label>
                    </div>
                    <div class="valoriinput">
                        <asp:ImageButton ID="ImageButtonAnalizza" CssClass="btn_per_load" runat="server"
                            ImageUrl="../AB_Immagini/icone32/Trova2.ico" Style="width: 42px" />
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
        </div>

        <div class="box" style="font-size: large">
            Risultato:
            <asp:Label ID="LabelRes" runat="server">--</asp:Label>
        </div>
    </div>
</asp:Content>
