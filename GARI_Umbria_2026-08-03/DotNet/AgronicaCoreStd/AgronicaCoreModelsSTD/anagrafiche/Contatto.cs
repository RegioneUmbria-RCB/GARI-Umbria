using AgronicaCoreModelsSTD.documenti;
using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.utente;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class Contatto
    {

        public string tipo { get; set; }
        public bool fittizio { get; set; }

        public int fisico_Giuridico { get; set; }

        public bool estero { get; set; }

        public bool visibilitaPubblica { get; set; }

        public string ragione_Sociale { get; set; }

        public int sa_cod { get; set; }

        public string cognome { get; set; }
        public string nome { get; set; }
        public string codiceFiscale { get; set; }

        public string nome_Breve { get; set; }
        public DateTime data_Nascita { get; set; }
        public string sesso { get; set; }
        public string convenevoli { get; set; }

        public string badge { get; set; }

        public List<IndirizzoAssociato> indirizzi { get; set; }

        public List<RubricaVoci> rubricaVoci { get; set; }

        public Utente utente { get; set; }

        public List<RisorseUmane> risorseUmane { get; set; }

        public List<Documento> documenti { get; set; }
        public string aziendaProprietaria { get; set; }

        /// Fattura Elettronica
        public string fe_Tipologia_Contatto { get; set; }
        public string fe_Rappresentante_Fiscale { get; set; }
        public string fe_Pec { get; set; }
        public string fe_SDI { get; set; }

        public PK primaryKey { get; set; }

        public bool flag_cancellazione { get; set; }

        public Contatto()
        {
            flag_cancellazione = false;
        }

        public class PK
        {

            public string partitaIva { get; set; }

            /// <summary>
            /// Cod_contatto su db
            /// </summary>
            public string codice { get; set; }

            public PK(string partitaIva, string codice)
            {
                this.partitaIva = partitaIva;
                this.codice = codice;
            }

            public PK()
            {
            }
        }

    }
}
