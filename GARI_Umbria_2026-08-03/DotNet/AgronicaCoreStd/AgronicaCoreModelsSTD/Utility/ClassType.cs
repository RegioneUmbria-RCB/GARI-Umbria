using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.note_intervento;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.contabilita;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.costanti
{        /*ClassType AgronicaCoreModelsSTD*/
    public class ClassType
    {
        //--ATTIVITA'
        public const String EsercizioCDC = "EsercizioCDC";
        public const String EsercizioRilievoCDC = "EsercizioRilievoCDC";
        public const String Macchina = "Macchina";
        public const String Progetto = "Progetto";
        public const String CapoAnimaleCDC = "CapoAnimaleCDC";
        public const String ProdottoDaTrattareCDC = "ProdottoDaTrattareCDC";

        public const String DettaglioFertilizzazione = "DettaglioFertilizzazione";
        public const String DettaglioRaccolta = "DettaglioRaccolta";
        public const String DettaglioRilievo = "DettaglioRilievo";
        public const String DettaglioIrrigazione = "DettaglioIrrigazione";
        public const String DettaglioSemina = "DettaglioSemina";
        public const String DettaglioTrattamento = "DettaglioTrattamento";
        public const String DettaglioRegistroSomministrazioni = "DettaglioRegistroSomministrazioni";

        public const String RisorsaAcqua = "RisorsaAcqua";
        public const String RisorsaMacchina = "RisorsaMacchina";
        public const String RisorsaPersona = "RisorsaPersona";
        public const String RisorsaProdotto = "RisorsaProdotto";
        public const String RisorsaRegistrazione = "RisorsaRegistrazione";
        public const String RisorsaSpecie = "RisorsaSpecie";
        public const String RisorsaDestinazioneUso = "RisorsaDestinazioneUso";
        public const String RisorsaAssegnatarioVisita = "RisorsaAssegnatarioVisita";
        public const String RisorsaZootecnica = "RisorsaZootecnica";
        public const String RisorsaCausale = "RisorsaCausale";

        //--METASCHEMA
        public const String Avversita = "Avversita";
        public const String GruppoAvversita = "GruppoAvversita";
        public const String DestinazioneUso = "DestinazioneUso";
        public const String Varieta = "Varieta";
        public const String Specie = "Specie";

        public const String Lavorazione = "Lavorazione";
        public const String AttivitaCDG = "AttivitaCDG";
        public const String JobComposito = "JobComposito";
        public const String Registrazione = "Registrazione";
        public const String Zootecnia = "Zootecnia";

    }
}