
var elencoRapportiContabili = null;
var elencoQualifiche = null;
var elencoMansioni = null;
var elencoTipiRapporto = null;
var elencoClassificazioniRisUm = null;
var elencoTipiRubrica = null;
var elencoTipiCosto = null;
var elencoRapportiSelezionati = null;
var dropDownCostiRapporti = null;
var dropDownTipologiaIndirizzo = null;
var dropDownStati = null;
var dropDownLingua = null;
var dropDownProvince = null;
var dropDownComuni = null;
var elencoCap = null;
var elencoIstitutiCredito = null;
var elencoConti = null;
var elencoCodiciLingue = null;
var ddlCentriAziendaliPrevValue = null;

const enum_tipoListino = {
    Acquisto: 1,
    Vendita: 2
}

var jsonRapporti = '[{"TipoRapporto_Des": "Continuativo","TipoRapporto_Cod": 0},{"TipoRapporto_Des": "Occasionale","TipoRapporto_Cod": 1}]';
elencoTipiRapporto = JSON.parse(jsonRapporti);

var jsonTipiRubrica = '[{"TipoRubrica_Des": "Telefono","TipoRubrica_Cod": 0}, {"TipoRubrica_Des": "Fax","TipoRubrica_Cod": 1},{"TipoRubrica_Des": "Cellulare","TipoRubrica_Cod": 2},{"TipoRubrica_Des": "E-Mail","TipoRubrica_Cod": 3},{"TipoRubrica_Des": "Sito Web","TipoRubrica_Cod": 4},{"TipoRubrica_Des": "Non Assegnato","TipoRubrica_Cod": 5}]';
elencoTipiRubrica = JSON.parse(jsonTipiRubrica);

var jsonTipiCosto = '[{"Udm_Des": "HA","Udm_Cod": 1}, {"Udm_Des": "Ora","Udm_Cod": 2}]';
elencoTipiCosto = JSON.parse(jsonTipiCosto);

var righeInseriteGrid_Rapporti_Contabili = "";
var righeModificateGrid_Rapporti_Contabili = "";
var righeCancellateGrid_Rapporti_Contabili = "";
var righeNonCancellate_Rapporti_Contabili = "";

var righeInseriteGrid_Costi = "";
var righeModificateGrid_Costi = "";
var righeCancellateGrid_Costi = "";
var righeNonCancellate_Costi = "";

var righeInseriteGrid_Rubrica = "";
var righeModificateGrid_Rubrica = "";
var righeCancellateGrid_Rubrica = "";

var righeInseriteGrid_Liquidita = "";
var righeModificateGrid_Liquidita = "";
var righeCancellateGrid_Liquidita = "";
var righeNonCancellate_Liquidita = "";

var righeInseriteGrid_Conti = "";
var righeModificateGrid_Conti = "";
var righeCancellateGrid_Conti = "";
var righeNonCancellate_Conti = "";

var righeInseriteGrid_IndirizzoTipo = "";
var righeModificateGrid_IndirizzoTipo = "";
var righeCancellateGrid_IndirizzoTipo = "";
var righeNonCancellate_IndirizzoTipo = "";

var righeInseriteGrid_Indirizzi = "";
var righeModificateGrid_Indirizzi = "";
var righeCancellateGrid_Indirizzi = "";
var righeNonCancellateGrid_Indirizzi = "";

// Inizializzazione delle seguenti variabili spostate nel document.ready per localizzazione

var elencoOperazioniXNote;

var elencoDocumentiFatturazione;

var elencoModalitaFatturazione;

var elencoTipiAbilitazioneLiquidita;

var elencoTipiIndirizzoStandard;

var elencoItaEste;

var elencoContiEconomici;           //contiene l'elenco di tutti i conti economici

var elencoContiPatrimoniali;        //contiene l'elenco di tutti i conti patrimoniali

var ContoEconMancante = false;      //true quando il conto economico default risulta mancante

var CodContoEcon;                   //contiene il valore attuale della ddl conto economico default

var ContoPatMancante = false;       //true quando il conto patrimoniale default risulta mancante

var CodContoPat;                    //contiene il valore attuale della ddl conto patrimoniale default

var ContoContattoMancante = false;  //true quando un valore dei conti nella griglia di rapporti contabili risulta mancante

var contiAlertString = "Il conto impostato come default non e' presente. E' possibile che sia stato cancellato. Verra' impostato un valore di default vuoto."

var elencoApplicabilitaTipiIndirizzo;

var id_cf_tipi_indirizzo_filtering = -99;


var contattoEditResxArray = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Anagrafica/App_LocalResources/New_Contatto_Edit.aspx.resx"
];

var IMPOSTAZIONE_COD_CALO_PESO_DEFAULT = 1101;
var permessoCaloPesoDefault = false;