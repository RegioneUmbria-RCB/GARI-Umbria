<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="MenuPrincipaleBS.aspx.vb" Inherits="AgroAgenda_2010.MenuPrincipaleBS" %>

<%@ Import Namespace="AgronicaCoreDataProvider.TipiEnumerativi" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>
<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    
    <div class="container nopadding" id="menu_principale">
        <div class="row">
            <!-- Colonna SX -->
            <div class="col-lg-4 col-md-6 col-sm-12" style="margin-bottom: 100px;">
                <div id="container_menu2" class="row">
                    <div id="container_semaforo" class="col-lg-12 text-center" style="border-bottom: 3px double #ddd;">
                        <span>
                            <img id="Semaforo_DPI" src="../AB_Immagini/Icone24/Sfera_Rossa_24.ico" height="16"
                                runat="server">DPI
                        </span> 
                        <span>
                            <img id="Semaforo_Fito" src="../AB_Immagini/Icone24/Sfera_Rossa_24.ico" height="16"
                                        runat="server">Fito 
                        </span>
                        <span>
                            <img id="Semaforo_Meteo" src="../AB_Immagini/Icone24/Sfera_Rossa_24.ico" height="16"
                                                runat="server">Meteo 
                        </span>
                        <span>
                            <img id="Semaforo_CAP" src="../AB_Immagini/Icone24/Sfera_Rossa_24.ico" height="16"
                                                        runat="server">Cap.Cli.
                        </span>
                    </div>
                    <div id="container_search" class="col-lg-12 text-center">   
                        <asp:TextBox ID="TxtSearch" runat="server" CssClass="form-control" style="width: 70%; float: left;">
                        </asp:TextBox>
                        <div class="btn btn-default" id="btn_search" style="float: right; margin-top: 0;">
                            <i class="fa fa-search"></i> Cerca
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                    
                </div>

                <div id="container_inbox" class="row">
                    <div class="col-lg-12" style="padding-top: 5px;">
                         <span style="color: #052747; text-transform:uppercase; font-size: 18px;"><i class="fa fa-inbox"></i> Inbox</span>
                         <a href="../pannellodicontrollo/pannellodicontrollo.aspx"><span style="color: #052747; text-transform:uppercase; font-size: 18px; float:right;"><i class="fa fa-bars"></i></span></a>
                    </div>
                    <div class="col-lg-12" style="padding-top: 5px;">
                        <hr style="margin: 0;" />
                    </div>

                    <div id="container_inbox_body" class="col-lg-12">

                         <!-- Nav tabs -->
                        <ul class="nav nav-tabs" role="tablist">
                            <li role="presentation"><a href="#scadenze" aria-controls="home" role="tab" data-toggle="tab">Scadenze <div class="etichetta_tot">6</div></a></li>
                            <li role="presentation"><a href="#non_conformita" aria-controls="profile" role="tab" data-toggle="tab">Non Conformità <div class="etichetta_tot">16</div></a></li>
                        </ul>

                        <!-- Tab panes -->
                        <div class="tab-content">
                            <div role="tabpanel" class="tab-pane fade" id="scadenze">
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015 15:22</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Categoria 1</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>           
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Categoria</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                   
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Categoria 2</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                           
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane fade" id="non_conformita">
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015 15:22</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                           
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="mittente"> - <i class="fa fa-file-text"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>

                <div id="container_preferiti" class="row">
                    <div class="col-lg-12" style="padding-top: 5px;">
                         <span style="color: #fff; text-transform:uppercase; font-size: 18px;"><i class="fa fa-star"></i> Preferiti</span>
                         <a href="#" onclick="GestisciPreferiti();"><span style="color: #fff; text-transform:uppercase; font-size: 18px; float:right;"><i class="fa fa-plus-circle"></i></span></a>
                    </div>
                    <div class="col-lg-12" style="padding-top: 5px;">
                    <hr style="margin: 0;" />
                    </div>
                    <div id="container_preferiti_body" class="col-lg-12" style="padding-top: 10px; padding-bottom: 10px;">
                    </div>
                </div>

                
                <div id="container_comunicazioni" class="row">
                    <div class="col-lg-12" style="padding-top: 5px;">
                         <span style="color: #052747; text-transform:uppercase; font-size: 18px;"><i class="fa fa-bookmark"></i> Comunicazioni</span>
                         <a href="#"><span style="color: #052747; text-transform:uppercase; font-size: 18px; float:right;"><i class="fa fa-envelope"></i></span></a>
                    </div>
                    <div class="col-lg-12" style="padding-top: 5px;">
                        <hr style="margin: 0;" />
                    </div>
                    <div id="container_comunicazioni_body" class="col-lg-12">

                        <!-- Nav tabs -->
                        <ul class="nav nav-tabs" role="tablist">
                            <li role="presentation" class="active"><a href="#interna" aria-controls="home" role="tab" data-toggle="tab">Interne</a></li>
                            <li role="presentation"><a href="#agronica" aria-controls="profile" role="tab" data-toggle="tab">Agronica</a></li>
                            <li role="presentation"><a href="#logs" aria-controls="messages" role="tab" data-toggle="tab">Logs</a></li>
                        </ul>

                        <!-- Tab panes -->
                        <div class="tab-content">
                            <div role="tabpanel" class="tab-pane fade in active" id="interna">
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015 15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane fade" id="agronica">
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015 15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane fade" id="logs">
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015 15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                                <div class="message">
                                    <div class="row message_head">
                                        <div class="col-lg-12">
                                            <span class="data">16/10/2015</span>
                                            <span class="orario">15:22</span>
                                            <span class="mittente"> - <i class="fa fa-user"></i> Mittente</span>
                                        </div>
                                    </div>
                                    <div class="row message_body">
                                        <div class="col-lg-12">
                                            <div class="titolo"><a href="#">titolo</a></div>
                                            <div class="intro">Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec nulla neque, tempus at sem ut, malesuada tempus mauris. Suspendisse nec urna quis nulla cursus iaculis vel et metus. Interdum et malesuada fames ac ante ipsum primis in faucibus</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

   
                    </div>
                </div>

            </div> <!-- end SX -->

            <!-- Colonna DX -->
            <div class="col-lg-8 col-md-6 col-sm-12">
                <div id="container_banner" class="row" style="padding: 15px;">
                    Banner pubblicitari
                </div>

                <!-- Navigazione Mobile -->
                <div class="row">
                    <nav class="navbar navbar-default" style="display: none;">
                      <div class="container-fluid">
                        <div class="navbar-header">
                          <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                            <span class="sr-only">Toggle navigation</span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                          </button>
                        </div>

                        <div class="collapse navbar-collapse" id="menu_principale">
                          <ul class="nav navbar-nav">
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                            <li><a href="#">Link</a></li>
                <%--            <li class="active"><a href="#">Link <span class="sr-only">(current)</span></a></li>
                            <li><a href="#">Link</a></li>
                            <li class="dropdown">
                              <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">Dropdown <span class="caret"></span></a>
                              <ul class="dropdown-menu">
                                <li><a href="#">Action</a></li>
                                <li><a href="#">Another action</a></li>
                                <li><a href="#">Something else here</a></li>
                                <li role="separator" class="divider"></li>
                                <li><a href="#">Separated link</a></li>
                                <li role="separator" class="divider"></li>
                                <li><a href="#">One more separated link</a></li>
                              </ul>
                            </li>--%>

                          </ul>
                        </div>
                      </div>
                    </nav>
                </div>

                <!-- Navigazione Desktop -->
                <div class="row">
                    <div class="col-lg-12 col-md-12 col-sm-12" style="padding: 15px; margin-bottom: 75px;">
                        <div id="container_menu" class="row">

                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_green">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Anagrafica e Catasto Aziende</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_green">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Agenda Operazioni Colturali, Contabili e Zootecniche</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_green">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione Magazzini</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_blue">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione Contatti<br />(Clienti / Fornitori / Lavoratori / Terzisti)</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_blue">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione Consistenze Zootecniche</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_blue">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Contabilità<br />(CO.GE.)</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_blue">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">DSS - Supporti Descisionali</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_blue">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Cartografia Aziendale</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_orange">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Registrazioni<br />Light / Smart</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_orange">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Analisi<br />Produzioni / Costi / Ricavi</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_orange">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione<br />Conferimenti / Accettazione Prodotto</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Pannello di Controllo Qualità</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione<br />Documentale / Scadenziario</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Agricoltura Biologica</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione Processi<br />Post-Raccolta</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Gestione Utenti e Permessi</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Manutenzioni & Utility</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Messaggistica</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">                         
                                <a href="../AlberoImprese/AlberoImprese.aspx">
                                    <div class="bg_white" style="height:72px; ">
                                        <div class="icona bg_purple">
                                            <img src="../AB_Immagini/icone32/impresa.ico" />
                                        </div>
                                        <div class="titolo">
                                            <h5 style="display: inline-block;">Elaborazioni<br />Stampe / Statistiche</h5>
                                        </div>
                                    </div>
                                </a>  
                            </div>

                        
                            
                          <%--  <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Agenda Operazioni Colturali, Contabili e Zootecniche</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione Magazzini</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione Contatti<br />(Clienti / Fornitori / Lavoratori / Terzisti)</h5>
                                    </a>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione Consistenze Zootecniche</h5>
                                    </a>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Contabilità<br />(CO.GE.)</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>DSS - Supporti Descisionali</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Cartografia Aziendale</h5>
                                    </a>
                                </div>
                            </div>

   
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Registrazioni<br />Light / Smart</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Analisi<br />Produzioni / Costi / Ricavi</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione<br />Conferimenti / Accettazione Prodotto</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Pannello di Controllo Qualità</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione<br />Documentale / Scadenziario</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Agricoltura Biologica</h5>
                                    </a>
                                </div>
                            </div>

     
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione Processi<br />Post-Raccolta</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center nopadding">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Gestione Utenti e Permessi</h5>
                                    </a>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Manutenzioni & Utility</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Messaggistica</h5>
                                    </a>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 text-center">
                                <div>
                                    <a href="../AlberoImprese/AlberoImprese.aspx">
                                        <img src="../AB_Immagini/icone32/impresa.ico" />
                                        <h5>Elaborazioni<br />Stampe / Statistiche</h5>
                                    </a>
                                </div>
                            </div>
                    --%>

                        </div> 
                    </div>
                </div>


            </div> <!-- end DX --> 
        
        </div>
        


    </div> <!-- end container -->


    <!-- Dialog Gestione preferiti -->
    <div class="modal fade" id="modalPreferiti">
        <div class="modal-dialog">
            <div class="modal-content">
                <form id="form_nuovo_elemento" method="get" action="">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="lbl_new_item">
                            Configura i tuoi preferiti
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-lg-12" data-role="fieldcontain">
                                <label class="col-lg-3" for="<%=ddl_preferito1.ClientID %>">
                                    <asp:Label ID="lbl1" runat="server" meta:resourcekey="lblPreferitoResource1">Preferito</asp:Label>
                                    1
                                </label>
                                <asp:DropDownList ID="ddl_preferito1" runat="server" CssClass="col-lg-9">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-12" data-role="fieldcontain">
                                <label class="col-lg-3" for="<%=ddl_preferito2.ClientID %>">
                                    <asp:Label ID="Label1" runat="server" meta:resourcekey="lblPreferitoResource1">Preferito</asp:Label>
                                    2
                                </label>
                                <asp:DropDownList ID="ddl_preferito2" runat="server" CssClass="col-lg-9">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-12" data-role="fieldcontain">
                                <label class="col-lg-3" for="<%=ddl_preferito3.ClientID %>">
                                    <asp:Label ID="Label2" runat="server" meta:resourcekey="lblPreferitoResource1">Preferito</asp:Label>
                                    3
                                </label>
                                <asp:DropDownList ID="ddl_preferito3" runat="server" CssClass="col-lg-9">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-12" data-role="fieldcontain">
                                <label class="col-lg-3" for="<%=ddl_preferito4.ClientID %>">
                                    <asp:Label ID="Label3" runat="server" meta:resourcekey="lblPreferitoResource1">Preferito</asp:Label>
                                    4
                                </label>
                                <asp:DropDownList ID="ddl_preferito4" runat="server" CssClass="col-lg-9">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-12" data-role="fieldcontain">
                                <label class="col-lg-3" for="<%=ddl_preferito5.ClientID %>">
                                    <asp:Label ID="Label4" runat="server" meta:resourcekey="lblPreferitoResource1">Preferito</asp:Label>
                                    5
                                </label>
                                <asp:DropDownList ID="ddl_preferito5" runat="server" CssClass="col-lg-9">
                                </asp:DropDownList>
                            </div>
      
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-success" ID="btn_salva_preferiti" onclick="">
                            <i class="fa fa-plus"></i>
                            Salva</button>

                    </div>
                </form>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        $(document).bind("mobileinit", function () {
            ajaxFormsEnabled = false,
            ajaxLinksEnabled = false,
            ajaxEnabled = false
        });
    </script>
    <script type="text/javascript" src="../Scripts/jMsAjax.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../Scripts/WS.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript">
        var p2 = '../Ajax/ajax.asmx/';

        var utente = '<%=Username %>';
        var password = '<%=Password %>';
        var vDPI_1 = '<%=DPI_1 %>';
        var vDPI_2 = '<%=DPI_2 %>';
        var vFITO_1 = '<%=FITO_1 %>';
        var vFITO_2 = '<%=FITO_2 %>';
        var vMETEO_1 = '<%=METEO_1 %>';
        var vMETEO_2 = '<%=METEO_2 %>';
        var vCAP_1 = '<%=CAP_1 %>';
        var vCAP_2 = '<%=CAP_2 %>';
        var t_out = '<%=TimeOut %>';

        //************ DPI
        function Imposta_DPI1() {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_DPI1",
                success: function (msg) {
                    $('#<%=Semaforo_DPI.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        function Imposta_DPI2(connessione) {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_DPI2",
                success: function (msg) {
                    $('#<%=Semaforo_DPI.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }

        function DPI_2() {
            //interrogo per la seconda connessione
            var url_DPI = $.jmsajaxurl({
                url: vDPI_2,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2, NomeUtente: utente, Password: password }
            });

            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testDPI',
                url: url_DPI,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_DPI2();
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }

        //************ FITO
        function Imposta_FITO1() {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_FITO1",
                success: function (msg) {
                    $('#<%=Semaforo_Fito.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        function Imposta_FITO2(connessione) {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_FITO2",
                success: function (msg) {
                    $('#<%=Semaforo_Fito.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        function FITO_2() {
            //interrogo per la seconda connessione
            var url_FITO = $.jmsajaxurl({
                url: vFITO_2,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2, NomeUtente: utente, Password: password }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testFITO',
                url: url_FITO,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_FITO2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }


        //************ METEO
        function Imposta_METEO1() {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_METEO1",
                success: function (msg) {
                    $('#<%=Semaforo_Meteo.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }
        function Imposta_METEO2(connessione) {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_METEO2",
                success: function (msg) {
                    $('#<%=Semaforo_Meteo.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        function METEO_2() {
            //interrogo per la seconda connessione
            var url_METEO = $.jmsajaxurl({
                url: vMETEO_2,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2 }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'test',
                url: url_METEO,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_METEO2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }

        //************ CAP
        function Imposta_CAP1() {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_CAP1",
                success: function (msg) {
                    $('#<%=Semaforo_CAP.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }
        function Imposta_CAP2(connessione) {
            $.ajax({
                type: "POST", data: "{}", contentType: "application/json; charset=utf-8", dataType: "json",
                url: p2 + "ok_CAP2",
                success: function (msg) {
                    $('#<%=Semaforo_CAP.ClientID %>').attr('src', "../AB_Immagini/Icone24/Sfera_Verde_24.ico");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        function CAP_2() {
            //interrogo per la seconda connessione
            var url_CAP = $.jmsajaxurl({
                url: vCAP_2,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2 }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testCAP',
                url: url_CAP,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_CAP2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }



        //init
        function CheckWebService() {
            //DPI
            var url_DPI = $.jmsajaxurl({
                url: vDPI_1,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2, NomeUtente: utente, Password: password }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testDPI',
                url: url_DPI,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_DPI1();
                    }
                    else {
                        DPI_2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    DPI_2();
                }
            });

            //FITO
            var url_Fito = $.jmsajaxurl({
                url: vFITO_1,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2, NomeUtente: utente, Password: password }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testFITO',
                url: url_Fito,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_FITO1();
                    } else {
                        FITO_2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    FITO_2();
                }
            });

            //METEO             
            var url_METEO = $.jmsajaxurl({
                url: vMETEO_1,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2 }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'test',
                url: url_METEO,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_METEO1();
                    }
                    else {
                        METEO_2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    METEO_2();
                }
            });


            //CAP             
            var url_CAP = $.jmsajaxurl({
                url: vCAP_1,
                method: "Verifica_Collegamento_new",
                data: { Num_1: 1, Num_2: 2 }
            });
            $.ajax({
                type: "GET",
                contentType: "application/javascript; charset=utf-8",
                dataType: "jsonp",
                timeout: t_out,
                async: false,
                jsonpCallback: 'testCAP',
                url: url_CAP,
                success: function (msg) {
                    if (msg.d == 3) {
                        Imposta_CAP1();
                    }
                    else {
                        CAP_2();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    CAP_2();
                }
            });
        }


        function test(obj) {
        }
        function testDPI(obj) {
        }
        function testFITO(obj) {
        }
        function testCAP(obj) {
        }


        function GestisciPreferiti() {
            $('#modalPreferiti').modal('show');
        }


        $(document).ready(function () {
            CheckWebService();
        });
    </script>
    <script language="javascript" type="text/javascript">

        function getUrlVars() {
            var vars = {};
            var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
                vars[key] = value;
            });
            return vars;
        }


        function controllaMessaggi() {
            var m = getUrlVars()["m"];
            if (m !== null)
                if (m !== undefined) {
                    m = m.toString().replace(/%20/g, " ");
                    alert(m);
                    location.replace('Step_1.aspx')
                }
        }


        $(document).ready(function () {

            MostraNotifiche();

            controllaMessaggi();
        });


        //SEZIONE NOTIFICHE DA AGRONICA

        function MostraNotifiche() {
            var p2 = 'Menu.aspx/MostraNotifiche';
            $.ajax({
                type: "POST",
                url: p2,
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d != "") {
                        alertify.set({ delay: 10000 });
                        alertify.success(msg.d);
                        return false;
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        };
        $(document).ready(function () {
            $(document.body).on("click", '#alertify-logs', function () {
                var p2 = 'Menu.aspx/NascondiNotifiche';
                $.ajax({
                    type: "POST",
                    url: p2,
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d != false) {
                            alertify.set({ delay: 1000 });
                            alertify.success("il messaggio non verrà più visualizzato");
                            return false;
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    }
                });
            });


            // Carico i preferiti
            $.ajax({
                type: 'POST',
                url: 'MenuPrincipaleBS.aspx/Carica_Preferiti',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    //alert(r.d);
                    $('#container_preferiti_body').empty();
                    $('#container_preferiti_body').html(r.d);
                }
            });

            // Salvo i preferiti
            $(document.body).on("click", '#btn_salva_preferiti', function () {
                $.ajax({
                    type: 'POST',
                    url: 'MenuPrincipaleBS.aspx/Salva_Preferiti',
                    data: "{pref1:'" + $('#<%=ddl_preferito1.ClientID%>').val() + "', pref2:'" + $('#<%=ddl_preferito2.ClientID%>').val() + "', pref3:'" + $('#<%=ddl_preferito3.ClientID%>').val() + "', pref4:'" + $('#<%=ddl_preferito4.ClientID%>').val() + "', pref5:'" + $('#<%=ddl_preferito5.ClientID%>').val() + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        $('#modalPreferiti').modal('hide');
                        location.reload();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Errore nel salvataggio dei preferiti');
                    }
                });
            });

        });

        //SEZIONE NOTIFICHE DA AGRONICA
        
    </script>
</asp:Content>
