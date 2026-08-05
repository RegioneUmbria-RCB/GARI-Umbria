using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
   public class LeggiElencoCompletoProdotti
    {
        public string piva { get; set; }
        public int xSa_Cod { get; set; }
        public int xFabbricato_Cod { get; set; }
        public int xTipoDestinazione { get; set; }
        public int Elem_Cod { get; set; }
        public Boolean soloInGiacenza { get; set; }
        public string FiltroDescrizioneProdotto { get; set; }
        public string Mode { get; set; }
        public string Cau_Mov { get; set; }
        public string Data_Movimento_Str { get; set; }
        public int xPUARegolamento { get; set; }
        public string xLottoAccettazione { get; set; }
        public Boolean leggiUMformulati { get; set; }
        public string metaschema { get; set; }
        public Boolean Flag_QtaNoZero { get; set; }
        public int xTipoPUARegolamento { get; set; }
        public string Elenco_Specie { get; set; }
        public string Elenco_Varieta { get; set; }
        public string xFiltroAggiuntivoMateriePrime { get; set; }
        public Boolean creaGriglia { get; set; }
        public int filtroProdottiValorizzati { get; set; }
        public Boolean flagDiversificaDesFertilizzanti { get; set; }
        public int FiltroCodiceProdotto { get; set; }
        public int FiltroCodiceTrappola { get; set; }

    }
}
