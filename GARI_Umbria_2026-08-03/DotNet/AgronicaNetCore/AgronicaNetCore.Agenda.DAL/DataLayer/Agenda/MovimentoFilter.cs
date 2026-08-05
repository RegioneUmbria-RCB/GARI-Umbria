namespace AgronicaNetCore.Agenda.DAL.DataLayer.Agenda
{
    /// <summary>
    /// Represents a simple movement filter for warehouse queries
    /// </summary>
    public class MovimentoFilter
    {
        public string ChiaveMagazzino { get; set; }
        public int CategoriaProdotto { get; set; }
        public int Pro_Cod { get; set; }
        public int Mat_Cod { get; set; }
        public string Lotto { get; set; }
        public int UnitaDiMisura { get; set; }

        public MovimentoFilter(string chiaveMagazzino, int categoriaProdotto, int proCod, int matCod, string lotto, int unitaDiMisura)
        {
            ChiaveMagazzino = chiaveMagazzino;
            CategoriaProdotto = categoriaProdotto;
            Pro_Cod = proCod;
            Mat_Cod = matCod;
            Lotto = lotto;
            UnitaDiMisura = unitaDiMisura;
        }
    }
}
