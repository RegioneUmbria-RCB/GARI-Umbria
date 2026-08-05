using System;

namespace InData.Agenda
{
    public class LeggiAgenda
    {
        public string Piva { get; set; }
        public DateTime DataRiferimento { get; set; }
        public DateTime DataUltimaSincro { get; set; }
        public bool SoloImpiantiAttivi { get; set; }
        public bool LeggiSoloBrogliacci { get; set; }
        public LeggiAgenda(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, bool soloImpiantiAttivi, bool leggiSoloBrogliacci)
        {
            Piva = piva;
            DataRiferimento = dataRiferimento;
            DataUltimaSincro = dataUltimaSincro;
            SoloImpiantiAttivi = soloImpiantiAttivi;
            LeggiSoloBrogliacci = leggiSoloBrogliacci;
        }
    }
}
