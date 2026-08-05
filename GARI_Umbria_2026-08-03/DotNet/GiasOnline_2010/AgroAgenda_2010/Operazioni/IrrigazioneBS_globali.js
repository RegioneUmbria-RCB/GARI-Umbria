
const enum_tipoOperazione = {
    Lettura: 0,
    Scrittura: 1,
    Modifica: 2,
    Cancella: 3,
    Duplica: 10
};

const enum_tipoOperazione_Agenda = {
    QuadernoDiCampagna : 1,
    Ricetta : 2,
    RicettaBrogliaccio : 3
};

const enum_tipoSalvataggio = {
    Salva_Esci: 1,
    Salva_Nuovo: 2,
    Salva_Duplica: 3,
    Salva_Costi: 4,
    Salva_Ricetta_NuovoDettaglio: 5
};

const enum_Note_Intervento_Gruppi = {
    //La prima tab delle Note("Giustificazioni"), non ha un enumeratore perchè è l'insieme
    //delle Note_Intervento_Gruppi che hanno codice >0 e che sono visibili
    Giustificazioni: "Giustificazioni",
    Meteo : -1,
    Vento_Intensita : -2,
    Vento_Direzione : -3,
    Temperatura : -4,
    Orario : -5,
    Motivazione : -6,
};

const enum_tipoOperazione_Agenda_Target = {
    Reale : 1,
    Planning : 2
};

const enum_PagineAgenda_2010 = {
    Gis : 59
};

const enum_SiteRedirector = {
    GiasLan : 0
};

var righeTutteGrid_Impianti_Irrigazione = "";

var Elenco_Checkbox_Note = [];

const Empty_obj_DSS_Irrigazione = {
    "ID_DSS_Irrigazione": 0,
    "Descrizione_DSS_Irrigazione": "Nessun Consiglio",
    "Qta_Acqua_DSS_Irrigazione": 0,
    "Udm_Cod_DSS_Irrigazione": 0,
    "Min_Data_Turno": null,
    "Max_Data_Turno": null
};

var Turni_Salvati = [];