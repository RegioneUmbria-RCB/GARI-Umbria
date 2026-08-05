<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Modifica_Multipla_Zoo.aspx.vb" Inherits="AgroAgenda_2010.Modifica_Multipla_Zoo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .window-footer {
            position: absolute;
            bottom: 0;
            display: block;
            width: 95%;
            margin-top: 150px;
            padding: 19px 0 20px;
            text-align: right;
            border-top: 1px solid #e5e5e5;
        }

        .window-content {
            overflow: auto;
            height: calc(100% - 90px);
            padding: 10px
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hidden_modificaMultipla" runat="server" />
    <asp:HiddenField runat="server" ID="hdKendoModifica_Multipla_Zoo" />
    <asp:HiddenField ID="hidden_presetColumnsToUpdate" runat="server" />
    <input type="hidden" id="hf_UtenteAbilitatoScrittura" runat="server" />

    <div class="row">
        <div class="col-xs-12">
            <div id="divKendoModifica_Multipla_Zoo" style="overflow: auto; margin-top: 10px; margin-bottom: 10px;"></div>
        </div>
    </div>   

    <div id="winModificaMultipla" style="display: none">

        <div class="window-content">

            <div class="row">
                <div id="scelta_parametro">
                    <p id="avvertimentoModificaMultipla" style="font-weight: bold"></p>
                    
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriDaModificare %>" runat="server">Parametri da Modificare</asp:Localize></label>
                    </div>

                    <div data-container-for="Unità Produttiva" class="k-edit-field">
                        <input type="text" id="Cmb_Parametri" />
                    </div>

                </div>
            </div>

            <div class="row">
                <hr class="style1" />

                <div id="modifica_Razza_Capo">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RazzaCapo %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Modifica_Razza_Capo" style="width: 300px" />
                    </div>
                </div>

                <%--<div id="modifica_Categoria_Capo">
                    <div class="k-edit-label">
                        <label>Categoria Capo</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Categoria_Capo" style="width: 300px" />
                    </div>
                </div>--%>

                <div id="modifica_Matricola_Madre">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MatricolaMadre %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Matricola_Madre" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Razza_Madre">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RazzaMadre %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Razza_Madre" style="width: 300px" />

                    </div>
                </div>
                
                <div id="modifica_Matricola_Padre">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MatricolaPadre %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Matricola_Padre" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Razza_Padre">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RazzaPadre %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Razza_Padre" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Sesso">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Sesso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Sesso" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Validato">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Validato %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Validato" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_FornitoreFatt">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FornitoreFatturazione %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Fornitore_Fatt" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_FornitoreProv">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FornitoreProvenienza %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Fornitore_Prov" style="width: 300px" />
                    </div>
                </div>
                
                <div id="modifica_StallaSvezz">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StallaSvezzamento %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Stalla_Svezz" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Metodo_Produzione">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MetodoProduzione %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_modifica_Metodo_Produzione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_CF_Detentore">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodFiscDetentore %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_modifica_CF_Detentore" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_CF_Proprietario">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodFiscProprietario %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_modifica_CF_Proprietario" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Certificato">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CertificatoINTRA %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_modifica_Certificato" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Modello4_Ingresso">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumModello4Ingresso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_modifica_Modello4_Ingresso" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Modello4_Uscita">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumModello4Uscita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_modifica_Modello4_Uscita" style="width: 300px" />
                    </div>
                </div>

                <div id="Modello4_Ingresso_Prenotazione">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceModello4Ingresso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Modello4_Ingresso_Prenotazione" style="width: 300px" />
                    </div>
                </div>

                <div id="Modello4_Uscita_Prenotazione">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceModello4Uscita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Modello4_Uscita_Prenotazione" style="width: 300px" />
                    </div>
                </div>
                
                <div id="Lotto_Fornitore">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LottoFornitore %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Lotto_Fornitore" style="width: 300px" />
                    </div>
                </div>
                   
                <div id="Lotto">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Lotto %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Lotto" style="width: 300px" />
                    </div>
                </div>

                <div id="Data_Documento_Ingresso">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDocumentoIngresso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="date" id="Txt_Data_Documento_Ingresso" />
                    </div>
                </div>

                <div id="Data_Documento_Uscita">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDocumentoUscita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="date" id="Txt_Data_Documento_Uscita" />
                    </div>
                </div>
                
                <div id="modifica_codice_azienda_prov">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodiceAziendaFornitore %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Codice_Azienda_Fornitore" style="width: 300px" />
                    </div>
                </div>
                
                <div id="modifica_codice_azienda_nasc">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CodAUSLAziendaNascita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Codice_Azienda_Nascita" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_ddt_ingresso">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroDDTIngresso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_N_Bolla_Fornitore" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_data_ddt_ingresso">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDDTIngresso %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="date" class="k-input k-textbox" id="Txt_Data_DDT_Ingresso" style="width: 300px" />
                    </div>
                </div>
                
                <div id="modifica_ddt_uscita">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, NumeroDDTUscita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_N_Bolla_Uscita" style="width: 300px" />
                    </div>
                </div>
                
                <div id="modifica_data_ddt_uscita">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataDDTUscita %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="date" class="k-input k-textbox" id="Txt_Data_DDT_Uscita" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Incremento_Teorico">
                    <div class="k-edit-label">
                        <label><asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, IncrementoTeoricoKg %>" runat="server"></asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Incremento_Teorico" style="width: 300px" />
                    </div>
                </div>

            </div>
        </div>

        <div class="window-footer">
            <button type="button" class="k-primary k-button" id="btn_SavePresetColumns" onclick="savePresetColumns();">
                <span class="k-icon k-i-check"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SavePresetColumns %>" runat="server">Salva Preset</asp:Localize>
            </button>
            <button type="button" class="k-primary k-button" id="btn_ApplicaModifiche" onclick="applicaModifiche();">
                <span class="k-icon k-i-check"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ApplicaModifiche %>" runat="server">Applica Modifiche</asp:Localize>
            </button>
            <button type="button" class="k-button" id="btn_AnnullaModifiche" onclick="chiudiModificaMultipla();">
                <span class="k-icon k-i-cancel"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Annulla %>" runat="server">Annulla</asp:Localize>
            </button>
        </div>

    </div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script>
        var indirizzohttp = "./Modifica_Multipla_Zoo.aspx";
        var idKendoModifica_Multipla_Zoo = "<%= hdKendoModifica_Multipla_Zoo.ClientID%>";
        var modificaMultipla = $('#<%=hidden_ModificaMultipla.ClientID %>').val();
        var presetColumnsToUpdate = $('#<%=hidden_presetColumnsToUpdate.ClientID %>').val();
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_zoo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_zoo_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_zoo_jQueryDocReady.js") %>"></script>

</asp:Content>
