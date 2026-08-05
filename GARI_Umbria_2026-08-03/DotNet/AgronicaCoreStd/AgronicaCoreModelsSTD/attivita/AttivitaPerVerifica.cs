using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.attivita
{
    public class AttivitaPerVerifica
    {
        public List<Attivita> AttivitaOggettoDiVerifica { get; set; } = new List<Attivita>();
        public List<Attivita> AttivitaNonOggettoDiVerifica { get; set; } = new List<Attivita>();
    }
}
