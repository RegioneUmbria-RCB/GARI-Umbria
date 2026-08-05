

function impostaRiepilogoPesiUC(ID_Controllo_TabMovimenti) {

    //if (lavCodAccettazione === true) {
        KendoNumTB("inPesoTotaleRiepilogo").enable(true);
    //} else {
    //    KendoNumTB("inPesoTotaleRiepilogo").enable(false);
    //}

    let testataDoc = getDatiContabTestata();

    if (testataDoc !== undefined && testataDoc !== null && testataDoc.IdAgenda !== 0) {

        let taraVeicolo = kendo.parseFloat(testataDoc.TaraVeicolo);
        let taraImballi = kendo.parseFloat(testataDoc.TaraImballi);




        //tipoPesoRiepilogo = parseInt(testataDoc.TipoPeso);
        //let peso = kendo.parseFloat(testataDoc.Peso);

        let pesoNetto = 0;
        //if (testataDoc.ConteggiTotali !== undefined && testataDoc.ConteggiTotali !== null) {
        //    pesoNetto = kendo.parseFloat(testataDoc.ConteggiTotali.PesoNettoProd);
        //}
        pesoNetto = kendo.parseFloat(testataDoc.PesoNettoProd);

        let imballiVuoti = kendo.parseFloat(testataDoc.ImballiVuoti);

        let pesoLordo = pesoNetto + taraImballi;
        let pesoTotale = pesoLordo + imballiVuoti + taraVeicolo;

        //testataDoc.Peso nella nuova versione contiene sempre il peso totale
        let pesoTotaleDb = kendo.parseFloat(testataDoc.Peso);

        //if (lavCodAccettazione === true) {
            pesoTotale = pesoTotaleDb;
        //}

        if (pesoTotale !== pesoTotaleDb) {
            kendo.alert(TraduzioneMultiResx(resxRiepilogoPesiUC, "PesoTotaleCalcolatoDiversoDaMemorizzato", "il peso totale calcolato è diverso da quello memorizzato"));
        }


        //if (tipoPesoRiepilogo === 1)
        //    pesoNetto = peso;
        //else
        //    pesoLordo = peso;


        let degrado = 0;
        if ($("#" + ID_Controllo_TabMovimenti).data("kendoGrid").columns.some(function (column) { return column.field === "Degrado_Calcolato"; }))
            degrado = kendo.parseFloat($("#" + ID_Controllo_TabMovimenti).data("kendoGrid").dataSource.aggregates().Degrado_Calcolato.sum);

        ImpostaControlliRiepilogoPesi(pesoTotale, taraVeicolo, pesoLordo, taraImballi, pesoNetto, degrado, 0, imballiVuoti);

        RicalcolaRiepilogoPesi(true);

    }

    //Public Enum enTipoPeso
    //    peso_lordo = 0
    //    Peso_Netto = 1
    //End Enum
}

function inPesoTotaleRiepilogo_change(e) {

    let pesoTotale = kendo.parseFloat(e.sender.value());

    //if (lavCodAccettazione === true) {
        let taraVeicolo = kendo.parseFloat(Get_KendoNumTBValue("inTaraVeicoloRiepilogo"));

        if (taraVeicolo === 0 && lavCodAccettazione === true) {
            Set_KendoNumTBValue("inTaraVeicoloRiepilogo", pesoTotale);
        }

    //} else {
    //    RicalcolaRiepilogoPesi(false);
    //}

    //Il peso totale va messo anche in testata, almeno per il momento che non c'è la possibilità di modificarlo
    Set_KendoNumTBValue("inPesoTotale", kendo.parseFloat(pesoTotale));

}

function inTaraVeicoloRiepilogo_change(e) {

    if (lavCodAccettazione === true) {
        let pesoTotale = kendo.parseFloat(Get_KendoNumTBValue("inPesoTotaleRiepilogo"));

        if (pesoTotale === 0) {
            let taraVeicolo = kendo.parseFloat(e.sender.value());
            Set_KendoNumTBValue("inPesoTotaleRiepilogo", taraVeicolo);

            //Il peso totale va messo anche in testata, almeno per il momento che non c'è la possibilità di modificarlo
            Set_KendoNumTBValue("inPesoTotale", taraVeicolo);
        }

    } else {
        RicalcolaRiepilogoPesi(false);
    }
}

function riepilogoPesi_change(e) {
    RicalcolaRiepilogoPesi(false);
}

function RicalcolaRiepilogoPesi(inizializzazione) {

    let pesoTotale = kendo.parseFloat(Get_KendoNumTBValue("inPesoTotaleRiepilogo"));
    let taraVeicolo = kendo.parseFloat(Get_KendoNumTBValue("inTaraVeicoloRiepilogo"));    // Questo è l'unico modificabile da interfaccia
    let imballiVuoti = kendo.parseFloat(Get_KendoNumTBValue("inImballiVuotiRiepilogo"));
    let pesoLordo = kendo.parseFloat(Get_KendoNumTBValue("inPesoLordoRiepilogo"));
    let taraImballi = kendo.parseFloat(Get_KendoNumTBValue("inTaraImballiRiepilogo"));      //Mi arriva preciso come somma di riga (fissato su db)
    let pesoNetto = kendo.parseFloat(Get_KendoNumTBValue("inPesoNettoRiepilogo"));          //Mi arriva come somma di riga
    let variazione = kendo.parseFloat(Get_KendoNumTBValue("inValoreDegradoRiepilogo"));
    let pesoEffettivo = kendo.parseFloat(Get_KendoNumTBValue("inPesoPagamentoRiepilogo"));


    //il peso netto mi arriva sempre come la somma dei pesi netti di riga

    //tara imballi mi arriva sempre preciso

    pesoLordo = pesoNetto + taraImballi;

    //if (lavCodAccettazione === false) {
    //    pesoTotale = pesoLordo + imballiVuoti + taraVeicolo;
    //}

    pesoEffettivo = pesoNetto - variazione;



    //if (inizializzazione === true && (kendo.parseFloat(taraVeicolo) + Math.abs(pesoLordo) > 0)) {
    //    pesoTotale = kendo.parseFloat(taraVeicolo) + Math.abs(pesoLordo);
    //}

    //switch (tipoPesoRiepilogo) {
    //    case 0: //Peso LORDO
    //        pesoNetto = kendo.parseFloat(pesoTotale) - kendo.parseFloat(taraVeicolo) - kendo.parseFloat(taraImballi);
    //        break;

    //    case 1: //Peso NETTO
    //        pesoLordo = kendo.parseFloat(pesoNetto) + kendo.parseFloat(taraImballi);
    //        break;
    //}

    //pesoLordo = kendo.parseFloat(pesoNetto) + kendo.parseFloat(taraImballi);

    ////variazionePercentuale = Format(MSScheda.TextMatrix(Riga, COL_VARIAZIONE), "##0.0") & "%"
    ////variazione = (kendo.parseFloat(MSScheda.TextMatrix(Riga, COL_PESO_NETTO)) / 100) * MSScheda.TextMatrix(Riga, COL_VARIAZIONE);


    //pesoEffettivo = kendo.parseFloat(pesoNetto) - kendo.parseFloat(variazione);
    ////prezzoTotale = kendo.parseFloat(prezzo) * kendo.parseFloat(pesoEffettivo);

    ImpostaControlliRiepilogoPesi(pesoTotale, taraVeicolo, pesoLordo, taraImballi, pesoNetto, variazione, pesoEffettivo, imballiVuoti);
}

function ImpostaControlliRiepilogoPesi(pesoTotale, taraVeicolo, pesoLordo, taraImballi, pesoNetto, variazione, pesoEffettivo, imballiVuoti) {

    Set_KendoNumTBValue("inPesoTotaleRiepilogo", kendo.parseFloat(pesoTotale));

    //Il peso totale va messo anche in testata, almeno per il momento che non c'è la possibilità di modificarlo
    Set_KendoNumTBValue("inPesoTotale", kendo.parseFloat(pesoTotale));

    Set_KendoNumTBValue("inTaraVeicoloRiepilogo", kendo.parseFloat(taraVeicolo));
    Set_KendoNumTBValue("inImballiVuotiRiepilogo", kendo.parseFloat(imballiVuoti));
    Set_KendoNumTBValue("inPesoLordoRiepilogo", kendo.parseFloat(pesoLordo));
    Set_KendoNumTBValue("inTaraImballiRiepilogo", kendo.parseFloat(taraImballi));
    Set_KendoNumTBValue("inPesoNettoRiepilogo", kendo.parseFloat(pesoNetto));
    Set_KendoNumTBValue("inValoreDegradoRiepilogo", kendo.parseFloat(variazione));
    Set_KendoNumTBValue("inPesoPagamentoRiepilogo", kendo.parseFloat(pesoEffettivo));
}

function VerificaCongruenzaPesi() {
    let msg = "";
    let pesoTotale = kendo.parseFloat(Get_KendoNumTBValue("inPesoTotaleRiepilogo"));
    let taraVeicolo = kendo.parseFloat(Get_KendoNumTBValue("inTaraVeicoloRiepilogo"));
    let imballiVuoti = kendo.parseFloat(Get_KendoNumTBValue("inImballiVuotiRiepilogo"));
    let pesoLordo = kendo.parseFloat(Get_KendoNumTBValue("inPesoLordoRiepilogo"));
    
    if (pesoTotale !== (taraVeicolo + imballiVuoti + pesoLordo) &&
        pesoTotale !== 0) {

        if (lavCodAccettazione === false || taraVeicolo !== 0) {
            msg = TraduzioneMultiResx(resxRiepilogoPesiUC, "AttenzionePesiDelDocumentoNonCoerenti",
                'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento è diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.');
        }

        //Se sono stato modificati il peso totale o la tara totale e non sono nel salvataggio della riga e c'è una sola riga ricalcolo il peso lordo
        if (lavCodAccettazione && !sonoInDettaglioRigaDoc) {
            msg = Forza_Modifica_Riga_Doc(pesoTotale - (taraVeicolo + imballiVuoti), msg); 
            UscitaDaRigaDoc();
        }        
        
    }

    return msg;
}