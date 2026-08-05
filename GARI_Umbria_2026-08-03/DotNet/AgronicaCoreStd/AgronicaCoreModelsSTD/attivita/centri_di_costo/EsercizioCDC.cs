using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.documenti;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{

    /// <summary>
    /// Corrisponde ad un record nella tabella Mov_Destinazioni
    /// </summary>
    public class EsercizioCDC: CentroDiCosto
    {

        public Esercizio esercizio { get; set; }

        /// <summary>
        /// superficie interessata (può essere di meno rispetto a quella di anagrafica) - Mov_Destinazioni.QTA2
        /// </summary>
        public decimal superficieTrattata { get; set; }

        /// <summary>
        /// La superficie NON trattata (Sup. dell'esercizio - superficie trattata) Mov_Destinazioni.Sup_Riduzione_BufferZone
        /// </summary>
        public decimal superficieRiduzioneBufferZone { get; set; }

        /// <summary>
        /// percentuale di riduzione della deriva degli ugelli dell'atomizzatore - Mov_Destinazioni.Perc_Riduzione_Deriva
        /// </summary>
        public decimal percentualeRiduzioneDeriva { get; set; }

        /// <summary>
        /// scarico di prodotti; la quantità distribuita è: superficie * Dose_Ha_Reale (tabella Mov_Destinazioni.QTA) 'TODO: FEDE VERIFICA
        /// </summary>
        public decimal quantita { get; set; }

        public EsercizioCDC()
        {
            classType = costanti.ClassType.EsercizioCDC;
            tipo = Tipo.Esercizio;
        }

        public Documento documento { get; set; }

    }
}
