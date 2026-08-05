using System;

namespace AgronicaCoreAPI.models.entities
{
    public class GerarchiaImpreseTipologiaEntity
    {
        public string Piva;
        public int TipologiaGerarchie_Cod;
        public string TipologiaGerarchie_Des;
        public int TipologiaGerarchie_Dettagli_Livello_Cod;
        public string TipologiaGerarchie_Dettagli_Livello_Des;
        public int TipologiaGerarchie_Dettagli_Livello;
        public int Gruppi_Utente_Cod;
        public int Flag_Amministrazione;
        public int Flag_Inserimento;
        public int Flag_Modifica;
        public int Flag_Cancellazione;
        public int Flag_Informazioni;
    }
}
