using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class MUZ
    {
        //Dati Base
        public string Piva { get; set; }
        public int Area_Cod { get; set; }
        public string Area_Des { get; set; }
        public int Tessitura_cod { get; set; }
        public string Altimetria { get; set; }
        public double SO { get; set; }
        public string TipoZona { get; set; }
        public string Geometry { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }

        public enum_Operazioni_MUZ Operazione_Cod { get; set; }

        //Dati correlati
        public int Gruppo_Area_Cod { get; set; }
        public string Gruppo_Area_Des { get; set; }
        public List<ParticelleCatastali_MUZ> Particelle_Catastali_Aggiungi { get; set; }
        public List<ParticelleCatastali_MUZ> Particelle_Catastali_Elimina { get; set; }
        public List<int> Analisi_Testate_Aggiungi { get; set; }
        public List<int> Analisi_Testate_Elimina { get; set; }
        public List<Appezzamento_MUZ> appezzamento { get; set; }

        //Proprietà valorizzate solo in lettura
        public List<ParticelleCatastali_MUZ> Particelle_Catastali { get; set; }
        public List<AnalisiTestate_MUZ> Analisi_Testate { get; set; }
    }

    public class Appezzamento_MUZ
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
    }

    public class ParticelleCatastali_MUZ
    {
        public string Provincia { get; set; }
        public string Comune { get; set; }
        public string Sezione { get; set; }
        public int Foglio { get; set; }
        public int Numero { get; set; }
        public string Subalterno { get; set; }

        //Dati aggiuntivi - Proprietà valorizzate solo in lettura
        public double Sup_Condotta { get; set; }
        public DateTime Condotta_Validita_Inizio { get; set; }
        public DateTime Condotta_Validita_Fine { get; set; }

        public enum_TitoloPossesso_MUZ Titolo_Possesso { get; set; }

        public string Provincia_Esteso { get; set; }
        public string Comune_Esteso { get; set; }
    }

    public class AnalisiTestate_MUZ
    {
        public int Analisi_Testata_Cod { get; set; }
        public string Analisi_Testata_Des { get; set; }
    }

    public enum enum_TitoloPossesso_MUZ {
        Altro = 0,
        Proprieta = 1,
        Comodato = 2,
        AffittoContratto = 3,
        AffittoSenzaContratto = 4,
        InContoTerzi = 5,
        InConvenzione = 6,
        InCompartecipazione = 7
    }

    public enum enum_Operazioni_MUZ
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3
    }
}
