<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="RegCaricoScaricoPassaporti.aspx.vb" Inherits="AgroAgenda_2010.RegCaricoScaricoPassaporti" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="RegCaricoScaricoPassaporti.css?<% =Application("GiasVersioneCorrente")%>" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="pannelloInfo">

        <img id="pannelloInfoImg" src="../AB_Immagini/Varie/LegendaRegistroBudPlant.jpg" style="display:none;height:100%" />
        <%--<table class="tabellaInfo">
            <tbody>


                
                <tr class="tabellaInfoIntestazione">
                    <td colspan="4">AGGIORNA MOVIMENTI (Aggiornamento automatico del Registro).</td>
                </tr>

                
                <tr>
                    <td class="kendoRiga_pp_orange" rowspan="2">&gt;riga arancione</td>
                    <td rowspan="2">Nuovo movimento da operazione fatta (conferimento, lavorazione, delivery, etc.).</td>
                    <td class="kendoRiga_pp_orange">icona confirm</td>
                    <td class="kendoRiga_pp_orange">SALVO su Registro</td>
                </tr>
                <tr>
                    <td class="kendoRiga_pp_orange" >icona bidone</td>
                    <td class="kendoRiga_pp_orange" >NON SALVO su Registro (non sarà ripresentata ai prossimi Aggiornamenti).</td>
                </tr>
                

                <tr>
                    <td class="kendoRiga_pp_green">&gt;riga verde</td>
                    <td rowspan="2">Movimento da operazione modificata.</td>
                    <td class="kendoRiga_pp_green">icona confirm</td>
                    <td class="kendoRiga_pp_green">SALVO su Registro la modifica.</td>
                </tr>

                <tr>
                    <td class="kendoRiga_pp_yellow">&gt;riga gialla</td>                    
                    <td class="kendoRiga_pp_yellow">icona bidone</td>
                    <td class="kendoRiga_pp_yellow">NON SALVO su Registro la modifica (non sarà ripresentata ai prossimi Aggiornamenti).</td>
                </tr>
                
                 <tr>
                    <td class="kendoRiga_pp_red_foreWhite" rowspan="2">&gt;riga rossa</td>
                    <td rowspan="2">Movimento da operazione cancellata.</td>
                    <td class="kendoRiga_pp_red_foreWhite">icona confirm</td>
                    <td class="kendoRiga_pp_red_foreWhite">CANCELLO relativa riga di Registro (non sarà ripresentata ai prossimi Aggiornamenti).</td>
                </tr>
                <tr>
                    <td class="kendoRiga_pp_red_foreWhite">icona bidone</td>
                    <td class="kendoRiga_pp_red_foreWhite">MANTENGO riga di Registro (non sarà ripresentata ai prossimi Aggiornamenti).</td>
                </tr>
                
                

                
                <tr class="tabellaInfoIntestazione">
                    <td colspan="4">RECUPERA RIGHE CANCELLATE O INCONGRUENTI (Aggiornamento automatico del Registro).</td>
                </tr>
                <tr>
                    <td><span>Colorazioni come sopra.</span></td>
                    <td >Come sopra per recuparare i movimenti non coerenti con il Registro.</td>
                    <td class="">Come sopra.</td>
                    <td class="">Come sopra (non sarà più presentata né agli Aggiornamenti né al prossimo Recupero).</td>
                </tr>

                
                <tr class="tabellaInfoIntestazione">
                    <td colspan="4">+ Aggiungi nuovo elemento (inserimento manuale di riga nel Registro).</td>
                </tr>
                <tr>
                    <td><span>&gt;riga bianca</span></td>
                    <td >Inserimento di riga direttamente in Registro.</td>
                    <td class="">Salva le modifiche</td>
                    <td class="">Nuova riga di Registro inserita direttamente con data entry.</td>
                </tr>
            </tbody>
        </table>--%>

    </div>

    <div class="container" style="margin-bottom: 70px">
        <div class="row" style="margin-top: 15px">
            <div id="frmTestata" class="form-group">
                <div class="row">
                    <div class="col-lg-3">
                        <div class="input-group" id="Data_DA">
                            <label class="input-group-addon control-label " id="lbl_Data_DA" for="Txt_Data_DA">
                                <asp:Localize meta:resourcekey="lbl_Data_DA" runat="server">Dal:</asp:Localize>
                            </label>
                            <input class="form-control kendoDatePicker" id="Txt_Data_DA" aria-describedby="lbl_Data_DA" type="text" />
                        </div>
                    </div>
                    <div class="col-lg-3">
                        <div class="input-group" id="Data_A">
                            <label class="input-group-addon control-label " id="lbl_Data_A" for="Txt_Data_A">
                                <asp:Localize meta:resourcekey="lbl_Data_A" runat="server">Al:</asp:Localize></label>
                            <input class="form-control kendoDatePicker" id="Txt_Data_A" aria-describedby="lbl_Data_A" type="text" />
                        </div>
                    </div>

                </div>
                <div class="row">

                    <asp:Button ID="BTN_Date" runat="server" Text="Button" Style="display: none" />

                    <asp:Panel CssClass="col-lg-2" ID="divBtnElabora" runat="server">
                        <div class="btn btn-success" id="btn_ricerca">
                            <span class="fa fa-cog lampeggiante"></span><span class="lampeggiante">
                                <asp:Localize meta:resourcekey="lbl_Elabora_Dati" runat="server">Aggiorna Movimenti</asp:Localize></span>
                        </div>
                    </asp:Panel>


                    <asp:Panel CssClass="col-lg-2" ID="divBtnSampalibera" runat="server">
                        <div class="btn btn-success" id="btn_stampalibera">
                            <span class="fa fa-print lampeggiante"></span><span class="lampeggiante">
                                <asp:Localize meta:resourcekey="lbl_Stampa_Libera" runat="server">Etichetta Vuota</asp:Localize></span>
                        </div>
                    </asp:Panel>

                    <asp:Panel CssClass="col-lg-4" ID="divBtnRecuperaRighe" runat="server">
                        <div class="btn btn-success" id="btn_recupera">
                            <span class="fa fa-cog lampeggiante"></span><span class="lampeggiante">
                                <asp:Localize meta:resourcekey="lbl_Recupera" runat="server">RECUPERA RIGHE CANCELLATE O INCONGRUENTI</asp:Localize></span>
                        </div>
                    </asp:Panel>
                    <asp:Panel CssClass="col-lg-4" ID="divBtnInfo" runat="server">
                        <div class="btn btn-success" id="btn_info">
                            <span class="fa fa-question lampeggiante"></span><span class="lampeggiante">
                                <asp:Localize meta:resourcekey="lbl_info" runat="server">INFO</asp:Localize></span>
                        </div>
                    </asp:Panel>
                </div>

                <div class="row">
                    <div class="col-lg-3">
                        <div id="messaggi" style="margin: 13px; font-weight: bold; color: red;"></div>
                    </div>
                </div>


            </div>
        </div>


        <div id="kendoRegistroPassaporti">
        </div>
        <input type="hidden" id="hdKendoRegistroPassaporti" />

    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">


    <script type="text/javascript">
        var piva = "<%= piva %>";
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RegCaricoScaricoPassaporti.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RegCaricoScaricoPassaportiKendo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RegCaricoScaricoPassaporti_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RegCaricoScaricoPassaporti_jQueryDocReady.js") %>"></script>

</asp:Content>
