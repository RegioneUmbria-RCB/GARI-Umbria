<%@ Page Title="Gestione Allegati" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="GestioneAllegati.aspx.vb" Inherits="AgroAgenda_2010.GestioneAllegati" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% If Operazione <> 0 Then %>

    <div id="datiDocumento" class="row">

        <div class="col-lg-12 border_si">

            <div class="row">

                <div class="col-md-12 text-center">
                    <h4 style="color: #052747; text-transform: uppercase;">Dati Documento</h4>
                    <input type="hidden" id="Txt_ID_Elenco" value="0" />
                    <input type="hidden" id="Txt_ID_Alert_Entita" value="0" />
                    <input type="hidden" id="Txt_Allegati_Documenti_Cod" value="0" />
                </div>

                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Stato" for="Cmb_TipoDocumento">Tipo Documento</span>
                                <input type="text" id="Cmb_TipoDocumento" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-6">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info lbl_required" id="lbl_Documento" for="Txt_Num_Documento">N° Documento</span>
                                <input type="text" id="Txt_Num_Documento" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-6">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Documento_Ente" for="Txt_Ente_Rilascio">Ente Rilascio</span>
                                <input type="text" id="Txt_Ente_Rilascio" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-6">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info lbl_required" id="lbl_Documento_Rilascio" for="Txt_Data_Rilascio"><i class="fa fa-calendar"></i>Data Rilascio</span>
                                <input type="text" id="Txt_Data_Rilascio" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-md-6 col-sm-6">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info lbl_required" id="lbl_Documento_Scadenza" for="Txt_Data_Scadenza"><i class="fa fa-calendar"></i>Data Scadenza</span>
                                <input type="text" id="Txt_Data_Scadenza" class="form-control " />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Documento_Allegato" for="Btn_Sfoglia_Allegato">Allegato</span>
                                <span class="input-group-btn">
                                    <button id="Btn_Sfoglia_Allegato" class="btn btn-default" type="button" onclick="openFileDialogFinto();" style="margin-top: 0px;">
                                        <span class="fa fa-folder-open"></span>
                                    </button>
                                </span>
                                <input type="text" id="Txt_Documento_Allegato" class="form-control" readonly="readonly" required />
                                <input type="file" id="File_Allegato" onchange="scriviPercorsoFileSuTxt();" style="display: none;" />
                                <input type="hidden" id="File_Caricato" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <label class="input-group-addon alert-info" id="lbl_Descrizione" for="Txt_Descrizione>">Descrizione</label>
                                <textarea id="Txt_Descrizione" cols="20" class="form-control" aria-describedby="lbl_Descrizione" rows="2"></textarea>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

    </div>

    <div class="row">

        <div class="col-lg-12 text-right">
            <div class="btn btn-info" id="btn_salva_allegato">
                <i class="fa fa-plus"></i>Aggiungi
            </div>
            <div class="btn btn-success" id="btn_modifica_allegato" style="display: none;">
                <i class="fa fa-pencil"></i>Modifica
            </div>
            <div class="btn btn-default" id="btn_annulla_allegato" style="display: none;">
                <i class="fa fa-ban"></i>Annulla
            </div>
        </div>

    </div>

    <% End If %>

    <div class="row" style="margin-top: 20px;">
        <div class="col-lg-12">
            <div class="table-responsive">
                <input type="hidden" id="kendoAllegati" />
                <div id="gridAllegati">
                </div>
            </div>
        </div>
    </div>

    <iframe id="iframe" style="display: none;"></iframe>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneAllegati.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneAllegati_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GestioneAllegati_ws_client.js") %>"></script>

    <script type="text/javascript">
        var obj_Allegati = <%= objAllegati.ToString  %>;
        var operazione = <%= Operazione  %>;
        var kendoServer = <%= If(kendoServer, "true", "false")  %>;
        var fromPatentiniContatto = <%= If(fromPatentiniContatto, "true", "false")  %>;
    </script>

</asp:Content>
