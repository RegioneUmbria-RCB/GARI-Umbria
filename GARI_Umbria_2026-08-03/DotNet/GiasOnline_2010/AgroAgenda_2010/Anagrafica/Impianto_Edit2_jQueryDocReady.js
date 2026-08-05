/**

DOC READY

**/

var objParametri_Agenda;
var permesso_centro;
var permesso_impresa;

var specievegetali = "";
const AGRODATAFINEIMP = new Date(2100, 11, 31, 0, 0, 0, 0);
const DEFAULTDATE = kendo.parseDate("0001-01-01T00:00:00");

var dati_impianto_Loaded = false;
var dati_accessori_Loaded = false;
var dati_distinte_Loaded = false;

//Dati Impianto
var TxtSuperficie;
var TxtValiditaInizio;
var TxtValiditaFine;

// - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
var Txt_DataInizioInnesto;
var Txt_DataInizioProduzione;
// - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -


//Dati Accessori
var Txt_DistanzaTraFila_M;
var Txt_DistanzaSuFila_M;
var Txt_Interbina;
var Txt_Germinabilita;
var Txt_Superficie2;
var TxtPianteHa;
var TxtPianteImpianto;
var TxtCopDataInizio;
var TxtCopDataFine;
var TXT_UnitaVitata;

var Txt_Resa1;
var Txt_Resa2;

var Txt_CodBMBDBT_M;
var Txt_CodBMBDBT_F;
var Txt_Genetica_M;
var Txt_Genetica_F;
var Txt_OffType_M;
var Txt_OffType_F;
var Txt_DistanzaSuFila_F;
var Txt_DistanzaTraFila_F;
var Txt_PartiTuberi;

//Dati Distinte
var gridDistinte;

var gridCodici;
var gridParticelle;

var Txt_ValiditaInizio_Distinta;
var Txt_ValiditaFine_Distinta;
var Txt_PianteImpianto2;
var Txt_semina_prevista;
var Txt_fioritura_prevista;
var Txt_raccolta_prevista;
var Txt_Lotto;
//  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
var Txt_PianteImpianto_Femmina;
var Txt_PianteImpianto_Maschio;
// - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
// var Txt_Sup_Prog;

var chk_distinta_chiusa;
var chk_secondo_raccolto;

var TxtN;
var TxtP2O5;
var TxtK2O;
var TxtMgO;

//Combo
var Cmb_Specie;
var Cmb_Finalita;
var Cmb_Cultivar;
var Cmb_CodiciTerreno;

var Cmb_TipologiaVarietale;
var Cmb_Copertura;
var Cmb_ImpIrrigazione;
var Cmb_FormaAllevamento;
var Cmb_Portinnesto;
var Cmb_SeminaTrapianto;
var Cmb_ProvenienzaSeme;
var Cmb_DettaglioVarietaPersonalizzato;
var Cmb_CodiceZona;
var Cmb_ConduzioneSu;
var Cmb_ConduzioneTra;
var Cmb_TagliatoTuberi;

var Cmb_Regolamento;
var Cmb_Disciplinare;
var Cmb_IAF;
var Cmb_CapitolatoPrivato;
var Cmb_OrganismoReferente;
// - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
var Cmb_LicenzaColtivazione;
//- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
var Cmb_Riferimento_Trasferimento_Dati;
var Cmb_MagazzinoConferimento;
var Cmb_RegolamentoConc;
var Cmb_FinalitaConc;
var Cmb_Stato;
var Cmb_PianoSemina;

var Cmb_Codici;

var Cmb_Operazioni;
var windowOperazione;
var Operazione_Sel;

var Cmb_Particelle;
var Cmb_CodiciParticelle;


var btn_salva_distinta;
var btnSalva;
var btnSalvaScrivi;
var btnSalva_e_Op; 

var flag_schedaDatiAccessoriInizializzata = false;
var btn_calcola_pianteImpianto;

var flag_disciplinareprivato;

var impiantoEditResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/Impianto_Edit2.aspx.resx"
];

jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            impiantoEditResx.push(readResxFile(resxSinglePath, "Impianto_Edit2_jQueryDocReady.js"));
        });
    }

    docReady();

});

async function docReady() {

    $("#TxtSuperficie").on("change", function () {
        TestSuperficieCfrGis();
    });

    btnSalva = $("#btnSalva");
    btnSalvaScrivi = $("#btnSalvaScrivi");
    btnSalva_e_Op = $("#btnSalva_e_Op");
    btn_salva_distinta = $("#btn_salva_distinta");

    btn_nuova_distinta = $("#btn_nuova_distinta");
    btn_nuova_distinta.click(nuovaDistinta);

    btn_calcola_pianteImpianto = $("#btn_calcola_pianteImpianto");
    //btn_calcola_pianteImpianto.click(calcola_pianteImpianto);

    objParametri_Agenda = JSON.parse(objP_agenda);

    flag_disciplinareprivato = await LeggiDisciplinarePrivato();

    if (objParametri_Agenda.Tipo_Operazione !== "0") {
        btnSalva.show();
        btnSalva_e_Op.show();
        btn_nuova_distinta.show();
    } else {
        btnSalva.hide();
        btnSalva_e_Op.hide();
        btn_nuova_distinta.hide();
    }

    if (objParametri_Agenda.Tipo_Operazione === "1") {
        btnSalvaScrivi.show();
    } else {
        btnSalvaScrivi.hide();
    }

    if (!obj_Permessi_IAF) {
        $("#rowIAF").hide();
    } else {
        $("#rowIAF").show();
    }

    $("#tabs").kendoTabStrip({
        select: function (e) {
            let elem = $(e.item).index();
            switch (elem) {
                case 0:
                    if (!dati_impianto_Loaded) {
                        WaitFrame.show();
                        dati_impianto().then(() => {
                            WaitFrame.hide();
                        });
                        dati_impianto_Loaded = true;
                    }
                    if (!dati_distinte_Loaded) {
                        WaitFrame.show();
                        dati_distinte().then(() => {
                            WaitFrame.hide();
                        });
                        dati_distinte_Loaded = true;
                    }
                    break;
                case 1:
                    if (!dati_accessori_Loaded) {
                        WaitFrame.show();
                        dati_accessori().then(() => {
                            WaitFrame.hide();
                        });
                        dati_accessori_Loaded = true;
                    }
                    break;
            }
        },
        animation: {
            open: {
                effects: "fadeIn"
            }
        }
    });
    kendoTab = $("#tabs").data("kendoTabStrip");

    // imposta tab attivo passando parametro query string
    var qsTab = Request_QueryString("tab");
    if (qsTab !== null && qsTab !== "") {
        if (!dati_impianto_Loaded) {
            WaitFrame.show();
            dati_impianto().then(() => {
                WaitFrame.hide();
            });
            dati_impianto_Loaded = true;
        }
        kendoTab.select(qsTab);
    } else {
        kendoTab.select(0);
    } 
}

function dati_impianto() {

    return new Promise((resolve, reject) => {
        let promises = new Array();
        promises.push(inizializzaCampi_DatiImpianto(), impostaDati_DatiImpianto());
        Promise.all(promises).then(() => {
            //if (obj_Impianto.destinazioni_presenti) {
            //    if (obj_Impianto.veg_cod !== 0) {
            //        Cmb_Specie.enable(false);
            //        //Cmb_GruppoVegetale.enable(false);
            //        if (obj_Impianto.grfi_cod !== 0 && obj_Impianto.grfi_cod !== "" && obj_Impianto.grfi_cod !== undefined) {
            //            Cmb_Finalita.enable(false);
            //        }
            //    } else {
            //        Cmb_CodiciTerreno.enable(false);
            //    }
            //    if (obj_Impianto.gru_cod !== 0) {
            //        $("#ChkTerrenoNudo").prop("disabled", true);
            //    }
            //}

            if (objParametri_Agenda.Tipo_Operazione === "0") {
                TxtSuperficie.enable(false);
                TxtValiditaInizio.enable(false);
                TxtValiditaFine.enable(false);

                //- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                Txt_DataInizioInnesto.enable(false);
                Txt_DataInizioProduzione.enable(false);
                //- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                if (Cmb_Specie !== undefined) {
                    Cmb_Specie.enable(false);
                    Cmb_Finalita.enable(false);
                    Cmb_Cultivar.enable(false);
                }

                if (Cmb_CodiciTerreno !== undefined) {
                    Cmb_CodiciTerreno.enable(false);
                }
            }

            resolve();
        });
    });

}

function dati_accessori() {

    return new Promise((resolve, reject) => {
        let promises = new Array();
        promises.push(inizializzaCampi_DatiAccessori(), impostaDati_DatiAccessori());
        Promise.all(promises).then(() => {

            if (objParametri_Agenda.Tipo_Operazione === "0") {
                Txt_DistanzaTraFila_M.enable(false);
                Txt_DistanzaSuFila_M.enable(false);
                Txt_Interbina.enable(false);
                Txt_Germinabilita.enable(false);
                Txt_Superficie2.enable(false);
                TxtPianteHa.enable(false);
                TxtPianteImpianto.enable(false);
                TxtCopDataInizio.enable(false);
                TxtCopDataFine.enable(false);
                TXT_UnitaVitata.enable(false);

                Txt_Resa1.enable(false);
                Txt_Resa2.enable(false);

                $("#Txt_CodBMBDBT_M").prop('disabled', true);
                $("#Txt_CodBMBDBT_F").prop('disabled', true);
                $("#Txt_Genetica_M").prop('disabled', true);
                $("#Txt_Genetica_F").prop('disabled', true);
                $("#Txt_OffType_M").prop('disabled', true);
                $("#Txt_OffType_F").prop('disabled', true);
                $("#Txt_DistanzaSuFila_F").prop('disabled', true);
                $("#Txt_DistanzaTraFila_F").prop('disabled', true);
                $("#Txt_PartiTuberi").prop('disabled', true);
                $("#Txt_CodiceImpianto").prop('disabled', true);

                Cmb_TipologiaVarietale.enable(false);
                Cmb_Copertura.enable(false);
                Cmb_ImpIrrigazione.enable(false);
                Cmb_FormaAllevamento.enable(false);
                Cmb_Portinnesto.enable(false);
                Cmb_SeminaTrapianto.enable(false);
                Cmb_ProvenienzaSeme.enable(false);
                Cmb_DettaglioVarietaPersonalizzato.enable(false);
                Cmb_CodiceZona.enable(false);
                Cmb_ConduzioneSu.enable(false);
                Cmb_ConduzioneTra.enable(false);
                Cmb_TagliatoTuberi.enable(false);

                $("#ChkCoverCrops").prop('disabled', true);
                $("#ChkMonitorato").prop('disabled', true);
                $("#ChkConsociazione").prop('disabled', true);
                //  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                $("#ChkMaschiSesto").prop('disabled', true);
                // - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
            }

            resolve();
        });
    });

}

function dati_distinte() {
    return new Promise((resolve, reject) => {
        let promises = new Array();
        promises.push(inizializzaCampi_DatiDistinte(), impostaDati_DatiDistinte());
        Promise.all(promises).then(() => {
            impostaPermessiDatiDistinta(false);

            selezionaUltimaDistintaInfo();

            resolve();
        });
    });
}

function selezionaUltimaDistintaInfo() {
    rowArr = gridDistinte.content.find("tr");
    if (rowArr.length > 0) {
        var datiRiga = gridDistinte.dataItem(rowArr[0]);
        impostaDatiDistinta(datiRiga);
        impostaPermessiDatiDistinta(false);
        chk_distinta_chiusa.enable(false);
        if (replicaGIAS !== "") chk_distinta_replica.enable(false);
        gridDistinte.select(rowArr[0]);
    }
}