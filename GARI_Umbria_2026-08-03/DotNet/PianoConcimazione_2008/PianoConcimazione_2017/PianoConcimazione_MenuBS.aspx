<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/MasterConcimazione.Master"
    CodeBehind="PianoConcimazione_MenuBS.aspx.vb" Inherits="PianoConcimazione_2017.PianoConcimazione_MenuBS" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <style type="text/css">
        .btn-warning {
            color: #fff;
            background-color: #f0ad4e;
            border-color: #eea236;
        }

        .btnWidth {
            width: 170px;
        }

        .kendoRiga_AgendaOperazBloccata, .kendoRiga_AgendaOperazBloccata:hover {
            background-color: #d7d7c1;
        }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_CurrentPiva" runat="server" />
    <asp:HiddenField ID="hd_filtroRegolamentiPrivati" runat="server" />

    <div class="panel panel-primary gias-piano-concimazione-menu-header gias-mt-1 gias-ml-1 gias-mr-1">
        <div class="panel-heading">
            <h4 class="panel-title"><b></b></h4>
        </div>
        
        <div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Dal %>" runat="server">Dal</asp:Localize>                                        
                                    </span>
                                    <asp:TextBox ID="Txt_ValiditaInizio" runat="server" CssClass="form-control" onblur="controllaData(this)" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info">
                                        <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Al %>" runat="server">Al</asp:Localize>
                                    </span>
                                    <asp:TextBox ID="Txt_ValiditaFine" runat="server" CssClass="form-control" onblur="controllaData(this)" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                        <div class="btn btn-success btn_100 gias-btn-primary" onclick="Aggiorna();">
                            <i class="fa fa-refresh">
                            </i>
                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Aggiorna %>" runat="server">Aggiorna</asp:Localize>
                        </div>
                    </div>
                </div>
                
                <!-- PIANO CONCIMAZIONE -->
                <%If ((permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) AndAlso tipologiaPagina = enum_PUARegolamenti_Tipo.PianoComcimazione) Then%>
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 ">
                        <nav class="navbar navbar-default"  >
                            <div class="container-fluid">
                                <div class="div_menu_tool">
                                    <ul class="nav navbar-nav visible-lg visible-md visible-sm visible-xs" style="width:100%">
                                        <li class="dropdown">
                                            <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                <i class="fa fa-bars gias-font-size-14px" style="font-size: 30px;"></i>
                                                <span class="hidden-none margin-r" style="text-transform: uppercase">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Nuovo %>" runat="server">NUOVO</asp:Localize>
                                                </span> 
                                                <span class="caret"></span>
                                            </a>
                                            <ul class="dropdown-menu">
                                                <li onclick=" Nuovo(Enum_Metodo.Bilancio.value,true,true,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, BilancioSingolaAziendaSingolaColtura %>" runat="server">BILANCIO Singola Azienda Singola Coltura</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <li onclick=" Nuovo(Enum_Metodo.Bilancio.value,true,false,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, BilancioSingolaAziendaMultiColtura %>" runat="server">BILANCIO Singola Azienda Multi-Coltura</asp:Localize>                                                            
                                                        </span>
                                                    </a>
                                                </li>
                                                
                                                <%If (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazioneMassivo).Scrittura = True) Then%>
                                                <li onclick=" Nuovo(Enum_Metodo.Bilancio.value,false,false,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, BilancioMultiAziendeMultiColtura %>" runat="server">BILANCIO Multi-Aziende Multi-Coltura</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <% End If%>
                                                
                                                <li onclick=" Nuovo(Enum_Metodo.Schede.value,true,true,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SchedeSingolaAziendaSingolaColtura %>" runat="server">SCHEDE Singola Azienda Singola Coltura</asp:Localize>                                                            
                                                        </span> 
                                                    </a>
                                                </li>
                                                <li onclick=" Nuovo(Enum_Metodo.Schede.value,true,false,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SchedeSingolaAziendaMultiColtura %>" runat="server">SCHEDE Singola Azienda Multi-Coltura</asp:Localize>                                                            
                                                        </span> 
                                                    </a>
                                                </li>
                                                
                                                <%If (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazioneMassivo).Scrittura = True) Then%>
                                                <li onclick=" Nuovo(Enum_Metodo.Schede.value,false,false,1)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, SchedeMultiAziendeMultiColtura %>" runat="server">SCHEDE Multi-Aziende Multi-Coltura</asp:Localize>                                                            
                                                        </span> 
                                                    </a>
                                                </li>
                                                <% End If%>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>    
                
                <% End If%>
                
                <!-- PUA -->
                <%If (((permessi.getPermesso(enum_Security_Attivita.Gest_PUA).Scrittura = True) Or (permessi.getPermesso(enum_Security_Attivita.Gest_PUA_2).Scrittura = True)) AndAlso tipologiaPagina = enum_PUARegolamenti_Tipo.PUA) Then%>
                <div class="row">
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                        <div class="btn btn-info btn_100" onclick="Nuovo(1,false,false,2);">
                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NuovoPUA %>" runat="server">Nuovo PUA</asp:Localize>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                    </div>

                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_PUA).Scrittura = True) Then%>
                    <div class="col-lg-4 col-md-4 col-xs-12 ">
                        <div class="btn btn-warning btn_100" onclick="ImportaComunicazione();">
                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, ImportaComunicazione %>" runat="server">Importa Comunicazione</asp:Localize>
                        </div>
                    </div>
                    <% End If%>
                </div>
                <% End If%>     
                
                <!-- 11/10/21 Anna: Piano Nutrizionale -->
                <!-- PIANO NUTRIZIONALE --> 
                <%If ((permessi.getPermesso(enum_Security_Attivita.Piano_Nutrizionale).Scrittura = True) AndAlso (tipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale OrElse tipologiaPagina = enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF)) Then%> 
                

                 <!-- PER IL MOMENTO IL PIANO NUTRIZIONALE IBF HA UN SOLO METODO (BILANCIO), QUINDI HA BISOGNO DI UN SINGOLO PULSANTE, SENZA MENU A TENDINA --> 
                 <!-- IN FUTURO SCOMMENTARE IL PEZZO SOTTO, PER RIUNIRE LE LISTE --> 
                 <!-- in caso eliminare da qui -->                 
                <%If (mostraPrivati_xPianoNutrizionale = True) Then%> 
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 ">
                        <nav class="navbar navbar-default"  >
                            <div class="container-fluid">
                                <div class="div_menu_tool">
                                    <ul class="nav navbar-nav visible-lg visible-md visible-sm visible-xs" style="width:100%">
                                        <li class="dropdown">
                                            <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                <i class="fa fa-bars" style=" font-size: 30px;"></i>
                                                <span class="hidden-none margin-r" style="text-transform: uppercase">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NuovoPianoNutrizionale %>" runat="server">NUOVO PIANO NUTRIZIONALE</asp:Localize>
                                                </span> 
                                                <span class="caret"></span>
                                            </a>
                                            
                                            <ul class="dropdown-menu">
                                                <li onclick="Nuovo(Enum_Metodo.Bilancio.value,true,true,5);"">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoBilancioAnalisi %>" runat="server">Metodo BILANCIO (con analisi)</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <li onclick="Nuovo(Enum_Metodo.Bilancio.value,true,true,4);"">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoBilancioAnalisiBarbabietolaZucchero %>" runat="server">Metodo BILANCIO Barbabietola da Zucchero (con analisi)</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <li onclick=" Nuovo(Enum_Metodo.Schede.value,true,true,4)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoSemplificatoNoAnalisiBarbabietolaZucchero %>" runat="server">Metodo SEMPLIFICATO Barbabietola da Zucchero (senza analisi)</asp:Localize>
                                                        </span> 
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                 <% End If%>  

                 <%If (mostraPrivati_xPianoNutrizionale = False) Then%> 
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 ">
                        <nav class="navbar navbar-default"  >
                            <div class="container-fluid">
                                <div class="div_menu_tool">
                                    <ul class="nav navbar-nav visible-lg visible-md visible-sm visible-xs" style="width:100%">
                                        <li class="dropdown "  onclick="Nuovo(Enum_Metodo.Bilancio.value,true,true,5);"">
                                            <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                <span class="hidden-none margin-r" style="text-transform: uppercase">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NuovoPianoNutrizionale %>" runat="server">NUOVO PIANO NUTRIZIONALE</asp:Localize>
                                                </span> 
                                            </a>   
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                 <% End If%>  
                <!-- in caso eliminare fino a qui -->                 


                <!-- SCOMMENTARE IN CASO DI PIU' OPZIONI PER IL PIANO IBF, eliminando il pezzo indicato sopra --> 
                <!--
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-xs-12 ">
                        <nav class="navbar navbar-default"  >
                            <div class="container-fluid">
                                <div class="div_menu_tool">
                                    <ul class="nav navbar-nav visible-lg visible-md visible-sm visible-xs" style="width:100%">
                                        <li class="dropdown">
                                            <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                <i class="fa fa-bars" style=" font-size: 30px;"></i>
                                                <span class="hidden-none margin-r" style="text-transform: uppercase">
                                                    <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NuovoPianoNutrizionale %>" runat="server">NUOVO PIANO NUTRIZIONALE</asp:Localize>
                                                </span> 
                                                <span class="caret"></span>
                                            </a>
                                            
                                            <ul class="dropdown-menu">
                                                <li onclick="Nuovo(Enum_Metodo.Bilancio.value,true,true,5);"">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoBilancioAnalisi %>" runat="server">Metodo BILANCIO (con analisi)</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <%--<li onclick=" Nuovo(Enum_Metodo.Schede.value,true,true,5)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoSemplificatoNoAnalisi %>" runat="server">Metodo SEMPLIFICATO (senza analisi)</asp:Localize>
                                                        </span> 
                                                    </a>
                                                </li>--%>
                                                <%If (mostraPrivati_xPianoNutrizionale = True) Then%> 
                                                <li onclick="Nuovo(Enum_Metodo.Bilancio.value,true,true,4);"">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoBilancioAnalisiBarbabietolaZucchero %>" runat="server">Metodo BILANCIO Barbabietola da Zucchero (con analisi)</asp:Localize>
                                                        </span>
                                                    </a>
                                                </li>
                                                <li onclick=" Nuovo(Enum_Metodo.Schede.value,true,true,4)">
                                                    <a onclick="return 0">
                                                        <span>
                                                            <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, MetodoSemplificatoNoAnalisiBarbabietolaZucchero %>" runat="server">Metodo SEMPLIFICATO Barbabietola da Zucchero (senza analisi)</asp:Localize>
                                                        </span> 
                                                    </a>
                                                </li>
                                                <% End If%>  
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                -->    
                <% End If%>               

            </div>
        </div>
    </div>
    
    <div class="panel panel-primary gias-piano-concimazione-menu-grid gias-ml-1 gias-mr-1">
        <div class="panel-heading">
            <h4 class="panel-title">
                <b></b>
            </h4>
        </div>
        <div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-lg-12 nopadding"  style="overflow:scroll">
                        <div id="kendo_PianiConcimazione">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div id="window_PianiDistribuzione">          
        <div class="jumbotron" style="padding-left: 15px; padding-right: 15px;">
            <div class="row">
                <div class="col-lg-5 col-sm-5 col-md-5 col-xs-12"">
                    <nav class="navbar navbar-default"  >
                        <div class="container-fluid">
                            <div class="collapse navbar-collapse div_menu_tool">
                                <ul class="nav navbar-nav visible-lg visible-md visible-sm visible-xs"  style="width: 100%;">
                                    <!-- NUOVO -->
                                    <%If (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then%>
                                    <li onclick="NuovoPD()">
                                        <a onclick="return 0">
                                            <span>
                                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, NuovoPD %>" runat="server">Nuovo Piano Distribuzione</asp:Localize>
                                            </span>
                                        </a>
                                    </li>
                                    <% End If%>
                                </ul>
                            </div>
                        </div>
                    </nav>
                </div>
            
                <div class="col-lg-5 col-sm-5 col-md-5 col-xs-12" style="float: right;padding: 10px">
                    <div class="col-lg-4 col-sm-4 col-md-4 col-xs-12">
                        <div>
                            <h4><asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Dal %>" runat="server">Dal</asp:Localize>:</h4>
                            <input id="start_date_dist" onblur="controllaData(this)" maxlength="10" runat="server" /> 
                        </div>
                    </div>
                    <div class="col-lg-4 col-sm-4 col-md-4 col-xs-12">
                        <div>
                            <h4><asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Al %>" runat="server">Al</asp:Localize>:</h4>
                            <input id="end_date_dist" onblur="controllaData(this)" maxlength="10" runat="server" /> 
                        </div>
                    </div>
                    <div class="col-lg-4 col-sm-4 col-md-4 col-xs-12">
                        <div>
                            <div class="btn btn-success" onclick="AggiornaDist();">
                                <i class="fa fa-refresh"></i>   
                                <asp:Localize Text="<%$ Resources: PianoConcimazione_2017, Aggiorna %>" runat="server">Aggiorna</asp:Localize>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="row">
                <div class="col-12"  style="overflow: scroll">
                    <div id="kendo_PianiDistribuzione">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script id="templatePianoConcimazione" type="text/x-kendo-template">
        <div class="k-button" id="btnSbloccaPC" onClick="BloccaSbloccaPC(0);" title="Sblocca Piani Concimazione selezionati" style="display: none">
            <span class="fa fa-unlock"></span>
        </div>
        <div class="k-button" id="btnBloccaPC" onClick="BloccaSbloccaPC(1);" title="Blocca Piani Concimazione selezionati" style="display: none">
            <span class="fa fa-lock"></span>
        </div>
    </script>
    
    <script id="templatePUA" type="text/x-kendo-template">
        <div class="k-button" id="btnSbloccaPUA" onClick="BloccaSbloccaPUA(0);" title="Sblocca PUA selezionati" style="display: none">
            <span class="fa fa-unlock"></span>
        </div>
        <div class="k-button" id="btnBloccaPUA" onClick="BloccaSbloccaPUA(1);" title="Blocca PUA selezionati" style="display: none">
            <span class="fa fa-lock"></span>
        </div>
    </script>

    <script>
        var permessoBloccaPC =
        <% If (permessi.getPermesso(enum_Security_Attivita.PianoConcimazione_Blocca_Sblocca).Scrittura = True) Then %>
            true;
            <%else %>
        false;
            <%end if %>


        var permessoBloccaPUA =
            <% If (permessi.getPermesso(enum_Security_Attivita.PUA_Blocca_Sblocca).Scrittura = True) Then %>
            true;
            <%else %>
        false;
            <%end if %>

        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var currentPiva = $('#<%=hd_CurrentPiva.ClientID %>').val();

        var TipologiaPagina = <%= tipologiaPagina %>;
        var MostraPrivati = <%= mostraPrivati_xPianoNutrizionale %>;
        var filtroRegolamentiPrivati = $('#<%=hd_filtroRegolamentiPrivati.ClientID %>').val();

        var Txt_ValiditaInizio = "<%= Txt_ValiditaInizio.ClientID %>"
        var Txt_ValiditaFine = "<%= Txt_ValiditaFine.ClientID %>"

        var start_date_dist = "<%= start_date_dist.ClientID %>"
        var end_date_dist = "<%= end_date_dist.ClientID %>"

        var UtenteAbilitatoScritturaPC =
            <% If (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Scrittura = True) Then %>
            true;
            <%else %>
        false;
            <%end if %>


        var UtenteAbilitatoScritturaPUA =
            <% If (permessi.getPermesso(enum_Security_Attivita.Gest_PUA_2).Scrittura = True) Then %>
            true;
            <%else %>
        false;
            <%end if %>

        //11/10/214 Anna: Piano Nutrizionale
        var UtenteAbilitatoScritturaPianoNutrizionale =
            <% If (permessi.getPermesso(enum_Security_Attivita.Piano_Nutrizionale).Scrittura = True) Then %>
            true;
            <%else %>
        false;
            <%end if %>    

        function kendo_PianiConcimazione_LeggiAjax() {

            let param = JSON.stringify({
                data_inizio: $('#<%=Txt_ValiditaInizio.ClientID %>').val(),
                data_fine: $('#<%=Txt_ValiditaFine.ClientID %>').val(),
                TipologiaPagina: TipologiaPagina,
                filtroRegolamentiPrivati: filtroRegolamentiPrivati
            })

            ajaxAgronicaSync("PianoConcimazione_MenuBS.aspx/LeggiPianiConcimazione", param, false,
                function (risposta) {
                    if (risposta.RispostaOK) {
                        jSonParsed_Kendo_PianiConcimazione = JSON.parse(risposta.RispostaStringa);
                    }
                    else {
                        alert(risposta.Errore);
                    }
                }, null);
        }

        function leggixPratiche_PianiConcimazione_LeggiAjax() {
            return new Promise((resolve, reject) => {
                ajaxAgronica("PianoConcimazione_MenuBS.aspx/LeggixPratichePianiConcimazione",
                    JSON.stringify({ data_inizio: $('#<%=Txt_ValiditaInizio.ClientID %>').val(), data_fine: $('#<%=Txt_ValiditaFine.ClientID %>').val(), TipologiaPagina: TipologiaPagina }),
                    function (risposta) {
                        resolve(risposta);
                    }, null);
            });
        }

        var start;
        var end;
        function FiltroDate() {

            // create DatePicker from input HTML element
            function startChange() {
                var startDate = start.value(),
                    endDate = end.value();

                //if (startDate) {
                //    startDate = new Date(startDate);
                //    startDate.setDate(startDate.getDate());
                //    end.min(startDate);
                //} else if (endDate) {
                //    start.max(new Date(endDate));
                //} else {
                //    endDate = new Date();
                //    start.max(endDate);
                //    end.min(endDate);
                //}
            }

            function endChange() {
                var endDate = end.value(),
                    startDate = start.value();

                //if (endDate) {
                //    endDate = new Date(endDate);
                //    endDate.setDate(endDate.getDate());
                //    start.max(endDate);
                //} else if (startDate) {
                //    end.min(new Date(startDate));
                //} else {
                //    endDate = new Date();
                //    start.max(endDate);
                //    end.min(endDate);
                //}
            }

            start = $('#<%=Txt_ValiditaInizio.ClientID %>').kendoDatePicker({
                change: startChange
            }).data("kendoDatePicker");

            end = $('#<%=Txt_ValiditaFine.ClientID %>').kendoDatePicker({
                change: endChange
            }).data("kendoDatePicker");

            ripristinaPreferenzeFiltriDateDaCookie(start, end);

            //start.max(end.value());
            //end.min(start.value());
        }

        function FiltroDatePD() {

            // create DatePicker from input HTML element
            function startChange() {
                var startDate = start.value(),
                    endDate = end.value();

                //if (startDate) {
                //    startDate = new Date(startDate);
                //    startDate.setDate(startDate.getDate());
                //    end.min(startDate);
                //} else if (endDate) {
                //    start.max(new Date(endDate));
                //} else {
                //    endDate = new Date();
                //    start.max(endDate);
                //    end.min(endDate);
                //}
            }

            function endChange() {
                var endDate = end.value(),
                    startDate = start.value();

                //if (endDate) {
                //    endDate = new Date(endDate);
                //    endDate.setDate(endDate.getDate());
                //    start.max(endDate);
                //} else if (startDate) {
                //    end.min(new Date(startDate));
                //} else {
                //    endDate = new Date();
                //    start.max(endDate);
                //    end.min(endDate);
                //}
            }

            var start = $('#<%=start_date_dist.ClientID %>').kendoDatePicker({
                change: startChange
            }).data("kendoDatePicker");

            var end = $('#<%=end_date_dist.ClientID %>').kendoDatePicker({
                change: endChange
            }).data("kendoDatePicker");

            //start.max(end.value());
            //end.min(start.value());
        }
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoConcimazione_MenuBS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoConcimazione_MenuBS_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoConcimazione_MenuBS_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("PianoConcimazione_MenuBS_globali.js") %>"></script>

</asp:Content>
