using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System.Text;
using AgronicaCoreModelsSTD.utente;
using AgronicaCoreModelsSTD.attivita.risorse;

namespace AgronicaCoreDTOStd.InData.Visite
{

    public class LeggiVisite
    {
        public Utente operatore;
        public Impresa azienda;
        public CentroAziendale centro_aziendale;
        public UtilizzoTerreno specie;
        public DateTime data_da;
        public DateTime data_a;
        public List<String> impianti;
        public List<AttivitaPersonalizzata> operazioni;
        public int tipoVisita;
        public RisorsaZootecnica risorsaZootecnica;
        public bool withDettaglioRilievo; 
    }
}
