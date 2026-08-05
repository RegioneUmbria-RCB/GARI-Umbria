namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    public class ProdottoDaTrattareCDC : CentroDiCosto
    {
        public MovimentoDiMagazzino giacenzaMagazzino {   get; set; }

        /// <summary>
        /// quantità prodotto interessata (può essere di meno rispetto a quella dello scarico) - Mov_Dettagli.QTA
        /// </summary>
        public decimal qtaTrattata { get; set; }

        public ProdottoDaTrattareCDC()
        {
            classType = costanti.ClassType.ProdottoDaTrattareCDC;
            tipo = Tipo.ProdottoDaTrattare;
        }
    }
}
