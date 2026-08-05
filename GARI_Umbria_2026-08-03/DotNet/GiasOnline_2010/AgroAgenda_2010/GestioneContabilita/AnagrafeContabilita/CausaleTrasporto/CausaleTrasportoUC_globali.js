var elencoTipoCausale = null;

function ElencoTipoCausaliTrasporto() {
    if (elencoTipoCausale == null) {
        elencoTipoCausale = [
            { Tipo_Cod: 0, Tipo_Des: TraduzioneMultiResx(resxObj, "TuttiDocumentiContabili", "Tutti i Documenti Contabili") },
            { Tipo_Cod: 1, Tipo_Des: TraduzioneMultiResx(resxObj, "DocumentiContabiliAttivi", "Documenti Contabili Attivi") },
            { Tipo_Cod: 2, Tipo_Des: TraduzioneMultiResx(resxObj, "DocumentiContabiliPassivi", "Documenti Contabili Passivi") },
            { Tipo_Cod: 3, Tipo_Des: TraduzioneMultiResx(resxObj, "AccettazioneBeni", "Accettazione Beni") },
            { Tipo_Cod: 5, Tipo_Des: TraduzioneMultiResx(resxObj, "ContrattiAffitto", "Contratti Affitto") }
        ];
    }
    return elencoTipoCausale;
}