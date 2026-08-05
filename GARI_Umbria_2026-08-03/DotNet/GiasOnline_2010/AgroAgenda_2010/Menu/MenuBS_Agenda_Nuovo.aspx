<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master"
    CodeBehind="MenuBS_Agenda_Nuovo.aspx.vb" Inherits="AgroAgenda_2010.MenuBS_Agenda_Nuovo" meta:resourcekey="PageResource1" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="MenuBS_Agenda_Nuovo.css<%= "?" & Application("GiasVersioneCorrente").ToString %>" rel="stylesheet"/>

    <asp:PlaceHolder ID="Meteo_headerPlaceHeader" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneNuovoAllegato" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoGestioneVisualizaAllegato" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoCostiRicaviDaQdCeCdG" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoFlagMostraBtnSalvaCDG" />
    <asp:HiddenField runat="server" ID="hf_esistonoCostiCollegatiCDG" />
    <asp:HiddenField runat="server" ID="hf_LinkPaginaOrigine" />
    <asp:HiddenField runat="server" ID="hf_LavCodRaccolta" />
    <!-- nuovi div-->
    <div class="row" style="margin-bottom:80px;">
        <div class="col-lg-12">

           

            <div class="row">

                <!-- preferiti e nuove operazioni (parte di sinistra) -->
                <div class="col-lg-2">

                    <div class="panelbarPreferiti k-content">
                        <ul id="panelbarPreferitiUl">
                            <li id="preferitiList" class="k-state-active k-active">
                                <%--parte di panel bar con i preferiti--%>
                                <!-- Preferiti -->

                                <span id="panelbarPreferitiTitle"><asp:Localize meta:resourcekey="jsLblListaOperazioni" runat="server">Lista Operazioni</asp:Localize></span> 

                                <div id="div_preferiti" class="panel panel-primary">

                                    <div id="pnlSxOperazioni">
                                        <div class="panel-heading">
                                            <h4><i class="fa fa-star"></i><asp:Localize meta:resourcekey="jsLblOperazioniPreferite" runat="server">Operazioni Preferite</asp:Localize></h4>
                                        </div>

                                        <%If (permessi.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni).Scrittura = True) Then%>
                                        <div id="pnlGestionePreferiti">
                                            <div class="k-button" id="btnApriToolbarPreferiti" onclick="apriToolbarPreferiti_EstraiIDControlloLSB();">
                                                <%--<i class="fa fa-pencil-square-o"><asp:Localize meta:resourcekey="title_btnApriToolbarPreferiti" runat="server">Modifica Preferiti</asp:Localize></i> --%>
                                                <span class="fa fa-pencil-square-o"></span><span><asp:Localize meta:resourcekey="title_btnApriToolbarPreferiti" runat="server">Modifica Preferiti</asp:Localize></span> 
                                            </div>
                                            <div class="btn btn-success" id="btnSalvaPreferiti" onclick="salvaPreferiti_EstraiIDControlloLSB();" style="display:none">
                                                <%--<i class="fa fa-save"><asp:Localize meta:resourcekey="title_btnSalvaPreferiti" runat="server">Salva Preferiti</asp:Localize></i>--%>
                                                <span class="fa fa-save"></span><span><asp:Localize meta:resourcekey="title_btnSalvaPreferiti" runat="server">Salva Preferiti</asp:Localize></span>
                                            </div>
                                        </div>
                                        <%End If%>

                                        <div id="pnlPreferiti">
                                            <select id="lsbPreferiti"></select>  
                                        </div>
                                        <div id="pnlPreferitiZoo">
                                            <select id="lsbPreferitiZoo"></select>  
                                        </div>

                                        <div class="panel-heading">
                                            <h4><i class="fa fa-slack"></i><asp:Localize meta:resourcekey="heading_altreOperazioni" runat="server">Altre Operazioni</asp:Localize></h4>
                                        </div>
                                        <div class="panel-body" style="padding: 12px;">
                                            <div id="pnlddlOperazioni">
                                                <input id="ddlOperazioni" class="ddlOperazioni" name="ddlOperazioni" style="width:100%"/>
                                            </div>
                                            <div id="pnlddlOperazioniZoo">
                                                <input id="ddlOperazioniZoo" class="ddlOperazioniZoo" name="ddlOperazioniZoo" style="width:100%"/>
                                            </div>
                                            <input type="button" id="btnNuovaOperazione" value="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Nuovo %>' runat='server'></asp:Localize>" onclick="btnNuovaOperazione_click();" class="btn btn-default" style="width: -webkit-fill-available" />
                                        </div>
                                    </div>
                       

                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Ricette).Scrittura = True) Then%>
                                    <div id="pnlSxRicette">
                                        <div class="panel-heading">
                                            <h4><i class="fa fa-star"></i><asp:Localize meta:resourcekey="heading_preferitiRicette" runat="server">Operazioni Preferite (Ricette)</asp:Localize></h4>
                                        </div>

                                        <div id="pnlPreferitiRicette">
                                            <select id="lsbPreferitiRicette"></select>  
                                        </div>
                                        <div class="panel-heading">
                                            <h4><i class="fa fa-slack"></i><asp:Localize meta:resourcekey="heading_operazioniRicette" runat="server">Altre Operazioni (Ricette)</asp:Localize></h4>
                                        </div>
                                        <div class="panel-body" style="padding: 12px;">
                                            <input id="ddlOperazioniRicette" class="ddlOperazioniRicette" name="ddlOperazioniRicette" style="width:100%"/>
                                            <input type="button" id="btnNuovaOperazioneRicetta" value="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Nuovo %>' runat='server'></asp:Localize>" onclick="btnNuovaOperazioneRicetta_click();" class="btn btn-default" style="width: -webkit-fill-available"/>
                                        </div>
                                    </div>
                                    <%End If%>

                                    <%If (permessi.getPermesso(enum_Security_Attivita.Brogliaccio).Scrittura = True) Then%>
                                    <div id="pnlSxBrogliaccio">
                                        <div class="panel-heading">
                                            <h4><i class="fa fa-star"></i><asp:Localize meta:resourcekey="heading_preferitiBrogliaccio" runat="server">Operazioni Preferite (Brogliaccio)</asp:Localize></h4> 
                                        </div>

                                        <div id="pnlPreferitiBrogliaccio">
                                            <select id="lsbPreferitiBrogliaccio"></select>  
                                        </div>
                                        <div class="panel-heading">
                                            <h4><i class="fa fa-slack"></i><asp:Localize meta:resourcekey="heading_operazioniBrogliaccio" runat="server">Altre Operazioni (Brogliaccio)</asp:Localize></h4>
                                        </div>
                                        <div class="panel-body" style="padding: 12px;">
                                            <input id="ddlOperazioniBrogliaccio" class="ddlOperazioniBrogliaccio" name="ddlOperazioniBrogliaccio" style="width:100%"/>
                                            <input type="button" id="btnNuovaOperazioneBrogliaccio" value="<asp:Localize Text='<%$ Resources: AgronicaAgenda_2010, Nuovo %>' runat='server'></asp:Localize>" onclick="btnNuovaOperazioneBrogliaccio_click();" class="btn btn-default" style="width: -webkit-fill-available" />
                                        </div>
                                    </div>
                                    <%End If%>

                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Ricette).Scrittura = True) Then%>
                                    <!--<div class="panel-heading">
                                        <h4><i class="fa fa-calendar-o"></i>Ordini di lavoro / Ricette</h4>
                                    </div>
                                    <div class="panel-body" style="padding: 12px;">
                                        <input id="ddlRicette" class="ddlRicette" name="ddlRicette" style="width:100%"/>
                                        <input type="button" id="btnNuovaRicetta" value="NUOVO" onclick="Gestione_Operazione_Menu('6e');" class="btn btn-default w100"/>
                                    </div>-->

                                    <%End If%>

                                </div>

                            </li>
                            <li class="">
                                <%--parte di panel bar con i bottoni--%>
                            </li>
                       </ul>
                    </div>

                </div>


                <%--parte grande dello schermo--%>
                <div class="col-lg-10">
                    
                    <!-- bottoni -->
                    <div id="bottoniera" class="row">
                       <div class="col-lg-12">
                            <nav class="navbar navbar-default">
                                <div class="container-fluid">
                                    <div id="div_menu_tool">
                                        <ul class="nav navbar-nav" style="width:100%;">
                
                                            
                                            <!-- BOTTONE Profitosan -->
                                            <li onclick="Gestione_Operazione_Menu('13');">
                                                <a onclick="return 0">
                                                    <img alt="" src="../AB_Immagini/icone24/profitosan.png" />
                                                    <asp:Label ID="lblBancheDati" runat="server" meta:resourcekey="lblBancheDatiResource1" style="vertical-align:bottom;"></asp:Label>
                                               </a>
                                            </li>  

                                            <!-- BOTTONE WidgetMeteo -->
                                            <%--<li onclick="">                                         
                                                <a onclick="return 0" style="max-height: 38px !important;">
                                                    <cc1:AgroMeteo ID="agrometeo" runat="server" />
                                                 </a>
                                            </li>--%>
                                            <% If Not Master.flag_MenuBS_2017 Then %>
                                            
                                            <%If (permessi.getPermesso(enum_Security_Attivita.Rilevamento_Smart_Gis).Scrittura = True) Then%>
                                            <!-- BOTTONE Rilevamento Appezzamenti Da Smartphone -->
                                            <li onclick="Gestione_Operazione_Menu('35');">
                                                <a onclick="return 0">
                                                    <i class="fa fa-globe" style="font-size:23px;"></i>
                                                    <span><asp:Localize meta:resourcekey="text_mappaturaSmart" runat="server">Mappatura Smart</asp:Localize></span>
                                                </a>
                                            </li>
                                            <%End If%>

                                            
                                            <% End If %>

                                            <!-- BOTTONE Verifica Conformita -->
                                            <li onclick="Gestione_Operazione_Menu('10');">
                                                <a onclick="return 0">
                                                    <i class="fa fa-check-square-o" style="font-size:23px;"></i>
                                                    <span><asp:Localize meta:resourcekey="text_verificaConformita" runat="server">VERIFICA CONFORMITA'</asp:Localize></span>
                                                </a>
                                            </li>


                                            <!-- MENU UTILITY -->
                                            <li class="dropdown" id="menuUtility" style="display:none">
                                            <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                <i class="fa fa-cog" style="font-size: 23px;"></i>
                                                <span class="hidden-none margin-r">UTILITY</span>
                                                <span class="caret"></span>
                                            </a>
                                                <ul class="dropdown-menu">

                                                <%If (permessi.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_MultiModificaInterventi).Scrittura = True) Then%>
                                                  <li onclick="Gestione_Operazione_Menu('5d');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-pencil-square-o" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_ModificaSelezionate" runat="server">Modifica Le Operazioni selezionate</asp:Localize></span>
                                                        </a>
                                                    </li> 

                                                    <!--<li onclick="Gestione_Operazione_Menu('5c');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-filter" style="font-size:23px; margin: 0;"></i>
                                                            <span>Filtra e Aggiungi Costi Accessori a più operazioni contemporaneamente</span>
                                                        </a>
                                                    </li>-->
                                                <%End If%>

                                                <!-- BLOCCA OPERAZIONI SELEZIONATE -->
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>

                                                    <li onclick="bloccaOperazioni();">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-lock" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_BloccaSelezionate" runat="server">Blocca Operazioni selezionate</asp:Localize></span>
                                                        </a>
                                                    </li>
                                                <%End If%>

                                                <!-- SBLOCCA OPERAZIONI SELEZIONATE -->
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Agenda_Operazioni_Sblocco).Scrittura = True) Then%>
                                                    <li onclick="sbloccaOperazioni();">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-unlock" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_SbloccaSelezionate" runat="server">Sblocca Operazioni Selezionate</asp:Localize></span>
                                                        </a>
                                                    </li>
                                                <%End If%>

                                                <!-- MENU RICETTE -->
                                                <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Ricette).Scrittura = True) Then%>

                                                    <li onclick="Gestione_Operazione_Menu('6b');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-exclamation" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_RicettaDaSelezionate" runat="server">Crea Ricetta da Operazioni selezionate</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                    <li onclick="Gestione_Operazione_Menu('6d');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-exclamation" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_PianificaAttivita" runat="server">Pianifica Attività</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                    <li onclick="Gestione_Operazione_Menu('6c');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-exclamation" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_MenuRicette" runat="server">Menu Ricette</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                <%End If%>

                                                <!-- IMPORTAZIONE CIO -->
                                                <%If (permessi.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_Importazione_Anagrafiche_CIO).Scrittura = True) Then%>

                                                    <li onclick="Gestione_Operazione_Menu('47');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-arrow-down" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_ImportaAnagrafiche" runat="server">Importa Anagrafiche</asp:Localize></span>
                                                        </a>
                                                    </li>
                                                <%End If%>
                                                    
                                                <%If SincroMenuVerificaPermessiAccesso() Then %>
                                                    <li onclick="Gestione_Operazione_Menu('52');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-arrows-h" style="font-size:23px; margin: 0;"></i>
                                                            <span><asp:Localize meta:resourcekey="text_MenuImportExportSincro" runat="server">Menù Esportazioni, Importazioni e Sincronizzazioni</asp:Localize></span>
                                                        </a>
                                                    </li>
                                                 <% end if %>

                                                </ul>
                                            </li>

                                            <!-- STAMPE -->
                                            <!-- NB: vengono caricate le stampe preferite tramite webservice!! -->
                                            <!-- Sotto sono quelle di default nel caso non siano presenti le stampe preferite -->
                                            <li id="li_stampe" class="dropdown">
                                                <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                    <i class="fa fa-print" style="font-size: 23px;"></i>
                                                    <span class="hidden-none margin-r"><asp:Localize meta:resourcekey="text_MenuStampa" runat="server">STAMPA</asp:Localize></span>
                                                    <span class="caret"></span>
                                                </a>
                                                <ul id="menu_stampe" class="dropdown-menu">
                                                    <li onclick="Gestione_Operazione_Menu('Stampa-150');">
                                                        <a onclick="return 0">                               
                                                            <span><asp:Localize meta:resourcekey="text_SchedaCampagna" runat="server">Scheda Campagna</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                    <li onclick="Gestione_Operazione_Menu('Stampa-152');">
                                                        <a onclick="return 0">
                                                            <span ><asp:Localize meta:resourcekey="text_GlobalGap" runat="server">Global Gap</asp:Localize></span>
                                                        </a>
                                                    </li>
                        
                                                    <li onclick="Gestione_Operazione_Menu('Stampa-14');">
                                                        <a onclick="return 0">
                                                            <span><asp:Localize meta:resourcekey="text_RegistroTrattamenti" runat="server">Registro Trattamenti</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                    <li onclick="Gestione_Operazione_Menu('Stampa-115');">
                                                        <a onclick="return 0">
                                                            <span><asp:Localize meta:resourcekey="text_RegistroFertilizzazioni" runat="server">Registro Fertilizzazioni</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                    <li onclick="Gestione_Operazione_Menu('7z');">
                                                        <a onclick="return 0">
                                                            <span><asp:Localize meta:resourcekey="text_MenuStampe" runat="server">Menu Stampe</asp:Localize></span>
                                                        </a>
                                                    </li>

                                                </ul>
                                            </li>

                                            <!-- MENU LINK -->
                                            <li class="dropdown<%= IIf(Master.flag_MenuBS_2017, " hidden", "") %>">
                                                <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                                                    <i class="fa fa-bars" style="  font-size: 23px;"></i>
                                                    <span class="hidden-none margin-r">LINK</span>
                                                    <span class="caret"></span>
                                                </a>

                                                <ul class="dropdown-menu">

                                                    <!-- Anagrafica -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_AnagraficaAzienda).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('16');">
                                                        <a onclick="return 0">
                                                            <img alt="Anagrafica" src="../AB_Immagini/icone32/Impresa.ico" />
                                                            <span class="hidden-none margin-r"><asp:Label runat="server" ID="lblAnagrafica" meta:resourcekey="lblAnagraficaResource1">Anagrafica</asp:Label></span>
                                                        </a>
                                                    </li>
                                                    <%End If%>

                                                    <!-- Profilazione Guidata azienda pc_anteprima -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Pianificazione_Produzione_Vegetale).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('36');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-leaf" style="margin:0px; font-size:23px;"></i>                                                        
                                                            <span class="hidden-none margin-r"><asp:Label runat="server" ID="lblProfilazioneGuidata" meta:resourcekey="lblProfilazioneGuidataResource1">Piano Colturale</asp:Label></span>
                                                        </a>
                                                    </li>
                                                    <%End If%>

                                                    <!-- Magazzini -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('15');">
                                                        <a onclick="return 0">
                                                            <img alt="Gestione Magazzino" src="../AB_Immagini/icone32/Magazzino01.ico" />
                                                            <span class="hidden-none margin-r"><asp:Label runat="server" ID="lblGestioneMagazzino" meta:resourcekey="lblGestioneMagazzinoResource1">Gestione Magazzino</asp:Label></span>
                                                        </a>
                                                    </li>
                                                     <%End If%>

                                                    <%--<!-- Contatti -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('23');">
                                                        <a onclick="return 0">
                                                            <img alt="Gestione Contatti" src="../AB_Immagini/icone32/Contatti.ico" />
                                                            <span class="hidden-none margin-r"><asp:Label runat="server" ID="lblGestioneContatti" meta:resourcekey="lblGestioneContattiResource1">Gestione Contatti</asp:Label></span>
                                                        </a>
                                                    </li>  
                                                     <%End If%>--%>

                                                    <!-- Profilazione -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.ProfilazioneImpresa).Scrittura = True) Then%>
                                                        <li onclick="Gestione_Operazione_Menu('22');">
                                                            <a onclick="return 0">
                                                                <img alt="Profilazione Impresa" src="../AB_Immagini/icone32/Check32a.ico" />
                                                                <span class="hidden-none margin-r">Profilazione Impresa</span>
                                                            </a>
                                                        </li>                                         
                                                     <%End If%>

                                                    <!-- Gestione Pratiche e Servizi -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gestione_Servizi).Lettura = True OrElse permessi.getPermesso(enum_Security_Attivita.Gestione_Servizi_NEW).Lettura = True) Then%>
                                                        <li onclick="Gestione_Operazione_Menu('44');">
                                                            <a onclick="return 0">
                                                                <span style="margin-left: 30px;" class="hidden-none margin-r">Gestione Pratiche e Servizi</span>
                                                            </a>
                                                        </li>                                         
                                                     <%End If%>
                                                
                                                   <!-- Cartografia Aziendale -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_CartografiaAziendale).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('33');">
                                                        <a onclick="return 0">
                                                            <img alt="Gestione Utenti" src="../AB_Immagini/icone32/Cartografia01.ico" />
                                                            <span class="hidden-none margin-r">Cartografia Aziendale</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                   <!-- Gestione Utenti -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_UtentiPermessi).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('25');">
                                                        <a onclick="return 0">
                                                            <img alt="Gestione Utenti" src="../AB_Immagini/icone32/Persona32.ico" />
                                                            <span class="hidden-none margin-r">Gestione Utenti</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                    <!-- Impostazioni Utente -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('21');">
                                                        <a onclick="return 0">
                                                            <img alt="Impostazioni Utente" src="../AB_Immagini/icone32/Impostazioni01.ico" />
                                                            <span class="hidden-none margin-r">Impostazioni Utente</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>        
                                                 
                                                   <!-- Piano concimazione -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.SupportoDecisioni_PianoConcimazione).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('24');">
                                                        <a onclick="return 0">
                                                            <img alt="Impostazioni Utente" src="../AB_Immagini/icone32/Caratteristiche.ico" />
                                                            <span class="hidden-none margin-r">Piano Concimazione</span>
                                                        </a>
                                                    </li> 
                                                     <%End If%>   
                                                 
                                                   <!-- Planning -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Pianificazione_Produzione_Vegetale).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('27');">
                                                        <a onclick="return 0">
                                                            <img alt="Analisi Dati Schede Rilievi" src="../AB_Immagini/icone32/Documenti.ico"/> 
                                                            <span class="hidden-none margin-r">Pianificazione Piano Colturale</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>  

                                                   <!-- PUA ZOOTECNICO -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_PUA).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('37');">
                                                        <a onclick="return 0">
                                                            <img alt="PUA Zootecnico" src="../AB_Immagini/icone32/Mucca32.ico"/> 
                                                            <span class="hidden-none margin-r">PUA Zootecnico</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>  

                                                   <!-- PUA ZOOTECNICO 2.0 -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_PUA_2).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('53');">
                                                        <a onclick="return 0">
                                                            <img alt="PUA Zootecnico" src="../AB_Immagini/icone32/Mucca32.ico"/> 
                                                            <span class="hidden-none margin-r">Piano Utilizzazione Agronomica (PUA 2.0)</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>  

                                                   <!-- Analisi Dati Schede Rilievi -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.AnalisiSchedeRilievi).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('26');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Analisi Dati Schede Rilievi" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Analisi Dati Schede Rilievi</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>      

                                                   <!-- Tabelle Lookup Agenda -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.GestioneAvanzataETabelleLookUp).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('28');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Analisi Dati Schede Rilievi" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Tabelle Lookup Agenda</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%> 
                                                
                                                   <!-- Report Sostenibilità -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.ReportSostenibilità).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('29');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Analisi Dati Schede Rilievi" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Report Sostenibilità</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                   <!-- Analisi dei costi -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_AnalisiCosti).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('34');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Analisi dei costi</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>                                                     
                                                    
                                                    <!-- Gestione Completa CdG  -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gestione_Completa_CdG).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('41');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Controllo di Gestione</span>
                                                        </a>
                                                    </li>  
                                                     <%End If%>
                                                    
                                                    <!-- Inserimento costi CdG  -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_CdG).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('48');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Inserimento Costi</span>
                                                        </a>
                                                    </li>  
                                                     <%End If%>

                                                     
                                                    <!-- Inserimento ricavi CdG  -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_CdG).Scrittura = True) Then%>
                                                    <%-- SOSPESO
                                                        <li onclick="Gestione_Operazione_Menu('50');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Inserimento Ricavi</span>
                                                        </a>
                                                    </li>  --%>
                                                     <%End If%>
                                                    
                                                    <!-- Inserimento ricavi CdG  -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_CdG).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('51');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Scarico Tempi</span>
                                                        </a>
                                                    </li>  
                                                     <%End If%>

                                                    <!-- Report CdG  -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gestione_Report_CdG).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('49');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-eur" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:23px;">Report Costi - Ricavi</span>
                                                        </a>
                                                    </li>  
                                                     <%End If%>

                                                    <!-- scadenzario -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Scadenziario_Menu).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('31');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Scadenzario" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Scadenzario</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                    <!-- Menu Visite -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Visite_Lista).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('38');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Visite" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Visite</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                
                                                    <!-- Messaggistica ed SMS -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.invioSMS).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('32');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Scadenzario" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Messaggistica ed SMS</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   

                                                    
                                                
                                                    <!-- Report Percorsi -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.ReportPercorsi).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('43');">
                                                        <a onclick="return 0">
                                                            <img alt="Scadenzario" src="../AB_Immagini/icone32/Layer_Crust_Pin_32.png" />
                                                            <span style="margin-left: 8px;" class="hidden-none margin-r">Report Percorsi</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>   


                                                    <!-- CONDIZIONALITA -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_CartellaAziendale_Condizionalita).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('39');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Condizionalità" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Condizionalità</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>  

                                                    <!-- STATISTICHE UTILIZZO -->
                                                    <%If (permessi.getPermesso(enum_Security_Attivita.Statistiche_Sito).Lettura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('42');">
                                                        <a onclick="return 0">
                                                            <!-- <img alt="Condizionalità" src="../AB_Immagini/icone32/Impostazioni01.ico" /> -->
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Statistiche Utilizzo</span>
                                                        </a>
                                                    </li> 
                                                    <%End If%>  

                                                    <!-- Manuale GIAS e VideoCorsi -->
                                                    <li onclick="Gestione_Operazione_Menu('40');">
                                                        <a onclick="return 0">
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Manuale GIAS e Videocorsi</span>
                                                        </a>
                                                    </li>
                                                                   
                                                   <!-- Menu Stampe -->
                                                   <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Stampe).Scrittura = True) Then%>
                                                    <li onclick="Gestione_Operazione_Menu('7z');">
                                                        <a onclick="return 0">
                                                            <i class="fa fa-print" style="margin:0px; font-size:23px;"></i>
                                                            <span style="margin-left:14px;">Menu Stampe</span>
                                                        </a>
                                                    </li>
                                                    <%End If%>   

                                                    <!-- Torna al vecchio Menu -->
                                                    <li onclick="Gestione_Operazione_Menu('30');">
                                                        <a onclick="return 0">
                                                            <span style="margin-left: 40px;" class="hidden-none margin-r">Passa al Menu Precedente</span>
                                                        </a>
                                                    </li>

                                                </ul>
                                             </li>                        
                                     
               
                                        </ul>
                                    </div>
                                <!-- /.container-fluid -->
                                </div>
                            </nav>
                        </div>
                    </div>

                    <!-- filtri di ricerca -->
                    <div class="col-lg-12">
                        <ul id="panelbar" style="margin-bottom:20px;">
                            <li class="k-state-active k-active" id="panelbar_filtri">
                                <span class="k-link k-state-selected k-selected"><asp:Localize meta:resourcekey="jsLblFiltriRicerca" runat="server">Filtri di Ricerca</asp:Localize></span>
                                <div style="padding-top:10px;padding-bottom:10px;">
                                    <!-- filtro centro e specie -->
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="input-group">
                                                <label id="lblCentro" class="input-group-addon control-label alert-info" for="ddlCentri"><asp:Localize meta:resourcekey="lbl_centro" runat="server">Centro Aziendale</asp:Localize></label>
                                                <input type="text" id="ddlCentri" class="form-control" aria-describedby="lblCentro" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="input-group">
                                                <label id="lblSpecie" class="input-group-addon control-label alert-info" for="ddlSpecie"><asp:Localize meta:resourcekey="lbl_specie" runat="server">Specie</asp:Localize></label>
                                                <input type="text" id="ddlSpecie" class="form-control" aria-describedby="lblSpecie" />
                                            </div>
                                        </div>
                                    </div>

                                    <!-- filtro operazioni e appezzamenti -->
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="input-group">
                                                <label id="lbltipooperazione" class="input-group-addon control-label alert-info" for="ddlTipoOperazione">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TipoOperazione %>" runat="server">Tipo Operazione</asp:Localize>
                                                </label>
                                                <input type="text" class="form-control" id="ddlTipoOperazione" aria-describedby="lbltipooperazione" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6" id="divFiltroImpianti">
                                            <div class="input-group">
                                                <label id="lblimpianti" class="input-group-addon control-label alert-info" for="ddlImpianti">
                                                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Impianti %>" runat="server"></asp:Localize>
                                                </label>
                                                <input type="text" class="form-control" id="ddlImpianti" aria-describedby="lblimpianti"/>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- filtro date -->
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="input-group">
                                                <label id="lbldatainizio" class="input-group-addon control-label alert-info" for="<%= data_inizio.ClientID %>"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DA %>" runat="server"></asp:Localize></label>
                                                <input type="text" class="datepicker form-control" style="padding:5px;" id="data_inizio" maxlength="10" runat="server" aria-describedby="lbldatainizio"/>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="input-group">
                                                <label id="lbldatafine" class="input-group-addon control-label alert-info" for="<%= data_fine.ClientID %>"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, A %>" runat="server"></asp:Localize></label>
                                                <input type="text" class="datepicker form-control" style="padding:5px;" id="data_fine" maxlength="10" runat="server" aria-describedby="lbldatafine"/>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Pulsante -->
                                    <div class="row">
                                        <div class="col-lg-2">
                                            <div class="btn btn-success xonne-btn-primary" onclick="btnAggiorna_click();">
                                            <i class="fa fa-refresh"></i><asp:Localize meta:resourcekey="text_btnAggiorna" runat="server">Aggiorna</asp:Localize></div>
                                        </div>
                                    </div>
                                </div>

                            </li>
                        </ul>
                    </div>

                    <!-- lista operazioni -->
                    <div class="row">
                        <div class="col-lg-12" >
                            <!-- Tabella Risultati -->
                            <div role="tabpanel">
                                <!-- Nav tabs -->
                                <ul class="nav nav-tabs" role="tablist" id="tabDati">
                                    
                                    <!-- TUTTE -->
                                    <li class="active"><a href="#tb_tutte" id="linkTutte" role="tab" data-toggle="tab" aria-controls="tb_tutte" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_Tutte" runat="server">Quaderno (QDCA)</asp:Localize><span id="numElem_Qdc" class="numElem"></span></a></li>

                                    <!-- RICETTE - "DA FARE" -->
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Ricette).Lettura = True) Then%>
                                        <li><a href="#tb_ricette" id="linkRicette" role="tab" data-toggle="tab" aria-controls="tb_ricette" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_ordiniLavoroRicette" runat="server">Ordini di lavoro / Ricette</asp:Localize><span id="numElem_Ricette" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- BROGLIACCIO - "FATTO" -->
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Brogliaccio).Lettura = True) Then%>
                                        <li><a href="#tb_brogliaccio" id="linkBrogliaccio" role="tab" data-toggle="tab" aria-controls="tb_brogliaccio" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_brogliaccio" runat="server">Brogliaccio</asp:Localize><span id="numElem_Brogliaccio" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- COLTURALI -->
                                    <%If (False AndAlso VisualizzaTabColturali) Then%>
                                        <li><a href="#tb_colturali" id="linkColturali" role="tab" data-toggle="tab" aria-controls="tb_colturali" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_colturali" runat="server">Colturali</asp:Localize><span id="numElem_opColturali" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- MAGAZZINO/CONTABILI -->
                                    <%If (False AndAlso VisualizzaTabMagContab) Then%>
                                        <li><a href="#tb_magcont" id="linkMagCont" role="tab" data-toggle="tab" aria-controls="tb_magcont" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_magazzinoContabili" runat="server">Magazzino/Contabili</asp:Localize><span id="numElem_opMagCont" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- VISITE/AUDIT -->
                                    <%If (False AndAlso VisualizzaTabAudit) Then%>
                                        <li><a href="#tb_audit" id="linkAudit" role="tab" data-toggle="tab" aria-controls="tb_audit" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_visiteAudit" runat="server">Visite/Audit</asp:Localize><span id="numElem_opAudit" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- ZOO -->
                                    <%If (permessi.getPermesso(enum_Security_Attivita.Gest_Stalle).Lettura = True) Then%>
                                        <li><a href="#tb_zoo" id="linkZoo" role="tab" data-toggle="tab" aria-controls="tb_zoo" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_zoo" runat="server">Zoo</asp:Localize><span id="numElem_opZoo" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- MACCHINE -->
                                    <%If (False AndAlso VisualizzaTabMacchine) Then%>
                                        <li><a href="#tb_macchine" id="linkMacchine" role="tab" data-toggle="tab" aria-controls="tb_macchine" onclick="tab_onClick(this);"><asp:Localize meta:resourcekey="anchor_macchine" runat="server">Macchine</asp:Localize><span id="numElem_opMacchine" class="numElem"></span></a></li>
                                    <%End If%>

                                    <!-- METEO -->
                                    <li id="AgroMeteoContainer" style="max-height:36px;margin-top:-4px;">
                                        <div>
                                            <cc1:AgroMeteo ID="agrometeo" runat="server" />
                                        </div>
                                    </li>
                                </ul>
                                <div class="tab-content" id="tabDiv">
                                    <div class="tab-pane fade active in" id="tb_tutte" role="tabpanel" style="overflow: auto" aria-labelledby="linkTutte">
                                        <div id="t_tutte"></div>
                                        
                                        <div id="divTipoGrigliaOperazioniContainer" class="row" style="margin-top: 7px; display:none">                                            
                                            <div class="col-lg-6" id="divTipoGrigliaOperazioni">
                                                <div class="input-group">
                                                    <label id="lblTipoGrigliaOperazioni" class="input-group-addon control-label alert-info" for="ddlTipoGrigliaOperazioni">
                                                        <asp:Localize Text="<%$ Resources: TipoGriglia %>" runat="server"></asp:Localize>
                                                    </label>
                                                    <select class="form-control" id="ddlTipoGrigliaOperazioni" aria-describedby="lblTipoGrigliaOperazioni">
                                                        <option value="1"><asp:Localize Text="<%$ Resources: ListaOperazioniStandard %>" runat="server"></asp:Localize></option>
                                                        <option value="2"><asp:Localize Text="<%$ Resources: ListaOperazioniPerCampo %>" runat="server"></asp:Localize></option>
                                                        <option value="3"><asp:Localize Text="<%$ Resources: ListaOperazioniPerImpianto %>" runat="server"></asp:Localize></option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="divKendoOperazioni" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendo_Valorizzazione"/>

                                        <div id="divKendoOperazioniDettagliDestinazioni" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoOperazioniDettagliDestinazioni_Valorizzazione"/>

                                        <input type="hidden" id="ricetta_cod" runat="server" />
                                    </div>

                                    <div class="tab-pane fade" id="tb_ricette" role="tabpanel" style="overflow: auto" aria-labelledby="linkRicette">
                                        <div id="divKendoRicette" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoRicette_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_brogliaccio" role="tabpanel" style="overflow: auto" aria-labelledby="linkBrogliaccio">
                                        <div id="divKendoBrogliaccio" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoBrogliaccio_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_colturali" role="tabpanel" style="overflow: auto" aria-labelledby="linkColturali">
                                        <div id="divKendoColturali" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoColturali_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_magcont" role="tabpanel" style="overflow: auto" aria-labelledby="linkMagCont">
                                        <div id="divKendoMagCont" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoMagCont_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_audit" role="tabpanel" style="overflow: auto" aria-labelledby="linkAudit">
                                        <div id="divKendoAudit" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoAudit_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_zoo" role="tabpanel" style="overflow: auto" aria-labelledby="linkZoo">
                                        <div id="divKendoZoo" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoZoo_Valorizzazione"/>
                                    </div>

                                    <div class="tab-pane fade" id="tb_macchine" role="tabpanel" style="overflow: auto" aria-labelledby="linkMacchine">
                                        <div id="divKendoMacchine" class="divGrigliaKendo"></div>
                                        <input type="hidden" id="hdKendoMacchine_Valorizzazione"/>
                                    </div>

                                </div>
                            </div>
                            <!-- end Tabella Risultati -->
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>



        <!-- Dialog Nuova/aggiungi ricetta -->
    <div class="modal fade" id="modalCreaRicetta" data-backdrop="static" data-keyboard="false" >
        <div class="modal-dialog">
            <div class="modal-content">
                <form id="form_nuova_ricetta" method="get" action="">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="lbl_new_item">                            
                            <asp:Localize meta:resourcekey="jsLblCreaRicetta" runat="server">Crea Ricetta</asp:Localize>
                        </h4>
                    </div>
                    <div class="modal-body">
                       <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info"><asp:Localize meta:resourcekey="jsLblDescrizione" runat="server">Descrizione</asp:Localize></span>
                                            <asp:TextBox ID="Txt_Ricetta_Descrizione" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info"><asp:Localize meta:resourcekey="jsLblNumero" runat="server">Numero</asp:Localize></span>
                                            <asp:TextBox ID="Txt_Ricetta_Numero" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">                                                            
                            <div class="col-lg-6">
                                <div class="input-group">
                                    <label id="lbldatainizio_ricetta" class="input-group-addon control-label alert-info" for="<%= data_inizio_ricetta.ClientID %>">
                                    <asp:Localize meta:resourcekey="lbl_dataInizio" runat="server">Da</asp:Localize>
                                    </label>
                                    <input class="kendoCalendar form-control"  id="data_inizio_ricetta" maxlength="10" runat="server" aria-describedby="lbldatainizio_ricetta" required/>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="input-group">
                                    <label id="lbldatafine_ricetta" class="input-group-addon control-label alert-info" for="<%= data_fine_ricetta.ClientID %>">
                                    <asp:Localize meta:resourcekey="lbl_dataFine" runat="server">A</asp:Localize>
                                    </label>
                                    <input class="kendoCalendar form-control" id="data_fine_ricetta" maxlength="10" runat="server" aria-describedby="lbldatafine_ricetta" required/>
                                </div>
                            </div>
                        </div>
<%--                        <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <asp:Panel ID="NoteGiustPanel" runat="server" meta:resourcekey="NoteGiustPanelResource1">
                                    <!-- Note -->
                                    <div role="tabpanel">
                                        <ul class="nav nav-tabs" role="tablist">
                                            <li id="tabGiust" role="presentation"><a href="#tabs-2" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblGiustificazioni" runat="server" meta:resourcekey="lblGiustificazioniResource1">Note Ricorrenti</asp:Label>
                                            </a></li>
                                            <li id="tabNote" role="presentation"><a href="#tabs-1" aria-controls="home" role="tab" data-toggle="tab">
                                                <asp:Label ID="lblNote" runat="server" meta:resourcekey="lblNoteResource1">Note</asp:Label>
                                            </a></li>
                                            <li id="tabMeteo" role="presentation" runat="server"><a href="#<%=tab_meteo.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="LblMeteo" runat="server" meta:resourcekey="LblMeteoResource1">Meteo</asp:Label>
                                            </a></li>
                                            <li id="tabVentoIntensita" role="presentation" runat="server"><a href="#<%=tab_ventoint.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblVentoIntensità" runat="server" meta:resourcekey="lblVentoIntensitàResource1">Vento Intensita</asp:Label>
                                            </a></li>
                                            <li id="tabVentoDirezione" runat="server" role="presentation"><a href="#<%=tab_ventodir.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblVentoDirezione" runat="server" meta:resourcekey="lblVentoDirezioneResource1">Vento Direzione</asp:Label>
                                            </a></li>
                                            <li id="tabTemperatura" runat="server" role="presentation"><a href="#<%=tab_temperatura.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblTemperatura" runat="server" meta:resourcekey="lblTemperaturaResource1">Temperatura</asp:Label>
                                            </a></li>
                                            <li id="tabOrario" runat="server" role="presentation"><a href="#<%=tab_orario.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblOrario" runat="server" meta:resourcekey="lblOrarioResource1">Orario</asp:Label>
                                            </a></li>
                                            <li id="tabMotivazioni" runat="server" role="presentation"><a href="#<%=tab_motivazioni.ClientID%>" aria-controls="home" role="tab"
                                                data-toggle="tab">
                                                <asp:Label ID="lblMotivazione" runat="server" meta:resourcekey="lblMotivazioneResource1">Motivazione</asp:Label>
                                            </a></li>
                                        </ul>

                                        <div class="tab-content">
                                            <div id="tabs-2" role="tabpanel" class="tab-pane active" style="max-height: 180px; overflow: auto;">
                                                <asp:UpdatePanel ID="update_consigli" runat="server">
                                                    <ContentTemplate>
                                                        <asp:CheckBoxList ID="CBL_Consigli" CssClass="checkbox table table-striped" runat="server" Width="100%"
                                                            meta:resourcekey="CBL_ConsigliResource1">
                                                        </asp:CheckBoxList>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <div id="tabs-1" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:TextBox ID="Txt_Note" runat="server" TextMode="MultiLine" CssClass="form-control"
                                                    Style="height: 100%" meta:resourcekey="Txt_NoteResource1"></asp:TextBox>
                                            </div>
                                            <div id="tab_meteo" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_Meteo" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>
                                            <div id="tab_ventoint" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_VentoIntensita" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>
                                            <div id="tab_ventodir" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_VentoDirezione" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>
                                            <div id="tab_temperatura" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_Temperatura" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>
                                            <div id="tab_orario" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_Orario" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>
                                            <div id="tab_motivazioni" runat="server" role="tabpanel" class="tab-pane" style="max-height: 180px; overflow: auto;">
                                                <asp:CheckBoxList ID="CBL_Motivazione" CssClass="form-control" Height="76px" runat="server" Width="100%"></asp:CheckBoxList>
                                            </div>

                                        </div>
                                    </div>
                                </asp:Panel>

                            </div>
                        </div>--%>

                        <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <span class="input-group-addon alert-info"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Nota %>" runat="server">Nota</asp:Localize></span>
                                            <asp:TextBox ID="Txt_Ricetta_Nota" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <input type="hidden" id="piva_ricetta" />
                                <input type="hidden" id="sacod_ricetta" />
                                <input type="hidden" id="idagenda_ricetta" />
                                <input type="hidden" id="datainizio_ricetta_min" />
                                <input type="hidden" id="datafine_ricetta_max" />
                            <div id="lbl_operazioni_ricetta_da_creare">
                            </div>
                            </div>
                         </div>
                    </div>     
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" id="btn_nuova_ricetta">
                            <i class="fa fa-plus"></i>
                            <asp:Localize meta:resourcekey="jsLblCreaRicetta" runat="server">Crea Ricetta</asp:Localize></button>
                    </div>
            
                </form>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->





</asp:Content>
<asp:Content ID="cont" ContentPlaceHolderID="ContentScript" runat="server">
    <div id="cont_script"></div>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.jstree.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Ricette.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Brogliaccio.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Colturali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_MagCont.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Audit.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Macchine.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_Zoo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBS_Agenda_Nuovo_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("MenuBs_Agenda_Nuovo_globali.js") %>"></script>
    <script type="text/javascript" src="../Scripts/bootbox.min.js?<% =Application("GiasVersioneCorrente")%>"></script>

    <script type="text/javascript">

        var Attiva_Menu_Agenda_Visualizzazione_Dettagli_Operazioni = <%=permessi.getPermesso(enum_Security_Attivita.Menu_Agenda_Visualizzazione_Dettagli_Operazioni).Scrittura.ToString().ToLower() %>;
        var lav_cod_copiabili = [<%=String.Join(",", LAV_COD_COPIABILI)%>];
        var lav_cod_non_editabili = [<%=String.Join(",", LAV_COD_NON_EDITABILI)%>];

        var lav_cod_ricettabili = [<%=String.Join(",", ListLavCodRicettabili)%>];

        var defaultTab = <%= DefaultTab %>;
        var mode ="<%= Mode %>";
        var permessoImpostazioniUtente = <%=permessi.getPermesso(enum_Security_Attivita.Gest_UtentiImpostazioni).Scrittura.ToString().ToLower()%>;
        var data_inizio_ClientID = '#<%= data_inizio.ClientID %>';
        var data_fine_ClientID = '#<%= data_fine.ClientID %>';


        var ricetta_descrizione_ClientID = '#<%= Txt_Ricetta_Descrizione.ClientID %>';
        var ricetta_numero_ClientID = '#<%= Txt_Ricetta_Numero.ClientID %>';
        var ricetta_datainizio_ClientID = '#<%= data_inizio_ricetta.ClientID %>';
        var ricetta_datafine_ClientID = '#<%= data_fine_ricetta.ClientID %>';
        var ricetta_nota_ClientID = '#<%= Txt_Ricetta_Nota.ClientID %>';

        var dati_Tabella;
        var ricetta_cod_ClientID = '#<%= ricetta_cod.ClientID %>';

        var UtenteAbilitatoRicette =  <%=permessi.getPermesso(enum_Security_Attivita.Gest_Ricette).Scrittura.ToString().ToLower()%>;
        var UtenteAbilitatoBrogliaccio =  <%=permessi.getPermesso(enum_Security_Attivita.Brogliaccio).Scrittura.ToString().ToLower()%>;

        var enumRicette = <% = enum_Security_Attivita.Gest_Ricette %>;
        var enumBrogliaccio = <% = enum_Security_Attivita.Brogliaccio %>;
        var enumAgenda = <% = enum_Security_Attivita.Agenda_AccessoMenu %>;
        var enumZoo = <% = enum_Security_Attivita.Gest_Stalle %>;

        var enumScrittura = <% = enum_Security_Operazione.Scrittura %>;
        var enumModifica = <% = enum_Security_Operazione.Modifica %>;
        var enumCancellazione = <% = enum_Security_Operazione.Cancellazione %>;
        var enumLettura = <% = enum_Security_Operazione.Lettura %>;

        function Carica_Elenco_Preferiti_Stampe(deferred) {
            var GestioneWaitFrame = true;
            if (deferred != undefined) { GestioneWaitFrame = false };
            ajaxAgronica('MenuBS_Agenda_Nuovo.aspx/Preleva_Preferiti_Stampe',
                '{}',
                function (risposta) {

                    //codice success
                    var objElenco = JSON.parse(risposta.RispostaStringa);
                    var objMenuStampe = document.getElementById("li_stampe")
                    var html = "";

                    if (Object.keys(objElenco).length == 0) {
                        //objMenuStampe.innerHTML = "";
                    }

                    //else if (Object.keys(objElenco).length == 1) {

                    //    var key = Object.keys(objElenco)[0];
                    //    var nomeStampa = objElenco[key];
                    //    objMenuStampe.setAttribute("onclick", "Gestione_Operazione_Menu('" + key + "');");

                    //    html += '<a onclick="return 0">'
                    //        + '<i class="fa fa-print" style="font-size:23px;"></i>'
                    //        + '<span>STAMPA</span>';

                    //    objMenuStampe.innerHTML = html;
                    //}

                    //else if (Object.keys(objElenco).length > 1) {

                    else {

                        objMenuStampe.setAttribute("class", "dropdown");

                        html += '<a class="dropdown-toggle" data-toggle="dropdown" href="#">'
                            + '<i class="fa fa-print" style="font-size:23px;"></i>'
                            + '<span class="hidden-none margin-r">STAMPA</span>'
                            + '<span class="caret"></span>'
                            + '</a>'
                            + '<ul class="dropdown-menu">';

                        for (var key in objElenco) {
                            var strGino = "<li onclick='Gestione_Operazione_Menu(\" " + key + " \");'>" +
                                "<a onclick='return 0'>" +
                                "<span>" + objElenco[key] + "</span>" +
                                "</a>" +
                                "</li>";
                            html += strGino;
                        }

                        // aggiungo menu stampe alle stampe preferite
                        html += '<li onclick="Gestione_Operazione_Menu(\'7z\');"><a onclick="return 0"><span>Menu Stampe</span></a></li>';

                        html += '</ul>';

                        objMenuStampe.innerHTML = html;
                    }

                    if (deferred != undefined) {
                        deferred.resolve();
                    }

                }, null, undefined, GestioneWaitFrame, deferred);
        }

        function Gestione_Operazione_Menu(v) {

            // Validazione per alcuni click (stampe)
            if (v === "11")
                return;

            $('.error').empty();

            var spe = $('#ddlSpecie').val();
            var lavcod = 0

            if (v === '6e') {
                lavcod = getkeynuovoRicette();
                if (lavcod === "")
                    return;
            }

            var URL_WS = '';
            var parameteri = '';

            //PER QUESTE OPERAZIONI E' NECESSARIO PASSARE LE RIGHE SELEZIONATE
            if (v === '5d' || v === '6b' || v === '45' || v === '46') {

                var rows = elementiGrigliaSelezionati();

                //Impedisco la modifica per l'operazione di Cura
                if (v === '5d' && rows.findIndex(r => parseInt(r.Lav_cod) === 5004) > -1)
                    return;

                var final_rows = [];
                jQuery.each(rows, function (i, val) {
                    final_rows.push({
                        Data: val['Data'],
                        Operazione: val['chiave_composita'],
                        Piva: val['Piva'],
                        Sa_Cod: val['sa_cod'],
                        Lav_Cod: val['Lav_cod'],
                        Lav_Des: val['Lav_Des'],
                        Id_Agenda: val['id_agenda'],
                        Blocco_Flag: val['blocco_flag'],
                        Veg_Cod: val['Veg_cod'],
                        Ricetta_Cod: val['Ricetta_Cod'],
                        Rag_Soc: val['Rag_Soc'],
                        Gru_Des: val['gru_des']
                    });
                });

                URL_WS = 'MenuBS_Agenda_Nuovo.aspx/Gestione_Operazione_ConRigheSelezionate';
                testoJSON = JsonEscape(JSON.stringify(final_rows));
                parameteri = '{lav_cod:"' + lavcod + '", tipo_operazione:"' + v + '", specie:"' + spe + '", righe_selezionate: "' + testoJSON + '"}';

            } else {

                URL_WS = 'MenuBS_Agenda_Nuovo.aspx/Gestione_Operazione';
                let strImpianti = Get_MultiselString("ddlImpianti");
                strImpianti = strImpianti === null ? "" : strImpianti;
                parameteri = '{lav_cod:"' + lavcod + '", tipo_operazione:"' + v + '", specie:"' + spe + '", impianti: "' + strImpianti + '" }';

            }


            // Webservice per Operazione Menu
            $.ajax({
                type: 'POST',
                url: URL_WS,
                data: parameteri,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (risposta) {

                    if (risposta.d.RispostaOK) {
                        if ((risposta.d.ParametroDue_stringa) && (risposta.d.ParametroDue_stringa != "")) {
                            switch (risposta.d.Tipo) {
                                // Script JS
                                case "1":
                                    $('#cont_script').append(risposta.d.ParametroDue_stringa);
                                    break;
                                // Inserimento codice HTML in....
                                case "2":
                                    break;
                            }
                        } else {
                            if (risposta.d.RispostaStringa !== "") {
                                window.location = risposta.d.RispostaStringa;
                                WaitFrame.show();
                            } else {
                                kendo.alert("Operazione non implementata");
                            }
                        }
                    } else
                        alert(risposta.d.Errore);
                }
            });
        }

        function AddPreferiti(lav_cod, lav_des) {
            var k;
            if (!localStorage.OpPreferite)
                k = [];
            else
                k = JSON.parse(localStorage.OpPreferite);

            //controllo se esiste già lavCod
            var i = 0;
            var trovato = false;
            for (i = 0; i < k.length; i++) {
                if (k[i].lav_cod == lav_cod)
                    trovato = true;
            }
            //aggiungo
            if (trovato == false)
                k.push({ lav_cod: lav_cod, lav_des: lav_des });
            //se ho più di 10 elementi elimino l ultimo
            if (k.length > 4)
                k.shift();
            //salvo
            localStorage.OpPreferite = JSON.stringify(k);
        }

        function getkeynuovoRicette() {

            var op = $("#ddlRicette").data("kendoDropDownList");
            var valore = op.value();

            if (valore == "") {
                alert('Selezionare una ricetta!');
                return "";
            } else {
                return valore;
            }
        }

        function onCollapsePannelloFiltri(e) {
            var date = new Date();
            date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
            $.removeCookie("MenuBS_Agenda_Nuovo.PannelloFiltriAperto");
            $.cookie("MenuBS_Agenda_Nuovo.PannelloFiltriAperto", "False", { expires: date, path: '/' });
        }

        function onExpandPannelloFiltri(e) {
            var date = new Date();
            date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
            $.removeCookie("MenuBS_Agenda_Nuovo.PannelloFiltriAperto");
            $.cookie("MenuBS_Agenda_Nuovo.PannelloFiltriAperto", "True", { expires: date, path: '/' });
        }

        function inizializzaPannelloFiltri() {
            $("#panelbar").kendoPanelBar();

            if ($.cookie("MenuBS_Agenda_Nuovo.PannelloFiltriAperto") !== "True") {
                $("#panelbar").data("kendoPanelBar").collapse($("#panelbar_filtri"));
            }

            $("#panelbar").data("kendoPanelBar").bind("collapse", onCollapsePannelloFiltri);
            $("#panelbar").data("kendoPanelBar").bind("expand", onExpandPannelloFiltri);
        }

        function redirectOperazionePreferita(lav_cod) {

            if (Request_QueryString("gis") === "true") {
                if (!window.parent.apriPreselezioneAgendaVerifica()) {
                    window.parent.preselezioneAgenda(lav_cod, undefined, -1, Enum_TipoOperazioneDB.Scrittura.value);
                }

            } else {
                let Veg_Cod = -1;
                let kendoSpecie = $("#ddlSpecie").data("kendoDropDownList");
                if ((kendoSpecie.value() !== "0" || kendoSpecie.value() !== "")) {
                    Veg_Cod = kendoSpecie.value();
                }

                // Webservice per Redirect Preferiti
                $.ajax({
                    type: 'POST',
                    url: 'MenuBS_Agenda_Nuovo.aspx/NuovaOperazioneAgenda',
                    data: "{lavcod:'" + lav_cod + "', Veg_Cod: '" + Veg_Cod + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        if (r.d != "error") {
                            window.location.replace(r.d);
                        } else {
                            kendo.alert("Non si hanno i permessi per eseguire l'operazione");
                        }
                    }
                });
            }
        }

        function redirectOperazioneRicettaPreferita(lav_cod) {

            // Webservice per Redirect Preferiti
            $.ajax({
                type: 'POST',
                url: 'MenuBS_Agenda_Nuovo.aspx/NuovaOperazioneRicettaAgenda',
                data: "{lavcod:'" + lav_cod + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d != "error") {
                        window.location.replace(r.d);
                    } else {
                        kendo.alert("Non si hanno i permessi per eseguire l'operazione");
                    }
                }
            });
        }

        function redirectOperazioneBrogliaccioPreferita(lav_cod) {

            // Webservice per Redirect Preferiti
            $.ajax({
                type: 'POST',
                url: 'MenuBS_Agenda_Nuovo.aspx/NuovaOperazioneBrogliaccioAgenda',
                data: "{lavcod:'" + lav_cod + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    if (r.d != "error") {
                        window.location.replace(r.d);
                    } else {
                        kendo.alert("Non si hanno i permessi per eseguire l'operazione");
                    }
                }
            });
        }

        function DoPostBack_ControlliSiNo(str) {

            WaitFrame.show();
            var dCancellazione = $.Deferred();
            var dRicaricamentoDati = $.Deferred();

            var GestioneWaitFrame = false;

            if (str.startsWith('del_elem')) {

                SalvaParametriDiv('#frmInput', false);

                ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/elimina_operazione_multipla", JSON.stringify({ strChiaviComposite: str, proseguiInCasoDiAlert: false }),
                    function (risposta) {

                        dCancellazione.resolve();

                        if (risposta.RispostaOK) {
                            $("<div></div>").kendoAlert({ title: "Tutto Bene", content: risposta.RispostaStringa }).data("kendoAlert").open();
                            btnAggiorna_click(dRicaricamentoDati); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                        } else {
                            //non cancellabile
                            dRicaricamentoDati.resolve();
                            MessaggioErrore(risposta.Errore);
                        }

                    }, function (risposta) {
                        dCancellazione.resolve();
                        dRicaricamentoDati.resolve();
                        if (risposta.RispostaConferma == true) {
                            //ConfermaControlliSiNo(risposta.Errore, 'conferma_' + str);
                            messaggioConfermaKendo(risposta.Errore, 'conferma_' + str);
                        } else {
                            MessaggioErrore(risposta.Errore);
                        }

                    }, undefined, GestioneWaitFrame, dCancellazione);

            } else if (str.startsWith('conferma_del_elem')) {

                SalvaParametriDiv('#frmInput', false);

                ajaxAgronica("MenuBS_Agenda_Nuovo.aspx/elimina_operazione_multipla", JSON.stringify({ strChiaviComposite: str, proseguiInCasoDiAlert: true }),
                    function (risposta) {

                        dCancellazione.resolve();

                        if (risposta.RispostaOK) {
                            $("<div></div>").kendoAlert({ title: "Tutto Bene", content: risposta.RispostaStringa }).data("kendoAlert").open();
                            btnAggiorna_click(dRicaricamentoDati); //emulo il click del bottone aggiorna in quanto la cancellazione di un elemento potrebbe influire sugli elementi delle altre tab
                        }
                        else {
                            //non cancellabile
                            dRicaricamentoDati.resolve();
                            MessaggioErrore(risposta.Errore);
                        }

                    }, function (risposta) {
                        dCancellazione.resolve();
                        dRicaricamentoDati.resolve();
                        MessaggioErrore(risposta.Errore);

                    }, undefined, GestioneWaitFrame, dCancellazione);
            }

            $.when(dCancellazione, dRicaricamentoDati).done(function () {
                WaitFrame.hide();
            });

        }

        function CaricaGrigliaOperazioni(deferred) {

            if ($("#divKendoOperazioni").html() === '') {
                var data_inizio = $(data_inizio_ClientID).val();
                var data_fine = $(data_fine_ClientID).val();
                var sa_cod = $('#ddlCentri').val();
                var veg_cod = $('#ddlSpecie').val();

                var TipoOperazione = [];
                var tipoOp = $('#ddlTipoOperazione').data("kendoMultiSelect")
                if (tipoOp) {
                    TipoOperazione = tipoOp.value();
                }

                var impianti = [];
                var imp = $('#ddlImpianti').data("kendoMultiSelect");
                if (imp) {
                    impianti = imp.value();
                }

                KendoOperazioni_leggi(undefined, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti);
            }
        }

        function CaricaGrigliaOperazioniDestinazioneDettagli(deferred, RaggruppaPerCampo) {

            if ($("#divKendoOperazioniDettagliDestinazioni").html() === '') {
                var data_inizio = $(data_inizio_ClientID).val();
                var data_fine = $(data_fine_ClientID).val();
                var sa_cod = $('#ddlCentri').val();
                var veg_cod = $('#ddlSpecie').val();

                var TipoOperazione = [];
                var tipoOp = $('#ddlTipoOperazione').data("kendoMultiSelect")
                if (tipoOp) {
                    TipoOperazione = tipoOp.value();
                }

                var impianti = [];
                var imp = $('#ddlImpianti').data("kendoMultiSelect");
                if (imp) {
                    impianti = imp.value();
                }

                KendoOperazioniDestinazioneDettagli_leggi(undefined, data_inizio, data_fine, sa_cod, veg_cod, deferred, TipoOperazione, impianti, RaggruppaPerCampo);
            }

        }
    </script>

    <script id="templateLegendaMenuAgendaOperazioniTutte" type="text/x-kendo-template">
        
            <%If CaricaDatiApp AndAlso (permessi.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_ImportazioneRicetteDaInterscambioApp).Scrittura = True) Then%>
            <div style="float:left;">
                <div id="btnImportaAgendaDaTabelleAPP" class='btn btn-warning' style='display:block;border:0px;margin-bottom:0px;' onclick='<%= If(SincroDatiApp, "CaricaAgendaDaTabelleAPP", "ImportaAgendaDaTabelleAPP") %>(<%= If(ImportaSoloAziendaSelezionata, "true", "false") %>);'>Carica dati APP</div>
            </div>
            <%End If%>
        <div class="hidden-xs" style="float:right;max-width:415px;">
          <div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:\\#cccccc;"></div>
            <div style="float:left;margin-left:3px;"><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OperazioneBloccata %>" runat="server"></asp:Localize></div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:yellow;margin-left:10px;"></div>
            <div style="float:left;margin-left:3px;"><asp:Localize Text="<%$ Resources: OperazioneDaVerificare %>" runat="server"></asp:Localize></div>
            <div style="float:left;">
                <span style="float:left;width:28px;height:12px;border:1px solid black;background-color:\\#80ffff"></span>
                <span style="margin-left:3px;"><asp:Localize Text="<%$ Resources: OperazionePianificata %>" runat="server"></asp:Localize></span>
            </div>
            <div style="float:both;"></div>
          </div>
        </div>
    </script>

    <script id="templateLegendaMenuAgendaRicette" type="text/x-kendo-template">
        <div style="float:right;max-width:275px;">
          <div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:\\#cccccc;"></div>
            <div style="float:left;margin-left:3px;">Ricetta Bloccata</div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:yellow;margin-left:10px;"></div>
            <div style="float:left;margin-left:3px;">Ricetta Pubblica</div>
            <div style="float:left;">
                <span style="float:left;width:28px;height:12px;border:1px solid black;background-color:lightgreen"></span>
                <span style="margin-left:3px;">Ricetta salvata in Brogliaccio</span>
            </div>
            <div style="float:both;"></div>
          </div>
        </div>
    </script>

    <script id="templateLegendaMenuAgendaBrogliaccio" type="text/x-kendo-template">
        <%If CaricaDatiApp AndAlso (permessi.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_ImportazioneRicetteDaInterscambioApp).Scrittura = True) Then%>
        <div style="float:left;">
            <div id="btnImportaRicetteDaTabelleAPP" class='btn btn-warning' style='display:block;border:0px;margin-bottom:0px;' onclick='<%= If(SincroDatiApp, "CaricaRicetteDaTabelleAPP", "ImportaRicetteDaTabelleAPP") %>(<%= If(ImportaSoloAziendaSelezionata, "true", "false") %>);'>Carica dati APP</div>
        </div>
        <%End If%>
        <%If (permessi.getPermesso(enum_Security_Attivita.ManutenzioneArchivi_CodificaProdottiDaInterscambioApp).Scrittura = True) Then%>
        <%--<div style="float:left;">
            <div id="btnImportaProdottiInterventiAPP" class='btn btn-warning' style='display:block;border:0px;margin-bottom:0px;' onclick='ImportaProdottiInterventiAPP(<%= If(ImportaSoloAziendaSelezionata, "true", "false") %>);'>Carica prodotti APP</div>
        </div>--%>
        <div style="float:left;">
            <div id="btnCodificaProdottiAPP" class='btn btn-warning' style='display:block;border:0px;margin-bottom:0px;' onclick='CodificaProdottiAPP();'>Codifica prodotti APP</div>
        </div>
        <%End If%>
        <div style="float:right;max-width:275px;">
          <div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:\\#cccccc;"></div>
            <div style="float:left;margin-left:3px;">Ricetta Bloccata</div>
            <div style="float:left;width:28px;height:12px;border:1px solid black;background-color:yellow;margin-left:10px;"></div>
            <div style="float:left;margin-left:3px;">Ricetta Pubblica</div>
            <div style="float:left;">
                <span style="float:left;width:28px;height:12px;border:1px solid black;background-color:lightgreen"></span>
                <span style="margin-left:3px;">Ricetta salvata in Agenda</span>
            </div>
            <div style="float:both;"></div>
          </div>
        </div>
    </script>

    <style>
        .info_elem, .info_impianti, .info_appezza, .info_centro, .info_campo, .info_piva, .info_catasto, .info_fabbricato, .info_contatto, .info_macchina {
            color: #052747 !important;
        }

        .k-multiselect .k-chip {
            height: auto;
            padding: 2px 6px;
        }
        
        .k-multiselect .k-chip .k-chip-label{
            white-space: normal;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel_script" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


