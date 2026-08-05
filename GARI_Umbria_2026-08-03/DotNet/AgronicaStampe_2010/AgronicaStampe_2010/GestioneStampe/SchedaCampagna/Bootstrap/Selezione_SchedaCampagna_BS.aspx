<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master" CodeBehind="Selezione_SchedaCampagna_BS.aspx.vb" Inherits="AgronicaStampe_2010.Selezione_SchedaCampagna_BS" %>

<%@ Import Namespace="AgronicaCoreDataProvider" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .boxBtnSezioniStampa {
            margin: 10px 0;
        }

        .boxOpzioniStampa {
            margin: 25px 0;
        }

        #titoloOpzioniStampa {
            margin: 0 0 15px 0;
        }

        .equal-btn {
          flex: 1;
          text-align: center;  
        }
        .btn + .btn {
          margin-left: 0.5rem; 
        }

        .fa.fa-print.cfp-default-style, .fa.fa-plus.cfp-default-style {
            height: auto;
            width: auto;
            color: inherit;
            background-image: none !important;
            transform: none !important;
        }

        .fa.fa-print.cfp-default-style::before, .fa.fa-plus.cfp-default-style::before {
            opacity: 1;
        }

        .cfp-underline-btn{
            font-size: 14px;
            font-weight: 600;
            color: #002850;
            cursor: pointer;
        }
        .cfp-underline-btn:hover{
            color: #000;
            text-decoration: underline;
        }
        .btn.btn-info.cfp-btn-outline{
            border: 1px solid #002850 !important;
            background-color: #fff !important;
            color: #002850 !important;
        }
        .btn.btn-info.cfp-btn-outline:hover{
            border: 1px solid #002850 !important;
            background-color: #e3e7ec !important;
            color: #002850 !important;
        }
        .cfp-btn-checkbox{
            display: flex;
            align-items: center;
            justify-content: start;
            height: 24px !important;
            font-size: 12px;
            border-radius: 99px;
        }
        .cfp-btn-checkbox + .cfp-btn-checkbox{
            margin-left: 0 !important;
            margin-top: 4px !important;
        }
        .cfp-checkbox-col {
            display: flex;
            flex-direction: column;
        }
        .cfp-checkbox-row {
            display: flex;
            align-items: center;
            gap: 6px;
            position: relative;
        }
        .cfp-checkbox-row:has(.kendoSwitch:not([style*="display: none"])) {
            margin-bottom: 4px;
        }
        .cfp-checkbox-row label{
            margin-bottom: 0 !important;
        }

        .cfp-checkbox-row-children {
            margin-left: 28px;
            padding-left: 2px;
        }
        .cfp-checkbox-row-children:has(.kendoSwitch:not([style*="display: none"]))::before {
            content: '';
            position: absolute;
            top: 3px;
            left: -20px;
            width: 15px;
            height: 14px;
            border-left: 1px solid #666;
            border-bottom: 1px solid #666;
            border-radius: 0 0 0 6px;
            pointer-events: none;
        }

        .cfp-resume-row{
            display: flex;
            justify-content: space-between; 
            padding-bottom: 9px;
            margin-bottom: 7px;
            font-size: 12px;
            border-bottom: 1px solid #ddd;
        }
        .cfp-resume-label{
            opacity: 0.5;
            font-weight: 600;
        }
        .cfp-resume-value{
            font-weight: 400;
        }

        .cfp-flex-parent-centered {
            display: flex;
            align-items: center;
        }


    </style>
    <script type="text/javascript">

        function SelezionaDeselezionaTutti() {

            if ($('#chkSelezionaTutteImprese').is(':checked')) {
                //seleziono tutto
                $('.ChkSelezionaImpresa').each(function () {
                    $(this).children('input').attr('checked', 'checked');

                });
            }
            else {

                //deseleziono tutto
                $('.ChkSelezionaImpresa').each(function () {
                    $(this).children('input').removeAttr('checked');
                });
            }
        }

        function SelezionaDeselezionaSoloDifesa() {

            if ($('#checkMacch').children('input').is(':checked')) {
                //seleziono tutto
                $('#checkSoloMacchDif').children('input').attr('checked', 'checked');
                $('#checkSoloMacchDif').children('input').removeAttr('disabled');
            }
            else {
                $('#checkSoloMacchDif').children('input').removeAttr('checked');
                $('#checkSoloMacchDif').children('input').attr('disabled', 'disabled');
            }
        }

        <%--$(document).ready(function () {
            $("#<%=Txt_TempoRientro.ClientID%>").kendoTextBox({
                placeholder: "Name",
                label: ""
            });--%>
        //           $(".datepicker").datepicker({ format: 'dd/mm/yyyy' }); //bootstrap version
        $('#chkSelezionaTutteImprese').click(function () {
            SelezionaDeselezionaTutti();
        });


        // ----------------------------------------------
        // Vedi pageload prima del postback lato Server
        // 
        // ----------------------------------------------

        $('#check_trattamenti').click(function () {
            SelezionaDeselezionaFito();
        });


        if ($('#check_trattamenti').children('input').is(':checked')) {
            $('#check_fitoregolatori').children('input').removeAttr('disabled');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="gRowRiepilogo1" class="row">
    <!-- HEADER -->
    <div style="display: flex; width: 100%; padding: 32px 32px; border-bottom: 1px solid #ddd; background: #FFF;">
        <div style="display: flex; width: 100%; justify-content: space-between; align-items: center;">
            <div style="display: flex; align-items: center;">
                <div style="display: flex; align-items: center; justify-content: center; width: 58px; height: 58px; border-radius: 999px; border: 1px solid #ddd;">
                    <i class="fa fa-print cfp-default-style" style="font-size: 25px; color: #002850; opacity: 0.4; margin-top: -1px;"></i>
                </div>
                <div style="display: flex; flex-direction: column; padding-left: 16px;">
                    <div style="font-size: 14px; padding-bottom: 4px;">Tipo stampa</div>
                    <div style="font-size: 16px; font-weight: 600;" id="printTypeValue"></div>
                </div>
            </div>
            <div style="display: flex; align-items: center;">
                <div class="cfp-underline-btn" onclick="visualizzaElencoReport()" runat="server">
                    Visualizza elenco report
                </div>
                <div class="btn btn-info cfp-btn-outline" id="btn_stampaProva" style="margin-left: 12px;">
                    <i class="fa fa-print cfp-default-style"></i>Stampa - Prova
                </div>
                <div class="btn btn-info" id="btn_stampaDefinitiva" style="margin-left: 12px;">
                    <i class="fa fa-print cfp-default-style"></i>Stampa - Definitiva
                </div>
            </div>
        </div>
    </div>

    <div style="display: flex; padding-top: 16px; padding-bottom: 16px; align-items: stretch;">
        <!-- LEFT COLUMN -->
        <div class="display: flex; flex-direction: column; col-sm-9">
            <div style="display: flex; align-items: stretch; padding: 20px; border-radius: 16px; border: 1px solid #ddd; background: #FFF">
                <!-- SECTIONS VISIBILITY -->
                <div style="display: flex; flex-direction: column; width: 50%; padding-right: 16px;">
                    <div style="font-size: 14px; font-weight: 600; padding-bottom: 6px; border-bottom: 1px solid #ddd;">Visibilità sezioni</div>
                    <div style="flex-grow: 1; width: 100%; margin-top: 18px; border-radius: 10px; border: 1px solid #ddd; padding: 16px;">
                        <div style="display: flex; align-items: center; justify-content: space-between; width: 100%; border-bottom: 1px solid #ddd; margin-bottom: 16px; padding-bottom: 12px;">
                            <div>
                                <div class="btn btn-info cfp-btn-outline cfp-btn-checkbox" id="btnSelezionaTutteSezioni" onclick="attivaDisattivaSezioniDaStampare(true)" runat="server">
                                    <i class="fa fa-check-square"></i> <span style="text-transform: none;">Seleziona tutte</span>
                                </div>
                                <div class="btn btn-info cfp-btn-outline cfp-btn-checkbox" id="btnDeselezionaTutteSezioni" onclick="attivaDisattivaSezioniDaStampare(false)" runat="server">
                                    <i class="fa fa-square"></i> <span style="text-transform: none;">Deseleziona tutte</span>
                                </div>
                            </div>
                            <div class="btn btn-info cfp-btn-outline" id="btn_daStampareAncheSeVuote">
                                <i class="fa fa-list" style="margin-top: 2.5px;"></i> <span style="text-transform: none;">Imposta visibilità sezioni vuote</span>
                            </div>
                        </div>
                        <div style="display: flex; width: 100%">
                            <!-- LEFT CHECKBOXES -->
                            <div style="width: 50%">
                                <div class="cfp-checkbox-col" id="sezioniNonVeneto-left">
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="a" id="check_frontespizio" name="check_frontespizio" class="kendoSwitch" />
                                        <label for="check_frontespizio">Frontespizio</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="s" id="check_personale" name="check_personale" class="kendoSwitch" />
                                        <label for="check_personale">Personale con Patentino</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="o" id="check_dati_catastali" name="check_dati_catastali" class="kendoSwitch" />
                                        <label for="check_dati_catastali">Dati Catastali</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="p" id="check_semine" name="check_semine" class="kendoSwitch" />
                                        <label for="check_semine">Semine / Trapianti</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="b" id="check_Fertilizzazioni" name="check_Fertilizzazioni" class="kendoSwitch" />
                                        <label for="check_Fertilizzazioni">Fertilizzazioni</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="c" id="check_trattamenti" name="check_trattamenti" class="kendoSwitch" />
                                        <label for="check_trattamenti">Trattamenti</label>
                                    </div>
                                    <div class="cfp-checkbox-row cfp-checkbox-row-children">
                                        <input type="checkbox" value="t" id="check_fitoregolatori" name="check_fitoregolatori" class="kendoSwitch" />
                                        <label for="check_fitoregolatori">Fitoregolatori</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="d" id="check_fasi_fenologiche" name="check_fasi_fenologiche" class="kendoSwitch" />
                                        <label for="check_fasi_fenologiche">Osservazioni Fasi Fenologiche</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="e" id="check_trappole" name="check_trappole" class="kendoSwitch" />
                                        <label for="check_trappole">Trappole Installate</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="f" id="check_ril_avver_trappole" name="check_ril_avver_trappole" class="kendoSwitch" />
                                        <label for="check_ril_avver_trappole">Rilievi Avversità nelle Trappole</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="g" id="check_ril_avver_campo" name="check_ril_avver_campo" class="kendoSwitch" />
                                        <label for="check_ril_avver_campo">Rilievi Avversità in Campo</label>
                                    </div>
                                    <div class="cfp-checkbox-row cfp-checkbox-row-children">
                                        <input type="checkbox" name="chkAvversitaQta" id="chkAvversitaQta" class="kendoSwitch" />
                                        <label for="chkAvversitaQta" id="lblAvversitaQta">Visualizza le Quantità rilevate in Campo</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="h" id="check_irrigazione" name="check_irrigazione" class="kendoSwitch" />
                                        <label for="check_irrigazione">Irrigazione</label>
                                    </div>
                                </div>
                                
                                <div class="cfp-checkbox-row" id="sezioniVeneto-left" style="display: none">
                                    <input type="checkbox" value="y" id="check_manutenzione_veneto" name="check_manutenzione_veneto" class="kendoSwitch" />
                                    <label for="check_manutenzione">Manutenzione Macchinari (Scheda A)</label>
                                </div>
                            </div>

                            <!-- RIGHT CHECKBOXES -->
                            <div style="width: 50%">
                                <div class="cfp-checkbox-col" id="sezioniNonVeneto-right">
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="i" id="check_operazioni_colturali" name="check_operazioni_colturali" class="kendoSwitch" />
                                        <label for="check_operazioni_colturali">Altre Operazioni Colturali</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="l" id="check_ind_maturita" name="check_ind_maturita" class="kendoSwitch" />
                                        <label for="check_ind_maturita">Indici di Maturità</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="n" id="check_rilievo_prod" name="check_rilievo_prod" class="kendoSwitch" />
                                        <label for="check_rilievo_prod">Rilievo Produzione e Data Raccolta</label>
                                    </div>
                                    <div class="cfp-checkbox-row cfp-checkbox-row-children">
                                        <input type="checkbox" name="chkTutteRaccolte" id="chkTutteRaccolte" class="kendoSwitch" />
                                        <label for="chkTutteRaccolte" id="lblTutteRaccolte">Visualizza tutte le Raccolte</label>
                                    </div>
                                    <div class="cfp-checkbox-row cfp-checkbox-row-children" >
                                        <input type="checkbox" name="chkQtaQtaRaccolte" id="chkQtaQtaRaccolte" class="kendoSwitch" />
                                        <label for="chkQtaQtaRaccolte" id="lblQtaQtaRaccolte">Visualizza le Quantità raccolte</label>
                                    </div>
                                    <div class="cfp-checkbox-row cfp-checkbox-row-children">
                                        <input type="checkbox" name="chkDataUltimaRaccolta" id="chkDataUltimaRaccolta" class="kendoSwitch" />
                                        <label for="chkDataUltimaRaccolta" id="lblDataUltimaRaccolta">Visualizza la Data Ultima Raccolta</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="m" id="check_piogge" name="check_piogge" class="kendoSwitch" />
                                        <label for="check_piogge">Piogge</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="q" id="check_informazioni" name="check_informazioni" class="kendoSwitch" />
                                        <label for="check_informazioni">Informazioni/Dichiarazioni</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="y" id="check_manutenzione" name="check_manutenzione" class="kendoSwitch" />
                                        <label for="check_manutenzione">Manutenzione Macchinari</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="r" id="check_visite_ispettive" name="check_visite_ispettive" class="kendoSwitch" />
                                        <label for="check_visite_ispettive">Visite Ispettive</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" value="1" id="check_trattamenti_post_raccolta" name="check_trattamenti_post_raccolta" class="kendoSwitch" />
                                        <label for="check_trattamenti_post_raccolta">Trattamenti Post Raccolta</label>
                                    </div>
                                    <div class="cfp-checkbox-row" id="div_u" style="display: none">
                                        <input type="checkbox" value="u" id="check_pratiche_ecologiche" name="check_pratiche_ecologiche" class="kendoSwitch" />
                                        <label for="check_pratiche_ecologiche">Pratiche Ecologiche</label>
                                    </div>
                                    <div class="cfp-checkbox-row" id="div_w" style="display: none">
                                        <input type="checkbox" value="w" id="check_formazione" name="check_formazione" class="kendoSwitch" />
                                        <label for="check_formazione">Formazione</label>
                                    </div>
                                    <div class="cfp-checkbox-row" id="div_x" style="display: none">
                                        <input type="checkbox" value="x" id="check_gestione_rifiuti" name="check_gestione_rifiuti" class="kendoSwitch" />
                                        <label for="check_gestione_rifiuti">Gestione Rifiuti</label>
                                    </div>
                                    <div class="cfp-checkbox-row" id="div_v" style="display: none">
                                        <input type="checkbox" value="v" id="check_verifiche_conf" name="check_verifiche_conf" class="kendoSwitch" />
                                        <label for="check_verifiche_conf">Verifiche Conformità</label>
                                    </div>
                                </div>
                                
                                <div class="cfp-checkbox-row" id="sezioniVeneto-right" style="display: none">
                                    <label id="lblSchede" for="ddlSchede"><b>Schede:</b></label>
                                    <input name="ddlSchede" id="ddlSchede" class="form-control" />
                                </div>
                            </div>  
                        </div>
                    </div>
                </div>

                <!-- OPTIONS -->
                <div style="display: flex; flex-direction: column; width: 50%; padding-left: 16px;">
                    <div style="font-size: 14px; font-weight: 600; padding-bottom: 6px; border-bottom: 1px solid #ddd;">Opzioni</div>
                    <div style="width: 100%; padding-top: 18px;">

                        <!-- PERIOD -->
                        <div id="Data_InizioChk" style="display: flex; flex-direction: column; width: 100%; margin-bottom: 16px; border-radius: 10px; border: 1px solid #ddd; padding: 16px;">
                            <div style="display: flex; width: 100%;">
                                <div style="width: 40%; display: flex; flex-direction: column;">
                                    <span style="font-size: 14px; color: #575757; padding-top: 3px; padding-bottom: 2px;">Periodo</span>
                                    <div style="display: flex; align-items: center;">
                                        <label for="checkData" style="margin-right: 8px; margin-bottom: -3px;">Data</label>
                                        <input type="checkbox" name="checkData" id="checkData" class="kendoSwitch" />
                                        <label for="checkData" style="margin-left: 8px; margin-bottom: -3px;">Intervallo</label>
                                    </div>
                                </div>
                                <div style="width: 60%; display: flex; align-items: center;">
                                    <div id="Data" style="width: 100%;">
                                        <div class="form-horizontal">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon lbl_required" id="lbl_Stampa_Giorno" for="txtStampaGiorno">Data</span>
                                                    <input id="txtStampaGiorno" name="txtStampaGiorno" class="kendoCalendar" style="width: 100%; max-width: none;" maxlength="10" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="Data_Inizio" style="display: flex; width: 100%">
                                        <div class="form-horizontal" style="padding-right: 4px;">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon lbl_required" id="lbl_Stampa_Validita_Inizio" for="txtValiditaInizio">Data inizio</span>
                                                    <input id="txtValiditaInizio" name="txtValiditaInizio" class="kendoCalendar" style="width: 100%; max-width: none;" maxlength="10" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-horizontal" style="padding-left: 4px;">
                                            <div class="form-group">
                                                <div class="input-group">
                                                    <span class="input-group-addon lbl_required" id="lbl_Stampa_Validita_Fine" for="txtValiditaFine">Data fine</span>
                                                    <input id="txtValiditaFine" name="txtValiditaFine" class="kendoCalendar" style="width: 100%; max-width: none;" maxlength="10" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div style="padding: 8px 14px; background: #f1f3f7; border-radius: 6px; margin-top: -8px;">
                                    <span style="opacity: 0.5;">Impostando una data esterna al periodo di attività del centro aziendale si potrebbe ottenere una stampa nulla</span>
                                </div>
                        </div>

                        <!-- REGION -->
                        <div style="display: flex; width: 100%; margin-bottom: 16px; border-radius: 10px; border: 1px solid #ddd; padding: 16px; padding-bottom: 8px;">
                            <div style="width: 40%; display: flex; flex-direction: column;">
                                <span style="font-size: 14px; color: #575757; padding-top: 3px; padding-bottom: 6px;">Visualizza logo regione</span>
                                <input type="checkbox" id="checkLogoRegione" name="checkLogoRegione" class="kendoSwitch" />
                                <label for="checkLogoRegione"></label>
                            </div>
                            <div style="width: 60%;">
                                <div class="input-group">
                                    <label id="lblddlRegioni" class="input-group-addon lbl_required" for="ddlRegioni">Regione</label>
                                    <input name="ddlRegioni" id="ddlRegioni" class="form-control" />
                                </div>
                            </div>
                        </div>

                        <!-- CROPS -->
                        <div id="divRotazione" style="display: flex; width: 100%; margin-bottom: 16px; border-radius: 10px; border: 1px solid #ddd; padding: 16px; padding-bottom: 12px;">
                            <div style="width: 40%; display: flex; flex-direction: column;">
                                <span style="font-size: 14px; color: #575757; padding-top: 3px; padding-bottom: 6px;">Visualizza colture ins. manualmente</span>
                                <input type="checkbox" id="chkPrioritaColturePrecedenti" name="chkPrioritaColturePrecedenti" class="kendoSwitch" />
                                <label for="chkPrioritaColturePrecedenti"></label>
                            </div>
                            <div style="width: 60%; display: flex; align-items: center;">
                                <div class="btn btn-info cfp-btn-outline" onclick="tabellaRotazione()" runat="server" style="width: 100%;">
                                    <i class="fa fa-plus cfp-default-style" style="margin-top: 2px;"></i><span style="text-transform: none;">Inserisci manualmente colture precedenti</span>
                                </div>
                            </div>
                        </div>

                        <!-- TOGGLES -->
                        <div style="display: flex; flex-direction: column; width: 100%; margin-bottom: 16px; border-radius: 10px; border: 1px solid #ddd; padding: 16px;">
                            <div style="display: flex; width: 100%;">
                                <div style="width: 50%">
                                    <div class="cfp-checkbox-col">
                                        <div class="cfp-checkbox-row">
                                            <input type="checkbox" name="chkVisualizzaTipologieVarietali" id="chkVisualizzaTipologieVarietali" class="kendoSwitch" />
                                            <label for="chkVisualizzaTipologieVarietali">Visualizza Tipologie Varietali</label>
                                        </div>
                                        <div class="cfp-checkbox-row" id="divChkVisualizzaCapitolatoPrivato" style="display: none">
                                            <input type="checkbox" name="chkVisualizzaCapitolatoPrivato" id="chkVisualizzaCapitolatoPrivato" class="kendoSwitch" />
                                            <label for="chkVisualizzaCapitolatoPrivato" id="lblVisualizzaCapitolatoPrivato">Visualizza Capitolato Privato</label>
                                        </div>
                                        <div class="cfp-checkbox-row">
                                            <input type="checkbox" name="chkVisualizzaFinalita" id="chkVisualizzaFinalita" class="kendoSwitch" />
                                            <label for="chkVisualizzaFinalita">Visualizza Finalità</label>
                                        </div>
                                        <div class="cfp-checkbox-row">
                                            <input type="checkbox" name="chkRaggruppaXCampo" id="chkRaggruppaXCampo" class="kendoSwitch" />
                                            <label id="lblchkRaggruppaXCampo" for="chkRaggruppaXCampo">Raggruppa per Intervento</label>
                                        </div>
                                        <div class="cfp-checkbox-row" id="divMostraDataStampa" style="display: none ">
                                            <input type="checkbox" id="checkMostraDataStampa" name="checkMostraDataStampa"  class="kendoSwitch" />
                                            <label for="checkMostraDataStampa">Mostra data di stampa</label>
                                        </div>
                                        <div class="cfp-checkbox-row" id="divImpostaOrganismoReferente" style="display: none">
                                            <input type="checkbox" id="checkImpostaOrganismoReferente" name="checkImpostaOrganismoReferente"  class="kendoSwitch" />
                                            <label for="checkImpostaOrganismoReferente">Imposta l'organismo referente come intestatario (Scheda Interventi Agronomici)</label>
                                        </div>   
                                        <div class="cfp-checkbox-row" id="divCheck_RegCondizionalita" style="display: none">
                                            <input type="checkbox" id="check_RegCondizionalita" name="check_RegCondizionalita"  class="kendoSwitch" />
                                            <label for="check_RegCondizionalita">Stampa Rif. Reg. Condizionalità</label>
                                        </div>
                                        <div class="cfp-checkbox-row" id="divcheck_RegMisura10" style="display: none">
                                            <input type="checkbox" id="check_RegMisura10" name="check_RegMisura10"  class="kendoSwitch" />
                                            <label for="check_RegMisura10">Stampa Rif. Misura 10 lotta Integrata</label>
                                        </div>
                                    </div>
                                </div>
                                <div style="width: 50%">
                                    <div class="cfp-checkbox-col">
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" name="chkVisualizzaAcquaHa" id="chkVisualizzaAcquaHa" class="kendoSwitch" />
                                        <label for="chkVisualizzaAcquaHa">Visualizza Acqua/ha</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                        <input type="checkbox" name="chkStampaAnnoImpiantoPluriennali" id="chkStampaAnnoImpiantoPluriennali" class="kendoSwitch" />
                                        <label for="chkStampaAnnoImpiantoPluriennali">Stampa anno impianto colture pluriennali</label>
                                    </div>
                                    <div class="cfp-checkbox-row">
                                            <input type="checkbox" name="chkMostraValoriSignificativiNeiRilievi" id="chkMostraValoriSignificativiNeiRilievi" class="kendoSwitch" />
                                            <label for="chkMostraValoriSignificativiNeiRilievi">Mostra nei rilievi anche valori a zero</label>
                                    </div>
                                        <div class="cfp-checkbox-row" id="divCheckStampaODC">
                                            <input type="checkbox" id="checkStampaODC" name="checkStampaODC" class="kendoSwitch" />
                                            <label for="checkStampaODC">Mostra Firma OdC</label>
                                        </div>
                                    </div>
                                    <div class="cfp-checkbox-row" id="divcheck_RegPSR" style="display: none">
                                        <input type="checkbox" id="check_RegPSR" name="check_RegPSR"  class="kendoSwitch" />
                                        <label for="check_RegPSR">Stampa Rif. Reg. P.S.R.</label>
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Separatore aggiunto -->
                            <!-- GlobalGAP nascosto e modificato per allineamento in linea -->   
                            <div id="divcheckVisualizzaLotto" style="display: none; margin-bottom: 10px;">
                                <input type="checkbox" name="checkVisualizzaLotto" id="checkVisualizzaLotto" class="kendoSwitch" />
                                <label for="checkVisualizzaLotto">Visualizza Lotto Impianto/Esercizio</label>
                            </div>
                        </div>
                        
                        <!-- GLOBALGAP E TEMPO RIENTRO OPTIONS -->
                        <div id='divGlobalTempo' style="display: none; flex-direction: column; width: 100%; margin-bottom: 16px; border-radius: 10px; border: 1px solid #ddd; padding: 16px;">
                            <div id="DivGlobalGap" class="cfp-flex-parent-centered" style="display: none; margin-bottom: 10px; flex-wrap: nowrap;">
                                <!-- Elemento 1: Checkbox e Label -->
                                <span style="margin-right: 15px; display: inline-flex; align-items: center;"> 
                                    <input type="checkbox" id="check_globalgap" name="check_globalgap" class="kendoSwitch" />
                                    <label for="check_globalgap" style="margin-left: 5px; white-space: nowrap;">Visualizza titolo GLOBAL - GAP</label>
                                </span>

                                <span class="btn btn-info cfp-btn-outline" onclick="img_globalGap()" runat="server" style="display: inline-flex; align-items: center; flex-shrink: 0;">
                                    <i class="fa fa-eye" style="margin-right: 5px;"></i>Inserisci i dati GLOBAL - GAP
                                </span>
                            </div>
                            <div id="DivTempoRientro"  style="display: none;margin-bottom: 10px;">
                                <div class="form-group">
                                    <div class="input-group">
                                        <span class="input-group-addon lbl_required" id="lbl_descrizione_prodotto">Tempo di rientro [ore]:</span>
                                        <input type="text" id="txt_tempo_rientro" class="form-control" style="width: 150px;" value="48" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- AREA -->
                        <div style="display: flex; width: 100%; border-radius: 10px; border: 1px solid #ddd; padding: 16px; padding-bottom: 8px;">
                            <div style="width: 50%; display: flex; flex-direction: column;">
                                <span style="font-size: 14px; color: #575757; padding-top: 3px; padding-bottom: 6px;">Tipo superficie</span>
                                <div style="display: flex; align-items: center;">
                                    <label for="checkSuperfici" style="margin-right: 8px; margin-bottom: -3px;">Appezzamento</label>
                                    <input type="checkbox" name="checkSuperfici" id="checkSuperfici" class="kendoSwitch" />
                                    <label for="checkSuperfici" style="margin-left: 8px; margin-bottom: -3px;">Impianto</label>
                                </div>
                            </div>
                            <div style="width: 50%; display: flex; flex-direction: column; justify-content: flex-start; gap: 8px;">
                                <div class="input-group" style="width: 100%;">
                                    <label id="lbloddlArrotondamento" class="input-group-addon lbl_required" for="ddlArrotondamento">Arrotondamento</label>
                                    <input name="ddlArrotondamento" id="ddlArrotondamento" class="form-control" />
                                </div>
                                <div id="DivOrdinamento" class="input-group" style="width: 100%; display: none">
                                    <label id="lblOrdinamentoDDL" class="input-group-addon lbl_required" for="ddlOrdinamento">Ordina per:</label>
                                    <input name="ddlOrdinamento" id="ddlOrdinamento" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- RIGHT COLUMN -->
        <div class="col-sm-3" style="display: flex; flex-direction: column;">
            <div style="display: flex; flex-grow: 1; flex-direction: column; border-radius: 16px; border: 1px solid #ddd; overflow: hidden;">
                <!-- RESUME -->
                <div id="Riepilogo_Azienda" runat="server" style="display: flex; flex-direction: column; padding: 20px; background: #f1f3f7; border-bottom: 1px solid #ddd;">
                    <div style="font-size: 14px; font-weight: 600; padding-bottom: 6px; margin-bottom: 18px; border-bottom: 1px solid #ddd;">Riepilogo</div>
                    <div class="cfp-resume-row">
                        <div class="cfp-resume-label">Partita IVA</div>
                        <div class="cfp-resume-value"><asp:Label ID="Lbl_Piva" runat="server" /></div>
                    </div>
                    <div class="cfp-resume-row">
                        <div class="cfp-resume-label">Ragione Sociale</div>
                        <div class="cfp-resume-value"><asp:Label ID="Lbl_Impresa" runat="server" /></div>
                    </div> 
                    <div class="cfp-resume-row">
                        <div class="cfp-resume-label">Centro Aziendale</div>
                        <div class="cfp-resume-value"><asp:Label ID="Lbl_Centro" runat="server" /></div>
                    </div> 
                    <div class="cfp-resume-row" id="divCentroAziendale" style="display: none">
                        <label id="lblCentroAziendale" for="ddlCentroAziendale"></label>
                        <input name="ddlCentroAziendale" id="ddlCentroAziendale" class="form-control" style="width: 200px" />
                    </div> 
                    <div class="cfp-resume-row">
                        <div class="cfp-resume-label">Specie vegetale</div>
                        <div class="cfp-resume-value"><asp:Label ID="Lbl_Specie_Vegetale" runat="server" /></div>
                    </div> 
                    <div class="cfp-resume-row" id="divSpecieVegetale" style="display: none">
                        <label id="lblSpecieVegetale" for="ddlSpecieVegetale"></label>
                        <input name="ddlSpecieVegetale" id="ddlSpecieVegetale" class="form-control" style="width: 200px" />
                    </div> 
                    <div class="cfp-resume-row">
                        <div class="cfp-resume-label">Periodo attivita' centro</div>
                        <div class="cfp-resume-value"><asp:Label ID="Lbl_Validita_Inizio" runat="server" /> <span>&nbsp;al </span> <asp:Label ID="Lbl_Validita_Fine" runat="server"/></div>
                    </div>
                </div>
                
                <!-- WAREHOUSE -->
                <div style="display: flex; flex-direction: column; flex-grow: 1; width: 100%; padding: 20px; background: #FFF;">                   
                    <div style="font-size: 14px; font-weight: 600; padding-bottom: 6px; margin-bottom: 18px; border-bottom: 1px solid #ddd;">Stampe magazzino</div>
                    <div id="Maga" style="display: flex; flex-direction: column; width: 100%;">
                        <div class="form-horizontal" style="margin-bottom: 16px;">
                            <div class="input-group">
                                <label id="LBLddlMagazzino" class="input-group-addon lbl_required" for="ddlMagazzino">Magazzino</label>
                                <input name="ddlMagazzino" id="ddlMagazzino" class="form-control" />
                            </div>
                        </div>
                        <div class="form-horizontal">
                            <div class="input-group">
                                <div class="btn btn-info cfp-btn-outline" id="btn_MagFertilizzanti" style="width: 100%;">
                                    <i class="fa fa-print cfp-default-style"></i> <span style="text-transform: none;">Stampa scheda fertilizzanti</span>
                                </div>
                            </div>
                        </div>
                        <div class="form-horizontal">
                            <div class="input-group">
                                <div class="btn btn-info cfp-btn-outline" id="btn_MagProdFitosanitari" style="width: 100%;">
                                    <i class="fa fa-print cfp-default-style"></i> <span style="text-transform: none;">Stampa scheda prod. fitosanitari</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

    <div id="elencoReportWindow">
        <div id="divKendoElencoReport"></div>
        <div id="DIV_MessaggipopupEr">
        </div>
    </div>
    <div id="tabellaRotazione">
        <div id="divKendoRotazione"></div>
        <div id="DIV_Messaggipopup">
        </div>
    </div>
    <div id="SezioniVuoteWindow">
        <div style="text-align: left;">

            <div id="dialogSezioniVuote" title="Sezioni Vuote">
                        <div>
                            <!--- Bottoni Salvattaggio e Annulla -->
                            <div>
                                <input type="checkbox" value="a" id="check_frontespizio_SezioniVuote" name="check_frontespizio_SezioniVuote" class="kendoSwitch" />
                                <label for="check_frontespizio_SezioniVuote">Frontespizio</label>
                            </div>
                            <div>
                                <input type="checkbox" value="b" id="check_fertilizzazioni_SezioniVuote" name="check_fertilizzazioni_SezioniVuote" class="kendoSwitch" />
                                <label for="check_fertilizzazioni_SezioniVuote">Fertilizzazioni</label>
                            </div>
                            <div>
                                <input type="checkbox" value="c" id="check_trattamenti_SezioniVuote" name="check_trattamenti_SezioniVuote" class="kendoSwitch" />
                                <label for="check_trattamenti_SezioniVuote">Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori</label>
                            </div>
                            <div>
                                <input type="checkbox" value="d" id="check_osservazioni_SezioniVuote" name="check_osservazioni_SezioniVuote" class="kendoSwitch" />
                                <label for="check_osservazioni_SezioniVuote">Osservazioni Fasi Fenologiche</label>
                            </div>
                            <div>
                                <input type="checkbox" value="e" id="check_trappole_SezioniVuote" name="check_trappole_SezioniVuote" class="kendoSwitch" />
                                <label for="check_trappole_SezioniVuote">Trappole Installate</label>
                            </div>
                            <div>
                                <input type="checkbox" value="f" id="check_rilievi_SezioniVuote" name="check_rilievi_SezioniVuote" class="kendoSwitch" />
                                <label for="check_rilievi_SezioniVuote">Rilievi Avversità nelle Trappole</label>
                            </div>
                            <div>
                                <input type="checkbox" value="g" id="check_avversita_SezioniVuote" name="check_avversita_SezioniVuote" class="kendoSwitch" />
                                <label for="check_avversita_SezioniVuote">Rilievi Avversità in Campo</label>
                            </div>
                            <div>
                                <input type="checkbox" value="h" id="check_irrigazione_SezioniVuote" name="check_irrigazione_SezioniVuote" class="kendoSwitch" />
                                <label for="check_irrigazione_SezioniVuote">Irrigazione</label>
                            </div>
                            <div>
                                <input type="checkbox" value="i" id="check_operazioni_SezioniVuote" name="check_operazioni_SezioniVuote" class="kendoSwitch" />
                                <label for="check_operazioni_SezioniVuote">Altre Operazioni Colturali</label>
                            </div>
                            <div>
                                <input type="checkbox" value="l" id="check_indici_SezioniVuote" name="check_indici_SezioniVuote" class="kendoSwitch" />
                                <label for="check_indici_SezioniVuote">Indice di Maturità e Raccolta</label>
                            </div>
                            <div>
                                <input type="checkbox" value="m" id="check_piogge_SezioniVuote" name="check_piogge_SezioniVuote" class="kendoSwitch" />
                                <label for="check_piogge_SezioniVuote">Piogge</label>
                            </div>
                            <div>
                                <input type="checkbox" value="n" id="check_rilievo_SezioniVuote" name="check_rilievo_SezioniVuote" class="kendoSwitch" />
                                <label for="check_rilievo_SezioniVuote">Rilievo Produzione e data Raccolta</label>
                            </div>
                            <div>
                                <input type="checkbox" value="1" id="check_trattamentiPostRaccolta_SezioniVuote" name="check_trattamentiPostRaccolta_SezioniVuote" class="kendoSwitch" />
                                <label for="check_trattamentiPostRaccolta_SezioniVuote">Trattamenti Post Raccolta</label>
                            </div>

                            <!--- Modificare di seguito per l'ultima lettera registrata x -->
                            <div class="btn btn-info" id="btn_Aggiungi_Sezioni_Vuote" onclick='aggiungiSezioneVuote()' style="float: left; margin-right: 5px;">
                                <i class="fa fa-plus">Aggiungi</i>
                            </div>
                            <div class="btn btn-info" id="btn_Annulla_Sezioni_Vuote" onclick='annullaSezioniVuote()' style="float: left; margin-right: 5px;">
                                <i>Annulla</i>
                            </div>
                        </div>
               
            </div>
        </div>
    </div>

    <div id="globalGapWindow">
        <div style="text-align: left;">

            <div id="dialogSezioniGlobalGap" title="Dati GLOBALGAP">
               
                        <div class="input-group">
                            <span class="input-group-addon lbl_required" style="width: 50px" id="lbl_revisione">Revisione:</span>
                            <input type="text" id="txt_revisione" style="width: 850px" class="form-control " />
                        </div>
                        <div class="btn btn-info" onclick="DatiGlobalGap()" runat="server">
                            <i class="fa fa-eye"></i>Salva i dati per stamparli poi..
                        </div>
                        <div class="btn btn-info" onclick="ChiudiGlobalGap()" runat="server">
                            <i class="fa fa-eye"></i>Torna al filtro
                        </div>
                   
            </div>
        </div>
    </div>
    
    <div id="hide" class="jumbotron">

    <hr />
    <br />




  

    <input type="hidden" id="hdQS_DataOggi" runat="server" />
    <input type="hidden" id="hdQS_DataInizioAnnata" runat="server" />
    <input type="hidden" id="hdQS_DataFineAnnata" runat="server" />
    <input type="hidden" id="checkedSezioneVuote" runat="server" />

    <!-- Questi sostituiscono l'uso della Session all'interno della pagina per passaggi client/servers -->
    <input type="hidden" id="hds_piva" runat="server" />
    <input type="hidden" id="hdReportSelezionato" runat="server" />
    <input type="hidden" id="hd_VegCod" runat="server" />
    <input type="hidden" id="hd_Sa_Cod" runat="server" />
    <input type="hidden" id="hd_Lista_Sa_Cod_Da_Variabili_Stampe" runat="server" />
    <input type="hidden" id="hd_Lista_Veg_Cod_Da_Variabili_Stampe" runat="server" />
    <input type="hidden" id="hd_Sezione" runat="server" />
    <input type="hidden" id="hd_ElencoReportAbilitati" runat="server" />
    <input type="hidden" id="hd_ElencoSezioneSuPermessi" runat="server" />
    <input type="hidden" id="hd_globalGapDiv" runat="server" />
    <input type="hidden" id="hd_regolamentiDiv" runat="server" />
    <input type="hidden" ID="hd_ordinamentoDiv" runat="server" />
    <input type="hidden" ID="hd_TempoDiRientro" runat="server" />
    <input type="hidden" id="hd_visualizzaImpinatiFiltrati" runat="server" />
    <input type="hidden" id="hd_codiciTerreno" runat="server" />
    <input type="hidden" id="hd_hashDestUso" runat="server" />
    <input type="hidden" id="hd_GruCod" runat="server" />
    <input type="hidden" id="hd_Flag_ErbOrt" runat="server" />
    <input type="hidden" id="hd_PathAllegati" runat="server" />
    <input type="hidden" id="hd_StampaCampagna_Default_Vedi_AvversitaQta" runat="server" />

    <input type="hidden" id="hdKendoTabellaRotazioneValore" />
    <input type="hidden" id="hdKendoTabellaElencoReport" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Selezione_SchedaCampagna_BS_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Selezione_SchedaCampagna_BS.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Selezione_SchedaCampagna_BS_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Selezione_SchedaCampagna_BS_ws_client.js") %>"></script>

    <script type="text/javascript">
        var cIdQS_DataOggi = "#<%=hdQS_DataOggi.ClientID() %>";
        var cIdQS_DataInizioAnnata = "#<%=hdQS_DataInizioAnnata.ClientID() %>";
        var cIdQS_DataFineAnnata = "#<%=hdQS_DataFineAnnata.ClientID() %>";
        var cIds_piva = "#<%=hds_piva.ClientID() %>";
        var cIdReportSelezionato = "#<%=hdReportSelezionato.ClientID() %>";
        var cId_VegCod = "#<%=hd_VegCod.ClientID() %>";
        var cId_Sa_Cod = "#<%=hd_Sa_Cod.ClientID() %>";

        var cId_Lista_Sa_Cod_Da_Variabili_Stampe = "#<%=hd_Lista_Sa_Cod_Da_Variabili_Stampe.ClientID() %>";
        var cId_Lista_Veg_Cod_Da_Variabili_Stampe = "#<%=hd_Lista_Veg_Cod_Da_Variabili_Stampe.ClientID() %>";

        var cId_Sezione = "#<%=hd_Sezione.ClientID() %>";
        var cId_ElencoReportAbilitati = "#<%=hd_ElencoReportAbilitati.ClientID() %>";
        var cId_ElencoSezioneSuPermessi = "#<%=hd_ElencoSezioneSuPermessi.ClientID() %>";
        var cId_GlobalGapDiv = "#<%=hd_globalGapDiv.ClientID() %>";
        var cId_RegolamentiDiv = "#<%=hd_regolamentiDiv.ClientID() %>";
        var cId_OrdinamentoDiv = "#<%=hd_ordinamentoDiv.ClientID() %>";
        var cId_TempoDiRientroDiv = "#<%=hd_TempoDiRientro.ClientID() %>";
        var cId_VisualizzaImpiantiFiltrati = "#<%=hd_visualizzaImpinatiFiltrati.ClientID() %>";
        var cId_codiciTerreno = "#<%=hd_codiciTerreno.ClientID() %>";
        var cId_hashDestUso = "#<%=hd_hashDestUso.ClientID() %>";
        var cId_gruCod = "#<%=hd_GruCod.ClientID() %>";
        var cId_flagErbOrt = "#<%=hd_Flag_ErbOrt.ClientID() %>";
        var cId_pathAllegati = "#<%=hd_PathAllegati.ClientID() %>";
        var cId_StampaCampagna_Default_Vedi_AvversitaQta = "#<%=hd_StampaCampagna_Default_Vedi_AvversitaQta.ClientID() %>";


        var c_mostaFirmaODC = <%=mostraFirmaODC.ToString.ToLower%>;
        var c_mostraDataOdierna = <%=mostraDataDiStampa.ToString.ToLower %>;
        var c_impostaOrganismoReferente = <%=DefaultCheckImpostaOrganismoReferente.ToString.ToLower %>;

        var breadcrum_Info = "~<%=nomeStampa.ToString() %>";
        idSezioneDashBoard = -1;
        if (typeof creaBreadcrumb === 'function') {
            creaBreadcrumb();
        }

        // print type inside the header
        const breadcrumInfoFormatted = breadcrum_Info.replace(/~/g, '');
        $('#printTypeValue').text(breadcrumInfoFormatted);
        
        $("#btn_daStampareAncheSeVuote").click(btn_daStampareAncheSeVuote);
        $("#btn_MagProdFitosanitari").click(stampaMagazzinoFalse);
        $("#btn_MagFertilizzanti").click(stampaMagazzinoTrue);


        function btn_daStampareAncheSeVuote() {
            $("#SezioniVuoteWindow").data("kendoWindow").center().open();
        }
        
        function img_globalGap() {

            var param = null;
            var datiGGap = "";



            ajaxAgronicaSync(indirizzohttp + "/ImpostaDatiGlobalGap",
                param,
                false,
                function (risposta) {

                    document.getElementById('txt_revisione').value = risposta.RispostaStringa;
                }, null);


            $("#globalGapWindow").data("kendoWindow").center().open();
        }

        function tabellaRotazione() {
            WaitFrame.show();
            Leggi_ColturePrecedenti();
            WaitFrame.hide();
            $("#tabellaRotazione").data("kendoWindow").maximize().open();
        }
        
        function visualizzaElencoReport() {
            Leggi_ElencoReport();
            $("#elencoReportWindow").data("kendoWindow").maximize().open();
            
        }
      <%--  function stampaDiProvaDefinitiva(e) {
            sezioni
            value = e.checked ? 0 : 1;
            $("#<%=RblStampaProva.ClientID%> input").each(function () {
                if ($(this).val() == value) {
                    $(this.click());
                }
            });
        }--%>

    </script>
</asp:Content>
