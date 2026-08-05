<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Campo_Edit.aspx.vb" Inherits="AgroAgenda_2010.Campo_Edit" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div data-toggle="validator" role="form" class="row" style="margin-bottom: 100px;">


        <div class="col-lg-12 col-md-12">
            <div class="row">
                <div class="col-lg-10 col-md-10" style="padding: 10px 15px; line-height: 1.6;">
                    <%--                 Riferimenti:
                    <b><asp:Label ID="LblRiferimenti" runat="server">
                        </asp:Label></b>--%>
                    <div class="col-lg-4 col-md-4 col-sm-12">
                        <div>
                            <label>
                                <asp:RadioButton ID="Opt_Senza_Catasto" runat="server" GroupName="TipoGestione" ClientIDMode="Static"></asp:RadioButton>
                                <asp:Localize meta:resourcekey="CampoSenzaGestioneCatastale" runat="server">Campo <b>senza</b> gestione catastale</asp:Localize>
                            </label>
                        </div>
                        <div>
                            <label>
                                <asp:RadioButton ID="Opt_Con_Catasto" runat="server" GroupName="TipoGestione" ClientIDMode="Static"></asp:RadioButton>
                                <asp:Localize meta:resourcekey="CampoConGestioneCatastale" runat="server">Campo <b>con</b> gestione catastale</asp:Localize>
                            </label>
                        </div>
                    </div>

                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div>
                            <asp:Localize meta:resourcekey="SuperficieAgricolaUtilizzata" runat="server">Superficie Agricola Utilizzata</asp:Localize>:
                            <b>
                                <asp:Label ID="lblSupUtil" runat="server" ClientIDMode="Static">
                                </asp:Label></b> [ha]
                        </div>
                        <div>
                            <asp:Localize meta:resourcekey="RapportoSAUSuperficieCatastale" runat="server">Rapporto SAU/Sup. Catastale</asp:Localize>:
                            <b>
                                <asp:Label ID="lblRapporto" runat="server" ClientIDMode="Static">
                                </asp:Label></b> [%]
                        </div>
                        <div class="div_SupCatasto">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SuperficieCatastale %>" runat="server">Superficie Catastale</asp:Localize>:
                            <b>
                                <asp:Label ID="lblSupCatasto" runat="server" ClientIDMode="Static">
                                </asp:Label></b> [ha]
                        </div>
                    </div>
                    <div class="col-lg-2 col-md-2 col-sm-12">
                        <asp:CheckBox ID="chk_serra" runat="server" AutoPostBack="false" ClientIDMode="Static" />
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Serra %>" runat="server">Serra</asp:Localize>
                    </div>

                </div>
                <div class="col-lg-2 col-md-2 text-right">
                    <%--<div class="btn btn-success" onclick="$('#<%=ImgBtn_SalvaTutto.ClientID %>').click();">--%>

                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura = True) And (Operazione <> enum_TipoOperazioneDB.Lettura) Then%>

                    <div class="btn btn-success xi-btn-primary" onclick="ValidaxSubmit();" style="margin-bottom: 20px;">
                        <i class="fa fa-floppy-o"></i><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Salva %>" runat="server">Salva</asp:Localize>
                    </div>
                    <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                    <button type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown"
                        aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important;">
                        <span class="caret"></span><span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" style="right: 0; left: auto !important;">
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(0); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEdEsci %>" runat="server">Salva ed Esci</asp:Localize>
                        </a></li>
                        <li><a href="#" onclick="$('#<%=tipo_salva.ClientID %>').val(1); ValidaxSubmit();">
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SalvaEContinua %>" runat="server">Salva e Continua</asp:Localize>
                        </a></li>
                    </ul>
                    <% End If%>
                    <asp:HiddenField ID="tipo_salva" runat="server" ClientIDMode="Static" />
                    <asp:ImageButton ID="ImgBtn_SalvaTutto" runat="server" ImageUrl="~/AB_Immagini/Icone32/Dischetto.ico"
                        Style="display: none" ClientIDMode="Static" />

                    <% End If%>
                </div>
            </div>


        </div>

        <div class="col-lg-12" id="tabs" style="margin-bottom: 100px;">
            <ul id="navigator" class="nav nav-tabs" data-tabs="tabs">
                <li class="active tab_dati_riferimento"><a href="#tab_dati_riferimento" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiRiferimento %>" runat="server">Dati Riferimento</asp:Localize>
                </a></li>
                <li class="tab_superficie"><a href="#tab_superficie" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneSuperficie %>" runat="server">Gestione Superficie</asp:Localize>
                </a></li>
                <li class="tab_catasto"><a href="#tab_catasto" data-toggle="tab">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiCatastali %>" runat="server">Dati Catastali</asp:Localize>
                </a></li>
            </ul>

            <div id="my-tab-content" class="tab-content">
                <!-- TAB 1 -->
                <div class="tab-pane active" id="tab_dati_riferimento">

                    <div class="jumbotron">
                        <div class="row">
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <i class="fa fa-info-circle"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DateVuoteIndicanoValiditàNonLimitata %>" runat="server">Le date possono essere lasciate vuote per indicare validità non limitata.</asp:Localize><br />
                                <i class="fa fa-info-circle"></i><asp:Localize meta:resourcekey="SeValorizzateVengonoFiltratiGliAppezzamenti" runat="server">Se valorizzate, gli appezzamenti verranno filtrati di conseguenza.</asp:Localize>
                                <div>
                                    <i class="fa fa-exclamation-triangle"></i>
                                    <small>
                                        <b><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CENTROValidità %>" runat="server">CENTRO validità:</asp:Localize></b>
                                        <asp:Label ID="lbl_centro_data_inizio" runat="server" CssClass="txtUI" BorderStyle="None" ClientIDMode="Static"></asp:Label>
                                        - 
                                        <asp:Label ID="lbl_centro_data_fine" runat="server" CssClass="txtUI" BorderStyle="None" ClientIDMode="Static"></asp:Label>
                                    </small>
                                </div>
                                <br />
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_ValiditaInizio" for="TxtValiditaInizio" clientidmode="Static">
                                                <i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàDal %>" runat="server">Validità Dal</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtValiditaInizio" ClientIDMode="Static" runat="server" CssClass="form-control" aria-describedby="lbl_ValiditaInizio"> </asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_ValiditaFine" for="TxtValiditaFine">
                                                <i class="fa fa-calendar"></i>
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ValiditàAl %>" runat="server">Validità Al</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtValiditaFine" ClientIDMode="Static" runat="server" CssClass="form-control" aria-describedby="lbl_ValiditaFine"> </asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <i class="fa fa-info-circle"></i>
                                <asp:Localize meta:resourcekey="SeDenominazioneVieneLasciatoVuotoVerràApplicatoUnNomeCasuale" runat="server">
                                    Se Denominazione viene lasciato vuoto, verrà applicato un nome casuale..
                                </asp:Localize><br />
                                <br />
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon lbl_required" id="lbl_Denominazione" for="TxtDenominazione">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Denominazione %>" runat="server">Denominazione</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtDenominazione" ClientIDMode="Static" runat="server" CssClass="form-control" aria-describedby="lbl_Denominazione"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-12 col-md-12 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="lbl_Codice" for="TxtCodice">
                                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Codice %>" runat="server">Codice</asp:Localize>
                                            </span>
                                            <asp:TextBox ID="TxtCodice" ClientIDMode="Static" runat="server" CssClass="form-control" aria-describedby="lbl_Codice"> </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;">
                                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DatiColtura %>" runat="server">Dati Coltura</asp:Localize>
                                        </h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12 col-md-12 col-sm-12">
                                        <i class="fa fa-info-circle"></i><asp:Localize meta:resourcekey="InformazioniFacoltativePerSpecializzareIlCampo" runat="server">
                                            Queste informazioni facoltative consentono di assegnare una "specializzazione" al campo.
                                        </asp:Localize><br />
                                        <br />
                                    </div>

                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon control-label " id="lbl_OrientamentoColturale" for="Cmb_OrientamentoColturale">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrientamentoColturale %>" runat="server">Orientamento Colturale</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_OrientamentoColturale" ClientIDMode="Static" runat="server" CssClass="form-control selectpicker"
                                                        data-live-search="true" aria-describedby="lbl_OrientamentoColturale">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon control-label " id="lbl_SpecieVegetale" for="Cmb_SpecieVegetale">
                                                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SpecieVegetale %>" runat="server">Specie Vegetale</asp:Localize>
                                                    </span>
                                                    <asp:DropDownList ID="Cmb_SpecieVegetale" ClientIDMode="Static" runat="server" CssClass="form-control selectpicker"
                                                        data-live-search="true" aria-describedby="lbl_SpecieVegetale">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12 border_si">
                                <div class="row">
                                    <div class="col-md-12 text-center">
                                        <h4 style="color: #052747; text-transform: uppercase;"><asp:Localize Text="<%$ Resources:CodiciCampo %>" runat="server">Codici Campo</asp:Localize></h4>
                                    </div>
                                </div>
                                <div class="row">
                                    <input type="hidden" id="kendoCodiciCampo" />
                                    <div class="col-lg-12">
                                        <div id="tabCodici">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <%If (Operazione = enum_TipoOperazioneDB.Scrittura) Then%>
                        <div class="row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div class="col-lg-12 text-center">
                                        <div class="btn btn-success dropdown-toggle" data-toggle="dropdown" onclick="creaAppezzamento()"
                                            aria-haspopup="true" aria-expanded="false" style="margin-top: -20px !important; text-transform: uppercase;" id="btnCreaAppezzamento">
                                            <asp:Localize meta:resourcekey="CreaAppezzamento" runat="server">Crea Appezzamento</asp:Localize>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <% End If %>
                        <!-- DA VERIFICARE: Da gestire quando e se verrà aggiunta la gestione catastale (compresa di i18n) -->
                        <div class="row" style="display: none;">
                            <div class="col-lg-12 text-center">
                                <h4 style="color: #052747; text-transform: uppercase;">Gestione Catastale</h4>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label " id="lbl_Superficie" for="Txt_Superficie">Sup. Catastale [ha]</span>
                                            <asp:TextBox ID="Txt_Superficie" ClientIDMode="Static" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label " id="lbl_SuperficieSAU" for="Txt_SuperficieSAU">Sup. Agricola Utilizzata [ha]</span>
                                            <asp:TextBox ID="Txt_SuperficieSAU" ClientIDMode="Static" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label " id="lbl_SuperficiePercentuale" for="Txt_SuperficiePercentuale">Rap. SAU/Sup. Catastale [%]</span>
                                            <asp:TextBox ID="Txt_SuperficiePercentuale" ClientIDMode="Static" runat="server" CssClass="form-control txtUI"></asp:TextBox>
                                            <asp:TextBox ID="Txt_SuperficieCatastale" ClientIDMode="Static" runat="server" CssClass="form-control txtUI" Style="display: none"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12" id="sup_serra">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label " id="lbl_SuperficieCoperta" for="Txt_SuperficieCoperta">Superficie Coperta [ha]</span>
                                            <asp:TextBox ID="Txt_SuperficieCoperta" ClientIDMode="Static" runat="server" CssClass="form-control txtUI"></asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>


                    </div>
                </div>

                <!-- TAB 2 -->
                <div class="tab-pane" id="tab_superficie">
                    <div class="jumbotron">
                        <div class="row" style="display: none">
                            <div class="col-lg-6 col-md-6 col-sm-12">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon control-label " id="lbl_DataVariazioneAppAggr" for="TxtDataVariazioneAppAggr"><i class="fa fa-calendar"></i>Data di Variazione</span>
                                            <asp:TextBox ID="TxtDataVariazioneAppAggr" ClientIDMode="Static" runat="server" CssClass="form-control txtUI datepicker"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <%--   
                        <%If visible_Campo_Cod = False Then%>--%>
                        <div id="tabGestCat"></div>
                        <%--    <%End If%>--%>
                    </div>
                </div>

                <!-- TAB 3 -->
                <div class="tab-pane" id="tab_catasto">
                    <div class="jumbotron">
                        <div id="tabParticelle"></div>
                        <div id="kendotabParticelle"></div>
                    </div>
                </div>


            </div>

        </div>





    </div>



    <!-- VARIE -->
    <!-- contiene l'indice del tab selezionato, viene controllato lato server CalledFromClient_SetIndexTab -->
    <input type="hidden" id="clickedTabUI" runat="server" />
    <!-- salva la chiamata alla funzione per il postback -->
    <input type="hidden" id="fooName_PostedBack" runat="server" />
    <!-- parametri le funzione di postback -->
    <input type="hidden" id="fooName_Param_PostedBack" runat="server" />
    <br />
    <input id="InizioCentro" name="InizioCentro" type="hidden" runat="server" clientidmode="Static" />
    <input id="FineCentro" name="FineCentro" type="hidden" runat="server" clientidmode="Static" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script src="../Scripts/footable.min.js?<% =Application("GiasVersioneCorrente")%>" type="text/javascript"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Campo_Edit.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Campo_Edit_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Campo_Edit_jQueryDocReady.js") %>" ></script>

    <script id="popupCodici_Template" type="text/x-kendo-template">
        <div class="k-edit-label">
            <label for="Codice" class="campiObbligatori"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Codice %>" runat="server">Codice</asp:Localize></label>
        </div>
        <div data-container-for="Codice" class="k-edit-field">
            <div required="required" data-bind="value:Id_Cod" id="Cmb_Codici"></div>
        </div>

        <div class="k-edit-label">
            <label for="valore"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,Valore %>" runat="server">Valore</asp:Localize></label>
        </div>
        <div data-container-for="valore" class="k-edit-field">
            <input required="required" type="text" data-type="text" class="k-input k-textbox" name="valore" data-bind="value:Val_Cod" />
        </div>
    </script>

    <script type="text/javascript">

        <% If jsGestCat IsNot Nothing %>
        var jsGestCat = '<%=jsGestCat.ToString.Replace("'", "\'") %>';
        <% else %>
        var jsGestCat = "";
        <% end If %>


        <% If jsParticelle IsNot Nothing %>
        var jsParticelle = '<%=jsParticelle.ToString.Replace("'", "\'")%>';
        <%  Else %>
        var jsParticelle = "";
         <% end If %>


        <% If jsCodici IsNot Nothing %>
        var jsCodici = <%=jsCodici.ToString.Replace("'", "\'")%>;
        <%  Else %>
        var jsCodici = "";
         <% end If %>


        var obj_Codici = <%= cmb_Codici.ToString %>;
</script>

    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
