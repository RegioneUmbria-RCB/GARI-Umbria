import { enum_SEMINA_TIPO } from "app/Model/TipiEnumerativi";

export var Elenco_Opzioni_Semina= [
    {
        codice: enum_SEMINA_TIPO.Solo_Semina_Default,
        descrizione: "Registra Solo Semina/Trapianto"
    },
    {
        codice: enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default,
        descrizione: "Registra Semina/Trapianto ed Aggiorna l'anagrafica dell'Appezzamento/Impianto (Specie, Varietà e Integrato/BIO)"
    },
    {
        codice: enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default,
        descrizione: "Fraziona gli Appezzamenti in base ai Lotti delle Materie Prime e Registra le Semine/Trapianti sui nuovi Appezzamenti/Impianti"
    }
];
