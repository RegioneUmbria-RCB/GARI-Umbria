using AgronicaCoreModelsSTD.anagrafiche;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public enum enum_TipoGriglia
    {
        None = 0,
        Protocolli = 1,              // griglia Protocolli
        ProtocolliInCorso = 2,       // griglia Protocolli In Corso
        PrescrizioniVeterinarie = 3, // griglia Prescrizioni Veterinarie e Indicazioni Terapeutiche
    }

    enum enum_TipoPrescrizione
    {
        UNDEFINED = 0,
        Veterinaria = 1,
        Protocollo_Terapeutico = 2,
        Da_Protocollo = 3,
        Indicazione_Terapeutica = 4,
        Rifornimento_Scorta = 5,
        Da_Protocollo_GIAS = 6,
    }

    public class LeggiPrescrizioni
    {
        public int? Ricetta_Cod { get; set; }

        public string Piva { get; set; }
        
        public int? Sa_Cod { get; set; }

        public int? Sta_Num { get; set; }

        /// <summary>
        /// enum_TipoLetturaGriglia
        /// </summary>
        public int? Tipo_Griglia { get; set; }

        /// <summary>
        /// enum_TipoPrescrizione
        /// </summary>
        public int? Tipo_Cod { get; set; }

        public IntervalloTemporale validita { get; set; }

        public LeggiPrescrizioni()
        {
            Piva = "";
            Sa_Cod = 0;
            Sta_Num = 0;
            validita = new IntervalloTemporale();
        }

        public LeggiPrescrizioni(int ricettaCod) => Ricetta_Cod = ricettaCod;

        public LeggiPrescrizioni(int ricettaCod, int tipoCod)
        {
            Ricetta_Cod = ricettaCod;
            Tipo_Cod = tipoCod;
        }

        public LeggiPrescrizioni(int ricettaCod, int tipo, bool isGrid)
        {
            Ricetta_Cod = ricettaCod;
            if (isGrid)
                Tipo_Griglia = tipo;
            else
                Tipo_Cod = tipo;
        }

        public bool ReadingProtocolli()
        {
            return 
                (Tipo_Cod.HasValue &&
                    (new List<int> { (int)enum_TipoPrescrizione.Protocollo_Terapeutico, (int)enum_TipoPrescrizione.Da_Protocollo_GIAS }).Contains(Tipo_Cod.Value)) ||
                (Tipo_Griglia.HasValue &&
                    (new List<int> { (int)enum_TipoGriglia.Protocolli, (int)enum_TipoGriglia.ProtocolliInCorso }).Contains(Tipo_Griglia.Value));
            ;
        }

        public bool ReadingProtocolliInCorso()
        {
            return (Tipo_Griglia.HasValue && Tipo_Griglia.Value == (int)enum_TipoGriglia.ProtocolliInCorso) ||
                (Tipo_Cod.HasValue && Tipo_Cod.Value == (int)enum_TipoPrescrizione.Da_Protocollo_GIAS);
        }
        
        public bool ReadingPresVetIndTerap()
        {
            return (Tipo_Griglia.HasValue && Tipo_Griglia.Value == (int)enum_TipoGriglia.PrescrizioniVeterinarie) ||
                Tipo_Cod.HasValue &&
                    (new List<int> { (int)enum_TipoPrescrizione.Veterinaria, (int)enum_TipoPrescrizione.Indicazione_Terapeutica }).Contains(Tipo_Cod.Value);
        }

    }
}
