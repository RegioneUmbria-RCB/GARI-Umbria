         

//VARIABILI GLOBALI

//Elenchi Json
/**
 * 
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_Elenco_Ripartizione(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [{}];
    }

    Elenco_Ripartizione = [
        { Tipo_Ripartizione: 0, Tipo_Des_Ripartizione: TraduzioneMultiResx(resxObj, "ManualeSingoliImpianti", "Manuale sui singoli impianti") },
        { Tipo_Ripartizione: 1, Tipo_Des_Ripartizione: TraduzioneMultiResx(resxObj, "AutomaticaSuperfici", "Automatica sulle superfici") },
        { Tipo_Ripartizione: 2, Tipo_Des_Ripartizione: TraduzioneMultiResx(resxObj, "AutomaticaPianteImpianto", "Automatica sulle piante/impianto") }
    ];
}

/**
 * 
 * @param {any} resxObj Se passato, imposta le descrizioni sulla lingua impostata per l'utente
 */
function Inizializza_Elenco_FiltroImpianti(resxObj) {

    if (resxObj === null || resxObj === undefined || (Array.isArray(resxObj) && resxObj.length === 0)) {
        resxObj = [{}];
    }

    Elenco_FiltroImpianti = [
        {
            Tipo_Filtro: 0, Tipo_Filtro_Des:
                TraduzioneMultiResx(resxObj, "Specie", "Specie") + "-" +
                TraduzioneMultiResx(resxObj, "Varietà", "Varietà") + "-" +
                TraduzioneMultiResx(resxObj, "Regolamento", "Regolamento")
        },
        {
            Tipo_Filtro: 1, Tipo_Filtro_Des:
                TraduzioneMultiResx(resxObj, "Specie", "Specie") + "-" +
                TraduzioneMultiResx(resxObj, "Varietà", "Varietà")
        },
        { Tipo_Filtro: 2, Tipo_Filtro_Des: TraduzioneMultiResx(resxObj, "SpecieVegetale", "Specie Vegetale") }
    ];
}

var Elenco_Ripartizione = [];
var Elenco_FiltroImpianti = [];

// Variabili

var righeInseriteGrid_Impianti;
var righeModificateGrid_Impianti;
var righeCancellateGrid_Impianti;


var id_mov_det = 0;
var elem_cod = 0;
var mat_cod = 0;
var filtro_impianti = 0;

var Modalita_Ripartizione = 0;
var Obbligo_Ripartizione = 0;

var resxSceltaImpiantiUC = [];
