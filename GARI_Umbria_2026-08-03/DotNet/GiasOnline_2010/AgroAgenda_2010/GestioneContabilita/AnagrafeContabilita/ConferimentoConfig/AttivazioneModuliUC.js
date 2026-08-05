function ImpostaSwitches() {
    ImpostaSwitch("#chkTrasformazioniVegetali", statoAttivazione.trasformazioniVegetali, !statoAttivazione.disattivaTrasformazioniVegetali);
    ImpostaSwitch("#chkTrasformazioniAnimali", statoAttivazione.trasformazioniAnimali, !statoAttivazione.disattivaTrasformazioniAnimali);
}

function ImpostaSwitch(idControllo, checked, enabled) {
    let chk = $(idControllo).data("kendoSwitch");

    if (chk === undefined || chk === null || chk === "") {
        $(idControllo).kendoSwitch({
            checked: checked,
            enabled: enabled
        });
    }
    else {
        chk.check(checked);
        chk.enable(enabled);
    }
}

function Conferma_Annulla_AttivazioneModuli() {
    Conferimento_Attivazione_ModuliUC_DocReady();
}
