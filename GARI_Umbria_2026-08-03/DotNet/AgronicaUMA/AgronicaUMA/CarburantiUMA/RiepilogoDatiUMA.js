
async function SetCampiRiepilogoDatiAzienda(dt) {
    var dt0 = dt[0];
    var pivaReale = "";
    pivaReale = await CercaPivaRealeD(QS_Piva)

    if (pivaReale != "") {
        $("#piva").html(pivaReale);
    } else {
        $("#piva").html(QS_Piva);
    }
    $("#conto").html(QS_Type == -1 ? "Terzi" : "Proprio");
    $("#azienda").html(dt0.azienda);
    $("#cuaa").html(dt0.cuaa);
    $("#address").html(dt0.address);
    $("#citta").html(dt0.citta);
    $("#cap").html(dt0.cap);
    $("#prov").html(dt0.prov);
    $("#telefono").html(dt0.telefono);
    $("#email").html(dt0.email);
}

function SetCampiRiepilogoDatiCarburanti(dt) {
    var dt0 = dt[0];

    SetCarburantiSectionHtml("litriAssRichiestaAnnoPrec",
        dt0.litriAssRichiestaAnnoPrec_Gasolio, dt0.litriAssRichiestaAnnoPrec_Benzina, dt0.litriAssRichiestaAnnoPrec_GasolioSerra,
        dt0.litriAssRichiestaAnnoPrec_Richiedente, dt0.litriAssRichiestaAnnoPrec_Approvatore);

    SetCarburantiSectionHtml("litriAssRendicontazioneAnnoPrec",
        dt0.litriAssRendicontazioneAnnoPrec_Gasolio, dt0.litriAssRendicontazioneAnnoPrec_Benzina, dt0.litriAssRendicontazioneAnnoPrec_GasolioSerra,
        dt0.litriAssRendicontazioneAnnoPrec_Richiedente, dt0.litriAssRendicontazioneAnnoPrec_Approvatore);

    SetCarburantiSectionHtml("litriRimAnnoPrec",
        dt0.litriRimAnnoPrec_Gasolio, dt0.litriRimAnnoPrec_Benzina, dt0.litriRimAnnoPrec_GasolioSerra,
        null, null);

    $("#litriRecuperoAccDichiarati").html(GetCarburantiHtml(dt0.litriRecuperoAccDichiarati_Gasolio, dt0.litriRecuperoAccDichiarati_Benzina, dt0.litriRecuperoAccDichiarati_GasolioSerra));
    $("#litriRecuperoAccConfermati").html(GetCarburantiHtml(dt0.litriRecuperoAccConfermati_Gasolio, dt0.litriRecuperoAccConfermati_Benzina, dt0.litriRecuperoAccConfermati_GasolioSerra));

    //if (dt0.litriRecuperoAccDichiarati_Gasolio > 0 || dt0.litriRecuperoAccDichiarati_Benzina > 0 || dt0.litriRecuperoAccDichiarati_GasolioSerra > 0 ||
    //    dt0.litriRecuperoAccConfermati_Gasolio > 0 || dt0.litriRecuperoAccConfermati_Benzina > 0 || dt0.litriRecuperoAccConfermati_GasolioSerra > 0) {
    //    $("#litriRecuperoAccSection").show();
    //} else {
    //    $("#litriRecuperoAccSection").hide();
    //}

    SetCarburantiSectionHtml("litriAnticipoAnnoCorr",
        dt0.litriAnticipoAnnoCorr_Gasolio, dt0.litriAnticipoAnnoCorr_Benzina, dt0.litriAnticipoAnnoCorr_GasolioSerra,
        null, null);

    SetCarburantiSectionHtml("litriAssPrimaRichiesta",
        dt0.litriAssPrimaRichiesta_Gasolio, dt0.litriAssPrimaRichiesta_Benzina, dt0.litriAssPrimaRichiesta_GasolioSerra,
        dt0.litriAssPrimaRichiesta_Richiedente, dt0.litriAssPrimaRichiesta_Approvatore);

    var integrativeSection = document.getElementById('richiesteIntegrative');
    integrativeSection.innerHTML = "";

    var Numero_Integrative = dt0.Numero_Integrative;
    for (i = 1; i <= Numero_Integrative; i++) {
        var litriAssIntegrazione_Gasolio = dt0['litriAssIntegrazione' + i + '_Gasolio'];
        var litriAssIntegrazione_Benzina = dt0['litriAssIntegrazione' + i + '_Benzina'];
        var litriAssIntegrazione_GasolioSerra = dt0['litriAssIntegrazione' + i + '_GasolioSerra'];
        var litriAssIntegrazione_Richiedente = dt0['litriAssIntegrazione' + i + '_Richiedente'];
        var litriAssIntegrazione_Approvatore = dt0['litriAssIntegrazione' + i + '_Approvatore'];

        var newDiv = document.createElement('div');
        newDiv.id = 'litriAssIntegrazione' + i + 'Section';

        newDiv.innerHTML = `
               <div class="row">
                   <div class="form-horizontal">
                       <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                           <label class="infoLabel">
                               Litri assegnati con integrazione ` + i + `
                          </label>
                       </div >
                       <div id="litriAssIntegrazione` + i + `">
                            ` + GetCarburantiHtml(litriAssIntegrazione_Gasolio, litriAssIntegrazione_Benzina, litriAssIntegrazione_GasolioSerra) + `
                      </div>
                   </div>
               </div>
               <div class="row">
                  <div class="form-horizontal">
                       <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                           <label class="infoLabel">
                               Richiedente
                          </label>
                       </div >
                       <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                           <label class="info" id="litriAssIntegrazione` + i + `_Richiedente">
                                ` + litriAssIntegrazione_Richiedente + `
                          </label>
                       </div>
                   </div>
               </div>
               <div class="row">
                   <div class="form-horizontal">
                       <div class="col-lg-5 col-md-5 col-sm-12 text-left">
                           <label class="infoLabel">
                               Approvatore
                          </label>
                       </div >
                       <div class="col-lg-7 col-md-7 col-sm-12 text-left">
                           <label class="info" id="litriAssIntegrazione` + i + `_Approvatore">
                                `+ litriAssIntegrazione_Approvatore + `
                          </label>
                       </div>
                   </div>
               </div>
               <div style="height: 30px"></div>
            `;

        integrativeSection.appendChild(newDiv);
    }

    $("#litriAcquistatiAllaData").html(GetCarburantiHtml(dt0.litriAcquistatiAllaData_Gasolio, dt0.litriAcquistatiAllaData_Benzina, dt0.litriAcquistatiAllaData_GasolioSerra));
    $("#litriAcquistabiliAllaData").html(GetCarburantiHtml(dt0.litriAcquistabiliAllaData_Gasolio, dt0.litriAcquistabiliAllaData_Benzina, dt0.litriAcquistabiliAllaData_GasolioSerra));

}

function SetCarburantiSectionHtml(name, gasolio, benzina, gasolio_serra, richiedente, approvatore) {

    $("#" + name).html(GetCarburantiHtml(gasolio, benzina, gasolio_serra));

    if (richiedente != null) {
        $("#" + name + "_Richiedente").html(richiedente);
    }
    if (approvatore != null) {
        $("#" + name + "_Approvatore").html(approvatore);
    }

    //var sectionContainer = $("#" + name + "Section");
    //if (sectionContainer != null) {
    //    if (gasolio > 0 || benzina > 0 || gasolio_serra > 0) {
    //        sectionContainer.show();
    //    } else {
    //        sectionContainer.hide();
    //    }
    //}
}

function GetCarburantiHtml(gasolio, benzina, gasolio_serra) {
    let html = `
        <div class="col-lg-2 col-md-2 col-sm-12">
           <label class="info">
        ` + (gasolio > 0 ? ("Gasolio: " + Math.round(gasolio) + " lt") : "") + `
           </label>
        </div>
        <div class="col-lg-3 col-md-3 col-sm-12">
           <label class="info">
        ` + (gasolio_serra > 0 ? ("Gasolio Serra: " + Math.round(gasolio_serra) + " lt") : "") + `
           </label>
        </div>
        <div class="col-lg-2 col-md-2 col-sm-12">
           <label class="info">
        ` + (benzina > 0 ? ("Benzina: " + Math.round(benzina) + " lt") : "") + `
           </label>
        </div>
    `;

    return html
}

//function HideCampiRiepilogoDatiCarburanti() {
//    $("#litriAssRichiestaAnnoPrecSection").hide();
//    $("#litriAssRendicontazioneAnnoPrecSection").hide();
//    $("#litriRimAnnoPrecSection").hide();
//    $("#litriRecuperoAccSection").hide();
//    $("#litriRecuperoAccSectionSection").hide();
//    $("#litriAnticipoAnnoCorrSection").hide();
//    $("#litriAssPrimaRichiestaSection").hide();
//    $("#litriAcquistatiAllaDataSection").hide();
//    $("#litriAcquistabiliAllaDataSection").hide();
//}