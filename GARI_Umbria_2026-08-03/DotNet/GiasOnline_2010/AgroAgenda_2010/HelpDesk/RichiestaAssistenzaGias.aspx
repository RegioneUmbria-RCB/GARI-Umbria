<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RichiestaAssistenzaGias.aspx.vb"
    Inherits="AgroAgenda_2010.RichiestaAssistenzaGias" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div id="panelArea" class="panel-group searchArea">

        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">

                <div class="jumbotron" style="padding-bottom: 0;">
                    <div class="container" style="width: 100%;">
                        <div class="container_tabIndice" style="padding: 0px; /*margin-bottom: 70px*/">
                            <div class="panel-group Indice">
                                <div class="row">


                                </div>

                                <div class="row">                                

                                </div>

                                <div class="row">
                                    
                                </div>


                                <%--controlli Bart--%>
                                <div class="row">
                                    <%--1 Combo Azienda----%>
                                    <div class="col-lg-6 col-md-6 col-sm-12" id="id_div_Azienda">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" for="cmbAzienda">Azienda:</label>
                                                    <select name="cmbAzienda" id="cmbAzienda" class="form-control"></select>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <%--2 Combo Motivo richiesta--%>
                                    <div class="col-lg-6 col-md-6 col-sm-12" id="id_div_motivoRichiesta">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" for="cmbMotivoRichiesta">Motivo della richiesta:</label>
                                                    <select name="cmbMotivoRichiesta" id="cmbMotivoRichiesta" class="form-control"></select>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <%--3 Combo Richiesta Banche Dati--%>
                                    <div class="col-lg-6 col-md-6 col-sm-12" id="id_div_RichiestaBancheDati">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" id="lbl_RichiestaBancheDati" for="cmbRichiestaBancheDati">Richiesta inserimento su banche dati:</label>
                                                    <select name="cmbRichiestaBancheDati" id="cmbRichiestaBancheDati" class="form-control"></select>
                                                </div>
                                            </div>
                                        </div>
                                    </div>                                    
                                </div>
                                <%--4 Text box a scomparsa:compare solo se non è selezionata banche dati in  TipoMotivoRichiesta --%>
                                <div class="row">
                                    <div class="col-lg-6 col-md-6 col-sm-12">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <label class="input-group-addon lbl_required" id="lbl_text_nbc" for="text_nbc">Specificare i dettagli:</label>
                                                    <asp:TextBox ID="text_nbc" runat="server" CssClass="form-control" >
                                                    </asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <%--Bottone invia mail--%>
                                <div class="row">
                                    <div class="btn btn-success buttonClass" id="btn_invia_email" style="margin-left: 10px;">
                                        <span class="fa fa-send"></span>Invia Richiesta
                                    </div>
                                </div>

                                
                            
                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>


    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiestaAssistenzaGias_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiestaAssistenzaGias.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiestaAssistenzaGias_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RichiestaAssistenzaGias_ws_client.js") %>"></script>
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var objP_utenti = '<%=objparametri_utenti_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
    </script>

</asp:Content>
