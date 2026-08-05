/* Lorenzo Lavezzo - 30/09/2024
 * Questa classe nasce solo come mezzo di trasporto per 
 * iniettare all'interno di un servizio, una o più configurazioni prese
 * da file di configurazione
 */

namespace AgronicaDataProvider6.Models
{
    public class SqlSettings
    {
        public bool UseSequence { get; set; } = true;
        public bool UseReadUnCommitted { get; set; } = true;
    }
}
