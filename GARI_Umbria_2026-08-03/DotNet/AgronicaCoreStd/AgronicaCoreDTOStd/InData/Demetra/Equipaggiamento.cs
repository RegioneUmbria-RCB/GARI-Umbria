using AgronicaCoreDTOStd.InData.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Equipaggiamento
    {
        public string descrizione;
        public string tipo;
        public string modello;
        public string targa;
        public string alimentazione;
        public DateTime? data_ultima_taratura;
        public string telaio;
        public DateTime? scadenza_taratura;
        public DateTime? ultimo_aggiornamento;
        public string utente_ultima_modifica;
        public Validita validita;
        public bool flag_cancellazione;
        public string nr_certificato;
    }
}
