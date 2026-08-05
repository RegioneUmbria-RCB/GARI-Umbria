<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/OperazioneBootstrap.master" CodeBehind="Rilievi_2.aspx.vb" Inherits="AgroAgenda_2010.Rilievi_2" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Master/OperazioneBootstrap.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentOperazioniHeader" runat="server">

    <style>
        .Fioritura {
            box-shadow: 1px 1px rgba(0,0,0,.075) inset;
            background-color:  lightcoral;
        }
    </style>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Rilievi_2_kendoEvents.js") %>"></script>
    <script type="text/javascript">


        function ValidaxSubmit() {

            //rimuovo quanto disabilitato per poter ottenere i dati lato server:
            //http://stackoverflow.com/questions/7357256/disabled-form-inputs-do-not-appear-in-the-request

            var grid = $("#divRilievi").data("kendoGrid");
            var data = grid.dataSource.data();

            $("#" + hdRilievi_clientID).val(JSON.stringify(data));

            //click per salvataggio
            $("#<%=Master.Property_ImgBtn_Salva.ClientID %>").click();

        }

        function SupTrattata() {
            return $('#<%=Txt_SupTrattata.ClientId %>').val().replace(',', '.');
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

        function AggiornaDopo_SupTrattata() {

            $('#<%=btn_MostraTrappole.ClientId %>').click();

            //Btn_AggiornaDosiGriglia_click();
        }


    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentOperazioniContenuti" runat="server">
    <div class="jumbotron" style="padding-left: 15px; padding-right: 15px;">
        <div class="col-md-12">

            <asp:UpdatePanel ID="UpdateSuperfici" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-md-6 col-xs-6">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <asp:Label ID="lblSupHaSelezionata" runat="server" meta:resourcekey="lblSupHaSelezionataResource1"
                                            CssClass="input-group-addon alert-info">Sup. [ha] Selezionata</asp:Label>
                                        <input type="text" runat="server" class="form-control SommaSuperficie" id="Txt_SupSelezionata"
                                            readonly="readonly" disabled="disabled" value="0" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class=" col-md-6 col-xs-6">
                            <div class="form-horizonta">
                                <div class="form-group">
                                    <div class="input-group">
                                        <asp:Label ID="lblSupHaTrattata" runat="server" meta:resourcekey="lblSupHaTrattataResource1"
                                            CssClass="input-group-addon alert-info">Sup. [ha] Trattata</asp:Label>
                                        <input type="text" runat="server" class="form-control SommaSuperficieTrattata" id="Txt_SupTrattata"
                                            value="0" />
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="row" >

            <asp:UpdatePanel ID="UpdatePanel_Tabella" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <!-- BOTTONE INSERISCI  -->
                    <div class="col-md-12 nopadding">
                        <hr />
                    </div>
                    <div class="col-md-12 text-center">
                        <div id="MostraTabella" class="btn btn-info btn_per_load" onclick="$('#<%=btn_MostraTrappole.ClientId %>').click(); letturaTabella_Kendo(); "  >
                            <i class="fa fa-arrow-down"></i>
                            <asp:Label ID="lblMostraTrappole" runat="server">VISUALIZZA DETTAGLI</asp:Label>
                            <i class="fa fa-arrow-down"></i>
                        </div>
                                        
                        <asp:Button  ID="btn_MostraTrappole"  runat="server"   Style="display: none" />
                         
                        <div id="NascondiTabella" class="btn btn-info btn_per_load" onclick="$('#<%= Btn_NascondiTrappole.ClientID%>').click();" >
                            <i class="fa fa-arrow-top"></i>
                            <asp:Label ID="Label1" runat="server">SBLOCCA APPEZZAMENTI</asp:Label>
                            <i class="fa fa-arrow-top"></i>
                        </div>

                        <asp:Button ID="Btn_NascondiTrappole"  runat="server" Style="display: none" />

                    </div>

                    <div class="row">
                        <div id="divRilievi"></div>
                        <input type="hidden" id="hdRilievi" name="hdRilievi"  runat="server" />
                        <input type="hidden" id="hdImpianti" name="hdImpianti" runat="server" />
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>


    </div>
    <script>

        var hdRilievi_clientID = "<%= hdrilievi.clientid%>";

    </script>

</asp:Content>

