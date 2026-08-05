<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="UtilityModificaChiavi.aspx.vb" Inherits="AgroAgenda_2010.UtilityModificaChiavi" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="panel-group">
        <div class="col-lg-12" id="tabstripUtilityModificaChiavi" style="margin-top: 20px;">
            <ul>
                <li id="li_cambio_piva_UtilityModificaChiavi">Utility Cambio PIVA</li>
                <li id="li_cambio_CF_piva_contatto_UtilityModificaChiavi">Utility Cambio CF/Piva Contatto</li>
                <li id="li_cambio_CF_utente_UtilityModificaChiavi">Utility Cambio CF Utente</li>
            </ul>

            <!--Tab Cambio PIVA-->
            <div id="tab_cambio_piva_UtilityModificaChiavi">

                <!--Drop Down Impresa-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_impresa_cambio_CF_utente_UtilityModificaChiavi">Impresa:</span>
                                    <input type="text" id='ddl_impresa_cambio_piva_UtilityModificaChiavi' class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--TextBox Nuova Partita IVA-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div id="div_nuova_PIVA_cambio_piva_UtilityModificaChiavi" class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_nuova_PIVA_cambio_piva_UtilityModificaChiavi">Partita IVA nuova:</span>
                                    <input type="text" id="txt_nuova_PIVA_cambio_piva_UtilityModificaChiavi" class="form-control " />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Bottone di Salvataggio Cambio PIVA-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12" id="divSalva_cambio_piva_UtilityModificaChiavi">
                        <div class="btn btn-success btn_100" onclick="Conferma_cambio_piva_UtilityModificaChiavi();">
                            <i class="fa fa-floppy-o"></i>Modifica
                        </div>
                    </div>
                </div>
            </div>

            <!--Tab Cambio CF/Piva Contatto-->
            <div id="tab_cambio_CF_piva_contatto_UtilityModificaChiavi">

                <!--Note Drop Down Imprese con Contatti-->
                <div class="row">
                    <div id="Note_ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi">
                        <div class="col-lg-8 col-md-8 col-sm-12" style="padding-bottom: 10px">
                            <i aria-hidden="true"></i><strong>Seleziona un'impresa dall'elenco delle Imprese che hanno dei Contatti:</strong>
                        </div>
                    </div>
                </div>

                <!--Drop Down Imprese con Contatti-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi">Impresa:</span>
                                    <input type="text" id='ddl_impresa_cambio_CF_piva_contatto_UtilityModificaChiavi' class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Note Drop Down Imprese con Contatti-->
                <div class="row">
                    <div id="Note_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi">
                        <div class="col-lg-8 col-md-8 col-sm-12" style="padding-bottom: 10px">
                            <i aria-hidden="true"></i><strong>Elenco dei Contatti creati dall'impresa selezionata:</strong>
                        </div>
                    </div>
                </div>

                <!--Drop Down Contatti-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div id="div_ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi" class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi">Contatto:</span>
                                    <input type="text" id='ddl_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi' class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--TextBox Nuovo Codice Fiscale/PIVA-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div id="div_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi" class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi">Codice Fiscale Nuovo:</span>
                                    <input type="text" id="txt_nuovo_CF_PIVA_cambio_CF_piva_contatto_UtilityModificaChiavi" class="form-control " />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Bottone di Salvataggio Cambio CF/Piva Contatto-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12" id="divSalva_cambio_CF_piva_contatto_UtilityModificaChiavi">
                        <div class="btn btn-success btn_100" onclick="Conferma_cambio_CF_piva_contatto_UtilityModificaChiavi();">
                            <i class="fa fa-floppy-o"></i>Modifica
                        </div>
                    </div>
                </div>
            </div>

            <!--Tab Cambio CF Utente-->
            <div id="tab_cambio_CF_utente_UtilityModificaChiavi">

                <!--Drop Down Utente-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_utente_cambio_CF_utente_UtilityModificaChiavi">Utente:</span>
                                    <input type="text" id='ddl_utente_cambio_CF_utente_UtilityModificaChiavi' class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <!--TextBox Nuovo Codice Fiscale-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12">
                        <div id="div_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi" class="form-horizontal">
                            <div class="form-group">
                                <div class="input-group">
                                    <span class="input-group-addon alert-info" id="lbl_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi">Codice Fiscale Nuovo:</span>
                                    <input type="text" id="txt_nuovo_CF_cambio_CF_utente_UtilityModificaChiavi" class="form-control " />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--Bottone di Salvataggio Cambio CF Utente-->
                <div class="row">
                    <div class="col-lg-8 col-md-8 col-sm-12" id="divSalva_cambio_CF_utente_UtilityModificaChiavi">
                        <div class="btn btn-success btn_100" onclick="Conferma_cambio_CF_utente_UtilityModificaChiavi();">
                            <i class="fa fa-floppy-o"></i>Modifica
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <input type="hidden" id="hf_Type" runat="server" />

    <input type="hidden" id="hf_Piva" runat="server" />

    <input type="hidden" id="hf_UtenteAbilitatoLettura_Cambio_Piva_Impresa" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura_Cambio_Piva_Impresa" runat="server" />

    <input type="hidden" id="hf_UtenteAbilitatoLettura_Cambio_CF_Piva_Contatto" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto" runat="server" />

    <input type="hidden" id="hf_UtenteAbilitatoLettura_Cambio_CF_Utente" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura_Cambio_CF_Utente" runat="server" />

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UtilityModificaChiavi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UtilityModificaChiavi_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UtilityModificaChiavi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("UtilityModificaChiavi_ws_client.js") %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggiArrayCostanti.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script>
    </script>

</asp:Content>
