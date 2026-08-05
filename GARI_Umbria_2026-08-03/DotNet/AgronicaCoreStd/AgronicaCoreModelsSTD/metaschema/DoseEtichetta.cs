using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class DoseEtichetta : BaseCodeDescr
    {
        public string CodiceConcatenato { get; set; }
        public string DescrizioneConcatenata { get; set; }
        public decimal DoseMin { get; set; }
        public decimal DoseMax { get; set; }
        public UnitaDiMisura Udm { get; set; }
        public decimal AcquaMin { get; set; }
        public decimal AcquaMax { get; set; }
        public UnitaDiMisura UdmAcqua { get; set; }
        public string Da_Epoca { get; set; }
        public string A_Epoca { get; set; }
        public string Epoca_Des { get; set; } //DT: popolato solo come decodifica, nella Descrizione Concatenata, non nel CodiceConcatenato
        public int Limite { get; set; }
        public UnitaDiMisura UdmLimite { get; set; }
        public string strCLTOSS_Grado { get; set; } //DT: non più usato, va sempre valorizzato come stringa vuota
        public FlagFioritura Flag_Fioritura { get; set; }
        public int IntervalloTrattamenti_Min { get; set; }
        public int IntervalloTrattamenti_Max { get; set; }
        public Mdi Mdi { get; set; }
        public FlagProtetto Flag_Protetto { get; set; }
        public int FormulatiXAllegatiNormative_IDRiga { get; set; }
        public string DataSmaltimentoScorte { get; set; }
        public int Gruppo_Dosaggi { get; set; }
        public int Num_Max_Interventi_Globali { get; set; }
        public DoseEtichetta(int codice) : base(codice, "") { }
        public DoseEtichetta() : base() { }
    }
}

