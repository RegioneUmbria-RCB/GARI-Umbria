
var indirizzohttp = "./New_Contatto_Edit.aspx";
var indirizzohttp_Ricerca_Documenti = "Contab/RicercaDocumenti.asmx";

function Contatto_RapportiContabili(options) {

    var apertoDaPopup = $(Controls.AperturaDaPopup).val();
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    if (apertoDaPopup == "True" && tipoCarica == 1) {
        //var cod_Rapporto = $(Controls.PopupCodRapporto).val();
        //SelezionaKendoDropDownItem("ddl_Rapporto_Principale", cod_Rapporto, "Cod_Rapporto");
        //AggiungiRapportoContabilePrincilapele();
    } else {
        var piva = $(Controls.PivaAzienda).val();
        var codContatto = $(Controls.xCodContatto).val();
        var param = kendo.stringify({
            piva: piva,
            codContatto: codContatto
        });
        ajaxAgronica(indirizzohttp + "/CaricaRapportiContabili",
            param,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                for (var x = 0; x < risp.length; x++) {                         //controllo per ogni record individuato se i conti relativi sono ancora presenti
                    if (risp[x]["Cod_Conto"] != 0 && risp[x]["Cod_Conto"] != -1) {
                        if (!elencoContiEconomici.find(function (conto, index) { //controllo sia i conti economici
                            if (conto.Cod_Conto == risp[x]["Cod_Conto"])
                                return true;
                        })) {
                            risp[x]["Cod_Conto"] = -1; //in caso manchino dei conti li imposto a -1 (vuoto)
                            risp[x]["Conto_Descr"] = ""; //e tolgo la descrizione
                            ContoContattoMancante = true;
                        }
                    } else {
                        risp[x]["Cod_Conto"] = -1; //in caso manchino dei conti li imposto a -1 (vuoto)
                        risp[x]["Conto_Descr"] = ""; //e tolgo la descrizione
                    }

                    if (risp[x]["Cod_Conto_Pat"] != 0 && risp[x]["Cod_Conto_Pat"] != -1) {
                        if (!elencoContiPatrimoniali.find(function (conto, index) { //che quelli patrimoniali
                            if (conto.Cod_Conto_Pat == risp[x]["Cod_Conto_Pat"])
                                return true;
                        })) {
                            risp[x]["Cod_Conto_Pat"] = -1;
                            risp[x]["Conto_Descr_Pat"] = "";
                            ContoContattoMancante = true;
                        }
                    } else {
                        risp[x]["Cod_Conto_Pat"] = -1;
                        risp[x]["Conto_Descr_Pat"] = "";
                    }
                }
                options.success(risp);
            }, null);

    }
}

function Contatto_Rubrica(options) {
    options.success(jsRubrica);
}

function Contatto_Indirizzi(options) {
    options.success(jsIndirizziNew);
}

function Contatto_Costi(options) {
    options.success(jsCostiNew);
}

function Contatto_Liquidita(options) {
    options.success(jsLiquidita);
}

function Contatto_Conti(options) {
    options.success(jsConti);
}

function PopolaKendoListRapportiContabiliSelezionati() {
    var codContatto = $(Controls.xCodContatto).val();
    var piva = $(Controls.PivaAzienda).val();
    var param = kendo.stringify({
        piva: piva,
        codContatto: codContatto
    });

    ajaxAgronicaSync(indirizzohttp + "/KendoListRapportiContabiliSelezionati",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            elencoRapportiSelezionati = risp;
        }, null);
    return elencoRapportiSelezionati;
}


function PopolaElencoQualifiche(flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoQualifiche === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/TabellaQualifiche",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Qualifica_Cod": 0,
                        "Qualifica_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoQualifiche = risp;
            }, null);
    }
    return elencoQualifiche;
}

function PopolaElencoMansioni(flagVuoto) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoMansioni === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/TabellaMansioni",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Mansione_Cod": 0,
                        "Mansione_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoMansioni = risp;
            }, null);
    }
    return elencoMansioni;
}

function PopolaElencoCodiciLingue(flagVuoto) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoCodiciLingue === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/Leggi_Codici_Lingue",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Codice": "",
                        "Descrizione": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoCodiciLingue = risp.sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1);
            }, null);
    }
    return elencoCodiciLingue;

}

function PopolaElencoRapportiContabili(flagVuoto) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoRapportiContabili === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/TabellaRapportiContabili",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Cod_RisUm": 0,
                        "Rag_Soc": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoRapportiContabili = risp;
            }, null);
    }
    return elencoRapportiContabili;
}

function PopolaElencoClassRisUm(flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoClassificazioniRisUm === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/TabellaClassificazioneRisUm",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Classificazione_Cod": 0,
                        "Classificazione_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoClassificazioniRisUm = risp;
            }, null);
    }
    return elencoClassificazioniRisUm;
}

function PopolaRappresentantiFiscali(flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    var param = "";

    ajaxAgronicaSync(indirizzohttp + "/CaricaRappresentantiFiscali",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Cod_RisUm": 0,
                    "Rag_Soc": ""
                };
                risp.unshift(objVuoto);
            }
            var ddlClientId = Controls.RappFiscale;
            $(ddlClientId).empty();
            $.each(risp, function (index, item) {
                $(ddlClientId).append('<option value ="' + item.Cod_RisUm + '">' + item.Rag_Soc + '</option>');
            }
            );

        }, null);

}

function PopolaTipoRubrica(flagVuoto) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    var param = "";

    ajaxAgronicaSync(indirizzohttp + "/TipiRubrica",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Cod_RisUm": 0,
                    "Rag_Soc": ""
                };
                risp.unshift(objVuoto);
            }
            elencoTipiRubrica = risp;

        }, null);

    return elencoTipiRubrica;
}

function DropDownRapportiContabiliXCosti() {
    var elenco = null;
    var codContatto = $(Controls.xCodContatto).val();
    var piva = $(Controls.PivaAzienda).val();
    var param = kendo.stringify({
        piva: piva,
        codContatto: codContatto
    });

    ajaxAgronicaSync(indirizzohttp + "/DropDownRapportiContabili",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            dropDownCostiRapporti = risp;
        }, null);

    return dropDownCostiRapporti;

}

function Controlla_Costi(righeInserite, righeModificate) {

    var param = "{righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "'}";
    var risposta = "";
    ajaxAgronicaSync(indirizzohttp + "/Controlla_Costi",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risposta = risp;
        }, null);
    return risposta;

}

function Check_Movimenti_Collegati_Risorsa_Umana(codRisUm) {
    var codContatto = CodiceContatto();
    var sa_cod = $(Controls.Visibilita).val();
    var piva = $(Controls.PivaAzienda).val();

    var param = kendo.stringify(
        {
            piva: piva,
            codContatto: codContatto,
            codRisUm: codRisUm,
            sa_Cod: sa_cod
        });

    var risposta = "";
    ajaxAgronicaSync(indirizzohttp + "/ControllaMovimentiContabili",
        param,
        false,
        function (risp) {
            risposta = "";
        }, function (risp) {
            risposta = risp.Errore;
        });
    return risposta;
}

function Check_Esistenza_SquadreCdG_Risorsa_Umana(codRisUm) {
    var codContatto = CodiceContatto();
    var sa_cod = $(Controls.Visibilita).val();
    var piva = $(Controls.PivaAzienda).val();

    var param = kendo.stringify(
        {
            piva: piva,
            codContatto: codContatto,
            codRisUm: codRisUm
        });

    var risposta = "";
    ajaxAgronicaSync(indirizzohttp + "/ControllaEsistenzaSquadreCdG",
        param,
        false,
        function (risp) {
            risposta = "";
        }, function (risp) {
            risposta = risp.Errore;
        });
    return risposta;
}

function Check_Movimenti_Collegati_Liquidita(codLiquidita) {

    var codContatto = CodiceContatto();
    var piva = $(Controls.PivaAzienda).val();

    var param = kendo.stringify(
        {
            piva: piva,
            codContatto: codContatto,
            codLiquidita: codLiquidita
        });
    var risposta = "";
    ajaxAgronicaSync(indirizzohttp + "/ControllaLiquidita",
        param,
        false,
        function (risp) {
            risposta = "";
        }, function (risp) {
            risposta = risp.Errore;
        });
    return risposta;
}

function AggiornaEffettivo(flagEsci, risultato) {

    var checkOk = false;
    var salvaOk = false;
    var gestisciContabilita = GestisciDatiContabili();

    var grid_rapportiContabili = $("#griglia_rapporti_contabili").data("kendoGrid");
    var grid_Rubrica = $("#griglia_rapporti_contabili").data("kendoGrid");
    var grid_Costi = $("#griglia_costi").data("kendoGrid");
    var grid_Indirizzi = $("#grdIndirizzi").data("kendoGrid");

    var grid_Liquidita = null;
    var grid_Conti = null;
    if (gestisciContabilita == true) {
        grid_Liquidita = $("#griglia_liquidita").data("kendoGrid");
        grid_Conti = $("#griglia_conti").data("kendoGrid");
    }

    var grid_rapportiContabiliSource = grid_rapportiContabili.dataSource;

    var objParametri = new Object();
    objParametri.CodContatto = $(Controls.xCodContatto).val();
    objParametri.TipoOperazione = $(Controls.TipoOperazioneContatto).val();
    objParametri.righeInseriteGrid_Rapporti_Contabili = righeInseriteGrid_Rapporti_Contabili;
    objParametri.righeModificateGrid_Rapporti_Contabili = righeModificateGrid_Rapporti_Contabili;
    objParametri.righeCancellateGrid_Rapporti_Contabili = righeCancellateGrid_Rapporti_Contabili;
    objParametri.righeTotaliGrid_Rapporti_Contabili = grid_rapportiContabiliSource.view().length;
    objParametri.righeNonCancellateGrid_Rapporti_Contabili = righeNonCancellate_Rapporti_Contabili;

    objParametri.righeInseriteGrid_Costi = righeInseriteGrid_Costi;
    objParametri.righeModificateGrid_Costi = righeModificateGrid_Costi;
    objParametri.righeCancellateGrid_Costi = righeCancellateGrid_Costi;
    objParametri.righeNonCancellateGrid_Costi = righeNonCancellate_Costi;

    objParametri.righeInseriteGrid_Rubrica = righeInseriteGrid_Rubrica;
    objParametri.righeModificateGrid_Rubrica = righeModificateGrid_Rubrica;
    objParametri.righeCancellateGrid_Rubrica = righeCancellateGrid_Rubrica;


    objParametri.righeInseriteGrid_Indirizzi = righeInseriteGrid_Indirizzi;
    objParametri.righeModificateGrid_Indirizzi = righeModificateGrid_Indirizzi;
    objParametri.righeCancellateGrid_Indirizzi = righeCancellateGrid_Indirizzi;
    objParametri.righeNonCancellateGrid_Indirizzi = righeNonCancellateGrid_Indirizzi;


    if (gestisciContabilita == true) {
        objParametri.righeInseriteGrid_Liquidita = righeInseriteGrid_Liquidita;
        objParametri.righeModificateGrid_Liquidita = righeModificateGrid_Liquidita;
        objParametri.righeCancellateGrid_Liquidita = righeCancellateGrid_Liquidita;
        objParametri.righeNonCancellateGrid_Liquidita = righeNonCancellate_Liquidita;

        objParametri.righeInseriteGrid_Conti = righeInseriteGrid_Conti;
        objParametri.righeModificateGrid_Conti = righeModificateGrid_Conti;
        objParametri.righeCancellateGrid_Conti = righeCancellateGrid_Conti;
        objParametri.righeNonCancellateGrid_Conti = righeNonCancellate_Conti;
    }

    objParametri.Fittizio = getKendoSwitch("cb_fittizio");
    objParametri.ItaEste = Get_KendoDDLValue("ddl_ItaEste");
    objParametri.CodiceFiscaleEstero = $(Controls.CF_Estero).val();
    objParametri.RagioneSociale = $(Controls.RagioneSociale).val();
    objParametri.Cognome = $(Controls.Cognome).val();
    objParametri.Nome = $(Controls.Nome).val();
    objParametri.DataNascita = $(Controls.DataNascita).val();
    objParametri.PIVA = $(Controls.PIVA).val();
    objParametri.CodiceFiscale = $(Controls.CF).val();
    objParametri.NumeroBadge = $(Controls.Badge).val();
    objParametri.EUDR = getKendoSwitch("cb_eudr");

    objParametri.PEC = $(Controls.Pec).val();
    objParametri.CodiceSDI = $(Controls.CodiceSDI).val();

    var convenevoli = Get_KendoDDLValue("ddlConvenevoli");
    objParametri.Convenevoli = convenevoli;

    var ddlTipoContatto = $(Controls.TipologiaContatto);
    objParametri.TipologiaContatto = ddlTipoContatto.val();

    var ddlRapFisc = Get_KendoDDLValue("Ddl_Rappresentante_Fiscale");
    objParametri.RappFiscale = ddlRapFisc;

    objParametri.DichIntentiProtocollo = $(Controls.DichiarazioneIntentoProtocollo).val();
    objParametri.DichIntentiData = $('input[name$="txt_dich_intenti_data"]').val();

    var optVisibilita = $(Controls.Visibilita);
    objParametri.Visibilita = optVisibilita.val();

    var ddlSesso = $(Controls.Sesso);
    objParametri.Sesso = ddlSesso.val();

    var tipoUtenteOptions = $(Controls.TipoUtente);
    var selected = tipoUtenteOptions.find('input:checked').val();

    var cod_contatto = CodiceContatto();
    var x_piva = $(Controls.PivaAzienda).val();
    var altriDati = new Object();

    altriDati.Note = $("#Txt_Altri_Dati_Note").val();
    altriDati.Note2 = $("#Txt_Altri_Dati_Note2").val();
    altriDati.NoteOperazioni = KendoMultisel("multiselNoteOperazioni").value().join("|");
    altriDati.NoteOperazioni2 = KendoMultisel("multiselNoteOperazioni2").value().join("|");
    altriDati.CodiceAccisa = $(Controls.CodiceAccisa).val();
    altriDati.UfficioDogane = Get_KendoDDLValue("ddl_Cod_Uff_Dogan");
    if (permessoCaloPesoDefault) {
        altriDati.CaloPeso = $("#Txt_Calo_Peso").data("kendoNumericTextBox").value();
        altriDati.CoeffCaloPeso = $("#Txt_Coeff_Calo_Peso").data("kendoNumericTextBox").value();
    }
    //Controlli validità CF e Piva
    if (!objParametri.Fittizio) {

        //Persona Fisica
        if (selected == "0") {
            var cod_contatto = objParametri.CodiceFiscale;
            if (cod_contatto.length === 11) {

                //controllo CF generato automaticamente
                var cod_contatto_split = cod_contatto.split("F")
                if (!(/^\d+$/.test(cod_contatto_split[1]))) {
                    var confirmation = confirm("Sintassi codice fiscale non corretta. Proseguire?");
                    if (!confirmation) {
                        return false;
                    }
                }

            } else if (cod_contatto.length != 16) {
                var confirmation = confirm("Sintassi codice fiscale non corretta. Proseguire?");
                if (!confirmation) {
                    return false;
                }
            }
        }
        //Persona Giuridica
        else if (objParametri.ItaEste != 2) {
            var cod_contatto = objParametri.PIVA;
            if (cod_contatto.length === 11) {

                //controllo contatto generato automaticamente
                var cod_contatto_split = cod_contatto.split("F")

                if (cod_contatto_split[0].length != 11) {
                    if (!/^\d+$/.test(cod_contatto_split[1])) {
                        var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                        if (!confirmation) {
                            return false;
                        }
                    }
                }
                else if (!(/^\d+$/.test(cod_contatto))) {
                    var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                    if (!confirmation) {
                        return false;
                    }
                }

            } else {
                var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                if (!confirmation) {
                    return false;
                }
            }

        }

    }

    //Controlli validità CF e Piva
    if (!objParametri.Fittizio) {

        //Persona Fisica
        if (selected == "0") {
            var cod_contatto = objParametri.CodiceFiscale;
            if (cod_contatto.length === 11) {

                //controllo CF generato automaticamente
                var cod_contatto_split = cod_contatto.split("F")
                if (!(/^\d+$/.test(cod_contatto_split[1]))) {
                    var confirmation = confirm("Sintassi codice fiscale non corretta. Proseguire?");
                    if (!confirmation) {
                        return false;
                    }
                }

            } else if (cod_contatto.length != 16) {
                var confirmation = confirm("Sintassi codice fiscale non corretta. Proseguire?");
                if (!confirmation) {
                    return false;
                }
            }
        }
        //Persona Giuridica
        else if (objParametri.ItaEste != 2) {
            var cod_contatto = objParametri.PIVA;
            if (cod_contatto.length === 11) {

                //controllo contatto generato automaticamente
                var cod_contatto_split = cod_contatto.split("F")

                if (cod_contatto_split[0].length != 11) {
                    if (!/^\d+$/.test(cod_contatto_split[1])) {
                        var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                        if (!confirmation) {
                            return false;
                        }
                    }
                }
                else if (!(/^\d+$/.test(cod_contatto))) {
                    var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                    if (!confirmation) {
                        return false;
                    }
                }

            } else {
                var confirmation = confirm("Sintassi partita Iva non corretta. Proseguire?");
                if (!confirmation) {
                    return false;
                }
            }

        }

    }

    if (cod_contatto == x_piva) {
        altriDati.RifDepositoFiscale = $(Controls.RifDepositoFiscale).val();
        altriDati.CodiceUA = $(Controls.CodiceUA).val();
        altriDati.CodContoGaranzia = $(Controls.CodContoGaranzia).val();
        altriDati.TipoDestinazione = "";
        altriDati.OrigineDestinazione = Get_KendoDDLValue("ddl_Origine_Spedizione");
    }
    else {
        altriDati.RifDepositoFiscale = "";
        altriDati.CodiceUA = "";
        altriDati.CodContoGaranzia = "";
        altriDati.TipoDestinazione = Get_KendoDDLValue("ddl_Cod_Tipo_Destinazione");
        altriDati.OrigineDestinazione = "";
    }
    objParametri.AltriDati = altriDati;

    var dettagliContabilita = new Object();
    dettagliContabilita.Sconto_Cliente = $("#idScontoCliente").data("kendoNumericTextBox").value();
    dettagliContabilita.Sconto_Add1 = $("#idScontoAdd1").data("kendoNumericTextBox").value();
    dettagliContabilita.Sconto_Add2 = $("#idScontoAdd2").data("kendoNumericTextBox").value();
    dettagliContabilita.Sconto_Add3 = $("#idScontoAdd3").data("kendoNumericTextBox").value();

    dettagliContabilita.Iva_Default = Get_KendoDDLValue("ddl_iva_default");

    dettagliContabilita.Conto_Econ = Get_KendoDDLValue("ddl_contoEco_default");
    dettagliContabilita.Conto_Pat = Get_KendoDDLValue("ddl_contoPat_default");

    dettagliContabilita.Agente_Cod = Get_KendoDDLValue("ddl_agente");
    dettagliContabilita.CapoArea_Cod = Get_KendoDDLValue("ddl_capo_area");
    dettagliContabilita.Vettore_Cod = Get_KendoDDLValue("ddl_vettore");
    dettagliContabilita.Referente_Conferimento = Get_KendoDDLValue("ddl_referenteConferimento");

    dettagliContabilita.Fatturazione_Automatica = Get_KendoDDLValue("ddl_fatturazione_automatica");
    dettagliContabilita.Documento_Fatturazione = Get_KendoDDLValue("ddl_documento_fatturazione");
    dettagliContabilita.Mod_Pag_Default = Get_KendoDDLValue("ddl_modalita_pagamento");
    dettagliContabilita.Iban_Default = Get_KendoDDLValue("ddl_iban_default");
    dettagliContabilita.Provvigione_Agente = $("#idProvvigioneAgente").data("kendoNumericTextBox").value();
    dettagliContabilita.Provvigione_Capo_area = $("#idProvvigioneACapoArea").data("kendoNumericTextBox").value();
    dettagliContabilita.Listino_Prezzi_Acq = Get_KendoDDLValue("ddl_listino_prezzi_acq");
    dettagliContabilita.Listino_Prezzi_Ven = Get_KendoDDLValue("ddl_listino_prezzi_ven");
    dettagliContabilita.GestioneVettore_Cod = Get_KendoDDLValue("ddl_gestione_vettore");
    dettagliContabilita.IndirizzoFatturazione_Cod = Get_KendoDDLValue("ddl_indirizzo_fatturazione");

    dettagliContabilita.DestinazioneDiversa_Cod = KendoDDL("ddl_destinazione_diversa").value();
    if (dettagliContabilita.DestinazioneDiversa_Cod == -1) {
        dettagliContabilita.DestinazioneDiversa_Cod = 0;
    }
    dettagliContabilita.IndirizzoDestinazioneDiversa_Cod = 0;
    if (KendoDDL("ddl_ind_destinazione_diversa") !== null && KendoDDL("ddl_ind_destinazione_diversa") !== undefined) {
        dettagliContabilita.IndirizzoDestinazioneDiversa_Cod = Get_KendoDDLValue("ddl_ind_destinazione_diversa");
    }

    dettagliContabilita.TipoIndirizzoDefault = $("input[name$='hf_dettCont_tipo_ind_default']").val();
    objParametri.DettagliContabilita = dettagliContabilita;

    objParametri.OpzioniContatti = $(Controls.OpzioniContatti).val();
    objParametri.GestisciContabilita = gestisciContabilita;
    objParametri.CaloPesoDefault_daContatto = permessoCaloPesoDefault;
    objParametri.ImpresaGias = $(Controls.ImpresaGias).val();
    objParametri.TipoUtente = selected;
    objParametri.Memo = $("#Txt_Memo").val();
    objParametri.Nome_Breve_Azienda = "";
    objParametri.Nome_Breve_Persona = "";
    if (objParametri.TipoUtente === "0") {
        objParametri.Nome_Breve_Persona = $("#Txt_Nome_Breve_Persona").val();
    }
    else {
        objParametri.Nome_Breve_Azienda = $("#Txt_Nome_Breve_Azienda").val();
    }

    var errori = "";
    var paramEscaped = kendoEscapeOggetto(objParametri);
    var pivaAzienda = $(Controls.PivaAzienda).val();

    var param = "{piva:'" + pivaAzienda + "', param: '" + paramEscaped + "'}";

    // Eseguo Check Preliminari
    ajaxAgronicaSync(indirizzohttp + "/CheckPreSalva",
        param, false,
        function (risposta) {
            checkOk = true;
        },
        function (risposta) {
            if (typeof risposta.RispostaStringa !== 'undefined') {
                errori = risposta.RispostaStringa;
            } else {
                errori = risposta;
            }            
        });

    if (!checkOk) {
        erroreSubmitGriglia(grid_rapportiContabili);
        erroreSubmitGriglia(grid_Rubrica);
        erroreSubmitGriglia(grid_Costi);
        erroreSubmitGriglia(grid_Indirizzi);

        if (gestisciContabilita == true) {
            erroreSubmitGriglia(grid_Liquidita);
            erroreSubmitGriglia(grid_Conti);
        }
        kendo.alert(errori);
        return false;
    }

    errori = "";
    ajaxAgronicaSync(indirizzohttp + "/SalvaTutto",
        param, false,
        function (risposta) {
            salvaOk = true;
            risultato.codContatto = risposta.RispostaStringa;
        },
        function (risposta) {
            errori = risposta.RispostaStringa;
            risultato.codContatto = -1;
        });

    if (!salvaOk) {
        erroreSubmitGriglia(grid_rapportiContabili);
        erroreSubmitGriglia(grid_Rubrica);
        erroreSubmitGriglia(grid_Costi);
        erroreSubmitGriglia(grid_Indirizzi);

        if (gestisciContabilita == true) {
            erroreSubmitGriglia(grid_Liquidita);
            erroreSubmitGriglia(grid_Conti);
        }
        //MessaggioErrore_Bootstrap(errori, "DIV_Messaggi");
        kendo.alert(errori);
        return false;
    } else {
        //MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
        return true;
    }

}

function AggiornaEffettivoIndirizziTipo(esci, options) {
    var checkOk = false;
    var salvaOk = false;
    var errori = "";

    var grid = $("#griglia_indirizzi_tipo").data("kendoGrid");

    var objParametri = new Object();
    objParametri.righeInserite = righeInseriteGrid_IndirizzoTipo;
    objParametri.righeModificate = righeModificateGrid_IndirizzoTipo;
    objParametri.righeCancellate = righeCancellateGrid_IndirizzoTipo;
    objParametri.righeNonCancellate = righeNonCancellate_IndirizzoTipo;

    var piva = $(Controls.PivaAzienda).val();
    var paramEscaped = kendoEscapeOggetto(objParametri);

    var param = "{piva:'" + piva + "', param: '" + paramEscaped + "'}";

    ajaxAgronicaSync(indirizzohttp + "/SalvaIndirizziTipo",
        param, false,
        function (risposta) {
            salvaOk = true;
        },
        function (risposta) {
            errori = risposta.Errore;
        });

    if (!salvaOk) {
        erroreSubmitGriglia(grid);
        kendo.alert(errori);
        MessaggioErrore_Bootstrap(errori, "DIV_Messaggi");
        return false;
    }
    else {
        if (esci) {
            let dialog = $("#nuovoTipoIndirizzoWindow").data("kendoWindow");
            dialog.close();
        }

        // ricarico la ddl con i tipi di indirizzo
        var tipoContatto = $(Controls.TipoUtente).find('input:checked').val();
        var cod_contatto = $(Controls.xCodContatto).val();
        //CaricaTipologieIndfcaricatipoloirizzi(tipoContatto, cod_contatto);
        CaricaTipologieIndirizziKendo(tipoContatto, cod_contatto);

        if (salvaOk) {
            var gridIndirizzi = $("#grdIndirizzi").data("kendoGrid");

            let colGrdIndirizzi = $.map(gridIndirizzi.dataSource.data(), function (v) {
                return { id: v.Cod_Indirizzo, tipoIndirizzo: v.Tipo_Indirizzo };
            });

            let colGrdTipo = $.map(grid.dataSource.data(), function (v) {
                return v.InidrizzoTipoCod;
            });

            let grdIndirizziData = gridIndirizzi.dataSource.data()
            let grdIndirizziTipoData = grid.dataSource.data()

            let intersection = colGrdIndirizzi.filter(x => !colGrdTipo.includes(x.tipoIndirizzo));

            for (i = 0; i < intersection.length; i++) {
                for (j = 0; j < grdIndirizziData.length; j++) {
                    if (grdIndirizziData[j].Tipo_Indirizzo == intersection[i].tipoIndirizzo) {
                        grdIndirizziData[j].Tipo_Indirizzo = 0;
                        grdIndirizziData[j].Tipo_Indirizzo_Desc = "";
                    }

                }
            }
            for (i = 0; i < grdIndirizziTipoData.length; i++) {
                for (j = 0; j < grdIndirizziData.length; j++) {
                    if (grdIndirizziData[j].Tipo_Indirizzo == grdIndirizziTipoData[i].InidrizzoTipoCod && parseInt($(Controls.TipoUtente).find('input:checked').val()) != grdIndirizziTipoData[i].ApplicabilitaCod) {
                        grdIndirizziData[j].Tipo_Indirizzo = 0;
                        grdIndirizziData[j].Tipo_Indirizzo_Desc = "";
                    }
                }
            }
            gridIndirizzi.refresh();
        }

        MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "SalvataggioTipiIndirizziEffettuatoCorrettamente", "Salvataggio Tipi Indirizzo effettuato correttamente"), "DIV_Messaggi");
        return true;
    }
}

function erroreSubmitGriglia(grid) {
    var dsSort = [];
    if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
        // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
        // che non si vedono più
        ripristinaRigheCancellateKendoGrid(grid, dsSort);
    }
}


function AggiungiRapportoContabilePrincilapele() {

    var ddl = KendoDDL("ddl_Rapporto_Principale");
    var codContatto = $(Controls.xCodContatto).val();
    var codRapporto = ddl.value();
    var desRapporto = ddl.text();
    var piva = $(Controls.PivaAzienda).val();

    var gridDataSource = "";

    var param = kendo.stringify({
        piva: piva,
        codRapporto: codRapporto,
        desRapporto: desRapporto,
        codContatto: codContatto
    });
    if (codRapporto === "0")
        return;

    // Eseguo Check Preliminari
    ajaxAgronicaSync(indirizzohttp + "/AggiungiRapportoContabilePrincilapele",
        param, false,
        function (risp) {

            gridDataSource = JSON.parse(risp.RispostaStringa);
            gridDataSource.Validita_Inizio = new Date(gridDataSource.Validita_Inizio);
            gridDataSource.Validita_Fine = new Date(gridDataSource.Validita_Fine);

            var grid = $("#griglia_rapporti_contabili").data("kendoGrid");

            grid.dataSource.data([]);
            grid.dataSource.insert(0, gridDataSource);
            grid.dataSource.sync();
            grid.refresh();

        },
        function (risp) {
            errori = risp.RispostaStringa;
        });

}

function get_Convenevoli(options) {

    var convenevoli = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        convenevoli = $(Controls.ConvenevoleSalvato).val();
    }

    var tipo = $(Controls.TipoUtente).find('input:checked').val();
    var parametri = kendo.stringify({
        tipoContatto: tipo,
        convenevoli: convenevoli
    });

    ajaxAgronica(indirizzohttp + "/GetConvenevoli",
        parametri,
        function (risposta) {
            let convenevoli = JSON.parse(risposta.RispostaStringa);
            options.success(convenevoli);

            var tipoCarica = $(Controls.TipoOperazioneContatto).val();
            if (tipoCarica != 1) {
                var widgetId = "ddlConvenevoli";
                var widget = KendoDDL(widgetId);
                var dataSource = widget.dataSource;

                var value = $(Controls.ConvenevoleSalvato).val();
                widget.select(function (dataItem) {
                    return dataItem.Text === value;
                });
                widget.close();
            }


        }, null, null, false);

}

function Leggi_OriginiSpedizione(options) {
    var originiSped = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var parametri = kendo.stringify({});

    ajaxAgronica(indirizzohttp + "/Leggi_Origini_Spedizione",
        parametri,
        function (risposta) {
            let originiSped = JSON.parse(risposta.RispostaStringa);
            options.success(originiSped);

            if (tipoCarica != 1) {
                var value = $(Controls.OrigineDestinazione).val();

                var widgetId = "ddl_Origine_Spedizione";
                var widget = KendoDDL(widgetId);
                var dataSource = widget.dataSource;
                widget.select(function (dataItem) {
                    return dataItem.Codice == value;
                });
                widget.close();

            }

        }, null, null, false);


}

function Leggi_Tipologie_Destinazione(options) {

    var tipologie = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var parametri = kendo.stringify({});

    ajaxAgronica(indirizzohttp + "/Leggi_Tipologie_Destinazione",
        parametri,
        function (risposta) {
            let tipologie = JSON.parse(risposta.RispostaStringa);
            options.success(tipologie);

            if (tipoCarica != 1) {
                var value = $(Controls.TipoDestinazione).val();

                var widgetId = "ddl_Cod_Tipo_Destinazione";
                var widget = KendoDDL(widgetId);
                var dataSource = widget.dataSource;
                widget.select(function (dataItem) {
                    return dataItem.Codice == value;
                });
                widget.close();

            }

        }, null, null, false);

}

function Leggi_Agenti(options) {

    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.Agenti;
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, "", 0, "", false);
    options.success(resp);

    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Agente_Cod).val();
        SelezionaKendoDropDownItem("ddl_agente", value, "Cod_RisUm");
    }

}

function Leggi_referenteConferimento(options) {

    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.Referente_Conferimento;
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, "", 0, "", false);
    options.success(resp);

    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_referenteConferimento_cod).val();
        SelezionaKendoDropDownItem("ddl_referenteConferimento", value, "Cod_RisUm");
    }

}

function Leggi_ItaEste(options) {
    options.success(elencoItaEste);
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.IDCF).val();
        if (value != 2) { value = 0; }
        SelezionaKendoDropDownItem("ddl_ItaEste", value, "Codice");
    }

}

function Carica_Lista_Rapporti_Contabili() {
    var rappContabili = "";
    var parametri = kendo.stringify({
        tipoContatto: $(Controls.TipoUtente).find('input:checked').val()
    });

    ajaxAgronicaSync(indirizzohttp + "/DropDownRapportoContabilePrincipale2",
        parametri,
        false,
        function (risposta) {
            rappContabili = JSON.parse(risposta.RispostaStringa);
        }, null);

    var ddl = KendoDDL("ddl_Rapporto_Principale");
    if (rappContabili != "" && rappContabili != undefined) {
        ddl.dataSource.data(rappContabili);
    } else {
        ddl.dataSource.data({});
    }

}

function Leggi_Stati(options) {
    var parametri = kendo.stringify({});
    ajaxAgronicaSync(indirizzohttp + "/CaricaStati",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp.sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1));
        }, null);
    SelezionaKendoDropDownItem("ddl_Stato", "IT", "Codice");

}

function Leggi_Statikendo() {

    var parametri = kendo.stringify({});
    ajaxAgronicaSync(indirizzohttp + "/CaricaStati",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            dropDownStati = risp.sort((a, b) => (a.Stato_Des > b.Stato_Des) ? 1 : -1);
        }, null);

}


function Leggi_Lingue(options) {
    var parametri = kendo.stringify({});
    ajaxAgronicaSync(indirizzohttp + "/CaricaLingue",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
    SelezionaKendoDropDownItem("ddl_Lingua", "IT", "Codice");

}

function Leggi_LinguaKendo() {
    var parametri = kendo.stringify({});
    ajaxAgronicaSync(indirizzohttp + "/CaricaLingue",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa)
            $.each(risp, function (i, e) {
                if (risp[i].Lingua_Des == "")
                    e.ORDINE = 0;
                else {
                    e.ORDINE = 1;
                }
            });
            risp.push({ ORDINE: 0, Codice_Lingua: "000", Lingua_Des: "" })
            dropDownLingua = risp.sort((a, b) => (a.Lingua_Des > b.Lingua_Des) ? 1 : -1);
        }, null);
}


function Leggi_ProvinceKendo(stato) {
    if (stato === "" || stato == null || stato == undefined) {
        stato = "IT";
    }
    var parametri = kendo.stringify({ stato });
    ajaxAgronicaSync(indirizzohttp + "/CaricaProvinceKendo",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);

            $.each(risp, function (i, e) {
                if (risp[i].Provincia_des == "Non Definita")
                    e.ORDINE = 0;
                else {
                    e.ORDINE = 1;
                }
            });
            dropDownProvince = risp.sort((a, b) => (a.ORDINE < b.ORDINE) ? -1 : ((a.Provincia_des > b.Provincia_des) ? 1 : -1));

        }, null);
}


function Leggi_ComuniKendo(provincia, comuni_prov) {

    if (provincia != "" && provincia != null && provincia != undefined) {
        var parametri = kendo.stringify({ provincia, comuni_prov });
        ajaxAgronicaSync(indirizzohttp + "/CaricaComuniKendo",
            parametri,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                $.each(risp, function (i, e) {
                    if (risp[i].Comune_des == "")
                        e.ORDINE = 0;
                    else {
                        e.ORDINE = 1;
                    }
                });
                risp.push({ ORDINE: 0, Comune_cod: "000", Comune_des: "" })
                dropDownComuni = risp.sort((a, b) => (a.Comune_des > b.Comune_des) ? 1 : -1);
            }, null);
    } //else alert("Selezionare una Provincia");
}



function Leggi_Cliente_Fornitore(cod_RisUm) {
    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.ClientiFornitori;
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var testoRicerca = "";

    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, testoRicerca, cod_RisUm, "", false);
    if (resp !== null && resp !== undefined) {
        var dataSource = [];
        var x = new Object();
        x.Cod_RisUm = resp[0].Cod_RisUm;
        x.Rag_Soc_Completa = resp[0].Rag_Soc_Completa;
        x.Cod_Contatto = resp[0].Cod_Contatto;
        dataSource.push(x);

        var dropDown = KendoDDL("ddl_destinazione_diversa");
        dropDown.dataSource.data(dataSource);
        Inizializza_Combo_Indirizzo_Destinazione_Diversa(resp[0].Cod_Contatto, resp[0].Id_CF);

        var selezionato = $(Controls.DettCont_Tipo_Indirizzo_Default_Destinazione_Diversa).val();
        if (selezionato != 0 && selezionato !== null && selezionato !== undefined) {
            SelezionaKendoDropDownItem("ddl_ind_destinazione_diversa", selezionato, "IndirizzoTipo_Cod");
        }

    }
    return resp;
}

function Leggi_Clienti_Fornitori_Filtered(options) {
    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.ClientiFornitori;
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var testoRicerca = JSON.stringify(options.data.filter.filters);

    if (elencoContatti[tipoRapporto] !== null && elencoContatti[tipoRapporto] !== undefined) {
        elencoContatti[tipoRapporto] = null;
    }

    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, testoRicerca, 0, "", false);
    options.success(resp);

}

function Leggi_Tipi_Indirizzi(options) {
    var cod_Contatto = this.data.Cod_Contatto;
    var id_CF = this.data.Id_CF;
    var piva = $(Controls.PivaAzienda).val();

    var parametri = kendo.stringify({
        Piva: piva,
        Cod_Contatto: cod_Contatto,
        Id_CF: id_CF,
        ContattoEstero: ContattoEstero()
    });
    ajaxAgronicaSync(indirizzohttp + "/CaricaIndirizziPersonalizzati",
        parametri,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Tipo_Indirizzo_Default).val();
        SelezionaKendoDropDownItem("ddl_indirizzo_fatturazione", value, "IndirizzoTipo_Cod");
    }

}

function Leggi_Capi_Area(options) {
    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.CapoArea;
    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, "", 0, "", false);
    options.success(resp);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_CapoArea_Cod).val();
        SelezionaKendoDropDownItem("ddl_capo_area", value, "Cod_RisUm");
    }
}

function Leggi_Terzisti(options) {
    var piva = $(Controls.PivaAzienda).val();
    var accettazioneConGerarchia = 0;
    var pivaPadreGerarchia = "";
    var tipoRapporto = enum_TipoRapporto.Vettori;

    let resp = RicercaContattiDocumentoSync(false, piva, tipoRapporto, true, true, true, accettazioneConGerarchia, pivaPadreGerarchia, "", 0, "", false);
    options.success(resp);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Vettore_Cod).val();
        SelezionaKendoDropDownItem("ddl_vettore", value, "Cod_RisUm");
    }
}

function Leggi_Uffici_Dogane(options) {

    var uffici = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var parametri = kendo.stringify({});

    ajaxAgronica(indirizzohttp + "/Leggi_Uffici_Dogane",
        parametri,
        function (risposta) {
            let uffici = JSON.parse(risposta.RispostaStringa);
            options.success(uffici.sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1));

            if (tipoCarica != 1) {
                var value = $(Controls.UfficioDogane).val();

                var widgetId = "ddl_Cod_Uff_Dogan";
                var widget = KendoDDL(widgetId);
                var dataSource = widget.dataSource;
                widget.select(function (dataItem) {
                    return dataItem.Codice == value;
                });
                widget.close();

            }

        }, null, null, false);
}

function Leggi_Gestione_Vettore(options) {
    var risp = RicercaGestioneVettoreSync();
    options.success(risp);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_GestioneVettore_Cod).val();
        SelezionaKendoDropDownItem("ddl_gestione_vettore", value, "GestioneVettore_Cod");
    }

}


function Leggi_Liquidita(options) {
    var liquidita = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var parametri = kendo.stringify({});

    ajaxAgronica(indirizzohttp + "/Leggi_Liquidita",
        parametri,
        function (risposta) {
            let liquidita = JSON.parse(risposta.RispostaStringa).sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1);
            options.success(liquidita);

            var tipoCarica = $(Controls.TipoOperazioneContatto).val();
            var selezionato = null;
            if (tipoCarica != 1) {
                selezionato = $(Controls.DettCont_Iban_Default).val();
            } else {
                selezionato = 0;
            }

            SelezionaKendoDropDownItem("ddl_iban_default", selezionato, "Codice");

        }, null, null, false);
}

function Leggi_Aliqote_Iva(options) {
    let elencoIVA = RicercaIVA_Aliquote("");
    options.success(elencoIVA);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Iva_Default).val();
        SelezionaKendoDropDownItem("ddl_iva_default", value, "Cod_IVA");
    }
}

function Leggi_Conti_Economici(options) {
    elencoContiEconomici = RicercaContoEconomico(true, $(Controls.PivaAzienda).val(), new Date().getFullYear(), "", null);
    options.success(elencoContiEconomici);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Conto_Economico_Default).val();
        if (value != 0 && value != -1) {
            if (elencoContiEconomici.find(
                function (conto, index) { //controllo se il conto economico di default è presente nella lista dei conti
                    if (conto.Cod_Conto == value)
                        return true;
                })) {
                CodContoEcon = value; //se lo trovo bene

            } else {
                CodContoEcon = -1; //altrimenti lo imposto a -1 (vuoto)
                ContoEconMancante = true;
            }
        } else {
            CodContoEcon = -1; //altrimenti lo imposto a -1 (vuoto)
        }
    }
}

function Leggi_Conti_Patrimoniali(options) {
    elencoContiPatrimoniali = RicercaContoPatrimoniale(true, $(Controls.PivaAzienda).val(), new Date().getFullYear(), "", null);
    options.success(elencoContiPatrimoniali);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Conto_Patrimoniale_Default).val();
        if (value != 0 && value != -1) {
            if (elencoContiPatrimoniali.find(
                function (conto, index) { //come sopra
                    if (conto.Cod_Conto_Pat == value)
                        return true;
                })) {
                CodContoPat = value;
            } else {
                CodContoPat = -1;
                ContoPatMancante = true;
            }
        } else {
            CodContoPat = -1; //altrimenti lo imposto a -1 (vuoto)
        }
    }
}

function Leggi_ModalitaPagamento(options) {
    var x_piva = $(Controls.PivaAzienda).val();
    options.success(RicercaPagamenti_Causali(false, x_piva, 0).sort((a, b) => (a.Cau_Pagamento_Sigla > b.Cau_Pagamento_Sigla) ? 1 : -1));
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {
        var value = $(Controls.DettCont_Mod_Pag_Default).val();
        SelezionaKendoDropDownItem("ddl_modalita_pagamento", value, "Cau_Pagamento");
    }
}

function Leggi_DocumentiFatturazione(options) {

    options.success(elencoDocumentiFatturazione.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    var selezionato = null;
    if (tipoCarica != 1) {
        selezionato = $(Controls.DettCont_Documento_Fatturazione).val();
    } else {
        selezionato = 0;
    }
    SelezionaKendoDropDownItem("ddl_documento_fatturazione", selezionato, "LAV_COD");
}

function Leggi_Listini(options) {

    var piva = $(Controls.PivaAzienda).val();
    var xOrderBY = " lp.Piva, lcp.Listino_Classe_Des, lp.Listino_Des";
    var dropDownId = "";
    var value = -1;

    let listini = RicercaListini(piva, -1, -1, this.data.tipo_Classe, -1, -1, -1, xOrderBY);
    options.success(listini);

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    if (tipoCarica != 1) {

        if (this.data.tipo_Classe === 2) {
            dropDownId = "ddl_listino_prezzi_ven";
            value = $(Controls.DettCont_Listino_Prezzi_Ven).val();
        } else {
            dropDownId = "ddl_listino_prezzi_acq";
            value = $(Controls.DettCont_Listino_Prezzi_Acq).val();
        }
        SelezionaKendoDropDownItem(dropDownId, value, "Listino_Cod");
    }
}

function Leggi_FatturazioneAutomatica(options) {

    options.success(elencoModalitaFatturazione.sort((a, b) => (a.Descrizione > b.Descrizione) ? 1 : -1));
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    var selezionato = null;
    if (tipoCarica != 1) {
        selezionato = $(Controls.DettCont_Fatturazione_Automatica).val();
    } else {
        selezionato = 1;
    }
    SelezionaKendoDropDownItem("ddl_fatturazione_automatica", selezionato, "Codice");
}

function get_RappresentantiFiscali(options) {

    var rappFiscali = "";
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var parametri = kendo.stringify({});


    ajaxAgronica(indirizzohttp + "/CaricaRappresentantiFiscali",
        parametri,
        function (risposta) {
            let rappFiscali = JSON.parse(risposta.RispostaStringa);
            options.success(rappFiscali);

            var tipoCarica = $(Controls.TipoOperazioneContatto).val();
            if (tipoCarica != 1) {
                var value = $(Controls.RappFiscaleSalvato).val();

                var widgetId = "Ddl_Rappresentante_Fiscale";
                var widget = KendoDDL(widgetId);
                var dataSource = widget.dataSource;
                widget.select(function (dataItem) {
                    return dataItem.Cod_RisUm == value;
                });
                widget.close();

            }

        }, null, null, false);

}


function RiempiNoteOperazioni(options) {
    options.success(elencoOperazioniXNote.sort((a, b) => (a.LAV_DES > b.LAV_DES) ? 1 : -1));
}

function SelezionaKendoDropDownItem(widgetId, value, campoRicerca) {
    var widget = KendoDDL(widgetId);
    if (widget != undefined) {
        var dataSource = widget.dataSource;
        widget.select(function (dataItem) {
            switch (campoRicerca) {
                case "Cod_RisUm":
                    return dataItem.Cod_RisUm == value;
                case "GestioneVettore_Cod":
                    return dataItem.GestioneVettore_Cod == value;
                case "Cod_IVA":
                    return dataItem.Cod_IVA == value;
                case "Cod_Conto_Pat":
                    return dataItem.Cod_Conto_Pat == value;
                case "Cod_Conto":
                    return dataItem.Cod_Conto == value;
                case "Codice":
                    return dataItem.Codice == value;
                case "LAV_COD":
                    return dataItem.LAV_COD == value;
                case "Cau_Pagamento":
                    return dataItem.Cau_Pagamento == value;
                case "Listino_Cod":
                    return dataItem.Listino_Cod == value;
                case "IndirizzoTipo_Cod":
                    return dataItem.IndirizzoTipo_Cod == value;
                case "Cod_Rapporto":
                    return dataItem.Cod_Rapporto == value;
            }

        });
        widget.close();
    }
}

function Popola_Istituti_Credito(flagVuoto) {
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoIstitutiCredito === null) {
        var param = "";
        ajaxAgronicaSync(indirizzohttp + "/Leggi_Istituti_Credito",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Istituto_Cod": 0,
                        "Istituto_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoIstitutiCredito = risp;
            }, null);
    }
    return elencoIstitutiCredito;
}

function LeggiOpzioniContab() {
    let param = kendo.stringify({ piva: $(Controls.PivaAzienda).val(), objP_server: objP_server, objP_utenti: objP_utenti });

    ajaxAgronicaSync(url + pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/LeggiOpzioni_DocContabili",
        param,
        false,
        function (risposta) {
            $(OpzioniContatti).val(risposta.RispostaStringa);
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

function Popola_Piano_Conti(flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoConti === null) {

        var piva = $(Controls.PivaAzienda).val();
        var param = kendo.stringify({
            piva: piva
        });

        ajaxAgronica(indirizzohttp + "/Leggi_Piano_Conti",
            param,
            function (risposta) {
                elencoConti = JSON.parse(risposta.RispostaStringa);
            }, null);

    }
    return elencoConti;
}


function ApriLinkPostit(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    var Cod_RisUm = dataItem.Cod_RisUm;
    var piva = $(Controls.PivaAzienda).val();

    var param = kendo.stringify({
        piva: piva,
        Cod_RisUm: Cod_RisUm
    });
    var redirectUrl = "";

    ajaxAgronica(indirizzohttp + "/Ottieni_Link_PosiIt",
        param,
        function (risposta) {
            redirectUrl = risposta.RispostaStringa;
            apriGestionePostIt(redirectUrl);
        }, null);

}

function apri_associazione_listini(tipoListino) {

    var cod_listino = null;

    var piva = $(Controls.PivaAzienda).val();
    var saCod = $(Controls.Visibilita).val();
    var cod_contatto = CodiceContatto();
    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    switch (tipoListino) {
        case enum_tipoListino.Acquisto:
            if (KendoDDL("ddl_listino_prezzi_acq") !== null && KendoDDL("ddl_listino_prezzi_acq") !== undefined) {
                listinoCod = KendoDDL("ddl_listino_prezzi_acq").value();
            }
            break;
        case enum_tipoListino.Vendita:
            if (KendoDDL("ddl_listino_prezzi_ven") !== null && KendoDDL("ddl_listino_prezzi_ven") !== undefined) {
                listinoCod = KendoDDL("ddl_listino_prezzi_ven").value();
            }
            break;
    }

    // TODO Listino null????

    if (tipoCarica == 1) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(contattoEditResxArray, "Attenzione", "Attenzione"),
            messages: {
                okText: TraduzioneMultiResx(contattoEditResxArray, "Si", "Sì"),
                cancel: TraduzioneMultiResx(contattoEditResxArray, "No", "No")
            },
            content: TraduzioneMultiResx(contattoEditResxArray, "OccorreSalvareIlContattoPerPoterAssociareListini", "E' necessario salvare il contatto prima di potere associare i listini. Proseguire? ")
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {

            var flag = controlla_form();
            if (flag) {

                let risultato = { codContatto: -1 };
                if (AggiornaDati(true, risultato)) {
                    ChiamaPaginaAssociazioneListini(piva, cod_contatto, saCod, cod_listino, tipoListino);
                }
            }
            else {
                pretty_alert("Attenzione!", TraduzioneMultiResx(contattoEditResxArray, "InserireIDatiObbligatori", "Inserire i dati obbligatori"));
            }
        });

        kendoConfirm.result.fail(function () {
        });

        kendoConfirm.open();
    }
    else {
        ChiamaPaginaAssociazioneListini(piva, cod_contatto, saCod, cod_listino, tipoListino);
    }

}

function apri_gestione_documenti() {
    var piva = $(Controls.PivaAzienda).val();
    var cod_contatto = CodiceContatto();

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();

    if (tipoCarica == 1) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(contattoEditResxArray, "Attenzione", "Attenzione"),
            messages: {
                okText: TraduzioneMultiResx(contattoEditResxArray, "Si", "Sì"),
                cancel: TraduzioneMultiResx(contattoEditResxArray, "No", "No")
            },
            content: TraduzioneMultiResx(contattoEditResxArray, "OccorreSalvareIlContattoPerPoterInserirePatentiniEDocumenti", "E' necessario salvare il contatto prima di potere inserire i dati dei patentini e/o altri documenti. Proseguire? ")
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {

            var flag = controlla_form();
            if (flag) {

                let risultato = { codContatto: -1 };
                if (AggiornaDati(true, risultato)) {

                    var paginaLink = "";
                    var param = kendo.stringify({
                        piva: piva,
                        cod_contatto: risultato.codContatto
                    });

                    ajaxAgronica(indirizzohttp + "/Ottieni_Link_Modifica_Contatto",
                        param,
                        function (risposta) {
                            var apertoDaPopup = $(Controls.AperturaDaPopup).val();
                            if (apertoDaPopup === "True") {
                                gestisciValore(risultato.codContatto);
                            }
                            redirectUrl = risposta.RispostaStringa;
                            window.location.href = redirectUrl;
                            ChiamaPaginaGestioneDocumenti(piva, risultato.codContatto);

                        }, null);
                }
            }
            else {
                kendo.alert(TraduzioneMultiResx(contattoEditResxArray, "InserireIDatiObbligatori", "Inserire i dati obbligatori"));
            }
        });

        kendoConfirm.result.fail(function () {
        });

        kendoConfirm.open();
    }
    else {
        ChiamaPaginaGestioneDocumenti(piva, cod_contatto);
    }

}

function ChiamaPaginaAssociazioneListini(piva, cod_contatto, saCod, cod_listino, tipoListino) {

    $('#ucAssociazioneListini').show();

    var window = $('#ucAssociazioneListini').data("kendoWindow");
    if (window === null || window == undefined) {

        // inizializzo lo uc di associazione contatto con listini
        initFiltriAssocListini(piva, tipoListino, true, null, cod_contatto, saCod);

        $('#ucAssociazioneListini').kendoWindow({
            title: "Associazione Contatti Listini",
            modal: true,
            resizable: false,
            iframe: false,
            width: "90%",
            height: "85%",
            //content: url,
            close: function () {
                setTimeout(function () {
                    $('#ucAssociazioneListini').hide();
                }, 200);
            }
        }).data('kendoWindow').center();
    }
    else {
        setFiltriAssocListini(tipoListino, true, null, cod_contatto, saCod);
        window.open();
        $("#panelbarAssociazioniUC").data("kendoPanelBar").collapse("#panelItemGrigliaAssociazioniListini");
    }

}

function ChiamaPaginaGestioneDocumenti(piva, cod_contatto) {
    $.ajax({
        type: "POST",
        url: indirizzohttp + "/GestioneAllegati",
        data: "{Piva:'" + piva + "', Cod_Contatto:'" + cod_contatto + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            apriGestioneAllegati(msg.d);
        }
    });

}

function SvuotaIndirizziKendo() {
    let grid = $("#grdIndirizzi").data("kendoGrid");
    if (grid !== null && grid !== undefined)
        grid.dataSource.data([]);
}


function AggiornaSelectVisibilita(tipoPersona) {
    var piva = $(Controls.PivaAzienda).val();
    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/Aggiorna_Visibilita',
        data: "{tipo_persona:" + tipoPersona + ", piva:'" + piva + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            var sa_cod = $(Controls.Visibilita).val();
            $(Controls.Visibilita).empty();
            $(Controls.Visibilita).append(r.d);
            $('.selectpicker').selectpicker('refresh');
            $(Controls.Visibilita).val(sa_cod);
        }
    });
}

function CaricaTipologieIndirizzi(tipoPersona, cod_contatto) {
    var piva = $(Controls.PivaAzienda).val();
    var param = kendo.stringify(
        {
            piva: piva,
            tipo_persona: tipoPersona,
            codContatto: cod_contatto,
            itaEste: ItalianoEstero()
        });

    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/CaricaTipologiaIndirizzo',
        data: param,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            $(Controls.TipoIndirizzo).empty();
            $(Controls.TipoIndirizzo).append(r.d);
            $('.selectpicker').selectpicker('refresh');

        }
    });
}

function CaricaTipologieIndirizziKendo(tipoPersona, cod_contatto) {
    var piva = $(Controls.PivaAzienda).val();
    var param = kendo.stringify(
        {
            piva: piva,
            tipo_persona: tipoPersona,
            codContatto: cod_contatto,
            itaEste: ItalianoEstero()
        });

    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/CaricaTipologiaIndirizzoKendo',
        data: param,
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            dropDownTipologiaIndirizzo = JSON.parse(r.d.RispostaStringa);
        }
    });
}

function Carica_Comuni(provincia, q) {
    $.ajax({
        type: 'POST',
        url: 'New_Contatto_Edit.aspx/Carica_Comuni',
        data: "{provincia:'" + provincia + "'}",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: false,
        success: function (r) {

            $(Controls.Comune).empty();
            $(Controls.Comune).parent().find('button').removeClass('disabled');
            $(Controls.Comune).append(r.d[0]);

            $(Controls.Comune).selectpicker('refresh');

            var comune = q.toLowerCase();
            $(Controls.Comune + ' option').each(function () {
                if (comune == $(this).text()) {
                    $(this).attr('selected', 'selected');
                }

                //i = i + 1;
            });

            $(Controls.Comune).selectpicker('refresh');

        }
    });

}

function LeggiIndirizziTipo(options) {

    var piva = $(Controls.PivaAzienda).val();

    var param = kendo.stringify(
        { piva: piva }
    );
    ajaxAgronica(indirizzohttp + "/CaricaIndirizziTipo",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        },
        function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });

}

function leggiContattiXIndirizziTipo(options) {

    var risultatoVuoto = [];
    var piva = $(Controls.PivaAzienda).val();
    var elencoContatti = "";
    var testo = JSON.stringify(options.data.filter.filters);
    if (testo === "[]") {
        var contattoVuoto = new Object();
        contattoVuoto.Cod_Contatto = "";
        contattoVuoto.Contatto_Des = "";
        risultatoVuoto.push(contattoVuoto);

        options.success(risultatoVuoto);
        return;
    }

    var url = GetUrlLetturaTabelleGestionali() + indirizzohttp_Ricerca_Documenti + "/ListaContatti";

    var param = kendo.stringify(
        {
            objP_server: objP_server,
            Piva: piva,
            Cod_Contatto: '',
            Cod_RisUm: 0,
            FlagPubblico: true,
            ID_CF: id_cf_tipi_indirizzo_filtering,
            Cod_RisUm_Origine: 0,
            Piva_SuperUser_Origine: '',
            Progressivo: '',
            testoRicerca: testo,
            xOrderBy: ' Contatti.Rag_Soc ASC, Contatti.Cognome ASC, Contatti.Nome ASC '
        });

    ajaxAgronica(url, param,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            var contatti = risp.map(function (val) {
                return {
                    Contatto_Des: val.nome,
                    Cod_Contatto: val.cod_contatto,
                    Id_cf: val.id_cf
                };
            });

            //var contattiFiltered = contatti.filter(function (el) {
            //    return el.Id_cf == id_cf_tipi_indirizzo_filtering;
            //});


            options.success(contatti);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(contattoEditResxArray, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

function ControllaContattoRiferimenti() {
    var retVal = true;
    var cod_contatto = CodiceContatto();
    var x_piva = $(Controls.PivaAzienda).val();

    var param = JSON.stringify(
        {
            piva: x_piva,
            codContatto: cod_contatto,
            saCod: -999
        });

    ajaxAgronicaSync(indirizzohttp + "/ControllaContattoRiferimenti",
        param, false,
        function (risposta) {
            if (risposta.Errore !== "") {
                kendo.alert(risposta.Errore);
                retVal = false;
            }
        },
        function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
            retVal = false;
        });

    return retVal;

}

function RedirectAnagrafica(targetUrl) {

    let parametri = JSON.stringify(
        { targetUrl: targetUrl });

    ajaxAgronica(indirizzohttp + "/RedirectToMenuAnagrafica",
        parametri,
        function (risposta) {
            let parametroVisibilita = riportaParametroVisibilita();
            if (risposta.RispostaStringa.indexOf("?") >= 0) {
                parametroVisibilita = parametroVisibilita.replace("?", "&")
            }
            window.location.href = risposta.RispostaStringa + parametroVisibilita;
        }, null, null, false);
}

function riportaParametroVisibilita() {
    const urlParams = new URLSearchParams(window.location.search);
    const myParam = urlParams.get('visibilita');
    let returnString = ""
    if (myParam != undefined) {
        returnString = "?visibilita=" + myParam;
    }
    return returnString;
}

//ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
function GetUrlDocAgenda2010(piva, cod_contatto, operazione) {
    var parametri = kendo.stringify({
        "piva": piva,
        "cod_contatto": cod_contatto,
        "operazione": operazione,
    });

    var risp = "";

    ajaxAgronicaSync(indirizzohttp + "/GetUrlDocAgenda2010", parametri, false,
        function (risposta) {
            risp = risposta.RispostaStringa;
        }, null);

    return risp;
}

function LeggiImpostazioniUtente(impostazione_cod) {
    let resp = "notSet";
    let param = {
        objP_Utenti: objP_utenti,
        impostazione_cod: impostazione_cod
    }

    ajaxAgronica(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_Impostazioni", JSON.stringify(param),
        function (risposta) {
            resp = risposta.RispostaStringa;
        }, null, null, false);
    return resp;
}

function LeggiImpostazioniSuperuser(impostazione_cod) {
    let resp;
    let param = {
        objP_Utenti: objP_utenti,
        impostazione_cod: impostazione_cod,
        Username_1Utente_o_2SuperUser: 2
    }

    ajaxAgronicaSync(pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_ImpostazioniByUsername_1Utente_o_2SuperUser",
        JSON.stringify(param), false,
        function (risposta) {
            resp = risposta.RispostaStringa;
        }, null);
    return resp;
}
