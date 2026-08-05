using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita.Info
{
    public class InfoOperazione
    {
        #region Properties
        public bool IsTrattamento = false;
        public bool IsFertilizzazione = false;
        public bool IsLavorazione = false;
        public bool IsRaccolta = false;
        public bool IsSemina = false;
        public bool IsVisita = false;
        public bool isNonUtilizzo = false;
        public bool IsAbbattimento = false;
        public bool IsRilievo = false;
        public bool IsZoo = false;
        public bool IsIrrigazione = false;
        public bool IsFertirrigazione = false;

        public bool IsCarico = false;
        public bool IsScarico = false;
        public bool IsRegistrazione = false;

        public string Cau_Mov = "";
        public int Elem_Cod = 0;

        public int BaseCode = 0;
        public int TopCode = 2000000000;

        public string classType = "";

        public Attivita.Tipo_Attivita TipoOperazioneAgenda = 0;

        public centri_di_costo.Tipo TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio;

        public bool controlloPolverulenti = false;
        /// <summary>
        /// Apertura operazioni create con il vecchio che potrebbero avere 
        /// N avversità salvate: in questi casi prendiamo solo la prima
        /// </summary>
        public bool PregressoConMultiAvversita = false;
        #endregion

        public InfoOperazione() { }

    }
}
