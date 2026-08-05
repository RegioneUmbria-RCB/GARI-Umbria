


    // -----------------------------------------------------------------
    // TODO  Visibilità dei controlli in base alle autorizzazioni
    // -----------------------------------------------------------------

    

// ---------------------------
// --- Inizio Visibilità ---
// ---------------------------

function Imposta_Visibilita_FormProdottoUC(operazione) {

    // valori operazione
    //  0 visualizzazione
    //  1 inserimento
    //  2 modifica
    //  3 cancellazione
    // 10 copia
    
    if (elencoCausali_Riga !== null && elencoCausali_Riga.length > 1) {

        $("#groupCausale_Riga").show();
        RendiObbligatorio("ddlCausale_Riga", true);

    } else {

        $("#groupCausale_Riga").hide();
        RendiObbligatorio("ddlCausale_Riga", false);

    }

    //TODO Imposta_Pannelli");

    //TODO Impostare a false tutti i campi che di base non si devono vedere");
    
    switch (cIdLavCod) {

        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Ricevuto.value:
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
            //if (raccolteXConferimenti_AbilitazioneGenerale() === 0) {
                $("#panelBar_OrdiniCliente").show();
                $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
            //}
            $("#panelBar_DDTCliente").hide();
            break;

        case enum_LavCod.Fattura_Ricevuta.value:
            if (getKendoSwitch("chkAccompagnatoria")) {
                // Mostro la tab degli ordini
                $("#panelBar_OrdiniCliente").show();
                $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
                $("#panelBar_DDTCliente").hide();
            }
            else {
                // Altrimenti, mostro la tab dei ddt
                $("#panelBar_DDTCliente").show();
                $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_DDTCliente"));
                $("#panelBar_OrdiniCliente").hide();
            }
            break;

        case enum_LavCod.Fattura_Emessa.value:
            if (getKendoSwitch("chkAccompagnatoria")) {
                // Mostro la tab degli ordini
                $("#panelBar_OrdiniCliente").show();
                $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
                $("#panelBar_DDTCliente").hide();
            }
            else {
                // Altrimenti, mostro la tab dei ddt
                $("#panelBar_DDTCliente").show();
                $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_DDTCliente"));
                $("#panelBar_OrdiniCliente").hide();
            }
            break;

        default:
            $("#panelBar_OrdiniCliente").hide();
            $("#panelBar_DDTCliente").hide();
            break;

    }

    ImpostaVisibilita_RifOrdine(cIdLavCod);
    ImpostaVisibilita_NotaDDTEsterna(cIdLavCod);

    $("#panelBar_Magazzini").hide();
    $("#panelBar_Denuncia").hide();
    $("#panelBar_Prodotto").hide();
    $("#panelBar_Imputazioni").hide();
    $("#panelBar_DatiEconomici").hide();

    ImpostaVisibilitaLottoAccettazione();

    //let is_FF = false;
    //if (
    //    ((operazione === enum_TipoOperazioneDB.Lettura.value ||
    //            operazione === enum_TipoOperazioneDB.Modifica.value ||
    //            operazione === enum_TipoOperazioneDB.Copia.value) &&
    //        is_Trasf_Veg_Anim_FormProdottoUC()) ||
    //    (operazione !== enum_TipoOperazioneDB.Lettura.value &&
    //        operazione !== enum_TipoOperazioneDB.Modifica.value &&
    //        operazione !== enum_TipoOperazioneDB.Copia.value &&
    //        is_FF_FormProdottoUC())
    //) {
    //    is_FF = true;
    //}
    //impostaVisibilitaFreshAndFood_FormProdottoUC(is_FF);

    $("#lblParametroQualitativo").hide();
    setKendoSwitchVisible("chkParametroQualitativo", false);

    Visibilita_Degrado(false, false);

    //TODO Decidere per righe e pulsanti   

    //TODO Imposta visibilità
    switch (operazione) {
        case enum_TipoOperazioneDB.Cancellazione.value:
            // Cancellazione
            // TODO ?
            break;

        default:

            switch (Qs_Mode.toLowerCase()) {

                case "contratto":

                    Imposta_ContrattoAffitto(operazione);
                    break;

                // ===================================================================================

                case "magazzino":

                    Imposta_MovimentoMagazzino(operazione);
                    break;
                // ===================================================================================

                case "bolla":

                    Imposta_DocumentoTrasporto(operazione);
                    break;
                // ===================================================================================

                case "fattura":

                    Imposta_Fattura(operazione);
                    break;
                // ===================================================================================

                case "compravendita":

                    Imposta_Compravendita(operazione);
                    break;
                // ===================================================================================
                
                case "trasferimento":

                    Imposta_Trasferimento(operazione);
                    break;
                // ===================================================================================
                
            }
    }

    impostaPanelBarContabilita();

    //impostaVisibilitaDatiEconomici(operazione, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0)));

    Visibilita_DdlProdAlias(gestioneAlias(), false);
}

function impostaVisibilitaBtnNuovoProdotto(elemCod) {

    $("#btnNuovoProdotto").hide();

    if ($("input[name$='hf_utenteAbilitatoProdottiScrittura']").val() == "True") {
        switch (elemCod) {

            case CARBURANTI:
            case SEMENTI:
            case ALTRE_MATERIE:
            case SEMILAVORATI_VEGETALI:
            case MATERIE_VEGETALI:
            case BENI_CONFEZ_VEGETALE:
            case SEMILAVORATI_ANIMALI:
            case MATERIE_ANIMALI:
            case BENI_CONFEZ_ANIMALE:
            case TRASFORMATI_ANIMALI:
            case TRASFORMATI_VEGETALI:
                $("#btnNuovoProdotto").show();
                break;
        }
    }
}

function impostaVisibilitaDatiEconomici(elemCod) {

    //tutto questo è condizionato anche dalla categoria, per cui mi devo assicurare che al cambio della categoria non rispunti fuori

    if ($("input[name$='hf_UtenteAbilitatoGestionePrezziLettura']").val() !== "True") {
        //non ho il permesso di leggere i prezzi
        //non ho necessità di verificare la categoria, perché non posso cmq vedere questo blocco, anche se la categoria lo prevede
        $("#panelBar_DatiEconomici").hide();
    } else {

        //alcune categorie non prevedono i dati economici

        switch (elemCod) {

            case RIGA_DESCRIZIONE_LIBERA:
                $("#panelBar_DatiEconomici").hide();
                break;

            default:

                //mostro il blocco
                $("#panelBar_DatiEconomici").show();

                //in caso di movimenti di magazzino o contratto affitto, visualizza solo il prezzo
                if (isMovimentoMagazzino(cIdLavCod) === true || isContrattoAffitto()) {
                    $("#btn_ricerca_prezzo_listino").hide();
                    $("#prezzoRiferitoAGroup").hide();
                    $("#valoreRiferimentoGroup").hide();
                    $("#imponibileGroup").hide();
                    $("#cardIva").hide();
                    $("#cardSconti").hide();
                    $("#id_prezzo_sconto_ricavati").hide();
                    $("#imponibileNettoGroup").hide();
                    $("#id_importi").hide();
                }

                if (isContrattoAffitto()) {
                    $("#groupCategorieMagazzino").hide();
                    $("#btnNuovoProdotto").hide();                    
                }

                if ($("input[name$='hf_UtenteAbilitatoGestionePrezziScrittura']").val() !== "True") {
                    //il mio utente non ha i permessi per modificare i prezzi
                    impostaReadOnlyPlusKendo("#panelBar_DatiEconomici", true);
                } else {
                    //ho i permessi per fare tutto ==> business as usual (se ho aperto la riga in lettura verranno cmq bloccati tutti i campi)
                }

        }

    }

}

function impostaReadOnlyPlusKendo(selectorPadre, flagReadOnly) {
    $(selectorPadre).find("input").attr("readonly", flagReadOnly);
    $(selectorPadre).find("textarea").attr("readonly", flagReadOnly);
    //if (flagReadOnly === true) {
        $(selectorPadre).find(":radio").attr("disabled", flagReadOnly);
        $(selectorPadre).find(":checkbox").attr("disabled", flagReadOnly);
    //} else {
    //    $(selectorPadre).find(":radio").removeAttr("disabled");
    //    $(selectorPadre).find(":checkbox").removeAttr("disabled");
    //}


    //ddl
    $(selectorPadre).find("[data-role='dropdownlist']").each(function (item, index) {
        KendoDDL(this.id).readonly(flagReadOnly);
    });

    //multi select
    $(selectorPadre).find("[data-role='multiselect']").each(function (item, index) {
        KendoMultisel(this.id).readonly(flagReadOnly);
    });

    //calendar
    $(selectorPadre).find("[data-role='datepicker']").each(function (item, index) {
        KendoDate(this.id).readonly(flagReadOnly);
    });

    //numeric
    $(selectorPadre).find("[data-role='numerictextbox']").each(function (item, index) {
        KendoNumTB(this.id).readonly(flagReadOnly);
    });

    //switch
    $(selectorPadre).find("[data-role='switch']").each(function (item, index) {
        KendoSwitch(this.id).readonly(flagReadOnly);
    });
}

function impostaPanelBarContabilita() {

    // Faccio vincere sempre questa impostazione generale
    switch (gestioneContabilita) {
        case enum_Livello_GestContabilita_NonGestita:
            $("#panelBar_Imputazioni").hide();
            break;

        case enum_Livello_GestContabilita_Fatturazione:
            $("#panelBar_Imputazioni").hide();
            break;

        case enum_Livello_GestContabilita_Bilancio:
            break;

        default:
            break;
    }
}

function Imposta_ContrattoAffitto(operazione) {

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    $("#panelBar_Prodotto").show();

    // Pendenza

    hf_PendenzaIniziale = enum_Pendenza.MovPendente;

    // Lotto Impianto e Calibro

    Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  // TODO Verificare se va bene dft ""
    Set_KendoDDLValueNoDef("ddlCalibro", 0);
    KendoDDL("ddlCalibro").enable(false);
    KendoDDL("ddlLottoImpianto").enable(false);

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, ddlCategorieMagazzinoValue);

}

function Imposta_MovimentoMagazzino(operazione) {

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    // CAU_MOV = Qs_CaricoScarico
    $("#panelBar_Prodotto").show();

    hf_PendenzaIniziale = enum_Pendenza.MovPendente;

    //Set_KendoDDLValueNoDef("ddlProdottoDes", -1);

    //Set_KendoDDLValueNoDef("ddlLottoAccettazione", "");

    Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  // TODO Verificare se va bene dft ""
    Set_KendoDDLValueNoDef("ddlCalibro", 0);
      
    // Disabilito momentaneamente le combo del lotto e del calibro
    KendoDDL("ddlCalibro").enable(false);
    KendoDDL("ddlLottoImpianto").enable(false);

    //AzzeraQuantita();
    //AzzeraPrezziScontiImporti();

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            // TODO Testare
            ////set_data("idDataMovimento", formattedDate(new Date(Qs_DataSelezionata), '/'), null);
            // Ora dovrebbe essere già ok

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }
    
    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, ddlCategorieMagazzinoValue );

}

function Visibilita_Mag_Provenienza(flagVisibile, flagObbligatorio) {
    if (flagVisibile === true) {
        $("#groupUbicProvenienza").show();
        $("#groupGiacenzaProvenienza").show();

        RendiObbligatorio("ddlUbicProvenienza", flagObbligatorio);
    } else {
        $("#groupUbicProvenienza").hide();
        $("#groupGiacenzaProvenienza").hide();

        RendiObbligatorio("ddlUbicProvenienza", false);
    }
}

function Visibilita_Mag_Destinazione(flagVisibile, flagObbligatorio) {
    if (flagVisibile === true) {
        $("#groupUbicDestinazione").show();
        $("#groupGiacenzaDestinazione").show();

        RendiObbligatorio("ddlUbicDestinazione", flagObbligatorio);
    } else {
        $("#groupUbicDestinazione").hide();
        $("#groupGiacenzaDestinazione").hide();

        RendiObbligatorio("ddlUbicDestinazione", false);
    }
}

function Imposta_DocumentoTrasporto(operazione) {

    switch (Qs_Tipo.toLowerCase()) {

        case CAU_MAGAZZINO:

            $("#panelBar_Prodotto").show();

            hf_PendenzaIniziale = enum_Pendenza.MovPendente;
            break;

    }

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            // TODO Testare
            ////set_data("idDataMovimento", formattedDate(new Date(Qs_DataSelezionata), '/'), null);
            // Ora dovrebbe essere già ok

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }

    if (Qs_CaricoScarico === CAU_CARICO) {

        // CARICO
        Imposta_DocumentoTrasporto_Ricevuto();

    } else if (Qs_CaricoScarico === CAU_SCARICO) {

        // SCARICO
        Imposta_DocumentoTrasporto_Emesso();

    } 
}

function Imposta_DocumentoTrasporto_Ricevuto() {

    let categoria = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, categoria);

    // Conferimento: non mostro diversi campi
    if (lavCodAccettazione) {
        $("#idParametroQualitativoCalibro").hide();
        $("#lblCalibro").hide();
        KendoDDL("ddlCalibro").wrapper.hide();
        $("#lblParametroQualitativo").hide();
        setKendoSwitchVisible("chkParametroQualitativo", false);

        $("#groupLottoImpianto").hide();
        $("#groupAggregaLottoImpianto").hide();
        ClearDDL("ddlLottoImpianto");
        Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
        //KendoDDL("ddlLottoImpianto").wrapper.hide();

        $("#lblAggregaLottoImpianto").hide();
        setKendoSwitchVisible("chkAggregaLottoImpianto", false);

    }  

}

function Imposta_DocumentoTrasporto_Emesso() {

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));

}

function Imposta_Fattura(operazione) {

    hf_PendenzaIniziale = enum_Pendenza.MovPendente;

    // TODO verificare se nel Lan c'è
    //If Qs_RagSocContatto <> "" Then
    //    Me.Chk_Conti.Visible = True
    //    Me.Chk_Conti.Text = Me.Chk_Conti.Text + Qs_RagSocContatto
    //Else 
    //    Me.Chk_Conti.Visible = False
    //End If


    switch (Qs_Tipo.toLowerCase()) {

        case CAU_MAGAZZINO:

            $("#panelBar_Prodotto").show();
            break;

        case CAU_ANIMALE:

        // Non gestito
        //Me.Pannello_Animale.Visible = True
        //Imposta_ZOO()
            break;
    }

    // TODO
    Carica_Pannello_Economico();

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            // TODO Testare
            ////set_data("idDataMovimento", formattedDate(new Date(Qs_DataSelezionata), '/'), null);
            // Ora dovrebbe essere già ok

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }

    if (Qs_CaricoScarico === CAU_CARICO) {

        Imposta_Fattura_Ricevuta();

    } else if (Qs_CaricoScarico === CAU_SCARICO) {

        Imposta_Fattura_Emessa();

    }

}

function Imposta_Fattura_Ricevuta() {

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));
    
}

function Imposta_Fattura_Emessa() {

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));

}

//TODO Compravendita
function Imposta_Compravendita(operazione) {

    $("#panelBar_Prodotto").show();

    // TODO  Me.Chk_Conti.Visible = False

    hf_PendenzaIniziale = enum_Pendenza.MovPendente;
     
    // TODO
    Carica_Pannello_Economico();

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            // TODO Testare
            ////set_data("idDataMovimento", formattedDate(new Date(Qs_DataSelezionata), '/'), null);
            // Ora dovrebbe essere già ok

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }

    if (Qs_CaricoScarico === CAU_CARICO) {

        Imposta_Compravendita_Acquisto();

    } else if (Qs_CaricoScarico === CAU_SCARICO) {

        Imposta_Compravendita_Vendita();

    }

}

function Imposta_Compravendita_Acquisto() {

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));

}

function Imposta_Compravendita_Vendita() {

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));

}

function Imposta_Trasferimento(operazione) {

    $("#panelBar_Prodotto").show();
    AbilitaSalvataggio();
  
    Set_KendoDDLValueNoDef("ddlProdottoDes", -1);
    
    Set_KendoDDLValueNoDef("ddlLottoAccettazione", "");
    Set_KendoDDLValueNoDef("ddlLottoImpianto", ""); // TODO Verificare se va bene dft ""
    Set_KendoDDLValueNoDef("ddlCalibro", 0);
     
    // Disabilito momentaneamente le combo del lotto e del calibro
    KendoDDL("ddlCalibro").enable(false);
    KendoDDL("ddlLottoImpianto").enable(false);

    //AzzeraQuantita();
    //AzzeraPrezziScontiImporti();

    switch (operazione) {
        // Visualizzazione
        case enum_TipoOperazioneDB.Lettura.value:

            DisabilitaSalvataggio();
            break;

        // Inserimento
        case enum_TipoOperazioneDB.Scrittura.value:

            // TODO Testare
            ////set_data("idDataMovimento", formattedDate(new Date(Qs_DataSelezionata), '/'), null);
            // Ora dovrebbe essere già ok

            AbilitaSalvataggio();
            break;

        // Modifica
        case enum_TipoOperazioneDB.Modifica.value:

            AbilitaSalvataggio();
            break;

        // Copia
        case enum_TipoOperazioneDB.Copia.value:

            AbilitaSalvataggio();
            break;
    }

    ImpostaVisibilita_PerCategoria(CAU_TRASFERIMENTO, parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")));

}

function Carica_Pannello_Economico() {
    // TODO Vedi .vb
}

function ImpostaVisibilitaElencoOSingolaRiga(formSingolaRiga) {

    // Il pulsante esci viene mostrato solo se si entra in visualizzazione
    MostraBtnEsciRiga(false);

    // chiudo e nascondo il tab ordini cliente
    //$("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
    //$("#panelBar_OrdiniCliente").hide();

    // Se formSingolaRiga è true mostro form riga di dettaglio e non mostro elenco righe e scarichi da giacenze, altrimenti viceversa
    // Se mostro form riga di dettaglio in base al lav_cod definisco se mostrare TAB impianti legati a entrata conferimento  
    if (formSingolaRiga) {
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Riepilogo]).attr("style", "display:none");
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Dettaglio]).attr("style", "display:inline-block");
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");
        tabStrip_Dettagli.select(index_tabStrip_Dettagli_Dettaglio);

    } else {
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Riepilogo]).attr("style", "display:inline-block");
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Dettaglio]).attr("style", "display:none");
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).attr("style", "display:none");
        tabStrip_Dettagli.select(index_tabStrip_Dettagli_Riepilogo);

        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none"); //TODO
        //if (Qs_CaricoScarico !== CAU_CARICO) {
        //    //TODO RIPRISTINARE $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:inline-block");
        //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");
        //} else {
        //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");
        //}

    }

}

function ImpostaVisibilitaLottoAccettazione() {

    $("#groupTxtLottoAccettazione").hide();
    $("#groupDdlLottoAccettazione").hide();
    $("#groupConfezionamentoLotto").hide();

    Set_KendoDDLValue("ddlConfezionamentoLotto", "");

    RendiObbligatorioLottoAccettazione("txtLottoAccettazione", false)
    RendiObbligatorioLottoAccettazione("ddlLottoAccettazione", false)

    // Se il lotto non è gestito non mostro nulla
    let w_gest_lotti = getGestioneLotti();

    if (w_gest_lotti === enum_Gestione_Lotti_Nessuna) {

        Set_KendoDDLValue("ddlLottoAccettazione", "");
        $("#txtLottoAccettazione").val("");

    } else {

        if (Qs_CaricoScarico === CAU_CARICO) {

            $("#groupTxtLottoAccettazione").show();
            if (GestitoConfezionamentoLotto()) {
                $("#groupConfezionamentoLotto").show();
            }

            if (!impostazioni_Edit_Lotto_Accettazione &&
                lavCodAccettazione)
                $("#txtLottoAccettazione").attr("readonly", true);

            if (w_gest_lotti === enum_Gestione_Lotti_Obbligatoria) {

                RendiObbligatorioLottoAccettazione("txtLottoAccettazione", true);

            }

        } else if (Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO) {

            $("#groupDdlLottoAccettazione").show();
             
            if (w_gest_lotti === enum_Gestione_Lotti_Obbligatoria) {

                RendiObbligatorioLottoAccettazione("ddlLottoAccettazione", true)

            }
        }

    }

    GestisciDataScadenzaLotto(w_gest_lotti);
    
}

function RendiObbligatorioLottoAccettazione(idControllo, flagObbligatorio) {

    if (lavCodAccettazione === true) {

        // In caso di accettazione il lotto viene proposto al salvataggio, per cui modifico solo la label

        let labelForIdControllo = 'label[for="' + idControllo + '"]';

        if (flagObbligatorio === true) {

            $(labelForIdControllo).addClass("campiObbligatori");

        } else {

            $(labelForIdControllo).removeClass("campiObbligatori");

        }        

    } else {

        RendiObbligatorio(idControllo, flagObbligatorio);

    }

}

//Imposta visibilità magazzino ed altri campi
function ImpostaVisibilita_PerCategoria(cauMov, elemCod) {

    //a parte perché deve anche intersecarsi con un permesso particolare
    impostaVisibilitaDatiEconomici(elemCod);

    switch (elemCod) {

        case RIGA_DESCRIZIONE_LIBERA:

            Visibilita_TxtBeniStrumentali(true, true);

            Visibilita_TxtExtra_Str(false, false);
            Visibilita_DdlProdotto(false, false);
            $("#panelBar_Quantita").hide();

            Visibilita_UnitaMisura(false, false);

            Visibilita_Quantita(false, false);
            Visibilita_QuantitaRiscontrata(false, false);
            Visibilita_KgNetti(false, false);
            Visibilita_KgLordi(false, false);
            Visibilita_KgNettiRiscontrati(false, false);
            Visibilita_KgLordiRiscontrati(false, false);

            Visibilita_Tara(false, false);
            Visibilita_TaraRiscontrata(false, false);

            Visibilita_BtnRiscontrati(false);

            $("#panelBar_Imputazioni").hide();
            $("#idNote").hide();
            
            //non usano magazzino
            $("#panelBar_Magazzini").hide();
            Visibilita_Mag_Provenienza(false, false);
            Visibilita_Mag_Destinazione(false, false);
            Visibilita_Degrado(false, false);

            break;

        case ALTRI_BENI:
            
            Visibilita_TxtBeniStrumentali(true, true);
            Visibilita_TxtExtra_Str(true, false);   //TODO: TxtExtra_Str Visibile ?!?
            Visibilita_DdlProdotto(false, false);
            $("#panelBar_Quantita").show();
            Visibilita_UnitaMisura(true, true);
            Visibilita_Quantita(true, true);
            $("#panelBar_Imputazioni").show();
            $("#idNote").hide();

            //non usano magazzino
            $("#panelBar_Magazzini").hide();
            Visibilita_Mag_Provenienza(false, false);
            Visibilita_Mag_Destinazione(false, false);

            impostaVisibilitaFreshAndFood_FormProdottoUC(is_Trasf_Veg_Anim_FormProdottoUC());

            break;

        case SERVIZI:

            Visibilita_TxtBeniStrumentali(false, false);
            Visibilita_TxtExtra_Str(true, false);   //TODO: TxtExtra_Str Visibile ?!?
            Visibilita_DdlProdotto(true, true);
            $("#panelBar_Quantita").show(); 
            Visibilita_UnitaMisura(true, true);
            Visibilita_Quantita(true, true);
            $("#panelBar_Imputazioni").show();
            $("#idNote").hide();

            //non usano magazzino
            $("#panelBar_Magazzini").hide();
            Visibilita_Mag_Provenienza(false, false);
            Visibilita_Mag_Destinazione(false, false);

            impostaVisibilitaFreshAndFood_FormProdottoUC(is_Trasf_Veg_Anim_FormProdottoUC());

            break;

        default:

            Visibilita_TxtBeniStrumentali(false, false);

            if (elemCod === FERTILIZZANTI || elemCod === FORMULATI || elemCod === INNESCHI) {
                Visibilita_TxtExtra_Str(false, false);
            }
            else {
                Visibilita_TxtExtra_Str(true, false);   //TODO: TxtExtra_Str Visibile ?!?
            }
            Visibilita_DdlProdotto(true, true);
            if (FF_gest_materiale_vivaistico || lavCodAccettazionePomodoro)
                Visibilita_UnitaMisura(false, false);
            else
                Visibilita_UnitaMisura(true, true);

            if (isContrattoAffitto()) {

                $("#panelBar_Quantita").hide();

            } else {

                $("#panelBar_Quantita").show();

            }

            impostaVisibilitaFreshAndFood_FormProdottoUC(is_Trasf_Veg_Anim_FormProdottoUC()); 
            
            $("#panelBar_Imputazioni").show();

            if (isContrattoAffitto()) {

                $("#panelBar_Magazzini").hide();
                Visibilita_Mag_Provenienza(false, false);
                Visibilita_Mag_Destinazione(false, false);

            } else {

                //usano magazzino

                $("#panelBar_Magazzini").show();
                if (cauMov === CAU_CARICO) {

                    // CARICO
                    Visibilita_Mag_Provenienza(false, false);
                    Visibilita_Mag_Destinazione(true, true);

                } else if (cauMov === CAU_SCARICO) {

                    // SCARICO
                    Visibilita_Mag_Provenienza(true, true);
                    Visibilita_Mag_Destinazione(false, false);

                } else if (cauMov === CAU_TRASFERIMENTO) {

                    // TRASFERIMENTO
                    Visibilita_Mag_Provenienza(true, true);
                    Visibilita_Mag_Destinazione(true, true);
                }

            }

    }

}


function isVisibile_RifOrdine(lavCod) {
    let isVisibile = false;

    switch (lavCod) {
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.DDT_Ricevuto.value:
            isVisibile = true;
        break;

        default:
            isVisibile = false;
        break;
    }

    return isVisibile;
}

function ImpostaVisibilita_RifOrdine(lavCod) {
    let isvisibile = isVisibile_RifOrdine(lavCod);
    $("#colRigaRifNOrdine").toggle(isvisibile);
    $("#colRigaRifDataOrdine").toggle(isvisibile);
}


function isVisibile_NotaDDTEsterna(lavCod) {
    let isVisibile = false;

    switch (lavCod) {
    case enum_LavCod.DDT_Emesso.value:
    case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        isVisibile = true;
        break;

    default:
        isVisibile = false;
        break;
    }

    return isVisibile;
}

function ImpostaVisibilita_NotaDDTEsterna(lavCod) {
    let isvisibile = isVisibile_NotaDDTEsterna(lavCod);
    $("#colNotaDDTEsterna").toggle(isvisibile);
    $("#colNotaRigaDDTEsterna").toggle(isvisibile);
    $("#colNotaDataDDTEsterna").toggle(isvisibile);
}

function Visibilita_PUARegolamento(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupPUARegolamento").show();
        RendiObbligatorio("ddlPUARegolamento", flagObbligatorio);
    } else {
        $("#groupPUARegolamento").hide();
        RendiObbligatorio("ddlPUARegolamento", false);
    }
}

function Visibilita_DdlProdotto(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupDdlProdotto").show();
        RendiObbligatorio("ddlProdottoDes", flagObbligatorio);
    } else {
        $("#groupDdlProdotto").hide();
        RendiObbligatorio("ddlProdottoDes", false);
    }
}



function Visibilita_ParametriIndici(elemCod) {

    switch (elemCod) {

        case RIGA_DESCRIZIONE_LIBERA: case ALTRI_BENI: 

            $("#id_parametri_indici_list_GHG div").html("");
            $("#panelBar_ParametriGhG").hide();

            /*$("#id_parametri_qualitativi_list div").html("");*/

            parametri_indici_creati = false;

            break;

        // TUTTO IL RESTO
        default:

            //se esistono indici precendentemente il div era nascosto --> ricarico (vedi passaggio da descrizione libera a trasformato)
            if (foundParamQualIndici == true && parametri_indici_creati == false) {

                //Ricarica Parametri GHG                
                creaParametriQualitativi_Indici_GHG()                

            }

            break;
    }
}



function Visibilita_DdlProdAlias(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupDdlProdAlias").show();
        RendiObbligatorio("ddlProdAlias", flagObbligatorio);
    } else {
        $("#groupDdlProdAlias").hide();
        RendiObbligatorio("ddlProdAlias", false);
    }
}

function Visibilita_Ferti_Dettagli(flagVisibilita, flagObbligatorio) {
    Visibilita_Ferti_N(flagVisibilita, flagObbligatorio);
    Visibilita_Ferti_P2O5(flagVisibilita, flagObbligatorio);
    Visibilita_Ferti_K2O(flagVisibilita, flagObbligatorio);
    Visibilita_Ferti_Cu(flagVisibilita, flagObbligatorio);
}

function Visibilita_Ferti_N(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#bloccoN").show();
        RendiObbligatorio("idTxt_N", flagObbligatorio);
    } else {
        $("#bloccoN").hide();
        RendiObbligatorio("idTxt_N", false);
    }
}

function Visibilita_Ferti_P2O5(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#bloccoP2O5").show();
        RendiObbligatorio("idTxt_P2O5", flagObbligatorio);
    } else {
        $("#bloccoP2O5").hide();
        RendiObbligatorio("idTxt_P2O5", false);
    }
}

function Visibilita_Ferti_K2O(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#bloccoK2O").show();
        RendiObbligatorio("idTxt_K2O", flagObbligatorio);
    } else {
        $("#bloccoK2O").hide();
        RendiObbligatorio("idTxt_K2O", false);
    }
}

function Visibilita_Ferti_Cu(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#bloccoCu").show();
        RendiObbligatorio("idTxt_Cu", flagObbligatorio);
    } else {
        $("#bloccoCu").hide();
        RendiObbligatorio("idTxt_Cu", false);
    }
}

function Visibilita_TxtBeniStrumentali(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupBeniStrumentali").show();
        RendiObbligatorio("txtBeniStrumentali", flagObbligatorio);
    } else {
        $("#groupBeniStrumentali").hide();
        RendiObbligatorio("txtBeniStrumentali", false);
    }
}

function Visibilita_TxtExtra_Str(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupExtra_Str").show();
        RendiObbligatorio("txtExtra_Str", flagObbligatorio);
    } else {
        $("#groupExtra_Str").hide();
        RendiObbligatorio("txtExtra_Str", false);
    }
}

function Visibilita_Quantita(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupQuantita").show();
        RendiObbligatorio("idQuantita", flagObbligatorio);
        RendiGreaterThan0("idQuantita", flagObbligatorio);
    } else {
        $("#groupQuantita").hide();
        RendiObbligatorio("idQuantita", false);
        RendiGreaterThan0("idQuantita", false);
    }
}

function Visibilita_QuantitaRiscontrata(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupQuantitaRiscontrata").show();
        RendiObbligatorio("idQuantitaRiscontrata", flagObbligatorio);
    } else {
        $("#groupQuantitaRiscontrata").hide();
        RendiObbligatorio("idQuantitaRiscontrata", false);
    }
}

function Visibilita_KgNetti(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupKgNetti").show();
        RendiObbligatorio("idKgNetti", flagObbligatorio);
        //RendiGreaterThan0("idKgNetti", flagObbligatorio);
    } else {
        $("#groupKgNetti").hide();
        RendiObbligatorio("idKgNetti", false);
        //RendiGreaterThan0("idKgNetti", false);
    }
}

function Visibilita_KgLordi(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupKgLordi").show();
        RendiObbligatorio("idKgLordi", flagObbligatorio);
        //RendiGreaterThan0("idKgLordi", flagObbligatorio);
    } else {
        $("#groupKgLordi").hide();
        RendiObbligatorio("idKgLordi", false);
        //RendiGreaterThan0("idKgLordi", false);
    }
}

function Visibilita_Tara(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupTara").show();
        RendiObbligatorio("idTara", flagObbligatorio);
        RendiGreaterThan0("idTara", flagObbligatorio);
    } else {
        $("#groupTara").hide();
        RendiObbligatorio("idTara", false);
        RendiGreaterThan0("idTara", false);
    }
}

function Visibilita_KgNettiRiscontrati(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupKgNettiRiscontrati").show();
        RendiObbligatorio("idKgNettiRiscontrati", flagObbligatorio);
        //RendiGreaterThan0("idKgNettiRiscontrati", flagObbligatorio);
    } else {
        $("#groupKgNettiRiscontrati").hide();
        RendiObbligatorio("idKgNettiRiscontrati", false);
        //RendiGreaterThan0("idKgNettiRiscontrati", false);
    }
}

function Visibilita_KgLordiRiscontrati(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupKgLordiRiscontrati").show();
        RendiObbligatorio("idKgLordiRiscontrati", flagObbligatorio);
        //RendiGreaterThan0("idKgLordiRiscontrati", flagObbligatorio);
    } else {
        $("#groupKgLordiRiscontrati").hide();
        RendiObbligatorio("idKgLordiRiscontrati", false);
        //RendiGreaterThan0("idKgLordiRiscontrati", false);
    }
}

function Visibilita_TaraRiscontrata(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupTaraRiscontrata").show();
        RendiObbligatorio("idTaraRiscontrata", flagObbligatorio);
        RendiGreaterThan0("idTaraRiscontrata", flagObbligatorio);
    } else {
        $("#groupTaraRiscontrata").hide();
        RendiObbligatorio("idTaraRiscontrata", false);
        RendiGreaterThan0("idTaraRiscontrata", false);
    }
}

function Visibilita_BtnRiscontrati(flagVisibilita) {
    $("#boxFunzioniPesiRiscontrati").toggle(flagVisibilita);
}

function Visibilita_Degrado(flagVisibilita, flagObbligatorio) {

    if (flagVisibilita === true) {
        $("#groupDegradoPerc").show();
        $("#groupDegradoKg").show();
        $("#groupDegradoKgEffettivi").show();
        RendiObbligatorio("idDegradoPerc", flagObbligatorio);
    } else {
        $("#groupDegradoPerc").hide();
        $("#groupDegradoKg").hide();
        $("#groupDegradoKgEffettivi").hide();
        RendiObbligatorio("idDegradoPerc", false);
    }
}

function Gestione_Visibilita_Degrado(tipoAccesso_Degrado) {

    if (lavCodAccettazionePomodoro || lavCodAccettazione) {

        switch (tipoAccesso_Degrado) {

            //caso controllo facoltativo
            case enum_DegradoVisibilita.Facoltativo:
                Visibilita_Degrado(true, false);
                break;

            //caso controllo obbligatorio
            case enum_DegradoVisibilita.Obbligatorio:
                Visibilita_Degrado(true, true);
                break;

            //caso controllo non gestito
            case enum_DegradoVisibilita.Non_Gestito:
                Visibilita_Degrado(false, false);
                break;

            //caso default = facoltativo
            default:
                Visibilita_Degrado(true, false);
                break;

        }

    } else {

        Visibilita_Degrado(false, false);

    }
    
}

function Visibilita_UnitaMisura(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupUnitaMisura").show();
        RendiObbligatorio("ddlUM", flagObbligatorio);
    } else {
        $("#groupUnitaMisura").hide();
        RendiObbligatorio("ddlUM", false);
    }
}

function Visibilita_DoseEtichetta(flagVisibilita, flagObbligatorio) {
    if (flagVisibilita === true) {
        $("#groupDoseEtichetta").show();
        RendiObbligatorio("txtDoseEtichetta", flagObbligatorio);
    } else {
        $("#groupDoseEtichetta").hide();
        RendiObbligatorio("txtDoseEtichetta", false);
    }
}

function isImputazioneImpiantiAttiva() {
    let attivaImputazioneImpianti = false;

    //setup abilitato
    //lavCodAccettazione true
    //isContattoImpresaGias true
    //is_210_FormProdottoUC true
    //FF_gest_materiale_vivaistico false
    //raccolte pre-esistenti associate a false
    // → in scrittura controllare se l'array globale ha almeno un valore
    // → in modifica controllare se i valori dell'array globale che potevano essere pre-associati per l'id_mov_det, abbiano tipo_associazione = 0
    // In sostanza l'utente lavorando sulla pagina, può modificare la categoria prodotto e le raccolte associate

    if (imputazioneImpianti_AbilitazioneGenerale() && (isContattoImpresaGias || isProduttoreImpresaGias) && is_210_FormProdottoUC()) {

        let numRaccolte = raccolteConfUC_set(); // Prelevo solamente il numero totale di raccolte disponibili
        if (numRaccolte === 0) {
            attivaImputazioneImpianti = true;
        }
        else {
            let isOperazioneScrittura = riga_originale_entrata_FormProdottoUC === undefined || riga_originale_entrata_FormProdottoUC === null ? true : false;
            //let associateRaccoltePreEsistenti = false;
            if (isOperazioneScrittura === true) {
                attivaImputazioneImpianti = raccolteXConferimenti_dsSelezionate.length === 0;
            }
            else {
                let dsRaccolteSelPreEsistenti = raccolteXConferimenti_dsSelezionate.filter(function (dr) {
                    return parseInt(dr.Tipo_Associazione) === 1;
                });

                attivaImputazioneImpianti = dsRaccolteSelPreEsistenti.length === 0;
            }
        }
    }

    return attivaImputazioneImpianti;
}

function isRaccolteXConferimentiAttive() {
    let attivaRaccolteConf = false;

    if (raccolteXConferimenti_AbilitazioneGenerale() && (isContattoImpresaGias || isProduttoreImpresaGias) && is_210_FormProdottoUC()) {

        let numRaccolte = raccolteConfUC_set(); // Prelevo solamente il numero totale di raccolte disponibili

        if (numRaccolte > 0) {

            let isOperazioneScrittura = riga_originale_entrata_FormProdottoUC === undefined || riga_originale_entrata_FormProdottoUC === null ? true : false;
            if (isOperazioneScrittura === true) {
                attivaRaccolteConf = true;
            }
            else {
                let idMovDetConf = parseInt(riga_originale_entrata_FormProdottoUC.Id_Mov_Det);

                let dsRaccolteSelCreateAutom = raccolteXConferimenti_dsSelezionate.filter(function (dr) {
                    return idMovDetConf === parseInt(dr.Conf_Id_Mov_Det) && parseInt(dr.Tipo_Associazione) === 0;
                });

                attivaRaccolteConf = dsRaccolteSelCreateAutom.length === 0;
            }
        }
        
    }

    return attivaRaccolteConf;
}

function Visibilita_PanelRaccolte(flagVisibilita) {
    if (flagVisibilita === true) {
        $("#panelBar_RaccolteXConferimenti").show();
        $("#raccolteXConferimenti_btnAssocia").prop("disabled", false);
        //$("#panelbarFormProdottoUC").data("kendoPanelBar").expand("#panelBar_RaccolteXConferimenti");
    }
    else {
        $("#panelBar_RaccolteXConferimenti").hide();
        $("#raccolteXConferimenti_btnAssocia").prop("disabled", true);
        //$("#panelbarFormProdottoUC").data("kendoPanelBar").collapse("#panelBar_RaccolteXConferimenti");
    }
}

function Titolo_PanelRaccolte() {
    // TODO Creare una descrizione più parlante una volta creata l'impostazione per definire il periodo delle raccolte visibili:
    // data del conferimento - x giorni
    $("#panelBar_RaccolteXConferimenti > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "Raccolte", "Raccolte").toUpperCase());
}

function RendiObbligatorio(idControllo, flagObbligatorio) {
    let nameControllo = $("#" + idControllo)[0].name;

    if (flagObbligatorio === true) {
        $("#" + idControllo).attr("required", "");
        $('label[for="' + nameControllo + '"]').addClass("campiObbligatori");
    } else {
        $("#" + idControllo).removeAttr("required");
        $('label[for="' + nameControllo + '"]').removeClass("campiObbligatori");
    }
}

function RendiGreaterThan0(idControllo, flagObbligatorio) {
    if (flagObbligatorio === true) {
        $("#" + idControllo).addClass("GreaterThan0");
    } else {
        $("#" + idControllo).removeClass("GreaterThan0");
    }
}

function GestitoConfezionamentoLotto() {

    if (elencoConfezionamentoLotto.length <= 1) {
        return false
    }

    // Il confezionamento del lotto viene gestito solo su documenti di acquisto
    // (Ordine Acquisto, DDT Ricevuto, Fattura Accompagnatoria Ricevuta, Carico Magazzino) 
    // per i soli prodotti di banca dati (FERTILIZZANTI, FORMULATI, INSETTI, INNESCHI, TRAPPOLE)

    let documentoAcquisto = false;

    if (Qs_CaricoScarico === CAU_CARICO) {

        if (cIdLavCod === enum_LavCod.Ordine_Acquisto.value ||
            cIdLavCod === enum_LavCod.DDT_Ricevuto.value ||
            cIdLavCod === enum_LavCod.Carico_Magazzino.value) {
            documentoAcquisto = true;
        }

        if (cIdLavCod === enum_LavCod.Fattura_Ricevuta.value && getKendoSwitch("chkAccompagnatoria") == true) {
            documentoAcquisto = true;
        }

        if (cIdLavCod === enum_LavCod.Nota_Accredito_Ricevuta.value && getKendoSwitch("chkAccompagnatoria") == true) {
            documentoAcquisto = true;
        }

    }

    let prodottoBancaDati = false;

    if (documentoAcquisto ===true && KendoDDL("ddlCategorieMagazzino") !== undefined) {

        ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

        if (ddlCategorieMagazzinoValue === FERTILIZZANTI ||
            ddlCategorieMagazzinoValue === FORMULATI ||
            ddlCategorieMagazzinoValue === INSETTI ||
            ddlCategorieMagazzinoValue === INNESCHI ||
            ddlCategorieMagazzinoValue === TRAPPOLE) {
            prodottoBancaDati = true;
        }

    }

    if (documentoAcquisto && prodottoBancaDati) {

        return true

    }

    return false

}

/**
 * Restituisce il valore dell'impostazione per l'elem_cod attualmente selezionato
 * @returns {int} Valori possibili 0 = enum_Gestione_Lotti_Nessuna, 1 = enum_Gestione_Lotti_Obbligatoria, 2 = enum_Gestione_Lotti_Facoltativa
 */
function ValImpDataScadLotto() {

    let gestDataScad = enum_Gestione_Lotti_Nessuna;

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    if (ddlCategorieMagazzinoValue !== 0 && impostazioni_DataScadenza_Lotto !== undefined && impostazioni_DataScadenza_Lotto !== null && impostazioni_DataScadenza_Lotto.length > 0) {

        let itemCat = arrayLookup(impostazioni_DataScadenza_Lotto, "Elem_Cod", ddlCategorieMagazzinoValue);
        if (itemCat !== undefined && itemCat !== null) {
            gestDataScad = itemCat.Impostazione_Valore;
        }

        //avevo impostato il lotto come obbligatorio in questa categorie, ma visto che mi trovo in un ordine, sovrascrivo l'impostazione per dire che è facoltativo
        if (lavCodOrdine === true && gestDataScad === enum_Gestione_Lotti_Obbligatoria) {
            gestDataScad = enum_Gestione_Lotti_Facoltativa;
        }
    }

    return gestDataScad;
}

function GestisciDataScadenzaLotto(gestLotto) {

    if (gestLotto === enum_Gestione_Lotti_Nessuna || Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO) {
        $("#divColDataScadenza").hide();
        RendiObbligatorio("dpDataScadenza", false);
    }
    else {
        let gestDataScad = ValImpDataScadLotto();

        if (gestDataScad === enum_Gestione_Lotti_Nessuna) {
            $("#divColDataScadenza").hide();
            RendiObbligatorio("dpDataScadenza", false);
        }
        else if (gestDataScad === enum_Gestione_Lotti_Facoltativa) {
            $("#divColDataScadenza").show();
            RendiObbligatorio("dpDataScadenza", false);
        }
        else if (gestDataScad === enum_Gestione_Lotti_Obbligatoria) {
            $("#divColDataScadenza").show();
            RendiObbligatorio("dpDataScadenza", gestLotto === enum_Gestione_Lotti_Obbligatoria);
        }
    }
}
