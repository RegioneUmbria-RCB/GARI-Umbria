<%@ Control Language="vb"
AutoEventWireup="true" 
ClassName="mappaBS2"
CodeBehind="mappaBS2.ascx.vb"
Inherits="AgronicaControlliGIS.mappaBS2" %>


<!-- ascx per pagina GIS in versione bootstrap -->

<script>
    var variabile_gps = 'replace_gps';
    var GiasBase_Domain = '__basePath__';
    var PATH_GIASBASE = '__PATH_GIASBASE__'
</script>

<style type="text/css">

    .gis-navbar .fa {
        font-size: 16px;
    }

    .nav.navbar-nav > div {
        float: left;
        margin-left: 1px;
        margin-right: 1px;
    }

        .nav.navbar-nav > div:first-child {
            margin-left: 0px;
            margin-right: 1px;
        }

        .nav.navbar-nav > div:last-child {
            margin-left: 1px;
            margin-right: 0px;
        }

    .masked-container > .k-maskedtextbox {
        background-color: transparent;
    }

    .gis-red {
        color: red !important;
    }

    .gis-menuImg {
        width: 16px;
        height: 16px;
        margin-right: 10px;
    }

    .row.display-flex {
        display: flex;
        align-items: center;
    }

    .row.display-flex > [class*='col-'] {
        display: flex;
        flex-direction: column;
    }

    .Nascosto {
        display: none !important;
    }

</style>

<div id="mainGisBS" style="position:absolute; width:100%; height:100%;">

    <nav id="mainGisNavBar" class="navbar navbar-default" style="margin:0px;">
        <div id="mainGisNavBarHeader" class="navbar-header">

            <div class="col-xs-1 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menuFix">
                <!-- imposta impresa da selezione -->
                <div class="img_tool StrumentoGPS" id="StrumentoGPSHeader">
                    <span class="fa fa-location-arrow fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-1 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menuFix">
                <!-- info -->
                <div class="img_tool" id="info_appezzamentoHeader">
                    <span class="fa fa-info fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu1">
                <!-- Poligono -->
                <div class="img_tool" id="disenga_poligonoHeader">
                    <span class="fa fa-pencil-square-o fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu2">
                <!-- Insieme di Punti -->
                <div class="img_tool" id="multipointHeader">
                    <span class="fa fa-map-marker fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu1">
                <!-- Salva -->
                <div class="img_tool" id="save-buttonHeader">
                    <span class="fa fa-floppy-o fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu1 menu2">
                <!-- Elimina -->
                <div class="img_tool" id="delete-buttonHeader">
                    <span class="fa fa-trash-o fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu2">
                <!-- Nuova Op agenda -->
                <div class="img_tool" id="nuova_agendaHeader">
                    <span class="fa fa-book fa-3x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm hidden-xs">
                <ul class="nav navbar-nav">
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                            <!-- disegna -->
                            <div class="img_tool" id="disenga_poligonoHeader">
                                <span id="StrumentoDisegnoCorrente" class="fa fa-pencil-square-o fa-2x"></span>
                            </div>
                            <span class="caret" style="margin-left: 10px; margin-top: -5px"></span>
                        </a>
                        <ul class="dropdown-menu">
                            <li><a href="#" onclick="interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.POLYGON); return false;"><span id="StrumentoDisegnoCorrentePoly" class="fa fa-pencil-square-o fa-3x"></span><span id="lbl_menu_poligono"><asp:Localize meta:resourcekey="lbl_menu_poligono" runat="server">Poligono</asp:Localize></span></a></li>
                            <li><a href="#" onclick="interfaccia.switchDrawingMode(google.maps.drawing.OverlayType.MARKER); return false;"><span id="StrumentoDisegnoCorrentePoint" class="fa fa-map-pin fa-3x"></span><span id="lbl_menu_punto"><asp:Localize meta:resourcekey="lbl_menu_punto" runat="server">Punto</asp:Localize></span></a></li>
                            <li><a href="#" onclick="preparaXSalvataggioPoligoni(); return false;"><span id="StrumentoDisegnoCorrenteSalva" class="fa fa-floppy-o fa-3x"></span><span id="lbl_menu_salva"><asp:Localize meta:resourcekey="lbl_menu_salva" runat="server">Salva</asp:Localize></span></a></li>
                            <li><a href="#" onclick="deleteSelectedShape(); return false;"><span id="StrumentoDisegnoCorrenteElimina" class="fa fa-trash-o fa-3x"></span><span id="lbl_menu_elimina"><asp:Localize meta:resourcekey="lbl_menu_elimina" runat="server">Elimina</asp:Localize></span></a></li>
                        </ul>
                    </li>
                </ul>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu3">
                <!-- visite -->
                <div class="img_tool" id="nuova_visitaHeader">
                    <span class="fa fa-calendar fa-2x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu3">
                <!-- imposta impresa da selezione -->
                <div class="img_tool" id="ImpostaImpresaDaSelezioneHeader">
                    <span class="fa fa-arrows-h fa-2x"></span>
                </div>
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menu3">
                <!-- mantengo il selettore pagine sempre nella stessa posizione-->
            </div>

            <div class="col-xs-2 hidden-lg hidden-md hidden-sm A-Gis-Tool-xs menuSel">
                <!-- passaggio attraverso le varie pagine di menu -->
                <div class="img_tool" id="menuHeaderSel">
                    <!-- style="margin-left:-13px">-->
                    <span id="menuSel1" class="menuSelTxt">1</span><span>.</span>
                    <span id="menuSel2" class="menuSelTxt">2</span><span>.</span>
                    <span id="menuSel3" class="menuSelTxt">3</span>
                </div>
            </div>

            <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-gis-navbar-collapse-1" aria-expanded="false" style="margin-right:7px; background-color:transparent;">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
        </div>

        <!-- Collect the nav links, forms, and other content for toggling -->
        <div class="collapse navbar-collapse" id="bs-gis-navbar-collapse-1" style="border-color:transparent;padding-left: 0px !important;padding-right: 0px !important;">

            <!-- strumenti in primo piano -->
            <div class="nav navbar-nav gis-navbar" style="width:100%;">

                <!--posizioneGPS-->
                <div class=" hidden-xs hidden-readonly">
                    <div class="k-button StrumentoGPS" id="StrumentoGPS">
                        <span class="fa fa-location-arrow"></span>
                    </div>
                </div>

                <!-- info -->
                <div class="hidden-xs">
                    <div class="k-button" id="info_appezzamento" title="Informazioni sul poligono">
                        <span class="fa fa-info-circle"></span>
                    </div>
                </div>

                <!-- info -->
                <div class="hidden-xs">
                    <div class="k-button" id="apriMiniWindowEditToolbar" title="Apri strumenti di gestione elementi grafici">
                        <span class="fa fa-pencil-square-o"></span>
                    </div>
                </div>

                <!-- strumenti in secondo piano-->
                <div class="hidden-readonly">
                    <!--<li class="gis-tool dropdown hidden-readonly">-->
                    <!--<div class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Menu<span class="caret"></span></div>-->
                    <div class="k-button" id="gisMenuStrumenti">
                        <span class="fa fa-bars"></span>
                        <span class="caret"></span>
                    </div>
                    <ul id="gisMenuStrumenti-menu">
                        <li id="li_print">
                            <a href="#">
                                <!-- stampa -->
                                <div class="img_tool" id="print">
                                    <img class="gis-menuImg" src="replace_print" title="Stampa Immagine"  alt="" /><span id="lbl_print"><asp:Localize meta:resourcekey="lbl_print" runat="server">Stampa Immagine</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_select_poligono">
                            <a href="#">
                                <!-- select -->
                                <div class="img_tool" id="select_poligono">
                                    <img class="gis-menuImg" src="replace_select" title="Ripulisci Selezione" alt="" /><span id="lbl_select_poligono"><asp:Localize meta:resourcekey="lbl_select_poligono" runat="server">Ripulisci Selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_selectPerSpecie">
                            <a href="#">
                                <div class="img_tool" id="selectPerSpecie">
                                    <img class="gis-menuImg" src="replace_selectPerSpecie" title="Seleziona per impianti omogenei" alt="" /><span id="lbl_SelectPerSpecie"><asp:Localize meta:resourcekey="lbl_SelectPerSpecie" runat="server">Seleziona per impianti omogenei</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_righello-button">
                            <a href="#" onclick="clickRighello(); return false;">
                                <!-- righello -->
                                <div class="img_tool" id="righello-button">
                                    <img class="gis-menuImg" src="replace_ruler" title="Misura distanze" alt="" /><span id="lbl_righello_button"><asp:Localize meta:resourcekey="lbl_righello_button" runat="server">Misura Distanze</asp:Localize></span>
                                    <input type="checkbox" id="righello" style="display: none;" />
                                </div>
                            </a>
                        </li>
                        <li id="li_import-button">
                            <a href="#">
                                <!-- importa -->
                                <div class="img_tool" id="import-button">
                                    <img class="gis-menuImg" src="replace_import" title="Importa poligoni" alt="" /><span id="lbl_import_button"><asp:Localize meta:resourcekey="lbl_import_button" runat="server">Importazione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_export-button">
                            <a href="#">
                                <div class="img_tool" id="export-button">
                                    <img class="gis-menuImg" src="replace_export" title="Esporta dati" alt="" /><span id="lbl_export_button"><asp:Localize meta:resourcekey="lbl_export_button" runat="server">Esportazione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_nuova_ricetta">
                            <a href="#">
                                <div class="img_tool" id="nuova_ricetta">
                                    <img class="gis-menuImg" src="replace_img_agenda" title="Crea Ricetta (occorre selezionare prima uno o più impianti colturali, una pianificazione (Testata) oppure un campo)" alt="" />
                                    <span id="lbl_nuova_ricetta"><asp:Localize meta:resourcekey="lbl_nuova_ricetta" runat="server">Crea Ricetta</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ab-button">
                            <a href="#" onclick="clickAB(); return false;">
                                <div class="img_tool" id="ab-button">
                                    <img class="gis-menuImg" src="replace_ab" title="Inserisci A-B direzione movimento" alt="" /><span id="menuLineaAB"><asp:Localize meta:resourcekey="menuLineaAB" runat="server">Inserisci Linea Guida A-B</asp:Localize></span>
                                    <input type="checkbox" id="chkAB" style="display: none;" />
                                    <input type="hidden" id="hiddenA" />
                                    <input type="hidden" id="hiddenB" />
                                    <input type="hidden" id="hiddenCurrentSelectedAB" />
                                </div>
                            </a>
                        </li>
                        <li id="li_PianoRateoVariabile">
                            <a href="#">
                                <div class="img_tool" id="PianoRateoVariabile">
                                    <%--<asp:Literal runat="server" Text="<%$ Resources:AgronicaControlliGIS.mappaBS2.ascx, menuGeneraRateoVar %>" />--%>
                                    <%--<asp:Localize meta:resourcekey="menuGeneraRateoVar" runat="server"></asp:Localize>--%>
                                    <img class="gis-menuImg" src="replace_pfRateo" title="Gestione piano a rateo variabile" alt="" /><span id="menuGeneraRateoVar"><asp:Localize meta:resourcekey="menuGeneraRateoVar" runat="server">Genera un piano a rateo variabile sul poligono</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ripartoCatasto-button">
                            <a href="#">
                                <div class="img_tool" id="ripartoCatasto-button">
                                    <img class="gis-menuImg" src="replace_img_catasto" title="Apri Strumento di Riparto Catasto (impostata eventualmente sugli impianti selezionati)" alt="" /><span id="lbl_ripartoCatasto_button"><asp:Localize meta:resourcekey="lbl_ripartoCatasto_button" runat="server">Apri Strumento di Riparto Catasto</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ritaglio-sfondo">
                            <a href="#">
                                <!-- ritaglio sfondo -->
                                <div class="img_tool" id="ritaglio-sfondo">
                                    <img class="gis-menuImg" src="replace_img_ritaglia_sfondo" title="Ritaglia sfondo" alt="" /><span id="lbl_ritaglio_sfondo"><asp:Localize meta:resourcekey="lbl_ritaglio_sfondo" runat="server">Ritaglia sfondo</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ritaglio-edit-punti-gps-button">
                            <a href="#">
                                <!-- work in progress -->
                                <!-- edit dei punti scomponi/ricomponi -->
                                <div class="img_tool" id="ritaglio-edit-punti-gps-button">
                                    <img class="gis-menuImg" src="replace_img_edit_punti" title="Scomponi e modifica per punti" alt="" /><span id="lbl_ritaglio_edit_punti_gps_button"><asp:Localize meta:resourcekey="lbl_ritaglio_edit_punti_gps_button" runat="server">Scomponi e modifica per punti</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_CopiaOggettoCopia">
                            <a href="#">
                                <div class="img_tool" id="CopiaOggettoCopia">
                                    <img class="gis-menuImg" src="replace_img_CopiaOggettoCopia" title="Copia oggetto grafico" alt="" /><span id="lbl_CopiaOggettoCopia"><asp:Localize meta:resourcekey="lbl_CopiaOggettoCopia" runat="server">Copia oggetto grafico</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        
                        <li id="li_CopiaOggettoIncolla">
                            <a href="#">
                                <div class="img_tool" id="CopiaOggettoIncolla">
                                    <img class="gis-menuImg" src="replace_img_CopiaOggettoIncolla" title="Incolla oggetto grafico" alt="" /><span id="lbl_CopiaOggettoIncolla"><asp:Localize meta:resourcekey="lbl_CopiaOggettoIncolla" runat="server">Incolla oggetto grafico</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_GeneraPlanning">
                            <a href="#">
                                <div class="img_tool" id="GeneraPlanning">
                                    <img class="gis-menuImg" src="replace_img_GeneraPlanning" title="Genera una nuova pianificazione da selezione" alt="" /><span id="lbl_GeneraPlanning"><asp:Localize meta:resourcekey="lbl_GeneraPlanning" runat="server">Genera una nuova pianificazione da selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_BufferZone">
                            <a href="#">
                                <div class="img_tool" id="BufferZone">
                                    <img class="gis-menuImg" src="replace_BufferZone" title="Strumento di gestione fascia di rispetto da selezione" alt="" /><span id="lbl_BufferZone"><asp:Localize meta:resourcekey="lbl_BufferZone" runat="server">Strumento di gestione fascia di rispetto da selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_AnalisiMeteo">
                            <a href="#">
                                <div class="img_tool" id="AnalisiMeteo">
                                    <img class="gis-menuImg" src="replace_AnalisiMeteo" title="Strumento di gestione analisi dati meteo da selezione" alt="" /><span id="lbl_AnalisiMeteo"><asp:Localize meta:resourcekey="lbl_AnalisiMeteo" runat="server">Strumento di gestione analisi dati meteo da selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_AnalisiModelli">
                            <a href="#">
                                <div class="img_tool" id="AnalisiModelli">
                                    <img class="gis-menuImg" src="replace_AnalisiModelli" title="Strumento di gestione analisi DSS da selezione" alt="" /><span id="lbl_AnalisiModelli"><asp:Localize meta:resourcekey="lbl_AnalisiModelli" runat="server">Strumento di gestione analisi DSS da selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>                        
                        <li id="li_AnalisiRilievi">
                            <a href="#">
                                <div class="img_tool" id="AnalisiRilievi">
                                    <img class="gis-menuImg" src="replace_AnalisiRilievi" title="Analisi dati schede rilievi" alt="" /><span id="lbl_AnalisiRilievi"><asp:Localize meta:resourcekey="lbl_AnalisiRilievi" runat="server">Analisi dati schede rilievi</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_AnalisiDatiReteAcqua">
                            <a href="#">
                                <div class="img_tool" id="AnalisiDatiReteAcqua">
                                    <img class="gis-menuImg" src="replace_AnalisiDatiReteAcqua" title="Analisi Dati Rete Acqua" alt="" /><span id="lbl_AnalisiDatiReteAcqua"><asp:Localize meta:resourcekey="lbl_AnalisiDatiReteAcqua" runat="server">Analisi Dati Rete Acqua</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ImpostaImpresaDaSelezione">
                            <a href="#">
                                <div class="img_tool" id="ImpostaImpresaDaSelezione">
                                    <span class="fa fa-arrows-h"></span>
                                    <span id="lbl_ImpostaImpresaDaSelezione"><asp:Localize meta:resourcekey="lbl_ImpostaImpresaDaSelezione" runat="server">Imposta Impresa Da Selezione</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                        <li id="li_ApriAnalisi">
                            <a href="#">
                                <div class="img_tool" id="ApriAnalisi">
                                    <img class="gis-menuImg" src="replace_Ingredienti" title="Analisi" alt="" /><span id="lbl_ApriAnalisi"><asp:Localize meta:resourcekey="lbl_ApriAnalisi" runat="server">Apri Analisi</asp:Localize></span>
                                </div>
                            </a>
                        </li>                        
                        <li id="li_sr">
                            <a href="#">
                                <div class="img_tool" id="DialogSr">
                                    <img class="gis-menuImg" src="replace_sr" title="Definizione sistemi di riferimento e trasformazioni" alt="" /><span id="lbl_sr"><asp:Localize meta:resourcekey="lbl_sr" runat="server">Definizione sistemi di riferimento e trasformazioni</asp:Localize></span>
                                </div>
                            </a>
                        </li>
                    </ul>
                </div>


                <div class="hidden-xs">
                    <div id="sitebar_coordinate" style="display: grid;grid-template-columns: auto auto auto;">
                        <div id="formatoCoord" class="k-button" style="border-top-right-radius:0px; border-bottom-right-radius:0px;" title="Cambia formato coordinate"><!-- i18n -->
                            <span class="fa fa-exchange"></span>
                        </div>
                        <div class="masked-container" style="align-self:center;">
                            <input type="text" id="find_lat" style="width:12em; border-radius:0px; border-left:none;" />
                            <input type="text" id="find_lng" style="width:12em; border-radius:0px; border-left:none;" />
                        </div>
                        <div id="findCoord" class="k-button" style="border-top-left-radius:0px; border-bottom-left-radius:0px; border-left:none;" title="Cerca coordinate"> <!-- i18n -->
                            <span class="fa fa-search"></span>
                        </div>
                    </div>
                </div>

                <div class="hidden-xs">
                    <div id="indirizzoTB" class="k-textbox k-space-right" style="padding-bottom:1px; background-color: #fff; width: 15em;">
                        <input type="text" id="address" value="" class="valid" placeholder="Indirizzo" aria-invalid="false">
                        <!--<a class="k-icon k-i-search findAddress" title="Cerca indirizzo"></a>-->
                        <span class="fa fa-search findAddress" title="Cerca indirizzo" style="position: absolute; right: 0px;top: 50%; transform: translateY(-50%); cursor: pointer;"></span>
                    </div>
                </div>

                <div class="">
                    <div id="gisToolMenu" class="k-button">
                        <span class="fa fa-map-marker"></span>
                        <span class="caret"></span>
                        <ul id="gisToolMenu-menu">
                            <li class="hidden-xs"><a href="#" onclick="IndirizzoLatLong(); return false;" id="anchor_IndirizzoLatLong"><asp:Localize meta:resourcekey="anchor_IndirizzoLatLong" runat="server">Indirizzo da coordinate</asp:Localize></a></li>
                            <li class="hidden-xs"><a href="#" onclick="CoordDaSelezione(); return false;" id="anchor_CoordDaSelezione"><asp:Localize meta:resourcekey="anchor_CoordDaSelezione" runat="server">Imposta coordinate da impianto</asp:Localize></a></li>
                            <li><a href="#" onclick="ApriGoogleMaps(); return false;" id="anchor_ApriGoogleMaps"><asp:Localize meta:resourcekey="anchor_ApriGoogleMaps" runat="server">Naviga verso</asp:Localize></a></li>
                            <li><a href="#" onclick="VisualizzazioneTotaleImposta(); return false;" id="anchor_VisualizzazioneTotaleImposta"><asp:Localize meta:resourcekey="anchor_VisualizzazioneTotaleImposta" runat="server">Carica dintorni</asp:Localize></a></li>
                        </ul>
                    </div>
                </div>
                <!--<li class="gis-tool hidden-xs dropdown">-->
                <!--<div class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false"><span class="fa fa-map-marker"></span> <span class="caret"></span></div>-->
                <!--<ul class="dropdown-menu">-->
                <!--<li><a href="#" onclick="ApriGoogleMaps(); return false;">Naviga Verso</a></li>-->
                <!--       <li><a href="#" onclick="ricercaIndirizzo();return false;">Cerca questo Indirizzo</a></li>-->
                <!--<li><a href="#" onclick="IndirizzoLatLong(); return false;">Indirizzo Da Coordinate</a></li>-->
                <!--      <li><a href="#" onclick="nascondiAddress(); return false;">Nascondi PIN</a></li>-->
                <!--<li><a href="#" onclick="CoordDaSelezione(); AggiornaLatLon(); return false;">Imposta Coordinate da impianto</a></li>-->
                <!--      <li><a href="#" onclick="ricercaCoordinate(); return false;">Cerca queste coordinate</a></li>-->
                <!--      <li><a href="#" onclick="CoordFromGPS(); return false;">Posizione GPS</a></li>-->
                <!--<li><a href="#" onclick="VisualizzazioneTotaleImposta(); return false;">Carica Dintorni</a></li>-->
                <!--</ul>-->
                <!--</li>-->

                <div style="display:grid; grid-auto-flow:column; grid-column-gap:2px; float:right;">

                    <div>
                        <div class="k-button" onclick="LayerAttivaDisattiva(); return false;">
                            <span><img class="gis-menuImg" src="replace_LayerDoveDisegno" id="LayerDoveDisegno" style="float: left" alt="" /> </span>
                            <span id="navBarLayerAttivoDescrizione"><asp:Localize meta:resourcekey="navBarLayerAttivoDescrizione" runat="server">Layer</asp:Localize></span>
                        </div>
                    </div>

                    <div class="hidden-readonly">
                        <div class="k-button" onclick="TematizzazioneAttivaDisattiva(); return false;">
                            <span><img class="gis-menuImg" src="replace_gestisci_layer" id="TematizzazioneDoveDisegno" style="float: left" alt="" /> </span>
                            <span id="navBarTematizzazioneAttivaDescrizione">Temi</span>
                        </div>
                    </div>

                    <div class="hidden-readonly" id="satPFToolsNavBar">
                        <div class="k-button" onclick="SatPFAttivaDisattiva(); return false;">
                            <span class="fa fa-globe"></span>
                            <span id="navBarSatAttivoDescrizione"><asp:Localize meta:resourcekey="navBarSatAttivoDescrizione" runat="server">Sat</asp:Localize></span>
                        </div>
                    </div>

                    <div class="hidden-readonly" id="wmsToolsNavBar">
                        <div class="k-button" onclick="WmsAttivaDisattiva(); return false;">
                            <span class="fa fa-table"></span>
                            <span id="navBarWmsAttivoDescrizione"><asp:Localize meta:resourcekey="navBarWmsAttivoDescrizione" runat="server">Wms</asp:Localize></span>
                        </div>
                    </div>

                </div>

            </div>

        </div>

    </nav>


    <!--miniWindowEditToolbar-->
    <div id="miniWindowEditToolbar" class="hidden-xs">

        <!-- nuovo -->
        <div class="hidden-xs hidden-readonly" id="disenga_poligono_dropdown">
            <div class="k-button" id="disenga_poligono" style="display:none">
                <!--<div id="img_poligono">-->
                <span class="fa fa-pencil-square-o"></span>
                <span class="caret"></span>
                <!--</div>-->
                <!--<div id="img_multipoint" style="display: none">
            <img class="menuImg" src="replace_multipoint" title="Disegna un nuovo insieme di punti" alt="Disegna un nuovo insieme di punti" alt="" />
            </div>-->

                <input type="hidden" id="hiddenMultipoint" />
                <input type="hidden" id="hiddenMultipointModifica" />
                <input type="checkbox" id="chkMP" style="display: none;" />
            </div>




        </div>

        <div style="display: grid;grid-template-columns: auto;">

            <div id="selimg_EraseSelection" class="k-button">
                <span class="fa fa-eraser fa-lg"></span>                
            </div>

            <div id="selimg_poligono" class="k-button">
                <span class="fa fa-pencil-square-o fa-lg"></span>
                <!--<span>Disegna un nuovo poligono</span>-->
            </div>

            <div id="selimg_multipoint" class="k-button">
                <span class="fa fa-map-marker fa-lg"></span>
                <!--<span>Disegna un nuovo insieme di punti</span>-->
            </div>
            <!-- salva -->
            <div id="save-button" class="hidden-xs hidden-readonly k-button">
                <div id="btn-save-button" title="Salva le modifiche"> <!-- i18n -->
                    <span class="fa fa-floppy-o fa-lg"></span>
                </div>
            </div>

            <div id="save-plus" class="hidden-xs hidden-readonly k-button">
                <div id="btn-plus-button" title="Nuovo impianto su appezzamento"> <!-- i18n -->
                    <span class="fa fa-plus fa-lg"></span>
                </div>
            </div>

            <div id="copia-oggettoCopia" class="hidden-xs hidden-readonly k-button">
                <div id="btn-copia-oggettoCopia" title="Copia oggetto grafico"> <!-- i18n -->
                    <span class="fa fa-files-o fa-lg"></span>
                </div>
            </div>

            
            <div id="copia-oggettoIncolla" class="hidden-xs hidden-readonly k-button">
                <div id="btn-copia-oggettoIncolla" title="Incolla oggetto grafico"> <!-- i18n -->
                    <span class="fa fa-paste fa-lg"></span>
                </div>
            </div>

            <!-- delete -->
            <div id="delete-button" class="hidden-xs hidden-readonly k-button">
                <div id="btn-delete-button" title="Elimina un poligono"> <!-- i18n -->
                    <span class="fa fa-trash-o fa-lg"></span>
                </div>
            </div>

            <div id="caricaDatiPFXML" class="hidden-xs hidden-readonly k-button">
                <div id="btn-caricaDatiPFXML" title="Mostra di dati di precision Farming"> <!-- i18n -->
                    <span class="fa fa-share-square-o fa-lg"></span>
                </div>
            </div>
        </div>
    </div>
    <!--fine miniWindowEditToolbar-->
    <!-- mappa -->
    <div id="mapBS">
        <div id="mapDockLeft"></div>
        <div id="map" style="height:100%;"></div>
        <div id="mapDockRight"></div>
    </div>

    <!-- layers -->
    <div id="scala_colori_contenitore">

        <div class="row">
            <div class="col-lg-12">
                <div class="input-group">
                    <label class="input-group-addon" id="lbl_tema_scala_colori"><asp:Localize meta:resourcekey="lbl_tema_scala_colori" runat="server">Seleziona un tema:</asp:Localize></label>
                    <select class="form-control k-content" id="cmbViste" onchange="cmbViste_change()"></select>
                    <span class="input-group-btn" id="gestisci_dettagli_layer" title="Gestisci Dettagli Tematizzazione"> <!-- i18n -->
                        <a class="form-control k-content btn">
                            <span class="fa fa-bars"></span>
                        </a>
                    </span>
                </div>
            </div>
        </div>


        <!--<div style='height: 40px; color: white; background-color: rgba(63,72,204,0.8); text-align: center'>
            <div class="col-lg-12">

                <div class="col-lg-2">
                    Seleziona una vista:
                </div>

                <div id="cmbScalaColori" class="col-lg-6">
                    <select class="form-control" id="cmbViste_" onchange="cmbViste_change()"></select>
                </div>

                <div class="col-lg-1">
                    <img src="replace_gestisci_layer" id="gestisci_dettagli_layer_"
                         style="float: left; width: 20px; cursor: pointer;" title="Gestisci Dettagli Tematizzazione" />
                </div>


            </div>
        </div>-->
        <!--<div id="scala_colori_descrizione" class="col-lg-12">
        </div>-->
        <!--<div class="col-lg-12">
            <div id="scala_colori" style="position: relative; left:-14px"></div>
        </div>-->

        <div class="row">
            <div class="col-lg-12">
                <div id="scala_colori"></div>
            </div>
        </div>

    </div>


    <!--strumenti satellitari e precision farming-->
    <div id="wmsTools">

        <div style="display:grid; grid-template-columns: 1fr auto; grid-column-gap: 10px;">

            <div>
                <input id="WmsDdlSensoreElaborazione" style="width:-webkit-fill-available;" />
            </div>

            <div class="input-group">
                <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_info_mappa"><asp:Localize meta:resourcekey="lbl_info_mappa" runat="server">Info su click in mappa</asp:Localize></span>
                <div class="form-control k-content" style="width:auto;">
                    <input type="checkbox" id="wmsChkAggiornaClickMappa" class="k-checkbox" />
                    <label class="k-checkbox-label" for="wmsChkAggiornaClickMappa" style="padding-left: 12.6px;"></label>
                </div>
            </div>

        </div>

        <div class="input-group">
            <span class="input-group-addon" id="wms_lbl_trasparenza"><asp:Localize meta:resourcekey="wms_lbl_trasparenza" runat="server">Trasparenza:</asp:Localize></span>
            <div class="form-control k-content" style="padding:2px 25px;">
                <input id="wmsSliderTrasparenza" />
            </div>
        </div>

    </div>

    <!--strumenti satellitari e precision farming-->
    <div id="satPfTools">

        <div id="satTabStrip" style="padding-bottom:10px">
            <ul>
                <li class="k-state-active k-active" id="satTabStrip_Calendari">
                    <asp:Localize meta:resourcekey="satTabStrip_Calendari" runat="server">Calendario</asp:Localize>
                </li>
                <li id="satTabStrip_Grafici">
                    <asp:Localize meta:resourcekey="satTabStrip_Grafici" runat="server">Grafici</asp:Localize>
                </li>
            </ul>

            <!--TAB CALENDARIO-->
            <div>
                <div style="display:flex;">
                    <div id="pf_data" style="flex-shrink:0;"></div>
                    <div id="SatElaborazioneMsg" style="flex-grow:1; border:1px solid #ccc; margin-left:10px;overflow-y:auto;overflow-x:hidden;"></div>
                </div>
            </div>

            <!--TAB GRAFICI-->
            <div>
                <div id="satKendoMicroChart"></div>
            </div>

        </div>


        <div class="row">

            <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
                <input id="PfDdlSensoreElaborazione" style="width:-webkit-fill-available;" />
            </div>

            <div class="col-lg-6 col-md-12 col-sm-12 col-xs-12">
                <div class="input-group">
                    <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_info_mappa_row"><asp:Localize meta:resourcekey="lbl_info_mappa_row" runat="server">Info Su click in mappa</asp:Localize></span>
                    <div class="form-control k-content" style="width:auto;">
                        <input type="checkbox" id="satChkAggiornaClickMappa" class="k-checkbox" />
                        <label class="k-checkbox-label" for="satChkAggiornaClickMappa" style="padding-left: 12.6px;"></label>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="input-group">
                    <span class="input-group-addon" id="sat_lbl_trasparenza" style="width: 100%; text-align: left !important;"><asp:Localize meta:resourcekey="sat_lbl_trasparenza" runat="server">Trasparenza:</asp:Localize></span>
                    <div class="form-control k-content" style="width:auto; padding:2px 25px;">
                        <input id="satSliderTrasparenza" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="padding-top: 5px; padding-bottom: 5px;">
                <div class="btn btn-info" id="ElaboraMappaDettagliataSuSelezione" style="width: -webkit-fill-available;">
                    <span class="fa fa-globe"></span>
                    <span id="lbl_ElaboraMappaDettagliataSuSelezione"><asp:Localize meta:resourcekey="lbl_ElaboraMappaDettagliataSuSelezione" runat="server">Elabora Mappa Dettagliata su Poligoni Selezionati</asp:Localize></span>
                </div>
            </div>
        </div>

    </div>

    <!-- -->
    <div id="FinestraTemporale_Tool">

        <div id="layerFinestraTemporale" style="margin-top: 7px">
            <div class="">
                <!--<span class="k-link agroAccordionTitolo"><span class="fa fa-calendar"></span><span>Limita visualizzazione</span></span>-->

                <div id="DateContainer" class="row" style="margin-top: 5px">

                    <div id="divDateVisualizzaAvanzata_RicercheComuni" class="DateVisualizzaSemplice" style="margin-bottom: 7px;">
                        <select id="ddlDateVisualizzaAvanzata_RicercheComuni" style="width: 100%">
                            <option value="0" id="opt_DateVisualizzaDefault"><asp:Localize meta:resourcekey="opt_DateVisualizzaDefault" runat="server">Seleziona una ricerca da qui o scegli le date</asp:Localize></option>
                            <option value="1" id="opt_DateVisualizzaAdOggi"><asp:Localize meta:resourcekey="opt_DateVisualizzaAdOggi" runat="server">Validi ad oggi</asp:Localize></option>
                            <option value="2" id="opt_DateVisualizzaAnnata"><asp:Localize meta:resourcekey="opt_DateVisualizzaAnnata" runat="server">Validi su annata agraria in corso</asp:Localize></option>
                            <option value="3" id="opt_DateVisualizzaFiltri"><asp:Localize meta:resourcekey="opt_DateVisualizzaFiltri" runat="server">Imposta i filtri iniziali</asp:Localize></option>
                        </select>
                    </div>

                    <div id="DateContainer_inizio">

                        <div class="input-group DateVisualizzaAvanzata" id="rndBtnLimiti_start" style="width: -webkit-fill-available;">
                            <input type="radio" name="limite_inferiore" id="limite_inferiore_sucessivo" value="sucessivo" style="margin-left: 6px" />
                            <label for="limite_inferiore_sucessivo" id="lbl_limite_inferiore_sucessivo"><asp:Localize meta:resourcekey="lbl_limite_inferiore_sucessivo" runat="server">Successivo al</asp:Localize></label>
                            <input type="radio" name="limite_inferiore" id="limite_inferiore_precedente" value="precedente" style="margin-left: 16px" checked />
                            <label for="limite_inferiore_precedente" id="lbl_limite_inferiore_precedente"><asp:Localize meta:resourcekey="lbl_limite_inferiore_precedente" runat="server">Precedente al</asp:Localize></label>
                        </div>

                        <div class="input-group">
                            <label style="text-align: left !important;" class="input-group-addon" for="limita_data_da" id="lbl_limita_data_da"><asp:Localize meta:resourcekey="lbl_limita_data_da" runat="server">Data inizio:</asp:Localize></label>
                            <input type="text" id="limita_data_da" class="kendoDatePicker" style="width: 100%;" />
                        </div>
                    </div>
                    <div id="DateContainer_fine">

                        <div class="input-group DateVisualizzaAvanzata" id="rndBtnLimiti_end" style="width: -webkit-fill-available;">
                            <input type="radio" name="limite_superiore" id="limite_superiore_sucessivo" value="sucessivo" style="margin-left: 6px" checked />
                            <label for="limite_superiore_sucessivo" id="lbl_limite_superiore_sucessivo"><asp:Localize meta:resourcekey="lbl_limite_superiore_sucessivo" runat="server">Successivo al</asp:Localize></label>
                            <input type="radio" name="limite_superiore" id="limite_superiore_precedente" value="precedente" style="margin-left: 16px" />
                            <label for="limite_superiore_precedente" id="lbl_limite_superiore_precedente"><asp:Localize meta:resourcekey="lbl_limite_superiore_precedente" runat="server">Precedente al</asp:Localize></label>
                        </div>

                        <div class="input-group">
                            <label style="text-align: left !important;" class="input-group-addon" for="limita_data_a" id="lbl_limita_data_a"><asp:Localize meta:resourcekey="lbl_limita_data_a" runat="server">Data fine:</asp:Localize></label>
                            <input type="text" id="limita_data_a" class="kendoDatePicker" style="width: 100%;" />
                        </div>
                    </div>
                </div>




                <div class="row">
                    <div style="display: none">
                        <div class="btn btn-info" id="aggiorna_date" style="width: -webkit-fill-available;">
                            <span class="fa fa-refresh"></span>
                            <span id="lbl_aggiorna_date"><asp:Localize meta:resourcekey="lbl_aggiorna_date" runat="server">Aggiorna</asp:Localize></span>
                        </div>
                    </div>
                    <div style="margin-top:2px">
                        <div class="btn btn-info" id="visualizza_Tutto" style="width: -webkit-fill-available;">
                            <span class="fa fa-globe"></span>
                            <span id="lbl_visualizza_Tutto"><asp:Localize meta:resourcekey="lbl_visualizza_Tutto" runat="server">Mostra Tutto</asp:Localize></span>
                        </div>
                    </div>
                    <div style="margin-top:2px">
                        <div class="btn btn-info" id="visualizza_avanzata" style="width: -webkit-fill-available;">
                            <span class="fa fa-search"></span>
                            <span id="lbl_visualizza_avanzata"><asp:Localize meta:resourcekey="lbl_visualizza_avanzata" runat="server">Ricerca Avanzata</asp:Localize></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- TOOL -->
    <div id="contenitore_tool">

        <div id="pulsantieraEsterna_tool" style="padding-left: 10px; padding-right: 10px; display: none;">
            <!--<img src="replace_piu" id="espandi_contenuti" style="display:none; cursor: pointer; float: left;
                width: 32px; height: 32px;" alt="piu" title="replace_piu_meno" />-->

            <span id="testoLayerDoveDisegno" class="layerDoveDisegno"><asp:Localize meta:resourcekey="testoLayerDoveDisegno" runat="server">Nessun layer selezionato.</asp:Localize></span>

            <!--<div id="numero_identita"></div>-->
        </div>

        <div id="sidebar">
            <div>

                <div id="layerGPSIcona">
                    

                    <div>
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_mostra_GPS"><asp:Localize meta:resourcekey="lbl_mostra_GPS" runat="server">Mostra icona GPS</asp:Localize></span>
                            <div class="form-control k-content" style="width:auto;">
                                <input type="checkbox" id="chk_mostra_GPS" class="k-checkbox" />
                                <label class="k-checkbox-label" for="chk_mostra_GPS" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_mostra_markerClusters">
                                <asp:Localize meta:resourcekey="lbl_mostra_markerClusters" runat="server">Raggruppa Descrizioni</asp:Localize></span>

                            <div class="form-control k-content" style="width: auto;">
                                <input type="checkbox" id="chk_mostra_markerClusters" checked="checked" class="k-checkbox" />
                                <label class="k-checkbox-label" for="chk_mostra_markerClusters" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div class="input-group">
                            <span class="input-group-addon" style="width: 100%; text-align: left !important;" id="lbl_mostra_descrizioneImpianto"><asp:Localize meta:resourcekey="lbl_mostra_descrizioneImpianto" runat="server">Mostra Descrizione Sintetica</asp:Localize></span>
                            <div class="form-control k-content" style="width:auto;">
                                <input type="checkbox" id="chk_mostra_descrizioneImpianto" class="k-checkbox" checked="checked" />
                                <label class="k-checkbox-label" for="chk_mostra_descrizioneImpianto" style="padding-left: 12.6px;"></label>
                            </div>
                        </div>
                    </div>
                </div>


                <div id="layerTipologie">

                    <div>
                        <div style="display: grid; grid-template-columns: 1fr auto auto auto;">

                            replace_x_tipologia_layer

                            <div class="k-button" id="gestisci_layer_principale" title="Gestione colori layer" style="border-radius: 0px; padding-left: 6px; padding-right: 6px;"> <!-- i18n -->
                                <span class="fa fa-bars"></span>
                            </div>
                            <div class="k-button" id="LayerSelezionaTutto" title="Seleziona tutto" style="border-radius: 0px; padding-left: 6px; padding-right: 6px;"> <!-- i18n -->
                                <span class="fa fa-check"></span>
                            </div>
                            <div class="k-button" id="LayerDeSelezionaTutto" title="Deseleziona tutto" style="border-top-left-radius: 0px; border-bottom-left-radius: 0px; padding-left: 6px; padding-right: 6px;"> <!-- i18n -->
                                <span class="fa fa-times"></span>
                            </div>
                        </div>
                    </div>

                    <div id="check-layer" style="overflow-x: hidden;">
                    </div>

                </div>

            </div>
        </div>
    </div>

    <div id="lblMessaggiFiltri" style="color:red; font-size:13px; font-weight:bold; background-color: rgba(255, 255, 255, 0.8)" class=""></div>


    <div id="mapInfo" style="width: 0px; height: 0px; display: none">
        <input type="hidden" id="ChiaveAlberoMultiSelezione" />
    </div>

    <script>
        $(document).ready(function () {


            //$('#visualizza_Tutto').button();

            $(document).on("click", "#visualizza_avanzata", function () {
                let h = "";
                if ($("#rndBtnLimiti_start").is(":visible")) {
                    h = '<span class="fa fa-search"></span><span>Ricerca Avanzata</span>'; // i8n
                    $(".DateVisualizzaAvanzata").hide();
                } else {
                    h = '<span class="fa fa-search"></span><span>Ricerca Semplice</span>'; // i8n
                    $(".DateVisualizzaAvanzata").show();
                }

                $("#visualizza_avanzata").html(h);
            });

            $(document).on("click", "#visualizza_Tutto", function () {
                $('#limite_inferiore_successivo').prop("checked", "");
                $('#limita_data_da').val("");

                $('#limite_superiore_successivo').prop("checked", "");
                $('#limita_data_a').val("");

                $('#limite_inferiore_precedente').prop("checked", "checked");
                $('#limite_inferiore_precedente').checked = true;

                $('#limite_superiore_sucessivo').prop("checked", "checked");
                $('#limite_superiore_sucessivo').checked = true;
                //$('#aggiorna_date').click();
            });

        });
    </script>


    <!-- dialog a comparsa -->
    <!-- Dialog Nuovo Impianto -->
    <div id="pop_up_impianto" style="padding:0px;" class="load-hidden">

        <input type="hidden" id="hiddenPunti_Nuovo" />

        <div id="contenutoImpianto" style="/*min-height: 400px; max-height:400px; overflow-y: auto;*/ padding:20px;">

            <div id="pop_up_impianto_layer" class="row">

                <div id="layerContainer">
                </div>

                <div class="col-lg-1 col-md-1 col-sm-1 col-xs-1" id="pop_up_impianto_icolayer">
                    <img id="imgpop_up_impianto_icolayer" src="" alt="" />
                </div>


                <div class="col-lg-2 col-md-2 col-sm-2 col-xs-9" style="margin-top: 7px" id="">
                    <div class="form-group">
                        <div class="input-group">
                            <span style="font-weight: bold" id="pop_up_impianto_nomelayer">
                            </span>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-5 col-sm-5 col-xs-12" id="pop_up_impiantoxDescr">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" for="txtElementoGrafico_Des" id="lbl_ElementoGrafico_Des">
                                <asp:Localize meta:resourcekey="lbl_ElementoGrafico_Des" runat="server">Descrizione aggiuntiva:</asp:Localize>
                            </label>
                            <input class="form-control" type="text" id="txtElementoGrafico_Des" />
                        </div>
                    </div>
                </div>


                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12" id="pop_up_impiantoxGenerazionePoligoni">
                    <div class="form-group">
                        <div class="input-group">
                            <label class="input-group-addon" for="cmbElementoGrafico_GenerazionePoligoni" id="lbl_cmbElementoGrafico_GenerazionePoligoni">
                                <asp:Localize meta:resourcekey="lbl_cmbElementoGrafico_GenerazionePoligoni" runat="server">Genera sui layer:</asp:Localize>
                            </label>
                            <select class="form-control" id="cmbElementoGrafico_GenerazionePoligoni">
                                <!--<option id="cmbElementoGrafico_GenerazionePoligoni_1" value="1">Appezzamento</option>
                                <option id="cmbElementoGrafico_GenerazionePoligoni_2" value="2">Impianto</option>
                                <option id="cmbElementoGrafico_GenerazionePoligoni_3" value="3">Impianto e Appezzamento</option>-->
                            </select>
                        </div>
                    </div>
                </div>

            </div>


            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <hr style="border: solid 1px #ff0000" />
                </div>

            </div>

            <div class="form-horizontal">
                <ul id="panelBarDatiAzienda">

                    <li class="k-state-active k-active agroAccordion" id="panelBarDatiAzienda_Testata">
                        <span class="k-link agroAccordionTitolo"><span class="fa fa-list"></span><span id="panelBarDatiAziendaTestata_Titolo"><asp:Localize meta:resourcekey="panelBarDatiAziendaTestata_Titolo" runat="server">Dati Aziendali, di Appezzamento e Superfici</asp:Localize></span></span>

                        <div class="agroContainer" id="containerDatiAzienda_Testata" style="margin-top: 12px; margin-bottom: 12px">
                            <!-- Dati azienda -->
                            <div class="row">

                                <div id="div_pop_up_impianto_azienda" class="col-lg-4 col-md-4 col-sm-4 col-xs-12">

                                    <div class="form-group">
                                        <div class="input-group">
                                            <label id="lbl_pop_up_impianto_azienda" class="input-group-addon" for="pop_up_impianto_azienda">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_impianto_azienda" runat="server">Azienda</asp:Localize>
                                            </label>
                                            <select class="form-control" id="pop_up_impianto_azienda" name="pop_up_impianto_azienda"></select>
                                            <span class="input-group-btn" id="PopupNuovaImpresa-Button">
                                                <a class="form-control btn btn-secondary" style="border-bottom-right-radius: 4px; border-top-right-radius: 4px;" title="Nuova Impresa"><span class="fa fa-plus"></span></a> <!-- i18n -->
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div id="div_pop_up_centro" class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_centro" id="lblpop_up_centro">
                                                <asp:Localize meta:resourcekey="lblpop_up_centro" runat="server">Centro Aziendale</asp:Localize>
                                            </label>

                                            <select class="form-control" id="pop_up_centro" name="pop_up_centro"></select>
                                            <span class="input-group-btn" id="PopupNuovoSa-Button">
                                                <a class="form-control btn btn-secondary" style="border-bottom-right-radius: 4px; border-top-right-radius: 4px;" title="Nuovo centro"><span class="fa fa-plus"></span></a> <!-- i18n -->
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div id="div_pop_up_campo" class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_campo" id="lblpop_up_campo">
                                                <asp:Localize meta:resourcekey="lblpop_up_campo" runat="server">Campo</asp:Localize>
                                            </label>

                                            <select class="form-control" id="pop_up_campo" name="pop_up_campo"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="stringaIndirizzo" id="lbl_stringaIndirizzo">
                                                <asp:Localize meta:resourcekey="lbl_stringaIndirizzo" runat="server">Indirizzo</asp:Localize>
                                            </label>
                                            <input class="form-control" type="text" id="stringaIndirizzo" />

                                        </div>
                                    </div>
                                </div>


                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_nome" id="lbl_pop_up_nome">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_nome" runat="server">Nome</asp:Localize>
                                            </label>
                                            <input class="form-control" type="text" id="pop_up_nome" name="pop_up_nome" value="" />

                                        </div>
                                    </div>
                                </div>

                                <!--al momento commentato, decommantare quando si vorrà modificare da GIS questo dato -->
                                <!--<div class="col-lg-3 col-md-5 col-sm-5 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_qdc">
                                                Rif.QDC
                                            </label>
                                            <input class="form-control CodiceAnagrafe" type="text" id="pop_up_qdc" name="pop_up_qdc" data-idCod="1104" value="" />

                                        </div>
                                    </div>
                                </div>-->
                            </div>
                        </div>
                        <!-- Date -->
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-group">
                                    <div class="input-group" id="">
                                        <label class="input-group-addon" for="pop_up_data_inizio" id="lblpop_up_data_inizio">
                                            <asp:Localize meta:resourcekey="lblpop_up_data_inizio" runat="server">Data Inizio:</asp:Localize>
                                        </label>
                                        <input class="form-control kendoDatePicker" type="text" id="pop_up_data_inizio" name="pop_up_data_inizio" value="" />


                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-group">
                                    <div class="input-group" id="">
                                        <label class="input-group-addon" for="pop_up_data_fine" id="lblpop_up_data_fine">
                                            <asp:Localize meta:resourcekey="lblpop_up_data_fine" runat="server">Data Fine:</asp:Localize>
                                        </label>
                                        <input class="form-control kendoDatePicker" type="text" id="pop_up_data_fine" name="pop_up_data_fine" value="" />

                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Dati superficie -->
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-group">
                                    <div class="input-group" id="">
                                        <label class="input-group-addon" for="pop_up_sup_google" id="lbl_pop_up_sup_google">
                                            <asp:Localize meta:resourcekey="lbl_pop_up_sup_google" runat="server">Sup. calcolata (HA):</asp:Localize>
                                        </label>

                                        <input class="form-control" type="text" id="pop_up_sup_google" name="pop_up_sup_google" value="" disabled="disabled" />
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                <div class="form-group">
                                    <div class="input-group" id="">
                                        <label class="input-group-addon" for="pop_up_sup_app" id="lbl_pop_up_sup_app">
                                            <asp:Localize meta:resourcekey="lbl_pop_up_sup_app" runat="server">Superficie (HA)</asp:Localize>
                                        </label>

                                        <input class="form-control" type="text" id="pop_up_sup_app" name="pop_up_sup_app" value="" />
                                    </div>
                                </div>
                            </div>

                        </div>



                    </li>


                    <li class="k-state-active k-active agroAccordion" id="panelBarDatiAzienda_Specie">
                        <span class="k-link agroAccordionTitolo"><span class="fa fa-leaf"></span><span id="panelBarDatiAziendaSpecie_Titolo"><asp:Localize meta:resourcekey="panelBarDatiAziendaSpecie_Titolo" runat="server">Dati Agronomici</asp:Localize></span></span>

                        <div class="agroContainer" id="containerDatiAzienda_Specie">
                            <!-- Dati Specie ... -->
                            <!-- Dati specie cultivar ecc ..-->
                            <div class="row display-flex">
                                
                                <div class="col-sm-2 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <input type="checkbox" id="terreno_nudo" class="k-checkbox" style="margin: 0;" onclick="TerrenoNudoClick(this)">
                                            <label class="k-checkbox-label" for="terreno_nudo" id="lbl_terrenonudo">
                                                <asp:Localize meta:resourcekey="lbl_terrenonudo" runat="server">Terreno nudo (nessuna coltura)</asp:Localize>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-10 col-xs-12 TerrenoNudo">
                                    <div class="form-group ">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_destuso" id="lbl_pop_up_destuso">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_destuso" runat="server">Destinazione d'uso</asp:Localize>
                                            </label>

                                            <select class="form-control cmb" id="pop_up_destuso" name="pop_up_destuso"></select>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-5 col-xs-12 ColturaPresente">
                                    <div class="form-group ">
                                        <div class="input-group" id="divpop_up_specie">
                                            <label class="input-group-addon" for="pop_up_specie" id="lbl_pop_up_specie">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_specie" runat="server">Specie</asp:Localize>
                                            </label>

                                            <select class="form-control cmb" id="pop_up_specie" name="pop_up_specie"></select>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-5 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_varieta" id="lbl_pop_up_varieta">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_varieta" runat="server">Varietà</asp:Localize>
                                            </label>
                                            <select class="form-control" id="pop_up_varieta" name="pop_up_varieta"></select>

                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_finalita" id="lbl_pop_up_finalita">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_finalita" runat="server">Finalità</asp:Localize>
                                            </label>
                                            <select class="form-control" id="pop_up_finalita" name="pop_up_finalita"></select>

                                        </div>
                                    </div>
                                </div>


                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="DatiTipologia">
                                            <label class="input-group-addon" for="pop_up_tipologia" id="lbl_pop_up_tipologia">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_tipologia" runat="server">Tipologia</asp:Localize>
                                            </label>
                                            <select class="form-control" id="pop_up_tipologia" name="pop_up_tipologia"></select>

                                        </div>
                                    </div>
                                </div>

                            </div>


                        </div>

                    </li>

                    <li class="agroAccordion" id="panelBarDatiAzienda_Altro">
                        <span class="k-link agroAccordionTitolo"><span class="fa fa-check-square"></span><span id="panelBarDatiAziendaAltro_Titolo"><asp:Localize meta:resourcekey="panelBarDatiAziendaAltro_Titolo" runat="server">Opzioni</asp:Localize></span></span>

                        <div class="agroContainer" id="containerDatiAzienda_Altro">

                            <!-- altre informazioni -->
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_d_semina" id="lbl_pop_up_semina">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_semina" runat="server">Data Semina Prevista</asp:Localize>
                                            </label>
                                            <input class="form-control kendoDatePicker" type="text" id="pop_up_d_semina" name="pop_up_d_semina" value="" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_d_raccolta" id="lbl_pop_up_d_raccolta">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_d_raccolta" runat="server">Data Raccolta Prevista</asp:Localize>
                                            </label>
                                            <input class="form-control kendoDatePicker" type="text" id="pop_up_d_raccolta" name="pop_up_d_raccolta" value="" />
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_disciplinare" id="lbl_pop_up_disciplinare">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_disciplinare" runat="server">Disciplinare</asp:Localize>
                                            </label>
                                            <select class="form-control" id="pop_up_disciplinare" name="pop_up_disciplinare"></select>

                                        </div>
                                    </div>
                                </div>


                            </div>

                            <!-- lotto -->
                            <div id="kws_appezza" class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_app_originale" id="lbl_pop_up_app_originale">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_app_originale" runat="server">N. Originale</asp:Localize>
                                            </label>
                                            <input class="form-control" type="text" id="pop_up_app_originale" name="pop_up_app_originale" value="" />
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_app_n_appezza" id="lbl_pop_up_app_n_appezza">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_app_n_appezza" runat="server">N. Impianto</asp:Localize>
                                            </label>
                                            <input class="form-control" type="text" id="pop_up_app_n_appezza" name="pop_up_app_n_appezza" value="" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="NuovoImpiantoCodiciAggiuntivi" class="row">

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_app_PianoSemina" id="lbl_pop_up_app_PianoSemina">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_app_PianoSemina" runat="server">Piano Semina</asp:Localize>
                                            </label>
                                            <input class="form-control CodiceAnagrafe" type="text" id="pop_up_app_PianoSemina" name="pop_up_app_PianoSemina" value="" data-idCod="1071" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_app_OrganismoReferente" id="lbl_pop_up_app_OrganismoReferente">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_app_OrganismoReferente" runat="server">Organismo Referente</asp:Localize>
                                            </label>
                                            <select id="pop_up_app_OrganismoReferente" name="pop_up_app_OrganismoReferente"
                                                    class="form-control CodiceAnagrafe" data-idCod="1074"></select>
                                        </div>
                                    </div>
                                </div>

                            </div> <!-- row -->

                            <div class="row">
                                <div class="form-group col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="div_lotto">
                                            <label class="input-group-addon" for="pop_up_lotto" id="lbl_pop_up_lotto">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_lotto" runat="server">Lotto</asp:Localize>
                                            </label>
                                            <input class="form-control" type="text" id="pop_up_lotto" name="pop_up_lotto" value="" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </li>
                </ul>



                <div class="frmGisSepatoreGrande"></div>




                <div id="placeData_Inizio_Fine"></div>

                <!--<div id="divOpzioni_pop_up_impianto">
                    <p class="heading">
                        Opzioni
                    </p>

                    <div class="content">


                        <div id="Dati_data_inizio_fine">


                        </div>

                    </div>

                </div>-->


            </div>


            <div style="display: flex; justify-content: flex-end;">
                <div id="btnVerificaVicini" class="k-button" data-role="button" role="button" aria-disabled="false" tabindex="0">
                    <span class="fa fa-crosshairs fa-2x"></span>
                    <span id="lbl_verificaVicini"><asp:Localize meta:resourcekey="lbl_verificaVicini" runat="server">Verifica prossimità</asp:Localize></span>
                </div>
            </div>

            <!--<div class="row">

                <div id="righelloSpecie" style="float: right; margin-top: -15px; margin-bottom: -15px;">
                    <img src="replace_ruler" alt="" />
                </div>
                <div id="righelloCampoVicino" style="float: right; margin-top: -15px; margin-bottom: -15px;">
                    <img src="replace_ruler-info" alt="" />
                </div>

            </div>-->
            <!--<div id="responseInterferenze" class="row">
            </div>-->

        </div>



        <!-- Dialog -->
        <div id="dialogCultivarRicette" title="Selezione per Specie e Varietà"> <!-- i18n -->
            <label id="lbl_CultivarRicette" style="min-width: 300px">
                <asp:Localize meta:resourcekey="lbl_CultivarRicette" runat="server">Selezione per Specie e Varietà:</asp:Localize>
            </label>
            <div id="tblCultivarRicette">
            </div>
            <input type="hidden" id="hidCultivarRicette" />
            <input type="hidden" id="hidCultivarRicetteNodoPartenza" />
            <hr />
            <input type="checkbox" id="ckRibaltaGrafica" /><label for="ckRibaltaGrafica" id="lblRibaltaGrafica" style="min-width: 300px">
                <asp:Localize meta:resourcekey="lblRibaltagrafica" runat="server">Copia elementi grafici sul layer impianti leggendo da:</asp:Localize>
            </label>
            <select id="cmbLayerRibaltaGrafica">
                <option value="1" id="opt_RibaltaGraficaAppezzamenti">
                    <asp:Localize meta:resourcekey="opt_RibaltaGraficaAppezzamenti" runat="server">Appezzamenti</asp:Localize>
                </option>
            </select>
            <br />
        </div>
        <!-- i18n -->
        <div id="dialogRateo" title="parametri mappa di prescrizione">

            <div class="row">
                <!-- Descrizione -->
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="input-group" id="div_txtDescrizioneDelPiano">
                        <label class="input-group-addon control-label " id="lbl_txtDescrizioneDelPiano" for="txt_txtDescrizioneDelPiano">
                            <asp:Localize meta:resourcekey="lbl_txtDescrizioneDelPiano" runat="server">Descrizione:</asp:Localize>
                        </label>
                        <input class="form-control" id="txtDescrizioneDelPiano" aria-describedby="lbl_txtDescrizioneDelPiano" type="text" />
                    </div>
                </div>
            </div>
            <div class="row">
                
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                    <div class="form-group ">
                        <div class="input-group" id="divlblDialogRateo_dataSentinel">
                            <label class="input-group-addon" for="txtDialogRateo_dataSentinel" id="lbl_DialogRateo_dataSentinel">
                                <asp:Localize meta:resourcekey="lbl_DialogRateo_dataSentinel" runat="server">Data di lettura dei parametri satellitari:</asp:Localize>
                            </label>

                            <input type="text" id="txtDialogRateo_dataSentinel" name="txtDialogRateo_dataSentinel" value="" class="form-control kendoDatePicker" />

                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                    <div class="form-group ">
                        <div class="input-group" id="divlblDialogRateo_celle">
                            <label class="input-group-addon" for="txtCellSize" id="lbl_CellSize">
                                <asp:Localize meta:resourcekey="lbl_CellSize" runat="server">Dimensione in metri delle celle:</asp:Localize>
                            </label>

                            <input type="text" id="txtCellSize" name="txtCellSize" value="20" style="width: 75px"
                                   class="form-control" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- i18n -->
        <div id="dialogGeneraPlanning" title="genera una nuova pianificazione in base a selezione">
            <label for="txtNomePlanning" id="lbl_NomePlanning">
                <asp:Localize meta:resourcekey="lbl_NomePlanning" runat="server">Nome della pianificazione:</asp:Localize>
            </label>
            <input type="text" id="txtNomePlanning" name="txtNomePlanning" value="" style="width: 266px"
                   class="" /><br />
            <hr />
            <input type="checkbox" id="ckAggregaEntitaGrafichePerPianificazione" />
            <label for="ckAggregaEntitaGrafichePerPianificazione" id="lbl_AggregaEntitaGrafiche" style="min-width: 200px">
                <asp:Localize meta:resourcekey="lbl_AggregaEntitaGrafiche" runat="server">Aggrega la grafica (se possibile), con uno scarto pari a:</asp:Localize>
            </label>
            <input type="text" id="txtAggregaEntitaGrafichePerPianificazioneScartoM" disabled="disabled"
                   name="txtNomePlanning" value="0" style="width: 30px" class="" />
            <div id="gplus" class="bottone" onclick="gplus();" style="font-size: 0.9em; float: right;">
                +
            </div>
            <div id="gminus" class="bottone" onclick="gminus();" style="font-size: 0.9em; float: right;">
                -
            </div>
            <br />
        </div>
        <!-- i18n -->
        <div id="dialogAB" title="imposta linee guida">
            <label for="lblAB" id="lbl_DialogAB">
                <asp:Localize meta:resourcekey="lbl_DialogAB" runat="server">Conferma salvataggio?</asp:Localize>
            </label>
        </div>
        <!-- i18n -->
        <div id="dialogMultipoint" title="Salvataggio dei punti inseriti">
            <label for="txtDialogMultipointDes" id="lbl_dialogMultipointDes">
                <asp:Localize meta:resourcekey="lbl_dialogMultipointDes" runat="server">Assegna questa descrizione:</asp:Localize>
            </label>
            <input type="text" id="txtDialogMultipointDes" class="textUI" /><br />
            <label id="lbl_dialogMultipoint">
                <asp:Localize meta:resourcekey="lbl_dialogMultipoint" runat="server">Conferma salvataggio?</asp:Localize>
            </label>
        </div>
        <div id="dialogConfermaAssociazione" class="load-hidden">
            <div style="display:grid; grid-auto-flow:column; grid-template-columns: max-content 250px; grid-template-rows: auto auto; grid-gap:10px; align-items:center; justify-items:end;">

                <label>Associa la superficie calcolata (<strong><span id="lblDialogSup"></span></strong>) [ha] a ?</label>
                <label id="lbl_dialogAssociazioneGIS">
                    <asp:Localize meta:resourcekey="lbl_dialogAssociazioneGIS" runat="server">Genera elemento GIS sui layer:</asp:Localize>
                </label>

                <div style="width:-webkit-fill-available;">
                    <select class="form-control" id="option_associa">
                        <option value="1" id="opt_associaNessuno"><asp:Localize meta:resourcekey="opt_associaNessuno" runat="server">Nessuno</asp:Localize></option>
                        <option value="2" id="opt_associaImpianto"><asp:Localize meta:resourcekey="opt_associaImpianto" runat="server">Impianto</asp:Localize></option>
                        <option value="3" id="opt_associaImpiantoAppezz"><asp:Localize meta:resourcekey="opt_associaImpiantoAppezz" runat="server">Impianto e Appezzamento</asp:Localize></option>
                    </select>
                </div>

                <div style="width:-webkit-fill-available;">
                    <select class="form-control" id="cmbAssocia_GenerazionePoligoni">
                        <option id="cmbAssocia_GenerazionePoligoni_1" value="1"><asp:Localize meta:resourcekey="cmbAssocia_GenerazionePoligoni_1" runat="server">Appezzamento</asp:Localize></option>
                        <option id="cmbAssocia_GenerazionePoligoni_2" value="2"><asp:Localize meta:resourcekey="cmbAssocia_GenerazionePoligoni_2" runat="server">Impianto</asp:Localize></option>
                        <option id="cmbAssocia_GenerazionePoligoni_3" value="3"><asp:Localize meta:resourcekey="cmbAssocia_GenerazionePoligoni_3" runat="server">Impianto e Appezzamento</asp:Localize></option>
                    </select>
                </div>

            </div>

            <!--
                        <div class="row">
                            <div class="col-lg-5 col-md-5 col-sm-5 col-xs-12">
                                <div class="input-group">
                                    <label class="input-group-addon" for="option_associa">
                                        Associa la superficie calcolata (<strong>
                                            <span id="lblDialogSup">
                                            </span>
                                        </strong>) [ha] a ?
                                    </label>
                                    <select class="form-control" id="option_associa">
                                        <option value="1">Nessuno</option>
                                        <option value="2">Impianto</option>
                                        <option value="3">Impianto e Appezzamento</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-5 col-md-5 col-sm-5 col-xs-12">
                                <div class="form-group">
                                    <div class="input-group">
                                        <label class="input-group-addon" for="cmbAssocia_GenerazionePoligoni">
                                            Genera elemento GIS sui layer:
                                        </label>
                                        <select class="form-control" id="cmbAssocia_GenerazionePoligoni">
                                            <option id="cmbAssocia_GenerazionePoligoni_1" value="1">Appezzamento</option>
                                            <option id="cmbAssocia_GenerazionePoligoni_2" value="2">Impianto</option>
                                            <option id="cmbAssocia_GenerazionePoligoni_3" value="3">Impianto e Appezzamento</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>
            -->

        </div>
        <div id="dialogAllertOperazioniAppezza" title="Continuare?">
            <label id="lbl_alert_operazioni_Appezza" style="width: 240px">
            </label>
        </div>
        <div id="dialogAllertOperazioni" title="Continuare?">
            <label id="lbl_alert_operazioni" style="width: 240px">
            </label>
            <input type="hidden" id="dialogAllertOperazioni_hidden" />
        </div>



        <!-- Dialog Modifica Impianto -->
        <div id="pop_up_modificaImpianto" class="load-hidden">

            <input type="hidden" id="hiddenPunti_modifica" />
            <input type="hidden" id="hiddenID" />

            <div class="form-horizontal" style="/*min-height: 400px; max-height:400px; overflow: scroll*/">
                <ul id="panelBarDatiModifica">
                    <li class="k-state-active k-active agroAccordion" id="panelBarDatiModifica_Testata">
                        <span class="k-link agroAccordionTitolo">
                            <span class="fa fa-list"></span>
                            <span id="panelBarDatiModificaTestata_Titolo"><asp:Localize meta:resourcekey="panelBarDatiModificaTestata_Titolo" runat="server">Dati Aziendali, di Appezzamento e Superfici</asp:Localize></span>
                        </span>
                        
                        <div>
                            
                            <div class="row" style="margin-bottom:15px;">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" id="stileAnagraficaRiassuntoModifica">
                                    <span id="lbl_smallDescrAzienda_modifica" style="font-weight: bold;">
                                        <asp:Localize meta:resourcekey="lbl_smallDescrAzienda_modifica" runat="server">Azienda:</asp:Localize>
                                    </span><span id="smallDescrAzienda_modifica"></span><br />
                                    <span id="lbl_smallDescrCentro_modifica" style="font-weight: bold;">
                                        <asp:Localize meta:resourcekey="lbl_smallDescrCentro_modifica" runat="server">Centro az.:</asp:Localize>
                                    </span><span id="smallDescrCentro_modifica"></span><br />
                                    <span id="lbl_smallDescrAlbero_modifica" style="font-weight: bold;">
                                        <asp:Localize meta:resourcekey="lbl_smallDescrAlbero_modifica" runat="server">Anagrafica:</asp:Localize>
                                    </span><span id="smallDescrAlbero_modifica"></span>
                                </div>
                                
                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12" id="divImpiantiFigli_Modifica">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="cmbImpiantiFigli_Modifica" id="lblImpiantiFigli_Modifica">
                                                <asp:Localize meta:resourcekey="lblImpiantiFigli_Modifica" runat="server">Impianto:</asp:Localize>
                                            </label>
                                            <input type="hidden" id="HiddenSelezioneAlberoAnagraficaAlberoAnagraficaModificaImpianto" />
                                            <select class="form-control" id="cmbImpiantiFigli_Modifica"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div id="divDatiAppezza" class="row">
                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="via_stringa_modifica" id="lbl_via_stringa_modifica"><asp:Localize meta:resourcekey="lbl_via_stringa_modifica" runat="server">Via</asp:Localize></label>
                                            <input class="form-control" type="text" id="via_stringa_modifica" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="pop_up_modificaImpianto_DatiImp">
                                            <label class="input-group-addon" for="pop_up_nome_appezza_modifica" id="lbl_pop_up_nome_appezza_modifica"><asp:Localize meta:resourcekey="lbl_pop_up_nome_appezza_modifica" runat="server">Nome</asp:Localize></label>
                                            <input class="form-control" type="text" id="pop_up_nome_appezza_modifica" name="pop_up_nome_appezza_modifica" value="" />
                                        </div>
                                    </div>
                                </div>
                                
                                <!--al momento commentato, decommantare quando si vorrà modificare da GIS questo dato -->
                                <!--<div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                    <div class="form-group">
                                    <div class="input-group" id="pop_up_modificaImpianto_qdc">
                                    <label class="input-group-addon" for="pop_up_qdc_appezza_modifica">
                                    Rif. QDC
                                    </label>
                                    <input class="form-control m_CodiceAnagrafe" type="text" id="pop_up_qdc_appezza_modifica" name="pop_up_qdc_appezza_modifica" data-idCod="1104" value="" />
                                    </div>
                                    </div>
                                    </div>-->
                                
                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-12" id="pop_up_Associa_Modifica_GenerazionePoligoni">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="cmbAssocia_Modifica_GenerazionePoligoni" id="lblbAssocia_Modifica_GenerazionePoligoni"><asp:Localize meta:resourcekey="lblbAssocia_Modifica_GenerazionePoligoni" runat="server">Memorizza:</asp:Localize></label>
                                            <select class="form-control" id="cmbAssocia_Modifica_GenerazionePoligoni">
                                                <!--<option id="cmbAssocia_Modifica_GenerazionePoligoni_1" value="1">Appezzamento</option>
                                                    <option id="cmbAssocia_Modifica_GenerazionePoligoni_2" value="2">Impianto</option>
                                                    <option id="cmbAssocia_Modifica_GenerazionePoligoni_3" value="3">Impianto e Appezzamento</option>-->                                        </select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="row">
                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_sup_precedente" id="lbl_pop_up_sup_precedente"><asp:Localize meta:resourcekey="lbl_pop_up_sup_precedente" runat="server">Sup. Originale [ha]:</asp:Localize></label>
                                            <input type="text" id="pop_up_sup_precedente" name="pop_up_sup_precedente" value="" style="min-width: 74px" class="form-control" disabled="disabled">
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_sup_google_modifica" id="lbl_pop_up_sup_google_modifica">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_sup_google_modifica" runat="server">Sup. calcolata [ha]:</asp:Localize>
                                            </label>
                                            <input type="text" id="pop_up_sup_google_modifica" name="pop_up_sup_google_modifica" style="min-width: 74px" class="form-control" value="" disabled="disabled">
                                            <span class="input-group-btn" id="arrow">
                                                <a class="form-control btn btn-secondary" style="border-bottom-right-radius: 4px; border-top-right-radius: 4px;" title="Riporta"><span class="fa fa-arrow-right"></span></a>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_sup_app_modifica" id="lbl_pop_up_sup_app_modifica">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_sup_app_modifica" runat="server">Sup. Nuova [ha]:</asp:Localize>
                                            </label>
                                            <input type="text" id="pop_up_sup_app_modifica" name="pop_up_sup_app_modifica" value="" class="form-control" style="">
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12" id="pop_up_sup_app_modifica_divAssocia">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="opt_associa_modifica" id="lbl_opt_associa_modifica"><asp:Localize meta:resourcekey="lbl_opt_associa_modifica" runat="server">Associa la superficie a?</asp:Localize></label>
                                            <select class="form-control" id="opt_associa_modifica">
                                                <option value="1" id="opt_associaModificaNessuno"><asp:Localize meta:resourcekey="opt_associaModificaNessuno" runat="server">Nessuno</asp:Localize></option>
                                                <option value="2" id="opt_associaModificaImpianto"><asp:Localize meta:resourcekey="opt_associaModificaImpianto" runat="server">Impianto</asp:Localize></option>
                                                <option value="3" id="opt_associaModificaImpiantoAppezz"><asp:Localize meta:resourcekey="opt_associaModificaImpiantoAppezz" runat="server">Impianto e Appezzamento</asp:Localize></option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->
                            
                            <div class="row" id="pop_up_sup_app_modifica_divModificaDatiDiAnagrafica">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="text-align: left !important;" id="lbl_modificaDatiAnagrafica">
                                            <asp:Localize meta:resourcekey="lbl_modificaDatiAnagrafica" runat="server">Modifica i dati di anagrafica</asp:Localize>
                                        </span>
                                        <div class="form-control" style="width:auto;">
                                            <input type="checkbox" id="pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica" class="k-checkbox" style="margin:0px;"/>
                                            <label class="k-checkbox-label" for="pop_up_sup_app_modifica_chkModificaDatiDiAnagrafica" style="padding-left: 12.6px;"></label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>

                    <li class="k-state-active k-active agroAccordion" id="panelBarDatiModifica_Specie">
                        <span class="k-link agroAccordionTitolo"><span class="fa fa-leaf"></span><span id="panelBarDatiModificaSpecie_Titolo"><asp:Localize meta:resourcekey="panelBarDatiModificaSpecie_Titolo" runat="server">Dati Agronomici</asp:Localize></span></span>

                        <div>
                            <div class="row display-flex">
                                <div class="col-sm-2 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group">
                                            <input type="checkbox" id="terreno_nudo_modifica" class="k-checkbox" style="margin: 0;" onclick="TerrenoNudoClick(this)">
                                            <label class="k-checkbox-label" for="terreno_nudo_modifica" id="lbl_terrenonudo_modifica">
                                                <asp:Localize meta:resourcekey="lbl_terrenonudo_modifica" runat="server">Terreno nudo (nessuna coltura)</asp:Localize>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-sm-10 col-xs-12 TerrenoNudo">
                                    <div class="form-group ">
                                        <div class="input-group">
                                            <label class="input-group-addon" for="pop_up_destuso" id="lbl_pop_up_destuso_modifica">
                                                <asp:Localize meta:resourcekey="lbl_pop_up_destuso_modifica" runat="server">Destinazione d'uso</asp:Localize>
                                            </label>
                                            <select class="form-control cmb" id="pop_up_destuso_modifica" name="pop_up_destuso_modifica"></select>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-sm-5 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_specie_modifica" id="lbl_pop_up_specie_modifica"><asp:Localize meta:resourcekey="lbl_pop_up_specie_modifica" runat="server">Specie</asp:Localize></label>
                                            <select class="form-control" id="pop_up_specie_modifica" name="pop_up_specie_modifica"></select>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-sm-5 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_varieta_modifica" id="lbl_pop_up_varieta_modifica"><asp:Localize meta:resourcekey="lbl_pop_up_varieta_modifica" runat="server">Varietà</asp:Localize></label>
                                            <select class="form-control" id="pop_up_varieta_modifica" name="pop_up_varieta_modifica"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->
                            
                            <div class="row">
                                <div class="col-sm-6 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_finalita_modifica" id="lbl_pop_up_finalita_modifica"><asp:Localize meta:resourcekey="lbl_pop_up_finalita_modifica" runat="server">Finalità</asp:Localize></label>
                                            <select class="form-control" id="pop_up_finalita_modifica" name="pop_up_finalita_modifica"></select>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-sm-6 col-xs-12 ColturaPresente">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_tipologia_modifica" id="lbl_pop_up_tipologia_modifica"><asp:Localize meta:resourcekey="lbl_pop_up_tipologia_modifica" runat="server">Tipologia</asp:Localize></label>
                                            <select class="form-control" id="pop_up_tipologia_modifica" name="pop_up_tipologia_modifica"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->
                            
                            <div id="Dati_data_inizio_fine_modifica" class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_m_data_inizio" id="lblpop_up_m_data_inizio_modifica"><asp:Localize meta:resourcekey="lblpop_up_m_data_inizio_modifica" runat="server">Data Inizio:</asp:Localize></label>
                                            <input type="text" id="pop_up_m_data_inizio" name="pop_up_m_data_inizio" value="" class=" form-control kendoDatePicker" />
                                        </div>
                                    </div>
                                    <div id="div_appezza_data_inizio" class="divAppezzaDate divAppezzaDateNascondi">
                                        <i id="i_appezza_data_inizio" class="fa fa-exclamation-triangle WarningDateAppezza" style="display:none" aria-hidden="true"></i>
                                        <small>
                                            <span style="font-weight: bold" id="bold_appezza_data_inizio"><asp:Localize meta:resourcekey="bold_appezza_data_inizio" runat="server">APPEZZAMENTO validità inizio:</asp:Localize> </span>
                                            <em id="lbl_appezza_data_inizio"></em>
                                        </small>
                                    </div>
                                </div>
                                
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_m_data_fine" id="lblpop_up_m_data_fine"><asp:Localize meta:resourcekey="lblpop_up_m_data_fine" runat="server">Data Fine:</asp:Localize></label>
                                            <input type="text" id="pop_up_m_data_fine" name="pop_up_m_data_fine" value="" class="form-control kendoDatePicker" />
                                        </div>
                                    </div>
                                    
                                    <div id="div_appezza_data_fine" class="divAppezzaDate divAppezzaDateNascondi">
                                        <i id="i_appezza_data_fine" class="fa fa-exclamation-triangle WarningDateAppezza" style="display:none" aria-hidden="true"></i>
                                        <small>
                                            <span style="font-weight: bold" id="bold_appezza_data_fine"><asp:Localize meta:resourcekey="bold_appezza_data_fine" runat="server">APPEZZAMENTO validità fine:</asp:Localize> </span>
                                            <em id="lbl_appezza_data_fine"></em>
                                        </small>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->

                        </div>
                    </li>

                    <li class="k-state-active k-active agroAccordion" id="panelBarDatiModifica_Altro">

                        <span class="k-link agroAccordionTitolo"><span class="fa fa-check-square"></span><span id="panelBarDatiModificaAltro_Titolo"><asp:Localize meta:resourcekey="panelBarDatiModificaAltro_Titolo" runat="server">Opzioni</asp:Localize></span></span>

                        <div>
                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="div_m_CodiceSocio">
                                            <label class="input-group-addon" for="txt_m_CodiceSocio" id="lbl_m_CodiceSocio"><asp:Localize meta:resourcekey="lbl_m_CodiceSocio" runat="server">Codice Socio:</asp:Localize></label>
                                            <input class="form-control" type="text" id="txt_m_CodiceSocio" name="txt_m_CodiceSocio" value="" />
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="div_m_lotto">
                                            <label class="input-group-addon" for="pop_up_m_lotto" id="lbl_pop_up_m_lotto"><asp:Localize meta:resourcekey="lbl_pop_up_m_lotto" runat="server">Lotto:</asp:Localize></label>
                                            <input class="form-control" type="text" id="pop_up_m_lotto" name="pop_up_m_lotto" value="" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->
                            
                            <div id="kws_m_appezza" class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_m_app_originale" id="lbl_pop_up_m_app_originale"><asp:Localize meta:resourcekey="lbl_pop_up_m_app_originale" runat="server">N. Originale</asp:Localize></label>
                                            <input class="form-control" type="text" id="pop_up_m_app_originale" name="pop_up_m_app_originale" value="" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_app_m_n_appezza" id="lbl_pop_up_app_m_n_appezza"><asp:Localize meta:resourcekey="lbl_pop_up_app_m_n_appezza" runat="server">N. Impianto</asp:Localize></label>
                                            <input type="text" id="pop_up_app_m_n_appezza" name="pop_up_app_m_n_appezza" value="" class="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->
                            
                            <div id="ModificaImpiantoCodiciAggiuntivi" class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_m_app_PianoSemina" id="lbl_pop_up_m_app_PianoSemina"><asp:Localize meta:resourcekey="lbl_pop_up_m_app_PianoSemina" runat="server">Piano Semina</asp:Localize></label>
                                            <input class="form-control m_CodiceAnagrafe" type="text" id="pop_up_m_app_PianoSemina" name="pop_up_m_app_PianoSemina" value="" data-idCod="1071" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-12">
                                    <div class="form-group">
                                        <div class="input-group" id="">
                                            <label class="input-group-addon" for="pop_up_app_m_OrganismoReferente" id="lbl_pop_up_app_m_OrganismoReferente"><asp:Localize meta:resourcekey="lbl_pop_up_app_m_OrganismoReferente" runat="server">Organismo Referente</asp:Localize></label>
                                            <select id="pop_up_app_m_OrganismoReferente" name="pop_up_app_m_OrganismoReferente" class="form-control m_CodiceAnagrafe" data-idCod="1074"></select>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- row -->

                        </div>
                    </li>
                </ul>
            </div>

            <div style="display: flex; justify-content: flex-end; margin-top:20px;">
                <div id="btnVerificaVicini_2" class="k-button" data-role="button" role="button" aria-disabled="false" tabindex="0">
                    <span class="fa fa-crosshairs fa-2x"></span>
                    <span id="lbl_verificaVicini_2"><asp:Localize meta:resourcekey="lbl_verificaVicini_2" runat="server">Verifica prossimità</asp:Localize></span>
                </div>
            </div>
        </div>


        <!-- i18n -->
        <!-- Dialog Elimina Multipoint -->
        <div id="dialogEliminaMultipoint" title="conferma eliminazione">
            <fieldset>
                <legend></legend>
                <br />
                <label for="ckConfermaEliminaMultipoint" id="lbl_confermaEliminaMultipoint">
                    <asp:Localize meta:resourcekey="lbl_confermaEliminaMultipoint" runat="server">conferma eliminazione:</asp:Localize>
                </label>
                <input type="checkbox" id="ckConfermaEliminaMultipoint" style="width: 150px" />
            </fieldset>
        </div>
        <!-- Dialog Elimina punti scomposti -->
        <div id="dialogEliminaImpiantoPuntiScomposti" title="conferma conclusione lavoro">
            <fieldset>
                <legend></legend>
                <br />
                <label for="ckConfermaEliminaPuntiScomposti" id="lbl_confermaEliminaPuntiScomposti">
                    <asp:Localize meta:resourcekey="lbl_confermaEliminaPuntiScomposti" runat="server">conferma conclusione:</asp:Localize>
                </label>
                <input type="checkbox" id="ckConfermaEliminaPuntiScomposti" style="width: 150px" />
            </fieldset>
        </div>
        <!-- Dialog Elimina Impianto -->
        <div id="dialogEliminaImpianto" title="conferma eliminazione">
            <fieldset>
                <legend></legend>
                <div id="divEliminaGrafica" style="margin-bottom: -6px">
                    <label for="ckEliminaGrafica" style="width: 150px">
                        - Elimina Entità Grafiche
                    </label>
                    <input type="checkbox" id="ckEliminaGrafica" checked="checked" />
                </div>
                <br />
                <div id="divEliminaPrecision" style="margin-bottom: -6px">
                    <label for="ckEliminaPrecision" style="width: 150px">
                        - Elimina i dati raccolti con la Precision Farming
                    </label>
                    <input type="checkbox" id="ckEliminaPrecision" />
                </div>
                <br />
                <div id="divEliminaPrecisionAB" style="margin-bottom: -6px">
                    <label for="ckEliminaPrecisionABLine" style="width: 150px">
                        - Elimina linee guida AB di Precision Farming
                    </label>
                    <input type="checkbox" id="ckEliminaPrecisionABLine" />
                </div>
                <br />
                <div id="divEliminaDatoPalm" style="margin-bottom: -6px">
                    <label for="ckEliminaDatoPalm" style="width: 150px">
                        - Elimina il dato misurato con Gias PALM
                    </label>
                    <input type="checkbox" id="ckEliminaDatoPalm" />
                </div>
                <br />
                <div id="divEliminaEntitaGIAS" style="margin-bottom: -6px">
                    <label for="ckEliminaImpianto" style="width: 150px">
                        - Elimina Entità Gias (Appezzamento/Impianto)
                    </label>
                    <input type="checkbox" id="ckEliminaImpianto" />
                </div>
                <br />
                <br />
                <label for="ckConfermaElimina" style="width: 150px">
                    <strong>conferma eliminazione:</strong>
                </label>
                <input type="checkbox" id="ckConfermaElimina" />
            </fieldset>
        </div>
        <!-- i18n -->
        <!--Dialog sessione scaduta-->
        <div id="dialogSessioneScaduta" style="display: none" title="La sessione è scaduta.">
            <fieldset>
                <legend></legend>
                <div id="lblManualLogin">
                    <p>
                        <span style="font-weight: bold" id="bold_sessioneScaduta"><asp:Localize meta:resourcekey="bold_sessioneScaduta" runat="server">La sessione è scaduta. Eseguire di nuovo il login.</asp:Localize></span>
                        <br />
                        Se non appare in automatico la finestra di login <a href="../GST_Autenticazione/Autenticazione.aspx">
                            fare click qui.
                        </a>
                    </p>
                </div>
                <div id="lblAutoLogin">
                </div>
            </fieldset>
        </div>
        <!-- DIALOG Gestion Colori Layer -->
        <div id="dialogGestioneColoriLayer" title="">
            <fieldset>
                <legend></legend>
                <div id="Div2">
                    <input type="hidden" id="hiddenPrincipale_1_Dettagli_2" />
                    <select id="ddlTipologiaLayer" name="ddlTipologiaLayer" onchange="Cambia_ddlTipologiaLayer()"
                            style="width: 300px"></select>
                    <br />

                    <div id="place_tabella" style="overflow: auto; max-height: 300px">
                        <!-- questo sarà sostituito a runtime da javascript, ma nel frattempo occupa uno spazio per centrare il dialog.-->
                        <div style="min-width: 677px; min-height: 299px">&nbsp;</div>
                    </div>

                </div>
            </fieldset>
        </div>
        <!-- Nuova Azienda-->
        <div data-role="page" id="popup_nuova_azienda" title="Nuova Azienda">
            <fieldset>
                <legend><asp:Localize meta:resourcekey="lbl_pop_up_impianto_azienda" runat="server">Azienda</asp:Localize></legend>
                <div data-role="content">
                    <label for="txt_rag_Soc" id="lbl_rag_Soc">
                        <asp:Localize meta:resourcekey="lbl_rag_Soc" runat="server">Ragione Sociale:</asp:Localize>
                    </label>
                    <input type="text" id="txt_rag_Soc" name="txt_rag_Soc" value="" style="width: 295px" />
                    <br />
                    <br />
                    <label for="txt_Piva" id="lbl_Piva">
                        <asp:Localize meta:resourcekey="lbl_Piva" runat="server">PIVA:</asp:Localize>
                    </label>
                    <input type="text" id="txt_Piva" maxlength="11" name="txt_Piva" value="" style="width: 295px" />
                    <div class="img_tool" id="nuovapiva-button" style="float: right; margin-top: -15px;
                margin-bottom: -15px;">
                        <img src="replace_piu" alt="" />
                    </div>
                    <br />
                    <label for="txt_CodiceSocio" id="lbl_CodiceSocio">
                        <asp:Localize meta:resourcekey="lbl_CodiceSocio" runat="server">Codice Socio:</asp:Localize>
                    </label>
                    <input type="text" id="txt_CodiceSocio" name="txt_CodiceSocio" value="" style="width: 295px" />
                    <br />
                    <br />
                    <div id="indirizzoImprese">
                    </div>
                    <div data-role="collapsible" id="OpzioniNuovaImpresa">
                        <span class="heading" id="lbl_OpzioniNuovaImpresa">
                            <asp:Localize meta:resourcekey="lbl_OpzioniNuovaImpresa" runat="server">Opzioni</asp:Localize>
                        </span>
                        <div class="content">
                            <label for="Cmb_Imprese_Padre" id="lbl_Cmb_Imprese_Padre">
                                <asp:Localize meta:resourcekey="lbl_Cmb_Imprese_Padre" runat="server">Impresa Padre</asp:Localize>
                            </label>
                            <select id="Cmb_Imprese_Padre" name="Cmb_Imprese_Padre" style="width: 300px"></select>
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>
        <script type="text/javascript">
            var imprese_ID_ind = 'indirizzoImprese';
            var centro_ID_ind = 'indirizzoCentro';

            $(document).ready(function () {
                generaIndirizzo(imprese_ID_ind);
                generaIndirizzo(centro_ID_ind);

                CaricaStatiISO(imprese_ID_ind, "");
                CaricaStatiISO(centro_ID_ind, "");

                ascoltaStato(imprese_ID_ind);
                ascoltaStato(centro_ID_ind);

                inizializzaDDLProvincie('ddl_Provincia_', imprese_ID_ind);
                inizializzaDDLProvincie('ddl_Provincia_', centro_ID_ind);

                ascoltaProvincia(imprese_ID_ind);
                ascoltaProvincia(centro_ID_ind);
            });


            function impostaValoriIndirizzo(via, numero, frazione, citta, provincia, sigla, nazione, nazioneISO) {

                if (numero == 0) {
                    $('#txt_Via_' + imprese_ID_ind).val(via);
                    $('#txt_Via_' + centro_ID_ind).val(via);
                }
                else {
                    $('#txt_Via_' + imprese_ID_ind).val(via + ", " + numero);
                    $('#txt_Via_' + centro_ID_ind).val(via + ", " + numero);
                }

                $('#txt_Frazione_' + imprese_ID_ind).val(frazione);
                $('#txt_Frazione_' + centro_ID_ind).val(frazione);

                $('#txt_Stato_' + imprese_ID_ind).val(nazione);
                $('#txt_Stato_' + centro_ID_ind).val(nazione);

                $('#txt_ISO_Stato_' + imprese_ID_ind).val(nazioneISO);
                $('#txt_ISO_Stato_' + centro_ID_ind).val(nazioneISO);

                $('#ddl_Stato_' + imprese_ID_ind).val(nazioneISO);
                $('#ddl_Stato_' + centro_ID_ind).val(nazioneISO);


                inizializzaDDLProvincie('ddl_Provincia_', imprese_ID_ind);
                inizializzaDDLProvincie('ddl_Provincia_', centro_ID_ind);

                if (nazioneISO == 'IT') {
                    $('#ddl_Provincia_' + imprese_ID_ind).val(sigla);
                    $('#ddl_Provincia_' + centro_ID_ind).val(sigla);
                } else {
                    $('#ddl_Provincia_' + imprese_ID_ind).val("000");
                    $('#ddl_Provincia_' + centro_ID_ind).val("000");
                }

                caricaComune(imprese_ID_ind, citta);
                caricaComune(centro_ID_ind, citta);

                //se la città è vuota o nulla evidenzio in rosso
                if (!citta) {
                    console.log("Selezionare la città");
                    $('#txt_Frazione_' + imprese_ID_ind).css({ 'background-color': 'red' });
                    $('#txt_Frazione_' + centro_ID_ind).css({ 'background-color': 'red' });
                } else {
                    $('#txt_Frazione_' + imprese_ID_ind).css({ 'background-color': '' });
                    $('#txt_Frazione_' + centro_ID_ind).css({ 'background-color': '' });
                }

            }

            function ascoltaStato(id) {
                $('#ddl_Stato_' + id).change(function () {
                    $("#txt_Stato_" + id).val($('#ddl_Stato_' + id + ' option:selected').text());
                    $("#txt_ISO_Stato_" + id).val($('#ddl_Stato_' + id).val());
                });
            }

            function ascoltaProvincia(id) {
                $('#ddl_Provincia_' + id).change(function () {
                    var Chiave = $(this).val();
                    var pag;
                    pag = location.pathname.split("/")[location.pathname.split("/").length - 1] + "/CaricaComuni";

                    $.ajax({
                        type: "POST",
                        url: pag,
                        data: "{ Chiave: '" + Chiave + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            $('#ddl_Comune_' + id).html(msg.d);
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            alert(xhr.status);
                            alert(thrownError);
                        }
                    });
                });
            }


            function CaricaStatiISO(id, selezione) {

                var pag;
                pag = location.pathname.split("/")[location.pathname.split("/").length - 1] + "/CaricaStatiISO";
                $.ajax({
                    type: "POST",
                    url: pag,
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        $('#ddl_Stato_' + id).html(msg.d);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }

            function caricaComune(id, selezione) {
                var Chiave = $('#ddl_Provincia_' + id).val();
                var pag;
                pag = location.pathname.split("/")[location.pathname.split("/").length - 1] + "/CaricaComuni";
                $.ajax({
                    type: "POST",
                    url: pag,
                    data: "{ Chiave: '" + Chiave + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        $('#ddl_Comune_' + id).html(msg.d);
                        //seleziono
                        var i = 0;
                        $('#ddl_Comune_' + id + ' option').each(function () {
                            if ($(this).text().toString().toUpperCase() == selezione.toString().toUpperCase()) {
                                $(this).attr('selected', 'selected');
                            }
                        });

                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });
            }


            function inizializzaDDLProvincie(id, imprese_ID_ind) {
                var indirizzohttp = location.pathname;
                indirizzohttp = indirizzohttp.split("/")[indirizzohttp.split("/").length - 1];

                var ISO_Stato = $("#txt_ISO_Stato_" + imprese_ID_ind).val();
                utility.log("#txt_ISO_Stato_" + imprese_ID_ind);
                utility.log("ISO_Stato = " + ISO_Stato);

                $.ajax({
                    type: "POST",
                    url: indirizzohttp + "/CaricaProvince",
                    async: false,
                    cache: false,
                    data: "{'ISO_Stato': '" + ISO_Stato + "' }",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        $('#' + id + imprese_ID_ind).html(msg.d);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert(xhr.status);
                        alert(thrownError);
                    }
                });

            }



            function generaIndirizzo(tipo) {
                $('<label for="' + 'txt_Via_' + tipo + '">Via</label>').appendTo('#' + tipo);

                //via
                $('<input />', {
                    id: 'txt_Via_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //frazione
                $('<label for="' + 'txt_Frazione_' + tipo + '">Frazione</label>').appendTo('#' + tipo);
                $('<input />', {
                    id: 'txt_Frazione_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //provincia
                $('<label for="' + 'ddl_Provincia_' + tipo + '">Provincia</label>').appendTo('#' + tipo);
                $('<select />', {
                    id: 'ddl_Provincia_' + tipo
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //comuni
                $('<label for="' + 'ddl_Comune_' + tipo + '">Comune</label>').appendTo('#' + tipo);
                $('<select />', {
                    id: 'ddl_Comune_' + tipo
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //stato (ddl)
                $('<label for="' + 'ddl_Stato_' + tipo + '">Stato</label>').appendTo('#' + tipo);
                $('<select />', {
                    id: 'ddl_Stato_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //stato
                $('<label for="' + 'txt_Stato_' + tipo + '">Stato</label>').appendTo('#' + tipo);
                $('<input />', {
                    id: 'txt_Stato_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);

                //stato (sigla iso 3166)
                $('<div style="">').appendTo('#' + tipo);
                $('<label for="' + 'txt_ISO_Stato_' + tipo + '">Stato iso</label>').appendTo('#' + tipo);
                $('<input />', {
                    id: 'txt_ISO_Stato_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('</div>').appendTo('#' + tipo);

                //note
                $('<label for="' + 'txt_Note_' + tipo + '">Note</label>').appendTo('#' + tipo);
                $('<input />', {
                    id: 'txt_Note_' + tipo,
                    type: 'text'
                }).appendTo('#' + tipo);
                $('<br />').appendTo('#' + tipo);
            }
        </script>
        <!-- i18n -->
        <!-- Nuovo Centro Aziendale-->
        <div data-role="page" id="popup_nuovo_centro" title="Nuovo Centro Aziendale">
            <fieldset>
                <legend><asp:Localize meta:resourcekey="lblpop_up_centro" runat="server">Centro Aziendale</asp:Localize></legend>
                <div data-role="content">
                    <label for="txt_sa_nome" id="lbl_sa_nome">
                        <asp:Localize meta:resourcekey="lbl_sa_nome" runat="server">Nome:</asp:Localize>
                    </label>
                    <input type="text" id="txt_sa_nome" name="txt_sa_nome" value="" style="width: 295px" />
                    <br />
                    <br />
                    <div id="indirizzoCentro">
                    </div>
                    <div data-role="collapsible" id="Div3">
                        <span class="heading" id="par_Opzioni">
                            <asp:Localize meta:resourcekey="par_Opzioni" runat="server">Opzioni</asp:Localize>
                        </span>
                        <div class="content">
                        </div>
                    </div>
                </div>
            </fieldset>
        </div>

        <script language="javascript" type="text/javascript">

            function dialogBufferZoneIntersection_tools_Change() {

                //reset iniziale..
                $("#dialogBufferZoneIntersection_disegna").hide();
                $("#dialogBufferZoneIntersection_salva").hide();

                if ($("#dialogBufferZoneIntersection_tools").val() == 0) {
                    $("#dialogBufferZoneIntersection_disegna").show();
                } else {
                    $("#dialogBufferZoneIntersection_salva").show();
                }

            }

        </script>
        
        <!-- i18n -->
        <div id="dialogScomponiPunti" title="Strumento di scomposizione e ricomposizione poligoni">
            <div id="dialogScomponiPunti_Punti">
                <div id="dialogScomponiPuntiScelta1"><asp:Localize meta:resourcekey="dialogScomponiPuntiScelta1" runat="server">1. Punti selezionati</asp:Localize></div>
                <div id="btnDialogScomponiPunti_AnnullaVtx" class="bottone" onclick="DialogScomponiPunti_AnnullaVtx()" style="font-size: 0.9em;">
                    <asp:Localize meta:resourcekey="btnDialogScomponiPunti_AnnullaVtx" runat="server">Annulla ultimo</asp:Localize>
                </div>
                <div id="btnDialogScomponiPunti_AnnullaTutti" class="bottone" onclick="DialogScomponiPunti_AnnullaTutti()" style="font-size: 0.9em;">
                    <asp:Localize meta:resourcekey="btnDialogScomponiPunti_AnnullaTutti" runat="server">Annulla tutti</asp:Localize>
                </div>
            </div>
            <hr />
            <div id="dialogScomponiPunti_Associazione">
                <div id="dialogScomponiPuntiScelta2"><asp:Localize meta:resourcekey="dialogScomponiPuntiScelta2" runat="server">2. Seleziona un elemento</asp:Localize></div>
                <div id="btnDialogScomponiPunti_Associa" class="bottone" onclick="DialogScomponiPunti_Associa()" style="font-size: 0.9em;">
                    <asp:Localize meta:resourcekey="btnDialogScomponiPunti_Associa" runat="server">Genera poligono da punti selezionati</asp:Localize>
                </div>
            </div>
            <hr />
            <div id="dialogScomponiPunti_Fine">
                <div id="dialogScomponiPuntiScelta3"><asp:Localize meta:resourcekey="dialogScomponiPuntiScelta3" runat="server">3. Termina il lavoro</asp:Localize></div>
                <div id="btnDialogScomponiPunti_Termina" class="bottone" onclick="DialogScomponiPunti_Termina()" style="font-size: 0.9em;">
                    <asp:Localize meta:resourcekey="btnDialogScomponiPunti_Termina" runat="server">Concludi il lavoro</asp:Localize>
                </div>
            </div>
            <hr />
        </div>
        
        <!-- i18n -->
        <div id="dialogBufferZoneIntersection" title="Strumento di gestione fasce di rispetto non trattate">


            <div id="dialogBufferZoneIntersection_tool_bar">
                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <select id="dialogBufferZoneIntersection_tools" class="form-control" onchange="dialogBufferZoneIntersection_tools_Change()">
                            <option value="0" id="opt_bufferZoneAssocia"><asp:Localize meta:resourcekey="opt_bufferZoneAssocia" runat="server">Associazione dei dati</asp:Localize></option>
                            <option value="1" id="opt_bufferZoneDisegno"><asp:Localize meta:resourcekey="opt_bufferZoneDisegno" runat="server">Strumenti di disegno</asp:Localize></option>
                        </select>
                    </div>
                </div>
            </div>

            <div id="dialogBufferZoneIntersection_disegna"></div>


            <div id="dialogBufferZoneIntersection_salva">

                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12" id="dialogBufferZoneIntersection_DatoLetto" style="font-weight: bold"></div>
                </div>

                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12" id="div_bufferZoneDatoSalvato">
                        <asp:Localize meta:resourcekey="div_bufferZoneDatoSalvato" runat="server">Informazioni appezzamento che saranno memorizzate:</asp:Localize>
                    </div>
                </div>

                <!--
                Distanza (m) da :
                -	Corpi idrici superficiali
                -	Aree residenziali/pubbliche
                -	Allevamenti
                -	Vegetazione naturale/non coltivata

                -->

                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12" style="font-weight:bold" id="div_bufferZoneDistanza">
                        <asp:Localize meta:resourcekey="div_bufferZoneDistanza" runat="server">Distanza (m) da :</asp:Localize>
                    </div>
                </div>

                <div class="row">

                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <div class="input-group">
                            <label id="txtDistBZ_CorpiIdrici_a" class="input-group-addon">
                                <asp:Localize meta:resourcekey="txtDistBZ_CorpiIdrici_a" runat="server">Corpi idrici superficiali [m]</asp:Localize>
                            </label>

                            <input class="form-control" type="text" id="txtDistBZ_CorpiIdrici" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <div class="input-group">
                            <label id="txtDistBZ_AreeResPub_a" class="input-group-addon">
                                <asp:Localize meta:resourcekey="txtDistBZ_AreeResPub_a" runat="server">Aree residenziali/pubbliche [m]</asp:Localize>
                            </label>

                            <input class="form-control" type="text" id="txtDistBZ_AreeResPub" />
                        </div>
                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <div class="input-group">
                            <label id="txtDistBZ_Allevamenti_a" class="input-group-addon">
                                <asp:Localize meta:resourcekey="txtDistBZ_Allevamenti_a" runat="server">Allevamenti [m]</asp:Localize>
                            </label>

                            <input class="form-control" type="text" id="txtDistBZ_Allevamenti" />
                        </div>
                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <div class="input-group">
                            <label id="txtDistBZ_VegNatNonColt_a" class="input-group-addon">
                                <asp:Localize meta:resourcekey="txtDistBZ_VegNatNonColt_a" runat="server">Vegetazione naturale/non coltivata [m]</asp:Localize>
                            </label>

                            <input class="form-control" type="text" id="txtDistBZ_VegNatNonColt" />
                        </div>
                    </div>

                </div>

                <hr />

                <div class="row">

                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-12">
                        <div class="input-group">
                            <label id="lblBufferZoneSup" class="input-group-addon">
                                <asp:Localize meta:resourcekey="lblBufferZoneSup" runat="server">Superficie</asp:Localize>
                            </label>

                            <input class="form-control" type="text" id="txtSupBZ_Riduzione" />

                            <span class="input-group-btn" id="arrowBufferZone">
                                
                                <!-- i18n -->
                                <a class="form-control btn btn-secondary" style="border-bottom-right-radius: 4px; border-top-right-radius: 4px;" title="Gestione Colori layer principale">
                                    <span class="fa fa-arrow-left"></span>
                                    <span id="txtSupBZ_Riduzione_Ricalcolata"></span>
                                </a>
                            </span>




                        </div>
                    </div>

                </div>


            </div>
        </div>
        
        <!-- i18n -->
        <div id="pop_up_opAgenda_Preselezione" title="Selezione Operazione di Agenda">
            <div id="contentpop_up_opAgenda_Preselezione">
            </div>
        </div>
        <!-- Dialog Import
        <div id="pop_up_import_shape" title="Gestione sincronizzazioni ed importazioni">
            <iframe src="" id="frameShape" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0"></iframe>
        </div>
        <div id="pop_up_export_shape" title="Gestione esportazioni">
            <iframe src="" id="frameShapePF" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0"></iframe>
        </div>
        <div id="pop_up_Ricette" title="Gestione Ricette">
            <iframe src="" id="frameRicette" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0"></iframe>
        </div>
        <div id="pop_up_Catasto" title="Gestione Catasto">
            <iframe src="" id="frameCatasto" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0"></iframe>
        </div>

        <div id="pop_up_Ritaglia" title="Ritaglia Immagine">
            <iframe src="" id="frameRitaglia" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0" allowfullscreen></iframe>
        </div>
        <div id="pop_up_opAgenda" title="Operazione di Agenda">
            <iframe src="" id="framepop_up_opAgenda" style="width: 100%; height: 100%; display: block;
        border: 0" frameborder="0" allowfullscreen></iframe>
        </div>
        <div id="pop_up_Meteo" title="Analisi Dati">
            <iframe src="" id="framepop_up_Meteo" style="width: 100%; height: 100%; display: block;
            border: 0" frameborder="0" allowfullscreen></iframe>
        </div>
        <div id="pop_up_visite" title="Visite">
            <iframe src="" id="framepop_up_visite" style="width: 100%; height: 100%; display: block;
                border: 0" frameborder="0" allowfullscreen></iframe>
        </div>
        -->

    </div>
    
    
    <script language="javascript" type="text/javascript">

        $(".TerrenoNudo").addClass("Nascosto");

        function TerrenoNudoClick(sender) {
            if (sender.checked) {
                $(".ColturaPresente").addClass("Nascosto");
                $(".TerrenoNudo").removeClass("Nascosto");
            } else {
                $(".TerrenoNudo").addClass("Nascosto");
                $(".ColturaPresente").removeClass("Nascosto");
            }
        }
    </script>

</div>

