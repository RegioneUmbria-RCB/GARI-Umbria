<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Modifica_Multipla_PianoColturale.aspx.vb" Inherits="AgroAgenda_2010.Modifica_Multipla_PianoColturale" %>


<%--<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Modifica_Multipla_PianoColturale.aspx.vb" Inherits="AgroAgenda_2010.Modifica_Multipla_PianoColturale" %>--%>

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

        .k-confirm {
            max-width: 50% !important;
        }
        
        .k-confirm .k-window-content {
            max-height: 500px !important;
            overflow: auto !important;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField runat="server" ID="hdTestataTemp" />
    <asp:HiddenField runat="server" ID="hdKendoModifica_Multipla_PianoColturale" />
    <asp:HiddenField runat="server" ID="hdChiavi" />
    <asp:HiddenField ID="hidden_modificaMultipla" runat="server" />
    <asp:HiddenField ID="hidden_ModalitaMonoAzienda" runat="server" />

    <asp:HiddenField ID="hd_usaFiltroRicercaNG" runat="server" />
    <asp:HiddenField ID="hd_Piva" runat="server" />

    <div id="id_MainContainer" class="container" style="padding: 0;">
        <div class="row">
            <div class="col-xs-12">
                <div class="btn btn-success" id="btnFiltraEsercizi">
                    <span class="fa fa-search"></span>
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FiltraEsercizi %>" runat="server">Filtra Esercizi</asp:Localize>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-12">
                <div id="divKendoModifica_Multipla_PianoColturale" style="overflow: auto; margin-top: 10px; margin-bottom: 10px;"></div>
            </div>
        </div>
    </div>

    <!-- Modifica Multipla -->
    <div id="winModificaMultipla" style="display: none">

        <div class="window-content">
            <div class="row">
                <div id="scelta_parametro">
                    <p id="avvertimentoAppBloccatiModificaMultipla" style="font-weight: bold; color: red"></p>
                    <p id="avvertimentoEserciziChiusiModificaMultipla" style="font-weight: bold; color: red"></p>
                    <p id="avvertimentoModificaMultipla" style="font-weight: bold"></p>
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriDaModificare %>" runat="server">Parametri da Modificare</asp:Localize></label>
                    </div>
                    <div data-container-for="Unità Produttiva" class="k-edit-field">
                        <input type="text" id="Cmb_Parametri" />
                    </div>
                </div>

                <div id="modifica_esercizi">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ApplicaA %>" runat="server">Applica a</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Esercizi" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_esercizi_data">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Data %>" runat="server">Data</asp:Localize></label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Esercizi" />
                    </div>
                </div>
            </div>

            <div class="row">
                <hr class="style1" />
                <div id="modifica_finalita">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Finalità %>" runat="server">Finalità</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Finalita" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Regolamento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Regolamento %>" runat="server">Regolamento</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Regolamento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_DPI">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Disciplinare %>" runat="server">Disciplinare</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Disciplinare" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_disciplinare">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Disciplinare %>" runat="server">Disciplinare</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Disciplinare" style="width: 300px" />
                    </div>
                    <div class="k-edit-label">
                        <label>IAF</label>
                        <!-- i18n -->
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_IAF" style="width: 300px" />
                    </div>

                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tipologia %>" runat="server">Tipologia</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Tipologia" style="width: 300px" />
                    </div>

                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, StatoImpianto %>" runat="server">Stato Impianto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_StatoImpianto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_n">
                    <div class="k-edit-label">
                        <label>N (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_N" />
                    </div>
                </div>

                <div id="modifica_p">
                    <div class="k-edit-label">
                        <label>P (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_P" />
                    </div>
                </div>

                <div id="modifica_k">
                    <div class="k-edit-label">
                        <label>K (kg/ha)</label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Mod_K" />
                    </div>
                </div>

                <div id="modifica_varieta">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Varietà %>" runat="server">Varietà</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Varieta" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Grva">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GruppoVarietale %>" runat="server">Gruppo Varietale</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Mod_Grva" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_CapitolatoPrivato">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CapitolatoPrivato %>" runat="server">Capitolato Privato</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_CapitolatoPrivato" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Certificazione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Certificazione %>" runat="server">Certificazione</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" class="k-input k-textbox" id="Txt_Certificazione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_OrganismoReferente">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, OrganismoReferente %>" runat="server">Organismo Referente</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_OrganismoReferente" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_MagazzinoConferimento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MagazzinoConferimento %>" runat="server">Magazzino Conferimento</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_MagazzinoConferimento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_metodo_produzione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MetodoProduzione %>" runat="server">Metodo Produzione</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Metodo_Produzione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Data_Fine_Appezzamento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ChiusuraAppezzamento %>" runat="server">Chiusura Appezzamento</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Fine_Appezzamento" />
                    </div>
                </div>

                <div id="modifica_Data_Fine_Impianto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ChiusuraImpianto %>" runat="server">Chiusura Impianto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Fine_Impianto" />
                    </div>
                </div>

                <div id="modifica_Copertura">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Copertura %>" runat="server">Copertura</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Copertura" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_FormaAllevamento">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, FormaAllevamento %>" runat="server">Forma Allevamento</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_FormaAllevamento" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Portinnesto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Portinnesto %>" runat="server">Portinnesto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Portinnesto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Data_Inizio_Portinnesto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, MessaDimoraPortinnesto %>" runat="server">Messa a dimora Portinnesto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Inizio_Portinnesto" />
                    </div>
                </div>

                <div id="modifica_SuFila">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SuFila %>" runat="server">Distanza Su Fila</asp:Localize>
                            [m]
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_SuFila" />
                    </div>
                </div>

                <div id="modifica_TraFila">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, TraFila %>" runat="server">Distanza Tra Fila</asp:Localize>
                            [m]
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_TraFila" />
                    </div>
                </div>

                <div id="modifica_Resa">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ResaPrevista %>" runat="server">Resa Prevista</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Resa" />
                    </div>
                </div>

                <div id="modifica_DataSemina">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataSeminaPrevista %>" runat="server">Data Semina Prevista</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_DataSemina" />
                    </div>
                </div>

                <div id="modifica_DataRaccolta">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataRaccoltaPrevista %>" runat="server">Data Raccolta Prevista</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_DataRaccolta" />
                    </div>
                </div>

                <div id="modifica_DataFioritura">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataFiorituraPrevista %>" runat="server">Data Fioritura Prevista</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_DataFioritura" />
                    </div>
                </div>

                <div id="modifica_ImpIrrigazione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ImpiantoIrrigazione %>" runat="server">Impianto Irrigazione</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_ImpIrrigazione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_nrAppBio">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AppBioCod %>" runat="server">Cod. Biologico App.</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_nrAppBio" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_FlagSecondoRaccolto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, SecondoRaccolto %>" runat="server">Secondo Raccolto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="FlagSecondoRaccolto" />
                    </div>
                </div>

                <div id="modifica_CertificazioneAziendale">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CertificazioneAziendale %>" runat="server">Certificazione Aziendale</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="CmbMulti_CertificazioneAziendale" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Contributi">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Contributi %>" runat="server">Contributi</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="CmbMulti_Contributi" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_CertificazioneProdotto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CertificazioneProdotto %>" runat="server">Certificazione Prodotto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_CertificazioneProdotto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Residuo">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Residuo %>" runat="server">Residuo</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Residuo" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_LicenzaColtivazione">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LicenzaColtivazione %>" runat="server">Licenza Coltivazione</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_LicenzaColtivazione" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_RiferimentoTrasferimentoDati">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, RiferimentoTrasferimentoDati %>" runat="server">Riferimento Trasferimento Dati</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_RiferimentoTrasferimentoDati" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Tecnico">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Tecnico %>" runat="server">Tecnico</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="CmbMulti_Tecnico" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_PianoSemina">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, PianoSemina %>" runat="server">Piano Semina</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_PianoSemina" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Prodotto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Prodotto %>" runat="server">Prodotto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Cmb_Prodotto" style="width: 300px" />
                    </div>
                </div>

                <div id="modifica_Data_Inizio_Impianto">
                    <div class="k-edit-label">
                        <label>
                            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataInizioImpianto %>" runat="server">Inizio Impianto</asp:Localize>
                        </label>
                    </div>
                    <div class="k-edit-field">
                        <input type="text" id="Txt_Data_Inizio_Impianto" />
                    </div>
                </div>

            </div>
        </div>

        <div class="window-footer">
            <button type="button" class="k-primary k-button" id="btn_ApplicaModifiche" onclick="applicaModifiche(1);">
                <%--chiamante: 1 = modifica multipla; 2 = gestione esercizi--%>
                <span class="k-icon k-i-check"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ApplicaModifiche %>" runat="server">Applica Modifiche</asp:Localize>
            </button>
            <button type="button" class="k-button" id="btn_AnnullaModifiche" onclick="chiudiModificaMultipla();">
                <span class="k-icon k-i-cancel"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Annulla %>" runat="server">Annulla</asp:Localize>
            </button>
        </div>

    </div>

    <!-- Apertura / Chiusura Esercizi (GestioneEsercizi)-->
    <div id="winGestioneEsercizi" style="display: none">

        <div class="window-content">
            <div class="row">
                <div id="scelta_azione">
                    <p id="avvertimentoAppBloccatiGestioneEsercizi" style="font-weight: bold; color: red"></p>
                    <%--<div data-container-for="Unità Produttiva" class="k-edit-field">
                        <input type="text" id="Cmb_Azioni" />
                    </div>--%>

                    <div class="col-lg-6 col-md-6 col-sm-12">
                        <div class="form-group">
                            <div class="input-group">
                                <label class="input-group-addon lbl_required" id="lblAzioni" for="Cmb_Azioni">
                                    <asp:Localize meta:resourcekey="Azione" runat="server">Azione</asp:Localize>:
                                </label>
                                <input name="Cmb_Azioni" id="Cmb_Azioni" class="form-control" />
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-5 col-md-6 col-sm-12">
                        <div class="input-group">
                            <label class="input-group-addon lbl_required" id="lblDataChiusura" for="txtDataChiusura">
                                <i class="fa fa-calendar"></i>
                                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, DataChiusura %>" runat="server">Data Chiusura</asp:Localize>
                            </label>
                            <input type="text" name="txtDataChiusura" id="txtDataChiusura" class="form-control kendoDate" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <hr class="style1" />

        <div class="window-footer">
            <button type="button" class="k-primary k-button" id="btn_ApplicaModifiche_GestioneEsercizi" onclick="applicaModifiche(2);">
                <%--chiamante: 1 = modifica multipla; 2 = gestione esercizi--%>
                <span class="k-icon k-i-check"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Esegui %>" runat="server">Esegui</asp:Localize>
            </button>

            <button type="button" class="k-button" id="btn_AnnullaGestioneEsercizi" onclick="chiudiGestioneEsercizi();">
                <span class="k-icon k-i-cancel"></span>&nbsp;
                <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Annulla %>" runat="server">Annulla</asp:Localize>
            </button>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <%--<script id="templateKendoEsercizi" type="text/x-kendo-template">
        <div id="btnGestioneEsercizi" class="k-button k-button-icontext" onclick="caricaGestioneEsercizi(true);">
            <span class="k-icon k-i-logout"></span>
            <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010,ChiusuraAperturaEsercizi %>" runat="server">Chiusura/Apertura Esercizi</asp:Localize>
        </div>
    </script>--%>

    <script type="text/javascript">
        var indirizzohttp = "./Modifica_Multipla_PianoColturale.aspx";
        var filtroneImpostato = <%= FiltroneImpostato.ToString.ToLower %>;
        var idTestataTemp = "<%= hdTestataTemp.ClientID%>";
        var idKendoModifica_Multipla_PianoColturale = "<%= hdKendoModifica_Multipla_PianoColturale.ClientID%>";
        var idChiavi = "<%= hdChiavi.ClientID%>";

        var modificaMultipla = $('#<%=hidden_ModificaMultipla.ClientID %>').val();
        var modalitaMonoAzienda = $('#<%=hidden_ModalitaMonoAzienda.ClientID %>').val();

        var usaFiltroRicercaNG = $('#<%=hd_usaFiltroRicercaNG.ClientID %>').val() == 'True' ? true : false;
        var Piva = $('#<%=hd_Piva.ClientID %>').val();
    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_PianoColturale.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_PianoColturale_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_PianoColturale_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Modifica_Multipla_PianoColturale_globali.js") %>"></script>

</asp:Content>
