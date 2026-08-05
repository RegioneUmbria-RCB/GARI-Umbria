var IDControllo = null;
var elencoModuloGenerazione = null;

var isAttivaCampiMinMax = [3];   // Attivo per: Tipo_Cod: 3 (Numero)
var isAttivaCampoDecimali = [3];    // Attivo per: Tipo_Cod: 3 (Numero)
var editabiliPerCondizione = [ "Valore_Minimo", "Valore_Massimo", "NumDecimali_Massimo" ];

var elencoTipoParametro = null; 

function ElencoTipoParametri() {
    if (elencoTipoParametro == null) {
        elencoTipoParametro = [
            { Tipo_Cod: 0, Tipo_Des: "" },
            { Tipo_Cod: 1, Tipo_Des: TraduzioneMultiResx(resxObj, "SceltaDaLista", "Scelta da lista") },
            { Tipo_Cod: 2, Tipo_Des: TraduzioneMultiResx(resxObj, "GruppoFatturazione", "Gruppo Fatturazione") },
            { Tipo_Cod: 3, Tipo_Des: TraduzioneMultiResx(resxObj, "Numero", "Numero") },
            { Tipo_Cod: 4, Tipo_Des: TraduzioneMultiResx(resxObj, "Caratteri", "Caratteri") },
            { Tipo_Cod: 5, Tipo_Des: TraduzioneMultiResx(resxObj, "Data", "Data") }
        ];
    }
    return elencoTipoParametro;
}