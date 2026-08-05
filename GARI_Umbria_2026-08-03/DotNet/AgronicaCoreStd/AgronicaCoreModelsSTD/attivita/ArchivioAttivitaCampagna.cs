using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{
    public class ArchivioAttivitaCampagna
    {
        public List<List<Attivita>> Attivita { get; set; } = new List<List<Attivita>>();
        public List<List<Attivita>> Brogliacci { get; set; } = new List<List<Attivita>>();
        public List<Attivita> AttivitaPianificate { get; set; } = new List<Attivita>();
        public List<int> AttivitaCancellate { get; set; } = new List<int>();
        public List<string> BrogliacciCancellati { get; set; } = new List<string>();
        public List<string> AttivitaPianificateCancellate { get; set; } = new List<string>();
        public DateTime DataUltimaSincro { get; set; } = DateTime.Now;
        public bool LetturaNonEseguita { get; set; } = false;
    }
}
