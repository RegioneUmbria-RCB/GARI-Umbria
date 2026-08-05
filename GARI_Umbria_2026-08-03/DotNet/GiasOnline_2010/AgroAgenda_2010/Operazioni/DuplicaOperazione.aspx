<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    ValidateRequest="false" CodeBehind="DuplicaOperazione.aspx.vb" Inherits="AgroAgenda_2010.DuplicaOperazione"
    meta:resourcekey="PageResource1" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        /* il foglio di stile generale, ingrossa le label ma non le caselle adiacenti...restringo*/
        .input-group > span {
             height: auto !important; 
        }  
        
        #MsgNoCopiaMagazzino {
             color:red; 
             font-weight: bold;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_CurrentPiva" runat="server" />
    <asp:HiddenField ID="hd_dtInterventi" runat="server" />
    <asp:HiddenField ID="hd_ids" runat="server" />
    <asp:HiddenField ID="hd_isFertirrigazione" runat="server" Value="False" />

    <asp:UpdatePanel ID="UpdatePanelScript" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="updateHidden" runat="server">
        <ContentTemplate>
            <input type="hidden" id="HiddenVarie" runat="server" />
            <input type="hidden" id="hdOperazioniSelezionate" runat="server" />
            <input type="hidden" id="hdKendo_Impianti" runat="server" />
            <input type="hidden" id="hdImpiantiSelezionati" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="container" style="margin-bottom: 70px">

        <div class="row" style="margin-bottom: 10px; padding: 0 15px;">

            <asp:Button ID="Btn_FiltroneNuovo" ToolTip="Seleziona le aziende su cui copiare l'intervento in tutti gli impianti"
                runat="server" Text="FILTRO NUOVO" Style="display: none; margin-left: 30px; margin-right: 10px; color: red"
                CssClass="bottone btn_per_load Btn_FiltroneNuovo" Visible="false" />
            <div class="col-md-2">
                <div class="btn btn-success" id="cBtn_FiltroneNuovo">
                    <span class="fa fa-search lampeggiante"></span>
                    <span class="lampeggiante">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltraImpianti %>" runat="server">Filtra Impianti</asp:Localize>
                    </span>
                </div>
            </div>

            <asp:Button ID="Btn_FiltraAziende" ToolTip="Seleziona le aziende su cui copiare l'intervento in tutti gli impianti"
                runat="server" Text="FILTRA AZIENDE" Style="display: none; margin-left: 30px; margin-right: 10px; color: red"
                CssClass="bottone btn_per_load Btn_FiltraAziende" Visible="false" />
            <div class="col-md-2 hideBtn">
                <div class="btn btn-success" id="cBtn_FiltraAziende">
                    <span class="fa fa-search lampeggiante"></span>
                    <span class="lampeggiante">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltraAziende %>" runat="server">Filtra Aziende</asp:Localize>
                    </span>
                </div>
            </div>

            <asp:Button ID="Btn_FiltraImpianti" ToolTip="Seleziona gli Impianti su cui copiare l'intervento"
                runat="server" Text="FILTRO AVANZATO" Style="display: none; margin-left: 5px; margin-right: 5px; color: green; font-size: 11px"
                CssClass="bottone btn_per_load Btn_FiltraImpianti" Visible="false" />
            <div class="col-md-2 hideBtn">
                <div class="btn btn-success" id="cBtn_FiltraImpianti">
                    <span class="fa fa-search lampeggiante"></span>
                        <span class="lampeggiante">                        
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltroAvanzato %>" runat="server">Filtro Avanzato</asp:Localize>
                        </span>
                </div>
            </div>

            <asp:Button ID="Btn_SalvaTutto" ToolTip="Salva le nuove operazioni" runat="server"
                Text="SALVA" Style="display: none; margin-left: 30px; margin-right: 5px; color: red" CssClass="bottone btn_per_load Btn_SalvaTutto"
                meta:resourcekey="ImgBtnSalvaTuttoResource1" />
            <div class="col-md-2">
                <div class="btn btn-success" id="cBtn_SalvaTutto">
                    <span class="fa fa-save lampeggiante"></span>
                    <span class="lampeggiante">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                    </span>
                </div>
            </div>


            <div class="col-lg-4 col-md-4 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon lbl_required" id="spn_ImpostaCopiaCosti" for="chk_ImpostaCopiaCosti">
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CopiaDatiMacchineOperatori %>" runat="server">Copia i dati di Macchine ed Operatori</asp:Localize>
                            </span>
                            <asp:CheckBox ID="chk_ImpostaCopiaCosti" runat="server" CssClass="" meta:resourcekey="chk_ImpostaCopiaCostiResource1"></asp:CheckBox>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div  id="MsgNoCopiaMagazzino" style="display:none">
                    <span class="MsgNoCopiaMagazzino">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ATTENZIONESelezionateImpreseDiverseScarichiMagazzinoNonVerrannoRegistrati %>" runat="server">ATTENZIONE! Selezionate imprese diverse, gli scarichi di magazzino non verranno registrati</asp:Localize>
                    </span>
                </div>
            </div>
        </div>


        <div class="row" style="margin-bottom: 10px; padding: 0 15px;">

            <ul id="panelbarDuplica">

                <li class="k-state-active k-active" id="panelBarOperazioni">
                    <span class="k-link k-state-selected k-selected">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OperazioniCheSarannoDuplicate %>" runat="server">Operazioni che saranno duplicate</asp:Localize>
                    </span>

                    <%--Operazioni Selezionate, sotto update panel condizionale per evitare la perdita della selezione--%>
                    <asp:UpdatePanel ID="UpdatePanelOperazioni" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="divKendoOperazioni"></div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </li>


                <li class="k-state-active k-active" id="panelBarDatePropagate" style="display: none">

                    <span class="k-link k-state-selected k-selected">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InterventoTemporaleAmmissibile %>" runat="server">Intervento Temporale Ammissibile</asp:Localize>
                    </span>

                    <div class="row">
                        <div class="col-lg-5 col-md-5 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_ValiditaInizio" for="<%=Txt_ValiditaInizio.clientid %>">
                                            <label style="margin-top: -5px; margin-right: 5px;">
                                                <asp:Label ID="lbldal" runat="server" meta:resourcekey="lbldalResource1">dal</asp:Label>
                                            </label>
                                        </span>
                                        <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="txtUI kendoDatePicker" meta:resourcekey="Txt_ValiditaInizioResource1"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-5 col-md-5 col-sm-12">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_ValiditaFine" for="<%=Txt_ValiditaFine.clientid %>">
                                            <label style="margin-top: -5px; margin-right: 5px;">
                                                <asp:Label ID="lblAl" runat="server" meta:resourcekey="lblAlResource1">al</asp:Label>
                                            </label>
                                        </span>
                                        <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="txtUI kendoDatePicker" meta:resourcekey="Txt_ValiditaFineResource1"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </li>

                <li class="k-state-active k-active" id="panelBarInterventi" style="display: none">

                    <span class="k-link k-state-selected k-selected">                        
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiepilogoInterventi %>" runat="server">Riepilogo Interventi</asp:Localize>
                    </span>

                    <asp:UpdatePanel ID="UpdatePanel_OperazioniSingole" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <!-- DataOperazione -->
                            <div class="row">
                                <div class="col-lg-5 col-md-5 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon lbl_required" id="lbl_DataOperazione" for="TxtDataOperazione">
                                                    <asp:Label ID="lblData" runat="server" meta:resourcekey="lblDataResource1">Data</asp:Label></span>
                                                <asp:TextBox ID="Txt_DataIntervento" runat="server" CssClass="txtUI kendoDatePicker" meta:resourcekey="Txt_DataInterventoResource1"></asp:TextBox>
                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <!-- Note -->
                                <div class="col-lg-5 col-md-5 col-sm-12">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <span class="input-group-addon lbl_required" id="lbl_Note" for="TxtNote">
                                                    <asp:Label ID="lblNote" runat="server" meta:resourcekey="lblNoteResource1">Note</asp:Label></span>
                                                <asp:TextBox ID="Txt_Note" runat="server" CssClass="txtUI" TextMode="MultiLine" Width="420px"
                                                    meta:resourcekey="Txt_NoteResource1"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-sm-12">
                                    <div class="btn btn-success" id="cImgBtn_InterventoInserisci">
                                        <span class="fa fa-save lampeggiante"></span>
                                        <span class="lampeggiante">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, InserisciNuovaOperazioneNellaLista %>" runat="server">Inserisci una Nuova Operazione Nella Lista</asp:Localize>
                                        </span>
                                    </div>
                                    <asp:ImageButton ID="ImgBtn_InterventoInserisci" runat="server" ImageUrl="../AB_Immagini/Icone16/FrecciaRossa_S.ico"
                                        meta:resourcekey="ImgBtn_InterventoInserisciResource1" CssClass="ImgBtn_InterventoInserisci" Style="display: none" />
                                </div>
                            </div>

                            <div class="row">
                                <asp:GridView ID="DataGridInterventi" runat="server" AutoGenerateColumns="False"
                                    Width="100%" CellPadding="5" CssClass="ui-widget-content" Caption="RIEPILOGO NUOVE OPERAZIONI"
                                    meta:resourcekey="DataGridInterventiResource1">
                                    <Columns>
                                        <asp:ButtonField ButtonType="Button" ItemStyle-CssClass="btnElimina" Text="Elimina" HeaderText="" CommandName="EliminaIntervento"
                                            meta:resourcekey="ButtonFieldResource1" />
                                        <asp:BoundField DataField="ID" HeaderText="ID">
                                            <ItemStyle CssClass="displaynone" />
                                            <HeaderStyle CssClass="displaynone" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Data" HeaderText="Data" HtmlEncode="false" meta:resourcekey="BoundFieldResource1"></asp:BoundField>
                                        <asp:BoundField DataField="Note" HeaderText="Note" HtmlEncode="false" meta:resourcekey="BoundFieldResource2"></asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="ui-widget-header" />
                                    <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                                    <RowStyle CssClass="rigaImpianti" />
                                </asp:GridView>
                            </div>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                </li>

                <li class="k-state-active k-active" id="panelBarImpianti">
                    <span class="k-link k-state-selected k-selected">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiepilogoImpianti %>" runat="server">Riepilogo Impianti</asp:Localize>          
                    </span>
                    <div id="divKendoImpianti"></div>

                    <div>
                    </div>
                </li>
            </ul>
        </div>
    </div>

</asp:Content>

<asp:Content ID="ContentScript" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript">


        function SelezionaDeselezionaTutti() {
            if ($('#chkSelezionaTuttiImpianti').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').prop('checked', true); //.attr('checked', 'checked');
                });
            }
            else {
                $('.ChkSelezionaImpianto').each(function () {
                    $(this).children('input').prop('checked', false); //.removeAttr('checked');
                });
            }
        }

        function DoPostBack_ControlliSiNo(key) {
            if (key == 'Salva') {
                $("#<%=HiddenVarie.ClientID %>").val("OK");
                $("#<%=Btn_SalvaTutto.ClientID %>").click();

            }
        }

        var panelbarDuplica;

        var KendoOperazioni;
        var KendoImpianti;

        var ids = $('#<%=hd_ids.ClientID %>').val();
        var id_hdOperazioniSelezionate = "#<%=hdOperazioniSelezionate.ClientID %>";
        var id_hdKendo_Impianti = "#<%=hdKendo_Impianti.ClientID %>";
        var id_hdImpiantiSelezionati = "#<%=hdImpiantiSelezionati.ClientID %>";
        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var currentPiva = $('#<%=hd_CurrentPiva.ClientID %>').val();

        var dtInterventi = $('#<%=hd_dtInterventi.ClientID %>').val();

        var isFertirrigazione = $('#<%=hd_isFertirrigazione.ClientID %>').val() == 'True' ? true : false;
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DuplicaOperazione.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DuplicaJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("DuplicaOperazione_ws_client.js") %>"></script>

    <script type="text/javascript">

        function EndRequestHandler() {
            event_bind();
        }

    </script>

</asp:Content>
